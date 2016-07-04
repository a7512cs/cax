using NXOpen;
using System.Collections.Generic;
using WeData;
namespace CaxUGforEspritt
{
    partial class ZMachining
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public NXOpen.Assemblies.Component WEComp ;
        public Body WEBody;
        public List<Face> ListFaceToInsertAttr = new List<Face>();
        public Face Base_Face;
        public List<DisplayableObject> hideDispalyObject = new List<DisplayableObject>();
        public KeyValuePair<skey, string> kvp;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZMachining));
            this.buttonX1_SelectFaces = new DevComponents.DotNetBar.ButtonX();
            this.buttonX1_Execute = new DevComponents.DotNetBar.ButtonX();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonX1_Closed = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonX1_SelectFaces
            // 
            this.buttonX1_SelectFaces.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1_SelectFaces.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1_SelectFaces.Location = new System.Drawing.Point(23, 13);
            this.buttonX1_SelectFaces.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonX1_SelectFaces.Name = "buttonX1_SelectFaces";
            this.buttonX1_SelectFaces.Size = new System.Drawing.Size(236, 27);
            this.buttonX1_SelectFaces.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX1_SelectFaces.TabIndex = 1;
            this.buttonX1_SelectFaces.Text = "選擇";
            this.buttonX1_SelectFaces.Click += new System.EventHandler(this.buttonX1_SelectFaces_Click);
            // 
            // buttonX1_Execute
            // 
            this.buttonX1_Execute.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1_Execute.Location = new System.Drawing.Point(0, 0);
            this.buttonX1_Execute.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonX1_Execute.Name = "buttonX1_Execute";
            this.buttonX1_Execute.Size = new System.Drawing.Size(0, 0);
            this.buttonX1_Execute.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CaxUGforEspritt.Properties.Resources.附件1_圖文平行;
            this.pictureBox1.InitialImage = global::CaxUGforEspritt.Properties.Resources._1_logo11;
            this.pictureBox1.Location = new System.Drawing.Point(9, 88);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(224, 40);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // buttonX1_Closed
            // 
            this.buttonX1_Closed.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1_Closed.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1_Closed.Location = new System.Drawing.Point(238, 88);
            this.buttonX1_Closed.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonX1_Closed.Name = "buttonX1_Closed";
            this.buttonX1_Closed.Size = new System.Drawing.Size(56, 40);
            this.buttonX1_Closed.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX1_Closed.TabIndex = 4;
            this.buttonX1_Closed.Text = "關閉";
            this.buttonX1_Closed.Click += new System.EventHandler(this.buttonX1_Closed_Click);
            // 
            // groupPanel1
            // 
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.buttonX1_SelectFaces);
            this.groupPanel1.Location = new System.Drawing.Point(9, 10);
            this.groupPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(285, 74);
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
            this.groupPanel1.TabIndex = 6;
            this.groupPanel1.Text = "請選擇線割面建立適當加工路徑";
            // 
            // ZMachining
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 135);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.buttonX1_Closed);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonX1_Execute);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ZMachining";
            this.Text = "ZMachining";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX buttonX1_SelectFaces;
        private DevComponents.DotNetBar.ButtonX buttonX1_Execute;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevComponents.DotNetBar.ButtonX buttonX1_Closed;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;

    }
}