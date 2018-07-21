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
// Create Time         :    2017/7/25 14:18:40
// Update Time         :    2017/7/25 14:18:40
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Diagnostics;
using System.Management;
// ===============================================================================
namespace FKBaseUtils
{
    public static class FKProcessHelper
    {
        /// <summary>
        /// 结束进程树
        /// </summary>
        /// <param name="parent">父进程</param>
        public static void KillProcessTree(this Process parent)
        {
            var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + parent.Id);
            var moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                Process childProcess = Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));    // 通过子进程ID获取该进程实例
                childProcess.KillProcessTree();                                                     // 调用拓展方法结束当前进程的所有子进程
            }

            // 不能结束自己
            if (parent.Id != Process.GetCurrentProcess().Id)
                parent.Kill();                                                                      // 结束当前进程
        }
        /// <summary>
        /// 结束指定进程和它的进程树（所有子进程）
        /// </summary>
        /// <param name="pid">进程Id</param>
        public static void KillProcessTreeById(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);  // 获取当前进程
                try
                {
                    process.KillProcessTree();              // 结束进程树
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            catch
            {
                Console.WriteLine($"Kill process tree failed, can't get the process ID =  {pid}");
                var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
                var moc = searcher.Get();
                foreach (ManagementObject mo in moc)
                {
                    Process childProcess = Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));    // 通过子进程ID获取该进程实例
                    childProcess.KillProcessTree();                                                     // 调用拓展方法结束当前进程的所有子进程
                }
            }
        }
    }
}