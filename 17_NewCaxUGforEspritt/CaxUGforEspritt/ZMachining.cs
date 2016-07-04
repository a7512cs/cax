using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NXOpen;
using CimforceCaxTwPublic;
using NXOpen.Utilities;
using NXOpen.UF;
using CSUGFunc;
using NXCustomerComponent;
using NXOpen.Features;


namespace CaxUGforEspritt
{
    public partial class ZMachining : DevComponents.DotNetBar.Office2007Form
    {
        public static Session theSession = Session.GetSession();
        public static UI theUI = UI.GetUI();
        public static UFSession theUfSession = UFSession.GetUFSession();

        public static TaggedObject[] SelectFaceTaggedObjectAry = null;
        public static Body SelectFaceBBoxBody = null;
        public static Body boxBody = null;
        public static Block blockFeat = null;
        public static Tag theBlock = NXOpen.Tag.Null;
        public static NXObject theBlockObj = null;
        public static double[] minWCS, maxWCS;
        public static bool status;
        public static BoundingBoxPt sBoundingBoxPt = new BoundingBoxPt();
        public static bool IsOK = false;

        public struct BoundingBoxPt
        {
            public double[] minWCS;
            public double[] maxWCS;
        }

        public ZMachining()
        {
            InitializeComponent();

        }

        private void buttonX1_SelectFaces_Click(object sender, EventArgs e)
        {
            Part displayPart = theSession.Parts.Display;
            displayPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);

            List<NXObject> ListNXObjToDelete = new List<NXObject>();
            NXObject tempNXObject = null;
            SelectFaceBBoxBody = null;
            SelectFaceTaggedObjectAry = null;
            blockFeat = null;

            bool IsSelFaces = true;
            bool IsCreateWEPath = false;
            //選面
            Tag[] SelectFaceTagAry = null;
            status = SelectFaces(out SelectFaceTaggedObjectAry, out SelectFaceTagAry);

            if (SelectFaceTagAry.Length == 0)
            {
                //CaxLog.ShowListingWindow("選面 失敗");
                IsSelFaces = false;
                return;
            }
            
            AskBoundingBoxExact(SelectFaceTaggedObjectAry.ToList(), out sBoundingBoxPt.minWCS, out sBoundingBoxPt.maxWCS);
            
            //1.建BBox取得最小最大點後就刪掉
            double[] offset = new double[6] { 0, 0, 0, 0, 0, 0 };
            status = NewCreateBlock(sBoundingBoxPt.minWCS, sBoundingBoxPt.maxWCS, offset, out SelectFaceBBoxBody);
            if (!status)
            {
                CaxLog.ShowListingWindow("建BBox 失敗");
                CaxWE.RecordFailed(ref hideDispalyObject, WEComp, kvp);
            }

