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
using DevComponents.DotNetBar;
using OutputExcelForm.Excel;

namespace OutputExcelForm
{
    public partial class OutputForm : Form
    {
        #region 全域變數
        bool status;
        public static GridPanel MEPanel = new GridPanel();
        public static GridPanel TEPanel = new GridPanel();
        public static ISession session = MyHibernateHelper.SessionFactory.OpenSession();
        public static Dictionary<DB_MEMain, IList<Com_Dimension>> DicDimensionData = new Dictionary<DB_MEMain, IList<Com_Dimension>>();
        public static Dictionary<DB_TEMain, IList<Com_ShopDoc>> DicShopDocData = new Dictionary<DB_TEMain, IList<Com_ShopDoc>>();
//         public string partNoComboboxText
//         {
//             get { return PartNoCombobox.Text; }
//         }
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
            MEPanel = SGC_MEPanel.PrimaryGrid;
            TEPanel = SGC_TEPanel.PrimaryGrid;
            //GetDataFromDatabase aaa = new GetDataFromDatabase(this);
            //string[] orderArray = new string[]{};            
        }

        private void InitializeHideControlItems()
        {
            PartNoCombobox.Enabled = false;
            CusVerCombobox.Enabled = false;
            OpVerCombobox.Enabled = false;
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
            OpVerCombobox.Enabled = false;
            Op1Combobox.Enabled = false;

            PartNoCombobox.Items.Clear();
            CusVerCombobox.Items.Clear();
            OpVerCombobox.Items.Clear();
            Op1Combobox.Items.Clear();

            PartNoCombobox.Text = "";
            CusVerCombobox.Text = "";
            OpVerCombobox.Text = "";
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
            OpVerCombobox.Enabled = false;
            Op1Combobox.Enabled = false;

            CusVerCombobox.Items.Clear();
            OpVerCombobox.Items.Clear();
            Op1Combobox.Items.Clear();

            CusVerCombobox.Text = "";
            OpVerCombobox.Text = "";
            Op1Combobox.Text = "";

            status = GetDataFromDatabase.SetCusVerData(((Sys_Customer)CusComboBox.SelectedItem), PartNoCombobox.Text, CusVerCombobox);
            if (!status)
            {
                MessageBox.Show("版次資料取得失敗，請聯繫開發工程師");
                this.Close();
            }
            if (CusVerCombobox.Items.Count == 1)
            {
                CusVerCombobox.Text = CusVerCombobox.Items[0].ToString();
            }
        }

        private void CusVerCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(((Com_PEMain)CusVerCombobox.SelectedItem).peSrNo.ToString());
            OpVerCombobox.Enabled = true;
            Op1Combobox.Enabled = false;

            OpVerCombobox.Items.Clear();
            Op1Combobox.Items.Clear();

            OpVerCombobox.Text = "";
            Op1Combobox.Text = "";

