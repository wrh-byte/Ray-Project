using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsMrModels
{
    /// <summary>
    /// UpdateOnlineWordModel
    /// </summary>
    public class UpdateOnlineWordModel
    {
        /// <summary>
        /// orgId
        /// </summary>
        public string orgId { get; set; }

        /// <summary>
        /// orgName
        /// </summary>
        public string orgName { get; set; }

        /// <summary>
        /// content
        /// </summary>
        public Dictionary<string,string> content { get; set; }
    }
}
