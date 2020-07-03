using MiniblinkNet.MiniBlink;
using MiniblinkNet.MiniBlink.Interface;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MiniblinkNet
{
    public static class MiniblinkSetting
    {
        private static Dictionary<string, NetFunc> _ref = new Dictionary<string, NetFunc>();
        private static Dictionary<long, IMiniblink> _miniblinks = new Dictionary<long, IMiniblink>();

        public static void EnableHighDPISupport()
        {
            MBApi.wkeEnableHighDPISupport();
        }

        public static void SetProxy(WKEProxy proxy)
        {
            MBApi.wkeSetProxy(proxy);
        }

        internal static IntPtr CreateWebView(IMiniblink miniblink)
        {
            var handle = MBApi.wkeCreateWebView();
            _miniblinks[handle.ToInt64()] = miniblink;
            return handle;
        }

        internal static void DestroyWebView(IntPtr handle)
        {
            _miniblinks.Remove(handle.ToInt64());
        }

        public static void BindNetFunc(NetFunc func)
        {
            func.jsFunc = new wkeJsNativeFunction((es, state) =>
            {
                var mb = MBApi.jsGetWebView(es);
                if (_miniblinks.ContainsKey(mb.ToInt64()) == false)
                {
                    return 0;
                }
                var bro = _miniblinks[mb.ToInt64()];
                var handle = GCHandle.FromIntPtr(state);
                var nfunc = (NetFunc)handle.Target;
                var arglen = MBApi.jsArgCount(es);
                var args = new List<object>();
                for (var i = 0; i < arglen; i++)
                {
                    args.Add(MBApi.jsArg(es, i).ToValue(bro, es));
                }

                return nfunc.OnFunc(bro, args.ToArray()).ToJsValue(bro, es);
            });
            _ref[func.Name] = func;

            var ptr = GCHandle.ToIntPtr(GCHandle.Alloc(func));

            MBApi.wkeJsBindFunction(func.Name, func.jsFunc, ptr, 0);
        }
    }
}
