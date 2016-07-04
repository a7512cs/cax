using System;
using NXOpen;
using NXOpen.UF;
using System.Collections.Generic;
using System.Windows.Forms;
using NXOpen.Utilities;
using CimforceCaxTwPublic;
using CimforceCaxTwMD;
using CimforceCaxTwMFG;
using System.IO;
using NXCustomerComponent;
using System.Collections;
using System.Data;
using CaxUFLib;
using NXOpen.Assemblies;
using NXOpen.Features;
using CaxUGforEspritt;
using System.Threading;
using WeData;
using WE_Get_Thrugh_Pnt;

public class MelodyWrapper
{
    public Dictionary<WeListKey, WeFaceGroup> WEFaceDict;
}

#region CAX LEO TW1409007 20141025
public class MACHINETYPE
{
    public List<Type> Types;
    public struct Type
    {
        public string key;
        public int type;
    }

    public MACHINETYPE()
    {
        Types = new List<Type>();
        string[] orderArray = { "選擇加工類型", "垂直方孔", "垂直外形", "垂直圓孔", "斜銷孔", "錐度孔", "錐度外形", "開放式外形", "開放式錐度外形" };
        for (int i = 0; i < orderArray.Length; i++)
        {
            Type TEMPTYPE = new Type();
            TEMPTYPE.key = orderArray[i];
            TEMPTYPE.type = i;
            Types.Add(TEMPTYPE);
        }


    }
}
#endregion
public class Program
{
    // class members
    private static Session theSession;
    private static UI theUI;
    private static UFSession theUfSession;
    public static Program theProgram;
    public static bool isDisposeCalled;
    public static string ATTR_CIM_WEDM_CSYS = "WEDM_CSYS";
    public static SelectWorkPart cSelectWorkPartFrom;

    public static string IsCountersunk = "SHCS_Hole";

    //------------------------------------------------------------------------------
    // Constructor
    //------------------------------------------------------------------------------
    public Program()
    {
        try
        {
            theSession = Session.GetSession();
            theUI = UI.GetUI();
            theUfSession = UFSession.GetUFSession();
            isDisposeCalled = false;
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----
            // UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ex.Message);
        }
    }

    public static List<Component> sComponent = new List<Component>();
    public static List<Part> sPart = new List<Part>();
    public static List<Component> ssComponent = new List<Component>();
    public static List<Part> ssPart = new List<Part>();
    public static Dictionary<skey, string> ssDictionary = new Dictionary<skey, string>();
    public static weExportData sWEData = new weExportData();
    public static List<Component> sssComponent = new List<Component>();
    public static List<Part> sssPart = new List<Part>();
    public static Dictionary<skeyFailed, string> FailedSection = new Dictionary<skeyFailed, string>();
    //public static Dictionary<skey, string> WEFaceDict = new Dictionary<skey, string>();

    //     public struct WeGroupFace
    //     {
    //         public string faceGroup;    //面1,面2,面3....
    //     }



    public struct Key
    {
        public string Y;
        public string Z;
    }

    public struct RefCornerFace
    {
        public Face faceA;
        public Face faceB;
        public Face faceC;
        public Face faceD;
    }

    public static string MFG_COLOR = "MFG_COLOR";
    public static string COLOR_ID = "COLOR_ID";
    public static string CIM_SECTION = "CIM_SECTION";
    public static string MACHING_TYPE = "MACHING_TYPE";
    public static string CIM_WORK_FACE = "CIM_WORK_FACE";
    public static string CIM_EDM_FACE = "CIM_EDM_FACE";
    public static string CIM_EDM_WEDM = "CIM_EDM_WEDM";
    public static string FEATURE_TYPE = "FEATURE_TYPE";


    //------------------------------------------------------------------------------
    //  Explicit Activation
    //      This entry point is used to activate the application explicitly
    //------------------------------------------------------------------------------
    public static int Main(string[] args)
    {
        int retValue = 0;
        try
        {
            //CaxLoadingDlg sCaxLoadingDlg = new CaxLoadingDlg();
            //sCaxLoadingDlg.Run();
            //sCaxLoadingDlg.SetLoadingText("數據計算中...");
            theProgram = new Program();

            /*
            List<CaxAsm.CompPart> AsmCompAry = new List<CaxAsm.CompPart>();
            CaxAsm.GetAsmCompTree(out AsmCompAry);

            Component comp = null;
            Component comp_WE = null;
            Part compPart = null;
            for (int i = 0; i < AsmCompAry.Count; i++)
            {
                if (AsmCompAry[i].componentOcc.Name == "X903J-0011T_U100_WE_WEDMS1_T")
                {
                    comp = AsmCompAry[i].componentOcc;
                }
                if (AsmCompAry[i].componentOcc.Name == "X903J-0011T_U100_WE")
                {
                    comp_WE = AsmCompAry[i].componentOcc;
                    compPart = AsmCompAry[i].partPrototype;
                }
            }

            CaxAsm.SetWorkComponent(comp);
            for (int i = 0; i < compPart.Bodies.ToArray().Length;i++ )
            {
                WAVEGeometry(comp_WE, compPart.Bodies.ToArray()[i]);
            }
            
            theProgram.Dispose();
            return retValue;
            */


            CaxWE cCaxWE = new CaxWE();
            bool status = cCaxWE.Execute();


            theProgram.Dispose();
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----

        }
        return retValue;
    }

