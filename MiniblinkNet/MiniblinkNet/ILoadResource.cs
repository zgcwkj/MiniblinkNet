using System;

namespace MiniblinkNet
{
    public interface ILoadResource
    {
        byte[] ByUri(Uri uri);
        string Domain { get; }
    }
}
