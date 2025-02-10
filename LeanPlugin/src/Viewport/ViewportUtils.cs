using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace YANUS_Connector.Viewport
{
    public class ViewportUtils
    {
        //Helper to clear the view to be exported
        public static void CaptureViewport(View activeView)
        {
            // Create a new override settings for the view
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            ogs.SetSurfaceBackgroundPatternVisible(true);
            ogs.SetSurfaceForegroundPatternVisible(true);
            ogs.SetProjectionLineColor(new Color(0, 0, 0)); // Set color to white or match the background

            ogs.SetProjectionLineWeight(1);
            // Get all elements in the view
            FilteredElementCollector collector2 = new FilteredElementCollector(activeView.Document, activeView.Id);
            collector2.WhereElementIsNotElementType();

            foreach (Element element in collector2)
            {
                // Apply the override settings to all visible elements
                activeView.SetElementOverrides(element.Id, ogs);
            }

            // Turn off annotation categories (text, levels, etc.)
            HideAnnotationCategories(activeView, activeView.Document);

            // Turn off edges or any lines on the surfaces
            activeView.DetailLevel = ViewDetailLevel.Fine; // Ensures that fine details (like lines) are minimized

            //  hide levels if they are visible in the 3D view
            HideCategoryInView(activeView, BuiltInCategory.OST_Levels, activeView.Document);

            //  hide grids if they are visible in the 3D view
            HideCategoryInView(activeView, BuiltInCategory.OST_Grids, activeView.Document);
        }

        public static string ExportImage(Document doc,string name)
        {
            // Save screenshot as an image file
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string imagePath = Path.Combine(desktopPath, name);

            ImageExportOptions options = new ImageExportOptions
            {
                ZoomType = ZoomFitType.FitToPage,
                ExportRange = ExportRange.VisibleRegionOfCurrentView,
                FilePath = imagePath,
                HLRandWFViewsFileType = ImageFileType.JPEGLossless,
                PixelSize = 1024,
                ImageResolution = ImageResolution.DPI_150,
                ViewName = doc.ActiveView.Name,
                FitDirection = FitDirectionType.Horizontal,
                
            };

            doc.ExportImage(options);

            return imagePath;
        }










        // Helper function to hide annotation categories in the view
        private static void HideAnnotationCategories(View view, Document doc)
        {
            // Hide text notes (and other annotation-related categories like dimensions, tags, etc.)
            Category textNotesCategory = doc.Settings.Categories.get_Item(BuiltInCategory.OST_TextNotes);
            HideCategoryInView(view, BuiltInCategory.OST_TextNotes, doc);

            // Hide dimensions, tags, and other annotations
            HideCategoryInView(view, BuiltInCategory.OST_Dimensions, doc);
            HideCategoryInView(view, BuiltInCategory.OST_Tags, doc);
            HideCategoryInView(view, BuiltInCategory.OST_GenericAnnotation, doc);
            HideCategoryInView(view, BuiltInCategory.OST_Levels, doc);
            HideCategoryInView(view, BuiltInCategory.OST_DirectionEdgeLines, doc);
            HideCategoryInView(view, BuiltInCategory.OST_FloorsInteriorEdges, doc);
            HideCategoryInView(view, BuiltInCategory.OST_RoofsInteriorEdges, doc);
            HideCategoryInView(view, BuiltInCategory.OST_EdgeSlab, doc);
            HideCategoryInView(view, BuiltInCategory.OST_XRaySideEdge, doc);
            HideCategoryInView(view, BuiltInCategory.OST_XRayProfileEdge, doc);
#if V2023 || V2024
            HideCategoryInView(view, BuiltInCategory.OST_SlabEdgeTags, doc);
#endif

            // Iterate through subcategories if needed
            CategoryNameMap subCategories = textNotesCategory.SubCategories;
            foreach (Category subCat in subCategories)
            {
                if (view.CanCategoryBeHidden(subCat.Id))
                {
                    view.SetCategoryHidden(subCat.Id, true);
                }
            }
        }

        // Helper function to hide specific categories in the view
        private static void HideCategoryInView(View view, BuiltInCategory category, Document doc)
        {
            Category cat = doc.Settings.Categories.get_Item(category);
            if (cat != null && view.CanCategoryBeHidden(cat.Id))
            {
                view.SetCategoryHidden(cat.Id, true);
            }
        }
    }
}
