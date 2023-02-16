using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Proton;

namespace ProtonT {
    public partial class RegularForm: Form {
        public RegularForm() {
            InitializeComponent();

            this.Load += BorderTestForm_Load;
        }

        private async void BorderTestForm_Load(object? sender, EventArgs e) {
            var WebView = new ProtonWebView(this) { Dock = DockStyle.Fill };
            
            //WebView.AllowResizable = true;

            var environment = await CoreWebView2Environment.CreateAsync(null, "chromium");
            await WebView.EnsureCoreWebView2Async(environment);
            WebView.DefaultBackgroundColor = Color.Transparent;

            //WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebView.CoreWebView2.SetVirtualHostNameToFolderMapping("local-123.com", AppDomain.CurrentDomain.BaseDirectory + "wwwroot", CoreWebView2HostResourceAccessKind.Allow);
            WebView.CoreWebView2.Navigate("http://local-123.com/demo_portal.html");

            this.Controls.Add(WebView);
        }
    }
}
