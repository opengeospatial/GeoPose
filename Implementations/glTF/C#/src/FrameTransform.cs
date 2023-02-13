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

// Implemention order: 3 - follows Position.
// These classes define transformations of a Position in one 3D frame to a Position in another 3D frame.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using GeoAPI.CoordinateSystems.Transformations;

using Positions;
using Support;

namespace FrameTransforms
{
    /// <summary>
    /// A FrameTransform is a generic container for information that defines mapping between reference frames.
    /// Most transformation have a context with necessary ancillary information
    /// that parameterizes the transformation of a Position in one frame to a corresponding Position is another.
    /// Such context may include, for example, some or all of the information that may be conveyed in an ISO 19111 CRS specification
    /// or a proprietary naming, numbering, or modelling scheme as used by EPSG, NASA Spice, or SEDRIS SRM.
    /// Subclasses of FrameTransform exist precisely to hold this context in conjunction with code
    /// implementing a Transform function.
    /// <remark>
    /// </remark>
    /// </summary>
    public abstract class FrameTransform
    {
        public virtual Position Transform(Position point)
        {
            // The defualt is to apply the identity transformation
            Position result = point;
            return result;
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
    public class Extrinsic : FrameTransform
    {
        internal Extrinsic()
        {

        }
        public Extrinsic(string authority, string id, string parameters)
        {
            this.authority = authority;
            this.id = id;
            this.parameters = parameters;
        }
        /// <summary>
        /// The core function of a transformation is the implement a specific frame transformation
        /// i.e. the transformation of a triple of point coordinates in the outer frame to a triple of point coordinates in the inner frame.
        /// When this is not possible due to lack of an appropriate tranformation procedure,
        /// the triple (NaN, NaN, NaN) [three IEEE 574 not-a-number vales] is returned.
        /// Note that an "authority" is not necessarily a standards organization but rather an entity that provides
        /// a register of some kind for a category of frame- and/or frame transform specifications that is useful and stable enough
        /// for someone to implement transformation functions.
        /// An implementation need not implement all possbile transforms.
        /// </summary>
        /// <note>
        /// This would be a good element to implement as a set of plugin.
        /// </note>
        /// <param name="point"></param>
        /// <returns></returns>
        public override Position Transform(Position point)
        {
            string uri = authority.ToLower().Replace("//www.", "");
            if (uri == "https://proj.org" || uri == "https://osgeo.org")
            {
                CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();
                string WKT = "PROJCRS[\"GeoPose LTP-ENU\", GEOGCS[\"WGS 84 (G873)\", DATUM[\"World_Geodetic_System_1984_G873\", " +
                    "SPHEROID[\"WGS 84\",6378137,298.257223563, AUTHORITY[\"EPSG\",\"7030\"]], AUTHORITY[\"EPSG\",\"1153\"]], " +
                    "PRIMEM[\"Greenwich\",0, AUTHORITY[\"EPSG\",\"8901\"]], UNIT[\"degree\",0.0174532925199433, AUTHORITY[\"EPSG\",\"9122\"]], " +
                    "AUTHORITY[\"EPSG\",\"9054\"]] CONVERSION[\"Topocentric LTP-ENU\", " +
                    "METHOD[\"Geographic/topocentric conversions\", ID[\"EPSG\",9837]], " +
                    "PARAMETER[\"Latitude of topocentric origin\",55, ANGLEUNIT[\"degree\",0.0174532925199433], ID[\"EPSG\",8834]], " +
                    "PARAMETER[\"Longitude of topocentric origin\",5, ANGLEUNIT[\"degree\",0.0174532925199433], ID[\"EPSG\",8835]], " +
                    "PARAMETER[\"Ellipsoidal height of topocentric origin\",0, LENGTHUNIT[\"metre\",1], ID[\"EPSG\",8836]], " +
                    "ID[\"EPSG\",15594]] CS[Cartesian,3], " +
                    "AXIS[\"topocentric East (U)\",east, ORDER[1], LENGTHUNIT[\"metre\",1]], " +
                    "AXIS[\"topocentric North (V)\",north, ORDER[2], LENGTHUNIT[\"metre\",1]], " +
                    "AXIS[\"topocentric height (W)\",up, ORDER[3], LENGTHUNIT[\"metre\",1]] " +
                    "USAGE[ AREA[\"Planet Earth\"], BBOX[-90,-180,90,180]], ID[\"GeoPose\",LTP-ENU]]";

                var from = GeographicCoordinateSystem.WGS84;
                var to = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(30, true);

                // convert points from one coordinate system to another
                ProjNet.CoordinateSystems.Transformations.ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(from, to);

                ProjNet.Geometries.XY businessCoordinate = new ProjNet.Geometries.XY(-0.127758, 51.507351);
                ProjNet.Geometries.XY searchLocationCoordinate = new ProjNet.Geometries.XY(-0.142500, 51.539188);

                MathTransform mathTransform = trans.MathTransform;
                var businessLocation = mathTransform.Transform(businessCoordinate.X, businessCoordinate.Y);
                var searchLocation = mathTransform.Transform(searchLocationCoordinate.X, searchLocationCoordinate.Y);



                return noTransform;
            }
            else if (uri == "https://epsg.org")
            {
                return noTransform;
            }
            else if (uri == "https://iers.org")
            {
                return noTransform;
            }
            else if (uri == "https://naif.jpl.nasa.gov")
            {
                return noTransform;
            }
            else if (uri == "https://sedris.org")
            {
                return noTransform;
            }
            else if (uri == "https://iau.org")
            {
                return noTransform;
            }
            else if (uri == "https://geopose.io")
            {
                // extract pair of CS wkts or CS plus transformation from the id string
                // get the point to transform from the parameters string, JSON-encoded as
                //  {"lat": 12.345, "lon": -22.54, "h": 11.22}
                //  if contains "=>" then that splits the outer and inner specs
                //  else check as special case: ID[\"EPSG\",5819]]$ or AUTHORITY[\"EPGS\",\"5819\"]]$
#if REPLACED
                //var inP = new Positions.NoPosition();
                double[] xyz = new double[3];
                if (point is CartesianPosition)
                {
                    xyz[0] = ((CartesianPosition)point).x;
                    xyz[1] = ((CartesianPosition)point).y;
                    xyz[2] = ((CartesianPosition)point).z;
                }
                else if(point is GeodeticPosition)
                {
                    xyz[0] = ((GeodeticPosition)point).lon;
                    xyz[1] = ((GeodeticPosition)point).lat;
                    xyz[2] = ((GeodeticPosition)point).h;
                }
                else
                {
                    return new Positions.NoPosition();
                }
                var cf = new CoordinateSystemFactory();
                var f = new CoordinateTransformationFactory();
                string wkt25831 = "PROJCS[\"ETRS89 / UTM zone 31N\",GEOGCS[\"ETRS89\",DATUM[\"European_Terrestrial_Reference_System_1989\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6258\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4258\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",3],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"Easting\",EAST],AXIS[\"Northing\",NORTH],AUTHORITY[\"EPSG\",\"25831\"]]";
                string wkt3857 = "PROJCS[\"WGS 84 / World Mercator\",GEOGCS[\"WGS 84 sphere\",DATUM[\"WGS_1984 sphere\",SPHEROID[\"WGS 84 sphere\",6378137,0.0]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]]],PROJECTION[\"Mercator_1SP\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",0],PARAMETER[\"scale_factor\",1],PARAMETER[\"false_easting\",0],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"Easting\",EAST],AXIS[\"Northing\",NORTH]]";
                var cs25831 = cf.CreateFromWkt(wkt25831);
                var cs3857 = cf.CreateFromWkt(wkt3857);
                var transformTo3857 = f.CreateFromCoordinateSystems(cs25831, cs3857);
                double[] ret = transformTo3857.MathTransform.Transform(xyz);
                if (ret != null)
                {
                    var outP = new Positions.CartesianPosition(ret[0], ret[1], xyz[2]);
                    return outP;
                }
#endif // REPLACED
                if (Support.ExtrinsicSupport.IsDerivedCRS(id))
                {
                    //
                    string epsgNumber = Support.ExtrinsicSupport.GetEPSGNumber(id);
                    if (epsgNumber == null || epsgNumber == "")
                    {
                        return noTransform;
                    }
                    else if (epsgNumber == "5819")
                    {
                        // get lat0, lon0, h0
                        double[] origin = new double[3];
                        if (Support.ExtrinsicSupport.GetOriginParameters(id, ref origin))
                        {
                            Positions.GeodeticPosition inPoint = Support.ExtrinsicSupport.GetPositionFromParameters(this.parameters);
                            Positions.GeodeticPosition tangentPoint = new Positions.GeodeticPosition(origin[0], origin[1], origin[2]);
                            Positions.CartesianPosition outPoint = Support.LTP_ENU.GeodeticToEnu(inPoint, tangentPoint);
                        }

                    }
                    else
                    {
                        return noTransform;
                    }
                }
                else if (Support.ExtrinsicSupport.IsFromAndToCRS(id))
                {
                    string fromCRS = "";
                    string toCRS = "";
                    if (Support.ExtrinsicSupport.GetFromAndToCRS(id, out fromCRS, out toCRS))
                    {

                        var cf = new CoordinateSystemFactory();
                        var f = new CoordinateTransformationFactory();
                        CoordinateSystem csIn = null;
                        try
                        {
                            csIn = cf.CreateFromWkt(fromCRS);
                        }
                        catch (Exception ex)
                        {
                            return noTransform;
                        }
                        CoordinateSystem csOut = null;
                        try
                        {
                            csOut = cf.CreateFromWkt(toCRS);
                        }
                        catch (Exception ex)
                        {
                            return noTransform;
                        }
                        //var cs3857 = cf.CreateFromWkt(wkt3857);
                        ProjNet.CoordinateSystems.Transformations.ICoordinateTransformation transform = null;
                        if (csIn != null && csOut != null)
                        {
                            Positions.GeodeticPosition inPoint = Support.ExtrinsicSupport.GetPositionFromParameters(this.parameters);
                            transform = f.CreateFromCoordinateSystems(csIn, csOut);
                            double[] xyz = new double[3] {inPoint.lon, inPoint.lat, inPoint.h}; // note lon, lat, h order
                            double[] XYZ = new double[3];
                            double[] ret = transform.MathTransform.Transform(xyz);
                            XYZ[0] = ret[0];
                            XYZ[1] = ret[1];
                            if (ret.Length == 2)
                            {
                                XYZ[2] = xyz[2];
                            }
                            else
                            {
                                XYZ[2] = ret[2];
                            }
                        }
                        else
                        {
                            Console.WriteLine("Coordinate transformation failed: ");
                        }
                    }
                    else
                    {
                        Console.WriteLine("from or to CS unrecognized");
                    }

                }
                else
                {
                    Console.WriteLine("id string missing \"=>\".");
                }

                return new Positions.NoPosition();
            }
            return noTransform;
        }
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
        static Position noTransform = new NoPosition();
    }
    /// <summary>
    /// A specialized specification of the WGS84 (EPSG 4326) geodetic frame to a local tangent plane East, North, Up frame.
    /// <remark>
    /// The origin of the coordinate system associated with the frame is a Position - the origin -
    /// which is the *only* distinguished Position associated with the coodinate system associated with the inner frame (range).
    /// </remark>
    /// </summary>
    public class WGS84ToLTP_ENU : FrameTransform
    {
        internal WGS84ToLTP_ENU()
        {

        }
        public WGS84ToLTP_ENU(GeodeticPosition origin)
        {
            this.Origin = origin;
        }
        public override Position Transform(Position point)
        {
            double east, north, up;
            Support.LTP_ENU.GeodeticToEnu(((GeodeticPosition)point).lat, ((GeodeticPosition)point).lon, ((GeodeticPosition)point).h,
                Origin.lat, Origin.lon, Origin.h, out east, out north, out up);
            CartesianPosition outPoint = new CartesianPosition(east, north, up);
            return outPoint;
        }

        /// <summary>
        /// A single geodetic position defines the tangent point for a transform to LTP-ENU.
        /// </summary>
        public GeodeticPosition Origin { get; set; } = null;
    }

    // A simple translation frame transform.
    // The FrameTransform is created with an offset.
    // The Transform adds the offset ot an input Cartesian Position and reurns a Cartesian Position
    public class Translation : FrameTransform
    {
        internal Translation()
        {
        }
        public Translation(double xOffset, double yOffset, double zOffset)
        {
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.zOffset = zOffset;
        }
        public override Position Transform(Position point)
        {
            return new CartesianPosition(((CartesianPosition)point).x, ((CartesianPosition)point).y, ((CartesianPosition)point).z);
        }
        public double xOffset { get; set; } = 0.0;
        public double yOffset { get; set; } = 0.0;
        public double zOffset { get; set; } = 0.0;
    }
}
