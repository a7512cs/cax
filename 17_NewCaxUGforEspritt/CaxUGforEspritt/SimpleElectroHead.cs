using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using NXOpen.Features;
using NXOpen.Utilities;
using NXOpen.GeometricUtilities;
using NXOpen.UF;
using NXOpen.BlockStyler;
using CimforceCaxTwPublic;
using CaxUGforEspritt;


public partial class SimpleElectroHeadDlg2
{
    private static UFSession theUfSession = UFSession.GetUFSession();
    //private OffsetFace boxOffset;
    public Body boxBody;
    public Block blockFeat;
    private const int TEMP_LAYER = 249;
    private const double PERP_TOL = 0.001;
    private const double MINIMUM_LEN = 0.1;
    private int originaLayerStatus;

    /// <summary>
    /// 依WCS取得物件陣列的最小方盒
    /// </summary>
    /// <param name="objects"></param>
    /// <param name="min">最小角落點(ACS)</param>
    /// <param name="max">最大角落點(ACS)</param>
    public static void AskBoundingBoxExact(List<TaggedObject> objects, out double[] min, out double[] max)
    {
        double[] minPoint = { 9999999999, 9999999999, 9999999999 };
        double[] maxPoint = { -9999999999, -9999999999, -9999999999 };
        double[,] directions = { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
        double[] distances = { 0, 0, 0 };
        double[] min_corner = { 0, 0, 0 };
        double[] max_corner = { 0, 0, 0 };
        Tag wcsCsys;

        theUfSession.Csys.AskWcs(out wcsCsys);

        foreach (NXObject obj in objects)
        {
            theUfSession.Modl.AskBoundingBoxExact(obj.Tag, wcsCsys, min_corner, directions, distances);
            for (int i = 0; i < 3; i++)
            {
                max_corner[i] = min_corner[i];
                for (int j = 0; j < 3; j++)
                {
                    max_corner[i] += directions[j, i] * distances[j];
                }
            }
            min_corner = MapAbsToWcs(min_corner);
            max_corner = MapAbsToWcs(max_corner);

            minPoint[0] = (minPoint[0] < min_corner[0]) ? minPoint[0] : min_corner[0];
            minPoint[1] = (minPoint[1] < min_corner[1]) ? minPoint[1] : min_corner[1];
            minPoint[2] = (minPoint[2] < min_corner[2]) ? minPoint[2] : min_corner[2];

            maxPoint[0] = (maxPoint[0] > max_corner[0]) ? maxPoint[0] : max_corner[0];
            maxPoint[1] = (maxPoint[1] > max_corner[1]) ? maxPoint[1] : max_corner[1];
            maxPoint[2] = (maxPoint[2] > max_corner[2]) ? maxPoint[2] : max_corner[2];
        }

        min = MapWcsToAbs(minPoint);
        max = MapWcsToAbs(maxPoint);
    }

    public static double[] MapAbsToWcs(double[] pt)
    {
        double[] ptWcs = new double[3];
        theUfSession.Csys.MapPoint(UFConstants.UF_CSYS_ROOT_COORDS, pt, UFConstants.UF_CSYS_ROOT_WCS_COORDS, ptWcs);

        return ptWcs;
    }

    public static double[] MapWcsToAbs(double[] pt)
    {
        double[] ptAbs = new double[3];
        theUfSession.Csys.MapPoint(UFConstants.UF_CSYS_ROOT_WCS_COORDS, pt, UFConstants.UF_CSYS_ROOT_COORDS, ptAbs);

        return ptAbs;
    }

    public static double[] ConvetToAbsVector(double[] vtr)
    {
        double[] origin = { 0, 0, 0 };
        double[] newVtr = new double[3];
        origin = MapWcsToAbs(origin);
        vtr = MapWcsToAbs(vtr);
        theUfSession.Vec3.Sub(vtr, origin, newVtr);

        return newVtr;
    }

    public static bool IsParallel(double[] vt1, double[] vt2)
    {
        int is_parallel;

        theUfSession.Vec3.IsParallel(vt1, vt2, PERP_TOL, out is_parallel);

        return (is_parallel == 1) ? true : false;
    }

    public static bool IsSameDirection(double[] vt1, double[] vt2)
    {
        double dot_value;
        theUfSession.Vec3.Dot(vt1, vt2, out dot_value);

        return (dot_value > 0) ? true : false;
    }

    /// <summary>
    /// 建立方塊
    /// </summary>
    /// <param name="min">最小角落點(ACS)</param>
    /// <param name="max">最大角落點(ACS)</param>
    /// <param name="offset">六個方向的offset值 (-X, -Y, -Z, +X, +Y, +Z)</param>
    /// <returns></returns>
    public void CreateBlock(double[] min, double[] max, double[] offset)
    {
        Part workPart = theSession.Parts.Work;
        BlockFeatureBuilder blockFeatureBuilder1;
        Point3d originPoint1 = new Point3d(min[0] - offset[0], min[1] - offset[1], min[2] - offset[2]);
        Point3d cornerPoint1 = new Point3d(max[0] + offset[3], max[1] + offset[4], max[2] + offset[5]);
        if (blockFeat == null)
        {
            blockFeatureBuilder1 = workPart.Features.CreateBlockFeatureBuilder(null);
            blockFeatureBuilder1.Type = BlockFeatureBuilder.Types.DiagonalPoints;
            Point orig = workPart.Points.CreatePoint(originPoint1);
            Point corner = workPart.Points.CreatePoint(cornerPoint1);
            blockFeatureBuilder1.OriginPoint = orig;
            blockFeatureBuilder1.PointFromOrigin = corner;
            blockFeatureBuilder1.SetTwoDiagonalPoints(originPoint1, cornerPoint1);
            blockFeatureBuilder1.SetBooleanOperationAndTarget(Feature.BooleanType.Create, null);
            blockFeat = (Block)blockFeatureBuilder1.CommitFeature();
            boxBody = blockFeat.GetBodies()[0];
        }
        else
        {
            blockFeatureBuilder1 = workPart.Features.CreateBlockFeatureBuilder(blockFeat);
            blockFeatureBuilder1.Type = BlockFeatureBuilder.Types.DiagonalPoints;
            blockFeatureBuilder1.OriginPoint.SetCoordinates(originPoint1);
            blockFeatureBuilder1.PointFromOrigin.SetCoordinates(cornerPoint1);
            blockFeatureBuilder1.SetTwoDiagonalPoints(originPoint1, cornerPoint1);
            blockFeatureBuilder1.SetBooleanOperationAndTarget(Feature.BooleanType.Create, null);
            blockFeatureBuilder1.CommitFeature();
        }
        blockFeatureBuilder1.Destroy();
    }

    /// <summary>
    /// 將面析出
    /// </summary>
    /// <param name="faces"></param>
    /// <returns></returns>
    public Body ExtractFaces(List<Face> faces)
    {
        Body result = null;
        Part workPart = theSession.Parts.Work;

        NXOpen.Features.ExtractFaceBuilder extractFaceBuilder1;
        extractFaceBuilder1 = workPart.Features.CreateExtractFaceBuilder(null);
        extractFaceBuilder1.Type = NXOpen.Features.ExtractFaceBuilder.ExtractType.Face;
        extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;
        extractFaceBuilder1.DeleteHoles = true;
        extractFaceBuilder1.FixAtCurrentTimestamp = true;

        extractFaceBuilder1.ReplacementAssistant.SetNewParents(faces.ToArray());
        FaceDumbRule faceDumbRule1;
        faceDumbRule1 = workPart.ScRuleFactory.CreateRuleFaceDumb(faces.ToArray());
        SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
        rules1[0] = faceDumbRule1;
        extractFaceBuilder1.FaceChain.ReplaceRules(rules1, false);

        try {
            ExtractFace extract;
            extract = ( ExtractFace)extractFaceBuilder1.Commit();
            Tag bodyTag;
            theUfSession.Modl.AskFeatBody(extract.Tag, out bodyTag);
            result = (Body)NXObjectManager.Get(bodyTag);
        }
        catch(Exception ex)
        {
            theUI.NXMessageBox.Show("ExtractFaces", NXMessageBox.DialogType.Error, ex.ToString());
        }
        extractFaceBuilder1.Destroy();


        return result;
    }

    /// <summary>
    /// 將sheet bodies縫合
    /// </summary>
    /// <param name="sheetBodies"></param>
    /// <returns></returns>
    public Body SewSheets(List<Body> sheetBodies)
    {
        if (sheetBodies == null || sheetBodies.Count == 0)
            return null;
        else if (sheetBodies.Count == 1)
            return sheetBodies[0];

        Part workPart = theSession.Parts.Work;
        NXOpen.Features.SewBuilder sewBuilder1;
        sewBuilder1 = workPart.Features.CreateSewBuilder(null);
        sewBuilder1.Tolerance = workPart.Preferences.Modeling.DistanceToleranceData;
        sewBuilder1.TargetBodies.Add(sheetBodies[0]);
        List<Body> toolBodies = new List<Body>(sheetBodies);
        toolBodies.RemoveAt(0);
        sewBuilder1.ToolBodies.Add(toolBodies.ToArray());
        NXObject nXObject2;
        nXObject2 = sewBuilder1.Commit();
        sewBuilder1.Destroy();
        Body[] bodies = ((Sew)nXObject2).GetBodies();
        return bodies[0];
    }

    /// <summary>
    /// 沿sheet的邊界往外延伸一段距離 (類型為offset的面似乎無法延伸, 可能須轉為b-surface)
    /// </summary>
    /// <param name="sheet"></param>
    /// <param name="distance">延伸的距離</param>
    /// <returns></returns>
    public Body TrimAndExtend(Body sheet, double distance)
    {
        Part workPart = theSession.Parts.Work;
        NXOpen.Features.TrimExtendBuilder trimExtendBuilder1;
        trimExtendBuilder1 = workPart.Features.CreateTrimExtendBuilder(null);
        trimExtendBuilder1.TargetExtendDistance.RightHandSide = distance.ToString();
        trimExtendBuilder1.ExtensionMethod = TrimExtendBuilder.ExtensionMethods.NaturalTangent;

        EdgeSheetBoundaryRule edgeSheetBoundaryRule1;
        edgeSheetBoundaryRule1 = workPart.ScRuleFactory.CreateRuleEdgeSheetBoundary(sheet);
        SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
        rules2[0] = edgeSheetBoundaryRule1;
        trimExtendBuilder1.TargetCollector.ReplaceRules(rules2, false);
        NXObject nXObject3;
        nXObject3 = trimExtendBuilder1.Commit();
        trimExtendBuilder1.Destroy();

        TrimExtend trimExtend = (TrimExtend)nXObject3;
        return trimExtend.GetBodies()[0];
    }

    /// <summary>
    /// 用tool body的面來修剪target body
    /// </summary>
    /// <param name="target"></param>
    /// <param name="tool"></param>
    /// <param name="reverse"></param>
    public void TrimBodyByFaces(Body target, Body tool, bool reverse)
    {
        Part workPart = theSession.Parts.Work;
        TrimBody2Builder trimBody2Builder1 = workPart.Features.CreateTrimBody2Builder(null);
        Body[] bodies1 = { target };
        BodyDumbRule bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1);
        SelectionIntentRule[] rules3 = { bodyDumbRule1 };
        ScCollector scCollector1 = workPart.ScCollectors.CreateCollector();
        scCollector1.ReplaceRules(rules3, false);
        trimBody2Builder1.TargetBodyCollector = scCollector1;

        trimBody2Builder1.BooleanTool.ToolOption = NXOpen.GeometricUtilities.BooleanToolBuilder.BooleanToolType.FaceOrPlane;
        FaceBodyRule faceBodyRule1;
        faceBodyRule1 = workPart.ScRuleFactory.CreateRuleFaceBody(tool);
        SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
        rules2[0] = faceBodyRule1;
        trimBody2Builder1.BooleanTool.FacePlaneTool.ToolFaces.FaceCollector.ReplaceRules(rules2, false);

        trimBody2Builder1.BooleanTool.ReverseDirection = reverse;
        trimBody2Builder1.Tolerance = workPart.Preferences.Modeling.DistanceToleranceData;

        trimBody2Builder1.Commit();
        trimBody2Builder1.Destroy();
    }

