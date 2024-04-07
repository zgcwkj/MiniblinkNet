using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MiniblinkNet.MiniBlink
{
    public interface IMiniblink
    {
        IntPtr MiniblinkHandle { get; }
        string Url { get; }
        bool IsDocumentReady { get; }
        string DocumentTitle { get; }
        int DocumentWidth { get; }
        int DocumentHeight { get; }
        int ContentWidth { get; }
        int ContentHeight { get; }
        int ViewWidth { get; }
        int ViewHeight { get; }
        int ScrollTop { get; set; }
        int ScrollLeft { get; set; }
        int ScrollHeight { get; }
        int ScrollWidth { get; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        float Zoom { get; set; }
        string UserAgent { get; }
        DeviceParameter DeviceParameter { get; }
        CookieCollection Cookies { get; }
        /// <summary>
        /// 是否在接收到拖放的文件时触发window的dropFile事件
        /// </summary>
        bool FireDropFile { get; set; }
        /// <summary>
        /// 资源加载的Handler集合，一个请求发起前先从此集合中尝试加载资源，无法加载才发起真实请求。
        /// </summary>
        IList<ILoadResource> ResourceLoader { get; }
        /// <summary>
        /// 资源缓存
        /// </summary>
        IResourceCache ResourceCache { get; set; }
        /// <summary>
        /// 是否启用内存缓存
        /// </summary>
        bool MemoryCacheEnable { get; set; }
        /// <summary>
        /// 是否启用渲染
        /// </summary>
        bool HeadlessEnabled { get; set; }
        /// <summary>
        /// 是否启用npapi
        /// </summary>
        bool NpapiPluginsEnable { get; set; }
        /// <summary>
        /// 是否启用跨域检查
        /// </summary>
        bool CspCheckEnable { get; set; }
        /// <summary>
        /// 是否启用触摸事件
        /// </summary>
        bool TouchEnabled { get; set; }
        /// <summary>
        /// 是否启用鼠标事件
        /// </summary>
        bool MouseEnabled { get; set; }

        event EventHandler<UrlChangedEventArgs> UrlChanged;
        event EventHandler<NavigateEventArgs> NavigateBefore;
        event EventHandler<DocumentReadyEventArgs> DocumentReady;
        event EventHandler<ConsoleMessageEventArgs> ConsoleMessage;
        event EventHandler<RequestEventArgs> RequestBefore;
        event EventHandler<PaintUpdatedEventArgs> PaintUpdated;
        event EventHandler<DownloadEventArgs> Download;
        event EventHandler<AlertEventArgs> AlertBefore;
        event EventHandler<ConfirmEventArgs> ConfirmBefore;
        event EventHandler<PromptEventArgs> PromptBefore;
        event EventHandler<DidCreateScriptContextEventArgs> DidCreateScriptContext;
        event EventHandler<WindowOpenEventArgs> WindowOpen;
        event EventHandler<EventArgs> Destroy;

        void ScrollTo(int x, int y);
        void RegisterJsFunc(object target);
        void ShowDevTools();
        object RunJs(string script);
        object CallJsFunc(string funcName, params object[] param);
        void BindNetFunc(NetFunc func, bool bindToSubFrame = false);
        bool GoForward();
        bool GoBack();
        void SetProxy(WKEProxy proxy);
        void LoadUri(string uri);
        void LoadHtml(string html, string baseUrl = null);
        void StopLoading();
        void Reload();
        void DrawToBitmap(Action<ScreenshotImage> callback);
        void Print(Action<PrintPreviewDialog> callback);
        void SafeInvoke(Action<object> callback, object state = null);
    }
}
