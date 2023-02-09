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

import { stdin as input } from 'node:process';
import * as proj4 from 'proj4';
import * as GeoPose from './GeoPose'
import * as Position from './Position';
import * as Orientation from './Orientation';
import * as LTPENU from './WGS84ToLTPENU';
import * as Basic from './Basic';
import * as Advanced from './Advanced';
import * as Local from './Local';
import * as FrameTransform from './FrameTransform';
import * as Extras from './Extras';

class Display {
    public static Output(spaceTrain: GeoPose.GeoPose, trainWagons: GeoPose.GeoPose[], trainPassengers: GeoPose.GeoPose[]): void {
        console.log("\r\n========== Space Train at Local Clock UNIX Time " + spaceTrain.validTime.toString() + "==========\r\n");
        console.log(spaceTrain.toJSON());
        trainWagons.forEach(function (wagon) {

            console.log("=-=-=-=-=- Wagon -=-=-=-=-=: " + wagon.poseID.id.substring(1 + wagon.poseID.id.lastIndexOf('/')) + "\r\n");
            console.log(wagon.toJSON());
            trainPassengers.forEach(function (passenger) {

                if (passenger.parentPoseID.id == wagon.poseID.id) {
                    console.log("---------- Passenger ----------: " + passenger.poseID.id.substring(1 + passenger.poseID.id.lastIndexOf('/')) + "\r\n");
                    console.log(passenger.toJSON());
                }
            })
        })
    }
}

//  - Verify that PROJ is configured and working
console.log("========== Checking PROJ ==========");
//    Source coordinates will be in Longitude/Latitude, WGS84
var source = proj4.Proj('EPSG:4326');
//    Destination coordinates in meters, global spherical mercators projection
var dest = proj4.Proj('EPSG:3785');

//  - Transform point coordinates
var p = proj4.toPoint([-76.0, 45.0, 11.0]);   
let q = proj4.transform(source, dest, p);
let r = proj4.transform(dest, source, q);
console.log("X : " + p.x + " \nY : " + p.y + " \nZ : " + p.z);
console.log("X : " + q.x + " \nY : " + q.y + " \nZ : " + q.z);
console.log("X : " + r.x + " \nY : " + r.y + " \nZ : " + r.z);

let d = new LTPENU.LTP_ENU();
let from = new Position.GeodeticPosition(-1.0, 52.0, 15.0);
let origin = new Position.GeodeticPosition(-1.00005, 52.0, 15.3);
let to = new Position.CartesianPosition(0, 0, 0);
d.GeodeticToEnu(from, origin, to);
console.log('from: lat: ' + from.lat.toString() + " lon: " + from.lon.toString() + " h: " + from.h.toString());
console.log('  to: x: ' + to.x.toString() + " y: " + to.y.toString() + " z: " + to.z.toString());

//  - Display some example GeoPoses
console.log("========== Example GeoPoses ==========");
let myYPRLocal = new Basic.BasicYPR("OS_GB: BasicYPR",
    new Position.GeodeticPosition(51.5, -1.5, 12.3),
    new Orientation.YPRAngles(1, 2, 3));
let json = myYPRLocal.toJSON();
console.log(json);
let myQLocal = new Basic.BasicQuaternion("OS_GB: BasicQ",
    new Position.GeodeticPosition(51.5, -1.5, 23.4),
    new Orientation.Quaternion(0.1, 0.2, 0.3, 1.0));
json = myQLocal.toJSON();
console.log(json);
let myALocal = new Advanced.Advanced("OS_GB: Advanced",
    new FrameTransform.Extrinsic("epsg", "5819", "[1.5, -1.5, 23.4]"),
    new Orientation.Quaternion(0.1, 0.2, 0.3, 1.0));
json = myALocal.toJSON();
console.log(json);
let myLLocal = new Local.Local("OS_GB: Local",
    new FrameTransform.Translation(9.0, 8.7, 7.6),
    new Orientation.YPRAngles(1, 2, 3));
json = myLLocal.toJSON();
console.log(json);

//  - A "Space Train" example with interpose linkages
console.log("========== Space Train ==========");
// Create Mars Express in the current International Celestial Reference Frame ICRF2 
let marsExpress = new Advanced.Advanced("https://example.com/nodes/MarsExpress/1", 
    new FrameTransform.Extrinsic("https://www.iers.org/",
        "icrf3",
        "{\"x\": 1234567890.9876,\"y\": 2345678901.8765, \"z\": 3456789012.7654}"),
    new Orientation.Quaternion(0, 0, 0, 1));
marsExpress.validTime = 1674767748003;

