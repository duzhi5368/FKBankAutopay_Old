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
// Create Time         :    2017/7/18 17:39:04
// Update Time         :    2017/7/18 17:39:04
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierBank
{
    public static class GetTaskLogRequestHandler
    {
        /// <summary>
        /// 向服务器查询日志返回错误的值枚举
        /// </summary>
        public enum ENUM_GetTaskLogResponseError
        {
            eGetTaskLogResponseError_UnknownFailed = 0,       // 查询失败，理由不明
            eGetTaskLogResponseError_Successed = 1,           // 获取成功
            eGetTaskLogResponseError_UnvalidMsg = 2,          // 日志消息解析不合法
            eGetTaskLogResponseError_UnvalidTaskId = 3,       // 日志任务ID不合法
            eGetTaskLogResponseError_DBLogError = 4,          // 日志数据库有错误
            eGetTaskLogResponseError_CannotFindTaskDB = 5,    // 找不到该日志所在的Log日期表
            eGetTaskLogResponseError_CannotFindLogDB = 6,     // 找不到该日志所在DB文件
        };
        public static int OnMsg(int nNodeID, string strRequestString, MainForm mainForm, ref string strRespone)
        {
            int nErrorCode = (int)ENUM_GetTaskLogResponseError.eGetTaskLogResponseError_UnknownFailed;
            // 检查请求合法性
            if (!IsValidGetTaskLogMsg(strRequestString, RSAKeyContainer.GetInstance.GetJavaPublicKey()))
            {
                // 消息检查未通过
                nErrorCode = (int)ENUM_GetTaskLogResponseError.eGetTaskLogResponseError_UnvalidMsg;
                LOGGER.WARN($"Server get task log request is not a invalid msg. \n Request = {strRequestString}");
            }
            else
            {
                SGetTaskLogInfo info = ParseStructFromRequest(strRequestString, RSAKeyContainer.GetInstance.GetCSharpPrivateKey());
                if (info.taskID <= 0)
                {
                    // 消息不正常
                    nErrorCode = (int)ENUM_GetTaskLogResponseError.eGetTaskLogResponseError_UnvalidTaskId;
                    LOGGER.WARN($"Server get task log request is not a invalid msg. \n Request = {strRequestString} \n TaskInfo = {info.ToLogString()}");
                }
                else
                {
                    // 消息解析成功，开始逻辑处理
                    {
                        List<string> strTaskLogFile = FKLog.FKSQLiteLogMgr.GetInstance.GetTaskLogDBFileName(info.taskID);
                        if(strTaskLogFile.Count <= 0)
                        {
                            // 找不到对应的日志文件编号
                            LOGGER.WARN($"Can't find task log date. TaskID = {info.taskID}");
                            nErrorCode = (int)ENUM_GetTaskLogResponseError.eGetTaskLogResponseError_CannotFindTaskDB;
                            strRespone = ("{\"detail\":[],\"logCount\":0,\"nodeID\":") + nNodeID + (",\"taskID\":") + info.taskID + (",\"status\":") + nErrorCode + ("}");
                            return nErrorCode;
                        }
                        bool bIsAllFileExist = true;
                        string strLostFile = string.Empty;
                        for(int i = 0; i < strTaskLogFile.Count; ++i)
                        {
                            if (!File.Exists(strTaskLogFile[i]))
                            {
                                strLostFile = strTaskLogFile[i];
                                bIsAllFileExist = false;
                                break;
                            }
                        }
                        if (!bIsAllFileExist)
                        {
                            // 对应的日志文件不存在
                            LOGGER.WARN($"Log file is not exist. TaskID = {info.taskID}, Lost file = {strLostFile}");
                            nErrorCode = (int)ENUM_GetTaskLogResponseError.eGetTaskLogResponseError_CannotFindLogDB;
                            strRespone = ("{\"detail\":[],\"logCount\":0,\"nodeID\":") + nNodeID + (",\"taskID\":") + info.taskID + (",\"status\":") + nErrorCode + ("}");
                            return nErrorCode;
                        }

                        string strDetails = LOGGER.GetLogByTaskID(info.taskID, strTaskLogFile[0]);
                        int nDetailsNum = LOGGER.GetLogNumByTaskID(info.taskID, strTaskLogFile[0]);
                        nErrorCode = (int)ENUM_GetTaskLogResponseError.eGetTaskLogResponseError_Successed;
                        strRespone = ("{") + strDetails + (",\"logCount\":") + nDetailsNum + (",\"nodeID\":") 
                            + nNodeID + (",\"taskID\":") + info.taskID + (",\"status\":") + nErrorCode + ("}");
                        return nErrorCode;

                    }
                }
            }

            strRespone = ("{\"detail\":[],\"logCount\":0,\"nodeID\":") + nNodeID + (",\"taskID\":0,\"status\":") + nErrorCode + ("}");
            return nErrorCode;
        }

        /// <summary>
        /// 检查消息有效性
        /// </summary>
        /// <param name="strMsg"></param>
        /// <param name="strRSAPublicKey"></param>
        /// <returns></returns>
        private static bool IsValidGetTaskLogMsg(string strMsg, string strRSAPublicKey)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            S2CMessage_GetTaskLogRequest request = new S2CMessage_GetTaskLogRequest();
            dynamic data;
            try
            {
                data = js.Deserialize<dynamic>(strMsg);   // 反序列化
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Server get task log request is not a invalid JSON format. \n Error = {e.ToString()} \n Request = {strMsg}");
                return false;
            }

            try
            {
                request.info.taskID = int.Parse(data["taskID"]);
                request.timestamp = long.Parse(data["timestamp"]);
                request.signature = data["signature"];
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Parse server get task log request failed. \n Error = {e.ToString()} \n Request = {strMsg}");
                return false;
            }

            string strGetSrc = request.ToGetParamsString();
            string strHashData = "";
            FKBaseUtils.FKHash.GetHash(strGetSrc, ref strHashData);
            string strCSharpPublicKey = FKBaseUtils.FKRSAEncrypt.ConvertRSAPublicKey_Java2DotNet(strRSAPublicKey);
            bool bCheckSignSuccessed = FKBaseUtils.FKRSASignature.IsValidRSASign(strCSharpPublicKey, strHashData, request.signature);
            if (!bCheckSignSuccessed)
            {
                LOGGER.WARN($"Server get task log request signature check failed.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 解析一个日志请求消息，组装一个日志请求结构
        /// </summary>
        /// <param name="strRequestString"></param>
        /// <param name="strCSharpPrivateKey"></param>
        /// <returns></returns>
        private static SGetTaskLogInfo ParseStructFromRequest(string strRequestString, string strCSharpPrivateKey)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            S2CMessage_GetTaskLogRequest request = new S2CMessage_GetTaskLogRequest();
            dynamic data;
            try
            {
                data = js.Deserialize<dynamic>(strRequestString);   // 反序列化
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Server get task log request is not a invalid JSON format. \n Error = {e.ToString()} \n Request = {strRequestString}");
                return request.info;
            }

            try
            {
                request.info.taskID = int.Parse(data["taskID"]);
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Parse server get task log request failed. \n Error = {e.ToString()} \n Request = {strRequestString}");
                return request.info;
            }
            return request.info;
        }
    }
}