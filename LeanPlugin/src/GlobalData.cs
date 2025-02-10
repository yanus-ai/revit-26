using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YANUS_Connector.GUI;

namespace YANUS_Connector
{
    public static class GlobalData
    {
        
        public static string websrc { get; set; } = "";
        public static WebView webView { get; set; } = null;
        //public static string? jwt { get; set; } = null;
        public static bool isLogin { get; set; } = false; 
        public static UIControlledApplication app { get; set; }
    }
}
