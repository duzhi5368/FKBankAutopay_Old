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
// Create Time         :    2017/7/18 16:27:19
// Update Time         :    2017/7/18 16:27:19
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Windows.Forms;
// ===============================================================================
namespace FKCashierBank
{
    public partial class MainForm : Form
    {
        private bool DoCmd(string strCmd, string[] strParams, ref string strOutput)
        {
            if(string.Compare(strCmd, "help", true) == 0)
            {
                return DoCmd_Help(strParams, ref strOutput);
            }
            if(string.Compare(strCmd, "bill", true) == 0)
            {
                return DoCmd_Bill(strParams, ref strOutput);
            }
            if (string.Compare(strCmd, "ip", true) == 0)
            {
                return DoCmd_IP(strParams, ref strOutput);
            }
            strOutput = $"Unknown cmd : {strCmd}, Please use \"help\" to get more infos.";
            return false;
        }
        private bool DoCmd_IP(string[] strParams, ref string strOutput)
        {
            if (strParams.Length < 2)
            {
                strOutput = "请输入對方IP PORT\n";
                return false;
            }
            try
            {
                System.Net.Sockets.TcpClient c = new System.Net.Sockets.TcpClient();
                c.Connect(strParams[0], Int32.Parse(strParams[1]));
                string ip = ((System.Net.IPEndPoint)c.Client.LocalEndPoint).Address.ToString();
                c.Close();
                strOutput = $"连接到{strParams[0]}:{strParams[1]}的本地IP是{ip}";
            }
            catch (Exception)
            {
                return true;
            }

            return true;
        }

        private bool DoCmd_Help(string[] strParams, ref string strOutput)
        {
            strOutput = "help - 输出帮助\n";
            strOutput += "bill [BANKNAME] - 查询一个银行流水：BANKNAME - CCB CMB BCM \n";
            return true;
        }

