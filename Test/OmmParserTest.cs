using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using One_Sgp4;
using One_Sgp4.omm;

namespace Test
{
    class OmmParserTest
    {
        [Test]
        public void testValidtXml()
        {
            string testpath = TestContext.CurrentContext.TestDirectory + @"\resources\singleOmm.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(testpath);

            List<Omm> result = ParserOMM.Parse(doc);
            Assert.That(result.Count, Is.EqualTo(1));

            Assert.That(result[0].getName, Is.EqualTo("ISS (ZARYA)"));
            Assert.That(result[0].getId, Is.EqualTo("1998-067A"));
            Assert.That(result[0].getCenterName, Is.EqualTo("EARTH"));
            Assert.That(result[0].getRefFrame, Is.EqualTo("TEME"));
            Assert.That(result[0].getTimeSystem, Is.EqualTo("UTC"));
            Assert.That(result[0].getModel, Is.EqualTo("SGP4"));

            string dateString = result[0].getEpochTime().getDateToString();
            Assert.That(dateString, Is.EqualTo("14.01.2024"));
            Assert.That(result[0].getMeanMotion, Is.EqualTo(15.49370626));
            Assert.That(result[0].getEccentricity, Is.EqualTo(0.0004916));
            Assert.That(result[0].getInclination, Is.EqualTo(51.6428));
            Assert.That(result[0].getAscendingNode, Is.EqualTo(3.1154));
            Assert.That(result[0].getPareicenter, Is.EqualTo(89.2408));
            Assert.That(result[0].getMeanAnomoly, Is.EqualTo(12.3067));

            Assert.That(result[0].getEphemeris, Is.EqualTo(0.0));
            Assert.That(result[0].getNoradCatId, Is.EqualTo("25544"));
            Assert.That(result[0].getElementSet, Is.EqualTo(999));
            Assert.That(result[0].getRevAtEpoch, Is.EqualTo(43452));
            Assert.That(result[0].getDragTerm, Is.EqualTo(0.26444E-3));
            Assert.That(result[0].getFirstMeanMotion, Is.EqualTo(0.14338E-3));
            Assert.That(result[0].getSecondMeanMotion, Is.EqualTo(0.0));
        }

        [Test]
        public void compareOmmTle()
        {
            string tleString1 = "1 25544U 98067A   24014.23659323  .00014338  00000+0  26444-3 0  9995";
            string tleString2 = "2 25544  51.6428   3.1154 0004916  89.2408  12.3067 15.49370626434523";
            Tle resultTle = ParserTLE.parseTle(tleString1, tleString2);

            string testpath = TestContext.CurrentContext.TestDirectory + @"\resources\singleOmm.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(testpath);

            List<Omm> result = ParserOMM.Parse(doc);

            Assert.That(resultTle.getEccentriciy, Is.EqualTo(result[0].getEccentricity()));
            Assert.That(resultTle.getFirstMeanMotion, Is.EqualTo(result[0].getFirstMeanMotion()));
            Assert.That(resultTle.getMeanMotion, Is.EqualTo(result[0].getMeanMotion()));
            Assert.That(resultTle.getDrag, Is.EqualTo(result[0].getDragTerm()));
            Assert.That(resultTle.getEphemeris, Is.EqualTo(result[0].getEphemeris()));
            Assert.That(resultTle.getSecondMeanMotion, Is.EqualTo(result[0].getSecondMeanMotion()));

            Assert.That(resultTle.getInclination, Is.EqualTo(result[0].getInclination()));
            Assert.That(2000+resultTle.getEpochYear(), Is.EqualTo(result[0].getEpochTime().getYear()));
            Assert.That(resultTle.getMeanAnomoly, Is.EqualTo(result[0].getMeanAnomoly()));
            Assert.That(resultTle.getPerigee, Is.EqualTo(result[0].getPareicenter()));
            Assert.That(resultTle.getRelevationNumber, Is.EqualTo(result[0].getRevAtEpoch()));
            Assert.That(resultTle.getRightAscendingNode, Is.EqualTo(result[0].getAscendingNode()));
            Assert.That(resultTle.getClassification(), Is.EqualTo(result[0].getClassification()));
        }

        [Test]
        public void runOrbitCalc()
        {
            string tleString1 = "1 25544U 98067A   24014.23659323  .00014338  00000+0  26444-3 0  9995";
            string tleString2 = "2 25544  51.6428   3.1154 0004916  89.2408  12.3067 15.49370626434523";
            Tle tleData = ParserTLE.parseTle(tleString1, tleString2);

            string testpath = TestContext.CurrentContext.TestDirectory + @"\resources\singleOmm.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(testpath);

            Omm ommData = ParserOMM.Parse(doc)[0];

            EpochTime startTime = new EpochTime(2024, 14.50);
            EpochTime stopTime = new EpochTime(2024, 15.0);

            One_Sgp4.Sgp4 sgp4PropagatorTle = new Sgp4(tleData, Sgp4.wgsConstant.WGS_84);
            sgp4PropagatorTle.runSgp4Cal(startTime, stopTime, 1 / 30.0);
            List<One_Sgp4.Sgp4Data> resulTletDataList = sgp4PropagatorTle.getResults();

            One_Sgp4.Sgp4 sgp4PropagatorOmm = new Sgp4(ommData, Sgp4.wgsConstant.WGS_84);
            sgp4PropagatorOmm.runSgp4Cal(startTime, stopTime, 1 / 30.0);
            List<One_Sgp4.Sgp4Data> resulOmmDataList = sgp4PropagatorOmm.getResults();

            Assert.That(resulTletDataList.Count, Is.EqualTo(resulOmmDataList.Count));
            Assert.That(resulTletDataList, Is.EqualTo(resulTletDataList));
        }

        [Test]
        public void testParseMultipleOmm()
        {
            string testpath = TestContext.CurrentContext.TestDirectory + @"\resources\omm.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(testpath);

            List<Omm> result = ParserOMM.Parse(doc);

            Assert.That(result.Count, Is.EqualTo(20));
        }
    }
}
