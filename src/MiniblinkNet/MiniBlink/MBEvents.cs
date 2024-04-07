using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MiniblinkNet.MiniBlink
{
    public class MiniblinkEventArgs : EventArgs
    {
        internal MiniblinkEventArgs()
        {
        }
    }

    public class UrlChangedEventArgs : MiniblinkEventArgs
    {
        public string Url { get; internal set; }
        public FrameContext Frame { get; internal set; }

        internal UrlChangedEventArgs()
        {
        }
    }

    public class NavigateEventArgs : MiniblinkEventArgs
    {
        public string Url { get; internal set; }
        public NavigateType Type { get; internal set; }
        public bool Cancel { get; set; }

        internal NavigateEventArgs()
        {
        }
    }

    public class DocumentReadyEventArgs : MiniblinkEventArgs
    {
        public FrameContext Frame { get; internal set; }

        internal DocumentReadyEventArgs()
        {
        }
    }

    public class ConsoleMessageEventArgs : MiniblinkEventArgs
    {
        public wkeConsoleLevel Level { get; internal set; }
        public string Message { get; internal set; }
        public string SourceName { get; internal set; }
        public int SourceLine { get; internal set; }
        public string StackTrace { get; internal set; }

        internal ConsoleMessageEventArgs()
        {
        }
    }

    public class PaintUpdatedEventArgs : MiniblinkEventArgs
    {
        public IntPtr WebView { get; internal set; }
        public IntPtr Param { get; internal set; }
        public Bitmap Image { get; internal set; }
        public Rectangle Rect { get; internal set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public bool Cancel { get; set; }

        internal PaintUpdatedEventArgs()
        {
        }
    }

    public class DownloadEventArgs : MiniblinkEventArgs
    {
        public string Url { get; }
        public long FileLength { get; internal set; }
        public event EventHandler<DownloadProgressEventArgs> Progress;
        public event EventHandler<DownloadFinishEventArgs> Finish;
        public bool Cancel { get; set; }
        public string FilePath { get; set; }
        internal WebResponse Response;

        internal DownloadEventArgs(string url)
        {
            Url = url;
        }

        internal void OnProgress(DownloadProgressEventArgs e)
        {
            Progress?.Invoke(this, e);
        }

        internal void OnFinish(DownloadFinishEventArgs e)
        {
            Finish?.Invoke(this, e);
        }
    }

    public class DownloadFinishEventArgs : EventArgs
    {
        public Exception Error { get; internal set; }
        public bool IsCompleted { get; internal set; }
    }

    public class DownloadProgressEventArgs : EventArgs
    {
        public long Total { get; internal set; }
        public long Received { get; internal set; }
        public byte[] Data { get; internal set; }
        public bool Cancel { get; set; }
    }

    public class AlertEventArgs : EventArgs
    {
        public FrmAlert Window { get; set; }

        internal AlertEventArgs()
        {
        }
    }

    public class ConfirmEventArgs : MiniblinkEventArgs
    {
        public FrmConfirm Window { get; set; }
        public bool? Result { get; set; }

        internal ConfirmEventArgs()
        {
        }
    }

    public class PromptEventArgs : MiniblinkEventArgs
    {
        public FrmPrompt Window { get; set; }
        public string Result { get; set; }

        internal PromptEventArgs()
        {
        }
    }

    public class DidCreateScriptContextEventArgs : MiniblinkEventArgs
    {
        public FrameContext Frame { get; internal set; }

        internal DidCreateScriptContextEventArgs()
        {
        }
    }

    public class WindowOpenEventArgs : MiniblinkEventArgs
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public IDictionary<string, string> Specs { get; private set; }
        public bool Replace { get; set; }
        public string ReturnValue { get; set; }
        public bool LoadUrl { get; set; }

        internal WindowOpenEventArgs()
        {
            Specs = new Dictionary<string, string>();
            LoadUrl = true;
        }
    }

    public class RequestAsync
    {
        public RequestEventArgs Request { get; }
        public object State { get; }

        internal string ContentType { get; private set; }
        internal byte[] Data { get; private set; }

        internal RequestAsync(RequestEventArgs request, object state)
        {
            Request = request;
            State = state;
        }

        public void SetData(byte[] data)
        {
            Data = data;
        }

        public void SetContentType(string contentType)
        {
            ContentType = contentType;
        }
    }

    public class NetDataEventArgs : MiniblinkEventArgs
    {
        public bool Cancel { get; set; }

        public string Url
        {
            get { return _request.Url; }
        }

        public string Method
        {
            get { return _request.Method; }
        }

        public string ContentType
        {
            get { return MBApi.wkeNetGetMIMEType(_request.NetJob).ToUTF8String(); }
            set { MBApi.wkeNetSetMIMEType(_request.NetJob, value); }
        }

        internal byte[] Data { get; private set; }
        private RequestEventArgs _request;

        internal NetDataEventArgs(RequestEventArgs request)
        {
            _request = request;
        }

        public void SetData(byte[] data)
        {
            Data = data;
        }

        public IDictionary<string, string> GetHeaders()
        {
            var ptr = MBApi.wkeNetGetRawResponseHead(_request.NetJob);
            var slist = (wkeSlist)Marshal.PtrToStructure(ptr, typeof(wkeSlist));
            return slist.ToDict();
        }
    }

    public class ResponseEventArgs : MiniblinkEventArgs
    {
        internal bool IsSetData { get; private set; }

        private byte[] _data;

        public byte[] Data
        {
            get { return _data; }
            set
            {
                _data = value;
                IsSetData = true;
            }
        }

        public string Url
        {
            get { return _request.Url; }
        }

        public string Method
        {
            get { return _request.Method; }
        }

        public string ContentType
        {
            get { return MBApi.wkeNetGetMIMEType(_request.NetJob).ToUTF8String(); }
            set { MBApi.wkeNetSetMIMEType(_request.NetJob, value); }
        }

        private RequestEventArgs _request;

        internal ResponseEventArgs(RequestEventArgs request, byte[] data)
        {
            _data = data;
            _request = request;
        }

        public IDictionary<string, string> GetHeaders()
        {
            var ptr = MBApi.wkeNetGetRawResponseHead(_request.NetJob);
            var slist = (wkeSlist)Marshal.PtrToStructure(ptr, typeof(wkeSlist));
            return slist.ToDict();
        }
    }

    public class RequestEventArgs : MiniblinkEventArgs
    {
        private enum Status
        {
            Before,
            Async,
            Post,
            Net,
            Valid
        }

        public string Url { get; private set; }
        public string Method { get; }
        public bool Cancel { get; set; }
        public byte[] Data { get; set; }

        /// <summary>
        /// 接收到网络数据时
        /// </summary>
        public event EventHandler<NetDataEventArgs> NetData;

        /// <summary>
        /// 加载失败时
        /// </summary>
        public event EventHandler<EventArgs> LoadFail;

        /// <summary>
        /// 请求内容最终呈现之前
        /// </summary>
        public event EventHandler<ResponseEventArgs> Response;

        /// <summary>
        /// 请求结束
        /// </summary>
        public event EventHandler<EventArgs> Finish;

        internal IMiniblink Miniblink { get; }
        internal object State { get; set; }
        internal IntPtr NetJob { get; }
        private Status _status;
        private MBPostBody _body;

        internal RequestEventArgs(IMiniblink miniblink, string url, IntPtr job)
        {
            var type = MBApi.wkeNetGetRequestMethod(job);
            switch (type)
            {
                case wkeRequestType.Get:
                    Method = "GET";
                    break;
                case wkeRequestType.Post:
                    Method = "POST";
                    break;
                case wkeRequestType.Put:
                    Method = "PUT";
                    break;
                default:
                    Method = "UNKNOW";
                    break;
            }

            Url = url;
            Miniblink = miniblink;
            NetJob = job;
            _status = Status.Before;
            var idx = url.IndexOf("#", StringComparison.Ordinal);
            if (idx > 0)
            {
                ResetUrl(url.Substring(0, idx));
            }
        }

        public void Async(Action<RequestAsync> callback, object state = null)
        {
            MBApi.wkeNetHoldJobToAsynCommit(NetJob);
            _status = Status.Async;
            Task.Factory.StartNew(s =>
            {
                var ps = (object[])s;
                var req = (RequestEventArgs)ps[0];
                var pm = ps[1];
                var e = new RequestAsync(req, pm);
                try
                {
                    callback(e);
                }
                finally
                {
                    req.Miniblink.SafeInvoke(_ =>
                    {
                        if (e.ContentType != null)
                        {
                            MBApi.wkeNetSetMIMEType(req.NetJob, e.ContentType);
                        }

                        if (e.Data != null)
                        {
                            req.SetData(e.Data);
                        }

                        MBApi.wkeNetContinueJob(req.NetJob);

                        var t = req.NetData?.GetInvocationList().Length;
                        t += req.LoadFail?.GetInvocationList().Length;
                        t += req.Response?.GetInvocationList().Length;
                        if (t == 0)
                        {
                            req._status = Status.Valid;
                            Finish?.Invoke(req, new EventArgs());
                        }
                        else
                        {
                            req._status = Status.Post;
                        }
                    });
                }
            }, new[] { this, state });
        }

        public MBPostBody GetPostBody()
        {
            if ("post".SW(Method) && _body == null)
            {
                _body = new MBPostBody(NetJob);
            }

            return _body;
        }

        public void SetHeader(string name, string value)
        {
            MBApi.wkeNetSetHTTPHeaderField(NetJob, name, value);
        }

        public void ResetUrl(string url)
        {
            MBApi.wkeNetChangeRequestUrl(NetJob, url);
            Url = url;
        }

        private void SetData(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                MBApi.wkeNetSetData(NetJob, data, data.Length);
            }
            else
            {
                MBApi.wkeNetSetData(NetJob, new byte[] { 0 }, 1);
            }
        }

        internal bool OnBegin()
        {
            if (_status == Status.Async)
            {
                Cancel = false;
                return Cancel;
            }

            if (_status == Status.Before && Data != null)
            {
                SetData(Data);
                Cancel = true;
                _status = Status.Valid;
            }
            else if (Response?.GetInvocationList().Length > 0)
            {
                MBApi.wkeNetHookRequest(NetJob);
                Cancel = false;
            }

            if (Cancel)
            {
                MBApi.wkeNetCancelRequest(NetJob);

                if (Data != null)
                {
                    OnResponse(Data);
                }
                else
                {
                    Finish?.Invoke(this, new EventArgs());
                }
            }
            else
            {
                _status = Status.Post;
            }

            return Cancel;
        }

        internal bool OnNetData()
        {
            _status = Status.Net;
            var cancel = false;

            if (NetData != null)
            {
                var e = new NetDataEventArgs(this);
                NetData?.Invoke(this, e);

                if (e.Data != null)
                {
                    SetData(e.Data);
                }

                cancel = e.Cancel;
            }

            if (Response == null || Response.GetInvocationList().Length == 0)
            {
                Finish?.Invoke(this, new EventArgs());
            }

            return cancel;
        }

        internal void OnFail()
        {
            _status = Status.Net;

            LoadFail?.Invoke(this, new EventArgs());

            if (Response == null || Response.GetInvocationList().Length == 0)
            {
                Finish?.Invoke(this, new EventArgs());
            }
        }

        internal void OnResponse(byte[] data)
        {
            _status = Status.Valid;

            if (Response != null)
            {
                var e = new ResponseEventArgs(this, data);
                Response?.Invoke(this, e);
                if (e.IsSetData)
                {
                    SetData(e.Data);
                }
            }

            Finish?.Invoke(this, new EventArgs());
        }
    }
}