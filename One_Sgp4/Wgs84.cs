/*
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
