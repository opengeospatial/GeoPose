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

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ProjNet;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using GeoAPI.CoordinateSystems.Transformations;

namespace Support
{
    // This is an implementation of EPSG method 9837 using sections 4.1.1 and 4.1.2 of https://www.iogp.org/wp-content/uploads/2019/09/373-07-02.pdf.
    public class LTP_ENU
    {
        // WGS-84 geodetic constants
        const double a = 6378137.0;         // WGS-84 Earth semimajor axis (m)

        const double b = 6356752.314245;     // Derived Earth semiminor axis (m)
        const double f = (a - b) / a;           // Ellipsoid Flatness
        const double f_inv = 1.0 / f;       // Inverse flattening

        //const double f_inv = 298.257223563; // WGS-84 Flattening Factor of the Earth 
        //const double b = a - a / f_inv;
        //const double f = 1.0 / f_inv;

        const double a_sq = a * a;
        const double b_sq = b * b;
        const double e_sq = f * (2 - f);    // Square of Eccentricity

        // Converts WGS-84 Geodetic point (lat, lon, h) to the 
        // Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z).
        public static void GeodeticToEcef(double lat, double lon, double h,
                                            out double x, out double y, out double z)
        {
            // Convert to radians in notation consistent with the paper:
            var lambda = DegreesToRadians(lat);
            var phi = DegreesToRadians(lon);
            var s = Math.Sin(lambda);
            var N = a / Math.Sqrt(1 - e_sq * s * s);

            var sin_lambda = Math.Sin(lambda);
            var cos_lambda = Math.Cos(lambda);
            var cos_phi = Math.Cos(phi);
            var sin_phi = Math.Sin(phi);

            x = (h + N) * cos_lambda * cos_phi;
            y = (h + N) * cos_lambda * sin_phi;
            z = (h + (1 - e_sq) * N) * sin_lambda;
        }

        // Converts the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z) to 
        // (WGS-84) Geodetic point (lat, lon, h).
        public static void EcefToGeodetic(double x, double y, double z,
                                            out double lat, out double lon, out double h)
        {
            var eps = e_sq / (1.0 - e_sq);
            var p = Math.Sqrt(x * x + y * y);
            var q = Math.Atan2((z * a), (p * b));
            var sin_q = Math.Sin(q);
            var cos_q = Math.Cos(q);
            var sin_q_3 = sin_q * sin_q * sin_q;
            var cos_q_3 = cos_q * cos_q * cos_q;
            var phi = Math.Atan2((z + eps * b * sin_q_3), (p - e_sq * a * cos_q_3));
            var lambda = Math.Atan2(y, x);
            var v = a / Math.Sqrt(1.0 - e_sq * Math.Sin(phi) * Math.Sin(phi));
            h = (p / Math.Cos(phi)) - v;

            lat = RadiansToDegrees(phi);
            lon = RadiansToDegrees(lambda);
        }

        // Converts the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z) to 
        // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
        // (WGS-84) Geodetic point (lat0, lon0, h0).
        public static void EcefToEnu(double x, double y, double z,
                                        double lat0, double lon0, double h0,
                                        out double xEast, out double yNorth, out double zUp)
        {
            // Convert to radians in notation consistent with the paper:
            var lambda = DegreesToRadians(lat0);
            var phi = DegreesToRadians(lon0);
            var s = Math.Sin(lambda);
            var N = a / Math.Sqrt(1 - e_sq * s * s);

            var sin_lambda = Math.Sin(lambda);
            var cos_lambda = Math.Cos(lambda);
            var cos_phi = Math.Cos(phi);
            var sin_phi = Math.Sin(phi);

            double x0 = (h0 + N) * cos_lambda * cos_phi;
            double y0 = (h0 + N) * cos_lambda * sin_phi;
            double z0 = (h0 + (1 - e_sq) * N) * sin_lambda;

            double xd, yd, zd;
            xd = x - x0;
            yd = y - y0;
            zd = z - z0;

            // This is the matrix multiplication
            xEast = -sin_phi * xd + cos_phi * yd;
            yNorth = -cos_phi * sin_lambda * xd - sin_lambda * sin_phi * yd + cos_lambda * zd;
            zUp = cos_lambda * cos_phi * xd + cos_lambda * sin_phi * yd + sin_lambda * zd;
        }