    /// <summary>
    /// 求面上的參數中點及法向量
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="point"></param>
    /// <param name="unit_normal"></param>
    public void AskFacePointNromal(Face obj, out double[] point, out double[] unit_normal)
    {
        point = new double[3];
        unit_normal = new double[3];
        double[] dummy = new double[3];
        double[] uvbox = new double[4];
        double[] radii = new double[2];

        theUfSession.Modl.AskFaceUvMinmax(obj.Tag, uvbox);
        double[] param = { (uvbox[0] + uvbox[1]) / 2, (uvbox[2] + uvbox[3]) / 2 };
        theUfSession.Modl.AskFaceProps(obj.Tag, param, point, dummy, dummy, dummy, dummy, unit_normal, radii);
    }

    /// <summary>
    /// 顯示選取的面的法線方向()
    /// </summary>
    public void ShowDirection()
    {
        //CaxLog.ShowListingWindow("ShowDirection");
        //CaxLog.ShowListingWindow("boxBody:"+boxBody);
        //CaxLog.ShowListingWindow("3");
        TaggedObject[] selFaces = faceSelect.GetProperties().GetTaggedObjectVector("SelectedObjects");
        //TaggedObject[] selFaces = ZMachining.SelectFaceTaggedObjectAry;
        Face selFace = (Face)selFaces[0];
        Body selBody = selFace.GetBody();
        theUfSession.Obj.SetTranslucency(selBody.Tag, 60);

        if (selFaces.Length == 0)
        {
            //CaxLog.ShowListingWindow("4");
            offset.GetProperties().SetLogical("ShowHandle", false);
            offset1.GetProperties().SetLogical("ShowHandle", false);
            offset2.GetProperties().SetLogical("ShowHandle", false);
            offset3.GetProperties().SetLogical("ShowHandle", false);
            offset4.GetProperties().SetLogical("ShowHandle", false);
            offset5.GetProperties().SetLogical("ShowHandle", false);

            if (boxBody != null)
            {
                //CaxLog.ShowListingWindow("5");
                theUfSession.Obj.DeleteObject(boxBody.Tag);
                theUfSession.Layer.SetStatus(TEMP_LAYER, originaLayerStatus);
                boxBody = null;
                blockFeat = null;
            }
            return;
        }

        
        if (boxBody != null)
        {
            //CaxLog.ShowListingWindow("6");
            theUfSession.Obj.DeleteObject(boxBody.Tag);
            theUfSession.Layer.SetStatus(TEMP_LAYER, originaLayerStatus);
            boxBody = null;
            blockFeat = null;
        }
        PreviewBlock(null);

        //CaxLog.ShowListingWindow("7");
        //double[] point, normal;
        //AskFacePointNromal((Face)selFaces[0], out point, out normal);
        //double[] offsetDir = new double[3];
        //theUfSession.Vec3.Negate(normal, offsetDir);

        //設定reverseDir的預設方向
        //reverseDir.GetProperties().SetPoint("Origin", new Point3d(point[0], point[1], point[2]));
        //reverseDir.GetProperties().SetVector("Direction", new Vector3d(offsetDir[0], offsetDir[1], offsetDir[2]));
        //reverseDir.GetProperties().SetLogical("Flip", false);
    }

