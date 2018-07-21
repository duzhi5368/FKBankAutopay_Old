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
// Create Time         :    2017/7/13 11:54:09
// Update Time         :    2017/7/13 11:54:09
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
// ===============================================================================
/*
    RSA相关说明
    私钥：用来解密和签名，自己使用
    公钥：公开，用于加密和验证签名，别人使用
    签名验证：  用户发送数据前，使用私钥签名，别人用它给的公钥验证签名，保证该信息是由它发送的。
                用户接受数据前，别人使用公钥对数据加密，它用私钥进行解密，保证信息只有它可以接受的到。
    加密过程：  字符串明文-> 转码为Byte[]字节流 -> 公钥cer证书加密 -> 密文Byte[]字节流 -> 网络发送
    解密过程：  密文Byte[]字节流 -> 私钥pfx证书解密 -> Byte[]字节流明文 -> 转码字符串明文

    1）消息发送者产生一个密钥对（私钥+公钥），然后将公钥发送给消息接收者
    2）消息发送者使用消息摘要MD5算法对原文进行加密（加密后的密文称作摘要）
    3）消息发送者将上述的摘要使用私钥加密得到密文--这个过程就被称作签名处理，得到的密文就被称作签名（注意，这个签名是名词）
    4）消息发送者将原文与密文发给消息接收者
    5）消息接收者使用公钥对密文（即签名）进行解密，得到摘要值content1
    6）消息接收者使用与消息发送者相同的消息摘要算法对原文进行加密，得到摘要值content2
    7）比较content1是不是与content2相等，若相等，则说明消息没有被篡改（消息完整性），也说明消息却是来源于上述的消息发送方（因为其他人是无法伪造签名的，这就完成了“抗否认性”和“认证消息来源”）
*/
//------------------------------------------------------------
namespace FKBaseUtils
{
    public static class FKRSAEncrypt
    {
        /// <summary>
        /// RSA私钥格式转换 java->.net
        /// </summary>
        /// <param name="strPrivateKey">Java生成的RSA私钥</param>
        /// <returns>C#使用的私钥</returns>
        public static string ConvertRSAPrivateKey_Java2DotNet(string strPrivateJavaRSAKey)
        {
            try
            {
                RsaPrivateCrtKeyParameters privateKeyParam =
                    (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(strPrivateJavaRSAKey));
                return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                                Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                                Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                                Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                                Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                                Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                                Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                                Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                                Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] Convert RSA private key (Java to DotNet) failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
        /// <summary>
        /// RSA私钥格式转换 .net->Java
        /// </summary>
        /// <param name="privateKey">.net生成的私钥</param>
        /// <returns>JAVA用的私钥</returns>
        public static string ConvertRSAPrivateKey_DotNet2Java(string strPrivateDotnetRSAKey)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strPrivateDotnetRSAKey);

                BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
                BigInteger exp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
                BigInteger d = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("D")[0].InnerText));
                BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("P")[0].InnerText));
                BigInteger q = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Q")[0].InnerText));
                BigInteger dp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DP")[0].InnerText));
                BigInteger dq = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DQ")[0].InnerText));
                BigInteger qinv = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("InverseQ")[0].InnerText));
                RsaPrivateCrtKeyParameters privateKeyParam = new RsaPrivateCrtKeyParameters(m, exp, d, p, q, dp, dq, qinv);
                PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);
                byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetEncoded();
                return Convert.ToBase64String(serializedPrivateBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Convert RSA private key (DotNet to Java) failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
        /// <summary>
        /// RSA公钥格式转换 java->.net
        /// </summary>
        /// <param name="publicKey">JAVA生成的公钥</param>
        /// <returns>C#使用的公钥</returns>
        public static string ConvertRSAPublicKey_Java2DotNet(string publicKey)
        {
            try
            {
                RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
                return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                    Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                    Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Convert RSA public key (Java to DotNet) failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
        /// <summary>
        /// RSA公钥格式转换 .net->java
        /// </summary>
        /// <param name="publicKey">C#生成的公钥</param>
        /// <returns>JAVA使用的公钥</returns>
        public static string ConvertRSAPublicKey_DotNet2Java(string publicKey)
        {
            try
            {
                XmlDocument doc = new XmlDocument(); doc.LoadXml(publicKey);
                BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
                BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
                RsaKeyParameters pub = new RsaKeyParameters(false, m, p);
                SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pub);
                byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
                return Convert.ToBase64String(serializedPublicBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Convert RSA public key (DotNet to Java) failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 生成一套.net的RSA密钥
        /// </summary>
        /// <param name="strXmlKeys">私钥</param>
        /// <param name="strXMLPublicKey">公钥</param>
        public static bool GenerateDotNetRSAKey(out string strXmlPrivateKeys, out string strXmlPublicKey)
        {
            strXmlPrivateKeys = string.Empty;
            strXmlPublicKey = string.Empty;
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                strXmlPrivateKeys = rsa.ToXmlString(true);
                strXmlPublicKey = rsa.ToXmlString(false);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Generate RSA keys failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// .net RSA公钥加密
        /// </summary>
        /// <param name="strSrcCode">等待加密的字符串</param>
        /// <param name="strPublicKey">公钥</param>
        /// <param name="bIsOAEP">是否使用最优非对称加密填充</param>
        /// <returns>加密后的密文</returns>
        public static string RSAEncryptByDotNetPublicKey(string strSrcString, string strDotNetPublicKey, bool bIsOAEP = false)
        {
            try
            {
                byte[] PlainTextArray;
                byte[] CypherTextArray;
                string strResult;
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(strDotNetPublicKey);
                PlainTextArray = (new UnicodeEncoding()).GetBytes(strSrcString);
                CypherTextArray = rsa.Encrypt(PlainTextArray, bIsOAEP);
                strResult = Convert.ToBase64String(CypherTextArray);
                return strResult;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] RSA encrypt failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
        /// <summary>
        /// .net RSA私钥解密
        /// </summary>
        /// <param name="strDecryptString">等待解密的字符串</param>
        /// <param name="strPrivateKey">私钥</param>
        /// <param name="bIsOAEP">是否使用最优非对称加密填充</param>
        /// <returns>解密后的明文</returns>
        public static string RSADecryptByDotNetPrivateKey(string strEncryptedString, string strDotNetPrivateKey, bool bIsOAEP = false)
        {
            try
            {
                byte[] PlainTextArray;
                byte[] DypherTextArray;
                string strResult;
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(strDotNetPrivateKey);
                PlainTextArray = Convert.FromBase64String(strEncryptedString);
                DypherTextArray = rsa.Decrypt(PlainTextArray, bIsOAEP);
                strResult = (new UTF8Encoding()).GetString(DypherTextArray);
                return strResult;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] RSA decrypt failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }
    }
}