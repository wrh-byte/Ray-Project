using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LIMS_API.Bll;
using LIMS_API.Blls.CommonBlls;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Controllers.LimsEnvController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileController : Controller
    {
        private static string RunPath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath;
        private static readonly string OtherFilePath = RunPath + "/OtherFile";

        /// <summary>
        /// UpLoadFile
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpLoadFile(IFormFile file)
        {
            if (file==null)
            {
                return Json(new
                {
                    error = "Upload File is Null",
                    success = false
                });
            }
            string host = Request.Host.Value;
            string savePath = OtherFilePath + "/"+ file.FileName;
            if (!Directory.Exists(OtherFilePath))
            {
                Directory.CreateDirectory(OtherFilePath);
            }
            file.CopyTo(new FileStream(savePath, FileMode.Create));
            return Json(new
            {
                downloadUrl = "https://" + host + "/api/File/GetFile?fileName=" + file.FileName,
                success = true
            });
        }

        /// <summary>
        /// GetFile
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetFile(string fileName)
        {
            try
            {
                var filePath = OtherFilePath + "/" + fileName;
                if (System.IO.File.Exists(filePath))
                {
                    return PhysicalFile(filePath, "application/octet-stream", fileName);
                }
                return Json(new { success = false });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// UpLoadExternalFile
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpLoadExternalFile(IFormFile file)
        {
            try
            {
                string host = Request.Host.Value;
                string orgId = Request.Form["organizationId"];
                string orgName = Request.Form["organizationName"];
                string folderName = Request.Form["folderName"];
                if (file == null || string.IsNullOrEmpty(orgId) || string.IsNullOrEmpty(orgName) || string.IsNullOrEmpty(folderName))
                {
                    return Json(new
                    {
                        success = false,
                        message = "organizationId or organizationName or folderName is null"
                    });
                }

                string orgPath = $"{RunPath}/{folderName}/{orgId}_{orgName}";
                //Rename The File
                string now = DateTime.Now.ToString("yyyyMMddhhmmss");
                string fileName = $"exf_{file.Name}_{now}.{file.FileName.Split('.')[1]}";
                string savePath = $"{orgPath}/{fileName}";
                if (!Directory.Exists(orgPath))
                {
                    Directory.CreateDirectory(orgPath);
                }
                file.CopyTo(new FileStream(savePath, FileMode.Create));

                return Json(new
                {
                    fileName = fileName,
                    downloadUrl = $"https://{host}/api/File/GetExternalFile?orgId={orgId}&orgName={orgName}&fileName={fileName}",
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
        /// GetExternalFile
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="folderName"></param>
        /// <param name="fileName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetExternalFile(string orgId, string orgName, string folderName, string fileName, long e, string token)
        {
            HttpContext.Response.Headers.Add("Content-Disposition", "inline;filename=" + fileName);
            string host = Request.Host.Value;
            string baseUrl = $"https://{host}/api/File/GetExternalFile?orgId={orgId}&orgName={orgName}&fileName={fileName}";
            baseUrl += "&e=" + e;
            string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
            long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
            if (e >= now)
            {
                if (secret == token)
                {
                    var filePath = $"{RunPath}/{folderName}/{orgId}_{orgName}/{fileName}";
                    if (System.IO.File.Exists(filePath))
                    {
                        return PhysicalFile(filePath, "application/octet-stream", fileName);
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
