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
    class Sgp4Rec
    {

    public int rec_satnum;

	public int rec_epochyr;
	public int rec_init;
	public int rec_epochtynumrev;
	public int rec_error;
	public double rec_a;
	public double rec_altp;
	public double rec_alta;
	public double rec_epochdays;
	public double rec_mjdsatepoch;
    public double rec_nddot;
    public double rec_ndot;
	public double rec_bstar;
	public double rec_rcse;
	public double rec_inclo;
	public double rec_omegao;
	public double rec_ecco;
	public double rec_argpo;
	public double rec_mo;
	public double rec_no;
	public double rec_eptime;
	public double rec_srtime;
	public double rec_sptime;
	public double rec_deltamin;

	public double rec_ep;
	public double rec_xincp;
	public double rec_omegap;
	public double rec_argpp;
	public double rec_mp;

	public double[] rec_r;
	public double[] rec_v;

    public NearEarthObjects neo;
    public DeepSpaceObjects dso;


    public Sgp4Rec()
    {
        rec_satnum = -1;

        rec_epochyr = 0;
        rec_init = 0;
        rec_epochtynumrev = 0;
        rec_error = 0;
        rec_a = 0.0;
        rec_altp = 0.0;
        rec_alta = 0.0;
        rec_epochdays = 0.0;
        rec_mjdsatepoch = 0.0;
        rec_nddot = 0.0;
        rec_ndot = 0.0;
        rec_bstar = 0.0;
        rec_rcse = 0.0;
        rec_inclo = 0.0;
        rec_omegao = 0.0;
        rec_ecco = 0.0;
        rec_argpo = 0.0;
        rec_mo = 0.0;
        rec_no = 0.0;
        rec_eptime = 0.0;
        rec_srtime = 0.0;
        rec_sptime = 0.0;
        rec_deltamin = 0.0;

        rec_ep = 0.0;
        rec_xincp = 0.0;
        rec_omegap = 0.0;
        rec_argpp = 0.0;
        rec_mp = 0.0;

        rec_r = new double[3];
        rec_v = new double[3];
        rec_r[0] = rec_r[1] = rec_r[2] = 0.0;
        rec_v[0] = rec_v[1] = rec_v[2] = 0.0;

        neo = new NearEarthObjects();
        dso = new DeepSpaceObjects();
    }

    }
}
