using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.OriginalRecordModels
{
    public class AddStandardModel
    {
        public String analyteName { get; set; }//测试参数
        public String analyteUnit { get; set; }//测试参数单位
        public String value { get; set; }//质控样结果
        public String scalarAddition { get; set; }//理论加标量
        public String recoveryRate { get; set; }//回收率
        public String range { get; set; }//合格范围
    }
}
