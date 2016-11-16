using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevComponents.DotNetBar.Controls;
using CaxGlobaltek;
using NHibernate;

namespace DeleteDataBase
{
    public class GetData
    {
        public static bool status;
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
    }
}
