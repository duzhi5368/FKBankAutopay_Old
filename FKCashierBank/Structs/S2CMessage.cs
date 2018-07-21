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
// Create Time         :    2017/7/18 17:46:51
// Update Time         :    2017/7/18 17:46:51
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
// ===============================================================================
namespace FKCashierBank
{
    internal class S2CMessage_OutTaskRequest : S2C_MessageBase
    {
        public SOutTaskInfo info { get; set; }
        public int nMsgID = (int)ENUM_S2CMsgID.eS2CMsgID_OutTaskRequest;

        public S2CMessage_OutTaskRequest()
        {
            timestamp = 0;
            signature = "";
            info = new SOutTaskInfo();
        }
        public override string ToGetParamsString()
        {
            string strRet = "";
            try
            {
                strRet += "amount=";
                strRet += info.amount;
                strRet += "&fromAccount=";
                strRet += info.fromAccount;
                strRet += "&fromBankCode=";
                strRet += info.fromBankCode;
                strRet += "&fromBankName=";
                strRet += info.fromBankName;
                strRet += "&holderName=";
                strRet += info.holderName;
                strRet += "&msgID=";
                strRet += nMsgID;
                strRet += "&nodeID=";
                strRet += info.nodeID;
                strRet += "&password=";
                strRet += info.password;
                strRet += "&remarks=";
                strRet += info.remarks;
                strRet += "&taskID=";
                strRet += info.taskID;
                strRet += "&timestamp=";
                strRet += timestamp;
                strRet += "&toAccount=";
                strRet += info.toAccount;
                strRet += "&toBankCode=";
                strRet += info.toBankCode;
                strRet += "&toBankName=";
                strRet += info.toBankName;
                strRet += "&tradePassword=";
                strRet += info.tradePassword;
            }
            catch { }
            return strRet;
        }
    }

    internal class S2CMessage_GetTaskLogRequest : S2C_MessageBase
    {
        public SGetTaskLogInfo info { get; set; }
        public int nMsgID = (int)ENUM_S2CMsgID.eS2CMsgID_GetTaskLogRequest;

        public S2CMessage_GetTaskLogRequest()
        {
            timestamp = 0;
            signature = "";
            info = new SGetTaskLogInfo();
        }
        public override string ToGetParamsString()
        {
            string strRet = "";
            try
            {
                strRet += "msgID=";
                strRet += nMsgID;
                strRet += "&taskID=";
                strRet += info.taskID;
                strRet += "&timestamp=";
                strRet += timestamp;
            }
            catch { }
            return strRet;
        }
    }

    internal class S2CMessage_GetBillRequest : S2C_MessageBase
    {
        public SBillTaskInfo info { get; set; }
        public int nMsgID = (int)ENUM_S2CMsgID.eS2CMsgID_GetBillTaskRequest;

        public S2CMessage_GetBillRequest()
        {
            timestamp = 0;
            signature = "";
            info = new SBillTaskInfo();
        }
        public override string ToGetParamsString()
        {
            string strRet = "";
            try
            {
                strRet += "accountNumber=";
                strRet += info.accountNumber;
                strRet += "&bankCode=";
                strRet += info.bankCode;
                strRet += "&bankId=";
                strRet += info.bankId;
                strRet += "&bankName=";
                strRet += info.bankName;
                strRet += "&endTime=";
                strRet += info.endTime;
                strRet += "&msgID=";
                strRet += nMsgID;
                strRet += "&password=";
                strRet += info.password;
                strRet += "&startTime=";
                strRet += info.startTime;
                strRet += "&taskID=";
                strRet += info.taskID;
                strRet += "&timestamp=";
                strRet += timestamp;
                strRet += "&uKeyPassword=";
                strRet += info.uKeyPassword;
                strRet += "&username=";
                strRet += info.username;
            }
            catch { }
            return strRet;
        }
    }
}