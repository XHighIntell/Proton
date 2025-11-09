using System;
using Microsoft.Web.WebView2.Core;

namespace Proton;

///<summary>Event args for the <see cref="WebView.MessageReceived"/> event.</summary>
public class ProtonMessageReceivedEventArgs: EventArgs {
    ///<summary>Gets the <see cref="ProtonMessage"/>.</summary>
    public ProtonMessage Message { get; set; }

    ///<summary>Gets the raw event of <seealso cref="CoreWebView2"/>.</summary>
    public CoreWebView2WebMessageReceivedEventArgs Raw { get; set; }
}