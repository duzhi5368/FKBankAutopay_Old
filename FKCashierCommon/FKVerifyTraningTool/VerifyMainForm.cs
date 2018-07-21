using FKVerifyLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
//------------------------------------------------------------
namespace FKVerifyTraningTool
{
    public partial class VerifyMainForm : Form
    {
        #region 类变量
        private CodeHelper GlobalCH = null;
        private Image SrcImage = null;
        private Image BinaryImage = null;           // 二值化图像
        private Image NoNoiseImage = null;          // 去噪图像
        private Image ReverseImage = null;          // 反色图像
        private bool m_bIsAllowUpdateConfig = true; // 当前是否允许自动更新配置数据
        private List<Rectangle> CharRects = new List<Rectangle>();
        private List<Image> CharImages = new List<Image>();
        private string VerifyValue = "";
        private int CostTime = 0;
        #endregion
        public VerifyMainForm()
        {
            InitializeComponent();

            GlobalCH = new CodeHelper();

            CodeInfo defaultInfo = new CodeInfo();
            defaultInfo.ClearCode();
            defaultInfo.Url = "";
            defaultInfo.ImageTemp = null;
            defaultInfo.CodeCount = 0;
            defaultInfo.RectangleCut = new Rectangle(0, 0, 100, 100);
            defaultInfo.PixelMin = 8;
            defaultInfo.PixelMax = 120;
            defaultInfo.NoiseLv = 0;
            defaultInfo.IsAutoSelectRect = true;
            defaultInfo.CodeRectangles.Clear();
            defaultInfo.BinaryValues = Enumerable.Repeat((byte)127, 1).ToArray();

            GlobalCH.LoadCodeInfo(defaultInfo);
        }

