using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;

namespace MiniblinkNet.MiniBlink
{
    public class CookieCollection
    {
        public int Count
        {
            get { return _container.Count; }
        }

        private bool _enable;

        public bool Enable
        {
            get { return _enable; }
            set
            {
                if (_enable != value)
                {
                    if (value)
                    {
                        _miniblink.RequestBefore -= ClearCookie;
                    }
                    else
                    {
                        MBApi.wkePerformCookieCommand(_miniblink.MiniblinkHandle, wkeCookieCommand.ClearAllCookies);
                        MBApi.wkePerformCookieCommand(_miniblink.MiniblinkHandle, wkeCookieCommand.ClearSessionCookies);
                        MBApi.wkePerformCookieCommand(_miniblink.MiniblinkHandle, wkeCookieCommand.FlushCookiesToFile);
                        MBApi.wkePerformCookieCommand(_miniblink.MiniblinkHandle, wkeCookieCommand.ReloadCookiesFromFile);
                        _miniblink.RequestBefore += ClearCookie;
                        _container = new CookieContainer();
                    }
                }

                _enable = value;
            }
        }

        private CookieContainer _container = new CookieContainer();
        private IMiniblink _miniblink;
        private string _file;

        internal CookieCollection(IMiniblink miniblink, string path)
        {
            _file = path;
            _miniblink = miniblink;
            _miniblink.NavigateBefore += NavigateBefore;
            _enable = true;
        }

        private void ClearCookie(object sender, RequestEventArgs e)
        {
            MBApi.wkePerformCookieCommand(_miniblink.MiniblinkHandle, wkeCookieCommand.ClearAllCookies);
            MBApi.wkePerformCookieCommand(_miniblink.MiniblinkHandle, wkeCookieCommand.ClearSessionCookies);
        }

        private void NavigateBefore(object sender, NavigateEventArgs e)
        {
            if (Enable == false) return;
            if (_miniblink.Url == e.Url)
            {
                _container = new CookieContainer();
            }
        }

        private void Reload(string host)
        {
            if (File.Exists(_file) == false)
            {
                return;
            }
            MBApi.wkePerformCookieCommand(_miniblink.MiniblinkHandle, wkeCookieCommand.FlushCookiesToFile);
            _container = new CookieContainer();
            var rows = File.ReadAllLines(_file, Encoding.UTF8);
            foreach (var row in rows)
            {
                if (row.StartsWith("# ")) continue;
                var items = row.Split('\t');
                if (items.Length != 7) continue;
                var domain = items[0].ToLower();
                var httpOnly = domain.StartsWith("#HttpOnly_", StringComparison.OrdinalIgnoreCase);
                if (httpOnly)
                {
                    domain = domain.Substring(domain.IndexOf("_", StringComparison.Ordinal) + 1).ToLower();
                }

                if (domain.StartsWith("."))
                {
                    if (host.EndsWith(domain) == false && ("." + host).Equals(domain) == false)
                    {
                        continue;
                    }
                }
                else if (host.Equals(domain) == false)
                {
                    continue;
                }

                if ("0" == items[4])
                {
                    items[4] = "1200";
                }

                var cookie = new Cookie
                {
                    HttpOnly = httpOnly,
                    Domain = domain.TrimStart('.'),
                    Path = items[2],
                    Secure = "true".Equals(items[3], StringComparison.OrdinalIgnoreCase),
                    Expires = new DateTime(1970, 1, 1).AddSeconds(long.Parse(items[4])),
                    Name = items[5].Split('=')[0],
                    Value = Utils.UrlEncode(items[6])
                };

                _container.Add(cookie);
            }
        }

        public ReadOnlyCollection<Cookie> GetCookies(string url = null)
        {
            var uri = new Uri(url ?? _miniblink.Url);
            Reload(uri.Host);
            var list = new List<Cookie>();
            foreach (Cookie item in _container.GetCookies(uri))
            {
                if (list.Contains(item) == false)
                {
                    list.Add(item);
                }
            }

            return list.AsReadOnly();
        }

        private static string GetCurlCookie(Cookie cookie)
        {
            var ck = $"{cookie.Name}={cookie.Value};expires={cookie.Expires:R};domain={cookie.Domain};path={cookie.Path};";
            if (cookie.Secure)
            {
                ck += "secure;";
            }

            if (cookie.HttpOnly)
            {
                ck += "httponly;";
            }

            return ck;
        }

        public void Add(Cookie cookie)
        {
            if (cookie == null) return;
            if (string.IsNullOrEmpty(cookie.Path))
            {
                cookie.Path = "/";
            }

            if (string.IsNullOrEmpty(cookie.Domain))
            {
                cookie.Domain = new Uri(_miniblink.Url).Host;
            }
            MBApi.wkeSetCookie(_miniblink.MiniblinkHandle, GetCurlCookie(cookie));
            _container.Add(cookie);
        }

        public void Clear()
        {
            var ck = "";
            foreach (var cookie in GetCookies())
            {
                cookie.Expires = DateTime.MinValue;
                ck += GetCurlCookie(cookie);
            }

            MBApi.wkeSetCookie(_miniblink.MiniblinkHandle, ck);
            MBApi.wkePerformCookieCommand(_miniblink.MiniblinkHandle, wkeCookieCommand.FlushCookiesToFile);
            _container = new CookieContainer();
        }

        public bool Contains(Cookie cookie)
        {
            if (cookie == null) return false;

            if (string.IsNullOrEmpty(cookie.Path))
            {
                cookie.Path = "/";
            }

            if (string.IsNullOrEmpty(cookie.Domain))
            {
                cookie.Domain = new Uri(_miniblink.Url).Host;
            }
            var list = _container.GetCookies(new Uri("http://" + cookie.Domain + cookie.Path));
            foreach (Cookie item in list)
            {
                if (cookie.Name == item.Name && cookie.Value == item.Value && cookie.Path == item.Path)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Remove(Cookie cookie)
        {
            if (cookie == null) return false;

            if (string.IsNullOrEmpty(cookie.Path))
            {
                cookie.Path = "/";
            }

            if (string.IsNullOrEmpty(cookie.Domain))
            {
                cookie.Domain = new Uri(_miniblink.Url).Host;
            }

            if (Contains(cookie))
            {
                cookie.Expires = DateTime.MinValue;
                var ck = GetCurlCookie(cookie);
                MBApi.wkeSetCookie(_miniblink.MiniblinkHandle, ck);
                return true;
            }

            return false;
        }
    }
}
