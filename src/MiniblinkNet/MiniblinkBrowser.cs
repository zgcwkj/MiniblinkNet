using MiniblinkNet.MiniBlink;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MiniblinkNet
{
    public partial class MiniblinkBrowser : UserControl, IMiniblink
    {
        static MiniblinkBrowser()
        {
            if (IsDesignMode() == false)
            {
                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
                MiniblinkSetting.BindNetFunc(new NetFunc(_popHookName, OnHookPop));
                MiniblinkSetting.BindNetFunc(new NetFunc(_openHookName, OnHookWindowOpen));
                MiniblinkSetting.BindNetFunc(new NetFunc(_callNet, OnCallNet));
            }
        }

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("DotNetZip,"))
            {
                var resource = typeof(IMiniblink).Namespace + ".Files.DotNetZip.dll";
                var curAsm = Assembly.GetExecutingAssembly();
                using (var sm = curAsm.GetManifestResourceStream(resource))
                {
                    if (sm == null)
                    {
                        throw new Exception("没有找到DotNetZip.dll");
                    }

                    var data = new byte[sm.Length];
                    sm.Read(data, 0, data.Length);
                    return Assembly.Load(data);
                }
            }

            return null;
        }
        private static string _procName = Process.GetCurrentProcess().ProcessName;
        private static string _popHookName = "func" + Guid.NewGuid().ToString().Replace("-", "");
        private static string _openHookName = "func" + Guid.NewGuid().ToString().Replace("-", "");
        private static string _callNet = "func" + Guid.NewGuid().ToString().Replace("-", "");
        private EventHandler<PaintUpdatedEventArgs> _browserPaintUpdated;
        private wkePaintBitUpdatedCallback _paintBitUpdated;
        private wkePaintUpdatedCallback _paintUpdated;
        private wkeCreateViewCallback _createView;
        private ConcurrentQueue<MouseEventArgs> _mouseMoveEvents = new ConcurrentQueue<MouseEventArgs>();
        private bool _fiexdCursor;

        private ConcurrentDictionary<long, RequestEventArgs> _requestMap =
            new ConcurrentDictionary<long, RequestEventArgs>();

        private List<NetFunc> _funcs = new List<NetFunc>();
        private bool _v8IsReady;
        private List<FrameContext> _iframes = new List<FrameContext>();
        private bool _lockMouseMove;

        public MiniblinkBrowser()
        {
            InitializeComponent();

            ResourceLoader = new List<ILoadResource>();

            if (!IsDesignMode() && !DesignMode)
            {
                MiniblinkHandle = MiniblinkSetting.CreateWebView(this);

                if (MiniblinkHandle == IntPtr.Zero)
                {
                    throw new WKECreateException();
                }

                _browserPaintUpdated += BrowserPaintUpdated;
                _paintBitUpdated = OnWkeOnPaintBitUpdated;
                _paintUpdated = OnWkeOnPaintUpdated;
                _createView = OnCreateView;
                RequestBefore += LoadResource;
                RequestBefore += LoadCache;
                DidCreateScriptContext += V8Ready;

                MBApi.wkeSetContextMenuEnabled(MiniblinkHandle, false);
                MBApi.wkeSetDragEnable(MiniblinkHandle, false);
                MBApi.wkeSetDragDropEnable(MiniblinkHandle, false);
                MBApi.wkeSetNavigationToNewWindowEnable(MiniblinkHandle, true);
                MBApi.wkeOnCreateView(MiniblinkHandle, _createView, IntPtr.Zero);
                MBApi.wkeSetHandle(MiniblinkHandle, Handle);
                MBApi.wkeOnPaintBitUpdated(MiniblinkHandle, _paintBitUpdated, IntPtr.Zero);
                MBApi.wkeOnPaintUpdated(MiniblinkHandle, _paintUpdated, IntPtr.Zero);

                DeviceParameter = new DeviceParameter(this);
                Cookies = new CookieCollection(this, "cookies.dat");
                BmpPaintMode = true;
            }
        }

        private void LoadCache(object sender, RequestEventArgs e)
        {
            if (ResourceCache == null || "get".SW(e.Method) == false || ResourceCache.Matchs(e.Url) == false)
                return;

            var data = ResourceCache.Get(e.Url);

            if (data != null)
            {
                e.Data = data;
            }
            else
            {
                e.Response += (rs, re) => { ResourceCache.Save(re.Url, re.Data); };
            }
        }

        private IntPtr OnCreateView(IntPtr mb, IntPtr param, wkeNavigationType type, IntPtr url, IntPtr windowFeatures)
        {
            if (_createView == null)
            {
                return IntPtr.Zero;
            }

            if (type == wkeNavigationType.LinkClick)
            {
                var e = new MiniBlink.NavigateEventArgs
                {
                    Url = url.WKEToUTF8String(),
                    Type = NavigateType.BlankLink
                };
                OnNavigateBefore(e);

                if (e.Cancel == false)
                {
                    LoadUri(e.Url);
                }
            }
            else
            {
                OnNavigateBefore(mb, param, type, url);
            }

            return IntPtr.Zero;
        }

        private void DestroyCallback()
        {
            _paintBitUpdated = null;
            _createView = null;
            _wkeLoadUrlBegin = null;
            _wkeLoadUrlEnd = null;
            _wkeLoadUrlFail = null;
            _wkeNetResponse = null;
            MBApi.wkeOnPaintBitUpdated(MiniblinkHandle, null, IntPtr.Zero);
            MBApi.wkeOnPaintUpdated(MiniblinkHandle, null, IntPtr.Zero);
            MBApi.wkeOnURLChanged2(MiniblinkHandle, null, IntPtr.Zero);
            MBApi.wkeOnNavigation(MiniblinkHandle, null, IntPtr.Zero);
            MBApi.wkeOnDocumentReady2(MiniblinkHandle, null, IntPtr.Zero);
            MBApi.wkeOnConsole(MiniblinkHandle, null, IntPtr.Zero);
            MBApi.wkeNetOnResponse(MiniblinkHandle, null, IntPtr.Zero);
            MBApi.wkeOnLoadUrlBegin(MiniblinkHandle, null, IntPtr.Zero);
            MBApi.wkeOnLoadUrlEnd(MiniblinkHandle, null, IntPtr.Zero);
            MBApi.wkeOnDownload(MiniblinkHandle, null, IntPtr.Zero);
            MBApi.wkeOnCreateView(MiniblinkHandle, null, IntPtr.Zero);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Enabled = false;
            Destroy?.Invoke(this, new EventArgs());
            if (IsDesignMode() == false)
            {
                DestroyCallback();
                MiniblinkSetting.DestroyWebView(MiniblinkHandle);
            }

            ResourceCache = null;
            ResourceLoader.Clear();
            _requestMap.Clear();
            _mouseMoveEvents = null;
            _funcs.Clear();
            _iframes.Clear();

            base.OnHandleDestroyed(e);
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void OnWkeOnPaintUpdated(IntPtr webView, IntPtr param, IntPtr hdc,
            int x, int y, int w, int h)
        {
            if (!BmpPaintMode)
            {
                var e = new PaintUpdatedEventArgs
                {
                    WebView = webView,
                    Param = MBApi.wkeGetViewDC(webView),
                    Rect = new Rectangle(x, y, w, h),
                    Width = Width,
                    Height = Height
                };
                PaintUpdated?.Invoke(this, e);
                if (e.Cancel == false)
                {
                    Invalidate(new Rectangle(x, y, w, h));
                }
            }
        }

        private void OnPaint(IntPtr hWnd)
        {
            var ps = new WinPaint();
            var wHdc = WinApi.BeginPaint(hWnd, ref ps);
            var mbHdc = MBApi.wkeGetViewDC(MiniblinkHandle);
            var x = ps.rcPaint.left;
            var y = ps.rcPaint.top;
            var w = ps.rcPaint.right - ps.rcPaint.left;
            var h = ps.rcPaint.bottom - ps.rcPaint.top;
            WinApi.BitBlt(wHdc, x, y, w, h, mbHdc, x, y, (int)WinConst.SRCCOPY);
            WinApi.EndPaint(wHdc, ref ps);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!IsDesignMode() && !IsDisposed && BmpPaintMode)
            {
                using (var bitmap = DrawToBitmap(e.ClipRectangle))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawImage(bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
                }
            }
        }

        private void OnWkeOnPaintBitUpdated(IntPtr webView, IntPtr param, IntPtr buf, IntPtr rectPtr, int width, int height)
        {
            if (buf == IntPtr.Zero || _paintBitUpdated == null || BmpPaintMode == false) return;
            var stride = width * 4 + width * 4 % 4;
            var rect = (wkeRect)Marshal.PtrToStructure(rectPtr, typeof(wkeRect));
            using (var view = new Bitmap(width, height, stride, PixelFormat.Format32bppPArgb, buf))
            {
                var e = new PaintUpdatedEventArgs
                {
                    WebView = webView,
                    Image = view,
                    Rect = new Rectangle(rect.x, rect.y, rect.w, rect.h),
                    Width = width,
                    Height = height
                };
                PaintUpdated?.Invoke(this, e);

                if (!e.Cancel)
                {
                    _browserPaintUpdated(this, e);
                }
            }
        }

        private void BrowserPaintUpdated(object sender, PaintUpdatedEventArgs e)
        {
            if (!IsDisposed)
            {
                using (var g = CreateGraphics())
                {
                    var rect = new RectangleF(e.Rect.X, e.Rect.Y, e.Rect.Width, e.Rect.Height);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.DrawImage(e.Image, rect, rect, GraphicsUnit.Pixel);
                }
            }
        }

        private void LoadResource(object sender, RequestEventArgs e)
        {
            if (ResourceLoader.Count < 1)
                return;
            if ("get".SW(e.Method) == false)
                return;
            if (e.Url.SW("http:") == false && e.Url.SW("https:") == false)
                return;

            var uri = new Uri(e.Url);

            foreach (var handler in ResourceLoader.ToArray())
            {
                if (handler.Domain.Equals(uri.Host, StringComparison.OrdinalIgnoreCase) == false)
                    continue;
                var data = handler.ByUri(uri);
                if (data != null)
                {
                    e.Data = data;
                    break;
                }
            }
        }

        private static object OnCallNet(NetFuncContext context)
        {
            if (context.Paramters.Length == 0) return null;
            var name = Convert.ToString(context.Paramters[0]);
            if (string.IsNullOrEmpty(name)) return null;
            var bro = (MiniblinkBrowser)context.Miniblink;
            var func = bro._funcs.FirstOrDefault(i => i.Name == name);
            var ps = context.Paramters.Skip(1).ToArray();
            return func?.OnFunc(context.Miniblink, ps);
        }

        private string BindFuncJs(bool isMain)
        {
            var js = "";
            foreach (var func in _funcs)
            {
                if (isMain == false && func.BindToSub == false)
                    continue;

                var call = _callNet;
                if (isMain == false)
                {
                    call = $"window.top['{call}']";
                }

                js += $@"
                window.{func.Name}=function(){{
                    var arr = Array.prototype.slice.call(arguments);
                    var args = ['{func.Name}'].concat(arr);
                    return {call}.apply(null,args);
                }};";
            }

            return js;
        }

        private void HookJs(DidCreateScriptContextEventArgs e)
        {
            var map = new Dictionary<string, string>
            {
                {"popHookName", $"'{_popHookName}'"},
                {"openHookName", $"'{_openHookName}'"}
            };
            var vars = string.Join(";", map.Keys.Select(k => $"var {k}={map[k]};")) + ";";
            var js = string.Join(".", new[] { typeof(MiniblinkBrowser).Namespace, "Files", "browser.js" });

            using (var sm = typeof(MiniblinkBrowser).Assembly.GetManifestResourceStream(js))
            {
                if (sm != null)
                {
                    using (var reader = new StreamReader(sm, Encoding.UTF8))
                    {
                        js = vars + reader.ReadToEnd() + ";" + BindFuncJs(e.Frame.IsMain);
                    }

                    e.Frame.RunJs(js);
                }
            }
        }

        private void V8Ready(object sender, DidCreateScriptContextEventArgs e)
        {
            _v8IsReady = true;

            if (e.Frame.IsMain == false)
            {
                _iframes.Add(e.Frame);
            }

            HookJs(e);
        }

        private AlertEventArgs OnAlert(string message, string title)
        {
            var args = new AlertEventArgs
            {
                Window = new FrmAlert
                {
                    Message = message,
                    Text = title
                }
            };
            OnAlertBefore(args);
            args.Window?.ShowDialog();
            return args;
        }

        private ConfirmEventArgs OnConfirm(string message, string title)
        {
            var args = new ConfirmEventArgs
            {
                Window = new FrmConfirm
                {
                    Message = message,
                    Text = title
                }
            };
            OnConfirmBefore(args);
            args.Window?.ShowDialog();
            if (args.Result.HasValue == false && args.Window != null)
            {
                args.Result = args.Window.IsOk;
            }

            return args;
        }

        private PromptEventArgs OnPrompt(string message, string input, string title)
        {
            var args = new PromptEventArgs
            {
                Window = new FrmPrompt
                {
                    Text = title,
                    Message = message,
                    Value = input
                }
            };
            OnPromptBefore(args);
            args.Window?.ShowDialog();
            if (args.Result == null && args.Window != null)
            {
                args.Result = args.Window.Value;
            }

            return args;
        }

        private static object OnHookPop(NetFuncContext context)
        {
            if (context.Paramters.Length < 1)
            {
                return null;
            }

            var bro = (MiniblinkBrowser)context.Miniblink;
            var type = context.Paramters[0].ToString().ToLower();

            if ("alert" == type)
            {
                var msg = "";
                var title = new Uri(bro.Url).Host;
                if (context.Paramters.Length > 1 && context.Paramters[1] != null)
                {
                    msg = context.Paramters[1].ToString();
                }

                if (context.Paramters.Length > 2 && context.Paramters[2] != null)
                {
                    dynamic opt = context.Paramters[2];
                    if (opt.title != null)
                    {
                        title = opt.title.ToString();
                    }
                }

                bro.OnAlert(msg, title);
                return null;
            }

            if ("confirm" == type)
            {
                var msg = "";
                var title = new Uri(bro.Url).Host;
                if (context.Paramters.Length > 1 && context.Paramters[1] != null)
                {
                    msg = context.Paramters[1].ToString();
                }

                if (context.Paramters.Length > 2 && context.Paramters[2] != null)
                {
                    dynamic opt = context.Paramters[2];
                    if (opt.title != null)
                    {
                        title = opt.title.ToString();
                    }
                }

                var e = bro.OnConfirm(msg, title);
                return e.Result.GetValueOrDefault();
            }

            if ("prompt" == type)
            {
                var msg = "";
                var input = "";
                var title = new Uri(bro.Url).Host;
                if (context.Paramters.Length > 1 && context.Paramters[1] != null)
                {
                    msg = context.Paramters[1].ToString();
                }

                if (context.Paramters.Length > 2 && context.Paramters[2] != null)
                {
                    input = context.Paramters[2].ToString();
                }

                if (context.Paramters.Length > 3 && context.Paramters[3] != null)
                {
                    dynamic opt = context.Paramters[3];
                    if (opt.title != null)
                    {
                        title = opt.title.ToString();
                    }
                }

                var e = bro.OnPrompt(msg, input, title);
                return e.Result;
            }

            return null;
        }

        private static object OnHookWindowOpen(NetFuncContext context)
        {
            string url = null;
            string name = null;
            string specs = null;
            string replace = null;
            var map = new Dictionary<string, string>();

            if (context.Paramters.Length > 0 && context.Paramters[0] != null)
            {
                url = context.Paramters[0].ToString();
            }

            if (context.Paramters.Length > 1 && context.Paramters[1] != null)
            {
                name = context.Paramters[1].ToString();
            }

            if (context.Paramters.Length > 2 && context.Paramters[2] != null)
            {
                specs = context.Paramters[2].ToString();
            }

            if (context.Paramters.Length > 3 && context.Paramters[3] != null)
            {
                replace = context.Paramters[3].ToString();
            }

            if (specs != null)
            {
                var items = specs.Split(',');
                foreach (var item in items)
                {
                    var kv = item.Split('=');
                    if (kv.Length == 1)
                    {
                        map[kv[0]] = "";
                    }
                    else if (kv.Length == 2)
                    {
                        map[kv[0]] = kv[1];
                    }
                }
            }

            var bro = (MiniblinkBrowser)context.Miniblink;
            var navArgs = new MiniBlink.NavigateEventArgs
            {
                Url = url,
                Type = NavigateType.WindowOpen
            };
            bro.OnNavigateBefore(navArgs);
            if (navArgs.Cancel)
            {
                return null;
            }

            var e = bro.OnWindowOpen(url, name, map, "true" == replace);
            return e.ReturnValue;
        }

        private void OnDropFiles(bool isDone, int x, int y, string[] files)
        {
            if (FireDropFile)
            {
                var data = string.Join(",", files);
                x += ScrollLeft;
                y += ScrollTop;
                CallJsFunc("fireDropFileEvent", data, x, y, isDone);
            }
        }

        private void DragFileEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
                var items = (Array)e.Data.GetData(DataFormats.FileDrop);
                var files = items.Cast<string>().ToArray();
                var p = PointToClient(new Point(e.X, e.Y));
                OnDropFiles(false, p.X, p.Y, files);
            }
        }

        private void DragFileDrop(object sender, DragEventArgs e)
        {
            var items = (Array)e.Data.GetData(DataFormats.FileDrop);
            var files = items.Cast<string>().ToArray();
            var p = PointToClient(new Point(e.X, e.Y));
            OnDropFiles(true, p.X, p.Y, files);
        }

        internal static bool IsDesignMode()
        {
            var returnFlag = false;

#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                returnFlag = true;
            }
            else if (_procName == "devenv")
            {
                returnFlag = true;
            }
