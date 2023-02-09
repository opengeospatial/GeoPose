"use strict";
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
Object.defineProperty(exports, "__esModule", { value: true });
exports.BasicQuaternion = exports.BasicYPR = exports.Basic = void 0;
const Extras = require("./Extras");
const FrameTransform = require("./FrameTransform");
const Orientation = require("./Orientation");
const GeoPose = require("./GeoPose");
// Implemention order: 6 - follows GeoPose.
// This is the simplest family of GeoPoses - the 80% part of a 80/20 solution.
/// <summary>
/// The Basic GeoPoses share the use of a local tangent plane, east-north-up frame transform.
/// The types of Basic GeoPose are distinguished by the method used to specify orientation of the inner frame.
/// </summary>
class Basic extends GeoPose.GeoPose {
}
exports.Basic = Basic;
/// <summary>
/// A Basic-YPR GeoPose uses yaw, pitch, and roll angles measured in degrees to define the orientation of the inner frame..
/// </summary>
class BasicYPR extends Basic {
    constructor(id, tangentPoint, yprAngles) {
        super();
        this.poseID = new Extras.PoseID(id);
        this.FrameTransform = new FrameTransform.WGS84ToLTPENU(tangentPoint);
        this.Orientation = yprAngles;
    }
    /// <summary>
    /// This function returns a Json encoding of a Basic-YPR GeoPose
    /// </summary>
    toJSON() {
        let indent = "";
        let sb = [''];
        if (FrameTransform != null && Orientation != null) {
            sb.push("{\r\n  " + indent);
            if (this.validTime != null) {
                sb.push("\"validTime\": " + this.validTime.toString() + ",\r\n" + indent + "  ");
            }
            if (this.poseID != null && this.poseID.id != "") {
                sb.push("\"poseID\": \"" + this.poseID.id + "\",\r\n" + indent + "  ");
            }
            if (this.parentPoseID != null && this.parentPoseID.id != "") {
                sb.push("\"parentPoseID\": \"" + this.parentPoseID.id + "\",\r\n" + indent + "  ");
            }
            sb.push("\"position\": \r\n  {\r\n    " + indent + "\"lat\": " +
                this.FrameTransform.Origin.lat + ",\r\n    " + indent +
                "\"lon\": " + this.FrameTransform.Origin.lon + ",\r\n    " + indent +
                "\"h\":   " + this.FrameTransform.Origin.h);
            sb.push("\r\n  " + indent + "},");
            sb.push("\r\n  " + indent);
            sb.push("\"angles\": \r\n  {\r\n    " + indent + "\"yaw\":   " +
                this.Orientation.yaw + ",\r\n    " + indent +
                "\"pitch\": " + this.Orientation.pitch + ",\r\n    " + indent +
                "\"roll\":  " + this.Orientation.roll);
            sb.push("\r\n  " + indent + "}");
            sb.push("\r\n" + indent + "}");
        }
        return sb.join('');
    }
}
exports.BasicYPR = BasicYPR;
/// <summary>
/// A Basic-Quaternion GeoPose uses a unit quaternions to define the orientation of the inner frame..
/// <remark>
/// See the OGS GeoPose 1.0 standard for a full description.
/// </remark>
/// </summary>
class BasicQuaternion extends Basic {
    constructor(id, tangentPoint, quaternion) {
        super();
        this.poseID = new Extras.PoseID(id);
        this.FrameTransform = new FrameTransform.WGS84ToLTPENU(tangentPoint);
        this.Orientation = quaternion;
    }
    /// <summary>
    /// This function returns a Json encoding of a Basic-Quaternion GeoPose
    /// </summary>
    toJSON() {
        let indent = "";
        let sb = [''];
        if (this.FrameTransform.Origin != null && this.Orientation != null) {
            sb.push("{\r\n  " + indent);
            if (this.validTime != null) {
                sb.push("\"validTime\": " + this.validTime.toString() + ",\r\n" + indent + "  ");
            }
            if (this.poseID != null && this.poseID.id != "") {
                sb.push("\"poseID\": \"" + this.poseID.id + "\",\r\n" + indent + "  ");
            }
            if (this.parentPoseID != null && this.parentPoseID.id != "") {
                sb.push("\"parentPoseID\": \"" + this.parentPoseID.id + "\",\r\n" + indent + "  ");
            }
            sb.push("\"position\": \r\n  {\r\n    " + indent + "\"lat\": " +
                this.FrameTransform.Origin.lat + ",\r\n    " + indent +
                "\"lon\": " + this.FrameTransform.Origin.lon +
                ",\r\n    " + indent +
                "\"h\":   " + this.FrameTransform.Origin.h);
            sb.push("\r\n  " + indent + "},");
            sb.push("\r\n  " + indent);
            sb.push("\"quaternion\": \r\n  {\r\n    " + indent + "\"x\":   " +
                this.Orientation.x + ",\r\n      " + indent +
                "\"y\": " + this.Orientation.y + ",\r\n      " + indent +
                "\"z\": " + this.Orientation.z + ",\r\n      " + indent +
                "\"w\":  " + this.Orientation.w);
            sb.push("\r\n  " + indent + "}");
            sb.push("\r\n" + indent + "}");
            return sb.join('');
        }
    }
}
exports.BasicQuaternion = BasicQuaternion;
//# sourceMappingURL=Basic.js.map