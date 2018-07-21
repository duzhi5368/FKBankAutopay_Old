namespace FKAutoGetNetImage
{
    partial class Main_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main_Form));
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Url = new System.Windows.Forms.TextBox();
            this.textBox_IntervalTime = new System.Windows.Forms.TextBox();
            this.textBox_StartIndex = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_DirName = new System.Windows.Forms.TextBox();
            this.button_StartDownload = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "图片Url链接：";
            // 
            // textBox_Url
            // 
            this.textBox_Url.Location = new System.Drawing.Point(14, 27);
            this.textBox_Url.Name = "textBox_Url";
            this.textBox_Url.Size = new System.Drawing.Size(343, 21);
            this.textBox_Url.TabIndex = 1;
            // 
            // textBox_IntervalTime
            // 
            this.textBox_IntervalTime.Location = new System.Drawing.Point(14, 78);
            this.textBox_IntervalTime.Name = "textBox_IntervalTime";
            this.textBox_IntervalTime.Size = new System.Drawing.Size(100, 21);
            this.textBox_IntervalTime.TabIndex = 2;
            this.textBox_IntervalTime.Text = "1";
            // 
            // textBox_StartIndex
            // 
            this.textBox_StartIndex.Location = new System.Drawing.Point(257, 78);
            this.textBox_StartIndex.Name = "textBox_StartIndex";
            this.textBox_StartIndex.Size = new System.Drawing.Size(100, 21);
            this.textBox_StartIndex.TabIndex = 3;
            this.textBox_StartIndex.Text = "1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "间隔刷新时间（s）：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(257, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "起始编号：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(134, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "图片名前缀：";
            // 
            // textBox_DirName
            // 
            this.textBox_DirName.Location = new System.Drawing.Point(134, 78);
            this.textBox_DirName.Name = "textBox_DirName";
            this.textBox_DirName.Size = new System.Drawing.Size(100, 21);
            this.textBox_DirName.TabIndex = 6;
            this.textBox_DirName.Text = "Test";
            // 
            // button_StartDownload
            // 
            this.button_StartDownload.Location = new System.Drawing.Point(123, 117);
            this.button_StartDownload.Name = "button_StartDownload";
            this.button_StartDownload.Size = new System.Drawing.Size(120, 23);
            this.button_StartDownload.TabIndex = 8;
            this.button_StartDownload.Text = "开始自动下载";
            this.button_StartDownload.UseVisualStyleBackColor = true;
            this.button_StartDownload.Click += new System.EventHandler(this.button_StartDownload_Click);
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 155);
            this.Controls.Add(this.button_StartDownload);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_DirName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_StartIndex);
            this.Controls.Add(this.textBox_IntervalTime);
            this.Controls.Add(this.textBox_Url);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main_Form";
            this.Text = "FK批量网络图片下载器";
            this.Load += new System.EventHandler(this.Main_Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Url;
        private System.Windows.Forms.TextBox textBox_IntervalTime;
        private System.Windows.Forms.TextBox textBox_StartIndex;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_DirName;
        private System.Windows.Forms.Button button_StartDownload;
    }
}

