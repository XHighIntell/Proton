using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace Proton;

///<summary>Implements generic messages that are sent from <see cref="ProtonWebView"/>.</summary>
public class ProtonMessage {
    public const string ACTION_KEY_NAME = "action";
    public const string DATA_KEY_NAME = "data";
    public const string CALL_BACK_ID_KEY_NAME = "id";


    public ProtonMessage(ProtonWebView webView, string webMessageAsJson) {
        this.WebView = webView;
        this.Node = JsonNode.Parse(webMessageAsJson) as JsonObject ?? throw new Exception("Why null?");
        this.Action = Node[ACTION_KEY_NAME]?.GetValue<string>() ?? throw new Exception("ProtonMessage.Action can't be null.");
        this.Data = Node[DATA_KEY_NAME];
        this.CallBackId = Node[CALL_BACK_ID_KEY_NAME]?.GetValue<string>();
    }

    #region properties
    ///<summary>Gets the <see cref="ProtonWebView"/> instance that received the message.</summary>
    public ProtonWebView WebView { get; private set; }

    ///<summary>Gets the raw message as a <see cref="JsonObject"/>.</summary>
    public JsonObject Node { get; private set; }

    ///<summary>Gets the action of the message.</summary>
    public string Action { get; private set; }

    ///<summary>Gets the data of the message.</summary>
    public JsonNode? Data { get; private set; }

    ///<summary>Gets the id of the message. If id exists, it means the message supports send back response.</summary>
    private string? CallBackId { get; set; }

    ///<summary>Indicates whether a response is mandatory.</summary>
    public bool ResponseRequired { get { return CallBackId != null; } }

    ///<summary>Returns true if a response was sent.</summary>
    public bool ResponseSent { get; private set; }
    #endregion

    ///<summary>Send the response back to the JavaScript side.</summary>
    public void SendResponse(JsonNode? data) {
        if (ResponseRequired == false) throw new Exception("This message does not require a response.");
        if (ResponseSent == true) throw new Exception("SendResponse cannot be called more than once.");

        var message = new JsonObject() {
            [ACTION_KEY_NAME] = "callback",
            [DATA_KEY_NAME] = data,
            [CALL_BACK_ID_KEY_NAME] = CallBackId,
        };

        WebView.CoreWebView2.PostWebMessageAsJson(message.ToString());
        ResponseSent = true;
    }

    ///<summary>Sends the exception response back to the JavaScript side.</summary>
    public void SendException(Exception exception) {
        if (ResponseRequired == false) throw new Exception("This message does not require a response.");
        if (ResponseSent == true) throw new Exception("SendResponse cannot be called more than once.");

        var message = new JsonObject() {
            [ACTION_KEY_NAME] = "callback_exception",
            [DATA_KEY_NAME] = ExceptionToNode(exception),
            [CALL_BACK_ID_KEY_NAME] = CallBackId,
        };

        WebView.CoreWebView2.PostWebMessageAsJson(message.ToString());
        ResponseSent = true;
    }


    public static JsonNode ExceptionToNode(Exception exception) {
        if (exception is AggregateException aggregateException) {
            return new JsonObject() {
                ["type"] = exception.GetType().Name,
                ["message"] = exception.Message,
                ["stack"] = exception.StackTrace,
                ["errors"] = new JsonArray(aggregateException.InnerExceptions.Select(ex => ExceptionToNode(ex)).ToArray()),
            };
        }
        else {
            return new JsonObject() {
                ["type"] = exception.GetType().Name,
                ["message"] = exception.Message,
                ["stack"] = exception.StackTrace,
            };
        }
           
    }

}
