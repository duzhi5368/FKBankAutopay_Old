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
using System.Collections.Generic;
using FKBaseUtils;
// ===============================================================================
namespace FKLog
{
    /// <summary>
    /// 任务日志时间表管理器
    /// Comment: 通过该表查找到指定TaskID所在的日志文件，
    ///           然后再读取日志文件获取指定TaskID的日志记录
    /// </summary>
    public class FKSQLiteLogMgr : FKSingleton<FKSQLiteLogMgr>
    {
        private FKSQLiteLogConnection   m_SQLiteObject = null;      // 数据库连接器对象
        private SQLiteCommand           m_SQLiteCmd = null;         // 等待被指定的数据库命令

        private FKSQLiteLogMgr()
        {

        }

        /// <summary>
        /// 对外接口：添加一条Task流水日期映射表
        /// </summary>
        /// <param name="nTaskID"></param>
        /// <param name="time"></param>
        public bool AddTaskTimeLog(int nTaskID, DateTime time)
        {
            try
            {
                FKSQLiteLogTimeNode node = new FKSQLiteLogTimeNode();
                node.TaskId = nTaskID;
                node.LogTime = time.ToString("yyyy-MM-dd");

                return AddTaskTimeLogToDB(node);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Add task time log failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 获取指定Task的日志所在的数据库名
        /// </summary>
        /// <param name="nTaskID"></param>
        /// <returns></returns>
        public List<string> GetTaskLogDBFileName(int nTaskID)
        {
            List<string> TaskTimesList = null;
            if (m_SQLiteObject == null)
            {
                // 初次创建DB文件
                FKSystemFileSystemHelper.CreateDir(FKSystemFileSystemHelper.GetWorkdir() + "\\" + FKLogConsts.SQLITE_LOG_DIR_NAME);
                string str = FKSystemFileSystemHelper.GetWorkdir() + "\\" + FKLogConsts.SQLITE_LOG_DIR_NAME
                    + "\\" + FKLogConsts.SQLITE_LOG_TASK_TIME_FILE_NAME;
                m_SQLiteObject = new FKSQLiteLogConnection(str);
                if (m_SQLiteObject == null)
                {
                    return null;
                }
            }

            try
            {
                string sql = "select * from SDBTaskNode where TaskID = " + nTaskID;
                m_SQLiteCmd = m_SQLiteObject.CreateCommand(sql);

                var results = m_SQLiteCmd.ExecuteQuery<FKSQLiteLogTimeNode>().ToArray();
                if (results.Length <= 0)
                {
                    return null;
                }
                
                TaskTimesList = new List<string>();
                for (int i = 0; i < results.Length; i++)
                {
                    //存在不同taskid对应的记录在一个db中的情况,需要进行路径过滤
                    string dateDBName = FKSystemFileSystemHelper.GetWorkdir() + "\\" + FKLogConsts.SQLITE_LOG_DIR_NAME
                        + "\\" + results[i].LogTime + FKLogConsts.SQLITE_LOG_DIR_NAME;

                    bool bIsAlreadyInclude = false;
                    foreach (string strIt in TaskTimesList)
                    {
                        if (string.Compare(strIt, dateDBName) == 0) // 文件已记录在列表中
                        {
                            bIsAlreadyInclude = true;
                            break;
                        }
                    }
                    if (!bIsAlreadyInclude)
                    {
                        TaskTimesList.Add(dateDBName);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"[Error] Select task time Failed. taskId = {nTaskID} : error = {e.ToString()}" );
                return null;
            }
            return TaskTimesList;
        }

        /// <summary>
        /// 是否有指定DB文件
        /// </summary>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        private bool IsDBFileExist(string strFileName)
        {
            try
            {
                if (m_SQLiteObject == null)
                return false;

                FKSystemFileSystemHelper.CreateDir(FKSystemFileSystemHelper.GetWorkdir() 
                    + "\\" + FKLogConsts.SQLITE_LOG_DIR_NAME);
                return (m_SQLiteObject.DatabasePath.CompareTo(FKSystemFileSystemHelper.GetWorkdir() + "\\"
                    + FKLogConsts.SQLITE_LOG_DIR_NAME + "\\" + strFileName) == 0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Check if a DB file existed failed. Error = {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 创建本数据库
        /// </summary>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        private FKSQLiteLogConnection CreateDBFile(string strFileName)
        {
            FKSQLiteLogConnection db = null;
            try
            {
                FKSystemFileSystemHelper.CreateDir(FKSystemFileSystemHelper.GetWorkdir() 
                    + "\\" + FKLogConsts.SQLITE_LOG_DIR_NAME);
                string str = FKSystemFileSystemHelper.GetWorkdir() + "\\" 
                    + FKLogConsts.SQLITE_LOG_DIR_NAME + "\\" + strFileName;

                db = new FKSQLiteLogConnection(str);
                db.CreateTable<FKSQLiteLogTimeNode>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Create DB file Failed. Error = {e.ToString()}");
                return null;
            }
            return db;
        }

        /// <summary>
        /// 添加一条Task流水日期映射表
        /// </summary>
        /// <param name="node"></param>
        private bool AddTaskTimeLogToDB(FKSQLiteLogTimeNode node)
        {
            try
            {
                if (!IsDBFileExist(FKLogConsts.SQLITE_LOG_TASK_TIME_FILE_NAME))
                {
                    m_SQLiteObject = CreateDBFile(FKLogConsts.SQLITE_LOG_TASK_TIME_FILE_NAME);
                }
                return (m_SQLiteObject.Insert(node) > 0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Error] Add task time log to DB Failed. Error = {e.ToString()}");
                return false;
            }
        }
    }
}
