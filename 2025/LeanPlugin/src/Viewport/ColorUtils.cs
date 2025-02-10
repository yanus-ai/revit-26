using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YANUSConnector.src.Viewport
{
    public class ColorUtils
    {
        // Function to generate a unique random color
        public static Color GetUniqueRandomColor(List<Color> existingColors)
        {
            Color randomColor;
            bool isDuplicate;

            do
            {
                randomColor = GetRandomColor();
                isDuplicate = false;

                // Iterate over every color and compare RGB values
                foreach (Color existingColor in existingColors)
                {
                    if (AreColorsEqual(randomColor, existingColor))
                    {
                        isDuplicate = true;
                        break;
                    }
                }
            }
            while (isDuplicate);  // Repeat if the color is duplicate

            return randomColor;
        }
        private static Color GetRandomColor()
        {
            Random random = new Random();
            byte r = (byte)random.Next(0, 256);
            byte g = (byte)random.Next(0, 256);
            byte b = (byte)random.Next(0, 256);
            return new Color(r, g, b);
        }

        private static bool AreColorsEqual(Color color1, Color color2)
        {
            return color1.Red == color2.Red &&
                   color1.Green == color2.Green &&
                   color1.Blue == color2.Blue;
        }
    }
}
