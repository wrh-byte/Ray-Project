using System;
using System.Collections.Generic;
using Aspose.Words;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Blls.ReportBlls.ReportCommonBlls;
using LIMS_API.Models;
using LIMS_API.Models.ReportModels.DeliverDetectionReportModels;
using LIMS_API.Models.ReportModels.FinalReportModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Bll.ReportBll.DeliverDetectionReport
{
    public class DeliverDetectionReportBll
    {
        //private static string RunPath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath;
        //private static string SignImagePath = RunPath + "/SignImage";
        //file Path
        //private static string tempPath = RunPath + "/Template/ReportTemplate/";
        //private static string deliverReportTempPath = tempPath + "";

        private static Dictionary<int, string> testResultDic;


        public static string CreateDeliverDetectionReport(DeliverDetectionReportModel ddModel)
        {
            try
            {
                string deliverReportPath = "ReportTemplate/DeliverDetectionReportTemplate/";

                PathManagementBll pathManagement = new PathManagementBll(ddModel.organizationName, ddModel.organizationId);
                string deliverReportTempPath = pathManagement.GetTemplatePath(deliverReportPath +"DeliverDetectionReportTemplate.doc");

                testResultDic = new Dictionary<int, string>
                {
                    {1,pathManagement.GetTemplatePath(deliverReportPath+"TestResultTemplate/TestResult1.doc") },
                    {2,pathManagement.GetTemplatePath(deliverReportPath+"TestResultTemplate/TestResult2.doc") },
                    {3,pathManagement.GetTemplatePath(deliverReportPath+"TestResultTemplate/TestResult3.doc") },
                    {4,pathManagement.GetTemplatePath(deliverReportPath+"TestResultTemplate/TestResult4.doc") }

                };

                //Get Template FinalReportTemplate.doc
                Document doc = new Document(deliverReportTempPath);


                //Add generate Date
                ddModel.generateDate = DateTime.Now.ToString("yyyy年MM月dd日");

                //create sign
                //writer
                ReportImageBll reportImageBll = new ReportImageBll();
                //create writer sign image
                reportImageBll.InsertImgToDocument(ddModel, doc, "writerSignImage", ReportImageBll.ImageType.SignImage,null, ddModel.editor, 70, 20);
                //create reviewer sign image
                reportImageBll.InsertImgToDocument(ddModel, doc, "reviewerSignImage", ReportImageBll.ImageType.SignImage,null, ddModel.reviewer, 70, 20);

                //2.Report Engine
                ReportingEngine engine = new ReportingEngine();
                engine.BuildReport(doc, ddModel, "r");




                //3.Create Result Table
                CreateTestAndSamplingResultTable(ddModel, engine, doc);

                //4.Detection method and instrument Table(区分有分包备注和无分包备注时的模板不同)
                TestMethodInfoBll tbll = new TestMethodInfoBll();
                Document testMethodInfoDoc = tbll.BuildTestMethodInfo(ddModel);
                CommonBll.AppendDocument(doc, testMethodInfoDoc, ImportFormatMode.KeepSourceFormatting);


                //Add Remark At DocumentEnd
                DocumentBuilder endRemarkbuilder = new DocumentBuilder(doc);
                endRemarkbuilder.MoveToDocumentEnd();
                endRemarkbuilder.Writeln();


                //Add Report Remark
                endRemarkbuilder.Writeln(ddModel.reportRemark == null ? "" : ddModel.reportRemark);
                Paragraph p = endRemarkbuilder.InsertParagraph();
                p.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                p.AppendChild(new Run(doc, "***报告结束***"));


                //ReplaceTheUnit
                FormulaHelper formulaHelper = new FormulaHelper(doc);
                formulaHelper.ReplaceTheFormula();

                //Add water Mark
                if (ddModel.watermark)
                {
                    CommonBll.CreateWatermarkIntoDocument(doc,"草稿报告");
                }

                //Save the Final Report
                string fileType = "Doc";
                string savePath = CommonBll.CreateDraftReportSavePath(ddModel.reportName, fileType, ddModel.organizationName, ddModel.organizationId);
                //set the font is Simsun
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
        /// 创建测试结果表格
        /// </summary>
        /// <param name="frModel"></param>
        public static void CreateTestAndSamplingResultTable(DeliverDetectionReportModel ddModel, ReportingEngine engine, Document doc)
        {
            List<ResultBundle> resultBundles = ddModel.resultBundleList;
            if (resultBundles != null && resultBundles.Count > 0)
            {
                foreach (var resultBundle in resultBundles)
                {
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    //创建检测表格
                    ResultTable testResultTable = resultBundle.testResultTable;
                    if (testResultTable.resultRows != null && testResultTable.resultRows.Count > 0)
                    {
                        builder.MoveToDocumentEnd();
                        builder.Writeln(testResultTable.matrixName + "检测结果:");
                        builder.Writeln("检测样品描述:" + testResultTable.sampleRemark);

                        List<FResultRow> fResultRows = testResultTable.resultRows;
                        //检测无频次模板
                        AnalysisResultTableBll.SplitAndBuildTables(testResultDic, 4, fResultRows, true, engine, doc);
                    }
                }
            }
        }

    }
}
