using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CaxGlobaltek;
using NHibernate;
using System.Collections;

namespace DeleteDataBase
{
    public partial class Form1 : Form
    {
        bool status;
        public static ISession session = MyHibernateHelper.SessionFactory.OpenSession();
        public static ITransaction tx = session.BeginTransaction();
        public Form1()
        {
            InitializeComponent();
            InitializeHideControlItems();
            GetCustomerFromDatabase();
        }

        private void InitializeHideControlItems()
        {
            PartNumCombobox.Enabled = false;
            CusVerCombobox.Enabled = false;
        }

        private void GetCustomerFromDatabase()
        {
            status = GetData.SetCustomerData(CusCombobox);
            if (!status)
            {
                MessageBox.Show("客戶資料取得失敗，請聯繫開發工程師");
                this.Close();
            }
        }

        private void CusCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PartNumCombobox.Enabled = true;
            CusVerCombobox.Enabled = false;

            PartNumCombobox.Items.Clear();
            CusVerCombobox.Items.Clear();

            PartNumCombobox.Text = "";
            CusVerCombobox.Text = "";

            status = GetData.SetPartNoData(((Sys_Customer)CusCombobox.SelectedItem), PartNumCombobox);
            if (!status)
            {
                MessageBox.Show("料號資料取得失敗，請聯繫開發工程師");
                this.Close();
            }
        }

        private void PartNumCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CusVerCombobox.Enabled = true;
            //Op1Combobox.Enabled = false;

            CusVerCombobox.Items.Clear();
            //Op1Combobox.Items.Clear();

            CusVerCombobox.Text = "";
            //Op1Combobox.Text = "";

            status = GetData.SetCusVerData(PartNumCombobox.Text, CusVerCombobox);
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

        private void buttonX1_Click(object sender, EventArgs e)
        {
            try
            {
                ISession session = MyHibernateHelper.SessionFactory.OpenSession();
                ITransaction tx = session.BeginTransaction();

                Com_PEMain comPEMain = new Com_PEMain();
                #region 由料號&版次查peSrNo
                comPEMain = session.QueryOver<Com_PEMain>().Where(x => x.partName == PartNumCombobox.Text)
                                                           .Where(x => x.customerVer == CusVerCombobox.Text)
                                                           .SingleOrDefault<Com_PEMain>();
                #endregion

                session.Delete(comPEMain);
                tx.Commit();
                MessageBox.Show("刪除成功");
                CusCombobox_SelectedIndexChanged(sender, e);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
            
            
            //tx.Commit();
            /*
            IList<Com_PartOperation> ComPartOperation = new List<Com_PartOperation>();
            #region 由peSrNo取得所有的partOperation
            ComPartOperation = session.QueryOver<Com_PartOperation>().Where(x => x.comPEMain == comPEMain)
                                                                     .List<Com_PartOperation>();
            //MessageBox.Show(ComPartOperation.Count.ToString());
            #endregion

            IList<Com_MEMain> comMEMain = new List<Com_MEMain>();
            IList<Com_MEMain> ListcomMEMain = new List<Com_MEMain>();
            #region 由ComPartOperation取得所有的MEMain
            foreach (Com_PartOperation i in ComPartOperation)
            {
                comMEMain = session.QueryOver<Com_MEMain>().Where(x => x.comPartOperation.partOperationSrNo == i.partOperationSrNo)
                                                               .List<Com_MEMain>();
                foreach (Com_MEMain ii in comMEMain)
                {
                    ListcomMEMain.Add(ii);
                }
            }
            #endregion
            */

        }
    }
}
