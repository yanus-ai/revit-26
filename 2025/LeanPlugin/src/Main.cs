using System;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media.Media3D;
using Material = Autodesk.Revit.DB.Material;
using System.Xml.Linq;
using System.Windows;
using YANUSConnector.GUI;
using Microsoft.Web.WebView2.Core;
using YANUSConnector.Adapter;
using System.Resources;
using YANUSConnector.Http;

namespace YANUSConnector

{
    /// <remarks>
    /// This application's main class. The class must be Public.
    /// </remarks>
    public class Main : IExternalApplication
    {
        public static string apiValue;
        // Both OnStartup and OnShutdown must be implemented as public method
        public Result OnStartup(UIControlledApplication application)
        {
            GlobalData.app = application;

            application.CreateRibbonTab("YANUS Connector");

            // Add a new ribbon panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("YANUS Connector", "Connection");
            var imagePath = Path.Combine("Resources", "images", "logo.png");

            byte[] iconImage = (byte[])YANUSConnector.Properties.Resources.ResourceManager.GetObject("logo", YANUSConnector.Properties.Resources.Culture);
            byte[] loginIconImage = (byte[])YANUSConnector.Properties.Resources.ResourceManager.GetObject("loginLogo", YANUSConnector.Properties.Resources.Culture);

            ContextualHelp contextualHelp = new ContextualHelp(ContextualHelpType.Url, "https://yanus.ai/plugins");

            // Create a push button to trigger a command add it to the ribbon panel.
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            PushButtonData Send3DModelCommandData = new PushButtonData("Send3DModel",
               "Send 3D Model", thisAssemblyPath, "YANUSConnector.Commands.Send3DModelCommand");
            Send3DModelCommandData.ToolTip = "Click from within a 3D/Camera view to send data to Yanus.AI\nDon't use Wireframe Visual Style";
            Send3DModelCommandData.LargeImage = LoadImageFromByteArray(iconImage!);
            Send3DModelCommandData.SetContextualHelp(contextualHelp);

            PushButton Send3DModelCommandbtn = ribbonPanel.AddItem(Send3DModelCommandData) as PushButton;

            ribbonPanel.AddSlideOut();

            TextBoxData textData = new TextBoxData("backend api");
            textData.LongDescription = "Insert the api that the btn will send the image data to it.";
            TextBox tBox = ribbonPanel.AddItem(textData) as TextBox;
            tBox.Width = 100;
            tBox.PromptText = "Insert api here";
            tBox.Value = "https://app.yanus.ai/api/1.1/wf/revitintegration";
            //tBox.Value = "https://app.yanus.ai/version-test/api/1.1/wf/revitintegration";
            //tBox.Value = "https://vistack4.bubbleapps.io/version-test/api/1.1/wf/revitintegration";
            apiValue = tBox.Value as string;
            tBox.EnterPressed += TBox_EnterPressed;


            //Loginnnnnn
            RibbonPanel loginRibbonPanel = application.CreateRibbonPanel("YANUS Connector", "Signin");
            PushButtonData LoginCommandData = new PushButtonData("LoginCommand",
            "Login", thisAssemblyPath, "YANUSConnector.Commands.LoginCommand");
            LoginCommandData.ToolTip = "Click to login";
            LoginCommandData.LargeImage = LoadImageFromByteArray(loginIconImage!);
            LoginCommandData.SetContextualHelp(contextualHelp);


            PushButton LoginCommandbtn = (PushButton)loginRibbonPanel.AddItem(LoginCommandData);

            //Logoutttt
            RibbonPanel logoutRibbonPanel = application.CreateRibbonPanel("YANUS Connector", "Logout");
            PushButtonData LogoutCommandData = new PushButtonData("LogoutCommand",
            "Logout", thisAssemblyPath, "YANUSConnector.Commands.LogoutCommand");
            LogoutCommandData.ToolTip = "Click to logout";
            LogoutCommandData.LargeImage = LoadImageFromByteArray(loginIconImage!);
            LogoutCommandData.SetContextualHelp(contextualHelp);


            PushButton LogoutCommandbtn = (PushButton)logoutRibbonPanel.AddItem(LogoutCommandData);

            ////Testttttttttttttttt
            //RibbonPanel ribbonPanelTest = application.CreateRibbonPanel("YANUS Connector", "Test");
            //ribbonPanelTest.Visible = true;
            //PushButtonData buttonData = new PushButtonData("Test",
            //  "Test Btn", thisAssemblyPath, "YANUSConnector.Test");

            //PushButton pushButton = ribbonPanelTest.AddItem(buttonData) as PushButton;

            TokenManager.EnsureFileExists();
            var token = TokenManager.ReadToken();
            if (token != "" && token != null)
            {
                RevitAdapter.ShowAppButtons();
            }
            else
            {
                RevitAdapter.HideAppButtons();
            }
            return Result.Succeeded;
        }

        private void TBox_EnterPressed(object? sender, Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs e)
        {

            apiValue = (string)(sender as TextBox).Value;
            TaskDialog.Show("API Changed", apiValue);

        }
        public BitmapImage LoadImageFromByteArray(byte[] imageData)
        {
            using (var ms = new MemoryStream(imageData))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();  // To make it cross-thread accessible
                return bitmapImage;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // nothing to clean up in this simple case
            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Test : IExternalCommand
    {
        // The main Execute method (inherited from IExternalCommand) must be public
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;

                Document doc = uidoc.Document;
                View activeView = doc.ActiveView;
                //var elems = MEPClashDetection.FindMEPCollisions(doc);
                //foreach (var elem in elems)
                //{
                //    TaskDialog.Show("Ss", elem.Item1.Name);
                //}

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