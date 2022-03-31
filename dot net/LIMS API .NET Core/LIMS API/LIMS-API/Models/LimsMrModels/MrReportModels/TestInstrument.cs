using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsMrModels.MrReportModels
{
    public class TestInstrument
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string instrumentName { get; set; }
        /// <summary>
        /// 设备型号
        /// </summary>
        public string instrumentCode { get; set; }
    }
}
