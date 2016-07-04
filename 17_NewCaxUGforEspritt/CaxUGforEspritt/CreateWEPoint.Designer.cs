namespace CaxUGforEspritt
{
    partial class CreateWEPoint
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public static bool IsManual = false;
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
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn1 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn2 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn3 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn4 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn5 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateWEPoint));
            this.superGridControlCreateWEPt = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.buttonOK_CreateWEPt = new DevComponents.DotNetBar.ButtonX();
            this.buttonCancel_CreateWEPt = new DevComponents.DotNetBar.ButtonX();
            this.styleManager1 = new DevComponents.DotNetBar.StyleManager(this.components);
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.checkBoxX1_manual = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // superGridControlCreateWEPt
            // 
            this.superGridControlCreateWEPt.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.superGridControlCreateWEPt.Location = new System.Drawing.Point(9, 10);
            this.superGridControlCreateWEPt.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.superGridControlCreateWEPt.Name = "superGridControlCreateWEPt";
            gridColumn1.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            gridColumn1.HeaderText = "#";
            gridColumn1.Name = "ListGroupCount";
            gridColumn1.SortIndicator = DevComponents.DotNetBar.SuperGrid.SortIndicator.None;
            gridColumn1.Width = 40;
            gridColumn2.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            gridColumn2.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridComboBoxExEditControl);
            gridColumn2.HeaderText = "類型";
            gridColumn2.Name = "GridWeType";
            gridColumn2.Width = 120;
            gridColumn3.AllowEdit = false;
            gridColumn3.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            gridColumn3.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn3.HeaderText = "X";
            gridColumn3.Name = "X";
            gridColumn3.SortIndicator = DevComponents.DotNetBar.SuperGrid.SortIndicator.None;
            gridColumn4.AllowEdit = false;
            gridColumn4.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            gridColumn4.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn4.HeaderText = "Y";
            gridColumn4.Name = "Y";
            gridColumn4.SortIndicator = DevComponents.DotNetBar.SuperGrid.SortIndicator.None;
            gridColumn5.AutoSizeMode = DevComponents.DotNetBar.SuperGrid.ColumnAutoSizeMode.Fill;
            gridColumn5.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            gridColumn5.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridButtonXEditControl);
            gridColumn5.HeaderText = "佈點區域";
            gridColumn5.Name = "ListWEArea";
            gridColumn5.SortIndicator = DevComponents.DotNetBar.SuperGrid.SortIndicator.None;
            gridColumn5.Width = 120;
            this.superGridControlCreateWEPt.PrimaryGrid.Columns.Add(gridColumn1);
            this.superGridControlCreateWEPt.PrimaryGrid.Columns.Add(gridColumn2);
            this.superGridControlCreateWEPt.PrimaryGrid.Columns.Add(gridColumn3);
            this.superGridControlCreateWEPt.PrimaryGrid.Columns.Add(gridColumn4);
            this.superGridControlCreateWEPt.PrimaryGrid.Columns.Add(gridColumn5);
            this.superGridControlCreateWEPt.PrimaryGrid.DefaultRowHeight = 28;
            this.superGridControlCreateWEPt.PrimaryGrid.MultiSelect = false;
            this.superGridControlCreateWEPt.Size = new System.Drawing.Size(520, 394);
            this.superGridControlCreateWEPt.TabIndex = 0;
            this.superGridControlCreateWEPt.Text = "superGridControl1";
            this.superGridControlCreateWEPt.CellClick += new System.EventHandler<DevComponents.DotNetBar.SuperGrid.GridCellClickEventArgs>(this.superGridControlCreateWEPt_CellClick);
            this.superGridControlCreateWEPt.CellValueChanged += new System.EventHandler<DevComponents.DotNetBar.SuperGrid.GridCellValueChangedEventArgs>(this.superGridControlCreateWEPt_CellValueChanged);
            // 
            // buttonOK_CreateWEPt
            // 
            this.buttonOK_CreateWEPt.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonOK_CreateWEPt.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonOK_CreateWEPt.Location = new System.Drawing.Point(366, 494);
            this.buttonOK_CreateWEPt.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonOK_CreateWEPt.Name = "buttonOK_CreateWEPt";
            this.buttonOK_CreateWEPt.Size = new System.Drawing.Size(80, 53);
            this.buttonOK_CreateWEPt.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonOK_CreateWEPt.TabIndex = 1;
            this.buttonOK_CreateWEPt.Text = "確認";
            this.buttonOK_CreateWEPt.Click += new System.EventHandler(this.buttonOK_CreateWEPt_Click);
            // 
            // buttonCancel_CreateWEPt
            // 
            this.buttonCancel_CreateWEPt.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonCancel_CreateWEPt.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonCancel_CreateWEPt.Location = new System.Drawing.Point(450, 494);
            this.buttonCancel_CreateWEPt.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonCancel_CreateWEPt.Name = "buttonCancel_CreateWEPt";
            this.buttonCancel_CreateWEPt.Size = new System.Drawing.Size(80, 53);
            this.buttonCancel_CreateWEPt.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonCancel_CreateWEPt.TabIndex = 2;
            this.buttonCancel_CreateWEPt.Text = "取消";
            this.buttonCancel_CreateWEPt.Click += new System.EventHandler(this.buttonCancel_CreateWEPt_Click);
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // groupPanel1
            // 
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.checkBoxX1_manual);
            this.groupPanel1.Controls.Add(this.labelX1);
            this.groupPanel1.Location = new System.Drawing.Point(10, 410);
            this.groupPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(519, 80);
            // 
            // 
            // 
            this.groupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel1.Style.BackColorGradientAngle = 90;
            this.groupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderBottomWidth = 1;
            this.groupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderLeftWidth = 1;
            this.groupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderRightWidth = 1;
            this.groupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderTopWidth = 1;
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel1.TabIndex = 4;
            this.groupPanel1.Text = "溫馨提醒";
            // 
            // checkBoxX1_manual
            // 
            // 
            // 
            // 
            this.checkBoxX1_manual.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.checkBoxX1_manual.Location = new System.Drawing.Point(210, 35);
            this.checkBoxX1_manual.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxX1_manual.Name = "checkBoxX1_manual";
            this.checkBoxX1_manual.Size = new System.Drawing.Size(92, 18);
            this.checkBoxX1_manual.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.checkBoxX1_manual.TabIndex = 1;
            this.checkBoxX1_manual.Text = "轉手動任務";
            this.checkBoxX1_manual.CheckedChanged += new System.EventHandler(this.checkBoxX1_manual_CheckedChanged);
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(15, 12);
            this.labelX1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(484, 18);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "當加工區域經人工判斷後發現有誤，表示此任務解析失敗，請勾選\"轉手動任務\"選項";
            this.labelX1.TextAlignment = System.Drawing.StringAlignment.Center;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CaxUGforEspritt.Properties.Resources.附件1_圖文平行;
            this.pictureBox1.Location = new System.Drawing.Point(9, 494);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(352, 53);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // CreateWEPoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 569);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonCancel_CreateWEPt);
            this.Controls.Add(this.buttonOK_CreateWEPt);
            this.Controls.Add(this.superGridControlCreateWEPt);
            this.DoubleBuffered = true;
            this.EnableGlass = false;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "CreateWEPoint";
            this.Text = "手動佈點對話框";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CreateWEPoint_FormClosed);
            this.Load += new System.EventHandler(this.CreateWEPoint_Load);
            this.groupPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.SuperGrid.SuperGridControl superGridControlCreateWEPt;
        private DevComponents.DotNetBar.ButtonX buttonOK_CreateWEPt;
        private DevComponents.DotNetBar.ButtonX buttonCancel_CreateWEPt;
        private DevComponents.DotNetBar.StyleManager styleManager1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private DevComponents.DotNetBar.Controls.CheckBoxX checkBoxX1_manual;
        private DevComponents.DotNetBar.LabelX labelX1;
    }
}