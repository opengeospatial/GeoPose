/*
 * Copyright (c) 2023 The Dani Elenga Foundation

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

// Implemention order: 4 - follows FrameTransform.
// These classes define rotations of a 3D frame transforming a Position to a rotated Position.

using System;

using Positions;

/// <summary>
/// The abstract root of the Orientation hierarchy.
/// <note>
/// An Orientation is a generic container for information that defines rotation within a coordinate system associated with a reference frame.
/// An Orientation may have a specialized context with necessary ancillary information
/// that parameterizes the rotation.
/// Such context may include, for example, part of the information that may be conveyed in an ISO 19111 CRS specification
/// or a proprietary naming, numbering, or modelling scheme as used by EPSG, NASA Spice, or SEDRIS SRM.
/// Subclasses of Orientation exist precisely to hold this context in conjunction with code
/// implementing a Rotate function.
/// </note>
/// </summary>

namespace Orientations
{
    /// <summary>
    /// The abstract root of the Orientation hierarchy.
    /// <note>
    /// An Orientation is a generic container for information that defines rotation within a coordinate system associated with a reference frame.
    /// An Orientation may have a specialized context with necessary ancillary information
    /// that parameterizes the rotation.
    /// Such context may include, for example, part of the information that may be conveyed in an ISO 19111 CRS specification
    /// or a proprietary naming, numbering, or modelling scheme as used by EPSG, NASA Spice, or SEDRIS SRM.
    /// Subclasses of Orientation exist precisely to hold this context in conjunction with code
    /// implementing a Rotate function.
    /// </note>
    /// </summary>
    public abstract class Orientation
    {
        public virtual Position Rotate(Position point)
        {
            return point;
        }
    }
    /// <summary>
    /// A specialization of Orientation using Yaw, Pitch, and Roll angles in degrees.
    /// <remark>
    /// This style of Orientation is best for easy human interpretation.
    /// It suffers from some computational inefficiencies, awkward interpolation, and singularities.
    /// </remark>
    /// </summary>
    public class YPRAngles : Orientation
    {
        // Prevent public use of empty constructor.
        internal YPRAngles()
        {

        }
        public YPRAngles(double yaw, double pitch, double roll)
        {
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
        }
        public override Position Rotate(Position point)
        {
            // convert to quaternion and use quaternion rotation
            Quaternion q = YPRAngles.ToQuaternion(this.yaw, this.pitch, this.roll);
            return Quaternion.Transform(point, q);
        }
        public static Quaternion ToQuaternion(double yaw, double pitch, double roll)
        {
            // GeoPose uses angles in degrees for human readability
            // Convert degrees to radians.
            yaw *= (Math.PI / 180.0);
            pitch *= (Math.PI / 180.0);
            roll *= (Math.PI / 180.0);

            double cosRoll = Math.Cos(roll * 0.5);
            double sinRoll = Math.Sin(roll * 0.5);
            double cosPitch = Math.Cos(pitch * 0.5);
            double sinPitch = Math.Sin(pitch * 0.5);
            double cosYaw = Math.Cos(yaw * 0.5);
            double sinYaw = Math.Sin(yaw * 0.5);

            double w = cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw;
            double x = sinRoll * cosPitch * cosYaw - cosRoll * sinPitch * sinYaw;
            double y = cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw;
            double z = cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw;

            double norm = Math.Sqrt(x * x + y * y + z * z + w * w);
            if (norm <= double.MinValue)
            {
                return new Quaternion(x, y, z, w);
            }
            return new Quaternion(x / norm, y / norm, z / norm, w / norm);
        }
        /// <summary>
        /// A left-right angle in degrees.
        /// </summary>
        public double yaw { get; set; } = double.NaN;
        /// <summary>
        /// An up-down angle in degrees.
        /// </summary>
        public double pitch { get; set; } = double.NaN;
        /// <summary>
        /// A side-to-side angle in degrees.
        /// </summary>
        public double roll { get; set; } = double.NaN;
    }
    /// <summary>
    /// A specialization of Orientation using a unit quaternion.
    /// </summary>
    /// <remark>
    /// This style of Orientation is best for computation.
    /// It is not easily interpreted or visualized by humans.
    /// </remark>
    public class Quaternion : Orientation
    {
        internal Quaternion()
        {

        }
        public Quaternion(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public override Position Rotate(Position point)
        {
            return Quaternion.Transform(point, this);
        }
        public YPRAngles ToYPRAngles(Quaternion q)
        {
            YPRAngles yprAngles = new YPRAngles();

            // roll (x-axis rotation)
            double sinRollCosPitch = 2.0 * (q.w * q.x + q.y * q.z);
            double cosRollCosPitch = 1.0 - 2.0 * (q.x * q.x + q.y * q.y);
            yprAngles.roll = Math.Atan2(sinRollCosPitch, cosRollCosPitch) * (180.0 / Math.PI); // in degrees

            // pitch (y-axis rotation)
            double sinPitch = Math.Sqrt(1.0 + 2.0 * (q.w * q.y - q.x * q.z));
            double cosPitch = Math.Sqrt(1.0 - 2.0 * (q.w * q.y - q.x * q.z));
            yprAngles.pitch = (2.0 * Math.Atan2(sinPitch, cosPitch) - Math.PI / 2.0) * (180.0 / Math.PI); // in degrees

            // yaw (z-axis rotation)
            double sinYawCosPitch = 2.0 * (q.w * q.z + q.x * q.y);
            double cosYawCosPitch = 1.0 - 2.0 * (q.y * q.y + q.z * q.z);
            yprAngles.yaw = Math.Atan2(sinYawCosPitch, cosYawCosPitch) * (180.0 / Math.PI); // in degrees

            return yprAngles;
        }
        public static Position Transform(Position inPoint, Quaternion rotation)
        {
            CartesianPosition point = (CartesianPosition)inPoint;
            double x2 = rotation.x + rotation.x;
            double y2 = rotation.y + rotation.y;
            double z2 = rotation.z + rotation.z;

            double wx2 = rotation.w * x2;
            double wy2 = rotation.w * y2;
            double wz2 = rotation.w * z2;
            double xx2 = rotation.x * x2;
            double xy2 = rotation.x * y2;
            double xz2 = rotation.x * z2;
            double yy2 = rotation.y * y2;
            double yz2 = rotation.y * z2;
            double zz2 = rotation.z * z2;

            return new CartesianPosition(
                point.x * (1.0f - yy2 - zz2) + point.y * (xy2 - wz2) + point.z * (xz2 + wy2),
                point.x * (xy2 + wz2) + point.y * (1.0f - xx2 - zz2) + point.z * (yz2 - wx2),
                point.x * (xz2 - wy2) + point.y * (yz2 + wx2) + point.z * (1.0f - xx2 - yy2));
        }
        /// <summary>
        /// The x component.
        /// </summary>
        public double x { get; set; } = double.NaN;
        /// <summary>
        /// The y component.
        /// </summary>
        public double y { get; set; } = double.NaN;
        /// <summary>
        /// The z component.
        /// </summary>
        public double z { get; set; } = double.NaN;
        /// <summary>
        /// The w component.
        /// </summary>
        public double w { get; set; } = double.NaN;
    }
}