        #region 自定义绘制函数
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
        }
        private void groupBox2_Paint(object sender, PaintEventArgs e)
        {
            // 绘制背景色
            int x = 7;
            int y = 43;
            int width = 380;
            int height = 540;
            e.Graphics.FillRectangle(new SolidBrush(Color.Gray), x, y, width, height);


            Font textFont = new Font("Arial", 12);
            SolidBrush textBrush = new SolidBrush(Color.Yellow);

            if (SrcImage != null)
            {
                Rectangle srcRect = new Rectangle(x + 5, y + 5, SrcImage.Width * 2, SrcImage.Height * 2);
                e.Graphics.DrawImage(SrcImage, srcRect);
                e.Graphics.DrawString("原始图像", textFont, textBrush, 370 - 4 * 20 ,y + 5);
            }
            if(BinaryImage != null)
            {
                Rectangle srcRect = new Rectangle(x + 5, y + 10 + SrcImage.Height * 2, 
                    BinaryImage.Width * 2, BinaryImage.Height * 2);
                e.Graphics.DrawImage(BinaryImage, srcRect);
                e.Graphics.DrawString("二值化图像", textFont, textBrush, 370 - 5 * 20, y + 10 + SrcImage.Height * 2);
            }
            if(NoNoiseImage != null)
            {
                Rectangle srcRect = new Rectangle(x + 5, y + 10 + SrcImage.Height * 2 + 5 + BinaryImage.Height * 2,
                    NoNoiseImage.Width * 2, NoNoiseImage.Height * 2);
                e.Graphics.DrawImage(NoNoiseImage, srcRect);
                e.Graphics.DrawString("去噪图像", textFont, textBrush, 370 - 4 * 20, y + 10 + SrcImage.Height * 2 + 5 + BinaryImage.Height * 2);
            }
            if(ReverseImage != null)
            {
                Rectangle srcRect = new Rectangle(x + 5, y + 10 + SrcImage.Height * 2 + 5 + NoNoiseImage.Height * 2 + 5 + NoNoiseImage.Height * 2,
                    ReverseImage.Width * 2, ReverseImage.Height * 2);
                e.Graphics.DrawImage(ReverseImage, srcRect);
                e.Graphics.DrawString("反色图像", textFont, textBrush, 370 - 4 * 20, y + 10 + SrcImage.Height * 2 + 5 + BinaryImage.Height * 2 + 5 + NoNoiseImage.Height * 2);
            }
            if(CharRects.Count > 0)
            {
                Rectangle srcRect = new Rectangle(x + 5, y + 10 + SrcImage.Height * 2 + 5 + BinaryImage.Height * 2 + 5 + NoNoiseImage.Height * 2 + 5 + ReverseImage.Height * 2,
                    ReverseImage.Width * 2, ReverseImage.Height * 2);
                e.Graphics.DrawImage(ReverseImage, srcRect);
                e.Graphics.DrawString("区域选择", textFont, textBrush, 370 - 4 * 20, y + 10 + SrcImage.Height * 2 + 5 + BinaryImage.Height * 2 + 5 + NoNoiseImage.Height * 2 + 5 + ReverseImage.Height * 2);

                for (int i = 0; i < CharRects.Count; ++i)
                {
                    Pen redPen = new Pen(Color.Red, 1);
                    Rectangle rect = CharRects[i];
                    rect.X *= 2;
                    rect.Y *= 2;
                    rect.Width *= 2;
                    rect.Height *= 2;
                    rect.X += x + 5;
                    rect.Y += y + 10 + SrcImage.Height * 2 + 5 + BinaryImage.Height * 2 + 5 + ReverseImage.Height * 2 + 5 + NoNoiseImage.Height * 2;
                    e.Graphics.DrawRectangle(redPen, rect);

                    // 单字符记录
                    CharImages.Insert(i, cropImage(ReverseImage, CharRects[i]));
                }
            }
            if(CharImages.Count > 0)
            {
                e.Graphics.DrawString("最终字符", textFont, textBrush, 370 - 4 * 20, 
                    y + 10 + SrcImage.Height * 2 + 5 + BinaryImage.Height * 2 + 5 + BinaryImage.Height * 2 + 5 + NoNoiseImage.Height * 2 + 5 + ReverseImage.Height * 2 );
                int nXBegin = x + 5;
                for (int i = 0; i < CharImages.Count; ++i)
                {
                    Rectangle srcRect = new Rectangle(nXBegin, y + 10 + SrcImage.Height * 2 + 5 + BinaryImage.Height * 2 + 5 + ReverseImage.Height * 2 + 5 + NoNoiseImage.Height * 2 + 5 + ReverseImage.Height * 2,
                        CharImages[i].Width * 2, CharImages[i].Height * 2);
                    e.Graphics.DrawImage(CharImages[i], srcRect);
                    nXBegin += (CharImages[i].Width * 2 + 10);
                }
            }
            if(VerifyValue != "")
            {
                string str = string.Format("验证码为： {0}   消耗时间: {1}毫秒", VerifyValue, CostTime);
                e.Graphics.DrawString(str, textFont, textBrush, 5, height + y - 20);
            }
            else
            {
                if(CharImages.Count > 0)
                {
                    string str = string.Format("验证码为： ****   消耗时间: {0}毫秒", CostTime);
                    e.Graphics.DrawString(str, textFont, textBrush, 5, height + y - 20);
                }
            }
        }
        private Image cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }
        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            // 绘制背景色
            int x = 55;
            int y = 355;
            int width = 64;
            int height = 64;
            e.Graphics.FillRectangle(new SolidBrush(Color.Gray), x, y, width, height);
        }
        #endregion

        #region 界面操作回调
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = trackBar1.Value.ToString();
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            UpdateConfig();
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            UpdateConfig();
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            UpdateConfig();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string resultFile = "";

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "D:\\";
            openFileDialog1.Filter = "FK验证码配置文件 (*.fkc.png)|*.fkc.png|FK验证码配置文件 (*.fkc)|*.fkc|全部文件 (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                resultFile = openFileDialog1.FileName;
                textBox1.Text = resultFile;
                LoadConfigFromFile(textBox1.Text);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            //设置文件类型 
            if (GlobalCH.m_ci.ImageTemp == null)
            {
                sfd.Filter = "FK验证码配置文件 (*.fkc)|*.fkc";
            }
            else
            {
                sfd.Filter = "FK验证码配置文件 (*.fkc.png)|*.fkc.png";
            }
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string localFilePath = sfd.FileName.ToString();
                SaveConfigToFile(localFilePath);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string resultFile = "";

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "D:\\";
            openFileDialog1.Filter = "图片文件 (*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG|全部文件 (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                resultFile = openFileDialog1.FileName;
                textBox5.Text = resultFile;
                LoadVerifyPic(resultFile);
            }
        }
        private void button4_Click_1(object sender, EventArgs e)
        {
            AutoAddVerify();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string resultFile = "";

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "D:\\";
            openFileDialog1.Filter = "FK验证码配置文件 (*.fkc.png)|*.fkc.png";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                resultFile = openFileDialog1.FileName;

            textBox1.Text = resultFile;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            string resultFile = "";

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "D:\\";
            openFileDialog1.Filter = "图片文件 (*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG|全部文件 (*.*)|*.* ";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                resultFile = openFileDialog1.FileName;

            textBox5.Text = resultFile;
        }
        /// <summary>
        /// 批量训练按钮
        /// </summary>
        /// 使用方式：D:\Work\Temp\Dir{0}.png@1@100
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click_1(object sender, EventArgs e)
        {
            string[] s = textBox5.Text.Split(new char[] { '@' });
            if (s.Length < 3)
                return;

            int nBegin = int.Parse(s[1]);
            int nEnd = int.Parse(s[2]);
            string strFormat = s[0];
            for (int i = nBegin; i < nEnd; ++i)
            {
                LoadVerifyPic(string.Format(strFormat, i));

                PopupForm form = new PopupForm();
                form.ShowDialog();
                AutoAddVerify(form.GetTextBoxString());
            }
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
            else
            {
                if (textBox2.Text.Length <= 1)
                    e.Handled = true;
            }
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
            else
            {
                if (textBox3.Text.Length <= 1)
                    e.Handled = true;
            }
        }
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
            else
            {
                if (textBox4.Text.Length <= 1)
                    e.Handled = true;
            }
        }
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                AutoAddVerify();
        }
        #endregion

        /// <summary>
        /// 更新配置文件
        /// </summary>
        public void UpdateConfig()
        {
            if (!m_bIsAllowUpdateConfig)
            {
                return;
            }
            CodeInfo defaultInfo = new CodeInfo();
            defaultInfo = FKDeepObjCopier.Clone(GlobalCH.m_ci);

            defaultInfo.PixelMin = Convert.ToInt32(textBox3.Text);
            defaultInfo.PixelMax = Convert.ToInt32(textBox4.Text);
            defaultInfo.NoiseLv = noiseLvTrackBar.Value;
            defaultInfo.IsAutoSelectRect = true;
            defaultInfo.CodeRectangles.Clear();
            defaultInfo.BinaryValues = Enumerable.Repeat((byte)Convert.ToInt32(textBox2.Text), 1).ToArray();

            GlobalCH.LoadCodeInfo(defaultInfo);
        }
        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="strFile"></param>
        public void SaveConfigToFile(string strFile)
        {
            CodeInfo.SaveToFile(GlobalCH.m_ci, strFile);
        }
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="strFile"></param>
        public void LoadConfigFromFile(string strFile)
        {
            GlobalCH.m_ci.ClearCode();
            GlobalCH.LoadCodeInfo(strFile);

            // 临时关闭自动更新
            m_bIsAllowUpdateConfig = false;

            // 更新到面板
            trackBar1.Value = GlobalCH.m_ci.BinaryValues[0];
            //trackBar1.Invalidate();
            textBox2.Text = trackBar1.Value.ToString();
            textBox3.Text = GlobalCH.m_ci.PixelMin.ToString();
            textBox4.Text = GlobalCH.m_ci.PixelMax.ToString();
            noiseLvTrackBar.Value = GlobalCH.m_ci.NoiseLv;
            noiseLvTrackBar.Invalidate();

            // 手动更新数据
            m_bIsAllowUpdateConfig = true;
            UpdateConfig();
        }
        // 加载新图片
        // bUseReverseColor 是否使用反色
        public void LoadVerifyPic(string strPath)
        {
            SrcImage = Image.FromFile(strPath);
            int nBlackCount = 0;
            int nWhiteCount = 0;
            // 二值化
            BinaryImage = GlobalCH.GetBinaryImage(SrcImage, GlobalCH.m_ci.BinaryValues[0], out nBlackCount, out nWhiteCount);
            Image tmpBinaryImage = BinaryImage.Clone() as Image;
            // 去噪
            NoNoiseImage = GlobalCH.ClearNoise(tmpBinaryImage, GlobalCH.m_ci.NoiseLv);
            Image tmpNoNoiseImage = NoNoiseImage.Clone() as Image;
            // 反色
            if (nBlackCount > nWhiteCount)
            {
                ReverseImage = GlobalCH.ReverseColors(tmpNoNoiseImage);
            }
            else { 
                ReverseImage = tmpNoNoiseImage;
            }
            Image tmpReverseImage = ReverseImage.Clone() as Image;

            // 计算方格位置
            CharRects.Clear();
            CharImages.Clear();
            CharRects = GlobalCH.GetCharRect(tmpReverseImage, GlobalCH.m_ci.PixelMin, GlobalCH.m_ci.PixelMax);

            // 实际识别的处理时间
            var watch = System.Diagnostics.Stopwatch.StartNew();
            VerifyValue = GlobalCH.GetCodeString(SrcImage);
            watch.Stop();
            CostTime = (int)watch.ElapsedMilliseconds;

            groupBox2.Invalidate();
        }
        /// <summary>
        /// 手动训练一张参考图片
        /// </summary>
        public void AutoAddVerify()
        {
            string str = textBox6.Text;
            int nMax = str.Length > CharImages.Count ? CharImages.Count : str.Length;
            for(int i = 0; i < nMax; ++i)
            {
                GlobalCH.m_ci.AddCode(str.ElementAt(i), CharImages[i]);
            }
            textBox6.Text = "";
        }
        public void AutoAddVerify(string str)
        {
            int nMax = str.Length > CharImages.Count ? CharImages.Count : str.Length;
            for (int i = 0; i < nMax; ++i)
            {
                GlobalCH.m_ci.AddCode(str.ElementAt(i), CharImages[i]);
            }
            textBox6.Text = "";
        }

        // 更新降噪强度
        private void noiseLvTrackBar_Scroll(object sender, EventArgs e)
        {
            UpdateConfig();
        }
    }
}
