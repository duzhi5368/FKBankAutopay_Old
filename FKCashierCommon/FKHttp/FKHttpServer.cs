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
// Create Time         :    2017/7/14 10:02:36
// Update Time         :    2017/7/14 10:02:36
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;
// ===============================================================================
namespace FKHttp
{
    public class FKHttpServer
    {
        private HttpListener m_Listener = null;
        private Func<Form, HttpListenerRequest, string, string> m_RequestHandlerMethod = null;
        private Form m_ShowForm = null;
        private bool m_IsSupportingSSL = false;

        /// <summary>
        /// 使用SSL证书
        /// </summary>
        /// <returns></returns>
        private bool UseSSLCert()
        {
            X509Certificate2 cert = null;
            try
            {
                // 创建证书
                cert = new X509Certificate2(FKHttpConsts.HTTPS_CERT_FILE_NAME, FKHttpConsts.HTTPS_CERT_PASSWORD,
                    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

                // 添加证书到当前用户证书列表
                try
                {
                    X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.MaxAllowed);
                    if (!store.Certificates.Contains(cert))
                    {
                        store.Add(cert);
                    }
                    store.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[Warning] Add https certificate to store failed. Error = {e.ToString()}");
                }
                m_IsSupportingSSL = true;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Add https certificate failed. Error = {e.ToString()}");
                return false;
            }
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="ownerForm"></param>
        /// <param name="requestHandlerMethod"></param>
        /// <param name="IsUseSSLCert"></param>
        /// <param name="strListenIPPort"></param>
        public FKHttpServer(Form ownerForm, Func<Form, HttpListenerRequest, string, string> requestHandlerMethod, 
            bool IsUseSSLCert, string strListenIPPort)
        {
            m_IsSupportingSSL = false;

            if (!HttpListener.IsSupported)
            {
                Console.WriteLine($"[Error] FKWebServer needs Windows XP SP2, Server 2003 or later.");
                throw new Exception($"[Error] FKWebServer needs Windows XP SP2, Server 2003 or later.");
            }
            if (string.IsNullOrEmpty(strListenIPPort))
            {
                Console.WriteLine($"[Error] Prefixes params is empty.");
                throw new Exception($"[Error] Prefixes params is empty.");
            }

            if (IsUseSSLCert)
            {
                if (UseSSLCert())
                {
                    Console.WriteLine($"[Info] Load SSL certificate successed.");
                }
                else
                {
                    Console.WriteLine($"[Info] Load SSL certificate failed.");
                }
            }
            else
            {
                Console.WriteLine($"[Info] Didn't use SSL certificate.");
            }

            m_Listener = new HttpListener();
            m_Listener.Prefixes.Add(strListenIPPort);
            m_RequestHandlerMethod = requestHandlerMethod;
            m_ShowForm = ownerForm;

            try
            {
                m_Listener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Listener start error : " + e.ToString());
                throw new Exception($"[Error] Listener start error : " + e.ToString());
            }
        }
        /// <summary>
        /// 开始运行
        /// </summary>
        public void Start()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine($"[Info] FKWebServer begin listening...");
                try
                {
                    if (m_Listener == null)
                    {
                        Console.WriteLine($"[Error] Http listener is unused.");
                        throw new Exception($"[Error] Http listener is unused.");
                    }

                    while (m_Listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                Console.WriteLine($"[Info] Recived a connect...");

                                HttpListenerRequest request = ctx.Request;
                                string srtRequestMsg = "";
                                using (var reader = new StreamReader(request.InputStream, Encoding.UTF8))
                                {
                                    srtRequestMsg = reader.ReadToEnd();
                                }
                                // 回调处理
                                string strResponeMsg = m_RequestHandlerMethod(m_ShowForm, ctx.Request, srtRequestMsg);

                                // 发送ResponeMsg
                                byte[] buf = Encoding.UTF8.GetBytes(strResponeMsg);
                                ctx.Response.ContentType = "application/json";
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.SendChunked = false;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"[Error] Handle http connect failed. Error: " + e.ToString());
                                throw new Exception($"[Error] Handle http connect failed. Error: " + e.ToString());
                            }
                            finally
                            {
                                // 永远记得关闭Stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, m_Listener.GetContext());
                    }
                }
                catch (System.Net.HttpListenerException)
                {
                    // do nothing
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[Error] Http working thread failed. Error: " + e.ToString());
                    throw new Exception($"[Error] Http working thread failed. Error: " + e.ToString());
                }
            });
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            try
            {
                if (m_Listener != null)
                {
                    m_Listener.Stop();
                    m_Listener.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Stop http listener failed. Error: " + e.ToString());
                throw new Exception($"[Error] Stop http listener failed. Error: " + e.ToString());
            }
        }
        /// <summary>
        /// 是否WebService启动中
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            return m_Listener.IsListening;
        }
        /// <summary>
        /// 是否支持HTTPS监听，若False则可能未启动或者仅支持Http
        /// </summary>
        /// <returns></returns>
        public bool IsSupportingSSL()
        {
            return m_IsSupportingSSL;
        }
    }
}