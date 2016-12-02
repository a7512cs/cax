namespace CreateBallon
{
    partial class CreateBallonDlg
    {
        /// <summary>
        /// 全域變數
        /// </summary>
        public bool Is_Keep;



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
            this.chb_keepOrigination = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.chb_Regeneration = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.OK = new DevComponents.DotNetBar.ButtonX();
            this.Cancel = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // chb_keepOrigination
            // 
            // 
            // 
            // 
            this.chb_keepOrigination.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chb_keepOrigination.Font = new System.Drawing.Font("標楷體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chb_keepOrigination.Location = new System.Drawing.Point(12, 9);
            this.chb_keepOrigination.Name = "chb_keepOrigination";
            this.chb_keepOrigination.Size = new System.Drawing.Size(427, 72);
            this.chb_keepOrigination.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.chb_keepOrigination.TabIndex = 2;
            this.chb_keepOrigination.Text = "保留原始泡泡圖\r\n(已產生過泡泡且有新增或修改量測檢具)";
            this.chb_keepOrigination.CheckedChanged += new System.EventHandler(this.chb_keepOrigination_CheckedChanged);
            // 
            // chb_Regeneration
            // 
            // 
            // 
            // 
            this.chb_Regeneration.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chb_Regeneration.Font = new System.Drawing.Font("標楷體", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chb_Regeneration.Location = new System.Drawing.Point(12, 83);
            this.chb_Regeneration.Name = "chb_Regeneration";
            this.chb_Regeneration.Size = new System.Drawing.Size(427, 55);
            this.chb_Regeneration.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.chb_Regeneration.TabIndex = 3;
            this.chb_Regeneration.Text = "重新產生泡泡圖\r\n(尚未產生過泡泡或想重新更新泡泡)";
            this.chb_Regeneration.CheckedChanged += new System.EventHandler(this.chb_Regeneration_CheckedChanged);
            // 
            // OK
            // 
            this.OK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.OK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.OK.Location = new System.Drawing.Point(120, 151);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 44);
            this.OK.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.OK.TabIndex = 4;
            this.OK.Text = "確定";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Cancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.Cancel.Location = new System.Drawing.Point(253, 151);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 44);
            this.Cancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Cancel.TabIndex = 5;
            this.Cancel.Text = "關閉";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // CreateBallonDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 207);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.chb_Regeneration);
            this.Controls.Add(this.chb_keepOrigination);
            this.DoubleBuffered = true;
            this.EnableGlass = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateBallonDlg";
            this.Text = "泡泡圖選項";
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.StyleManager styleManager1;
        private DevComponents.DotNetBar.Controls.CheckBoxX chb_keepOrigination;
        private DevComponents.DotNetBar.Controls.CheckBoxX chb_Regeneration;
        private DevComponents.DotNetBar.ButtonX OK;
        private DevComponents.DotNetBar.ButtonX Cancel;
    }
}