using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using CaxGlobaltek;
using DevComponents.DotNetBar.Controls;
using System.Windows.Forms;
using DevComponents.DotNetBar.SuperGrid;

namespace OutputExcelForm
{
    //public class Com_MEMain
    //{
    //    public virtual Int32 meSrNo { get; set; }
    //    public virtual Com_PartOperation comPartOperation { get; set; }
    //    public virtual Sys_MEExcel sysMEExcel { get; set; }
    //    public virtual IList<Com_Dimension> comDimension { get; set; }
    //    public virtual string createDate { get; set; }
    //}

    public struct DB_MEMain
    {
        public Com_MEMain comMEMain { get; set; }
        public string excelTemplateFilePath { get; set; }
        public string factory { get; set; }
        //public string excelForm { get; set; }
        //public string meSrNo { get; set; }
        //public string partOperationSrNo { get; set; }
        //public string meExcelSrNo { get; set; }
    }
    public struct DB_Dimension
    {
        public IList<Com_Dimension> comDimension { get; set; }
    }

    public class GetDataFromDatabase
    {
        public static bool status;
        public static int count = -1;
        
        public static ISession session = MyHibernateHelper.SessionFactory.OpenSession();
        public static bool SetCustomerData(ComboBoxEx CusComboBox)
        {
            try
            {
                IList<Sys_Customer> customerName = session.QueryOver<Sys_Customer>().List<Sys_Customer>();
                CusComboBox.DisplayMember = "customerName";
                //CusComboBox.ValueMember = "customerSrNo";
                foreach (Sys_Customer i in customerName)
                {
                    CusComboBox.Items.Add(i);
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool SetPartNoData(Sys_Customer customerSrNo, ComboBoxEx PartNoCombobox)
        {
            try
            {
                //MessageBox.Show(customerSrNo.customerSrNo.ToString());
                IList<Com_PEMain> comPEMain = session.QueryOver<Com_PEMain>().Where(x => x.sysCustomer == customerSrNo).List<Com_PEMain>();
                foreach (Com_PEMain i in comPEMain)
                {
                    if (PartNoCombobox.Items.Contains(i.partName))
                    {
                        continue;
                    }
                    PartNoCombobox.Items.Add(i.partName);
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool SetCusVerData(string PartNoComboboxText, ComboBoxEx CusVerCombobox)
        {
            try
            {
                IList<Com_PEMain> comPEMain = session.QueryOver<Com_PEMain>().Where(x => x.partName == PartNoComboboxText).List<Com_PEMain>();
                CusVerCombobox.DisplayMember = "customerVer";
                CusVerCombobox.ValueMember = "peSrNo";
                foreach (Com_PEMain i in comPEMain)
                {
                    CusVerCombobox.Items.Add(i);
                }
                //CusVerCombobox.Items.Add(comPEMain);

            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool SetOp1Data(Com_PEMain peSrNo, ComboBoxEx Op1Combobox)
        {
            try
            {
                IList<Com_PartOperation> comPartOperation = session.QueryOver<Com_PartOperation>().Where(x => x.comPEMain == peSrNo).List<Com_PartOperation>();
                Op1Combobox.DisplayMember = "operation1";
                Op1Combobox.ValueMember = "partOperationSrNo";
                //Op1Combobox.DataSource = comPartOperation;
                foreach (Com_PartOperation i in comPartOperation)
                {
                    Op1Combobox.Items.Add(i);
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool SetMEExcelData(Com_PartOperation comPartOperation, ref GridPanel SGCPanel)
        {
            try
            {
                IList<Com_MEMain> comMEMain = session.QueryOver<Com_MEMain>().Where(x => x.comPartOperation == comPartOperation).List<Com_MEMain>();

                foreach (Com_MEMain i in comMEMain)
                {
                    count++;

                    #region 由meExcelSrNo取得對應的ExcelType
                    Sys_MEExcel sysMEExcel = session.QueryOver<Sys_MEExcel>().Where(x => x.meExcelSrNo == i.sysMEExcel.meExcelSrNo).SingleOrDefault<Sys_MEExcel>();
                    List<string> ExcelData = new List<string>();
                    status = GetExcelForm.GetMEExcelForm(sysMEExcel.meExcelType, out ExcelData);
                    if (!status)
                    {
                        return false;
                    } 
                    #endregion

                    #region 插入Panel
                    object[] o = new object[] { false, sysMEExcel.meExcelType, ""
                        , string.Format("桌面：{0}_{1}_OP{2}資料夾", OutputForm.PartNoCombobox.Text, OutputForm.CusVerCombobox.Text, OutputForm.Op1Combobox.Text) };
                    SGCPanel.Rows.Add(new GridRow(o));
                    SGCPanel.GetCell(count, 0).Value = false;
                    SGCPanel.GetCell(count, 2).EditorType = typeof(GridComboBoxExEditControl);
                    GridComboBoxExEditControl singleCell = SGCPanel.GetCell(count, 2).EditControl as GridComboBoxExEditControl;
                    singleCell.Items.Add("");
                    foreach (string tempStr in ExcelData)
                    {
                        singleCell.Items.Add(tempStr);
                    }
                    SGCPanel.GetCell(count, 2).Value = "雙擊此區選擇表單";
                    #endregion

                    /*
                    count++;
                    GridCell aaaa = OutputForm.MEPanel.GetCell(count, 2); 
                    aaaa.EditorType = typeof(GridComboBoxExEditControl); 
                    GridComboBoxExEditControl ddd = (GridComboBoxExEditControl)aaaa.EditControl; 
                    foreach (string tempStr in ExcelData)
                    {
                        ddd.Items.Add(tempStr);
                    }
                    */
                }
                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            return true;
        }
        public static bool SetTEExcelData(Com_PartOperation comPartOperation, ref GridPanel SGCPanel)
        {
            try
            {
                IList<Com_TEMain> comTEMain = session.QueryOver<Com_TEMain>().Where(x => x.comPartOperation == comPartOperation).List<Com_TEMain>();
                
                foreach (Com_TEMain i in comTEMain)
                {
                    count++;

                    #region 由teExcelSrNo取得對應的ExcelType
                    Sys_TEExcel sysTEExcel = session.QueryOver<Sys_TEExcel>().Where(x => x.teExcelSrNo == i.sysTEExcel.teExcelSrNo).SingleOrDefault<Sys_TEExcel>();
                    List<string> ExcelData = new List<string>();
                    status = GetExcelForm.GetTEExcelForm(sysTEExcel.teExcelType, out ExcelData);
                    if (!status)
                    {
                        return false;
                    }
                    #endregion

                    #region 插入Panel
                    object[] o = new object[] { false, sysTEExcel.teExcelType, ""
                        , string.Format("桌面：{0}_{1}_OP{2}資料夾", OutputForm.PartNoCombobox.Text, OutputForm.CusVerCombobox.Text, OutputForm.Op1Combobox.Text) };
                    SGCPanel.Rows.Add(new GridRow(o));
                    SGCPanel.GetCell(count, 0).Value = false;
                    SGCPanel.GetCell(count, 2).EditorType = typeof(GridComboBoxExEditControl);
                    GridComboBoxExEditControl singleCell = SGCPanel.GetCell(count, 2).EditControl as GridComboBoxExEditControl;
                    singleCell.Items.Add("");
                    foreach (string tempStr in ExcelData)
                    {
                        singleCell.Items.Add(tempStr);
                    }
                    SGCPanel.GetCell(count, 2).Value = "雙擊此區選擇表單";
                    #endregion
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool GetDimensionDataFromPanel(out Dictionary<DB_MEMain, IList<Com_Dimension>> DicDimenData)
        {
            DicDimenData = new Dictionary<DB_MEMain, IList<Com_Dimension>>();
            try
            {
                for (int i = 0; i < OutputForm.panel.Rows.Count; i++)
                {
                    if (((bool)OutputForm.panel.GetCell(i, 0).Value) == false)
                    {
                        continue;
                    }
                    DB_MEMain sDB_MEMain = new DB_MEMain();
                    Sys_MEExcel meExcelSrNo = session.QueryOver<Sys_MEExcel>().Where(x => x.meExcelType == OutputForm.panel.GetCell(i, 1).Value.ToString()).SingleOrDefault<Sys_MEExcel>();
                    Com_MEMain comMEMain = session.QueryOver<Com_MEMain>()
                                              .Where(x => x.comPartOperation == (Com_PartOperation)OutputForm.Op1Combobox.SelectedItem)
                                              .Where(xx => xx.sysMEExcel == meExcelSrNo)
                                              .SingleOrDefault<Com_MEMain>();
                    IList<Com_Dimension> comDimension = session.QueryOver<Com_Dimension>()
                                                        .Where(x => x.comMEMain == comMEMain)
                                                        .List<Com_Dimension>();
                    sDB_MEMain.comMEMain = comMEMain;
                    sDB_MEMain.excelTemplateFilePath = string.Format(@"{0}\{1}.xls", OutputForm.serverMEConfig, OutputForm.panel.GetCell(i, 2).Value.ToString());
                    sDB_MEMain.factory = OutputForm.panel.GetCell(i, 2).Value.ToString();
                    DicDimenData.Add(sDB_MEMain, comDimension);
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        
    }
}
