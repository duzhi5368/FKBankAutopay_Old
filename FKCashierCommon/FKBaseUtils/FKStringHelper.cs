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
// Create Time         :    2017/7/13 11:51:36
// Update Time         :    2017/7/13 11:51:36
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Text;
using System.Xml;
// ===============================================================================
namespace FKBaseUtils
{
    public static class FKStringHelper
    {
        /// <summary>
        /// 判断一个string是否是xml格式（用来判断一段钥匙是C#版的还是JAVA版的）
        /// </summary>
        /// <param name="xmlstring"></param>
        /// <returns></returns>
        public static bool IsValidationXMLFormat(string strSourceString)
        {
            // TODO: 使用异常来判断，非常糟糕……需要更换方式验证
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strSourceString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断一个字符是否含有中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsHasChineseCode(string strSrc)
        {
            try
            {
                string tmp = string.Empty;
                for (int i = 0; i < strSrc.Length; ++i)
                {
                    tmp = strSrc.Substring(i, 1);
                    byte[] arr = Encoding.GetEncoding("GB2312").GetBytes(tmp);
                    if (arr.Length == 2)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Check if a string has Chinese code failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 遮罩一个字符串（目的是防止向用户显示明文信息）
        /// </summary>
        /// <param name="strSrc"></param>
        /// <param name="bIsUsePartHide"></param>
        /// <returns></returns>
        public static string MaskString(string strSrc, bool bIsUsePartHide = true)
        {
            try
            {
                if (strSrc.Length <= 0)
                    return string.Empty;

                string strRet = string.Empty;
                bool bIsReallyUsePartHide = bIsUsePartHide;
                bool bIsReallyUseXOREncrypt = true;

                if (IsHasChineseCode(strRet))
                {
                    // 若有中文，强制 * 替换，不进行XOR
                    bIsReallyUsePartHide = true;
                    bIsReallyUseXOREncrypt = false;
                }

                // 第一步，进行 * 替换
                if (bIsReallyUsePartHide)
                {
                    if (strSrc.Length == 1)
                    {
                        strRet = "*";
                    }
                    else if(strSrc.Length == 2)
                    {
                        strRet = "**";
                    }
                    else if (strSrc.Length == 3)
                    {
                        strRet = "***";
                    }
                    if (strSrc.Length >= 4 && strSrc.Length < 10)
                    {
                        strRet = strSrc.Substring(0, strSrc.Length - 4);
                        strRet += "****";
                    }
                    else if (strSrc.Length >= 10 && strSrc.Length < 20)
                    {
                        strRet = strSrc.Substring(0, strSrc.Length - 6);
                        strRet += "******";
                    }
                    else if (strSrc.Length > 20)
                    {
                        strRet = strSrc.Substring(0, 15);   // 仅取得前15位足够了
                        strRet += "**...**";
                    }
                }
                else
                {
                    strRet = strSrc;
                }

                // 第二步，进行 XOR 加密
                if (bIsReallyUseXOREncrypt)
                    return Convert.ToBase64String(Encoding.ASCII.GetBytes(FKXOREncrypt.FKXOREncryptOrDecrypt(strRet)));
                else
                    return strRet;
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] Try to mask a string failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
    }
}