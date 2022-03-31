using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.ReportModels.SiteSureyReportModels
{
    public class SResultColumn
    {
        /// <summary>
        /// 样品编号
        /// </summary>
        public string sampleNum { get; set; }

        /// <summary>
        /// 样品标识
        /// </summary>
        public string sampleMark { get; set; }

        /// <summary>
        /// 采样日期
        /// </summary>
        public string samplingDate { get; set; }

        /// <summary>
        /// 接收日期
        /// </summary>
        public string receivedDate { get; set; }

        /// <summary>
        /// 基质名称
        /// </summary>
        public string matrixName { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
    }
}
