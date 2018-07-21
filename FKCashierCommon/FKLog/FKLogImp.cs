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
// Create Time         :    2017/7/13 16:47:54
// Update Time         :    2017/7/13 16:47:54
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using FKBaseUtils;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
// ===============================================================================
namespace FKLog
{
    public class FKLogImp : FKSingleton<FKLogImp>
    {
        /// <summary>
        /// Form使用的Log添加函数
        /// </summary>
        /// <param name="color"></param>
        /// <param name="strText"></param>
        public delegate void LogAppendDelegate(Color color, string strText);
        /// <summary>
        /// 日志级别
        /// </summary>
        public enum ENUM_LogLevel
        {
            eLogLevel_Debug = 0,        // IDE控制台输出，不写入任何文件，不在用户界面输出
            eLogLevel_Info,             // 写入TXT文件，IDE控制台输出，在用户界面输出，不写入DB
            eLogLevel_Warning,          // 写入TXT文件，IDE控制台输出，用户界面输出，写入DB
            eLogLevel_Error,            // 写入TXT文件，IDE控制台输出，用户界面输出，写入DB
        }

        #region ==== 成员变量 ====

        // TODO: RichTextBox, TextBox 之后考虑移除使用回调或者反射。另外，应该考虑自定义控件
        private TraceSource m_Trace = null;
        private object      m_objAddLogLock = null;
        private int         m_nLogID = 0;
        private string      m_strTxtFileName = FKLogConsts.DEFAULT_TEXT_LOG_FILE_NAME;
        private RichTextBox m_LogRichTextBox = null;
        private TextBox     m_CommandEditor = null;
        private FKSQLiteLog m_DBLog = null;

        #endregion ==== 类变量 ====

        #region ==== 构造函数 ====

        private FKLogImp()
        {
            try
            {
                m_Trace = new TraceSource("FKLog");
                m_Trace.Switch = new SourceSwitch("SourceSwitch", "Verbose");
                m_DBLog = new FKSQLiteLog();

                m_objAddLogLock = new object();
            }
            catch (Exception e)
            {
                throw new Exception($"[Error] Create FKLogInstance failed. Error = {e.ToString()}");
            }
        }

        #endregion ==== 构造函数 ====

        #region ==== 对外接口 ====

