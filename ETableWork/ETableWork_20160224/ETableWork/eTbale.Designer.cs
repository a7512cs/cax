using System.Collections;
using CimforceCaxTwPublic;
using System.Collections.Generic;
using NXOpen.Annotations;

namespace ETableWork
{
    partial class eTbale
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

        private string SHOPDOC_OPER_NAME = "Oper_Tool_Cimforce";
        private string ATTR_CATEGORY = "CIMFORCE";

        public string asmName;
        public string ROOT_PATH;
        public CimforceCaxTwCNC.MesAttrCNC sMesDatData;
        public ExportWorkTabel sExportWorkTabel;
        public CaxAsm.CimAsmCompPart sCimAsmCompPart;
        public List<ListToolLengeh> ListToolLengehAry;
        public double[] minDesignBodyWcs;
        public double[] maxDesignBodyWcs;
//         public List<CaxPart.BaseCorner> baseCornerAry;
        public baseFace sBaseFaces;
        public string section_face = "";
        public string baseHoleName = "";
        public string beforeCNCName = "";
        public string afterCNCName = "";
        public bool isMultiFixture = false;
        public string companyName = "";
        public bool hasCMM = false;
        public bool hasMultiFixture = false;
        public BaseNote elecPartNoNote;
        public List<CaxAsm.CompPart> fixtureLst;
        public List<NXOpen.Assemblies.Component> subDesignCompLst;
        //public CimforceCaxTwCNC.BladeLength cBladeLength;
        //public Program.WorkPieceElecTaskKey sWorkPieceElecTaskKey;
        public BaseDist sBaseDist;  // 20150817 基準面距離
        public ClampCalibrateParam clampCalibParam;  // 20151013 判斷是否重新裝夾/校正之參數

        public string support_machine = "";
        public string machine_type = "";
        public string controller = "";
        public string machine_group = "";
        public string postFunction = "";
        public double BOTTOM_Z;
        public string CLEARANE_PLANE;   // 20150721 安全高度

