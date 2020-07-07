using Ionic.Zip;
using MiniblinkNet.MiniBlink;
using MiniblinkNet.MiniBlink.Interface;
using MiniblinkNet.Windows;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NavigateEventArgs = MiniblinkNet.MiniBlink.NavigateEventArgs;

namespace MiniblinkNet
{
    public partial class MiniblinkBrowser : UserControl, IMiniblink
    {
        /// <summary>
        /// 包地址
        /// </summary>
        private static string Namespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        //private static string Namespace = typeof(IMiniblink).Namespace;

        static MiniblinkBrowser()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
            MiniblinkSetting.BindNetFunc(new NetFunc(_popHookName, OnHookPop));
            MiniblinkSetting.BindNetFunc(new NetFunc(_openHookName, OnHookWindowOpen));
            MiniblinkSetting.BindNetFunc(new NetFunc(_callNet, OnCallNet));
        }

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("DotNetZip,"))
            {
                var resource = Namespace + ".Files.DotNetZip.dll";
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

        #region 属性

        public IList<ILoadResource> ResourceLoader { get; }
        public CookieCollection Cookies { get; }
        public bool MouseMoveOptimize { get; set; } = true;
        public IResourceCache ResourceCache { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IntPtr MiniblinkHandle { get; private set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DeviceParameter DeviceParameter { get; }

        private bool _fireDropFile;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FireDropFile
        {
            get { return _fireDropFile; }
            set
            {
                if (_fireDropFile == value)
                {
                    return;
                }

                if (value)
                {
                    DragDrop += DragFileDrop;
                    DragEnter += DragFileEnter;
                }
                else
                {
                    DragDrop -= DragFileDrop;
                    DragEnter -= DragFileEnter;
                }

                _fireDropFile = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Url => MBApi.wkeGetURL(MiniblinkHandle).ToUTF8String() ?? string.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDocumentReady => MBApi.wkeIsDocumentReady(MiniblinkHandle);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DocumentTitle => MBApi.wkeGetTitle(MiniblinkHandle).ToUTF8String() ?? string.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int DocumentWidth => MBApi.wkeGetWidth(MiniblinkHandle);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int DocumentHeight => MBApi.wkeGetHeight(MiniblinkHandle);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ContentWidth => MBApi.wkeGetContentWidth(MiniblinkHandle);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ContentHeight => MBApi.wkeGetContentHeight(MiniblinkHandle);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ViewWidth => MBApi.wkeGetWidth(MiniblinkHandle);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ViewHeight => MBApi.wkeGetHeight(MiniblinkHandle);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanGoBack => MBApi.wkeCanGoBack(MiniblinkHandle);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanGoForward => MBApi.wkeCanGoForward(MiniblinkHandle);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Zoom
        {
            get { return IsDesignMode() ? 0 : MBApi.wkeGetZoomFactor(MiniblinkHandle); }
            set
            {
                if (!IsDesignMode())
                {
                    MBApi.wkeSetZoomFactor(MiniblinkHandle, value);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string UserAgent
        {
            get { return IsDesignMode() ? "" : MBApi.wkeGetUserAgent(MiniblinkHandle).ToUTF8String(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ScrollTop
        {
            get
            {
                return IsDesignMode()
                    ? 0
                    : Convert.ToInt32(
                        RunJs("return Math.max(document.documentElement.scrollTop,document.body.scrollTop)"));
            }
            set
            {
                if (!IsDesignMode())
                {
                    ScrollTo(ScrollLeft, value);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ScrollLeft
        {
            get
            {
                return IsDesignMode()
                    ? 0
                    : Convert.ToInt32(
                        RunJs("return Math.max(document.documentElement.scrollLeft,document.body.scrollLeft)"));
            }
            set
            {
                if (!IsDesignMode())
                {
                    ScrollTo(value, ScrollTop);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ScrollHeight
        {
            get
            {
                return IsDesignMode()
                    ? 0
                    : Convert.ToInt32(
                        RunJs("return Math.max(document.documentElement.scrollHeight,document.body.scrollHeight)"));
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ScrollWidth
        {
            get
            {
                return IsDesignMode()
                    ? 0
                    : Convert.ToInt32(
                        RunJs("return Math.max(document.documentElement.scrollWidth,document.body.scrollWidth)"));
            }
        }

        private bool _memoryCacheEnable = true;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MemoryCacheEnable
        {
            get { return _memoryCacheEnable; }
            set
            {
                _memoryCacheEnable = value;

                if (!IsDesignMode())
                {
                    MBApi.wkeSetMemoryCacheEnable(MiniblinkHandle, _memoryCacheEnable);
                }
            }
        }

        private bool _headlessEnabled;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HeadlessEnabled
        {
            get { return _headlessEnabled; }
            set
            {
                _headlessEnabled = value;

                if (!IsDesignMode())
                {
                    MBApi.wkeSetHeadlessEnabled(MiniblinkHandle, _headlessEnabled);
                }
            }
        }

        private bool _npapiPluginsEnable = true;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool NpapiPluginsEnable
        {
            get { return _npapiPluginsEnable; }
            set
            {
                _npapiPluginsEnable = value;

                if (!IsDesignMode())
                {
                    MBApi.wkeSetNpapiPluginsEnabled(MiniblinkHandle, _npapiPluginsEnable);
                }
            }
        }

        private bool _cspCheckEnable;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CspCheckEnable
        {
            get { return _cspCheckEnable; }
            set
            {
                _cspCheckEnable = value;

                if (!IsDesignMode())
                {
                    MBApi.wkeSetCspCheckEnable(MiniblinkHandle, _cspCheckEnable);
                }
            }
        }

        private bool _touchEnabled;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool TouchEnabled
        {
            get { return _touchEnabled; }
            set
            {
                _touchEnabled = value;

                if (!IsDesignMode())
                {
                    MBApi.wkeSetTouchEnabled(MiniblinkHandle, _touchEnabled);
                }
            }
        }

        private bool _mouseEnabled = true;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MouseEnabled
        {
            get { return _mouseEnabled; }
            set
            {
                _mouseEnabled = value;

                if (!IsDesignMode())
                {
                    MBApi.wkeSetMouseEnabled(MiniblinkHandle, _mouseEnabled);
                }
            }
        }

        #endregion

        #region 事件

        public event EventHandler<PaintUpdatedEventArgs> PaintUpdated;

        private wkeDidCreateScriptContextCallback _wkeDidCreateScriptContextCallback;
        private EventHandler<DidCreateScriptContextEventArgs> _didCreateScriptContextCallback;

        public event EventHandler<DidCreateScriptContextEventArgs> DidCreateScriptContext
        {
            add
            {
                if (_wkeDidCreateScriptContextCallback == null)
                {
                    _wkeDidCreateScriptContextCallback = new wkeDidCreateScriptContextCallback(onWkeDidCreateScriptContextCallback);
                    MBApi.wkeOnDidCreateScriptContext(MiniblinkHandle, _wkeDidCreateScriptContextCallback, IntPtr.Zero);
                }

                _didCreateScriptContextCallback += value;
            }
            remove { _didCreateScriptContextCallback -= value; }
        }

        public event EventHandler<WindowOpenEventArgs> WindowOpen;
        public event EventHandler<EventArgs> Destroy;

        protected virtual WindowOpenEventArgs OnWindowOpen(string url, string name,
            IDictionary<string, string> specs, bool replace)
        {
            var args = new WindowOpenEventArgs
            {
                Name = name,
                Url = url,
                Replace = replace
            };
            if (specs != null)
            {
                foreach (var item in specs)
                {
                    args.Specs.Add(item);
                }
            }

            WindowOpen?.Invoke(this, args);

            if (args.LoadUrl && url != null)
            {
                LoadUri(url);
            }

            return args;
        }

        protected virtual void onWkeDidCreateScriptContextCallback(IntPtr webView, IntPtr param, IntPtr frame,
            IntPtr context, int extensionGroup, int worldId)
        {
            var e = new DidCreateScriptContextEventArgs
            {
                Frame = new FrameContext(this, frame)
            };
            _didCreateScriptContextCallback?.Invoke(this, e);
        }

        private wkeURLChangedCallback2 _wkeUrlChanged;
        private EventHandler<UrlChangedEventArgs> _urlChanged;

        public event EventHandler<UrlChangedEventArgs> UrlChanged
        {
            add
            {
                if (_wkeUrlChanged == null)
                {
                    MBApi.wkeOnURLChanged2(MiniblinkHandle, _wkeUrlChanged = new wkeURLChangedCallback2(OnUrlChanged),
                        IntPtr.Zero);
                }

                _urlChanged += value;
            }
            remove { _urlChanged -= value; }
        }

        protected virtual void OnUrlChanged(IntPtr mb, IntPtr param, IntPtr frame, IntPtr url)
        {
            _urlChanged?.Invoke(this, new UrlChangedEventArgs
            {
                Url = url.WKEToUTF8String(),
                Frame = new FrameContext(this, frame)
            });
        }

        private wkeNavigationCallback _wkeNavigateBefore;
        private EventHandler<NavigateEventArgs> _navigateBefore;

        public event EventHandler<NavigateEventArgs> NavigateBefore
        {
            add
            {
                if (_wkeNavigateBefore == null)
                {
                    MBApi.wkeOnNavigation(MiniblinkHandle,
                        _wkeNavigateBefore = new wkeNavigationCallback(OnNavigateBefore),
                        IntPtr.Zero);
                }

                _navigateBefore += value;
            }
            remove
            {
                _navigateBefore -= value;

                if (_navigateBefore == null)
                {
                    MBApi.wkeOnNavigation(MiniblinkHandle, null, IntPtr.Zero);
                }
            }
        }

        protected virtual byte OnNavigateBefore(IntPtr mb, IntPtr param, wkeNavigationType type, IntPtr url)
        {
            if (_navigateBefore == null)
                return 1;

            var e = new NavigateEventArgs
            {
                Url = url.WKEToUTF8String()
            };
            switch (type)
            {
                case wkeNavigationType.BackForward:
                    e.Type = NavigateType.BackForward;
                    break;
                case wkeNavigationType.FormReSubmit:
                    e.Type = NavigateType.ReSubmit;
                    break;
                case wkeNavigationType.FormSubmit:
                    e.Type = NavigateType.Submit;
                    break;
                case wkeNavigationType.LinkClick:
                    e.Type = NavigateType.LinkClick;
                    break;
                case wkeNavigationType.ReLoad:
                    e.Type = NavigateType.ReLoad;
                    break;
                case wkeNavigationType.Other:
                    e.Type = NavigateType.Other;
                    break;
                default:
                    throw new Exception("未知的重定向类型：" + type);
            }
            OnNavigateBefore(e);

            return (byte)(e.Cancel ? 0 : 1);
        }

        protected virtual void OnNavigateBefore(NavigateEventArgs args)
        {
            _navigateBefore(this, args);
        }

        private wkeDocumentReady2Callback _wkeDocumentReady;
        private EventHandler<DocumentReadyEventArgs> _documentReady;

        public event EventHandler<DocumentReadyEventArgs> DocumentReady
        {
            add
            {
                if (_wkeDocumentReady == null)
                {
                    MBApi.wkeOnDocumentReady2(MiniblinkHandle,
                        _wkeDocumentReady = new wkeDocumentReady2Callback(OnDocumentReady),
                        IntPtr.Zero);
                }

                _documentReady += value;
            }
            remove { _documentReady -= value; }
        }

        protected virtual void OnDocumentReady(IntPtr mb, IntPtr param, IntPtr frameId)
        {
            _documentReady?.Invoke(this, new DocumentReadyEventArgs
            {
                Frame = new FrameContext(this, frameId)
            });
        }

        private wkeConsoleCallback _wkeConsoleMessage;
        private EventHandler<ConsoleMessageEventArgs> _consoleMessage;

        public event EventHandler<ConsoleMessageEventArgs> ConsoleMessage
        {
            add
            {
                if (_wkeConsoleMessage == null)
                {
                    MBApi.wkeOnConsole(MiniblinkHandle, _wkeConsoleMessage = new wkeConsoleCallback(OnConsoleMessage),
                        IntPtr.Zero);
                }

                _consoleMessage += value;
            }
            remove
            {
                _consoleMessage -= value;

                if (_consoleMessage == null)
                {
                    MBApi.wkeOnConsole(MiniblinkHandle, null, IntPtr.Zero);
                }
            }
        }

        protected virtual void OnConsoleMessage(IntPtr mb, IntPtr param, wkeConsoleLevel level, IntPtr message,
            IntPtr sourceName, uint sourceLine, IntPtr stackTrace)
        {
            _consoleMessage?.Invoke(this, new ConsoleMessageEventArgs
            {
                Level = level,
                Message = message.WKEToUTF8String(),
                SourceLine = (int)sourceLine,
                SourceName = sourceName.WKEToUTF8String(),
                StackTrace = stackTrace.WKEToUTF8String()
            });
        }


        private wkeDownloadCallback _wkeDownload;
        private EventHandler<DownloadEventArgs> _download;

        public event EventHandler<DownloadEventArgs> Download
        {
            add
            {
                if (_wkeDownload == null)
                {
                    MBApi.wkeOnDownload(MiniblinkHandle, _wkeDownload = new wkeDownloadCallback(OnDownload),
                        IntPtr.Zero);
                }

                _download += value;
            }
            remove { _download -= value; }
        }

        public event EventHandler<AlertEventArgs> AlertBefore;

        protected virtual void OnAlertBefore(AlertEventArgs e)
        {
            AlertBefore?.Invoke(this, e);
        }

        public event EventHandler<ConfirmEventArgs> ConfirmBefore;

        protected virtual void OnConfirmBefore(ConfirmEventArgs e)
        {
            ConfirmBefore?.Invoke(this, e);
        }

        public event EventHandler<PromptEventArgs> PromptBefore;

        protected virtual void OnPromptBefore(PromptEventArgs e)
        {
            PromptBefore?.Invoke(this, e);
        }

        protected virtual byte OnDownload(IntPtr mb, IntPtr param, IntPtr url)
        {
            var downloader = new Downloader(this);
            var e = downloader.Create(url.ToUTF8String());
            _download?.Invoke(this, e);
            downloader.Execute(e);
            return 0;
        }

        private wkeLoadUrlBeginCallback _wkeLoadUrlBegin;
        private wkeNetResponseCallback _wkeNetResponse;
        private wkeLoadUrlEndCallback _wkeLoadUrlEnd;
        private wkeLoadUrlFailCallback _wkeLoadUrlFail;
        private EventHandler<RequestEventArgs> _requestBefore;

        public event EventHandler<RequestEventArgs> RequestBefore
        {
            add
            {
                if (_requestBefore == null)
                {
                    MBApi.wkeOnLoadUrlBegin(MiniblinkHandle,
                        _wkeLoadUrlBegin = new wkeLoadUrlBeginCallback(OnLoadUrlBegin),
                        IntPtr.Zero);
                    MBApi.wkeNetOnResponse(MiniblinkHandle, _wkeNetResponse = new wkeNetResponseCallback(OnNetResponse),
                        IntPtr.Zero);
                    MBApi.wkeOnLoadUrlEnd(MiniblinkHandle, _wkeLoadUrlEnd = new wkeLoadUrlEndCallback(OnLoadUrlEnd),
                        IntPtr.Zero);
                    MBApi.wkeOnLoadUrlFail(MiniblinkHandle, _wkeLoadUrlFail = new wkeLoadUrlFailCallback(OnLoadFail),
                        IntPtr.Zero);
                }

                _requestBefore += value;
            }
            remove { _requestBefore -= value; }
        }

        protected virtual bool OnLoadUrlBegin(IntPtr mb, IntPtr param, IntPtr url, IntPtr job)
        {
            var e = new RequestEventArgs(this, url.ToUTF8String(), job);

            if (_requestBefore != null)
            {
                _requestBefore?.Invoke(this, e);
                _requestMap.AddOrUpdate(job.ToInt64(), e, (k, v) => e);
                e.State = job.ToInt64();
                e.Finish += (fs, fe) =>
                {
                    var req = (RequestEventArgs)fs;
                    _requestMap.TryRemove((long)req.State, out req);
                };
            }
            return e.OnBegin();
        }

        protected virtual bool OnNetResponse(IntPtr mb, IntPtr param, string url, IntPtr job)
        {
            RequestEventArgs req;
            if (_requestMap.TryGetValue(job.ToInt64(), out req))
            {
                return req.OnNetData();
            }

            return false;
        }

        protected virtual void OnLoadFail(IntPtr mb, IntPtr param, IntPtr url, IntPtr job)
        {
            RequestEventArgs req;
            if (_requestMap.TryGetValue(job.ToInt64(), out req))
            {
                req.OnFail();
            }
        }

        private void OnLoadUrlEnd(IntPtr mb, IntPtr param, IntPtr url, IntPtr job, IntPtr buf, int length)
        {
            RequestEventArgs req;
            if (_requestMap.TryGetValue(job.ToInt64(), out req))
            {
                var data = new byte[length];
                if (buf != IntPtr.Zero)
                {
                    Marshal.Copy(buf, data, 0, length);
                }
                req.OnResponse(data);
            }
        }

        #endregion

        #region 公共方法

        public void SafeInvoke(Action<object> callback, object state = null)
        {
            if (InvokeRequired)
            {
                Invoke(callback, state);
            }
            else
            {
                callback(state);
            }
        }

        public void ShowDevTools()
        {
            var dir = Path.Combine(Application.StartupPath, "front_end");
            if (Directory.Exists(dir) == false)
            {
                var zipPath = Namespace + ".Files.front_end.zip";
                using (var sm = Assembly.GetExecutingAssembly().GetManifestResourceStream(zipPath))
                {
                    using (var zip = ZipFile.Read(sm))
                    {
                        zip.ExtractAll(Application.StartupPath);
                    }
                }
            }
            var path = Path.Combine(dir, "inspector.html");
            MBApi.wkeShowDevtools(MiniblinkHandle, path, null, IntPtr.Zero);
        }

        public object RunJs(string script)
        {
            var es = MBApi.wkeGlobalExec(MiniblinkHandle);
            return MBApi.jsEvalExW(es, script, true).ToValue(this, es);
        }

        public object CallJsFunc(string funcName, params object[] param)
        {
            var es = MBApi.wkeGlobalExec(MiniblinkHandle);
            var func = MBApi.jsGetGlobal(es, funcName);
            if (func == 0)
                throw new WKEFunctionNotFondException(funcName);
            var args = param.Select(i => i.ToJsValue(this, es)).ToArray();
            return MBApi.jsCall(es, func, MBApi.jsUndefined(), args, args.Length).ToValue(this, es);
        }

        public void BindNetFunc(NetFunc func, bool bindToSubFrame = false)
        {
            func.BindToSub = bindToSubFrame;
            _funcs.Add(func);

            if (_v8IsReady)
            {
                foreach (var f in _iframes)
                {
                    f.RunJs(BindFuncJs(false));
                }
                RunJs(BindFuncJs(true));
            }
        }

        public bool GoForward()
        {
            return MBApi.wkeGoForward(MiniblinkHandle);
        }

        public void EditorSelectAll()
        {
            MBApi.wkeEditorSelectAll(MiniblinkHandle);
        }

        public void EditorUnSelect()
        {
            MBApi.wkeEditorUnSelect(MiniblinkHandle);
        }

        public void EditorCopy()
        {
            MBApi.wkeEditorCopy(MiniblinkHandle);
        }

        public void EditorCut()
        {
            MBApi.wkeEditorCut(MiniblinkHandle);
        }

        public void EditorPaste()
        {
            MBApi.wkeEditorPaste(MiniblinkHandle);
        }

        public void EditorDelete()
        {
            MBApi.wkeEditorDelete(MiniblinkHandle);
        }

        public void EditorUndo()
        {
            MBApi.wkeEditorUndo(MiniblinkHandle);
        }

        public void EditorRedo()
        {
            MBApi.wkeEditorRedo(MiniblinkHandle);
        }

        public bool GoBack()
        {
            return MBApi.wkeGoBack(MiniblinkHandle);
        }

        public void SetProxy(WKEProxy proxy)
        {
            MBApi.wkeSetViewProxy(MiniblinkHandle, proxy);
        }

        public void LoadUri(string uri)
        {
            if (string.IsNullOrEmpty(uri?.Trim()))
                return;

            if (uri.SW("http:") || uri.SW("https:"))
            {
                MBApi.wkeLoadURL(MiniblinkHandle, uri);
            }
            else
            {
                MBApi.wkeLoadFileW(MiniblinkHandle, uri);
            }
        }

        public void LoadHtml(string html, string baseUrl = null)
        {
            if (baseUrl == null)
            {
                MBApi.wkeLoadHTML(MiniblinkHandle, html);
            }
            else
            {
                MBApi.wkeLoadHtmlWithBaseUrl(MiniblinkHandle, html, baseUrl);
            }
        }

        public void StopLoading()
        {
            MBApi.wkeStopLoading(MiniblinkHandle);
        }

        public void Reload()
        {
            MBApi.wkeReload(MiniblinkHandle);
        }

        #endregion

        private static string _popHookName = "func" + Guid.NewGuid().ToString().Replace("-", "");
        private static string _openHookName = "func" + Guid.NewGuid().ToString().Replace("-", "");
        private static string _callNet = "func" + Guid.NewGuid().ToString().Replace("-", "");
        private EventHandler<PaintUpdatedEventArgs> _browserPaintUpdated;
        private wkePaintBitUpdatedCallback _paintBitUpdated;
        private wkeCreateViewCallback _createView;
        private ConcurrentQueue<MouseEventArgs> _mouseMoveEvents = new ConcurrentQueue<MouseEventArgs>();
        private AutoResetEvent _mouseMoveAre = new AutoResetEvent(false);
        private bool _fiexdCursor;
        private bool _lockPaint;
        private ConcurrentDictionary<long, RequestEventArgs> _requestMap =
            new ConcurrentDictionary<long, RequestEventArgs>();
        private List<NetFunc> _funcs = new List<NetFunc>();
        private bool _v8IsReady;
        private List<FrameContext> _iframes = new List<FrameContext>();

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

                DeviceParameter = new DeviceParameter(this);
                Cookies = new CookieCollection(this, "cookies.dat");
                Task.Factory.StartNew(FireMouseMove);
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
                e.Response += (rs, re) =>
                {
                    ResourceCache.Save(re.Url, re.Data);
                };
            }
        }

        private IntPtr OnCreateView(IntPtr mb, IntPtr param, wkeNavigationType type, IntPtr url, IntPtr windowFeatures)
        {
            if (type == wkeNavigationType.LinkClick)
            {
                var e = new NavigateEventArgs
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

            return new IntPtr(1);
        }

        private void DestroyCallback()
        {
            if (!IsDesignMode())
            {
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
                Destroy?.Invoke(this, new EventArgs());
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            DestroyCallback();
            PaintUpdated = null;
            ResourceCache = null;
            ResourceLoader.Clear();
            _requestMap.Clear();
            _mouseMoveEvents = null;
            _mouseMoveAre.Dispose();
            _funcs.Clear();
            _iframes.Clear();
            MiniblinkSetting.DestroyWebView(MiniblinkHandle);
            base.OnHandleDestroyed(e);
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_lockPaint) return;

            if (!IsDesignMode() && !IsDisposed)
            {
                _lockPaint = true;
                using (var bitmap = DrawToBitmap())
                {
                    var rect = new RectangleF(e.ClipRectangle.X, e.ClipRectangle.Y,
                        e.ClipRectangle.Width, e.ClipRectangle.Height);
                    e.Graphics.DrawImage(bitmap, rect, rect, GraphicsUnit.Pixel);
                }

                _lockPaint = false;
            }
        }

        private void OnWkeOnPaintBitUpdated(IntPtr webView, IntPtr param, IntPtr buf, IntPtr rectPtr, int width, int height)
        {
            if (buf == IntPtr.Zero || _lockPaint) return;
            _lockPaint = true;
            var stride = width * 4 + width * 4 % 4;
            var rect = (wkeRect)Marshal.PtrToStructure(rectPtr, typeof(wkeRect));
            using (var view = new Bitmap(width, height, stride, PixelFormat.Format32bppPArgb, buf))
            {
                var e = new PaintUpdatedEventArgs
                {
                    WebView = webView,
                    Param = param,
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

            _lockPaint = false;
        }

        private void BrowserPaintUpdated(object sender, PaintUpdatedEventArgs e)
        {
            if (!IsDisposed)
            {
                using (var g = CreateGraphics())
                {
                    var rect = new RectangleF(e.Rect.X, e.Rect.Y, e.Rect.Width, e.Rect.Height);
                    g.DrawImage(e.Image, rect, rect, GraphicsUnit.Pixel);
                }
            }
        }

        public void ScrollTo(int x, int y)
        {
            if (IsDocumentReady)
            {
                RunJs($"window.scrollTo({x},{y})");
            }
        }

        public void RegisterNetFunc(object target)
        {
            var tg = target;
            var methods = tg.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<NetFuncAttribute>();
                if (attr == null) continue;
                BindNetFunc(new NetFunc(attr.Name ?? method.Name, ctx =>
                {
                    var m = (MethodInfo)ctx.State;
                    object ret;
                    var mps = m.GetParameters();
                    if (mps.Length < 1)
                    {
                        ret = m.Invoke(tg, null);
                    }
                    else
                    {
                        var param = ctx.Paramters;
                        var mpvs = new object[mps.Length];
                        for (var i = 0; i < mps.Length; i++)
                        {
                            var mp = mps[i];
                            var v = param.Length > i ? param[i] : null;
                            if (v != null)
                            {
                                var pt = mp.ParameterType;
                                if (pt.IsGenericType)
                                {
                                    if (pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    {
                                        pt = pt.GetGenericArguments().First();
                                    }
                                }
                                if (pt == typeof(DateTime) && !(v is DateTime))
                                {
                                    long l_date;
                                    if (long.TryParse(v.ToString(), out l_date))
                                    {
                                        v = l_date.ToDate();
                                    }
                                }
                                if (v is JsFunc || pt == typeof(object) || pt == typeof(ExpandoObject))
                                {
                                    mpvs[i] = v;
                                }
                                else
                                {
                                    mpvs[i] = Convert.ChangeType(v, pt);
                                }
                            }
                            else if (mp.ParameterType.IsValueType)
                            {
                                mpvs[i] = Activator.CreateInstance(mp.ParameterType);
                            }
                            else
                            {
                                mpvs[i] = null;
                            }
                        }

                        ret = m.Invoke(tg, mpvs);
                    }

                    return ret;
                }, method), attr.BindToSubFrame);
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

        public void DrawToBitmap(Action<ScreenshotImage> callback)
        {
            new DrawToBitmapUtil(this).ToImage(callback);
        }

        public Bitmap DrawToBitmap(Rectangle? rect = null)
        {
            if (rect.HasValue == false)
            {
                rect = new Rectangle(0, 0, ViewWidth, ViewHeight);
            }

            using (var image = new Bitmap(ViewWidth, ViewHeight))
            {
                var bitmap = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                MBApi.wkePaint(MiniblinkHandle, bitmap.Scan0, 0);
                image.UnlockBits(bitmap);
                return image.Clone(rect.Value, PixelFormat.Format32bppArgb);
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
            var js = string.Join(".", typeof(MiniblinkBrowser).Namespace, "Files", "browser.js");

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
            var navArgs = new NavigateEventArgs
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

        public void Print(Action<PrintPreviewDialog> callback)
        {
            new PrintUtil(this).Start(callback);
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

        internal bool IsDesignMode()
        {
            var returnFlag = false;

#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                returnFlag = true;
            }
            else if (Process.GetCurrentProcess().ProcessName == "devenv")
            {
                returnFlag = true;
            }
#endif

            return returnFlag || DesignMode;
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

        private void FireMouseMove()
        {
            while (_mouseMoveAre != null)
            {
                _mouseMoveAre.WaitOne();

                MouseEventArgs e;
                while (_mouseMoveEvents != null && _mouseMoveEvents.TryDequeue(out e))
                {
                    SafeInvoke(s =>
                    {
                        OnWkeMouseEvent(WinConst.WM_MOUSEMOVE, (MouseEventArgs)s);
                        base.OnMouseMove((MouseEventArgs)s);
                    }, e);
                }

                _mouseMoveAre?.Reset();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (MouseMoveOptimize)
            {
                _mouseMoveEvents.Enqueue(e);
                _mouseMoveAre.Set();
            }
            else
            {
                OnWkeMouseEvent(WinConst.WM_MOUSEMOVE, e);
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

                case WinConst.WM_SETCURSOR:
                    {
                        if (_fiexdCursor == false)
                        {
                            SetWkeCursor();
                            base.WndProc(ref m);
                        }

                        break;
                    }

                default:
                    {
                        base.WndProc(ref m);
                        break;
                    }
            }
        }

        #endregion
    }
}
