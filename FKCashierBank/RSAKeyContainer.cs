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
// Create Time         :    2017/7/17 16:04:58
// Update Time         :    2017/7/17 16:04:58
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using LOGGER = FKLog.FKLogger;
// ===============================================================================
namespace FKCashierBank
{
    public class RSAKeyContainer : FKBaseUtils.FKSingleton<RSAKeyContainer>
    {
        private string m_strCSharpPrivateKey = string.Empty;
        private string m_strCSharpPublicKey = string.Empty;
        private string m_strJavaPublicKey = string.Empty;

        private RSAKeyContainer()
        {
            GenCSharpKey();
        }

        private void GenCSharpKey()
        {
            try
            {
                string strXMLPublicKey = "";
                string strXMLPrivateKey = "";
                FKBaseUtils.FKRSAEncrypt.GenerateDotNetRSAKey(out strXMLPrivateKey, out strXMLPublicKey);
                m_strCSharpPublicKey = FKBaseUtils.FKRSAEncrypt.ConvertRSAPublicKey_DotNet2Java(strXMLPublicKey);
                m_strCSharpPrivateKey = FKBaseUtils.FKRSAEncrypt.ConvertRSAPrivateKey_DotNet2Java(strXMLPrivateKey);
            }
            catch(Exception e)
            {
                LOGGER.ERROR($"Create C# RSA key failed. Error = {e.ToString()}");
            }
        }

        public string GetCSharpPublicKey()
        {
            if (string.IsNullOrEmpty(m_strCSharpPublicKey))
            {
                LOGGER.WARN($"C# RSA public key was empty.");
                GenCSharpKey();
            }

            return m_strCSharpPublicKey;
        }

        public string GetCSharpPrivateKey()
        {
            if (string.IsNullOrEmpty(m_strCSharpPrivateKey))
            {
                LOGGER.WARN($"C# RSA private key was empty.");
                GenCSharpKey();
            }

            return m_strCSharpPrivateKey;
        }

        public void SetJavaPublicKey(string key)
        {
            if(!string.Equals(key, m_strJavaPublicKey))
            {
                LOGGER.WARN($"Java RSA public key changed. {m_strJavaPublicKey} -> {key}");
            }
            m_strJavaPublicKey = key;
        }

        public string GetJavaPublicKey()
        {
            return m_strJavaPublicKey;
        }

        public void Clear()
        {
            m_strJavaPublicKey = m_strCSharpPublicKey = m_strCSharpPrivateKey = string.Empty;
        }
    }
}