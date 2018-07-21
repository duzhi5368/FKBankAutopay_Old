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
// Create Time         :    2017/7/13 17:55:16
// Update Time         :    2017/7/13 17:55:16
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Collections.Generic;
// ===============================================================================
namespace FKHttp
{
    public static class FKHttpClient
    {
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="strGetUrl"></param>
        /// <returns></returns>
        public static string GET(string strGetUrl, int nTimeOutMilliSecond = FKHttpConsts.GET_TIMEOUT_MILLISECOND)
        {
            // 初始设置
            ServicePointManager.DefaultConnectionLimit = 200;

            HttpWebRequest      request         = null;
            HttpWebResponse     response        = null;
            Stream              responseStream  = null;
            StreamReader        streamReader    = null;
            try
            {
                request = (HttpWebRequest)(WebRequest.Create(strGetUrl));
                request.Timeout = nTimeOutMilliSecond;
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                // 获取返回内容
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                return streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Http Get function failed. Error = {e.ToString()}");
                return string.Empty;
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Close();
                if (responseStream != null)
                    responseStream.Close();
                if (response != null)
                    response.Close();
                if (request != null)
                    request.Abort();
            }
        }
        public static string GET(string strGetUrl, Dictionary<string, string> dicGetHeader, int nTimeOutMilliSecond = FKHttpConsts.GET_TIMEOUT_MILLISECOND)
        {
            // 初始设置
            ServicePointManager.DefaultConnectionLimit = 200;

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream responseStream = null;
            StreamReader streamReader = null;
            try
            {
                request = (HttpWebRequest)(WebRequest.Create(strGetUrl));
                request.Timeout = nTimeOutMilliSecond;
                request.Method = "GET";
                if (dicGetHeader != null)
                {
                    foreach (var header in dicGetHeader)
                    {
                        if (header.Key == "Content-Type")
                        {
                            request.ContentType = header.Value;
                        }
                        else if (header.Key == "User-Agent")
                        {
                            request.UserAgent = header.Value;
                        }
                        else if (header.Key == "Accept")
                        {
                            request.Accept = header.Value;
                        }
                        else if (header.Key == "Host")
                        {
                            request.Host = header.Value;
                        }
                        else if (header.Key == "Connection")
                        {
                            request.Connection = header.Value;
                        }
                        else if (header.Key == "Referer")
                        {
                            request.Referer = header.Value;
                        }
                        else
                        {
                            request.Headers.Add(header.Key, header.Value);
                        }
                    }
                }
                else
                {
                    request.ContentType = "text/html;charset=UTF-8";
                }
                

                // 获取返回内容
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                return streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Http Get function failed. Error = {e.ToString()}");
                return string.Empty;
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Close();
                if (responseStream != null)
                    responseStream.Close();
                if (response != null)
                    response.Close();
                if (request != null)
                    request.Abort();
            }
        }

