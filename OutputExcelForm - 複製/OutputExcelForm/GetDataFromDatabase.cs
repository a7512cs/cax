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
    public class GetDataFromDatabase
    {
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
        public static bool SetMEPanel(Com_PartOperation comPartOperation, ref SuperGridControl SGCPanel)
        {
            try
            {
                IList<Com_MEMain> comMEMain = session.QueryOver<Com_MEMain>().Where(x => x.comPartOperation == comPartOperation).List<Com_MEMain>();
                
                foreach (Com_MEMain i in comMEMain)
                {
                    //由meExcelSrNo取得對應的ExcelType
                    Sys_MEExcel sysMEExcel = session.QueryOver<Sys_MEExcel>().Where(x => x.meExcelSrNo == i.sysMEExcel.meExcelSrNo).SingleOrDefault<Sys_MEExcel>();
                    
                    GetExcelForm.GetMEExcelForm(sysMEExcel.excelType);
                    GridRow singleRow = new GridRow(false, sysMEExcel.excelType, "", "");
                    SGCPanel.PrimaryGrid.Rows.Add(singleRow);
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
