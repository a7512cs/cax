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
        public static string RevRowHeight = "";

        public class TablePosi
        {
            public string PartNumberPos = "PartNumberPos";
            public string PartDescriptionPos = "PartDescriptionPos";
            public string RevStartPos = "RevStartPos";
            public string PartUnitPos = "PartUnitPos";
            public string TolTitle0Pos = "TolTitle0Pos";
            public string TolTitle1Pos = "TolTitle1Pos";
            public string TolTitle2Pos = "TolTitle2Pos";
            public string TolTitle3Pos = "TolTitle3Pos";
            public string TolTitle4Pos = "TolTitle4Pos";
            public string TolValue0Pos = "TolValue0Pos";
            public string TolValue1Pos = "TolValue1Pos";
            public string TolValue2Pos = "TolValue2Pos";
            public string TolValue3Pos = "TolValue3Pos";
            public string TolValue4Pos = "TolValue4Pos";
            public string RevDateStartPos = "RevDateStartPos";
            public string AuthDatePos = "AuthDatePos";
            public string MaterialPos = "MaterialPos";
            public string ProcNamePos = "ProcNamePos";
            public string PageNumberPos = "PageNumberPos";
            public string SecondPartNumberPos = "SecondPartNumberPos";
            public string SecondPageNumberPos = "SecondPageNumberPos";
        }

        public PartInformationDlg()
        {
            InitializeComponent();
        }

        private void PartInformationDlg_Load(object sender, EventArgs e)
        {
            //取得DraftingData
            CaxGetDatData.GetDraftingConfigData(out cDraftingConfig);

            //預設開啟範圍區分checkbox
            chb_Region.Checked = true;

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

            //載入圖片
            
        }

        private void OK_Click(object sender, EventArgs e)
        {
            //抓取目前圖紙數量和Tag
            int SheetCount = 0;
            NXOpen.Tag[] SheetTagAry = null;
            theUfSession.Draw.AskDrawings(out SheetCount, out SheetTagAry);
            //NXOpen.Drawings.DrawingSheet aaa = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(SheetTagAry[0]);
            //NXOpen.Drawings.DrawingSheet bb = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(SheetTagAry[1]);

            TablePosi cTablePosi = new TablePosi();
            DicPartInformation = new Dictionary<string, string>();

            for (int i = 0; i < SheetCount;i++)
            {
                if (i==0)
                {
                    DicPartInformation.Add(cTablePosi.PartNumberPos, PartNumberText.Text);
                    DicPartInformation.Add(cTablePosi.PartDescriptionPos, PartDescriptionText.Text);
                    DicPartInformation.Add(cTablePosi.PartUnitPos, PartUnitText.Text);
                    DicPartInformation.Add(cTablePosi.RevStartPos, DraftingRevText.Text);
                    DicPartInformation.Add(cTablePosi.RevDateStartPos, DateTime.Now.ToShortDateString());
                    DicPartInformation.Add(cTablePosi.AuthDatePos, DateTime.Now.ToShortDateString());
                    DicPartInformation.Add(cTablePosi.MaterialPos, MaterialText.Text);
                    DicPartInformation.Add(cTablePosi.PageNumberPos, "1/" + SheetCount.ToString());
                }
                else
                {
                    bool check = false;
                    string tempSecondPartNumberPos = "", tempSecondPageNumberPos = "";

                    check = DicPartInformation.TryGetValue(cTablePosi.SecondPartNumberPos, out tempSecondPartNumberPos);
                    if (!check)
                    {
                        DicPartInformation.Add(cTablePosi.SecondPartNumberPos, PartNumberText.Text);
                    }
                    

                    //check = DicPartInformation.TryGetValue(cTablePosi.SecondPageNumberPos, out tempSecondPageNumberPos);
                    //if (check)
                    //{
                    //    tempSecondPageNumberPos = tempSecondPageNumberPos + "," + (i + 1).ToString() + "/" + SheetCount.ToString();
                    //    DicPartInformation[cTablePosi.SecondPageNumberPos] = tempSecondPageNumberPos;
                    //}
                    //else
                    //{
                    //    DicPartInformation.Add(cTablePosi.SecondPageNumberPos, (i + 1).ToString() + "/" + SheetCount.ToString());
                    //}
                }
            }

            for (int i = 0; i < SheetCount; i++)
            {
                //輪巡每個Sheet
                NXOpen.Drawings.DrawingSheet CurrentSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(SheetTagAry[i]);
                NXOpen.Drawings.DrawingSheet drawingSheet1 = (NXOpen.Drawings.DrawingSheet)workPart.DrawingSheets.FindObject(CurrentSheet.Name);
                drawingSheet1.Open();

                //寫入頁數資訊(加入sheet2以上)
                if (i>0)
                {
                    string tempString = (i + 1).ToString() + "/" + SheetCount.ToString();
                    DicPartInformation[cTablePosi.SecondPageNumberPos] = tempString;
                }

                for (int ii = 0; ii < cDraftingConfig.Drafting.Count; ii++)
                {
                    string SheetSize = cDraftingConfig.Drafting[ii].SheetSize;
                    if (Math.Ceiling(CurrentSheet.Height) == Convert.ToDouble(SheetSize.Split(',')[0]) && Math.Ceiling(CurrentSheet.Length) == Convert.ToDouble(SheetSize.Split(',')[1]))
                    {
                        foreach (KeyValuePair<string, string> kvp in DicPartInformation)
                        {
                            if (i == 0 && (kvp.Key == cTablePosi.SecondPageNumberPos || kvp.Key == cTablePosi.SecondPartNumberPos))
                            {
                                continue;
                            }
                            else if (i != 0 && kvp.Key != cTablePosi.SecondPageNumberPos &&  kvp.Key != cTablePosi.SecondPartNumberPos)
                            {
                                continue;
                            }
                            Point3d TextPt = new Point3d();
                            string FontSize = "";
                            GetTextPos(ii, kvp.Key, kvp.Value, out TextPt, out FontSize);
                            InsertNote(kvp.Value, TextPt, FontSize);
                        }
                    }
                }

            }

            //塞入屬性
            foreach (KeyValuePair<string,string> kvp in DicPartInformation)
            {
                if (kvp.Key == cTablePosi.SecondPageNumberPos || kvp.Key == cTablePosi.SecondPartNumberPos)
                {
                    continue;
                }
                workPart.SetAttribute(kvp.Key, kvp.Value);
            }

            //Tag b;
            //theUfSession.Draw.AskCurrentDrawing(out b);
            //NXOpen.Drawings.DrawingSheet aa = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(b);

            //for (int i = 0; i < cDraftingConfig.Drafting.Count;i++ )
            //{
            //    string SheetSize = cDraftingConfig.Drafting[i].SheetSize;
            //    if (Math.Ceiling(aa.Height) == Convert.ToDouble(SheetSize.Split(',')[0]) && Math.Ceiling(aa.Length) == Convert.ToDouble(SheetSize.Split(',')[1]))
            //    {
            //        foreach (KeyValuePair<string,string> kvp in DicPartInformation)
            //        {
            //            Point3d TextPt = new Point3d();
            //            string FontSize = "";
            //            GetTextPos(i, kvp.Key, kvp.Value, out TextPt, out FontSize);
            //            InsertNote(kvp.Value, TextPt, FontSize);
            //        }
            //    }
            //}

        }

        public static bool InsertNote(string text, Point3d textLocation, string FontSize)
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Annotation->Note...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");
                NXOpen.Annotations.SimpleDraftingAid nullAnnotations_SimpleDraftingAid = null;
                NXOpen.Annotations.DraftingNoteBuilder draftingNoteBuilder1;
                draftingNoteBuilder1 = workPart.Annotations.CreateDraftingNoteBuilder(nullAnnotations_SimpleDraftingAid);
                draftingNoteBuilder1.Origin.Plane.PlaneMethod = NXOpen.Annotations.PlaneBuilder.PlaneMethodType.XyPlane;
                draftingNoteBuilder1.Origin.SetInferRelativeToGeometry(false);
                draftingNoteBuilder1.Origin.SetInferRelativeToGeometry(true);
                draftingNoteBuilder1.Origin.Anchor = NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidCenter;

                //text文字
                string[] text1 = new string[1];
                //text1[0] = "<C2>" + text + "<C>";
                text1[0] = text;
                draftingNoteBuilder1.Text.TextBlock.SetText(text1);

                draftingNoteBuilder1.Style.LetteringStyle.GeneralTextSize = Convert.ToDouble(FontSize);

                theSession.SetUndoMarkName(markId1, "Note Dialog");
                draftingNoteBuilder1.Origin.Plane.PlaneMethod = NXOpen.Annotations.PlaneBuilder.PlaneMethodType.XyPlane;
                draftingNoteBuilder1.Origin.SetInferRelativeToGeometry(true);
                //NXOpen.Annotations.LeaderData leaderData1;
                //leaderData1 = workPart.Annotations.CreateLeaderData();
                //leaderData1.StubSize = 5.0;
                //leaderData1.Arrowhead = NXOpen.Annotations.LeaderData.ArrowheadType.FilledArrow;
                //draftingNoteBuilder1.Leader.Leaders.Append(leaderData1);
                //leaderData1.StubSide = NXOpen.Annotations.LeaderSide.Inferred;
                double symbolscale1;
                symbolscale1 = draftingNoteBuilder1.Text.TextBlock.SymbolScale;
                double symbolaspectratio1;
                symbolaspectratio1 = draftingNoteBuilder1.Text.TextBlock.SymbolAspectRatio;
                draftingNoteBuilder1.Origin.SetInferRelativeToGeometry(true);
                draftingNoteBuilder1.Origin.SetInferRelativeToGeometry(true);
                NXOpen.Annotations.Annotation.AssociativeOriginData assocOrigin1;
                assocOrigin1.OriginType = NXOpen.Annotations.AssociativeOriginType.Drag;
                NXOpen.View nullView = null;
                assocOrigin1.View = nullView;
                assocOrigin1.ViewOfGeometry = nullView;
                NXOpen.Point nullPoint = null;
                assocOrigin1.PointOnGeometry = nullPoint;
                assocOrigin1.VertAnnotation = null;
                assocOrigin1.VertAlignmentPosition = NXOpen.Annotations.AlignmentPosition.TopLeft;
                assocOrigin1.HorizAnnotation = null;
                assocOrigin1.HorizAlignmentPosition = NXOpen.Annotations.AlignmentPosition.TopLeft;
                assocOrigin1.AlignedAnnotation = null;
                assocOrigin1.DimensionLine = 0;
                assocOrigin1.AssociatedView = nullView;
                assocOrigin1.AssociatedPoint = nullPoint;
                assocOrigin1.OffsetAnnotation = null;
                assocOrigin1.OffsetAlignmentPosition = NXOpen.Annotations.AlignmentPosition.TopLeft;
                assocOrigin1.XOffsetFactor = 0.0;
                assocOrigin1.YOffsetFactor = 0.0;
                assocOrigin1.StackAlignmentPosition = NXOpen.Annotations.StackAlignmentPosition.Above;
                draftingNoteBuilder1.Origin.SetAssociativeOrigin(assocOrigin1);

                //text擺放位置
                draftingNoteBuilder1.Origin.Origin.SetValue(null, nullView, textLocation);

                draftingNoteBuilder1.Origin.SetInferRelativeToGeometry(true);
                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Note");
                NXObject nXObject1;
                nXObject1 = draftingNoteBuilder1.Commit();
                theSession.DeleteUndoMark(markId2, null);
                theSession.SetUndoMarkName(markId1, "Note");
                theSession.SetUndoMarkVisibility(markId1, null, NXOpen.Session.MarkVisibility.Visible);
                draftingNoteBuilder1.Destroy();


                // ----------------------------------------------
                //   Menu: Tools->Journal->Stop Recording
                // ----------------------------------------------
            }
            catch (System.Exception ex)
            {
                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ex.Message);
                return false;
            }
            return true;
        }

        public static bool GetTextPos(int i, string KeyToCompare, string ValueToCompare, out Point3d TextPos, out string FontSize)
        {
            TextPos = new Point3d();
            FontSize = "";
            try
            {
                string ptStr = "";
                if (cDraftingConfig.Drafting[i].PageNumberPosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].PageNumberPos;
                    FontSize = cDraftingConfig.Drafting[i].PageNumberFontSize;
                }
                else if (cDraftingConfig.Drafting[i].PartDescriptionPosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].PartDescriptionPos;
                    FontSize = cDraftingConfig.Drafting[i].PartDescriptionFontSize;
                }
                else if (cDraftingConfig.Drafting[i].AuthDatePosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].AuthDatePos;
                    FontSize = cDraftingConfig.Drafting[i].AuthDateFontSize;
                }
                else if (cDraftingConfig.Drafting[i].MaterialPosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].MaterialPos;
                    int Subtraction = ValueToCompare.Length - 5;
                    if (Subtraction <= 0)
                    {
                        FontSize = cDraftingConfig.Drafting[i].MaterialFontSize;
                    }
                    else
                    {
                        FontSize = (Convert.ToDouble(cDraftingConfig.Drafting[i].MaterialFontSize) - (Convert.ToDouble(cDraftingConfig.Drafting[i].MatMinFontSize) * Subtraction)).ToString();
                        if (Convert.ToDouble(FontSize) <= 0)
                        {
                            FontSize = cDraftingConfig.Drafting[i].MatMinFontSize;
                        }
                    }
                }
                else if (cDraftingConfig.Drafting[i].PartNumberPosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].PartNumberPos;
                    FontSize = cDraftingConfig.Drafting[i].PartNumberFontSize;
                }
                else if (cDraftingConfig.Drafting[i].PartUnitPosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].PartUnitPos;
                    FontSize = cDraftingConfig.Drafting[i].PartUnitFontSize;
                }
                else if (cDraftingConfig.Drafting[i].ProcNamePosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].ProcNamePos;
                    FontSize = cDraftingConfig.Drafting[i].ProcNameFontSize;
                }
                else if (cDraftingConfig.Drafting[i].RevDateStartPosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].RevDateStartPos;
                    FontSize = cDraftingConfig.Drafting[i].RevDateFontSize;
                }
                else if (cDraftingConfig.Drafting[i].RevStartPosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].RevStartPos;
                    FontSize = cDraftingConfig.Drafting[i].RevFontSize;
                }
                else if (cDraftingConfig.Drafting[i].SecondPageNumberPosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].SecondPageNumberPos;
                    FontSize = cDraftingConfig.Drafting[i].PageNumberFontSize;
                }
                else if (cDraftingConfig.Drafting[i].SecondPartNumberPosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].SecondPartNumberPos;
                    FontSize = cDraftingConfig.Drafting[i].PartNumberFontSize;
                }
                else if (cDraftingConfig.Drafting[i].TolTitle0PosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].TolTitle0Pos;
                    FontSize = cDraftingConfig.Drafting[i].TolFontSize;
                }
                else if (cDraftingConfig.Drafting[i].TolTitle1PosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].TolTitle1Pos;
                    FontSize = cDraftingConfig.Drafting[i].TolFontSize;
                }
                else if (cDraftingConfig.Drafting[i].TolTitle2PosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].TolTitle2Pos;
                    FontSize = cDraftingConfig.Drafting[i].TolFontSize;
                }
                else if (cDraftingConfig.Drafting[i].TolTitle3PosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].TolTitle3Pos;
                    FontSize = cDraftingConfig.Drafting[i].TolFontSize;
                }
                else if (cDraftingConfig.Drafting[i].TolTitle4PosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].TolTitle4Pos;
                    FontSize = cDraftingConfig.Drafting[i].TolFontSize;
                }
                else if (cDraftingConfig.Drafting[i].TolValue0PosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].TolValue0Pos;
                    FontSize = cDraftingConfig.Drafting[i].TolFontSize;
                }
                else if (cDraftingConfig.Drafting[i].TolValue1PosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].TolValue1Pos;
                    FontSize = cDraftingConfig.Drafting[i].TolFontSize;
                }
                else if (cDraftingConfig.Drafting[i].TolValue2PosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].TolValue2Pos;
                    FontSize = cDraftingConfig.Drafting[i].TolFontSize;
                }
                else if (cDraftingConfig.Drafting[i].TolValue3PosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].TolValue3Pos;
                    FontSize = cDraftingConfig.Drafting[i].TolFontSize;
                }
                else if (cDraftingConfig.Drafting[i].TolValue4PosText == KeyToCompare)
                {
                    ptStr = cDraftingConfig.Drafting[i].TolValue4Pos;
                    FontSize = cDraftingConfig.Drafting[i].TolFontSize;
                }
                TextPos.X = Convert.ToDouble(ptStr.Split(',')[0]);
                TextPos.Y = Convert.ToDouble(ptStr.Split(',')[1]);
                TextPos.Z = Convert.ToDouble(ptStr.Split(',')[2]);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UserAddNote_Click(object sender, EventArgs e)
        {
            string aa = "<$t>0.05";
            Point3d bb = new Point3d();
            InsertNote(aa, bb,"3");
        }
    }
}
