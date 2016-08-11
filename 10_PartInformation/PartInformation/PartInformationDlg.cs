using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;
using CaxGlobaltek;
using NXOpen.Utilities;

namespace PartInformation
{
    public partial class PartInformationDlg : DevComponents.DotNetBar.Office2007Form
    {

        public static Session theSession = Session.GetSession();
        public static UI theUI;
        public static UFSession theUfSession = UFSession.GetUFSession();
        public static Part workPart = theSession.Parts.Work;
        public static Part displayPart = theSession.Parts.Display;

        public static DraftingConfig cDraftingConfig = new DraftingConfig();
        public static Point3d NotePosition = new Point3d();
        public static Dictionary<string, string> DicPartInformation = new Dictionary<string, string>();

        public static Dictionary<string, string> DicPartNumberPos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicCusRevPos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicPartDescriptionPos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicRevStartPos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicPartUnitPos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicRevDateStartPos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicAuthDatePos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicMaterialPos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicPageNumberPos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicTolTitle0Pos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicTolTitle1Pos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicTolTitle2Pos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicTolTitle3Pos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicTolTitle4Pos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicTolValue0Pos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicTolValue1Pos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicTolValue2Pos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicTolValue3Pos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicTolValue4Pos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicSecondPartNumberPos = new Dictionary<string, string>();
        public static Dictionary<string, string> DicSecondPageNumberPos = new Dictionary<string, string>();

        public static string RevRowHeight = "";
        public static string symbol = "", symbol_1 = "";
        public static double SheetHeight, SheetWidth;
        //public static StringBuilder sb;

        public class TablePosi
        {
            public static string PartNumberPos = "PartNumberPos";
            public static string CusRevPos = "CusRevPos";
            public static string PartDescriptionPos = "PartDescriptionPos";
            public static string RevStartPos = "RevStartPos";
            public static string PartUnitPos = "PartUnitPos";
            public static string TolTitle0Pos = "TolTitle0Pos";
            public static string TolTitle1Pos = "TolTitle1Pos";
            public static string TolTitle2Pos = "TolTitle2Pos";
            public static string TolTitle3Pos = "TolTitle3Pos";
            public static string TolTitle4Pos = "TolTitle4Pos";
            public static string TolValue0Pos = "TolValue0Pos";
            public static string TolValue1Pos = "TolValue1Pos";
            public static string TolValue2Pos = "TolValue2Pos";
            public static string TolValue3Pos = "TolValue3Pos";
            public static string TolValue4Pos = "TolValue4Pos";
            public static string RevDateStartPos = "RevDateStartPos";
            public static string AuthDatePos = "AuthDatePos";
            public static string MaterialPos = "MaterialPos";
            public static string ProcNamePos = "ProcNamePos";
            public static string PageNumberPos = "PageNumberPos";
            public static string SecondPartNumberPos = "SecondPartNumberPos";
            public static string SecondPageNumberPos = "SecondPageNumberPos";
        }

        //public static class WorkPartAttribute
        //{
        //    public static string PartNumberText = "PartNumberText";
        //    public static string PartDescriptionText = "PartDescriptionText";
        //    public static string CurRevText = "CurRevText";
        //    public static string PartUnitText = "PartUnitText";
        //    public static string MaterialText = "MaterialText";
        //    public static string DraftingText = "DraftingText";
        //    public static string Tolerance0Text = "Tolerance0Text";
        //    public static string Tolerance1Text = "Tolerance1Text";
        //    public static string Tolerance2Text = "Tolerance2Text";
        //    public static string Tolerance3Text = "Tolerance3Text";
        //    public static string AngleText = "AngleText";
        //}

        public PartInformationDlg()
        {
            InitializeComponent();
        }

