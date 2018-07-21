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
// Create Time         :    2017/7/13 17:55:16
// Update Time         :    2017/7/13 17:55:16
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
// ===============================================================================
namespace FKLog
{
    public static class FKLogger
    {
        public static void DEBUG(string strMsg)
        {
            FKLogImp.GetInstance.AddLog(FKLogImp.ENUM_LogLevel.eLogLevel_Debug, strMsg, 0);
        }
        public static void INFO(string strMsg)
        {
            FKLogImp.GetInstance.AddLog(FKLogImp.ENUM_LogLevel.eLogLevel_Info, strMsg, 0);
        }
        public static void WARN(string strMsg, int nTaskID = 0)
        {
            FKLogImp.GetInstance.AddLog(FKLogImp.ENUM_LogLevel.eLogLevel_Warning, strMsg, nTaskID);
        }
        public static void ERROR(string strMsg, int nTaskID = 0)
        {
            FKLogImp.GetInstance.AddLog(FKLogImp.ENUM_LogLevel.eLogLevel_Error, strMsg, nTaskID);
        }
        public static void LOG(FKLogImp.ENUM_LogLevel eLevel, string strMsg, int nTaskID)
        {
            FKLogImp.GetInstance.AddLog(eLevel, strMsg, nTaskID);
        }
        public static string GetLogByTaskID(int nTaskID, string DBFileName)
        {
            return FKLogImp.GetInstance.GetLogByTaskID(nTaskID, DBFileName);
        }
        public static int GetLogNumByTaskID(int nTaskID, string DBFileName)
        {
            return FKLogImp.GetInstance.GetLogNumByTaskID(nTaskID, DBFileName);
        }
    }
}