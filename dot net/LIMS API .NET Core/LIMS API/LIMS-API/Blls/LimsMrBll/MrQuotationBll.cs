using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using LIMS_API.Bll;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models.LimsMRModels;

namespace LIMS_API.Blls.LimsMRBll
{
    /// <summary>
    /// MrQuotationBll
    /// </summary>
    public class MrQuotationBll
    {
        /// <summary>
        /// CreateMrQuotation
        /// </summary>
        /// <param name="quotationModel"></param>
        /// <returns></returns>
        public static string CreateMrQuotation(MrQuotationModel quotationModel)
        {
            PathManagementBll pathManagement = new PathManagementBll(quotationModel.organizationName, quotationModel.organizationId, "LIMS-MR-TEMP");
            string temp = pathManagement.GetTemplatePath("QuotationTemplate.doc");
            Document doc = new Document(temp);
            ReportingEngine engine = new ReportingEngine();
            engine.BuildReport(doc, quotationModel, "q");

            string fileType = "Doc";
            string savePath = CommonBll.CreateSaveFilePath("Quotation_" + quotationModel.quotationNo, fileType, quotationModel.organizationName, quotationModel.organizationId);
            //set the font is Simsun
            FontSettings fontSettings = new FontSettings();
            fontSettings.SubstitutionSettings.TableSubstitution.SetSubstitutes("SimSun");
            doc.FontSettings = fontSettings;
            doc.Save(savePath, CommonBll.GetSaveFormat(fileType));
            return savePath;
        }
    }
}
