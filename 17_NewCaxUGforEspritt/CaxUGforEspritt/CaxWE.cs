using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen.Utilities;
using System.Windows.Forms;
using CaxUGforEspritt;
using NXOpen;
using NXOpen.UF;
using NXOpen.Assemblies;
using CimforceCaxTwMFG;
using CimforceCaxTwPublic;
using System.Threading;
using WeData;
using CimforceCaxTwMD;
using CaxUFLib;
using System.IO;
using NXOpen.Features;
using WE_Get_Thrugh_Pnt;
using NXWEThruPntVer2;
using NXOpenUI;
using UploadMFG;
using NXCustomerComponent;
using DevComponents.DotNetBar;
using CSUGFunc;
using Newtonsoft.Json;

namespace CaxUGforEspritt
{

    public class CaxWE
    {
        private static Session theSession;
        private static UI theUI;
        private static UFSession theUfSession;

        public CaxWE()
        {
            theSession = Session.GetSession();
            theUI = UI.GetUI();
            theUfSession = UFSession.GetUFSession();
        }

        public static string ATTR_CIM_WEDM_CSYS = "WEDM_CSYS";
        public static SelectWorkPart cSelectWorkPartFrom;
        public static string U_DESIGN_REFER = "";
        public static List<Component> sComponent = new List<Component>();
        public static List<Part> sPart = new List<Part>();
        public static List<Component> ssComponent = new List<Component>();
        public static List<Part> ssPart = new List<Part>();
        public static Dictionary<skey, string> ssDictionary = new Dictionary<skey, string>();
        public static weExportData sWEData = new weExportData();
        public static List<Component> sssComponent = new List<Component>();
        public static List<Part> sssPart = new List<Part>();
        public static Mes2MfgJsonClass cMes2MfgJsonClass = new Mes2MfgJsonClass();
        public static string SlopePin = "";
        public static string Task_Type = "";
        public static NXOpen.Session.UndoMarkId OriginalMark;
        public static List<NXOpen.Session.UndoMarkId> ListUndoMark = new List<NXOpen.Session.UndoMarkId>();
        public static Dictionary<skey, string> sectionFaceDic = new Dictionary<skey, string>();
        public static string ReferencePosi_False = "";
        public static List<Body> ListCopyBody = new List<Body>();
        public static List<Body> ListOriginalBody = new List<Body>();

//         public struct skey
//         {
//             public Component comp;
//             public string section;
//             public string wkface;
//         }

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

        public struct KeyNormal
        {
            public string X;
            public string Y;
            public string Z;
        }

        public struct Center_Dir
        {
            public double[] center;
            public double[] dir;
        }

        public static string MFG_COLOR = "MFG_COLOR";
        public static string COLOR_ID = "COLOR_ID";
        public static string CIM_SECTION = "CIM_SECTION";
        public static string MACHING_TYPE = "MACHING_TYPE";
        public static string CIM_WORK_FACE = "CIM_WORK_FACE";
        public static string CIM_EDM_FACE = "CIM_EDM_FACE";
        public static string CIM_EDM_WEDM = "CIM_EDM_WEDM";
        public static string FEATURE_TYPE = "FEATURE_TYPE";
        public static string WE_SECTION = "WEDMS";

        public bool Execute()
        {
            //CaxLog.ShowListingWindow("107");
            bool status;
            status = ExportPartForWE();
            if (!status)
            {
                return false;
            }

            return true;
        }

        public static bool ExportPartForWE()
        {
            try
            {
                CaxLoadingDlg sCaxLoadingDlg = new CaxLoadingDlg();
                //sCaxLoadingDlg.Run();
                //sCaxLoadingDlg.SetLoadingText("數據計算中...");

                bool status;
                CaxAsm.SetWorkComponent(null);

                Part workPart = theSession.Parts.Work;

                /*
                //針對光寶做修改，谷崧線割WEDMS、光寶線割W  2015/8/31
                CaxFile.MesData mesData;
                bool statusb = CaxFile.ReadMesIP(out mesData);
                if (!statusb)
                    return false;
                string companyName = mesData.COMPANY_NAME;

                
                string MfgConfigDir = "";
                status = CaxMFG.GetMfgConfigDir(out MfgConfigDir);
                if (!status)
                {
                    CaxLog.ShowListingWindow("ConfigDir取得失敗");
                    return false;
                }

                string CompanyConfig = "";
                CompanyConfig = string.Format(@"{0}\{1}", MfgConfigDir, "COMPANY.txt");
                if (!System.IO.File.Exists(CompanyConfig))
                {
                    CaxLog.ShowListingWindow("CompanyConfig取得失敗或是沒有CompanyConfig檔案");
                    return false;
                }

                COMPANY_ARY cCOMPANY_ARY;
                status = GetCompanyData(CompanyConfig, out cCOMPANY_ARY);
                if (!status)
                {
                    CaxLog.ShowListingWindow("cCOMPANY_ARY取得失敗");
                    return false;
                }

                foreach (COMPANY i in cCOMPANY_ARY.COMPANYARY)
                {
                    if (companyName == i.COMPANYNAME)
                    {
                        WE_SECTION = i.WENAME;
                    }
                }
                */

                //取得組立架構
                List<CaxAsm.CompPart> AsmCompAry1;
                CaxAsm.CimAsmCompPart sCimAsmCompPart1;
                status = CaxAsm.GetAllAsmCompStruct(out AsmCompAry1, out sCimAsmCompPart1);
                if (sCimAsmCompPart1.design.comp == null)
                {
                    sCaxLoadingDlg.Stop();
                    CaxLog.ShowListingWindow("找不到設計零件檔案...");
                    return false;
                }

                //電極不刪檔案
                if (sCimAsmCompPart1.electorde.Count!=0)
                {

                }
                else
                {
                    foreach (CaxAsm.CompPart asmcomp in AsmCompAry1)
                    {
                        //if (asmcomp.componentOcc.DisplayName.ToUpper().IndexOf("_WEDMS") > 0)
                        if (asmcomp.componentOcc.DisplayName.ToUpper().IndexOf("_" + WE_SECTION) > 0)
                        {
                            status = ClosedAllFile(workPart, asmcomp);
                            if (!status)
                            {
                                sCaxLoadingDlg.Stop();
                                CaxLog.ShowListingWindow("刪線割檔案失敗");
                                return false;
                            }
                        }
                        else if (asmcomp.componentOcc.DisplayName.ToUpper().IndexOf("_WECAM") > 0)
                        {
                            status = ClosedAllFile(workPart, asmcomp);
                            if (!status)
                            {
                                sCaxLoadingDlg.Stop();
                                CaxLog.ShowListingWindow("刪線割下料檔案失敗");
                                return false;
                            }
                        }
                    }
                }
                
                CaxAsm.SetWorkComponent(null);
                
                //取得合併入子基準角方位
                if (cMes2MfgJsonClass.PROCESS_DESIGN != null && (cMes2MfgJsonClass.PROCESS_DESIGN == "U" || cMes2MfgJsonClass.PROCESS_DESIGN == "U_MODIFIED"))
                {
                    foreach (CaxAsm.CompPart asmcomp in AsmCompAry1)
                    {
                        string DesignPartAttr = "";
                        try
                        {
                            DesignPartAttr = asmcomp.componentOcc.GetStringAttribute("CIM_TYPE");
                        }
                        catch (System.Exception ex)
                        { continue; }

                        if (DesignPartAttr == "DESIGN")
                        {
                            CaxPart.RefCornerFace cRefCornerFace = new CaxPart.RefCornerFace();
                            CaxPart.GetBaseCornerFaceAry(asmcomp.componentOcc, out cRefCornerFace);
                            double[] Xdir = new double[3]{1,0,0};
                            double[] Adir = new double[3];
                            double dot_product;
                            //double[] Bdir = new double[3];
                            AskFaceDir(cRefCornerFace.faceA, out Adir);
                            //AskFaceDir(cRefCornerFace.faceB, out Bdir);
                            theUfSession.Vec3.Dot(Xdir, Adir, out dot_product);
                            if (dot_product>0)
                            {
                                U_DESIGN_REFER = "4";
                            }

                        }
                    }
                }
                
             
            
                //取得線割下料偏移量
                String mes2mfg_path = "";
                CaxMFG.GetMes2MfgDatPath((Part)workPart, out mes2mfg_path);
                CaxMFG.ReadMesJsonData(mes2mfg_path, out cMes2MfgJsonClass);
                //取得任務類型
                Task_Type = cMes2MfgJsonClass.TASK_TYPE;
                
                //加入公差、表面粗糙、訊息三個EXCEL資訊
                Dictionary<int, string> mdColorDic = new Dictionary<int, string>();
                status = GetColorDataAry(out mdColorDic);
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    CaxLog.ShowListingWindow("取得公差、表面粗糙資訊 失敗");
                    return false;
                }
                
                //取得組立架構下的物件
                List<CaxAsm.CompPart> AsmCompAry;
                CaxAsm.CimAsmCompPart sCimAsmCompPart;
                status = CaxAsm.GetAllAsmCompStruct(out AsmCompAry, out sCimAsmCompPart);
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    CaxLog.ShowListingWindow("取得組立架構下的物件 失敗");
                    return false;
                }
                Dictionary<string, CaxAsm.CompPart> AsmCompDic = new Dictionary<string, CaxAsm.CompPart>();
                for (int i = 0; i < AsmCompAry.Count; i++)
                {
                    string attr_value = "";
                    try
                    {
                        //有找到該屬性表示是跑位的空零件，忽略
                        attr_value = AsmCompAry[i].componentOcc.GetStringAttribute("REFERENCE_COMPONENT");
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
                status = GetSectionFaceDic(AsmCompAry, sCimAsmCompPart, out sectionFaceDic);
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    //CaxLog.ShowListingWindow("沒有須執行的線割電極或已經執行過了");
                    return false;
                }

                //建立線割檔
                Dictionary<WeListKey, WeFaceGroup> WEFaceDict = new Dictionary<WeListKey, WeFaceGroup>();
                status = CreateWePart(sectionFaceDic, sCimAsmCompPart, AsmCompDic, sCaxLoadingDlg, out WEFaceDict);
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    CaxLog.ShowListingWindow("CreateWePart 失敗");
                    return false;
                }
                
                //隱藏原始comp
                List<DisplayableObject> hideDispalyObject = new List<DisplayableObject>();
                status = HideAddComp(ref hideDispalyObject, sCimAsmCompPart, sectionFaceDic);
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    CaxLog.ShowListingWindow("HideAddComp 失敗");
                    return false;
                }
                //CaxLog.ShowListingWindow("233");
                //顯示新comp
                List<DisplayableObject> showDispalyObject = new List<DisplayableObject>();
                foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WEFaceDict)
                {
                    showDispalyObject.Add(kvp.Value.comp);
                }
                theSession.DisplayManager.UnblankObjects(showDispalyObject.ToArray());
                CaxPart.Refresh();

                //return false;
                #region 自動辨識佈穿線點
                //List<List<Face>> angledPinFaceSets = new List<List<Face>>();
                //List<List<Face>> openShaped = new List<List<Face>>();
                //foreach(KeyValuePair<WeListKey,WeFaceGroup> singleGroup in WEFaceDict)
                //{
                //    foreach (FaceGroupPnt singleFaceSet in singleGroup.Value.sFaceGroupPnt)
                //    {
                //        if (singleFaceSet.faceOccAry.Count != 0)
                //        {
                //            if (singleFaceSet.faceOccAry[0].GetStringAttribute("WE_TYPE") == "4")
                //            {
                //                angledPinFaceSets.Add(singleGroup.Value.sFaceGroupPnt[0].faceOccAry);
                //            }
                //            else if (singleFaceSet.faceOccAry[0].GetStringAttribute("WE_TYPE") == "7")
                //            {
                //                openShaped.Add(singleGroup.Value.sFaceGroupPnt[0].faceOccAry);
                //            }
                //        }
                //    }
                //}
                //CaxLog.ShowListingWindow(angledPinFaceSets.Count().ToString());
                //CaxLog.ShowListingWindow(openShaped.Count().ToString());
                //UGPUBLIC_1000_PublicFunc publicFunc = new UGPUBLIC_1000_PublicFunc();
                //List<Tuple<List<Face>, List<Face>>> groupedAnglePin = new List<Tuple<List<Face>, List<Face>>>();
                //foreach (List<Face> singleFaceSet in openShaped)
                //{
                //    List<Face> adjcentFaces = new List<Face>();
                //    foreach (Face singleFace in singleFaceSet)
                //    {
                //        List<Face> singleFaceAdjcents = new List<Face>();
                //        publicFunc.getAdjacFaces(singleFace, singleFace.GetBody().GetFaces().ToList(), out singleFaceAdjcents);
                //        foreach (Face singleAdjacFace in singleFaceAdjcents)
                //        {
                //            if (!adjcentFaces.Contains(singleAdjacFace) && !singleFaceSet.Contains(singleAdjacFace))
                //            {
                //                adjcentFaces.Add(singleAdjacFace);
                //            }
                //        }
                //    }
                //    foreach (List<Face> singleCompareFaceSet in angledPinFaceSets)
                //    {
                //        if (singleCompareFaceSet.Intersect(adjcentFaces).ToList().Count() == singleFaceSet.Count() + 2)
                //        {
                //            var facePair = new Tuple<List<Face>, List<Face>>(singleFaceSet, singleCompareFaceSet);
                //            groupedAnglePin.Add(facePair);
                //            //singleFaceSet.isToolEdge = true;
                //            //singleCompareFaceSet.workType = "4";
                //            //singleFaceSet.containPin = singleCompareFaceSet.originalFaces;
                //            break;
                //        }
                //    }
                //}
                //CaxLog.ShowListingWindow(groupedAnglePin.Count.ToString());
                //PRCES115_AnglePinDevideFaces devideObj = new PRCES115_AnglePinDevideFaces();
                //foreach (Tuple<List<Face>, List<Face>> singleFaceGroup in groupedAnglePin)
                //{
                //    devideObj.execute(singleFaceGroup, ref WEFaceDict);
                //}

                #region 2015-0407-Dropped by Leo
                
                List<int> ListIsAnalyzeSucess = new List<int>();
                
                foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WEFaceDict)
                {
                    bool isDone = false;
                    try
                    {
                        //string WE_PNT_X = kvp.Value.sFaceGroupPnt[0].faceOccAry[0].GetStringAttribute("WE_PNT_X");
                        //string WE_PNT_Y = kvp.Value.sFaceGroupPnt[0].faceOccAry[0].GetStringAttribute("WE_PNT_Y");
                        //if ((WE_PNT_X == "0.0") && (WE_PNT_Y == "0.0"))
                        //{
                        //    isDone = false;
                        //}
                        //else
                        //{
                        //    isDone = true;
                        //}
                    }
                    catch (NXException ex)
                    {
                        if (ex.ToString() == "The attribute not found.")
                        {
                            isDone = false;
                        }
                    }
                    
                    if (isDone == false)
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
                        Body designBodyProto;
                        CaxPart.GetLayerBody((Part)kvp.Value.comp.Prototype, out designBodyProto);


                        //回傳-2，表示解析失敗(刀口無法切面)
                        int x = cPRCES100_GetHoleType.execute(designBodyProto,allWeFaceGroup, ref WEFaceDict);
                        ListIsAnalyzeSucess.Add(x);

                        
                    }
                }
                //return true;
