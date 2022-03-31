
namespace LabelPrint
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.PrinterComboBox = new Sunny.UI.UIComboBox();
            this.uiMarkLabel1 = new Sunny.UI.UIMarkLabel();
            this.StartBtn = new Sunny.UI.UIButton();
            this.SuspendLayout();
            // 
            // PrinterComboBox
            // 
            this.PrinterComboBox.DataSource = null;
            this.PrinterComboBox.FillColor = System.Drawing.Color.White;
            this.PrinterComboBox.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.PrinterComboBox.Location = new System.Drawing.Point(57, 121);
            this.PrinterComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PrinterComboBox.MinimumSize = new System.Drawing.Size(63, 0);
            this.PrinterComboBox.Name = "PrinterComboBox";
            this.PrinterComboBox.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.PrinterComboBox.Size = new System.Drawing.Size(273, 34);
            this.PrinterComboBox.TabIndex = 1;
            this.PrinterComboBox.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiMarkLabel1
            // 
            this.uiMarkLabel1.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.uiMarkLabel1.Location = new System.Drawing.Point(53, 82);
            this.uiMarkLabel1.Name = "uiMarkLabel1";
            this.uiMarkLabel1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.uiMarkLabel1.Size = new System.Drawing.Size(153, 23);
            this.uiMarkLabel1.TabIndex = 2;
            this.uiMarkLabel1.Text = "请选择打印机：";
            this.uiMarkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StartBtn
            // 
            this.StartBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.StartBtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.StartBtn.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.StartBtn.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.StartBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.StartBtn.Location = new System.Drawing.Point(57, 177);
            this.StartBtn.MinimumSize = new System.Drawing.Size(1, 1);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.StartBtn.Size = new System.Drawing.Size(273, 39);
            this.StartBtn.Style = Sunny.UI.UIStyle.LightBlue;
            this.StartBtn.TabIndex = 3;
            this.StartBtn.Text = "开始打印";
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 296);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.uiMarkLabel1);
            this.Controls.Add(this.PrinterComboBox);
            this.Name = "Main";
            this.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.Style = Sunny.UI.UIStyle.Custom;
            this.Text = "LABEL PRINT";
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UIComboBox PrinterComboBox;
        private Sunny.UI.UIMarkLabel uiMarkLabel1;
        private Sunny.UI.UIButton StartBtn;
    }
}

