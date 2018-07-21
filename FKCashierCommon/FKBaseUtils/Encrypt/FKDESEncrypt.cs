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
// Create Time         :    2017/7/13 11:36:02
// Update Time         :    2017/7/13 11:36:02
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
// ===============================================================================
namespace FKBaseUtils
{
    public static class FKDESEncrypt
    {
        /// <summary>
        /// DES加密（JAVA风格）
        /// </summary>
        /// <param name="strSrcCode"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string DesEncryptWithJavaStyle(string strSrcCode, string strKey)
        {
            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(strKey);
                byte[] keyIV = keyBytes;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(strSrcCode);

                // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
                desProvider.Mode = CipherMode.ECB;
                MemoryStream memStream = new MemoryStream();
                CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
                crypStream.Write(inputByteArray, 0, inputByteArray.Length);
                crypStream.FlushFinalBlock();

                return Convert.ToBase64String(memStream.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Des encrypt failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
        /// <summary>
        /// 解密JAVA DES加密后的数据
        /// </summary>
        /// <param name="strDecryptString"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string DesDecryptWithJavaStyle(string strDecryptString, string strKey)
        {
            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(strKey);
                byte[] keyIV = keyBytes;
                byte[] inputByteArray = Convert.FromBase64String(strDecryptString);

                // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
                desProvider.Mode = CipherMode.ECB;
                MemoryStream memStream = new MemoryStream();
                CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
                crypStream.Write(inputByteArray, 0, inputByteArray.Length);
                crypStream.FlushFinalBlock();

                return Encoding.Default.GetString(memStream.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Des decrypt failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
    }
}