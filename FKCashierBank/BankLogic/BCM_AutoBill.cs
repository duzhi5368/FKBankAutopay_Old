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
// Create Time         :    2017/7/19 10:38:12
// Update Time         :    2017/7/19 10:38:12
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using FKWebAutomatic;
using FKWndAutomatic;
using System.Threading;
using LOGGER = FKLog.FKLogger;
using NUnit.Framework;
using Tesseract;
using System.IO;
using FKVerifyLib;
using System.Drawing;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Globalization;
using FKXlsLib;
// ===============================================================================
namespace FKCashierBank
{

    struct BCM_BankInfo
    {
        public string cardNo { get; set; }
        public string userName { get; set; }
        public string safeValue { get; set; }
        public string PSessionId { get; set; }
        public string uuid { get; set; }

        public override string ToString()
        {
            string strRet = "---------------------------\n";
            try
            {
                strRet += ("卡号：" + FKBaseUtils.FKStringHelper.MaskString(cardNo, false) + "\n");
                strRet += ("用户：" + FKBaseUtils.FKStringHelper.MaskString(userName, false) + "\n");

                strRet += ("卡号：" + cardNo + "\n");
                strRet += ("用户：" + userName + "\n");

                strRet += ("safeValue：" + safeValue + "\n");
                strRet += ("PSessionId：" + PSessionId + "\n");
                strRet += ("uuid：" + uuid + "\n");
            }
            catch { }
            finally
            {
                strRet += "---------------------------";
            }
            return strRet;
        }
    }
    /// <summary>
    /// 交通银行自动流水查询核心流程
    /// </summary>
    internal class BCM_AutoBill : BankAutoBillBase
    {
        private static FreqLimit s_FreqLimit = new FreqLimit();
        public override bool Init()
        {
            s_FreqLimit.Check();
            try { 
                // 如果没有登录过，清除前面残留的ie
                if (s_bIsLogining == false)
                {
                    FKWebDriver.GetInstance.Close();
                    ForceShutdownIE();
                    Thread.Sleep(1000);
                }
            }
            catch(Exception e)
            {
                LOGGER.ERROR($"Init failed ... Error = {e.ToString()}");
                FKWebDriver.GetInstance.Close();
                ForceShutdownIE();
                return false;
            }

            return true;
        }
        public override bool AutoBill(int nTaskID, SBillTaskInfo info, ref SBillTaskResult result)
        {
            // 判断当前是否是同一个账号
            if (!IsSameAccount(info))
            {
                FKWebDriver.GetInstance.Close();
                ForceShutdownIE();
                s_bIsLogining = false;
            }
            // 先尝试进行登录
            string bankCode = "BCM";
            string logPrefix = $"银行代码[{bankCode}]获取明细，TaskID[{info.taskID}]，";
            LOGGER.INFO($"{logPrefix}正在尝试登录 ...");
            s_bIsLogining = TryLogin(info, ref result);
            LOGGER.INFO($"{logPrefix}登录返回结果[{s_bIsLogining}]");
            if (!s_bIsLogining)
            {
                LOGGER.INFO($"{logPrefix}登录失败，失败原因[{result.msg}]");
                FKWebDriver.GetInstance.Close();
                ForceShutdownIE();
                return false;
            }

            LOGGER.INFO($"{logPrefix}登录成功，开始获取明细");
            // 查询
            bool ret = QueryAutoBill(info, ref result);

            LOGGER.INFO($"{logPrefix}获取明细返回结果[{ret}]");

            if (ret == false)
            {
                LOGGER.INFO($"{logPrefix}获取明细失败，失败原因[{result.msg}]");
            }
            return ret;
        }
        public override void Clear()
        {
            //throw new NotImplementedException();
        }

