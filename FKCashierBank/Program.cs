
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using LOGGER = FKLog.FKLogger;

namespace FKCashierBank
{
    static class Program
    {
        #region WIN32接口

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler hHandler, bool add);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        #endregion

        static MainForm s_MainForm = null;
        static EventHandler s_ConsoleEventHandler = null;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            // 初始化
            Console.WriteLine("[Info] Start ...");

            if (!ResetConsole()) { return; }
            if (!HandleProcessExist()) { return; }
            if (!SetMinidumpHandle()) { return; }
            if (!CheckDotNetVersion()) { return; }
            if (!SetAutoRestart()) { return; }

            s_MainForm = new MainForm();
            if (!ResetForm()) { return; }

            try
            {
                Application.Run(s_MainForm);
            }
            catch (Exception e)
            {
                LOGGER.ERROR($"Application has fetal error occured. Error = {e.ToString()}");
            }
            finally
            {
                AppShutdown(FKConfig.IsAutoRestart);
            }
        }

        /// <summary>
        /// 检查进程双开
        /// </summary>
        /// <returns></returns>
        public static bool HandleProcessExist()
        {
            // 禁止双开的标示
            bool bIsCreateNew = false;
            Mutex mutex = new Mutex(false, Application.ProductName, out bIsCreateNew);
            if (!bIsCreateNew)
            {
                // 已经双开
                Process instance = GetExistProcess();
                if (instance != null)
                {
                    ForceProcessForeground(instance);
                    Application.Exit();
                }
            }
            return true;
        }

        /// <summary>
        /// 检查程序是否已经在运行
        /// </summary>
        /// <returns></returns>
        private static Process GetExistProcess()
        {
            Process curProcess = Process.GetCurrentProcess();
            foreach (Process pro in Process.GetProcessesByName(curProcess.ProcessName))
            {
                if (pro.Id != curProcess.Id)
                {
                    return pro;
                }
            }
            return null;
        }

        /// <summary>
        /// 使程序前台显示
        /// </summary>
        /// <param name="instance"></param>
        private static void ForceProcessForeground(Process instance)
        {
            IntPtr mainFormHandle = instance.MainWindowHandle;
            if (mainFormHandle != IntPtr.Zero)
            {
                ShowWindowAsync(mainFormHandle, 1);
                SetForegroundWindow(mainFormHandle);
            }
        }

        /// <summary>
        /// 设置dump处理
        /// </summary>
        private static bool SetMinidumpHandle()
        {
            try
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
                AppDomain.CurrentDomain.UnhandledException += MiniDumpExceptionHandler;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Set minidump failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 异常Minidump回调处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MiniDumpExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            FKBaseUtils.FKMiniDump.CreateMiniDump();
            AppShutdown(FKConfig.IsAutoRestart);
        }

        /// <summary>
        /// 检查本地DotNet版本
        /// </summary>
        private static bool CheckDotNetVersion()
        {
            string strRet = string.Empty;
            List<string> NetFrameworkVersoin = FKBaseUtils.FKSystemEnviSettingHelper.GetDotNetVersion();
            foreach (string s in NetFrameworkVersoin)
            {
                strRet += s;
            }
            Console.WriteLine($"[Info] local dotnet version : {strRet}");
            return true;
        }

        /// <summary>
        /// 重置控制台窗口
        /// </summary>
        /// <returns></returns>
        private static bool ResetConsole()
        {
            if (FKConfig.IsHideConsole)
            {
                FKBaseUtils.FKCommonFunc.HideConsole();
            }
            else
            {
                FKBaseUtils.FKCommonFunc.ResizeConsole();
            }
            return true;
        }

        /// <summary>
        /// 设置Form状态
        /// </summary>
        /// <returns></returns>
        private static bool ResetForm()
        {
            if (FKConfig.IsHideForm)
            {
                s_MainForm.TopLevel = false;
            }
            return true;
        }

        /// <summary>
        /// 设置程序的自启动
        /// </summary>
        /// <returns></returns>
        private static bool SetAutoRestart()
        {
            FKBaseUtils.FKSystemEnviSettingHelper.SetAutoStartup(FKConfig.IsAutoRestart);
            return true;
        }

        /// <summary>
        /// 委托声明
        /// </summary>
        /// <param name="eEventType"></param>
        /// <returns></returns>
        private delegate bool EventHandler(ENUM_Console_CtrlType eEventType);
        /// <summary>
        /// 设置控制台消息处理函数
        /// </summary>
        /// <returns></returns>
        private static bool HandleConsoleEvent()
        {
            s_ConsoleEventHandler += new EventHandler(ConsoleCloseEventHandler);
            SetConsoleCtrlHandler(s_ConsoleEventHandler, true);
            return true;
        }

        /// <summary>
        /// 需要处理的Console消息枚举
        /// </summary>
        enum ENUM_Console_CtrlType
        {
            eConsoleCtrlType_Close_Event = 2,
            eConsoleCtrlType_Shutdown_Event = 6,
        }

        /// <summary>
        /// 控制台消息回调函数
        /// </summary>
        /// <param name="eEventType"></param>
        /// <returns></returns>
        private static bool ConsoleCloseEventHandler(ENUM_Console_CtrlType eEventType)
        {
            switch (eEventType)
            {
                case ENUM_Console_CtrlType.eConsoleCtrlType_Close_Event:
                    AppShutdown(FKConfig.IsAutoRestart);
                    return false;
                case ENUM_Console_CtrlType.eConsoleCtrlType_Shutdown_Event:
                    AppShutdown(FKConfig.IsAutoRestart);
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 关闭整个App，指定秒数后自动重新启动
        /// </summary>
        /// <param name="bIsNeedAutoRestart"></param>
        /// <param name="nSeconds"></param>
        private static void AppShutdown(bool bIsNeedAutoRestart, int nSeconds = 10)
        {
            if (s_MainForm != null)
            {
                s_MainForm.SafeShutdown();
            }

            if (bIsNeedAutoRestart)
            {
                // 开启新线程
                ProcessStartInfo info = new ProcessStartInfo();
                info.Arguments = "/C ping 127.0.0.1 -n " + nSeconds + " && \"" + Application.ExecutablePath + "\"";
                info.FileName = "cmd.exe";
                Process.Start(info);
            }

            // 强制全部关闭
            Environment.Exit(0);
        }
    }
}
