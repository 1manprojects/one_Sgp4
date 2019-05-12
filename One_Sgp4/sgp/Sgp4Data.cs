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


namespace One_Sgp4
{
    public class Sgp4Data
    {

    /**
    * \brief Sgp4Data Class definition.
    *
    * This class holds the calculated position and velocity vectors of the
    * satellite.
    */

        private int satNumber = -1; /*!< Satellite Number of Data */
        private Point3d pos; /*!< 3D-pointData for position Data */
        private Point3d vel; /*!< 3D-pointData for velocity Data */

        //! SGP4-Data constructor.
        /*!
        \param integer SateliteNumber.
        */
        public Sgp4Data( int satNr = -1)
        {
            satNumber = satNr;
            pos = new Point3d();
            vel = new Point3d();
        }

        //! set the Satellite Number.
        /*!
        \param int Nr.
        */
        public void setSatNumber(int Nr)
        {
            satNumber = Nr;
        }

        //! set the X-Coordinate for Position.
        /*!
        \param double X
        */
        public void setX(double x)
        {
            pos.x = x;
        }

        //! set the Y-Coordinate for Position.
        /*!
        \param double Y
        */
        public void setY(double y)
        {
            pos.y = y;
        }

        //! set the Z-Coordinate for Position.
        /*!
        \param double Z
        */
        public void setZ(double z)
        {
            pos.z = z;
        }

        //! set the x-Velocity.
        /*!
        \param double xdot
        */
        public void setXDot(double xdot)
        {
            vel.x = xdot;
        }

        //! set the y-Velocity.
        /*!
        \param double ydot
        */
        public void setYDot(double ydot)
        {
            vel.y = ydot;
        }

        //! set the z-Velocity.
        /*!
        \param double zdot
        */
        public void setZDot(double zdot)
        {
            vel.z = zdot;
        }


        //! Returns the Satellite Number.
        /*!
        \return double SateliteNr
        */
        public int getSatNumber()
        {
            return satNumber;
        }

        //! Returns the Position Data as a 3d-Point.
        /*!
        \return double x, y, z;
        */
        public Point3d getPositonData()
        {
            return pos;
        }

        //! Returns the velocity Data as a 3d-Point.
        /*!
        \return double x, y, z;
        */
        public Point3d getVelocityData()
        {
            return vel;
        }

        //! Returns the X Position.
        /*!
        \return double x
        */
        public double getX()
        {
            return pos.x;
        }

        //! Returns the Y Position.
        /*!
        \return double y
        */
        public double getY()
        {
            return pos.y;
        }

        //! Returns the Z Position.
        /*!
        \return double z
        */
        public double getZ()
        {
            return pos.z;
        }

        //! Returns the X Velocity.
        /*!
        \return double xDot
        */
        public double getXDot()
        {
            return vel.x;
        }

        //! Returns the Y Velocity.
        /*!
        \return double yDot
        */
        public double getYDot()
        {
            return vel.y;
        }

        //! Returns the Z Velocity.
        /*!
        \return double zDot
        */
        public double getZDot()
        {
            return vel.z;
        }

        //! Returns position as String.
        /*!
        \string double X Y Z
        */
        public string getPosDataString()
        {
            string result;
            result = pos.x.ToString() + " :: " + pos.y.ToString() + " :: " +
                pos.z.ToString();
            return result;
        }

        //! Returns velocity as String.
        /*!
        \string double XDot YDot ZDot
        */
        public string getVelDataString()
        {
            string result;
            result = vel.x.ToString() + " :: " + vel.y.ToString() + " :: " +
                vel.z.ToString();
            return result;
        }

        //! Clears all Data
        /*!
        */
        public void clear()
        {
            vel = null;
            pos = null;
        }
    }
}