    public static bool AskFaceCenter(Face sFace, out double[] center, out double[] dir)
    {
        center = new double[3];
        dir = new double[3];
        try
        {
            int type;
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

    public static void CreateNewPart(string PartFullPath, string OldPartName, string NewPartName, ref NXOpen.Assemblies.Component NewComp, out Body NewWEBody)
    {
        string NewFileName = Path.GetDirectoryName(PartFullPath) + @"\" + OldPartName + "_" + NewPartName + ".prt";
        File.Copy(PartFullPath, NewFileName, false);
        CaxAsm.CreateNewEmptyComp(NewFileName, out NewComp);
        CaxAsm.AddComponentToAsmByDefault(NewFileName, out NewComp);
        CaxAsm.SetWorkComponent(NewComp);
        NewComp.Blank();
        CaxPart.GetLayerBody(NewComp, out NewWEBody);
    }

    public static bool DecideOuterInner(NXOpen.Assemblies.Component comp, WorkPiece WP, out string outer_inner, out string reference_posi)
    {
        outer_inner = null;
        reference_posi = null;
        try
        {
            string workname = comp.Name;//TEST
            //SetDisplayPart(workname);
            RefCornerFace sRefCornerFace;
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
                    RotateObjectByZ(body, Rotate_Angle);
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
                    RotateObjectByZ(body, Rotate_Angle);
                    reference_posi = "1";
                    //sWEData.REFERENCE_POSITION = "1";
                }
                else if (((cornerFaceA_dir[0] == XPositive[0] && cornerFaceA_dir[1] == XPositive[1] && cornerFaceA_dir[2] == XPositive[2]) &&
                         (cornerFaceB_dir[0] == YNegative[0] && cornerFaceB_dir[1] == YNegative[1] && cornerFaceB_dir[2] == YNegative[2])) ||
                         ((cornerFaceA_dir[0] == YNegative[0] && cornerFaceA_dir[1] == YNegative[1] && cornerFaceA_dir[2] == YNegative[2]) &&
                         (cornerFaceB_dir[0] == XPositive[0] && cornerFaceB_dir[1] == XPositive[1] && cornerFaceB_dir[2] == XPositive[2])))
                {
                    double Rotate_Angle = -180;
                    RotateObjectByZ(body, Rotate_Angle);
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
                    RotateObjectByZ(body, Rotate_Angle);
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
                    RotateObjectByZ(body, Rotate_Angle);
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

    //------------------------------------------------------------------------------
    // Following method disposes all the class members
    //------------------------------------------------------------------------------
    public void Dispose()
    {
        try
        {
            if (isDisposeCalled == false)
            {
                //TODO: Add your application code here 
            }
            isDisposeCalled = true;
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----

        }
    }

    public static int GetUnloadOption(string arg)
    {
        //Unloads the image explicitly, via an unload dialog
        //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);

        //Unloads the image immediately after execution within NX
        return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);

        //Unloads the image when the NX session terminates
        // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
    }

    public static bool CreateNewWEFile(string FilePath, string FileName, out NXObject nXObject2)
    {
        nXObject2 = null;
        try
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Assemblies->Components->Create New Component...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Create New Component");

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            FileNew fileNew1;
            fileNew1 = theSession.Parts.FileNew();

            theSession.SetUndoMarkName(markId2, "New Component File Dialog");

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "New Component File");

            theSession.DeleteUndoMark(markId3, null);

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "New Component File");

            fileNew1.TemplateFileName = "model-plain-1-mm-template.prt";

            fileNew1.Application = FileNewApplication.Modeling;

            fileNew1.Units = NXOpen.Part.Units.Millimeters;

            //fileNew1.RelationType = "";

            //fileNew1.UsesMasterModel = "No";

            fileNew1.TemplateType = FileNewTemplateType.Item;

            //fileNew1.NewFileName = "C:\\Users\\yukai\\Desktop\\model3.prt";
            fileNew1.NewFileName = FilePath + FileName + ".prt";

            fileNew1.MasterFileName = "";

            fileNew1.UseBlankTemplate = false;

            fileNew1.MakeDisplayedPart = false;

            theSession.DeleteUndoMark(markId4, null);

            theSession.SetUndoMarkName(markId2, "New Component File");

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Assemblies.CreateNewComponentBuilder createNewComponentBuilder1;
            createNewComponentBuilder1 = workPart.AssemblyManager.CreateNewComponentBuilder();

            createNewComponentBuilder1.NewComponentName = "MODEL1";

            createNewComponentBuilder1.ReferenceSet = NXOpen.Assemblies.CreateNewComponentBuilder.ComponentReferenceSetType.EntirePartOnly;

            createNewComponentBuilder1.ReferenceSetName = "Entire Part";

            theSession.SetUndoMarkName(markId5, "Create New Component Dialog");

            //createNewComponentBuilder1.NewComponentName = "MODEL3";
            createNewComponentBuilder1.NewComponentName = FileName;

            // ----------------------------------------------
            //   Dialog Begin Create New Component
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId6;
            markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Create New Component");

            theSession.DeleteUndoMark(markId6, null);

            NXOpen.Session.UndoMarkId markId7;
            markId7 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Create New Component");

            createNewComponentBuilder1.NewFile = fileNew1;

            NXOpen.Session.UndoMarkId markId8;
            markId8 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Create New component");

            NXObject nXObject1;
            nXObject1 = createNewComponentBuilder1.Commit();

            theSession.DeleteUndoMark(markId7, null);

            theSession.SetUndoMarkName(markId5, "Create New Component");

            createNewComponentBuilder1.Destroy();

            theSession.DeleteUndoMark(markId8, null);

            theSession.DeleteUndoMarksUpToMark(markId2, null, false);

            NXOpen.Session.UndoMarkId markId9;
            markId9 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Make Work Part");

            NXOpen.Assemblies.Component component1 = (NXOpen.Assemblies.Component)nXObject1;
            PartLoadStatus partLoadStatus1;
            theSession.Parts.SetWorkComponent(component1, out partLoadStatus1);

            workPart = theSession.Parts.Work;
            partLoadStatus1.Dispose();
            theSession.SetUndoMarkName(markId9, "Make Work Part");

            // ----------------------------------------------
            //   Menu: Insert->Associative Copy->WAVE Geometry Linker...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId10;
            markId10 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Features.Feature nullFeatures_Feature = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.WaveLinkBuilder waveLinkBuilder1;
            waveLinkBuilder1 = workPart.BaseFeatures.CreateWaveLinkBuilder(nullFeatures_Feature);

            NXOpen.Features.WaveDatumBuilder waveDatumBuilder1;
            waveDatumBuilder1 = waveLinkBuilder1.WaveDatumBuilder;

            NXOpen.Features.CompositeCurveBuilder compositeCurveBuilder1;
            compositeCurveBuilder1 = waveLinkBuilder1.CompositeCurveBuilder;

            NXOpen.Features.WaveSketchBuilder waveSketchBuilder1;
            waveSketchBuilder1 = waveLinkBuilder1.WaveSketchBuilder;

            NXOpen.Features.WaveRoutingBuilder waveRoutingBuilder1;
            waveRoutingBuilder1 = waveLinkBuilder1.WaveRoutingBuilder;

            NXOpen.Features.WavePointBuilder wavePointBuilder1;
            wavePointBuilder1 = waveLinkBuilder1.WavePointBuilder;

            NXOpen.Features.ExtractFaceBuilder extractFaceBuilder1;
            extractFaceBuilder1 = waveLinkBuilder1.ExtractFaceBuilder;

            NXOpen.Features.MirrorBodyBuilder mirrorBodyBuilder1;
            mirrorBodyBuilder1 = waveLinkBuilder1.MirrorBodyBuilder;

            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

            waveLinkBuilder1.Type = NXOpen.Features.WaveLinkBuilder.Types.BodyLink;

            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

            extractFaceBuilder1.AngleTolerance = 45.0;

            waveDatumBuilder1.DisplayScale = 2.0;

            extractFaceBuilder1.ParentPart = NXOpen.Features.ExtractFaceBuilder.ParentPartType.OtherPart;

            mirrorBodyBuilder1.ParentPartType = NXOpen.Features.MirrorBodyBuilder.ParentPart.OtherPart;

            theSession.SetUndoMarkName(markId10, "WAVE Geometry Linker Dialog");

            compositeCurveBuilder1.Section.DistanceTolerance = 0.002;

            compositeCurveBuilder1.Section.ChainingTolerance = 0.0019;

            extractFaceBuilder1.Associative = true;

            extractFaceBuilder1.MakePositionIndependent = false;

            extractFaceBuilder1.FixAtCurrentTimestamp = false;

            extractFaceBuilder1.HideOriginal = false;

            extractFaceBuilder1.InheritDisplayProperties = false;

            SelectObjectList selectObjectList1;
            selectObjectList1 = extractFaceBuilder1.BodyToExtract;

            extractFaceBuilder1.CopyThreads = true;

            NXOpen.Assemblies.Component component2 = (NXOpen.Assemblies.Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT N004J-0008T_core_062_A0 1");
            Body body1 = (Body)component2.FindObject("PARTIAL_PROTO#.Bodies|Body66");
            bool added1;
            added1 = selectObjectList1.Add(body1);

            Part part1 = (Part)theSession.Parts.FindObject("N004J-0008T_core_062_A0");
            PartLoadStatus partLoadStatus2;
            partLoadStatus2 = part1.LoadFully();

            partLoadStatus2.Dispose();
            NXOpen.Session.UndoMarkId markId11;
            markId11 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

            theSession.DeleteUndoMark(markId11, null);

            NXOpen.Session.UndoMarkId markId12;
            markId12 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

            //NXObject nXObject2;
            nXObject2 = waveLinkBuilder1.Commit();

            theSession.DeleteUndoMark(markId12, null);

            theSession.SetUndoMarkName(markId10, "WAVE Geometry Linker");

            waveLinkBuilder1.Destroy();

            NXOpen.Session.UndoMarkId markId13;
            markId13 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Make Displayed Part");

            NXOpen.Assemblies.Component[] components1 = new NXOpen.Assemblies.Component[1];
            components1[0] = component1;
            ErrorList errorList1;
            errorList1 = component1.DisplayComponentsExact(components1);

            errorList1.Clear();

            PartLoadStatus partLoadStatus3;
            NXOpen.PartCollection.SdpsStatus status1;
            status1 = theSession.Parts.SetDisplay(workPart, true, true, out partLoadStatus3);

            workPart = theSession.Parts.Work;
            displayPart = theSession.Parts.Display;
            partLoadStatus3.Dispose();
            // ----------------------------------------------
            //   Menu: Edit->Object Display...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId14;
            markId14 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

            theSession.SetUndoMarkName(markId14, "Class Selection Dialog");

            NXOpen.Session.UndoMarkId markId15;
            markId15 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Class Selection");

            theSession.DeleteUndoMark(markId15, null);

            NXOpen.Session.UndoMarkId markId16;
            markId16 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Class Selection");

            theSession.DeleteUndoMark(markId16, null);

            theSession.SetUndoMarkName(markId14, "Class Selection");

            theSession.DeleteUndoMark(markId14, null);

            // ----------------------------------------------
            //   Dialog Begin Edit Object Display
            // ----------------------------------------------
            // ----------------------------------------------
            //   Dialog Begin Color
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId17;
            markId17 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Edit Object Display");

            DisplayModification displayModification1;
            displayModification1 = theSession.DisplayManager.NewDisplayModification();

            displayModification1.ApplyToAllFaces = true;

            displayModification1.ApplyToOwningParts = false;

            displayModification1.NewColor = 87;

            DisplayableObject[] objects1 = new DisplayableObject[1];
            Body body2 = (Body)workPart.Bodies.FindObject("LINKED_BODY(1)");
            objects1[0] = body2;
            displayModification1.Apply(objects1);

            displayModification1.Dispose();
            // ----------------------------------------------
            //   Menu: File->Save All
            // ----------------------------------------------
            //             PartSaveStatus partSaveStatus1;
            //             partSaveStatus1 = workPart.SaveAs("E:\\NewWEFile002");
            //             
            //             partSaveStatus1.Dispose();

            NXOpen.PDM.PartFromTemplateBuilder partFromTemplateBuilder1;
            try
            {
                // This operation is not supported in native mode
                partFromTemplateBuilder1 = theSession.Parts.PDMPartManager.NewPartFromTemplateBuilder();
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(3520030);
            }

            NXOpen.Session.UndoMarkId markId18;
            markId18 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            theSession.SetUndoMarkName(markId18, "Name Parts Dialog");

            NXOpen.Session.UndoMarkId markId19;
            markId19 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Name Parts");

            theSession.DeleteUndoMark(markId19, null);

            NXOpen.Session.UndoMarkId markId20;
            markId20 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Name Parts");

            workPart.AssignPermanentName(FilePath + FileName/*+".prt"*/);

            theSession.DeleteUndoMark(markId20, null);

            theSession.SetUndoMarkName(markId18, "Name Parts");

            //null.Dispose();
            PartSaveStatus partSaveStatus1;
            partSaveStatus1 = workPart.Save(NXOpen.BasePart.SaveComponents.True, NXOpen.BasePart.CloseAfterSave.False);

            partSaveStatus1.Dispose();
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

    public static bool WaveLinkBody(Body body, out NXOpen.Features.Feature wavelink_feat)
    {
        wavelink_feat = null;

        try
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Insert->Associative Copy->WAVE Geometry Linker...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Features.Feature nullFeatures_Feature = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.WaveLinkBuilder waveLinkBuilder1;
            waveLinkBuilder1 = workPart.BaseFeatures.CreateWaveLinkBuilder(nullFeatures_Feature);

            NXOpen.Features.WaveDatumBuilder waveDatumBuilder1;
            waveDatumBuilder1 = waveLinkBuilder1.WaveDatumBuilder;

            NXOpen.Features.CompositeCurveBuilder compositeCurveBuilder1;
            compositeCurveBuilder1 = waveLinkBuilder1.CompositeCurveBuilder;

            NXOpen.Features.WaveSketchBuilder waveSketchBuilder1;
            waveSketchBuilder1 = waveLinkBuilder1.WaveSketchBuilder;

            NXOpen.Features.WaveRoutingBuilder waveRoutingBuilder1;
            waveRoutingBuilder1 = waveLinkBuilder1.WaveRoutingBuilder;

            NXOpen.Features.WavePointBuilder wavePointBuilder1;
            wavePointBuilder1 = waveLinkBuilder1.WavePointBuilder;

            NXOpen.Features.ExtractFaceBuilder extractFaceBuilder1;
            extractFaceBuilder1 = waveLinkBuilder1.ExtractFaceBuilder;

            NXOpen.Features.MirrorBodyBuilder mirrorBodyBuilder1;
            mirrorBodyBuilder1 = waveLinkBuilder1.MirrorBodyBuilder;

            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

            waveLinkBuilder1.Type = NXOpen.Features.WaveLinkBuilder.Types.BodyLink;

            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

            //extractFaceBuilder1.AngleTolerance = 45.0;

            //waveDatumBuilder1.DisplayScale = 2.0;

            extractFaceBuilder1.ParentPart = NXOpen.Features.ExtractFaceBuilder.ParentPartType.OtherPart;

            mirrorBodyBuilder1.ParentPartType = NXOpen.Features.MirrorBodyBuilder.ParentPart.OtherPart;

            theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker Dialog");

            //compositeCurveBuilder1.Section.DistanceTolerance = 0.0254;

            //compositeCurveBuilder1.Section.ChainingTolerance = 0.02413;

            extractFaceBuilder1.Associative = true;

            extractFaceBuilder1.MakePositionIndependent = false;

            extractFaceBuilder1.FixAtCurrentTimestamp = false;

            extractFaceBuilder1.HideOriginal = false;

            extractFaceBuilder1.InheritDisplayProperties = false;

            SelectObjectList selectObjectList1;
            selectObjectList1 = extractFaceBuilder1.BodyToExtract;

            extractFaceBuilder1.CopyThreads = true;

            //NXOpen.Assemblies.Component component1 = (NXOpen.Assemblies.Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT CIMTEST006_U002_A0 1");
            Body body1 = body;
            bool added1;
            added1 = selectObjectList1.Add(body1);

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

            NXObject nXObject1;
            nXObject1 = waveLinkBuilder1.Commit();

            wavelink_feat = (NXOpen.Features.Feature)nXObject1;

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker");

            waveLinkBuilder1.Destroy();
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }

    public static bool NewWEFileColor(Body body, int color)
    {
        try
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Edit->Object Display...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

            theSession.SetUndoMarkName(markId1, "Class Selection Dialog");

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Class Selection");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Class Selection");

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Class Selection");

            theSession.DeleteUndoMark(markId1, null);

            // ----------------------------------------------
            //   Dialog Begin Edit Object Display
            // ----------------------------------------------
            // ----------------------------------------------
            //   Dialog Begin Color
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Edit Object Display");

            DisplayModification displayModification1;
            displayModification1 = theSession.DisplayManager.NewDisplayModification();

            displayModification1.ApplyToAllFaces = true;

            displayModification1.ApplyToOwningParts = false;

            displayModification1.NewColor = color;

            DisplayableObject[] objects1 = new DisplayableObject[1];
            //Body body1 = (Body)workPart.Bodies.FindObject("LINKED_BODY(1)");
            Body body1 = body;
            objects1[0] = body1;
            displayModification1.Apply(objects1);

            displayModification1.Dispose();
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

    public static bool SpinZAxisByAngle(NXOpen.Assemblies.Component rotatecomp, Point3d orgpt, double input_angle)
    {
        try
        {
            double angle = input_angle * Math.PI / 180;

            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Assemblies->Component Position->Move Component...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Positioning.ComponentPositioner componentPositioner1;
            componentPositioner1 = workPart.ComponentAssembly.Positioner;

            componentPositioner1.ClearNetwork();

            NXOpen.Assemblies.Arrangement arrangement1 = (NXOpen.Assemblies.Arrangement)workPart.ComponentAssembly.Arrangements.FindObject("Arrangement 1");
            componentPositioner1.PrimaryArrangement = arrangement1;

            componentPositioner1.BeginMoveComponent();

            NXOpen.Positioning.Network network1;
            network1 = componentPositioner1.EstablishNetwork();

            NXOpen.Positioning.ComponentNetwork componentNetwork1 = (NXOpen.Positioning.ComponentNetwork)network1;
            componentNetwork1.MoveObjectsState = true;

            NXOpen.Assemblies.Component nullAssemblies_Component = null;
            componentNetwork1.DisplayComponent = nullAssemblies_Component;

            componentNetwork1.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;

            componentNetwork1.RemoveAllConstraints();

            NXObject[] movableObjects1 = new NXObject[1];
            //         NXOpen.Assemblies.Component component1 = (NXOpen.Assemblies.Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT CIMTESTE_Insert1_A0 1");
            NXOpen.Assemblies.Component component1 = rotatecomp;
            movableObjects1[0] = component1;
            componentNetwork1.SetMovingGroup(movableObjects1);

            componentNetwork1.Solve();

            theSession.SetUndoMarkName(markId1, "Move Component Dialog");

            componentNetwork1.MoveObjectsState = true;

            //         Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            Point3d origin1 = orgpt;
            Vector3d vector1 = new Vector3d(0.0, 0.0, 1.0);
            Direction direction1;
            direction1 = workPart.Directions.CreateDirection(origin1, vector1, NXOpen.SmartObject.UpdateOption.AfterModeling);

            componentNetwork1.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Component Update");

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Angle");

            bool loaded1;
            loaded1 = componentNetwork1.IsReferencedGeometryLoaded();

            componentNetwork1.BeginDrag();

            Vector3d translation1 = new Vector3d(0.0, 0.0, 0.0);
            Matrix3x3 rotation1;

            rotation1.Xx = Math.Cos(angle);
            rotation1.Xy = -Math.Sin(angle);
            rotation1.Xz = 0.0;
            rotation1.Yx = Math.Sin(angle);
            rotation1.Yy = Math.Cos(angle);
            rotation1.Yz = 0.0;
            rotation1.Zx = 0.0;
            rotation1.Zy = 0.0;
            rotation1.Zz = 1.0;
            componentNetwork1.DragByTransform(translation1, rotation1);

            componentNetwork1.EndDrag();

            componentNetwork1.ResetDisplay();

            componentNetwork1.ApplyToModel();

            componentNetwork1.Solve();

            componentPositioner1.ClearNetwork();

            int nErrs1;
            nErrs1 = theSession.UpdateManager.AddToDeleteList(componentNetwork1);

            int nErrs2;
            nErrs2 = theSession.UpdateManager.DoUpdate(markId2);

            NXOpen.Positioning.Network network2;
            network2 = componentPositioner1.EstablishNetwork();

            NXOpen.Positioning.ComponentNetwork componentNetwork2 = (NXOpen.Positioning.ComponentNetwork)network2;
            componentNetwork2.MoveObjectsState = true;

            componentNetwork2.DisplayComponent = nullAssemblies_Component;

            componentNetwork2.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;

            componentNetwork2.RemoveAllConstraints();

            NXObject[] movableObjects2 = new NXObject[1];
            movableObjects2[0] = component1;
            componentNetwork2.SetMovingGroup(movableObjects2);

            componentNetwork2.Solve();

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Component");

            theSession.DeleteUndoMark(markId4, null);

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Component");

            componentNetwork2.Solve();

            componentPositioner1.ClearNetwork();

            int nErrs3;
            nErrs3 = theSession.UpdateManager.AddToDeleteList(componentNetwork2);

            int nErrs4;
            nErrs4 = theSession.UpdateManager.DoUpdate(markId2);

            componentPositioner1.DeleteNonPersistentConstraints();

            int nErrs5;
            nErrs5 = theSession.UpdateManager.DoUpdate(markId2);

            theSession.DeleteUndoMark(markId5, null);

            theSession.SetUndoMarkName(markId1, "Move Component");

            componentPositioner1.EndMoveComponent();

            NXOpen.Assemblies.Arrangement nullAssemblies_Arrangement = null;
            componentPositioner1.PrimaryArrangement = nullAssemblies_Arrangement;

            theSession.DeleteUndoMark(markId2, null);

            theSession.DeleteUndoMark(markId3, null);
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

    public static void InsertColor(string InsertColorNum, out string COLOR_ID)
    {
        COLOR_ID = null;

        if (InsertColorNum == "11")
        {
            COLOR_ID = "71";
        }
        else if (InsertColorNum == "12")
        {
            COLOR_ID = "101";
        }
        else if (InsertColorNum == "13")
        {
            COLOR_ID = "144";
        }
        else if (InsertColorNum == "14")
        {
            COLOR_ID = "143";
        }
        else if (InsertColorNum == "21")
        {
            COLOR_ID = "39";
        }
        else if (InsertColorNum == "22")
        {
            COLOR_ID = "41";
        }
        else if (InsertColorNum == "23")
        {
            COLOR_ID = "77";
        }
        else if (InsertColorNum == "24")
        {
            COLOR_ID = "114";
        }
        else if (InsertColorNum == "31")
        {
            COLOR_ID = "176";
        }
        else if (InsertColorNum == "32")
        {
            COLOR_ID = "134";
        }
        else if (InsertColorNum == "33")
        {
            COLOR_ID = "206";
        }
        else if (InsertColorNum == "34")
        {
            COLOR_ID = "214";
        }
        else if (InsertColorNum == "41")
        {
            COLOR_ID = "149";
        }
        else if (InsertColorNum == "42")
        {
            COLOR_ID = "155";
        }
        else if (InsertColorNum == "43")
        {
            COLOR_ID = "198";
        }
        else if (InsertColorNum == "44")
        {
            COLOR_ID = "197";
        }
        else if (InsertColorNum == "51")
        {
            COLOR_ID = "187";
        }
        else if (InsertColorNum == "52")
        {
            COLOR_ID = "195";
        }
        else if (InsertColorNum == "53")
        {
            COLOR_ID = "196";
        }
        else if (InsertColorNum == "54")
        {
            COLOR_ID = "174";
        }
        else if (InsertColorNum == "61")
        {
            COLOR_ID = "39";
        }
        else if (InsertColorNum == "62")
        {
            COLOR_ID = "41";
        }
        else if (InsertColorNum == "63")
        {
            COLOR_ID = "77";
        }
        else if (InsertColorNum == "64")
        {
            COLOR_ID = "114";
        }
        else if (InsertColorNum == "71")
        {
            COLOR_ID = "44";
        }
        else if (InsertColorNum == "72")
        {
            COLOR_ID = "36";
        }
        else if (InsertColorNum == "73")
        {
            COLOR_ID = "130";
        }
        else if (InsertColorNum == "74")
        {
            COLOR_ID = "201";
        }

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
        else if (machingAng >= 10)
        {
            machingAng_range = "12";
        }
        else if (machingAng <= 0.01)
        {
            machingAng_range = "0";
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

    public static bool RotateObjectByZ(Body sbody, double Rotate_Angle)
    {
        try
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

            Point nullPoint = null;
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

            Point point1;
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

            Point point2;
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

            Point point3;
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

            Point point4;
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

            Point point5;
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

            Point point6;
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
        catch (System.Exception ex)
        {
            return false;
        }
        return true;


    }

    public static bool RotateObjectByX(Body sbody, double Rotate_Angle)
    {
        try
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
            Vector3d vector1 = new Vector3d(1.0, 0.0, 0.0);
            Direction direction1;
            direction1 = workPart.Directions.CreateDirection(origin1, vector1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Point nullPoint = null;
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

            Point point1;
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

            Point point2;
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

            Point point3;
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

            Point point4;
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

            Point point5;
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

            Point point6;
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
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.ToString());
            return false;
        }
        return true;
    }

    public static bool RotateObjectByY(Body sbody, double Rotate_Angle)
    {
        try
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
            Vector3d vector1 = new Vector3d(0.0, 1.0, 0.0);
            Direction direction1;
            direction1 = workPart.Directions.CreateDirection(origin1, vector1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Point nullPoint = null;
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

            Point point1;
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

            Point point2;
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

            Point point3;
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

            Point point4;
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

            Point point5;
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

            Point point6;
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
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.ToString());
            return false;
        }
        return true;
    }

    public static bool MoveDistByZaxis(Body movebody, double movedis)
    {
        try
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

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = "40";

            moveObjectBuilder1.TransformMotion.DistanceBetweenPointsDistance.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.RadialDistance.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.Angle.RightHandSide = "180";

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
            Body body1 = movebody;
            bool added1;
            added1 = moveObjectBuilder1.ObjectToMoveObject.Add(body1);

            Unit unit1;
            unit1 = moveObjectBuilder1.TransformMotion.RadialOriginDistance.Units;

            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            Vector3d vector1 = new Vector3d(0.0, 0.0, 1.0);
            Direction direction1;
            direction1 = workPart.Directions.CreateDirection(origin1, vector1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            moveObjectBuilder1.TransformMotion.DistanceVector = direction1;

            Point3d origin2 = new Point3d(0.0, 0.0, 0.0);
            direction1.Origin = origin2;

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = movedis.ToString();

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = movedis.ToString();

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            NXObject nXObject1;
            nXObject1 = moveObjectBuilder1.Commit();

            NXObject[] objects1;
            objects1 = moveObjectBuilder1.GetCommittedObjects();

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Move Object");

            moveObjectBuilder1.Destroy();

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

    public static bool MoveDisByXaxis(Body movebody, double movedis)
    {
        try
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

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = "40";

            moveObjectBuilder1.TransformMotion.DistanceBetweenPointsDistance.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.RadialDistance.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.Angle.RightHandSide = "180";

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
            Body body1 = movebody;
            bool added1;
            added1 = moveObjectBuilder1.ObjectToMoveObject.Add(body1);

            Unit unit1;
            unit1 = moveObjectBuilder1.TransformMotion.RadialOriginDistance.Units;

            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            Vector3d vector1 = new Vector3d(1.0, 0.0, 0.0);
            Direction direction1;
            direction1 = workPart.Directions.CreateDirection(origin1, vector1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            moveObjectBuilder1.TransformMotion.DistanceVector = direction1;

            Point3d origin2 = new Point3d(0.0, 0.0, 0.0);
            direction1.Origin = origin2;

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = movedis.ToString();

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = movedis.ToString();

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            NXObject nXObject1;
            nXObject1 = moveObjectBuilder1.Commit();

            NXObject[] objects1;
            objects1 = moveObjectBuilder1.GetCommittedObjects();

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Move Object");

            moveObjectBuilder1.Destroy();

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

    public static bool MoveDisByYaxis(Body movebody, double movedis)
    {
        try
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

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = "40";

            moveObjectBuilder1.TransformMotion.DistanceBetweenPointsDistance.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.RadialDistance.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.Angle.RightHandSide = "180";

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
            Body body1 = movebody;
            bool added1;
            added1 = moveObjectBuilder1.ObjectToMoveObject.Add(body1);

            Unit unit1;
            unit1 = moveObjectBuilder1.TransformMotion.RadialOriginDistance.Units;

            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            Vector3d vector1 = new Vector3d(0.0, 1.0, 0.0);
            Direction direction1;
            direction1 = workPart.Directions.CreateDirection(origin1, vector1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            moveObjectBuilder1.TransformMotion.DistanceVector = direction1;

            Point3d origin2 = new Point3d(0.0, 0.0, 0.0);
            direction1.Origin = origin2;

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = movedis.ToString();

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = movedis.ToString();

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            NXObject nXObject1;
            nXObject1 = moveObjectBuilder1.Commit();

            NXObject[] objects1;
            objects1 = moveObjectBuilder1.GetCommittedObjects();

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Move Object");

            moveObjectBuilder1.Destroy();

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

    public static bool SetDisplayPart(string partname)
    {
        try
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Make Displayed Part");

            //             NXOpen.Assemblies.Component component1 = (NXOpen.Assemblies.Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT N004J-0008T_core_062_A0 1");
            //             NXOpen.Assemblies.Component[] components1 = new NXOpen.Assemblies.Component[1];
            //             components1[0] = component1;
            //             ErrorList errorList1;
            //             errorList1 = component1.DisplayComponentsExact(components1);
            // 
            //             errorList1.Clear();

            Part part1 = (Part)theSession.Parts.FindObject(partname);
            PartLoadStatus partLoadStatus1;
            NXOpen.PartCollection.SdpsStatus status1;
            status1 = theSession.Parts.SetDisplay(part1, true, true, out partLoadStatus1);

            workPart = theSession.Parts.Work;
            displayPart = theSession.Parts.Display;
            partLoadStatus1.Dispose();
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

    public static bool RePositionWEDMPart(Component newComp, Part newDisplayPart)
    {
        NXOpen.Features.Feature[] features = newDisplayPart.Features.ToArray();
        //CIM_ELECTRODE_BODY = MID_POINT
        string ATTR_CIM_ELECTRODE_BODY = "CIM_ELECTRODE_BODY";
        string ATTR_CIM_EDM_FACE = "CIM_EDM_FACE";
        string ATTR_MID_POINT = "MID_POINT";

        //紀錄原始工作組件
        NXOpen.Session.UndoMarkId markId1;
        markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "RePositionWEDMPartSTART");

        //取midpoint / Zface
        NXOpen.Features.Feature feat_mid_point = null;
        Face Zface = null;
        NXOpen.Point centerpoint = null;

        for (int i = 0; i < features.Length; i++)
        {
            string str = "";
            try
            {
                str = features[i].GetStringAttribute(ATTR_CIM_ELECTRODE_BODY);
                if (str == ATTR_MID_POINT)
                {
                    feat_mid_point = features[i];

                    Tag point_t = NXOpen.Tag.Null;
                    theUfSession.Point.AskPointOutput(feat_mid_point.Tag, out point_t);
                    if (point_t != NXOpen.Tag.Null)
                    {
                        centerpoint = (NXOpen.Point)NXObjectManager.Get(point_t);
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        Body[] bodys = newDisplayPart.Bodies.ToArray();
        for (int i = 0; i < bodys.Length; i++)
        {
            Face[] faces = bodys[i].GetFaces();
            for (int j = 0; j < faces.Length; j++)
            {
                string str = "";
                try
                {
                    str = faces[j].GetStringAttribute(ATTR_CIM_EDM_FACE);
                    if (str == "Z")
                    {
                        Zface = faces[j];
                    }
                }
                catch (System.Exception ex)
                {

                }
            }
        }

        if (Zface == null || centerpoint == null)
        {
            return false;
        }

        bool status = J_PartSpinbyFacetoZ(Zface.GetBody(), Zface, centerpoint);
        if (!status)
        {
            return false;
        }

        //         double x = 10;
        //         double y = 10;
        //         double z = 10;
        //         status = MoveInstance(newComp, x,y,z);
        //         if (!status)
        //         {
        //             return false;
        //         }

        NXOpen.Features.Feature outfeat = null;
        //         status = J_MidPointCsys(centerpoint, out outfeat);
        //         if (!status)
        //         {
        //             return false;
        //         }

        //線割坐標系寫屬性
        if (outfeat != null)
        {
            outfeat.SetAttribute(ATTR_CIM_ELECTRODE_BODY, Program.ATTR_CIM_WEDM_CSYS);
        }
        return true;
    }

    public static bool J_MidPointCsys(NXOpen.Point point, out NXOpen.Features.Feature outfeat)
    {
        Session theSession = Session.GetSession();
        Part workPart = theSession.Parts.Work;
        Part displayPart = theSession.Parts.Display;
        outfeat = null;
        // ----------------------------------------------
        //   Menu: Insert->Datum/Point->Datum CSYS...
        // ----------------------------------------------
        NXOpen.Session.UndoMarkId markId1;
        markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

        NXOpen.Features.Feature nullFeatures_Feature = null;

        try
        {
            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.DatumCsysBuilder datumCsysBuilder1;
            datumCsysBuilder1 = workPart.Features.CreateDatumCsysBuilder(nullFeatures_Feature);

            Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Unit unit2 = (Unit)workPart.UnitCollection.FindObject("Degrees");
            Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

            Expression expression3;
            expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression4;
            expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

            Expression expression5;
            expression5 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression6;
            expression6 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

            expression1.RightHandSide = "0";

            expression3.RightHandSide = "0";

            expression5.RightHandSide = "0";

            expression2.RightHandSide = "0";

            expression4.RightHandSide = "0";

            expression6.RightHandSide = "0";

            theSession.SetUndoMarkName(markId1, "Datum CSYS Dialog");

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Datum CSYS");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Datum CSYS");

            //                 Point3d origin1 = new Point3d(-57.9123594676726, 17.5644890813366, 10.2499120462857);
            Point3d origin1 = point.Coordinates;
            Vector3d xDirection1 = new Vector3d(1.0, 0.0, 0.0);
            Vector3d yDirection1 = new Vector3d(0.0, 1.0, 0.0);
            Xform xform1;
            xform1 = workPart.Xforms.CreateXform(origin1, xDirection1, yDirection1, NXOpen.SmartObject.UpdateOption.WithinModeling, 1.0);

            //                 Point3d origin2 = new Point3d(-57.9123594676726, 17.5644890813366, 10.2499120462857);
            xform1.SetOrigin(origin1);

            Matrix3x3 orientation1;
            orientation1.Xx = 1.0;
            orientation1.Xy = 0.0;
            orientation1.Xz = 0.0;
            orientation1.Yx = 0.0;
            orientation1.Yy = 1.0;
            orientation1.Yz = 0.0;
            orientation1.Zx = 0.0;
            orientation1.Zy = 0.0;
            orientation1.Zz = 1.0;
            xform1.SetOrientation(orientation1);

            CartesianCoordinateSystem cartesianCoordinateSystem1;
            cartesianCoordinateSystem1 = workPart.CoordinateSystems.CreateCoordinateSystem(xform1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Datum CSYS");

            workPart.Expressions.Delete(expression1);

            workPart.Expressions.Delete(expression3);

            workPart.Expressions.Delete(expression5);

            workPart.Expressions.Delete(expression2);

            workPart.Expressions.Delete(expression4);

            workPart.Expressions.Delete(expression6);

            datumCsysBuilder1.Csys = cartesianCoordinateSystem1;

            datumCsysBuilder1.DisplayScaleFactor = 1.25;

            Feature[] feats_bef = workPart.Features.ToArray();
            NXObject nXObject1;
            nXObject1 = datumCsysBuilder1.Commit();

            outfeat = (NXOpen.Features.Feature)nXObject1;
            if (outfeat == null)
            {
                Feature[] feats_aft = workPart.Features.ToArray();

                for (int i = 0; i < feats_aft.Length; i++)
                {
                    bool featexist = false;
                    for (int j = 0; j < feats_bef.Length; j++)
                    {
                        if (feats_aft[i] == feats_bef[j])
                        {
                            featexist = true;
                        }
                    }
                    if (!featexist)
                    {
                        outfeat = feats_aft[i];
                    }
                }
            }

            datumCsysBuilder1.Destroy();
        }
        catch (System.Exception ex)
        {
            CaxLog.WriteLog("J_MidPointCsys catch!");
            return false;
        }

        // ----------------------------------------------
        //   Menu: Tools->Journal->Stop Recording
        // ----------------------------------------------
        return true;
    }

    public static bool J_PartSpinbyFacetoZ(Body body, Face faceZ, NXOpen.Point point)
    {
        Session theSession = Session.GetSession();
        Part workPart = theSession.Parts.Work;
        Part displayPart = theSession.Parts.Display;
        // ----------------------------------------------
        //   Menu: Edit->Move Object...
        // ----------------------------------------------
        NXOpen.Session.UndoMarkId markId1;
        markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

        try
        {
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

            moveObjectBuilder1.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.Angle;

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.DistanceBetweenPointsDistance.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.RadialDistance.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.Angle.RightHandSide = "90";

            moveObjectBuilder1.TransformMotion.DistanceAngle.Distance.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.DistanceAngle.Angle.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.DeltaEnum = NXOpen.GeometricUtilities.ModlMotion.Delta.ReferenceWcsWorkPart;

            moveObjectBuilder1.TransformMotion.DeltaXc.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.DeltaYc.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.DeltaZc.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.AlongCurveAngle.AlongCurve.Expression.RightHandSide = "0";

            moveObjectBuilder1.TransformMotion.AlongCurveAngle.AlongCurveAngle.RightHandSide = "0";

            theSession.SetUndoMarkName(markId1, "Move Object Dialog");

            Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            Vector3d vector1 = new Vector3d(0.0, 0.0, 1.0);
            Direction direction1;
            direction1 = workPart.Directions.CreateDirection(origin1, vector1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            NXOpen.Point nullPoint = null;
            Axis axis1;
            axis1 = workPart.Axes.CreateAxis(nullPoint, direction1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            moveObjectBuilder1.TransformMotion.AngularAxis = axis1;

            moveObjectBuilder1.TransformMotion.AngularAxis = axis1;

            //             Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(4)");
            Body body1 = body;
            bool added1;
            added1 = moveObjectBuilder1.ObjectToMoveObject.Add(body1);

            moveObjectBuilder1.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.AlignAxisVector;

            Unit unit1;
            unit1 = moveObjectBuilder1.TransformMotion.RadialOriginDistance.Units;

            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            //             NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject("UNPARAMETERIZED_FEATURE(4)");
            //             Face face1 = (Face)brep1.FindObject("FACE 843 {(-53.4123594676726,18.5644890813366,12.2499120462857) UNPARAMETERIZED_FEATURE(4)}");
            Face face1 = faceZ;
            Direction direction2;
            direction2 = workPart.Directions.CreateDirection(face1, Sense.Forward, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Axis axis2;
            axis2 = workPart.Axes.CreateAxis(nullPoint, direction2, NXOpen.SmartObject.UpdateOption.WithinModeling);

            moveObjectBuilder1.TransformMotion.AlignVector = axis2;

            Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            //NXOpen.Features.PointFeature pointFeature1 = (NXOpen.Features.PointFeature)workPart.Features.FindObject("POINT(3)");
            //             Point point1 = (Point)pointFeature1.FindObject("POINT 1");
            NXOpen.Point point1 = point;
            Xform nullXform = null;
            NXOpen.Point point2;
            point2 = workPart.Points.CreatePoint(point1, nullXform, NXOpen.SmartObject.UpdateOption.WithinModeling);

            axis2.Point = point2;

            Expression expression3;
            expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Point3d origin2 = new Point3d(0.0, 0.0, 0.0);
            Vector3d vector2 = new Vector3d(0.0, 0.0, 1.0);
            Direction direction3;
            direction3 = workPart.Directions.CreateDirection(origin2, vector2, NXOpen.SmartObject.UpdateOption.WithinModeling);

            moveObjectBuilder1.TransformMotion.ToVector = direction3;

            moveObjectBuilder1.TransformMotion.AlignVector = axis2;

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            NXObject nXObject1;
            nXObject1 = moveObjectBuilder1.Commit();

            NXObject[] objects1;
            objects1 = moveObjectBuilder1.GetCommittedObjects();

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Move Object");

            moveObjectBuilder1.Destroy();

            workPart.Expressions.Delete(expression2);

            workPart.Expressions.Delete(expression3);

            workPart.Expressions.Delete(expression1);
        }
        catch (System.Exception ex)
        {
            //             MessageBox.Show(ex.ToString());
            //             CaxLog.WriteLog("J_PartSpinbyFacetoZ catch!");
            //             return false;
        }

        // ----------------------------------------------
        //   Menu: Tools->Journal->Stop Recording
        // ----------------------------------------------

        return true;
    }

    public static bool GetBaseCornerFaceAryOnPart(NXOpen.Assemblies.Component component, out RefCornerFace sRefCornerFace)
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
            //CaxPart.ShowListingWindow(ex.Message);
            return false;
        }

        return true;
    }

    public static bool CreateNewComp(string NewCompFullPath, string NewCompName, out NXObject nXObject1)
    {
        nXObject1 = null;
        try
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Assemblies->Components->Create New Component...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Create New Component");

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            FileNew fileNew1;
            fileNew1 = theSession.Parts.FileNew();

            theSession.SetUndoMarkName(markId2, "New Component File Dialog");

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "New Component File");

            theSession.DeleteUndoMark(markId3, null);

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "New Component File");

            fileNew1.TemplateFileName = "assembly-mm-template.prt";

            fileNew1.Application = FileNewApplication.Assemblies;

            fileNew1.Units = NXOpen.Part.Units.Millimeters;

            //fileNew1.RelationType = "";

            //fileNew1.UsesMasterModel = "No";

            fileNew1.TemplateType = FileNewTemplateType.Item;

            fileNew1.NewFileName = NewCompFullPath;

            fileNew1.MasterFileName = "";

            fileNew1.UseBlankTemplate = false;

            fileNew1.MakeDisplayedPart = false;

            theSession.DeleteUndoMark(markId4, null);

            theSession.SetUndoMarkName(markId2, "New Component File");

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Assemblies.CreateNewComponentBuilder createNewComponentBuilder1;
            createNewComponentBuilder1 = workPart.AssemblyManager.CreateNewComponentBuilder();

            createNewComponentBuilder1.NewComponentName = "ASSEMBLY1";

            createNewComponentBuilder1.ReferenceSet = NXOpen.Assemblies.CreateNewComponentBuilder.ComponentReferenceSetType.EntirePartOnly;

            createNewComponentBuilder1.ReferenceSetName = "Entire Part";

            theSession.SetUndoMarkName(markId5, "Create New Component Dialog");

            createNewComponentBuilder1.NewComponentName = NewCompName;

            // ----------------------------------------------
            //   Dialog Begin Create New Component
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId6;
            markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Create New Component");

            theSession.DeleteUndoMark(markId6, null);

            NXOpen.Session.UndoMarkId markId7;
            markId7 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Create New Component");

            createNewComponentBuilder1.NewFile = fileNew1;

            NXOpen.Session.UndoMarkId markId8;
            markId8 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Create New component");

            //NXObject nXObject1;
            nXObject1 = createNewComponentBuilder1.Commit();

            theSession.DeleteUndoMark(markId7, null);

            theSession.SetUndoMarkName(markId5, "Create New Component");

            createNewComponentBuilder1.Destroy();

            theSession.DeleteUndoMark(markId8, null);

            theSession.DeleteUndoMarksUpToMark(markId2, null, false);

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

    public static bool MoveInstance(NXOpen.Assemblies.Component incomp, double Xdis, double Ydis, double Zdis)
    {
        NXOpen.Tag ins_t = NXOpen.Tag.Null;
        string part_name = "";
        string refset_name = "";
        string instance_name = "";
        double[] origin = new double[3];
        double[] csys_matrix = new double[9];
        double[,] transform = new double[4, 4];

        //**** MOVE ****
        try
        {
            theUfSession.Assem.AskComponentData(incomp.Tag, out part_name, out refset_name, out instance_name, origin, csys_matrix, transform);
        }
        catch (System.Exception ex)
        {

            return false;
        }
        //             origin[0] = origin[0] + Xdis;
        //             origin[1] = origin[1] + Ydis;
        //             origin[2] = origin[2] + Zdis;

        double[] comp_xvect = { csys_matrix[0], csys_matrix[1], csys_matrix[2] };
        double[] comp_yvect = { csys_matrix[3], csys_matrix[4], csys_matrix[5] };
        double[] csys_origin = { origin[0], origin[1], origin[2] };
        double[] wcs_mm = new double[16];
        double[] wcs_xvect = { csys_matrix[0], csys_matrix[1], csys_matrix[2] };
        double[] wcs_yvect = { csys_matrix[3], csys_matrix[4], csys_matrix[5] };

        double[] next_csys_origin = { origin[0] + Xdis, origin[1] + Ydis, origin[2] + Zdis };

        theUfSession.Mtx4.CsysToCsys(
                        csys_origin,
                        comp_xvect,
                        comp_yvect,
                        next_csys_origin,
                        wcs_xvect,
                        wcs_yvect,
                        wcs_mm
                        );

        double[,] xform = new double[4, 4];
        xform[0, 0] = wcs_mm[0];
        xform[0, 1] = wcs_mm[1];
        xform[0, 2] = wcs_mm[2];
        xform[0, 3] = wcs_mm[3];

        xform[1, 0] = wcs_mm[4];
        xform[1, 1] = wcs_mm[5];
        xform[1, 2] = wcs_mm[6];
        xform[1, 3] = wcs_mm[7];

        xform[2, 0] = wcs_mm[8];
        xform[2, 1] = wcs_mm[9];
        xform[2, 2] = wcs_mm[10];
        xform[2, 3] = wcs_mm[11];

        xform[3, 0] = wcs_mm[12];
        xform[3, 1] = wcs_mm[13];
        xform[3, 2] = wcs_mm[14];
        xform[3, 3] = wcs_mm[15];

        try
        {
            ins_t = theUfSession.Assem.AskInstOfPartOcc(incomp.Tag);
            if (ins_t == NXOpen.Tag.Null)
            {
                CaxLog.ShowListingWindow("null");
            }
        }
        catch (System.Exception ex)
        {
            return false;
        }
        try
        {
            theUfSession.Assem.RepositionPartOccurrence(incomp.Tag, xform, UFAssem.LevelOption.UseExistingLevel);
        }
        catch (System.Exception ex)
        {
            CaxLog.ShowListingWindow("RepositionPartOccurrence : " + ex.Message);
            return false;
        }
        return true;
    }

    public static bool ExportPartForWE()
    {
        try
        {
//             CaxLoadingDlg sCaxLoadingDlg = new CaxLoadingDlg();
//             sCaxLoadingDlg.Run();
//             sCaxLoadingDlg.SetLoadingText("數據計算中...");

            bool status;

            Part workPart = theSession.Parts.Work;

            //加入公差、表面粗糙、訊息三個EXCEL資訊
            Dictionary<int, string> mdColorDic = new Dictionary<int, string>();
            status = GetColorDataAry(out mdColorDic);
            if (!status)
            {
                return false;
            }
            
            //取得組件下的物件
            List<CaxAsm.CompPart> AsmCompAry;
            CaxAsm.CimAsmCompPart sCimAsmCompPart; 
            status = CaxAsm.GetAllAsmCompStruct(out AsmCompAry, out sCimAsmCompPart);
            if (!status)
            {
                return false;
            }
            Dictionary<string, CaxAsm.CompPart> AsmCompDic = new Dictionary<string, CaxAsm.CompPart>();
            for (int i = 0; i < AsmCompAry.Count; i++)
            {
                string attr_value = "";
                try
                {
                    attr_value = AsmCompAry[i].componentOcc.GetStringAttribute("REFERENCE_COMPONENT");

                    //有找到該屬性表示是跑位的空零件，忽略
                    continue;
                }
                catch (System.Exception ex)
                { }

                bool chk_key;
                chk_key = AsmCompDic.ContainsKey(AsmCompAry[i].componentOcc.Name.ToUpper());
                if (!chk_key)
                {
                    AsmCompDic.Add(AsmCompAry[i].componentOcc.Name.ToUpper(), AsmCompAry[i]);
                }
            }

            //對每一個comp計算線割工段存入Dictionary,Key=(comp,工段,加工面),Value=comp_tag
            Dictionary<skey, string> sectionFaceDic = new Dictionary<skey, string>();
            status = GetSectionFaceDic(sCimAsmCompPart, out sectionFaceDic);
            if (!status)
            {
                return false;
            }
            
            //建立線割檔
            Dictionary<WeListKey, WeFaceGroup> WEFaceDict = new Dictionary<WeListKey, WeFaceGroup>();

            status = CreateWePart(sectionFaceDic, sCimAsmCompPart, AsmCompDic, out WEFaceDict);
            if (!status)
            {
                return false;
            }
            
            //隱藏原始comp
            List<DisplayableObject> hideDispalyObject = new List<DisplayableObject>();
            hideDispalyObject.Add(sCimAsmCompPart.design.comp);
            hideDispalyObject.Add(sCimAsmCompPart.electorde_all.comp);
            hideDispalyObject.Add(sCimAsmCompPart.product.comp);
            foreach (KeyValuePair<skey, string> kvp in sectionFaceDic)
            {
                Tag hideOlderCompTag = (Tag)Convert.ToInt32(kvp.Value);
                Component hideOlderComp = (Component)NXObjectManager.Get(hideOlderCompTag);
                hideDispalyObject.Add(hideOlderComp);
            }
            theSession.DisplayManager.BlankObjects(hideDispalyObject.ToArray());

            //顯示新comp
            List<DisplayableObject> showDispalyObject = new List<DisplayableObject>();
            foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WEFaceDict)
            {
                showDispalyObject.Add(kvp.Value.comp);
            }
            theSession.DisplayManager.UnblankObjects(showDispalyObject.ToArray());
            CaxPart.Refresh();

            #region 自動辨識佈穿線點
            /*
            foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WEFaceDict)
            {
                List<List<Face>> allWeFaceGroup = new List<List<Face>>();
                for (int i = 0; i < kvp.Value.sFaceGroupPnt.Count; i++)
                {
                    List<Face> eachWeFaceGroup = new List<Face>();
                    for (int j = 0; j < kvp.Value.sFaceGroupPnt[i].faceOccAry.Count; j++)
                    {
                        eachWeFaceGroup.Add(kvp.Value.sFaceGroupPnt[i].faceOccAry[j]);
                    }
                    allWeFaceGroup.Add(eachWeFaceGroup);
                }
                PRCES100_GetHoleType cPRCES100_GetHoleType = new PRCES100_GetHoleType();
                int x = cPRCES100_GetHoleType.execute(allWeFaceGroup, ref WEFaceDict);
                if (x != 0)
                {
                    //return;
                }
            }
            */
            #endregion
           
            //sCaxLoadingDlg.Stop();
            
            Application.EnableVisualStyles();
            cSelectWorkPartFrom= new SelectWorkPart();
            cSelectWorkPartFrom.WEFaceDict = new Dictionary<WeListKey, WeFaceGroup>();
            cSelectWorkPartFrom.WEFaceDict = WEFaceDict;

            cSelectWorkPartFrom.mdColorDic = new Dictionary<int, string>();
            cSelectWorkPartFrom.mdColorDic = mdColorDic;

            //cSelectWorkPartFrom.weCompDic = new Dictionary<string, CaxAsm.AsmCompPart>();
            //cSelectWorkPartFrom.weCompDic = weCompDic;

            //             MelodyWrapper cMelodyWrapper = new MelodyWrapper();
            //             cMelodyWrapper.WEFaceDict = WEFaceDict;
            // 
            //             Thread threadFrom = new Thread(() => OpenForm(cMelodyWrapper));
            //             threadFrom.Start();
            //             threadFrom.Join();
            //cSelectWorkPartFrom.Show();

            NXOpenUI.FormUtilities.ReparentForm(cSelectWorkPartFrom);
            System.Windows.Forms.Application.Run(cSelectWorkPartFrom);
            cSelectWorkPartFrom.Dispose();

            foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WEFaceDict)
            {
                for (int i = 0; i < kvp.Value.sFaceGroupPnt.Count; i++)
                {
                    for (int j = 0; j < kvp.Value.sFaceGroupPnt[i].faceOccAry.Count; j++)
                    {
                        try
                        {
                            Face NewFaceOcc = kvp.Value.sFaceGroupPnt[i].faceOccAry[j];
                            Face NewFaceOcc_Prototype = (Face)NewFaceOcc.Prototype;
                            string getNewFaceColorAttr = kvp.Value.sFaceGroupPnt[i].faceOccAry[j].GetStringAttribute("COLOR_ID");
                            theUfSession.Obj.SetColor(NewFaceOcc_Prototype.Tag, Convert.ToInt32(getNewFaceColorAttr));
                        }
                        catch (System.Exception ex)
                        {

                        }
                    }
                }
            }
            CaxPart.Save();

            //顯示原始comp
            theSession.DisplayManager.UnblankObjects(hideDispalyObject.ToArray());
            //             for (int i=0;i<hideDispalyObject.Count;i++)
            //             {
            //                 Component oldcomp = (Component)hideDispalyObject[i];
            //                 CaxAsm.SetWorkComponent(oldcomp);
            //             }
            CaxAsm.SetWorkComponent(null);

            //隱藏新的comp
            theSession.DisplayManager.BlankObjects(showDispalyObject.ToArray());

            if (cSelectWorkPartFrom.DialogResult != DialogResult.OK)
            {
                return false;
            }


        }
        catch (System.Exception ex)
        {
            CaxLog.ShowListingWindow(ex.Message);
            return false;
        }

        return true;
    }

    public static void OpenForm(MelodyWrapper cMelodyWrapper)
    {
        Application.EnableVisualStyles();
        SelectWorkPart cSelectWorkPartFrom = new SelectWorkPart();
        cSelectWorkPartFrom.WEFaceDict = new Dictionary<WeListKey, WeFaceGroup>();
        cSelectWorkPartFrom.WEFaceDict = cMelodyWrapper.WEFaceDict;
        cSelectWorkPartFrom.ShowDialog();
    }

    private static bool CreateWePart(Dictionary<skey, string> sectionFaceDic, CaxAsm.CimAsmCompPart sCimAsmCompPart, Dictionary<string, CaxAsm.CompPart> AsmCompDic, out Dictionary<WeListKey, WeFaceGroup> WEFaceDict)
    {
        WEFaceDict = new Dictionary<WeListKey, WeFaceGroup>();

        try
        {
            bool status;
            Part workPart = theSession.Parts.Work;

            CaxAsm.SetWorkComponent(null);

            //CaxLog.ShowListingWindow("sectionFaceDic : " + sectionFaceDic.Count.ToString());

            #region 產生線割檔、平移、旋轉

            Body OlderBody = null;
            List<DisplayableObject> hideDispalyObject = new List<DisplayableObject>();
            foreach (KeyValuePair<skey, string> kvp in sectionFaceDic)
            {
                
                CaxPart.GetLayerBody(kvp.Key.comp, out OlderBody);

                Part weCompPart = (Part)kvp.Key.comp.Prototype;

                //產生線割檔名
                string WE_PRT_NAME = "";
                WE_PRT_NAME = string.Format(@"{0}_{1}", kvp.Key.section, kvp.Key.wkface);

                ////隱藏原始comp
                //List<DisplayableObject> hideDispalyObject = new List<DisplayableObject>();
                //hideDispalyObject.Add(sCimAsmCompPart.design.comp);
                //hideDispalyObject.Add(sCimAsmCompPart.electorde_all.comp);
                //hideDispalyObject.Add(sCimAsmCompPart.product.comp);
                //foreach (KeyValuePair<skey, string> kvp1 in sectionFaceDic)
                //{
                //    Tag hideOlderCompTag = (Tag)Convert.ToInt32(kvp1.Value);
                //    Component hideOlderComp = (Component)NXObjectManager.Get(hideOlderCompTag);
                //    hideDispalyObject.Add(hideOlderComp);
                //}
                //theSession.DisplayManager.BlankObjects(hideDispalyObject.ToArray());

                ////產生工件圖
                //workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Trimetric, NXOpen.View.ScaleAdjustment.Fit);
                //string ImagePath = "";
                //ImagePath = string.Format(@"{0}\{1}_{2}", Path.GetDirectoryName(weCompPart.FullPath), kvp.Key.comp.Name, WE_PRT_NAME);
                //theUfSession.Disp.CreateImage(ImagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);

                //新建線割檔
                Body NewWEBody = null;
                string NewCompFullPath = Path.GetDirectoryName(weCompPart.FullPath) + @"\" + kvp.Key.comp.Name + "_" + WE_PRT_NAME + ".prt";

                bool chk_exist = false;
                bool chk_key;
                Component newWeComp = null;
                CaxAsm.CompPart sAsmComp;
                chk_key = AsmCompDic.TryGetValue(Path.GetFileNameWithoutExtension(NewCompFullPath).ToUpper(), out sAsmComp);
                if (chk_key)
                {
                    //組立架構中 已存在 該線割零件
                    newWeComp = sAsmComp.componentOcc;
                    chk_exist = true;
                    //File.Delete(NewCompFullPath);
                    //CaxAsm.DeleteComponent(newWeComp);
                }
                else
                {
                    //組立架構中 不存在 該線割零件
                    CaxAsm.SetWorkComponent(null);
                    //CaxLog.ShowListingWindow(NewCompFullPath);

                    //                     if (System.IO.File.Exists(NewCompFullPath))
                    //                     {
                    //                         CaxLog.ShowListingWindow("Exists");
                    //                     }

                    // string strbuf = string.Format(@"{0}\{1}_1.prt", Path.GetDirectoryName(NewCompFullPath), Path.GetFileNameWithoutExtension(NewCompFullPath));
                    // NewCompFullPath = strbuf;
                    CaxPart.CloseSelectedParts(NewCompFullPath);

                    status = CaxAsm.CreateNewEmptyComp(NewCompFullPath, out newWeComp);
                    if (!status)
                    {
                        return false;
                    }
                    CaxAsm.SetWorkComponent(newWeComp);

                    NXOpen.Features.Feature wavelink_feat;
                    status = WaveLinkBody(OlderBody, out wavelink_feat);
                    if (!status)
                    {
                        return false;
                    }

                    CaxFeat.GetFeatBody(wavelink_feat, out NewWEBody);
                    RemoveParameters(NewWEBody);
                }

                //隱藏原始comp
                
                hideDispalyObject.Add(sCimAsmCompPart.design.comp);
                hideDispalyObject.Add(sCimAsmCompPart.electorde_all.comp);
                hideDispalyObject.Add(sCimAsmCompPart.product.comp);
                foreach (KeyValuePair<skey, string> kvp1 in sectionFaceDic)
                {
                    Tag hideOlderCompTag = (Tag)Convert.ToInt32(kvp1.Value);
                    Component hideOlderComp = (Component)NXObjectManager.Get(hideOlderCompTag);
                    hideDispalyObject.Add(hideOlderComp);
                }
                theSession.DisplayManager.BlankObjects(hideDispalyObject.ToArray());

                Thread.Sleep(0);

                ////產生工件圖
                //workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Trimetric, NXOpen.View.ScaleAdjustment.Fit);
                //string ImagePath = "";
                //ImagePath = string.Format(@"{0}\{1}_{2}", Path.GetDirectoryName(weCompPart.FullPath), kvp.Key.comp.Name, WE_PRT_NAME);
                //theUfSession.Disp.CreateImage(ImagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);

                ////將產生的線割檔加入隱藏列中，以便第二個檔案產生時可以隱藏
                //hideDispalyObject.Add(newWeComp);

                //return false;
                CaxPart.GetLayerBody(newWeComp, out NewWEBody);
                string SlopePin = "";
                try
                {
                    SlopePin = sCimAsmCompPart.design.comp.GetStringAttribute("DESCRIPTION");
                }
                catch (System.Exception ex)
                { }

                if (!chk_exist)
                {
                    if (sCimAsmCompPart.electorde.Count != 0)//小製程
                    {
                        //零件平移
                        string newWeCompName = newWeComp.Name;
                        Face[] NewWEBodyFaceAry = NewWEBody.GetFaces();

                        Face basef = null;
                        Face zf = null;
                        for (int i = 0; i < NewWEBodyFaceAry.Length; i++)
                        {
                            try
                            {
                                string basef_attr = NewWEBodyFaceAry[i].GetStringAttribute("ELECTRODE");
                                basef = NewWEBodyFaceAry[i];

                                #region 平移
                                if (basef_attr == "BASE_FACE")
                                {
                                    //CaxLog.ShowListingWindow(newWeCompName);
                                    CaxAsm.SetWorkComponent(newWeComp);
                                    double[] dir = new double[3];
                                    double[] center = new double[3];
                                    AskFaceCenter(NewWEBodyFaceAry[i], out center, out dir);
                                    Point3d sPoint3d = new Point3d();
                                    sPoint3d.X = 0 - center[0];
                                    sPoint3d.Y = 0 - center[1];
                                    sPoint3d.Z = 0 - center[2];
                                    MoveDisByXaxis(NewWEBodyFaceAry[i].GetBody(), sPoint3d.X);
                                    MoveDisByYaxis(NewWEBodyFaceAry[i].GetBody(), sPoint3d.Y);
                                    MoveDistByZaxis(NewWEBodyFaceAry[i].GetBody(), sPoint3d.Z);
                                    //break;
                                }
                                #endregion
                            }
                            catch (System.Exception ex)
                            {
                                try
                                {
                                    string edm_face = NewWEBodyFaceAry[i].GetStringAttribute("CIM_EDM_FACE");
                                    string edm_wedm = NewWEBodyFaceAry[i].GetStringAttribute(CIM_EDM_WEDM);
                                    if (edm_face == kvp.Key.wkface && edm_wedm == kvp.Key.section)
                                    {
                                        zf = NewWEBodyFaceAry[i];
                                    }
                                    else
                                    { continue; }
                                }
                                catch (System.Exception ex1)
                                { continue; }
                            }
                        }

                        #region new 旋轉
                        CaxGeom.FaceData basefData;
                        CaxGeom.GetFaceData(basef.Tag, out basefData);
                        CaxGeom.FaceData zfData;
                        CaxGeom.GetFaceData(zf.Tag, out zfData);
                        Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
                        Vector3d firstFace = new Vector3d(basefData.dir[0], basefData.dir[1], basefData.dir[2]);
                        Vector3d secondFace = new Vector3d(zfData.dir[0], zfData.dir[1], zfData.dir[2]);
                        Vector3d xDirection = new Vector3d(1.0, 0.0, 0.0);
                        Vector3d zDirection = new Vector3d(0.0, 0.0, 1.0);
                        MoveObject(basef.GetBody(), origin1, firstFace, secondFace, origin1, xDirection, zDirection);
                        #endregion

                        #region 決定基準角方位
                        RefCornerFace sRefCornerFace;
                        CaxGeom.FaceData sFaceDataA, sFaceDataB;
                        GetBaseCornerFaceAryOnPart(newWeComp, out sRefCornerFace);
                        Face cornerFaceA = sRefCornerFace.faceA;
                        Edge[] cornerFaceEdgeAry = cornerFaceA.GetEdges();
                        foreach (Edge SingleEdgeOnCornerFace in cornerFaceEdgeAry)
                        {
                            Face BlendFace = CaxGeom.GetNeighborFace(cornerFaceA, SingleEdgeOnCornerFace);
                            int type;
                            theUfSession.Modl.AskFaceType(BlendFace.Tag, out type);
                            double[] center = new double[3];
                            double[] dir = new double[3];
                            if (type == 16 || type == 23)
                            {
                                AskFaceCenter(BlendFace, out center, out dir);
                            }
                            if (center[1] > 0 && center[2] < 0)
                            {
                                RotateObjectByX(NewWEBody, 180);
                                BlendFace.SetAttribute("reference_posi", "Top");
                            }
                            else if (center[1] > 0 && center[2] > 0)
                            {
                                RotateObjectByX(NewWEBody, 180);
                                BlendFace.SetAttribute("reference_posi", "Bot");
                            }
                            else if (center[1] < 0 && center[2] > 0)
                            {
                                BlendFace.SetAttribute("reference_posi", "Top");
                            }
                            else if (center[1] < 0 && center[2] < 0)
                            {
                                BlendFace.SetAttribute("reference_posi", "Bot");
                            }
                        }
                        #endregion

                        #region old 旋轉
                        //零件旋轉
                        //                     for (int i = 0; i < NewWEBodyFaceAry.Length; i++)
                        //                     {
                        //                         try
                        //                         {
                        //                             string edm_face = NewWEBodyFaceAry[i].GetStringAttribute(CIM_EDM_FACE);
                        //                             string edm_wedm = NewWEBodyFaceAry[i].GetStringAttribute(CIM_EDM_WEDM);
                        //                             if (edm_face == kvp.Key.wkface && edm_wedm == kvp.Key.section)
                        //                             {
                        //                                 Point3d ssPoint3d = new Point3d(0, 0, 0);
                        //                                 Point sPoint = workPart.Points.CreatePoint(ssPoint3d);
                        //                                 J_PartSpinbyFacetoZ(NewWEBodyFaceAry[i].GetBody(), NewWEBodyFaceAry[i], sPoint);
                        //                                 break;
                        //                             }
                        //                         }
                        //                         catch (System.Exception ex)
                        //                         {
                        //                             continue;
                        //                         }
                        //                     }
                        #endregion

                        #region 長翅膀
                        //由BODY抓取被裁切面(BASE_FACE與外迴圈的面)
                        List<NXObject> List_Feat_Obj = new List<NXObject>();//存Enlarge出來的特徵
                        List<Face> BaseFaces = new List<Face>();
                        foreach (Face face in NewWEBodyFaceAry)
                        {
                            string bodyface_attr = "";
                            try
                            {
                                bodyface_attr = face.GetStringAttribute("ELECTRODE");
                                if (bodyface_attr == "BASE_FACE")
                                {
                                    BaseFaces.Add(face);
                                    Edge[] LoopEdgeAry = CaxUF_Lib.GetLoopEdges(face, (EdgeLoopType)1);
                                    foreach (Edge LoopEdge in LoopEdgeAry)
                                    {
                                        Face OuterLoopFace;
                                        OuterLoopFace = CaxGeom.GetNeighborFace(face, LoopEdge);
                                        BaseFaces.Add(OuterLoopFace);
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            { }
                        }

                        //由BASE_FACE抓內迴圈的面建立BBOX並取得此BBOX的BODY
                        List<Face> NeiFace = new List<Face>();
                        Tag theBlock = Tag.Null;
                        NXObject theBlockObj = null;
                        foreach (Face face in NewWEBodyFaceAry)
                        {
                            string bodyface_attr = "";
                            try
                            {
                                bodyface_attr = face.GetStringAttribute("ELECTRODE");
                                if (bodyface_attr == "BASE_FACE")
                                {
                                    Edge[] LoopEdgeAry = CaxUF_Lib.GetLoopEdges(face, (EdgeLoopType)2);
                                    foreach (Edge LoopEdge in LoopEdgeAry)
                                    {
                                        Face NeighborFace;
                                        NeighborFace = CaxGeom.GetNeighborFace(face, LoopEdge);
                                        NeiFace.Add(NeighborFace);
                                    }
                                    Tag[] LoopFaceTagAry = new Tag[NeiFace.Count];
                                    for (int i = 0; i < NeiFace.Count; i++)
                                    {
                                        LoopFaceTagAry[i] = NeiFace[i].Tag;
                                    }
                                    theBlock = theProgram.CreateWrapBlock(LoopFaceTagAry);
                                    theBlockObj = (NXObject)NXObjectManager.Get(theBlock);
                                    //List_Feat_Obj.Add(theBlockObj);//待確認是否正確
                                }
                            }
                            catch (System.Exception ex)
                            { }
                        }

                        //由BBOX取得四周的面
                        CaxGeom.FaceData sFaceData;
                        double[] sFaceData_dir = new double[3];
                        Body Bbox_Body = (Body)NXObjectManager.Get(theBlock);
                        Face[] Bbox_Body_FaceAry = Bbox_Body.GetFaces();
                        List<Tag> ListAroundBboxFace = new List<Tag>();
                        foreach (Face tempFace in Bbox_Body_FaceAry)
                        {
                            //ListAroundBboxFace.Add(tempFace.Tag);
                            CaxGeom.GetFaceData(tempFace.Tag, out sFaceData);
                            sFaceData_dir = sFaceData.dir;
                            if (sFaceData_dir[0] == 0 /*&& sFaceData_dir[2] != 0*/)
                            {
                                ListAroundBboxFace.Add(tempFace.Tag);
                            }
                        }

                        //擴大四周的面並與BaseFaces裁切
                        string[] EnlargeScale = { "10000", "10000", "10000", "10000" };
                        NXObject AroundFaceFeatObj;
                        Tag AroundFaceFeatObjTag;
                        NXOpen.Features.Feature outFeat = null;
                        foreach (Tag AroundFace in ListAroundBboxFace)
                        {
                            Tag[] AroundFaceFeatTagAry;
                            J_SurfaceEnlarge(AroundFace, EnlargeScale, out AroundFaceFeatObj, out AroundFaceFeatObjTag);
                            theUfSession.Modl.AskFeatFaces(AroundFaceFeatObjTag, out AroundFaceFeatTagAry);
                            Face EnlargeFace;
                            CaxTransType.TagFaceToNXOpenFace(AroundFaceFeatTagAry[0], out EnlargeFace);
                            List_Feat_Obj.Add(AroundFaceFeatObj);
                            foreach (Face bf in BaseFaces)
                            {
                                status = DivideFace(bf, EnlargeFace, out outFeat);
                                if (!status)
                                {
                                    continue;
                                }
                            }
                        }
                        RemoveParameters(NewWEBody);
                        foreach (NXObject FeatObj in List_Feat_Obj)
                        {
                            CaxPart.DeleteNXObject(FeatObj);
                        }

                        status = DeleteBody(theBlockObj);
                        if (!status)
                        {
                            return false;
                        }

                        Body NewBody = (Body)NewWEBody.Prototype;
                        Face[] NewBodyFaceAry1 = NewBody.GetFaces();
                        
                        //取得目前零件要加工的工段線割面
                        List<Face> CurrentSectionWEFace = new List<Face>();
                        foreach (Face tempFace in NewBodyFaceAry1)
                        {
                            string CurrentSectionAttr = "";
                            string CurrentWorkFaceAttr = "";
                            try
                            {
                                CurrentWorkFaceAttr = tempFace.GetStringAttribute("CIM_EDM_FACE");
                                continue;
                            }
                            catch (System.Exception ex)
                            {
                                try
                                {
                                    CurrentSectionAttr = tempFace.GetStringAttribute("CIM_EDM_WEDM");
                                    if (CurrentSectionAttr == kvp.Key.section)
                                    {
                                        CurrentSectionWEFace.Add(tempFace);
                                    }
                                }
                                catch (System.Exception ex1)
                                {continue;}
                            }
                        }
                        
                        //抓BASE_FACE切完後的面
//                         Dictionary<Key, string> SortFace = new Dictionary<Key, string>();
//                         List<double> minBBOX_Y = new List<double>();
//                         List<double> minBBOX_Z = new List<double>();
//                         List<double> minBBOX_Y_3 = new List<double>();
//                         List<double> minBBOX_Z_3 = new List<double>();
//                         bool chk;
//                         foreach (Face SingleBaseFace in NewBodyFaceAry1)
//                         {
//                             Tag SingleBaseFaceTag = SingleBaseFace.Tag;
//                             CaxGeom.GetFaceData(SingleBaseFaceTag, out sFaceData);
//                             if (sFaceData.dir[0] == 1 && sFaceData.dir[1] == 0 && sFaceData.dir[2] == 0)
//                             {
//                                 double[] minWcs = new double[3];
//                                 double[] maxWcs = new double[3];
//                                 CaxPart.AskBoundingBoxExactByWCS(SingleBaseFaceTag, out minWcs, out maxWcs);
//                                 if (minWcs[0] > -0.001 && minWcs[0] < 0.001)
//                                 {
//                                     minBBOX_Y.Add(minWcs[1]);
//                                     minBBOX_Z.Add(minWcs[2]);
// 
//                                     Key sKey = new Key();
//                                     sKey.Y = minWcs[1].ToString();
//                                     sKey.Z = minWcs[2].ToString();
// 
//                                     string KeyValue;
//                                     chk = SortFace.TryGetValue(sKey, out KeyValue);
//                                     if (chk)
//                                     {
//                                         string mergeTagValue = KeyValue + "_" + SingleBaseFace.ToString();
//                                         SortFace[sKey] = mergeTagValue;
//                                     }
//                                     else
//                                     {
//                                         SortFace.Add(sKey, SingleBaseFace.ToString());
//                                     }
//                                 }
//                             }
//                         }
//                         minBBOX_Y.Sort();
//                         minBBOX_Z.Sort();
                        //先取得左上角與右下角的面
                        List<Face> ListSetAttrFace = new List<Face>(); //要塞屬性的面
                        List<Face> ListPullFace = new List<Face>(); //要拉伸的面
//                         List<Face> LeftTop_RightBot = new List<Face>(); //左上角與右下角
//                         Key skey;
//                         string value = "";
//                         string[] splitvalur;
//                         Tag tempFaceTag = Tag.Null;
//                         Face tempFace1;
//                         //左上角
//                         skey.Y = minBBOX_Y[0].ToString();
//                         skey.Z = minBBOX_Z[minBBOX_Z.Count - 1].ToString();
//                         SortFace.TryGetValue(skey, out value);
//                         splitvalur = value.Split(' ');
//                         tempFaceTag = (Tag)Convert.ToInt32(splitvalur[1]);
//                         tempFace1 = (Face)NXObjectManager.Get(tempFaceTag);
//                         LeftTop_RightBot.Add(tempFace1);
//                         //右下角
//                         skey.Y = minBBOX_Y[minBBOX_Y.Count - 1].ToString();
//                         skey.Z = minBBOX_Z[0].ToString();
//                         SortFace.TryGetValue(skey, out value);
//                         splitvalur = value.Split(' ');
//                         tempFaceTag = (Tag)Convert.ToInt32(splitvalur[1]);
//                         tempFace1 = (Face)NXObjectManager.Get(tempFaceTag);
//                         LeftTop_RightBot.Add(tempFace1);
// 
//                         //由左上角與右下角找相鄰面
//                         foreach (Face tempface in LeftTop_RightBot)
//                         {
//                             Edge[] FaceEdgeAry = tempface.GetEdges();
//                             foreach (Edge SingleEdge in FaceEdgeAry)
//                             {
//                                 Face aa = CaxGeom.GetNeighborFace(tempface, SingleEdge);
//                                 double[] FaceCenter = new double[3];
//                                 double[] FaceDir = new double[3];
//                                 CaxPart.AskBoundingBoxExactByWCS(aa.Tag, out FaceCenter, out FaceDir);
//                                 if (FaceCenter[0] > -0.01 && FaceCenter[0] < 0.01)
//                                 {
//                                     ListSetAttrFace.Add(aa);
//                                 }
//                             }
//                         }

                        //由目前工段的線割面找相鄰面
                        foreach (Face SingleBodyFace in CurrentSectionWEFace)
                        {
                            string SingleBodyFaceAttr = "";
                            try
                            {
                                SingleBodyFaceAttr = SingleBodyFace.GetStringAttribute("MFG_COLOR");
                                if (SingleBodyFaceAttr == "213" || SingleBodyFaceAttr == "214" || SingleBodyFaceAttr == "215")
                                {
                                    Edge[] SingleBodyFaceEdgeAry = SingleBodyFace.GetEdges();
                                    foreach (Edge SingleBodyFaceEdge in SingleBodyFaceEdgeAry)
                                    {
                                        Face NeiFaceOnBaseFace = CaxGeom.GetNeighborFace(SingleBodyFace, SingleBodyFaceEdge);
                                        double[] NeiFaceOnBaseFaceCenter = new double[3];
                                        double[] NeiFaceOnBaseFaceDir = new double[3];
                                        AskFaceCenter(NeiFaceOnBaseFace, out NeiFaceOnBaseFaceCenter, out NeiFaceOnBaseFaceDir);
                                        if (NeiFaceOnBaseFaceCenter[0] > -0.01 && NeiFaceOnBaseFaceCenter[0] < 0.01)
                                        {
                                            ListSetAttrFace.Add(NeiFaceOnBaseFace);
                                        }
                                    }
                                }
                                else
                                { continue; }
                            }
                            catch (System.Exception ex)
                            { continue; }
                        }

                        foreach (Face CurrentSingleWEFace in CurrentSectionWEFace)
                        {
                            string CurrentSection = CurrentSingleWEFace.GetStringAttribute("CIM_EDM_WEDM");
                            string CurrentWEColor = CurrentSingleWEFace.GetStringAttribute("MFG_COLOR");
                            foreach (Face tempface in ListSetAttrFace)
                            {
                                tempface.SetAttribute("CIM_EDM_WEDM", CurrentSection);
                                tempface.SetAttribute("MFG_COLOR", CurrentWEColor);
                            }
                            break;
                        }
                        
                        //取得欲拉伸的面
                        foreach (Face tempface in ListSetAttrFace)
                        {
                            Edge[] tempfaceEdgeAry = tempface.GetEdges();
                            foreach (Edge tempfaceEdge in tempfaceEdgeAry)
                            {
                                Face tempPullFace = CaxGeom.GetNeighborFace(tempface, tempfaceEdge);
                                double[] tempCenter = new double[3];
                                double[] tempDir = new double[3];
                                AskFaceCenter(tempPullFace, out tempCenter, out tempDir);
                                if (tempCenter[0] < 0)
                                {
                                    ListPullFace.Add(tempPullFace);
                                }
                            }
                        }

                        //執行面拉伸功能
                        foreach (Face tempPullFace in ListPullFace)
                        {
                            double[] center = new double[3];
                            double[] dir = new double[3];
                            AskFaceCenter(tempPullFace, out center, out dir);
                            Point3d pt = new Point3d();
                            pt.X = center[0];
                            pt.Y = center[1];
                            pt.Z = center[2];
                            PullSelectFace(NewBody, tempPullFace, pt, "2");
                        }
                        RemoveParameters(NewBody);
                        #endregion
                    }
                    else//大製程
                    {
                        if (SlopePin == "斜銷")
                        {
                            CaxLog.ShowListingWindow("是斜銷");
                            //CaxAsm.SetWorkComponent(newWeComp);
                            if (kvp.Key.wkface == "T")
                            {
                                RotateObjectByZ(NewWEBody, 90);
                            }
                            else if (kvp.Key.wkface == "R")
                            {
                                RotateObjectByX(NewWEBody, 90);
                            }

                            #region NEW製程平移
                            double[] WPmin1 = new double[3];
                            double[] WPmax1 = new double[3];
                            Point3d firstpnt = new Point3d();
                            Point3d secondpnt = new Point3d();
                            CaxPart.AskBoundingBoxExactByWCS(NewWEBody.Tag, out WPmin1, out WPmax1);
                            firstpnt.X = (WPmax1[0] + WPmin1[0]) / 2;
                            firstpnt.Y = (WPmax1[1] + WPmin1[1]) / 2;
                            firstpnt.Z = WPmin1[2];
                            secondpnt.X = 0.0;
                            secondpnt.Y = 0.0;
                            secondpnt.Z = 0.0;
                            NewMoveObjectByPntToPnt(NewWEBody, firstpnt, secondpnt);
                            #endregion

                            List<Face> ListOffsetFace = new List<Face>();
                            Face[] NewWEBodyFaceAry = NewWEBody.GetFaces();
                            foreach (Face SingleNewWEBodyFace in NewWEBodyFaceAry)
                            {
                                string WEmfgcolor = "";
                                string WEsection = "";
                                string WEworkface = "";
                                try
                                {
                                    WEmfgcolor = SingleNewWEBodyFace.GetStringAttribute("MFG_COLOR");
                                    WEsection = SingleNewWEBodyFace.GetStringAttribute("WE_SECTION");
                                    WEworkface = SingleNewWEBodyFace.GetStringAttribute("WE_WORK_FACE");
                                    if (kvp.Key.section == WEsection && kvp.Key.wkface == WEworkface)
                                    {
                                        ListOffsetFace.Add(SingleNewWEBodyFace);
                                        //SingleNewWEBodyFace.Highlight();
                                        //CaxLog.ShowListingWindow(SingleNewWEBodyFace.ToString());
                                    }
                                }
                                catch (System.Exception ex)
                                { }
                            }

                            Body NewWEBodyProto = (Body)NewWEBody.Prototype;
                            foreach (Face offsetface in ListOffsetFace)
                            {
                                status = OffsetFace(NewWEBodyProto, offsetface, "0.02");
                                if (!status)
                                {
                                    CaxLog.ShowListingWindow("Offset Failed");
                                    return false;
                                }
                            }

                            RemoveParameters(NewWEBodyProto);

                            //return false;


                        }
                        else
                        {
                            #region NEW製程旋轉
                            CaxAsm.SetWorkComponent(newWeComp);
                            if (kvp.Key.wkface == "T")
                            {
                                RotateObjectByZ(NewWEBody, 90);
                            }
                            else if (kvp.Key.wkface == "F")
                            {
                                RotateObjectByY(NewWEBody, 90);
                            }
                            else if (kvp.Key.wkface == "R")
                            {
                                RotateObjectByX(NewWEBody, 90);
                            }
                            #endregion

                            #region NEW製程平移
                            WorkPiece WP1 = new WorkPiece();
                            double[] WPmin1 = new double[3];
                            double[] WPmax1 = new double[3];
                            Point3d firstpnt = new Point3d();
                            Point3d secondpnt = new Point3d();
                            CaxPart.AskBoundingBoxExactByWCS(NewWEBody.Tag, out WPmin1, out WPmax1);
                            firstpnt.X = (WPmax1[0] + WPmin1[0]) / 2;
                            firstpnt.Y = (WPmax1[1] + WPmin1[1]) / 2;
                            firstpnt.Z = WPmin1[2];
                            secondpnt.X = 0.0;
                            secondpnt.Y = 0.0;
                            secondpnt.Z = 0.0;
                            NewMoveObjectByPntToPnt(NewWEBody, firstpnt, secondpnt);
                            #endregion

                            #region NEW製程判斷機內、機外
                            string outer_inner1 = "";
                            string reference_posi1 = "";
                            RefCornerFace sRefCornerFace;
                            CaxGeom.FaceData sFaceDataA, sFaceDataB;
                            double[] cornerFaceA_dir = new double[3];
                            double[] cornerFaceB_dir = new double[3];
                            double[] XPositive = { 1, 0, 0 };
                            double[] XNegative = { -1, 0, 0 };
                            double[] YPositive = { 0, 1, 0 };
                            double[] YNegative = { 0, -1, 0 };
                            if (WP1.WP_Length >= 200 && WP1.WP_Wide >= 200 && WP1.WP_Height >= 100)
                            {
                                #region 旋轉工件使基準角符合機內校正：長與X平行
                                outer_inner1 = "2";
                                if (WP1.WP_Length < WP1.WP_Wide)
                                {
                                    double Rotate_Angle = 90;
                                    RotateObjectByZ(NewWEBody, Rotate_Angle);
                                }
                                GetBaseCornerFaceAryOnPart(newWeComp, out sRefCornerFace);
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
                                    RotateObjectByZ(NewWEBody, Rotate_Angle);
                                    reference_posi1 = "1";
                                }
                                else if (((cornerFaceA_dir[0] == XPositive[0] && cornerFaceA_dir[1] == XPositive[1] && cornerFaceA_dir[2] == XPositive[2]) &&
                                 (cornerFaceB_dir[0] == YNegative[0] && cornerFaceB_dir[1] == YNegative[1] && cornerFaceB_dir[2] == YNegative[2])) ||
                                 ((cornerFaceA_dir[0] == YNegative[0] && cornerFaceA_dir[1] == YNegative[1] && cornerFaceA_dir[2] == YNegative[2]) &&
                                 (cornerFaceB_dir[0] == XPositive[0] && cornerFaceB_dir[1] == XPositive[1] && cornerFaceB_dir[2] == XPositive[2])))
                                {
                                    double Rotate_Angle = -180;
                                    RotateObjectByZ(NewWEBody, Rotate_Angle);
                                    reference_posi1 = "2";
                                }
                                else if (((cornerFaceA_dir[0] == XPositive[0] && cornerFaceA_dir[1] == XPositive[1] && cornerFaceA_dir[2] == XPositive[2]) &&
                                         (cornerFaceB_dir[0] == YPositive[0] && cornerFaceB_dir[1] == YPositive[1] && cornerFaceB_dir[2] == YPositive[2])) ||
                                         ((cornerFaceA_dir[0] == YPositive[0] && cornerFaceA_dir[1] == YPositive[1] && cornerFaceA_dir[2] == YPositive[2]) &&
                                         (cornerFaceB_dir[0] == XPositive[0] && cornerFaceB_dir[1] == XPositive[1] && cornerFaceB_dir[2] == XPositive[2])))
                                {
                                    reference_posi1 = "1";
                                }
                                #endregion
                            }
                            else
                            {
                                #region 旋轉工件使基準角符合機外校正：長與Y平行
                                //outer_inner1 = "1";
                                outer_inner1 = "2";//谷崧測試用
                                if (WP1.WP_Length > WP1.WP_Wide)
                                {
                                    double Rotate_Angle = 90;
                                    RotateObjectByZ(NewWEBody, Rotate_Angle);
                                }
                                GetBaseCornerFaceAryOnPart(newWeComp, out sRefCornerFace);//TEST
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
                                    reference_posi1 = "4";
                                }
                                else if (((cornerFaceA_dir[0] == XNegative[0] && cornerFaceA_dir[1] == XNegative[1] && cornerFaceA_dir[2] == XNegative[2]) &&
                                         (cornerFaceB_dir[0] == YNegative[0] && cornerFaceB_dir[1] == YNegative[1] && cornerFaceB_dir[2] == YNegative[2])) ||
                                         ((cornerFaceA_dir[0] == YNegative[0] && cornerFaceA_dir[1] == YNegative[1] && cornerFaceA_dir[2] == YNegative[2]) &&
                                         (cornerFaceB_dir[0] == XNegative[0] && cornerFaceB_dir[1] == XNegative[1] && cornerFaceB_dir[2] == XNegative[2])))
                                {
                                    double Rotate_Angle = -180;
                                    RotateObjectByZ(NewWEBody, Rotate_Angle);
                                    reference_posi1 = "1";
                                }
                                else if (((cornerFaceA_dir[0] == XPositive[0] && cornerFaceA_dir[1] == XPositive[1] && cornerFaceA_dir[2] == XPositive[2]) &&
                                         (cornerFaceB_dir[0] == YPositive[0] && cornerFaceB_dir[1] == YPositive[1] && cornerFaceB_dir[2] == YPositive[2])) ||
                                         ((cornerFaceA_dir[0] == YPositive[0] && cornerFaceA_dir[1] == YPositive[1] && cornerFaceA_dir[2] == YPositive[2]) &&
                                         (cornerFaceB_dir[0] == XPositive[0] && cornerFaceB_dir[1] == XPositive[1] && cornerFaceB_dir[2] == XPositive[2])))
                                {
                                    reference_posi1 = "1";
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                } 
                //return false;
                
                WeListKey weGroupList = new WeListKey();
                weGroupList.compName = newWeComp.Name.ToUpper();

                Face[] NewBodyFaceAry = NewWEBody.GetFaces();
                for (int ii = 0; ii < NewBodyFaceAry.Length; ii++)
                {
                    try
                    {
                        string weSection = "";
                        string weWorkFace = "";
                        if (sCimAsmCompPart.electorde.Count != 0)
                        {
                            weSection = NewBodyFaceAry[ii].GetStringAttribute("CIM_EDM_WEDM");
                            //weWorkFace = NewBodyFaceAry[ii].GetStringAttribute("CIM_EDM_FACE");
                        }
                        else
                        {
                            if (SlopePin == "斜銷")
                            {
                                try
                                {
                                    weSection = NewBodyFaceAry[ii].GetStringAttribute("CIM_SECTION");
                                    weWorkFace = NewBodyFaceAry[ii].GetStringAttribute("CIM_WORK_FACE");
                                    if (weSection == "EDM1" && (weWorkFace == "T" || weWorkFace == "F" || weWorkFace == "R"))
                                    {
                                        weSection = NewBodyFaceAry[ii].GetStringAttribute("WE_SECTION");
                                        weWorkFace = NewBodyFaceAry[ii].GetStringAttribute("WE_WORK_FACE");
                                    }
                                    else
                                    { continue; }
                                }
                                catch (System.Exception ex)
                                {
                                    weSection = NewBodyFaceAry[ii].GetStringAttribute("WE_SECTION");
                                    weWorkFace = NewBodyFaceAry[ii].GetStringAttribute("WE_WORK_FACE");
                                }
                            } 
                            else
                            {
                                weSection = NewBodyFaceAry[ii].GetStringAttribute("CIM_SECTION");
                                weWorkFace = NewBodyFaceAry[ii].GetStringAttribute("CIM_WORK_FACE");
                            }
                        }

                        string[] splitWeSectionAry = weSection.Split(',');
                        for (int j = 0; j < splitWeSectionAry.Length; j++)
                        {
                            if (sCimAsmCompPart.electorde.Count != 0)//修改電極抓取線割面時的錯誤，之前會抓到加工面
                            {
                                string NewBodyFaceColor = NewBodyFaceAry[ii].GetStringAttribute("MFG_COLOR");
                                if (splitWeSectionAry[j] == kvp.Key.section && (NewBodyFaceColor == "213" || NewBodyFaceColor == "214" || NewBodyFaceColor == "215"))
                                {
                                    weGroupList.section = splitWeSectionAry[j];
                                    weGroupList.wkface = "Z";

                                    WeFaceGroup KeyComp;
                                    bool chk_value = false;
                                    chk_value = WEFaceDict.TryGetValue(weGroupList, out KeyComp);
                                    if (chk_value)
                                    {
                                        string mergeKeyComp = KeyComp.faceGroup + "," + NewBodyFaceAry[ii].Tag.ToString();
                                        KeyComp.faceGroup = mergeKeyComp;
                                        WEFaceDict[weGroupList] = KeyComp;
                                    }
                                    else
                                    {
                                        KeyComp.sFaceGroupPnt = new List<FaceGroupPnt>();
                                        KeyComp.comp = newWeComp;
                                        KeyComp.faceGroup = NewBodyFaceAry[ii].Tag.ToString();
                                        WEFaceDict.Add(weGroupList, KeyComp);//將每個comp的線割面存起來
                                    }
                                }
                            }
                            else
                            {
                                if (splitWeSectionAry[j] == kvp.Key.section && weWorkFace == kvp.Key.wkface)
                                {
                                    weGroupList.section = splitWeSectionAry[j];
                                    weGroupList.wkface = weWorkFace;

                                    WeFaceGroup KeyComp;
                                    bool chk_value = false;
                                    chk_value = WEFaceDict.TryGetValue(weGroupList, out KeyComp);
                                    if (chk_value)
                                    {
                                        string mergeKeyComp = KeyComp.faceGroup + "," + NewBodyFaceAry[ii].Tag.ToString();
                                        KeyComp.faceGroup = mergeKeyComp;
                                        WEFaceDict[weGroupList] = KeyComp;
                                    }
                                    else
                                    {
                                        KeyComp.sFaceGroupPnt = new List<FaceGroupPnt>();
                                        KeyComp.comp = newWeComp;
                                        KeyComp.faceGroup = NewBodyFaceAry[ii].Tag.ToString();
                                        WEFaceDict.Add(weGroupList, KeyComp);//將每個comp的線割面存起來
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    { continue; }
                }

                Thread.Sleep(0);

                //產生工件圖
                workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Trimetric, NXOpen.View.ScaleAdjustment.Fit);
                string ImagePath = "";
                ImagePath = string.Format(@"{0}\{1}_{2}", Path.GetDirectoryName(weCompPart.FullPath), kvp.Key.comp.Name, WE_PRT_NAME);
                theUfSession.Disp.CreateImage(ImagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);

                //將產生的線割檔加入隱藏列中，以便第二個檔案產生時可以隱藏
                hideDispalyObject.Add(newWeComp);
            }
            //return false;

            #endregion
            //return false;
            //CaxLog.ShowListingWindow(WEFaceDict.Count.ToString());

            #region 將線割零件檔的面全上色碼1
            Body wePartBody = null;
            foreach (KeyValuePair<WeListKey, WeFaceGroup> WEFace in WEFaceDict)
            {
                Part compPart = (Part)WEFace.Value.comp.Prototype;
                CaxPart.GetLayerBody(compPart, out wePartBody);
                Face[] wePartBodyFaceAry = wePartBody.GetFaces();
                for (int ii = 0; ii < wePartBodyFaceAry.Length; ii++)
                {
                    theUfSession.Obj.SetColor(wePartBodyFaceAry[ii].Tag, 1);
                }
            }

            #endregion

            CaxAsm.SetWorkComponent(null);

            //註解中
            #region 讀取線割零件檔屬性取得製程色並上色

            //             Body weCompbodyOcc = null;
            //             foreach (KeyValuePair<WeListKey, WeFaceGroup> WEFace in WEFaceDict)
            //             {
            //                 CaxPart.GetLayerBody(WEFace.Value.comp, out weCompbodyOcc);
            //                 Face[] weCompbodyFaceOccAry = weCompbodyOcc.GetFaces();
            //                 for (int ii = 0; ii < weCompbodyFaceOccAry.Length; ii++)
            //                 {
            //                     string NewBodyMFGColor = "";
            //                     Face TempNewBodyFace = weCompbodyFaceOccAry[ii];
            //                     try
            //                     {
            //                         NewBodyMFGColor = TempNewBodyFace.GetStringAttribute("MFG_COLOR");
            //                         theUfSession.Obj.SetColor(TempNewBodyFace.Tag, Convert.ToInt32(NewBodyMFGColor));
            //                     }
            //                     catch (System.Exception ex)
            //                     {
            //                         continue;
            //                     }
            //                 }
            //             }

            #endregion

            Dictionary<WeListKey, WeFaceGroup> bufWEFaceDict = new Dictionary<WeListKey, WeFaceGroup>();

            foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WEFaceDict)
            {
                string[] allfaceAry = kvp.Value.faceGroup.Split(',');
                string[] copyAllfaceAry = allfaceAry;

                for (int i = 0; i < allfaceAry.Length; i++)
                {
                    Face faceOcc = null;
                    CaxTransType.TagStrToFace(allfaceAry[i], out faceOcc);

                    WeFaceGroup sWeFaceGroup;
                    WEFaceDict.TryGetValue(kvp.Key, out sWeFaceGroup);

                    if (i == 0)
                    {
                        //第一個面，直接加入
                        FaceGroupPnt sFaceGroupPnt;
                        sFaceGroupPnt.faceOccAry = new List<Face>();
                        sFaceGroupPnt.pnt_x = "0.0";
                        sFaceGroupPnt.pnt_y = "0.0";

                        List<Face> NeighborFaceAry = new List<Face>();
                        NeighborFaceAry.Add(faceOcc);
                        GetWeNeighborFace(faceOcc, copyAllfaceAry, ref NeighborFaceAry, sCimAsmCompPart);
                        sFaceGroupPnt.faceOccAry.AddRange(NeighborFaceAry);

                        sWeFaceGroup.sFaceGroupPnt = new List<FaceGroupPnt>();
                        sWeFaceGroup.sFaceGroupPnt.Add(sFaceGroupPnt);
                        bufWEFaceDict.Add(kvp.Key, sWeFaceGroup);
                        //weFaceGroupAllList.Add(sFaceGroupPnt);
                    }
                    else
                    {
                        bool chk_face = false;
                        for (int j = 0; j < sWeFaceGroup.sFaceGroupPnt.Count; j++)
                        {
                            for (int k = 0; k < sWeFaceGroup.sFaceGroupPnt[j].faceOccAry.Count; k++)
                            {
                                if (faceOcc.Tag == sWeFaceGroup.sFaceGroupPnt[j].faceOccAry[k].Tag)
                                {
                                    chk_face = true;
                                    goto GOTO_CHK_FACE;
                                }
                            }
                        }

                    GOTO_CHK_FACE:
                        if (!chk_face)
                        {
                            FaceGroupPnt sFaceGroupPnt;
                            sFaceGroupPnt.faceOccAry = new List<Face>();
                            sFaceGroupPnt.pnt_x = "0.0";
                            sFaceGroupPnt.pnt_y = "0.0";

                            List<Face> NeighborFaceAry = new List<Face>();
                            NeighborFaceAry.Add(faceOcc);
                            GetWeNeighborFace(faceOcc, copyAllfaceAry, ref NeighborFaceAry, sCimAsmCompPart);
                            sFaceGroupPnt.faceOccAry.AddRange(NeighborFaceAry);

                            //sWeFaceGroup.sFaceGroupPnt = new List<FaceGroupPnt>();
                            sWeFaceGroup.sFaceGroupPnt.Add(sFaceGroupPnt);

                            bool chk_key;
                            WeFaceGroup bufWeFaceGroup;
                            chk_key = bufWEFaceDict.TryGetValue(kvp.Key, out bufWeFaceGroup);
                            if (chk_key)
                            {
                                bufWEFaceDict[kvp.Key] = sWeFaceGroup;
                            }
                            else
                            {
                                bufWEFaceDict.Add(kvp.Key, sWeFaceGroup);
                            }
                        }
                    }
                }
            }

            WEFaceDict = new Dictionary<WeListKey, WeFaceGroup>();
            WEFaceDict = bufWEFaceDict;
            return false;
            CaxPart.Save();
        }
        catch (System.Exception ex)
        {
            CaxLog.ShowListingWindow(ex.Message);
            CaxLog.ShowListingWindow("錯誤000");
            return false;
        }

        return true;
    }

    private static bool GetSectionFaceDic(CaxAsm.CimAsmCompPart sCimAsmCompPart, out Dictionary<skey, string> sectionFaceDic)
    {
        sectionFaceDic = new Dictionary<skey, string>();

        try
        {
            bool status;

            //取得有線割面的零件(設計零件&電極)
            Dictionary<string, CaxAsm.AsmCompPart> weCompDic = new Dictionary<string, CaxAsm.AsmCompPart>();
            status = GetWeCompDic(sCimAsmCompPart, out weCompDic);
            if (!status)
            { 
                return false; 
            }
            
            #region 將製程色、公差色塞回零件檔屬性欄中，並儲存檔案
            //將製程色塞回零件檔屬性欄中，並儲存檔案
            Body weCompBodyOcc = null;
            foreach (KeyValuePair<string, CaxAsm.AsmCompPart> weComp in weCompDic)
            {
                status = CaxPart.GetLayerBody(weComp.Value.comp, out weCompBodyOcc);
                if (!status)
                { 
                    return false;
                }

                Face[] ssBodyFaceAry = weCompBodyOcc.GetFaces();
                for (int ii = 0; ii < ssBodyFaceAry.Length; ii++)
                {
                    if (ssBodyFaceAry[ii].Color == 215 || ssBodyFaceAry[ii].Color == 213 || ssBodyFaceAry[ii].Color == 214 || ssBodyFaceAry[ii].Color == 105)
                    {
                        //塞製程色
                        Face ssBodyFaceProto = (Face)ssBodyFaceAry[ii].Prototype;
                        ssBodyFaceProto.SetAttribute("MFG_COLOR", ssBodyFaceAry[ii].Color.ToString());

                        //塞公差色
                        string tolColor = ssBodyFaceProto.Color.ToString();
                        ssBodyFaceProto.SetAttribute("TOL_COLOR", tolColor);
                    }
                }
            }
            CaxPart.Save();

            #endregion

            Body compBodyOcc = null;
            foreach (KeyValuePair<string, CaxAsm.AsmCompPart> kvp in weCompDic) //欲執行的線割零件
            {
                status = CaxPart.GetLayerBody(kvp.Value.comp, out compBodyOcc);
                if (!status)
                {
                    return false; 
                }

                Face[] compBodyFaceOccAry = compBodyOcc.GetFaces();
                foreach (Face compBodyFaceOcc in compBodyFaceOccAry)
                {
                    skey weFaceKey = new skey();

                    //                     if (compBodyFaceOcc.Color == 213 || compBodyFaceOcc.Color == 214 || compBodyFaceOcc.Color == 215)
                    //                     {
                    string ssBodyFaceAttr_Sec = "";
                    string ssBodyFaceAttr_Wkf = "";
                    try
                    {
                        if (sCimAsmCompPart.electorde.Count != 0)
                        {
                            ssBodyFaceAttr_Sec = compBodyFaceOcc.GetStringAttribute("CIM_EDM_WEDM");
                            ssBodyFaceAttr_Wkf = compBodyFaceOcc.GetStringAttribute("CIM_EDM_FACE");//加工面：Z
                            if (ssBodyFaceAttr_Wkf != "Z")
                            { continue; }
                        }
                        else
                        {
                            if (compBodyFaceOcc.Color == 213 || compBodyFaceOcc.Color == 214 || compBodyFaceOcc.Color == 215 || compBodyFaceOcc.Color == 105)
                            {
                                try
                                {
                                    ssBodyFaceAttr_Sec = compBodyFaceOcc.GetStringAttribute("CIM_SECTION");//工段：WEDMS1 或 WEDMS1,WEDMS2
                                    ssBodyFaceAttr_Wkf = compBodyFaceOcc.GetStringAttribute("CIM_WORK_FACE");//加工面：T
                                }
                                catch (System.Exception ex)
                                {
                                    MessageBox.Show("請確認是否有指定加工面");
                                    return false;
                                }
                            }
                            else
                            { continue; }
                        }

                        string[] ssBodyFaceSecAry = ssBodyFaceAttr_Sec.Split(',');
                        foreach (string ssBodyFaceSec in ssBodyFaceSecAry)
                        {
                            weFaceKey.comp = kvp.Value.comp;
                            weFaceKey.section = ssBodyFaceSec;
                            weFaceKey.wkface = ssBodyFaceAttr_Wkf;

                            bool chk;
                            chk = sectionFaceDic.ContainsKey(weFaceKey);
                            if (!chk)
                            {
                                sectionFaceDic.Add(weFaceKey, kvp.Value.comp.Tag.ToString());
                            }
                        }
                    }
                    catch (System.Exception ex)
                    { continue; }
                    //}

                }
            }
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }

    private static bool GetWeCompDic(CaxAsm.CimAsmCompPart sCimAsmCompPart, out Dictionary<string, CaxAsm.AsmCompPart> weCompDic)
    {
        weCompDic = new Dictionary<string, CaxAsm.AsmCompPart>();
        try
        {
            bool status;
            bool chk_key = false;
            Body elecBodyOcc = null;
            for (int i = 0; i < sCimAsmCompPart.electorde.Count; i++)
            {
                //取得電極第一層實體
                status = CaxPart.GetLayerBody(sCimAsmCompPart.electorde[i].comp, out elecBodyOcc);
                if (!status)
                {
                    return false;
                }

                Face[] elecBodyFaceAry = elecBodyOcc.GetFaces();
                for (int j = 0; j < elecBodyFaceAry.Length; j++)
                {
                    if (elecBodyFaceAry[j].Color == 213 || elecBodyFaceAry[j].Color == 214 || elecBodyFaceAry[j].Color == 215)
                    {
                        chk_key = weCompDic.ContainsKey(sCimAsmCompPart.electorde[i].comp.Name);
                        if (!chk_key)
                        {
                            weCompDic.Add(sCimAsmCompPart.electorde[i].comp.Name, sCimAsmCompPart.electorde[i]);
                        }
                        break;
                    }
                }
            }
            
            //取得工件第一層實體
            Body designBodyOcc;
            status = CaxPart.GetLayerBody(sCimAsmCompPart.design.comp, out designBodyOcc);
            if (!status)
            {
                return false;
            }
            Face[] designBodyOccFaceAry = designBodyOcc.GetFaces();
            for (int j = 0; j < designBodyOccFaceAry.Length; j++)
            {
                if (designBodyOccFaceAry[j].Color == 213 || designBodyOccFaceAry[j].Color == 214 || designBodyOccFaceAry[j].Color == 215 || designBodyOccFaceAry[j].Color == 105)
                {
                    chk_key = weCompDic.ContainsKey(sCimAsmCompPart.design.comp.Name);
                    if (!chk_key)
                    {
                        weCompDic.Add(sCimAsmCompPart.design.comp.Name, sCimAsmCompPart.design);
                    }
                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }

    private static bool GetColorDataAry(out Dictionary<int, string> mdColorDic)
    {
        bool status;

        mdColorDic = new Dictionary<int, string>();

        try
        {
            bool chk_key = false;
            string tolStr = "";
            List<CimforceCaxTwMD.CaxMD.ColorExcel> colorExcelAry = new List<CimforceCaxTwMD.CaxMD.ColorExcel>();

            //取得公差色
            status = CaxMD.GetToleranceColorDataAry(out colorExcelAry);
            if (!status)
            {
                return false;
            }
            for (int i = 0; i < colorExcelAry.Count; i++)
            {
                tolStr = string.Format("{0}_{1}", colorExcelAry[i].tol_upper, colorExcelAry[i].tol_lower);
                chk_key = mdColorDic.ContainsKey(Convert.ToInt32(colorExcelAry[i].ugid));
                if (!chk_key)
                {
                    mdColorDic.Add(Convert.ToInt32(colorExcelAry[i].ugid), tolStr);
                }
            }

            //取得VDI色
            status = CaxMD.GetVDIColorDataAry(out colorExcelAry);
            if (!status)
            {
                return false;
            }
            for (int i = 0; i < colorExcelAry.Count; i++)
            {
                tolStr = string.Format("{0}_{1}", colorExcelAry[i].tol_upper, colorExcelAry[i].tol_lower);
                chk_key = mdColorDic.ContainsKey(Convert.ToInt32(colorExcelAry[i].ugid));
                if (!chk_key)
                {
                    mdColorDic.Add(Convert.ToInt32(colorExcelAry[i].ugid), tolStr);
                }
            }

            //取得訊息色
            status = CaxMD.GetMessageColorDataAry(out colorExcelAry);
            if (!status)
            {
                return false;
            }
            for (int i = 0; i < colorExcelAry.Count; i++)
            {
                tolStr = string.Format("{0}_{1}", colorExcelAry[i].tol_upper, colorExcelAry[i].tol_lower);
                chk_key = mdColorDic.ContainsKey(Convert.ToInt32(colorExcelAry[i].ugid));
                if (!chk_key)
                {
                    mdColorDic.Add(Convert.ToInt32(colorExcelAry[i].ugid), tolStr);
                }
            }
            if (mdColorDic.Count == 0)
            {
                MessageBox.Show("未成功顏色資訊");
                return false;
            }
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }

    public static bool GetWeNeighborFace(Face face, string[] copyAllfaceAry, ref List<Face> faceAry, CaxAsm.CimAsmCompPart sCimAsmCompPart)
    {
        try
        {
            Face neighborFace = null;

            if (sCimAsmCompPart.electorde.Count != 0)//電極
            {
                Edge[] edgeAry = face.GetEdges();
                
                for (int j = 0; j < edgeAry.Length; j++)
                {
                    neighborFace = null;
                    neighborFace = CaxGeom.GetNeighborFace(face, edgeAry[j]);
                    if (neighborFace == null)
                    { continue; }
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
                        { continue; }
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
                            if (neighborFace == faceAry[k])
                            {
                                chk_face = true;
                                break;
                            }
                        }
                        if (!chk_face)
                        {
                            faceAry.Add(neighborFace);
                            GetWeNeighborFace(neighborFace, copyAllfaceAry, ref faceAry, sCimAsmCompPart);
                        }
                    }
                    catch (System.Exception ex)
                    { continue; }
                }
            } 
            else//工件
            {
                Edge[] edgeAry = face.GetEdges();
                string faceMachingType = face.GetStringAttribute("MACHING_TYPE");
                for (int j = 0; j < edgeAry.Length; j++)
                {
                    neighborFace = null;
                    neighborFace = CaxGeom.GetNeighborFace(face, edgeAry[j]);
                    if (neighborFace == null)
                    { continue; }
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
                        { continue; }
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
                            if (neighborFace == faceAry[k])
                            {
                                chk_face = true;
                                break;
                            }
                        }
                        if (!chk_face)
                        {
                            string NeiFaceMachingType = neighborFace.GetStringAttribute("MACHING_TYPE");
                            if (NeiFaceMachingType == faceMachingType)
                            {
                                faceAry.Add(neighborFace);
                                GetWeNeighborFace(neighborFace, copyAllfaceAry, ref faceAry, sCimAsmCompPart);
                            }
                        }
                    }
                    catch (System.Exception ex)
                    { continue; }
                }
            }
        }
        catch (System.Exception ex)
        {
            CaxLog.ShowListingWindow(ex.Message);
            CaxLog.ShowListingWindow("錯誤002");
            return false;
        }

        return true;
    }

    public static bool MoveObject(Body body, Point3d origin1, Vector3d xDirection1, Vector3d yDirection1, Point3d origin2, Vector3d xDirection2, Vector3d yDirection2)
    {
        try
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

            moveObjectBuilder1.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.CsysToCsys;

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = "0";

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

            Body body1 = body;
            //Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(1)");
            bool added1;
            added1 = moveObjectBuilder1.ObjectToMoveObject.Add(body1);

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

            Unit unit1;
            unit1 = moveObjectBuilder1.TransformMotion.RadialOriginDistance.Units;

            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Unit unit2;
            unit2 = moveObjectBuilder1.TransformMotion.DistanceAngle.Angle.Units;

            Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

            Expression expression3;
            expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression4;
            expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

            Expression expression5;
            expression5 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression6;
            expression6 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

            expression1.RightHandSide = "0";

            expression3.RightHandSide = "0";

            expression5.RightHandSide = "0";

            expression2.RightHandSide = "0";

            expression4.RightHandSide = "0";

            expression6.RightHandSide = "0";

            theSession.SetUndoMarkName(markId2, "CSYS Dialog");

            // ----------------------------------------------
            //   Dialog Begin CSYS
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "CSYS");

            theSession.DeleteUndoMark(markId3, null);

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "CSYS");

            //Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            //Vector3d xDirection1 = new Vector3d(1.0, 0.0, 0.0);
            //Vector3d yDirection1 = new Vector3d(0.0, 1.0, 0.0);
            Xform xform1;
            xform1 = workPart.Xforms.CreateXform(origin1, xDirection1, yDirection1, NXOpen.SmartObject.UpdateOption.WithinModeling, 1.0);

            /*
            Point3d origin2 = new Point3d(0.0, 0.0, 0.0);
            xform1.SetOrigin(origin2);

            Matrix3x3 orientation1;
            orientation1.Xx = 1.0;
            orientation1.Xy = 0.0;
            orientation1.Xz = 0.0;
            orientation1.Yx = 0.0;
            orientation1.Yy = 1.0;
            orientation1.Yz = 0.0;
            orientation1.Zx = 0.0;
            orientation1.Zy = 0.0;
            orientation1.Zz = 1.0;
            xform1.SetOrientation(orientation1);
            */

            CartesianCoordinateSystem cartesianCoordinateSystem1;
            cartesianCoordinateSystem1 = workPart.CoordinateSystems.CreateCoordinateSystem(xform1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            theSession.DeleteUndoMark(markId4, null);

            theSession.SetUndoMarkName(markId2, "CSYS");

            workPart.Expressions.Delete(expression1);

            workPart.Expressions.Delete(expression3);

            workPart.Expressions.Delete(expression5);

            workPart.Expressions.Delete(expression2);

            workPart.Expressions.Delete(expression4);

            workPart.Expressions.Delete(expression6);

            theSession.DeleteUndoMark(markId2, null);

            moveObjectBuilder1.TransformMotion.FromCsys = cartesianCoordinateSystem1;

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

            Expression expression7;
            expression7 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression8;
            expression8 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

            Expression expression9;
            expression9 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression10;
            expression10 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

            Expression expression11;
            expression11 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression12;
            expression12 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

            expression7.RightHandSide = "0";

            expression9.RightHandSide = "0";

            expression11.RightHandSide = "0";

            expression8.RightHandSide = "0";

            expression10.RightHandSide = "0";

            expression12.RightHandSide = "0";

            theSession.SetUndoMarkName(markId5, "CSYS Dialog");

            // ----------------------------------------------
            //   Dialog Begin CSYS
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId6;
            markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "CSYS");

            theSession.DeleteUndoMark(markId6, null);

            NXOpen.Session.UndoMarkId markId7;
            markId7 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "CSYS");

            //Point3d origin3 = new Point3d(0.0, 0.0, 0.0);
            //Vector3d xDirection2 = new Vector3d(1.0, 0.0, 0.0);
            //Vector3d yDirection2 = new Vector3d(0.0, 0.984807753012208, 0.17364817766693);
            Xform xform2;
            xform2 = workPart.Xforms.CreateXform(origin2, xDirection2, yDirection2, NXOpen.SmartObject.UpdateOption.WithinModeling, 1.0);

            /*
            Point3d origin4 = new Point3d(0.0, 0.0, 0.0);
            xform2.SetOrigin(origin4);

            Matrix3x3 orientation2;
            orientation2.Xx = 1.0;
            orientation2.Xy = 0.0;
            orientation2.Xz = 0.0;
            orientation2.Yx = -0.0;
            orientation2.Yy = 0.984807753012208;
            orientation2.Yz = 0.17364817766693;
            orientation2.Zx = 0.0;
            orientation2.Zy = -0.17364817766693;
            orientation2.Zz = 0.984807753012208;
            xform2.SetOrientation(orientation2);
            */

            CartesianCoordinateSystem cartesianCoordinateSystem2;
            cartesianCoordinateSystem2 = workPart.CoordinateSystems.CreateCoordinateSystem(xform2, NXOpen.SmartObject.UpdateOption.WithinModeling);

            theSession.DeleteUndoMark(markId7, null);

            theSession.SetUndoMarkName(markId5, "CSYS");

            workPart.Expressions.Delete(expression7);

            workPart.Expressions.Delete(expression9);

            workPart.Expressions.Delete(expression11);

            workPart.Expressions.Delete(expression8);

            workPart.Expressions.Delete(expression10);

            workPart.Expressions.Delete(expression12);

            theSession.DeleteUndoMark(markId5, null);

            moveObjectBuilder1.TransformMotion.ToCsys = cartesianCoordinateSystem2;

            NXOpen.Session.UndoMarkId markId8;
            markId8 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            theSession.DeleteUndoMark(markId8, null);

            NXOpen.Session.UndoMarkId markId9;
            markId9 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            NXObject nXObject1;
            nXObject1 = moveObjectBuilder1.Commit();

            NXObject[] objects1;
            objects1 = moveObjectBuilder1.GetCommittedObjects();

            theSession.DeleteUndoMark(markId9, null);

            theSession.SetUndoMarkName(markId1, "Move Object");

            moveObjectBuilder1.Destroy();

            NXOpen.Session.UndoMarkId markId10;
            markId10 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "");

            int nErrs1;
            nErrs1 = theSession.UpdateManager.DoUpdate(markId10);

            theSession.DeleteUndoMark(markId10, "");

            NXOpen.Session.UndoMarkId markId11;
            markId11 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "");

            int nErrs2;
            nErrs2 = theSession.UpdateManager.DoUpdate(markId11);

            theSession.DeleteUndoMark(markId11, "");
        }
        catch (System.Exception ex)
        {
            //CaxLog.ShowListingWindow(ex.Message);
            return false;
        }

        return true;
    }

    public static void MoveObjectByPntToPnt(Point3d firstpnt, Point3d secondpnt)
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

        moveObjectBuilder1.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.PointToPoint;

        moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = "-16";

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

        Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(1)");
        bool added1;
        added1 = moveObjectBuilder1.ObjectToMoveObject.Add(body1);

        NXOpen.Session.UndoMarkId markId2;
        markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

        Unit unit1;
        unit1 = moveObjectBuilder1.TransformMotion.RadialOriginDistance.Units;

        Expression expression1;
        expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

        Expression expression2;
        expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("p57_x=0.00000000000", unit1);

        Expression expression3;
        expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("p58_y=0.00000000000", unit1);

        Expression expression4;
        expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("p59_z=0.00000000000", unit1);

        Expression expression5;
        expression5 = workPart.Expressions.CreateSystemExpressionWithUnits("p60_xdelta=0.00000000000", unit1);

        Expression expression6;
        expression6 = workPart.Expressions.CreateSystemExpressionWithUnits("p61_ydelta=0.00000000000", unit1);

        Expression expression7;
        expression7 = workPart.Expressions.CreateSystemExpressionWithUnits("p62_zdelta=0.00000000000", unit1);

        Expression expression8;
        expression8 = workPart.Expressions.CreateSystemExpressionWithUnits("p63_radius=0.00000000000", unit1);

        Unit unit2;
        unit2 = moveObjectBuilder1.TransformMotion.DistanceAngle.Angle.Units;

        Expression expression9;
        expression9 = workPart.Expressions.CreateSystemExpressionWithUnits("p64_angle=0.00000000000", unit2);

        Expression expression10;
        expression10 = workPart.Expressions.CreateSystemExpressionWithUnits("p65_zdelta=0.00000000000", unit1);

        Expression expression11;
        expression11 = workPart.Expressions.CreateSystemExpressionWithUnits("p66_radius=0.00000000000", unit1);

        Expression expression12;
        expression12 = workPart.Expressions.CreateSystemExpressionWithUnits("p67_angle1=0.00000000000", unit2);

        Expression expression13;
        expression13 = workPart.Expressions.CreateSystemExpressionWithUnits("p68_angle2=0.00000000000", unit2);

        Expression expression14;
        expression14 = workPart.Expressions.CreateSystemExpressionWithUnits("p69_distance=0", unit1);

        Expression expression15;
        expression15 = workPart.Expressions.CreateSystemExpressionWithUnits("p70_arclen=0", unit1);

        Unit nullUnit = null;
        Expression expression16;
        expression16 = workPart.Expressions.CreateSystemExpressionWithUnits("p71_percent=0", nullUnit);

        expression2.RightHandSide = "0";

        expression3.RightHandSide = "0";

        expression4.RightHandSide = "0";

        expression5.RightHandSide = "0";

        expression6.RightHandSide = "0";

        expression7.RightHandSide = "0";

        expression8.RightHandSide = "0";

        expression9.RightHandSide = "0";

        expression10.RightHandSide = "0";

        expression11.RightHandSide = "0";

        expression12.RightHandSide = "0";

        expression13.RightHandSide = "0";

        expression14.RightHandSide = "0";

        expression16.RightHandSide = "100";

        expression15.RightHandSide = "0";

        theSession.SetUndoMarkName(markId2, "Point Dialog");

        expression2.RightHandSide = "0.00000000000";

        expression3.RightHandSide = "0.00000000000";

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

        expression16.RightHandSide = "100.00000000000";

        // ----------------------------------------------
        //   Dialog Begin Point
        // ----------------------------------------------
        Expression expression17;
        expression17 = workPart.Expressions.CreateSystemExpressionWithUnits("p72_x=0.00000000000", unit1);

        Scalar scalar1;
        scalar1 = workPart.Scalars.CreateScalarExpression(expression17, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Expression expression18;
        expression18 = workPart.Expressions.CreateSystemExpressionWithUnits("p73_y=0.00000000000", unit1);

        Scalar scalar2;
        scalar2 = workPart.Scalars.CreateScalarExpression(expression18, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Expression expression19;
        expression19 = workPart.Expressions.CreateSystemExpressionWithUnits("p74_z=0.00000000000", unit1);

        Scalar scalar3;
        scalar3 = workPart.Scalars.CreateScalarExpression(expression19, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Point point1;
        point1 = workPart.Points.CreatePoint(scalar1, scalar2, scalar3, NXOpen.SmartObject.UpdateOption.WithinModeling);

        expression2.RightHandSide = "0";

        expression3.RightHandSide = "0";

        expression4.RightHandSide = "0";

        expression2.RightHandSide = "0.00000000000";

        expression3.RightHandSide = "0.00000000000";

        expression4.RightHandSide = "0.00000000000";

        expression2.RightHandSide = "0";

        expression3.RightHandSide = "0";

        expression4.RightHandSide = "0";

        expression2.RightHandSide = "0.00000000000";

        expression3.RightHandSide = "0.00000000000";

        expression4.RightHandSide = "0.00000000000";

        expression2.RightHandSide = "0";

        expression3.RightHandSide = "0";

        expression4.RightHandSide = "0";

        expression2.RightHandSide = "0.00000000000";

        expression3.RightHandSide = "0.00000000000";

        expression4.RightHandSide = "0.00000000000";

        Point3d scaleAboutPoint1 = new Point3d(-32.9464555338105, -2.74231927655402, 0.0);
        Point3d viewCenter1 = new Point3d(32.9464555338104, 2.74231927655393, 0.0);
        displayPart.ModelingViews.WorkView.ZoomAboutPoint(1.25, scaleAboutPoint1, viewCenter1);

        Point3d scaleAboutPoint2 = new Point3d(-26.3571644270484, -2.19385542124322, 0.0);
        Point3d viewCenter2 = new Point3d(26.3571644270484, 2.19385542124314, 0.0);
        displayPart.ModelingViews.WorkView.ZoomAboutPoint(1.25, scaleAboutPoint2, viewCenter2);

        Point3d scaleAboutPoint3 = new Point3d(-21.0857315416387, -1.75508433699459, 0.0);
        Point3d viewCenter3 = new Point3d(21.0857315416387, 1.7550843369945, 0.0);
        displayPart.ModelingViews.WorkView.ZoomAboutPoint(1.25, scaleAboutPoint3, viewCenter3);

        Point3d scaleAboutPoint4 = new Point3d(-16.749931644331, -1.36451627326904, 0.0);
        Point3d viewCenter4 = new Point3d(16.749931644331, 1.36451627326895, 0.0);
        displayPart.ModelingViews.WorkView.ZoomAboutPoint(1.25, scaleAboutPoint4, viewCenter4);

        Point3d coordinates1 = firstpnt;
        Point point2;
        point2 = workPart.Points.CreatePoint(coordinates1);

        int nErrs1;
        nErrs1 = theSession.UpdateManager.AddToDeleteList(point2);

        workPart.Points.DeletePoint(point1);

        Expression expression20;
        expression20 = workPart.Expressions.CreateSystemExpressionWithUnits("p58_x=-30.9971648622904", unit1);

        Scalar scalar4;
        scalar4 = workPart.Scalars.CreateScalarExpression(expression20, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Expression expression21;
        expression21 = workPart.Expressions.CreateSystemExpressionWithUnits("p59_y=-18.9776759880042", unit1);

        Scalar scalar5;
        scalar5 = workPart.Scalars.CreateScalarExpression(expression21, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Expression expression22;
        expression22 = workPart.Expressions.CreateSystemExpressionWithUnits("p60_z=1.77635683940025e-015", unit1);

        Scalar scalar6;
        scalar6 = workPart.Scalars.CreateScalarExpression(expression22, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Point point3;
        point3 = workPart.Points.CreatePoint(scalar4, scalar5, scalar6, NXOpen.SmartObject.UpdateOption.WithinModeling);

        expression2.RightHandSide = "-30.9971648622904";

        expression3.RightHandSide = "-18.9776759880042";

        expression4.RightHandSide = "0";

        expression2.RightHandSide = "-30.99716486229";

        expression3.RightHandSide = "-18.97767598800";

        expression4.RightHandSide = "0.00000000000";

        NXOpen.Session.UndoMarkId markId3;
        markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

        theSession.DeleteUndoMark(markId3, null);

        NXOpen.Session.UndoMarkId markId4;
        markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

        theSession.DeleteUndoMark(markId4, null);

        theSession.SetUndoMarkName(markId2, "Point");

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
            workPart.Expressions.Delete(expression3);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

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

        workPart.Expressions.Delete(expression1);

        theSession.DeleteUndoMark(markId2, null);

        moveObjectBuilder1.TransformMotion.FromPoint = point3;

        Scalar scalar7;
        scalar7 = workPart.Scalars.CreateScalarExpression(expression20, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Scalar scalar8;
        scalar8 = workPart.Scalars.CreateScalarExpression(expression21, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Scalar scalar9;
        scalar9 = workPart.Scalars.CreateScalarExpression(expression22, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Point point4;
        point4 = workPart.Points.CreatePoint(scalar7, scalar8, scalar9, NXOpen.SmartObject.UpdateOption.WithinModeling);

        NXOpen.Session.UndoMarkId markId5;
        markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

        Expression expression23;
        expression23 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

        Expression expression24;
        expression24 = workPart.Expressions.CreateSystemExpressionWithUnits("p57_x=0.00000000000", unit1);

        Expression expression25;
        expression25 = workPart.Expressions.CreateSystemExpressionWithUnits("p58_y=0.00000000000", unit1);

        Expression expression26;
        expression26 = workPart.Expressions.CreateSystemExpressionWithUnits("p59_z=0.00000000000", unit1);

        Expression expression27;
        expression27 = workPart.Expressions.CreateSystemExpressionWithUnits("p60_xdelta=0.00000000000", unit1);

        Expression expression28;
        expression28 = workPart.Expressions.CreateSystemExpressionWithUnits("p61_ydelta=0.00000000000", unit1);

        Expression expression29;
        expression29 = workPart.Expressions.CreateSystemExpressionWithUnits("p62_zdelta=0.00000000000", unit1);

        Expression expression30;
        expression30 = workPart.Expressions.CreateSystemExpressionWithUnits("p63_radius=0.00000000000", unit1);

        Expression expression31;
        expression31 = workPart.Expressions.CreateSystemExpressionWithUnits("p64_angle=0.00000000000", unit2);

        Expression expression32;
        expression32 = workPart.Expressions.CreateSystemExpressionWithUnits("p65_zdelta=0.00000000000", unit1);

        Expression expression33;
        expression33 = workPart.Expressions.CreateSystemExpressionWithUnits("p66_radius=0.00000000000", unit1);

        Expression expression34;
        expression34 = workPart.Expressions.CreateSystemExpressionWithUnits("p67_angle1=0.00000000000", unit2);

        Expression expression35;
        expression35 = workPart.Expressions.CreateSystemExpressionWithUnits("p68_angle2=0.00000000000", unit2);

        Expression expression36;
        expression36 = workPart.Expressions.CreateSystemExpressionWithUnits("p69_distance=0", unit1);

        Expression expression37;
        expression37 = workPart.Expressions.CreateSystemExpressionWithUnits("p70_arclen=0", unit1);

        Expression expression38;
        expression38 = workPart.Expressions.CreateSystemExpressionWithUnits("p71_percent=0", nullUnit);

        expression24.RightHandSide = "-30.99716486229";

        expression25.RightHandSide = "-18.977675988";

        expression26.RightHandSide = "0";

        expression27.RightHandSide = "0";

        expression28.RightHandSide = "0";

        expression29.RightHandSide = "0";

        expression30.RightHandSide = "0";

        expression31.RightHandSide = "0";

        expression32.RightHandSide = "0";

        expression33.RightHandSide = "0";

        expression34.RightHandSide = "0";

        expression35.RightHandSide = "0";

        expression36.RightHandSide = "0";

        expression38.RightHandSide = "100";

        expression37.RightHandSide = "0";

        theSession.SetUndoMarkName(markId5, "Point Dialog");

        Expression expression39;
        expression39 = workPart.Expressions.CreateSystemExpressionWithUnits("p72_x=0.00000000000", unit1);

        Scalar scalar10;
        scalar10 = workPart.Scalars.CreateScalarExpression(expression39, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Expression expression40;
        expression40 = workPart.Expressions.CreateSystemExpressionWithUnits("p73_y=0.00000000000", unit1);

        Scalar scalar11;
        scalar11 = workPart.Scalars.CreateScalarExpression(expression40, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Expression expression41;
        expression41 = workPart.Expressions.CreateSystemExpressionWithUnits("p74_z=0.00000000000", unit1);

        Scalar scalar12;
        scalar12 = workPart.Scalars.CreateScalarExpression(expression41, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Point point5;
        point5 = workPart.Points.CreatePoint(scalar10, scalar11, scalar12, NXOpen.SmartObject.UpdateOption.WithinModeling);

        expression24.RightHandSide = "0.00000000000";

        expression25.RightHandSide = "0.00000000000";

        expression26.RightHandSide = "0.00000000000";

        expression27.RightHandSide = "0.00000000000";

        expression28.RightHandSide = "0.00000000000";

        expression29.RightHandSide = "0.00000000000";

        expression30.RightHandSide = "0.00000000000";

        expression31.RightHandSide = "0.00000000000";

        expression32.RightHandSide = "0.00000000000";

        expression33.RightHandSide = "0.00000000000";

        expression34.RightHandSide = "0.00000000000";

        expression35.RightHandSide = "0.00000000000";

        expression38.RightHandSide = "100.00000000000";

        // ----------------------------------------------
        //   Dialog Begin Point
        // ----------------------------------------------
        workPart.Points.DeletePoint(point5);

        Expression expression42;
        expression42 = workPart.Expressions.CreateSystemExpressionWithUnits("p59_x=0.00000000000", unit1);

        Scalar scalar13;
        scalar13 = workPart.Scalars.CreateScalarExpression(expression42, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Expression expression43;
        expression43 = workPart.Expressions.CreateSystemExpressionWithUnits("p60_y=0.00000000000", unit1);

        Scalar scalar14;
        scalar14 = workPart.Scalars.CreateScalarExpression(expression43, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Expression expression44;
        expression44 = workPart.Expressions.CreateSystemExpressionWithUnits("p61_z=0.00000000000", unit1);

        Scalar scalar15;
        scalar15 = workPart.Scalars.CreateScalarExpression(expression44, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Point point6;
        point6 = workPart.Points.CreatePoint(scalar13, scalar14, scalar15, NXOpen.SmartObject.UpdateOption.WithinModeling);

        expression24.RightHandSide = "0";

        expression25.RightHandSide = "0";

        expression26.RightHandSide = "0";

        expression24.RightHandSide = "0.00000000000";

        expression25.RightHandSide = "0.00000000000";

        expression26.RightHandSide = "0.00000000000";

        expression24.RightHandSide = "0";

        expression25.RightHandSide = "0";

        expression26.RightHandSide = "0";

        expression24.RightHandSide = "0.00000000000";

        expression25.RightHandSide = "0.00000000000";

        expression26.RightHandSide = "0.00000000000";

        expression24.RightHandSide = "0";

        expression25.RightHandSide = "0";

        expression26.RightHandSide = "0";

        expression24.RightHandSide = "0.00000000000";

        expression25.RightHandSide = "0.00000000000";

        expression26.RightHandSide = "0.00000000000";

        Point3d scaleAboutPoint5 = new Point3d(2.26232842988366, -1.97755981633192, 0.0);
        Point3d viewCenter5 = new Point3d(-2.26232842988369, 1.97755981633184, 0.0);
        displayPart.ModelingViews.WorkView.ZoomAboutPoint(0.8, scaleAboutPoint5, viewCenter5);

        Point3d scaleAboutPoint6 = new Point3d(2.82791053735457, -2.47194977041489, 0.0);
        Point3d viewCenter6 = new Point3d(-2.8279105373546, 2.47194977041481, 0.0);
        displayPart.ModelingViews.WorkView.ZoomAboutPoint(0.8, scaleAboutPoint6, viewCenter6);

        Point3d scaleAboutPoint7 = new Point3d(3.53488817169323, -3.08993721301861, 0.0);
        Point3d viewCenter7 = new Point3d(-3.53488817169325, 3.08993721301852, 0.0);
        displayPart.ModelingViews.WorkView.ZoomAboutPoint(0.8, scaleAboutPoint7, viewCenter7);

        Point3d coordinates2 = secondpnt;
        Point point7;
        point7 = workPart.Points.CreatePoint(coordinates2);

        int nErrs2;
        nErrs2 = theSession.UpdateManager.AddToDeleteList(point7);

        workPart.Points.DeletePoint(point6);

        Expression expression45;
        expression45 = workPart.Expressions.CreateSystemExpressionWithUnits("p59_x=-20.4668123157059", unit1);

        Scalar scalar16;
        scalar16 = workPart.Scalars.CreateScalarExpression(expression45, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Expression expression46;
        expression46 = workPart.Expressions.CreateSystemExpressionWithUnits("p60_y=1.75891567799956", unit1);

        Scalar scalar17;
        scalar17 = workPart.Scalars.CreateScalarExpression(expression46, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Expression expression47;
        expression47 = workPart.Expressions.CreateSystemExpressionWithUnits("p61_z=0", unit1);

        Scalar scalar18;
        scalar18 = workPart.Scalars.CreateScalarExpression(expression47, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Point point8;
        point8 = workPart.Points.CreatePoint(scalar16, scalar17, scalar18, NXOpen.SmartObject.UpdateOption.WithinModeling);

        expression24.RightHandSide = "-20.4668123157059";

        expression25.RightHandSide = "1.75891567799956";

        expression26.RightHandSide = "0";

        expression24.RightHandSide = "-20.46681231571";

        expression25.RightHandSide = "1.75891567800";

        expression26.RightHandSide = "0.00000000000";

        NXOpen.Session.UndoMarkId markId6;
        markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

        theSession.DeleteUndoMark(markId6, null);

        NXOpen.Session.UndoMarkId markId7;
        markId7 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

        theSession.DeleteUndoMark(markId7, null);

        theSession.SetUndoMarkName(markId5, "Point");

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression24);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression25);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression26);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression27);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression28);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression29);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression30);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression31);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression32);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression33);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression34);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression35);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression36);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression37);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        try
        {
            // Expression is still in use.
            workPart.Expressions.Delete(expression38);
        }
        catch (NXException ex)
        {
            ex.AssertErrorCode(1050029);
        }

        workPart.Expressions.Delete(expression23);

        theSession.DeleteUndoMark(markId5, null);

        moveObjectBuilder1.TransformMotion.ToPoint = point8;

        Scalar scalar19;
        scalar19 = workPart.Scalars.CreateScalarExpression(expression45, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Scalar scalar20;
        scalar20 = workPart.Scalars.CreateScalarExpression(expression46, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Scalar scalar21;
        scalar21 = workPart.Scalars.CreateScalarExpression(expression47, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

        Point point9;
        point9 = workPart.Points.CreatePoint(scalar19, scalar20, scalar21, NXOpen.SmartObject.UpdateOption.WithinModeling);

        NXOpen.Session.UndoMarkId markId8;
        markId8 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

        theSession.DeleteUndoMark(markId8, null);

        NXOpen.Session.UndoMarkId markId9;
        markId9 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

        NXObject nXObject1;
        nXObject1 = moveObjectBuilder1.Commit();

        NXObject[] objects1;
        objects1 = moveObjectBuilder1.GetCommittedObjects();

        theSession.DeleteUndoMark(markId9, null);

        theSession.SetUndoMarkName(markId1, "Move Object");

        moveObjectBuilder1.Destroy();

        workPart.Points.DeletePoint(point4);

        workPart.Points.DeletePoint(point9);

        // ----------------------------------------------
        //   Menu: Tools->Journal->Stop Recording
        // ----------------------------------------------

    }

    public static bool NewMoveObjectByPntToPnt(Body MoveBody, Point3d firstpnt, Point3d secondpnt)
    {

        try
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

            moveObjectBuilder1.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.PointToPoint;

            moveObjectBuilder1.TransformMotion.DistanceValue.RightHandSide = "-16";

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

            Body body1 = MoveBody;
            bool added1;
            added1 = moveObjectBuilder1.ObjectToMoveObject.Add(body1);

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

            Unit unit1;
            unit1 = moveObjectBuilder1.TransformMotion.RadialOriginDistance.Units;

            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("p257_x=0.00000000000", unit1);

            Expression expression3;
            expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("p258_y=0.00000000000", unit1);

            Expression expression4;
            expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("p259_z=0.00000000000", unit1);

            Expression expression5;
            expression5 = workPart.Expressions.CreateSystemExpressionWithUnits("p260_xdelta=0.00000000000", unit1);

            Expression expression6;
            expression6 = workPart.Expressions.CreateSystemExpressionWithUnits("p261_ydelta=0.00000000000", unit1);

            Expression expression7;
            expression7 = workPart.Expressions.CreateSystemExpressionWithUnits("p262_zdelta=0.00000000000", unit1);

            Expression expression8;
            expression8 = workPart.Expressions.CreateSystemExpressionWithUnits("p263_radius=0.00000000000", unit1);

            Unit unit2;
            unit2 = moveObjectBuilder1.TransformMotion.DistanceAngle.Angle.Units;

            Expression expression9;
            expression9 = workPart.Expressions.CreateSystemExpressionWithUnits("p264_angle=0.00000000000", unit2);

            Expression expression10;
            expression10 = workPart.Expressions.CreateSystemExpressionWithUnits("p265_zdelta=0.00000000000", unit1);

            Expression expression11;
            expression11 = workPart.Expressions.CreateSystemExpressionWithUnits("p266_radius=0.00000000000", unit1);

            Expression expression12;
            expression12 = workPart.Expressions.CreateSystemExpressionWithUnits("p267_angle1=0.00000000000", unit2);

            Expression expression13;
            expression13 = workPart.Expressions.CreateSystemExpressionWithUnits("p268_angle2=0.00000000000", unit2);

            Expression expression14;
            expression14 = workPart.Expressions.CreateSystemExpressionWithUnits("p269_distance=0", unit1);

            Expression expression15;
            expression15 = workPart.Expressions.CreateSystemExpressionWithUnits("p270_arclen=0", unit1);

            Unit nullUnit = null;
            Expression expression16;
            expression16 = workPart.Expressions.CreateSystemExpressionWithUnits("p271_percent=0", nullUnit);

            expression2.RightHandSide = "0";

            expression3.RightHandSide = "0";

            expression4.RightHandSide = "0";

            expression5.RightHandSide = "0";

            expression6.RightHandSide = "0";

            expression7.RightHandSide = "0";

            expression8.RightHandSide = "0";

            expression9.RightHandSide = "0";

            expression10.RightHandSide = "0";

            expression11.RightHandSide = "0";

            expression12.RightHandSide = "0";

            expression13.RightHandSide = "0";

            expression14.RightHandSide = "0";

            expression16.RightHandSide = "100";

            expression15.RightHandSide = "0";

            theSession.SetUndoMarkName(markId2, "Point Dialog");

            expression2.RightHandSide = "0.00000000000";

            expression3.RightHandSide = "0.00000000000";

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

            expression16.RightHandSide = "100.00000000000";

            // ----------------------------------------------
            //   Dialog Begin Point
            // ----------------------------------------------
            Expression expression17;
            expression17 = workPart.Expressions.CreateSystemExpressionWithUnits("p272_x=0.00000000000", unit1);

            Scalar scalar1;
            scalar1 = workPart.Scalars.CreateScalarExpression(expression17, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Expression expression18;
            expression18 = workPart.Expressions.CreateSystemExpressionWithUnits("p273_y=0.00000000000", unit1);

            Scalar scalar2;
            scalar2 = workPart.Scalars.CreateScalarExpression(expression18, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Expression expression19;
            expression19 = workPart.Expressions.CreateSystemExpressionWithUnits("p274_z=0.00000000000", unit1);

            Scalar scalar3;
            scalar3 = workPart.Scalars.CreateScalarExpression(expression19, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Point point1;
            point1 = workPart.Points.CreatePoint(scalar1, scalar2, scalar3, NXOpen.SmartObject.UpdateOption.WithinModeling);

            expression2.RightHandSide = "0";

            expression3.RightHandSide = "0";

            expression4.RightHandSide = "0";

            expression2.RightHandSide = "0.00000000000";

            expression3.RightHandSide = "0.00000000000";

            expression4.RightHandSide = "0.00000000000";

            expression2.RightHandSide = "0";

            expression3.RightHandSide = "0";

            expression4.RightHandSide = "0";

            expression2.RightHandSide = "0.00000000000";

            expression3.RightHandSide = "0.00000000000";

            expression4.RightHandSide = "0.00000000000";

            expression2.RightHandSide = "0";

            expression3.RightHandSide = "0";

            expression4.RightHandSide = "0";

            expression2.RightHandSide = "0.00000000000";

            expression3.RightHandSide = "0.00000000000";

            expression4.RightHandSide = "0.00000000000";

            expression2.RightHandSide = "0";

            expression3.RightHandSide = "0";

            expression4.RightHandSide = "0";

            expression2.RightHandSide = "0.00000000000";

            expression3.RightHandSide = "0.00000000000";

            expression4.RightHandSide = "0.00000000000";

            expression2.RightHandSide = "10";

            workPart.Points.DeletePoint(point1);

            expression2.RightHandSide = "10";

            expression3.RightHandSide = "0.00000000000";

            expression4.RightHandSide = "0.00000000000";

            Point3d coordinates1 = new Point3d(10.0, 0.0, 0.0);
            Point point2;
            point2 = workPart.Points.CreatePoint(coordinates1);

            expression3.RightHandSide = "10";

            expression2.RightHandSide = "10";

            expression3.RightHandSide = "10";

            expression4.RightHandSide = "0.00000000000";

            workPart.Points.DeletePoint(point2);

            Point3d coordinates2 = new Point3d(10.0, 10.0, 0.0);
            Point point3;
            point3 = workPart.Points.CreatePoint(coordinates2);

            expression4.RightHandSide = "20";

            expression2.RightHandSide = "10";

            expression3.RightHandSide = "10";

            expression4.RightHandSide = "20";

            workPart.Points.DeletePoint(point3);

            Point3d coordinates3 = firstpnt;
            Point point4;
            point4 = workPart.Points.CreatePoint(coordinates3);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

            theSession.DeleteUndoMark(markId3, null);

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

            expression2.RightHandSide = "10";

            expression3.RightHandSide = "10";

            expression4.RightHandSide = "20";

            workPart.Points.DeletePoint(point4);

            Point3d coordinates4 = firstpnt;
            Point point5;
            point5 = workPart.Points.CreatePoint(coordinates4);

            theSession.DeleteUndoMark(markId4, null);

            theSession.SetUndoMarkName(markId2, "Point");

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
                workPart.Expressions.Delete(expression3);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

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

            workPart.Expressions.Delete(expression1);

            theSession.DeleteUndoMark(markId2, null);

            moveObjectBuilder1.TransformMotion.FromPoint = point5;

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

            Expression expression20;
            expression20 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression21;
            expression21 = workPart.Expressions.CreateSystemExpressionWithUnits("p257_x=0.00000000000", unit1);

            Expression expression22;
            expression22 = workPart.Expressions.CreateSystemExpressionWithUnits("p258_y=0.00000000000", unit1);

            Expression expression23;
            expression23 = workPart.Expressions.CreateSystemExpressionWithUnits("p259_z=0.00000000000", unit1);

            Expression expression24;
            expression24 = workPart.Expressions.CreateSystemExpressionWithUnits("p260_xdelta=0.00000000000", unit1);

            Expression expression25;
            expression25 = workPart.Expressions.CreateSystemExpressionWithUnits("p261_ydelta=0.00000000000", unit1);

            Expression expression26;
            expression26 = workPart.Expressions.CreateSystemExpressionWithUnits("p262_zdelta=0.00000000000", unit1);

            Expression expression27;
            expression27 = workPart.Expressions.CreateSystemExpressionWithUnits("p263_radius=0.00000000000", unit1);

            Expression expression28;
            expression28 = workPart.Expressions.CreateSystemExpressionWithUnits("p264_angle=0.00000000000", unit2);

            Expression expression29;
            expression29 = workPart.Expressions.CreateSystemExpressionWithUnits("p265_zdelta=0.00000000000", unit1);

            Expression expression30;
            expression30 = workPart.Expressions.CreateSystemExpressionWithUnits("p266_radius=0.00000000000", unit1);

            Expression expression31;
            expression31 = workPart.Expressions.CreateSystemExpressionWithUnits("p267_angle1=0.00000000000", unit2);

            Expression expression32;
            expression32 = workPart.Expressions.CreateSystemExpressionWithUnits("p268_angle2=0.00000000000", unit2);

            Expression expression33;
            expression33 = workPart.Expressions.CreateSystemExpressionWithUnits("p269_distance=0", unit1);

            Expression expression34;
            expression34 = workPart.Expressions.CreateSystemExpressionWithUnits("p270_arclen=0", unit1);

            Expression expression35;
            expression35 = workPart.Expressions.CreateSystemExpressionWithUnits("p271_percent=0", nullUnit);

            expression21.RightHandSide = "10";

            expression22.RightHandSide = "10";

            expression23.RightHandSide = "20";

            expression24.RightHandSide = "0";

            expression25.RightHandSide = "0";

            expression26.RightHandSide = "0";

            expression27.RightHandSide = "0";

            expression28.RightHandSide = "0";

            expression29.RightHandSide = "0";

            expression30.RightHandSide = "0";

            expression31.RightHandSide = "0";

            expression32.RightHandSide = "0";

            expression33.RightHandSide = "0";

            expression35.RightHandSide = "100";

            expression34.RightHandSide = "0";

            theSession.SetUndoMarkName(markId5, "Point Dialog");

            Point3d coordinates5 = secondpnt;
            Point point6;
            point6 = workPart.Points.CreatePoint(coordinates5);

            expression21.RightHandSide = "0.00000000000";

            expression22.RightHandSide = "0.00000000000";

            expression23.RightHandSide = "0.00000000000";

            expression24.RightHandSide = "0.00000000000";

            expression25.RightHandSide = "0.00000000000";

            expression26.RightHandSide = "0.00000000000";

            expression27.RightHandSide = "0.00000000000";

            expression28.RightHandSide = "0.00000000000";

            expression29.RightHandSide = "0.00000000000";

            expression30.RightHandSide = "0.00000000000";

            expression31.RightHandSide = "0.00000000000";

            expression32.RightHandSide = "0.00000000000";

            expression35.RightHandSide = "100.00000000000";

            // ----------------------------------------------
            //   Dialog Begin Point
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId6;
            markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

            theSession.DeleteUndoMark(markId6, null);

            NXOpen.Session.UndoMarkId markId7;
            markId7 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

            workPart.Points.DeletePoint(point6);

            Point3d coordinates6 = secondpnt;
            Point point7;
            point7 = workPart.Points.CreatePoint(coordinates6);

            theSession.DeleteUndoMark(markId7, null);

            theSession.SetUndoMarkName(markId5, "Point");

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression21);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression22);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression23);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression24);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression25);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression26);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression27);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression28);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression29);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression30);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression31);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression32);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression33);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression34);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression35);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            workPart.Expressions.Delete(expression20);

            theSession.DeleteUndoMark(markId5, null);

            moveObjectBuilder1.TransformMotion.ToPoint = point7;

            NXOpen.Session.UndoMarkId markId8;
            markId8 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            theSession.DeleteUndoMark(markId8, null);

            NXOpen.Session.UndoMarkId markId9;
            markId9 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Object");

            NXObject nXObject1;
            nXObject1 = moveObjectBuilder1.Commit();

            NXObject[] objects1;
            objects1 = moveObjectBuilder1.GetCommittedObjects();

            theSession.DeleteUndoMark(markId9, null);

            theSession.SetUndoMarkName(markId1, "Move Object");

            moveObjectBuilder1.Destroy();
        }
        catch (System.Exception ex)
        {
            return true;
        }
        // ----------------------------------------------
        //   Menu: Tools->Journal->Stop Recording
        // ----------------------------------------------
        return true;
    }

    public static void J_SurfaceEnlarge(Tag face, string[] percent_size, out NXObject FeatObj, out Tag FeatObjTag)
    {
        Session theSession = Session.GetSession();
        Part workPart = theSession.Parts.Work;
        Part displayPart = theSession.Parts.Display;
        // ----------------------------------------------
        //   Menu: Edit->Surface->Enlarge...
        // ----------------------------------------------

        NXOpen.Features.Enlarge nullFeatures_Enlarge = null;

        if (!workPart.Preferences.Modeling.GetHistoryMode())
        {
            throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
        }

        NXOpen.Features.EnlargeBuilder enlargeBuilder1;
        enlargeBuilder1 = workPart.Features.CreateEnlargeBuilder(nullFeatures_Enlarge);

        enlargeBuilder1.ResizeParameters.UStart.Expression.RightHandSide = "0";

        enlargeBuilder1.ResizeParameters.UStart.Expression.RightHandSide = "0";

        enlargeBuilder1.ResizeParameters.UEnd.Expression.RightHandSide = "0";

        enlargeBuilder1.ResizeParameters.UEnd.Expression.RightHandSide = "0";

        enlargeBuilder1.ResizeParameters.VStart.Expression.RightHandSide = "0";

        enlargeBuilder1.ResizeParameters.VStart.Expression.RightHandSide = "0";

        enlargeBuilder1.ResizeParameters.VEnd.Expression.RightHandSide = "0";

        enlargeBuilder1.ResizeParameters.VEnd.Expression.RightHandSide = "0";

        enlargeBuilder1.ResizeParameters.AnchorPosition = NXOpen.GeometricUtilities.SurfaceRangeBuilder.AnchorPositionType.Vertex1;

        enlargeBuilder1.ResizeParameters.UStart.Expression.RightHandSide = percent_size[0].ToString();

        enlargeBuilder1.ResizeParameters.UEnd.Expression.RightHandSide = percent_size[1].ToString();

        enlargeBuilder1.ResizeParameters.VStart.Expression.RightHandSide = percent_size[2].ToString();

        enlargeBuilder1.ResizeParameters.VEnd.Expression.RightHandSide = percent_size[3].ToString();

        enlargeBuilder1.ExtensionType = NXOpen.Features.EnlargeBuilder.ExtensionTypes.Natural;

        enlargeBuilder1.IsCopy = true;

        enlargeBuilder1.ResizeParameters.UStart.Expression.RightHandSide = percent_size[0].ToString();

        enlargeBuilder1.ResizeParameters.UEnd.Expression.RightHandSide = percent_size[1].ToString();

        enlargeBuilder1.ResizeParameters.VStart.Expression.RightHandSide = percent_size[2].ToString();

        enlargeBuilder1.ResizeParameters.VEnd.Expression.RightHandSide = percent_size[3].ToString();

        enlargeBuilder1.ResizeParameters.UStart.Update(NXOpen.GeometricUtilities.OnPathDimensionBuilder.UpdateReason.Path);

        enlargeBuilder1.ResizeParameters.UEnd.Update(NXOpen.GeometricUtilities.OnPathDimensionBuilder.UpdateReason.Path);

        enlargeBuilder1.ResizeParameters.VStart.Update(NXOpen.GeometricUtilities.OnPathDimensionBuilder.UpdateReason.Path);

        enlargeBuilder1.ResizeParameters.VEnd.Update(NXOpen.GeometricUtilities.OnPathDimensionBuilder.UpdateReason.Path);
        Face temptFace = null;
        CaxTransType.TagFaceToNXOpenFace(face, out temptFace);
        //             NXOpen.Features.ExtractFace extractFace1 = (NXOpen.Features.ExtractFace)workPart.Features.FindObject(mainBody.JournalIdentifier);
        //             Face face1 = (Face)extractFace1.FindObject(temptFace.JournalIdentifier);
        Face face1 = temptFace;
        enlargeBuilder1.Face.Value = face1;

        enlargeBuilder1.ResizeParameters.UStart.Update(NXOpen.GeometricUtilities.OnPathDimensionBuilder.UpdateReason.Path);

        enlargeBuilder1.ResizeParameters.UEnd.Update(NXOpen.GeometricUtilities.OnPathDimensionBuilder.UpdateReason.Path);

        enlargeBuilder1.ResizeParameters.VStart.Update(NXOpen.GeometricUtilities.OnPathDimensionBuilder.UpdateReason.Path);

        enlargeBuilder1.ResizeParameters.VEnd.Update(NXOpen.GeometricUtilities.OnPathDimensionBuilder.UpdateReason.Path);

        enlargeBuilder1.ResizeParameters.UStart.Expression.RightHandSide = percent_size[0].ToString();

        enlargeBuilder1.ResizeParameters.UEnd.Expression.RightHandSide = percent_size[1].ToString();

        enlargeBuilder1.ResizeParameters.VStart.Expression.RightHandSide = percent_size[2].ToString();

        enlargeBuilder1.ResizeParameters.VEnd.Expression.RightHandSide = percent_size[3].ToString();

        enlargeBuilder1.ResizeParameters.UStart.Update(NXOpen.GeometricUtilities.OnPathDimensionBuilder.UpdateReason.Path);

        enlargeBuilder1.ResizeParameters.UEnd.Update(NXOpen.GeometricUtilities.OnPathDimensionBuilder.UpdateReason.Path);

        enlargeBuilder1.ResizeParameters.VStart.Update(NXOpen.GeometricUtilities.OnPathDimensionBuilder.UpdateReason.Path);

        enlargeBuilder1.ResizeParameters.VEnd.Update(NXOpen.GeometricUtilities.OnPathDimensionBuilder.UpdateReason.Path);

        enlargeBuilder1.ResizeParameters.UStart.Expression.RightHandSide = percent_size[0].ToString();

        //NXObject nXObject1;
        FeatObj = enlargeBuilder1.Commit();
        FeatObjTag = FeatObj.Tag;

        DisplayModification displayModification1;
        displayModification1 = theSession.DisplayManager.NewDisplayModification();

        displayModification1.ApplyToAllFaces = false;

        displayModification1.SetNewGrid(0, 0);

        displayModification1.PoleDisplayState = false;

        displayModification1.KnotDisplayState = false;

        //DisplayableObject[] objects1 = new DisplayableObject[1];
        //NXOpen.Features.Enlarge enlarge1 = (NXOpen.Features.Enlarge)nXObject1;
        //Face face2 = (Face)enlarge1.FindObject("FACE 1 {(93.3827568535445,-8.2689637651,1.7575638410756) ENLARGE(7)}");
        //objects1[0] = face2;
        // displayModification1.Apply(objects1);

        //face2.Color = 32767;

        //   theSession.SetUndoMarkName(markId1, "Enlarge");

        Expression expression1 = enlargeBuilder1.ResizeParameters.VStart.Expression;
        Expression expression2 = enlargeBuilder1.ResizeParameters.VEnd.Expression;
        Expression expression3 = enlargeBuilder1.ResizeParameters.UStart.Expression;
        Expression expression4 = enlargeBuilder1.ResizeParameters.UEnd.Expression;
        enlargeBuilder1.Destroy();

        // ----------------------------------------------
        //   Menu: Tools->Journal->Stop Recording
        // ----------------------------------------------
    }

    public static bool DivideFace(Face baseFace, Face enlargeFace, out  NXOpen.Features.Feature outFeature)
    {
        outFeature = null;
        try
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Insert->Trim->Divide Face...
            // ----------------------------------------------

            NXOpen.Features.Feature nullFeatures_Feature = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.DividefaceBuilder dividefaceBuilder1;
            dividefaceBuilder1 = workPart.Features.CreateDividefaceBuilder(nullFeatures_Feature);

            dividefaceBuilder1.BlankOption = true;

            NXOpen.GeometricUtilities.ProjectionOptions projectionOptions1;
            projectionOptions1 = dividefaceBuilder1.ProjectionOption;

            ScCollector scCollector1;
            scCollector1 = workPart.ScCollectors.CreateCollector();

            //             NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject("UNPARAMETERIZED_FEATURE(4)");
            //             Face face1 = (Face)brep1.FindObject("FACE 23 {(62.5,90.25,3.8499999999996) UNPARAMETERIZED_FEATURE(4)}");
            Face face1 = baseFace;
            Face[] boundaryFaces1 = new Face[0];
            FaceTangentRule faceTangentRule1;
            faceTangentRule1 = workPart.ScRuleFactory.CreateRuleFaceTangent(face1, boundaryFaces1, 0.01);

            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
            rules1[0] = faceTangentRule1;
            scCollector1.ReplaceRules(rules1, false);

            dividefaceBuilder1.FacesToDivide = scCollector1;

            Section section1;
            section1 = workPart.Sections.CreateSection(0.0019, 0.002, 0.01);

            section1.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

            ScCollector scCollector2;
            scCollector2 = workPart.ScCollectors.CreateCollector();

            //             NXOpen.Features.Enlarge enlarge1 = (NXOpen.Features.Enlarge)workPart.Features.FindObject("ENLARGE(devil)");
            //             Face face2 = (Face)enlarge1.FindObject("FACE 1 {(62.5,100.5,-4.1500000000005) ENLARGE(6)}");

            Face face2 = enlargeFace;
            Face[] boundaryFaces2 = new Face[0];
            FaceTangentRule faceTangentRule2;
            faceTangentRule2 = workPart.ScRuleFactory.CreateRuleFaceTangent(face2, boundaryFaces2, 0.01);

            SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
            rules2[0] = faceTangentRule2;
            scCollector2.ReplaceRules(rules2, false);

            bool added1;
            added1 = dividefaceBuilder1.DividingObjectsList.Add(scCollector2);

            projectionOptions1.ProjectDirectionMethod = NXOpen.GeometricUtilities.ProjectionOptions.DirectionType.FaceNormal;


            NXOpen.Features.Feature feature1;
            feature1 = dividefaceBuilder1.CommitFeature();

            outFeature = feature1;

            dividefaceBuilder1.Destroy();

            section1.Destroy();

        }
        catch (System.Exception ex)
        {
            //CaxLog.ShowListingWindow(ex.ToString());
            return false;
        }
        return true;
    }

    public Tag CreateWrapBlock(Tag[] objects)
    {
        Tag result = Tag.Null;
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
        Tag blockFeat = Tag.Null;
        string xLen = (max[0] - min[0]).ToString();
        string yLen = (max[1] - min[1]).ToString();
        string zLen = (max[2] - min[2]).ToString();
        string[] edgeLen = { xLen, yLen, zLen };

        //建立block特徵
        theUfSession.Modl.CreateBlock(FeatureSigns.Nullsign, Tag.Null, min, edgeLen, out blockFeat);
        if (blockFeat != Tag.Null)
        {
            //取得block特徵底下的body
            theUfSession.Modl.AskFeatBody(blockFeat, out result);
        }

        return result;
    }

    public static bool PullSelectFace(Body body, Face sel_face, Point3d pt, string pull_length)
    {
        try
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Insert->Synchronous Modeling->Pull Face...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Features.PullFace nullFeatures_PullFace = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.PullFaceBuilder pullFaceBuilder1;
            pullFaceBuilder1 = workPart.Features.CreatePullFaceBuilder(nullFeatures_PullFace);

            pullFaceBuilder1.Motion.DistanceAngle.OrientXpress.AxisOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Axis.Passive;

            pullFaceBuilder1.Motion.DistanceAngle.OrientXpress.PlaneOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Plane.Passive;

            pullFaceBuilder1.Motion.AlongCurveAngle.AlongCurve.IsPercentUsed = true;

            pullFaceBuilder1.Motion.AlongCurveAngle.AlongCurve.Expression.RightHandSide = "0";

            pullFaceBuilder1.Motion.AlongCurveAngle.AlongCurve.Expression.RightHandSide = "0";

            pullFaceBuilder1.Motion.OrientXpress.AxisOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Axis.Passive;

            pullFaceBuilder1.Motion.OrientXpress.PlaneOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Plane.Passive;

            pullFaceBuilder1.Motion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.Distance;

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = "10";

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = "0";

            pullFaceBuilder1.Motion.DistanceBetweenPointsDistance.RightHandSide = "0";

            pullFaceBuilder1.Motion.DistanceBetweenPointsDistance.RightHandSide = "0";

            pullFaceBuilder1.Motion.RadialDistance.RightHandSide = "0";

            pullFaceBuilder1.Motion.RadialDistance.RightHandSide = "0";

            pullFaceBuilder1.Motion.Angle.RightHandSide = "0";

            pullFaceBuilder1.Motion.Angle.RightHandSide = "0";

            pullFaceBuilder1.Motion.DistanceAngle.Distance.RightHandSide = "0";

            pullFaceBuilder1.Motion.DistanceAngle.Distance.RightHandSide = "0";

            pullFaceBuilder1.Motion.DistanceAngle.Angle.RightHandSide = "0";

            pullFaceBuilder1.Motion.DistanceAngle.Angle.RightHandSide = "0";

            pullFaceBuilder1.Motion.DeltaEnum = NXOpen.GeometricUtilities.ModlMotion.Delta.ReferenceWcsWorkPart;

            pullFaceBuilder1.Motion.DeltaXc.RightHandSide = "0";

            pullFaceBuilder1.Motion.DeltaYc.RightHandSide = "0";

            pullFaceBuilder1.Motion.DeltaZc.RightHandSide = "0";

            pullFaceBuilder1.Motion.AlongCurveAngle.AlongCurve.Expression.RightHandSide = "0";

            pullFaceBuilder1.Motion.AlongCurveAngle.AlongCurve.Expression.RightHandSide = "0";

            pullFaceBuilder1.Motion.AlongCurveAngle.AlongCurveAngle.RightHandSide = "0";

            pullFaceBuilder1.Motion.AlongCurveAngle.AlongCurveAngle.RightHandSide = "0";

            theSession.SetUndoMarkName(markId1, "Pull Face Dialog");

            //NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject("UNPARAMETERIZED_FEATURE(1)");
            //Face face1 = (Face)brep1.FindObject("FACE 29 {(-2,-8.5,-1) UNPARAMETERIZED_FEATURE(1)}");
            NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject(body.GetFeatures()[0].JournalIdentifier);
            //CaxLog.ShowListingWindow(body.GetFeatures()[0].JournalIdentifier);
            Face face1 = (Face)brep1.FindObject(sel_face.JournalIdentifier);
            //Face face1 = sel_face;
            ICurve[] curves1 = new ICurve[0];
            //Point3d seedPoint1 = new Point3d(-2.02131196152968, -8.49999999999998, 1.29111335797215);
            Point3d seedPoint1 = pt;
            FaceRegionBoundaryRule faceRegionBoundaryRule1;
            faceRegionBoundaryRule1 = workPart.ScRuleFactory.CreateRuleFaceRegionBoundary(face1, curves1, seedPoint1, 0.002);

            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
            rules1[0] = faceRegionBoundaryRule1;
            pullFaceBuilder1.FaceToPull.ReplaceRules(rules1, false);

            Scalar scalar1;
            scalar1 = workPart.Scalars.CreateScalar(0.5, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Scalar scalar2;
            scalar2 = workPart.Scalars.CreateScalar(0.5, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Point point1;
            point1 = workPart.Points.CreatePoint(face1, scalar1, scalar2, NXOpen.SmartObject.UpdateOption.WithinModeling);

            Direction direction1;
            direction1 = workPart.Directions.CreateDirection(face1, point1, Sense.Forward, NXOpen.SmartObject.UpdateOption.WithinModeling);

            pullFaceBuilder1.Motion.DistanceVector = direction1;

            Point point2;
            point2 = workPart.Points.CreatePoint(face1, scalar1, scalar2, NXOpen.SmartObject.UpdateOption.WithinModeling);

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = "0.5";

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = "1";

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = "1.5";

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = "2";

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = "2.5";

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = "3";

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = "3.5";

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = "4";

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = "4.5";

            pullFaceBuilder1.Motion.DistanceValue.RightHandSide = pull_length;

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Pull Face");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Pull Face");

            NXObject nXObject1;
            nXObject1 = pullFaceBuilder1.Commit();

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Pull Face");

            Expression expression1 = pullFaceBuilder1.Motion.DistanceValue;
            pullFaceBuilder1.Destroy();

            workPart.Points.DeletePoint(point2);

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

    public static bool DeleteBody(NXObject BodyObj)
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
            //NXOpen.Features.Block block1 = (NXOpen.Features.Block)workPart.Features.FindObject("BLOCK(3)");
            objects1[0] = BodyObj;
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

    public static bool OffsetFace(Body body, Face sel_face, string OffsetValue)
    {
        try
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Tools->Repeat Command->3 Offset Face
            // ----------------------------------------------
            // ----------------------------------------------
            //   Menu: Insert->Offset/Scale->Offset Face...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Features.Feature nullFeatures_Feature = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.OffsetFaceBuilder offsetFaceBuilder1;
            offsetFaceBuilder1 = workPart.Features.CreateOffsetFaceBuilder(nullFeatures_Feature);

            //offsetFaceBuilder1.Distance.RightHandSide = "10";

            theSession.SetUndoMarkName(markId1, "Offset Face Dialog");

            offsetFaceBuilder1.Distance.RightHandSide = "0";

            //NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject("UNPARAMETERIZED_FEATURE(1)");
            //Face face1 = (Face)brep1.FindObject("FACE 8 {(-60.1572914500404,-0.58080666606,0) UNPARAMETERIZED_FEATURE(1)}");
            NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject(body.GetFeatures()[0].JournalIdentifier);
            Face face1 = (Face)brep1.FindObject(sel_face.JournalIdentifier);
            Face[] boundaryFaces1 = new Face[0];
            FaceTangentRule faceTangentRule1;
            faceTangentRule1 = workPart.ScRuleFactory.CreateRuleFaceTangent(face1, boundaryFaces1, 0.01);

            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
            rules1[0] = faceTangentRule1;
            offsetFaceBuilder1.FaceCollector.ReplaceRules(rules1, false);

            offsetFaceBuilder1.Distance.RightHandSide = OffsetValue;

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Offset Face");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Offset Face");

            NXObject nXObject1;
            nXObject1 = offsetFaceBuilder1.Commit();

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Offset Face");

            Expression expression1 = offsetFaceBuilder1.Distance;
            offsetFaceBuilder1.Destroy();

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

    public static void WAVEGeometry(Component component, Body body)
    {
        Session theSession = Session.GetSession();
        Part workPart = theSession.Parts.Work;
        Part displayPart = theSession.Parts.Display;
        // ----------------------------------------------
        //   Menu: Insert->Associative Copy->WAVE Geometry Linker...
        // ----------------------------------------------
        NXOpen.Session.UndoMarkId markId1;
        markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

        NXOpen.Features.Feature nullFeatures_Feature = null;

        if (!workPart.Preferences.Modeling.GetHistoryMode())
        {
            throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
        }

        NXOpen.Features.WaveLinkBuilder waveLinkBuilder1;
        waveLinkBuilder1 = workPart.BaseFeatures.CreateWaveLinkBuilder(nullFeatures_Feature);

        NXOpen.Features.WaveDatumBuilder waveDatumBuilder1;
        waveDatumBuilder1 = waveLinkBuilder1.WaveDatumBuilder;

        NXOpen.Features.CompositeCurveBuilder compositeCurveBuilder1;
        compositeCurveBuilder1 = waveLinkBuilder1.CompositeCurveBuilder;

        NXOpen.Features.WaveSketchBuilder waveSketchBuilder1;
        waveSketchBuilder1 = waveLinkBuilder1.WaveSketchBuilder;

        NXOpen.Features.WaveRoutingBuilder waveRoutingBuilder1;
        waveRoutingBuilder1 = waveLinkBuilder1.WaveRoutingBuilder;

        NXOpen.Features.WavePointBuilder wavePointBuilder1;
        wavePointBuilder1 = waveLinkBuilder1.WavePointBuilder;

        NXOpen.Features.ExtractFaceBuilder extractFaceBuilder1;
        extractFaceBuilder1 = waveLinkBuilder1.ExtractFaceBuilder;

        NXOpen.Features.MirrorBodyBuilder mirrorBodyBuilder1;
        mirrorBodyBuilder1 = waveLinkBuilder1.MirrorBodyBuilder;

        extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

        waveLinkBuilder1.Type = NXOpen.Features.WaveLinkBuilder.Types.BodyLink;

        extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

        extractFaceBuilder1.AngleTolerance = 45.0;

        waveDatumBuilder1.DisplayScale = 2.0;

        extractFaceBuilder1.ParentPart = NXOpen.Features.ExtractFaceBuilder.ParentPartType.OtherPart;

        mirrorBodyBuilder1.ParentPartType = NXOpen.Features.MirrorBodyBuilder.ParentPart.OtherPart;

        theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker Dialog");

        compositeCurveBuilder1.Section.DistanceTolerance = 0.002;

        compositeCurveBuilder1.Section.ChainingTolerance = 0.0019;

        extractFaceBuilder1.Associative = true;

        extractFaceBuilder1.MakePositionIndependent = false;

        extractFaceBuilder1.FixAtCurrentTimestamp = false;

        extractFaceBuilder1.HideOriginal = false;

        extractFaceBuilder1.InheritDisplayProperties = false;

        SelectObjectList selectObjectList1;
        selectObjectList1 = extractFaceBuilder1.BodyToExtract;

        extractFaceBuilder1.CopyThreads = true;

        //NXOpen.Assemblies.Component component1 = (NXOpen.Assemblies.Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT X903J-0011T_U100_WE 1");
        NXOpen.Assemblies.Component component1 = component;
        //CaxLog.ShowListingWindow("PROTO#.Bodies|"+body.JournalIdentifier.ToString());
        Body body1 = (Body)component1.FindObject("PROTO#.Bodies|" + body.JournalIdentifier);
        //CaxLog.ShowListingWindow(body1.IsOccurrence.ToString());
        //CaxLog.ShowListingWindow(Path.GetFileNameWithoutExtension(body1.OwningComponent.Name));

        bool added1;
        added1 = selectObjectList1.Add(body1);

        NXOpen.Session.UndoMarkId markId2;
        markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

        theSession.DeleteUndoMark(markId2, null);

        NXOpen.Session.UndoMarkId markId3;
        markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

        NXObject nXObject1;
        nXObject1 = waveLinkBuilder1.Commit();

        theSession.DeleteUndoMark(markId3, null);

        theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker");

        waveLinkBuilder1.Destroy();

        // ----------------------------------------------
        //   Menu: Tools->Journal->Stop Recording
        // ----------------------------------------------

    }


}

