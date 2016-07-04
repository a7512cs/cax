using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CaxUGforEspritt;//2014-1021

using DevComponents.DotNetBar;
using DevComponents.DotNetBar.SuperGrid;
using DevComponents.DotNetBar.SuperGrid.Style;

using NXOpen;
using System.Collections;
using NXOpen.UF;
using NXOpen.Utilities;
using CimforceCaxTwMFG;

namespace CaxUGforEspritt
{
    #region 2014-1021
    public partial class mainControl : DevComponents.DotNetBar.Office2007Form
    {
        public DataTable WEDataTable;
        public mainControl(EACHGROUPARRAY WEPoint)
        {
            InitializeComponent();
            //createFakeDatas(ref WEPoint); //Unnote this line if want to create fake datas to test
            WEDataTable = new DataTable();
            loadDataTable(WEPoint, ref WEDataTable);
            supGridMain.PrimaryGrid.DataSource = WEDataTable;
            supGridMain.PrimaryGrid.ShowRowHeaders = false;
            supGridMain.PrimaryGrid.ColumnHeader.RowHeight = 30;
            string[] orderArray = { "選擇加工類型", "垂直方孔", "垂直外形", "垂直圓孔", "斜銷孔", "錐度孔", "錐度外形", "開放式外形","開放式錐度外形" };
            foreach (DataColumn singleCol in WEDataTable.Columns)
            {
                GridColumn supGridColInput = new GridColumn();
                supGridColInput.Name = singleCol.ColumnName.ToString();
                switch (singleCol.ColumnName.ToString())
                {
                    case "選擇點":
                        supGridColInput.EditorType = typeof(FragrantButtonX);
                        supGridColInput.EditorParams = new object[] { };
                        supGridColInput.Width = 50;
                        break;
                    case "狀態":
                        supGridColInput.EditorType = typeof(FragrantCheckBoxX);
                        supGridColInput.Width = 50;
                        supGridColInput.AllowEdit = false;
                        break;
                    case "逃孔":
                        //supGridColInput.EditorType = typeof(FragrantCheckBoxX);
                        supGridColInput.Width = 50;
                        break;
                    case "加工類型":
                        supGridColInput.EditorType = typeof(FragrantComboBox);
                        supGridColInput.EditorParams = new object[] { orderArray };
                        supGridColInput.Width = 80;
                        break;
                    case "X":
                        supGridColInput.Width = 60;
                        break;
                    case "Y":
                        supGridColInput.Width = 60;
                        break;
                    case "加工角度":
                        supGridColInput.Width = 60;
                        break;
                    default:
                        supGridColInput.Width = 60;
                        supGridColInput.AllowEdit = false;
                        break;
                }
                supGridColInput.MarkRowDirtyOnCellValueChange = false;
                supGridColInput.ColumnSortMode = ColumnSortMode.None;
                supGridMain.PrimaryGrid.Columns.Add(supGridColInput);
            }
            foreach (DataRow singleRow in WEDataTable.Rows)
            {
                GridRow supGridRowInput = new GridRow();
                supGridMain.TabIndex = 1;
                supGridMain.PrimaryGrid.Rows.Add(supGridRowInput);
            }
        }

        private GridButtonXEditControl btnChoose = new GridButtonXEditControl();
        private GridCheckBoxEditControl chkBoxAlert = new GridCheckBoxEditControl();
        //private GridCheckBoxEditControl chkBoxSHCS = new GridCheckBoxEditControl();
        private GridComboBoxExEditControl cmbBoxTyp = new GridComboBoxExEditControl();

