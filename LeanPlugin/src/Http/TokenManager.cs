using Autodesk.Revit.UI;
using System;
using System.IO;

namespace YANUS_Connector.Http
{
    public static class TokenManager
    {
        private static string _filePath = "C:\\ProgramData\\TYPUS_Connector\\token.txt";

        // Method to initialize the file path
        public static void Initialize(string filePath)
        {
            _filePath = filePath;
        }

        // Static method to write the token to the file
        public static void WriteToken(string token)
        {
            try
            {
                // Write the token to the file (overwriting if it exists)
                File.WriteAllText(_filePath, token);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Exception", $"Error writing token to file: {ex.Message}");
            }
        }

        // Static method to read the token from the file
        public static string ReadToken()
        {
            try
            {
                // Check if the file exists
                if (File.Exists(_filePath))
                {
                    // Read the token from the file
                    return File.ReadAllText(_filePath);
                }
                else
                {
                    TaskDialog.Show("Exception", "Token file does not exist.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Exception", $"Error reading token from file: {ex.Message}");
                return null;
            }
        }

        // Static method to check if the file exists, and create it with an empty string if not
        public static void EnsureFileExists()
        {
            try
            {
                // Ensure the directory exists, if not, create it
                string directoryPath = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Check if the file already exists
                if (!File.Exists(_filePath))
                {
                    // Create an empty file and write an empty string
                    File.WriteAllText(_filePath, "");
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Exception", $"Error ensuring file exists: {ex.Message}");
            }
        }
    }
}
