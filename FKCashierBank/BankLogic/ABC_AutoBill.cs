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
using FKVerifyLib;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Linq;
using System.Collections.Generic;
using LOGGER = FKLog.FKLogger;
using Tesseract;
using NUnit.Framework;
using FKXlsLib;
using System.Globalization;
using System.Text;
// ===============================================================================
namespace FKCashierBank
{
    /// <summary>
    /// 交通银行自动流水查询核心流程
    /// </summary>
    internal class ABC_AutoBill : BankAutoBillBase
    {
        private static bool s_bIsLogining = false;
        private static FreqLimit s_FreqLimit = new FreqLimit();
        #region ==== 继承接口 ====

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            s_FreqLimit.Check();
            try { 
                // 如果没有登录过，清除前面残留的ie
                if (s_bIsLogining == false)
                {
                    var bankCode = "ABC";
                    LOGGER.INFO($"银行代码为[{bankCode}]的账号未在线，清理当前在线账号");
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
        /// <summary>
        /// 自动查询核心函数
        /// </summary>
        /// <param name="nTaskID"></param>
        /// <param name="info"></param>
        /// <param name="result"></param>
        /// <returns></returns>
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
            string logPrefix = $"银行代码[ABC]获取明细，TaskID[{info.taskID}]，";
            LOGGER.INFO($"{logPrefix}正在尝试登录 ...");

            s_bIsLogining = TryLogin(info, ref result);
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

            if (!ret)
            {
                LOGGER.INFO($"{logPrefix}获取明细失败，失败原因[{result.msg}]");
            }
            else
            {
                LOGGER.INFO($"{logPrefix}获取明细成功");
            }
            return ret;
        }
        /// <summary>
        /// 清理函数
        /// </summary>
        public override void Clear()
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 刷新事件
        /// </summary>
        public static void Update()
        {
            if (!s_bIsLogining) // 未登录，不做任何刷新
                return;
            try
            {
                // 农行刷新时经常超时导致刷新失败，重试3次，有效减少超时导致的失败。
                int retryCount = 3;
                for (int i = 0; i < retryCount; i++)
                {
                    // 刷新首页按钮，保证登录状态
                    if (FKWebAutomatic.FKWebDriver.GetInstance.ClickByXPath("/html/body/div[5]/div/div[2]/ul/li[1]/span"))
                    {
                        LOGGER.INFO("ABC刷新主页成功");
                        return;
                    }
                    else
                    {
                        // 出现任何异常，立即关闭浏览器设置为未登录，等待下次登录
                        LOGGER.INFO($"ABC刷新主页，失败[ClickByID返回false],等待重试");
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.INFO($"ABC刷新主页，失败[{e.Message}]");
            }

            LOGGER.INFO($"ABC刷新主页失败，清理ie等待下次重新登录");
            // 重试后仍然失败
            FKWebDriver.GetInstance.Close();
            ForceShutdownIE();
            s_bIsLogining = false;

        }
        /// <summary>
        /// 刷新推荐的间隔时间( 单位：秒 )
        /// </summary>
        /// <returns></returns>
        public static int GetUpdateIdleSecond()
        {
            return 60;
        }

        #endregion ==== 继承接口 =====

        #region ==== 核心函数 ====

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
            // 已登录，从页面获取account
            try
            {
                FKWebDriver.GetInstance.SwitchToFrameByID("contentFrame");
                // 这是判断abc，如果不是abc，此项将为0，获取失败
                string accountNumber = FKWebDriver.GetInstance.GetAttributeByXPath("//*[@id=\"debit\"]", "data-acctid");
                if (String.IsNullOrEmpty(accountNumber))
                {
                    LOGGER.INFO($"当前登录用户为空，需要重新登录");
                    return false;
                }
                if (String.Compare(info.accountNumber, accountNumber, true) != 0)
                {
                    LOGGER.INFO($"新登录用户[{FKBaseUtils.FKStringHelper.MaskString(info.accountNumber)}], 当前在线用户[{FKBaseUtils.FKStringHelper.MaskString(accountNumber)}]");
                    return false;
                }
                FKWebDriver.GetInstance.SwitchToParentFrame();
            }
            catch
            {
                LOGGER.INFO($"新登录用户[{FKBaseUtils.FKStringHelper.MaskString(info.accountNumber)}], 当前在线用户可能不是ABC用户");
                return false;
            }
            LOGGER.INFO($"当前在线用户[{FKBaseUtils.FKStringHelper.MaskString(info.accountNumber)}]和登录用户一致");
            return true;
        }
        

        /// <summary>
        /// 查询流水（在已登录成功后）
        /// </summary>
        /// <param name="info"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool QueryAutoBill(SBillTaskInfo info, ref SBillTaskResult result)
        {
            /** 完整的获取明细curl命令，此命令最终返回的是一个标准xls的文件，需要进一步解析文件才能提取出明细
             * ----------------------------------------------------------------------------------
             curl 'https://perbank.abchina.com/EbankSite/AccountTradeDetailDownloadAct.do' 
            -H 'Accept:	text/html, application/xhtml+xml, *\/*' 
            - H 'Referer: https://perbank.abchina.com/EbankSite/AccountTradeDetailQueryInitAct.do'
            - H 'Accept-Language: zh-CN'
            - H 'User-Agent:	Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko'
            - H 'Content-Type: application/x-www-form-urlencoded'
            - H 'Accept-Encoding: gzip, deflate'
            - H 'Host: perbank.abchina.com'
            - H 'Content-Length: 136'
            - H 'Connection: Keep-Alive'
            - H 'Cookie:	WT_FPC=id=10.235.177.249-2296909152.30606638:lv=1500978056097:ss=1500977553848; ASP.NET_SessionId=elwlscn2t0puisyczziuwwdt; _ABCPerbankLogonCookie__=; BIGipServerpool_perbank_EbankSite=!Q5CBtVY5fUNWSa38cCqlIAZb8CxfUQboxPynXFLAUdu31cHrqqE7Bl6NuLXg8tH9gLUKWOZc2cVc9J8='
            --data 'acctId=6228482568556511677&acctOpenBankId=34905&acctType=401&provCode=12&acctCurCode=156&oofeFlg=0&trnStartDt=20160804&trnEndDt=20170727'
            
             * ----------------------------------------------------------------------------------
             * --data参数解释
             * acctId           卡号 从传入参数中获取
             * acctOpenBankId   开户行 需要从页面提取
             * acctType         不懂 但是应该是账户相关 需要从页面提取
             * provCode         省份code 页面提取
             * acctCurCode      不懂 页面提取
             * oofeFlg          不懂 传0
             * trnStartDt       开始日期
             * trnEndDt         结束日期
             * **/
            if (!s_bIsLogining)
            {
                // 未登录
                return false;
            }
            result.taskID = info.taskID;
            try
            {
                // 实际测试过程中发现这个链接是随着账号改变，需要在首页中获取
                string postUrl = FKConfig.ABCBillUrl;
                if (string.IsNullOrEmpty(postUrl))
                {
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                    result.msg = "ABC Post url获取失败";
                    return false;
                }
                // 构造post content
                var postContentString = GetAutoBillPostContent(info);
                if (string.IsNullOrEmpty(postContentString))
                {
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                    result.msg = "ABC Post content构造失败";
                    return false;
                }
                // 构造post header
                var postHeader = GetAutoBillPostHeader();

                // POST获取文件
                string tmpFile = System.IO.Path.GetTempFileName();
                string postResult = (string)FKHttp.FKHttpClient.POST(postUrl, postContentString, postHeader, 300000, tmpFile);
            
                if (File.Exists(tmpFile))
                {
                    bool bRet = parseXlsContent(tmpFile, ref result, info);
                    int nBillCount = 0;
                    if(result.billsList != null)
                    {
                        nBillCount = result.billsList.Count;
                    }
                    LOGGER.INFO($"XLS Tmp file = {tmpFile} InfoLen = {nBillCount}");
                    //delFile(tmpFile); // TODO: 前期测试不要删除，文件不太大的话，不会引发过多问题，之后版本删除
                    return bRet;
                }
                else
                {
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                    LOGGER.INFO($"Can't find XLS Tmp file = {tmpFile}");
                    return false;
                }
            }
            catch (Exception e)
            {
                result.msg = $"QueryAutoBill抛出异常[{e.Message}]";
                return false;
            }
        }
        /// <summary>
        /// 尝试登录
        /// </summary>
        /// <param name="info"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryLogin(SBillTaskInfo info, ref SBillTaskResult result)
        {
            if (s_bIsLogining)
                return true;

            bool isOCRPass = false;
            try
            {
                //FKWebDriver webDriver = FKWebDriver.GetInstance;
                string loginUrl = FKConfig.ABCLoginUrl;
                // 此部分代码根据现有网银官网的页面设计
                // 如果个人银行网页改版，此处可能需要根据新页面做相应调整
                FKWebAutomatic.FKWebDriver.GetInstance.OpenUrl(loginUrl);

                // 等待出现登录界面
                var element = FKWebAutomatic.FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@id=\"username\"]", 30000);
                if (element == null)
                {
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_WaitElementTimeout;
                    result.msg = "等待登录账号元素超时，可能页面未正常打开";
                    return false;
                }

                // 填充登录账号
                FKWebAutomatic.FKWebDriver.GetInstance.SetTextByID("username", info.username);
                // 填充密码
                LoginPassword(info.password);
                // 填充验证码,由于验证码可能识别失败，所以需要重试
                int retryCount = 20;
                for (int i = 0; i < retryCount; i ++)
                {
                    string ocr = GetVerificationCodeString();
                   
                    // 如果识别个数都不对 没必要输入了 重刷验证码再识别
                    if (ocr.Length == 4)
                    {
                        FKWebAutomatic.FKWebDriver.GetInstance.SetTextByID("code", ocr);
                        //移走焦点
                        FKWebAutomatic.FKWebDriver.GetInstance.ClickByID("username");
                        // 填充完成后等待识别验证码打钩
                        Thread.Sleep(5000);
                        // 第一次此元素为隐藏，等待出现
                        FKWebAutomatic.FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@id=\"imgError\"]",10000);
                        // 识别结果获取
                        string successString = FKWebAutomatic.FKWebDriver.GetInstance.GetAttributeByXPath("//*[@id=\"imgError\"]", "class");
                        if (successString == "v-code-error right")
                        {
                            // 点击登录
                            FKWebAutomatic.FKWebDriver.GetInstance.ClickByID("logo");
                            bool isInput = false;
                            // 用户名提示为空 重新输入
                            if (FKWebAutomatic.FKWebDriver.GetInstance.IsElementVisiableByXPath("//*[@id=\"username-error\"]"))
                            {
                                LOGGER.INFO("用户名为空，重新输入");
                                FKWebAutomatic.FKWebDriver.GetInstance.SetTextByID("username", info.username);
                                isInput = true;
                            }
                            // 密码提示为空 重新输入
                            if (FKWebAutomatic.FKWebDriver.GetInstance.IsElementVisiableByXPath("//*[@id=\"powerpass_ie_dyn_Msg\"]"))
                            {
                                LOGGER.INFO("密码为空或者不正确，重新输入");
                                LoginPassword(info.password,true);
                                isInput = true;
                            }

                            // 多次输入验证码失败后 银行重新清空用户名密码，重填
                            if (FKWebAutomatic.FKWebDriver.GetInstance.IsElementVisiableByXPath("/html/body/div[2]/div[5]/div/div[1]/div[5]/form/div[5]"))
                            {
                                FKWebAutomatic.FKWebDriver.GetInstance.SetTextByID("username", info.username);
                                LoginPassword(info.password, true);
                                isInput = true;
                            }
                            if (isInput)
                            {
                                // 输入后需要重新点击登录
                                FKWebAutomatic.FKWebDriver.GetInstance.ClickByID("logo");
                            }
                            isOCRPass = true;
                            break;
                        }
                        Console.WriteLine($"successString = {successString}");
                    }
                    // 失败 重新刷新验证码
                    FKWebAutomatic.FKWebDriver.GetInstance.ClickByID("vCode");
                    Thread.Sleep(1000);
                }

                if (!isOCRPass)
                {
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_WaitElementTimeout;
                    result.msg = "验证码超过重试次数";
                    return false;
                }
                
                // 超时等待登陆成功
                element = FKWebAutomatic.FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@id=\"intro4-1\"]", 30000);
                if (element != null)
                {
                    LOGGER.INFO("成功获取登录element，登录成功");
                    return true;
                }
                else
                {
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_LoginFailed;
                    result.msg = $"等待首页超时";
                    return false;
                }
            }
            catch (Exception e)
            {
                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_LoginFailed;
                result.msg = $"TryLogin抛出异常[[{e.Message}]";
                return false;
            }
        }
        /// <summary>
        /// 强制关闭 IEDriver 和 IE 进程
        /// </summary>
        private static void ForceShutdownIE()
        {
            FKBaseUtils.FKCommonFunc.RunConsoleCommand("taskkill.exe", " /f /IM IEDriverServer.exe /IM iexplore.exe");
        }

