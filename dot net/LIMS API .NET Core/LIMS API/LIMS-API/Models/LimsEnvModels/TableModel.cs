using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models
{
    public class TableModel
    {
        /// <summary>
        /// 表格描述
        /// </summary>
        public String tableDescription { get; set; }

        /// <summary>
        /// 样品状态描述
        /// </summary>
        public String sampleRemark { get; set; }
        public List<Row> table { get; set; }
        public class Row
        {
            public List<String> column { get; set; }
        }
    }
}
