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
// Create Time         :    2017/7/17 15:18:29
// Update Time         :    2017/7/17 15:18:29
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
// ===============================================================================
namespace FKCashierBank
{
    /// <summary>
    /// 基类
    /// </summary>
    public abstract class StructBase
    {
        /// <summary>
        /// 完整Log
        /// </summary>
        /// <returns></returns>
        public abstract string ToLogString();
        /// <summary>
        /// 简要Log (用于存放DB)
        /// </summary>
        /// <returns></returns>
        public abstract string ToSimpleString();
    }

    /// <summary>
    /// 客户端节点给服务器主动发送的消息基类
    /// </summary>
    public abstract class C2SStruct : StructBase
    {
        public long timestamp { get; set; }
        public string signature { get; set; }
    }

    /// <summary>
    /// 服务器主动发送给客户端节点的消息基类
    /// </summary>
    public abstract class S2CStruct : StructBase
    {

    }
}