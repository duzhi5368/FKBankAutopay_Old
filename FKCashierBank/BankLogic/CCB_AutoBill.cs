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

// ===============================================================================
namespace FKCashierBank
{
    /// <summary>
    /// 建设银行自动流水查询核心流程
    /// </summary>
    internal class CCB_AutoBill : BankAutoBillBase
    {
        #region ==== 静态常量 ====

        private static Dictionary<string, string> s_CCBStandardCurrency = new Dictionary<string, string>()
        {
            { "人民币","CNY"},
            { "美元","USD"},
            { "英镑","GBP"},
        };
        #endregion ==== 静态常量 =====

        #region ==== 成员变量 ====

        private static bool s_bIsLogining = false;      // 是否当前正处於登录状态
        private static SBillTaskInfo s_BankInfo;
        private static FreqLimit s_FreqLimit = new FreqLimit();

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
                FKWebDriver.GetInstance.Close();
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
            s_BankInfo = info;
            // 判断当前是否是同一个账号
            if (!IsSameAccount(info))
            {
                FKWebDriver.GetInstance.Close();
                ForceShutdownIE();
                s_bIsLogining = false;
            }
            // 先尝试进行登录
            string bankCode = "CCB";
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
                LOGGER.INFO("CCB刷新主页");
                // 刷新首页按钮，保证登录状态

                if (s_BankInfo != null)
                {
                    Thread.Sleep(1000);
                    /*var element = FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@id=\"txmainfrm\"]", 10000);
                    if (element == null)
                    {
                        LOGGER.INFO($"CCB刷新主页，失败[WaitUntilVisibleByXPath(txmainfrm) 等待超时 ]");
                        FKWebDriver.GetInstance.Close();
                        ForceShutdownIE();
                        s_bIsLogining = false;
                        return;
                    }*/
                    if (!FKWebDriver.GetInstance.SwitchToFrameByID("txmainfrm"))
                    {
                        LOGGER.INFO($"CCB刷新主页，失败[SwitchToFrameByID(txmainfrm)返回false]");
                        FKWebDriver.GetInstance.Close();
                        ForceShutdownIE();
                        s_bIsLogining = false;
                        return;
                    }
                    Thread.Sleep(1000);
                    /*var element = FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@id=\"jhform\"]", 10000);
                    if (element == null)
                    {
                        LOGGER.INFO($"CCB刷新主页，失败[WaitUntilVisibleByXPath(jhform) 等待超时 ]");
                        FKWebDriver.GetInstance.Close();
                        ForceShutdownIE();
                        s_bIsLogining = false;
                        return;
                    }*/
                    if (!FKWebDriver.GetInstance.SwitchToFrameByID("result"))
                    {
                        LOGGER.INFO($"CCB刷新主页，失败[SwitchToFrameByID(result)返回false]");
                        FKWebDriver.GetInstance.Close();
                        ForceShutdownIE();
                        s_bIsLogining = false;
                        return;
                    }
                    Thread.Sleep(1000);
                    var cardNumber = FKWebDriver.GetInstance.GetAttributeByXPath("//*[@id=\"diaoyong_flag\"]", "value");

                    FKWebDriver.GetInstance.SwitchToParentFrame();
                    FKWebDriver.GetInstance.SwitchToParentFrame();

                    if (cardNumber != s_BankInfo.accountNumber)
                    {
                        LOGGER.INFO($"CCB刷新主页失败，账号不一致");
                        FKWebDriver.GetInstance.Close();
                        ForceShutdownIE();
                        s_bIsLogining = false;
                        return;
                    }
                }

