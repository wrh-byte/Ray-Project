using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models
{
    public class QuotationModel
    {
        public string organizationId { get; set; }
        public string organizationName { get; set; }

        /// <summary>
        /// 委托书编号
        /// </summary>
        public string quotationCode { get; set; }


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
        /// 分包费用小计
        /// </summary>
        public string subtotalcosts { get; set; }

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
        /// 项目类型
        /// </summary>
        public string projectType { get; set; }

        /// <summary>
        /// 测试周期
        /// </summary>
        public string testCycle { get; set; }


        /// <summary>
        /// 报告语言
        /// </summary>
        public string reportLanguage { get; set; }

        /// <summary>
        /// 是否加盖CMA章
        /// </summary>
        public bool stampCMA { get; set; }

        /// <summary>
        /// 余样处理
        /// </summary>
        public string resSampleTreatment { get; set; }

        /// <summary>
        /// 测试方法
        /// </summary>
        public string proTestMethod { get; set; }

        /// <summary>
        /// 检测分包
        /// </summary>
        public string subcontract { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

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
        public string taxpayerNumber { get; set; }

        /// <summary>
        /// 公司地址
        /// </summary>
        public string companyAddress { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 开户行名称
        /// </summary>
        public string bankName { get; set; }

        /// <summary>
        /// 开户行账户
        /// </summary>
        public string bankAccount { get; set; }

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


        /// <summary>
        /// 检测计划 费用明细
        /// </summary>
        public List<TestingPlan> testingPlanList { get; set; }
        public class TestingPlan
        {
            /// <summary>
            /// 基质
            /// </summary>
            public string matrix { get; set; }
            /// <summary>
            /// 项目内容
            /// </summary>
            public string projectContent { get; set; }

            /// <summary>
            /// 测试费用
            /// </summary>
            public string cost { get; set; }

            /// <summary>
            /// 点位
            /// </summary>
            public string point { get; set; }

            /// <summary>
            /// 频次
            /// </summary>
            public string frequency { get; set; }

            /// <summary>
            /// 周期
            /// </summary>
            public string cycle { get; set; }

            /// <summary>
            /// 小计(元)
            /// </summary>
            public string subtotal { get; set; }
        }

        /// <summary>
        /// 分包项目
        /// </summary>
        public List<TestingPlan> subcontractList { get; set; }


        /// <summary>
        /// 检测方法依据
        /// </summary>
        public List<TestStandard> testStandardList { get; set; }
        public class TestStandard
        {
            /// <summary>
            /// 基质
            /// </summary>
            public string matrix { get; set; }

            /// <summary>
            /// 检测项目
            /// </summary>
            public string projectName { get; set; }

            /// <summary>
            /// 检测标准名称
            /// </summary>
            public string standardName { get; set; }
        }
    }
}
