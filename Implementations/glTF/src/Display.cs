using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    internal static class Display
    {
        public static void Output(GeoPoseX.GeoPose marsExpress, List<GeoPoseX.Local> wagons, List<GeoPoseX.GeoPose> passengers)
        {
            Console.WriteLine("\r\n========== Mars Express at Local Clock UNIX Time " + marsExpress.validTime.timeValue + "==========\r\n");
            Console.WriteLine(marsExpress.ToJSON(""));
            foreach (GeoPoseX.GeoPose wagon in wagons)
            {
                Console.WriteLine("=-=-=-=-=- Wagon -=-=-=-=-=: " + wagon.poseID.id.Substring(1 + wagon.poseID.id.LastIndexOf('/')) + "\r\n");
                Console.WriteLine(wagon.ToJSON(""));
                foreach (GeoPoseX.GeoPose passenger in passengers)
                {
                    if (passenger.parentPoseID.id == wagon.poseID.id)
                    {
                        Console.WriteLine("---------- Passenger ----------: " + passenger.poseID.id.Substring(1 + passenger.poseID.id.LastIndexOf('/')) + "\r\n");
                        Console.WriteLine(passenger.ToJSON(""));
                    }

                }
            }
        }
    }
}
