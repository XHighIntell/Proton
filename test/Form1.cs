using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

using Newtonsoft.Json.Linq;

using Intell.Win32;
using Proton;

namespace Test {
    public partial class Form1 : ProtonForm {

        public Form1() : base() {
            InitializeComponent();
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.EnableTransparent = true;

            Load += Form_Load;
        }

        private async void Form_Load(object sender, EventArgs e) {

            WebView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(WebView);

            var environment = await CoreWebView2Environment.CreateAsync(null, "chromium");
            await WebView.EnsureCoreWebView2Async(environment);
            
            WebView.DefaultBackgroundColor = Color.Transparent;
            //WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebView.CoreWebView2.SetVirtualHostNameToFolderMapping("local-123.com", AppDomain.CurrentDomain.BaseDirectory + "html", CoreWebView2HostResourceAccessKind.Allow);
            WebView.CoreWebView2.Navigate("http://local-123.com/portal.html");

            WebView.WebMessageReceived += WebView_WebMessageReceived;

            button1.Click += button1_Click;



        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e) {
            var message = new ProtonWebViewMessage(WebView, e.WebMessageAsJson);
            var isProcessed = CoreMessageHandler.ProcessRequest(message);


            if (isProcessed == true) return;
        }


        private unsafe void button1_Click(object sender, EventArgs e) {

        }
    }
}
