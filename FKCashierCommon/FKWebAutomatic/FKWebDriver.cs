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
// Create Time         :    2017/7/15 11:05:23
// Update Time         :    2017/7/15 11:05:23
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using FKBaseUtils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
// ===============================================================================
namespace FKWebAutomatic
{
    public class FKWebDriver : FKSingleton<FKWebDriver>
    {
        #region ==== 浏览器类型 =====

        public const string browser_Firefox = "Firefox";
        public const string browser_Chrome = "Chrome";
        public const string browser_InternetExplorer = "InternetExplorer";
        public const string browser_PhantomJS = "PhantomJS";
        public const string browser_HtmlUnit = "HtmlUnit";
        public const string browser_HtmlUnitWithJavaScript = "HtmlUnitWithJavaScript";
        public const string browser_Opera = "Opera";
        public const string browser_Safari = "Safari";
        public const string browser_IPhone = "IPhone";
        public const string browser_IPad = "IPad";
        public const string browser_Android = "Android";

        #endregion ==== 浏览器类型 =====

        internal IWebDriver m_Driver = null;

        #region ==== 内部构造 ====

        /// <summary>
        /// 构造函数
        /// </summary>
        private FKWebDriver()
        {
            
            /*
             * 防止在getinstance的时候立即弹出IE，此部分代码延迟到openurl
             * 
            try
            {
                m_Driver = CreateDriver(browser_InternetExplorer, false, "http://127.0.0.1:4444/wd/hub");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Create web driver failed. Error = {e.ToString()}");
                throw e;
            }*/
        }
        /// <summary>
        /// 创建Selenium WebDriver对象
        /// </summary>
        /// <param name="strBrowserName"></param>
        /// <param name="bIsRemote"></param>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        private static IWebDriver CreateDriver(string strBrowserName, bool bIsRemote, string strUrl)
        {
            IWebDriver pWebDriver = null;
            if (bIsRemote)
            {
                pWebDriver = ConnectToRemoteWebDriver(strBrowserName, strUrl);
            }
            else
            {
                pWebDriver = StartEmdedWebDriver(strBrowserName);
            }
            return pWebDriver;
        }
        /// <summary>
        /// 连接一个远程IEWebDriver
        /// </summary>
        /// <param name="strBrowserName"></param>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        private static IWebDriver ConnectToRemoteWebDriver(string strBrowserName, string strUrl)
        {
            DesiredCapabilities caps = null;
            Uri hubUri = new Uri(strUrl);

            switch (strBrowserName)
            {

                case browser_Firefox:
                    caps = DesiredCapabilities.Firefox();
                    break;
                case browser_Chrome:
                    caps = DesiredCapabilities.Chrome();
                    break;
                case browser_InternetExplorer:
                    caps = DesiredCapabilities.InternetExplorer();
                    break;
                case browser_PhantomJS:
                    caps = DesiredCapabilities.PhantomJS();
                    break;
                case browser_HtmlUnit:
                    caps = DesiredCapabilities.HtmlUnit();
                    break;
                case browser_HtmlUnitWithJavaScript:
                    caps = DesiredCapabilities.HtmlUnitWithJavaScript();
                    break;
                case browser_Opera:
                    caps = DesiredCapabilities.Opera();
                    break;
                case browser_Safari:
                    caps = DesiredCapabilities.Safari();
                    break;
                case browser_IPhone:
                    caps = DesiredCapabilities.IPhone();
                    break;
                case browser_IPad:
                    caps = DesiredCapabilities.IPad();
                    break;
                case browser_Android:
                    caps = DesiredCapabilities.Android();
                    break;
                default:
                    throw new ArgumentException(String.Format(@"<{0}> 类型浏览器不被支持。请注意区分大小写。", strBrowserName),
                                                "浏览器类型名");
            }
            RemoteWebDriver pDriver = new RemoteWebDriver(hubUri, caps);
            return pDriver;
        }
        /// <summary>
        /// 启动一种类型的IEWebDriver
        /// </summary>
        /// <param name="strBrowserName"></param>
        /// <returns></returns>
        private static IWebDriver StartEmdedWebDriver(string strBrowserName)
        {
            switch (strBrowserName)
            {

                case browser_Firefox:
                    return new FirefoxDriver();
                case browser_Chrome:
                    return new ChromeDriver();
                case browser_InternetExplorer:
                    return new InternetExplorerDriver(FKSystemFileSystemHelper.GetWorkdir(), new InternetExplorerOptions(), TimeSpan.FromMinutes(1));
                case browser_PhantomJS:
                    return new PhantomJSDriver();
                case browser_Safari:
                    return new SafariDriver();
                default:
                    throw new ArgumentException(String.Format(@"<{0}> 类型浏览器不被支持。请注意区分大小写。", strBrowserName),
                                                "浏览器类型名");
            }
        }

