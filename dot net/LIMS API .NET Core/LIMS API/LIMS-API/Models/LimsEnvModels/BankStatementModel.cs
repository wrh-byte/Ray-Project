using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models
{
    public class BankStatementModel
    {
        /// <summary>
        /// Salesforce OrgId
        /// </summary>
        public string organizationId { get; set; }

        /// <summary>
        /// Salesforce OrgName
        /// </summary>
        public string organizationName { get; set; }

        /// <summary>
        /// 发票名称
        /// </summary>
        public string invoiceName { get; set; }

        /// <summary>
        /// 委托单位名称
        /// </summary>
        public string client { get; set; }

        /// <summary>
        /// 委托单位地址
        /// </summary>
        public string clientAddress { get; set; }

        /// <summary>
        /// 委托联系人
        /// </summary>
        public string clientContact { get; set; }

        /// <summary>
        /// 委托联系人电话
        /// </summary>
        public string clientContactPhone { get; set; }

        /// <summary>
        /// 受检单位
        /// </summary>
        public string inspectedUnit { get; set; }

        /// <summary>
        /// 受检单位地址
        /// </summary>
        public string inspectedUnitAddress { get; set; }

        /// <summary>
        /// 项目开始时间
        /// </summary>
        public string startDate { get; set; }

        /// <summary>
        /// 项目结束时间
        /// </summary>
        public string endDate { get; set; } 

        /// <summary>
        /// 受检单位联系人
        /// </summary>
        public string inspectedUnitContact { get; set; }

        /// <summary>
        /// 受检单位联系电话
        /// </summary>
        public string inspectedUnitPhone { get; set; }

        /// <summary>
        /// 客服姓名
        /// </summary>
        public string customerServiceName { get; set; }

        /// <summary>
        /// 客服电话
        /// </summary>
        public string customerServicePhone { get; set; }

        /// <summary>
        /// 检测计划 费用明细
        /// </summary>
        public List<MTestingPlan> testingPlanList { get; set; }
        public class MTestingPlan
        {
            /// <summary>
            /// 基质
            /// </summary>
            public String matrix { get; set; }
            /// <summary>
            /// 项目内容
            /// </summary>
            public String projectContent { get; set; }

            /// <summary>
            /// 测试费用
            /// </summary>
            public String cost { get; set; }

            /// <summary>
            /// 点位
            /// </summary>
            public String point { get; set; }

            /// <summary>
            /// 频次
            /// </summary>
            public String frequency { get; set; }

            /// <summary>
            /// 周期
            /// </summary>
            public String cycle { get; set; }

            /// <summary>
            /// 小计(元)
            /// </summary>
            public String subtotal { get; set; }
        }


        /// <summary>
        /// 测试费用小计
        /// </summary>
        public string testCost { get; set; }

        /// <summary>
        /// 现场勘察及差旅费用
        /// </summary>
        public string siteInvestigationAmount { get; set; }

        /// <summary>
        /// 费用总计
        /// </summary>
        public string totalAmount { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public string taxRate { get; set; }

        /// <summary>
        /// 税收管理费
        /// </summary>
        public string taxManagementAmount { get; set; }

        /// <summary>
        /// 费用合计
        /// </summary>
        public string totalAmountInTax { get; set; }

        /// <summary>
        /// 最终优惠价格
        /// </summary>
        public string finalPrice { get; set; }

        /// <summary>
        /// 发票抬头(公司名称)
        /// </summary>
        public string invoiceHeader { get; set; }

        /// <summary>
        /// 发票类型
        /// </summary>
        public string invoiceType { get; set; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string invoiceTaxpayerNumber { get; set; }

        /// <summary>
        /// 公司地址
        /// </summary>
        public string invoiceCompanyAddress { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string invoicePhone { get; set; }

        /// <summary>
        /// 开户行名称
        /// </summary>
        public string invoiceBankName { get; set; }

        /// <summary>
        /// 开户行账户
        /// </summary>
        public string invoiceBankAccount { get; set; }

        /// <summary>
        /// 合作公司名称
        /// </summary>
        public string cooCompanyName { get; set; }

        /// <summary>
        /// 合作公司地址
        /// </summary>
        public string cooCompanyAddress { get; set; }

        /// <summary>
        /// 合作公司税号
        /// </summary>
        public string cooCompanyTaxNumber { get; set; }

        /// <summary>
        /// 合作公司开户行
        /// </summary>
        public string cooCompanyBankName { get; set; }

        /// <summary>
        /// 合作公司开户行账户
        /// </summary>
        public string cooCompanyBankAccount { get; set; }
    }
}
