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
// Create Time         :    2017/7/13 15:27:51
// Update Time         :    2017/7/13 15:27:51
// Class Version       :    v1.0.0.0
// Class Description   :    
// ===============================================================================
using System;
using SQLite;
using FKBaseUtils;
// ===============================================================================
namespace FKLog
{
    /// <summary>
    /// 数据库日志管理器
    /// </summary>
    public class FKSQLiteLog
    {
        private FKSQLiteLogConnection   m_DBObject = null;   // 数据库连接器对象
        private SQLiteCommand           m_SqlCmd = null;     // 等待被指定的数据库命令

        /// <summary>
        /// 是否已经存在当日DB文件
        /// </summary>
        /// <param name="t">当前时间</param>
        /// <returns>true表示已经存在当日DB文件</returns>
        private bool IsDBFileExistByTime(DateTime t)
        {
            try
            {
                if (m_DBObject == null)
                    return false;

                FKSystemFileSystemHelper.CreateDir(FKSystemFileSystemHelper.GetWorkdir() + "\\" + FKLogConsts.SQLITE_LOG_DIR_NAME);
                string strDBFileName = GetDBFileNameByTime(t);
                return (m_DBObject.DatabasePath.CompareTo(strDBFileName) == 0);
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] Check if a DB file existed failed. Error = {e.ToString()}");
                return false;
            }
        }
        /// <summary>
        /// 创建当日DB文件
        /// </summary>
        /// <param name="t">当时时间</param>
        /// <returns>创建后的DB文件对象</returns>
        private FKSQLiteLogConnection CreateDBFileByTime(DateTime t)
        {
            FKSQLiteLogConnection db = null;
            try
            {
                FKSystemFileSystemHelper.CreateDir(FKSystemFileSystemHelper.GetWorkdir() + "\\" + FKLogConsts.SQLITE_LOG_DIR_NAME);
                db = new FKSQLiteLogConnection(GetDBFileNameByTime(t));
                db.CreateTable<FKSQLiteLogNode>();
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] Create DB file Failed. Error = {e.ToString()}");
                return null;
            }
            return db;
        }
        /// <summary>
        /// 获取DB文件名
        /// </summary>
        /// <param name="t">当日时间</param>
        /// <returns>字符串，DB文件名</returns>
        private string GetDBFileNameByTime(DateTime t)
        {
            string strCurLogName = t.ToString("yyyy-MM-dd");
            return FKSystemFileSystemHelper.GetWorkdir() + "\\" + FKLogConsts.SQLITE_LOG_DIR_NAME
                + "\\" + strCurLogName + FKLogConsts.SQLITE_LOG_FILE_SUFFIX;
        }

        /// <summary>
        /// 对外接口：添加一条DB Log记录
        /// </summary>
        /// <param name="node"></param>
        public bool AddLogToDB(FKSQLiteLogNode node)
        {
            try
            {
                if (!IsDBFileExistByTime(node.LogTime))
                {
                    m_DBObject = CreateDBFileByTime(node.LogTime);
                }
                return (m_DBObject.Insert(node) > 0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Add log to Failed. Error = {e.ToString()}");
                return false;
            }
        }
        /// <summary>
        /// 查询一个指定TaskID的日志信息
        /// </summary>
        /// <param name="nTaskID">任务ID</param>
        /// <returns></returns>
        public string GetLogByTaskID(int nTaskID, string DBFileName)
        {
            try
            {
                m_DBObject = new FKSQLiteLogConnection(DBFileName);
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error]Create FKDBConnection Failed. Error = {e.ToString()}");
                return string.Empty;
            }

            if (m_DBObject == null)
            {
                Console.WriteLine($"[Error]Create FKDBConnection Failed.");
                return string.Empty;
            }

            try
            {
                string sql = "select * from SDBLogNode where TaskID = " + nTaskID;
                m_SqlCmd = m_DBObject.CreateCommand(sql);

                var results = m_SqlCmd.ExecuteQuery<FKSQLiteLogNode>().ToArray();
                string strRet = "";
                for (var i = 0; i < results.Length; i++)
                {
                    strRet += "{\"id\":";
                    strRet += results[i].Id;
                    strRet += ",\"logId\":";
                    strRet += results[i].LogId;
                    strRet += ",\"logTime\":\"";
                    strRet += results[i].LogTime.ToString("yyyy-MM-dd H:mm:ss");
                    strRet += "\",\"logInfo\":\"";
                    strRet += results[i].LogInfo;
                    strRet += "\"}";
                    if (i + 1 < results.Length)
                    {
                        strRet += ',';
                    }
                }
                return strRet;
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] Select DBLog Failed. taskId = {nTaskID} : error = {e.ToString()}");
                return string.Empty;
            }
        }
        /// <summary>
        /// 查询一个指定TaskID的日志信息总条目数
        /// </summary>
        /// <param name="nTaskID"></param>
        /// <returns></returns>
        public int GetLogsCountByTaskID(int nTaskID)
        {
            if (m_DBObject == null)
            {
                return 0;
            }
            try
            {
                string sql = "select count(*) from SDBLogNode where TaskID = " + nTaskID;
                var numCount = m_DBObject.CreateCommand(sql).ExecuteScalar<int>();
                return numCount;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Get DBLog's count Failed. taskId = {nTaskID} : error = {e.ToString()}");
                return 0;
            }
        }
    }
}
