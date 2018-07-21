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
// Create Time         :    2017/7/19 10:33:02
// Update Time         :    2017/7/19 10:33:02
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using FKBaseUtils;
using FKWebAutomatic;
using System.Threading;
using System.Collections.Generic;
using LOGGER = FKLog.FKLogger;
using System.Linq;
using HtmlAgilityPack;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Globalization;
using FKWndAutomatic;
using FKVerifyLib;
using System.IO;
using System.Drawing;
using Tesseract;
using System.Web;
using Newtonsoft.Json;
// ===============================================================================
namespace FKCashierBank
{
    /// <summary>
    /// 建设银行自动流水查询核心流程
    /// </summary>
    internal class CITIC_AutoBill : BankAutoBillBase
    {
        #region ==== 静态常量 ====
        private static FreqLimit s_FreqLimit = new FreqLimit();
        /*
        private static Dictionary<string, int> tradeTypeDict = new Dictionary<string, int>()
        {
            { "转账存入",(int)SBillTaskResult.ENUM_BillTradeType.eBillTradeType_TransferDeposit},
            { "跨行转出",(int)SBillTaskResult.ENUM_BillTradeType.eBillTradeType_InterBankWithdraw},
            { "账户管理费",(int)SBillTaskResult.ENUM_BillTradeType.eBillTradeType_AccountManagementFee},
            { "补扣账户管理费",(int)SBillTaskResult.ENUM_BillTradeType.eBillTradeType_AccountManagementFee},
            { "电子汇入",(int)SBillTaskResult.ENUM_BillTradeType.eBillTradeType_ElectronicDeposit},
            { "收费",(int)SBillTaskResult.ENUM_BillTradeType.eBillTradeType_Fee},
            { "unkown",(int)SBillTaskResult.ENUM_BillTradeType.eBillTradeType_Unknown},
        };
        */

        #endregion ==== 静态常量 =====

        #region ==== 成员变量 ====

        private static bool s_bIsLogining = false;      // 是否当前正处於登录状态

        #endregion ==== 成员变量 =====

        #region ==== 继承接口 ====

        /// <summary>
        /// 在每次执行查询行为之前的清理动作
        /// </summary>
        /// <param name="bNewAccount"></param>
        /// <returns></returns>
        public override bool Init()
        {
            s_FreqLimit.Check();
            try
            {
                // 如果没有登录过，清除前面残留的ie
                if (!s_bIsLogining)
                {
                    FKWebDriver.GetInstance.Close();
                    ForceShutdownIE();
                    Thread.Sleep(1000);
                }
            }
            catch(Exception e)
            {
                LOGGER.ERROR($"Init failed ... Error = {e.ToString()}");
                ForceShutdownIE();
                return false;
            }
           

            return true;
        }

        /// <summary>
        /// 每次收到服务器消息，执行查询
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
            string bankCode = "CITIC";
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

