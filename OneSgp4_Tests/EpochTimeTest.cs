using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using One_Sgp4;

namespace OneSgp4_Tests
{
    [TestFixture]
    public class EpochTimeTest
    {

        //[TestCase("2018-12-31 12:00:00,000", 2019, 0.5)]    // Should this work? It's mentioned here: https://celestrak.com/columns/v04n03/#FAQ02 
        [TestCase("2019-01-01 12:00:00,000", 2019, 1.5 )]
        [TestCase("2019-02-01 12:00:00,000", 2019, 32.5 )]
        [TestCase("2019-03-01 12:00:00,000", 2019, 60.5 )]
        [TestCase("2019-04-01 12:00:00,000", 2019, 91.5 )]
        [TestCase("2019-05-01 12:00:00,000", 2019, 121.5)]
        [TestCase("2019-06-01 12:00:00,000", 2019, 152.5)]
        [TestCase("2019-07-01 12:00:00,000", 2019, 182.5 )]
        [TestCase("2019-08-01 12:00:00,000", 2019, 213.5 )]
        [TestCase("2019-09-01 12:00:00,000", 2019, 244.5 )]
        [TestCase("2019-10-01 12:00:00,000", 2019, 274.5 )]
        [TestCase("2019-11-01 12:00:00,000", 2019, 305.5 )]
        [TestCase("2019-12-01 12:00:00,000", 2019, 335.5 )]
        [TestCase("2020-01-01 12:00:00,000", 2020, 1.5)]      // leapyear
        [TestCase("2020-02-01 12:00:00,000", 2020, 32.5)]
        [TestCase("2020-03-01 12:00:00,000", 2020, 61.5)]
        [TestCase("2020-04-01 12:00:00,000", 2020, 92.5)]
        [TestCase("2020-05-01 12:00:00,000", 2020, 122.5)]
        [TestCase("2020-06-01 12:00:00,000", 2020, 153.5)]
        [TestCase("2020-07-01 12:00:00,000", 2020, 183.5)]
        [TestCase("2020-08-01 12:00:00,000", 2020, 214.5)]
        [TestCase("2020-09-01 12:00:00,000", 2020, 245.5)]
        [TestCase("2020-10-01 12:00:00,000", 2020, 275.5)]
        [TestCase("2020-11-01 12:00:00,000", 2020, 306.5)]
        [TestCase("2020-12-01 12:00:00,000", 2020, 336.5)] 
        [TestCase("2017-11-22 14:13:59,612", 2017, 326.59305107)]     // Bug when ToDate() rounds 59,612 to 60 seconds. (-> Ms setting never worked before)
        public void TestDayofYearCalculation(string dt, int year, double doy) {
            DateTime dateTime = DateTime.ParseExact(dt, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
            EpochTime et = new EpochTime(year, doy);

            Assert.That(et.getYear(), Is.EqualTo(dateTime.Year));
            Assert.That(et.getMonth(), Is.EqualTo(dateTime.Month));
            Assert.That(et.getDay(), Is.EqualTo(dateTime.Day));
            Assert.That(et.getHour(), Is.EqualTo(dateTime.Hour));
            Assert.That(et.getMin(), Is.EqualTo(dateTime.Minute));
            Assert.That((int)et.getSec(), Is.EqualTo(dateTime.Second));

            Assert.That(et.getDayOfYear(), Is.EqualTo(doy));
            Assert.That(et.toDateTime(), Is.EqualTo(dateTime));
        }

        [TestCase(2.524218, 9, 0, 0, 1995, 10, 1,0)]
        [TestCase(0.3865996, 0, 0, 0, 2019, 12, 30, -76.0)]
        [TestCase(3.3327650, 0, 0, 0, 2018, 2, 15, 46)]
        public void TestSidrealTime(double res, int hh, int mm, int ss, int yyyy, int MM, int dd, double longitude)
        {
            //9,0,0,1995,10,1
            EpochTime testTime = new EpochTime(hh, mm, ss, yyyy, MM, dd);
            double time = testTime.getLocalSiderealTime(longitude);
            Assert.GreaterOrEqual(time, res);
            Assert.Less(time, res + 0.000001);
        }

    }
}
