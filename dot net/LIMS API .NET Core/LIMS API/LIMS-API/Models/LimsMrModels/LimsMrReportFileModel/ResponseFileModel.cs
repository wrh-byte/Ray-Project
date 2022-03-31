using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.LimsMRModels.LimsMRReportFileModel
{
    /// <summary>
    /// ResponseFile
    /// </summary>
    public class ResponseFileModel
    {
        /// <summary>
        /// fileInfos
        /// </summary>
        public List<FileInfo> fileInfos { get; set; }

        /// <summary>
        /// isSuccess
        /// </summary>
        public bool isSuccess { get; set; }

        /// <summary>
        /// message
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// error
        /// </summary>
        public string error { get; set; }

        public class FileInfo
        {
            public string downLoadUrl { get; set; }
            public string fileName { get; set; }
        }
    }
}
