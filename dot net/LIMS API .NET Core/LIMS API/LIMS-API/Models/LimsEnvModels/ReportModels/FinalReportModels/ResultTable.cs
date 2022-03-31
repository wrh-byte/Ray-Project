using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.ReportModels.FinalReportModels
{
    public class ResultTable
    {
        public bool isOrgGas { get; set; }//是否有组织废气
        public bool isNoise { get; set; }//是否为噪声
        public bool isLampblack { get; set; }//是否油烟
        public PointInfo samplingPoint { get; set; }//点位信息
        public bool frequency { get; set; }//是否有频次
        public bool isNotPoint { get; set; }//是否有点位/仅针对采样表格
        public string matrixName { get; set; }//被检测基质名称
        public string sampleRemark { get; set; }//样品状态描述
        public List<FResultRow> resultRows { get; set; }
    }
}
