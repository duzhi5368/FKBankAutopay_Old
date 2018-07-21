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
// Create Time         :    2017/7/13 13:26:23
// Update Time         :    2017/7/13 13:26:23
// Class Version       :    v1.0.0.0
// Class Description   :    系统环境设置
// ===============================================================================
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
// ===============================================================================
namespace FKBaseUtils
{
    public static class FKSystemEnviSettingHelper
    {
        #region ==== WIN32 API ====

        [DllImport("kernel32.dll")]
        private static extern uint SetThreadExecutionState(ExecutionFlag flags);
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(int Description, int ReservedValue);

        [Flags]
        enum ExecutionFlag : uint
        {
            System = 0x00000001,
            Display = 0x00000002,
            Continus = 0x80000000,
        }

        #endregion ==== WIN32 API ====

        /// <summary>
        ///阻止系统休眠，直到线程结束恢复休眠策略
        /// </summary>
        /// <param name="includeDisplay">是否阻止关闭显示器</param>
        public static void PreventSystemSleep(bool bIsIncludeDisplay = false)
        {
            if (bIsIncludeDisplay)
                SetThreadExecutionState(ExecutionFlag.System | ExecutionFlag.Display | ExecutionFlag.Continus);
            else
                SetThreadExecutionState(ExecutionFlag.System | ExecutionFlag.Continus);
        }

        /// <summary>
        ///恢复系统休眠策略
        /// </summary>
        public static void ResotreSystemSleep()
        {
            SetThreadExecutionState(ExecutionFlag.Continus);
        }

        /// <summary>
        /// 本机是否可以连接互联网
        /// </summary>
        /// <returns></returns>
        public static bool IsCanConnectInternet()
        {
            try
            {
                int des = 0;
                return InternetGetConnectedState(des, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Get internet connect state failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 获取本地IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Get local IP failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取IP（针对多网卡情况做出支持）
        /// </summary>
        /// <param name="targetIP"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static string GetLocalIPAddressByTargetIP(string targetIP, string port="443")
        {
            string localIP = string.Empty;
            try
            {
                TcpClient c = new TcpClient();
                c.Connect(targetIP, Int32.Parse(port));
                localIP = ((IPEndPoint)c.Client.LocalEndPoint).Address.ToString();
                c.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Get local IP failed. targetIP = {targetIP}, targetPort = {port}, Error = {e.ToString()}");
                return string.Empty;
            }
            return localIP;
        }

        /// <summary>
        /// 获取IP（针对多网卡情况做出支持）
        /// </summary>
        /// <param name="targetUri"></param>
        /// <returns></returns>
        public static string GetLocalIPAddressByUri(string targetUri)
        {
            string localIP = string.Empty;
            try
            {
                Uri uri = new Uri(targetUri);
                int port = uri.Port;

                Match m = Regex.Match(targetUri, @"\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}");
                string targetIP = string.Empty;
                if (m.Success)
                {
                    targetIP = m.Value;
                }
                else
                {
                    // 非IP，使用域名获取
                    targetIP = uri.Host;
                }

                TcpClient c = new TcpClient();
                c.Connect(targetIP, port);
                localIP = ((IPEndPoint)c.Client.LocalEndPoint).Address.ToString();
                c.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Get local IP failed. targetUri = {targetUri}, Error = {e.ToString()}");
                return string.Empty;
            }
            return localIP;
        }

        /// <summary>
        /// 获取本机.net版本
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDotNetVersion()
        {
            List<string> retVersionList = new List<string>();
            try
            {
                using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                {
                    foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                    {
                        if (versionKeyName.StartsWith("v"))
                        {
                            RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                            string name = (string)versionKey.GetValue("Version", "");
                            string sp = versionKey.GetValue("SP", "").ToString();
                            string install = versionKey.GetValue("Install", "").ToString();

                            if (!string.IsNullOrEmpty(install))
                            {
                                if ((!string.IsNullOrEmpty(sp)) && install == "1")
                                {
                                    retVersionList.Add(versionKeyName + " " + name + "  SP" + sp);
                                }
                            }

                            if (!string.IsNullOrEmpty(name))
                            {
                                continue;
                            }

                            foreach (string subKeyName in versionKey.GetSubKeyNames())
                            {
                                RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                                name = (string)subKey.GetValue("Version", "");
                                if (!string.IsNullOrEmpty(name))
                                {
                                    sp = subKey.GetValue("SP", "").ToString();
                                }
                                install = subKey.GetValue("Install", "").ToString();
                                if (string.IsNullOrEmpty(install))
                                {
                                    retVersionList.Add(versionKeyName + " " + name);
                                }
                                else
                                {
                                    if ((!string.IsNullOrEmpty(sp)) && install == "1")
                                    {
                                        retVersionList.Add(subKeyName + " " + name + " SP" + sp);
                                    }
                                    else if (install == "1")
                                    {
                                        retVersionList.Add(subKeyName + " " + name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] Get local dotnet version failed. Error = {e.ToString()}");
                return null;
            }
            return retVersionList;
        }

        /// <summary>
        /// 设置本程序启动执行
        /// </summary>
        /// <param name="bIsAutoStartup"></param>
        /// <returns></returns>
        public static bool SetAutoStartup(bool bIsAutoStartup)
        {
            try
            {
                string strCallingExeName = Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().Location);
                string strWorkdir = FKSystemFileSystemHelper.GetExeFilePath();
                RegistryKey parentKey = Registry.CurrentUser;
                RegistryKey subKey = parentKey.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");

                subKey.DeleteValue(strCallingExeName, false); // 先删再写，目的是覆盖
                if (bIsAutoStartup)
                {
                    subKey.SetValue(strCallingExeName, strWorkdir);
                }
                subKey.Close();
                parentKey.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Set app auto run mode failed. Error = {e.ToString()}");
                return false;
            }
        }
    }
}