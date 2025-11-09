using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

using Intell.Win32;
using System.Text.Json.Nodes;

namespace Proton;


public class ProtonWebView: WebView2 {

    private readonly Form _ownerForm;
    private Rectangle _captionRectangle = Rectangle.Empty;
    private bool _allowResizable = false;

    ///<summary>
    /// Create a new <see cref="ProtonWebView"/> WinForms control. After construction the <see cref="ProtonWebView.CoreWebView2"/>
    ///     property is null. Call <see cref="ProtonWebView"/>.EnsureCoreWebView2Async(<see cref="CoreWebView2Environment"/>, <see cref="CoreWebView2ControllerOptions"/>)
    ///     to initialize the underlying <see cref="CoreWebView2"/>.
    ///</summary>
    public ProtonWebView(Form form) {
        _ownerForm = form;

        if (_ownerForm is ProtonForm protonForm) {
            protonForm._webViews.Add(this);
        }

        WebMessageReceived += (sender, e) => { onWebMessageReceived(e); };
        NavigationCompleted += (sender, e) => { onNavigationCompleted(); };
    }

    
    ///<summary>Occurs when a <see cref="ProtonMessage"/> is received from.</summary>
    public event EventHandler<ProtonMessageReceivedEventArgs>? ProtonMessageReceived;

    #region properties
    public Form Form {
        get { return _ownerForm; }
    }
    public Rectangle TitleBarRectangle { 
        get { return _captionRectangle; } 
    }

    ///<summary>Allow the parent <see cref="Form"/> to resize  on corner drag of the <see cref="ProtonWebView"/>.</summary>
    [Category("Proton")]
    [Description("Allow WebView2 to resize Form from dragging in corner.")]
    public bool AllowResizable {
        get { return _allowResizable; }
        set { _allowResizable = value; }
    }
    #endregion

    #region Methods

    ///<summary>Send a message using Proton. On the JavaScript side, use 'proton.onMessage.addListener' to handle incoming messages.</summary>
    ///<param name="action">The name of the message.</param>
    public void PostProtonMessage(string action) {
        var message = new JsonObject() { [ProtonMessage.ACTION_KEY_NAME] = action };
        this.CoreWebView2.PostWebMessageAsJson(message.ToJsonString());
    }

    ///<summary>Send a message using Proton. On the JavaScript side, use 'proton.onMessage.addListener' to handle incoming messages.</summary>
    ///<param name="action">The name of the message.</param>
    ///<param name="data">The additional data of the message.</param>
    public void PostProtonMessage(string action, JsonNode? data) {
        var message = new JsonObject() {
            [ProtonMessage.ACTION_KEY_NAME] = action,
            [ProtonMessage.DATA_KEY_NAME] = data,
        };
        this.CoreWebView2.PostWebMessageAsJson(message.ToJsonString());
    }
    #endregion

    void onNavigationCompleted() {
        PostProtonMessage("proton.init", 2);
    }
    void onWebMessageReceived(CoreWebView2WebMessageReceivedEventArgs e) {
        // 1. process internal message
        // 2. do not invoke events if the message is an internal message

        // --1--
        var message = new ProtonMessage(this, e.WebMessageAsJson);
        if (processFormMessage(message) == true) return;
        
        ProtonMessageReceived?.Invoke(this, new ProtonMessageReceivedEventArgs() {
            Message = message,
            Raw = e,
        });
    }
    bool processFormMessage(ProtonMessage message) {
        var action = message.Action;
        var data = message.Data;

        if (_ownerForm == null) return false;

        if (action == "window.getAll") {
            message.SendResponse(new JsonObject {
                ["text"] = _ownerForm.Text,
                ["windowState"] = (int)_ownerForm.WindowState,
                ["borderStyle"] = (int)_ownerForm.FormBorderStyle,
                ["allowResizable"] = _allowResizable,
            });
            return true;
        }
        else if (action == "window.setText") { 
            _ownerForm.Text = data.GetValue<string>();
            return true;
        }
        else if (action == "window.setWindowState") {
            _ownerForm.WindowState = (FormWindowState)data.GetValue<int>();
            return true;
        }
        else if (action == "window.setBorderStyle") {
            _ownerForm.FormBorderStyle = (FormBorderStyle)data.GetValue<int>();
            return true;
        }
        else if (action == "window.setAllowResizable") {
            _allowResizable = data.GetValue<bool>();
            return true;
        }
        else if (action == "window.startDrag") {
            User32.ReleaseCapture();
            User32.SendMessageA(_ownerForm.Handle, WindowsMessages.WM_NCLBUTTONDOWN, User32.NCHitTest.HTCAPTION, 0);
            return true;
        }
        else if (action == "window.startResize") {
            var hittest = data.GetValue<int>();
            User32.ReleaseCapture();
            User32.SendMessageA(_ownerForm.Handle, WindowsMessages.WM_NCLBUTTONDOWN, hittest, 0);
            return true;
        }
        else if (action == "window.setCaptionRectangle") {
            var x = data["x"].GetValue<int>();
            var y = data["y"].GetValue<int>();
            var width = data["width"].GetValue<int>();
            var height = data["height"].GetValue<int>();

            _captionRectangle = new Rectangle(x, y, width, height);
            return true;
        }
        else if (action == "window.openContextMenu") {
            User32.GetCursorPos(out Point point);
            IntPtr hMenu = User32.GetSystemMenu(_ownerForm.Handle, false);
            int cmd = User32.TrackPopupMenu(hMenu, 0x100, point.X, point.Y, 0, _ownerForm.Handle, IntPtr.Zero);
            if (cmd > 0) User32.SendMessageA(_ownerForm.Handle, WindowsMessages.WM_SYSCOMMAND, cmd, 0);

            return true;
        }
        

        return false;
    }

}
