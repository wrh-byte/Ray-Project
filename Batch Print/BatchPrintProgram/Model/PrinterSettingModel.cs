using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchPrintProgram.Model
{
    public class PrinterSettingModel
    {
        /// <summary>
        /// 颜色模式(如果该页应以彩色打印，则为 true；反之，则为 false。默认值由打印机决定。)
        /// </summary>
        public bool colorModel { get; set; }

        /// <summary>
        /// 单双面打印
        /// </summary>
        public Duplex printType { get; set; }

        /// <summary>
        /// 打印方向(如果页面应横向打印，则为 true；反之，则为 false。默认值由打印机决定。)
        /// </summary>
        public bool direction { get; set; }
    }
}
