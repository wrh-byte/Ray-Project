using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Blls.ReportBlls.ReportCommonBlls;
using LIMS_API.Models.LimsMrModels.MrReportModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Drawing;
using System.IO;
using Aspose.BarCode;
using Aspose.Words.Drawing;
using Bitmap = System.Drawing.Bitmap;
using GraphicsUnit = System.Drawing.GraphicsUnit;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace LIMS_API.Blls.LimsMrBll.MrReportBlls
{
    /// <summary>
    /// MrReportBll
    /// </summary>
    public class MrReportBll
    {
        private static Dictionary<int, string> _testDataDic;

        private static string _sampleImagePath =
            ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath +
            "/LimsMrExternalFile/MrSampleImageFile/";

        /// <summary>
        /// CreateCommonReport
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>
        public static string CreateCommonReport(MrReportModel reportModel)
        {
            try
            {
                string reportPath = "ReportTemplates/CommonReportTemp/";
                PathManagementBll pathManagement = new PathManagementBll(reportModel.organizationName, reportModel.organizationId, "LIMS-MR-TEMP");
                //1Template Mapper

                string reportTempPath = pathManagement.GetTemplatePath(reportPath + "ReportTemp.doc");
                Document doc = new Document(reportTempPath);

                _testDataDic = new Dictionary<int, string>
                {
                    {1,pathManagement.GetTemplatePath(reportPath + "TestTableTemps/ResultDataTemplate/ResultTableTemplate1.doc")},
                    {2,pathManagement.GetTemplatePath(reportPath + "TestTableTemps/ResultDataTemplate/ResultTableTemplate2.doc")},
                    {3,pathManagement.GetTemplatePath(reportPath + "TestTableTemps/ResultDataTemplate/ResultTableTemplate3.doc")},
                    {4,pathManagement.GetTemplatePath(reportPath + "TestTableTemps/ResultDataTemplate/ResultTableTemplate4.doc")}
                };
                //2Build Main Page
                ReportingEngine engine = new ReportingEngine();
                engine.BuildReport(doc, reportModel, "r");

                //3Build Sample Page
                string sampleTempPath = pathManagement.GetTemplatePath(reportPath + "TestTableTemps/TestSampleTemplate.doc");
                Document sampleDoc = new Document(sampleTempPath);
                engine.BuildReport(sampleDoc, reportModel, "r");
                //4Insert Sample Image
                ReportImageBll reportImageBll = new ReportImageBll();

                if (reportModel.sampleImageInfoList != null)
                {
                    foreach (var imageName in reportModel.sampleImageInfoList)
                    {
                        string imagePath =
                            _sampleImagePath + $"{reportModel.organizationId}_{reportModel.organizationName}/{imageName}";
                        InsertImgToDocument(sampleDoc, "SampleImage", imagePath, 400, 300);
                    }
                }
                //5add sample Page to main page
                CommonBll.AppendDocument(doc, sampleDoc, ImportFormatMode.KeepSourceFormatting);

                //6Add result page
                string testPageTempPath = pathManagement.GetTemplatePath(reportPath + "TestTableTemps/TestDataTemplate.doc");
                if (reportModel.resultBundleList != null && reportModel.resultBundleList.Count > 0)
                {
                    foreach (var resultBundle in reportModel.resultBundleList)
                    {
                        Document testPageDoc = new Document(testPageTempPath);
                        object[] dataSources = { reportModel, resultBundle };
                        string[] dataSourceNames = { "r", "d" };
                        engine.BuildReport(testPageDoc, dataSources, dataSourceNames);
                        AnalysisResultTableBll.SplitAndBuildTables(_testDataDic, 4, resultBundle.resultTable.resultRows, false, engine, testPageDoc);
                        CommonBll.AppendDocument(doc, testPageDoc, ImportFormatMode.KeepSourceFormatting);
                        //插入样品测试图片
                        string testImageTempPath =
                            pathManagement.GetTemplatePath(reportPath + "TestTableTemps/TestSampleImageTemplate.doc");
                        Document testImageDoc = new Document(testImageTempPath);
                        engine.BuildReport(testImageDoc, reportModel, "r");
                        if (resultBundle.testImageNameList != null)
                        {
                            foreach (var imageName in resultBundle.testImageNameList)
                            {
                                string imagePath =
                                    _sampleImagePath + $"{reportModel.organizationId}_{reportModel.organizationName}/{imageName}";
                                InsertImgToDocument(testImageDoc, "TestImage", imagePath, 400, 300);
                            }
                        }
                        CommonBll.AppendDocument(doc, testImageDoc, ImportFormatMode.KeepSourceFormatting);
                    }
                }

                //Add ** Final ** Test
                string endPageTempPath = pathManagement.GetTemplatePath(reportPath + "TestTableTemps/ReportEndTemplate.doc");
                Document endDoc = new Document(endPageTempPath);
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.MoveToDocumentEnd();
                builder.Writeln("");
                builder.InsertDocument(endDoc, ImportFormatMode.KeepSourceFormatting);

                //Add Added Riding Seal
                int pageCount = doc.PageCount;
                Image[] images = SplitImage(pageCount, pathManagement.GetTemplatePath(reportPath + "检验检测专用章白底.png"));
                DocumentBuilder imageBuilder = new DocumentBuilder(doc);

                for (int i = 0; i < doc.Sections.Count; i++)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    images[i].Save(memoryStream, ImageFormat.Png);
                    Shape shape = new Shape(doc, ShapeType.Image);
                    shape.ImageData.SetImage(memoryStream);
                    shape.Left = 300;
                    shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
                    shape.RelativeVerticalPosition = RelativeVerticalPosition.Page;
                    shape.HorizontalAlignment = HorizontalAlignment.Right;
                    shape.VerticalAlignment = VerticalAlignment.Center;

                    if (i!=0)
                    {
                        builder.MoveToSection(i);
                        builder.InsertNode(shape);
                    }
                }
                //Save Document
                string fileType = "Doc";
                string savePath = CommonBll.CreateDraftReportSavePath(reportModel.reportCode, fileType,
                    reportModel.organizationName, reportModel.organizationId);
                //.set the font is Simsun
                FontSettings fontSettings = new FontSettings();
                fontSettings.SubstitutionSettings.TableSubstitution.SetSubstitutes("SimSun");
                doc.FontSettings = fontSettings;
                doc.Save(savePath, CommonBll.GetSaveFormat(fileType));
                return savePath;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        /// <summary>
        /// SplitImage
        /// </summary>
        /// <param name="num"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static Image[] SplitImage(int num,string imagePath)
        {
            List<Bitmap> lists = new List<Bitmap>();
            Image image = Image.FromFile(imagePath);
            int w = (image.Width/2) / (num-1);
            int halfWidth = image.Width / 2;
            Bitmap bitmap = null;
            //first 50% 
            for (int i = 0; i < num; i++)
            {
                int bitMapWidth = 0;
                if (i==0)
                {
                    bitMapWidth = halfWidth;
                }
                else
                {
                    bitMapWidth = w;
                }
                bitmap = new Bitmap(bitMapWidth, image.Height);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);
                    int x = 0;
                    int rectWidth = 0;
                    int angleWidth = 0;
                    if (i==0)
                    {
                        x = 0;
                        rectWidth = halfWidth;
                        angleWidth = halfWidth;
                    }
                    else
                    {
                        x = halfWidth + (i * w);
                        rectWidth = w;
                        angleWidth = bitmap.Width;
                    }
                    Rectangle rect = new Rectangle(x, 0, rectWidth, image.Height);
                    g.DrawImage(image, new Rectangle(0, 0, angleWidth, bitmap.Height), rect, GraphicsUnit.Pixel);

                }
                lists.Add(bitmap);
            }
            return lists.ToArray();
        }

        /// <summary>
        /// InsertImgToDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="bookMarkName"></param>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void InsertImgToDocument(Document doc, string bookMarkName, string path, double width, double height)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    DocumentBuilder imageBuilder = new DocumentBuilder(doc);
                    imageBuilder.MoveToBookmark(bookMarkName);
                    imageBuilder.InsertImage(path, width, height);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
