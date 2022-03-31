using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.OriginalRecordModels
{
    public class OriginalRecordResultColumnModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 直读结果
        /// </summary>
        public string valueRead { get; set; }
    }
}
