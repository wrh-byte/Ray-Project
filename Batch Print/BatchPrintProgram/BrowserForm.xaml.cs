using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Text;
using WorldPrint;

namespace BatchPrintProgram
{
    /// <summary>
    /// BrowserForm.xaml 的交互逻辑
    /// </summary>
    public partial class BrowserForm : Window
    {
        private Authorization a;
        public BrowserForm()
        {

            InitializeComponent();
            //设置为内核为IE11
            SetWebBrowserFeatures(11);
            //获取URL发送给浏览器
            a = new Authorization();
            webBrowser.Navigate(new Uri(a.CreatrUrl()));
        }
        /// <summary>
        /// 页面完成触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                //屏蔽脚本错误弹窗
                SuppressScriptErrors((System.Windows.Controls.WebBrowser)sender, true);

                //获取当前页面网页url(Navigated)
               string nowUrl = e.Uri.ToString();
                if (nowUrl.Contains("access_token"))
                {
                    //获取并保存token
                    Global.token = a.GetURLData(nowUrl, "access_token");
                    if (string.IsNullOrEmpty(Global.token))
                    {
                        System.Windows.Forms.MessageBox.Show("获取Token失败", "Error");
                    }
                    else
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogManager.Severity.Error, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// 屏蔽脚本错误
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="Hide"></param>
        public void SuppressScriptErrors(System.Windows.Controls.WebBrowser wb, bool Hide)
        {
            FieldInfo fiComWebBrowser = typeof(System.Windows.Controls.WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;

            object objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });
        }

        /// <summary>
        /// 设置内嵌WebBrowser版本
        /// </summary>
        /// <param name="ieVersion">版本号</param>
        public static void SetWebBrowserFeatures(int ieVersion)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
                return;
            //获取程序及名称  
            var appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //得到浏览器的模式的值  
            UInt32 ieMode = GeoEmulationModee(ieVersion);
            var featureControlRegKey = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\";
            //设置浏览器对应用程序（appName）以什么模式（ieMode）运行  
            Registry.SetValue(featureControlRegKey + "FEATURE_BROWSER_EMULATION",
                appName, ieMode, RegistryValueKind.DWord);
            // enable the features which are "On" for the full Internet Explorer browser              
            Registry.SetValue(featureControlRegKey + "FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION",
                appName, 1, RegistryValueKind.DWord);
        }

        /// <summary>  
        /// 通过版本得到浏览器模式的值  
        /// </summary>  
        /// <param name="browserVersion"></param>  
        /// <returns></returns>  
        static UInt32 GeoEmulationModee(int browserVersion)
        {
            UInt32 mode = 11000; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 Standards mode.   
            switch (browserVersion)
            {
                case 7:
                    mode = 7000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode.   
                    break;
                case 8:
                    mode = 8000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode.   
                    break;
                case 9:
                    mode = 9000; // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode.                      
                    break;
                case 10:
                    mode = 10000; // Internet Explorer 10.  
                    break;
                case 11:
                    mode = 11000; // Internet Explorer 11  
                    break;
            }
            return mode;
        }


    }
}
