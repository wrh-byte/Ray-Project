using System;
using System.Collections.Generic;
using System.IO;

namespace LIMS_API.Models.ReportModels.SiteSureyReportModels
{
    /// <summary>
    /// 场调报告
    /// </summary>
    public class SiteSurveyReportModel:ReportBasicModel
    {
        #region Head

        /// <summary>
        /// 报告模板类型
        /// </summary>
        public string tempType { get; set; }

        #endregion

        #region MethodTable

        /// <summary>
        /// 分析样品数量
        /// </summary>
        public string sampleNum { get; set; }

        /// <summary>
        /// 分析样品状态
        /// </summary>
        public string sampleState { get; set; }


        /// <summary>
        /// 测试方法表格
        /// </summary>
        public List<SiteSureyMethodInfo> methodInfoTable { get; set; }

        #endregion

        //Organic and Inorganic TestResult

        /// <summary>
        /// 有机测试表格
        /// </summary>
        public List<List<SResultRow>> organicResultTableList { get; set; }

        /// <summary>
        /// 无机理化测试表格
        /// </summary>
        public List<List<SResultRow>> inorganicResuleTableList { get; set; }


        /// <summary>
        /// 质控表格
        /// </summary>
        public List<QCTableBundle> qcTable { get; set; }

    }
}
