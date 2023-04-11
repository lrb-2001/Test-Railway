using FrameModel;
using log4net;
using MongoDB.Driver;
using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace FrameCommon;

public class LogLock
{
    private static readonly ILog log = LogManager.GetLogger(typeof(LogLock));

    static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
    static int WritedCount = 0;
    static int FailedCount = 0;
    static string _contentRoot = string.Empty;
    static bool isLocal = AppSettings.app(new string[] { "isLocal" }).ObjToBool();
    private static SqlSugarClient sqlSugarWrite;//写
    public LogLock(string contentPath)
    {
        _contentRoot = contentPath;
        //var result = SqlSugarHelper.GetInstance();
        var result = SqlSugarHelper.GetInstance(GlobalConfig.frameCoreAgileConfig.connectionConfig.frameCon);
        sqlSugarWrite = result["write"];
    }

    /// <summary>
    /// 获取方法名
    /// </summary>
    /// <returns></returns>
    static string GetCurrentMethodFullName()
    {
        try
        {
            int depth = 2;
            StackTrace st = new StackTrace();
            int maxFrames = st.GetFrames().Length;
            StackFrame sf;
            string methodName, className;
            Type classType;
            do
            {
                sf = st.GetFrame(depth++);
                classType = sf.GetMethod().DeclaringType;
                className = classType.ToString();
            } while (className.EndsWith("Exception") && depth < maxFrames);
            methodName = sf.GetMethod().Name;
            return className + "." + methodName;
        }
        catch (Exception e)
        {
            log4net.LogManager.GetLogger("Core.Log").Error(e.Message, e);
            return "获取方法名失败";
        }
    }

    /// <summary>
    /// 日志记录 -- 信息
    /// </summary>
    /// <param name="info">内容</param>
    /// <param name="actionName">位置</param>
    /// <param name="isLocal">是否存入本地文件</param>
    public static void Info(string info, string actionName, bool _isLocal = true)
    {
        isLocal = isLocal && _isLocal;
        if (actionName == null)
            actionName = GetCurrentMethodFullName();
        if (isLocal)
        {
            Log("Info", info, actionName);
        }
        AddSqlServerDBLog("Info", actionName, info);
    }

    /// <summary>
    /// 日志记录 -- 错误
    /// </summary>
    /// <param name="info">内容</param>
    /// <param name="actionName">位置</param>
    /// <param name="isLocal">是否存入本地文件</param>
    public static void Error(string info, string actionName = null, bool _isLocal = true)
    {
        isLocal = isLocal && _isLocal;
        if (actionName == null)
            actionName = GetCurrentMethodFullName();
        if (isLocal)
        {
            Log("Error", info, actionName);
        }
        AddSqlServerDBLog("Error", actionName, info);
    }

    /// <summary>
    /// 日志记录 -- Debug
    /// </summary>
    /// <param name="request">入参</param>
    /// <param name="actionName">位置</param>
    /// <param name="result">返参</param>
    /// <param name="isLocal">是否存入本地文件</param>
    public static void Debug(string request, string actionName, string result, bool _isLocal = true)
    {
        isLocal = isLocal && _isLocal;
        if (actionName == null)
            actionName = GetCurrentMethodFullName();
        if (isLocal)
        {
            Log("Debug", request, actionName, result);
        }
        AddSqlServerDBLog("Debug", actionName, "Request：" + request + "\r\nResult：" + result);
    }

    public static void Log(string type, string info, string actionName, string result = "")
    {
        try
        {
            //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
            //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
            //      从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
            //      因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
            LogWriteLock.EnterWriteLock();

            string folderName = string.Format("Log{0}", DateTime.Now.ToString("yyyyMMdd"));
            string fileUrl = string.Format("Log/{0}", folderName);
            var folderPath = Path.Combine(_contentRoot, fileUrl);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var allFiles = new DirectoryInfo(folderPath);
            var logFilePath = string.Empty;

            logFilePath = Path.Combine(folderPath, $"logInfo.log");
            string logContent = string.Empty;
            string msg = info;
            switch (type)
            {
                case "Error":
                    logContent = (
                          DateTime.Now + $"\r\n" +
                          $"Error-->>{actionName} \r\n 错误信息：" + info + "\r\n\r\n"
                          );
                    break;
                case "Info":
                    logContent = (
                          DateTime.Now + $"\r\n" +
                          $"Info-->>{actionName} \r\n " + info + "\r\n\r\n"
                          );
                    break;
                case "Debug":
                    logContent = (
                          DateTime.Now + $"\r\n" +
                          $"Debug-->> {actionName}\r\nRequest：" + info + "\r\nResult：" + result + "\r\n\r\n"
                          );
                    msg = "Request：" + info + "\r\nResult：" + result;
                    break;
                default:
                    break;
            }

            AddMongoDBLog(type, actionName, msg);

            File.AppendAllText(logFilePath, logContent);
            WritedCount++;
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
            FailedCount++;
        }
        finally
        {
            //退出写入模式，释放资源占用
            //注意：一次请求对应一次释放
            //      若释放次数大于请求次数将会触发异常[写入锁定未经保持即被释放]
            //      若请求处理完成后未释放将会触发异常[此模式不下允许以递归方式获取写入锁定]
            LogWriteLock.ExitWriteLock();
        }
    }

