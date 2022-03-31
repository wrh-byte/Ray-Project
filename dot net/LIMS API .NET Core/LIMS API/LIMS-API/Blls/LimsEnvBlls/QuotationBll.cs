using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Aspose.Words;
using Aspose.Words.Fields;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using Aspose.Words.Tables;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Document = Aspose.Words.Document;
using Table = Aspose.Words.Tables.Table;

namespace LIMS_API.Bll
{
    public class QuotationBll
    {
        public static string createQuotation(QuotationModel quotationModel)
        {
            //Get Template Path
            PathManagementBll pathManagement = new PathManagementBll(quotationModel.organizationName,quotationModel.organizationId);
            string temp = pathManagement.GetTemplatePath("QuotationTemplate.doc");

            //The Data Processing
            Dictionary<string, int> mapper = new Dictionary<string, int>();
            int i = 1;
            if (quotationModel.testingPlanList!=null&&quotationModel.testingPlanList.Count>0)
            {
                foreach (var item in quotationModel.testingPlanList)
                {
                    if (CommonBll.GetHanNumFromString(item.projectContent) >= 80)
                    {
                        if (mapper.ContainsKey(item.projectContent))
                        {
                            int no = 0;
                            mapper.TryGetValue(item.projectContent, out no);
                            item.projectContent = "见附表" + no;
                        }
                        else
                        {
                            mapper.Add(item.projectContent, i);
                            item.projectContent = "见附表" + i;
                            i++;
                        }
                    }
                }
            }

            if (quotationModel.subcontractList!=null&&quotationModel.subcontractList.Count>0)
            {
                foreach (var item in quotationModel.subcontractList)
                {
                    if (CommonBll.GetHanNumFromString(item.projectContent) >= 80)
                    {
                        if (mapper.ContainsKey(item.projectContent))
                        {
                            int no = 0;
                            mapper.TryGetValue(item.projectContent, out no);
                            item.projectContent = "见附表" + no;
                        }
                        else
                        {
                            mapper.Add(item.projectContent, i);
                            item.projectContent = "见附表" + i;
                            i++;
                        }
                    }
                }
            }

            if (quotationModel.testStandardList!=null&& quotationModel.testStandardList.Count>0)
            {
                foreach (var item in quotationModel.testStandardList)
                {
                    if (CommonBll.GetHanNumFromString(item.projectName) >= 80)
                    {
                        if (mapper.ContainsKey(item.projectName))
                        {
                            int no = 0;
                            mapper.TryGetValue(item.projectName, out no);
                            item.projectName = "见附表" + no;
                        }
                        else
                        {
                            mapper.Add(item.projectName, i);
                            item.projectName = "见附表" + i;
                            i++;
                        }
                    }
                }
            }


            Document doc = new Document(temp);


            //判断是否有分包信息并且，进行分包行的删除与保留
            if (quotationModel.subcontractList==null||quotationModel.subcontractList.Count==0)
            {
                Table quotationTable = (Table)doc.GetChild(NodeType.Table, 0, true);
                //行删除14 15 16 17
                quotationTable.Rows.RemoveAt(14);
                quotationTable.Rows.RemoveAt(14);
                quotationTable.Rows.RemoveAt(14);
                quotationTable.Rows.RemoveAt(14);
            }

            ReportingEngine engine = new ReportingEngine();
            engine.BuildReport(doc, quotationModel, "q");


            //Create checkbox
            string[] checkboxName =
            {
                    "chinese","CMA",
                    "selectedDetection","customerSpecified",
                    "destroyed","getBack","other",
                    "ordinaryInvoice","specializedInvoice"

                };
            CommonBll.CreateCheckBox(doc, checkboxName);
            if (quotationModel.reportLanguage != null)
            {
                if (quotationModel.reportLanguage.Contains("中文"))
                {
                    FormField formField = doc.Range.FormFields["chinese"];
                    formField.Checked = true;
                }
            }
            if (quotationModel.stampCMA)
            {
                FormField formField = doc.Range.FormFields["CMA"];
                formField.Checked = true;
            }
            if (quotationModel.resSampleTreatment != null)
            {
                if (quotationModel.resSampleTreatment.Contains("销毁"))
                {
                    FormField formField = doc.Range.FormFields["destroyed"];
                    formField.Checked = true;
                }
                else if (quotationModel.resSampleTreatment.Contains("自行取回"))
                {
                    FormField formField = doc.Range.FormFields["getBack"];
                    formField.Checked = true;
                }
                else if (quotationModel.resSampleTreatment.Contains("其他"))
                {
                    FormField formField = doc.Range.FormFields["other"];
                    formField.Checked = true;
                }
            }
            if (quotationModel.proTestMethod != null)
            {
                if (quotationModel.proTestMethod.Contains("授权")|| quotationModel.proTestMethod.Contains("宇相津准"))
                {
                    FormField formField = doc.Range.FormFields["selectedDetection"];
                    formField.Checked = true;
                }
                else if (quotationModel.proTestMethod.Contains("客户"))
                {
                    FormField formField = doc.Range.FormFields["customerSpecified"];
                    formField.Checked = true;
                }
            }
            if (quotationModel.invoiceType != null)
            {
                if (quotationModel.invoiceType.Contains("增值税普通发票"))
                {
                    FormField formField = doc.Range.FormFields["ordinaryInvoice"];
                    formField.Checked = true;
                }
                else if (quotationModel.invoiceType.Contains("增值税专用发票"))
                {
                    FormField formField = doc.Range.FormFields["specializedInvoice"];
                    formField.Checked = true;
                }
            }

            //插入附表
            CommonBll.InsertSchedule(doc, mapper);

            //ReplaceTheUnit
            FormulaHelper formulaHelper = new FormulaHelper(doc);
            formulaHelper.ReplaceTheFormula();

            string fileType = "Doc";
            string savePath = CommonBll.CreateSaveFilePath("Quotation_" + quotationModel.quotationCode, fileType, quotationModel.organizationName, quotationModel.organizationId);
            //set the font is Simsun
            FontSettings fontSettings = new FontSettings();
            fontSettings.SubstitutionSettings.TableSubstitution.SetSubstitutes("SimSun");
            doc.FontSettings = fontSettings;
            doc.Save(savePath, CommonBll.GetSaveFormat(fileType));
            return savePath;
        }
    }
}
