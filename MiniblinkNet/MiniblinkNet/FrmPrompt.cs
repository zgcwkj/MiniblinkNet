using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniblinkNet
{
    public partial class FrmPrompt : Form
    {
        /// <summary>
        /// 消息区的最大高度
        /// </summary>
        public int MessageMaxHeight { get; set; }

        /// <summary>
        /// 消息区的最大宽度
        /// </summary>
        public int MessageMaxWidth
        {
            get { return lblMsg.MaximumSize.Width; }
            set
            {
                lblMsg.MaximumSize = new Size(value, int.MaxValue);
            }
        }

        /// <summary>
        /// 消息区的内容
        /// </summary>
        public string Message
        {
            get { return lblMsg.Text; }
            set { lblMsg.Text = value; }
        }

        private bool _isOk;

        /// <summary>
        /// 用户输入的内容
        /// </summary>
        public string Value
        {
            get { return _isOk ? txt.Text : null; }
            set { txt.Text = value; }
        }

        public FrmPrompt()
        {
            InitializeComponent();
            MessageMaxWidth = (int)(Screen.PrimaryScreen.WorkingArea.Width * 0.5);
            MessageMaxHeight = (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.7);
        }

        private void lblMsg_Resize(object sender, EventArgs e)
        {
            if (lblMsg.Width > pnlMsg.Width)
            {
                Width = Width + lblMsg.Width - pnlMsg.Width;
            }

            if (lblMsg.Height > pnlMsg.Height)
            {
                var nh = Height + lblMsg.Height - pnlMsg.Height;
                if (nh > MessageMaxHeight)
                {
                    nh = MessageMaxHeight;
                    Width += 20;
                }
                Height = nh;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _isOk = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmPrompt_Load(object sender, EventArgs e)
        {
            ActiveControl = txt;
        }
    }
}