    private void BuildOffsetHandle()
    {
        if (boxBody != null)
        {
            double[] xdir = ConvetToAbsVector(new double[] { 1, 0, 0 });
            double[] ydir = ConvetToAbsVector(new double[] { 0, 1, 0 });
            double[] zdir = ConvetToAbsVector(new double[] { 0, 0, 1 });
            double[] point, normal;
            NXOpen.BlockStyler.LinearDimension tmpBlk;
            Face[] faces = boxBody.GetFaces();
            foreach (Face face in faces)
            {
                AskFacePointNromal(face, out point, out normal);
                if (IsParallel(normal, xdir))
                {
                    if (!IsSameDirection(normal, xdir))
                    {
                        tmpBlk = offset;
                    }
                    else
                    {
                        tmpBlk = offset3;
                    }
                        
                }
                else if (IsParallel(normal, ydir))
                {
                    if (!IsSameDirection(normal, ydir))
                    {
                        tmpBlk = offset1;
                    }
                    else
                    {
                        tmpBlk = offset4;
                    }
                        
                }
                else
                {
                    if (!IsSameDirection(normal, zdir))
                    {
                        tmpBlk = offset2;
                        continue;
                    }
                    else
                    {
                        tmpBlk = offset5;
                        continue;
                    }
                }

                tmpBlk.Show = true;
                tmpBlk.GetProperties().SetLogical("ShowHandle", true);
                tmpBlk.GetProperties().SetPoint("HandleOrigin", new Point3d(point[0], point[1], point[2]));
                tmpBlk.GetProperties().SetVector("HandleOrientation", new Vector3d(normal[0], normal[1], normal[2]));
            }
        }
    }

