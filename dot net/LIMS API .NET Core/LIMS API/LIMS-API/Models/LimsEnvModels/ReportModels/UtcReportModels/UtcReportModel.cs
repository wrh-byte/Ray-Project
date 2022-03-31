using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LIMS_API.Models.ReportModels.FinalReportModels;

namespace LIMS_API.Models.LimsEnvModels.ReportModels.UtcReportModels
{
    /// <summary>
    /// UtcReportModel
    /// </summary>
    public class UtcReportModel : FinalReportModel
    {

        /// <summary>
        /// 采样依据
        /// </summary>
        public List<string> samplingGistList { get; set; }

        /// <summary>
        /// 评价依据
        /// </summary>
        public List<string> evaluationGistList { get; set; }

        /// <summary>
        /// 结果评价
        /// </summary>
        public string resultEvaluation { get; set; }

        /// <summary>
        /// 结果值
        /// </summary>
        public List<WaterSupplyResult> waterSupplyResult { get; set; }

        /// <summary>
        /// 技术说明
        /// </summary>
        public List<OtherInfo> otherInfoTable { get; set; }
    }
}
