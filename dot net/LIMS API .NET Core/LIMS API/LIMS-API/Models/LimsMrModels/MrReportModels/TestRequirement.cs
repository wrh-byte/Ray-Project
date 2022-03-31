using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsMrModels.MrReportModels
{
    /// <summary>
    /// 测试要求
    /// </summary>
    public class TestRequirement
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string serialNumber { get; set; }

        /// <summary>
        /// 测试项目
        /// </summary>
        public string testProject { get; set; }
    }
}
