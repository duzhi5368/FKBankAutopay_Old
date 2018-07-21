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
// Create Time         :    2017/7/17 15:08:01
// Update Time         :    2017/7/17 15:08:01
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
        private void HandleBillTask()
        {
            try
            {
                SBillTaskInfo info = m_BillTasksList.Peek();    // 取出消息，不删除

                LOGGER.INFO($"Handle a new bill task:\n {info.ToLogString()} \n, still left {m_BillTasksList.Count - 1} tasks in queue.");

                SBillTaskResult dealResult = new SBillTaskResult();
                dealResult.taskID = info.taskID;
                dealResult.nodeID = GetCurCashierNodeID();
                dealResult.bankId = info.bankId;

                // 记录当前在处理的任务ID
                SetCurTaskID(info.taskID);

                // 开始处理
                AutomationBill(info, ref dealResult);

                LOGGER.WARN($"Auto get bill task is done. Now ready to send msg to server. TaskID = {info.taskID}.", info.taskID);

                // 发送回馈消息给服务器完毕
                HttpSendResult SendResult = RequestSender.SendRequestToServer(dealResult);
                if (SendResult.IsSendSuccessed())
                {
                    SetLastTaskID(info.taskID);
                    SetCurTaskID(INVALID_TASK_ID);
                    LOGGER.INFO($"Send get bill result successed. TaskID = {info.taskID}. \n {SendResult.ToLogString()}");
                }
                else
                {
                    // result = null; 无论是否发送给服务器成功，只要执行成功，就不允许清除
                    SetLastTaskID(INVALID_TASK_ID);
                    SetCurTaskID(INVALID_TASK_ID);
                    LOGGER.ERROR($"Send get bill result failed. TaskID = {info.taskID}. \n {SendResult.ToLogString()}", info.taskID);
                }
                LOGGER.WARN($"Get bill task deal over. TaskID = {info.taskID}.", info.taskID);
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"HandleBillTask exception.msg: {e.Message}.");
            }
            finally
            {
                SetCurTaskID(INVALID_TASK_ID);
                m_BillTasksList.Dequeue();       // 无论如何，该任务必须删除
            }

            return;
        }

        /// <summary>
        /// 实际自动处理一次流水查询
        /// </summary>
        /// <param name="info"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool AutomationBill(SBillTaskInfo info, ref SBillTaskResult result)
        {
            // 检查是否支持该银行
            int nBankTypeID = GetBillBankTypeIDByName(info.bankCode);
            if (nBankTypeID <= 0)
            {
                LOGGER.ERROR($"Auto bill failed: Unknown bank code - {info.bankCode}");
                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_UnsupportBankCode;
                return false;
            }
            // 检查重要参数是否合法
            if (string.IsNullOrEmpty(info.accountNumber) ||
                string.IsNullOrEmpty(info.username) ||
                string.IsNullOrEmpty(info.password))
            {
                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_ArgumentInvalid;
                LOGGER.ERROR($"Auto bill failed: account info invalid");
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
            BankAutoBillBase imp = null;
            try
            {
                switch (nBankTypeID)
                {
                    case 1:
                        imp = new CCB_AutoBill();
                        break;
                    case 2:
                        imp = new BCM_AutoBill();
                        break;
                    case 3:
                        imp = new CMB_AutoBill();
                        break;
                    case 4:
                        imp = new ABC_AutoBill();
                        break;
                    case 5:
                        imp = new CITIC_AutoBill();
                        break;
                    default:
                        break;
                }

                if (imp == null)
                {
                    LOGGER.ERROR($"Auto bill failed: imp not found");
                    result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_UnknownError;
                    bSuccessed = false;
                }
                else
                {
                    LOGGER.INFO($"Auto bill start: id = {info.taskID}");
                    imp.Init();
                    bSuccessed = imp.AutoBill(info.taskID, info, ref result);
                    imp.Clear();
                    LOGGER.INFO($"Auto bill finish: id = {info.taskID}");
                }
                
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Auto bill error occured. Error = {e.ToString()}");
                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_UnknownError;
                bSuccessed = false;
            }
            DateTime endTime = DateTime.Now;
            TimeSpan span = endTime - startTime;

            if (!bSuccessed)
            {
                //LOGGER.ERROR($"Auto bill failed: imp.AutoBill() return false");
                // 执行失败
                result.status = (int)SBillTaskResult.ENUM_BillActionStatus.eBillActionStatus_AutoProcessFailed;
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
        /// 通过BankName获取BankID [注意：仅负责流水查询的银行映射]
        /// </summary>
        /// <param name="strBankName"></param>
        /// <returns></returns>
        public static int GetBillBankTypeIDByName(string strBankName)
        {
            if (string.Compare(strBankName, "CCB", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return 1;   // 建设银行
            }
            else if (string.Compare(strBankName, "BCM", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return 2;   // 交通银行
            }
            else if (string.Compare(strBankName, "CMB", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return 3;   // 招商银行
            }
            else if (string.Compare(strBankName, "ABC", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return 4;   // 农业银行
            }
            else if (string.Compare(strBankName, "CITIC", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return 5;   // 中信银行
            }
            // todo: add new bank code here
            return -1;
        }
    }
}