        /// <summary>
        /// 每次查询完毕的释放行为
        /// </summary>
        public override void Clear()
        {

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
                int retryCount = 3;
                for (int i = 0; i < retryCount; i++)
                {
                    if (FKWebAutomatic.FKWebDriver.GetInstance.ClickByXPath("//*[@class=\"index_page\"]"))
                    {
                        var element = FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@class=\"each_card\"]", 30000);
                        if (element != null )
                        {
                            LOGGER.INFO("CITIC刷新主页成功");
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
                        LOGGER.INFO($"CITIC刷新主页，失败[ClickByID返回false],等待重试");
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Update CITIC bank error occured, IE driver will shutdown. Error = {e.ToString()}");
                // 出现任何异常，立即关闭浏览器设置为未登录，等待下次登录
                FKWebDriver.GetInstance.Close();
                ForceShutdownIE();
                s_bIsLogining = false;
            }
            LOGGER.INFO($"CITIC刷新主页失败，清理ie等待下次重新登录");
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
                // 这是判断abc，如果不是abc，此项将为0，获取失败
                string cardCode = FKWebDriver.GetInstance.GetAttributeByXPath("//*[@class=\"each_card\"]", "cardcode");
                if (String.IsNullOrEmpty(cardCode))
                {
                    LOGGER.INFO($"当前登录用户为空，需要重新登录");
                    return false;
                }
                string[] cardCodeArr = cardCode.Split('|');
                if (cardCodeArr.Length < 2)
                {
                    LOGGER.INFO($"当前登录用户未找到，需要重新登录");
                    return false;
                }
                string accountNumber = cardCodeArr[1];
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
            }
            catch
            {
                LOGGER.INFO($"新登录用户[{FKBaseUtils.FKStringHelper.MaskString(info.accountNumber)}], 当前在线用户可能不是CITIC用户");
                return false;
            }
            LOGGER.INFO($"当前在线用户[{FKBaseUtils.FKStringHelper.MaskString(info.accountNumber)}]和登录用户一致");
            return true;
        }
        /// <summary>
        /// 查询流水
        /// </summary>
        /// <param name="info"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool QueryAutoBill(SBillTaskInfo info, ref SBillTaskResult result)
        {
            if (!s_bIsLogining)
            {
                // 未登录
                return false;
            }
            result.taskID = info.taskID;
            try
            {
                for (int i = 1; true; i++)
                {
                    string postUrl = $"{FKConfig.CITICBillUrl}?EMP_SID={getEMPSID()}";
                    if (string.IsNullOrEmpty(postUrl))
                    {
                        result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                        return false;
                    }
                    // 构造post content
                    var postContentString = GetAutoBillPostContent(info,i);
                    if (string.IsNullOrEmpty(postContentString))
                    {
                        result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                        return false;
                    }
                    // 构造post header
                    var postHeader = GetAutoBillPostHeader();
                    string postHeaderString = string.Join(";", postHeader.Select(x => x.Key + "=" + x.Value).ToArray());
                    string tmpFile = System.IO.Path.GetTempFileName();
                    // 获取post response 
                    string postResult = (string)FKHttp.FKHttpClient.POST(postUrl, postContentString, postHeader, 300000, tmpFile);
                    Console.WriteLine(postResult);
                    var doc = new HtmlDocument();
                    doc.Load(tmpFile);
                    var titleNode = doc.DocumentNode.SelectSingleNode("//title");
                    if (titleNode != null && titleNode.InnerHtml.Trim().Contains($"超时"))
                    {
                        result.msg = "刷新功能正常，获取链接出现主页超时";
                        LOGGER.WARN(result.msg);
                        s_bIsLogining = false;
                        result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                        return false;
                    }
                    //bool bRet = ParseAutoBillHtmlPage(doc, ref result, info);

                    var content = File.ReadAllLines(tmpFile, System.Text.Encoding.GetEncoding("gb2312"));
                    var allContent = String.Join(" ", content);
                    bool bRet = ParseAutoBillRegex(allContent, ref result, info);
                    LOGGER.INFO($"第{i}次获取流水之后条数{result.billsList.Count}");
                    if (bRet == false)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                result.msg = $"QueryAutoBill抛出异常{e.Message}";
                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                LOGGER.WARN($"Parse CCB bill html page failed. Error = {e.ToString()}");
                return false;
            }

            if (result.billsList.Count == 0)
            {
                result.msg = $"未获取到流水数据";
                LOGGER.WARN(result.msg);
                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                return false;
            }
            result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_Successed;
            return true;

        }
        /// <summary>
        /// 尝试进行登录
        /// </summary>
        /// <param name="info"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryLogin(SBillTaskInfo info, ref SBillTaskResult result)
        {
            // 已经处於登录状态
            if (s_bIsLogining)
                return true;

            try
            {
                string loginUrl = FKConfig.CITICLoginUrl;
                // 此部分代码根据现有CITIC的页面设计
                // 如果CITIC个人银行网页改版，此处可能需要根据新页面做相应调整
                FKWebDriver.GetInstance.OpenUrl(loginUrl);

                // 等待出现登录界面
                var element = FKWebDriver.GetInstance.WaitUntilVisibleByXPath("/html/body/form[1]/div[2]/div[3]/div[1]/ul/li[4]", 20000);
                if (element == null)
                {
                    result.msg = "等待登录页超时";
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_WaitElementTimeout;
                    return false;
                }


                if (FKWebDriver.GetInstance.IsElementVisiableByXPath("//*[@id=\"type2\"]"))
                {
                    LOGGER.INFO($"检测到U盾，跳过用户名输入");
                }
                else
                {
                    // 填充登录账号
                    FKWebDriver.GetInstance.SetTextByName("logonNoCert", info.accountNumber);
                }
                

                // 填充密码
                LoginPassword(info.password);

                var isOcrEnable = FKWebDriver.GetInstance.IsElementVisiableByXPath("//*[@class=\"loginInputVerity\"]");

                Console.WriteLine($"isOcrEnable {isOcrEnable}");
                var isOcrPass = false;
                // 处理验证码
                if (isOcrEnable)
                {
                    // 填充验证码
                    // 由于验证码可能识别失败，所以需要重试
                    int retryCount = 20;
                    for (int i = 0; i < retryCount; i++)
                    {
                        // 获取ocr
                        string ocr = getOCR();
                        // 如果识别个数都不对 没必要输入了 重刷验证码再识别
                        if (ocr.Length == 4)
                        {
                            FKWebAutomatic.FKWebDriver.GetInstance.SetTextByName("verifyCode", ocr.Replace("*", ""));
                            // 填充完成后等待识别验证码打钩
                            Thread.Sleep(1000);
                            EnterOCRErrorDialog();
                            // 识别结果获取
                            string rightSrc = FKWebAutomatic.FKWebDriver.GetInstance.GetAttributeByXPath("//*[@class=\"imgVeritySign\"]", "src");
                            // 判断验证码是否通过
                            if (rightSrc.Contains("images/default/start_button.gif"))
                            {
                                isOcrPass = true;
                                break;
                            }
                            Console.WriteLine($"src is [{rightSrc}]");

                        }

                        // 失败 重新刷新验证码
                        FKWebDriver.GetInstance.ClickByID("pinImg");
                        Thread.Sleep(1000);
                    }
                    if (!isOcrPass)
                    {
                        result.msg = "验证码超过重试次数";
                        result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_WaitElementTimeout;
                        return false;
                    }
                }
                
                bool logButton = FKWebDriver.GetInstance.DoubleClickByID("logonButton");
                // 点击登录
                Console.WriteLine($"log button {logButton}");

                // 中信银行最近改版，未通过手机绑定的用户会跳转到手机绑定页面 这里处理此页面直接跳过
                element = FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@class=\"tradingCon\"]", 30000);
                if (element == null)
                {
                    LOGGER.INFO($"未检测出验证页面");
                }
                else
                {
                    var a = element.FindElement(OpenQA.Selenium.By.TagName("a"));

                    if (a != null && a.Text == "跳过")
                    {
                        LOGGER.INFO($"检测出手机验证页面，点击跳过");
                        a.Click();
                        Thread.Sleep(2000);
                        FKWebDriver.GetInstance.ClickByID("jump");
                    }
                }
                
                // 超时
                element = FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@class=\"index_page\"]", 30000);
                if (element == null)
                {
                    result.msg = "等待主页超时";
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_WaitElementTimeout;
                    return false;
                }

                // 到了这里，将认为是成功了
                return true;
            }
            catch (Exception e)
            {
                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_LoginFailed;
                result.msg = $"TryLogin抛出异常[{e.Message}]";
                return false;
            }
        }