        // 刷新事件
        public static void Update()
        {
            if (!s_bIsLogining) // 未登录，不做任何刷新
                return;

            try
            {
                FKWebAutomatic.FKWebDriver.GetInstance.SwitchToParentFrame();
                FKWebAutomatic.FKWebDriver.GetInstance.SwitchToFrameByID("frameMain");
                Thread.Sleep(500);
                /*
                string[] xpaths =
                {
                    "/html/body",
                    "/html/body/div[2]",
                    "/html/body/div[2]/div[1]",
                    "/html/body/div[2]/div[1]/div[2]/",
                    "/html/body/div[2]/div[1]/div[2]/ul",
                    "/html/body/div[2]/div[1]/div[2]/ul/li[1]",
                    "/html/body/div[2]/div[1]/div[2]/ul/li[1]/a",
                };
                foreach (var xpath in xpaths)
                {
                    if (!FKWebAutomatic.FKWebDriver.GetInstance.ClickByXPath(xpath))
                    {
                        LOGGER.ERROR($"点击{xpath}失败");
                    }
                }*/
                int retryCount = 3;
                for (int i = 0; i < retryCount; i++)
                {
                    if (FKWebAutomatic.FKWebDriver.GetInstance.ClickByXPath("/html/body/div[2]/div[1]/div[2]/ul/li[1]/a"))
                    {
                        var element = FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@id=\"tranArea\"]", 30000);
                        if (element != null)
                        {
                            LOGGER.INFO("BCM刷新主页成功");
                            return;
                        }
                        else
                        {
                            LOGGER.ERROR($"刷新等待主页元素超时，等待重试");
                        }
                    }
                    else
                    {
                        // 出现任何异常，立即关闭浏览器设置为未登录，等待下次登录
                        LOGGER.INFO($"BCM刷新主页，失败[ClickByID返回false],等待重试");
                    }
                    Thread.Sleep(5000);
                }
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Update BCM bank error occured, IE driver will shutdown. Error = {e.ToString()}");
                // 出现任何异常，立即关闭浏览器设置为未登录，等待下次登录
                FKWebDriver.GetInstance.Close();
                ForceShutdownIE();
                s_bIsLogining = false;
            }
            LOGGER.INFO($"BCM刷新主页失败，清理ie等待下次重新登录");
            FKWebDriver.GetInstance.Close();
            ForceShutdownIE();
            s_bIsLogining = false;
        }

