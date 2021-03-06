﻿namespace MainProgram
{
    partial class MainProgramDlg
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
            this.ControlerGroup = new System.Windows.Forms.GroupBox();
            this.chb_M198 = new System.Windows.Forms.CheckBox();
            this.chb_M98 = new System.Windows.Forms.CheckBox();
            this.chb_Fanuc = new System.Windows.Forms.CheckBox();
            this.chb_Heidenhain = new System.Windows.Forms.CheckBox();
            this.chb_Simens = new System.Windows.Forms.CheckBox();
            this.NCGroup = new System.Windows.Forms.GroupBox();
            this.DeleteSel = new DevComponents.DotNetBar.ButtonX();
            this.listView5 = new System.Windows.Forms.ListView();
            this.OperName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ToolNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView4 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView3 = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DownButton = new DevComponents.DotNetBar.ButtonX();
            this.UpButton = new DevComponents.DotNetBar.ButtonX();
            this.CopyItem = new DevComponents.DotNetBar.ButtonX();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RemoveButton = new DevComponents.DotNetBar.ButtonX();
            this.comboBoxNCgroup = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.AddButton = new DevComponents.DotNetBar.ButtonX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.AddCondition = new System.Windows.Forms.GroupBox();
            this.ClearTextBox = new DevComponents.DotNetBar.ButtonX();
            this.UserDefineTxt = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.UserAddCondition = new DevComponents.DotNetBar.ButtonX();
            this.UserCondition = new System.Windows.Forms.TextBox();
            this.ExportMainProg = new DevComponents.DotNetBar.ButtonX();
            this.CloseDlg = new DevComponents.DotNetBar.ButtonX();
            this.ControlerGroup.SuspendLayout();
            this.NCGroup.SuspendLayout();
            this.AddCondition.SuspendLayout();
            this.SuspendLayout();
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // ControlerGroup
            // 
            this.ControlerGroup.Controls.Add(this.chb_M198);
            this.ControlerGroup.Controls.Add(this.chb_M98);
            this.ControlerGroup.Controls.Add(this.chb_Fanuc);
            this.ControlerGroup.Controls.Add(this.chb_Heidenhain);
            this.ControlerGroup.Controls.Add(this.chb_Simens);
            this.ControlerGroup.Location = new System.Drawing.Point(12, 12);
            this.ControlerGroup.Name = "ControlerGroup";
            this.ControlerGroup.Size = new System.Drawing.Size(190, 94);
            this.ControlerGroup.TabIndex = 0;
            this.ControlerGroup.TabStop = false;
            this.ControlerGroup.Text = "控制器";
            // 
            // chb_M198
            // 
            this.chb_M198.AutoSize = true;
            this.chb_M198.Location = new System.Drawing.Point(135, 68);
            this.chb_M198.Name = "chb_M198";
            this.chb_M198.Size = new System.Drawing.Size(52, 16);
            this.chb_M198.TabIndex = 9;
            this.chb_M198.Text = "M198";
            this.chb_M198.UseVisualStyleBackColor = true;
            this.chb_M198.CheckedChanged += new System.EventHandler(this.chb_M198_CheckedChanged);
            // 
            // chb_M98
            // 
            this.chb_M98.AutoSize = true;
            this.chb_M98.Location = new System.Drawing.Point(83, 68);
            this.chb_M98.Name = "chb_M98";
            this.chb_M98.Size = new System.Drawing.Size(46, 16);
            this.chb_M98.TabIndex = 8;
            this.chb_M98.Text = "M98";
            this.chb_M98.UseVisualStyleBackColor = true;
            this.chb_M98.CheckedChanged += new System.EventHandler(this.chb_M98_CheckedChanged);
            // 
            // chb_Fanuc
            // 
            this.chb_Fanuc.AutoSize = true;
            this.chb_Fanuc.Location = new System.Drawing.Point(18, 68);
            this.chb_Fanuc.Name = "chb_Fanuc";
            this.chb_Fanuc.Size = new System.Drawing.Size(52, 16);
            this.chb_Fanuc.TabIndex = 7;
            this.chb_Fanuc.Text = "Fanuc";
            this.chb_Fanuc.UseVisualStyleBackColor = true;
            this.chb_Fanuc.CheckedChanged += new System.EventHandler(this.chb_Fanuc_CheckedChanged);
            // 
            // chb_Heidenhain
            // 
            this.chb_Heidenhain.AutoSize = true;
            this.chb_Heidenhain.Location = new System.Drawing.Point(18, 46);
            this.chb_Heidenhain.Name = "chb_Heidenhain";
            this.chb_Heidenhain.Size = new System.Drawing.Size(77, 16);
            this.chb_Heidenhain.TabIndex = 6;
            this.chb_Heidenhain.Text = "Heidenhain";
            this.chb_Heidenhain.UseVisualStyleBackColor = true;
            this.chb_Heidenhain.CheckedChanged += new System.EventHandler(this.chb_Heidenhain_CheckedChanged);
            // 
            // chb_Simens
            // 
            this.chb_Simens.AutoSize = true;
            this.chb_Simens.Location = new System.Drawing.Point(18, 24);
            this.chb_Simens.Name = "chb_Simens";
            this.chb_Simens.Size = new System.Drawing.Size(62, 16);
            this.chb_Simens.TabIndex = 5;
            this.chb_Simens.Text = "Siemens";
            this.chb_Simens.UseVisualStyleBackColor = true;
            this.chb_Simens.CheckedChanged += new System.EventHandler(this.chb_Simens_CheckedChanged);
            // 
            // NCGroup
            // 
            this.NCGroup.Controls.Add(this.DeleteSel);
            this.NCGroup.Controls.Add(this.listView5);
            this.NCGroup.Controls.Add(this.listView4);
            this.NCGroup.Controls.Add(this.listView3);
            this.NCGroup.Controls.Add(this.DownButton);
            this.NCGroup.Controls.Add(this.UpButton);
            this.NCGroup.Controls.Add(this.CopyItem);
            this.NCGroup.Controls.Add(this.listView1);
            this.NCGroup.Controls.Add(this.RemoveButton);
            this.NCGroup.Controls.Add(this.comboBoxNCgroup);
            this.NCGroup.Controls.Add(this.AddButton);
            this.NCGroup.Controls.Add(this.labelX2);
            this.NCGroup.Location = new System.Drawing.Point(12, 120);
            this.NCGroup.Name = "NCGroup";
            this.NCGroup.Size = new System.Drawing.Size(477, 335);
            this.NCGroup.TabIndex = 2;
            this.NCGroup.TabStop = false;
            this.NCGroup.Text = "程式";
            // 
            // DeleteSel
            // 
            this.DeleteSel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.DeleteSel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.DeleteSel.Location = new System.Drawing.Point(364, 23);
            this.DeleteSel.Name = "DeleteSel";
            this.DeleteSel.Size = new System.Drawing.Size(61, 23);
            this.DeleteSel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.DeleteSel.TabIndex = 17;
            this.DeleteSel.Text = "刪除";
            this.DeleteSel.Click += new System.EventHandler(this.DeleteSel_Click);
            // 
            // listView5
            // 
            this.listView5.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.OperName,
            this.ToolNumber});
            this.listView5.GridLines = true;
            this.listView5.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView5.Location = new System.Drawing.Point(17, 52);
            this.listView5.Name = "listView5";
            this.listView5.Size = new System.Drawing.Size(173, 277);
            this.listView5.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView5.TabIndex = 16;
            this.listView5.UseCompatibleStateImageBehavior = false;
            this.listView5.View = System.Windows.Forms.View.Details;
            this.listView5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listView5_MouseUp);
            // 
            // OperName
            // 
            this.OperName.Text = "程式名";
            this.OperName.Width = 82;
            // 
            // ToolNumber
            // 
            this.ToolNumber.Text = "刀號";
            this.ToolNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ToolNumber.Width = 82;
            // 
            // listView4
            // 
            this.listView4.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView4.GridLines = true;
            this.listView4.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView4.Location = new System.Drawing.Point(241, 288);
            this.listView4.Name = "listView4";
            this.listView4.Size = new System.Drawing.Size(184, 41);
            this.listView4.TabIndex = 14;
            this.listView4.UseCompatibleStateImageBehavior = false;
            this.listView4.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 180;
            // 
            // listView3
            // 
            this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.listView3.GridLines = true;
            this.listView3.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView3.Location = new System.Drawing.Point(241, 52);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(184, 43);
            this.listView3.TabIndex = 13;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Width = 180;
            // 
            // DownButton
            // 
            this.DownButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.DownButton.ColorTable = DevComponents.DotNetBar.eButtonColor.MagentaWithBackground;
            this.DownButton.Location = new System.Drawing.Point(431, 191);
            this.DownButton.Name = "DownButton";
            this.DownButton.Size = new System.Drawing.Size(39, 23);
            this.DownButton.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.DownButton.TabIndex = 8;
            this.DownButton.Click += new System.EventHandler(this.DownButton_Click);
            // 
            // UpButton
            // 
            this.UpButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.UpButton.ColorTable = DevComponents.DotNetBar.eButtonColor.MagentaWithBackground;
            this.UpButton.Location = new System.Drawing.Point(431, 150);
            this.UpButton.Name = "UpButton";
            this.UpButton.Size = new System.Drawing.Size(39, 23);
            this.UpButton.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.UpButton.TabIndex = 7;
            this.UpButton.Click += new System.EventHandler(this.UpButton_Click);
            // 
            // CopyItem
            // 
            this.CopyItem.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.CopyItem.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.CopyItem.Location = new System.Drawing.Point(241, 23);
            this.CopyItem.Name = "CopyItem";
            this.CopyItem.Size = new System.Drawing.Size(61, 23);
            this.CopyItem.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.CopyItem.TabIndex = 6;
            this.CopyItem.Text = "複製";
            this.CopyItem.Click += new System.EventHandler(this.CopyItem_Click);
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(241, 101);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(184, 181);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listView1_ItemDrag);
            this.listView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView1_DragDrop);
            this.listView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.listView1_DragEnter);
            this.listView1.DragOver += new System.Windows.Forms.DragEventHandler(this.listView1_DragOver);
            this.listView1.DragLeave += new System.EventHandler(this.listView1_DragLeave);
            this.listView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseUp);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 200;
            // 
            // RemoveButton
            // 
            this.RemoveButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.RemoveButton.ColorTable = DevComponents.DotNetBar.eButtonColor.MagentaWithBackground;
            this.RemoveButton.Location = new System.Drawing.Point(196, 191);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(39, 23);
            this.RemoveButton.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.RemoveButton.TabIndex = 5;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // comboBoxNCgroup
            // 
            this.comboBoxNCgroup.DisplayMember = "Text";
            this.comboBoxNCgroup.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxNCgroup.FormattingEnabled = true;
            this.comboBoxNCgroup.ItemHeight = 16;
            this.comboBoxNCgroup.Location = new System.Drawing.Point(82, 21);
            this.comboBoxNCgroup.Name = "comboBoxNCgroup";
            this.comboBoxNCgroup.Size = new System.Drawing.Size(108, 22);
            this.comboBoxNCgroup.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.comboBoxNCgroup.TabIndex = 0;
            this.comboBoxNCgroup.SelectedIndexChanged += new System.EventHandler(this.comboBoxNCgroup_SelectedIndexChanged);
            // 
            // AddButton
            // 
            this.AddButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.AddButton.BackColor = System.Drawing.Color.White;
            this.AddButton.ColorTable = DevComponents.DotNetBar.eButtonColor.MagentaWithBackground;
            this.AddButton.Font = new System.Drawing.Font("新細明體", 9F);
            this.AddButton.Location = new System.Drawing.Point(196, 150);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(39, 23);
            this.AddButton.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.AddButton.TabIndex = 3;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(17, 23);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(75, 23);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "群組名稱：";
            // 
            // AddCondition
            // 
            this.AddCondition.Controls.Add(this.ClearTextBox);
            this.AddCondition.Controls.Add(this.UserDefineTxt);
            this.AddCondition.Controls.Add(this.UserAddCondition);
            this.AddCondition.Controls.Add(this.UserCondition);
            this.AddCondition.Location = new System.Drawing.Point(208, 12);
            this.AddCondition.Name = "AddCondition";
            this.AddCondition.Size = new System.Drawing.Size(281, 94);
            this.AddCondition.TabIndex = 1;
            this.AddCondition.TabStop = false;
            this.AddCondition.Text = "新增條件";
            // 
            // ClearTextBox
            // 
            this.ClearTextBox.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.ClearTextBox.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.ClearTextBox.Location = new System.Drawing.Point(229, 42);
            this.ClearTextBox.Name = "ClearTextBox";
            this.ClearTextBox.Size = new System.Drawing.Size(45, 46);
            this.ClearTextBox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ClearTextBox.TabIndex = 5;
            this.ClearTextBox.Text = "清空";
            this.ClearTextBox.Click += new System.EventHandler(this.ClearTextBox_Click);
            // 
            // UserDefineTxt
            // 
            this.UserDefineTxt.DisplayMember = "Text";
            this.UserDefineTxt.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.UserDefineTxt.FormattingEnabled = true;
            this.UserDefineTxt.ItemHeight = 16;
            this.UserDefineTxt.Location = new System.Drawing.Point(178, 18);
            this.UserDefineTxt.Name = "UserDefineTxt";
            this.UserDefineTxt.Size = new System.Drawing.Size(96, 22);
            this.UserDefineTxt.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.UserDefineTxt.TabIndex = 2;
            this.UserDefineTxt.SelectedIndexChanged += new System.EventHandler(this.UserDefineTxt_SelectedIndexChanged);
            // 
            // UserAddCondition
            // 
            this.UserAddCondition.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.UserAddCondition.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.UserAddCondition.Location = new System.Drawing.Point(178, 42);
            this.UserAddCondition.Name = "UserAddCondition";
            this.UserAddCondition.Size = new System.Drawing.Size(45, 46);
            this.UserAddCondition.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.UserAddCondition.TabIndex = 1;
            this.UserAddCondition.Text = "新增";
            this.UserAddCondition.Click += new System.EventHandler(this.UserAddCondition_Click);
            // 
            // UserCondition
            // 
            this.UserCondition.Location = new System.Drawing.Point(6, 18);
            this.UserCondition.Multiline = true;
            this.UserCondition.Name = "UserCondition";
            this.UserCondition.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.UserCondition.Size = new System.Drawing.Size(166, 70);
            this.UserCondition.TabIndex = 0;
            this.UserCondition.WordWrap = false;
            // 
            // ExportMainProg
            // 
            this.ExportMainProg.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.ExportMainProg.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.ExportMainProg.Location = new System.Drawing.Point(158, 462);
            this.ExportMainProg.Name = "ExportMainProg";
            this.ExportMainProg.Size = new System.Drawing.Size(75, 23);
            this.ExportMainProg.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ExportMainProg.TabIndex = 3;
            this.ExportMainProg.Text = "確認輸出";
            this.ExportMainProg.Click += new System.EventHandler(this.ExportMainProg_Click);
            // 
            // CloseDlg
            // 
            this.CloseDlg.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.CloseDlg.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.CloseDlg.Location = new System.Drawing.Point(266, 461);
            this.CloseDlg.Name = "CloseDlg";
            this.CloseDlg.Size = new System.Drawing.Size(75, 23);
            this.CloseDlg.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.CloseDlg.TabIndex = 4;
            this.CloseDlg.Text = "關閉";
            this.CloseDlg.Click += new System.EventHandler(this.CloseDlg_Click);
            // 
            // MainProgramDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 495);
            this.Controls.Add(this.CloseDlg);
            this.Controls.Add(this.ExportMainProg);
            this.Controls.Add(this.NCGroup);
            this.Controls.Add(this.AddCondition);
            this.Controls.Add(this.ControlerGroup);
            this.DoubleBuffered = true;
            this.Name = "MainProgramDlg";
            this.Text = "MainProgramDlg";
            this.Load += new System.EventHandler(this.MainProgramDlg_Load);
            this.ControlerGroup.ResumeLayout(false);
            this.ControlerGroup.PerformLayout();
            this.NCGroup.ResumeLayout(false);
            this.AddCondition.ResumeLayout(false);
            this.AddCondition.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.StyleManager styleManager1;
        private System.Windows.Forms.GroupBox ControlerGroup;
        private System.Windows.Forms.CheckBox chb_M198;
        private System.Windows.Forms.CheckBox chb_M98;
        private System.Windows.Forms.CheckBox chb_Fanuc;
        private System.Windows.Forms.CheckBox chb_Heidenhain;
        private System.Windows.Forms.CheckBox chb_Simens;
        private System.Windows.Forms.GroupBox NCGroup;
        private System.Windows.Forms.ListView listView1;
        private DevComponents.DotNetBar.ButtonX RemoveButton;
        private DevComponents.DotNetBar.Controls.ComboBoxEx comboBoxNCgroup;
        private DevComponents.DotNetBar.ButtonX AddButton;
        private DevComponents.DotNetBar.LabelX labelX2;
        private System.Windows.Forms.GroupBox AddCondition;
        private DevComponents.DotNetBar.ButtonX DownButton;
        private DevComponents.DotNetBar.ButtonX UpButton;
        private DevComponents.DotNetBar.ButtonX CopyItem;
        private DevComponents.DotNetBar.Controls.ComboBoxEx UserDefineTxt;
        private DevComponents.DotNetBar.ButtonX UserAddCondition;
        private System.Windows.Forms.TextBox UserCondition;
        private DevComponents.DotNetBar.ButtonX ExportMainProg;
        private DevComponents.DotNetBar.ButtonX CloseDlg;
        private System.Windows.Forms.ListView listView4;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.ColumnHeader OperName;
        private System.Windows.Forms.ColumnHeader ToolNumber;
        public System.Windows.Forms.ListView listView5;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private DevComponents.DotNetBar.ButtonX DeleteSel;
        private DevComponents.DotNetBar.ButtonX ClearTextBox;
    }
}