using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Threading;
using WeData;
using System.IO;
using NXCustomerComponent;
using NXOpen.UF;
using Newtonsoft.Json;
using System.Threading.Tasks;
using DevComponents.DotNetBar;
using CimforceCaxTwFixture;

namespace CaxUGforEspritt
{

    public partial class SelectWorkPart : DevComponents.DotNetBar.Office2007Form
    {
        private static Session theSession;
        private static UI theUI;
        private static UFSession theUfSession;
        public static Dictionary<WeListKey, WeFaceGroup> WE_FACE_DIC = new Dictionary<WeListKey, WeFaceGroup>();
        public static WeGroupFacePnt sWeGroupFacePnt;
        public static string CurrentPartStatus = "";
        public static WeFixSystem cWeFixSystem = new WeFixSystem();
        public bool status;
        public static string IsCountersunk = "SHCS_Hole";
        public static int Click_Form1_Index;

        public delegate void MyInvoke(Form xx);

        public struct GridTask
        {
            public int sel_index;
            public string object_no;
        }



        public SelectWorkPart()
        {
            InitializeComponent();
            InitializeGrid();
            theSession = Session.GetSession();
            theUI = UI.GetUI();
            theUfSession = UFSession.GetUFSession();
        }

        private void InitializeGrid()
        {
            GridPanel panel = superGridMainControl.PrimaryGrid;
            panel.Columns["ListWEPoint"].EditorType = typeof(ListWEPoint);
            panel.Columns["ListWorkPiece"].EditorType = typeof(ListWorkPiece);
            panel.Columns["WeFixture"].EditorType = typeof(WeFixture);
        }

        #region 點"選擇治具"
        internal class WeFixture : GridButtonXEditControl
        {
            WeListKey sWeListKey;
            public WeFixture()
            {
                try
                {
                    Click += WeFixtureClick;
                }
                catch (System.Exception ex)
                {
                }
            }
            public void WeFixtureClick(object sender, EventArgs e)
            {
                //擺要執行的動作
                WeFixture cListWEPoint = (WeFixture)sender;

                //****
                //隱藏所有的NewComp
                List<NXOpen.Assemblies.Component> hideNewComp = new List<NXOpen.Assemblies.Component>();
                foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WE_FACE_DIC)
                {
                    hideNewComp.Add(kvp.Value.comp);
                }
                Hide_All_Comp(hideNewComp);

                //顯示選取的NewComp
                int Click_Form1_Index = cListWEPoint.EditorCell.RowIndex;

                sWeListKey = new WeListKey();
                sWeListKey.compName = cListWEPoint.EditorCell.GridRow.Cells["ListWorkPiece"].Value.ToString().ToUpper();
                sWeListKey.section = cListWEPoint.EditorCell.GridRow.Cells["Process"].Value.ToString();
                sWeListKey.wkface = cListWEPoint.EditorCell.GridRow.Cells["WK_Face"].Value.ToString();

                WeFaceGroup sWeFaceGroup = new WeFaceGroup();
                WE_FACE_DIC.TryGetValue(sWeListKey, out sWeFaceGroup);
                CurrentPartStatus = sWeFaceGroup.isAnalyzeSucess.ToString();
                Show_Select_componet(sWeFaceGroup.comp);
                //****

                Form mainForm = cListWEPoint.FindForm();

                CaxAsm.SetWorkComponent(sWeFaceGroup.comp);
                //Program.WeGroupFacePnt sWeGroupFacePnt;
                sWeGroupFacePnt.mainForm = mainForm;
                sWeGroupFacePnt.sWeFace = sWeFaceGroup;
                sWeGroupFacePnt.sWeListKey = sWeListKey;
                

                //int Click_Form1_Index = cListWEPoint.EditorCell.RowIndex;

                mainForm.Hide();
                Thread threadFrom = new Thread(new ParameterizedThreadStart(DoWork));
                threadFrom.Start(mainForm);
            }

            public void DoWork(object mainForm)
            {
                MyInvoke mi = new MyInvoke(open_new_form);
                this.BeginInvoke(mi, new Object[] { mainForm });
                //System.Windows.Forms.Application.DoEvents();
            }

            public void open_new_form(Form mainForm)
            {
                WeFixtureDlg CreateWeFixtureDlg = new WeFixtureDlg(cWeFixSystem.allowFixtureDataLstSY, 
                                                                                                  cWeFixSystem.FixtureDir,
                                                                                                  WE_FACE_DIC,
                                                                                                  Click_Form1_Index,
                                                                                                  sWeListKey);

                CreateWeFixtureDlg.Owner = mainForm;
                CreateWeFixtureDlg.Show();
                CreateWeFixtureDlg.Owner.Enabled = false;
            }
        }
        #endregion

        #region 點"佈穿線點"
        internal class ListWEPoint : GridButtonXEditControl
        {
            public ListWEPoint()
            {
                try
                {
                    Click += ListWEPointClick;
                }
                catch (System.Exception ex)
                {
                }
            }
            public void ListWEPointClick(object sender, EventArgs e)
            {
                try
                {
                    //擺要執行的動作
                    ListWEPoint cListWEPoint = (ListWEPoint)sender;

                    //****
                    //隱藏所有的NewComp
                    List<NXOpen.Assemblies.Component> hideNewComp = new List<NXOpen.Assemblies.Component>();
                    foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WE_FACE_DIC)
                    {
                        hideNewComp.Add(kvp.Value.comp);
                    }
                    Hide_All_Comp(hideNewComp);

                    //顯示選取的NewComp
                    int Click_Form1_Index = cListWEPoint.EditorCell.RowIndex;

                    WeListKey sWeListKey = new WeListKey();
                    sWeListKey.compName = cListWEPoint.EditorCell.GridRow.Cells["ListWorkPiece"].Value.ToString().ToUpper();
                    sWeListKey.section = cListWEPoint.EditorCell.GridRow.Cells["Process"].Value.ToString();
                    sWeListKey.wkface = cListWEPoint.EditorCell.GridRow.Cells["WK_Face"].Value.ToString();

                    WeFaceGroup sWeFaceGroup = new WeFaceGroup();
                    WE_FACE_DIC.TryGetValue(sWeListKey, out sWeFaceGroup);
                    CurrentPartStatus = sWeFaceGroup.isAnalyzeSucess.ToString();
                    Show_Select_componet(sWeFaceGroup.comp);
                    //****

                    Form mainForm = cListWEPoint.FindForm();

                    CaxAsm.SetWorkComponent(sWeFaceGroup.comp);
                    //Program.WeGroupFacePnt sWeGroupFacePnt;
                    sWeGroupFacePnt.mainForm = mainForm;
                    sWeGroupFacePnt.sWeFace = sWeFaceGroup;
                    sWeGroupFacePnt.sWeListKey = sWeListKey;


                    //int Click_Form1_Index = cListWEPoint.EditorCell.RowIndex;

                    mainForm.Hide();
                    Thread threadFrom = new Thread(new ParameterizedThreadStart(DoWork));
                    threadFrom.Start(mainForm);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            public void DoWork(object mainForm)
            {
                MyInvoke mi = new MyInvoke(open_new_form);
                this.BeginInvoke(mi, new Object[] { mainForm });
                //System.Windows.Forms.Application.DoEvents();
            }

            public void open_new_form(Form mainForm)
            {
                CreateWEPoint CreateWEPointForm = new CreateWEPoint(sWeGroupFacePnt, CurrentPartStatus);

                CreateWEPointForm.Owner = mainForm;
                CreateWEPointForm.Show();
                CreateWEPointForm.Owner.Enabled = false;
            }
        }
        #endregion

        #region 點"物件名稱"，顯示選擇的物件
        internal class ListWorkPiece : GridLabelXEditControl
        {
            public ListWorkPiece()
            {
                try
                {
                    Click += ListWorkPieceClick;
                }
                catch (System.Exception ex)
                {
                }
            }
            public void ListWorkPieceClick(object sender, EventArgs e)
            {

                //                 try
                //                 {
                //                     //擺要執行的動作
                //                     ListWorkPiece ss = (ListWorkPiece)sender;
                //                     int Click_Form1_Index = ss.EditorCell.RowIndex;
                //                     List<NXOpen.Assemblies.Component> hideNewComp = new List<NXOpen.Assemblies.Component>();
                //                     foreach (KeyValuePair<WeListKey,WeFaceGroup> kvp in WE_FACE_DIC)
                //                     {
                //                         hideNewComp.Add(kvp.Value.comp);
                //                     }
                //                     Hide_All_Comp(hideNewComp);
                //                     return;
                //                     Show_Select_componet(Program.sssComponent[Click_Form1_Index]);
                //                     CaxAsm.SetWorkComponent(Program.sssComponent[Click_Form1_Index]);
                //                 }
                //                 catch (System.Exception ex)
                //                 {
                //                     MessageBox.Show(ex.ToString());
                //                 }

            }
        }
        #endregion

