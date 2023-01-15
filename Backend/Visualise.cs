using RoutingApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingApp
{
    public static class Visualise
    {


        internal static void WriteToKmlWithTrassenAsPoints(List<Edge> _tree, List<Point> _points,
            Dictionary<Line, List<Point>> linePointsDict, List<Line> _TrassenSegments)
        {
            double dist = 0;
            List<double> distArr = new List<double>();
            _tree.ForEach(x => x.RoutedLines.ForEach(y => distArr.Add(y.Length)));
            var LinesList = new List<Line>();

            distArr = distArr.Distinct().OrderBy(x => x).ToList();

            distArr.ForEach(x => dist += x);

            var listLines = new List<Line>();
            foreach (var item in _tree)
            {
                listLines.AddRange(item.RoutedLines);
            }



            var linesToWriteBeg = new List<string> {
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
                "<kml xmlns=\"http://www.opengis.net/kml/2.2\">",
                   "<Document>",
            "<name>LineString.kml</name>",
            $"<description> {(int)dist} Meters IN RADIUS {Config.ENDRADIUS} meters</description>",
            "<open>1</open>",
            "<LookAt>",
            $"<longitude>{listLines.LastOrDefault().StartPoint.Longtitude.ToString().Replace(',', '.')}</longitude>",
            $"<latitude>{listLines.LastOrDefault().StartPoint.Latitude.ToString().Replace(',', '.')}</latitude>",
            "<altitude>0</altitude>",
            "<range>150</range>",
            "<tilt>50</tilt>",
            "<heading>0</heading>",
            "</LookAt>"
            };
            //var mergedArr = new string[] { };
            foreach (var line in listLines)
            {
                var middle = new List<string>
                 {
                "<Placemark>",
            "<name>TrassenSegment</name>",
            "<LineString>",
            "<extrude>1</extrude>",
            "<tessellate>1</tessellate>",
            "<coordinates>",
            $"{line.StartPoint.Longtitude.ToString().Replace(',', '.')},{line.StartPoint.Latitude.ToString().Replace(',', '.')},0 {line.EndPoint.Longtitude.ToString().Replace(',', '.')},{line.EndPoint.Latitude.ToString().Replace(',', '.')},0",
            "</coordinates>",
            "</LineString>",
            "<Style>",
            "<LineStyle>",
            "<color>#ff0000ff</color>",
            "<width>4</width>",
            "</LineStyle>",
            "</Style>",
            "</Placemark>",
                    };
                linesToWriteBeg.AddRange(middle);
            }

            foreach (var _point in _points)
            {

                var middlePoints = new List<string>
            {
                 "<Placemark>",
            "<name> </name>",
            "<Style>",
            "<Icon>",
            "<href>",
            "http://maps.google.com/mapfiles/kml/pushpin/ylw-pushpin.png",
            "</href>",
            "</Icon>",
            "</Style>",
            "<Point>",
            "<coordinates>",
            $"{_point.Longtitude.ToString().Replace(',', '.')}, {_point.Latitude.ToString().Replace(',', '.')} 0",
            "</coordinates>",
            "</Point>",
            "</Placemark>",
            };
                linesToWriteBeg.AddRange(middlePoints);
            }
            int counter = 0;
            foreach (var trasse in linePointsDict.Keys)
            {
                counter++;
                var middle = new List<string>
                 {
              "<Placemark>",
            "<name>TrassenSegment</name>",
            "<description>This is the</description>",
            "<LineString>",
            "<extrude>1</extrude>",
            "<tessellate>1</tessellate>",
            "<coordinates>",
            $"{trasse.StartPoint.Longtitude.ToString().Replace(',', '.')},{trasse.StartPoint.Latitude.ToString().Replace(',', '.')},0 {trasse.EndPoint.Longtitude.ToString().Replace(',', '.')},{trasse.EndPoint.Latitude.ToString().Replace(',', '.')},0",
            "</coordinates>",
            "</LineString>",
            "<Style>",
            "<LineStyle>",
            "<color>ff1ce2d9</color>",
            "<width>10</width>",
            "</LineStyle>",
            "</Style>",
            "</Placemark>",
                    };
                linesToWriteBeg.AddRange(middle);
            }


            foreach (var line in _TrassenSegments)
            {

                var TrassSegm = new List<string>
            {
            "<Placemark>",
            "<name>TrassenSegment</name>",
            "<LineString>",
            "<extrude>1</extrude>",
            "<tessellate>1</tessellate>",
            "<coordinates>",
            $"{line.StartPoint.Longtitude.ToString().Replace(',', '.')},{line.StartPoint.Latitude.ToString().Replace(',', '.')},0 {line.EndPoint.Longtitude.ToString().Replace(',', '.')},{line.EndPoint.Latitude.ToString().Replace(',', '.')},0",
            "</coordinates>",
            "</LineString>",
            "<Style>",
            "<LineStyle>",
            "<color>ff1ce2d9</color>",
            "<width>10</width>",
            "</LineStyle>",
            "</Style>",
            "</Placemark>",
            };
                linesToWriteBeg.AddRange(TrassSegm);
            }

            var end = new List<string>
            {
                "</Document>",
            "</kml>"
            };



            linesToWriteBeg.AddRange(end);

            string path = Config.PATH_TO_SAVE_KML_TRASSEN;
            try
            {
                File.WriteAllLines(path, linesToWriteBeg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        internal static void WriteToKmlOnlyLineStrings(List<Edge> globalTree)
        {
            var linesToWriteBeg = new List<string> {
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
                "<kml xmlns=\"http://www.opengis.net/kml/2.2\">",
                   "<Document>",
            "<name>LineString.kml</name>",
            $"<description>  Meters IN RADIUS {Config.ENDRADIUS} meters</description>",
            "<open>1</open>",
            "<LookAt>",
            $"<longitude>{globalTree[0].Point1.Longtitude.ToString().Replace(',', '.')}</longitude>",
            $"<latitude>{globalTree[0].Point1.Latitude.ToString().Replace(',', '.')}</latitude>",
            "<altitude>0</altitude>",
            "<range>150</range>",
            "<tilt>50</tilt>",
            "<heading>0</heading>",
            "</LookAt>" };

            foreach (var edge in globalTree)
            {
                string coordsStr = string.Empty;
                foreach (var edgePoint in edge.RoutedPoints)
                {
                    coordsStr += edgePoint.Longtitude.ToString().Replace(',', '.');
                    coordsStr += ",";
                    coordsStr += edgePoint.Latitude.ToString().Replace(',', '.');
                    coordsStr += ",";
                    coordsStr += "0";
                    coordsStr += " ";
                }
                coordsStr = coordsStr.Remove(coordsStr.Length - 1);
                var middle = new List<string>
                 {
                "<Placemark>",
            "<name>TrassenSegment</name>",
            "<LineString>",
            "<extrude>1</extrude>",
            "<tessellate>1</tessellate>",
            "<coordinates>",
            $"{coordsStr}",
            "</coordinates>",
            "</LineString>",
            "<Style>",
            "<LineStyle>",
            "<color>#ff0000ff</color>",
            "<width>4</width>",
            "</LineStyle>",
            "</Style>",
            "</Placemark>",
                    };
                linesToWriteBeg.AddRange(middle);
            }

            var last = new List<string>() {  "</Document>",
            "</kml>" };
            linesToWriteBeg.AddRange(last);
            string path = Config.PATH_TO_SAVE_KML_LINESTRINGS;
            try
            {
                File.WriteAllLines(path, linesToWriteBeg);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
