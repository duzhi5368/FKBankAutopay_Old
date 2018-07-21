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
// Create Time         :    2017/8/3 13:45:08
// Update Time         :    2017/8/3 13:45:08
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierBank
{
    public static class GetBillTaskRequestHandler
    {
        public enum ENUM_GetBillTaskResponseError
        {
            eGetBillTaskResponseError_UnknownFailed = 0,        // 未知错误
            eGetBillTaskResponseError_Successed = 1,            // 成功
            eGetBillTaskResponseError_UnvalidMsg = 2,           // 不合法消息
            eGetBillTaskResponseError_UnvalidTaskId = 3,        // 无效TaskID
        }

        public static int OnMsg(int nNodeID, string strRequestString, MainForm mainForm, ref string strRespone)
        {
            int nErrorCode = (int)ENUM_GetBillTaskResponseError.eGetBillTaskResponseError_UnknownFailed;
            // 检查请求合法性
            if (!IsValidGetBillRequestMsg(strRequestString, RSAKeyContainer.GetInstance.GetJavaPublicKey()))
            {
                // 消息检查未通过
                nErrorCode = (int)ENUM_GetBillTaskResponseError.eGetBillTaskResponseError_UnvalidMsg;
                LOGGER.WARN($"Server get bill task request is not a invalid msg. \n Request = {strRequestString}");
            }
            else
            {
                List<SBillTaskInfo> infos = ParseStructFromRequest(strRequestString, RSAKeyContainer.GetInstance.GetCSharpPrivateKey());
                if (infos[0].taskID <= 0)
                {
                    // 消息不正常
                    nErrorCode = (int)ENUM_GetBillTaskResponseError.eGetBillTaskResponseError_UnvalidTaskId;
                    LOGGER.WARN($"Server get bill task request is not a invalid msg. \n Request = {strRequestString} \n TaskInfo = {infos[0].ToLogString()}");
                }
                else
                {
                    // 消息解析成功，开始逻辑处理
                    {
                        // 实际处理
                        for (int i = 0; i < infos.Count; ++i)
                        {
                            mainForm.AddBillTaskToWaitQueue(infos[i]);
                        }
                        nErrorCode = (int)ENUM_GetBillTaskResponseError.eGetBillTaskResponseError_Successed;
                    }
                }
            }
            strRespone = ("{\"nodeID\":") + nNodeID + (",\"status\":") + nErrorCode + ("}");
            return nErrorCode;
        }

        private static bool IsValidGetBillRequestMsg(string strMsg, string strRSAPublicKey)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            S2CMessage_GetBillRequest request = new S2CMessage_GetBillRequest();
            dynamic data;
            try
            {
                data = js.Deserialize<dynamic>(strMsg);   // 反序列化
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Server get bill task request is not a invalid JSON format. \n Error = {e.ToString()} \n Request = {strMsg}");
                return false;
            }

            try
            {
                request.info.taskID = int.Parse(data["taskID"]);
                request.info.bankId = data["bankId"];
                request.info.bankCode = data["bankCode"];
                request.info.bankName = data["bankName"];
                request.info.username = data["username"];
                request.info.accountNumber = data["accountNumber"];
                request.info.password = data["password"];
                request.info.uKeyPassword = data["uKeyPassword"];
                request.info.startTime = data["startTime"];
                request.info.endTime = data["endTime"];
                request.timestamp = long.Parse(data["timestamp"]);
                request.signature = data["signature"];
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Parse server get bill task request failed. \n Error = {e.ToString()} \n Request = {strMsg}");
                return false;
            }

            
            string strGetSrc = request.ToGetParamsString();
            string strHashData = "";
            FKBaseUtils.FKHash.GetHash(strGetSrc, ref strHashData);
            string strCSharpPublicKey = FKBaseUtils.FKRSAEncrypt.ConvertRSAPublicKey_Java2DotNet(strRSAPublicKey);
            bool bCheckSignSuccessed = FKBaseUtils.FKRSASignature.IsValidRSASign(strCSharpPublicKey, strHashData, request.signature);
            if (!bCheckSignSuccessed)
            {
                LOGGER.WARN($"Server get bill task request signature check failed.");
                return false;
            }
            return true;
        }

        private static List<SBillTaskInfo> ParseStructFromRequest(string strRequestString, string strCSharpPrivateKey)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            SBillTaskInfo info = new SBillTaskInfo();
            List<SBillTaskInfo> result = new List<SBillTaskInfo>();
            List<string> beginTimeList = new List<string>();
            List<string> endTimeList = new List<string>();

            dynamic data;
            try
            {
                data = js.Deserialize<dynamic>(strRequestString);   // 反序列化
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Server get bill task request is not a invalid JSON format. \n Error = {e.ToString()} \n Request = {strRequestString}");
                result.Add(info);
                return result;
            }

            string strLoginPassword = "";
            string strUKeyPassword = "";
            try
            {
                info.taskID = int.Parse(data["taskID"]);
                info.bankId = data["bankId"];
                info.bankCode = data["bankCode"];
                info.bankName = data["bankName"];
                info.username = data["username"];
                info.accountNumber = data["accountNumber"];
                strLoginPassword = data["password"];
                strUKeyPassword = data["uKeyPassword"];


                {   // 时间修正
                    // 优化时间 2016-09-11 10:59:59 - 2016-09-13 23:59:59
                    // 原有算法 [2016-09-11 10:59:59 - 2016-09-12 10:59:59] [2016-09-12 10:59:59 - 2016-09-13 23:59:59]
                    // 现有算法 [2016-09-11 10:59:59 - 2016-09-11 23:59:59] [2016-09-12 00:00:00 - 2016-09-12 23:59:59] [2016-09-13 00:00:00 - 2016-09-13 23:59:59] 

                   

                    long lStartTime = 0;
                    bool b1 = long.TryParse(data["startTime"], out lStartTime);
                    long lEndTime = 0;
                    bool b2 = long.TryParse(data["endTime"], out lEndTime);

                    if (b1 && b2)
                    {
                        if ((lEndTime - lStartTime) > 86400000)
                        {
                            // 多日要开始分割
                            // 第一天
                            DateTime s = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                            s = s.AddMilliseconds(lStartTime);
                            string strBeginDate = s.ToString("yyyyMMdd HH:mm:ss");

                            beginTimeList.Add(strBeginDate);
                            endTimeList.Add(strBeginDate.Substring(0, 8) + " 23:59:59");

                            s = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                            s = s.AddMilliseconds(lEndTime);
                            string strEndDate = s.ToString("yyyyMMdd HH:mm:ss");
                            // 中间的天
                            DateTime middleStart = DateTime.ParseExact(
                                strBeginDate.Substring(0, 8) + " 23:59:59", 
                                "yyyyMMdd HH:mm:ss",System.Globalization.CultureInfo.CurrentCulture);
                            DateTime middleEnd = DateTime.ParseExact(
                                strEndDate.Substring(0, 8) + " 00:00:00",
                                "yyyyMMdd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);

                            s = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                            lStartTime = (long)middleStart.Subtract(s).TotalMilliseconds;
                            lEndTime = (long)middleEnd.Subtract(s).TotalMilliseconds;
                            if (lEndTime > lStartTime)
                            {
                                long lTmpBegin = lStartTime;
                                long lTmpEnd = lTmpBegin + 86400000 - 1000;
                                for (;;)
                                {
                                    s = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                                    s = s.AddMilliseconds(lTmpBegin);
                                    beginTimeList.Add(s.ToString("yyyyMMdd HH:mm:ss"));

                                    s = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                                    s = s.AddMilliseconds(lTmpEnd);
                                    endTimeList.Add(s.ToString("yyyyMMdd HH:mm:ss"));

                                    lTmpBegin = lTmpEnd + 1;
                                    lTmpEnd = lTmpBegin + 86400000 - 1000;
                                    if (lTmpBegin >= lEndTime)
                                        break;
                                    if (lTmpEnd >= lEndTime)
                                        lTmpEnd = lEndTime;
                                }
                            }
                            // 最后一天
                            beginTimeList.Add(strEndDate.Substring(0, 8) + " 00:00:00");
                            endTimeList.Add(strEndDate);
                        }
                        else
                        {
                            // 一日之内
                            DateTime s = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                            s = s.AddMilliseconds(lStartTime);
                            beginTimeList.Add(s.ToString("yyyyMMdd HH:mm:ss"));
                            s = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                            s = s.AddMilliseconds(lEndTime);
                            endTimeList.Add(s.ToString("yyyyMMdd HH:mm:ss"));
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LOGGER.WARN($"Parse server get bill task request failed. \n Error = {e.ToString()} \n Request = {strRequestString}");
                info.taskID = 0;     // 注意清零，外面靠该值判断是否解析成功的
                result.Add(info);
                return result;
            }

            // 解析密码
            try
            {
                // 根据约定进行RSA解密密码
                string xmlKey = FKBaseUtils.FKRSAEncrypt.ConvertRSAPrivateKey_Java2DotNet(strCSharpPrivateKey);
                info.password = FKBaseUtils.FKRSAEncrypt.RSADecryptByDotNetPrivateKey(strLoginPassword, xmlKey);
                if (string.IsNullOrEmpty(info.password))
                {
                    LOGGER.WARN($"Decrypt password failed. Password = {strLoginPassword}");
                    info.taskID = 0;
                    result.Add(info);
                    return result;
                }

                info.uKeyPassword = "";
                //request.info.uKeyPassword = FKBaseUtils.FKRSAEncrypt.RSADecryptByDotNetPrivateKey(strUKeyPassword, xmlKey);
                //if (string.IsNullOrEmpty(request.info.uKeyPassword))
                //{
                //    LOGGER.WARN($"Decrypt ukey password failed. Password = {strUKeyPassword}");
                //    request.info.taskID = 0;
                //    return request.info;
                //}
            }
            catch (Exception e)
            {
                LOGGER.WARN($"Decrypt passwords failed. \n Error = {e.ToString()} \n Request = {strRequestString}");
                info.taskID = 0;
                result.Add(info);
                return result;
            }

            if (beginTimeList.Count != 0 && endTimeList.Count != 0 && beginTimeList.Count == endTimeList.Count)
            {
                for (int i = 0; i < beginTimeList.Count; ++i)
                {
                    SBillTaskInfo tmp = new SBillTaskInfo();
                    tmp = (SBillTaskInfo)info.Clone();
                    tmp.startTime = beginTimeList[i];
                    tmp.endTime = endTimeList[i];
                    result.Add(tmp);
                }
            }
            return result;
        }
    }
}