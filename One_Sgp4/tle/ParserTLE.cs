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
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;

namespace One_Sgp4
{
    public class ParserTLE
    {
        /**
      * \brief ParseTle class
      *
      * This class handles the reading and converting of TLE information
      * eiter reading each single element from a string or by giving it a txt
      * file.
      */


        //! TleParser constructor.
        /*!
        */
        public ParserTLE()
        {

        }

        //! Reads TwoLineElement data and converts it to Tle
        /*!
        \param string Line 1
        \param string Line 2
        \param string Name = null
        * if name = null then Internatioanl Designater is taken as name
        * Example
        * NOAA 14                 
        * 1 23455U 94089A   15094.47912277  .00000079  00000-0  64323-4 0  9995
        * 2 23455  98.7542 177.4401 0008423 292.6752 195.2467 14.14031457 45115 
        \return Tle tle-Class
        */
        public static Tle parseTle(string tleLine1, string tleLine2,
            string tleName = null)
        {
            int satCl = 0;
            string noradId;
            int startYear = 0;
            int startNumber = 0;
            string intDes = "--";
            int epochYear;
            double epochDay;

            double firstMeanMotion;
            double secondMeanMotion;
            double dragTerm;
            double ephemeris;
            int setNumber = 0;
            int checksum1;
            int count = 0;

            //Start Line 1
            //check if data maches Checksumm
            bool valid1 = isValid(tleLine1);
            bool valid2 = isValid(tleLine2);
       
            if (!valid1 || !valid2)
            {
                throw new InvalidDataException($"The data contained checksum error(s) in {((valid1) ? "" : "line1")} {((valid2) ? "" : "line2")}. ");
            }

            Tle ret;
            try
            {
                try
                {
                    string[] s1 = tleLine1.Split(' ');
                    string[] line1 = new string[9];
                    
                    for (int i = 0; i < s1.Length; i++)
                    {
                        if (s1[i].Length > 0)
                        {
                            line1[count] = s1[i];
                            count++;
                        }
                    }

                    string sclass = line1[1].Substring(line1[1].Length - 1);
                    if (sclass == "U")
                    {
                        satCl = 0x0;
                    }
                    if (sclass == "C")
                    {
                        satCl = 0x1;
                    }
                    if (sclass == "S")
                    {
                        satCl = 0x2;
                    }

                    noradId = line1[1].Remove(line1[1].Length - 1);

                    //check if Line contains International Designator Information
                    //if Not then skip setting them
                    int noID = 0;
                    if (count == 8)
                    {
                        noID = -1;
                    }
                    else
                    {
                        startYear = Convert.ToInt32(line1[2].Substring(0, 2));
                        startNumber = Convert.ToInt32(line1[2].Substring(2, 3));
                        intDes = line1[2].Substring(5);
                    }

                    epochYear = Convert.ToInt32(line1[3 + noID].Substring(0, 2));
                    string epDay = line1[3 + noID].Substring(2);
                    epochDay = double.Parse(epDay, CultureInfo.GetCultureInfo("en-US"));

                    firstMeanMotion = double.Parse(line1[4 + noID], CultureInfo.GetCultureInfo("en-US"));

                    int zeros = Convert.ToInt32(line1[5 + noID].Substring(line1[5].Length - 1));
                    if (line1[5 + noID].Equals("00000+0"))
                    {
                        line1[5 + noID] = line1[5 + noID].Substring(0, line1[5 + noID].IndexOf('+'));
                    }
                    else
                    {
                        line1[5 + noID] = line1[5 + noID].Substring(0, line1[5 + noID].IndexOf('-'));
                    }
                    if (line1[5 + noID].Length > 0)
                    {
                        if (line1[5 + noID][0] == '+' || line1[5 + noID][0] == '-')
                        {
                            line1[5 + noID] = line1[5 + noID].Insert(1, ".");
                            for (int i = 0; i < zeros; i++)
                                line1[5 + noID] = line1[5 + noID].Insert(2, "0");
                        }
                        else
                        {
                            line1[5 + noID] = line1[5 + noID].Insert(0, ".");
                            for (int i = 0; i < zeros; i++)
                                line1[5 + noID] = line1[5 + noID].Insert(1, "0");
                        }
                        secondMeanMotion = double.Parse(line1[5 + noID], CultureInfo.GetCultureInfo("en-US"));
                    }
                    else
                    {
                        secondMeanMotion = 0.0;
                    }

                    zeros = Convert.ToInt32(line1[6 + noID].Substring(line1[6 + noID].Length - 1));
                    if (line1[6 + noID][line1[6 + noID].Length - 2] == '-')
                    {
                        line1[6 + noID] = line1[6 + noID].Substring(0, line1[6 + noID].IndexOf('-'));
                    }
                    else
                    {
                        line1[6 + noID] = line1[6 + noID].Substring(0, line1[6 + noID].IndexOf('+'));
                    }
                    if (line1[6 + noID].Length > 0)
                    {
                        if (line1[6 + noID][0] == '+' || line1[6 + noID][0] == '-')
                        {
                            line1[6 + noID] = line1[6 + noID].Insert(1, ".");
                            for (int i = 0; i < zeros; i++)
                                line1[6 + noID] = line1[6 + noID].Insert(2, "0");
                        }
                        else
                        {
                            line1[6 + noID] = line1[6 + noID].Insert(0, ".");
                            for (int i = 0; i < zeros; i++)
                                line1[6 + noID] = line1[6 + noID].Insert(1, "0");
                        }
                        dragTerm = double.Parse(line1[6 + noID], CultureInfo.GetCultureInfo("en-US"));
                    }
                    else
                    {
                        dragTerm = 0.0;
                    }

                    ephemeris = double.Parse(line1[7 + noID], CultureInfo.GetCultureInfo("en-US"));

                    //check if Element Setnumber is included in TLE line
                    //if not then there is only Checksum here
                    if (line1[8 + noID].Length > 1)
                    {
                        setNumber = Convert.ToInt32(line1[8 + noID].Substring(0, line1[8 + noID].Length - 1));
                        checksum1 = Convert.ToInt32(line1[8 + noID].Substring(line1[8 + noID].Length - 1));
                    }
                    else
                    {
                        checksum1 = Convert.ToInt32(line1[8 + noID]);
                    }
                } catch (Exception ex)
                {
                    throw new InvalidDataException("Could not parse Line 1.", ex);
                }

                int satNumber = 0;
                double inclination = 0;
                double rightAscension = 0;
                double eccentricity = 0;
                double perigee = 0;
                double meanAnomoly = 0;
                double meanMotion = 0;
                double relevationNumber = 0;
                int checksum2 = 0;

                //Start Line2 
                try
                {
                    string[] s2 = tleLine2.Split(' ');
                    string[] line2 = new string[9];
                    count = 0;
                    for (int i = 0; i < s2.Length; i++)
                    {
                        if (s2[i].Length > 0)
                        {
                            line2[count] = s2[i];
                            count++;
                        }
                    }

                    satNumber = Convert.ToInt32(line2[1]);
                    inclination = double.Parse(line2[2], CultureInfo.GetCultureInfo("en-US"));
                    rightAscension = double.Parse(line2[3], CultureInfo.GetCultureInfo("en-US"));
                    line2[4] = line2[4].Insert(0, ".");
                    eccentricity = double.Parse(line2[4], CultureInfo.GetCultureInfo("en-US"));
                    perigee = double.Parse(line2[5], CultureInfo.GetCultureInfo("en-US"));
                    meanAnomoly = double.Parse(line2[6], CultureInfo.GetCultureInfo("en-US"));
                    if (line2[8] != null)
                    {
                        meanMotion = double.Parse(line2[7], CultureInfo.GetCultureInfo("en-US"));
                        checksum2 = Convert.ToInt32(line2[8].Substring(line2[8].Length - 1));
                        relevationNumber = double.Parse(line2[8].Substring(0, line2[8].Length - 1),
                            CultureInfo.GetCultureInfo("en-US"));
                    }
                    else
                    {
                        checksum2 = Convert.ToInt32(line2[7].Substring(line2[7].Length - 1));
                        meanMotion = double.Parse(line2[7].Substring(0, 11),
                            CultureInfo.GetCultureInfo("en-US"));
                        relevationNumber = double.Parse(line2[7].Substring(11, 5),
                            CultureInfo.GetCultureInfo("en-US"));
                    }

                    if (tleName == null)
                    {
                        tleName = startYear.ToString() + startNumber.ToString() + intDes;
                    }
                    if (tleName[0] == '0' && tleName[1] == ' ')
                    {
                        tleName = tleName.Remove(0, 2);
                    }
                } catch (Exception ex)
                {
                    throw new InvalidDataException("Could not parse Line 2.", ex);
                }

                ret = new Tle(tleName, noradId, (Enum.satClass)satCl, startYear, startNumber, intDes,
                epochYear, epochDay, firstMeanMotion, secondMeanMotion, dragTerm,
                ephemeris, setNumber, checksum1, satNumber, inclination, rightAscension,
                eccentricity, perigee, meanAnomoly, meanMotion, relevationNumber, checksum2);

            }
            catch (Exception ex)
            {
                // whateverhappened this is not a valid TLE
                throw new InvalidDataException("Data contained parse error(s).", ex);
            }
            return ret;
        }

