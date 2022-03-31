using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.OriginalRecordModels
{
    public class StandardSampleModel
    {
        /// <summary>
        /// 标样编号
        /// </summary>
        public string standardSampleCode { get; set; }

        /// <summary>
        /// 标物名称
        /// </summary>
        public string qcTypeCode { get; set; }

        /// <summary>
        /// 测试参数
        /// </summary>
        public string analyteName { get; set; }

        /// <summary>
        /// 测试参数单位
        /// </summary>
        public string analyteUnit { get; set; }

        /// <summary>
        /// 标样测试值
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 标准值范围
        /// </summary>
        public string range { get; set; }

        /// <summary>
        /// 是否合格
        /// </summary>
        public string isQualified { get; set; }
    }
}
