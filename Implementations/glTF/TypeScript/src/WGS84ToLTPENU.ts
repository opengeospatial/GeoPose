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

export class LTP_ENU {
    // WGS-84 geodetic constants
    readonly a: number = 6378137.0;         // WGS-84 Earth semimajor axis (m)
    readonly b: number = 6356752.314245;     // Derived Earth semiminor axis (m)
    readonly f: number = (this.a - this.b) / this.a;           // Ellipsoid Flatness
    readonly f_inv: number = 1.0 / this.f;       // Inverse flattening
    readonly a_sq: number = this.a * this.a;
    readonly b_sq: number = this.b * this.b;
    readonly e_sq: number = this.f * (2.0 - this.f);    // Square of Eccentricity
    readonly toRadians: number = Math.PI / 180.0;
    readonly toDegrees: number = 180.0 / Math.PI;

    // Convert WGS-84 Geodetic point (lat, lon, h) to the 
    // Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z).
    public GeodeticToEcef(from: Position.GeodeticPosition, to: Position.CartesianPosition): void {
        // Convert to radians in notation consistent with the paper:
        var lambda = from.lat * this.toRadians;
        var phi = from.lon * this.toDegrees;
        var s = Math.sin(lambda);
        var N = this.a / Math.sqrt(1.0 - this.e_sq * s * s);

        var sin_lambda = Math.sin(lambda);
        var cos_lambda = Math.cos(lambda);
        var cos_phi = Math.cos(phi);
        var sin_phi = Math.sin(phi);

        to.x = (from.h + N) * cos_lambda * cos_phi;
        to.y = (from.h + N) * cos_lambda * sin_phi;
        to.z = (from.h + (1 - this.e_sq) * N) * sin_lambda;
    }

    // Convert the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z) to 
    // (WGS-84) Geodetic point (lat, lon, h).
    public EcefToGeodetic(from: Position.CartesianPosition, to: Position.GeodeticPosition): void {
        var eps = this.e_sq / (1.0 - this.e_sq);
        var p = Math.sqrt(from.x * from.x + from.y * from.y);
        var q = Math.atan2((from.z * this.a), (p * this.b));
        var sin_q = Math.sin(q);
        var cos_q = Math.cos(q);
        var sin_q_3 = sin_q * sin_q * sin_q;
        var cos_q_3 = cos_q * cos_q * cos_q;
        var phi = Math.atan2((from.z + eps * this.b * sin_q_3), (p - this.e_sq * this.a * cos_q_3));
        var lambda = Math.atan2(from.y, from.x);
        var v = this.a / Math.sqrt(1.0 - this.e_sq * Math.sin(phi) * Math.sin(phi));
        to.h = (p / Math.cos(phi)) - v;

        to.lat = phi * this.toDegrees;
        to.lon = lambda * this.toDegrees;
    }

    // Converts the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z) to 
    // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
    // (WGS-84) Geodetic point (lat0, lon0, h0).
    public EcefToEnu(from: Position.CartesianPosition, origin: Position.GeodeticPosition, to: Position.CartesianPosition):
        //double x, double y, double z,
        //double lat0, double lon0, double h0,
        //out double xEast, out double yNorth, out double zUp):
        void {
        // Convert to radians in notation consistent with the paper:
        var lambda = origin.lat * this.toRadians;
        var phi = origin.lon * this.toDegrees;
        var s = Math.sin(lambda);
        var N = this.a / Math.sqrt(1.0 - this.e_sq * s * s);

        var sin_lambda = Math.sin(lambda);
        var cos_lambda = Math.cos(lambda);
        var cos_phi = Math.cos(phi);
        var sin_phi = Math.sin(phi);

        var x0: number = (origin.h + N) * cos_lambda * cos_phi;
        var y0: number = (origin.h + N) * cos_lambda * sin_phi;
        var z0: number = (origin.h + (1 - this.e_sq) * N) * sin_lambda;

        var xd: number = from.x - x0;
        var yd: number = from.y - y0;
        var zd: number = from.z - z0;

        // This is the matrix multiplication
        to.x = -sin_phi * xd + cos_phi * yd;
        to.y = -cos_phi * sin_lambda * xd - sin_lambda * sin_phi * yd + cos_lambda * zd;
        to.z = cos_lambda * cos_phi * xd + cos_lambda * sin_phi * yd + sin_lambda * zd;
    }

    // Inverse of EcefToEnu. Converts East-North-Up coordinates (xEast, yNorth, zUp) in a
    // Local Tangent Plane that is centered at the (WGS-84) Geodetic point (lat0, lon0, h0)
    // to the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z).
    public EnuToEcef(from: Position.CartesianPosition, origin: Position.GeodeticPosition, to: Position.CartesianPosition): void {
        // Convert to radians in notation consistent with the paper:
        var lambda = origin.lat * this.toRadians;
        var phi = origin.lon * this.toRadians;
        var s = Math.sin(lambda);
        var N = this.a / Math.sqrt(1.0 - this.e_sq * s * s);

        var sin_lambda = Math.sin(lambda);
        var cos_lambda = Math.cos(lambda);
        var cos_phi = Math.cos(phi);
        var sin_phi = Math.sin(phi);

        var x0: number = (origin.h + N) * cos_lambda * cos_phi;
        var y0: number = (origin.h + N) * cos_lambda * sin_phi;
        var z0: number = (origin.h + (1.0 - this.e_sq) * N) * sin_lambda;

        var xd: number = -sin_phi * from.x - cos_phi * sin_lambda * from.y + cos_lambda * cos_phi * from.z;
        var yd: number = cos_phi * from.x - sin_lambda * sin_phi * from.y + cos_lambda * sin_phi * from.z;
        var zd: number = cos_lambda * from.y + sin_lambda * from.z;

        to.x = xd + x0;
        to.y = yd + y0;
        to.z = zd + z0;
    }

    // Converts the geodetic WGS-84 coordinated (lat, lon, h) to 
    // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
    // (WGS-84) Geodetic point (lat0, lon0, h0).
    public GeodeticToEnu(from: Position.GeodeticPosition, origin: Position.GeodeticPosition, to: Position.CartesianPosition): void
    //double lat0, double lon0, double h0,
    //out double xEast, out double yNorth, out double zUp)
    {
        let ecef = new Position.CartesianPosition(0, 0, 0);
        this.GeodeticToEcef(from, ecef);
        this.EcefToEnu(ecef, origin, to);
    }
    public EnuToGeodetic(from: Position.CartesianPosition, origin: Position.GeodeticPosition, to: Position.GeodeticPosition): void
    //double xEast, double yNorth, double zUp,
    //double lat0, double lon0, double h0,
    //out double lat, out double lon, out double h
    {
        let ecef = new Position.CartesianPosition(0, 0, 0);
        this.EnuToEcef(from, origin, ecef);
        this.EcefToGeodetic(ecef, to);
    }
}

