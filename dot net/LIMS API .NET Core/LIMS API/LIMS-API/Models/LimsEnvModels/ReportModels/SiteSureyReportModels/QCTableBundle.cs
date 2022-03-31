using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.ReportModels.SiteSureyReportModels
{
    public class QCTableBundle
    {
        /// <summary>
        /// 表格类型
        /// </summary>
        public string tableType { get; set; }

        /// <summary>
        /// 基质
        /// </summary>
        public string qcMatrix { get; set; }

        /// <summary>
        /// 分析日期
        /// </summary>
        public string analysisDate { get; set; }

        /// <summary>
        /// 样品编号
        /// </summary>
        public string sampleCode { get; set; }

        /// <summary>
        /// 受检单位/项目名称
        /// </summary>
        public string inspectedUnitName { get; set; }

        /// <summary>
        /// 报告编号
        /// </summary>
        public string reportCode { get; set; }

        /// <summary>
        /// 质控表格数据行
        /// </summary>
        public List<QCRow> qcRowList { get; set; }
    }
}