        private void SelectWorkPart_Load(object sender, EventArgs e)
        {
            #region 將WINFORM視窗顯示在右上角
            //int x = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            //int y = 0;
            //this.Location = new System.Drawing.Point(x, y);
            #endregion

            #region 讀取治具資訊
            status = cWeFixSystem.GetFixtureConfigDat();
            if (!status)
            {
                CaxLog.ShowListingWindow("取得治具資訊失敗，請確認是否有治具配置檔");
            }

//             WeFixtureDlg cWeFixtureDlg = new WeFixtureDlg();
//             cWeFixtureDlg.allowFixtureDataLstSY = new List<WeFixData>();
//             cWeFixtureDlg.allowFixtureDataLstSY = cWeFixSystem.allowFixtureDataLstSY;


            #endregion

            GridRow row = new GridRow();

            WE_FACE_DIC = WEFaceDict;

            #region 解析成功(包含特徵辨識失敗的任務)
            int index = 0;
            foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WE_FACE_DIC)
            {
                index++;
                int isAnalyzeSucess = kvp.Value.isAnalyzeSucess;

                #region 處理回傳-2情況
                if (isAnalyzeSucess == -2)
                {
                    bool check_isSetPnt = true;
                    for (int i = 0; i < kvp.Value.sFaceGroupPnt.Count; i++)
                    {
                        List<Face> ListTempFace = new List<Face>();
                        ListTempFace = kvp.Value.sFaceGroupPnt[i].faceOccAry;
                        foreach (Face j in ListTempFace)
                        {
                            string TempFaceType = "";
                            string TempFacePt_X = "";
                            try
                            {
                                TempFaceType = j.GetStringAttribute("WE_TYPE");
                                TempFacePt_X = j.GetStringAttribute("WE_PNT_X");
                                if (TempFaceType == "" || TempFacePt_X == "NaN")
                                {
                                    check_isSetPnt = false;
                                    break;
                                }
                            }
                            catch (System.Exception ex)
                            {
                                check_isSetPnt = false;
                                break;
                            }
                        }
                        if (!check_isSetPnt)
                        {
                            break;
                        }
                    }

                    if (check_isSetPnt)
                    {
                        row = new GridRow(index, "已佈點", kvp.Key.compName, kvp.Key.section, kvp.Key.wkface, "手動佈點", "選擇治具");
                    }
                    else
                    {
                        row = new GridRow(index, "手動佈點", kvp.Key.compName, kvp.Key.section, kvp.Key.wkface, "手動佈點", "選擇治具");
                    }
                    superGridMainControl.PrimaryGrid.Rows.Add(row);
                    CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務，請進入手動佈點查看是否有可自動線割的區域");
                    continue;
                }
                #endregion

                #region 處理解析成功情況
                bool check = false;
                for (int i = 0; i < kvp.Value.sFaceGroupPnt.Count; i++)
                {
                    List<Face> ListTempFace = new List<Face>();
                    ListTempFace = kvp.Value.sFaceGroupPnt[i].faceOccAry;
                    foreach (Face j in ListTempFace)
                    {
                        string TempFaceType = "";
                        string TempFacePt_X = "";
                        try
                        {
                            TempFaceType = j.GetStringAttribute("WE_TYPE");
                            TempFacePt_X = j.GetStringAttribute("WE_PNT_X");
                            if (TempFaceType == "" || TempFacePt_X == "NaN")
                            {
                                row = new GridRow(index, "未佈點", kvp.Value.comp.Name, kvp.Key.section, kvp.Key.wkface, "手動佈點", "選擇治具");
                                superGridMainControl.PrimaryGrid.Rows.Add(row);
                                check = true;
                                break;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            row = new GridRow(index, "未佈點", kvp.Value.comp.Name, kvp.Key.section, kvp.Key.wkface, "手動佈點", "選擇治具");
                            superGridMainControl.PrimaryGrid.Rows.Add(row);
                            check = true;
                            break;
                        }
                    }
                    if (check)
                    {
                        break;
                    }
                }
                if (!check)
                {
                    row = new GridRow(index, "已佈點", kvp.Value.comp.Name, kvp.Key.section, kvp.Key.wkface, "手動佈點", "選擇治具");
                    superGridMainControl.PrimaryGrid.Rows.Add(row);
                }
                #endregion
              
            }

            #endregion

            #region 解析失敗
            foreach (KeyValuePair<skeyFailed, string> kvp in Program.FailedSection)
            {
                index++;
                row = new GridRow(index, "手動任務", kvp.Key.compName, kvp.Key.section, kvp.Key.wkface, "手動佈點", "選擇治具");
                superGridMainControl.PrimaryGrid.Rows.Add(row);
                superGridMainControl.PrimaryGrid.GetCell(index - 1, 5).ReadOnly = true;
                //CaxLog.ShowListingWindow(superGridMainControl.PrimaryGrid.GetCell(index-1, 5).Visible);
            }
            #endregion

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

            bool status;
            CaxLoadingDlg sCaxLoadingDlg1 = new CaxLoadingDlg();
//             sCaxLoadingDlg1.Run();
//             sCaxLoadingDlg1.SetLoadingText("數據計算中...");

            Session theSession = Session.GetSession();
            UFSession theUfSession = UFSession.GetUFSession();
            try
            {
                bool chk_Status = false;
                for (int i = 0; i < superGridMainControl.PrimaryGrid.Rows.Count; i++)
                {
                    string CurrentStatus = superGridMainControl.PrimaryGrid.GridPanel.GetCell(i, 1).Value.ToString();
                    if (CurrentStatus == "未佈點")
                    {
                        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "尚有零件未完成佈點，請進入手動佈點，檢視是否轉手動任務");
                        chk_Status = true;
                    }
                }

                //取得stock.dat中的備料尺寸大小，主要抓取長與寬的值塞入線割零件的長與寬
                string Folder_Stock = string.Format(@"{0}\{1}", Path.GetDirectoryName(theSession.Parts.Display.FullPath), "stock.dat");
                Mfg2MesJsonClass stockJson = new Mfg2MesJsonClass();
                status = GetStockData(Folder_Stock, out stockJson);
                double SPEC_Length = 0.0, SPEC_Width = 0.0, SPEC_Height = 0.0;
                if (stockJson.SPEC_LENGTH != "")
                {
                    SPEC_Length = Convert.ToDouble(stockJson.SPEC_LENGTH);
                }
                if (stockJson.SPEC_WIDE != "")
                {
                    SPEC_Width = Convert.ToDouble(stockJson.SPEC_WIDE);
                }
                if (stockJson.SPEC_HIGH != "")
                {
                    SPEC_Height = Convert.ToDouble(stockJson.SPEC_HIGH);
                }
                

                if (!chk_Status)
                {
                    //建立線割 JSON 類別
                    weExportData cweExportData = new weExportData();
                    cweExportData.EACHGROUPARRAY = new List<CimforceCaxTwMFG.EACHGROUPARRAY>();
                    int CurrentRow = -1;
                    bool check_isEDM = false;
                    
                    #region 成功CASE
                    foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WE_FACE_DIC)
                    {
                        CurrentRow++;
                        string IsManual = superGridMainControl.PrimaryGrid.GridPanel.GetCell(CurrentRow, 1).Value.ToString();
                        
                        //判斷是否有選治具，如果沒選擇不給提交
                        if (IsManual != "手動任務")
                        {
                            string IS_FIX = superGridMainControl.PrimaryGrid.GridPanel.GetCell(CurrentRow, 6).Value.ToString();
                            if (IS_FIX == "選擇治具")
                            {
                                string compName = superGridMainControl.PrimaryGrid.GridPanel.GetCell(CurrentRow, 2).Value.ToString();
                                string Msg = "零件" + compName + "為自動任務且尚未選取治具，是否確定提交?";
                                eTaskDialogResult chk_yes_no;
                                chk_yes_no = CaxMsg.ShowMsgYesNo(Msg);
                                if (chk_yes_no == eTaskDialogResult.No)
                                {
                                    return;
                                }
                                //UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "零件"+compName+"尚未選取治具");
                            }
                        }

                        EACHGROUPARRAY cEACHGROUPARRAY = new EACHGROUPARRAY();
                        cEACHGROUPARRAY.PROGRAM_ARRAY = new List<CimforceCaxTwMFG.PROGRAMARRAY>();

                        //取得電極號
                        string ELEC_NO = "";
                        try
                        {
                            ELEC_NO = kvp.Value.comp.GetStringAttribute("CIM_ELEC_NO");
                            //CaxLog.ShowListingWindow(ELEC_NO);
                        }
                        catch (System.Exception ex)
                        { }

                        //取得原始電極comp名字
                        string ELEC_NAME = "";
                        try
                        {
                            ELEC_NAME = kvp.Value.comp.GetStringAttribute("CIM_ELEC_NAME");
                        }
                        catch (System.Exception ex)
                        { }

                        #region 對原始電極與線割電極comp塞已做過線割的屬性
                        if (ELEC_NAME != "")
                        {
                            List<CaxAsm.CompPart> AsmCompAry;
                            CaxAsm.CimAsmCompPart sCimAsmCompPart;
                            CaxAsm.GetAllAsmCompStruct(out AsmCompAry, out sCimAsmCompPart);
                            if (sCimAsmCompPart.electorde.Count != 0)
                            {
                                foreach (CaxAsm.AsmCompPart eleComp in sCimAsmCompPart.electorde)
                                {
                                    try
                                    {
                                        eleComp.comp.GetStringAttribute("REFERENCE_COMPONENT");
                                        continue;
                                    }
                                    catch (System.Exception ex)
                                    { }
                                    if (ELEC_NAME == eleComp.comp.Name)
                                    {
                                        //CaxLog.ShowListingWindow("111");
                                        //CaxLog.ShowListingWindow(eleComp.comp.Name);
                                        kvp.Value.comp.SetAttribute("WE_FINISHED", "Y");
                                        eleComp.comp.SetAttribute("WE_FINISHED", "Y");
                                        //CaxLog.ShowListingWindow("222");
                                    }
                                }
                            }
                        }
                        #endregion


                        Body weCompBody;
                        CaxPart.GetLayerBody(kvp.Value.comp, out weCompBody);
                        Face[] weCompBodyFaceAry = weCompBody.GetFaces();

                        WorkPiece WP = new WorkPiece();
                        double[] WPmin = new double[3];
                        double[] WPmax = new double[3];
                        CaxPart.AskBoundingBoxExactByWCS(weCompBody.Tag, out WPmin, out WPmax);
                        WP.WP_Height = WPmax[2] - WPmin[2];


                        if (ELEC_NO == "")
                        {
                            //判斷備料的長寬高，哪一個與線割的高接近，如果接近，表示這個值是高，其他則再判斷哪個是長哪個是寬
                            GetWE_LWH(SPEC_Length, SPEC_Width, SPEC_Height, ref WP);
                        }
                        else
                        {
                            WP.WP_Length = WPmax[0] - WPmin[0];
                            WP.WP_Wide = WPmax[1] - WPmin[1];
                        }

                        

                        string outer_inner = "";
                        string reference_posi = "";

                        bool check_isZMachining = false;

                        //判斷此任務是電極或工件
                        foreach (Face tempFace in weCompBodyFaceAry)
                        {
                            string existBaseFace = "";
                            string isZMachining = "";
                            try
                            {
                                existBaseFace = tempFace.GetStringAttribute("ELECTRODE");
                                if (existBaseFace == "BASE_FACE")
                                {
                                    check_isEDM = true;
                                }
                                isZMachining = tempFace.GetStringAttribute("CIM_EDM_FACE");
                                if (isZMachining == "Z")
                                {
                                    check_isZMachining = true;
                                }
                            }
                            catch (System.Exception ex)
                            { continue; }
                        }

                        if (check_isEDM)
                        {
                            foreach (Face tempFace in weCompBodyFaceAry)
                            {
                                string ReferPosi = "";
                                try
                                {
                                    ReferPosi = tempFace.GetStringAttribute("reference_posi");
                                    reference_posi = ReferPosi;
                                    outer_inner = "2";
                                    cEACHGROUPARRAY.ELEC_NO = ELEC_NO;                //電極號
                                }
                                catch (System.Exception ex)
                                { continue; }
                            }
                        }
                        else
                        {
                            DecideOuterInner(kvp.Value.comp, WP, out outer_inner, out reference_posi);
                            if (reference_posi == "" || reference_posi == null)
                            {
                                reference_posi = "1";
                            }
                        }

                        int wecolor1 = 1;
                        foreach (FaceGroupPnt loopFaceGroupPnt in kvp.Value.sFaceGroupPnt)
                        {
                            wecolor1++; //從顏色2開始

                            //判斷是否為佈點失敗(failed)，如果有則跳過
                            
                            string WE_PNT_X = "";
                            WE_PNT_X = loopFaceGroupPnt.faceOccAry[0].GetStringAttribute("WE_PNT_X");
                            //CaxLog.ShowListingWindow(WE_PNT_X);
                            if (WE_PNT_X == "ESPRIT手動處理" || WE_PNT_X == "ITG" || WE_PNT_X == "DERROR")
                            {
                                continue;
                            }
                            //foreach (Face loopFaceOcc in loopFaceGroupPnt.faceOccAry)
                            //{
                            //    try
                            //    {
                            //        WE_PNT_X = loopFaceOcc.GetStringAttribute("WE_PNT_X");
                            //    }
                            //    catch (System.Exception ex)
                            //    { }
                            //}
                            //if (WE_PNT_X.Contains("failed"))
                            //{
                            //    continue;
                            //}

                            string IS_SHCS = "N";
                            string MACHING_TYPE = "";
                            string MACHING_THICKNESS = "";
                            string MACHING_ANGLE = "";
                            string WORKHEIGHT_XY = "";
                            string WORKHEIGHT_UV = "";
                            string UPPER_TOLERANCE = "";
                            string LOWER_TOLERANCE = "";
                            string MACHING_COUNT = "";
                            string COLOR_ID = "";

                            List<double> weFaceBoxAry = new List<double>();
                            List<double> maching_ang = new List<double>();
                            List<Face> slopeface1 = new List<Face>();
                            foreach (Face loopFaceOcc in loopFaceGroupPnt.faceOccAry)
                            {
                                string attr_value = "";

                                #region 判斷是否逃孔
                                try
                                {
                                    attr_value = loopFaceOcc.GetStringAttribute("FEATURE_TYPE");
                                    if (attr_value == IsCountersunk)
                                    {
                                        IS_SHCS = "Y";
                                    }
                                }
                                catch (System.Exception ex)
                                { }
                                #endregion

                                #region 判斷加工類型
                                attr_value = "";
                                try
                                {
                                    attr_value = loopFaceOcc.GetStringAttribute("WE_TYPE");
                                    attr_value = CreateWEPoint.WE_TYPE_ChineseTransNumber(ref attr_value);

                                    MACHING_TYPE = attr_value;
                                }
                                catch (System.Exception ex)
                                { }
                                #endregion

                                break;
                            }

                            #region 計算每組加工厚度 (目前只考慮T面狀況)
                            foreach (Face loopFaceOcc in loopFaceGroupPnt.faceOccAry)
                            {
                                double[] minWcs = new double[3];
                                double[] maxWcs = new double[3];
                                CaxPart.AskBoundingBoxExactByWCS(loopFaceOcc.Tag, out minWcs, out maxWcs);
                                weFaceBoxAry.Add(minWcs[2]);
                                weFaceBoxAry.Add(maxWcs[2]);
                            }
                            #endregion


                            //取得上下公差
                            bool chk_isMultiTol = false;
                            double Tol_Region = 0.0;
                            Dictionary<string, TolValue> TolColor_Top_Low = new Dictionary<string, TolValue>();
                            if (check_isEDM == false)
                            {
                                AskTolerance(loopFaceGroupPnt, out Tol_Region, out TolColor_Top_Low, out chk_isMultiTol, out UPPER_TOLERANCE, out LOWER_TOLERANCE);
                            }
                            else
                            {
                                AskTolerance(loopFaceGroupPnt, ref UPPER_TOLERANCE, ref LOWER_TOLERANCE);
                            }
                            //loopFaceGroupPnt.faceOccAry[0].Highlight();
                            //CaxLog.ShowListingWindow("up:" + UPPER_TOLERANCE);
                            //CaxLog.ShowListingWindow("low:" + LOWER_TOLERANCE);
                            if (UPPER_TOLERANCE == "")
                            {
                                loopFaceGroupPnt.faceOccAry[0].Highlight();
                            }

                            #region 加工刀次
                            
                            
                            //if (IsManual != "手動任務")
                            //{
                                if (check_isEDM == false)
                                {
                                    if (chk_isMultiTol == true)
                                    {
                                    select_Yes_No:
                                        string Msg = "偵測到同一組線割面有多公差\nYES：手動指定(選擇公差計算刀次)\nNO：自動匹配(以最大精度計算刀次)";
                                        eTaskDialogResult chk_yes_no;
                                        chk_yes_no = CaxMsg.ShowMsgYesNo(Msg);
                                        if (chk_yes_no == eTaskDialogResult.Yes)
                                        {
                                            this.Hide();
                                            //彈出對話框讓用戶選擇
                                            //Application.EnableVisualStyles();
                                            MultiTolDlg cMultiTolDlg = new MultiTolDlg(TolColor_Top_Low);
                                            cMultiTolDlg.ShowDialog();
                                            
                                            if (cMultiTolDlg.DialogResult == DialogResult.OK)
                                            {
                                                MACHING_COUNT = GetMachiningCount(Convert.ToDouble(MultiTolDlg.MultiTol.Tol_Region), MACHING_COUNT);

                                                UPPER_TOLERANCE = MultiTolDlg.MultiTol.Tol_Upper;
                                                LOWER_TOLERANCE = MultiTolDlg.MultiTol.Tol_Lower;
                                            }
                                            else
                                            {
                                                goto
                                                    select_Yes_No;
                                            }

                                            this.Show();
                                        }
                                        else
                                        {
                                            MACHING_COUNT = GetMachiningCount(Tol_Region, MACHING_COUNT);
                                        }
                                    }
                                    else
                                    {
                                        //單一公差，自動對應
                                        MACHING_COUNT = GetMachiningCount(Tol_Region, MACHING_COUNT);
                                    }
                                }
                                else
                                {
                                    double UpperTolerance = Math.Abs(Convert.ToDouble(UPPER_TOLERANCE));
                                    double LowerTolerance = Math.Abs(Convert.ToDouble(LOWER_TOLERANCE));
                                    double maxTolerance = Math.Max(UpperTolerance, LowerTolerance);
                                    if (maxTolerance > 0.05)
                                    {
                                        MACHING_COUNT = "1";
                                    }
                                    else if (maxTolerance > 0.02 && maxTolerance <= 0.05)
                                    {
                                        MACHING_COUNT = "2";
                                    }
                                    else if (maxTolerance > 0.01 && maxTolerance <= 0.02)
                                    {
                                        MACHING_COUNT = "3";
                                    }
                                    else if (maxTolerance <= 0.01)
                                    {
                                        MACHING_COUNT = "3";
                                    }
                                }
                            //}
                            
                            #endregion

                            #region 程式面XY、UV
                            GetSlopeFace(loopFaceGroupPnt, ref slopeface1);

                            List<double> bboxofface_ZMin1 = new List<double>();
                            List<double> bboxofface_ZMax1 = new List<double>();
                            double[] minWcs_XYUV1 = new double[3];
                            double[] maxWcs_XYUV1 = new double[3];
                            for (int i = 0; i < slopeface1.Count; i++)
                            {
                                CaxPart.AskBoundingBoxExactByWCS(slopeface1[i].Tag, out minWcs_XYUV1, out maxWcs_XYUV1);
                                //CaxLog.ShowListingWindow(Math.Abs(minWcs_XYUV1[2]).ToString());
                                //bboxofface_ZMin1.Add(Math.Abs(minWcs_XYUV1[2]));
                                //bboxofface_ZMax1.Add(Math.Abs(maxWcs_XYUV1[2]));
                                bboxofface_ZMin1.Add(minWcs_XYUV1[2]);
                                bboxofface_ZMax1.Add(maxWcs_XYUV1[2]);
                            }
                            bboxofface_ZMin1.Sort(); //算程式面XY
                            bboxofface_ZMax1.Sort(); //算程式面UV
                            double workheightXY1 = new double();
                            double workheightUV1 = new double();
                            if (bboxofface_ZMin1.Count != 0 && bboxofface_ZMax1.Count != 0)
                            {
                                if (check_isZMachining)
                                {
                                    workheightXY1 = bboxofface_ZMax1[bboxofface_ZMax1.Count - 1] / 2;
                                    workheightUV1 = bboxofface_ZMax1[bboxofface_ZMax1.Count - 1];
                                }
                                else
                                {
                                    workheightXY1 = bboxofface_ZMin1[bboxofface_ZMin1.Count - 1];
                                    workheightUV1 = bboxofface_ZMax1[0];
                                }
                            }
                            else
                            {
                                workheightXY1 = 0.0;
                                workheightUV1 = 0.0;
                            }
                            WORKHEIGHT_XY = Math.Round(workheightXY1, 4).ToString();
                            WORKHEIGHT_UV = Math.Round(workheightUV1, 4).ToString();
                            #endregion


                            #region 取得加工厚度
                            //取得加工厚度
                            weFaceBoxAry.Sort();
                            double thickness = weFaceBoxAry[weFaceBoxAry.Count - 1] - weFaceBoxAry[0];
                            askThicknessRange(thickness, out MACHING_THICKNESS);
                            #endregion


                            #region 計算每組加工角度
                            foreach (Face loopFaceOcc in loopFaceGroupPnt.faceOccAry)
                            {
                                CFace ff = new CFace();
                                double[] ff_nor = new double[3];
                                ff_nor = ff.GetNormal(loopFaceOcc.Tag);
                                double temp_ang = (ff_nor[0] * 0 + ff_nor[1] * 0 + ff_nor[2] * 1) /
                                                    (Math.Sqrt((ff_nor[0] * ff_nor[0]) + (ff_nor[1] * ff_nor[1]) + (ff_nor[2] * ff_nor[2])));
                                double ang = Math.Acos(temp_ang) * 180 / Math.PI;
                                double real_ang = Math.Abs(90 - ang);
                                maching_ang.Add(real_ang);
                            }
                            #endregion


                            #region 取得加工角度
                            //取得加工角度
                            if (maching_ang.Count != 0)
                            {
                                maching_ang.Sort();
                                double machingAng = maching_ang[maching_ang.Count - 1];
                                askMachingAngleRange(machingAng, out MACHING_ANGLE);
                            }
                            #endregion


                            //取得線割顏色代碼
                            askEspritColor(wecolor1, out COLOR_ID);
                            foreach (Face loopFaceOcc in loopFaceGroupPnt.faceOccAry)
                            {
                                loopFaceOcc.SetAttribute("COLOR_ID", wecolor1.ToString());
                                //Face loopFaceOcc_Prototype = (Face)loopFaceOcc.Prototype;
                                //loopFaceOcc_Prototype.SetAttribute("COLOR_ID", wecolor1.ToString());
                                //theUfSession.Obj.SetColor(loopFaceOcc_Prototype.Tag, wecolor1);
                            }

                            //CaxLog.ShowListingWindow("8");



                            #region 判斷 PROGRAMARRAY 數據是否為空

                            //-----下面TEST
                            PROGRAMARRAY cPROGRAMARRAY = new PROGRAMARRAY();
                            cPROGRAMARRAY.FACEGROUPMEMBERS = new List<NXOpen.Tag>();
                            cPROGRAMARRAY.MACHING_TYPE = MACHING_TYPE;                                      //加工類型
                            cPROGRAMARRAY.COLOR_ID = COLOR_ID;                                            //顏色代碼
                            cPROGRAMARRAY.IS_SHCS = IS_SHCS;                                                //是否逃孔
                            cPROGRAMARRAY.MACHING_THICKNESS = MACHING_THICKNESS;                            //加工厚度
                            cPROGRAMARRAY.MACHING_COUNT = MACHING_COUNT;                                       //加工刀次
                            cPROGRAMARRAY.MACHING_ANGLE = MACHING_ANGLE;                                    //加工角度
                            cPROGRAMARRAY.UPPER_TOLERANCE = UPPER_TOLERANCE;                                     //上公差
                            cPROGRAMARRAY.LOWER_TOLERANCE = LOWER_TOLERANCE;                                     //下公差
                            cPROGRAMARRAY.WORKHEIGHT_XY = WORKHEIGHT_XY;                                       //程式面XY
                            cPROGRAMARRAY.WORKHEIGHT_UV = WORKHEIGHT_UV;                                       //程式面UV

                            foreach (Face loopFaceOcc in loopFaceGroupPnt.faceOccAry)
                            {
                                string pnt_x = loopFaceOcc.GetStringAttribute("WE_PNT_X");
                                string pnt_y = loopFaceOcc.GetStringAttribute("WE_PNT_Y");
                                cPROGRAMARRAY.MOVEPOSI_X = pnt_x;               //X跑位值
                                cPROGRAMARRAY.MOVEPOSI_Y = pnt_y;               //Y跑位值
                                break;
                            }

                            cEACHGROUPARRAY.PROGRAM_ARRAY.Add(cPROGRAMARRAY);
                            //-----上面TEST

                            if (IsManual != "手動任務")
                            {
                                //PROGRAMARRAY cPROGRAMARRAY = new PROGRAMARRAY();
                                //cPROGRAMARRAY.FACEGROUPMEMBERS = new List<NXOpen.Tag>();
                                //cPROGRAMARRAY.MACHING_TYPE = MACHING_TYPE;                                      //加工類型
                                //cPROGRAMARRAY.COLOR_ID = COLOR_ID;                                            //顏色代碼
                                //cPROGRAMARRAY.IS_SHCS = IS_SHCS;                                                //是否逃孔
                                //cPROGRAMARRAY.MACHING_THICKNESS = MACHING_THICKNESS;                            //加工厚度
                                //cPROGRAMARRAY.MACHING_COUNT = MACHING_COUNT;                                       //加工刀次
                                //cPROGRAMARRAY.MACHING_ANGLE = MACHING_ANGLE;                                    //加工角度
                                //cPROGRAMARRAY.UPPER_TOLERANCE = UPPER_TOLERANCE;                                     //上公差
                                //cPROGRAMARRAY.LOWER_TOLERANCE = LOWER_TOLERANCE;                                     //下公差
                                //cPROGRAMARRAY.WORKHEIGHT_XY = WORKHEIGHT_XY;                                       //程式面XY
                                //cPROGRAMARRAY.WORKHEIGHT_UV = WORKHEIGHT_UV;                                       //程式面UV

                                //foreach (Face loopFaceOcc in loopFaceGroupPnt.faceOccAry)
                                //{
                                //    string pnt_x = loopFaceOcc.GetStringAttribute("WE_PNT_X");
                                //    string pnt_y = loopFaceOcc.GetStringAttribute("WE_PNT_Y");
                                //    cPROGRAMARRAY.MOVEPOSI_X = pnt_x;               //X跑位值
                                //    cPROGRAMARRAY.MOVEPOSI_Y = pnt_y;               //Y跑位值
                                //    break;
                                //}

                                //cEACHGROUPARRAY.PROGRAM_ARRAY.Add(cPROGRAMARRAY);

                                //if (cPROGRAMARRAY.MACHING_TYPE == "")
                                //{
                                //    CaxLog.ShowListingWindow("加工類型 取得失敗...");
                                //    return;
                                //}
                                //if (cPROGRAMARRAY.COLOR_ID == "")
                                //{
                                //    CaxLog.ShowListingWindow("顏色代碼 取得失敗...");
                                //    return;
                                //}
                                //if (cPROGRAMARRAY.IS_SHCS == "")
                                //{
                                //    CaxLog.ShowListingWindow("是否逃孔 取得失敗...");
                                //    return;
                                //}
                                //if (cPROGRAMARRAY.MACHING_THICKNESS == "")
                                //{
                                //    CaxLog.ShowListingWindow("加工厚度 取得失敗...");
                                //    return;
                                //}
                                //if (cPROGRAMARRAY.MACHING_COUNT == "")
                                //{
                                //    CaxLog.ShowListingWindow("加工刀次 取得失敗...");
                                //    return;
                                //}
                                //if (cPROGRAMARRAY.MACHING_ANGLE == "")
                                //{
                                //    CaxLog.ShowListingWindow("加工角度 取得失敗...");
                                //    return;
                                //}
                                //if (cPROGRAMARRAY.MOVEPOSI_X == "")
                                //{
                                //    CaxLog.ShowListingWindow("X跑位值 取得失敗...");
                                //    return;
                                //}
                                //if (cPROGRAMARRAY.MOVEPOSI_Y == "")
                                //{
                                //    CaxLog.ShowListingWindow("Y跑位值 取得失敗...");
                                //    return;
                                //}
                                //if (cPROGRAMARRAY.UPPER_TOLERANCE == "")
                                //{
                                //    CaxLog.ShowListingWindow("上公差 取得失敗...");
                                //    return;
                                //}
                                //if (cPROGRAMARRAY.LOWER_TOLERANCE == "")
                                //{
                                //    CaxLog.ShowListingWindow("下公差 取得失敗...");
                                //    return;
                                //}
                                //if (cPROGRAMARRAY.WORKHEIGHT_XY == "")
                                //{
                                //    CaxLog.ShowListingWindow("程式面XY 取得失敗...");
                                //    return;
                                //}
                                //if (cPROGRAMARRAY.WORKHEIGHT_UV == "")
                                //{
                                //    CaxLog.ShowListingWindow("程式面UV 取得失敗...");
                                //    return;
                                //}
                            }
                            else//這邊是手動任務
                            {
                                //PROGRAMARRAY cPROGRAMARRAY = new PROGRAMARRAY();
                                //cPROGRAMARRAY.FACEGROUPMEMBERS = new List<NXOpen.Tag>();
                                //cPROGRAMARRAY.MACHING_TYPE = MACHING_TYPE;                                      //加工類型
                                //cPROGRAMARRAY.COLOR_ID = COLOR_ID;                                            //顏色代碼
                                //cPROGRAMARRAY.IS_SHCS = IS_SHCS;                                                //是否逃孔
                                //cPROGRAMARRAY.MACHING_THICKNESS = MACHING_THICKNESS;                            //加工厚度
                                //cPROGRAMARRAY.MACHING_COUNT = MACHING_COUNT;                                       //加工刀次
                                //cPROGRAMARRAY.MACHING_ANGLE = MACHING_ANGLE;                                    //加工角度
                                //cPROGRAMARRAY.UPPER_TOLERANCE = UPPER_TOLERANCE;                                     //上公差
                                //cPROGRAMARRAY.LOWER_TOLERANCE = LOWER_TOLERANCE;                                     //下公差
                                //cPROGRAMARRAY.WORKHEIGHT_XY = WORKHEIGHT_XY;                                       //程式面XY
                                //cPROGRAMARRAY.WORKHEIGHT_UV = WORKHEIGHT_UV;                                       //程式面UV

                                ////CaxLog.ShowListingWindow("9");
                                ///*
                                //foreach (Face loopFaceOcc in loopFaceGroupPnt.faceOccAry)
                                //{
                                //    cPROGRAMARRAY.MOVEPOSI_X = "0";               //X跑位值
                                //    cPROGRAMARRAY.MOVEPOSI_Y = "0";               //Y跑位值
                                //    break;
                                //}
                                //cEACHGROUPARRAY.PROGRAM_ARRAY.Add(cPROGRAMARRAY);
                                //*/
                                
                                //string pnt_x = "";
                                //string pnt_y = "";
                                //foreach (Face loopFaceOcc in loopFaceGroupPnt.faceOccAry)
                                //{
                                //    pnt_x = loopFaceOcc.GetStringAttribute("WE_PNT_X");
                                //    pnt_y = loopFaceOcc.GetStringAttribute("WE_PNT_Y");
                                //    break;
                                //}

                                //if (pnt_x.Contains("failed") || pnt_y.Contains("failed"))
                                //{
                                //    continue;
                                //}
                                //else
                                //{
                                //    cPROGRAMARRAY.MOVEPOSI_X = pnt_x;               //X跑位值
                                //    cPROGRAMARRAY.MOVEPOSI_Y = pnt_y;               //Y跑位值
                                //}
                                //cEACHGROUPARRAY.PROGRAM_ARRAY.Add(cPROGRAMARRAY);
                            }

                            #endregion
                        }
                        

                        if (IsManual == "手動任務")
                        {
                            if (check_isEDM)
                            {
                                CaxAsm.SetWorkComponent(kvp.Value.comp);
                                cEACHGROUPARRAY.WE_TRUE_FALSE = "FALSE";
                                try
                                {
                                    if (kvp.Value.isAnalyzeSucess == -2)
                                    {
                                        CaxPart.DeleteNXObject(CaxWE.ListCopyBody[CurrentRow]);
                                        Part weCompPart = (Part)kvp.Value.comp.Prototype;
                                        status = SetPartColor(weCompPart, kvp.Key.section);
                                        if (!status)
                                        {
                                            CaxLog.ShowListingWindow("SetPartColor失敗");
                                        }
                                        //CaxLog.ShowListingWindow("111");
                                    }
                                    else
                                    {
                                        CaxPart.DeleteNXObject(CaxWE.ListOriginalBody[CurrentRow]);
                                        Part weCompPart = (Part)kvp.Value.comp.Prototype;
                                        status = SetPartColor(weCompPart, kvp.Key.section, 10);
                                        if (!status)
                                        {
                                            CaxLog.ShowListingWindow("SetPartColor失敗");
                                        }
                                        //CaxLog.ShowListingWindow("222");
                                    }
                                    
                                }
                                catch (System.Exception ex)
                                {
                                    CaxLog.ShowListingWindow(ex.ToString());
                                }

                                //Part weCompPart = (Part)kvp.Value.comp.Prototype;

                                //status = SetPartColor(weCompPart, kvp.Key.section);
                                //if (!status)
                                //{
                                //    CaxLog.ShowListingWindow("SetPartColor失敗");
                                //}
                            }
                            else
                            {
                                //CaxLog.ShowListingWindow("000");
                                CaxAsm.SetWorkComponent(kvp.Value.comp);
                                cEACHGROUPARRAY.WE_TRUE_FALSE = "FALSE";
                                try
                                {
                                    if (kvp.Value.isAnalyzeSucess == -2)
                                    {
                                        //test
                                        //Body bodyocc;
                                        //CaxPart.GetLayerBody((Part)kvp.Value.comp.Prototype, out bodyocc);
                                        //CaxWE.RemoveParameters(bodyocc);
                                        CaxPart.DeleteNXObject(CaxWE.ListCopyBody[CurrentRow]);
                                        Part weCompPart = (Part)kvp.Value.comp.Prototype;
                                        status = SetPartColor(weCompPart, kvp.Key.section);
                                        if (!status)
                                        {
                                            CaxLog.ShowListingWindow("SetPartColor失敗");
                                        }

                                        //CaxPart.DeleteNXObject(CaxWE.ListOriginalBody[CurrentRow]);
                                    }
                                    else
                                    {
                                        //CaxLog.ShowListingWindow("222");
                                        CaxPart.DeleteNXObject(CaxWE.ListOriginalBody[CurrentRow]);
                                        Part weCompPart = (Part)kvp.Value.comp.Prototype;
                                        status = SetPartColor(weCompPart, kvp.Key.section, 10);
                                        if (!status)
                                        {
                                            CaxLog.ShowListingWindow("SetPartColor失敗");
                                        }
                                    }
                                    //CaxPart.DeleteNXObject(CaxWE.ListOriginalBody[CurrentRow]);
                                }
                                catch (System.Exception ex)
                                {
                                    CaxLog.ShowListingWindow(ex.ToString());
                                }

                                
                                //Part weCompPart = (Part)kvp.Value.comp.Prototype;
                                //status = SetPartColor(weCompPart, kvp.Key.section);
                                //if (!status)
                                //{
                                //    CaxLog.ShowListingWindow("SetPartColor失敗");
                                //}

                            }
                        }
                        else
                        {
                            if (check_isEDM)
                            {
                                CaxAsm.SetWorkComponent(kvp.Value.comp);
                                cEACHGROUPARRAY.WE_TRUE_FALSE = "TRUE";
                                try
                                {
                                    CaxPart.DeleteNXObject(CaxWE.ListCopyBody[CurrentRow]);
                                }
                                catch (System.Exception ex)
                                {
                                    CaxLog.ShowListingWindow(ex.ToString());
                                }
                            }
                            else
                            {
                                CaxAsm.SetWorkComponent(kvp.Value.comp);
                                cEACHGROUPARRAY.WE_TRUE_FALSE = "TRUE";
                                try
                                {
                                    CaxPart.DeleteNXObject(CaxWE.ListCopyBody[CurrentRow]);
                                }
                                catch (System.Exception ex)
                                {
                                    CaxLog.ShowListingWindow(ex.ToString());
                                }
                            }
                        }

                        cEACHGROUPARRAY.WP_PHOTO = kvp.Value.comp.Name + ".jpg";        //工件圖
                        cEACHGROUPARRAY.REFERENCE_POSITION = reference_posi;            //基準角方位
                        cEACHGROUPARRAY.WORKPIECE_LENGTH = WP.WP_Length.ToString("f4"); //工件長
                        cEACHGROUPARRAY.WORKPIECE_WIDTH = WP.WP_Wide.ToString("f4"); //工件寬

                        cEACHGROUPARRAY.WORKPIECE_HEIGHT = WP.WP_Height.ToString("f4"); //工件高
                        cEACHGROUPARRAY.SECTION_ID = kvp.Key.section;                   //工段
                        cEACHGROUPARRAY.OUTER_INNER = outer_inner;                      //線割零件檔
                        cEACHGROUPARRAY.WE_PART = kvp.Value.comp.Name + ".prt";         //線割零件檔
                        if (kvp.Value.WE_FIX == null)
                        {
                            cEACHGROUPARRAY.WE_FIX = "";
                        }
                        else
                        {
                            cEACHGROUPARRAY.WE_FIX = kvp.Value.WE_FIX;                    //廠商_TYPE_治具名稱
                        }

                        if (kvp.Value.WE_FIX_PATH == null)
                        {
                            cEACHGROUPARRAY.WE_FIX_PATH = "";
                        }
                        else
                        {
                            cEACHGROUPARRAY.WE_FIX_PATH = kvp.Value.WE_FIX_PATH; //治具圖檔路徑
                        }

                        if (kvp.Value.IS_FIX == null)
                        {
                            cEACHGROUPARRAY.IS_FIX = "N";
                        }
                        else
                        {
                            cEACHGROUPARRAY.IS_FIX = kvp.Value.IS_FIX; //是否已組裝治具
                        }
                        //cEACHGROUPARRAY.WE_STL = kvp.Value.comp.Name + ".stl";         //線割STL檔
                        cweExportData.EACHGROUPARRAY.Add(cEACHGROUPARRAY);

                        #region 判斷 EACHGROUPARRAY 數據是否為空

                        if (cEACHGROUPARRAY.WP_PHOTO == "")
                        {
                            CaxLog.ShowListingWindow("工件圖 取得失敗...");
                            return;
                        }
                        if (cEACHGROUPARRAY.REFERENCE_POSITION == "")
                        {
                            CaxLog.ShowListingWindow("基準角方位 取得失敗...");
                            return;
                        }
                        if (cEACHGROUPARRAY.WORKPIECE_HEIGHT == "")
                        {
                            CaxLog.ShowListingWindow("工件高 取得失敗...");
                            return;
                        }
                        if (cEACHGROUPARRAY.SECTION_ID == "")
                        {
                            CaxLog.ShowListingWindow("工段 取得失敗...");
                            return;
                        }
                        if (cEACHGROUPARRAY.OUTER_INNER == "")
                        {
                            CaxLog.ShowListingWindow("線割零件檔 取得失敗...");
                            return;
                        }
                        if (cEACHGROUPARRAY.WE_PART == "")
                        {
                            CaxLog.ShowListingWindow("線割零件檔 取得失敗...");
                            return;
                        }
                        //                     if (cEACHGROUPARRAY.WE_FIX == "")
                        //                     {
                        //                         CaxLog.ShowListingWindow("治具 取得失敗...");
                        //                         return;
                        //                     }

                        #endregion
                        

                    }
                    #endregion

                    #region 失敗CASE(只記錄自動解析失敗，不包含人工轉手動任務的CASE)
                    foreach (KeyValuePair<skeyFailed, string> kvp in Program.FailedSection)
                    {
                        Part weCompPart = (Part)kvp.Key.comp.Prototype;
                        //CaxLog.ShowListingWindow("756 weCompPart:" + weCompPart);
                        //將零件全上1的顏色
                        status = SetPartColor(weCompPart, kvp.Key.section);
                        if (!status)
                        {
                            CaxLog.ShowListingWindow("759 SetPartColor失敗");
                        }
                        //產生線割檔名
                        string WE_PRT_NAME = "";
                        WE_PRT_NAME = string.Format(@"{0}_{1}", kvp.Key.section, kvp.Key.wkface);

                        //新建線割檔
                        string NewCompFullPath = Path.GetDirectoryName(weCompPart.FullPath) + @"\" + kvp.Key.comp.Name + "_" + WE_PRT_NAME + ".prt";

                        //CaxLog.ShowListingWindow("NewCompFullPath:" + NewCompFullPath);
                        Body weCompBody;
                        Body[] weCompBodyAry = weCompPart.Bodies.ToArray();
                        weCompBody = weCompBodyAry[0];
                        //CaxPart.GetLayerBody(kvp.Key.comp, out weCompBody);
                        WorkPiece WP = new WorkPiece();
                        double[] WPmin = new double[3];
                        double[] WPmax = new double[3];
                        CaxPart.AskBoundingBoxExactByWCS(weCompBody.Tag, out WPmin, out WPmax);
                        WP.WP_Height = WPmax[2] - WPmin[2];

                        GetWE_LWH(SPEC_Length, SPEC_Width, SPEC_Height, ref WP);
                        //CaxLog.ShowListingWindow("WP.WP_Height:" + WP.WP_Height);


                        EACHGROUPARRAY cEACHGROUPARRAY = new EACHGROUPARRAY();
                        cEACHGROUPARRAY.PROGRAM_ARRAY = new List<CimforceCaxTwMFG.PROGRAMARRAY>();

                        cEACHGROUPARRAY.SECTION_ID = kvp.Key.section;
                        cEACHGROUPARRAY.WE_TRUE_FALSE = "FALSE";
                        cEACHGROUPARRAY.WE_PART = kvp.Key.comp.Name + ".prt";
                        cEACHGROUPARRAY.REFERENCE_POSITION = CaxWE.ReferencePosi_False;
                        cEACHGROUPARRAY.WORKPIECE_HEIGHT = WP.WP_Height.ToString("f4"); //工件高
                        cEACHGROUPARRAY.WORKPIECE_LENGTH = WP.WP_Length.ToString("f4"); //工件長
                        cEACHGROUPARRAY.WORKPIECE_WIDTH = WP.WP_Wide.ToString("f4"); //工件寬

                        PROGRAMARRAY cPROGRAMARRAY = new PROGRAMARRAY();
                        cPROGRAMARRAY.FACEGROUPMEMBERS = new List<NXOpen.Tag>();
                        cPROGRAMARRAY.MACHING_TYPE = "0";                                      //加工類型
                        cPROGRAMARRAY.COLOR_ID = "0";                                            //顏色代碼
                        cPROGRAMARRAY.IS_SHCS = "0";                                                //是否逃孔
                        cPROGRAMARRAY.MACHING_THICKNESS = "0";                            //加工厚度
                        cPROGRAMARRAY.MACHING_COUNT = "0";                                       //加工刀次
                        cPROGRAMARRAY.MACHING_ANGLE = "0";                                    //加工角度
                        cPROGRAMARRAY.UPPER_TOLERANCE = "0";                                     //上公差
                        cPROGRAMARRAY.LOWER_TOLERANCE = "0";                                     //下公差
                        cPROGRAMARRAY.WORKHEIGHT_XY = "0";                                       //程式面XY
                        cPROGRAMARRAY.WORKHEIGHT_UV = "0";                                       //程式面UV
                        cPROGRAMARRAY.MOVEPOSI_X = "0";               //X跑位值
                        cPROGRAMARRAY.MOVEPOSI_Y = "0";               //Y跑位值

                        cEACHGROUPARRAY.PROGRAM_ARRAY.Add(cPROGRAMARRAY);


                        cweExportData.EACHGROUPARRAY.Add(cEACHGROUPARRAY);
                    }
                    #endregion

                    //輸出ugwe2mes.dat
                    string ugwe2mesJsonPath = string.Format(@"{0}\{1}", Path.GetDirectoryName(theSession.Parts.Display.FullPath), "ugwe2mes.dat");
                    if (check_isEDM == true)
                    {
                        if (File.Exists(ugwe2mesJsonPath))
                        {
                            weExportData olddata = new weExportData();

                            status = Getugwe2mesData(ugwe2mesJsonPath, out olddata);
                            if (!status)
                            {
                                CaxLog.ShowListingWindow("取得ugwe2mes失敗");
                            }

                            List<CaxAsm.CompPart> ListComp = new List<CaxAsm.CompPart>();
                            CaxAsm.GetAllAsmCompTree(out ListComp);
                            
                            foreach (CaxAsm.CompPart tempComp in ListComp)
                            {
                                foreach (EACHGROUPARRAY temp in olddata.EACHGROUPARRAY)
                                {
                                    //if (tempComp.componentOcc.Name.ToUpper().IndexOf("_WEDMS") > 0 && tempComp.componentOcc.Name.ToUpper().IndexOf(temp.ELEC_NO) > 0)
                                    if (tempComp.componentOcc.Name.ToUpper().IndexOf("_"+WE_SECTION) > 0 &&
                                        tempComp.componentOcc.Name.ToUpper().IndexOf(temp.ELEC_NO) > 0)
                                    {
                                        cweExportData.EACHGROUPARRAY.Add(temp);
                                    }
                                }
                            }
                        }
                    }
                    status = CaxFile.WriteJsonFileData(ugwe2mesJsonPath, cweExportData);
                    if (!status)
                    {
                        CaxLog.ShowListingWindow("ugwe2mes.dat 輸出失敗...");
                        return;
                    }

                    CaxPart.Save();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                //sCaxLoadingDlg1.Stop();

            }
            catch (System.Exception ex)
            {
                
                CaxLog.ShowListingWindow(ex.ToString());
                CaxLog.ShowListingWindow("SelectWorkPart錯誤");
                return;
            }
            sCaxLoadingDlg1.Stop();
        }

