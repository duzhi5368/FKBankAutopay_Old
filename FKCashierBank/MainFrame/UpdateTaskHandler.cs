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
// Create Time         :    2017/7/19 14:51:53
// Update Time         :    2017/7/19 14:51:53
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Windows.Forms;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierBank
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 更新任务
        /// </summary>
        private DateTime HandleUpdateTask(DateTime suggestEarlistUpdateTime, int nOprationType, string strBankCode)
        {
            if(DateTime.Compare(suggestEarlistUpdateTime, DateTime.Now) > 0)
            {
                // 还没到预期的刷新时间，什么都不做
                return suggestEarlistUpdateTime;
            }

            if (nOprationType == 1)
            {
                int nBankType = MainForm.GetBillBankTypeIDByName(strBankCode);
                // 之前负责的是流水查询任务
                switch (nBankType)
                {
                    case 1:
                        // 建设银行
                        CCB_AutoBill.Update();
                        return suggestEarlistUpdateTime.AddSeconds(CCB_AutoBill.GetUpdateIdleSecond());
                    case 2:
                        // 交通银行
                        BCM_AutoBill.Update();
                        return suggestEarlistUpdateTime.AddSeconds(BCM_AutoBill.GetUpdateIdleSecond());
                    case 3:
                        // 招商银行
                        CMB_AutoBill.Update();
                        return suggestEarlistUpdateTime.AddSeconds(CMB_AutoBill.GetUpdateIdleSecond());
                    case 4:
                        // 招商银行
                        ABC_AutoBill.Update();
                        return suggestEarlistUpdateTime.AddSeconds(ABC_AutoBill.GetUpdateIdleSecond());
                    case 5:
                        // 中信银行
                        CITIC_AutoBill.Update();
                        return suggestEarlistUpdateTime.AddSeconds(CITIC_AutoBill.GetUpdateIdleSecond());
                }
            }

            if (nOprationType == 2)
            {
                int nBankType = MainForm.GetOutBankTypeIDByName(strBankCode);
                switch (nBankType)
                {
                    case 1:
                        // 民生银行
                        CMBC_AutoOut.Update();
                        return suggestEarlistUpdateTime.AddSeconds(CMBC_AutoOut.GetUpdateIdleSecond());
                }
            }

            return DateTime.Now;
        }
    }
}