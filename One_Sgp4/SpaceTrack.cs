﻿/*
 * Copyright 2017 Nikolai Reed <reed@1manprojects.de>
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

using System;
using System.Net;
using System.Collections.Specialized;

namespace One_Sgp4
{
    public class SpaceTrack
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