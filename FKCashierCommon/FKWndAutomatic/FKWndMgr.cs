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
// Create Time         :    2017/7/15 15:38:21
// Update Time         :    2017/7/15 15:38:21
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
// ===============================================================================
namespace FKWndAutomatic
{
    public class FKWndMgr
    {
        /// <summary>
        /// 内部查询到一个窗口的基本信息
        /// </summary>
        internal struct SBaseWindowInfo
        {
            public IntPtr hWnd;
            public string szWindowName;
            public string szClassName;
        }
        /// <summary>
        /// 光标在一个窗口中的位置枚举
        /// </summary>
        public enum ENUM_CursorPosInWnd
        {
            eCursorPosInWnd_Auto = 0,           // 默认位置
            eCursorPosInWnd_FirstPos = 1,       // 窗口首位（最左边）
            eCursorPosInWnd_LastPos = 2,        // 窗口末位（最右边）
        };

        const int DEFAULT_IDLE_TIME_MILLISECOND = 50;


        private string m_strNeedToFindWndClassName = "";            // 需要查找的窗口类名
        private string m_strNeedToFindWndTitleName = "";            // 需要查找的窗口标题
        private bool   m_bIsSingleWnd = false;                      // 是否没有兄弟窗口
        private bool   m_bIsIngoreCase = true;                      // 是否忽略大小写检查
        private IntPtr m_pResultWndHandle = IntPtr.Zero;            // 最终查找到的窗口句柄

        public FKWndMgr()
        {
            m_pResultWndHandle = IntPtr.Zero;
        }

        #region ==== 对外接口 ====

