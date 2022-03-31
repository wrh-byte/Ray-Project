using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.OriginalRecordModels
{
    public class QCTableModel
    {
        /// <summary>
        /// 质控类型:平行样-空白加标-基质加标-基质加标平行
        /// </summary>
        public string qcType { get; set; }

        /// <summary>
        /// 质控编号
        /// </summary>
        public string sampleCode { get; set; }

        /// <summary>
        /// 加标溶液编号
        /// </summary>
        public string qcTypeCode { get; set; }

        /// <summary>
        /// 平行样
        /// </summary>
        public List<ParallelSampleModel> parallelSamples { get; set; }

        /// <summary>
        /// 空白加标
        /// </summary>
        public List<AddStandardModel> blkAddStandards { get; set; }

        /// <summary>
        /// 基质加标
        /// </summary>
        public List<AddStandardModel> addStandards { get; set; }

        /// <summary>
        /// 基质加标平行
        /// </summary>
        public List<AddStandardParalleModel> addStandardParalles { get; set; }
    }
}
