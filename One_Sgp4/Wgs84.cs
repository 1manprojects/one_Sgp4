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
    public class WGS_84
    {
        /**
     * \brief WGS_84 Class definition.
     *
     * This class defines the World Geodetic System of 1984 used for the orbit
     * predictions.
     */
        public const double radiusEarthKM = 6378.137; //!< double Radius of the Earch in km
        public const double mu = 398600.5;
        public const double j2 = 0.00108262998905;
        public const double j3 = -0.00000253215306;
        public const double j4 = -0.00000161098761;
    }
}