        #endregion ==== 内部构造 ====

        #region ==== 接口函数 ====

        /// <summary>
        /// 关闭浏览器驱动
        /// </summary>
        public void Close()
        {
            try
            {
                if (m_Driver != null)
                {
                    m_Driver.Close();
                    m_Driver.Dispose();
                    m_Driver = null;
                }
            }
            catch{}
            finally
            {
                m_Driver = null;
            }
            
        }
        /// <summary>
        /// 执行指定JS脚本
        /// </summary>
        /// <param name="strJSCode"></param>
        /// <returns></returns>
        public bool ExecuteScript(string strJSCode)
        {
            if(m_Driver == null  || (!(m_Driver is IJavaScriptExecutor)))
            {
                return false;
            }
            try
            {
                (m_Driver as IJavaScriptExecutor).ExecuteScript(strJSCode);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] WebDriver execute js failed. JSScript = {strJSCode}, Error = {e.ToString()}");
                return false;
            }
        }
        /// <summary>
        /// 根据元素ID保存一张图片到本地
        /// </summary>
        /// <param name="strElemId"></param>
        /// <param name="strSaveLocalPath"></param>
        /// <returns></returns>
        public bool SaveImageById(string strElemId, string strSaveLocalPath)
        {
            try
            {
                // 查找元素
                IWebElement e = (m_Driver as RemoteWebDriver).FindElementById(strElemId);
                if (e == null)
                {
                    return false;
                }

                // 清空剪切板
                Clipboard.Clear();

                // 复制图片到剪切板
                Actions action = new Actions(m_Driver);
                action.ContextClick(e).Build().Perform();
                action.SendKeys("^c").Build().Perform();

                // 剪切板拷贝需要时间
                const int MAX_WAIT_TIME = 40; 
                int nTryTimes = 0;
                // 最大等待 MAX_WAIT_TIME * 50 = 2秒
                while (!Clipboard.ContainsImage())
                {
                    Thread.Sleep(50);
                    nTryTimes++;

                    // 等足够次数了
                    if(nTryTimes >= MAX_WAIT_TIME)
                    {
                        break;
                    }
                }

                // 删除原图
                if (File.Exists(strSaveLocalPath))
                {
                    File.Delete(strSaveLocalPath);
                }
                // 保存新图
                if (Clipboard.ContainsImage())
                {
                    Image image = Clipboard.GetImage();
                    image.Save(strSaveLocalPath, System.Drawing.Imaging.ImageFormat.Png);
                }
                

                // 检查图片文件是否存在
                // 最大等待 MAX_WAIT_TIME * 100 = 4秒
                nTryTimes = 0;
                while (!File.Exists(strSaveLocalPath))
                {
                    Thread.Sleep(100);
                    nTryTimes++;

                    // 等足够次数了
                    if (nTryTimes >= MAX_WAIT_TIME)
                    {
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] WebDriver save iamge by id failed. Error = {e.ToString()}");
                return false;
            }

            // 最后检查
            if(File.Exists(strSaveLocalPath))
            {
                return true;
            }
            else
            {
                Console.WriteLine($"[Error] WebDriver save iamge by id failed.");
                return false;
            }
        }
        public Bitmap GetEntireScreenshot()
        {
            var _driver = (m_Driver as RemoteWebDriver);
            Bitmap stitchedImage = null;
            try
            {
                long totalwidth1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return document.body.offsetWidth");//documentElement.scrollWidth");

                long totalHeight1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return  document.body.parentNode.scrollHeight");

                int totalWidth = (int)totalwidth1;
                int totalHeight = (int)totalHeight1;

                // Get the Size of the Viewport
                long viewportWidth1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return document.body.clientWidth");//documentElement.scrollWidth");
                long viewportHeight1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return window.innerHeight");//documentElement.scrollWidth");