        /// <summary>
        /// Https 证书检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckIfServerCertificateValid(object sender, X509Certificate cert,
            X509Chain chain, SslPolicyErrors errors)
        {
            // 全部接受
            return true;
        }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="strPostUrl"></param>
        /// <param name="strPostData"></param>
        /// <param name="nTimeOutMilliSecond"></param>
        /// <returns></returns>
        public static string POST(string strPostUrl, string strPostData, Dictionary<string, string> dicPostHeader = null, int nTimeOutMilliSecond = FKHttpConsts.POST_TIMEOUT_MILLOSECOND, string responseToFile = "")
        {
            // 安全清除Http碎片连接
            System.GC.Collect();
            // 初始设置
            ServicePointManager.DefaultConnectionLimit = 200;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpWebRequest      request         = null;
            Stream              requestStream   = null;
            HttpWebResponse     response        = null;
            Stream              responseStream  = null;
            StreamReader        streamReader    = null;

            if(dicPostHeader != null)
            {
                string strHeaderInfo = string.Empty;
                foreach (string key in dicPostHeader.Keys)
                {
                    strHeaderInfo += string.Format("{0}={1};", key, dicPostHeader[key]);
                }
                Console.WriteLine("============================");
                Console.WriteLine($"POST URL = {strPostUrl}");
                Console.WriteLine($"POST DATA = {strPostData}");
                Console.WriteLine($"POST HEADER = {strHeaderInfo}");
                Console.WriteLine("============================");
            }

            try
            {
                if (strPostUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    // https 请求
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckIfServerCertificateValid);
                    request = WebRequest.Create(strPostUrl) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    // http请求
                    request = (HttpWebRequest)(WebRequest.Create(new Uri(strPostUrl)));
                }

                byte[] byteArray = Encoding.UTF8.GetBytes(strPostData);

                request.Timeout = nTimeOutMilliSecond;
                request.ReadWriteTimeout = FKHttpConsts.POST_READWRITE_TIMEOUT_MILLOSECOND;
                request.Method = "POST";
                request.KeepAlive = false;
                request.AllowAutoRedirect = false;
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;

                if (dicPostHeader != null)
                {
                    foreach (var header in dicPostHeader)
                    {
                        if (header.Key == "Content-Type")
                        {
                            request.ContentType = header.Value;
                        }
                        else if (header.Key == "User-Agent")
                        {
                            request.UserAgent = header.Value;
                        }
                        else if (header.Key == "Accept")
                        {
                            request.Accept = header.Value;
                        }
                        else if (header.Key == "Host")
                        {
                            request.Host = header.Value;
                        }
                        else if (header.Key == "Connection")
                        {
                            request.Connection = header.Value;
                        }
                        else if (header.Key == "Referer")
                        {
                            request.Referer = header.Value;
                        }
                        else
                        {
                            request.Headers.Add(header.Key, header.Value);
                        }                   
                    }                  
                }

                requestStream = request.GetRequestStream();
                requestStream.Write(byteArray, 0, byteArray.Length);
                requestStream.Close();
                // 获取返回内容
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                if (string.IsNullOrEmpty(responseToFile))
                {
                    /* Old read
                    using (streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
                    {
                        return streamReader.ReadToEnd();
                    }
                    */

                    // new read
                    byte[] bytes = StreamToString(responseStream);
                    return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                }
                else
                {
                    StreamToFile_New(responseStream, responseToFile);
                    /* Old read
                    FileStream responseFile = new FileStream(responseToFile,FileMode.Create,FileAccess.ReadWrite);
                    responseStream.Flush();
                    responseStream.CopyTo(responseFile);
                    responseFile.Flush();
                    responseFile.Close();
                    */
                    return "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Http Post function failed. Error = {e.ToString()}");
                return string.Empty;
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Close();
                if (responseStream != null)
                    responseStream.Close();
                if (response != null)
                    response.Close();
                if (request != null)
                    request.Abort();
            }
        }

        private static byte[] StreamToString(Stream stream)
        {
            byte[] buffer = new byte[0x1000];   // 4kb 
            MemoryStream ostream = new MemoryStream();
           
            int nByte = 0;
            while ((nByte = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ostream.Write(buffer, 0, nByte);
            }
            byte[] responeBytes = ostream.ToArray();
            return responeBytes;
        }

        private static void StreamToFile_New(Stream stream, string fileName)
        {
            try
            {
                byte[] bytes = StreamToString(stream);
                // 把 byte[] 写入文件   
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(bytes);
                bw.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] StreamToFile_New failed. Error = {e.ToString()}");
            }
        }
        private static void StreamToFile(Stream stream, string fileName)
        {
            try
            {
                // 把 Stream 转换成 byte[]   
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                // 设置当前流的位置为流的开始   
                stream.Seek(0, SeekOrigin.Begin);

                // 把 byte[] 写入文件   
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(bytes);
                bw.Close();
                fs.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] StreamToFile failed. Error = {e.ToString()}");
            }
        }
    }
}
