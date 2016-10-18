using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CaxGlobaltek;
using DevComponents.DotNetBar.SuperGrid;
using System.Collections;
using NHibernate;
using System.IO;

namespace OutputExcelForm
{
    public partial class OutputForm : Form
    {
        #region 全域變數
        bool status;
        public static GridPanel panel = new GridPanel();
        public static ISession session = MyHibernateHelper.SessionFactory.OpenSession();
        public static string serverMEConfig = "", serverTEConfig = "";
        public static Dictionary<DB_MEMain, IList<Com_Dimension>> DicDimenData = new Dictionary<DB_MEMain, IList<Com_Dimension>>();
        #endregion

        public OutputForm()
        {
            InitializeComponent();
            InitializeGrid();
            InitializeHideControlItems();
            GetCustomerFromDatabase();
        }

        private void InitializeGrid()
        {
            panel = SGCPanel.PrimaryGrid;

            
            //string[] orderArray = new string[]{};            
        }

        private void InitializeHideControlItems()
        {
            PartNoCombobox.Enabled = false;
            CusVerCombobox.Enabled = false;
            Op1Combobox.Enabled = false;
        }

        private void GetCustomerFromDatabase()
        {
            status = GetDataFromDatabase.SetCustomerData(CusComboBox);
            if (!status)
            {
                MessageBox.Show("客戶資料取得失敗，請聯繫開發工程師");
                this.Close();
            }
        }

        private void CusComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PartNoCombobox.Enabled = true;
            CusVerCombobox.Enabled = false;
            Op1Combobox.Enabled = false;

            PartNoCombobox.Items.Clear();
            CusVerCombobox.Items.Clear();
            Op1Combobox.Items.Clear();

            PartNoCombobox.Text = "";
            CusVerCombobox.Text = "";
            Op1Combobox.Text = "";

            status = GetDataFromDatabase.SetPartNoData(((Sys_Customer)CusComboBox.SelectedItem), PartNoCombobox);
            if (!status)
            {
                MessageBox.Show("料號資料取得失敗，請聯繫開發工程師");
                this.Close();
            }
        }

        private void PartNoCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CusVerCombobox.Enabled = true;
            Op1Combobox.Enabled = false;

            CusVerCombobox.Items.Clear();
            Op1Combobox.Items.Clear();

            CusVerCombobox.Text = "";
            Op1Combobox.Text = "";

            status = GetDataFromDatabase.SetCusVerData(PartNoCombobox.Text, CusVerCombobox);
            if (!status)
            {
                MessageBox.Show("版次資料取得失敗，請聯繫開發工程師");
                this.Close();
            }
            if (CusVerCombobox.Items.Count == 1)
            {
                CusVerCombobox.Text = ((Com_PEMain)CusVerCombobox.Items[0]).customerVer;
                //MessageBox.Show(((Com_PEMain)CusVerCombobox.Items[0]).customerVer);
            }
        }

        private void CusVerCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(((Com_PEMain)CusVerCombobox.SelectedItem).peSrNo.ToString());
            Op1Combobox.Enabled = true;

            Op1Combobox.Items.Clear();

            Op1Combobox.Text = "";


            status = GetDataFromDatabase.SetOp1Data(((Com_PEMain)CusVerCombobox.SelectedItem), Op1Combobox);
            if (!status)
            {
                MessageBox.Show("製程資料取得失敗，請聯繫開發工程師");
                this.Close();
            }
        }

        private void Op1Combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel.Rows.Clear();
            //從資料庫中取得有關ME的表單資料
            status = GetDataFromDatabase.SetMEExcelData(((Com_PartOperation)Op1Combobox.SelectedItem), ref panel);
            if (!status)
            {
                MessageBox.Show("SetMEExcelData資料取得失敗，請聯繫開發工程師");
                this.Close();
            }

            //從資料庫中取得有關TE的表單資料
            status = GetDataFromDatabase.SetTEExcelData(((Com_PartOperation)Op1Combobox.SelectedItem), ref panel);
            if (!status)
            {
                MessageBox.Show("SetTEExcelData資料取得失敗，請聯繫開發工程師");
                this.Close();
            }

        }

        private void OK_Click(object sender, EventArgs e)
        {
            //檢查是否有選取表單格式
            status = CheckFun.CheckAll();
            if (!status)
            {
                return;
            }

            //建立桌面資料夾存放產生的Excel
            string ExcelFolder = string.Format(@"{0}\{1}_{2}_OP{3}", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), PartNoCombobox.Text, CusVerCombobox.Text, Op1Combobox.Text);
            if (!Directory.Exists(ExcelFolder))
            {
                Directory.CreateDirectory(ExcelFolder);
            }
            
            try
            {
                //由選取的Op1與Excel表單，查出資料庫的Com_MEMain
                status = GetDataFromDatabase.GetDimensionDataFromPanel(out DicDimenData);
                if (!status)
                {
                    MessageBox.Show("由Panel資料查DB時發生錯誤，請聯繫開發工程師");
                    this.Close();
                }
                //開始輸出ME的Excel
                status = GetExcelForm.InsertDataToExcel();
                if (!status)
                {
                    MessageBox.Show("輸出ME的Excel時發生錯誤，請聯繫開發工程師");
                    this.Close();
                }

                //由選取的Op1與Excel表單，查出資料庫的Com_TEMain


                MessageBox.Show("表單輸出完成！");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.Close();
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        

    }
}
