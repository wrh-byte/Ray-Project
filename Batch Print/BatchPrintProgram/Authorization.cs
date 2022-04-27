using System;
using System.Net;
using System.Runtime.InteropServices;
using RestSharp;
using WorldPrint;
using MessageBox = System.Windows.MessageBox;

namespace BatchPrintProgram
{
    public class Authorization
    {

        //获取TokenUrl测试
        //private string getTokenUrl = "https://test.salesforce.com/services/oauth2/authorize";
        //获取TokenUrl社区
        //private string getTokenUrl = "https://dev-ogcn.cs57.force.com/services/oauth2/authorize";
        //客户端Id
        private string client_id = "3MVG959Nd8JMmavQq8Ejqh0uP1CR_mPUM4nFn_SXPp8ofyoKHt6jlzX3MwY4e.GDiRex2tgFweY57MW4ap.wY";
        //回调地址
        private string redirect_uri = "https://127.0.0.1:8088/callback";
        //获取数据地址
        //private string getdownload_url = "https:%2F%2Fogc--uat.my.salesforce.com/services/apexrest/preorder/v2";


        //监测网络状态
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(int Description, int ReservedValue);

        /// <summary>
        /// 生成OAuth2验证URL
        /// </summary>
        /// <returns></returns>
        public string CreatrUrl()
        {
            //设置验证方式
            string response_type = "token";
            //获取token的链接和请求参数
            String url = Global.oauthUrl + "?" + "client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&response_type=" + response_type;
            return url;
        }

        /// <summary>
        /// 根据URL获取相应参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetURLData(string url,string name)
        {
            var lasturl = url.Replace('#', '?');
            string[] separateURL = lasturl.Split('?');
            System.Collections.Specialized.NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(separateURL[1]);
            var Str = queryString[name];
            return Str;
        }

        /// <summary>
        /// 根据token获取下载地址
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public string GetDownloadJson(string getDateUrl, string access_token,string fileToken)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient(getDateUrl);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + access_token);
                request.AddHeader("Content-Type", "text/plain");
                request.AddParameter("text/plain", fileToken, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                LogManager.WriteLog(LogManager.Severity.Trace, "responseStatusCode:" + response.StatusCode +"    "+ "responseContent:" + response.Content);
                return response.Content;
            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogManager.Severity.Error, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// 用于检查网络是否可以连接互联网,true表示连接成功,false表示连接失败
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectInternet()
        {
            int Description = 0;
            return InternetGetConnectedState(Description, 0);
        }
    }
}