        #endregion ==== 核心函数 ====

        #region ==== 密码控件 ====
        private static System.IntPtr ActiveWindow(string strWndTitle, string strWndClass, FKWndMgr.ENUM_CursorPosInWnd type)
        {
            FKWndMgr fw = new FKWndMgr();
            System.IntPtr hWnd = fw.FindWindowHandler("Internet Explorer", strWndTitle, strWndClass, false, true);
            if (hWnd == null || hWnd == System.IntPtr.Zero)
                return System.IntPtr.Zero;
            FKWndMgr.SetCursorPosInWnd(hWnd, type);
            return hWnd;
        }

        public static void LoginPassword(string strValue)
        {
            Thread.Sleep(300);
            System.IntPtr hwnd = ActiveWindow("", "ATL:Edit", FKWndMgr.ENUM_CursorPosInWnd.eCursorPosInWnd_FirstPos);
            FKWinIO.Input(strValue);
        }

        public static bool EnterOCRErrorDialog()
        {
            Thread.Sleep(300);
            System.IntPtr hwnd = ActiveWindow("", "#32770", FKWndMgr.ENUM_CursorPosInWnd.eCursorPosInWnd_FirstPos);
            if (hwnd != null)
            {
                FKWinIO.KeyPress(13);
                return true;
            }
            return false;
        }

        #endregion ==== 密码控件 ====

        #region ==== http相关辅助函数 ====

