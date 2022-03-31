using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LIMS_API.Models.LimsMrModels.MrReportModels;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Blls.LimsMrBll.MrReportBlls;

namespace LIMS_API.Controllers.LimsMrController
{
    /// <summary>
    /// ALL Report Info Is Here
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LimsMrReportController : Controller
    {
        /// <summary>
        /// CreateReport
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateReport(MrReportModel reportModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = MrReportBll.CreateCommonReport(reportModel);
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
