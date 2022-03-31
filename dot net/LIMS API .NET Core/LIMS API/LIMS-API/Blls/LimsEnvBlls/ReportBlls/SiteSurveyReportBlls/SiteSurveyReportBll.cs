using System;
using System.Collections.Generic;
using Aspose.Cells;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models.ReportModels.SiteSureyReportModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Row = Aspose.Cells.Row;
using SaveFormat = Aspose.Cells.SaveFormat;

namespace LIMS_API.Bll
{
    //worksheet.Cells[行, 列].Style.Border.Left.Style = (ExcelBorderStyle)CellBorderType.Dotted;
    public class SiteSurveyReportBll
    {
        /// <summary>
        /// CreateSiteSurveyReport
        /// </summary>
        /// <param name="srModel"></param>
        /// <returns></returns>
        public static string CreateSiteSurveyReport(SiteSurveyReportModel srModel)
        {
            try
            {
                PathManagementBll pathManagement = new PathManagementBll(srModel.organizationName, srModel.organizationId);
                string temp = pathManagement.GetTemplatePath("ReportTemplate/SiteSurveyTemplate/场调模板A.xlsx");
                Workbook tempWb = new Workbook(temp);

                //New WorkBook
                Workbook newWorkbook = new Workbook();


                //Add Cover Page
                //Cover Info
                Worksheet coverWs = tempWb.Worksheets[0];
                //orderName
                CommonBll.InsertContentInCells(coverWs, "D8", srModel.orderName);
                //clientName
                CommonBll.InsertContentInCells(coverWs, "D10", srModel.clientName);
                //clientAddress
                CommonBll.InsertContentInCells(coverWs, "D11", srModel.clientAddress);
                //companyName
                CommonBll.InsertContentInCells(coverWs, "A37", srModel.companyName);
                newWorkbook.Worksheets[0].Copy(coverWs);

                //Add Description page
                var desWs = newWorkbook.Worksheets.Add("说明");
                desWs.Copy(tempWb.Worksheets[1]);

                //Create Method Table
                if (srModel.methodInfoTable.Count>0)
                {
                    var methodWs = newWorkbook.Worksheets.Add("方法");
                    methodWs.Copy(CreateMethodTable(tempWb, srModel));
                }


                //Create Result Table

                #region 测试结果


                //有机测试结果
                if (srModel.organicResultTableList.Count > 0)
                {
                    List<List<SResultRow>> organicResultTableList = srModel.organicResultTableList;
                    for(int i = 0; i < organicResultTableList.Count; i++)
                    {
                        Worksheet newWs = newWorkbook.Worksheets.Add("有机测试"+i);
                        Worksheet oWs = tempWb.Worksheets[3];
                        List<SResultRow> organicResultList = organicResultTableList[i];
                        newWs.Copy(CreateResultTable(oWs, srModel, organicResultList));
                    }
                }

                //无机理化测试结果
                if (srModel.inorganicResuleTableList.Count > 0)
                {
                    List<List<SResultRow>> inorganicResuleTableList = srModel.inorganicResuleTableList;
                    for (int i = 0; i < inorganicResuleTableList.Count; i++)
                    {
                        Worksheet newWs = newWorkbook.Worksheets.Add("金属及理化" + i);
                        Worksheet oWs = tempWb.Worksheets[4];
                        List<SResultRow> inorganicResuleList = inorganicResuleTableList[i];
                        newWs.Copy(CreateResultTable(oWs, srModel, inorganicResuleList));
                    }
                }

                #endregion

                #region 质控部分

                if (srModel.qcTable.Count>0)
                {
                    for (int i=0;i<srModel.qcTable.Count;i++)
                    {
                        QCTableBundle qcTableBundle = srModel.qcTable[i];
                        if (qcTableBundle.tableType== "无机质控")
                        {
                            Worksheet newWs = newWorkbook.Worksheets.Add("无机质控" + i);
                            newWs.Copy(Inorganic_QC(tempWb,srModel, qcTableBundle));
                        }
                        else if(qcTableBundle.tableType== "无机平行样")
                        {
                            Worksheet newWs = newWorkbook.Worksheets.Add("无机平行样" + i);
                            newWs.Copy(Inorganic_Parallel(tempWb, srModel, qcTableBundle));
                        }
                        else if (qcTableBundle.tableType == "无机空白加标")
                        {
                            Worksheet newWs = newWorkbook.Worksheets.Add("无机空白加标" + i);
                            newWs.Copy(Inorganic_Blank_Markup(tempWb, srModel, qcTableBundle));
                        }
                        else if (qcTableBundle.tableType == "无机基质加标")
                        {
                            Worksheet newWs = newWorkbook.Worksheets.Add("无机基质加标" + i);
                            newWs.Copy(Inorganic_Matrix_Markup(tempWb, srModel, qcTableBundle));
                        }
                        else if (qcTableBundle.tableType == "无机基质加标平行")
                        {
                            Worksheet newWs = newWorkbook.Worksheets.Add("无机基质加标平行" + i);
                            newWs.Copy(Inorganic_Matrix_Markup_Parallel(tempWb, srModel, qcTableBundle));
                        }
                        else if (qcTableBundle.tableType == "有机平行样")
                        {
                            Worksheet newWs = newWorkbook.Worksheets.Add("有机平行样" + i);
                            newWs.Copy(Organic_Parallel(tempWb, srModel, qcTableBundle));
                        }
                        else if (qcTableBundle.tableType == "有机空白加标")
                        {
                            Worksheet newWs = newWorkbook.Worksheets.Add("有机空白加标" + i);
                            newWs.Copy(Organic_Blank_Markup(tempWb, srModel, qcTableBundle));
                        }
                        else if (qcTableBundle.tableType == "有机基质加标")
                        {
                            Worksheet newWs = newWorkbook.Worksheets.Add("有机基质加标" + i);
                            newWs.Copy(Organic_Matrix_Markup(tempWb, srModel, qcTableBundle));
                        }
                    }
                }

                #endregion

                string fileType = "Xlsx";
                string savePath = CommonBll.CreateDraftReportSavePath(srModel.reportName, fileType, srModel.organizationName, srModel.organizationId);
                newWorkbook.Save(savePath, SaveFormat.Xlsx);
                return savePath;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        /// <summary>
        /// CreateMethodTable
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="srModel"></param>
        public static Worksheet CreateMethodTable(Workbook wb, SiteSurveyReportModel srModel)
        {
            Worksheet ws = wb.Worksheets[2];

            CommonBll.InsertContentInCells(ws, "A1", "报告编号：" + srModel.orderName);
            CommonBll.InsertContentInCells(ws, "C2", srModel.sampleNum);
            CommonBll.InsertContentInCells(ws, "G2", srModel.sampleState);
            CommonBll.InsertContentInCells(ws, "C3", srModel.getSampleDate);
            CommonBll.InsertContentInCells(ws, "G3", srModel.analysisDate);


            int rowNum = srModel.methodInfoTable.Count;
            int rowIndex = 4;
            ws.Cells.InsertRows(rowIndex, rowNum);

            List<int> mergeIndex = new List<int>();
            for (int i = 0; i < rowNum; i++)
            {
                Row row = ws.Cells.Rows[rowIndex];
                row.Height = 50;
                
                row[0].Value = srModel.methodInfoTable[i].sampleType;
                row[1].Value = srModel.methodInfoTable[i].testProject;
                row[2].Value = srModel.methodInfoTable[i].testStandard;
                row[4].Value = srModel.methodInfoTable[i].detectionLimit;
                row[5].Value = srModel.methodInfoTable[i].equipmentName;
                row[7].Value = srModel.methodInfoTable[i].serialNumber;

                //merge cell
                ws.Cells.Merge(rowIndex, 2, 1, 2);
                ws.Cells.Merge(rowIndex, 5, 1, 2);
                rowIndex++;
            }

            //merge range
            //Range range = ws.Cells.CreateRange(4, 0, rowNum, 1);
            //range.Merge();
            return ws;
        }

        /// <summary>
        /// CreateResultTable
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="srModel"></param>
        public static Worksheet CreateResultTable(Worksheet ws, SiteSurveyReportModel srModel,List<SResultRow> resultTable)
        {

            //报告编号
            CommonBll.InsertContentInCells(ws, "A3", "报告编号："+srModel.orderName);
            //受检项目名称
            CommonBll.InsertContentInCells(ws, "A4", "受检单位/项目名称:"+srModel.inspectedUnitName);

            ////设置打印标题行
            //PageSetup pageSetup = ws.PageSetup;
            //pageSetup.PrintTitleColumns = "$A:$C";
            //pageSetup.PrintTitleRows = "$1:$5";


            List<Aspose.Cells.Cell> cellStyleSetList = new List<Aspose.Cells.Cell>();

            int rowCount = resultTable.Count;
            int excelRowIndex = 5;
            for (int i = 0; i < rowCount; i++)
            {
                SResultRow row = resultTable[i];
                Row excelRow = ws.Cells.Rows[excelRowIndex];
                excelRow[0].Value = row.analyteName;
                excelRow[1].Value = row.methodCode;
                excelRow[2].Value = row.detectionLimit;
                excelRow[3].Value = row.unit;

                //change style
                cellStyleSetList.Add(excelRow[0]);
                cellStyleSetList.Add(excelRow[1]);
                cellStyleSetList.Add(excelRow[2]);
                cellStyleSetList.Add(excelRow[3]);

                List<SResultColumn> columns = row.resultColumns;
                int colCount = columns.Count;

                //col num
                int excelColIndex = 4;
                for (int j = 0; j < colCount; j++)
                {
                    SResultColumn col = columns[j];
                    if (i == 0)
                    {
                        Row row0 = ws.Cells.Rows[0];
                        row0[excelColIndex].Value = col.sampleNum;
                        Row row1 = ws.Cells.Rows[1];
                        row1[excelColIndex].Value = col.sampleMark;
                        Row row2 = ws.Cells.Rows[2];
                        row2[excelColIndex].Value = col.samplingDate;
                        Row row3 = ws.Cells.Rows[3];
                        row3[excelColIndex].Value = col.receivedDate;
                        Row row4 = ws.Cells.Rows[4];
                        row4[excelColIndex].Value = col.matrixName;

                        //set cell style
                        cellStyleSetList.Add(row0[excelColIndex]);
                        cellStyleSetList.Add(row1[excelColIndex]);
                        cellStyleSetList.Add(row2[excelColIndex]);
                        cellStyleSetList.Add(row3[excelColIndex]);
                        cellStyleSetList.Add(row4[excelColIndex]);
                    }

                    excelRow[excelColIndex].Value = col.value;
                    cellStyleSetList.Add(excelRow[excelColIndex]);

                    excelColIndex++;
                }

                excelRowIndex++;
            }

            //set style
            CommonBll.SetCellsStyle(cellStyleSetList);

            return ws;
        }

        /// <summary>
        /// 无机质控
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="srModel"></param>
        /// <returns></returns>
        public static Worksheet Inorganic_QC(Workbook wb,SiteSurveyReportModel srModel ,QCTableBundle qcTableBundle)
        {
            Worksheet ws = wb.Worksheets[5];
            CommonBll.InsertContentInCells(ws, "C1", srModel.inspectedUnitName);
            CommonBll.InsertContentInCells(ws, "G1", srModel.orderName);
            CommonBll.InsertContentInCells(ws, "G2", qcTableBundle.qcMatrix);

            List<Aspose.Cells.Cell> cellStyleSetList = new List<Aspose.Cells.Cell>();
            List<QCRow> list = qcTableBundle.qcRowList;
            int rowIndex = 3;
            for (int i = 0; i < list.Count; i++)
            {
                Row row = ws.Cells.Rows[rowIndex];
                QCRow listItem = list[i];
                row[0].Value = listItem.analyteName;
                row[1].Value = listItem.sampleCode;
                row[2].Value = listItem.qcSampleCode;
                row[3].Value = listItem.analysisDate;
                row[4].Value = listItem.value;
                row[5].Value = listItem.unit;
                row[6].Value = listItem.standardRange;

                cellStyleSetList.Add(row[0]);
                cellStyleSetList.Add(row[1]);
                cellStyleSetList.Add(row[2]);
                cellStyleSetList.Add(row[3]);
                cellStyleSetList.Add(row[4]);
                cellStyleSetList.Add(row[5]);
                cellStyleSetList.Add(row[6]);

                rowIndex++;
            }

            CommonBll.SetCellsStyle(cellStyleSetList);

            return ws;
        }

        /// <summary>
        /// 无机空白加标
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="srModel"></param>
        /// <returns></returns>
        public static Worksheet Inorganic_Blank_Markup(Workbook wb, SiteSurveyReportModel srModel, QCTableBundle qcTableBundle)
        {
            Worksheet ws = wb.Worksheets[6];
            CommonBll.InsertContentInCells(ws, "C1", "受检单位/项目名称:" + srModel.inspectedUnitName);
            CommonBll.InsertContentInCells(ws, "I1", srModel.orderName);
            CommonBll.InsertContentInCells(ws, "I2", qcTableBundle.qcMatrix);

            List<Aspose.Cells.Cell> cellStyleSetList = new List<Aspose.Cells.Cell>();
            var list = qcTableBundle.qcRowList;
            int rowIndex = 3;
            for (int i = 0; i < list.Count; i++)
            {
                Row row = ws.Cells.Rows[rowIndex];
                var listItem = list[i];
                row[0].Value = listItem.analyteName;
                row[1].Value = listItem.sampleCode;
                row[2].Value = listItem.analysisDate;
                row[3].Value = listItem.sampleConcentration;
                row[4].Value = listItem.unit;
                row[5].Value = listItem.much;
                row[6].Value = listItem.measured;
                row[7].Value = listItem.recoveryRate;
                row[8].Value = listItem.controlRange;

                cellStyleSetList.Add(row[0]);
                cellStyleSetList.Add(row[1]);
                cellStyleSetList.Add(row[2]);
                cellStyleSetList.Add(row[3]);
                cellStyleSetList.Add(row[4]);
                cellStyleSetList.Add(row[5]);
                cellStyleSetList.Add(row[6]);
                cellStyleSetList.Add(row[7]);
                cellStyleSetList.Add(row[8]);

                rowIndex++;
            }

            CommonBll.SetCellsStyle(cellStyleSetList);

            return ws;
        }

        /// <summary>
        /// 无机基质加标
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="srModel"></param>
        /// <returns></returns>
        public static Worksheet Inorganic_Matrix_Markup(Workbook wb, SiteSurveyReportModel srModel, QCTableBundle qcTableBundle)
        {
            Worksheet ws = wb.Worksheets[7];
            CommonBll.InsertContentInCells(ws, "C1", "受检单位/项目名称:" + srModel.inspectedUnitName);
            CommonBll.InsertContentInCells(ws, "I1", srModel.orderName);
            CommonBll.InsertContentInCells(ws, "I2", qcTableBundle.qcMatrix);

            List<Aspose.Cells.Cell> cellStyleSetList = new List<Aspose.Cells.Cell>();
            var list = qcTableBundle.qcRowList;
            int rowIndex = 3;
            for (int i = 0; i < list.Count; i++)
            {
                Row row = ws.Cells.Rows[rowIndex];
                var listItem = list[i];
                row[0].Value = listItem.analyteName;
                row[1].Value = listItem.sampleCode;
                row[2].Value = listItem.analysisDate;
                row[3].Value = listItem.sampleResult;
                row[4].Value = listItem.unit;
                row[5].Value = listItem.much;
                row[6].Value = listItem.measured;
                row[7].Value = listItem.recoveryRate;
                row[8].Value = listItem.controlRange;

                cellStyleSetList.Add(row[0]);
                cellStyleSetList.Add(row[1]);
                cellStyleSetList.Add(row[2]);
                cellStyleSetList.Add(row[3]);
                cellStyleSetList.Add(row[4]);
                cellStyleSetList.Add(row[5]);
                cellStyleSetList.Add(row[6]);
                cellStyleSetList.Add(row[7]);
                cellStyleSetList.Add(row[8]);

                rowIndex++;
            }

            CommonBll.SetCellsStyle(cellStyleSetList);

            return ws;
        }

        /// <summary>
        /// 无机基质加标平行
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="srModel"></param>
        /// <returns></returns>
        public static Worksheet Inorganic_Matrix_Markup_Parallel(Workbook wb, SiteSurveyReportModel srModel, QCTableBundle qcTableBundle)
        {
            Worksheet ws = wb.Worksheets[8];
            CommonBll.InsertContentInCells(ws, "C1", "受检单位/项目名称:" + srModel.inspectedUnitName);
            CommonBll.InsertContentInCells(ws, "K1", srModel.orderName);
            CommonBll.InsertContentInCells(ws, "K2", qcTableBundle.qcMatrix);

            List<Aspose.Cells.Cell> cellStyleSetList = new List<Aspose.Cells.Cell>();
            var list = qcTableBundle.qcRowList;
            int rowIndex = 3;
            for (int i = 0; i < list.Count; i++)
            {
                Row row = ws.Cells.Rows[rowIndex];
                var listItem = list[i];
                row[0].Value = listItem.analyteName;
                row[1].Value = listItem.sampleCode;
                row[2].Value = listItem.analysisDate;
                row[3].Value = listItem.sampleResult;
                row[4].Value = listItem.unit;
                row[5].Value = listItem.much;
                row[6].Value = listItem.measured;
                row[7].Value = listItem.m_markup;
                row[8].Value = listItem.recoveryRate;
                row[9].Value = listItem.parallelRecoveryRate;
                row[10].Value = listItem.aveRecoveryRate;
                row[11].Value = listItem.relativeDeviation;
                row[12].Value = listItem.controlRange;

                cellStyleSetList.Add(row[0]);
                cellStyleSetList.Add(row[1]);
                cellStyleSetList.Add(row[2]);
                cellStyleSetList.Add(row[3]);
                cellStyleSetList.Add(row[4]);
                cellStyleSetList.Add(row[5]);
                cellStyleSetList.Add(row[6]);
                cellStyleSetList.Add(row[7]);
                cellStyleSetList.Add(row[8]);
                cellStyleSetList.Add(row[9]);
                cellStyleSetList.Add(row[10]);
                cellStyleSetList.Add(row[11]);
                cellStyleSetList.Add(row[12]);

                rowIndex++;
            }

            CommonBll.SetCellsStyle(cellStyleSetList);

            return ws;
        }

        /// <summary>
        /// 无机平行样
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="srModel"></param>
        /// <returns></returns>
        public static Worksheet Inorganic_Parallel(Workbook wb, SiteSurveyReportModel srModel, QCTableBundle qcTableBundle)
        {
            Worksheet ws = wb.Worksheets[9];
            CommonBll.InsertContentInCells(ws, "C1", "受检单位/项目名称:" + srModel.inspectedUnitName);
            CommonBll.InsertContentInCells(ws, "H1", srModel.orderName);
            CommonBll.InsertContentInCells(ws, "H2", qcTableBundle.qcMatrix);

            List<Aspose.Cells.Cell> cellStyleSetList = new List<Aspose.Cells.Cell>();
            var list = qcTableBundle.qcRowList;
            int rowIndex = 3;
            for (int i = 0; i < list.Count; i++)
            {
                Row row = ws.Cells.Rows[rowIndex];
                var listItem = list[i];
                row[0].Value = listItem.analyteName;
                row[1].Value = listItem.sampleCode;
                row[2].Value = listItem.analysisDate;
                row[3].Value = listItem.sampleResult;
                row[4].Value = listItem.parallelResults;
                row[5].Value = listItem.unit;
                row[6].Value = listItem.relativeDeviation;
                row[7].Value = listItem.controlRange;

                cellStyleSetList.Add(row[0]);
                cellStyleSetList.Add(row[1]);
                cellStyleSetList.Add(row[2]);
                cellStyleSetList.Add(row[3]);
                cellStyleSetList.Add(row[4]);
                cellStyleSetList.Add(row[5]);
                cellStyleSetList.Add(row[6]);
                cellStyleSetList.Add(row[7]);

                rowIndex++;
            }

            CommonBll.SetCellsStyle(cellStyleSetList);

            return ws;
        }

        /// <summary>
        /// 有机空白加标
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="srModel"></param>
        /// <returns></returns>
        public static Worksheet Organic_Blank_Markup(Workbook wb, SiteSurveyReportModel srModel, QCTableBundle qcTableBundle)
        {
            Worksheet ws = wb.Worksheets[10];
            CommonBll.InsertContentInCells(ws, "B1", "受检单位/项目名称:" + srModel.inspectedUnitName);
            CommonBll.InsertContentInCells(ws, "G1", srModel.orderName);
            CommonBll.InsertContentInCells(ws, "E3", qcTableBundle.qcMatrix);
            CommonBll.InsertContentInCells(ws, "G2", qcTableBundle.analysisDate);
            CommonBll.InsertContentInCells(ws, "G3", qcTableBundle.sampleCode);

            List<Aspose.Cells.Cell> cellStyleSetList = new List<Aspose.Cells.Cell>();
            var list = qcTableBundle.qcRowList;
            int rowIndex = 4;
            for (int i = 0; i < list.Count; i++)
            {
                Row row = ws.Cells.Rows[rowIndex];
                var listItem = list[i];
                row[0].Value = listItem.analyteName;
                row[1].Value = listItem.concentration;
                row[2].Value = listItem.unit;
                row[3].Value = listItem.much;
                row[4].Value = listItem.measured;
                row[5].Value = listItem.recoveryRate;
                row[6].Value = listItem.controlRange;

                cellStyleSetList.Add(row[0]);
                cellStyleSetList.Add(row[1]);
                cellStyleSetList.Add(row[2]);
                cellStyleSetList.Add(row[3]);
                cellStyleSetList.Add(row[4]);
                cellStyleSetList.Add(row[5]);
                cellStyleSetList.Add(row[6]);

                rowIndex++;
            }

            CommonBll.SetCellsStyle(cellStyleSetList);

            return ws;
        }

        /// <summary>
        /// 有机基质加标
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="srModel"></param>
        /// <returns></returns>
        public static Worksheet Organic_Matrix_Markup(Workbook wb, SiteSurveyReportModel srModel, QCTableBundle qcTableBundle)
        {
            Worksheet ws = wb.Worksheets[11];
            CommonBll.InsertContentInCells(ws, "B1", "受检单位/项目名称:" + srModel.inspectedUnitName);
            CommonBll.InsertContentInCells(ws, "G1", srModel.orderName);
            CommonBll.InsertContentInCells(ws, "E3", qcTableBundle.qcMatrix);
            CommonBll.InsertContentInCells(ws, "G2", qcTableBundle.analysisDate);
            CommonBll.InsertContentInCells(ws, "G3", qcTableBundle.sampleCode);


            List<Aspose.Cells.Cell> cellStyleSetList = new List<Aspose.Cells.Cell>();
            var list = qcTableBundle.qcRowList;
            int rowIndex = 4;
            for (int i = 0; i < list.Count; i++)
            {
                Row row = ws.Cells.Rows[rowIndex];
                var listItem = list[i];
                row[0].Value = listItem.analyteName;
                row[1].Value = listItem.sampleResult;
                row[2].Value = listItem.unit;
                row[3].Value = listItem.much;
                row[4].Value = listItem.measured;
                row[5].Value = listItem.recoveryRate;
                row[6].Value = listItem.controlRange;

                cellStyleSetList.Add(row[0]);
                cellStyleSetList.Add(row[1]);
                cellStyleSetList.Add(row[2]);
                cellStyleSetList.Add(row[3]);
                cellStyleSetList.Add(row[4]);
                cellStyleSetList.Add(row[5]);
                cellStyleSetList.Add(row[6]);

                rowIndex++;
            }

            CommonBll.SetCellsStyle(cellStyleSetList);

            return ws;
        }

        /// <summary>
        /// 有机平行样
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="srModel"></param>
        /// <returns></returns>
        public static Worksheet Organic_Parallel(Workbook wb, SiteSurveyReportModel srModel, QCTableBundle qcTableBundle)
        {
            Worksheet ws = wb.Worksheets[12];
            CommonBll.InsertContentInCells(ws, "B1", "受检单位/项目名称:" + srModel.inspectedUnitName);
            CommonBll.InsertContentInCells(ws, "G1", srModel.orderName);
            CommonBll.InsertContentInCells(ws, "G2", qcTableBundle.analysisDate);
            CommonBll.InsertContentInCells(ws, "E3", qcTableBundle.qcMatrix);
            CommonBll.InsertContentInCells(ws, "G3", qcTableBundle.sampleCode);

            List<Aspose.Cells.Cell> cellStyleSetList = new List<Aspose.Cells.Cell>();
            var list = qcTableBundle.qcRowList;
            int rowIndex = 4;
            for (int i = 0; i < list.Count; i++)
            {
                Row row = ws.Cells.Rows[rowIndex];
                var listItem = list[i];
                row[0].Value = listItem.analyteName;
                row[1].Value = listItem.sampleResult;
                row[2].Value = listItem.parallelResults;
                row[3].Value = listItem.unit;
                row[4].Value = listItem.relativeDeviation;
                row[5].Value = listItem.rdControlRange;
                ws.Cells.Merge(rowIndex, 5, 1, 2);



                cellStyleSetList.Add(row[0]);
                cellStyleSetList.Add(row[1]);
                cellStyleSetList.Add(row[2]);
                cellStyleSetList.Add(row[3]);
                cellStyleSetList.Add(row[4]);
                cellStyleSetList.Add(row[5]);
                cellStyleSetList.Add(row[6]);

                rowIndex++;
            }

            CommonBll.SetCellsStyle(cellStyleSetList);

            return ws;
        }

    }
}
