using LIMS_API.Models.LimsMrModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspose.BarCode;
using Aspose.BarCode.Generation;
using Aspose.Pdf.Forms;
using Aspose.Words;
using Aspose.Words.Reporting;
using Aspose.Words.Tables;
using LIMS_API.Blls.CommonBlls;

namespace LIMS_API.Blls.LimsMrBll
{
    /// <summary>
    /// MrLaboratoryJobBll
    /// </summary>
    public class MrLaboratoryJobBll
    {
        /// <summary>
        /// CreateLaboratoryJob
        /// </summary>
        /// <param name="lModel"></param>
        /// <returns></returns>
        public static string CreateLaboratoryJob(MrLaboratoryJobModel labModel)
        {
            try
            {

                //Get Template
                PathManagementBll pathManagement = new PathManagementBll(labModel.organizationName, labModel.organizationId, "LIMS-MR-TEMP");
                string temp = pathManagement.GetTemplatePath("LaboratoryJobTemplate.doc");
                Document doc = new Document(temp);



                ReportingEngine engine = new ReportingEngine();
                engine.BuildReport(doc, labModel, "l");

                //1.Create Order Name BarCode
                //2.Insert Barcode
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.MoveToBookmark("OrderNameBarCode");
                builder.InsertImage(CreateBarCode(labModel.orderName));

                //读取表格
                Table sampleInfoTable = (Table)doc.GetChild(NodeType.Table, 2, true);
                for (int i = 1; i < sampleInfoTable.Rows.Count; i++)
                {
                    Cell cell = sampleInfoTable.Rows[i].Cells[0];
                    string sampleCode = cell.GetText();
                    if (sampleCode.EndsWith('\a'))
                    {
                        sampleCode = sampleCode.TrimEnd('\a');
                    }
                    
                    DocumentBuilder tableImageBuilder = new DocumentBuilder(doc);
                    tableImageBuilder.MoveToCell(2,i,0,0);
                    tableImageBuilder.InsertImage(CreateBarCode(sampleCode));
                }
                

                //ReplaceTheUnit
                FormulaHelper formulaHelper = new FormulaHelper(doc);
                formulaHelper.ReplaceTheFormula();

                string fileType = "Pdf";
                string savePath = CommonBll.CreateSaveFilePath("LabJob_", fileType, labModel.organizationName, labModel.organizationId);
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
        /// CreateBarCode
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static MemoryStream CreateBarCode(string code)
        {
            BarCodeBuilder barCodeBuilder = new BarCodeBuilder(code);
            MemoryStream imgStream = new MemoryStream();
            barCodeBuilder.Save(imgStream, ImageFormat.Png);
            return imgStream;
        }
    }
}
