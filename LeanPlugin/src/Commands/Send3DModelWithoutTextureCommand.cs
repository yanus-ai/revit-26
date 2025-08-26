using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using YANUS_Connector.Adapter;
using YANUS_Connector.Http;
using YANUS_Connector.src.Viewport;
using YANUS_Connector.Viewport;

namespace YANUS_Connector.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Send3DModelWithoutTextureCommand : IExternalCommand
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

                TokenManager.EnsureFileExists();

                Dictionary<string, Color> map = new Dictionary<string, Color>();

                // Check if the active view is a 3D View
                //if (activeView is View3D)
                //{
                // Collect all elements in the active view
                ElementId viewId = uidoc.ActiveView.Id;
                FilteredElementCollector collector = new FilteredElementCollector(doc, viewId);


                // Define unique colors for materials
                List<Color> uniqueColors = new List<Color>
                    {
                        new Color(255, 0, 0),    // Red
                        new Color(0, 255, 0),    // Green
                        new Color(0, 0, 255),    // Blue
                        new Color(255, 255, 0),  // Yellow
                        new Color(255, 0, 255),  // Magenta
                        new Color(0, 255, 255),  // Cyan
                        new Color(192, 192, 192),// Light Gray
                        new Color(128, 0, 0),    // Dark Red
                        new Color(0, 128, 0),    // Dark Green
                        new Color(0, 0, 128),    // Dark Blue
                        new Color(255, 165, 0),  // Orange
                        new Color(75, 0, 130),   // Indigo
                        new Color(238, 130, 238),// Violet
                        new Color(128, 128, 128),// Gray
                        new Color(128, 128, 0),  // Olive
                        new Color(0, 128, 128),  // Teal
                        new Color(255, 105, 180),// Hot Pink
                        new Color(139, 69, 19),  // Saddle Brown
                        new Color(255, 182, 193),// Light Pink
                        new Color(70, 130, 180), // Steel Blue
                        new Color(0, 100, 0),    // Dark Green
                        new Color(255, 228, 196),// Bisque
                        new Color(34, 139, 34),  // Forest Green
                        new Color(255, 99, 71),  // Tomato
                        new Color(139, 0, 0),    // Dark Red
                        new Color(112, 128, 144),// Slate Gray
                        new Color(135, 206, 250),// Light Sky Blue
                        new Color(210, 105, 30), // Chocolate
                        new Color(128, 0, 128),  // Purple
                        new Color(173, 255, 47), // Green-Yellow
                        new Color(245, 222, 179),// Wheat
                        new Color(100, 149, 237),// Cornflower Blue
                        new Color(255, 218, 185),// Peach Puff
                        new Color(154, 205, 50), // Yellow-Green
                        new Color(0, 191, 255),  // Deep Sky Blue
                        new Color(127, 255, 0),  // Chartreuse
                        new Color(255, 69, 0),   // Red-Orange
                        new Color(240, 230, 140),// Khaki
                        new Color(147, 112, 219),// Medium Purple
                        new Color(46, 139, 87),  // Sea Green
                        new Color(205, 92, 92),  // Indian Red
                        new Color(218, 165, 32), // Goldenrod
                        new Color(0, 255, 127),  // Spring Green
                        new Color(244, 164, 96), // Sandy Brown
                        new Color(186, 85, 211), // Medium Orchid
                        new Color(50, 205, 50),  // Lime Green
                        new Color(255, 20, 147), // Deep Pink
                        new Color(199, 21, 133), // Medium Violet Red
                        new Color(72, 61, 139),  // Dark Slate Blue
                    };


                using (Transaction tx = new Transaction(doc, "Change Active Materials to Unique Colors"))
                {
                    tx.Start();

                    // Get unique materials from the visible elements
                    HashSet<ElementId> uniqueMaterialIds = RevitAdapter.GetUniqueMaterialIds(doc);
                    if (uniqueMaterialIds.Count == 0)
                    {
                        TaskDialog.Show("Yanus Error", "Only views that contain materials can be sent through this connection.");
                        return Result.Failed;
                    }

                    activeView.DisplayStyle = DisplayStyle.FlatColors;

                    var orginalTheme = ((int)UIThemeManager.CurrentTheme);

                    UIThemeManager.CurrentTheme = UITheme.Dark;

                    ViewportUtils.CaptureViewport(activeView);

                    string imagePathUnmapped = ViewportUtils.ExportImage(doc, "RevitSolidWithoutTextureScreenshot.jpg");


                    // Map unique colors to each material
                    int index = 0;
                    foreach (ElementId materialId in uniqueMaterialIds)
                    {
                        Material material = doc.GetElement(materialId) as Material;


                        if (material != null)
                        {
                            // Set the material color to the unique color, cycling through the list
                            if (index < uniqueColors.Count)
                            {
                                material.Color = uniqueColors[index];
                                map.Add(material.Name, uniqueColors[index]);
                                index++;
                            }
                            else
                            {
                                // Generate a new random color that doesn't already exist in uniqueColors
                                Color newRandomColor = ColorUtils.GetUniqueRandomColor(uniqueColors);
                                uniqueColors.Add(newRandomColor);

                                material.Color = newRandomColor;
                                map.Add(material.Name, newRandomColor);
                                index++;
                            }
                        }

                    }
                   


                    string imagePath = ViewportUtils.ExportImage(doc, "RevitSolidTextureScreenshot.jpg");


                    // TaskDialog.Show("Screenshot", $"Screenshot saved to: {imagePath}");

                    //send to the api
                    var json = HttpHandler.ImageDataToJson(imagePath, map, imagePathUnmapped);
                    if (json != "")
                    {
                        json.WriteJsonToTxtFile("C:\\3DModelWithoutTexture.json");
                        HttpHandler.SendToBubbleAPI(json);
                    }
                    else
                    {
                        TaskDialog.Show("YANUS Connector Error", "Sending to YANUS has failed, please login again.");
                        RevitAdapter.HideAppButtons();
                    }
                    tx.RollBack();
                    UIThemeManager.CurrentTheme = (UITheme)orginalTheme;

                }
                uidoc.RefreshActiveView();

                //}
                //else
                //{
                //    TaskDialog.Show("Active Camera", "The active view is not a 3D view. Active view: " + activeView.Name);
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
