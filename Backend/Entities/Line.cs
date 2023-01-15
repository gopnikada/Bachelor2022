using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace RoutingApp
{
    internal class Line
    {
        public static int IndexStat { get; set; }
        public int Index { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public double XDiff { get; set; }
        public double YDiff { get; set; }
        public double Length { get; set; }
        public Polynomial Formula { get; set; }
        public double K { get; set; }
        public double B { get; set; }
        public Line() { }
        public Line(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            XDiff = Math.Abs(EndPoint.Longtitude - StartPoint.Longtitude);//module
            YDiff = Math.Abs(EndPoint.Latitude - StartPoint.Latitude);
            Length = measureDist(startPoint, endPoint);
                //Math.Sqrt(Math.Pow((double)YDiff, 2) +
                //Math.Pow((double)XDiff, 2));

            Formula = MakeFormula();
            K = Formula.Coefficients[1];
            B = Formula.Coefficients[0];
           
        }
   public (double, Point) FindDistanceToPoint(Point point)
        {

            var A = Formula.Coefficients[1];
            var B = -1;
            var C = Formula.Coefficients[0];

            var sqrtEd = Math.Sqrt(Math.Pow(A, 2) +
                Math.Pow(B, 2));

            var amount = A * point.Longtitude + B * point.Latitude + C;
            if (amount < 0) amount *= -1;
            //var d = amount / sqrtEd;

            var k2 = -1 / A;
            var b2 = point.Latitude - k2 * point.Longtitude;


            var pointA = Matrix<double>.Build.DenseOfArray(new double[,] {
            {A, -1 },
            {k2, -1}
            });

            var pointB = Vector<double>.Build.Dense(new double[]
            { -C, -b2 }
            );

            var pointInters = pointA.Solve(pointB);

            //double d = measureDist(new Point(pointInters[0], pointInters[1]), point);
            double d = ApiRequest.RouterFromToGetLines(StartPoint, point).Item1;
            bool perpOnLine = CheckPointOnLine(pointInters);

            if (!perpOnLine)
            {
                var line1 = new Line(StartPoint, point);
                var line2 = new Line(EndPoint, point);

                if (line1.Length <= line2.Length)
                {
                    return (d, StartPoint);
                }
                else
                {
                    return (d, EndPoint);
                }
            }

            return (d, new Point(pointInters[0], pointInters[1]));
        }

        private bool CheckPointOnLine(Vector<double> point)
        {
            double Xp1 = StartPoint.Longtitude;
            double Yp1 = StartPoint.Latitude;

            double Xp2 = EndPoint.Longtitude;
            double Yp2 = EndPoint.Latitude;

            double Xi = point[0];
            double Yi = point[1];

            if (Xp1 < Xp2 && Yp1 < Yp2)
            {
                if (Xp1 <= Xi && Xi <= Xp2 &&
                    Yp1 <= Yi && Yp1 <= Yp2)
                {
                    return true;
                }
                else return false;
            }
            else if (Xp1 < Xp2 && Yp1 > Yp2)
            {
                if (Xp1 <= Xi && Xi <= Xp2 &&
                    Yp1 >= Yi && Yp1 >= Yp2)
                {
                    return true;
                }
                else return false;
            }
            else if (Xp1 > Xp2 && Yp1 > Yp2)
            {
                if (Xp1 >= Xi && Xi >= Xp2 &&
                    Yp1 >= Yi && Yp1 >= Yp2)
                {
                    return true;
                }
                else return false;
            }
            else if (Xp1 > Xp2 && Yp1 < Yp2)
            {
                if (Xp1 >= Xi && Xi >= Xp2 &&
                    Yp1 <= Yi && Yp1 <= Yp2)
                {
                    return true;
                }
                else return false;
            }
            else
            {
                return true;
            }
        }

        Polynomial MakeFormula()
        {
            var A = Matrix<double>.Build.DenseOfArray(new double[,] {
            {StartPoint.Longtitude, 1 },
            {EndPoint.Longtitude, 1}
            });

            var b = Vector<double>.Build.Dense(new double[]
            { StartPoint.Latitude, EndPoint.Latitude }
            );

            var x = A.Solve(b);

            return new Polynomial(x[1], x[0]);
        }

        public double FindAngle(Line line2)
        {
            //double amount = (K - line2.K) / 1 + K * line2.K;
            //if (amount < 1) amount *= -1;
            //return Math.Atan(amount);
            double amount =  1 + (K * line2.K);
            return amount;
        }

        public double measureDist(Point startPoint, Point endPoint)
        {  // generally used geo measurement function
            double lat1 = startPoint.Latitude;
            double lon1 = startPoint.Longtitude;
            double lat2 = endPoint.Latitude;
            double lon2 = endPoint.Longtitude;

            var R = 6378.137; // Radius of earth in KM
            var dLat = lat2 * Math.PI / 180 - lat1 * Math.PI / 180;
            var dLon = lon2 * Math.PI / 180 - lon1 * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;
            return d * 1000; // meters
        }
        public static List<Point> LineZerlegung(Line line, int _discrGrade)
        {

            var points = new List<Point>();
            points.Add(line.StartPoint);
            points.Add(line.EndPoint);
            double lineLength = line.Length;


            if (lineLength > _discrGrade)
            {
                int segmentsNumber = (int)Math.Truncate(lineLength / _discrGrade);

                double xdiff = line.XDiff / segmentsNumber;
                double ydiff = line.YDiff / segmentsNumber;

                Point currentPointToAddTo = line.StartPoint;

                for (int j = 1; j < segmentsNumber; j++)
                {
                    var newPoint = new Point(currentPointToAddTo.Longtitude + xdiff, currentPointToAddTo.Latitude + ydiff);
                    points.Add(newPoint);
                    currentPointToAddTo = newPoint;
                }
            }
            return points;
        }

        public override bool Equals(object obj)
        {
            //if ((obj as Line).StartPoint == StartPoint && (obj as Line).StartPoint == StartPoint)
            if((obj as Line).Length == Length)
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