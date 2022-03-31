using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LIMS_API.Bll;
using LIMS_API.Blls.CommonBlls;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Controllers.LimsEnvController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LimsInstrumentController : Controller
    {
        private static string RunPath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath;
        private static readonly string InstrumentFilePath = RunPath + "/InstrumentFile";

        /// <summary>
        /// UpLoadInstrumentFile
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpLoadInstrumentFile(IFormFile file)
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
                        success = false,
                        message = "organizationId or organizationName or folderName is null"
                    });
                }

                string orgPath = $"{InstrumentFilePath}/{orgId}_{orgName}";
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
                    downloadUrl = $"https://{host}/api/LimsInstrument/GetInstrumentFile?orgId={orgId}&orgName={orgName}&fileName={fileName}",
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
        /// GetInstrumentFile
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="fileName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetInstrumentFile(string orgId, string orgName, string fileName, long e, string token)
        {
            HttpContext.Response.Headers.Add("Content-Disposition", "inline;filename=" + fileName);
            string host = Request.Host.Value;
            string baseUrl = $"https://{host}/api/LimsInstrument/GetInstrumentFile?orgId={orgId}&orgName={orgName}&fileName={fileName}";
            baseUrl += "&e=" + e;
            string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
            long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
            if (e >= now)
            {
                if (secret == token)
                {
                    var filePath = $"{InstrumentFilePath}/{orgId}_{orgName}/{fileName}";
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
