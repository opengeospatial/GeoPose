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

import * as proj4 from 'proj4';
import * as Position from './Position';

// Implemention order: 3 - follows Position.
// These classes define transformations of a Position in one 3D frame to a Position in another 3D frame.

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
export abstract class FrameTransform {
    public abstract Transform(point: Position.Position): Position.Position;
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
export class Extrinsic extends FrameTransform {
    public constructor(authority: string, id: string, parameters: string) {
        super();
        this.authority = authority;
        this.id = id;
        this.parameters = parameters;
    }
    /// <summary>
    /// The core function of a transformation is to implement a specific frame transformation
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
    public override Transform(point: Position.Position): Position.Position {
        let uri = this.authority.toLowerCase().replace("//www.", "");
        if (uri == "https://proj.org" || uri == "https://osgeo.org") {
            var outer = proj4.Proj('EPSG:4326');    //source coordinates will be in Longitude/Latitude, WGS84
            var inner = proj4.Proj('EPSG:3785');     //destination coordinates in meters, global spherical mercato
            var cp = point as Position.CartesianPosition;
            let p = proj4.Point(cp.x, cp.y, cp.z);
            proj4.transform(outer, inner, p);
            // convert points from one coordinate system to another
            let outP = new Position.CartesianPosition(p.x, p.y, p.z);
            return outP;
        }
        else if (uri == "https://epsg.org") {
            return Position.NoPosition;
        }
        else if (uri == "https://iers.org") {
            return Position.NoPosition;
        }
        else if (uri == "https://naif.jpl.nasa.gov") {
            return Position.NoPosition;
        }
        else if (uri == "https://sedris.org") {
            return Position.NoPosition;
        }
        else if (uri == "https://iau.org") {
            return Position.NoPosition;
        }
        return Position.NoPosition;
    }
    /// <summary>
    /// The name or identification of the definer of the category of frame specification.
    /// A Uri that usually but not always points to a valid web address.
    /// </summary>
    public authority: string;
    /// <summary>
    /// A string that uniquely identifies a frame type.
    /// The interpretation of the string is determined by the authority.
    /// </summary>
    public id: string;
    /// <summary>
    /// A string that holds any parameters required by the authority to define a frame of the given type as specified by the id.
    /// The interpretation of the string is determined by the authority.
    /// </summary>
    public parameters: string;
    public static noTransform: Position.Position = new Position.NoPosition();
}
/// <summary>
/// A specialized specification of the WGS84 (EPSG 4326) geodetic frame to a local tangent plane East, North, Up frame.
/// <remark>
/// The origin of the coordinate system associated with the frame is a Position - the origin -
/// which is the *only* distinguished Position associated with the coodinate system associated with the inner frame (range).
/// </remark>
/// </summary>
export class WGS84ToLTPENU extends FrameTransform {
    public constructor(origin: Position.GeodeticPosition) {
        super();
        this.Origin = origin;
    }
    public override Transform(point: Position.Position): Position.Position {
        let geoPoint = point as Position.GeodeticPosition;
        let outPoint: Position.CartesianPosition;
        GeodeticToEnu(this.Origin, geoPoint, outPoint);
        return outPoint;
    }

    /// <summary>
    /// A single geodetic position defines the tangent point for a transform to LTP-ENU.
    /// </summary>
    public Origin: Position.GeodeticPosition;
}

export function GeodeticToEnu(origin: Position.GeodeticPosition, geoPoint: Position.GeodeticPosition, enuPoint: Position.CartesianPosition) {
    let out = new Position.CartesianPosition(0, 0, 0);
    return out;
}

// A simple translation frame transform.
// The FrameTransform is created with an offset.
// The Transform adds the offset ot an input Cartesian Position and reurns a Cartesian Position
export class Translation extends FrameTransform {
    public constructor(xOffset: number, yOffset: number, zOffset: number) {
        super();
        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.zOffset = zOffset;
    }
    public override Transform(point: Position.Position): Position.Position {
        let cp = point as Position.CartesianPosition;
        let p = new Position.CartesianPosition(cp.x + this.xOffset, cp.y + this.yOffset, cp.z + this.zOffset);
        return p;
    }
    public xOffset: number;
    public yOffset: number;
    public zOffset: number;
}