    /// <summary>
    /// 日志记录文件类型
    /// </summary>
    /// <param name="prefix">方法名</param>
    /// <param name="request">入参字符串</param>
    /// <param name="result">返参字符串</param>
    /// <param name="IsHeader"></param>
    public static void OutLogFile(string prefix, string request, string result, bool IsHeader = true)
    {
        try
        {
            //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
            //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
            //      从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
            //      因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
            LogWriteLock.EnterWriteLock();

            string folderName = string.Format("ServicesLog{0}", DateTime.Now.ToString("yyyyMMdd"));
            string fileUrl = string.Format("Log/{0}", folderName);
            var folderPath = Path.Combine(_contentRoot, fileUrl);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var logFilePath = GetAvailableFileWithPrefixOrderSize(folderPath, prefix);

            var now = DateTime.Now;
            string logContent = String.Join("\r\n", request);
            if (IsHeader)
            {
                logContent = (
                   "--------------------------------\r\n" +
                   DateTime.Now + "|\r\n" +
                   String.Join("\r\nstrRequest:", request) + "\r\n" +
                   String.Join("\r\nstrReturn:", result) + "\r\n"
                   );
            }

            //if (logContent.IsNotEmptyOrNull() && logContent.Length > 500)
            //{
            //    logContent = logContent.Substring(0, 500) + "\r\n";
            //}

            File.AppendAllText(logFilePath, logContent);
            WritedCount++;
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
            FailedCount++;
        }
        finally
        {
            //退出写入模式，释放资源占用
            //注意：一次请求对应一次释放
            //      若释放次数大于请求次数将会触发异常[写入锁定未经保持即被释放]
            //      若请求处理完成后未释放将会触发异常[此模式不下允许以递归方式获取写入锁定]
            LogWriteLock.ExitWriteLock();
        }


    }

    /// <summary>
    /// 根据文件大小获取指定前缀的可用文件名
    /// </summary>
    /// <param name="folderPath">文件夹</param>
    /// <param name="prefix">文件前缀</param>
    /// <param name="size">文件大小(1m)</param>
    /// <param name="ext">文件后缀(.log)</param>
    /// <returns>可用文件名</returns>
    public static string GetAvailableFileWithPrefixOrderSize(string folderPath, string prefix, int size = 1 * 1024 * 1024, string ext = ".log")
    {
        var allFiles = new DirectoryInfo(folderPath);
        var selectFiles = allFiles.GetFiles().Where(fi => fi.Name.ToLower().Contains(prefix.ToLower()) && fi.Extension.ToLower() == ext.ToLower() && fi.Length < size).OrderByDescending(d => d.Name).ToList();

        if (selectFiles.Count > 0)
        {
            return selectFiles.FirstOrDefault().FullName;
        }

        return Path.Combine(folderPath, $@"{prefix}_{DateTime.Now.DateToTimeStamp()}.log");
    }


    /// <summary>
    /// MongoDB数据添加数据
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="actionName">位置</param>
    /// <param name="msg">内容</param>
    public static void AddMongoDBLog(string type, string actionName, string msg)
    {
        string connection = AppSettings.app(new string[] { "MongoDB", "Connection" });
        //与Mongodb建立连接
        MongoClient client = new MongoClient(connection);
        //获得数据库,没有则自动创建
        IMongoDatabase db = client.GetDatabase("LogDB");

        IMongoCollection<LogModel> log = db.GetCollection<LogModel>("Logs");
        LogModel data = new LogModel()
        {
            ID = Guid.NewGuid().ToString().Replace("-", ""),
            CreateTime = DateTime.Now,
            LogType = type,
            ClassName = "LogLock",
            Domain = actionName,
            LoggerName = "Logs",
            Message = msg
        };
        //添加一条数据
        log.InsertOne(data);

    }

    /// <summary>
    /// SqlServer添加数据
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="actionName">位置</param>
    /// <param name="msg">内容</param>
    public static void AddSqlServerDBLog(string type, string actionName, string msg)
    {
        try
        {
            LogModel data = new LogModel()
            {
                ID = Guid.NewGuid().ToString().Replace("-", ""),
                CreateTime = DateTime.Now,
                LogType = type,
                ClassName = "LogLock",
                Domain = actionName,
                LoggerName = "Logs",
                Message = msg
            };
            var result = sqlSugarWrite.Insertable<LogModel>(data).ExecuteCommand();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

}

public static class UtilConvert
{
    /// <summary>
    /// 获取当前时间的时间戳
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static string DateToTimeStamp(this DateTime thisValue)
    {
        TimeSpan ts = thisValue - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }
}
