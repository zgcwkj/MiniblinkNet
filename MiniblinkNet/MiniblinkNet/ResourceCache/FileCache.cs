using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MiniblinkNet.ResourceCache
{
    public class FileCache : IResourceCache
    {
        public List<string> UrlRegex { get; }
        public int SlidingMinute { get; set; }

        public FileCache()
        {
            SlidingMinute = 30;
            UrlRegex = new List<string>();
        }

        public bool Matchs(string url)
        {
            if (UrlRegex.Count > 0)
            {
                foreach (var item in UrlRegex)
                {
                    if (Regex.IsMatch(url, item, RegexOptions.IgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public byte[] Get(string url)
        {
            throw new NotImplementedException();
        }

        public void Save(string url, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
