namespace AddDeleteDB
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.TabControl = new DevComponents.DotNetBar.SuperTabControl();
            this.STC_Panel = new DevComponents.DotNetBar.SuperTabControlPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DelButton = new DevComponents.DotNetBar.ButtonX();
            this.SelNum = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AddButton = new DevComponents.DotNetBar.ButtonX();
            this.AddText = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.superTabItem1 = new DevComponents.DotNetBar.SuperTabItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.TabControl)).BeginInit();
            this.TabControl.SuspendLayout();
            this.STC_Panel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            this.TabControl.ControlBox.CloseBox.Name = "";
            // 
            // 
            // 
            this.TabControl.ControlBox.MenuBox.Name = "";
            this.TabControl.ControlBox.Name = "";
            this.TabControl.ControlBox.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.TabControl.ControlBox.MenuBox,
            this.TabControl.ControlBox.CloseBox});
            this.TabControl.Controls.Add(this.STC_Panel);
            this.TabControl.Location = new System.Drawing.Point(12, 12);
            this.TabControl.Name = "TabControl";
            this.TabControl.ReorderTabsEnabled = true;
            this.TabControl.SelectedTabFont = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold);
            this.TabControl.SelectedTabIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(370, 274);
            this.TabControl.TabFont = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TabControl.TabIndex = 0;
            this.TabControl.Tabs.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.superTabItem1});
            this.TabControl.Text = "superTabControl1";
            this.TabControl.SelectedTabChanged += new System.EventHandler<DevComponents.DotNetBar.SuperTabStripSelectedTabChangedEventArgs>(this.TabControl_SelectedTabChanged);
            // 
            // STC_Panel
            // 
            this.STC_Panel.Controls.Add(this.groupBox2);
            this.STC_Panel.Controls.Add(this.groupBox1);
            this.STC_Panel.Controls.Add(this.listView1);
            this.STC_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.STC_Panel.Location = new System.Drawing.Point(0, 62);
            this.STC_Panel.Name = "STC_Panel";
            this.STC_Panel.Size = new System.Drawing.Size(370, 212);
            this.STC_Panel.TabIndex = 0;
            this.STC_Panel.TabItem = this.superTabItem1;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.DelButton);
            this.groupBox2.Controls.Add(this.SelNum);
            this.groupBox2.Controls.Add(this.labelX2);
            this.groupBox2.Font = new System.Drawing.Font("標楷體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox2.Location = new System.Drawing.Point(214, 126);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(153, 74);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "移除資料";
            // 
            // DelButton
            // 
            this.DelButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.DelButton.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.DelButton.Location = new System.Drawing.Point(6, 45);
            this.DelButton.Name = "DelButton";
            this.DelButton.Size = new System.Drawing.Size(141, 23);
            this.DelButton.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.DelButton.TabIndex = 2;
            this.DelButton.Text = "刪除";
            this.DelButton.Click += new System.EventHandler(this.DelButton_Click);
            // 
            // SelNum
            // 
            // 
            // 
            // 
            this.SelNum.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.SelNum.Location = new System.Drawing.Point(88, 20);
            this.SelNum.Name = "SelNum";
            this.SelNum.Size = new System.Drawing.Size(59, 23);
            this.SelNum.TabIndex = 1;
            this.SelNum.Text = "labelX3";
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(6, 22);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(96, 23);
            this.labelX2.TabIndex = 0;
            this.labelX2.Text = "已選擇數量：";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.AddButton);
            this.groupBox1.Controls.Add(this.AddText);
            this.groupBox1.Controls.Add(this.labelX1);
            this.groupBox1.Font = new System.Drawing.Font("標楷體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox1.Location = new System.Drawing.Point(214, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(153, 108);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "新增資料";
            // 
            // AddButton
            // 
            this.AddButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.AddButton.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.AddButton.Location = new System.Drawing.Point(6, 79);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(141, 23);
            this.AddButton.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.AddButton.TabIndex = 2;
            this.AddButton.Text = "新增";
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // AddText
            // 
            // 
            // 
            // 
            this.AddText.Border.Class = "TextBoxBorder";
            this.AddText.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.AddText.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.AddText.Location = new System.Drawing.Point(6, 51);
            this.AddText.Name = "AddText";
            this.AddText.PreventEnterBeep = true;
            this.AddText.Size = new System.Drawing.Size(141, 23);
            this.AddText.TabIndex = 1;
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(6, 22);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(75, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "手動輸入：";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(205, 197);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseUp);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "已存在資料";
            this.columnHeader1.Width = 201;
            // 
            // superTabItem1
            // 
            this.superTabItem1.AttachedControl = this.STC_Panel;
            this.superTabItem1.GlobalItem = false;
            this.superTabItem1.Image = global::AddDeleteDB.Properties.Resources.company_48px;
            this.superTabItem1.Name = "superTabItem1";
            this.superTabItem1.Text = "客戶";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "company_48px.png");
            this.imageList1.Images.SetKeyName(1, "Database_128px.ico");
            this.imageList1.Images.SetKeyName(2, "OIS_48px.bmp");
            this.imageList1.Images.SetKeyName(3, "material_48px.bmp");
            this.imageList1.Images.SetKeyName(4, "material_48px.png");
            this.imageList1.Images.SetKeyName(5, "OIS_48px.png");
            this.imageList1.Images.SetKeyName(6, "files_32px.png");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 296);
            this.Controls.Add(this.TabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "新增/移除資料庫";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.TabControl)).EndInit();
            this.TabControl.ResumeLayout(false);
            this.STC_Panel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.SuperTabControl TabControl;
        private DevComponents.DotNetBar.SuperTabControlPanel STC_Panel;
        private DevComponents.DotNetBar.SuperTabItem superTabItem1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.GroupBox groupBox2;
        private DevComponents.DotNetBar.ButtonX DelButton;
        private DevComponents.DotNetBar.LabelX SelNum;
        private DevComponents.DotNetBar.LabelX labelX2;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevComponents.DotNetBar.ButtonX AddButton;
        private DevComponents.DotNetBar.Controls.TextBoxX AddText;
        private DevComponents.DotNetBar.LabelX labelX1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}

