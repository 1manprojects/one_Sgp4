using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace One_Sgp4.omm
{
    public class ParserOMM
    {
        /**
        * \brief ParserOMM class
        *
        * This class handles the reading and converting of OMM XML Format to the TLE Class information
        */

        public static List<Omm> Parse(XmlDocument ommDocument)
        {
            XmlNamespaceManager namespaces = new XmlNamespaceManager(ommDocument.NameTable);
            namespaces.AddNamespace("ns", "http://www.w3.org/2001/XMLSchema-instance");

            List<Omm> omms = new List<Omm>();
            XmlNodeList ommXmlItems = ommDocument.GetElementsByTagName("omm");

            for (int i = 0; i < ommXmlItems.Count; i++)
            {
                XmlNode ommItem = ommXmlItems.Item(i);

                XmlNode tleMetaDataNode = ommItem.SelectSingleNode("body/segment/metadata");
                XmlNode tleMeanElementsNode = ommItem.SelectSingleNode("body/segment/data/meanElements");
                XmlNode tleParametersNode = ommItem.SelectSingleNode("body/segment/data/tleParameters");

                //metadata
                string name = tleMetaDataNode.SelectSingleNode("OBJECT_NAME", namespaces).InnerText;
                string noradId = tleMetaDataNode.SelectSingleNode("OBJECT_ID").InnerText;
                string timeSystem = tleMetaDataNode.SelectSingleNode("TIME_SYSTEM").InnerText;
                string refFrame = tleMetaDataNode.SelectSingleNode("REF_FRAME").InnerText;
                string centerName = tleMetaDataNode.SelectSingleNode("CENTER_NAME").InnerText;
                string model = tleMetaDataNode.SelectSingleNode("MEAN_ELEMENT_THEORY").InnerText;

                //meanElements
                EpochTime epochTime = parseOmmEpoch(tleMeanElementsNode.SelectSingleNode("EPOCH").InnerText, timeSystem.Equals("UTC"));

                double meanMotion = parseStringToDouble(tleMeanElementsNode.SelectSingleNode("MEAN_MOTION").InnerText);
                double eccentricity = parseStringToDouble(tleMeanElementsNode.SelectSingleNode("ECCENTRICITY").InnerText);
                double inclination = parseStringToDouble(tleMeanElementsNode.SelectSingleNode("INCLINATION").InnerText);
                double ascendingNode = parseStringToDouble(tleMeanElementsNode.SelectSingleNode("RA_OF_ASC_NODE").InnerText);
                double pareicenter = parseStringToDouble(tleMeanElementsNode.SelectSingleNode("ARG_OF_PERICENTER").InnerText);
                double meanAnomoly = parseStringToDouble(tleMeanElementsNode.SelectSingleNode("MEAN_ANOMALY").InnerText);

                //tleParameters
                double ephemeris = parseStringToDouble(tleParametersNode.SelectSingleNode("EPHEMERIS_TYPE").InnerText);
                Enum.satClass classification = ParserTLE.parseClassification(tleParametersNode.SelectSingleNode("CLASSIFICATION_TYPE").InnerText);
                string noradCatId = tleParametersNode.SelectSingleNode("NORAD_CAT_ID").InnerText;
                int elementSetNr = Int32.Parse(tleParametersNode.SelectSingleNode("ELEMENT_SET_NO").InnerText);
                double revAtEpoch = parseStringToDouble(tleParametersNode.SelectSingleNode("REV_AT_EPOCH").InnerText);
                double dragTerm = parseStringToDouble(tleParametersNode.SelectSingleNode("BSTAR").InnerText);
                double firstMeanMotion = parseStringToDouble(tleParametersNode.SelectSingleNode("MEAN_MOTION_DOT").InnerText);
                double secondMeanMotion = parseStringToDouble(tleParametersNode.SelectSingleNode("MEAN_MOTION_DDOT").InnerText);

                omms.Add(new Omm(name, noradId, timeSystem, refFrame, centerName, model,
                    epochTime, meanMotion, eccentricity, inclination, ascendingNode, pareicenter, meanAnomoly,
                    ephemeris, classification, noradCatId, elementSetNr, revAtEpoch, dragTerm, firstMeanMotion, secondMeanMotion));

            }

            return omms;
        }

        public static EpochTime parseOmmEpoch(string ommString, bool utc)
        {
            if (utc)
            {
                ommString = ommString + "Z";
            }
            DateTime dt = DateTime.ParseExact(ommString, "yyyy-MM-ddTHH:mm:ss.ffffffZ", CultureInfo.InvariantCulture);
            return new EpochTime(dt);
        }

        private static double parseStringToDouble(string rawValue)
        {
            if (rawValue.StartsWith("."))
            {
                return Double.Parse(("0" + rawValue), CultureInfo.GetCultureInfo("en-US"));
            } else
            {
                return Double.Parse(rawValue, CultureInfo.GetCultureInfo("en-US"));
            }
        }

    }
}
