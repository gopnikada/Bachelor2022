using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingApp
{
    internal class Edge
    {
        public List<Point> RoutedPoints = new List<Point>();

        public int Vertex1 { get; set; }
        public int Vertex2 { get; set; }
        public double Weight { get; set; }
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }
        public List<Line> RoutedLines { get; set; }

        public static (List<Edge>, Dictionary<Line, List<Point>>) RouteOptimalWayForMultiplePoints(List<Point> pointsToClaster, int _radius, double minLength)
        {
            Dictionary<Point, Line> clustered = Point.Clusterize(pointsToClaster, _radius, minLength);

            if(clustered is null)
            {
                return (null, null);
            }
            var linePointsDict = clustered.GroupBy(r => r.Value)
                  .ToDictionary(t => t.Key, t => t.Select(r => r.Key).ToList());

            var globalTree = new List<Edge>();
            foreach (var linePoints in linePointsDict)
            {
                var treeOneLine = new List<Edge>();
                var e = new Edge { Point1 = linePoints.Key.StartPoint, Point2 = linePoints.Key.EndPoint };

                var (minDist, minPointInters) = linePoints.Value.Min(x => linePoints.Key.FindDistanceToPoint(x));//todoCheck

                var withIntersPoint = linePoints.Value;
                withIntersPoint.Add(minPointInters);

                var (graph, treeWithEdges1) = AlgMul.InitWeights(withIntersPoint);
                var (distance, tree) = AlgMul.Prim(graph, linePoints.Value);

                tree.ForEach(x => x.RoutedPoints = ApiRequest.RouterFromToGetPoints(x.Point1, x.Point2).Item2);
                tree.ForEach(x => x.RoutedLines = ApiRequest.RouterFromToGetLines(x.Point1, x.Point2).Item2);

                globalTree.AddRange(tree);
            }
            return (globalTree, linePointsDict);
        }

    }
}
