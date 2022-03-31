using System;
using System.Collections.Generic;
using Aspose.Words;
using Aspose.Words.Fonts;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Bll
{
    public class TestResultBll
    {

        public static string CreateTestResultTemplate(TestResultModel trModel)
        {
            try
            {
                //Get Template
                PathManagementBll pathManagement = new PathManagementBll(trModel.organizationName,trModel.organizationId);
                string temp = pathManagement.GetTemplatePath("TestResultTemplate.doc");
                
                Document doc = new Document(temp);

                CommonBll.FillBookmark(doc, trModel);

                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                builder.MoveToBookmark("testResult");

                List<TableModel.Row> testResultTable = trModel.testResultTable.table;
                String tableDescription = trModel.testResultTable.tableDescription;
                int rowCount = testResultTable.Count;
                int colCount = testResultTable[0].column.Count;


                builder.Writeln(tableDescription + "采样结果记录:");
                for (int i = 0; i < rowCount; i++)//row
                {
                    for (int j = 0; j < colCount; j++)//col
                    {
                        builder.InsertCell();
                        builder.Write(testResultTable[i].column[j] == null ? "" : testResultTable[i].column[j]);
                    }
                    builder.EndRow();
                }
                builder.EndTable();
                builder.Writeln("");

                string fileType = "Pdf";
                string savePath = CommonBll.CreateSaveFilePath("TestResult", fileType, trModel.organizationName,trModel.organizationId);
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
    }
}
