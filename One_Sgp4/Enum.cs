/*
 * Copyright 2017 Nikolai Reed <reed@1manprojects.de>
 *
 * Licensed under the GNU Lesser General Public License v3 (LGPL-3)
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.gnu.org/licenses/lgpl-3.0.de.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace One_Sgp4
{
    public class Enum
    {
        /**
      * \brief Enum Satellite class
      *
      * This class defnies the classification of the Satellites as 
      * defined in TLE dokumentation
      */
        public enum satClass
        {
            UNCLASSIFIED = 0, //!< int 0 unclassified satellite 
            CLASSIFIED = 1, //!< int 1 classified satellite
            SECRET = 2 //!< int 2 secret satellite
        };
    }
}
