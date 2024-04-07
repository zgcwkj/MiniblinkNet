using MiniblinkNet.MiniBlink;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MiniblinkNet
{
    public partial class MiniblinkBrowser
    {
        public event EventHandler<PaintUpdatedEventArgs> PaintUpdated;

        private wkeDidCreateScriptContextCallback _wkeDidCreateScriptContextCallback;
        private EventHandler<DidCreateScriptContextEventArgs> _didCreateScriptContextCallback;

        public event EventHandler<DidCreateScriptContextEventArgs> DidCreateScriptContext
        {
            add
            {
                if (_wkeDidCreateScriptContextCallback == null)
                {
                    _wkeDidCreateScriptContextCallback = new wkeDidCreateScriptContextCallback(OnWkeDidCreateScriptContextCallback);
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

        protected virtual void OnWkeDidCreateScriptContextCallback(IntPtr webView, IntPtr param, IntPtr frame,
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
                    MBApi.wkeOnConsole(MiniblinkHandle,
                        _wkeConsoleMessage = new wkeConsoleCallback(OnConsoleMessage),
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
                    MBApi.wkeOnDownload(MiniblinkHandle,
                        _wkeDownload = new wkeDownloadCallback(OnDownload),
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
                if (_wkeLoadUrlBegin == null)
                {
                    MBApi.wkeOnLoadUrlBegin(MiniblinkHandle,
                        _wkeLoadUrlBegin = new wkeLoadUrlBeginCallback(OnLoadUrlBegin),
                        IntPtr.Zero);
                    MBApi.wkeNetOnResponse(MiniblinkHandle,
                        _wkeNetResponse = new wkeNetResponseCallback(OnNetResponse),
                        IntPtr.Zero);
                    MBApi.wkeOnLoadUrlEnd(MiniblinkHandle,
                        _wkeLoadUrlEnd = new wkeLoadUrlEndCallback(OnLoadUrlEnd),
                        IntPtr.Zero);
                    MBApi.wkeOnLoadUrlFail(MiniblinkHandle,
                        _wkeLoadUrlFail = new wkeLoadUrlFailCallback(OnLoadFail),
                        IntPtr.Zero);
                }

                _requestBefore += value;
            }
            remove { _requestBefore -= value; }
        }

        protected virtual bool OnLoadUrlBegin(IntPtr mb, IntPtr param, IntPtr url, IntPtr job)
        {
            if (_wkeLoadUrlBegin == null)
                return false;

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
            if (_wkeNetResponse == null)
                return false;

            RequestEventArgs req;
            if (_requestMap.TryGetValue(job.ToInt64(), out req))
            {
                return req.OnNetData();
            }

            return false;
        }

        protected virtual void OnLoadFail(IntPtr mb, IntPtr param, IntPtr url, IntPtr job)
        {
            if (_wkeLoadUrlFail == null)
                return;

            RequestEventArgs req;
            if (_requestMap.TryGetValue(job.ToInt64(), out req))
            {
                req.OnFail();
            }
        }

        private void OnLoadUrlEnd(IntPtr mb, IntPtr param, IntPtr url, IntPtr job, IntPtr buf, int length)
        {
            if (_wkeLoadUrlEnd == null)
                return;

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
    }
}
