using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.OriginalRecordModels
{
    public class AddStandardParalleModel
    {
        public string analyteName { get; set; }//测试参数
        public string analyteUnit { get; set; }//测试参数单位
        public string scalarAddition { get; set; }//理论加标量
        public string recoveryRate { get; set; }//基质加标回收率
        public string recoveryRateParaller { get; set; }//基质加标平行回收率
        public string averageRecoveryRate { get; set; }//平均回收率
        public string relativeDeviation { get; set; }//相对偏差
        public string range { get; set; }//合格范围
    }
}
