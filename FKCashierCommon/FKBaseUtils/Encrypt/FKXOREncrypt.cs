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
// Create Time         :    2017/7/13 11:46:37
// Update Time         :    2017/7/13 11:46:37
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Text;
// ===============================================================================
namespace FKBaseUtils
{
    public static class FKXOREncrypt
    {
        public static string FKXOREncryptOrDecrypt(string text, string strXORKey = FKBaseUtilsConsts.FK_XOR_ENCRYPT_KEY)
        {
            try
            {
                var result = new StringBuilder();

                for (int c = 0; c < text.Length; c++)
                {
                    char character = text[c];
                    uint charCode = (uint)character;
                    int keyPosition = c % strXORKey.Length;
                    char keyChar = strXORKey[keyPosition];
                    uint keyCode = (uint)keyChar;
                    uint combinedCode = charCode ^ keyCode;
                    char combinedChar = (char)combinedCode;

                    result.Append(combinedChar);
                }

                return result.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] XOR encrypt/decrypt failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
    }
}