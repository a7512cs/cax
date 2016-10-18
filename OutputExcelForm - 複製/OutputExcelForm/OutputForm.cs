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

namespace OutputExcelForm
{
    public partial class OutputForm : Form
    {
        #region 全域變數
        bool status;
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
            GridPanel MEPanel = SGCPanel_ME.PrimaryGrid;
            string[] orderArray = new string[]{};

            MEPanel.Columns["表單選擇"].EditorType = typeof(ExcelComboBox);
            MEPanel.Columns["表單選擇"].EditorParams = new object[] { orderArray };
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
            //MessageBox.Show(((Com_PartOperation)Op1Combobox.SelectedItem).partOperationSrNo.ToString());
            status = GetDataFromDatabase.SetMEPanel(((Com_PartOperation)Op1Combobox.SelectedItem), ref SGCPanel_ME);
            if (!status)
            {
                MessageBox.Show("SetMEPanel資料取得失敗，請聯繫開發工程師");
                this.Close();
            }
        }

    }
}
