using System;
using System.Threading.Tasks;
using NLog;

namespace NBCZ.Common
{
     public class LogHelper
    {
        private Logger logger;

        public LogHelper(Logger log)
        {
            this.logger = log;
        }

        public void Info(string message)
        {
            this.logger.Info(message);
        }

        public void Info(string message, Exception e)
        {
            this.logger.Info(message, e);
        }

        public void Debug(string message)
        {
            this.logger.Debug(message);
        }

        public void Debug(string message, Exception e)
        {
            this.logger.Debug(message, e);
        }

        public void Warning(string message)
        {
            this.logger.Warn(message);
        }

        public void Warning(string message, Exception e)
        {
            this.logger.Warn(message, e);
        }

        public void Error(string message)
        {
            this.logger.Error(message);
        }

        public void Error(string message, Exception e)
        {
            this.logger.Error(message, e);
        }

        public void Fatal(string message)
        {
            this.logger.Fatal(message);
        }

        public void Fatal(string message, Exception e)
        {
            this.logger.Fatal(message, e);
        }

        /// <summary>
        /// Request日志写入
        /// </summary>
        public static void WriteRequestLog(LogLevel logLevel, string message)
        {
            Task.Run(() =>
            {
                LogHelper m_Log = LogFactory.GetLogger(LogType.RequestLog);
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        m_Log.Debug(message);
                        break;
                    case LogLevel.Error:
                        m_Log.Error(message);
                        break;
                    case LogLevel.Info:
                        m_Log.Info(message);
                        break;
                    case LogLevel.Warning:
                        m_Log.Warning(message);
                        break;
                }

            });

        }
    }

     /// <summary>
     /// 日志类型
     /// </summary>
     public enum LogType
     {
         GlobalLog,
         RequestLog
     }

     /// <summary>
     /// 日志等级
     /// </summary>
     public enum LogLevel
     {
         Error,
         Debug,
         Warning,
         Info
     }

    public class LogFactory
    {
        static LogFactory()
        {
        }


        public static LogHelper GetLogger(Type type)
        {
            return new LogHelper(LogManager.GetLogger(type.FullName));
        }

        public static LogHelper GetLogger(string str)
        {
            return new LogHelper(LogManager.GetLogger(str));
        }

        public static LogHelper GetLogger(LogType logType)
        {
            return new LogHelper(LogManager.GetLogger(logType.ToString()));
        }
    }
}
