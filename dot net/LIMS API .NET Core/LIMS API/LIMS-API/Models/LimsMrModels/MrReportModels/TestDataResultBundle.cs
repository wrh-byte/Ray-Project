using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsMrModels.MrReportModels
{
    public class TestDataResultBundle
    {
        /// <summary>
        /// 测试项目序号
        /// </summary>
        public string testItemNumber { get; set; }
        /// <summary>
        /// 测试项目名称
        /// </summary>
        public string testItemName { get; set; }
        /// <summary>
        /// 测试设备
        /// </summary>
        public List<TestInstrument> testInstruments { get; set; }
        /// <summary>
        /// 环境条件
        /// </summary>
        public string envCondition { get; set; }
        /// <summary>
        /// 测试标准
        /// </summary>
        public string testStandard { get; set; }
        /// <summary>
        /// 测试条件
        /// </summary>
        public string testCondition { get; set; }
        /// <summary>
        /// 测试结果
        /// </summary>
        public TestDataResultTable resultTable { get; set; }
        /// <summary>
        /// 测试图片名
        /// </summary>
        public List<string> testImageNameList { get; set; }
    }
}