            status = GetDataFromDatabase.SetOpVerData(((Sys_Customer)CusComboBox.SelectedItem), 
                                                        PartNoCombobox.Text, 
                                                        CusVerCombobox.Text, 
                                                        OpVerCombobox);
            if (!status)
            {
                MessageBox.Show("製程資料取得失敗，請聯繫開發工程師");
                this.Close();
            }
            if (OpVerCombobox.Items.Count == 1)
            {
                OpVerCombobox.Text = OpVerCombobox.Items[0].ToString();
            }
        }

        private void OpVerCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Op1Combobox.Enabled = true;
            Op1Combobox.Items.Clear();
            Op1Combobox.Text = "";

            status = GetDataFromDatabase.SetOp1Data(((Sys_Customer)CusComboBox.SelectedItem),
                                                      PartNoCombobox.Text,
                                                      CusVerCombobox.Text,
                                                      OpVerCombobox.Text,
                                                      Op1Combobox);
            if (!status)
            {
                MessageBox.Show("製程資料取得失敗，請聯繫開發工程師");
                this.Close();
            }
        }

        private void Op1Combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MEPanel.Rows.Clear();
            TEPanel.Rows.Clear();
            //從資料庫中取得有關ME的表單資料
            status = GetDataFromDatabase.SetMEExcelData(((Com_PartOperation)Op1Combobox.SelectedItem)
                                                        , PartNoCombobox.Text
                                                        , CusVerCombobox.Text
                                                        , OpVerCombobox.Text
                                                        , Op1Combobox.Text
                                                        , ref MEPanel);
            if (!status)
            {
                MessageBox.Show("SetMEExcelData資料取得失敗，請聯繫開發工程師");
                this.Close();
            }

            //從資料庫中取得有關TE的表單資料
            status = GetDataFromDatabase.SetTEExcelData(((Com_PartOperation)Op1Combobox.SelectedItem)
                                                        , PartNoCombobox.Text
                                                        , CusVerCombobox.Text
                                                        , OpVerCombobox.Text
                                                        , Op1Combobox.Text
                                                        , ref TEPanel);
            if (!status)
            {
                MessageBox.Show("SetTEExcelData資料取得失敗，請聯繫開發工程師");
                this.Close();
            }

        }

        private void OK_Click(object sender, EventArgs e)
        {
            string ExcelFolder = string.Format(@"{0}\{1}_{2}_{3}_OP{4}"
                                                , Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                                                , PartNoCombobox.Text
                                                , CusVerCombobox.Text
                                                , OpVerCombobox.Text
                                                , Op1Combobox.Text);

            status = Excel_CommonFun.CheckExcelProcess();
            if (!status)
            {
                MessageBox.Show("請先關閉所有Excel再重新執行輸出，如沒有EXCEL在執行，請開啟工作管理員關閉背景EXCEL");
                return;
            }

            if (SuperTabControl.SelectedTab.Text == "ME表單")
            {
                //ME_檢查是否有選取表單格式
                status = CheckFun.CheckAll("ME", MEPanel);
                if (!status)
                {
                    return;
                }

                //ME_建立桌面資料夾存放產生的Excel
                if (!Directory.Exists(ExcelFolder))
                {
                    Directory.CreateDirectory(ExcelFolder);
                }

                //ME_由選取的Op1與Excel表單，查出資料庫的Com_MEMain
                status = GetDataFromDatabase.GetDimensionData(Op1Combobox, out DicDimensionData);
                if (!status)
                {
                    MessageBox.Show("由Panel資料查DicDimensionData時發生錯誤，請聯繫開發工程師");
                    this.Close();
                }
                //ME_開始輸出ME的Excel
                status = GetExcelForm.InsertDataToMEExcel(PartNoCombobox.Text, CusVerCombobox.Text, OpVerCombobox.Text, Op1Combobox.Text);
                if (!status)
                {
                    MessageBox.Show("輸出ME的Excel時發生錯誤，請聯繫開發工程師");
                    this.Close();
                }
            }
            else if (SuperTabControl.SelectedTab.Text == "TE表單")
            {
                //TE_檢查是否有選取表單格式
                status = CheckFun.CheckAll("TE", TEPanel);
                if (!status)
                {
                    return;
                }

                //TE_建立桌面資料夾存放產生的Excel
                if (!Directory.Exists(ExcelFolder))
                {
                    Directory.CreateDirectory(ExcelFolder);
                }

                //TE_由選取的Op1與Excel表單，查出資料庫的Com_TEMain
                status = GetDataFromDatabase.GetShopDocData(Op1Combobox, out DicShopDocData);
                if (!status)
                {
                    MessageBox.Show("由Panel資料查DicShopDocData時發生錯誤，請聯繫開發工程師");
                    this.Close();
                }
                //TE_開始輸出TE的Excel
                status = GetExcelForm.InsertDataToTEExcel(PartNoCombobox.Text, CusVerCombobox.Text, OpVerCombobox.Text, Op1Combobox.Text);
                if (!status)
                {
                    MessageBox.Show("輸出TE的Excel時發生錯誤，請聯繫開發工程師");
                    this.Close();
                }
            }

            MessageBox.Show("表單輸出完成！");
            this.Close();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        


    }
}
