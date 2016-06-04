using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using NXOpen.UF;
using CimforceCaxTwCNC;
using CimforceCaxTwFixture;
using LicenseCheck;
using CimforceCaxTwPublic;
using System.IO;
using DevComponents.DotNetBar;
using NXOpen.CAM;
using NXOpen.Annotations;
using NXOpen.Utilities;

namespace ETableWork
{
    class Class001_ETableInit
    {
        //↓↓↓↓↓↓↓↓↓↓宣告所有參數↓↓↓↓↓↓↓↓↓↓//
        private static Session theSession;
        private static UI theUI;
        private static UFSession theUfSession;
        private static Part displayPart;

//         private const string SUPPORT_MACHINE = "";
//         private const string MACHINE_TYPE = "";
//         private const string CONTROLLER = "";

        public string asmName;                  // 檔案名稱
        public string rootPath;                 // 檔案路徑
        public string postFunctionName;         // 可用的Post名稱
        public string section_face;             // 裝夾圖圖紙名稱
        public string baseHoleName;             // 放電基準孔圖紙名稱
        public string beforeCNCName;            // 加工前檢測圖圖紙名稱
        public string afterCNCName;             // 加工後檢測圖圖紙名稱
        public string fixture_type;             // 治具類型
        public string CLEARANE_PLANE;           // 安全高度 20150721

        // 讀取資料
        public ToolCuttingLength cToolCuttingLength;
        public MesAttrCNC sMesDatData;
        public ConfigData config;

        // 組立架構
        public List<CaxAsm.CompPart> AsmCompAry;
        public CaxAsm.CimAsmCompPart sCimAsmCompPart;
        public double[] minDesignBodyWcs;
        public double[] maxDesignBodyWcs;
//         public List<CaxPart.BaseCorner> baseCornerAry;
        public List<CaxAsm.CompPart> fixtureLst;
        public List<NXOpen.Assemblies.Component> subDesignCompLst;

        public bool hasMultiFixture;
        public bool isMultiFixture;
        public bool isASM;
        // operation...
        public List<ListToolLengeh> ListToolLengehAry;
        // 一些距離...
        public ExportWorkTabel sExportWorkTabel;
        // 電極件號
        public BaseNote elecPartNoNote;

        // 20150817 裝夾圖上的基準面距離
        public BaseDist sBaseDist;
        // 20150811 基準面資訊
        public RefData refData;
        public baseFace sBaseFaces;
        // 20151013 判斷是否重新裝夾/校正之參數
        public ClampCalibrateParam clampCalibParam;

        //↑↑↑↑↑↑↑↑↑↑宣告所有參數↑↑↑↑↑↑↑↑↑↑//
        // 參數初始化
        public Class001_ETableInit()
        {
            theSession = Session.GetSession();
            theUI = UI.GetUI();
            theUfSession = UFSession.GetUFSession();
            displayPart = theSession.Parts.Display;

            asmName = "";
            rootPath = "";
            postFunctionName = "";
            baseHoleName = "";
            beforeCNCName = "";
            afterCNCName = "";
            fixture_type = "";

            cToolCuttingLength = new ToolCuttingLength();
            sMesDatData = new MesAttrCNC();
            config = new ConfigData();

            AsmCompAry = new List<CaxAsm.CompPart>();
            sCimAsmCompPart = new CaxAsm.CimAsmCompPart();
//             baseCornerAry = new List<CaxPart.BaseCorner>();
            fixtureLst = new List<CaxAsm.CompPart>();
            subDesignCompLst = new List<NXOpen.Assemblies.Component>();

            hasMultiFixture = false;
            isMultiFixture = false;
            isASM = false;

            ListToolLengehAry = new List<ListToolLengeh>();
            sExportWorkTabel = new ExportWorkTabel();
            elecPartNoNote = null;

            sBaseDist = new BaseDist();
        }


        public int GetAllParameter()
        {
            bool status;
            int errorNum;
            // 開啟讀取對話框
            CaxLoadingDlg sCaxLoadingDlg = null;
            sCaxLoadingDlg = new CaxLoadingDlg();
            try
            {
                // 確認啟動環境
                errorNum = ConfirmAllEnv();
                if (errorNum < 0)
                {
                    if (errorNum != -1)
                    {
                        CaxLog.ShowListingWindow("確認啟動環境時發生未知錯誤...");
                    }
                    sCaxLoadingDlg.Stop();
                    return -1;
                }

                // 取得檔案名稱、路徑
                theUfSession.Part.AskPartName(displayPart.Tag, out asmName);
                rootPath = Path.GetDirectoryName(asmName);

                // 讀取資料
                errorNum = GetAllStructs();
                if (errorNum < 0)
                {
                    if (errorNum != -1)
                    {
                        CaxLog.ShowListingWindow("讀取資料時發生未知錯誤...");
                    }
                    sCaxLoadingDlg.Stop();
                    return -1;
                }

                // 確認確認是否有建立裝夾圖 (DEPO多三張圖)
                errorNum = ConfirmSheets();
                if (errorNum < 0)
                {
                    if (errorNum != -1)
                    {
                        CaxLog.ShowListingWindow("圖紙確認時發生未知錯誤...");
                    }
                    sCaxLoadingDlg.Stop();
                    return -1;
                }

                // 4. 取得組立架構
                errorNum = GetComponentTree();
                if (errorNum < 0)
                {
                    if (errorNum != -1)
                    {
                        CaxLog.ShowListingWindow("取得組立架構時發生未知錯誤...");
                    }
                    sCaxLoadingDlg.Stop();
                    return -1;
                }

                // 20150811 新增檢查是否需重新出裝夾圖
                // 取得當前基準面資訊
                status = GetRefData(sCimAsmCompPart.design.comp, out refData);
                if (!status)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "取得當前基準面資訊發生錯誤...");
                    return -1;
                }

                // 檢查裝夾圖屬性是否正確
                bool isRefDataMatch;
                errorNum = CheckRefData(sMesDatData, refData, out isRefDataMatch);
                if (errorNum < 0)
                {
                    if (errorNum != -1)
                    {
                        CaxLog.ShowListingWindow("檢查裝夾圖屬性發生未知錯誤...");
                    }
                    sCaxLoadingDlg.Stop();
                    return -1;
                }
                // 基準面資訊不一致
                if (!isRefDataMatch)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "裝夾圖版本錯誤，請重新輸出。");
                    sCaxLoadingDlg.Stop();
                    return -1;
                }

                // 20150817 取得裝夾圖上的基準面距離  
                status = GetBaseDist();
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "取得裝夾圖上的基準面距離時發生未知錯誤...");
                    return -1;
                }

                // 20151013 新增輸出判斷是否重新裝夾/校正之參數
                status = GetClampCalibrateParam(sCimAsmCompPart.design.comp, sCimAsmCompPart.fixture.comp, refData, out clampCalibParam);
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "取得判斷是否重新裝夾/校正之參數時發生未知錯誤...");
                    return -1;
                }