#endif

            return returnFlag;
        }

        #region 消息处理

        protected override void OnResize(EventArgs e)
        {
            if (!IsDesignMode() && MiniblinkHandle != IntPtr.Zero)
            {
                MBApi.wkeResize(MiniblinkHandle, Width, Height);
            }

            base.OnResize(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            var code = e.KeyValue;
            var flags = (uint)wkeKeyFlags.WKE_REPEAT;

            if (Utils.IsExtendedKey(e.KeyCode))
            {
                flags |= (uint)wkeKeyFlags.WKE_EXTENDED;
            }

            if (MBApi.wkeFireKeyUpEvent(MiniblinkHandle, code, flags, false))
            {
                e.Handled = true;
            }

            base.OnKeyUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var code = e.KeyValue;
            var flags = (uint)wkeKeyFlags.WKE_REPEAT;

            if (Utils.IsExtendedKey(e.KeyCode))
            {
                flags |= (uint)wkeKeyFlags.WKE_EXTENDED;
            }

            if (MBApi.wkeFireKeyDownEvent(MiniblinkHandle, code, flags, false))
            {
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            var code = e.KeyChar;
            var flags = (uint)wkeKeyFlags.WKE_REPEAT;

            if (MBApi.wkeFireKeyPressEvent(MiniblinkHandle, code, flags, false))
            {
                e.Handled = true;
            }

            base.OnKeyPress(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            WinApi.SetCapture(Handle);
            WinConst msg = 0;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    msg = WinConst.WM_LBUTTONDOWN;
                    break;
                case MouseButtons.Middle:
                    msg = WinConst.WM_MBUTTONDOWN;
                    break;
                case MouseButtons.Right:
                    msg = WinConst.WM_RBUTTONDOWN;
                    break;
            }

            if (msg != 0)
            {
                OnWkeMouseEvent(msg, e);
            }

            if (MouseMoveOptimize)
            {
                _fiexdCursor = true;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            WinApi.ReleaseCapture();
            WinConst msg = 0;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    msg = WinConst.WM_LBUTTONUP;
                    break;
                case MouseButtons.Middle:
                    msg = WinConst.WM_MBUTTONUP;
                    break;
                case MouseButtons.Right:
                    msg = WinConst.WM_RBUTTONUP;
                    break;
            }

            if (msg != 0)
            {
                OnWkeMouseEvent(msg, e);
            }

            _fiexdCursor = false;

            base.OnMouseUp(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            WinConst msg = 0;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    msg = WinConst.WM_LBUTTONDBLCLK;
                    break;
                case MouseButtons.Middle:
                    msg = WinConst.WM_MBUTTONDBLCLK;
                    break;
                case MouseButtons.Right:
                    msg = WinConst.WM_RBUTTONDBLCLK;
                    break;
            }

            if (msg != 0)
            {
                OnWkeMouseEvent(msg, e);
            }

            base.OnMouseDoubleClick(e);
        }

        private void MouseMoveInvoke()
        {
            if (_mouseMoveEvents == null)
                return;

            MouseEventArgs e;
            if (_mouseMoveEvents.TryDequeue(out e))
            {
                BeginInvoke(new Action<MouseEventArgs>(s =>
                {
                    OnWkeMouseEvent(WinConst.WM_MOUSEMOVE, s);
                    base.OnMouseMove(s);
                    MouseMoveInvoke();
                }), e);
            }
            else
            {
                _lockMouseMove = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (MouseMoveOptimize)
            {
                _mouseMoveEvents.Enqueue(e);
                if (_lockMouseMove == false)
                {
                    _lockMouseMove = true;
                    MouseMoveInvoke();
                }
            }
            else
            {
                OnWkeMouseEvent(WinConst.WM_MOUSEMOVE, e);
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            uint flags = 0;

            if (ModifierKeys.HasFlag(Keys.Control))
                flags |= (uint)wkeMouseFlags.WKE_CONTROL;
            if (ModifierKeys.HasFlag(Keys.LShiftKey))
                flags |= (uint)wkeMouseFlags.WKE_SHIFT;

            if (e.Button.HasFlag(MouseButtons.Left))
                flags |= (uint)wkeMouseFlags.WKE_LBUTTON;
            if (e.Button.HasFlag(MouseButtons.Middle))
                flags |= (uint)wkeMouseFlags.WKE_MBUTTON;
            if (e.Button.HasFlag(MouseButtons.Right))
                flags |= (uint)wkeMouseFlags.WKE_RBUTTON;

            MBApi.wkeFireMouseWheelEvent(MiniblinkHandle, e.X, e.Y, e.Delta, flags);
            base.OnMouseWheel(e);
        }

        private void OnWkeMouseEvent(WinConst msg, MouseEventArgs e)
        {
            if (MouseEnabled || TouchEnabled)
            {
                var flags = 0;

                if (ModifierKeys.HasFlag(Keys.Control))
                    flags |= (int)wkeMouseFlags.WKE_CONTROL;
                if (ModifierKeys.HasFlag(Keys.LShiftKey))
                    flags |= (int)wkeMouseFlags.WKE_SHIFT;

                if (e.Button.HasFlag(MouseButtons.Left))
                    flags |= (int)wkeMouseFlags.WKE_LBUTTON;
                if (e.Button.HasFlag(MouseButtons.Middle))
                    flags |= (int)wkeMouseFlags.WKE_MBUTTON;
                if (e.Button.HasFlag(MouseButtons.Right))
                    flags |= (int)wkeMouseFlags.WKE_RBUTTON;

                MBApi.wkeFireMouseEvent(MiniblinkHandle, (int)msg, e.X, e.Y, flags);
            }
        }

        private void SetWkeCursor()
        {
            var type = MBApi.wkeGetCursorInfoType(MiniblinkHandle);
            switch (type)
            {
                case wkeCursorInfo.Hand:
                    if (Cursor != Cursors.Hand)
                    {
                        Cursor = Cursors.Hand;
                    }

                    break;
                case wkeCursorInfo.IBeam:
                    if (Cursor != Cursors.IBeam)
                    {
                        Cursor = Cursors.IBeam;
                    }

                    break;
                case wkeCursorInfo.Pointer:
                    if (Cursor != Cursors.Default)
                    {
                        Cursor = Cursors.Default;
                    }

                    break;
                case wkeCursorInfo.ColumnResize:
                    if (Cursor != Cursors.VSplit)
                    {
                        Cursor = Cursors.VSplit;
                    }

                    break;
                case wkeCursorInfo.RowResize:
                    if (Cursor != Cursors.HSplit)
                    {
                        Cursor = Cursors.HSplit;
                    }

                    break;
                case wkeCursorInfo.Cross:
                    if (Cursor != Cursors.Cross)
                    {
                        Cursor = Cursors.Cross;
                    }

                    break;
                default:
                    Console.WriteLine("未实现的指针类型：" + type);
                    break;
            }
        }

        private void SetImeStartPos()
        {
            var caret = MBApi.wkeGetCaretRect(MiniblinkHandle);
            var comp = new CompositionForm
            {
                dwStyle = (int)WinConst.CFS_POINT | (int)WinConst.CFS_FORCE_POSITION,
                ptCurrentPos =
                {
                    x = caret.x,
                    y = caret.y
                }
            };
            var imc = WinApi.ImmGetContext(Handle);
            WinApi.ImmSetCompositionWindow(imc, ref comp);
            WinApi.ImmReleaseContext(Handle, imc);
        }

        protected override void WndProc(ref Message m)
        {
            if (IsDesignMode())
            {
                base.WndProc(ref m);
            }
            else
            {
                switch ((WinConst)m.Msg)
                {
                    case WinConst.WM_INPUTLANGCHANGE:
                        {
                            DefWndProc(ref m);
                            break;
                        }

                    case WinConst.WM_IME_STARTCOMPOSITION:
                        {
                            SetImeStartPos();
                            break;
                        }

                    case WinConst.WM_SETFOCUS:
                        {
                            MBApi.wkeSetFocus(MiniblinkHandle);
                            break;
                        }

                    case WinConst.WM_KILLFOCUS:
                        {
                            MBApi.wkeKillFocus(MiniblinkHandle);
                            break;
                        }

                    case WinConst.WM_SETCURSOR:
                        {
                            if (MouseButtons == MouseButtons.None)
                            {
                                _fiexdCursor = false;
                            }

                            if (_fiexdCursor == false)
                            {
                                SetWkeCursor();
                                base.WndProc(ref m);
                            }

                            break;
                        }

                    case WinConst.WM_ERASEBKGND:
                        {
                            m.Result = new IntPtr(1);
                            break;
                        }

                    case WinConst.WM_PAINT:
                        if (BmpPaintMode)
                        {
                            base.WndProc(ref m);
                        }
                        else
                        {
                            OnPaint(m.HWnd);
                            m.Result = new IntPtr(1);
                            DefWndProc(ref m);
                        }

                        break;
                    default:
                        {
                            base.WndProc(ref m);
                            break;
                        }
                }
            }
        }

        #endregion
    }
}