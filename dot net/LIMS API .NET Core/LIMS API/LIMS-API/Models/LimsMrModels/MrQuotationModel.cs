using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsMRModels
{
    /// <summary>
    /// MRQuotationModel
    /// </summary>
    public class MrQuotationModel
    {
        /// <summary>
        /// organizationId
        /// </summary>
        public string organizationId { get; set; }

        /// <summary>
        /// organizationName
        /// </summary>
        public string organizationName { get; set; }

        /// <summary>
        /// quotationNo
        /// </summary>
        public string quotationNo { get; set; }

        /// <summary>
        /// sendingDate
        /// </summary>
        public string sendingDate { get; set; }

        /// <summary>
        /// clientName
        /// </summary>
        public string clientName { get; set; }

        /// <summary>
        /// clientContactName
        /// </summary>
        public string clientContactName { get; set; }

        /// <summary>
        /// clientContactTel
        /// </summary>
        public string clientContactTel { get; set; }

        /// <summary>
        /// clientContactEmail
        /// </summary>
        public string clientContactEmail { get; set; }

        /// <summary>
        /// quotationDetailList
        /// </summary>
        public List<QuotationDetail> quotationDetailList { get; set; }

        /// <summary>
        /// 测试费用小计
        /// </summary>
        public string testAmount { get; set; }

        /// <summary>
        /// 其他费用
        /// </summary>
        public string otherFee { get; set; }

        /// <summary>
        /// 费用合计
        /// </summary>
        public string totalAmount { get; set; }

        /// <summary>
        /// 增值税
        /// </summary>
        public string addedValueTax { get; set; }

        /// <summary>
        /// 费用合计含税
        /// </summary>
        public string totalAmountInTax { get; set; }

        /// <summary>
        /// 最终价格
        /// </summary>
        public string discountAmountInTax { get; set; }

        /// <summary>
        /// 其他费用小计明细
        /// </summary>
        public string otherFeeDetail { get; set; }

        /// <summary>
        /// account
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// accountNo
        /// </summary>
        public string accountNo { get; set; }

        /// <summary>
        /// accountBank
        /// </summary>
        public string accountBank { get; set; }

        /// <summary>
        /// applicationNumber
        /// </summary>
        public string applicationNumber { get; set; }

        /// <summary>
        /// ctiContactName
        /// </summary>
        public string ctiContactName { get; set; }

        /// <summary>
        /// ctiContactTel
        /// </summary>
        public string ctiContactTel { get; set; }

        /// <summary>
        /// ctiContactFax
        /// </summary>
        public string ctiContactFax { get; set; }

        /// <summary>
        /// ctiContactNo
        /// </summary>
        public string ctiContactNo { get; set; }

        /// <summary>
        /// ctiContactEmail
        /// </summary>
        public string ctiContactEmail { get; set; }

        /// <summary>
        /// QuotationDetail
        /// </summary>
        public class QuotationDetail
        {
            public string sampleName { get; set; }
            public string serviceItem { get; set; }
            public string testWay { get; set; }
            public string unitPrice { get; set; }
            public string quantity { get; set; }
            public string serviceType { get; set; }
            public string serviceFee { get; set; }
            public string amount { get; set; }
        }
    }
}
