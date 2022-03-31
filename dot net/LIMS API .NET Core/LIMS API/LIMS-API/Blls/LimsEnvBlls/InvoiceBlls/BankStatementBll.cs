using System.IO;
using Aspose.Words;
using Aspose.Words.Fields;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Blls.LimsEnvBlls.InvoiceBlls
{
    /// <summary>
    /// Print Bank Statement
    /// </summary>
    public class BankStatementBll
    {
        /// <summary>
        /// createBankStatement
        /// </summary>
        /// <param name="bsModel"></param>
        /// <returns></returns>
        public static string createBankStatement(BankStatementModel bsModel)
        {
            PathManagementBll pathManagement = new PathManagementBll(bsModel.organizationName, bsModel.organizationId);
            string templatePath = pathManagement.GetTemplatePath("BankStatementTemplate.doc");

            //Read Document
            Document doc = new Document(templatePath);
            //Create Report Engine
            ReportingEngine engine = new ReportingEngine();
            engine.BuildReport(doc, bsModel, "s");

            //Create Checkbox
            string[] checkboxName = {"ordinaryInvoice","specializedInvoice"};
            CommonBll.CreateCheckBox(doc, checkboxName);
            if (bsModel.invoiceType != null)
            {
                if (bsModel.invoiceType.Contains("增值税普通发票"))
                {
                    FormField formField = doc.Range.FormFields["ordinaryInvoice"];
                    formField.Checked = true;
                }
                else if (bsModel.invoiceType.Contains("增值税专用发票"))
                {
                    FormField formField = doc.Range.FormFields["specializedInvoice"];
                    formField.Checked = true;
                }
            }

            string fileType = "Doc";
            string savePath = CommonBll.CreateSaveFilePath("BS_"+ bsModel.invoiceName, fileType, bsModel.organizationName, bsModel.organizationId);
            //set the font is Simsun
            FontSettings fontSettings = new FontSettings();
            fontSettings.SubstitutionSettings.TableSubstitution.SetSubstitutes("SimSun");
            doc.FontSettings = fontSettings;
            doc.Save(savePath, CommonBll.GetSaveFormat(fileType));
            return savePath;
        }
    }
}
