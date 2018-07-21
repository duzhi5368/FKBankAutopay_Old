namespace FKCashierTPP
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.textBox_Cmd = new System.Windows.Forms.TextBox();
            this.button_DoCmd = new System.Windows.Forms.Button();
            this.button_Help = new System.Windows.Forms.Button();
            this.Log_richTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textBox_Cmd
            // 
            this.textBox_Cmd.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.textBox_Cmd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Cmd.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_Cmd.Location = new System.Drawing.Point(12, 527);
            this.textBox_Cmd.MaxLength = 100;
            this.textBox_Cmd.Name = "textBox_Cmd";
            this.textBox_Cmd.Size = new System.Drawing.Size(467, 29);
            this.textBox_Cmd.TabIndex = 6;
            // 
            // button_DoCmd
            // 
            this.button_DoCmd.BackColor = System.Drawing.Color.White;
            this.button_DoCmd.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_DoCmd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_DoCmd.Location = new System.Drawing.Point(521, 527);
            this.button_DoCmd.Name = "button_DoCmd";
            this.button_DoCmd.Size = new System.Drawing.Size(75, 34);
            this.button_DoCmd.TabIndex = 5;
            this.button_DoCmd.Text = "执行";
            this.button_DoCmd.UseVisualStyleBackColor = false;
            // 
            // button_Help
            // 
            this.button_Help.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.button_Help.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_Help.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Help.ForeColor = System.Drawing.Color.Red;
            this.button_Help.Location = new System.Drawing.Point(485, 529);
            this.button_Help.Name = "button_Help";
            this.button_Help.Size = new System.Drawing.Size(30, 23);
            this.button_Help.TabIndex = 7;
            this.button_Help.Text = "?";
            this.button_Help.UseVisualStyleBackColor = false;
            // 
            // Log_richTextBox
            // 
            this.Log_richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Log_richTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.Log_richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Log_richTextBox.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Log_richTextBox.ForeColor = System.Drawing.SystemColors.Menu;
            this.Log_richTextBox.Location = new System.Drawing.Point(12, 12);
            this.Log_richTextBox.Name = "Log_richTextBox";
            this.Log_richTextBox.ReadOnly = true;
            this.Log_richTextBox.Size = new System.Drawing.Size(584, 506);
            this.Log_richTextBox.TabIndex = 4;
            this.Log_richTextBox.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 563);
            this.Controls.Add(this.textBox_Cmd);
            this.Controls.Add(this.button_DoCmd);
            this.Controls.Add(this.button_Help);
            this.Controls.Add(this.Log_richTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FK自动收付款节点【第三方】";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_Cmd;
        private System.Windows.Forms.Button button_DoCmd;
        private System.Windows.Forms.Button button_Help;
        private System.Windows.Forms.RichTextBox Log_richTextBox;
    }
}