        //public ArrayList operationAry = new ArrayList();
        //public ArrayList postListAry = new ArrayList();

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn9 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn10 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn11 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn12 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn13 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn14 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn15 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn16 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(eTbale));
            this.buttonXViewSetting = new DevComponents.DotNetBar.ButtonX();
            this.labelView = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxNote = new System.Windows.Forms.TextBox();
            this.checkBoxIS_STL = new System.Windows.Forms.CheckBox();
            this.checkBoxIS_SHOPDOC = new System.Windows.Forms.CheckBox();
            this.checkBoxIS_POST = new System.Windows.Forms.CheckBox();
            this.styleManager1 = new DevComponents.DotNetBar.StyleManager(this.components);
            this.superGridControlOper = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonXCancel = new DevComponents.DotNetBar.ButtonX();
            this.buttonXOK = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.checkBoxIS_CMM = new System.Windows.Forms.CheckBox();
            this.groupPanel2 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.labelXFixture = new DevComponents.DotNetBar.LabelX();
            this.labelXWorkSection = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.line1 = new DevComponents.DotNetBar.Controls.Line();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupPanel1.SuspendLayout();
            this.groupPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonXViewSetting
            // 
            this.buttonXViewSetting.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonXViewSetting.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonXViewSetting.Location = new System.Drawing.Point(78, 40);
            this.buttonXViewSetting.Name = "buttonXViewSetting";
            this.buttonXViewSetting.Size = new System.Drawing.Size(75, 25);
            this.buttonXViewSetting.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonXViewSetting.TabIndex = 13;
            this.buttonXViewSetting.Text = "設定";
            this.buttonXViewSetting.Click += new System.EventHandler(this.buttonXViewSetting_Click);
            // 
            // labelView
            // 
            this.labelView.AutoSize = true;
            this.labelView.BackColor = System.Drawing.Color.Transparent;
            this.labelView.ForeColor = System.Drawing.Color.Red;
            this.labelView.Location = new System.Drawing.Point(171, 45);
            this.labelView.Name = "labelView";
            this.labelView.Size = new System.Drawing.Size(52, 15);
            this.labelView.TabIndex = 12;
            this.labelView.Text = "未定義";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(5, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "截圖視角";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(6, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "備註";
            // 
            // textBoxNote
            // 
            this.textBoxNote.Location = new System.Drawing.Point(49, 78);
            this.textBoxNote.Name = "textBoxNote";
            this.textBoxNote.Size = new System.Drawing.Size(685, 25);
            this.textBoxNote.TabIndex = 8;
            // 
            // checkBoxIS_STL
            // 
            this.checkBoxIS_STL.AutoSize = true;
            this.checkBoxIS_STL.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxIS_STL.ForeColor = System.Drawing.Color.Red;
            this.checkBoxIS_STL.Location = new System.Drawing.Point(222, 7);
            this.checkBoxIS_STL.Name = "checkBoxIS_STL";
            this.checkBoxIS_STL.Size = new System.Drawing.Size(122, 20);
            this.checkBoxIS_STL.TabIndex = 3;
            this.checkBoxIS_STL.Text = "STL 模擬檔案";
            this.checkBoxIS_STL.UseVisualStyleBackColor = true;
            // 
            // checkBoxIS_SHOPDOC
            // 
            this.checkBoxIS_SHOPDOC.AutoSize = true;
            this.checkBoxIS_SHOPDOC.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxIS_SHOPDOC.Location = new System.Drawing.Point(98, 7);
            this.checkBoxIS_SHOPDOC.Name = "checkBoxIS_SHOPDOC";
            this.checkBoxIS_SHOPDOC.Size = new System.Drawing.Size(88, 20);
            this.checkBoxIS_SHOPDOC.TabIndex = 2;
            this.checkBoxIS_SHOPDOC.Text = "Shop Doc";
            this.checkBoxIS_SHOPDOC.UseVisualStyleBackColor = true;
            // 
            // checkBoxIS_POST
            // 
            this.checkBoxIS_POST.AutoSize = true;
            this.checkBoxIS_POST.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxIS_POST.Location = new System.Drawing.Point(9, 7);
            this.checkBoxIS_POST.Name = "checkBoxIS_POST";
            this.checkBoxIS_POST.Size = new System.Drawing.Size(67, 20);
            this.checkBoxIS_POST.TabIndex = 1;
            this.checkBoxIS_POST.Text = "POST";
            this.checkBoxIS_POST.UseVisualStyleBackColor = true;
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // superGridControlOper
            // 
            this.superGridControlOper.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.superGridControlOper.Location = new System.Drawing.Point(11, 253);
            this.superGridControlOper.Name = "superGridControlOper";
            this.superGridControlOper.PrimaryGrid.ColumnHeader.RowHeaderText = "";
            this.superGridControlOper.PrimaryGrid.ColumnHeader.RowHeight = 30;
            gridColumn9.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            gridColumn9.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn9.HeaderText = "#.";
            gridColumn9.Name = "GridColumnIndex";
            gridColumn9.Width = 40;
            gridColumn10.DataPropertyName = "";
            gridColumn10.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridImageEditControl);
            gridColumn10.HeaderText = "狀態";
            gridColumn10.Name = "GridColumnStatus";
            gridColumn10.Width = 70;
            gridColumn11.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn11.HeaderText = "刀具名稱";
            gridColumn11.Name = "GridColumnToolName";
            gridColumn11.Width = 150;
            gridColumn12.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn12.HeaderText = "伸出長";
            gridColumn12.Name = "GridColumnToolL";
            gridColumn13.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn13.HeaderText = "Opreation";
            gridColumn13.Name = "GridColumnOper";
            gridColumn13.Width = 160;
            gridColumn14.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleRight;
            gridColumn14.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn14.HeaderText = "Gap";
            gridColumn14.Name = "GridColumnGap";
            gridColumn14.Width = 70;
            gridColumn15.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleRight;
            gridColumn15.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn15.HeaderText = "切削長";
            gridColumn15.Name = "GridColumnCuttingL";
            gridColumn16.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleRight;
            gridColumn16.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridLabelXEditControl);
            gridColumn16.HeaderText = "最大切削長";
            gridColumn16.Name = "GridColumnCuttingMaxL";
            gridColumn16.Width = 110;
            this.superGridControlOper.PrimaryGrid.Columns.Add(gridColumn9);
            this.superGridControlOper.PrimaryGrid.Columns.Add(gridColumn10);
            this.superGridControlOper.PrimaryGrid.Columns.Add(gridColumn11);
            this.superGridControlOper.PrimaryGrid.Columns.Add(gridColumn12);
            this.superGridControlOper.PrimaryGrid.Columns.Add(gridColumn13);
            this.superGridControlOper.PrimaryGrid.Columns.Add(gridColumn14);
            this.superGridControlOper.PrimaryGrid.Columns.Add(gridColumn15);
            this.superGridControlOper.PrimaryGrid.Columns.Add(gridColumn16);
            this.superGridControlOper.PrimaryGrid.GroupHeaderHeight = 25;
            this.superGridControlOper.PrimaryGrid.ShowRowHeaders = false;
            this.superGridControlOper.Size = new System.Drawing.Size(742, 278);
            this.superGridControlOper.TabIndex = 6;
            this.superGridControlOper.Text = "superGridControl";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "no");
            this.imageList1.Images.SetKeyName(1, "yes");
            this.imageList1.Images.SetKeyName(2, "exclamation");
            // 
            // buttonXCancel
            // 
            this.buttonXCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonXCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonXCancel.Location = new System.Drawing.Point(645, 595);
            this.buttonXCancel.Name = "buttonXCancel";
            this.buttonXCancel.Size = new System.Drawing.Size(108, 38);
            this.buttonXCancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonXCancel.TabIndex = 7;
            this.buttonXCancel.Text = "取消";
            this.buttonXCancel.Click += new System.EventHandler(this.buttonXCancel_Click);
            // 
            // buttonXOK
            // 
            this.buttonXOK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonXOK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonXOK.Location = new System.Drawing.Point(531, 595);
            this.buttonXOK.Name = "buttonXOK";
            this.buttonXOK.Size = new System.Drawing.Size(108, 38);
            this.buttonXOK.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonXOK.TabIndex = 8;
            this.buttonXOK.Text = "確定";
            this.buttonXOK.Click += new System.EventHandler(this.buttonXOK_Click);
            // 
            // groupPanel1
            // 
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.checkBoxIS_CMM);
            this.groupPanel1.Controls.Add(this.label3);
            this.groupPanel1.Controls.Add(this.textBoxNote);
            this.groupPanel1.Controls.Add(this.buttonXViewSetting);
            this.groupPanel1.Controls.Add(this.labelView);
            this.groupPanel1.Controls.Add(this.checkBoxIS_POST);
            this.groupPanel1.Controls.Add(this.label4);
            this.groupPanel1.Controls.Add(this.checkBoxIS_SHOPDOC);
            this.groupPanel1.Controls.Add(this.checkBoxIS_STL);
            this.groupPanel1.Location = new System.Drawing.Point(10, 110);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(743, 137);
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
            this.groupPanel1.TabIndex = 9;
            this.groupPanel1.Text = "輸出設定";
            // 
            // checkBoxIS_CMM
            // 
            this.checkBoxIS_CMM.AutoSize = true;
            this.checkBoxIS_CMM.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxIS_CMM.Location = new System.Drawing.Point(360, 7);
            this.checkBoxIS_CMM.Name = "checkBoxIS_CMM";
            this.checkBoxIS_CMM.Size = new System.Drawing.Size(67, 20);
            this.checkBoxIS_CMM.TabIndex = 15;
            this.checkBoxIS_CMM.Text = "CMM";
            this.checkBoxIS_CMM.UseVisualStyleBackColor = true;
            // 
            // groupPanel2
            // 
            this.groupPanel2.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel2.Controls.Add(this.labelXFixture);
            this.groupPanel2.Controls.Add(this.labelXWorkSection);
            this.groupPanel2.Controls.Add(this.labelX1);
            this.groupPanel2.Controls.Add(this.labelX2);
            this.groupPanel2.Location = new System.Drawing.Point(10, 12);
            this.groupPanel2.Name = "groupPanel2";
            this.groupPanel2.Size = new System.Drawing.Size(743, 92);
            // 
            // 
            // 
            this.groupPanel2.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel2.Style.BackColorGradientAngle = 90;
            this.groupPanel2.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel2.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderBottomWidth = 1;
            this.groupPanel2.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel2.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderLeftWidth = 1;
            this.groupPanel2.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderRightWidth = 1;
            this.groupPanel2.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderTopWidth = 1;
            this.groupPanel2.Style.CornerDiameter = 4;
            this.groupPanel2.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel2.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.groupPanel2.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel2.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel2.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel2.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel2.TabIndex = 10;
            this.groupPanel2.Text = "任務訊息";
            // 
            // labelXFixture
            // 
            this.labelXFixture.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelXFixture.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelXFixture.ForeColor = System.Drawing.Color.Blue;
            this.labelXFixture.Location = new System.Drawing.Point(86, 34);
            this.labelXFixture.Name = "labelXFixture";
            this.labelXFixture.Size = new System.Drawing.Size(648, 23);
            this.labelXFixture.TabIndex = 11;
            this.labelXFixture.Text = "labelXFixture";
            // 
            // labelXWorkSection
            // 
            this.labelXWorkSection.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelXWorkSection.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelXWorkSection.ForeColor = System.Drawing.Color.Blue;
            this.labelXWorkSection.Location = new System.Drawing.Point(50, 4);
            this.labelXWorkSection.Name = "labelXWorkSection";
            this.labelXWorkSection.Size = new System.Drawing.Size(536, 23);
            this.labelXWorkSection.TabIndex = 9;
            this.labelXWorkSection.Text = "labelXWorkSection";
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(4, 4);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(52, 23);
            this.labelX1.TabIndex = 8;
            this.labelX1.Text = "工段:";
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(4, 34);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(86, 23);
            this.labelX2.TabIndex = 10;
            this.labelX2.Text = "治具定義:";
            // 
            // labelX3
            // 
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(656, 537);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(97, 23);
            this.labelX3.TabIndex = 11;
            this.labelX3.Text = "單位：mm";
            // 
            // line1
            // 
            this.line1.Location = new System.Drawing.Point(10, 566);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(743, 23);
            this.line1.TabIndex = 12;
            this.line1.Text = "line1";
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.Image = global::ETableWork.Properties.Resources.exclamation_mark_red_icon16;
            this.pictureBox3.Location = new System.Drawing.Point(304, 537);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(20, 20);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox3.TabIndex = 13;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = global::ETableWork.Properties.Resources.no_16;
            this.pictureBox2.Location = new System.Drawing.Point(170, 537);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(20, 20);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::ETableWork.Properties.Resources.yes_16;
            this.pictureBox1.Location = new System.Drawing.Point(10, 537);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(20, 20);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 540);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 15);
            this.label1.TabIndex = 14;
            this.label1.Text = "符合刀具切削長";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(196, 540);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 14;
            this.label2.Text = "切削長過長";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(330, 540);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(187, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "不符合刀具切削長，需換刀";
            // 
            // eTbale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 645);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.line1);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.buttonXOK);
            this.Controls.Add(this.buttonXCancel);
            this.Controls.Add(this.superGridControlOper);
            this.Controls.Add(this.groupPanel2);
            this.DoubleBuffered = true;
            this.EnableGlass = false;
            this.MaximizeBox = false;
            this.Name = "eTbale";
            this.Text = "輸出工單";
            this.Load += new System.EventHandler(this.eTbale_Load);
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel1.PerformLayout();
            this.groupPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxIS_STL;
        private System.Windows.Forms.CheckBox checkBoxIS_SHOPDOC;
        private System.Windows.Forms.CheckBox checkBoxIS_POST;
        private System.Windows.Forms.Label labelView;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxNote;
        private DevComponents.DotNetBar.StyleManager styleManager1;
        private DevComponents.DotNetBar.SuperGrid.SuperGridControl superGridControlOper;
        private System.Windows.Forms.ImageList imageList1;
        private DevComponents.DotNetBar.ButtonX buttonXViewSetting;
        private DevComponents.DotNetBar.ButtonX buttonXCancel;
        private DevComponents.DotNetBar.ButtonX buttonXOK;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel2;
        public DevComponents.DotNetBar.LabelX labelXWorkSection;
        private DevComponents.DotNetBar.LabelX labelX1;
        public DevComponents.DotNetBar.LabelX labelXFixture;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.Line line1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBoxIS_CMM;
    }
}