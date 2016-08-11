using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using NXOpen.UF;
using NXOpen.Annotations;
using NXOpen.Drawings;

namespace CaxGlobaltek
{
    public class CaxME
    {
        public static Session theSession = Session.GetSession();
        public static UI theUI;
        public static UFSession theUfSession = UFSession.GetUFSession();
        public static Part workPart = theSession.Parts.Work;
        public static Part displayPart = theSession.Parts.Display;

        public class DimenAttr
        {
            public static string OldColor = "OldColor";
            public static string IPQC_Gauge = "IPQC_Gauge";
            public static string IPQC_Freq = "IPQC_Freq";
            public static string SelfCheck_Gauge = "SelfCheck_Gauge";
            public static string SelfCheck_Freq = "SelfCheck_Freq";
            public static string BallonNum = "BallonNum";
            public static string BallonLocation = "BallonLocation";
        }

        public class BoxCoordinate
        {
            public double[] upper_left = new double[3] { 0, 0, 0 };
            public double[] lower_left = new double[3] { 0, 0, 0 };
            public double[] lower_right = new double[3] { 0, 0, 0 };
            public double[] upper_right = new double[3] { 0, 0, 0 };
        }

        public struct FcfData
        {
            public string Characteristic { get; set; }
            public string ZoneShape { get; set; }
            public string ToleranceValue { get; set; }
            public string MaterialModifier { get; set; }
            public string PrimaryDatum { get; set; }
            public string PrimaryMaterialModifier { get; set; }
            public string SecondaryDatum { get; set; }
            public string SecondaryMaterialModifier { get; set; }
            public string TertiaryDatum { get; set; }
            public string TertiaryMaterialModifier { get; set; }
        }

        /// <summary>
        /// 回傳標註尺寸的顏色
        /// </summary>
        /// <param name="nNXObject"></param>
        /// <returns></returns>
        public static int GetDimensionColor(NXObject nNXObject)
        {
            int oldColor = -1;
            try
            {
                string DimenType = nNXObject.GetType().ToString();
                if (DimenType == "NXOpen.Annotations.VerticalDimension")
                {
                    NXOpen.Annotations.VerticalDimension dimension = (NXOpen.Annotations.VerticalDimension)nNXObject;
                    return oldColor = dimension.Color;
                }
                else if (DimenType == "NXOpen.Annotations.PerpendicularDimension")
                {
                    NXOpen.Annotations.PerpendicularDimension dimension = (NXOpen.Annotations.PerpendicularDimension)nNXObject;
                    return oldColor = dimension.Color;
                }
                else if (DimenType == "NXOpen.Annotations.MinorAngularDimension")
                {
                    NXOpen.Annotations.MinorAngularDimension dimension = (NXOpen.Annotations.MinorAngularDimension)nNXObject;
                    return oldColor = dimension.Color;
                }
                else if (DimenType == "NXOpen.Annotations.ChamferDimension")
                {
                    NXOpen.Annotations.ChamferDimension dimension = (NXOpen.Annotations.ChamferDimension)nNXObject;
                    return oldColor = dimension.Color;
                }
                else if (DimenType == "NXOpen.Annotations.HorizontalDimension")
                {
                    NXOpen.Annotations.HorizontalDimension dimension = (NXOpen.Annotations.HorizontalDimension)nNXObject;
                    return oldColor = dimension.Color;
                }
                else if (DimenType == "NXOpen.Annotations.HoleDimension")
                {
                    NXOpen.Annotations.HoleDimension dimension = (NXOpen.Annotations.HoleDimension)nNXObject;
                    return oldColor = dimension.Color;
                }
                else if (DimenType == "NXOpen.Annotations.DiameterDimension")
                {
                    NXOpen.Annotations.DiameterDimension dimension = (NXOpen.Annotations.DiameterDimension)nNXObject;
                    return oldColor = dimension.Color;
                }
                else if (DimenType == "NXOpen.Annotations.CylindricalDimension")
                {
                    NXOpen.Annotations.CylindricalDimension dimension = (NXOpen.Annotations.CylindricalDimension)nNXObject;
                    return oldColor = dimension.Color;
                }
                else if (DimenType == "NXOpen.Annotations.RadiusDimension")
                {
                    NXOpen.Annotations.RadiusDimension dimension = (NXOpen.Annotations.RadiusDimension)nNXObject;
                    return oldColor = dimension.Color;
                }
                else if (DimenType == "NXOpen.Annotations.ArcLengthDimension")
                {
                    NXOpen.Annotations.ArcLengthDimension dimension = (NXOpen.Annotations.ArcLengthDimension)nNXObject;
                    return oldColor = dimension.Color;
                }
                else if (DimenType == "NXOpen.Annotations.AngularDimension")
                {
                    NXOpen.Annotations.AngularDimension dimension = (NXOpen.Annotations.AngularDimension)nNXObject;
                    return oldColor = dimension.Color;
                }
                else if (DimenType == "NXOpen.Annotations.ParallelDimension")
                {
                    NXOpen.Annotations.ParallelDimension dimension = (NXOpen.Annotations.ParallelDimension)nNXObject;
                    return oldColor = dimension.Color;
                }
            }
            catch (System.Exception ex)
            {
                
            }
            return oldColor;
        }

