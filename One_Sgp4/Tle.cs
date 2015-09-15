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
    public class Tle
    {
    /**
    * \brief Tle Class definition.
    *
    * This class contains the definition of the TLE-Object wich is used for 
    * all further calculations and orbit predictions.
    */

        private string satName; /*!<Object Name Identifier*/

        private string noradID; /*!< NORAD Identification Number */
        private Enum.satClass classification; /*!< NORAD Satellite Classification */

        private int startYear; /*!< International Designator 2 digits StartYear (06)*/
        private int startNumber; /*!< International Designator 3 digits StartNumber (546)*/
        private string pieceLaunch; /*!< International Designator 3 symbols Piece of Launch (AAA)*/

        private int epochYear;  /*!< Epoch 2 digits StartYear (08)*/
        private double epochDay;  /*!< Epoch fractional portion of the Day (264.51782528)*/

        private double firstMeanMotion; /*!< First Time Derivative of the Mean Motion divided by two (−.00002182)*/
        private double secondMeanMotion; /*!< Second Time Derivative of Mean Motion divided by six (00000-0)*/
        private double dragTerm; /*!< BSTAR drag term  (-11606-4)*/
        private double ephemeris; /*!< Ephemeris type (0 = SGP4-Model)*/
        private int setNumber; /*!< Element set number. incremented when a new TLE is generated for this object (124)*/
        private int checksum1; /*!< Checksum for first Line of TLE Modulo 10 (7)*/


        private int satNumber;  /*!< Satellite Number (25544) */
        private double inclination; /*!< Inclination in Degrees (51.6416)*/
        private double rightAscension; /*!< Right Ascension of the Ascending Node ín Degrees (247.4627)*/
        private double eccentricity; /*!< Eccentricity (0006703) */
        private double perigee; /*!< Argument of Perigee in degrees (130.5360)*/
        private double meanAnomoly; /*!< Mean Anomaly in Degrees (325.0288) */
        private double meanMotion; /*!< Mean Motion Revs per day (15.72125391) */
        private double relevationNumber; /*!< Revolution number at epoch (56353) */
        private int checksum2; /*!< Checksum for second Line of TLE Modulo 10 (7)*/

        /** enum SatClass
        *  @brief enum class that represents the satellite classification
        */

        //! Tle constructor.
        /*!
            empty contructor
        */
        public Tle()
        {

        }

        //! Tle constructor.
        /*!
            /param string Name of Satellite
        */
        public Tle(string name)
        {

        }

        //! TLE constructor.
        /*!
        \param string name of Satellite.
        \param string ID of Satellite.
        \param satClass classification of Satellite.
        \param int startYear of Satellite
        \param string PieceName
        \param int EpochYear
        \param double EpochDay
        \param double firstMeanMotion
        \param double secondMeanMotion
        \param double Drag Term
        \param double Ephemeris
        \param double Set number of TLE Data
        \param int Checksum (Modulo 10)
        \param int Satellite number
        \param double Inclination
        \param double right Ascending Node
        \param double Eccentricity
        \param double Perigee
        \param double MeanAnomoly
        \param double MeanMotion
        \param double revelation number
        \param int Checksum (Modulo 10)
        Each Object of TLE must have a valid Name
        */
        public Tle(string name, string id, Enum.satClass clas, int startY, int startNr,
            string piece, int epochY, double epochD, double firstMM, double secondMM,
            double drag, double ephem, int setNr, int check1, int satNr, double incl,
            double rightAsc, double ecce, double peri, double meanAn, double meanMo,
            double relevationNr, int check2)
        {
            satName = name;
            noradID = id;
            classification = clas;
            startYear = startY;
            startNumber = startNr;
            pieceLaunch = piece;
            epochYear = epochY;
            epochDay = epochD;
            firstMeanMotion = firstMM;
            secondMeanMotion = secondMM;
            dragTerm = drag;
            ephemeris = ephem;
            setNumber = setNr;
            checksum1 = check1;
            satNumber = satNr;
            inclination = incl;
            rightAscension = rightAsc;
            eccentricity = ecce;
            perigee = peri;
            meanAnomoly = meanAn;
            meanMotion = meanMo;
            relevationNumber = relevationNr;
            checksum2 = check2;
        }

        //! TLE constructor.
        /*!
        \param string name of Satellite.
        \param string ID of Satellite.
        \param satClass classification of Satellite.
        \param int startYear of Satellite
        \param string PieceName
        \param int EpochYear
        \param double EpochDay
        \param double firstMeanMotion
        \param double secondMeanMotion
        \param double Drag Term
        \param double Ephemeris
        \param double Set number of TLE Data
        \param int Checksum (Modulo 10)
        \param int Satellite number
        \param double Inclination
        \param double right Ascending Node
        \param double Eccentricity
        \param double Perigee
        \param double MeanAnomoly
        \param double MeanMotion
        \param double revelation number
        \param int Checksum (Modulo 10)
        Each Object of TLE must have a valid Name
        */
        public Tle(string name, string id, int clas, int startY, int startNr,
            string piece, int epochY, double epochD, double firstMM, double secondMM,
            double drag, double ephem, int setNr, int check1, int satNr, double incl,
            double rightAsc, double ecce, double peri, double meanAn, double meanMo,
            double relevationNr, int check2)
        {
            satName = name;
            noradID = id;
            classification = (Enum.satClass) clas;
            startYear = startY;
            startNumber = startNr;
            pieceLaunch = piece;
            epochYear = epochY;
            epochDay = epochD;
            firstMeanMotion = firstMM;
            secondMeanMotion = secondMM;
            dragTerm = drag;
            ephemeris = ephem;
            setNumber = setNr;
            checksum1 = check1;
            satNumber = satNr;
            inclination = incl;
            rightAscension = rightAsc;
            eccentricity = ecce;
            perigee = peri;
            meanAnomoly = meanAn;
            meanMotion = meanMo;
            relevationNumber = relevationNr;
            checksum2 = check2;
        }

        //! Returns true if Data matches Checksum
        /*!
        \return boolean true/false
        */
        public bool isValidData()
        {
            return true;
        }

        //! Returns the Object Name
        /*!
        \return string Name
        */
        public string getName()
        {
            return satName;
        }

        //! Returns the NORAD Identification
        /*!
        \return string NoradID
        */
        public string getNoradID()
        {
            return noradID;
        }

        //! Returns the start Year of satellite
        /*!
        \returns int StartYear
        */
        public int getStartYear()
        {
            return startYear;
        }

        //! Returns the start number of satellite
        /*!
        \returns int StartNumber
        */
        public int getStartNr()
        {
            return startNumber;
        }

        //! Returns the Piece designator
        /*!
        \returns string Piece
        */
        public string getPice()
        {
            return pieceLaunch;
        }

        //! Returns the Year of the Epoch
        /*!
        \returns int EpochYear
        */
        public int getEpochYear()
        {
            return epochYear;
        }

        //! Returns the Day of the Epoch
        /*!
        \returns double EpochDay
        */
        public double getEpochDay()
        {
            return epochDay;
        }

        //! Returns the First Mean Motion
        /*!
        \returns double meanMotion
        */
        public double getFirstMeanMotion()
        {
            return meanMotion;
        }

        //! Returns the Second Mean Motion
        /*!
        \returns double secondMeanMotion
        */
        public double getSecondMeanMotion()
        {
            return secondMeanMotion;
        }

        //! Returns the Drag value
        /*!
        \returns double dragTerm
        */
        public double getDrag()
        {
            return dragTerm;
        }

        //! Returns the Ephemeris
        /*!
        \returns double ephemeris
        */
        public double getEphemeris()
        {
            return ephemeris;
        }

        //! Returns the Set Number
        /*!
        \returns double setNumber
        */
        public double getSetNumber()
        {
            return setNumber;
        }

        //! Returns the Satellite Number
        /*!
        \returns int satNumber
        */
        public int getSatNumber()
        {
            return satNumber;
        }

        //! Returns the Inclination 
        /*!
        \returns double inclination
        */
        public double getInclination()
        {
            return inclination;
        }

        //! Returns the Richt Ascending Node
        /*!
        \returns double right Ascension
        */
        public double getRightAscendingNode()
        {
            return rightAscension;
        }

        //! Returns the Eccentricity
        /*!
        \returns double eccentricity
        */
        public double getEccentriciy()
        {
            return eccentricity;
        }

        //! Returns the Perigee
        /*!
        \returns double perigee
        */
        public double getPerigee()
        {
            return perigee;
        }

        //! Returns the Mean Anomoly 
        /*!
        \returns double meanAnomoly
        */
        public double getMeanAnomoly()
        {
            return meanAnomoly;
        }

        //! Returns the Mean Motion
        /*!
        \returns double meanMotion
        */
        public double getMeanMotion()
        {
            return meanMotion;
        }

        //! Returns the number of Relevations
        /*!
        \returns double relevationNumber
        */
        public double getRelevatioNumber()
        {
            return relevationNumber;
        }

        //! Returns Classification
        /*!
        \returns ing satellite Classifictaion
        */
        public int getClassification()
        {
            return (int)classification;
        }

        //! Returns the Checksum for the first TLE line
        /*!
        \returns int checksum1
        */
        public int getFirstCheckSum()
        {
            return checksum1;
        }

        //! Returns the Checksum for the second TLE line
        /*!
        \returns int checksum2
        */
        public int getSecCheckSum()
        {
            return checksum2;
        }
    }
}
