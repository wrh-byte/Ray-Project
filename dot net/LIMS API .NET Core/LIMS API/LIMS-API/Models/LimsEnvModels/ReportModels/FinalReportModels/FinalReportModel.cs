using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.ReportModels.FinalReportModels
{
    /// <summary>
    /// FinalReportModel
    /// </summary>
    public class FinalReportModel:ReportBasicModel
    {
        /// <summary>
        /// 检测内容
        /// </summary>
        public string testContent { get; set; }
        /// <summary>
        /// 界定采样现场图片文件夹
        /// </summary>
        public string orderId { get; set; }
        /// <summary>
        /// 采样现场图片List
        /// </summary>
        public List<string> samplingImageList { get; set; }

        #region 测试结果表格
        /// <summary>
        /// 测试采样结果 1对1
        /// </summary>
        public List<ResultBundle> resultBundleList { get; set; }
        #endregion

        #region 检测项目及仪器
        public List<OtherInfo> otherInfoTable { get; set; }
        /// <summary>
        /// 分包备注
        /// </summary>
        public string subRemark { get; set; }

        #endregion
    }
}
