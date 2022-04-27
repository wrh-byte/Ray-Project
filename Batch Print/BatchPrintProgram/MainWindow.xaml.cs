using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows;
using WorldPrint;
using System.Net;
using System.Printing;
using System.Threading;
using System.Windows.Forms;
using BatchPrintProgram.Model;
using Application = System.Windows.Forms.Application;
using ListBox = System.Windows.Controls.ListBox;
using MessageBox = System.Windows.MessageBox;

namespace BatchPrintProgram
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //全局变量
        //自定义文件夹路径
        private string folderPath;
        //打印队列
        public static List<PrintModel> printList;
        //完成队列
        public static List<FinishModel> finishList;
        static AutoResetEvent myResetEvent = new AutoResetEvent(false);
        bool pause = false;
        bool stop = false;
        private INIConfig ini = null;
        private List<string> printerList = null;
        //刷新UI事件定义
        public delegate void RefreshUIHandler(string listName);
        public static event RefreshUIHandler RefreshUIEvent;


        public MainWindow()
        {
            InitializeComponent();
            try
            {
                //绑定刷新UI事件
                RefreshUIEvent+=new RefreshUIHandler(RefreshUIList);

                string inipath = Application.UserAppDataPath + "\\Config.ini";
                ini = new INIConfig(inipath);
                ini.CreateConfig();

                //获取全部打印机
                printerList = PrintHelper.GetAllPrinter();
                foreach (var item in printerList)
                {
                    this.PrintercomboBox.Items.Add(item);
                }
                //配置License
                SetAsposeLicense();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                LogManager.WriteLog(LogManager.Severity.Error, exception.Message);
            }
        }
        /// <summary>
        /// 窗体完成加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //初始化队列
                finishList = new List<FinishModel>();
                printList = new List<PrintModel>();
                //获取ini中打印机设置并填充到combox
                GetInIPrinterSetting();

                //窗口初始化获取传入参数
                string[] pargs = Environment.GetCommandLineArgs();
                if (pargs.Length == 2 && pargs[1].Contains("arg"))//浏览器启动
                {
                    string[] par = pargs[1].Split('&');
                    Global.fileCode = par[1].Split('=')[1];//获取filecode
                    Global.oauthUrl = par[2].Split('=')[1];//获取OAuth2验证URL
                    Global.getDateUrl = par[3].Split('=')[1];//获取数据接口URL

                    //验证并获取downloadList
                    OAuthAndGetDownList();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                LogManager.WriteLog(LogManager.Severity.Error, exception.Message);
            }

        }
        /// <summary>
        /// 选择文件夹点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void choosefilebtn_Click(object sender, RoutedEventArgs e)
        {
            //获取文件夹路径
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowDialog();//打开文件夹会话
            string printfolderPath = folder.SelectedPath;

            int isCanResult = IsCanPrint(printfolderPath);
            if (isCanResult == 0)
            {
                //符合规格
                folderPath = printfolderPath;
                UpdateListBoxFile(listBoxFile, folderPath);//将文件加载到listbox
            }
            else if (isCanResult == 1)
            {
                MessageBox.Show("文件夹中不能有除doc/docx或jpg/png/jpge或pdf格式的其他文件！","Error");
            }
            else if (isCanResult == 3)
            {
                MessageBox.Show("文件夹为空！","Error");
            }
        }

        /// <summary>
        /// 检查文件夹中打印文件格式
        /// </summary>
        /// <returns></returns>
        private int IsCanPrint(string foldPath)
        {
            if (!string.IsNullOrEmpty(foldPath))
            {
                int result = 0;
                DirectoryInfo mydir = new DirectoryInfo(foldPath);
                FileSystemInfo[] fsis = mydir.GetFileSystemInfos();
                if (fsis.Count() <= 0)
                {
                    return 3;
                }
                foreach (FileSystemInfo fsi in fsis)
                {
                    if (fsi is FileInfo)
                    {

                        string[] fileNamearr = fsi.Name.Split('.');
                        string fileType = fileNamearr.LastOrDefault().ToLower();
                        if ((fileType != "doc" && fileType != "docx") && (fileType != "jpg" && fileType != "png" && fileType != "jpge") && (fileType != "pdf"))
                        {
                            result = 1;
                            break;
                        }
                    }
                }
                return result;
            }
            else
            {
                return 2;//文件夹路径为空
            }
        }

        /// <summary>
        /// 把文件加载到ListBox
        /// </summary>
        private void UpdateListBoxFile(ListBox listBox, string filePath)
        {
            listBox.Items.Clear();
            DirectoryInfo mydir = new DirectoryInfo(filePath);
            foreach (FileSystemInfo fsi in mydir.GetFileSystemInfos())
            {
                if (fsi is FileInfo)
                {
                    FileInfo fi = (FileInfo)fsi;
                    listBoxFile.Items.Add(fi.Name);
                }
            }
        }

        /// <summary>
        /// 开始打印按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Startbtn_Click(object sender, RoutedEventArgs e)
        {
            string printer = PrintercomboBox.Text;

            if (printList.Count > 0)
            {
                ColorcomboBox.IsEnabled = false;
                TypecomboBox.IsEnabled = false;
                PrintercomboBox.IsEnabled = false;
                DirectioncomboBox.IsEnabled = false;

                stop = false;
                //设置开始打印按钮不可用
                this.Startbtn.IsEnabled = false;
                this.Pausebtn.IsEnabled = true;
                this.Stopbtn.IsEnabled = true;
                //打印线程
                Thread prinThread = new Thread(new ParameterizedThreadStart(StartPrint));
                prinThread.SetApartmentState(ApartmentState.STA);
                prinThread.IsBackground = true;
                prinThread.Start(printer);

                //打印状态线程
                Thread sThread = new Thread(new ParameterizedThreadStart(RefreshStatus));
                sThread.SetApartmentState(ApartmentState.STA);
                sThread.IsBackground = true;
                sThread.Start(printer);

            }
            else
            {
                MessageBox.Show("打印队列为空", "Error");
            }
        }

        /// <summary>
        /// 打印机状态更新
        /// </summary>
        /// <param name="printer"></param>
        public void RefreshStatus(object printer)
        {
            while (true)
            {
                Thread.Sleep(500);
                var status = PrintHelper.GetPrinterStatus(printer.ToString());
                statuslabel.Dispatcher.Invoke(new Action(() =>
                {
                    statuslabel.Content = status; }));
            }
        }

        /// <summary>
        /// 暂停打印按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pausebtn_Click(object sender, RoutedEventArgs e)
        {
            pause = true;
            this.Pausebtn.IsEnabled = false;
            this.Continuebtn.IsEnabled = true;
        }

        /// <summary>
        /// 停止打印按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stopbtn_Click(object sender, RoutedEventArgs e)
        {

            ColorcomboBox.IsEnabled = true;
            TypecomboBox.IsEnabled = true;
            PrintercomboBox.IsEnabled = true;
            DirectioncomboBox.IsEnabled = true;

            stop = true;
            this.Stopbtn.IsEnabled = false;
            this.Startbtn.IsEnabled = true;
            this.Continuebtn.IsEnabled = false;
        }

        /// <summary>
        /// 继续打印按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Continuebtn_Click(object sender, RoutedEventArgs e)
        {
            pause = false;
            myResetEvent.Set();
            this.Continuebtn.IsEnabled = false;
            this.Pausebtn.IsEnabled = true;
        }

        /// <summary>
        /// 开始打印
        /// </summary>
        public void StartPrint(object printer)
        {
            try
            {
                if (printList.Count > 0)
                {
                A: while (true)
                    {
                        if (stop)//停止打印
                        {
                            return;
                        }
                        if (pause)//暂停打印
                        {
                            myResetEvent.WaitOne();
                        }
                        if (printList.Count > 0)
                        {
                            printList[0].status = "正在打印";
                            RefreshUIEvent("PrintList");
                            //开始打印
                            LogManager.WriteLog(LogManager.Severity.Trace,"打印文件路径:"+printList[0].filepath+"   打印份数:"+printList[0].copies+"   选择打印机:"+printer);

                            //Get PrinterSettingModel
                            PrinterSettingModel PrinterSettingModel = GetAttributeSetPrinter();
                            //Print
                            PrintHelper.PrintPdfAndWord(printList[0], printer.ToString(), PrinterSettingModel);
                            //刷新队列
                            RefreshUIEvent("PrintList");
                            RefreshUIEvent("FinishList");
                        }
                        else
                        {
                            while (true)
                            {
                                Thread.Sleep(1000);
                                if (stop)//停止打印
                                {
                                    return;
                                }
                                if (pause)//暂停打印
                                {
                                    myResetEvent.WaitOne();
                                }
                                if (printList.Count > 0)
                                {
                                    goto A;
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("打印队列为空", "Error");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                LogManager.WriteLog(LogManager.Severity.Error, $"StartPrint: {exception.Message}");
                throw;
            }
        }

        /// <summary>
        /// 打印成功
        /// </summary>
        /// <param name="filepath"></param>
        public static void PrintSuccess(PrintModel printModel)
        {
            //从打印队列移除
            printList.Remove(printModel);
            //加入完成列表
            finishList.Add(new FinishModel
            {
                filepath = printModel.filepath,
                fileName = printModel.fileName,
                order = printModel.order,
                status = "打印完成"
            });
        }


        /// <summary>
        /// 获取界面打印机设置并设置打印机
        /// </summary>
        public PrinterSettingModel GetAttributeSetPrinter()
        {
            //设置打印机
            string colorMode = "";
            string printType = "";
            string direction = "";
            ColorcomboBox.Dispatcher.Invoke(new Action(() => { colorMode = ColorcomboBox.Text; }));
            TypecomboBox.Dispatcher.Invoke(new Action(() => { printType = TypecomboBox.Text; }));
            DirectioncomboBox.Dispatcher.Invoke(new Action(() => { direction = DirectioncomboBox.Text; }));
            PrinterSettingModel printerSettingModel=new PrinterSettingModel();
            if (!string.IsNullOrEmpty(colorMode))//颜色模式
            {
                if (colorMode == "彩色打印")
                {
                    printerSettingModel.colorModel = true;
                }
                else if (colorMode == "单色打印")
                {
                    printerSettingModel.colorModel = false;
                }
            }
            if (!string.IsNullOrEmpty(printType))
            {
                if (printType == "单面打印")
                {
                    printerSettingModel.printType = Duplex.Simplex;
                }
                else if (printType == "双面打印")
                {
                    printerSettingModel.printType = Duplex.Default;
                }
            }
            if (!string.IsNullOrEmpty(direction))
            {
                if (direction == "横向打印")
                {
                    printerSettingModel.direction = true;
                }
                else if (direction == "纵向打印")
                {
                    printerSettingModel.direction = false;
                }
            }

            return printerSettingModel;

        }

        /// <summary>
        /// 添加到打印队列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPrintListbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintHelper printHelper = new PrintHelper();
                //第一次添加
                if (printList.Count==0)
                {
                    printList = printHelper.AddPrintList(folderPath);
                }
                else
                {
                    //再次添加文件夹将打印队列合并
                    printList = printList.Concat(printHelper.AddPrintList(folderPath)).ToList();
                }
                //刷新打印队列
                RefreshUIEvent("PrintList");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                LogManager.WriteLog(LogManager.Severity.Error, exception.Message);
                throw;
            }

        }

        /// <summary>
        /// 进行OAuth2验证并获取下载队列
        /// </summary>
        public void OAuthAndGetDownList()
        {
            Authorization authorization = new Authorization();
            //设置下载文件夹地址
            Global.downloadPath = Application.UserAppDataPath + "\\Download List";
            //判断下载保存文件夹是否存在
            if (!Directory.Exists(Global.downloadPath))
            {
                Directory.CreateDirectory(Global.downloadPath);//若文件夹不存在创建新路径
            }
            ////监测网络状态
            if (!Authorization.IsConnectInternet())
            {
                MessageBox.Show("未连接到互联网，请稍后再试......", "Error");
                return;
            }
            if (string.IsNullOrEmpty(Global.token))//未获取Token
            {
                BrowserForm browserForm = new BrowserForm();
                browserForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                browserForm.Topmost = true;
                //打开新窗口进行OAuth2验证
                browserForm.ShowDialog();
            }
            LogManager.WriteLog(LogManager.Severity.Trace, "token:"+Global.token +"  "+ "getDateUrl:" + Global.getDateUrl+"  "+"fileCode:" + Global.fileCode);

            //根据Token与filecode取Json
            if (!string.IsNullOrEmpty(Global.getDateUrl))
            {
                if (!string.IsNullOrEmpty(Global.fileCode))
                {
                    if (!string.IsNullOrEmpty(Global.token))
                    {
                        Global.downloadJson = authorization.GetDownloadJson(Global.getDateUrl, Global.token, Global.fileCode);
                    }
                    else
                    {
                        MessageBox.Show("Token为空", "Error");
                        LogManager.WriteLog(LogManager.Severity.Error, "Token为空 token:"+Global.token);
                    }
                }
                else
                {
                    MessageBox.Show("文件码为空", "Error");
                    LogManager.WriteLog(LogManager.Severity.Error, "文件码为空 fileCode:"+Global.fileCode);
                }
            }
            else
            {
                MessageBox.Show("提取数据地址为空", "Error");
                LogManager.WriteLog(LogManager.Severity.Error, "提取数据地址为空 getDateUrl:" + Global.getDateUrl);
            }
            LogManager.WriteLog(LogManager.Severity.Debug, "downloadJson:" + Global.downloadJson);
            //判断是否获取了下载地址
            if (!string.IsNullOrEmpty(Global.downloadJson))
            {
                if (Global.downloadJson == "[]")
                {
                    MessageBox.Show("获取文件key失效");
                    return;
                }
                //Json对象转换,添加入下载队列
                Global.downloadList = Global.downloadList.Concat(Global.downloadList = JsonHelper.DeserializeJsonToList<DownloadModel>(Global.downloadJson)).ToList();
                RefreshUIEvent("DownLoadList");
            }
            else
            {
                MessageBox.Show("未获取下载地址","Error");
            }

        }

        /// <summary>
        /// 开始下载按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Startdownloadbtn_Click(object sender, RoutedEventArgs e)
        {
            //开启下载线程
            Thread downloadThread = new Thread(Download);
            downloadThread.SetApartmentState(ApartmentState.STA);
            downloadThread.IsBackground = true;
            downloadThread.Start();
        }

        /// <summary>
        /// 遍历下载列表
        /// </summary>
        public void Download()
        {
            try
            {
                do
                {
                    //下载并添加入打印队列
                    StartDownLoad(Global.downloadList[0]);
                    //刷新打印队列UI
                    RefreshUIEvent("PrintList");
                    //刷新下载队列UI
                    RefreshUIEvent("DownLoadList");
                } while (Global.downloadList.Count>0);
                //下载完毕清空下载队列与下载json
                Global.downloadJson = "";
            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogManager.Severity.Error, exception.Message);
            }
        }

        /// <summary>
        /// HTTP下载远程文件并保存本地的函数
        /// </summary>
        /// <param name="url">下载链接</param>
        public void StartDownLoad(DownloadModel downloadModel)
        {
            try
            {
                var fileName = downloadModel.fileName;
                WebClient client = new WebClient();
                //异步下载
                //下载进度改变事件
                //client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                //下载完成事件
                //client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFile(new Uri(downloadModel.fileUrl), Global.downloadPath + "\\" + fileName);
                DownLoadSuccess(downloadModel);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "文件:" + downloadModel.fileName + "下载遇到问题");
                LogManager.WriteLog(LogManager.Severity.Error, "fileName:"+ downloadModel.fileName+"   downloadurl:"+downloadModel.fileUrl+ "   ErroInfo:"+ exception.Message);
                throw;
            }
        }

        /// <summary>
        /// 下载完成
        /// </summary>
        /// <param name="downloadModel"></param>
        public void DownLoadSuccess(DownloadModel downloadModel)
        {
            //下载完成加入打印队列
            printList.Add(new PrintModel
            {
                fileName = downloadModel.fileName,
                filepath = Global.downloadPath + "\\" + downloadModel.fileName,
                status = "准备打印",
                copies = downloadModel.copies
            });
            //从下载队列移除
            Global.downloadList.Remove(downloadModel);
        }

        /// <summary>
        /// 清空下载队列按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CleanBtn_Click(object sender, RoutedEventArgs e)
        {
            Global.downloadList.Clear();
            if (Global.downloadList.Count == 0)
            {
                //清空下载列表
                listBoxdownfile.Items.Clear();
            }
        }

        /// <summary>
        /// 刷新队列UI
        /// </summary>
        public void RefreshUIList(string listName)
        {
            if (listName=="DownLoadList")
            {
                listBoxdownfile.Dispatcher.Invoke(new Action(() => { listBoxdownfile.Items.Clear(); }));
                if (Global.downloadList.Count > 0)
                {
                    for (int i = 0; i < Global.downloadList.Count; i++)
                    {
                        var item = Global.downloadList[i];
                        listBoxdownfile.Dispatcher.Invoke(new Action(() => { listBoxdownfile.Items.Add(item.fileName); }));
                    }
                }
            }
            else if (listName == "PrintList")
            {
                PrintListView.Dispatcher.Invoke(new Action(()=>{ PrintListView.Items.Clear(); }));
                for (int i = 0; i < printList.Count; i++)
                {
                    printList[i].order = i+1;
                    PrintListView.Dispatcher.Invoke(new Action(() => { PrintListView.Items.Add(printList[i]); }));
                }
            }
            else if(listName=="FinishList")
            {
                FinishlistView.Dispatcher.Invoke(new Action(() => { FinishlistView.Items.Clear(); }));
                for (int i = 0; i < finishList.Count; i++)
                {
                    finishList[i].order = i + 1;
                    FinishlistView.Dispatcher.Invoke(new Action(() => { FinishlistView.Items.Add(finishList[i]); }));
                }
            }

        }


        /// <summary>
        /// 清空打印队列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CleanPLBtn_Click(object sender, RoutedEventArgs e)
        {
            printList.Clear();
            PrintListView.Items.Clear();
        }

        /// <summary>
        /// 打印机选择后存储
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintercomboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            object obj = (object) e.AddedItems;
            string printerName = (((object[])(obj))[0]).ToString();
            ini.WriteToInI("Printer", "printerName",printerName);
        }

        /// <summary>
        /// 颜色模式存储
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorcomboBox_DropDownClosed(object sender, EventArgs e)
        {
            string colorMode = ColorcomboBox.Text;
            ini.WriteToInI("Printer", "colorMode", colorMode);
        }

        /// <summary>
        /// 打印类型存储
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TypecomboBox_DropDownClosed(object sender, EventArgs e)
        {
            string printType = TypecomboBox.Text;
            ini.WriteToInI("Printer", "printType", printType);
        }

        /// <summary>
        /// 打印方向存储
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DirectioncomboBox_DropDownClosed(object sender, EventArgs e)
        {
            string printDir = DirectioncomboBox.Text;
            ini.WriteToInI("Printer", "printDir", printDir);
        }


        /// <summary>
        /// 获取ini文件中打印机所有设置并填充到combox下拉框
        /// </summary>
        public void GetInIPrinterSetting()
        {
            string printerName = ini.GetInIValue("Printer", "printerName");
            string colorMode = ini.GetInIValue("Printer", "colorMode");
            string printType = ini.GetInIValue("Printer", "printType");
            string printDir = ini.GetInIValue("Printer", "printDir");
            if (printerList.Contains(printerName))
            {
                PrintercomboBox.Text = printerName;
            }
            ColorcomboBox.Text = colorMode;
            TypecomboBox.Text = printType;
            DirectioncomboBox.Text = printDir;
        }

        public void SetAsposeLicense()
        {
            //配置Aspose
            string license = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Aspose.Total.lic";
            Aspose.Pdf.License license1 = new Aspose.Pdf.License();
            license1.SetLicense(license);
            Aspose.Words.License license2 = new Aspose.Words.License();
            license2.SetLicense(license);
        }

        private void DeleteFromPrintListEvent(object sender, RoutedEventArgs e)
        {
            int index = this.PrintListView.SelectedIndex;
            printList.RemoveAt(index);
            RefreshUIEvent("PrintList");
        }

        private void DeleteFromDownLoadListEvent(object sender, RoutedEventArgs e)
        {
            int index = this.listBoxdownfile.SelectedIndex;
            Global.downloadList.RemoveAt(index);
            RefreshUIEvent("DownLoadList");
        }
    }
}
