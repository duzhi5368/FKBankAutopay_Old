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
// Create Time         :    2017/7/17 14:18:22
// Update Time         :    2017/7/17 14:18:22
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierBank
{
    public partial class MainForm : Form
    {
        private Queue<SOutTaskInfo>     m_OutTasksList = new Queue<SOutTaskInfo>();         // 出款任务队列
        private Queue<SBillTaskInfo>    m_BillTasksList = new Queue<SBillTaskInfo>();       // 流水查询任务队列

        enum ENUM_LogicTaskType
        {
            eLogicTaskType_Unknown = 0,
            eLogicTaskType_Bill = 1,        // 流水查询任务
            eLogicTaskType_Out = 2,         // 出款任务
        }
        /// <summary>
        /// 逻辑线程
        /// </summary>
        private void LogicThreadMain()
        {
            int nIdleTime = FKConfig.LoginThreadIdleTime;
            LOGGER.INFO($"Start Logic thread. Idle time = {nIdleTime} millseconds.");

            // 建议最早更新时间：若早于该时间则一定不会进行更新事件通知
            DateTime SuggestEarlistUpdateTime = DateTime.Now;
            string strBankType = string.Empty;
            int nOprationType = 0;  // 0: Unknown 1: GetBill 2: Out

            while (!m_bIsNeedCloseLogicThread)
            {
                if(m_BillTasksList.Count > 0)
                {
                    try
                    {
                        nOprationType = 1;
                        strBankType = m_BillTasksList.Peek().bankCode;
                        HandleBillTask();
                    }
                    catch(Exception e)
                    {
                        LOGGER.ERROR($"Handle bill task failed. Error = {e.ToString()}");
                    }
                }
                else if(m_OutTasksList.Count > 0)
                {
                    try
                    {
                        nOprationType = 2;
                        strBankType = m_OutTasksList.Peek().fromBankCode;
                        HandleOutTask();
                    }
                    catch (Exception e)
                    {
                        LOGGER.ERROR($"Handle out task failed. Error = {e.ToString()}");
                    }
                }

                {
                  // 每次无论出入账与否，都尝试进行一次刷新
                  try
                  {
                      // 根据上一次执行的任务进行update更新
                      SuggestEarlistUpdateTime = HandleUpdateTask(SuggestEarlistUpdateTime, nOprationType, strBankType);
                  }
                  catch (Exception e)
                  {
                      LOGGER.ERROR($"Handle update task failed. Error = {e.ToString()}");
                  }

                  // 逻辑线程休眠
                  Thread.Sleep(nIdleTime);
                }
            }

            LOGGER.INFO($"Logic thread exit");
        }
    }
}