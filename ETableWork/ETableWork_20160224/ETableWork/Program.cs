using System;
using NXOpen;
using NXOpen.UF;
using System.Windows.Forms;
using ETableWork;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using NXOpen.CAM;
using NXOpen.Utilities;
using LicenseCheck;
using NXOpenUI;
using CimforceCaxTwPublic;
using CimforceCaxTwCNC;
using System.Collections.Generic;
using NXOpen.Annotations;
using DevComponents.DotNetBar;


public class Program
{
    //user define
//     private const string SUPPORT_MACHINE = "";
//     private const string MACHINE_TYPE = "";
//     private const string CONTROLLER = "";
    //private const string POST_FUNCTION = "COXON_FANUC_OKK";

//    private const string POST_LIST_DIR = "D:\\0_SIMFORCE\\0_Project\\0_SrcCode\\ETableWork\\post_list.csv";
//     private const string SUPPORT_MACHINE = "";
//     private const string MACHINE_TYPE = "";
//     private const string CONTROLLER = "";
//     private const string MACHINE_GROUP = "";

    // class members
    private static Session theSession;
    private static UI theUI;
    private static UFSession theUfSession;
    public static Program theProgram;
    public static bool isDisposeCalled;


//     public static string ATTR_CIM_Z_MOVE = "CIM_Z_MOVE";
//     public static string ATTR_CIM_Z_BASE = "CIM_Z_BASE";
//     public static string ATTR_CIM_FACE_X_OFFSET = "CIM_FACE_X_OFFSET";
//     public static string ATTR_CIM_FACE_Y_OFFSET = "CIM_FACE_Y_OFFSET";

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

