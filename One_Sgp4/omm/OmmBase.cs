namespace One_Sgp4.omm
{
    public class OmmBase
    {
        private static string centerName;
        private static string id;
        private static string model;
        private static string name;
        private static string refFrame;
        private static string timeSystem;
        private double ascendingNode;
        private Enum.satClass classification;
        private double dragTerm;
        private double eccentricity;
        private string ElementSet;
        private int ElementSetNr;

        private double ephemeris;

        private EpochTime epoch;
        private double firstMeanMotion;
        private double inclination;
        private double meanAnomoly;
        private double meanMotion;
        private string noradCatId;
        private double pareicenter;
        private double revAtEpoch;
        private double secondMeanMotion;

        public EpochTime getEpochTime()
        {
            return epoch;
        }

        public double getMeanMotion()
        {
            return meanMotion;
        }
    }
}