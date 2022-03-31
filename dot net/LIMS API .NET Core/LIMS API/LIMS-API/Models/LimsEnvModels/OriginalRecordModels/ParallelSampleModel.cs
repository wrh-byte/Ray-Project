using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.OriginalRecordModels
{
    public class ParallelSampleModel
    {
        public String analyteName { get; set; }//测试参数
        public String analyteUnit { get; set; }//测试参数单位
        public String sampleValue { get; set; }//样品结果
        public String parallelSampleValue { get; set; }//平行样结果
        public String relativeDeviation { get; set; }//相对偏差
        public String range { get; set; }//合格范围
    }
}
