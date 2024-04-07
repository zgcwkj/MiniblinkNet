using MiniblinkNet.MiniBlink;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MiniblinkNet
{
    public partial class MiniblinkBrowser
    {
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

        /// <summary>
        /// 是否使用位图绘屏模式
        /// 位图绘屏性能更差但是兼容性更高
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool BmpPaintMode
        {
            get;
            set;
        }
    }
}
