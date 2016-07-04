using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.SuperGrid;
using CimforceCaxTwPublic;
using CimforceCaxTwMD;
using CimforceCaxTwMFG;
using NXOpen;
using NXOpen.Utilities;
using NXOpen.UF;
using NXOpen.Features;
using System.Collections;
using WeData;
using WE_Get_Thrugh_Pnt;
using CSUGFunc;
using DevComponents.DotNetBar;

namespace CaxUGforEspritt
{
    public partial class CreateWEPoint : DevComponents.DotNetBar.Office2007Form
    {
        public static WeGroupFacePnt sWeGroupFacePntInput;
        public static List<NXObject> ListnXObject1 = new List<NXObject>(); //紀錄座標點建出來的球體
        public static NXObject nXObject1;
        public static List<FaceGroupPnt> weFaceGroupAllList = new List<FaceGroupPnt>();
        public static List<Face> ListHighlightFace = new List<Face>();
        public static string CurrentPartStatus = "";

        //         public static int Form1Index;
        //         public static int Form2Index;


        public CreateWEPoint(WeGroupFacePnt sWeGroupFacePnt, string InputCurrentPartStatus)
        {
            sWeGroupFacePntInput = sWeGroupFacePnt;
            CurrentPartStatus = InputCurrentPartStatus;
            InitializeComponent();
            InitializeGrid();
            //Form1Index = 0;
        }

        #region FragrantComboBox

        public class FragrantComboBox : GridComboBoxExEditControl
        {

            public FragrantComboBox(IEnumerable orderArray)
            {
                DataSource = orderArray;
                DropDownStyle = ComboBoxStyle.DropDownList;
                this.DropDownClosed += new EventHandler(GridWeTypeChanged);
            }

            public void GridWeTypeChanged(object sender, EventArgs e)
            {
                try
                {
                    FragrantComboBox thisSender = (FragrantComboBox)sender;
                    int CurrentRow = thisSender.EditorCell.RowIndex;
                    string CurrentMachiningType = thisSender.SelectedItem.ToString();
                    GridRow tempGridRow = (GridRow)this.EditorCell.Parent;
                    if (CurrentMachiningType == "垂直方孔" || CurrentMachiningType == "垂直圓孔" || CurrentMachiningType == "錐度孔")
                    {
                        tempGridRow.Cells["X"].Value = "ESPRIT_AUTO";
                        tempGridRow.Cells["Y"].Value = "ESPRIT_AUTO";
                        tempGridRow.Cells["ListWEArea"].Value = "已佈點";
                    }
                    else 
                    {
                        tempGridRow.Cells["X"].Value = "NaN";
                        tempGridRow.Cells["Y"].Value = "NaN";
                        tempGridRow.Cells["ListWEArea"].Value = "開始佈點";
                    }
                    //CaxLog.ShowListingWindow("thisSender:" + thisSender.ToString());//顯示此列表的總數量
                    //CaxLog.ShowListingWindow("thisSender.SelectedItem.ToString:" + thisSender.SelectedItem.ToString());//顯示選到的中文字



                }
                catch (System.Exception ex)
                {
                	
                }
                
            }


        }

        #endregion

        private void InitializeGrid()
        {
            GridPanel panel = superGridControlCreateWEPt.PrimaryGrid;

            panel.Columns["ListWEArea"].EditorType = typeof(ListWEArea);

            string[] orderArray = { "選擇加工類型", "垂直方孔", "垂直外形", "垂直圓孔", "斜銷孔", "錐度孔", "錐度外形", "開放式外形", "開放式錐度外形" };

            panel.Columns["GridWeType"].EditorType = typeof(FragrantComboBox);
            panel.Columns["GridWeType"].EditorParams = new object[] { orderArray };


            //superGridControlCreateWEPt.CellValueChanged += superGridControlCreateWEPt_CellValueChanged;
        }