        /// <summary>
        /// 設定標註尺寸顏色
        /// </summary>
        /// <param name="nNXObject"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool SetDimensionColor(NXObject nNXObject, int color)
        {
            try
            {
                DisplayModification displayModification1;
                displayModification1 = theSession.DisplayManager.NewDisplayModification();
                displayModification1.ApplyToAllFaces = false;
                displayModification1.ApplyToOwningParts = false;
                displayModification1.NewColor = color;
                DisplayableObject[] objects1 = new DisplayableObject[1];

                string DimenType = nNXObject.GetType().ToString();
                if (DimenType == "NXOpen.Annotations.VerticalDimension")
                {
                    NXOpen.Annotations.VerticalDimension dimension = (NXOpen.Annotations.VerticalDimension)nNXObject;
                    objects1[0] = dimension;
                    displayModification1.Apply(objects1);
                }
                else if (DimenType == "NXOpen.Annotations.PerpendicularDimension")
                {
                    NXOpen.Annotations.PerpendicularDimension dimension = (NXOpen.Annotations.PerpendicularDimension)nNXObject;
                    objects1[0] = dimension;
                    displayModification1.Apply(objects1);
                }
                else if (DimenType == "NXOpen.Annotations.MinorAngularDimension")
                {
                    NXOpen.Annotations.MinorAngularDimension dimension = (NXOpen.Annotations.MinorAngularDimension)nNXObject;
                    objects1[0] = dimension;
                    displayModification1.Apply(objects1);
                }
                else if (DimenType == "NXOpen.Annotations.ChamferDimension")
                {
                    NXOpen.Annotations.ChamferDimension dimension = (NXOpen.Annotations.ChamferDimension)nNXObject;
                    objects1[0] = dimension;
                    displayModification1.Apply(objects1);
                }
                else if (DimenType == "NXOpen.Annotations.HorizontalDimension")
                {
                    NXOpen.Annotations.HorizontalDimension dimension = (NXOpen.Annotations.HorizontalDimension)nNXObject;
                    objects1[0] = dimension;
                    displayModification1.Apply(objects1);
                }
                else if (DimenType == "NXOpen.Annotations.HoleDimension")
                {
                    NXOpen.Annotations.HoleDimension dimension = (NXOpen.Annotations.HoleDimension)nNXObject;
                    objects1[0] = dimension;
                    displayModification1.Apply(objects1);
                }
                else if (DimenType == "NXOpen.Annotations.DiameterDimension")
                {
                    NXOpen.Annotations.DiameterDimension dimension = (NXOpen.Annotations.DiameterDimension)nNXObject;
                    objects1[0] = dimension;
                    displayModification1.Apply(objects1);
                }
                else if (DimenType == "NXOpen.Annotations.CylindricalDimension")
                {
                    NXOpen.Annotations.CylindricalDimension dimension = (NXOpen.Annotations.CylindricalDimension)nNXObject;
                    objects1[0] = dimension;
                    displayModification1.Apply(objects1);
                }
                else if (DimenType == "NXOpen.Annotations.RadiusDimension")
                {
                    NXOpen.Annotations.RadiusDimension dimension = (NXOpen.Annotations.RadiusDimension)nNXObject;
                    objects1[0] = dimension;
                    displayModification1.Apply(objects1);
                }
                else if (DimenType == "NXOpen.Annotations.ArcLengthDimension")
                {
                    NXOpen.Annotations.ArcLengthDimension dimension = (NXOpen.Annotations.ArcLengthDimension)nNXObject;
                    objects1[0] = dimension;
                    displayModification1.Apply(objects1);
                }
                else if (DimenType == "NXOpen.Annotations.AngularDimension")
                {
                    NXOpen.Annotations.AngularDimension dimension = (NXOpen.Annotations.AngularDimension)nNXObject;
                    objects1[0] = dimension;
                    displayModification1.Apply(objects1);
                }
                else if (DimenType == "NXOpen.Annotations.ParallelDimension")
                {
                    NXOpen.Annotations.ParallelDimension dimension = (NXOpen.Annotations.ParallelDimension)nNXObject;
                    objects1[0] = dimension;
                    displayModification1.Apply(objects1);
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 畫Annotation的方框
        /// </summary>
        /// <param name="AnnotationTag"></param>
        /// <returns></returns>
        public static bool DrawTextBox(Tag AnnotationTag)
        {
            try
            {
                int[] mpi = new int[100];
                double[] mpr = new double[70], orientation = new double[9];
                string rad, dia;

                theUfSession.Drf.AskObjectPreferences(AnnotationTag, mpi, mpr, out rad, out dia);
                double angle = mpr[3];
                double angRad = angle * Math.PI / 180.0;
                double[] upper_left = new double[3] { 0, 0, 0 };
                double[] lower_left = new double[3] { 0, 0, 0 };
                double[] lower_right = new double[3] { 0, 0, 0 };
                double[] upper_right = new double[3] { 0, 0, 0 };
                double length, height;

                theUfSession.Drf.AskAnnotationTextBox(AnnotationTag, upper_left, out length, out height);

                //左下角
                lower_left[0] = upper_left[0] + height * Math.Sin(angRad);
                lower_left[1] = upper_left[1] - height * Math.Cos(angRad);
                //右下角
                lower_right[0] = lower_left[0] + length * Math.Cos(angRad);
                lower_right[1] = lower_left[1] + length * Math.Sin(angRad);
                //右上角
                upper_right[0] = upper_left[0] + length * Math.Cos(angRad);
                upper_right[1] = upper_left[1] + length * Math.Sin(angRad);

                UFObj.DispProps props = new UFObj.DispProps();
                props.color = 0; // only color is needed, use system default color

                theUfSession.Disp.DisplayTemporaryLine(Tag.Null, UFDisp.ViewType.UseWorkView, upper_left, lower_left, ref props);
                theUfSession.Disp.DisplayTemporaryLine(Tag.Null, UFDisp.ViewType.UseWorkView, lower_left, lower_right, ref props);
                theUfSession.Disp.DisplayTemporaryLine(Tag.Null, UFDisp.ViewType.UseWorkView, lower_right, upper_right, ref props);
                theUfSession.Disp.DisplayTemporaryLine(Tag.Null, UFDisp.ViewType.UseWorkView, upper_right, upper_left, ref props);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 取得TextBox的四個角落座標
        /// </summary>
        /// <param name="AnnotationTag"></param>
        /// <param name="cBoxCoordinate"></param>
        /// <returns></returns>
        public static bool GetTextBoxCoordinate(Tag AnnotationTag, out BoxCoordinate cBoxCoordinate)
        {
            cBoxCoordinate = new BoxCoordinate();
            try
            {
                int[] mpi = new int[100];
                double[] mpr = new double[70], orientation = new double[9];
                string rad, dia;

                theUfSession.Drf.AskObjectPreferences(AnnotationTag, mpi, mpr, out rad, out dia);
                double angle = mpr[3];
                double angRad = angle * Math.PI / 180.0;
                //double[] upper_left = new double[3] { 0, 0, 0 };
                //double[] lower_left = new double[3] { 0, 0, 0 };
                //double[] lower_right = new double[3] { 0, 0, 0 };
                //double[] upper_right = new double[3] { 0, 0, 0 };
                double length, height;

                theUfSession.Drf.AskAnnotationTextBox(AnnotationTag, cBoxCoordinate.upper_left, out length, out height);
                
                //左下角
                cBoxCoordinate.lower_left[0] = cBoxCoordinate.upper_left[0] + height * Math.Sin(angRad);
                cBoxCoordinate.lower_left[1] = cBoxCoordinate.upper_left[1] - height * Math.Cos(angRad);
                //右下角
                cBoxCoordinate.lower_right[0] = cBoxCoordinate.lower_left[0] + length * Math.Cos(angRad);
                cBoxCoordinate.lower_right[1] = cBoxCoordinate.lower_left[1] + length * Math.Sin(angRad);
                //右上角
                cBoxCoordinate.upper_right[0] = cBoxCoordinate.upper_left[0] + length * Math.Cos(angRad);
                cBoxCoordinate.upper_right[1] = cBoxCoordinate.upper_left[1] + length * Math.Sin(angRad);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 建立泡泡在圖紙上
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="BallonPt"></param>
        /// <returns></returns>
        public static bool CreateBallonOnSheet(string Number, Point3d BallonPt)
        {
            try
            {// ----------------------------------------------
                //   Menu: Insert->Annotation->Identification Symbol...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Annotations.IdSymbol nullAnnotations_IdSymbol = null;
                NXOpen.Annotations.IdSymbolBuilder idSymbolBuilder1;
                idSymbolBuilder1 = workPart.Annotations.IdSymbols.CreateIdSymbolBuilder(nullAnnotations_IdSymbol);

                idSymbolBuilder1.Origin.Plane.PlaneMethod = NXOpen.Annotations.PlaneBuilder.PlaneMethodType.XyPlane;

                idSymbolBuilder1.Origin.SetInferRelativeToGeometry(false);

                idSymbolBuilder1.Origin.Anchor = NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidCenter;

                idSymbolBuilder1.UpperText = "<C0.7>" + Number + "<C>";

                idSymbolBuilder1.Size = 3.5;

                theSession.SetUndoMarkName(markId1, "Identification Symbol Dialog");

                idSymbolBuilder1.Origin.Plane.PlaneMethod = NXOpen.Annotations.PlaneBuilder.PlaneMethodType.XyPlane;

                idSymbolBuilder1.Origin.SetInferRelativeToGeometry(false);

                NXOpen.Annotations.LeaderData leaderData1;
                leaderData1 = workPart.Annotations.CreateLeaderData();

                leaderData1.StubSize = 5.0;

                leaderData1.Arrowhead = NXOpen.Annotations.LeaderData.ArrowheadType.FilledArrow;

                idSymbolBuilder1.Leader.Leaders.Append(leaderData1);

                leaderData1.StubSide = NXOpen.Annotations.LeaderSide.Inferred;

                idSymbolBuilder1.Origin.SetInferRelativeToGeometry(false);

                idSymbolBuilder1.Origin.SetInferRelativeToGeometry(false);

                NXOpen.Annotations.Annotation.AssociativeOriginData assocOrigin1;
                assocOrigin1.OriginType = NXOpen.Annotations.AssociativeOriginType.Drag;
                View nullView = null;
                assocOrigin1.View = nullView;
                assocOrigin1.ViewOfGeometry = nullView;
                Point nullPoint = null;
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
                idSymbolBuilder1.Origin.SetAssociativeOrigin(assocOrigin1);

                Point3d point1 = new Point3d(BallonPt.X, BallonPt.Y, 0.0);
                idSymbolBuilder1.Origin.Origin.SetValue(null, nullView, point1);

                idSymbolBuilder1.Origin.SetInferRelativeToGeometry(false);

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Identification Symbol");

                NXObject nXObject1;
                nXObject1 = idSymbolBuilder1.Commit();

                theSession.DeleteUndoMark(markId2, null);

                theSession.SetUndoMarkName(markId1, "Identification Symbol");

                theSession.SetUndoMarkVisibility(markId1, null, NXOpen.Session.MarkVisibility.Visible);

                idSymbolBuilder1.Destroy();

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

                NXOpen.Annotations.IdSymbolBuilder idSymbolBuilder2;
                idSymbolBuilder2 = workPart.Annotations.IdSymbols.CreateIdSymbolBuilder(nullAnnotations_IdSymbol);

                idSymbolBuilder2.Origin.Plane.PlaneMethod = NXOpen.Annotations.PlaneBuilder.PlaneMethodType.XyPlane;

                idSymbolBuilder2.Origin.SetInferRelativeToGeometry(false);

                idSymbolBuilder2.Origin.Anchor = NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidCenter;

                idSymbolBuilder2.UpperText = "<C1>5<C>";

                idSymbolBuilder2.Size = 3.5;

                idSymbolBuilder2.Style.LineArrowStyle.TextToLineDistance = 0.0;

                theSession.SetUndoMarkName(markId3, "Identification Symbol Dialog");

                idSymbolBuilder2.Origin.Plane.PlaneMethod = NXOpen.Annotations.PlaneBuilder.PlaneMethodType.XyPlane;

                idSymbolBuilder2.Origin.SetInferRelativeToGeometry(false);

                NXOpen.Annotations.LeaderData leaderData2;
                leaderData2 = workPart.Annotations.CreateLeaderData();

                leaderData2.StubSize = 5.0;

                leaderData2.Arrowhead = NXOpen.Annotations.LeaderData.ArrowheadType.FilledArrow;

                idSymbolBuilder2.Leader.Leaders.Append(leaderData2);

                leaderData2.StubSide = NXOpen.Annotations.LeaderSide.Inferred;

                idSymbolBuilder2.Origin.SetInferRelativeToGeometry(false);

                idSymbolBuilder2.Origin.SetInferRelativeToGeometry(false);

                // ----------------------------------------------
                //   Dialog Begin Identification Symbol
                // ----------------------------------------------
                idSymbolBuilder2.Destroy();

                theSession.UndoToMark(markId3, null);

                theSession.DeleteUndoMark(markId3, null);

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

        /// <summary>
        /// 取得Fcf的資料
        /// Characteristic
        /// ZoneShape
        /// ToleranceValue
        /// MaterialModifier
        /// PrimaryDatum/PrimaryMaterialModifier
        /// SecondaryDatum/SecondaryMaterialModifier
        /// TertiaryDatum/TertiaryMaterialModifier
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="Characteristic"></param>
        /// <param name="ToleranceValue"></param>
        /// <returns></returns>
        public static bool GetFcfData(NXOpen.Annotations.DraftingFcf temp, out FcfData sFcfData)
        {
            sFcfData = new FcfData();
            try
            {
                NXOpen.Annotations.DraftingFeatureControlFrameBuilder draftingFeatureControlFrameBuilder1;
                draftingFeatureControlFrameBuilder1 = workPart.Annotations.CreateDraftingFeatureControlFrameBuilder(temp);
                sFcfData.Characteristic = draftingFeatureControlFrameBuilder1.Characteristic.ToString();

                TaggedObject taggedObject2;
                taggedObject2 = draftingFeatureControlFrameBuilder1.FeatureControlFrameDataList.FindItem(0);
                NXOpen.Annotations.FeatureControlFrameDataBuilder featureControlFrameDataBuilder1 = (NXOpen.Annotations.FeatureControlFrameDataBuilder)taggedObject2;
                sFcfData.ZoneShape = featureControlFrameDataBuilder1.ZoneShape.ToString();
                sFcfData.ToleranceValue = featureControlFrameDataBuilder1.ToleranceValue;
                sFcfData.MaterialModifier = featureControlFrameDataBuilder1.MaterialModifier.ToString();

                sFcfData.PrimaryDatum = featureControlFrameDataBuilder1.PrimaryDatumReference.Letter;
                sFcfData.PrimaryMaterialModifier = featureControlFrameDataBuilder1.PrimaryDatumReference.MaterialCondition.ToString();
                sFcfData.SecondaryDatum = featureControlFrameDataBuilder1.SecondaryDatumReference.Letter;
                sFcfData.SecondaryMaterialModifier = featureControlFrameDataBuilder1.SecondaryDatumReference.MaterialCondition.ToString();
                sFcfData.TertiaryDatum = featureControlFrameDataBuilder1.TertiaryDatumReference.Letter;
                sFcfData.TertiaryMaterialModifier = featureControlFrameDataBuilder1.TertiaryDatumReference.MaterialCondition.ToString();
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 刪除指定的泡泡
        /// </summary>
        /// <param name="BallonNum"></param>
        /// <returns></returns>
        public static bool DeleteBallon(string BallonNum)
        {
            try
            {
                IdSymbolCollection aa = workPart.Annotations.IdSymbols;
                IdSymbol[] bb = aa.ToArray();
                foreach (NXOpen.Annotations.IdSymbol i in bb)
                {

                    NXOpen.Annotations.IdSymbolBuilder idSymbolBuilder1;
                    idSymbolBuilder1 = workPart.Annotations.IdSymbols.CreateIdSymbolBuilder(i);
                    string a = idSymbolBuilder1.UpperText;
                    idSymbolBuilder1.Destroy();
                    a = a.Remove(0, 6);
                    a = a.Remove(1, 3);
                    if (BallonNum == a)
                    {
                        //NXObject[] objects1 = new NXObject[1];
                        //objects1[0] = i;
                        //theSession.UpdateManager.AddToDeleteList(objects1);
                        CaxPublic.DelectObject(i);
                        
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 修改Sheet名稱
        /// </summary>
        /// <param name="TargetSheet"></param>
        /// <param name="SheetName"></param>
        /// <returns></returns>
        public static bool SheetRename(DrawingSheet TargetSheet, string SheetName)
        {
            try
            {
                NXOpen.Drawings.DrawingSheetBuilder drawingSheetBuilder1;
                drawingSheetBuilder1 = workPart.DrawingSheets.DrawingSheetBuilder(TargetSheet);
                drawingSheetBuilder1.Name = SheetName;
                drawingSheetBuilder1.Commit();
                drawingSheetBuilder1.Destroy();
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
