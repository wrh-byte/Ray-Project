using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LIMS_API.Models
{
    /// <summary>
    /// 采样工作单
    /// </summary>
    public class SamplingWordOrderModel
    {
        public string organizationId { get; set; }
        public string organizationName { get; set; }
        /// <summary>
        /// 采样工作单编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 采样工作单识别码
        /// </summary>
        public string securityCode { get; set; }

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
        /// 项目类型
        /// </summary>
        public string projectType { get; set; }

        /// <summary>
        /// 采样地址
        /// </summary>
        public string samplingAddress { get; set; }

        /// <summary>
        /// 技术要求
        /// </summary>
        public string technicalRequirement { get; set; }

        /// <summary>
        /// 采样开始时间
        /// </summary>
        public string samplingStartDate { get; set; }

        /// <summary>
        /// 采样结束时间
        /// </summary>
        public string samplingEndDate { get; set; }

        /// <summary>
        /// 采样备注
        /// </summary>
        public string samplingRemark { get; set; }

        /// <summary>
        /// 采样计划
        /// </summary>
        public List<SamplingPlan> samplingPlanList { get; set; }

        public class SamplingPlan
        {
            /// <summary>
            /// 基质
            /// </summary>
            public string matrix { get; set; }
            /// <summary>
            /// 检测项目/参数
            /// </summary>
            public string project { get; set; }

            /// <summary>
            /// 分析方法/标准
            /// </summary>
            public string analyticalMethod { get; set; }
            /// <summary>
            /// 样品点位
            /// </summary>
            public string point { get; set; }
            /// <summary>
            /// 样品频次
            /// </summary>
            public string frequency { get; set; }
            /// <summary>
            /// 样品天数周期
            /// </summary>
            public string cycle { get; set; }

            /// <summary>
            /// 参考限值标准
            /// </summary>
            public string referLimitStandard { get; set; }

            /// <summary>
            /// 参考限值
            /// </summary>
            public string referLimit { get; set; }
        }

        /// <summary>
        /// 控制采样方案是否显示
        /// </summary>
        public bool samplingProgramDisplay { get; set; }

        /// <summary>
        /// 采样方案
        /// </summary>
        public List<SamplingProgram> samplingProgramList { get; set; }

        public class SamplingProgram
        {
            /// <summary>
            /// 检测项目/参数
            /// </summary>
            public string project { get; set; }

            /// <summary>
            /// 采样仪器
            /// </summary>
            public string samplingEquipment { get; set; }

            /// <summary>
            /// 采样耗材
            /// </summary>
            public string samplingMaterial { get; set; }

            /// <summary>
            /// 采样流量
            /// </summary>
            public string samplingFlow { get; set; }

            /// <summary>
            /// 采样时间	
            /// </summary>
            public string samplingTime { get; set; }

            /// <summary>
            /// 最小样品量
            /// </summary>
            public string minSampleSize { get; set; }

            /// <summary>
            /// 样品保存条件
            /// </summary>
            public string sampleKeepCondition { get; set; }

            /// <summary>
            /// 样品时效性
            /// </summary>
            public string sampleTimeliness { get; set; }

            /// <summary>
            /// 固定剂
            /// </summary>
            public string fixative { get; set; }

            /// <summary>
            /// 采样注意事项
            /// </summary>
            public string samplingConsiderations { get; set; }

            /// <summary>
            /// 空白
            /// </summary>
            public string samplingBlank { get; set; }
        }
    }
}