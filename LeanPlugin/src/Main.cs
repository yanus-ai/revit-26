using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using YANUS_Connector.Adapter;
using YANUS_Connector.Http;

namespace YANUS_Connector

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

            application.CreateRibbonTab("TYPUS.AI Connector");

            // Add a new ribbon panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("TYPUS.AI Connector", "Connection");
            //var imagePath = Path.Combine("Resources", "images", "logo.png");

            //byte[] iconImage = (byte[])Properties.Resources.ResourceManager.GetObject("logo", Properties.Resources.Culture);
            //byte[] loginIconImage = (byte[])Properties.Resources.ResourceManager.GetObject("loginLogo", Properties.Resources.Culture);

            Bitmap iconImageBitmap = (Bitmap)Properties.Resources.ResourceManager.GetObject("typus_logo", Properties.Resources.Culture);
            byte[] iconImage = ImageToByteArray(iconImageBitmap);

            Bitmap loginIconImageBitmap = (Bitmap)Properties.Resources.ResourceManager.GetObject("typus_logo_reversed", Properties.Resources.Culture);
            byte[] loginIconImage = ImageToByteArray(loginIconImageBitmap);


            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //string imagePath = Path.Combine(desktopPath, "test_image.png");
            //File.WriteAllBytes(imagePath, iconImage);


            ContextualHelp contextualHelp = new ContextualHelp(ContextualHelpType.Url, "https://app.typus.ai/plugins");

            // Create a push button to trigger a command add it to the ribbon panel.
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            PushButtonData Send3DModelCommandData = new PushButtonData("Capture Regions",
               "Capture Regions", thisAssemblyPath, "YANUS_Connector.Commands.Send3DModelCommand");
            Send3DModelCommandData.ToolTip = "Click from within a 3D/Camera view to send data to TYPUS.AI\nDon't use Wireframe Visual Style";
            if (iconImage != null)
                Send3DModelCommandData.LargeImage = LoadImageFromByteArray(iconImage);
            Send3DModelCommandData.SetContextualHelp(contextualHelp);

            PushButton Send3DModelCommandbtn = ribbonPanel.AddItem(Send3DModelCommandData) as PushButton;



            //Send 3DModel WithoutTexture
            //RibbonPanel ribbonPanel1 = application.CreateRibbonPanel("YANUS Connector", "Connection Without Texture");

            ribbonPanel.AddSeparator();

            PushButtonData Send3DModelWithoutTextureCommandData = new PushButtonData("Capture",
               "Capture", thisAssemblyPath, "YANUS_Connector.Commands.Send3DModelWithoutTextureCommand");
            Send3DModelWithoutTextureCommandData.ToolTip = "Click from within a 3D/Camera view to send data to TYPUS.AI\nDon't use Wireframe Visual Style";
            if (iconImage != null)
                Send3DModelWithoutTextureCommandData.LargeImage = LoadImageFromByteArray(iconImage);
            Send3DModelWithoutTextureCommandData.SetContextualHelp(contextualHelp);

            PushButton Send3DModelWithoutTextureCommandbtn = ribbonPanel.AddItem(Send3DModelWithoutTextureCommandData) as PushButton;

            ribbonPanel.AddSlideOut();

            TextBoxData textData = new TextBoxData("backend api");
            textData.LongDescription = "Insert the api that the btn will send the image data to it.";
            TextBox tBox = ribbonPanel.AddItem(textData) as TextBox;
            tBox.Width = 250;
            tBox.PromptText = "Insert api here";
            tBox.Value = "https://app.typus.ai/api/webhooks/create-input-image";
            //tBox.Value = "https://app.yanus.ai/api/1.1/wf/revitintegration";
            //tBox.Value = "https://app.yanus.ai/version-test/api/1.1/wf/revitintegration";
            //tBox.Value = "https://vistack4.bubbleapps.io/version-test/api/1.1/wf/revitintegration";
            apiValue = tBox.Value as string;
            tBox.EnterPressed += TBox_EnterPressed;

            //TextBoxData textData1 = new TextBoxData("backend api1");
            //textData1.LongDescription = "Insert the api that the btn will send the image data to it.";
            //TextBox tBox1 = ribbonPanel1.AddItem(textData1) as TextBox;
            //tBox1.Width = 100;
            //tBox1.PromptText = "Insert api here";
            //tBox1.Value = "https://app.yanus.ai/api/1.1/wf/revitintegration";
            ////tBox.Value = "https://app.yanus.ai/version-test/api/1.1/wf/revitintegration";
            ////tBox.Value = "https://vistack4.bubbleapps.io/version-test/api/1.1/wf/revitintegration";
            //apiValue = tBox1.Value as string;
            //tBox1.EnterPressed += TBox_EnterPressed;


            //Loginnnnnn
            RibbonPanel loginRibbonPanel = application.CreateRibbonPanel("TYPUS.AI Connector", "Signin");
            PushButtonData LoginCommandData = new PushButtonData("LoginCommand",
            "Login", thisAssemblyPath, "YANUS_Connector.Commands.LoginCommand");
            LoginCommandData.ToolTip = "Click to login";
            LoginCommandData.LargeImage = LoadImageFromByteArray(loginIconImage);
            LoginCommandData.SetContextualHelp(contextualHelp);


            PushButton LoginCommandbtn = (PushButton)loginRibbonPanel.AddItem(LoginCommandData);

            //Logoutttt
            RibbonPanel logoutRibbonPanel = application.CreateRibbonPanel("TYPUS.AI Connector", "Logout");
            PushButtonData LogoutCommandData = new PushButtonData("LogoutCommand",
            "Logout", thisAssemblyPath, "YANUS_Connector.Commands.LogoutCommand");
            LogoutCommandData.ToolTip = "Click to logout";
            LogoutCommandData.LargeImage = LoadImageFromByteArray(loginIconImage);
            LogoutCommandData.SetContextualHelp(contextualHelp);


            PushButton LogoutCommandbtn = (PushButton)logoutRibbonPanel.AddItem(LogoutCommandData);

            ////Testttttttttttttttt
            //RibbonPanel ribbonPanelTest = application.CreateRibbonPanel("YANUS Connector", "Test");
            //ribbonPanelTest.Visible = true;
            //PushButtonData buttonData = new PushButtonData("Test",
            //  "Test Btn", thisAssemblyPath, "YANUS_Connector.Test");

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

        private byte[] ImageToByteArray(System.Drawing.Bitmap image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        private void TBox_EnterPressed(object sender, Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs e)
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