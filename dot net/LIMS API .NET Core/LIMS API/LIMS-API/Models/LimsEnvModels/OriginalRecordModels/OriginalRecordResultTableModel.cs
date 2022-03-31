using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.OriginalRecordModels
{
    public class OriginalRecordResultTableModel
    {
        /// <summary>
        /// 公式
        /// </summary>
        public string formula { get; set; }

        /// <summary>
        /// 平均值
        /// </summary>
        public List<string> valueAVEs { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 检出限
        /// </summary>
        public string stdLimit { get; set; }

        /// <summary>
        /// 结果行
        /// </summary>
        public List<OriginalRecordResultRowModel> resultRows { get; set; }
    }
}
