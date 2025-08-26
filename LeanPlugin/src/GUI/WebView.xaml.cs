using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using YANUS_Connector.Adapter;
using YANUS_Connector.Http;

namespace YANUS_Connector.GUI
{
    /// <summary>
    /// Interaction logic for WebView.xaml
    /// </summary>
    public partial class WebView : Window
    {
        public WebView()
        {
            InitializeComponent();
            Task task = InitializeWebView();
        }

        private CoreWebView2Environment _environment;

        private async Task InitializeWebViewAsync()
        {
            if (_environment == null)
            {
                _environment = await CoreWebView2Environment.CreateAsync();
            }

            await webView.EnsureCoreWebView2Async(_environment);
        }
        private async Task InitializeWebView()
        {
            try
            {
                //       if (webView?.CoreWebView2 != null)
                //       {
                //           webView.CoreWebView2.SourceChanged -= SrcChanged;
                //           webView.Dispose();
                //       }

                //       webView = new WebView2();
                //       var env = await CoreWebView2Environment.CreateAsync();


                //await webView.EnsureCoreWebView2Async(env);
                //       webView.CoreWebView2.SourceChanged += SrcChanged;


                // Re-add the SourceChanged event handler
                if (webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.SourceChanged -= SrcChanged;
                    webView.Dispose();
                    webView = new Microsoft.Web.WebView2.Wpf.WebView2();


                    // Re-add the SourceChanged event handler
                    webView.CoreWebView2.SourceChanged += SrcChanged;

                }

                //await  InitializeWebViewAsync();
                var userDataFolder = "C:\\TYPUSConnectorWebView";
                if (!Directory.Exists(userDataFolder))
                {
                    Directory.CreateDirectory(userDataFolder);
                }

                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
                //var env = await CoreWebView2Environment.CreateAsync(null, null);

                await webView.EnsureCoreWebView2Async(env);

                //#if (!V2023)
                //                {
                //                }
                //#else
                //                { 
                //                    await webView.EnsureCoreWebView2Async();

                //                }
                //#endif
                webView.CoreWebView2.SourceChanged += SrcChanged;

                if (!GlobalData.isLogin)
                {
                    //old
                    ////webView.Source = new Uri("https://app.yanus.ai/version-test/signup?m=revitlogin");
                    //webView.Source = new Uri("https://app.yanus.ai/signup?m=revitlogin");

                    //new production
                    //webView.Source = new Uri("https://app.yanus.ai/auth?m=plugin");
                    webView.Source = new Uri("https://app.typus.ai/login");
                    //new development
                    //webView.Source = new Uri("https://app.yanus.ai/version-test/auth?m=plugin");

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in Web View: " + e.Message, "Application");
            }
        }
        private void SrcChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            GlobalData.websrc = webView.CoreWebView2.Source;
            string jwt = JwtExtractor.ExtractJwt(GlobalData.websrc);
            if (jwt != null)
            {
                this.Close();
                //GlobalData.jwt = jwt;
                TokenManager.WriteToken(jwt);
                //MessageBox.Show(GlobalData.jwt);
                RevitAdapter.ShowAppButtons();


            }
        }
        public async Task ChangeSrc(string src)
        {
            try
            {
                webView.Source = new Uri(src);
            }
            catch (Exception e)
            {
                MessageBox.Show(" Error in Web View " + e.Message, "Application");
            }
        }
    }
}
