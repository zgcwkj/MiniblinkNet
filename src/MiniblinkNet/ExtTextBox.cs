using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniblinkNet
{
    public partial class ExtTextBox : UserControl
    {
        private Color _borderColor;
        /// <summary>
        /// 边框颜色
        /// </summary>
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                pnlBottom.BackColor = _borderColor;
                pnlLeft.BackColor = _borderColor;
                pnlRight.BackColor = _borderColor;
                pnlTop.BackColor = _borderColor;
            }
        }

        private int _borderWidth;
        /// <summary>
        /// 边框大小
        /// </summary>
        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                _borderWidth = value;
                var w = (pnlLeft.Width - _borderWidth) * 2;
                var h = (pnlTop.Height - _borderWidth) * 2;
                if (w > 0)
                {
                    Width -= w;
                }
                else
                {
                    Width += w * -1;
                }

                if (h > 0)
                {
                    SetHeight(Height - h);
                }
                else
                {
                    SetHeight(Height + h * -1);
                }

                pnlLeft.Width = _borderWidth;
                pnlRight.Width = _borderWidth;
                pnlTop.Height = _borderWidth;
                pnlBottom.Height = _borderWidth;
            }
        }

        private Padding _paddingWidth;
        /// <summary>
        /// 内填充大小
        /// </summary>
        public Padding PaddingWidth
        {
            get { return _paddingWidth; }
            set
            {
                _paddingWidth = value;
                var w = _paddingWidth.Horizontal + pnlLeft.Width + pnlRight.Width + txt.Width;
                var h = _paddingWidth.Vertical + pnlTop.Height + pnlBottom.Height + txt.Height;
                txt.Location = new Point(_paddingWidth.Left, _paddingWidth.Top);
                Width = w;
                SetHeight(h);
            }
        }

        public override Font Font
        {
            get { return txt.Font; }
            set { txt.Font = value; }
        }

        public override string Text
        {
            get { return txt.Text; }
            set { txt.Text = value; }
        }

        private int _height = 22;

        public ExtTextBox()
        {
            InitializeComponent();
            BorderColor = Color.LightGray;
            txt.Resize += Txt_Resize;
        }

        private void Txt_Resize(object sender, EventArgs e)
        {
            PaddingWidth = PaddingWidth;
        }

        private void SetHeight(int height)
        {
            _height = height;
            Height = _height;
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            txt.BackColor = BackColor;
            base.OnBackColorChanged(e);
        }

        protected override void OnResize(EventArgs e)
        {
            txt.Width = Width - (pnlLeft.Width + pnlRight.Width + PaddingWidth.Horizontal);
            Height = _height;
            base.OnResize(e);
        }

        private void ExtTextBox_Load(object sender, EventArgs e)
        {
            BackColor = Color.White;
        }
    }
}
