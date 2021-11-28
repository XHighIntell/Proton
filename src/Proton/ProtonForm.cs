﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json.Linq;

using Intell.Win32;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Proton {
    public partial class ProtonForm : Form {


        public ProtonForm() {
            CoreMessageHandler = new CoreMessageHandler(this);
        }


        ///<summary>Gets the instance of WebView2.</summary>
        public WebView2 WebView { get; set; }

        ///<summary>Gets core message handler.</summary>
        public CoreMessageHandler CoreMessageHandler { get; private set; }

        [Category("Proton")]
        [Description("Enable Trasparent of Proton form will prevent all childrens control from drawing.")]
        public bool EnableTransparent { get; set; }

        [Category("Proton")]
        [Description("Allow WebView2 to resize Proton Form from dragging in corner.")]
        public bool AllowResizable { get; set; }


        protected override void OnPaint(PaintEventArgs e) {
            if (this.FormBorderStyle == FormBorderStyle.None && EnableTransparent == true) return;
            base.OnPaint(e);
        }
        protected override void OnPaintBackground(PaintEventArgs e) {
            if (this.FormBorderStyle == FormBorderStyle.None && EnableTransparent == true) return;
            base.OnPaintBackground(e);
        }
        protected unsafe override void OnResize(EventArgs e) {
            if (WebView != null && WebView.CoreWebView2 != null) {
                Messenger.onWindowStateChange(WebView, (int)this.WindowState);
            }


            base.OnResize(e);
        }


        protected unsafe override void WndProc(ref Message m) {

            if (m.Msg == WindowsMessages.WM_CREATE) {
                if (this.FormBorderStyle == FormBorderStyle.None) {
                    var ws = User32.GetWindowLongA(this.Handle, -16);
                    ws |= User32.WindowStyles.WS_THICKFRAME | User32.WindowStyles.WS_SYSMENU;

                    //if (this.MaximizeBox == true) ws |= User32.WindowStyles.WS_MAXIMIZEBOX;
                    //else ws &= ~User32.WindowStyles.WS_MAXIMIZEBOX;
                    //
                    //if (this.MinimizeBox == true) ws |= User32.WindowStyles.WS_MINIMIZEBOX;
                    //else ws &= ~User32.WindowStyles.WS_MINIMIZEBOX;


                    //ws = ws & ~User32.WindowStyles.WS_CLIPCHILDREN;
                    //ws = ws & (~User32.WindowStyles.WS_THICKFRAME);

                    User32.SetWindowLongA(this.Handle, -16, ws);

                    if (EnableTransparent == true) {
                        var ex = User32.GetWindowLongA(this.Handle, -20);
                        User32.SetWindowLongA(this.Handle, -20, ex | User32.WindowStylesExtended.WS_EX_LAYERED);
                        User32.SetLayeredWindowAttributes(this.Handle, 0xFFFFFF, 255, 1);
                    }
                }
            }
            else if (m.Msg == WindowsMessages.WM_NCACTIVATE) {
                if (this.FormBorderStyle == FormBorderStyle.None) {
                    if (m.WParam == IntPtr.Zero) {
                        //m.Result = (IntPtr)0;
                        
                        //return;
                    }
                    
                    m.Result = (IntPtr)1;
                    User32.DefWindowProc(m.HWnd, m.Msg, (IntPtr)1, new IntPtr(-1));
                    return;

                }
            }
            if (m.Msg == WindowsMessages.WM_ERASEBKGND) {

                if (this.FormBorderStyle == FormBorderStyle.None && EnableTransparent == true) {
                    m.Result = new IntPtr(1);
                    return;
                }
            }
            if (m.Msg == WindowsMessages.WM_PAINT) {

                if (this.FormBorderStyle == FormBorderStyle.None && EnableTransparent == true) {
                    m.Result = IntPtr.Zero;
                    return;
                }
            }
            else if (m.Msg == WindowsMessages.WM_NCPAINT) {
                if (this.FormBorderStyle == FormBorderStyle.None && EnableTransparent == true) return;
            }

            if (m.Msg == WindowsMessages.WM_GETMINMAXINFO) {
                if (this.FormBorderStyle == FormBorderStyle.None) {
                    var MINMAXINFO = (User32.MINMAXINFO*)m.LParam;

                    var wa = Screen.PrimaryScreen.WorkingArea;

                    //MINMAXINFO->ptMaxPosition.x = -9;
                    //MINMAXINFO->ptMaxPosition.y = -9;
                    //MINMAXINFO->ptMaxSize.x = 1000;
                    MINMAXINFO->ptMaxSize.x = wa.Width - MINMAXINFO->ptMaxPosition.x * 2;
                    MINMAXINFO->ptMaxSize.y = wa.Height - MINMAXINFO->ptMaxPosition.y * 2;
                }
            }
            else if (m.Msg == WindowsMessages.WM_NCCALCSIZE) {  //WM_NCCALCSIZE

                if (this.FormBorderStyle == FormBorderStyle.None) {
                    if (m.WParam.ToInt32() == 1) {
                        var ws = User32.GetWindowLongA(this.Handle, -16);
                        // 0: the new coordinates of a window
                        // 1: the coordinates of the window before it was moved or resized
                        // 2: the coordinates of the window's client area before the window was moved or resized

                        var rect = (User32.NCCALCSIZE_PARAMS*)m.LParam;


                        if ((ws & 0x01000000) == 0x01000000) {
                            var screen = Screen.FromHandle(this.Handle).WorkingArea;

                            rect->rgrc0.left = 0;
                            rect->rgrc0.top = 0;
                            rect->rgrc0.right = screen.Width;
                            rect->rgrc0.bottom = screen.Height;

                            return;
                        }

                        //rect->rgrc0.left = 90;
                        m.Result = IntPtr.Zero;
                        return;

                    }
                    else {
                        //var rect = (User32.RECT*)m.LParam;
                        //rect->left += border_thickness.left;
                        //rect->right -= border_thickness.right;
                        //rect->bottom -= border_thickness.bottom;
                        //m.Result = IntPtr.Zero;
                        //return;
                    }
                }


            }
            else if (m.Msg == WindowsMessages.WM_NCHITTEST) { 
                if (this.FormBorderStyle == FormBorderStyle.None) {
                    int borderSize = 3;      // Grip size

                    var pos = new Point(m.LParam.ToInt32());
                    pos = this.PointToClient(pos);

                    var x = pos.X;
                    var y = pos.Y;

                    if (x <= borderSize) {
                        if (y <= borderSize) {
                            m.Result = (IntPtr)13; // HTTOPLEFT
                            return;
                        }
                        else if (y <= this.ClientSize.Height - borderSize) {
                            m.Result = (IntPtr)10; // HTLEFT
                            return;
                        }
                        else {
                            m.Result = (IntPtr)16; // HTBOTTOMLEFT
                            return;
                        }
                    }
                    else if (x <= this.ClientSize.Width - borderSize) {
                        if (y <= borderSize) {
                            m.Result = (IntPtr)12;
                            return;
                        }
                        else if (y >= this.ClientSize.Height - borderSize) {
                            m.Result = (IntPtr)15; // HTBOTTOMLEFT
                            return;
                        }

                    }
                    else {
                        if (y <= borderSize) {
                            m.Result = (IntPtr)14;
                            return;
                        }
                        else if (y <= this.ClientSize.Height - borderSize) {
                            m.Result = (IntPtr)11;
                            return;
                        }
                        else {
                            m.Result = (IntPtr)17;
                            return;
                        }
                    }

                    if (CoreMessageHandler.Storage.CaptionRectangle.Contains(x, y) == true) {
                        m.Result = (IntPtr)2;
                        return;
                        
                    };
                }
            }
            base.WndProc(ref m);
        }


    }
}
