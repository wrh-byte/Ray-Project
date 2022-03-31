using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LIMS_API.Models
{
    public class ResponseModel
    {
        public string downLoadUrl { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }
    }
}