# one_Sgp4

[![Actions Status](https://github.com/1manprojects/one_Sgp4/workflows/.NET%20Core/badge.svg)](https://github.com/1manprojects/one_Sgp4/actions)

## Introduction

This library calculates the orbits of satellites via TLEs data from the web. Other functions for coordination transformation, time calculations and the calculation of contact windows depending on the observers coordinates on earth are also available. 

This library was created since I could not find a c# library with the necessary functions that I required. The calculations for the orbit propagations are based on the [SpaceTrack Report 3](https://celestrak.com/NORAD/documentation/spacetrk.pdf). Other Calculations for Time and coordination transformations were taken from [Dr. T.S. Kelso website] (http://celestrak.com/columns/).



This library calculates the orbits of satellites via TLEs data. Other functions for coordination transformation, time calculations, and the calculation of contact windows depending on the observer's coordinates on earth are also available.

This library was created since I could not find a c# library with the necessary functions that I required. The calculations for the orbit propagations are based on the [SpaceTrack Report 3](https://celestrak.com/NORAD/documentation/spacetrk.pdf). Other Calculations for Time and coordination transformations were taken from [Dr. T.S. Kelso website](http://celestrak.com/columns/).

### Limitations:

Currently, only the  Simplified General Perturbations (SGP) model is implemented which is optimized for objects in a Low Earth Orbit (LEO) [1](https://celestrak.com/NORAD/documentation/spacetrk.pdf). For Objects in a higher orbit, the Simplified Deep Space Perturbations (SDP) model should be used. 




### Accuracy:

The SGP4 implementation is accurate in the KM range meaning the calculated position of an object can be off by around 1 km [2](https://celestrak.com/NORAD/documentation/spacetrk.pdf). This error will grow larger for each day calculated. Thus it is recommended to update the TLEs used daily.

### Future Work:

In the future, I would like to implement the SDP4 model as well as the improved SGP8 and SDP8 models.


## Installing via NuGet
The easiest way to install One_Sgp4 is via NuGet.

In Visual Studio's Package Manager Console, enter the following command:
```
Install-Package One_Sgp4
```

### Manually
Download the latest dll file [here](https://github.com/1manprojects/one_Sgp4/releases) and import it into youre project.

## Usage
A more complete Example can be found in the [OneSGP4_Example Project](https://github.com/1manprojects/one_Sgp4/blob/master/OneSGP4_Example/Program.cs).


Example TLE for ISS:
```
0 UME 1 (ISS 1)
1  8709U 76019A   17083.91463156 +.00000030 +00000-0 +91115-4 0  9995
2  8709 069.6748 319.8382 0011306 339.5223 102.9699 13.71383337054833
```
This is can be done using the parser or creating a new TLE element Manually
The Parser can be given the TLE lines as a string
```
Tle tleItem = ParserTLE.parseTle(
                "1  8709U 76019A   17083.91463156 +.00000030 +00000-0 +91115-4 0  9995",
                "2  8709 069.6748 319.8382 0011306 339.5223 102.9699 13.71383337054833",
                "ISS 1");
```
or one can give a txt-file containing a large list of TLE-Data
```
List<Tle> tleList = ParserTLE.ParseFile("PATH_TO_TLE.txt");
```

To calculate the Orbit positiont of the object in question the start and stop times need to be defined. These Epoch Times should be given in UTC-Time. 
```
EpochTime startTime = new EpochTime(DateTime.UtcNow());
EpochTime stopTime = new EpochTime(2017,100.5); (Year 2017, 100 day at 12:00 HH)
//Add 15 Seconds to EpochTime
anotherTime.addTick(15);
//Add 20 Min to EpochTime
anotherTime.addMinutes(20);
//Add 1 hour to EpochTime
anotherTime.addHours(1);
//Add 2 Days to EpochTime
anotherTime.addDays(2);
```
The calculation are then done using the SGP4 class with the TLE-data and the WGS-Constant (1 = WGS84; 0 = WGS72) with WGS84 being standard. The propagator will calculate from the starting time to the defined end time. Each step of the calculation can be defined from up to minutes or even seconds and lower. The results can be then retrived containing the Positon (X,Y,Z) and its Velocity (X_dot, Y_dot, Z_dot)
```
One_Sgp4.Sgp4 sgp4Propagator = new Sgp4(tleItem, Int<WGSCONSTANT>);
sgp4Propagator.runSgp4Cal(startTime, stopTime, Double<calculation step in minutes>);
List<One_Sgp4.Sgp4Data> resultDataList = new List<Sgp4Data>();
resultDataList = sgp4Propagator.getRestults();
```
It is also possible to calculate the position of the Satellite at a single timepoint
```
Sgp4Data satellitePos = getSatPositionAtTime(satellite, epoch, wgs);
```

### Other Functions

Furthermore it is possible to calculate if a satellite will be visible from the ground at a certain location and time. For this one has to set a location of the observer on earth.

Coordinate of an observer on Ground latitude, longitude, height(in meters)
```
  One_Sgp4.Coordinate observer = new Coordinate(35.554595, 18.888574, 0);
```
Convert to ECI (Earth Centerd Inertial) coordinate system
```
  One_Sgp4.Point3d eci = observer.toECI(startTime.getLocalSiderealTime());
```
Get Local SiderealTime for Observer
```
  double localSiderealTime = startTime.getLocalSiderealTime(observer.getLongitude());
```

To Calculate the SubPoint of Satellite on ground
```
One_Sgp4.Coordinate satOnGround = One_Sgp4.SatFunctions.calcSatSubPoint(startTime, resultDataList[0], Sgp4.wgsConstant.WGS_84);
```

Check if a satellite is currently visible
```
bool satelliteIsVisible = One_Sgp4.SatFunctions.isSatVisible(observer, 0.0, startTime, satellitePos);
```
Calculate the Spherical Coordinates from an Observer to a Satelite. This will return a 3D-Point that contains the following information range from the point to the satellite in km, the Azimuth in radians and the Elevation in radians.
```
One_Sgp4.Point3d spherical = One_Sgp4.SatFunctions.calcSphericalCoordinate(observer, startTime, resultDataList[0]);
```
To Calculate the upcomming passes of the satellite for the next 5 days
```
List<Pass> passes = One_Sgp4.SatFunctions.CalculatePasses(observer, tleISS, new EpochTime(DateTime.UtcNow), 15, 5, Sgp4.wgsConstant.WGS_84);
```

**License:** MIT
