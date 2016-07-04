using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CaxUGforEspritt;

using WeData;
using DevComponents.DotNetBar.SuperGrid;
using CimforceCaxTwPublic;
using DevComponents;
using NXOpen;

namespace CaxUGforEspritt
{
    public partial class MultiTolDlg : DevComponents.DotNetBar.Office2007Form
    {
        public Dictionary<string, TolValue> TolColor_Top_Low = new Dictionary<string, TolValue>();
        private bool IS_PROGRAM = false;
        
 
        public MultiTolDlg(Dictionary<string, TolValue> TolColor_Top_Low)
        {
            this.TolColor_Top_Low = TolColor_Top_Low;
            InitializeComponent();
            superGridControlMultiTol.CellValueChanged += superGridControlMultiTol_CellValueChanged;
        }


        private void MultiTolDlg_Load(object sender, EventArgs e)
        {
            //設定對話框風格
            StyleController styleControl = new StyleController();
            styleControl.SetStyleManager(styleManager1);
            styleControl.SetAllStyle(this);

            GridPanel panel = superGridControlMultiTol.PrimaryGrid;
            GridRow singleGrid = new GridRow();
            foreach (KeyValuePair<string, TolValue> kvp in TolColor_Top_Low)
            {
                singleGrid = new GridRow(false, kvp.Key, kvp.Value.Tol_Upper, kvp.Value.Tol_Lower, kvp.Value.Tol_Region);
                panel.Rows.Add(singleGrid);
            }
        }

        void superGridControlMultiTol_CellValueChanged(object sender, GridCellValueChangedEventArgs e)
        {
            GridCell cell = e.GridCell;

            if (cell.GridColumn.Name.Equals("選擇") == true && !IS_PROGRAM)
            {
                IS_PROGRAM = true;

                GridRow row = cell.GridRow;

                for (int i = 0; i < superGridControlMultiTol.PrimaryGrid.Rows.Count; i++)
                {
                    if (i == row.Index)
                    {
                        continue;
                    }
                    bool check = (bool)superGridControlMultiTol.PrimaryGrid.GridPanel.GetCell(i, 0).Value;
                    if (check)
                    {
                        superGridControlMultiTol.PrimaryGrid.GridPanel.GetCell(i, 0).Value = (bool)false;
                    }
                }

                bool cur_check = (bool)superGridControlMultiTol.PrimaryGrid.GridPanel.GetCell(row.Index, 0).Value;
                if (!cur_check)
                {
                    superGridControlMultiTol.PrimaryGrid.GridPanel.GetCell(row.Index, 0).Value = (bool)false;
                }
                else
                {
                    superGridControlMultiTol.PrimaryGrid.GridPanel.GetCell(row.Index, 0).Value = (bool)true;
                }
                //CaxLog.ShowListingWindow(row.Index.ToString());

                IS_PROGRAM = false;
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            
            bool check_sel = false;
            for (int i = 0; i < superGridControlMultiTol.PrimaryGrid.Rows.Count; i++)
            {
                check_sel = (bool)superGridControlMultiTol.PrimaryGrid.GridPanel.GetCell(i, 0).Value;
                if (check_sel)
                {
                    MultiTol.Tol_Region = superGridControlMultiTol.PrimaryGrid.GridPanel.GetCell(i, 4).Value.ToString();
                    MultiTol.Tol_Upper = superGridControlMultiTol.PrimaryGrid.GridPanel.GetCell(i, 2).Value.ToString();
                    MultiTol.Tol_Lower = superGridControlMultiTol.PrimaryGrid.GridPanel.GetCell(i, 3).Value.ToString();
                    break;
                }
            }
            if (!check_sel)
            {
                this.Visible = false;
                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "請選擇一組公差，決定加工刀次");
                this.Visible = true;
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonX2_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void MultiTolDlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        private void MultiTolDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            
        }




    }
}
