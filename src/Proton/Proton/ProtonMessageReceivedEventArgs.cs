using System;
using Microsoft.Web.WebView2.Core;

namespace Proton;

///<summary>Event args for the <see cref="WebView.MessageReceived"/> event.</summary>
public class ProtonMessageReceivedEventArgs: EventArgs {

    public ProtonMessageReceivedEventArgs(ProtonMessage message, CoreWebView2WebMessageReceivedEventArgs raw) {
        Message = message;
        Raw = raw;
    }

    ///<summary>Gets the <see cref="ProtonMessage"/>.</summary>
    public ProtonMessage Message { get; }

    ///<summary>Gets the native event arguments from <seealso cref="CoreWebView2"/>.</summary>
    public CoreWebView2WebMessageReceivedEventArgs Raw { get; }
}