        //         private bool createFakeDatas(ref WEData fakeData)
        //         {
        //             fakeData = new WEData();
        //             fakeData.EACHGROUPARRAY = new List<EACHGROUPARRAY>();
        //             Random rnd = new Random();
        //             int totalNums = rnd.Next(5, 220);
        //             for (int i = 0; i < totalNums; i++)
        //             {
        //                 EACHGROUPARRAY temporary = new EACHGROUPARRAY();
        //                 temporary.MACHING_TYPE = null;//rnd.Next(1,7).ToString();
        //                 temporary.IS_SHCS = rnd.Next(0, 1).ToString();
        //                 temporary.MACHING_ANGLE = "0";
        //                 temporary.MACHING_THICKNESS = Convert.ToString((Convert.ToDouble(rnd.Next(10, 40)) + rnd.NextDouble())).Substring(0, 6);
        //                 temporary.MACHING_COUNT = rnd.Next(1, 3).ToString();
        //                 temporary.UPPER_TOLERANCE = (rnd.NextDouble()).ToString().Substring(0, 6);
        //                 temporary.LOWER_TOLERANCE = (rnd.NextDouble()).ToString().Substring(0, 6);
        //                 temporary.WORKHEIGHT_XY = Convert.ToString((Convert.ToDouble(rnd.Next(10, 40)) + rnd.NextDouble())).Substring(0, 6);
        //                 temporary.WORKHEIGHT_UV = Convert.ToString((Convert.ToDouble(rnd.Next(10, 40)) + rnd.NextDouble())).Substring(0, 6);
        //                 temporary.MOVEPOSI_X = "0";
        //                 temporary.MOVEPOSI_Y = "0";
        //                 temporary.COLOR_ID = "";
        //                 temporary.FACEGROUPMEMBERS = new List<NXOpen.Tag>();
        //                 fakeData.EACHGROUPARRAY.Add(temporary);
        //             }
        //             return true;
        //         }

