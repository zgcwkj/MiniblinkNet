using System;
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
        /// 窗体边框
        /// </summary>
        internal FormBorder()
        {

        }

        /// <summary>
        /// 边框大小
        /// </summary>
        public int Size { get; set; } = 0;

        /// <summary>
        /// 焦点状态
        /// </summary>
        public bool FocusState { get; set; } = true;

        /// <summary>
        /// 边框颜色
        /// </summary>
        public Color Color { get; set; } = Color.Red;

        /// <summary>
        /// 失去焦点时颜色
        /// </summary>
        public Color NoColor { get; set; } = Color.WhiteSmoke;

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="form">窗体</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        /// <param name="noColor">失去焦点时颜色</param>
        public void Set(dynamic form, int size, Color color)
        {
            this.Set(form, new Padding(size), size, color, Color.WhiteSmoke);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="form">窗体</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色十六进制</param>
        /// <param name="noColor">失去焦点时颜色十六进制</param>
        public void Set(dynamic form, int size, string color, string noColor)
        {
            this.Set(form, new Padding(size), size, ColorTranslator.FromHtml(color), ColorTranslator.FromHtml(noColor));
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="form">窗体</param>
        /// <param name="padding">填充</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色十六进制</param>
        /// <param name="noColor">失去焦点时颜色十六进制</param>
        public void Set(dynamic form, Padding padding, int size, string color, string noColor)
        {
            this.Set(form, padding, size, ColorTranslator.FromHtml(color), ColorTranslator.FromHtml(noColor));
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="form">窗体</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        /// <param name="noColor">失去焦点时颜色</param>
        public void Set(dynamic form, Padding padding, int size, Color color, Color noColor)
        {
            Size = size;
            Color = color;
            NoColor = noColor;
            ((Form)form).Padding = padding;
            ((Form)form).Paint += (s, e) => Paint(s, e, form);
            ((Form)form).Activated += (s, e) => FormActivated(s, e, form);
            ((Form)form).Deactivate += (s, e) => FormDeactivate(s, e, form);
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        private void Paint(object s, PaintEventArgs e, dynamic form)
        {
            Pen pen = new Pen(FocusState ? Color : NoColor, Size);
            e.Graphics.DrawRectangle(pen, Size / 2, Size / 2, form.Width - Size, form.Height - Size);
        }

        /// <summary>
        /// 获得焦点
        /// </summary>
        private void FormActivated(object sender, EventArgs e, dynamic form)
        {
            FocusState = true;
            form.Invalidate();
        }

        /// <summary>
        /// 失去焦点
        /// </summary>
        private void FormDeactivate(object sender, EventArgs e, dynamic form)
        {
            FocusState = false;
            form.Invalidate();
        }
    }
}