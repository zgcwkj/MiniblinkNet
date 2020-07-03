using System.Drawing;
using System.Windows.Forms;

namespace MiniblinkNet
{
    /// <summary>
    /// 窗体边框
    /// </summary>
    public class FormBorder
    {
        /// <summary>
        /// 边框大小
        /// </summary>
        public int Size { get; set; } = 0;

        /// <summary>
        /// 边框颜色
        /// </summary>
        public Color Color { get; set; } = Color.Red;

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="form">窗体</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        public void Set(dynamic form, int size, Color color)
        {
            form.Padding = new Padding(size);
            Size = size;
            Color = color;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="form">窗体</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色十六进制</param>
        public void Set(dynamic form, int size, string color)
        {
            form.Padding = new Padding(size);
            Size = size;
            Color = ColorTranslator.FromHtml(color);
        }

        /// <summary>
        /// 窗体边框
        /// </summary>
        internal FormBorder()
        {

        }
    }
}