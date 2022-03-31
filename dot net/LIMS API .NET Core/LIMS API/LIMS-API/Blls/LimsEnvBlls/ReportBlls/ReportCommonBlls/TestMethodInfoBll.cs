using System;
using System.Reflection;
using Aspose.Words;
using Aspose.Words.Reporting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Blls.ReportBlls.ReportCommonBlls
{
    /// <summary>
    /// 报告测试方法仪器信息类
    /// </summary>
    public class TestMethodInfoBll
    {
        private static string RunPath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath;
        //file Path
        private static string tempPath = RunPath + "/Template/LIMS-ENV-TEMP/UniversalTemplate/ReportTemplate/";
        private static string testMethodTempPath = tempPath + "TestMethodInfoTempate/TestMethods.doc";
        private static string testMethodWithSubTempPath = tempPath + "TestMethodInfoTempate/TestMethodsWithSubcontract.doc";

        public Document BuildTestMethodInfo<T>(T entity)
        {
            object result = null;
            Type entiType = typeof(T);
            try
            {
                PropertyInfo propertyInfo = entiType.GetProperty("subRemark");
                result = propertyInfo.GetValue(entity);

                string tempPath;
                if (string.IsNullOrEmpty(result.ToString()))
                {
                    tempPath = testMethodTempPath;
                }
                else
                {
                    tempPath = testMethodWithSubTempPath;
                }
                Document doc = new Document(tempPath);
                ReportingEngine engine = new ReportingEngine();
                engine.BuildReport(doc, entity, "r");

                return doc;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        } 
    }
}
