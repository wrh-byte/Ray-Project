using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LIMS_API.Bll;
using LIMS_API.Bll.LimsEnvBlls;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Blls.LimsEnvBlls.InvoiceBlls;
using LIMS_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Controllers.LimsEnvController
{

    /// <summary>
    /// Invoice相关
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LimsInvoiceController : Controller
    {
        private static string RunPath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath;
        private static string _invoiceAssociatedFilePath = RunPath + "/InvoiceAssociatedFile";
        /// <summary>
        /// 打印对账单
        /// </summary>
        /// <param name="bsModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PrintBankStatement(BankStatementModel bsModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = BankStatementBll.createBankStatement(bsModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    fileName = fileName,
                    fileType = fileName.Split('.')[1],
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=ResponseFile&fileName=" + fileName + "&organizationId=" + bsModel.organizationId + "&organizationName=" + bsModel.organizationName,
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
        /// 发票外部文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpLoadInvoiceFile(IFormFile file)
        {
            try
            {
                string host = Request.Host.Value;
                string orgId = Request.Form["organizationId"];
                string orgName = Request.Form["organizationName"];
                if (file == null || string.IsNullOrEmpty(orgId) || string.IsNullOrEmpty(orgName))
                {
                    return Json(new
                    {
                        success = false
                    });
                }

                string orgPath = $"{_invoiceAssociatedFilePath}/{orgId}_{orgName}";
                //Rename The File
                string now = DateTime.Now.ToString("yyyyMMddhhmmss");
                string fileName = $"invoiceFile_{file.Name}_{now}.{file.FileName.Split('.')[1]}";
                string savePath = $"{orgPath}/{fileName}";
                if (!Directory.Exists(orgPath))
                {
                    Directory.CreateDirectory(orgPath);
                }
                file.CopyTo(new FileStream(savePath, FileMode.Create));

                return Json(new
                {
                    fileName = fileName,
                    downloadUrl = $"https://{host}/api/LimsInvoice/GetInvoiceFile?orgId={orgId}&orgName={orgName}&fileName={fileName}",
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
        /// 获取发票外部文件
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="fileName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetInvoiceFile(string orgId, string orgName, string fileName, long e, string token)
        {
            HttpContext.Response.Headers.Add("Content-Disposition", "inline;filename=" + fileName);
            string host = Request.Host.Value;
            string baseUrl = $"https://{host}/api/LimsInvoice/GetInvoiceFile?orgId={orgId}&orgName={orgName}&fileName={fileName}";
            baseUrl += "&e=" + e;
            string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
            long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
            if (e >= now)
            {
                if (secret == token)
                {
                    var filePath = $"{_invoiceAssociatedFilePath}/{orgId}_{orgName}/{fileName}";
                    if (System.IO.File.Exists(filePath))
                    {
                        FileInfo fi = new FileInfo(filePath);
                        FileStream fs = fi.OpenRead();
                        byte[] buffer = new byte[fi.Length];
                        fs.Read(buffer, 0, Convert.ToInt32(fi.Length));
                        var resp = File(buffer, "application/word");
                        fs.Close();
                        return resp;
                    }
                    return Json(new { success = false });
                }
                else
                {
                    return Json(new { message = "Token is not valid", success = false });
                }
            }
            else
            {
                return Json(new { message = "Time is expiration", success = false });
            }
        }
    }
}
