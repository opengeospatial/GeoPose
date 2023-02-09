// * Copyright(c) 2023 The Dani Elenga Foundation
// *
// * Permission is hereby granted, free of charge, to any person obtaining a copy
// * of this software and associated documentation files(the "Software"), to deal
// *     in the Software without restriction, including without limitation the rights
// * to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// * copies of the Software, and to permit persons to whom the Software is
// * furnished to do so, subject to the following conditions:
// *
// * The above copyright notice and this permission notice shall be included in all
// * copies or substantial portions of the Software.
// *
// * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// *     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// *     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// * SOFTWARE.
// *

import * as Position from './Position';

// Implemention order: 4 - follows FrameTransform.
// These classes define rotations of a 3D frame transforming a Position to a rotated Position.


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
export abstract class Orientation {
    abstract Rotate(point: Position.CartesianPosition): Position.Position;
}

/// <summary>
/// A specialization of Orientation using Yaw, Pitch, and Roll angles measured in degrees.
/// <remark>
/// This style of Orientation is best for easy human interpretation.
/// It suffers from some computational inefficiencies, awkward interpolation, and singularities.
/// </remark>
/// </summary>
export class YPRAngles extends Orientation {
    public constructor(yaw: number, pitch: number, roll: number) {
        super();
        this.yaw = yaw;
        this.pitch = pitch;
        this.roll = roll;
    }

    /// <summary>
    /// The function is to apply a YPR transformation
    /// </summary>
    public override Rotate(point: Position.CartesianPosition): Position.Position {
        // convert to quaternion and use quaternion rotation
        let q = YPRAngles.ToQuaternion(this.yaw, this.pitch, this.roll);
        return Quaternion.Transform(point, q);
    }
    public static ToQuaternion(yaw: number, pitch: number, roll: number): Quaternion {
        // GeoPose angles are measured in degrees for human readability
        // Convert degrees to radians.
        yaw *= (Math.PI / 180.0);
        pitch *= (Math.PI / 180.0);
        roll *= (Math.PI / 180.0);

        let cosRoll = Math.cos(roll * 0.5);
        let sinRoll = Math.sin(roll * 0.5);
        let cosPitch = Math.cos(pitch * 0.5);
        let sinPitch = Math.sin(pitch * 0.5);
        let cosYaw = Math.cos(yaw * 0.5);
        let sinYaw = Math.sin(yaw * 0.5);

        let w = cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw;
        let x = sinRoll * cosPitch * cosYaw - cosRoll * sinPitch * sinYaw;
        let y = cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw;
        let z = cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw;

        let norm = Math.sqrt(x * x + y * y + z * z + w * w);
        let q = new Quaternion(x, y, z, w);
        if (norm > 0.0) {
            q.x = q.x / norm;
            q.y = q.y / norm;
            q.z = q.z / norm;
            q.w = q.w / norm;
        }
        return q;
    }
    /// <summary>
    /// A left-right angle in degrees.
    /// </summary>
    public yaw: number;
    /// <summary>
    /// A forward-looking up-down angle in degrees.
    /// </summary>
    public pitch: number;
    /// <summary>
    /// A side-to-side angle in degrees.
    /// </summary>
    public roll: number;
}
/// <summary>
/// Quaternion is a specialization of Orientation using a unit quaternion.
/// </summary>
/// <remark>
/// This style of Orientation is best for computation.
/// It is not easily interpreted or visualized by humans.
/// </remark>
export class Quaternion extends Orientation {
    public constructor(x: number, y: number, z: number, w: number) {
        super();
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    public override Rotate(point: Position.CartesianPosition): Position.Position {
        return Quaternion.Transform(point, this);
    }
    public ToYPRAngles(q: Quaternion): YPRAngles {

        // roll (x-axis rotation)
        let sinRollCosPitch = 2.0 * (q.w * q.x + q.y * q.z);
        let cosRollCosPitch = 1.0 - 2.0 * (q.x * q.x + q.y * q.y);
        let roll = Math.atan2(sinRollCosPitch, cosRollCosPitch) * (180.0 / Math.PI); // in degrees

        // pitch (y-axis rotation)
        let sinPitch = Math.sqrt(1.0 + 2.0 * (q.w * q.y - q.x * q.z));
        let cosPitch = Math.sqrt(1.0 - 2.0 * (q.w * q.y - q.x * q.z));
        let pitch = (2.0 * Math.atan2(sinPitch, cosPitch) - Math.PI / 2.0) * (180.0 / Math.PI); // in degrees

        // yaw (z-axis rotation)
        let sinYawCosPitch = 2.0 * (q.w * q.z + q.x * q.y);
        let cosYawCosPitch = 1.0 - 2.0 * (q.y * q.y + q.z * q.z);
        let yaw = Math.atan2(sinYawCosPitch, cosYawCosPitch) * (180.0 / Math.PI); // in degrees
        let yprAngles = new YPRAngles(yaw, pitch, roll);
        return yprAngles;
    }
    public static Transform(inPoint: Position.CartesianPosition, rotation: Quaternion): Position.CartesianPosition {
        let point = new Position.CartesianPosition(inPoint.x, inPoint.y, inPoint.z);
        let x2 = rotation.x + rotation.x;
        let y2 = rotation.y + rotation.y;
        let z2 = rotation.z + rotation.z;

        let wx2 = rotation.w * x2;
        let wy2 = rotation.w * y2;
        let wz2 = rotation.w * z2;
        let xx2 = rotation.x * x2;
        let xy2 = rotation.x * y2;
        let xz2 = rotation.x * z2;
        let yy2 = rotation.y * y2;
        let yz2 = rotation.y * z2;
        let zz2 = rotation.z * z2;

        let p = new Position.CartesianPosition(
            point.x * (1.0 - yy2 - zz2) + point.y * (xy2 - wz2) + point.z * (xz2 + wy2),
            point.x * (xy2 + wz2) + point.y * (1.0 - xx2 - zz2) + point.z * (yz2 - wx2),
            point.x * (xz2 - wy2) + point.y * (yz2 + wx2) + point.z * (1.0 - xx2 - yy2));
        return p;
    }
    /// <summary>
    /// The x component.
    /// </summary>
    public x: number;
    /// <summary>
    /// The y component.
    /// </summary>
    public y: number;
    /// <summary>
    /// The z component.
    /// </summary>
    public z: number;
    /// <summary>
    /// The w component.
    /// </summary>
    public w: number;
}

