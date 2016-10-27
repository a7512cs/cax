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
            this.PartNoCombobox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.CusVerCombobox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.SuperTabControl = new DevComponents.DotNetBar.SuperTabControl();
            this.superTabControlPanel2 = new DevComponents.DotNetBar.SuperTabControlPanel();
            this.SGC_TEPanel = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.gridColumn1 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn2 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn3 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn4 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn5 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.Tab_TE = new DevComponents.DotNetBar.SuperTabItem();
            this.superTabControlPanel1 = new DevComponents.DotNetBar.SuperTabControlPanel();
            this.SGC_MEPanel = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.select = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.ExcelType = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.ExcelForm = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.OutputPath = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.Tab_ME = new DevComponents.DotNetBar.SuperTabItem();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.Op1Combobox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.OK = new DevComponents.DotNetBar.ButtonX();
            this.Close = new DevComponents.DotNetBar.ButtonX();
            ((System.ComponentModel.ISupportInitialize)(this.SuperTabControl)).BeginInit();
            this.SuperTabControl.SuspendLayout();
            this.superTabControlPanel2.SuspendLayout();
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
            this.PartNoCombobox.DisplayMember = "Text";
            this.PartNoCombobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.PartNoCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PartNoCombobox.FormattingEnabled = true;
            this.PartNoCombobox.ItemHeight = 16;
            this.PartNoCombobox.Location = new System.Drawing.Point(312, 18);
            this.PartNoCombobox.Name = "PartNoCombobox";
            this.PartNoCombobox.Size = new System.Drawing.Size(148, 22);
            this.PartNoCombobox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.PartNoCombobox.TabIndex = 5;
            this.PartNoCombobox.SelectedIndexChanged += new System.EventHandler(this.PartNoCombobox_SelectedIndexChanged);
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
            this.CusVerCombobox.DisplayMember = "Text";
            this.CusVerCombobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CusVerCombobox.FormattingEnabled = true;
            this.CusVerCombobox.ItemHeight = 16;
            this.CusVerCombobox.Location = new System.Drawing.Point(103, 58);
            this.CusVerCombobox.Name = "CusVerCombobox";
            this.CusVerCombobox.Size = new System.Drawing.Size(92, 22);
            this.CusVerCombobox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.CusVerCombobox.TabIndex = 8;
            this.CusVerCombobox.SelectedIndexChanged += new System.EventHandler(this.CusVerCombobox_SelectedIndexChanged);
            // 
            // SuperTabControl
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            this.SuperTabControl.ControlBox.CloseBox.Name = "";
            // 
            // 
            // 
            this.SuperTabControl.ControlBox.MenuBox.Name = "";
            this.SuperTabControl.ControlBox.Name = "";
            this.SuperTabControl.ControlBox.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.SuperTabControl.ControlBox.MenuBox,
            this.SuperTabControl.ControlBox.CloseBox});
            this.SuperTabControl.Controls.Add(this.superTabControlPanel2);
            this.SuperTabControl.Controls.Add(this.superTabControlPanel1);
            this.SuperTabControl.Location = new System.Drawing.Point(12, 94);
            this.SuperTabControl.Name = "SuperTabControl";
            this.SuperTabControl.ReorderTabsEnabled = true;
            this.SuperTabControl.SelectedTabFont = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold);
            this.SuperTabControl.SelectedTabIndex = 0;
            this.SuperTabControl.Size = new System.Drawing.Size(448, 221);
            this.SuperTabControl.TabFont = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.SuperTabControl.TabIndex = 9;
            this.SuperTabControl.Tabs.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.Tab_ME,
            this.Tab_TE});
            this.SuperTabControl.Text = "superTabControl1";
            // 
            // superTabControlPanel2
            // 
            this.superTabControlPanel2.Controls.Add(this.SGC_TEPanel);
            this.superTabControlPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superTabControlPanel2.Location = new System.Drawing.Point(0, 45);
            this.superTabControlPanel2.Name = "superTabControlPanel2";
            this.superTabControlPanel2.Size = new System.Drawing.Size(448, 176);
            this.superTabControlPanel2.TabIndex = 0;
            this.superTabControlPanel2.TabItem = this.Tab_TE;
            // 
            // SGC_TEPanel
            // 
            this.SGC_TEPanel.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.SGC_TEPanel.Location = new System.Drawing.Point(3, 3);
            this.SGC_TEPanel.Name = "SGC_TEPanel";
            // 
            // 
            // 
            this.SGC_TEPanel.PrimaryGrid.Columns.Add(this.gridColumn1);
            this.SGC_TEPanel.PrimaryGrid.Columns.Add(this.gridColumn2);
            this.SGC_TEPanel.PrimaryGrid.Columns.Add(this.gridColumn3);
            this.SGC_TEPanel.PrimaryGrid.Columns.Add(this.gridColumn4);
            this.SGC_TEPanel.PrimaryGrid.Columns.Add(this.gridColumn5);
            this.SGC_TEPanel.PrimaryGrid.ShowRowHeaders = false;
            this.SGC_TEPanel.Size = new System.Drawing.Size(442, 170);
            this.SGC_TEPanel.TabIndex = 0;
            this.SGC_TEPanel.Text = "superGridControl1";
            // 
            // gridColumn1
            // 
            this.gridColumn1.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.gridColumn1.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridCheckBoxXEditControl);
            this.gridColumn1.Name = "選擇";
            this.gridColumn1.Width = 40;
            // 
            // gridColumn2
            // 
            this.gridColumn2.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.gridColumn2.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            this.gridColumn2.Name = "Excel";
            this.gridColumn2.Width = 60;
            // 
            // gridColumn3
            // 
            this.gridColumn3.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.gridColumn3.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            this.gridColumn3.Name = "NC群組";
            this.gridColumn3.Width = 70;
            // 
            // gridColumn4
            // 
            this.gridColumn4.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.gridColumn4.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridComboBoxExEditControl);
            this.gridColumn4.Name = "廠區";
            // 
            // gridColumn5
            // 
            this.gridColumn5.AutoSizeMode = DevComponents.DotNetBar.SuperGrid.ColumnAutoSizeMode.Fill;
            this.gridColumn5.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            this.gridColumn5.Name = "輸出路徑(桌面)";
            // 
            // Tab_TE
            // 
            this.Tab_TE.AttachedControl = this.superTabControlPanel2;
            this.Tab_TE.GlobalItem = false;
            this.Tab_TE.Image = global::OutputExcelForm.Properties.Resources.wrench_32px;
            this.Tab_TE.Name = "Tab_TE";
            this.Tab_TE.Text = "TE表單";
            // 
            // superTabControlPanel1
            // 
            this.superTabControlPanel1.Controls.Add(this.SGC_MEPanel);
            this.superTabControlPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superTabControlPanel1.Location = new System.Drawing.Point(0, 45);
            this.superTabControlPanel1.Name = "superTabControlPanel1";
            this.superTabControlPanel1.Size = new System.Drawing.Size(448, 176);
            this.superTabControlPanel1.TabIndex = 1;
            this.superTabControlPanel1.TabItem = this.Tab_ME;
            // 
            // SGC_MEPanel
            // 
            this.SGC_MEPanel.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.SGC_MEPanel.Location = new System.Drawing.Point(3, 3);
            this.SGC_MEPanel.Name = "SGC_MEPanel";
            // 
            // 
            // 
            this.SGC_MEPanel.PrimaryGrid.ColumnDragBehavior = DevComponents.DotNetBar.SuperGrid.ColumnDragBehavior.None;
            this.SGC_MEPanel.PrimaryGrid.Columns.Add(this.select);
            this.SGC_MEPanel.PrimaryGrid.Columns.Add(this.ExcelType);
            this.SGC_MEPanel.PrimaryGrid.Columns.Add(this.ExcelForm);
            this.SGC_MEPanel.PrimaryGrid.Columns.Add(this.OutputPath);
            this.SGC_MEPanel.PrimaryGrid.ShowRowHeaders = false;
            this.SGC_MEPanel.Size = new System.Drawing.Size(442, 170);
            this.SGC_MEPanel.TabIndex = 0;
            this.SGC_MEPanel.Text = "superGridControl1";
            // 
            // select
            // 
            this.select.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.select.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridCheckBoxXEditControl);
            this.select.Name = "選擇";
            this.select.Width = 40;
            // 
            // ExcelType
            // 
            this.ExcelType.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.ExcelType.Name = "Excel";
            this.ExcelType.Width = 60;
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
            this.OutputPath.Name = "輸出路徑(桌面)";
            // 
            // Tab_ME
            // 
            this.Tab_ME.AttachedControl = this.superTabControlPanel1;
            this.Tab_ME.GlobalItem = false;
            this.Tab_ME.Image = global::OutputExcelForm.Properties.Resources.files_32px;
            this.Tab_ME.Name = "Tab_ME";
            this.Tab_ME.Text = "ME表單";
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
            this.Op1Combobox.DisplayMember = "Text";
            this.Op1Combobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Op1Combobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Op1Combobox.FormattingEnabled = true;
            this.Op1Combobox.ItemHeight = 16;
            this.Op1Combobox.Location = new System.Drawing.Point(312, 56);
            this.Op1Combobox.Name = "Op1Combobox";
            this.Op1Combobox.Size = new System.Drawing.Size(148, 22);
            this.Op1Combobox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Op1Combobox.TabIndex = 12;
            this.Op1Combobox.SelectedIndexChanged += new System.EventHandler(this.Op1Combobox_SelectedIndexChanged);
            // 
            // OK
            // 
            this.OK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.OK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.OK.Image = global::OutputExcelForm.Properties.Resources.ok_32px;
            this.OK.Location = new System.Drawing.Point(115, 326);
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
            this.Controls.Add(this.Op1Combobox);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.SuperTabControl);
            this.Controls.Add(this.CusVerCombobox);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.PartNoCombobox);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.CusComboBox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelX1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OutputForm";
            this.Text = "輸出表單";
            ((System.ComponentModel.ISupportInitialize)(this.SuperTabControl)).EndInit();
            this.SuperTabControl.ResumeLayout(false);
            this.superTabControlPanel2.ResumeLayout(false);
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
        private DevComponents.DotNetBar.SuperTabControl SuperTabControl;
        private DevComponents.DotNetBar.SuperTabControlPanel superTabControlPanel1;
        private DevComponents.DotNetBar.SuperTabItem Tab_ME;
        private System.Windows.Forms.PictureBox pictureBox4;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.ButtonX OK;
        private DevComponents.DotNetBar.ButtonX Close;
        private DevComponents.DotNetBar.SuperGrid.SuperGridControl SGC_MEPanel;
        private DevComponents.DotNetBar.SuperGrid.GridColumn select;
        private DevComponents.DotNetBar.SuperGrid.GridColumn ExcelType;
        private DevComponents.DotNetBar.SuperGrid.GridColumn ExcelForm;
        private DevComponents.DotNetBar.SuperGrid.GridColumn OutputPath;
        private DevComponents.DotNetBar.SuperTabControlPanel superTabControlPanel2;
        private DevComponents.DotNetBar.SuperTabItem Tab_TE;
        private DevComponents.DotNetBar.SuperGrid.SuperGridControl SGC_TEPanel;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn1;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn2;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn3;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn4;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn5;
        private DevComponents.DotNetBar.Controls.ComboBoxEx PartNoCombobox;
        private DevComponents.DotNetBar.Controls.ComboBoxEx CusVerCombobox;
        private DevComponents.DotNetBar.Controls.ComboBoxEx Op1Combobox;
    }
}

