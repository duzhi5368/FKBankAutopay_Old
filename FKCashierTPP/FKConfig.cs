﻿/* 
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
// Create Time         :    2017/7/16 17:19:55
// Update Time         :    2017/7/16 17:19:55
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System.Configuration;
// ===============================================================================
namespace FKCashierTPP
{
    public class FKConfig
    {
        public static bool IsHideConsole
        {
            get
            {
                return ConfigurationManager.AppSettings["IsHideConsole"] == "true";
            }
        }
        public static bool IsHideForm
        {
            get
            {
                return ConfigurationManager.AppSettings["IsHideForm"] == "true";
            }
        }
        public static bool IsAutoStart
        {
            get
            {
                return ConfigurationManager.AppSettings["IsAutoStart"] == "true";
            }
        }
        public static bool IsAutoRestart
        {
            get
            {
                return ConfigurationManager.AppSettings["IsAutoRestart"] == "true";
            }
        }
        public static bool IsUseHttpServer
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["IsUseHttpServer"] != "false";
                }
                catch { return true; }
            }
        }
        public static string WebServerListenUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["WebServerListenUrl"];
            }
        }
        public static int LoginThreadIdleTime
        {
            get
            {
                int nRet = 0;
                bool b = int.TryParse(ConfigurationManager.AppSettings["LoginThreadIdleTime"], out nRet);
                if (b)
                    return nRet;
                else
                    return 3000;
            }
        }
        public static int ConnectServerTimeout
        {
            get
            {
                int nRet = 0;
                bool b = int.TryParse(ConfigurationManager.AppSettings["ConnectServerTimeout"], out nRet);
                if (b)
                    return nRet;
                else
                    return 3000;
            }
        }
        public static string ServerReviceOutcomeResultUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ServerReviceOutcomeResultUrl"];
            }
        }
        public static string ServerReviceTPPBillResultUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ServerReviceTPPBillResultUrl"];
            }
        }
        public static string ServerRegisteUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ServerRegisteUrl"];
            }
        }
        public static string ServerLogoutUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ServerLogoutUrl"];
            }
        }
        public static string ServerHeartbeatUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ServerHeartbeatUrl"];
            }
        }
        public static string LocalIP
        {
            get
            {
                return ConfigurationManager.AppSettings["LocalIP"];
            }
        }
        public static string BankCardNo
        {
            get
            {
                return ConfigurationManager.AppSettings["BankCardNo"];
            }
        }
        public static int HeartbeatIdleTime
        {
            get
            {
                int nRet = 0;
                bool b = int.TryParse(ConfigurationManager.AppSettings["HeartbeatIdleTime"], out nRet);
                if (b)
                    return nRet;
                else
                    return 30000;
            }
        }
        public static bool IsUsePing
        {
            get
            {
                return ConfigurationManager.AppSettings["IsUsePing"] == "true";
            }
        }
        public static bool IsUseHttpsWebServer
        {
            get
            {
                return ConfigurationManager.AppSettings["IsUseHttpsWebServer"] == "true";
            }
        }
    }
}