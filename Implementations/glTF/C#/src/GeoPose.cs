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

// Implemention order: 5 - follows Orientation.
// This is the root of the GeoPose inheritance hierarchy.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Positions;
using FrameTransforms;
using Orientations;
using Extras;

namespace GeoPose
{
    /// <summary>
    /// The abstract root of the GeoPose Basic and Advanced classes.
    /// </summary>
    public abstract class GeoPose
    {
        // Optional and non-standard but conforming added property:
        //   an identifier unique within an application.
        public PoseID? poseID { get; set; } = null;

        // Optional and non-standard but conforming added property:
        //  a PoseID type identifier of another GeoPose in the direction of the root of a pose tree.
        public PoseID? parentPoseID { get; set; } = null;

        // Optional and non-standard but conforming added property:
        //   a poseID identifier in the direction of the root of a pose tree.
        public UnixTime? validTime { get; set; } = null;
        public abstract FrameTransform FrameTransform { get; set; }
        public abstract Orientation Orientation { get; set; }
        public abstract string ToJSON(string indent);
    }
}
