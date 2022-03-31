using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models
{
    public class TestResultModel
    {
        public String organizationId { get; set; }
        public String organizationName { get; set; }
        public String pageHeader { get; set; }
        public String orderName { get; set; }
        public String clientName { get; set; }
        public String clientAddress { get; set; }
        public String companyName { get; set; }
        public String companyAddress { get; set; }
        public String companyPhone { get; set; }
        public String inspectedUnitName { get; set; }
        public String inspectedUnitAddress { get; set; }
        public String matrix { get; set; }
        public String samplingStartDate { get; set; }//采样开始时间
        public String samplingEndDate { get; set; }//采样结束时间
        public String testStartDate { get; set; }//检测开始时间
        public String testEndDate { get; set; }//检测结束时间
        public TableModel testResultTable { get; set; }
    }
}