//                  foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WEFaceDict)
//                  {
//                      for (int i = 0; i < kvp.Value.sFaceGroupPnt.Count;i++ )
//                      {
//                          foreach (Face ii in kvp.Value.sFaceGroupPnt[i].faceOccAry)
//                          {
//                              ii.Highlight();
//                          }
//                      }
//                  }

                //ListIsAnalyzeSucess.Add(-2);//----test用----
                for (int i=0;i<ListIsAnalyzeSucess.Count;i++)
                {
                    for (int j = i; j < WEFaceDict.Keys.Count; j++)
                    {
                        WeListKey tempWeListKey = new WeListKey();
                        tempWeListKey = WEFaceDict.Keys.ElementAt(j);

                        WeFaceGroup tempWeFaceGroup = new WeFaceGroup();

                        WEFaceDict.TryGetValue(tempWeListKey, out tempWeFaceGroup);
                        tempWeFaceGroup.isAnalyzeSucess = ListIsAnalyzeSucess[i];
                        WEFaceDict[tempWeListKey] = tempWeFaceGroup;
                        break;
                    }
                }

                #endregion
                //return false;
                #endregion

                sCaxLoadingDlg.Stop();
                Application.EnableVisualStyles();
                cSelectWorkPartFrom = new SelectWorkPart();
                cSelectWorkPartFrom.WEFaceDict = new Dictionary<WeListKey, WeFaceGroup>();
                cSelectWorkPartFrom.WEFaceDict = WEFaceDict;
                cSelectWorkPartFrom.WE_SECTION = WE_SECTION;

                cSelectWorkPartFrom.mdColorDic = new Dictionary<int, string>();
                cSelectWorkPartFrom.mdColorDic = mdColorDic;
                

                NXOpenUI.FormUtilities.ReparentForm(cSelectWorkPartFrom);
                System.Windows.Forms.Application.Run(cSelectWorkPartFrom);
                cSelectWorkPartFrom.Dispose();

                if (cSelectWorkPartFrom.DialogResult != System.Windows.Forms.DialogResult.Cancel)
                {
                    //CaxLog.ShowListingWindow("111");
                    foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WEFaceDict)
                    {
                        //CaxLog.ShowListingWindow("386 kvp.Value.sFaceGroupPnt.Count:" + kvp.Value.sFaceGroupPnt.Count);
                        for (int i = 0; i < kvp.Value.sFaceGroupPnt.Count; i++)
                        {
                            for (int j = 0; j < kvp.Value.sFaceGroupPnt[i].faceOccAry.Count; j++)
                            {
                                //CaxLog.ShowListingWindow("391 kvp.Value.sFaceGroupPnt[i].faceOccAry.Count:" + kvp.Value.sFaceGroupPnt[i].faceOccAry.Count);
                                try
                                {
                                    Face NewFaceOcc = kvp.Value.sFaceGroupPnt[i].faceOccAry[j];
                                    Face NewFaceOcc_Prototype = (Face)NewFaceOcc.Prototype;
                                    string getNewFaceColorAttr = kvp.Value.sFaceGroupPnt[i].faceOccAry[j].GetStringAttribute("COLOR_ID");
                                    //CaxLog.ShowListingWindow("395 getNewFaceColorAttr:" + getNewFaceColorAttr);
                                    if (NewFaceOcc_Prototype == null)
                                    {
                                        theUfSession.Obj.SetColor(NewFaceOcc.Tag, Convert.ToInt32(getNewFaceColorAttr));
                                    }
                                    else
                                    {
                                        theUfSession.Obj.SetColor(NewFaceOcc_Prototype.Tag, Convert.ToInt32(getNewFaceColorAttr));
                                    }
                                }
                                catch (System.Exception ex)
                                {

                                }
                            }
                        }
                    }
                    //隱藏新的comp
                    theSession.DisplayManager.BlankObjects(showDispalyObject.ToArray());
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

                #region 隱藏A0版Component
                List<CaxAsm.CompPart> AsmCompAryToHide = new List<CaxAsm.CompPart>();
                CaxAsm.CimAsmCompPart sCimAsmCompPartToHide = new CaxAsm.CimAsmCompPart();
                CaxAsm.GetAsmCompStruct(out AsmCompAryToHide, out sCimAsmCompPartToHide);
                //隱藏所有的零件
                foreach (CaxAsm.CompPart compPart in AsmCompAryToHide)
                {
                    CaxAsm.ComponentHide(compPart.componentOcc);
                }
                //顯示Design零件
                CaxAsm.ComponentShow(sCimAsmCompPartToHide.design.comp);

                //如為U類，則要再次隱藏放在design中的insert件和blank
                foreach (CaxAsm.CompPart compPart in AsmCompAryToHide)
                {
                    string insertName = "";
                    try
                    {
                        insertName = compPart.componentOcc.GetStringAttribute(CaxDefineParam.ATTR_CIM_TYPE);
                    }
                    catch (System.Exception ex)
                    {
                        continue;
                    }
                    
                    if (insertName == "INSERT")
                    {
                        CaxAsm.ComponentHide(compPart.componentOcc);
                    }
                }
                CaxAsm.ComponentHide(sCimAsmCompPartToHide.blank.comp);
                #endregion

                //if (cSelectWorkPartFrom.DialogResult != DialogResult.OK)
                //{
                //    return false;
                //}


            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow(ex.ToString());
                return false;
            }

            return true;
        }

        public static bool ClosedAllFile(Part workPart, CaxAsm.CompPart asmcomp)
        {
            bool status = false;
            string Folder_Parts = "";
            string DisplayName = "";
            try
            {
                
                //CaxLog.ShowListingWindow("476");
                Folder_Parts = string.Format(@"{0}\{1}", Path.GetDirectoryName(workPart.FullPath), asmcomp.componentOcc.DisplayName+".prt");
                //CaxLog.ShowListingWindow("Folder_Parts:" + Folder_Parts);
                DisplayName = asmcomp.componentOcc.DisplayName;
                
                try
                {
                    status = CaxAsm.DeleteComponent(asmcomp.componentOcc);
                    if (!status)
                    {
                        CaxLog.ShowListingWindow("刪除組立架構中的線割零件檔失敗，請手動刪除");
                        return false;
                    }
                }
                catch (System.Exception ex)
                {}

                try
                {
                    status = CaxPart.CloseSelectedParts(DisplayName);
                    if (!status)
                    {
                        //CaxLog.ShowListingWindow("刪除記憶體中的線割零件檔失敗，請手動刪除");
                        //return false;
                    }
                }
                catch (System.Exception ex)
                {}

                try
                {
                    if (System.IO.File.Exists(Folder_Parts))
                    {
                        System.IO.File.Delete(Folder_Parts);
                    } 
                }
                catch (System.Exception ex)
                {}
                
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private static bool CreateWePart(Dictionary<skey, string> sectionFaceDic, 
                                                             CaxAsm.CimAsmCompPart sCimAsmCompPart, 
                                                             Dictionary<string, CaxAsm.CompPart> AsmCompDic, 
                                                             CaxLoadingDlg sCaxLoadingDlg,
                                                             out Dictionary<WeListKey, WeFaceGroup> WEFaceDict)
        {
            WEFaceDict = new Dictionary<WeListKey, WeFaceGroup>();
            Program.FailedSection = new Dictionary<skeyFailed, string>();
            try
            {
                bool status;
                Part workPart = theSession.Parts.Work;

                CaxAsm.SetWorkComponent(null);

                #region 產生線割檔、平移、旋轉

                Body OlderBody = null;
                List<DisplayableObject> hideDispalyObject = new List<DisplayableObject>();
                foreach (KeyValuePair<skey, string> kvp in sectionFaceDic)
                {
                    CaxPart.GetLayerBody(kvp.Key.comp, out OlderBody);

                    //取得olderComp的名字
                    string OlderCompName = kvp.Key.comp.Name;

                    Part weCompPart = (Part)kvp.Key.comp.Prototype;

                    //產生線割檔名( [SECTION ] _ [WORKFACE] )
                    string WE_PRT_NAME = "";
                    WE_PRT_NAME = string.Format(@"{0}_{1}", kvp.Key.section, kvp.Key.wkface);

                    //新建線割檔( 路徑 \ XX _ [SECTION] _ [WORKFACE] )
                    Body NewWEBody = null;
                    string NewCompFullPath = "";

                    NewCompFullPath = Path.GetDirectoryName(weCompPart.FullPath) + @"\" + kvp.Key.comp.Name + "_" + WE_PRT_NAME + ".prt";

                    //取得模號件號名稱
                    //string[] Mold_Part_Name_Ary = kvp.Key.comp.Name.Split('_');
                    //string Mold_Part_ASM = string.Format("{0}_{1}_{2}", Mold_Part_Name_Ary[0], Mold_Part_Name_Ary[1],"WE_PART");
                    //string Mold_Part_ASM_Path = Path.GetDirectoryName(weCompPart.FullPath) + @"\" + Mold_Part_ASM + ".prt";

                    bool chk_UDesign = false;
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
                    }
                    else
                    {
                        //組立架構中 不存在 該線割零件

                        #region 建立線割零件檔

                        if (System.IO.File.Exists(NewCompFullPath))
                        {
                            File.Delete(NewCompFullPath);
                        }

                        status = CreateNewWEComp(kvp.Key.comp, NewCompFullPath, OlderBody, out newWeComp, out NewWEBody);//此body是part的body
                        if (!status)
                        {
                            CaxLog.ShowListingWindow("CreateNewWEComp 失敗");
                            return false;
                        }
                        ListOriginalBody.Add(NewWEBody);
                        #endregion
                    }
                    //return false;
                    //隱藏原始comp
                    HideAddComp(ref hideDispalyObject, sCimAsmCompPart, sectionFaceDic);

                    CaxPart.GetLayerBody(newWeComp, out NewWEBody);//此body是comp的body
                    try
                    {
                        SlopePin = sCimAsmCompPart.design.comp.GetStringAttribute("DESCRIPTION");
                    }
                    catch (System.Exception ex)
                    {}
                    
                    bool Is_fallmaterial = false;
                    status = IsFallMaterial(NewWEBody,out Is_fallmaterial);
                    if (!status)
                    {
                        CaxLog.ShowListingWindow("判斷是否為下料任務失敗");
                    }

                    if (!chk_exist && chk_UDesign != true)
                    {
                        if (sCimAsmCompPart.electorde.Count != 0)//小製程
                        {
                            #region 小製程
                            //取得此電極的Number
                            string ELEC_NO = kvp.Key.comp.GetStringAttribute("CIM_ELEC_NO");
                            newWeComp.SetAttribute("CIM_ELEC_NO", ELEC_NO);
                            newWeComp.SetAttribute("CIM_ELEC_NAME", OlderCompName);

                            //對電極塞已經做過的屬性
                            //kvp.Key.comp.SetAttribute("WE_FINISHED", "1");

                            //對原comp塞線割檔的名字
                            kvp.Key.comp.SetAttribute("WE_PART_NAME", kvp.Key.comp.Name + "_" + WE_PRT_NAME);

                            //零件平移
                            string newWeCompName = newWeComp.Name;
                            Face[] NewWEBodyFaceAry = NewWEBody.GetFaces();

                            Face basef = null;
                            Face zf = null;
                            CaxGeom.FaceData basefData = new CaxGeom.FaceData();
                            bool IsCreatWing = false;
                            bool IsZMachining = false;

                            //平移電極
                            status = MoveEDM(NewWEBodyFaceAry, newWeComp, kvp, kvp.Key.section, out basef, out zf, out basefData, out IsCreatWing, out IsZMachining);
                            if (!status)
                            {
                                CaxLog.ShowListingWindow("MoveEDM 失敗");
                                CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                continue;
                            }

                            //旋轉電極
                            status = RotateEDM(IsZMachining, ref basef, ref basefData, ref zf);
                            if (!status)
                            {
                                CaxLog.ShowListingWindow("RotateEDM 失敗");
                                CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                continue;
                            }


                            //return false;

                            #region 決定基準角方位，複製檔案，清角處理
                            Body NewWEBodyForCopy = (Body)NewWEBody.Prototype;
                            NXObject CopyBodyNXobj = null;
                            Body CopyBody = null;

                            RefCornerFace sRefCornerFace;
                            //CaxGeom.FaceData sFaceDataA, sFaceDataB;
                            GetBaseCornerFaceAryOnPart(newWeComp, out sRefCornerFace);
                            Face cornerFaceA = sRefCornerFace.faceA;
                            Edge[] cornerFaceEdgeAry = cornerFaceA.GetEdges();
                            foreach (Edge SingleEdgeOnCornerFace in cornerFaceEdgeAry)
                            {
                                Face BlendFace = CaxGeom.GetNeighborFace(cornerFaceA, SingleEdgeOnCornerFace);
                                int type;
                                double[] dir = new double[3];
                                double[] center = new double[3];

                                theUfSession.Modl.AskFaceType(BlendFace.Tag, out type);
                                if (type == 16 || type == 23)
                                {
                                    AskFaceCenter(BlendFace, out center, out dir);
                                }
                                else
                                { continue; }

                                if (IsZMachining)//Z方向加工
                                {
                                    //決定Z方向加工的基準角方位，目前都給Z且基準角放在三或四象限，將資訊塞在基準角的面上
                                    ZMachAskReferPosi(center, ref BlendFace, NewWEBody);

                                    status = J_CopyPaste(NewWEBodyForCopy, out CopyBodyNXobj, out CopyBody);
                                    if (!status)
                                    {
                                        CaxLog.ShowListingWindow("J_CopyPaste 失敗");
                                    }
                                    ListCopyBody.Add(CopyBody);
                                    theUfSession.Obj.SetLayer(CopyBody.Tag, 10);

                                    sCaxLoadingDlg.Stop();

                                    //加入清角問題解決方案
                                    string Msg = "是否處理電極清角?";
                                    eTaskDialogResult chk_yes_no;
                                    chk_yes_no = CaxMsg.ShowMsgYesNo(Msg);
                                    if (chk_yes_no == eTaskDialogResult.Yes)
                                    {
                                    OpenDialog:
                                        Application.EnableVisualStyles();
                                        ZMachining cZMachiningDlg = new ZMachining();
                                        cZMachiningDlg.WEComp = newWeComp;
                                        cZMachiningDlg.WEBody = NewWEBody;
                                        cZMachiningDlg.hideDispalyObject = hideDispalyObject;
                                        cZMachiningDlg.kvp = kvp;

                                        NXOpenUI.FormUtilities.ReparentForm(cZMachiningDlg);
                                        System.Windows.Forms.Application.Run(cZMachiningDlg);
                                        cZMachiningDlg.Dispose();

                                        List<Face> ListFaceToSetAttr = new List<Face>();
                                        ListFaceToSetAttr = cZMachiningDlg.ListFaceToInsertAttr;
                                        foreach (Face tempFace in ListFaceToSetAttr)
                                        {
                                            tempFace.SetAttribute("MFG_COLOR", "214");
                                            tempFace.SetAttribute("CIM_EDM_WEDM", kvp.Key.section);
                                            tempFace.SetAttribute("CIM_EDM_FACE", kvp.Key.wkface);
                                            tempFace.SetAttribute("MACHING_TYPE", "1");
                                        }

                                        newWeComp = cZMachiningDlg.WEComp;
                                        NewWEBody = cZMachiningDlg.WEBody;
                                    }

                                }
                                else
                                {
                                    //決定XY方向加工的基準角方位，將資訊塞在基準角的面上
                                    XYMachAskReferPosi(center, NewWEBody, ref BlendFace);

                                    status = J_CopyPaste(NewWEBodyForCopy, out CopyBodyNXobj, out CopyBody);
                                    if (!status)
                                    {
                                        CaxLog.ShowListingWindow("J_CopyPaste 失敗");
                                    }
                                    ListCopyBody.Add(CopyBody);
                                    theUfSession.Obj.SetLayer(CopyBody.Tag, 10);
                                }
                            }
                            #endregion

                            #region 拍照
                            workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Trimetric, NXOpen.View.ScaleAdjustment.Fit);
                            string ImagePath = "";
                            ImagePath = string.Format(@"{0}\{1}_{2}", Path.GetDirectoryName(weCompPart.FullPath), kvp.Key.comp.Name, WE_PRT_NAME);
                            theUfSession.Disp.CreateImage(ImagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);
                            #endregion

                            //輸出STL
                            CaxPart.ExportBinarySTL(newWeComp, 0.05, 0.05);

                            //輸出BLANK

                            //return false;
                            //CaxLog.ShowListingWindow("IsCreatWing:"+IsCreatWing);

                            if (IsCreatWing)
                            {
                                #region 長翅膀
                                //由BODY抓取被裁切面(BASE_FACE與外迴圈的面)
                                List<NXObject> List_Feat_Obj = new List<NXObject>();//存Enlarge出來的特徵
                                List<Face> BaseFaces = new List<Face>();
                                status = GetCuttedFaceToWing(NewWEBodyFaceAry, out BaseFaces);
                                if (!status)
                                {
                                    CaxLog.ShowListingWindow("GetCuttedFaceToWing 失敗");
                                    CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                    RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                    continue;
                                }

                                //由BASE_FACE抓內迴圈的面建立BBOX並取得此BBOX的BODY
                                List<Face> NeiFace = new List<Face>();
                                Tag theBlock = Tag.Null;
                                NXObject theBlockObj = null;
                                status = CreateBBoxFromInnerLoop(NewWEBodyFaceAry, out NeiFace, out theBlock, out theBlockObj);
                                if (!status)
                                {
                                    CaxLog.ShowListingWindow("CreateBBoxFromInnerLoop 失敗");
                                    CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                    RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                    continue;
                                }

                                //由BBOX取得四周的面
                                List<Tag> ListAroundBboxFace = new List<Tag>();
                                status = GetAroundFaceFromBBox(theBlock, out ListAroundBboxFace);
                                if (!status)
                                {
                                    CaxLog.ShowListingWindow("GetAroundFaceFromBBox 失敗");
                                    CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                    RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                    continue;
                                }

//                                 foreach (Tag i in ListAroundBboxFace)
//                                 {
//                                     Face ii = (Face)NXObjectManager.Get(i);
//                                     ii.Highlight();
//                                 }
//                                 CaxLog.ShowListingWindow("123");

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

                                status = DeleteBodyObj(theBlockObj);
                                if (!status)
                                {
                                    CaxLog.ShowListingWindow("DeleteBodyObj 失敗");
                                    CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                    RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                    continue;
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
                                        CurrentWorkFaceAttr = tempFace.GetStringAttribute("CIM_EDM_FACE");//此處濾掉WorkFace
                                        continue;
                                    }
                                    catch (System.Exception ex)
                                    {
                                        try
                                        {
                                            CurrentSectionAttr = tempFace.GetStringAttribute("CIM_EDM_WEDM");
                                            if (CurrentSectionAttr == kvp.Key.section)
                                            {
                                                double[] center = new double[3];
                                                double[] dir = new double[3];
                                                AskFaceCenter(tempFace, out center, out dir);//濾掉基準座上的線割面，否則長翅膀會多長
                                                if (center[0] != 0)
                                                {
                                                    CurrentSectionWEFace.Add(tempFace);
                                                }
                                            }
                                        }
                                        catch (System.Exception ex1)
                                        { continue; }
                                    }
                                }
                                //先取得左上角與右下角的面
                                List<Face> ListSetAttrFace = new List<Face>(); //要塞屬性的面
                                List<Face> ListPullFace = new List<Face>(); //要拉伸的面

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
                            #endregion
                        }
                        else//大製程
                        {
                            #region 斜銷、滑塊入子
                            if (Task_Type == "3" || Is_fallmaterial == true/*SlopePin == "斜銷" || SlopePin == "滑塊入子" || Is_fallmaterial == true*/)
                            {
                                if (kvp.Key.wkface == "T")
                                {
                                    //RotateObjectByX(NewWEBody, 90);
                                }
                                else if (kvp.Key.wkface == "F")
                                {
                                    RotateObjectByX(NewWEBody, 90);
                                }
                                else if (kvp.Key.wkface == "L")
                                {
                                    RotateObjectByY(NewWEBody, 90);
                                }

                                //continue;

                                #region 斜銷平移(第一工位)
                                if (kvp.Key.section == "WECAM")
                                {
                                    status = MoveWorkPiece(NewWEBody);
                                    if (!status)
                                    {
                                        CaxLog.ShowListingWindow("MoveSlopePin 失敗");
                                        CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                        RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                        continue;
                                    }
                                }
                                #endregion

                                #region 斜銷轉正(第一工位)
                                Face[] NewWEBodyFaceAry = NewWEBody.GetFaces();
                                Face MaxMPFaceArea = null;
                                double dot_project, RotateAngle;
                                double[] Nor = new double[3];

                                //下料轉正
                                if (kvp.Key.section == "WECAM")
                                {
                                    status = GetMPFaceMaxArea(NewWEBodyFaceAry.ToList(), out MaxMPFaceArea);
                                    if (!status)
                                    {
                                        CaxLog.ShowListingWindow("找最大面積失敗");
                                    }

                                    Nor = new double[3] { 1, 0, 0 };
                                    status = GetSlopePinRotateAngle(MaxMPFaceArea, Nor, out dot_project, out RotateAngle);
                                    if (!status)
                                    {
                                        CaxLog.ShowListingWindow("GetSlopePinRotateAngle失敗");
                                    }

                                    if (RotateAngle > 0)
                                    {
                                        if (dot_project > 0)
                                        {
                                            RotateAngle = RotateAngle * -1;
                                        }
                                        J_MoveObject_RotateZ(NewWEBody, RotateAngle);
                                    }
                                    //return false;
                                }

                                /*
                                //一般線割轉正
                                if (kvp.Key.section != "WECAM")
                                {
                                    MaxMPFaceArea.Highlight();

                                    Nor = new double[3] { 0, 0, -1 };
                                    status = GetSlopePinRotateAngle(MaxMPFaceArea, Nor, out dot_project, out RotateAngle);
                                    if (!status)
                                    {
                                        CaxLog.ShowListingWindow("GetSlopePinRotateAngle失敗");
                                    }
                                    //CaxLog.ShowListingWindow("1060RotateAngle:" + RotateAngle);
                                    
                                    if (RotateAngle > 0)
                                    {
                                        if (dot_project > 0)
                                        {
                                            RotateAngle = RotateAngle * -1;
                                        }
                                        //CaxLog.ShowListingWindow("1067RotateAngle:" + RotateAngle);
                                        J_MoveObject_RotateY(NewWEBody, RotateAngle);
                                    }
                                    //return false;
                                }
                                */
                                #endregion


                                Body NewWEBodyForCopy = (Body)NewWEBody.Prototype;
                                NXObject CopyBodyNXobj = null;
                                Body CopyBody = null;
                                status = J_CopyPaste(NewWEBodyForCopy, out CopyBodyNXobj, out CopyBody);
                                if (!status)
                                {
                                    CaxLog.ShowListingWindow("J_CopyPaste 失敗");
                                }
                                ListCopyBody.Add(CopyBody);
                                //CaxLog.ShowListingWindow("copy" + kvp.Key.section + CopyBody);
                                theUfSession.Obj.SetLayer(CopyBody.Tag, 10);

                                //return false;

                                #region 拍照
                                workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Trimetric, NXOpen.View.ScaleAdjustment.Fit);
                                string ImagePath = "";
                                ImagePath = string.Format(@"{0}\{1}_{2}", Path.GetDirectoryName(weCompPart.FullPath), kvp.Key.comp.Name, WE_PRT_NAME);
                                theUfSession.Disp.CreateImage(ImagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);
                                #endregion

                                List<Face> ListOffsetFace = new List<Face>();
                                //CaxLog.ShowListingWindow("NewWEBody.isocc:" + NewWEBody.IsOccurrence);
                                Body NewWEBodyProto = (Body)NewWEBody.Prototype;//test用
                                //CaxLog.ShowListingWindow("NewWEBodyProto : "+NewWEBodyProto.ToString());
                                //Face[] NewWEBodyFaceAry = NewWEBody.GetFaces();
                                Face[] NewWEBodyProtoFaceAry = NewWEBodyProto.GetFaces();
                                //CaxLog.ShowListingWindow("NewWEBodyProtoFaceAry : " + NewWEBodyProtoFaceAry.Length);
                                foreach (Face SingleNewWEBodyProtoFace in NewWEBodyProtoFaceAry)
                                {
                                    string WEmfgcolor = "";
                                    string WEsection = "";
                                    string WEworkface = "";
                                    try
                                    {
                                        WEmfgcolor = SingleNewWEBodyProtoFace.GetStringAttribute("MFG_COLOR");
                                        WEsection = SingleNewWEBodyProtoFace.GetStringAttribute("WE_SECTION");
                                        WEworkface = SingleNewWEBodyProtoFace.GetStringAttribute("WE_WORK_FACE");
                                        if (kvp.Key.section == WEsection && kvp.Key.wkface == WEworkface)
                                        {
                                            ListOffsetFace.Add(SingleNewWEBodyProtoFace);
                                        }
                                    }
                                    catch (System.Exception ex)
                                    { }
                                }

                                //當有需偏移的面時，執行此功能
                                //CaxLog.ShowListingWindow(ListOffsetFace.Count.ToString());
                                if (ListOffsetFace.Count != 0 || kvp.Key.section == "WECAM")
                                {
                                    //return false;
                                    try
                                    {
                                        //Z方向上建立SheetBody
                                        Body PlaneBody;
                                        status = J_CreateSheetBody(NewWEBody, out PlaneBody);
                                        if (!status)
                                        {
                                            //CaxLog.ShowListingWindow("J_CreateSheetBody 失敗");
                                            CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                            RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                            continue;
                                        }

                                        //return false;

                                        Face[] PlaneBodyFaceAry = PlaneBody.GetFaces();
                                        Edge[] NewWEBodyProtoEdgeAry = NewWEBodyProto.GetEdges();
                                        NXObject testobj;

                                        status = J_Project(PlaneBodyFaceAry[0], NewWEBodyProtoEdgeAry, out testobj);
                                        if (!status)
                                        {
                                            //CaxLog.ShowListingWindow("J_Project 失敗");
                                            CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                            RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                            continue;
                                        }

                                        Feature testobjFea = (Feature)testobj;
                                        NXObject[] testobjFeaObj = testobjFea.GetEntities();
                                        NXObject TrimmedSheetObj;

                                        #region 由原始模型對SheetBody進行TrimmedSheet
                                        status = J_NewTrimmedSheet(PlaneBody, testobjFeaObj, out TrimmedSheetObj);
                                        if (!status)
                                        {
                                            CaxPart.DeleteNXObject(TrimmedSheetObj);
                                            CaxPart.DeleteNXObject(PlaneBody);
                                            //CaxLog.ShowListingWindow("J_NewTrimmedSheet 失敗");
                                            CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                            RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                            continue;
                                        }
                                        RemoveParameters(PlaneBody);
                                        DeleteBodyObj(testobj);
                                        #endregion

                                        Body CreateSolidBody;
                                        status = J_Thicken(PlaneBody, out CreateSolidBody);
                                        if (!status)
                                        {
                                            CaxPart.DeleteNXObject(PlaneBody);
                                            //CaxLog.ShowListingWindow("J_Thicken 失敗");
                                            CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                            RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                            continue;
                                        }
                                        RemoveParameters(CreateSolidBody);
                                        DeleteBody(PlaneBody);

                                        //return false;
                                        Face TempFaceToGetInnerLoop = null;
                                        Face[] CreateSolidBodyFaceAry = CreateSolidBody.GetFaces();
                                        foreach (Face tempFace in CreateSolidBodyFaceAry)
                                        {
                                            double[] dir = new double[3];
                                            AskFaceDir(tempFace, out dir);
                                            if (dir[2] >= -1 && dir[2] < -0.8)
                                            {
                                                TempFaceToGetInnerLoop = tempFace;
                                            }
                                        }
                                        TempFaceToGetInnerLoop.Highlight();
                                        //return false;

                                        #region 由TrimmedSheet取得內Loop，並投影到Z平面取得線段
                                        //Edge[] PlaneBodyEdgeAry = CaxUF_Lib.GetLoopEdges(PlaneBodyFaceAry[0], (EdgeLoopType)2);
                                        Edge[] PlaneBodyEdgeAry = CaxUF_Lib.GetLoopEdges(TempFaceToGetInnerLoop, (EdgeLoopType)2);
                                        //CaxLog.ShowListingWindow("PlaneBodyEdgeAry:" + PlaneBodyEdgeAry.Length);
                                        //return false;
                                        NXObject ProjectCurveObj;
                                        status = J_ProjectToZPlane(PlaneBodyEdgeAry, out ProjectCurveObj);
                                        if (!status)
                                        {
                                            //CaxLog.ShowListingWindow("J_ExtrudeCurve失敗");
                                            CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                            RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                            continue;
                                        }
                                        Feature ProjectFea = (Feature)ProjectCurveObj;
                                        NXObject[] ProjectFeaObj = ProjectFea.GetEntities();
                                        #endregion

                                        //return false;

                                        #region 取得工件高
                                        double[] minWcsFromNewWEBody = new double[3];
                                        double[] maxWcsFromNewWEBody = new double[3];
                                        CaxPart.AskBoundingBoxExactByWCS(NewWEBodyProto.Tag, out minWcsFromNewWEBody, out maxWcsFromNewWEBody);
                                        double WP_Height = maxWcsFromNewWEBody[2] - minWcsFromNewWEBody[2];
                                        #endregion

                                        #region 由線段進行拉伸，並刪除不要的Body
                                        Body ExtrudeBody;
                                        status = J_ExtrudeCurve(ProjectFeaObj, WP_Height.ToString(), out ExtrudeBody);
                                        if (!status)
                                        {
                                            CaxPart.DeleteNXObject(ExtrudeBody);
                                            //CaxLog.ShowListingWindow("J_ExtrudeCurve失敗");
                                            CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                            RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                            continue;
                                        }
                                        RemoveParameters(ExtrudeBody);
                                        DeleteBody(CreateSolidBody);
                                        #endregion

                                        //return false;

                                        #region 開始進行射線，取得需偏移的面
                                        List<Face> ExtrudeBodyFaceAry = ExtrudeBody.GetFaces().ToList();
                                        List<Face> ListFaceFromNewWEBody = NewWEBodyProtoFaceAry.ToList();
                                        List<Face> ListOffsetFaceFromExtrudeBody = new List<Face>();
                                        //由ExtrudeBody的面射回NewWEBody，進行比對
                                        status = GetOffsetFace(ExtrudeBodyFaceAry, ListFaceFromNewWEBody, ListOffsetFace, ref ListOffsetFaceFromExtrudeBody);
                                        if (!status)
                                        {
                                            //CaxLog.ShowListingWindow("GetOffsetFace失敗");
                                            CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                            RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                            continue;
                                        }
                                        #endregion

                                        #region 開始偏移
                                        bool Is_OffsetValueEqualZero = false;
                                        if (cMes2MfgJsonClass.OFFSET_VALUE == "0")
                                        {
                                            Is_OffsetValueEqualZero = true;
                                            string Msg = "偵測偏移量為0，是否繼續執行?";
                                            eTaskDialogResult chk_yes_no;
                                            chk_yes_no = CaxMsg.ShowMsgYesNo(Msg);
                                            if (chk_yes_no == eTaskDialogResult.No)
                                            {
                                                return false;
                                            }
                                        }
                                        if (!Is_OffsetValueEqualZero)
                                        {
                                            status = J_OffsetFace(ExtrudeBody, ListOffsetFaceFromExtrudeBody.ToArray(), cMes2MfgJsonClass.OFFSET_VALUE);
                                            if (!status)
                                            {
                                                CaxPart.DeleteNXObject(ExtrudeBody);
                                                //CaxLog.ShowListingWindow("J_OffsetFace失敗");
                                                CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                                RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                                continue;
                                            }
                                        }
                                        RemoveParameters(ExtrudeBody);
                                        #endregion

                                        #region 塞ExtrudeBody面屬性
                                        foreach (Face TempFace in ExtrudeBodyFaceAry)
                                        {
                                            CFace ff = new CFace();
                                            double[] Face_Normal = new double[3];
                                            Face_Normal = ff.GetNormal(TempFace.Tag);

                                            Face_Normal[0] = Math.Round(Face_Normal[0], 0, MidpointRounding.AwayFromZero);
                                            Face_Normal[1] = Math.Round(Face_Normal[1], 0, MidpointRounding.AwayFromZero);
                                            Face_Normal[2] = Math.Round(Face_Normal[2], 0, MidpointRounding.AwayFromZero);

                                            //double[] center = new double[3];
                                            //double[] dir = new double[3];
                                            //AskFaceCenter(TempFace, out center, out dir);
                                            //dir[2] = Math.Round(dir[2], 0, MidpointRounding.AwayFromZero);
                                            if (Face_Normal[2] != 1 && Face_Normal[2] != -1)
                                            {
                                                TempFace.SetAttribute("MFG_COLOR", "105");
                                                TempFace.SetAttribute("CIM_SECTION", kvp.Key.section);
                                                TempFace.SetAttribute("CIM_WORK_FACE", kvp.Key.wkface);
                                                TempFace.SetAttribute("WE_TYPE", "2");//目前先塞7，之後可能改6
                                            }
                                        }
                                        #endregion

                                        #region 刪除原本的模型
                                        ListOriginalBody.Remove(NewWEBodyProto);
                                        DeleteBody(NewWEBodyProto);
                                        NewWEBody = ExtrudeBody;
                                        ListOriginalBody.Add(NewWEBody);
                                        #endregion

                                        //return false;

                                    }
                                    catch (System.Exception ex)
                                    {}
                                    //return false;

                                }
                                else//這邊做第二工段
                                {
                                    if (kvp.Key.section != "WECAM")
                                    {
                                    //寫一個對話框讓使用者選第二工段的加工外型 1:割槽 2:T型 3:L型
                                    OpenDialog:
                                        Application.EnableVisualStyles();
                                        PingType OpenPingTypeDlg = new PingType();

                                        FormUtilities.ReparentForm(OpenPingTypeDlg);
                                        System.Windows.Forms.Application.Run(OpenPingTypeDlg);
                                        OpenPingTypeDlg.Dispose();

                                        if (OpenPingTypeDlg.DialogResult != System.Windows.Forms.DialogResult.OK)
                                        {
                                            UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "尚未指定斜銷類型");
                                            goto OpenDialog;
                                        }

                                        //當不等於槽類斜銷時，執行以下
                                        if (PingType.StringPingType != "1")
                                        {
                                            List<Face> ListFaceFromNewWEBody = NewWEBodyProtoFaceAry.ToList();
                                            List<Face> ListWEFaceFromNewWEBody = new List<Face>();

                                            #region 抓出要線割的面
                                            status = GetWEFace(ListFaceFromNewWEBody, out ListWEFaceFromNewWEBody);
                                            if (!status)
                                            {
                                                //CaxLog.ShowListingWindow("取得線割面失敗");
                                                CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                                RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                                continue;
                                            }
                                            #endregion

                                            List<Face> ListMaxAreaFace = new List<Face>();

                                            #region 算出最大面積
                                            status = GetWEFaceMaxArea(ListWEFaceFromNewWEBody, out ListMaxAreaFace);
                                            if (!status)
                                            {
                                                //CaxLog.ShowListingWindow("取得最大面積失敗");
                                                CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                                RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                                continue;
                                            }
                                            #endregion

                                            List<Edge> ListIntersectionEdge = new List<Edge>();
                                            List<Face> ListFaceToDivide = new List<Face>();

                                            #region 由最大線割面取得要被裁切的面、共用邊
                                            status = GetIntersectionEdgeAndFaceToDivideFromMaxAreaFace(ListMaxAreaFace, out ListIntersectionEdge, out ListFaceToDivide);
                                            if (!status)
                                            {
                                                //CaxLog.ShowListingWindow("取得要被裁切的面和共用邊失敗");
                                                CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                                RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                                continue;
                                            }
                                            #endregion

                                            //return false;

                                            UGPUBLIC_1000_PublicFunc cUGPUBLIC_1000_PublicFunc = new UGPUBLIC_1000_PublicFunc();
                                            List<NXObject> ListLineObj = new List<NXObject>();

                                            #region 由共用邊上的點建立線段
                                            status = CreateLineFromIntersectionEdge(ListIntersectionEdge, out ListLineObj);
                                            if (!status)
                                            {
                                                //CaxLog.ShowListingWindow("CreateLineFromIntersectionEdge 失敗");
                                                CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                                RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                                continue;
                                            }
                                            #endregion

                                            //return false;

                                            Feature outFea = null; //裁切後回傳的特徵(型態：NXOpen.Features.Divideface)
                                            Tag[] devideObjectsTag;
                                            List<Face> SaveDivideFace = new List<Face>();

                                            #region 開始裁切放電區的面
                                            Face SameFace = null;
                                            for (int i = 0; i < ListFaceToDivide.Count; i++)
                                            {
                                                Face tempFace = ListFaceToDivide[i];
                                                if (i == 0)
                                                {
                                                    SameFace = tempFace;
                                                }
                                                else
                                                {
                                                    if (SameFace == ListFaceToDivide[i])
                                                    { continue; }
                                                }

                                                status = LineDivide(tempFace, ListLineObj, out outFea);
                                                if (!status)
                                                {
                                                    //CaxLog.ShowListingWindow("裁切失敗");
                                                    CaxLog.ShowListingWindow("UG特徵解析失敗，已將" + kvp.Key.section + "工段轉為手動任務");
                                                    RecordFailed(ref hideDispalyObject, newWeComp, kvp);
                                                    continue;
                                                }

                                                NXOpen.Features.Divideface DivideFaceFeat = (NXOpen.Features.Divideface)outFea;
                                                Face[] DivideFaceFeatAry = DivideFaceFeat.GetFaces();
                                                for (int ii = 0; ii < DivideFaceFeatAry.Length; ii++)
                                                {
                                                    Face sDivideFace = (Face)DivideFaceFeatAry[ii];
                                                    SaveDivideFace.Add(sDivideFace);
                                                }
                                            }
                                            #endregion

                                            #region 殺參、刪除線段特徵
                                            RemoveParameters(NewWEBody);
                                            foreach (NXObject SingleObj in ListLineObj)
                                            {
                                                CaxPart.DeleteNXObject(SingleObj);
                                            }
                                            #endregion

                                            #region 由線割面再找裁切後的面上屬性
                                            CaxGeom.FaceData sFaceData;
                                            List<Face> ListDivideFace = new List<Face>();
                                            foreach (Face tempFace in ListMaxAreaFace)
                                            {
                                                CaxGeom.GetFaceData(tempFace.Tag, out sFaceData);
                                                Center_Dir sWEFaceData = new Center_Dir();
                                                AskFaceCenter(tempFace, out sWEFaceData.center, out sWEFaceData.dir);
                                                Edge[] FaceEdgeAry = tempFace.GetEdges();
                                                foreach (Edge tempEdge in FaceEdgeAry)
                                                {
                                                    Face FaceToFind = CaxGeom.GetNeighborFace(tempFace, tempEdge);
                                                    Center_Dir sFaceToFindData = new Center_Dir();
                                                    AskFaceCenter(FaceToFind, out sFaceToFindData.center, out sFaceToFindData.dir);
                                                    double TwoPtValue = sFaceToFindData.center[1] - sWEFaceData.center[1];
                                                    if ((TwoPtValue > 0 && sFaceData.dir[1] > 0) || (TwoPtValue < 0 && sFaceData.dir[1] < 0))
                                                    {
                                                        //FaceToFind.Highlight();
                                                        ListDivideFace.Add(FaceToFind);
                                                        FaceToFind.SetAttribute("CIM_SECTION", kvp.Key.section);
                                                        FaceToFind.SetAttribute("CIM_WORK_FACE", kvp.Key.wkface);
                                                        FaceToFind.SetAttribute("MACHING_TYPE", "7");
                                                        FaceToFind.SetAttribute("MFG_COLOR", "214");
                                                        FaceToFind.SetAttribute("TOL_COLOR", "83");//要問徐總粗加工的公差
                                                        break;
                                                    }
                                                }
                                            }
                                            #endregion

                                            //foreach (Face a in ListDivideFace)
                                            //{
                                            //    a.Highlight();
                                            //}

                                            //return false;

                                            List<Face> SaveFaceToBeFiltered = new List<Face>();

                                            #region 紀錄裁切面且要被過濾的相鄰的面
                                            foreach (Face tempFace in ListMaxAreaFace)
                                            {
                                                SaveFaceToBeFiltered.Add(tempFace);
                                            }
                                            foreach (Face tempFace in SaveDivideFace)
                                            {
                                                SaveFaceToBeFiltered.Add(tempFace);
                                            }
                                            #endregion

                                            #region 由裁切後的面找相鄰面，此相鄰面法向量與Z軸夾角大約80~91度
                                            double[] vertexZ = new double[3] { 0, 0, 1 };
                                            double[] vertexY = new double[3] { 0, 1, 0 };
                                            double[] FaceNor = new double[3];
                                            double smallAngle_Radians = 0.0;
                                            double largeAngle_Radians = 0.0;
                                            double smallAngle_Degree = 0.0;
                                            foreach (Face tempFace in ListDivideFace)
                                            {
                                                Edge[] tempFaceEdgeAry = tempFace.GetEdges();
                                                foreach (Edge tempFaceEdge in tempFaceEdgeAry)
                                                {
                                                    Face NeiFace = CaxGeom.GetNeighborFace(tempFace, tempFaceEdge);

                                                    GetFaceNor(NeiFace, out FaceNor);

                                                    theUfSession.Modl.AskVectorAngle(vertexZ, FaceNor, out smallAngle_Radians, out largeAngle_Radians);
                                                    smallAngle_Degree = smallAngle_Radians * 180 / Math.PI;

                                                    if (smallAngle_Degree > 80 && smallAngle_Degree < 91)
                                                    {
                                                        int count = 0;
                                                        for (int i = 0; i < SaveFaceToBeFiltered.Count; i++)
                                                        {
                                                            if (NeiFace != SaveFaceToBeFiltered[i])
                                                            {
                                                                count++;
                                                            }
                                                        }
                                                        if (count == SaveFaceToBeFiltered.Count)
                                                        {
                                                            //CaxLog.ShowListingWindow("5656");
                                                            theUfSession.Modl.AskVectorAngle(vertexY, FaceNor, out smallAngle_Radians, out largeAngle_Radians);
                                                            smallAngle_Degree = smallAngle_Radians * 180 / Math.PI;
                                                            //CaxLog.ShowListingWindow("smallAngle_Degree:" + smallAngle_Degree);
                                                            if (smallAngle_Degree >= 0 && smallAngle_Degree < 10)
                                                            {
                                                                //CaxLog.ShowListingWindow("5656");
                                                                //NeiFace.Highlight();
                                                                NeiFace.SetAttribute("CIM_SECTION", kvp.Key.section);
                                                                NeiFace.SetAttribute("CIM_WORK_FACE", kvp.Key.wkface);
                                                                NeiFace.SetAttribute("MACHING_TYPE", "7");
                                                                NeiFace.SetAttribute("MFG_COLOR", "214");
                                                                NeiFace.SetAttribute("TOL_COLOR", "83");//要問徐總粗加工的公差
                                                            }
                                                        }
                                                    }
                                                    //CaxLog.ShowListingWindow("smallAngle_Degree:" + smallAngle_Degree.ToString());
                                                    //CaxLog.ShowListingWindow("largeAngle_Degree:" + largeAngle_Degree.ToString());
                                                    //CaxLog.ShowListingWindow("-----");
                                                }
                                                //break;
                                            }
                                            #endregion

                                            //return false;

                                            NXObject nxobj;
                                            double faceArea1 = 0.0;
                                            double faceArea2 = 0.0;
                                            double faceArea3 = 0.0;
                                            Face face1 = null;
                                            Face face2 = null;
                                            Face face3 = null;
                                            List<Face> Machining_F = new List<Face>();
                                            List<Face> Machining_R = new List<Face>();
                                            double[] firstpt, secondpt, thirdpt, fourthpt;
                                            foreach (Face TempFace in ListMaxAreaFace)
                                            {
                                                ListLineObj = new List<NXObject>();
                                                double[] minWcs = new double[3];
                                                double[] maxWcs = new double[3];
                                                CaxPart.AskBoundingBoxExactByWCS(TempFace.Tag, out minWcs, out maxWcs);
                                                double length = maxWcs[0] - minWcs[0];
                                                double height = maxWcs[2] - minWcs[2];

                                                //建立四個點
                                                firstpt = new double[3] { minWcs[0] + length / 5, minWcs[1], minWcs[2] - 1 };
                                                secondpt = new double[3] { minWcs[0] + length / 5, minWcs[1], minWcs[2] + height + 1 };
                                                thirdpt = new double[3] { maxWcs[0] - length / 5, maxWcs[1], maxWcs[2] - height - 1 };
                                                fourthpt = new double[3] { maxWcs[0] - length / 5, maxWcs[1], maxWcs[2] + 1 };
                                                Feature _NullFeat;
                                                cUGPUBLIC_1000_PublicFunc.createLineVia2Points(firstpt, secondpt, out nxobj, out _NullFeat);
                                                ListLineObj.Add(nxobj);
                                                cUGPUBLIC_1000_PublicFunc.createLineVia2Points(thirdpt, fourthpt, out nxobj, out _NullFeat);
                                                ListLineObj.Add(nxobj);

                                                //return false;

                                                cUGPUBLIC_1000_PublicFunc.devideSurface(ListLineObj, TempFace, out outFea);
                                                theUfSession.Modl.AskFeatFaces(outFea.Tag, out devideObjectsTag);

                                                for (int i = 0; i < devideObjectsTag.Length; i++)
                                                {
                                                    Face tempface = (Face)NXObjectManager.Get(devideObjectsTag[i]);
                                                    MeasureFaces cMeasureFaces;
                                                    AskFaceAreaAndPerimeter(tempface, out cMeasureFaces);
                                                    if (i == 0)
                                                    {
                                                        faceArea1 = cMeasureFaces.Area;
                                                        face1 = tempface;
                                                    }
                                                    else if (i == 1)
                                                    {
                                                        faceArea2 = cMeasureFaces.Area;
                                                        face2 = tempface;
                                                    }
                                                    else
                                                    {
                                                        faceArea3 = cMeasureFaces.Area;
                                                        face3 = tempface;
                                                    }
                                                }
                                                if (faceArea1 > faceArea2 && faceArea1 > faceArea3)
                                                {
                                                    Machining_R.Add(face1);
                                                    Machining_F.Add(face2);
                                                    Machining_F.Add(face3);
                                                    //face1.Highlight();
                                                }
                                                else if (faceArea2 > faceArea1 && faceArea2 > faceArea3)
                                                {
                                                    Machining_R.Add(face2);
                                                    Machining_F.Add(face1);
                                                    Machining_F.Add(face3);
                                                    //face2.Highlight();
                                                }
                                                else
                                                {
                                                    Machining_R.Add(face3);
                                                    Machining_F.Add(face1);
                                                    Machining_F.Add(face2);
                                                    //face3.Highlight();
                                                }

                                            }

                                            //return false;

                                            #region 塞屬性、殺參、刪線段
                                            foreach (Face TempFace in Machining_R)
                                            {
                                                TempFace.SetAttribute("CIM_SECTION", kvp.Key.section);
                                                TempFace.SetAttribute("CIM_WORK_FACE", kvp.Key.wkface);
                                                TempFace.SetAttribute("MACHING_TYPE", "7");
                                                TempFace.SetAttribute("MFG_COLOR", "213");
                                                TempFace.SetAttribute("TOL_COLOR", "83");//要問徐總粗加工的公差
                                            }

                                            foreach (Face TempFace in Machining_F)
                                            {
                                                TempFace.SetAttribute("CIM_SECTION", kvp.Key.section);
                                                TempFace.SetAttribute("CIM_WORK_FACE", kvp.Key.wkface);
                                                TempFace.SetAttribute("MACHING_TYPE", "7");
                                                TempFace.SetAttribute("MFG_COLOR", "214");
                                                TempFace.SetAttribute("TOL_COLOR", "83");//要問徐總粗加工的公差
                                            }

                                            RemoveParameters(NewWEBody);

                                            foreach (NXObject TargetObj in ListLineObj)
                                            {
                                                DeleteBodyObj(TargetObj);
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region 模仁、合併入子
                            else
                            {
                                #region NEW製程旋轉
                                CaxAsm.SetWorkComponent(newWeComp);
                                if (kvp.Key.wkface == "T")
                                {
                                    //RotateObjectByZ(NewWEBody, 90);
                                }
                                else if (kvp.Key.wkface == "F" || kvp.Key.wkface == "BA")
                                {
                                    RotateObjectByX(NewWEBody, -90);
                                }
                                else if (kvp.Key.wkface == "R" || kvp.Key.wkface == "L")
                                {
                                    RotateObjectByY(NewWEBody, 90);
                                }
                                #endregion

                                #region NEW製程平移
                                if (Task_Type != "3"/*SlopePin != "滑塊"*/)
                                {
                                    status = MoveWorkPiece(NewWEBody);
                                    if (!status)
                                    {
                                        //CaxLog.ShowListingWindow("MoveWorkPiece 失敗");
                                        hideDispalyObject.Add(newWeComp);
                                        skeyFailed keyy = new skeyFailed();
                                        keyy.comp = newWeComp;
                                        keyy.compName = newWeComp.Name;
                                        keyy.section = kvp.Key.section;
                                        keyy.wkface = kvp.Key.wkface;
                                        Program.FailedSection.Add(keyy, kvp.Value);
                                        continue;
                                    }
                                }
                                #endregion

                                #region NEW製程判斷機內、機外
                                double[] WPmin1 = new double[3];
                                double[] WPmax1 = new double[3];
                                string outer_inner1 = "";
                                string reference_posi1 = "";
                                WorkPiece WP1 = new WorkPiece();
                                CaxPart.AskBoundingBoxExactByWCS(NewWEBody.Tag, out WPmin1, out WPmax1);
                                WP1.WP_Length = WPmax1[0] - WPmin1[0];
                                WP1.WP_Wide = WPmax1[1] - WPmin1[1];
                                WP1.WP_Height = WPmax1[2] - WPmin1[2];
                                InnerOuterAndRefPosi(newWeComp, WP1, out outer_inner1, out reference_posi1);
                                ReferencePosi_False = reference_posi1;
                                #endregion

                                #region 如果是模板，則將坐標系平移至導柱孔中心點
                                if (cMes2MfgJsonClass.TASK_TYPE == "1")
                                {
                                    Face[] WeBodyFaceAry = NewWEBody.GetFaces();
                                    List<Face> ListGuideFace = new List<Face>();
                                    foreach (Face singleFace in WeBodyFaceAry)
                                    {
                                        string GuideFaceAttr = "";
                                        try
                                        {
                                            GuideFaceAttr = singleFace.GetStringAttribute("FEATURE_TYPE");
                                        }
                                        catch (System.Exception ex)
                                        {
                                            continue;
                                        }
                                        if (GuideFaceAttr == "GUIDE_BUSHING_HOLE")
                                        {
                                            ListGuideFace.Add(singleFace);
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    
                                    //Dictionary<Face, string> GuideFaceCenter = new Dictionary<Face, string>();
                                    //Face TargetFace;
                                    double[] center = new double[3];
                                    foreach (Face singleFace in ListGuideFace)
                                    {
                                        //singleFace.Highlight();
                                        
                                        double[] dir = new double[3];
                                        AskFaceCenter(singleFace, out center, out dir);
                                        if (reference_posi1 == "1")
                                        {
                                            if (center[0]>0 && center[1]>0)
                                            {
                                                break;
                                            }
                                        }
                                        else if (reference_posi1 == "2")
                                        {
                                            if (center[0]<0 && center[1]>0)
                                            {
                                                break;
                                            }
                                        }
                                        else if (reference_posi1 == "4")
                                        {
                                            if (center[0]>0 && center[1]<0)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    
                                    Point3d firstpnt = new Point3d();
                                    Point3d secondpnt = new Point3d(0.0, 0.0, 0.0);
                                    firstpnt.X = center[0];
                                    firstpnt.Y = center[1];
                                    firstpnt.Z = 0.0;
                                    NewMoveObjectByPntToPnt(NewWEBody, firstpnt, secondpnt);

                                }
                                #endregion

                                //return false;

                                Body NewWEBodyForCopy = (Body)NewWEBody.Prototype;
                                NXObject CopyBodyNXobj = null;
                                Body CopyBody = null;
                                status = J_CopyPaste(NewWEBodyForCopy, out CopyBodyNXobj, out CopyBody);
                                if (!status)
                                {
                                    //CaxLog.ShowListingWindow("J_CopyPaste 失敗");
                                }
                                ListCopyBody.Add(CopyBody);
                                theUfSession.Obj.SetLayer(CopyBody.Tag, 10);

                                #region 拍照
                                workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Trimetric, NXOpen.View.ScaleAdjustment.Fit);
                                string ImagePath = "";
                                ImagePath = string.Format(@"{0}\{1}_{2}", Path.GetDirectoryName(weCompPart.FullPath), kvp.Key.comp.Name, WE_PRT_NAME);
                                theUfSession.Disp.CreateImage(ImagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);
                                #endregion

                                //輸出STL
                                CaxPart.ExportBinarySTL(newWeComp, 0.05, 0.05);
                            }
                            #endregion
                            
                        }
                    }
                    //return false;

                    WeListKey weGroupList = new WeListKey();
                    weGroupList.compName = newWeComp.Name.ToUpper();


                    Face[] NewBodyFaceAry = null;
                    NewBodyFaceAry = NewWEBody.GetFaces();
                    
                    for (int ii = 0; ii < NewBodyFaceAry.Length; ii++)
                    {
                        //CaxLog.ShowListingWindow(NewBodyFaceAry[ii].IsOccurrence.ToString());
                        try
                        {
                            string weSection = "";
                            string weWorkFace = "";
                            if (sCimAsmCompPart.electorde.Count != 0)
                            {
                                weSection = NewBodyFaceAry[ii].GetStringAttribute("CIM_EDM_WEDM");
                                try
                                {
                                    string BaseFaceAttr = NewBodyFaceAry[ii].GetStringAttribute("ELECTRODE");
                                    if (BaseFaceAttr == "BASE_FACE")
                                    { continue; }
                                }
                                catch (System.Exception ex)
                                { }
                                //weWorkFace = NewBodyFaceAry[ii].GetStringAttribute("CIM_EDM_FACE");
                            }
                            else
                            {
                                if (Task_Type == "3" || Is_fallmaterial == true/*SlopePin == "斜銷" || SlopePin == "滑塊入子" || Is_fallmaterial*/)
                                {
                                    try
                                    {
                                        weSection = NewBodyFaceAry[ii].GetStringAttribute("CIM_SECTION");
                                        weWorkFace = NewBodyFaceAry[ii].GetStringAttribute("CIM_WORK_FACE");
                                        if (weSection == "EDM1" && (weWorkFace == "T" || weWorkFace == "F" || weWorkFace == "R"))//未來如有CNC則加入判斷
                                        {
                                            weSection = NewBodyFaceAry[ii].GetStringAttribute("WE_SECTION");
                                            weWorkFace = NewBodyFaceAry[ii].GetStringAttribute("WE_WORK_FACE");
                                        }
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

                                        WeFaceGroup KeyComp = new WeFaceGroup();
                                        

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

                                        WeFaceGroup KeyComp = new WeFaceGroup();
                                        
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

                    //Thread.Sleep(0);

                    //產生工件圖
//                     workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Trimetric, NXOpen.View.ScaleAdjustment.Fit);
//                     string ImagePath = "";
//                     ImagePath = string.Format(@"{0}\{1}_{2}", Path.GetDirectoryName(weCompPart.FullPath), kvp.Key.comp.Name, WE_PRT_NAME);
//                     theUfSession.Disp.CreateImage(ImagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);

                    //將產生的線割檔加入隱藏列中，以便第二個檔案產生時可以隱藏
                    hideDispalyObject.Add(newWeComp);
                }
                #endregion

                
                List<Body> ListcompPrototype = new List<Body>();
                foreach (KeyValuePair<skeyFailed,string>kvp in Program.FailedSection)
                {
                    Part compPrototype = (Part)kvp.Key.comp.Prototype;
                    Body[] compPrototypeAry = compPrototype.Bodies.ToArray();
                    ListcompPrototype.AddRange(compPrototypeAry);

                    //foreach (Body i in ListcompPrototype)
                    //{
                    //    CaxLog.ShowListingWindow("failedbody:" + i);
                    //}

                    List<Body> intersectionOriginalBody = ListcompPrototype.Intersect(ListOriginalBody).ToList();
                    
                    if (intersectionOriginalBody.Count() > 0)
                    {
                        CaxAsm.SetWorkComponent(kvp.Key.comp);
                        foreach (Body i in intersectionOriginalBody)
                        {
                            //i.Highlight();
                            ListOriginalBody.Remove(i);
                            CaxPart.DeleteNXObject(i);
                        }
                        if (kvp.Key.section == "WECAM")
                        {
                            //CaxPart.DeleteNXObject(ListcompPrototype[0]);
                        }
                        CaxAsm.SetWorkComponent(null);
                    }
                }


                //return false;

                List<Body> intersectionCopyBody = ListcompPrototype.Intersect(ListCopyBody).ToList();
                if (intersectionCopyBody.Count() > 0)
                {
                    foreach (Body i in intersectionCopyBody)
                    {
                        ListCopyBody.Remove(i);
                        //CaxPart.DeleteNXObject(CaxWE.ListOriginalBody[CurrentRow]);
                    }
                }
                
                
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

                Dictionary<WeListKey, WeFaceGroup> bufWEFaceDict = new Dictionary<WeListKey, WeFaceGroup>();

                foreach (KeyValuePair<WeListKey, WeFaceGroup> kvp in WEFaceDict)
                {
                    WeFaceGroup sWeFaceGroupTemp;
                    WEFaceDict.TryGetValue(kvp.Key, out sWeFaceGroupTemp);
                    bufWEFaceDict[kvp.Key] = sWeFaceGroupTemp;

                    string[] allfaceAry = kvp.Value.faceGroup.Split(',');
                    string[] copyAllfaceAry = allfaceAry;

                    for (int i = 0; i < allfaceAry.Length; i++)
                    {
                        Face faceOcc = null;
                        CaxTransType.TagStrToFace(allfaceAry[i], out faceOcc);

                        WeFaceGroup sWeFaceGroup;
                        bufWEFaceDict.TryGetValue(kvp.Key, out sWeFaceGroup);
                        //WEFaceDict.TryGetValue(kvp.Key, out sWeFaceGroup);


                        if (i == 0)
                        {
                            //第一個面，直接加入
                            FaceGroupPnt sFaceGroupPnt;
                            sFaceGroupPnt.faceOccAry = new List<Face>();
                            sFaceGroupPnt.pnt_x = "0.0";
                            sFaceGroupPnt.pnt_y = "0.0";

                            List<Face> NeighborFaceAry = new List<Face>();
                            NeighborFaceAry.Add(faceOcc);
                            //faceOcc.Highlight();
                            GetWeNeighborFace(faceOcc, copyAllfaceAry, ref NeighborFaceAry, sCimAsmCompPart);
                            sFaceGroupPnt.faceOccAry.AddRange(NeighborFaceAry);

                            sWeFaceGroup.sFaceGroupPnt = new List<FaceGroupPnt>();
                            sWeFaceGroup.sFaceGroupPnt.Add(sFaceGroupPnt);
                            bufWEFaceDict[kvp.Key] = sWeFaceGroup;//test
                            //bufWEFaceDict.Add(kvp.Key, sWeFaceGroup);
                            //WEFaceDict[kvp.Key] = bufWEFaceDict[kvp.Key];//此處很有問題
                            //weFaceGroupAllList.Add(sFaceGroupPnt);
                            //return false;
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
                                    //bufWeFaceGroup.sFaceGroupPnt.Add(sFaceGroupPnt);
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
                //return false;
                //CaxPart.Save();
            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow(ex.ToString());
                CaxLog.ShowListingWindow("錯誤000");
                return false;
            }

            return true;
        }

        private static void XYMachAskReferPosi( double[] center, Body NewWEBody, ref Face BlendFace )
        {
            if (center[1] > 0 && center[2] < 0)
            {
                RotateObjectByX(NewWEBody, 180);
                BlendFace.SetAttribute("reference_posi", "TOP");
                ReferencePosi_False = "TOP";
            }
            else if (center[1] > 0 && center[2] > 0)
            {
                RotateObjectByX(NewWEBody, 180);
                BlendFace.SetAttribute("reference_posi", "BOT");
                ReferencePosi_False = "BOT";
            }
            else if (center[1] < 0 && center[2] > 0)
            {
                BlendFace.SetAttribute("reference_posi", "TOP");
                ReferencePosi_False = "TOP";
            }
            else if (center[1] < 0 && center[2] < 0)
            {
                BlendFace.SetAttribute("reference_posi", "BOT");
                ReferencePosi_False = "BOT";
            }
        }
        private static void ZMachAskReferPosi( double[] center, ref Face BlendFace, Body NewWEBody )
        {
            if (center[0] > 0 && center[1] < 0)//基準角在第四象限
            {
                //RotateObjectByZ(NewWEBody, 180);
                BlendFace.SetAttribute("reference_posi", "Z");
                ReferencePosi_False = "Z";
            }
            else if (center[0] > 0 && center[1] > 0)//基準角在第一象限，零件旋轉180
            {
                RotateObjectByZ(NewWEBody, 180);
                BlendFace.SetAttribute("reference_posi", "Z");
                ReferencePosi_False = "Z";
            }
            else if (center[0] < 0 && center[1] > 0)//基準角在第二象限，零件旋轉180
            {
                RotateObjectByZ(NewWEBody, 180);
                BlendFace.SetAttribute("reference_posi", "Z");
                ReferencePosi_False = "Z";
            }
            else if (center[0] < 0 && center[1] < 0)//基準角在第三象限
            {
                BlendFace.SetAttribute("reference_posi", "Z");
                ReferencePosi_False = "Z";
            }
        }

        private static bool IsFallMaterial(Body NewWEBody,out bool Is_fallmaterial)
        {
            Is_fallmaterial = false;
            try
            {
                Face[] NewWEBodyFaceAry = NewWEBody.GetFaces();
                
                foreach (Face tempFace in NewWEBodyFaceAry)
                {
                    string FaceColor = "";
                    try
                    {
                        FaceColor = tempFace.GetStringAttribute("MFG_COLOR");
                    }
                    catch (System.Exception ex)
                    {
                    	
                    }
                    if (FaceColor == "105")
                    {
                        Is_fallmaterial = true;
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

        public static void RecordFailed(ref List<DisplayableObject> hideDispalyObject, Component newWeComp, KeyValuePair<skey, string> kvp)
        {
            hideDispalyObject.Add(newWeComp);
            skeyFailed keyy = new skeyFailed();
            keyy.comp = newWeComp;
            keyy.compName = newWeComp.Name;
            keyy.section = kvp.Key.section;
            keyy.wkface = kvp.Key.wkface;
            Program.FailedSection.Add(keyy, kvp.Value);
        }
       
        private static bool GetSlopePinRotateAngle(Face tempFace, double[] Nor, out double dot_project, out double RotateAngle)
        {
            dot_project = 0;
            RotateAngle = 0;
            double[] dir = new double[3];
            try
            {
                AskFaceDir(tempFace, out dir);
                theUfSession.Vec3.Dot(dir, Nor, out dot_project);
                double[] minWCS001 = new double[3];
                double[] maxWCS001 = new double[3];
                double[] Xdir = new double[3];
                double[] Ydir = new double[3];
                double x2_x1, y2_y1;
                CaxPart.AskBoundingBoxExactByWCS(tempFace.Tag, out minWCS001, out maxWCS001);
                if (Nor[0]==1)
                {
                    y2_y1 = maxWCS001[1] - minWCS001[1];
                    x2_x1 = maxWCS001[0] - minWCS001[0];
                }
                else
                {
                    y2_y1 = maxWCS001[2] - minWCS001[2];
                    x2_x1 = maxWCS001[0] - minWCS001[0];
                }
                RotateAngle = Math.Atan(y2_y1 / x2_x1) * 180 / Math.PI;
                RotateAngle = Math.Round(RotateAngle, 3);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private static bool GetOffsetFace(List<Face> ExtrudeBodyFaceAry, List<Face> ListFaceFromNewWEBody, List<Face> ListOffsetFace, ref List<Face> ListOffsetFaceFromExtrudeBody)
        {
            try
            {
                foreach (Face tempOffsetFace in ExtrudeBodyFaceAry)
                {
                    double[] center = new double[3];
                    double[] dir = new double[3];
                    AskFaceCenter(tempOffsetFace, out center, out dir);
                    UFModl.RayHitPointInfo cRayHitPointInfo;
                    CLA101_CSUGFunc.detectPointOnSurface(center, ListFaceFromNewWEBody, out cRayHitPointInfo);
                    Face OffsetFaceFromNewWEBody = (Face)NXObjectManager.Get(cRayHitPointInfo.hit_face);
                    try
                    {
                        string Tol_attr = OffsetFaceFromNewWEBody.GetStringAttribute("TOL_COLOR");
                        tempOffsetFace.SetAttribute("TOL_COLOR", Tol_attr);
                    }
                    catch (System.Exception ex)
                    { }

                    foreach (Face temp in ListOffsetFace)
                    {
                        if (OffsetFaceFromNewWEBody == temp)
                        {
                            int summ = 0;
                            for (int i = 0; i < ListOffsetFaceFromExtrudeBody.Count; i++)
                            {
                                if (tempOffsetFace != ListOffsetFaceFromExtrudeBody[i])
                                {
                                    summ++;
                                }
                            }
                            if (summ == ListOffsetFaceFromExtrudeBody.Count)
                            {
                                ListOffsetFaceFromExtrudeBody.Add(tempOffsetFace);
                            }
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
        
        private static bool MoveWorkPiece(Body NewWEBody)
        {
            try
            {
                double[] WPmin1 = new double[3];
                double[] WPmax1 = new double[3];
                Point3d firstpnt = new Point3d();
                Point3d secondpnt = new Point3d(0.0,0.0,0.0);
                CaxPart.AskBoundingBoxExactByWCS(NewWEBody.Tag, out WPmin1, out WPmax1);
                firstpnt.X = (WPmax1[0] + WPmin1[0]) / 2;
                firstpnt.Y = (WPmax1[1] + WPmin1[1]) / 2;
                firstpnt.Z = WPmin1[2];
                NewMoveObjectByPntToPnt(NewWEBody, firstpnt, secondpnt);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private static bool J_CreateSheetBody(Body NewWEBody,out Body PlaneBody)
        {
            PlaneBody = null;
            bool status = false;
            try
            {
                double[] minWCS0 = new double[3];
                double[] maxWCS0 = new double[3];
                CaxPart.AskBoundingBoxExactByWCS(NewWEBody.Tag, out minWCS0, out maxWCS0);

                minWCS0[0] = minWCS0[0] - 10;
                minWCS0[1] = minWCS0[1] - 10;
                maxWCS0[0] = maxWCS0[0] + 10;
                maxWCS0[1] = maxWCS0[1] + 10;

                status = J_CreateSheetBodyOnZPlane(minWCS0[0], minWCS0[1], maxWCS0[0], maxWCS0[1], out PlaneBody);
                if (!status)
                {
                    //CaxLog.ShowListingWindow("J_CreateSheetBodyOnZPlane 失敗");
                    return false;
                }
                RemoveParameters(PlaneBody);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private static bool GetAroundFaceFromBBox(Tag theBlock, out List<Tag> ListAroundBboxFace)
        {
            ListAroundBboxFace = new List<Tag>();
            try
            {
                CaxGeom.FaceData sFaceData;
                Body Bbox_Body = (Body)NXObjectManager.Get(theBlock);
                Face[] Bbox_Body_FaceAry = Bbox_Body.GetFaces();
                double[] sFaceData_dir = new double[3];
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
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private static bool CreateBBoxFromInnerLoop(Face[] NewWEBodyFaceAry, out List<Face> NeiFace, out Tag theBlock,out NXObject theBlockObj)
        {
            NeiFace = new List<Face>();
            theBlock = Tag.Null;
            theBlockObj = null;
            try
            {
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
                            theBlock = CreateWrapBlock(LoopFaceTagAry);
                            theBlockObj = (NXObject)NXObjectManager.Get(theBlock);
                            //List_Feat_Obj.Add(theBlockObj);//待確認是否正確
                        }
                    }
                    catch (System.Exception ex)
                    { }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private static bool GetCuttedFaceToWing(Face[] NewWEBodyFaceAry, out List<Face> BaseFaces)
        {
            BaseFaces = new List<Face>();
            try
            {
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
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private static bool RotateEDM(bool IsZMachining, ref Face basef, ref CaxGeom.FaceData basefData, ref Face zf)
        {
            try
            {
                if (!IsZMachining)
                {
                    CaxGeom.GetFaceData(basef.Tag, out basefData);
                }
                Vector3d firstFace = new Vector3d(basefData.dir[0], basefData.dir[1], basefData.dir[2]);
                CaxGeom.FaceData zfData = new CaxGeom.FaceData();
                CaxGeom.GetFaceData(zf.Tag, out zfData);
                Point3d origin1 = new Point3d(0.0, 0.0, 0.0);

                Vector3d secondFace = new Vector3d(zfData.dir[0], zfData.dir[1], zfData.dir[2]);
                Vector3d xDirection = new Vector3d(1.0, 0.0, 0.0);
                Vector3d zDirection = new Vector3d(0.0, 0.0, 1.0);

                MoveObject(basef.GetBody(), origin1, firstFace, secondFace, origin1, xDirection, zDirection);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private static bool MoveEDM(Face[] NewWEBodyFaceAry, Component newWeComp,
            KeyValuePair<skey, string> kvp, string section, out Face basef, out Face zf, out CaxGeom.FaceData basefData, out bool IsCreatWing, out bool IsZMachining)
        {
            basef = null;
            zf = null;
            IsCreatWing = false;
            IsZMachining = false;
            basefData = new CaxGeom.FaceData();
            try
            {
                for (int i = 0; i < NewWEBodyFaceAry.Length; i++)
                {
                    try
                    {
                        string basef_attr = NewWEBodyFaceAry[i].GetStringAttribute("ELECTRODE");
                        basef = NewWEBodyFaceAry[i];

                        #region 平移
                        if (basef_attr == "BASE_FACE")
                        {
                            try
                            {
                                string basefacecolor = basef.GetStringAttribute("MFG_COLOR");
                                string sectionAry = basef.GetStringAttribute("CIM_EDM_WEDM");
                                string[] sectionArySplit = sectionAry.Split(',');
                                foreach (string singleSection in sectionArySplit)
                                {
                                    if (singleSection == section && (basefacecolor == "213" || basefacecolor == "214" || basefacecolor == "215"))
                                    {
                                        IsCreatWing = true;
                                    }
                                }
//                                 if (basefacecolor == "213" || basefacecolor == "214" || basefacecolor == "215")
//                                 {
//                                     IsCreatWing = true;
//                                 }
                            }
                            catch (System.Exception ex)
                            { }

                            //這邊處理Z方向加工
                            try
                            {
                                string edm_face = basef.GetStringAttribute("CIM_EDM_FACE");
                                string edm_wedm = basef.GetStringAttribute(CIM_EDM_WEDM);
                                CaxPart.RefCornerFace sRefCornerFace1;
                                CaxPart.GetBaseCornerFaceAry(newWeComp, out sRefCornerFace1);
                                if (edm_face == kvp.Key.wkface && edm_wedm == kvp.Key.section)
                                {
                                    zf = basef;
                                    basef = sRefCornerFace1.faceA;
                                    CFace GetFaceNor1 = new CFace();
                                    CFace.CFaceData aa1 = GetFaceNor1.getFacedata(sRefCornerFace1.faceA.Tag);
                                    CaxGeom.GetFaceData(sRefCornerFace1.faceA.Tag, out basefData);
                                    IsZMachining = true;
                                }
                            }
                            catch (System.Exception ex)
                            { }


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
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private static bool HideAddComp(ref List<DisplayableObject> hideDispalyObject, CaxAsm.CimAsmCompPart sCimAsmCompPart, Dictionary<skey, string> sectionFaceDic)
        {
            try
            {
                hideDispalyObject.Add(sCimAsmCompPart.design.comp);
                hideDispalyObject.Add(sCimAsmCompPart.electorde_all.comp);
                hideDispalyObject.Add(sCimAsmCompPart.product.comp);
                hideDispalyObject.Add(sCimAsmCompPart.fixture.comp);
                for (int i = 0; i < sCimAsmCompPart.electorde.Count; i++)
                {
                    hideDispalyObject.Add(sCimAsmCompPart.electorde[i].comp);
                }
                foreach (KeyValuePair<skey, string> kvp1 in sectionFaceDic)
                {
                    Tag hideOlderCompTag = (Tag)Convert.ToInt32(kvp1.Value);
                    Component hideOlderComp = (Component)NXObjectManager.Get(hideOlderCompTag);
                    hideDispalyObject.Add(hideOlderComp);
                }
                theSession.DisplayManager.BlankObjects(hideDispalyObject.ToArray());
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private static bool CreateNewWEComp(Component comp,string NewCompFullPath, Body OlderBody, out Component newWeComp, out Body NewWEBody)
        {
            NewWEBody = null;
            newWeComp = null;
            try
            {
                bool status = false;
                CaxAsm.SetWorkComponent(null);
                CaxPart.CloseSelectedParts(NewCompFullPath);
                
                status = CaxAsm.CreateNewEmptyComp(NewCompFullPath, out newWeComp);
                if (!status)
                {
                    return false;
                }

                CaxAsm.SetWorkComponent(newWeComp);

                NXOpen.Features.Feature wavelink_feat;
                //CaxLog.ShowListingWindow("OlderBodyisOCC:" + OlderBody.IsOccurrence);
                status = WAVEGeometry(comp, OlderBody, out wavelink_feat);
                //status = WaveLinkBody(OlderBody, out wavelink_feat);
                if (!status)
                {
                    return false;
                }
                CaxFeat.GetFeatBody(wavelink_feat, out NewWEBody);
                RemoveParameters(NewWEBody);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
           
        }

        private static bool CreateWePartASM(string Mold_Part_ASM, string Mold_Part_ASM_Path,out Component Mold_Part_Comp)
        {
            bool status = false;
            Mold_Part_Comp = null;
            try
            {
                List<CaxAsm.CompPart> AsmCompAry;
                CaxAsm.GetAllAsmCompTree(out AsmCompAry);

                Dictionary<string, string> AsmCompName = new Dictionary<string, string>();
                foreach (CaxAsm.CompPart compname in AsmCompAry)
                {
                    //CaxLog.ShowListingWindow("compname:" + compname.componentOcc.Name);
                    string Attr = "";
                    status = AsmCompName.TryGetValue(compname.componentOcc.Name, out Attr);
                    if (!status)
                    {
                        AsmCompName.Add(compname.componentOcc.Name, "1");
                    }
                }

                status = AsmCompName.ContainsKey(Mold_Part_ASM);
                if (!status)
                {
                    CaxLog.ShowListingWindow("Mold_Part_ASM_Path : " + Mold_Part_ASM_Path);
                    status = CaxAsm.CreateNewEmptyComp(Mold_Part_ASM_Path, out Mold_Part_Comp);
                    if (!status)
                    {
                        CaxLog.ShowListingWindow("123 : " );
                        return false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private static bool GetFaceNor(Face tempFace,out double[] FaceNor)
        {
            FaceNor = new double[3];
            try
            {
                CFace FaceToGetNor = new CFace();
                FaceNor = FaceToGetNor.GetNormal(tempFace.Tag);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private static bool CreateLineFromIntersectionEdge(List<Edge> IntersectionEdge,out List<NXObject> ListLineObj)
        {
            ListLineObj = new List<NXObject>();
            try
            {
                NXObject nxobj;
                UGPUBLIC_1000_PublicFunc cUGPUBLIC_1000_PublicFunc = new UGPUBLIC_1000_PublicFunc();
                Point3d FirstPt, SecondPt;
                foreach (Edge tempEdge in IntersectionEdge)
                {
                    tempEdge.GetVertices(out FirstPt, out SecondPt);
                    if (FirstPt.Z > SecondPt.Z)
                    {
                        FirstPt.Z = FirstPt.Z + 30;
                        SecondPt.Z = SecondPt.Z - 30;
                    }
                    else
                    {
                        FirstPt.Z = FirstPt.Z - 30;
                        SecondPt.Z = SecondPt.Z + 30;
                    }

                    double[] firstpt = new double[3] { FirstPt.X, FirstPt.Y, FirstPt.Z };
                    double[] secondpt = new double[3] { SecondPt.X, SecondPt.Y, SecondPt.Z };
                    Feature _NullFeat = null;
                    cUGPUBLIC_1000_PublicFunc.createLineVia2Points(firstpt, secondpt, out nxobj, out _NullFeat);
                    ListLineObj.Add(nxobj);
                    //break;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }
        
        private static bool GetIntersectionEdgeAndFaceToDivideFromMaxAreaFace(List<Face> ListMaxAreaFace, 
                                                                                                                                   out List<Edge> IntersectionEdge, 
                                                                                                                                   out List<Face> ListFaceToDivide)
        {
            IntersectionEdge = new List<Edge>();
            ListFaceToDivide = new List<Face>();
            try
            {
                Face FaceToDivide = null;
                foreach (Face tempFace in ListMaxAreaFace)
                {
                    Edge[] FaceEdgeAry = tempFace.GetEdges();
                    foreach (Edge tempEdge in FaceEdgeAry)
                    {
                        FaceToDivide = CaxGeom.GetNeighborFace(tempFace, tempEdge);
                        string FaceToDivideSection = "";
                        string FaceToDivideColor = "";
                        try
                        {
                            FaceToDivideSection = FaceToDivide.GetStringAttribute("WE_SECTION");
                        }
                        catch (System.Exception ex)
                        {
                            FaceToDivideColor = FaceToDivide.GetStringAttribute("MFG_COLOR");
                            if (FaceToDivideColor == "213" || FaceToDivideColor == "214" || FaceToDivideColor == "215" || FaceToDivideColor == "105")
                            { continue; }
                        }

                        if (FaceToDivideSection == "WECAM" || (FaceToDivideColor != "213" && FaceToDivideColor != "214" && FaceToDivideColor != "215" && FaceToDivideColor != "105"))
                        {
                            IntersectionEdge.Add(tempEdge);
                            ListFaceToDivide.Add(FaceToDivide);
                            break;
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

        private static bool GetMPFaceMaxArea(List<Face> ListWEFaceFromNewWEBody, out Face MaxAreaFace)
        {
            MaxAreaFace = null;
            try
            {
                Dictionary<string, string> FaceArea = new Dictionary<string, string>();
                List<double> ListFaceArea = new List<double>();
                foreach (Face TempFace in ListWEFaceFromNewWEBody)
                {
                    string tempFaceColor = "";
                    try
                    {
                        tempFaceColor = TempFace.GetStringAttribute("MFG_COLOR");
                    }
                    catch (System.Exception ex)
                    {
                        continue;
                    }
                    
                    if (tempFaceColor != "105")
                    {
                        continue;
                    }

                    MeasureFaces cMeasureFaces;
                    AskFaceAreaAndPerimeter(TempFace, out cMeasureFaces);
                    ListFaceArea.Add(cMeasureFaces.Area);

                    bool chk_area;
                    string FaceString = "";
                    chk_area = FaceArea.TryGetValue(cMeasureFaces.Area.ToString(), out FaceString);
                    if (chk_area)
                    {
                        string merge = FaceString + "," + TempFace.Tag.ToString();
                        FaceArea[cMeasureFaces.Area.ToString()] = merge;
                    }
                    else
                    {
                        FaceArea[cMeasureFaces.Area.ToString()] = TempFace.Tag.ToString();
                    }
                }

                ListFaceArea.Sort();
                
                string FaceStringAry = "";
                FaceArea.TryGetValue(ListFaceArea[ListFaceArea.Count - 1].ToString(), out FaceStringAry);
                string[] SplitFaceStringAry = FaceStringAry.Split(',');
                for (int i = 0; i < SplitFaceStringAry.Length; i++)
                {
                    Tag FaceTag = (Tag)Convert.ToInt32(SplitFaceStringAry[i]);
                    MaxAreaFace = (Face)NXObjectManager.Get(FaceTag);
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private static bool GetWEFaceMaxArea(List<Face> ListWEFaceFromNewWEBody, out List<Face> ListMaxAreaFace)
        {
            ListMaxAreaFace = new List<Face>();
            try
            {
                Dictionary<string, string> FaceArea = new Dictionary<string, string>();
                List<double> ListFaceArea = new List<double>();
                foreach (Face TempFace in ListWEFaceFromNewWEBody)
                {
                    MeasureFaces cMeasureFaces;
                    AskFaceAreaAndPerimeter(TempFace, out cMeasureFaces);
                    ListFaceArea.Add(cMeasureFaces.Area);

                    bool chk_area;
                    string FaceString = "";
                    chk_area = FaceArea.TryGetValue(cMeasureFaces.Area.ToString(), out FaceString);
                    if (chk_area)
                    {
                        string merge = FaceString + "," + TempFace.Tag.ToString();
                        FaceArea[cMeasureFaces.Area.ToString()] = merge;
                    }
                    else
                    {
                        FaceArea[cMeasureFaces.Area.ToString()] = TempFace.Tag.ToString();
                    }
                }

                ListFaceArea.Sort();

                string FaceStringAry = "";
                FaceArea.TryGetValue(ListFaceArea[ListFaceArea.Count - 1].ToString(), out FaceStringAry);
                string[] SplitFaceStringAry = FaceStringAry.Split(',');
                for (int i = 0; i < SplitFaceStringAry.Length; i++)
                {
                    Tag FaceTag = (Tag)Convert.ToInt32(SplitFaceStringAry[i]);
                    Face MaxAreaFace = (Face)NXObjectManager.Get(FaceTag);
                    ListMaxAreaFace.Add(MaxAreaFace);
                    //MaxAreaFace.Highlight();
                    if (SplitFaceStringAry.Length == 1 && PingType.StringPingType == "2")
                    {
                        FaceArea.TryGetValue(ListFaceArea[ListFaceArea.Count - 2].ToString(), out FaceStringAry);
                        FaceTag = (Tag)Convert.ToInt32(FaceStringAry);
                        MaxAreaFace = (Face)NXObjectManager.Get(FaceTag);
                        ListMaxAreaFace.Add(MaxAreaFace);
                        //MaxAreaFace.Highlight();
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private static bool GetWEFace(List<Face> ListFaceFromNewWEBody, out List<Face> ListWEFaceFromNewWEBody)
        {
            ListWEFaceFromNewWEBody = new List<Face>();
            try
            {
                foreach (Face TempFace in ListFaceFromNewWEBody)
                {
                    string FaceColor = "";
                    try
                    {
                        FaceColor = TempFace.GetStringAttribute("MFG_COLOR");
                        if (FaceColor == "213" || FaceColor == "214" || FaceColor == "215")
                        {
                            ListWEFaceFromNewWEBody.Add(TempFace);
                        }
                    }
                    catch (System.Exception ex)
                    { continue; }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private static bool InnerOuterAndRefPosi(Component newWeComp, WorkPiece WP1, out string outer_inner1, out string reference_posi1)
        {
            outer_inner1 = "";
            reference_posi1 = "";
            try
            {
                Body NewWEBody;
                CaxPart.GetLayerBody(newWeComp, out NewWEBody);

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
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }

        private static bool GetSectionFaceDic( List<CaxAsm.CompPart> AsmCompAry,CaxAsm.CimAsmCompPart sCimAsmCompPart, out Dictionary<skey, string> sectionFaceDic)
        {
            sectionFaceDic = new Dictionary<skey, string>();

            try
            {
                bool status;

                //取得有線割面的零件(設計零件&電極)
                Dictionary<string, Component> weCompDic = new Dictionary<string, Component>();
                status = GetWeCompDic(AsmCompAry, sCimAsmCompPart, out weCompDic);
                if (!status)
                {
                    return false;
                }
                if (weCompDic.Count == 0)
                {
                    return false;
                }

                #region 將製程色、公差色塞回零件檔屬性欄中，並儲存檔案
                //將製程色塞回零件檔屬性欄中，並儲存檔案
                Body weCompBodyOcc = null;
                foreach (KeyValuePair<string, Component> weComp in weCompDic)
                {
                    status = CaxPart.GetLayerBody(weComp.Value, out weCompBodyOcc);
                    if (!status)
                    {
                        return false;
                    }

                    Face[] ssBodyFaceAry = weCompBodyOcc.GetFaces();
                    for (int ii = 0; ii < ssBodyFaceAry.Length; ii++)
                    {
                        if (ssBodyFaceAry[ii].Color == 215 || ssBodyFaceAry[ii].Color == 213 || ssBodyFaceAry[ii].Color == 214 || ssBodyFaceAry[ii].Color == 105)
                        {
                            Face ssBodyFaceProto = (Face)ssBodyFaceAry[ii].Prototype;
                            string tolColor = ssBodyFaceProto.Color.ToString();

                            //塞製程色
                            ssBodyFaceProto.SetAttribute("MFG_COLOR", ssBodyFaceAry[ii].Color.ToString());

                            //塞公差色
                            ssBodyFaceProto.SetAttribute("TOL_COLOR", tolColor);
                        }
                    }  

                    
                }
                CaxPart.Save();

                #endregion

                Body compBodyOcc = null;
                foreach (KeyValuePair<string, Component> kvp in weCompDic) //欲執行的線割零件
                {
                    status = CaxPart.GetLayerBody(kvp.Value, out compBodyOcc);
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
                                        compBodyFaceOcc.Highlight();
                                        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "有上線割色卻無加工面，請確認是否有指定加工面或工段");
                                        return false;
                                    }
                                }
                                else
                                { continue; }
                            }

                            string[] ssBodyFaceSecAry = ssBodyFaceAttr_Sec.Split(',');
                            foreach (string ssBodyFaceSec in ssBodyFaceSecAry)
                            {
                                weFaceKey.comp = kvp.Value;
                                weFaceKey.section = ssBodyFaceSec;
                                weFaceKey.wkface = ssBodyFaceAttr_Wkf;

                                bool chk;
                                chk = sectionFaceDic.ContainsKey(weFaceKey);
                                if (!chk)
                                {
                                    sectionFaceDic.Add(weFaceKey, kvp.Value.Tag.ToString());
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

        private static bool GetWeCompDic(List<CaxAsm.CompPart> AsmCompAry, CaxAsm.CimAsmCompPart sCimAsmCompPart, out Dictionary<string, Component> weCompDic)
        {
            weCompDic = new Dictionary<string, Component>();
            try
            {
                bool status;
                bool chk_key = false;

                if (sCimAsmCompPart.electorde.Count != 0)
                {
                    Body elecBodyOcc = null;
                    for (int i = 0; i < sCimAsmCompPart.electorde.Count; i++)
                    {
                        //取得電極是否有做過的屬性
                        string WE_FINISHED = "";
                        try
                        {
                            WE_FINISHED = sCimAsmCompPart.electorde[i].comp.GetStringUserAttribute("WE_FINISHED", 0);
                        }
                        catch (System.Exception ex)
                        { }

                        if (WE_FINISHED == "Y")
                        {
                            continue;
                        }

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
                                    weCompDic.Add(sCimAsmCompPart.electorde[i].comp.Name, sCimAsmCompPart.electorde[i].comp);
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //取得工件第一層實體
                    Body designBodyOcc = null;
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
                                weCompDic.Add(sCimAsmCompPart.design.comp.Name, sCimAsmCompPart.design.comp);
                            }
                            break;
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

        private static bool GetColorDataAry(out Dictionary<int, string> mdColorDic)
        {
            bool status;

            mdColorDic = new Dictionary<int, string>();

            try
            {
                bool chk_key = false;
                string tolStr = "";
                List<CimforceCaxTwMD.CaxMD.ColorExcel> colorExcelAry = new List<CimforceCaxTwMD.CaxMD.ColorExcel>();
                //CaxLog.ShowListingWindow("1");
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
                //CaxLog.ShowListingWindow("2");
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
                //CaxLog.ShowListingWindow("3");
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
                //CaxLog.ShowListingWindow("4");
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
                string FaceColor = face.GetStringAttribute("MFG_COLOR");

                if (sCimAsmCompPart.electorde.Count != 0)//電極
                {
                    Edge[] edgeAry = face.GetEdges();
                    //CaxLog.ShowListingWindow("edgeAry.Length : " + edgeAry.Length);
                    for (int j = 0; j < edgeAry.Length; j++)
                    {
                        //edgeAry[j].Highlight();
                        neighborFace = null;
                        neighborFace = CaxGeom.GetNeighborFace(face, edgeAry[j]);
                        if (neighborFace == null)
                        { continue; }
                        //neighborFace.Highlight();
                        //                     CaxLog.ShowListingWindow("neighborFace : " + neighborFace.Tag.ToString());

                        try
                        {
                            bool chk_face_str = false;
                            foreach (string loopFace in copyAllfaceAry)
                            {
                                //neighborFace.Highlight();
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
                                //neighborFace.Highlight();
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
                    Face faceproto = (Face)face.Prototype;
                    string faceMachingType = "";
                    try
                    {
                        faceMachingType = face.GetStringAttribute("WE_TYPE");
                    }
                    catch (System.Exception ex)
                    {
                        faceMachingType = face.GetStringAttribute("MACHING_TYPE");
                    }
                    
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
                                string NeiFaceMachingType = "";
                                try
                                {
                                    NeiFaceMachingType = neighborFace.GetStringAttribute("WE_TYPE");
                                }
                                catch (System.Exception ex)
                                {
                                    NeiFaceMachingType = neighborFace.GetStringAttribute("MACHING_TYPE");
                                }
                                string NeiFaceMFG_COLOR = neighborFace.GetStringAttribute("MFG_COLOR");
                                if (NeiFaceMachingType == faceMachingType && NeiFaceMFG_COLOR == FaceColor)
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
                CaxLog.ShowListingWindow(ex.ToString());
                CaxLog.ShowListingWindow("錯誤002");
                return false;
            }
            //return false;
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
                //CaxLog.ShowListingWindow(ex.ToString());
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
                CaxLog.ShowListingWindow(ex.ToString());
                return false;
            }
            return true;
        }

        public static Tag CreateWrapBlock(Tag[] objects)
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

        public static bool DeleteBodyObj(NXObject BodyObj)
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
                //CaxLog.ShowListingWindow("type:" + type);
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
                CaxLog.ShowListingWindow(ex.ToString());
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
                //CaxLog.ShowListingWindow(ex.ToString());
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
                //CaxPart.ShowListingWindow(ex.ToString());
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

        public static bool CombineBody(Body firstbody, Body secondbody, out Body newbody)
        {
            newbody = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Combine->Unite...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

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

                theSession.SetUndoMarkName(markId1, "##12Unite Dialog");

                Body body1 = (Body)workPart.Bodies.FindObject(firstbody.JournalIdentifier);
                bool added1;
                added1 = booleanBuilder1.Targets.Add(body1);

                TaggedObject[] targets1 = new TaggedObject[1];
                targets1[0] = body1;
                booleanRegionSelect1.AssignTargets(targets1);

                ScCollector scCollector2;
                scCollector2 = workPart.ScCollectors.CreateCollector();

                Body[] bodies1 = new Body[1];
                Body body2 = (Body)workPart.Bodies.FindObject(secondbody.JournalIdentifier);
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

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "##12Unite");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "##12Unite");

                NXObject nXObject1;
                nXObject1 = booleanBuilder1.Commit();
                BooleanFeature a = (BooleanFeature)nXObject1;
                Body[] b = a.GetBodies();
                if (b.Length > 0)
                {
                    newbody = b[0];
                }
                else
                {
                    return false;
                }
                /*CaxLog.ShowListingWindow(b.Length.ToString());*/
                //CaxLog.ShowListingWindow(nXObject1.GetType().ToString());
                //newbody = (Body)nXObject1;


                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "##12Unite");

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

        public static bool J_OffsetFace(Body body,Face[] offsetface,string offsetvalue)
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

                offsetFaceBuilder1.Distance.RightHandSide = "0.05";

                theSession.SetUndoMarkName(markId1, "Offset Face Dialog");

                NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject(body.JournalIdentifier);
//                 Face face1 = (Face)brep1.FindObject("FACE 18 {(46.3488007880139,-2.2604632931332,1.975) UNPARAMETERIZED_FEATURE(1)}");
//                 Face[] boundaryFaces1 = new Face[0];
//                 FaceTangentRule faceTangentRule1;
//                 faceTangentRule1 = workPart.ScRuleFactory.CreateRuleFaceTangent(face1, boundaryFaces1, 0.01);
// 
//                 Face face2 = (Face)brep1.FindObject("FACE 17 {(47.3736980994661,-0.0249657383689,1.975) UNPARAMETERIZED_FEATURE(1)}");
//                 Face[] boundaryFaces2 = new Face[0];
//                 FaceTangentRule faceTangentRule2;
//                 faceTangentRule2 = workPart.ScRuleFactory.CreateRuleFaceTangent(face2, boundaryFaces2, 0.01);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[offsetface.Length];
                for (int i = 0; i < offsetface.Length;i++ )
                {
                    Face face = offsetface[i];
                    Face[] boundaryFaces = new Face[0];
                    FaceTangentRule faceTangentRule;
                    faceTangentRule = workPart.ScRuleFactory.CreateRuleFaceTangent(face, boundaryFaces, 0.01);
                    rules1[i] = faceTangentRule;
                }

                //SelectionIntentRule[] rules1 = new SelectionIntentRule[2];
                //rules1[0] = faceTangentRule1;
                //rules1[1] = faceTangentRule2;
                offsetFaceBuilder1.FaceCollector.ReplaceRules(rules1, false);

                offsetFaceBuilder1.Distance.RightHandSide = offsetvalue;

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

        public static bool J_CreateSheetBodyOnZPlane(double minX,double minY,double maxX,double maxY,out Body PlaneBody)
        {
            PlaneBody = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Design Feature->Extrude...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

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

                extrudeBuilder1.Limits.StartExtend.Value.RightHandSide = "0";

                extrudeBuilder1.Limits.EndExtend.Value.RightHandSide = "25";

                extrudeBuilder1.Offset.StartOffset.RightHandSide = "0";

                extrudeBuilder1.Offset.EndOffset.RightHandSide = "5";

                extrudeBuilder1.Limits.StartExtend.Value.RightHandSide = "0";

                extrudeBuilder1.Limits.EndExtend.Value.RightHandSide = "5";

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

                theSession.SetUndoMarkName(markId1, "Extrude Dialog");

                section1.DistanceTolerance = 0.002;

                section1.ChainingTolerance = 0.0019;

                section1.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Enter Sketch");

                theSession.BeginTaskEnvironment();

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                Sketch nullSketch = null;
                SketchInPlaceBuilder sketchInPlaceBuilder1;
                sketchInPlaceBuilder1 = workPart.Sketches.CreateNewSketchInPlaceBuilder(nullSketch);

                Unit unit2;
                unit2 = extrudeBuilder1.Offset.StartOffset.Units;

                Expression expression2;
                expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

                Expression expression3;
                expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

                SketchAlongPathBuilder sketchAlongPathBuilder1;
                sketchAlongPathBuilder1 = workPart.Sketches.CreateSketchAlongPathBuilder(nullSketch);

                sketchAlongPathBuilder1.PlaneLocation.Expression.RightHandSide = "0";

                theSession.SetUndoMarkName(markId3, "Create Sketch Dialog");

                // ----------------------------------------------
                //   Dialog Begin Create Sketch
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Create Sketch");

                theSession.DeleteUndoMark(markId4, null);

                NXOpen.Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Create Sketch");

                theSession.Preferences.Sketch.CreateInferredConstraints = true;

                theSession.Preferences.Sketch.ContinuousAutoDimensioning = false;

                theSession.Preferences.Sketch.DimensionLabel = NXOpen.Preferences.SketchPreferences.DimensionLabelType.Expression;

                theSession.Preferences.Sketch.TextSizeFixed = true;

                theSession.Preferences.Sketch.FixedTextSize = 3.0;

                theSession.Preferences.Sketch.ConstraintSymbolSize = 3.0;

                theSession.Preferences.Sketch.DisplayObjectColor = false;

                NXObject nXObject1;
                nXObject1 = sketchInPlaceBuilder1.Commit();
                
                Sketch sketch1 = (Sketch)nXObject1;
                NXOpen.Features.Feature feature1;
                feature1 = sketch1.Feature;

                NXOpen.Session.UndoMarkId markId6;
                markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "update");

                int nErrs1;
                nErrs1 = theSession.UpdateManager.DoUpdate(markId6);

                sketch1.Activate(NXOpen.Sketch.ViewReorient.True);

                theSession.DeleteUndoMark(markId5, null);

                theSession.SetUndoMarkName(markId3, "Create Sketch");

                sketchInPlaceBuilder1.Destroy();

                sketchAlongPathBuilder1.Destroy();

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
                    workPart.Expressions.Delete(expression2);
                }
                catch (NXException ex)
                {
                    ex.AssertErrorCode(1050029);
                }

                theSession.DeleteUndoMarksUpToMark(markId3, null, true);

                NXOpen.Session.UndoMarkId markId7;
                markId7 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Open Sketch");

                theSession.ActiveSketch.SetName("SKETCH_000");

                NXOpen.Session.UndoMarkId markId8;
                markId8 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Profile short list");

                // ----------------------------------------------
                //   Dialog Begin Profile
                // ----------------------------------------------
                // ----------------------------------------------
                //   Menu: Insert->Curve->Rectangle...
                // ----------------------------------------------
                theSession.DeleteUndoMark(markId8, "Curve");

                NXOpen.Session.UndoMarkId markId9;
                markId9 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Profile short list");

                NXOpen.Session.UndoMarkId markId10;
                markId10 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Create Rectangle");

                theSession.SetUndoMarkVisibility(markId10, "Create Rectangle", NXOpen.Session.MarkVisibility.Visible);

                // ----------------------------------------------
                // Creating rectangle using By 2 Points method 
                // ----------------------------------------------
                //Point3d startPoint1 = new Point3d(-80.0, 50.0, 0.0);
                //Point3d endPoint1 = new Point3d(80.0, 50.0, 0.0);
                Point3d startPoint1 = new Point3d(minX, maxY, 0.0);
                Point3d endPoint1 = new Point3d(maxX, maxY, 0.0);
                Line line1;
                line1 = workPart.Curves.CreateLine(startPoint1, endPoint1);

                //Point3d startPoint2 = new Point3d(80.0, 50.0, 0.0);
                //Point3d endPoint2 = new Point3d(80.0, -50.0, 0.0);
                Point3d startPoint2 = new Point3d(maxX, maxY, 0.0);
                Point3d endPoint2 = new Point3d(maxX, minY, 0.0);
                Line line2;
                line2 = workPart.Curves.CreateLine(startPoint2, endPoint2);

                //Point3d startPoint3 = new Point3d(80.0, -50.0, 0.0);
                //Point3d endPoint3 = new Point3d(-80.0, -50.0, 0.0);
                Point3d startPoint3 = new Point3d(maxX, minY, 0.0);
                Point3d endPoint3 = new Point3d(minX, minY, 0.0);
                Line line3;
                line3 = workPart.Curves.CreateLine(startPoint3, endPoint3);

                //Point3d startPoint4 = new Point3d(-80.0, -50.0, 0.0);
                //Point3d endPoint4 = new Point3d(-80.0, 50.0, 0.0);
                Point3d startPoint4 = new Point3d(minX, minY, 0.0);
                Point3d endPoint4 = new Point3d(minX, maxY, 0.0);
                Line line4;
                line4 = workPart.Curves.CreateLine(startPoint4, endPoint4);

                theSession.ActiveSketch.AddGeometry(line1, NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);

                theSession.ActiveSketch.AddGeometry(line2, NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);

                theSession.ActiveSketch.AddGeometry(line3, NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);

                theSession.ActiveSketch.AddGeometry(line4, NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);

                NXOpen.Sketch.ConstraintGeometry geom1_1;
                geom1_1.Geometry = line1;
                geom1_1.PointType = NXOpen.Sketch.ConstraintPointType.EndVertex;
                geom1_1.SplineDefiningPointIndex = 0;
                NXOpen.Sketch.ConstraintGeometry geom2_1;
                geom2_1.Geometry = line2;
                geom2_1.PointType = NXOpen.Sketch.ConstraintPointType.StartVertex;
                geom2_1.SplineDefiningPointIndex = 0;
                SketchGeometricConstraint sketchGeometricConstraint1;
                sketchGeometricConstraint1 = theSession.ActiveSketch.CreateCoincidentConstraint(geom1_1, geom2_1);

                NXOpen.Sketch.ConstraintGeometry geom1_2;
                geom1_2.Geometry = line2;
                geom1_2.PointType = NXOpen.Sketch.ConstraintPointType.EndVertex;
                geom1_2.SplineDefiningPointIndex = 0;
                NXOpen.Sketch.ConstraintGeometry geom2_2;
                geom2_2.Geometry = line3;
                geom2_2.PointType = NXOpen.Sketch.ConstraintPointType.StartVertex;
                geom2_2.SplineDefiningPointIndex = 0;
                SketchGeometricConstraint sketchGeometricConstraint2;
                sketchGeometricConstraint2 = theSession.ActiveSketch.CreateCoincidentConstraint(geom1_2, geom2_2);

                NXOpen.Sketch.ConstraintGeometry geom1_3;
                geom1_3.Geometry = line3;
                geom1_3.PointType = NXOpen.Sketch.ConstraintPointType.EndVertex;
                geom1_3.SplineDefiningPointIndex = 0;
                NXOpen.Sketch.ConstraintGeometry geom2_3;
                geom2_3.Geometry = line4;
                geom2_3.PointType = NXOpen.Sketch.ConstraintPointType.StartVertex;
                geom2_3.SplineDefiningPointIndex = 0;
                SketchGeometricConstraint sketchGeometricConstraint3;
                sketchGeometricConstraint3 = theSession.ActiveSketch.CreateCoincidentConstraint(geom1_3, geom2_3);

                NXOpen.Sketch.ConstraintGeometry geom1_4;
                geom1_4.Geometry = line4;
                geom1_4.PointType = NXOpen.Sketch.ConstraintPointType.EndVertex;
                geom1_4.SplineDefiningPointIndex = 0;
                NXOpen.Sketch.ConstraintGeometry geom2_4;
                geom2_4.Geometry = line1;
                geom2_4.PointType = NXOpen.Sketch.ConstraintPointType.StartVertex;
                geom2_4.SplineDefiningPointIndex = 0;
                SketchGeometricConstraint sketchGeometricConstraint4;
                sketchGeometricConstraint4 = theSession.ActiveSketch.CreateCoincidentConstraint(geom1_4, geom2_4);

                NXOpen.Sketch.ConstraintGeometry geom1;
                geom1.Geometry = line1;
                geom1.PointType = NXOpen.Sketch.ConstraintPointType.None;
                geom1.SplineDefiningPointIndex = 0;
                SketchGeometricConstraint sketchGeometricConstraint5;
                sketchGeometricConstraint5 = theSession.ActiveSketch.CreateHorizontalConstraint(geom1);

                NXOpen.Sketch.ConstraintGeometry conGeom1_1;
                conGeom1_1.Geometry = line1;
                conGeom1_1.PointType = NXOpen.Sketch.ConstraintPointType.None;
                conGeom1_1.SplineDefiningPointIndex = 0;
                NXOpen.Sketch.ConstraintGeometry conGeom2_1;
                conGeom2_1.Geometry = line2;
                conGeom2_1.PointType = NXOpen.Sketch.ConstraintPointType.None;
                conGeom2_1.SplineDefiningPointIndex = 0;
                SketchGeometricConstraint sketchGeometricConstraint6;
                sketchGeometricConstraint6 = theSession.ActiveSketch.CreatePerpendicularConstraint(conGeom1_1, conGeom2_1);

                NXOpen.Sketch.ConstraintGeometry conGeom1_2;
                conGeom1_2.Geometry = line2;
                conGeom1_2.PointType = NXOpen.Sketch.ConstraintPointType.None;
                conGeom1_2.SplineDefiningPointIndex = 0;
                NXOpen.Sketch.ConstraintGeometry conGeom2_2;
                conGeom2_2.Geometry = line3;
                conGeom2_2.PointType = NXOpen.Sketch.ConstraintPointType.None;
                conGeom2_2.SplineDefiningPointIndex = 0;
                SketchGeometricConstraint sketchGeometricConstraint7;
                sketchGeometricConstraint7 = theSession.ActiveSketch.CreatePerpendicularConstraint(conGeom1_2, conGeom2_2);

                NXOpen.Sketch.ConstraintGeometry conGeom1_3;
                conGeom1_3.Geometry = line3;
                conGeom1_3.PointType = NXOpen.Sketch.ConstraintPointType.None;
                conGeom1_3.SplineDefiningPointIndex = 0;
                NXOpen.Sketch.ConstraintGeometry conGeom2_3;
                conGeom2_3.Geometry = line4;
                conGeom2_3.PointType = NXOpen.Sketch.ConstraintPointType.None;
                conGeom2_3.SplineDefiningPointIndex = 0;
                SketchGeometricConstraint sketchGeometricConstraint8;
                sketchGeometricConstraint8 = theSession.ActiveSketch.CreatePerpendicularConstraint(conGeom1_3, conGeom2_3);

                NXOpen.Sketch.ConstraintGeometry conGeom1_4;
                conGeom1_4.Geometry = line4;
                conGeom1_4.PointType = NXOpen.Sketch.ConstraintPointType.None;
                conGeom1_4.SplineDefiningPointIndex = 0;
                NXOpen.Sketch.ConstraintGeometry conGeom2_4;
                conGeom2_4.Geometry = line1;
                conGeom2_4.PointType = NXOpen.Sketch.ConstraintPointType.None;
                conGeom2_4.SplineDefiningPointIndex = 0;
                SketchGeometricConstraint sketchGeometricConstraint9;
                sketchGeometricConstraint9 = theSession.ActiveSketch.CreatePerpendicularConstraint(conGeom1_4, conGeom2_4);

                theSession.Preferences.Sketch.AutoDimensionsToArcCenter = false;

                theSession.ActiveSketch.Update();

                theSession.Preferences.Sketch.AutoDimensionsToArcCenter = true;

                SmartObject[] geoms1 = new SmartObject[4];
                geoms1[0] = line1;
                geoms1[1] = line2;
                geoms1[2] = line3;
                geoms1[3] = line4;
                theSession.ActiveSketch.UpdateConstraintDisplay(geoms1);

                SmartObject[] geoms2 = new SmartObject[4];
                geoms2[0] = line1;
                geoms2[1] = line2;
                geoms2[2] = line3;
                geoms2[3] = line4;
                theSession.ActiveSketch.UpdateDimensionDisplay(geoms2);

                // ----------------------------------------------
                //   Menu: Task->Finish Sketch
                // ----------------------------------------------
                theSession.ActiveSketch.Deactivate(NXOpen.Sketch.ViewReorient.True, NXOpen.Sketch.UpdateLevel.SketchOnly);

                theSession.DeleteUndoMarksSetInTaskEnvironment();

                theSession.EndTaskEnvironment();

                theSession.DeleteUndoMark(markId2, null);

                section1.DistanceTolerance = 0.002;

                section1.ChainingTolerance = 0.0019;

                NXOpen.Session.UndoMarkId markId11;
                markId11 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                ICurve[] curves1 = new ICurve[4];
                curves1[0] = line2;
                curves1[1] = line3;
                curves1[2] = line4;
                curves1[3] = line1;
                Point3d seedPoint1 = new Point3d(18.6666666666667, -3.33333333333333, 0.0);
                RegionBoundaryRule regionBoundaryRule1;
                regionBoundaryRule1 = workPart.ScRuleFactory.CreateRuleRegionBoundary(sketch1, curves1, seedPoint1, 0.002);

                //return false;

                section1.AllowSelfIntersection(true);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = regionBoundaryRule1;
                NXObject nullNXObject = null;
                Point3d helpPoint1 = new Point3d(0.0, 0.0, 0.0);
                try
                {
                    section1.AddToSection(rules1, nullNXObject, nullNXObject, nullNXObject, helpPoint1, NXOpen.Section.Mode.Create, false);
                }
                catch (System.Exception ex)
                {
                	
                }
                

                theSession.DeleteUndoMark(markId11, null);

                Direction direction1;
                direction1 = workPart.Directions.CreateDirection(sketch1, Sense.Forward, NXOpen.SmartObject.UpdateOption.WithinModeling);

                extrudeBuilder1.Direction = direction1;

                Body[] targetBodies3 = new Body[1];
                Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(1)");
                targetBodies3[0] = body1;
                extrudeBuilder1.BooleanOperation.SetTargetBodies(targetBodies3);

                extrudeBuilder1.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Unite;

                Body[] targetBodies4 = new Body[1];
                targetBodies4[0] = body1;
                extrudeBuilder1.BooleanOperation.SetTargetBodies(targetBodies4);

                extrudeBuilder1.Limits.StartExtend.Value.RightHandSide = "-10";

                extrudeBuilder1.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Unite;

                Body[] targetBodies5 = new Body[1];
                targetBodies5[0] = body1;
                extrudeBuilder1.BooleanOperation.SetTargetBodies(targetBodies5);

                extrudeBuilder1.Limits.EndExtend.Value.RightHandSide = "-10";

                extrudeBuilder1.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

                Body[] targetBodies6 = new Body[1];
                targetBodies6[0] = body1;
                extrudeBuilder1.BooleanOperation.SetTargetBodies(targetBodies6);

                NXOpen.Session.UndoMarkId markId12;
                markId12 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Extrude");

                theSession.DeleteUndoMark(markId12, null);

                NXOpen.Session.UndoMarkId markId13;
                markId13 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Extrude");

                Body[] targetBodies7 = new Body[0];
                extrudeBuilder1.BooleanOperation.SetTargetBodies(targetBodies7);

                extrudeBuilder1.ParentFeatureInternal = true;

                NXOpen.Features.Feature feature2;
                feature2 = extrudeBuilder1.CommitFeature();
                Tag PlaneBodyTag;
                theUfSession.Modl.AskFeatBody(feature2.Tag, out PlaneBodyTag);
                PlaneBody = (Body)NXObjectManager.Get(PlaneBodyTag);
                //RemoveParameters(PlaneBody);

                //return false;

                theSession.DeleteUndoMark(markId13, null);

                theSession.SetUndoMarkName(markId1, "Extrude");

                Expression expression4 = extrudeBuilder1.Limits.StartExtend.Value;
                Expression expression5 = extrudeBuilder1.Limits.EndExtend.Value;
                extrudeBuilder1.Destroy();

                workPart.Expressions.Delete(expression1);

                // ----------------------------------------------
                //   Menu: Tools->Journal->Stop Recording
                // ----------------------------------------------
            }
            catch (System.Exception ex)
            {
                //CaxLog.ShowListingWindow("CreateSheetBodyOnZPlane  ex:" + ex.ToString());
                return false;
            }
            return true;
        }

        public static bool TrimmedSheet(Body TargetBody,Body ToolBody,Edge[] ToolEdgeAry,Face[] ToolFaceAry,out NXObject nXObject1)
        {
            nXObject1 = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Trim->Trimmed Sheet...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.Feature nullFeatures_Feature = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.TrimSheetBuilder trimSheetBuilder1;
                trimSheetBuilder1 = workPart.Features.CreateTrimsheetBuilder(nullFeatures_Feature);

                trimSheetBuilder1.Tolerance = 0.002;

                theSession.SetUndoMarkName(markId1, "Trimmed Sheet Dialog");

                Body body1 = (Body)workPart.Bodies.FindObject(TargetBody.JournalIdentifier);
                bool added1;
                added1 = trimSheetBuilder1.TargetBodies.Add(body1);

                Point3d coordinates1 = new Point3d(-28.1702538304025, 5.24198950614347, -10.0);
                Point point1;
                point1 = workPart.Points.CreatePoint(coordinates1);

                RegionPoint regionPoint1;
                regionPoint1 = workPart.CreateRegionPoint(point1, body1);

                trimSheetBuilder1.Regions.Append(regionPoint1);

                Direction nullDirection = null;
                trimSheetBuilder1.ProjectionDirection.ProjectVector = nullDirection;

                Section section1;
                section1 = workPart.Sections.CreateSection(0.0019, 0.002, 0.01);

                section1.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

                bool added2;
                added2 = trimSheetBuilder1.BoundaryObjects.Add(section1);

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                Edge[] edges1 = new Edge[ToolEdgeAry.Length];
                //NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject(ToolBody.JournalIdentifier);
                edges1 = ToolEdgeAry;

                EdgeDumbRule edgeDumbRule1;
                edgeDumbRule1 = workPart.ScRuleFactory.CreateRuleEdgeDumb(edges1);

                section1.AllowSelfIntersection(true);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = edgeDumbRule1;
                NXObject nullNXObject = null;
                Point3d helpPoint1 = new Point3d(0.0, 0.0, 0.0);
                section1.AddToSection(rules1, nullNXObject, nullNXObject, nullNXObject, helpPoint1, NXOpen.Section.Mode.Create, false);

                theSession.DeleteUndoMark(markId3, null);

                ScCollector scCollector1;
                scCollector1 = workPart.ScCollectors.CreateCollector();

                Face[] faces1 = ToolFaceAry;

                FaceDumbRule faceDumbRule1;
                faceDumbRule1 = workPart.ScRuleFactory.CreateRuleFaceDumb(faces1);

                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = faceDumbRule1;
                scCollector1.ReplaceRules(rules2, false);

                bool added3;
                added3 = trimSheetBuilder1.BoundaryObjects.Add(scCollector1);

                trimSheetBuilder1.ProjectionDirection.ProjectVector = nullDirection;

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Trimmed Sheet");

                theSession.DeleteUndoMark(markId4, null);

                NXOpen.Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Trimmed Sheet");

                //NXObject nXObject1;
                nXObject1 = trimSheetBuilder1.Commit();

                theSession.DeleteUndoMark(markId5, null);

                theSession.SetUndoMarkName(markId1, "Trimmed Sheet");

                trimSheetBuilder1.Destroy();

                // ----------------------------------------------
                //   Menu: Tools->Journal->Stop Recording
                // ----------------------------------------------

            }
            catch (System.Exception ex)
            {
                //CaxLog.ShowListingWindow(ex.ToString());
                return false;
            }
            return true;
            
        }

        public static bool J_ProjectToZPlane(Edge[] ProjectEdgeAry,out NXObject ProjectCurveObj)
        {
            ProjectCurveObj = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Tools->Repeat Command->3 Project Curve
                // ----------------------------------------------
                // ----------------------------------------------
                //   Menu: Insert->Curve from Curves->Project...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.Feature nullFeatures_Feature = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.ProjectCurveBuilder projectCurveBuilder1;
                projectCurveBuilder1 = workPart.Features.CreateProjectCurveBuilder(nullFeatures_Feature);

                Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
                Vector3d normal1 = new Vector3d(0.0, 0.0, 1.0);
                Plane plane1;
                plane1 = workPart.Planes.CreatePlane(origin1, normal1, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
                Expression expression1;
                expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                Expression expression2;
                expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                projectCurveBuilder1.CurveFitData.Tolerance = 0.002;

                projectCurveBuilder1.CurveFitData.AngleTolerance = 0.01;

                theSession.SetUndoMarkName(markId1, "Project Curve Dialog");

                projectCurveBuilder1.SectionToProject.DistanceTolerance = 0.002;

                projectCurveBuilder1.SectionToProject.ChainingTolerance = 0.0019;

                projectCurveBuilder1.SectionToProject.AngleTolerance = 0.01;

                projectCurveBuilder1.SectionToProject.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.CurvesAndPoints);

                NXObject[] geom1 = new NXObject[0];
                plane1.SetGeometry(geom1);

                plane1.SetMethod(NXOpen.PlaneTypes.MethodType.FixedZ);

                NXObject[] geom2 = new NXObject[0];
                plane1.SetGeometry(geom2);

                Point3d origin2 = new Point3d(0.0, 0.0, 0.0);
                plane1.Origin = origin2;

                Matrix3x3 matrix1;
                matrix1.Xx = 1.0;
                matrix1.Xy = 0.0;
                matrix1.Xz = 0.0;
                matrix1.Yx = 0.0;
                matrix1.Yy = 1.0;
                matrix1.Yz = 0.0;
                matrix1.Zx = 0.0;
                matrix1.Zy = 0.0;
                matrix1.Zz = 1.0;
                plane1.Matrix = matrix1;

                plane1.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);

                plane1.Evaluate();

                plane1.SetMethod(NXOpen.PlaneTypes.MethodType.FixedZ);

                projectCurveBuilder1.PlaneToProjectTo = plane1;

                plane1.SetMethod(NXOpen.PlaneTypes.MethodType.FixedZ);

                NXObject[] geom3 = new NXObject[0];
                plane1.SetGeometry(geom3);

                Point3d origin3 = new Point3d(0.0, 0.0, 0.0);
                plane1.Origin = origin3;

                Matrix3x3 matrix2;
                matrix2.Xx = 1.0;
                matrix2.Xy = 0.0;
                matrix2.Xz = 0.0;
                matrix2.Yx = 0.0;
                matrix2.Yy = 1.0;
                matrix2.Yz = 0.0;
                matrix2.Zx = 0.0;
                matrix2.Zy = 0.0;
                matrix2.Zz = 1.0;
                plane1.Matrix = matrix2;

                plane1.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);

                plane1.Evaluate();

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                /*
                Edge[] edges1 = new Edge[1];
                NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject("UNPARAMETERIZED_FEATURE(2)");
                Edge edge1 = (Edge)brep1.FindObject("EDGE * -22 * 1 {(-45.0746694140296,3.4644637972742,-10)(-3.9024107186143,3.4644637972742,-10)(37.269847976801,3.4644637972742,-10) UNPARAMETERIZED_FEATURE(2)}");
                edges1[0] = edge1;
                EdgeDumbRule edgeDumbRule1;
                edgeDumbRule1 = workPart.ScRuleFactory.CreateRuleEdgeDumb(edges1);

                projectCurveBuilder1.SectionToProject.AllowSelfIntersection(true);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = edgeDumbRule1;
                NXObject nullNXObject = null;
                Point3d helpPoint1 = new Point3d(-37.6563296529692, 3.46446379727425, -10.0);
                projectCurveBuilder1.SectionToProject.AddToSection(rules1, edge1, nullNXObject, nullNXObject, helpPoint1, NXOpen.Section.Mode.Create, false);

                theSession.DeleteUndoMark(markId3, null);

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

                NXOpen.Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                Edge[] edges2 = new Edge[1];
                Edge edge2 = (Edge)brep1.FindObject("EDGE * -5 * 1 {(-48.0721455808123,3.4644637972742,-10)(-46.573407497421,3.4644637972742,-10)(-45.0746694140297,3.4644637972742,-10) UNPARAMETERIZED_FEATURE(2)}");
                edges2[0] = edge2;
                EdgeDumbRule edgeDumbRule2;
                edgeDumbRule2 = workPart.ScRuleFactory.CreateRuleEdgeDumb(edges2);

                projectCurveBuilder1.SectionToProject.AllowSelfIntersection(true);

                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = edgeDumbRule2;
                Point3d helpPoint2 = new Point3d(-45.6443730169544, 3.46446379727425, -10.0);
                projectCurveBuilder1.SectionToProject.AddToSection(rules2, edge2, nullNXObject, nullNXObject, helpPoint2, NXOpen.Section.Mode.Create, false);
                */
                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

                NXOpen.Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                NXObject nullNXObject = null;
                /*Edge[] edges = new Edge[ProjectEdgeAry.Length];*/
                for (int i = 0; i < ProjectEdgeAry.Length;i++ )
                {
                    Edge[] edges = new Edge[1];
                    Edge edge = ProjectEdgeAry[i];
                    edges[0] = edge;
                    EdgeDumbRule edgeDumbRule;
                    edgeDumbRule = workPart.ScRuleFactory.CreateRuleEdgeDumb(edges);

                    projectCurveBuilder1.SectionToProject.AllowSelfIntersection(true);

                    SelectionIntentRule[] rules = new SelectionIntentRule[1];
                    rules[0] = edgeDumbRule;
                    Point3d helpPoint2 = new Point3d(-45.6443730169544, 3.46446379727425, -10.0);
                    projectCurveBuilder1.SectionToProject.AddToSection(rules, edge, nullNXObject, nullNXObject, helpPoint2, NXOpen.Section.Mode.Create, false);
                }

                theSession.DeleteUndoMark(markId5, null);

                theSession.DeleteUndoMark(markId4, null);

                NXOpen.Session.UndoMarkId markId6;
                markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Project Curve");

                theSession.DeleteUndoMark(markId6, null);

                NXOpen.Session.UndoMarkId markId7;
                markId7 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Project Curve");

                NXObject nXObject1;
                nXObject1 = projectCurveBuilder1.Commit();

                ProjectCurveObj = nXObject1;

                theSession.DeleteUndoMark(markId7, null);

                theSession.SetUndoMarkName(markId1, "Project Curve");

                projectCurveBuilder1.SectionToProject.CleanMappingData();

                projectCurveBuilder1.SectionToProject.CleanMappingData();

                projectCurveBuilder1.Destroy();

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

        public static bool J_ExtrudeCurve(NXObject[] ProjectFeaObj, string WP_Height, out Body ExtrudeBody)
        {
            ExtrudeBody = null;
            try
            {
                // ------------------------------------------------------------------
                List<Line> ListLine = new List<Line>();
                List<Arc> ListArc = new List<Arc>();
                List<Spline> ListSpline = new List<Spline>();
                List<Ellipse> ListEllipse = new List<Ellipse>();
                int count = 0;
                bool chk_Line = false;
                bool chk_Arc = false;
                bool chk_Spline = false;
                bool chk_Ellipse = false;

                SeparateCurveType(ProjectFeaObj, out ListLine, out chk_Line, 
                    out ListArc, out chk_Arc, 
                    out ListSpline, out chk_Spline, 
                    out ListEllipse, out chk_Ellipse, 
                    out count);

                /*
                

                for (int i = 0; i < ProjectFeaObj.Length; i++)
                {
                    string ProjectFeaObjType = ProjectFeaObj[i].GetType().ToString();
                    if (ProjectFeaObjType == "NXOpen.Line")
                    {
                        chk_Line = true;
                        Line TempLine = (Line)ProjectFeaObj[i];
                        ListLine.Add(TempLine);
                    }
                    else if (ProjectFeaObjType == "NXOpen.Arc")
                    {
                        chk_Arc = true;
                        Arc TempArc = (Arc)ProjectFeaObj[i];
                        ListArc.Add(TempArc);
                    }
                    else if (ProjectFeaObjType == "NXOpen.Spline")
                    {
                        chk_Spline = true;
                        Spline TempSpline = (Spline)ProjectFeaObj[i];
                        ListSpline.Add(TempSpline);
                    }
                    else if (ProjectFeaObjType == "NXOpen.Ellipse")
                    {
                        chk_Ellipse = true;
                        Ellipse TempEllipse = (Ellipse)ProjectFeaObj[i];
                        ListEllipse.Add(TempEllipse);
                    }
                }

                int count = 0;
                if (ListLine.Count != 0)
                {
                    count++;
                }
                if (ListArc.Count != 0)
                {
                    count++;
                }
                if (ListSpline.Count != 0)
                {
                    count++;
                }
                if (ListEllipse.Count != 0)
                {
                    count++;
                }
                */
                // ------------------------------------------------------------------

                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Design Feature->Extrude...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

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

                NXOpen.GeometricUtilities.SmartVolumeProfileBuilder smartVolumeProfileBuilder1;
                smartVolumeProfileBuilder1 = extrudeBuilder1.SmartVolumeProfile;

                smartVolumeProfileBuilder1.OpenProfileSmartVolumeOption = false;

                smartVolumeProfileBuilder1.CloseProfileRule = NXOpen.GeometricUtilities.SmartVolumeProfileBuilder.CloseProfileRuleType.Fci;

                theSession.SetUndoMarkName(markId1, "Extrude Dialog");

                section1.DistanceTolerance = 0.002;

                section1.ChainingTolerance = 0.0019;

                section1.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                //自己的CODE
                section1.AllowSelfIntersection(true);
                SelectionIntentRule[] rules1 = new SelectionIntentRule[count];
                for (int i = 0; i < count; i++ )
                {
                    CurveDumbRule curveDumbRule1;
                    if (chk_Line)
                    {
                        chk_Line = false;
                        IBaseCurve[] curves1 = ListLine.ToArray();
                        curveDumbRule1 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves1);
                        rules1[i] = curveDumbRule1;
                        continue;
                    }
                    if (chk_Arc)
                    {
                        chk_Arc = false;
                        IBaseCurve[] curves1 = ListArc.ToArray();
                        curveDumbRule1 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves1);
                        rules1[i] = curveDumbRule1;
                        continue;
                    }
                    if (chk_Spline)
                    {
                        chk_Spline = false;
                        IBaseCurve[] curves1 = ListSpline.ToArray();
                        curveDumbRule1 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves1);
                        rules1[i] = curveDumbRule1;
                        continue;
                    }
                    if (chk_Ellipse)
                    {
                        chk_Ellipse = false;
                        IBaseCurve[] curves1 = ListEllipse.ToArray();
                        curveDumbRule1 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves1);
                        rules1[i] = curveDumbRule1;
                        continue;
                    }
                }

                NXObject nullNXObject = null;
                Point3d helpPoint1 = new Point3d(0.0, 0.0, 0.0);
                section1.AddToSection(rules1, nullNXObject, nullNXObject, nullNXObject, helpPoint1, NXOpen.Section.Mode.Create, false);
                

                theSession.DeleteUndoMark(markId3, null);

                Point3d origin1 = new Point3d(-6.47203698918781, 0.522491608788982, -5.0);
                Vector3d vector1 = new Vector3d(0.0, 0.0, 1.0);
                Direction direction1;
                direction1 = workPart.Directions.CreateDirection(origin1, vector1, NXOpen.SmartObject.UpdateOption.WithinModeling);

                extrudeBuilder1.Direction = direction1;

                Unit unit2;
                unit2 = extrudeBuilder1.Offset.StartOffset.Units;

                Expression expression2;
                expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

                extrudeBuilder1.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

                Body[] targetBodies2 = new Body[1];
                targetBodies2[0] = nullBody;
                extrudeBuilder1.BooleanOperation.SetTargetBodies(targetBodies2);

                theSession.DeleteUndoMark(markId2, null);

                extrudeBuilder1.Limits.EndExtend.Value.RightHandSide = WP_Height;

                extrudeBuilder1.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

                Body[] targetBodies3 = new Body[1];
                targetBodies3[0] = nullBody;
                extrudeBuilder1.BooleanOperation.SetTargetBodies(targetBodies3);

                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Extrude");

                theSession.DeleteUndoMark(markId4, null);

                NXOpen.Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Extrude");

                extrudeBuilder1.ParentFeatureInternal = false;

                NXOpen.Features.Feature feature1;
                feature1 = extrudeBuilder1.CommitFeature();
                //feature = feature1;

                Tag ExtrudeBodyTag;
                theUfSession.Modl.AskFeatBody(feature1.Tag, out ExtrudeBodyTag);
                ExtrudeBody = (Body)NXObjectManager.Get(ExtrudeBodyTag);

                theSession.DeleteUndoMark(markId5, null);

                theSession.SetUndoMarkName(markId1, "Extrude");

                Expression expression3 = extrudeBuilder1.Limits.StartExtend.Value;
                Expression expression4 = extrudeBuilder1.Limits.EndExtend.Value;
                extrudeBuilder1.Destroy();

                workPart.Expressions.Delete(expression1);

                workPart.Expressions.Delete(expression2);

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

        private static bool SeparateCurveType(NXObject[] ProjectFeaObj, 
            out List<Line> ListLine, out bool chk_Line,
            out List<Arc> ListArc, out bool chk_Arc,
            out List<Spline> ListSpline, out bool chk_Spline,
            out List<Ellipse> ListEllipse, out bool chk_Ellipse,
            out int count)
        {
            ListLine = new List<Line>();
            ListArc = new List<Arc>();
            ListSpline = new List<Spline>();
            ListEllipse = new List<Ellipse>();
            count = 0;
            chk_Line = false;
            chk_Arc = false;
            chk_Spline = false;
            chk_Ellipse = false;
            try
            {
                for (int i = 0; i < ProjectFeaObj.Length; i++)
                {
                    string ProjectFeaObjType = ProjectFeaObj[i].GetType().ToString();
                    if (ProjectFeaObjType == "NXOpen.Line")
                    {
                        chk_Line = true;
                        Line TempLine = (Line)ProjectFeaObj[i];
                        ListLine.Add(TempLine);
                    }
                    else if (ProjectFeaObjType == "NXOpen.Arc")
                    {
                        chk_Arc = true;
                        Arc TempArc = (Arc)ProjectFeaObj[i];
                        ListArc.Add(TempArc);
                    }
                    else if (ProjectFeaObjType == "NXOpen.Spline")
                    {
                        chk_Spline = true;
                        Spline TempSpline = (Spline)ProjectFeaObj[i];
                        ListSpline.Add(TempSpline);
                    }
                    else if (ProjectFeaObjType == "NXOpen.Ellipse")
                    {
                        chk_Ellipse = true;
                        Ellipse TempEllipse = (Ellipse)ProjectFeaObj[i];
                        ListEllipse.Add(TempEllipse);
                    }
                }

                if (ListLine.Count != 0)
                {
                    count++;
                }
                if (ListArc.Count != 0)
                {
                    count++;
                }
                if (ListSpline.Count != 0)
                {
                    count++;
                }
                if (ListEllipse.Count != 0)
                {
                    count++;
                }
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

        public static bool AskFaceAreaAndPerimeter(Face Faces, out MeasureFaces theMeasure)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            theMeasure = null;
            try
            {
                List<IParameterizedSurface> iSurface = new List<IParameterizedSurface>();
                iSurface.Add(Faces);
                Unit area_units = workPart.UnitCollection.GetBase("Area");
                Unit length_units = workPart.UnitCollection.GetBase("Length");
                theMeasure = workPart.MeasureManager.NewFaceProperties(area_units, length_units, 0.999, iSurface.ToArray());
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool DivideFace(Face TargetFace, NXObject AroundFaceFeatObj, Face ToolFace)
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Trim->Divide Face...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

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

                theSession.SetUndoMarkName(markId1, "Divide Face Dialog");

                ScCollector scCollector1;
                scCollector1 = workPart.ScCollectors.CreateCollector();
                /*
                Tag[] facefeatTagAry;
                theUfSession.Modl.AskFaceFeats(TargetFace.Tag,out facefeatTagAry);
                Feature facefeat = (Feature)NXObjectManager.Get(facefeatTagAry[0]);
                //CaxLog.ShowListingWindow("facefeat.JournalIdentifier:"+facefeat.JournalIdentifier);
                NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject(facefeat.JournalIdentifier);
                Face face1 = (Face)brep1.FindObject(TargetFace.JournalIdentifier);
                */
                Face face1 = TargetFace;
                
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

                //NXOpen.Features.Enlarge enlarge1 = (NXOpen.Features.Enlarge)workPart.Features.FindObject("ENLARGE(3)");
                //Face face2 = (Face)enlarge1.FindObject("FACE 1 {(-1.3974915708125,0.9516113993744,1.5) ENLARGE(3)}");
                
                NXOpen.Features.Enlarge enlarge = (NXOpen.Features.Enlarge)AroundFaceFeatObj;
                Face face2 = (Face)enlarge.FindObject(ToolFace.JournalIdentifier);
                
                NXObject[] enlargeEntities = enlarge.GetEntities();

                CaxLog.ShowListingWindow("enlarge:" + enlarge.ToString());
                CaxLog.ShowListingWindow("enlargeEntities.Length:" + enlargeEntities.Length);
                CaxLog.ShowListingWindow("enlarge.JournalIdentifier:"+enlarge.JournalIdentifier);
                CaxLog.ShowListingWindow("ToolFace.JournalIdentifier:"+ToolFace.JournalIdentifier);

                //Face face2 = ToolFace;
                //Face face2 = (Face)enlargeEntities[0];
                Face[] boundaryFaces2 = new Face[0];
                FaceTangentRule faceTangentRule2;
                faceTangentRule2 = workPart.ScRuleFactory.CreateRuleFaceTangent(face2, boundaryFaces2, 0.5);

                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = faceTangentRule2;
                scCollector2.ReplaceRules(rules2, false);

                bool added1;
                added1 = dividefaceBuilder1.DividingObjectsList.Add(scCollector2);

                projectionOptions1.ProjectDirectionMethod = NXOpen.GeometricUtilities.ProjectionOptions.DirectionType.FaceNormal;

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Divide Face");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Divide Face");

                NXOpen.Features.Feature feature1;
                feature1 = dividefaceBuilder1.CommitFeature();

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "Divide Face");

                dividefaceBuilder1.Destroy();

                section1.Destroy();


            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow(ex.ToString());
                return false;
            }

            return true;
        }

        public static bool LineDivide(Face TargetFace, List<NXObject> ListObj, out Feature feature)
        {
            feature = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Trim->Divide Face...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

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

                theSession.SetUndoMarkName(markId1, "Divide Face Dialog");

                ScCollector scCollector1;
                scCollector1 = workPart.ScCollectors.CreateCollector();

                //NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject("UNPARAMETERIZED_FEATURE(1)");
                //Face face1 = (Face)brep1.FindObject("FACE 35 {(44.7309141587635,-0.0243096683962,2.1703852636144) UNPARAMETERIZED_FEATURE(1)}");
                Face face1 = TargetFace;
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

                bool added1;
                added1 = dividefaceBuilder1.DividingObjectsList.Add(section1);

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                Curve nullCurve = null;
                NXOpen.Features.AssociativeLine associativeLine1 = null;
                CurveFeatureChainRule curveFeatureChainRule2 = null;
                foreach (NXObject SingleObj in ListObj)
                {
                    NXOpen.Features.Feature[] features2 = new NXOpen.Features.Feature[1];
                    associativeLine1 = (NXOpen.Features.AssociativeLine)SingleObj;
                    features2[0] = associativeLine1;
                    Line line1 = (Line)associativeLine1.GetEntities()[0];
                    curveFeatureChainRule2 = workPart.ScRuleFactory.CreateRuleCurveFeatureChain(features2, line1, nullCurve, false, 0.0019);
                    section1.AllowSelfIntersection(true);
                    SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                    rules2[0] = curveFeatureChainRule2;
                    Line line2 = (Line)associativeLine1.GetEntities()[0];
                    NXObject nullNXObject = null;
                    Point3d helpPoint1 = new Point3d(0, 0, 0);
                    section1.AddToSection(rules2, line2, nullNXObject, nullNXObject, helpPoint1, NXOpen.Section.Mode.Create, false);
                }

                //section1.AllowSelfIntersection(true);

                //SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                //rules2[0] = curveFeatureChainRule2;
                //Line line2 = (Line)associativeLine1.GetEntities()[0];
                //NXObject nullNXObject = null;
                //Point3d helpPoint1 = new Point3d(44.7164031137519, 0.951611399374421, 9.77354928639329);
                //section1.AddToSection(rules2, line2, nullNXObject, nullNXObject, helpPoint1, NXOpen.Section.Mode.Create, false);
                /*
                NXOpen.Features.Feature[] features1 = new NXOpen.Features.Feature[1];
                NXOpen.Features.AssociativeLine associativeLine1 = (NXOpen.Features.AssociativeLine)workPart.Features.FindObject("LINE(3)");
                features1[0] = associativeLine1;
                NXOpen.Assemblies.Component component1 = (NXOpen.Assemblies.Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT X999J-0061T_LF-PIN_186_A0_WEDMS1_T 1");
                Line line1 = (Line)component1.FindObject("PROTO#.Features|LINE(3)|CURVE 1");
                Curve nullCurve = null;
                CurveFeatureChainRule curveFeatureChainRule1;
                curveFeatureChainRule1 = workPart.ScRuleFactory.CreateRuleCurveFeatureChain(features1, line1, nullCurve, false, 0.0019);

                section1.AllowSelfIntersection(true);

                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = curveFeatureChainRule1;
                Line line2 = (Line)associativeLine1.FindObject("CURVE 1");
                NXObject nullNXObject = null;
                Point3d helpPoint1 = new Point3d(44.7164031137519, 0.951611399374421, 9.77354928639329);
                section1.AddToSection(rules2, line2, nullNXObject, nullNXObject, helpPoint1, NXOpen.Section.Mode.Create, false);
                */

                theSession.DeleteUndoMark(markId3, null);

                projectionOptions1.ProjectDirectionMethod = NXOpen.GeometricUtilities.ProjectionOptions.DirectionType.FaceNormal;

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Divide Face");

                theSession.DeleteUndoMark(markId4, null);

                NXOpen.Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Divide Face");


                feature = dividefaceBuilder1.CommitFeature();

                theSession.DeleteUndoMark(markId5, null);

                theSession.SetUndoMarkName(markId1, "Divide Face");

                dividefaceBuilder1.Destroy();

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

        public static bool TwoPtCreateBlock()
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Tools->Repeat Command->6 Block
                // ----------------------------------------------
                // ----------------------------------------------
                //   Menu: Insert->Design Feature->Block...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

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

                blockFeatureBuilder1.Type = NXOpen.Features.BlockFeatureBuilder.Types.TwoPointsAndHeight;

                blockFeatureBuilder1.BooleanOption.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

                Body[] targetBodies2 = new Body[1];
                targetBodies2[0] = nullBody;
                blockFeatureBuilder1.BooleanOption.SetTargetBodies(targetBodies2);

                theSession.SetUndoMarkName(markId1, "Block Dialog");

                Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
                Expression expression1;
                expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                Expression expression2;
                expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("p1_x=-133.422444911802", unit1);

                Scalar scalar1;
                scalar1 = workPart.Scalars.CreateScalarExpression(expression2, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression3;
                expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("p2_y=58.4771858393618", unit1);

                Scalar scalar2;
                scalar2 = workPart.Scalars.CreateScalarExpression(expression3, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression4;
                expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("p3_z=0", unit1);

                Scalar scalar3;
                scalar3 = workPart.Scalars.CreateScalarExpression(expression4, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Point point1;
                point1 = workPart.Points.CreatePoint(scalar1, scalar2, scalar3, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression5;
                expression5 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                Expression expression6;
                expression6 = workPart.Expressions.CreateSystemExpressionWithUnits("p5_x=-133.169958277689", unit1);

                Scalar scalar4;
                scalar4 = workPart.Scalars.CreateScalarExpression(expression6, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression7;
                expression7 = workPart.Expressions.CreateSystemExpressionWithUnits("p6_y=66.5787593272623", unit1);

                Scalar scalar5;
                scalar5 = workPart.Scalars.CreateScalarExpression(expression7, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression8;
                expression8 = workPart.Expressions.CreateSystemExpressionWithUnits("p7_z=-4.44089209850063e-016", unit1);

                Scalar scalar6;
                scalar6 = workPart.Scalars.CreateScalarExpression(expression8, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Point point2;
                point2 = workPart.Points.CreatePoint(scalar4, scalar5, scalar6, NXOpen.SmartObject.UpdateOption.WithinModeling);

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Block");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Block");

                blockFeatureBuilder1.Type = NXOpen.Features.BlockFeatureBuilder.Types.TwoPointsAndHeight;

                blockFeatureBuilder1.OriginPoint = point1;

                blockFeatureBuilder1.PointFromOrigin = point2;

                Point3d originPoint1 = new Point3d(-133.422444911802, 58.4771858393618, 0.0);
                Point3d cornerPoint1 = new Point3d(-133.169958277689, 66.5787593272623, -4.44089209850063e-016);
                blockFeatureBuilder1.SetTwoPointsAndHeight(originPoint1, cornerPoint1, "5");

                blockFeatureBuilder1.SetBooleanOperationAndTarget(NXOpen.Features.Feature.BooleanType.Create, nullBody);

                NXOpen.Features.Feature feature1;
                feature1 = blockFeatureBuilder1.CommitFeature();

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "Block");

                blockFeatureBuilder1.Destroy();

                workPart.Expressions.Delete(expression1);

                workPart.Expressions.Delete(expression5);

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

        public static bool J_MoveObject_RotateZ(Body sbody,double angle)
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

                theSession.SetUndoMarkName(markId1, "Move Object Dialog");

                //Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(1)");
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

                /*
                Expression expression2;
                expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

                Expression expression3;
                expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                Expression expression4;
                expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("p32_x=0.00000000000", unit1);

                Expression expression5;
                expression5 = workPart.Expressions.CreateSystemExpressionWithUnits("p33_y=0.00000000000", unit1);

                Expression expression6;
                expression6 = workPart.Expressions.CreateSystemExpressionWithUnits("p34_z=0.00000000000", unit1);

                Expression expression7;
                expression7 = workPart.Expressions.CreateSystemExpressionWithUnits("p35_xdelta=0.00000000000", unit1);

                Expression expression8;
                expression8 = workPart.Expressions.CreateSystemExpressionWithUnits("p36_ydelta=0.00000000000", unit1);

                Expression expression9;
                expression9 = workPart.Expressions.CreateSystemExpressionWithUnits("p37_zdelta=0.00000000000", unit1);

                Expression expression10;
                expression10 = workPart.Expressions.CreateSystemExpressionWithUnits("p38_radius=0.00000000000", unit1);

                Unit unit2;
                unit2 = moveObjectBuilder1.TransformMotion.DistanceAngle.Angle.Units;

                Expression expression11;
                expression11 = workPart.Expressions.CreateSystemExpressionWithUnits("p39_angle=0.00000000000", unit2);

                Expression expression12;
                expression12 = workPart.Expressions.CreateSystemExpressionWithUnits("p40_zdelta=0.00000000000", unit1);

                Expression expression13;
                expression13 = workPart.Expressions.CreateSystemExpressionWithUnits("p41_radius=0.00000000000", unit1);

                Expression expression14;
                expression14 = workPart.Expressions.CreateSystemExpressionWithUnits("p42_angle1=0.00000000000", unit2);

                Expression expression15;
                expression15 = workPart.Expressions.CreateSystemExpressionWithUnits("p43_angle2=0.00000000000", unit2);

                Expression expression16;
                expression16 = workPart.Expressions.CreateSystemExpressionWithUnits("p44_distance=0", unit1);

                Expression expression17;
                expression17 = workPart.Expressions.CreateSystemExpressionWithUnits("p45_arclen=0", unit1);

                Unit nullUnit = null;
                Expression expression18;
                expression18 = workPart.Expressions.CreateSystemExpressionWithUnits("p46_percent=0", nullUnit);

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

                expression15.RightHandSide = "0";

                expression16.RightHandSide = "0";

                expression18.RightHandSide = "100";

                expression17.RightHandSide = "0";
                
                theSession.SetUndoMarkName(markId2, "Point Dialog");
                

                Expression expression19;
                expression19 = workPart.Expressions.CreateSystemExpressionWithUnits("p47_x=0.00000000000", unit1);

                Scalar scalar1;
                scalar1 = workPart.Scalars.CreateScalarExpression(expression19, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression20;
                expression20 = workPart.Expressions.CreateSystemExpressionWithUnits("p48_y=0.00000000000", unit1);

                Scalar scalar2;
                scalar2 = workPart.Scalars.CreateScalarExpression(expression20, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression21;
                expression21 = workPart.Expressions.CreateSystemExpressionWithUnits("p49_z=0.00000000000", unit1);

                Scalar scalar3;
                scalar3 = workPart.Scalars.CreateScalarExpression(expression21, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Point point1;
                point1 = workPart.Points.CreatePoint(scalar1, scalar2, scalar3, NXOpen.SmartObject.UpdateOption.WithinModeling);

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
                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

                theSession.DeleteUndoMark(markId3, null);

                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

                workPart.Points.DeletePoint(point1);

                Expression expression22;
                expression22 = workPart.Expressions.CreateSystemExpressionWithUnits("p33_x=0.00000000000", unit1);

                Scalar scalar4;
                scalar4 = workPart.Scalars.CreateScalarExpression(expression22, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression23;
                expression23 = workPart.Expressions.CreateSystemExpressionWithUnits("p34_y=0.00000000000", unit1);

                Scalar scalar5;
                scalar5 = workPart.Scalars.CreateScalarExpression(expression23, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression24;
                expression24 = workPart.Expressions.CreateSystemExpressionWithUnits("p35_z=0.00000000000", unit1);

                Scalar scalar6;
                scalar6 = workPart.Scalars.CreateScalarExpression(expression24, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Point point2;
                point2 = workPart.Points.CreatePoint(scalar4, scalar5, scalar6, NXOpen.SmartObject.UpdateOption.WithinModeling);

                expression4.RightHandSide = "0";

                expression5.RightHandSide = "0";

                expression6.RightHandSide = "0";

                expression4.RightHandSide = "0.00000000000";

                expression5.RightHandSide = "0.00000000000";

                expression6.RightHandSide = "0.00000000000";

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

                Scalar scalar7;
                scalar7 = workPart.Scalars.CreateScalarExpression(expression22, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Scalar scalar8;
                scalar8 = workPart.Scalars.CreateScalarExpression(expression23, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Scalar scalar9;
                scalar9 = workPart.Scalars.CreateScalarExpression(expression24, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Point point3;
                point3 = workPart.Points.CreatePoint(scalar7, scalar8, scalar9, NXOpen.SmartObject.UpdateOption.WithinModeling);
                */

                //axis1.Point = point2;
                Point3d a = new Point3d(0.0,0.0,0.0);
                Point b = workPart.Points.CreatePoint(a);
                axis1.Point = b;

                moveObjectBuilder1.TransformMotion.AngularAxis = axis1;
                
                moveObjectBuilder1.TransformMotion.Angle.RightHandSide = (-1 * angle).ToString();

                moveObjectBuilder1.TransformMotion.Angle.RightHandSide = (-1 * angle).ToString();

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

                //workPart.Points.DeletePoint(point3);

                //workPart.Expressions.Delete(expression2);

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

        public static bool J_MoveObject_RotateY(Body sbody, double angle)
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

                theSession.SetUndoMarkName(markId1, "Move Object Dialog");

                //Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(1)");
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

                /*
                Expression expression2;
                expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

                Expression expression3;
                expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                Expression expression4;
                expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("p32_x=0.00000000000", unit1);

                Expression expression5;
                expression5 = workPart.Expressions.CreateSystemExpressionWithUnits("p33_y=0.00000000000", unit1);

                Expression expression6;
                expression6 = workPart.Expressions.CreateSystemExpressionWithUnits("p34_z=0.00000000000", unit1);

                Expression expression7;
                expression7 = workPart.Expressions.CreateSystemExpressionWithUnits("p35_xdelta=0.00000000000", unit1);

                Expression expression8;
                expression8 = workPart.Expressions.CreateSystemExpressionWithUnits("p36_ydelta=0.00000000000", unit1);

                Expression expression9;
                expression9 = workPart.Expressions.CreateSystemExpressionWithUnits("p37_zdelta=0.00000000000", unit1);

                Expression expression10;
                expression10 = workPart.Expressions.CreateSystemExpressionWithUnits("p38_radius=0.00000000000", unit1);

                Unit unit2;
                unit2 = moveObjectBuilder1.TransformMotion.DistanceAngle.Angle.Units;

                Expression expression11;
                expression11 = workPart.Expressions.CreateSystemExpressionWithUnits("p39_angle=0.00000000000", unit2);

                Expression expression12;
                expression12 = workPart.Expressions.CreateSystemExpressionWithUnits("p40_zdelta=0.00000000000", unit1);

                Expression expression13;
                expression13 = workPart.Expressions.CreateSystemExpressionWithUnits("p41_radius=0.00000000000", unit1);

                Expression expression14;
                expression14 = workPart.Expressions.CreateSystemExpressionWithUnits("p42_angle1=0.00000000000", unit2);

                Expression expression15;
                expression15 = workPart.Expressions.CreateSystemExpressionWithUnits("p43_angle2=0.00000000000", unit2);

                Expression expression16;
                expression16 = workPart.Expressions.CreateSystemExpressionWithUnits("p44_distance=0", unit1);

                Expression expression17;
                expression17 = workPart.Expressions.CreateSystemExpressionWithUnits("p45_arclen=0", unit1);

                Unit nullUnit = null;
                Expression expression18;
                expression18 = workPart.Expressions.CreateSystemExpressionWithUnits("p46_percent=0", nullUnit);

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

                expression15.RightHandSide = "0";

                expression16.RightHandSide = "0";

                expression18.RightHandSide = "100";

                expression17.RightHandSide = "0";
                
                theSession.SetUndoMarkName(markId2, "Point Dialog");
                

                Expression expression19;
                expression19 = workPart.Expressions.CreateSystemExpressionWithUnits("p47_x=0.00000000000", unit1);

                Scalar scalar1;
                scalar1 = workPart.Scalars.CreateScalarExpression(expression19, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression20;
                expression20 = workPart.Expressions.CreateSystemExpressionWithUnits("p48_y=0.00000000000", unit1);

                Scalar scalar2;
                scalar2 = workPart.Scalars.CreateScalarExpression(expression20, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression21;
                expression21 = workPart.Expressions.CreateSystemExpressionWithUnits("p49_z=0.00000000000", unit1);

                Scalar scalar3;
                scalar3 = workPart.Scalars.CreateScalarExpression(expression21, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Point point1;
                point1 = workPart.Points.CreatePoint(scalar1, scalar2, scalar3, NXOpen.SmartObject.UpdateOption.WithinModeling);

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
                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

                theSession.DeleteUndoMark(markId3, null);

                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Point");

                workPart.Points.DeletePoint(point1);

                Expression expression22;
                expression22 = workPart.Expressions.CreateSystemExpressionWithUnits("p33_x=0.00000000000", unit1);

                Scalar scalar4;
                scalar4 = workPart.Scalars.CreateScalarExpression(expression22, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression23;
                expression23 = workPart.Expressions.CreateSystemExpressionWithUnits("p34_y=0.00000000000", unit1);

                Scalar scalar5;
                scalar5 = workPart.Scalars.CreateScalarExpression(expression23, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Expression expression24;
                expression24 = workPart.Expressions.CreateSystemExpressionWithUnits("p35_z=0.00000000000", unit1);

                Scalar scalar6;
                scalar6 = workPart.Scalars.CreateScalarExpression(expression24, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Point point2;
                point2 = workPart.Points.CreatePoint(scalar4, scalar5, scalar6, NXOpen.SmartObject.UpdateOption.WithinModeling);

                expression4.RightHandSide = "0";

                expression5.RightHandSide = "0";

                expression6.RightHandSide = "0";

                expression4.RightHandSide = "0.00000000000";

                expression5.RightHandSide = "0.00000000000";

                expression6.RightHandSide = "0.00000000000";

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

                Scalar scalar7;
                scalar7 = workPart.Scalars.CreateScalarExpression(expression22, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Scalar scalar8;
                scalar8 = workPart.Scalars.CreateScalarExpression(expression23, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Scalar scalar9;
                scalar9 = workPart.Scalars.CreateScalarExpression(expression24, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Point point3;
                point3 = workPart.Points.CreatePoint(scalar7, scalar8, scalar9, NXOpen.SmartObject.UpdateOption.WithinModeling);
                */

                //axis1.Point = point2;
                Point3d a = new Point3d(0.0, 0.0, 0.0);
                Point b = workPart.Points.CreatePoint(a);
                axis1.Point = b;

                moveObjectBuilder1.TransformMotion.AngularAxis = axis1;

                moveObjectBuilder1.TransformMotion.Angle.RightHandSide = (-1 * angle).ToString();

                moveObjectBuilder1.TransformMotion.Angle.RightHandSide = (-1 * angle).ToString();

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

                //workPart.Points.DeletePoint(point3);

                //workPart.Expressions.Delete(expression2);

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

        public static bool J_TrimmedSheetBodyTest(Body TargetBody, Body ToolBody, Edge[] ToolEdgeAry, Face[] ToolFaceAry, out NXObject nXObject1)
        {
            nXObject1 = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Trim->Trimmed Sheet...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.Feature nullFeatures_Feature = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.TrimSheetBuilder trimSheetBuilder1;
                trimSheetBuilder1 = workPart.Features.CreateTrimsheetBuilder(nullFeatures_Feature);

                trimSheetBuilder1.Tolerance = 0.002;

                theSession.SetUndoMarkName(markId1, "Trimmed Sheet Dialog");

                Body body1 = (Body)workPart.Bodies.FindObject(TargetBody.JournalIdentifier);
                bool added1;
                added1 = trimSheetBuilder1.TargetBodies.Add(body1);

                Point3d coordinates1 = new Point3d(-30.4267273348854, -9.37502123850766, -10.0);
                Point point1;
                point1 = workPart.Points.CreatePoint(coordinates1);

                RegionPoint regionPoint1;
                regionPoint1 = workPart.CreateRegionPoint(point1, body1);

                trimSheetBuilder1.Regions.Append(regionPoint1);

                Direction nullDirection = null;
                trimSheetBuilder1.ProjectionDirection.ProjectVector = nullDirection;

                Section section1;
                section1 = workPart.Sections.CreateSection(0.0019, 0.002, 0.01);

                section1.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

                bool added2;
                added2 = trimSheetBuilder1.BoundaryObjects.Add(section1);

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                Edge[] edges1 = new Edge[ToolEdgeAry.Length];
                //NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject("UNPARAMETERIZED_FEATURE(1)");
                edges1 = ToolEdgeAry;
                
                EdgeDumbRule edgeDumbRule1;
                edgeDumbRule1 = workPart.ScRuleFactory.CreateRuleEdgeDumb(edges1);

                section1.AllowSelfIntersection(true);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = edgeDumbRule1;
                NXObject nullNXObject = null;
                Point3d helpPoint1 = new Point3d(0.0, 0.0, 0.0);
                section1.AddToSection(rules1, nullNXObject, nullNXObject, nullNXObject, helpPoint1, NXOpen.Section.Mode.Create, false);

                theSession.DeleteUndoMark(markId3, null);

                ScCollector scCollector1;
                scCollector1 = workPart.ScCollectors.CreateCollector();

                Face[] faces1 = ToolFaceAry;


                FaceDumbRule faceDumbRule1;
                faceDumbRule1 = workPart.ScRuleFactory.CreateRuleFaceDumb(faces1);

                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = faceDumbRule1;
                scCollector1.ReplaceRules(rules2, false);

                bool added3;
                added3 = trimSheetBuilder1.BoundaryObjects.Add(scCollector1);

                trimSheetBuilder1.ProjectionDirection.ProjectVector = nullDirection;

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Trimmed Sheet");

                theSession.DeleteUndoMark(markId4, null);

                NXOpen.Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Trimmed Sheet");

                //NXObject nXObject1;
                nXObject1 = trimSheetBuilder1.Commit();

                theSession.DeleteUndoMark(markId5, null);

                theSession.SetUndoMarkName(markId1, "Trimmed Sheet");

                trimSheetBuilder1.Destroy();

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

        public static bool J_Project(Face TargetFace, Edge[] ToolEdgeAry, out NXObject nXObject1)
        {
            nXObject1 = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Curve from Curves->Project...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.Feature nullFeatures_Feature = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.ProjectCurveBuilder projectCurveBuilder1;
                projectCurveBuilder1 = workPart.Features.CreateProjectCurveBuilder(nullFeatures_Feature);

                Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
                Vector3d normal1 = new Vector3d(0.0, 0.0, 1.0);
                Plane plane1;
                plane1 = workPart.Planes.CreatePlane(origin1, normal1, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
                Expression expression1;
                expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                Expression expression2;
                expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                projectCurveBuilder1.CurveFitData.Tolerance = 0.002;

                projectCurveBuilder1.CurveFitData.AngleTolerance = 0.01;

                projectCurveBuilder1.AngleToProjectionVector.RightHandSide = "0";

                theSession.SetUndoMarkName(markId1, "Project Curve Dialog");

                projectCurveBuilder1.SectionToProject.DistanceTolerance = 0.002;

                projectCurveBuilder1.SectionToProject.ChainingTolerance = 0.0019;

                projectCurveBuilder1.SectionToProject.AngleTolerance = 0.01;

                projectCurveBuilder1.SectionToProject.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.CurvesAndPoints);

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                Edge[] edges1 = ToolEdgeAry;

                EdgeDumbRule edgeDumbRule1;
                edgeDumbRule1 = workPart.ScRuleFactory.CreateRuleEdgeDumb(edges1);

                projectCurveBuilder1.SectionToProject.AllowSelfIntersection(true);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = edgeDumbRule1;
                NXObject nullNXObject = null;
                Point3d helpPoint1 = new Point3d(0.0, 0.0, 0.0);
                projectCurveBuilder1.SectionToProject.AddToSection(rules1, nullNXObject, nullNXObject, nullNXObject, helpPoint1, NXOpen.Section.Mode.Create, false);

                theSession.DeleteUndoMark(markId3, null);

                theSession.DeleteUndoMark(markId2, null);

                plane1.SetMethod(NXOpen.PlaneTypes.MethodType.Distance);

                NXObject[] geom1 = new NXObject[1];
                //NXOpen.Features.Brep brep2 = (NXOpen.Features.Brep)workPart.Features.FindObject("UNPARAMETERIZED_FEATURE(2)");
                //Face face1 = (Face)brep2.FindObject("FACE 1 {(0,0,-10) UNPARAMETERIZED_FEATURE(2)}");
                Face face1 = TargetFace;
                geom1[0] = face1;
                plane1.SetGeometry(geom1);

                plane1.SetFlip(false);

                plane1.SetReverseSide(false);

                Expression expression3;
                expression3 = plane1.Expression;

                expression3.RightHandSide = "0";

                plane1.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);

                plane1.Evaluate();

                plane1.SetMethod(NXOpen.PlaneTypes.MethodType.Distance);

                NXObject[] geom2 = new NXObject[1];
                geom2[0] = face1;
                plane1.SetGeometry(geom2);

                plane1.SetFlip(false);

                plane1.SetReverseSide(false);

                Expression expression4;
                expression4 = plane1.Expression;

                expression4.RightHandSide = "0";

                plane1.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);

                plane1.Evaluate();

                projectCurveBuilder1.PlaneToProjectTo = plane1;

                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Project Curve");

                theSession.DeleteUndoMark(markId4, null);

                NXOpen.Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Project Curve");

                //NXObject nXObject1;
                nXObject1 = projectCurveBuilder1.Commit();

                theSession.DeleteUndoMark(markId5, null);

                theSession.SetUndoMarkName(markId1, "Project Curve");

                projectCurveBuilder1.SectionToProject.CleanMappingData();

                projectCurveBuilder1.SectionToProject.CleanMappingData();

                projectCurveBuilder1.Destroy();

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

        public static bool J_NewTrimmedSheet(Body PlaneBody, NXObject[] NXObj, out NXObject nXObject1)
        {
            nXObject1 = null;
            try
            {
                List<Line> ListLine = new List<Line>();
                List<Arc> ListArc = new List<Arc>();
                List<Spline> ListSpline = new List<Spline>();
                List<Ellipse> ListEllipse = new List<Ellipse>();

                int count = 0;

                bool chk_Line = false;
                bool chk_Arc = false;
                bool chk_Spline = false;
                bool chk_Ellipse = false;

                //CaxLog.ShowListingWindow("1");
                SeparateCurveType(NXObj,
                    out ListLine, out chk_Line,
                    out ListArc, out chk_Arc,
                    out ListSpline, out chk_Spline,
                    out ListEllipse, out chk_Ellipse,
                    out count);
                //CaxLog.ShowListingWindow("2");

                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Trim->Trimmed Sheet...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.Feature nullFeatures_Feature = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.TrimSheetBuilder trimSheetBuilder1;
                trimSheetBuilder1 = workPart.Features.CreateTrimsheetBuilder(nullFeatures_Feature);

                trimSheetBuilder1.Tolerance = 0.002;

                theSession.SetUndoMarkName(markId1, "Trimmed Sheet Dialog");

                //Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(2)");
                Body body1 = PlaneBody;
                bool added1;
                added1 = trimSheetBuilder1.TargetBodies.Add(body1);

                Point3d coordinates1 = new Point3d(-31.9181142487178, -10.4043583199806, -10.0);
                Point point1;
                point1 = workPart.Points.CreatePoint(coordinates1);

                RegionPoint regionPoint1;
                regionPoint1 = workPart.CreateRegionPoint(point1, body1);

                trimSheetBuilder1.Regions.Append(regionPoint1);

                Direction nullDirection = null;
                trimSheetBuilder1.ProjectionDirection.ProjectVector = nullDirection;

                Section section1;
                section1 = workPart.Sections.CreateSection(0.0019, 0.002, 0.01);

                section1.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

                bool added2;
                added2 = trimSheetBuilder1.BoundaryObjects.Add(section1);

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                section1.AllowSelfIntersection(true);
                SelectionIntentRule[] rules1 = new SelectionIntentRule[count];
                for (int i = 0; i < count; i++)
                {
                    CurveDumbRule curveDumbRule1;
                    if (chk_Line)
                    {
                        chk_Line = false;
                        IBaseCurve[] curves1 = ListLine.ToArray();
                        curveDumbRule1 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves1);
                        rules1[i] = curveDumbRule1;
                        continue;
                    }
                    if (chk_Arc)
                    {
                        chk_Arc = false;
                        IBaseCurve[] curves1 = ListArc.ToArray();
                        curveDumbRule1 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves1);
                        rules1[i] = curveDumbRule1;
                        continue;
                    }
                    if (chk_Spline)
                    {
                        chk_Spline = false;
                        IBaseCurve[] curves1 = ListSpline.ToArray();
                        curveDumbRule1 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves1);
                        rules1[i] = curveDumbRule1;
                        continue;
                    }
                    if (chk_Ellipse)
                    {
                        chk_Ellipse = false;
                        IBaseCurve[] curves1 = ListEllipse.ToArray();
                        curveDumbRule1 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves1);
                        rules1[i] = curveDumbRule1;
                        continue;
                    }
                }

                NXObject nullNXObject = null;
                Point3d helpPoint1 = new Point3d(0.0, 0.0, 0.0);
                section1.AddToSection(rules1, nullNXObject, nullNXObject, nullNXObject, helpPoint1, NXOpen.Section.Mode.Create, false);

                theSession.DeleteUndoMark(markId3, null);

                trimSheetBuilder1.ProjectionDirection.ProjectVector = nullDirection;

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Trimmed Sheet");

                theSession.DeleteUndoMark(markId4, null);

                NXOpen.Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Trimmed Sheet");

                //NXObject nXObject1;
                nXObject1 = trimSheetBuilder1.Commit();

                theSession.DeleteUndoMark(markId5, null);

                theSession.SetUndoMarkName(markId1, "Trimmed Sheet");

                trimSheetBuilder1.Destroy();

                // ----------------------------------------------
                //   Menu: Tools->Journal->Stop Recording
                // ----------------------------------------------
            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow("ex:"+ex);
                return false;
            }
            return true;
        }
        
        public static bool J_CopyPaste(Body NewWEBody,out NXObject CopyBodyNXObj,out Body CopyBody)
        {
            CopyBodyNXObj = null;
            CopyBody = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Edit->Copy
                // ----------------------------------------------
                workPart.PmiManager.RestoreUnpastedObjects();

                // ----------------------------------------------
                //   Menu: Edit->Paste
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXObject[] features1 = new NXObject[1];
                NXOpen.Features.Brep brep1 = (NXOpen.Features.Brep)workPart.Features.FindObject(NewWEBody.JournalIdentifier);
                features1[0] = brep1;
                NXOpen.Features.CopyPasteBuilder copyPasteBuilder1;
                copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);

                copyPasteBuilder1.SetBuilderVersion((NXOpen.Features.CopyPasteBuilder.BuilderVersion)(7));

                NXOpen.Features.FeatureReferencesBuilder featureReferencesBuilder1;
                featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();

                Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
                Vector3d normal1 = new Vector3d(0.0, 0.0, 1.0);
                Plane plane1;
                plane1 = workPart.Planes.CreatePlane(origin1, normal1, NXOpen.SmartObject.UpdateOption.WithinModeling);

                Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
                Expression expression1;
                expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                Expression expression2;
                expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                featureReferencesBuilder1.AutomaticMatch(true);

                theSession.SetUndoMarkName(markId1, "Paste Feature Dialog");

                NXOpen.Features.MatchedReferenceBuilder[] matchedReferenceData1;
                matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Paste Feature");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Paste Feature");

                NXObject nXObject1;
                nXObject1 = copyPasteBuilder1.Commit();
                CopyBodyNXObj = nXObject1;

                NXOpen.Features.Brep Feat = (NXOpen.Features.Brep)nXObject1;
                CaxFeat.GetFeatBody(Feat, out CopyBody);
                

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "Paste Feature");

                NXOpen.Features.Brep brep2 = (NXOpen.Features.Brep)nXObject1;
                Expression[] expressions1;
                expressions1 = brep2.GetExpressions();

                copyPasteBuilder1.Destroy();

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
                return false;
            }
            return true;
        }

        public static bool BodyToHide(Body BodyToHide)
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Hide");

                DisplayableObject[] objects1 = new DisplayableObject[1];
                //NXOpen.Assemblies.Component component1 = (NXOpen.Assemblies.Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT BOMTEST030_M001_ET110_WEDMS1_Z 1");
                //Body body1 = (Body)component1.FindObject("PROTO#.Bodies|UNPARAMETERIZED_FEATURE(2)");
                //objects1[0] = body1;
                objects1[0] = BodyToHide;
                theSession.DisplayManager.BlankObjects(objects1);

                displayPart.ModelingViews.WorkView.FitAfterShowOrHide(NXOpen.View.ShowOrHideType.HideOnly);

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

        public static bool J_Thicken(Body TargetBody,out Body CreateSolidBody)
        {
            CreateSolidBody = null;
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Insert->Offset/Scale->Thicken...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.Features.Feature nullFeatures_Feature = null;

                if (!workPart.Preferences.Modeling.GetHistoryMode())
                {
                    throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
                }

                NXOpen.Features.ThickenBuilder thickenBuilder1;
                thickenBuilder1 = workPart.Features.CreateThickenBuilder(nullFeatures_Feature);

                thickenBuilder1.Tolerance = 0.002;

                thickenBuilder1.FirstOffset.RightHandSide = "0";

                thickenBuilder1.SecondOffset.RightHandSide = "2";

                thickenBuilder1.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

                Body[] targetBodies1 = new Body[1];
                Body nullBody = null;
                targetBodies1[0] = nullBody;
                thickenBuilder1.BooleanOperation.SetTargetBodies(targetBodies1);

                theSession.SetUndoMarkName(markId1, "##11Thicken Dialog");

                //Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(3)");
                Body body1 = TargetBody;
                FaceBodyRule faceBodyRule1;
                faceBodyRule1 = workPart.ScRuleFactory.CreateRuleFaceBody(body1);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = faceBodyRule1;
                thickenBuilder1.FaceCollector.ReplaceRules(rules1, false);

                thickenBuilder1.ReverseDirection = true;

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "##11Thicken");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "##11Thicken");

                NXObject nXObject1;
                nXObject1 = thickenBuilder1.Commit();
                //CaxLog.ShowListingWindow("nXObject1.type:" + nXObject1.GetType());
                NXOpen.Features.Thicken a = (NXOpen.Features.Thicken)nXObject1;
                CaxFeat.GetFeatBody(a, out CreateSolidBody);
                

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "##11Thicken");

                Expression expression1 = thickenBuilder1.SecondOffset;
                Expression expression2 = thickenBuilder1.FirstOffset;
                thickenBuilder1.Destroy();

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

        public static void TESTWAVE(Component Comp, Body oldbody,out NXOpen.Features.Feature wavelink)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            //CaxLog.ShowListingWindow(Path.GetFileNameWithoutExtension(workPart.FullPath));
            //CaxLog.ShowListingWindow(Path.GetFileNameWithoutExtension(oldbody.OwningPart.FullPath));
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

            //NXOpen.Features.WaveDatumBuilder waveDatumBuilder1;
            //waveDatumBuilder1 = waveLinkBuilder1.WaveDatumBuilder;

            //NXOpen.Features.CompositeCurveBuilder compositeCurveBuilder1;
            //compositeCurveBuilder1 = waveLinkBuilder1.CompositeCurveBuilder;

            //NXOpen.Features.WaveSketchBuilder waveSketchBuilder1;
            //waveSketchBuilder1 = waveLinkBuilder1.WaveSketchBuilder;

            //NXOpen.Features.WaveRoutingBuilder waveRoutingBuilder1;
            //waveRoutingBuilder1 = waveLinkBuilder1.WaveRoutingBuilder;

            //NXOpen.Features.WavePointBuilder wavePointBuilder1;
            //wavePointBuilder1 = waveLinkBuilder1.WavePointBuilder;

            NXOpen.Features.ExtractFaceBuilder extractFaceBuilder1;
            extractFaceBuilder1 = waveLinkBuilder1.ExtractFaceBuilder;

            //NXOpen.Features.MirrorBodyBuilder mirrorBodyBuilder1;
            //mirrorBodyBuilder1 = waveLinkBuilder1.MirrorBodyBuilder;

            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

            waveLinkBuilder1.Type = NXOpen.Features.WaveLinkBuilder.Types.BodyLink;

            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

            extractFaceBuilder1.AngleTolerance = 45.0;

            //waveDatumBuilder1.DisplayScale = 2.0;

            extractFaceBuilder1.ParentPart = NXOpen.Features.ExtractFaceBuilder.ParentPartType.OtherPart;

           // mirrorBodyBuilder1.ParentPartType = NXOpen.Features.MirrorBodyBuilder.ParentPart.OtherPart;

            theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker Dialog");

            //compositeCurveBuilder1.Section.DistanceTolerance = 0.002;

            //compositeCurveBuilder1.Section.ChainingTolerance = 0.0019;

            extractFaceBuilder1.Associative = true;

            extractFaceBuilder1.MakePositionIndependent = false;

            extractFaceBuilder1.FixAtCurrentTimestamp = false;

            extractFaceBuilder1.HideOriginal = false;

            extractFaceBuilder1.InheritDisplayProperties = false;

            SelectObjectList selectObjectList1;
            selectObjectList1 = extractFaceBuilder1.BodyToExtract;

            extractFaceBuilder1.CopyThreads = true;

            //NXOpen.Assemblies.Component component1 = (NXOpen.Assemblies.Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT X903J-0011T_U100_WE 1");
            Body body1 = (Body)Comp.FindObject("PROTO#.Bodies|" + oldbody.JournalIdentifier);
           // Body body1 = oldbody;
            bool added1;
            added1 = selectObjectList1.Add(body1);


            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

            NXObject nXObject1;
            nXObject1 = waveLinkBuilder1.Commit();

            wavelink = (NXOpen.Features.Feature)nXObject1;

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker");

            extractFaceBuilder1.Destroy();
            waveLinkBuilder1.Destroy();


            // ----------------------------------------------
            //   Menu: Tools->Journal->Stop Recording
            // ----------------------------------------------

        }

        public static bool WAVEGeometry(Component component, Body body, out NXOpen.Features.Feature wavelink)
        {
            wavelink = null;
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
                //CaxLog.ShowListingWindow("body1CisoCC:" + body1.IsOccurrence);
                //CaxLog.ShowListingWindow(body1.IsOccurrence.ToString());
                //CaxLog.ShowListingWindow(Path.GetFileNameWithoutExtension(body1.OwningPart.FullPath));
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

                wavelink = (NXOpen.Features.Feature)nXObject1;

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker");

                waveLinkBuilder1.Destroy();

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

        public static bool WaveLinkBody(Component component, Body body, out NXOpen.Features.Feature wavelink)
        {
            wavelink = null;
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

                CaxAsm.SetWorkComponent(component);
                //NXOpen.Assemblies.Component component1 = (NXOpen.Assemblies.Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT X903J-0011T_U100_WE 1");
                //NXOpen.Assemblies.Component component1 = component;

                //Body body1 = (Body)component1.FindObject("PROTO#.Bodies|" + body.JournalIdentifier);
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

                wavelink = (NXOpen.Features.Feature)nXObject1;

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker");

                waveLinkBuilder1.Destroy();

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

        public static bool GetCompanyData(string jsonPath, out COMPANY_ARY cCOMPANY_ARY)
        {
            cCOMPANY_ARY = new COMPANY_ARY();

            try
            {
                bool status;

                //判斷檔案是否存在
                if (!System.IO.File.Exists(jsonPath))
                {
                    return false;
                }
                string jsonText;
                status = ReadFileDataUTF8(jsonPath, out jsonText);
                if (!status)
                {
                    return false;
                }
                cCOMPANY_ARY = JsonConvert.DeserializeObject<COMPANY_ARY>(jsonText);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool ReadFileDataUTF8(string file_path, out string allContent)
        {
            allContent = "";

            if (!System.IO.File.Exists(file_path))
            {
                return false;
            }

            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(file_path, Encoding.UTF8);

            int index = 0;
            while ((line = file.ReadLine()) != null)
            {
                if (index == 0)
                {
                    allContent += line;
                }
                else
                {
                    allContent += "\n";
                    allContent += line;
                }
                index++;
            }
            file.Close();

            return true;
        }
    }
}