        // 刷新推荐的间隔时间( 单位：秒 )
        public static int GetUpdateIdleSecond()
        {
            return 60;
        }

        
        /// <summary>
        /// 判断当前已打开的IE中账号 与 当前任务账号 是否一致
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool IsSameAccount(SBillTaskInfo info)
        {
            // 未登录，直接登录即可
            if (!s_bIsLogining)
            {
                return true;
            }
           
            try
            {
                var currUser = getCardInfo();
                if (currUser.userName == info.username)
                {
                    LOGGER.INFO($"当前在线用户[{FKBaseUtils.FKStringHelper.MaskString(info.username)}]和登录用户一致");
                    return true;
                }
            }
            catch
            {
                LOGGER.INFO($"新登录用户[{FKBaseUtils.FKStringHelper.MaskString(info.username)}], 当前在线用户可能不是BCM用户");
                return false;
            }
            LOGGER.INFO($"新登录用户[{FKBaseUtils.FKStringHelper.MaskString(info.username)}], 当前在线用户可能不是BCM用户");
            return false;
        }
        private bool QueryAutoBill(SBillTaskInfo info, ref SBillTaskResult result)
        {

            //return true;
            if (!s_bIsLogining)
            {
                // 未登录
                return false;
            }
            result.taskID = info.taskID;
            try
            {
                
                // 实际测试过程中发现这个链接是随着账号改变，需要在首页中获取
                string postUrl = $"https://pbank.95559.com.cn/personbank/account/acTranRecordQuery.stream";
                if (string.IsNullOrEmpty(postUrl))
                {
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                    result.msg = "BCM Post url获取失败";
                    return false;
                }
                // 构造post content
                var postContentString = GetAutoBillPostContent(info);
                if (string.IsNullOrEmpty(postContentString))
                {
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                    result.msg = "BCM Post content构造失败";
                    return false;
                }
                // 构造post header
                var postHeader = GetAutoBillPostHeader();

                // POST获取文件
                string tmpFile = System.IO.Path.GetTempFileName();
                FKHttp.FKHttpClient.POST(postUrl, postContentString, postHeader, 300000, tmpFile);

                parseXlsContent(tmpFile, ref result, info);
               
            }
            catch (Exception e)
            {
                result.msg = $"QueryAutoBill抛出异常[{e.Message}]";
                return false;
            }
            return true;
        }
        private bool TryLogin(SBillTaskInfo info, ref SBillTaskResult result)
        {
            if (s_bIsLogining == false)
            {
                try
                {
                    //FKWebDriver webDriver = FKWebDriver.GetInstance;
                    string loginUrl = FKConfig.BCMLoginUrl;
                    // 此部分代码根据现有交行的页面设计
                    // 如果交行个人银行网页改版，此处可能需要根据新页面做相应调整

                    FKWebAutomatic.FKWebDriver.GetInstance.OpenUrl(loginUrl);

                    // 等待出现登录界面
                    var element = FKWebAutomatic.FKWebDriver.GetInstance.WaitUntilVisibleByXPath("/html/body/div[2]/div/div/div[2]/input", 10000);
                    if (element == null)
                    {
                        result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_WaitElementTimeout;
                        result.msg = $"等待登录页面超时";
                        return false;
                    }

                    // 填充登录账号
                    FKWebAutomatic.FKWebDriver.GetInstance.SetTextByID("alias", info.username);

                    // 填充密码
                    LoginPassword(info.password);
                    // FKWebAutomatic.FKWebDriver.GetInstance.SetTextByID("password", info.password);

                    if (FKWebAutomatic.FKWebDriver.GetInstance.IsElementVisiableByID("input_captcha"))
                    {

                        int retryCount = 20;
                        for (int i = 0; i < retryCount; i++)
                        {
                            string ocr = GetVerificationCodeString();
                            
                            // 如果识别个数都不对 没必要输入了 重刷验证码再识别
                            if (ocr.Length == 5)
                            {
                                FKWebAutomatic.FKWebDriver.GetInstance.SetTextByID("input_captcha", ocr);
                                //移走焦点
                                FKWebAutomatic.FKWebDriver.GetInstance.ClickByID("alias");
                                // 填充完成后等待识别验证码打钩
                                Thread.Sleep(5000);
                                // 第一次此元素为隐藏，等待出现
                                FKWebAutomatic.FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@id=\"captchaFlg\"]", 10000);
                                // 识别结果获取
                                string successString = FKWebAutomatic.FKWebDriver.GetInstance.GetAttributeByXPath("//*[@id=\"captchaFlg\"]", "class");
                                if (successString == "right-cpt")
                                {
                                    break;
                                }
                                Console.WriteLine($"successString = {successString}");
                            }
                            // 失败 重新刷新验证码
                            FKWebAutomatic.FKWebDriver.GetInstance.ClickByXPath("//*[@class=\"captchas-img-bg\"]");
                            Thread.Sleep(1000);
                        }
                    }

                    // 点击登录
                    FKWebAutomatic.FKWebDriver.GetInstance.ClickByID("loginBtn");
                    element = FKWebAutomatic.FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@id=\"prepareForm\"]", 6000);
                    if (element != null)
                    {
                        var url = FKWebAutomatic.FKWebDriver.GetInstance.GetAttributeByXPath("//*[@id=\"prepareForm\"]", "action");
                        LOGGER.INFO($"当前url:{url}");
                        if (url.Contains("syVerifyCustomerNewControl.do") == true)
                        {
                            LOGGER.INFO($"U盾未绑定:{url}");
                            result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_WaitElementTimeout;
                            result.msg = $"U盾未绑定";
                            return false;
                        }
                    }
                   
                    // 超时
                    element = FKWebAutomatic.FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@id=\"frameMain\"]", 60000);
                    if (element == null)
                    {
                        result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_WaitElementTimeout;
                        result.msg = $"等待主页超时";
                        return false;
                    }
                    LOGGER.INFO("登录成功");
                    
