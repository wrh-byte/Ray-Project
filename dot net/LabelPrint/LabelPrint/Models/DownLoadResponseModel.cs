using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint.Models
{
    public class DownLoadResponseModel
    {
        public List<string> DownLoadUrls { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }
}
