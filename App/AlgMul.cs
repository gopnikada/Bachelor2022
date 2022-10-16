using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingApp
{
    internal static class AlgMul
    {
        private static int MinKey(double[] key, bool[] set, int verticesCount)
        {
            double min = int.MaxValue;
            int minIndex = 0;

            for (int v = 0; v < verticesCount; ++v)
            {
                if (set[v] == false && key[v] < min)
                {
                    min = key[v];
                    minIndex = v;
                }
            }

            return minIndex;
        }

        private static void Print(int[] parent, double[,] graph, int verticesCount)
        {
            Console.WriteLine("Edge     Weight");
            for (int i = 1; i < verticesCount; ++i)
                Console.WriteLine(graph[i, parent[i]]);
        }

        public static (double, List<Edge>) Prim(double[,] graph, List<Point> points)
        {
            int verticesCount = graph.GetLength(0);

            int[] parent = new int[verticesCount];
            double[] key = new double[verticesCount];
            bool[] mstSet = new bool[verticesCount];

            for (int i = 0; i < verticesCount; ++i)
            {
                key[i] = int.MaxValue;
                mstSet[i] = false;
            }

            key[0] = 0;
            parent[0] = -1;

            for (int count = 0; count < verticesCount - 1; ++count)
            {
                int u = MinKey(key, mstSet, verticesCount);
                mstSet[u] = true;

                for (int v = 0; v < verticesCount; ++v)
                {
                    if (Convert.ToBoolean(graph[u, v]) && mstSet[v] == false && graph[u, v] < key[v])
                    {
                        parent[v] = u;
                        key[v] = graph[u, v];
                    }
                }
            }

            double distance = 0;
            var tree = new List<Edge>();

            for (int i = 1; i < verticesCount; ++i)
            {
                var e = new Edge() { Vertex1 = parent[i], Vertex2 = i, Weight = graph[i, parent[i]], Point1 = points[parent[i]], Point2 = points[i] };
                tree.Add(e);

                distance += graph[i, parent[i]];
            }

            return (distance, tree);
        }

        public static (double[,], List<Edge>) InitWeights(List<Point> points)
        {
            double[,] matrixArr = new double[points.Count, points.Count];
            var matrix = new List<List<double>>() { };
            var tree = new List<Edge>();
            foreach (var pointFrom in points)
            {
                var raw = new List<double>() { };
                foreach (var pointTo in points)
                {
                    var edge = new Edge();
                    var routed = ApiRequest.RouterFromToGetLines(pointFrom, pointTo);
                    edge.RoutedLines = routed.Item2;
                    tree.Add(edge);
                    var value = (pointTo != pointFrom) ? routed.Item1 : 0;
                    raw.Add(value);

                }
                matrix.Add(raw);
            }
            int counter = 0;
            foreach (var raw in matrix)
            {
                var rawArr = raw.ToArray();
                double[] myRowVector = raw.ToArray();

                matrixArr.SetRow(counter, myRowVector);
                counter++;
            }
            return (matrixArr, tree);
        }
    }

}
