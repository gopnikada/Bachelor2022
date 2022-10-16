
using Newtonsoft.Json;
using RoutingApp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RoutingApp
{
    public class MainWork
    {
        internal static List<Line> TrassenSegments = new List<Line>();
        internal static List<Point> PointsRouteDistOverflow = new List<Point>();

        public static string RouteMultiplePoints(List<Point> pointsToClaster)
        {
            var (globalTree, linePointsDict) = Edge.RouteOptimalWayForMultiplePoints(pointsToClaster, Config.InitRadius, 1);
            string json = string.Empty;
            if (globalTree is null)
            {
                json = JsonConvert.SerializeObject(Config.RadiusOverflowMsg);
            }
            else
            {
                Visualise.WriteToKmlWithTrassenAsPoints(globalTree, pointsToClaster, linePointsDict, TrassenSegments);
                Visualise.WriteToKmlOnlyLineStrings(globalTree);

                #region json
                var fCols = new List<GeoJSON.Net.Feature.Feature>();
                foreach (var edge in globalTree)
                {
                    List<GeoJSON.Net.Geometry.Position> positions = new List<GeoJSON.Net.Geometry.Position>();
                    GeoJSON.Net.Geometry.LineString linestring;


                    for (int i = 0; i < edge.RoutedPoints.Count; i++)
                    {

                        var pointToJson = edge.RoutedPoints[i];
                        GeoJSON.Net.Geometry.Position pointToJsonPos = new GeoJSON.Net.Geometry.Position(pointToJson.Latitude, pointToJson.Longtitude);
                        positions.Add(pointToJsonPos);
                    }
                    linestring = new GeoJSON.Net.Geometry.LineString(positions);
                    GeoJSON.Net.Feature.Feature feature = new GeoJSON.Net.Feature.Feature(linestring);
                    feature.Properties.Add("NewPoint", edge.Point1.ToJson());
                    feature.Properties.Add("OldPoint", edge.Point2.ToJson());
                    feature.Properties.Add("Length", edge.Weight);
                    feature.Properties.Add("status", $"OK");

                    fCols.Add(feature);
                }
                List<GeoJSON.Net.Geometry.Position> positionsOver = new List<GeoJSON.Net.Geometry.Position>();
                foreach (var pOver in PointsRouteDistOverflow)
                {
                    GeoJSON.Net.Geometry.Position positionOver = new GeoJSON.Net.Geometry.Position(pOver.Latitude, pOver.Longtitude);
                    positionsOver.Add(positionOver);
                    GeoJSON.Net.Geometry.Point geoPoint = new GeoJSON.Net.Geometry.Point(positionOver);
                    GeoJSON.Net.Feature.Feature featureOver = new GeoJSON.Net.Feature.Feature(geoPoint);
                    featureOver.Properties.Add("status", $"Trassen not found in {Config.MAXRADIUS}");
                    fCols.Add(featureOver);
                }

                GeoJSON.Net.Feature.FeatureCollection featureCollection =
                    new GeoJSON.Net.Feature.FeatureCollection(fCols);



                #endregion

                
                json = JsonConvert.SerializeObject(featureCollection);
            }
            return json;
        }
        public static string RouteOnePoint(Point point)
        {
            return RouteMultiplePoints(new List<RoutingApp.Point>() { point });
        }

    }
}