        private void PartInformationDlg_Load(object sender, EventArgs e)
        {
            //取得DraftingData
            CaxGetDatData.GetDraftingConfigData(out cDraftingConfig);
            
            //取得製圖版次的增加高度
            RevRowHeight = cDraftingConfig.Drafting[0].RevRowHeight;

            //預設開啟範圍區分checkbox
            chb_Point.Checked = true;

            //取得零件料號
            PartNumberText.Text = workPart.JournalIdentifier;

            //取得單位,1=Metric, 2=English
            int UnitsNum;
            theUfSession.Part.AskUnits(workPart.Tag, out UnitsNum);
            if (UnitsNum == 1)
            {
                PartUnitText.Text = "mm";
            }
            else
            {
                PartUnitText.Text = "inch";
            }

            #region 取得零件屬性，如已做過，則取屬性重新塞回欄位

            //---料號PartNumber
            try
            {
                if (PartNumberText.Text != workPart.GetStringAttribute(TablePosi.PartNumberPos))
                {
                    PartNumberText.Text = workPart.GetStringAttribute(TablePosi.PartNumberPos);
                }
            }
            catch (System.Exception ex)
            {
            	
            }

            //---客戶版次CusRev
            try
            {
                CusRevText.Text = workPart.GetStringAttribute(TablePosi.CusRevPos);
            }
            catch (System.Exception ex)
            {
                CusRevText.Text = "";
            }

            //---品名PartDescription
            try
            {
                PartDescriptionText.Text = workPart.GetStringAttribute(TablePosi.PartDescriptionPos);
            }
            catch (System.Exception ex)
            {
                PartDescriptionText.Text = "";
            }

            //---單位PartUnit
            try
            {
                if (PartUnitText.Text != workPart.GetStringAttribute(TablePosi.PartUnitPos))
                {
                    PartUnitText.Text = workPart.GetStringAttribute(TablePosi.PartUnitPos);
                }
            }
            catch (System.Exception ex)
            {
                
            }

            //---材質Material
            try
            {
                MaterialText.Text = workPart.GetStringAttribute(TablePosi.MaterialPos);
            }
            catch (System.Exception ex)
            {
                MaterialText.Text = "";
            }

            //---製圖版次DraftingRev
            try
            {
                DraftingRevText.Text = workPart.GetStringAttribute(TablePosi.RevStartPos);
            }
            catch (System.Exception ex)
            {
                DraftingRevText.Text = "";
            }

            //---公差TolValue0
            try
            {
                TolValue0.Text = workPart.GetStringAttribute(TablePosi.TolValue0Pos);
            }
            catch (System.Exception ex)
            {
                TolValue0.Text = "0.25";
            }

            //---公差TolValue1
            try
            {
                TolValue1.Text = workPart.GetStringAttribute(TablePosi.TolValue1Pos);
            }
            catch (System.Exception ex)
            {
                TolValue1.Text = "0.12";
            }

            //---公差TolValue2
            try
            {
                TolValue2.Text = workPart.GetStringAttribute(TablePosi.TolValue2Pos);
            }
            catch (System.Exception ex)
            {
                TolValue2.Text = "0.05";
            }

            //---公差TolValue3
            try
            {
                TolValue3.Text = workPart.GetStringAttribute(TablePosi.TolValue3Pos);
            }
            catch (System.Exception ex)
            {
                TolValue3.Text = "";
            }

            #endregion
            

        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            //抓取目前圖紙數量和Tag
            int SheetCount = 0;
            NXOpen.Tag[] SheetTagAry = null;
            theUfSession.Draw.AskDrawings(out SheetCount, out SheetTagAry);

            //建立字典資料庫存取(舊版)
            DicPartInformation = new Dictionary<string, string>();
            //建立字典資料庫存取(新版)
            DicPartNumberPos = new Dictionary<string, string>();
            DicCusRevPos = new Dictionary<string, string>();
            DicPartDescriptionPos = new Dictionary<string, string>();
            DicRevStartPos = new Dictionary<string, string>();
            DicPartUnitPos = new Dictionary<string, string>();
            DicRevDateStartPos = new Dictionary<string, string>();
            DicAuthDatePos = new Dictionary<string, string>();
            DicMaterialPos = new Dictionary<string, string>();
            DicPageNumberPos = new Dictionary<string, string>();
            DicSecondPartNumberPos = new Dictionary<string, string>();
            DicSecondPageNumberPos = new Dictionary<string, string>();
            DicTolTitle0Pos = new Dictionary<string, string>();
            DicTolTitle1Pos = new Dictionary<string, string>();
            DicTolTitle2Pos = new Dictionary<string, string>();
            DicTolTitle3Pos = new Dictionary<string, string>();


            #region 將對話框的資料填入字典中

            for (int i = 0; i < SheetCount; i++)
            {
                if (i == 0)
                {
                    //舊版(全部放在同一字典)
                    //DicPartInformation.Add(TablePosi.PartNumberPos, PartNumberText.Text);//料號
                    //DicPartInformation.Add(TablePosi.PartDescriptionPos, PartDescriptionText.Text);//品名
                    //DicPartInformation.Add(TablePosi.PartUnitPos, PartUnitText.Text);//單位
                    //DicPartInformation.Add(TablePosi.RevStartPos, DraftingRevText.Text);//版次
                    //DicPartInformation.Add(TablePosi.RevDateStartPos, DateTime.Now.ToShortDateString());//版次日期
                    //DicPartInformation.Add(TablePosi.AuthDatePos, DateTime.Now.ToShortDateString());//日期
                    //DicPartInformation.Add(TablePosi.MaterialPos, MaterialText.Text);//材質
                    //DicPartInformation.Add(TablePosi.PageNumberPos, "1/" + SheetCount.ToString());//頁數

                    //新版(資料分開放)
                    DicPartNumberPos.Add(TablePosi.PartNumberPos, PartNumberText.Text);//料號
                    DicCusRevPos.Add(TablePosi.CusRevPos, CusRevText.Text);//客戶版次
                    DicPartDescriptionPos.Add(TablePosi.PartDescriptionPos, PartDescriptionText.Text);//品名
                    DicPartUnitPos.Add(TablePosi.PartUnitPos, PartUnitText.Text);//單位
                    DicRevStartPos.Add(TablePosi.RevStartPos, DraftingRevText.Text);//製圖版次
                    DicRevDateStartPos.Add(TablePosi.RevDateStartPos, DateTime.Now.ToShortDateString());//版次日期
                    DicAuthDatePos.Add(TablePosi.AuthDatePos, DateTime.Now.ToShortDateString());//出圖日期
                    DicMaterialPos.Add(TablePosi.MaterialPos, MaterialText.Text);//材質
                    DicPageNumberPos.Add(TablePosi.PageNumberPos, "1/" + SheetCount.ToString());//頁數

                    if (TolValue0.Text != "")
                    {
                        DicTolValue0Pos.Add(TablePosi.TolValue0Pos, "<$t>" + TolValue0.Text);
                        DicTolTitle0Pos.Add(TablePosi.TolTitle0Pos, TolTitle0.Text);
                    }
                    if (TolValue1.Text != "")
                    {
                        DicTolValue1Pos.Add(TablePosi.TolValue1Pos, "<$t>" + TolValue1.Text);
                        DicTolTitle1Pos.Add(TablePosi.TolTitle1Pos, TolTitle1.Text);
                    }
                    if (TolValue2.Text != "")
                    {
                        DicTolValue2Pos.Add(TablePosi.TolValue2Pos, "<$t>" + TolValue2.Text);
                        DicTolTitle2Pos.Add(TablePosi.TolTitle2Pos, TolTitle2.Text);
                    }
                    if (AngleText.Text != "")
                    {
                        DicTolValue3Pos.Add(TablePosi.TolValue3Pos, "<$t>" + AngleText.Text + "<$s>");
                        DicTolTitle3Pos.Add(TablePosi.TolTitle3Pos, "<F18>角度");
                    }
                }
                else
                {
                    bool check = false;
                    string tempSecondPartNumberPos = "";

                    check = DicSecondPartNumberPos.TryGetValue(TablePosi.SecondPartNumberPos, out tempSecondPartNumberPos);
                    if (!check)
                    {
                        DicSecondPartNumberPos.Add(TablePosi.SecondPartNumberPos, PartNumberText.Text);//第二頁以上的料號
                    }
                }
            }

            #endregion

            //取得所有Note
            NXOpen.Annotations.Note[] NotesAry = workPart.Notes.ToArray();

            for (int i = 0; i < SheetCount; i++)
            {
                //輪巡每個Sheet
                NXOpen.Drawings.DrawingSheet CurrentSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(SheetTagAry[i]);
                NXOpen.Drawings.DrawingSheet drawingSheet1 = (NXOpen.Drawings.DrawingSheet)workPart.DrawingSheets.FindObject(CurrentSheet.Name);
                drawingSheet1.Open();
                
                //寫入頁數資訊(加入sheet2以上)
                if (i > 0)
                {
                    string tempString = (i + 1).ToString() + "/" + SheetCount.ToString();
                    //DicPartInformation[TablePosi.SecondPageNumberPos] = tempString;
                    DicSecondPageNumberPos[TablePosi.SecondPageNumberPos] = tempString;
                }

                for (int ii = 0; ii < cDraftingConfig.Drafting.Count; ii++)
                {
                    string SheetSize = cDraftingConfig.Drafting[ii].SheetSize;
                    if (Math.Ceiling(CurrentSheet.Height) != Convert.ToDouble(SheetSize.Split(',')[0]) || Math.Ceiling(CurrentSheet.Length) != Convert.ToDouble(SheetSize.Split(',')[1]))
                    {
                        continue;
                    }

                    if (i == 0)
                    {
                        #region 處理料號(以客戶版次做判斷)
                        foreach (KeyValuePair<string, string> kvp in DicPartNumberPos)
                        {
                            string PartNumCusRev = "";
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            PartNumCusRev = kvp.Value + "(" + CusRevText.Text + ")";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, PartNumCusRev, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, PartNumCusRev, FontSize, TextPt);
                        }

                        /*
                        foreach (KeyValuePair<string, string> kvp in DicPartNumberPos)
                        {
                            string PartNumCusRev = "";
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                }
                                if (NoteValue == "")
                                {
                                    Point3d TextPt = new Point3d();
                                    string FontSize = "";
                                    PartNumCusRev = kvp.Value + "(" + CusRevText.Text + ")";
                                    Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, PartNumCusRev, out TextPt, out FontSize);
                                    Functions.InsertNote(kvp.Key, PartNumCusRev, FontSize, TextPt);
                                }
                            }
                            else
                            {
                                Point3d TextPt = new Point3d();
                                string FontSize = "";
                                PartNumCusRev = kvp.Value + "(" + CusRevText.Text + ")";
                                Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, PartNumCusRev, out TextPt, out FontSize);
                                Functions.InsertNote(kvp.Key, PartNumCusRev, FontSize, TextPt);
                            }
                        }
                        */
                        #endregion

                        #region 處理品名
                        foreach (KeyValuePair<string, string> kvp in DicPartDescriptionPos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                        }
                        #endregion

                        #region 處理單位
                        foreach (KeyValuePair<string, string> kvp in DicPartUnitPos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                        }
                        #endregion

                        #region 處理材質
                        foreach (KeyValuePair<string, string> kvp in DicMaterialPos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                        }
                        #endregion

                        //判斷零件屬性中的版次與對話框的版次是否相同，如果不同表示要新增
                        List<string> ListRev = new List<string>();
                        foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                        {
                            try
                            {
                                ListRev.Add(singleNote.GetStringAttribute(TablePosi.RevStartPos));
                            }
                            catch (System.Exception ex)
                            {
                            	continue;
                            }
                        }

                        int compareCount = 0;
                        foreach (string singleRev in ListRev)
                        {
                            if (DicRevStartPos[TablePosi.RevStartPos] != singleRev)
                            {
                                compareCount++;
                            }
                        }

                        if (compareCount == ListRev.Count)
                        {
                            #region 處理製圖版次
                            foreach (KeyValuePair<string, string> kvp in DicRevStartPos)
                            {
                                string NoteValue = "";
                                int DraftingCount = 0;
                                if (NotesAry.Length != 0)
                                {
                                    foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                    {
                                        try
                                        {
                                            NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                            if (NoteValue == kvp.Value)
                                            {
                                                break;
                                            }
                                            DraftingCount++;
                                        }
                                        catch (System.Exception ex)
                                        {
                                            continue;
                                        }
                                    }
                                }
                                if (NoteValue == kvp.Value)
                                {
                                    break;
                                }

                                Point3d TextPt = new Point3d();
                                string FontSize = "";
                                Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                                if (DraftingCount != 0)
                                {
                                    TextPt.Y = TextPt.Y + (Convert.ToDouble(RevRowHeight) * DraftingCount);
                                }
                                Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                            }
                            #endregion

                            #region 處理製圖版次日期
                            foreach (KeyValuePair<string, string> kvp in DicRevDateStartPos)
                            {
                                int DraftingCount = 0;
                                if (NotesAry.Length != 0)
                                {
                                    string NoteValue = "";
                                    foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                    {
                                        try
                                        {
                                            NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                            DraftingCount++;
                                        }
                                        catch (System.Exception ex)
                                        {
                                            continue;
                                        }
                                    }
                                }

                                Point3d TextPt = new Point3d();
                                string FontSize = "";
                                Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                                if (DraftingCount != 0)
                                {
                                    TextPt.Y = TextPt.Y + (Convert.ToDouble(RevRowHeight) * DraftingCount);
                                }
                                Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                            }
                            #endregion
                        }




                        /*
                        string DraftingAttri = "";
                        try
                        {
                            DraftingAttri = workPart.GetStringAttribute(TablePosi.RevStartPos);
                        }
                        catch (System.Exception ex)
                        {
                            DraftingAttri = "";
                        }

                        if (DicRevStartPos[TablePosi.RevStartPos] != DraftingAttri || DraftingAttri == "")
                        {
                            #region 處理製圖版次
                            foreach (KeyValuePair<string, string> kvp in DicRevStartPos)
                            {
                                string NoteValue = "";
                                int DraftingCount = 0;
                                if (NotesAry.Length != 0)
                                {
                                    foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                    {
                                        try
                                        {
                                            NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                            if (NoteValue == kvp.Value)
                                            {
                                                break;
                                            }
                                            DraftingCount++;
                                        }
                                        catch (System.Exception ex)
                                        {
                                            continue;
                                        }
                                    }
                                }
                                if (NoteValue == kvp.Value)
                                {
                                    break;
                                }

                                Point3d TextPt = new Point3d();
                                string FontSize = "";
                                Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                                if (DraftingCount != 0)
                                {
                                    TextPt.Y = TextPt.Y + (Convert.ToDouble(RevRowHeight) * DraftingCount);
                                }
                                Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                            }
                            #endregion

                            #region 處理製圖版次日期
                            foreach (KeyValuePair<string, string> kvp in DicRevDateStartPos)
                            {
                                int DraftingCount = 0;
                                if (NotesAry.Length != 0)
                                {
                                    string NoteValue = "";
                                    foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                    {
                                        try
                                        {
                                            NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                            DraftingCount++;
                                        }
                                        catch (System.Exception ex)
                                        {
                                            continue;
                                        }
                                    }
                                }

                                Point3d TextPt = new Point3d();
                                string FontSize = "";
                                Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                                if (DraftingCount != 0)
                                {
                                    TextPt.Y = TextPt.Y + (Convert.ToDouble(RevRowHeight) * DraftingCount);
                                }
                                Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                            }
                            #endregion
                        }
                        */
                        
                        #region 處理出圖日期
                        foreach (KeyValuePair<string, string> kvp in DicAuthDatePos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                }
                                if (NoteValue == "")
                                {
                                    Point3d TextPt = new Point3d();
                                    string FontSize = "";
                                    Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                                    Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                                }
                            }
                            else
                            {
                                Point3d TextPt = new Point3d();
                                string FontSize = "";
                                Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                                Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                            }
                        }
                        #endregion

                        #region 處理頁數
                        foreach (KeyValuePair<string, string> kvp in DicPageNumberPos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                        }
                        #endregion

                        #region 處理TolTitle0
                        foreach (KeyValuePair<string, string> kvp in DicTolTitle0Pos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt, NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidLeft);
                        }
                        #endregion

