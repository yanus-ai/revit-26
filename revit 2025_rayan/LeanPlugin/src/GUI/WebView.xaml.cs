using YANUSConnector;
using YANUSConnector.Http;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YANUSConnector.Adapter;
using System.Security.Policy;

namespace YANUSConnector.GUI
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
        private async Task InitializeWebView()
        {
            try
            {
                var env = await CoreWebView2Environment.CreateAsync(null, "C:/YanusConnectorWebView");
                await webView.EnsureCoreWebView2Async(env);
                webView.CoreWebView2.SourceChanged += srcChanged!;
                //webView.Source = "https://app.yanus.ai/version-test/signup?m=revitlogin";
            }
            catch (Exception e)
            {
                MessageBox.Show(" Error in Web View " + e.Message, "Application");
            }
        }
        private void srcChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            GlobalData.websrc = webView.CoreWebView2.Source;
            string? jwt = JwtExtractor.ExtractJwt(GlobalData.websrc);
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
