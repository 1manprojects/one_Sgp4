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
using System.Security;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace One_Sgp4
{
    class WebTleLoader
    {
        //***************************************************************************
        /*
         * Api based on https://www.space-track.org/documentation#/howto
         * by Mr. R. Bovard
         * 
         * modified by Nikolai Reed 2015
        */
        public class WebClientEx : WebClient
        {
            // Create the container to hold all Cookie objects
            private CookieContainer _cookieContainer = new CookieContainer();
            // Override the WebRequest method so we can store the cookie 
            // container as an attribute of the Web Request object
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);

                if (request is HttpWebRequest)
                    (request as HttpWebRequest).CookieContainer = _cookieContainer;
                return request;
            }
        }   // END WebClient Class

        public static string GetSpaceTrack(string[] noradId, string username, string password)
        {
            string uriBase = "https://www.space-track.org";
            string requestController = "/basicspacedata";
            string requestAction = "/query";

            string predicateValues = "/class/tle_latest/ORDINAL/1/NORAD_CAT_ID/" +
                string.Join(",", noradId) + "/orderby/NORAD_CAT_ID/format/3le";
            string request = uriBase + requestController + requestAction + predicateValues;

            // Create new WebClient object to communicate with the service
            using (var client = new WebClientEx())
            {
                // Store the user authentication information
                var data = new NameValueCollection
                {
                    { "identity", username },
                    { "password", password },
                };

                // Generate the URL for the API Query and return the response
                var response2 = client.UploadValues(uriBase + "/ajaxauth/login", data);
                var response4 = client.DownloadData(request);
                return (System.Text.Encoding.Default.GetString(response4));
            }
        }
        //**************************************************************************

    }
}