using System;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

using Intell.Win32;

namespace Proton {

    ///<summary>Represents a handler to process webview messages for window.</summary>
    public class FormMessageHandler: IDisposable {

        ///<summary>Initializes a new instance of the <seealso cref="FormMessageHandler"/> class.</summary>
        public FormMessageHandler(ProtonForm owner, WebView webView) {

            this.Owner = owner;
            this.WebView2 = webView;

            _previousWindowState = this.Owner.WindowState;

            RegisterEvents();
        }


        // private field
        bool _disposed = false;
        FormWindowState _previousWindowState;

        // properties
        public ProtonForm Owner { get; private set; }
        
        public WebView WebView2 { get; private set; }
        
        //public DefaultMessageHandlerStorage Storage { get; private set; }
        
        public Rectangle CaptionRectangle { get; set; }

        // methods
        public bool ProcessRequest(WebView2Message message) {
            var action = message.Action;
            var o = message.Data;

            if (action == "window.getAll") {
                message.SendResponse(new JObject {
                    ["text"] = Owner.Text,
                    ["windowState"] = (int)Owner.WindowState,
                    ["borderStyle"] = (int)Owner.FormBorderStyle,
                    ["allowResizable"] = Owner.AllowResizable,
                });
            }
            else if (action == "window.setText") Owner.Text = o.Value<string>();
            else if (action == "window.setWindowState") Owner.WindowState = (FormWindowState)o.Value<int>();
            else if (action == "window.setBorderStyle") Owner.FormBorderStyle = (FormBorderStyle)o.Value<int>();
            else if (action == "window.setAllowResizable") Owner.AllowResizable = o.Value<bool>();
            else if (action == "window.startDrag") {
                User32.ReleaseCapture();
                User32.SendMessageA(Owner.Handle, WindowsMessages.WM_NCLBUTTONDOWN, User32.NCHitTest.HTCAPTION, 0);
            }
            else if (action == "window.startResize") {
                var hittest = o.Value<int>();
                User32.ReleaseCapture();
                User32.SendMessageA(Owner.Handle, WindowsMessages.WM_NCLBUTTONDOWN, hittest, 0);
            }
            else if (action == "window.setCaptionRectangle") {
                var x = o.Value<int>("x");
                var y = o.Value<int>("y");
                var width = o.Value<int>("width");
                var height = o.Value<int>("height");

                CaptionRectangle = new Rectangle(x, y, width, height);
            }
            else if (action == "window.openContextMenu") {
                User32.GetCursorPos(out Point point);
                IntPtr hMenu = User32.GetSystemMenu(Owner.Handle, false);
                int cmd = User32.TrackPopupMenu(hMenu, 0x100, point.X, point.Y, 0, Owner.Handle, IntPtr.Zero);
                if (cmd > 0) User32.SendMessageA(Owner.Handle, WindowsMessages.WM_SYSCOMMAND, cmd, 0);

                return true;
            }


            else return false;

            return true;
            

        }
        public void Dispose() {
            if (_disposed == true) return;

            UnRegisterEvents();

            _disposed = true;
        }

        // private methods
        void RegisterEvents() {
            Owner.Resize += Resize;
        }
        void UnRegisterEvents() {
            Owner.Resize -= Resize;
        }

        void Resize(object sender, EventArgs e) {

            if (_previousWindowState != Owner.WindowState) {

                _previousWindowState = Owner.WindowState;

                WebView2.PostMessage("window.onWindowStateChange", new JObject() {
                    ["windowState"] = (int)Owner.WindowState
                });
            }
            
        }

    }


}





