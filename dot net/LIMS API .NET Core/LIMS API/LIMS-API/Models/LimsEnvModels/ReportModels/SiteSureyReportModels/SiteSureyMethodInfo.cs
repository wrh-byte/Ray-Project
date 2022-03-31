using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.ReportModels.SiteSureyReportModels
{
    public class SiteSureyMethodInfo
    {            /// <summary>
        /// 样品类型
        /// </summary>
        public string sampleType { get; set; }

        /// <summary>
        /// 检测项目
        /// </summary>
        public string testProject { get; set; }

        /// <summary>
        /// 检测方法依据
        /// </summary>
        public string testStandard { get; set; }

        /// <summary>
        /// 检出限
        /// </summary>
        public string detectionLimit { get; set; }

        /// <summary>
        /// 检测设备名称及型号
        /// </summary>
        public string equipmentName { get; set; }

        /// <summary>
        /// 出厂编号
        /// </summary>
        public string serialNumber { get; set; }
    }
}