    //------------------------------------------------------------------------------
    //  Explicit Activation
    //      This entry point is used to activate the application explicitly
    //------------------------------------------------------------------------------
    public static int Main(string[] args)
    {
        int retValue = 0;
        try
        {
            theProgram = new Program();

            //TODO: Add your application code here 
            Class000_Main.DoEverything(Class000_Main.METHOD_NORMAL);

//             bool status;
//             Class001_ETableInit eTableWorkInit = new Class001_ETableInit();
//             status = eTableWorkInit.GetAllParameter();
//             if (!status)
//             {
//                 theProgram.Dispose();
//                 return retValue;
//             }
#region stewart hide 20150529
            
//             #region �T�{�Ұ�����
// 
//             //�T�{�Ұ�����
//             string license_status = "";
//             license_status = License.ChkCimforceLicense();
//             if (license_status != "CIMFORCE_LICENSE_SUCCESS")
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "License ���~!");
//                 theProgram.Dispose();
//                 return retValue;
//             }
// 
//             Part displayPrt = theSession.Parts.Display;
//             if (displayPrt == null)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ERROR,���}�ҹs��.");
//                 theProgram.Dispose();
//                 return retValue;
//             }
// 
//             int module_id;
//             theUfSession.UF.AskApplicationModule(out module_id);
//             if (module_id != UFConstants.UF_APP_CAM)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "�Х������� CAM �Ҳ�");
//                 theProgram.Dispose();
//                 return retValue;
//             }
// 
//             #endregion
// 
// //             bool status;
// 
//             //���o�ɮצW��
//             string asmName;
//             theUfSession.Part.AskPartName(displayPrt.Tag, out asmName);
// 
//             string rootPath;
//             rootPath = Path.GetDirectoryName(asmName);
// 
//             #region ���omes2cam.dat���(���c)
// 
//             string mes2camDatPath = CaxCNC.GetMes2CamDatPath(displayPrt);
//             if (mes2camDatPath == "")
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "mes2cam.dat Ū������");
//                 theProgram.Dispose();
//                 return retValue;
//             }
// 
//             //���omes2cam.dat���
//             CimforceCaxTwCNC.MesAttrCNC sMesDatData;
//             if (!CaxCNC.ReadMesAttrCNCJsonData(mes2camDatPath, out sMesDatData))
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, mes2camDatPath + " Ū������!");
//                 theProgram.Dispose();
//                 return retValue;
//             }
//             #endregion
// 
//             #region ���o�M��̤j���d�ةR(���c)
// 
//             //���o�M��̤j���d�ةR(���c)
//             CimforceCaxTwCNC.ToolCuttingLength cToolCuttingLength;
//             status = CaxCNC.ReadToolCuttingLengthCNCJsonData(out cToolCuttingLength);
//             if (!status || cToolCuttingLength == null)
//             {
//                 CaxLog.ShowListingWindow("MES �M��ةR�t�m��Ū������...");
//                 theProgram.Dispose();
//                 return retValue;
//             }
// 
//             /*
//             //���o�˱󦡤M���̤j���d�ةR(���c)
//             CimforceCaxTwCNC.BladeLength cBladeLength;
//             status = CaxCNC.ReadBladeLengthCNCJsonData(out cBladeLength);
//             if (!status || cBladeLength == null)
//             {
//                 CaxLog.ShowListingWindow("MES �M��ةR�t�m��Ū������...");
//                 theProgram.Dispose();
//                 return retValue;
//             }
//             */
// 
//             #endregion
// 
//             #region Ū���t�m��(ETableConfig.txt)
//             string JsonConfigFilePath = string.Format(@"{0}\Cimforce\CNC\config\{1}", CaxFile.GetCimforceEnvDir(), "ETableConfig.txt");
//             ConfigData config = new ConfigData();
//             status = ReadJsonData(JsonConfigFilePath, out config);
//             if (!status)
//             {
//                 CaxLog.ShowListingWindow("Ū���t�m��ETableConfig.txt�ɵo�Ϳ��~");
//                 return -1;
//             }
// 
// 
// 
//             #endregion
// 
//             #region 2014/04/09 �s�W���x��������POST
// 
//             //2014/04/09 �s�W���x��������POST�������������������������������������������p��X�ק�
//             /*
//             //Ū��post�������x�����c
//             PostCNC classPostCNC = new PostCNC();
//             status = CaxCNC.ReadPostCNCJsonData(out classPostCNC);
//             if (!status)
//             {
//                 theProgram.Dispose();
//                 return retValue;
//             }
//             for (int i = 0; i < classPostCNC.CAM_POST.Count;i++ )
//             {
//                 if (classPostCNC.CAM_POST[i].MAC_MODEL_NO.Trim() == sMesDatData.MAC_MODEL_NO.Trim())
//                 {
//                     postFunctionName = classPostCNC.CAM_POST[i].POST_NAME.Trim();
//                     break;
//                 }
//             }
//             */
// 
//             //���o�Ҧ��i�Ϊ�Post�W��
//             int count;
//             string[] PostNames;
//             theUfSession.Cam.OptAskPostNames(out count, out PostNames);
// 
//             string postFunctionName = "";
//             postFunctionName = sMesDatData.MAC_POST_NM;
//             if (postFunctionName == "")
//             {
//                 CaxLog.ShowListingWindow("���x����������POSTŪ�����~...");
//                 theProgram.Dispose();
//                 return retValue;
//             }
// 
//             for (int i = 0; i < PostNames.Length;i++ )
//             {
//                 if (PostNames[i].ToUpper() == sMesDatData.MAC_POST_NM.ToUpper())
//                 {
//                     postFunctionName = PostNames[i];
//                     break;
//                 }
//             }
// 
//             //2014/04/09 �s�W���x��������POST�������������������������������������������p��X�ק�
// 
//             #endregion
// 
//             #region �T�{�O�_���إ߸˧��� 2014/01/06  (20150525 �אּ�T�{�O�_���إߥ|�i�ϡA�å[�J�P�_���q�W��)
//             string section_face = string.Format(@"{0}_{1}", sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID);
//             string baseHoleName = "";
//             string beforeCNCName = "";
//             string afterCNCName = "";
//             // DEPO
//             if (config.companyName.ToUpper() == "DEPO")
//             {
//                 baseHoleName = string.Format(@"{0}_{1}_{2}", sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID, "BaseHole");
//                 beforeCNCName = string.Format(@"{0}_{1}_{2}", sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID, "BeforeCNC");
//                 afterCNCName = string.Format(@"{0}_{1}_{2}", sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID, "AfterCNC");
//                 int drawingNum = 0;
//                 string drawingName = "";
//                 Tag[] drawings;
//                 bool chk_drafting_name = false;
//                 bool chk_baseHole_name = false;
//                 bool chk_beforeCNC_name = false;
//                 bool chk_afterCNC_name = false;
//                 theUfSession.Draw.AskDrawings(out drawingNum, out drawings);
//                 for (int i = 0; i < drawingNum; i++)
//                 {
//                     theUfSession.Obj.AskName(drawings[i], out drawingName);
//                     if (drawingName.ToUpper() == section_face.ToUpper())
//                     {
//                         chk_drafting_name = true;
//                     }
//                     if (drawingName.ToUpper() == baseHoleName.ToUpper())
//                     {
//                         chk_baseHole_name = true;
//                     }
//                     if (drawingName.ToUpper() == beforeCNCName.ToUpper())
//                     {
//                         chk_beforeCNC_name = true;
//                     }
//                     if (drawingName.ToUpper() == afterCNCName.ToUpper())
//                     {
//                         chk_afterCNC_name = true;
//                     }
//                 }
//                 if (!chk_drafting_name)
//                 {
//                     UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ERROR,�|���إ� " + section_face + " �˧���");
//                     theProgram.Dispose();
//                     return retValue;
//                 }
//                 // �Ҥ��~�ˬd�o�T�i��
//                 if (sMesDatData.PART_TYPE_ID == "0")
//                 {
//                     if (!chk_baseHole_name)
//                     {
//                         string dialogText = string.Format("�q����Ǥչϩ|���إߡA�O�_���n�X�q�l�u��H");
//                         eTaskDialogResult result = CaxMsg.ShowMsgYesNo(dialogText, eTaskDialogIcon.Help, "�q����Ǥչϩ|���إ�");
//                         if (result == eTaskDialogResult.Yes)
//                         {
//                             baseHoleName = "";
//                         }
//                         if (result == eTaskDialogResult.No)
//                         {
//                             theProgram.Dispose();
//                             return retValue;
//                         }
//                     }
//                     if (!chk_beforeCNC_name)
//                     {
//                         string dialogText = string.Format("�[�u�e�˴��ϩ|���إߡA�O�_���n�X�q�l�u��H");
//                         eTaskDialogResult result = CaxMsg.ShowMsgYesNo(dialogText, eTaskDialogIcon.Help, "�[�u�e�˴��ϩ|���إ�");
//                         if (result == eTaskDialogResult.Yes)
//                         {
//                             beforeCNCName = "";
//                         }
//                         if (result == eTaskDialogResult.No)
//                         {
//                             theProgram.Dispose();
//                             return retValue;
//                         }
//                     }
//                     if (!chk_afterCNC_name)
//                     {
//                         string dialogText = string.Format("�[�u���˴��ϩ|���إߡA�O�_���n�X�q�l�u��H");
//                         eTaskDialogResult result = CaxMsg.ShowMsgYesNo(dialogText, eTaskDialogIcon.Help, "�[�u���˴��ϩ|���إ�");
//                         if (result == eTaskDialogResult.Yes)
//                         {
//                             afterCNCName = "";
//                         }
//                         if (result == eTaskDialogResult.No)
//                         {
//                             theProgram.Dispose();
//                             return retValue;
//                         }
//                     }
//                 }
//             }
//             else if (config.companyName.ToUpper() == "COXON")
//             {
//                 //�T�{�O�_���إ߸˧��� 2014/01/06
//                 int drawingNum = 0;
//                 string drawingNam = "";
//                 Tag[] drawings;
//                 bool chk_drawing_name = false;
//                 theUfSession.Draw.AskDrawings(out drawingNum, out drawings);
//                 for (int i = 0; i < drawingNum; i++)
//                 {
//                     theUfSession.Obj.AskName(drawings[i], out drawingNam);
//                     if (drawingNam.ToUpper() == section_face.ToUpper())
//                     {
//                         chk_drawing_name = true;
//                     }
//                 }
//                 if (!chk_drawing_name)
//                 {
//                     UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ERROR,�|���إ� " + section_face + " �˧���");
//                     theProgram.Dispose();
//                     return retValue;
//                 }
//             }
//             #endregion
// 
//             #region ���o�ե߬[�c
// 
//             Part displayPart = theSession.Parts.Display;
// 
//             //���o�ե߬[�c
//             int err;
//             List<CaxAsm.CompPart> AsmCompAry;
//             err = CaxAsm.GetAsmCompTree(out AsmCompAry);
//             if (err != 0)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "�ե߬[�cŪ������...");
//                 theProgram.Dispose();
//                 return retValue;
//             }
//             CaxAsm.CimAsmCompPart sCimAsmCompPart;
//             status = CaxAsm.GetCimAsmCompStruct(AsmCompAry, out sCimAsmCompPart);
//             if (!status)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "�ե߬[�cŪ������...");
//                 theProgram.Dispose();
//                 return retValue;
//             }
//             if (sCimAsmCompPart.design.comp == null)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "�]�p�s��Ū������...");
//                 theProgram.Dispose();
//                 return retValue;
//             }
//             if (sCimAsmCompPart.fixture.comp == null)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "�v��s��Ū������...");
//                 theProgram.Dispose();
//                 return retValue;
//             }
// 
//             Body designBody = null;
//             status = CaxPart.GetLayerBody(sCimAsmCompPart.design.part, out designBody);
//             if (!status)
//             {
//                 theProgram.Dispose();
//                 return retValue;
//             }
// 
//             #endregion
// 
//             #region ���o�]�p�s�󪺤ؤo�j�p
// 
//             //���o�]�p�s�󪺤ؤo�j�p
//             Body designBodyOcc = null;
//             CaxTransType.BodyPrototypeToNXOpenOcc(sCimAsmCompPart.design.comp.Tag, designBody.Tag, out designBodyOcc);
// 
//             double[] minDesignBodyWcs;
//             double[] maxDesignBodyWcs;
//             CaxPart.AskBoundingBoxExactByWCS(designBodyOcc.Tag, out minDesignBodyWcs, out maxDesignBodyWcs);
// 
//             #endregion
// 
//             #region ���o�]�p�s�󪺰�ǭ�
// 
//             //���o�]�p�s�󪺰�ǭ�
//             List<CaxPart.BaseCorner> baseCornerAry = new List<CaxPart.BaseCorner>();
//             status = CaxPart.GetBaseCornerFaceAry(sCimAsmCompPart.design.comp, out baseCornerAry);
//             if (!status)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "��Ǩ����o���~...");
//                 theProgram.Dispose();
//                 return retValue;
//             }
// 
//             #endregion
// 
//             #region ���o�v�������A�P�_�O�_���h�v��\��B�O�_���h�v��
//             //���o�v������
//             string fixture_type = "";
//             string attr_value = "";
//             // �P�_�O�_���h�v��
//             bool isMultiFuixture = false;
//             if (config.hasMultiFixture == "1")
//             {
//                 // ���h�v��\��
//                 int fixtureNo = 0;
//                 for (int i = 0; i < AsmCompAry.Count; i++)
//                 {
//                     try
//                     {
//                         string attr = AsmCompAry[i].componentOcc.GetStringAttribute(CaxDefineParam.ATTR_CIM_TYPE);
//                         if (attr == "FIXTURE")
//                         {
//                             fixtureNo++;
//                         }
//                     }
//                     catch (System.Exception ex)
//                     {
//                         continue;
//                     }
//                 }
//                 if (fixtureNo > 1)
//                 {
//                     isMultiFuixture = true;
//                     attr_value = "�h�v��";
//                 }
//                 else
//                 {
//                     try
//                     {
//                         attr_value = sCimAsmCompPart.fixture.comp.GetStringAttribute("FIXTURE_TYPE");
//                     }
//                     catch (System.Exception ex)
//                     {
//                         attr_value = "";
//                     }
//                 }
//             }
//             else
//             {
//                 // �S���h�v��\��
//                 try
//                 {
//                     attr_value = sCimAsmCompPart.fixture.comp.GetStringAttribute("FIXTURE_TYPE");
//                 }
//                 catch (System.Exception ex)
//                 {
//                     attr_value = "";
//                 }
//             }
//             fixture_type = attr_value;
// 
//             if (fixture_type == "")
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "�Х�����˧��v��...");
//                 theProgram.Dispose();
//                 return retValue;             
//             }
// 
//             #endregion
// 
// 
// 
//             //1.���oopration���W�ٰ}�C 
//             //2.���o�̤j���d����
//             //3.���opart offset (Gap)
//             List<ListToolLengeh> ListToolLengehAry = new List<ListToolLengeh>();
//             status = GetOperationData(section_face, cToolCuttingLength, sMesDatData, out ListToolLengehAry);
//             if (!status)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "�M����|��T���o���~...");
//                 theProgram.Dispose();
//                 return retValue;
//             }
// 
// 
//             //1.���o�u��T����BO��Z�b�������Z��
//             //2.��Ǩ�������y�Э��I���Z��
//             //3.��Ǩ�����(�Z�����I��������)��y�Э��I���Z��
//             //4.��Ǩ��u��(�Z�����I���u����)��y�Э��I���Z��
//             ExportWorkTabel sExportWorkTabel;
//             status = GetCsysToRefFaceData(sCimAsmCompPart, out sExportWorkTabel);
//             if (!status)
//             {
//                 theProgram.Dispose();
//                 return retValue;
//             }
// 
//             //���o�q���󸹪�Note
//             BaseNote elecPartNoNote = null;
//             string baseNoteValue = "";
//             BaseNote[] noteAry = displayPart.Notes.ToArray();
//             for (int i = 0; i < noteAry.Length;i++ )
//             {
//                 baseNoteValue = "";
// 
//                 try
//                 {
//                     baseNoteValue = noteAry[i].GetStringAttribute("ELEC_PART_NO");
//                 }
//                 catch (System.Exception ex)
//                 {
//                     baseNoteValue = "";
//                 }
//                 if (baseNoteValue == "1")
//                 {
//                     elecPartNoNote = noteAry[i];
//                 }
//             }
//             
#endregion

