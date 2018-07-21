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
// Create Time         :    2017/7/15 15:38:48
// Update Time         :    2017/7/15 15:38:48
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
// #define USE_KEYBD_EVENT

using System;
using System.Threading;
// ===============================================================================
namespace FKWndAutomatic
{
    public static class FKWinIO
    {
        const int DEFAULT_IDLE_TIME_MILLISECOND = 50;

        #region ==== 内部函数 ====
        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        internal static bool Init()
        {
            try
            {
                if (!Win32API.InitializeWinIo())
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] WinIO init failed. Error = {e.ToString()}");
                return false;
            }
            KBCWait4IBE();
            return true;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        internal static void Shutdown()
        {
            try
            {
                Win32API.ShutdownWinIo();
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] WinIO shutdown failed. Error = {e.ToString()}");
            }
        }
        /// <summary>
        /// 清除缓冲
        /// </summary>
        internal static void KBCWait4IBE()
        {
            try
            {
                int dwVal = 0;
                do
                {
                    bool flag = Win32API.GetPortVal((IntPtr)0x64, out dwVal, 1);
                }
                while ((dwVal & 0x2) != 0/* > 0*/);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WinIO wait for IBE failed. Error = {e.ToString()}");
            }
        }
        /// <summary>
        /// 按键按下
        /// </summary>
        /// <param name="ch"></param>
        internal static void KeyDown(uint ch)
        {
            try
            {
                int btScancode = 0;
                btScancode = Win32API.MapVirtualKey(ch, 0);
                KBCWait4IBE();
                Win32API.SetPortVal(Win32API.WINDOWS_KEY_CMD, (IntPtr)0xD2, 1);
                KBCWait4IBE();
                Win32API.SetPortVal(Win32API.WINDOWS_KEY_DATA, (IntPtr)0xE2, 1); //(IntPtr)0x60, 1);
                KBCWait4IBE();
                Win32API.SetPortVal(Win32API.WINDOWS_KEY_CMD, (IntPtr)0xD2, 1);
                KBCWait4IBE();
                Win32API.SetPortVal(Win32API.WINDOWS_KEY_DATA, (IntPtr)btScancode, 1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WinIO key down failed. Error = {e.ToString()}");
            }
        }
        /// <summary>
        /// 按键松开
        /// </summary>
        /// <param name="ch"></param>
        internal static void KeyUp(uint ch)
        {
            try
            {
                int btScancode = 0;
                btScancode = Win32API.MapVirtualKey(ch, 0);
                KBCWait4IBE();
                Win32API.SetPortVal(Win32API.WINDOWS_KEY_CMD, (IntPtr)0xD2, 1);
                KBCWait4IBE();
                Win32API.SetPortVal(Win32API.WINDOWS_KEY_DATA, (IntPtr)0xE0, 1); //(IntPtr)0x60, 1);
                KBCWait4IBE();
                Win32API.SetPortVal(Win32API.WINDOWS_KEY_CMD, (IntPtr)0xD2, 1);
                KBCWait4IBE();
                Win32API.SetPortVal(Win32API.WINDOWS_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WinIO key up failed. Error = {e.ToString()}");
            }
        }
        /// <summary>
        /// 对需要shift支持的特殊字符的编码转换
        /// </summary>
        /// <param name="cIn"></param>
        /// <param name="cOut"></param>
        /// <param name="bIsUseShift"></param>
        internal static void TranslateShiftUse(char cIn, out char cOut, out bool bIsUseShift)
        {
            if (cIn >= 65 && cIn <= 90)
            {
                cOut = (char)(cIn + 32);
                bIsUseShift = true;
                return;
            }
            if (cIn == 126) // ~
            {
                cOut = (char)96;//`
                bIsUseShift = true;
                return;
            }
            if (cIn == 33) // !
            {
                cOut = (char)49;//1
                bIsUseShift = true;
                return;
            }
            if (cIn == 64) // @
            {
                cOut = (char)50;//2
                bIsUseShift = true;
                return;
            }
            if (cIn == 35) // #
            {
                cOut = (char)51;//3
                bIsUseShift = true;
                return;
            }
            if (cIn == 36) // $
            {
                cOut = (char)52;//4
                bIsUseShift = true;
                return;
            }
            if (cIn == 37) // %
            {
                cOut = (char)53;//5
                bIsUseShift = true;
                return;
            }
            if (cIn == 94) // ^
            {
                cOut = (char)54;//6
                bIsUseShift = true;
                return;
            }
            if (cIn == 38) // &
            {
                cOut = (char)55;//7
                bIsUseShift = true;
                return;
            }
            if (cIn == 42) // *
            {
                cOut = (char)56;//8
                bIsUseShift = true;
                return;
            }
            if (cIn == 40) // (
            {
                cOut = (char)57;//9
                bIsUseShift = true;
                return;
            }
            if (cIn == 41) // )
            {
                cOut = (char)48;//0
                bIsUseShift = true;
                return;
            }
            if (cIn == 95) // _
            {
                cOut = (char)45;//-
                bIsUseShift = true;
                return;
            }
            if (cIn == 43) // +
            {
                cOut = (char)61;//=
                bIsUseShift = true;
                return;
            }
            if (cIn == 123) // {
            {
                cOut = (char)91;// [
                bIsUseShift = true;
                return;
            }
            if (cIn == 125) // }
            {
                cOut = (char)93;// ]
                bIsUseShift = true;
                return;
            }
            if (cIn == 124) // |
            {
                cOut = (char)92;// \
                bIsUseShift = true;
                return;
            }
            if (cIn == 58) // :
            {
                cOut = (char)59;// ;
                bIsUseShift = true;
                return;
            }
            if (cIn == 34) // "
            {
                cOut = (char)39;// '
                bIsUseShift = true;
                return;
            }
            if (cIn == 60) // <
            {
                cOut = (char)44;// ,
                bIsUseShift = true;
                return;
            }
            if (cIn == 62) // >
            {
                cOut = (char)46;// .
                bIsUseShift = true;
                return;
            }
            if (cIn == 63) // ?
            {
                cOut = (char)47;// /
                bIsUseShift = true;
                return;
            }
            cOut = cIn;
            bIsUseShift = false;
        }

        #endregion ==== 内部函数 ====

        #region ==== 对外接口 ====

        /// <summary>
        /// 按键：Alt + key
        /// </summary>
        /// <param name="keycode">参见 System.Windows.Forms.Keys 定义</param>
        /// <returns></returns>
        public static bool AltKeyPress(int keycode)
        {
            try
            {
                if (!Init())
                {
                    return false;
                }
                
                Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                KeyDown((uint)System.Windows.Forms.Keys.Menu);    // Alt = System.Windows.Forms.Menu
                KeyDown((uint)keycode);
                Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                KeyUp((uint)keycode);
                KeyUp((uint)System.Windows.Forms.Keys.Menu);
                Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);

                Shutdown();
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] WinIO Alt+{keycode} failed. Error = {e.ToString()}");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 按键: Ctrl + key
        /// </summary>
        /// <param name="keycode">参见 System.Windows.Forms.Keys 定义</param>
        /// <returns></returns>
        public static bool CtrlKeyPress(int keycode)
        {
            try
            {
                if (!Init())
                {
                    return false;
                }
                Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                KeyDown((uint)System.Windows.Forms.Keys.ControlKey);
                KeyDown((uint)keycode);
                Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                KeyUp((uint)keycode);
                KeyUp((uint)System.Windows.Forms.Keys.ControlKey);
                Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);

                Shutdown();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WinIO Ctrl+{keycode} failed. Error = {e.ToString()}");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 输入字符
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bQuickMode">是否允许使用粘贴</param>
        /// <returns></returns>
        public static bool Input(string text, bool bQuickMode = false)
        {
            try
            {
                if (!Init())
                {
                    return false;
                }
                if ((!FKBaseUtils.FKStringHelper.IsHasChineseCode(text)) && (!bQuickMode))
                {
                    // 非中文等Unicode字符
                    foreach (char c in text)
                    {
                        char nTranslatedChar = (char)(0);
                        bool bIsUseShift = false;

                        // 转义
                        TranslateShiftUse((char)c, out nTranslatedChar, out bIsUseShift);

                        // 按下Shift
                        if (bIsUseShift)
                        {
#if USE_KEYBD_EVENT
                            Win32API.keybd_event((byte)System.Windows.Forms.Keys.ShiftKey, 0, 0, 0);
#else
                            KeyDown((uint)System.Windows.Forms.Keys.ShiftKey);
#endif
                        }
                        Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                        short sChar = Win32API.VkKeyScan(nTranslatedChar);
                        KeyDown((uint)sChar);
                        Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                        KeyUp((uint)sChar);
                        Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);

                        // 释放Shift
                        if (bIsUseShift)
                        {
#if USE_KEYBD_EVENT
                            Win32API.keybd_event((byte)System.Windows.Forms.Keys.ShiftKey, 0, 0x0002, 0);
#else
                            KeyUp((uint)System.Windows.Forms.Keys.ShiftKey);
#endif
                        }
                    }
                }
                else
                {
                    // 将Unicode复制到剪切板
                    System.Windows.Forms.Clipboard.Clear();
                    System.Windows.Forms.Clipboard.SetDataObject(text);

#if USE_KEYBD_EVENT
                    // 方式一黏贴
                    Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                    Win32API.keybd_event((byte)System.Windows.Forms.Keys.LControlKey, 0, 0, 0);  //按下左Control键。
                    Win32API.keybd_event((byte)System.Windows.Forms.Keys.V, 0, 0, 0);            //按下v
                    Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                    Win32API.keybd_event((byte)System.Windows.Forms.Keys.V, 0, 2, 0);            //放开v
                    Win32API.keybd_event((byte)System.Windows.Forms.Keys.LControlKey, 0, 2, 0);  //放开control
                    Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
#else
                    // 方式二黏贴
                    Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                    KeyDown((uint)System.Windows.Forms.Keys.ControlKey);
                    KeyDown((uint)System.Windows.Forms.Keys.V);
                    Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                    KeyUp((uint)System.Windows.Forms.Keys.V);
                    KeyUp((uint)System.Windows.Forms.Keys.ControlKey);
                    Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
#endif
                }

                Shutdown();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WinIO input string failed. Error = {e.ToString()}");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 按 指定键
        /// </summary>
        /// <param name="keycode">参见 System.Windows.Forms.Keys 定义</param>
        /// <returns></returns>
        public static bool KeyPress(int keycode)
        {
            try
            {
                if (!Init())
                {
                    return false;
                }
                Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                Char c = (Char)keycode;
                KeyDown(c);
                Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                KeyUp(c);
                Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);

                Shutdown();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WinIO press key failed. Error = {e.ToString()}");
                return false;
            }
            return true;
        }

        #endregion ==== 对外接口 ====
    }
}