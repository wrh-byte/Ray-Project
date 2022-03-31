using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LIMS_API.Blls.LimsEnvBlls.ReportBlls.UtcReportBlls;
using LIMS_API.Models.LimsEnvModels.ReportModels.UtcReportModels;
using LIMS_API.Blls.CommonBlls;
using System.IO;

namespace LIMS_API.Controllers.LimsEnvController
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LimsUtcReportController : Controller
    {
        /// <summary>
        /// 饮用水报告
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateDraftReport(UtcReportModel reportModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = UtcReportBll.CreateReport(reportModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    fileName = fileName,
                    fileType = fileName.Split('.')[1],
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=DraftReports&fileName=" + fileName + "&organizationId=" + reportModel.organizationId + "&organizationName=" + reportModel.organizationName,
                    success = true,
                });

            }
            catch (Exception exception)
            {
                LogHelper.Error("Error Message:" + exception.Message + " " + "StackTrace" + exception.StackTrace);
                return Json(new { success = false, error = exception.Message });
            }
        }
    }
}
