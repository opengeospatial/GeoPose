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

// Implemention order: 2 - follows Extras.
// These classes define positions in a 3D frame using different conventions.

/// <summary>
/// The abstract root of the Position hierarchy.
/// <note>
/// Because these various ways to express Position share no underlying structure,
/// the abstract root class definition is simply an empty shell.
/// </note>
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Positions
{
    /// <summary>
    /// A specialization of Position for using two angles and a height for geodetic positions.
    /// </summary>
    public class GeodeticPosition : Position
    {
        internal GeodeticPosition()
        {

        }
        public GeodeticPosition(double lat, double lon, double h)
        {
            this.lat = lat;
            this.lon = lon;
            this.h = h;
        }

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
    public class CartesianPosition : Position
    {
        public CartesianPosition(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

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
    public class NoPosition : Position
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
    /// The abstract root of the Position hierarchy.
    /// <note>
    /// Because the various ways to express Position share no underlying structure,
    /// the class definition is simply an empty shell.
    /// </note>
    /// </summary>
    public abstract class Position
    {

    }
}
