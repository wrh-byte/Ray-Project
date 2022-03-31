using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsEnvModels
{
    /// <summary>
    /// ContractReviewModel
    /// </summary>
    public class ContractReviewModel
    {
        public string organizationId { get; set; }
        public string organizationName { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string clientName { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        public string contractNo { get; set; }

        /// <summary>
        /// 参保人
        /// </summary>
        public string protectPerson { get; set; }

        public string inspectionDate { get; set; }
        public string testItem { get; set; }
        public string remark { get; set; }
        public string approver { get; set; }
        public string approverDate { get; set; }
    }
}
