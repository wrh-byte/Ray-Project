using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.Words;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Blls.ReportBlls.ReportCommonBlls;
using LIMS_API.Models.OriginalRecordModels;


namespace LIMS_API.Blls.LimsEnvBlls
{
    /// <summary>
    /// 打印原始记录
    /// </summary>
    public class OriginalRecordBll
    {
        private static ReportingEngine _engine;
        private static Dictionary<string, string> _qcTempDic;

        /// <summary>
        /// 生成原始记录Main
        /// </summary>
        /// <param name="orModel"></param>
        /// <returns></returns>
        public static string CreateOriginalRecord(OriginalRecordModel orModel)
        {
            try
            {
                //Get Template
                PathManagementBll pathManagement = new PathManagementBll(orModel.organizationName,orModel.organizationId);

                //原始记录
                string originalRecordTempPath = pathManagement.GetTemplatePath("OriginalRecordTemplate/OriginalRecordTemplate.doc");
                //测试表格
                Dictionary<int, string> originalDataDic = new Dictionary<int, string>
                {
                    {4,pathManagement.GetTemplatePath("OriginalRecordTemplate/ResultTableTemp/ResultTableTemplate.doc") },
                };

                //油烟检测表格
                Dictionary<int, string> lamResultTempDic = new Dictionary<int, string>
                {
                    {5,pathManagement.GetTemplatePath("OriginalRecordTemplate/ResultTableTemp/LamResultTemplate.doc") }
                };

                //标准曲线
                string standardLineTempPath = pathManagement.GetTemplatePath("OriginalRecordTemplate/StandardLinedAndSolution/StandardLinedTemplate.doc");
                //标准溶液
                string standardSolutionTempPath = pathManagement.GetTemplatePath("OriginalRecordTemplate/StandardLinedAndSolution/StandardSolutionTemplate.doc");
                //标样表格
                string standardSampleTempPath = pathManagement.GetTemplatePath("OriginalRecordTemplate/QCTemplate/StandardSampleTemplate.doc");
                //QC Table
                _qcTempDic = new Dictionary<string, string>
                {
                    {"平行样",pathManagement.GetTemplatePath("OriginalRecordTemplate/QCTemplate/ParallelSampleQCTableTemplate.doc") },
                    {"空白加标",pathManagement.GetTemplatePath("OriginalRecordTemplate/QCTemplate/BlkAddStandardQCTableTemplate.doc") },
                    {"基质加标",pathManagement.GetTemplatePath("OriginalRecordTemplate/QCTemplate/AddStandardQCTableTemplate.doc") },
                    {"基质加标平行",pathManagement.GetTemplatePath("OriginalRecordTemplate/QCTemplate/AddStandardParalleQCTableTemplate.doc") },

                };

                Document doc = new Document(originalRecordTempPath);
                DocumentBuilder builder = new DocumentBuilder(doc);

                //Create ReportEngine
                _engine = new ReportingEngine();


                //.Pre Blank Cell
                if (orModel.baseInfo.matrixName=="噪声")
                {
                    orModel.baseInfo.blankKey = "检出限";
                    orModel.baseInfo.blankValue = orModel.baseInfo.limitsValue;
                }
                else
                {
                    orModel.baseInfo.blankKey = "样品基质";
                    orModel.baseInfo.blankValue = orModel.baseInfo.matrixName;
                }

                //.Create BaseInfo
                _engine.BuildReport(doc,orModel,"o");

                //.Create SignImage

                ReportImageBll reportImageBll = new ReportImageBll();
                DocumentBuilder namebuilder = new DocumentBuilder(doc);
                if (orModel.baseInfo.reviewerSignImage!=null)
                {
                    reportImageBll.InsertImgToDocument(orModel, doc, "reviewerSignImage", ReportImageBll.ImageType.SignImage, null, orModel.baseInfo.reviewerSignImage, 80, 30);
                }
                else if (orModel.baseInfo.reviewerName!=null)
                {
                    namebuilder.MoveToBookmark("reviewerSignImage");
                    namebuilder.Write(orModel.baseInfo.reviewerName);
                }

                if (orModel.baseInfo.experimenterSignImage!=null)
                {
                    reportImageBll.InsertImgToDocument(orModel, doc, "experimenterSignImage", ReportImageBll.ImageType.SignImage, null, orModel.baseInfo.experimenterSignImage, 80, 30);
                }
                else if (orModel.baseInfo.experimenterName!=null)
                {
                    namebuilder.MoveToBookmark("experimenterSignImage");
                    namebuilder.Write(orModel.baseInfo.experimenterName);
                }
                
                
                //.Create TestInfo
                builder.MoveToDocumentEnd();
                builder.Writeln("二、测试原始数据：");
                //若为油烟则使用油烟模板
                if (orModel.baseInfo.matrixName=="油烟")
                {
                    SplitResultTable(lamResultTempDic, 5, orModel.resultTable,doc);
                }
                else
                {
                    SplitResultTable(originalDataDic, 4, orModel.resultTable,doc);
                }
                

                //.Create StandardLine
                if (orModel.standardLined!=null)
                {
                    builder.MoveToDocumentEnd();
                    builder.Writeln("标准曲线：");
                    CommonBll.GenerateTableAppendToDoc(doc, standardLineTempPath, orModel.standardLined,"l");
                }
                
                //.Create StandardSolution
                if (orModel.standardSolution!=null)
                {
                    builder.MoveToDocumentEnd();
                    builder.Writeln("标准溶液：");
                    CommonBll.GenerateTableAppendToDoc(doc,standardSolutionTempPath,orModel.standardSolution,"s");
                }
                


                //.QCTable
                if (orModel.parallelSampleQCTables != null|| orModel.blkAddStandardQCTables!=null|| orModel.addStandardQCTables != null|| orModel.addStandardParalleQCTables != null|| orModel.standardSamples != null)
                {
                    builder.MoveToDocumentEnd();
                    builder.Writeln("三、质控信息：");
                }

                //.StandardSample 标样表格
                if (orModel.standardSamples != null&&orModel.standardSamples.Count>0)
                {
                    builder.MoveToDocumentEnd();
                    builder.Writeln("标样：");
                    CommonBll.GenerateTableAppendToDoc(doc, standardSampleTempPath, orModel.standardSamples, "p");
                }

                if (orModel.parallelSampleQCTables!=null&&orModel.parallelSampleQCTables.Count>0)
                {
                    builder.MoveToDocumentEnd();
                    builder.Writeln("平行样：");
                    GenerateQCTable(doc,orModel.parallelSampleQCTables);
                }
                //空白加标
                if (orModel.blkAddStandardQCTables!=null&&orModel.blkAddStandardQCTables.Count>0)
                {
                    builder.MoveToDocumentEnd();
                    builder.Writeln("空白加标：");
                    GenerateQCTable(doc, orModel.blkAddStandardQCTables);
                }
                //基质加标
                if (orModel.addStandardQCTables!=null&&orModel.addStandardQCTables.Count>0)
                {
                    builder.MoveToDocumentEnd();
                    builder.Writeln("基质加标：");
                    GenerateQCTable(doc, orModel.addStandardQCTables);
                }
                //基质加标平行
                if (orModel.addStandardParalleQCTables!=null&&orModel.addStandardParalleQCTables.Count>0)
                {
                    builder.MoveToDocumentEnd();
                    builder.Writeln("基质加标平行：");
                    GenerateQCTable(doc, orModel.addStandardParalleQCTables);
                }

                //ReplaceTheUnit
                FormulaHelper formulaHelper = new FormulaHelper(doc);
                formulaHelper.ReplaceTheFormula();

                string fileType = "Doc";
                string savePath = CommonBll.CreateSaveFilePath("Original_" + orModel.baseInfo.orderName, fileType, orModel.organizationName,orModel.organizationId);
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
        /// 生成质控表格
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="qcTableList"></param>
        public static void GenerateQCTable(Document doc, List<QCTableModel> qcTableList)
        {
            foreach (QCTableModel qcTable in qcTableList)
            {
                string tempPath;
                _qcTempDic.TryGetValue(qcTable.qcType, out tempPath);
                if (!string.IsNullOrEmpty(tempPath))
                {
                    CommonBll.GenerateTableAppendToDoc(doc, tempPath, qcTable, "q");
                }
            }
        }


        public static void SplitResultTable(Dictionary<int,string> dic,int maxColumnsNum, OriginalRecordResultTableModel resultTable, Document doc)
        {
            List<OriginalRecordResultRowModel> resultRows = resultTable.resultRows;
            if (resultRows.Count > 0)
            {
                List<OriginalRecordResultColumnModel> fResultColumns = resultRows[0].resultColumns;
                if (fResultColumns.Count > 0)
                {
                    int colNum = fResultColumns.Count;
                    if (colNum <= 4)
                    {
                        if (resultRows.Count > 0)
                        {
                            string averageValue = "";
                            if (resultTable.valueAVEs.Count>0)
                            {
                                averageValue = resultTable.valueAVEs[0];
                            }
                            OriginalResultTable originalRecordResultTable = new OriginalResultTable
                            {
                                formula = resultTable.formula,
                                remark = resultTable.remark,
                                limit = resultTable.stdLimit,
                                average = averageValue,
                                resultRows = resultTable.resultRows
                            };
                            SelectTemplateBuildTable(dic,colNum,doc, originalRecordResultTable);
                        }
                    }
                    else
                    {
                        //列数大于三
                        int q = colNum / maxColumnsNum;
                        int r = colNum % maxColumnsNum;
                        int splits = q + (r == 0 ? 0 : 1);
                        //splits拆分表格数
                        for (int i = 0; i < splits; i++)
                        {
                            List<OriginalRecordResultRowModel> newtable = new List<OriginalRecordResultRowModel>();
                            //i拆分table序号
                            for (int j = 0; j < resultRows.Count; j++)
                            {
                                OriginalRecordResultRowModel row = resultRows[j];
                                OriginalRecordResultRowModel newRow = new OriginalRecordResultRowModel();
                                newRow.analyteName = row.analyteName;
                                newRow.analyteCode = row.analyteCode;
                                newRow.analyteUnit = row.analyteUnit;
                                newRow.stdLimit = row.stdLimit;
                                newRow.resultColumns = new List<OriginalRecordResultColumnModel>();
                                List<OriginalRecordResultColumnModel> col = row.resultColumns;
                                for (int k = i * maxColumnsNum; k < (i + 1) * maxColumnsNum; k++)
                                {
                                    if (k < col.Count)
                                    {
                                        OriginalRecordResultColumnModel resultColumn = col[k];
                                        newRow.resultColumns.Add(resultColumn);
                                    }
                                }
                                newtable.Add(newRow);
                            }

                            if (newtable.Count > 0)
                            {
                                string averageValue = "";
                                if (resultTable.valueAVEs.Count>=splits)
                                {
                                    averageValue = resultTable.valueAVEs[i];
                                }
                                OriginalResultTable originalResultTable = new OriginalResultTable
                                {
                                    formula = resultTable.formula,
                                    remark = resultTable.remark,
                                    limit = resultTable.stdLimit,
                                    average = averageValue,
                                    resultRows = newtable
                                };
                                if (i < q)
                                {
                                    SelectTemplateBuildTable(dic, maxColumnsNum, doc, originalResultTable);
                                }
                                else
                                {
                                    SelectTemplateBuildTable(dic, r, doc, originalResultTable);
                                }
                            }
                        }
                    }
                }
            }
        }



        /// <summary>
        /// SelectTemplateBuildTable
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="columnNum"></param>
        /// <param name="doc"></param>
        /// <param name="originalResultTable"></param>
        public static void SelectTemplateBuildTable(Dictionary<int, string> dic, int columnNum, Document doc, OriginalResultTable originalResultTable)
        {
            string tempPath;
            if (dic.ContainsKey(columnNum))
            {
                dic.TryGetValue(columnNum, out tempPath);
            }
            else
            {
                tempPath = dic.First(c => c.Key == dic.Keys.Max()).Value;
            }
            if (!string.IsNullOrEmpty(tempPath))
            {
                DocumentGenerate(tempPath, doc, originalResultTable);
            }
        }

        /// <summary>
        /// originalResultTable
        /// </summary>
        /// <param name="tempPath"></param>
        /// <param name="doc"></param>
        /// <param name="originalResultTable"></param>
        public static void DocumentGenerate(string tempPath, Document doc, OriginalResultTable originalResultTable)
        {
            Document c1Doc = new Document(tempPath);
            _engine.BuildReport(c1Doc, originalResultTable, "resultTable");

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();
            builder.InsertDocument(c1Doc, ImportFormatMode.KeepSourceFormatting);
        }

        /// <summary>
        /// 原始记录数据表格信息
        /// </summary>
        public class OriginalResultTable
        {
            /// <summary>
            /// 平均值
            /// </summary>
            public string average { get; set; }

            /// <summary>
            /// 公式
            /// </summary>
            public string formula { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string remark { get; set; }

            /// <summary>
            /// 检出限
            /// </summary>
            public string limit { get; set; }

            /// <summary>
            /// 测试结果表格
            /// </summary>
            public List<OriginalRecordResultRowModel> resultRows { get; set; }
        }

    }
}
