using System;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

using Intell.Win32;

namespace Proton {

    ///<summary>Represents a default message proccess handler.</summary>
    public class CoreMessageHandler {

        ///<summary>Initializes a new instance of the <seealso cref="CoreMessageHandler"/> class.</summary>
        public CoreMessageHandler(ProtonForm owner) {
            this.Owner = owner;
            this.Storage = new CoreMessageStorage();
        }

        public ProtonForm Owner { get; private set; }

        public CoreMessageStorage Storage { get; private set; }

        public bool ProcessRequest(ProtonWebViewMessage message) {
            var action = message.Action;
            var o = message.Request;

            if (action == "browserWindow.getAll") {
                var data = new JObject {
                    ["windowState"] = (int)Owner.WindowState,
                    ["borderStyle"] = (int)Owner.FormBorderStyle,
                    ["allowResizable"] = Owner.AllowResizable,
                };


                message.SendResponse(data);

                return true;
            }
            else if (action == "browserWindow.drag") {
                User32.ReleaseCapture();
                User32.SendMessageA(Owner.Handle, WindowsMessages.WM_NCLBUTTONDOWN, User32.NCHitTest.HTCAPTION, 0);

                return true;
            }
            else if (action == "browserWindow.startResize") {
                var hittest = o["hit"].Value<int>();
                User32.ReleaseCapture();
                User32.SendMessageA(Owner.Handle, WindowsMessages.WM_NCLBUTTONDOWN, hittest, 0);
                return true;
            }
            else if (action == "browserWindow.setCaptionRectangle") {
                var rect = o["rect"];
                var x = rect.Value<int>("x");
                var y = rect.Value<int>("y");
                var width = rect.Value<int>("width");
                var height = rect.Value<int>("height");

                Storage.CaptionRectangle = new Rectangle(x, y, width, height);
                return true;
            }
            else if (action == "browserWindow.openContextMenu") {
                User32.GetCursorPos(out Point point);
                IntPtr hMenu = User32.GetSystemMenu(Owner.Handle, false);
                int cmd = User32.TrackPopupMenu(hMenu, 0x100, point.X, point.Y, 0, Owner.Handle, IntPtr.Zero);
                if (cmd > 0) User32.SendMessageA(Owner.Handle, WindowsMessages.WM_SYSCOMMAND, cmd, 0);

                return true;
            }
            else if (action == "browserWindow.setWindowState") {
                var value = (FormWindowState)o["value"].Value<int>();

                if (value == FormWindowState.Maximized && Owner.MaximizeBox == false) return true;
                
                if (Owner.WindowState != value) Owner.WindowState = value;

                return true;
            }
            else if (action == "browserWindow.releaseCapture") {
                User32.ReleaseCapture();
                return true;
            }
            else return false;


            

        }
    }


}