                int viewportWidth = (int)viewportWidth1;
                int viewportHeight = (int)viewportHeight1;


                // Split the Screen in multiple Rectangles
                List<Rectangle> rectangles = new List<Rectangle>();
                // Loop until the Total Height is reached
                for (int i = 0; i < totalHeight; i += viewportHeight)
                {
                    int newHeight = viewportHeight;
                    // Fix if the Height of the Element is too big
                    if (i + viewportHeight > totalHeight)
                    {
                        newHeight = totalHeight - i;
                    }
                    // Loop until the Total Width is reached
                    for (int ii = 0; ii < totalWidth; ii += viewportWidth)
                    {
                        int newWidth = viewportWidth;
                        // Fix if the Width of the Element is too big
                        if (ii + viewportWidth > totalWidth)
                        {
                            newWidth = totalWidth - ii;
                        }

                        // Create and add the Rectangle
                        Rectangle currRect = new Rectangle(ii, i, newWidth, newHeight);
                        rectangles.Add(currRect);
                    }
                }

                // Build the Image
                stitchedImage = new Bitmap(totalWidth, totalHeight);
                // Get all Screenshots and stitch them together
                Rectangle previous = Rectangle.Empty;
                foreach (var rectangle in rectangles)
                {
                    // Calculate the Scrolling (if needed)
                    if (previous != Rectangle.Empty)
                    {
                        int xDiff = rectangle.Right - previous.Right;
                        int yDiff = rectangle.Bottom - previous.Bottom;
                        // Scroll
                        //selenium.RunScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                        ((IJavaScriptExecutor)_driver).ExecuteScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                        System.Threading.Thread.Sleep(200);
                    }

                    // Take Screenshot
                    var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();

                    // Build an Image out of the Screenshot
                    Image screenshotImage;
                    using (MemoryStream memStream = new MemoryStream(screenshot.AsByteArray))
                    {
                        screenshotImage = Image.FromStream(memStream);
                    }

                    // Calculate the Source Rectangle
                    Rectangle sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);

                    // Copy the Image
                    using (Graphics g = Graphics.FromImage(stitchedImage))
                    {
                        g.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                    }

