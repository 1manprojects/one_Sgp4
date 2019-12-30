using NUnit.Framework;
using One_Sgp4;

namespace OneSgp4_Tests
{
    [TestFixture]
    public class SatTest
    {
        [TestCase(15, 12, 0, 2019, 12, 24)]
        [TestCase(0, 12, 0, 2019, 12, 25)]
        [TestCase(16, 12, 0, 2019, 12, 30)]
        [TestCase(23, 27, 0, 2020, 1, 1)]
        [TestCase(10, 1, 0, 2020, 2, 18)]
        [TestCase(11, 15, 0, 2020, 1, 15)]
        [TestCase(21, 50, 0, 2020, 3, 2)]
        [TestCase(20, 30, 0, 2020, 1, 3)]
        public void TestSatGroundPosition(int hh, int mm, int ss, int yyyy, int MM, int dd)
        {
            Tle tleISS = ParserTLE.parseTle(
                "1 25544U 98067A   19356.46068278  .00000035  00000-0  86431-5 0  9990",
                "2 25544  51.6420 147.9381 0007793  61.6458  55.7201 15.50124783204461",
                "ISS 1");


            EpochTime testTime = new EpochTime(hh, mm, ss, yyyy, MM, dd);
            Sgp4Data data = SatFunctions.getSatPositionAtTime(tleISS, testTime, Sgp4.wgsConstant.WGS_84);
            Assert.IsNotNull(data);
            Coordinate ground = SatFunctions.calcSatSubPoint(testTime, data, Sgp4.wgsConstant.WGS_84);
            Assert.Greater(ground.getHeight(), 0);
            Assert.LessOrEqual(ground.getLongitude(), 180.0);
            Assert.Greater(ground.getLongitude(), -180.0);
            Assert.LessOrEqual(ground.getLatetude(), 90.0);
            Assert.Greater(ground.getLatetude(), -90.0);

        }
    }
}