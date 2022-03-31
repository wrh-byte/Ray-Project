using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsMrModels
{
    /// <summary>
    /// MrLaboratoryJobModel
    /// </summary>
    public class MrLaboratoryJobModel
    {
        /// <summary>
        /// organizationId
        /// </summary>
        public string organizationId { get; set; }

        /// <summary>
        /// organizationName
        /// </summary>
        public string organizationName { get; set; }

        public string laboratoryGroupName { get; set; }
        public string orderName { get; set; }
        public string reportLanguage { get; set; }
        public string reportLayout { get; set; }
        public string settlementDate { get; set; }
        public string submitDate { get; set; }
        public string serviceType { get; set; }
        public string isStamp { get; set; }
        public string csName { get; set; }
        public string remark { get; set; }
        public string projectQualification { get; set; }
        public List<TestInfo> testInfoList { get; set; }
        public List<SampleInfo> sampleInfoList { get; set; }
        public List<TestDetail> testDetailList { get; set; }

        public class TestInfo
        {
            public string testStep { get; set; }
            public string testGroup { get; set; }
            public string testProject { get; set; }
            public string testSample { get; set; }
        }

        public class SampleInfo
        {
            public string sampleCode { get; set; }
            public string sampleName { get; set; }
            public string sampleAttribute { get; set; }
            public string testProject { get; set; }
        }

        public class TestDetail
        {
            public string testCode { get; set; }
            public string testProject { get; set; }
            public string testNumber { get; set; }
            public string analyte { get; set; }
            public string testWay { get; set; }
            public string testCondition { get; set; }
        }
    }
}
