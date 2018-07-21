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
// Create Time         :    2017/7/17 13:43:12
// Update Time         :    2017/7/17 13:43:12
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierBank
{
    internal static class RequestReciver
    {
        /// <summary>
        /// 收到的网络消息处理函数
        /// </summary>
        /// <param name="mainForm"></param>
        /// <param name="request"></param>
        /// <param name="strRequestString"></param>
        /// <returns></returns>
        public static string Func_OnRequest(Form f, HttpListenerRequest request, string strRequestString)
        {
            MainForm mainForm = null;
            if(!(f is MainForm))
            {
                LOGGER.ERROR("Can't get main form object.");
                return DefaultErrorResponse(-1, -1);
            }
            else
            {
                mainForm = f as MainForm;
            }
            if(mainForm == null)
            {
                LOGGER.ERROR("Can't get main form object.");
                return DefaultErrorResponse(-1, -1);
            }

            int nNodeID = mainForm.GetCurCashierNodeID();
            int nErrorCode = -1;
            string strRespone = string.Empty;
            try
            {
                int nMsgID = GetMsgIDFromResponse(strRequestString);
                switch (nMsgID)
                {
                    case (int)ENUM_S2CMsgID.eS2CMsgID_OutTaskRequest:
                        nErrorCode = OutTaskRequestHandler.OnMsg(nNodeID, strRequestString, mainForm, ref strRespone);
                        return strRespone;
                    case (int)ENUM_S2CMsgID.eS2CMsgID_GetTaskLogRequest:
                        nErrorCode = GetTaskLogRequestHandler.OnMsg(nNodeID, strRequestString, mainForm, ref strRespone);
                        break;
                    case (int)ENUM_S2CMsgID.eS2CMsgID_ManageAction:
                        break;
                    case (int)ENUM_S2CMsgID.eS2CMsgID_GetBillTaskRequest:
                        nErrorCode = GetBillTaskRequestHandler.OnMsg(nNodeID, strRequestString, mainForm, ref strRespone);
                        break;
                    default:
                        LOGGER.WARN($"Unknown server request msg. \n Request = {strRequestString}");
                        break;
                }
            }
            catch(Exception e)
            {
                LOGGER.WARN($"Deal with server request msg error occured. Error = {e.ToString()} \n Request = {strRequestString}");
                return DefaultErrorResponse(nNodeID, nErrorCode);
            }

            return DefaultErrorResponse(nNodeID, nErrorCode);
        }

        /// <summary>
        /// 解析消息获取其消息ID
        /// </summary>
        /// <param name="strMsg"></param>
        /// <returns></returns>
        private static int GetMsgIDFromResponse(string strMsg)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            dynamic data;
            try
            {
                data = js.Deserialize<dynamic>(strMsg);   // 反序列化
            }
            catch(Exception e)
            {
                LOGGER.ERROR($"Server request is not JSON format. \n Error = {e.ToString()} \n Respone = {strMsg}");
                return -1;
            }

            // 获取消息类型
            int msgID = -1;
            try
            {
                msgID = int.Parse(data["msgID"]);
            }
            catch(Exception e)
            {
                LOGGER.ERROR($"Server request is not include invalid msgID. \n Error = {e.ToString()} \n Respone = {strMsg}");
                return -1;
            }
            return msgID;
        }

        /// <summary>
        /// 通知服务器的默认错误Response
        /// </summary>
        private static string DefaultErrorResponse(int nNodeID, int nErrorCode)
        {
            return ("{\"nodeID\":") + nNodeID + (",\"status\":") + nErrorCode + ("}");
        }
    }
}