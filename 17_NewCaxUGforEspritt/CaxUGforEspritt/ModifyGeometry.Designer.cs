using NXOpen;
namespace CaxUGforEspritt
{
    partial class ModifyGeometry
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Body TargetBody;
        
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModifyGeometry));
            this.buttonX1TargetFace = new DevComponents.DotNetBar.ButtonX();
            this.buttonX1ToolFace = new DevComponents.DotNetBar.ButtonX();
            this.buttonX1Execute = new DevComponents.DotNetBar.ButtonX();
            this.buttonX1Undo = new DevComponents.DotNetBar.ButtonX();
            this.buttonX3Cancel = new DevComponents.DotNetBar.ButtonX();
            this.buttonX2Confirm = new DevComponents.DotNetBar.ButtonX();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.styleManager1 = new DevComponents.DotNetBar.StyleManager(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonX1TargetFace
            // 
            this.buttonX1TargetFace.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1TargetFace.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1TargetFace.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonX1TargetFace.Location = new System.Drawing.Point(22, 38);
            this.buttonX1TargetFace.Name = "buttonX1TargetFace";
            this.buttonX1TargetFace.Size = new System.Drawing.Size(285, 43);
            this.buttonX1TargetFace.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX1TargetFace.TabIndex = 0;
            this.buttonX1TargetFace.Text = "1.選擇欲被取代的面(Target Face)";
            this.buttonX1TargetFace.Click += new System.EventHandler(this.buttonX1TargetFace_Click);
            // 
            // buttonX1ToolFace
            // 
            this.buttonX1ToolFace.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1ToolFace.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1ToolFace.Location = new System.Drawing.Point(22, 108);
            this.buttonX1ToolFace.Name = "buttonX1ToolFace";
            this.buttonX1ToolFace.Size = new System.Drawing.Size(285, 43);
            this.buttonX1ToolFace.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX1ToolFace.TabIndex = 1;
            this.buttonX1ToolFace.Text = "2.選擇欲取代的面(Tool Face)";
            this.buttonX1ToolFace.Click += new System.EventHandler(this.buttonX1ToolFace_Click);
            // 
            // buttonX1Execute
            // 
            this.buttonX1Execute.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1Execute.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1Execute.Location = new System.Drawing.Point(35, 216);
            this.buttonX1Execute.Name = "buttonX1Execute";
            this.buttonX1Execute.Size = new System.Drawing.Size(121, 44);
            this.buttonX1Execute.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX1Execute.TabIndex = 2;
            this.buttonX1Execute.Text = "執行取代功能";
            this.buttonX1Execute.Click += new System.EventHandler(this.buttonX1Execute_Click);
            // 
            // buttonX1Undo
            // 
            this.buttonX1Undo.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1Undo.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1Undo.Location = new System.Drawing.Point(197, 216);
            this.buttonX1Undo.Name = "buttonX1Undo";
            this.buttonX1Undo.Size = new System.Drawing.Size(123, 44);
            this.buttonX1Undo.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX1Undo.TabIndex = 3;
            this.buttonX1Undo.Text = "回復上一步";
            this.buttonX1Undo.Click += new System.EventHandler(this.buttonX1Undo_Click);
            // 
            // buttonX3Cancel
            // 
            this.buttonX3Cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX3Cancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX3Cancel.Location = new System.Drawing.Point(262, 311);
            this.buttonX3Cancel.Name = "buttonX3Cancel";
            this.buttonX3Cancel.Size = new System.Drawing.Size(74, 45);
            this.buttonX3Cancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX3Cancel.TabIndex = 5;
            this.buttonX3Cancel.Text = "取消";
            this.buttonX3Cancel.Click += new System.EventHandler(this.buttonX3Cancel_Click);
            // 
            // buttonX2Confirm
            // 
            this.buttonX2Confirm.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX2Confirm.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX2Confirm.Location = new System.Drawing.Point(173, 311);
            this.buttonX2Confirm.Name = "buttonX2Confirm";
            this.buttonX2Confirm.Size = new System.Drawing.Size(74, 45);
            this.buttonX2Confirm.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX2Confirm.TabIndex = 4;
            this.buttonX2Confirm.Text = "確認";
            this.buttonX2Confirm.Click += new System.EventHandler(this.buttonX2Confirm_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonX1TargetFace);
            this.groupBox1.Controls.Add(this.buttonX1ToolFace);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 177);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "請選擇欲修改的面";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CaxUGforEspritt.Properties.Resources._1_logo2;
            this.pictureBox1.Location = new System.Drawing.Point(13, 281);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(154, 102);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            this.styleManager1.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154))))));
            // 
            // ModifyGeometry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 394);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonX3Cancel);
            this.Controls.Add(this.buttonX2Confirm);
            this.Controls.Add(this.buttonX1Undo);
            this.Controls.Add(this.buttonX1Execute);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ModifyGeometry";
            this.Text = "幾何圖形修改";
            this.Load += new System.EventHandler(this.ModifyGeometry_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX buttonX1TargetFace;
        private DevComponents.DotNetBar.ButtonX buttonX1ToolFace;
        private DevComponents.DotNetBar.ButtonX buttonX1Execute;
        private DevComponents.DotNetBar.ButtonX buttonX1Undo;
        private DevComponents.DotNetBar.ButtonX buttonX3Cancel;
        private DevComponents.DotNetBar.ButtonX buttonX2Confirm;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevComponents.DotNetBar.StyleManager styleManager1;
    }
}