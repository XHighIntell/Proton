using System;
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

            WebView = new WebView(this) { Dock = DockStyle.Fill };
            this.Controls.Add(WebView);

            var environment = await CoreWebView2Environment.CreateAsync(null, "chromium");
            await WebView.EnsureCoreWebView2Async(environment);


            WebView.DefaultBackgroundColor = Color.Transparent;
            //WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebView.CoreWebView2.SetVirtualHostNameToFolderMapping("local-123.com", AppDomain.CurrentDomain.BaseDirectory + "html", CoreWebView2HostResourceAccessKind.Allow);
            WebView.CoreWebView2.Navigate("http://local-123.com/portal.html");
            WebView.MessageReceived += WebView_MessageReceived;
            button1.Click += button1_Click;



        }

        private void WebView_MessageReceived(object sender, MessageReceivedEventArgs e) {
            var x = 123;
            
        }


        private unsafe void button1_Click(object sender, EventArgs e) {
            var settings = WebView.CoreWebView2.Environment.CreatePrintSettings();
            
            
            var task = WebView.CoreWebView2.PrintToPdfAsync(@"E:\My Life\OneDrive\Projects\Intell.Proton\Test\bin\a.pdf", settings);
            task.ContinueWith(ee => {
                var a = ee;

                var x = 123;
                x = x++;
            });
        }
    }
}
