using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsMrModels.MrReportModels
{
    /// <summary>
    /// MrReportModel
    /// </summary>
    public class MrReportModel
    {
        /// <summary>
        /// public string organizationId { get; set; }
        /// </summary>
        public string organizationId { get; set; }
        /// <summary>
        /// organizationName
        /// </summary>
        public string organizationName { get; set; }
        #region Header
        /// <summary>
        /// 报告编号
        /// </summary>
        public string reportCode { get; set; }
        /// <summary>
        /// 申请单位
        /// </summary>
        public string apply { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 样品名称
        /// </summary>
        public string sampleName { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 材质
        /// </summary>
        public string texture { get; set; }
        /// <summary>
        ///样品接收日期
        /// </summary>
        public string receivingDate { get; set; }
        /// <summary>
        /// 样品检测日期
        /// </summary>
        public string inspectionDate { get; set; }
        /// <summary>
        /// 检测要求
        /// </summary>
        public List<TestRequirement> testRequirementList { get; set; }
        /// <summary>
        /// 批准人
        /// </summary>
        public string approver { get; set; }
        /// <summary>
        /// 批准日期
        /// </summary>
        public string approvalDate { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string companyName { get; set; }
        /// <summary>
        /// 公司地址
        /// </summary>
        public string companyAddress { get; set; }
        #endregion


        #region SampleInfo
        /// <summary>
        /// 测试样品信息
        /// </summary>
        public List<MrReportSampleInfo> sampleInfoList { get; set; }
        /// <summary>
        /// 样品图片列表
        /// </summary>
        public List<string> sampleImageInfoList { get; set; }

        #endregion
        /// <summary>
        /// resultBundleList
        /// </summary>
        public List<TestDataResultBundle> resultBundleList { get; set; }

    }
}