        //! Parse TLE Data from File 
        /*!
            \pram string filepath
            \return list<Tle> Two Line Element Data list
        */
        public static List<Tle> ParseFile(string filename, string satName = null)
        {
            List<Tle> results = new List<Tle>();
            string name = satName ?? Path.GetFileNameWithoutExtension(filename);

            using (var reader = new StreamReader(filename))
            {
                string line;
                string line1 = null;

                while ((line = reader.ReadLine()) != null)
                {
                    var lineNr = line.Split(' ')[0];
                    if (lineNr != "1" && lineNr != "2")
                    {
                        name = line;
                        line1 = null;
                    }
                    else
                    {
                        if (lineNr == "1")
                        {
                            line1 = line;
                        }
                        if (lineNr == "2" && (line1 != null))
                        {
                            results.Add(parseTle(line1, line, name));
                        }
                    }
                }
            }
            return results;
        }

        //! Validate TLE Data against checksumm
        /*!
            \pram string Tle line
            \return bool true if tle line matches up to checksum
            The summ of all numbers with minus seen as 1 and 0 for characters
            and whitespaces mod 10 musst match up with the checksumm 
        */
        public static bool isValid(string line1)
        {
            int sum1 = 0;
            for (int i = 0; i < line1.Count() -1; i++ )
            {
                if (char.IsNumber(line1[i]))
                {
                    sum1 = sum1 + (int)Char.GetNumericValue(line1[i]);
                }
                else
                {
                    if (line1[i] == '-')
                    {
                        sum1++;
                    }
                }
            }
            int result = sum1 % 10;
            int checksum = (int)Char.GetNumericValue(line1[line1.Count() - 1]);
            if (result == checksum)
                return true;
            else
                return false;
        }
    }
}
