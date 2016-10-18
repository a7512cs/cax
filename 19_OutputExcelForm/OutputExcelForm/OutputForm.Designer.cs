namespace OutputExcelForm
{
    partial class OutputForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        public void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutputForm));
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.CusComboBox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            PartNoCombobox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            CusVerCombobox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.superTabControl1 = new DevComponents.DotNetBar.SuperTabControl();
            this.superTabControlPanel1 = new DevComponents.DotNetBar.SuperTabControlPanel();
            this.SGCPanel = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.select = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.ExcelType = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.ExcelForm = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.OutputPath = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.Tab_METE = new DevComponents.DotNetBar.SuperTabItem();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            Op1Combobox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.OK = new DevComponents.DotNetBar.ButtonX();
            this.Close = new DevComponents.DotNetBar.ButtonX();
            ((System.ComponentModel.ISupportInitialize)(this.superTabControl1)).BeginInit();
            this.superTabControl1.SuspendLayout();
            this.superTabControlPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX1.Location = new System.Drawing.Point(49, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(58, 35);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "客戶：";
            // 
            // CusComboBox
            // 
            this.CusComboBox.DisplayMember = "Text";
            this.CusComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CusComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CusComboBox.FormattingEnabled = true;
            this.CusComboBox.ItemHeight = 16;
            this.CusComboBox.Location = new System.Drawing.Point(103, 17);
            this.CusComboBox.Name = "CusComboBox";
            this.CusComboBox.Size = new System.Drawing.Size(92, 22);
            this.CusComboBox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.CusComboBox.TabIndex = 2;
            this.CusComboBox.SelectedIndexChanged += new System.EventHandler(this.CusComboBox_SelectedIndexChanged);
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX2.Location = new System.Drawing.Point(258, 12);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(58, 35);
            this.labelX2.TabIndex = 4;
            this.labelX2.Text = "料號：";
            // 
            // PartNoCombobox
            // 
            PartNoCombobox.DisplayMember = "Text";
            PartNoCombobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            PartNoCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            PartNoCombobox.FormattingEnabled = true;
            PartNoCombobox.ItemHeight = 16;
            PartNoCombobox.Location = new System.Drawing.Point(312, 18);
            PartNoCombobox.Name = "PartNoCombobox";
            PartNoCombobox.Size = new System.Drawing.Size(148, 22);
            PartNoCombobox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            PartNoCombobox.TabIndex = 5;
            PartNoCombobox.SelectedIndexChanged += new System.EventHandler(this.PartNoCombobox_SelectedIndexChanged);
            // 
            // labelX3
            // 
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX3.Location = new System.Drawing.Point(49, 53);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(58, 35);
            this.labelX3.TabIndex = 7;
            this.labelX3.Text = "版次：";
            // 
            // CusVerCombobox
            // 
            CusVerCombobox.DisplayMember = "Text";
            CusVerCombobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            CusVerCombobox.FormattingEnabled = true;
            CusVerCombobox.ItemHeight = 16;
            CusVerCombobox.Location = new System.Drawing.Point(103, 58);
            CusVerCombobox.Name = "CusVerCombobox";
            CusVerCombobox.Size = new System.Drawing.Size(92, 22);
            CusVerCombobox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            CusVerCombobox.TabIndex = 8;
            CusVerCombobox.SelectedIndexChanged += new System.EventHandler(this.CusVerCombobox_SelectedIndexChanged);
            // 
            // superTabControl1
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            this.superTabControl1.ControlBox.CloseBox.Name = "";
            // 
            // 
            // 
            this.superTabControl1.ControlBox.MenuBox.Name = "";
            this.superTabControl1.ControlBox.Name = "";
            this.superTabControl1.ControlBox.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.superTabControl1.ControlBox.MenuBox,
            this.superTabControl1.ControlBox.CloseBox});
            this.superTabControl1.Controls.Add(this.superTabControlPanel1);
            this.superTabControl1.Location = new System.Drawing.Point(12, 94);
            this.superTabControl1.Name = "superTabControl1";
            this.superTabControl1.ReorderTabsEnabled = true;
            this.superTabControl1.SelectedTabFont = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold);
            this.superTabControl1.SelectedTabIndex = 0;
            this.superTabControl1.Size = new System.Drawing.Size(448, 221);
            this.superTabControl1.TabFont = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.superTabControl1.TabIndex = 9;
            this.superTabControl1.Tabs.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.Tab_METE});
            this.superTabControl1.Text = "superTabControl1";
            // 
            // superTabControlPanel1
            // 
            this.superTabControlPanel1.Controls.Add(this.SGCPanel);
            this.superTabControlPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superTabControlPanel1.Location = new System.Drawing.Point(0, 45);
            this.superTabControlPanel1.Name = "superTabControlPanel1";
            this.superTabControlPanel1.Size = new System.Drawing.Size(448, 176);
            this.superTabControlPanel1.TabIndex = 1;
            this.superTabControlPanel1.TabItem = this.Tab_METE;
            // 
            // SGCPanel
            // 
            this.SGCPanel.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.SGCPanel.Location = new System.Drawing.Point(3, 3);
            this.SGCPanel.Name = "SGCPanel";
            // 
            // 
            // 
            this.SGCPanel.PrimaryGrid.ColumnDragBehavior = DevComponents.DotNetBar.SuperGrid.ColumnDragBehavior.None;
            this.SGCPanel.PrimaryGrid.Columns.Add(this.select);
            this.SGCPanel.PrimaryGrid.Columns.Add(this.ExcelType);
            this.SGCPanel.PrimaryGrid.Columns.Add(this.ExcelForm);
            this.SGCPanel.PrimaryGrid.Columns.Add(this.OutputPath);
            this.SGCPanel.PrimaryGrid.ShowRowHeaders = false;
            this.SGCPanel.Size = new System.Drawing.Size(442, 170);
            this.SGCPanel.TabIndex = 0;
            this.SGCPanel.Text = "superGridControl1";
            // 
            // select
            // 
            this.select.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.select.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridCheckBoxXEditControl);
            this.select.Name = "選擇";
            this.select.Width = 50;
            // 
            // ExcelType
            // 
            this.ExcelType.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.ExcelType.Name = "Excel表單";
            this.ExcelType.Width = 70;
            // 
            // ExcelForm
            // 
            this.ExcelForm.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.ExcelForm.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridComboBoxExEditControl);
            this.ExcelForm.Name = "廠區";
            this.ExcelForm.Width = 120;
            // 
            // OutputPath
            // 
            this.OutputPath.AutoSizeMode = DevComponents.DotNetBar.SuperGrid.ColumnAutoSizeMode.Fill;
            this.OutputPath.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            this.OutputPath.Name = "輸出路徑";
            // 
            // Tab_METE
            // 
            this.Tab_METE.AttachedControl = this.superTabControlPanel1;
            this.Tab_METE.GlobalItem = false;
            this.Tab_METE.Image = global::OutputExcelForm.Properties.Resources.files_32px;
            this.Tab_METE.Name = "Tab_METE";
            this.Tab_METE.Text = "各式表單";
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::OutputExcelForm.Properties.Resources.OIS_48px;
            this.pictureBox4.Location = new System.Drawing.Point(221, 53);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(31, 35);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 10;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::OutputExcelForm.Properties.Resources.folder_48px;
            this.pictureBox3.Location = new System.Drawing.Point(12, 53);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(31, 35);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 6;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::OutputExcelForm.Properties.Resources.material_48px;
            this.pictureBox2.Location = new System.Drawing.Point(221, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(31, 35);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::OutputExcelForm.Properties.Resources.company_48px;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(31, 35);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // labelX4
            // 
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX4.Location = new System.Drawing.Point(258, 53);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(58, 35);
            this.labelX4.TabIndex = 11;
            this.labelX4.Text = "製程：";
            // 
            // Op1Combobox
            // 
            Op1Combobox.DisplayMember = "Text";
            Op1Combobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            Op1Combobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            Op1Combobox.FormattingEnabled = true;
            Op1Combobox.ItemHeight = 16;
            Op1Combobox.Location = new System.Drawing.Point(312, 56);
            Op1Combobox.Name = "Op1Combobox";
            Op1Combobox.Size = new System.Drawing.Size(148, 22);
            Op1Combobox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            Op1Combobox.TabIndex = 12;
            Op1Combobox.SelectedIndexChanged += new System.EventHandler(this.Op1Combobox_SelectedIndexChanged);
            // 
            // OK
            // 
            this.OK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.OK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.OK.Image = global::OutputExcelForm.Properties.Resources.ok_32px;
            this.OK.Location = new System.Drawing.Point(103, 326);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(96, 41);
            this.OK.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.OK.TabIndex = 13;
            this.OK.Text = "輸出表單";
            this.OK.TextAlignment = DevComponents.DotNetBar.eButtonTextAlignment.Right;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Close
            // 
            this.Close.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Close.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.Close.Image = global::OutputExcelForm.Properties.Resources.cancel_32px;
            this.Close.Location = new System.Drawing.Point(254, 326);
            this.Close.Name = "Close";
            this.Close.Size = new System.Drawing.Size(96, 41);
            this.Close.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Close.TabIndex = 14;
            this.Close.Text = "關閉視窗";
            this.Close.TextAlignment = DevComponents.DotNetBar.eButtonTextAlignment.Right;
            this.Close.Click += new System.EventHandler(this.Close_Click);
            // 
            // OutputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 376);
            this.Controls.Add(this.Close);
            this.Controls.Add(this.OK);
            this.Controls.Add(Op1Combobox);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.superTabControl1);
            this.Controls.Add(CusVerCombobox);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(PartNoCombobox);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.CusComboBox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelX1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OutputForm";
            this.Text = "輸出表單";
            ((System.ComponentModel.ISupportInitialize)(this.superTabControl1)).EndInit();
            this.superTabControl1.ResumeLayout(false);
            this.superTabControlPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx CusComboBox;
        private System.Windows.Forms.PictureBox pictureBox2;
        private DevComponents.DotNetBar.LabelX labelX2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.SuperTabControl superTabControl1;
        private DevComponents.DotNetBar.SuperTabControlPanel superTabControlPanel1;
        private DevComponents.DotNetBar.SuperTabItem Tab_METE;
        private System.Windows.Forms.PictureBox pictureBox4;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.ButtonX OK;
        private DevComponents.DotNetBar.ButtonX Close;
        private DevComponents.DotNetBar.SuperGrid.SuperGridControl SGCPanel;
        private DevComponents.DotNetBar.SuperGrid.GridColumn select;
        private DevComponents.DotNetBar.SuperGrid.GridColumn ExcelType;
        private DevComponents.DotNetBar.SuperGrid.GridColumn ExcelForm;
        private DevComponents.DotNetBar.SuperGrid.GridColumn OutputPath;
        public static DevComponents.DotNetBar.Controls.ComboBoxEx PartNoCombobox;
        public static DevComponents.DotNetBar.Controls.ComboBoxEx CusVerCombobox;
        public static DevComponents.DotNetBar.Controls.ComboBoxEx Op1Combobox;
    }
}

