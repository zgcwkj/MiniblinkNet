using Ionic.Zip;
using MiniblinkNet.MiniBlink;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace MiniblinkNet
{
    public partial class MiniblinkBrowser
    {
        public Bitmap DrawToBitmap(Rectangle? rect = null)
        {
            if (rect.HasValue == false)
            {
                rect = new Rectangle(0, 0, ViewWidth, ViewHeight);
            }

            using (var image = new Bitmap(ViewWidth, ViewHeight))
            {
                var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                MBApi.wkePaint(MiniblinkHandle, data.Scan0, 0);
                image.UnlockBits(data);
                return image.Clone(rect.Value, PixelFormat.Format32bppArgb);
            }
        }

        public void DrawToBitmap(Action<ScreenshotImage> callback)
        {
            new DrawToBitmapUtil(this).ToImage(callback);
        }

        public void RegisterJsFunc(object target)
        {
            var tg = target;
            var methods = tg.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<JsFuncAttribute>();
                if (attr == null) continue;
                BindNetFunc(new NetFunc(attr.Name ?? method.Name, ctx =>
                {
                    var m = (MethodInfo)ctx.State;
                    object ret;
                    var mps = m.GetParameters();
                    if (mps.Length < 1)
                    {
                        ret = m.Invoke(tg, null);
                    }
                    else
                    {
                        var param = ctx.Paramters;
                        var mpvs = new object[mps.Length];
                        for (var i = 0; i < mps.Length; i++)
                        {
                            var mp = mps[i];
                            var v = param.Length > i ? param[i] : null;
                            if (v != null)
                            {
                                var pt = mp.ParameterType;
                                if (pt.IsGenericType)
                                {
                                    if (pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    {
                                        pt = pt.GetGenericArguments().First();
                                    }
                                }
                                if (pt == typeof(DateTime) && !(v is DateTime))
                                {
                                    long date;
                                    if (long.TryParse(v.ToString(), out date))
                                    {
                                        v = date.ToDate();
                                    }
                                }
                                if (v is JsFunc || pt == typeof(object) || pt == typeof(ExpandoObject))
                                {
                                    mpvs[i] = v;
                                }
                                else
                                {
                                    mpvs[i] = Convert.ChangeType(v, pt);
                                }
                            }
                            else if (mp.ParameterType.IsValueType)
                            {
                                mpvs[i] = Activator.CreateInstance(mp.ParameterType);
                            }
                            else
                            {
                                mpvs[i] = null;
                            }
                        }

                        ret = m.Invoke(tg, mpvs);
                    }

                    var t = ret as Task;
                    if (t != null)
                    {
                        if (ret.GetType().IsGenericType)
                        {
                            var frame = new DispatcherFrame();
                            t.ContinueWith(ct => frame.Continue = false);
                            Dispatcher.PushFrame(frame);
                            var p = ret.GetType().GetProperty("Result");
                            ret = p.GetValue(ret, null);
                        }
                        else
                        {
                            t.Wait();
                        }
                    }

                    return ret;
                }, method), attr.BindToSubFrame);
            }
        }

        public void ScrollTo(int x, int y)
        {
            if (IsDocumentReady)
            {
                RunJs($"window.scrollTo({x},{y})");
            }
        }

        public void SafeInvoke(Action<object> callback, object state = null)
        {
            if (IsDisposed)
            {
                return;
            }
            if (InvokeRequired)
            {
                Invoke(callback, state);
            }
            else
            {
                callback(state);
            }
        }

        public void ShowDevTools()
        {
            var dir = Path.Combine(Application.StartupPath, "front_end");
            if (Directory.Exists(dir) == false)
            {
                var zipPath = typeof(MiniblinkBrowser).Namespace + ".Files.front_end.zip";
                using (var sm = Assembly.GetExecutingAssembly().GetManifestResourceStream(zipPath))
                {
                    using (var zip = ZipFile.Read(sm))
                    {
                        zip.ExtractAll(Application.StartupPath);
                    }
                }
            }
            var path = Path.Combine(dir, "inspector.html");
            MBApi.wkeShowDevtools(MiniblinkHandle, path, null, IntPtr.Zero);
        }

        public object RunJs(string script)
        {
            var es = MBApi.wkeGlobalExec(MiniblinkHandle);
            return MBApi.jsEvalExW(es, script, true).ToValue(this, es);
        }

        public object CallJsFunc(string funcName, params object[] param)
        {
            var es = MBApi.wkeGlobalExec(MiniblinkHandle);
            var func = MBApi.jsGetGlobal(es, funcName);
            if (func == 0)
                throw new WKEFunctionNotFondException(funcName);
            var args = param.Select(i => i.ToJsValue(this, es)).ToArray();
            return MBApi.jsCall(es, func, MBApi.jsUndefined(), args, args.Length).ToValue(this, es);
        }

        public void BindNetFunc(NetFunc func, bool bindToSubFrame = false)
        {
            func.BindToSub = bindToSubFrame;
            _funcs.Add(func);

            if (_v8IsReady)
            {
                foreach (var f in _iframes)
                {
                    f.RunJs(BindFuncJs(false));
                }
                RunJs(BindFuncJs(true));
            }
        }

        public bool GoForward()
        {
            return MBApi.wkeGoForward(MiniblinkHandle);
        }

        public void EditorSelectAll()
        {
            MBApi.wkeEditorSelectAll(MiniblinkHandle);
        }

        public void EditorUnSelect()
        {
            MBApi.wkeEditorUnSelect(MiniblinkHandle);
        }

        public void EditorCopy()
        {
            MBApi.wkeEditorCopy(MiniblinkHandle);
        }

        public void EditorCut()
        {
            MBApi.wkeEditorCut(MiniblinkHandle);
        }

        public void EditorPaste()
        {
            MBApi.wkeEditorPaste(MiniblinkHandle);
        }

        public void EditorDelete()
        {
            MBApi.wkeEditorDelete(MiniblinkHandle);
        }

        public void EditorUndo()
        {
            MBApi.wkeEditorUndo(MiniblinkHandle);
        }

        public void EditorRedo()
        {
            MBApi.wkeEditorRedo(MiniblinkHandle);
        }

        public bool GoBack()
        {
            return MBApi.wkeGoBack(MiniblinkHandle);
        }

        public void SetProxy(WKEProxy proxy)
        {
            MBApi.wkeSetViewProxy(MiniblinkHandle, proxy);
        }

        public void LoadUri(string uri)
        {
            if (string.IsNullOrEmpty(uri?.Trim()))
                return;

            if (uri.SW("http:") || uri.SW("https:"))
            {
                MBApi.wkeLoadURL(MiniblinkHandle, uri);
            }
            else
            {
                MBApi.wkeLoadFileW(MiniblinkHandle, uri);
            }
        }

        public void LoadHtml(string html, string baseUrl = null)
        {
            if (baseUrl == null)
            {
                MBApi.wkeLoadHTML(MiniblinkHandle, html);
            }
            else
            {
                MBApi.wkeLoadHtmlWithBaseUrl(MiniblinkHandle, html, baseUrl);
            }
        }

        public void StopLoading()
        {
            MBApi.wkeStopLoading(MiniblinkHandle);
        }

        public void Reload()
        {
            MBApi.wkeReload(MiniblinkHandle);
        }

        public void Print(Action<PrintPreviewDialog> callback)
        {
            new PrintUtil(this).Start(callback);
        }
    }
}
