using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.ReportModels.FinalReportModels
{
    public class ResultBundle
    {
        /// <summary>
        /// 测试信息表
        /// </summary>
        public ResultTable testResultTable { get; set; }

        /// <summary>
        /// 采样信息表
        /// </summary>
        public ResultTable samplingResultTable { get; set; }
    }
}
