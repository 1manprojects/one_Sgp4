using One_Sgp4;
using System;
using System.Collections.Generic;

namespace OneSGP4_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //Parse three line element
            Tle tleItem = ParserTLE.parseTle(
                "1  8709U 76019A   17083.91463156 +.00000030 +00000-0 +91115-4 0  9995",
                "2  8709 069.6748 319.8382 0011306 339.5223 102.9699 13.71383337054833",
                "ISS 1");

            //Parse tle from file
            List<Tle> tleList = ParserTLE.ParseFile("tleData.txt");

            //Get TLE from Space-Track.org
            //list of satellites by their NORAD ID
            string[] noradIDs = { "8709", "43572" };
            try
            {
                One_Sgp4.SpaceTrack.GetSpaceTrack(noradIDs, "USERNAME", "PASSWORD");
            }
            catch { Console.Out.WriteLine("Error could not retrive TLE's from Space-Track, Login credentials might be wrong"); }


            //Create Time points
            EpochTime startTime = new EpochTime(DateTime.UtcNow);
            EpochTime anotherTime = new EpochTime(2018, 100.5); //(Year 2017, 100 day at 12:00 HH)
            EpochTime stopTime = new EpochTime(DateTime.UtcNow.AddHours(1));

            //Calculate Satellite Position and Speed
            One_Sgp4.Sgp4 sgp4Propagator = new Sgp4(tleItem, Sgp4.wgsConstant.WGS_84);
            //set calculation parameters StartTime, EndTime and caclulation steps in minutes
            sgp4Propagator.runSgp4Cal(startTime, stopTime, 1 / 30.0); // 1/60 => caclulate sat points every 2 seconds
            List<One_Sgp4.Sgp4Data> resultDataList = new List<Sgp4Data>();
            //Return Results containing satellite Position x,y,z (ECI-Coordinates in Km) and Velocity x_d, y_d, z_d (ECI-Coordinates km/s) 
            resultDataList = sgp4Propagator.getRestults();

            //Coordinate of an observer on Ground lat, long, height(in meters)
            One_Sgp4.Coordinate observer = new Coordinate(35.554595, 18.888574, 0);
            //Convert to ECI coordinate system
            One_Sgp4.Point3d eci = observer.toECI(startTime.getLocalSiderealTime());
            //Get Local SiderealTime for Observer
            double localSiderealTime = startTime.getLocalSiderealTime(observer.getLongitude());

            //Calculate SubPoint of Satellite on ground
            One_Sgp4.Coordinate satOnGround = One_Sgp4.SatFunctions.calcSatSubPoint(startTime, resultDataList[0], Sgp4.wgsConstant.WGS_84);

            //Calculate if Satellite is Visible for a certain Observer on ground at certain timePoint
            Boolean satelliteIsVisible = One_Sgp4.SatFunctions.isSatVisible(observer, 0.0, startTime, resultDataList[0]);

            //Calculate Sperical Coordinates from an Observer to Satellite
            //returns 3D-Point with range(km), azimuth(radians), elevation(radians) to the Satellite
            One_Sgp4.Point3d spherical = One_Sgp4.SatFunctions.calcSphericalCoordinate(observer, startTime, resultDataList[0]);
        }
    }
}
