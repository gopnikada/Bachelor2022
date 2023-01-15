using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingApp
{
    internal class PointFromDB
    {
        public double Longtitude { get; set; }
        public double Latitude { get; set; }

        public double DistToPoint { get; set; }
        public double RoutedDistToPoint { get; set; }

        public Point TargetPoint { get; set; }

        public List<Point> RoutedPoints { get; set; }
        public int N { get; set; }


        public PointFromDB(int n, double longtitude, double latitude, double distToPoint, Point targetPoint)
        {
            N = n;
            Longtitude = longtitude;
            Latitude = latitude;
            DistToPoint = distToPoint;
            TargetPoint = targetPoint;
        }
        public PointFromDB(double longtitude, double latitude, double distToPoint)
        {
            Longtitude = longtitude;
            Latitude = latitude;
            DistToPoint = distToPoint;
           
        }
        public PointFromDB(double longtitude, double latitude)
        {
            Longtitude = longtitude;
            Latitude = latitude;
        }

        public PointFromDB()
        {
        }

        internal Point ToPoint()
        {
            return new Point(Longtitude, Latitude);
        }

        
        public double measureDistToPoint(Point endPoint)
        {  // generally used geo measurement function
            double lat1 = Latitude;
            double lon1 = Longtitude;
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
        public override string ToString()
        {
            return Latitude+ " " + Longtitude;
        }
    }
}
