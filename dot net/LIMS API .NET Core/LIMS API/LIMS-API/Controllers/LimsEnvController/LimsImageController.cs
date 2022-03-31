using System;
using System.IO;
using LIMS_API.Bll;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Controllers.LimsEnvController
{
    /// <summary>
    /// LimsImageController
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LimsImageController : Controller
    {
        private static string RunPath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath;
        private static string SamplingSiteImagePath = RunPath + "/SamplingSiteImage";
        private static string SignImagePath = RunPath + "/SignImage";


        /// <summary>
        /// UpLoadSamplingSiteImage
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpLoadSamplingSiteImage(IFormFile image)
        {
            try
            {
                string host = Request.Host.Value;
                string orgId = Request.Form["organizationId"];
                string orgName = Request.Form["organizationName"];
                string orderId = Request.Form["orderId"];
                if (image == null || string.IsNullOrEmpty(orgId) || string.IsNullOrEmpty(orgName))
                {
                    return Json(new
                    {
                        success = false
                    });
                }

                string orderPath = $"{SamplingSiteImagePath}/{orgId}_{orgName}/{orderId}";
                //Rename The File
                string now = DateTime.Now.ToString("yyyyMMddhhmmss");
                string imageName = $"Image_{orderId}_{now}.{image.FileName.Split('.')[1]}";
                string savePath = $"{orderPath}/{imageName}";
                if (!Directory.Exists(orderPath))
                {
                    Directory.CreateDirectory(orderPath);
                }
                image.CopyTo(new FileStream(savePath, FileMode.Create));

                return Json(new
                {
                    imageName = imageName,
                    downloadUrl = $"https://{host}/api/LimsImage/GetSamplingSiteImage?orgId={orgId}&orgName={orgName}&orderId={orderId}&imageName={imageName}",
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
        /// GetSamplingSiteImage
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="orderName"></param>
        /// <param name="imageName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSamplingSiteImage(string orgId, string orgName,string orderId, string imageName, long e, string token)
        {
            HttpContext.Response.Headers.Add("Content-Disposition", "inline;filename=" + imageName);
            string host = Request.Host.Value;
            string baseUrl = $"https://{host}/api/LimsImage/GetSamplingSiteImage?orgId={orgId}&orgName={orgName}&orderId={orderId}&imageName={imageName}";
            baseUrl += "&e=" + e;
            string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
            long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
            if (e >= now)
            {
                if (secret == token)
                {
                    var filePath = $"{SamplingSiteImagePath}/{orgId}_{orgName}/{orderId}/{imageName}";
                    if (System.IO.File.Exists(filePath))
                    {
                        FileInfo fi = new FileInfo(filePath);
                        FileStream fs = fi.OpenRead();
                        byte[] buffer = new byte[fi.Length];
                        fs.Read(buffer, 0, Convert.ToInt32(fi.Length));
                        var resp = File(buffer, "image/jpeg");
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


        /// <summary>
        /// DeleteSamplingSiteImage
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="orderId"></param>
        /// <param name="fileName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DeleteSamplingSiteImage(string orgId, string orgName,string orderId, string fileName, long e, string token)
        {
            string host = Request.Host.Value;
            string baseUrl = $"https://{host}/api/LimsImage/DeleteSamplingSiteImage?orgId={orgId}&orgName={orgName}&orderId={orderId}&fileName={fileName}";
            baseUrl += "&e=" + e;
            string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
            long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
            if (e >= now)
            {
                if (secret == token)
                {
                    var filePath = $"{SamplingSiteImagePath}/{orgId}_{orgName}/{orderId}/{fileName}";
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, message = "File is not exists" });
                }
                return Json(new { message = "Token is not valid", success = false });
            }
            return Json(new { message = "Time is expiration", success = false });
        }
        /// <summary>
        /// UpLoadSignatureImage
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpLoadSignatureImage(IFormFile image)
        {
            string host = Request.Host.Value;
            string orgId = Request.Form["organizationId"];
            string orgName = Request.Form["organizationName"];
            string id = Request.Form["userId"];
            if (image == null || id == null || String.IsNullOrEmpty(orgId) || String.IsNullOrEmpty(orgName))
            {
                return Json(new
                {
                    success = false
                });
            }
            string orgPath = SignImagePath + "/" + orgId + "_" + orgName;
            string imageName = id + "." + image.FileName.Split('.')[1];
            string savePath = orgPath + "/" + imageName;
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(orgPath);
            }
            image.CopyTo(new FileStream(savePath, FileMode.Create));
            return Json(new
            {
                downloadUrl = "https://" + host + "/api/LimsImage/GetSignatureImage?orgId=" + orgId + "&orgName=" + orgName + "&imageName=" + imageName,
                success = true
            });
        }

        /// <summary>
        /// GetSignatureImage
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="imageName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSignatureImage(string orgId, string orgName, string imageName, long e, string token)
        {
            HttpContext.Response.Headers.Add("Content-Disposition", "inline;filename=" + imageName);
            string host = Request.Host.Value;
            string baseUrl = "https://" + host + "/api/LimsImage/GetSignatureImage?orgId=" + orgId + "&orgName=" + orgName + "&imageName=" + imageName;
            baseUrl += "&e=" + e;
            string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
            long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
            if (e >= now)
            {
                if (secret == token)
                {
                    var filePath = SignImagePath + "/" + orgId + "_" + orgName + "/" + imageName;
                    if (System.IO.File.Exists(filePath))
                    {
                        FileInfo fi = new FileInfo(filePath);
                        FileStream fs = fi.OpenRead();
                        byte[] buffer = new byte[fi.Length];
                        fs.Read(buffer, 0, Convert.ToInt32(fi.Length));
                        var resp = File(buffer, "image/jpeg");
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