        private bool loadDataTable(EACHGROUPARRAY WEInput, ref DataTable inputDataTable)
        {
            inputDataTable.Columns.Add("狀態");
            inputDataTable.Columns.Add("選擇點");
            inputDataTable.Columns.Add("X");
            inputDataTable.Columns.Add("Y");
            inputDataTable.Columns.Add("加工類型");
            inputDataTable.Columns.Add("逃孔");
            inputDataTable.Columns.Add("加工角度");
            inputDataTable.Columns.Add("加工厚度");
            inputDataTable.Columns.Add("加工刀次");
            inputDataTable.Columns.Add("上公差");
            inputDataTable.Columns.Add("下公差");
            inputDataTable.Columns.Add("程式面XY");
            inputDataTable.Columns.Add("程式面UV");
            inputDataTable.Columns.Add("面群");
            inputDataTable.Columns["面群"].DataType = typeof(List<NXOpen.Tag>);

            //             foreach (EACHGROUPARRAY item in WEInput.EACHGROUPARRAY)
            //             {
            //                 inputDataTable.Rows.Add();
            //                 int currentNum = inputDataTable.Rows.Count - 1;
            //                 inputDataTable.Rows[currentNum][0] = "False";
            //                 inputDataTable.Rows[currentNum][1] = "...";
            //                 inputDataTable.Rows[currentNum][2] = null;
            //                 inputDataTable.Rows[currentNum][3] = null;
            //                 inputDataTable.Rows[currentNum][4] = "選擇加工類型";
            //                 inputDataTable.Rows[currentNum][5] = item.IS_SHCS;
            //                 inputDataTable.Rows[currentNum][6] = item.MACHING_ANGLE;
            //                 inputDataTable.Rows[currentNum][7] = item.MACHING_THICKNESS;
            //                 inputDataTable.Rows[currentNum][8] = item.MACHING_COUNT;
            //                 inputDataTable.Rows[currentNum][9] = item.UPPER_TOLERANCE;
            //                 inputDataTable.Rows[currentNum][10] = item.LOWER_TOLERANCE;
            //                 inputDataTable.Rows[currentNum][11] = item.WORKHEIGHT_XY;
            //                 inputDataTable.Rows[currentNum][12] = item.WORKHEIGHT_UV;
            //                 inputDataTable.Rows[currentNum][13] = item.FACEGROUPMEMBERS;
            //             }

            foreach (PROGRAMARRAY item in WEInput.PROGRAM_ARRAY)
            {
                inputDataTable.Rows.Add();
                int currentNum = inputDataTable.Rows.Count - 1;
                inputDataTable.Rows[currentNum][0] = "False";
                inputDataTable.Rows[currentNum][1] = "...";
                inputDataTable.Rows[currentNum][2] = null;
                inputDataTable.Rows[currentNum][3] = null;
                inputDataTable.Rows[currentNum][4] = "選擇加工類型";
                inputDataTable.Rows[currentNum][5] = item.IS_SHCS;
                inputDataTable.Rows[currentNum][6] = item.MACHING_ANGLE;
                inputDataTable.Rows[currentNum][7] = item.MACHING_THICKNESS;
                inputDataTable.Rows[currentNum][8] = item.MACHING_COUNT;
                inputDataTable.Rows[currentNum][9] = item.UPPER_TOLERANCE;
                inputDataTable.Rows[currentNum][10] = item.LOWER_TOLERANCE;
                inputDataTable.Rows[currentNum][11] = item.WORKHEIGHT_XY;
                inputDataTable.Rows[currentNum][12] = item.WORKHEIGHT_UV;
                inputDataTable.Rows[currentNum][13] = item.FACEGROUPMEMBERS;
            }

            return true;
        }
        private void mainControl_Load(object sender, EventArgs e)
        {

        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            bool chkIfItsDone = true;
            foreach (GridRow singleRow in supGridMain.PrimaryGrid.Rows)
            {
                if (singleRow.Cells["狀態"].Value.ToString() == "False")
                    chkIfItsDone = false;
            }
            if (chkIfItsDone)
                this.Close();
            else
            {
                DialogResult result = MessageBox.Show("資料不完整！", "警告",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
    #endregion
    internal class FragrantComboBox : GridComboBoxExEditControl
    {
        public FragrantComboBox(IEnumerable orderArray)
        {
            DataSource = orderArray;
            SelectedIndexChanged += FragrantComboBoxClick;
        }
        public void FragrantComboBoxClick(object sender, EventArgs e)
        {
            GridCell tempCell = EditorCell;
            tempCell.Value = this.SelectedValue;
        }
    }


    internal class FragrantButtonX : GridButtonXEditControl
    {
        private static UFSession theUfSession_;
        public FragrantButtonX()
        {
            Click += FragrantButtonXClick;
        }
        public void FragrantButtonXClick(object sender, EventArgs e)
        {
            theUfSession_ = UFSession.GetUFSession();
            Session theSession_ = Session.GetSession();
            Part workPart = theSession_.Parts.Work;
            Part displayPart = theSession_.Parts.Display;
            //mainControl a = (mainControl)sender;
            //a.Close();
            string title = "Select point";
            int response = 0;
            //mainControl c = (mainControl)a;
            //c.Close();
            GridRow tempGridRow = (GridRow)this.EditorCell.Parent;
            List<Tag> FaceTag = new List<Tag>();
            FaceTag = (List<Tag>)tempGridRow.Cells["面群"].Value;
            foreach (Tag a in FaceTag)
            {
                Face b = (Face)NXObjectManager.Get(a);
                //Face b_proto = (Face)b.Prototype;
                b.Highlight();
            }
            Tag point_tag = NXOpen.Tag.Null;
            UFUi.PointBaseMethod base_method = UFUi.PointBaseMethod.PointInferred;
            double[] base_pt = new double[3];
            //theUfSession_.Ui.LockUgAccess(NXOpen.UF.UFConstants.UF_UI_FROM_CUSTOM);
            workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Top, NXOpen.View.ScaleAdjustment.Fit);
            theUfSession_.Ui.PointConstruct(title, ref base_method, out point_tag, base_pt, out response);
            //theUfSession_.Ui.UnlockUgAccess(NXOpen.UF.UFConstants.UF_UI_FROM_CUSTOM);
            tempGridRow.Cells["X"].Value = Math.Round(base_pt[0],4).ToString();
            tempGridRow.Cells["Y"].Value = Math.Round(base_pt[1],4).ToString();
            tempGridRow.Cells["狀態"].Value = true;
            foreach (Tag a in FaceTag)
            {
                Face b = (Face)NXObjectManager.Get(a);
                //Face b_proto = (Face)b.Prototype;
                b.Unhighlight();
            }
        }
    }

    internal class FragrantCheckBoxX : GridCheckBoxEditControl
    {
        public FragrantCheckBoxX()
        {

        }
    }
}
