using MiniblinkNet.MiniBlink;
using System;
using System.Linq;

namespace MiniblinkNet
{
    public delegate object JsFunc(params object[] param);

    public class JsFuncWapper
    {
        private string _name;
        private IMiniblink _miniblink;

        internal JsFuncWapper(IMiniblink control, long jsvalue, IntPtr es)
        {
            _miniblink = control;
            _name = "func" + Guid.NewGuid().ToString().Replace("-", "");
            MBApi.jsSetGlobal(es, _name, jsvalue);
        }

        public object Call(params object[] param)
        {
            object result = null;

            _miniblink.SafeInvoke(s =>
            {
                var es = MBApi.wkeGlobalExec(_miniblink.MiniblinkHandle);
                var value = MBApi.jsGetGlobal(es, _name);
                var jsps = param.Select(i => i.ToJsValue(_miniblink, es)).ToArray();
                result = MBApi.jsCall(es, value, MBApi.jsUndefined(), jsps, jsps.Length).ToValue(_miniblink, es);
                MBApi.jsSetGlobal(es, _name, MBApi.jsUndefined());
            });

            return result;
        }
    }
}