    public double[] CheckOffsetValue(UIBlock block, double[] min, double[] max)
    {
        double[] offsetDist = new double[6];
        NXOpen.BlockStyler.LinearDimension[] offsetDims = { offset, offset1, offset2, offset3, offset4, offset5 };

        for (int i = 0; i < 6; i++)
            offsetDist[i] = offsetDims[i].GetProperties().GetDouble("Value");

        if (block == null)
            return offsetDist;

        //確認產生的方塊長度要符合設定的最小值
        double[] minWcs = MapAbsToWcs(min);
        double[] maxWcs = MapAbsToWcs(max);
        double totalLength;
        for (int i = 0; i < 6; i++)
        {
            if (block == offsetDims[i])
            {
                int j = (i < 3) ? i + 3 : i - 3;  //3,4,5,0,1,2
                int k = (i < 3) ? i : i - 3;      //0,1,2,0,1,2
                totalLength = offsetDist[i] + Math.Abs(maxWcs[k] - minWcs[k]) + offsetDist[j];
                if (totalLength <= MINIMUM_LEN)
                {
                    offsetDist[i] = MINIMUM_LEN - Math.Abs(maxWcs[k] - minWcs[k]) - offsetDist[j];
                    offsetDims[i].GetProperties().SetDouble("Value", offsetDist[i]);
                }
            }
        }

        return offsetDist;
    }

