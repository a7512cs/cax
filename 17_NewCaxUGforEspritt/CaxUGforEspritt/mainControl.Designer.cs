namespace CaxUGforEspritt
{
    partial class mainControl
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
            this.btnConfirm = new System.Windows.Forms.Button();
            this.supGridMain = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.SuspendLayout();
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(470, 706);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(100, 35);
            this.btnConfirm.TabIndex = 1;
            this.btnConfirm.Text = "確定";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // supGridMain
            // 
            this.supGridMain.DefaultVisualStyles.ColumnHeaderStyles.Default.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.supGridMain.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.supGridMain.Font = new System.Drawing.Font("微軟正黑體", 8F);
            this.supGridMain.Location = new System.Drawing.Point(13, 13);
            this.supGridMain.Name = "supGridMain";
            this.supGridMain.PrimaryGrid.DefaultVisualStyles.ColumnHeaderStyles.Default.Font = new System.Drawing.Font("微軟正黑體", 8F);
            this.supGridMain.PrimaryGrid.MultiSelect = false;
            this.supGridMain.PrimaryGrid.SelectionGranularity = DevComponents.DotNetBar.SuperGrid.SelectionGranularity.Row;
            this.supGridMain.Size = new System.Drawing.Size(557, 687);
            this.supGridMain.TabIndex = 2;
            this.supGridMain.Text = "superGridControl1";
            // 
            // mainControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 753);
            this.Controls.Add(this.supGridMain);
            this.Controls.Add(this.btnConfirm);
            this.DoubleBuffered = true;
            this.EnableGlass = false;
            this.Name = "mainControl";
            this.Text = "mainControl";
            this.Load += new System.EventHandler(this.mainControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.StyleManager styleManager1;
        private System.Windows.Forms.Button btnConfirm;
        private DevComponents.DotNetBar.SuperGrid.SuperGridControl supGridMain;
    }
}