//                 // 20151102 檢查圖紙上的標註是否存在
//                 bool isNoteExist = false;
//                 // 裝夾圖圖紙名稱
//                 string sheetName = string.Format(@"{0}_{1}", sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID);
//                 status = CheckNoteExist(sheetName, out isNoteExist);
//                 if (!status)
//                 {
//                     sCaxLoadingDlg.Stop();
//                     UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "檢查圖紙上的標註是否存在時發生未知錯誤...");
//                     return -1;
//                 }
//                 if (!isNoteExist)
//                 {
//                     sCaxLoadingDlg.Stop();
//                     UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "裝夾圖資訊錯誤，請重新輸出再上傳。");
//                     return -1;
//                 }

                // 取得設計零件檔資訊
                errorNum = GetLayerBodyData();
                if (errorNum < 0)
                {
                    if (errorNum != -1)
                    {
                        CaxLog.ShowListingWindow("取得設計零件檔資訊時發生未知錯誤...");
                    }
                    sCaxLoadingDlg.Stop();
                    return -1;
                }

                // 取得治具類型，判斷是否有多治具功能、是否為多治具
                errorNum = GetFixtureData();
                if (errorNum < 0)
                {
                    if (errorNum != -1)
                    {
                        CaxLog.ShowListingWindow("取得治具資料時發生未知錯誤...");
                    }
                    sCaxLoadingDlg.Stop();
                    return -1;
                }

                // 判斷是否為組立加工，是則找出所有次主件
                status = DetermineASM();
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "判斷是否為組立加工時發生未知錯誤...");
                    return -1;
                }

                //1.取得opration的名稱陣列 
                //2.取得最大切削長度
                //3.取得part offset (Gap)
                status = GetOperationData();
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "刀具路徑資訊取得錯誤...");
                    return -1;
                }

                //1.取得工件T面轉BO面Z軸偏移的距離
                //2.基準角底面到座標原點的距離
                //3.基準角長面(距離原點較長的面)到座標原點的距離
                //4.基準角短面(距離原點較短的面)到座標原點的距離
                status = GetCsysToRefFaceData();
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "取得偏移距離錯誤...");
                    return -1;
                }

                // 20150721 取得安全高度
                status = GetSafetyHeight();
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "取得安全高度錯誤...");
                    return -1;
                }

                // 取得電極件號的Note
                status = GetElecPartNoNote();
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "取得電極件號錯誤...");
                    return -1;
                }

                // 關閉讀取對話框
                sCaxLoadingDlg.Stop();
            }
            catch (System.Exception ex)
            {
                sCaxLoadingDlg.Stop();
                return -1001;
            }
            return 0;

        }

        private int ConfirmAllEnv()
        {
            try
            {
                //確認啟動環境
                string license_status = "";
                license_status = License.ChkCimforceLicense();
                if (license_status != "CIMFORCE_LICENSE_SUCCESS")
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "License 錯誤!");
                    return -1;
                }

                Part displayPrt = theSession.Parts.Display;
                if (displayPrt == null)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ERROR,未開啟零件.");
                    return -1;
                }

                int module_id;
                theUfSession.UF.AskApplicationModule(out module_id);
                if (module_id != UFConstants.UF_APP_CAM)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "請先切換至 CAM 模組");
                    return -1;
                }
            }
            catch (System.Exception ex)
            {
                return -1001;
            }
            return 0;
        }

        private int GetAllStructs()
        {
            bool status;
            try
            {
                //取得mes2cam.dat路徑
                string mes2camDatPath = CaxCNC.GetMes2CamDatPath(displayPart);
                if (mes2camDatPath == "")
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "mes2cam.dat 讀取失敗");
                    return -1;
                }
                //取得mes2cam.dat資料
                if (!CaxCNC.ReadMesAttrCNCJsonData(mes2camDatPath, out sMesDatData))
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, mes2camDatPath + " 讀取失敗!");
                    return -1;
                }

                //取得刀具最大切削壽命(結構)
                status = CaxCNC.ReadToolCuttingLengthCNCJsonData(out cToolCuttingLength);
                if (!status || cToolCuttingLength == null)
                {
                    CaxLog.ShowListingWindow("MES 刀具壽命配置檔讀取失敗...");
                    return -1;
                }

                // 讀取配置檔(ETableConfig.txt)
                string JsonConfigFilePath = string.Format(@"{0}\Cimforce\CNC\config\{1}", CaxFile.GetCimforceEnvDir(), "ETableConfig.txt");
                status = ReadJsonData(JsonConfigFilePath, out config);
                if (!status)
                {
                    CaxLog.ShowListingWindow("讀取配置檔ETableConfig.txt時發生錯誤");
                    return -1;
                }

                //2014/04/09 新增機台類型對應POST↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓小伍提出修改
                //取得所有可用的Post名稱
                int count;
                string[] PostNames;
                theUfSession.Cam.OptAskPostNames(out count, out PostNames);

                postFunctionName = sMesDatData.MAC_POST_NM;
                if (postFunctionName == "")
                {
                    CaxLog.ShowListingWindow("機台類型對應的POST讀取錯誤...");
                    return -1;
                }

                for (int i = 0; i < PostNames.Length; i++)
                {
                    if (PostNames[i].ToUpper() == sMesDatData.MAC_POST_NM.ToUpper())
                    {
                        postFunctionName = PostNames[i];
                        break;
                    }
                }
                //2014/04/09 新增機台類型對應POST↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑小伍提出修改
            }
            catch (System.Exception ex)
            {
                return -1001;
            }
            return 0;
        }

        private int ConfirmSheets()
        {
            try
            {
                // 取得圖紙
                Tag[] drawings;
                int drawingNum = 0;
                theUfSession.Draw.AskDrawings(out drawingNum, out drawings);
                // DEPO
                if (config.companyName.ToUpper() == "DEPO")
                {
                    section_face = string.Format(@"{0}_{1}", sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID);
                    baseHoleName = string.Format(@"{0}_{1}_{2}", sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID, "BaseHole");
                    beforeCNCName = string.Format(@"{0}_{1}_{2}", sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID, "BeforeCNC");
                    afterCNCName = string.Format(@"{0}_{1}_{2}", sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID, "AfterCNC");
                    string drawingName = "";
                    bool chk_drafting_name = false;
                    bool chk_baseHole_name = false;
                    bool chk_beforeCNC_name = false;
                    bool chk_afterCNC_name = false;
                    for (int i = 0; i < drawingNum; i++)
                    {
                        theUfSession.Obj.AskName(drawings[i], out drawingName);
                        if (drawingName.ToUpper() == section_face.ToUpper())
                        {
                            chk_drafting_name = true;
                        }
                        if (drawingName.ToUpper() == baseHoleName.ToUpper())
                        {
                            chk_baseHole_name = true;
                        }
                        if (drawingName.ToUpper() == beforeCNCName.ToUpper())
                        {
                            chk_beforeCNC_name = true;
                        }
                        if (drawingName.ToUpper() == afterCNCName.ToUpper())
                        {
                            chk_afterCNC_name = true;
                        }
                    }
                    if (!chk_drafting_name)
                    {
                        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ERROR,尚未建立 " + section_face + " 裝夾圖");
                        return -1;
                    }
                    // 模仁才檢查這三張圖
                    if (sMesDatData.PART_TYPE_ID == "0" || sMesDatData.PART_TYPE_ID == "4")
                    {
                        if (!chk_baseHole_name)
                        {
                            string dialogText = string.Format("電極基準孔圖尚未建立，是否仍要出電子工單？");
                            eTaskDialogResult result = CaxMsg.ShowMsgYesNo(dialogText, eTaskDialogIcon.Help, "電極基準孔圖尚未建立");
                            if (result == eTaskDialogResult.Yes)
                            {
                                baseHoleName = "";
                            }
                            if (result == eTaskDialogResult.No)
                            {
                                return -1;
                            }
                        }
                        if (!chk_beforeCNC_name)
                        {
                            string dialogText = string.Format("加工前檢測圖尚未建立，是否仍要出電子工單？");
                            eTaskDialogResult result = CaxMsg.ShowMsgYesNo(dialogText, eTaskDialogIcon.Help, "加工前檢測圖尚未建立");
                            if (result == eTaskDialogResult.Yes)
                            {
                                beforeCNCName = "";
                            }
                            if (result == eTaskDialogResult.No)
                            {
                                return -1;
                            }
                        }
                        if (!chk_afterCNC_name)
                        {
                            string dialogText = string.Format("加工後檢測圖尚未建立，是否仍要出電子工單？");
                            eTaskDialogResult result = CaxMsg.ShowMsgYesNo(dialogText, eTaskDialogIcon.Help, "加工後檢測圖尚未建立");
                            if (result == eTaskDialogResult.Yes)
                            {
                                afterCNCName = "";
                            }
                            if (result == eTaskDialogResult.No)
                            {
                                return -1;
                            }
                        }
                    }
                    else
                    {
                        baseHoleName = "";
                        beforeCNCName = "";
                        afterCNCName = "";
                    }
                }
                else if (config.companyName.ToUpper() == "COXON")
                {
                    //確認是否有建立裝夾圖 2014/01/06
                    section_face = string.Format(@"{0}_{1}", sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID);
                    string drawingName = "";
                    bool chk_drawing_name = false;
                    for (int i = 0; i < drawingNum; i++)
                    {
                        theUfSession.Obj.AskName(drawings[i], out drawingName);
                        if (drawingName.ToUpper() == section_face.ToUpper())
                        {
                            chk_drawing_name = true;
                        }
                    }
                    if (!chk_drawing_name)
                    {
                        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ERROR,尚未建立 " + section_face + " 裝夾圖");
                        return -1;
                    }
                }
            }
            catch (System.Exception ex)
            {
                return -1001;
            }
            return 0;
        }

        // 20150812
        // 取得基準面資訊
        private bool GetRefData(NXOpen.Assemblies.Component component, out RefData refData)
        {
            refData = new RefData();
            refData.REF_A_HANDLE = "";
            refData.REF_B_HANDLE = "";
            refData.REF_C_HANDLE = "";
            refData.REF_D_HANDLE = "";
            refData.REF_X_HANDLE = "";
            refData.REF_Y_HANDLE = "";
            refData.REF_Z_HANDLE = "";
            refData.REF_A_BOX = "";
            refData.REF_B_BOX = "";
            refData.REF_C_BOX = "";
            refData.REF_D_BOX = "";
            try
            {
                Part compPart = (Part)component.Prototype;
                Body[] BodyAry = compPart.Bodies.ToArray();
                string attr_valueABCD = "";
                string attr_valueXYZ = "";
                Face baseFaceA = null;
                Face baseFaceB = null;
                Face baseFaceC = null;
                Face baseFaceD = null;
                List<Face> baseFaceX = new List<Face>();
                List<Face> baseFaceY = new List<Face>();
                List<Face> baseFaceZ = new List<Face>();
                for (int i = 0; i < BodyAry.Length; i++)
                {
                    // 20150720 搜基準面時排除不屬於layer 1 的面
                    if (BodyAry[i].Layer != 1)
                    {
                        continue;
                    }
                    Face[] bodyFaceAry = BodyAry[i].GetFaces();
                    for (int j = 0; j < bodyFaceAry.Length; j++)
                    {
                        // 找基準面屬性
                        attr_valueABCD = "";
                        try
                        {
                            //attr_value = bodyFaceAry[j].GetStringAttribute(CaxDefineParam.ATTR_CIM_TYPE);
                            attr_valueABCD = bodyFaceAry[j].GetStringAttribute(CaxDefineParam.ATTR_CIM_REF_FACE);
                            if (attr_valueABCD == "A")
                            {
                                baseFaceA = bodyFaceAry[j];
                            }
                            else if (attr_valueABCD == "B")
                            {
                                baseFaceB = bodyFaceAry[j];
                            }
                            else if (attr_valueABCD == "C")
                            {
                                baseFaceC = bodyFaceAry[j];
                            }
                            else if (attr_valueABCD == "D")
                            {
                                baseFaceD = bodyFaceAry[j];
                            }

                        }
                        catch (System.Exception ex)
                        {
                            attr_valueABCD = "";
                        }
                        // 20150617 找基準面偏移屬性
                        attr_valueXYZ = "";
                        try
                        {
                            attr_valueXYZ = bodyFaceAry[j].GetStringAttribute("CIM_REF");
                            if (attr_valueXYZ == "X")
                            {
                                baseFaceX.Add(bodyFaceAry[j]);
                            }
                            else if (attr_valueXYZ == "Y")
                            {
                                baseFaceY.Add(bodyFaceAry[j]);
                            }
                            else if (attr_valueXYZ == "Z")
                            {
                                baseFaceZ.Add(bodyFaceAry[j]);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            attr_valueXYZ = "";
                        }
                    }
                }
                Tag faceTagOcc;

                if (baseFaceA != null)
                {
                    // Face轉成Occurance
                    faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceA.Tag);
                    Face baseFaceOcc = (Face)NXObjectManager.Get(faceTagOcc);
                    // 找face的box
                    double[] minbodyOcc;
                    double[] maxbodyOcc;
                    CaxPart.AskBoundingBoxExactByWCS(baseFaceOcc.Tag, out minbodyOcc, out maxbodyOcc);
                    // 輸出box & handle
                    refData.REF_A_BOX = Math.Round(minbodyOcc[0], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(minbodyOcc[1], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(minbodyOcc[2], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(maxbodyOcc[0], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(maxbodyOcc[1], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(maxbodyOcc[2], 4, MidpointRounding.AwayFromZero).ToString("f2");
                    refData.REF_A_HANDLE = theUfSession.Tag.AskHandleOfTag(baseFaceA.Tag);
                }
                if (baseFaceB != null)
                {
                    // Face轉成Occurance
                    faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceB.Tag);
                    Face baseFaceOcc = (Face)NXObjectManager.Get(faceTagOcc);
                    // 找face的box
                    double[] minbodyOcc;
                    double[] maxbodyOcc;
                    CaxPart.AskBoundingBoxExactByWCS(baseFaceOcc.Tag, out minbodyOcc, out maxbodyOcc);
                    // 輸出box & handle
                    refData.REF_B_BOX = Math.Round(minbodyOcc[0], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(minbodyOcc[1], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(minbodyOcc[2], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(maxbodyOcc[0], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(maxbodyOcc[1], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(maxbodyOcc[2], 4, MidpointRounding.AwayFromZero).ToString("f2");
                    refData.REF_B_HANDLE = theUfSession.Tag.AskHandleOfTag(baseFaceB.Tag);
                }
                if (baseFaceC != null)
                {
                    // Face轉成Occurance
                    faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceC.Tag);
                    Face baseFaceOcc = (Face)NXObjectManager.Get(faceTagOcc);
                    // 找face的box
                    double[] minbodyOcc;
                    double[] maxbodyOcc;
                    CaxPart.AskBoundingBoxExactByWCS(baseFaceOcc.Tag, out minbodyOcc, out maxbodyOcc);
                    // 輸出box & handle
                    refData.REF_C_BOX = Math.Round(minbodyOcc[0], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(minbodyOcc[1], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(minbodyOcc[2], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(maxbodyOcc[0], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(maxbodyOcc[1], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(maxbodyOcc[2], 4, MidpointRounding.AwayFromZero).ToString("f2");
                    refData.REF_C_HANDLE = theUfSession.Tag.AskHandleOfTag(baseFaceC.Tag);
                }
                if (baseFaceD != null)
                {
                    // Face轉成Occurance
                    faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceD.Tag);
                    Face baseFaceOcc = (Face)NXObjectManager.Get(faceTagOcc);
                    // 找face的box
                    double[] minbodyOcc;
                    double[] maxbodyOcc;
                    CaxPart.AskBoundingBoxExactByWCS(baseFaceOcc.Tag, out minbodyOcc, out maxbodyOcc);
                    // 輸出box & handle
                    refData.REF_D_BOX = Math.Round(minbodyOcc[0], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(minbodyOcc[1], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(minbodyOcc[2], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(maxbodyOcc[0], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(maxbodyOcc[1], 4, MidpointRounding.AwayFromZero).ToString("f2") + ", "
                                      + Math.Round(maxbodyOcc[2], 4, MidpointRounding.AwayFromZero).ToString("f2");
                    refData.REF_D_HANDLE = theUfSession.Tag.AskHandleOfTag(baseFaceD.Tag);
                }
                // 20150617 將基準偏移面存入struct
                if (baseFaceX.Count != 0)
                {
                    List<string> tempHandleLst = new List<string>();
                    for (int i = 0; i < baseFaceX.Count; i++)
                    {
                        faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceX[i].Tag);
                        Face baseFaceOcc = (Face)NXObjectManager.Get(faceTagOcc);
                        string tempHandle = theUfSession.Tag.AskHandleOfTag(baseFaceX[i].Tag);
                        tempHandleLst.Add(tempHandle);
                    }
                    tempHandleLst.Sort();
                    foreach (string singleHandle in tempHandleLst)
                    {
                        refData.REF_X_HANDLE += (singleHandle + ", ");
                    }
                }
                if (baseFaceY.Count != 0)
                {
                    List<string> tempHandleLst = new List<string>();
                    for (int i = 0; i < baseFaceY.Count; i++)
                    {
                        faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceY[i].Tag);
                        Face baseFaceOcc = (Face)NXObjectManager.Get(faceTagOcc);
                        string tempHandle = theUfSession.Tag.AskHandleOfTag(baseFaceY[i].Tag);
                        tempHandleLst.Add(tempHandle);
                    }
                    tempHandleLst.Sort();
                    foreach (string singleHandle in tempHandleLst)
                    {
                        refData.REF_Y_HANDLE += (singleHandle + ", ");
                    }
                }
                if (baseFaceZ.Count != 0)
                {
                    List<string> tempHandleLst = new List<string>();
                    for (int i = 0; i < baseFaceZ.Count; i++)
                    {
                        faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceZ[i].Tag);
                        Face baseFaceOcc = (Face)NXObjectManager.Get(faceTagOcc);
                        string tempHandle = theUfSession.Tag.AskHandleOfTag(baseFaceZ[i].Tag);
                        tempHandleLst.Add(tempHandle);
                    }
                    tempHandleLst.Sort();
                    foreach (string singleHandle in tempHandleLst)
                    {
                        refData.REF_Z_HANDLE += (singleHandle + ", ");
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        // 判斷基準面資訊和裝夾圖屬性是否吻合
        private int CheckRefData(MesAttrCNC sMesDatData, RefData refData, out bool isRefDataMatch)
        {
            isRefDataMatch = false;
            // 裝夾圖屬性key
            string ATTR_REF_A_HANDLE = "REF_A_HANDLE";
            string ATTR_REF_B_HANDLE = "REF_B_HANDLE";
            string ATTR_REF_C_HANDLE = "REF_C_HANDLE";
            string ATTR_REF_D_HANDLE = "REF_D_HANDLE";
            string ATTR_REF_X_HANDLE = "REF_X_HANDLE";
            string ATTR_REF_Y_HANDLE = "REF_Y_HANDLE";
            string ATTR_REF_Z_HANDLE = "REF_Z_HANDLE";
            string ATTR_REF_A_BOX = "REF_A_BOX";
            string ATTR_REF_B_BOX = "REF_B_BOX";
            string ATTR_REF_C_BOX = "REF_C_BOX";
            string ATTR_REF_D_BOX = "REF_D_BOX";
            try
            {
//                 RefData refData;
//                 // 取得當前基準面資訊
//                 bool status = GetRefData(sCimAsmCompPart.design.comp, out refData);
//                 if (!status)
//                 {
//                     UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "取得當前基準面資訊發生錯誤...");
//                     return -1;
//                 }
                // 裝夾圖圖紙名稱
                string draftingSheetName = string.Format(@"{0}_{1}", sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID);
                // 取得圖紙
                Tag[] drawings;
                int drawingNum;
                theUfSession.Draw.AskDrawings(out drawingNum, out drawings);
                NXOpen.Drawings.DrawingSheet sectionSheet = null;
                for (int i = 0; i < drawingNum; i++)
                {
                    string tempDrawingName;
                    theUfSession.Obj.AskName(drawings[i], out tempDrawingName);
                    if (tempDrawingName.ToUpper() == draftingSheetName.ToUpper())
                    {
                        sectionSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(drawings[i]);
                        break;
                    }
                }
                // 取得屬性，判斷是否與當前基準面資訊一致
                try
                {
                    string REF_A_HANDLE = sectionSheet.GetStringAttribute(ATTR_REF_A_HANDLE);
                    string REF_B_HANDLE = sectionSheet.GetStringAttribute(ATTR_REF_B_HANDLE);
                    string REF_C_HANDLE = sectionSheet.GetStringAttribute(ATTR_REF_C_HANDLE);
                    string REF_D_HANDLE = sectionSheet.GetStringAttribute(ATTR_REF_D_HANDLE);
                    string REF_X_HANDLE = sectionSheet.GetStringAttribute(ATTR_REF_X_HANDLE);
                    string REF_Y_HANDLE = sectionSheet.GetStringAttribute(ATTR_REF_Y_HANDLE);
                    string REF_Z_HANDLE = sectionSheet.GetStringAttribute(ATTR_REF_Z_HANDLE);
                    string REF_A_BOX = sectionSheet.GetStringAttribute(ATTR_REF_A_BOX);
                    string REF_B_BOX = sectionSheet.GetStringAttribute(ATTR_REF_B_BOX);
                    string REF_C_BOX = sectionSheet.GetStringAttribute(ATTR_REF_C_BOX);
                    string REF_D_BOX = sectionSheet.GetStringAttribute(ATTR_REF_D_BOX);
                    if (REF_A_HANDLE == refData.REF_A_HANDLE &&
                        REF_B_HANDLE == refData.REF_B_HANDLE &&
                        REF_C_HANDLE == refData.REF_C_HANDLE &&
                        REF_D_HANDLE == refData.REF_D_HANDLE &&
                        REF_X_HANDLE == refData.REF_X_HANDLE &&
                        REF_Y_HANDLE == refData.REF_Y_HANDLE &&
                        REF_Z_HANDLE == refData.REF_Z_HANDLE &&
                        REF_A_BOX == refData.REF_A_BOX &&
                        REF_B_BOX == refData.REF_B_BOX &&
                        REF_C_BOX == refData.REF_C_BOX &&
                        REF_D_BOX == refData.REF_D_BOX)
                    {
                        isRefDataMatch = true;
                    }
                    else
                    {
                        isRefDataMatch = false;
                    }
                }
                catch (System.Exception ex)
                {
                    isRefDataMatch = false;
                }
            }
            catch (System.Exception ex)
            {
                return -1001;
            }
            return 0;
        }
        // 取得裝夾圖上之基準距離屬性
        private bool GetBaseDist()
        {
            string ATTR_MIN_X_POSITION = "MIN_X_POSITION";
            string ATTR_MIN_Y_POSITION = "MIN_Y_POSITION";
            string ATTR_MIN_Z_POSITION = "MIN_Z_POSITION";
            string ATTR_MAX_X_POSITION = "MAX_X_POSITION";
            string ATTR_MAX_Y_POSITION = "MAX_Y_POSITION";
            string ATTR_MAX_Z_POSITION = "MAX_Z_POSITION";
            try
            {
                // 裝夾圖圖紙名稱
                string draftingSheetName = string.Format(@"{0}_{1}", sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID);
                // 取得圖紙
                Tag[] drawings;
                int drawingNum;
                theUfSession.Draw.AskDrawings(out drawingNum, out drawings);
                NXOpen.Drawings.DrawingSheet sectionSheet = null;
                for (int i = 0; i < drawingNum; i++)
                {
                    string tempDrawingName;
                    theUfSession.Obj.AskName(drawings[i], out tempDrawingName);
                    if (tempDrawingName.ToUpper() == draftingSheetName.ToUpper())
                    {
                        sectionSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(drawings[i]);
                        break;
                    }
                }
                // 取得屬性
                try
                {
                    string XminStr = sectionSheet.GetStringAttribute(ATTR_MIN_X_POSITION);
                    string YminStr = sectionSheet.GetStringAttribute(ATTR_MIN_Y_POSITION);
                    string ZminStr = sectionSheet.GetStringAttribute(ATTR_MIN_Z_POSITION);
                    string XmaxStr = sectionSheet.GetStringAttribute(ATTR_MAX_X_POSITION);
                    string YmaxStr = sectionSheet.GetStringAttribute(ATTR_MAX_Y_POSITION);
                    string ZmaxStr = sectionSheet.GetStringAttribute(ATTR_MAX_Z_POSITION);
                    sBaseDist.MIN_X_POSITION = Math.Round(Convert.ToDouble(XminStr), 3, MidpointRounding.AwayFromZero);
                    sBaseDist.MIN_Y_POSITION = Math.Round(Convert.ToDouble(YminStr), 3, MidpointRounding.AwayFromZero);
                    sBaseDist.MIN_Z_POSITION = Math.Round(Convert.ToDouble(ZminStr), 3, MidpointRounding.AwayFromZero);
                    sBaseDist.MAX_X_POSITION = Math.Round(Convert.ToDouble(XmaxStr), 3, MidpointRounding.AwayFromZero);
                    sBaseDist.MAX_Y_POSITION = Math.Round(Convert.ToDouble(YmaxStr), 3, MidpointRounding.AwayFromZero);
                    sBaseDist.MAX_Z_POSITION = Math.Round(Convert.ToDouble(ZmaxStr), 3, MidpointRounding.AwayFromZero);
                }
                catch (System.Exception ex)
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        // 20151013 新增輸出判斷是否重新裝夾/校正之參數
        private bool GetClampCalibrateParam(NXOpen.Assemblies.Component designComp, NXOpen.Assemblies.Component fixtureComp, RefData refData, out ClampCalibrateParam ccParams)
        {
            ccParams = new ClampCalibrateParam();
            ccParams.AXIS_ANGLE = "";
            ccParams.BASE_CORNER_QUADRANT = "";
            ccParams.BASE_CORNER_VECTOR = "";
            ccParams.Z_AXIS_BOTTOM_HEIGHT = "";
            try
            {
                if (refData.REF_D_HANDLE == "")
                {
                    // 方料

                    // 1. 取得基準角象限
                    Tag baseFaceTagA = theUfSession.Tag.AskTagOfHandle(refData.REF_A_HANDLE);
                    Tag baseFaceTagB = theUfSession.Tag.AskTagOfHandle(refData.REF_B_HANDLE);
                    Tag baseFaceTagC = theUfSession.Tag.AskTagOfHandle(refData.REF_C_HANDLE);
                    Tag faceTagOccA = theUfSession.Assem.FindOccurrence(designComp.Tag, baseFaceTagA);
                    Tag faceTagOccB = theUfSession.Assem.FindOccurrence(designComp.Tag, baseFaceTagB);
                    Tag faceTagOccC = theUfSession.Assem.FindOccurrence(designComp.Tag, baseFaceTagC);
                    CaxGeom.FaceData baseFaceDataA, baseFaceDataB, baseFaceDataC;
                    CaxGeom.GetFaceData(faceTagOccA, out baseFaceDataA);
                    CaxGeom.GetFaceData(faceTagOccB, out baseFaceDataB);
                    CaxGeom.GetFaceData(faceTagOccC, out baseFaceDataC);
                    // 判斷ABC基準面法向量
                    if (baseFaceDataA.dir[0] >= 0.9 || baseFaceDataB.dir[0] >= 0.9 || baseFaceDataC.dir[0] > 0.9)
                    {
                        // X+
                        if (baseFaceDataA.dir[1] >= 0.9 || baseFaceDataB.dir[1] >= 0.9 || baseFaceDataC.dir[1] > 0.9)
                        {
                            // Y+
                            ccParams.BASE_CORNER_QUADRANT = "1";
                        }
                        else
                        {
                            // Y-
                            ccParams.BASE_CORNER_QUADRANT = "4";
                        }
                    }
                    else
                    {
                        // X-
                        if (baseFaceDataA.dir[1] >= 0.9 || baseFaceDataB.dir[1] >= 0.9 || baseFaceDataC.dir[1] > 0.9)
                        {
                            // Y+
                            ccParams.BASE_CORNER_QUADRANT = "2";
                        }
                        else
                        {
                            // Y-
                            ccParams.BASE_CORNER_QUADRANT = "3";
                        }
                    }

                    // 2. 取得治具旋轉角度(長短軸角度
                    double rotateAngle = getCAxis_byCompRotation(fixtureComp);
                    ccParams.AXIS_ANGLE = rotateAngle.ToString("f2");

                    // 3. 取得Z軸底面高度
                    double[] min, max;
                    Body designBodyOcc;
                    CaxPart.GetLayerBody(designComp, out designBodyOcc);
                    CaxPart.AskBoundingBoxExactByABS(designBodyOcc.Tag, out min, out max);
                    ccParams.Z_AXIS_BOTTOM_HEIGHT = min[2].ToString("f2");
                }
                else
                {
                    // 圓料

                    // 1. 取得基準面向量 (對刀面REF B)
                    if (refData.REF_B_HANDLE == "")
                    {
                        ccParams.BASE_CORNER_VECTOR = "0.00, 0.00, 0.00";
                    }
                    else
                    {
                        Tag baseFaceTagB = theUfSession.Tag.AskTagOfHandle(refData.REF_B_HANDLE);
                        Tag faceTagOccB = theUfSession.Assem.FindOccurrence(designComp.Tag, baseFaceTagB);
                        CaxGeom.FaceData baseFaceDataB;
                        CaxGeom.GetFaceData(faceTagOccB, out baseFaceDataB);
                        ccParams.BASE_CORNER_VECTOR = baseFaceDataB.dir[0].ToString("f2") + ", " + baseFaceDataB.dir[1].ToString("f2") + ", " + baseFaceDataB.dir[2].ToString("f2");
                    }
                    // 2. 取得Z軸底面高度
                    double[] min, max;
                    Body designBodyOcc;
                    CaxPart.GetLayerBody(designComp, out designBodyOcc);
                    CaxPart.AskBoundingBoxExactByABS(designBodyOcc.Tag, out min, out max);
                    ccParams.Z_AXIS_BOTTOM_HEIGHT = min[2].ToString("f2");
                }
            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow(ex.ToString());
                return false;
            }
            return true;
        }

//         // 20151102 檢查圖紙上的標註是否存在
//         private bool CheckNoteExist(string sheetName, out bool isNoteExist)
//         {
//             isNoteExist = false;
//             try
//             {
//                 NXOpen.Annotations.BaseNote[] noteAry = displayPart.Notes.ToArray();
//                 for (int i = 0; i < noteAry.Length; i++)
//                 {
//                     try
//                     {
//                         string temp = noteAry[i].GetStringAttribute("IS_NOTE_EXIST");
//                         if (temp.ToUpper() == sheetName.ToUpper())
//                         {
//                             isNoteExist = true;
//                             return true;
//                         }
//                     }
//                     catch (System.Exception ex)
//                     { }
//                 }
//             }
//             catch (System.Exception ex)
//             {
//                 return false;
//             }
//             return true;
//         }

        // 計算C軸角度
        public double getCAxis_byCompRotation(NXOpen.Assemblies.Component cp)
        {

            String pn = "", rn = "", i_n = "";
            double[] comp_origin = { 0, 0, 0 };
            double[] csys_matrix = new double[9];
            double[,] transform = new double[4, 4];
            theUfSession.Assem.AskComponentData(cp.Tag, out pn, out rn, out i_n, comp_origin, csys_matrix, transform);

            double[] rotate_matrix = new double[9];
            double[] m_matrix = new double[16];
            m_matrix[0] = transform[0, 0];
            m_matrix[1] = transform[0, 1];
            m_matrix[2] = transform[0, 2];
            m_matrix[3] = transform[0, 3];

            m_matrix[4] = transform[1, 0];
            m_matrix[5] = transform[1, 1];
            m_matrix[6] = transform[1, 2];
            m_matrix[7] = transform[1, 3];

            m_matrix[8] = transform[2, 0];
            m_matrix[9] = transform[2, 1];
            m_matrix[10] = transform[2, 2];
            m_matrix[11] = transform[2, 3];

            m_matrix[12] = transform[3, 0];
            m_matrix[13] = transform[3, 1];
            m_matrix[14] = transform[3, 2];
            m_matrix[15] = transform[3, 3];


            theUfSession.Mtx4.AskRotation(m_matrix, rotate_matrix);

//             CLog log = new CLog();
//             log.showlogByMtx9(rotate_matrix);
//             log.showlog("==========");
//             log.showlogByMtx16(m_matrix);


            double rx = Math.Atan2(rotate_matrix[7], rotate_matrix[8]);
            double ry = Math.Atan2((-1) * rotate_matrix[6], Math.Sqrt(rotate_matrix[7] + rotate_matrix[7] + rotate_matrix[8] * rotate_matrix[8]));
            double rz = Math.Atan2(rotate_matrix[3], rotate_matrix[0]);


            rx = rx * 180 / Math.PI;
            ry = ry * 180 / Math.PI;
            rz = rz * 180 / Math.PI;
//             log.showlog("==========");
//             log.showlog("rx : " + rx);
//             log.showlog("ry : " + ry);
//             log.showlog("rz : " + rz);


            return rz;
        }

//         // 判斷圖紙上之任務資訊是否正確
//         private int CheckMissionData(string sheetName)
//         {
//             try
//             {
//                 // 圖紙屬性key
//                 string MOLD_NO = "MOLD_NO";
//                 string DES_VER_NO = "DES_VER_NO";
//                 string WORK_NO = "WORK_NO";
//                 string PART_NO = "PART_NO";
//                 string MFC_NO = "MFC_NO";
//                 // 取得圖紙
//                 Tag[] drawings;
//                 int drawingNum;
//                 theUfSession.Draw.AskDrawings(out drawingNum, out drawings);
//                 NXOpen.Drawings.DrawingSheet thisSheet = null;
//                 for (int i = 0; i < drawingNum; i++)
//                 {
//                     string tempDrawingName;
//                     theUfSession.Obj.AskName(drawings[i], out tempDrawingName);
//                     if (tempDrawingName.ToUpper() == sheetName.ToUpper())
//                     {
//                         thisSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(drawings[i]);
//                         break;
//                     }
//                 }
//                 
//                 // 檢查任務資訊是否和MES傳入之值吻合
//                 bool isMissionDataCorrect = false;
//                 try
//                 {
//                     string mold_no = thisSheet.GetStringAttribute(MOLD_NO);
//                     string des_ver_no = thisSheet.GetStringAttribute(DES_VER_NO);
//                     string work_no = thisSheet.GetStringAttribute(WORK_NO);
//                     string part_no = thisSheet.GetStringAttribute(PART_NO);
//                     string mfc_no = thisSheet.GetStringAttribute(MFC_NO);
//                     if (mold_no == sMesDatData.MOLD_NO &&
//                         des_ver_no == sMesDatData.DES_VER_NO &&
//                         work_no == sMesDatData.WORK_NO &&
//                         part_no == sMesDatData.PART_NO &&
//                         mfc_no == sMesDatData.MFC_NO)
//                     {
//                         isMissionDataCorrect = true;
//                     }
//                     else
//                     {
//                         isMissionDataCorrect = false;
//                     }
//                 }
//                 catch (System.Exception ex)
//                 {
//                     isMissionDataCorrect = false;
//                 }
//                 if (!isMissionDataCorrect)
//                 {
//                     UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "裝夾圖版本錯誤，請重新輸出。");
//                     return -1;
//                 }
// 
//             }
//             catch (System.Exception ex)
//             {
//                 return -1001;
//             }
//             return 0;
//         }

        private int GetComponentTree()
        {
            bool status;
            try
            {
                //取得組立架構
                int err;
                err = CaxAsm.GetAsmCompTree(out AsmCompAry);
                if (err != 0)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "組立架構讀取失敗...");
                    return -1;
                }
                status = CaxAsm.GetCimAsmCompStruct(AsmCompAry, out sCimAsmCompPart);
                if (!status)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "組立架構讀取失敗...");
                    return -1;
                }
                if (sCimAsmCompPart.design.comp == null)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "設計零件讀取失敗...");
                    return -1;
                }
                if (sCimAsmCompPart.fixture.comp == null)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "治具零件讀取失敗...");
                    return -1;
                }
                if (sCimAsmCompPart.blank.comp == null)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "請先建立blank...");
                    return -1;
                }
            }
            catch (System.Exception ex)
            {
                return -1001;
            }
            return 0;
        }

        private int GetLayerBodyData()
        {
            bool status;
            try
            {
                // 取得設計零件的LayerBody
                Body designBody = null;
                status = CaxPart.GetLayerBody(sCimAsmCompPart.design.part, out designBody);
                if (!status)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "設計零件的LayerBody讀取失敗...");
                    return -1;
                }
                // 取得設計零件的尺寸大小
                Body designBodyOcc = null;
                CaxTransType.BodyPrototypeToNXOpenOcc(sCimAsmCompPart.design.comp.Tag, designBody.Tag, out designBodyOcc);
                CaxPart.AskBoundingBoxExactByWCS(designBodyOcc.Tag, out minDesignBodyWcs, out maxDesignBodyWcs);

//                 // 取得設計零件的基準面
//                 status = GetBaseCornerFaceAry(sCimAsmCompPart.design.comp, out baseCornerAry);
//                 if (!status)
//                 {
//                     UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "基準角取得錯誤...");
//                     return -1;
//                 }
                //讀取基準面 A B C D
                status = GetBaseCornerFaceABCD(sCimAsmCompPart.design.comp, out sBaseFaces);
                if (!status)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "請先定義基準角(面)!");
                    return -1;
                }

                // 判斷是否建立Blank素材檔
                if (sCimAsmCompPart.blank.comp == null)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "請先建立Blank素材檔...");
                    return -1;
                }

            }
            catch (System.Exception ex)
            {
                return -1001;
            }
            return 0;
        }

        private int GetFixtureData()
        {
            try
            {
                //取得治具類型
                string attr_value = "";
                // 判斷是否有多治具功能
                hasMultiFixture = (config.hasMultiFixture == "1");
                if (hasMultiFixture)
                {
                    // 有多治具功能
                    int fixtureNo = 0;
                    foreach (CaxAsm.CompPart compPart in AsmCompAry)
                    {
                        try
                        {
                            string attr = compPart.componentOcc.GetStringAttribute(CaxDefineParam.ATTR_CIM_TYPE);
                            if (attr == "FIXTURE")
                            {
                                fixtureLst.Add(compPart);
                                fixtureNo++;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            attr_value = "";
                            continue;
                        }
                    }
                    if (fixtureNo > 1)
                    {
                        isMultiFixture = true;
                        attr_value = "多治具";
                    }
                    else
                    {
                        isMultiFixture = false;
                        try
                        {
                            attr_value = sCimAsmCompPart.fixture.comp.GetStringAttribute("FIXTURE_TYPE");
                        }
                        catch (System.Exception ex)
                        {
                            attr_value = "";
                        }
                    }
                }
                else
                {
                    // 沒有多治具功能
                    try
                    {
                        attr_value = sCimAsmCompPart.fixture.comp.GetStringAttribute("FIXTURE_TYPE");
                    }
                    catch (System.Exception ex)
                    {
                        attr_value = "";
                    }
                }
                fixture_type = attr_value;


                if (fixture_type == "")
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "請先裝夾治具...");
                    return -1;
                }
            }
            catch (System.Exception ex)
            {
                return -1001;
            }
            return 0;
        }

        private bool DetermineASM()
        {
            try
            {
                if (sMesDatData.PROCESS_DESIGN == "ASM" || sMesDatData.PROCESS_DESIGN == "ASM_MODIFIED")
                {
                    isASM = true;
                    string attr_value = "";
                    foreach (CaxAsm.CompPart compPart in AsmCompAry)
                    {
                        try
                        {
                            attr_value = compPart.componentOcc.GetStringAttribute(CaxDefineParam.ATTR_CIM_TYPE);
                            if (attr_value == CaxDefineParam.ATTR_CIM_TYPE_SUB_DESIGN)
                            {
                                subDesignCompLst.Add(compPart.componentOcc);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            attr_value = "";
                            continue;
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool GetOperationData()
        {
            try
            {
                //Issue #9630
                // 刀具壽命限制有份兩段
                // 1，單條operation超出刀具壽命
                // 2，同一把刀所有的operation超出刀具壽命
                //  之前因為捨棄式刀具沒有上，也沒有注意,CAX段把這兩個全限制了
                // 現在我在測試捨棄式刀具，就有發現問題，因為捨棄式刀具是可以更換刀片的，所以現在的需求需要改下
                // 
                // 當是整體式刀具，就按現在的規則（1和2都不要滿足）
                // 如果是捨棄式刀具，需要修改成
                // 1，單條OPERATION超出刀具壽命 這條還是不變
                // 2，同一把刀所有的OPERATION超出刀具壽命 這條不需要限制（也就是還是可以出工單）

                bool status;

                //取得opration的名稱陣列
                //ArrayList operationAry = new ArrayList();
                Part workPart = theSession.Parts.Work;
                Part dispPart = theSession.Parts.Display;
                //string workFace_name = "";

                try
                {
                    if (dispPart.CAMSetup == null)
                    {
                        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ERROR,請先建立Operation.");
                        return false;
                    }
                }
                catch (System.Exception ex)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ERROR,請先建立Operation.");
                    return false;
                }

                //取得Operation Name
                OperationCollection Operations = dispPart.CAMSetup.CAMOperationCollection;
                Operation[] OperationAry = Operations.ToArray();
                if (OperationAry.Length == 0)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ERROR,請先建立Operation.");
                    return false;
                }

                //判斷Program Order Group 名稱與工段是否相同 2014/01/06
                NCGroup operationGroup;
                bool chk_faca_name = false;
                double max_length = 0.0;
                List<ToolLengehStatus> toolStatusAry = new List<ToolLengehStatus>();

                ListToolLengeh sListToolLengeh;
                sListToolLengeh.oper = null;
                sListToolLengeh.isOK = false;
                sListToolLengeh.isOverToolLength = false;
                sListToolLengeh.tool_name = "";
                sListToolLengeh.tool_ext_length = "";
                sListToolLengeh.oper_name = "";
                sListToolLengeh.cutting_length = 0.0;
                sListToolLengeh.cutting_length_max = 0.0;
                sListToolLengeh.part_offset = 0.0;

                ToolLengehStatus sToolLengehStatus;
                sToolLengehStatus.tool_name = "";
                sToolLengehStatus.tool_ext_length = "";
                sToolLengehStatus.cutting_length_max = 0.0;

                //判斷是否為電極
                if (sMesDatData.PART_TYPE_ID == "5")
                {
                    //電極

                    for (int a = 0; a < sMesDatData.ED_PARTS.Count; a++)
                    {
                        for (int i = 0; i < OperationAry.Length; i++)
                        {
                            operationGroup = OperationAry[i].GetParent(CAMSetup.View.ProgramOrder);
                            //if (section_face.ToUpper() == operationGroup.Name.ToUpper())
                            //{
                            chk_faca_name = true;

                            /*
                            double gap = sMesDatData.ED_PARTS[a].DISCHARGE_GAP;
                            if (gap > 0)
                            {
                                gap = gap * (-1);
                            }

                            status = CaxCAM.SetPartOffset(gap, "WORKPIECE_EL_AREA");
                            if (!status)
                            {
                                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, OperationAry[i].Name + " Part Offset 設定失敗...");
                                return false;
                            }
                            */

                            //Type operType = OperationAry[i].GetType();
                            //CaxLog.ShowListingWindow("OPERATION_TYPE : " + operType.Name.ToString());

                            //CaxLog.ShowListingWindow("GetStatus : " + OperationAry[i].GetStatus().ToString());
                            if (OperationAry[i].GetStatus() != CAMObject.Status.Complete && OperationAry[i].GetStatus() != CAMObject.Status.Repost)
                            {
                                status = CaxCAM.GenerateToolPath(OperationAry[i].Name);
                                if (!status)
                                {
                                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, OperationAry[i].Name + " Generate 失敗...");
                                    return false;
                                }
                            }

                            //CaxLog.ShowListingWindow("ToString : " + OperationAry[i].ToString());

                            //取得operation
                            sListToolLengeh.oper = OperationAry[i];

                            //取得operation名稱
                            sListToolLengeh.oper_name = OperationAry[i].Name;

                            //取得刀具GROUP
                            operationGroup = OperationAry[i].GetParent(CAMSetup.View.MachineTool);


                            //CaxLog.ShowListingWindow("TOOL_NAME : " + operationGroup.Name.ToString());

                            //GetToolData(operationGroup.Name);

                            //取得刀具名稱
                            string[] MachineToolNameSplitAry = operationGroup.Name.Split('_');
                            if (MachineToolNameSplitAry.Length < 3)
                            {
                                CaxLog.ShowListingWindow("刀具名稱錯誤 : " + operationGroup.Name);
                                return false;
                            }
                            sListToolLengeh.tool_name = MachineToolNameSplitAry[0];

                            sListToolLengeh.tool_ext_length = MachineToolNameSplitAry[2];

                            //取得切削長度
                            sListToolLengeh.cutting_length = OperationAry[i].GetToolpathCuttingLength();
                            //sListToolLengeh.cutting_length = 170000;

                            //取得part offset (Gap)
                            sListToolLengeh.part_offset = sMesDatData.ED_PARTS[a].DISCHARGE_GAP;
                            //CaxLog.ShowListingWindow(sMesDatData.ED_PARTS[a].DISCHARGE_GAP.ToString());

                            //判斷切削長度是否過長
                            max_length = 0.0;
                            sListToolLengeh.isOK = false;
                            sListToolLengeh.cutting_length_max = 0.0;
                            for (int j = 0; j < cToolCuttingLength.data.Count; j++)
                            {
                                if (cToolCuttingLength.data[j].TOOL_STD_ID.ToUpper() == sListToolLengeh.tool_name.ToUpper())
                                {
                                    for (int k = 0; k < cToolCuttingLength.data[j].HRC_ARRAY.Count; k++)
                                    {
//                                         // 20151012 新增比對工件材質，預設為第一筆數據
//                                         if (!(sMesDatData.MATERIAL_NAME2.ToUpper() == cToolCuttingLength.data[j].HRC_ARRAY[k].HRC_TYPE_NAME.ToUpper())
//                                              && k != 0)
//                                         {
//                                             continue;
//                                         }
//                                         max_length = cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER;
// 
//                                         if ((cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER * 1000) > sListToolLengeh.cutting_length)
//                                         {
//                                             sListToolLengeh.isOK = true;
//                                             goto GOTO_ISOK;
//                                         }
//                                         else
//                                         {
//                                             sListToolLengeh.isOK = false;
//                                             goto GOTO_ISOK;
//                                         }

                                        if (cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER > max_length)
                                        {
                                            max_length = cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER;
                                        }

                                        if ((cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER * 1000) > sListToolLengeh.cutting_length)
                                        {
                                            sListToolLengeh.isOK = true;
                                            goto GOTO_ISOK;
                                        }
                                    }
                                }
                            }

                        GOTO_ISOK:

                            //取得最大切削長度 (MES單位為m，所以要*1000 = mm)
                            sListToolLengeh.cutting_length_max = max_length * 1000;

                            bool chk_tool_name = false;
                            for (int j = 0; j < toolStatusAry.Count; j++)
                            {
                                if (sListToolLengeh.tool_name == toolStatusAry[j].tool_name)
                                {
                                    chk_tool_name = true;
                                }
                            }
                            if (!chk_tool_name)
                            {
                                //sToolLengehStatus.isOverToolLength = false;
                                sToolLengehStatus.tool_name = sListToolLengeh.tool_name;
                                sToolLengehStatus.tool_ext_length = sListToolLengeh.tool_ext_length;
                                sToolLengehStatus.cutting_length_max = sListToolLengeh.cutting_length_max;
                                toolStatusAry.Add(sToolLengehStatus);
                            }

                            ListToolLengehAry.Add(sListToolLengeh);
                            continue;
                            //}

                        }
                    }
                }
                else
                {
                    //非電極(工件)

                    for (int i = 0; i < OperationAry.Length; i++)
                    {
                        sListToolLengeh.oper = null;
                        sListToolLengeh.isOK = false;
                        sListToolLengeh.isOverToolLength = false;
                        sListToolLengeh.tool_name = "";
                        sListToolLengeh.tool_ext_length = "";
                        sListToolLengeh.oper_name = "";
                        sListToolLengeh.cutting_length = 0.0;
                        sListToolLengeh.cutting_length_max = 0.0;
                        sListToolLengeh.part_offset = 0.0;

                        operationGroup = OperationAry[i].GetParent(CAMSetup.View.ProgramOrder);
                        if (section_face.ToUpper() == operationGroup.Name.ToUpper())
                        {
                            chk_faca_name = true;

                            //Type operType = OperationAry[i].GetType();
                            //CaxLog.ShowListingWindow("OPERATION_TYPE : " + operType.Name.ToString());

                            //CaxLog.ShowListingWindow("GetStatus : " + OperationAry[i].GetStatus().ToString());
                            if (OperationAry[i].GetStatus() != CAMObject.Status.Complete && OperationAry[i].GetStatus() != CAMObject.Status.Repost)
                            {
                                status = CaxCAM.GenerateToolPath(OperationAry[i].Name);
                                if (!status)
                                {
                                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, OperationAry[i].Name + " Generate 失敗...");
                                    return false;
                                }
                            }

                            //CaxLog.ShowListingWindow("ToString : " + OperationAry[i].ToString());


                            //取得operation
                            sListToolLengeh.oper = OperationAry[i];

                            //取得operation名稱
                            sListToolLengeh.oper_name = OperationAry[i].Name;

                            //取得刀具GROUP
                            operationGroup = OperationAry[i].GetParent(CAMSetup.View.MachineTool);


                            //CaxLog.ShowListingWindow("TOOL_NAME : " + operationGroup.Name.ToString());

                            //GetToolData(operationGroup.Name);

                            //取得刀具名稱
                            string[] MachineToolNameSplitAry = operationGroup.Name.Split('_');
                            if (MachineToolNameSplitAry.Length < 3)
                            {
                                CaxLog.ShowListingWindow("刀具名稱錯誤 : " + operationGroup.Name);
                                return false;
                            }

                            sListToolLengeh.tool_name = MachineToolNameSplitAry[0];

                            for (int kk = 0; kk < MachineToolNameSplitAry.Length; kk++)
                            {
                                try
                                {
                                    int tempLength = Convert.ToInt16(MachineToolNameSplitAry[kk]);
                                    if (tempLength != 0)
                                    {
                                        sListToolLengeh.tool_ext_length = MachineToolNameSplitAry[kk];
                                        break;
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                	continue;
                                }
                            }

                            //CaxLog.ShowListingWindow("tool_ext_length : " + sListToolLengeh.tool_ext_length);

                            //取得切削長度
                            sListToolLengeh.cutting_length = OperationAry[i].GetToolpathCuttingLength();

                            //取得part offset (Gap)
                            double part_offset = 0;
                            CaxCAM.AskPartOffset(out part_offset);
                            sListToolLengeh.part_offset = part_offset;

                            //判斷切削長度是否過長
                            max_length = 0.0;
                            sListToolLengeh.isOK = false;
                            sListToolLengeh.cutting_length_max = 0.0;
                            for (int j = 0; j < cToolCuttingLength.data.Count; j++)
                            {
                                if (cToolCuttingLength.data[j].TOOL_STD_ID.ToUpper() == sListToolLengeh.tool_name.ToUpper())
                                {
                                    for (int k = 0; k < cToolCuttingLength.data[j].HRC_ARRAY.Count; k++)
                                    {
//                                         // 20151012 新增比對工件材質，預設為第一筆數據
//                                         if (!(sMesDatData.MATERIAL_NAME2.ToUpper() == cToolCuttingLength.data[j].HRC_ARRAY[k].HRC_TYPE_NAME.ToUpper())
//                                              && k != 0)
//                                         {
//                                             continue;
//                                         }
//                                             max_length = cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER;
// 
//                                         if ((cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER * 1000) > sListToolLengeh.cutting_length)
//                                         {
//                                             sListToolLengeh.isOK = true;
//                                             goto GOTO_ISOK;
//                                         }
//                                         else
//                                         {
//                                             sListToolLengeh.isOK = false;
//                                             goto GOTO_ISOK;
//                                         }
                                        if (cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER > max_length)
                                        {
                                            max_length = cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER;
                                        }

                                        if ((cToolCuttingLength.data[j].HRC_ARRAY[k].TOOL_METER * 1000) > sListToolLengeh.cutting_length)
                                        {
                                            sListToolLengeh.isOK = true;
                                            goto GOTO_ISOK;
                                        }

                                    }
                                }
                            }

                        GOTO_ISOK:

                            //取得最大切削長度 (MES單位為m，所以要*1000 = mm)
                            sListToolLengeh.cutting_length_max = max_length * 1000;

                            bool chk_tool_name = false;
                            for (int j = 0; j < toolStatusAry.Count; j++)
                            {
                                if (sListToolLengeh.tool_name == toolStatusAry[j].tool_name)
                                {
                                    chk_tool_name = true;
                                }
                            }
                            if (!chk_tool_name)
                            {
                                //sToolLengehStatus.isOverToolLength = false;
                                sToolLengehStatus.tool_name = sListToolLengeh.tool_name;
                                sToolLengehStatus.tool_ext_length = sListToolLengeh.tool_ext_length;//20150515 Andy取消這行註解
                                sToolLengehStatus.cutting_length_max = sListToolLengeh.cutting_length_max;
                                toolStatusAry.Add(sToolLengehStatus);
                            }

                            ListToolLengehAry.Add(sListToolLengeh);
                            continue;
                        }

                    }

                }
                CaxPart.Refresh();


                if (!chk_faca_name)
                {
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "Program Order 未找到 " + section_face + " Group!");
                    return false;
                }
                #region 20151230 Stewart Hide
                /*
                 * 2015 12月 小伍哥提出的"路徑切削米數和刀具壽命比對"
                 * 現階段規則：
                 * 整體式刀具：單條路徑切削米數不能超過刀具壽命
                 * 捨棄式刀具：單條路徑切削米數不能刀片單刃壽命
                 * 即不需判斷"同一把刀所有的operation超出刀具壽命"
                 *
                double totalOperLength = 0.0;
                for (int i = 0; i < toolStatusAry.Count; i++)
                {
                    //20150123 
                    // 刀具壽命限制有份兩段
                    // 1，單條operation超出刀具壽命
                    // 2，同一把刀所有的operation超出刀具壽命

                    // 當是整體式刀具，就按現在的規則（1和2都不要滿足）
                    // 如果是捨棄式刀具，需要修改成
                    // 1，單條OPERATION超出刀具壽命 這條還是不變
                    // 2，同一把刀所有的OPERATION超出刀具壽命 這條不需要限制（也就是還是可以出工單）

                    if (toolStatusAry[i].tool_name.ToUpper().IndexOf("I") == 0)
                    {
                        //捨棄式刀具
                        CaxLog.WriteLog("捨棄式刀具 : " + toolStatusAry[i].tool_name);
                        continue;
                    }

                    CaxLog.WriteLog("整體式刀具 : " + toolStatusAry[i].tool_name);

                    totalOperLength = 0.0;
                    for (int j = 0; j < ListToolLengehAry.Count; j++)
                    {
                        if (toolStatusAry[i].tool_name == ListToolLengehAry[j].tool_name &&
                            toolStatusAry[i].tool_ext_length == ListToolLengehAry[j].tool_ext_length)
                        {
                            totalOperLength += ListToolLengehAry[j].cutting_length;
                        }
                    }
                    if (totalOperLength > toolStatusAry[i].cutting_length_max)
                    {
                        for (int j = 0; j < ListToolLengehAry.Count; j++)
                        {
                            if (toolStatusAry[i].tool_name == ListToolLengehAry[j].tool_name)
                            {
                                sListToolLengeh.oper = ListToolLengehAry[j].oper;
                                sListToolLengeh.isOK = ListToolLengehAry[j].isOK;
                                sListToolLengeh.isOverToolLength = true;
                                sListToolLengeh.tool_name = ListToolLengehAry[j].tool_name;
                                sListToolLengeh.oper_name = ListToolLengehAry[j].oper_name;
                                sListToolLengeh.cutting_length = ListToolLengehAry[j].cutting_length;
                                sListToolLengeh.cutting_length_max = ListToolLengehAry[j].cutting_length_max;

                                //2015-03-20 新增
                                sListToolLengeh.part_offset = ListToolLengehAry[j].part_offset;
                                sListToolLengeh.tool_ext_length = ListToolLengehAry[j].tool_ext_length;


                                ListToolLengehAry[j] = sListToolLengeh;
                            }
                        }
                    }
                }
                */
                #endregion

            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 1.取得工件T面轉BO面Z軸偏移的距離
        /// 2.基準角底面到座標原點的距離
        /// 3.基準角長面(距離原點較長的面)到座標原點的距離
        /// 4.基準角短面(距離原點較短的面)到座標原點的距離
        /// </summary>
        /// <param name="sCimAsmCompPart"></param>
        /// <returns></returns>
        private bool GetCsysToRefFaceData()
        {
            sExportWorkTabel.X_OFFSET = "";
            sExportWorkTabel.Y_OFFSET = "";
            sExportWorkTabel.Z_BASE = "";
            sExportWorkTabel.Z_MOVE = "";
            try
            {
                //ExportWorkTabel sExportWorkTabel;
                string attr_value = "";
                //Z_MOVE
//                 // 20150618 改成取C面到座標原點距離  20150624
//                 Face baseC = null;
//                 double[] dirC = new double[3];
//                 for (int i = 0; i < baseCornerAry.Count; i++)
//                 {
//                     try
//                     {
//                         string attr = baseCornerAry[i].face.GetStringAttribute(CaxDefineParam.ATTR_CIM_REF_FACE);
//                         if (attr == "C")
//                         {
//                             baseC = baseCornerAry[i].face;
//                             dirC = baseCornerAry[i].sFaceData.dir;
//                             break;
//                         }
//                     }
//                     catch (System.Exception ex)
//                     {
//                     	continue;
//                     }
//                 }
//                 if (dirC[0] < -0.9999)
//                 {
//                     sExportWorkTabel.Z_MOVE = minDesignBodyWcs[0].ToString("f4");
//                 }
//                 else if (dirC[0] > 0.9999)
//                 {
//                     sExportWorkTabel.Z_MOVE = maxDesignBodyWcs[0].ToString("f4");
//                 }
//                 else if (dirC[1] < -0.9999)
//                 {
//                     sExportWorkTabel.Z_MOVE = minDesignBodyWcs[1].ToString("f4");
//                 }
//                 else if (dirC[1] > 0.9999)
//                 {
//                     sExportWorkTabel.Z_MOVE = maxDesignBodyWcs[1].ToString("f4");
//                 }
//                 else if (dirC[2] < -0.9999)
//                 {
//                     sExportWorkTabel.Z_MOVE = minDesignBodyWcs[2].ToString("f4");
//                 }
//                 else if (dirC[2] > 0.9999)
//                 {
//                     sExportWorkTabel.Z_MOVE = maxDesignBodyWcs[2].ToString("f4");
//                 }
//                 else
//                 {
//                     sExportWorkTabel.Z_MOVE = "";
//                 }
                try
                {
                    attr_value = "";
                    attr_value = displayPart.GetStringAttribute(CaxDefineParam.ATTR_CIM_Z_MOVE);
                }
                catch (System.Exception ex)
                {
                    attr_value = "";
                }
                if (attr_value == "")
                {
                    try
                    {
                        attr_value = "";
                        attr_value = sCimAsmCompPart.design.part.GetStringAttribute(CaxDefineParam.ATTR_CIM_Z_MOVE);
                    }
                    catch (System.Exception ex)
                    {
                        attr_value = "";
                    }
                }
                sExportWorkTabel.Z_MOVE = attr_value;

                //X_OFFSET
                try
                {
                    attr_value = "";
                    attr_value = displayPart.GetStringAttribute(CaxDefineParam.ATTR_CIM_FACE_X_OFFSET);
                }
                catch (System.Exception ex)
                {
                    attr_value = "";
                }
                if (attr_value == "")
                {
                    try
                    {
                        attr_value = "";
                        attr_value = sCimAsmCompPart.design.part.GetStringAttribute(CaxDefineParam.ATTR_CIM_FACE_X_OFFSET);
                    }
                    catch (System.Exception ex)
                    {
                        attr_value = "";
                    }
                }
                sExportWorkTabel.X_OFFSET = attr_value;

                //Y_OFFSET
                try
                {
                    attr_value = "";
                    attr_value = displayPart.GetStringAttribute(CaxDefineParam.ATTR_CIM_FACE_Y_OFFSET);
                }
                catch (System.Exception ex)
                {
                    attr_value = "";
                }
                if (attr_value == "")
                {
                    try
                    {
                        attr_value = "";
                        attr_value = sCimAsmCompPart.design.part.GetStringAttribute(CaxDefineParam.ATTR_CIM_FACE_Y_OFFSET);
                    }
                    catch (System.Exception ex)
                    {
                        attr_value = "";
                    }
                }
                sExportWorkTabel.Y_OFFSET = attr_value;

                //Z_BASE
                try
                {
                    attr_value = "";
                    attr_value = displayPart.GetStringAttribute(CaxDefineParam.ATTR_CIM_Z_BASE);
                }
                catch (System.Exception ex)
                {
                    attr_value = "";
                }
                if (attr_value == "")
                {
                    try
                    {
                        attr_value = "";
                        attr_value = sCimAsmCompPart.design.part.GetStringAttribute(CaxDefineParam.ATTR_CIM_Z_BASE);
                    }
                    catch (System.Exception ex)
                    {
                        attr_value = "";
                    }
                }
                sExportWorkTabel.Z_BASE = attr_value;

            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        // 20150721 取得安全高度...from 元富
        private bool GetSafetyHeight()
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;

                NXOpen.CAM.OrientGeometry orientGeometry1 = null;

                NCGroup[] CAMGroupAry = workPart.CAMSetup.CAMGroupCollection.ToArray();
                for (int i = 0; i < CAMGroupAry.Length; i++)
                {
                    try
                    {
                        if (CAMGroupAry[i].GetType().Name == "OrientGeometry")
                        {
                            orientGeometry1 = (NXOpen.CAM.OrientGeometry)CAMGroupAry[i];
                            break;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        continue;
                    }
                }
                if (orientGeometry1 == null)
                {
                    // 取得安全高度
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "取得安全高度失敗...");
                    return false;
                }

                NXOpen.CAM.MillOrientGeomBuilder millOrientGeomBuilder1;
                millOrientGeomBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillOrientGeomBuilder(orientGeometry1);
                Plane plane1 = (Plane)millOrientGeomBuilder1.TransferClearanceBuilder.PlaneXform;

                Expression expression5;
                expression5 = plane1.Expression;
                CLEARANE_PLANE = expression5.RightHandSide.ToString();
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool GetElecPartNoNote()
        {
            try
            {
                string baseNoteValue = "";
                BaseNote[] noteAry = displayPart.Notes.ToArray();
                for (int i = 0; i < noteAry.Length; i++)
                {
                    baseNoteValue = "";

                    try
                    {
                        baseNoteValue = noteAry[i].GetStringAttribute("ELEC_PART_NO");
                    }
                    catch (System.Exception ex)
                    {
                        baseNoteValue = "";
                    }
                    if (baseNoteValue == "1")
                    {
                        elecPartNoNote = noteAry[i];
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool GetBaseCornerFaceAry(NXOpen.Assemblies.Component component, out List<CimforceCaxTwPublic.CaxPart.BaseCorner> baseCornerAry)
        {
            baseCornerAry = new List<CimforceCaxTwPublic.CaxPart.BaseCorner>();

            try
            {
                //取得基準面(A,B,C)

                Part compPart = (Part)component.Prototype;
                Body[] BodyAry = compPart.Bodies.ToArray();

                Face baseFaceA = null;
                Face baseFaceB = null;
                Face baseFaceC = null;

                string attr_value = "";
                for (int i = 0; i < BodyAry.Length; i++)
                {
                    if (BodyAry[i].Layer != 1)
                    {
                        continue;
                    }
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
                            }
                            else if (attr_value == "B")
                            {
                                baseFaceB = bodyFaceAry[j];
                            }
                            else if (attr_value == "C")
                            {
                                baseFaceC = bodyFaceAry[j];
                            }
                        }
                        catch (System.Exception ex)
                        {
                            attr_value = "";
                            continue;
                        }
                    }
                }

                CimforceCaxTwPublic.CaxPart.BaseCorner sBaseCorner_A;
                Tag faceTagOcc;
                faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceA.Tag);
                sBaseCorner_A.face = (Face)NXObjectManager.Get(faceTagOcc);
                CaxGeom.GetFaceData(faceTagOcc, out sBaseCorner_A.sFaceData);

                CimforceCaxTwPublic.CaxPart.BaseCorner sBaseCorner_B;
                faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceB.Tag);
                sBaseCorner_B.face = (Face)NXObjectManager.Get(faceTagOcc);
                CaxGeom.GetFaceData(faceTagOcc, out sBaseCorner_B.sFaceData);

                CimforceCaxTwPublic.CaxPart.BaseCorner sBaseCorner_C;
                faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceC.Tag);
                sBaseCorner_C.face = (Face)NXObjectManager.Get(faceTagOcc);
                CaxGeom.GetFaceData(faceTagOcc, out sBaseCorner_C.sFaceData);

                if (Math.Abs(Math.Round(sBaseCorner_A.sFaceData.dir[0], 6, MidpointRounding.AwayFromZero)) >= 1.0)
                {
                    //A面與X軸平行
                    baseCornerAry.Add(sBaseCorner_B);
                    baseCornerAry.Add(sBaseCorner_A);
                }
                else
                {
                    //A面與Y軸平行
                    baseCornerAry.Add(sBaseCorner_A);
                    baseCornerAry.Add(sBaseCorner_B);
                }
                baseCornerAry.Add(sBaseCorner_C);
            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        //取得 零件基準角 陣列 (Y、X、Z 三方向面排序)
        public bool GetBaseCornerFaceABCD(NXOpen.Assemblies.Component component, out baseFace sBaseFaces)
        {
            sBaseFaces = new baseFace();
            sBaseFaces.baseFaceA.face = null;
            sBaseFaces.baseFaceB.face = null;
            sBaseFaces.baseFaceC.face = null;
            sBaseFaces.baseFaceD.face = null;
            sBaseFaces.baseFaceXLst = new List<CaxPart.BaseCorner>();
            sBaseFaces.baseFaceYLst = new List<CaxPart.BaseCorner>();
            sBaseFaces.baseFaceZLst = new List<CaxPart.BaseCorner>();
            sBaseFaces.hasA = false;
            sBaseFaces.hasB = false;
            sBaseFaces.hasC = false;
            sBaseFaces.hasD = false;
            sBaseFaces.hasX = false;
            sBaseFaces.hasY = false;
            sBaseFaces.hasZ = false;

            try
            {
                //取得基準面(A,B,C,D)

                Part compPart = (Part)component.Prototype;
                Body[] BodyAry = compPart.Bodies.ToArray();

                Face baseFaceA = null;
                Face baseFaceB = null;
                Face baseFaceC = null;
                Face baseFaceD = null;
                List<Face> baseFaceX = new List<Face>();
                List<Face> baseFaceY = new List<Face>();
                List<Face> baseFaceZ = new List<Face>();

                string attr_valueABCD = "";
                string attr_valueXYZ = "";
                for (int i = 0; i < BodyAry.Length; i++)
                {
                    // 20150720 搜基準面時排除不屬於layer 1 的面
                    if (BodyAry[i].Layer != 1)
                    {
                        continue;
                    }
                    Face[] bodyFaceAry = BodyAry[i].GetFaces();
                    for (int j = 0; j < bodyFaceAry.Length; j++)
                    {
                        // 找基準面屬性
                        attr_valueABCD = "";
                        try
                        {
                            //attr_value = bodyFaceAry[j].GetStringAttribute(CaxDefineParam.ATTR_CIM_TYPE);
                            attr_valueABCD = bodyFaceAry[j].GetStringAttribute(CaxDefineParam.ATTR_CIM_REF_FACE);
                            if (attr_valueABCD == "A")
                            {
                                baseFaceA = bodyFaceAry[j];
                            }
                            else if (attr_valueABCD == "B")
                            {
                                baseFaceB = bodyFaceAry[j];
                            }
                            else if (attr_valueABCD == "C")
                            {
                                baseFaceC = bodyFaceAry[j];
                            }
                            else if (attr_valueABCD == "D")
                            {
                                baseFaceD = bodyFaceAry[j];
                            }

                        }
                        catch (System.Exception ex)
                        {
                            attr_valueABCD = "";
                        }
                        // 20150617 找基準面偏移屬性
                        attr_valueXYZ = "";
                        try
                        {
                            attr_valueXYZ = bodyFaceAry[j].GetStringAttribute("CIM_REF");
                            if (attr_valueXYZ == "X")
                            {
                                baseFaceX.Add(bodyFaceAry[j]);
                            }
                            else if (attr_valueXYZ == "Y")
                            {
                                baseFaceY.Add(bodyFaceAry[j]);
                            }
                            else if (attr_valueXYZ == "Z")
                            {
                                baseFaceZ.Add(bodyFaceAry[j]);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            attr_valueXYZ = "";
                        }

                    }
                }
                Tag faceTagOcc;

                // 20150805 針對CNC轉正後ABC面不一定對應到XYZ軸 之處理
                if (baseFaceA != null && baseFaceB != null && baseFaceC != null)
                {
                    // ABC面
                    foreach (Face singleBaseFace in new Face[] { baseFaceA, baseFaceB, baseFaceC })
                    {
                        faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, singleBaseFace.Tag);
                        Face singleBaseFaceOcc = (Face)NXObjectManager.Get(faceTagOcc);
                        CaxGeom.FaceData singleBaseFaceData;
                        CaxGeom.GetFaceData(faceTagOcc, out singleBaseFaceData);
                        if (Math.Abs(singleBaseFaceData.dir[0]) >= 0.8)
                        {
                            // X向，存到A面
                            sBaseFaces.baseFaceA.face = singleBaseFaceOcc;
                            sBaseFaces.baseFaceA.sFaceData = singleBaseFaceData;
                            sBaseFaces.hasA = true;
                        }
                        else if (Math.Abs(singleBaseFaceData.dir[1]) >= 0.8)
                        {
                            // Y向，存到B面
                            sBaseFaces.baseFaceB.face = singleBaseFaceOcc;
                            sBaseFaces.baseFaceB.sFaceData = singleBaseFaceData;
                            sBaseFaces.hasB = true;
                        }
                        else if (Math.Abs(singleBaseFaceData.dir[2]) >= 0.8)
                        {
                            // Z向，存到C面
                            sBaseFaces.baseFaceC.face = singleBaseFaceOcc;
                            sBaseFaces.baseFaceC.sFaceData = singleBaseFaceData;
                            sBaseFaces.hasC = true;
                        }
                    }
                    // XYZ面 (基準面偏移)
                    List<Face> faceXYZLst = new List<Face>();
                    faceXYZLst.AddRange(baseFaceX);
                    faceXYZLst.AddRange(baseFaceY);
                    faceXYZLst.AddRange(baseFaceZ);
                    CaxPart.BaseCorner tempCorner;
                    foreach (Face singleBaseFace in faceXYZLst)
                    {
                        faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, singleBaseFace.Tag);
                        tempCorner.face = (Face)NXObjectManager.Get(faceTagOcc);
                        CaxGeom.GetFaceData(faceTagOcc, out tempCorner.sFaceData);

                        if (Math.Abs(tempCorner.sFaceData.dir[0]) >= 0.8)
                        {
                            // X向，存到基準面X
                            sBaseFaces.baseFaceXLst.Add(tempCorner);
                            sBaseFaces.hasX = true;
                        }
                        else if (Math.Abs(tempCorner.sFaceData.dir[1]) >= 0.8)
                        {
                            // Y向，存到基準面Y
                            sBaseFaces.baseFaceYLst.Add(tempCorner);
                            sBaseFaces.hasY = true;
                        }
                        else if (Math.Abs(tempCorner.sFaceData.dir[2]) >= 0.8)
                        {
                            // Z向，存到基準面Z
                            sBaseFaces.baseFaceZLst.Add(tempCorner);
                            sBaseFaces.hasZ = true;
                        }
                    }
                }
                else
                {
                    if (baseFaceA != null)
                    {
                        faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceA.Tag);
                        sBaseFaces.baseFaceA.face = (Face)NXObjectManager.Get(faceTagOcc);
                        CaxGeom.GetFaceData(faceTagOcc, out sBaseFaces.baseFaceA.sFaceData);
                        sBaseFaces.hasA = true;
                    }
                    if (baseFaceB != null)
                    {
                        faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceB.Tag);
                        sBaseFaces.baseFaceB.face = (Face)NXObjectManager.Get(faceTagOcc);
                        CaxGeom.GetFaceData(faceTagOcc, out sBaseFaces.baseFaceB.sFaceData);
                        sBaseFaces.hasB = true;
                    }
                    if (baseFaceC != null)
                    {
                        faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceC.Tag);
                        sBaseFaces.baseFaceC.face = (Face)NXObjectManager.Get(faceTagOcc);
                        CaxGeom.GetFaceData(faceTagOcc, out sBaseFaces.baseFaceC.sFaceData);
                        sBaseFaces.hasC = true;
                    }
                    if (baseFaceD != null)
                    {
                        faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceD.Tag);
                        sBaseFaces.baseFaceD.face = (Face)NXObjectManager.Get(faceTagOcc);
                        CaxGeom.GetFaceData(faceTagOcc, out sBaseFaces.baseFaceD.sFaceData);
                        sBaseFaces.hasD = true;
                    }
                    // 20150617 將基準偏移面存入struct
                    CaxPart.BaseCorner tempCorner;
                    if (baseFaceX.Count != 0)
                    {
                        for (int i = 0; i < baseFaceX.Count; i++)
                        {
                            faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceX[i].Tag);
                            tempCorner.face = (Face)NXObjectManager.Get(faceTagOcc);
                            CaxGeom.GetFaceData(faceTagOcc, out tempCorner.sFaceData);
                            sBaseFaces.baseFaceXLst.Add(tempCorner);
                            sBaseFaces.hasX = true;
                        }
                    }
                    if (baseFaceY.Count != 0)
                    {
                        for (int i = 0; i < baseFaceY.Count; i++)
                        {
                            faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceY[i].Tag);
                            tempCorner.face = (Face)NXObjectManager.Get(faceTagOcc);
                            CaxGeom.GetFaceData(faceTagOcc, out tempCorner.sFaceData);
                            sBaseFaces.baseFaceYLst.Add(tempCorner);
                            sBaseFaces.hasY = true;
                        }
                    }
                    if (baseFaceZ.Count != 0)
                    {
                        for (int i = 0; i < baseFaceZ.Count; i++)
                        {
                            faceTagOcc = theUfSession.Assem.FindOccurrence(component.Tag, baseFaceZ[i].Tag);
                            tempCorner.face = (Face)NXObjectManager.Get(faceTagOcc);
                            CaxGeom.GetFaceData(faceTagOcc, out tempCorner.sFaceData);
                            sBaseFaces.baseFaceZLst.Add(tempCorner);
                            sBaseFaces.hasZ = true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                //CaxPart.ShowListingWindow(ex.Message);
                return false;
            }

            return true;
        }

        // 讀取配置檔
        public static bool ReadJsonData(string JsonStyleLoadPath, out ConfigData JsonRead)
        {
            JsonRead = new ConfigData();
            try
            {
                bool status;
                // 檢查檔案是否存在
                if (!System.IO.File.Exists(JsonStyleLoadPath))
                {
                    CaxLog.ShowListingWindow("配置檔不存在：" + JsonStyleLoadPath);
                    return false;
                }
                string jsonText;
                status = CaxFile.ReadFileDataUTF8(JsonStyleLoadPath, out jsonText);
                if (!status)
                {
                    CaxLog.ShowListingWindow("配置檔讀取失敗：" + JsonStyleLoadPath);
                    return false;
                }
                JsonRead = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigData>(jsonText);
            }
            catch (System.Exception ex)
            {
                //CaxLog.ShowListingWindow(ex.Message);
                return false;
            }
            return true;
        }

    }
}