        private bool ParseAutoBillRegex(string content, ref SBillTaskResult result, SBillTaskInfo info)
        {
            Regex reg = new Regex($"\\{{stdessvldt=(?<date>.*?), " +
                                    $"stdes2bref=(?<sumary>.*?), " +
                                    $"stdes1opna=(?<stdes1opna>.*?), " +
                                    $"stdsumtrsq=(?<serialnumber>.*?), " +
                                    $"equipmentNO=(?<ignore2>.*?), " +
                                    $"stdessrvfg=(?<stdessrvfg>.*?), " +
                                    $"std400desc=(?<std400desc>.*?), " +
                                    $"stdesstrno=(?<stdesstrno>.*?), " +
                                    $"stdes2opid=(?<stdes2opid>.*?), " +
                                    $"stdessdcfg=(?<stdessdcfg>.*?), " +
                                    $"stdes2bfcd=(?<stdes2bfcd>.*?), " +
                                    $"stdessctfg=(?<stdessctfg>.*?), " +
                                    $"stdes2opna=(?<stdes2opna>.*?), " +
                                    $"stdesstrdt=(?<stdesstrdt>.*?), " +
                                    $"stdoppacna=(?<username>.*?), " +
                                    $"stdesstram=(?<amount>.*?), " +
                                    $"stdessfnfg=(?<stdessfnfg>.*?), " +
                                    $"stdes1bfcd=(?<stdes1bfcd>.*?), " +
                                    $"stdessacbl=(?<balance>.*?), " +
                                    $"stdesstrtm=(?<time>.*?), " +
                                    $"stdes1opid=(?<stdes1opid>.*?), " +
                                    $"stdesstrcd=(?<stdesstrcd>.*?), " +
                                    $"fndoppacno=(?<cardnumber>.*?), " +
                                    $"stdoppbrna=(?<bankname>.*?)" +
                                    $"\\}}" +
                                    $"");


            var matches = reg.Matches(content);

            if (matches.Count == 0)
            {
                LOGGER.INFO("未找到有效流水数据，可能已经查到最后一页");
                return false;
            }

            foreach (Match one in matches)
            {
                SBillTaskResult.SBankBillInfo billItem = new SBillTaskResult.SBankBillInfo();
                // 解析单一流水对象
                billItem.submitTime = GetRegexTradeTime($"{one.Groups["date"].Value}-{one.Groups["time"].Value}");
                billItem.serialNo = one.Groups["serialnumber"].Value;
                if (billItem.serialNo == "null")
                {
                    billItem.serialNo = "";
                }
                billItem.accountBankName = one.Groups["bankname"].Value;
                if (billItem.accountBankName == "null")
                {
                    billItem.accountBankName = "";
                }
                billItem.accountName = one.Groups["username"].Value;
                if (billItem.accountName == "null")
                {
                    billItem.accountName = "";
                }
                billItem.additionalComment = one.Groups["sumary"].Value;
                if (billItem.additionalComment == "null")
                {
                    billItem.additionalComment = "";
                }
                billItem.balance = one.Groups["balance"].Value;
                if (billItem.balance == "null")
                {
                    billItem.balance = "";
                }
                billItem.amount = one.Groups["amount"].Value;
                if (billItem.amount == "null")
                {
                    billItem.amount = "0.0";
                }
                billItem.accountNumber = one.Groups["cardnumber"].Value;

                var inOrOut = one.Groups["stdessdcfg"].Value;

                if (inOrOut == "C")
                {
                    billItem.amount = "+" + billItem.amount;
                }
                else if(inOrOut == "D")
                {
                    billItem.amount = "-" + billItem.amount;
                }
                else
                {
                    LOGGER.ERROR($"未知存取类型[{inOrOut}]");
                    continue;
                }
                billItem.tradeType = TransformTradeTypeFromSummary(one.Groups["stdes2bfcd"].Value, inOrOut);
                if (ResultFilter.TimeFilter(info, billItem.submitTime))
                {
                    result.billsList.Add(billItem);
                }

            }
            return true;
        }


        public static string GetRegexTradeTime(string dateTimeStr)
        {
            DateTime dateTime;
            DateTime.TryParseExact(dateTimeStr, "yyyyMMdd-HHmmss", null, DateTimeStyles.None, out dateTime);
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            
        }
        /// <summary>
        /// 解析流水页面
        /// </summary>
        /// <param name="html"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool ParseAutoBillHtmlPage(HtmlDocument html, ref SBillTaskResult result, SBillTaskInfo info)
        {
            try
            {
                var nodes = html.DocumentNode.SelectSingleNode("./table/tbody");
                if (nodes == null)
                {
                    LOGGER.INFO("未找到有效流水数据，可能已经查到最后一页");
                    return false;
                }
                var items = nodes.SelectNodes("./tr");
                if (items == null)
                {
                    LOGGER.INFO("未找到有效流水数据，可能已经查到最后一页");
                    return false;
                }
                if (items.Count == 0)
                {
                    LOGGER.INFO("未找到有效流水数据，可能已经查到最后一页");
                    return false;
                }

                foreach (var node in nodes.SelectNodes("./tr"))
                {
                    SBillTaskResult.SBankBillInfo billItem = new SBillTaskResult.SBankBillInfo();
                    // 解析单一流水对象
                    if (ParseOneBillHtmlPage(node, ref billItem) == false)
                    {
                        continue;
                    }
                    // citic明细没有时间，一律不过滤
                    /*if (ResultFilter.TimeFilter(info, billItem.submitTime))
                    {
                        result.billsList.Add(billItem);
                    }*/

                    result.billsList.Add(billItem);
                }

                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_Successed;
            }
            catch (Exception e)
            {
                result.msg = "CITIC解析返回html错误";
                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                LOGGER.WARN($"CITIC解析返回html错误：{e.Message}");
                return false;
            }
            return true;
        }


