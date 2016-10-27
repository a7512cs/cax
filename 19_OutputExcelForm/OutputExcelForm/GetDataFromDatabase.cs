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
    }
    public struct DB_Dimension
    {
        public IList<Com_Dimension> comDimension { get; set; }
    }

    public struct DB_TEMain
    {
        public Com_TEMain comTEMain { get; set; }
        public string excelTemplateFilePath { get; set; }
        public string ncGroupName { get; set; }
        public string factory { get; set; }
    }
    public struct DB_ShopDoc
    {
        public IList<Com_ShopDoc> comShopDoc { get; set; }
    }

    public class GetDataFromDatabase
    {
        public static bool status;
        public static ISession session = MyHibernateHelper.SessionFactory.OpenSession();

//         public static OutputForm aa;
// 
//         public GetDataFromDatabase(OutputForm bb)
//         {
//             aa = bb;
//         }


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
                IList<Com_PEMain> comPEMain = session.QueryOver<Com_PEMain>()
                                              .Where(x => x.sysCustomer == customerSrNo)
                                              .List<Com_PEMain>();
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
                IList<Com_PEMain> comPEMain = session.QueryOver<Com_PEMain>()
                                              .Where(x => x.partName == PartNoComboboxText)
                                              .List<Com_PEMain>();
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
                IList<Com_PartOperation> comPartOperation = session.QueryOver<Com_PartOperation>()
                                                            .Where(x => x.comPEMain == peSrNo)
                                                            .List<Com_PartOperation>();
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
        public static bool SetMEExcelData(Com_PartOperation comPartOperation, string partNo, string cusVer, string op1, ref GridPanel MEPanel)
        {
            try
            {
                IList<Com_MEMain> comMEMain = session.QueryOver<Com_MEMain>()
                                              .Where(x => x.comPartOperation == comPartOperation).List<Com_MEMain>();
                int MECount = 0;
                foreach (Com_MEMain i in comMEMain)
                {
                    #region 由meExcelSrNo取得對應的ExcelType
                    Sys_MEExcel sysMEExcel = session.QueryOver<Sys_MEExcel>()
                                             .Where(x => x.meExcelSrNo == i.sysMEExcel.meExcelSrNo)
                                             .SingleOrDefault<Sys_MEExcel>();
                    List<string> ExcelData = new List<string>();
                    status = GetExcelForm.GetMEExcelForm(sysMEExcel.meExcelType, out ExcelData);
                    if (!status)
                    {
                        return false;
                    } 
                    #endregion

                    #region 插入Panel
                    object[] o = new object[] { false, sysMEExcel.meExcelType, ""
                                                , string.Format("{0}_{1}_OP{2}資料夾"
                                                , partNo
                                                , cusVer
                                                , op1) };
                    MEPanel.Rows.Add(new GridRow(o));
                    MEPanel.GetCell(MECount, 0).Value = false;
                    MEPanel.GetCell(MECount, 2).EditorType = typeof(GridComboBoxExEditControl);
                    GridComboBoxExEditControl singleCell = MEPanel.GetCell(MECount, 2).EditControl as GridComboBoxExEditControl;
                    singleCell.Items.Add("");
                    foreach (string tempStr in ExcelData)
                    {
                        singleCell.Items.Add(tempStr);
                    }
                    MEPanel.GetCell(MECount, 2).Value = "(雙擊)選擇表單";
                    #endregion

                    MECount++;

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
        public static bool SetTEExcelData(Com_PartOperation comPartOperation, string partNo, string cusVer, string op1, ref GridPanel TEPanel)
        {
            try
            {
                IList<Com_TEMain> comTEMain = session.QueryOver<Com_TEMain>()
                                              .Where(x => x.comPartOperation == comPartOperation).List<Com_TEMain>();
                int TECount = 0;
                foreach (Com_TEMain i in comTEMain)
                {
                    #region 由teExcelSrNo取得對應的ExcelType
                    Sys_TEExcel sysTEExcel = session.QueryOver<Sys_TEExcel>()
                                             .Where(x => x.teExcelSrNo == i.sysTEExcel.teExcelSrNo).SingleOrDefault<Sys_TEExcel>();
                    List<string> ExcelData = new List<string>();
                    status = GetExcelForm.GetTEExcelForm(sysTEExcel.teExcelType, out ExcelData);
                    if (!status)
                    {
                        return false;
                    }
                    #endregion

                    #region 插入Panel
                    object[] o = new object[] { false, sysTEExcel.teExcelType, i.ncGroupName, ""
                                                , string.Format("{0}_{1}_OP{2}資料夾"
                                                , partNo
                                                , cusVer
                                                , op1) };
                    TEPanel.Rows.Add(new GridRow(o));
                    TEPanel.GetCell(TECount, 0).Value = false;
                    TEPanel.GetCell(TECount, 3).EditorType = typeof(GridComboBoxExEditControl);
                    GridComboBoxExEditControl singleCell = TEPanel.GetCell(TECount, 3).EditControl as GridComboBoxExEditControl;
                    singleCell.Items.Add("");
                    foreach (string tempStr in ExcelData)
                    {
                        singleCell.Items.Add(tempStr);
                    }
                    TEPanel.GetCell(TECount, 3).Value = "(雙擊)選擇表單";
                    #endregion

                    TECount++;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool GetDimensionDataFromPanel(ComboBoxEx Op1Combobox, out Dictionary<DB_MEMain, IList<Com_Dimension>> DicDimensionData)
        {
            DicDimensionData = new Dictionary<DB_MEMain, IList<Com_Dimension>>();
            try
            {
                for (int i = 0; i < OutputForm.MEPanel.Rows.Count; i++)
                {
                    if (((bool)OutputForm.MEPanel.GetCell(i, 0).Value) == false)
                    {
                        continue;
                    }
                    DB_MEMain sDB_MEMain = new DB_MEMain();
                    Sys_MEExcel meExcelSrNo = session.QueryOver<Sys_MEExcel>()
                                              .Where(x => x.meExcelType == OutputForm.MEPanel.GetCell(i, 1).Value.ToString())
                                              .SingleOrDefault<Sys_MEExcel>();
                    if (meExcelSrNo == null)
                    {
                        continue;
                    }
                    Com_MEMain comMEMain = session.QueryOver<Com_MEMain>()
                                           .Where(x => x.comPartOperation == (Com_PartOperation)Op1Combobox.SelectedItem)
                                           .Where(x => x.sysMEExcel == meExcelSrNo)
                                           .SingleOrDefault<Com_MEMain>();
                    IList<Com_Dimension> comDimension = session.QueryOver<Com_Dimension>()
                                                        .Where(x => x.comMEMain == comMEMain)
                                                        .List<Com_Dimension>();
                    sDB_MEMain.comMEMain = comMEMain;
                    sDB_MEMain.excelTemplateFilePath = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}\{6}.xls"
                                                                        , "\\\\192.168.31.55"
                                                                        , "cax"
                                                                        , "Globaltek"
                                                                        , "ME_Config"
                                                                        , "Config"
                                                                        , OutputForm.MEPanel.GetCell(i, 1).Value.ToString()
                                                                        , OutputForm.MEPanel.GetCell(i, 2).Value.ToString());
                    sDB_MEMain.factory = OutputForm.MEPanel.GetCell(i, 2).Value.ToString();
                    DicDimensionData.Add(sDB_MEMain, comDimension);
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool GetShopDocDataFromPanel(ComboBoxEx Op1Combobox, out Dictionary<DB_TEMain, IList<Com_ShopDoc>> DicShopDocData)
        {
            DicShopDocData = new Dictionary<DB_TEMain, IList<Com_ShopDoc>>();
            try
            {
                for (int i = 0; i < OutputForm.TEPanel.Rows.Count; i++)
                {
                    if (((bool)OutputForm.TEPanel.GetCell(i, 0).Value) == false)
                    {
                        continue;
                    }
                    
                    Sys_TEExcel teExcelSrNo = session.QueryOver<Sys_TEExcel>()
                                              .Where(x => x.teExcelType == OutputForm.TEPanel.GetCell(i, 1).Value.ToString())
                                              .SingleOrDefault<Sys_TEExcel>();
                    if (teExcelSrNo == null)
                    {
                        continue;
                    }
                    IList<Com_TEMain> comTEMain = session.QueryOver<Com_TEMain>()
                                                  .Where(x => x.comPartOperation == (Com_PartOperation)Op1Combobox.SelectedItem)
                                                  .Where(x => x.sysTEExcel == teExcelSrNo)
                                                  .List<Com_TEMain>();
                    foreach (Com_TEMain ii in comTEMain)
                    {
                        DB_TEMain sDB_TEMain = new DB_TEMain();
                        IList<Com_ShopDoc> comDimension = session.QueryOver<Com_ShopDoc>()
                                                          .Where(x => x.comTEMain == ii)
                                                          .List<Com_ShopDoc>();
                        sDB_TEMain.comTEMain = ii;
                        sDB_TEMain.excelTemplateFilePath = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}\{6}.xls"
                                                                            , "\\\\192.168.31.55"
                                                                            , "cax"
                                                                            , "Globaltek"
                                                                            , "TE_Config"
                                                                            , "Config"
                                                                            , OutputForm.TEPanel.GetCell(i, 1).Value.ToString()
                                                                            , OutputForm.TEPanel.GetCell(i, 3).Value.ToString());
                        sDB_TEMain.ncGroupName = ii.ncGroupName;
                        sDB_TEMain.factory = OutputForm.TEPanel.GetCell(i, 3).Value.ToString();
                        DicShopDocData.Add(sDB_TEMain, comDimension);
                    }
                    
                    //sDB_TEMain.comTEMain = comTEMain;
                    //sDB_TEMain.excelTemplateFilePath = string.Format(@"{0}\{1}.xls", OutputForm.serverTEConfig, OutputForm.TEPanel.GetCell(i, 3).Value.ToString());
                    //sDB_TEMain.factory = OutputForm.TEPanel.GetCell(i, 3).Value.ToString();
                    //DicShopDocData.Add(sDB_TEMain, comDimension);
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
