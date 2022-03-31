using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.ReportModels.SiteSureyReportModels
{
    public class SResultRow
    {
        /// <summary>
        /// 分析指标
        /// </summary>
        public string analyteName { get; set; }

        /// <summary>
        /// 是否有机分类(加粗体)
        /// </summary>
        public bool isOrganicType { get; set; }

        /// <summary>
        /// 方法编号
        /// </summary>
        public string methodCode { get; set; }

        /// <summary>
        /// 检出限
        /// </summary>
        public string detectionLimit { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; set; }

        /// <summary>
        /// 结果列
        /// </summary>
        public List<SResultColumn> resultColumns { get; set; }
    }
}
