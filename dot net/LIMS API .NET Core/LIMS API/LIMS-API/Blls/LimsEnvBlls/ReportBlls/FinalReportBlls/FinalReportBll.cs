using System;
using System.Collections.Generic;
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Blls.ReportBlls.ReportCommonBlls;
using LIMS_API.Models;
using LIMS_API.Models.ReportModels.FinalReportModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Bll
{
    /// <summary>
    /// Create FinalReportBll
    /// </summary>
    public class FinalReportBll
    {

        //Template Mapper
        //噪声测试模板
        private static Dictionary<int,string> _noiseTestDic;
        
        //噪声采样模板
        private static Dictionary<int,string> _noiseSamplingDic;
        
        //有组织废气测模板
        private static Dictionary<int,string> _orgGasTestDic;

        //有组织废气采样模板
        private static Dictionary<int,string> _orgGasSamplingDic;

        //油烟测试模板
        private static Dictionary<int, string> _lampblackTestDic;

        //油烟采样模板
        private static Dictionary<int, string> _lampblackSamplingDic;

        //有频次测试模板
        private static Dictionary<int,string> _resultWithFrequencyDic;

        //无频次测试模板
        private static Dictionary<int,string> _resultWithOutFrequencyDic;

        //带点位采样模板
        private static Dictionary<int,string> _samplingWithPDic;

        //带点位频次采样模板
        private static Dictionary<int, string> _samplingWithPFDic;

        /// <summary>
        /// CreateFinalReportTemplate
        /// </summary>
        /// <param name="frModel"></param>
        /// <returns></returns>
        public static string CreateFinalReportTemplate(FinalReportModel frModel)
        {
            try
            {
                string finalReportPath = "ReportTemplate/FinalReportTemplate/";
                PathManagementBll pathManagement = new PathManagementBll(frModel.organizationName,frModel.organizationId);
                //Template Mapper

                string finalReportTempPath = pathManagement.GetTemplatePath(finalReportPath + "FinalReportTemplate.doc");

                #region GetTempPath
                //噪声测试模板
                _noiseTestDic = new Dictionary<int, string>
                {
                    {1,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/NoiseTemplate/TestTemplate/NoiseTestResult1.doc")},
                    {2,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/NoiseTemplate/TestTemplate/NoiseTestResult2.doc")},
                    {3,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/NoiseTemplate/TestTemplate/NoiseTestResult3.doc")}
                };

                //噪声采样模板
                _noiseSamplingDic = new Dictionary<int, string>
                {
                    {1,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/NoiseTemplate/SamplingTemplate/NoiseSamplingResult1.doc") },
                    {2,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/NoiseTemplate/SamplingTemplate/NoiseSamplingResult2.doc") },
                    {3,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/NoiseTemplate/SamplingTemplate/NoiseSamplingResult3.doc") },
                    {4,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/NoiseTemplate/SamplingTemplate/NoiseSamplingResult4.doc") },
                    {5,pathManagement.GetTemplatePath(finalReportPath+"/NoiseTemplate/SamplingTemplate/NoiseSamplingResult5.doc") },
                };

                //有组织废气测模板
                _orgGasTestDic = new Dictionary<int, string>
                {
                    {1,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/OrgGasTemplate/TestTemplate/OrgGasTestResult1.doc") },
                    {2,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/OrgGasTemplate/TestTemplate/OrgGasTestResult2.doc") },
                    {3,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/OrgGasTemplate/TestTemplate/OrgGasTestResult3.doc") },
                    {4,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/OrgGasTemplate/TestTemplate/OrgGasTestResult4.doc") },
                    {5,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/OrgGasTemplate/TestTemplate/OrgGasTestResult5.doc") },
                };

                //有组织废气采样模板
                _orgGasSamplingDic = new Dictionary<int, string>
                {
                    {1,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/OrgGasTemplate/SamplingTemplate/OrgGasSamplingResult1.doc") },
                    {2,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/OrgGasTemplate/SamplingTemplate/OrgGasSamplingResult2.doc") },
                    {3,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/OrgGasTemplate/SamplingTemplate/OrgGasSamplingResult3.doc") },
                    {4,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/OrgGasTemplate/SamplingTemplate/OrgGasSamplingResult4.doc") },
                    {5,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/OrgGasTemplate/SamplingTemplate/OrgGasSamplingResult5.doc") },
                };

                //油烟测试模板
                _lampblackTestDic = new Dictionary<int, string>
                {
                    {6,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/LampblackTemplate/TestTemplate/LampblackTestResult.doc") }
                };

                //油烟采样模板
                _lampblackSamplingDic = new Dictionary<int, string>
                {
                    {6,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/LampblackTemplate/SamplingTemplate/LampblackSamplingResult.doc") }
                };

                //有频次测试模板
                _resultWithFrequencyDic = new Dictionary<int, string>
                {
                    {1,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/ResultWithFrequencyTemplate/ResultWithFrequency1.doc") },
                    {2,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/ResultWithFrequencyTemplate/ResultWithFrequency2.doc") },
                    {3,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/ResultWithFrequencyTemplate/ResultWithFrequency3.doc") }
                };

                //无频次测试模板
                _resultWithOutFrequencyDic = new Dictionary<int, string>
                {
                    {1, pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/ResultWithOutFrequencyTemplate/ResultWithOutFrequency1.doc")},
                    {2, pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/ResultWithOutFrequencyTemplate/ResultWithOutFrequency2.doc")},
                    {3, pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/ResultWithOutFrequencyTemplate/ResultWithOutFrequency3.doc")},
                    {4, pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/ResultWithOutFrequencyTemplate/ResultWithOutFrequency4.doc")},
                };

                //带点位采样模板
                _samplingWithPDic = new Dictionary<int, string>
                {
                    {1,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/SamplingWithP/SamplingWithP1.doc") },
                    {2,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/SamplingWithP/SamplingWithP2.doc") },
                    {3,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/SamplingWithP/SamplingWithP3.doc") },
                    {4,pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/SamplingWithP/SamplingWithP4.doc") }
                };

                //带点位频次采样模板
                _samplingWithPFDic = new Dictionary<int, string>
                {
                    {1, pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/SamplingWithPF/SamplingWithPF1.doc")},
                    {2, pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/SamplingWithPF/SamplingWithPF2.doc")},
                    {3, pathManagement.GetTemplatePath(finalReportPath+"TestResultTemplate/SamplingWithPF/SamplingWithPF3.doc")}
                };
                #endregion

                //Get Template FinalReportTemplate.doc
                Document doc = new Document(finalReportTempPath);
                ReportingEngine engine = new ReportingEngine();


                //1.Add generate Date
                frModel.generateDate = DateTime.Now.ToString("yyyy年MM月dd日");

                #region 1.Create Image 
                ReportImageBll reportImageBll = new ReportImageBll();
                //2.create sampling image

                
                //3.create writer sign image
                reportImageBll.InsertImgToDocument(frModel, doc, "writerSignImage", ReportImageBll.ImageType.SignImage, null,frModel.editor, 80, 30);
                //4.create reviewer sign image
                reportImageBll.InsertImgToDocument(frModel, doc, "reviewerSignImage", ReportImageBll.ImageType.SignImage, null,frModel.reviewer, 80, 30);
                #endregion
                //5.Report Engine
                engine.BuildReport(doc, frModel, "r");

                //6.Insert SamplingImage
                List<string> samplingImageList = frModel.samplingImageList;
                if (samplingImageList!=null&&samplingImageList.Count>0)
                {
                    //倒装List元素
                    List<string> descendList = new List<string>();
                    for (int i = samplingImageList.Count-1; i >= 0; i--)
                    {
                        descendList.Add(samplingImageList[i]);
                    }
                    foreach (var item in descendList)
                    {
                        reportImageBll.InsertImgToDocument(frModel, doc, "samplingImage", ReportImageBll.ImageType.SamplingSiteImage, frModel.orderId, item, 400, 300);
                    }
                }
                

                //7.Create Result Table
                CreateTestAndSamplingResultTable(frModel, engine, doc);


                //8.Detection method and instrument Table(区分有分包备注和无分包备注时的模板不同)
                TestMethodInfoBll tbll = new TestMethodInfoBll();
                Document testMethodInfoDoc = tbll.BuildTestMethodInfo(frModel);
                CommonBll.AppendDocument(doc, testMethodInfoDoc, ImportFormatMode.KeepSourceFormatting);


                //9.Add Remark At DocumentEnd
                DocumentBuilder endRemarkbuilder = new DocumentBuilder(doc);
                endRemarkbuilder.MoveToDocumentEnd();
                endRemarkbuilder.Writeln();
                endRemarkbuilder.Writeln(frModel.reportRemark == null ? "" : frModel.reportRemark);
                Paragraph p = endRemarkbuilder.InsertParagraph();
                p.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                p.AppendChild(new Run(doc, "***报告结束***"));


                //10.ReplaceTheUnit
                FormulaHelper formulaHelper = new FormulaHelper(doc);
                formulaHelper.ReplaceTheFormula();

                //11.Add Water Mark
                if (frModel.watermark)
                {
                    CommonBll.CreateWatermarkIntoDocument(doc, "草稿报告");
                }
                
                //12.Save the Final Report
                string fileType = "Doc";
                string savePath = CommonBll.CreateDraftReportSavePath(frModel.reportName, fileType,
                    frModel.organizationName, frModel.organizationId);
                //13.set the font is Simsun
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
        /// 创建测试/采样结果表格(入口)
        /// </summary>
        /// <param name="frModel"></param>
        public static void CreateTestAndSamplingResultTable(FinalReportModel frModel, ReportingEngine engine, Document doc)
        {
            List<ResultBundle> resultBundles = frModel.resultBundleList;
            if (resultBundles != null && resultBundles.Count > 0)
            {
                foreach (var resultBundle in resultBundles)
                {
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    //创建检测表格
                    ResultTable testResultTableInfos = resultBundle.testResultTable;
                    if (testResultTableInfos != null && testResultTableInfos.resultRows != null && testResultTableInfos.resultRows.Count > 0)
                    {
                        List<FResultRow> testResultTable = testResultTableInfos.resultRows;
                        //有组织废气模板
                        if (testResultTableInfos.isOrgGas)
                        {
                            builder.MoveToDocumentEnd();
                            builder.Bold = true;
                            builder.Writeln("有组织废气检测结果：");
                            builder.Bold = false;
                            builder.Writeln("");
                            builder.Write(testResultTableInfos.samplingPoint.Points_Code__c + testResultTableInfos.samplingPoint.Point_Name__c + "检测结果：");
                            AnalysisResultTableBll.SplitAndBuildTables(_orgGasTestDic,5,testResultTable,true,engine,doc);
                        }
                        //噪声测试表格
                        else if (testResultTableInfos.isNoise)
                        {
                            builder.MoveToDocumentEnd();
                            builder.Writeln(testResultTableInfos.matrixName + "检测结果:");
                            AnalysisResultTableBll.SplitAndBuildTables(_noiseTestDic,3,testResultTable,true,engine,doc);
                        }
                        //油烟
                        else if (testResultTableInfos.isLampblack)
                        {
                            builder.MoveToDocumentEnd();
                            builder.Bold = true;
                            builder.Writeln("有组织废气检测结果：");
                            builder.Bold = false;
                            builder.Writeln("");
                            builder.Write("检测结果：");
                            AnalysisResultTableBll.SplitAndBuildTables(_lampblackTestDic, 6, testResultTable, true, engine, doc);
                        }
                        //其他基质测试表格
                        else
                        {
                            builder.MoveToDocumentEnd();
                            builder.Writeln(testResultTableInfos.matrixName + "检测结果:");
                            builder.Writeln("检测样品描述:" + testResultTableInfos.sampleRemark);

                            List<FResultRow> resultRows = testResultTableInfos.resultRows;
                            if (testResultTableInfos.frequency)
                            {
                                //检测有频次模板
                                //SplitTestTableWithFrequency(fResultRows, true, engine, doc);
                                AnalysisResultTableBll.SplitAndBuildTables(_resultWithFrequencyDic,3,resultRows,true,engine,doc);
                            }
                            else
                            {
                                //检测无频次模板
                                //SplitTestTable(fResultRows, true, engine, doc);
                                AnalysisResultTableBll.SplitAndBuildTables(_resultWithOutFrequencyDic,4,resultRows,true,engine,doc);
                            }
                        }

                    }

                    //创建采样结果表格 
                    ResultTable samplingTableInfos = resultBundle.samplingResultTable;
                    if (samplingTableInfos != null && samplingTableInfos.resultRows != null && samplingTableInfos.resultRows.Count > 0)
                    {
                        List<FResultRow> samplingResultTable = samplingTableInfos.resultRows;
                        if (samplingTableInfos.isOrgGas)//有组织废气模板
                        {
                            builder.MoveToDocumentEnd();
                            builder.Write(samplingTableInfos.samplingPoint.Points_Code__c + samplingTableInfos.samplingPoint.Point_Name__c + "采样信息：");
                            AnalysisResultTableBll.SplitAndBuildTables(_orgGasSamplingDic, 5, samplingResultTable, true, engine, doc);
                        }
                        else if (samplingTableInfos.isLampblack)
                        {
                            builder.MoveToDocumentEnd();
                            builder.Writeln("油烟净化器信息:");
                            AnalysisResultTableBll.SplitAndBuildTables(_lampblackSamplingDic, 7, samplingResultTable, true, engine, doc);
                        }
                        else if (samplingTableInfos.isNoise)//噪声采样表格
                        {
                            builder.MoveToDocumentEnd();
                            builder.Writeln(testResultTableInfos.matrixName + "现场信息:");
                            AnalysisResultTableBll.SplitAndBuildTables(_noiseSamplingDic, 5, samplingResultTable, true, engine, doc);
                        }
                        else
                        {
                            builder.MoveToDocumentEnd();
                            builder.Writeln(samplingTableInfos.matrixName + "采样信息");
                            if (samplingTableInfos.frequency == true && samplingTableInfos.isNotPoint == true)
                            {
                                //有频次无点位模板
                                AnalysisResultTableBll.SplitAndBuildTables(_resultWithFrequencyDic, 3, samplingResultTable, true, engine, doc);
                            }
                            else if (samplingTableInfos.frequency == true && samplingTableInfos.isNotPoint == false)
                            {
                                //有频次有点位模板
                                //SplitSamplingTableWithPointAndFre(samplingResultTable, false, engine, doc);
                                AnalysisResultTableBll.SplitAndBuildTables(_samplingWithPFDic, 3, samplingResultTable, true, engine, doc);
                            }
                            else if (samplingTableInfos.frequency == false && samplingTableInfos.isNotPoint == true)
                            {
                                //无频次无点位
                                //SplitTestTable(samplingResultTable, false, engine, doc);
                                AnalysisResultTableBll.SplitAndBuildTables(_resultWithOutFrequencyDic, 4, samplingResultTable, true, engine, doc);
                            }
                            else if (samplingTableInfos.frequency == false && samplingTableInfos.isNotPoint == false)
                            {
                                //无频次有点位
                                //SplitSamplingTableWithPoint(samplingResultTable, false, engine, doc);
                                AnalysisResultTableBll.SplitAndBuildTables(_samplingWithPDic, 4, samplingResultTable, true, engine, doc);
                            }
                        }
                    }
                }
            }
        }
    }
}
