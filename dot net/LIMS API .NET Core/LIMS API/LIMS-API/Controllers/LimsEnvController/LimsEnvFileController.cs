using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models.LimsMRModels.LimsMRReportFileModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace LIMS_API.Controllers.LimsEnvController
{
    /// <summary>
    /// LimsEnvFileController
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LimsEnvFileController : Controller
    {
        private static string RunPath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath;
        private static readonly string dataSheetExtFilePath = RunPath + "/LimsEnvExternalFile/DataSheetFile";
        private static readonly string orderExtFilePath = RunPath + "/LimsEnvExternalFile/OrderExternalFile";

        /// <summary>
        /// UploadDataSheetFiels
        /// </summary>
        /// <param name="files"></param>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadDataSheetFiels(List<IFormFile> files, string orgId, string orgName)
        {
            ResponseFileModel responseFile = new ResponseFileModel();
            responseFile.error = "";
            try
            {
                if (files != null && files.Count > 0)
                {
                    string host = Request.Host.Value;

                    responseFile.fileInfos = new List<ResponseFileModel.FileInfo>();

                    StringBuilder builder = new StringBuilder();

                    var filePath = dataSheetExtFilePath;
                    var dirPath = $"{filePath}/{orgId}_{orgName}/";
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    foreach (var item in files)
                    {
                        var thisPath = $"{dirPath}/{item.FileName}";
                        if (System.IO.File.Exists(thisPath))
                        {
                            LogHelper.Warn($"{item.FileName} is exists");
                            builder.Append($"{item.FileName} is exists");
                            continue;
                        }

                        using (var stream = new FileStream(thisPath, FileMode.Create))
                        {
                            ResponseFileModel.FileInfo fileInfo = new ResponseFileModel.FileInfo();
                            fileInfo.fileName = item.FileName;
                            fileInfo.downLoadUrl =
                                $"https://{host}/api/LimsEnvFile/GetDataSheetFile?orgId={orgId}&orgName={orgName}&fileName={item.FileName}";
                            await item.CopyToAsync(stream);
                            responseFile.fileInfos.Add(fileInfo);
                        }
                    }

                    responseFile.isSuccess = true;
                    responseFile.message = builder.ToString();
                }
                else
                {
                    responseFile.isSuccess = false;
                    responseFile.message = "The uploaded file is empty";
                }

            }
            catch (Exception e)
            {
                responseFile.isSuccess = false;
                responseFile.error = e.Message;
            }
            string jsonStr = JsonConvert.SerializeObject(responseFile);
            return new ContentResult { Content = jsonStr, ContentType = "application/json" };

        }

        /// <summary>
        /// DeleteDataSheetFile
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="fileName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DeleteDataSheetFile(string orgId, string orgName, string fileName, long e, string token)
        {
            string host = Request.Host.Value;
            string baseUrl = $"https://{host}/api/LimsEnvFile/DeleteDataSheetFile?orgId={orgId}&orgName={orgName}&fileName={fileName}";
            baseUrl += "&e=" + e;
            string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
            long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
            if (e >= now)
            {
                if (secret == token)
                {
                    var filePath = $"{dataSheetExtFilePath}/{orgId}_{orgName}/{fileName}";
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
        /// GetDataSheetFile
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="fileName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetDataSheetFile(string orgId, string orgName, string fileName, long e, string token)
        {
            string host = Request.Host.Value;
            string baseUrl = $"https://{host}/api/LimsEnvFile/GetDataSheetFile?orgId={orgId}&orgName={orgName}&fileName={fileName}";
            baseUrl += "&e=" + e;
            string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
            long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
            if (e >= now)
            {
                if (secret == token)
                {
                    var filePath = $"{dataSheetExtFilePath}/{orgId}_{orgName}/{fileName}";
                    if (System.IO.File.Exists(filePath))
                    {
                        return PhysicalFile(filePath, "application/octet-stream", fileName);
                    }
                    return Json(new { success = false });
                }
                return Json(new { message = "Token is not valid", success = false });
            }
            return Json(new { message = "Time is expiration", success = false });
        }

        /// <summary>
        /// UploadOrderFiels
        /// </summary>
        /// <param name="files"></param>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadOrderFiels(List<IFormFile> files, string orgId, string orgName)
        {
            ResponseFileModel responseFile = new ResponseFileModel();
            responseFile.error = "";
            try
            {
                if (files != null && files.Count > 0)
                {
                    string host = Request.Host.Value;

                    responseFile.fileInfos = new List<ResponseFileModel.FileInfo>();

                    StringBuilder builder = new StringBuilder();

                    var filePath = orderExtFilePath;
                    var dirPath = $"{filePath}/{orgId}_{orgName}/";
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    foreach (var item in files)
                    {
                        var thisPath = $"{dirPath}/{item.FileName}";
                        if (System.IO.File.Exists(thisPath))
                        {
                            LogHelper.Warn($"{item.FileName} is exists");
                            builder.Append($"{item.FileName} is exists");
                            continue;
                        }

                        using (var stream = new FileStream(thisPath, FileMode.Create))
                        {
                            await item.CopyToAsync(stream);
                            ResponseFileModel.FileInfo fileInfo = new ResponseFileModel.FileInfo();
                            fileInfo.fileName = item.FileName;
                            fileInfo.downLoadUrl =
                                $"https://{host}/api/LimsEnvFile/GetOrderFile?orgId={orgId}&orgName={orgName}&fileName={item.FileName}";
                            responseFile.fileInfos.Add(fileInfo);

                        }
                    }

                    responseFile.isSuccess = true;
                    responseFile.message = builder.ToString();
                }
                else
                {
                    responseFile.isSuccess = false;
                    responseFile.message = "The uploaded file is empty";
                }

            }
            catch (Exception e)
            {
                responseFile.isSuccess = false;
                responseFile.error = e.Message;
            }
            string jsonStr = JsonConvert.SerializeObject(responseFile);
            return new ContentResult { Content = jsonStr, ContentType = "application/json" };

        }

        /// <summary>
        /// DeleteOrderFile
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="fileName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DeleteOrderFile(string orgId, string orgName, string fileName, long e, string token)
        {
            string host = Request.Host.Value;
            string baseUrl = $"https://{host}/api/LimsEnvFile/DeleteOrderFile?orgId={orgId}&orgName={orgName}&fileName={fileName}";
            baseUrl += "&e=" + e;
            string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
            long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
            if (e >= now)
            {
                if (secret == token)
                {
                    var filePath = $"{orderExtFilePath}/{orgId}_{orgName}/{fileName}";
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
        /// GetOrderFile
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="fileName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetOrderFile(string orgId, string orgName, string fileName, long e, string token)
        {
            string host = Request.Host.Value;
            string baseUrl = $"https://{host}/api/LimsEnvFile/GetOrderFile?orgId={orgId}&orgName={orgName}&fileName={fileName}";
            baseUrl += "&e=" + e;
            string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
            long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
            if (e >= now)
            {
                if (secret == token)
                {
                    var filePath = $"{orderExtFilePath}/{orgId}_{orgName}/{fileName}";
                    if (System.IO.File.Exists(filePath))
                    {
                        return PhysicalFile(filePath, "application/octet-stream", fileName);
                    }
                    return Json(new { success = false });
                }
                return Json(new { message = "Token is not valid", success = false });
            }
            return Json(new { message = "Time is expiration", success = false });
        }
    }
}
