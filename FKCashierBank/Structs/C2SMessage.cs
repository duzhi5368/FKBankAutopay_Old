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
// Create Time         :    2017/7/17 16:32:58
// Update Time         :    2017/7/17 16:32:58
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Web.Script.Serialization;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierBank
{
    /// <summary>
    /// 出款结果消息 C->S
    /// </summary>
    internal class C2SMessage_OutTaskResult : C2S_MessageBase
    {
        public SOutTaskResult info { get; set; }
 
        public override void CaclVerifyCode(string strCSharpPrivateKey)
        {
            try
            {
                info.timestamp = CaclTimestamp();
                string strRet = "";

                strRet += "dianZiHuiDan=";
                strRet += info.dianZiHuiDan;
                strRet += "&moneyAfterTrade=";
                strRet += info.moneyAfterTrade;
                strRet += "&moneyBeforeTrade=";
                strRet += info.moneyBeforeTrade;
                strRet += "&msg=";
                strRet += info.msg;
                strRet += "&nodeID=";
                strRet += info.nodeID;
                strRet += "&status=";
                strRet += info.status;
                strRet += "&taskID=";
                strRet += info.taskID;
                strRet += "&timestamp=";
                strRet += info.timestamp;
                strRet += "&tradeNo=";
                strRet += info.tradeNo;
                strRet += "&transactionTime=";
                strRet += info.transactionTime;

                string strHashData = "";
                FKBaseUtils.FKHash.GetHash(strRet, ref strHashData);
                info.signature = FKBaseUtils.FKRSASignature.RSASign(strCSharpPrivateKey, strHashData);
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Cacl verify code failed. Error = {e.ToString()}");
            }
        }

        public override string ToPostDataString(string strCSharpPrivateKey)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                CaclVerifyCode(strCSharpPrivateKey);
                string strInfo = serializer.Serialize(info);
                int nLastPos = strInfo.LastIndexOf("}");
                strInfo = strInfo.Substring(0, nLastPos);
                strInfo += ",\"timestamp\":";
                strInfo += info.timestamp;
                strInfo += ",\"signature\":\"";
                strInfo += info.signature;
                strInfo += "\"}";
                return strInfo;
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Format msg failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
    }
    /// <summary>
    /// 查询流水结果消息
    /// </summary>
    internal class C2SMessage_GetBillResult : C2S_MessageBase
    {
        public SBillTaskResult info { get; set; }

        public override void CaclVerifyCode(string strCSharpPrivateKey)
        {
            try
            {
                info.timestamp = CaclTimestamp();
                string strRet = "";

                strRet += "bankId=";
                strRet += info.bankId;
                strRet += "&msg=";
                strRet += info.msg;
                strRet += "&nodeID=";
                strRet += info.nodeID;
                strRet += "&status=";
                strRet += info.status;
                strRet += "&taskID=";
                strRet += info.taskID;
                strRet += "&timestamp=";
                strRet += info.timestamp;

                string strHashData = "";
                FKBaseUtils.FKHash.GetHash(strRet, ref strHashData);
                info.signature = FKBaseUtils.FKRSASignature.RSASign(strCSharpPrivateKey, strHashData);
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Cacl verify code failed. Error = {e.ToString()}");
            }
        }

        public override string ToPostDataString(string strCSharpPrivateKey)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                CaclVerifyCode(strCSharpPrivateKey);
                string strInfo = serializer.Serialize(this.info);
                return strInfo;
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Format msg failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
    }
    /// <summary>
    /// 注册节点消息 C->S
    /// </summary>
    internal class C2SMessage_RequestNode : C2S_MessageBase
    {
        public SRegisteNode info { get; set; }

        public override void CaclVerifyCode(string strCSharpPrivateKey)
        {
            try
            {
                info.timestamp = CaclTimestamp();
                string strRet = "";

                strRet += "bankCardNo=";
                strRet += info.bankCardNo;
                strRet += "&ip=";
                strRet += info.ip;
                strRet += "&publicKey=";
                strRet += info.publicKey;
                strRet += "&timestamp=";
                strRet += info.timestamp;

                string strHashData = "";
                FKBaseUtils.FKHash.GetHash(strRet, ref strHashData);
                info.signature = FKBaseUtils.FKRSASignature.RSASign(strCSharpPrivateKey, strHashData);

                // LOGGER.DEBUG($"Hash src = \n{strRet}\n dest = {strHashData}");
                // LOGGER.DEBUG($"Sign key = \n{strCSharpPrivateKey}\n src = {strHashData}\n dest = {info.signature}");
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Cacl verify code failed. Error = {e.ToString()}");
            }
        }

        public override string ToPostDataString(string strCSharpPrivateKey)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                CaclVerifyCode(strCSharpPrivateKey);
                string strInfo = serializer.Serialize(this.info);
                return strInfo;
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Format msg failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
    }
    /// <summary>
    /// 心跳消息 C->S
    /// </summary>
    internal class C2SMessage_Heartbeat : C2S_MessageBase
    {
        public SHeartbeat info { get; set; }

        public override void CaclVerifyCode(string strCSharpPrivateKey)
        {
            try
            {
                info.timestamp = CaclTimestamp();
                string strRet = "";

                strRet += "id=";
                strRet += info.id;
                strRet += "&taskID=";
                strRet += info.taskID;
                strRet += "&timestamp=";
                strRet += info.timestamp;

                string strHashData = "";
                FKBaseUtils.FKHash.GetHash(strRet, ref strHashData);
                info.signature = FKBaseUtils.FKRSASignature.RSASign(strCSharpPrivateKey, strHashData);
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Cacl verify code failed. Error = {e.ToString()}");
            }
        }

        public override string ToPostDataString(string strCSharpPrivateKey)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                CaclVerifyCode(strCSharpPrivateKey);
                string strInfo = serializer.Serialize(this.info);
                return strInfo;
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Format msg failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
    }
    /// <summary>
    /// 注销节点消息 C->S
    /// </summary>
    internal class C2SMessage_UnregisteNode : C2S_MessageBase
    {
        public SUnregisteNode info { get; set; }

        public override void CaclVerifyCode(string strCSharpPrivateKey)
        {
            try
            {
                info.timestamp = CaclTimestamp();
                string strRet = "";

                strRet += "id=";
                strRet += info.id;
                strRet += "&timestamp=";
                strRet += info.timestamp;

                string strHashData = "";
                FKBaseUtils.FKHash.GetHash(strRet, ref strHashData);
                info.signature = FKBaseUtils.FKRSASignature.RSASign(strCSharpPrivateKey, strHashData);
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Cacl verify code failed. Error = {e.ToString()}");
            }
        }

        public override string ToPostDataString(string strCSharpPrivateKey)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                CaclVerifyCode(strCSharpPrivateKey);
                string strInfo = serializer.Serialize(this.info);
                return strInfo;
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Format msg failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
    }
}