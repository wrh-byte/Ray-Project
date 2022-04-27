using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BatchPrintProgram.Model;
using Microsoft.Shell;

namespace BatchPrintProgram
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application,ISingleInstanceApp
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (SingleInstance<App>.InitializeAsFirstInstance("1"))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();
                SingleInstance<App>.Cleanup();
            }
        }

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            //浏览器多次请求时获取参数
            if (args.Count > 0 && args[1].Contains("arg"))
            {
                string[] par = args[1].Split('&');
                Global.fileCode = par[1].Split('=')[1];
                Global.oauthUrl = par[2].Split('=')[1];
                Global.getDateUrl = par[3].Split('=')[1];

                MainWindow mw = new MainWindow();
                mw.OAuthAndGetDownList();
            }


            // handle command line arguments of second instance
            if (this.MainWindow.WindowState == WindowState.Minimized)
            {
                this.MainWindow.WindowState = WindowState.Normal;
            }
            this.MainWindow.Activate();

            return true;
        }
        #endregion
    }
}
