using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using LIMS_API.Bll;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models.PersonnelModels;

namespace LIMS_API.Blls.LimsEnvBlls.PersonnelBlls
{
    public class ArchivesBll
    {
        public static string createArchives(ArchivesModel arModel)
        {
            PathManagementBll pathManagement = new PathManagementBll(arModel.organizationName, arModel.organizationId);
            string temp = pathManagement.GetTemplatePath("ArchivesTemplate.doc");
            
            Document doc = new Document(temp);

            ReportingEngine engine = new ReportingEngine();
            engine.BuildReport(doc, arModel, "a");
            
            string fileType = "Doc";
            string savePath = CommonBll.CreateSaveFilePath("Archives_" + arModel.name, fileType, arModel.organizationName, arModel.organizationId);
            //set the font is Simsun
            FontSettings fontSettings = new FontSettings();
            fontSettings.SubstitutionSettings.TableSubstitution.SetSubstitutes("SimSun");
            doc.FontSettings = fontSettings;
            doc.Save(savePath, CommonBll.GetSaveFormat(fileType));
            return savePath;
        }
    }
}
