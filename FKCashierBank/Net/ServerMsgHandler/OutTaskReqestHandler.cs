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
// Create Time         :    2017/7/18 17:35:48
// Update Time         :    2017/7/18 17:35:48
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Web.Script.Serialization;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierBank
{
    public static class OutTaskRequestHandler
    {
        /// <summary>
        /// 向服务器出账消息返回错误的值枚举
        /// </summary>
        public enum ENUM_OutTaskResponeError
        {
            eOutTaskRequest_UnknownFailed = 0,  // 出账失败，理由不明
            eOutTaskRequest_Successed = 1,      // 出账成功
            eOutTaskRequest_UnvalidMsg = 2,     // 出账消息解析不合法
            eOutTaskRequest_UnvalidTaskId = 3,  // 出账任务ID不合法
        };
        public static int OnMsg(int nNodeID, string strRequestString, MainForm mainForm, ref string strRespone)
        {
            int nErrorCode = (int)ENUM_OutTaskResponeError.eOutTaskRequest_UnknownFailed;

            // 检查请求合法性
            if(!IsValidOutTaskMsg(strRequestString, RSAKeyContainer.GetInstance.GetJavaPublicKey()))
            {
                // 消息检查未通过
                nErrorCode = (int)ENUM_OutTaskResponeError.eOutTaskRequest_UnvalidMsg;
                LOGGER.WARN($"Server out task request is not a invalid msg. \n Request = {strRequestString}");
            }
            else
            {
                SOutTaskInfo info = ParseStructFromRequest(strRequestString, RSAKeyContainer.GetInstance.GetCSharpPrivateKey());
                if(info.taskID <= 0)
                {
                    // 消息不正常
                    nErrorCode = (int)ENUM_OutTaskResponeError.eOutTaskRequest_UnvalidTaskId;
                    LOGGER.WARN($"Server out task request is not a invalid msg. \n Request = {strRequestString} \n TaskInfo = {info.ToLogString()}");
                }
                else
                {
                    // 消息解析成功，开始逻辑处理
                    {
                        // 实际处理
                        mainForm.AddOutTaskToWaitQueue(info);
                        nErrorCode = (int)ENUM_OutTaskResponeError.eOutTaskRequest_Successed;
                    }
                }
            }

            strRespone = ("{\"nodeID\":") + nNodeID + (",\"status\":") + nErrorCode + ("}");
            return nErrorCode;
        }

        /// <summary>
        /// 检查消息有效性
        /// </summary>
        /// <param name="strMsg"></param>
        /// <param name="strRSAPublicKey"></param>
        /// <returns></returns>
        private static bool IsValidOutTaskMsg(string strMsg, string strRSAPublicKey)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            S2CMessage_OutTaskRequest result = new S2CMessage_OutTaskRequest();
            dynamic data;
            try
            {
                data = js.Deserialize<dynamic>(strMsg);   // 反序列化
            }
            catch(Exception e)
            {
                LOGGER.WARN($"Server out task request is not a invalid JSON format. \n Error = {e.ToString()} \n Request = {strMsg}");
                return false;
            }

            try
            {
                result.info.taskID = int.Parse(data["taskID"]);
                result.info.nodeID = int.Parse(data["nodeID"]);
                result.info.fromBankCode = data["fromBankCode"];
                result.info.fromBankName = data["fromBankName"];
                result.info.fromAccount = data["fromAccount"];
                result.info.password = data["password"];
                result.info.tradePassword = data["tradePassword"];
                result.info.toBankCode = data["toBankCode"];
                result.info.toBankName = data["toBankName"];
                result.info.toAccount = data["toAccount"];
                result.info.holderName = data["holderName"];
                result.info.amount = data["amount"];
                result.info.remarks = data["remarks"];
                result.timestamp = long.Parse(data["timestamp"]);
                result.signature = data["signature"];
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Parse server out task request failed. \n Error = {e.ToString()} \n Request = {strMsg}");
                return false;
            }

            string strGetSrc = result.ToGetParamsString();
            string strHashData = "";
            FKBaseUtils.FKHash.GetHash(strGetSrc, ref strHashData);
            string strCSharpPublicKey = FKBaseUtils.FKRSAEncrypt.ConvertRSAPublicKey_Java2DotNet(strRSAPublicKey);
            bool bCheckSignSuccessed = FKBaseUtils.FKRSASignature.IsValidRSASign(strCSharpPublicKey, strHashData, result.signature);
            if (!bCheckSignSuccessed)
            {
                LOGGER.WARN($"Server out task request signature check failed.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 解析一个出账请求消息，组装得到一个内部出账任务结构
        /// </summary>
        /// <param name="strRequestString"></param>
        /// <param name="strCSharpPrivateKey"></param>
        /// <returns></returns>
        private static SOutTaskInfo ParseStructFromRequest(string strRequestString, string strCSharpPrivateKey)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            S2CMessage_OutTaskRequest result = new S2CMessage_OutTaskRequest();
            dynamic data;
            try
            {
                data = js.Deserialize<dynamic>(strRequestString);   // 反序列化
            }
            catch(Exception e)
            {
                LOGGER.WARN($"Server out task request is not a invalid JSON format. \n Error = {e.ToString()} \n Request = {strRequestString}");
                return result.info;
            }

            string strLoginPassword = "";
            string strTradePassword = "";
            try
            {
                result.info.taskID = int.Parse(data["taskID"]);
                result.info.nodeID = int.Parse(data["nodeID"]);
                result.info.fromBankCode = data["fromBankCode"];
                result.info.fromBankName = data["fromBankName"];
                result.info.fromAccount = data["fromAccount"];
                strLoginPassword = data["password"];
                strTradePassword = data["tradePassword"];
                result.info.toBankCode = data["toBankCode"];
                result.info.toBankName = data["toBankName"];
                result.info.toAccount = data["toAccount"];
                result.info.holderName = data["holderName"];
                result.info.amount = data["amount"];
                result.info.remarks = data["remarks"];
            }
            catch(Exception e)
            {
                LOGGER.WARN($"Parse server out task request failed. \n Error = {e.ToString()} \n Request = {strRequestString}");
                result.info.taskID = 0;     // 注意清零，外面靠该值判断是否解析成功的
                return result.info;
            }

            try
            {
                // 根据约定进行RSA解密密码
                string xmlKey = FKBaseUtils.FKRSAEncrypt.ConvertRSAPrivateKey_Java2DotNet(strCSharpPrivateKey);
                result.info.password = FKBaseUtils.FKRSAEncrypt.RSADecryptByDotNetPrivateKey(strLoginPassword, xmlKey);
                if (string.IsNullOrEmpty(result.info.password))
                {
                    LOGGER.WARN($"Decrypt password failed. Password = {strLoginPassword}");
                    result.info.taskID = 0;
                    return result.info;
                }
                result.info.tradePassword = FKBaseUtils.FKRSAEncrypt.RSADecryptByDotNetPrivateKey(strTradePassword, xmlKey);
                if (string.IsNullOrEmpty(result.info.tradePassword))
                {
                    LOGGER.WARN($"Decrypt trade password failed. Password = {strTradePassword}");
                    result.info.taskID = 0;
                    return result.info;
                }
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Decrypt passwords failed. \n Error = {e.ToString()} \n Request = {strRequestString}");
                result.info.taskID = 0; 
                return result.info;
            }

            // 一切正常
            return result.info;
        }
    }
}