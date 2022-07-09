using System;
using System.IO;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Proton {



    public class WebView : WebView2 {


        public WebView(ProtonForm owner) {

            FormMessageHandler = new FormMessageHandler(owner, this);

            this.WebMessageReceived += WebView_WebMessageReceived;
        }

        

        ///<summary>Gets the default message handler.</summary>
        public FormMessageHandler FormMessageHandler { get; private set; }


        ///<summary>Posts the specified Message to the top level document in this WebView.</summary>
        public void PostMessage(string action, JToken data) {
            if (data == null) {
                CoreWebView2.PostWebMessageAsJson(@"{""" + WebView2Message.__action + @""":""" + JsonConvert.ToString(action) + @"""}");
            }
            else {
                var message = new JObject {
                    [WebView2Message.__action] = action,
                    [WebView2Message.__data] = data
                };

                CoreWebView2.PostWebMessageAsJson(message.ToString(Formatting.None));
            }
        }


        
        // events
        ///<summary>MessageReceived dispatches after web content sends a message to the app host via proton.postMessage.</summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;


        // handle events
        void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e) {
            var message = new WebView2Message(this, e.WebMessageAsJson);

            // 1. process message with FormMessageHandler
            // 2. dispatch MessageReceived events 
            // 3. throw error if promise callback is not call
            //  - exception should not be thrown because users may want async task
            //  - throw in WebView2.WebMessageReceived will be catch by WebView2, so it's pointless

            // --1--
            var isProcessed = FormMessageHandler.ProcessRequest(message);
            if (isProcessed == true) return;

            // --2--
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs() { Message = message, Raw = e });

            // --3--
            // if (message.IsPromise == true && message.IsSentResponse == false) throw new Exception("SendResponse must be call for promise.");            
        }


    }



}
