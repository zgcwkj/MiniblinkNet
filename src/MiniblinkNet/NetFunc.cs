using MiniblinkNet.MiniBlink;
using System;

namespace MiniblinkNet
{
    public delegate object TempNetFunc(object[] param);

    public delegate object NetFuncDelegate(NetFuncContext context);

    public class NetFuncContext
    {
        public string Name { get; internal set; }
        public object State { get; internal set; }
        public IMiniblink Miniblink { get; internal set; }
        public object[] Paramters { get; internal set; }
    }

    public class NetFunc
    {
        public string Name { get; }

        internal wkeJsNativeFunction JsFunc;
        internal bool BindToSub;

        private NetFuncDelegate _func;
        private object _state;

        public NetFunc(string name, NetFuncDelegate func, object state = null)
        {
            Name = name;
            _func = func;
            _state = state;
        }

        internal object OnFunc(IMiniblink miniblink, object[] param)
        {
            return _func(new NetFuncContext
            {
                Name = Name,
                Miniblink = miniblink,
                State = _state,
                Paramters = param ?? new object[0]
            });
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class JsFuncAttribute : Attribute
    {
        public string Name { get; }
        public bool BindToSubFrame { get; set; }

        public JsFuncAttribute(string functionName = null)
        {
            Name = functionName;
        }
    }
}