        private string GetMachiningCount(double Tol_Region, string MACHING_COUNT)
        {
            if (Tol_Region > 0.05)
            {
                MACHING_COUNT = "1";
            }
            else if (Tol_Region > 0.02 && Tol_Region <= 0.05)
            {
                MACHING_COUNT = "2";
            }
            else if (Tol_Region > 0.01 && Tol_Region <= 0.02)
            {
                MACHING_COUNT = "3";
            }
            else if (Tol_Region <= 0.01)
            {
                MACHING_COUNT = "3";
            } 
            return MACHING_COUNT;
        }

        private void GetWE_LWH(double SPEC_Length, double SPEC_Width, double SPEC_Height, ref WorkPiece WP)
        {
            if (Math.Abs(SPEC_Length - WP.WP_Height) < 1 && Math.Abs(SPEC_Length - WP.WP_Height) >= 0)
            {
                if (SPEC_Width > SPEC_Height)
                {
                    WP.WP_Length = SPEC_Width;
                    WP.WP_Wide = SPEC_Height;
                }
                else
                {
                    WP.WP_Length = SPEC_Height;
                    WP.WP_Wide = SPEC_Width;
                }
            }
            else if (Math.Abs(SPEC_Width - WP.WP_Height) < 1 && Math.Abs(SPEC_Width - WP.WP_Height) >= 0)
            {
                if (SPEC_Length > SPEC_Height)
                {
                    WP.WP_Length = SPEC_Length;
                    WP.WP_Wide = SPEC_Height;
                }
                else
                {
                    WP.WP_Length = SPEC_Height;
                    WP.WP_Wide = SPEC_Length;
                }
            }
            else if (Math.Abs(SPEC_Height - WP.WP_Height) < 1 && Math.Abs(SPEC_Height - WP.WP_Height) >= 0)
            {
                if (SPEC_Width > SPEC_Length)
                {
                    WP.WP_Length = SPEC_Width;
                    WP.WP_Wide = SPEC_Length;
                }
                else
                {
                    WP.WP_Length = SPEC_Length;
                    WP.WP_Wide = SPEC_Width;
                }
            }
        }

