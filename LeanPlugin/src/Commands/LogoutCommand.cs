using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using YANUS_Connector.Adapter;
using YANUS_Connector.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YANUS_Connector.Http;
using System.Windows;

namespace YANUS_Connector.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class LogoutCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;


                // Show a confirmation dialog
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to logout?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                // Check the user's choice
                if (result == MessageBoxResult.Yes)
                {
                    // Execute the code only if the user clicks "Yes"
                    TokenManager.EnsureFileExists();
                    TokenManager.WriteToken("");
                    RevitAdapter.HideAppButtons();
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
