using RoutingApp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RoutingApp
{
    public class Point
    {
        public double Latitude { get; set; }
        public double Longtitude { get; set; }

        public Point(double longtitude, double latitude)
        {
            Latitude = latitude;
            Longtitude = longtitude;
        }
        public Point()
        {

        }

        internal PointFromDB ToPointDb()
        {
            return new PointFromDB(Longtitude, Latitude);
        }
        public override string ToString()
        {
            return Latitude.ToString().Replace(',', '.') + " " + Longtitude.ToString().Replace(',', '.');
        }
        public string ToJson()
        {
            return Longtitude.ToString().Replace(',', '.') + " " + Latitude.ToString().Replace(',', '.');
        }

        internal static Dictionary<Point, Line> Clusterize(List<Point> pointsToClaster, int _radius, double minLength)
        {

            Dictionary<Point, Line> clusteredPoints = new Dictionary<Point, Line>();
            List<PointFromDB> pointsDb = new List<PointFromDB>() { };
            List<PointFromDB> pointsDb2 = new List<PointFromDB>() { };
            List<DBLineString> lines = new List<DBLineString>();
            List<DBLineString> linesForTrassenShow = new List<DBLineString>();
            List<Line> linesForShow = new List<Line>();

            bool keepLooping = true;
            //prod
            foreach (var point in pointsToClaster)
            {
                int factor = 1;
                List<DBLineString> nearestLineStringsromDb = new List<DBLineString>();
                while (true)
                {
                    nearestLineStringsromDb = DBHandler.ReadLinesWithPointsDb(point, _radius);

                    if (nearestLineStringsromDb.Count == 0)
                    {
                        _radius += 100*factor;
                        factor++;
                        Config.ENDRADIUS = _radius;
                        if (_radius > Config.MAXRADIUS)
                        {
                            MainWork.PointsRouteDistOverflow.Add(point);
                            keepLooping = false;
                            _radius = Config.InitRadius;
                            break;
                            
                        }
                    }
                    else
                    {
                        keepLooping = true;
                        break;
                    }
                }
                if (keepLooping)
                {
                    var linesList = new List<Line>();
                    foreach (var lineString in nearestLineStringsromDb)
                    {
                        linesList.AddRange(lineString.LinesFronPoints);
                    }

                    var nearestDBLine = linesList.OrderBy(x => x.FindDistanceToPoint(point).Item1).FirstOrDefault();




                    var distToNearestDbLine = nearestDBLine.FindDistanceToPoint(point).Item1;

                    if (clusteredPoints.Count > 0)
                    {
                        var alrPoint = clusteredPoints.Keys.ToList().OrderBy(x => ApiRequest.RouterFromToGetLines(x, point).Item1).FirstOrDefault();
                        var distToMinAlrPoint = ApiRequest.RouterFromToGetLines(alrPoint, point).Item1;
                        if (distToNearestDbLine > distToMinAlrPoint)
                        {
                            nearestDBLine = new Line(alrPoint, new Point(alrPoint.Longtitude + 0.00001, alrPoint.Latitude + 0.00001));
                        }
                    }

                    clusteredPoints.Add(point, nearestDBLine);

                    MainWork.TrassenSegments.AddRange(linesList);
                }
                else
                {
                    continue;
                }
            }
           

            return clusteredPoints;
        }
        internal static List<Point> HalbierenOneLine(Line _line)
        {
            var toReturn = new List<Point>();

            Point point1 = _line.StartPoint; ;
            Point point2 = _line.EndPoint;
            toReturn.Add(point1);
            toReturn.Add(point2);

            var line = new Line(point1, point2);

            double xdiff = Math.Abs(line.StartPoint.Longtitude - line.EndPoint.Longtitude) / 2;
            double ydiff = Math.Abs(line.StartPoint.Latitude - line.EndPoint.Latitude) / 2;

            //PointFromDB currentPointToAddTo = line.StartPoint.ToPointDb();
            PointFromDB newPoint;
            double lat = 0;
            if (point1.Latitude < point2.Latitude)
            {
                lat = point1.Latitude + ydiff;
            }
            else
            {
                lat = point2.Latitude + ydiff;
            }

            if (point1.Longtitude < point2.Longtitude)
            {
                newPoint = new PointFromDB(point1.Longtitude + xdiff, lat);
            }
            else
            {
                newPoint = new PointFromDB(point2.Longtitude + xdiff, lat);
            }

            toReturn.Add(new Point(newPoint.Longtitude, newPoint.Latitude));

            return toReturn;
        }

      
        public override bool Equals(object obj)
        {
            if ((obj as Point).Latitude == Latitude && (obj as Point).Longtitude == Longtitude)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}