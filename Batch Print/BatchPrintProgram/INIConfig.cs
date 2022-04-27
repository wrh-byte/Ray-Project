using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WorldPrint;

namespace BatchPrintProgram
{
    class INIConfig
    {
        private string path;

        public INIConfig(string path)
        {
            this.path = path;
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string iniPath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
            int size, string filePath);
        /// <summary>
        /// 创建InI文件
        /// </summary>
        public void CreateConfig()
        {
            try
            {
                if (!File.Exists(path))
                {
                    FileStream fileStream = new FileStream(path, FileMode.Create);
                    fileStream.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogManager.Severity.Error, $"Create Folder Error: {exception.Message}");
                throw;
            }

        }
        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="printerName"></param>
        public void WriteToInI(string section,string key,string value)
        {
            try
            {
                WritePrivateProfileString(section,key,value,this.path);
            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogManager.Severity.Error, $"Create Folder Error: {exception.Message}");
                throw;
            }
        }

        /// <summary>
        /// 从配置文件读取信息
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        public string GetInIValue(string section, string key)
        {
            try
            {
                StringBuilder temp = new StringBuilder(255);
                GetPrivateProfileString(section, key, "", temp, 255, this.path);
                return temp.ToString();
            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogManager.Severity.Error, $"Create Folder Error: {exception.Message}");
                throw;
            }
        }
    }
}
