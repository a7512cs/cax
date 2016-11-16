namespace DeleteDataBase
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.CusCombobox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.PartNumCombobox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.CusVerCombobox = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.buttonX1 = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX1.Location = new System.Drawing.Point(23, 26);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(75, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "客戶：";
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX2.Location = new System.Drawing.Point(23, 65);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(75, 23);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "料號：";
            // 
            // CusCombobox
            // 
            this.CusCombobox.DisplayMember = "Text";
            this.CusCombobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CusCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CusCombobox.FormattingEnabled = true;
            this.CusCombobox.ItemHeight = 16;
            this.CusCombobox.Location = new System.Drawing.Point(77, 26);
            this.CusCombobox.Name = "CusCombobox";
            this.CusCombobox.Size = new System.Drawing.Size(121, 22);
            this.CusCombobox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.CusCombobox.TabIndex = 2;
            this.CusCombobox.SelectedIndexChanged += new System.EventHandler(this.CusCombobox_SelectedIndexChanged);
            // 
            // PartNumCombobox
            // 
            this.PartNumCombobox.DisplayMember = "Text";
            this.PartNumCombobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.PartNumCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PartNumCombobox.FormattingEnabled = true;
            this.PartNumCombobox.ItemHeight = 16;
            this.PartNumCombobox.Location = new System.Drawing.Point(77, 66);
            this.PartNumCombobox.Name = "PartNumCombobox";
            this.PartNumCombobox.Size = new System.Drawing.Size(121, 22);
            this.PartNumCombobox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.PartNumCombobox.TabIndex = 3;
            this.PartNumCombobox.SelectedIndexChanged += new System.EventHandler(this.PartNumCombobox_SelectedIndexChanged);
            // 
            // CusVerCombobox
            // 
            this.CusVerCombobox.DisplayMember = "Text";
            this.CusVerCombobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CusVerCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CusVerCombobox.FormattingEnabled = true;
            this.CusVerCombobox.ItemHeight = 16;
            this.CusVerCombobox.Location = new System.Drawing.Point(77, 106);
            this.CusVerCombobox.Name = "CusVerCombobox";
            this.CusVerCombobox.Size = new System.Drawing.Size(121, 22);
            this.CusVerCombobox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.CusVerCombobox.TabIndex = 4;
            // 
            // labelX3
            // 
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX3.Location = new System.Drawing.Point(23, 105);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(75, 23);
            this.labelX3.TabIndex = 5;
            this.labelX3.Text = "版次：";
            // 
            // buttonX1
            // 
            this.buttonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1.Location = new System.Drawing.Point(77, 160);
            this.buttonX1.Name = "buttonX1";
            this.buttonX1.Size = new System.Drawing.Size(75, 23);
            this.buttonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX1.TabIndex = 6;
            this.buttonX1.Text = "刪除";
            this.buttonX1.Click += new System.EventHandler(this.buttonX1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 203);
            this.Controls.Add(this.buttonX1);
            this.Controls.Add(this.CusVerCombobox);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.PartNumCombobox);
            this.Controls.Add(this.CusCombobox);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx CusCombobox;
        private DevComponents.DotNetBar.Controls.ComboBoxEx PartNumCombobox;
        private DevComponents.DotNetBar.Controls.ComboBoxEx CusVerCombobox;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.ButtonX buttonX1;
    }
}

