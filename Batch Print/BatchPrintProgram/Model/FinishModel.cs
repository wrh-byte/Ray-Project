using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchPrintProgram.Model
{
    public class FinishModel
    {
        /// <summary>
        /// 打印次序
        /// </summary>
        public int order { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// 打印文件路径
        /// </summary>
        public string filepath { get; set; }

        /// <summary>
        /// 打印状态
        /// </summary>
        public string status { get; set; }
    }
}
