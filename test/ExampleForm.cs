using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Proton;

namespace Example {
    public partial class ExampleForm : ProtonForm {
        public ExampleForm() {
            InitializeComponent();

            this.Load += (sender, e) => {
                CreateWebView();
            };
            
            //this.WindowState = 0;
            //this.FormBorderStyle = FormBorderStyle.Fixed3D;


        }

        async void CreateWebView() {
            WebView = new WebView(this) { Dock = DockStyle.Fill };
            this.Controls.Add(WebView);

            var lastestBrowserExecutableFolder = WebView2Helper.GetLatestBrowserExecutableFolder();
            var environment = await CoreWebView2Environment.CreateAsync(lastestBrowserExecutableFolder, "chromium");
            await WebView.EnsureCoreWebView2Async(environment);


            WebView.DefaultBackgroundColor = Color.Transparent;
            //WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebView.CoreWebView2.SetVirtualHostNameToFolderMapping("local-123.com", AppDomain.CurrentDomain.BaseDirectory + "html", CoreWebView2HostResourceAccessKind.Allow);
            WebView.CoreWebView2.Navigate("http://local-123.com/portal.html");


            WebView.MessageReceived += WebView_MessageReceived;
        }

        private void WebView_MessageReceived(object sender, MessageReceivedEventArgs e) {


            //WebView.PostMessage("a", new JObject() { ["x"] = 123 });
            if (e.Message.Action == "Hello") {
                e.Message.SendResponse(new JObject() { ["message"] = "Response from C#" });
            }
            
            

            
        }


    }



}
