namespace MEDownload
{
    partial class MEDownloadDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.styleManager1 = new DevComponents.DotNetBar.StyleManager(this.components);
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.PartNocomboBox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.CusRevcomboBox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.Oper1comboBox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.listView = new System.Windows.Forms.ListView();
            this.buttonDownload = new DevComponents.DotNetBar.ButtonX();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.labelX6 = new DevComponents.DotNetBar.LabelX();
            this.comboBoxCusName = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.SuspendLayout();
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Font = new System.Drawing.Font("新細明體", 13F);
            this.labelX1.Location = new System.Drawing.Point(19, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(160, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "工程師職稱：ME";
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Font = new System.Drawing.Font("新細明體", 13F);
            this.labelX2.Location = new System.Drawing.Point(19, 70);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(75, 23);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "料號：";
            // 
            // labelX3
            // 
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Font = new System.Drawing.Font("新細明體", 13F);
            this.labelX3.Location = new System.Drawing.Point(19, 99);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(110, 23);
            this.labelX3.TabIndex = 2;
            this.labelX3.Text = "客戶版次：";
            // 
            // labelX4
            // 
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Font = new System.Drawing.Font("新細明體", 13F);
            this.labelX4.Location = new System.Drawing.Point(19, 128);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(90, 23);
            this.labelX4.TabIndex = 3;
            this.labelX4.Text = "製程序：";
            // 
            // PartNocomboBox
            // 
            this.PartNocomboBox.DisplayMember = "Text";
            this.PartNocomboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.PartNocomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PartNocomboBox.FormattingEnabled = true;
            this.PartNocomboBox.ItemHeight = 16;
            this.PartNocomboBox.Location = new System.Drawing.Point(75, 67);
            this.PartNocomboBox.Name = "PartNocomboBox";
            this.PartNocomboBox.Size = new System.Drawing.Size(136, 22);
            this.PartNocomboBox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.PartNocomboBox.TabIndex = 4;
            this.PartNocomboBox.SelectedIndexChanged += new System.EventHandler(this.PartNocomboBox_SelectedIndexChanged);
            // 
            // CusRevcomboBox
            // 
            this.CusRevcomboBox.DisplayMember = "Text";
            this.CusRevcomboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CusRevcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CusRevcomboBox.FormattingEnabled = true;
            this.CusRevcomboBox.ItemHeight = 16;
            this.CusRevcomboBox.Location = new System.Drawing.Point(112, 96);
            this.CusRevcomboBox.Name = "CusRevcomboBox";
            this.CusRevcomboBox.Size = new System.Drawing.Size(99, 22);
            this.CusRevcomboBox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.CusRevcomboBox.TabIndex = 5;
            this.CusRevcomboBox.SelectedIndexChanged += new System.EventHandler(this.CusRevcomboBox_SelectedIndexChanged);
            // 
            // Oper1comboBox
            // 
            this.Oper1comboBox.DisplayMember = "Text";
            this.Oper1comboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Oper1comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Oper1comboBox.FormattingEnabled = true;
            this.Oper1comboBox.ItemHeight = 16;
            this.Oper1comboBox.Location = new System.Drawing.Point(95, 125);
            this.Oper1comboBox.Name = "Oper1comboBox";
            this.Oper1comboBox.Size = new System.Drawing.Size(116, 22);
            this.Oper1comboBox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Oper1comboBox.TabIndex = 6;
            this.Oper1comboBox.SelectedIndexChanged += new System.EventHandler(this.Oper1comboBox_SelectedIndexChanged);
            // 
            // listView
            // 
            this.listView.Location = new System.Drawing.Point(12, 192);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(208, 191);
            this.listView.TabIndex = 7;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.List;
            // 
            // buttonDownload
            // 
            this.buttonDownload.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonDownload.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonDownload.Location = new System.Drawing.Point(75, 389);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(75, 23);
            this.buttonDownload.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonDownload.TabIndex = 8;
            this.buttonDownload.Text = "下載";
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // labelX5
            // 
            // 
            // 
            // 
            this.labelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX5.Font = new System.Drawing.Font("新細明體", 13F);
            this.labelX5.Location = new System.Drawing.Point(75, 166);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(92, 23);
            this.labelX5.TabIndex = 9;
            this.labelX5.Text = "下載資訊";
            // 
            // labelX6
            // 
            // 
            // 
            // 
            this.labelX6.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX6.Font = new System.Drawing.Font("新細明體", 13F);
            this.labelX6.Location = new System.Drawing.Point(19, 41);
            this.labelX6.Name = "labelX6";
            this.labelX6.Size = new System.Drawing.Size(75, 23);
            this.labelX6.TabIndex = 10;
            this.labelX6.Text = "客戶：";
            // 
            // comboBoxCusName
            // 
            this.comboBoxCusName.DisplayMember = "Text";
            this.comboBoxCusName.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxCusName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCusName.FormattingEnabled = true;
            this.comboBoxCusName.ItemHeight = 16;
            this.comboBoxCusName.Location = new System.Drawing.Point(75, 38);
            this.comboBoxCusName.Name = "comboBoxCusName";
            this.comboBoxCusName.Size = new System.Drawing.Size(136, 22);
            this.comboBoxCusName.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.comboBoxCusName.TabIndex = 11;
            this.comboBoxCusName.SelectedIndexChanged += new System.EventHandler(this.comboBoxCusName_SelectedIndexChanged);
            // 
            // MEDownloadDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 426);
            this.Controls.Add(this.comboBoxCusName);
            this.Controls.Add(this.labelX6);
            this.Controls.Add(this.labelX5);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.Oper1comboBox);
            this.Controls.Add(this.CusRevcomboBox);
            this.Controls.Add(this.PartNocomboBox);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MEDownloadDlg";
            this.Text = "ME下載";
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.StyleManager styleManager1;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.Controls.ComboBoxEx PartNocomboBox;
        private DevComponents.DotNetBar.Controls.ComboBoxEx CusRevcomboBox;
        private DevComponents.DotNetBar.Controls.ComboBoxEx Oper1comboBox;
        private System.Windows.Forms.ListView listView;
        private DevComponents.DotNetBar.ButtonX buttonDownload;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.LabelX labelX6;
        private DevComponents.DotNetBar.Controls.ComboBoxEx comboBoxCusName;
    }
}