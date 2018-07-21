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
// Create Time         :    2017/7/17 14:47:50
// Update Time         :    2017/7/17 14:47:50
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
// ===============================================================================
namespace FKCashierBank
{
    internal enum ENUM_S2CMsgID
    {
        eS2CMsgID_OutTaskRequest = 1,           // 请求出账消息
        eS2CMsgID_GetTaskLogRequest = 2,        // 请求获取客户端Log
        eS2CMsgID_ManageAction = 3,             // 服务器管理命令
        eS2CMsgID_GetBillTaskRequest = 6,       // 服务器请求查询流水消息

        eS2CMsgID_Max,
    }
    /// <summary>
    /// 银行出账消息结构
    /// </summary>
    internal class SOutTaskInfo : S2CStruct
    {
        public int taskID { get; set; }
        public int nodeID { get; set; }
        public string fromBankCode { get; set; }
        public string fromBankName { get; set; }
        public string fromAccount { get; set; }
        public string password { get; set; }
        public string tradePassword { get; set; }
        public string toBankCode { get; set; }
        public string toBankName { get; set; }
        public string toAccount { get; set; }
        public string holderName { get; set; }
        public string amount { get; set; }
        public string remarks { get; set; }

        public string testScriptFile { get; set; }

        public SOutTaskInfo()
        {
            taskID = 0;
            nodeID = 0;
            fromBankCode = "";
            fromBankName = "";
            fromAccount = "";
            password = "";
            tradePassword = "";
            toBankCode = "";
            toBankName = "";
            toAccount = "";
            holderName = "";
            amount = "";
            remarks = "";
            testScriptFile = "";
        }

        public override string ToLogString()
        {
            string strRet = "---------------------------\n";
            try
            {
                if (!string.IsNullOrEmpty(testScriptFile))
                {
                    strRet += ("测试脚本：" + testScriptFile + "\n");
                }
                strRet += ("出账流水：" + taskID + "\n");
                strRet += ("出账银行：" + fromBankName + " (" + fromBankCode + ")\n");
                strRet += ("出账卡号：" + FKBaseUtils.FKStringHelper.MaskString(fromAccount) + "\n");
                strRet += ("登陆密码：" + FKBaseUtils.FKStringHelper.MaskString(password) + "\n");
                strRet += ("交易密码：" + FKBaseUtils.FKStringHelper.MaskString(tradePassword) + "\n");
                strRet += ("收款银行：" + toBankName + " (" + toBankCode + ")\n");
                strRet += ("收款卡号：" + FKBaseUtils.FKStringHelper.MaskString(toAccount) + "\n");
                strRet += ("收款人名：" + FKBaseUtils.FKStringHelper.MaskString(holderName) + "\n");
                strRet += ("汇款附言：" + FKBaseUtils.FKStringHelper.MaskString(remarks) + "\n");
                strRet += ("汇款金额：" + amount + "\n");
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
            string strRet = "流水编号：" + taskID;
            strRet += " 出款信息：" + fromBankName;
            strRet += " ";
            strRet += fromAccount.Length > 3 ? fromAccount.Substring(0, fromAccount.Length - 3) + "***" : fromAccount;
            strRet += " 收款信息：" + toBankName;
            strRet += " ";
            strRet += toAccount.Length > 3 ? toAccount.Substring(0, toAccount.Length - 3) + "***" : toAccount;
            strRet += " ";
            strRet += holderName.Length > 1 ? holderName.Substring(0, holderName.Length - 1) + "*" : toAccount;
            strRet += " 金额：";
            strRet += amount;
            return strRet;
        }
    }

    /// <summary>
    /// 查询日志消息结构
    /// </summary>
    internal class SGetTaskLogInfo : S2CStruct
    {
        public int taskID { get; set; }

        public SGetTaskLogInfo()
        {
            taskID = 0;
        }
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
    /// 银行流水查询消息结构
    /// </summary>
    internal class SBillTaskInfo : S2CStruct, ICloneable
    {
        public int taskID { get; set; }
        public string bankCode { get; set; }        // 银行名称，例如CMBC, CCB, BCM, CMB等，参见GetBillBankTypeIDByName
        public string bankName { get; set; }        // 银行中文名称：“民生银行”“中国银行”等，该值来自服务器，客户端不应当使用
        public string bankId { get; set; }          // 银行号码（服务器发送过来，客户端不做任何处理直接返回）
        // 不同于卡号，这是账号，建行一个账号可以绑定多张卡
        public string username { get; set; }
        // 卡号
        public string accountNumber { get; set; }
        public string password { get; set; }
        public string uKeyPassword { get; set; }

        // 查询开始日期
        public string startTime { get; set; }
        // 查询结束日期
        public string endTime { get; set; }

        public SBillTaskInfo()
        {
            taskID = 0;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public override string ToLogString()
        {
            string strRet = "---------------------------\n";
            try
            {
                strRet += ("出账流水ID：" + taskID + "\n");
                strRet += ("银行账号：" + FKBaseUtils.FKStringHelper.MaskString(username) + "(" + bankCode + ")\n");
                strRet += ("银行卡号：" + FKBaseUtils.FKStringHelper.MaskString(accountNumber) + "\n");
                strRet += ("登陆密码：" + FKBaseUtils.FKStringHelper.MaskString(password) + "\n");
                strRet += ("查询流水开始日期：" + startTime + "\n");
                strRet += ("查询流水截止日期：" + endTime + "\n");
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