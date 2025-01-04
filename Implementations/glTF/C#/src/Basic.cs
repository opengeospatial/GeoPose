Copyright (c) 2023 The Dani Elenga Foundation

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

// Implemention order: 6 - follows GeoPose.
// This is the simplest family of GeoPoses - the 80% part of a 80/20 solution.

using System;
using System.Text;

using GeoPose;
using Positions;
using FrameTransforms;
using Orientations;
using Extras;

namespace Basic
{
    public abstract class Basic : GeoPose.GeoPose
    {
        /// <summary>
        /// A Position specified in spherical coordinates with height above a reference surface -
        /// usually an ellipsoid of revolution or a gravitational equipotential surface.
        /// </summary>
        private WGS84ToLTP_ENU _frameTransform = new WGS84ToLTP_ENU();
        public override FrameTransform FrameTransform
        {
            get
            {
                return _frameTransform;
            }
            set
            {
                if (value.GetType() == typeof(WGS84ToLTP_ENU))
                {
                    _frameTransform = (WGS84ToLTP_ENU)value;
                }
            }
        }
    }

    /// <summary>
    /// A Basic-YPR GeoPose.
    /// <remark>
    /// See the OGS GeoPose 1.0 standard for a full description.
    /// </remark>
    /// </summary>
    public class BasicYPR : Basic
    {
        internal BasicYPR()
        {

        }
        public BasicYPR(string id, GeodeticPosition tangentPoint, YPRAngles yprAngles)
        {
            this.poseID = new PoseID(id);
            this.FrameTransform = new WGS84ToLTP_ENU(tangentPoint);
            this.Orientation = yprAngles;
        }
        private YPRAngles _orientation = new YPRAngles();
        /// <summary>
        /// An Orientation specified as three rotations.
        /// </summary>
        public override Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (value.GetType() == typeof(YPRAngles))
                {
                    _orientation = (YPRAngles)value;
                }
            }
        }
        /// <summary>
        /// Return a string containing Json encoding of a Basic-YPR GeoPose
        /// </summary>
        public override string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            if (FrameTransform != null && Orientation != null)
            {
                sb.Append("{\r\n\t\t" + indent);
                if (validTime != null && validTime.timeValue != String.Empty)
                {
                    sb.Append("\"validTime\": " + validTime.timeValue + ",\r\n" + indent + "  ");
                }
                if (poseID != null && poseID.id != String.Empty)
                {
                    sb.Append("\"poseID\": \"" + poseID.id + "\",\r\n" + indent + "  ");
                }
                if (parentPoseID != null && parentPoseID.id != String.Empty)
                {
                    sb.Append("\"parentPoseID\": \"" + parentPoseID.id + "\",\r\n" + indent + "  ");
                }
                sb.Append("\"position\": {\r\n\t\t\t" + indent + "\"lat\": " + ((WGS84ToLTP_ENU)FrameTransform).Origin.lat + ",\r\n\t\t\t" + indent +
                    "\"lon\": " + ((WGS84ToLTP_ENU)FrameTransform).Origin.lon + ",\r\n\t\t\t" + indent +
                    "\"h\":   " + ((WGS84ToLTP_ENU)FrameTransform).Origin.h);
                sb.Append("\r\n\t\t" + indent + "},");
                sb.Append("\r\n\t\t" + indent);
                sb.Append("\"angles\": {\r\n\t\t\t" + indent + "\"yaw\":   " + ((YPRAngles)Orientation).yaw + ",\r\n\t\t\t" + indent +
                    "\"pitch\": " + ((YPRAngles)Orientation).pitch + ",\r\n\t\t\t" + indent +
                    "\"roll\":  " + ((YPRAngles)Orientation).roll);
                sb.Append("\r\n\t\t" + indent + "}");
                sb.Append("\r\n\t" + indent + "}");
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// A Basic-Quaternion GeoPose.
    /// <remark>
    /// See the OGS GeoPose 1.0 standard for a full description.
    /// </remark>
    /// </summary>
    public class BasicQuaternion : Basic
    {
        internal BasicQuaternion()
        {
        }
        public BasicQuaternion(string id, GeodeticPosition tangentPoint, Quaternion quaternion)
        {
            this.poseID = new PoseID(id);
            this.FrameTransform = new WGS84ToLTP_ENU(tangentPoint);
            this.Orientation = quaternion;
        }

        /// <summary>
        /// An Orientation specified as a unit quaternion.
        /// </summary>
        private Quaternion _orientation = new Quaternion();
        public override Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (value.GetType() == typeof(Quaternion))
                {
                    _orientation = (Quaternion)value;
                }
            }
        }

        /// <summary>
        /// This function returns a Json encoding of a Basic-Quaternion GeoPose
        /// </summary>
        public override string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            if (((WGS84ToLTP_ENU)FrameTransform).Origin != null && Orientation != null)
            {
                sb.Append("{\r\n\t\t" + indent);
                if (validTime != null && validTime.timeValue != String.Empty)
                {
                    sb.Append("\"validTime\": " + validTime.timeValue + ",\r\n" + indent + "  ");
                }
                if (poseID != null && poseID.id != String.Empty)
                {
                    sb.Append("\"poseID\": \"" + poseID.id + "\",\r\n" + indent + "  ");
                }
                if (parentPoseID != null && parentPoseID.id != String.Empty)
                {
                    sb.Append("\"parentPoseID\": \"" + parentPoseID.id + "\",\r\n" + indent + "  ");
                }
                sb.Append("\"position\": {\r\n\t\t\t" + indent + "\"lat\": " +
                    ((WGS84ToLTP_ENU)FrameTransform).Origin.lat + ",\r\n\t\t\t" + indent +
                    "\"lon\": " + ((WGS84ToLTP_ENU)FrameTransform).Origin.lon +
                    ",\r\n\t\t\t" + indent +
                    "\"h\":   " + ((WGS84ToLTP_ENU)FrameTransform).Origin.h);
                sb.Append("\r\n\t\t" + indent + "},");
                sb.Append("\r\n\t\t" + indent);
                sb.Append("\"angles\": {\r\n\t\t\t" + indent + "\"x\":   " + ((Quaternion)Orientation).x + ",\r\n\t\t\t" + indent +
                    "\"y\": " + ((Quaternion)Orientation).y + ",\r\n\t\t\t" + indent +
                    "\"z\": " + ((Quaternion)Orientation).z + ",\r\n\t\t\t" + indent +
                    "\"w\":  " + ((Quaternion)Orientation).w);
                sb.Append("\r\n\t\t" + indent + "}");
                sb.Append("\r\n\t" + indent + "}");
            }
            return sb.ToString();
        }
    }
}
