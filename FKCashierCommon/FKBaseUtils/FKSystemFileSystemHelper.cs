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
// Create Time         :    2017/7/13 13:49:00
// Update Time         :    2017/7/13 13:49:00
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
// ===============================================================================
namespace FKBaseUtils
{
    public static class FKSystemFileSystemHelper
    {
        /// <summary>
        /// 查找指定文件夹内指定后缀的文件
        /// e.g: GetFirstFileFromDirWithSuffix("C:\dir", ".txt");
        /// </summary>
        /// <param name="strDir"></param>
        /// <param name="strSuffix"></param>
        /// <returns></returns>
        public static List<string> GetFilesFromDirWithSuffix(string strDirPath, string strSuffix)
        {
            try
            {
                List<string> listResult = new List<string>();
                string[] files = Directory.GetFiles(strDirPath);    // 得到全部文件
                foreach (string file in files)
                {
                    string exname = file.Substring(file.LastIndexOf(".") + 1); // 得到后缀名
                    if (strSuffix.IndexOf(file.Substring(file.LastIndexOf(".") + 1)) > -1)
                    {
                        listResult.Add(file);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Get files from dir with suffix failed. Error = {e.ToString()}");
                return null;
            }
            return null;
        }

        /// <summary>
        /// 清空一个文件夹
        /// </summary>
        /// <param name="strDirPath"></param>
        /// <returns></returns>
        public static bool ClearDir(string strDirPath)
        {
            try
            {
                if (!Directory.Exists(strDirPath))
                {
                    return true;
                }

                foreach (string d in Directory.GetFileSystemEntries(strDirPath))
                {
                    if (File.Exists(d))
                    {
                        FileInfo fileInfo = new FileInfo(d);
                        fileInfo.Attributes = FileAttributes.Normal;
                        File.Delete(d);                 //直接删除其中的文件  
                    }
                    else
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(d);
                        if (dirInfo.GetFiles().Length != 0)
                        {
                            ClearDir(dirInfo.FullName); //递归删除子文件夹
                        }
                        Directory.Delete(d, false);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Clear dir failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 创建一个文件夹
        /// </summary>
        /// <param name="strDirPath"></param>
        /// <returns></returns>
        public static bool CreateDir(string strDirPath)
        {
            try
            {
                if (Directory.Exists(strDirPath))
                    return true;

                Directory.CreateDirectory(strDirPath);
                return Directory.Exists(strDirPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Create dir failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 获取IE下载目录
        /// </summary>
        /// <returns></returns>
        public static string GetIEDownloadDir()
        {
            try
            {
                String path = String.Empty;
                RegistryKey rKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main");
                if (rKey != null)
                {
                    path = (String)rKey.GetValue("Default Download Directory");
                }
                if (String.IsNullOrEmpty(path))
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\downloads";
                }
                return path;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Get IE download dir failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取IE缓存目录（网络图片，JS等文件存放地）
        /// </summary>
        public static string GetIECacheDir()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), "Content.IE5");
                return path;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Get IE cache dir failed. Error = {e.ToString()}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 清空IE缓存目录下指定文件
        /// </summary>
        public static void ClearIEDownloadDir(string strFileName)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), "Content.IE5");
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

            //遍历该路径下的所有文件
            foreach (string fileName in files)
            {
                if (fileName.IndexOf(strFileName) != -1)
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                }
            }
        }

        /// <summary>
        /// 获取本Exe文件所在路径
        /// </summary>
        /// <returns></returns>
        public static string GetExeFilePath()
        {
            return Process.GetCurrentProcess().MainModule.FileName;
        }

        /// <summary>
        /// 获取本应用程序当前工作目录
        /// </summary>
        /// <returns></returns>
        public static string GetWorkdir()
        {
            return Application.StartupPath;
        }
    }
}