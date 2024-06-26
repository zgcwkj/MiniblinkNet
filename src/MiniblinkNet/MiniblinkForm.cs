﻿using MiniblinkNet.MiniBlink;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniblinkNet
{
    public partial class MiniblinkForm : Form, IMessageFilter
    {
        /// <summary>
        /// 是否透明模式
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsTransparent { get; }

        /// <summary>
        /// 是否允许在无边框模式下调整窗体大小
        /// </summary>
        public bool NoneBorderResize { get; set; }

        /// <summary>
        /// 窗体边框和颜色
        /// </summary>
		public FormBorder Border { get; private set; }

        /// <summary>
        /// 窗体阴影长度
        /// </summary>
		public FormShadowWidth ShadowWidth { get; private set; }

        /// <summary>
        /// 调整大小的触发范围
        /// </summary>
        public FormResizeWidth ResizeWidth { get; }

        public MiniblinkBrowser View { get; }
        private ResizeDirect _direct;
        private bool _isdrop = false;
        private string _guid { get; } = Guid.NewGuid().ToString().Replace("-", "");
        private string _dragfunc => "drag" + _guid;
        private string _maxfunc => "max" + _guid;
        private string _minfunc => "min" + _guid;
        private string _closefunc => "close" + _guid;

        public MiniblinkForm() : this(false)
        {
        }

        public MiniblinkForm(bool isTransparent)
        {
            Application.AddMessageFilter(this);
            Border = new FormBorder();
            ShadowWidth = new FormShadowWidth();
            //透明模式
            IsTransparent = isTransparent;
            if (IsTransparent)
            {
                NoneBorderResize = true;
                FormBorderStyle = FormBorderStyle.None;
                View.PaintUpdated += FormPaint;
            }
            //调整大小
            Controls.Add(View = new MiniblinkBrowser
            {
                Dock = DockStyle.Fill
            });
            ResizeWidth = new FormResizeWidth(5);
            //注册基础事件
            View.BindNetFunc(new NetFunc(_dragfunc, DropStart));
            View.BindNetFunc(new NetFunc(_maxfunc, MaxFunc));
            View.BindNetFunc(new NetFunc(_minfunc, MinFunc));
            View.BindNetFunc(new NetFunc(_closefunc, CloseFunc));
            //注册脚本事件
            View.DocumentReady += RegisterJsEvent;
            View.RegisterJsFunc(this);
            //加载完成事件
            Load += new EventHandler(this.FormLoad);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// 加载完成时
        /// </summary>
        private void FormLoad(object sender, EventArgs e)
        {
            Shown += (ls, le) =>
            {
                SetFormStartPos();
                DrawShadow();
            };
            //焦点事件
            Activated += FormActivated;
            Deactivate += FormDeactivate;
            //调整大小事件
            SizeChanged += FormSizeChanged;
            //透明模式
            if (IsTransparent)
            {
                SetTransparent();
                TransparentPaint(Width, Height, MBApi.wkeGetViewDC(View.MiniblinkHandle));
            }
        }

        /// <summary>
        /// 设置透明
        /// </summary>
        private void SetTransparent()
        {
            var style = WinApi.GetWindowLong(Handle, (int)WinConst.GWL_EXSTYLE);
            if ((style & (int)WinConst.WS_EX_LAYERED) != (int)WinConst.WS_EX_LAYERED)
            {
                WinApi.SetWindowLong(Handle, (int)WinConst.GWL_EXSTYLE, style | (int)WinConst.WS_EX_LAYERED);
            }
            MBApi.wkeSetTransparent(View.MiniblinkHandle, true);
        }

        /// <summary>
        /// 设置 Form 起点坐标
        /// </summary>
        private void SetFormStartPos()
        {
            switch (StartPosition)
            {
                case FormStartPosition.CenterScreen:
                    Left = Screen.PrimaryScreen.WorkingArea.Width / 2 - Width / 2;
                    Top = Screen.PrimaryScreen.WorkingArea.Height / 2 - Height / 2;
                    break;

                case FormStartPosition.CenterParent:
                    if (ParentForm != null)
                    {
                        Left = ParentForm.Left + ParentForm.Width / 2 - Width / 2;
                        Top = ParentForm.Top + ParentForm.Height / 2 - Height / 2;
                    }
                    break;
            }
        }

        /// <summary>
        /// 绘制阴影
        /// </summary>
        private void DrawShadow()
        {
            if (ShadowWidth == null)
            {
                return;
            }

            if (ShadowWidth.Bottom + ShadowWidth.Left + ShadowWidth.Right + ShadowWidth.Top == 0)
            {
                return;
            }

            var v = 2;
            WinApi.DwmSetWindowAttribute(Handle, 2, ref v, 4);
            var margins = new MARGINS
            {
                top = ShadowWidth.Top,
                left = ShadowWidth.Left,
                right = ShadowWidth.Right,
                bottom = ShadowWidth.Bottom
            };
            WinApi.DwmExtendFrameIntoClientArea(Handle, ref margins);
        }

        /// <summary>
        /// 获得焦点
        /// </summary>
        public virtual void FormActivated(object sender, EventArgs e)
        {
            MBApi.wkeRunJSW(View.MiniblinkHandle, "window.activated!=undefined?window.activated():'unbound event';");
        }

        /// <summary>
        /// 失去焦点
        /// </summary>
        public virtual void FormDeactivate(object sender, EventArgs e)
        {
            MBApi.wkeRunJSW(View.MiniblinkHandle, "window.deactivate!=undefined?window.deactivate():'unbound event';");
        }

        /// <summary>
        /// 调整窗体大小
        /// </summary>
        public virtual void FormSizeChanged(object sender, EventArgs e)
        {
            //输出宽高
            MBApi.wkeRunJSW(View.MiniblinkHandle, $"window.sizechanged!=undefined?window.sizechanged({Width},{Height}):'unbound event';");
            //页面图标
            MBApi.wkeRunJSW(View.MiniblinkHandle, $"window.statechanged!=undefined?window.statechanged({(int)WindowState}):'unbound event';");
        }

        /// <summary>
        /// 最大化和还原
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private object MaxFunc(NetFuncContext context)
        {
            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            return null;
        }

        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private object MinFunc(NetFuncContext context)
        {
            WindowState = FormWindowState.Minimized;
            return null;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private object CloseFunc(NetFuncContext context)
        {
            Close();
            return null;
        }

        /// <summary>
        /// 拖动窗体和禁用拖动窗体
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private object DropStart(NetFuncContext context)
        {
            if (_isdrop == false && WindowState != FormWindowState.Maximized && MouseButtons == MouseButtons.Left)
            {
                _isdrop = true;
                var dropPos = MousePosition;
                var dropLoc = Location;
                var me = View.MouseEnabled;
                View.MouseEnabled = false;
                WatchMouse(p =>
                {
                    var nx = p.X - dropPos.X;
                    var ny = p.Y - dropPos.Y;
                    nx = dropLoc.X + nx;
                    ny = dropLoc.Y + ny;
                    Invoke(new Action(() =>
                    {
                        Location = new Point(nx, ny);
                        Cursor = Cursors.SizeAll;
                    }));
                }, () =>
                {
                    Invoke(new Action(() =>
                    {
                        ResetCursor();
                        View.MouseEnabled = me;
                    }));
                    _isdrop = false;
                });
                Cursor = Cursors.SizeAll;
            }

            return null;
        }

        private void FormPaint(object sender, PaintUpdatedEventArgs e)
        {
            if (!IsDisposed)
            {
                IntPtr dc;
                if (View.BmpPaintMode)
                {
                    dc = MBApi.wkeGetViewDC(View.MiniblinkHandle);
                }
                else
                {
                    dc = e.Param;
                }

                TransparentPaint(e.Width, e.Height, dc);
                e.Cancel = true;
            }
        }

        private void TransparentPaint(int width, int height, IntPtr memDc)
        {
            var dst = new WinPoint { x = Left, y = Top };
            var src = new WinPoint();
            var size = new WinSize(width, height);

            var blend = new BlendFunction
            {
                BlendOp = (byte)WinConst.AC_SRC_OVER,
                SourceConstantAlpha = 255,
                AlphaFormat = (byte)WinConst.AC_SRC_ALPHA
            };
            WinApi.UpdateLayeredWindow(Handle, IntPtr.Zero, ref dst, ref size, memDc, ref src, 0, ref blend,
                (int)WinConst.ULW_ALPHA);
        }

        /// <summary>
        /// 注册脚本事件
        /// </summary>
        private void RegisterJsEvent(object sender, DocumentReadyEventArgs e)
        {
            var map = new Dictionary<string, string>
            {
                {"maxName", _maxfunc},
                {"minName", _minfunc},
                {"closeName", _closefunc},
                {"dragName", _dragfunc},
            };
            var vars = string.Join(";", map.Keys.Select(k => $"var {k}='{map[k]}'")) + ";";
            var js = string.Join(".", typeof(MiniblinkForm).Namespace, "Files", "form.js");

            using (var sm = typeof(MiniblinkForm).Assembly.GetManifestResourceStream(js))
            {
                if (sm != null)
                {
                    using (var reader = new StreamReader(sm, Encoding.UTF8))
                    {
                        js = vars + reader.ReadToEnd();
                    }
                }
            }

            e.Frame.RunJs(js);
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (IsDisposed)
            {
                return false;
            }

            if (NoneBorderResize == false)
            {
                return false;
            }

            if (FormBorderStyle != FormBorderStyle.None)
            {
                return false;
            }

            if (WindowState != FormWindowState.Normal)
            {
                return false;
            }

            var ctrl = FromChildHandle(m.HWnd);
            if (ctrl == null || ctrl.FindForm() != this)
            {
                return false;
            }

            var wMsg = (WinConst)m.Msg;

            switch (wMsg)
            {
                //鼠标移动
                case WinConst.WM_MOUSEMOVE:
                    _direct = ShowResizeCursor(PointToClient(MousePosition));
                    break;

                //鼠标单击
                case WinConst.WM_LBUTTONDOWN:
                    if (_direct != ResizeDirect.None)
                    {
                        ResizeMsg();
                        return true;
                    }
                    break;
            }

            return false;
        }

        private ResizeDirect ShowResizeCursor(Point point)
        {
            var rect = ClientRectangle;
            var direct = ResizeDirect.None;

            if (point.X <= ResizeWidth.Left)
            {
                if (point.Y <= ResizeWidth.Top)
                {
                    direct = ResizeDirect.LeftTop;
                }
                else if (point.Y + ResizeWidth.Right >= rect.Height)
                {
                    direct = ResizeDirect.LeftBottom;
                }
                else
                {
                    direct = ResizeDirect.Left;
                }
            }
            else if (point.Y <= ResizeWidth.Top)
            {
                if (point.X <= ResizeWidth.Left)
                {
                    direct = ResizeDirect.LeftTop;
                }
                else if (point.X + ResizeWidth.Right >= rect.Width)
                {
                    direct = ResizeDirect.RightTop;
                }
                else
                {
                    direct = ResizeDirect.Top;
                }
            }
            else if (point.X + ResizeWidth.Right >= rect.Width)
            {
                if (point.Y <= ResizeWidth.Top)
                {
                    direct = ResizeDirect.RightTop;
                }
                else if (point.Y + ResizeWidth.Bottom >= rect.Height)
                {
                    direct = ResizeDirect.RightBottom;
                }
                else
                {
                    direct = ResizeDirect.Right;
                }
            }
            else if (point.Y + ResizeWidth.Bottom >= rect.Height)
            {
                if (point.X <= ResizeWidth.Left)
                {
                    direct = ResizeDirect.LeftBottom;
                }
                else if (point.X + ResizeWidth.Right >= rect.Width)
                {
                    direct = ResizeDirect.RightBottom;
                }
                else
                {
                    direct = ResizeDirect.Bottom;
                }
            }
            else if (_isdrop == false)
            {
                if (Cursor != DefaultCursor)
                {
                    Cursor = DefaultCursor;
                }
            }

            switch (direct)
            {
                case ResizeDirect.Bottom:
                case ResizeDirect.Top:
                    Cursor = Cursors.SizeNS;
                    break;

                case ResizeDirect.Left:
                case ResizeDirect.Right:
                    Cursor = Cursors.SizeWE;
                    break;

                case ResizeDirect.LeftTop:
                case ResizeDirect.RightBottom:
                    Cursor = Cursors.SizeNWSE;
                    break;

                case ResizeDirect.RightTop:
                case ResizeDirect.LeftBottom:
                    Cursor = Cursors.SizeNESW;
                    break;
            }
            return direct;
        }

        private void ResizeMsg()
        {
            const int wmszLeft = 0xF001;
            const int wmszRight = 0xF002;
            const int wmszTop = 0xF003;
            const int wmszTopleft = 0xF004;
            const int wmszTopright = 0xF005;
            const int wmszBottom = 0xF006;
            const int wmszBottomleft = 0xF007;
            const int wmszBottomright = 0xF008;

            var param = 0;
            switch (_direct)
            {
                case ResizeDirect.Top:
                    Cursor = Cursors.SizeNS;
                    param = wmszTop;
                    break;

                case ResizeDirect.Bottom:
                    Cursor = Cursors.SizeNS;
                    param = wmszBottom;
                    break;

                case ResizeDirect.Left:
                    Cursor = Cursors.SizeWE;
                    param = wmszLeft;
                    break;

                case ResizeDirect.Right:
                    Cursor = Cursors.SizeWE;
                    param = wmszRight;
                    break;

                case ResizeDirect.LeftTop:
                    Cursor = Cursors.SizeNWSE;
                    param = wmszTopleft;
                    break;

                case ResizeDirect.LeftBottom:
                    Cursor = Cursors.SizeNESW;
                    param = wmszBottomleft;
                    break;

                case ResizeDirect.RightTop:
                    Cursor = Cursors.SizeNESW;
                    param = wmszTopright;
                    break;

                case ResizeDirect.RightBottom:
                    Cursor = Cursors.SizeNWSE;
                    param = wmszBottomright;
                    break;
            }

            if (param != 0)
            {
                WinApi.SendMessage(Handle, (uint)WinConst.WM_SYSCOMMAND, new IntPtr(0xF000 | param), IntPtr.Zero);
            }
        }

        private enum ResizeDirect
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
            LeftTop,
            LeftBottom,
            RightTop,
            RightBottom
        }

        private static void WatchMouse(Action<Point> onMove, Action onFinish = null)
        {
            var last = MousePosition;

            Task.Factory.StartNew(() =>
            {
                var waiter = new SpinWait();

                while (MouseButtons.HasFlag(MouseButtons.Left))
                {
                    var curr = MousePosition;

                    if (curr.Equals(last) == false)
                    {
                        onMove(curr);
                        last = curr;
                    }

                    waiter.SpinOnce();
                }

                onFinish?.Invoke();
            });
        }
    }
}