                if (!FKWebDriver.GetInstance.ClickByID("pnav_V6020000"))
                {
                    // 出现任何异常，立即关闭浏览器设置为未登录，等待下次登录
                    LOGGER.INFO($"CCB刷新主页，失败[ClickByID返回false]");
                    FKWebDriver.GetInstance.Close();
                    ForceShutdownIE();
                    s_bIsLogining = false;
                }
            }
            catch (Exception e)
            {
                LOGGER.INFO($"CCB刷新主页，失败[{e.Message}]");
                // 出现任何异常，立即关闭浏览器设置为未登录，等待下次登录
                FKWebDriver.GetInstance.Close();
                ForceShutdownIE();
                s_bIsLogining = false;
            }
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
                // 这是判断ccb，如果不是ccb，此项将为0，获取失败
                string userName = FKWebDriver.GetInstance.GetAttributeByXPath("/html/body/div[6]/div[6]/div[1]/div[1]/p/b/span", "title");
                if (String.IsNullOrEmpty(userName))
                {
                    return false;
                }
                if (String.Compare(info.username, userName, true) != 0)
                {
                    LOGGER.INFO($"新登录用户[{FKBaseUtils.FKStringHelper.MaskString(info.username)}], 当前在线用户[{FKBaseUtils.FKStringHelper.MaskString(userName)}]");
                    return false;
                }
            }
            catch {
                LOGGER.INFO($"新登录用户[{FKBaseUtils.FKStringHelper.MaskString(info.username)}], 当前在线用户可能不是CCB用户");
                return false;
            }
            LOGGER.INFO($"当前在线用户[{FKBaseUtils.FKStringHelper.MaskString(info.username)}]和登录用户一致");
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
                result.msg = "查流水前未登录";
                return false;
            }
            result.taskID = info.taskID;
            try
            {
                // 实际测试过程中发现这个链接是随着账号改变，需要在首页中获取
                string postUrl = GetAutoBillPostUrl();
                //LOGGER.WARN($"===========================postUrl is {postUrl}");
                if (string.IsNullOrEmpty(postUrl))
                {
                    result.msg = "获取流水url错误";
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                    return false;
                }
                
                for (int i = 1; true; i++)
                {
                    // 构造post header
                    var postHeader = GetAutoBillPostHeader();

                    // 构造post content
                    var postContentString = GetAutoBillPostContent(info,i);
                    //LOGGER.WARN($"============================postContentString is {postContentString}");
                    if (string.IsNullOrEmpty(postContentString))
                    {
                        result.msg = "构造流水postContent错误";
                        result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                        return false;
                    }

                    //string header = string.Join("\r\n", postHeader.Select(x => x.Key + ": " + x.Value).ToArray());
                    //LOGGER.INFO($"============================postHeader is {header}");

                    
                    // 使用string的方式获取httpresponse
                    string postResult = FKHttp.FKHttpClient.POST(postUrl, postContentString, postHeader, 300000);
                    //LOGGER.INFO($"response is {postResult}");
                    // post response是一个Html页面
                    var doc = new HtmlDocument();
                    doc.LoadHtml(postResult);
                    /*    
                    string tmpFile = System.IO.Path.GetTempFileName();
                    string postResult = FKHttp.FKHttpClient.POST(postUrl, postContentString, postHeader, 300000, tmpFile);
                    //LOGGER.INFO($"response is {postResult}");
                    // post response是一个Html页面
                    var doc = new HtmlDocument();
                    doc.Load(tmpFile,System.Text.Encoding.UTF8);
                    */
                    // 解析post response的Html页面
                    bool bRet =  ParseAutoBillHtmlPage(doc, ref result, info);
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
                LOGGER.WARN($"Parse CCB bill html page failed. Error = {e.ToString()}");
                return false;
            }
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
                string loginUrl = FKConfig.CCBLoginUrl;
                // 此部分代码根据现有建行的页面设计
                // 如果建行个人银行网页改版，此处可能需要根据新页面做相应调整
                FKWebDriver.GetInstance.OpenUrl(loginUrl);

                // 等待出现登录界面
                var element = FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@id=\"fclogin\"]", 30000);
                if (element == null)
                {
                    result.msg = "等待登录页超时";
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_WaitElementTimeout;
                    return false;
                }

                // CCB的登录页面是iframe内嵌html，需要首先switch
                FKWebDriver.GetInstance.SwitchToFrameByID("fclogin");

                // 填充登录账号
                FKWebDriver.GetInstance.SetTextByID("USERID", info.username);

                // 填充密码
                FKWebDriver.GetInstance.SetTextByID("LOGPASS", info.password);

                // 点击登录
                FKWebDriver.GetInstance.ClickByID("loginButton");

                // 超时
                element = FKWebDriver.GetInstance.WaitUntilVisibleByXPath("//*[@id=\"pnav_V6010000\"]", 30000);
                if (element == null)
                {
                    result.msg = "等待主页超时";
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_WaitElementTimeout;
                    return false;
                }
                if (!FKWebDriver.GetInstance.ClickByID("pnav_V6020000"))
                {
                    result.msg = "点击首页出错";
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_LoginFailed;
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


        /// <summary>
        /// 强制关闭IEDriver
        /// </summary>
        private static void ForceShutdownIE()
        {
            FKCommonFunc.RunConsoleCommand("taskkill.exe", " /f /IM IEDriverServer.exe /IM iexplore.exe");
        }


        /// <summary>
        /// 获取查询POST 正文参数
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string GetAutoBillPostContent(SBillTaskInfo info, int pageNumber=1)
        {
            // 如下是获取流水必须的参数，对于不懂得参数一律直接传
            /* 
             * ACC_NO=6217004220011286774& ->卡号(一个账户下可以挂多个卡号，所以需要提供卡号)
             * ACC_SIGN =0101010& ->不懂
             * TXCODE =310200& ->不懂
             * SKEY =lL4XRK& ->这个在登录后首页隐藏的input可以获取到
             * USERID =450326199308192729& ->同上
             * ACCTYPE2 =12& ->不懂
             * PAGE=1& ->页数
             * CURRENT_PAGE=1&->页数
             * LUGANGTONG =0&->不懂
             * START_DATE =20160711&->开始日期
             * END_DATE =20170718->结束日期
            */

            // POST参数表
            Dictionary<string, string> postContent = new Dictionary<string, string>();
            // 固定参数
            postContent.Add("ACC_SIGN", "0101010");
            postContent.Add("TXCODE", "310200");
            postContent.Add("ACCTYPE2", "12");
            postContent.Add("PAGE", $"{pageNumber}");
            if (pageNumber == 1)
            {
                postContent.Add("flagnext", $"1");
            }
            else
            {
                postContent.Add("flagnext", $"4");
            }
            postContent.Add("CURRENT_PAGE", $"1");
            postContent.Add("LUGANGTONG", "0");
            // 非固定参数
            try
            {
                postContent.Add("ACC_NO", info.accountNumber);
                postContent.Add("START_DATE", info.startTime.Substring(0,8));
                postContent.Add("END_DATE", info.endTime.Substring(0, 8));
                // 如下路径是通过firebug获取，网页如果有调整此处需要相应调整
                // 否则无法获取到对应的element
                postContent.Add("SKEY", FKWebDriver.GetInstance.GetAttributeByXPath("/html/body/div[6]/div[24]/div[3]/form/input[3]", "value"));
                postContent.Add("USERID", FKWebDriver.GetInstance.GetAttributeByXPath("/html/body/div[6]/div[24]/div[3]/form/input[4]", "value"));

            }
            catch(Exception e)
            {
                LOGGER.WARN($"Parser CCB bank post data failed. Error = {e.ToString()}");
                return string.Empty;
            }

            // POST参数拼接
            return string.Join("&", postContent.Select(x => x.Key + "=" + x.Value).ToArray());
        }
        /// <summary>
        /// 获取查询POST Url路径
        /// </summary>
        /// <returns></returns>
        private string GetAutoBillPostUrl()
        {
            try
            {
                return FKWebDriver.GetInstance.GetAttributeByXPath("/html/body/div[6]/div[24]/div[2]/form", "action");
            }
            catch(Exception e)
            {
                LOGGER.WARN($"Parser CCB bank post data failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取查询POST 头数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetAutoBillPostHeader()
        {
            Dictionary<string, string> postHeader = new Dictionary<string, string>();

            postHeader.Add("Cookie", FKWebAutomatic.FKWebDriver.GetInstance.GetAllCookies());
            postHeader.Add("Content-Type", "application/x-www-form-urlencoded");
            //postHeader.Add("Content-Type", "text/html");
            postHeader.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:54.0) Gecko/20100101 Firefox/54.0");

            return postHeader;
        }


        /// <summary>
        /// 转义 货币
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        private string TransformCurrencyTypeFromString(string currency)
        {
            if (s_CCBStandardCurrency.ContainsKey(currency))
            {
                return s_CCBStandardCurrency[currency];
            }
            return $"unknown-currency [{currency}]";
        }
        /// <summary>
        /// 转义 交易类型
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        private int TransformTradeTypeFromSummary(string summary, string amount)
        {
            if (summary.Contains("管理费") || summary.Contains("收费"))
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_ToBank);
            }
            if (summary.Contains("息"))
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_FromBank);
            }
            if (summary.Contains("ATM存款") || summary.Contains("电子汇入") || summary.Contains("跨行转入")
                || summary.Contains("现金存入") || summary.Contains("转账存入"))
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_FromCustomer);
            }
            if (summary.Contains("跨行转出"))
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_ToCustomer);
            }

            try
            {
                // 已经无法识别了，依靠金额做处理
                if (amount.Contains("-"))
                {
                    return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_ToCustomer);
                }
                else
                {
                    return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_FromCustomer);
                }
            }
            catch
            {
                return (int)(SBillTaskResult.ENUM_BillTradeType.eBillTradeType_Unknown);
            }
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
                billItem.submitTime = GetTradeTime(node.SelectSingleNode("./td[2]").InnerHtml.Trim());

                if (string.IsNullOrEmpty(billItem.submitTime))
                {
                    LOGGER.ERROR("交易时间为空");
                    return false;
                }
                // 支出cny
                string outMoney = GetTradeAmount(node.SelectSingleNode("./td[3]").InnerHtml.Trim());
                // 收入cny
                string inMoney = GetTradeAmount(node.SelectSingleNode("./td[4]").InnerHtml.Trim());
                
                if (string.IsNullOrEmpty(inMoney) && string.IsNullOrEmpty(outMoney))
                {
                    LOGGER.ERROR("收入支出项同时为空");
                    return false;
                }
                if (string.IsNullOrEmpty(outMoney) && !string.IsNullOrEmpty(inMoney))
                {
                    //billItem.amount = Double.Parse(inMoney);
                    billItem.amount = (inMoney);
                }
                else if (string.IsNullOrEmpty(inMoney) && !string.IsNullOrEmpty(outMoney))
                {
                    //billItem.amount = -Double.Parse(outMoney);
                    billItem.amount = "-" + (outMoney);
                }
                else
                {
                    LOGGER.INFO($"进出账同时为空[{inMoney}][{outMoney}]");
                    return false;
                }
                // 余额cny
                string balance = GetTradeAmount(node.SelectSingleNode("./td[5]").InnerHtml.Trim());
                
                //billItem.balance = Double.Parse(balance);
                billItem.balance = (balance);
                // 对方账号
                billItem.accountNumber = GetBankCardNumber(node.SelectSingleNode("./script").InnerHtml.Trim());
                
                // 对方用户名
                billItem.accountName = GetUserName(node.SelectSingleNode("./td[6]").InnerHtml.Trim());
                
                // 币种
                string currency = GetCurrency(node.SelectSingleNode("./td[7]").InnerHtml.Trim());
                
                billItem.currency = TransformCurrencyTypeFromString(currency);

                // 摘要
                string summary = GetTradeSummary(node.SelectSingleNode("./td[8]").InnerHtml.Trim());
                billItem.digest = summary;
                // 交易类型
                billItem.tradeType = TransformTradeTypeFromSummary(summary, billItem.amount);

                // 附言
                billItem.additionalComment = GetAdditionalComment(node.SelectSingleNode("./td[9]").InnerHtml.Trim());
            }
            catch(Exception e)
            {
                LOGGER.WARN($"Parser CCB one bill failed. Error = {e.ToString()}");
                return false;
            }

            LOGGER.DEBUG($"Get one bill, info = {billItem.ToString()}");
            return true;
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
                var nodes = html.DocumentNode.SelectSingleNode("//body/form").SelectSingleNode("//table");
                var items = nodes.SelectNodes("//tr[@class='td_span']");
                // 当获取的数据为空时仍然有一个td_span 但是里面的数据为空
                if (items.Count == 1)
                {
                    if(items[0].GetAttributeValue("zcsr", "") == "|")
                    {
                        return false;
                    }
                }
                foreach (var node in nodes.SelectNodes("//tr[@class='td_span']"))
                {
                    SBillTaskResult.SBankBillInfo billItem = new SBillTaskResult.SBankBillInfo();
                    // 解析单一流水对象
                    if(ParseOneBillHtmlPage(node, ref billItem) == false)
                    {
                        continue;
                    }
                    if (ResultFilter.TimeFilter(info, billItem.submitTime))
                    {
                        result.billsList.Add(billItem);
                    }
                }

                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_Successed;
            }
            catch(Exception e)
            {
                result.msg = "CCB解析返回html错误";
                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
                LOGGER.WARN($"CCB解析返回html错误：{e.Message}");
                return false;
            }
            return true;
        }

        public static string GetCurrency(string innerHtml)
        {
            return innerHtml;
        }
        public static string GetTradeTime(string innerHtml)
        {
            var matches = Regex.Matches(innerHtml, @"\d{6,8}");
            if (matches.Count >= 2)
            {
                DateTime dateTime;
                string dateTimeStr = matches[0].Value + "-" + matches[1].Value;
                DateTime.TryParseExact(dateTimeStr, "yyyyMMdd-HHmmss", null, DateTimeStyles.None, out dateTime);

                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return "";
        }
        public static string GetTradeAmount(string innerHtml)
        {
            var matches = Regex.Matches(innerHtml, @"(?<=formatAmt\([\'""])\d+[\.\d*]*");
            if (matches.Count > 0)
            {
                return matches[0].Value;
            }
            return "";
        }
        public static string GetBankCardNumber(string innerHtml)
        {
            // 匹配字符串
            // document.write("<td width='13%'  title='"+accountProtect2('6228480415327811774')+"'>")
            string pattern = @"(?<=accountProtect2\([\'""])[^\'^""]*";
            var matches = Regex.Matches(innerHtml, pattern);
            if (matches.Count > 0)
            {
                return matches[0].Value;
            }
            return "";
        }
        public static string GetUserName(string innerHtml)
        {
            // 匹配字符串
            // document.write("<td width='13%'  title='"+accountProtect2('6228480415327811774')+"'>")
            string pattern = @"(?<=chLengthWrit\([\'""])[^\'^""]*";
            var matches = Regex.Matches(innerHtml, pattern);
            if (matches.Count > 0)
            {
                return matches[0].Value;
            }
            return "";
        }
        public static string GetAdditionalComment(string innerHtml)
        {
            // 匹配字符串
            // <div id="S" ><script language="javascript">chLengthWrit('跨行转出','18')</script></div>
            string pattern = @"(?<=chLengthWrit\([\'""])[^\'^""]*";
            var matches = Regex.Matches(innerHtml, pattern);
            if (matches.Count > 0)
            {
                return matches[0].Value;
            }
            return "";
        }
        public static string GetTradeSummary(string innerHtml)
        {
            // 匹配字符串
            // <div id="S" ><script language="javascript">chLengthWrit('跨行转出','18')</script></div>
            string pattern = @"(?<=chLengthWrit\([\'""])[^\'^""]*";
            var matches = Regex.Matches(innerHtml, pattern);
            if (matches.Count > 0)
            {
                return matches[0].Value;
            }
            return "";
        }

        #endregion ==== 核心函数 ====

        #region ==== TestCase ====

        /// <summary>
        /// 如下是测试代码
        /// 此 testcase请使用 nunit3-console.exe FKCashierBank.exe -v 执行
        /// </summary>
        [TestCase()]
        public static void testParseHtml()
        {
            string htmlPath = "test/detail.html";
            // HtmlAgilityPack的bug，需要先调用Remove 否则form无法取出
            HtmlNode.ElementsFlags.Remove("form");
            var doc = new HtmlDocument();
            doc.Load(htmlPath, System.Text.Encoding.UTF8);
            testParse(doc);
            Assert.That(CCB_AutoBill.GetHtmlItemCount(doc), Is.EqualTo(7));

        }
        public static void testParse(HtmlDocument html)
        {
            var nodes = html.DocumentNode.SelectSingleNode("//body/form").SelectSingleNode("//table");
            foreach (var node in nodes.SelectNodes("//tr[@class='td_span']"))
            {

                Console.WriteLine("-------------------------------------------------------");
                //Console.WriteLine(node.OuterHtml);
                //Console.WriteLine("-------------------------------------------------------");
                // 交易日期时间
                Console.WriteLine("交易日期时间:" + GetTradeTime(node.SelectSingleNode("./td[2]").InnerHtml.Trim()));
                // 支出cny
                Console.WriteLine("支出cny:" + GetTradeAmount(node.SelectSingleNode("./td[3]").InnerHtml.Trim()));
                // 收入cny
                Console.WriteLine("收入cny:" + GetTradeAmount(node.SelectSingleNode("./td[4]").InnerHtml.Trim()));
                // 余额cny
                Console.WriteLine("余额cny:" + GetTradeAmount(node.SelectSingleNode("./td[5]").InnerHtml.Trim()));
                // 对方账号
                //Console.WriteLine("对方账号:" + node.SelectSingleNode("./div[@id='L']").InnerHtml.Trim());
                Console.WriteLine("对方账号:" + GetBankCardNumber(node.SelectSingleNode("./script").InnerHtml.Trim()));
                // 对方用户名
                Console.WriteLine("对方用户名:" + GetUserName(node.SelectSingleNode("./td[6]").InnerHtml.Trim()));
                //Console.WriteLine("对方用户名:" + node.SelectSingleNode("./td[6]").GetAttributeValue("value", ""));
                // 币种
                Console.WriteLine("币种:" + GetCurrency(node.SelectSingleNode("./td[7]").InnerHtml.Trim()));
                // 摘要
                Console.WriteLine("摘要:" + GetTradeSummary(node.SelectSingleNode("./td[8]").InnerHtml.Trim()));
                // 交易类型描述
                Console.WriteLine("交易类型描述:" + GetAdditionalComment(node.SelectSingleNode("./td[8]").InnerHtml.Trim()));
            }
        }
        public static int GetHtmlItemCount(HtmlDocument html)
        {
            var nodes = html.DocumentNode.SelectSingleNode("//body/form").SelectSingleNode("//table");
            var count = 0;
            foreach (var node in nodes.SelectNodes("//tr[@class='td_span']"))
            {
                count++;
            }
            return count;
        }

        #endregion ==== TestCase ====
    }
}