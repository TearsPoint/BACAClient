using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;
using Base.IO;
using Base.Ex;

namespace Base
{
    /// <summary>
    /// 日志记录者
    /// </summary>
    public static class Loger
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        static int _eventId;
        static int EventId
        {
            get { return _eventId++; }
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="errorSourceName"></param>
        /// <param name="errorMethodName"></param>
        /// <param name="ex"></param>
        /// <param name="type"></param>
        public static void WriteEntry(string errorSourceName, string errorMethodName, Exception exception, EventLogEntryType type)
        {
            try
            {
                WriteEntry(errorSourceName, CreateErrorMsg(exception, errorMethodName), type);
            }
            catch (Exception ex)
            {
                throw new Exception(CreateErrorMsg(ex, "Base.Loger.Log(...)"));
            }
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="errorSourceName"></param>
        /// <param name="errorMethodName"></param>
        /// <param name="ex"></param>
        /// <param name="type"></param>
        public static void WriteEntry(string errorSourceName, string errorMsg, EventLogEntryType type)
        {
            //using (EventLog el = new EventLog())
            //{
            //    el.Log = "";
            //    el.Source = errorSourceName;
            //    el.WriteEntry(errorMsg, type, EventId, 0);
            //}
        }
        static int _logNo = 0;
        /// <summary>
        /// 控制台日志
        /// </summary>
        /// <param name="info"></param>
        public static void ConsoleLog(string info, bool isLogStackTrace = false)
        {
            if (AppSetting.IsDebugHost)
            {
                _logNo++;
                Console.WriteLine(_logNo.ToString() + ": " + info);

                if (isLogStackTrace)
                {
                    StackTrace st = new StackTrace();
                    foreach (var item in st.GetFrames())
                    {
                        try
                        {
                            Console.WriteLine((item.GetFileName().IsNullOrWhiteSpace() ? "" : (item.GetFileName() + "\r")) + " \t" + item.GetMethod().FullName() + " " + item.GetFileLineNumber());
                        }
                        catch { }
                    }
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 记录日志到系统日志文件（Bin下的Log文件夹中）
        /// </summary>
        /// <param name="errorSourceName"></param>
        /// <param name="exception"></param>
        public static string Log(string errorSourceName, string errorMethodName, Exception exception)
        {
            try
            {
                WriteEntry(errorSourceName, CreateErrorMsg(exception, errorMethodName), EventLogEntryType.Error);
                ConsoleLog(errorSourceName + ":\r\n" + CreateErrorMsg(exception, errorMethodName));
                return CreateErrorJson(Log(errorSourceName, CreateErrorMsg(exception, errorMethodName)));
            }
            catch
            {
            }
            return CreateErrorJson("未知错误");
        }

        public static string CreateErrorJson(string err)
        {
            return new { err = err }.ToJson();
        }

        /// <summary>
        /// 文件超过3M，就新建一个文件，在之前的日志文件名后加上标志 【(_)】
        /// </summary>
        static int _fileNameAppend = 0;

        /// <summary>
        /// 记录日志到系统日志文件（Bin下的Log文件夹中）
        /// </summary>
        /// <param name="errorSourceName"></param>
        /// <param name="errorMsg"></param>
        public static string Log(string errorSourceName, string errorMsg)
        {
            StringBuilder currInfo = new StringBuilder();
            try
            {
                StringBuilder logInfo = new StringBuilder();
                DirectoryInfo di = new DirectoryInfo(Runtime.LogPath);
                if (!di.Exists) Directory.CreateDirectory(di.FullName);
                FileInfo[] fis = new FileInfo[] { };
                if (di.GetFiles().Count() > 0)
                    fis = di.GetFiles().Where(a => a.Name.Contains(DateTime.Now.ToString("yyyy-MM-dd"))).ToArray();

                foreach (var item in fis)
                {
                    int temp = 0;
                    if (item.Name.IndexOf("_(") > 0)
                        int.TryParse(item.Name.Substring(item.Name.IndexOf("_(") + 2, item.Name.IndexOf(")") - (item.Name.IndexOf("_(") + 2)), out temp);
                    if (temp > _fileNameAppend)
                        _fileNameAppend = temp;
                }

                string logFilePath = string.Empty;
                logFilePath = Path.Combine(Runtime.LogPath, string.Format("{0}{1}.txt", GlobalParameter.LogFilePrefixName, DateTime.Now.ToString("yyyy-MM-dd")));
                if (_fileNameAppend > 0)
                    logFilePath = Path.Combine(Runtime.LogPath, string.Format("{0}{1}_({2}).txt", GlobalParameter.LogFilePrefixName, DateTime.Now.ToString("yyyy-MM-dd"), _fileNameAppend));

                if (File.Exists(logFilePath))
                {
                    FileInfo fi = new FileInfo(logFilePath);
                    if (fi.Length / (1024 * 1024) > 3)
                    {
                        logFilePath = Path.Combine(Runtime.LogPath, string.Format("{0}{1}_({2}).txt", GlobalParameter.LogFilePrefixName, DateTime.Now.ToString("yyyy-MM-dd"), ++_fileNameAppend));
                    }
                }

                using (FileStream fs = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        logInfo.Append(sr.ReadToEnd());
                    }
                }

                using (FileStream fs = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    currInfo.AppendLine(string.Format("{0}:{1}", errorSourceName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    currInfo.AppendLine(errorMsg);

                    logInfo.AppendLine(currInfo.ToString());
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(logInfo.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WriteEntry(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex, EventLogEntryType.Error);
            }
            return currInfo.ToString();
        }

        /// <summary>
        /// 记录日志信息
        /// </summary>
        /// <param name="errorSourceName"></param>
        /// <param name="errorMsg"></param>
        public static string Log(string logFileName, string title, string errorMsg)
        {
            StringBuilder currInfo = new StringBuilder();
            try
            {
                StringBuilder logInfo = new StringBuilder();
                DirectoryInfo di = new DirectoryInfo(Runtime.LogPath);
                FileInfo[] fis = new FileInfo[] { };
                if (di.GetFiles().Count() > 0)
                    fis = di.GetFiles().Where(a => a.Name.Contains(DateTime.Now.ToString("yyyy-MM-dd"))).ToArray();

                //foreach (var item in fis)
                //{
                //    int temp = 0;
                //    if (item.Name.IndexOf("_(") > 0)
                //        int.TryParse(item.Name.Substring(item.Name.IndexOf("_(") + 2, item.Name.IndexOf(")") - (item.Name.IndexOf("_(") + 2)), out temp);
                //    if (temp > _fileNameAppend)
                //        _fileNameAppend = temp;
                //}

                string logFilePath = string.Empty;
                logFilePath = Path.Combine(Runtime.LogPath, string.Format("{0}{1}.txt", GlobalParameter.LogFilePrefixName, logFileName));
                if (_fileNameAppend > 0)
                    logFilePath = Path.Combine(Runtime.LogPath, string.Format("{0}{1}_({2}).txt", GlobalParameter.LogFilePrefixName, logFileName, _fileNameAppend));

                if (File.Exists(logFilePath))
                {
                    FileInfo fi = new FileInfo(logFilePath);
                    if (fi.Length / (1024 * 1024) > 3)
                    {
                        logFilePath = Path.Combine(Runtime.LogPath, string.Format("{0}{1}_({2}).txt", GlobalParameter.LogFilePrefixName, DateTime.Now.ToString("yyyy-MM-dd"), ++_fileNameAppend));
                    }
                }

                using (FileStream fs = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        logInfo.Append(sr.ReadToEnd());
                    }
                }

                using (FileStream fs = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    currInfo.AppendLine(errorMsg);
                    logInfo.AppendLine(errorMsg);
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(logInfo.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WriteEntry(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex, EventLogEntryType.Error);
            }
            return currInfo.ToString();
        }
        /// <summary>
        /// 创建异常消息串
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="errorMethodName"></param>
        /// <returns></returns>
        public static string CreateErrorMsg(this Exception ex, string errorMethodName = "")
        {
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendLine(errorMethodName);
            if (!string.IsNullOrEmpty(ex.Message)) errorMsg.AppendLine(ex.Message);
            if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message)) errorMsg.AppendLine(ex.InnerException.Message);
            if (!string.IsNullOrEmpty(ex.StackTrace)) errorMsg.AppendLine(ex.StackTrace);
            return errorMsg.ToString();
        }
    }

}
