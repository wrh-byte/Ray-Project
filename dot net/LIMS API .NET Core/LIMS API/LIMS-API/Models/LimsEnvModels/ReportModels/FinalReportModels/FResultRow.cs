using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.ReportModels.FinalReportModels
{
    public class FResultRow
    {
        public string samplingDate { get; set; }//采样时间
        public string samplingTime { get; set; }//采样时间
        public string frequency { get; set; }//采样频次
        public string analyteName { get; set; }//检测项目
        public string unit { get; set; }//单位
        public string samplingPointName { get; set; }//采样信息点位
        public List<FResultColumn> resultColumns { get; set; }
    }
}