        #endregion ==== 核心函数 ====

        #region ==== 明细xls处理 =====

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
                    // 跳过前三行(标题 本账号信息 列表头)
                    if (i <= 3)
                    {
                        recordList.Clear();
                        continue;
                    }
                    /*
                     交易日期	交易时间	交易金额	本次余额	对方户名	对方账号	        交易行	            交易渠道	交易类型	交易用途	交易摘要
                     20160828	102524	    +270.00	    284.91	    罗春波	    6228270921220010475	江西省分行9999行	网上银行	转账		            网银转账
                    */
                    
                    SBillTaskResult.SBankBillInfo billItem = new SBillTaskResult.SBankBillInfo();
                    string date = (string)recordList.ElementAt(0);
                    string time = (string)recordList.ElementAt(1);
                    if (string.IsNullOrEmpty(time))
                    {
                        time = "000000";
                    }

                    string dateTime = $"{date}-{time}";
                    billItem.submitTime = GetTradeTime(dateTime);

                    // 时间不为空的需要过滤 为空总是返回 防止漏掉
                    if (!string.IsNullOrEmpty((string)recordList.ElementAt(1)))
                    {
                        if(!ResultFilter.TimeFilter(info, billItem.submitTime))
                        {
                            recordList.Clear();
                            continue;
                        }
                    }
                    //billItem.amount = Double.Parse($"{(string)recordList.ElementAt(2)}");
                    //billItem.balance = Double.Parse($"{(string)recordList.ElementAt(3)}");
                    billItem.amount = ($"{(string)recordList.ElementAt(2)}");
                    billItem.balance = ($"{(string)recordList.ElementAt(3)}");
                    billItem.accountName = $"{(string)recordList.ElementAt(4)}";
                    billItem.accountNumber = $"{(string)recordList.ElementAt(5)}";
                    billItem.accountBankName = $"{(string)recordList.ElementAt(6)}";
                    billItem.tradeChannel = $"{(string)recordList.ElementAt(7)}";
                    billItem.digest = $"{(string)recordList.ElementAt(10)}";
                    billItem.tradeType = TransformTradeTypeFromSummary($"{(string)recordList.ElementAt(10)}", $"{(string)recordList.ElementAt(2)}");
                    billItem.tradeUsage = $"{(string)recordList.ElementAt(9)}";
                    billItem.additionalComment = $"{(string)recordList.ElementAt(10)}";
                    // 明细未提供
                    // billItem.currency = ?

