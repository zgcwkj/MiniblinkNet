using System;

namespace MiniblinkNet.LocalHttp
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        public string Path { get; }
        public RequestType Type { get; }

        public RouteAttribute(RequestType type, string path)
        {
            Type = type;
            Path = path;
        }
    }

    public class GetAttribute : RouteAttribute
    {
        public GetAttribute(string path = null)
            : base(RequestType.GET, path)
        {
        }
    }

    public class PostAttribute : RouteAttribute
    {
        public PostAttribute(string path = null)
            : base(RequestType.POST, path)
        {
        }
    }
}
