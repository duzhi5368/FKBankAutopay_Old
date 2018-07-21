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
// Create Time         :    2017/7/13 11:36:01
// Update Time         :    2017/7/13 11:36:01
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Security.Cryptography;
using System.Text;
// ===============================================================================

namespace FKBaseUtils
{
    public static class FKMD5Enrypt
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="strSrcCode"></param>
        /// <returns></returns>
        public static string ToMD5Encrypt(string strSrcCode)
        {
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(Encoding.Default.GetBytes(strSrcCode));
                return BitConverter.ToString(result).Replace("-", "");
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] MD5 encrypt failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
    }
}
