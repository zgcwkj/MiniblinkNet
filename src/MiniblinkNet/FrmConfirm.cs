using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniblinkNet
{
    public partial class FrmConfirm : Form
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

        /// <summary>
        /// 是否选择的是确定
        /// </summary>
        public bool IsOk { get; private set; }

        public FrmConfirm()
        {
            InitializeComponent();
            MessageMaxWidth = (int) (Screen.PrimaryScreen.WorkingArea.Width * 0.5);
            MessageMaxHeight = (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.7);
        }

        private void LblMsg_Resize(object sender, EventArgs e)
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

        private void BtnOk_Click(object sender, EventArgs e)
        {
            IsOk = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
