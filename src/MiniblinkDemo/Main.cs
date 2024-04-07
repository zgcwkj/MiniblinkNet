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
            this.Text = "Main";
            //
            MiniblinkSetting.EnableHighDPISupport();
            View.ResourceLoader.Add(new EmbedLoader(typeof(Main).Assembly, "wwwroot", "loc.res"));
            View.LoadUri("http://loc.res/index.html");
            //View.LoadUri("http://zgcwkj.cn");

            View.ShowDevTools();
        }


    }
}
