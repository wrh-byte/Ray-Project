using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Printing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Application = Microsoft.Office.Interop.Word.Application;
using MessageBox = System.Windows.MessageBox;
using System.Collections.Specialized;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading;
using System.Security;
using BatchPrintProgram;
using BatchPrintProgram.Model;
using Aspose.Pdf;
using Aspose.Words;
using System.Drawing.Printing;
using Aspose.Pdf.Facades;
using SaveFormat = Aspose.Words.SaveFormat;

namespace WorldPrint
{
    class PrintHelper
    {
        /// <summary>
        /// 获取所有打印机
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllPrinter()
        {
            try
            {
                ManagementObjectCollection queryCollection;
                string _classname = "SELECT * FROM Win32_Printer";
                Dictionary<string, ManagementObject> dict = new Dictionary<string, ManagementObject>();
                ManagementObjectSearcher query = new ManagementObjectSearcher(_classname);
                queryCollection = query.Get();
                List<string> result = new List<string>();
                foreach (ManagementObject mo in queryCollection)
                {
                    string printerName = mo["Name"].ToString();
                    result.Add(printerName);
                }
                return result;
            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogManager.Severity.Error, exception.Message);
                throw;
            }

        }

        /// <summary>
        /// 获取共享打印机信息
        /// </summary>
        /// <param name="sharePrinterPath"></param>
        /// <returns></returns>
        public static string[] GetSharePrinterPathInfo(string sharePrinterPath)
        {
            return sharePrinterPath.Split('\\');
        }


