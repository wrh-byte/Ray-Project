using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIMS_API.Models.OriginalRecordModels
{
    public class BaseInfoModel
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string orderName { get; set; }

        /// <summary>
        /// 检测项目
        /// </summary>
        public string testItemName { get; set; }

        /// <summary>
        /// 检测人
        /// </summary>
        public string experimenterName { get; set; }

        /// <summary>
        /// 检测人签名图片
        /// </summary>
        public string experimenterSignImage { get; set; }

        /// <summary>
        /// 检测日期
        /// </summary>
        public string testData { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string reviewerName { get; set; }

        /// <summary>
        /// 审核人签名图片
        /// </summary>
        public string reviewerSignImage { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public string reviewData { get; set; }

        /// <summary>
        /// 测试设备名称
        /// </summary>
        public string testEquipmentName { get; set; }

        /// <summary>
        /// 测试设备编号
        /// </summary>
        public string testEquipmentCode { get; set; }

        /// <summary>
        /// 检测方法
        /// </summary>
        public string testStarndardName { get; set; }

        /// <summary>
        /// 样品基质
        /// </summary>
        public string matrixName { get; set; }

        /// <summary>
        /// 前处理日期
        /// </summary>
        public string pretreatmentDate { get; set; }

        /// <summary>
        /// 检出限
        /// </summary>
        public string limitsValue { get; set; }

        /// <summary>
        /// 空白格键
        /// </summary>
        public string blankKey { get; set; }

        /// <summary>
        /// 空白格值
        /// </summary>
        public string blankValue { get; set; }
    }
}
