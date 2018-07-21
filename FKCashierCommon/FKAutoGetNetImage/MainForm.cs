//------------------------------------------------------------
// Modified: Frankie.W 20170712
//------------------------------------------------------------
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;
//------------------------------------------------------------
namespace FKAutoGetNetImage
{
    public partial class Main_Form : Form
    {
        #region 变量

        private int      m_nCurImageIndex = 0;
        private string   m_strUrl = "";
        private string   m_strLocalDirName = "";
        private int      m_nIntervalTime = 0;

        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public Main_Form()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Form_Load(object sender, EventArgs e)
        {
            this.Text = "FK批量网络图片下载器 v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// 生成一个新图片名
        /// </summary>
        /// <returns></returns>
        public string GenNewImageName()
        {
            return m_strLocalDirName + "_" + (m_nCurImageIndex++) + ".bmp";
        }
        /// <summary>
        /// 开启下载按钮按下处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_StartDownload_Click(object sender, EventArgs e)
        {
            m_nCurImageIndex        = 0;
            m_strUrl                = textBox_Url.Text;
            m_nIntervalTime         = int.TryParse(textBox_IntervalTime.Text, out m_nCurImageIndex) ?
                    int.Parse(textBox_IntervalTime.Text) * 1000 : 1000;
            m_strLocalDirName       = textBox_DirName.Text;
            int nStartIndex         = int.TryParse(textBox_StartIndex.Text, out m_nCurImageIndex) ?
                    int.Parse(textBox_StartIndex.Text) : 1;
            m_nCurImageIndex = nStartIndex;

            try
            {
                System.Timers.Timer timer = new System.Timers.Timer(m_nIntervalTime);
                timer.Elapsed += (sender2, e2) => OnTimer(sender2, e2, this);
                timer.AutoReset = true;
                timer.Enabled = true;

                // 立刻手动调用
                OnTimer(null, null, this);
            }
            catch { }
        }
        /// <summary>
        /// 定时下载图片
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        /// <param name="form"></param>
        private static void OnTimer(Object source, ElapsedEventArgs e, Main_Form form)
        {
            Bitmap img = null;
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            try
            {
                Uri imgUrl = new Uri(form.m_strUrl);
                req = (HttpWebRequest)(WebRequest.Create(imgUrl));
                req.Timeout = (int)(form.m_nIntervalTime * 0.8);
                string requestType = imgUrl.Scheme;
                if (string.Equals(requestType, "https"))
                {
                    req.Method = "POST";
                }
                else
                {
                    req.Method = "GET";
                }
                res = (HttpWebResponse)(req.GetResponse());
                img = new Bitmap(res.GetResponseStream());

                string strImageDir = Environment.CurrentDirectory + "\\" + form.m_strLocalDirName + "\\";
                if (!Directory.Exists(strImageDir)){
                    Directory.CreateDirectory(strImageDir);
                }
                string strTotalName = strImageDir + form.GenNewImageName();
                img.Save(strTotalName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                res.Close();
            }
        }
    }
}