        // Inverse of EcefToEnu. Converts East-North-Up coordinates (xEast, yNorth, zUp) in a
        // Local Tangent Plane that is centered at the (WGS-84) Geodetic point (lat0, lon0, h0)
        // to the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z).
        public static void EnuToEcef(double xEast, double yNorth, double zUp,
                                        double lat0, double lon0, double h0,
                                        out double x, out double y, out double z)
        {
            // Convert to radians in notation consistent with the paper:
            var lambda = DegreesToRadians(lat0);
            var phi = DegreesToRadians(lon0);
            var s = Math.Sin(lambda);
            var N = a / Math.Sqrt(1 - e_sq * s * s);

            var sin_lambda = Math.Sin(lambda);
            var cos_lambda = Math.Cos(lambda);
            var cos_phi = Math.Cos(phi);
            var sin_phi = Math.Sin(phi);

            double x0 = (h0 + N) * cos_lambda * cos_phi;
            double y0 = (h0 + N) * cos_lambda * sin_phi;
            double z0 = (h0 + (1 - e_sq) * N) * sin_lambda;

            double xd = -sin_phi * xEast - cos_phi * sin_lambda * yNorth + cos_lambda * cos_phi * zUp;
            double yd = cos_phi * xEast - sin_lambda * sin_phi * yNorth + cos_lambda * sin_phi * zUp;
            double zd = cos_lambda * yNorth + sin_lambda * zUp;

            x = xd + x0;
            y = yd + y0;
            z = zd + z0;
        }

        // Converts the geodetic WGS-84 coordinated (lat, lon, h) to 
        // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
        // (WGS-84) Geodetic point (lat0, lon0, h0).
        public static void GeodeticToEnu(double lat, double lon, double h,
                                            double lat0, double lon0, double h0,
                                            out double xEast, out double yNorth, out double zUp)
        {
            double x, y, z;
            GeodeticToEcef(lat, lon, h, out x, out y, out z);
            EcefToEnu(x, y, z, lat0, lon0, h0, out xEast, out yNorth, out zUp);
        }
        // Converts the geodetic WGS-84 coordinated (lat, lon, h) to 
        // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
        // (WGS-84) Geodetic point (lat0, lon0, h0).
        public static Positions.CartesianPosition GeodeticToEnu(Positions.GeodeticPosition geodeticPosition,
            Positions.GeodeticPosition tangentPosition)
        {
            double x, y, z;
            double xEast, yNorth, zUp; 
            GeodeticToEcef(geodeticPosition.lat, geodeticPosition.lon, geodeticPosition.h, out x, out y, out z);
            EcefToEnu(x, y, z, tangentPosition.lat, tangentPosition.lon, tangentPosition.h, out xEast, out yNorth, out zUp);
            Positions.CartesianPosition enuPosition = new Positions.CartesianPosition(xEast, yNorth, zUp);
            return enuPosition;
        }
        public static void EnuToGeodetic(double xEast, double yNorth, double zUp,
                                             double lat0, double lon0, double h0,
                                            out double lat, out double lon, out double h
                                            )
        {
            double x, y, z;
            EnuToEcef(xEast, yNorth, zUp, lat0, lon0, h0, out x, out y, out z);
            EcefToGeodetic(x, y, z, out lat, out lon, out h);
        }

        static double DegreesToRadians(double degrees)
        {
            return Math.PI / 180.0 * degrees;
        }

