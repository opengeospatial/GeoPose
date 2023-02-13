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

// Implemention order: 7 - follows Basic GeoPose.
// This is the most general GeoPose - the largest part of the 20% part of a 80/20 solution.
// The difficult implementation is creating the interface layer between the
// Extrinsic specification and external authorities and data sources.


using System;
using System.Text;

using GeoPose;
using Extras;
using FrameTransforms;
using Orientations;

namespace Advanced
{ 
/// <summary>
/// Advanced GeoPose.
/// </summary>
public class Advanced : GeoPose.GeoPose
{
    internal Advanced()
    {

    }
    public Advanced(PoseID poseID, Extrinsic frameTransform, Quaternion orientation)
    {
        this.poseID = poseID;
        FrameTransform = frameTransform;
        Orientation = orientation;
    }

    /// <summary>
    /// A Frame Specification defining a frame with associated coordinate system whose Position is the origin.
    /// </summary>
    private Extrinsic _frameTransform = new Extrinsic();
    public override FrameTransform FrameTransform
    {
        get
        {
            return _frameTransform;
        }
        set
        {
            if (value.GetType() == typeof(Extrinsic))
            {
                _frameTransform = (Extrinsic)value;
            }
            // else throw expected extrinsic exception
        }
    }
    /// <summary>
    /// An Orientation specified as a unit quaternion.
    /// </summary>
    private Quaternion _orientation = new Quaternion();
    public override Orientation Orientation
    {
        get
        {
            return _orientation;
        }
        set
        {
            if (value.GetType() == typeof(Quaternion))
            {
                _orientation = (Quaternion)value;
            }
        }
    }
    /// <summary>
    /// Milliseconds of Unix time ticks (optional).
    /// </summary>
    //public long? ValidTime { get; set; } = 0;
    /// <summary>
    /// This function returns a Json encoding of an Advanced GeoPose
    /// </summary>
    public override string ToJSON(string indent)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{\r\n" + indent + "  ");
        if (validTime != null && validTime.timeValue != String.Empty)
        {
            sb.Append("\"validTime\": " + validTime.timeValue + ",\r\n" + indent + "  ");
        }
        if (poseID != null && poseID.id != String.Empty)
        {
            sb.Append("\"poseID\": \"" + poseID.id + "\",\r\n" + indent + "  ");
        }
        if (parentPoseID != null && parentPoseID.id != String.Empty)
        {
            sb.Append("\"parentPoseID\": \"" + parentPoseID.id + "\",\r\n" + indent + "  ");
        }
        sb.Append("\"frameSpecification\":\r\n" + indent + "  " + "{\r\n" + indent + "    \"authority\": \"" +
            ((Extrinsic)FrameTransform).authority.Replace("\"", "\\\"") + "\",\r\n" + indent + "    \"id\": \"" +
            ((Extrinsic)FrameTransform).id.Replace("\"", "\\\"") + "\",\r\n" + indent + "    \"parameters\": \"" +
            ((Extrinsic)FrameTransform).parameters.Replace("\"", "\\\"") + "\"\r\n" + indent + "  },\r\n" + indent + "  ");
        sb.Append("\"quaternion\":\r\n" + indent + "  {\r\n" + indent + "    \"x\":" + ((Quaternion)Orientation).x + ",\"y\":" +
            ((Quaternion)Orientation).y + ",\"z\":" +
            ((Quaternion)Orientation).z + ",\"w\":" +
            ((Quaternion)Orientation).w);
        sb.Append("\r\n" + indent + "  }\r\n" + indent + "}\r\n");
        return sb.ToString();
    }
}
}
