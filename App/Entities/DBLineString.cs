using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingApp
{
    internal class DBLineString
    {
        public List<PointFromDB> PointsFromDB { get; set; }
        public int Ogr_fid { get; }
        public List<Line> LinesFronPoints = new List<Line>();

        public DBLineString()
        {
            PointsFromDB = new List<PointFromDB>() { };
        }

        public DBLineString(int ogr_fid, List<PointFromDB> pointsFromLineString)
        {
            Ogr_fid = ogr_fid;
            PointsFromDB = pointsFromLineString;

            for (int i = 0; i < PointsFromDB.Count-1; i++)
            {
                Point point1 = PointsFromDB[i].ToPoint();
                Point point2 = PointsFromDB[i + 1].ToPoint();

                var lineToaAdd = new Line(point1, point2);

                LinesFronPoints.Add(lineToaAdd);
            }

        }
        public static List<DBLineString> Halbieren(List<DBLineString> _dbLines)
        {
            List<DBLineString> resultLines = new List<DBLineString>();
            var allPointsFromLines = new List<PointFromDB>() { };
            //foreach (var dbLine in _dbLines)
            //{
            //    allPointsFromLines.AddRange(dbLine.PointsFromDB);
            //}
            int count = 0;
            //var newLines = _dbLines;
            List<DBLineString> newLines = new List<DBLineString>();
            foreach (var item in _dbLines)
            {
                List<PointFromDB> lst = new List<PointFromDB>();
                foreach (var item2 in item.PointsFromDB)
                {
                    var listEnt = new PointFromDB(item2.Longtitude, item2.Latitude);
                    lst.Add(listEnt);
                }
                newLines.Add(new DBLineString(item.Ogr_fid, lst));
            }

            foreach (var dbline in _dbLines)
            {
                int countPoints = 0;
                for (int i = 0; i < dbline.PointsFromDB.Count - 1; i++)
                {
                    Point point1 = dbline.PointsFromDB[i].ToPoint();
                    Point point2 = dbline.PointsFromDB[i + 1].ToPoint();

                    var line = new Line(point1, point2);

                    if (line.Length < 10) break;

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


                    allPointsFromLines.Add(newPoint);

                    var genPoints = new List<PointFromDB>();

                    var first = new PointFromDB(line.StartPoint.Longtitude, line.StartPoint.Latitude);
                    var sec = new PointFromDB(newPoint.Longtitude, newPoint.Latitude);
                    var thi = new PointFromDB(line.EndPoint.Longtitude, line.EndPoint.Latitude);

                    genPoints.Add(first);
                    genPoints.Add(sec);
                    genPoints.Add(thi);

                    var point1TODel = first;
                    var point2ToDel = thi;

                    foreach (var item in newLines)
                    {
                        PointFromDB pointToCh1 = item.PointsFromDB.Where(x => x.Latitude == point1TODel.Latitude && x.Longtitude == point1TODel.Longtitude).FirstOrDefault();
                        PointFromDB pointToCh2 = item.PointsFromDB.Where(x => x.Latitude == point2ToDel.Latitude && x.Longtitude == point2ToDel.Longtitude).FirstOrDefault();

                        if (item.PointsFromDB.Contains(pointToCh1) && item.PointsFromDB.Contains(pointToCh2))
                        {
                            item.PointsFromDB.Remove(pointToCh1);
                            item.PointsFromDB.Remove(pointToCh2);

                            item.PointsFromDB.Add(first);
                            item.PointsFromDB.Add(sec);
                            item.PointsFromDB.Add(thi);

                            //newLines.Add(new DBLine(0, genPoints));

                            //item.PointsFromDB.Remove()
                            break;
                        }

                    }
                    countPoints++;
                    //Console.WriteLine($"{countPoints} / {dbline.PointsFromDB.Count}");
                }
                count++;
                Console.WriteLine($"{count} / {_dbLines.Count}");
            }

            return newLines;
        }

        public static List<List<Line>> GroupingLines(List<DBLineString> lines, double minLength)
        {
            var globalLinesGetrenntList = new List<List<Line>>();
            foreach (var lineWithPointsFromDb in lines)
            {
                var separatedLinesThatAreBreaked = new List<Line>();
                List<Line> aglo = new List<Line>();
                for (int i = 0; i < lineWithPointsFromDb.PointsFromDB.Count - 1; i++)
                {
                    var breakedLine = new Line(new Point(lineWithPointsFromDb.PointsFromDB[i].Longtitude, lineWithPointsFromDb.PointsFromDB[i].Latitude),
                           new Point(lineWithPointsFromDb.PointsFromDB[i + 1].Longtitude, lineWithPointsFromDb.PointsFromDB[i + 1].Latitude));

                    if (breakedLine.Length < minLength)
                    {
                        aglo.Add(breakedLine);
                    }
                    else
                    {
                        if (separatedLinesThatAreBreaked.Count == 0 && aglo.Count > 0)
                        {
                            separatedLinesThatAreBreaked.Add(aglo[0]);
                        }
                        if (aglo.Count > 1)
                        {
                            var firstLineInAglo = aglo.FirstOrDefault().StartPoint;
                            var lastLineInAglo = aglo.LastOrDefault().EndPoint;

                            aglo.Clear();

                            var agloLuftLine = new Line(firstLineInAglo, lastLineInAglo);

                            var middlePoint = new Point(firstLineInAglo.Longtitude + agloLuftLine.XDiff / 2, firstLineInAglo.Latitude + agloLuftLine.YDiff / 2);

                            var lastLongPoint = separatedLinesThatAreBreaked.LastOrDefault().EndPoint;

                            var replacementLine1 = new Line(lastLongPoint, middlePoint);
                            var replacementLine2 = new Line(middlePoint, breakedLine.StartPoint);
                            separatedLinesThatAreBreaked.Add(replacementLine1);
                            separatedLinesThatAreBreaked.Add(replacementLine2);
                        }
                        else
                        {
                            separatedLinesThatAreBreaked.Add(breakedLine);
                        }

                    }

                }

                globalLinesGetrenntList.Add(separatedLinesThatAreBreaked);
            }
            return globalLinesGetrenntList;
        }

    }
}
