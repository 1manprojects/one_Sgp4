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

        private const double a_Wgs72 = 6378.135; //!< double WGS72 const in Km
        private const double a_Wgs84 = 6378.137; //!< double WGS84 const in Km
        private const double f = 1.0 / 298.26;
        private const double delta = 1.0e-07;
        private const double f_2 = (f * (2.0 - f)) * (f * (2.0 - f));

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

                double lsr = time.getLocalSiderealTime(coordinate.getLongitude());
                Point3d groundLocation = coordinate.toECI(lsr);

                Point3d v = new Point3d();
                v.x = satPosData.getX() - groundLocation.x;
                v.y = satPosData.getY() - groundLocation.y;
                v.z = satPosData.getZ() - groundLocation.z;

                double r_lat = coordinate.getLatetude() * toRadians;

                double sin_lat = Math.Sin(r_lat);
                double cos_lat = Math.Cos(r_lat);
                double sin_srt = Math.Sin(lsr);
                double cos_srt = Math.Cos(lsr);

                
                double rs = sin_lat * cos_srt * v.x
                          + sin_lat * sin_srt * v.y
                          - cos_lat * v.z;
                double re = - sin_srt * v.x
                            + cos_srt * v.y;
                double rz = cos_lat * cos_srt * v.x
                            + cos_lat * sin_srt * v.y + sin_lat * v.z;

                double range = Math.Sqrt(rs * rs + re * re + rz * rz);
                double elevation = Math.Asin(rz / range);
                double azimuth = Math.Atan(-re / rs);

                if (rs > 0.0)
                {
                    azimuth += pi;
                }
                if (azimuth < 0.0)
                {
                    azimuth += twoPi;
                }

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
            \return Point3d containing range, azimuth, elevation
        */
        public static Point3d calcSphericalCoordinate(Coordinate coordinate,
            EpochTime time, Sgp4Data satPosData)
        {

            double lsr = time.getLocalSiderealTime(coordinate.getLongitude());
            Point3d groundLocation = coordinate.toECI(lsr);
            Point3d result = new Point3d();

            Point3d v = new Point3d();
            v.x = satPosData.getX() - groundLocation.x;
            v.y = satPosData.getY() - groundLocation.y;
            v.z = satPosData.getZ() - groundLocation.z;

            double r_lat = coordinate.getLatetude() * toRadians;

            double sin_lat = Math.Sin(r_lat);
            double cos_lat = Math.Cos(r_lat);
            double sin_srt = Math.Sin(lsr);
            double cos_srt = Math.Cos(lsr);


            double rs = sin_lat * cos_srt * v.x
                      + sin_lat * sin_srt * v.y
                      - cos_lat * v.z;
            double re = -sin_srt * v.x
                        + cos_srt * v.y;
            double rz = cos_lat * cos_srt * v.x
                        + cos_lat * sin_srt * v.y + sin_lat * v.z;

            result.x = Math.Sqrt(rs * rs + re * re + rz * rz);
            result.y = Math.Atan(-re / rs);
            result.z = Math.Asin(rz / result.x);

            if (rs > 0.0)
            {
                result.y += pi;
            }
            if (result.y < 0.0)
            {
                result.y += twoPi;
            }

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
            int wgsID = 0)
        {
            double sat_X = satPosData.getX();
            double sat_Y = satPosData.getY();
            double sat_Z = satPosData.getZ();

            double wgs_R = a_Wgs72;
            if (wgsID == 1)
                wgs_R = a_Wgs84;


            double r = Math.Sqrt( (sat_X * sat_X) + (sat_Y * sat_Y) );
            double latitude = AcTan(sat_Z , r);
            double phi;
            double c;
            do
            {
                phi = latitude;
                double c_sin2 = Math.Sin(phi) * Math.Sin(phi);
                c = 1.0 / Math.Sqrt(1.0 - (f_2 * c_sin2));
                latitude = AcTan(sat_Z + (wgs_R * c * f_2 * Math.Sin(phi)), r);
            }
            while (Math.Abs( latitude - phi) > delta);

            double longitude = AcTan(sat_Y, sat_X) - time.getLocalSiderealTime();
            
            double height = (r / Math.Cos(latitude)) - wgs_R * c;

            latitude = toDegrees * latitude;
            longitude = toDegrees * longitude;

            return new Coordinate(latitude, longitude, height);
        }

    }
}

