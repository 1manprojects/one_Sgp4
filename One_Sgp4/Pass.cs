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
        private Coordinate location { get; }
        private EpochTime startOfContact { get; }
        private EpochTime endOfContact { get; }
        private double maxElevation { get; }

        public Pass(Coordinate Observer, EpochTime ContactStart, EpochTime ContactEnd, double maxElevation)
        {
            this.location = Observer;
            this.startOfContact = ContactStart;
            this.endOfContact = ContactEnd;
            this.maxElevation = maxElevation;
        }

        public override string ToString()
        {
            return string.Format("Start Of Contact: {0}, End of Contact: {1}, Max Elevation: {2}", startOfContact.ToString(), endOfContact.ToString(), maxElevation);
        }
    }
}
