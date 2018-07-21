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
// Create Time         :    2017/7/15 15:20:31
// Update Time         :    2017/7/15 15:20:31
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Runtime.InteropServices;
using System.Text;
// ===============================================================================
namespace FKWndAutomatic
{
    public static class Win32API
    {
        internal struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        };

        internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // windows硬件按键消息
        internal const int WINDOWS_KEY_CMD  = 0x64;
        internal const int WINDOWS_KEY_DATA = 0x60;

        // windows窗口类型消息
        internal const int WINDOWS_SW_SHOWNORMAL = 5;
        internal const int WINDOWS_SW_HIDE = 0;

        // windows按键消息类型
        internal const int WINDOWS_WM_MOUSEMOVE = 0x200;
        internal const int WINDOWS_WM_LBUTTONDOWN = 0x201;
        internal const int WINDOWS_WM_LBUTTONUP = 0x202;
        internal const int WINDOWS_WM_KEYDOWN = 0x0100;
        internal const int WINDOWS_WM_CHAR = 0x102;
        internal const int WINDOWS_WM_NCHITTEST = 0x0084;
        internal const int WINDOWS_WM_NCLBUTTONDOWN = 0x00A1;
        internal const int WINDOWS_WM_NCLBUTTONUP = 0x00A2;

        #region ==== KERNEL32 DLL =====
        [DllImport("kernel32.dll")]
        internal static extern uint GetLastError();
        #endregion ==== KERNEL32 DLL =====


        #region ==== USER32 DLL =====

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowTextA(IntPtr hwnd, StringBuilder lpString, long nMaxCount);

        [DllImport("User32.dll")]
        internal static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("User32.dll")]
        internal static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("User32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("User32.dll")]
        internal static extern IntPtr ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll")]
        internal static extern int IsWindowVisible(IntPtr hWnd);

        [DllImport("User32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        internal static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

        [DllImport("User32.dll")]
        internal static extern int GetWindowRect(IntPtr hWnd, ref Rect lpRect);

        [DllImport("User32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("User32.dll")]
        internal static extern int SetCursorPos(int x, int y);

        #endregion ==== USER32 DLL =====

        #region ==== USER32 DLL Used for WinIO ====

        [DllImport("User32.dll")]
        public static extern int MapVirtualKey(uint Ucode, uint uMapType);
        [DllImport("User32.dll")]
        public static extern short VkKeyScan(char key);

        [DllImport("User32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(
                                    byte bVk, // 虚拟键值  
                                    byte bScan,
                                    int dwFlags, // 这里是整数类型 0 为按下，2为释放  
                                    int dwExtraInfo
                                    );

        #endregion ==== USER32 DLL Used for WinIO ====

        #region ==== WINIO DLL =====

        [DllImport("WinIo32.dll")]
        public static extern bool InitializeWinIo();

        [DllImport("WinIo32.dll")]
        public static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal, byte bSize);

        [DllImport("WinIo32.dll")]
        public static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal, byte bSize);

        [DllImport("WinIo32.dll")]
        public static extern byte MapPhysToLin(byte pbPhysAddr, uint dwPhysSize, IntPtr PhysicalMemoryHandle);

        [DllImport("WinIo32.dll")]
        public static extern bool UnmapPhysicalMemory(IntPtr PhysicalMemoryHandle, byte pbLinAddr);

        [DllImport("WinIo32.dll")]
        public static extern bool GetPhysLong(IntPtr pbPhysAddr, byte pdwPhysVal);

        [DllImport("WinIo32.dll")]
        public static extern bool SetPhysLong(IntPtr pbPhysAddr, byte dwPhysVal);

        [DllImport("WinIo32.dll")]
        public static extern void ShutdownWinIo();

        #endregion ==== WINIO DLL =====
    }
}