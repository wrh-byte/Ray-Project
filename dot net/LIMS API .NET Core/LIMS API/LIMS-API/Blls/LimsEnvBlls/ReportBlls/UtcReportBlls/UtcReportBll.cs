using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Blls.ReportBlls.ReportCommonBlls;
using LIMS_API.Models.LimsEnvModels.ReportModels.UtcReportModels;

namespace LIMS_API.Blls.LimsEnvBlls.ReportBlls.UtcReportBlls
{
    /// <summary>
    /// UtcReportBll
    /// </summary>
    public class UtcReportBll
    {
        /// <summary>
        /// CreateReport
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>
        public static string CreateReport(UtcReportModel reportModel)
        {
            string reportPath = "ReportTemplate/WaterSupplyReportTemplate";
            PathManagementBll pathManagement = new PathManagementBll(reportModel.organizationName, reportModel.organizationId);
            string reportTempPath = pathManagement.GetTemplatePath(reportPath + "/WaterSupplyReportTemp.doc");
            Document doc = new Document(reportTempPath);

            //报告生成日期
            reportModel.generateDate = DateTime.Now.ToString("yyyy年MM月dd日");

            ReportingEngine engine = new ReportingEngine();
            engine.BuildReport(doc, reportModel, "r");

            //Insert Sign Image
            ReportImageBll reportImageBll = new ReportImageBll();
            reportImageBll.InsertImgToDocument(reportModel, doc, "writerSignImage", ReportImageBll.ImageType.SignImage, null,reportModel.editor, 80, 30);

            FormulaHelper formulaHelper = new FormulaHelper(doc);
            formulaHelper.ReplaceTheFormula();

            string fileType = "Doc";
            string savePath = CommonBll.CreateDraftReportSavePath(reportModel.reportName, fileType,
                reportModel.organizationName, reportModel.organizationId);
            //.set the font is Simsun
            FontSettings fontSettings = new FontSettings();
            fontSettings.SubstitutionSettings.TableSubstitution.SetSubstitutes("SimSun");
            doc.FontSettings = fontSettings;
            doc.Save(savePath, CommonBll.GetSaveFormat(fileType));
            return savePath;
        }
    }
}
