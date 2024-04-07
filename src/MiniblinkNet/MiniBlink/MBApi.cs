using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MiniblinkNet.MiniBlink
{
    internal class MBApi
    {
        private static bool isAutoFile { get; set; } = false;
        private static bool is64 { get; set; } = IntPtr.Size == 8;

        private const string DLL_auto = "miniblink.dll";
        private const string DLL_x86 = "miniblink_x86.dll";
        private const string DLL_x64 = "miniblink_x64.dll";

        static MBApi()
        {
            var runPath = AppDomain.CurrentDomain.BaseDirectory;
            isAutoFile = File.Exists($"{runPath}/{DLL_auto}");
            //
            wkeInitialize();
        }

        [DllImport(DLL_auto, EntryPoint = "wkeInitialize", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeInitialize_auto();

        [DllImport(DLL_x86, EntryPoint = "wkeInitialize", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeInitialize_x86();

        [DllImport(DLL_x64, EntryPoint = "wkeInitialize", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeInitialize_x64();

        public static void wkeInitialize()
        {
            if (isAutoFile)
            {
                wkeInitialize_auto();
            }
            else if (is64)
            {
                wkeInitialize_x64();
            }
            else
            {
                wkeInitialize_x86();
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeDestroyWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeDestroyWebView_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeDestroyWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeDestroyWebView_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeDestroyWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeDestroyWebView_x64(IntPtr webView);

        public static void wkeDestroyWebView(IntPtr webView)
        {
            if (isAutoFile)
            {
                wkeInitialize_auto();
            }
            else if (is64)
            {
                wkeDestroyWebView_x64(webView);
            }
            else
            {
                wkeDestroyWebView_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetTouchEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetTouchEnabled_auto(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool enable);

        [DllImport(DLL_x86, EntryPoint = "wkeSetTouchEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetTouchEnabled_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool enable);

        [DllImport(DLL_x64, EntryPoint = "wkeSetTouchEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetTouchEnabled_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool enable);

        public static void wkeSetTouchEnabled(IntPtr webView, bool enable)
        {
            if (isAutoFile)
            {
                wkeSetTouchEnabled_auto(webView, enable);
            }
            else if (is64)
            {
                wkeSetTouchEnabled_x64(webView, enable);
            }
            else
            {
                wkeSetTouchEnabled_x86(webView, enable);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetMouseEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetMouseEnabled_auto(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool enable);

        [DllImport(DLL_x86, EntryPoint = "wkeSetMouseEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetMouseEnabled_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool enable);

        [DllImport(DLL_x64, EntryPoint = "wkeSetMouseEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetMouseEnabled_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool enable);

        public static void wkeSetMouseEnabled(IntPtr webView, bool enable)
        {
            if (isAutoFile)
            {
                wkeSetMouseEnabled_auto(webView, enable);
            }
            else if (is64)
            {
                wkeSetMouseEnabled_x64(webView, enable);
            }
            else
            {
                wkeSetMouseEnabled_x86(webView, enable);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetDeviceParameter", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeSetDeviceParameter_auto(IntPtr webView, string device, string s, int i, float f);

        [DllImport(DLL_x86, EntryPoint = "wkeSetDeviceParameter", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeSetDeviceParameter_x86(IntPtr webView, string device, string s, int i, float f);

        [DllImport(DLL_x64, EntryPoint = "wkeSetDeviceParameter", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeSetDeviceParameter_x64(IntPtr webView, string device, string s, int i, float f);

        public static void wkeSetDeviceParameter(IntPtr webView, string type, string s, int i, float f)
        {
            if (isAutoFile)
            {
                wkeSetDeviceParameter_auto(webView, type, s, i, f);
            }
            else if (is64)
            {
                wkeSetDeviceParameter_x64(webView, type, s, i, f);
            }
            else
            {
                wkeSetDeviceParameter_x86(webView, type, s, i, f);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeCreateWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeCreateWebView_auto();

        [DllImport(DLL_x86, EntryPoint = "wkeCreateWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeCreateWebView_x86();

        [DllImport(DLL_x64, EntryPoint = "wkeCreateWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeCreateWebView_x64();

        public static IntPtr wkeCreateWebView()
        {
            if (isAutoFile)
            {
                return wkeCreateWebView_auto();
            }
            else if (is64)
            {
                return wkeCreateWebView_x64();
            }

            return wkeCreateWebView_x86();
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetNavigationToNewWindowEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetNavigationToNewWindowEnable_auto(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x86, EntryPoint = "wkeSetNavigationToNewWindowEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetNavigationToNewWindowEnable_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x64, EntryPoint = "wkeSetNavigationToNewWindowEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetNavigationToNewWindowEnable_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetNavigationToNewWindowEnable(IntPtr webView, bool enable)
        {
            if (isAutoFile)
            {
                wkeSetNavigationToNewWindowEnable_auto(webView, enable);
            }
            else if (is64)
            {
                wkeSetNavigationToNewWindowEnable_x64(webView, enable);
            }
            else
            {
                wkeSetNavigationToNewWindowEnable_x86(webView, enable);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetNpapiPluginsEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetNpapiPluginsEnabled_auto(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x86, EntryPoint = "wkeSetNpapiPluginsEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetNpapiPluginsEnabled_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x64, EntryPoint = "wkeSetNpapiPluginsEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetNpapiPluginsEnabled_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetNpapiPluginsEnabled(IntPtr webView, bool enable)
        {
            if (isAutoFile)
            {
                wkeSetNpapiPluginsEnabled_auto(webView, enable);
            }
            else if (is64)
            {
                wkeSetNpapiPluginsEnabled_x64(webView, enable);
            }
            else
            {
                wkeSetNpapiPluginsEnabled_x86(webView, enable);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetHeadlessEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetHeadlessEnabled_auto(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x86, EntryPoint = "wkeSetHeadlessEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetHeadlessEnabled_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x64, EntryPoint = "wkeSetHeadlessEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetHeadlessEnabled_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetHeadlessEnabled(IntPtr webView, bool enable)
        {
            if (isAutoFile)
            {
                wkeSetHeadlessEnabled_auto(webView, enable);
            }
            else if (is64)
            {
                wkeSetHeadlessEnabled_x64(webView, enable);
            }
            else
            {
                wkeSetHeadlessEnabled_x86(webView, enable);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetCspCheckEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetCspCheckEnable_auto(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x86, EntryPoint = "wkeSetCspCheckEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetCspCheckEnable_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x64, EntryPoint = "wkeSetCspCheckEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetCspCheckEnable_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetCspCheckEnable(IntPtr webView, bool enable)
        {
            if (isAutoFile)
            {
                wkeSetCspCheckEnable_auto(webView, enable);
            }
            else if (is64)
            {
                wkeSetCspCheckEnable_x64(webView, enable);
            }
            else
            {
                wkeSetCspCheckEnable_x86(webView, enable);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetMemoryCacheEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetMemoryCacheEnable_auto(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x86, EntryPoint = "wkeSetMemoryCacheEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetMemoryCacheEnable_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x64, EntryPoint = "wkeSetMemoryCacheEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetMemoryCacheEnable_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetMemoryCacheEnable(IntPtr webView, bool enable)
        {
            if (isAutoFile)
            {
                wkeSetMemoryCacheEnable_auto(webView, enable);
            }
            else if (is64)
            {
                wkeSetMemoryCacheEnable_x64(webView, enable);
            }
            else
            {
                wkeSetMemoryCacheEnable_x86(webView, enable);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetContextMenuEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetContextMenuEnabled_auto(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x86, EntryPoint = "wkeSetContextMenuEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetContextMenuEnabled_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x64, EntryPoint = "wkeSetContextMenuEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetContextMenuEnabled_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetContextMenuEnabled(IntPtr webView, bool enable)
        {
            if (isAutoFile)
            {
                wkeSetContextMenuEnabled_auto(webView, enable);
            }
            else if (is64)
            {
                wkeSetContextMenuEnabled_x64(webView, enable);
            }
            else
            {
                wkeSetContextMenuEnabled_x86(webView, enable);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetViewProxy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetViewProxy_auto(IntPtr webView, ref WKEProxy proxy);

        [DllImport(DLL_x86, EntryPoint = "wkeSetViewProxy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetViewProxy_x86(IntPtr webView, ref WKEProxy proxy);

        [DllImport(DLL_x64, EntryPoint = "wkeSetViewProxy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetViewProxy_x64(IntPtr webView, ref WKEProxy proxy);

        public static void wkeSetViewProxy(IntPtr webView, WKEProxy proxy)
        {
            if (isAutoFile)
            {
                wkeSetViewProxy_auto(webView, ref proxy);
            }
            else if (is64)
            {
                wkeSetViewProxy_x64(webView, ref proxy);
            }
            else
            {
                wkeSetViewProxy_x86(webView, ref proxy);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetHandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetHandle_auto(IntPtr webView, IntPtr handle);

        [DllImport(DLL_x86, EntryPoint = "wkeSetHandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetHandle_x86(IntPtr webView, IntPtr handle);

        [DllImport(DLL_x64, EntryPoint = "wkeSetHandle", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetHandle_x64(IntPtr webView, IntPtr handle);

        public static void wkeSetHandle(IntPtr webView, IntPtr handle)
        {
            if (isAutoFile)
            {
                wkeSetHandle_auto(webView, handle);
            }
            else if (is64)
            {
                wkeSetHandle_x64(webView, handle);
            }
            else
            {
                wkeSetHandle_x86(webView, handle);
            }
        }

        //[DllImport(DLL_x86, EntryPoint = "wkeSetHandleOffset", CallingConvention = CallingConvention.Cdecl)]
        //public static extern void wkeSetHandleOffset(IntPtr webView, int x, int y);

        [DllImport(DLL_auto, EntryPoint = "wkeSetTransparent", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetTransparent_auto(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x86, EntryPoint = "wkeSetTransparent", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetTransparent_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x64, EntryPoint = "wkeSetTransparent", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetTransparent_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetTransparent(IntPtr webView, bool enable)
        {
            if (isAutoFile)
            {
                wkeSetTransparent_auto(webView, enable);
            }
            else if (is64)
            {
                wkeSetTransparent_x64(webView, enable);
            }
            else
            {
                wkeSetTransparent_x86(webView, enable);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetDragEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetDragEnable_auto(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x86, EntryPoint = "wkeSetDragEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetDragEnable_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x64, EntryPoint = "wkeSetDragEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetDragEnable_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetDragEnable(IntPtr webView, bool enable)
        {
            if (isAutoFile)
            {
                wkeSetDragEnable_auto(webView, enable);
            }
            else if (is64)
            {
                wkeSetDragEnable_x64(webView, enable);
            }
            else
            {
                wkeSetDragEnable_x86(webView, enable);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetDragDropEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetDragDropEnable_auto(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x86, EntryPoint = "wkeSetDragDropEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetDragDropEnable_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x64, EntryPoint = "wkeSetDragDropEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetDragDropEnable_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetDragDropEnable(IntPtr webView, bool enable)
        {
            if (isAutoFile)
            {
                wkeSetDragDropEnable_auto(webView, enable);
            }
            else if (is64)
            {
                wkeSetDragDropEnable_x64(webView, enable);
            }
            else
            {
                wkeSetDragDropEnable_x86(webView, enable);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetUserAgent", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeSetUserAgent_auto(IntPtr webView, string userAgent);

        [DllImport(DLL_x86, EntryPoint = "wkeSetUserAgent", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeSetUserAgent_x86(IntPtr webView, string userAgent);

        [DllImport(DLL_x64, EntryPoint = "wkeSetUserAgent", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeSetUserAgent_x64(IntPtr webView, string userAgent);

        public static void wkeSetUserAgent(IntPtr webView, string userAgent)
        {
            if (isAutoFile)
            {
                wkeSetUserAgent_auto(webView, userAgent);
            }
            else if (is64)
            {
                wkeSetUserAgent_x64(webView, userAgent);
            }
            else
            {
                wkeSetUserAgent_x86(webView, userAgent);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetUserAgent", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern IntPtr wkeGetUserAgent_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGetUserAgent", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern IntPtr wkeGetUserAgent_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetUserAgent", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern IntPtr wkeGetUserAgent_x64(IntPtr webView);

        public static IntPtr wkeGetUserAgent(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGetUserAgent_auto(webView);
            }
            else if (is64)
            {
                return wkeGetUserAgent_x64(webView);
            }
            else
            {
                return wkeGetUserAgent_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeLoadURL", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeLoadURL_auto(IntPtr webView, string url);

        [DllImport(DLL_x86, EntryPoint = "wkeLoadURL", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeLoadURL_x86(IntPtr webView, string url);

        [DllImport(DLL_x64, EntryPoint = "wkeLoadURL", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeLoadURL_x64(IntPtr webView, string url);

        public static void wkeLoadURL(IntPtr webView, string url)
        {
            if (isAutoFile)
            {
                wkeLoadURL_auto(webView, url);
            }
            else if (is64)
            {
                wkeLoadURL_x64(webView, url);
            }
            else
            {
                wkeLoadURL_x86(webView, url);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeLoadHTML", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeLoadHTML_auto(IntPtr webView, string html);

        [DllImport(DLL_x86, EntryPoint = "wkeLoadHTML", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeLoadHTML_x86(IntPtr webView, string html);

        [DllImport(DLL_x64, EntryPoint = "wkeLoadHTML", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeLoadHTML_x64(IntPtr webView, string html);

        public static void wkeLoadHTML(IntPtr webView, string html)
        {
            if (isAutoFile)
            {
                wkeLoadHTML_auto(webView, html);
            }
            else if (is64)
            {
                wkeLoadHTML_x64(webView, html);
            }
            else
            {
                wkeLoadHTML_x86(webView, html);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeLoadHtmlWithBaseUrl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeLoadHtmlWithBaseUrl_auto(IntPtr webView, string html, string baseUrl);

        [DllImport(DLL_x86, EntryPoint = "wkeLoadHtmlWithBaseUrl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeLoadHtmlWithBaseUrl_x86(IntPtr webView, string html, string baseUrl);

        [DllImport(DLL_x64, EntryPoint = "wkeLoadHtmlWithBaseUrl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeLoadHtmlWithBaseUrl_x64(IntPtr webView, string html, string baseUrl);

        public static void wkeLoadHtmlWithBaseUrl(IntPtr webView, string html, string baseUrl)
        {
            if (isAutoFile)
            {
                wkeLoadHtmlWithBaseUrl_auto(webView, html, baseUrl);
            }
            else if (is64)
            {
                wkeLoadHtmlWithBaseUrl_x64(webView, html, baseUrl);
            }
            else
            {
                wkeLoadHtmlWithBaseUrl_x86(webView, html, baseUrl);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeLoadFileW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeLoadFileW_auto(IntPtr webView, string fileName);

        [DllImport(DLL_x86, EntryPoint = "wkeLoadFileW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeLoadFileW_x86(IntPtr webView, string fileName);

        [DllImport(DLL_x64, EntryPoint = "wkeLoadFileW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeLoadFileW_x64(IntPtr webView, string fileName);

        public static void wkeLoadFileW(IntPtr webView, string fileName)
        {
            if (isAutoFile)
            {
                wkeLoadFileW_auto(webView, fileName);
            }
            else if (is64)
            {
                wkeLoadFileW_x64(webView, fileName);
            }
            else
            {
                wkeLoadFileW_x86(webView, fileName);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetURL", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetURL_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGetURL", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetURL_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetURL", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetURL_x64(IntPtr webView);

        public static IntPtr wkeGetURL(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGetURL_auto(webView);
            }
            else if (is64)
            {
                return wkeGetURL_x64(webView);
            }
            else
            {
                return wkeGetURL_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetFrameUrl", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetFrameUrl_auto(IntPtr webView, IntPtr frameId);

        [DllImport(DLL_x86, EntryPoint = "wkeGetFrameUrl", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetFrameUrl_x86(IntPtr webView, IntPtr frameId);

        [DllImport(DLL_x64, EntryPoint = "wkeGetFrameUrl", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetFrameUrl_x64(IntPtr webView, IntPtr frameId);

        public static IntPtr wkeGetFrameUrl(IntPtr webView, IntPtr frameId)
        {
            if (isAutoFile)
            {
                return wkeGetFrameUrl_auto(webView, frameId);
            }
            else if (is64)
            {
                return wkeGetFrameUrl_x64(webView, frameId);
            }
            else
            {
                return wkeGetFrameUrl_x86(webView, frameId);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeIsDocumentReady", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeIsDocumentReady_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeIsDocumentReady", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeIsDocumentReady_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeIsDocumentReady", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeIsDocumentReady_x64(IntPtr webView);

        public static bool wkeIsDocumentReady(IntPtr webView)
        {
            var result = 0;
            if (isAutoFile)
            {
                result = wkeIsDocumentReady_auto(webView);
            }
            else if (is64)
            {
                result = wkeIsDocumentReady_x64(webView);
            }
            else
            {
                result = wkeIsDocumentReady_x86(webView);
            }
            return result == 1;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeStopLoading", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeStopLoading_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeStopLoading", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeStopLoading_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeStopLoading", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeStopLoading_x64(IntPtr webView);

        public static void wkeStopLoading(IntPtr webView)
        {
            if (isAutoFile)
            {
                wkeStopLoading_auto(webView);
            }
            else if (is64)
            {
                wkeStopLoading_x64(webView);
            }
            else
            {
                wkeStopLoading_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeReload", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeReload_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeReload", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeReload_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeReload", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeReload_x64(IntPtr webView);

        public static void wkeReload(IntPtr webView)
        {
            if (isAutoFile)
            {
                wkeReload_auto(webView);
            }
            else if (is64)
            {
                wkeReload_x64(webView);
            }
            else
            {
                wkeReload_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetTitle", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetTitle_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGetTitle", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetTitle_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetTitle", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetTitle_x64(IntPtr webView);

        public static IntPtr wkeGetTitle(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGetTitle_auto(webView);
            }
            else if (is64)
            {
                return wkeGetTitle_x64(webView);
            }

            return wkeGetTitle_x86(webView);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeResize", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeResize_auto(IntPtr webView, int w, int h);

        [DllImport(DLL_x86, EntryPoint = "wkeResize", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeResize_x86(IntPtr webView, int w, int h);

        [DllImport(DLL_x64, EntryPoint = "wkeResize", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeResize_x64(IntPtr webView, int w, int h);

        public static void wkeResize(IntPtr webView, int w, int h)
        {
            if (isAutoFile)
            {
                wkeResize_auto(webView, w, h);
            }
            else if (is64)
            {
                wkeResize_x64(webView, w, h);
            }
            else
            {
                wkeResize_x86(webView, w, h);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetWidth", CallingConvention = CallingConvention.Cdecl)]
        private static extern int wkeGetWidth_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGetWidth", CallingConvention = CallingConvention.Cdecl)]
        private static extern int wkeGetWidth_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetWidth", CallingConvention = CallingConvention.Cdecl)]
        private static extern int wkeGetWidth_x64(IntPtr webView);

        public static int wkeGetWidth(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGetWidth_auto(webView);
            }
            else if (is64)
            {
                return wkeGetWidth_x64(webView);
            }
            else
            {
                return wkeGetWidth_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetHeight", CallingConvention = CallingConvention.Cdecl)]
        private static extern int wkeGetHeight_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGetHeight", CallingConvention = CallingConvention.Cdecl)]
        private static extern int wkeGetHeight_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetHeight", CallingConvention = CallingConvention.Cdecl)]
        private static extern int wkeGetHeight_x64(IntPtr webView);

        public static int wkeGetHeight(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGetHeight_auto(webView);
            }
            else if (is64)
            {
                return wkeGetHeight_x64(webView);
            }

            return wkeGetHeight_x86(webView);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetContentWidth", CallingConvention = CallingConvention.Cdecl)]
        private static extern int wkeGetContentWidth_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGetContentWidth", CallingConvention = CallingConvention.Cdecl)]
        private static extern int wkeGetContentWidth_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetContentWidth", CallingConvention = CallingConvention.Cdecl)]
        private static extern int wkeGetContentWidth_x64(IntPtr webView);

        public static int wkeGetContentWidth(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGetContentWidth_auto(webView);
            }
            else if (is64)
            {
                return wkeGetContentWidth_x64(webView);
            }

            return wkeGetContentWidth_x86(webView);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetContentHeight", CallingConvention = CallingConvention.Cdecl)]
        private static extern int wkeGetContentHeight_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGetContentHeight", CallingConvention = CallingConvention.Cdecl)]
        private static extern int wkeGetContentHeight_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetContentHeight", CallingConvention = CallingConvention.Cdecl)]
        private static extern int wkeGetContentHeight_x64(IntPtr webView);

        public static int wkeGetContentHeight(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGetContentHeight_auto(webView);
            }
            else if (is64)
            {
                return wkeGetContentHeight_x64(webView);
            }

            return wkeGetContentHeight_x86(webView);
        }

        [DllImport(DLL_auto, EntryPoint = "wkePaint", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkePaint_auto(IntPtr webView, IntPtr bits, byte pitch);

        [DllImport(DLL_x86, EntryPoint = "wkePaint", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkePaint_x86(IntPtr webView, IntPtr bits, byte pitch);

        [DllImport(DLL_x64, EntryPoint = "wkePaint", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkePaint_x64(IntPtr webView, IntPtr bits, byte pitch);

        public static void wkePaint(IntPtr webView, IntPtr bits, byte pitch)
        {
            if (isAutoFile)
            {
                wkePaint_auto(webView, bits, pitch);
            }
            else if (is64)
            {
                wkePaint_x64(webView, bits, pitch);
            }
            else
            {
                wkePaint_x86(webView, bits, pitch);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetViewDC", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetViewDC_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGetViewDC", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetViewDC_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetViewDC", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetViewDC_x64(IntPtr webView);

        public static IntPtr wkeGetViewDC(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGetViewDC_auto(webView);
            }
            else if (is64)
            {
                return wkeGetViewDC_x64(webView);
            }

            return wkeGetViewDC_x86(webView);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeCanGoBack", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeCanGoBack_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeCanGoBack", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeCanGoBack_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeCanGoBack", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeCanGoBack_x64(IntPtr webView);

        public static bool wkeCanGoBack(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeCanGoBack_auto(webView) != 0;
            }
            else if (is64)
            {
                return wkeCanGoBack_x64(webView) != 0;
            }

            return wkeCanGoBack_x86(webView) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGoBack", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeGoBack_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGoBack", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeGoBack_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGoBack", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeGoBack_x64(IntPtr webView);

        public static bool wkeGoBack(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGoBack_auto(webView) != 0;
            }
            else if (is64)
            {
                return wkeGoBack_x64(webView) != 0;
            }
            else
            {
                return wkeGoBack_x86(webView) != 0;
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeCanGoForward", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeCanGoForward_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeCanGoForward", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeCanGoForward_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeCanGoForward", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeCanGoForward_x64(IntPtr webView);

        public static bool wkeCanGoForward(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeCanGoForward_auto(webView) != 0;
            }
            else if (is64)
            {
                return wkeCanGoForward_x64(webView) != 0;
            }

            return wkeCanGoForward_x86(webView) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGoForward", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeGoForward_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGoForward", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeGoForward_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGoForward", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeGoForward_x64(IntPtr webView);

        public static bool wkeGoForward(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGoForward_auto(webView) != 0;
            }
            else if (is64)
            {
                return wkeGoForward_x64(webView) != 0;
            }

            return wkeGoForward_x86(webView) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeEditorSelectAll", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeEditorSelectAll_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeEditorSelectAll", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeEditorSelectAll_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeEditorSelectAll", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeEditorSelectAll_x64(IntPtr webView);

        public static void wkeEditorSelectAll(IntPtr webView)
        {
            if (isAutoFile)
            {
                wkeEditorSelectAll_auto(webView);
            }
            else if (is64)
            {
                wkeEditorSelectAll_x64(webView);
            }
            else
            {
                wkeEditorSelectAll_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeEditorUnSelect", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeEditorUnSelect_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeEditorUnSelect", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeEditorUnSelect_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeEditorUnSelect", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeEditorUnSelect_x64(IntPtr webView);

        public static void wkeEditorUnSelect(IntPtr webView)
        {
            if (isAutoFile)
            {
                wkeEditorUnSelect_auto(webView);
            }
            else if (is64)
            {
                wkeEditorUnSelect_x64(webView);
            }
            else
            {
                wkeEditorUnSelect_x86(webView);
            }
        }

        [DllImport(DLL_x86, EntryPoint = "wkeEditorCopy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorCopy_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeEditorCopy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorCopy_x64(IntPtr webView);

        public static void wkeEditorCopy(IntPtr webView)
        {
            if (isAutoFile)
            {
                wkeEditorCopy_x86(webView);
            }
            else if (is64)
            {
                wkeEditorCopy_x64(webView);
            }
            else
            {
                wkeEditorCopy_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeEditorCopy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorCopy_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeEditorCut", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorCut_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeEditorCut", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorCut_x64(IntPtr webView);

        public static void wkeEditorCut(IntPtr webView)
        {
            if (isAutoFile)
            {
                wkeEditorCopy_auto(webView);
            }
            else if (is64)
            {
                wkeEditorCut_x64(webView);
            }
            else
            {
                wkeEditorCut_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeEditorPaste", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorPaste_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeEditorPaste", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorPaste_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeEditorPaste", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorPaste_x64(IntPtr webView);

        public static void wkeEditorPaste(IntPtr webView)
        {
            if (isAutoFile)
            {
                wkeEditorPaste_auto(webView);
            }
            else if (is64)
            {
                wkeEditorPaste_x64(webView);
            }
            else
            {
                wkeEditorPaste_x64(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeEditorDelete", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorDelete_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeEditorDelete", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorDelete_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeEditorDelete", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorDelete_x64(IntPtr webView);

        public static void wkeEditorDelete(IntPtr webView)
        {
            if (isAutoFile)
            {
                wkeEditorDelete_auto(webView);
            }
            else if (is64)
            {
                wkeEditorDelete_x64(webView);
            }
            else
            {
                wkeEditorDelete_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeEditorUndo", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorUndo_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeEditorUndo", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorUndo_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeEditorUndo", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorUndo_x64(IntPtr webView);

        public static void wkeEditorUndo(IntPtr webView)
        {
            if (isAutoFile)
            {
                wkeEditorUndo_auto(webView);
            }
            else if (is64)
            {
                wkeEditorUndo_x64(webView);
            }
            else
            {
                wkeEditorUndo_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeEditorRedo", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorRedo_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeEditorRedo", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorRedo_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeEditorRedo", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorRedo_x64(IntPtr webView);

        public static void wkeEditorRedo(IntPtr webView)
        {
            if (isAutoFile)
            {
                wkeEditorRedo_auto(webView);
            }
            else if (is64)
            {
                wkeEditorRedo_x64(webView);
            }
            else
            {
                wkeEditorRedo_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetCookieW", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetCookie_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGetCookieW", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetCookie_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetCookieW", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetCookie_x64(IntPtr webView);

        public static IntPtr wkeGetCookie(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGetCookie_auto(webView);
            }
            else if (is64)
            {
                return wkeGetCookie_x64(webView);
            }
            else
            {
                return wkeGetCookie_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkePerformCookieCommand", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkePerformCookieCommand_auto(IntPtr webView, wkeCookieCommand command);

        [DllImport(DLL_x86, EntryPoint = "wkePerformCookieCommand", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkePerformCookieCommand_x86(IntPtr webView, wkeCookieCommand command);

        [DllImport(DLL_x64, EntryPoint = "wkePerformCookieCommand", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkePerformCookieCommand_x64(IntPtr webView, wkeCookieCommand command);

        public static void wkePerformCookieCommand(IntPtr webView, wkeCookieCommand command)
        {
            if (isAutoFile)
            {
                wkePerformCookieCommand_auto(webView, command);
            }
            else if (is64)
            {
                wkePerformCookieCommand_x64(webView, command);
            }
            else
            {
                wkePerformCookieCommand_x86(webView, command);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetCookieEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetCookieEnabled_auto(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x86, EntryPoint = "wkeSetCookieEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetCookieEnabled_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x64, EntryPoint = "wkeSetCookieEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetCookieEnabled_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetCookieEnabled(IntPtr webView, bool enable)
        {
            if (isAutoFile)
            {
                wkeSetCookieEnabled_auto(webView, enable);
            }
            else if (is64)
            {
                wkeSetCookieEnabled_x64(webView, enable);
            }
            else
            {
                wkeSetCookieEnabled_x86(webView, enable);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeIsCookieEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeIsCookieEnabled_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeIsCookieEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeIsCookieEnabled_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeIsCookieEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeIsCookieEnabled_x64(IntPtr webView);

        public static bool wkeIsCookieEnabled(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeIsCookieEnabled_auto(webView) != 0;
            }
            else if (is64)
            {
                return wkeIsCookieEnabled_x64(webView) != 0;
            }

            return wkeIsCookieEnabled_x86(webView) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeFireMouseEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireMouseEvent_auto(IntPtr webView, int message, int x, int y, int flags);

        [DllImport(DLL_x86, EntryPoint = "wkeFireMouseEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireMouseEvent_x86(IntPtr webView, int message, int x, int y, int flags);

        [DllImport(DLL_x64, EntryPoint = "wkeFireMouseEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireMouseEvent_x64(IntPtr webView, int message, int x, int y, int flags);

        public static bool wkeFireMouseEvent(IntPtr webView, int message, int x, int y, int flags)
        {
            if (isAutoFile)
            {
                return wkeFireMouseEvent_auto(webView, message, x, y, flags) != 0;
            }
            else if (is64)
            {
                return wkeFireMouseEvent_x64(webView, message, x, y, flags) != 0;
            }

            return wkeFireMouseEvent_x86(webView, message, x, y, flags) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeFireContextMenuEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireContextMenuEvent_auto(IntPtr webView, int x, int y, uint flags);

        [DllImport(DLL_x86, EntryPoint = "wkeFireContextMenuEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireContextMenuEvent_x86(IntPtr webView, int x, int y, uint flags);

        [DllImport(DLL_x64, EntryPoint = "wkeFireContextMenuEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireContextMenuEvent_x64(IntPtr webView, int x, int y, uint flags);

        public static bool wkeFireContextMenuEvent(IntPtr webView, int x, int y, uint flags)
        {
            if (isAutoFile)
            {
                return wkeFireContextMenuEvent_auto(webView, x, y, flags) != 0;
            }
            else if (is64)
            {
                return wkeFireContextMenuEvent_x64(webView, x, y, flags) != 0;
            }

            return wkeFireContextMenuEvent_x86(webView, x, y, flags) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeFireMouseWheelEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireMouseWheelEvent_auto(IntPtr webView, int x, int y, int delta, uint flags);

        [DllImport(DLL_x86, EntryPoint = "wkeFireMouseWheelEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireMouseWheelEvent_x86(IntPtr webView, int x, int y, int delta, uint flags);

        [DllImport(DLL_x64, EntryPoint = "wkeFireMouseWheelEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireMouseWheelEvent_x64(IntPtr webView, int x, int y, int delta, uint flags);

        public static bool wkeFireMouseWheelEvent(IntPtr webView, int x, int y, int delta, uint flags)
        {
            if (isAutoFile)
            {
                return wkeFireMouseWheelEvent_auto(webView, x, y, delta, flags) != 0;
            }
            else if (is64)
            {
                return wkeFireMouseWheelEvent_x64(webView, x, y, delta, flags) != 0;
            }

            return wkeFireMouseWheelEvent_x86(webView, x, y, delta, flags) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeFireKeyUpEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireKeyUpEvent_auto(IntPtr webView, int virtualKeyCode, uint flags, [MarshalAs(UnmanagedType.I1)] bool systemKey);

        [DllImport(DLL_x86, EntryPoint = "wkeFireKeyUpEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireKeyUpEvent_x86(IntPtr webView, int virtualKeyCode, uint flags, [MarshalAs(UnmanagedType.I1)] bool systemKey);

        [DllImport(DLL_x64, EntryPoint = "wkeFireKeyUpEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireKeyUpEvent_x64(IntPtr webView, int virtualKeyCode, uint flags, [MarshalAs(UnmanagedType.I1)] bool systemKey);

        public static bool wkeFireKeyUpEvent(IntPtr webView, int virtualKeyCode, uint flags, bool systemKey)
        {
            if (isAutoFile)
            {
                return wkeFireKeyUpEvent_auto(webView, virtualKeyCode, flags, systemKey) != 0;
            }
            else if (is64)
            {
                return wkeFireKeyUpEvent_x64(webView, virtualKeyCode, flags, systemKey) != 0;
            }

            return wkeFireKeyUpEvent_x86(webView, virtualKeyCode, flags, systemKey) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeFireKeyDownEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireKeyDownEvent_auto(IntPtr webView, int virtualKeyCode, uint flags, [MarshalAs(UnmanagedType.I1)] bool systemKey);

        [DllImport(DLL_x86, EntryPoint = "wkeFireKeyDownEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireKeyDownEvent_x86(IntPtr webView, int virtualKeyCode, uint flags, [MarshalAs(UnmanagedType.I1)] bool systemKey);

        [DllImport(DLL_x64, EntryPoint = "wkeFireKeyDownEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireKeyDownEvent_x64(IntPtr webView, int virtualKeyCode, uint flags, [MarshalAs(UnmanagedType.I1)] bool systemKey);

        public static bool wkeFireKeyDownEvent(IntPtr webView, int virtualKeyCode, uint flags, bool systemKey)
        {
            if (isAutoFile)
            {
                return wkeFireKeyDownEvent_auto(webView, virtualKeyCode, flags, systemKey) != 0;
            }
            else if (is64)
            {
                return wkeFireKeyDownEvent_x64(webView, virtualKeyCode, flags, systemKey) != 0;
            }

            return wkeFireKeyDownEvent_x86(webView, virtualKeyCode, flags, systemKey) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeFireKeyPressEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireKeyPressEvent_auto(IntPtr webView, int charCode, uint flags, [MarshalAs(UnmanagedType.I1)] bool systemKey);

        [DllImport(DLL_x86, EntryPoint = "wkeFireKeyPressEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireKeyPressEvent_x86(IntPtr webView, int charCode, uint flags, [MarshalAs(UnmanagedType.I1)] bool systemKey);

        [DllImport(DLL_x64, EntryPoint = "wkeFireKeyPressEvent", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireKeyPressEvent_x64(IntPtr webView, int charCode, uint flags, [MarshalAs(UnmanagedType.I1)] bool systemKey);

        public static bool wkeFireKeyPressEvent(IntPtr webView, int charCode, uint flags, bool systemKey)
        {
            if (isAutoFile)
            {
                return wkeFireKeyPressEvent_auto(webView, charCode, flags, systemKey) != 0;
            }
            else if (is64)
            {
                return wkeFireKeyPressEvent_x64(webView, charCode, flags, systemKey) != 0;
            }

            return wkeFireKeyPressEvent_x86(webView, charCode, flags, systemKey) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeFireWindowsMessage", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireWindowsMessage_auto(IntPtr webView, IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam, IntPtr result);

        [DllImport(DLL_x86, EntryPoint = "wkeFireWindowsMessage", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireWindowsMessage_x86(IntPtr webView, IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam, IntPtr result);

        [DllImport(DLL_x64, EntryPoint = "wkeFireWindowsMessage", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeFireWindowsMessage_x64(IntPtr webView, IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam, IntPtr result);

        public static bool wkeFireWindowsMessage(IntPtr webView, IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam, IntPtr result)
        {
            if (isAutoFile)
            {
                return wkeFireWindowsMessage_auto(webView, hWnd, message, wParam, lParam, result) != 0;
            }
            else if (is64)
            {
                return wkeFireWindowsMessage_x64(webView, hWnd, message, wParam, lParam, result) != 0;
            }

            return wkeFireWindowsMessage_x86(webView, hWnd, message, wParam, lParam, result) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetFocus", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeSetFocus_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeSetFocus", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeSetFocus_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeSetFocus", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeSetFocus_x64(IntPtr webView);

        public static bool wkeSetFocus(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeSetFocus_auto(webView) != 0;
            }
            else if (is64)
            {
                return wkeSetFocus_x64(webView) != 0;
            }

            return wkeSetFocus_x86(webView) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeKillFocus", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeKillFocus_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeKillFocus", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeKillFocus_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeKillFocus", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeKillFocus_x64(IntPtr webView);

        public static bool wkeKillFocus(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeKillFocus_auto(webView) != 0;
            }
            else if (is64)
            {
                return wkeKillFocus_x64(webView) != 0;
            }

            return wkeKillFocus_x86(webView) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetCaretRect", CallingConvention = CallingConvention.Cdecl)]
        private static extern wkeRect wkeGetCaretRect_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGetCaretRect", CallingConvention = CallingConvention.Cdecl)]
        private static extern wkeRect wkeGetCaretRect_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetCaretRect", CallingConvention = CallingConvention.Cdecl)]
        private static extern wkeRect wkeGetCaretRect_x64(IntPtr webView);

        public static wkeRect wkeGetCaretRect(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGetCaretRect_auto(webView);
            }
            else if (is64)
            {
                return wkeGetCaretRect_x64(webView);
            }

            return wkeGetCaretRect_x86(webView);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeRunJSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern long wkeRunJSW_auto(IntPtr webView, string script);

        [DllImport(DLL_x86, EntryPoint = "wkeRunJSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern long wkeRunJSW_x86(IntPtr webView, string script);

        [DllImport(DLL_x64, EntryPoint = "wkeRunJSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern long wkeRunJSW_x64(IntPtr webView, string script);

        public static long wkeRunJSW(IntPtr webView, string script)
        {
            if (isAutoFile)
            {
                return wkeRunJSW_auto(webView, script);
            }
            else if (is64)
            {
                return wkeRunJSW_x64(webView, script);
            }
            return wkeRunJSW_x86(webView, script);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGlobalExec", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGlobalExec_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGlobalExec", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGlobalExec_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGlobalExec", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGlobalExec_x64(IntPtr webView);

        public static IntPtr wkeGlobalExec(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGlobalExec_auto(webView);
            }
            else if (is64)
            {
                return wkeGlobalExec_x64(webView);
            }

            return wkeGlobalExec_x86(webView);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetZoomFactor", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetZoomFactor_auto(IntPtr webView, float factor);

        [DllImport(DLL_x86, EntryPoint = "wkeSetZoomFactor", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetZoomFactor_x86(IntPtr webView, float factor);

        [DllImport(DLL_x64, EntryPoint = "wkeSetZoomFactor", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetZoomFactor_x64(IntPtr webView, float factor);

        public static void wkeSetZoomFactor(IntPtr webView, float factor)
        {
            if (isAutoFile)
            {
                wkeSetZoomFactor_auto(webView, factor);
            }
            else if (is64)
            {
                wkeSetZoomFactor_x64(webView, factor);
            }
            else
            {
                wkeSetZoomFactor_x86(webView, factor);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetZoomFactor", CallingConvention = CallingConvention.Cdecl)]
        private static extern float wkeGetZoomFactor_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGetZoomFactor", CallingConvention = CallingConvention.Cdecl)]
        private static extern float wkeGetZoomFactor_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetZoomFactor", CallingConvention = CallingConvention.Cdecl)]
        private static extern float wkeGetZoomFactor_x64(IntPtr webView);

        public static float wkeGetZoomFactor(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGetZoomFactor_auto(webView);
            }
            else if (is64)
            {
                return wkeGetZoomFactor_x64(webView);
            }
            else
            {
                return wkeGetZoomFactor_x86(webView);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetString", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetString_auto(IntPtr wkeString);

        [DllImport(DLL_x86, EntryPoint = "wkeGetString", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetString_x86(IntPtr wkeString);

        [DllImport(DLL_x64, EntryPoint = "wkeGetString", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetString_x64(IntPtr wkeString);

        public static IntPtr wkeGetString(IntPtr wkeString)
        {
            if (isAutoFile)
            {
                return wkeGetString_auto(wkeString);
            }
            else if (is64)
            {
                return wkeGetString_x64(wkeString);
            }

            return wkeGetString_x86(wkeString);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetCursorInfoType", CallingConvention = CallingConvention.Cdecl)]
        private static extern wkeCursorInfo wkeGetCursorInfoType_auto(IntPtr webView);

        [DllImport(DLL_x86, EntryPoint = "wkeGetCursorInfoType", CallingConvention = CallingConvention.Cdecl)]
        private static extern wkeCursorInfo wkeGetCursorInfoType_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetCursorInfoType", CallingConvention = CallingConvention.Cdecl)]
        private static extern wkeCursorInfo wkeGetCursorInfoType_x64(IntPtr webView);

        public static wkeCursorInfo wkeGetCursorInfoType(IntPtr webView)
        {
            if (isAutoFile)
            {
                return wkeGetCursorInfoType_auto(webView);
            }
            else if (is64)
            {
                return wkeGetCursorInfoType_x64(webView);
            }

            return wkeGetCursorInfoType_x86(webView);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnURLChanged2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnURLChanged2_auto(IntPtr webView, wkeURLChangedCallback2 callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnURLChanged2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnURLChanged2_x86(IntPtr webView, wkeURLChangedCallback2 callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnURLChanged2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnURLChanged2_x64(IntPtr webView, wkeURLChangedCallback2 callback, IntPtr param);

        public static void wkeOnURLChanged2(IntPtr webView, wkeURLChangedCallback2 callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnURLChanged2_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnURLChanged2_x64(webView, callback, param);
            }
            else
            {
                wkeOnURLChanged2_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnPaintUpdated", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnPaintUpdated_auto(IntPtr webView, wkePaintUpdatedCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnPaintUpdated", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnPaintUpdated_x86(IntPtr webView, wkePaintUpdatedCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnPaintUpdated", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnPaintUpdated_x64(IntPtr webView, wkePaintUpdatedCallback callback, IntPtr param);

        public static void wkeOnPaintUpdated(IntPtr webView, wkePaintUpdatedCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnPaintUpdated_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnPaintUpdated_x64(webView, callback, param);
            }
            else
            {
                wkeOnPaintUpdated_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnNavigation", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnNavigation_auto(IntPtr webView, wkeNavigationCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnNavigation", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnNavigation_x86(IntPtr webView, wkeNavigationCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnNavigation", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnNavigation_x64(IntPtr webView, wkeNavigationCallback callback, IntPtr param);

        public static void wkeOnNavigation(IntPtr webView, wkeNavigationCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnNavigation_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnNavigation_x64(webView, callback, param);
            }
            else
            {
                wkeOnNavigation_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnCreateView", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnCreateView_auto(IntPtr webView, wkeCreateViewCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnCreateView", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnCreateView_x86(IntPtr webView, wkeCreateViewCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnCreateView", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnCreateView_x64(IntPtr webView, wkeCreateViewCallback callback, IntPtr param);

        public static void wkeOnCreateView(IntPtr webView, wkeCreateViewCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnCreateView_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnCreateView_x64(webView, callback, param);
            }
            else
            {
                wkeOnCreateView_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnDocumentReady2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnDocumentReady2_auto(IntPtr webView, wkeDocumentReady2Callback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnDocumentReady2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnDocumentReady2_x86(IntPtr webView, wkeDocumentReady2Callback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnDocumentReady2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnDocumentReady2_x64(IntPtr webView, wkeDocumentReady2Callback callback, IntPtr param);

        public static void wkeOnDocumentReady2(IntPtr webView, wkeDocumentReady2Callback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnDocumentReady2_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnDocumentReady2_x64(webView, callback, param);
            }
            else
            {
                wkeOnDocumentReady2_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnDownload", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnDownload_auto(IntPtr webView, wkeDownloadCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnDownload", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnDownload_x86(IntPtr webView, wkeDownloadCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnDownload", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnDownload_x64(IntPtr webView, wkeDownloadCallback callback, IntPtr param);

        public static void wkeOnDownload(IntPtr webView, wkeDownloadCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnDownload_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnDownload_x64(webView, callback, param);
            }
            else
            {
                wkeOnDownload_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnConsole", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnConsole_auto(IntPtr webView, wkeConsoleCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnConsole", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnConsole_x86(IntPtr webView, wkeConsoleCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnConsole", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnConsole_x64(IntPtr webView, wkeConsoleCallback callback, IntPtr param);

        public static void wkeOnConsole(IntPtr webView, wkeConsoleCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnConsole_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnConsole_x64(webView, callback, param);
            }
            else
            {
                wkeOnConsole_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetOnResponse", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetOnResponse_auto(IntPtr webView, wkeNetResponseCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeNetOnResponse", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetOnResponse_x86(IntPtr webView, wkeNetResponseCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeNetOnResponse", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetOnResponse_x64(IntPtr webView, wkeNetResponseCallback callback, IntPtr param);

        public static void wkeNetOnResponse(IntPtr webView, wkeNetResponseCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeNetOnResponse_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeNetOnResponse_x64(webView, callback, param);
            }
            else
            {
                wkeNetOnResponse_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnLoadUrlBegin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnLoadUrlBegin_auto(IntPtr webView, wkeLoadUrlBeginCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnLoadUrlBegin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnLoadUrlBegin_x86(IntPtr webView, wkeLoadUrlBeginCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnLoadUrlBegin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnLoadUrlBegin_x64(IntPtr webView, wkeLoadUrlBeginCallback callback, IntPtr param);

        public static void wkeOnLoadUrlBegin(IntPtr webView, wkeLoadUrlBeginCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnLoadUrlBegin_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnLoadUrlBegin_x64(webView, callback, param);
            }
            else
            {
                wkeOnLoadUrlBegin_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnLoadUrlEnd", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnLoadUrlEnd_auto(IntPtr webView, wkeLoadUrlEndCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnLoadUrlEnd", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnLoadUrlEnd_x86(IntPtr webView, wkeLoadUrlEndCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnLoadUrlEnd", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnLoadUrlEnd_x64(IntPtr webView, wkeLoadUrlEndCallback callback, IntPtr param);

        public static void wkeOnLoadUrlEnd(IntPtr webView, wkeLoadUrlEndCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnLoadUrlEnd_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnLoadUrlEnd_x64(webView, callback, param);
            }
            else
            {
                wkeOnLoadUrlEnd_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnDidCreateScriptContext", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnDidCreateScriptContext_auto(IntPtr webView, wkeDidCreateScriptContextCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnDidCreateScriptContext", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnDidCreateScriptContext_x86(IntPtr webView, wkeDidCreateScriptContextCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnDidCreateScriptContext", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnDidCreateScriptContext_x64(IntPtr webView, wkeDidCreateScriptContextCallback callback, IntPtr param);

        public static void wkeOnDidCreateScriptContext(IntPtr webView, wkeDidCreateScriptContextCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnDidCreateScriptContext_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnDidCreateScriptContext_x64(webView, callback, param);
            }
            else
            {
                wkeOnDidCreateScriptContext_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnWillReleaseScriptContext", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnWillReleaseScriptContext_auto(IntPtr webView, wkeWillReleaseScriptContextCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnWillReleaseScriptContext", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnWillReleaseScriptContext_x86(IntPtr webView, wkeWillReleaseScriptContextCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnWillReleaseScriptContext", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnWillReleaseScriptContext_x64(IntPtr webView, wkeWillReleaseScriptContextCallback callback, IntPtr param);

        public static void wkeOnWillReleaseScriptContext(IntPtr webView, wkeWillReleaseScriptContextCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnWillReleaseScriptContext_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnWillReleaseScriptContext_x64(webView, callback, param);
            }
            else
            {
                wkeOnWillReleaseScriptContext_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetSetMIMEType", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeNetSetMIMEType_auto(IntPtr job, string type);

        [DllImport(DLL_x86, EntryPoint = "wkeNetSetMIMEType", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeNetSetMIMEType_x86(IntPtr job, string type);

        [DllImport(DLL_x64, EntryPoint = "wkeNetSetMIMEType", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeNetSetMIMEType_x64(IntPtr job, string type);

        public static void wkeNetSetMIMEType(IntPtr job, string type)
        {
            if (isAutoFile)
            {
                wkeNetSetMIMEType_auto(job, type);
            }
            else if (is64)
            {
                wkeNetSetMIMEType_x64(job, type);
            }
            else
            {
                wkeNetSetMIMEType_x86(job, type);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetSetData", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeNetSetData_auto(IntPtr job, [MarshalAs(UnmanagedType.LPArray)] byte[] buf, int len);

        [DllImport(DLL_x86, EntryPoint = "wkeNetSetData", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeNetSetData_x86(IntPtr job, [MarshalAs(UnmanagedType.LPArray)] byte[] buf, int len);

        [DllImport(DLL_x64, EntryPoint = "wkeNetSetData", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeNetSetData_x64(IntPtr job, [MarshalAs(UnmanagedType.LPArray)] byte[] buf, int len);

        public static void wkeNetSetData(IntPtr job, byte[] buf, int len)
        {
            if (isAutoFile)
            {
                wkeNetSetData_auto(job, buf, len);
            }
            else if (is64)
            {
                wkeNetSetData_x64(job, buf, len);
            }
            else
            {
                wkeNetSetData_x86(job, buf, len);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetHookRequest", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetHookRequest_auto(IntPtr job);

        [DllImport(DLL_x86, EntryPoint = "wkeNetHookRequest", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetHookRequest_x86(IntPtr job);

        [DllImport(DLL_x64, EntryPoint = "wkeNetHookRequest", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetHookRequest_x64(IntPtr job);

        public static void wkeNetHookRequest(IntPtr job)
        {
            if (isAutoFile)
            {
                wkeNetHookRequest_auto(job);
            }
            else if (is64)
            {
                wkeNetHookRequest_x64(job);
            }
            else
            {
                wkeNetHookRequest_x86(job);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetHoldJobToAsynCommit", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetHoldJobToAsynCommit_auto(IntPtr job);

        [DllImport(DLL_x86, EntryPoint = "wkeNetHoldJobToAsynCommit", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetHoldJobToAsynCommit_x86(IntPtr job);

        [DllImport(DLL_x64, EntryPoint = "wkeNetHoldJobToAsynCommit", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetHoldJobToAsynCommit_x64(IntPtr job);

        public static void wkeNetHoldJobToAsynCommit(IntPtr job)
        {
            if (isAutoFile)
            {
                wkeNetHoldJobToAsynCommit_auto(job);
            }
            else if (is64)
            {
                wkeNetHoldJobToAsynCommit_x64(job);
            }
            else
            {
                wkeNetHoldJobToAsynCommit_x86(job);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetGetPostBody", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeNetGetPostBody_auto(IntPtr job);

        [DllImport(DLL_x86, EntryPoint = "wkeNetGetPostBody", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeNetGetPostBody_x86(IntPtr job);

        [DllImport(DLL_x64, EntryPoint = "wkeNetGetPostBody", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeNetGetPostBody_x64(IntPtr job);

        public static IntPtr wkeNetGetPostBody(IntPtr job)
        {
            if (isAutoFile)
            {
                return wkeNetGetPostBody_auto(job);
            }
            else if (is64)
            {
                return wkeNetGetPostBody_x64(job);
            }

            return wkeNetGetPostBody_x86(job);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetContinueJob", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetContinueJob_auto(IntPtr job);

        [DllImport(DLL_x86, EntryPoint = "wkeNetContinueJob", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetContinueJob_x86(IntPtr job);

        [DllImport(DLL_x64, EntryPoint = "wkeNetContinueJob", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetContinueJob_x64(IntPtr job);

        public static void wkeNetContinueJob(IntPtr job)
        {
            if (isAutoFile)
            {
                wkeNetContinueJob_auto(job);
            }
            else if (is64)
            {
                wkeNetContinueJob_x64(job);
            }
            else
            {
                wkeNetContinueJob_x86(job);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetGetRequestMethod", CallingConvention = CallingConvention.Cdecl)]
        private static extern wkeRequestType wkeNetGetRequestMethod_auto(IntPtr job);

        [DllImport(DLL_x86, EntryPoint = "wkeNetGetRequestMethod", CallingConvention = CallingConvention.Cdecl)]
        private static extern wkeRequestType wkeNetGetRequestMethod_x86(IntPtr job);

        [DllImport(DLL_x64, EntryPoint = "wkeNetGetRequestMethod", CallingConvention = CallingConvention.Cdecl)]
        private static extern wkeRequestType wkeNetGetRequestMethod_x64(IntPtr job);

        public static wkeRequestType wkeNetGetRequestMethod(IntPtr job)
        {
            if (isAutoFile)
            {
                return wkeNetGetRequestMethod_auto(job);
            }
            else if (is64)
            {
                return wkeNetGetRequestMethod_x64(job);
            }

            return wkeNetGetRequestMethod_x86(job);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeIsMainFrame", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeIsMainFrame_auto(IntPtr webview, IntPtr webFrame);

        [DllImport(DLL_x86, EntryPoint = "wkeIsMainFrame", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeIsMainFrame_x86(IntPtr webview, IntPtr webFrame);

        [DllImport(DLL_x64, EntryPoint = "wkeIsMainFrame", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeIsMainFrame_x64(IntPtr webview, IntPtr webFrame);

        public static bool wkeIsMainFrame(IntPtr webview, IntPtr frameId)
        {
            if (isAutoFile)
            {
                return wkeIsMainFrame_auto(webview, frameId) != 0;
            }
            else if (is64)
            {
                return wkeIsMainFrame_x64(webview, frameId) != 0;
            }

            return wkeIsMainFrame_x86(webview, frameId) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeGetGlobalExecByFrame", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetGlobalExecByFrame_auto(IntPtr webView, IntPtr frameId);

        [DllImport(DLL_x86, EntryPoint = "wkeGetGlobalExecByFrame", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetGlobalExecByFrame_x86(IntPtr webView, IntPtr frameId);

        [DllImport(DLL_x64, EntryPoint = "wkeGetGlobalExecByFrame", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeGetGlobalExecByFrame_x64(IntPtr webView, IntPtr frameId);

        public static IntPtr wkeGetGlobalExecByFrame(IntPtr webView, IntPtr frameId)
        {
            if (isAutoFile)
            {
                return wkeGetGlobalExecByFrame_auto(webView, frameId);
            }
            else if (is64)
            {
                return wkeGetGlobalExecByFrame_x64(webView, frameId);
            }

            return wkeGetGlobalExecByFrame_x86(webView, frameId);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeIsWebRemoteFrame", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeIsWebRemoteFrame_auto(IntPtr webView, IntPtr frameId);

        [DllImport(DLL_x86, EntryPoint = "wkeIsWebRemoteFrame", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeIsWebRemoteFrame_x86(IntPtr webView, IntPtr frameId);

        [DllImport(DLL_x64, EntryPoint = "wkeIsWebRemoteFrame", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte wkeIsWebRemoteFrame_x64(IntPtr webView, IntPtr frameId);

        public static bool wkeIsWebRemoteFrame(IntPtr webView, IntPtr frameId)
        {
            if (isAutoFile)
            {
                return wkeIsWebRemoteFrame_auto(webView, frameId) != 0;
            }
            else if (is64)
            {
                return wkeIsWebRemoteFrame_x64(webView, frameId) != 0;
            }

            return wkeIsWebRemoteFrame_x86(webView, frameId) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "wkeJsBindFunction", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeJsBindFunction_auto(string name, wkeJsNativeFunction fn, IntPtr param, uint argCount);

        [DllImport(DLL_x86, EntryPoint = "wkeJsBindFunction", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeJsBindFunction_x86(string name, wkeJsNativeFunction fn, IntPtr param, uint argCount);

        [DllImport(DLL_x64, EntryPoint = "wkeJsBindFunction", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeJsBindFunction_x64(string name, wkeJsNativeFunction fn, IntPtr param, uint argCount);

        public static void wkeJsBindFunction(string name, wkeJsNativeFunction fn, IntPtr param, uint argCount)
        {
            if (isAutoFile)
            {
                wkeJsBindFunction_auto(name, fn, param, argCount);
            }
            else if (is64)
            {
                wkeJsBindFunction_x64(name, fn, param, argCount);
            }
            else
            {
                wkeJsBindFunction_x86(name, fn, param, argCount);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "jsArgCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int jsArgCount_auto(IntPtr es);

        [DllImport(DLL_x86, EntryPoint = "jsArgCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int jsArgCount_x86(IntPtr es);

        [DllImport(DLL_x64, EntryPoint = "jsArgCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int jsArgCount_x64(IntPtr es);

        public static int jsArgCount(IntPtr es)
        {
            if (isAutoFile)
            {
                return jsArgCount_auto(es);
            }
            else if (is64)
            {
                return jsArgCount_x64(es);
            }

            return jsArgCount_x86(es);
        }

        [DllImport(DLL_auto, EntryPoint = "jsArg", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsArg_auto(IntPtr es, int argIdx);

        [DllImport(DLL_x86, EntryPoint = "jsArg", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsArg_x86(IntPtr es, int argIdx);

        [DllImport(DLL_x64, EntryPoint = "jsArg", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsArg_x64(IntPtr es, int argIdx);

        public static long jsArg(IntPtr es, int argIdx)
        {
            if (isAutoFile)
            {
                return jsArg_auto(es, argIdx);
            }
            else if (is64)
            {
                return jsArg_x64(es, argIdx);
            }

            return jsArg_x86(es, argIdx);
        }

        [DllImport(DLL_auto, EntryPoint = "jsGetKeys", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr jsGetKeys_auto(IntPtr es, long value);

        [DllImport(DLL_x86, EntryPoint = "jsGetKeys", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr jsGetKeys_x86(IntPtr es, long value);

        [DllImport(DLL_x64, EntryPoint = "jsGetKeys", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr jsGetKeys_x64(IntPtr es, long value);

        public static IntPtr jsGetKeys(IntPtr es, long value)
        {
            if (isAutoFile)
            {
                return jsGetKeys_auto(es, value);
            }
            else if (is64)
            {
                return jsGetKeys_x64(es, value);
            }

            return jsGetKeys_x86(es, value);
        }

        [DllImport(DLL_auto, EntryPoint = "jsTypeOf", CallingConvention = CallingConvention.Cdecl)]
        private static extern jsType jsTypeOf_auto(long v);

        [DllImport(DLL_x86, EntryPoint = "jsTypeOf", CallingConvention = CallingConvention.Cdecl)]
        private static extern jsType jsTypeOf_x86(long v);

        [DllImport(DLL_x64, EntryPoint = "jsTypeOf", CallingConvention = CallingConvention.Cdecl)]
        private static extern jsType jsTypeOf_x64(long v);

        public static jsType jsTypeOf(long v)
        {
            if (isAutoFile)
            {
                return jsTypeOf_auto(v);
            }
            else if (is64)
            {
                return jsTypeOf_x64(v);
            }

            return jsTypeOf_x86(v);
        }

        [DllImport(DLL_auto, EntryPoint = "jsToDouble", CallingConvention = CallingConvention.Cdecl)]
        private static extern double jsToDouble_auto(IntPtr es, long v);

        [DllImport(DLL_x86, EntryPoint = "jsToDouble", CallingConvention = CallingConvention.Cdecl)]
        private static extern double jsToDouble_x86(IntPtr es, long v);

        [DllImport(DLL_x64, EntryPoint = "jsToDouble", CallingConvention = CallingConvention.Cdecl)]
        private static extern double jsToDouble_x64(IntPtr es, long v);

        public static double jsToDouble(IntPtr es, long v)
        {
            if (isAutoFile)
            {
                return jsToDouble_auto(es, v);
            }
            else if (is64)
            {
                return jsToDouble_x64(es, v);
            }

            return jsToDouble_x86(es, v);
        }

        [DllImport(DLL_auto, EntryPoint = "jsToBoolean", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte jsToBoolean_auto(IntPtr es, long v);

        [DllImport(DLL_x86, EntryPoint = "jsToBoolean", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte jsToBoolean_x86(IntPtr es, long v);

        [DllImport(DLL_x64, EntryPoint = "jsToBoolean", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte jsToBoolean_x64(IntPtr es, long v);

        public static bool jsToBoolean(IntPtr es, long v)
        {
            if (isAutoFile)
            {
                return jsToBoolean_auto(es, v) != 0;
            }
            else if (is64)
            {
                return jsToBoolean_x64(es, v) != 0;
            }

            return jsToBoolean_x86(es, v) != 0;
        }

        [DllImport(DLL_auto, EntryPoint = "jsToTempStringW", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr jsToTempStringW_auto(IntPtr es, long v);

        [DllImport(DLL_x86, EntryPoint = "jsToTempStringW", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr jsToTempStringW_x86(IntPtr es, long v);

        [DllImport(DLL_x64, EntryPoint = "jsToTempStringW", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr jsToTempStringW_x64(IntPtr es, long v);

        public static IntPtr jsToTempStringW(IntPtr es, long v)
        {
            if (isAutoFile)
            {
                return jsToTempStringW_auto(es, v);
            }
            else if (is64)
            {
                return jsToTempStringW_x64(es, v);
            }

            return jsToTempStringW_x86(es, v);
        }

        [DllImport(DLL_auto, EntryPoint = "jsInt", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsInt_auto(int n);

        [DllImport(DLL_x86, EntryPoint = "jsInt", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsInt_x86(int n);

        [DllImport(DLL_x64, EntryPoint = "jsInt", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsInt_x64(int n);

        public static long jsInt(int n)
        {
            if (isAutoFile)
            {
                return jsInt_auto(n);
            }
            else if (is64)
            {
                return jsInt_x64(n);
            }

            return jsInt_x86(n);
        }

        [DllImport(DLL_auto, EntryPoint = "jsFloat", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsFloat_auto(float f);

        [DllImport(DLL_x86, EntryPoint = "jsFloat", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsFloat_x86(float f);

        [DllImport(DLL_x64, EntryPoint = "jsFloat", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsFloat_x64(float f);

        public static long jsFloat(float f)
        {
            if (isAutoFile)
            {
                return jsFloat_auto(f);
            }
            else if (is64)
            {
                return jsFloat_x64(f);
            }

            return jsFloat_x86(f);
        }

        [DllImport(DLL_auto, EntryPoint = "jsDouble", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsDouble_auto(double d);

        [DllImport(DLL_x86, EntryPoint = "jsDouble", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsDouble_x86(double d);

        [DllImport(DLL_x64, EntryPoint = "jsDouble", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsDouble_x64(double d);

        public static long jsDouble(double d)
        {
            if (isAutoFile)
            {
                return jsDouble_auto(d);
            }
            else if (is64)
            {
                return jsDouble_x64(d);
            }

            return jsDouble_x86(d);
        }

        [DllImport(DLL_auto, EntryPoint = "jsBoolean", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsBoolean_auto(bool b);

        [DllImport(DLL_x86, EntryPoint = "jsBoolean", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsBoolean_x86(bool b);

        [DllImport(DLL_x64, EntryPoint = "jsBoolean", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsBoolean_x64(bool b);

        public static long jsBoolean(bool b)
        {
            if (isAutoFile)
            {
                return jsBoolean_auto(b);
            }
            else if (is64)
            {
                return jsBoolean_x64(b);
            }

            return jsBoolean_x86(b);
        }

        [DllImport(DLL_auto, EntryPoint = "jsUndefined", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsUndefined_auto();

        [DllImport(DLL_x86, EntryPoint = "jsUndefined", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsUndefined_x86();

        [DllImport(DLL_x64, EntryPoint = "jsUndefined", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsUndefined_x64();

        public static long jsUndefined()
        {
            if (isAutoFile)
            {
                return jsUndefined_auto();
            }
            else if (is64)
            {
                return jsUndefined_x64();
            }

            return jsUndefined_x86();
        }

        [DllImport(DLL_auto, EntryPoint = "jsString", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern long jsString_auto(IntPtr es, string str);

        [DllImport(DLL_x86, EntryPoint = "jsString", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern long jsString_x86(IntPtr es, string str);

        [DllImport(DLL_x64, EntryPoint = "jsString", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern long jsString_x64(IntPtr es, string str);

        public static long jsString(IntPtr es, string str)
        {
            if (isAutoFile)
            {
                return jsString_auto(es, str);
            }
            else if (is64)
            {
                return jsString_x64(es, str);
            }

            return jsString_x86(es, str);
        }

        [DllImport(DLL_auto, EntryPoint = "jsEmptyObject", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsEmptyObject_auto(IntPtr es);

        [DllImport(DLL_x86, EntryPoint = "jsEmptyObject", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsEmptyObject_x86(IntPtr es);

        [DllImport(DLL_x64, EntryPoint = "jsEmptyObject", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsEmptyObject_x64(IntPtr es);

        public static long jsEmptyObject(IntPtr es)
        {
            if (isAutoFile)
            {
                return jsEmptyObject_auto(es);
            }
            else if (is64)
            {
                return jsEmptyObject_x64(es);
            }

            return jsEmptyObject_x86(es);
        }

        [DllImport(DLL_auto, EntryPoint = "jsEmptyArray", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsEmptyArray_auto(IntPtr es);

        [DllImport(DLL_x86, EntryPoint = "jsEmptyArray", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsEmptyArray_x86(IntPtr es);

        [DllImport(DLL_x64, EntryPoint = "jsEmptyArray", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsEmptyArray_x64(IntPtr es);

        public static long jsEmptyArray(IntPtr es)
        {
            if (isAutoFile)
            {
                return jsEmptyArray_auto(es);
            }
            else if (is64)
            {
                return jsEmptyArray_x64(es);
            }

            return jsEmptyArray_x86(es);
        }

        [DllImport(DLL_auto, EntryPoint = "jsFunction", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsFunction_auto(IntPtr es, IntPtr obj);

        [DllImport(DLL_x86, EntryPoint = "jsFunction", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsFunction_x86(IntPtr es, IntPtr obj);

        [DllImport(DLL_x64, EntryPoint = "jsFunction", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsFunction_x64(IntPtr es, IntPtr obj);

        public static long jsFunction(IntPtr es, IntPtr obj)
        {
            if (isAutoFile)
            {
                return jsFunction_auto(es, obj);
            }
            else if (is64)
            {
                return jsFunction_x64(es, obj);
            }

            return jsFunction_x86(es, obj);
        }

        [DllImport(DLL_auto, EntryPoint = "jsGet", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern long jsGet_auto(IntPtr es, long jsValue, string prop);

        [DllImport(DLL_x86, EntryPoint = "jsGet", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern long jsGet_x86(IntPtr es, long jsValue, string prop);

        [DllImport(DLL_x64, EntryPoint = "jsGet", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern long jsGet_x64(IntPtr es, long jsValue, string prop);

        public static long jsGet(IntPtr es, long jsValue, string prop)
        {
            if (isAutoFile)
            {
                return jsGet_auto(es, jsValue, prop);
            }
            else if (is64)
            {
                return jsGet_x64(es, jsValue, prop);
            }

            return jsGet_x86(es, jsValue, prop);
        }

        [DllImport(DLL_auto, EntryPoint = "jsSet", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void jsSet_auto(IntPtr es, long jsValue, string prop, long v);

        [DllImport(DLL_x86, EntryPoint = "jsSet", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void jsSet_x86(IntPtr es, long jsValue, string prop, long v);

        [DllImport(DLL_x64, EntryPoint = "jsSet", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void jsSet_x64(IntPtr es, long jsValue, string prop, long v);

        public static void jsSet(IntPtr es, long jsValue, string prop, long v)
        {
            if (isAutoFile)
            {
                jsSet_auto(es, jsValue, prop, v);
            }
            else if (is64)
            {
                jsSet_x64(es, jsValue, prop, v);
            }
            else
            {
                jsSet_x86(es, jsValue, prop, v);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "jsGetAt", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsGetAt_auto(IntPtr es, long jsValue, int index);

        [DllImport(DLL_x86, EntryPoint = "jsGetAt", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsGetAt_x86(IntPtr es, long jsValue, int index);

        [DllImport(DLL_x64, EntryPoint = "jsGetAt", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsGetAt_x64(IntPtr es, long jsValue, int index);

        public static long jsGetAt(IntPtr es, long jsValue, int index)
        {
            if (isAutoFile)
            {
                return jsGetAt_auto(es, jsValue, index);
            }
            else if (is64)
            {
                return jsGetAt_x64(es, jsValue, index);
            }

            return jsGetAt_x86(es, jsValue, index);
        }

        [DllImport(DLL_auto, EntryPoint = "jsSetAt", CallingConvention = CallingConvention.Cdecl)]
        private static extern void jsSetAt_auto(IntPtr es, long jsValue, int index, long v);

        [DllImport(DLL_x86, EntryPoint = "jsSetAt", CallingConvention = CallingConvention.Cdecl)]
        private static extern void jsSetAt_x86(IntPtr es, long jsValue, int index, long v);

        [DllImport(DLL_x64, EntryPoint = "jsSetAt", CallingConvention = CallingConvention.Cdecl)]
        private static extern void jsSetAt_x64(IntPtr es, long jsValue, int index, long v);

        public static void jsSetAt(IntPtr es, long jsValue, int index, long v)
        {
            if (isAutoFile)
            {
                jsSetAt_auto(es, jsValue, index, v);
            }
            else if (is64)
            {
                jsSetAt_x64(es, jsValue, index, v);
            }
            else
            {
                jsSetAt_x86(es, jsValue, index, v);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "jsGetLength", CallingConvention = CallingConvention.Cdecl)]
        private static extern int jsGetLength_auto(IntPtr es, long jsValue);

        [DllImport(DLL_x86, EntryPoint = "jsGetLength", CallingConvention = CallingConvention.Cdecl)]
        private static extern int jsGetLength_x86(IntPtr es, long jsValue);

        [DllImport(DLL_x64, EntryPoint = "jsGetLength", CallingConvention = CallingConvention.Cdecl)]
        private static extern int jsGetLength_x64(IntPtr es, long jsValue);

        public static int jsGetLength(IntPtr es, long jsValue)
        {
            if (isAutoFile)
            {
                return jsGetLength_auto(es, jsValue);
            }
            else if (is64)
            {
                return jsGetLength_x64(es, jsValue);
            }
            else
            {
                return jsGetLength_x86(es, jsValue);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "jsSetLength", CallingConvention = CallingConvention.Cdecl)]
        private static extern void jsSetLength_auto(IntPtr es, long jsValue, int length);

        [DllImport(DLL_x86, EntryPoint = "jsSetLength", CallingConvention = CallingConvention.Cdecl)]
        private static extern void jsSetLength_x86(IntPtr es, long jsValue, int length);

        [DllImport(DLL_x64, EntryPoint = "jsSetLength", CallingConvention = CallingConvention.Cdecl)]
        private static extern void jsSetLength_x64(IntPtr es, long jsValue, int length);

        public static void jsSetLength(IntPtr es, long jsValue, int length)
        {
            if (isAutoFile)
            {
                jsSetLength_auto(es, jsValue, length);
            }
            else if (is64)
            {
                jsSetLength_x64(es, jsValue, length);
            }
            else
            {
                jsSetLength_x86(es, jsValue, length);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "jsGetWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr jsGetWebView_auto(IntPtr es);

        [DllImport(DLL_x86, EntryPoint = "jsGetWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr jsGetWebView_x86(IntPtr es);

        [DllImport(DLL_x64, EntryPoint = "jsGetWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr jsGetWebView_x64(IntPtr es);

        public static IntPtr jsGetWebView(IntPtr es)
        {
            if (isAutoFile)
            {
                return jsGetWebView_auto(es);
            }
            else if (is64)
            {
                return jsGetWebView_x64(es);
            }
            else
            {
                return jsGetWebView_x86(es);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "jsEvalExW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern long jsEvalExW_auto(IntPtr es, string str, [MarshalAs(UnmanagedType.I1)] bool isInClosure);

        [DllImport(DLL_x86, EntryPoint = "jsEvalExW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern long jsEvalExW_x86(IntPtr es, string str, [MarshalAs(UnmanagedType.I1)] bool isInClosure);

        [DllImport(DLL_x64, EntryPoint = "jsEvalExW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern long jsEvalExW_x64(IntPtr es, string str, [MarshalAs(UnmanagedType.I1)] bool isInClosure);

        public static long jsEvalExW(IntPtr es, string str, bool isInClosure)
        {
            if (isAutoFile)
            {
                return jsEvalExW_auto(es, str, isInClosure);
            }
            else if (is64)
            {
                return jsEvalExW_x64(es, str, isInClosure);
            }
            else
            {
                return jsEvalExW_x86(es, str, isInClosure);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "jsCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsCall_auto(IntPtr es, long func, long thisObject, [MarshalAs(UnmanagedType.LPArray)] long[] args, int argCount);

        [DllImport(DLL_x86, EntryPoint = "jsCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsCall_x86(IntPtr es, long func, long thisObject, [MarshalAs(UnmanagedType.LPArray)] long[] args, int argCount);

        [DllImport(DLL_x64, EntryPoint = "jsCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsCall_x64(IntPtr es, long func, long thisObject, [MarshalAs(UnmanagedType.LPArray)] long[] args, int argCount);

        public static long jsCall(IntPtr es, long func, long thisObject, long[] args, int argCount)
        {
            if (isAutoFile)
            {
                return jsCall_auto(es, func, thisObject, args, argCount);
            }
            else if (is64)
            {
                return jsCall_x64(es, func, thisObject, args, argCount);
            }
            else
            {
                return jsCall_x86(es, func, thisObject, args, argCount);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "jsGetGlobal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern long jsGetGlobal_auto(IntPtr es, string prop);

        [DllImport(DLL_x86, EntryPoint = "jsGetGlobal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern long jsGetGlobal_x86(IntPtr es, string prop);

        [DllImport(DLL_x64, EntryPoint = "jsGetGlobal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern long jsGetGlobal_x64(IntPtr es, string prop);

        public static long jsGetGlobal(IntPtr es, string prop)
        {
            if (isAutoFile)
            {
                return jsGetGlobal_auto(es, prop);
            }
            else if (is64)
            {
                return jsGetGlobal_x64(es, prop);
            }
            else
            {
                return jsGetGlobal_x86(es, prop);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "jsSetGlobal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void jsSetGlobal_auto(IntPtr es, string prop, long jsValue);

        [DllImport(DLL_x86, EntryPoint = "jsSetGlobal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void jsSetGlobal_x86(IntPtr es, string prop, long jsValue);

        [DllImport(DLL_x64, EntryPoint = "jsSetGlobal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void jsSetGlobal_x64(IntPtr es, string prop, long jsValue);

        public static void jsSetGlobal(IntPtr es, string prop, long jsValue)
        {
            if (isAutoFile)
            {
                jsSetGlobal_auto(es, prop, jsValue);
            }
            else if (is64)
            {
                jsSetGlobal_x64(es, prop, jsValue);
            }
            else
            {
                jsSetGlobal_x86(es, prop, jsValue);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeShowDevtools", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeShowDevtools_auto(IntPtr webView, string path, wkeOnShowDevtoolsCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeShowDevtools", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeShowDevtools_x86(IntPtr webView, string path, wkeOnShowDevtoolsCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeShowDevtools", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeShowDevtools_x64(IntPtr webView, string path, wkeOnShowDevtoolsCallback callback, IntPtr param);

        public static void wkeShowDevtools(IntPtr webView, string path, wkeOnShowDevtoolsCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeShowDevtools_auto(webView, path, callback, param);
            }
            else if (is64)
            {
                wkeShowDevtools_x64(webView, path, callback, param);
            }
            else
            {
                wkeShowDevtools_x86(webView, path, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetGetHTTPHeaderFieldFromResponse", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetHTTPHeaderFieldFromResponse_auto(IntPtr job, string key);

        [DllImport(DLL_x86, EntryPoint = "wkeNetGetHTTPHeaderFieldFromResponse", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetHTTPHeaderFieldFromResponse_x86(IntPtr job, string key);

        [DllImport(DLL_x64, EntryPoint = "wkeNetGetHTTPHeaderFieldFromResponse", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetHTTPHeaderFieldFromResponse_x64(IntPtr job, string key);

        public static IntPtr wkeNetGetHTTPHeaderFieldFromResponse(IntPtr job, string key)
        {
            if (isAutoFile)
            {
                return wkeNetGetHTTPHeaderFieldFromResponse_auto(job, key);
            }
            else if (is64)
            {
                return wkeNetGetHTTPHeaderFieldFromResponse_x64(job, key);
            }
            else
            {
                return wkeNetGetHTTPHeaderFieldFromResponse_x86(job, key);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetGetMIMEType", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetMIMEType_auto(IntPtr job, IntPtr mime);

        [DllImport(DLL_x86, EntryPoint = "wkeNetGetMIMEType", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetMIMEType_x86(IntPtr job, IntPtr mime);

        [DllImport(DLL_x64, EntryPoint = "wkeNetGetMIMEType", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetMIMEType_x64(IntPtr job, IntPtr mime);

        public static IntPtr wkeNetGetMIMEType(IntPtr job)
        {
            if (isAutoFile)
            {
                return wkeNetGetMIMEType_auto(job, IntPtr.Zero);
            }
            else if (is64)
            {
                return wkeNetGetMIMEType_x64(job, IntPtr.Zero);
            }
            else
            {
                return wkeNetGetMIMEType_x86(job, IntPtr.Zero);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetSetHTTPHeaderField", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeNetSetHTTPHeaderField_auto(IntPtr job, string key, string value, [MarshalAs(UnmanagedType.I1)] bool response);

        [DllImport(DLL_x86, EntryPoint = "wkeNetSetHTTPHeaderField", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeNetSetHTTPHeaderField_x86(IntPtr job, string key, string value, [MarshalAs(UnmanagedType.I1)] bool response);

        [DllImport(DLL_x64, EntryPoint = "wkeNetSetHTTPHeaderField", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void wkeNetSetHTTPHeaderField_x64(IntPtr job, string key, string value, [MarshalAs(UnmanagedType.I1)] bool response);

        public static void wkeNetSetHTTPHeaderField(IntPtr job, string key, string value)
        {
            if (isAutoFile)
            {
                wkeNetSetHTTPHeaderField_auto(job, key, value, false);
            }
            else if (is64)
            {
                wkeNetSetHTTPHeaderField_x64(job, key, value, false);
            }
            else
            {
                wkeNetSetHTTPHeaderField_x86(job, key, value, false);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetGetHTTPHeaderField", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetHTTPHeaderField_auto(IntPtr job, string key);

        [DllImport(DLL_x86, EntryPoint = "wkeNetGetHTTPHeaderField", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetHTTPHeaderField_x86(IntPtr job, string key);

        [DllImport(DLL_x64, EntryPoint = "wkeNetGetHTTPHeaderField", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetHTTPHeaderField_x64(IntPtr job, string key);

        public static IntPtr wkeNetGetHTTPHeaderField(IntPtr job, string key)
        {
            if (isAutoFile)
            {
                return wkeNetGetHTTPHeaderField_auto(job, key);
            }
            else if (is64)
            {
                return wkeNetGetHTTPHeaderField_x64(job, key);
            }
            else
            {
                return wkeNetGetHTTPHeaderField_x86(job, key);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetCancelRequest", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetCancelRequest_auto(IntPtr job);

        [DllImport(DLL_x86, EntryPoint = "wkeNetCancelRequest", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetCancelRequest_x86(IntPtr job);

        [DllImport(DLL_x64, EntryPoint = "wkeNetCancelRequest", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetCancelRequest_x64(IntPtr job);

        public static void wkeNetCancelRequest(IntPtr job)
        {
            if (isAutoFile)
            {
                wkeNetCancelRequest_auto(job);
            }
            else if (is64)
            {
                wkeNetCancelRequest_x64(job);
            }
            else
            {
                wkeNetCancelRequest_x86(job);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetCookie", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetCookie_auto(IntPtr webView, [MarshalAs(UnmanagedType.LPArray)] byte[] url, [MarshalAs(UnmanagedType.LPArray)] byte[] cookie);

        [DllImport(DLL_x86, EntryPoint = "wkeSetCookie", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetCookie_x86(IntPtr webView, [MarshalAs(UnmanagedType.LPArray)] byte[] url, [MarshalAs(UnmanagedType.LPArray)] byte[] cookie);

        [DllImport(DLL_x64, EntryPoint = "wkeSetCookie", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetCookie_x64(IntPtr webView, [MarshalAs(UnmanagedType.LPArray)] byte[] url, [MarshalAs(UnmanagedType.LPArray)] byte[] cookie);

        public static void wkeSetCookie(IntPtr webView, string cookie)
        {
            var c = Encoding.UTF8.GetBytes(cookie);
            if (isAutoFile)
            {
                wkeSetCookie_auto(webView, null, c);
            }
            else if (is64)
            {
                wkeSetCookie_x64(webView, null, c);
            }
            else
            {
                wkeSetCookie_x86(webView, null, c);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnPaintBitUpdated", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnPaintBitUpdated_auto(IntPtr webView, wkePaintBitUpdatedCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnPaintBitUpdated", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnPaintBitUpdated_x86(IntPtr webView, wkePaintBitUpdatedCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnPaintBitUpdated", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnPaintBitUpdated_x64(IntPtr webView, wkePaintBitUpdatedCallback callback, IntPtr param);

        public static void wkeOnPaintBitUpdated(IntPtr webView, wkePaintBitUpdatedCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnPaintBitUpdated_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnPaintBitUpdated_x64(webView, callback, param);
            }
            else
            {
                wkeOnPaintBitUpdated_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeOnLoadUrlFail", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnLoadUrlFail_auto(IntPtr webView, wkeLoadUrlFailCallback callback, IntPtr param);

        [DllImport(DLL_x86, EntryPoint = "wkeOnLoadUrlFail", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnLoadUrlFail_x86(IntPtr webView, wkeLoadUrlFailCallback callback, IntPtr param);

        [DllImport(DLL_x64, EntryPoint = "wkeOnLoadUrlFail", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnLoadUrlFail_x64(IntPtr webView, wkeLoadUrlFailCallback callback, IntPtr param);

        public static void wkeOnLoadUrlFail(IntPtr webView, wkeLoadUrlFailCallback callback, IntPtr param)
        {
            if (isAutoFile)
            {
                wkeOnLoadUrlFail_auto(webView, callback, param);
            }
            else if (is64)
            {
                wkeOnLoadUrlFail_x64(webView, callback, param);
            }
            else
            {
                wkeOnLoadUrlFail_x86(webView, callback, param);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetGetRawResponseHead", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeNetGetRawResponseHead_auto(IntPtr job);

        [DllImport(DLL_x86, EntryPoint = "wkeNetGetRawResponseHead", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeNetGetRawResponseHead_x86(IntPtr job);

        [DllImport(DLL_x64, EntryPoint = "wkeNetGetRawResponseHead", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeNetGetRawResponseHead_x64(IntPtr job);

        public static IntPtr wkeNetGetRawResponseHead(IntPtr job)
        {
            if (isAutoFile)
            {
                return wkeNetGetRawResponseHead_auto(job);
            }
            else if (is64)
            {
                return wkeNetGetRawResponseHead_x64(job);
            }

            return wkeNetGetRawResponseHead_x86(job);
        }

        [DllImport(DLL_auto, EntryPoint = "wkeEnableHighDPISupport", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEnableHighDPISupport_auto();

        [DllImport(DLL_x86, EntryPoint = "wkeEnableHighDPISupport", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEnableHighDPISupport_x86();

        [DllImport(DLL_x64, EntryPoint = "wkeEnableHighDPISupport", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEnableHighDPISupport_x64();

        public static void wkeEnableHighDPISupport()
        {
            if (isAutoFile)
            {
                wkeEnableHighDPISupport_auto();
            }
            else if (is64)
            {
                wkeEnableHighDPISupport_x64();
            }
            else
            {
                wkeEnableHighDPISupport_x86();
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeSetProxy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetProxy_auto(ref WKEProxy proxy);

        [DllImport(DLL_x86, EntryPoint = "wkeSetProxy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetProxy_x86(ref WKEProxy proxy);

        [DllImport(DLL_x64, EntryPoint = "wkeSetProxy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetProxy_x64(ref WKEProxy proxy);

        public static void wkeSetProxy(WKEProxy proxy)
        {
            if (isAutoFile)
            {
                wkeSetProxy_auto(ref proxy);
            }
            else if (is64)
            {
                wkeSetProxy_x64(ref proxy);
            }
            else
            {
                wkeSetProxy_x86(ref proxy);
            }
        }

        [DllImport(DLL_auto, EntryPoint = "wkeNetChangeRequestUrl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeNetChangeRequestUrl_auto(IntPtr job, string url);

        [DllImport(DLL_x86, EntryPoint = "wkeNetChangeRequestUrl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeNetChangeRequestUrl_x86(IntPtr job, string url);

        [DllImport(DLL_x64, EntryPoint = "wkeNetChangeRequestUrl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void wkeNetChangeRequestUrl_x64(IntPtr job, string url);

        public static void wkeNetChangeRequestUrl(IntPtr job, string url)
        {
            if (isAutoFile)
            {
                wkeNetChangeRequestUrl_auto(job, url);
            }
            else if (is64)
            {
                wkeNetChangeRequestUrl_x64(job, url);
            }
            else
            {
                wkeNetChangeRequestUrl_x86(job, url);
            }
        }
    }
}
