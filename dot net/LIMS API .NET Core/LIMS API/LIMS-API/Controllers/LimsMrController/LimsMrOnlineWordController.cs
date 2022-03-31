using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Saving;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models.LimsMrModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Controllers.LimsMrController
{
    /// <summary>
    /// LimsMrOnlineWordController
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LimsMrOnlineWordController : Controller
    {
        string _uploadPath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath +
                            "/LimsMrExternalFile/";

        /// <summary>
        /// SaveOnlineWord
        /// </summary>
        [HttpPost]
        public IActionResult SaveOnlineWord()
        {
            try
            {
                string editor = Request.Form["editor"];
                string orgId = Request.Form["organizationId"];
                string orgName = Request.Form["organizationName"];
                string savePath = $"{_uploadPath}WIFile/{orgId}_{orgName}";
                string host = Request.Host.Value;
                // Create a unique file name
                string fileName = Guid.NewGuid() + ".docx";
                // Convert HTML text to byte array
                byte[] byteArray = Encoding.UTF8.GetBytes(editor.Contains("<html>") ? editor : "<html>" + editor + "</html>");
                // Generate Word document from the HTML
                MemoryStream stream = new MemoryStream(byteArray);
                Document Document = new Document(stream);
                // Create memory stream for the Word file
                Document.Save($"{savePath}/{fileName}", SaveFormat.Docx);
                return Json(new { success = true, fileName = fileName, downloadUrl = $"https://{host}/api/LimsMrOnlineWord/DownLoadFile?orgId={orgId}&orgName={orgName}&fileName={fileName}" });
            }
            catch (Exception e)
            {
                return Json(new {success = false, error = e.Message});
            }

        }

        /// <summary>
        /// DownLoadFile
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="fileName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DownLoadFile(string orgId, string orgName, string fileName, long e, string token)
        {
            string host = Request.Host.Value;
            string baseUrl = $"https://{host}/api/LimsMrOnlineWord/DownLoadFile?orgId={orgId}&orgName={orgName}&fileName={fileName}";
            baseUrl += "&e=" + e;
            string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
            long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
            if (e >= now)
            {
                if (secret == token)
                {
                    var filePath = $"{_uploadPath}WIFile/{orgId}_{orgName}/{fileName}";
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
        /// GetOnlineWordInHtml
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetOnlineWordInHtml(string orgId,string orgName,string fileName)
        {
            string result = "";
            string savePath = $"{_uploadPath}WIFile/{orgId}_{orgName}/{fileName}";
            if (System.IO.File.Exists(savePath))
            {
                Document doc = new Document(savePath);
                var outStream = new MemoryStream();
                // Set HTML options
                HtmlSaveOptions opt = new HtmlSaveOptions();
                opt.ExportImagesAsBase64 = true;
                opt.ExportFontsAsBase64 = true;
                // Convert Word document to HTML
                doc.Save(outStream, opt);
                // Read text from stream
                outStream.Position = 0;
                using (StreamReader reader = new StreamReader(outStream))
                {
                    result = reader.ReadToEnd();
                }
                return Json(new { success = true, content = result });
            }
            else
            {
                return Json(new {success = false, message = "file is not exist"});
            }
        }

        /// <summary>
        /// UpadteOnlineWord
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="content"></param>
        [HttpPost]
        public IActionResult UpdateOnlineWord(string orgId,string orgName, Dictionary<string, string> content)
        {
            try
            {

                foreach (string fileName in content.Keys)
                {
                    string filePath = $"{_uploadPath}WIFile/{orgId}_{orgName}/{fileName}";
                    if (System.IO.File.Exists(filePath))
                    {
                        string editor;
                        content.TryGetValue(fileName, out editor);
                        byte[] byteArray = Encoding.UTF8.GetBytes(editor.Contains("<html>") ? editor : "<html>" + editor + "</html>");
                        // Generate Word document from the HTML
                        MemoryStream stream = new MemoryStream(byteArray);
                        Document Document = new Document(stream);
                        // Create memory stream for the Word file
                        Document.Save(filePath, SaveFormat.Docx);
                    }
                }
                return Json(new { success = true});
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message });
            }
        }

        /// <summary>
        /// DeleteFile
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <param name="fileName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DeleteFile(string orgId, string orgName, string fileName, long e, string token)
        {
            string host = Request.Host.Value;
            string baseUrl = $"https://{host}/api/LimsMrOnlineWord/DeleteFile?orgId={orgId}&orgName={orgName}&fileName={fileName}";
            baseUrl += "&e=" + e;
            string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
            long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
            if (e >= now)
            {
                if (secret == token)
                {
                    var filePath = $"{_uploadPath}WIFile/{orgId}_{orgName}/{fileName}";
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, exists = System.IO.File.Exists(filePath), filePath = filePath, message = "File is not exists" });
                }
                return Json(new { message = "Token is not valid", success = false });
            }
            return Json(new { message = "Time is expiration", success = false });
        }

        [HttpPost]
        public FileResult SaveAndDownLoad()
        {
            try
            {
                string editor = Request.Form["editor"];
                // Create a unique file name
                string fileName = Guid.NewGuid() + ".docx";
                // Convert HTML text to byte array
                byte[] byteArray = Encoding.UTF8.GetBytes(editor.Contains("<html>") ? editor : "<html>" + editor + "</html>");
                // Generate Word document from the HTML
                MemoryStream stream = new MemoryStream(byteArray);
                Document Document = new Document(stream);
                // Create memory stream for the Word file
                var outputStream = new MemoryStream();
                Document.Save(outputStream, SaveFormat.Docx);
                outputStream.Position = 0;
                //Return generated Word file
                //return PhysicalFile(outputStream, "application/octet-stream", fileName);
                return File(outputStream, System.Net.Mime.MediaTypeNames.Application.Rtf, fileName);
            }
            catch (Exception e)
            {
                return null;
            }
        }


        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            string orgId = Request.Form["organizationId"];
            string orgName = Request.Form["organizationName"];
            string result = "";
            // Set file path
            string savePath = $"{_uploadPath}WIFile/{orgId}_{orgName}";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            var path = Path.Combine(savePath, file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            // Load Word document
            Document doc = new Document(path);
            var outStream = new MemoryStream();
            // Set HTML options
            HtmlSaveOptions opt = new HtmlSaveOptions();
            opt.ExportImagesAsBase64 = true;
            opt.ExportFontsAsBase64 = true;
            // Convert Word document to HTML
            doc.Save(outStream, opt);
            // Read text from stream
            outStream.Position = 0;
            using (StreamReader reader = new StreamReader(outStream))
            {
                result = reader.ReadToEnd();
            }
            return Json(new { success = true, content = result });
        }
    }
}
