using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using YANUSConnector.Adapter;
using YANUSConnector.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YANUSConnector.Http;

namespace YANUSConnector.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class LoginCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                TokenManager.EnsureFileExists();
                // Check if the window is already open
                if (GlobalData.webView == null || !GlobalData.webView.IsLoaded)
                {
                    // Create a new window if none exists or if it was closed
                    GlobalData.webView = new WebView();
                    GlobalData.webView.Closed += (sender, args) => GlobalData.webView = null; // Clear reference when closed
                    GlobalData.webView.Show();
                }
                else
                {
                    // Bring the existing window to the foreground
                    GlobalData.webView.Focus();
                }
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
}
