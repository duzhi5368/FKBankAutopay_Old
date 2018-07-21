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
// Create Time         :    2017/7/19 10:33:48
// Update Time         :    2017/7/19 10:33:48
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Globalization;
using LOGGER = FKLog.FKLogger;
using System.Threading;
// ===============================================================================
namespace FKCashierBank
{
    internal abstract class BankAutoBillBase
    { 
        /// <summary>
        /// 执行自动查流水之前进行的初始化动作（例如，是否要检查Session激活，是否要重新登录等）
        /// </summary>
        /// <returns></returns>
        public abstract bool Init();
        /// <summary>
        /// 银行执行自动查流水任务入口函数
        /// </summary>
        /// <param name="nTaskID">任务ID</param>
        /// <param name="info">信息详细情况：例如登录密码，收款时间限制等</param>
        /// <param name="result">过程中捕获的详细信息：例如流水情况等</param>
        /// <returns></returns>
        public abstract bool AutoBill(int nTaskID, SBillTaskInfo info, ref SBillTaskResult result);
        /// <summary>
        /// 执行自动查流水之后的释放性动作（例如，删除某些临时文件，关闭一些辅助对象等）
        /// </summary>
        public abstract void Clear();
    }

    class ResultFilter
    {

        /// <summary>
        /// 时间过滤器
        /// </summary>
        /// <param name="info"></param>
        /// <param name="oneBill"></param>
        /// <returns> false表示被过滤掉 true表示需保留</returns>
        public static bool TimeFilter(SBillTaskInfo info, string billTime)
        {
            string startTime = info.startTime;
            string endTime = info.endTime;

            //一天以内数据不做过滤
            if (startTime.Substring(0,8) == endTime.Substring(0, 8))
            {
                return true;
            }
            DateTime dateTimeStart;
            bool ret = DateTime.TryParseExact(startTime, "yyyyMMdd HH:mm:ss", null, DateTimeStyles.None, out dateTimeStart);
            if (!ret)
            {
                LOGGER.INFO($"过滤开始时间格式错误:{startTime}");
                return true;
            }
            DateTime dateTimeEnd;
            ret = DateTime.TryParseExact(endTime, "yyyyMMdd HH:mm:ss", null, DateTimeStyles.None, out dateTimeEnd);
            if (!ret)
            {
                LOGGER.INFO($"过滤结束时间格式错误:{endTime}");
                return true;
            }

            if (dateTimeEnd.CompareTo(dateTimeStart) < 0)
            {
                //LOGGER.INFO($"过滤结束时间:{endTime}早于开始时间:{startTime}");
                return true;
            }
            DateTime dateTimeSumbmit;
            ret = DateTime.TryParseExact(billTime, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out dateTimeSumbmit);
            if (!ret)
            {
                LOGGER.INFO($"明细时间格式错误:{billTime}");
                return true;
            }
            if (dateTimeSumbmit.CompareTo(dateTimeStart) < 0)
            {
                //LOGGER.INFO($"删除项时间[{billTime}],过滤开始时间[{startTime}]");
                return false;
            }
            else if (dateTimeSumbmit.CompareTo(dateTimeEnd) > 0)
            {
                //LOGGER.INFO($"删除项时间[{billTime}],过滤结束时间[{endTime}]");
                return false;
            }
            return true;
        }
    }

    class FreqLimit
    {
        private DateTime lastCallTime;
        private int secondsForEachCall;
        public FreqLimit(int seconds = 120)
        {
            secondsForEachCall = seconds;
            //lastCallTime = DateTime.Now;
        }
        public void Check()
        {
            var now = DateTime.Now;

            int leftSeconds = secondsForEachCall - (int)now.Subtract(lastCallTime).TotalSeconds;
            if (leftSeconds > 0)
            {
                LOGGER.INFO($"调用频率限制，睡眠:{leftSeconds}s");
                Thread.Sleep(leftSeconds * 1000);
            }
            lastCallTime = DateTime.Now;
        }
        
    }
}