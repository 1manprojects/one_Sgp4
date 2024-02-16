using System;
using System.Globalization;
using NUnit.Framework;
using One_Sgp4;
using One_Sgp4.omm;

namespace Test
{
    class EpochTimeTest
    {
        //[TestCase("2018-12-31 12:00:00,000", 2019, 0.5)]    // Should this work? It's mentioned here: https://celestrak.com/columns/v04n03/#FAQ02 
        [TestCase("2019-01-01 12:00:00,000", 2019, 1.5)]
        [TestCase("2019-02-01 12:00:00,000", 2019, 32.5)]
        [TestCase("2019-03-01 12:00:00,000", 2019, 60.5)]
        [TestCase("2019-04-01 12:00:00,000", 2019, 91.5)]
        [TestCase("2019-05-01 12:00:00,000", 2019, 121.5)]
        [TestCase("2019-06-01 12:00:00,000", 2019, 152.5)]
        [TestCase("2019-07-01 12:00:00,000", 2019, 182.5)]
        [TestCase("2019-08-01 12:00:00,000", 2019, 213.5)]
        [TestCase("2019-09-01 12:00:00,000", 2019, 244.5)]
        [TestCase("2019-10-01 12:00:00,000", 2019, 274.5)]
        [TestCase("2019-11-01 12:00:00,000", 2019, 305.5)]
        [TestCase("2019-12-01 12:00:00,000", 2019, 335.5)]
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
        public void TestDayofYearCalculation(string dt, int year, double doy)
        {
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

        [TestCase(2.524218, 9, 0, 0, 1995, 10, 1, 0)]
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

        [TestCase(2000, 11, 2, 14, 25, 23, DateTimeKind.Utc)]
        [TestCase(2020, 01, 12, 09, 00, 00, DateTimeKind.Local)]
        [TestCase(1999, 11, 2, 14, 25, 23, DateTimeKind.Utc)]
        [TestCase(2013, 05, 19, 23, 00, 15, DateTimeKind.Utc)]
        [TestCase(2020, 02, 29, 00, 54, 44, DateTimeKind.Local)]    //Leap Year
        public void TestDateConversion(int yyyy, int MM, int dd, int hh, int mm, int ss, DateTimeKind timeKind)
        {
            DateTime dt = new DateTime(yyyy, MM, dd, hh, mm, ss, timeKind);
            EpochTime et = new EpochTime(dt.ToUniversalTime());
            Assert.That(et.getYear(), Is.EqualTo(dt.ToUniversalTime().Year));
            Assert.That(et.getMonth(), Is.EqualTo(dt.ToUniversalTime().Month));
            Assert.That(et.getDay(), Is.EqualTo(dt.ToUniversalTime().Day));
            Assert.That(et.getHour(), Is.EqualTo(dt.ToUniversalTime().Hour));
            Assert.That(et.getMin(), Is.EqualTo(dt.ToUniversalTime().Minute));
            Assert.That((int)et.getSec(), Is.EqualTo(dt.ToUniversalTime().Second));

            DateTime ndt = et.toDateTime();
            Assert.That(ndt.Year, Is.EqualTo(dt.ToUniversalTime().Year));
            Assert.That(ndt.Month, Is.EqualTo(dt.ToUniversalTime().Month));
            Assert.That(ndt.Day, Is.EqualTo(dt.ToUniversalTime().Day));
            Assert.That(ndt.Hour, Is.EqualTo(dt.ToUniversalTime().Hour));
            Assert.That(ndt.Minute, Is.EqualTo(dt.ToUniversalTime().Minute));
            Assert.That(ndt.Second, Is.EqualTo(dt.ToUniversalTime().Second));
        }

        [TestCase(1995, 10, 1, 0, 0, 0, 2449991.5)]
        [TestCase(2020, 01, 12, 09, 00, 00, 2458860.875)]
        [TestCase(1999, 11, 2, 14, 25, 23, 2451485.10096)]
        [TestCase(2013, 05, 19, 23, 00, 15, 2456432.45851)]
        [TestCase(2020, 02, 29, 00, 54, 44, 2458908.53801)]
        public void TestJulianDate(int yyyy, int MM, int dd, int hh, int mm, int ss, double julianDate)
        {
            double e = 0.000005;
            EpochTime et = new EpochTime(hh, mm, ss, yyyy, MM, dd);
            double etJul = et.toJulianDate();
            Assert.That(etJul, Is.InRange(julianDate-e,julianDate+e));
        }

        
        [TestCase(1999,4,22,12,45,30, 3 * 86400.0, "25.04.1999-12:45:30")]
        [TestCase(1999, 12, 29, 12, 00, 00, 6 * 86400.0, "04.01.2000-12:00:00")]
        [TestCase(2020, 12, 29, 12, 00, 00, 6 * 86400.0, "04.01.2021-12:00:00")]
        [TestCase(2018, 11, 20, 05, 45, 33, 5 * 86400.0, "25.11.2018-05:45:33")]
        public void TestToString(int yyyy, int MM, int dd, int hh, int mm, int ss, double ticksToAdd, string result)
        {
            EpochTime epoch = new EpochTime(hh, mm, ss, yyyy, MM, dd);
            epoch.addTick(ticksToAdd);
            Assert.AreEqual(result, epoch.ToString());
        }

        [TestCase("2024-01-14T05:40:41.655072", "24014.23659323")]
        public void TestParseForOMMFormatString(string ommInput, string tleInput)
        {
            int epochYear = Convert.ToInt32(tleInput.Substring(0, 2));
            string epDay = tleInput.Substring(2);
            double epochDay = double.Parse(epDay, CultureInfo.GetCultureInfo("en-US"));
            EpochTime epochTle = new EpochTime(epochYear, epochDay);

            EpochTime epochOmm = ParserOMM.parseOmmEpoch(ommInput, true);

            Assert.AreEqual(epochTle.ToString(), epochOmm.ToString());
            Assert.AreEqual(epochTle.getEpoch(), epochOmm.getEpoch(), 0.0000001);
        }
    }
}
