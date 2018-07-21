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
// Create Time         :    2017/7/13 13:19:47
// Update Time         :    2017/7/13 13:19:47
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
// ===============================================================================
namespace FKBaseUtils
{
    /// <summary>
    /// WindowsMinidump类型
    /// </summary>
    // http://msdn.microsoft.com/en-us/library/windows/desktop/ms680519(v=vs.85).aspx
    [Flags]
    public enum MINIDUMP_TYPE
    {
        MiniDumpNormal = 0x00000000,
        MiniDumpWithDataSegs = 0x00000001,
        MiniDumpWithFullMemory = 0x00000002,
        MiniDumpWithHandleData = 0x00000004,
        MiniDumpFilterMemory = 0x00000008,
        MiniDumpScanMemory = 0x00000010,
        MiniDumpWithUnloadedModules = 0x00000020,
        MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
        MiniDumpFilterModulePaths = 0x00000080,
        MiniDumpWithProcessThreadData = 0x00000100,
        MiniDumpWithPrivateReadWriteMemory = 0x00000200,
        MiniDumpWithoutOptionalData = 0x00000400,
        MiniDumpWithFullMemoryInfo = 0x00000800,
        MiniDumpWithThreadInfo = 0x00001000,
        MiniDumpWithCodeSegs = 0x00002000,
        MiniDumpWithoutAuxiliaryState = 0x00004000,
        MiniDumpWithFullAuxiliaryState = 0x00008000,
        MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
        MiniDumpIgnoreInaccessibleMemory = 0x00020000,
        MiniDumpWithTokenInformation = 0x00040000,
        MiniDumpWithModuleHeaders = 0x00080000,
        MiniDumpFilterTriage = 0x00100000,
        MiniDumpValidTypeFlags = 0x001fffff
    }

    /// <summary>
    /// WindowsMiniDump信息格式
    /// </summary>
    // http://msdn.microsoft.com/en-us/library/windows/desktop/ms680366(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MINIDUMP_EXCEPTION_INFORMATION
    {
        public uint         ThreadId;
        public IntPtr       ExceptionPointers;
        public int          ClientPointers;
    }

    public static class FKMiniDump
    {
        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("DbgHelp.dll")]
        private static extern bool MiniDumpWriteDump(IntPtr hProcess, uint ProcessId,
            IntPtr hFile, int DumpType, ref MINIDUMP_EXCEPTION_INFORMATION ExceptionParam,
            IntPtr UserStreamParam, IntPtr CallbackParam);
        /// <summary>
        /// 创建一个MiniDump文件,用以调试分析
        /// </summary>
        public static void CreateMiniDump()
        {
            var proc = Process.GetCurrentProcess();
            var fileName = String.Format("FKMinidump_{0}_{1}.dmp", proc.ProcessName, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

            var info = new MINIDUMP_EXCEPTION_INFORMATION()
            {
                ThreadId            = GetCurrentThreadId(),
                ExceptionPointers   = Marshal.GetExceptionPointers(),
                ClientPointers      = 1
            };

            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                MiniDumpWriteDump(proc.Handle, (uint)proc.Id, fs.SafeFileHandle.DangerousGetHandle(),
                    (int)MINIDUMP_TYPE.MiniDumpWithFullMemory, ref info, IntPtr.Zero, IntPtr.Zero);
            }

            // Process.GetCurrentProcess().Kill();
        }
    }
}