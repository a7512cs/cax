using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using CimforceCaxTwPublic;
using WeData;
namespace CaxUGforEspritt
{
    partial class SelectWorkPart
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public static List<Component> DlgComponent = new List<Component>();
        public static List<Part> DlgPart = new List<Part>();
        public static GridTask sGridTask = new GridTask();
        public Dictionary<WeListKey, WeFaceGroup> WEFaceDict = new Dictionary<WeListKey, WeFaceGroup>();
        public Dictionary<int, string> mdColorDic = new Dictionary<int, string>();
        public string WE_SECTION = "";
        //public Dictionary<string, CaxAsm.AsmCompPart> weCompDic = new Dictionary<string, CaxAsm.AsmCompPart>();

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
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn6 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn7 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectWorkPart));
            this.buttonOK = new DevComponents.DotNetBar.ButtonX();
            this.buttonCancle = new DevComponents.DotNetBar.ButtonX();
            this.superGridMainControl = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.styleManager1 = new DevComponents.DotNetBar.StyleManager(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonOK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonOK.Location = new System.Drawing.Point(689, 274);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(76, 58);
            this.buttonOK.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "確認";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancle
            // 
            this.buttonCancle.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonCancle.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonCancle.Location = new System.Drawing.Point(783, 274);
            this.buttonCancle.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCancle.Name = "buttonCancle";
            this.buttonCancle.Size = new System.Drawing.Size(76, 58);
            this.buttonCancle.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonCancle.TabIndex = 3;
            this.buttonCancle.Text = "取消";
            this.buttonCancle.Click += new System.EventHandler(this.buttonCancle_Click);
            // 
            // superGridMainControl
            // 
            this.superGridMainControl.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.superGridMainControl.Location = new System.Drawing.Point(9, 34);
            this.superGridMainControl.Margin = new System.Windows.Forms.Padding(2);
            this.superGridMainControl.Name = "superGridMainControl";
            gridColumn1.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            gridColumn1.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            gridColumn1.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn1.HeaderText = "#";
            gridColumn1.Name = "ListObjNum";
            gridColumn1.SortIndicator = DevComponents.DotNetBar.SuperGrid.SortIndicator.None;
            gridColumn1.Width = 40;
            gridColumn2.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            gridColumn2.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            gridColumn2.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn2.HeaderText = "狀態";
            gridColumn2.Name = "Status";
            gridColumn2.SortIndicator = DevComponents.DotNetBar.SuperGrid.SortIndicator.None;
            gridColumn2.Width = 70;
            gridColumn3.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            gridColumn3.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn3.HeaderText = "物件名稱";
            gridColumn3.Name = "ListWorkPiece";
            gridColumn3.SortIndicator = DevComponents.DotNetBar.SuperGrid.SortIndicator.None;
            gridColumn3.Width = 330;
            gridColumn4.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            gridColumn4.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            gridColumn4.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn4.HeaderText = "製程";
            gridColumn4.Name = "Process";
            gridColumn4.SortIndicator = DevComponents.DotNetBar.SuperGrid.SortIndicator.None;
            gridColumn5.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            gridColumn5.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            gridColumn5.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn5.HeaderText = "加工面";
            gridColumn5.Name = "WK_Face";
            gridColumn5.SortIndicator = DevComponents.DotNetBar.SuperGrid.SortIndicator.None;
            gridColumn5.Width = 80;
            gridColumn6.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            gridColumn6.ColumnSortMode = DevComponents.DotNetBar.SuperGrid.ColumnSortMode.None;
            gridColumn6.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridButtonXEditControl);
            gridColumn6.HeaderText = "穿線點";
            gridColumn6.Name = "ListWEPoint";
            gridColumn6.SortIndicator = DevComponents.DotNetBar.SuperGrid.SortIndicator.None;
            gridColumn7.AutoSizeMode = DevComponents.DotNetBar.SuperGrid.ColumnAutoSizeMode.Fill;
            gridColumn7.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridButtonXEditControl);
            gridColumn7.HeaderText = "選擇治具";
            gridColumn7.Name = "WeFixture";
            this.superGridMainControl.PrimaryGrid.Columns.Add(gridColumn1);
            this.superGridMainControl.PrimaryGrid.Columns.Add(gridColumn2);
            this.superGridMainControl.PrimaryGrid.Columns.Add(gridColumn3);
            this.superGridMainControl.PrimaryGrid.Columns.Add(gridColumn4);
            this.superGridMainControl.PrimaryGrid.Columns.Add(gridColumn5);
            this.superGridMainControl.PrimaryGrid.Columns.Add(gridColumn6);
            this.superGridMainControl.PrimaryGrid.Columns.Add(gridColumn7);
            this.superGridMainControl.PrimaryGrid.DefaultRowHeight = 28;
            this.superGridMainControl.PrimaryGrid.MultiSelect = false;
            this.superGridMainControl.PrimaryGrid.SelectionGranularity = DevComponents.DotNetBar.SuperGrid.SelectionGranularity.Row;
            this.superGridMainControl.Size = new System.Drawing.Size(850, 223);
            this.superGridMainControl.TabIndex = 1;
            this.superGridMainControl.Text = "superGridControl1";
            this.superGridMainControl.RowClick += new System.EventHandler<DevComponents.DotNetBar.SuperGrid.GridRowClickEventArgs>(this.superGridMainControl_RowClick);
            this.superGridMainControl.Click += new System.EventHandler(this.superGridMainControl_Click);
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CaxUGforEspritt.Properties.Resources.附件1_圖文平行;
            this.pictureBox1.Location = new System.Drawing.Point(9, 274);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(648, 58);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // SelectWorkPart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 341);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.superGridMainControl);
            this.Controls.Add(this.buttonCancle);
            this.Controls.Add(this.buttonOK);
            this.DoubleBuffered = true;
            this.EnableGlass = false;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SelectWorkPart";
            this.Text = "線割零件對話框";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectWorkPart_FormClosing);
            this.Load += new System.EventHandler(this.SelectWorkPart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX buttonOK;
        private DevComponents.DotNetBar.ButtonX buttonCancle;
        public DevComponents.DotNetBar.SuperGrid.SuperGridControl superGridMainControl;
        private DevComponents.DotNetBar.StyleManager styleManager1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}