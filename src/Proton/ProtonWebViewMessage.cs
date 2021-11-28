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
using Newtonsoft.Json;


namespace Proton {
    public class ProtonWebViewMessage {

        ///<summary>Gets the webview that received the message.</summary>
        public WebView2 WebView { get; private set; }

        ///<summary>Gets the request message.</summary>
        public JObject Request { get; private set; }

        ///<summary>Gets the action of message.</summary>
        public string Action { get; private set; }

        ///<summary>Gets the callback id of message. Callback id will be null if message is not called by promise.</summary>
        public string CallBackId { get; private set; }

        ///<summary>Gets a value indicating whether the message can respond via SendResponse.</summary>
        public bool IsPromise { get { return CallBackId != null; } }


        public ProtonWebViewMessage(WebView2 webView, string webMessageAsJson) {
            this.WebView = webView;
            this.Request = JObject.Parse(webMessageAsJson);
            this.Action = Request["action"].Value<string>();
            this.CallBackId = Request["__callback"]?.Value<string>();
        }


        public void SendResponse(JToken data) {
            if (IsPromise == false) throw new Exception("Request is not a promise.");

            var message = new JObject() {
                ["action"] = "callback",
                ["id"] = CallBackId,
                ["data"] = data,
            };

            WebView.CoreWebView2.PostWebMessageAsJson(message.ToString(Formatting.None));
            
        }


    }
}
