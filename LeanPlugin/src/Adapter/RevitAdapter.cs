using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace YANUS_Connector.Adapter
{
    public static class RevitAdapter
    {
        public static void ShowAppButtons()
        {
            var list = GlobalData.app.GetRibbonPanels("YANUS Connector");
            foreach (var ribbon in list)
            {
                if (ribbon.Name == "Connection")
                    ribbon.Visible = true;
                else if (ribbon.Name == "Signin")
                    ribbon.Visible = false;
                else if (ribbon.Name == "Logout")
                    ribbon.Visible = true;
                else if (ribbon.Name == "Connection Without Texture")
                    ribbon.Visible = true;

            }
            GlobalData.isLogin = true;
        }
        public static void HideAppButtons()
        {
            var list = GlobalData.app.GetRibbonPanels("YANUS Connector");
            foreach (var ribbon in list)
            {
                if (ribbon.Name == "Connection")
                    ribbon.Visible = false;
                else if (ribbon.Name == "Signin")
                    ribbon.Visible = true;
                else if (ribbon.Name == "Logout")
                    ribbon.Visible = false;
                else if (ribbon.Name == "Connection Without Texture")
                    ribbon.Visible = false;

            }
            GlobalData.isLogin = false;
        }
        //* Used only in transaction so we hide elements
        public static HashSet<ElementId> GetUniqueMaterialIds(Document doc)
        {
            ElementId viewId = doc.ActiveView.Id;
            FilteredElementCollector collector = new FilteredElementCollector(doc, viewId);

            HashSet<ElementId> uniqueMaterialIds = new HashSet<ElementId>();

            List<ElementId> noMaterialElements = new List<ElementId>();


            foreach (Element element in collector)
            {
                ICollection<ElementId> materialIds = element.GetMaterialIds(false);


                //hide elements that have no material, like grid or box or dimention
                if (materialIds.Count == 0 && element.CanBeHidden(doc.ActiveView))
                    noMaterialElements.Add(element.Id);

                foreach (ElementId materialId in materialIds)
                {
                    uniqueMaterialIds.Add(materialId);
                }
            }
            if (noMaterialElements.Count > 0)
                doc.ActiveView.HideElements(noMaterialElements);

            return uniqueMaterialIds;
        }
    }
}
