using Microsoft.Web.WebView2.Core;
using Proton;

namespace QuickTemplate; 

public partial class Form1: ProtonForm {
    public Form1() {
        InitializeComponent();

        this.Text = "BorderTestForm - Proton";
        this.Load += BorderTestForm_Load;

        // Enable Trasparent of Proton form will prevent all childrens control from drawing.
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
        webView.CoreWebView2.Navigate("http://localhost.pro/demo.html");

        this.Controls.Add(webView);
    }

}
