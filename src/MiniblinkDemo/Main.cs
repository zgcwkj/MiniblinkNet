using MiniblinkNet;
using MiniblinkNet.ResourceLoader;

namespace MiniblinkDemo
{
    internal class Main : MiniblinkForm
    {
        public Main()
        {
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.MinimumSize = new System.Drawing.Size(200, 50);
            this.Text = "Main";
            //
            this.NoneBorderResize = true;
            ShadowWidth.SetAll(1);
            Border.Set(this, 1, "#B7558E", "#CBCBCB");
            View.ResourceLoader.Add(new EmbedLoader(typeof(Main).Assembly, "wwwroot", "loc.res"));
            View.LoadUri("http://loc.res/index.html");
            //启动高DPI
            MiniblinkSetting.EnableHighDPISupport();
        }

        [JsFunc]
        private void ShowWindow()
        {
            var main = new Main();
            main.Show();
        }

        [JsFunc]
        private void ShowDialogWindow()
        {
            var main = new Main();
            main.ShowDialog();
        }

        [JsFunc]
        private void ShowDevTools()
        {
            View.ShowDevTools();
        }
    }
}
