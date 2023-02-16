using Microsoft.Web.WebView2.Core;
using Proton;

namespace ProtonT {
    public partial class BorderLessProtonForm: ProtonForm {
        public BorderLessProtonForm() {
            InitializeComponent();
            this.Text = "BorderTestForm - Proton";
            this.Load += BorderTestForm_Load;

            this.EnableTransparent = true;
        }

        private async void BorderTestForm_Load(object? sender, EventArgs e) {
            var webView = new ProtonWebView(this) { Dock = DockStyle.Fill };
            webView.AllowResizable = true;

            var environment = await CoreWebView2Environment.CreateAsync(null, "chromium");
            await webView.EnsureCoreWebView2Async(environment);
            webView.DefaultBackgroundColor = Color.Transparent;

            //WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.SetVirtualHostNameToFolderMapping("localhost.pro", AppDomain.CurrentDomain.BaseDirectory + "wwwroot", CoreWebView2HostResourceAccessKind.Allow);
            webView.CoreWebView2.Navigate("http://localhost.pro/demo_portal.html");

            this.Controls.Add(webView);
        }

    }
}