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
// Create Time         :    2017/7/13 13:09:50
// Update Time         :    2017/7/13 13:09:50
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Security.Cryptography;
// ===============================================================================
namespace FKBaseUtils
{
    public static class FKRSASignature
    {
        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strRSAPrivateKey">私钥</param>
        /// <param name="strHash"></param>
        /// <returns>签名后数据</returns>
        public static string RSASign(string strRSAPrivateKey, string strHash)
        {
            string strSignatureRet = string.Empty;
            try
            {
                byte[] HashbyteSignature;
                byte[] EncryptedSignatureData;
                HashbyteSignature = Convert.FromBase64String(strHash);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                if (FKStringHelper.IsValidationXMLFormat(strRSAPrivateKey))
                {
                    rsa.FromXmlString(strRSAPrivateKey);
                }
                else
                {
                    string strXML = FKRSAEncrypt.ConvertRSAPrivateKey_Java2DotNet(strRSAPrivateKey);
                    if (string.IsNullOrEmpty(strXML))
                    {
                        return string.Empty;
                    }
                    rsa.FromXmlString(strXML);
                }

                RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
                rsaFormatter.SetHashAlgorithm("MD5");
                EncryptedSignatureData = rsaFormatter.CreateSignature(HashbyteSignature);
                strSignatureRet = Convert.ToBase64String(EncryptedSignatureData);
                return strSignatureRet;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] RSA signature failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
        /// <summary>
        /// 检查是否是有效的RSA签名，验证签名有效性
        /// </summary>
        /// <param name="strPublicKey">公钥</param>
        /// <param name="strHash">HASH描述</param>
        /// <param name="strSignedString">签名后的结果</param>
        /// <returns>是否验证通过</returns>
        public static bool IsValidRSASign(string strRSAPublicKey, string strHash, string strSignedString)
        {
            try
            {
                byte[] DeformatterData;
                byte[] HashbyteDeformatter;
                HashbyteDeformatter = Convert.FromBase64String(strHash);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                if (FKStringHelper.IsValidationXMLFormat(strRSAPublicKey))
                {
                    rsa.FromXmlString(strRSAPublicKey);
                }
                else
                {
                    string strXML = FKRSAEncrypt.ConvertRSAPublicKey_Java2DotNet(strRSAPublicKey);
                    if (string.IsNullOrEmpty(strXML))
                    {
                        return false;
                    }
                    rsa.FromXmlString(strXML);
                }

                RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm("MD5");
                DeformatterData = Convert.FromBase64String(strSignedString);

                return rsaDeformatter.VerifySignature(HashbyteDeformatter, DeformatterData);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] RSA signature failed. Error = {e.ToString()}");
                return false;
            }
        }
    }
}