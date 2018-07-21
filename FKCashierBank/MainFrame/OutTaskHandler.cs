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
// Create Time         :    2017/7/17 15:08:59
// Update Time         :    2017/7/17 15:08:59
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Threading;
using System.Windows.Forms;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierBank
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 处理流水查询任务
        /// </summary>
        private void HandleOutTask()
        {
            try
            {
                SOutTaskInfo info = m_OutTasksList.Peek();  // 取出消息，不删除

                LOGGER.INFO($"Handle a new out task:\n {info.ToLogString()} \n, still left {m_OutTasksList.Count - 1} tasks in queue.");

                SOutTaskResult dealResult = new SOutTaskResult();
                dealResult.taskID = info.taskID;
                dealResult.nodeID = GetCurCashierNodeID();

                // 记录当前在处理的任务ID
                SetCurTaskID(info.taskID);

                // 开始处理
                if(!AutomationOut(info, ref dealResult))
                {

                }

                LOGGER.WARN($"Auto out task is done. Now ready to send msg to server. TaskID = {info.taskID}.", info.taskID);
                // 通知服务器处理结果
                HttpSendResult sendResult = RequestSender.SendRequestToServer(dealResult);
                if (sendResult.IsSendSuccessed())
                {
                    SetLastTaskID(info.taskID);
                    SetCurTaskID(INVALID_TASK_ID);
                    LOGGER.INFO($"Send auto out task successed. TaskID = {info.taskID}. \n {sendResult.ToLogString()}");
                }
                else
                {
                    SetLastTaskID(INVALID_TASK_ID);
                    SetCurTaskID(INVALID_TASK_ID);
                    LOGGER.ERROR($"Send auto out task failed. TaskID = {info.taskID}. \n {sendResult.ToLogString()}", info.taskID);
                }
                LOGGER.WARN($"Out task deal over. TaskID = {info.taskID}.", info.taskID);
            }
            catch(Exception e)
            {
                LOGGER.ERROR($"Deal out task error occured. Error = {e.ToString()}");
            }
            finally
            {
                SetCurTaskID(INVALID_TASK_ID);
                m_OutTasksList.Dequeue();       // 无论如何，该任务必须删除
            }
            return;
        }

        /// <summary>
        /// 实际自动处理一次自动出款
        /// </summary>
        /// <param name="info"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool AutomationOut(SOutTaskInfo info, ref SOutTaskResult result)
        {
            // 检查是否支持该银行
            int nBankTypeID = GetOutBankTypeIDByName(info.fromBankCode);
            if (nBankTypeID <= 0)
            {
                result.status = (int)SOutTaskResult.ENUM_OutcomeActionStatus.eOutcomeActionStatus_UnsupportBankCode;
                return false;
            }

            // 先进行清理
            // 为保持长期登录状态，现不进行清理 - added by Frankie.W 2017-07-19
            //{
            //    FKWebAutomatic.FKWebDriver.GetInstance.FKClose();
            //    ForceShutdownIE();
            //    Thread.Sleep(1000);
            //}

            DateTime startTime = DateTime.Now;

            bool bSuccessed = false;
            BankAutoOutBase imp = null;
            try
            {
                switch (nBankTypeID)
                {
                    case 1:
                        imp = new CMBC_AutoOut();
                        break;
                    default:
                        break;
                }

                if(imp == null)
                {
                    LOGGER.ERROR($"Auto out failed.");
                    result.status = (int)SOutTaskResult.ENUM_OutcomeActionStatus.eOutcomeActionStatus_UnknownError;
                    bSuccessed = false;
                }
                else
                {
                    imp.Init();
                    bSuccessed = imp.AutoDo(info.taskID, info, ref result);
                    imp.Clear();
                }
            }
            catch(Exception e)
            {
                LOGGER.ERROR($"Auto do error occured. Error = {e.ToString()}");
                bSuccessed = false;
            }
            DateTime endTime = DateTime.Now;
            TimeSpan span = endTime - startTime;

            if (!bSuccessed)
            {
                LOGGER.ERROR($"Auto do failed.");
                // 执行失败
                result.status = (int)SOutTaskResult.ENUM_OutcomeActionStatus.eOutcomeActionStatus_AutoProcessFailed;
            }

            // 无论成功与否，都要进行清理
            // 为保持长期登录状态，现不进行清理 - added by Frankie.W 2017-07-19
            //{
            //    FKWebAutomatic.FKWebDriver.GetInstance.FKClose();
            //    ForceShutdownIE();
            //}

            // 并记录到日志时间数据库，以便日志查询
            FKLog.FKSQLiteLogMgr.GetInstance.AddTaskTimeLog(info.taskID, DateTime.Now);

            return bSuccessed;
        }

        /// <summary>
        /// 通过BankName获取BankID [注意：仅负责出款的银行映射]
        /// </summary>
        /// <param name="strBankName"></param>
        /// <returns></returns>
        public static int GetOutBankTypeIDByName(string strBankName)
        {
            if (string.Compare(strBankName, "CMBC", StringComparison.OrdinalIgnoreCase) == 0)
            {
                // 民生银行
                return 1;
            }
            // todo: add new bank code here
            return -1;
        }
    }
}