        /// <summary>
        /// 打印(调用第三方程序)
        /// </summary>
        /// <param name="printModel"></param>
        /// <param name="printername"></param>
        public static void Print(PrintModel printModel, string printername)
        {
            if (string.IsNullOrEmpty(printername))
            {
                printername = "Microsoft XPS Document Writer";
            }
            try
            {
                LocalPrintServer ps = new LocalPrintServer();
                PrintQueue pq = ps.GetPrintQueue(printername);

                Process p = new Process();
                //不现实调用程序窗口,但是对于某些应用无效
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //采用操作系统自动识别的模式
                p.StartInfo.UseShellExecute = true;
                //要打印的文件路径
                p.StartInfo.FileName = printModel.filepath;
                //指定执行的动作，是打印，即print，打开是 open
                p.StartInfo.Verb = "print";
                //将指定的打印机设为默认打印机
                SetDefaultPrinter(printername);
                //开始打印
                p.Start();
                //等待1秒 
                p.WaitForExit(800);
                p.Close();

                //打印监测是否打印完成
                while (true)
                {
                    Thread.Sleep(500);
                    var job = pq.GetPrintJobInfoCollection();
                    if (job.Count() != 0)
                    {
                        while (true)
                        {
                            Thread.Sleep(500);
                            var job2 = pq.GetPrintJobInfoCollection();
                            if (job2.Count() == 0)
                            {
                                MainWindow.PrintSuccess(printModel);//触发打印完成事件
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogManager.WriteLog(LogManager.Severity.Error, e.Message);
                throw;
            }

        }
        [DllImport("Winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDefaultPrinter(string printerName);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int pcchBuffer);

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="printModel"></param>
        /// <param name="printerName"></param>
        public static void PrintPdfAndWord(PrintModel printModel,string printerName,PrinterSettingModel printerSettingModel)
        {
            try
            {

                //打印机设置
                PrinterSettings printerSettings = new PrinterSettings();
                printerSettings.PrinterName = printerName;
                printerSettings.Copies = printModel.copies;
                printerSettings.Duplex = printerSettingModel.printType;

                //设置彩色打印
                SetPrinter(printerName,printerSettingModel);


                //打印页面设置
                PageSettings pageSettings =new PageSettings();
                pageSettings.Landscape = printerSettingModel.direction;
                //pageSettings.Color = printerSettingModel.colorModel;
                pageSettings.Margins=new Margins(5,5,5,5);

                var fileExtension = ReturnExtension(printModel.filepath);
                PdfViewer pdfViewer = new PdfViewer();
                pdfViewer.EndPrint+=new PrintEventHandler(EndPrint);
                pdfViewer.AutoResize = true;
                pdfViewer.PrinterJobName = printModel.fileName;

                if (fileExtension==".doc" || fileExtension==".docx")
                {
                    Aspose.Words.Document wordDoc = new Aspose.Words.Document(printModel.filepath);
                    using (MemoryStream dstStream = new MemoryStream())
                    {
                        wordDoc.Save(dstStream, SaveFormat.Pdf);
                        Aspose.Pdf.Document doc=new Aspose.Pdf.Document(dstStream);
                        pdfViewer.BindPdf(doc);
                        pdfViewer.PrintDocumentWithSettings(pageSettings, printerSettings);
                        dstStream.Position = 0;
                    }
                }
                else if (fileExtension==".pdf")
                {
                    Aspose.Pdf.Document doc = new Aspose.Pdf.Document(printModel.filepath);
                    pdfViewer.BindPdf(doc);
                    pdfViewer.PrintDocumentWithSettings(pageSettings, printerSettings);
                }
                else
                {
                    MessageBox.Show("不识别的格式", "Error");
                    return;
                }
                //判断打印是否完成
                do
                {
                    Thread.Sleep(50);
                } while (GetPrintJobsCollection(printerName).Count > 0);
                MainWindow.PrintSuccess(printModel);

            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogManager.Severity.Error, $"PrintPdfAndWord: {exception.Message}");
                throw;
            }
        }

        public static void EndPrint(object sender, PrintEventArgs e)
        {
            PrintDocument p=(PrintDocument)sender;
            string printerName = p.PrinterSettings.PrinterName;
        }



        /// <summary>
        /// 获取默认的打印机
        /// </summary>
        /// <returns></returns>
        static string GetDefaultPrinter()
        {
            const int ERROR_FILE_NOT_FOUND = 2;

            const int ERROR_INSUFFICIENT_BUFFER = 122;

            int pcchBuffer = 0;

            if (GetDefaultPrinter(null, ref pcchBuffer))
            {
                return null;
            }

            int lastWin32Error = Marshal.GetLastWin32Error();

            if (lastWin32Error == ERROR_INSUFFICIENT_BUFFER)
            {
                StringBuilder pszBuffer = new StringBuilder(pcchBuffer);
                if (GetDefaultPrinter(pszBuffer, ref pcchBuffer))
                {
                    return pszBuffer.ToString();
                }

                lastWin32Error = Marshal.GetLastWin32Error();
            }
            if (lastWin32Error == ERROR_FILE_NOT_FOUND)
            {
                return null;
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());


        }

        /// <summary>
        /// 调用Office-COM组件进行打印
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="printername"></param>
        public static void PrintMethodCOM(string filepath,string printername)
        {
            try
            {
                if (string.IsNullOrEmpty(printername))
                {
                    printername = "Microsoft XPS Document Writer";
                }
                object wordFile = filepath;
                object oMissing = Missing.Value;
                //自定义object类型的布尔值
                object oTrue = true;
                object oFalse = false;
                object doNotSaveChanges = WdSaveOptions.wdDoNotSaveChanges;
                //定义WORD Application相关
                Application appWord = new Application();
                //WORD程序不可见
                appWord.Visible = false;
                //不弹出警告框
                appWord.DisplayAlerts = WdAlertLevel.wdAlertsNone;
                //先保存默认的打印机
                string defaultPrinter = appWord.ActivePrinter;
                //打开要打印的文件
                //如果在其它函数调用中出错（doc为null),处理办法：建立临时文件夹，还要设置服务的权限属性
                Microsoft.Office.Interop.Word.Document doc = appWord.Documents.Open(
                        ref wordFile,
                        ref oMissing,
                        ref oTrue,
                        ref oFalse,
                        ref oMissing,
                        ref oMissing,
                        ref oMissing,
                        ref oMissing,
                        ref oMissing,
                        ref oMissing,
                        ref oMissing,
                        ref oMissing,
                        ref oMissing,
                        ref oMissing,
                        ref oMissing,
                        ref oMissing);

                //设置指定的打印机
                appWord.ActivePrinter = printername;
                //"Microsoft XPS Document Writer";

                //打印
                doc.PrintOut(
                    ref oTrue, //此处为true,表示后台打印
                    ref oFalse,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing,
                    ref oMissing
                    );
                //打印完关闭WORD文件
                doc.Close(ref doNotSaveChanges, ref oMissing, ref oMissing);
                //还原原来的默认打印机
                appWord.ActivePrinter = defaultPrinter;
                //退出WORD程序
                appWord.Quit(ref oMissing, ref oMissing, ref oMissing);
                doc = null;
                appWord = null;
            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogManager.Severity.Error, $"Printer: {printername}, File: {filepath}, Print Error: {exception.Message}");
            }
        }

        /// <summary>
        /// 解决 word调用打印机报错问题，创建一个临时文件夹
        /// </summary>
        /// <returns></returns>
        public bool CreateFolder()
        {
            bool ifsuccesss = false;
            try
            {
                string systempath = System.Environment.GetFolderPath(Environment.SpecialFolder.System);
                string fullpath = string.Empty;
                if (FindSystemWidth() == "32")
                {
                    fullpath = systempath + "\\config\\systemprofile\\Desktop";
                }
                else
                {
                    fullpath = systempath + "\\config\\systemprofile\\Desktop";
                }
                if (!Directory.Exists(fullpath))
                {
                    Directory.CreateDirectory(fullpath);
                }
                ifsuccesss = true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogManager.Severity.Error, $"Create Folder Error: {ex.Message}");
                ifsuccesss = false;
            }
            return ifsuccesss;

        }
        /// <summary>
        /// 获取系统的位数
        /// </summary>
        /// <returns></returns>
        public string FindSystemWidth()
        {
            ConnectionOptions oConn = new ConnectionOptions();
            System.Management.ManagementScope oMs = new System.Management.ManagementScope("\\\\localhost", oConn);
            System.Management.ObjectQuery oQuery = new System.Management.ObjectQuery("select AddressWidth from Win32_Processor");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
            ManagementObjectCollection oReturnCollection = oSearcher.Get();
            string addressWidth = null;

            foreach (ManagementObject oReturn in oReturnCollection)
            {
                addressWidth = oReturn["AddressWidth"].ToString();
            }

            return addressWidth;
        }

        /// <summary>
        /// 将文件添加入打印队列(本地打印)
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <returns></returns>
        public List<PrintModel> AddPrintList(string folderPath)
        {
            try
            {
                List<PrintModel> printList = new List<PrintModel>();
                if (string.IsNullOrEmpty(folderPath))
                {
                    MessageBox.Show("未选择文件夹");
                }
                else
                {

                    DirectoryInfo dirinfo = new DirectoryInfo(folderPath);
                    foreach (FileSystemInfo fsi in dirinfo.GetFileSystemInfos())
                    {
                        if (fsi is FileInfo)
                        {
                            FileInfo fi = (FileInfo)fsi;
                            if (fi.FullName != null)
                            {
                                printList.Add(new PrintModel
                                {
                                    filepath=fi.FullName,
                                    fileName = Path.GetFileName(fi.FullName),
                                    status = "准备打印",
                                    copies = 1
                                });
                            }
                        }
                    }

                }
                return printList;
            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogManager.Severity.Error, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// 获取打印机的当前状态
        /// </summary>
        /// <param name="PrinterDevice">打印机设备名称</param>
        /// <returns>打印机状态</returns>
        public static PrinterStatus GetPrinterStatus(string PrinterDevice)
        {
            try
            {
                if (string.IsNullOrEmpty(PrinterDevice))
                {
                    //若打印机为空
                    PrinterDevice = "Microsoft XPS Document Writer";
                }
                PrinterStatus ret = 0;
                string path = @"win32_printer.DeviceId='" + PrinterDevice + "'";
                ManagementObject printer = new ManagementObject(path);
                printer.Get();
                ret = (PrinterStatus)Convert.ToInt32(printer.Properties["PrinterStatus"].Value);
                return ret;
            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogManager.Severity.Error, exception.Message);
                throw;
            }

        }
        public enum PrinterStatus
        {
            其他状态 = 1,
            未知,
            空闲,
            正在打印,
            预热,
            停止打印,
            打印中,
            离线
        }

        /// <summary>
        /// 获取打印队列的作业数量
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public static StringCollection GetPrintJobsCollection(string printerName)
        {
            StringCollection printJobCollection = new StringCollection();
            string searchQuery = "SELECT * FROM Win32_PrintJob";
            /*searchQuery can also be mentioned with where Attribute,
                but this is not working in Windows 2000 / ME / 98 machines 
                and throws Invalid query error*/
            ManagementObjectSearcher searchPrintJobs =
                      new ManagementObjectSearcher(searchQuery);
            ManagementObjectCollection prntJobCollection = searchPrintJobs.Get();
            foreach (ManagementObject prntJob in prntJobCollection)
            {
                System.String jobName = prntJob.Properties["Name"].Value.ToString();
                //Job name would be of the format [Printer name], [Job ID]
                char[] splitArr = new char[1];
                splitArr[0] = Convert.ToChar(",");
                string prnterName = jobName.Split(splitArr)[0];
                string documentName = prntJob.Properties["Document"].Value.ToString();
                if (String.Compare(prnterName, printerName, true) == 0)
                {
                    printJobCollection.Add(documentName);
                }
            }
            return printJobCollection;
        }

        /// <summary>
        /// 获取本地打印机的打印任务集合
        /// </summary>
        /// <param name="PrinterName"></param>
        /// <returns></returns>
        protected static Dictionary<string, int> GetPrintQueue(string PrinterName)
        {
            Dictionary<string, int> tempDict = new Dictionary<string, int>();
            LocalPrintServer pr = new LocalPrintServer();
            pr.Refresh();
            EnumeratedPrintQueueTypes[] enumerationFlags =
            {
                EnumeratedPrintQueueTypes.Local,
                EnumeratedPrintQueueTypes.Connections,
            };
            foreach (PrintQueue pq in pr.GetPrintQueues(enumerationFlags))
            {
                if (pq.Name == PrinterName && pq.NumberOfJobs > 0)
                {
                    var jobs = pq.GetPrintJobInfoCollection();
                    foreach (var job in jobs)
                    {
                        tempDict.Add(job.Name + "_" + job.JobIdentifier.ToString(), job.JobIdentifier);
                    }
                }
            }
            return tempDict;
        }

        /// <summary>
        /// 打印机设置(PrintServer)
        /// </summary>
        /// <param name="setName"></param>
        public static void SetPrinter(string printerName, PrinterSettingModel printerSettingModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(printerName))
                {
                    PrintQueue pq = null;
                    if (printerName.Contains("\\"))
                    {
                        //是共享打印机
                        var sharePrinter = GetSharePrinterPathInfo(printerName);
                        PrintServer sharePrintServer=new PrintServer(@"\\"+sharePrinter[2]);
                        pq = sharePrintServer.GetPrintQueue(printerName);
                    }
                    else
                    {
                        LocalPrintServer localPrintServer = new LocalPrintServer();
                        pq = localPrintServer.GetPrintQueue(printerName);
                    }

                    PrintTicket pt = pq.UserPrintTicket;
                    if (printerSettingModel.colorModel)
                    {
                        pt.OutputColor = OutputColor.Color;
                    }
                    else
                    {
                        pt.OutputColor = OutputColor.Monochrome;
                    }
                    

                    //if (!string.IsNullOrEmpty(printType))
                    //{
                    //    if (printType== "单面打印")
                    //    {
                    //        pt.Duplexing = Duplexing.OneSided;
                    //    }
                    //    else if (printType == "双面打印")
                    //    {
                    //        pt.Duplexing = Duplexing.TwoSidedLongEdge;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(direction))
                    //{
                    //    if (direction == "横向打印")
                    //    {
                    //        pt.PageOrientation = PageOrientation.Landscape;
                    //    }
                    //    else if (direction == "纵向打印")
                    //    {
                    //        pt.PageOrientation = PageOrientation.Portrait;
                    //    }
                    //}
                    //LogManager.WriteLog(LogManager.Severity.Debug,"颜色模式:"+ colorMode);

                    ValidationResult result = pq.MergeAndValidatePrintTicket(pq.UserPrintTicket, pt);
                    pq.UserPrintTicket = result.ValidatedPrintTicket;
                    pq.Commit();
                }
            }
            catch (Exception exception)
            {
                LogManager.WriteLog(LogManager.Severity.Error, $"Create Folder Error: {exception.Message}");
                throw;
            }
        }


        /// <summary>
        /// 返回文件后缀名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReturnExtension(String filePath)
        {
            return Path.GetExtension(filePath).ToLower();
        }

    }
}