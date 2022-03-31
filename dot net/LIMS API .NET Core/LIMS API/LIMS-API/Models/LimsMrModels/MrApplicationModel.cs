using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsMrModels
{
    /// <summary>
    /// MrApplicationModel
    /// </summary>
    public class MrApplicationModel
    {
        /// <summary>
        /// organizationId
        /// </summary>
        public string organizationId { get; set; }

        /// <summary>
        /// organizationName
        /// </summary>
        public string organizationName { get; set; }

        //Information for Applicant
        /// <summary>
        /// 申请单号
        /// </summary>
        public string applicationNumber { get; set; }

        /// <summary>
        /// 客服名称
        /// </summary>
        public string csName { get; set; }

        /// <summary>
        /// 客服电话
        /// </summary>
        public string csPhone { get; set; }

        /// <summary>
        /// 客户联系人
        /// </summary>
        public string clientContactName { get; set; }

        /// <summary>
        /// 客户联系人电话
        /// </summary>
        public string clientContactTel { get; set; }

        /// <summary>
        /// 客户联系人移动电话
        /// </summary>
        public string clientContactPhone { get; set; }

        /// <summary>
        /// 客户联系人邮箱
        /// </summary>
        public string clientContactEmail { get; set; }

        /// <summary>
        /// 报告抬头公司名称
        /// </summary>
        public string reportTitleName { get; set; }

        /// <summary>
        /// 报告抬头公司地址
        /// </summary>
        public string reportTitleAddress { get; set; }

        //Testing Information

        /// <summary>
        /// 服务类型
        /// </summary>
        public string serviceType { get; set; }


        //Report
        /// <summary>
        /// 报告语言
        /// </summary>
        public string reportLanguage { get; set; }

        /// <summary>
        /// 报告布局
        /// </summary>
        public string reportLayout { get; set; }

        /// <summary>
        /// 加盖CMA
        /// </summary>
        public Boolean reportStampCMA { get; set; }

        /// <summary>
        /// 加盖CNAS
        /// </summary>
        public Boolean reportStampCNAS { get; set; }

        /// <summary>
        /// 报告版式
        /// </summary>
        public string reportForm { get; set; }

        /// <summary>
        /// 报告寄送地址选择
        /// </summary>
        public string reportDAPick { get; set; }

        /// <summary>
        /// 其他报告寄送地址存储
        /// </summary>
        public string reportDA { get; set; }

        //Invoice Requirement
        /// <summary>
        /// 发票类型
        /// </summary>
        public string invoiceType { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// 公司地址
        /// </summary>
        public string companyAddress { get; set; }

        /// <summary>
        /// 开户行名称
        /// </summary>
        public string bankName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string bankAccount { get; set; }

        /// <summary>
        /// 税号
        /// </summary>
        public string taxpayerNumber { get; set; }

        /// <summary>
        /// 公司电话
        /// </summary>
        public string companyTel { get; set; }
    }
}
