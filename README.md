# one_Sgp4

## Introduction

This library calculates the orbits of satellites via TLEs data from the web. Other functions for coordination transformation, time calculations and the calculation of contact windows depending on the observers coordinates on earth are also available. 

This library was created since I could not find a c# library with the necessary functions that I required. The calculations for the orbit propagations are based on the [SpaceTrack Report 3](https://celestrak.com/NORAD/documentation/spacetrk.pdf). Other Calculations for Time and coordination transformations were taken from [Dr. T.S. Kelso website] (http://celestrak.com/columns/).

## Download
Download the latest dll file [here](https://github.com/1manprojects/one_Sgp4/releases) and import it into youre project.

## Usage
After Importing the DLL into your project create a TLE element

Example TLE for ISS:
```
0 UME 1 (ISS 1)
1  8709U 76019A   17083.91463156 +.00000030 +00000-0 +91115-4 0  9995
2  8709 069.6748 319.8382 0011306 339.5223 102.9699 13.71383337054833
```
This is can be done using the parser or creating a new TLE element Manually
The Parser can be given the TLE lines as a string
```
Tle tleIem = ParserTLE.parseTle("0 UME 1 (ISS 1)",
  "1  8709U 76019A   17083.91463156 +.00000030 +00000-0 +91115-4 0  9995",
  "2  8709 069.6748 319.8382 0011306 339.5223 102.9699 13.71383337054833")
```
or one can give a txt-file containing a large list of TLE-Data
```
List<Tle> tleList = ParserTLE.ParseFile("PATH_TO_TLE.txt");
```

To calculate the Orbit positiont of the object in question the start and stop times need to be defined. These Epoch Times should be given in UTC-Time. 
```
EpochTime startTime = new EpochTime(DateTime.UtcNow());
EpochTime stopTime = new EpochTime(2017,100.5); (Year 2017, 100 day at 12:00 HH)
```
The calculation are then done using the SGP4 class with the TLE-data and the WGS-Constant (1 = WGS84; 0 = WGS72) with WGS84 being standard. The propagator will calculate from the starting time to the defined end time. Each step of the calculation can be defined from up to minutes or even seconds and lower. The results can be then retrived containing the Positon (X,Y,Z) and its Velocity (X_dot, Y_dot, Z_dot)
```
One_Sgp4.Sgp4 sgp4Propagator = new Sgp4(tleItem, Int<WGSCONSTANT>);
sgp4Propagator.runSgp4Cal(startTime, stopTime, Double<calculation step in minutes>);
List<One_Sgp4.Sgp4Data> resultDataList = new List<Sgp4Data>();
resultDataList = sgp4Propagator.getRestults();
```


**License:** [GNU LESSER GENERAL PUBLIC LICENSE Version 3](https://github.com/1manprojects/one_Sgp4/blob/master/LICENSE)
