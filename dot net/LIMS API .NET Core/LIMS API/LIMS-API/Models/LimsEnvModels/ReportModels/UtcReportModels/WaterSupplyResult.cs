using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsEnvModels.ReportModels.UtcReportModels
{
    public class WaterSupplyResult
    {
        /// <summary>
        /// 样品编号
        /// </summary>
        public string sampleName { get; set; }
        /// <summary>
        /// 采样地点
        /// </summary>
        public string samplingPointName { get; set; }
        /// <summary>
        /// 检测项目名称
        /// </summary>
        public string analyteName { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 测试结果
        /// </summary>
        public string resultValue { get; set; }
        /// <summary>
        /// 限值
        /// </summary>
        public string limitValue { get; set; }
    }
}