        public static string GetTradeTime(string innerHtml)
        {
            string date = innerHtml;
            if (innerHtml == "--")
            {
                date = "1970-01-01 00:00:00";
            }
            else
            {
                date = innerHtml + " 00:00:00";
            }
            return date;
        }

        /// <summary>
        /// 从Node中解析一条流水
        /// </summary>
        /// <param name="node"></param>
        /// <param name="billItem"></param>
        private bool ParseOneBillHtmlPage(HtmlNode node, ref SBillTaskResult.SBankBillInfo billItem)
        {
            try
            {
                
                // 交易卡号全是 --
                string accountNumber = System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[2]").InnerHtml).Trim();
                billItem.accountNumber = accountNumber;
                // 日期
                string date = System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[3]").InnerHtml).Trim();
                billItem.submitTime = GetTradeTime(date);
                // 支出
                string outMoney = System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[4]").InnerHtml).Trim();

                // 收入
                string inMoney = System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[5]").InnerHtml).Trim();

                if (outMoney == "--" && inMoney == "--")
                {
                    return false;
                }
                if (outMoney != "--")
                {
                    billItem.amount = "-" + outMoney;
                }
                else if (inMoney != "--")
                {
                    billItem.amount = inMoney;
                }
                // 余额
                string balance = System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[6]").InnerHtml).Trim();

                if (balance == "--")
                {
                    return false;
                }
                billItem.balance = balance;
                // 对方 格式 ==>招商银行 尾号3085 李欢<==
                string info = System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[7]").InnerHtml).Trim();
                

                // 受理机构 具体银行
                string bank = System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[8]").InnerHtml).Trim();
                billItem.accountBankName = bank;
                var infoArr = info.Split(' ');
                if (infoArr.Length >= 1)
                {
                    if (bank == "--")
                    {
                        billItem.accountBankName = infoArr[0];
                    }
                }
                if (infoArr.Length >= 2)
                {
                    if (accountNumber == "--")
                    {
                        billItem.accountNumber = infoArr[1];
                    }
                }
                if (infoArr.Length >= 3)
                {
                    billItem.accountName = infoArr[2];
                }
                // 摘要 客户填写的附言
                string summary = System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[9]").InnerHtml).Trim();
                billItem.additionalComment = summary;
                
                // 状态 ==>完成<==
                string status = System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[10]").InnerHtml).Trim();
                billItem.digest = info + " " + status;
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Parser CITIC one bill failed. Error = {e.ToString()}");
                return false;
            }

            LOGGER.DEBUG($"Get one bill, info = {billItem.ToString()}");
            return true;
        }


