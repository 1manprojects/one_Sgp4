/*
 * Copyright 2017 Nikolai Reed <reed@1manprojects.de>
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

namespace One_Sgp4
{
    public class Sgp4
    {
        /**
  * \brief GeoCoordinate class
  *
  * This class defnies the GeoCoordinates of Latetude, Longitude, hight and the
  * conversions to Earth Centerd Inertial.
  */

        private Tle tleElementData; //!< Tle tleElementData to calculate orbit from 
        private Sgp4Rec satCalcData; //!< Sgp4Rec satCalcData stores the results

        private double ainv = 0.0;
        private double ao = 0.0;
        private double argpm = 0.0;
        private double cnodm = 0.0;
        private double con42 = 0.0;
        private double cosim = 0.0, sinim = 0.0, cosomm = 0.0, sinomm = 0.0;
        private double cosio = 0.0;
        private double cosio2 = 0.0;
        private double day = 0.0;
        private double dndt = 0.0;
        private double eccsq = 0.0;
        private double einv = 0.0;
        private double em = 0.0, emsq = 0.0, gam = 0.0, rtemsq = 0.0;
        private double inclm = 0.0;
        private double mm = 0.0;
        private double omegam = 0.0;
        private double omeosq = 0.0;
        private double posq = 0.0;
        private double rad = 57.29577951308230;
        private double rp = 0.0;
        private double rteosq = 0.0;
        private double s1 = 0.0, s2 = 0.0, s3 = 0.0, s4 = 0.0, s5 = 0.0, s6 = 0.0;
        private double s7 = 0.0;
        private double sinio = 0.0;
        private double snodm = 0.0;
        private double ss1 = 0.0, ss2 = 0.0, ss3 = 0.0;
        private double ss4 = 0.0, ss5 = 0.0, ss6 = 0.0, ss7 = 0.0;
        private double sz1 = 0.0;
        private double sz2 = 0.0, sz3 = 0.0, sz11 = 0.0, sz12 = 0.0, sz13 = 0.0;
        private double sz21 = 0.0, sz22 = 0.0, sz23 = 0.0, sz31 = 0.0, sz32 = 0.0;
        private double sz33 = 0.0, nm = 0.0;
        private double z1 = 0.0;
        private double z2 = 0.0, z3 = 0.0, z11 = 0.0, z12 = 0.0, z13 = 0.0;
        private double z21 = 0.0, z22 = 0.0, z23 = 0.0, z31 = 0.0, z32 = 0.0;
        private double z33 = 0.0;

        private double e1 = 0.0;
        private double nuo = 0.0;
        private int year = 0;
        private double xpdotp = 229.1831180523293;

        private List<Sgp4Data> resultOrbitData;

        private double radiusEarthKm;
        private double j2, j3, j4, j3oj2, mu, xke, tumin;

        private TimeZone timeZone = TimeZone.CurrentTimeZone;

        private const double twoPi = 2.0 * Math.PI; //!< double constant two Pi
        public const double toRadians = Math.PI / 180.0; //!< double constant converstion to radians

        private EpochTime _startTime;
        private EpochTime _stopTime;
        private double _tick;

        public event EventHandler ThreadDone;

        public enum satClass
        {
            UNCLASSIFIED = 0, //!< int 0 unclassified satellite 
            CLASSIFIED = 1, //!< int 1 classified satellite
            SECRET = 2 //!< int 2 secret satellite
        };

        //! SGP4 constructor.
        /*!
        \param tle Two Line Elements
        \param int GravConst 0 = WGS72, 1 = WGS82
        initializes the Orbit-Calculation model
        */
        public Sgp4(Tle data, int wgsConstant)
        {
            setGrav(wgsConstant);

            tleElementData = data;
            satCalcData = new Sgp4Rec();

            resultOrbitData = new List<Sgp4Data>();

            //Load TLE Data in sg4Rec Class for calculation
            satCalcData.rec_satnum = data.getSatNumber();
            satCalcData.rec_epochyr = data.getEpochYear();
            satCalcData.rec_epochdays = data.getEpochDay();
            satCalcData.rec_bstar = data.getDrag();
            satCalcData.rec_inclo = data.getInclination();
            satCalcData.rec_omegao = data.getRightAscendingNode();
            satCalcData.rec_ecco = data.getEccentriciy();
            satCalcData.rec_argpo = data.getPerigee();
            satCalcData.rec_mo = data.getMeanAnomoly();
            satCalcData.rec_no = data.getMeanMotion();

            satCalcData.rec_no = satCalcData.rec_no / xpdotp;

            satCalcData.rec_a = Math.Pow(satCalcData.rec_no * tumin, (-2.0 / 3.0));
            satCalcData.rec_ndot = satCalcData.rec_ndot / (xpdotp * 1440.0);
            satCalcData.rec_nddot = satCalcData.rec_nddot / (xpdotp * 1440.0 * 1440);

            satCalcData.rec_inclo = satCalcData.rec_inclo / rad;
            satCalcData.rec_omegao = satCalcData.rec_omegao / rad;
            satCalcData.rec_argpo = satCalcData.rec_argpo / rad;
            satCalcData.rec_mo = satCalcData.rec_mo / rad;

            //Initalize newton rhapson iteration
            newtonm(satCalcData.rec_ecco, satCalcData.rec_mo, e1, nuo);
            
            satCalcData.rec_alta = satCalcData.rec_a *
                (1.0 + satCalcData.rec_ecco * satCalcData.rec_ecco) - 1.0;
            satCalcData.rec_altp = satCalcData.rec_a *
                (1.0 - satCalcData.rec_ecco * satCalcData.rec_ecco) - 1.0;

            //check Yeahr to find the the right Date
            //Currently will only work until 2058
            //Currently oldest man made object Vangard1 Launched 1958
            if (satCalcData.rec_epochyr < 58)
                year = satCalcData.rec_epochyr + 2000;
            else
                year = satCalcData.rec_epochyr + 1900;

            // Epoch time
            satCalcData.rec_eptime = (year - 1950) * 365 + (year - 1949) / 4
                    + satCalcData.rec_epochdays;
            
            EpochTime satTime = new EpochTime(satCalcData.rec_epochyr,
                satCalcData.rec_epochdays);

            satCalcData.rec_mjdsatepoch = satTime.toJulianDate();
            satCalcData.rec_mjdsatepoch = satCalcData.rec_mjdsatepoch - 2400000.5;

            satCalcData.rec_init = 1;
            satCalcData.neo.neo_t = 0.0;

            sgp4Init(satCalcData.rec_satnum, year, satCalcData.rec_mjdsatepoch - 33281.0);
            //end;
        }


        public void setStart(EpochTime starttime, EpochTime stoptime, double tick)
        {
            _startTime = starttime;
            _stopTime = stoptime;
            _tick = tick;
        }

        public void starThread()
        {
            runSgp4Cal(_startTime, _stopTime, _tick);
            if (ThreadDone != null)
                ThreadDone(this, EventArgs.Empty);
        }


        //! clear all Data.
        /*!
            clears all calculated and stored data
        */
        public void clear()
        {
            resultOrbitData.Clear();
            resultOrbitData = null;
            tleElementData = null;
            satCalcData.dso = null;
            satCalcData.neo = null;
            satCalcData = null;
        }

        //! Run the sgp4 calculations
        /*!
        \param EpochTime starttime 
        \param EpochTime stoptime
        \param double step in minutes
        calculates the orbit of the satellite starting from start to stoptime 
        */
        public void runSgp4Cal(EpochTime starttime, EpochTime stoptime, double step)
        {
            EpochTime time = starttime;
            int startY = starttime.getYear();
            int stopY = stoptime.getYear();
            if (startY < 1900)
            {
                if (startY < 50)
                    startY = startY + 2000;
                else
                    startY = startY + 1900;
            }
            if (stopY < 1900)
            {
                if (stopY < 50)
                    stopY = stopY + 2000;
                else
                    stopY = stopY + 1900;
            }
            double starD = starttime.getEpoch();
            double stopD = stoptime.getEpoch();

            satCalcData.rec_srtime = (startY - 1950) * 365 + (startY - 1949) / 4 + starD;
            satCalcData.rec_sptime = (stopY - 1950) * 365 + (stopY - 1949) / 4 + stopD;
            satCalcData.rec_deltamin = step;

            double temp_t = (satCalcData.rec_srtime - satCalcData.rec_eptime) * 1440.0;
            satCalcData.neo.neo_t = temp_t;

            double stopTime = (satCalcData.rec_sptime - satCalcData.rec_eptime) * 1440.0 +
                satCalcData.rec_deltamin;

            while (temp_t < stopTime)
            {
                resultOrbitData.Add(calcSgp4());
                temp_t = temp_t + satCalcData.rec_deltamin;
                satCalcData.neo.neo_t = temp_t;
            }
        }

        public void runSgp4Cal(int startY, double starD, int stopY,
            double stopD, double step)
        {
	        if (startY < 1900)
	        {
		        if (startY < 50)
			        startY = startY + 2000;
		        else
			        startY = startY + 1900;
	        }
	        if (stopY < 1900)
	        {
		        if (stopY < 50)
			        stopY = stopY + 2000;
		        else
			        stopY = stopY + 1900;
	        }

	        satCalcData.rec_srtime = (startY - 1950) * 365 + (startY - 1949) / 4 + starD;
	        satCalcData.rec_sptime = (stopY - 1950) * 365 + (stopY - 1949) / 4 + stopD;
	        satCalcData.rec_deltamin = step;



	        double temp_t = (satCalcData.rec_srtime - satCalcData.rec_eptime) * 1440.0;
            satCalcData.neo.neo_t = temp_t;

	        double stopTime = (satCalcData.rec_sptime - satCalcData.rec_eptime) * 1440.0 + 
		        satCalcData.rec_deltamin;

	        while (temp_t < stopTime)
	        {
                resultOrbitData.Add(calcSgp4());
		        temp_t = temp_t + satCalcData.rec_deltamin;
                satCalcData.neo.neo_t = temp_t;
	        }
        }

        Sgp4Data calcSgp4()
        {
	        double am, axnl, aynl, betal, cnod, cos2u, coseo1 = 0.0;
	        double cosi, cosip, cosisq, cossu, cosu, delm, delomg;
	        double ecose, el2, eo1, esine;
	        double cosim, emsq, sinim;
	        double argpdf, pl, mrt;
	        double mvt, rdotl, rl, rvdot, rvdotl, sin2u, sineo1 = 0.0;
	        double sini, sinip, sinsu, sinu, snod, su, t2, t3, t4, tem5, temp;
	        double temp1, temp2, tempa, tempe, templ;
	        double u, ux, uy, uz, vx, vy, vz;
	        double xinc;
	        double xl, xlm;
	        double xmdf, xmx, xmy, omegadf;
	        double xnode;
            double tc, x2o3;
	        int ktr;

	        x2o3 = 2.0 / 3.0;

            double vkmpersec = radiusEarthKm * xke / 60.0;

            satCalcData.rec_error = 0;

            xmdf = satCalcData.rec_mo + satCalcData.neo.neo_mdot * satCalcData.neo.neo_t;
            argpdf = satCalcData.rec_argpo + satCalcData.neo.neo_argpdot * satCalcData.neo.neo_t;
            omegadf = satCalcData.rec_omegao + satCalcData.neo.neo_omegadot * satCalcData.neo.neo_t;
	        argpm = argpdf;
	        mm = xmdf;
            t2 = satCalcData.neo.neo_t * satCalcData.neo.neo_t;
            omegam = omegadf + satCalcData.neo.neo_omegacf * t2;
            tempa = 1.0 - satCalcData.neo.neo_cc1 * satCalcData.neo.neo_t;
            tempe = satCalcData.rec_bstar * satCalcData.neo.neo_cc4 * satCalcData.neo.neo_t;
            templ = satCalcData.neo.neo_t2cof * t2;

	        if (satCalcData.neo.neo_isimp != 1) 
	        {
		        delomg = satCalcData.neo.neo_omgcof * satCalcData.neo.neo_t;
		        delm = satCalcData.neo.neo_xmcof *
                    (Math.Pow((1.0 + satCalcData.neo.neo_eta * Math.Cos(xmdf)), 3) - 
			        satCalcData.neo.neo_delmo);
		        temp = delomg + delm;
		        mm = xmdf + temp;
		        argpm = argpdf - temp;
		        t3 = t2 * satCalcData.neo.neo_t;
		        t4 = t3 * satCalcData.neo.neo_t;
		        tempa = tempa - satCalcData.neo.neo_d2 * t2
                    - satCalcData.neo.neo_d3 * t3 - satCalcData.neo.neo_d4 * t4;
		        tempe = tempe + satCalcData.rec_bstar * satCalcData.neo.neo_cc5
                    * (Math.Sin(mm) - satCalcData.neo.neo_sinmao);
		        templ = templ + satCalcData.neo.neo_t3cof * t3 + t4 *
                    (satCalcData.neo.neo_t4cof + satCalcData.neo.neo_t * 
			        satCalcData.neo.neo_t5cof);
	        }
	        nm = satCalcData.rec_no;
	        em = satCalcData.rec_ecco;
	        inclm = satCalcData.rec_inclo;
	        if ( satCalcData.neo.neo_method == 2 ) {
		        tc = satCalcData.neo.neo_t;
		        deepSpaceContr(satCalcData.dso.dso_irez,
                    satCalcData.dso.dso_d2201,
                    satCalcData.dso.dso_d2211, satCalcData.dso.dso_d3210,
			        satCalcData.dso.dso_d3222, satCalcData.dso.dso_d4410,
                    satCalcData.dso.dso_d4422, satCalcData.dso.dso_d5220,
			        satCalcData.dso.dso_d5232, satCalcData.dso.dso_d5421,
                    satCalcData.dso.dso_d5433, satCalcData.dso.dso_dedt,
			        satCalcData.dso.dso_del1, satCalcData.dso.dso_del2,
                    satCalcData.dso.dso_del3, satCalcData.dso.dso_didt,
			        satCalcData.dso.dso_dmdt, satCalcData.dso.dso_dnodt,
                    satCalcData.dso.dso_domdt, satCalcData.rec_argpo,
			        satCalcData.neo.neo_argpdot, satCalcData.neo.neo_t,
                    tc, satCalcData.dso.dso_gsto, satCalcData.dso.dso_xfact,
			        satCalcData.dso.dso_xlamo, satCalcData.rec_no);
	        }
	        // Check if mean motion is less than or equal to zero
	        if (nm <= 0.0)
	        {
                satCalcData.rec_error = 2;
                throw new System.ArgumentException(tleElementData.getName() + " -MeanMotion is zero or less", "Sgp4Calculation");
		        // throw an exception only if this is a fatal condition
		        // which may result in a divide by zero error, otherwise
		        // try and recover
	        }

	        am = Math.Pow((xke / nm), x2o3) * tempa * tempa;
	        nm = xke / Math.Pow(am, 1.5);
	        // subtract drag effects on the eccentricity
	        em = em - tempe;

	        // Check for eccentricity being out of bounds
	        if ((em >= 1.0) || (em < -0.001) || (am < 0.95))
	        {
                satCalcData.rec_error = 1;
                throw new System.ArgumentException(tleElementData.getName() + " -Eccentricity is out of bounds", "Sgp4Calculation");
	        }
	        // If it is less than zero, try and correct by making eccentricity a
	        // small value
	        if (em < 0.0)
		        em = 1.0e-6;
	        mm = mm + satCalcData.rec_no * templ;
	        xlm = mm + argpm + omegam;
	        emsq = em * em;
	        temp = 1.0 - emsq;
	        omegam = modfunc(omegam, twoPi);
	        argpm = modfunc(argpm, twoPi);
	        xlm = modfunc(xlm, twoPi);
	        mm = modfunc(xlm - argpm - omegam, twoPi);

	        sinim = Math.Sin(inclm);
	        cosim = Math.Cos(inclm);

	        satCalcData.rec_ep = em;
	        satCalcData.rec_xincp = inclm;
	        satCalcData.rec_argpp = argpm;
	        satCalcData.rec_omegap = omegam;
	        satCalcData.rec_mp = mm;
	        sinip = sinim;
	        cosip = cosim;
	        if (satCalcData.neo.neo_method == 2) {
                tc = satCalcData.neo.neo_t;
		        deepSpacePeriodic(satCalcData.dso.dso_e3, satCalcData.dso.dso_ee2,
                            satCalcData.dso.dso_peo,
						    satCalcData.dso.dso_pgho, satCalcData.dso.dso_pho,
						    satCalcData.dso.dso_pinco, satCalcData.dso.dso_plo,
						    satCalcData.dso.dso_se2, satCalcData.dso.dso_se3,
						    satCalcData.dso.dso_sgh2, satCalcData.dso.dso_sgh3,
						    satCalcData.dso.dso_sgh4, satCalcData.dso.dso_sh2,
						    satCalcData.dso.dso_sh3, satCalcData.dso.dso_si2,
						    satCalcData.dso.dso_si3, satCalcData.dso.dso_sl2,
						    satCalcData.dso.dso_sl3, satCalcData.dso.dso_sl4,
						    satCalcData.neo.neo_t, satCalcData.dso.dso_xgh2,
						    satCalcData.dso.dso_xgh3, satCalcData.dso.dso_xgh4,
						    satCalcData.dso.dso_xh2, satCalcData.dso.dso_xh3,
						    satCalcData.dso.dso_xi2, satCalcData.dso.dso_xi3,
						    satCalcData.dso.dso_xl2, satCalcData.dso.dso_xl3,
						    satCalcData.dso.dso_xl4, satCalcData.dso.dso_zmol,
						    satCalcData.dso.dso_zmos, 0);

		        // Correct for negative inclination
		        if (satCalcData.rec_xincp < 0.0)
		        {
			        satCalcData.rec_xincp = -satCalcData.rec_xincp;
			        satCalcData.rec_omegap = satCalcData.rec_omegap + Math.PI;
			        satCalcData.rec_argpp = satCalcData.rec_argpp - Math.PI;
		        }

		        // Another eccentricity check
		        if ((satCalcData.rec_ep < 0.0) || (satCalcData.rec_ep > 1.0))
		        {
                    satCalcData.rec_error = 1;
                    throw new System.ArgumentException(tleElementData.getName() + " -Eccentricity is out of bounds", "Sgp4Calculation");
		        }
	        }

	        if (satCalcData.neo.neo_method == 2) {
		        sinip = Math.Sin(satCalcData.rec_xincp);
		        cosip = Math.Cos(satCalcData.rec_xincp);
		        satCalcData.neo.neo_aycof = -0.5 * j3oj2 * sinip;
		        satCalcData.neo.neo_xlcof = -0.25 * j3oj2 * sinip * (3.0 + 5.0 * cosip)
			        / (1.0 + cosip);
	        }
	        axnl = satCalcData.rec_ep * Math.Cos(satCalcData.rec_argpp);
	        temp = 1.0 / (am * (1.0 - satCalcData.rec_ep * satCalcData.rec_ep));
	        aynl = satCalcData.rec_ep * Math.Sin(satCalcData.rec_argpp) + temp
		        * satCalcData.neo.neo_aycof;
	        xl = satCalcData.rec_mp + satCalcData.rec_argpp + satCalcData.rec_omegap + temp
		        * satCalcData.neo.neo_xlcof * axnl;

	        /* --------------------- solve kepler's equation --------------- */
	        u = modfunc(xl - satCalcData.rec_omegap, twoPi);
	        eo1 = u;
	        tem5 = 9999.9;
	        ktr = 1;

	        while (( Math.Abs(tem5) >= 1.0e-12) && (ktr <= 10)) 
	        {
		        sineo1 = Math.Sin(eo1);
		        coseo1 = Math.Cos(eo1);
		        tem5 = 1.0 - coseo1 * axnl - sineo1 * aynl;
		        tem5 = (u - aynl * coseo1 + axnl * sineo1 - eo1) / tem5;
		        if (Math.Abs(tem5) >= 0.95)
			        tem5 = tem5 > 0.0 ? 0.95 : -0.95;
		        eo1 = eo1 + tem5;
		        ktr = ktr + 1;
	        }

	        ecose = axnl * coseo1 + aynl * sineo1;
	        esine = axnl * sineo1 - aynl * coseo1;
	        el2 = axnl * axnl + aynl * aynl;
	        pl = am * (1.0 - el2);

	        if (pl < 0.0) 
	        {
                satCalcData.rec_error = 4;
                throw new System.InvalidOperationException(tleElementData.getName() + " -No data could be generated");
		        // This error results in no data generated
	        }
	        else 
	        {
		        rl = am * (1.0 - ecose);
		        rdotl = Math.Sqrt(am) *esine / rl;
		        rvdotl = Math.Sqrt(pl) / rl;
		        betal = Math.Sqrt(1.0 - el2);
		        temp =esine / (1.0 + betal);
		        sinu = am / rl * (sineo1 - aynl - axnl * temp);
		        cosu = am / rl * (coseo1 - axnl + aynl * temp);
		        su = Math.Atan2(sinu, cosu);
		        sin2u = (cosu + cosu) * sinu;
		        cos2u = 1.0 - 2.0 * sinu * sinu;
		        temp = 1.0 / pl;
		        temp1 = 0.5 * j2 * temp;
		        temp2 = temp1 * temp;

		        /* -------------- update for short period periodics ------------ */
		        if (satCalcData.neo.neo_method == 2) 
		        {
			        cosisq = cosip * cosip;
			        satCalcData.neo.neo_con41 = 3.0 * cosisq - 1.0;
			        satCalcData.neo.neo_x1mth2 = 1.0 - cosisq;
			        satCalcData.neo.neo_x7thm1 = 7.0 * cosisq - 1.0;
		        }

		        mrt = rl * (1.0 - 1.5 * temp2 * betal * satCalcData.neo.neo_con41)
			        + 0.5 * temp1 * satCalcData.neo.neo_x1mth2 * cos2u;
		        su = su - 0.25 * temp2 * satCalcData.neo.neo_x7thm1 * sin2u;
		        xnode = satCalcData.rec_omegap + 1.5 * temp2 * cosip * sin2u;
		        xinc = satCalcData.rec_xincp + 1.5 * temp2 * cosip * sinip * cos2u;
		        mvt = rdotl - nm * temp1 * satCalcData.neo.neo_x1mth2 * sin2u / xke;
		        rvdot = rvdotl
			        + nm
			        * temp1
			        * (satCalcData.neo.neo_x1mth2 * cos2u + 1.5 * satCalcData.neo.neo_con41)
			        / xke;

		        /* --------------------- orientation vectors ------------------- */
		        sinsu = Math.Sin(su);
		        cossu = Math.Cos(su);
		        snod = Math.Sin(xnode);
		        cnod = Math.Cos(xnode);
		        sini = Math.Sin(xinc);
		        cosi = Math.Cos(xinc);
		        xmx = -snod * cosi;
		        xmy = cnod * cosi;
		        ux = xmx * sinsu + cnod * cossu;
		        uy = xmy * sinsu + snod * cossu;
		        uz = sini * sinsu;
		        vx = xmx * cossu - cnod * sinsu;
		        vy = xmy * cossu - snod * sinsu;
		        vz = sini * cossu;

		        /* ------------------- position and velocity ------------------- */

		        satCalcData.rec_r[0] = mrt * ux;
		        satCalcData.rec_r[1] = mrt * uy;
		        satCalcData.rec_r[2] = mrt * uz;
		        satCalcData.rec_v[0] = mvt * ux + rvdot * vx;
		        satCalcData.rec_v[1] = mvt * uy + rvdot * vy;
		        satCalcData.rec_v[2] = mvt * uz + rvdot * vz;
	        }

	        if (satCalcData.rec_error > 0) 
	        {
                if (satCalcData.rec_error == 4)
                {

                }
	        }

	        Sgp4Data data = new Sgp4Data(satCalcData.rec_satnum);

            data.setX(satCalcData.rec_r[0] * radiusEarthKm);
            data.setY(satCalcData.rec_r[1] * radiusEarthKm);
            data.setZ(satCalcData.rec_r[2] * radiusEarthKm);

            data.setXDot(satCalcData.rec_v[0] * vkmpersec);
            data.setYDot(satCalcData.rec_v[1] * vkmpersec);
            data.setZDot(satCalcData.rec_v[2] * vkmpersec);

            return data;
        }


        /* -----------------------------------------------------------------------------
	    *
        * procedure deepSpacePeriodic
	    *
	    *
	    * SPACETRACK REPORT NO. 3
	    * Revisiting Spacetrack Report #3: Rev 2
	    * David A. Vallado, Paul Cawford, Richard Hujsak, T.S. Kelso
	    * http://www.celestrak.com/publications/AIAA/2006-6753/
	    *
	    * this procedure provides deep space long period periodic contributions
	    * to the mean elements. by design, these periodics are zero at epoch.
	    * this used to be dscom which included initialization, but it's really a
	    * recurring function.
	    *
	    * author : david vallado 719-573-2600 28 jun 2005
	    *
	    * inputs :
	    * e3 -
	    * ee2 -
	    * peo -
	    * pgho -
	    * pho -
	    * pinco -
	    * plo -
	    * se2 , se3 , sgh2, sgh3, sgh4, sh2, sh3, si2, si3, sl2, sl3, sl4 -
	    * t -
	    * xh2, xh3, xi2, xi3, xl2, xl3, xl4 -
	    * zmol -
	    * zmos -
	    *
	    * outputs :
	    * ep - eccentricity 0.0 - 1.0
	    * inclp - inclination
	    * nodep - right ascension of ascending node
	    * argpp - argument of perigee
	    * mp - mean anomaly
	    *
	    * locals :
	    * alfdp -
	    * betdp -
	    * cosip , sinip , cosop , sinop ,
	    * dalf -
	    * dbet -
	    * dls -
	    * f2, f3 -
	    * pe -
	    * pgh -
	    * ph -
	    * pinc -
	    * pl -
	    * sel , ses , sghl , sghs , shl , shs , sil , sinzf , sis ,
	    * sll , sls
	    * xls -
	    * xnoh -
	    * zf -
	    * zm -
	    *
	    * coupling :
	    * none.
	    *
	    * references :
	    * hoots, roehrich, norad spacetrack report #3 1980
	    * hoots, norad spacetrack report #6 1986
	    * hoots, schumacher and glover 2004
	    * vallado, crawford, hujsak, kelso 2006
	    *
	    *
	    * modified by Nikolai Reed 2015
	    ----------------------------------------------------------------------------*/
        void deepSpacePeriodic(double e3, double ee2, double peo, double pgho, double pho,
						        double pinco, double plo, double se2, double se3,
						        double sgh2, double sgh3, double sgh4, double sh2,
						        double sh3, double si2, double si3, double sl2, double sl3,
						        double sl4, double t, double xgh2, double xgh3,
						        double xgh4, double xh2, double xh3, double xi2,
						        double xi3, double xl2, double xl3, double xl4,
						        double zmol, double zmos, int init)
        { /* --------------------- local variables ------------------------ */
	        double alfdp, betdp, cosip, cosop, dalf, dbet, dls, f2, f3;
	        double pe, pgh, ph, pinc, pl, sel, ses, sghl, sghs, shll, shs;
	        double sil, sinip, sinop, sinzf, sis, sll, sls, xls, xnoh, zf, zm;
	        double zel, zes, znl, zns;

	        /* ---------------------- Constants ----------------------------- */
	        zns = 1.19459e-5;
	        zes = 0.01675;
	        znl = 1.5835218e-4;
	        zel = 0.05490;

	        /* --------------- calculate time varying periodics ----------- */
	        zm = zmos + zns * t;
	        if (init != 0)
		        zm = zmos;
	        zf = zm + 2.0 * zes * Math.Sin(zm);
	        sinzf = Math.Sin(zf);
	        f2 = 0.5 * sinzf * sinzf - 0.25;
	        f3 = -0.5 * sinzf * Math.Cos(zf);
	        ses = se2 * f2 + se3 * f3;
	        sis = si2 * f2 + si3 * f3;
	        sls = sl2 * f2 + sl3 * f3 + sl4 * sinzf;
	        sghs = sgh2 * f2 + sgh3 * f3 + sgh4 * sinzf;
	        shs = sh2 * f2 + sh3 * f3;
	        zm = zmol + znl * t;
	        if (init != 0)
		        zm = zmol;
	        zf = zm + 2.0 * zel * Math.Sin(zm);
	        sinzf = Math.Sin(zf);
	        f2 = 0.5 * sinzf * sinzf - 0.25;
	        f3 = -0.5 * sinzf * Math.Cos(zf);
	        sel = ee2 * f2 + e3 * f3;
	        sil = xi2 * f2 + xi3 * f3;
	        sll = xl2 * f2 + xl3 * f3 + xl4 * sinzf;
	        sghl = xgh2 * f2 + xgh3 * f3 + xgh4 * sinzf;
	        shll = xh2 * f2 + xh3 * f3;
	        pe = ses + sel;
	        pinc = sis + sil;
	        pl = sls + sll;
	        pgh = sghs + sghl;
	        ph = shs + shll;

	        if (init == 0) {
		        pe = pe - peo;
		        pinc = pinc - pinco;
		        pl = pl - plo;
		        pgh = pgh - pgho;
		        ph = ph - pho;
		        satCalcData.rec_xincp = satCalcData.rec_xincp + pinc;
		        satCalcData.rec_ep = satCalcData.rec_ep + pe;
		        sinip = Math.Sin(satCalcData.rec_xincp);
		        cosip = Math.Cos(satCalcData.rec_xincp);
		        if (satCalcData.rec_xincp >= 0.2) {
			        ph = ph / sinip;
			        pgh = pgh - cosip * ph;
			        satCalcData.rec_argpp = satCalcData.rec_argpp + pgh;
			        satCalcData.rec_omegap = satCalcData.rec_omegap + ph;
			        satCalcData.rec_mp = satCalcData.rec_mp + pl;
		        }
		        else {
			        sinop = Math.Sin(satCalcData.rec_omegap);
			        cosop = Math.Cos(satCalcData.rec_omegap);
			        alfdp = sinip * sinop;
			        betdp = sinip * cosop;
			        dalf = ph * cosop + pinc * cosip * sinop;
			        dbet = -ph * sinop + pinc * cosip * cosop;
			        alfdp = alfdp + dalf;
			        betdp = betdp + dbet;
			        satCalcData.rec_omegap = modfunc(satCalcData.rec_omegap, twoPi);
			        xls = satCalcData.rec_mp + satCalcData.rec_argpp + cosip * satCalcData.rec_omegap;
			        dls = pl + pgh - pinc * satCalcData.rec_omegap * sinip;
			        xls = xls + dls;
			        xnoh = satCalcData.rec_omegap;
			        satCalcData.rec_omegap = Math.Atan2(alfdp, betdp);
			        if (Math.Abs(xnoh - satCalcData.rec_omegap) > Math.PI)
			        if (Math.Abs(xnoh - satCalcData.rec_omegap) > Math.PI)
			        if (satCalcData.rec_omegap < xnoh)
				        satCalcData.rec_omegap = satCalcData.rec_omegap + twoPi;
			        else
				        satCalcData.rec_omegap = satCalcData.rec_omegap - twoPi;
			        satCalcData.rec_mp = satCalcData.rec_mp + pl;
			        satCalcData.rec_argpp = xls - satCalcData.rec_mp - cosip * satCalcData.rec_omegap;
		        }
	        }
        }

        
	/*-----------------------------------------------------------------------------
	*
	* procedure deepSpaceContr
	*
	*
	* SPACETRACK REPORT NO. 3
	* Revisiting Spacetrack Report #3: Rev 2
	* David A. Vallado, Paul Cawford, Richard Hujsak, T.S. Kelso
	* http://www.celestrak.com/publications/AIAA/2006-6753/
	*
	* this procedure provides deep space contributions to mean elements for
	* perturbing third body. these effects have been averaged over one
	* revolution of the sun and moon. for earth resonance effects, the
	* effects have been averaged over no revolutions of the satellite.
	* (mean motion)
	*
	* author : david vallado 719-573-2600 28 jun 2005
	*
	* inputs :
	* d2201, d2211, d3210, d3222, d4410, d4422, d5220, d5232, d5421, d5433 -
	* dedt -
	* del1, del2, del3 -
	* didt -
	* dmdt -
	* dnodt -
	* domdt -
	* irez - flag for resonance 0-none, 1-one day, 2-half day
	* argpo - argument of perigee
	* argpdot - argument of perigee dot (rate)
	* t - time
	* tc -
	* gsto - gst
	* xfact -
	* xlamo -
	* no - mean motion
	* atime -
	* em - eccentricity
	* ft -
	* argpm - argument of perigee
	* inclm - inclination
	* xli -
	* mm - mean anomaly
	* xni - mean motion
	* nodem - right ascension of ascending node
	*
	* outputs :
	* atime -
	* em - eccentricity
	* argpm - argument of perigee
	* inclm - inclination
	* xli -
	* mm - mean anomaly
	* xni -
	* nodem - right ascension of ascending node
	* dndt -
	* nm - mean motion
	*
	* locals :
	* delt -
	* ft -
	* theta -
	* x2li -
	* x2omi -
	* xl -
	* xldot -
	* xnddt -
	* xndt -
	* xomi -
	*
	* coupling :
	* none -
	*
	* references :
	* hoots, roehrich, norad spacetrack report #3 1980
	* hoots, norad spacetrack report #6 1986
	* hoots, schumacher and glover 2004
	* vallado, crawford, hujsak, kelso 2006
    *
    *
    * modified by Nikolai Reed 2015
	----------------------------------------------------------------------------*/
        private void deepSpaceContr(int irez, double d2201, double d2211, double d3210,
						        double d3222, double d4410, double d4422, double d5220,
						        double d5232, double d5421, double d5433, double dedt,
						        double del1, double del2, double del3, double didt,
						        double dmdt, double dnodt, double domdt, double argpo,
						        double argpdot, double t, double tc, double gsto,
						        double xfact, double xlamo, double no)
        {
	        int iretn, iret;
	        double delt, ft = 0.0, theta, x2li, x2omi, xl, xldot = 0.0, xnddt = 0.0;
	        double xndt = 0.0, xomi, g22, g32, g44, g52, g54, fasx2, fasx4, fasx6, rptim;
	        double step2, stepn, stepp;

	        fasx2 = 0.13130908;
	        fasx4 = 2.8843198;
	        fasx6 = 0.37448087;
	        g22 = 5.7686396;
	        g32 = 0.95240898;
	        g44 = 1.8014998;
	        g52 = 1.0508330;
	        g54 = 4.4108898;
	        rptim = 4.37526908801129966e-3;
	        stepp = 720.0;
	        stepn = -720.0;
	        step2 = 259200.0;

	        dndt = 0.0;
	        theta = modfunc(gsto + tc * rptim, twoPi);
	        em = em + dedt * t;
	        inclm = inclm + didt * t;
	        argpm = argpm + domdt * t;
	        omegam = omegam + dnodt * t;
	        mm = mm + dmdt * t;

	        ft = 0.0;
	        satCalcData.dso.dso_atime = 0.0;
	        if (irez != 0) {
		        if ((satCalcData.dso.dso_atime == 0.0)
			        || ((t >= 0.0) && (satCalcData.dso.dso_atime < 0.0))
			        || ((t < 0.0) && (satCalcData.dso.dso_atime >= 0.0))) {
			        if (t >= 0.0)
				        delt = stepp;
			        else
				        delt = stepn;
			        satCalcData.dso.dso_atime = 0.0;
			        satCalcData.dso.dso_xni = no;
			        satCalcData.dso.dso_xli = xlamo;
		        }
		        iretn = 381;
		        iret = 0;
		        while (iretn == 381) {
			        if ((Math.Abs(t) < Math.Abs(satCalcData.dso.dso_atime))
				        || (iret == 351)) {
				        if (t >= 0.0)
					        delt = stepn;
				        else
					        delt = stepp;
				        iret = 351;
				        iretn = 381;
			        }
			        else {
				        if (t > 0.0) 
					        delt = stepp;
				        else
					        delt = stepn;
				        if (Math.Abs(t - satCalcData.dso.dso_atime) >= stepp) {
					        iret = 0;
					        iretn = 381;
				        }
				        else {
					        ft = t - satCalcData.dso.dso_atime;
					        iretn = 0;
				        }
			        }

			        if (irez != 2) {
				        xndt = del1 * Math.Sin(satCalcData.dso.dso_xli - fasx2) + del2
					        * Math.Sin(2.0 * (satCalcData.dso.dso_xli - fasx4))
					        + del3
					        * Math.Sin(3.0 * (satCalcData.dso.dso_xli - fasx6));
				        xldot = satCalcData.dso.dso_xni + xfact;
				        xnddt = del1 * Math.Cos(satCalcData.dso.dso_xli - fasx2) + 2.0
					        * del2
					        * Math.Cos(2.0 * (satCalcData.dso.dso_xli - fasx4))
					        + 3.0 * del3
					        * Math.Cos(3.0 * (satCalcData.dso.dso_xli - fasx6));
				        xnddt = xnddt * xldot;
			        }
			        else {
				        xomi = argpo + argpdot * satCalcData.dso.dso_atime;
				        x2omi = xomi + xomi;
				        x2li = satCalcData.dso.dso_xli + satCalcData.dso.dso_xli;
				        xndt = d2201 * Math.Sin(x2omi + satCalcData.dso.dso_xli - g22)
					        + d2211 * Math.Sin(satCalcData.dso.dso_xli - g22)
					        + d3210
					        * Math.Sin(xomi + satCalcData.dso.dso_xli - g32)
					        + d3222
					        * Math.Sin(-xomi + satCalcData.dso.dso_xli - g32)
					        + d4410 * Math.Sin(x2omi + x2li - g44) + d4422
					        * Math.Sin(x2li - g44) + d5220
					        * Math.Sin(xomi + satCalcData.dso.dso_xli - g52)
					        + d5232
					        * Math.Sin(-xomi + satCalcData.dso.dso_xli - g52)
					        + d5421 * Math.Sin(xomi + x2li - g54) + d5433
					        * Math.Sin(-xomi + x2li - g54);
				        xldot = satCalcData.dso.dso_xni + xfact;
				        xnddt = d2201
					        * Math.Cos(x2omi + satCalcData.dso.dso_xli - g22)
					        + d2211
					        * Math.Cos(satCalcData.dso.dso_xli - g22)
					        + d3210
					        * Math.Cos(xomi + satCalcData.dso.dso_xli - g32)
					        + d3222
					        * Math.Cos(-xomi + satCalcData.dso.dso_xli - g32)
					        + d5220
					        * Math.Cos(xomi + satCalcData.dso.dso_xli - g52)
					        + d5232
					        * Math.Cos(-xomi + satCalcData.dso.dso_xli - g52)
					        + 2.0
					        * (d4410 * Math.Cos(x2omi + x2li - g44) + d4422
					        * Math.Cos(x2li - g44) + d5421
					        * Math.Cos(xomi + x2li - g54) + d5433
					        * Math.Cos(-xomi + x2li - g54));
				        xnddt = xnddt * xldot;
			        }

			        if (iretn == 381) {
				        satCalcData.dso.dso_xli = satCalcData.dso.dso_xli + xldot * delt
					        + xndt * step2;
				        satCalcData.dso.dso_xni = satCalcData.dso.dso_xni + xndt * delt
					        + xnddt * step2;
				        satCalcData.dso.dso_atime = satCalcData.dso.dso_atime + delt;
			        }
		        }

		        nm = satCalcData.dso.dso_xni + xndt * ft + xnddt * ft * ft * 0.5;
		        xl = satCalcData.dso.dso_xli + xldot * ft + xndt * ft * ft * 0.5;
		        if (irez != 1) {
			        mm = xl - 2.0 * omegam + 2.0 * theta;
			        dndt = nm - no;
		        }
		        else {
			        mm = xl - omegam - argpm + theta;
			        dndt = nm - no;
		        }

		        nm = no + dndt;
	        }
        } // end dsspace


        /*-----------------------------------------------------------------------------
     *
     *                             procedure sgp4init
     *
     *  this procedure initializes variables for sgp4. the fix to conform to
     *    ver 3.01 consisted only of adding a var so the 'o elements would be
     *    passed back for deep space initialization cases.
     *
     *  author        : david vallado                  719-573-2600    1 mar 2001
     *
     *  inputs        :
     *    satn        - satellite number
     *    year        - year of observation                    1950-2049
     *    epoch       - epoch time in days from jan 0, 1950. 0 hr
     *
     *  outputs       :
     *    no          - mean motion
     *    omegao      - longitude of ascending node
     *    init        - flag for first pass 1-yes, 0-not first pass
     *    nevalues    - near earth common values for subsequent calls
     *    dsvalues    - deep space common values for calls
     *
     *  locals        :
     *    cnodm       -
     *    snodm       -
     *    cosim       -
     *    sinim       -
     *    cosomm      -
     *    sinomm      -
     *    cc1sq       -
     *    cc2         -
     *    cc3         -
     *    coef        -
     *    coef1       -
     *    cosio4      -
     *    day         -
     *    dndt        -
     *    em          -
     *    emsq        -
     *    eeta        -
     *    etasq       -
     *    gam         -
     *    argpm       -
     *    ndem        -
     *    inclm       - inclination
     *    mm          - mean anomaly
     *    nm          - mean motion
     *    perige      -
     *    pinvsq      -
     *    psisq       -
     *    qzms24      -
     *    rtemsq      -
     *    s1, s2, s3, s4, s5, s6, s7          -
     *    sfour       -
     *    ss1, ss2, ss3, ss4, ss5, ss6, ss7         -
     *    sz1, sz2, sz3
     *    sz11, sz12, sz13, sz21, sz22, sz23, sz31, sz32, sz33        -
     *    tc          -
     *    temp        -
     *    temp1, temp2, temp3       -
     *    tsi         -
     *    xpidot      -
     *    xhdot1      -
     *    z1, z2, z3          -
     *    z11, z12, z13, z21, z22, z23, z31, z32, z33         -
     *
     *  coupling      :
     *    initl       -
     *    dscom       -
     *    dpper       -
     *    dsinit      -
     *
     *  references    :
     *    norad spacetrack report no.3 "models for propagation of norad element sets"
     *              1 nov 1980:  felix r. hoots, ronald l. roehrich
     *
     ----------------------------------------------------------------------------*/
        public void sgp4Init(int satn, int year, double epoch)
        {
            /* --------------------- local variables ------------------------ */
        double cc1sq = 0.0, cc2 = 0.0, cc3 = 0.0, coef = 0.0;
        double coef1 = 0.0, cosio4 = 0.0;
        double eeta = 0.0, etasq = 0.0;
        double perige = 0.0;
        double pinvsq = 0.0, psisq = 0.0, qzms24 = 0.0;
        double sfour = 0.0;
        ss1 = 0.0;
        ss2 = 0.0;
        ss3 = 0.0;
        ss4 = 0.0;
        ss5 = 0.0;
        ss6 = 0.0;
        ss7 = 0.0;
        sz1 = 0.0;
        sz2 = 0.0;
        sz3 = 0.0;
        sz11 = 0.0;
        sz12 = 0.0;
        sz13 = 0.0;
        sz21 = 0.0;
        sz22 = 0.0;
        sz23 = 0.0;
        sz31 = 0.0;
        sz32 = 0.0;
        sz33 = 0.0;
        z1 = 0.0;
        z2 = 0.0;
        z3 = 0.0;
        z11 = 0.0;
        z12 = 0.0;
        z13 = 0.0;
        z21 = 0.0;
        z22 = 0.0;
        z23 = 0.0;
        z31 = 0.0;
        z32 = 0.0;
        z33 = 0.0;
        double tc = 0.0, temp = 0.0, temp1 = 0.0, temp2 = 0.0;
        double temp3 = 0.0, tsi = 0.0, xpidot = 0.0, xhdot1 = 0.0;
        double qzms2t = 0.0, ss = 0.0, x2o3 = 0.0;

        ss = 78.0 / radiusEarthKm + 1.0;
        qzms2t = Math.Pow(((120.0 - 78.0) / radiusEarthKm), 4);
        x2o3 = 2.0 / 3.0;

        initPropagator(satn, satCalcData.rec_ecco, epoch, satCalcData.rec_inclo);

        if ((omeosq >= 0.0) || (satCalcData.rec_no >= 0.0))
        {
            satCalcData.neo.neo_isimp = 0;
            if (rp < (220.0 / radiusEarthKm + 1.0))
                satCalcData.neo.neo_isimp = 1;
            sfour = ss;
            qzms24 = qzms2t;
            perige = (rp - 1.0) * radiusEarthKm;

            if (perige < 156.0) {
                sfour = perige - 78.0;
                if (perige < 98.0)
                    sfour = 20.0;
                qzms24 = Math.Pow(((120.0 - sfour) / radiusEarthKm), 4.0);
                sfour = sfour / radiusEarthKm + 1.0;
            }
            pinvsq = 1.0 / posq;

            tsi = 1.0 / (ao - sfour);
            satCalcData.neo.neo_eta = ao * satCalcData.rec_ecco * tsi;
            etasq = satCalcData.neo.neo_eta * satCalcData.neo.neo_eta;
            eeta = satCalcData.rec_ecco * satCalcData.neo.neo_eta;
            psisq = Math.Abs(1.0 - etasq);
            coef = qzms24 * Math.Pow(tsi, 4.0);
            coef1 = coef / Math.Pow(psisq, 3.5);
            cc2 = coef1
                    * satCalcData.rec_no
                    * (ao * (1.0 + 1.5 * etasq + eeta * (4.0 + etasq)) + 0.375
                            * j2 * tsi / psisq * satCalcData.neo.neo_con41
                            * (8.0 + 3.0 * etasq * (8.0 + etasq)));
            satCalcData.neo.neo_cc1 = satCalcData.rec_bstar * cc2;
            cc3 = 0.0;
            if (satCalcData.rec_ecco > 1.0e-4)
                cc3 = -2.0 * coef * tsi * j3oj2 * satCalcData.rec_no * sinio
                        / satCalcData.rec_ecco;
            satCalcData.neo.neo_x1mth2 = 1.0 - cosio2;
            satCalcData.neo.neo_cc4 = 2.0
                    * satCalcData.rec_no
                    * coef1
                    * ao
                    * omeosq
                    * (satCalcData.neo.neo_eta * (2.0 + 0.5 * etasq) + satCalcData.rec_ecco
                            * (0.5 + 2.0 * etasq) - j2
                            * tsi
                            / (ao * psisq)
                            * (-3.0
                                    * satCalcData.neo.neo_con41
                                    * (1.0 - 2.0 * eeta + etasq
                                            * (1.5 - 0.5 * eeta)) + 0.75
                                    * satCalcData.neo.neo_x1mth2
                                    * (2.0 * etasq - eeta * (1.0 + etasq))
                                    * Math.Cos(2.0 * satCalcData.rec_argpo)));
            satCalcData.neo.neo_cc5 = 2.0 * coef1 * ao * omeosq
                    * (1.0 + 2.75 * (etasq + eeta) + eeta * etasq);
            cosio4 = cosio2 * cosio2;
            temp1 = 1.5 * j2 * pinvsq * satCalcData.rec_no;
            temp2 = 0.5 * temp1 * j2 * pinvsq;
            temp3 = -0.46875 * j4 * pinvsq * pinvsq * satCalcData.rec_no;
            satCalcData.neo.neo_mdot = satCalcData.rec_no + 0.5 * temp1 * rteosq
                    * satCalcData.neo.neo_con41 + 0.0625 * temp2 * rteosq
                    * (13.0 - 78.0 * cosio2 + 137.0 * cosio4);
            satCalcData.neo.neo_argpdot = -0.5 * temp1 * con42 + 0.0625 * temp2
                    * (7.0 - 114.0 * cosio2 + 395.0 * cosio4) + temp3
                    * (3.0 - 36.0 * cosio2 + 49.0 * cosio4);
            xhdot1 = -temp1 * cosio;
            satCalcData.neo.neo_omegadot = xhdot1
                    + (0.5 * temp2 * (4.0 - 19.0 * cosio2) + 2.0 * temp3
                            * (3.0 - 7.0 * cosio2)) * cosio;
            xpidot = satCalcData.neo.neo_argpdot + satCalcData.neo.neo_omegadot;
            satCalcData.neo.neo_omgcof = satCalcData.rec_bstar * cc3
                    * Math.Cos(satCalcData.rec_argpo);
            satCalcData.neo.neo_xmcof = 0.0;
            if (satCalcData.rec_ecco > 1.0e-4)
                satCalcData.neo.neo_xmcof = -x2o3 * coef * satCalcData.rec_bstar / eeta;
            satCalcData.neo.neo_omegacf = 3.5 * omeosq * xhdot1
                    * satCalcData.neo.neo_cc1;
            satCalcData.neo.neo_t2cof = 1.5 * satCalcData.neo.neo_cc1;
            satCalcData.neo.neo_xlcof = -0.25 * j3oj2 * sinio * (3.0 + 5.0 * cosio)
                    / (1.0 + cosio);
            satCalcData.neo.neo_aycof = -0.5 * j3oj2 * sinio;
            satCalcData.neo.neo_delmo = Math.Pow((1.0 + satCalcData.neo.neo_eta
                    * Math.Cos(satCalcData.rec_mo)), 3);
            satCalcData.neo.neo_sinmao = Math.Sin(satCalcData.rec_mo);
            satCalcData.neo.neo_x7thm1 = 7.0 * cosio2 - 1.0;

            satCalcData.rec_init = 0;

            if ((2 * Math.PI / satCalcData.rec_no) >= 225.0)
            {
                satCalcData.neo.neo_method = 2;
                satCalcData.neo.neo_isimp = 1;
                tc = 0.0;
                inclm = satCalcData.rec_inclo;

                deepSpaceCom(epoch, satCalcData.rec_ecco, satCalcData.rec_argpo, tc,
                            satCalcData.rec_inclo, satCalcData.rec_omegao,
                            satCalcData.rec_no);

                satCalcData.rec_mp = satCalcData.rec_mo; // tmp
                satCalcData.rec_argpp = satCalcData.rec_argpo;
                satCalcData.rec_ep = satCalcData.rec_ecco;
                satCalcData.rec_omegap = satCalcData.rec_omegao;
                satCalcData.rec_xincp = satCalcData.rec_inclo;

                deepSpacePeriodic(satCalcData.dso.dso_e3, satCalcData.dso.dso_ee2,
                        satCalcData.dso.dso_peo, satCalcData.dso.dso_pgho,
                        satCalcData.dso.dso_pho, satCalcData.dso.dso_pinco,
                        satCalcData.dso.dso_plo, satCalcData.dso.dso_se2,
                        satCalcData.dso.dso_se3, satCalcData.dso.dso_sgh2,
                        satCalcData.dso.dso_sgh3, satCalcData.dso.dso_sgh4,
                        satCalcData.dso.dso_sh2, satCalcData.dso.dso_sh3,
                        satCalcData.dso.dso_si2, satCalcData.dso.dso_si3,
                        satCalcData.dso.dso_sl2, satCalcData.dso.dso_sl3,
                        satCalcData.dso.dso_sl4, satCalcData.neo.neo_t,
                        satCalcData.dso.dso_xgh2, satCalcData.dso.dso_xgh3,
                        satCalcData.dso.dso_xgh4, satCalcData.dso.dso_xh2,
                        satCalcData.dso.dso_xh3, satCalcData.dso.dso_xi2,
                        satCalcData.dso.dso_xi3, satCalcData.dso.dso_xl2,
                        satCalcData.dso.dso_xl3, satCalcData.dso.dso_xl4,
                        satCalcData.dso.dso_zmol, satCalcData.dso.dso_zmos, 1);

                satCalcData.rec_mo = satCalcData.rec_mp; 
                satCalcData.rec_argpo = satCalcData.rec_argpp;
                satCalcData.rec_ecco = satCalcData.rec_ep;
                satCalcData.rec_omegao = satCalcData.rec_omegap;
                satCalcData.rec_inclo = satCalcData.rec_xincp;

                argpm = 0.0;
                omegam = 0.0;
                mm = 0.0;

                deepSpaceinit(cosim, emsq, satCalcData.rec_argpo, s1, s2, s3, s4, s5, sinim,
                        satCalcData.neo.neo_t, tc, satCalcData.dso.dso_gsto, satCalcData.rec_mo,
                        satCalcData.neo.neo_mdot, satCalcData.rec_no, satCalcData.rec_omegao,
                        satCalcData.neo.neo_omegadot, xpidot, z1, z3, z11, z13,
                        z21, z23, z31, z33, satCalcData.rec_ecco, eccsq);
            }

            if (satCalcData.neo.neo_isimp != 1) {
                cc1sq = satCalcData.neo.neo_cc1 * satCalcData.neo.neo_cc1;
                satCalcData.neo.neo_d2 = 4.0 * ao * tsi * cc1sq;
                temp = satCalcData.neo.neo_d2 * tsi * satCalcData.neo.neo_cc1 / 3.0;
                satCalcData.neo.neo_d3 = (17.0 * ao + sfour) * temp;
                satCalcData.neo.neo_d4 = 0.5 * temp * ao * tsi
                        * (221.0 * ao + 31.0 * sfour) * satCalcData.neo.neo_cc1;
                satCalcData.neo.neo_t3cof = satCalcData.neo.neo_d2 + 2.0 * cc1sq;
                satCalcData.neo.neo_t4cof = 0.25 * (3.0 * satCalcData.neo.neo_d3 + satCalcData.neo.neo_cc1
                        * (12.0 * satCalcData.neo.neo_d2 + 10.0 * cc1sq));
                satCalcData.neo.neo_t5cof = 0.2 * (3.0 * satCalcData.neo.neo_d4 + 12.0
                        * satCalcData.neo.neo_cc1 * satCalcData.neo.neo_d3 + 6.0
                        * satCalcData.neo.neo_d2 * satCalcData.neo.neo_d2 + 15.0
                        * cc1sq * (2.0 * satCalcData.neo.neo_d2 + cc1sq));
            }
        }

    }


     /*-----------------------------------------------------------------------------
     *
     *                           procedure dscom
     *
     *  this procedure provides deep space common items used by both the secular
     *    and periodics subroutines.  input is provided as shown. this routine
     *    used to be called dpper, but the functions inside weren't correct.
     *
     *  author        : david vallado                  719-573-2600    1 mar 2001
     *
     *  inputs        :
     *    epoch       -
     *    ep          - eccentricity
     *    argpp       - argument of perigee
     *    tc          -
     *    inclp       - inclination
     *    omegap      - longitude of ascending node
     *    np          - mean motion
     *
     *  outputs       :
     *    sinim       -
     *    cosim       -
     *    sinomm      -
     *    cosomm      -
     *    snodm       -
     *    cnodm       -
     *    day         -
     *    e3          -
     *    ee2         -
     *    em          - eccentricity
     *    emsq        - eccentricity squared
     *    gam         -
     *    peo         -
     *    pgho        -
     *    pho         -
     *    pinco       -
     *    plo         -
     *    rtemsq      -
     *    se2, se3         -
     *    sgh2, sgh3, sgh4        -
     *    sh2, sh3
     *    si2, si3         -
     *    sl2, sl3, sl4         -
     *    s1, s2, s3, s4, s5, s6, s7          -
     *    ss1, ss2, ss3, ss4, ss5, ss6, ss7         -
     *    sz1, sz2, sz3         -
     *    sz11, sz12, sz13, sz21, sz22, sz23, sz31, sz32, sz33        -
     *    xgh2, xgh3, xgh4        -
     *    xh2, xh3         -
     *    xi2, xi3         -
     *    xl2, xl3, xl4         -
     *    nm          - mean motion
     *    z1, z2, z3          -
     *    z11, z12, z13, z21, z22, z23, z31, z32, z33         -
     *    zmol        -
     *    zmos        -
     *
     *  locals        :
     *    a1, a2, a3, a4, a5, a6, a7, a8, a9, a10         -
     *    betasq      -
     *    cc          -
     *    ctem        -
     *    stem        -
     *    x1, x2, x3, x4, x5, x6, x7, x8          -
     *    xnodce      -
     *    xnoi        -
     *    zcosg       -
     *    zcosgl      -
     *    zcosh       -
     *    zcoshl      -
     *    zcosi       -
     *    zcosil      -
     *    zsing       -
     *    zsingl      -
     *    zsinh       -
     *    zsinhl      -
     *    zsini       -
     *    zsinil      -
     *    zx          -
     *    zy          -
     *
     *  coupling      :
     *    none.
     *
     *  references    :
     *    norad spacetrack report #3
     *
     ----------------------------------------------------------------------------*/
     private void deepSpaceCom(double epoch, double ep, double argpp, double tc,
                                double inclp, double omegap, double np) 
     {

        /* -------------------------- Constants ------------------------- */
        double zes = 0.01675;
        double zel = 0.05490;
        double c1ss = 2.9864797e-6;
        double c1l = 4.7968065e-7;
        double zsinis = 0.39785416;
        double zcosis = 0.91744867;
        double zcosgs = 0.1945905;
        double zsings = -0.98088458;

        /* --------------------- local variables ------------------------ */
        int lsflg;
        double a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, betasq, cc, ctem, stem;
        double x1, x2, x3, x4, x5, x6, x7, x8, xnodce, xnoi, zcosg, zcosgl, zcosh;
        double zcoshl, zcosi, zcosil, zsing, zsingl, zsinh, zsinhl, zsini, zsinil, zx, zy;

        nm = np;
        em = ep;
        snodm = Math.Sin(omegap);
        cnodm = Math.Cos(omegap);
        sinomm = Math.Sin(argpp);
        cosomm = Math.Cos(argpp);
        sinim = Math.Sin(inclp);
        cosim = Math.Cos(inclp);
        emsq = em * em;
        betasq = 1.0 - emsq;
        rtemsq = Math.Sqrt(betasq);

        /* ----------------- initialize lunar solar terms --------------- */
        satCalcData.dso.dso_peo = 0.0;
        satCalcData.dso.dso_pinco = 0.0;
        satCalcData.dso.dso_plo = 0.0;
        satCalcData.dso.dso_pgho = 0.0;
        satCalcData.dso.dso_pho = 0.0;
        day = epoch + 18261.5 + tc / 1440.0;
        xnodce = modfunc(4.5236020 - 9.2422029e-4 * day, twoPi);
        stem = Math.Sin(xnodce);
        ctem = Math.Cos(xnodce);
        zcosil = 0.91375164 - 0.03568096 * ctem;
        zsinil = Math.Sqrt(1.0 - zcosil * zcosil);
        zsinhl = 0.089683511 * stem / zsinil;
        zcoshl = Math.Sqrt(1.0 - zsinhl * zsinhl);
        gam = 5.8351514 + 0.0019443680 * day;
        zx = 0.39785416 * stem / zsinil;
        zy = zcoshl * ctem + 0.91744867 * zsinhl * stem;
        zx = Math.Atan2(zx, zy);
        zx = gam + zx - xnodce;
        zcosgl = Math.Cos(zx);
        zsingl = Math.Sin(zx);

        /* ------------------------- do solar terms --------------------- */
        zcosg = zcosgs;
        zsing = zsings;
        zcosi = zcosis;
        zsini = zsinis;
        zcosh = cnodm;
        zsinh = snodm;
        cc = c1ss;
        xnoi = 1.0 / nm;

        for (lsflg = 1; lsflg <= 2; lsflg++) {
            a1 = zcosg * zcosh + zsing * zcosi * zsinh;
            a3 = -zsing * zcosh + zcosg * zcosi * zsinh;
            a7 = -zcosg * zsinh + zsing * zcosi * zcosh;
            a8 = zsing * zsini;
            a9 = zsing * zsinh + zcosg * zcosi * zcosh;
            a10 = zcosg * zsini;
            a2 = cosim * a7 + sinim * a8;
            a4 = cosim * a9 + sinim * a10;
            a5 = -sinim * a7 + cosim * a8;
            a6 = -sinim * a9 + cosim * a10;

            x1 = a1 * cosomm + a2 * sinomm;
            x2 = a3 * cosomm + a4 * sinomm;
            x3 = -a1 * sinomm + a2 * cosomm;
            x4 = -a3 * sinomm + a4 * cosomm;
            x5 = a5 * sinomm;
            x6 = a6 * sinomm;
            x7 = a5 * cosomm;
            x8 = a6 * cosomm;

            z31 = 12.0 * x1 * x1 - 3.0 * x3 * x3;
            z32 = 24.0 * x1 * x2 - 6.0 * x3 * x4;
            z33 = 12.0 * x2 * x2 - 3.0 * x4 * x4;
            z1 = 3.0 * (a1 * a1 + a2 * a2) + z31 * emsq;
            z2 = 6.0 * (a1 * a3 + a2 * a4) + z32 * emsq;
            z3 = 3.0 * (a3 * a3 + a4 * a4) + z33 * emsq;
            z11 = -6.0 * a1 * a5 + emsq * (-24.0 * x1 * x7 - 6.0 * x3 * x5);
            z12 = -6.0 * (a1 * a6 + a3 * a5) + emsq
                    * (-24.0 * (x2 * x7 + x1 * x8) - 6.0 * (x3 * x6 + x4 * x5));
            z13 = -6.0 * a3 * a6 + emsq * (-24.0 * x2 * x8 - 6.0 * x4 * x6);
            z21 = 6.0 * a2 * a5 + emsq * (24.0 * x1 * x5 - 6.0 * x3 * x7);
            z22 = 6.0 * (a4 * a5 + a2 * a6) + emsq
                    * (24.0 * (x2 * x5 + x1 * x6) - 6.0 * (x4 * x7 + x3 * x8));
            z23 = 6.0 * a4 * a6 + emsq * (24.0 * x2 * x6 - 6.0 * x4 * x8);
            z1 = z1 + z1 + betasq * z31;
            z2 = z2 + z2 + betasq * z32;
            z3 = z3 + z3 + betasq * z33;
            s3 = cc * xnoi;
            s2 = -0.5 * s3 / rtemsq;
            s4 = s3 * rtemsq;
            s1 = -15.0 * em * s4;
            s5 = x1 * x3 + x2 * x4;
            s6 = x2 * x3 + x1 * x4;
            s7 = x2 * x4 - x1 * x3;

            /* ----------------------- do lunar terms ------------------- */
            if (lsflg == 1) {
                ss1 = s1;
                ss2 = s2;
                ss3 = s3;
                ss4 = s4;
                ss5 = s5;
                ss6 = s6;
                ss7 = s7;
                sz1 = z1;
                sz2 = z2;
                sz3 = z3;
                sz11 = z11;
                sz12 = z12;
                sz13 = z13;
                sz21 = z21;
                sz22 = z22;
                sz23 = z23;
                sz31 = z31;
                sz32 = z32;
                sz33 = z33;
                zcosg = zcosgl;
                zsing = zsingl;
                zcosi = zcosil;
                zsini = zsinil;
                zcosh = zcoshl * cnodm + zsinhl * snodm;
                zsinh = snodm * zcoshl - cnodm * zsinhl;
                cc = c1l;
            }
        }

        satCalcData.dso.dso_zmol = modfunc(4.7199672 + 0.22997150 * day - gam,
                twoPi);
        satCalcData.dso.dso_zmos = modfunc(6.2565837 + 0.017201977 * day, twoPi);

        /* ------------------------ do solar terms ---------------------- */
        satCalcData.dso.dso_se2 = 2.0 * ss1 * ss6;
        satCalcData.dso.dso_se3 = 2.0 * ss1 * ss7;
        satCalcData.dso.dso_si2 = 2.0 * ss2 * sz12;
        satCalcData.dso.dso_si3 = 2.0 * ss2 * (sz13 - sz11);
        satCalcData.dso.dso_sl2 = -2.0 * ss3 * sz2;
        satCalcData.dso.dso_sl3 = -2.0 * ss3 * (sz3 - sz1);
        satCalcData.dso.dso_sl4 = -2.0 * ss3 * (-21.0 - 9.0 * emsq) * zes;
        satCalcData.dso.dso_sgh2 = 2.0 * ss4 * sz32;
        satCalcData.dso.dso_sgh3 = 2.0 * ss4 * (sz33 - sz31);
        satCalcData.dso.dso_sgh4 = -18.0 * ss4 * zes;
        satCalcData.dso.dso_sh2 = -2.0 * ss2 * sz22;
        satCalcData.dso.dso_sh3 = -2.0 * ss2 * (sz23 - sz21);

        /* ------------------------ do lunar terms ---------------------- */
        satCalcData.dso.dso_ee2 = 2.0 * s1 * s6;
        satCalcData.dso.dso_e3 = 2.0 * s1 * s7;
        satCalcData.dso.dso_xi2 = 2.0 * s2 * z12;
        satCalcData.dso.dso_xi3 = 2.0 * s2 * (z13 - z11);
        satCalcData.dso.dso_xl2 = -2.0 * s3 * z2;
        satCalcData.dso.dso_xl3 = -2.0 * s3 * (z3 - z1);
        satCalcData.dso.dso_xl4 = -2.0 * s3 * (-21.0 - 9.0 * emsq) * zel;
        satCalcData.dso.dso_xgh2 = 2.0 * s4 * z32;
        satCalcData.dso.dso_xgh3 = 2.0 * s4 * (z33 - z31);
        satCalcData.dso.dso_xgh4 = -18.0 * s4 * zel;
        satCalcData.dso.dso_xh2 = -2.0 * s2 * z22;
        satCalcData.dso.dso_xh3 = -2.0 * s2 * (z23 - z21);
    }

    public void initPropagator(int satn, double ecco, double epoch, double inclo)
    {
	    /* --------------------- local variables ------------------------ */
	    double ak, d1, del, adel, po, x2o3;

	    x2o3 = 2.0 / 3.0;
	    eccsq = ecco * ecco;
	    omeosq = 1.0 - eccsq;
	    rteosq = Math.Sqrt(omeosq);
	    cosio = Math.Cos(inclo);
	    cosio2 = cosio * cosio;

	    ak = Math.Pow(xke /  satCalcData.rec_no, x2o3);
	    d1 = 0.75 * j2 * (3.0 * cosio2 - 1.0) / (rteosq * omeosq);
	    del = d1 / (ak * ak);
	    adel = ak
		    * (1.0 - del * del - del
		    * (1.0 / 3.0 + 134.0 * del * del / 81.0));
	    del = d1 / (adel * adel);
	    satCalcData.rec_no = satCalcData.rec_no / (1.0 + del);

	    ao = Math.Pow(xke / satCalcData.rec_no, x2o3);
	    sinio = Math.Sin(inclo);
	    po = ao * omeosq;
	    con42 = 1.0 - 5.0 * cosio2;
	    satCalcData.neo.neo_con41 = -con42 - cosio2 - cosio2;
	    ainv = 1.0 / ao; // FIXME is this used?
	    einv = 1.0 / ecco;
	    posq = po * po;
	    rp = ao * (1.0 - ecco);
	    satCalcData.neo.neo_method = 0;
	    if (rp < 1.0){

	    }
	    satCalcData.dso.dso_gsto = gstime(epoch + 2433281.5);
    }

        /*------------------------------------------------------------------------------
        *
        *                           procedure newtonm
        *
        *  this procedure performs the newton rhapson iteration to find the
        *    eccentric anomaly given the mean anomaly.  the true anomaly is also
        *    calculated.
        *
        *  author        : david vallado                  303-344-6037    1 mar 2001
        *
        *  inputs          description                    range / units
        *    ecc         - eccentricity                   0.0 to
        *    m           - mean anomaly                   -2pi to 2pi rad
        *
        *  outputs       :
        *    e0          - eccentric anomaly              0.0 to 2pi rad
        *    nu          - true anomaly                   0.0 to 2pi rad
        *
        *  locals        :
        *    e1          - eccentric anomaly, next value  rad
        *    sinv        - sine of nu
        *    cosv        - cosine of nu
        *    ktr         - index
        *    r1r         - cubic roots - 1 to 3
        *    r1i         - imaginary component
        *    r2r         -
        *    r2i         -
        *    r3r         -
        *    r3i         -
        *    s           - variables for parabolic solution
        *    w           - variables for parabolic solution
        *
        *  coupling      :
        *    atan2       - arc tangent function which also resloves quadrants
        *    cubic       - solves a cubic polynomial
        *    power       - raises a base number to an arbitrary power
        *    sinh        - hyperbolic sine
        *    cosh        - hyperbolic cosine
        *    sgn         - returns the sign of an argument
        *
        *  references    :
        *    vallado       2001, 72-75, alg 2, ex 2-1
        *
        -----------------------------------------------------------------------------*/
    private void newtonm(double ecc, double m, double e0, double nu) {
        int numiter = 50;
        double small = 0.00000001; // small value for tolerances

        double e1, sinv, cosv, r1r = 0.0;
        int ktr;

        /* -------------------------- hyperbolic ----------------------- */
        if ((ecc - 1.0) > small) {
            /* ------------ initial guess ------------- */
            if (ecc < 1.6)
                if (((m < 0.0) && (m > - Math.PI)) || (m > Math.PI))
                    e0 = m - ecc;
                else
                    e0 = m + ecc;
            else if ((ecc < 3.6) && (Math.Abs(m) > Math.PI)) // just edges)
                e0 = m - Math.Sign(m) * ecc;
            else
                e0 = m / (ecc - 1.0); // best over 1.8 in middle
            ktr = 1;
            e1 = e0
                    + ((m - ecc * Math.Sinh(e0) + e0) / (ecc * Math.Cosh(e0) - 1.0));
            while ((Math.Abs(e1 - e0) > small) && (ktr <= numiter)) {
                e0 = e1;
                e1 = e0
                        + ((m - ecc * Math.Sinh(e0) + e0) / (ecc
                                * Math.Cosh(e0) - 1.0));
                ktr++;
            }
            /* --------- find true anomaly ----------- */
            sinv = -(Math.Sqrt(ecc * ecc - 1.0) * Math.Sinh(e1))
                    / (1.0 - ecc * Math.Cosh(e1));
            cosv = (Math.Cosh(e1) - ecc) / (1.0 - ecc * Math.Cosh(e1));
            nu = Math.Atan2(sinv, cosv);
        } else {
            /* ---------------------- parabolic ------------------------- */
            if (Math.Abs(ecc - 1.0) < small) {
                // kbn cubic(1.0 / 3.0, 0.0, 1.0, -m, r1r, r1i, r2r, r2i, r3r,
                // r3i);
                e0 = r1r;
                // kbn if (fileout != null)
                // fprintf(fileout, "roots %11.7f %11.7f %11.7f %11.7f %11.7f
                // %11.7f\n",
                // r1r, r1i, r2r, r2i, r3r, r3i);
                /*
                    * s = 0.5 * (halfpi - atan(1.5 * m)); w = atan(power(tan(s),
                    * 1.0 / 3.0)); e0 = 2.0 * cot(2.0* w );
                    */
                ktr = 1;
                nu = 2.0 * Math.Atan(e0);
            } else {
                /* --------------------- elliptical --------------------- */
                if (ecc > small) {
                    /* ------------ initial guess ------------- */
                    if (((m < 0.0) && (m > - Math.PI)) || (m > Math.PI))
                        e0 = m - ecc;
                    else
                        e0 = m + ecc;
                    ktr = 1;
                    e1 = e0 + (m - e0 + ecc * Math.Sin(e0))
                            / (1.0 - ecc * Math.Cos(e0));
                    while ((Math.Abs(e1 - e0) > small) && (ktr <= numiter)) {
                        ktr++;
                        e0 = e1;
                        e1 = e0 + (m - e0 + ecc * Math.Sin(e0))
                                / (1.0 - ecc * Math.Cos(e0));
                    }
                    /* --------- find true anomaly ----------- */
                    sinv = (Math.Sqrt(1.0 - ecc * ecc) * Math.Sin(e1))
                            / (1.0 - ecc * Math.Cos(e1));
                    cosv = (Math.Cos(e1) - ecc) / (1.0 - ecc * Math.Cos(e1));
                    nu = Math.Atan2(sinv, cosv);
                } else {
                    /* --------------------- circular --------------------- */
                    ktr = 0;
                    nu = m;
                    e0 = m;
                }
            }
        }
    }

        /*-----------------------------------------------------------------------------
     *
     *                           procedure dsinit
     *
     *  this procedure provides deep space contributions to mean motion dot due
     *    to geopotential resonance with half day and one day orbits.
     *
     *  author        : david vallado                  719-573-2600    1 mar 2001
     *
     *  inputs        :
     *    cosim       -
     *    emsq        - eccentricity squared
     *    argpo       - argument of perigee
     *    s1, s2, s3, s4, s5          -
     *    sinim       -
     *    ss1, ss2, ss3, ss4, ss5         -
     *    sz1, sz3
     *    sz11, sz13, sz21, sz23, sz31, sz33        -
     *    t           - time
     *    tc          -
     *    gsto        -
     *    mo          - mean anomaly
     *    mdot        - mean anomaly dot (rate)
     *    no          - mean motion
     *    omegao      - longitude of ascending node
     *    omegadot    - longitude of ascending node dot (rate)
     *    xpidot      -
     *    z1, z3      -
     *    z11, z13, z21, z23, z31, z33         -
     *    em          - eccentricity
     *    argpm       - argument of perigee
     *    inclm       - inclination
     *    mm          - mean anomaly
     *    nm          - mean motion
     *    omegam      - longitude of ascending node
     *
     *  outputs       :
     *    em          - eccentricity
     *    argpm       - argument of perigee
     *    inclm       - inclination
     *    mm          - mean anomaly
     *    nm          - mean motion
     *    omegam      - longitude of ascending node
     *    irez        - flag for resonances               1-one day      - 2-half day
     *    atime       -
     *    d2201, d2211, d3210, d3222, d4410, d4422, d5220, d5232, d5421, d5433       -
     *    dedt        -
     *    didt        -
     *    dmdt        -
     *    dndt        -
     *    dnodt       -
     *    domdt       -
     *    del1, del2, del3        -
     *    ses         -
     *    sghl        -
     *    sghs        -
     *    sgs         -
     *    shl         -
     *    shs         -
     *    sis         -
     *    sls         -
     *    theta       -
     *    xfact       -
     *    xlamo       -
     *    xli         -
     *    xni
     *
     *  locals        :
     *    ainv2       -
     *    aonv        -
     *    cosisq      -
     *    eoc         -
     *    f220, f221, f311, f321, f322, f330, f441, f442, f522, f523, f542, f543        -
     *    g200, g201, g211, g300, g310, g322, g410, g422, g520, g521, g532, g533        -
     *    sini2       -
     *    temp        -
     *    temp1       -
     *    theta       -
     *    xno2        -
     *
     *  coupling      :
     *    none.
     *
     *  references    :
     *    norad spacetrack report #3
     *
     ----------------------------------------------------------------------------*/
     private void deepSpaceinit(double cosim, double emsq, double argpo, double s1,
                double s2, double s3, double s4, double s5, double sinim,
                 double t, double tc, double gsto, double mo, double mdot,
                double no, double omegao, double omegadot, double xpidot,
                double z1, double z3, double z11, double z13, double z21,
                double z23, double z31, double z33, double ecco, double eccsq) {

        /* --------------------- local variables ------------------------ */

        double ainv2, aonv = 0.0, cosisq, eoc, f220, f221, f311, f321, f322, f330;
        double f441, f442, f522, f523, f542, f543, g200, g201, g211, g300, g310, g322;
        double g410, g422, g520, g521, g532, g533, ses, sgs, sghl, sghs, shs, shll, sis;
        double sini2, sls, temp, temp1, theta, xno2, q22, q31, q33, root22, root44;
        double root54, rptim, root32, root52, x2o3, znl, emo, zns, emsqo;

        q22 = 1.7891679e-6;
        q31 = 2.1460748e-6;
        q33 = 2.2123015e-7;
        root22 = 1.7891679e-6;
        root44 = 7.3636953e-9;
        root54 = 2.1765803e-9;
        rptim = 4.37526908801129966e-3;
        root32 = 3.7393792e-7;
        root52 = 1.1428639e-7;
        x2o3 = 2.0 / 3.0;
        znl = 1.5835218e-4;
        zns = 1.19459e-5;

        /* -------------------- deep space initialization ------------ */
        satCalcData.dso.dso_irez = 0;
        if ((nm < 0.0052359877) && (nm > 0.0034906585))
            satCalcData.dso.dso_irez = 1;
        if ((nm >= 8.26e-3) && (nm <= 9.24e-3) && (em >= 0.5))
            satCalcData.dso.dso_irez = 2;

        /* ------------------------ do solar terms ------------------- */
        ses = ss1 * zns * ss5;
        sis = ss2 * zns * (sz11 + sz13);
        sls = -zns * ss3 * (sz1 + sz3 - 14.0 - 6.0 * emsq);
        sghs = ss4 * zns * (sz31 + sz33 - 6.0);
        shs = -zns * ss2 * (sz21 + sz23);
        if (inclm < 5.2359877e-2)
            shs = 0.0;
        if (sinim != 0.0)
            shs = shs / sinim;
        sgs = sghs - cosim * shs;

        /* ------------------------- do lunar terms ------------------ */
        satCalcData.dso.dso_dedt = ses + s1 * znl * s5;
        satCalcData.dso.dso_didt = sis + s2 * znl * (z11 + z13);
        satCalcData.dso.dso_dmdt = sls - znl * s3 * (z1 + z3 - 14.0 - 6.0 * emsq);
        sghl = s4 * znl * (z31 + z33 - 6.0);
        shll = -znl * s2 * (z21 + z23);
        if (inclm < 5.2359877e-2)
            shll = 0.0;
        satCalcData.dso.dso_domdt = sgs + sghl;
        satCalcData.dso.dso_dnodt = shs;
        if (sinim != 0.0) {
            satCalcData.dso.dso_domdt = satCalcData.dso.dso_domdt - cosim / sinim
                    * shll;
            satCalcData.dso.dso_dnodt = satCalcData.dso.dso_dnodt + shll / sinim;
        }

        /* ----------- calculate deep space resonance effects -------- */
        dndt = 0.0;
        theta = modfunc(gsto + tc * rptim, twoPi);
        em = em + satCalcData.dso.dso_dedt * t;
        // shouldn't emsq be changed now?????
        inclm = inclm + satCalcData.dso.dso_didt * t;
        argpm = argpm + satCalcData.dso.dso_domdt * t;
        omegam = omegam + satCalcData.dso.dso_dnodt * t;
        mm = mm + satCalcData.dso.dso_dmdt * t;
        /*
         * these are not needed since they do not get into any DS structs, and
         * are not used if (inclm < 0.0) { inclm = -inclm; argpm = argpm - pi;
         * omegam = omegam + pi; }
         */
        // sgp4fix for negative inclinations
        // the following if statement should be commented out
        // if (inclm < 0.0)
        // {
        // inclm = -inclm;
        // argpm = argpm - pi;
        // omegam = omegam + pi;
        // }
        /* -------------- initialize the resonance terms ------------- */
        if (satCalcData.dso.dso_irez != 0)
            aonv = Math.Pow(nm / xke, x2o3);

        /* ---------- geopotential resonance for 12 hour orbits ------ */
        if (satCalcData.dso.dso_irez == 2) {
            cosisq = cosim * cosim;
            emo = em;
            em = ecco;
            emsqo = emsq;
            emsq = eccsq;
            eoc = em * emsq;
            g201 = -0.306 - (em - 0.64) * 0.440;

            if (em <= 0.65) {
                g211 = 3.616 - 13.2470 * em + 16.2900 * emsq;
                g310 = -19.302 + 117.3900 * em - 228.4190 * emsq + 156.5910
                        * eoc;
                g322 = -18.9068 + 109.7927 * em - 214.6334 * emsq + 146.5816
                        * eoc;
                g410 = -41.122 + 242.6940 * em - 471.0940 * emsq + 313.9530
                        * eoc;
                g422 = -146.407 + 841.8800 * em - 1629.014 * emsq + 1083.4350
                        * eoc;
                g520 = -532.114 + 3017.977 * em - 5740.032 * emsq + 3708.2760
                        * eoc;
            } else {
                g211 = -72.099 + 331.819 * em - 508.738 * emsq + 266.724 * eoc;
                g310 = -346.844 + 1582.851 * em - 2415.925 * emsq + 1246.113
                        * eoc;
                g322 = -342.585 + 1554.908 * em - 2366.899 * emsq + 1215.972
                        * eoc;
                g410 = -1052.797 + 4758.686 * em - 7193.992 * emsq + 3651.957
                        * eoc;
                g422 = -3581.690 + 16178.110 * em - 24462.770 * emsq
                        + 12422.520 * eoc;
                if (em > 0.715)
                    g520 = -5149.66 + 29936.92 * em - 54087.36 * emsq
                            + 31324.56 * eoc;
                else
                    g520 = 1464.74 - 4664.75 * em + 3763.64 * emsq;
            }
            if (em < 0.7) {
                g533 = -919.22770 + 4988.6100 * em - 9064.7700 * emsq + 5542.21
                        * eoc;
                g521 = -822.71072 + 4568.6173 * em - 8491.4146 * emsq
                        + 5337.524 * eoc;
                g532 = -853.66600 + 4690.2500 * em - 8624.7700 * emsq + 5341.4
                        * eoc;
            } else {
                g533 = -37995.780 + 161616.52 * em - 229838.20 * emsq
                        + 109377.94 * eoc;
                g521 = -51752.104 + 218913.95 * em - 309468.16 * emsq
                        + 146349.42 * eoc;
                g532 = -40023.880 + 170470.89 * em - 242699.48 * emsq
                        + 115605.82 * eoc;
            }

            sini2 = sinim * sinim;
            f220 = 0.75 * (1.0 + 2.0 * cosim + cosisq);
            f221 = 1.5 * sini2;
            f321 = 1.875 * sinim * (1.0 - 2.0 * cosim - 3.0 * cosisq);
            f322 = -1.875 * sinim * (1.0 + 2.0 * cosim - 3.0 * cosisq);
            f441 = 35.0 * sini2 * f220;
            f442 = 39.3750 * sini2 * sini2;
            f522 = 9.84375
                    * sinim
                    * (sini2 * (1.0 - 2.0 * cosim - 5.0 * cosisq) + 0.33333333 * (-2.0
                            + 4.0 * cosim + 6.0 * cosisq));
            f523 = sinim
                    * (4.92187512 * sini2
                            * (-2.0 - 4.0 * cosim + 10.0 * cosisq) + 6.56250012 * (1.0 + 2.0 * cosim - 3.0 * cosisq));
            f542 = 29.53125
                    * sinim
                    * (2.0 - 8.0 * cosim + cosisq
                            * (-12.0 + 8.0 * cosim + 10.0 * cosisq));
            f543 = 29.53125
                    * sinim
                    * (-2.0 - 8.0 * cosim + cosisq
                            * (12.0 + 8.0 * cosim - 10.0 * cosisq));
            xno2 = nm * nm;
            ainv2 = aonv * aonv;
            temp1 = 3.0 * xno2 * ainv2;
            temp = temp1 * root22;
            satCalcData.dso.dso_d2201 = temp * f220 * g201;
            satCalcData.dso.dso_d2211 = temp * f221 * g211;
            temp1 = temp1 * aonv;
            temp = temp1 * root32;
            satCalcData.dso.dso_d3210 = temp * f321 * g310;
            satCalcData.dso.dso_d3222 = temp * f322 * g322;
            temp1 = temp1 * aonv;
            temp = 2.0 * temp1 * root44;
            satCalcData.dso.dso_d4410 = temp * f441 * g410;
            satCalcData.dso.dso_d4422 = temp * f442 * g422;
            temp1 = temp1 * aonv;
            temp = temp1 * root52;
            satCalcData.dso.dso_d5220 = temp * f522 * g520;
            satCalcData.dso.dso_d5232 = temp * f523 * g532;
            temp = 2.0 * temp1 * root54;
            satCalcData.dso.dso_d5421 = temp * f542 * g521;
            satCalcData.dso.dso_d5433 = temp * f543 * g533;
            satCalcData.dso.dso_xlamo = modfunc(mo + omegao + omegao - theta
                    - theta, twoPi);
            satCalcData.dso.dso_xfact = mdot + satCalcData.dso.dso_dmdt + 2.0
                    * (omegadot + satCalcData.dso.dso_dnodt - rptim) - no;
            em = emo;
            emsq = emsqo;
        }

        if (satCalcData.dso.dso_irez == 1) {
            g200 = 1.0 + emsq * (-2.5 + 0.8125 * emsq);
            g310 = 1.0 + 2.0 * emsq;
            g300 = 1.0 + emsq * (-6.0 + 6.60937 * emsq);
            f220 = 0.75 * (1.0 + cosim) * (1.0 + cosim);
            f311 = 0.9375 * sinim * sinim * (1.0 + 3.0 * cosim) - 0.75
                    * (1.0 + cosim);
            f330 = 1.0 + cosim;
            f330 = 1.875 * f330 * f330 * f330;
            satCalcData.dso.dso_del1 = 3.0 * nm * nm * aonv * aonv;
            satCalcData.dso.dso_del2 = 2.0 * satCalcData.dso.dso_del1 * f220 * g200
                    * q22;
            satCalcData.dso.dso_del3 = 3.0 * satCalcData.dso.dso_del1 * f330 * g300
                    * q33 * aonv;
            satCalcData.dso.dso_del1 = satCalcData.dso.dso_del1 * f311 * g310 * q31
                    * aonv;
            satCalcData.dso.dso_xlamo = modfunc(mo + omegao + argpo - theta, twoPi);
            satCalcData.dso.dso_xfact = mdot + xpidot - rptim
                    + satCalcData.dso.dso_dmdt + satCalcData.dso.dso_domdt
                    + satCalcData.dso.dso_dnodt - no;
        }

        /* ------------ for sgp4, initialize the integrator ---------- */
        if (satCalcData.dso.dso_irez != 0) {
            satCalcData.dso.dso_xli = satCalcData.dso.dso_xlamo;
            satCalcData.dso.dso_xni = no;
            satCalcData.dso.dso_atime = 0.0;
            nm = no + dndt;
        }
    }

        public double gstime(double jdut1)
        {
	        double temp, tut1;

	        tut1 = (jdut1 - 2451545.0) / 36525.0;
	        temp = -6.2e-6 * tut1 * tut1 * tut1 + 0.093104 * tut1 * tut1
		        + (876600.0 * 3600 + 8640184.812866) * tut1 + 67310.54841; // sec
	        temp = modfunc(temp * toRadians / 240.0, twoPi); 
            // 360/86400 = 1/240, to deg, to rad

	        /* ------------------------ check quadrants --------------------- */
	        if (temp < 0.0)
		        temp += twoPi;

	        return temp;
        }

        private double modfunc(double x, double y)
        {
	        if (y != 0)
	        {
		        return x - (int)(x / y) * y;
	        }
	        return 0;
        }

        public List<Sgp4Data> getRestults()
        {
            return resultOrbitData;
        }

        public void setGrav(int select)
        {
            switch (select)
            {
                case 0:
                    radiusEarthKm = WGS_72.radiusEarthKM;
                    mu = WGS_72.mu;
                    xke = 60.0 / Math.Sqrt(radiusEarthKm * radiusEarthKm * radiusEarthKm / mu);
                    tumin = 1.0 / xke;
                    j2 = WGS_72.j2;
                    j3 = WGS_72.j3;
                    j4 = WGS_72.j4;
                    j3oj2 = j3 / j2;
                    break;
                case 1:
                    radiusEarthKm = WGS_84.radiusEarthKM;
                    mu = WGS_84.mu;
                    xke = 60.0 / Math.Sqrt(radiusEarthKm * radiusEarthKm * radiusEarthKm / mu);
                    tumin = 1.0 / xke;
                    j2 = WGS_84.j2;
                    j3 = WGS_84.j3;
                    j4 = WGS_84.j4;
                    j3oj2 = j3 / j2;
                    break;
            }
        }
    }
}
