using System;
using System.Reflection;

namespace MiniblinkNet.LocalHttp
{
    public class NetApiError : EventArgs
    {
        public object Instance { get; internal set; }
        public MethodInfo Method { get; internal set; }
        public NetApiRequest Request { get; }
        public Exception Error { get; internal set; }
        public string Result { get; set; }

        internal NetApiError(NetApiRequest request)
        {
            Request = request;
        }
    }
}