        private string getEMPSID()
        {
            return FKWebDriver.GetInstance.GetAttributeByXPath("//input[@type=\"hidden\"][@name=\"EMP_SID\"]", "value");
        }
        /// <summary>
        /// 获取查询POST 头数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetAutoBillPostHeader()
        {
            /** 完整的获取明细curl命令，此命令最终返回的是一个标准xls的文件，需要进一步解析文件才能提取出明细
            * ----------------------------------------------------------------------------------
            curl 'https://i.bank.ecitic.com/perbank6/pb1310_account_detail.do?EMP_SID=JTAQGUATIOGAEWJPESGOEDCHEGEHJPDPHCGJIOBT' \
            -H 'Accept: *\/*' \
            -H 'Content-Type: application/x-www-form-urlencoded;charset=UTF-8' \
            -H 'Referer: https://i.bank.ecitic.com/perbank6/pb1310_account_detail_query.do?EMP_SID=JTAQGUATIOGAEWJPESGOEDCHEGEHJPDPHCGJIOBT' \
            -H 'Accept-Language: zh-CN' \
            -H 'Accept-Encoding: gzip, deflate' \
            -H 'User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko' \
            -H 'Host: i.bank.ecitic.com' \
            -H 'Connection: Keep-Alive' \
            -H 'Cache-Control: no-cache' \
            -H 'Cookie: BIGipServeri_80_pool=!nv4wXgniIub55XhZCuLF77H2HbcCn08Op5D4YRQ4dPwi84p4AmexMGUcY5TkukhDcUkNtxbnjd3Wbg==;JSESSIONID=0000_CfSs3BxYdnNqA65WwuobCK:19rkk4jdt;is_si_expire=0;iss_webanalytics_id=8ecd768b-911d-43bd-a849-6f056466be3e;nu=0;si=db54d8b0-917a-4bbb-80e5-5437fcef9b04;userAcctHash=0B457DE23BCB02620921DA2B8AB6D7D6' \
            --data 'accountNo=6217680704422380&largeAmount=12&opFlag=1&pageType=1&payAcctxt=6217680704422380&queryType=spacil&recordNum=10&recordSize=10&recordStart=11&stdessbgdt=20160804&stdesseddt=20170727&stdudfcyno=001&argetPage=11' 
            
             */

            Dictionary<string, string> postHeader = new Dictionary<string, string>();

            postHeader.Add("Cookie", FKWebAutomatic.FKWebDriver.GetInstance.GetAllCookies());
            postHeader.Add("Accept", "*/*");
            postHeader.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            postHeader.Add("Referer", $"https://i.bank.ecitic.com/perbank6/pb1310_account_detail_query.do?EMP_SID={getEMPSID()}");
            postHeader.Add("Accept-Language", "zh-CN");
            postHeader.Add("Accept-Encoding", "gzip, deflate");
            postHeader.Add("Host", "i.bank.ecitic.com");
            postHeader.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");

            return postHeader;
        }

        private string GetAutoBillPostContent(SBillTaskInfo info, int pageNumber)
        {
            // 如下是获取流水必须的参数，对于不懂得参数一律直接传
            /* accountNo=6217680704422380&  卡号 从传入参数中获取
             * largeAmount=12&              金额限定
             * opFlag=1&                    不懂 页面提取
             * pageType=1&                  页数相关
             * payAcctxt=6217680704422380&  卡号 从传入参数中获取
             * queryType=spacil&            页数相关
             * recordNum=10&                页数相关
             * recordSize=10&               页数相关
             * recordStart=11&              页数相关
             * stdessbgdt=20160804&         开始日期  
             * stdesseddt=20170727&         结束日期
             * stdudfcyno=001&              不懂
             * argetPage=11                 页数相关

            */

            // POST参数表
            
            Dictionary<string, string> postContent = new Dictionary<string, string>();
            /*if (File.Exists("data\\citic_auto_bill_content"))
            {
               var content = File.ReadAllLines("data\\citic_auto_bill_content");
               foreach (var oneLine in content)
               {
                   var keyAndValue = oneLine.Split(' ');
                   if (keyAndValue.Length >= 2)
                   {
                       postContent.Add(keyAndValue[0], keyAndValue[1]);
                   }
                   else
                   {
                       postContent.Add(keyAndValue[0], "");
                   }
               }
                int onePageNumber = 10;
                int recordStart = (onePageNumber * (pageNumber - 1) + 1);
                postContent.Add("recordStart", $"{recordStart}");
                postContent.Add("recordNum", $"{onePageNumber}");
            }*/
            
            postContent.Add("currList", "");
            postContent.Add("std400pgqf", "N");
            postContent.Add("opFlag", "0");
            postContent.Add("queryType", "sapcil");

            // 非固定参数
            try
            {
                int onePageNumber = 10;
                int recordStart = (onePageNumber * (pageNumber - 1) + 1);
                postContent.Add("recordStart", $"{recordStart}");
                postContent.Add("recordNum", $"{onePageNumber}");

                postContent.Add("accountNo", info.accountNumber);
                postContent.Add("payAcctxt", info.accountNumber);
                postContent.Add("stdessbgdt", info.startTime.Substring(0, 8));
                postContent.Add("stdesseddt", info.endTime.Substring(0, 8));
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Parser CITIC bank post data failed. Error = {e.ToString()}");
                return string.Empty;
            }
            
            
            // POST参数拼接
            return string.Join("&", postContent.Select(x => x.Key + "=" + x.Value).ToArray());
        }
        #endregion ==== http相关辅助函数 ====


