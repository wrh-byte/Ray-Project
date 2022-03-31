using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.ReportModels
{
    /// <summary>
    /// 报告共有
    /// </summary>
    public class ReportBasicModel
    {
        /// <summary>
        /// OrgId
        /// </summary>
        public string organizationId { get; set; }

        /// <summary>
        /// OrgName
        /// </summary>
        public string organizationName { get; set; }

        /// <summary>
        /// watermark
        /// </summary>
        public bool watermark { get; set; }

        /// <summary>
        /// 报告名称
        /// </summary>
        public string reportName { get; set; }

        /// <summary>
        /// OrderName
        /// </summary>
        public string orderName { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string clientName { get; set; }

        /// <summary>
        /// 客户地址
        /// </summary>
        public string clientAddress { get; set; }

        /// <summary>
        /// 受检单位/项目名称
        /// </summary>
        public string inspectedUnitName { get; set; }
        /// <summary>
        /// 受检单位/项目地址
        /// </summary>
        public string inspectedUnitAddress { get; set; }
        /// <summary>
        /// 首页公司名称
        /// </summary>
        public string companyName { get; set; }
        /// <summary>
        /// 页脚公司地址
        /// </summary>
        public string companyAddress { get; set; }
        /// <summary>
        /// 页脚公司联系电话
        /// </summary>
        public string companyPhone { get; set; }
        /// <summary>
        /// 编制人
        /// </summary>
        public string editor { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string reviewer { get; set; }
        /// <summary>
        /// 报告备注
        /// </summary>
        public string reportRemark { get; set; }
        /// <summary>
        /// 接收样品日期 - 场调报告
        /// </summary>
        public string getSampleDate { get; set; }
        /// <summary>
        /// 采样日期 - 普通报告
        /// </summary>
        public string samplingDate { get; set; }
        /// <summary>
        /// 分析日期/测试日期
        /// </summary>
        public string analysisDate { get; set; }
        /// <summary>
        /// 分包备注
        /// </summary>
        public string subRemark { get; set; }

        /// <summary>
        /// 报告封面生成日期
        /// </summary>
        public string generateDate { get; set; }

    }
}