                    // Set the Previous Rectangle
                    previous = rectangle;
                }
            }
            catch (Exception ex)
            {
                // handle
            }
            return stitchedImage;
        }

        /// <summary>
        /// 尝试保存一个image元素到本地
        /// </summary>
        /// <param name="strIDOrXPath"></param>
        /// <param name="strFilename"></param>
        /// <returns></returns>
        public bool SaveImageBySnapshot(string strIDOrXPath, string strFilename)
        {
            try
            {
                IWebElement webElem = null;
                do
                {
                    try
                    {
                        webElem = (m_Driver as RemoteWebDriver).FindElementById(strIDOrXPath);
                        if (webElem != null)
                        {
                            break;
                        }
                    }
                    catch{}
                    try
                    {
                        webElem = (m_Driver as RemoteWebDriver).FindElementByXPath(strIDOrXPath);
                        if (webElem != null)
                        {
                            break;
                        }
                    }
                    catch{}

                    // 其他获取方法在此扩展
                } while (false);

                if (webElem == null)
                {
                    Console.WriteLine($"[Error] WebDriver snapshot save iamge by id failed. Error = can not find {strIDOrXPath}");
                    return false;
                }
                if ((m_Driver as RemoteWebDriver) != null)
                {
                    var shotDriver = (m_Driver as ITakesScreenshot);
                    if (shotDriver == null)
                    {
                        Console.WriteLine($"[Error] WebDriver snapshot save iamge by id failed. shotDriver is null");
                        return false;
                    }
                    else
                    {
                        m_Driver.Manage().Window.Maximize();
                        //shotDriver.GetScreenshot().SaveAsFile("C:\\fuck.jpg", ImageFormat.Jpeg);

                    }
                    
                    // 截取全屏，然后拆出元素部分保存
                    Point ElemPoint = webElem.Location;
                    Point WindowPoint = (m_Driver as RemoteWebDriver).Manage().Window.Position;

                    int nElemWidth = webElem.Size.Width;
                    int nElemHeight = webElem.Size.Height;
                    var ss = shotDriver.GetScreenshot();
                    int nWebBrowserWidth = (m_Driver as RemoteWebDriver).Manage().Window.Size.Width;
                    int nWebBrowserHeight = (m_Driver as RemoteWebDriver).Manage().Window.Size.Height;
                    Bitmap fullScreenImg = (Bitmap)Image.FromStream(new MemoryStream(ss.AsByteArray));
                    Size imageSize = new Size(Math.Min(nElemWidth, fullScreenImg.Width),
                    Math.Min(nElemHeight, fullScreenImg.Height));
                    Rectangle rect = new Rectangle(ElemPoint, imageSize);
                    Image final = fullScreenImg.Clone(rect, fullScreenImg.PixelFormat);

                    final.Save(strFilename, ImageFormat.Bmp);
                    final.Dispose();
                    /*
                    // 灰度->二值化->降噪处理
                    Bitmap bitmap = new Bitmap(final);
                    Bitmap afterGray = FKImageLibrary.Gray.GrayScale(bitmap, 1);
                    Bitmap afterBinary = FKImageLibrary.Gray.BinaryZation(afterGray, binaryPara);
                    Bitmap afterDenoise = FKImageLibrary.Gray.NoiseReduction(afterBinary);

                    afterDenoise.Save(strFilename, ImageFormat.Bmp);
                    afterDenoise.Dispose();
                    */

                    if (!File.Exists(strFilename))
                    {
                        Console.WriteLine($"[Error] WebDriver snapshot save iamge by id failed. file not exist");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WebDriver snapshot save iamge by id failed. Error = {e.ToString()}");
                return false;
            }
            return false;
        }
        /// <summary>
        /// 打开一个Url
        /// </summary>
        /// <param name="strUrl"></param>
        public void OpenUrl(string strUrl)
        {
            try
            {
                if (m_Driver == null)
                {
                    m_Driver = CreateDriver(browser_InternetExplorer, false, "http://127.0.0.1:4444/wd/hub");
                    // m_Driver.Manage().Timeouts().SetScriptTimeout(System.TimeSpan.MinValue);
                    // m_Driver.Manage().Timeouts().SetPageLoadTimeout(System.TimeSpan.MinValue);
                }
                m_Driver.Navigate().GoToUrl(strUrl);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WebDriver open url failed. Error = {e.ToString()}");
            }
        }
        /// <summary>
        /// 切换Tab到指定Index (从0计数)
        /// </summary>
        /// <param name="nTabIndex"></param>
        public void SwitchToTab(int nTabIndex)
        {
            try
            {
                int nTabsNum = m_Driver.WindowHandles.Count;
                // 页面多于一个，说明IE有流氓插件开启了
                if (nTabsNum > 1)
                {
                    int nReallyTabIndex = nTabIndex >= nTabsNum ? nTabsNum - 1 : nTabIndex;
                    ArrayList tab = new ArrayList(m_Driver.WindowHandles);
                    string strWinTitle = tab[nTabIndex] as string;
                    m_Driver.SwitchTo().Window(strWinTitle);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WebDriver switch to tab failed. Error = {e.ToString()}");
            }
        }
        /// <summary>
        /// 切换Frame
        /// </summary>
        /// <param name="strFrameName"></param>
        public void SwitchToFrame(string strFrameName)
        {
            try
            {
                if (string.IsNullOrEmpty(strFrameName))
                {
                    (m_Driver as RemoteWebDriver).SwitchTo().DefaultContent();
                    return;
                }
                (m_Driver as RemoteWebDriver).SwitchTo().Frame(strFrameName);
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] WebDriver switch to frame failed. Name = {strFrameName}, Error = {e.ToString()}");
            }
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        public void DownloadFile()
        {
            SendKeys.SendWait("%s");
        }
        /// <summary>
        /// 点击关闭JS警告框
        /// </summary>
        /// <returns></returns>
        public bool AcceptAlertPopup()
        {
            try
            {
                IAlert e = (m_Driver as RemoteWebDriver).SwitchTo().Alert();
                const int MAX_TRY_TIME = 5;
                for (int i = 0; i < MAX_TRY_TIME; ++i)
                {
                    if (e != null)
                    {
                        e.Accept();
                        return true;
                    }
                    Thread.Sleep(100);
                }

                return false;
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] WebDriver accept alert failed. Error = {e.ToString()}");
                return false;
            }
        }

        #endregion ==== 接口函数 ====

        #region ==== 核心接口 ====

        /// <summary>
        /// 设置指定元素ID的控件的文字
        /// </summary>
        /// <param name="strElemID"></param>
        /// <param name="strText"></param>
        /// <returns></returns>
        public bool SetTextByID(string strElemID, string strText)
        {
            try
            {
                IWebElement e = (m_Driver as RemoteWebDriver).FindElementById(strElemID);
                if (e == null)
                {
                    return false;
                }
                // 去除只读属性
                string readonlyAtti = e.GetAttribute("readonly");
                if (!string.IsNullOrEmpty(readonlyAtti))
                {
                    (m_Driver as IJavaScriptExecutor).ExecuteScript("arguments[0].removeAttribute('readonly',[1])", e, readonlyAtti);
                }
                // 先清除原文本数据
                e.Clear();
                // 设置新值
                e.SendKeys(strText);

                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] WebDriver set text by id failed. ID = {strElemID}, Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 设置指定元素Name的控件的文字
        /// </summary>
        /// <param name="strElemName"></param>
        /// <param name="strText"></param>
        /// <returns></returns>
        public bool SetTextByName(string strElemName, string strText)
        {
            try
            {
                IWebElement e = (m_Driver as RemoteWebDriver).FindElementByName(strElemName);
                if (e == null)
                {
                    return false;
                }
                // 去除只读属性
                string readonlyAtti = e.GetAttribute("readonly");
                if (!string.IsNullOrEmpty(readonlyAtti))
                {
                    (m_Driver as IJavaScriptExecutor).ExecuteScript("arguments[0].removeAttribute('readonly',[1])", e, readonlyAtti);
                }
                // 先清除原文本数据
                e.Clear();
                // 设置新值
                e.SendKeys(strText);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WebDriver set text by id failed. Name = {strElemName}, Error = {e.ToString()}");
                return false;
            }
        }
        /// <summary>
        /// 获取指定元素ID的控件的文字
        /// </summary>
        /// <param name="strElemID"></param>
        /// <returns></returns>
        public string GetTextByID(string strElemID)
        {
            try
            {
                IWebElement e = (m_Driver as RemoteWebDriver).FindElementById(strElemID);
                if (null == e)
                {
                    return string.Empty;
                }

                return e.Text;
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] WebDriver get text by id failed. ID = {strElemID}, Error = {e.ToString()}");
                return string.Empty;
            }
        }
       
        public string GetAttributeByXPath(string strPath, string strAttribute)
        {
            try
            {
                IWebElement e = (m_Driver as RemoteWebDriver).FindElementByXPath(strPath);
                if (null == e)
                {
                    return string.Empty;
                }

                return e.GetAttribute(strAttribute);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WebDriver get attribute by id failed. XPATH = {strPath} att = {strAttribute}, Error = {e.Message}");
                return string.Empty;
            }
        }
        /// <summary>
        /// 点击指定元素ID的控件
        /// </summary>
        /// <param name="strElemID"></param>
        /// <returns></returns>
        public bool ClickByID(string strElemID)
        {
            try
            {
                IWebElement e = (m_Driver as RemoteWebDriver).FindElementById(strElemID);
                if (null == e)
                {
                    return false;
                }

                e.Click();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] WebDriver click by id failed. ID = {strElemID}, Error = {e.ToString()}");
                return false;
            }
        }
        /// <summary>
        /// 点击指定元素XPath的控件
        /// </summary>
        /// <param name="strElemXPath"></param>
        /// <returns></returns>
        public bool ClickByXPath(string strElemXPath)
        {
            try
            {
                IWebElement e = (m_Driver as RemoteWebDriver).FindElementByXPath(strElemXPath);
                if (null == e)
                {
                    return false;
                }

                e.Click();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WebDriver click by id failed. XPath = {strElemXPath}, Error = {e.ToString()}");
                return false;
            }
        }
        /// <summary>
        /// 双击指定元素ID的控件
        /// </summary>
        /// <param name="strElemID"></param>
        /// <returns></returns>
        public bool DoubleClickByID(string strElemID)
        {
            try
            {
                IWebElement e = (m_Driver as RemoteWebDriver).FindElementById(strElemID);
                if (null == e)
                {
                    return false;
                }

                new Actions(m_Driver).DoubleClick(e).Build().Perform();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WebDriver double click by id failed. ID = {strElemID}, Error = {e.ToString()}");
                return false;
            }
        }
        /// <summary>
        /// 检查指定XPath的元素是否可见
        /// </summary>
        /// <param name="strElemXPath"></param>
        /// <returns></returns>
        public bool IsElementVisiableByXPath(string strElemXPath)
        {
            try
            {
                IWebElement e = (m_Driver as RemoteWebDriver).FindElementByXPath(strElemXPath);
                if (e == null)
                {
                    return false;
                }
                if (e.Displayed)
                {
                    return true;
                }
                return false;
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] WebDriver find element visiable failed. XPath = {strElemXPath}, Error = {e.ToString()}");
                return false;
            }
        }
        /// <summary>
        /// 检查指定ID的元素是否可见
        /// </summary>
        /// <param name="strElemID"></param>
        /// <returns></returns>
        public bool IsElementVisiableByID(string strElemID)
        {
            try
            {
                IWebElement e = (m_Driver as RemoteWebDriver).FindElementById(strElemID);
                if (null == e)
                {
                    return false;
                }
                if (e.Displayed)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WebDriver find element visiable failed. ID = {strElemID}, Error = {e.ToString()}");
                return false;
            }
        }
        /// <summary>
        /// 等到指定XPath的元素可见
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="strXPath"></param>
        /// <param name="timeOutMilliseconds"></param>
        /// <returns></returns>
        public IWebElement WaitUntilVisibleByXPath(string strXPath, int timeOutMilliseconds)
        {
            try
            {
                TimeSpan timeOut = TimeSpan.FromMilliseconds(timeOutMilliseconds);

                Stopwatch sw = new Stopwatch();
                sw.Start();

                while (true)
                {
                    IWebElement element = null;
                    Exception lastException = null;
                    try
                    {
                        element = (m_Driver as RemoteWebDriver).FindElementByXPath(strXPath);
                    }
                    catch (Exception e)
                    {
                        lastException = e;
                    }

                    try
                    {
                        if (element != null)
                        {
                            if (element.Displayed)
                            {
                                sw.Stop();
                                return element;
                            }
                        }
                        Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        lastException = e;
                    }

                    // 超时
                    if (sw.Elapsed > timeOut)
                    {
                        Console.WriteLine($"[Warning] WebDriver wait element visiable failed. Timeout = {timeOut.TotalMilliseconds} ms, Error = {lastException.ToString()}");
                        return null;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] WebDriver wait element visiable failed. Error = {e.ToString()}");
                return null;
            }
        }
        /// <summary>
        /// 获取所有cookie
        /// </summary>
        /// <returns>返回string：";"号分割</returns>
        public string GetAllCookies()
        {
            string retString = string.Empty;
            try
            {
                //Driver.Manage.Cookies
                var allCookies = (m_Driver as RemoteWebDriver).Manage().Cookies.AllCookies;
                
                if (allCookies == null)
                {
                    return string.Empty;
                }
                foreach (var oneCookie in allCookies)
                {
                    if (!string.IsNullOrEmpty(retString))
                    {
                        retString += ";";
                    }
                    retString += (oneCookie.Name + "=" + oneCookie.Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WebDriver GetAllCookies failed. Error = {e.ToString()}");
                return string.Empty;
            }
            return retString;
        }
        /// <summary>
        /// 通过ID转移焦点到新Frame中
        /// </summary>
        /// <returns></returns>
        public bool SwitchToFrameByID(string strFrameID)
        {
            if (string.IsNullOrEmpty(strFrameID))
            {
                return false;
            }

            try
            {
                var frameElement = (m_Driver as RemoteWebDriver).FindElementById(strFrameID);
                if (frameElement == null)
                    return false;
                (m_Driver as RemoteWebDriver).SwitchTo().Frame(frameElement);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WebDriver SwitchToFrameByID({strFrameID}) failed. Error = {e.ToString()}");
                return false;
            }
            return true;

        }

        public bool SwitchToParentFrame()
        {
            try
            {
                (m_Driver as RemoteWebDriver).SwitchTo().ParentFrame();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] WebDriver switch to parent frame failed. Error = {e.ToString()}");
                return false;
            }
            return true;
        }
        #endregion ==== 核心接口 ====
    }
}