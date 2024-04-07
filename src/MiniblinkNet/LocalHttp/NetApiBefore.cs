using System;
using System.Reflection;

namespace MiniblinkNet.LocalHttp
{
    public class NetApiBefore : EventArgs
    {
        public object Instance { get; set; }
        public MethodInfo Method { get; set; }
        public NetApiRequest Request { get; }
        public string Result { get; set; }

        internal NetApiBefore(NetApiRequest request)
        {
            Request = request;
        }
    }
}
