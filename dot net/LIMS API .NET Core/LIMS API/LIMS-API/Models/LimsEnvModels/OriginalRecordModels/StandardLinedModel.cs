using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.OriginalRecordModels
{
    public class StandardLinedModel
    {
        /// <summary>
        /// 曲线名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 曲线日期
        /// </summary>
        public string curveDate { get; set; }

        /// <summary>
        /// 相关系数
        /// </summary>
        public string correlationCoefficient { get; set; }

        /// <summary>
        /// 曲线公式
        /// </summary>
        public string curveFormula { get; set; }

        /// <summary>
        /// 其他信息
        /// </summary>
        public string remark { get; set; }
    }
}
