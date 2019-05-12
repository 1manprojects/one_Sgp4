using System;
using System.Collections.Generic;
using System.Text;

namespace One_Sgp4
{
    /**
   * \brief PassDetail class
   *
   * This class holds more detailed information over elevation, azimuth for a satellite pass
   */
    public class PassDetail
    {
        public EpochTime time;  //!<  EpochTime timepoint
        public double elevation;//!< double elevation at timepoint in degrees
        public double azimuth; //!< double azimuth at timepoint in degrees
        public double range;  //!< double range to satellite in km

        //! Constructor
        /*!
        \param EpochTime timepoint
        \param double elevation at timepoint
        \param double azimuth at timepoint
        \pram double range at timepoint
        */
        public PassDetail(EpochTime timepoint, double elevation, double azimuth, double range)
        {
            this.time = timepoint;
            this.elevation = elevation;
            this.azimuth = azimuth;
            this.range = range;
        }

        override
        public String ToString()
        {
            return string.Format("{0} : Elevation: {1}°, Azimuth {2}°, Range {3}km ",time.getTimeToString(), elevation, azimuth, range);
        }
    }
}