            //有選線割面才讓使用者調整線割路徑
            if (IsSelFaces)
            {
            SelectPlaneToDecidePath:
                SimpleElectroHeadDlg2 cSimpleElectroHeadDlg2 = new SimpleElectroHeadDlg2();
                cSimpleElectroHeadDlg2.Show();
                if (boxBody == null)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "請點選方塊的面進行拖拉，決定線割路徑");
                    goto
                    SelectPlaneToDecidePath;
                }
                IsCreateWEPath = true;
            }



            //有調整路徑才繼續做以下動作
            if (IsCreateWEPath)
            {
                DeleteNXObject(SelectFaceBBoxBody);

                //2.電極與BBox相減取得要拉伸的面
                Face FaceToPull = null;
                status = GetFaceToPull(boxBody, out FaceToPull, out tempNXObject);
                if (!status)
                {
                    CaxLog.ShowListingWindow("取得要拉伸的面 失敗");
                    CaxWE.RecordFailed(ref hideDispalyObject, WEComp, kvp);
                }
                RemoveParameters(boxBody);
                //ListNXObjToDelete.Add(tempNXObject);
                //return;

                //3.建立實體(解決線割面沒貼基準座)
                Body CreateBody = null;
                status = CreateBlock(sBoundingBoxPt.minWCS, sBoundingBoxPt.maxWCS, out CreateBody, out tempNXObject);
                if (!status)
                {
                    CaxLog.ShowListingWindow("建立實體 失敗");
                    CaxWE.RecordFailed(ref hideDispalyObject, WEComp, kvp);
                }

                //ListNXObjToDelete.Add(tempNXObject);

                //4.實體與電極相加
                status = UniteBody(WEBody, CreateBody, out tempNXObject);
                if (!status)
                {
                    CaxLog.ShowListingWindow("實體結合 失敗");
                    CaxWE.RecordFailed(ref hideDispalyObject, WEComp, kvp);
                }
                RemoveParameters(WEBody);


                //5.NXObject MoveFaceObj = null;
                Body ExtrudeBody;
                status = MoveFace(FaceToPull, out ExtrudeBody, out tempNXObject);
                if (!status)
                {
                    CaxLog.ShowListingWindow("MoveFace失敗");
                    CaxWE.RecordFailed(ref hideDispalyObject, WEComp, kvp);
                    return;
                }
                RemoveParameters(ExtrudeBody);

                //6.拉伸後的實體與電極相減
                status = J_Subtract(WEBody, ExtrudeBody, out tempNXObject);
                if (!status)
                {
                    CaxLog.ShowListingWindow("第二次J_Subtract失敗");
                    CaxWE.RecordFailed(ref hideDispalyObject, WEComp, kvp);
                    return;
                }
                RemoveParameters(WEBody);

                List<Face> ListXRayFace = new List<Face>();
                status = GetFaceToXRay(ExtrudeBody, out ListXRayFace);
                if (!status)
                {
                    CaxLog.ShowListingWindow("GetFaceToXRay 失敗");
                    CaxWE.RecordFailed(ref hideDispalyObject, WEComp, kvp);
                }


                Face[] WEBodyFaceAry = WEBody.GetFaces();
                List<Face> ListWEBodyFace = new List<Face>();
                ListWEBodyFace = WEBodyFaceAry.ToList();

                status = XRayGetFace(ListXRayFace, ListWEBodyFace, out ListFaceToInsertAttr);
                if (!status)
                {
                    CaxLog.ShowListingWindow("射線失敗");
                    CaxWE.RecordFailed(ref hideDispalyObject, WEComp, kvp);
                }
                RemoveParameters(WEBody);
                RemoveParameters(ExtrudeBody);
                DeleteBody(ExtrudeBody);
            }
            
            
        }

        public static bool DeleteNXObject(NXObject Nxobject)
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
                UI theUI = UI.GetUI();

                objects1[0] = Nxobject;
                int nErrs1;
                nErrs1 = theSession.UpdateManager.AddToDeleteList(objects1);

                bool notifyOnDelete2;
                notifyOnDelete2 = theSession.Preferences.Modeling.NotifyOnDelete;

                int nErrs2;
                nErrs2 = theSession.UpdateManager.DoUpdate(markId2);

                theSession.DeleteUndoMark(markId1, null);
            }
            catch (System.Exception ex)
            {
                //CaxPart.ShowListingWindow(ex.Message);
                return false;
            }

            return true;
        }

        private bool CreateBlock(double[] FirstPt, double[] SecondPt, out Body CreateBody, out NXObject tempNXObject)
        {
            CreateBody = null;
            tempNXObject = null;
            try
            {
                //CaxPart.AskBoundingBoxExactByWCS(BlockBody.Tag, out sBoundingBoxPt.minWCS, out sBoundingBoxPt.maxWCS);
                //sBoundingBoxPt.minWCS[2] = -1;
                FirstPt[2] = -1;
                //補實體
                status = TwoPtCreateBlock(FirstPt, SecondPt, out CreateBody, out tempNXObject);
                if (!status)
                {
                    CaxLog.ShowListingWindow("TwoPtCreateBlock失敗");
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool NewCreateBlock(double[] min, double[] max, double[] offset,out Body boxBody)
        {
            boxBody = null;
            try
            {
                Part workPart = theSession.Parts.Work;
                BlockFeatureBuilder blockFeatureBuilder1;
                Point3d originPoint1 = new Point3d(min[0] - offset[0], min[1] - offset[1], min[2] - offset[2]);
                Point3d cornerPoint1 = new Point3d(max[0] + offset[3], max[1] + offset[4], max[2] + offset[5]);
                if (blockFeat == null)
                {
                    blockFeatureBuilder1 = workPart.Features.CreateBlockFeatureBuilder(null);
                    blockFeatureBuilder1.Type = BlockFeatureBuilder.Types.DiagonalPoints;
                    NXOpen.Point orig = workPart.Points.CreatePoint(originPoint1);
                    NXOpen.Point corner = workPart.Points.CreatePoint(cornerPoint1);
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
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private bool GetFaceToXRay(Body ExtrudeBody, out List<Face> ListXRayFace)
        {
            ListXRayFace = new List<Face>();
            try
            {
                Face[] CreateBodyFaceAry = ExtrudeBody.GetFaces();
                double[] dir;
                double small_angle_radius, large_angle_radius, small_angle_degree, large_angle_degree;
                double[] Zdir = new double[3] { 0, 0, 1 };
                foreach (Face tempFace in CreateBodyFaceAry)
                {
                    CFace GetFaceNor = new CFace();
                    dir = GetFaceNor.GetNormal(tempFace.Tag);
                    //AskFaceCenter(tempFace, out center, out dir);
                    theUfSession.Modl.AskVectorAngle(Zdir, dir, out small_angle_radius, out large_angle_radius);
                    small_angle_degree = small_angle_radius * 180 / Math.PI;
                    if (small_angle_degree == 0 || small_angle_degree < 2)
                    { }
                    else
                    {
                        ListXRayFace.Add(tempFace);
                        //tempFace.Highlight();
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private bool CreateBlockToUniteEDMBody(Body BlockBody,out NXObject tempNXObject)
        {
            tempNXObject = null;
            try
            {
                CaxPart.AskBoundingBoxExactByWCS(BlockBody.Tag, out sBoundingBoxPt.minWCS, out sBoundingBoxPt.maxWCS);
                sBoundingBoxPt.maxWCS[2] = -1;
                //補實體
                Body CreateBody;
                NXObject CreateBodyObj = null;
                //CaxLog.ShowListingWindow("sBoundingBoxPt.minWCS[0]:" + sBoundingBoxPt.minWCS[0]);
                //CaxLog.ShowListingWindow("sBoundingBoxPt.minWCS[1]:" + sBoundingBoxPt.minWCS[1]);
                //CaxLog.ShowListingWindow("sBoundingBoxPt.minWCS[2]:" + sBoundingBoxPt.minWCS[2]);
                status = TwoPtCreateBlock(sBoundingBoxPt.minWCS, sBoundingBoxPt.maxWCS,out CreateBody,out CreateBodyObj);
                if (!status)
                {
                    CaxLog.ShowListingWindow("TwoPtCreateBlock失敗");
                }
                //RemoveParameters(CreateBody);


                //實體與電極相加
                status = UniteBody(WEBody, CreateBody, out tempNXObject);
                if (!status)
                {
                    CaxLog.ShowListingWindow("實體結合失敗");
                }
                //RemoveParameters(WEBody);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool GetFaceToPull(Body BlockBody, out Face FaceToPull,out NXObject tempNXObject)
        {
            FaceToPull = null;
            tempNXObject = null;
            try
            {
                status = J_Subtract(BlockBody, WEBody, out tempNXObject);
                if (!status)
                {
                    //CaxLog.ShowListingWindow("第一次J_Subtract失敗");
                }
                RemoveParameters(BlockBody);

                Face[] BlockBodyFaceAry = BlockBody.GetFaces();
                double[] center1;
                List<double> ListDistance = new List<double>();
                Dictionary<Face, double> FaceDist = new Dictionary<Face, double>();
                foreach (Face tempFace in BlockBodyFaceAry)
                {

                    AskFaceCenter(tempFace, out center1);

                    double distance = 0 - center1[2];

                    ListDistance.Add(Math.Abs(distance));

                    FaceDist[tempFace] = Math.Abs(distance);
                }
                ListDistance.Sort();

                foreach (KeyValuePair<Face, double> kvp in FaceDist)
                {
                    if (ListDistance[0] == kvp.Value)
                    {
                        FaceToPull = kvp.Key;
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool CreateBBox(Tag[] SelectFaceTagAry, out Body BlockBody,out NXObject theBlockObj)
        {
            BlockBody = null;
            theBlockObj = null;
            try
            {
                theBlock = CreateWrapBlock(SelectFaceTagAry);
                theBlockObj = (NXObject)NXObjectManager.Get(theBlock);
                BlockBody = (Body)NXObjectManager.Get(theBlock);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool SelectFaces(out TaggedObject[] SelectFaceTaggedObjectAry, out Tag[] SelectFaceTagAry)
        {
            SelectFaceTaggedObjectAry = null;
            SelectFaceTagAry = null;
            try
            {
                CaxPart.SelectFaces(out SelectFaceTaggedObjectAry);
                
                SelectFaceTagAry = new Tag[SelectFaceTaggedObjectAry.Length];

                for (int i = 0; i < SelectFaceTaggedObjectAry.Length; i++)
                {
                    SelectFaceTagAry[i] = SelectFaceTaggedObjectAry[i].Tag;
                    //Face bb = (Face)NXObjectManager.Get(SelectFaceTagAry[i]);
                    //bb.Highlight();
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool XRayGetFace(List<Face> ListXRayFace, List<Face> ListSelectBody1Face, out List<Face> ListFaceToInsertAttr)
        {
            double[] center = new double[3];
            double[] dir = new double[3];
            ListFaceToInsertAttr = new List<Face>();
            
            try
            {
                //CaxLog.ShowListingWindow("ListXRayFace.Count:" + ListXRayFace.Count);
                foreach (Face tempFace in ListXRayFace)
                {
                    //tempFace.Highlight();

                    /*
                    List<double[]> ListPoint3d = new List<double[]>();
                    Edge[] tempFaceEdgeAry = tempFace.GetEdges();
                    CaxLog.ShowListingWindow("tempFaceEdgeAry.Length:" + tempFaceEdgeAry.Length);
                    foreach (Edge tempEdge in tempFaceEdgeAry)
                    {
                        Point3d vertex1,vertex2;
                        tempEdge.GetVertices(out vertex1, out vertex2);
                        double[] ver1 = new double[3];
                        double[] ver2 = new double[3];
                        ver1[0] = Math.Round(vertex1.X, 2);
                        ver1[1] = Math.Round(vertex1.Y, 2);
                        ver1[2] = Math.Round(vertex1.Z, 2);
                        ver2[0] = Math.Round(vertex2.X, 2);
                        ver2[1] = Math.Round(vertex2.Y, 2);
                        ver2[2] = Math.Round(vertex2.Z, 2);
                        //CaxLog.ShowListingWindow("vertex1.X:" + ver1[0]);
                        //CaxLog.ShowListingWindow("vertex1.Y:" + ver1[1]);
                        //CaxLog.ShowListingWindow("vertex1.Z:" + ver1[2]);
                        //CaxLog.ShowListingWindow("vertex2.X:" + ver2[0]);
                        //CaxLog.ShowListingWindow("vertex2.Y:" + ver2[1]);
                        //CaxLog.ShowListingWindow("vertex2.Z:" + ver2[2]);
                        //CaxLog.ShowListingWindow("-----");
                        if (ListPoint3d.Count == 0)
                        {
                            ListPoint3d.Add(ver1);
                            ListPoint3d.Add(ver2);
                        }
                        else
                        {
                            int sum1 = 0;
                            foreach (double[] tempdoublePt in ListPoint3d)
                            {
                                if (ver1[0] != tempdoublePt[0] || ver1[1] != tempdoublePt[1] || ver1[2] != tempdoublePt[2])
                                {
                                    sum1++;
                                }
                            }
                            if (sum1 == ListPoint3d.Count)
                            {
                                ListPoint3d.Add(ver1);
                            }

                            int sum2 = 0;
                            foreach (double[] tempdoublePt in ListPoint3d)
                            {
                                if (ver2[0] != tempdoublePt[0] || ver2[1] != tempdoublePt[1] || ver2[2] != tempdoublePt[2])
                                {
                                    sum2++;
                                }
                            }
                            if (sum2 == ListPoint3d.Count)
                            {
                                ListPoint3d.Add(ver2);
                            }
                        }
                    }
                    //return true;
                    CaxLog.ShowListingWindow("ListPoint3d.count:" + ListPoint3d.Count);
                    double CenterX = 0.0;
                    double CenterY = 0.0;
                    double CenterZ = 0.0;
                    for (int i = 0; i < ListPoint3d.Count;i++ )
                    {
                        CenterX = (ListPoint3d[i][0] + ListPoint3d[i + 1][0]) / 2;
                    }
                    
                    CaxLog.ShowListingWindow("CenterX[0]:" + CenterX);
                    CaxLog.ShowListingWindow("CenterY[1]:" + CenterY);
                    CaxLog.ShowListingWindow("CenterZ[2]:" + CenterZ);
                    */

                    AskFaceCenter(tempFace, out center);
                    AskFaceDir(tempFace, out dir);
                    center[0] = center[0] - (dir[0] / 10);
                    center[1] = center[1] - (dir[1] / 10);
                    center[2] = center[2] - (dir[2] / 10);
                    //CaxLog.ShowListingWindow("center[0]:" + center[0]);
                    //CaxLog.ShowListingWindow("center[1]:" + center[1]);
                    //CaxLog.ShowListingWindow("center[2]:" + center[2]);
                    //CaxLog.ShowListingWindow("----");
                    UFModl.RayHitPointInfo cRayHitPointInfo;
                    CLA101_CSUGFunc.detectPointOnSurface(center, ListSelectBody1Face, out cRayHitPointInfo);
                    Face HitFace = (Face)NXObjectManager.Get(cRayHitPointInfo.hit_face);
                    //string aa = theUfSession.Tag.AskHandleOfTag(cRayHitPointInfo.hit_face);
                    //CaxLog.ShowListingWindow("aa:" + aa);
                    //CaxLog.ShowListingWindow("cRayHitPointInfo.hit_face:" + cRayHitPointInfo.hit_face);
                    ListFaceToInsertAttr.Add(HitFace);
                    //HitFace.Highlight();
                }
                //CaxLog.ShowListingWindow("ListFaceToInsertAttr:" + ListFaceToInsertAttr.Count);
                //foreach (Face temp in ListFaceToInsertAttr)
                //{
                //    CaxLog.ShowListingWindow("temp.Tag:" + temp.Tag);
                //}
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private void buttonX1_Closed_Click(object sender, EventArgs e)
        {
            if (!IsOK)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "請先調整線割加工範圍的方塊大小，確認後請按OK");
            }
           
        }

        public static Tag CreateWrapBlock(Tag[] objects)
        {
            Tag result = NXOpen.Tag.Null;
            double[] min = { 99999999, 99999999, 99999999 };
            double[] max = { -99999999, -99999999, -99999999 };
            double[] bbox = new double[6];

            //依序計算每個物件的最小方盒, 並計算總體的最小方盒
            for (int i = 0; i < objects.Length; i++)
            {
                theUfSession.Modl.AskBoundingBox(objects[i], bbox);
                min[0] = (min[0] < bbox[0]) ? min[0] : bbox[0];
                min[1] = (min[1] < bbox[1]) ? min[1] : bbox[1];
                min[2] = (min[2] < bbox[2]) ? min[2] : bbox[2];
                max[0] = (max[0] > bbox[3]) ? max[0] : bbox[3];
                max[1] = (max[1] > bbox[4]) ? max[1] : bbox[4];
                max[2] = (max[2] > bbox[5]) ? max[2] : bbox[5];
            }
            
            //取得最小方盒三個方向長度的字串表示
            Tag blockFeat = NXOpen.Tag.Null;
            string xLen = (max[0] - min[0]).ToString();
            string yLen = (max[1] - min[1]).ToString();
            string zLen = (max[2] - min[2]).ToString();
            string[] edgeLen = { xLen, yLen, zLen };

            //建立block特徵
            theUfSession.Modl.CreateBlock(FeatureSigns.Nullsign, NXOpen.Tag.Null, min, edgeLen, out blockFeat);
            if (blockFeat != NXOpen.Tag.Null)
            {
                //取得block特徵底下的body
                theUfSession.Modl.AskFeatBody(blockFeat, out result);
            }

            return result;
        }

        public static bool TwoPtCreateBlock(double[] firstpt, double[] secondpt,out Body CreateBody,out NXObject CreateBlockObj)
        {
            CreateBody = null;
            CreateBlockObj = null;
            try
            {
                theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Tools->Repeat Command->5 Block
                // ----------------------------------------------
                // ----------------------------------------------
                //   Menu: Insert->Design Feature->Block...
                // ----------------------------------------------
                //NXOpen.Session.UndoMarkId markId1;
                //markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.Feature nullFeatures_Feature = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.BlockFeatureBuilder blockFeatureBuilder1;
                blockFeatureBuilder1 = workPart.Features.CreateBlockFeatureBuilder(nullFeatures_Feature);

                blockFeatureBuilder1.BooleanOption.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

                Body[] targetBodies1 = new Body[1];
                Body nullBody = null;
                targetBodies1[0] = nullBody;
                blockFeatureBuilder1.BooleanOption.SetTargetBodies(targetBodies1);

                blockFeatureBuilder1.Type = NXOpen.Features.BlockFeatureBuilder.Types.DiagonalPoints;

                blockFeatureBuilder1.BooleanOption.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

                Body[] targetBodies2 = new Body[1];
                targetBodies2[0] = nullBody;
                blockFeatureBuilder1.BooleanOption.SetTargetBodies(targetBodies2);

                //theSession.SetUndoMarkName(markId1, "Block Dialog");

                Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
                Expression expression1;
                expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                blockFeatureBuilder1.Type = NXOpen.Features.BlockFeatureBuilder.Types.DiagonalPoints;
                //blockFeatureBuilder1.Type = NXOpen.Features.BlockFeatureBuilder.Types.TwoPointsAndHeight;

                Point3d coordinates1 = new Point3d(0.0, 0.0, 0.0);
                NXOpen.Point point1;
                point1 = workPart.Points.CreatePoint(coordinates1);

                /*
                Expression expression2;
                expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("p51_x=-0.0386312072018633", unit1);

                Scalar scalar1;
                scalar1 = workPart.Scalars.CreateScalarExpression(expression2, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression3;
                expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("p52_y=17.980822888681", unit1);

                Scalar scalar2;
                scalar2 = workPart.Scalars.CreateScalarExpression(expression3, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression4;
                expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("p53_z=0", unit1);

                Scalar scalar3;
                scalar3 = workPart.Scalars.CreateScalarExpression(expression4, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Point point2;
                point2 = workPart.Points.CreatePoint(scalar1, scalar2, scalar3, NXOpen.SmartObject.UpdateOption.WithinModeling);

                CaxLog.ShowListingWindow("point2:" + point2.Coordinates.X);
                CaxLog.ShowListingWindow("point2:" + point2.Coordinates.Y);
                CaxLog.ShowListingWindow("point2:" + point2.Coordinates.Z);
                */

                int nErrs1;
                nErrs1 = theSession.UpdateManager.AddToDeleteList(point1);

                /*
                Expression expression5;
                expression5 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                Expression expression6;
                expression6 = workPart.Expressions.CreateSystemExpressionWithUnits("p55_x=4.50173956585706", unit1);

                Scalar scalar4;
                scalar4 = workPart.Scalars.CreateScalarExpression(expression6, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression7;
                expression7 = workPart.Expressions.CreateSystemExpressionWithUnits("p56_y=32.4312695502503", unit1);

                Scalar scalar5;
                scalar5 = workPart.Scalars.CreateScalarExpression(expression7, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression8;
                expression8 = workPart.Expressions.CreateSystemExpressionWithUnits("p57_z=0", unit1);

                Scalar scalar6;
                scalar6 = workPart.Scalars.CreateScalarExpression(expression8, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Point point3;
                point3 = workPart.Points.CreatePoint(scalar4, scalar5, scalar6, NXOpen.SmartObject.UpdateOption.WithinModeling);

                CaxLog.ShowListingWindow("point3:" + point3.Coordinates.X);
                CaxLog.ShowListingWindow("point3:" + point3.Coordinates.Y);
                CaxLog.ShowListingWindow("point3:" + point3.Coordinates.Z);
                */

                //NXOpen.Session.UndoMarkId markId2;
                //markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Block");

                //theSession.DeleteUndoMark(markId2, null);

                //NXOpen.Session.UndoMarkId markId3;
                //markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Block");

                blockFeatureBuilder1.Type = NXOpen.Features.BlockFeatureBuilder.Types.DiagonalPoints;
                //blockFeatureBuilder1.Type = NXOpen.Features.BlockFeatureBuilder.Types.TwoPointsAndHeight;

                Point3d FirstPt3d = new Point3d();
                FirstPt3d.X = firstpt[0];
                FirstPt3d.Y = firstpt[1];
                FirstPt3d.Z = firstpt[2];

                Point3d SecondPt3d = new Point3d();
                SecondPt3d.X = secondpt[0];
                SecondPt3d.Y = secondpt[1];
                SecondPt3d.Z = secondpt[2];

                NXOpen.Point FirstPt = workPart.Points.CreatePoint(FirstPt3d);
                NXOpen.Point SecondPt = workPart.Points.CreatePoint(SecondPt3d);

                //blockFeatureBuilder1.OriginPoint = point2;
                blockFeatureBuilder1.OriginPoint = FirstPt;
                //blockFeatureBuilder1.PointFromOrigin = point3;
                blockFeatureBuilder1.PointFromOrigin = SecondPt;

                Point3d originPoint1 = new Point3d(firstpt[0], firstpt[1], firstpt[2]);
                Point3d cornerPoint1 = new Point3d(secondpt[0], secondpt[1], secondpt[2]);
                blockFeatureBuilder1.SetTwoDiagonalPoints(originPoint1, cornerPoint1);
                //blockFeatureBuilder1.SetTwoPointsAndHeight(originPoint1, cornerPoint1, "20");

                blockFeatureBuilder1.SetBooleanOperationAndTarget(NXOpen.Features.Feature.BooleanType.Create, nullBody);

                NXOpen.Features.Feature feature1;
                feature1 = blockFeatureBuilder1.CommitFeature();

                CreateBlockObj = (NXObject)feature1;
                

                //CaxLog.ShowListingWindow("aa:" + aa);

                Tag BodyTag = NXOpen.Tag.Null;
                theUfSession.Modl.AskFeatBody(feature1.Tag, out BodyTag);

                CreateBody = (Body)NXObjectManager.Get(BodyTag);

                //theSession.DeleteUndoMark(markId3, null);

                //theSession.SetUndoMarkName(markId1, "Block");

                blockFeatureBuilder1.Destroy();

                workPart.Expressions.Delete(expression1);

                //workPart.Expressions.Delete(expression5);

                // ----------------------------------------------
                //   Menu: Tools->Journal->Stop Recording
                // ----------------------------------------------
            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow(ex.ToString());
                return false;
            }
            return true;
        }

        public static bool J_Subtract(Body TargetBody, Body ToolBody, out NXObject nXObject1)
        {
            nXObject1 = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Combine->Subtract...
                // ----------------------------------------------
                //NXOpen.Session.UndoMarkId markId1;
                //markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.BooleanFeature nullFeatures_BooleanFeature = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.BooleanBuilder booleanBuilder1;
                booleanBuilder1 = workPart.Features.CreateBooleanBuilderUsingCollector(nullFeatures_BooleanFeature);

                ScCollector scCollector1;
                scCollector1 = booleanBuilder1.ToolBodyCollector;

                NXOpen.GeometricUtilities.BooleanRegionSelect booleanRegionSelect1;
                booleanRegionSelect1 = booleanBuilder1.BooleanRegionSelect;

                booleanBuilder1.Tolerance = 0.002;

                booleanBuilder1.CopyTools = true;

                booleanBuilder1.Operation = NXOpen.Features.Feature.BooleanType.Subtract;

                //theSession.SetUndoMarkName(markId1, "##17Subtract Dialog");

                //Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(4)");
                Body body1 = TargetBody;
                bool added1;
                added1 = booleanBuilder1.Targets.Add(body1);

                TaggedObject[] targets1 = new TaggedObject[1];
                targets1[0] = body1;
                booleanRegionSelect1.AssignTargets(targets1);

                ScCollector scCollector2;
                scCollector2 = workPart.ScCollectors.CreateCollector();

                Body[] bodies1 = new Body[1];
                Body body2 = ToolBody;
                //Body body2 = (Body)workPart.Bodies.FindObject("BLOCK(6)");
                bodies1[0] = body2;
                BodyDumbRule bodyDumbRule1;
                bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = bodyDumbRule1;
                scCollector2.ReplaceRules(rules1, false);

                booleanBuilder1.ToolBodyCollector = scCollector2;

                TaggedObject[] targets2 = new TaggedObject[1];
                targets2[0] = body1;
                booleanRegionSelect1.AssignTargets(targets2);

                //NXOpen.Session.UndoMarkId markId2;
                //markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "##17Subtract");

                //theSession.DeleteUndoMark(markId2, null);

                //NXOpen.Session.UndoMarkId markId3;
                //markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "##17Subtract");

                //NXObject nXObject1;
                nXObject1 = booleanBuilder1.Commit();

                //theSession.DeleteUndoMark(markId3, null);

                //theSession.SetUndoMarkName(markId1, "##17Subtract");

                booleanBuilder1.Destroy();

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

        public static bool RemoveParameters(Body killparameter)
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Edit->Feature->Remove Parameters...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.RemoveParametersBuilder removeParametersBuilder1;
                removeParametersBuilder1 = workPart.Features.CreateRemoveParametersBuilder();

                theSession.SetUndoMarkName(markId1, "Remove Parameters Dialog");

                //Body body1 = (Body)workPart.Bodies.FindObject("LINKED_BODY(1)");
                Body body1 = killparameter;
                bool added1;
                added1 = removeParametersBuilder1.Objects.Add(body1);

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Remove Parameters");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Remove Parameters");

                NXObject nXObject1;
                nXObject1 = removeParametersBuilder1.Commit();

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "Remove Parameters");

                removeParametersBuilder1.Destroy();

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

        public static bool AskFaceCenter(Face sFace, out double[] center)
        {
            center = new double[3];
            
            try
            {
                int type;
                double[] dir = new double[3];
                double[] point = new double[3];
                double[] box = new double[6];
                double radius;
                double rad_data;
                int norm_dir;
                theUfSession.Modl.AskFaceData(sFace.Tag, out type, point, dir, box, out radius, out rad_data, out norm_dir);
                center[0] = (box[0] + box[3]) / 2;
                center[1] = (box[1] + box[4]) / 2;
                center[2] = (box[2] + box[5]) / 2;
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool AskFaceDir(Face sFace, out double[] dir)
        {
            dir = new double[3];
            try
            {
                CFace ff = new CFace();
                dir = ff.GetNormal(sFace.Tag);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool AskFaceType(Face sFace, out int type)
        {
            type = -1;
            try
            {
                double[] dir = new double[3];
                double[] point = new double[3];
                double[] box = new double[6];
                double radius;
                double rad_data;
                int norm_dir;
                theUfSession.Modl.AskFaceData(sFace.Tag, out type, point, dir, box, out radius, out rad_data, out norm_dir);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
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

        public static bool TrimBody(Body TargetBody,Body ToolBody,out Body TrimmedBody)
        {
            TrimmedBody = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Trim->Trim Body...
                // ----------------------------------------------
                //NXOpen.Session.UndoMarkId markId1;
                //markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.TrimBody2 nullFeatures_TrimBody2 = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.TrimBody2Builder trimBody2Builder1;
                trimBody2Builder1 = workPart.Features.CreateTrimBody2Builder(nullFeatures_TrimBody2);

                Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
                Vector3d normal1 = new Vector3d(0.0, 0.0, 1.0);
                Plane plane1;
                plane1 = workPart.Planes.CreatePlane(origin1, normal1, NXOpen.SmartObject.UpdateOption.WithinModeling);

                trimBody2Builder1.BooleanTool.FacePlaneTool.ToolPlane = plane1;

                Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
                Expression expression1;
                expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                Expression expression2;
                expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                trimBody2Builder1.Tolerance = 0.002;

                //theSession.SetUndoMarkName(markId1, "Trim Body Dialog");

                trimBody2Builder1.BooleanTool.ExtrudeRevolveTool.ToolSection.DistanceTolerance = 0.002;

                trimBody2Builder1.BooleanTool.ExtrudeRevolveTool.ToolSection.ChainingTolerance = 0.0019;

                ScCollector scCollector1;
                scCollector1 = workPart.ScCollectors.CreateCollector();

                Body[] bodies1 = new Body[1];
                //Body body1 = (Body)workPart.Bodies.FindObject("BLOCK(3)");
                Body body1 = TargetBody;
                bodies1[0] = body1;
                BodyDumbRule bodyDumbRule1;
                bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = bodyDumbRule1;
                scCollector1.ReplaceRules(rules1, false);

                trimBody2Builder1.TargetBodyCollector = scCollector1;

                Body body2 = (Body)workPart.Bodies.FindObject(ToolBody.Prototype.JournalIdentifier);
                FaceBodyRule faceBodyRule1;
                faceBodyRule1 = workPart.ScRuleFactory.CreateRuleFaceBody(body2);

                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = faceBodyRule1;
                trimBody2Builder1.BooleanTool.FacePlaneTool.ToolFaces.FaceCollector.ReplaceRules(rules2, false);

                //NXOpen.Session.UndoMarkId markId2;
                //markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Trim Body");

                //theSession.DeleteUndoMark(markId2, null);

                //NXOpen.Session.UndoMarkId markId3;
                //markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Trim Body");

                NXObject nXObject1;
                nXObject1 = trimBody2Builder1.Commit();
                //CaxLog.ShowListingWindow("nXObject1.GetType:" + nXObject1.GetType());
                NXOpen.Features.TrimBody2 sTrimBody2 = (NXOpen.Features.TrimBody2)nXObject1;
                //CaxLog.ShowListingWindow("sTrimBody2:" + sTrimBody2.ToString());

                Tag bodytag = NXOpen.Tag.Null;
                theUfSession.Modl.AskFeatBody(sTrimBody2.Tag,out bodytag);
                //CaxLog.ShowListingWindow("bodytag:" + bodytag.ToString());

                TrimmedBody = (Body)NXObjectManager.Get(bodytag);

                //Body newbody = (Body)nXObject1;
                //CaxLog.ShowListingWindow("newbody:"+newbody.ToString());

                //theSession.DeleteUndoMark(markId3, null);

                //theSession.SetUndoMarkName(markId1, "Trim Body");

                trimBody2Builder1.Destroy();

                try
                {
                    // Expression is still in use.
                    workPart.Expressions.Delete(expression2);
                }
                catch (NXException ex)
                {
                    ex.AssertErrorCode(1050029);
                }

                try
                {
                    // Expression is still in use.
                    workPart.Expressions.Delete(expression1);
                }
                catch (NXException ex)
                {
                    ex.AssertErrorCode(1050029);
                }

                plane1.DestroyPlane();

                // ----------------------------------------------
                //   Menu: Tools->Journal->Stop Recording
                // ----------------------------------------------
            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow(ex.ToString());
                return false;
            }
            return true;
        }

        public static bool ExtrudeFace(Face TargetFace,out Body ExtrudeBody)
        {
            ExtrudeBody = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Design Feature->Extrude...
                // ----------------------------------------------
                //NXOpen.Session.UndoMarkId markId1;
                //markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.Feature nullFeatures_Feature = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.ExtrudeBuilder extrudeBuilder1;
                extrudeBuilder1 = workPart.Features.CreateExtrudeBuilder(nullFeatures_Feature);

                Section section1;
                section1 = workPart.Sections.CreateSection(0.0019, 0.002, 0.01);

                extrudeBuilder1.Section = section1;

                extrudeBuilder1.AllowSelfIntersectingSection(true);

                Unit unit1;
                unit1 = extrudeBuilder1.Draft.FrontDraftAngle.Units;

                Expression expression1;
                expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("2.00", unit1);

                extrudeBuilder1.DistanceTolerance = 0.002;

                extrudeBuilder1.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

                Body[] targetBodies1 = new Body[1];
                Body nullBody = null;
                targetBodies1[0] = nullBody;
                extrudeBuilder1.BooleanOperation.SetTargetBodies(targetBodies1);

                extrudeBuilder1.Limits.StartExtend.Value.RightHandSide = "-5";

                extrudeBuilder1.Limits.EndExtend.Value.RightHandSide = "10";

                extrudeBuilder1.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

                Body[] targetBodies2 = new Body[1];
                targetBodies2[0] = nullBody;
                extrudeBuilder1.BooleanOperation.SetTargetBodies(targetBodies2);

                extrudeBuilder1.Draft.FrontDraftAngle.RightHandSide = "2";

                extrudeBuilder1.Draft.BackDraftAngle.RightHandSide = "2";

                extrudeBuilder1.Offset.StartOffset.RightHandSide = "0";

                extrudeBuilder1.Offset.EndOffset.RightHandSide = "5";

                NXOpen.GeometricUtilities.SmartVolumeProfileBuilder smartVolumeProfileBuilder1;
                smartVolumeProfileBuilder1 = extrudeBuilder1.SmartVolumeProfile;

                smartVolumeProfileBuilder1.OpenProfileSmartVolumeOption = false;

                smartVolumeProfileBuilder1.CloseProfileRule = NXOpen.GeometricUtilities.SmartVolumeProfileBuilder.CloseProfileRuleType.Fci;

                //theSession.SetUndoMarkName(markId1, "Extrude Dialog");

                section1.DistanceTolerance = 0.002;

                section1.ChainingTolerance = 0.0019;

                section1.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

                //NXOpen.Session.UndoMarkId markId2;
                //markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                Face[] facesOfFeatures1 = new Face[1];
                //NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject("UNPARAMETERIZED_FEATURE(2)");
                //Face face1 = (Face)brep1.FindObject("FACE 26 {(0.9910099493205,-2.4909997,0) UNPARAMETERIZED_FEATURE(2)}");
                Face face1 = TargetFace;
                facesOfFeatures1[0] = face1;
                EdgeBoundaryRule edgeBoundaryRule1;
                edgeBoundaryRule1 = workPart.ScRuleFactory.CreateRuleEdgeBoundary(facesOfFeatures1);

                section1.AllowSelfIntersection(true);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = edgeBoundaryRule1;
                NXObject nullNXObject = null;
                Point3d helpPoint1 = new Point3d(0.0, 0.0, 0.0);
                section1.AddToSection(rules1, nullNXObject, nullNXObject, nullNXObject, helpPoint1, NXOpen.Section.Mode.Create, false);

                //theSession.DeleteUndoMark(markId2, null);

                //NXOpen.Session.UndoMarkId markId3;
                //markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

                //NXOpen.Session.UndoMarkId markId4;
                //markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                //theSession.DeleteUndoMark(markId4, null);

                Direction direction1;
                direction1 = workPart.Directions.CreateDirection(face1, Sense.Forward, NXOpen.SmartObject.UpdateOption.WithinModeling);

                extrudeBuilder1.Direction = direction1;

                //theSession.DeleteUndoMark(markId3, null);

                extrudeBuilder1.Limits.StartExtend.Value.RightHandSide = "-6";

                extrudeBuilder1.Limits.EndExtend.Value.RightHandSide = "11";

                //NXOpen.Session.UndoMarkId markId5;
                //markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Extrude");

                //theSession.DeleteUndoMark(markId5, null);

                //NXOpen.Session.UndoMarkId markId6;
                //markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Extrude");

                extrudeBuilder1.ParentFeatureInternal = false;

                NXOpen.Features.Feature feature1;
                feature1 = extrudeBuilder1.CommitFeature();

                Tag BodyTag = NXOpen.Tag.Null;
                theUfSession.Modl.AskFeatBody(feature1.Tag, out BodyTag);

                ExtrudeBody = (Body)NXObjectManager.Get(BodyTag);


                //theSession.DeleteUndoMark(markId6, null);

                //theSession.SetUndoMarkName(markId1, "Extrude");

                Expression expression2 = extrudeBuilder1.Limits.StartExtend.Value;
                Expression expression3 = extrudeBuilder1.Limits.EndExtend.Value;
                extrudeBuilder1.Destroy();

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

        public static bool MoveFace(Face TargetFace, out Body ExtrudeBody,out NXObject nXObject1)
        {
            ExtrudeBody = null;
            nXObject1 = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Tools->Repeat Command->3 Move Face
                // ----------------------------------------------
                // ----------------------------------------------
                //   Menu: Insert->Synchronous Modeling->Move Face...
                // ----------------------------------------------
                //NXOpen.Session.UndoMarkId markId1;
                //markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.AdmMoveFace nullFeatures_AdmMoveFace = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.AdmMoveFaceBuilder admMoveFaceBuilder1;
                admMoveFaceBuilder1 = workPart.Features.CreateAdmMoveFaceBuilder(nullFeatures_AdmMoveFace);

                admMoveFaceBuilder1.FaceToMove.RelationScope = 511;

                admMoveFaceBuilder1.Motion.DistanceAngle.OrientXpress.AxisOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Axis.Passive;

                admMoveFaceBuilder1.Motion.DistanceAngle.OrientXpress.PlaneOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Plane.Passive;

                admMoveFaceBuilder1.Motion.AlongCurveAngle.AlongCurve.IsPercentUsed = true;

                admMoveFaceBuilder1.Motion.AlongCurveAngle.AlongCurve.Expression.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.AlongCurveAngle.AlongCurve.Expression.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.OrientXpress.AxisOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Axis.Passive;

                admMoveFaceBuilder1.Motion.OrientXpress.PlaneOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Plane.Passive;

                admMoveFaceBuilder1.FaceToMove.CoplanarEnabled = false;

                admMoveFaceBuilder1.FaceToMove.CoplanarAxesEnabled = false;

                admMoveFaceBuilder1.FaceToMove.CoaxialEnabled = false;

                admMoveFaceBuilder1.FaceToMove.EqualDiameterEnabled = false;

                admMoveFaceBuilder1.FaceToMove.TangentEnabled = false;

                admMoveFaceBuilder1.FaceToMove.SymmetricEnabled = false;

                admMoveFaceBuilder1.FaceToMove.OffsetEnabled = false;

                admMoveFaceBuilder1.FaceToMove.UseFaceBrowse = true;

                admMoveFaceBuilder1.Motion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.DistanceAngle;

                admMoveFaceBuilder1.Motion.DistanceValue.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.DistanceValue.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.DistanceBetweenPointsDistance.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.DistanceBetweenPointsDistance.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.RadialDistance.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.RadialDistance.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.Angle.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.Angle.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.DistanceAngle.Distance.RightHandSide = "9.5";

                admMoveFaceBuilder1.Motion.DistanceAngle.Distance.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.DistanceAngle.Angle.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.DistanceAngle.Angle.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.DeltaEnum = NXOpen.GeometricUtilities.ModlMotion.Delta.ReferenceAcsWorkPart;

                admMoveFaceBuilder1.Motion.DeltaXc.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.DeltaYc.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.DeltaZc.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.AlongCurveAngle.AlongCurve.Expression.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.AlongCurveAngle.AlongCurve.Expression.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.AlongCurveAngle.AlongCurveAngle.RightHandSide = "0";

                admMoveFaceBuilder1.Motion.AlongCurveAngle.AlongCurveAngle.RightHandSide = "0";

                //theSession.SetUndoMarkName(markId1, "Move Face Dialog");

                ScCollector scCollector1 = (ScCollector)workPart.FindObject("ENTITY 113 2");
                SelectionIntentRule[] rules1 = new SelectionIntentRule[0];
                scCollector1.ReplaceRules(rules1, false);

                Face[] faces1 = new Face[1];
                //NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject("UNPARAMETERIZED_FEATURE(2)");
                //Face face1 = (Face)brep1.FindObject("FACE 1 {(1,-2.5,3.00025) UNPARAMETERIZED_FEATURE(2)}");
                Face face1 = TargetFace;
                faces1[0] = face1;
                FaceDumbRule faceDumbRule1;
                faceDumbRule1 = workPart.ScRuleFactory.CreateRuleFaceDumb(faces1);

                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = faceDumbRule1;
                admMoveFaceBuilder1.FaceToMove.FaceCollector.ReplaceRules(rules2, false);

                Scalar scalar1;
                scalar1 = workPart.Scalars.CreateScalar(0.613294925794143, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Scalar scalar2;
                scalar2 = workPart.Scalars.CreateScalar(0.503595058044525, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                NXOpen.Point point1;
                point1 = workPart.Points.CreatePoint(face1, scalar1, scalar2, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Direction direction1;
                direction1 = workPart.Directions.CreateDirection(face1, point1, Sense.Forward, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Axis axis1;
                axis1 = workPart.Axes.CreateAxis(point1, direction1, NXOpen.SmartObject.UpdateOption.WithinModeling);

                admMoveFaceBuilder1.Motion.DistanceAngle.LinearAxis = axis1;

                NXOpen.Point point2;
                point2 = workPart.Points.CreatePoint(face1, scalar1, scalar2, NXOpen.SmartObject.UpdateOption.WithinModeling);

                NXOpen.Point point3;
                point3 = workPart.Points.CreatePoint(face1, scalar1, scalar2, NXOpen.SmartObject.UpdateOption.WithinModeling);

                admMoveFaceBuilder1.Motion.DistanceAngle.LinearAxis = axis1;

                workPart.Points.DeletePoint(point3);

                NXOpen.Point point4;
                point4 = workPart.Points.CreatePoint(face1, scalar1, scalar2, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Unit unit1;
                unit1 = admMoveFaceBuilder1.Motion.RadialOriginDistance.Units;

                Expression expression1;
                expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                workPart.Points.DeletePoint(point4);

                admMoveFaceBuilder1.Motion.DistanceAngle.LinearAxis = axis1;

                Vector3d angulardirection1 = new Vector3d(0.0, 1.0, 0.0);
                admMoveFaceBuilder1.Motion.DistanceAngle.AngularDirection = angulardirection1;

                admMoveFaceBuilder1.Motion.DistanceAngle.Distance.RightHandSide = "17";

                admMoveFaceBuilder1.Motion.DistanceAngle.Distance.RightHandSide = "17";

                //NXOpen.Session.UndoMarkId markId2;
                //markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Face");

                //theSession.DeleteUndoMark(markId2, null);

                //NXOpen.Session.UndoMarkId markId3;
                //markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Face");

                //NXObject nXObject1;
                nXObject1 = admMoveFaceBuilder1.Commit();

                NXOpen.Features.AdmMoveFace feat = (NXOpen.Features.AdmMoveFace)nXObject1;

                Tag ExtrudeBodyTag = NXOpen.Tag.Null;
                theUfSession.Modl.AskFeatBody(feat.Tag, out ExtrudeBodyTag);

                ExtrudeBody = (Body)NXObjectManager.Get(ExtrudeBodyTag);

                //CaxLog.ShowListingWindow(ExtrudeBody.ToString());


                //theSession.DeleteUndoMark(markId3, null);

                //theSession.SetUndoMarkName(markId1, "Move Face");

                Expression expression2 = admMoveFaceBuilder1.Motion.DistanceAngle.Distance;
                Expression expression3 = admMoveFaceBuilder1.Motion.DistanceAngle.Angle;
                admMoveFaceBuilder1.Destroy();

                workPart.Points.DeletePoint(point2);

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

        public static bool UniteBody(Body TargetBody, Body ToolBody, out NXObject nXObject1)
        {
            nXObject1 = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Combine->Unite...
                // ----------------------------------------------
                //NXOpen.Session.UndoMarkId markId1;
                //markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.BooleanFeature nullFeatures_BooleanFeature = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.BooleanBuilder booleanBuilder1;
                booleanBuilder1 = workPart.Features.CreateBooleanBuilderUsingCollector(nullFeatures_BooleanFeature);

                ScCollector scCollector1;
                scCollector1 = booleanBuilder1.ToolBodyCollector;

                NXOpen.GeometricUtilities.BooleanRegionSelect booleanRegionSelect1;
                booleanRegionSelect1 = booleanBuilder1.BooleanRegionSelect;

                booleanBuilder1.Tolerance = 0.002;

                booleanBuilder1.Operation = NXOpen.Features.Feature.BooleanType.Unite;

                //theSession.SetUndoMarkName(markId1, "##12Unite Dialog");

                //Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(1)");
                Body body1 = TargetBody;
                bool added1;
                added1 = booleanBuilder1.Targets.Add(body1);

                TaggedObject[] targets1 = new TaggedObject[1];
                targets1[0] = body1;
                booleanRegionSelect1.AssignTargets(targets1);

                ScCollector scCollector2;
                scCollector2 = workPart.ScCollectors.CreateCollector();

                Body[] bodies1 = new Body[1];
                //Body body2 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(4)");
                Body body2 = ToolBody;
                bodies1[0] = body2;
                BodyDumbRule bodyDumbRule1;
                bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = bodyDumbRule1;
                scCollector2.ReplaceRules(rules1, false);

                booleanBuilder1.ToolBodyCollector = scCollector2;

                TaggedObject[] targets2 = new TaggedObject[1];
                targets2[0] = body1;
                booleanRegionSelect1.AssignTargets(targets2);

                //NXOpen.Session.UndoMarkId markId2;
                //markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "##12Unite");

                //theSession.DeleteUndoMark(markId2, null);

                //NXOpen.Session.UndoMarkId markId3;
                //markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "##12Unite");

                //NXObject nXObject1;
                nXObject1 = booleanBuilder1.Commit();

                //theSession.DeleteUndoMark(markId3, null);

                //theSession.SetUndoMarkName(markId1, "##12Unite");

                booleanBuilder1.Destroy();

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
    }
}
