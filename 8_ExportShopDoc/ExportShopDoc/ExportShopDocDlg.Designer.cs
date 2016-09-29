namespace ExportShopDoc
{
    partial class ExportShopDocDlg
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
            this.buttonSelePath = new DevComponents.DotNetBar.ButtonX();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.OutputPath = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ConfirmRename = new DevComponents.DotNetBar.ButtonX();
            this.superGridProg = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.gridColumn1 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn2 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn8 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn3 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn4 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn6 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn7 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn9 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn5 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn10 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.comboBoxNCgroup = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.ExportExcel = new DevComponents.DotNetBar.ButtonX();
            this.CloseDlg = new DevComponents.DotNetBar.ButtonX();
            this.GroupSaveView = new DevComponents.DotNetBar.ButtonX();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.FixturePath = new System.Windows.Forms.TextBox();
            this.SelFixtuePath = new DevComponents.DotNetBar.ButtonX();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // buttonSelePath
            // 
            this.buttonSelePath.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonSelePath.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonSelePath.Location = new System.Drawing.Point(221, 21);
            this.buttonSelePath.Name = "buttonSelePath";
            this.buttonSelePath.Size = new System.Drawing.Size(47, 22);
            this.buttonSelePath.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonSelePath.TabIndex = 2;
            this.buttonSelePath.Text = "瀏覽";
            this.buttonSelePath.Click += new System.EventHandler(this.buttonSelePath_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.OutputPath);
            this.groupBox1.Controls.Add(this.buttonSelePath);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(279, 55);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "刀具路徑與清單輸出路徑";
            // 
            // OutputPath
            // 
            this.OutputPath.Location = new System.Drawing.Point(6, 21);
            this.OutputPath.Name = "OutputPath";
            this.OutputPath.Size = new System.Drawing.Size(207, 22);
            this.OutputPath.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ConfirmRename);
            this.groupBox2.Controls.Add(this.superGridProg);
            this.groupBox2.Controls.Add(this.comboBoxNCgroup);
            this.groupBox2.Controls.Add(this.labelX1);
            this.groupBox2.Location = new System.Drawing.Point(12, 134);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(279, 381);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "程式更名";
            // 
            // ConfirmRename
            // 
            this.ConfirmRename.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.ConfirmRename.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.ConfirmRename.Location = new System.Drawing.Point(201, 17);
            this.ConfirmRename.Name = "ConfirmRename";
            this.ConfirmRename.Size = new System.Drawing.Size(67, 23);
            this.ConfirmRename.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ConfirmRename.TabIndex = 5;
            this.ConfirmRename.Text = "確認更名";
            this.ConfirmRename.Click += new System.EventHandler(this.ConfirmRename_Click);
            // 
            // superGridProg
            // 
            this.superGridProg.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.superGridProg.Location = new System.Drawing.Point(6, 46);
            this.superGridProg.Name = "superGridProg";
            // 
            // 
            // 
            this.superGridProg.PrimaryGrid.ColumnDragBehavior = DevComponents.DotNetBar.SuperGrid.ColumnDragBehavior.None;
            this.superGridProg.PrimaryGrid.Columns.Add(this.gridColumn1);
            this.superGridProg.PrimaryGrid.Columns.Add(this.gridColumn2);
            this.superGridProg.PrimaryGrid.Columns.Add(this.gridColumn8);
            this.superGridProg.PrimaryGrid.Columns.Add(this.gridColumn3);
            this.superGridProg.PrimaryGrid.Columns.Add(this.gridColumn4);
            this.superGridProg.PrimaryGrid.Columns.Add(this.gridColumn6);
            this.superGridProg.PrimaryGrid.Columns.Add(this.gridColumn7);
            this.superGridProg.PrimaryGrid.Columns.Add(this.gridColumn9);
            this.superGridProg.PrimaryGrid.Columns.Add(this.gridColumn5);
            this.superGridProg.PrimaryGrid.Columns.Add(this.gridColumn10);
            this.superGridProg.PrimaryGrid.SelectionGranularity = DevComponents.DotNetBar.SuperGrid.SelectionGranularity.Row;
            this.superGridProg.Size = new System.Drawing.Size(262, 325);
            this.superGridProg.TabIndex = 4;
            this.superGridProg.Text = "superGridControl1";
            this.superGridProg.RowClick += new System.EventHandler<DevComponents.DotNetBar.SuperGrid.GridRowClickEventArgs>(this.superGridProg_RowClick);
            this.superGridProg.RowMouseUp += new System.EventHandler<DevComponents.DotNetBar.SuperGrid.GridRowMouseEventArgs>(this.superGridProg_RowMouseUp);
            // 
            // gridColumn1
            // 
            this.gridColumn1.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            this.gridColumn1.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            this.gridColumn1.HeaderText = "更名前";
            this.gridColumn1.Name = "更名前";
            this.gridColumn1.Width = 80;
            // 
            // gridColumn2
            // 
            this.gridColumn2.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            this.gridColumn2.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            this.gridColumn2.HeaderText = "更名後";
            this.gridColumn2.Name = "更名後";
            this.gridColumn2.Width = 80;
            // 
            // gridColumn8
            // 
            this.gridColumn8.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            this.gridColumn8.HeaderText = "刀號";
            this.gridColumn8.Name = "刀號";
            this.gridColumn8.Visible = false;
            this.gridColumn8.Width = 40;
            // 
            // gridColumn3
            // 
            this.gridColumn3.HeaderText = "刀具名稱";
            this.gridColumn3.Name = "刀具名稱";
            this.gridColumn3.Visible = false;
            this.gridColumn3.Width = 170;
            // 
            // gridColumn4
            // 
            this.gridColumn4.HeaderText = "刀柄名稱";
            this.gridColumn4.Name = "刀柄名稱";
            this.gridColumn4.Visible = false;
            this.gridColumn4.Width = 110;
            // 
            // gridColumn6
            // 
            this.gridColumn6.HeaderText = "加工長度";
            this.gridColumn6.Name = "加工長度";
            this.gridColumn6.Visible = false;
            this.gridColumn6.Width = 60;
            // 
            // gridColumn7
            // 
            this.gridColumn7.HeaderText = "進給";
            this.gridColumn7.Name = "進給";
            this.gridColumn7.Visible = false;
            this.gridColumn7.Width = 50;
            // 
            // gridColumn9
            // 
            this.gridColumn9.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            this.gridColumn9.HeaderText = "轉速";
            this.gridColumn9.Name = "轉速";
            this.gridColumn9.Visible = false;
            this.gridColumn9.Width = 50;
            // 
            // gridColumn5
            // 
            this.gridColumn5.HeaderText = "加工時間";
            this.gridColumn5.Name = "加工時間";
            this.gridColumn5.Visible = false;
            this.gridColumn5.Width = 70;
            // 
            // gridColumn10
            // 
            this.gridColumn10.AutoSizeMode = DevComponents.DotNetBar.SuperGrid.ColumnAutoSizeMode.Fill;
            this.gridColumn10.CellStyles.Default.TextColor = System.Drawing.Color.White;
            this.gridColumn10.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            this.gridColumn10.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridButtonXEditControl);
            this.gridColumn10.HeaderText = "拍照";
            this.gridColumn10.Name = "拍照";
            // 
            // comboBoxNCgroup
            // 
            this.comboBoxNCgroup.DisplayMember = "Text";
            this.comboBoxNCgroup.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxNCgroup.FormattingEnabled = true;
            this.comboBoxNCgroup.ItemHeight = 16;
            this.comboBoxNCgroup.Location = new System.Drawing.Point(74, 18);
            this.comboBoxNCgroup.Name = "comboBoxNCgroup";
            this.comboBoxNCgroup.Size = new System.Drawing.Size(121, 22);
            this.comboBoxNCgroup.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.comboBoxNCgroup.TabIndex = 1;
            this.comboBoxNCgroup.SelectedIndexChanged += new System.EventHandler(this.comboBoxNCgroup_SelectedIndexChanged);
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(8, 21);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(75, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "群組名稱：";
            // 
            // ExportExcel
            // 
            this.ExportExcel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.ExportExcel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.ExportExcel.Location = new System.Drawing.Point(132, 521);
            this.ExportExcel.Name = "ExportExcel";
            this.ExportExcel.Size = new System.Drawing.Size(75, 23);
            this.ExportExcel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ExportExcel.TabIndex = 6;
            this.ExportExcel.Text = "輸出工單";
            this.ExportExcel.Click += new System.EventHandler(this.ExportExcel_Click);
            // 
            // CloseDlg
            // 
            this.CloseDlg.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.CloseDlg.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.CloseDlg.Location = new System.Drawing.Point(216, 521);
            this.CloseDlg.Name = "CloseDlg";
            this.CloseDlg.Size = new System.Drawing.Size(75, 23);
            this.CloseDlg.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.CloseDlg.TabIndex = 7;
            this.CloseDlg.Text = "關閉";
            this.CloseDlg.Click += new System.EventHandler(this.CloseDlg_Click);
            // 
            // GroupSaveView
            // 
            this.GroupSaveView.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.GroupSaveView.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.GroupSaveView.Location = new System.Drawing.Point(18, 521);
            this.GroupSaveView.Name = "GroupSaveView";
            this.GroupSaveView.Size = new System.Drawing.Size(75, 23);
            this.GroupSaveView.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.GroupSaveView.TabIndex = 8;
            this.GroupSaveView.Text = "群組拍照";
            this.GroupSaveView.Click += new System.EventHandler(this.GroupSaveView_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.FixturePath);
            this.groupBox3.Controls.Add(this.SelFixtuePath);
            this.groupBox3.Location = new System.Drawing.Point(12, 73);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(279, 55);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "治具圖片路徑";
            // 
            // FixturePath
            // 
            this.FixturePath.Location = new System.Drawing.Point(6, 21);
            this.FixturePath.Name = "FixturePath";
            this.FixturePath.Size = new System.Drawing.Size(207, 22);
            this.FixturePath.TabIndex = 1;
            // 
            // SelFixtuePath
            // 
            this.SelFixtuePath.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.SelFixtuePath.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.SelFixtuePath.Location = new System.Drawing.Point(221, 21);
            this.SelFixtuePath.Name = "SelFixtuePath";
            this.SelFixtuePath.Size = new System.Drawing.Size(47, 22);
            this.SelFixtuePath.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.SelFixtuePath.TabIndex = 0;
            this.SelFixtuePath.Text = "瀏覽";
            this.SelFixtuePath.Click += new System.EventHandler(this.SelFixtuePath_Click);
            // 
            // ExportShopDocDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 550);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.GroupSaveView);
            this.Controls.Add(this.CloseDlg);
            this.Controls.Add(this.ExportExcel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.Name = "ExportShopDocDlg";
            this.Text = " ";
            this.Load += new System.EventHandler(this.ExportShopDocDlg_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.StyleManager styleManager1;
        private DevComponents.DotNetBar.ButtonX buttonSelePath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox OutputPath;
        private System.Windows.Forms.GroupBox groupBox2;
        private DevComponents.DotNetBar.SuperGrid.SuperGridControl superGridProg;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn1;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx comboBoxNCgroup;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.ButtonX ConfirmRename;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn3;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn4;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn5;
        private DevComponents.DotNetBar.ButtonX ExportExcel;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn6;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn7;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn8;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn9;
        private DevComponents.DotNetBar.ButtonX CloseDlg;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn10;
        private DevComponents.DotNetBar.ButtonX GroupSaveView;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox FixturePath;
        private DevComponents.DotNetBar.ButtonX SelFixtuePath;
    }
}