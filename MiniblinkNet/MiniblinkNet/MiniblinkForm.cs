using MiniblinkNet.MiniBlink;
using MiniblinkNet.Windows;
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
        /// 允许使用类样式控制窗体拖拽
        /// </summary>
        public bool DropByClass { get; set; }

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

        /// <summary>
        /// 表单窗口状态
        /// </summary>
        private FormWindowState _windowState = FormWindowState.Normal;

        /// <summary>
        /// 状态Rect
        /// </summary>
        private Rectangle? _stateRect;

        public new FormWindowState WindowState
        {
            get
            {
                return FormBorderStyle != FormBorderStyle.None ? base.WindowState : _windowState;
            }
            set
            {
                if (FormBorderStyle != FormBorderStyle.None)
                {
                    base.WindowState = value;
                    return;
                }
                if (_stateRect.HasValue == false)
                {
                    _stateRect = new Rectangle(Location, Size);
                }
                var rect = _stateRect.Value;

                if (value == FormWindowState.Maximized)
                {
                    if (_windowState != FormWindowState.Maximized)
                    {
                        _stateRect = new Rectangle(Location, Size);
                        Location = new Point(0, 0);
                        Size = Screen.PrimaryScreen.WorkingArea.Size;
                        base.WindowState = FormWindowState.Normal;
                    }
                }
                else if (value == FormWindowState.Minimized)
                {
                    if (base.WindowState == FormWindowState.Normal)
                    {
                        _stateRect = new Rectangle(Location, Size);
                    }

                    base.WindowState = value;
                }
                else if (value == FormWindowState.Normal)
                {
                    Location = rect.Location;
                    Size = rect.Size;
                    base.WindowState = value;
                }
                _windowState = value;
            }
        }
        public MiniblinkBrowser View { get; }

        private ResizeDirect _direct;
        private bool _isdrop;
        private string _dragfunc;
        private string _maxfunc;
        private string _minfunc;
        private string _closefunc;

        public MiniblinkForm() : this(false)
        {

        }

        public MiniblinkForm(bool isTransparent)
        {
            Application.AddMessageFilter(this);
            Border = new FormBorder();
            ShadowWidth = new FormShadowWidth();
            InitializeComponent();
            Controls.Add(View = new MiniblinkBrowser
            {
                Dock = DockStyle.Fill
            });

            IsTransparent = isTransparent;

            if (!IsDesignMode())
            {
                ResizeWidth = new FormResizeWidth(5);

                if (IsTransparent)
                {
                    NoneBorderResize = true;
                    FormBorderStyle = FormBorderStyle.None;
                    View.PaintUpdated += Miniblink_Paint;
                }

                DropByClass = FormBorderStyle == FormBorderStyle.None;
                var tmp = Guid.NewGuid().ToString().Replace("-", "");
                View.BindNetFunc(new NetFunc(_dragfunc = "drag" + tmp, DropStart));
                View.BindNetFunc(new NetFunc(_maxfunc = "max" + tmp, MaxFunc));
                View.BindNetFunc(new NetFunc(_minfunc = "min" + tmp, MinFunc));
                View.BindNetFunc(new NetFunc(_closefunc = "close" + tmp, CloseFunc));

                View.DocumentReady += RegisterJsEvent;
                View.RegisterNetFunc(this);
            }
        }

        /// <summary>
        /// 加载完成时
        /// </summary>
        private void MiniblinkForm_Load(object sender, EventArgs e)
        {
            Shown += (ls, le) =>
            {
                SetFormStartPos();
                DrawShadow();
            };

            Activated += FormActivated;

            Deactivate += FormDeactivate;

            SizeChanged += (ls, le) =>
            {
                FormSizeChanged(ls, le);
                FormMaxSizeChanged(ls, le);
                FormMinSizeChanged(ls, le);
            };

            if (!IsDesignMode() && IsTransparent)
            {
                SetTransparent();
                using (var image = View.DrawToBitmap())
                {
                    TransparentPaint(image, image.Width, image.Height);
                }
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
        private void FormActivated(object sender, EventArgs e)
        {
            MBApi.wkeRunJSW(View.MiniblinkHandle, "window.activated!=undefined?window.activated():'unbound event';");
        }

        /// <summary>
        /// 失去焦点
        /// </summary>
        private void FormDeactivate(object sender, EventArgs e)
        {
            MBApi.wkeRunJSW(View.MiniblinkHandle, "window.deactivate!=undefined?window.deactivate():'unbound event';");
        }

        /// <summary>
        /// 跳转窗体大小
        /// </summary>
        private void FormSizeChanged(object sender, EventArgs e)
        {
            MBApi.wkeRunJSW(View.MiniblinkHandle, $"window.sizechanged!=undefined?window.sizechanged({Width},{Height}):'unbound event';");
        }

        /// <summary>
        /// 最大化和还原
        /// </summary>
        private void FormMaxSizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                MBApi.wkeRunJSW(View.MiniblinkHandle, $"window.maxsizechanged!=undefined?window.maxsizechanged(2):'unbound event';");
            }
            else if (WindowState == FormWindowState.Normal)
            {
                MBApi.wkeRunJSW(View.MiniblinkHandle, $"window.maxsizechanged!=undefined?window.maxsizechanged(0):'unbound event';");
            }
        }

        /// <summary>
        /// 最小化
        /// </summary>
        private void FormMinSizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                MBApi.wkeRunJSW(View.MiniblinkHandle, $"window.minsizechanged!=undefined?window.minsizechanged(1):'unbound event';");
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == (int)WinConst.WM_SYSCOMMAND)
            {
                //窗口还原消息
                if (Utils.Dword(m.WParam).ToInt32() == 61728)
                {
                    WindowState = FormWindowState.Normal;
                    if (IsTransparent)
                    {
                        using (var image = View.DrawToBitmap())
                        {
                            TransparentPaint(image, image.Width, image.Height);
                        }
                    }
                }
            }

            if (m.Msg == (int)WinConst.WM_NCPAINT)
            {
                DrawShadow();
            }
        }

        private void Miniblink_Paint(object sender, PaintUpdatedEventArgs e)
        {
            if (!IsDisposed && !IsDesignMode())
            {
                TransparentPaint(e.Image, e.Image.Width, e.Image.Height);

                e.Cancel = true;
            }
        }

        private void TransparentPaint(Bitmap bitmap, int width, int height)
        {
            var oldBits = IntPtr.Zero;
            var hBitmap = IntPtr.Zero;
            var memDc = WinApi.CreateCompatibleDC(IntPtr.Zero);

            try
            {
                var dst = new WinPoint { x = Left, y = Top };
                var src = new WinPoint();
                var size = new WinSize(width, height);

                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                oldBits = WinApi.SelectObject(memDc, hBitmap);

                var blend = new BlendFunction
                {
                    BlendOp = (byte)WinConst.AC_SRC_OVER,
                    SourceConstantAlpha = 255,
                    AlphaFormat = (byte)WinConst.AC_SRC_ALPHA
                };

                WinApi.UpdateLayeredWindow(Handle, IntPtr.Zero, ref dst, ref size, memDc, ref src, 0, ref blend, (int)WinConst.ULW_ALPHA);
            }
            finally
            {
                if (hBitmap != IntPtr.Zero)
                {
                    WinApi.SelectObject(memDc, oldBits);
                    WinApi.DeleteObject(hBitmap);
                }
                WinApi.DeleteDC(memDc);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;

                if (CheckAero() == false)
                {
                    cp.ClassStyle |= (int)WinConst.CS_DROPSHADOW;
                }

                return cp;
            }
        }

        private static bool CheckAero()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                var enabled = 0;
                WinApi.DwmIsCompositionEnabled(ref enabled);
                return enabled == 1;
            }
            return false;
        }

        private void ResizeMsg()
        {
            const int WMSZ_LEFT = 0xF001;
            const int WMSZ_RIGHT = 0xF002;
            const int WMSZ_TOP = 0xF003;
            const int WMSZ_TOPLEFT = 0xF004;
            const int WMSZ_TOPRIGHT = 0xF005;
            const int WMSZ_BOTTOM = 0xF006;
            const int WMSZ_BOTTOMLEFT = 0xF007;
            const int WMSZ_BOTTOMRIGHT = 0xF008;

            var param = 0;
            switch (_direct)
            {
                case ResizeDirect.Top:
                    Cursor = Cursors.SizeNS;
                    param = WMSZ_TOP;
                    break;
                case ResizeDirect.Bottom:
                    Cursor = Cursors.SizeNS;
                    param = WMSZ_BOTTOM;
                    break;
                case ResizeDirect.Left:
                    Cursor = Cursors.SizeWE;
                    param = WMSZ_LEFT;
                    break;
                case ResizeDirect.Right:
                    Cursor = Cursors.SizeWE;
                    param = WMSZ_RIGHT;
                    break;
                case ResizeDirect.LeftTop:
                    Cursor = Cursors.SizeNWSE;
                    param = WMSZ_TOPLEFT;
                    break;
                case ResizeDirect.LeftBottom:
                    Cursor = Cursors.SizeNESW;
                    param = WMSZ_BOTTOMLEFT;
                    break;
                case ResizeDirect.RightTop:
                    Cursor = Cursors.SizeNESW;
                    param = WMSZ_TOPRIGHT;
                    break;
                case ResizeDirect.RightBottom:
                    Cursor = Cursors.SizeNWSE;
                    param = WMSZ_BOTTOMRIGHT;
                    break;
            }

            if (param != 0)
            {
                WinApi.SendMessage(Handle, (uint)WinConst.WM_SYSCOMMAND, new IntPtr(0xF000 | param), IntPtr.Zero);
            }
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

        /// <summary>
        /// 拖动窗体和禁用拖动窗体
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private object DropStart(NetFuncContext context)
        {
            if (DropByClass && _isdrop == false &&
                WindowState != FormWindowState.Maximized &&
                MouseButtons == MouseButtons.Left)
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
        /// 注册脚本事件
        /// </summary>
        private void RegisterJsEvent(object sender, DocumentReadyEventArgs e)
        {
            var map = new Dictionary<string, string>
            {
                {"maxName", _maxfunc},
                {"minName", _minfunc},
                {"closeName", _closefunc},
                {"dragName", _dragfunc}
            };
            var vars = string.Join(";", map.Keys.Select(k => $"var {k}='{map[k]}';")) + ";";
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

        internal bool IsDesignMode()
        {
            return View.IsDesignMode();
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

        public bool PreFilterMessage(ref Message m)
        {
            if (IsDisposed)
            {
                Application.RemoveMessageFilter(this);
                return false;
            }
            var ctrl = FromChildHandle(m.HWnd);

            if (ctrl == null || ctrl.FindForm() != this)
            {
                return false;
            }

            //鼠标单击
            if (m.Msg == (int)WinConst.WM_LBUTTONDOWN && _direct != ResizeDirect.None)
            {
                ResizeMsg();
                return true;
            }

            //鼠标移动
            if (m.Msg == (int)WinConst.WM_MOUSEMOVE)
            {
                if (NoneBorderResize && FormBorderStyle == FormBorderStyle.None &&
                    WindowState == FormWindowState.Normal)
                {
                    _direct = ShowResizeCursor(PointToClient(MousePosition));
                    return _direct != ResizeDirect.None;
                }
            }

            return false;
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
