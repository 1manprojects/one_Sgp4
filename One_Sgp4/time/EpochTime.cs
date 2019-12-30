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
    public class EpochTime : IComparable<EpochTime>
    {
        /**
      * \brief EpochTime class
      *
      * This class defnies the Time object for calculation of the orbits
      * The time is being stored as a double value that represents the day of 
      * the year and the time as the decimal place
      */
        private int hour;       //!< int Hour
        private int minutes;    //!< int Minute
        private double seconds; //!< double Seconds

        private int year;   //!< int Year
        private int month;  //!< int Month
        private int day;    //!< int Day

        private double epoch; //!< double epoch of the Date

        private const double secToDay = 86400.0; //!< double constant seconds of a day
        private int days = 365; //!< int days in the year

        private const double twoPi = 2.0 * Math.PI; //!< double constant two Pi
        public const double toRadians = Math.PI / 180.0; //!< double constant converstion to radians

        //! EpochTime constructor.
        /*!
        \param int hour
        \param int minutes
        \param double seconds
        \param int Year
        \param int Month
        \param int Day
        Constructs EpochTime with current Time in UTC and Date 
        */
        public EpochTime(int h, int m, double s, int yyyy, int mm, int dd)
        {
            hour = h;
            minutes = m;
            seconds = s;

            year = yyyy;
            month = mm;
            day = dd;

            epoch = getDayOfYear();
        }

        //! EpochTime constructor.
        /*!
        \param DateTime
        Contructs EpochTime from DateTime object local time
        */
        public EpochTime(DateTime _dateTime)
        {
            DateTime utc = _dateTime.ToUniversalTime();
            year = utc.Year;
            month = utc.Month;
            day = utc.Day;

            hour = utc.Hour;
            minutes = utc.Minute;
            seconds = Convert.ToDouble(utc.Second) + utc.Millisecond/1000.0;

            double d = getDayOfYear();
            epoch = d;
        }

        //! EpochTime constructor.
        /*!
        \param EpochTime
        Contructs EpochTime from EpochTime Object
        */
        public EpochTime(EpochTime _EpochTime)
        {
            hour = _EpochTime.getHour();
            minutes = _EpochTime.getMin();
            seconds = _EpochTime.getSec();

            year = _EpochTime.getYear();
            month = _EpochTime.getMonth();
            day = _EpochTime.getDay();

            epoch = _EpochTime.getEpoch();
        }

        //! EpochTime constructor.
        /*!
            \param int epoch year
            \param double epoch day
        Contructs EpochTime from epoch yeahr and day
        */
        public EpochTime(int epochYear, double EpochDay)
        {
            epoch = EpochDay;
            if (epochYear < 1900)
            {
                if (epochYear < 50)
                    year = epochYear + 2000;
                else
                    year = epochYear + 1900;
            }
            else
            {
                year = epochYear;
            }
            dayToDate(epochYear, EpochDay);
        }

        //! EpochTime constructor.
        /*!
        \return double Sidreal Time at current UTC-Time
        */
        private double getSiderealTime()
        {
            double rt = 86400.0;

            double jd = getJulianGreenwitch();
            double uT = (jd + 0.5) % 1.0;
            jd = jd - uT;
            double du = jd - 2451545.0;
            double tu = du / 36525.0;
            double gmst = 24110.54841 + 8640184.812866 * tu + 0.093104 * tu * tu - 0.0000062 * tu * tu * tu;
            double gms = (gmst + rt * 1.00273790934 * uT) % rt;
            double st = twoPi * gms / rt;
            //in radians*/
            return st;
        }

        //! retuns the Julian Day for current Time
        /*!
        \return double JulianDay
        */
        private double getJulianGreenwitch()
        {
            double a = Math.Floor((14.0 - month) / 12.0);
            double y = year + 4800 - a;
            double m = month + 12 * a - 3;
            double jd = day + Math.Floor((153 * m + 2) / 5) + 365 * y
                            + Math.Floor(y / 4) - Math.Floor(y / 100)
                            + Math.Floor(y / 400) - 32045;
            double tt = epoch % 1;
            jd = jd - 0.5 + tt;
            return jd;
        }

        //! returns the local Sidreal Time at a given Longitude
        /*!
        \return double local Sidreal Time
        */
        public double getLocalSiderealTime(double longitude = 0.0)
        {
            double st = getSiderealTime();
            double lst = st + (longitude * toRadians);
            if (lst < 0.0)
            {
                lst += twoPi;
            }
            return lst;
        }

        //! returns the Day of the Year and time as fraction of a day
        /*!
        \return double DayOfYear
        */
        public double getDayOfYear()
        {
            int[] months = new int[12] {31,28,31,30,31,30,31,31,30,31,30,31};
            if (year % 4 == 0)
            {
                months[1] = 29;
            }

            double doy = 0;
            for (int i = 0; i < month-1; i++)
            {
                doy = doy + months[i];
            }
            doy = doy + day;
            double t = (hour + ((minutes + ( seconds / 60.0)) / 60.0)) / 24.0;
            double dayY = doy + t;
            return dayY;
        }

        //! returns the current time as readable string
        /*!
        \return string Time HH:MM:SS.ss
        */
        public override string ToString()
        {
            double time = epoch - Math.Floor(epoch);
            time = time * 24;
            int hour = Convert.ToInt32(Math.Floor(time));
            time = (time - hour) * 60.0;
            int min = Convert.ToInt32(Math.Floor(time));
            int sec = Convert.ToInt32 ((time - min) * 60.0 );
            string date = getDay() + "." + getMonth().ToString("00") + "." + getYear() + "-" + hour.ToString("00") + ":" +
                          min.ToString("00") + ":" +
                          sec.ToString("00");
            return date;
        }

        //! EpochTime returns the Date of a year and epoch
        /*!
        \param int Yeahr
        \param double Epoch
        \return string HH:MM:SS.ss DD.MM.YYYY
        */
        private void dayToDate(int year, double epoch)
        {
            //check Yeahr to find the the right Date
            //Currently will only work until 2058
            //Currently oldest man made object Vangard1 Launched 1958
            if (year < 1900)
            {
                if (year < 58)
                    year = year + 2000;
                else
                    year = year + 1900;
            }
            int dayOfYear = Convert.ToInt32(Math.Floor(epoch));
            
            int[] months = new int[12] {31,28,31,30,31,30,31,31,30,31,30,31};
            if( year % 4 == 0)
            {
                months[1] = 29; 
            }
            int i = 1;
            int temp = 0;
            int dY = dayOfYear;

            while( dY > temp + months[i - 1] && i < 12 )
            {
                temp = temp + months[i - 1];
                i++;
            }
            month = i;
            day = dayOfYear - temp;

            double time = (epoch - dayOfYear) * 24.0;
            hour = Convert.ToInt32(Math.Floor(time));
            time = (time - hour) * 60.0;
            minutes = Convert.ToInt32(Math.Floor(time));
            seconds = (time - minutes) * 60.0;
        }

        //! Convert epoch to human readable time
        /*!
        * Converts the epoch and year value to hh:mm:ss.ssss format
        */
        private void convertEpochToTime()
        {
            double time = epoch - Math.Floor(epoch);
            time = time * 24;
            hour = Convert.ToInt32(Math.Floor(time));
            time = (time - hour) * 60.0;
            minutes = Convert.ToInt32(Math.Floor(time));
            seconds = (time - minutes) * 60.0;
        }

        //! adds an tick in seconds on current time
        /*!
        \param double tick
        * with each tick the time is increased until 365 days (366 days if
        * current year is a leap year) then the epoch will be set to 0.0 and 
        * the year is counted up.
        */
        public void addTick(double tick)
        {
            epoch = epoch + (tick / secToDay );
            if (year % 4 == 0)
            {
                days = 366;
            }
            else
            {
                days = 365;
            }
            if(epoch >= days + 1)
            {
                epoch = epoch % 1;
                year++;
            }
            convertEpochToTime();
        }

        public void addMinutes(double min)
        {
            this.addTick(min * 60.0);
        }

        public void addHours(double hour)
        {
            this.addMinutes(hour * 60.0);
        }

        public void addDays(double day)
        {
            this.addHours(day * 24.0);
        }

        //! returns the Time in Seconds of this object
        /*!
        \return double seconds
        */
        private double getSeconds()
        {
            convertEpochToTime();
            //double t_s = epoch % 1;
            double t_s = (hour * 60.0 * 60.0) + (minutes * 60.0) + seconds;
            return t_s;
        }

        //! returns the hour of this object
        /*!
            \return int Hour
        */
        public int getHour()
        {
            return hour;
        }

        //! returns the minute of this object
        /*!
            \return int minute
        */
        public int getMin()
        {
            return minutes;
        }

        //! returns the second of this object
        /*!
            \return double seconds
        */
        public double getSec()
        {
            return seconds;
        }

        //! returns the year of this object
        /*!
            \return int year
        */
        public int getYear()
        {
            return year;
        }

        //! returns the month of this object
        /*!
            \return int month
        */
        public int getMonth()
        {
            return month;
        }

        //! returns the Day of this object
        /*!
            \return int day
        */
        public int getDay()
        {
            return day;
        }

        //! returns the epoch of this object
        /*!
            \return double epoch
        */
        public double getEpoch()
        {
            return epoch;
        }

        //! convert to DateTime
        /*!
            \return DateTime time
        */
        public DateTime toDateTime()
        {
            dayToDate(year, epoch);
            DateTime date = new DateTime(year, month, day, hour, minutes, Convert.ToInt32(Math.Floor(seconds)));
            date = date.AddMilliseconds((seconds - Math.Floor(seconds))*1000);
            return date;
        }

        //! Returns the Date and Time in JulianDate
        /*!
        \return double JulianDate
        */
        public double toJulianDate()
        {
            double a = Math.Floor((14.0 - month) / 12.0);
            double y = year + 4800 - a;
            double m = month + 12 * a - 3;
            double jd = day + Math.Floor((153 * m + 2) / 5) + 365 * y + Math.Floor(y / 4) - Math.Floor(y / 100)
                   + Math.Floor(y / 400) - 32045;
            jd = jd - 0.5;
            double t = (hour + ((minutes + (seconds / 60.0)) / 60.0)) / 24.0;
            jd = jd + t;
            return jd;
        }

        //! Compares EpochTime with another EpochTime
        /*!
        \return int 1 if equals else -1
        */
        public int CompareTo(EpochTime other)
        {
            if (other == null)
                return 1;
            if ( year == other.year)
            {
                if (epoch == other.epoch)
                    return 0;
                else if (epoch < other.epoch)
                    return -1;
                else
                    return 1;
            }
            else if (year < other.year)
                return -1;
            else
                return 1;
        }

        //! get Time String
        /*!
        \return the time as Humanreadable string HH:mm:ss
        */
        public string getTimeToString()
        {
            double time = epoch - Math.Floor(epoch);
            time = time * 24;
            int hour = Convert.ToInt32(Math.Floor(time));
            time = (time - hour) * 60.0;
            int min = Convert.ToInt32(Math.Floor(time));
            int sec = Convert.ToInt32((time - min) * 60.0);
            string stringTime = hour.ToString("00") + ":" +
                          min.ToString("00") + ":" +
                          sec.ToString("00");
            return stringTime;
        }

        //! get Date String
        /*!
        \return the Date as Humanreadable string dd.MM.yyyy
        */
        public string getDateToString()
        {
            string date = getDay() + "." + getMonth().ToString("00") + "." + getYear();
            return date;
        }

        //! < Operator
        /*!
        \return boolean if EpochTime < EpochTime
        */
        public static bool operator <(EpochTime e1, EpochTime e2)
        {
            return (e1.getYear() < e2.getYear() || e1.getEpoch() < e2.getEpoch());
        }

        //! > Operator
        /*!
        \return boolean if EpochTime > EpochTime
        */
        public static bool operator >(EpochTime e1, EpochTime e2)
        {
            return (e1.getYear() > e2.getYear() || e1.getEpoch() > e2.getEpoch());
        }

        //! == Operator
        /*!
        \return double EpochTime_1 == EpochTime_2
        */
        public static bool operator ==(EpochTime e1, EpochTime e2)
        {
            return (e1.getEpoch() == e2.getEpoch() &&
                e1.getYear() == e2.getYear());
        }

        //! != Operator
        /*!
        \return double EpochTime_1 != EpochTime_2
        */
        public static bool operator !=(EpochTime e1, EpochTime e2)
        {
            return !(e1.getEpoch() == e2.getEpoch() &&
                e1.getYear() == e2.getYear());
        }

        //! - Operator
        /*!
        \return double EpochTime_1 - EpochTime_2 in days
        */
        public static double operator -(EpochTime e1, EpochTime e2)
        {
            if (e1 < e2)
                throw new ArgumentException();
            if (e1.getYear() > e2.getYear())
            {
                double days = 365.0;
                if (DateTime.IsLeapYear(e2.getYear()))
                {
                    days = 366.0;
                }
                return e1.getEpoch() + (days - e2.getEpoch());
            }
            else
            {
                return e1.getEpoch() - e1.getEpoch();
            }
        }
    }
}
