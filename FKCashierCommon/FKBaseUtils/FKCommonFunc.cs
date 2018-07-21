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
// Create Time         :    2017/7/13 13:30:36
// Update Time         :    2017/7/13 13:30:36
// Class Version       :    v1.0.0.0
// Class Description   :    已经难以分类的基础功能函数
// ===============================================================================
using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
// ===============================================================================
namespace FKBaseUtils
{
    public static class FKCommonFunc
    {
        #region ==== WIN32 API ====

        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("User32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        #endregion ==== WIN32 API ====

        /// <summary>
        /// 新线程打开Notepad
        /// </summary>
        /// <returns></returns>
        public static bool OpenNotepadInNewThread()
        {
            try
            {
                Thread newThread = new Thread(delegate ()
                {
                    Process p = null;
                    try
                    {
                        p = new Process();
                        p.StartInfo.FileName = "notepad";
                        p.Start();
                        p.WaitForExit();
                    }
                    catch (Exception e)
                    {
                        if (p != null)
                        {
                            p.Close();
                            p.Dispose();
                        }
                        throw e;    // 向上抛出错误
                    }
                });
                newThread.Start();

                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] Open Notepad exception occurred. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 判断远程主机是否存在
        /// </summary>
        /// <param name="strRomateIP"></param>
        /// <returns></returns>
        public static bool IsRomateExistByPingTest(string strRomateIP)
        {
            try
            {
                IPAddress   ip = IPAddress.Parse(strRomateIP);
                Ping        pingContorl = new Ping();
                PingOptions objOptions = new PingOptions();
                objOptions.DontFragment = true;
                string      data = "";
                byte[]      buffer = Encoding.UTF8.GetBytes(data);
                int         nTimeout = 120;

                PingReply reply = pingContorl.Send(ip, nTimeout, buffer, objOptions);
                pingContorl.Dispose();

                return (reply.Status == IPStatus.Success);
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] Try to ping romate ip failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 检查指定Url是否可以接受Http请求
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static bool IsAcceptHTTPRequest(string strUrl)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "HEAD";
                request.Timeout = 10000;    // TODO: 这里强制等待10秒不太好，可以考虑更短或者读取配置
                request.UseDefaultCredentials = true;
                request.AllowAutoRedirect = false;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                bool bRet = (response.StatusCode == HttpStatusCode.OK);
                response.Close();

                return bRet;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Url's http test failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 延迟一段时间执行指定程序
        /// </summary>
        /// <param name="nSeconds"></param>
        /// <returns></returns>
        public static bool StartAppInDelay(int nSeconds, string strExePath)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                string strArgment = string.Format("/C ping 127.0.0.1 -n {0} && \"{1}\"", nSeconds, strExePath);
                info.Arguments = strArgment;
                info.CreateNoWindow = true;
                info.FileName = "cmd.exe";
                Process.Start(info);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Start app in delay failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 重调整控制台窗口大小
        /// </summary>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <returns></returns>
        public static bool ResizeConsole(int nWidth = 100, int nHeight = 50)
        {
            try
            {
                Console.SetWindowSize(nWidth, nHeight);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Set console window's size failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 隐藏控制台
        /// </summary>
        /// <returns></returns>
        public static bool HideConsole()
        {
            try
            {
                IntPtr hWnd = FindWindow("ConsoleWindowClass", Console.Title);
                if (hWnd != IntPtr.Zero)
                {
                    ShowWindow(hWnd, 0);
                    return true;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Hide console window failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 显示控制台
        /// </summary>
        /// <returns></returns>
        public static bool ShowConsole()
        {
            try
            {
                IntPtr hWnd = FindWindow("ConsoleWindowClass", Console.Title);
                if (hWnd != IntPtr.Zero)
                {
                    ShowWindow(hWnd, 5);    // 5 = SW_SHOWNORAML
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Show console window failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 新线程执行一个控制台命令
        /// </summary>
        /// <param name="strExeFile"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public static string RunConsoleCommand(string strExeFile, string cmdParams)
        {
            Process cmdProcess = null;
            string strRet = string.Empty;
            try
            {
                cmdProcess = new Process();
                cmdProcess.StartInfo.FileName = "cmd.exe";
                cmdProcess.StartInfo.UseShellExecute = false;
                cmdProcess.StartInfo.RedirectStandardInput = true;
                cmdProcess.StartInfo.RedirectStandardOutput = true;
                cmdProcess.StartInfo.RedirectStandardError = true;
                cmdProcess.StartInfo.CreateNoWindow = true;
                cmdProcess.Start();

                //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                string str = string.Format(@"""{0}"" {1} {2}", strExeFile, cmdParams, "&exit");

                cmdProcess.StandardInput.WriteLine(str);
                cmdProcess.StandardInput.AutoFlush = true;
                strRet = cmdProcess.StandardOutput.ReadToEnd();
                cmdProcess.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Run new command failed. Exe = {strExeFile}, Params = {cmdParams}, Error = {e.ToString()}");
            }
            finally
            {
                if (cmdProcess != null)
                    cmdProcess.Close();
            }
            return strRet;
        }
    }
}