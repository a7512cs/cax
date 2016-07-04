namespace PostProcessor
{
    partial class PostProcessorDlg
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
            this.comboBoxNCgroup = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SelectAll = new System.Windows.Forms.CheckBox();
            this.SuperGridOperPanel = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.SingleSelect = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.OperName = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.PostProcessor = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.BroserPost = new DevComponents.DotNetBar.ButtonX();
            this.SuperGridPostPanel = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.PostProcessorName = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.Output = new DevComponents.DotNetBar.ButtonX();
            this.CloseDlg = new DevComponents.DotNetBar.ButtonX();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(9, 21);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(75, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "群組名稱：";
            // 
            // comboBoxNCgroup
            // 
            this.comboBoxNCgroup.DisplayMember = "Text";
            this.comboBoxNCgroup.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxNCgroup.FormattingEnabled = true;
            this.comboBoxNCgroup.ItemHeight = 16;
            this.comboBoxNCgroup.Location = new System.Drawing.Point(71, 19);
            this.comboBoxNCgroup.Name = "comboBoxNCgroup";
            this.comboBoxNCgroup.Size = new System.Drawing.Size(84, 22);
            this.comboBoxNCgroup.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.comboBoxNCgroup.TabIndex = 1;
            this.comboBoxNCgroup.SelectedIndexChanged += new System.EventHandler(this.comboBoxNCgroup_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SelectAll);
            this.groupBox1.Controls.Add(this.SuperGridOperPanel);
            this.groupBox1.Controls.Add(this.comboBoxNCgroup);
            this.groupBox1.Controls.Add(this.labelX1);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(243, 339);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "程式";
            // 
            // SelectAll
            // 
            this.SelectAll.AutoSize = true;
            this.SelectAll.Location = new System.Drawing.Point(161, 21);
            this.SelectAll.Name = "SelectAll";
            this.SelectAll.Size = new System.Drawing.Size(72, 16);
            this.SelectAll.TabIndex = 3;
            this.SelectAll.Text = "全部挑選";
            this.SelectAll.UseVisualStyleBackColor = true;
            this.SelectAll.CheckedChanged += new System.EventHandler(this.SelectAll_CheckedChanged);
            // 
            // SuperGridOperPanel
            // 
            this.SuperGridOperPanel.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.SuperGridOperPanel.Location = new System.Drawing.Point(9, 50);
            this.SuperGridOperPanel.Name = "SuperGridOperPanel";
            // 
            // 
            // 
            this.SuperGridOperPanel.PrimaryGrid.Columns.Add(this.SingleSelect);
            this.SuperGridOperPanel.PrimaryGrid.Columns.Add(this.OperName);
            this.SuperGridOperPanel.PrimaryGrid.Columns.Add(this.PostProcessor);
            this.SuperGridOperPanel.PrimaryGrid.MultiSelect = false;
            this.SuperGridOperPanel.Size = new System.Drawing.Size(224, 283);
            this.SuperGridOperPanel.TabIndex = 2;
            this.SuperGridOperPanel.Text = "superGridControl1";
            // 
            // SingleSelect
            // 
            this.SingleSelect.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.SingleSelect.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            this.SingleSelect.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridCheckBoxXEditControl);
            this.SingleSelect.Name = "挑選";
            this.SingleSelect.ResizeMode = DevComponents.DotNetBar.SuperGrid.ColumnResizeMode.None;
            this.SingleSelect.Width = 40;
            // 
            // OperName
            // 
            this.OperName.AutoSizeMode = DevComponents.DotNetBar.SuperGrid.ColumnAutoSizeMode.Fill;
            this.OperName.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.OperName.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            this.OperName.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            this.OperName.Name = "程式名稱";
            this.OperName.ResizeMode = DevComponents.DotNetBar.SuperGrid.ColumnResizeMode.None;
            this.OperName.Width = 60;
            // 
            // PostProcessor
            // 
            this.PostProcessor.AutoSizeMode = DevComponents.DotNetBar.SuperGrid.ColumnAutoSizeMode.Fill;
            this.PostProcessor.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.PostProcessor.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            this.PostProcessor.Name = "後處理器";
            this.PostProcessor.ResizeMode = DevComponents.DotNetBar.SuperGrid.ColumnResizeMode.None;
            this.PostProcessor.Visible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.BroserPost);
            this.groupBox3.Controls.Add(this.SuperGridPostPanel);
            this.groupBox3.Location = new System.Drawing.Point(261, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(306, 339);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "後處理器";
            // 
            // BroserPost
            // 
            this.BroserPost.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.BroserPost.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.BroserPost.Location = new System.Drawing.Point(6, 19);
            this.BroserPost.Name = "BroserPost";
            this.BroserPost.Size = new System.Drawing.Size(292, 25);
            this.BroserPost.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.BroserPost.TabIndex = 2;
            this.BroserPost.Text = "瀏覽後處理器";
            // 
            // SuperGridPostPanel
            // 
            this.SuperGridPostPanel.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.SuperGridPostPanel.Location = new System.Drawing.Point(6, 50);
            this.SuperGridPostPanel.Name = "SuperGridPostPanel";
            // 
            // 
            // 
            this.SuperGridPostPanel.PrimaryGrid.Columns.Add(this.PostProcessorName);
            this.SuperGridPostPanel.PrimaryGrid.MultiSelect = false;
            this.SuperGridPostPanel.PrimaryGrid.SelectionGranularity = DevComponents.DotNetBar.SuperGrid.SelectionGranularity.Row;
            this.SuperGridPostPanel.Size = new System.Drawing.Size(292, 283);
            this.SuperGridPostPanel.TabIndex = 0;
            this.SuperGridPostPanel.Text = "superGridControl2";
            this.SuperGridPostPanel.RowClick += new System.EventHandler<DevComponents.DotNetBar.SuperGrid.GridRowClickEventArgs>(this.SuperGridPostPanel_RowClick);
            // 
            // PostProcessorName
            // 
            this.PostProcessorName.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleLeft;
            this.PostProcessorName.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            this.PostProcessorName.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            this.PostProcessorName.Name = "後處理器名稱";
            this.PostProcessorName.Width = 280;
            // 
            // Output
            // 
            this.Output.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Output.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.Output.Location = new System.Drawing.Point(193, 354);
            this.Output.Name = "Output";
            this.Output.Size = new System.Drawing.Size(75, 23);
            this.Output.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Output.TabIndex = 5;
            this.Output.Text = "確認輸出";
            this.Output.Click += new System.EventHandler(this.Output_Click);
            // 
            // CloseDlg
            // 
            this.CloseDlg.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.CloseDlg.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.CloseDlg.Location = new System.Drawing.Point(288, 354);
            this.CloseDlg.Name = "CloseDlg";
            this.CloseDlg.Size = new System.Drawing.Size(75, 23);
            this.CloseDlg.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.CloseDlg.TabIndex = 6;
            this.CloseDlg.Text = "關閉";
            this.CloseDlg.Click += new System.EventHandler(this.CloseDlg_Click);
            // 
            // PostProcessorDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 389);
            this.Controls.Add(this.CloseDlg);
            this.Controls.Add(this.Output);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.Name = "PostProcessorDlg";
            this.Text = "PostProcessorDlg";
            this.Load += new System.EventHandler(this.PostProcessorDlg_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.StyleManager styleManager1;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx comboBoxNCgroup;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevComponents.DotNetBar.SuperGrid.SuperGridControl SuperGridOperPanel;
        private DevComponents.DotNetBar.SuperGrid.GridColumn OperName;
        private DevComponents.DotNetBar.SuperGrid.GridColumn PostProcessor;
        private System.Windows.Forms.GroupBox groupBox3;
        private DevComponents.DotNetBar.ButtonX BroserPost;
        private DevComponents.DotNetBar.SuperGrid.SuperGridControl SuperGridPostPanel;
        private DevComponents.DotNetBar.SuperGrid.GridColumn PostProcessorName;
        private DevComponents.DotNetBar.ButtonX Output;
        private DevComponents.DotNetBar.ButtonX CloseDlg;
        private System.Windows.Forms.CheckBox SelectAll;
        private DevComponents.DotNetBar.SuperGrid.GridColumn SingleSelect;
    }
}