using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Aspose.Words;
using Aspose.Words.Reporting;
using Aspose.Words.Tables;
using LIMS_API.Blls.CommonBlls;
using LIMS_API.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Bll.LimsEnvBlls
{
    public class LaboratoryJobBll
    {
        /// <summary>
        /// CreateLaboratoryJobTemplate
        /// </summary>
        /// <param name="labModel"></param>
        /// <returns></returns>
        public static string CreateLaboratoryJobTemplate(LaboratoryJobModel labModel)
        {
            try
            {

                //Get Template
                PathManagementBll pathManagement = new PathManagementBll(labModel.organizationName,labModel.organizationId);
                string temp = pathManagement.GetTemplatePath("LaboratoryJobTemplate.doc");
                Document doc = new Document(temp);


                Dictionary<string, int> mapper = new Dictionary<string, int>();
                int i = 1;
                if (labModel.samplingTestList!=null&&labModel.samplingTestList.Count>0)
                {
                    foreach (var item in labModel.samplingTestList)
                    {
                        if (CommonBll.GetHanNumFromString(item.detectionProject) >= 20)
                        {
                            if (mapper.ContainsKey(item.detectionProject))
                            {
                                int no = 0;
                                mapper.TryGetValue(item.detectionProject, out no);
                                item.detectionProject = "见附表" + no;
                            }
                            else
                            {
                                mapper.Add(item.detectionProject, i);
                                item.detectionProject = "见附表" + i;
                                i++;
                            }
                        }
                    }
                }


                ReportingEngine engine = new ReportingEngine();
                engine.BuildReport(doc, labModel, "l");

                //Insert Schedule Table
                CommonBll.InsertSchedule(doc, mapper);

                //ReplaceTheUnit
                FormulaHelper formulaHelper = new FormulaHelper(doc);
                formulaHelper.ReplaceTheFormula();

                string fileType = "Doc";
                string savePath = CommonBll.CreateSaveFilePath("LabJob_" + labModel.orderCode, fileType, labModel.organizationName,labModel.organizationId);
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
