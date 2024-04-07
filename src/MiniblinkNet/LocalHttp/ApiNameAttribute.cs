using System;

namespace MiniblinkNet.LocalHttp
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ApiNameAttribute : Attribute
    {
        public string Name { get; }

        public ApiNameAttribute(string name)
        {
            Name = name;
        }
    }
}
