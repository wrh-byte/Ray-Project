using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models
{
    public class LaboratoryJobModel
    {
        public string organizationId { get; set; }
        public string organizationName { get; set; }

        /// <summary>
        /// 实验室工作单标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 识别码
        /// </summary>
        public string securityCode { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string orderCode { get; set; }

        /// <summary>
        /// 收样日期
        /// </summary>
        public string receivedSampleDate { get; set; }

        /// <summary>
        /// 报告日期
        /// </summary>
        public string reportDate { get; set; }

        /// <summary>
        /// 实验室出结果日期
        /// </summary>
        public string resultDate { get; set; }

        /// <summary>
        /// 现场负责人姓名
        /// </summary>
        public string principalName { get; set; }

        /// <summary>
        /// 现场负责人电话
        /// </summary>
        public string principalPhone { get; set; }
        public string remark { get; set; }
        public List<SampleTestList> samplingTestList { get; set; }

        public class SampleTestList
        {
            /// <summary>
            /// 采样日期
            /// </summary>
            public string samplingDate { get; set; }

            /// <summary>
            /// 样品编号
            /// </summary>
            public string sampleCode { get; set; }

            /// <summary>
            /// 点位
            /// </summary>
            public string pozition { get; set; }

            /// <summary>
            /// 点位名称
            /// </summary>
            public string pozitionName { get; set; }

            /// <summary>
            /// 基质
            /// </summary>
            public string matrixName { get; set; }

            /// <summary>
            /// 样品状态
            /// </summary>
            public string samplestatus { get; set; }

            /// <summary>
            /// 检测项目
            /// </summary>
            public string detectionProject { get; set; }

            /// <summary>
            /// 检测方法
            /// </summary>
            public string detectionMethod { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string remark { get; set; }

        }
	}
}
