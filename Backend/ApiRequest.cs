using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoutingApp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RoutingApp
{
    internal static class ApiRequest
    {
      
        public static (double, List<Line>) RouterFromToGetLines(Point from, Point to)
        {
            var url = $"http://{Config.HOST}:{Config.PORT}/route?" +
               $"point={from.Latitude.ToString().Replace(',', '.')},{from.Longtitude.ToString().Replace(',', '.')}" +
               $"&point={to.Latitude.ToString().Replace(',', '.')},{to.Longtitude.ToString().Replace(',', '.')}" +
               $"&profile=foot&instructions=false&points_encoded=false";

            string responseJson = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        var serializer = new JsonSerializer();

                        using (var sr = new StreamReader(stream))
                        using (var jsonTextReader = new JsonTextReader(sr))
                        {
                            responseJson = serializer.Deserialize(jsonTextReader).ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            var routedLines = new List<Line>();
            double routedDistance = 0;
            try
            {
                JObject jsonObj = JObject.Parse(responseJson);
                string distStr = jsonObj["paths"][0]["distance"].ToString().Replace(',', '.');
                routedDistance = double.Parse(distStr,
                    CultureInfo.InvariantCulture);


                var routedTurns = jsonObj["paths"][0]["points"]["coordinates"];

                var routedPoints = new List<Point>() { };

                foreach (var point in routedTurns)
                {
                    routedPoints.Add(new Point(double.Parse(point[0].ToString().Replace(',', '.'),
                    CultureInfo.InvariantCulture), double.Parse(point[1].ToString().Replace(',', '.'),
                    CultureInfo.InvariantCulture)));
                }
                for (int i = 1; i < routedPoints.Count; i++)
                {
                    routedLines.Add(new Line(routedPoints[i - 1], routedPoints[i]));
                }

            }
            catch (Exception e)
            {

            }

            return (routedDistance, routedLines);
        }

     

       public static (double, List<Point>) RouterFromToGetPoints(Point from, Point to)
        {
            var url = $"http://{Config.HOST}:{Config.PORT}/route?" +
               $"point={from.Latitude.ToString().Replace(',', '.')},{from.Longtitude.ToString().Replace(',', '.')}" +
               $"&point={to.Latitude.ToString().Replace(',', '.')},{to.Longtitude.ToString().Replace(',', '.')}" +
               $"&profile=foot&instructions=false&points_encoded=false";

            List<Point> routedPoints = new List<Point>() { };
            string responseJson = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        var serializer = new JsonSerializer();

                        using (var sr = new StreamReader(stream))
                        using (var jsonTextReader = new JsonTextReader(sr))
                        {
                            responseJson = serializer.Deserialize(jsonTextReader).ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }



            double routedDistance = 0;
            try
            {
                JObject jsonObj = JObject.Parse(responseJson);
                string distStr = jsonObj["paths"][0]["distance"].ToString().Replace(',', '.');
                routedDistance = double.Parse(distStr,
                    CultureInfo.InvariantCulture);


                var routedTurns = jsonObj["paths"][0]["points"]["coordinates"];



                foreach (var point in routedTurns)
                {
                    routedPoints.Add(new Point(double.Parse(point[0].ToString().Replace(',', '.'),
                    CultureInfo.InvariantCulture), double.Parse(point[1].ToString().Replace(',', '.'),
                    CultureInfo.InvariantCulture)));
                }


            }
            catch (Exception e)
            {

            }

            return (routedDistance, routedPoints);
        }
    }
}
