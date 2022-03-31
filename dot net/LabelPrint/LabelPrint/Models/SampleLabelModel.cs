using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint.Models
{
    public class SampleLabelModel
    {
        public string OrgId { get; set; }
        public string OrgName { get; set; }

        /// <summary>
        /// 选用什么模板进行打印
        /// </summary>
        public string Type { get; set; }
        public List<Data> Datas { get; set; }
    }

    public class Data
    {
        public int CopiesOfLabel { get; set; }
        public string Code { get; set; }
        public string CsName { get; set; }
        public string DueTime { get; set; }
        public string Remark { get; set; }
        public string BarCode { get; set; }
        public string Name { get; set; }
        public string TakeTime { get; set; }
    }
}
