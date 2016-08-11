using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using NXOpen.UF;

namespace CreateBallon
{
    public class Functions
    {
        public static Session theSession = Session.GetSession();
        public static UI theUI;
        public static UFSession theUfSession = UFSession.GetUFSession();
        public static Part workPart = theSession.Parts.Work;
        public static Part displayPart = theSession.Parts.Display;

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
    }
}