        /// <summary>
        /// 查找指定窗口
        /// </summary>
        /// <param name="strRootWndTitle">需查找的窗口的根窗口标题, 若为 空  则表示不限制根窗口</param>
        /// <param name="strWndTitleName">需查找的窗口的窗口标题， 允许为空</param>
        /// <param name="strWndClassName">需朝赵的窗口的窗口类名， 允许为空</param>
        /// <param name="bIsIngoreCase">是否忽略大小写检查</param>
        /// <returns></returns>
        public IntPtr FindWindowHandler(string strRootWndTitle, string strWndTitleName, 
            string strWndClassName, bool bIsIngoreCase, bool bIsSingleWnd)
        {
            try
            {
                // 初始化查询条件信息
                m_strNeedToFindWndClassName = strWndClassName;
                m_strNeedToFindWndTitleName = strWndTitleName;
                m_bIsIngoreCase = bIsIngoreCase;
                m_pResultWndHandle = IntPtr.Zero;
                m_bIsSingleWnd = bIsSingleWnd;

                // 获取桌面窗口列表组
                SBaseWindowInfo[] topWindowList = GetAllDesktopWindows();
                for (int i = 0; i < topWindowList.Length; i++)
                {
                    if (string.IsNullOrEmpty(strRootWndTitle) ||                    // 不限制根窗口
                        topWindowList[i].szWindowName.Contains(strRootWndTitle))    // 找指定名浏览器父窗口
                    {
                        // 遍历子窗口
                        Win32API.EnumChildWindows(topWindowList[i].hWnd, Func_WindowInfoCompare, IntPtr.Zero);
                    }
                }
                return m_pResultWndHandle;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WindowMgr find window's handler failed. Error = {e.ToString()}");
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// 检查一个指定标题名的窗口是否存在
        /// </summary>
        /// <param name="strWndName"></param>
        /// <returns></returns>
        public static bool IsWndExistByTitle(string strWndName)
        {
            IntPtr pWndHandle = IntPtr.Zero;
            try
            {
                StringBuilder strTmpWndName = new StringBuilder(1024);
                Win32API.EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
                {
                    Win32API.GetWindowTextW(hWnd, strTmpWndName, strTmpWndName.Capacity);
                    if (string.Compare(strTmpWndName.ToString(), strWndName, true) == 0)
                    {
                        pWndHandle = hWnd;
                        return false;        // Warning: 匿名委托函数, 这个return不是IsWndExistByTitle的return...修改前请看清楚
                    }
                    return true;             // Warning: 匿名委托函数, 这个return不是IsWndExistByTitle的return...修改前请看清楚
                }, IntPtr.Zero);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WindowMgr find window's handler failed. Error = {e.ToString()}");
                return false;
            }

            return (pWndHandle != IntPtr.Zero);
        }

        /// <summary>
        /// 强制启用窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static bool EnableWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                return false;

            bool bRet = false;
            try
            {
                Win32API.EnableWindow(hWnd, true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WindowMgr enable window failed. Error = {e.ToString()}");
                return false;
            }
            return bRet;
        }
        
        /// <summary>
        /// 强制激活指定窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static bool FocusWindowActive(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                return false;

            try
            {
                if(Win32API.ShowWindow(hWnd, Win32API.WINDOWS_SW_SHOWNORMAL) == IntPtr.Zero)
                {
                    return false;
                }
                if (!Win32API.SetForegroundWindow(hWnd))
                {
                    return false;
                }
                if (Win32API.SetFocus(hWnd) == IntPtr.Zero)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WindowMgr set window focus failed. Error = {e.ToString()}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取父类窗口句柄
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static IntPtr GetParentWndHandle(IntPtr hWnd)
        {
            IntPtr ret = IntPtr.Zero;
            try
            {
                ret = Win32API.GetParent(hWnd);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WindowMgr get window parent's handler failed. Error = {e.ToString()}");
                return IntPtr.Zero;
            }
            return ret;
        }

        /// <summary>
        /// 设置光标在窗口中的位置
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="positionType"></param>
        public static void SetCursorPosInWnd(IntPtr hWnd, ENUM_CursorPosInWnd positionType)
        {
            try { 
                if (hWnd == IntPtr.Zero)
                    return;

                Win32API.Rect rect = new Win32API.Rect();
                Win32API.GetWindowRect(hWnd, ref rect);

                int param = (rect.Left + 1) + ((rect.Top + rect.Bottom) / 2) << 16;
                Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                Win32API.SendMessage(hWnd, Win32API.WINDOWS_WM_LBUTTONDOWN, param, param);
                Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
                Win32API.SendMessage(hWnd, Win32API.WINDOWS_WM_LBUTTONUP, param, param);
                Thread.Sleep(DEFAULT_IDLE_TIME_MILLISECOND);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WindowMgr set cursor to window failed. Error = {e.ToString()}");
                return;
            }
        }

        #endregion ==== 对外接口 ====

        #region ==== 内部函数 ====

        /// <summary>
        /// 遍历获取全部1阶窗口（桌面窗口）信息
        /// </summary>
        /// <returns></returns>
        private static SBaseWindowInfo[] GetAllDesktopWindows()
        {
            List<SBaseWindowInfo> WindowInfosList = new List<SBaseWindowInfo>();
            try
            {
                StringBuilder tmpSB = new StringBuilder(1024);

                Win32API.EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
                {
                    SBaseWindowInfo curWndInfo = new SBaseWindowInfo();
                    curWndInfo.hWnd = hWnd;
                    Win32API.GetWindowTextW(hWnd, tmpSB, tmpSB.Capacity);
                    curWndInfo.szWindowName = tmpSB.ToString();
                    Win32API.GetClassNameW(hWnd, tmpSB, tmpSB.Capacity);
                    curWndInfo.szClassName = tmpSB.ToString();

                    WindowInfosList.Add(curWndInfo);
                    return true;
                }, IntPtr.Zero);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WindowMgr get desktop windows handler failed. Error = {e.ToString()}");
                return null;
            }

            return WindowInfosList.ToArray();
        }

        /// <summary>
        /// 检查一个窗口类名是否和指定字符串匹配
        /// </summary>
        /// <param name="hSrcWnd"></param>
        /// <param name="strDest"></param>
        /// <param name="bIsIngoreCase"></param>
        /// <returns></returns>
        private bool IsWndClassNameCompare(IntPtr hSrcWnd, string strDest, bool bIsIngoreCase = true)
        {
            try
            {
                StringBuilder tmpSB = new StringBuilder(1024);
                int nRet = Win32API.GetClassNameW(hSrcWnd, tmpSB, tmpSB.Capacity);
                if (nRet == 0)
                    return false;

                return (string.Compare(tmpSB.ToString(), m_strNeedToFindWndClassName, bIsIngoreCase) == 0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WindowMgr compare windows class name failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 检查一个窗口标题是否和指定字符串匹配
        /// </summary>
        /// <param name="hSrcWnd"></param>
        /// <param name="strDest"></param>
        /// <param name="bIsIngoreCase"></param>
        /// <returns></returns>
        private bool IsWndTitleCompare(IntPtr hSrcWnd, string strDest, bool bIsIngoreCase = true)
        {
            try
            {
                StringBuilder tmpSB = new StringBuilder(1024);
                int nRet = Win32API.GetWindowTextW(hSrcWnd, tmpSB, tmpSB.Capacity);

                if (nRet == 0)
                {
                    return false;
                }
                    
                return (string.Compare(tmpSB.ToString(), m_strNeedToFindWndTitleName, bIsIngoreCase) == 0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WindowMgr compare windows title name failed. Error = {e.ToString()}");
                return false;
            }
        }
        
        /// <summary>
        /// 检查一个窗口是否是孤窗口（无兄弟窗口）
        /// </summary>
        /// <param name="hSrcWnd"></param>
        /// <returns></returns>
        private bool IsSingleWnd(IntPtr hSrcWnd)
        {
            if( Win32API.GetWindow(hSrcWnd, 2) == IntPtr.Zero
                && Win32API.GetWindow(hSrcWnd, 3) == IntPtr.Zero)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 回调：检查一个窗口是否符合指定限制
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lparam"></param>
        /// <returns></returns>
        private bool Func_WindowInfoCompare(IntPtr hWnd, IntPtr lparam)
        {
            try
            {
                // 无条件限制的检查，为避免无谓循环检查，直接返回
                if (string.IsNullOrEmpty(m_strNeedToFindWndClassName) && string.IsNullOrEmpty(m_strNeedToFindWndTitleName))
                {
                    return false;
                }

                // 需要进行双向检查
                if (!string.IsNullOrEmpty(m_strNeedToFindWndClassName) && !string.IsNullOrEmpty(m_strNeedToFindWndTitleName))
                {
                    if(IsWndClassNameCompare(hWnd, m_strNeedToFindWndClassName, m_bIsIngoreCase) &&
                       IsWndTitleCompare(hWnd, m_strNeedToFindWndTitleName, m_bIsIngoreCase))
                    {
                        if(m_bIsSingleWnd && IsSingleWnd(hWnd))
                        {
                            m_pResultWndHandle = hWnd;  // 孤窗口判定合理
                            return false;
                        }
                        else if((!m_bIsSingleWnd) && (!IsSingleWnd(hWnd)))
                        {
                            m_pResultWndHandle = hWnd;  // 强制非孤窗口判定合理
                            return false;
                        }
                        else
                        {
                            return true;
                        }

                    }
                    // 不符合规则
                    return true;
                }

                // 如果是要根据窗口类名进行查找
                if (!string.IsNullOrEmpty(m_strNeedToFindWndClassName))
                {
                    if (!IsWndClassNameCompare(hWnd, m_strNeedToFindWndClassName, m_bIsIngoreCase))
                    {
                        return true;    // 类名不对，继续查找
                    }


                    if (m_bIsSingleWnd && IsSingleWnd(hWnd))
                    {
                        m_pResultWndHandle = hWnd;  // 孤窗口判定合理
                        return false;
                    }
                    else if ((!m_bIsSingleWnd) && (!IsSingleWnd(hWnd)))
                    {
                        m_pResultWndHandle = hWnd;  // 强制非孤窗口判定合理
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                // 如果是要根据窗口标题名进行查找
                if (!string.IsNullOrEmpty(m_strNeedToFindWndTitleName))
                {
                    if (!IsWndTitleCompare(hWnd, m_strNeedToFindWndTitleName, m_bIsIngoreCase))
                    {
                        return true;    // 名字不对，继续查找
                    }


                    if (m_bIsSingleWnd && IsSingleWnd(hWnd))
                    {
                        m_pResultWndHandle = hWnd;  // 孤窗口判定合理
                        return false;
                    }
                    else if ((!m_bIsSingleWnd) && (!IsSingleWnd(hWnd)))
                    {
                        m_pResultWndHandle = hWnd;  // 强制非孤窗口判定合理
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WindowMgr compare windows infos failed. Error = {e.ToString()}");
                return true;
            }

            return true;    // 表示没有找到，继续遍历
        }

        #endregion ==== 内部函数 ====
    }
}