        public void SetRichTextBox(RichTextBox richbox)
        {
            m_LogRichTextBox = richbox;
        }
        public void SetCommandEditor(TextBox commandEditor)
        {
            m_CommandEditor = commandEditor;
        }
        /// <summary>
        /// 添加一行日志，注意：这里拒绝向外开放，请去使用 FKLogImp 中的接口
        /// </summary>
        /// <param name="eLevel"></param>
        /// <param name="strMsg"></param>
        /// <param name="nTaskID"></param>
        internal void AddLog(ENUM_LogLevel eLevel, string strMsg, int nTaskID = 0)
        {
            try
            {
                if (strMsg == null)
                {
                    strMsg = "Empty message";
                }
                DateTime curTime = DateTime.Now;
                string strDayTime = curTime.ToString("yyyy-MM-dd");
                m_strTxtFileName = FKSystemFileSystemHelper.GetWorkdir() + "\\" + FKLogConsts.TEXT_LOG_DIR_NAME 
                    + "\\" + "FKLog." + strDayTime + FKLogConsts.TEXT_LOG_FILE_SUFFIX;

                m_nLogID++;
                if (m_nLogID >= 999999)
                {
                    m_nLogID = 0;
                }

                StackTrace ss = new StackTrace(true);
                StackFrame sf = ss.GetFrame(2); // 上两层调用点
                MethodBase mb = sf.GetMethod(); // 上两层调用函数
                string strModuleName = mb.Module.Name;
                string strNameSpaceName = mb.DeclaringType.Namespace;
                string strFunctionName = mb.Name;
                string strFileName = sf.GetFileName();                  // 文件名
                int nLine = sf.GetFileLineNumber();                     // 调用行号
                string strFullName = mb.ReflectedType.FullName;         // 完全限定名
                string strClassName = mb.DeclaringType.Name;            // 类名
                string strThreadName = Thread.CurrentThread.Name;       // 线程名

                string strFinalMsg = FormatString(eLevel, curTime, m_nLogID, strMsg,
                    strClassName, strThreadName, strFullName, strFileName, nLine, 
                    eLevel == ENUM_LogLevel.eLogLevel_Error ? ss.ToString() : "");

                lock (m_objAddLogLock)
                {
                    Console.WriteLine(strFinalMsg);
                    WriteToIDE(eLevel, m_nLogID, strMsg);
                    WriteToFile(eLevel, strFinalMsg);
                    WriteToDB(eLevel, curTime, m_nLogID, strMsg, strFinalMsg, nTaskID);
                }
                //if (string.Compare(strThreadName, "FKLogicThread") != 0) //非主进程，禁止Form输出，不然会有可能出现死锁
                {
                    WriteToForm(eLevel, curTime, m_nLogID, strMsg, strFinalMsg, nTaskID);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Add log failed. Error = {e.ToString()}");
            }
        }
        /// <summary>
        /// 获取指定日志中指定TaskID的日志
        /// </summary>
        /// <param name="nTaskID"></param>
        /// <param name="DBFileName"></param>
        /// <returns></returns>
        internal string GetLogByTaskID(int nTaskID, string DBFileName)
        {
            if (m_DBLog == null)
                return string.Empty;
            return m_DBLog.GetLogByTaskID(nTaskID, DBFileName);
        }
        /// <summary>
        /// 获取指定日志中指定TaskID日志的数量
        /// </summary>
        /// <param name="nTaskID"></param>
        /// <param name="DBFileName"></param>
        /// <returns></returns>
        internal int GetLogNumByTaskID(int nTaskID, string DBFileName)
        {
            if (m_DBLog == null)
                return 0;
            return m_DBLog.GetLogsCountByTaskID(nTaskID);
        }

        #endregion ==== 对外接口 ====

        #region ==== 内部函数 ====

        /// <summary>
        /// 组成一个完整的自定义格式的日志字符串
        /// 请不要修改格式，规范在：http://10.66.72.200:8090/pages/viewpage.action?pageId=10289925
        /// </summary>
        /// <param name="eLevel"></param>
        /// <param name="curTime"></param>
        /// <param name="nLogID"></param>
        /// <param name="strMsg"></param>
        /// <returns></returns>
        private static string FormatString(ENUM_LogLevel eLevel, DateTime curTime, 
            int nLogID, string strCustomMsg, string strClassName, string strThreadName,
            string strFullName, string strFileName, int nLine, string strTraceStack)
        {
            string strLevel = "";
            if (eLevel == ENUM_LogLevel.eLogLevel_Debug)
                strLevel = "[DEBUG]";
            if (eLevel == ENUM_LogLevel.eLogLevel_Error)
                strLevel = "[ERROR]";
            if (eLevel == ENUM_LogLevel.eLogLevel_Info)
                strLevel = "[INFO]";
            if (eLevel == ENUM_LogLevel.eLogLevel_Warning)
                strLevel = "[WARNING]";

            if (nLogID >= 999990)
            {
                nLogID = 0;
            }

            string strTime = curTime.ToString("yyyy-MM-dd HH:mm:ss,fff");

            string strRet = strTime + "の" + strLevel + "の"
                + "[" + strClassName + "]:"
                + "[" + strThreadName + "]:"
                + "[" + strFullName + "(" + strFileName + ":" + nLine + ")]:"
                + strCustomMsg + "-"
                + strTraceStack + "の";
            return strRet;
        }

        /// <summary>
        /// 将自定义LogLevel转译为C#系统LogLevel
        /// </summary>
        /// <param name="eLevel"></param>
        /// <returns></returns>
        private static TraceEventType GetFormTypeAnalyzer(ENUM_LogLevel eLevel)
        {
            TraceEventType e = TraceEventType.Information;
            if (eLevel == ENUM_LogLevel.eLogLevel_Debug)
                e = TraceEventType.Verbose;
            if (eLevel == ENUM_LogLevel.eLogLevel_Error)
                e = TraceEventType.Error;
            if (eLevel == ENUM_LogLevel.eLogLevel_Warning)
                e = TraceEventType.Warning;
            if (eLevel == ENUM_LogLevel.eLogLevel_Info)
                e = TraceEventType.Information;

            return e;
        }

        /// <summary>
        /// 向富文本框添加一行文字
        /// </summary>
        /// <param name="color"></param>
        /// <param name="str"></param>
        private void LogAppendToForm(Color color, string str)
        {
            try
            {
                bool bIsNeedReturnFouse = false;
                if (m_LogRichTextBox == null)
                {
                    return;
                }
                if (m_CommandEditor != null)
                {
                    bIsNeedReturnFouse = m_CommandEditor.Focused;
                }
                // 滚动到最后一行
                m_LogRichTextBox.Focus();
                m_LogRichTextBox.Select(m_LogRichTextBox.TextLength, 0);
                m_LogRichTextBox.ScrollToCaret();

                m_LogRichTextBox.SelectionColor = color;
                m_LogRichTextBox.AppendText(str);
                m_LogRichTextBox.AppendText("\n");

                // 交还焦点
                if (bIsNeedReturnFouse)
                {
                    m_CommandEditor.Focus();
                }
            }
            catch
            {
                // 这里暂时不需要做处理，因为该函数调用频率很高，且重要性不高
            }
        }

        /// <summary>
        /// 写Log到IDE的输出面板
        /// </summary>
        /// <param name="eLevel"></param>
        /// <param name="nLogID"></param>
        /// <param name="strMsg"></param>
        /// <param name="strFinalMsg"></param>
        private void WriteToIDE(ENUM_LogLevel eLevel, int nLogID, string strMsg)
        {
            try
            {
                if (m_Trace == null)
                {
                    return;
                }
                m_Trace.TraceEvent(GetFormTypeAnalyzer(eLevel), nLogID, strMsg);
            }
            catch
            {
                // 这里暂时不需要做处理，因为该函数调用频率很高，且重要性不高
            }
        }

        /// <summary>
        /// 写Log到File文件中
        /// </summary>
        /// <param name="eLevel"></param>
        /// <param name="nLogID"></param>
        /// <param name="strMsg"></param>
        /// <param name="strFinalMsg"></param>
        private void WriteToFile(ENUM_LogLevel eLevel, string strFinalMsg)
        {
            try
            {
                if (((int)eLevel) < ((int)ENUM_LogLevel.eLogLevel_Info))
                {
                    return;
                }
                FKSystemFileSystemHelper.CreateDir(FKSystemFileSystemHelper.GetWorkdir() + "\\" + FKLogConsts.TEXT_LOG_DIR_NAME);
                using (StreamWriter w = File.AppendText(m_strTxtFileName))
                {
                    w.WriteLine(strFinalMsg);
                }
            }
            catch
            {
                // 这里暂时不需要做处理，因为该函数调用频率很高，且重要性不高
            }
        }

        /// <summary>
        /// 写Log到Form中显示
        /// </summary>
        /// <param name="eLevel"></param>
        /// <param name="nLogID"></param>
        /// <param name="strMsg"></param>
        /// <param name="strFinalMsg"></param>
        private void WriteToForm(ENUM_LogLevel eLevel, DateTime curTime, int nLogID, string strMsg, string strFinalMsg, int nTaskID)
        {
            try
            {
                if (m_LogRichTextBox == null)
                {
                    return;
                }

                // 官方标准Log格式太难看了。。。我还是自己设计一个form显示的吧。。。
                string strLevel = "";
                if (eLevel == ENUM_LogLevel.eLogLevel_Info) strLevel = "[INFO]";
                if (eLevel == ENUM_LogLevel.eLogLevel_Error) strLevel = "[EROR]";
                if (eLevel == ENUM_LogLevel.eLogLevel_Warning) strLevel = "[WARN]";
                string strTime = curTime.ToString("yyyy-MM-dd|HH:mm:ss");
                string strFormMsg = nLogID.ToString("000000") + "|" + strTime + "|" + strLevel + "|" + nTaskID + "|"  + strMsg;

                LogAppendDelegate la = new LogAppendDelegate(LogAppendToForm);
                if (eLevel == ENUM_LogLevel.eLogLevel_Error)
                {
                    m_LogRichTextBox.Invoke(la, Color.Red, strFormMsg);
                }
                else if (eLevel == ENUM_LogLevel.eLogLevel_Warning)
                {
                    m_LogRichTextBox.Invoke(la, Color.Yellow, strFormMsg);
                }
                else if (eLevel == ENUM_LogLevel.eLogLevel_Info)
                {
                    m_LogRichTextBox.Invoke(la, Color.White, strFormMsg);
                }
            }
            catch
            {
                // 这里暂时不需要做处理，因为该函数调用频率很高，且在WriteToDB处已做处理
            }
        }

        /// <summary>
        /// 写Log到DB中
        /// </summary>
        /// <param name="eLevel"></param>
        /// <param name="curTime"></param>
        /// <param name="nLogID"></param>
        /// <param name="strMsg"></param>
        /// <param name="strFinalMsg"></param>
        /// <param name="nTaskID"></param>
        private void WriteToDB(ENUM_LogLevel eLevel, DateTime curTime, int nLogID, string strMsg, string strFinalMsg, int nTaskID)
        {
            try
            {
                if (((int)eLevel) < ((int)ENUM_LogLevel.eLogLevel_Warning))
                {
                    return;
                }
                if (m_DBLog == null)
                {
                    return;
                }
                FKSQLiteLogNode node = new FKSQLiteLogNode();
                node.LogId      = m_nLogID;
                node.LogTime    = curTime;
                node.LogLevel   = (int)eLevel;
                node.LogInfo    = strMsg;
                node.TaskID     = nTaskID;
                m_DBLog.AddLogToDB(node);
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] Write a log to DB failed. Error = {e.Message}, Msg = {strFinalMsg}, TaskID = {nTaskID}");
            }
        }

        #endregion ==== 内部函数 ====
    }
}