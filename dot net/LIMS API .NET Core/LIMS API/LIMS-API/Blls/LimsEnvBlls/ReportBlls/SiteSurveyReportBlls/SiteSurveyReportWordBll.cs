using LIMS_API.Models;
using Microsoft.Extensions.DependencyInjection;
using Aspose.Words;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words.Reporting;
using Aspose.Words.Tables;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Blls.ReportBlls.ReportCommonBlls;
using LIMS_API.Models.ReportModels.SiteSureyReportModels;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Bll
{
    public class SiteSurveyReportWordBll
    {
        private static Dictionary<int, string> inorganicTempDic;
        private static Dictionary<int, string> organicTempDic;

        /// <summary>
        /// 创建场调报告
        /// </summary>
        /// <param name="srModel"></param>
        /// <returns></returns>
        public static string CreateSiteSurveyReport(SiteSurveyReportModel srModel)
        {
            string siteSurveyReportPath = "ReportTemplate/SiteSurveyTemplate/";
            PathManagementBll pathManagement = new PathManagementBll(srModel.organizationName,srModel.organizationId);

            //无机测试模板列数路径映射
            inorganicTempDic = new Dictionary<int, string>
            {
                {1,pathManagement.GetTemplatePath(siteSurveyReportPath+"TestDataTemplate/InorganicTemplate/InorganicTemplate1.doc")},
                {2,pathManagement.GetTemplatePath(siteSurveyReportPath+"TestDataTemplate/InorganicTemplate/InorganicTemplate2.doc")},
                {3,pathManagement.GetTemplatePath(siteSurveyReportPath+"TestDataTemplate/InorganicTemplate/InorganicTemplate3.doc")}
            };

            //有机测试模板列数路径映射
            organicTempDic = new Dictionary<int, string>
            {
                {1,pathManagement.GetTemplatePath(siteSurveyReportPath+"TestDataTemplate/OrganicTemplate/OrganicTemplate1.doc")},
                {2,pathManagement.GetTemplatePath(siteSurveyReportPath+"TestDataTemplate/OrganicTemplate/OrganicTemplate2.doc")},
                {3,pathManagement.GetTemplatePath(siteSurveyReportPath+"TestDataTemplate/OrganicTemplate/OrganicTemplate3.doc")}
            };


            Document testMethodDocument = new Document(pathManagement.GetTemplatePath(siteSurveyReportPath+"TestMethodTemplate/TestMethods.doc"));
            Document testMethodDocumentWithSub = new Document(pathManagement.GetTemplatePath(siteSurveyReportPath+"TestMethodTemplate/TestMethodsWithSubcontract.doc"));
            Document doc = new Document(pathManagement.GetTemplatePath(siteSurveyReportPath+"SiteSurveyTemplate.doc"));
            //Add generate Date
            srModel.generateDate = DateTime.Now.ToString("yyyy年MM月dd日");

            //writer
            ReportImageBll reportImageBll = new ReportImageBll();
            //create writer sign image
            reportImageBll.InsertImgToDocument(srModel, doc, "writerSignImage", ReportImageBll.ImageType.SignImage, null,srModel.editor, 70, 20);
            //create reviewer sign image
            reportImageBll.InsertImgToDocument(srModel, doc, "reviewerSignImage", ReportImageBll.ImageType.SignImage, null,srModel.reviewer, 70, 20);

            //1.Create Method Table
            ReportingEngine engine = new ReportingEngine();
            engine.BuildReport(doc, srModel, "s");

            //.Add test method Table（区分有分包备注和无分包备注时的模板不同）
            Document tmDoc;
            if (string.IsNullOrEmpty(srModel.subRemark))
            {
                tmDoc = testMethodDocument;
            }
            else
            {
                tmDoc = testMethodDocumentWithSub;
            }
            engine.BuildReport(tmDoc, srModel, "s");
            DocumentBuilder testMethodbuilder = new DocumentBuilder(doc);
            testMethodbuilder.MoveToDocumentEnd();
            testMethodbuilder.InsertDocument(tmDoc, ImportFormatMode.KeepSourceFormatting);
            //CommonBll.AppendDocument(doc, tmDoc, ImportFormatMode.KeepSourceFormatting);

            //2.Create Test Result Table
            //Inorganic Result
            CreateSplitResultTable(srModel, engine, doc, "InorganicTemplate");
            //Organic Result
            CreateSplitResultTable(srModel, engine, doc, "OrganicTemplate");

            //3.Create QCTable
            if (srModel.qcTable!=null&&srModel.qcTable.Count>0)
            {
                for (int i = 0; i < srModel.qcTable.Count; i++)
                {
                    QCTableBundle qcTableBundle = srModel.qcTable[i];
                    qcTableBundle.inspectedUnitName = srModel.inspectedUnitName;
                    qcTableBundle.reportCode = srModel.orderName;
                    string tableType = qcTableBundle.tableType;
                    string tempName = "";
                    if (tableType== "有机基质加标"||tableType== "有机空白加标"||tableType== "有机平行样")
                    {
                        if (srModel.tempType == "报告模板A")
                        {
                            tempName = tableType + 'A';
                        }
                        else if (srModel.tempType == "报告模板B")
                        {
                            tempName = tableType + 'B';
                        }
                    }
                    else
                    {
                        tempName = tableType;
                    }

                    Document qcDoc = new Document(pathManagement.GetTemplatePath(siteSurveyReportPath+"QCTemplate/" + tempName + ".doc"));
                    engine.BuildReport(qcDoc, qcTableBundle, "q");
                    CommonBll.AppendDocument(doc,qcDoc,ImportFormatMode.KeepSourceFormatting);
                }
            }

            //4.Add Final 添加报告结束
            //Add Remark At DocumentEnd
            DocumentBuilder endRemarkbuilder = new DocumentBuilder(doc);
            endRemarkbuilder.MoveToDocumentEnd();
            endRemarkbuilder.Writeln();
            Paragraph p = endRemarkbuilder.InsertParagraph();
            p.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            p.AppendChild(new Run(doc, "***报告结束***"));

            //ReplaceTheUnit
            FormulaHelper formulaHelper = new FormulaHelper(doc);
            formulaHelper.ReplaceTheFormula();

            //Add watermark
            if (srModel.watermark)
            {
                CommonBll.CreateWatermarkIntoDocument(doc,"草稿报告");
            }

            string fileType = "Doc";
            string savePath = CommonBll.CreateDraftReportSavePath(srModel.reportName, fileType, srModel.organizationName, srModel.organizationId);
            doc.Save(savePath, CommonBll.GetSaveFormat(fileType));
            return savePath;
        }

        /// <summary>
        /// 拆分检测结果表格
        /// </summary>
        /// <param name="srModel"></param>
        /// <param name="engine"></param>
        /// <param name="tempPath"></param>
        /// <param name="doc"></param>
        /// <param name="type"></param>
        public static void CreateSplitResultTable(SiteSurveyReportModel srModel,ReportingEngine engine,Document doc,string type)
        {
            int maxCol = 3;

            Dictionary<int, string> dic = null;
            List<List<SResultRow>> tableList = null;
            if (type== "InorganicTemplate")
            {
                dic = inorganicTempDic;
                tableList = srModel.inorganicResuleTableList;
            }
            else if (type== "OrganicTemplate")
            {
                dic = organicTempDic;
                tableList = srModel.organicResultTableList;
            }
            if (tableList.Count > 0)
            {
                foreach (var ResultTable in tableList)
                {
                    List<SResultRow> itemTable = ResultTable;
                    if (itemTable.Count > 0)
                    {
                        int colNum = itemTable[0].resultColumns.Count;
                        if (colNum <= maxCol)
                        {
                            //列数小于等于3正常生成
                            SelectTemplateAndCreate(dic, colNum, doc, srModel, engine, itemTable);
                        }
                        else
                        {
                            int q = colNum / maxCol;
                            int r = colNum % maxCol;
                            int splits = q + (r == 0 ? 0 : 1);
                            //列数大于3需要拆分
                            for (int i = 0; i < splits; i++)
                            {
                                List<SResultRow> newtable = new List<SResultRow>();
                                //i拆分table序号
                                for (int j = 0; j < itemTable.Count; j++)
                                {
                                    SResultRow row = itemTable[j];
                                    SResultRow newRow = new SResultRow();
                                    newRow.analyteName = row.analyteName;
                                    newRow.isOrganicType = row.isOrganicType;
                                    newRow.detectionLimit = row.detectionLimit;
                                    newRow.methodCode = row.methodCode;
                                    newRow.unit = row.unit;
                                    newRow.resultColumns = new List<SResultColumn>();
                                    List<SResultColumn> col = row.resultColumns;
                                    for (int k = i * maxCol; k < (i+1) * maxCol; k++)
                                    {
                                        if (k<col.Count)
                                        {
                                            SResultColumn resultColumn = col[k];
                                            newRow.resultColumns.Add(resultColumn);
                                        }
                                    }
                                    newtable.Add(newRow);
                                }

                                if (newtable.Count>0)
                                {
                                    if (i < q)
                                    {
                                        SelectTemplateAndCreate(dic, maxCol, doc, srModel, engine, newtable);
                                    }
                                    else
                                    {
                                        SelectTemplateAndCreate(dic, r, doc, srModel, engine, newtable);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 选择项目
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="colNum"></param>
        /// <param name="doc"></param>
        /// <param name="srModel"></param>
        /// <param name="engine"></param>
        /// <param name="itemTable"></param>
        public static void SelectTemplateAndCreate(Dictionary<int,string> dic,int colNum,Document doc,SiteSurveyReportModel srModel,ReportingEngine engine, List<SResultRow> itemTable)
        {
            string dataTempPath;
            if (dic.ContainsKey(colNum))
            {
                dic.TryGetValue(colNum, out dataTempPath);
            }
            else
            {
                dataTempPath = dic.First(c => c.Key == dic.Keys.Max()).Value;
            }
            if (!string.IsNullOrEmpty(dataTempPath))
            {
                CreateTestResultTable(srModel, engine, dataTempPath, doc, itemTable);
            }
        }

        /// <summary>
        /// 使用模板引擎创建表格
        /// </summary>
        /// <param name="srModel"></param>
        /// <param name="engine"></param>
        /// <param name="tempPath"></param>
        /// <param name="doc"></param>
        /// <param name="resultTable"></param>
        /// <param name="tempName"></param>
        public static void CreateTestResultTable(SiteSurveyReportModel srModel,ReportingEngine engine, string tempPath,Document doc,List<SResultRow> resultTable)
        {
            TestDataTable testDataTable = new TestDataTable();
            testDataTable.inspectedUnitName = srModel.inspectedUnitName;
            testDataTable.reportCode = srModel.orderName;
            testDataTable.resultTable = resultTable;
            Document testDoc = new Document(tempPath);
            engine.BuildReport(testDoc, testDataTable, "o");
            CommonBll.AppendDocument(doc, testDoc, ImportFormatMode.KeepSourceFormatting);
        }

    }

    public class TestDataTable
    {
        public string inspectedUnitName { get; set; }
        public string reportCode { get; set; }

        public List<SResultRow> resultTable { get; set; }
    }

}