//             Application.EnableVisualStyles();
// 
//             eTbale eworkTableDlg = new eTbale();
//             eworkTableDlg.labelXWorkSection.Text = eTableWorkInit.section_face;
//             eworkTableDlg.asmName = eTableWorkInit.asmName;
//             eworkTableDlg.ROOT_PATH = eTableWorkInit.rootPath;
//             eworkTableDlg.sMesDatData = eTableWorkInit.sMesDatData;
//             eworkTableDlg.labelXFixture.Text = eTableWorkInit.fixture_type;
//             eworkTableDlg.sCimAsmCompPart = eTableWorkInit.sCimAsmCompPart;
//             eworkTableDlg.maxDesignBodyWcs = eTableWorkInit.maxDesignBodyWcs;
//             eworkTableDlg.minDesignBodyWcs = eTableWorkInit.minDesignBodyWcs;
//             eworkTableDlg.baseCornerAry = new List<CaxPart.BaseCorner>();
//             eworkTableDlg.baseCornerAry.AddRange(eTableWorkInit.baseCornerAry);
// 
//             eworkTableDlg.support_machine = SUPPORT_MACHINE;
//             eworkTableDlg.machine_type = MACHINE_TYPE;
//             eworkTableDlg.controller = CONTROLLER;
//             eworkTableDlg.machine_group = eTableWorkInit.sMesDatData.MAC_POST_TYPE;
//             eworkTableDlg.postFunction = eTableWorkInit.postFunctionName;
//             eworkTableDlg.sExportWorkTabel = eTableWorkInit.sExportWorkTabel;
//             eworkTableDlg.BOTTOM_Z = eTableWorkInit.minDesignBodyWcs[2];
//             eworkTableDlg.section_face = eTableWorkInit.section_face;
//             eworkTableDlg.elecPartNoNote = eTableWorkInit.elecPartNoNote;
//             // 20150525 Stewart
//             eworkTableDlg.baseHoleName = eTableWorkInit.baseHoleName;
//             eworkTableDlg.beforeCNCName = eTableWorkInit.beforeCNCName;
//             eworkTableDlg.afterCNCName = eTableWorkInit.afterCNCName;
//             eworkTableDlg.isMultiFixture = eTableWorkInit.isMultiFixture;
//             eworkTableDlg.fixtureLst = eTableWorkInit.fixtureLst;
//             eworkTableDlg.subDesignCompLst = eTableWorkInit.subDesignCompLst;
//             // 20150721 �w������
//             eworkTableDlg.CLEARANE_PLANE = eTableWorkInit.CLEARANE_PLANE;
//             // 20150817 �˧��ϤW����ǭ��Z��
//             eworkTableDlg.sBaseDist = eTableWorkInit.sBaseDist;
//             // �ǤJ�t�m�ɸ�T
//             eworkTableDlg.companyName = eTableWorkInit.config.companyName;
//             eworkTableDlg.hasCMM = (eTableWorkInit.config.hasCMM == "1");
//             eworkTableDlg.hasMultiFixture = eTableWorkInit.hasMultiFixture;
// 
//             eworkTableDlg.ListToolLengehAry = new List<ListToolLengeh>();
//             eworkTableDlg.ListToolLengehAry.AddRange(eTableWorkInit.ListToolLengehAry);
// 
//             //�}�ҹ�ܮ�
//             FormUtilities.ReparentForm(eworkTableDlg);
//             System.Windows.Forms.Application.Run(eworkTableDlg);
//             eworkTableDlg.Dispose();

            theProgram.Dispose();
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----

        }
        return retValue;
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

    #region stewart hide 20150529


