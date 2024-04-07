using MiniblinkNet.MiniBlink;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MiniblinkNet
{
    public class Downloader
    {
        private IMiniblink _miniblink;
        private bool _cancel;

        internal Downloader(IMiniblink miniblink)
        {
            _miniblink = miniblink;
            _miniblink.Destroy += _miniblink_Destroy;
        }

        private void _miniblink_Destroy(object sender, EventArgs e)
        {
            _miniblink.Destroy -= _miniblink_Destroy;
            _cancel = true;
        }

        public DownloadEventArgs Create(string url)
        {
            var http = (HttpWebRequest)WebRequest.Create(url);
            http.Method = "get";
            http.AllowAutoRedirect = true;
            http.UserAgent = _miniblink.UserAgent;
            foreach (var item in _miniblink.Cookies.GetCookies(url))
            {
                http.CookieContainer.Add(item);
            }
            var resp = http.GetResponse();
            var e = new DownloadEventArgs(url)
            {
                FileLength = resp.ContentLength,
                Response = resp
            };
            return e;
        }

        private static void SaveToFile(DownloadEventArgs e)
        {
            var path = Path.Combine(Environment.GetEnvironmentVariable("TEMP") ?? "", Guid.NewGuid().ToString());
            var file = File.Create(path);
            e.Progress += (s, p) => { file.Write(p.Data, 0, p.Data.Length); };
            e.Finish += (s, p) =>
            {
                file.Close();

                if (p.Error == null)
                {
                    File.Move(path, e.FilePath);
                }
                else
                {
                    File.Delete(path);
                }
            };
        }

        public void Execute(DownloadEventArgs e)
        {
            if (e.Cancel)
            {
                e.Response?.Close();
                return;
            }

            if (e.FilePath != null)
            {
                SaveToFile(e);
            }
            Task.Factory.StartNew(() =>
            {
                var rec = 0L;
                using (var sm = e.Response.GetResponseStream())
                {
                    using (var buf = new MemoryStream(1024 * 1024 * 2))
                    {
                        int len;
                        var log = DateTime.Now.Ticks;
                        var data = new byte[1024 * 512];
                        while ((len = sm.Read(data, 0, data.Length)) > 0 && _cancel == false)
                        {
                            buf.Write(data, 0, len);
                            rec += len;
                            if (rec > e.FileLength)
                            {
                                throw new Exception("数据异常，无法下载");
                            }

                            if (new TimeSpan(DateTime.Now.Ticks - log).TotalSeconds >= 1 || rec == e.FileLength)
                            {
                                var pres = new DownloadProgressEventArgs
                                {
                                    Total = e.FileLength,
                                    Received = rec,
                                    Data = buf.ToArray()
                                };
                                if (_cancel == false)
                                {
                                    _miniblink.SafeInvoke(s => { e.OnProgress((DownloadProgressEventArgs)s); }, pres);
                                }

                                buf.SetLength(0);
                                buf.Position = 0;
                                log = DateTime.Now.Ticks;
                                if (pres.Cancel)
                                {
                                    _cancel = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }).ContinueWith(t =>
            {
                _miniblink.SafeInvoke(s =>
                {
                    e.OnFinish(new DownloadFinishEventArgs
                    {
                        Error = t.Exception?.GetBaseException(),
                        IsCompleted = _cancel == false
                    });
                });

                e.Response?.Close();
            });
        }
    }
}
