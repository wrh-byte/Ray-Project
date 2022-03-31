using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using Aspose.Words;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Fonts;
using Aspose.Words.Reporting;
using Aspose.Words.Tables;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SaveFormat = Aspose.Words.SaveFormat;

namespace LIMS_API.Bll
{
    public class SamplingWorkOrderBll
    {

        /// <summary>
        /// create SamplingWorkOrder
        /// </summary>
        /// <param name="swoModel"></param>
        public static string CreateSamplingWorkOrderTemplate(SamplingWordOrderModel swoModel)
        {
            try
            {
                //处理数据
                Dictionary<string, int> mapper = new Dictionary<string, int>();
                int i = 1;
                if (swoModel.samplingPlanList != null && swoModel.samplingPlanList.Count > 0)
                {
                    foreach (var item in swoModel.samplingPlanList)
                    {
                        if (CommonBll.GetHanNumFromString(item.project) >= 80)
                        {
                            if (mapper.ContainsKey(item.project))
                            {
                                int no = 0;
                                mapper.TryGetValue(item.project, out no);
                                item.project = "见附表" + no;
                            }
                            else
                            {
                                mapper.Add(item.project, i);
                                item.project = "见附表" + i;
                                i++;
                            }
                        }
                    }
                }

                //Get Template
                PathManagementBll pathManagement = new PathManagementBll(swoModel.organizationName,swoModel.organizationId);
                string temp = pathManagement.GetTemplatePath("SamplingJobTemplate/SamplingWorkOrderTemplate.doc");
                string planTemp = pathManagement.GetTemplatePath("SamplingJobTemplate/SamplingPlanTemplate.doc");
                Document doc = new Document(temp);

                ReportingEngine engine = new ReportingEngine();
                engine.BuildReport(doc, swoModel, "s");

                if (swoModel.samplingProgramDisplay)
                {
                    Document planDoc = new Document(planTemp);
                    engine.BuildReport(planDoc, swoModel, "p");
                    CommonBll.AppendDocument(doc, planDoc, ImportFormatMode.KeepSourceFormatting);
                }

                //插入附表
                CommonBll.InsertSchedule(doc, mapper);

                //ReplaceTheUnit
                FormulaHelper formulaHelper = new FormulaHelper(doc);
                formulaHelper.ReplaceTheFormula();

                string fileType = "doc";
                string savePath = CommonBll.CreateSaveFilePath("Sampling_"+swoModel.Code, fileType, swoModel.organizationName,swoModel.organizationId);
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