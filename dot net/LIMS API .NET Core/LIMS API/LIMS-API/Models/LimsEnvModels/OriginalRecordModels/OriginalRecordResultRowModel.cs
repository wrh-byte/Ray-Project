using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.OriginalRecordModels
{
    /// <summary>
    /// 原始记录行
    /// </summary>
    public class OriginalRecordResultRowModel
    {
        public string analyteId { get; set; }

        /// <summary>
        /// 测试参数code
        /// </summary>
        public string analyteCode { get; set; }

        /// <summary>
        /// 测试参数
        /// </summary>
        public string analyteName { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string analyteUnit { get; set; }

        /// <summary>
        /// 检出限
        /// </summary>
        public string stdLimit { get; set; }

        /// <summary>
        /// 列信息
        /// </summary>
        public List<OriginalRecordResultColumnModel> resultColumns { get; set; }
    }
}
