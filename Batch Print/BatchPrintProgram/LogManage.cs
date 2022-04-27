using System;
using System.IO;
using System.Windows.Forms;

namespace WorldPrint
{
    public static class LogManager
    {
        private static string logPath = string.Empty;

        private static string logFielPrefix = "打印日志";
        /// <summary>
        /// 日志文件前缀
        /// </summary>
        public static string LogFielPrefix
        {
            get { return logFielPrefix; }
            set { logFielPrefix = value; }
        }

        /// <summary>
        /// 保存日志的文件夹
        /// </summary>
        public static string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    //logPath = Application.StartupPath;
                    logPath = Application.UserAppDataPath;
                    //if (System.Web.HttpContext.Current == null)
                    //    // Windows Forms 应用
                    //    logPath = AppDomain.CurrentDomain.BaseDirectory;
                    //else
                    //    // Web 应用
                    //    logPath = AppDomain.CurrentDomain.BaseDirectory + @"bin\";
                }
                return logPath;
            }
            set { logPath = value; }
        }



        /// <summary>
        /// 写日志
        /// </summary>
        public static void WriteLog(string logFile, string msg)
        {
            try
            {
                string savePath = LogPath + "/logs/";

                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                System.IO.StreamWriter sw = System.IO.File.AppendText(
                    savePath + LogFielPrefix + logFile + " " +
                    DateTime.Now.ToString("yyyyMMdd") + ".Log.txt"
                    );
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + msg);
                sw.Close();
            }
            catch
            { }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public static void WriteLog(Severity logFile, string msg)
        {
            WriteLog(logFile.ToString(), msg);
        }


        /// <summary>
        /// 日志类型
        /// </summary>
        public enum Severity
        {
            Trace,
            Debug,
            Warning,
            Error
        }
    }
}
