namespace AssignGauge
{
    partial class AssignGaugeDlg
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
            this.SelfCheckGauge = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.SameIPQC = new System.Windows.Forms.CheckBox();
            this.SelfCheck_Units = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.IPQC_Units = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.SelfCheck_1 = new System.Windows.Forms.TextBox();
            this.SelfCheck_0 = new System.Windows.Forms.TextBox();
            this.IPQC_1 = new System.Windows.Forms.TextBox();
            this.IPQC_0 = new System.Windows.Forms.TextBox();
            this.IPQCGauge = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX6 = new DevComponents.DotNetBar.LabelX();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SelectObject = new DevComponents.DotNetBar.ButtonX();
            this.chb_Remove = new System.Windows.Forms.CheckBox();
            this.chb_Assign = new System.Windows.Forms.CheckBox();
            this.ListSheet = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.OK = new DevComponents.DotNetBar.ButtonX();
            this.Cancel = new DevComponents.DotNetBar.ButtonX();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.IQC_Units = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.IQC_1 = new System.Windows.Forms.TextBox();
            this.IQC_0 = new System.Windows.Forms.TextBox();
            this.IQCGauge = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX7 = new DevComponents.DotNetBar.LabelX();
            this.IQCcheckBox = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.IPQCcheckBox = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.FAIcheckBox = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.FQCcheckBox = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // SelfCheckGauge
            // 
            this.SelfCheckGauge.DisplayMember = "Text";
            this.SelfCheckGauge.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.SelfCheckGauge.DropDownHeight = 220;
            this.SelfCheckGauge.FormattingEnabled = true;
            this.SelfCheckGauge.IntegralHeight = false;
            this.SelfCheckGauge.ItemHeight = 16;
            this.SelfCheckGauge.Location = new System.Drawing.Point(8, 212);
            this.SelfCheckGauge.Name = "SelfCheckGauge";
            this.SelfCheckGauge.Size = new System.Drawing.Size(230, 22);
            this.SelfCheckGauge.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.SelfCheckGauge.TabIndex = 13;
            // 
            // SameIPQC
            // 
            this.SameIPQC.AutoSize = true;
            this.SameIPQC.Location = new System.Drawing.Point(77, 190);
            this.SameIPQC.Name = "SameIPQC";
            this.SameIPQC.Size = new System.Drawing.Size(62, 16);
            this.SameIPQC.TabIndex = 12;
            this.SameIPQC.Text = "同IPQC";
            this.SameIPQC.UseVisualStyleBackColor = true;
            this.SameIPQC.CheckedChanged += new System.EventHandler(this.SameIPQC_CheckedChanged);
            // 
            // SelfCheck_Units
            // 
            this.SelfCheck_Units.DisplayMember = "Text";
            this.SelfCheck_Units.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.SelfCheck_Units.FormattingEnabled = true;
            this.SelfCheck_Units.ItemHeight = 16;
            this.SelfCheck_Units.Location = new System.Drawing.Point(128, 240);
            this.SelfCheck_Units.Name = "SelfCheck_Units";
            this.SelfCheck_Units.Size = new System.Drawing.Size(110, 22);
            this.SelfCheck_Units.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.SelfCheck_Units.TabIndex = 11;
            // 
            // IPQC_Units
            // 
            this.IPQC_Units.DisplayMember = "Text";
            this.IPQC_Units.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.IPQC_Units.FormattingEnabled = true;
            this.IPQC_Units.ItemHeight = 16;
            this.IPQC_Units.Location = new System.Drawing.Point(128, 151);
            this.IPQC_Units.Name = "IPQC_Units";
            this.IPQC_Units.Size = new System.Drawing.Size(110, 22);
            this.IPQC_Units.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.IPQC_Units.TabIndex = 10;
            this.IPQC_Units.SelectedIndexChanged += new System.EventHandler(this.IPQC_Units_SelectedIndexChanged);
            // 
            // SelfCheck_1
            // 
            this.SelfCheck_1.Location = new System.Drawing.Point(79, 241);
            this.SelfCheck_1.Name = "SelfCheck_1";
            this.SelfCheck_1.Size = new System.Drawing.Size(43, 22);
            this.SelfCheck_1.TabIndex = 9;
            // 
            // SelfCheck_0
            // 
            this.SelfCheck_0.Location = new System.Drawing.Point(8, 241);
            this.SelfCheck_0.Name = "SelfCheck_0";
            this.SelfCheck_0.Size = new System.Drawing.Size(43, 22);
            this.SelfCheck_0.TabIndex = 8;
            // 
            // IPQC_1
            // 
            this.IPQC_1.Location = new System.Drawing.Point(79, 151);
            this.IPQC_1.Name = "IPQC_1";
            this.IPQC_1.Size = new System.Drawing.Size(43, 22);
            this.IPQC_1.TabIndex = 7;
            this.IPQC_1.TextChanged += new System.EventHandler(this.IPQC_1_TextChanged);
            // 
            // IPQC_0
            // 
            this.IPQC_0.Location = new System.Drawing.Point(8, 151);
            this.IPQC_0.Name = "IPQC_0";
            this.IPQC_0.Size = new System.Drawing.Size(43, 22);
            this.IPQC_0.TabIndex = 6;
            this.IPQC_0.TextChanged += new System.EventHandler(this.IPQC_0_TextChanged);
            // 
            // IPQCGauge
            // 
            this.IPQCGauge.DisplayMember = "Text";
            this.IPQCGauge.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.IPQCGauge.DropDownHeight = 220;
            this.IPQCGauge.FormattingEnabled = true;
            this.IPQCGauge.IntegralHeight = false;
            this.IPQCGauge.ItemHeight = 16;
            this.IPQCGauge.Location = new System.Drawing.Point(8, 123);
            this.IPQCGauge.Name = "IPQCGauge";
            this.IPQCGauge.Size = new System.Drawing.Size(230, 22);
            this.IPQCGauge.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.IPQCGauge.TabIndex = 5;
            this.IPQCGauge.SelectedIndexChanged += new System.EventHandler(this.IPQCGauge_SelectedIndexChanged);
            // 
            // labelX6
            // 
            this.labelX6.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX6.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX6.Location = new System.Drawing.Point(57, 240);
            this.labelX6.Name = "labelX6";
            this.labelX6.Size = new System.Drawing.Size(32, 23);
            this.labelX6.TabIndex = 4;
            this.labelX6.Text = "PC/";
            // 
            // labelX5
            // 
            this.labelX5.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX5.Location = new System.Drawing.Point(14, 188);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(75, 23);
            this.labelX5.TabIndex = 3;
            this.labelX5.Text = "SelfCheck：";
            // 
            // labelX4
            // 
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(57, 150);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(32, 23);
            this.labelX4.TabIndex = 2;
            this.labelX4.Text = "PC/";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.SelectObject);
            this.groupBox2.Controls.Add(this.chb_Remove);
            this.groupBox2.Controls.Add(this.chb_Assign);
            this.groupBox2.Controls.Add(this.ListSheet);
            this.groupBox2.Controls.Add(this.labelX1);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(247, 120);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selection";
            // 
            // SelectObject
            // 
            this.SelectObject.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.SelectObject.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.SelectObject.Location = new System.Drawing.Point(8, 72);
            this.SelectObject.Name = "SelectObject";
            this.SelectObject.Size = new System.Drawing.Size(228, 38);
            this.SelectObject.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.SelectObject.TabIndex = 4;
            this.SelectObject.Text = "選擇物件(0)";
            this.SelectObject.Click += new System.EventHandler(this.SelectObject_Click);
            // 
            // chb_Remove
            // 
            this.chb_Remove.AutoSize = true;
            this.chb_Remove.Location = new System.Drawing.Point(126, 50);
            this.chb_Remove.Name = "chb_Remove";
            this.chb_Remove.Size = new System.Drawing.Size(63, 16);
            this.chb_Remove.TabIndex = 3;
            this.chb_Remove.Text = "Remove";
            this.chb_Remove.UseVisualStyleBackColor = true;
            this.chb_Remove.CheckedChanged += new System.EventHandler(this.chb_Remove_CheckedChanged);
            // 
            // chb_Assign
            // 
            this.chb_Assign.AutoSize = true;
            this.chb_Assign.Location = new System.Drawing.Point(55, 50);
            this.chb_Assign.Name = "chb_Assign";
            this.chb_Assign.Size = new System.Drawing.Size(55, 16);
            this.chb_Assign.TabIndex = 2;
            this.chb_Assign.Text = "Assign";
            this.chb_Assign.UseVisualStyleBackColor = true;
            this.chb_Assign.CheckedChanged += new System.EventHandler(this.chb_Assign_CheckedChanged);
            // 
            // ListSheet
            // 
            this.ListSheet.DisplayMember = "Text";
            this.ListSheet.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ListSheet.FormattingEnabled = true;
            this.ListSheet.ItemHeight = 16;
            this.ListSheet.Location = new System.Drawing.Point(79, 19);
            this.ListSheet.Name = "ListSheet";
            this.ListSheet.Size = new System.Drawing.Size(157, 22);
            this.ListSheet.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ListSheet.TabIndex = 1;
            this.ListSheet.SelectedIndexChanged += new System.EventHandler(this.ListSheet_SelectedIndexChanged);
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(14, 21);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(67, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "選擇圖紙：";
            // 
            // OK
            // 
            this.OK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.OK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.OK.Location = new System.Drawing.Point(47, 421);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.OK.TabIndex = 1;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Cancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.Cancel.Location = new System.Drawing.Point(147, 421);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "關閉";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.FQCcheckBox);
            this.groupBox1.Controls.Add(this.FAIcheckBox);
            this.groupBox1.Controls.Add(this.IPQCcheckBox);
            this.groupBox1.Controls.Add(this.IQCcheckBox);
            this.groupBox1.Controls.Add(this.IQC_Units);
            this.groupBox1.Controls.Add(this.IQC_1);
            this.groupBox1.Controls.Add(this.IQC_0);
            this.groupBox1.Controls.Add(this.IQCGauge);
            this.groupBox1.Controls.Add(this.IPQCGauge);
            this.groupBox1.Controls.Add(this.IPQC_1);
            this.groupBox1.Controls.Add(this.SameIPQC);
            this.groupBox1.Controls.Add(this.SelfCheck_1);
            this.groupBox1.Controls.Add(this.labelX6);
            this.groupBox1.Controls.Add(this.SelfCheck_0);
            this.groupBox1.Controls.Add(this.SelfCheck_Units);
            this.groupBox1.Controls.Add(this.labelX5);
            this.groupBox1.Controls.Add(this.labelX7);
            this.groupBox1.Controls.Add(this.labelX4);
            this.groupBox1.Controls.Add(this.SelfCheckGauge);
            this.groupBox1.Controls.Add(this.IPQC_0);
            this.groupBox1.Controls.Add(this.IPQC_Units);
            this.groupBox1.Location = new System.Drawing.Point(12, 138);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(247, 277);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "檢具選擇/檢驗頻率";
            // 
            // IQC_Units
            // 
            this.IQC_Units.DisplayMember = "Text";
            this.IQC_Units.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.IQC_Units.FormattingEnabled = true;
            this.IQC_Units.ItemHeight = 16;
            this.IQC_Units.Location = new System.Drawing.Point(128, 68);
            this.IQC_Units.Name = "IQC_Units";
            this.IQC_Units.Size = new System.Drawing.Size(110, 22);
            this.IQC_Units.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.IQC_Units.TabIndex = 18;
            // 
            // IQC_1
            // 
            this.IQC_1.Location = new System.Drawing.Point(79, 68);
            this.IQC_1.Name = "IQC_1";
            this.IQC_1.Size = new System.Drawing.Size(43, 22);
            this.IQC_1.TabIndex = 17;
            // 
            // IQC_0
            // 
            this.IQC_0.Location = new System.Drawing.Point(8, 68);
            this.IQC_0.Name = "IQC_0";
            this.IQC_0.Size = new System.Drawing.Size(43, 22);
            this.IQC_0.TabIndex = 16;
            // 
            // IQCGauge
            // 
            this.IQCGauge.DisplayMember = "Text";
            this.IQCGauge.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.IQCGauge.FormattingEnabled = true;
            this.IQCGauge.ItemHeight = 16;
            this.IQCGauge.Location = new System.Drawing.Point(8, 40);
            this.IQCGauge.Name = "IQCGauge";
            this.IQCGauge.Size = new System.Drawing.Size(230, 22);
            this.IQCGauge.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.IQCGauge.TabIndex = 15;
            // 
            // labelX7
            // 
            this.labelX7.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX7.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX7.Location = new System.Drawing.Point(56, 67);
            this.labelX7.Name = "labelX7";
            this.labelX7.Size = new System.Drawing.Size(32, 23);
            this.labelX7.TabIndex = 2;
            this.labelX7.Text = "PC/";
            // 
            // IQCcheckBox
            // 
            this.IQCcheckBox.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.IQCcheckBox.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.IQCcheckBox.Location = new System.Drawing.Point(57, 16);
            this.IQCcheckBox.Name = "IQCcheckBox";
            this.IQCcheckBox.Size = new System.Drawing.Size(43, 23);
            this.IQCcheckBox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.IQCcheckBox.TabIndex = 19;
            this.IQCcheckBox.Text = "IQC";
            this.IQCcheckBox.CheckedChanged += new System.EventHandler(this.IQCcheckBox_CheckedChanged);
            // 
            // IPQCcheckBox
            // 
            this.IPQCcheckBox.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.IPQCcheckBox.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.IPQCcheckBox.Location = new System.Drawing.Point(106, 16);
            this.IPQCcheckBox.Name = "IPQCcheckBox";
            this.IPQCcheckBox.Size = new System.Drawing.Size(43, 23);
            this.IPQCcheckBox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.IPQCcheckBox.TabIndex = 20;
            this.IPQCcheckBox.Text = "IPQC";
            this.IPQCcheckBox.CheckedChanged += new System.EventHandler(this.IPQCcheckBox_CheckedChanged);
            // 
            // FAIcheckBox
            // 
            this.FAIcheckBox.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.FAIcheckBox.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.FAIcheckBox.Location = new System.Drawing.Point(8, 16);
            this.FAIcheckBox.Name = "FAIcheckBox";
            this.FAIcheckBox.Size = new System.Drawing.Size(43, 23);
            this.FAIcheckBox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.FAIcheckBox.TabIndex = 21;
            this.FAIcheckBox.Text = "FAI";
            this.FAIcheckBox.CheckedChanged += new System.EventHandler(this.FAIcheckBox_CheckedChanged);
            // 
            // FQCcheckBox
            // 
            this.FQCcheckBox.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.FQCcheckBox.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.FQCcheckBox.Location = new System.Drawing.Point(155, 16);
            this.FQCcheckBox.Name = "FQCcheckBox";
            this.FQCcheckBox.Size = new System.Drawing.Size(43, 23);
            this.FQCcheckBox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.FQCcheckBox.TabIndex = 22;
            this.FQCcheckBox.Text = "FQC";
            this.FQCcheckBox.CheckedChanged += new System.EventHandler(this.FQCcheckBox_CheckedChanged);
            // 
            // AssignGaugeDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 456);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Cancel);
            this.DoubleBuffered = true;
            this.EnableGlass = false;
            this.Name = "AssignGaugeDlg";
            this.Text = "AssignGaugeDlg";
            this.Load += new System.EventHandler(this.AssignGaugeDlg_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.StyleManager styleManager1;
        private System.Windows.Forms.TextBox SelfCheck_1;
        private System.Windows.Forms.TextBox SelfCheck_0;
        private System.Windows.Forms.TextBox IPQC_1;
        private System.Windows.Forms.TextBox IPQC_0;
        private DevComponents.DotNetBar.Controls.ComboBoxEx IPQCGauge;
        private DevComponents.DotNetBar.LabelX labelX6;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.Controls.ComboBoxEx SelfCheck_Units;
        private DevComponents.DotNetBar.Controls.ComboBoxEx IPQC_Units;
        private System.Windows.Forms.CheckBox SameIPQC;
        private DevComponents.DotNetBar.Controls.ComboBoxEx SelfCheckGauge;
        private DevComponents.DotNetBar.ButtonX Cancel;
        private DevComponents.DotNetBar.ButtonX OK;
        private System.Windows.Forms.GroupBox groupBox2;
        private DevComponents.DotNetBar.ButtonX SelectObject;
        private System.Windows.Forms.CheckBox chb_Remove;
        private System.Windows.Forms.CheckBox chb_Assign;
        private DevComponents.DotNetBar.Controls.ComboBoxEx ListSheet;
        private DevComponents.DotNetBar.LabelX labelX1;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx IQC_Units;
        private System.Windows.Forms.TextBox IQC_1;
        private System.Windows.Forms.TextBox IQC_0;
        private DevComponents.DotNetBar.Controls.ComboBoxEx IQCGauge;
        private DevComponents.DotNetBar.LabelX labelX7;
        private DevComponents.DotNetBar.Controls.CheckBoxX FQCcheckBox;
        private DevComponents.DotNetBar.Controls.CheckBoxX FAIcheckBox;
        private DevComponents.DotNetBar.Controls.CheckBoxX IPQCcheckBox;
        private DevComponents.DotNetBar.Controls.CheckBoxX IQCcheckBox;
    }
}