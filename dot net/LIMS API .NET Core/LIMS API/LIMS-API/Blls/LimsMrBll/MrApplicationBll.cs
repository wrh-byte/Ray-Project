using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Fields;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models.LimsMrModels;
using LIMS_API.Models.LimsMRModels;

namespace LIMS_API.Blls.LimsMrBll
{
    /// <summary>
    /// MrApplicationBll
    /// </summary>
    public class MrApplicationBll
    {
        /// <summary>
        /// CreateApplication
        /// </summary>
        /// <param name="applicationModel"></param>
        /// <returns></returns>
        public static string CreateApplication(MrApplicationModel applicationModel)
        {
            PathManagementBll pathManagement = new PathManagementBll(applicationModel.organizationName, applicationModel.organizationId, "LIMS-MR-TEMP");
            string temp = pathManagement.GetTemplatePath("ApplicationTemplate.doc");
            
            // DOC
            Document doc = new Document(temp);
            
            // ENGINE
            ReportingEngine engine = new ReportingEngine();
            engine.BuildReport(doc, applicationModel, "a");

            //Create Check Box
            string[] checkBoxNames = 
            {
                "cb_normal","cb_urgent","cb_extraUrgent","cb_chinese"
                ,"cb_english","cb_cande","cb_each","cb_eachApp","cb_eachSample"
                ,"cb_multiple","cb_cnas","cb_electronic","cb_paper","cb_sameToApp"
                ,"cb_sameToTitle","cb_otherAddress","cb_specialInvoice","cb_generalInvoice"
            };

            CommonBll.CreateCheckBox(doc,checkBoxNames);

            //Select Check Box
            if (applicationModel.serviceType!=null)
            {
                if (applicationModel.serviceType == "标准")
                {
                    FormField formField = doc.Range.FormFields["cb_normal"];
                    formField.Checked = true;
                }
                else if(applicationModel.serviceType == "加急")
                {
                    FormField formField = doc.Range.FormFields["cb_urgent"];
                    formField.Checked = true;
                }
                else if (applicationModel.serviceType == "特急")
                {
                    FormField formField = doc.Range.FormFields["cb_extraUrgent"];
                    formField.Checked = true;
                }
            }

            if (applicationModel.reportLanguage!=null)
            {
                if (applicationModel.reportLanguage == "中文")
                {
                    FormField formField = doc.Range.FormFields["cb_chinese"];
                    formField.Checked = true;
                }
                else if (applicationModel.reportLanguage == "英文")
                {
                    FormField formField = doc.Range.FormFields["cb_english"];
                    formField.Checked = true;
                }
                else if (applicationModel.reportLanguage == "中英文对照")
                {
                    FormField formField = doc.Range.FormFields["cb_cande"];
                    formField.Checked = true;
                }
            }

            if (applicationModel.reportLayout!=null)
            {
                if (applicationModel.reportLayout == "按样品")
                {
                    FormField formField = doc.Range.FormFields["cb_each"];
                    formField.Checked = true;
                }
                else if (applicationModel.reportLayout == "按项目")
                {
                    FormField formField = doc.Range.FormFields["cb_multiple"];
                    formField.Checked = true;
                }
                else if (applicationModel.reportLayout == "按申请表")
                {
                    FormField formField = doc.Range.FormFields["cb_eachApp"];
                    formField.Checked = true;
                }
                else if (applicationModel.reportLayout == "按样品按项目")
                {
                    FormField formField = doc.Range.FormFields["cb_eachSample"];
                    formField.Checked = true;
                }
            }

            if (applicationModel.reportStampCNAS)
            {
                FormField formField = doc.Range.FormFields["cb_cnas"];
                formField.Checked = true;
            }

            if (applicationModel.reportForm!=null)
            {
                if (applicationModel.reportForm=="电子")
                {
                    FormField formField = doc.Range.FormFields["cb_electronic"];
                    formField.Checked = true;
                }
                else if (applicationModel.reportForm == "纸质")
                {
                    FormField formField = doc.Range.FormFields["cb_paper"];
                    formField.Checked = true;
                }
            }

            if (applicationModel.reportDAPick!=null)
            {
                if (applicationModel.reportDAPick == "同申请公司")
                {
                    FormField formField = doc.Range.FormFields["cb_sameToApp"];
                    formField.Checked = true;
                }
                else if (applicationModel.reportDAPick == "同报告抬头公司")
                {
                    FormField formField = doc.Range.FormFields["cb_sameToTitle"];
                    formField.Checked = true;
                }
                else if (applicationModel.reportDAPick == "其它")
                {
                    FormField formField = doc.Range.FormFields["cb_otherAddress"];
                    formField.Checked = true;
                }
            }

            if (applicationModel.invoiceType!=null)
            {
                if (applicationModel.invoiceType == "增值税专用发票")
                {
                    FormField formField = doc.Range.FormFields["cb_specialInvoice"];
                    formField.Checked = true;
                }
                else if (applicationModel.invoiceType == "增值税普通发票")
                {
                    FormField formField = doc.Range.FormFields["cb_generalInvoice"];
                    formField.Checked = true;
                }
            }


            string fileType = "Doc";
            string savePath = CommonBll.CreateSaveFilePath("Application_", fileType, applicationModel.organizationName, applicationModel.organizationId);
            //set the font is Simsun
            FontSettings fontSettings = new FontSettings();
            fontSettings.SubstitutionSettings.TableSubstitution.SetSubstitutes("SimSun");
            doc.FontSettings = fontSettings;
            doc.Save(savePath, CommonBll.GetSaveFormat(fileType));
            return savePath;
        }
    }
}
