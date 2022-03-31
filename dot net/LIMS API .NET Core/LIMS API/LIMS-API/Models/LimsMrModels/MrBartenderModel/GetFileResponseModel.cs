using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsMrModels.MrBartenderModel
{
    public class GetFileResponseModel
    {
        public List<string> DownLoadUrls { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }
}