        private bool SetPartColor(Part weCompPart, string SectionID, int layer = 1)
        {
            Session theSession = Session.GetSession();
            UFSession theUfSession = UFSession.GetUFSession();
            try
            {
                Body PrototypeBody;
                CaxPart.GetLayerBody(weCompPart, out PrototypeBody, layer);
                Face[] PrototypeBodyFaceAry = PrototypeBody.GetFaces();

                string ColorAtt = "";
                foreach (Face prototypeFace in PrototypeBodyFaceAry)
                {
                    try
                    {
                        ColorAtt = prototypeFace.GetStringAttribute("MFG_COLOR");
                        string WE_PNT_X = "";
                        try
                        {
                            WE_PNT_X = prototypeFace.GetStringAttribute("WE_PNT_X");
                        }
                        catch (System.Exception ex)
                        {
                        }

                        if (WE_PNT_X.Contains("failed") || WE_PNT_X.Contains("DERROR") || WE_PNT_X.Contains("ITG") || WE_PNT_X.Contains("ESPRIT手動處理"))
                        {
                            if (SectionID == "WECAM" && ColorAtt == "105")
                            {
                                theUfSession.Obj.SetColor(prototypeFace.Tag, Convert.ToInt32(ColorAtt));
                            }
                            //else if ((SectionID == "WEDMS1" || SectionID == "WEDMS2") && (ColorAtt == "213" || ColorAtt == "214" || ColorAtt == "215"))
                            else if ((SectionID == WE_SECTION + "1" || SectionID == WE_SECTION + "2") && (ColorAtt == "213" || ColorAtt == "214" || ColorAtt == "215"))
                            {
                                theUfSession.Obj.SetColor(prototypeFace.Tag, Convert.ToInt32(ColorAtt));
                            }
                            else
                            {
                                theUfSession.Obj.SetColor(prototypeFace.Tag, 1);
                            }
                        }
                        else
                        {
                            if (SectionID == "WECAM" && ColorAtt == "105")
                            {
                                theUfSession.Obj.SetColor(prototypeFace.Tag, Convert.ToInt32(ColorAtt));
                            }
                            else if ((SectionID == WE_SECTION + "1" || SectionID == WE_SECTION + "2") && (ColorAtt == "213" || ColorAtt == "214" || ColorAtt == "215"))
                            {
                                theUfSession.Obj.SetColor(prototypeFace.Tag, Convert.ToInt32(ColorAtt));
                            }
                            else
                            {
                                theUfSession.Obj.SetColor(prototypeFace.Tag, 1);
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        theUfSession.Obj.SetColor(prototypeFace.Tag, 1);
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;

        }

        private void GetSlopeFace(FaceGroupPnt loopFaceGroupPnt, ref List<Face> slopeface1)
        {
            foreach (Face loopFaceOcc in loopFaceGroupPnt.faceOccAry)
            {
                //loopFaceOcc.Highlight();
                string faceMachingType = "";
                try
                {
                    faceMachingType = loopFaceOcc.GetStringAttribute("WE_TYPE");
                    if (faceMachingType == "4" || faceMachingType == "5" || faceMachingType == "6" || faceMachingType == "8")
                    {
                        slopeface1.Add(loopFaceOcc);
                    }
                }
                catch (System.Exception ex)
                { }
            }
        }

        private void AskTolerance(FaceGroupPnt loopFaceGroupPnt, out double Tol_Region, out Dictionary<string, TolValue> TolColor_Top_Low,
            out bool chk_isMultiTol, out string UpperTol, out string LowerTol)
        {
            UpperTol = "";
            LowerTol = "";
            Tol_Region = 0.0;
            TolColor_Top_Low = new Dictionary<string, TolValue>();
            List<double> ListTolValue = new List<double>();

            #region 輸出Dictionary TolColor_Top_Low
            foreach (Face loopFaceOcc in loopFaceGroupPnt.faceOccAry)
            {
                Face loopFaceProto = (Face)loopFaceOcc.Prototype;
                string tol_color = "";
                try
                {
                    if (loopFaceProto == null)
                    {
                        tol_color = loopFaceOcc.GetStringAttribute("TOL_COLOR");
                    }
                    else
                    {
                        tol_color = loopFaceProto.GetStringAttribute("TOL_COLOR");
                    }
                }
                catch (System.Exception ex)
                {
                    continue;
                }

                bool chk_key;
                string tolStr = "";
                chk_key = mdColorDic.TryGetValue(Convert.ToInt32(tol_color), out tolStr);
                if (chk_key)
                {
                    string tolcolor = "";
                    chk_key = TolColor_Top_Low.ContainsKey(tol_color);
                    if (!chk_key)
                    {
                        TolValue sTolValue = new TolValue();
                        sTolValue.Tol_Upper = tolStr.Split('_')[0];
                        sTolValue.Tol_Lower = tolStr.Split('_')[1];
                        sTolValue.Tol_Region = Math.Abs(Convert.ToDouble(sTolValue.Tol_Upper) - Convert.ToDouble(sTolValue.Tol_Lower)).ToString();
                        TolColor_Top_Low.Add(tol_color, sTolValue);
                    }

                    //string[] tolAry = tolStr.Split('_');
                    //if (tolAry.Length >= 2)
                    //{
                    //    UPPER_TOLERANCE = tolAry[0];
                    //    LOWER_TOLERANCE = tolAry[1];
                    //    break;
                    //}
                }
            }
            #endregion

            
            if (TolColor_Top_Low.Count>1)
            {
                chk_isMultiTol = true;
                foreach (KeyValuePair<string, TolValue> kvp in TolColor_Top_Low)
                {
                    Tol_Region = Math.Abs(Convert.ToDouble(kvp.Value.Tol_Upper) - Convert.ToDouble(kvp.Value.Tol_Lower));
                    ListTolValue.Add(Tol_Region);
                }
                ListTolValue.Sort();

                //取上下公差範圍最小的值(精度最大)
                Tol_Region = ListTolValue[0];

                //取得此範圍的上下公差
                foreach (KeyValuePair<string, TolValue> kvp in TolColor_Top_Low)
                {
                    if (Tol_Region.ToString() == kvp.Value.Tol_Region)
                    {
                        UpperTol = kvp.Value.Tol_Upper;
                        LowerTol = kvp.Value.Tol_Lower;
                        break;
                    }
                }
            }
            else
            {
                chk_isMultiTol = false;
                foreach (KeyValuePair<string, TolValue> kvp in TolColor_Top_Low)
                {
                    Tol_Region = Math.Abs(Convert.ToDouble(kvp.Value.Tol_Upper) - Convert.ToDouble(kvp.Value.Tol_Lower));
                    UpperTol = kvp.Value.Tol_Upper;
                    LowerTol = kvp.Value.Tol_Lower;
                }
            }
        }

        private void AskTolerance(FaceGroupPnt loopFaceGroupPnt, ref string UPPER_TOLERANCE, ref string LOWER_TOLERANCE)
        {
            //             double UpTol, LowTol;
            //             UPPER_TOLERANCE = "-999";
            //             LOWER_TOLERANCE = "999";
            List<double> ListTolValue = new List<double>();
            foreach (Face loopFaceOcc in loopFaceGroupPnt.faceOccAry)
            {
                Face ssface_proto = (Face)loopFaceOcc.Prototype;
                string tol_color = "";
                try
                {
                    if (ssface_proto == null)
                    {
                        tol_color = loopFaceOcc.GetStringAttribute("TOL_COLOR");
                    }
                    else
                    {
                        tol_color = ssface_proto.GetStringAttribute("TOL_COLOR");
                    }
                }
                catch (System.Exception ex)
                {
                    continue;
                }

                bool chk_key;
                string tolStr = "";
                chk_key = mdColorDic.TryGetValue(Convert.ToInt32(tol_color), out tolStr);
                if (chk_key)
                {
                    string[] tolAry = tolStr.Split('_');
                    if (tolAry.Length >= 2)
                    {
                        //UpTol = Convert.ToDouble(tolAry[0]);
                        //LowTol = Convert.ToDouble(tolAry[1]);
                        UPPER_TOLERANCE = tolAry[0];
                        LOWER_TOLERANCE = tolAry[1];
                        break;
                    }
                    //                     if ()
                    //                     {
                    //                     }
                }
            }
            if (UPPER_TOLERANCE == "")
            {
                UPPER_TOLERANCE = "0";
                LOWER_TOLERANCE = "0";
            }
        }

        public static bool Getugwe2mesData(string jsonPath, out weExportData cFeatureConfig)
        {
            cFeatureConfig = new weExportData();

            try
            {
                bool status;

                //判斷檔案是否存在
                if (!System.IO.File.Exists(jsonPath))
                {
                    return false;
                }
                string jsonText;
                status = ReadFileDataUTF8(jsonPath, out jsonText);
                if (!status)
                {
                    return false;
                }
                cFeatureConfig = JsonConvert.DeserializeObject<weExportData>(jsonText);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool ReadFileDataUTF8(string file_path, out string allContent)
        {
            allContent = "";

            if (!System.IO.File.Exists(file_path))
            {
                return false;
            }

            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(file_path, Encoding.UTF8);

            int index = 0;
            while ((line = file.ReadLine()) != null)
            {
                if (index == 0)
                {
                    allContent += line;
                }
                else
                {
                    allContent += "\n";
                    allContent += line;
                }
                index++;
            }
            file.Close();

            return true;
        }

        private void buttonCancle_Click(object sender, EventArgs e)
        {
            bool status;
            CaxAsm.SetWorkComponent(null);
            Part workPart = theSession.Parts.Work;

            List<CaxAsm.CompPart> AsmCompAry1;
            CaxAsm.CimAsmCompPart sCimAsmCompPart1;
            CaxAsm.GetAllAsmCompStruct(out AsmCompAry1, out sCimAsmCompPart1);

            foreach (CaxAsm.CompPart asmcomp in AsmCompAry1)
            {
                //if (asmcomp.componentOcc.DisplayName.ToUpper().IndexOf("_WEDMS") > 0) 
                if (asmcomp.componentOcc.DisplayName.ToUpper().IndexOf("_" + WE_SECTION) > 0)
                {
                    //取得已做過的線割電極COMP上的屬性(不能刪除)
                    string WE_FINISHED = "";
                    try
                    {
                        WE_FINISHED = asmcomp.componentOcc.GetStringAttribute("WE_FINISHED");
                        continue;
                    }
                    catch (System.Exception ex)
                    { }
                    status = CaxWE.ClosedAllFile(workPart, asmcomp);
                    if (!status)
                    {
                        CaxLog.ShowListingWindow("SelectWorkPart刪線割檔案失敗");
                        return;
                    }
                }
                else if (asmcomp.componentOcc.DisplayName.ToUpper().IndexOf("_WECAM") > 0)
                {
                    status = CaxWE.ClosedAllFile(workPart, asmcomp);
                    if (!status)
                    {
                        CaxLog.ShowListingWindow("SelectWorkPart刪線割下料檔案失敗");
                        return;
                    }
                }
            }
            //CaxLog.ShowListingWindow("555");
            /*
            Session theSession = Session.GetSession();
            string undotomark = "";
            theSession.UndoToMark(CaxWE.OriginalMark, undotomark);
            */

            /*
            bool status;
            weExportData cweExportData = new weExportData();
            cweExportData.EACHGROUPARRAY = new List<CimforceCaxTwMFG.EACHGROUPARRAY>();


            foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WE_FACE_DIC)
            {
                EACHGROUPARRAY cEACHGROUPARRAY = new EACHGROUPARRAY();
                cEACHGROUPARRAY.PROGRAM_ARRAY = new List<CimforceCaxTwMFG.PROGRAMARRAY>();

                string ELEC_NO = "";
                try
                {
                    ELEC_NO = kvp.Value.comp.GetStringAttribute("CIM_ELEC_NO");
                    //CaxLog.ShowListingWindow(ELEC_NO);
                }
                catch (System.Exception ex)
                { }

                Body weCompBody;
                CaxPart.GetLayerBody(kvp.Value.comp, out weCompBody);
                Face[] weCompBodyFaceAry = weCompBody.GetFaces();

                WorkPiece WP = new WorkPiece();
                double[] WPmin = new double[3];
                double[] WPmax = new double[3];
                CaxPart.AskBoundingBoxExactByWCS(weCompBody.Tag, out WPmin, out WPmax);
                WP.WP_Length = WPmax[0] - WPmin[0];
                WP.WP_Wide = WPmax[1] - WPmin[1];
                WP.WP_Height = WPmax[2] - WPmin[2];

                string outer_inner = "";
                string reference_posi = "";
                bool check_isEDM = false;
                bool check_isZMachining = false;

                //判斷此任務是電極或工件
                foreach (Face tempFace in weCompBodyFaceAry)
                {
                    string existBaseFace = "";
                    string isZMachining = "";
                    try
                    {
                        existBaseFace = tempFace.GetStringAttribute("ELECTRODE");
                        if (existBaseFace == "BASE_FACE")
                        {
                            check_isEDM = true;
                        }
                        isZMachining = tempFace.GetStringAttribute("CIM_EDM_FACE");
                        if (isZMachining == "Z")
                        {
                            check_isZMachining = true;
                        }
                    }
                    catch (System.Exception ex)
                    { continue; }
                }

                if (check_isEDM)
                {
                    foreach (Face tempFace in weCompBodyFaceAry)
                    {
                        string ReferPosi = "";
                        try
                        {
                            ReferPosi = tempFace.GetStringAttribute("reference_posi");
                            reference_posi = ReferPosi;
                            outer_inner = "2";
                            cEACHGROUPARRAY.ELEC_NO = ELEC_NO;                //電極號
                        }
                        catch (System.Exception ex)
                        { continue; }
                    }
                }
                else
                {
                    DecideOuterInner(kvp.Value.comp, WP, out outer_inner, out reference_posi);
                }


                cEACHGROUPARRAY.WE_TRUE_FALSE = "FALSE";
                cEACHGROUPARRAY.WP_PHOTO = kvp.Value.comp.Name + ".jpg";        //工件圖
                cEACHGROUPARRAY.REFERENCE_POSITION = reference_posi;            //基準角方位
                cEACHGROUPARRAY.WORKPIECE_HEIGHT = WP.WP_Height.ToString("f4"); //工件高
                cEACHGROUPARRAY.SECTION_ID = kvp.Key.section;                   //工段
                cEACHGROUPARRAY.OUTER_INNER = outer_inner;                      //線割零件檔
                cEACHGROUPARRAY.WE_PART = kvp.Value.comp.Name + ".prt";         //線割零件檔

                PROGRAMARRAY cPROGRAMARRAY = new PROGRAMARRAY();
                cPROGRAMARRAY.FACEGROUPMEMBERS = new List<NXOpen.Tag>();
                cPROGRAMARRAY.MACHING_TYPE = "0";                                      //加工類型
                cPROGRAMARRAY.COLOR_ID = "0";                                            //顏色代碼
                cPROGRAMARRAY.IS_SHCS = "0";                                                //是否逃孔
                cPROGRAMARRAY.MACHING_THICKNESS = "0";                            //加工厚度
                cPROGRAMARRAY.MACHING_COUNT = "0";                                       //加工刀次
                cPROGRAMARRAY.MACHING_ANGLE = "0";                                    //加工角度
                cPROGRAMARRAY.UPPER_TOLERANCE = "0";                                     //上公差
                cPROGRAMARRAY.LOWER_TOLERANCE = "0";                                     //下公差
                cPROGRAMARRAY.WORKHEIGHT_XY = "0";                                       //程式面XY
                cPROGRAMARRAY.WORKHEIGHT_UV = "0";                                       //程式面UV
                cPROGRAMARRAY.MOVEPOSI_X = "0";               //X跑位值
                cPROGRAMARRAY.MOVEPOSI_Y = "0";               //Y跑位值

                cEACHGROUPARRAY.PROGRAM_ARRAY.Add(cPROGRAMARRAY);

                cweExportData.EACHGROUPARRAY.Add(cEACHGROUPARRAY);
            }

            //Session theSession = Session.GetSession();

            //輸出ugwe2mes.dat
            string ugwe2mesJsonPath = string.Format(@"{0}\{1}", Path.GetDirectoryName(theSession.Parts.Display.FullPath), "ugwe2mes.dat");
            
            status = CaxFile.WriteJsonFileData(ugwe2mesJsonPath, cweExportData);
            if (!status)
            {
                CaxLog.ShowListingWindow("ugwe2mes.dat 輸出失敗...");
                return;
            }
            */
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public static void Show_Select_componet(NXOpen.Assemblies.Component comp)
        {
            Session theSession = Session.GetSession();
            List<DisplayableObject> DispalyObject = new List<DisplayableObject>();
            DispalyObject.Add(comp);
            theSession.DisplayManager.UnblankObjects(DispalyObject.ToArray());
            CaxPart.Refresh();
        }

        public static bool Hide_All_Comp(List<NXOpen.Assemblies.Component> components)
        {
            try
            {
                Session theSession = Session.GetSession();
                List<DisplayableObject> HideObjectList = new List<DisplayableObject>();
                foreach (NXOpen.Assemblies.Component xx in components)
                {
                    HideObjectList.Add(xx);
                }
                theSession.DisplayManager.BlankObjects(HideObjectList.ToArray());
            }
            catch (System.Exception ex)
            {
                return false;
            }
            CaxPart.Refresh();
            return true;
        }

        public static void askThicknessRange(double thickness, out string thickness_range)
        {
            thickness_range = "";

            if (thickness > 0 && thickness < 7.5)
            {
                thickness_range = "5";
            }
            else if (thickness >= 7.5 && thickness < 12.5)
            {
                thickness_range = "10";
            }
            else if (thickness >= 12.5 && thickness < 17.5)
            {
                thickness_range = "15";
            }
            else if (thickness >= 17.5 && thickness < 22.5)
            {
                thickness_range = "20";
            }
            else if (thickness >= 22.5 && thickness < 27.5)
            {
                thickness_range = "25";
            }
            else if (thickness >= 27.5 && thickness < 35)
            {
                thickness_range = "30";
            }
            else if (thickness >= 35 && thickness < 45)
            {
                thickness_range = "40";
            }
            else if (thickness >= 45 && thickness < 55)
            {
                thickness_range = "50";
            }
            else if (thickness >= 55 && thickness < 65)
            {
                thickness_range = "60";
            }
            else if (thickness >= 65 && thickness < 75)
            {
                thickness_range = "70";
            }
            else if (thickness >= 75 && thickness < 90)
            {
                thickness_range = "80";
            }
            else if (thickness >= 90 && thickness < 110)
            {
                thickness_range = "100";
            }
            else if (thickness >= 110 && thickness < 125)
            {
                thickness_range = "125";
            }
            else if (thickness >= 125 && thickness < 175)
            {
                thickness_range = "150";
            }
            else if (thickness >= 175)
            {
                thickness_range = "200";
            }
        }

        public static void askMachingAngleRange(double machingAng, out string machingAng_range)
        {
            machingAng_range = "";

            if (machingAng > 0.01 && machingAng < 6)
            {
                machingAng_range = "4";
            }
            else if (machingAng >= 6 && machingAng < 10)
            {
                machingAng_range = "8";
            }
            else if (machingAng >= 10 && machingAng < 12)
            {
                machingAng_range = "12";
            }
            else if (machingAng <= 0.01)
            {
                machingAng_range = "0";
            }
            else if (machingAng >= 12)
            {
                machingAng_range = machingAng.ToString();
            }
        }

        public static void askEspritColor(int wecolor, out string EspritColor)
        {
            EspritColor = "";
            if (wecolor == 2)
                EspritColor = "13434879";
            else if (wecolor == 3)
                EspritColor = "10092543";
            else if (wecolor == 4)
                EspritColor = "6750207";
            else if (wecolor == 5)
                EspritColor = "3407871";
            else if (wecolor == 6)
                EspritColor = "65535";
            else if (wecolor == 7)
                EspritColor = "16777164";
            else if (wecolor == 8)
                EspritColor = "13434828";
            else if (wecolor == 9)
                EspritColor = "10092492";
            else if (wecolor == 10)
                EspritColor = "6750156";
            else if (wecolor == 11)
                EspritColor = "3407820";
            else if (wecolor == 12)
                EspritColor = "65484";
            else if (wecolor == 13)
                EspritColor = "16777113";
            else if (wecolor == 14)
                EspritColor = "13434777";
            else if (wecolor == 15)
                EspritColor = "10092441";
            else if (wecolor == 16)
                EspritColor = "6750105";
            else if (wecolor == 17)
                EspritColor = "3407769";
            else if (wecolor == 18)
                EspritColor = "65433";
            else if (wecolor == 19)
                EspritColor = "16777062";
            else if (wecolor == 20)
                EspritColor = "13434726";
            else if (wecolor == 21)
                EspritColor = "10092390";
            else if (wecolor == 22)
                EspritColor = "6750054";
            else if (wecolor == 23)
                EspritColor = "3407718";
            else if (wecolor == 24)
                EspritColor = "65382";
            else if (wecolor == 25)
                EspritColor = "16777011";
            else if (wecolor == 26)
                EspritColor = "13434675";
            else if (wecolor == 27)
                EspritColor = "10092339";
            else if (wecolor == 28)
                EspritColor = "6750003";
            else if (wecolor == 29)
                EspritColor = "3407667";
            else if (wecolor == 30)
                EspritColor = "65331";
            else if (wecolor == 31)
                EspritColor = "16776960";
            else if (wecolor == 32)
                EspritColor = "13434624";
            else if (wecolor == 33)
                EspritColor = "10092288";
            else if (wecolor == 34)
                EspritColor = "6749952";
            else if (wecolor == 35)
                EspritColor = "3407616";
            else if (wecolor == 36)
                EspritColor = "65280";
            else if (wecolor == 37)
                EspritColor = "16764159";
            else if (wecolor == 38)
                EspritColor = "13421823";
            else if (wecolor == 39)
                EspritColor = "10079487";
            else if (wecolor == 40)
                EspritColor = "6737151";
            else if (wecolor == 41)
                EspritColor = "3394815";
            else if (wecolor == 42)
                EspritColor = "52479";
            else if (wecolor == 43)
                EspritColor = "16764108";
            else if (wecolor == 44)
                EspritColor = "13421772";
            else if (wecolor == 45)
                EspritColor = "10079436";
            else if (wecolor == 46)
                EspritColor = "6737100";
            else if (wecolor == 47)
                EspritColor = "3394764";
            else if (wecolor == 48)
                EspritColor = "52428";
            else if (wecolor == 49)
                EspritColor = "16764057";
            else if (wecolor == 50)
                EspritColor = "13421721";
            else if (wecolor == 51)
                EspritColor = "10079385";
            else if (wecolor == 52)
                EspritColor = "6737049";
            else if (wecolor == 53)
                EspritColor = "3394713";
            else if (wecolor == 54)
                EspritColor = "52377";
            else if (wecolor == 55)
                EspritColor = "16764006";
            else if (wecolor == 56)
                EspritColor = "13421670";
            else if (wecolor == 57)
                EspritColor = "10079334";
            else if (wecolor == 58)
                EspritColor = "6736998";
            else if (wecolor == 59)
                EspritColor = "3394662";
            else if (wecolor == 60)
                EspritColor = "52326";
            else if (wecolor == 61)
                EspritColor = "16763955";
            else if (wecolor == 62)
                EspritColor = "13421619";
            else if (wecolor == 63)
                EspritColor = "10079283";
            else if (wecolor == 64)
                EspritColor = "6736947";
            else if (wecolor == 65)
                EspritColor = "3394611";
            else if (wecolor == 66)
                EspritColor = "52275";
            else if (wecolor == 67)
                EspritColor = "16763904";
            else if (wecolor == 68)
                EspritColor = "13421568";
            else if (wecolor == 69)
                EspritColor = "10079232";
            else if (wecolor == 70)
                EspritColor = "6736896";
            else if (wecolor == 71)
                EspritColor = "3394560";
            else if (wecolor == 72)
                EspritColor = "52224";
            else if (wecolor == 73)
                EspritColor = "16751103";
            else if (wecolor == 74)
                EspritColor = "13408767";
            else if (wecolor == 75)
                EspritColor = "10066431";
            else if (wecolor == 76)
                EspritColor = "6724095";
            else if (wecolor == 77)
                EspritColor = "3381759";
            else if (wecolor == 78)
                EspritColor = "39423";
            else if (wecolor == 79)
                EspritColor = "16751052";
            else if (wecolor == 80)
                EspritColor = "13408716";
            else if (wecolor == 81)
                EspritColor = "10066380";
            else if (wecolor == 82)
                EspritColor = "6724044";
            else if (wecolor == 83)
                EspritColor = "3381708";
            else if (wecolor == 84)
                EspritColor = "39372";
            else if (wecolor == 85)
                EspritColor = "16751001";
            else if (wecolor == 86)
                EspritColor = "13408665";
            else if (wecolor == 87)
                EspritColor = "10066329";
            else if (wecolor == 88)
                EspritColor = "6723993";
            else if (wecolor == 89)
                EspritColor = "3381657";
            else if (wecolor == 90)
                EspritColor = "39321";
            else if (wecolor == 91)
                EspritColor = "16750950";
            else if (wecolor == 92)
                EspritColor = "13408614";
            else if (wecolor == 93)
                EspritColor = "10066278";
            else if (wecolor == 94)
                EspritColor = "6723942";
            else if (wecolor == 95)
                EspritColor = "3381606";
            else if (wecolor == 96)
                EspritColor = "39270";
            else if (wecolor == 97)
                EspritColor = "16750899";
            else if (wecolor == 98)
                EspritColor = "13408563";
            else if (wecolor == 99)
                EspritColor = "10066227";
            else if (wecolor == 100)
                EspritColor = "6723891";
            else if (wecolor == 101)
                EspritColor = "3381555";
            else if (wecolor == 102)
                EspritColor = "39219";
            else if (wecolor == 103)
                EspritColor = "16750848";
            else if (wecolor == 104)
                EspritColor = "13408512";
            else if (wecolor == 105)
                EspritColor = "10066176";
            else if (wecolor == 106)
                EspritColor = "6723840";
            else if (wecolor == 107)
                EspritColor = "3381504";
            else if (wecolor == 108)
                EspritColor = "39168";
            else if (wecolor == 109)
                EspritColor = "16738047";
            else if (wecolor == 110)
                EspritColor = "13395711";
            else if (wecolor == 111)
                EspritColor = "10053375";
            else if (wecolor == 112)
                EspritColor = "6711039";
            else if (wecolor == 113)
                EspritColor = "3368703";
            else if (wecolor == 114)
                EspritColor = "26367";
            else if (wecolor == 115)
                EspritColor = "16737996";
            else if (wecolor == 116)
                EspritColor = "13395660";
            else if (wecolor == 117)
                EspritColor = "10053324";
            else if (wecolor == 118)
                EspritColor = "6710988";
            else if (wecolor == 119)
                EspritColor = "3368652";
            else if (wecolor == 120)
                EspritColor = "26316";
            else if (wecolor == 121)
                EspritColor = "16737945";
            else if (wecolor == 122)
                EspritColor = "13395609";
            else if (wecolor == 123)
                EspritColor = "10053273";
            else if (wecolor == 124)
                EspritColor = "6710937";
            else if (wecolor == 125)
                EspritColor = "3368601";
            else if (wecolor == 126)
                EspritColor = "26265";
            else if (wecolor == 127)
                EspritColor = "16737894";
            else if (wecolor == 128)
                EspritColor = "13395558";
            else if (wecolor == 129)
                EspritColor = "10053222";
            else if (wecolor == 130)
                EspritColor = "6710886";
            else if (wecolor == 131)
                EspritColor = "3368550";
            else if (wecolor == 132)
                EspritColor = "26214";
            else if (wecolor == 133)
                EspritColor = "16737843";
            else if (wecolor == 134)
                EspritColor = "13395507";
            else if (wecolor == 135)
                EspritColor = "10053171";
            else if (wecolor == 136)
                EspritColor = "6710835";
            else if (wecolor == 137)
                EspritColor = "3368499";
            else if (wecolor == 138)
                EspritColor = "26163";
            else if (wecolor == 139)
                EspritColor = "16737792";
            else if (wecolor == 140)
                EspritColor = "13395456";
            else if (wecolor == 141)
                EspritColor = "10053120";
            else if (wecolor == 142)
                EspritColor = "6710784";
            else if (wecolor == 143)
                EspritColor = "3368448";
            else if (wecolor == 144)
                EspritColor = "26112";
            else if (wecolor == 145)
                EspritColor = "16724991";
            else if (wecolor == 146)
                EspritColor = "13382655";
            else if (wecolor == 147)
                EspritColor = "10040319";
            else if (wecolor == 148)
                EspritColor = "6697983";
            else if (wecolor == 149)
                EspritColor = "3355647";
            else if (wecolor == 150)
                EspritColor = "13311";
            else if (wecolor == 151)
                EspritColor = "16724940";
            else if (wecolor == 152)
                EspritColor = "13382604";
            else if (wecolor == 153)
                EspritColor = "10040268";
            else if (wecolor == 154)
                EspritColor = "6697932";
            else if (wecolor == 155)
                EspritColor = "3355596";
            else if (wecolor == 156)
                EspritColor = "13260";
            else if (wecolor == 157)
                EspritColor = "16724889";
            else if (wecolor == 158)
                EspritColor = "13382553";
            else if (wecolor == 159)
                EspritColor = "10040217";
            else if (wecolor == 160)
                EspritColor = "6697881";
            else if (wecolor == 161)
                EspritColor = "3355545";
            else if (wecolor == 162)
                EspritColor = "13209";
            else if (wecolor == 163)
                EspritColor = "16724838";
            else if (wecolor == 164)
                EspritColor = "13382502";
            else if (wecolor == 165)
                EspritColor = "10040166";
            else if (wecolor == 166)
                EspritColor = "6697830";
            else if (wecolor == 167)
                EspritColor = "3355494";
            else if (wecolor == 168)
                EspritColor = "13158";
            else if (wecolor == 169)
                EspritColor = "16724787";
            else if (wecolor == 170)
                EspritColor = "13382451";
            else if (wecolor == 171)
                EspritColor = "10040115";
            else if (wecolor == 172)
                EspritColor = "6697779";
            else if (wecolor == 173)
                EspritColor = "3355443";
            else if (wecolor == 174)
                EspritColor = "13107";
            else if (wecolor == 175)
                EspritColor = "16724736";
            else if (wecolor == 176)
                EspritColor = "13382400";
            else if (wecolor == 177)
                EspritColor = "10040064";
            else if (wecolor == 178)
                EspritColor = "6697728";
            else if (wecolor == 179)
                EspritColor = "3355392";
            else if (wecolor == 180)
                EspritColor = "13056";
            else if (wecolor == 181)
                EspritColor = "16711935";
            else if (wecolor == 182)
                EspritColor = "13369599";
            else if (wecolor == 183)
                EspritColor = "10027263";
            else if (wecolor == 184)
                EspritColor = "6684927";
            else if (wecolor == 185)
                EspritColor = "3342591";
            else if (wecolor == 186)
                EspritColor = "255";
            else if (wecolor == 187)
                EspritColor = "16711884";
            else if (wecolor == 188)
                EspritColor = "13369548";
            else if (wecolor == 189)
                EspritColor = "10027212";
            else if (wecolor == 190)
                EspritColor = "6684876";
            else if (wecolor == 191)
                EspritColor = "3342540";
            else if (wecolor == 192)
                EspritColor = "204";
            else if (wecolor == 193)
                EspritColor = "16711833";
            else if (wecolor == 194)
                EspritColor = "13369497";
            else if (wecolor == 195)
                EspritColor = "10027161";
            else if (wecolor == 196)
                EspritColor = "6684825";
            else if (wecolor == 197)
                EspritColor = "3342489";
            else if (wecolor == 198)
                EspritColor = "153";
            else if (wecolor == 199)
                EspritColor = "16711782";
            else if (wecolor == 200)
                EspritColor = "13369446";
            else if (wecolor == 201)
                EspritColor = "10027110";
            else if (wecolor == 202)
                EspritColor = "6684774";
            else if (wecolor == 203)
                EspritColor = "3342438";
            else if (wecolor == 204)
                EspritColor = "102";
            else if (wecolor == 205)
                EspritColor = "16711731";
            else if (wecolor == 206)
                EspritColor = "13369395";
            else if (wecolor == 207)
                EspritColor = "10027059";
            else if (wecolor == 208)
                EspritColor = "6684723";
            else if (wecolor == 209)
                EspritColor = "3342387";
            else if (wecolor == 210)
                EspritColor = "51";
            else if (wecolor == 211)
                EspritColor = "16711680";
            else if (wecolor == 212)
                EspritColor = "13369344";
            else if (wecolor == 213)
                EspritColor = "10027008";
            else if (wecolor == 214)
                EspritColor = "6684672";
            else if (wecolor == 215)
                EspritColor = "3342336";
            else if (wecolor == 216)
                EspritColor = "0";
        }

        public static bool DecideOuterInner(NXOpen.Assemblies.Component comp, WorkPiece WP, out string outer_inner, out string reference_posi)
        {
            outer_inner = null;
            reference_posi = null;
            try
            {
                string workname = comp.Name;//TEST
                //SetDisplayPart(workname);
                CimforceCaxTwPublic.CaxPart.RefCornerFace sRefCornerFace;
                CaxGeom.FaceData sFaceDataA, sFaceDataB;
                double[] cornerFaceA_dir = new double[3];
                double[] cornerFaceB_dir = new double[3];
                double[] XPositive = { 1, 0, 0 };
                double[] XNegative = { -1, 0, 0 };
                double[] YPositive = { 0, 1, 0 };
                double[] YNegative = { 0, -1, 0 };
                Body body;
                CaxPart.GetLayerBody(comp, out body);
                if (WP.WP_Length >= 200 && WP.WP_Wide >= 200 && WP.WP_Height >= 100)
                {
                    //*****旋轉工件使基準角符合機內校正：長與X平行*****
                    outer_inner = "2";
                    //sEACHGROUPARRAY.OUTER_INNER = "2";
                    if (WP.WP_Length < WP.WP_Wide)
                    {
                        double Rotate_Angle = 90;
                        //RotateObjectByZ(body, Rotate_Angle);
                    }
                    GetBaseCornerFaceAryOnPart(comp, out sRefCornerFace);//TEST
                    Face cornerFaceA = sRefCornerFace.faceA;
                    Face cornerFaceB = sRefCornerFace.faceB;
                    CaxGeom.GetFaceData(cornerFaceA.Tag, out sFaceDataA);
                    CaxGeom.GetFaceData(cornerFaceB.Tag, out sFaceDataB);
                    cornerFaceA_dir[0] = Math.Round(sFaceDataA.dir[0], 1, MidpointRounding.AwayFromZero);
                    cornerFaceA_dir[1] = Math.Round(sFaceDataA.dir[1], 1, MidpointRounding.AwayFromZero);
                    cornerFaceA_dir[2] = Math.Round(sFaceDataA.dir[2], 1, MidpointRounding.AwayFromZero);
                    cornerFaceB_dir[0] = Math.Round(sFaceDataB.dir[0], 1, MidpointRounding.AwayFromZero);
                    cornerFaceB_dir[1] = Math.Round(sFaceDataB.dir[1], 1, MidpointRounding.AwayFromZero);
                    cornerFaceB_dir[2] = Math.Round(sFaceDataB.dir[2], 1, MidpointRounding.AwayFromZero);
                    if (((cornerFaceA_dir[0] == XNegative[0] && cornerFaceA_dir[1] == XNegative[1] && cornerFaceA_dir[2] == XNegative[2]) &&
                        (cornerFaceB_dir[0] == YNegative[0] && cornerFaceB_dir[1] == YNegative[1] && cornerFaceB_dir[2] == YNegative[2])) ||
                        ((cornerFaceA_dir[0] == YNegative[0] && cornerFaceA_dir[1] == YNegative[1] && cornerFaceA_dir[2] == YNegative[2]) &&
                        (cornerFaceB_dir[0] == XNegative[0] && cornerFaceB_dir[1] == XNegative[1] && cornerFaceB_dir[2] == XNegative[2])))
                    {
                        double Rotate_Angle = -180;
                        //RotateObjectByZ(body, Rotate_Angle);
                        reference_posi = "1";
                        //sWEData.REFERENCE_POSITION = "1";
                    }
                    else if (((cornerFaceA_dir[0] == XPositive[0] && cornerFaceA_dir[1] == XPositive[1] && cornerFaceA_dir[2] == XPositive[2]) &&
                             (cornerFaceB_dir[0] == YNegative[0] && cornerFaceB_dir[1] == YNegative[1] && cornerFaceB_dir[2] == YNegative[2])) ||
                             ((cornerFaceA_dir[0] == YNegative[0] && cornerFaceA_dir[1] == YNegative[1] && cornerFaceA_dir[2] == YNegative[2]) &&
                             (cornerFaceB_dir[0] == XPositive[0] && cornerFaceB_dir[1] == XPositive[1] && cornerFaceB_dir[2] == XPositive[2])))
                    {
                        double Rotate_Angle = -180;
                        //RotateObjectByZ(body, Rotate_Angle);
                        reference_posi = "2";
                        //sWEData.REFERENCE_POSITION = "2";
                    }
                    else if (((cornerFaceA_dir[0] == XPositive[0] && cornerFaceA_dir[1] == XPositive[1] && cornerFaceA_dir[2] == XPositive[2]) &&
                             (cornerFaceB_dir[0] == YPositive[0] && cornerFaceB_dir[1] == YPositive[1] && cornerFaceB_dir[2] == YPositive[2])) ||
                             ((cornerFaceA_dir[0] == YPositive[0] && cornerFaceA_dir[1] == YPositive[1] && cornerFaceA_dir[2] == YPositive[2]) &&
                             (cornerFaceB_dir[0] == XPositive[0] && cornerFaceB_dir[1] == XPositive[1] && cornerFaceB_dir[2] == XPositive[2])))
                    {
                        reference_posi = "1";
                        //sWEData.REFERENCE_POSITION = "1";
                    }
                }
                else
                {
                    //*******旋轉工件使基準角符合機外校正：長與Y平行********
                    //outer_inner = "1";
                    outer_inner = "2";//谷崧測試用
                    //sEACHGROUPARRAY.OUTER_INNER = "1";
                    if (WP.WP_Length > WP.WP_Wide)
                    {
                        double Rotate_Angle = 90;
                        //RotateObjectByZ(body, Rotate_Angle);
                    }
                    GetBaseCornerFaceAryOnPart(comp, out sRefCornerFace);//TEST
                    Face cornerFaceA = sRefCornerFace.faceA;
                    Face cornerFaceB = sRefCornerFace.faceB;
                    CaxGeom.GetFaceData(cornerFaceA.Tag, out sFaceDataA);
                    CaxGeom.GetFaceData(cornerFaceB.Tag, out sFaceDataB);
                    cornerFaceA_dir[0] = Math.Round(sFaceDataA.dir[0], 1, MidpointRounding.AwayFromZero);
                    cornerFaceA_dir[1] = Math.Round(sFaceDataA.dir[1], 1, MidpointRounding.AwayFromZero);
                    cornerFaceA_dir[2] = Math.Round(sFaceDataA.dir[2], 1, MidpointRounding.AwayFromZero);
                    cornerFaceB_dir[0] = Math.Round(sFaceDataB.dir[0], 1, MidpointRounding.AwayFromZero);
                    cornerFaceB_dir[1] = Math.Round(sFaceDataB.dir[1], 1, MidpointRounding.AwayFromZero);
                    cornerFaceB_dir[2] = Math.Round(sFaceDataB.dir[2], 1, MidpointRounding.AwayFromZero);
                    if (((cornerFaceA_dir[0] == XPositive[0] && cornerFaceA_dir[1] == XPositive[1] && cornerFaceA_dir[2] == XPositive[2]) &&
                        (cornerFaceB_dir[0] == YNegative[0] && cornerFaceB_dir[1] == YNegative[1] && cornerFaceB_dir[2] == YNegative[2])) ||
                        ((cornerFaceA_dir[0] == YNegative[0] && cornerFaceA_dir[1] == YNegative[1] && cornerFaceA_dir[2] == YNegative[2]) &&
                        (cornerFaceB_dir[0] == XPositive[0] && cornerFaceB_dir[1] == XPositive[1] && cornerFaceB_dir[2] == XPositive[2])))
                    {
                        reference_posi = "4";
                        //sWEData.REFERENCE_POSITION = "4";
                    }
                    else if (((cornerFaceA_dir[0] == XNegative[0] && cornerFaceA_dir[1] == XNegative[1] && cornerFaceA_dir[2] == XNegative[2]) &&
                             (cornerFaceB_dir[0] == YNegative[0] && cornerFaceB_dir[1] == YNegative[1] && cornerFaceB_dir[2] == YNegative[2])) ||
                             ((cornerFaceA_dir[0] == YNegative[0] && cornerFaceA_dir[1] == YNegative[1] && cornerFaceA_dir[2] == YNegative[2]) &&
                             (cornerFaceB_dir[0] == XNegative[0] && cornerFaceB_dir[1] == XNegative[1] && cornerFaceB_dir[2] == XNegative[2])))
                    {
                        double Rotate_Angle = -180;
                        //RotateObjectByZ(body, Rotate_Angle);
                        reference_posi = "1";
                        //sWEData.REFERENCE_POSITION = "1";
                    }
                    else if (((cornerFaceA_dir[0] == XPositive[0] && cornerFaceA_dir[1] == XPositive[1] && cornerFaceA_dir[2] == XPositive[2]) &&
                             (cornerFaceB_dir[0] == YPositive[0] && cornerFaceB_dir[1] == YPositive[1] && cornerFaceB_dir[2] == YPositive[2])) ||
                             ((cornerFaceA_dir[0] == YPositive[0] && cornerFaceA_dir[1] == YPositive[1] && cornerFaceA_dir[2] == YPositive[2]) &&
                             (cornerFaceB_dir[0] == XPositive[0] && cornerFaceB_dir[1] == XPositive[1] && cornerFaceB_dir[2] == XPositive[2])))
                    {
                        reference_posi = "1";
                        //sWEData.REFERENCE_POSITION = "1";
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static void RotateObjectByZ(Body sbody, double Rotate_Angle)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Edit->Move Object...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Features.MoveObject nullFeatures_MoveObject = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.MoveObjectBuilder moveObjectBuilder1;
            moveObjectBuilder1 = workPart.BaseFeatures.CreateMoveObjectBuilder(nullFeatures_MoveObject);

            moveObjectBuilder1.TransformMotion.DistanceAngle.OrientXpress.AxisOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Axis.Passive;

            moveObjectBuilder1.TransformMotion.DistanceAngle.OrientXpress.PlaneOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Plane.Passive;

            moveObjectBuilder1.TransformMotion.AlongCurveAngle.AlongCurve.IsPercentUsed = true;

            moveObjectBuilder1.TransformMotion.AlongCurveAngle.AlongCurve.Expression.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.AlongCurveAngle.AlongCurve.Expression.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.OrientXpress.AxisOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Axis.Passive;

            moveObjectBuilder1.TransformMotion.OrientXpress.PlaneOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Plane.Passive;

            moveObjectBuilder1.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.Distance;

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = "-20";

            moveObjectBuilder1.TransformMotion.DistanceBetweenPointsDistance.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.RadialDistance.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.Angle.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.DistanceAngle.Distance.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.DistanceAngle.Angle.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.DeltaEnum = NXOpen.GeometricUtilities.ModlMotion.Delta.ReferenceWcsWorkPart;

            moveObjectBuilder1.TransformMotion.DeltaXc.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.DeltaYc.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.DeltaZc.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.AlongCurveAngle.AlongCurve.Expression.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.AlongCurveAngle.AlongCurveAngle.RightHandSide = "0";

            theSession.SetUndoMarkName(markId1, "Move Object Dialog");

            //Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(0)");
            Body body1 = sbody;
            bool added1;
            added1 = moveObjectBuilder1.ObjectToMoveObject.Add(body1);

            moveObjectBuilder1.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.Angle;

            Unit unit1;
            unit1 = moveObjectBuilder1.TransformMotion.RadialOriginDistance.Units;

            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            Vector3d vector1 = new Vector3d(0.0, 0.0, 1.0);
            Direction direction1;
            direction1 = workPart.Directions.CreateDirection(origin1, vector1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            NXOpen.Point nullPoint = null;
            Axis axis1;
            axis1 = workPart.Axes.CreateAxis(nullPoint, direction1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            moveObjectBuilder1.TransformMotion.AngularAxis = axis1;

            moveObjectBuilder1.TransformMotion.AngularAxis = axis1;

            Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

            Expression expression3;
            expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression4;
            expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("p263_x=0.00000000000", unit1);

            Expression expression5;
            expression5 = workPart.Expressions.CreateSystemExpressionWithUnits("p264_y=0.00000000000", unit1);

            Expression expression6;
            expression6 = workPart.Expressions.CreateSystemExpressionWithUnits("p265_z=0.00000000000", unit1);

            Expression expression7;
            expression7 = workPart.Expressions.CreateSystemExpressionWithUnits("p266_xdelta=0.00000000000", unit1);

            Expression expression8;
            expression8 = workPart.Expressions.CreateSystemExpressionWithUnits("p267_ydelta=0.00000000000", unit1);

            Expression expression9;
            expression9 = workPart.Expressions.CreateSystemExpressionWithUnits("p268_zdelta=0.00000000000", unit1);

            Expression expression10;
            expression10 = workPart.Expressions.CreateSystemExpressionWithUnits("p269_radius=0.00000000000", unit1);

            Unit unit2;
            unit2 = moveObjectBuilder1.TransformMotion.DistanceAngle.Angle.Units;

            Expression expression11;
            expression11 = workPart.Expressions.CreateSystemExpressionWithUnits("p270_angle=0.00000000000", unit2);

            Expression expression12;
            expression12 = workPart.Expressions.CreateSystemExpressionWithUnits("p271_zdelta=0.00000000000", unit1);

            Expression expression13;
            expression13 = workPart.Expressions.CreateSystemExpressionWithUnits("p272_radius=0.00000000000", unit1);

            Expression expression14;
            expression14 = workPart.Expressions.CreateSystemExpressionWithUnits("p273_angle1=0.00000000000", unit2);

            Expression expression15;
            expression15 = workPart.Expressions.CreateSystemExpressionWithUnits("p274_angle2=0.00000000000", unit2);

            Expression expression16;
            expression16 = workPart.Expressions.CreateSystemExpressionWithUnits("p275_distance=0", unit1);

            Expression expression17;
            expression17 = workPart.Expressions.CreateSystemExpressionWithUnits("p276_arclen=0", unit1);

            Unit nullUnit = null;
            Expression expression18;
            expression18 = workPart.Expressions.CreateSystemExpressionWithUnits("p277_percent=0", nullUnit);

            expression4.RightHandSide = "20";

            expression5.RightHandSide = "20";

            expression6.RightHandSide = "20";

            expression7.RightHandSide = "0";

            expression8.RightHandSide = "0";

            expression9.RightHandSide = "0";

            expression10.RightHandSide = "0";

            expression11.RightHandSide = "0";

            expression12.RightHandSide = "0";

            expression13.RightHandSide = "0";

            expression14.RightHandSide = "0";

            expression15.RightHandSide = "0";

            expression16.RightHandSide = "0";

            expression18.RightHandSide = "100";

            expression17.RightHandSide = "0";

            theSession.SetUndoMarkName(markId2, "Point Dialog");

            Expression expression19;
            expression19 = workPart.Expressions.CreateSystemExpressionWithUnits("p278_x=0.00000000000", unit1);

            Scalar scalar1;
            scalar1 = workPart.Scalars.CreateScalarExpression(expression19, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Expression expression20;
            expression20 = workPart.Expressions.CreateSystemExpressionWithUnits("p279_y=0.00000000000", unit1);

            Scalar scalar2;
            scalar2 = workPart.Scalars.CreateScalarExpression(expression20, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Expression expression21;
            expression21 = workPart.Expressions.CreateSystemExpressionWithUnits("p280_z=0.00000000000", unit1);

            Scalar scalar3;
            scalar3 = workPart.Scalars.CreateScalarExpression(expression21, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            NXOpen.Point point1;
            point1 = workPart.Points.CreatePoint(scalar1, scalar2, scalar3, NXOpen.SmartObject.UpdateOption.WithinModeling);

            expression4.RightHandSide = "0";

            expression5.RightHandSide = "0";

            expression6.RightHandSide = "0";

            expression4.RightHandSide = "0.00000000000";

            expression5.RightHandSide = "0.00000000000";

            expression6.RightHandSide = "0.00000000000";

            expression4.RightHandSide = "0";

            expression5.RightHandSide = "0";

            expression6.RightHandSide = "0";

            expression4.RightHandSide = "0.00000000000";

            expression5.RightHandSide = "0.00000000000";

            expression6.RightHandSide = "0.00000000000";

            expression7.RightHandSide = "0.00000000000";

            expression8.RightHandSide = "0.00000000000";

            expression9.RightHandSide = "0.00000000000";

            expression10.RightHandSide = "0.00000000000";

            expression11.RightHandSide = "0.00000000000";

            expression12.RightHandSide = "0.00000000000";

            expression13.RightHandSide = "0.00000000000";

            expression14.RightHandSide = "0.00000000000";

            expression15.RightHandSide = "0.00000000000";

            expression18.RightHandSide = "100.00000000000";

            expression4.RightHandSide = "0";

            expression5.RightHandSide = "0";

            expression6.RightHandSide = "0";

            expression4.RightHandSide = "0.00000000000";

            expression5.RightHandSide = "0.00000000000";

            expression6.RightHandSide = "0.00000000000";

            // ----------------------------------------------
            //   Dialog Begin Point
            // ----------------------------------------------
            expression4.RightHandSide = "0";

            workPart.Points.DeletePoint(point1);

            expression4.RightHandSide = "0";

            expression5.RightHandSide = "0.00000000000";

            expression6.RightHandSide = "0.00000000000";

            Expression expression22;
            expression22 = workPart.Expressions.CreateSystemExpressionWithUnits("p265_x=0", unit1);

            Scalar scalar4;
            scalar4 = workPart.Scalars.CreateScalarExpression(expression22, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Expression expression23;
            expression23 = workPart.Expressions.CreateSystemExpressionWithUnits("p266_y=0.00000000000", unit1);

            Scalar scalar5;
            scalar5 = workPart.Scalars.CreateScalarExpression(expression23, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Expression expression24;
            expression24 = workPart.Expressions.CreateSystemExpressionWithUnits("p267_z=0.00000000000", unit1);

            Scalar scalar6;
            scalar6 = workPart.Scalars.CreateScalarExpression(expression24, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            NXOpen.Point point2;
            point2 = workPart.Points.CreatePoint(scalar4, scalar5, scalar6, NXOpen.SmartObject.UpdateOption.WithinModeling);

            expression5.RightHandSide = "0";

            expression4.RightHandSide = "0";

            expression5.RightHandSide = "0";

            expression6.RightHandSide = "0.00000000000";

            workPart.Points.DeletePoint(point2);

            Expression expression25;
            expression25 = workPart.Expressions.CreateSystemExpressionWithUnits("p265_x=0", unit1);

            Scalar scalar7;
            scalar7 = workPart.Scalars.CreateScalarExpression(expression25, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Expression expression26;
            expression26 = workPart.Expressions.CreateSystemExpressionWithUnits("p266_y=0", unit1);

            Scalar scalar8;
            scalar8 = workPart.Scalars.CreateScalarExpression(expression26, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Expression expression27;
            expression27 = workPart.Expressions.CreateSystemExpressionWithUnits("p267_z=0.00000000000", unit1);

            Scalar scalar9;
            scalar9 = workPart.Scalars.CreateScalarExpression(expression27, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            NXOpen.Point point3;
            point3 = workPart.Points.CreatePoint(scalar7, scalar8, scalar9, NXOpen.SmartObject.UpdateOption.WithinModeling);

            expression6.RightHandSide = "0";

            expression4.RightHandSide = "0";

            expression5.RightHandSide = "0";

            expression6.RightHandSide = "0";

            workPart.Points.DeletePoint(point3);

            Expression expression28;
            expression28 = workPart.Expressions.CreateSystemExpressionWithUnits("p265_x=0", unit1);

            Scalar scalar10;
            scalar10 = workPart.Scalars.CreateScalarExpression(expression28, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Expression expression29;
            expression29 = workPart.Expressions.CreateSystemExpressionWithUnits("p266_y=0", unit1);

            Scalar scalar11;
            scalar11 = workPart.Scalars.CreateScalarExpression(expression29, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Expression expression30;
            expression30 = workPart.Expressions.CreateSystemExpressionWithUnits("p267_z=0", unit1);

            Scalar scalar12;
            scalar12 = workPart.Scalars.CreateScalarExpression(expression30, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            NXOpen.Point point4;
            point4 = workPart.Points.CreatePoint(scalar10, scalar11, scalar12, NXOpen.SmartObject.UpdateOption.WithinModeling);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

            theSession.DeleteUndoMark(markId3, null);

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

            expression4.RightHandSide = "0";

            expression5.RightHandSide = "0";

            expression6.RightHandSide = "0";

            workPart.Points.DeletePoint(point4);

            Expression expression31;
            expression31 = workPart.Expressions.CreateSystemExpressionWithUnits("p265_x=0", unit1);

            Scalar scalar13;
            scalar13 = workPart.Scalars.CreateScalarExpression(expression31, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Expression expression32;
            expression32 = workPart.Expressions.CreateSystemExpressionWithUnits("p266_y=0", unit1);

            Scalar scalar14;
            scalar14 = workPart.Scalars.CreateScalarExpression(expression32, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Expression expression33;
            expression33 = workPart.Expressions.CreateSystemExpressionWithUnits("p267_z=0", unit1);

            Scalar scalar15;
            scalar15 = workPart.Scalars.CreateScalarExpression(expression33, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            NXOpen.Point point5;
            point5 = workPart.Points.CreatePoint(scalar13, scalar14, scalar15, NXOpen.SmartObject.UpdateOption.WithinModeling);

            theSession.DeleteUndoMark(markId4, null);

            theSession.SetUndoMarkName(markId2, "Point");

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression4);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression5);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression6);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression7);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression8);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression9);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression10);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression11);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression12);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression13);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression14);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression15);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression16);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression17);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression18);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            workPart.Expressions.Delete(expression3);

            theSession.DeleteUndoMark(markId2, null);

            Scalar scalar16;
            scalar16 = workPart.Scalars.CreateScalarExpression(expression31, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Scalar scalar17;
            scalar17 = workPart.Scalars.CreateScalarExpression(expression32, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Scalar scalar18;
            scalar18 = workPart.Scalars.CreateScalarExpression(expression33, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            NXOpen.Point point6;
            point6 = workPart.Points.CreatePoint(scalar16, scalar17, scalar18, NXOpen.SmartObject.UpdateOption.WithinModeling);

            axis1.Point = point5;

            moveObjectBuilder1.TransformMotion.AngularAxis = axis1;

            moveObjectBuilder1.TransformMotion.Angle.RightHandSide = Rotate_Angle.ToString();

            moveObjectBuilder1.TransformMotion.Angle.RightHandSide = Rotate_Angle.ToString();

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            theSession.DeleteUndoMark(markId5, null);

            NXOpen.Session.UndoMarkId markId6;
            markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            NXObject nXObject1;
            nXObject1 = moveObjectBuilder1.Commit();

            NXObject[] objects1;
            objects1 = moveObjectBuilder1.GetCommittedObjects();

            theSession.DeleteUndoMark(markId6, null);

            theSession.SetUndoMarkName(markId1, "Move Object");

            moveObjectBuilder1.Destroy();

            workPart.Points.DeletePoint(point6);

            workPart.Expressions.Delete(expression2);

            workPart.Expressions.Delete(expression1);

            // ----------------------------------------------
            //   Menu: Tools->Journal->Stop Recording
            // ----------------------------------------------

        }

        public static bool GetBaseCornerFaceAryOnPart(NXOpen.Assemblies.Component component, out CimforceCaxTwPublic.CaxPart.RefCornerFace sRefCornerFace)
        {
            sRefCornerFace.faceA = null;
            sRefCornerFace.faceB = null;
            sRefCornerFace.faceC = null;
            sRefCornerFace.faceD = null;

            try
            {
                //取得基準面(A,B,C)

                Part compPart = (Part)component.Prototype;
                Body[] BodyAry = compPart.Bodies.ToArray();

                Face baseFaceA = null;
                Face baseFaceB = null;
                Face baseFaceC = null;
                Face baseFaceD = null;

                string attr_value = "";
                for (int i = 0; i < BodyAry.Length; i++)
                {
                    Face[] bodyFaceAry = BodyAry[i].GetFaces();
                    for (int j = 0; j < bodyFaceAry.Length; j++)
                    {
                        try
                        {
                            attr_value = "";
                            //attr_value = bodyFaceAry[j].GetStringAttribute(CaxDefineParam.ATTR_CIM_TYPE);
                            attr_value = bodyFaceAry[j].GetStringAttribute(CaxDefineParam.ATTR_CIM_REF_FACE);
                            if (attr_value == "A")
                            {
                                baseFaceA = bodyFaceAry[j];
                                sRefCornerFace.faceA = baseFaceA;
                            }
                            else if (attr_value == "B")
                            {
                                baseFaceB = bodyFaceAry[j];
                                sRefCornerFace.faceB = baseFaceB;
                            }
                            else if (attr_value == "C")
                            {
                                baseFaceC = bodyFaceAry[j];
                                sRefCornerFace.faceC = baseFaceC;
                            }
                            else if (attr_value == "D")
                            {
                                baseFaceD = bodyFaceAry[j];
                                sRefCornerFace.faceD = baseFaceD;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            attr_value = "";
                            continue;
                        }
                    }
                }

                //             if (baseFaceA == null && baseFaceB == null && baseFaceC == null && baseFaceD == null)
                //             {
                //                 return false;
                //             }
                // 
                //             Tag faceTagOcc;
                //             try
                //             {
                //                 faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceA.Tag);
                //                 sRefCornerFace.faceA = (Face)NXObjectManager.Get(faceTagOcc);
                //             }
                //             catch (System.Exception ex)
                //             {
                // 
                //             }
                // 
                //             try
                //             {
                //                 faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceB.Tag);
                //                 sRefCornerFace.faceB = (Face)NXObjectManager.Get(faceTagOcc);
                //             }
                //             catch (System.Exception ex)
                //             {
                // 
                //             }
                // 
                //             try
                //             {
                //                 faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceC.Tag);
                //                 sRefCornerFace.faceC = (Face)NXObjectManager.Get(faceTagOcc);
                //             }
                //             catch (System.Exception ex)
                //             {
                // 
                //             }
                // 
                //             try
                //             {
                //                 faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceD.Tag);
                //                 sRefCornerFace.faceD = (Face)NXObjectManager.Get(faceTagOcc);
                //             }
                //             catch (System.Exception ex)
                //             {
                // 
                //             }

            }
            catch (System.Exception ex)
            {
                //CaxPart.ShowListingWindow(ex.ToString());
                return false;
            }

            return true;
        }

        public void superGridMainControl_Click(object sender, EventArgs e)
        {
            //             try
            //             {
            //                 //擺要執行的動作
            // 
            //                 //                 GridRow a = new GridRow();
            //                 //                 
            //                 //                 CaxLog.ShowListingWindow(a.Cells["ListWorkPiece"].Value.ToString());
            //                 //                 return;
            //                 List<NXOpen.Assemblies.Component> hideNewComp = new List<NXOpen.Assemblies.Component>();
            //                 foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WE_FACE_DIC)
            //                 {
            //                     hideNewComp.Add(kvp.Value.comp);
            //                 }
            //                 Hide_All_Comp(hideNewComp);
            //                 return;
            //                 //Show_Select_componet(Program.sssComponent[Click_Form1_Index]);
            //                 //CaxAsm.SetWorkComponent(Program.sssComponent[Click_Form1_Index]);
            //             }
            //             catch (System.Exception ex)
            //             {
            //                 MessageBox.Show(ex.ToString());
            //             }
        }

        private void superGridMainControl_RowClick(object sender, GridRowClickEventArgs e)
        {
            //隱藏所有的NewComp
            List<NXOpen.Assemblies.Component> hideNewComp = new List<NXOpen.Assemblies.Component>();
            foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WE_FACE_DIC)
            {
                hideNewComp.Add(kvp.Value.comp);
            }
            Hide_All_Comp(hideNewComp);

            //顯示選取的NewComp
            Click_Form1_Index = e.GridRow.Index;
            superGridMainControl.PrimaryGrid.GetCell(Click_Form1_Index, 2).Value.ToString();
            WeListKey sWeListKey = new WeListKey();
            sWeListKey.compName = superGridMainControl.PrimaryGrid.GetCell(Click_Form1_Index, 2).Value.ToString();
            sWeListKey.section = superGridMainControl.PrimaryGrid.GetCell(Click_Form1_Index, 3).Value.ToString();
            sWeListKey.wkface = superGridMainControl.PrimaryGrid.GetCell(Click_Form1_Index, 4).Value.ToString();
            WeFaceGroup sWeFaceGroup = new WeFaceGroup();
            WE_FACE_DIC.TryGetValue(sWeListKey, out sWeFaceGroup);
            Show_Select_componet(sWeFaceGroup.comp);
        }

        public static bool DeleteBody(Body DeleteTargetBody)
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Edit->Delete...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Delete");

                bool notifyOnDelete1;
                notifyOnDelete1 = theSession.Preferences.Modeling.NotifyOnDelete;

                theSession.UpdateManager.ClearErrorList();

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Delete");

                NXObject[] objects1 = new NXObject[1];
                NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject(DeleteTargetBody.JournalIdentifier);
                objects1[0] = brep1;
                int nErrs1;
                nErrs1 = theSession.UpdateManager.AddToDeleteList(objects1);

                bool notifyOnDelete2;
                notifyOnDelete2 = theSession.Preferences.Modeling.NotifyOnDelete;

                int nErrs2;
                nErrs2 = theSession.UpdateManager.DoUpdate(markId2);

                theSession.DeleteUndoMark(markId1, null);

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

        private void SelectWorkPart_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        public static bool GetStockData(string jsonPath, out Mfg2MesJsonClass cStockData)
        {
            cStockData = new Mfg2MesJsonClass();

            try
            {
                bool status;

                //判斷檔案是否存在
                if (!System.IO.File.Exists(jsonPath))
                {
                    return false;
                }
                string jsonText;
                status = ReadFileDataUTF8(jsonPath, out jsonText);
                if (!status)
                {
                    return false;
                }
                cStockData = JsonConvert.DeserializeObject<Mfg2MesJsonClass>(jsonText);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }


        

    }
}
