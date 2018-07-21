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
// Create Time         :    2017/7/17 15:17:11
// Update Time         :    2017/7/17 15:17:11
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Collections.Generic;
// ===============================================================================
namespace FKCashierBank
{
    /// <summary>
    /// 通知银行出账返回消息的结构体 C->S
    /// </summary>
    internal class SOutTaskResult : C2SStruct
    {
        public enum ENUM_OutcomeActionStatus
        {
            eOutcomeActionStatus_UnknownError = 0,          // 未知错误
            eOutcomeActionStatus_Successed = 1,             // 出账成功
            eOutcomeActionStatus_CreateIEDriverFailed = 2,  // 创建IEDriver驱动失败
            eOutcomeActionStatus_OpenUrlFailed = 3,         // 打开Url页面失败
            eOutcomeActionStatus_AutoProcessFailed = 4,     // 自动处理流程中失败
            eOutcomeActionStatus_UnsupportBankCode = 5,     // 暂不支持的银行类型
        };
        public int nodeID { get; set; }
        public int status { get; set; }
        public int taskID { get; set; }
        public string tradeNo { get; set; }
        public string dianZiHuiDan { get; set; }
        public string moneyBeforeTrade { get; set; }
        public string moneyAfterTrade { get; set; }
        public string transactionTime { get; set; }
        public string msg { get; set; }

        public override string ToLogString()
        {
            string strRet = "---------------------------\n";
            try
            {
                strRet += ("节点编号：" + nodeID + "\n");
                strRet += ("出账结果：" + status + "\n");
                strRet += ("出账流水：" + taskID + "\n");
                strRet += ("出账时间：" + transactionTime + "\n");
                strRet += ("银行流水：" + FKBaseUtils.FKStringHelper.MaskString(tradeNo, false) + "\n");
                strRet += ("电子回单：" + FKBaseUtils.FKStringHelper.MaskString(dianZiHuiDan, false) + "\n");
                strRet += ("出账前金额：" + FKBaseUtils.FKStringHelper.MaskString(moneyBeforeTrade, false) + "\n");
                strRet += ("出账后金额：" + FKBaseUtils.FKStringHelper.MaskString(moneyAfterTrade, false) + "\n");
                strRet += ("错误信息：" + msg + "\n");
            }
            catch { }
            finally
            {
                strRet += "---------------------------";
            }
            return strRet;
        }

        public override string ToSimpleString()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 向服务器注册节点消息 C->S
    /// </summary>
    internal class SRegisteNode : C2SStruct
    {
        public string ip { get; set; }
        public string bankCardNo { get; set; }
        public string publicKey { get; set; }

        public override string ToLogString()
        {
            throw new NotImplementedException();
        }

        public override string ToSimpleString()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 心跳消息 C->S
    /// </summary>
    internal class SHeartbeat : C2SStruct
    {
        public int id { get; set; }
        public int taskID { get; set; }

        public override string ToLogString()
        {
            string strRet = "";
            try
            {
                strRet += ("NodeID：" + id + " ");
                strRet += ("TaskID：" + taskID + " ");
            }
            catch { }
            return strRet;
        }

        public override string ToSimpleString()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 注销节点 C->S
    /// </summary>
    internal class SUnregisteNode : C2SStruct
    {
        public int id { get; set; }

        public override string ToLogString()
        {
            string strRet = "";
            try
            {
                strRet += ("NodeID：" + id + " ");
            }
            catch { }
            return strRet;
        }

        public override string ToSimpleString()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 通知银行流水信息的结构体 C->S
    /// </summary>
    internal class SBillTaskResult : C2SStruct
    {
        public class SBankBillInfo
        {
            public SBankBillInfo()
            {
                currency = "CNY";
            }

            public string serialNo { get; set; }            // GUID
            public string submitTime { get; set; }          // 交易流水时间
            public string amount { get; set; }              // 资金更变值：若为收入，则可能例如 863.23 或 1082.00 或 0.00；若是支出，则是 -98.02 或 -1000.00
            public string accountNumber { get; set; }       // 对方账号
            public string accountName { get; set; }         // 对方用户名
            public string additionalComment { get; set; }   // 附言
            public int tradeType { get; set; }              // 交易类型
            public string digest { get; set;}               // 摘要（字符串类型，几乎相当于交易类型）
            public string balance { get; set; }             // 余额
            public string currency { get; set; }            // 币种，CNY USD GBP

