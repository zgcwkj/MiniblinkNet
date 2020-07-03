namespace MiniblinkNet
{
    partial class FrmConfirm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOk = new System.Windows.Forms.Button();
            this.pnlMsg = new System.Windows.Forms.Panel();
            this.lblMsg = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlMsg.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(64, 88);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(85, 27);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "确 定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // pnlMsg
            // 
            this.pnlMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMsg.AutoScroll = true;
            this.pnlMsg.BackColor = System.Drawing.Color.White;
            this.pnlMsg.Controls.Add(this.lblMsg);
            this.pnlMsg.Location = new System.Drawing.Point(0, 0);
            this.pnlMsg.Name = "pnlMsg";
            this.pnlMsg.Size = new System.Drawing.Size(251, 77);
            this.pnlMsg.TabIndex = 1;
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.BackColor = System.Drawing.Color.Transparent;
            this.lblMsg.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMsg.Location = new System.Drawing.Point(0, 0);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Padding = new System.Windows.Forms.Padding(10, 30, 10, 30);
            this.lblMsg.Size = new System.Drawing.Size(80, 77);
            this.lblMsg.TabIndex = 0;
            this.lblMsg.Text = "message";
            this.lblMsg.Resize += new System.EventHandler(this.LblMsg_Resize);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(156, 88);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 27);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取 消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FrmConfirm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 127);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.pnlMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfirm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.pnlMsg.ResumeLayout(false);
            this.pnlMsg.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel pnlMsg;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Button btnCancel;
    }
}