using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

    ///<summary>A Message that is sent by Javascript.</summary>
    public class WebView2Message {

        public const string __action = "action";
        public const string __data = "data";
        public const string __callbackId = "id";

        ///<summary>Gets the webview that received the message.</summary>
        public WebView2 WebView { get; private set; }

        ///<summary>Gets the request message.</summary>
        public JObject Message { get; private set; }

        ///<summary>Gets the action of message.</summary>
        public string Action { get; private set; }

        ///<summary>Gets the request message.</summary>
        public JToken Data { get; private set; }

        ///<summary>Gets the callback id of message. Callback id will be null if message is not called by promise.</summary>
        public string CallBackId { get; private set; }

        ///<summary>Gets a value indicating whether the message can respond via SendResponse.</summary>
        public bool IsPromise { get { return CallBackId != null; } }

        ///<summary>Gets value indicating whether <see cref="SendResponse"/> is called.</summary>
        public bool IsSentResponse { get; private set; }

        ///<summary>Initializes a new instance of the <seealso cref="WebView2Message"/>.</summary>
        public WebView2Message(WebView2 webView, string webMessageAsJson) {
            this.WebView = webView;
            this.Message = JObject.Parse(webMessageAsJson);
            this.Action = Message[__action].Value<string>();
            this.Data = Message[__data];
            this.CallBackId = Message[__callbackId]?.Value<string>();
        }

        ///<summary>Send response back to the javascript side. If the message is not a promise, an exception will be thrown.</summary>
        public void SendResponse(JToken data) {
            if (IsPromise == false) throw new Exception("Request is not a promise.");
            if (IsSentResponse == true) throw new Exception("SendReponse can't be called twice in the same message.");

            var message = new JObject() {
                [__action] = "callback",
                [__data] = data,
                [__callbackId] = CallBackId,
            };

            WebView.CoreWebView2.PostWebMessageAsJson(message.ToString(Formatting.None));

            IsSentResponse = true;
        }


    }
}
