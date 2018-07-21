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
// Create Time         :    2017/7/17 16:30:40
// Update Time         :    2017/7/17 16:30:40
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
// ===============================================================================
namespace FKCashierBank
{
    /// <summary>
    /// 客户端->服务器 的全部消息强制继承本类
    /// </summary>
    public abstract class C2S_MessageBase
    {
        /// <summary>
        /// 计算时间戳
        /// </summary>
        /// <returns></returns>
        public long CaclTimestamp()
        {
            long lTime = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            return lTime;
        }
        /// <summary>
        /// 计算签名
        /// </summary>
        /// <param name="strCSharpPrivateKey"></param>
        public abstract void CaclVerifyCode(string strCSharpPrivateKey);
        /// <summary>
        /// 转换结构为POST数据字符串
        /// </summary>
        /// <param name="strCSharpPrivateKey"></param>
        /// <returns></returns>
        public abstract string ToPostDataString(string strCSharpPrivateKey);
    }

    /// <summary>
    /// 服务器->客户端 的全部消息强制继承本类处理
    /// </summary>
    public abstract class S2C_MessageBase
    {
        public long timestamp { get; set; }
        public string signature { get; set; }

        public abstract string ToGetParamsString();
    }
}