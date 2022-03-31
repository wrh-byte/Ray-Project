using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aspose.Cells;
using Aspose.Words;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using LIMS_API.Bll;
using LIMS_API.Bll.LimsEnvBlls;
using LIMS_API.Bll.ReportBll.DeliverDetectionReport;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Blls.LimsEnvBlls;
using LIMS_API.Blls.ReportBlls.ReportCommonBlls;
using LIMS_API.Models;
using LIMS_API.Models.LimsEnvModels;
using LIMS_API.Models.OriginalRecordModels;
using LIMS_API.Models.ReportModels.DeliverDetectionReportModels;
using LIMS_API.Models.ReportModels.FinalReportModels;
using LIMS_API.Models.ReportModels.SiteSureyReportModels;
using limsapi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Controllers.LimsEnvController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LimsController : Controller
    {
        private static string RunPath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath;
        private static string DraftReportsPath = RunPath + "/DraftReports";
        private static string FinalReportsPath = RunPath + "/FinalReports";
        private static string ResponseFilePath = RunPath + "/ResponseFile";
        

        /// <summary>
        /// GetDownloadFile(ResponseFile)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="e"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDownloadFile(string folderName, string fileName, string organizationId,string organizationName, long e,string token)
        {
            try
            {

                //String requestUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}?folderName={folderName}&fileName={fileName}&organizationId={organizationId}&organizationName={organizationName}&e={e}";
                //HttpContext.Response.Headers.Add("Content-Disposition", "inline;filename=" + fileName);
                string host = Request.Host.Value;
                string baseUrl = "https://"+host+"/api/Lims/GetDownloadFile?folderName="+folderName+"&fileName="+fileName+"&organizationId="+organizationId+"&organizationName="+organizationName;
                baseUrl += "&e=" + e;
                string secret = CommonBll.HMACSHA1Text(ServiceProvider.Token, baseUrl);
                TimeSpan timeSpan = DateTime.Now - DateTime.Parse("1970-01-01");
                long now = Convert.ToInt64(timeSpan.TotalMilliseconds);
                if (e >= now)
                {
                    if (secret == token)
                    {
                        string filePath = "";
                        if (folderName== "ResponseFile")
                        {
                            filePath = ResponseFilePath + "/" + organizationName + "_" + organizationId + "/" + fileName;
                        }
                        else if(folderName== "DraftReports")
                        {
                            filePath = DraftReportsPath + "/" + organizationName + "_" + organizationId + "/" + fileName;
                        }
                        else if (folderName == "FinalReports")
                        {
                            filePath = FinalReportsPath + "/" + organizationName + "_" + organizationId + "/" + fileName;
                        }
                        //File Exist
                        bool exists = System.IO.File.Exists(filePath);
                        if (exists)
                        {
                            return PhysicalFile(filePath, "application/word", fileName);
                        }
                        else
                        {
                            ViewBag.name = fileName;
                            return View("View/NoFilePage.cshtml");
                        }
                    }
                    else
                    {
                        return Json(new { message = "Token is not valid", success = false });
                    }
                }
                else
                {
                    return Json(new { message = "Time is expiration" ,success = false});
                }

            }
            catch (Exception exception)
            {
                LogHelper.Error("Error Message:" + exception.Message + " " + "StackTrace" + exception.StackTrace);
                return Json(new { success = false, error = exception.Message });
            }

        }


        /// <summary>
        /// SamplingWorkOrder Create to PDF
        /// </summary>
        /// <param name="swoModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SamplingWorkOrder(SamplingWordOrderModel swoModel)
        {
            try
            {

                string host = Request.Host.Value;
                string savePath = SamplingWorkOrderBll.CreateSamplingWorkOrderTemplate(swoModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=ResponseFile&fileName=" + fileName + "&organizationId=" + swoModel.organizationId + "&organizationName=" + swoModel.organizationName,
                    success =true,
                });

            }
            catch (Exception exception)
            {
                LogHelper.Error("Error Message:" + exception.Message + " " + "StackTrace" + exception.StackTrace);
                return Json(new {success=false, error = exception.Message });
            }
        }


        /// <summary>
        /// Quotation
        /// </summary>
        /// <param name="quotationModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Quotation(QuotationModel quotationModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = QuotationBll.createQuotation(quotationModel);
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
        /// LaboratoryJob create to pdf
        /// </summary>
        /// <param name="labModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult LaboratoryJob(LaboratoryJobModel labModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = LaboratoryJobBll.CreateLaboratoryJobTemplate(labModel);
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
        /// TestResult(Out of date)
        /// </summary>
        /// <param name="trModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TestResult(TestResultModel trModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = TestResultBll.CreateTestResultTemplate(trModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=ResponseFile&fileName=" + fileName + "&organizationId=" + trModel.organizationId + "&organizationName=" + trModel.organizationName,
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
        /// FinalReport 生成草稿报告普通报告
        /// </summary>
        /// <param name="frModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateDraftReport(FinalReportModel frModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = FinalReportBll.CreateFinalReportTemplate(frModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    fileName = fileName,
                    fileType = fileName.Split('.')[1],
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=DraftReports&fileName=" + fileName + "&organizationId=" + frModel.organizationId + "&organizationName=" + frModel.organizationName,
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
        /// UpLoadDraftReport
        /// </summary>
        /// <param name="file"></param>
        /// <param name="orderReportName"></param>
        /// <param name="organizationId"></param>
        /// <param name="organizationName"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpLoadDraftReport(IFormFile file,string orderReportName,string organizationId,string organizationName)
        {
            try
            {
                string host = Request.Host.Value;
                if (file==null)
                {
                    return BadRequest();
                }
                if (!Directory.Exists(DraftReportsPath))
                {
                    Directory.CreateDirectory(DraftReportsPath);
                }

                //Rename The File
                string now = DateTime.Now.ToString("yyyyMMddhhmmss");
                string fileName = orderReportName + "_" + now + ".";
                string type = "";
                //docx
                if (file.ContentType== "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                {
                    type = "docx";
                }
                //doc
                else if(file.ContentType== "application/msword")
                {
                    type = "doc";
                }
                else
                {
                    type = file.FileName.Split('.')[1];
                }

                if (!string.IsNullOrEmpty(type))
                {
                    fileName += type;
                    String savePath = DraftReportsPath + "/"+organizationName + "_" + organizationId +"/"+ fileName;
                    file.CopyTo(new FileStream(savePath, FileMode.Create));

                    return Json(new
                    {
                        fileName = fileName,
                        fileType = type,
                        downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=DraftReports&fileName=" + fileName + "&organizationId=" + organizationId + "&organizationName=" + organizationName,
                        success = true,
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Unable to get file type",
                    });
                }

            }
            catch (Exception exception)
            {
                LogHelper.Error("Error Message:" + exception.Message + " " + "StackTrace" + exception.StackTrace);
                return Json(new { success = false, error = exception.Message });
            }
        }

        /// <summary>
        /// CreateFinalReport 生成最终报告pdf
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateFinalReport(FinalReportRequest request)
        {
            try
            {
                string host = Request.Host.Value;
                string fileName = request.fileName;
                string pdfFileName = "";
                string extendName = "";
                if (!string.IsNullOrEmpty(fileName))
                {
                    pdfFileName = fileName.Split('.')[0] + ".pdf";
                    extendName = fileName.Split('.')[1];
                }

                string draftReportPath = $"{DraftReportsPath}/{request.organizationName}_{request.organizationId}";
                string finalReportPath = $"{FinalReportsPath}/{request.organizationName}_{request.organizationId}";
                string docFilePath = $"{draftReportPath}/{fileName}";
                string pdfFilePath = $"{finalReportPath}/{pdfFileName}";
                if (Directory.Exists(finalReportPath) == false)
                {
                    Directory.CreateDirectory(finalReportPath);
                }

                if (extendName.ToLower() == "xlsx")
                {
                    //Excel
                    Workbook wb = new Workbook(docFilePath);
                    wb.Save(pdfFilePath, Aspose.Cells.SaveFormat.Pdf);
                }
                else if (extendName.ToLower() == "doc")
                {
                    //GET Word
                    Document doc = new Document(docFilePath);
                    //Insert approver 报告批准人签名图片
                    ReportImageBll reportImageBll = new ReportImageBll();
                    reportImageBll.InsertImgToDocument(request,doc, "approverSignImage",ReportImageBll.ImageType.SignImage,null,request.approver,80,30);

                    ReportingEngine engine = new ReportingEngine();
                    engine.BuildReport(doc, request, "f");

                    FontSettings fontSettings = new FontSettings();
                    fontSettings.SubstitutionSettings.TableSubstitution.SetSubstitutes("SimSun");
                    doc.FontSettings = fontSettings;
                    doc.Save(pdfFilePath, Aspose.Words.SaveFormat.Pdf);
                }

                return Json(new
                {
                    fileName = pdfFileName,
                    fileType = pdfFileName.Split('.')[1],
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=FinalReports&fileName=" + pdfFileName + "&organizationId=" + request.organizationId + "&organizationName=" + request.organizationName,
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
        /// CreateOriginalRecord
        /// </summary>
        /// <param name="orModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateOriginalRecord(OriginalRecordModel orModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = OriginalRecordBll.CreateOriginalRecord(orModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    fileName = fileName,
                    fileType = fileName.Split('.')[1],
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=ResponseFile&fileName=" + fileName + "&organizationId=" + orModel.organizationId + "&organizationName=" + orModel.organizationName,
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
        /// CreateSiteSurveyReport(Excel)生成场调报告Excel草稿
        /// </summary>
        /// <param name="srModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateSiteSurveyReport(SiteSurveyReportModel srModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = SiteSurveyReportBll.CreateSiteSurveyReport(srModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    fileName = fileName,
                    fileType = fileName.Split('.')[1],
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=DraftReports&fileName=" + fileName + "&organizationId=" + srModel.organizationId + "&organizationName=" + srModel.organizationName,
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
        /// CreateSiteSurveyReportWord(Word)生成场调报告Word草稿
        /// </summary>
        /// <param name="srModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateSiteSurveyReportWord(SiteSurveyReportModel srModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = SiteSurveyReportWordBll.CreateSiteSurveyReport(srModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    fileName = fileName,
                    fileType = fileName.Split('.')[1],
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=DraftReports&fileName=" + fileName + "&organizationId=" + srModel.organizationId + "&organizationName=" + srModel.organizationName,
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
        /// 生成送检报告草稿
        /// </summary>
        /// <param name="frModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateDeliverDetectionReport(DeliverDetectionReportModel ddModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = DeliverDetectionReportBll.CreateDeliverDetectionReport(ddModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    fileName = fileName,
                    fileType = fileName.Split('.')[1],
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=DraftReports&fileName=" + fileName + "&organizationId=" + ddModel.organizationId + "&organizationName=" + ddModel.organizationName,
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
        /// ContractReview
        /// </summary>
        /// <param name="crModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ContractReview(ContractReviewModel crModel)
        {
            try
            {
                string host = Request.Host.Value;
                string savePath = ContractReviewBll.createContractReview(crModel);
                string fileName = Path.GetFileName(savePath);
                return Json(new
                {
                    downloadUrl = "https://" + host + "/api/Lims/GetDownloadFile?folderName=ResponseFile&fileName=" + fileName + "&organizationId=" + crModel.organizationId + "&organizationName=" + crModel.organizationName,
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