/*
 * Copyright 2015 Nikolai Reed <reed@1manprojects.de>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace One_Sgp4
{
    class NearEarthObjects
    {

        public int neo_isimp;
        public int neo_method;

        public double neo_aycof;
        public double neo_con41;
        public double neo_cc1;
        public double neo_cc4;
        public double neo_cc5;
        public double neo_d2;
        public double neo_d3;
        public double neo_d4;
        public double neo_delmo;
        public double neo_eta;
        public double neo_argpdot;
        public double neo_omgcof;
        public double neo_sinmao;
        public double neo_t;
        public double neo_t2cof;
        public double neo_t3cof;
        public double neo_t4cof;
        public double neo_t5cof;
        public double neo_x1mth2;
        public double neo_x7thm1;
        public double neo_mdot;
        public double neo_omegadot;
        public double neo_xlcof;
        public double neo_xmcof;
        public double neo_omegacf;

        public NearEarthObjects()
        {
            neo_isimp = 0;
            neo_method = 0;
            neo_aycof = 0.0;
            neo_con41 = 0.0;
            neo_cc1 = 0.0;
            neo_cc4 = 0.0;
            neo_cc5 = 0.0;
            neo_d2 = 0.0;
            neo_d3 = 0.0;
            neo_d4 = 0.0;
            neo_delmo = 0.0;
            neo_eta = 0.0;
            neo_argpdot = 0.0;
            neo_omgcof = 0.0;
            neo_sinmao = 0.0;
            neo_t = 0.0;
            neo_t2cof = 0.0;
            neo_t3cof = 0.0;
            neo_t4cof = 0.0;
            neo_t5cof = 0.0;
            neo_x1mth2 = 0.0;
            neo_x7thm1 = 0.0;
            neo_mdot = 0.0;
            neo_omegadot = 0.0;
            neo_xlcof = 0.0;
            neo_xmcof = 0.0;
            neo_omegacf = 0.0;
        }

    }
}
