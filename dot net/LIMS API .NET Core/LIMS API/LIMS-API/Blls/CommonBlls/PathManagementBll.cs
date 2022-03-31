using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Blls.CommonBlls
{
    /// <summary>
    /// 
    /// </summary>
    public class PathManagementBll
    {
        private string orgPath { get; set; }
        private string universalPath { get; set; }

        /// <summary>
        /// PathManagementBll
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="orgId"></param>
        public PathManagementBll(string orgName,string orgId,string projectName= "LIMS-ENV-TEMP")
        {
            string EnvPath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath;
            orgPath = $"{EnvPath}/Template/{projectName}/{orgName}_{orgId}/";
            universalPath = $"{EnvPath}/Template/{projectName}/UniversalTemplate/";
        }

        public string GetTemplatePath(string followUpPath)
        {
            if (File.Exists(orgPath + followUpPath)|| Directory.Exists(orgPath + followUpPath))
            {
                return orgPath + followUpPath;
            }
            else
            {
                if (File.Exists(universalPath + followUpPath)|| Directory.Exists(universalPath + followUpPath))
                {
                    return universalPath + followUpPath;
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
