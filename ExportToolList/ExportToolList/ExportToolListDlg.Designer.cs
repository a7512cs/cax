namespace ExportToolList
{
    partial class ExportToolListDlg
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
            this.SGC = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.gridColumn1 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn2 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn3 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.OK = new DevComponents.DotNetBar.ButtonX();
            this.Cancel = new DevComponents.DotNetBar.ButtonX();
            this.comboBoxNCName = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.gridColumn4 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
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
            this.labelX1.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX1.Location = new System.Drawing.Point(12, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(111, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "群組名稱：";
            // 
            // SGC
            // 
            this.SGC.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.SGC.Location = new System.Drawing.Point(12, 50);
            this.SGC.Name = "SGC";
            // 
            // 
            // 
            this.SGC.PrimaryGrid.Columns.Add(this.gridColumn1);
            this.SGC.PrimaryGrid.Columns.Add(this.gridColumn2);
            this.SGC.PrimaryGrid.Columns.Add(this.gridColumn3);
            this.SGC.PrimaryGrid.Columns.Add(this.gridColumn4);
            this.SGC.PrimaryGrid.ShowRowHeaders = false;
            this.SGC.Size = new System.Drawing.Size(494, 249);
            this.SGC.TabIndex = 2;
            this.SGC.Text = "superGridControl1";
            // 
            // gridColumn1
            // 
            this.gridColumn1.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            this.gridColumn1.HeaderText = "OP名稱";
            this.gridColumn1.Name = "OP名稱";
            this.gridColumn1.Width = 60;
            // 
            // gridColumn2
            // 
            this.gridColumn2.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.gridColumn2.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            this.gridColumn2.HeaderText = "刀號";
            this.gridColumn2.Name = "刀號";
            this.gridColumn2.Width = 40;
            // 
            // gridColumn3
            // 
            this.gridColumn3.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            this.gridColumn3.HeaderText = "刀具名稱";
            this.gridColumn3.Name = "刀具名稱";
            this.gridColumn3.Width = 200;
            // 
            // OK
            // 
            this.OK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.OK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.OK.Font = new System.Drawing.Font("標楷體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.OK.Location = new System.Drawing.Point(165, 314);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.OK.TabIndex = 3;
            this.OK.Text = "輸出";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Cancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.Cancel.Font = new System.Drawing.Font("標楷體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Cancel.Location = new System.Drawing.Point(267, 314);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Cancel.TabIndex = 4;
            this.Cancel.Text = "關閉";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // comboBoxNCName
            // 
            this.comboBoxNCName.DisplayMember = "Text";
            this.comboBoxNCName.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxNCName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNCName.FormattingEnabled = true;
            this.comboBoxNCName.ItemHeight = 16;
            this.comboBoxNCName.Location = new System.Drawing.Point(96, 12);
            this.comboBoxNCName.Name = "comboBoxNCName";
            this.comboBoxNCName.Size = new System.Drawing.Size(121, 22);
            this.comboBoxNCName.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.comboBoxNCName.TabIndex = 5;
            this.comboBoxNCName.SelectedIndexChanged += new System.EventHandler(this.comboBoxNCName_SelectedIndexChanged);
            // 
            // gridColumn4
            // 
            this.gridColumn4.AutoSizeMode = DevComponents.DotNetBar.SuperGrid.ColumnAutoSizeMode.Fill;
            this.gridColumn4.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            this.gridColumn4.Name = "刀桿名稱";
            // 
            // ExportToolListDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 349);
            this.Controls.Add(this.comboBoxNCName);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.SGC);
            this.Controls.Add(this.labelX1);
            this.DoubleBuffered = true;
            this.Name = "ExportToolListDlg";
            this.Text = "ExportToolListDlg";
            this.Load += new System.EventHandler(this.ExportToolListDlg_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.StyleManager styleManager1;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.SuperGrid.SuperGridControl SGC;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn1;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn2;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn3;
        private DevComponents.DotNetBar.ButtonX OK;
        private DevComponents.DotNetBar.ButtonX Cancel;
        private DevComponents.DotNetBar.Controls.ComboBoxEx comboBoxNCName;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn4;
    }
}