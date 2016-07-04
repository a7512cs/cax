using WeData;
namespace CaxUGforEspritt
{
    partial class MultiTolDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public static TolValue MultiTol = new TolValue();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiTolDlg));
            this.superGridControlMultiTol = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonX1 = new DevComponents.DotNetBar.ButtonX();
            this.styleManager1 = new DevComponents.DotNetBar.StyleManager(this.components);
            this.buttonX2_Cancel = new DevComponents.DotNetBar.ButtonX();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // superGridControlMultiTol
            // 
            this.superGridControlMultiTol.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.superGridControlMultiTol.Location = new System.Drawing.Point(9, 10);
            this.superGridControlMultiTol.Margin = new System.Windows.Forms.Padding(2);
            this.superGridControlMultiTol.Name = "superGridControlMultiTol";
            gridColumn1.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridCheckBoxXEditControl);
            gridColumn1.HeaderText = "#";
            gridColumn1.Name = "選擇";
            gridColumn1.Width = 30;
            gridColumn2.HeaderText = "公差色";
            gridColumn2.Name = "公差色";
            gridColumn2.ReadOnly = true;
            gridColumn2.Width = 80;
            gridColumn3.Name = "上公差";
            gridColumn3.ReadOnly = true;
            gridColumn3.Width = 80;
            gridColumn4.Name = "下公差";
            gridColumn4.ReadOnly = true;
            gridColumn4.Width = 80;
            gridColumn5.AutoSizeMode = DevComponents.DotNetBar.SuperGrid.ColumnAutoSizeMode.Fill;
            gridColumn5.Name = "範圍(上-下)";
            gridColumn5.ReadOnly = true;
            this.superGridControlMultiTol.PrimaryGrid.Columns.Add(gridColumn1);
            this.superGridControlMultiTol.PrimaryGrid.Columns.Add(gridColumn2);
            this.superGridControlMultiTol.PrimaryGrid.Columns.Add(gridColumn3);
            this.superGridControlMultiTol.PrimaryGrid.Columns.Add(gridColumn4);
            this.superGridControlMultiTol.PrimaryGrid.Columns.Add(gridColumn5);
            this.superGridControlMultiTol.PrimaryGrid.MultiSelect = false;
            this.superGridControlMultiTol.Size = new System.Drawing.Size(405, 177);
            this.superGridControlMultiTol.TabIndex = 0;
            this.superGridControlMultiTol.Text = "superGridControl1";
            this.superGridControlMultiTol.CellValueChanged += new System.EventHandler<DevComponents.DotNetBar.SuperGrid.GridCellValueChangedEventArgs>(this.superGridControlMultiTol_CellValueChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CaxUGforEspritt.Properties.Resources._1_logo2;
            this.pictureBox1.Location = new System.Drawing.Point(9, 191);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(90, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // buttonX1
            // 
            this.buttonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1.Location = new System.Drawing.Point(257, 192);
            this.buttonX1.Margin = new System.Windows.Forms.Padding(2);
            this.buttonX1.Name = "buttonX1";
            this.buttonX1.Size = new System.Drawing.Size(76, 49);
            this.buttonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX1.TabIndex = 2;
            this.buttonX1.Text = "確定";
            this.buttonX1.Click += new System.EventHandler(this.buttonX1_Click);
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // buttonX2_Cancel
            // 
            this.buttonX2_Cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX2_Cancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX2_Cancel.Location = new System.Drawing.Point(338, 192);
            this.buttonX2_Cancel.Name = "buttonX2_Cancel";
            this.buttonX2_Cancel.Size = new System.Drawing.Size(76, 49);
            this.buttonX2_Cancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX2_Cancel.TabIndex = 3;
            this.buttonX2_Cancel.Text = "取消";
            this.buttonX2_Cancel.Click += new System.EventHandler(this.buttonX2_Cancel_Click);
            // 
            // MultiTolDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 250);
            this.Controls.Add(this.buttonX2_Cancel);
            this.Controls.Add(this.buttonX1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.superGridControlMultiTol);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MultiTolDlg";
            this.Text = "MultiTolDlg";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MultiTolDlg_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MultiTolDlg_FormClosed);
            this.Load += new System.EventHandler(this.MultiTolDlg_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.SuperGrid.SuperGridControl superGridControlMultiTol;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevComponents.DotNetBar.ButtonX buttonX1;
        private DevComponents.DotNetBar.StyleManager styleManager1;
        private DevComponents.DotNetBar.ButtonX buttonX2_Cancel;
    }
}