//  - Create four 10 m long wagons with identical seat layouts in frames local to Mars Express
//    and remember them in a wagons array
let wagons: Local.Local[] = [];
let wagon1 = new Local.Local("https://example.com/nodes/MarsExpress/1/Wagons/1",
    new FrameTransform.Translation(2.2, 0.82, -7.0),
    new Orientation.YPRAngles(0.2, 0.0, 23.0));
wagon1.parentPoseID = marsExpress.poseID;
wagons.push(wagon1);
let wagon2 = new Local.Local("https://example.com/nodes/MarsExpress/1/Wagons/2",
    new FrameTransform.Translation(12.2, 0.78, -7.0),
    new Orientation.YPRAngles(0.2, 0.0, 23.0));
wagon2.parentPoseID = marsExpress.poseID;
wagons.push(wagon2);
let wagon3 = new Local.Local("https://example.com/nodes/MarsExpress/1/Wagons/3",
    new FrameTransform.Translation(22.5, 0.77, -7.0),
    new Orientation.YPRAngles(0.2, 0.0, 23.0));
wagon3.parentPoseID = marsExpress.poseID;
wagons.push(wagon3);
let wagon4 = new Local.Local("https://example.com/nodes/MarsExpress/1/Wagons/4",
    new FrameTransform.Translation(33.2, 0.74, -7.0),
    new Orientation.YPRAngles(0.2, 0.0, 23.0));
wagon4.parentPoseID = marsExpress.poseID;
wagons.push(wagon4);

//  - Create passengers from the Cryptography Example Family (Alice, Bob, Carol, and Charlie)
//    in wagons 1 and 3 in local frames local to specific wagons and
//    remember them in a passenger list
let passengers: GeoPose.GeoPose[] = [];

//  - Alice is a clever thinker who has many questions and good ideas
let Alice = new Local.Local("https://example.com/nodes/MarsExpress/1/Passengers/Alice", new FrameTransform.Translation(2.2, 0.8, -7.0), new Orientation.YPRAngles(180.0, 1.0, 0.0));
Alice.parentPoseID = wagon1.poseID;
passengers.push(Alice);

//  - Bob is a nice fellow who guided us toward the frame transform in the early days
let Bob = new Local.Local("https://example.com/nodes/MarsExpress/1/Passengers/Bob", new FrameTransform.Translation(2.0, 0.8, -6.0), new Orientation.YPRAngles(180.0, 2.0, 0.0));
Bob.parentPoseID = wagon1.poseID;
passengers.push(Bob);

//  - Carol thinks that the Local GeoPose is needed and should be added to version 1.1.0
let Carol = new Local.Local("https://example.com/nodes/MarsExpress/1/Passengers/Carol", new FrameTransform.Translation(-5.0, 0.82, 6.0), new Orientation.YPRAngles(-2.0, 1.5, 0.0));
Carol.parentPoseID = wagon3.poseID;
passengers.push(Carol);

//  - Charlie is one of Carol's multiple personalities. Charlie does not believe in using any GeoPose not in the 1.0.0 standard
let Charlie =
    new Advanced.Advanced("https://charlie.com",
        new FrameTransform.Extrinsic(
            "https://ogc.org",
            "PROJCRS[\"GeoPose Local\",+GEOGCS[\"None)\"]+CS[Cartesian,3],+AXIS[\"x\",,ORDER[1],LENGTHUNIT[\"metre\",1]],+AXIS[\"y\",,ORDER[2],LENGTHUNIT[\"metre\",1]],+AXIS[\"z\",,ORDER[3],LENGTHUNIT[\"metre\",1]]+USAGE[AREA[\"+/-1000 m\"],BBOX[-1000,-1000,1000,1000],ID[\"GeoPose\",Local]]",
            "{\"x\": 1234567890.9876,\"y\": 2345678901.8765, \"z\": 3456789012.7654}"),
        new Orientation.Quaternion(0.0174509, 0.0130876, -0.0002284, 0.9997621));
Charlie.parentPoseID = wagon3.poseID;

//  - Charlie is going to do something time-dependent so we need to timestamp the current info
Charlie.validTime = marsExpress.validTime; // Use the Mars Express local clock
passengers.push(Charlie);

//  - Display the pose tree
Display.Output(marsExpress, wagons, passengers);

//  - After a minute, the Charlie personality decides that he must split from the Carol personality and
//    moves to the same seat in wagon 4.
//    Charlie's clock has an error of 327 millisecond with respect to marsExpress' clock
marsExpress.validTime = 1674767748003 + 60 * 1000;
Charlie.parentPoseID = wagon4.poseID;
//  - Charlie moved so we need to update his clock
Charlie.validTime = marsExpress.validTime + 327; // Use the Mars Express local clock

//  - Display new pose tree
Display.Output(marsExpress, wagons, passengers);

// - Done
console.log("Enter to exit")
input.read();
