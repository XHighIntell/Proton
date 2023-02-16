using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace Proton {
    ///<summary>Implements generic messages that are sent from <see cref="ProtonWebView"/>.</summary>
    public class ProtonMessage {
        public const string __action = "action";
        public const string __data = "data";
        public const string __callbackId = "id";

        bool _sentResponse = false;


        public ProtonMessage(ProtonWebView webView, string webMessageAsJson) {
            this.WebView = webView;
            this.Message = JObject.Parse(webMessageAsJson);
            this.Action = Message[__action].Value<string>();
            this.Data = Message[__data];
            this.CallBackId = Message[__callbackId]?.Value<string>();
        }

        #region properties
        ///<summary>Gets the Webview2 that received the message.</summary>
        public ProtonWebView WebView { get; private set; }

        ///<summary>Gets the request message.</summary>
        public JObject Message { get; private set; }

        ///<summary>Gets the action of the message.</summary>
        public string Action { get; private set; }

        ///<summary>Gets the data of the message.</summary>
        public JToken Data { get; private set; }

        ///<summary>Gets a value indicating whether the message supports send back response.</summary>
        public bool IsPromise { get { return CallBackId != null; } }

        ///<summary>Gets the id of the message. If id exists, it means the message supports send back response.</summary>
        private string CallBackId { get; set; }
        #endregion

        ///<summary>Send back response to JavaScript side.</summary>
        public void SendResponse(JToken data) {
            if (IsPromise == false) throw new Exception("This message is not a promise.");
            if (_sentResponse == true) throw new Exception("SendResponse cannot be called more than 1 time");

            _sentResponse = true;
            var message = new JObject() {
                [__action] = "callback",
                [__data] = data,
                [__callbackId] = CallBackId,
            };

            WebView.CoreWebView2.PostWebMessageAsJson(message.ToString(Formatting.None));
        }

    }
}
