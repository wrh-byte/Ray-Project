using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Blls.LimsMrBll;
using LIMS_API.Blls.LimsMRBll;
using LIMS_API.Models.LimsMrModels;
using LIMS_API.Models.LimsMRModels;
using LIMS_API.Models.LimsMRModels.LimsMRReportFileModel;
using LIMS_API.Models.LimsMrModels.MrBartenderModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace LIMS_API.Controllers.LimsMRController
{
    /// <summary>
    /// LimsMRController
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LimsMrController : Controller
    {

        /// <summary>
        /// CreateQuotation
        /// </summary>
        /// <param name="quotationModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Quotation(MrQuotationModel quotationModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = MrQuotationBll.CreateMrQuotation(quotationModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=ResponseFile&fileName=" + fileName + "&organizationId=" + quotationModel.organizationId + "&organizationName=" + quotationModel.organizationName,
                    success = true,
                });

            }
            catch (Exception exception)
            {
                LogHelper.Error("Error Message:" + exception.Message + " " + "StackTrace" + exception.StackTrace);
                return Json(new { success = false, error = exception.Message });
            }
        }

        /// <summary>
        /// CreateApplication
        /// </summary>
        /// <param name="applicationModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Application(MrApplicationModel applicationModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = MrApplicationBll.CreateApplication(applicationModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=ResponseFile&fileName=" + fileName + "&organizationId=" + applicationModel.organizationId + "&organizationName=" + applicationModel.organizationName,
                    success = true,
                });

            }
            catch (Exception exception)
            {
                LogHelper.Error("Error Message:" + exception.Message + " " + "StackTrace" + exception.StackTrace);
                return Json(new { success = false, error = exception.Message });
            }
        }


        /// <summary>
        /// LaboratoryJob
        /// </summary>
        /// <param name="labModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult LaboratoryJob(MrLaboratoryJobModel labModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = MrLaboratoryJobBll.CreateLaboratoryJob(labModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=ResponseFile&fileName=" + fileName + "&organizationId=" + labModel.organizationId + "&organizationName=" + labModel.organizationName,
                    success = true,
                });

            }
            catch (Exception exception)
            {
                LogHelper.Error("Error Message:" + exception.Message + " " + "StackTrace" + exception.StackTrace);
                return Json(new { success = false, error = exception.Message });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetBartenderLabelTemplateFilePath(MrGetBartenderLabelModel getModel)
        {
            GetFileResponseModel res = new GetFileResponseModel();
            try
            {
                string host = Request.Host.Value;
                PathManagementBll pm = new PathManagementBll(getModel.organizationName, getModel.organizationId, "LIMS-BARCODE-TEMP");
                string path = pm.GetTemplatePath("LabelPrintTemplate");
                DirectoryInfo folder = new DirectoryInfo(path);
                
                res.DownLoadUrls = new List<string>();
                foreach (var item in folder.GetFiles())
                {
                    res.DownLoadUrls.Add($"https://{host}/api/LimsMr/GetBartenderTemplateFile?orgId={getModel.organizationId}&orgName={getModel.organizationName}&fileName={item.Name}");
                }
                if (res.DownLoadUrls.Count>0)
                {
                    res.Success = true;
                }
                else
                {
                    res.Success = false;
                    res.Message = "No downLoadUrls";
                }
                return new ContentResult { Content = JsonConvert.SerializeObject(res), ContentType = "application/json" };
            }
            catch (Exception e)
            {
                LogHelper.Info(e.Message);
                res.Success = false;
                res.Message = e.Message;
                return new ContentResult { Content = JsonConvert.SerializeObject(res), ContentType = "application/json" };
            }
        }

        /// <summary>
        /// GetBartenderTemplateFile
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetBartenderTemplateFile(string orgId,string orgName,string fileName)
        {
            GetFileResponseModel res = new GetFileResponseModel();
            try
            {
                PathManagementBll pm = new PathManagementBll(orgName, orgId, "LIMS-MR-TEMP");
                string filePath = pm.GetTemplatePath($"LabelPrintTemplate/{fileName}");
                if (System.IO.File.Exists(filePath))
                {
                    return PhysicalFile(filePath, "application/octet-stream", fileName);
                }
                res.Success = false;
                res.Message = "file is not exists";
                return new ContentResult{Content=JsonConvert.SerializeObject(res), ContentType = "application/json"};
            }
            catch (Exception e)
            {
                LogHelper.Info(e.Message);
                res.Success = false;
                res.Error = e.Message;
                return new ContentResult { Content = JsonConvert.SerializeObject(res), ContentType = "application/json" };
            }
        }
    }
}
