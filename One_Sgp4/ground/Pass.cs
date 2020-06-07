/*
 * Copyright 2018 Nikolai Reed <reed@1manprojects.de>
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
namespace One_Sgp4
{
    public class Pass
    {
        private Coordinate observer; //!< Coordinates of observer
        private PassDetail start; //!< Pass details at start of pass
        private PassDetail maxElevation; //!< Pass details at max elevation
        private PassDetail end; //!< Pass details at end of pass

        //! Constructor
        /*!
        \param Coordinate of observer
        \param PassDetail at start of pass
        \param PassDetail at max elevation
        \param PassDetail at end of pass
        */
        public Pass(Coordinate Observer, PassDetail passStart, PassDetail maxElevation, PassDetail passEnd)
        {
            this.observer = Observer;
            this.start = passStart;
            this.end = passEnd;
            this.maxElevation = maxElevation;
        }

        //! get Coorindates of observer
        /*!
        \return Coordinate
        */
        public Coordinate getObserverLocation()
        {
            return this.observer;
        }

        //! get the EpochTime at start of the pass
        /*!
        \return EpochTime
        */
        public EpochTime getStartEpoch()
        {
            return this.start.time;
        }

        //! get the EpochTime at end
        /*!
        \return EpochTime
        */
        public EpochTime getEndEpoch()
        {
            return this.end.time;
        }

        //! get PassDetails at start
        /*!
        \return PassDetail
        */
        public PassDetail getPassDetailsAtStart()
        {
            return this.start;
        }

        //! get PassDetail at end
        /*!
        \return PassDetail
        */
        public PassDetail getPassDetailsAtEnd()
        {
            return this.end;
        }

        //! get PassDetails at max elevation
        /*!
        \return PassDetail
        */
        public PassDetail getPassDetailOfMaxElevation()
        {
            return this.maxElevation;
        }

        //! overriden ToString()
        /*!
        \return String
        */
        public override string ToString()
        {
            return string.Format("{0}:\nStart: {1}\nMax Elevation: {2}\nEnd: {3}", start.time.ToString(), start.ToString(), maxElevation.ToString(), end.ToString());
        }
    }
}
