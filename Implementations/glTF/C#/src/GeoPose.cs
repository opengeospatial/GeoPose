//    Copyright (c) 2020-2023, The DaniElenga Foundation.
//    Licensed under the MIT License (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at http://www.opensource.org/licenses/mit-license.php
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// <author>Steve Smyth</author>
// <website>https://opensiteplan.org</website>
//-----------------------------------------------------------------------

/// <summary>
/// This library is a C# implementation of the three OGC GeoPose 1.0 Basic and Advanced targets
/// with Json serialization of each, conforming to the OGC GeoPose 1.0 standard.
/// <version>
/// Version 0.9.11 
/// </version>
/// <date>
/// 13 December 2022
/// </date>
/// <note>
/// This library was created as a part of the CitySharp encoding of OGC CityGML 3.0.
/// It is used in the prototype extension of Khronos glTF 2.0 to encode the OGC CityGML 3.0 semantic model.
/// </note>
/// <note>
/// This C# 10 version targets the open source and platform-independent .NET 6 Core framework.
/// </note>
/// </summary>

using System;
using System.Text;

namespace GeoPose
{
    // *******************************************************************************

    /// <summary>
    /// The abstract root of the GeoPose Basic and Advanced classes.
    /// </summary>
    public abstract class GeoPose
    {
        public Position? Position { get; set; } = null;
        public Orientation? Orientation { get; set; } = null;
    }
    /// <summary>
    /// The abstract root of the Position hierarchy.
    /// <note>
    /// Because the various ways to express Position share no underlying structure,
    /// the class definition is simply an empty shell.
    /// </note>
    /// </summary>

