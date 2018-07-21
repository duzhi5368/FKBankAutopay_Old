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
// Create Time         :    2017/7/17 15:51:29
// Update Time         :    2017/7/17 15:51:29
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Web.Script.Serialization;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierBank
{
    internal static class RequestSender
    {
        /// <summary>
        /// 发送Http请求给服务器
        /// </summary>
        /// <param name="requestStruct"></param>
        /// <returns></returns>
        public static HttpSendResult SendRequestToServer(C2SStruct requestStruct)
        {
            int nTimeout = FKConfig.ConnectServerTimeout;                                   // 连接服务器的超时时间
            string strSendUrl = string.Empty;                                               // 发送的Url
            string strCSharpPrivateKey = RSAKeyContainer.GetInstance.GetCSharpPrivateKey(); // C# RSA私钥
            string strSendString = string.Empty;                                            // 需要发送的字符串
            string strMsgType = string.Empty;
            if(requestStruct is SOutTaskResult)
            {
                C2SMessage_OutTaskResult msg = new C2SMessage_OutTaskResult();
                msg.info = (SOutTaskResult)requestStruct;
                strSendString = msg.ToPostDataString(strCSharpPrivateKey);
                strMsgType = msg.GetType().ToString();
                strSendUrl = FKConfig.ServerReviceOutcomeResultUrl;
            }
            else if(requestStruct is SRegisteNode)
            {
                C2SMessage_RequestNode msg = new C2SMessage_RequestNode();
                msg.info = (SRegisteNode)requestStruct;
                strSendString = msg.ToPostDataString(strCSharpPrivateKey);
                strMsgType = msg.GetType().ToString();
                strSendUrl = FKConfig.ServerRegisteUrl;
            }
            else if(requestStruct is SHeartbeat)
            {
                C2SMessage_Heartbeat msg = new C2SMessage_Heartbeat();
                msg.info = (SHeartbeat)requestStruct;
                strSendString = msg.ToPostDataString(strCSharpPrivateKey);
                strMsgType = msg.GetType().ToString();
                strSendUrl = FKConfig.ServerHeartbeatUrl;
            }
            else if(requestStruct is SUnregisteNode)
            {
                C2SMessage_UnregisteNode msg = new C2SMessage_UnregisteNode();
                msg.info = (SUnregisteNode)requestStruct;
                strSendString = msg.ToPostDataString(strCSharpPrivateKey);
                strMsgType = msg.GetType().ToString();
                strSendUrl = FKConfig.ServerLogoutUrl;
            }
            else if (requestStruct is SBillTaskResult)
            {
                C2SMessage_GetBillResult msg = new C2SMessage_GetBillResult();
                msg.info = (SBillTaskResult)requestStruct;
                strSendString = msg.ToPostDataString(strCSharpPrivateKey);
                strMsgType = msg.GetType().ToString();
                strSendUrl = FKConfig.ServerReviceGetBillResultUrl;
            }
            else
            {
                LOGGER.WARN($"Can't send unknown request type.");
                return null;
            }

            return SendMsgToServer(strSendUrl, strSendString, strMsgType, nTimeout);
        }

        /// <summary>
        /// 实际消息发送
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strInfo"></param>
        /// <param name="strMsgInfo"></param>
        /// <param name="nTimeOut"></param>
        /// <returns></returns>
        private static HttpSendResult SendMsgToServer(string strUrl, string strInfo, string strMsgInfo, int nTimeOut)
        {
            try
            {
                HttpSendResult result = new HttpSendResult();
                string strResponse = (string)FKHttp.FKHttpClient.POST(strUrl, strInfo, null, nTimeOut);

                bool bSuccess = false;
                string strErrorMsg = "";
                int nID = 0;
                string strNewResponse = "";
                int nStatus = 0;
                string strJavaPublicKey = "";

                // 解析Respone
                ParserResponseMsg(strResponse, out strNewResponse, out bSuccess, out strErrorMsg, out nID, out nStatus, out strJavaPublicKey);

                result.RequestType = strMsgInfo;
                result.ServerUrl = strUrl;
                result.RequestMsg = strInfo;
                result.ResponseMsg = strNewResponse;
                result.bSuccess = bSuccess;
                result.ErrorMsg = strErrorMsg;
                result.NodeID = nID;
                result.ErrorStatus = nStatus;
                result.publicKey = strJavaPublicKey;

                if(bSuccess)
                    // 记录日志
                    LOGGER.DEBUG($"Send http msg type = {strMsgInfo}: \n-\nRequest = {strInfo} \n-\nResponse = {strNewResponse}");
                else
                    LOGGER.ERROR($"Send http msg type = {strMsgInfo}: \n-\nRequest = {strInfo} \n-\nResponse = {strNewResponse} \n-\nStatus = {nStatus} , Error = {strErrorMsg}");

                return result;
            }
            catch(Exception e)
            {
                LOGGER.ERROR($"Send http request msg to server failed. Error = {e.ToString()}");
                return null;
            }
        }

        /// <summary>
        /// 解析服务器响应消息
        /// </summary>
        /// <param name="strResponse"></param>
        /// <param name="strNewResponse"></param>
        /// <param name="bSuccessed"></param>
        /// <param name="strErrorMsg"></param>
        /// <param name="nID"></param>
        /// <param name="nStatus"></param>
        /// <param name="strJavaPublicKey"></param>
        private static void ParserResponseMsg(string strResponse, out string strNewResponse,
            out bool bSuccessed, out string strErrorMsg, out int nID, out int nStatus, out string strJavaPublicKey)
        {
            strJavaPublicKey = "";
            strNewResponse = strResponse;
            if (string.IsNullOrEmpty(strResponse))
            {
                bSuccessed = false;
                strErrorMsg = "未收到服务器有效返回消息，可能服务器无法连接或超时...";
                nID = -1;
                nStatus = 0;
                return;
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            dynamic data;
            try
            {
                data = js.Deserialize<dynamic>(strResponse);   // 反序列化
            }
            catch
            {
                bSuccessed = false;
                nID = -1;
                nStatus = 0;

                if (strResponse.IndexOf("[Error]") != -1)
                {
                    strErrorMsg = strResponse;
                    strNewResponse = "";
                }
                else
                {
                    strErrorMsg = "返回Respone不是有效的JSON格式...";
                }
                return;
            }


            int nNodeID = -1;
            int nRevStatus = 0;
            try
            {
                nNodeID = data["id"];
                nRevStatus = data["status"];
            }
            catch
            {
                bSuccessed = false;
                strErrorMsg = "返回Respone内格式非法...必须提供 id 和 status 字段...";
                nID = 0;
                nStatus = 0;
                return;
            }

            try
            {
                strJavaPublicKey = data["publicKey"];
            }
            catch
            {
                // 不处理，没有也是正常的
            }

            // 逻辑检查服务器返回值
            if(nRevStatus == 1)
            {
                bSuccessed = true;
            }
            else
            {
                bSuccessed = false;
            }
            strErrorMsg = ParserErrorID(nRevStatus);
            nID = nNodeID;
            nStatus = nRevStatus;
        }

        /// <summary>
        /// 解析服务器返回的错误码
        /// </summary>
        /// <param name="nID"></param>
        /// <returns></returns>
        private static string ParserErrorID(int nID)
        {
            switch (nID)
            {
                case 1:
                    return "Successed";
                case 2:
                    return "Unknown error occured in server.";
                case 3:
                    return "Node is already existed in server.";
                case 4:
                    return "Node is not existed in server.";
                case 5:
                    return "Signature check failed.";
                case 11:
                    return "Unknown back card info in server.";
                case 41:
                    return "Node is not active in server.";
            }
            return nID.ToString();
        }
    }
}