        #region ==== ocr(验证码)辅助函数 ====

        private string tesseractGetOCR(string fileName, string tessDir, PageSegMode pageMode = PageSegMode.SingleLine)
        {
            //Console.WriteLine($"tessdir {tessDir}");

            using (var engine = new TesseractEngine(tessDir, "eng", EngineMode.Default, "bank.citic"))
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
        private bool saveOCRFile(string fileName)
        {
            Console.WriteLine($"save ocr file  {fileName}");
            return FKWebAutomatic.FKWebDriver.GetInstance.SaveImageBySnapshot("pinImg", fileName);

        }
        private void delFile(string fileName)
        {
            //File.Delete(fileName);
        }
        private string customGetOCR(string fileName)
        {

            CodeHelper ch = new CodeHelper();
            string dir = System.Windows.Forms.Application.StartupPath;
            ch.LoadCodeInfo($"{dir}\\data\\abc-ocr.fkc");
            var srcImage = Image.FromFile(fileName);
            string ocr = ch.GetCodeString(srcImage);
            srcImage.Dispose();

            return ocr;
        }
        private string getOCR()
        {
            string tmpFile = System.IO.Path.GetTempFileName();
            if (!saveOCRFile(tmpFile))
            {
                return "";
            }
            string ocr = customGetOCR(tmpFile);
            Console.WriteLine($"CustomGetStringByImage {ocr}");

            string dir = System.Windows.Forms.Application.StartupPath;
            string tessDir = $"{dir}\\tessdata";
            string tessOCR = tesseractGetOCR(tmpFile, tessDir);

            Console.WriteLine($"OCRGetStringByImage {tessOCR}");

            delFile(tmpFile);

            return tessOCR;
        }
        #endregion ==== ocr(验证码)辅助函数 ====

        #region ==== 其他辅助工具 ====
        /// <summary>
        /// 强制关闭IEDriver
        /// </summary>
        private static void ForceShutdownIE()
        {
            FKCommonFunc.RunConsoleCommand("taskkill.exe", " /f /IM IEDriverServer.exe /IM iexplore.exe");
        }

        /// <summary>
        /// 转义 交易类型
        /// </summary>
        /// <param name="summary">摘要行</param>
        /// <param name="outcome">支出金额</param>
        /// <param name="income">收入金额</param>
        /// <returns></returns>
        private int TransformTradeTypeFromSummary(string summary, string inOrOut)
        {
            if (summary.Contains("税") || summary.Contains("年费"))
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_ToBank);
            }
            if (summary.Contains("息"))
            {
               return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_FromBank);
            }
            if (inOrOut == "C")
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_FromCustomer);
            }
            else if (inOrOut == "D")
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_ToCustomer);
            }
            return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_Unknown);

        }
        #endregion ==== 其他辅助工具 ====

        #region ==== TestCase ====
        [TestCase(TestName = "citic_html")]
        public void testParseHtml()
        {
            string htmlPath = "test/citic.html";
            HtmlNode.ElementsFlags.Remove("form");
            var doc = new HtmlDocument();
            doc.Load(htmlPath);
            testParse(doc);

        }
        public static void testParse(HtmlDocument html)
        {
            var dnodes = html.DocumentNode;
            Console.WriteLine("-------------------------------------------------------");
            foreach (var o in dnodes.ChildNodes){
                Console.WriteLine(o.Name);
            }
            Console.WriteLine("-------------------------------------------------------");
            var nodes = html.DocumentNode.SelectSingleNode("./table/tbody");
            
            foreach (var node in nodes.SelectNodes("./tr"))
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine(System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[1]").InnerHtml).Trim());
                Console.WriteLine(System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[2]").InnerHtml).Trim());
                Console.WriteLine(System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[3]").InnerHtml).Trim());
                Console.WriteLine(System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[4]").InnerHtml).Trim());
                Console.WriteLine(System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[5]").InnerHtml).Trim());
                Console.WriteLine(System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[6]").InnerHtml).Trim());
                Console.WriteLine(System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[7]").InnerHtml).Trim());
                Console.WriteLine(System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[8]").InnerHtml).Trim());
                Console.WriteLine(System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[9]").InnerHtml).Trim());
                Console.WriteLine(System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("./td[10]").InnerHtml).Trim());
            }
        }
        [TestCase(TestName = "citic_reg")]
        public void testParseReg()
        {
            string file = "test/citic.html";
            var content = File.ReadAllLines(file,System.Text.Encoding.GetEncoding("gb2312"));
            var allContent = String.Join(" ", content);

            /*
             {stdessvldt=20170818,
                 stdes2bref=ds,
                 stdes1opna=null,
                 stdsumtrsq=SOBS2017081800000236534405,
                 equipmentNO=308584000013                           22101                                                        ,
                 stdessrvfg=N,
                 std400desc=null,
                 stdesstrno=J0000007107057,
                 stdes2opid=null,
                 stdessdcfg=C,
                 stdes2bfcd=null,
                 stdessctfg=T,
                 stdes2opna=null,
                 stdesstrdt=20170818,
                 stdoppacna=李欢,
                 stdesstram=0.01,
                 stdessfnfg=null,
                 stdes1bfcd=999999,
                 stdessacbl=0.14,
                 stdesstrtm=171821,
                 stdes1opid=null,
                 stdesstrcd=3301031,
                 fndoppacno=6214830255033085,
                 stdoppbrna=招商银行}
             */
            Regex reg = new Regex(  $"\\{{stdessvldt=(?<date>.*?), " +
                                    $"stdes2bref=(?<sumary>.*?), " + 
                                    $"stdes1opna=(?<stdes1opna>.*?), " +
                                    $"stdsumtrsq=(?<serialnumber>.*?), " +
                                    $"equipmentNO=(?<ignore2>.*?), " +
                                    $"stdessrvfg=(?<stdessrvfg>.*?), " +
                                    $"std400desc=(?<std400desc>.*?), " +
                                    $"stdesstrno=(?<stdesstrno>.*?), " +
                                    $"stdes2opid=(?<stdes2opid>.*?), " +
                                    $"stdessdcfg=(?<stdessdcfg>.*?), " +
                                    $"stdes2bfcd=(?<stdes2bfcd>.*?), " +
                                    $"stdessctfg=(?<stdessctfg>.*?), " +
                                    $"stdes2opna=(?<stdes2opna>.*?), " +
                                    $"stdesstrdt=(?<stdesstrdt>.*?), " +
                                    $"stdoppacna=(?<username>.*?), " +
                                    $"stdesstram=(?<amount>.*?), " +
                                    $"stdessfnfg=(?<stdessfnfg>.*?), " +
                                    $"stdes1bfcd=(?<stdes1bfcd>.*?), " +
                                    $"stdessacbl=(?<balance>.*?), " +
                                    $"stdesstrtm=(?<time>.*?), " +
                                    $"stdes1opid=(?<stdes1opid>.*?), " +
                                    $"stdesstrcd=(?<stdesstrcd>.*?), " +
                                    $"fndoppacno=(?<cardnumber>.*?), " +
                                    $"stdoppbrna=(?<bankname>.*?)" +
                                    $"\\}}" +
                                    $"");


            var matches = reg.Matches(allContent);
            if (matches.Count > 0)
            {
                
                foreach (Match one in matches)
                {
                    Console.WriteLine($"--------------------------------------------");
                    Console.WriteLine($"match [{one.Groups["date"].Value}]");
                    Console.WriteLine($"match [{one.Groups["sumary"].Value}]");
                    Console.WriteLine($"match [{one.Groups["stdes1opna"].Value}]");
                    Console.WriteLine($"match [{one.Groups["serialnumber"].Value}]");
                    Console.WriteLine($"match [{one.Groups["username"].Value}]");
                    Console.WriteLine($"match [{one.Groups["balance"].Value}]");
                    Console.WriteLine($"match [{one.Groups["amount"].Value}]");
                    Console.WriteLine($"match [{one.Groups["time"].Value}]");
                    Console.WriteLine($"match [{one.Groups["cardnumber"].Value}]");
                    Console.WriteLine($"match [{one.Groups["bankname"].Value}]");
                    Console.WriteLine($"match [{one.Groups["stdessdcfg"].Value}]");
                    Console.WriteLine($"match [{one.Groups["stdessctfg"].Value}]");
                }
            }
            /*var matchesDate = Regex.Matches(allContent, @"(?<=stdessvldt\=)\d{8}");
            if (matchesDate.Count > 0)
            {
                foreach (Match one in matchesDate)
                {
                    Console.WriteLine($"match {one.Value}");
                }
            }

            var matchesSummary = Regex.Matches(allContent, @"(?<=stdes2bref\=)[\w]*(, stdes1opna=>?)");
            if (matchesSummary.Count > 0)
            {
                foreach (Match one in matchesSummary)
                {
                    Console.WriteLine($"match {one.Value}");
                }
            }*/
        }

        #endregion ==== TestCase ====
    }
}