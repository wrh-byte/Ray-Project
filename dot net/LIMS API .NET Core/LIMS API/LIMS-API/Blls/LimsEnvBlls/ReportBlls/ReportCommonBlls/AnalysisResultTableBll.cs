using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Reporting;
using LIMS_API.Models;
using LIMS_API.Models.LimsMrModels.MrReportModels;
using LIMS_API.Models.ReportModels.FinalReportModels;

namespace LIMS_API.Blls.ReportBlls.ReportCommonBlls
{
    /// <summary>
    /// TODO后期使用请使用泛型对该类进行优化
    /// </summary>
    public class AnalysisResultTableBll
    {

        /// <summary>
        /// SplitAndBuildTables
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="maxColumnsNum"></param>
        /// <param name="rowList"></param>
        /// <param name="deleteNullRow"></param>
        /// <param name="engine"></param>
        /// <param name="doc"></param>
        public static void SplitAndBuildTables(Dictionary<int, string> dic, int maxColumnsNum, List<ResultRow> rowList, bool deleteNullRow, ReportingEngine engine, Document doc)
        {
            if (rowList.Count > 0 && rowList[0].resultColumns != null)
            {
                //由第一行的列数判断表格的列数
                List<ResultColumn> rowColumns = rowList[0].resultColumns;
                if (rowColumns.Count > 0)
                {
                    //表格列数
                    int colNum = rowColumns.Count;
                    //不需要拆分
                    if (colNum <= maxColumnsNum)
                    {
                        if (deleteNullRow)
                        {
                            rowList = DeleteNullRow(rowList);
                        }
                        //根据行数选择模板并创建
                        SelectTemplateBuildTable(dic, colNum, engine, doc, rowList);
                    }
                    else
                    {
                        //需要拆分
                        int q = colNum / maxColumnsNum;
                        int r = colNum % maxColumnsNum;
                        int splits = q + (r == 0 ? 0 : 1);
                        //splits拆分表格数
                        for (int i = 0; i < splits; i++)
                        {
                            List<ResultRow> newTable = new List<ResultRow>();
                            //i拆分table序号
                            for (int j = 0; j < rowList.Count; j++)
                            {
                                ResultRow row = rowList[j];
                                ResultRow newRow = new ResultRow
                                {
                                    sampleName = row.sampleName,
                                    resultColumns = new List<ResultColumn>()
                                };
                                List<ResultColumn> columns = row.resultColumns;
                                for (int k = i * maxColumnsNum; k < (i + 1) * maxColumnsNum; k++)
                                {
                                    if (k < columns.Count)
                                    {
                                        ResultColumn resultColumn = columns[k];
                                        newRow.resultColumns.Add(resultColumn);
                                    }
                                }
                                newTable.Add(newRow);
                            }
                            //1.删除空行
                            if (deleteNullRow)
                            {
                                newTable = DeleteNullRow(newTable);
                            }
                            if (newTable.Count > 0)
                            {
                                //int newTableColNum = newTable[0].resultColumns.Count;
                                //SelectTemplateBuildTable(dic, newTableColNum, engine, doc, newTable);
                                //2.使用newTable创建
                                if (i < q)
                                {
                                    SelectTemplateBuildTable(dic, maxColumnsNum, engine, doc, newTable);
                                }
                                else
                                {
                                    SelectTemplateBuildTable(dic, r, engine, doc, newTable);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 拆分完整表格并生成
        /// </summary>
        /// <param name="dic">列数对应模板路径映射字典</param>
        /// <param name="maxColumnsNum">模板最大容纳列数</param>
        /// <param name="rowList">需打印的列</param>
        /// <param name="deleteNullRow">是否删除列</param>
        /// <param name="engine"></param>
        /// <param name="doc"></param>
        public static void SplitAndBuildTables(Dictionary<int, string> dic, int maxColumnsNum, List<FResultRow> rowList, bool deleteNullRow, ReportingEngine engine, Document doc)
        {
            if (rowList.Count > 0 && rowList[0].resultColumns != null)
            {
                //由第一行的列数判断表格的列数
                List<FResultColumn> rowColumns = rowList[0].resultColumns;
                if (rowColumns.Count > 0)
                {
                    //表格列数
                    int colNum = rowColumns.Count;
                    //不需要拆分
                    if (colNum <= maxColumnsNum)
                    {
                        if (deleteNullRow)
                        {
                            rowList = DeleteNullRow(rowList);
                        }
                        //根据行数选择模板并创建
                        if (rowList.Count>0)
                        {
                            SelectTemplateBuildTable(dic, colNum, engine, doc, rowList);
                        }
                    }
                    else
                    {
                        //需要拆分
                        int q = colNum / maxColumnsNum;
                        int r = colNum % maxColumnsNum;
                        int splits = q + (r == 0 ? 0 : 1);
                        //splits拆分表格数
                        for (int i = 0; i < splits; i++)
                        {
                            List<FResultRow> newTable = new List<FResultRow>();
                            //i拆分table序号
                            for (int j = 0; j < rowList.Count; j++)
                            {
                                FResultRow row = rowList[j];
                                FResultRow newRow = new FResultRow
                                {
                                    samplingDate = row.samplingDate,
                                    frequency = row.frequency,
                                    analyteName = row.analyteName,
                                    samplingPointName = row.samplingPointName,
                                    unit = row.unit,
                                    resultColumns = new List<FResultColumn>()
                                };
                                List<FResultColumn> columns = row.resultColumns;
                                for (int k = i * maxColumnsNum; k < (i + 1) * maxColumnsNum; k++)
                                {
                                    if (k < columns.Count)
                                    {
                                        FResultColumn resultColumn = columns[k];
                                        newRow.resultColumns.Add(resultColumn);
                                    }
                                }
                                newTable.Add(newRow);
                            }
                            //1.删除空行
                            if (deleteNullRow)
                            {
                                newTable = DeleteNullRow(newTable);
                            }
                            if (newTable.Count > 0)
                            {
                                //int newTableColNum = newTable[0].resultColumns.Count;
                                //SelectTemplateBuildTable(dic, newTableColNum, engine, doc, newTable);
                                //2.使用newTable创建
                                if (i < q)
                                {
                                    SelectTemplateBuildTable(dic, maxColumnsNum, engine, doc, newTable);
                                }
                                else
                                {
                                    SelectTemplateBuildTable(dic, r, engine, doc, newTable);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据列数选择相应模板并生成
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="columnNum"></param>
        /// <param name="maxTempNum"></param>
        /// <param name="engine"></param>
        /// <param name="doc"></param>
        /// <param name="rowList"></param>
        public static void SelectTemplateBuildTable(Dictionary<int, string> dic, int columnNum , ReportingEngine engine, Document doc, List<FResultRow> rowList, int maxTempNum=0)
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
                DocumentGenerate(engine, tempPath, doc, rowList);
            }
        }

        public static void SelectTemplateBuildTable(Dictionary<int, string> dic, int columnNum, ReportingEngine engine, Document doc, List<ResultRow> rowList, int maxTempNum = 0)
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
                DocumentGenerate(engine, tempPath, doc, rowList);
            }
        }

        /// <summary>
        /// 调用引擎生成文档
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="tempPath"></param>
        /// <param name="doc"></param>
        /// <param name="resultTable"></param>
        public static void DocumentGenerate(ReportingEngine engine, string tempPath, Document doc, List<FResultRow> resultTable)
        {
            TestDataTable testDataTable = new TestDataTable();
            //testDataTable.matrixName = "测试基质名称";
            //testDataTable.sampleRemark = "测试样品描述";
            testDataTable.resultTable = resultTable;
            Document c1Doc = new Document(tempPath);
            engine.BuildReport(c1Doc, testDataTable, "o");

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();
            builder.InsertDocument(c1Doc, ImportFormatMode.KeepSourceFormatting);
        }

        public static void DocumentGenerate(ReportingEngine engine, string tempPath, Document doc, List<ResultRow> resultTable)
        {
            TestDataTableCommon testDataTable = new TestDataTableCommon();
            testDataTable.resultRows = resultTable;
            Document c1Doc = new Document(tempPath);
            engine.BuildReport(c1Doc, testDataTable, "o");

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();
            builder.InsertDocument(c1Doc, ImportFormatMode.KeepSourceFormatting);
        }

        /// <summary>
        /// 传递给Report engine 的Model
        /// </summary>
        public class TestDataTable
        {
            public string matrixName { get; set; }
            public string sampleRemark { get; set; }
            public List<FResultRow> resultTable { get; set; }
        }

        public class TestDataTableCommon
        {
            public string matrixName { get; set; }
            public string sampleRemark { get; set; }
            public List<ResultRow> resultRows { get; set; }
        }


        /// <summary>
        /// 删除空行
        /// </summary>
        /// <param name="rowList"></param>
        /// <returns></returns>
        public static List<FResultRow> DeleteNullRow(List<FResultRow> rowList)
        {
            var rows = new List<FResultRow>();
            for (int i = 0; i < rowList.Count; i++)
            {
                var cols = rowList[i].resultColumns;
                int flag = 0;
                foreach (var col in cols)
                {
                    if (col.value == "-")
                    {
                        flag++;
                    }
                }
                if (flag != cols.Count)
                {
                    rows.Add(rowList[i]);
                }
            }
            return rows;
        }

        /// <summary>
        /// DeleteNullRow
        /// </summary>
        /// <param name="rowList"></param>
        /// <returns></returns>
        public static List<ResultRow> DeleteNullRow(List<ResultRow> rowList)
        {
            var rows = new List<ResultRow>();
            for (int i = 0; i < rowList.Count; i++)
            {
                var cols = rowList[i].resultColumns;
                int flag = 0;
                foreach (var col in cols)
                {
                    if (col.value == "-")
                    {
                        flag++;
                    }
                }
                if (flag != cols.Count)
                {
                    rows.Add(rowList[i]);
                }
            }
            return rows;
        }
    }
}
