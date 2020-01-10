/*
 * Copyright 2017 Nikolai Reed <reed@1manprojects.de>
 *
 * Licensed under the The MIT License (MIT)
 * You may obtain a copy of the License at
 *
 * https://tldrlegal.com/license/mit-license#summary
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;

namespace One_Sgp4
{
    /**
    * \brief InView Class definition.
    *
    * This class calculates the visibitly of the satellite to a coordinate,
    * the Spherical Coordinates and Satellite Sub-Points for the selected satelitte
    * For this the position vector of the satellite the time and
    * coordinates of the groundstation need to be available. From the starting
    * time of the orbit calculation the azimuth, elevation and range to the
    * ground station are calculated and if the satellite is in view at given
    * time it will return true. 
    */

    public class SatFunctions
    {
        public const double pi = Math.PI; //!< double constant Pi
        public const double twoPi = pi * 2.0; //!< double constant two Pi
        public const double toDegrees = 180.0 / pi; //!< double constant conversion to degree
        public const double toRadians = pi / 180.0; //!< double constant converstion to radians
        

        //! Ground constructor.
        /*!
            Empty constructor
        */
        public SatFunctions()
        {

        }

        //! Calculate visibility of a satellite from a point on Earth
        /*!
            \param Station to calcuate if satellite is in View
            \param TimeDate start time
            \param List<Sgp4Data> satellite position vector
            \param string name of the satellite
            \param double tick in witch time is increased by each step
            \return true if object is visible at given time and current location
        */
        public static bool isSatVisible(Coordinate coordinate, 
            double minElevation, EpochTime time, Sgp4Data satPosData)
        {
            Point3d res = calcSphericalCoordinate(coordinate, time, satPosData);
            double elevation = res.z;
            if (elevation >= minElevation)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //! Calculate Range, Azimuth and elevation for satellite
        //! for given time point and satellite position
        /*!
            \param Station to calcuate if satellite is in View
            \param TimeDate start time
            \param List<Sgp4Data> satellite position vector
            \return Point3d containing range(km), azimuth(degrees), elevation(degrees)
        */
        public static Point3d calcSphericalCoordinate(Coordinate coordinate,
            EpochTime time, Sgp4Data satPosData)
        {
            double lsr =  time.getLocalSiderealTime(coordinate.getLongitude());

            Point3d groundLocation = coordinate.toECI(time.getLocalSiderealTime());
            Point3d result = new Point3d();

            Point3d r = new Point3d();
            r.x = satPosData.getX() - groundLocation.x;
            r.y = satPosData.getY() - groundLocation.y;
            r.z = satPosData.getZ() - groundLocation.z;

            double r_lat = coordinate.getLatitude() * toRadians;

            double sin_lat = Math.Sin(r_lat);
            double cos_lat = Math.Cos(r_lat);
            double sin_theta = Math.Sin(lsr);
            double cos_theta = Math.Cos(lsr);

            double rs = sin_lat * cos_theta * r.x + sin_lat * sin_theta * r.y - cos_lat * r.z;
            double re = -sin_theta * r.x + cos_theta * r.y;
            double rz = cos_lat * cos_theta * r.x + cos_lat * sin_theta * r.y + sin_lat * r.z;

            result.x = Math.Sqrt((rs * rs) + (re * re) + (rz * rz));
            result.y = AcTan(-re , rs);
            result.z = Math.Asin(rz / result.x);

            result.y = (result.y + pi) * toDegrees;
            if (result.y > 360)
            {
                result.y = result.y - 360;
            }
            result.z = result.z * toDegrees;

            return result;
        }


        //! Calculate ArcTan(x/y) with correct result for
        //! cacluation of Sattelite subPoint
        /*!
            \param double x
            \param double y
            \return double ArcTan(x/y)
        */
        private static double AcTan(double x, double y)
        {
            double res;
            if (y == 0.0)
            {
                if (x > 0.0)
                {
                    res = pi / 2.0;
                }
                else
                {
                    res = 3.0 * pi / 2.0;
                }
            }
            else
            {
                if (y > 0.0)
                {
                    res = Math.Atan(x / y);
                }
                else
                {
                    res = pi + Math.Atan(x / y);
                }
            }
            return res;
        }

        //! Calculate Latitude, longitude and height for satellite on Earth
        //! at given time point and position of the satellite
        /*!
            \param TimeDate start time
            \param List<Sgp4Data> satellite position vector
            \param int WGS-Data to use 0 = WGS_72; 1 = WGS_84
            \param int Nr of iterations used to calculate the latetude
            \return Coordinate containing longitude, latitude, altitude/height
        */
        public static Coordinate calcSatSubPoint(EpochTime time, Sgp4Data satPosData,
            One_Sgp4.Sgp4.wgsConstant wgs)
        {
            double sat_X = satPosData.getX();
            double sat_Y = satPosData.getY();
            double sat_Z = satPosData.getZ();
            
            double f = WGS_72.f;
            double wgs_R = WGS_72.radiusEarthKM;
            if (wgs == Sgp4.wgsConstant.WGS_84) {
                f = WGS_84.f;
                wgs_R = WGS_84.radiusEarthKM;
            }            
            double delta = 1.0e-07;
            double f_2 = f * f;
            double e = 2 * f - f_2;            
            
            double r = Math.Sqrt( (sat_X * sat_X) + (sat_Y * sat_Y) );
            double latitude = AcTan(sat_Z , r);
            double c = 1.0;
            double height = 0.0;

            double phi;

            do
            {
                phi = latitude;
                c = 1.0 / (Math.Sqrt(1.0 - e * (Math.Sin(latitude) * Math.Sin(latitude))));
                latitude = AcTan(sat_Z + (wgs_R * c * e * Math.Sin(latitude)), r);
            }
            while (Math.Abs(latitude - phi) > delta);

            double longitude = AcTan(sat_Y, sat_X) - time.getLocalSiderealTime();
            height = (r / Math.Cos(latitude)) - (wgs_R * c);

            if (longitude < pi)
            {
                longitude += twoPi;
            }
            if (longitude > pi)
            {
                longitude -= twoPi;
            }

            latitude = toDegrees * latitude;
            longitude = toDegrees * longitude;

            return new Coordinate(latitude, longitude, height);
        }

        //! Calculate satellite position at a single point in time
        /*!
            \param tle of satellite
            \param EpochTime to calculate position
            \param int WGS-Data to use 0 = WGS_72; 1 = WGS_84
            \return SGP4Data containing satellite position data
        */
        public static Sgp4Data getSatPositionAtTime(Tle satellite, EpochTime atTime, Sgp4.wgsConstant wgs)
        {
            Sgp4 sgp4Propagator = new Sgp4(satellite, wgs);
            sgp4Propagator.runSgp4Cal(atTime, atTime, 1 / 60.0);
            return sgp4Propagator.getResults()[0];
        }

        //! Calculate Passes of a satellite for ceratin number of days from a starting time
        /*!
            \param Coordinate position of observer
            \param Tle satellite data
            \param EpochTime of startpoint
            \param int accuracy time between calculations default 15 seconds
            \param int number of days to calculate default 5 days
            \param int WGS-Data to use 0 = WGS_72; 1 = WGS_84 default WGS 84
            \return List<Pass> List of passes satellite is visible
        */
        public static List<Pass> CalculatePasses(Coordinate position, Tle satellite, EpochTime startTime, int accuracy = 15,
            int maxNumberOfDays = 5, Sgp4.wgsConstant wgs = Sgp4.wgsConstant.WGS_84)
        {
            List<Pass> results = new List<Pass>();
            EpochTime epoch = new EpochTime(startTime);
            EpochTime end = new EpochTime(startTime);
            end.addDays(maxNumberOfDays);

            while (epoch < end)
            {
                Sgp4Data satPos = getSatPositionAtTime(satellite, epoch, wgs);
                if (SatFunctions.isSatVisible(position, 0.0, epoch, satPos))
                {
                    Point3d spherical = SatFunctions.calcSphericalCoordinate(position, epoch, satPos);
                    PassDetail startDetails = new PassDetail(new EpochTime(epoch), spherical.z, spherical.y, spherical.x);
                    PassDetail maxEleDetails = new PassDetail(new EpochTime(epoch), spherical.z, spherical.y, spherical.x);
                    //double maxElevation = spherical.z;
                    epoch.addTick(accuracy);
                    satPos = getSatPositionAtTime(satellite, epoch, wgs);
                    while(spherical.z >= 0)
                    //while (SatFunctions.isSatVisible(position, 0.0, epoch, satPos))
                    {
                        
                        spherical = SatFunctions.calcSphericalCoordinate(position, epoch, satPos);
                        if (maxEleDetails.elevation < spherical.z)
                        {
                            maxEleDetails = new PassDetail(new EpochTime(epoch), spherical.z, spherical.y, spherical.x);
                        }
                        epoch.addTick(accuracy);
                        satPos = getSatPositionAtTime(satellite, epoch, wgs);
                    }
                    PassDetail endDetails = new PassDetail(new EpochTime(epoch), spherical.z, spherical.y, spherical.x);
                    results.Add(new One_Sgp4.Pass(position, startDetails, maxEleDetails, endDetails));
                }
                epoch.addTick(accuracy);
            }
            return results;            
        }
    }
}

