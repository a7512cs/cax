using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using CaxGlobaltek;
using NHibernate;

namespace AddDeleteDB
{
    public partial class Form1 : Form
    {
        private bool status;
        private static ISession session = MyHibernateHelper.SessionFactory.OpenSession();
        private List<string> listCustomerName = new List<string>();
        private List<string> listOperation2 = new List<string>();
        private List<string> listSelItems = new List<string>();
        private SuperTabStripSelectedTabChangedEventArgs CurrentE;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SelNum.Text = "0";

            #region 新增製程別
            SuperTabItem newOne = new DevComponents.DotNetBar.SuperTabItem();
            newOne.Text = "製程別";
            //newOne.Image = this.imageList1.Images[6];
            newOne.Image = Properties.Resources.OIS_48px;
            newOne.AttachedControl = STC_Panel;
            newOne.ImagePadding = superTabItem1.ImagePadding;
            newOne.ImageAlignment = superTabItem1.ImageAlignment;
            newOne.FixedTabSize = superTabItem1.FixedTabSize;
            newOne.TextAlignment = superTabItem1.TextAlignment;

            TabControl.Tabs.Add(newOne);
            #endregion

            #region 新增材質
            newOne = new DevComponents.DotNetBar.SuperTabItem();
            newOne.Text = "材質";
            //newOne.Image = this.imageList1.Images[6];
            newOne.Image = Properties.Resources.material_48px;
            newOne.AttachedControl = STC_Panel;
            newOne.ImagePadding = superTabItem1.ImagePadding;
            newOne.ImageAlignment = superTabItem1.ImageAlignment;
            newOne.FixedTabSize = superTabItem1.FixedTabSize;
            newOne.TextAlignment = superTabItem1.TextAlignment;

            TabControl.Tabs.Add(newOne);
            #endregion

            if (TabControl.SelectedTab.Text == "客戶")
            {
                listCustomerName = new List<string>();
                status = Customer.SetCustomerData(out listCustomerName);
                if (!status)
                {
                    MessageBox.Show("取得目前客戶名稱資料失敗");
                    return;
                }
                for (int i = 0; i < listCustomerName.Count; i++)
                {
                    ListViewItem tempViewItem = new ListViewItem(listCustomerName[i]);
                    listView1.Items.Add(tempViewItem);
                }
            }
            
        }

