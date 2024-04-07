using MiniblinkNet.MiniBlink;
using System;

namespace MiniblinkNet
{
    public class FrameContext
    {
        public IntPtr Id { get; }
        public bool IsMain { get; }
        public string Url { get; }
        public bool IsRemote { get; }
        private IMiniblink _mb;

        internal FrameContext(IMiniblink miniblink, IntPtr frameId)
        {
            _mb = miniblink;
            Id = frameId;
            IsMain = MBApi.wkeIsMainFrame(_mb.MiniblinkHandle, frameId);
            Url = MBApi.wkeGetFrameUrl(_mb.MiniblinkHandle, frameId).ToUTF8String();
            IsRemote = MBApi.wkeIsWebRemoteFrame(_mb.MiniblinkHandle, frameId);
        }

        public object RunJs(string script)
        {
            var es = MBApi.wkeGetGlobalExecByFrame(_mb.MiniblinkHandle, Id);
            return MBApi.jsEvalExW(es, script, true).ToValue(_mb, es);
        }
    }
}