    public void PreviewBlock(UIBlock uiBlock)
    {
        Session.UndoMarkId markId1;
        markId1 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Update Block");
        //CaxLog.ShowListingWindow("8");
        //CaxLog.ShowListingWindow("PreviewBlock");
        TaggedObject[] selFaces = faceSelect.GetProperties().GetTaggedObjectVector("SelectedObjects");
        //CaxLog.ShowListingWindow("PreviewBlockselFaces.Length:" + selFaces.Length);

        try
        {
            //CaxLog.ShowListingWindow("9");
            //CaxLog.ShowListingWindow("PreviewBlock -1");
            //取得選取的面的最小邊界方盒
            double[] min, max;
            AskBoundingBoxExact(selFaces.ToList(), out min, out max);
            double[] offsetDist = CheckOffsetValue(uiBlock, min, max);
            if (boxBody == null)
            {
                //CaxLog.ShowListingWindow("10");
                //設定TEMP_LAYER為visible only layer, 使得preview body在物件選取時不會被選到
                theUfSession.Layer.AskStatus(TEMP_LAYER, out originaLayerStatus);
                theUfSession.Layer.SetStatus(TEMP_LAYER, UFConstants.UF_LAYER_REFERENCE_LAYER);
                //根據最小邊界方盒建立方塊
                CreateBlock(min, max, offsetDist);
                //在方塊的六個面顯示handle
                BuildOffsetHandle();
            }
            else
            {
                //CaxLog.ShowListingWindow("11");
                //CaxLog.ShowListingWindow("PreviewBlock -2");
                //根據最小邊界方盒建立方塊
                CreateBlock(min, max, offsetDist);
            }


            //設定方塊透明度為70
            if (boxBody != null)
            {
                //CaxLog.ShowListingWindow("12");
                //CaxLog.ShowListingWindow("PreviewBlock -boxBody != null");
                theUfSession.Obj.SetTranslucency(boxBody.Tag, 60);
                theUfSession.Obj.SetLayer(boxBody.Tag, TEMP_LAYER);
            }
            //CaxLog.ShowListingWindow("PreviewBlock -done");

            theSession.UpdateManager.DoUpdate(markId1);
        }
        catch (Exception ex)
        {
            theSession.DeleteUndoMark(markId1, null);
            theSession.LogFile.WriteLine(ex.Message);
        }
    }

    public void Doit()
    {
        if (boxBody == null)
            return;
        //CaxLog.ShowListingWindow("13");

        Session.UndoMarkId markId1;
        markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "SimpleElectroHead");

        //取得使用介面上的設定值(選取的面, offset距離, 是否reverse)
        TaggedObject[] selFaces = faceSelect.GetProperties().GetTaggedObjectVector("SelectedObjects");
        bool reverse = reverseDir.GetProperties().GetLogical("Flip");
        double[] offsetDist = CheckOffsetValue(null, null, null);

        //將preview body移至工作層
        int workLayer;
        theUfSession.Layer.AskWorkLayer(out workLayer);
        theUfSession.Obj.SetLayer(boxBody.Tag, workLayer);
        theUfSession.Layer.SetStatus(TEMP_LAYER, originaLayerStatus);

        theUfSession.Obj.SetTranslucency(boxBody.Tag, 0);

        ZMachining.boxBody = boxBody;
        //ZMachining.IsOK = true;
        /*
        //將選取的面由實體析出
        List<Face> faces = new List<Face>();
        foreach (TaggedObject face in selFaces)
            faces.Add((Face)face);
        Body sheet = ExtractFaces(faces);

        if (sheet != null)
        {
            CaxLog.ShowListingWindow("14");
            //用縫合的面沿邊界往外延伸兩倍offset距離
            Body extend = TrimAndExtend(sheet, offsetDist.Max() * 2 + 1);
            extend.Blank();

            //用延伸後的sheet body將方塊修剪
            if (boxBody != null)
            {
                CaxLog.ShowListingWindow("15");
                TrimBodyByFaces(boxBody, extend, !reverse);
            }
        }
        boxBody = null;
        blockFeat = null;
        */
    }
}
