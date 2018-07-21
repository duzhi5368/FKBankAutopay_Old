/* 
 * WRANING: These codes below is far away from bugs with the god and his animal protecting
 *                  _oo0oo_                   ┏┓　　　┏┓
 *                 o8888888o                ┏┛┻━━━┛┻┓
 *                 88" . "88                ┃　　　　　　　┃ 　
 *                 (| -_- |)                ┃　　　━　　　┃
 *                 0\  =  /0                ┃　┳┛　┗┳　┃
 *               ___/`---'\___              ┃　　　　　　　┃
 *             .' \\|     |# '.             ┃　　　┻　　　┃
 *            / \\|||  :  |||# \            ┃　　　　　　　┃
 *           / _||||| -:- |||||- \          ┗━┓　　　┏━┛
 *          |   | \\\  -  #/ |   |          　　┃　　　┃神兽保佑
 *          | \_|  ''\---/''  |_/ |         　　┃　　　┃永无BUG
 *          \  .-\__  '-'  ___/-. /         　　┃　　　┗━━━┓
 *        ___'. .'  /--.--\  `. .'___       　　┃　　　　　　　┣┓
 *     ."" '<  `.___\_<|>_/___.' >' "".     　　┃　　　　　　　┏┛
 *    | | :  `- \`.;`\ _ /`;.`/ - ` : | |   　　┗┓┓┏━┳┓┏┛
 *    \  \ `_.   \_ __\ /__ _/   .-` /  /   　　　┃┫┫　┃┫┫
 *=====`-.____`.___ \_____/___.-`___.-'=====　　　┗┻┛　┗┻┛ 
 *                  `=---='　　　
 *          佛祖保佑       永无BUG
 */
