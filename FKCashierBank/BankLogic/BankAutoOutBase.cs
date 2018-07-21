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
// Create Time         :    2017/7/17 18:04:58
// Update Time         :    2017/7/17 18:04:58
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Collections.Generic;
// ===============================================================================
namespace FKCashierBank
{
    internal abstract class BankAutoOutBase
    {
        /// <summary>
        /// 执行自动出款之前进行的初始化动作（例如，是否要检查Session激活，是否要重新登录等）
        /// </summary>
        /// <returns></returns>
        public abstract bool Init();
        /// <summary>
        /// 银行执行出账任务入口函数
        /// </summary>
        /// <param name="nTaskID">任务ID</param>
        /// <param name="info">信息详细情况：例如登录密码，收款人，金额等</param>
        /// <param name="result">过程中捕获的详细信息：例如余额，流水单号等</param>
        /// <returns></returns>
        public abstract bool AutoDo(int nTaskID, SOutTaskInfo info, ref SOutTaskResult result);
        /// <summary>
        /// 执行自动出款之后的释放性动作（例如，删除某些临时文件，关闭一些辅助对象等）
        /// </summary>
        public abstract void Clear();
    }
}