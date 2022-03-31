using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BarTender;
using LabelPrint.Models;
using Newtonsoft.Json;
using RestSharp;
using Seagull.BarTender.Print;
using Sunny.UI;

namespace LabelPrint
{
    public class BartenderPrint
    {
        private static BarTender.Application btApp = new BarTender.Application();
        private static BarTender.Format btFormat = new BarTender.Format();


        /// <summary>
        /// 监测网络状态
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="ReservedValue"></param>
        /// <returns></returns>
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(int Description, int ReservedValue);

        /// <summary>
        /// 用于检查网络是否可以连接互联网,true表示连接成功,false表示连接失败
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectInternet()
        {
            int Description = 0;
            return InternetGetConnectedState(Description, 0);
        }

        /// <summary>
        /// Print
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tempPath"></param>
        /// <param name="usePrinter"></param>
        /// <param name="dataSourceList"></param>
        public static void Print(Data data, string tempPath, string usePrinter, List<string> dataSourceList)
        {
            //找到打印模板的标签页
            btFormat = btApp.Formats.Open(tempPath, false, usePrinter);
            //设置同序列的打印份数
            btFormat.PrintSetup.IdenticalCopiesOfLabel = 1;
            // 同样标签的份数
            btFormat.PrintSetup.IdenticalCopiesOfLabel = data.CopiesOfLabel;
            //设置需要打印的序列数
            //btFormat.PrintSetup.NumberSerializedLabels = 3;
            //向Bartender模板传递变量
            PropertyInfo[] propertyInfos = data.GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                string type = propertyInfo.PropertyType.FullName;
                if (type == "System.String"&&dataSourceList.Contains(propertyInfo.Name))
                {
                    btFormat.SetNamedSubStringValue(propertyInfo.Name, propertyInfo.GetValue(data).ToString());
                }
            }

            //第二个false设置打印时是否跳出打印属性
            btFormat.PrintOut(false, false);
            //退出是是否保存标签
            btFormat.Close(BtSaveOptions.btDoNotSaveChanges);
        }

        /// <summary>
        /// GetTempDownloadUrl
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public static List<string> GetTempDownloadUrl(string orgId, string orgName)
        {
            string requesuURL = "https://limsapi.eachcloud.co/api/LimsMr/GetBartenderLabelTemplateFilePath";
            var client = new RestClient(requesuURL);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{""organizationId"":""" + orgId + @""",""organizationName"":""" + orgName + @"""}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                if (response.Content != null)
                {
                    DownLoadResponseModel downLoadResponse =
                        JsonConvert.DeserializeObject<DownLoadResponseModel>(response.Content);
                    if (downLoadResponse != null && downLoadResponse.Success)
                    {
                        return downLoadResponse.DownLoadUrls;
                    }
                }
                else
                {
                    UIMessageBox.Show("下载模板链接Json请求返回为空无法转换", "警告", UIStyle.Gray);
                }

            }
            else
            {
                UIMessageBox.Show("拉取下载模板链接请求失败", "警告", UIStyle.Gray);
            }
            return null;
        }

        /// <summary>
        /// HttpDownloadFile
        /// </summary>
        /// <param name="url"></param>
        public static void HttpDownloadFile(string url)
        {
            WebClient client = new WebClient();
            string strFileName = url.Substring(url.LastIndexOf("=") + 1);
            string savePath = System.Windows.Forms.Application.UserAppDataPath + "Template";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            client.DownloadFile(new Uri(url), savePath + "\\" + strFileName);
        }
    }
}