        #region 點"開始佈點"
        internal class ListWEArea : GridButtonXEditControl
        {
            private static UFSession theUfSession;
            public ListWEArea()
            {
                try
                {
                    Click += ListWEAreaClick;
                }
                catch (System.Exception ex)
                {
                }
            }
            public void ListWEAreaClick(object sender, EventArgs e)
            {
                ListWEArea cListWEArea = (ListWEArea)sender;
                Form mainForm = cListWEArea.FindForm();
                mainForm.Hide();

                //如果有球體的資訊，在一開始進來時候就刪除
                if (ListnXObject1.Count != 0)
                {
                    foreach (NXObject temp in ListnXObject1)
                    {
                        CaxPart.DeleteNXObject(temp);
                    }
                    ListnXObject1 = new List<NXObject>();
                }

                try
                {
                    PartCleanupComponents();

                    if (ListHighlightFace.Count != 0)
                    {
                        foreach (Face tempFace in ListHighlightFace)
                        {
                            tempFace.Unhighlight();
                        }
                    }

                    List<Face> ListFaceGroup = new List<Face>();

                    int sel_index = cListWEArea.EditorCell.RowIndex;

                    for (int i = 0; i < sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[sel_index].faceOccAry.Count; i++)
                    {
                        sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[sel_index].faceOccAry[i].Highlight();
                        ListFaceGroup.Add(sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[sel_index].faceOccAry[i]);
                    }

                    //"1.垂直方孔", "2.垂直外形", "3.垂直圓孔", "4.斜銷孔", "5.錐度孔", "6.錐度外形", "7.開放式外形", "8.開放式錐度外形"

                    theUfSession = UFSession.GetUFSession();
                    Session theSession_ = Session.GetSession();
                    Part workPart = theSession_.Parts.Work;
                    Part displayPart = theSession_.Parts.Display;
                    string title = "Select point";
                    int response = 0;

                    Tag point_tag = NXOpen.Tag.Null;
                    UFUi.PointBaseMethod base_method = UFUi.PointBaseMethod.PointInferred;
                    double[] base_pt = new double[3];
                    displayPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Top, NXOpen.View.ScaleAdjustment.Fit);

                    //CaxLog.ShowListingWindow(sWeGroupFacePntInput.sWeListKey.section);

                    //CaxLog.ShowListingWindow("SlopePin:" + CaxWE.SlopePin);

                    GridRow tempGridRow = (GridRow)this.EditorCell.Parent;
                    string type = tempGridRow.Cells["GridWeType"].Value.ToString();
                    try
                    {
                        if (type == "選擇加工類型")
                        {
                            UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "請先選擇加工類型");
                        }
                        else
                        {
                            string machiningtype = "";
                            //CaxLog.ShowListingWindow("machiningtype:" + machiningtype);
                            try
                            {
                                machiningtype = sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[sel_index].faceOccAry[0].GetStringAttribute("WE_TYPE");
                            }
                            catch (System.Exception ex)
                            {
                                machiningtype = sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[sel_index].faceOccAry[0].GetStringAttribute("MACHING_TYPE");
                            }

                            if (type != "選擇加工類型" && (type == "開放式外形" || type == "開放式錐度外形"))
                            {
                                string tempX = "";
                                string tempY = "";
                                string titletitle = "";
                                for (int i = 0; i < 2; i++)
                                {
                                    if (i == 0)
                                    {
                                        titletitle = "選擇第一個穿線點";
                                    }
                                    else
                                    {
                                        titletitle = "選擇第二個穿線點";
                                    }
                                    theUfSession.Ui.PointConstruct(titletitle, ref base_method, out point_tag, base_pt, out response);
                                    //將手動佈的點用球體顯示
                                    CreateSphere(base_pt, out nXObject1, 186);
                                    ListnXObject1.Add(nXObject1);
                                    if (response == 2)
                                    {
                                        tempGridRow.Cells["X"].Value = tempX + "_" + Math.Round(base_pt[0], 4).ToString();
                                        tempGridRow.Cells["Y"].Value = tempY + "_" + Math.Round(base_pt[1], 4).ToString();
                                        tempX = Math.Round(base_pt[0], 4).ToString();
                                        tempY = Math.Round(base_pt[1], 4).ToString();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                if (response == 2)
                                {
                                    cListWEArea.EditorCell.Value = "已佈點";
                                }
                            }
                            else if (type != "選擇加工類型" && (type != "開放式外形" || type != "開放式錐度外形"))
                            {
                                theUfSession.Ui.PointConstruct(title, ref base_method, out point_tag, base_pt, out response);

                                //將手動佈的點用球體顯示
                                CreateSphere(base_pt, out nXObject1, 186);
                                ListnXObject1.Add(nXObject1);

                                if (/*CaxWE.SlopePin == "斜銷"*/ CaxWE.Task_Type == "3" && sWeGroupFacePntInput.sWeListKey.section == "WECAM")
                                {
                                    if (response == 2)
                                    {
                                        UFModl.RayHitPointInfo cRayHitPointInfo;
                                        CLA101_CSUGFunc.detectPointOnSurface(base_pt, ListFaceGroup, out cRayHitPointInfo);
                                        base_pt[0] = cRayHitPointInfo.hit_point[0] + (1.0 * cRayHitPointInfo.hit_normal[0]);
                                        base_pt[1] = cRayHitPointInfo.hit_point[1] + (1.0 * cRayHitPointInfo.hit_normal[1]);
                                        tempGridRow.Cells["X"].Value = Math.Round(base_pt[0], 4).ToString();
                                        tempGridRow.Cells["Y"].Value = Math.Round(base_pt[1], 4).ToString();
                                        cListWEArea.EditorCell.Value = "已佈點";
                                    }
                                }
                                else
                                {
                                    if (response == 2)
                                    {
                                        //GridRow tempGridRow = (GridRow)this.EditorCell.Parent;
                                        tempGridRow.Cells["X"].Value = Math.Round(base_pt[0], 4).ToString();
                                        tempGridRow.Cells["Y"].Value = Math.Round(base_pt[1], 4).ToString();
                                        cListWEArea.EditorCell.Value = "已佈點";
                                    }
                                }
                            }
                            else
                            {
                                //CaxLog.ShowListingWindow("2");
                                string tempX = "";
                                string tempY = "";
                                string titletitle = "";
                                for (int i = 0; i < 2; i++)
                                {
                                    if (i == 0)
                                    {
                                        titletitle = "選擇第一個穿線點";
                                    }
                                    else
                                    {
                                        titletitle = "選擇第二個穿線點";
                                    }
                                    theUfSession.Ui.PointConstruct(titletitle, ref base_method, out point_tag, base_pt, out response);

                                    //將手動佈的點用球體顯示
                                    CreateSphere(base_pt, out nXObject1, 186);
                                    ListnXObject1.Add(nXObject1);

                                    if (response == 2)
                                    {
                                        tempGridRow.Cells["X"].Value = tempX + "_" + Math.Round(base_pt[0], 4).ToString();
                                        tempGridRow.Cells["Y"].Value = tempY + "_" + Math.Round(base_pt[1], 4).ToString();
                                        tempX = Math.Round(base_pt[0], 4).ToString();
                                        tempY = Math.Round(base_pt[1], 4).ToString();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                if (response == 2)
                                {
                                    cListWEArea.EditorCell.Value = "已佈點";
                                }
                            }
                        }

                    }
                    catch (System.Exception ex)
                    {
                        //CaxLog.ShowListingWindow("辨識不出來");
                        if (type == "選擇加工類型")
                        {
                            UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "請先選擇加工類型");
                        }
                        else if (type == "開放式外形" || type == "開放式錐度外形")
                        {
                            //CaxLog.ShowListingWindow("選擇了開放式錐度外形OR開放式外形");
                            string tempX = "";
                            string tempY = "";
                            string titletitle = "";
                            for (int i = 0; i < 2; i++)
                            {
                                if (i == 0)
                                {
                                    titletitle = "選擇第一個穿線點";
                                }
                                else
                                {
                                    titletitle = "選擇第二個穿線點";
                                }
                                theUfSession.Ui.PointConstruct(titletitle, ref base_method, out point_tag, base_pt, out response);
                                //將手動佈的點用球體顯示
                                CreateSphere(base_pt, out nXObject1, 186);
                                ListnXObject1.Add(nXObject1);
                                if (response == 2)
                                {
                                    //GridRow tempGridRow = (GridRow)this.EditorCell.Parent;
                                    tempGridRow.Cells["X"].Value = tempX + "_" + Math.Round(base_pt[0], 4).ToString();
                                    tempGridRow.Cells["Y"].Value = tempY + "_" + Math.Round(base_pt[1], 4).ToString();
                                    tempX = Math.Round(base_pt[0], 4).ToString();
                                    tempY = Math.Round(base_pt[1], 4).ToString();
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (response == 2)
                            {
                                cListWEArea.EditorCell.Value = "已佈點";
                            }
                        }
                        else
                        {
                            //CaxLog.ShowListingWindow("選擇了開放式錐度外形OR開放式外形，但只能打一點");
                            theUfSession.Ui.PointConstruct(title, ref base_method, out point_tag, base_pt, out response);
                            //將手動佈的點用球體顯示
                            CreateSphere(base_pt, out nXObject1, 186);
                            ListnXObject1.Add(nXObject1);
                            if (response == 2)
                            {
                                //GridRow tempGridRow = (GridRow)this.EditorCell.Parent;
                                tempGridRow.Cells["X"].Value = Math.Round(base_pt[0], 4).ToString();
                                tempGridRow.Cells["Y"].Value = Math.Round(base_pt[1], 4).ToString();
                                cListWEArea.EditorCell.Value = "已佈點";
                            }
                        }
                    }


                    for (int i = 0; i < sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[sel_index].faceOccAry.Count; i++)
                    {
                        sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[sel_index].faceOccAry[i].Unhighlight();
                        //sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[sel_index].faceOccAry[i].SetAttribute("WE_TYPE", WE_TYPE_NUMBER);
                    }

                    PartCleanupComponents();
                }
                catch (System.Exception ex)
                {
                    CaxLog.ShowListingWindow("ex : " + ex.ToString());
                }
                mainForm.Show();
            }
        }
        #endregion

        private void CreateWEPoint_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Owner.Enabled = true;
            this.Owner.Show();
        }

        //自動佈點後，將值塞進來
        private void CreateWEPoint_Load(object sender, EventArgs e)
        {
            //             CaxLoadingDlg sCaxLoadingDlg = new CaxLoadingDlg();
            //             sCaxLoadingDlg.Run();
            //             sCaxLoadingDlg.SetLoadingText("數據計算中...");

            GridRow row = new GridRow();
            for (int i = 0; i < sWeGroupFacePntInput.sWeFace.sFaceGroupPnt.Count; i++)
            {
                string we_type = "選擇加工類型";
                string pnt_x = "";
                string pnt_y = "";
                for (int j = 0; j < sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry.Count; j++)
                {
                    try
                    {
                        we_type = sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry[j].GetStringAttribute("WE_TYPE");
                        //we_type = sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry[j].GetStringAttribute("MACHING_TYPE");
                        if (we_type == "1")
                        {
                            we_type = "垂直方孔";
                        }
                        else if (we_type == "2")
                        {
                            we_type = "垂直外形";
                        }
                        else if (we_type == "3")
                        {
                            we_type = "垂直圓孔";
                        }
                        else if (we_type == "4")
                        {
                            we_type = "斜銷孔";
                        }
                        else if (we_type == "5")
                        {
                            we_type = "錐度孔";
                        }
                        else if (we_type == "6")
                        {
                            we_type = "錐度外形";
                        }
                        else if (we_type == "7")
                        {
                            we_type = "開放式外形";
                        }
                        else if (we_type == "8")
                        {
                            we_type = "開放式錐度外形";
                        }
                    }
                    catch (System.Exception ex)
                    { }

                    //如果要做加工類型的排序，在這邊執行，並把pnt_x和pnt_y移到外面去

                    try
                    {
                        pnt_x = sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry[j].GetStringAttribute("WE_PNT_X");
                    }
                    catch (System.Exception ex)
                    { }

                    try
                    {
                        pnt_y = sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry[j].GetStringAttribute("WE_PNT_Y");
                    }
                    catch (System.Exception ex)
                    { }

                    if (we_type != "" && pnt_x != "" && pnt_y != "")
                    {
                        break;
                    }
                }

                if (we_type == "")
                {
                    we_type = "選擇加工類型";
                }
                //此處之後要判斷當自動布點切刀口失敗時，回傳pnt_x:failed則鎖住開始佈點
                if (pnt_x != "NaN" && pnt_y != "NaN" && we_type != "選擇加工類型" && pnt_x != "" && pnt_y != "" && pnt_x != "ITG" && pnt_x != "DERROR")
                {
                    row = new GridRow(i + 1, we_type, pnt_x, pnt_y, "已佈點");
                    superGridControlCreateWEPt.PrimaryGrid.Rows.Add(row);
                }
                else if (pnt_x == "NaN" || pnt_y == "NaN" || we_type == "選擇加工類型" || pnt_x == "" || pnt_y == "")
                {
                    row = new GridRow(i + 1, we_type, pnt_x, pnt_y, "開始佈點");
                    superGridControlCreateWEPt.PrimaryGrid.Rows.Add(row);
                }
                else if (pnt_x == "ITG" || pnt_x == "DERROR")
                {
                    row = new GridRow(i + 1, we_type, "ESPRIT手動處理", "ESPRIT手動處理", "已佈點");
                    superGridControlCreateWEPt.PrimaryGrid.Rows.Add(row);
                    superGridControlCreateWEPt.PrimaryGrid.GetCell(i, 4).ReadOnly = true;
                }
                
            }
            //superGridMainControl.PrimaryGrid.GetCell(index - 1, 5).ReadOnly = true;
            //sCaxLoadingDlg.Stop();


        }

        public static bool AskType_ChineseTransNumber(string WE_TYPE_CHINESE, out string WE_TYPE_NUM)
        {
            WE_TYPE_NUM = "";
            try
            {
                if (WE_TYPE_CHINESE == "垂直方孔")
                {
                    WE_TYPE_NUM = "1";
                }
                else if (WE_TYPE_CHINESE == "垂直外形")
                {
                    WE_TYPE_NUM = "2";
                }
                else if (WE_TYPE_CHINESE == "垂直圓孔")
                {
                    WE_TYPE_NUM = "3";
                }
                else if (WE_TYPE_CHINESE == "斜銷孔")
                {
                    WE_TYPE_NUM = "4";
                }
                else if (WE_TYPE_CHINESE == "錐度孔")
                {
                    WE_TYPE_NUM = "5";
                }
                else if (WE_TYPE_CHINESE == "錐度外形")
                {
                    WE_TYPE_NUM = "6";
                }
                else if (WE_TYPE_CHINESE == "開放式外形")
                {
                    WE_TYPE_NUM = "7";
                }
                else if (WE_TYPE_CHINESE == "開放式錐度外形")
                {
                    WE_TYPE_NUM = "8";
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool AskType_NumberTransChinese(string WE_TYPE_NUM, out string WE_TYPE_CHINESE)
        {
            WE_TYPE_CHINESE = "";
            try
            {
                if (WE_TYPE_NUM == "1")
                {
                    WE_TYPE_CHINESE = "垂直方孔";
                }
                else if (WE_TYPE_NUM == "2")
                {
                    WE_TYPE_CHINESE = "垂直外形";
                }
                else if (WE_TYPE_NUM == "3")
                {
                    WE_TYPE_CHINESE = "垂直圓孔";
                }
                else if (WE_TYPE_NUM == "4")
                {
                    WE_TYPE_CHINESE = "斜銷孔";
                }
                else if (WE_TYPE_NUM == "5")
                {
                    WE_TYPE_CHINESE = "錐度孔";
                }
                else if (WE_TYPE_NUM == "6")
                {
                    WE_TYPE_CHINESE = "錐度外形";
                }
                else if (WE_TYPE_NUM == "7")
                {
                    WE_TYPE_CHINESE = "開放式外形";
                }
                else if (WE_TYPE_NUM == "8")
                {
                    WE_TYPE_CHINESE = "開放式錐度外形";
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private void superGridControlCreateWEPt_CellValueChanged(object sender, GridCellValueChangedEventArgs e)
        {
            /*
            GridCell cell = e.GridCell;
            if (cell.GridColumn.Name.Equals("ListWEArea"))
            {
                GridRow row = cell.GridRow;
                for (int i = 0; i < superGridControlCreateWEPt.PrimaryGrid.Rows.Count; i++)
                {
                    if (i == row.Index)
                    {
                        continue;
                    }
                    bool check = (bool)superGridControlCreateWEPt.PrimaryGrid.GridPanel.GetCell(i, 0).Value;
                    if (check)
                    {
                        superGridControlCreateWEPt.PrimaryGrid.GridPanel.GetCell(i, 0).Value = (bool)false;
                    }
                }

                bool cur_check = (bool)superGridControlCreateWEPt.PrimaryGrid.GridPanel.GetCell(row.Index, 0).Value;
                if (!cur_check)
                {
                    superGridControlCreateWEPt.PrimaryGrid.GridPanel.GetCell(row.Index, 0).Value = (bool)false;
                }
                else
                {
                    superGridControlCreateWEPt.PrimaryGrid.GridPanel.GetCell(row.Index, 0).Value = (bool)true;
                }
            }
            */
        }

        private void buttonOK_CreateWEPt_Click(object sender, EventArgs e)
        {

            //Unhighlight
            if (ListHighlightFace.Count != 0)
            {
                foreach (Face tempFace in ListHighlightFace)
                {
                    tempFace.Unhighlight();
                }
            }

            //如果有球體的資訊，在一開始進來時候就刪除
            if (ListnXObject1.Count != 0)
            {
                foreach (NXObject temp in ListnXObject1)
                {
                    CaxPart.DeleteNXObject(temp);
                }
                ListnXObject1 = new List<NXObject>();
            }

            List<FaceGroupPnt> faceGroupPntAry = new List<FaceGroupPnt>();

            string we_type;
            string pnt_x;
            string pnt_y;
            bool check = false;

            for (int i = 0; i < superGridControlCreateWEPt.PrimaryGrid.Rows.Count; i++)
            {
                we_type = superGridControlCreateWEPt.ActiveGrid.GetCell(i, 1).Value.ToString();
                pnt_x = superGridControlCreateWEPt.ActiveGrid.GetCell(i, 2).Value.ToString();
                pnt_y = superGridControlCreateWEPt.ActiveGrid.GetCell(i, 3).Value.ToString();

//                 CaxLog.ShowListingWindow("we_type:" + we_type);
//                 CaxLog.ShowListingWindow("pnt_x:" + pnt_x);
//                 CaxLog.ShowListingWindow("pnt_y:" + pnt_y);
//                 CaxLog.ShowListingWindow("---");

                WE_TYPE_ChineseTransNumber(ref we_type);


                if (we_type == "選擇加工類型" || pnt_x == "NaN" || pnt_y == "NaN")
                {
                    check = true;
                }

                FaceGroupPnt sFaceGroupPnt;
                sFaceGroupPnt.faceOccAry = new List<Face>();
                sFaceGroupPnt.faceOccAry.AddRange(sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry);
                sFaceGroupPnt.pnt_x = pnt_x;
                sFaceGroupPnt.pnt_y = pnt_y;
                faceGroupPntAry.Add(sFaceGroupPnt);

                for (int j = 0; j < sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry.Count; j++)
                {
                    CaxPart.CreateStringAttr(sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry[j], CaxDefineParam.ATTR_CATEGORY, "WE_TYPE", we_type.ToString());
                    CaxPart.CreateStringAttr(sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry[j], CaxDefineParam.ATTR_CATEGORY, "WE_PNT_X", pnt_x.ToString());
                    CaxPart.CreateStringAttr(sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry[j], CaxDefineParam.ATTR_CATEGORY, "WE_PNT_Y", pnt_y.ToString());
                    CaxPart.CreateStringAttr((Face)sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry[j].Prototype, CaxDefineParam.ATTR_CATEGORY, "WE_TYPE", we_type.ToString());
                    CaxPart.CreateStringAttr((Face)sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry[j].Prototype, CaxDefineParam.ATTR_CATEGORY, "WE_PNT_X", pnt_x.ToString());
                    CaxPart.CreateStringAttr((Face)sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[i].faceOccAry[j].Prototype, CaxDefineParam.ATTR_CATEGORY, "WE_PNT_Y", pnt_y.ToString());
                }
            }

            if (check == true)
            {
                //變更為"未佈點"
                ((DevComponents.DotNetBar.SuperGrid.GridRow)(CaxWE.cSelectWorkPartFrom.superGridMainControl.ActiveRow)).Cells[1].Value = "未佈點";
                if (IsManual == true/* || CurrentPartStatus == "-2"*/)
                {
                    ((DevComponents.DotNetBar.SuperGrid.GridRow)(CaxWE.cSelectWorkPartFrom.superGridMainControl.ActiveRow)).Cells[1].Value = "手動任務";
                }
            }
            else
            {
                //變更為"已佈點"
                ((DevComponents.DotNetBar.SuperGrid.GridRow)(CaxWE.cSelectWorkPartFrom.superGridMainControl.ActiveRow)).Cells[1].Value = "已佈點";
                if (IsManual == true || CurrentPartStatus == "-2")
                {
                    ((DevComponents.DotNetBar.SuperGrid.GridRow)(CaxWE.cSelectWorkPartFrom.superGridMainControl.ActiveRow)).Cells[1].Value = "手動任務";
                }
            }

            WeFaceGroup sWeFaceGroup;
            SelectWorkPart.WE_FACE_DIC.TryGetValue(sWeGroupFacePntInput.sWeListKey, out sWeFaceGroup);
            sWeFaceGroup.sFaceGroupPnt = new List<FaceGroupPnt>();
            sWeFaceGroup.sFaceGroupPnt.AddRange(faceGroupPntAry);
            SelectWorkPart.WE_FACE_DIC[sWeGroupFacePntInput.sWeListKey] = sWeFaceGroup;

            IsManual = false;
            this.Owner.Enabled = true;
            this.Owner.Show();
            this.Close();

        }

        private void buttonCancel_CreateWEPt_Click(object sender, EventArgs e)
        {
            //Unhighlight
            if (ListHighlightFace.Count != 0)
            {
                foreach (Face tempFace in ListHighlightFace)
                {
                    tempFace.Unhighlight();
                }
            }

            //如果有球體的資訊，在一開始進來時候就刪除
            if (ListnXObject1.Count != 0)
            {
                foreach (NXObject temp in ListnXObject1)
                {
                    CaxPart.DeleteNXObject(temp);
                }
                ListnXObject1 = new List<NXObject>();
            }

            IsManual = false;
            this.Owner.Enabled = true;
            this.Owner.Show();
            this.Close();
            /*
            bool chk001 = false;
            if (IsManual)
            {
                ((DevComponents.DotNetBar.SuperGrid.GridRow)(CaxWE.cSelectWorkPartFrom.superGridMainControl.ActiveRow)).Cells[1].Value = "手動任務";
                IsManual = false;
                this.Owner.Enabled = true;
                this.Owner.Show();
                this.Close();
                chk001 = true;
            }

            if (!chk001)
            {
                bool chk002 = false;
                //UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "請確認此工件是否轉手動任務");
                string Msg = "請確認此工件是否轉手動任務，如需轉手動任務請選擇NO並勾選轉手動任務；仍要退出請選擇YES";
                eTaskDialogResult chk_yes_no;
                chk_yes_no = CaxMsg.ShowMsgYesNo(Msg);
                if (chk_yes_no == eTaskDialogResult.No)
                {
                    chk002 = true;
                }

                if (!chk002)
                {
                    IsManual = false;
                    this.Owner.Enabled = true;
                    this.Owner.Show();
                    this.Close();
                }
            }
            */
        }

        public static string WE_TYPE_ChineseTransNumber(ref string we_type)
        {
            if (we_type == "垂直方孔")
            {
                we_type = "1";
            }
            else if (we_type == "垂直外形")
            {
                we_type = "2";
            }
            else if (we_type == "垂直圓孔")
            {
                we_type = "3";
            }
            else if (we_type == "斜銷孔")
            {
                we_type = "4";
            }
            else if (we_type == "錐度孔")
            {
                we_type = "5";
            }
            else if (we_type == "錐度外形")
            {
                we_type = "6";
            }
            else if (we_type == "開放式外形")
            {
                we_type = "7";
            }
            else if (we_type == "開放式錐度外形")
            {
                we_type = "8";
            }
            return we_type;
        }

        public static bool GetWeNeighborFace(Face face, string[] copyAllfaceAry, ref List<Face> faceAry)
        {
            try
            {
                Face neighborFace = null;

                //HoleGroup sHoleGroup;
                //sHoleGroup.faceAry = new List<Face>();
                //sHoleGroup.type = holeType;

                //string attr_value = "";

                Edge[] edgeAry = face.GetEdges();
                for (int j = 0; j < edgeAry.Length; j++)
                {
                    neighborFace = null;
                    neighborFace = CaxGeom.GetNeighborFace(face, edgeAry[j]);
                    if (neighborFace == null)
                    {
                        continue;
                    }
                    //                     neighborFace.Highlight();
                    //                     CaxLog.ShowListingWindow("neighborFace : " + neighborFace.Tag.ToString());

                    try
                    {
                        bool chk_face_str = false;
                        foreach (string loopFace in copyAllfaceAry)
                        {
                            if (loopFace == neighborFace.Tag.ToString())
                            {
                                chk_face_str = true;
                                break;
                            }
                        }
                        if (!chk_face_str)
                        {
                            continue;
                        }
                        /*
                        attr_value = "";
                        attr_value = neighborFace.GetStringAttribute("Feature_Type");
                        if (sHoleGroup.type != attr_value)
                        {
                            continue;
                        }
                        */

                        bool chk_face = false;
                        for (int k = 0; k < faceAry.Count; k++)
                        {
                            if (faceAry[k] == neighborFace)
                            {
                                chk_face = true;
                            }
                        }
                        if (!chk_face)
                        {
                            faceAry.Add(neighborFace);
                            GetWeNeighborFace(neighborFace, copyAllfaceAry, ref faceAry);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        continue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow(ex.ToString());
                return false;
            }

            return true;
        }

        public static bool PartCleanupComponents()
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: File->Utilities->Part Cleanup...
                // ----------------------------------------------

                PartCleanup partCleanup1;
                partCleanup1 = theSession.NewPartCleanup();
                partCleanup1.TurnOffHighlighting = true;
                partCleanup1.PartsToCleanup = NXOpen.PartCleanup.CleanupParts.Components;
                partCleanup1.DoCleanup();
                partCleanup1.Dispose();
            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        private void superGridControlCreateWEPt_CellClick(object sender, GridCellClickEventArgs e)
        {
            //如果有球體的資訊，在一開始進來時候就刪除
            if (ListnXObject1.Count != 0)
            {
                foreach (NXObject temp in ListnXObject1)
                {
                    CaxPart.DeleteNXObject(temp);
                }
                ListnXObject1 = new List<NXObject>();
            }

            Session theSession_ = Session.GetSession();
            Part displayPart = theSession_.Parts.Display;
            displayPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Top, NXOpen.View.ScaleAdjustment.Fit);

            int Click_Form1_Index = e.GridCell.GridRow.Index;

            if (ListHighlightFace.Count != 0)
            {
                foreach (Face tempFace in ListHighlightFace)
                {
                    tempFace.Unhighlight();
                }
            }

            for (int i = 0; i < sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[Click_Form1_Index].faceOccAry.Count; i++)
            {
                sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[Click_Form1_Index].faceOccAry[i].Highlight();
                ListHighlightFace.Add(sWeGroupFacePntInput.sWeFace.sFaceGroupPnt[Click_Form1_Index].faceOccAry[i]);
            }

            string Xpoint = "";
            string Ypoint = "";
            try
            {
                Xpoint = superGridControlCreateWEPt.GetCell(Click_Form1_Index, 2).Value.ToString();
                Ypoint = superGridControlCreateWEPt.GetCell(Click_Form1_Index, 3).Value.ToString();
            }
            catch (System.Exception ex)
            { }

            if (Xpoint == "ESPRIT_AUTO" || Xpoint == "NaN" || Xpoint == "ESPRIT手動處理" || Xpoint == "")
            {
                return;
            }

            string[] Xpoint_split = Xpoint.Split('_');
            string[] Ypoint_split = Ypoint.Split('_');

            double[] firstPt = new double[3];
            double[] secondPt = new double[3];


            if (Xpoint_split.Length > 1)
            {
                firstPt = new double[3] { Convert.ToDouble(Xpoint_split[0]), Convert.ToDouble(Ypoint_split[0]), Convert.ToDouble(50) };
                secondPt = new double[3] { Convert.ToDouble(Xpoint_split[1]), Convert.ToDouble(Ypoint_split[1]), Convert.ToDouble(50) };
                CreateSphere(firstPt, out nXObject1, 186);
                ListnXObject1.Add(nXObject1);
                CreateSphere(secondPt, out nXObject1, 186);
                ListnXObject1.Add(nXObject1);
            }
            else
            {
                firstPt = new double[3] { Convert.ToDouble(Xpoint_split[0]), Convert.ToDouble(Ypoint_split[0]), Convert.ToDouble(50) };
                CreateSphere(firstPt, out nXObject1, 186);
                ListnXObject1.Add(nXObject1);
            }



        }

        private void checkBoxX1_manual_CheckedChanged(object sender, EventArgs e)
        {
            if (IsManual == true)
            {
                IsManual = false;
            }
            else
            {
                IsManual = true;
            }

        }

        public static bool CreateSphere(double[] point, out NXObject nXObject1, int colornum)
        {
            nXObject1 = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Design Feature->Sphere...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.Sphere nullFeatures_Sphere = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.SphereBuilder sphereBuilder1;
                sphereBuilder1 = workPart.Features.CreateSphereBuilder(nullFeatures_Sphere);

                sphereBuilder1.Diameter.RightHandSide = "10";

                sphereBuilder1.BooleanOption.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

                Body[] targetBodies1 = new Body[1];
                Body nullBody = null;
                targetBodies1[0] = nullBody;
                sphereBuilder1.BooleanOption.SetTargetBodies(targetBodies1);

                theSession.SetUndoMarkName(markId1, "Sphere Dialog");

                NXOpen.Point nullPoint = null;
                sphereBuilder1.CenterPoint = nullPoint;

                Unit unit1;
                unit1 = sphereBuilder1.Diameter.Units;

                Expression expression1;
                expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                Expression expression2;
                expression2 = workPart.Expressions.CreateSystemExpressionWithUnits(point[0].ToString(), unit1);

                Scalar scalar1;
                scalar1 = workPart.Scalars.CreateScalarExpression(expression2, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression3;
                expression3 = workPart.Expressions.CreateSystemExpressionWithUnits(point[1].ToString(), unit1);

                Scalar scalar2;
                scalar2 = workPart.Scalars.CreateScalarExpression(expression3, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression4;
                expression4 = workPart.Expressions.CreateSystemExpressionWithUnits(point[2].ToString(), unit1);

                Scalar scalar3;
                scalar3 = workPart.Scalars.CreateScalarExpression(expression4, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                NXOpen.Point point1;
                point1 = workPart.Points.CreatePoint(scalar1, scalar2, scalar3, NXOpen.SmartObject.UpdateOption.WithinModeling);

                sphereBuilder1.CenterPoint = point1;

                sphereBuilder1.Diameter.RightHandSide = "0.5";

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Sphere");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Sphere");

                //NXObject nXObject1;
                nXObject1 = sphereBuilder1.Commit();

                SetColor(nXObject1, colornum);

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "Sphere");

                Expression expression5 = sphereBuilder1.Diameter;
                sphereBuilder1.Destroy();

                workPart.Expressions.Delete(expression1);

                // ----------------------------------------------
                //   Menu: Tools->Journal->Stop Recording
                // ----------------------------------------------
            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        public static void SetColor(NXObject nxobject, int colornum)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Edit->Object Display...
            // ----------------------------------------------
            // ----------------------------------------------
            //   Dialog Begin Color
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Edit Object Display");

            DisplayModification displayModification1;
            displayModification1 = theSession.DisplayManager.NewDisplayModification();

            displayModification1.ApplyToAllFaces = false;

            displayModification1.ApplyToOwningParts = false;

            displayModification1.NewColor = colornum;

            DisplayableObject[] objects1 = new DisplayableObject[1];
            Body body1 = (Body)workPart.Bodies.FindObject(nxobject.JournalIdentifier);
            objects1[0] = body1;
            displayModification1.Apply(objects1);

            displayModification1.Dispose();
            // ----------------------------------------------
            //   Menu: Tools->Journal->Stop Recording
            // ----------------------------------------------

        }

    }
}
