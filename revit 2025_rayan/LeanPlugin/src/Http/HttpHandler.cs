using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using YANUSConnector.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Net;
using Autodesk.Revit.DB.Architecture;
using YANUSConnector.GUI;

namespace YANUSConnector.Http
{
    public static class HttpHandler
    {

        public static async void SendToBubbleAPI(string json)
        {
            if (json == null) { return; }

            string apiUrl = Main.apiValue;

            using (var client = new HttpClient())
            {
                // Set the request content type to JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    // Send the POST request
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        //TaskDialog.Show("Success", "Response: " + responseContent);
                        string Content = await response.Content.ReadAsStringAsync();
                        ModelResponse? modelResponse = JsonSerializer.Deserialize<ModelResponse>(Content);
                        if (modelResponse == null)
                            TaskDialog.Show("Error", "response content is wrong");
                        else
                        {
                            var msg = modelResponse.response.message;
                            //TaskDialog.Show("msg", msg);
                            //todo check msg and decide if go to login or all cool
                            if (msg == "Success")
                            {
                                TaskDialog.Show("Success", "3D Model data & textures sent successfully to YANUS.AI.");
                                //TaskDialog.Show("Success", modelResponse.response.link);

                                // Check if the window is already open
                                if (GlobalData.webView == null || !GlobalData.webView.IsLoaded)
                                {
                                    // Create a new window if none exists or if it was closed
                                    GlobalData.webView = new WebView();
                                    GlobalData.webView.Closed += (sender, args) => GlobalData.webView = null; // Clear reference when closed
                                    GlobalData.webView.Show();
                                    GlobalData.webView.ChangeSrc(modelResponse.response.link!);
                                }
                                else
                                {
                                    // Bring the existing window to the foreground
                                    GlobalData.webView.Focus();
                                }
                            }
                            else
                            {
                                Adapter.RevitAdapter.HideAppButtons();
                                TokenManager.WriteToken("");
                            }
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.MovedPermanently || response.StatusCode == HttpStatusCode.Found)
                    {
                        // Handle 301 or 302 redirection
                        string redirectUrl = response.Headers.Location.ToString();
                        TaskDialog.Show("Redirection", redirectUrl);

                        //// Optionally follow the redirection:
                        //HttpResponseMessage redirectResponse = await client.GetAsync(redirectUrl);

                        //string redirectedContent = await redirectResponse.Content.ReadAsStringAsync();
                        //TaskDialog.Show("Success", "Redirected content received: " + redirectedContent);
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        TaskDialog.Show("Error", "Error: " + response.StatusCode + "\n" + errorContent);
                    }
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("YANUS Exception", "Exception: " + ex.Message);

                }
            }
        }

        public static string ImageDataToJson(string imagePath, Dictionary<string, Color> map)
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            File.Delete(imagePath);

            // Convert the byte array to a Base64 string
            string base64Image = Convert.ToBase64String(imageBytes);


            //    write the nase64 to image
            //// Decode the Base64 string back into a byte array
            //byte[] imageBytes2 = Convert.FromBase64String(base64Image);

            //// Define the path where you want to save the new image
            //string desktopPath2 = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //string outputPath = Path.Combine(desktopPath2, "RevitSolidTextureScreenshotAfter.jpg");
            //// Write the byte array back into an image file
            //File.WriteAllBytes(outputPath, imageBytes2);


            var materialColorMappings = new List<MaterialColorMapping>();

            foreach (var kvp in map)
            {
                var R = kvp.Value.Red;
                var G = kvp.Value.Green;
                var B = kvp.Value.Blue;
                materialColorMappings.Add(new MaterialColorMapping
                {
                    MaterialName = kvp.Key,
                    Color = $"({R},{G},{B})"
                    //Color = new ColorDTO
                    //{
                    //    R = kvp.Value.Red,
                    //    G = kvp.Value.Green,
                    //    B = kvp.Value.Blue
                    //}
                });
            }
            // if user is not log in
            if (TokenManager.ReadToken() == null || TokenManager.ReadToken() == "")
            {
                return "";
            }

            var dataToSend = new
            {
                ImageData = base64Image,
                map = materialColorMappings,
                token = TokenManager.ReadToken()
            };

            // Create JsonSerializerOptions with indented formatting
            var options = new JsonSerializerOptions
            {
                //WriteIndented = true
            };

            // Serialize the object to JSON with indented formatting
            string json = JsonSerializer.Serialize(dataToSend, options);

            // Log the full Base64 string to a text file for inspection
            // * only in dev
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string logFilePath = Path.Combine(desktopPath, "base64_image.json");
            File.WriteAllText(logFilePath, json);

            return json;
        }
    }
}
