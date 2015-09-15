/*
 * Copyright 2015 Nikolai Reed <reed@1manprojects.de>
 *
 * Licensed under the GNU Lesser General Public License v3 (LGPL-3)
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.gnu.org/licenses/lgpl-3.0.de.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace One_Sgp4
{
    /**
    * \brief InView Class definition.
    *
    * This class calculates the contact windows and/or the Spherical Coordinates
    * for each satellite to every groundstation.
    * For this the position vector of the satellite and the
    * coordinates of the groundstation need to be available. From the starting
    * time of the orbit calculation the azimuth, elevation and range to the
    * ground station are calculated and if the satellite is in view a new
    * ContactWindow will be created.  
    */
    class calculateContacts
    {
        public const double pi = Math.PI; //!< double constant Pi
        public const double twoPi = pi * 2.0; //!< double constant two Pi
        public const double toDegrees = 180.0 / pi; //!< double constant conversion to degree
        public const double toRadians = pi / 180.0; //!< double constant converstion to radians

        //! Ground constructor.
        /*!
            Empty constructor
        */
        public calculateContacts()
        {

        }

        //! Calculate ContactWindows for satellite and groundstations
        /*!
            \param Station to calcuate if satellite is in View
            \param TimeDate start time
            \param List<Sgp4Data> satellite position vector
            \param string name of the satellite
            \param double tick in witch time is increased by each step
            \return true if object is visible at given time and current location
        */
        public static bool calcContactWindows(Coordinate coordinate, 
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
        /*!
            \param Station to calcuate if satellite is in View
            \param TimeDate start time
            \param List<Sgp4Data> satellite position vector
            \param string name of the satellite
            \param double tick in witch time is increased by each step
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

    }
}

