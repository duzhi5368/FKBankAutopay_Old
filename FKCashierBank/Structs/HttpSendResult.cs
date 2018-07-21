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
// Create Time         :    2017/7/17 15:42:02
// Update Time         :    2017/7/17 15:42:02
// Class Version       :    v1.0.0.0
// Class Description   :    客户端向服务器发送任何Http消息的行为结果
// ===============================================================================
using System;
// ===============================================================================
namespace FKCashierBank
{
    internal class HttpSendResult : StructBase
    {
        public bool bSuccess { get; set; }
        public int ErrorStatus { get; set; }
        public string ErrorMsg { get; set; }
        public string RequestType { get; set; }
        public string ServerUrl { get; set; }
        public string RequestMsg { get; set; }
        public string ResponseMsg { get; set; }
        public int NodeID { get; set; }
        public string publicKey { get; set; }

        public override string ToLogString()
        {
            string strRet = "---------------------------\n";
            try
            {
                strRet += ("节点编号：" + NodeID + "\n");
                strRet += ("消息类型：" + RequestType + "\n");
                strRet += ("解析结果：" + (bSuccess ? "成功" : "失败") + "\n");
                strRet += ("错误信息：" + ErrorStatus + " : " + ErrorMsg + "\n");
                strRet += ("发送目标：" + ServerUrl + "\n");
                strRet += ("发送内容：" + (RequestMsg.Length > 300 ? RequestMsg.Substring(0,300) + "..." : RequestMsg) + "\n");
                strRet += ("接收内容：" + ResponseMsg + "\n");
                strRet += "---------------------------";
            }
            catch { }
            return strRet;
        }

        public override string ToSimpleString()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 是否本消息发送成功了
        /// </summary>
        /// <returns></returns>
        public bool IsSendSuccessed()
        {
            return bSuccess;
        }
    }
}