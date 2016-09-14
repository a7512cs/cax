namespace PartInformation
{
    partial class PartInformationDlg
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelX8 = new DevComponents.DotNetBar.LabelX();
            this.UserDefineTxt = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.Angle = new System.Windows.Forms.PictureBox();
            this.UserAddNote = new DevComponents.DotNetBar.ButtonX();
            this.PlusMinus = new System.Windows.Forms.PictureBox();
            this.NoteBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.AngleText = new System.Windows.Forms.TextBox();
            this.labelX7 = new DevComponents.DotNetBar.LabelX();
            this.TolValue3 = new System.Windows.Forms.TextBox();
            this.TolValue2 = new System.Windows.Forms.TextBox();
            this.TolValue1 = new System.Windows.Forms.TextBox();
            this.TolValue0 = new System.Windows.Forms.TextBox();
            this.TolTitle3 = new System.Windows.Forms.TextBox();
            this.TolTitle2 = new System.Windows.Forms.TextBox();
            this.TolTitle1 = new System.Windows.Forms.TextBox();
            this.TolTitle0 = new System.Windows.Forms.TextBox();
            this.chb_Point = new System.Windows.Forms.CheckBox();
            this.chb_Region = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.MaterialText = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.DraftingRevText = new System.Windows.Forms.TextBox();
            this.PartDescriptionText = new System.Windows.Forms.TextBox();
            this.labelX6 = new DevComponents.DotNetBar.LabelX();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.OK = new DevComponents.DotNetBar.ButtonX();
            this.Cancel = new DevComponents.DotNetBar.ButtonX();
            this.PartNumberText = new DevComponents.DotNetBar.LabelX();
            this.CusRevText = new DevComponents.DotNetBar.LabelX();
            this.PartUnitText = new DevComponents.DotNetBar.LabelX();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Angle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlusMinus)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelX8);
            this.groupBox1.Controls.Add(this.UserDefineTxt);
            this.groupBox1.Controls.Add(this.Angle);
            this.groupBox1.Controls.Add(this.UserAddNote);
            this.groupBox1.Controls.Add(this.PlusMinus);
            this.groupBox1.Controls.Add(this.NoteBox);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(464, 422);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Part Information";
            // 
            // labelX8
            // 
            // 
            // 
            // 
            this.labelX8.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX8.Font = new System.Drawing.Font("標楷體", 13F);
            this.labelX8.Location = new System.Drawing.Point(335, 258);
            this.labelX8.Name = "labelX8";
            this.labelX8.Size = new System.Drawing.Size(122, 23);
            this.labelX8.TabIndex = 12;
            this.labelX8.Text = "使用者自定義";
            // 
            // UserDefineTxt
            // 
            this.UserDefineTxt.DisplayMember = "Text";
            this.UserDefineTxt.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.UserDefineTxt.FormattingEnabled = true;
            this.UserDefineTxt.ItemHeight = 16;
            this.UserDefineTxt.Location = new System.Drawing.Point(327, 287);
            this.UserDefineTxt.Name = "UserDefineTxt";
            this.UserDefineTxt.Size = new System.Drawing.Size(130, 22);
            this.UserDefineTxt.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.UserDefineTxt.TabIndex = 11;
            this.UserDefineTxt.SelectedIndexChanged += new System.EventHandler(this.UserDefineTxt_SelectedIndexChanged);
            // 
            // Angle
            // 
            this.Angle.BackColor = System.Drawing.Color.Transparent;
            this.Angle.Image = global::PartInformation.Properties.Resources.度1;
            this.Angle.Location = new System.Drawing.Point(364, 315);
            this.Angle.Name = "Angle";
            this.Angle.Size = new System.Drawing.Size(31, 34);
            this.Angle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Angle.TabIndex = 10;
            this.Angle.TabStop = false;
            this.Angle.Tag = "<$s>";
            this.Angle.Click += new System.EventHandler(this.Angle_Click);
            // 
            // UserAddNote
            // 
            this.UserAddNote.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.UserAddNote.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.UserAddNote.Location = new System.Drawing.Point(327, 388);
            this.UserAddNote.Name = "UserAddNote";
            this.UserAddNote.Size = new System.Drawing.Size(130, 28);
            this.UserAddNote.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.UserAddNote.TabIndex = 9;
            this.UserAddNote.Text = "新增";
            this.UserAddNote.Click += new System.EventHandler(this.UserAddNote_Click);
            // 
            // PlusMinus
            // 
            this.PlusMinus.BackColor = System.Drawing.Color.Transparent;
            this.PlusMinus.Image = global::PartInformation.Properties.Resources.正負;
            this.PlusMinus.Location = new System.Drawing.Point(327, 315);
            this.PlusMinus.Name = "PlusMinus";
            this.PlusMinus.Size = new System.Drawing.Size(31, 34);
            this.PlusMinus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PlusMinus.TabIndex = 3;
            this.PlusMinus.TabStop = false;
            this.PlusMinus.Tag = "<$t>";
            this.PlusMinus.Click += new System.EventHandler(this.PlusMinus_Click);
            // 
            // NoteBox
            // 
            this.NoteBox.Font = new System.Drawing.Font("新細明體", 15F);
            this.NoteBox.Location = new System.Drawing.Point(6, 256);
            this.NoteBox.Multiline = true;
            this.NoteBox.Name = "NoteBox";
            this.NoteBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.NoteBox.Size = new System.Drawing.Size(315, 160);
            this.NoteBox.TabIndex = 2;
            this.NoteBox.WordWrap = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.AngleText);
            this.groupBox3.Controls.Add(this.labelX7);
            this.groupBox3.Controls.Add(this.TolValue3);
            this.groupBox3.Controls.Add(this.TolValue2);
            this.groupBox3.Controls.Add(this.TolValue1);
            this.groupBox3.Controls.Add(this.TolValue0);
            this.groupBox3.Controls.Add(this.TolTitle3);
            this.groupBox3.Controls.Add(this.TolTitle2);
            this.groupBox3.Controls.Add(this.TolTitle1);
            this.groupBox3.Controls.Add(this.TolTitle0);
            this.groupBox3.Controls.Add(this.chb_Point);
            this.groupBox3.Controls.Add(this.chb_Region);
            this.groupBox3.Location = new System.Drawing.Point(230, 21);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(227, 229);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "一般公差";
            // 
            // AngleText
            // 
            this.AngleText.Location = new System.Drawing.Point(72, 179);
            this.AngleText.Name = "AngleText";
            this.AngleText.Size = new System.Drawing.Size(47, 22);
            this.AngleText.TabIndex = 11;
            // 
            // labelX7
            // 
            // 
            // 
            // 
            this.labelX7.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX7.Font = new System.Drawing.Font("新細明體", 12F);
            this.labelX7.Location = new System.Drawing.Point(19, 180);
            this.labelX7.Name = "labelX7";
            this.labelX7.Size = new System.Drawing.Size(75, 23);
            this.labelX7.TabIndex = 10;
            this.labelX7.Text = "角度：";
            // 
            // TolValue3
            // 
            this.TolValue3.Location = new System.Drawing.Point(125, 143);
            this.TolValue3.Name = "TolValue3";
            this.TolValue3.Size = new System.Drawing.Size(85, 22);
            this.TolValue3.TabIndex = 9;
            // 
            // TolValue2
            // 
            this.TolValue2.Location = new System.Drawing.Point(125, 115);
            this.TolValue2.Name = "TolValue2";
            this.TolValue2.Size = new System.Drawing.Size(85, 22);
            this.TolValue2.TabIndex = 8;
            // 
            // TolValue1
            // 
            this.TolValue1.Location = new System.Drawing.Point(125, 87);
            this.TolValue1.Name = "TolValue1";
            this.TolValue1.Size = new System.Drawing.Size(85, 22);
            this.TolValue1.TabIndex = 7;
            // 
            // TolValue0
            // 
            this.TolValue0.Location = new System.Drawing.Point(125, 59);
            this.TolValue0.Name = "TolValue0";
            this.TolValue0.Size = new System.Drawing.Size(85, 22);
            this.TolValue0.TabIndex = 6;
            // 
            // TolTitle3
            // 
            this.TolTitle3.Location = new System.Drawing.Point(19, 143);
            this.TolTitle3.Name = "TolTitle3";
            this.TolTitle3.Size = new System.Drawing.Size(100, 22);
            this.TolTitle3.TabIndex = 5;
            // 
            // TolTitle2
            // 
            this.TolTitle2.Location = new System.Drawing.Point(19, 115);
            this.TolTitle2.Name = "TolTitle2";
            this.TolTitle2.Size = new System.Drawing.Size(100, 22);
            this.TolTitle2.TabIndex = 4;
            // 
            // TolTitle1
            // 
            this.TolTitle1.Location = new System.Drawing.Point(19, 87);
            this.TolTitle1.Name = "TolTitle1";
            this.TolTitle1.Size = new System.Drawing.Size(100, 22);
            this.TolTitle1.TabIndex = 3;
            // 
            // TolTitle0
            // 
            this.TolTitle0.Location = new System.Drawing.Point(19, 59);
            this.TolTitle0.Name = "TolTitle0";
            this.TolTitle0.Size = new System.Drawing.Size(100, 22);
            this.TolTitle0.TabIndex = 2;
            // 
            // chb_Point
            // 
            this.chb_Point.AutoSize = true;
            this.chb_Point.Location = new System.Drawing.Point(19, 28);
            this.chb_Point.Name = "chb_Point";
            this.chb_Point.Size = new System.Drawing.Size(84, 16);
            this.chb_Point.TabIndex = 1;
            this.chb_Point.Text = "小數位區分";
            this.chb_Point.UseVisualStyleBackColor = true;
            this.chb_Point.CheckedChanged += new System.EventHandler(this.chb_Point_CheckedChanged);
            // 
            // chb_Region
            // 
            this.chb_Region.AutoSize = true;
            this.chb_Region.Location = new System.Drawing.Point(125, 27);
            this.chb_Region.Name = "chb_Region";
            this.chb_Region.Size = new System.Drawing.Size(72, 16);
            this.chb_Region.TabIndex = 0;
            this.chb_Region.Text = "範圍區分";
            this.chb_Region.UseVisualStyleBackColor = true;
            this.chb_Region.CheckedChanged += new System.EventHandler(this.chb_Region_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.PartUnitText);
            this.groupBox2.Controls.Add(this.CusRevText);
            this.groupBox2.Controls.Add(this.PartNumberText);
            this.groupBox2.Controls.Add(this.MaterialText);
            this.groupBox2.Controls.Add(this.DraftingRevText);
            this.groupBox2.Controls.Add(this.PartDescriptionText);
            this.groupBox2.Controls.Add(this.labelX6);
            this.groupBox2.Controls.Add(this.labelX5);
            this.groupBox2.Controls.Add(this.labelX4);
            this.groupBox2.Controls.Add(this.labelX3);
            this.groupBox2.Controls.Add(this.labelX2);
            this.groupBox2.Controls.Add(this.labelX1);
            this.groupBox2.Location = new System.Drawing.Point(6, 21);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(218, 229);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "基礎資訊";
            // 
            // MaterialText
            // 
            this.MaterialText.DisplayMember = "Text";
            this.MaterialText.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.MaterialText.FormattingEnabled = true;
            this.MaterialText.ItemHeight = 16;
            this.MaterialText.Location = new System.Drawing.Point(59, 156);
            this.MaterialText.Name = "MaterialText";
            this.MaterialText.Size = new System.Drawing.Size(152, 22);
            this.MaterialText.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.MaterialText.TabIndex = 12;
            // 
            // DraftingRevText
            // 
            this.DraftingRevText.Location = new System.Drawing.Point(92, 187);
            this.DraftingRevText.Name = "DraftingRevText";
            this.DraftingRevText.Size = new System.Drawing.Size(120, 22);
            this.DraftingRevText.TabIndex = 11;
            // 
            // PartDescriptionText
            // 
            this.PartDescriptionText.Location = new System.Drawing.Point(60, 59);
            this.PartDescriptionText.Name = "PartDescriptionText";
            this.PartDescriptionText.Size = new System.Drawing.Size(152, 22);
            this.PartDescriptionText.TabIndex = 7;
            // 
            // labelX6
            // 
            // 
            // 
            // 
            this.labelX6.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX6.Font = new System.Drawing.Font("新細明體", 12F);
            this.labelX6.Location = new System.Drawing.Point(6, 188);
            this.labelX6.Name = "labelX6";
            this.labelX6.Size = new System.Drawing.Size(94, 23);
            this.labelX6.TabIndex = 5;
            this.labelX6.Text = "製圖版次：";
            // 
            // labelX5
            // 
            // 
            // 
            // 
            this.labelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX5.Font = new System.Drawing.Font("新細明體", 12F);
            this.labelX5.Location = new System.Drawing.Point(6, 156);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(75, 23);
            this.labelX5.TabIndex = 4;
            this.labelX5.Text = "材質：";
            // 
            // labelX4
            // 
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Font = new System.Drawing.Font("新細明體", 12F);
            this.labelX4.Location = new System.Drawing.Point(6, 125);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(75, 23);
            this.labelX4.TabIndex = 3;
            this.labelX4.Text = "單位：";
            // 
            // labelX3
            // 
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Font = new System.Drawing.Font("新細明體", 12F);
            this.labelX3.Location = new System.Drawing.Point(6, 92);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(94, 23);
            this.labelX3.TabIndex = 2;
            this.labelX3.Text = "客戶版次：";
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Font = new System.Drawing.Font("新細明體", 12F);
            this.labelX2.Location = new System.Drawing.Point(6, 60);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(75, 23);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "品名：";
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Font = new System.Drawing.Font("新細明體", 12F);
            this.labelX1.Location = new System.Drawing.Point(6, 28);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(75, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "料號：";
            // 
            // OK
            // 
            this.OK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.OK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.OK.Location = new System.Drawing.Point(154, 440);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.OK.TabIndex = 1;
            this.OK.Text = "確認";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Cancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.Cancel.Location = new System.Drawing.Point(261, 440);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "關閉";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // PartNumberText
            // 
            // 
            // 
            // 
            this.PartNumberText.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.PartNumberText.Location = new System.Drawing.Point(59, 27);
            this.PartNumberText.Name = "PartNumberText";
            this.PartNumberText.Size = new System.Drawing.Size(152, 23);
            this.PartNumberText.TabIndex = 13;
            this.PartNumberText.Text = "labelX9";
            // 
            // CusRevText
            // 
            // 
            // 
            // 
            this.CusRevText.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.CusRevText.Location = new System.Drawing.Point(92, 92);
            this.CusRevText.Name = "CusRevText";
            this.CusRevText.Size = new System.Drawing.Size(119, 23);
            this.CusRevText.TabIndex = 14;
            this.CusRevText.Text = "labelX9";
            // 
            // PartUnitText
            // 
            // 
            // 
            // 
            this.PartUnitText.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.PartUnitText.Location = new System.Drawing.Point(60, 125);
            this.PartUnitText.Name = "PartUnitText";
            this.PartUnitText.Size = new System.Drawing.Size(151, 23);
            this.PartUnitText.TabIndex = 15;
            this.PartUnitText.Text = "labelX9";
            // 
            // PartInformationDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 472);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.Name = "PartInformationDlg";
            this.Text = "PartInformationDlg";
            this.Load += new System.EventHandler(this.PartInformationDlg_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Angle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlusMinus)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.StyleManager styleManager1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox NoteBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox AngleText;
        private DevComponents.DotNetBar.LabelX labelX7;
        private System.Windows.Forms.TextBox TolValue3;
        private System.Windows.Forms.TextBox TolValue2;
        private System.Windows.Forms.TextBox TolValue1;
        private System.Windows.Forms.TextBox TolValue0;
        private System.Windows.Forms.TextBox TolTitle3;
        private System.Windows.Forms.TextBox TolTitle2;
        private System.Windows.Forms.TextBox TolTitle1;
        private System.Windows.Forms.TextBox TolTitle0;
        private System.Windows.Forms.CheckBox chb_Point;
        private System.Windows.Forms.CheckBox chb_Region;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox DraftingRevText;
        private System.Windows.Forms.TextBox PartDescriptionText;
        private DevComponents.DotNetBar.LabelX labelX6;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.ButtonX OK;
        private DevComponents.DotNetBar.ButtonX Cancel;
        private System.Windows.Forms.PictureBox PlusMinus;
        private DevComponents.DotNetBar.ButtonX UserAddNote;
        private System.Windows.Forms.PictureBox Angle;
        private DevComponents.DotNetBar.Controls.ComboBoxEx UserDefineTxt;
        private DevComponents.DotNetBar.LabelX labelX8;
        private DevComponents.DotNetBar.Controls.ComboBoxEx MaterialText;
        private DevComponents.DotNetBar.LabelX PartUnitText;
        private DevComponents.DotNetBar.LabelX CusRevText;
        private DevComponents.DotNetBar.LabelX PartNumberText;
    }
}