            // ----------------- for abc -------------------
            public string accountBankName { get; set; }     // 交易银行
            public string tradeChannel { get; set; }        // 交易渠道
            public string tradeUsage { get; set; }          // 交易用途
            // ----------------- for abc -------------------
            public override string ToString()
            {
                string strRet = "---------------------------\n";
                try
                {
                    strRet += ("流水编号：" + serialNo + "\n");
                    strRet += ("交易时间：" + submitTime + "\n");
                    strRet += ("对方账户：" + FKBaseUtils.FKStringHelper.MaskString(accountName,false) + "\n");
                    strRet += ("对方卡号：" + FKBaseUtils.FKStringHelper.MaskString(accountNumber, false) + "\n");
                    strRet += ("附言：" + FKBaseUtils.FKStringHelper.MaskString(additionalComment, false) + "\n");
                    strRet += ("交易银行：" + accountBankName + "\n");
                    strRet += ("交易渠道：" + tradeChannel + "\n");
                    strRet += ("交易类型：" + tradeType + "\n");
                    strRet += ("摘要：" + digest + "\n");
                    strRet += ("交易用途：" + tradeUsage + "\n");
                    strRet += ("交易金额：" + amount + "\n");
                    strRet += ("余额：" + balance + "\n");
                    strRet += ("币种：" + currency + "\n");
                }
                catch { }
                finally
                {
                    strRet += "---------------------------";
                }
                return strRet;
            }

        }
        public enum ENUM_BillActionStatus
        {
            eBillActionStatus_UnknownError = 0,          // 未知错误
            eBillActionStatus_Successed = 1,             // 出账成功
            eBillActionStatus_CreateIEDriverFailed = 2,  // 创建IEDriver驱动失败
            eBillActionStatus_AutoProcessFailed = 3,     // 自动处理流程中失败
            eBillActionStatus_UnsupportBankCode = 4,     // 暂不支持的银行类型
            eBillActionStatus_LoginFailed  = 5,          // 登录失败
            eBillActionStatus_WaitElementTimeout = 6,    // 等待页面元素超时
            eBillActionStatus_ArgumentInvalid = 7,       // 用户名/密码/卡号不合法
        };

        public enum ENUM_BillTradeType
        {
            /*
            // ----------------- for ccb -------------------
            eBillTradeType_TransferDeposit = 0,             // 转账存入
            eBillTradeType_InterBankWithdraw = 1,           // 跨行转出
            eBillTradeType_AccountManagementFee = 2,        // 账户管理费
            eBillTradeType_ElectronicDeposit = 3,           // 电子汇入
            eBillTradeType_Fee = 4,                         // 收费
            // ----------------- for ccb -------------------

            // ----------------- for abc -------------------
            eBillTradeType_Transfer = 5,                    // 转账
            // ----------------- for abc -------------------


            eBillTradeType_Unknown,                        // 未知
            */

            eBillTradeType_Unknown  = 0,                // 未知（客户端表示已经无法识别的类型）
            eBillTradeType_FromBank = 1,                // 从银行来的钱（例如利息等）
            eBillTradeType_ToBank = 2,                  // 给银行的钱（例如银行税费，包括年费，利息税，管理费等）
            eBillTradeType_FromCustomer = 3,            // 从某个客户来的钱（方式不明，包括跨行转账，电子转账，同行转账，支付宝等等）
            eBillTradeType_ToCustomer = 4,              // 给某个客户出的钱（方式不明，只要是出钱给某个客户就算）
        };
        public int nodeID { get; set; }
        public int status { get; set; }
        public int taskID { get; set; }
        public string msg { get; set; }
        public string bankId { get; set; }              // 服务器发送的ID（不要做任何检测和处理，返回给服务器即可）
        public List<SBankBillInfo> billsList { get; set; }   // 流水详情

        public SBillTaskResult()
        {
            nodeID = status = taskID = 0;
            msg = string.Empty;
            billsList = new List<SBankBillInfo>();
        }
        public override string ToLogString()
        {
            string strRet = "---------------------------\n";
            try
            {
                strRet += ("节点编号：" + nodeID + "\n");
                strRet += ("查账结果：" + status + "\n");
                strRet += ("任务编号：" + taskID + "\n");
                strRet += ("错误信息：" + msg + "\n");
            }
            catch { }
            finally
            {
                strRet += "---------------------------";
            }
            return strRet;
        }

        public override string ToSimpleString()
        {
            throw new NotImplementedException();
        }
    }
}