                    result.billsList.Add(billItem);
                    recordList.Clear();
                }            
            }
            catch(Exception e)
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
        /// <summary>
        /// 转义 交易类型
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        private int TransformTradeTypeFromSummary(string summary, string moneyChange)
        {
            if (summary.Contains("手续费") || summary.Contains("年费"))
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_ToBank);
            }
            if (summary.Contains("网银转账") || summary.Contains("转支"))
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_ToCustomer);
            }
            if (summary.Contains("现存") || summary.Contains("支付宝发") || summary.Contains("转存"))
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_FromCustomer);
            }
            if (summary.Contains("息"))
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_FromBank);
            }
            try
            {
                int index = moneyChange.IndexOf("-");
                if(index <= -1)
                {
                    return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_FromCustomer);
                }
                else
                {
                    return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_ToCustomer);
                }
            }
            catch
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_Unknown);
            }
        }
        /// <summary>
        /// 转义 交易时间
        /// </summary>
        /// <param name="tradeTimeStr"></param>
        /// <returns></returns>
        public static string GetTradeTime(string tradeTimeStr)
        {
            DateTime dateTime;
            DateTime.TryParseExact(tradeTimeStr, "yyyyMMdd-HHmmss", null, DateTimeStyles.None, out dateTime);
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #endregion ==== 明细xls处理 =====

        #region ==== http相关辅助函数 ====

        /// <summary>
        /// 获取 查询POST 头数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetAutoBillPostHeader()
        {
            /** 完整的获取明细curl命令，此命令最终返回的是一个标准xls的文件，需要进一步解析文件才能提取出明细
            * ----------------------------------------------------------------------------------
            curl 'https://perbank.abchina.com/EbankSite/AccountTradeDetailDownloadAct.do' 
           -H 'Accept:	text/html, application/xhtml+xml, *\/*' 
           - H 'Referer: https://perbank.abchina.com/EbankSite/AccountTradeDetailQueryInitAct.do'
           - H 'Accept-Language: zh-CN'
           - H 'User-Agent:	Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko'
           - H 'Content-Type: application/x-www-form-urlencoded'
           - H 'Accept-Encoding: gzip, deflate'
           - H 'Host: perbank.abchina.com'
           - H 'Content-Length: 136'
           - H 'Connection: Keep-Alive'
           - H 'Cookie:	WT_FPC=id=10.235.177.249-2296909152.30606638:lv=1500978056097:ss=1500977553848; ASP.NET_SessionId=elwlscn2t0puisyczziuwwdt; _ABCPerbankLogonCookie__=; BIGipServerpool_perbank_EbankSite=!Q5CBtVY5fUNWSa38cCqlIAZb8CxfUQboxPynXFLAUdu31cHrqqE7Bl6NuLXg8tH9gLUKWOZc2cVc9J8='
           --data 'acctId=6228482568556511677&acctOpenBankId=34905&acctType=401&provCode=12&acctCurCode=156&oofeFlg=0&trnStartDt=20160804&trnEndDt=20170727'
           */

            Dictionary<string, string> postHeader = new Dictionary<string, string>();

            postHeader.Add("Cookie", FKWebAutomatic.FKWebDriver.GetInstance.GetAllCookies());
            postHeader.Add("Accept", "text/html, application/xhtml+xml, */*");
            postHeader.Add("Referer", "https://perbank.abchina.com/EbankSite/AccountTradeDetailQueryInitAct.do");
            postHeader.Add("Accept-Language", "zh-CN");
            postHeader.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");

            postHeader.Add("Content-Type", "application/x-www-form-urlencoded");
            postHeader.Add("Accept-Encoding", "gzip, deflate");
            postHeader.Add("Host", "perbank.abchina.com");
           
            return postHeader;
       }
        /// <summary>
        /// 获取 查询POST 内容数据
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string GetAutoBillPostContent(SBillTaskInfo info)
        {
           // 如下是获取流水必须的参数，对于不懂得参数一律直接传
           /* acctId=6228482568556511677&acctOpenBankId=34905&acctType=401&provCode=12&acctCurCode=156&oofeFlg=0&trnStartDt=20160804&trnEndDt=20170727'
           
            * acctId           卡号 从传入参数中获取
            * acctOpenBankId   开户行 需要从页面提取
            * acctType         不懂 但是应该是账户相关 需要从页面提取
            * provCode         省份code 页面提取
            * acctCurCode      不懂 页面提取
            * oofeFlg          不懂 传0
            * trnStartDt       开始日期
            * trnEndDt         结束日期
           */

            // POST参数表
            Dictionary<string, string> postContent = new Dictionary<string, string>();
            // 固定参数，经过实测 只需要acctType 但是这个无法从页面获取 先写死
            //postContent.Add("acctOpenBankId", "34905");
            postContent.Add("acctType", "401");
            //postContent.Add("provCode", "12");
            //postContent.Add("acctCurCode", "156");
            //postContent.Add("oofeFlg", "0");
            
            // 非固定参数
            try
            {
                postContent.Add("acctId", info.accountNumber);
                postContent.Add("trnStartDt", info.startTime.Substring(0, 8));
                postContent.Add("trnEndDt", info.endTime.Substring(0, 8));
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Parser ABC bank post data failed. Error = {e.ToString()}");
                return string.Empty;
            }

            // POST参数拼接
            return string.Join("&", postContent.Select(x => x.Key + "=" + x.Value).ToArray());
        }

        #endregion ==== http相关辅助函数 ====

        #region ==== 密码控件 ====

        /// <summary>
        /// 激活密码控件
        /// </summary>
        /// <param name="strWndTitle"></param>
        /// <param name="strWndClass"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static System.IntPtr ActiveWindow(string strWndTitle, string strWndClass, FKWndMgr.ENUM_CursorPosInWnd type)
        {
            FKWndMgr fw = new FKWndMgr();
            System.IntPtr hWnd = fw.FindWindowHandler("Internet Explorer", strWndTitle, strWndClass, false, true);
            if (hWnd == null || hWnd == System.IntPtr.Zero)
                return System.IntPtr.Zero;
            FKWndMgr.SetCursorPosInWnd(hWnd, type);
            return hWnd;
        }
        /// <summary>
        /// 输入密码
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="needClean"></param>
        public static void LoginPassword(string strValue, bool needClean = false)
        {
            Thread.Sleep(300);
            System.IntPtr hwnd = ActiveWindow("", "Edit", FKWndMgr.ENUM_CursorPosInWnd.eCursorPosInWnd_FirstPos);
            if (needClean)
            {
                int maxPassLen = 33;
                for (int i = 0;  i < maxPassLen; i++)
                {
                    // 删除
                    FKWinIO.KeyPress(46);
                }
            }
            
            FKWinIO.Input(strValue);
        }

        #endregion ==== 密码控件 ====

        #region ==== ocr(验证码)辅助函数 ====

        /// <summary>
        /// 保存验证码图片
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool SaveVerificationCodeFile(string fileName)
        {
            Console.WriteLine($"Save verification code file  {fileName}");
            return FKWebAutomatic.FKWebDriver.GetInstance.SaveImageBySnapshot("vCode", fileName);
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
            using (var engine = new TesseractEngine(tessDir, "eng", EngineMode.Default, "bank.abc"))
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
                return "";
            }

            // 使用自定义方式解析出字符串
            string CustomString = CustomGetStringByImage(tmpFile);
            // 使用OCR解析出字符串
            string ORCString = "";// OCRGetStringByImage(tmpFile);

            CustomString = CustomString.Replace("*", "");
            ORCString = ORCString.Replace("*", "");

            LOGGER.INFO($"验证码：FK = {CustomString} OCR = {ORCString}");
            // 删除图片文件
            File.Delete(tmpFile);

            if (CustomString.Length != 4 && ORCString.Length == 4)
            {
                return ORCString;
            }
            else if(CustomString.Length == 4 && ORCString.Length != 4)
            {
                return CustomString;
            }
            else
            {
                return CustomString;   // 优先自定义
            }
        }

        #endregion ==== ocr(验证码)辅助函数 ====

        #region ==== TestCase ====
        [TestCase()]
        public void testTessOCR()
        {
            /** 测试结果 最终选择mode = 7 即singleline模式 作为真正识别的mode
             => FKCashierBank.ABC_AutoBill.testTessOCR()
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
                    if (String.Compare(result[i-1],ocr,true) == 0)
                    {
                        //ocrResult = "yes";
                        right++;
                    }
                    //Console.WriteLine($"file is {file} ocr is \"{ocr}\", {ocrResult}");
                    total++;
                }
                Console.WriteLine($"mode {mode} total stat {total} {right} {(double)right/total * 100}%");
            }

        }
        [TestCase()]
        public void testXls()
        {
            SBillTaskResult result = new SBillTaskResult();
            SBillTaskInfo info = new SBillTaskInfo();
            parseXlsContent($"D:\\code\\FKCashier\\FKCashierBank\\bin\\x86\\Debug\\test\\detailabc20170727.xls", ref result, info);
        }
        #endregion ==== TestCase ====
    }
}