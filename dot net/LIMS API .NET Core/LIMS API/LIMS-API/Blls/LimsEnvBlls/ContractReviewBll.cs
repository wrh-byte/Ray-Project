using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Fields;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using Aspose.Words.Tables;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models.LimsEnvModels;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace LIMS_API.Blls.LimsEnvBlls
{
    /// <summary>
    /// 合同评审
    /// </summary>
    public class ContractReviewBll
    {
        /// <summary>
        /// createContractReview
        /// </summary>
        /// <param name="crModel"></param>
        /// <returns></returns>
        public static string createContractReview(ContractReviewModel crModel)
        {
            PathManagementBll pathManagement = new PathManagementBll(crModel.organizationName, crModel.organizationId);
            string temp = pathManagement.GetTemplatePath("ContractReviewTemplate.doc");
            Document doc = new Document(temp);

            ReportingEngine engine = new ReportingEngine();
            engine.BuildReport(doc, crModel, "c");


            string fileType = "Doc";
            string savePath = CommonBll.CreateSaveFilePath("ContractReview_" + DateTime.Now.ToString("yy-MM-dd"), fileType, crModel.organizationName, crModel.organizationId);
            //set the font is Simsun
            FontSettings fontSettings = new FontSettings();
            fontSettings.SubstitutionSettings.TableSubstitution.SetSubstitutes("SimSun");
            doc.FontSettings = fontSettings;
            doc.Save(savePath, CommonBll.GetSaveFormat(fileType));
            return savePath;
        }
    }
}
