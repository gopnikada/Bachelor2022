using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingApp
{
    public static class Config
    {
        internal static int ENDRADIUS = 0;
        internal static int MAXRADIUS = 2500;
        internal static int InitRadius = 100;
        internal static string RadiusOverflowMsg = $"No ways in radius {MAXRADIUS}";
        //RoutingEngine
        internal static string HOST = "localhost";
        internal static string PORT = "8989";

        //DB
        internal static string DBServer = "DESKTOP-7MM84NR";
        internal static string DBName = "master";
        internal static string DBConnString = $"Server={DBServer};Database={DBName};Trusted_Connection=True;";

        //paths to visualise
        public static string PATH_TO_SAVE_KML_TRASSEN = @"C:\Users\KyK\Documents\repos\Appold2\Generated\testttttttyt.kml";
        public static string PATH_TO_SAVE_KML_LINESTRINGS = @"C:\Users\KyK\Documents\repos\Appold2\Generated\LinseStrings.kml";
      
    }
}
