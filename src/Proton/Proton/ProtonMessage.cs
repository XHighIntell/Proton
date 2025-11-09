using System;
using System.Text.Json.Nodes;

namespace Proton;

///<summary>Implements generic messages that are sent from <see cref="ProtonWebView"/>.</summary>
public class ProtonMessage {
    public const string ACTION_KEY_NAME = "action";
    public const string DATA_KEY_NAME = "data";
    public const string CALL_BACK_ID_KEY_NAME = "id";

    bool _responseSent = false;


    public ProtonMessage(ProtonWebView webView, string webMessageAsJson) {
        this.WebView = webView;
        this.Message = JsonNode.Parse(webMessageAsJson) as JsonObject ?? throw new Exception("Why null?");
        this.Action = Message[ACTION_KEY_NAME]?.GetValue<string>() ?? throw new Exception("ProtonMessage.Action can't be null.");
        this.Data = Message[DATA_KEY_NAME];
        this.CallBackId = Message[CALL_BACK_ID_KEY_NAME]?.GetValue<string>();
    }

    #region properties
    ///<summary>Gets the Webview2 that received the message.</summary>
    public ProtonWebView WebView { get; private set; }

    ///<summary>Gets the request message.</summary>
    public JsonObject Message { get; private set; }

    ///<summary>Gets the action of the message.</summary>
    public string Action { get; private set; }

    ///<summary>Gets the data of the message.</summary>
    public JsonNode? Data { get; private set; }

    ///<summary>Gets the id of the message. If id exists, it means the message supports send back response.</summary>
    private string? CallBackId { get; set; }

    ///<summary>Indicates whether a response is mandatory.</summary>
    public bool ResponseRequired { get { return CallBackId != null; } }
    #endregion

    ///<summary>Send back response to JavaScript side.</summary>
    public void SendResponse(JsonNode data) {
        if (ResponseRequired == false) throw new Exception("This message is not a promise.");
        if (_responseSent == true) throw new Exception("SendResponse cannot be called more than 1 time");

        _responseSent = true;
        var message = new JsonObject() {
            [ACTION_KEY_NAME] = "callback",
            [DATA_KEY_NAME] = data,
            [CALL_BACK_ID_KEY_NAME] = CallBackId,
        };

        WebView.CoreWebView2.PostWebMessageAsJson(message.ToString());
    }

}