// =============================================================================== 
// Author              :    Frankie.W
// Create Time         :    2017/7/17 13:38:21
// Update Time         :    2017/7/17 13:38:21
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierBank
{
    public partial class MainForm : Form
    {
        private const int INVALID_NODE_ID = -1;
        private const int INVALID_TASK_ID = -1;
        private const int MAX_ALLOW_HEARTBEAT_FAILED_TIME = 3;

        private System.Windows.Forms.Timer m_InitTimer = null;      // 初始化时的单次定时器
        private System.Timers.Timer m_nHeartBeatTimer = null;       // 心跳定时器

        private int m_nLostHeartbeatCount = 0;                      // 已经丢失心跳的次数

        private FKHttp.FKHttpServer m_WebServer = null;             // WEB服务器对象
        private Thread m_LogicThread = null;                        // 逻辑线程对象
        private bool m_bIsNeedCloseLogicThread = false;             // 是否需要关闭逻辑线程

        private int m_nNodeId = INVALID_NODE_ID;                    // 本节点NodeID
        private int m_nCurTaskId = INVALID_TASK_ID;                 // 本节点当前正在处理的任务ID
        private int m_nLastTaskId = INVALID_TASK_ID;                // 本节点上一个成功处理的任务ID

        public MainForm()
        {
            InitializeComponent();

            // 禁止系统休眠
            FKBaseUtils.FKSystemEnviSettingHelper.PreventSystemSleep(true);
            // 创建日志对象
            FKLog.FKLogImp.GetInstance.SetCommandEditor(this.textBox_Cmd);
            FKLog.FKLogImp.GetInstance.SetRichTextBox(this.Log_richTextBox);
        }

        #region ==== 控件消息 ====

        /// <summary>
        /// Form 启动初始化函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, System.EventArgs e)
        {
            LOGGER.INFO("System init done, begin main form load.");

            // 设置标题
            this.Text = GetAppDescption();
            // 设置本线程名
            Thread.CurrentThread.Name = "FKMainThread";
            // 初次调用单例，创建RSA公私钥对
            RSAKeyContainer.GetInstance.ToString();

            m_InitTimer = new System.Windows.Forms.Timer();
            m_InitTimer.Interval = 1000;
            m_InitTimer.Tick += new EventHandler(OnTimer_Init);
            m_InitTimer.Start();
        }

        /// <summary>
        /// 手动关闭Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("确定退出程序吗？", "退出程序", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                LOGGER.WARN($"FKClose app by man-made.");
            }
        }

        /// <summary>
        /// 按下执行消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_DoCmd_Click(object sender, EventArgs e)
        {
            string strCmd = textBox_Cmd.Text;
            string[] strCmds = strCmd.Split(' ');
            if (strCmds.Length < 1)
            {
                button_Help_Click(null, null);
                return;
            }
            bool bSuccessed = false;
            string strRet = string.Empty;
            if (strCmds.Length == 1)
            {
                bSuccessed = DoCmd(strCmds[0], null, ref strRet);
            }
            else
            {
                bSuccessed = DoCmd(strCmds[0], strCmds.Skip(1).ToArray(), ref strRet);
            }
            if (bSuccessed)
            {
                LOGGER.INFO($"DoCmd {strCmds[0]} successed \n {strRet}");
            }
            else
            {
                LOGGER.WARN($"DoCmd {strCmds[0]} failed. \n {strRet}");
            }
        }

        /// <summary>
        /// 帮助按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Help_Click(object sender, EventArgs e)
        {
            textBox_Cmd.Text = "help";
            button_DoCmd_Click(null, null);
        }

        /// <summary>
        /// 打开工作目录按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_OpenDir_Click(object sender, EventArgs e)
        {
            string strCurWorkdir = FKBaseUtils.FKSystemFileSystemHelper.GetWorkdir();
            Process.Start(strCurWorkdir);
        }

        /// <summary>
        /// 命令行输入回车
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_Cmd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                button_DoCmd.PerformClick();
        }

        /// <summary>
        /// 设置Url按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SetUrlConfig_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 设置CardNo按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SetCardNoConfig_Click(object sender, EventArgs e)
        {

        }

        #endregion ==== 控件消息 ====

        #region ==== 功能函数 ====

        /// <summary>
        /// 获取App信息
        /// </summary>
        /// <returns></returns>
        private string GetAppDescption()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var descriptionAttribute = assembly
                 .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                 .OfType<AssemblyDescriptionAttribute>()
                 .FirstOrDefault();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = "unknown version";
            if (fvi != null)
                version = "v" + fvi.FileVersion;
            string description = "FKCashierNode";
            if (descriptionAttribute != null)
                description = descriptionAttribute.Description;
            string strInfo = description + version;
            return strInfo;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTimer_Init(object sender, EventArgs args)
        {
            // 计时器进行一次足够了
            if (m_InitTimer != null)
            {
                m_InitTimer.Stop();
            }
            // 创建Web服务器对象
            CreateWebServer();
            // 启动逻辑线程
            StartLogicThread();
            // 启动Web服务器监听
            StartWebServer();
            // 向服务器进行注册
            if (RegsiterToServer())
            {
                // 开始心跳
                StartHeartbeat();
            }

            LOGGER.INFO("Form init done.");
        }
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void FKClose()
        {
            // 关闭心跳
            StopHeartbeat();
            // 申请从服务器注销
            UnregsiterFromServer();
            // 关闭Web服务器监听
            StopWebServer();
            // 关闭逻辑线程
            StopLogicThread();
            // 强杀IEDriver
            ForceShutdownIE();

            LOGGER.INFO("Form close done.");
        }

        /// <summary>
        /// 创建Web服务器
        /// </summary>
        private void CreateWebServer()
        {
            try
            {
                if (FKConfig.IsUseHttpServer)
                {
                    m_WebServer = new FKHttp.FKHttpServer(this, RequestReciver.Func_OnRequest,
                        FKConfig.IsUseHttpsWebServer, FKConfig.WebServerListenUrl);
                }
            }
            catch(Exception e)
            {
                LOGGER.ERROR($"Create web server failed. Error = {e.ToString()}", 0);
            }

            if(m_WebServer == null || !m_WebServer.IsRunning())
            {
                LOGGER.WARN("Web server is not running.");
            }
            else
            {
                if (m_WebServer.IsSupportingSSL())
                {
                    LOGGER.INFO("Create web server successed. Support HTTPS");
                }
                else
                {
                    LOGGER.INFO("Create web server successed. Only support HTTP");
                }
            }
        }

        /// <summary>
        /// 启动Web服务器
        /// </summary>
        private void StartWebServer()
        {
            try
            {
                if (m_WebServer != null)
                {
                    m_WebServer.Start();
                }
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Start web server failed. Error = {e.ToString()}", 0);
            }
            LOGGER.INFO("Start web server successed.");
        }

        /// <summary>
        /// 关闭Web服务器
        /// </summary>
        private void StopWebServer()
        {
            try
            {
                if (m_WebServer != null)
                {
                    m_WebServer.Stop();
                }
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Stop web server failed. Error = {e.ToString()}", 0);
            }
            LOGGER.INFO("Stop web server successed.");
        }

        /// <summary>
        /// 创建逻辑线程
        /// </summary>
        private void StartLogicThread()
        {
            m_LogicThread = new Thread(LogicThreadMain);
            m_LogicThread.SetApartmentState(ApartmentState.STA);
            m_LogicThread.IsBackground = true;
            m_LogicThread.Name = "FKLogicThread";
            try
            {
                m_LogicThread.Start();
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Logic thread error occurred. Error = {e.ToString()}", 0);
            }
        }

        /// <summary>
        /// 关闭逻辑线程
        /// </summary>
        private void StopLogicThread()
        {
            m_bIsNeedCloseLogicThread = true;
            try
            {
                m_LogicThread.Abort();
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Stop logic thread error occurred. Error = {e.ToString()}", 0);
            }
        }

        /// <summary>
        /// 向服务器注册
        /// </summary>
        private bool RegsiterToServer()
        {
            // 先尝试检查远程服务器是否可以ping通
            bool bIsRomateCanPing = IsCanPingPass();

            if (!bIsRomateCanPing)
            {
                LOGGER.ERROR("Can't pass server ip ping test, will not use registe node and heart-beat");
                return false;
            }

            // 开始注册
            SRegisteNode node = new SRegisteNode();
            node.bankCardNo = FKConfig.BankCardNo;
            node.ip = string.IsNullOrEmpty(FKConfig.LocalIP) ? FKBaseUtils.FKSystemEnviSettingHelper.GetLocalIPAddressByUri(FKConfig.ServerRegisteUrl) : FKConfig.LocalIP;
            node.publicKey = RSAKeyContainer.GetInstance.GetCSharpPublicKey();
            LOGGER.INFO($"Begin to register to server. Bank card No = {node.bankCardNo}, Ip = {node.ip}");
            HttpSendResult result = RequestSender.SendRequestToServer(node);
            
            if(result.IsSendSuccessed() && result.ErrorStatus == 1 && (!string.IsNullOrEmpty(result.publicKey)))
            {
                SetCurCashierNodeID(result.NodeID);
                RSAKeyContainer.GetInstance.SetJavaPublicKey(result.publicKey);
                LOGGER.INFO($"Register to server successed. Node id = {result.NodeID}");
                return true;
            }
            else
            {
                SetCurCashierNodeID(INVALID_NODE_ID);
                LOGGER.ERROR($"Register to server failed. but still use registe node and heart-beat. Node id = {GetCurCashierNodeID()}");
                return true;
            }
        }

        /// <summary>
        /// 从服务器进行注销
        /// </summary>
        /// <returns></returns>
        private bool UnregsiterFromServer()
        {
            try
            {
                SUnregisteNode node = new SUnregisteNode();
                node.id = GetCurCashierNodeID();
                HttpSendResult result = RequestSender.SendRequestToServer(node);

                if (result.IsSendSuccessed() && result.ErrorStatus == 1)
                {
                    LOGGER.INFO($"Node unregiste successed. NodeID = {node.id}");
                    return true;
                }
                else
                {
                    LOGGER.WARN($"Node unregiste failed. NodeID = {node.id}");
                    return false;
                }
            }
            catch(Exception e)
            {
                LOGGER.ERROR($"Node unregiste error occured. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 启动心跳
        /// </summary>
        /// <returns></returns>
        private bool StartHeartbeat()
        {
            try
            {
                m_nHeartBeatTimer = new System.Timers.Timer(FKConfig.HeartbeatIdleTime);
                m_nHeartBeatTimer.Elapsed += (sender, e) => OnHeartbeatTimerEvent(sender, e, FKConfig.ServerHeartbeatUrl, this);
                m_nHeartBeatTimer.AutoReset = true;
                m_nHeartBeatTimer.Enabled = true;
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Create heart-beat timer failed. Error = {e.ToString()}");
                return false;
            }
            LOGGER.INFO($"Start heart-beat successed. TickTime = {FKConfig.HeartbeatIdleTime} ms.");
            return true;
        }

        /// <summary>
        /// 关闭心跳
        /// </summary>
        /// <returns></returns>
        private bool StopHeartbeat()
        {
            try
            {
                if(m_nHeartBeatTimer != null)
                {
                    m_nHeartBeatTimer.Stop();
                    m_nHeartBeatTimer.Close();
                    m_nHeartBeatTimer.Dispose();
                    LOGGER.INFO($"FKClose heart-beat successed.");
                }
                else
                {
                    LOGGER.INFO($"We did't use heart-beat.");
                }
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"FKClose heart-beat timer failed. Error = {e.ToString()}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查远程服务器是否可以ping通
        /// </summary>
        /// <returns></returns>
        private bool IsCanPingPass()
        {
            bool bRet = true;
            if (FKConfig.IsUsePing)
            {
                try
                {
                    Uri uri = new Uri(FKConfig.ServerRegisteUrl);
                    if (uri.Host == "")
                    {
                        LOGGER.WARN("Can't get server ip, stop ping service.");
                        bRet = false;
                    }
                    if (!FKBaseUtils.FKCommonFunc.IsRomateExistByPingTest(uri.Host))
                    {
                        LOGGER.WARN("Can't pass server ip ping test.");
                        bRet = false;
                    }
                }
                catch (Exception e)
                {
                    LOGGER.WARN($"Ping server test failed. Error = {e.ToString()}");
                    bRet = false;
                }
            }
            return bRet;
        }

        /// <summary>
        /// 心跳函数
        /// </summary>
        private static void OnHeartbeatTimerEvent(Object source, ElapsedEventArgs e, string strHeartbeatUrl, MainForm form)
        {
            Thread.CurrentThread.Name = "FKHeartbeatThread";
            if(!FKConfig.IsUseHttpServer)
            {
                return;
            }
            try
            {
                if (form.GetCurCashierNodeID() == INVALID_NODE_ID)
                {
                    // NodeID无效，则重新注册
                    form.ClearLostHeartbeatCount();

                    SRegisteNode node = new SRegisteNode();
                    node.bankCardNo = FKConfig.BankCardNo;
                    node.ip = string.IsNullOrEmpty(FKConfig.LocalIP) ? FKBaseUtils.FKSystemEnviSettingHelper.GetLocalIPAddressByUri(FKConfig.ServerRegisteUrl) : FKConfig.LocalIP;
                    node.publicKey = RSAKeyContainer.GetInstance.GetCSharpPublicKey();
                    LOGGER.INFO($"Begin to register to server. Bank card No = {node.bankCardNo}, Ip = {node.ip}, C# public key = {node.publicKey}");
                    HttpSendResult result = RequestSender.SendRequestToServer(node);

                    if (result.IsSendSuccessed() && result.ErrorStatus == 1 && (!string.IsNullOrEmpty(result.publicKey)))
                    {
                        form.SetCurCashierNodeID(result.NodeID);
                        RSAKeyContainer.GetInstance.SetJavaPublicKey(result.publicKey);
                        LOGGER.INFO($"Register to server successed. Node id = {result.NodeID}, Java public key = {RSAKeyContainer.GetInstance.GetJavaPublicKey()}");
                        return;
                    }

                    LOGGER.ERROR($"Register to server failed. Node id = {result.NodeID}, Java public key = {RSAKeyContainer.GetInstance.GetJavaPublicKey()}");
                    return;
                }
                else
                {
                    // 有效NodeID，表示注册成功，则发送心跳
                    SHeartbeat node = new SHeartbeat();
                    node.id = form.GetCurCashierNodeID();
                    node.taskID = form.GetCurTaskID();
                    HttpSendResult result = RequestSender.SendRequestToServer(node);

                    if (result.IsSendSuccessed() && result.ErrorStatus == 1 && result.NodeID > 0)
                    {
                        LOGGER.INFO($"Heart-beat successed. NodeID = {node.id}, TaskID = {node.taskID}");
                        return;
                    }
                    else
                    {
                        // 发送心跳失败
                        form.AddLostHeartbeatCount();   // 追加一次失败计数
                        if (form.GetLostHeartbeatCount() >= MAX_ALLOW_HEARTBEAT_FAILED_TIME)
                        {
                            // 心跳失联太多次
                            form.SetCurCashierNodeID(INVALID_NODE_ID);
                            RSAKeyContainer.GetInstance.SetJavaPublicKey(string.Empty);
                            LOGGER.WARN($"Heart-beat disconnect for a long time... Ready to register node again.");
                        }
                        else
                        {
                            // 心跳的确失联了，但还能忍
                            LOGGER.INFO($"Heart-beat connect failed {form.GetLostHeartbeatCount()} times. NodeID = {node.id}, TaskID = {node.taskID}");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                LOGGER.ERROR($"Heart-beat error occured. Error = {ex.ToString()}");
            }
        }

        #endregion ==== 功能函数 ====

        #region ==== 接口函数 ====

        /// <summary>
        /// 设置当前Node节点ID
        /// </summary>
        private void SetCurCashierNodeID(int nNodeID)
        {
            if(m_nNodeId != nNodeID)
            {
                LOGGER.WARN($"Node id changed, {m_nNodeId} -> {nNodeID}");
            }
            m_nNodeId = nNodeID;
        }
        /// <summary>
        /// 获取当前Node节点ID
        /// </summary>
        /// <returns></returns>
        internal int GetCurCashierNodeID()
        {
            return m_nNodeId;
        }
        /// <summary>
        /// 设置当前正在处理的TaskID
        /// </summary>
        /// <returns></returns>
        private void SetCurTaskID(int nTaskID)
        {
            m_nCurTaskId = nTaskID;
        }
        /// <summary>
        /// 获取当前任务ID
        /// </summary>
        /// <returns></returns>
        internal int GetCurTaskID()
        {
            return m_nCurTaskId;
        }
        /// <summary>
        /// 设置本节点上次成功处理的TaskID
        /// </summary>
        /// <param name="nTaskID"></param>
        private void SetLastTaskID(int nTaskID)
        {
            m_nLastTaskId = nTaskID;
        }
        /// <summary>
        /// 将丢失心跳次数清零
        /// </summary>
        internal void ClearLostHeartbeatCount()
        {
            m_nLostHeartbeatCount = 0;
        }
        /// <summary>
        /// 增加一次失败心跳次数
        /// </summary>
        internal void AddLostHeartbeatCount()
        {
            m_nLostHeartbeatCount++;
        }
        /// <summary>
        /// 获取当前心跳失败次数
        /// </summary>
        /// <returns></returns>
        internal int GetLostHeartbeatCount()
        {
            return m_nLostHeartbeatCount;
        }
        /// <summary>
        /// 安全释放App
        /// </summary>
        internal void SafeShutdown()
        {
            LOGGER.INFO($"Begin to safe shutdown. NodeID = {GetCurCashierNodeID()}");

            try
            {
                // 计划下一部清除行为
                FKClose();
                // 清空所有的Key
                RSAKeyContainer.GetInstance.Clear();
            }
            catch(Exception e)
            {
                LOGGER.WARN($"Safe shutdown failed, we will force close. Error = {e.ToString()}");
            }
        }
        /// <summary>
        /// 强制关闭IE和WebDriver进程
        /// </summary>
        private void ForceShutdownIE()
        {
            FKBaseUtils.FKCommonFunc.RunConsoleCommand("taskkill.exe", " /f /IM IEDriverServer.exe");
            FKBaseUtils.FKCommonFunc.RunConsoleCommand("taskkill.exe", " /f /IM iexplore.exe");
        }
        /// <summary>
        /// 添加一个出款任务到等待队列
        /// </summary>
        /// <param name="info"></param>
        internal void AddOutTaskToWaitQueue(SOutTaskInfo info)
        {
            LOGGER.INFO($"Add a new out task to queue. TaskID = {info.taskID}");
            m_OutTasksList.Enqueue(info);
        }
        /// <summary>
        /// 添加一个新流水查询任务到等待队列
        /// </summary>
        /// <param name="info"></param>
        internal void AddBillTaskToWaitQueue(SBillTaskInfo info)
        {
            LOGGER.INFO($"Add a new bill task to queue. TaskID = {info.taskID}, Time = {info.startTime}-{info.endTime}");
            m_BillTasksList.Enqueue(info);
        }

        #endregion ==== 接口函数 ====
    }
}
