using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using CaxGlobaltek;

namespace PartInformation
{
    public class Functions
    {
        //public static DraftingConfig cDraftingConfig = new DraftingConfig();

        /// <summary>
        /// 單筆資料插入
        /// </summary>
        /// <param name="attrStr"></param>
        /// <param name="text"></param>
        /// <param name="FontSize"></param>
        /// <param name="textLocation"></param>
        /// <param name="defult"></param>
        /// <returns></returns>
        public static bool InsertNote(string attrStr, string text, string FontSize, Point3d textLocation, NXOpen.Annotations.OriginBuilder.AlignmentPosition defult = NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidCenter)
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
                draftingNoteBuilder1.Origin.Anchor = defult;
                
                //text文字
                workPart.Fonts.AddFont("chineset");
                string[] text1 = new string[1];
                //text1[0] = "<C2>" + text + "<C>";
                
                text1[0] = "<F2>" + text + "<F>";
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
                //塞屬性，以利重複執行時抓取資料
                nXObject1.SetAttribute(attrStr, text);
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

        /// <summary>
        /// 多筆資料插入
        /// </summary>
        /// <param name="attrStr"></param>
        /// <param name="text"></param>
        /// <param name="FontSize"></param>
        /// <param name="textLocation"></param>
        /// <param name="defult"></param>
        /// <returns></returns>
        public static bool InsertNote(string attrStr, string[] text, string FontSize, Point3d textLocation, NXOpen.Annotations.OriginBuilder.AlignmentPosition defult = NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidCenter)
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
                draftingNoteBuilder1.Origin.Anchor = defult;

                //text文字
                workPart.Fonts.AddFont("chineset");
                string[] text1 = new string[text.Length];
                //text1[0] = "<C2>" + text + "<C>";
                for (int i = 0; i < text.Length;i++ )
                {
                    text1[i] = "<F2>" + text[i] + "<F>";
                }
                //text1[0] = "<F18>" + text + "<F>";
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
                //塞屬性，以利重複執行時抓取資料
                nXObject1.SetAttribute("Createdby","CAX");
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

        public static bool GetTextPos(DraftingConfig cDraftingConfig, int i, string KeyToCompare, string ValueToCompare, out Point3d TextPos, out string FontSize)
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
                    int Subtraction = ValueToCompare.Length - 15;
                    if (Subtraction <= 0)
                    {
                        FontSize = cDraftingConfig.Drafting[i].PartNumberFontSize;
                    }
                    else
                    {
                        FontSize = (Convert.ToDouble(cDraftingConfig.Drafting[i].PartNumberFontSize) - (Convert.ToDouble(cDraftingConfig.Drafting[i].MatMinFontSize) * Subtraction)).ToString();
                        
                        if (Convert.ToDouble(FontSize) <= 0)
                        {
                            FontSize = cDraftingConfig.Drafting[i].MatMinFontSize;
                        }
                    }
                    //FontSize = cDraftingConfig.Drafting[i].PartNumberFontSize;
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
                    int Subtraction = ValueToCompare.Length - 15;
                    if (Subtraction <= 0)
                    {
                        FontSize = cDraftingConfig.Drafting[i].PartNumberFontSize;
                    }
                    else
                    {
                        FontSize = (Convert.ToDouble(cDraftingConfig.Drafting[i].PartNumberFontSize) - (Convert.ToDouble(cDraftingConfig.Drafting[i].MatMinFontSize) * Subtraction)).ToString();
                        if (Convert.ToDouble(FontSize) <= 0)
                        {
                            FontSize = cDraftingConfig.Drafting[i].MatMinFontSize;
                        }
                    }
                    //FontSize = cDraftingConfig.Drafting[i].PartNumberFontSize;
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
    }
}
