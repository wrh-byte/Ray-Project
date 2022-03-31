using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using LIMS_API.Bll;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Blls.LimsEnvBlls.PersonnelBlls;
using LIMS_API.Models.PersonnelModels;

namespace LIMS_API.Controllers.LimsEnvController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LimsPersonnelController : Controller
    {
        
        [HttpPost]
        public IActionResult archivesTable(ArchivesModel arModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = ArchivesBll.createArchives(arModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=ResponseFile&fileName=" + fileName + "&organizationId=" + arModel.organizationId + "&organizationName=" + arModel.organizationName,
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