//     /// <summary>
//     /// 1.���o�u��T����BO��Z�b�������Z��
//     /// 2.��Ǩ�������y�Э��I���Z��
//     /// 3.��Ǩ�����(�Z�����I��������)��y�Э��I���Z��
//     /// 4.��Ǩ��u��(�Z�����I���u����)��y�Э��I���Z��
//     /// </summary>
//     /// <param name="sCimAsmCompPart"></param>
//     /// <returns></returns>
//     public static bool GetCsysToRefFaceData(CaxAsm.CimAsmCompPart sCimAsmCompPart, out ExportWorkTabel sExportWorkTabel)
//     {
//         sExportWorkTabel.X_OFFSET = "";
//         sExportWorkTabel.Y_OFFSET = "";
//         sExportWorkTabel.Z_BASE = "";
//         sExportWorkTabel.Z_MOVE = "";
// 
//         try
//         {
//             Part displayPart = theSession.Parts.Display;
//             //ExportWorkTabel sExportWorkTabel;
//             string attr_value = "";
// 
// 
// 
//             //Z_MOVE
//             try
//             {
//                 attr_value = "";
//                 attr_value = displayPart.GetStringAttribute(CaxDefineParam.ATTR_CIM_Z_MOVE);
//             }
//             catch (System.Exception ex)
//             {
//                 attr_value = "";
//             }
//             if (attr_value == "")
//             {
//                 try
//                 {
//                     attr_value = "";
//                     attr_value = sCimAsmCompPart.design.part.GetStringAttribute(CaxDefineParam.ATTR_CIM_Z_MOVE);
//                 }
//                 catch (System.Exception ex)
//                 {
//                     attr_value = "";
//                 }
//             }
//             sExportWorkTabel.Z_MOVE = attr_value;
// 
//             //X_OFFSET
//             try
//             {
//                 attr_value = "";
//                 attr_value = displayPart.GetStringAttribute(CaxDefineParam.ATTR_CIM_FACE_X_OFFSET);
//             }
//             catch (System.Exception ex)
//             {
//                 attr_value = "";
//             }
//             if (attr_value == "")
//             {
//                 try
//                 {
//                     attr_value = "";
//                     attr_value = sCimAsmCompPart.design.part.GetStringAttribute(CaxDefineParam.ATTR_CIM_FACE_X_OFFSET);
//                 }
//                 catch (System.Exception ex)
//                 {
//                     attr_value = "";
//                 }
//             }
//             sExportWorkTabel.X_OFFSET = attr_value;
// 
//             //Y_OFFSET
//             try
//             {
//                 attr_value = "";
//                 attr_value = displayPart.GetStringAttribute(CaxDefineParam.ATTR_CIM_FACE_Y_OFFSET);
//             }
//             catch (System.Exception ex)
//             {
//                 attr_value = "";
//             }
//             if (attr_value == "")
//             {
//                 try
//                 {
//                     attr_value = "";
//                     attr_value = sCimAsmCompPart.design.part.GetStringAttribute(CaxDefineParam.ATTR_CIM_FACE_Y_OFFSET);
//                 }
//                 catch (System.Exception ex)
//                 {
//                     attr_value = "";
//                 }
//             }
//             sExportWorkTabel.Y_OFFSET = attr_value;
// 
//             //Z_BASE
//             try
//             {
//                 attr_value = "";
//                 attr_value = displayPart.GetStringAttribute(CaxDefineParam.ATTR_CIM_Z_BASE);
//             }
//             catch (System.Exception ex)
//             {
//                 attr_value = "";
//             }
//             if (attr_value == "")
//             {
//                 try
//                 {
//                     attr_value = "";
//                     attr_value = sCimAsmCompPart.design.part.GetStringAttribute(CaxDefineParam.ATTR_CIM_Z_BASE);
//                 }
//                 catch (System.Exception ex)
//                 {
//                     attr_value = "";
//                 }
//             }
//             sExportWorkTabel.Z_BASE = attr_value;
// 
//         }
//         catch (System.Exception ex)
//         {
//             return false;
//         }
//         return true;
//     }
// 
//     public static void GetToolData(string tool_name)
//     {
//         Session theSession = Session.GetSession();
//         Part workPart = theSession.Parts.Work;
//         Part displayPart = theSession.Parts.Display;
//         NXOpen.Session.UndoMarkId markId1;
//         markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Edit SBL6R3-20-2-6-6_BBT30-MEGA-8N-90_25");
// 
//         NXOpen.Session.UndoMarkId markId2;
//         markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");
// 
//         NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)workPart.CAMSetup.CAMGroupCollection.FindObject(tool_name);
// 
//         NXOpen.CAM.MillToolBuilder millToolBuilder1;
//         millToolBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillToolBuilder(tool1);
// 
//         theSession.SetUndoMarkName(markId2, "No Description Dialog");
// 
//         CaxLog.ShowListingWindow(millToolBuilder1.TlDiameterBuilder.Value.ToString());
//         CaxLog.ShowListingWindow(millToolBuilder1.TlCor1RadBuilder.Value.ToString());
// 
//         CaxLog.ShowListingWindow("TlTaperAngBuilder : " + millToolBuilder1.TlTaperAngBuilder.Value.ToString());
//         CaxLog.ShowListingWindow("TlTipAngBuilder : " + millToolBuilder1.TlTipAngBuilder.Value.ToString());
// 
//         CaxLog.ShowListingWindow("TlHeightBuilder : " + millToolBuilder1.TlHeightBuilder.Value.ToString());
//         CaxLog.ShowListingWindow("TlFluteLnBuilder : " + millToolBuilder1.TlFluteLnBuilder.Value.ToString());
//         CaxLog.ShowListingWindow("TlNumFlutesBuilder : " + millToolBuilder1.TlNumFlutesBuilder.Value.ToString());
// 
//         //CaxLog.ShowListingWindow("TlTaperAngBuilder : " + millToolBuilder1.tlu);
//         //CaxLog.ShowListingWindow("TlTipAngBuilder : " + millToolBuilder1.TlTipAngBuilder.Value.ToString());
//         
// 
//         CaxLog.ShowListingWindow("TaperedShankDiameterBuilder : " + millToolBuilder1.TaperedShankDiameterBuilder.Value.ToString());
//         CaxLog.ShowListingWindow("TaperedShankLengthBuilder : " + millToolBuilder1.TaperedShankLengthBuilder.Value.ToString());
//         CaxLog.ShowListingWindow("TaperedShankTaperLengthBuilder : " + millToolBuilder1.TaperedShankTaperLengthBuilder.Value.ToString());
// 
//         CaxLog.ShowListingWindow("TlHolderLibref : " + millToolBuilder1.TlHolderLibref.ToString());
// 
//         Tool.Types type;
//         Tool.Subtypes subtype;
//         tool1.GetTypeAndSubtype(out type, out subtype);
// 
//         CaxLog.ShowListingWindow("type : " + type.ToString());
//         CaxLog.ShowListingWindow("subtype : " + subtype.ToString());
// 
//         NXOpen.CAM.TToolBuilder tToolBuilder1;
//         tToolBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateTToolBuilder(tool1);
// 
//         theSession.SetUndoMarkName(markId2, "Milling Tool-T Cutter Dialog");
// 
// 
// 
// //         tToolBuilder1.TlLowCorRadBuilder.Value = 1.0;
// // 
// //         tToolBuilder1.TlUpCorRadBuilder.Value = 2.0;
// 
//         CaxLog.ShowListingWindow("NumberOfSections : " + millToolBuilder1.HolderSectionBuilder.NumberOfSections.ToString());
// 
//         for (int i = 0; i < millToolBuilder1.HolderSectionBuilder.NumberOfSections;i++ )
//         {
//         CaxLog.ShowListingWindow(i.ToString() );
//             NXObject holderDataObj = millToolBuilder1.HolderSectionBuilder.GetSection(i);
// 
//             double lowerDiameter;
//             double length;
//             double taperAngle;
//             double upperDiameter;
//             double cornerRadius;
// 
//             millToolBuilder1.HolderSectionBuilder.GetAllParameters(holderDataObj, out lowerDiameter, out length, out taperAngle, out upperDiameter, out cornerRadius);
// 
//             CaxLog.ShowListingWindow("i : " + i.ToString());
//             CaxLog.ShowListingWindow("lowerDiameter : " + lowerDiameter.ToString());
//             CaxLog.ShowListingWindow("length : " + length.ToString());
//             CaxLog.ShowListingWindow("taperAngle : " + taperAngle.ToString());
//             CaxLog.ShowListingWindow("upperDiameter : " + upperDiameter.ToString());
//             CaxLog.ShowListingWindow("cornerRadius : " + cornerRadius.ToString());
// 
//         }
//         CaxLog.ShowListingWindow("Done ");
//        
//         
//         //millToolBuilder1.TlDiameterBuilder.Value = 7.0;
// 
//         //millToolBuilder1.TlCor1RadBuilder.Value = 4.0;
// 
//         NXOpen.Session.UndoMarkId markId3;
//         markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "No Description");
// 
//         theSession.DeleteUndoMark(markId3, null);
// 
//         NXOpen.Session.UndoMarkId markId4;
//         markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "No Description");
// 
//         NXObject nXObject1;
//         try
//         {
//             // Ramp Angle must be greater than zero.
//             nXObject1 = millToolBuilder1.Commit();
//         }
//         catch (NXException ex)
//         {
//             ex.AssertErrorCode(3776149);
//         }
// 
//         theSession.UndoToMarkWithStatus(markId4, null);
// 
//         theSession.DeleteUndoMark(markId4, null);
// 
//         millToolBuilder1.Destroy();
// 
//         theSession.UndoToMark(markId2, null);
// 
//         theSession.DeleteUndoMark(markId2, null);
// 
//         theSession.DeleteUndoMark(markId2, null);
// 
//         theSession.UndoToMark(markId1, "Edit SBL6R3-20-2-6-6_BBT30-MEGA-8N-90_25");
// 
//         theSession.DeleteUndoMarksUpToMark(markId1, "Edit SBL6R3-20-2-6-6_BBT30-MEGA-8N-90_25", false);
// 
//     }
// 
//     public static bool GetOperationData(string section_face, CimforceCaxTwCNC.ToolCuttingLength cToolCuttingLength, CimforceCaxTwCNC.MesAttrCNC sMesDatData, out List<ListToolLengeh> ListToolLengehAry)
//     {
//         ListToolLengehAry = new List<ListToolLengeh>();
// 
//         try
//         {
//             //Issue #9630
//             // �M��ةR�������q
//             // 1�A���operation�W�X�M��ةR
//             // 2�A�P�@��M�Ҧ���operation�W�X�M��ةR
//             //  ���e�]���˱󦡤M��S���W�A�]�S���`�N,CAX�q��o��ӥ�����F
//             // �{�b�ڦb���ձ˱󦡤M��A�N���o�{���D�A�]���˱󦡤M��O�i�H�󴫤M�����A�ҥH�{�b���ݨD�ݭn��U
//             // 
//             // ��O���馡�M��A�N���{�b���W�h�]1�M2�����n�����^
//             // �p�G�O�˱󦡤M��A�ݭn�ק令
//             // 1�A���OPERATION�W�X�M��ةR �o���٬O����
//             // 2�A�P�@��M�Ҧ���OPERATION�W�X�M��ةR �o�����ݭn����]�]�N�O�٬O�i�H�X�u��^
// 
//             bool status;
// 
//             //���oopration���W�ٰ}�C
//             //ArrayList operationAry = new ArrayList();
//             Part workPart = theSession.Parts.Work;
//             Part dispPart = theSession.Parts.Display;
//             //string workFace_name = "";
// 
//             try
//             {
//                 if (dispPart.CAMSetup == null)
//                 {
//                     UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ERROR,�Х��إ�Operation.");
//                     return false;
//                 }
//             }
//             catch (System.Exception ex)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ERROR,�Х��إ�Operation.");
//                 return false;
//             }
// 
//             //���oOperation Name
//             OperationCollection Operations = dispPart.CAMSetup.CAMOperationCollection;
//             Operation[] OperationAry = Operations.ToArray();
//             if (OperationAry.Length == 0)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ERROR,�Х��إ�Operation.");
//                 return false;
//             }
// 
// 
//             //�P�_Program Order Group �W�ٻP�u�q�O�_�ۦP 2014/01/06
//             NCGroup operationGroup;
//             bool chk_faca_name = false;
//             double max_length = 0.0;
//             List<ToolLengehStatus> toolStatusAry = new List<ToolLengehStatus>();
// 
//             ListToolLengeh sListToolLengeh;
//             sListToolLengeh.oper = null;
//             sListToolLengeh.isOK = false;
//             sListToolLengeh.isOverToolLength = false;
//             sListToolLengeh.tool_name = "";
//             sListToolLengeh.tool_ext_length = "";
//             sListToolLengeh.oper_name = "";
//             sListToolLengeh.cutting_length = 0.0;
//             sListToolLengeh.cutting_length_max = 0.0;
//             sListToolLengeh.part_offset = 0.0;
// 
//             ToolLengehStatus sToolLengehStatus;
//             sToolLengehStatus.tool_name = "";
//             sToolLengehStatus.tool_ext_length = "";
//             sToolLengehStatus.cutting_length_max = 0.0;
// 
//             //�P�_�O�_���q��
//             if (sMesDatData.PART_TYPE_ID == "5")
//             {
//                 //�q��
// 
//                 for (int a = 0; a < sMesDatData.ED_PARTS.Count;a++ )
//                 {
//                     for (int i = 0; i < OperationAry.Length; i++)
//                     {
//                         operationGroup = OperationAry[i].GetParent(CAMSetup.View.ProgramOrder);
//                         //if (section_face.ToUpper() == operationGroup.Name.ToUpper())
//                         //{
//                         chk_faca_name = true;
// 
//                         /*
//                         double gap = sMesDatData.ED_PARTS[a].DISCHARGE_GAP;
//                         if (gap > 0)
//                         {
//                             gap = gap * (-1);
//                         }
// 
//                         status = CaxCAM.SetPartOffset(gap, "WORKPIECE_EL_AREA");
//                         if (!status)
//                         {
//                             UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, OperationAry[i].Name + " Part Offset �]�w����...");
//                             return false;
//                         }
//                         */
// 
//                         //Type operType = OperationAry[i].GetType();
//                         //CaxLog.ShowListingWindow("OPERATION_TYPE : " + operType.Name.ToString());
// 
//                         //CaxLog.ShowListingWindow("GetStatus : " + OperationAry[i].GetStatus().ToString());
//                         if (OperationAry[i].GetStatus() != CAMObject.Status.Complete && OperationAry[i].GetStatus() != CAMObject.Status.Repost)
//                         {
//                             status = CaxCAM.GenerateToolPath(OperationAry[i].Name);
//                             if (!status)
//                             {
//                                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, OperationAry[i].Name + " Generate ����...");
//                                 return false;
//                             }
//                         }
// 
//                         //CaxLog.ShowListingWindow("ToString : " + OperationAry[i].ToString());
// 
//                         //���ooperation
//                         sListToolLengeh.oper = OperationAry[i];
// 
//                         //���ooperation�W��
//                         sListToolLengeh.oper_name = OperationAry[i].Name;
// 
//                         //���o�M��GROUP
//                         operationGroup = OperationAry[i].GetParent(CAMSetup.View.MachineTool);
// 
// 
//                         //CaxLog.ShowListingWindow("TOOL_NAME : " + operationGroup.Name.ToString());
// 
//                         //GetToolData(operationGroup.Name);
// 
//                         //���o�M��W��
//                         string[] MachineToolNameSplitAry = operationGroup.Name.Split('_');
//                         if (MachineToolNameSplitAry.Length < 3)
//                         {
//                             CaxLog.ShowListingWindow("�M��W�ٿ��~ : " + operationGroup.Name);
//                             return false;
//                         }
//                         sListToolLengeh.tool_name = MachineToolNameSplitAry[0];
// 
//                         sListToolLengeh.tool_ext_length = MachineToolNameSplitAry[2];
// 
//                         //���o���d����
//                         sListToolLengeh.cutting_length = OperationAry[i].GetToolpathCuttingLength();
//                         //sListToolLengeh.cutting_length = 170000;
// 
//                         //���opart offset (Gap)
//                         sListToolLengeh.part_offset = sMesDatData.ED_PARTS[a].DISCHARGE_GAP;
//                         //CaxLog.ShowListingWindow(sMesDatData.ED_PARTS[a].DISCHARGE_GAP.ToString());
// 
//                         //�P�_���d���׬O�_�L��
//                         max_length = 0.0;
//                         sListToolLengeh.isOK = false;
//                         sListToolLengeh.cutting_length_max = 0.0;
//                         for (int j = 0; j < cToolCuttingLength.data.Count; j++)
//                         {
//                             if (cToolCuttingLength.data[j].TOOL_STD_ID.ToUpper() == sListToolLengeh.tool_name.ToUpper())
//                             {
//                                 for (int k = 0; k < cToolCuttingLength.data[j].HRC_ARRAY.Count; k++)
//                                 {
//                                     if (cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER > max_length)
//                                     {
//                                         max_length = cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER;
//                                     }
// 
//                                     if ((cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER * 1000) > sListToolLengeh.cutting_length)
//                                     {
//                                         sListToolLengeh.isOK = true;
//                                         goto GOTO_ISOK;
//                                     }
//                                 }
//                             }
//                         }
// 
//                     GOTO_ISOK:
// 
//                         //���o�̤j���d���� (MES��쬰m�A�ҥH�n*1000 = mm)
//                         sListToolLengeh.cutting_length_max = max_length * 1000;
// 
//                         bool chk_tool_name = false;
//                         for (int j = 0; j < toolStatusAry.Count; j++)
//                         {
//                             if (sListToolLengeh.tool_name == toolStatusAry[j].tool_name)
//                             {
//                                 chk_tool_name = true;
//                             }
//                         }
//                         if (!chk_tool_name)
//                         {
//                             //sToolLengehStatus.isOverToolLength = false;
//                             sToolLengehStatus.tool_name = sListToolLengeh.tool_name;
//                             sToolLengehStatus.tool_ext_length = sListToolLengeh.tool_ext_length;
//                             sToolLengehStatus.cutting_length_max = sListToolLengeh.cutting_length_max;
//                             toolStatusAry.Add(sToolLengehStatus);
//                         }
// 
//                         ListToolLengehAry.Add(sListToolLengeh);
//                         continue;
//                         //}
// 
//                     }
//                 }
//             }
//             else
//             {
//                 //�D�q��(�u��)
// 
//                 for (int i = 0; i < OperationAry.Length; i++)
//                 {
//                     sListToolLengeh.oper = null;
//                     sListToolLengeh.isOK = false;
//                     sListToolLengeh.isOverToolLength = false;
//                     sListToolLengeh.tool_name = "";
//                     sListToolLengeh.tool_ext_length = "";
//                     sListToolLengeh.oper_name = "";
//                     sListToolLengeh.cutting_length = 0.0;
//                     sListToolLengeh.cutting_length_max = 0.0;
//                     sListToolLengeh.part_offset = 0.0;
// 
//                     operationGroup = OperationAry[i].GetParent(CAMSetup.View.ProgramOrder);
//                     if (section_face.ToUpper() == operationGroup.Name.ToUpper())
//                     {
//                         chk_faca_name = true;
// 
//                         //Type operType = OperationAry[i].GetType();
//                         //CaxLog.ShowListingWindow("OPERATION_TYPE : " + operType.Name.ToString());
// 
//                         //CaxLog.ShowListingWindow("GetStatus : " + OperationAry[i].GetStatus().ToString());
//                         if (OperationAry[i].GetStatus() != CAMObject.Status.Complete && OperationAry[i].GetStatus() != CAMObject.Status.Repost)
//                         {
//                             status = CaxCAM.GenerateToolPath(OperationAry[i].Name);
//                             if (!status)
//                             {
//                                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, OperationAry[i].Name + " Generate ����...");
//                                 return false;
//                             }
//                         }
// 
//                         //CaxLog.ShowListingWindow("ToString : " + OperationAry[i].ToString());
// 
// 
//                         //���ooperation
//                         sListToolLengeh.oper = OperationAry[i];
// 
//                         //���ooperation�W��
//                         sListToolLengeh.oper_name = OperationAry[i].Name;
// 
//                         //���o�M��GROUP
//                         operationGroup = OperationAry[i].GetParent(CAMSetup.View.MachineTool);
// 
// 
//                         //CaxLog.ShowListingWindow("TOOL_NAME : " + operationGroup.Name.ToString());
// 
//                         //GetToolData(operationGroup.Name);
// 
//                         //���o�M��W��
//                         string[] MachineToolNameSplitAry = operationGroup.Name.Split('_');
//                         if (MachineToolNameSplitAry.Length < 3)
//                         {
//                             CaxLog.ShowListingWindow("�M��W�ٿ��~ : " + operationGroup.Name);
//                             return false;
//                         }
// 
//                         sListToolLengeh.tool_name = MachineToolNameSplitAry[0];
//                         sListToolLengeh.tool_ext_length = MachineToolNameSplitAry[2];
// 
//                         //CaxLog.ShowListingWindow("tool_ext_length : " + sListToolLengeh.tool_ext_length);
// 
//                         //���o���d����
//                         sListToolLengeh.cutting_length = OperationAry[i].GetToolpathCuttingLength();
// 
//                         //���opart offset (Gap)
//                         double part_offset = 0;
//                         CaxCAM.AskPartOffset(out part_offset);
//                         sListToolLengeh.part_offset = part_offset;
// 
//                         //�P�_���d���׬O�_�L��
//                         max_length = 0.0;
//                         sListToolLengeh.isOK = false;
//                         sListToolLengeh.cutting_length_max = 0.0;
//                         for (int j = 0; j < cToolCuttingLength.data.Count; j++)
//                         {
//                             if (cToolCuttingLength.data[j].TOOL_STD_ID.ToUpper() == sListToolLengeh.tool_name.ToUpper())
//                             {
//                                 for (int k = 0; k < cToolCuttingLength.data[j].HRC_ARRAY.Count; k++)
//                                 {
//                                     if (cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER > max_length)
//                                     {
//                                         max_length = cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER;
//                                     }
// 
//                                     if ((cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER * 1000) > sListToolLengeh.cutting_length)
//                                     {
//                                         sListToolLengeh.isOK = true;
//                                         goto GOTO_ISOK;
//                                     }
//                                 }
//                             }
//                         }
// 
//                     GOTO_ISOK:
// 
//                         //���o�̤j���d���� (MES��쬰m�A�ҥH�n*1000 = mm)
//                         sListToolLengeh.cutting_length_max = max_length * 1000;
// 
//                         bool chk_tool_name = false;
//                         for (int j = 0; j < toolStatusAry.Count; j++)
//                         {
//                             if (sListToolLengeh.tool_name == toolStatusAry[j].tool_name)
//                             {
//                                 chk_tool_name = true;
//                             }
//                         }
//                         if (!chk_tool_name)
//                         {
//                             //sToolLengehStatus.isOverToolLength = false;
//                             sToolLengehStatus.tool_name = sListToolLengeh.tool_name;
//                             sToolLengehStatus.tool_ext_length = sListToolLengeh.tool_ext_length;//20150515 Andy�����o�����
//                             sToolLengehStatus.cutting_length_max = sListToolLengeh.cutting_length_max;
//                             toolStatusAry.Add(sToolLengehStatus);
//                         }
// 
//                         ListToolLengehAry.Add(sListToolLengeh);
//                         continue;
//                     }
// 
//                 }
// 
//             }
//             CaxPart.Refresh();
// 
// 
//             if (!chk_faca_name)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "Program Order ����� " + section_face + " Group!");
//                 return false;
//             }
// 
//             double totalOperLength = 0.0;
//             for (int i = 0; i < toolStatusAry.Count; i++)
//             {
//                 //20150123 
//                 // �M��ةR�������q
//                 // 1�A���operation�W�X�M��ةR
//                 // 2�A�P�@��M�Ҧ���operation�W�X�M��ةR
// 
//                 // ��O���馡�M��A�N���{�b���W�h�]1�M2�����n�����^
//                 // �p�G�O�˱󦡤M��A�ݭn�ק令
//                 // 1�A���OPERATION�W�X�M��ةR �o���٬O����
//                 // 2�A�P�@��M�Ҧ���OPERATION�W�X�M��ةR �o�����ݭn����]�]�N�O�٬O�i�H�X�u��^
// 
//                 if (toolStatusAry[i].tool_name.ToUpper().IndexOf("I") == 0)
//                 {
//                     //�˱󦡤M��
//                     CaxLog.WriteLog("�˱󦡤M�� : " + toolStatusAry[i].tool_name);
//                     continue;
//                 }
// 
//                 CaxLog.WriteLog("���馡�M�� : " + toolStatusAry[i].tool_name);
// 
//                 totalOperLength = 0.0;
//                 for (int j = 0; j < ListToolLengehAry.Count; j++)
//                 {
//                     if (toolStatusAry[i].tool_name == ListToolLengehAry[j].tool_name &&
//                         toolStatusAry[i].tool_ext_length == ListToolLengehAry[j].tool_ext_length)
//                     {
//                         totalOperLength += ListToolLengehAry[j].cutting_length;
//                     }
//                 }
//                 if (totalOperLength > toolStatusAry[i].cutting_length_max)
//                 {
//                     for (int j = 0; j < ListToolLengehAry.Count; j++)
//                     {
//                         if (toolStatusAry[i].tool_name == ListToolLengehAry[j].tool_name)
//                         {
//                             sListToolLengeh.oper = ListToolLengehAry[j].oper;
//                             sListToolLengeh.isOK = ListToolLengehAry[j].isOK;
//                             sListToolLengeh.isOverToolLength = true;
//                             sListToolLengeh.tool_name = ListToolLengehAry[j].tool_name;
//                             sListToolLengeh.oper_name = ListToolLengehAry[j].oper_name;
//                             sListToolLengeh.cutting_length = ListToolLengehAry[j].cutting_length;
//                             sListToolLengeh.cutting_length_max = ListToolLengehAry[j].cutting_length_max;
// 
//                             //2015-03-20 �s�W
//                             sListToolLengeh.part_offset = ListToolLengehAry[j].part_offset;
//                             sListToolLengeh.tool_ext_length = ListToolLengehAry[j].tool_ext_length;
// 
// 
//                             ListToolLengehAry[j] = sListToolLengeh;
//                         }
//                     }
//                 }
//             }
//         }
//         catch (System.Exception ex)
//         {
//             return false;
//         }
// 
//         return true;
//     }
// 
//     // Ū���t�m��
//     public static bool ReadJsonData(string JsonStyleLoadPath, out ConfigData JsonRead)
//     {
//         JsonRead = new ConfigData();
//         try
//         {
//             bool status;
//             // �ˬd�ɮ׬O�_�s�b
//             if (!System.IO.File.Exists(JsonStyleLoadPath))
//             {
//                 CaxLog.ShowListingWindow("�t�m�ɤ��s�b�G" + JsonStyleLoadPath);
//                 return false;
//             }
//             string jsonText;
//             status = CaxFile.ReadFileDataUTF8(JsonStyleLoadPath, out jsonText);
//             if (!status)
//             {
//                 CaxLog.ShowListingWindow("�t�m��Ū�����ѡG" + JsonStyleLoadPath);
//                 return false;
//             }
//             JsonRead = JsonConvert.DeserializeObject<ConfigData>(jsonText);
//         }
//         catch (System.Exception ex)
//         {
//             //CaxLog.ShowListingWindow(ex.Message);
//             return false;
//         }
//         return true;
//     }

    #endregion


}

