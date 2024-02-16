namespace One_Sgp4.omm
{
    public class Omm
    {
        private string name;
        private string id;
        private string timeSystem;
        private string refFrame;
        private string centerName;
        private string model;

        private EpochTime epoch;
        private double meanMotion;
        private double eccentricity;
        private double inclination;
        private double ascendingNode;
        private double pareicenter;
        private double meanAnomoly;

        private double ephemeris;
        private Enum.satClass classification;
        private string noradCatId;
        private int elementSet;
        private double revAtEpoch;
        private double dragTerm;
        private double firstMeanMotion;
        private double secondMeanMotion;

        public Omm(string name, string id, string timeSystem, string refFrame, string centerName, string model, EpochTime epoch, double meanMotion, double eccentricity, double inclination, double ascendingNode, double pareicenter, double meanAnomoly, double ephemeris, Enum.satClass classification, string noradCatId, int elementSet, double revAtEpoch, double dragTerm, double firstMeanMotion, double secondMeanMotion)
        {
            this.name = name;
            this.id = id;
            this.timeSystem = timeSystem;
            this.refFrame = refFrame;
            this.centerName = centerName;
            this.model = model;

            this.epoch = epoch;
            this.meanMotion = meanMotion;
            this.eccentricity = eccentricity;
            this.inclination = inclination;
            this.ascendingNode = ascendingNode;
            this.pareicenter = pareicenter;
            this.meanAnomoly = meanAnomoly;

            this.ephemeris = ephemeris;
            this.classification = classification;
            this.noradCatId = noradCatId;
            this.elementSet = elementSet;
            this.revAtEpoch = revAtEpoch;
            this.dragTerm = dragTerm;
            this.firstMeanMotion = firstMeanMotion;
            this.secondMeanMotion = secondMeanMotion;
        }

        public double getMeanMotion()
        {
            return meanMotion;
        }

        public EpochTime getEpochTime()
        {
            return epoch;
        }

        public double getMeanAnomoly()
        {
            return meanAnomoly;
        }

        public double getEphemeris()
        {
            return ephemeris;
        }

        public int getClassification()
        {
            return (int)classification;
        }

        public string getName()
        {
            return name;
        }

        public string getId()
        {
            return id;
        }

        public string getTimeSystem()
        {
            return timeSystem;
        }

        public string getRefFrame()
        {
            return refFrame;
        }

        public string getCenterName()
        {
            return centerName;
        }

        public string getModel()
        {
            return model;
        }

        public double getEccentricity()
        {
            return eccentricity;
        }
        public double getInclination()
        {
            return inclination;
        }
        public double getAscendingNode()
        {
            return ascendingNode;
        }
        public double getPareicenter()
        {
            return pareicenter;
        }
        public string getNoradCatId()
        {
            return noradCatId;
        }
        public int getElementSet()
        {
            return elementSet;
        }
        public double getRevAtEpoch()
        {
            return revAtEpoch;
        }
        public double getDragTerm()
        {
            return dragTerm;
        }
        public double getFirstMeanMotion()
        {
            return firstMeanMotion;
        }
        public double getSecondMeanMotion()
        {
            return secondMeanMotion;
        }


    }
}
