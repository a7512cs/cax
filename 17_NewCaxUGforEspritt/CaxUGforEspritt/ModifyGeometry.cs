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

namespace CaxUGforEspritt
{
    public partial class ModifyGeometry : DevComponents.DotNetBar.Office2007Form
    {

        #region 全域變數
        public static Face TargetFace;//欲被取代的面
        public static Face ToolFace;//取代的面
        public static bool chkUndo;//檢查是否有執行動作
        public static bool status = false;
        #endregion

        public ModifyGeometry()
        {
            InitializeComponent();
        }

        private void ModifyGeometry_Load(object sender, EventArgs e)
        {

        }

        private void buttonX2Confirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonX3Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonX1TargetFace_Click(object sender, EventArgs e)
        {
            TaggedObject selectFaceObj ;
            Point3d pt;
            status = CaxPart.SelectFace(out selectFaceObj, out pt);
            if (status)
            {
                buttonX1TargetFace.Text = "已指定Target Face";
            }
            else
            {
                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "請選擇Target Face");
                return;
            }
            Tag selectFaceTag = (Tag)selectFaceObj.Tag;
            TargetFace = (Face)NXObjectManager.Get(selectFaceTag);
            TargetFace.Highlight();
        }

        private void buttonX1ToolFace_Click(object sender, EventArgs e)
        {
            TaggedObject selectFaceObj;
            Point3d pt;
            status = CaxPart.SelectFace(out selectFaceObj, out pt);
            if (status)
            {
                buttonX1ToolFace.Text = "已指定Tool Face";
            }
            else
            {
                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "請選擇Tool Face");
                return;
            }
            Tag selectFaceTag = (Tag)selectFaceObj.Tag;
            ToolFace = (Face)NXObjectManager.Get(selectFaceTag);
            ToolFace.Highlight();
        }

        private void buttonX1Execute_Click(object sender, EventArgs e)
        {
            
            status = ReplaceFace(TargetBody, TargetFace, ToolFace);
            if (!status)
            {
                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "取代失敗");
            }
            else
            {
                TargetFace.Unhighlight();
                ToolFace.Unhighlight();
                buttonX1TargetFace.Text = "1.選擇欲被取代的面(Target Face)";
                buttonX1ToolFace.Text = "2.選擇欲取代的面(Tool Face)";
                chkUndo = true;
            }
        }

        private void buttonX1Undo_Click(object sender, EventArgs e)
        {
            if (chkUndo)
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;

                bool marksRecycled1;
                bool undoUnavailable1;
                theSession.UndoLastNVisibleMarks(1, out marksRecycled1, out undoUnavailable1);
            }
        }

        public static bool NewReplaceFace(Body TargetBody, Face TargeFace, Body ToolBody, Face ToolFace)
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Synchronous Modeling->Replace Face...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.Feature nullFeatures_Feature = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.ReplaceFaceBuilder replaceFaceBuilder1;
                replaceFaceBuilder1 = workPart.Features.CreateReplaceFaceBuilder(nullFeatures_Feature);

                replaceFaceBuilder1.OffsetDistance.RightHandSide = "0";

                replaceFaceBuilder1.OffsetDistance.RightHandSide = "0";

                theSession.SetUndoMarkName(markId1, "Replace Face Dialog");

                Face[] faces1 = new Face[1];
                NXOpen.Features.Block block1 = (NXOpen.Features.Block)workPart.Features.FindObject(TargetBody.JournalIdentifier);
                Face face1 = TargeFace;
                //NXOpen.Features.Block block1 = (NXOpen.Features.Block)workPart.Features.FindObject(TargetBody.JournalIdentifier);
                //Face face1 = (Face)block1.FindObject(TargeFace.JournalIdentifier);
                faces1[0] = face1;
                FaceDumbRule faceDumbRule1;
                faceDumbRule1 = workPart.ScRuleFactory.CreateRuleFaceDumb(faces1);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = faceDumbRule1;
                replaceFaceBuilder1.FaceToReplace.ReplaceRules(rules1, false);

                Face[] faces2 = new Face[1];
                NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject(ToolBody.JournalIdentifier);
                Face face2 = (Face)brep1.FindObject(ToolFace.JournalIdentifier);
                faces2[0] = face2;
                FaceDumbRule faceDumbRule2;
                faceDumbRule2 = workPart.ScRuleFactory.CreateRuleFaceDumb(faces2);

                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = faceDumbRule2;
                replaceFaceBuilder1.ReplacementFaces.ReplaceRules(rules2, false);

                replaceFaceBuilder1.ReverseDirection = false;

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Replace Face");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Replace Face");

                NXObject nXObject1;
                nXObject1 = replaceFaceBuilder1.Commit();

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "Replace Face");

                Expression expression1 = replaceFaceBuilder1.OffsetDistance;
                replaceFaceBuilder1.Destroy();

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

        public static bool ReplaceFace(Body body, Face TargeFace, Face ToolFace)
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Synchronous Modeling->Replace Face...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.Feature nullFeatures_Feature = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.ReplaceFaceBuilder replaceFaceBuilder1;
                replaceFaceBuilder1 = workPart.Features.CreateReplaceFaceBuilder(nullFeatures_Feature);

                replaceFaceBuilder1.OffsetDistance.RightHandSide = "0";

                replaceFaceBuilder1.OffsetDistance.RightHandSide = "0";

                theSession.SetUndoMarkName(markId1, "Replace Face Dialog");

                Face[] faces1 = new Face[1];
                NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject(body.JournalIdentifier);
                Face face1 = (Face)TargeFace.Prototype;
                faces1[0] = face1;
                FaceDumbRule faceDumbRule1;
                faceDumbRule1 = workPart.ScRuleFactory.CreateRuleFaceDumb(faces1);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = faceDumbRule1;
                replaceFaceBuilder1.FaceToReplace.ReplaceRules(rules1, false);

                Face[] faces2 = new Face[1];
                Face face2 = (Face)ToolFace.Prototype;
                faces2[0] = face2;
                FaceDumbRule faceDumbRule2;
                faceDumbRule2 = workPart.ScRuleFactory.CreateRuleFaceDumb(faces2);

                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = faceDumbRule2;
                replaceFaceBuilder1.ReplacementFaces.ReplaceRules(rules2, false);

                replaceFaceBuilder1.ReverseDirection = false;

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Replace Face");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Replace Face");

                NXObject nXObject1;
                nXObject1 = replaceFaceBuilder1.Commit();

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "Replace Face");

                Expression expression1 = replaceFaceBuilder1.OffsetDistance;
                replaceFaceBuilder1.Destroy();

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