        /// <summary>
        /// 模拟流水查询消息
        /// </summary>
        /// <param name="strParams"></param>
        /// <param name="strOutput"></param>
        /// <returns></returns>
        private bool DoCmd_Bill(string[] strParams, ref string strOutput)
        {
            if(strParams.Length < 1)
            {
                strOutput = "请输入银行类型参数：BANKNAME - CCB CMB BCM ABC\n";
                return false;
            }
            string strBankType = strParams[0];
            int nBankType = GetBillBankTypeIDByName(strBankType);
            if(nBankType <= 0)
            {
                strOutput = "不支持流水功能的银行类型，请使用：BANKNAME - CCB CMB BCM ABC\n";
                return false;
            }
            SBillTaskInfo info = new SBillTaskInfo();
            info.taskID = new Random().Next(1, 99);
            info.bankCode = strBankType;
            // CCB测试
            if (nBankType == 1)
            {
            /*
                info.username = "xnian1074";
                info.accountNumber = "6217004220011286774";
                info.password = "hjk987po"; 
                */
                if (strParams.Length >= 2)
                {
                    if (strParams[1].StartsWith("he"))
                    {
                        info.username = "hedongh4146";
                        info.accountNumber = "6217000420004215509";
                        info.password = "qaz159zz";
                    }
                    else if (strParams[1].StartsWith("xn"))
                    {
                        info.username = "xnian1074";
                        info.accountNumber = "6217004220011286774";
                        info.password = "hjk987po";
                    }
                }
                else
                {
                    info.username = "hedongh4146";
                    info.accountNumber = "6217000420004215509";
                    info.password = "qaz159zz";
                }
/*+++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 测试传入具体时间是否有返回结果
                if (strParams.Length >= 3)
                {
                    if (strParams[2].StartsWith("1"))
                    {
                        //yyyyMMDDhhmmdd 
                        info.startTime = "20160711120000";
                        info.endTime = "20170724120000";
                    }
                    else if (strParams[2].StartsWith("2"))
                    {
                        //yyyyMMDD hhmmdd 
                        info.startTime = "20160711+120000";
                        info.endTime = "20170724+120000";
                    }
                    else if (strParams[2].StartsWith("3"))
                    {
                        //yyyyMMDD hh:mm:dd 
                        info.startTime = "20160711+12%3A00%3A00";
                        info.endTime = "20170724+12%3A00%3A00";
                    }
                    else if (strParams[2].StartsWith("4"))
                    {
                        //yyyyMMDD hh/mm/dd 
                        info.startTime = "20160711+12%2F00%2F00";
                        info.endTime = "20170724+12%2F00%2F00";
                    }
                    else if (strParams[2].StartsWith("5"))
                    {
                        //yyyy/MM/DDhhmmdd 
                        info.startTime = "2016%2F07%2F11120000";
                        info.endTime = "2017%2F07%2F24120000";
                    }
                    else if (strParams[2].StartsWith("6"))
                    {
                        //yyyy/MM/DD hhmmdd 
                        info.startTime = "2016%2F07%2F11+120000";
                        info.endTime = "2017%2F07%2F24+120000";
                    }
                    else if (strParams[2].StartsWith("7"))
                    {
                        //yyyy/MM/DD hh:mm:dd 
                        info.startTime = "2016%2F07%2F11+12%3A00%3A00";
                        info.endTime = "2017%2F07%2F24+12%3A00%3A00";
                    }
                    else if (strParams[2].StartsWith("8"))
                    {
                        //yyyy/MM/DD hh/mm/dd 
                        info.startTime = "2016%2F07%2F11+12%2F00%2F00";
                        info.endTime = "2017%2F07%2F24+12%2F00%2F00";
                    }
                }
                else
                {
                    info.startTime = "20161005 17:18:25";
                    info.endTime = "20170724 01:15:56";
                }
+++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
                info.startTime = "20161005 17:18:25";
                info.endTime = "20170816 21:15:56";

            }
            else if  (nBankType == 2)
            {
                info.username = "C00LOgsy";
                info.accountNumber = "6222623210002390520";
                info.password = "wsx159xx"; 
                info.startTime = "20160711";
                info.endTime = "20170724";
            }
            else if (nBankType == 4)
            {
                if (strParams.Length >= 2)
                {
                    if (strParams[1].StartsWith("fje"))
                    {
                        info.username = "fjef5653zkj";
                        info.accountNumber = "6228480668714219478";
                        info.password = "hjk987po";
                    }
                    else if (strParams[1].StartsWith("fjv"))
                    {
                        info.username = "fjvujg4596gc";
                        info.accountNumber = "6228482568556511677";
                        info.password = "qa1245ws";
                    }
                }
                else
                {
                    info.username = "fjef5653zkj";
                    info.accountNumber = "6228480668714219478";
                    info.password = "hjk987po";
                }
/*+++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 测试传入具体时间是否有返回结果
                if (strParams.Length >= 3)
                {
                    if (strParams[2].StartsWith("1"))
                    {
                        //yyyyMMDDhhmmdd 
                        info.startTime = "20160711120000";
                        info.endTime = "20170724120000";
                    }
                    else if (strParams[2].StartsWith("2"))
                    {
                        //yyyyMMDD hhmmdd 
                        info.startTime = "20160711+120000";
                        info.endTime = "20170724+120000";
                    }
                    else if (strParams[2].StartsWith("3"))
                    {
                        //yyyyMMDD hh:mm:dd 
                        info.startTime = "20160711+12%3A00%3A00";
                        info.endTime = "20170724+12%3A00%3A00";
                    }
                    else if (strParams[2].StartsWith("4"))
                    {
                        //yyyyMMDD hh/mm/dd 
                        info.startTime = "20160711+12%2F00%2F00";
                        info.endTime = "20170724+12%2F00%2F00";
                    }
                    else if (strParams[2].StartsWith("5"))
                    {
                        //yyyy/MM/DDhhmmdd 
                        info.startTime = "2016%2F07%2F11120000";
                        info.endTime = "2017%2F07%2F24120000";
                    }
                    else if (strParams[2].StartsWith("6"))
                    {
                        //yyyy/MM/DD hhmmdd 
                        info.startTime = "2016%2F07%2F11+120000";
                        info.endTime = "2017%2F07%2F24+120000";
                    }
                    else if (strParams[2].StartsWith("7"))
                    {
                        //yyyy/MM/DD hh:mm:dd 
                        info.startTime = "2016%2F07%2F11+12%3A00%3A00";
                        info.endTime = "2017%2F07%2F24+12%3A00%3A00";
                    }
                    else if (strParams[2].StartsWith("8"))
                    {
                        //yyyy/MM/DD hh/mm/dd 
                        info.startTime = "2016%2F07%2F11+12%2F00%2F00";
                        info.endTime = "2017%2F07%2F24+12%2F00%2F00";
                    }
                }
                else
                {
                    info.startTime = "20160831 10:02:56";
                    info.endTime = "20170321 00:00:00";
                }
+++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
                info.startTime = "20160831 10:02:56";
                info.endTime = "20170321 00:00:00";
            }
            else if (nBankType == 5)
            {
                info.username = "KING01ZMM";
                info.accountNumber = "6217680704422380";
                info.password = "qw7845er";
                info.startTime = "20160921 01:10:15";
                info.endTime = "20170819 10:02:56";
            }
            AddBillTaskToWaitQueue(info);

            /*
            string respone = "";
            GetBillTaskRequestHandler.OnMsg(0, "1", this, ref respone);
            */

            strOutput = "添加 测试银行流水查询任务 完成";
            return true;
        }
    }
}