                        #region 處理TolTitle1
                        foreach (KeyValuePair<string, string> kvp in DicTolTitle1Pos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt, NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidLeft);
                        }
                        #endregion

                        #region 處理TolTitle2
                        foreach (KeyValuePair<string, string> kvp in DicTolTitle2Pos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt, NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidLeft);
                        }
                        #endregion

                        #region 處理TolTitle3
                        foreach (KeyValuePair<string, string> kvp in DicTolTitle3Pos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt, NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidLeft);
                        }
                        #endregion

                        #region 處理TolValue0
                        foreach (KeyValuePair<string, string> kvp in DicTolValue0Pos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt, NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidLeft);
                        }
                        #endregion

                        #region 處理TolValue1
                        foreach (KeyValuePair<string, string> kvp in DicTolValue1Pos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt, NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidLeft);
                        }
                        #endregion

                        #region 處理TolValue2
                        foreach (KeyValuePair<string, string> kvp in DicTolValue2Pos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt, NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidLeft);
                        }
                        #endregion

                        #region 處理TolValue3
                        foreach (KeyValuePair<string, string> kvp in DicTolValue3Pos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt, NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidLeft);
                        }
                        #endregion

                    }
                    else
                    {
                        #region 處理料號
                        foreach (KeyValuePair<string, string> kvp in DicSecondPartNumberPos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                }
                                if (NoteValue == "")
                                {
                                    Point3d TextPt = new Point3d();
                                    string FontSize = "";
                                    Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                                    Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                                }
                            }
                            else
                            {
                                Point3d TextPt = new Point3d();
                                string FontSize = "";
                                Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                                Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                            }
                        }
                        #endregion

                        #region 處理頁數
                        foreach (KeyValuePair<string, string> kvp in DicSecondPageNumberPos)
                        {
                            if (NotesAry.Length != 0)
                            {
                                string NoteValue = "";
                                foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                                {
                                    try
                                    {
                                        NoteValue = singleNote.GetStringAttribute(kvp.Key);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        continue;
                                    }
                                    if (NoteValue != "")
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }
                                }
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                        }
                        #endregion
                    }
                    
                    /*
                    foreach (KeyValuePair<string, string> kvp in DicPartInformation)
                    {
                        if (i == 0 && (kvp.Key == TablePosi.SecondPageNumberPos || kvp.Key == TablePosi.SecondPartNumberPos))
                        {
                            continue;
                        }
                        else if (i != 0 && kvp.Key != TablePosi.SecondPageNumberPos && kvp.Key != TablePosi.SecondPartNumberPos)
                        {
                            continue;
                        }

                        if (NotesAry.Length != 0)
                        {
                            #region 處理有Note但都沒有屬性的CASE
                            int HasAttriCount = 0;
                            foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                            {
                                string NoteAttrValue = "";
                                try
                                {
                                    NoteAttrValue = singleNote.GetStringAttribute(kvp.Key);
                                }
                                catch (System.Exception ex)
                                {
                                    HasAttriCount++;
                                    continue;
                                }
                            }
                            if (HasAttriCount == NotesAry.Length)
                            {
                                //如果Note沒有屬性，則相當於一開始狀態
                                Point3d TextPt = new Point3d();
                                string FontSize = "";
                                Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                                Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                                break;
                            }
                            #endregion

                            #region 處理有Note且有屬性的CASE
                            foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                            {
                                string NoteAttrValue = "";
                                try
                                {
                                    NoteAttrValue = singleNote.GetStringAttribute(kvp.Key);
                                }
                                catch (System.Exception ex)
                                {
                                    continue;
                                }

                                if (NoteAttrValue == kvp.Value)
                                {

                                }
                            }
                            #endregion
                        }
                        else
                        {
                            //Point3d TextPt = new Point3d();
                            //string FontSize = "";
                            //Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            //Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                        }
                        
                        if (NotesAry.Length != 0)
                        {
                            int count0 = 0;
                            foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                            {
                                string NoteAttr = "";
                                try
                                {
                                    NoteAttr = singleNote.GetStringAttribute("GlobalTek"); 
                                }
                                catch (System.Exception ex)
                                {
                                    count0++;
                                    continue;
                                }

                                //如果所有的Note都不是系統給的，則相當於一開始狀態
                                if (count0 == NotesAry.Length)
                                {
                                    Point3d TextPt = new Point3d();
                                    string FontSize = "";
                                    Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                                    Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                                    break;
                                }
                                else
                                {
                                    //當有舊資料，且是料號、出圖日期、單位時，則不做事情
                                    if ((NoteAttr == TablePosi.PartNumberPos && NoteAttr == kvp.Key) ||
                                          (NoteAttr == TablePosi.AuthDatePos && NoteAttr == kvp.Key) ||
                                          (NoteAttr == TablePosi.PartUnitPos && NoteAttr == kvp.Key))
                                    {
                                        break;
                                    }
                                    else if ((NoteAttr == TablePosi.RevStartPos && NoteAttr == kvp.Key) ||
                                               (NoteAttr == TablePosi.RevDateStartPos && NoteAttr == kvp.Key))
                                    {

                                    }
                                    else if (NoteAttr != kvp.Key)
                                    {
                                        continue;
                                    }
                                    else if (NoteAttr == kvp.Key)
                                    {
                                        CaxPublic.DelectObject(singleNote);
                                    }

                                    Point3d TextPt = new Point3d();
                                    string FontSize = "";
                                    Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                                    //(有舊資料)如果是製圖版次資料，則增加高度位置
                                    if (kvp.Key == TablePosi.RevStartPos || kvp.Key == TablePosi.RevDateStartPos)
                                    {
                                        TextPt.Y = TextPt.Y + Convert.ToDouble(RevRowHeight);
                                    }
                                    Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                                }
                            }
                        }
                        else
                        {
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                        }
                        

                        if (NotesAry.Length != 0)
                        {
                            foreach (NXOpen.Annotations.Note singleNote in NotesAry)
                            {
                                string NoteAttr = "";
                                try
                                {
                                    NoteAttr = singleNote.GetStringAttribute("GlobalTek");
                                }
                                catch (System.Exception ex)
                                {
                                    continue;
                                }

                                //當有舊資料，且是料號、出圖日期、單位時，則不做事情
                                if (  (NoteAttr == TablePosi.PartNumberPos && NoteAttr == kvp.Key) ||
                                      (NoteAttr == TablePosi.AuthDatePos && NoteAttr == kvp.Key) || 
                                      (NoteAttr == TablePosi.PartUnitPos && NoteAttr == kvp.Key))
                                {
                                    break;
                                }
                                else if ((NoteAttr == TablePosi.RevStartPos && NoteAttr == kvp.Key) ||
                                    (NoteAttr == TablePosi.RevDateStartPos && NoteAttr == kvp.Key) )
                                {

                                }
                                else
                                {
                                    CaxPublic.DelectObject(singleNote);
                                }

                                Point3d TextPt = new Point3d();
                                string FontSize = "";
                                Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                                //(有舊資料)如果是製圖版次資料，則增加高度位置
                                if (kvp.Key == TablePosi.RevStartPos || kvp.Key == TablePosi.RevDateStartPos)
                                {
                                    TextPt.Y = TextPt.Y + Convert.ToDouble(RevRowHeight);
                                }
                                Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                            }
                        }
                        else
                        {
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            Functions.GetTextPos(cDraftingConfig, ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            Functions.InsertNote(kvp.Key, kvp.Value, FontSize, TextPt);
                        }
                        
                    }
                    */

                }

            }

            //塞入屬性
            foreach (KeyValuePair<string,string> kvp in DicPartNumberPos)
            {
                workPart.SetAttribute(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, string> kvp in DicPartDescriptionPos)
            {
                workPart.SetAttribute(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, string> kvp in DicPartUnitPos)
            {
                workPart.SetAttribute(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, string> kvp in DicAuthDatePos)
            {
                workPart.SetAttribute(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, string> kvp in DicMaterialPos)
            {
                workPart.SetAttribute(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, string> kvp in DicRevStartPos)
            {
                workPart.SetAttribute(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, string> kvp in DicRevDateStartPos)
            {
                workPart.SetAttribute(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, string> kvp in DicCusRevPos)
            {
                workPart.SetAttribute(kvp.Key, kvp.Value);
            }
        }

        private void PlusMinus_Click(object sender, EventArgs e)
        {
            symbol = PlusMinus.Tag.ToString();
            NoteBox.Text = NoteBox.Text.Insert(NoteBox.SelectionStart, symbol);
            NoteBox.Select(NoteBox.Text.Length, 0);
        }

        private void Angle_Click(object sender, EventArgs e)
        {
            symbol = Angle.Tag.ToString();
            NoteBox.Text = NoteBox.Text.Insert(NoteBox.SelectionStart, symbol);
            NoteBox.Select(NoteBox.Text.Length, 0);
        }

        private void UserAddNote_Click(object sender, EventArgs e)
        {
            //string AddNoteStr = "";
            //NoteBox.Text = NoteBox.Text.Replace(PlusMinus.Tag.ToString().Split(',')[0], PlusMinus.Tag.ToString().Split(',')[1]);
            //AddNoteStr = AddNoteStr.Replace(Diameter.Tag.ToString().Split(',')[0], Diameter.Tag.ToString().Split(',')[1]);
            Tag currentsheet;
            theUfSession.Draw.AskCurrentDrawing(out currentsheet);
            NXOpen.Drawings.DrawingSheet CurrentSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(currentsheet);

            Point3d UserNotePos = new Point3d(CurrentSheet.Length, CurrentSheet.Height, 0);
            Functions.InsertNote("UserAddNote", NoteBox.Text, "3", UserNotePos);

        }

        private void Diameter_Click(object sender, EventArgs e)
        {
            symbol = Diameter.Tag.ToString();
            NoteBox.Text = NoteBox.Text.Insert(NoteBox.SelectionStart, symbol);
        }

        private void ConicalTaper_Click(object sender, EventArgs e)
        {
            symbol = ConicalTaper.Tag.ToString();
            NoteBox.Text = NoteBox.Text.Insert(NoteBox.SelectionStart, symbol);
        }

        private void Slope_Click(object sender, EventArgs e)
        {
            symbol = Slope.Tag.ToString();
            NoteBox.Text = NoteBox.Text.Insert(NoteBox.SelectionStart, symbol);
        }

        private void chb_Point_CheckedChanged(object sender, EventArgs e)
        {
            if (chb_Point.Checked == true)
            {
                chb_Region.Checked = false;
                TolTitle0.Text = "X.";
                TolTitle1.Text = " .X";
                TolTitle2.Text = " .XX";
            }
        }

        private void chb_Region_CheckedChanged(object sender, EventArgs e)
        {
            if (chb_Region.Checked == true)
            {
                chb_Point.Checked = false;
                TolTitle0.Text = "";
                TolTitle1.Text = "";
                TolTitle2.Text = "";
            }
        }

        


    }
}
