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
// Create Time         :    2017/7/19 10:39:09
// Update Time         :    2017/7/19 10:39:09
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Diagnostics;
using System.Threading;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierBank
{
    /// <summary>
    /// 招商银行自动流水查询核心流程
    /// </summary>
    internal class CMB_AutoBill : BankAutoBillBase
    {
        #region ==== 外部接口 ====

        public override bool Init()
        {
            if (/*bNewAccount*/true)        // todo:
            {
                s_bIsClientOpening = false;
                s_bIsClientOpening = SafeStartUpCMBClient();
            }
            return s_bIsClientOpening;
        }
        public override bool AutoBill(int nTaskID, SBillTaskInfo info, ref SBillTaskResult result)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            SafeKillCMBClient();
        }

        // 刷新事件
        public static void Update()
        {

        }

        // 刷新推荐的间隔时间( 单位：秒 )
        public static int GetUpdateIdleSecond()
        {
            return 10;
        }

        #endregion ==== 外部接口 ====

        #region ==== 成员变量 ====

        private const string CMBAppName = "PersonalBankPortal.exe";
        private const string CMDAppDefaultPath = "C:\\Windows\\SysWOW64\\PersonalBankPortal.exe";
        private static bool s_bIsClientOpening = false;

        #endregion ==== 成员变量 ====

        #region ==== 核心函数 ====

        /// <summary>
        /// 启动招商银行客户端
        /// </summary>
        private static bool SafeStartUpCMBClient()
        {
            try
            {
                // 首先尝试安全关闭招商银行客户端
                SafeKillCMBClient();
                // 给系统一些时间，慢慢来
                Thread.Sleep(1000);
                // 尝试正常启动
                Process.Start(CMDAppDefaultPath);
            }
            catch(Exception e)
            {
                LOGGER.WARN($"Start CMB client error ouccred. Error = {e.ToString()}");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 安全关闭招商银行客户端
        /// </summary>
        private static void SafeKillCMBClient()
        {
            try
            {
                int[] CMBProcessIDList = SearchProcessIDByName(CMBAppName);
                foreach (var id in CMBProcessIDList)
                {
                    // 首先，尝试合理释放
                    var process = Process.GetProcessById(id);
                    process.Close();
                }
                foreach (var id in CMBProcessIDList)
                {
                    // 然后，暴力释放
                    FKBaseUtils.FKProcessHelper.KillProcessTreeById(id);
                }
                // 最后，屠杀
                FKBaseUtils.FKCommonFunc.RunConsoleCommand("taskkill.exe", " /f /IM " + CMBAppName);
            }
            catch(Exception e)
            {
                LOGGER.WARN($"Kill CMB client error ouccred. Error = {e.ToString()}");
            }
        }
        /// <summary>
        /// 查找指定名的进程ID组
        /// </summary>
        /// <param name="strProcName"></param>
        /// <returns></returns>
        private static int[] SearchProcessIDByName(string strProcName)
        {
            int[] ret = new int[8];   // 最大认为会有8个用户
            try
            {
                Process[] ps = Process.GetProcessesByName(strProcName);
                if (ps.Length > 0)
                {
                    int nIndex = 0;
                    foreach(var process in ps)
                    {
                        if (nIndex < 8 && nIndex >= 0)
                            ret[nIndex] = process.Id;
                        nIndex++;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                LOGGER.WARN($"Search process ID by name failed. Error = {e.ToString()}");
                return null;
            }
            return ret;
        }

        #endregion ==== 核心函数 ====
    }
}