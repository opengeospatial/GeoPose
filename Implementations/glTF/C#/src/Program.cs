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

using GeoPose;

// Create Mars Express in the current International Celestial Reference Frame ICRF2 
Advanced marsExpress = new Advanced(new PoseID("https://example.com/nodes/MarsExpress/1"),
    new Extrinsic("https://www.iers.org/", "icrf3", "{\"x\": 1234567890.9876,\"y\": 2345678901.8765, \"z\": 3456789012.7654}"),
    new Quaternion(0,0,0,1));
marsExpress.validTime = new UnixTime(1674767748003);

// Create four 10 m long wagons with identical seat layouts in frames local to Mars Express and remember them in a wagon list
List<Local> wagons = new List<Local>();
Local wagon1 = new Local("https://example.com/nodes/MarsExpress/1/Wagons/1", new Translation( 2.2, 0.82, -7.0), new YPRAngles(0.2, 0.0, 23.0));
wagon1.parentPoseID = marsExpress.poseID;
wagons.Add(wagon1);
Local wagon2 = new Local("https://example.com/nodes/MarsExpress/1/Wagons/2", new Translation(12.2, 0.78, -7.0), new YPRAngles(0.2, 0.0, 23.4));
wagon2.parentPoseID = marsExpress.poseID;
wagons.Add(wagon2);
Local wagon3 = new Local("https://example.com/nodes/MarsExpress/1/Wagons/3", new Translation(22.5, 0.77, -7.0), new YPRAngles(0.2, 0.0, 21.0));
wagon3.parentPoseID = marsExpress.poseID;
wagons.Add(wagon3);
Local wagon4 = new Local("https://example.com/nodes/MarsExpress/1/Wagons/4", new Translation(33.2, 0.74, -7.0), new YPRAngles(0.2, 0.0, 42.0));
wagon4.parentPoseID = marsExpress.poseID;
wagons.Add(wagon4);

// Create passengers from the SWG in wagons 1 and 3 in local frames local to specific wagons and remember them in a passenger list
List<GeoPoseX.GeoPose> passengers = new List<GeoPoseX.GeoPose>();

//  - Jerome is a clever thinker who has many questions and good ideas
Local jerome = new Local("https://example.com/nodes/MarsExpress/1/Passengers/Jerome", new Translation(2.2, 0.8, -7.0), new YPRAngles(180.0, 1.0, 0.0));
jerome.parentPoseID = wagon1.poseID;
passengers.Add(jerome);

//  - Josh is a nice fellow who guided us towrd the frame transform in the early days
Local josh = new Local("https://example.com/nodes/MarsExpress/1/Passengers/Josh", new Translation(2.0, 0.8, -6.0), new YPRAngles(180.0, 2.0, 0.0));
josh.parentPoseID = wagon1.poseID;
passengers.Add(josh);

//  - Steve thinks that the Local GeoPose is needed and should be added to version 1.1.0
Local steve = new Local("https://example.com/nodes/MarsExpress/1/Passengers/Steve", new Translation(-5.0, 0.82, 6.0), new YPRAngles(-2.0, 1.5, 0.0));
steve.parentPoseID = wagon3.poseID;
passengers.Add(steve);

//  - Carl is one of Steve's multiple personalities who does not believe in using any GeoPose not in the 1.0.0 standard
Advanced carl =
    new Advanced(new PoseID("https://carlsmyth.com"),
    new Extrinsic(
        "https://ogc.org",
        "PROJCRS[\"GeoPose Local\",+GEOGCS[\"None)\"]+CS[Cartesian,3],+AXIS[\"x\",,ORDER[1],LENGTHUNIT[\"metre\",1]],+AXIS[\"y\",,ORDER[2],LENGTHUNIT[\"metre\",1]],+AXIS[\"z\",,ORDER[3],LENGTHUNIT[\"metre\",1]]+USAGE[AREA[\"+/-1000 m\"],BBOX[-1000,-1000,1000,1000],ID[\"GeoPose\",Local]]",
        "{\"x\": 1234567890.9876,\"y\": 2345678901.8765, \"z\": 3456789012.7654}"),
    new Quaternion(0.0174509, 0.0130876, -0.0002284, 0.9997621));
carl.parentPoseID = wagon3.poseID;

// Carl is going to do something time-dependent so we need to timestamp the current info
carl.validTime = marsExpress.validTime; // Use the Mars Express local clock
passengers.Add(carl);

// Display the pose tree
Example.Display.Output(marsExpress, wagons, passengers);

// After a minute, the Carl personality decides that he must split from the Steve personality and moves to the same seat in wagon 4
marsExpress.validTime = new UnixTime(1674767748003 + 60*1000 + 327);
carl.parentPoseID = wagon4.poseID;
// Carl moved so we need to update the validTime
carl.validTime = marsExpress.validTime; // Use the Mars Express local clock

// Display new pose tree
Example.Display.Output(marsExpress, wagons, passengers);

