using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using YANUS_Connector.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
#if !V2021
using System.Text.Json;
#else
using Newtonsoft.Json;
#endif
using System.Net;
using YANUS_Connector.GUI;
using System.Net.Http.Headers;
using YANUS_Connector.src.Viewport;
using System.Diagnostics;


namespace YANUS_Connector.Http
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
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenManager.ReadToken());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                LoadingWindow loadingWindow = new LoadingWindow();

                try
                {


                    // Show the loading screen
                    loadingWindow.Show();

                    // Send the POST request
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    // Close the loading screen once the response is received
                    loadingWindow.Close();

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        //TaskDialog.Show("Success", "Response: " + responseContent);
                        string Content = await response.Content.ReadAsStringAsync();

#if !V2021
                        ModelResponse modelResponse = JsonSerializer.Deserialize<ModelResponse>(Content);
#else
                        ModelResponse modelResponse = JsonConvert.DeserializeObject<ModelResponse>(Content);
#endif
                        if (modelResponse == null)
                            TaskDialog.Show("Error", "response content is wrong");
                        else
                        {
                            var msg = modelResponse.response.message;
                            //TaskDialog.Show("msg", msg);
                            //todo check msg and decide if go to login or all cool
                            if (msg.Contains("success"))
                            {
                                TaskDialog.Show("Success", "3D Model data & textures sent successfully to TYPUS.AI.");
                                //TaskDialog.Show("Success", modelResponse.response.link);

                                // Check if the window is already open
                                if (GlobalData.webView == null || !GlobalData.webView.IsLoaded)
                                {
                                    // Create a new window if none exists or if it was closed
                                    GlobalData.webView = new WebView();
                                    GlobalData.webView.Closed += (sender, args) => GlobalData.webView = null; // Clear reference when closed
                                    GlobalData.webView.Show();
                                    if (modelResponse.response.link != null)
                                    {
                                        await GlobalData.webView.ChangeSrc(modelResponse.response.link).ConfigureAwait(false);
                                    }

                                }
                                else
                                {
                                    // Bring the existing window to the foreground
                                    GlobalData.webView.WindowState = System.Windows.WindowState.Normal;
                                    GlobalData.webView.Focus();
                                }
                            }
                            else if (msg == "credit_error")
                            {

                                TaskDialog dialog = new TaskDialog("Not sufficient Credits")
                                {
                                    MainInstruction = "Visit our website",
                                    MainContent = modelResponse.response.messageText,
                                    AllowCancellation = true
                                };

                                // Adding a custom button
                                dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Open Website");

                                // Show the dialog and handle the button click
                                TaskDialogResult result = dialog.Show();

                                if (result == TaskDialogResult.CommandLink1)
                                {
                                    Process.Start(new ProcessStartInfo
                                    {
                                        FileName = $"{modelResponse.response.link}",
                                        UseShellExecute = true
                                    });
                                }

                            }
                            else if (msg == "subscription_error")
                            {

                                TaskDialog dialog = new TaskDialog("Please subscribe")
                                {
                                    MainInstruction = "Visit our website",
                                    MainContent = modelResponse.response.messageText,
                                    AllowCancellation = true
                                };

                                // Adding a custom button
                                dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Open Website");

                                // Show the dialog and handle the button click
                                TaskDialogResult result = dialog.Show();

                                if (result == TaskDialogResult.CommandLink1)
                                {
                                    Process.Start(new ProcessStartInfo
                                    {
                                        FileName = $"{modelResponse.response.link}",
                                        UseShellExecute = true
                                    });
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
                    loadingWindow.Close();
                    TaskDialog.Show("TYPUS.AI Exception", "Exception: " + ex.Message);
                }

            }
        }

        public static string ImageDataToJson(string imagePath, Dictionary<string, Color> map, string unmappedImage = "")
        {

            string base64Image = ImageUtils.ConvertImageToBase64(imagePath);
            string base64ImageUnmapped = ImageUtils.ConvertImageToBase64(unmappedImage);

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
                InputImage = base64ImageUnmapped,
                map = materialColorMappings,
                token = TokenManager.ReadToken()
            };



            // Serialize the object to JSON with indented formatting
            string json = string.Empty;
#if !V2021
            // Create JsonSerializerOptions with indented formatting
            var options = new JsonSerializerOptions
            {
                //WriteIndented = true
            };
            json = JsonSerializer.Serialize(dataToSend, options);
#else
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            // Serialize object to JSON string with formatting options
            json = JsonConvert.SerializeObject(dataToSend, settings);
#endif
            // Log the full Base64 string to a text file for inspection
            // * only in dev
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string logFilePath = Path.Combine(desktopPath, "base64_image.json");
            File.WriteAllText(logFilePath, json);

            return json;
        }
    }
}
