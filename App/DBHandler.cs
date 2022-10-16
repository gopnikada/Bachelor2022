using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingApp
{
    internal static class DBHandler
    {
       
             

        public static List<DBLineString> ReadLinesWithPointsDb(Point targetPoint, int radius)
        {
            string query2 = $"use {Config.DBName} SELECT [ogr_fid],[geog].STAsText(),[fid], " +
                $"[geog].STDistance(geography::STGeomFromText(Concat('POINT(', {targetPoint.Longtitude.ToString().Replace(',', '.')}, ' ', {targetPoint.Latitude.ToString().Replace(',', '.')}, ')'), 4326)) as dist " +
                $"FROM[HL_ProduktGeodatenImport].[dbo].[Trassen_PYUR_BackUp] " +
                $"WHERE geog.STDistance(geography::STGeomFromText(Concat('POINT(', {targetPoint.Longtitude.ToString().Replace(',', '.')}, ' ', {targetPoint.Latitude.ToString().Replace(',', '.')}, ')'), 4326)) <= {radius} " +
                $"ORDER BY dist";
            //string query = $"use {DBName} select * from dbo.fnGetNearestPoints3('{targetPoint.Latitude.ToString().Replace(',', '.')}'," +
            //   $" '{targetPoint.Longtitude.ToString().Replace(',', '.')}', {radius}) OPTION(MAXRECURSION 10000)";
            var linesList = new List<DBLineString>() { };

            using (SqlConnection connection = new SqlConnection(Config.DBConnString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query2, connection);

                command.CommandTimeout = 90000;
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int ogr_fid = int.Parse(reader.GetValue(0).ToString());
                        string pointsString = reader.GetValue(1).ToString();

                        List<PointFromDB> pointsFromLineString = LineStringToPoints(pointsString);

                        var lineDb = new DBLineString(ogr_fid, pointsFromLineString);
                        linesList.Add(lineDb);
                    }
                }

                reader.Close();
            }


            return linesList;
        }
               
        public static List<PointFromDB> LineStringToPoints(string pointsString)
        {
            var pointsList = new List<PointFromDB>() { };


            var pointsArr = pointsString.Remove(pointsString.Length - 1, 1).Substring(12).Split(new string[] { ", " }, StringSplitOptions.None);
            foreach (var point in pointsArr
                //.Where((r,i) => i>0)
                )
            {
                var latLongArr = point.Split(' ');

                double longtitude = double.Parse(latLongArr[0].Replace(',', '.'), CultureInfo.InvariantCulture);
                double latitude = double.Parse(latLongArr[1].Replace(',', '.'), CultureInfo.InvariantCulture);
                pointsList.Add(new PointFromDB(longtitude, latitude));
            }
            return pointsList;
        }
       


    }
}
