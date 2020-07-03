using System;
using System.Reflection;

namespace MiniblinkNet.LocalHttp
{
    public class NetApiAfter : EventArgs
    {
        public object Instance { get; internal set; }
        public MethodInfo Method { get; internal set; }
        public NetApiRequest Request { get; }
        public string Result { get; internal set; }

        internal NetApiAfter(NetApiRequest request)
        {
            Request = request;
        }
    }
}