                    Thread.Sleep(1000);
                    if (FKWebAutomatic.FKWebDriver.GetInstance.SwitchToFrameByID("frameMain") == false)
                    {
                        LOGGER.INFO("切换iframe到frameMain失败");
                        result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_WaitElementTimeout;
                        result.msg = $"切换iframe失败";
                        return false;
                    }
                    string PSessionId = FKWebAutomatic.FKWebDriver.GetInstance.GetAttributeByXPath("//*[@name=\"PSessionId\"]","value");
                    LOGGER.INFO($"切换iframe到frameMain成功,PSessionId is {PSessionId}");
                    //string ReqSafeFields = FKWebAutomatic.FKWebDriver.GetInstance.GetAttributeByXPath("//*[@name=\"ReqSafeFields\"]", "value");
                    //LOGGER.INFO($"切换iframe到frameMain成功,ReqSafeFields is {ReqSafeFields}");
                    s_bIsLogining = true;
                    

                }
                catch (Exception e)
                {
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_LoginFailed;
                    result.msg = $"TryLogin error [{e.Message}]";
                    return false;
                }

                return true;
            }
            return s_bIsLogining;
        }

        private static System.IntPtr ActiveWindow(string strWndTitle, string strWndClass, FKWndMgr.ENUM_CursorPosInWnd type)
        {
            FKWndMgr fw = new FKWndMgr();
            System.IntPtr hWnd = fw.FindWindowHandler("", strWndTitle, strWndClass,true,true);
            if (hWnd == null || hWnd == System.IntPtr.Zero)
                return System.IntPtr.Zero;
            FKWndMgr.SetCursorPosInWnd(hWnd, FKWndMgr.ENUM_CursorPosInWnd.eCursorPosInWnd_FirstPos);
            return hWnd;
        }

        static private void SelectWnd(System.IntPtr hWnd)
        {
            FKWndMgr.SetCursorPosInWnd(hWnd, FKWndMgr.ENUM_CursorPosInWnd.eCursorPosInWnd_LastPos);
        }

        public static void LoginPassword(string strValue)
        {
            Thread.Sleep(300);
            System.IntPtr hwnd = ActiveWindow("", "ATL:Edit", FKWndMgr.ENUM_CursorPosInWnd.eCursorPosInWnd_FirstPos);
            FKWinIO.Input(strValue);
        }

        private static void ForceShutdownIE()
        {
            FKBaseUtils.FKCommonFunc.RunConsoleCommand("taskkill.exe", " /f /IM IEDriverServer.exe /IM iexplore.exe");
        }
        #region ==== HTTP辅助函数 ====

        /// <summary>
        /// 获取 查询POST 内容数据
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string GetAutoBillPostContent(SBillTaskInfo info)
        {
            // 如下是获取流水必须的参数，对于不懂得参数一律直接传
            /* PSessionId=088c8ed8ffff005abb5246f007a67eeb&
                x-channel=0&
                menuCode=P002000&
                step=batchDownLoad&
                cardNo=3d6fe2168b70566a7aa44c124f3265cd&
                selectCardNo=6222623210002390520&
                startDate=20160723&
                endDate=20170822&
                acoAcRecord=&
                queryType=&
                serialNo=&
                page=1
            */
            var cardInfo = getCardInfo();

            if (string.IsNullOrEmpty(cardInfo.cardNo))
            {
                return null;
            }
            // POST参数表
            Dictionary<string, string> postContent = new Dictionary<string, string>();
            postContent.Add("x-channel", "0");
            postContent.Add("menuCode", "P002000");
            postContent.Add("step", "batchDownLoad");
            postContent.Add("acoAcRecord", "");
            postContent.Add("queryType", "");
            postContent.Add("serialNo", "");
            postContent.Add("page", "1");
            // 非固定参数
            try
            {
                postContent.Add("PSessionId", cardInfo.PSessionId);
                postContent.Add("cardNo", cardInfo.uuid);
                postContent.Add("selectCardNo", cardInfo.cardNo);
                postContent.Add("startDate", info.startTime.Substring(0, 8));
                postContent.Add("endDate", info.endTime.Substring(0, 8));
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Parser BCM bank post data failed. Error = {e.ToString()}");
                return string.Empty;
            }

            // POST参数拼接
            return string.Join("&", postContent.Select(x => x.Key + "=" + x.Value).ToArray());
        }
        /// <summary>
        /// 获取 查询POST 头数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetAutoBillPostHeader()
        {
            /** 
            * ----------------------------------------------------------------------------------
           POST /personbank/account/acTranRecordQuery.do HTTP/1.1
                Accept: text/html, application/xhtml+xml, *\/*
                X - HttpWatch - RID: 79974 - 10267
                Referer: https://pbank.95559.com.cn/personbank/account/acTranRecordQuery.do?PSessionId=088c8ed8ffff005abb5246f007a67eeb&x-channel=0&menuCode=P002000&random=0.7627985635679371&ReqSafeFields=tiFoX0DrnqLu6AzK6NWbcD0kp1zccPVTnRtTrdxjBWeBXWf5THrQ43k6PX%252BuH51O&xFirstEntry=1&cardNo=3d6fe2168b70566a7aa44c124f3265cd
                            Accept - Language: zh-CN
                User - Agent: Mozilla / 5.0(Windows NT 6.1; WOW64; Trident / 7.0; rv: 11.0) like Gecko
                Content - Type: application / x - www - form - urlencoded
                Accept - Encoding: gzip, deflate
                  Host: pbank.95559.com.cn
                Content - Length: 232
                Connection: Keep - Alive
                Cache - Control: no - cache
                Cookie: captcha_validate = 088c8ed8ffff005abb5246f007a67eeb; com.bocom.jump.bp.channel.http.support.SmartLocaleResolver.LOCALE = zh_CN; cityPanel = 340,3680; JSESSIONID = 0000aOPqk9utf0PmIMNeZSOTNnF: -1; oldSafeInput = false */
            var cardInfo = getCardInfo();
          
            string referer = $"https://pbank.95559.com.cn/personbank/account/acTranRecordQuery.do";

            Dictionary<string, string> postHeader = new Dictionary<string, string>();
            
            postHeader.Add("Cookie", FKWebAutomatic.FKWebDriver.GetInstance.GetAllCookies());
            postHeader.Add("Accept", "text/html, application/xhtml+xml, */*");
            postHeader.Add("Referer",referer);
            postHeader.Add("Accept-Language", "zh-CN");
            postHeader.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");

            postHeader.Add("Content-Type", "application/x-www-form-urlencoded");
            postHeader.Add("Accept-Encoding", "gzip, deflate");
            postHeader.Add("Host", "pbank.95559.com.cn");

            return postHeader;
        }
        private BCM_BankInfo getCardInfo()
        {
            string PSessionId = FKWebAutomatic.FKWebDriver.GetInstance.GetAttributeByXPath("//*[@name=\"PSessionId\"]", "value");
            Dictionary<string, string> getHeader = new Dictionary<string, string>();

            getHeader.Add("Cookie", FKWebAutomatic.FKWebDriver.GetInstance.GetAllCookies());
            getHeader.Add("Accept", "text/javascript;charset=utf-8");
            getHeader.Add("Referer", $"https://pbank.95559.com.cn/personbank/app/pebs.do?PSessionId={PSessionId}&x-channel=0&menuCode=&appId=&startMenu=&ibpsProtocolReq=&pebsUrl=&args=#1");
            getHeader.Add("Accept-Language", "zh-CN");
            getHeader.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");

            getHeader.Add("Content-Type", "application/x-www-form-urlencoded");
            getHeader.Add("Accept-Encoding", "gzip, deflate");
            getHeader.Add("Host", "pbank.95559.com.cn");

            string getUrl = $"https://pbank.95559.com.cn/personbank/system/syCardList.ajax?PSessionId={PSessionId}&x-channel=0&menuCode=P002000";
            string jsonData = FKHttp.FKHttpClient.GET(getUrl, getHeader);
            LOGGER.INFO(FKHttp.FKHttpClient.GET(getUrl,getHeader));
            BCM_BankInfo bcm_BankInfo = new BCM_BankInfo();
            try
            {
                var o = JsonConvert.DeserializeObject<dynamic>(jsonData);
                bcm_BankInfo.userName = (string)o.RSP_BODY.accounts[0].alias;
                bcm_BankInfo.PSessionId = (string)o.RSP_BODY.PSessionId;
                bcm_BankInfo.safeValue = (string)o.RSP_BODY.safeValue;
                bcm_BankInfo.uuid = (string)o.RSP_BODY.accounts[0].uuid;
                bcm_BankInfo.cardNo = ((string)o.RSP_BODY.accounts[0].cardNo).Replace(" ","");
            }
            catch { }

            LOGGER.INFO($"bcm_BankInfo is {bcm_BankInfo.ToString()}");
            
            return bcm_BankInfo;
        }


        /// <summary>
        /// 解析XLS文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool parseXlsContent(string filename, ref SBillTaskResult result, SBillTaskInfo info)
        {
            ExcelReader reader = ExcelReader.CreateReader(filename);
            try
            {
                List<object> recordList = new List<object>();
                int i = 0;
                while (reader.Read(1, ++i, ref recordList))
                {
                    // 跳过第一行(标题 本账号信息 列表头)
                    if (i <= 1)
                    {
                        recordList.Clear();
                        continue;
                    }
                    /*
                     交易日期	交易时间	交易金额	本次余额	对方户名	对方账号	        交易行	            交易渠道	交易类型	交易用途	交易摘要
                     20160828	102524	    +270.00	    284.91	    罗春波	    6228270921220010475	江西省分行9999行	网上银行	转账		            网银转账
                    */

                    SBillTaskResult.SBankBillInfo billItem = new SBillTaskResult.SBankBillInfo();
                    string date = (string)recordList.ElementAt(1);
                    string time = "";
                    if (string.IsNullOrEmpty(time))
                    {
                        time = "000000";
                    }

                    string dateTime = $"{date}-{time}";
                    billItem.submitTime = GetTradeTime(dateTime);

                    // 时间不为空的需要过滤 为空总是返回 防止漏掉
                    /*if (!string.IsNullOrEmpty((string)recordList.ElementAt(1)))
                    {
                        if (!ResultFilter.TimeFilter(info, billItem.submitTime))
                        {
                            recordList.Clear();
                            continue;
                        }
                    }*/
                    //billItem.amount = Double.Parse($"{(string)recordList.ElementAt(2)}");
                    //billItem.balance = Double.Parse($"{(string)recordList.ElementAt(3)}");
                    billItem.tradeChannel = $"{(string)recordList.ElementAt(3)}";
                    string inMoney = (string)recordList.ElementAt(4);
                    string outMoney = (string)recordList.ElementAt(5);
                    if (inMoney == "--" && outMoney != "--")
                    {
                        billItem.amount = "-" + outMoney;
                    }
                    else if (inMoney != "--" && outMoney == "--")
                    {
                        billItem.amount = inMoney;
                    }
                    else
                    {
                        LOGGER.ERROR("支出收入项均无效");
                        continue;
                    }
                    billItem.accountName = $"{(string)recordList.ElementAt(7)}";
                    billItem.accountNumber = $"{(string)recordList.ElementAt(8)}";

                    billItem.accountBankName = $"{(string)recordList.ElementAt(9)}";
                    billItem.balance = ($"{(string)recordList.ElementAt(10)}");
                    
                    
                    
                    billItem.digest = $"{(string)recordList.ElementAt(11)}";
                    billItem.tradeType = TransformTradeTypeFromSummary($"{(string)recordList.ElementAt(11)}", inMoney, outMoney);
                    //billItem.tradeUsage = $"{(string)recordList.ElementAt(9)}";
                    //billItem.additionalComment = $"{(string)recordList.ElementAt(10)}";
                    // 明细未提供
                    // billItem.currency = ?

                    //LOGGER.INFO(billItem.ToString());
                    result.billsList.Add(billItem);
                    recordList.Clear();
                }
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"解析 XLS 文件失败, Error = {e.ToString()}");
                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                result.msg = "解析 XLS 文件失败.";
                return false;
            }
            finally
            {
                reader.Close();
            }
            result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_Successed;
            result.msg = "Successed";
            return true;
        }
        public static string GetTradeTime(string tradeTimeStr)
        {
            DateTime dateTime;
            DateTime.TryParseExact(tradeTimeStr, "yyyy-MM-dd-HHmmss", null, DateTimeStyles.None, out dateTime);
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        private int TransformTradeTypeFromSummary(string summary, string inBalance, string outBalance)
        {
            if (summary.Contains("税") || summary.Contains("年费"))
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_ToBank);
            }
            if (summary.Contains("息"))
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_FromBank);
            }
            if (summary.Contains("转") || summary.Contains("支付") || string.IsNullOrEmpty(summary))
            {
                if (inBalance != "--")
                {
                    return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_FromCustomer);
                }
                else if (outBalance != "--")
                {
                    return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_ToCustomer);
                }

            }

            return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_Unknown);

        }
        #endregion ==== HTTP辅助函数 ====

        #region ==== ocr(验证码)辅助函数 ====

        /// <summary>
        /// 保存验证码图片
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool SaveVerificationCodeFile(string fileName)
        {
            Console.WriteLine($"Save verification code file  {fileName}");
            return FKWebAutomatic.FKWebDriver.GetInstance.SaveImageBySnapshot("//*[@class=\"captchas-img-bg\"]", fileName);
        }
        /// <summary>
        /// ocr 识别验证码
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="pageMode"></param>
        /// <returns></returns>
        private string OCRGetStringByImage(string fileName, PageSegMode pageMode = PageSegMode.SingleLine)
        {
            string dir = System.Windows.Forms.Application.StartupPath;
            string tessDir = $"{dir}\\tessdata";
            using (var engine = new TesseractEngine(tessDir, "eng", EngineMode.Default, "bank.BCM"))
            {
                using (var img = Pix.LoadFromFile(fileName))
                {
                    using (var page = engine.Process(img, pageMode))
                    {
                        var text = page.GetText();
                        text = text.Replace(" ", "");
                        text = text.Replace("\t", "");
                        text = text.Replace("\n", "");
                        text = text.Replace("\r", "");
                        text = text.Replace("~", "");
                        return text.Trim();
                    }
                }
            }
        }
        /// <summary>
        /// 自定义识别验证码
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string CustomGetStringByImage(string fileName)
        {
            CodeHelper ch = new CodeHelper();
            string dir = System.Windows.Forms.Application.StartupPath;
            ch.LoadCodeInfo($"{dir}\\data\\abc-ocr.fkc");
            var srcImage = Image.FromFile(fileName);
            string ret = ch.GetCodeString(srcImage);
            srcImage.Dispose();

            return ret;
        }
        /// <summary>
        /// 获取验证码字符串
        /// </summary>
        /// <returns></returns>
        private string GetVerificationCodeString()
        {
            string tmpFile = System.IO.Path.GetTempFileName();
            // 保存图片文件
            if (!SaveVerificationCodeFile(tmpFile))
            {
                LOGGER.INFO($"保存文件失败");
                return "";
            }

            // 使用自定义方式解析出字符串
            string CustomString = CustomGetStringByImage(tmpFile);
            // 使用OCR解析出字符串
            string ORCString = OCRGetStringByImage(tmpFile);

            CustomString = CustomString.Replace("*", "");
            ORCString = ORCString.Replace("*", "");

            LOGGER.INFO($"验证码：FK = {CustomString} OCR = {ORCString}");
            // 删除图片文件
            File.Delete(tmpFile);
            int charCount = 5;
            if (CustomString.Length != charCount && ORCString.Length == charCount)
            {
                return ORCString;
            }
            else if (CustomString.Length == charCount && ORCString.Length != charCount)
            {
                return CustomString;
            }
            else
            {
                return ORCString;   // 自定义训练好之前优先tesser
            }
        }

        #endregion ==== ocr(验证码)辅助函数 ====

        #region ==== TestCase ====
        [TestCase()]
        public void testTessOCR()
        {
            /** 测试结果 最终选择mode = 7 即singleline模式 作为真正识别的mode
             => FKCashierBank.BCM_AutoBill.testTessOCR()
            mode 1 total stat 49 16 32.6530612244898%
            mode 2 total stat 49 16 32.6530612244898%
            mode 3 total stat 49 16 32.6530612244898%
            mode 4 total stat 49 19 38.7755102040816%
            mode 5 total stat 49 0 0%
            mode 6 total stat 49 19 38.7755102040816%
            mode 7 total stat 49 19 38.7755102040816%
            mode 8 total stat 49 15 30.6122448979592%
            mode 9 total stat 49 0 0%
            mode 10 total stat 49 0 0%
             **/
            string[] result = {
                "SXLG", "KEIB", "YOuv", "oxNd", "HGfr", "ZOBk",
                "sXLG", "HGfr", "OmQu", "PWUT", "VNDJ", "OxNd",
                "oxNd", "asoL", "aSoL", "OmQu", "OmQu", "YryZ",
                "VBGY", "FHPD", "VqHn", "hvfF", "YryZ", "VqHn",
                "raGv", "VqHn", "GLbf", "OPVa", "kYbV", "VqHa",
                "tDBO", "rGyA", "WtuN", "aTtq", "bXfQ", "DKkt",
                "FneK", "mbNx", "sHca", "opYh", "ekrC", "ekrC",
                "MAhi", "vEQO", "VEQO", "GZjE", "yrHP", "Otci",
                "FOBu" };

            for (int mode = 1; mode < (int)PageSegMode.Count; mode++)
            {
                var total = 0;
                var right = 0;
                for (int i = 1; i < 50; i++)
                {
                    string file = $"test\\testocr\\Test_{i}.bmp";
                    string ocr = OCRGetStringByImage(file, (PageSegMode)mode);
                    //string ocrResult = "no";
                    if (String.Compare(result[i - 1], ocr, true) == 0)
                    {
                        //ocrResult = "yes";
                        right++;
                    }
                    //Console.WriteLine($"file is {file} ocr is \"{ocr}\", {ocrResult}");
                    total++;
                }
                Console.WriteLine($"mode {mode} total stat {total} {right} {(double)right / total * 100}%");
            }

        }
        [TestCase()]
        public void testXls()
        {
            SBillTaskResult result = new SBillTaskResult();
            SBillTaskInfo info = new SBillTaskInfo();
        }
        #endregion ==== TestCase ====

        private static bool s_bIsLogining = false;
    }
}