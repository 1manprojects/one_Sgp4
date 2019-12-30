using System;
using System.IO;
using NUnit.Framework;
using One_Sgp4;

namespace Test
{
    class TleTest
    {
        [Test]
        public void TleParseFromLinesShouldSucceed()
        {
            //             0.........1.........2.........3.........4.........5.........6.........7   
            string line1 = "1 42784U 17036Vvv 17175.91623346  .00001083  00000-0  52625-4 0  9993";
            string line2 = "2 42784  97.4499 235.6602 0011188 243.9018 116.1066 15.20524655   207";

            Tle t = ParserTLE.parseTle(line1, line2);

            Assert.That(t.isValidData, Is.True);
            Assert.That(t.getClassification(), Is.EqualTo(0));
            Assert.That(t.getDrag(), Is.EqualTo(0.000052625));
            Assert.That(t.getEccentriciy(), Is.EqualTo(0.0011188));
            Assert.That(t.getEphemeris(), Is.EqualTo(0));
            Assert.That(t.getEpochDay(), Is.EqualTo(175.91623346));
            Assert.That(t.getEpochYear(), Is.EqualTo(17));
            Assert.That(t.getFirstMeanMotion(), Is.EqualTo(.00001083));
            Assert.That(t.getInclination(), Is.EqualTo(97.4499));
            Assert.That(t.getMeanAnomoly(), Is.EqualTo(116.1066));
            Assert.That(t.getMeanMotion(), Is.EqualTo(15.20524655));
            Assert.That(t.getName(), Is.EqualTo("1736Vvv"));
            Assert.That(t.getNoradID(), Is.EqualTo("42784"));
            Assert.That(t.getPerigee(), Is.EqualTo(243.9018));
            Assert.That(t.getPice(), Is.EqualTo("Vvv"));
            Assert.That(t.getRelevationNumber(), Is.EqualTo(20));
            Assert.That(t.getRightAscendingNode(), Is.EqualTo(235.6602));
            Assert.That(t.getSatNumber(), Is.EqualTo(42784));
            Assert.That(t.getSecondMeanMotion(), Is.EqualTo(0));
            Assert.That(t.getSetNumber(), Is.EqualTo(999));
            Assert.That(t.getStartNr(), Is.EqualTo(36));
            Assert.That(t.getStartYear(), Is.EqualTo(17));

            t = ParserTLE.parseTle(line1, line2, "Pegasus");
            Assert.That(t.isValidData, Is.True);
            Assert.That(t.getClassification(), Is.EqualTo(0));
            Assert.That(t.getDrag(), Is.EqualTo(0.000052625));
            Assert.That(t.getEccentriciy(), Is.EqualTo(0.0011188));
            Assert.That(t.getEphemeris(), Is.EqualTo(0));
            Assert.That(t.getEpochDay(), Is.EqualTo(175.91623346));
            Assert.That(t.getEpochYear(), Is.EqualTo(17));
            Assert.That(t.getFirstMeanMotion(), Is.EqualTo(.00001083));
            Assert.That(t.getInclination(), Is.EqualTo(97.4499));
            Assert.That(t.getMeanAnomoly(), Is.EqualTo(116.1066));
            Assert.That(t.getMeanMotion(), Is.EqualTo(15.20524655));
            Assert.That(t.getName(), Is.EqualTo("Pegasus"));
            Assert.That(t.getNoradID(), Is.EqualTo("42784"));
            Assert.That(t.getPerigee(), Is.EqualTo(243.9018));
            Assert.That(t.getPice(), Is.EqualTo("Vvv"));
            Assert.That(t.getRelevationNumber(), Is.EqualTo(20));
            Assert.That(t.getRightAscendingNode(), Is.EqualTo(235.6602));
            Assert.That(t.getSatNumber(), Is.EqualTo(42784));
            Assert.That(t.getSecondMeanMotion(), Is.EqualTo(0));
            Assert.That(t.getSetNumber(), Is.EqualTo(999));
            Assert.That(t.getStartNr(), Is.EqualTo(36));
            Assert.That(t.getStartYear(), Is.EqualTo(17));
        }

        [Test]
        public void TleParseFromCorruptLinesShouldFail()
        {
            string line1 = "1 42784U 17036V   17175.91623346  .00001083  00000 - 0  52625 - 4 0  9993";
            string line2 = "2 42784  97.4499 235.6602 0011188 243.9018 116.1066 15.20524655   207";

            try
            {
                Tle t = ParserTLE.parseTle(line1, line2);
                Assert.Fail("This should raise an exception!");
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.TypeOf<InvalidDataException>());
                Assert.That(ex.Message, Contains.Substring("parse error"));
            }
        }

        [TestCase("1 42784U 17036V   17175.91623346  .00001083  00000-0  52625-4 0  9995",
                  "2 42784  97.4499 235.6602 0011188 243.9018 116.1066 15.20524655   207",
        TestName = "Checksum error in line1")]

        [TestCase("1 42784U 17036V   17175.91623346  .00001083  00000-0  52625-4 0  9993",
                  "2 42784  97.4499 235.6602 0011188 243.9018 116.1066 15.20524655   204",
        TestName = "Checksum error in line2")]

        [TestCase("1 42784U 17036V   17175.91623346  .00001083  00000-0  52625-4 0  999x",
                  "2 42784  97.4499 235.6602 0011188 243.9018 116.1066 15.20524655   204",
        TestName = "Checksum error in both lines")]
        public void TleParseFromLinesWithChecksumErrorShouldFail(string line1, string line2)
        {
            try
            {
                Tle t = ParserTLE.parseTle(line1, line2);
                Assert.Fail("This should raise an exception!");
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.TypeOf<InvalidDataException>());
                Assert.That(ex.Message, Contains.Substring("checksum error"));
            }
        }
    }
}