    /// <summary>
    /// A Basic-YPR GeoPose.
    /// <remark>
    /// See the OGS GeoPose 1.0 standard for a full description.
    /// </remark>
    /// </summary>
    public class BasicYPR : GeoPose
    {
        /// <summary>
        /// A Position specified in spherical coordinates with height above a reference surface -
        /// usually an ellipsoid of revolution or a gravitational equipotential surface.
        /// </summary>
        public new GeodeticPosition Position { get; set; } = new GeodeticPosition();
        /// <summary>
        /// An Orientation specified as three rotations.
        /// </summary>
        public new YPRAngles Orientation { get; set; } = new YPRAngles();
        /// <summary>
        /// This function returns a Json encoding of a Basic-YPR GeoPose
        /// </summary>
        public string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            if (Position != null && Orientation != null)
            {
                sb.Append("{\r\n\t\t" + indent);
                sb.Append("\"position\": {\r\n\t\t\t" + indent + "\"lat\": " + Position.lat + ",\r\n\t\t\t" + indent +
                    "\"lon\": " + Position.lon + ",\r\n\t\t\t" + indent +
                    "\"h\":   " + Position.h);
                sb.Append("\r\n\t\t" + indent + "},");
                sb.Append("\r\n\t\t" + indent);
                sb.Append("\"angles\": {\r\n\t\t\t" + indent + "\"yaw\":   " + Orientation.yaw + ",\r\n\t\t\t" + indent +
                    "\"pitch\": " + Orientation.pitch + ",\r\n\t\t\t" + indent +
                    "\"roll\":  " + Orientation.roll);
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
    public class BasicQuaternion : GeoPose
    {
        /// <summary>
        /// A WGS84 (EPSG 4327) Position specified in spherical coordinates with height above the WGS84 ellipsoid.
        /// </summary>
        public new GeodeticPosition Position { get; set; } = new GeodeticPosition();
        /// <summary>
        /// An Orientation specified as a unit quaternion.
        /// </summary>
        public new Quaternion Orientation { get; set; } = new Quaternion();
        /// <summary>
        /// This function returns a Json encoding of a Basic-Quaternion GeoPose
        /// </summary>
        public string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            if (Position != null && Orientation != null)
            {
                sb.Append("{\r\n\t\t" + indent);
                sb.Append("\"position\": {\r\n\t\t\t" + indent + "\"lat\": " + Position.lat + ",\r\n\t\t\t" + indent +
                    "\"lon\": " + Position.lon + ",\r\n\t\t\t" + indent +
                    "\"h\":   " + Position.h);
                sb.Append("\r\n\t\t" + indent + "},");
                sb.Append("\r\n\t\t" + indent);
                sb.Append("\"angles\": {\r\n\t\t\t" + indent + "\"x\":   " + Orientation.x + ",\r\n\t\t\t" + indent +
                    "\"y\": " + Orientation.y + ",\r\n\t\t\t" + indent +
                    "\"z\": " + Orientation.z + ",\r\n\t\t\t" + indent +
                    "\"w\":  " + Orientation.w);
                sb.Append("\r\n\t\t" + indent + "}");
                sb.Append("\r\n\t" + indent + "}");
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Advanced GeoPose.
    /// </summary>
    public class Advanced : GeoPose
    {
        /// <summary>
        /// A Frame Specification defining a frame with associated coordinate system whose Position is the origin.
        /// </summary>
        public new FrameSpecification Position { get; set; } = new FrameSpecification();
        /// <summary>
        /// An Orientation specified as a unit quaternion.
        /// </summary>
        public new Quaternion Orientation { get; set; } = new Quaternion();
        /// <summary>
        /// Milliseconds of Unix time ticks (optional).
        /// </summary>
        public long? ValidTime { get; set; } = 0;
        /// <summary>
        /// This function returns a Json encoding of an Advanced GeoPose
        /// </summary>
        public string ToJSON()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"frameSpecification\":{\"authority\":" + Position.authority + ",\"id\":" + Position.id + ",\"parameters\":" + Position.parameters + "},");
            sb.Append("\"quaternion\":{\"x\":" + Orientation.x + ",\"y\":" + Orientation.y + ",\"z\":" + Orientation.z + ",\"w\":" + Orientation.w);
            sb.Append("}}");
            return sb.ToString();
        }
    }

    // *******************************************************************************

    public abstract class Position
    {

    }
    /// <summary>
    /// The abstract root of the Orientation hierarchy.
    /// <note>
    /// Because the various ways to express Orientation share no underlying structure,
    /// the class definition is simply an empty shell.
    /// </note>
    /// </summary>
    public abstract class Orientation
    {

    }
    /// <summary>
    /// A specialization of Position for using two angles and a height for geodetic positions.
    /// </summary>
    public class GeodeticPosition : Position
    {
        /// <summary>
        /// A latitude in degrees, positive north of equator and negative south of equator.
        /// The latitude is the angle between the plane of the equator and a plane tangent to the ellipsoid at the given point.
        /// </summary>
        public double lat { get; set; } = double.NaN;
        /// <summary>
        /// A longitude in degrees, positive east of the prime meridian and negative west of prime meridian.
        /// </summary>
        public double lon { get; set; } = double.NaN;
        /// <summary>
        /// A distance in meters, measured with respect to an implied (Basic) or specified (Advanced) reference surface,
        /// postive opposite the direction of the force of gravity,
        /// and negative in the direction of the force of gravity.
        /// </summary>
        public double h { get; set; } = double.NaN;
    }
    /// <summary>
    /// A specialization of Position for geocentric positions.
    /// </summary>
    public class GeocentricPosition : Position
    {
        /// <summary>
        /// A coordinate value in meters, along an axis (x-axis) that typically has origin at
        /// the center of mass, lies in the same plane as the y axis, and perpendicular to the y axis,
        /// forming a right-hand coordinate system with the z-axis in the up direction.
        /// </summary>
        public double x { get; set; } = double.NaN;
        /// <summary>
        /// A coordinate value in meters, along an axis (y-axis) that typically has origin at
        /// the center of mass, lies in the same plane as the x axis, and perpendicular to the x axis,
        /// forming a right-hand coordinate system with the z-axis in the up direction.
        /// </summary>
        public double y { get; set; } = double.NaN;
        /// <summary>
        /// A coordinate value in meters, along the z-axis.
        /// </summary>
        public double z { get; set; } = double.NaN;
    }
    /// <summary>
    /// A specialization of Position for using two angles and a height for geodetic positions.
    /// </summary>
    public class ENUPosition : Position
    {
        /// <summary>
        /// A coordinate value in meters, along an axis (east-axis) that has origin at
        /// a point tangent to an ellisoid defined by a geodetic frame,
        /// is perpendicular to the north axis,
        /// forms a right-hand coordinate system with the north and up axes, and
        /// which lies in a plane tangent to the ellisoid at that point.
        /// </summary>
        public double East { get; set; } = double.NaN;
        /// <summary>
        /// A coordinate value in meters, along an axis (north-axis) that has origin at
        /// a point tangent to an ellisoid defined by a geodetic frame,
        /// is perpendicular to the east axis,
        /// forms a right-hand coordinate system with the east and up axes, and
        /// which lies in a plane tangent to the ellisoid at that point.
        /// </summary>
        public double North { get; set; } = double.NaN;
        /// <summary>
        /// A coordinate value in meters, measured perpendicular to the tangent plane,
        /// positive in the up direction.
        /// </summary>
        public double Up { get; set; } = double.NaN;
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
    /// <summary>
    /// A derived pose within a LTP-ENU frame.
    /// <remark>
    /// Not part of the standard, but usefl when operating within the LTP-ENU frame defined by a Basic GeoPose.
    /// </remark>
    /// </summary>
    public class ENUPose : GeoPose
    {
        /// <summary>
        /// The east, north, up Position.
        /// </summary>
        public new ENUPosition Position { get; set; } = new ENUPosition();
        /// <summary>
        /// The yaw, pitch, roll orientation.
        /// </summary>
        public new YPRAngles Orientation { get; set; } = new YPRAngles();
        /// <summary>
        /// This function returns a Json encoding of a Basic-YPR GeoPose
        /// </summary>
        public string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\r\n\t\t" + indent);
            sb.Append("\"position\": {\r\n\t\t\t" + indent + "\"east\": " + Position.East + ",\r\n\t\t\t" + indent +
                "\"north\": " + Position.North + ",\r\n\t\t\t" + indent +
                "\"up\":   " + Position.Up);
            sb.Append("\r\n\t\t" + indent + "},");
            sb.Append("\r\n\t\t" + indent);
            sb.Append("\"angles\": {\r\n\t\t\t" + indent + "\"yaw\":   " + Orientation.yaw + ",\r\n\t\t\t" + indent +
                "\"pitch\": " + Orientation.pitch + ",\r\n\t\t\t" + indent +
                "\"roll\":  " + Orientation.roll);
            sb.Append("\r\n\t\t" + indent + "}");
            sb.Append("\r\n\t" + indent + "}");
            return sb.ToString();
        }
    }
    /// <summary>
    /// A FrameSpecification is a generic container for information that defines a reference frame.
    /// <remark>
    /// A FrameSpecification can be abstracted as a Position:
    /// The origin of the coordinate system associated with the frame is a Position and serves in that role
    /// in the Advanced GeoPose.
    /// The origin, is in fact the *only* distinguished Position associated with the coodinate system.
    /// </remark>
    /// </summary>
    public class FrameSpecification
    {
        /// <summary>
        /// The name or identification of the definer of the category of frame specification.
        /// A Uri that usually but not always points to a valid web address.
        /// </summary>
        public string authority { get; set; } = "";
        /// <summary>
        /// A string that uniquely identifies a frame type.
        /// The interpretation of the string is determined by the authority.
        /// </summary>
        public string id { get; set; } = "";
        /// <summary>
        /// A string that holds any parameters required by the authority to define a frame of the given type as specified by the id.
        /// The interpretation of the string is determined by the authority.
        /// </summary>
        public string parameters { get; set; } = "";
    }
}
