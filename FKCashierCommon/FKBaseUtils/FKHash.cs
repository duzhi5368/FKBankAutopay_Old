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
// Create Time         :    2017/7/13 11:43:37
// Update Time         :    2017/7/13 11:43:37
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Security.Cryptography;
using System.Text;
// ===============================================================================
namespace FKBaseUtils
{
    public static class FKHash
    {
        /// <summary>
        /// 获取Hash描述表
        /// </summary>
        /// <param name="strSource">等待签名的文件</param>
        /// <param name="strHashData">Hash描述</param>
        /// <returns>是否执行成功</returns>
        public static bool GetHash(string strSource, ref string strHashData)
        {
            try
            {
                byte[] bBuffer;
                byte[] bHashData;
                HashAlgorithm md5 = HashAlgorithm.Create("MD5");
                bBuffer = Encoding.GetEncoding("UTF-8").GetBytes(strSource);
                bHashData = md5.ComputeHash(bBuffer);
                strHashData = Convert.ToBase64String(bHashData);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] MD5 hash failed. Error = {e.ToString()}");
                return false;
            }
        }
    }
}