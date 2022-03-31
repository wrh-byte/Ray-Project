using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.OriginalRecordModels
{
    /// <summary>
    /// 原始记录信息
    /// </summary>
    public class OriginalRecordModel
    {
        /// <summary>
        /// orgName
        /// </summary>
        public string organizationName { get; set; }

        /// <summary>
        /// orgId
        /// </summary>
        public string organizationId { get; set; }

        /// <summary>
        /// 原始记录名称
        /// </summary>
        public string originalRecordName { get; set; }

        /// <summary>
        /// 原始记录编号
        /// </summary>
        public string originalRecordCode { get; set; }

        /// <summary>
        /// 基本信息
        /// </summary>
        public BaseInfoModel baseInfo { get; set; }

        /// <summary>
        /// 测试结果
        /// </summary>
        public OriginalRecordResultTableModel resultTable { get; set; }

        /// <summary>
        /// 标准曲线
        /// </summary>
        public StandardLinedModel standardLined { get; set; }

        /// <summary>
        /// 标准溶液
        /// </summary>
        public StandardSolutionModel standardSolution { get; set; }

        /// <summary>
        /// 标样
        /// </summary>
        public List<StandardSampleModel> standardSamples { get; set; }

        /// <summary>
        /// 平行样
        /// </summary>
        public List<QCTableModel> parallelSampleQCTables { get; set; }

        /// <summary>
        /// 空白加标
        /// </summary>
        public List<QCTableModel> blkAddStandardQCTables { get; set; }

        /// <summary>
        /// 基质加标
        /// </summary>
        public List<QCTableModel> addStandardQCTables { get; set; }

        /// <summary>
        /// 基质加标平行
        /// </summary>
        public List<QCTableModel> addStandardParalleQCTables { get; set; }
    }
}
