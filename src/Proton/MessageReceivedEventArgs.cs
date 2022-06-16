using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Web.WebView2.Core;

namespace Proton {

    ///<summary> Event args for the <see cref="WebView.MessageReceived"/> event.</summary>
    public class MessageReceivedEventArgs : EventArgs {


        ///<summary>Gets the URI of the document that sent this web message.</summary>
        public WebView2Message Message { get; set; }

        ///<summary>Gets th</summary>
        public CoreWebView2WebMessageReceivedEventArgs Raw { get; set; }
    }


}
