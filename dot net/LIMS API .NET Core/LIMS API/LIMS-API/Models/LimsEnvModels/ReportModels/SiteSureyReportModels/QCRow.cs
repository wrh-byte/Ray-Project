using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.ReportModels.SiteSureyReportModels
{
    public class QCRow
    {
        /// <summary>
        /// 分析指标
        /// </summary>
        public string analyteName { get; set; }

        /// <summary>
        /// 是否有机分类(字体加粗)
        /// </summary>
        public bool isOrganicType { get; set; }

        /// <summary>
        /// 样品编号
        /// </summary>
        public string sampleCode { get; set; }

        /// <summary>
        /// 质控样品编号
        /// </summary>
        public string qcSampleCode { get; set; }

        /// <summary>
        /// 分析日期
        /// </summary>
        public string analysisDate { get; set; }

        /// <summary>
        /// 测定值
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; set; }

        /// <summary>
        /// 标准值范围
        /// </summary>
        public string standardRange { get; set; }

        /// <summary>
        /// 样品浓度
        /// </summary>
        public string sampleConcentration { get; set; }

        /// <summary>
        /// 加标量
        /// </summary>
        public string much { get; set; }

        /// <summary>
        /// 实测值
        /// </summary>
        public string measured { get; set; }

        /// <summary>
        /// 回收率控制范围
        /// </summary>
        public string controlRange { get; set; }

        /// <summary>
        /// 样品结果
        /// </summary>
        public string sampleResult { get; set; }

        /// <summary>
        /// 实测加标平行样品量μg
        /// </summary>
        public string m_markup { get; set; }

        /// <summary>
        /// 加标样品回收率％
        /// </summary>
        public string saRecoveryRate { get; set; }

        /// <summary>
        /// 加标平行样品回收率％
        /// </summary>
        public string parallelRecoveryRate { get; set; }

        /// <summary>
        /// 平均回收率％
        /// </summary>
        public string aveRecoveryRate { get; set; }

        /// <summary>
        /// 相对偏差%
        /// </summary>
        public string relativeDeviation { get; set; }

        /// <summary>
        /// 相对偏差控制范围％
        /// </summary>
        public string rdControlRange { get; set; }

        /// <summary>
        /// 平行样结果
        /// </summary>
        public string parallelResults { get; set; }

        /// <summary>
        /// 空白样品浓度
        /// </summary>
        public string concentration { get; set; }

        /// <summary>
        /// 平行样品结果
        /// </summary>
        public string parallelSampleResult { get; set; }

        /// <summary>
        /// 回收率%
        /// </summary>
        public string recoveryRate { get; set; }
    }
}
