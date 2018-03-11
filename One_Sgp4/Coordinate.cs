﻿/*
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
   * \brief GeoCoordinate class
   *
   * This class defnies the GeoCoordinates of Latetude, Longitude, hight and the
   * conversions to Earth Centerd Inertial.
   */
    public class Coordinate
    {
        public const double pi = Math.PI; //!< double constant Pi
        public const double twoPi = pi * 2.0; //!< double constant two Pi
        public const double toDegrees = 180.0 / pi; //!< double constant conversion to degree
        public const double toRadians = pi / 180.0; //!< double constant converstion to radians

        private double latetude; //!< double Latetude in degree
        private double longitude; //!< double longitude in degree
        private double height; //!< double height in meters

        private const double a_Wgs72 = 6378.135; //!< double WGS72 const in Km
        private const double a_Wgs84 = 6378.137; //!< double WGS84 const in Km
        private const double f = 1.0 / 298.26;

        //! GeoCoordinate constructor.
        /*!
        \param double latetude
        \param double longitude
        \param double hight default 0.0
        */
        public Coordinate(double _latetude, double _longitude,
                             double _height = 0.0)
        {
            latetude = _latetude;
            longitude = _longitude;
            height = _height;
        }

        //! Returns the GeoCoordinates as a string
        /*!
        \return string GeoCoordinate
        */
        public string toString()
        {
            string ret = "Lat: " + latetude +
                        " Long: " + longitude +
                        " Hight: " + height;
            return ret;
        }

        //! Returns the Latetude
        /*!
        \return double Latetude
        */
        public double getLatetude()
        {
            return latetude;
        }

        //! Returns the Longitude
        /*!
        \return double longitude
        */
        public double getLongitude()
        {
            return longitude;
        }

        //! Returns the height
        /*!
        \return double height
        */
        public double getHeight()
        {
            return height;
        }

        //! Convert to ECI
        /*!
        \param double SidrealTime
        \return point3D ECI-Position vector of the Coordinate
        */
        public Point3d toECI(double siderealTime)
        {
            double srt = siderealTime;
            double lat_rad = toRadians * latetude;
            Point3d eciPos = new Point3d();

            double c = 1.0 / Math.Sqrt(1.0 + f * (f - 2.0) *
                       (Math.Sin(lat_rad) * Math.Sin(lat_rad)));
            double s = (1.0 - f) * (1.0 - f) * c;
            eciPos.x = a_Wgs72 * c * Math.Cos(lat_rad) * Math.Cos(srt);
            eciPos.y = a_Wgs72 * c * Math.Cos(lat_rad) * Math.Sin(srt);
            eciPos.z = a_Wgs72 * s * Math.Sin(lat_rad);

            return eciPos;
        }


    }
}
