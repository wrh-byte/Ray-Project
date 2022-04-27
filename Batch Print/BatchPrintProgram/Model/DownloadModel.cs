using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchPrintProgram.Model
{
    public class DownloadModel
    {
        /// <summary>
        /// 文件下载地址
        /// </summary>
        public string fileUrl { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// 打印份数
        /// </summary>
        public short copies { get; set; }
    }
}
