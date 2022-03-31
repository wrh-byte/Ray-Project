using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsMrModels.MrReportModels
{
    public class ResultRow
    {
        /// <summary>
        /// 样品名称
        /// </summary>
        public string sampleName { get; set; }
        public List<ResultColumn> resultColumns { get; set; }
    }
}
