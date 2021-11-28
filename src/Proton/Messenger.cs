using System;
using System.Drawing;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json.Linq;


namespace Proton {
    public static class Messenger {

        public static void onWindowStateChange(WebView2 webView, int windowState) {

            //var script = "proton.browserWindow.onmessage({ action: 'browserWindow.onWindowStateChange', windowState: " + windowState + "})";

            //webView.CoreWebView2.ExecuteScriptAsync(script);

            webView.CoreWebView2.PostWebMessageAsJson(@"{ ""action"": ""browserWindow.onWindowStateChange"", ""windowState"": " + windowState + "}");
        }
    }
}
