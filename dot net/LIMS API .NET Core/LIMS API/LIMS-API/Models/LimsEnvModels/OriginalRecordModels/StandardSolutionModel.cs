using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.OriginalRecordModels
{
    public class StandardSolutionModel
    {
        /// <summary>
        /// 标准溶液名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 配置日期
        /// </summary>
        public string configureDate { get; set; }
    }
}
