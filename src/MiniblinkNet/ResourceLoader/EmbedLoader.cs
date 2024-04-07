using System;
using System.Linq;
using System.Reflection;

namespace MiniblinkNet.ResourceLoader
{
    public class EmbedLoader : ILoadResource
    {
        private Assembly _assembly;
        private string _asmDir;
        private string _namespace;
        private string _domain;

        public EmbedLoader(Assembly resAssembly, string resAsmDir, string domain)
        {
            _assembly = resAssembly;
            _asmDir = resAsmDir;
            _domain = domain;
            _namespace = resAssembly.EntryPoint.DeclaringType?.Namespace;
        }

        public byte[] ByUri(Uri uri)
        {
            var items = uri.Segments.Take(uri.Segments.Length - 1).Select(i => i.Replace("-", "_"));
            var dir = string.Join("", items).TrimStart('/').Replace("/", ".");
            var path = string.Join(".", _namespace, _asmDir, dir + uri.Segments.Last());

            using (var sm = _assembly.GetManifestResourceStream(path))
            {
                if (sm == null)
                {
                    return null;
                }

                var data = new byte[sm.Length];
                sm.Read(data, 0, data.Length);
                return data;
            }
        }

        public string Domain => _domain;
    }
}