        static double RadiansToDegrees(double radians)
        {
            return 180.0 / Math.PI * radians;
        }
    }
    public class ExtrinsicSupport
    {
        public static bool IsDerivedCRS(string idString)
        {
            return idString.ToLower().Contains("conversion[");
        }
        public static bool IsFromAndToCRS(string idString)
        {
            return idString.Contains("=>");
        }
        public static bool GetFromAndToCRS(string idString, out string fromCRS, out string toCRS)
        {
            fromCRS = "";
            toCRS = "";
            // Split at =>
            int arrowIndex = idString.IndexOf("=>");
            if (arrowIndex < 1)
            {
                return false;
            }
            fromCRS = idString.Substring(0, arrowIndex);
            toCRS = idString.Substring(arrowIndex + 2);
            return true;
        }
        public static string GetEPSGNumber(string wktString)
        {
            string epsgNumber = "";
            // look at end of WKT for WKT1 or WKT2 ID
            // "ID\["EPSG",\d+\]\]$" or "AUTHORITY\["EPSG",\"\d+\"\]\]$"
            Regex reID = new Regex("(id\\[\\\"epsg\\\",\\d+\\]\\]$)");
            Regex reAuthority = new Regex("(authority\\[\\\"epsg\\\",\\\"\\d+\\\"\\]\\]$)");
            string thisMatch = "";
            MatchCollection matches;
            if ((matches = reID.Matches(wktString.ToLower())).Count > 0)
            {
                thisMatch = matches[0].Value;
            }
            else if ((matches = reAuthority.Matches(wktString.ToLower())).Count > 0)
            {
                thisMatch = matches[0].Value;
            }
            if (thisMatch != "")
            {
                Regex reNumber = new Regex("\\d+");
                matches = reNumber.Matches(thisMatch);
                if (matches.Count > 0)
                {
                    epsgNumber = matches[0].Value;
                }
            }
            return epsgNumber;
        }
        public static bool GetOriginParameters(string wktString, ref double[] origin)
        {
            // PARAMETER[\"Latitude of topocentric origin\",55,
            origin[0] = GetSignedDoubleInRe("parameter\\[\\\"latitude.+,-?\\d+\\.?\\d*", wktString);
            origin[1] = GetSignedDoubleInRe("parameter\\[\\\"longitude.+,-?\\d+\\.?\\d*", wktString);
            origin[2] = GetSignedDoubleInRe("parameter\\[\\\"[^\\]]*height.+,-?\\d+\\.?\\d*", wktString);
            return (!double.IsNaN(origin[0]) && !double.IsNaN(origin[1]) && !double.IsNaN(origin[2]));
        }
        public static Positions.GeodeticPosition GetOriginParameters(string wktString)
        {
            // PARAMETER[\"Latitude of topocentric origin\",55,
            double lat = GetSignedDoubleInRe("parameter\\[\\\"latitude.+,-?\\d+\\.?\\d*", wktString);
            double lon = GetSignedDoubleInRe("parameter\\[\\\"longitude.+,-?\\d+\\.?\\d*", wktString);
            double h = GetSignedDoubleInRe("parameter\\[\\\"[^\\]]*height.+,-?\\d+\\.?\\d*", wktString);
            if (!double.IsNaN(lat) && !double.IsNaN(lon) && !double.IsNaN(h))
            {
                return new Positions.GeodeticPosition(lat, lon, h);
            }
            return new Positions.NoPosition();
        }
        public static double GetSignedDoubleInRe(string reString, string inputString)
        {
            double result = double.NaN;
            Regex re = new Regex(reString);
            MatchCollection matches = re.Matches(inputString.ToLower());
            if (matches.Count > 0)
            {
                string thisMatch = matches[0].Value;
                re = new Regex("-?\\d+\\.?\\d*");
                matches = re.Matches(thisMatch);
                if (matches.Count > 0)
                {
                    result = double.Parse(matches[0].Value);
                }
            }
            return result;
        }
        public static Positions.GeodeticPosition GetPositionFromParameters(string paramString)
        {
            // JSON encoded: {"lat": 12.345, "lon": -22.54, "h": 11.22}
            double lat = GetSignedDoubleInRe("\\\"lat\\\"\\s*:\\s*-?\\d+(\\.\\d*)?", paramString);
            double lon = GetSignedDoubleInRe("\\\"lon\\\"\\s*:\\s*-?\\d+(\\.\\d*)?", paramString);
            double h = GetSignedDoubleInRe("\\\"h\\\"\\s*:\\s*-?\\d+(\\.\\d*)?", paramString);
            if (!double.IsNaN(lat) && !double.IsNaN(lon) && !double.IsNaN(h))
            {
                return new Positions.GeodeticPosition(lat, lon, h);
            }
            return new Positions.NoPosition();
        }

    }
}