        private void TabControl_SelectedTabChanged(object sender, SuperTabStripSelectedTabChangedEventArgs e)
        {
            try
            {
                listView1.Items.Clear();
                AddText.Text = "";
                if (TabControl.SelectedTab.Text == "客戶")
                {
                    listCustomerName = new List<string>();
                    status = Customer.SetCustomerData(out listCustomerName);
                    if (!status)
                    {
                        MessageBox.Show("取得目前客戶名稱資料失敗");
                        return;
                    }
                    for (int i = 0; i < listCustomerName.Count; i++)
                    {
                        ListViewItem tempViewItem = new ListViewItem(listCustomerName[i]);
                        listView1.Items.Add(tempViewItem);
                    }
                }
                else if (TabControl.SelectedTab.Text == "製程別")
                {
                    listOperation2 = new List<string>();
                    status = Operation2.SetOperation2Data(out listOperation2);
                    if (!status)
                    {
                        MessageBox.Show("取得目前製程別資料失敗");
                        return;
                    }
                    for (int i = 0; i < listOperation2.Count; i++)
                    {
                        ListViewItem tempViewItem = new ListViewItem(listOperation2[i]);
                        listView1.Items.Add(tempViewItem);
                    }
                }
                else if (TabControl.SelectedTab.Text == "材質")
                {
                }
                CurrentE = e;
            }
            catch (System.Exception ex)
            {
            	
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (TabControl.SelectedTab.Text == "客戶")
                {
                    status = CommonFun.CheckData(AddText.Text, listCustomerName);
                    if (!status)
                    {
                        AddText.Text = "";
                        return;
                    }

                    Sys_Customer sysCustomer = new Sys_Customer();
                    sysCustomer.customerName = AddText.Text;
                    session.Save(sysCustomer);
                    ITransaction trans = session.BeginTransaction();
                    trans.Commit();
                    
                    TabControl_SelectedTabChanged(sender, CurrentE);
                }
                else if (TabControl.SelectedTab.Text == "製程別")
                {
                    status = CommonFun.CheckData(AddText.Text, listOperation2);
                    if (!status)
                    {
                        AddText.Text = "";
                        return;
                    }

                    Sys_Operation2 sysOperation2 = new Sys_Operation2();
                    sysOperation2.operation2Name = AddText.Text;
                    session.Save(sysOperation2);
                    ITransaction trans = session.BeginTransaction();
                    trans.Commit();

                    TabControl_SelectedTabChanged(sender, CurrentE);
                }
                else if (TabControl.SelectedTab.Text == "材質")
                {
                }
                AddText.Text = "";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            int selCount = 0;
            listSelItems = new List<string>();
            foreach (ListViewItem tmpLstView in listView1.Items)
            {
                if (tmpLstView.Selected == true)
                {
                    selCount++;
                    listSelItems.Add(tmpLstView.Text);
                }
            }
            SelNum.Text = selCount.ToString();
        }

        private void DelButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (TabControl.SelectedTab.Text == "客戶")
                {
                    IList<Sys_Customer> sysCustomer = session.QueryOver<Sys_Customer>().List<Sys_Customer>();
                    IList<Com_PEMain> comPEMain = session.QueryOver<Com_PEMain>().List<Com_PEMain>();

                    List<string> DelSelData = new List<string>();
                    foreach (string i in listSelItems)
                    {
                        foreach (Sys_Customer ii in sysCustomer)
                        {
                            if (i != ii.customerName)
                            {
                                continue;
                            }
                            foreach (Com_PEMain y in comPEMain)
                            {
                                if (y.sysCustomer == ii)
                                {
                                    DelSelData.Add(i);
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    if (DelSelData.Count > 0)
                    {
                        string cantDelStr = "";
                        for (int i = 0; i < DelSelData.Count;i++ )
                        {
                            if (i == 0)
                            {
                                cantDelStr = DelSelData[i];
                            }
                            else
                            {
                                cantDelStr = cantDelStr + "、" + DelSelData[i];
                            }
                        }
                        MessageBox.Show("選項：" + cantDelStr + "已使用中，無法刪除，請重新操作！");
                        SelNum.Text = "0";
                        listSelItems = new List<string>();
                        return;
                    }

                    foreach (string i in listSelItems)
                    {
                        foreach (Sys_Customer ii in sysCustomer)
                        {
                            if (i == ii.customerName)
                            {
                                session.Delete(ii);
                                break;
                            }
                        }
                    }
                    ITransaction trans = session.BeginTransaction();
                    trans.Commit();

                    TabControl_SelectedTabChanged(sender, CurrentE);
                }
                else if (TabControl.SelectedTab.Text == "製程別")
                {
                    IList<Sys_Operation2> sysOperation2 = session.QueryOver<Sys_Operation2>().List<Sys_Operation2>();
                    IList<Com_PartOperation> comPartOperation = session.QueryOver<Com_PartOperation>().List<Com_PartOperation>();

                    List<string> DelSelData = new List<string>();
                    foreach (string i in listSelItems)
                    {
                        foreach (Sys_Operation2 ii in sysOperation2)
                        {
                            if (i != ii.operation2Name)
                            {
                                continue;
                            }
                            foreach (Com_PartOperation y in comPartOperation)
                            {
                                if (y.sysOperation2 == ii)
                                {
                                    DelSelData.Add(i);
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    if (DelSelData.Count > 0)
                    {
                        string cantDelStr = "";
                        for (int i = 0; i < DelSelData.Count; i++)
                        {
                            if (i == 0)
                            {
                                cantDelStr = DelSelData[i];
                            }
                            else
                            {
                                cantDelStr = cantDelStr + "、" + DelSelData[i];
                            }
                        }
                        MessageBox.Show("選項：" + cantDelStr + "已使用中，無法刪除，請重新操作！");
                        SelNum.Text = "0";
                        listSelItems = new List<string>();
                        return;
                    }

                    foreach (string i in listSelItems)
                    {
                        foreach (Sys_Operation2 ii in sysOperation2)
                        {
                            if (i == ii.operation2Name)
                            {
                                session.Delete(ii);
                                break;
                            }
                        }
                    }
                    ITransaction trans = session.BeginTransaction();
                    trans.Commit();

                    TabControl_SelectedTabChanged(sender, CurrentE);
                }
                else if (TabControl.SelectedTab.Text == "材質")
                {
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
