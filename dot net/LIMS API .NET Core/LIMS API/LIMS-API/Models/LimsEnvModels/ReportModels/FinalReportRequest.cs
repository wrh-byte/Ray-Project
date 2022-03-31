using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace limsapi.Models
{
    public class FinalReportRequest
    {
        /// <summary>
        /// 报告文件名称
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// OrgId
        /// </summary>
        public string organizationId { get; set; }
        /// <summary>
        /// OrgName
        /// </summary>
        public string organizationName { get; set; }
        /// <summary>
        /// 报告批准人
        /// </summary>
        public string approver { get; set; }
    }
}
