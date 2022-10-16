using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoutingApp;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var point1 = new Point(11.001387016281072, 51.00289015052401);
            var point2 = new Point(10.952048616166008, 51.024579079916755);
            var point3 = new Point(10.999276540111577, 51.00540979210159);
            var point4 = new Point(11.004732925840262, 51.00352584567375);
            var point5 = new Point(11.002805815488756, 51.00103478885242);

            Config.PATH_TO_SAVE_KML_TRASSEN = @"C:\Users\KyK\Documents\repos\Appold2\Generated\trassenChanchedPath.kml";
            Config.PATH_TO_SAVE_KML_LINESTRINGS = @"C:\Users\KyK\Documents\repos\Appold2\Generated\LinseStrings2.kml";
            
            var points = new List<Point>() { point1, point2, point3
                , point4, point5
                };
            
            var jsonMul = MainWork.RouteMultiplePoints(points);
            //var jsonOne = RoutingApp.MainWork.RouteOnePoint(point1);
           
        }
    }
}
