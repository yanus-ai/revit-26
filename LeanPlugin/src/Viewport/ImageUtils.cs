using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YANUS_Connector.src.Viewport
{
    public class ImageUtils
    {
        public static string ConvertImageToBase64(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return string.Empty;
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            File.Delete(imagePath);

            // Convert the byte array to a Base64 string
            string base64Image = Convert.ToBase64String(imageBytes);

            return base64Image;
        }
    }
}
