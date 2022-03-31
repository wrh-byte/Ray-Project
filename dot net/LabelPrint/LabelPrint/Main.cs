using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using LabelPrint.Models;
using Newtonsoft.Json;
using Sunny.UI;

namespace LabelPrint
{

    public partial class Main : UIForm
    {

        private readonly string[] _args = null;
        private SampleLabelModel _sampleLabel;
        //模板路径 Type1
        private readonly string _tempPath = System.Windows.Forms.Application.UserAppDataPath + "Template\\SampleLabel.btw";
        List<string> tempDataSourceNameList = new List<string>{ "Code", "CsName", "DueTime", "Remark", "BarCode", "Name", "TakeTime" }; 
        //UTC模板路径 Type2
        private readonly string _UTCTempPath = System.Windows.Forms.Application.UserAppDataPath + "Template\\UTCSampleLabel.btw";
        List<string> UTCTempDataSourceNameList = new List<string> { "BarCode", "Name" };
        public Main(string[] args)
        {
            InitializeComponent();
            _args = args;
        }


        //Main Windows Load Event
        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                //Load Printer in PrinterComboBox
                LoadPrinter();
                //1.check connect
                if (!BartenderPrint.IsConnectInternet())
                {
                    UIMessageBox.Show("未连入互联网，请先链接网络", "警告", UIStyle.Gray);
                }

                //2. convert model
                if (_args != null && _args.Length > 0)
                {
                    string args = _args[0];
                    string[] strings = args.Split("=");
                    if (strings.Length >= 2)
                    {
                        string json = HttpUtility.UrlDecode(strings[1], Encoding.UTF8);
                        if (json.LastIndexOf('/')>=0)
                        {
                            json = json.TrimEnd('/');
                        }
                        if (!string.IsNullOrEmpty(json))
                        {
                            try
                            {
                                _sampleLabel = JsonConvert.DeserializeObject<SampleLabelModel>(json);
                            }
                            catch (Exception)
                            {
                                UIMessageBox.Show("转换json失败", "警告", UIStyle.Gray);
                                throw;
                            }
                            if (_sampleLabel == null)
                            {
                                UIMessageBox.Show("转换后对象为空", "警告", UIStyle.Gray);
                            }
                        }
                        else
                        {
                            UIMessageBox.Show("json字符串为空", "警告", UIStyle.Gray);
                        }
                    }
                    else
                    {
                        UIMessageBox.Show("传入参数异常", "警告", UIStyle.Gray);
                    }
                }
                else
                {
                    UIMessageBox.Show("未传入参数args", "警告", UIStyle.Gray);
                }

                //3. download the tempPackage
                if (_sampleLabel != null)
                {
                    List<string> downloadTempUrls = BartenderPrint.GetTempDownloadUrl(_sampleLabel.OrgId, _sampleLabel.OrgName);
                    foreach (var downloadTempUrl in downloadTempUrls)
                    {
                        BartenderPrint.HttpDownloadFile(downloadTempUrl);
                    }
                }
                else
                {
                    UIMessageBox.Show("拉取模板文件失败", "警告", UIStyle.Gray);
                }
            }
            catch (Exception exception)
            {
                UIMessageBox.Show(exception.Message, "警告", UIStyle.Gray);
            }
        }

        private void LoadPrinter()
        {
            PrintDocument printDocument = new PrintDocument();
            //获取默认的打印机名
            string strDefaultPrinter = printDocument.PrinterSettings.PrinterName;

            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                this.PrinterComboBox.Items.Add(printer);
                if (printer == strDefaultPrinter)//把默认打印机设为缺省值
                {
                    PrinterComboBox.SelectedIndex = PrinterComboBox.Items.IndexOf(printer);
                }
            }
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            List<Data> datas = _sampleLabel.Datas;
            if (datas != null && datas.Count > 0)
            {
                foreach (var item in datas)
                {
                    string tempPath="";
                    List<string> dataSourceNameList = null; 
                    if (_sampleLabel.Type=="Type1")
                    {
                        tempPath = _tempPath;
                        dataSourceNameList = tempDataSourceNameList;
                    }
                    else if (_sampleLabel.Type == "Type2")
                    {
                        tempPath = _UTCTempPath;
                        dataSourceNameList = UTCTempDataSourceNameList;
                    }
                    BartenderPrint.Print(item, tempPath, PrinterComboBox.Text, dataSourceNameList);
                }
                this.Close();
            }
            else
            {
                UIMessageBox.Show("打印数据为空", "错误", UIStyle.Gray);
            }
        }
    }
}
