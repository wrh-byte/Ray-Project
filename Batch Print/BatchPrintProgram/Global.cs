using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchPrintProgram.Model;

namespace BatchPrintProgram
{
    public static class Global
    {
        /// <summary>
        /// 全局变量下载地址
        /// </summary>
        public static string downloadJson { get; set; }
        /// <summary>
        /// 下载文件夹地址
        /// </summary>
        public static string downloadPath { get; set; }
        /// <summary>
        /// 文件下载识别码
        /// </summary>
        public static string fileCode { get; set; }
        /// <summary>
        /// 下载Model接口地址
        /// </summary>
        public static string getDateUrl { get; set; }
        /// <summary>
        /// OAuth2验证地址
        /// </summary>
        public static string oauthUrl { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public static string token { get; set; }
        /// <summary>
        /// 下载队列
        /// </summary>
        public static List<DownloadModel> downloadList=new List<DownloadModel>();
    }
}
