using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.AdvTree;
using NXOpen.UF;
using NXOpen;
using System.IO;
using DevComponents.DotNetBar;
using CaxGlobaltek;
using NXOpen.Utilities;

namespace BomList
{
    public partial class BomListDlg : Form
    {
        private static bool status;
        public static Session theSession = Session.GetSession();
        public static UI theUI;
        public static UFSession theUfSession = UFSession.GetUFSession();
        public static Part workPart = theSession.Parts.Work;
        public static Part displayPart = theSession.Parts.Display;
        private ElementStyle _RightAlignFileSizeStyle = null;
        public BomListDlg()
        {
            InitializeComponent();
        }

        private void BomListDlg_Load(object sender, EventArgs e)
        {
            advTree1.BeginUpdate();
            Node node = new Node();
            node.Tag = (NXOpen.Assemblies.Component)displayPart.ComponentAssembly.RootComponent;
            node.Text = Path.GetFileNameWithoutExtension(displayPart.FullPath);
            //node.Cells.Add(new Cell("Local Disk"));
            advTree1.Nodes.Add(node);
            node.ExpandVisibility = eNodeExpandVisibility.Visible;
            advTree1.EndUpdate();
            _RightAlignFileSizeStyle = new ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;
        }

        private void advTree1_BeforeExpand(object sender, AdvTreeNodeCancelEventArgs e)
        {
            Node parent = e.Node;
            if (parent.Nodes.Count > 0) return;

            NXOpen.Assemblies.Component clickComp = (NXOpen.Assemblies.Component)parent.Tag;
            List<NXOpen.Assemblies.Component> compAry = new List<NXOpen.Assemblies.Component>();
            status = CaxAsm.GetCompChildren(clickComp, ref compAry);
            if (!status)
            {
                MessageBox.Show("尋找子Component失敗，請聯繫開發工程師");
                this.Close();
            }

            status = InsertSubComponent(parent, compAry);
            if (!status)
            {
                MessageBox.Show("插入子Component失敗，請聯繫開發工程師");
                this.Close();
            }
        }
        private bool InsertSubComponent(Node parent, List<NXOpen.Assemblies.Component> compAry)
        {
            try
            {
                foreach (NXOpen.Assemblies.Component i in compAry)
                {
                    Node node = new Node();
                    node.Tag = (NXOpen.Assemblies.Component)NXObjectManager.Get(i.Tag);
                    node.Text = Path.GetFileNameWithoutExtension(i.DisplayName);
                    node.ExpandVisibility = eNodeExpandVisibility.Visible;
                    parent.Nodes.Add(node);
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
