using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NXOpen;
using NXOpen.UF;
using System.Collections;
using CimforceCaxTwPublic;

/*
using System;
using System.Collections;
using System.IO;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;
using NXOpen.Assemblies;*/
using NXOpen.Utilities;
using NXOpen.CAM;
using NXOpen.Features;
using CimforceCaxTwPublic;
using DevComponents.DotNetBar.SuperGrid;
using DevComponents.DotNetBar;
using CimforceCaxTwCNC;
using System.Diagnostics;
using Newtonsoft.Json;


namespace ETableWork
{
    public partial class eTbale : DevComponents.DotNetBar.Office2007Form
    {
        private static Session theSession;
        private static UI theUI;
        private static UFSession theUfSession;

        private static ArrayList listSolidObjectToExport = new ArrayList();
        private static string VIEW_DADA = "";
        private static Matrix3x3 VIEW_MATRIX;
        private static double VIEW_SCALE;

        private static double FACET_TOLER = 0.002;
        private static double ADJ_TOLER = 0.002;

        private static double FIXTURE_FACET_TOLER = 0.08;
        private static double FIXTURE_ADJ_TOLER = 0.08;

        //圖片檔名
        private static string IMG_YES = "yes_16.png";
        private static string IMG_NO = "no_16.png";

        CaxLoadingDlg sCaxLoadingDlg = null;

        //20150216 刀具-精銑面對應By Andy
        public class FaceToolMappingConfig
        {
            public string MappingType { get; set; }
            public string pointMaxDist { get; set; }
            public string pointMinDist { get; set; }
        }
        public class FaceToolMap
        {
            public string finishFace { get; set; }
            public List<string> finishTool { get; set; }
        }
        public class FaceToolMapping
        {
            public string MOLD_NO { get; set; }
            public string DES_VER_NO { get; set; }
            public string WORK_NO { get; set; }
            public string PART_NO { get; set; }
            public string MFC_NO { get; set; }
            public string MFC_TASK_NO { get; set; }
            public string MAC_MODEL { get; set; }
            public string TASK_NO { get; set; }
            public string TASK_SRNO { get; set; }
            public List<FaceToolMap> FaceToolMapList { get; set; }
        }
        public static double pointMaxDist;
        public static double pointMinDist;

        public eTbale()
        {
            InitializeComponent();
            InitializeGrid();
            checkBoxIS_POST.Checked = true;
            checkBoxIS_SHOPDOC.Checked = true;
            checkBoxIS_STL.Checked = true;
            checkBoxIS_CMM.Checked = false;
            VIEW_SCALE = 0;

            theSession = Session.GetSession();
            theUI = UI.GetUI();
            theUfSession = UFSession.GetUFSession();
        }

        private void InitializeGrid()
        {
            GridPanel panel = superGridControlOper.PrimaryGrid;

            // Set the ImageEdit column EditorType to our EditControl type
            // so that we can send it the ImageList and SizeMode to use

            GridColumn column = panel.Columns["GridColumnStatus"];
            column.EditorType = typeof(MyGridImageEditControl);
            column.EditorParams = new object[] { imageList1, ImageSizeMode.Zoom };

            // Set the LabelX column EditorType to our EditControl type
            // so that we can handle the link reference mouse clicks

//             column = panel.Columns["LabelX"];
//             column.EditorType = typeof(MyGridLabelXEditControl);
        }

        #region MyGridImageEditControl

        /// <summary>
        /// GridImageEditControl with the ability
        /// to pass in a default ImageList and ImageBoxSizeMode
        /// </summary>
        private class MyGridImageEditControl : GridImageEditControl
        {
            public MyGridImageEditControl(
                ImageList imageList, ImageSizeMode sizeMode)
            {
                ImageList = imageList;
                ImageSizeMode = sizeMode;
            }
        }

        #endregion

        private bool RunPost()
        {
            //MessageBox.Show(ROOT_PATH);
            
            //Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Tools->Operation Navigator->Output->NX Post Postprocess...
            // ----------------------------------------------
            string postOutPath = "";

            NXOpen.CAM.CAMObject[] objects1 = new NXOpen.CAM.CAMObject[1];
            for (int i = 0; i < ListToolLengehAry.Count; i++)
            {
                postOutPath = string.Format(@"{0}\{1}\{2}.{3}", ROOT_PATH, "POST", ListToolLengehAry[i].oper_name, machine_group);

                NXOpen.CAM.CAMObject camObj = (NXOpen.CAM.CAMObject)workPart.CAMSetup.CAMOperationCollection.FindObject(ListToolLengehAry[i].oper_name);
                objects1[0] = camObj;
                workPart.CAMSetup.PostprocessWithSetting(objects1, postFunction, postOutPath, NXOpen.CAM.CAMSetup.OutputUnits.PostDefined, NXOpen.CAM.CAMSetup.PostprocessSettingsOutputWarning.PostDefined, NXOpen.CAM.CAMSetup.PostprocessSettingsReviewTool.PostDefined);
            }
            
            return true;
        }

        private bool RunShopDoc(string taskRootDir)
        {
            bool status;
            CaxLog.WriteLog("taskRootDir : " + taskRootDir);

            try
            {
                //顯示零件的目錄
                string displayPartPath = Path.GetDirectoryName(theSession.Parts.Display.FullPath);
                CaxLog.WriteLog("displayPartPath : " + displayPartPath);


                //建立SHOP_DOC目錄
                string shopdocFolder = "";
                shopdocFolder = string.Format(@"{0}\{1}", taskRootDir, "SHOP_DOC");
                CaxFile.CreateWinFolder(shopdocFolder);
                CaxLog.WriteLog("shopdocFolder : " + shopdocFolder);

                //建立SHOP_DOC_TEMP目錄
                string shopdocTempFolder = "";
                shopdocTempFolder = string.Format(@"{0}\{1}", displayPartPath, "SHOP_DOC_TEMP");
                CaxFile.CreateWinFolder(shopdocTempFolder);
                CaxLog.WriteLog("shopdocTempFolder : " + shopdocTempFolder);

                //設定SHOPDOC輸出路徑
                string shopdocFile = "";
                shopdocFile = string.Format(@"{0}\{1}", taskRootDir, "SHOPDOC_CAM");
                CaxLog.WriteLog("shopdocFile : " + shopdocFile);

//                 //輸出執行GenerateDoc時，寫出檔案的目錄
//                 string shopdocOutputPath = string.Format(@"{0}\{1}\{2}", displayPartPath, "SHOP_DOC", "shopdocOutput.txt");
//                 CaxLog.WriteLog("shopdocOutputPath : " + shopdocOutputPath);
// 
//                 if (System.IO.File.Exists(shopdocOutputPath))
//                 {
//                     CaxLog.WriteLog("Delete shopdocOutputPath : " + shopdocOutputPath);
//                     System.IO.File.Delete(shopdocOutputPath);
//                 }
//                 CaxFile.WriteFileData(shopdocOutputPath, shopdocFolder);

                //執行Shop Documentation
                CaxLog.WriteLog("SHOPDOC_OPER_NAME : " + SHOPDOC_OPER_NAME);
                theUfSession.Shopdoc.GenerateDoc(SHOPDOC_OPER_NAME, shopdocFile, UFSetup.OutputUnits.OutputUnitsMetric);

                //抓取刀具路徑圖
                CaxLog.WriteLog("ExportOprationToolPathPhoto");
                ExportOprationToolPathPhoto(shopdocTempFolder);


                //移動裝夾圖
                string tempPhotoPath = "";
                string shopdocPhotoPath = "";

                tempPhotoPath = string.Format(@"{0}\{1}.jpg", displayPartPath, labelXWorkSection.Text);
                shopdocPhotoPath = string.Format(@"{0}\{1}.jpg", shopdocFolder, labelXWorkSection.Text);
                //CaxLog.WriteLog(tempPhotoPath);
                if (System.IO.File.Exists(tempPhotoPath))
                {
                    System.IO.File.Copy(tempPhotoPath, shopdocPhotoPath, true);
                }

                tempPhotoPath = string.Format(@"{0}\{1}.cgm", displayPartPath, labelXWorkSection.Text);
                shopdocPhotoPath = string.Format(@"{0}\{1}.cgm", shopdocFolder, labelXWorkSection.Text);
                //CaxLog.WriteLog(tempPhotoPath);
                if (System.IO.File.Exists(tempPhotoPath))
                {
                    System.IO.File.Copy(tempPhotoPath, shopdocPhotoPath, true);
                }
				
                //刪除 SHOPDOC_CAM
                if (System.IO.File.Exists(shopdocFile))
                {
                    System.IO.File.Delete(shopdocFile);
                }
                CaxLog.WriteLog("RunShopDoc Done");


                string[] shopdocTempFolderFileAry = System.IO.Directory.GetFiles(shopdocTempFolder);
                for (int i = 0; i < shopdocTempFolderFileAry.Length; i++)
                {
                    string newFilePath = "";
                    newFilePath = string.Format(@"{0}\{1}", shopdocFolder, Path.GetFileName(shopdocTempFolderFileAry[i]));
                    System.IO.File.Copy(shopdocTempFolderFileAry[i], newFilePath,true);
                    System.IO.File.Delete(shopdocTempFolderFileAry[i]);
                }


                /*
                //取得系統TEMPLATES目錄
                //string temp_folder = Environment.GetFolderPath(Environment.SpecialFolder.Templates);
                string temp_folder = CaxCNC.GetCimLocalProgramsDir() + @"\TEMP";


                //移動裝夾圖
                tempPhotoPath = string.Format(@"{0}\{1}.png", temp_folder, labelXWorkSection.Text);
                shopdocPhotoPath = string.Format(@"{0}\{1}.png", shopdocFolder, labelXWorkSection.Text);
                if (System.IO.File.Exists(tempPhotoPath))
                {
                    System.IO.File.Copy(tempPhotoPath, shopdocPhotoPath, true);
                    System.IO.File.Delete(tempPhotoPath);
                }

                //刪除TEMP內的檔案
                string[] tempFiles = Directory.GetFiles(temp_folder);
                for (int i = 0; i < tempFiles.Length;i++ )
                {
                    if (System.IO.File.Exists(tempFiles[i]))
                    {
                        try
                        {
                            System.IO.File.Delete(tempFiles[i]);
                        }
                        catch (System.Exception ex)
                        {
                    	
                        }
                    }
                }
                */
            }
            catch (System.Exception ex)
            {
                CaxLog.WriteLog(ex.ToString());
                return false;
            }

            return true;
        }

        // 20150525 Stewart 將多個component輸出至同一個stl檔，mainComponent控制輸出檔案之位置和名稱，其餘subComponentLst僅加入bodyOcc
        private bool ExportBinarySTL_multiComponent(NXOpen.Assemblies.Component mainComponent, List<NXOpen.Assemblies.Component> subComponentLst, double facet_tol, double adj_tol)
        {
            bool status;
            //theSession = Session.GetSession();
            //theUfSession = UFSession.GetUFSession();
            //Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            //theUI = UI.GetUI();

            Part part = null;
            if (mainComponent == null)
            {
                part = displayPart;
            }
            else
            {
                part = (Part)mainComponent.Prototype;
            }

            //取得跟目錄
            string displayDir = Path.GetDirectoryName(displayPart.FullPath);

            //取得沒有副檔名的檔案名稱
            string fileName = Path.GetFileNameWithoutExtension(part.FullPath);

            //取得輸出STL的全路徑
            string stlFile = string.Format(@"{0}\{1}", displayDir, fileName);

            //string stlFile = (part.FullPath.Replace(".prt", ""));
            IntPtr fileHandle = IntPtr.Zero;

            try
            {
                List<Body> listSolidObjectToExport = new List<Body>();
                //theUfSession.Std.OpenTextStlFile(stlFile, false, out fileHandle);
                theUfSession.Std.OpenBinaryStlFile(stlFile, false, fileName, out fileHandle);
                status = CreateListOfObjectToExport(part, mainComponent, out listSolidObjectToExport);
                if (!status)
                {
                    return false;
                }

                foreach (NXOpen.Assemblies.Component TEMP_subComp in subComponentLst)
                {
                    if (TEMP_subComp != null)
                    {
                        List<Body> TEMP_listSolidObjectToExport = new List<Body>();
                        Part TEMP_part = (Part)TEMP_subComp.Prototype;
                        status = CreateListOfObjectToExport(TEMP_part, TEMP_subComp, out TEMP_listSolidObjectToExport);
                        if (!status)
                        {
                            return false;
                        }

//                         for (int i = 0; i < TEMP_listSolidObjectToExport.Count; i++)
//                         {
//                             listSolidObjectToExport.Add(TEMP_listSolidObjectToExport[i]);
//                         }
                        listSolidObjectToExport.AddRange(TEMP_listSolidObjectToExport);
                    }
                }

                status = PutBodiesInStlFile(fileHandle, listSolidObjectToExport, facet_tol, adj_tol);
                if (!status)
                {
                    return false;
                }

                theUfSession.Std.CloseStlFile(fileHandle);
            }
            catch (NXOpen.NXException ex)
            {
                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ex.Message);
                return false;
            }

            return true;
        }

        private bool CreateListOfObjectToExport(Part part, NXOpen.Assemblies.Component component, out List<Body> listSolidObjectToExport)
        {
            listSolidObjectToExport = new List<Body>();

            try
            {
                int type = 0;
                int subType = 0;
                int status = 0;

                string attr_value = "";
                try
                {
                    attr_value = part.GetStringAttribute(CaxDefineParam.ATTR_CIM_TYPE);
                }
                catch (System.Exception ex)
                {
                    attr_value = "";
                }

                bool is_design = false;
                // 20150525 Stewart
                if (attr_value == CaxDefineParam.ATTR_CIM_TYPE_DESIGN || attr_value == CaxDefineParam.ATTR_CIM_TYPE_SUB_DESIGN)
                {
                    is_design = true;
                }

                BodyCollection bodiesColl = part.Bodies;

                Body[] BodyAry = bodiesColl.ToArray();

                if (BodyAry.Length == 0)
                {
                    FeatureCollection Features = part.Features;
                    Feature[] FeatureAry = Features.ToArray();
                    for (int i = 0; i < FeatureAry.Length; i++)
                    {
                        NXObject[] EntitiesAry = FeatureAry[i].GetEntities();
                        int num = EntitiesAry.Length;
                    }
                }

                for (int i = 0; i < BodyAry.Length; i++)
                {
                    if (is_design)
                    {
                        if (BodyAry[i].Layer != CaxDefineParam.CIM_BODY_LAYER_NUM)
                        {
                            continue;
                        }
                    }

                    theUfSession.Obj.AskTypeAndSubtype(BodyAry[i].Tag, out type, out subType);
                    status = theUfSession.Obj.AskStatus(BodyAry[i].Tag);

                    if (type == UFConstants.UF_solid_type && status == UFConstants.UF_OBJ_ALIVE)
                    {
                        NXObject bodyObj = component.FindOccurrence(BodyAry[i]);
                        Body bodyOcc = (Body)bodyObj;

                        listSolidObjectToExport.Add(bodyOcc);
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        private bool PutBodiesInStlFile(IntPtr fileHandle, List<Body> listSolidObjectToExport, double facet_tol, double adj_tol)
        {
            try
            {
                Tag wcsTag = NXOpen.Tag.Null;
                Tag[] negatedTags;

                int bodyType = 0;
                int countOfFaces = 0;
                int errorNumber = 0;
                int numNegated = 0;

                UFStd.StlError[] errorsInfo;

                theUfSession.Csys.AskWcs(out wcsTag);

                //             for (int i = 0; i < listSolidObjectToExport.Count; i++)
                //             {
                foreach (Body body in listSolidObjectToExport)
                {


                    try
                    {
                        if (body.Tag != NXOpen.Tag.Null)
                        {
                            theUfSession.Modl.AskBodyType(body.Tag, out bodyType);

                            if (bodyType == UFConstants.UF_MODL_SOLID_BODY)
                            {
                                try
                                {
                                    countOfFaces = body.GetFaces().Length;
                                    theUfSession.Std.PutSolidInStlFile(fileHandle, wcsTag, body.Tag, 0, 100, facet_tol, out errorNumber, out errorsInfo);
                                }
                                catch (System.Exception ex)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                try
                                {
                                    Tag[] sheetsTags = new Tag[body.GetFaces().Length];

                                    if (body.GetFaces().Length == 1)
                                    {
                                        sheetsTags[0] = body.Tag;
                                        theUfSession.Std.PutSheetsInStlFile(fileHandle, wcsTag, 1, sheetsTags, 0, 100, facet_tol, adj_tol, out numNegated, out negatedTags, out errorNumber, out errorsInfo);
                                    }
                                    else
                                    {
                                        Face[] face = body.GetFaces();

                                        for (int j = 0; j < body.GetFaces().Length; j++)
                                        {
                                            theUfSession.Modl.ExtractFace(face[j].Tag, 0, out sheetsTags[j]);
                                        }

                                        theUfSession.Std.PutSheetsInStlFile(fileHandle, wcsTag, body.GetFaces().Length, sheetsTags, 0, 100, facet_tol, adj_tol, out numNegated, out negatedTags, out errorNumber, out errorsInfo);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    continue;
                                }



                                /* if body.GetFaces */
                            } /* if bodyType */
                        } /* if body.Tag */
                    }
                    catch (System.Exception ex)
                    {
                        continue;
                    }

                } /* foreach */
                //            } /* for i= 0 */
            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        // 20150525 Stewart 沒在用的function
//         private bool ExportBinarySTL(NXOpen.Tag partTag, NXOpen.Assemblies.Component component, double facet_tol, double adj_tol)
//         {
//             //theSession = Session.GetSession();
//             //theUfSession = UFSession.GetUFSession();
//             //Part workPart = theSession.Parts.Work;
//             Part displayPart = theSession.Parts.Display;
//             //theUI = UI.GetUI();
// 
//             Part part = (Part)NXObjectManager.Get(partTag);
// 
//             string displayDir = Path.GetDirectoryName(displayPart.FullPath);
//             string stlFile = string.Format(@"{0}\{1}", displayDir, Path.GetFileNameWithoutExtension(part.FullPath));
// 
//             //string stlFile = (part.FullPath.Replace(".prt", ""));
//             IntPtr fileHandle = IntPtr.Zero;
// 
//             string fileName = Path.GetFileNameWithoutExtension(part.FullPath);
// 
// 
//             try
//             {
//                 //theUfSession.Std.OpenTextStlFile(stlFile, false, out fileHandle);
//                 theUfSession.Std.OpenBinaryStlFile(stlFile, false, fileName, out fileHandle);
//                 CreateListOfObjectToExport(part, component);
//                 PutBodiesInStlFile(fileHandle, listSolidObjectToExport, facet_tol, adj_tol);
//                 theUfSession.Std.CloseStlFile(fileHandle);
//                 listSolidObjectToExport = new ArrayList();
//             }
//             catch (NXOpen.NXException ex)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ex.Message);
//             }
// 
// 
// 
//             return true;
//         }
//         
//         // 20150525 Stewart 將多個component輸出至同一個stl檔，mainComponent控制輸出檔案之位置和名稱，其餘subComponentLst僅加入bodyOcc
//         private bool ExportBinarySTL(NXOpen.Assemblies.Component mainComponent, List<NXOpen.Assemblies.Component> subComponentLst, double facet_tol, double adj_tol)
//         {
//             bool status;
//             //theSession = Session.GetSession();
//             //theUfSession = UFSession.GetUFSession();
//             //Part workPart = theSession.Parts.Work;
//             Part displayPart = theSession.Parts.Display;
//             //theUI = UI.GetUI();
// 
//             Part part = null;
//             if (mainComponent == null)
//             {
//                 part = displayPart;
//             }
//             else
//             {
//                 part = (Part)mainComponent.Prototype;
//             }
// 
//             //取得跟目錄
//             string displayDir = Path.GetDirectoryName(displayPart.FullPath);
// 
//             //取得沒有副檔名的檔案名稱
//             string fileName = Path.GetFileNameWithoutExtension(part.FullPath);
// 
//             //取得輸出STL的全路徑
//             string stlFile = string.Format(@"{0}\{1}", displayDir, fileName);
// 
//             //string stlFile = (part.FullPath.Replace(".prt", ""));
//             IntPtr fileHandle = IntPtr.Zero;
// 
//             try
//             {
//                 List<Body> listSolidObjectToExport = new List<Body>();
//                 //theUfSession.Std.OpenTextStlFile(stlFile, false, out fileHandle);
//                 theUfSession.Std.OpenBinaryStlFile(stlFile, false, fileName, out fileHandle);
//                 status = CreateListOfObjectToExport(part, mainComponent, out listSolidObjectToExport);
//                 if (!status)
//                 {
//                     return false;
//                 }
// 
//                 foreach (NXOpen.Assemblies.Component subComp in subComponentLst)
//                 {
//                     if (subComp != null)
//                     {
//                         List<Body> TEMP_listSolidObjectToExport = new List<Body>();
//                         Part TEMP_part = (Part)subComp.Prototype;
//                         status = CreateListOfObjectToExport(TEMP_part, subComp, out TEMP_listSolidObjectToExport);
//                         listSolidObjectToExport.AddRange(TEMP_listSolidObjectToExport);
//                     }
//                 }
// 
//                 status = PutBodiesInStlFile(fileHandle, listSolidObjectToExport, facet_tol, adj_tol);
//                 if (!status)
//                 {
//                     return false;
//                 }
// 
//                 theUfSession.Std.CloseStlFile(fileHandle);
//             }
//             catch (NXOpen.NXException ex)
//             {
//                 UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ex.Message);
//                 return false;
//             }
// 
//             return true;
//         }
// 
//         private void CreateListOfObjectToExport(Part part, NXOpen.Assemblies.Component component)
//         {
//             int type = 0;
//             int subType = 0;
//             int status = 0;
// 
//             string attr_value = "";
//             try
//             {
//                 attr_value = part.GetStringAttribute(CaxDefineParam.ATTR_CIM_TYPE);
//             }
//             catch (System.Exception ex)
//             {
//                 attr_value = "";
//             }
// 
//             bool is_design = false;
//             if (attr_value == CaxDefineParam.ATTR_CIM_TYPE_DESIGN)
//             {
//                 is_design = true;
//             }
// 
// 
//             BodyCollection bodiesColl = part.Bodies;
// 
//             Body[] BodyAry = bodiesColl.ToArray();
// 
//             if (BodyAry.Length == 0)
//             {
//                 FeatureCollection Features = part.Features;
//                 Feature[] FeatureAry = Features.ToArray();
//                 for (int i = 0; i < FeatureAry.Length;i++ )
//                 {
//                     NXObject[] EntitiesAry = FeatureAry[i].GetEntities();
//                     int num = EntitiesAry.Length;
//                 }
//             }
// 
//             for (int i = 0; i < BodyAry.Length;i++ )
//             {
//                 if (is_design)
//                 {
//                     if (BodyAry[i].Layer != CaxDefineParam.CIM_BODY_LAYER_NUM)
//                     {
//                         continue;
//                     }
//                 }
// 
//                 theUfSession.Obj.AskTypeAndSubtype(BodyAry[i].Tag, out type, out subType);
//                 status = theUfSession.Obj.AskStatus(BodyAry[i].Tag);
// 
//                 if (type == UFConstants.UF_solid_type && status == UFConstants.UF_OBJ_ALIVE)
//                 {
//                     NXObject bodyObj = component.FindOccurrence(BodyAry[i]);
//                     Body bodyOcc = (Body)bodyObj;
// 
//                     listSolidObjectToExport.Add(bodyOcc);
//                 } /* if type */
//             }
// 
//             /*
//             foreach (Body body in bodiesColl)
//             {
//                 theUfSession.Obj.AskTypeAndSubtype(body.Tag, out type, out subType);
//                 status = theUfSession.Obj.AskStatus(body.Tag);
// 
//                 if (type == UFConstants.UF_solid_type && status == UFConstants.UF_OBJ_ALIVE)
//                 {
//                     NXObject bodyObj = component.FindOccurrence(body);
//                     Body bodyOcc = (Body)bodyObj;
// 
//                     listSolidObjectToExport.Add(bodyOcc);
//                 } 
//             } 
//             */
//         }
// 
//         private void PutBodiesInStlFile(IntPtr fileHandle, ArrayList listSolidObjectToExport, double facet_tol, double adj_tol)
//         {
//             Tag wcsTag = NXOpen.Tag.Null;
//             Tag[] negatedTags;
// 
//             int bodyType = 0;
//             int countOfFaces = 0;
//             int errorNumber = 0;
//             int numNegated = 0;
// 
//             UFStd.StlError[] errorsInfo;
// 
//             theUfSession.Csys.AskWcs(out wcsTag);
// 
// //             for (int i = 0; i < listSolidObjectToExport.Count; i++)
// //             {
//             foreach (Body body in listSolidObjectToExport)
//             {
// 
// 
//                 try
//                 {
//                     if (body.Tag != NXOpen.Tag.Null)
//                     {
//                         theUfSession.Modl.AskBodyType(body.Tag, out bodyType);
// 
//                         if (bodyType == UFConstants.UF_MODL_SOLID_BODY)
//                         {
//                             try
//                             {
//                                 countOfFaces = body.GetFaces().Length;
//                                 theUfSession.Std.PutSolidInStlFile(fileHandle, wcsTag, body.Tag, 0, 100, facet_tol, out errorNumber, out errorsInfo);
//                             }
//                             catch (System.Exception ex)
//                             {
//                                 continue;
//                             }
//                         }
//                         else
//                         {
//                             try
//                             {
//                                 Tag[] sheetsTags = new Tag[body.GetFaces().Length];
// 
//                                 if (body.GetFaces().Length == 1)
//                                 {
//                                     sheetsTags[0] = body.Tag;
//                                     theUfSession.Std.PutSheetsInStlFile(fileHandle, wcsTag, 1, sheetsTags, 0, 100, facet_tol, adj_tol, out numNegated, out negatedTags, out errorNumber, out errorsInfo);
//                                 }
//                                 else
//                                 {
//                                     Face[] face = body.GetFaces();
// 
//                                     for (int j = 0; j < body.GetFaces().Length; j++)
//                                     {
//                                         theUfSession.Modl.ExtractFace(face[j].Tag, 0, out sheetsTags[j]);
//                                     }
// 
//                                     theUfSession.Std.PutSheetsInStlFile(fileHandle, wcsTag, body.GetFaces().Length, sheetsTags, 0, 100, facet_tol, adj_tol, out numNegated, out negatedTags, out errorNumber, out errorsInfo);
//                                 }
//                             }
//                             catch (System.Exception ex)
//                             {
//                                 continue;
//                             }
// 
// 
//                         
//                             /* if body.GetFaces */
//                         } /* if bodyType */
//                     } /* if body.Tag */
//                 }
//                 catch (System.Exception ex)
//                 {
//                     continue;
//                 }
// 
//             } /* foreach */
// //            } /* for i= 0 */
//         }

        public static bool ReadFileAry(string file_path,ArrayList fileAry)
        {
            fileAry.Clear();

            if (!System.IO.File.Exists(file_path))
            {
                return false;
            }

            System.IO.StreamReader file = new System.IO.StreamReader(file_path);

            string line = "";
            while ((line = file.ReadLine()) != null)
            {
                fileAry.Add(line);
            }
            file.Close();

            return true;
        }

        public bool ExportOprationToolPathPhoto(string photo_folder)
        {
            Part dispPart = theSession.Parts.Display;
            ListingWindow lw = theSession.ListingWindow;

            OperationCollection Operations = dispPart.CAMSetup.CAMOperationCollection;

            Part workPart = theSession.Parts.Work;

            /*
            dispPart.Views.WorkView.Fit();
            if (labelView.Text == "定義完成")
            {
                dispPart.ModelingViews.WorkView.Orient(VIEW_MATRIX);
                dispPart.ModelingViews.WorkView.SetScale(VIEW_SCALE);
            }
            */

            //workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Trimetric, NXOpen.View.ScaleAdjustment.Fit);
            //Point3d scaleAboutPoint1 = new Point3d(0.0, 0.0, 0.0);
            //Point3d viewCenter1 = new Point3d(0.0, 0.0, 0.0);
            //workPart.ModelingViews.WorkView.ZoomAboutPoint(0.8, scaleAboutPoint1, viewCenter1);

            Operation[] OperationAry = Operations.ToArray();

            for (int i = 0; i < OperationAry.Length; i++)
            {

                //將路徑顯示設定為不顯示刀具
                //        OperationBuilder operBuilder = OperationBuilderFactory(oper);
                OperationBuilder operBuilder = null;
                DisplayTool.ToolDisplayTypes oldToolDispType = DisplayTool.ToolDisplayTypes.None;
                if (operBuilder != null)
                {
                    oldToolDispType = operBuilder.PathDisplayOptions.ToolDisplay.ToolDisplayType;
                    operBuilder.PathDisplayOptions.ToolDisplay.ToolDisplayType = DisplayTool.ToolDisplayTypes.None;
                    operBuilder.PathDisplayOptions.PathDisplayColors.Common = workPart.Colors.Find(211); //blue
                    operBuilder.PathDisplayOptions.PathDisplayColors.SetAllCommonColor();
                    operBuilder.PathDisplayOptions.PathDisplay.PathDisplayType = DisplayPath.PathDisplayTypes.Silhouette;
                    operBuilder.Commit();
                    operBuilder.Destroy();
                }
                bool showWidth = workPart.Preferences.LineVisualization.ShowWidths;
                workPart.Preferences.LineVisualization.ShowWidths = true;


                //replay tool path
                CAMObject[] objects1 = new CAMObject[1] { OperationAry[i] };
                workPart.CAMSetup.ReplayToolPath(objects1);

                //string temp_folder = Environment.GetFolderPath(Environment.SpecialFolder.Templates);

                //擷取jpg
                UFDisp.ImageFormat format = UFDisp.ImageFormat.Jpeg;
                string ext = "jpg";
                //    string fileName = string.Format(@"{0}\{1}_{2}.{3}", "C:\\Users\\yif\\AppData\\Local\\Temp", theProgram, OperationAry[i].Name, ext);
                string fileName = string.Format(@"{0}\{1}.{2}", photo_folder, OperationAry[i].Name, ext);
                theUfSession.Disp.CreateImage(fileName, format, UFDisp.BackgroundColor.White);

                workPart.Preferences.LineVisualization.ShowWidths = showWidth;
                workPart.Views.Refresh();
                //    return fileName;
            }

            return true;
        }

        private void eTbale_Load(object sender, EventArgs e)
        {
//             StyleController styleControl = new StyleController();
//             styleControl.SetStyleManager(styleManager1);
//             styleControl.SetAllStyle(this);
// 
//             labelView.ForeColor = System.Drawing.Color.Red;
//             labelXFixture.ForeColor = System.Drawing.Color.Blue;
//             labelXWorkSection.ForeColor = System.Drawing.Color.Blue;

            string img_name = "no";
            for (int i = 0; i < ListToolLengehAry.Count; i++)
            {
                if (ListToolLengehAry[i].isOK)
                {
                    if (ListToolLengehAry[i].isOverToolLength)
                    {
                        img_name = "exclamation";
                    }
                    else
                    {
                        img_name = "yes";
                    }
                }
                else
                {
                    img_name = "no";
                }
                
                GridRow row = new GridRow(
                    (i + 1).ToString(),
                    img_name,
                    ListToolLengehAry[i].tool_name,
                    ListToolLengehAry[i].tool_ext_length,
                    ListToolLengehAry[i].oper_name,
                    ListToolLengehAry[i].part_offset.ToString("f3"),
                    ListToolLengehAry[i].cutting_length.ToString("n0"),
                    ListToolLengehAry[i].cutting_length_max.ToString("n0")
                    );

                superGridControlOper.PrimaryGrid.Rows.Add(row);
            }
            // 201505226 CMM全部不要
            checkBoxIS_CMM.Enabled = false;
            checkBoxIS_CMM.Visible = false;
            if (!hasCMM)
            {
            }
        }

        public void buttonXOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListToolLengehAry.Count; i++)
            {
                if (!ListToolLengehAry[i].isOK || ListToolLengehAry[i].isOverToolLength == true)
                {
                    //20141029 更改為切削路徑過長不能輸出工單
                    CaxMsg.ShowMsg("切削路徑過長.");
                    return;

                    /*
                    eTaskDialogResult seTaskDialogResult;
                    seTaskDialogResult = CaxMsg.ShowMsgYesNo("切削路徑過長，是否要輸出工單?");
                    if (seTaskDialogResult == eTaskDialogResult.Yes)
                    {
                        break;
                    }
                    else
                    {
                        return;
                    }
                    */
                }
            }

            sCaxLoadingDlg = new CaxLoadingDlg();
            sCaxLoadingDlg.Run();
            sCaxLoadingDlg.SetLoadingText("讀取資料...");

            bool status;

            if (sMesDatData.PART_TYPE_ID == "5")
            {
                //輸出電極工單
                status = ExportElectrodeTable();
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "工單輸出錯誤...");
                    return;
                }

                
            }
            else
            {
                //電極以外的零件(工件)
                status = ExportWorkPieceTable();
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "工單輸出錯誤...");
                    return;
                }
            }

            
            sCaxLoadingDlg.Stop();
            UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "執行成功...");

            try
            {
                this.Close();
            }
            catch (System.Exception ex)
            {

            }

        }

        private void buttonXCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonXViewSetting_Click(object sender, EventArgs e)
        {
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;

            VIEW_MATRIX = workPart.ModelingViews.WorkView.Matrix;
            VIEW_SCALE = workPart.ModelingViews.WorkView.Scale;

            //workPart.ModelingViews.WorkView.Orient(viewMatrix);
            //workPart.ModelingViews.WorkView.SetScale(scale);

            //取得輸出字串
            string viewMatrixStr;
            string scaleStr;
            viewMatrixStr = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", VIEW_MATRIX.Xx, VIEW_MATRIX.Xy, VIEW_MATRIX.Xz, VIEW_MATRIX.Yx, VIEW_MATRIX.Yy, VIEW_MATRIX.Yz, VIEW_MATRIX.Zx, VIEW_MATRIX.Zy, VIEW_MATRIX.Zz);
            scaleStr = string.Format("{0}", VIEW_SCALE);
            VIEW_DADA = string.Format("{0}_{1}", viewMatrixStr, scaleStr);

            //寫入參數
            // PartYif.CreateStringAttr(displayPart, ATTR_CATEGORY, "TRANS_VIEW", VIEW_DADA);

            //存檔
            //             PartSaveStatus partSaveStatus1;
            //             partSaveStatus1 = displayPart.Save(NXOpen.BasePart.SaveComponents.True, NXOpen.BasePart.CloseAfterSave.False);

            labelView.Text = "定義完成";
            labelView.ForeColor = System.Drawing.Color.Green;
        }

        public static bool ExportJTAsASingleFile(string OutputJtFile, string ConfigFile)
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: File->Export->JT...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                JtCreator jtCreator1;
                jtCreator1 = theSession.PvtransManager.CreateJtCreator();

                jtCreator1.IncludePmi = NXOpen.JtCreator.PmiOption.PartAndAsm;

                jtCreator1.ConfigFile = ConfigFile;

                jtCreator1.JtfileStructure = NXOpen.JtCreator.FileStructure.Monolithic;

                jtCreator1.AutolowLod = true;

                jtCreator1.PreciseGeom = true;

                theSession.SetUndoMarkName(markId1, "Export JT Dialog");

                ListCreator listCreator1;
                listCreator1 = jtCreator1.NewLevel();

                listCreator1.Chordal = 0.001;

                listCreator1.Angular = 20.0;

                listCreator1.TessOption = NXOpen.ListCreator.TessellationOption.Defined;

                jtCreator1.LodList.Append(listCreator1);

                ListCreator listCreator2;
                listCreator2 = jtCreator1.NewLevel();

                listCreator2.Chordal = 0.001;

                listCreator2.Angular = 20.0;

                listCreator2.TessOption = NXOpen.ListCreator.TessellationOption.Defined;

                jtCreator1.LodList.Append(listCreator2);

                ListCreator listCreator3;
                listCreator3 = jtCreator1.NewLevel();

                listCreator3.Chordal = 0.001;

                listCreator3.Angular = 20.0;

                listCreator3.TessOption = NXOpen.ListCreator.TessellationOption.Defined;

                jtCreator1.LodList.Append(listCreator3);

                listCreator2.Chordal = 0.0035;

                listCreator2.Angular = 0.0;

                listCreator2.Simplify = 0.4;

                listCreator2.AdvCompression = 0.5;

                listCreator3.Chordal = 0.01;

                listCreator3.Angular = 0.0;

                listCreator3.Simplify = 0.1;

                listCreator3.AdvCompression = 1.0;

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Export JT");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Export JT");

                jtCreator1.OutputJtFile = OutputJtFile;

                NXObject nXObject1;
                nXObject1 = jtCreator1.Commit();

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "Export JT");

                jtCreator1.Destroy();

            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool ExportWorkPieceTable()
        {
            try
            {
                bool status = false;
                Part displayPart = theSession.Parts.Display;

                string exportETableStr = "";
                string bufStr = "";
                string nameStr = "";
                string valueStr = "";

                exportETableStr += "{";
                exportETableStr += "\n\t\"work\":[";
                sCaxLoadingDlg.SetLoadingText("輸出裝夾圖...");

                //建立SHOP_DOC目錄
                string shopdocFolder = "";
                shopdocFolder = string.Format(@"{0}\{1}", ROOT_PATH, "SHOP_DOC");
                CaxFile.CreateWinFolder(shopdocFolder);

                //建立SHOP_DOC_TEMP目錄
                string shopdocTempFolder = "";
                shopdocTempFolder = string.Format(@"{0}\{1}", ROOT_PATH, "SHOP_DOC_TEMP");
                CaxFile.CreateWinFolder(shopdocTempFolder);

                #region 輸出裝夾圖，判斷公司名稱為DEPO: 新增加工前後檢測圖、放電基準孔圖(若沒有則填空白"")

                //取得裝夾圖CGM輸出路徑
                string cgmPath = string.Format(@"{0}\{1}.cgm", shopdocTempFolder, section_face);
                // 20151102 出圖前顯示所有Curve, Sketch, Drawing Objects
                int numberShown = theSession.DisplayManager.ShowByType(NXOpen.DisplayManager.ShowHideType.Sketches, NXOpen.DisplayManager.ShowHideScope.AnyInAssembly);
                numberShown = theSession.DisplayManager.ShowByType(NXOpen.DisplayManager.ShowHideType.Curves, NXOpen.DisplayManager.ShowHideScope.AnyInAssembly);
                numberShown = theSession.DisplayManager.ShowByType(NXOpen.DisplayManager.ShowHideType.DrawingObjects, NXOpen.DisplayManager.ShowHideScope.AnyInAssembly);
                CaxPart.Update();

                // 20150520 Stewart
                // 出裝夾圖前先隱藏所有零件，只顯示design零件
                List<CaxAsm.CompPart> AsmCompAry = new List<CaxAsm.CompPart>();
                CaxAsm.GetAsmCompStruct(out AsmCompAry, out sCimAsmCompPart);
                foreach (CaxAsm.CompPart compPart in AsmCompAry)
                {
                    CaxAsm.ComponentHide(compPart.componentOcc);
                }
                CaxAsm.ComponentShow(sCimAsmCompPart.design.comp);

                //輸出裝夾圖CGM
                status = CaxExport.ExportCGM(section_face, cgmPath);
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    CaxLog.ShowListingWindow("裝夾圖輸出錯誤...CGM");
                    return false;
                }
                //取得裝夾圖JPG輸出路徑
                string jpgPath = Path.ChangeExtension(cgmPath, "jpg");
                status = CaxExport.ExportCGM2JPG(section_face, cgmPath, jpgPath);
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    CaxLog.ShowListingWindow("裝夾圖輸出錯誤...JPG");
                    return false;
                }

                //裝夾圖名稱
                nameStr = "FIXTURE_PNG";
                valueStr = string.Format(@"{0}.jpg", section_face);
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //裝夾圖路徑
                nameStr = "FIXTURE_PATH";
                valueStr = string.Format(@"{0}/{1}.jpg", "SHOP_DOC", section_face);
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                 CaxAsm.ComponentShow(sCimAsmCompPart.fixture.comp);

                if (companyName.ToUpper() == "DEPO")
                {
                    // 20150525 Stewart 新增三張圖  同裝夾圖處理方式
                    // 1. 輸出cgm, 2. 輸出jpg, 3. 輸出圖片名稱, 4. 輸出圖片路徑
                    // 新增 匯出 放電基準孔圖檔名稱(BASEHOLE_PNG, BASEHOLE_PATH)
                    if (baseHoleName != "")
                    {
                        //輸出放電基準孔CGM
                        string cgmPath_BaseHole = string.Format(@"{0}\{1}.cgm", shopdocFolder, baseHoleName);
                        status = CaxExport.ExportCGM(baseHoleName, cgmPath_BaseHole);
                        if (!status)
                        {
                            sCaxLoadingDlg.Stop();
                            CaxLog.ShowListingWindow("放電基準孔輸出錯誤...CGM");
                            return false;
                        }
                        //輸出放電基準孔JPG
                        string jpgPath_BaseHole = Path.ChangeExtension(cgmPath_BaseHole, "jpg");
                        status = CaxExport.ExportCGM2JPG(baseHoleName, cgmPath_BaseHole, jpgPath_BaseHole);
                        if (!status)
                        {
                            sCaxLoadingDlg.Stop();
                            CaxLog.ShowListingWindow("放電基準孔輸出錯誤...JPG");
                            return false;
                        }
                        //匯出放電基準孔名稱
                        nameStr = "BASEHOLE_PNG";
                        valueStr = string.Format(@"{0}.jpg", baseHoleName);
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //匯出放電基準孔路徑
                        nameStr = "BASEHOLE_PATH";
                        valueStr = string.Format(@"{0}/{1}.jpg", "SHOP_DOC", baseHoleName);
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }
                    else
                    {
                        // 沒出放電基準孔，填空白
                        nameStr = "BASEHOLE_PNG";
                        valueStr = "";
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "BASEHOLE_PATH";
                        valueStr = "";
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }
                    // 新增 匯出 加工前檢測圖檔名稱(BEFORECNC_PNG, BEFORECNC_PATH)
                    if (beforeCNCName != "")
                    {
                        //輸出加工前檢測圖CGM
                        string cgmPath_BeforeCNC = string.Format(@"{0}\{1}.cgm", shopdocFolder, beforeCNCName);
                        status = CaxExport.ExportCGM(beforeCNCName, cgmPath_BeforeCNC);
                        if (!status)
                        {
                            sCaxLoadingDlg.Stop();
                            CaxLog.ShowListingWindow("加工前檢測圖輸出錯誤...CGM");
                            return false;
                        }
                        //輸出加工前檢測圖JPG
                        string jpgPath_BeforeCNC = Path.ChangeExtension(cgmPath_BeforeCNC, "jpg");
                        status = CaxExport.ExportCGM2JPG(beforeCNCName, cgmPath_BeforeCNC, jpgPath_BeforeCNC);
                        if (!status)
                        {
                            sCaxLoadingDlg.Stop();
                            CaxLog.ShowListingWindow("加工前檢測圖輸出錯誤...JPG");
                            return false;
                        }
                        //匯出加工前檢測圖名稱
                        nameStr = "BEFORECNC_PNG";
                        valueStr = string.Format(@"{0}.jpg", beforeCNCName);
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //匯出加工前檢測圖路徑
                        nameStr = "BEFORECNC_PATH";
                        valueStr = string.Format(@"{0}/{1}.jpg", "SHOP_DOC", beforeCNCName);
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    }
                    else
                    {
                        // 沒出加工前檢測圖，填空白
                        nameStr = "BEFORECNC_PNG";
                        valueStr = "";
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "BEFORECNC_PATH";
                        valueStr = "";
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }
                    // 新增 匯出 加工後檢測圖檔名稱(AFTERCNC_PNG, AFTERCNC_PATH)
                    if (afterCNCName != "")
                    {
                        //輸出加工後檢測圖CGM
                        string cgmPath_AfterCNC = string.Format(@"{0}\{1}.cgm", shopdocFolder, afterCNCName);
                        status = CaxExport.ExportCGM(afterCNCName, cgmPath_AfterCNC);
                        if (!status)
                        {
                            sCaxLoadingDlg.Stop();
                            CaxLog.ShowListingWindow("放電基準孔輸出錯誤...CGM");
                            return false;
                        }
                        //輸出加工後檢測圖JPG
                        string jpgPath_AfterCNC = Path.ChangeExtension(cgmPath_AfterCNC, "jpg");
                        status = CaxExport.ExportCGM2JPG(afterCNCName, cgmPath_AfterCNC, jpgPath_AfterCNC);
                        if (!status)
                        {
                            sCaxLoadingDlg.Stop();
                            CaxLog.ShowListingWindow("放電基準孔輸出錯誤...JPG");
                            return false;
                        }
                        //匯出加工後檢測圖名稱
                        nameStr = "AFTERCNC_PNG";
                        valueStr = string.Format(@"{0}.jpg", afterCNCName);
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //匯出加工後檢測圖路徑
                        nameStr = "AFTERCNC_PATH";
                        valueStr = string.Format(@"{0}/{1}.jpg", "SHOP_DOC", afterCNCName);
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }
                    else
                    {
                        // 沒出加工後檢測圖，填空白
                        nameStr = "AFTERCNC_PNG";
                        valueStr = "";
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "AFTERCNC_PATH";
                        valueStr = "";
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }

                }
                else // if (companyName.ToUpper() == "COXON")
                {
                    // 不是帝寶，全部填空白
                    nameStr = "BASEHOLE_PNG";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    nameStr = "BASEHOLE_PATH";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    nameStr = "BEFORECNC_PNG";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    nameStr = "BEFORECNC_PATH";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    nameStr = "AFTERCNC_PNG";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    nameStr = "AFTERCNC_PATH";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                }
                #endregion

                //匯出上傳KEY (模具號碼)
                nameStr = "MOLD_NO";
                valueStr = sMesDatData.MOLD_NO;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出上傳KEY (設計版本號碼)
                nameStr = "DES_VER_NO";
                valueStr = sMesDatData.DES_VER_NO;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出上傳KEY (工令單號)
                nameStr = "WORK_NO";
                valueStr = sMesDatData.WORK_NO;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出上傳KEY (零件號碼)
                nameStr = "PART_NO";
                valueStr = sMesDatData.PART_NO;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出上傳KEY (製程版本編號)
                nameStr = "MFC_NO";
                valueStr = sMesDatData.MFC_NO;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出上傳KEY (製程項目編號)
                nameStr = "MFC_TASK_NO";
                valueStr = sMesDatData.MFC_TASK_NO;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出上傳KEY (CAM工作流水號)
                nameStr = "TASK_NO";
                valueStr = sMesDatData.TASK_NO;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出上傳KEY (機台型號工廠內代號)
                nameStr = "MAC_MODEL_NO";
                valueStr = sMesDatData.MAC_MODEL_NO;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出上傳KEY (CNC CAM POST 工作流水號)
                nameStr = "TASK_SRNO";
                valueStr = sMesDatData.TASK_SRNO;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出任務名稱(TASK_NAME)
                nameStr = "TASK_NAME";
                valueStr = string.Format(@"{0}_{1}_{2}_{3}", sMesDatData.MOLD_NO, sMesDatData.PART_NO, sMesDatData.DES_VER_NO, sMesDatData.MAC_MODEL_NO);
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出 工段_粗精洗 名稱(SECTION_ID)
                nameStr = "SECTION_ID";
                //valueStr = string.Format(@"{0}_{1}_{2}", sMesDatData.section_id, sMesDatData.mill_type_id, sMesDatData.face_type_id);
                valueStr = string.Format(@"{0}", sMesDatData.SECTION_ID);
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出 加工面 名稱(FACE_TYPE_ID)
                nameStr = "FACE_TYPE_ID";
                valueStr = string.Format(@"{0}", sMesDatData.FACE_TYPE_ID);
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                // 20150525 新增key值: 外包殘料檔案名稱, 邊線檔案名稱, 邊面檔案名稱
                if (companyName.ToUpper() == "DEPO")
                {
                    // 2個檔案名稱都從mes繼承過來，等CAX更新以後才能從sMesDatData點出來~~
                    // 20150624移到下面輸出stl時一起出key
//                     // 新增 匯出 外包殘料檔案名稱(OUT_STL_NAME)
//                     nameStr = "OUT_STL_NAME";
//                     valueStr = sMesDatData.OUT_STL_NAME;
//                     exportETableStr += "\n\t\t";
//                     bufStr = exportETableStr;
//                     exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    // 新增 匯出 邊線邊面檔案名稱(SIDELINE_NAME)
                    nameStr = "SIDEPART_NAME";
                    valueStr = sMesDatData.SIDEPART_NAME;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                }
                else // if(COXON)
                {
                    // 沒有這2個檔案，填空白
//                     nameStr = "OUT_STL_NAME";
//                     valueStr = "";
//                     exportETableStr += "\n\t\t";
//                     bufStr = exportETableStr;
//                     exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    nameStr = "SIDEPART_NAME";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                }

                // 20150526新增 匯出 機外or機內校正(CALIBRATOR) "1":機外   "2":機內
                nameStr = "CALIBRATOR";
                valueStr = "";
                string measureType = "";
                try
                {
                    measureType = sCimAsmCompPart.fixture.comp.GetStringAttribute("CIM_FIXTURE_MEASURE_FINAL");
                }
                catch (System.Exception ex)
                {
                }
                if (measureType == "OUT")
                {
                    valueStr = "1";
                }
                if (measureType == "IN")
                {
                    valueStr = "2";
                }
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                // 20150526新增 匯出 校正方法(CALIBRATOR_METHOD) "1":雙邊求中  "2":單邊求中第三象限  "3":單邊求中第四象限  "4":求圓心(模板)
                // 目前先空著!!
                nameStr = "CALIBRATOR_METHOD";
                valueStr = "";
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                double[] minWcs;
                double[] maxWcs;
                //基準角到X中心
                if (sBaseFaces.hasA)
                {
                    CaxPart.AskBoundingBoxExactByWCS(sBaseFaces.baseFaceA.face.Tag, out minWcs, out maxWcs);
                    nameStr = "CONNER_X";
                    if (sBaseFaces.baseFaceA.sFaceData.dir[0] > 0.9)
                    {
                        valueStr = maxWcs[0].ToString("f4");
                    }
                    else
                    {
                        valueStr = minWcs[0].ToString("f4");
                    }
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                }
                else
                {
                    nameStr = "CONNER_X";
                    valueStr = maxDesignBodyWcs[0].ToString("f4");
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                }
                //基準角到Y中心
                if (sBaseFaces.hasB)
                {
                    CaxPart.AskBoundingBoxExactByWCS(sBaseFaces.baseFaceB.face.Tag, out minWcs, out maxWcs);
                    nameStr = "CONNER_Y";
                    if (sBaseFaces.baseFaceB.sFaceData.dir[1] > 0.9)
                    {
                        valueStr = maxWcs[1].ToString("f4");
                    }
                    else
                    {
                        valueStr = minWcs[1].ToString("f4");
                    }
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                }
                else
                {
                    nameStr = "CONNER_Y";
                    valueStr = maxDesignBodyWcs[1].ToString("f4");
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                }
                //基準角到Z中心
                if (sBaseFaces.hasC)
                {
                    CaxPart.AskBoundingBoxExactByWCS(sBaseFaces.baseFaceC.face.Tag, out minWcs, out maxWcs);
                    nameStr = "CONNER_Z";
                    if (sBaseFaces.baseFaceC.sFaceData.dir[2] > 0.9)
                    {
                        valueStr = maxWcs[2].ToString("f4");
                    }
                    else
                    {
                        valueStr = minWcs[2].ToString("f4");
                    }
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                }
                else
                {
                    nameStr = "CONNER_Z";
                    valueStr = maxDesignBodyWcs[2].ToString("f4");
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                }
//                 for (int i = 0; i < baseCornerAry.Count; i++)
//                 {
//                     CaxPart.AskBoundingBoxExactByWCS(baseCornerAry[i].face.Tag, out minWcs, out maxWcs);
// 
//                     if (baseCornerAry[i].sFaceData.dir[0] > 0.9999)
//                     {
//                         //基準角到X中心
//                         nameStr = "CONNER_X";
//                         valueStr = maxWcs[0].ToString("f4");
//                         exportETableStr += "\n\t\t";
//                         bufStr = exportETableStr;
//                         exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                     }
//                     else if (baseCornerAry[i].sFaceData.dir[0] < -0.9999)
//                     {
//                         //基準角到X中心
//                         nameStr = "CONNER_X";
//                         valueStr = minWcs[0].ToString("f4");
//                         exportETableStr += "\n\t\t";
//                         bufStr = exportETableStr;
//                         exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                     }
//                     else if (baseCornerAry[i].sFaceData.dir[1] > 0.9999)
//                     {
//                         //基準角到Y中心
//                         nameStr = "CONNER_Y";
//                         valueStr = maxWcs[1].ToString("f4");
//                         exportETableStr += "\n\t\t";
//                         bufStr = exportETableStr;
//                         exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                     }
//                     else if (baseCornerAry[i].sFaceData.dir[1] < -0.9999)
//                     {
//                         //基準角到Y中心
//                         nameStr = "CONNER_Y";
//                         valueStr = minWcs[1].ToString("f4");
//                         exportETableStr += "\n\t\t";
//                         bufStr = exportETableStr;
//                         exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                     }
//                     else if (baseCornerAry[i].sFaceData.dir[2] > 0.9999)
//                     {
//                         //基準角到Z中心
//                         nameStr = "CONNER_Z";
//                         valueStr = maxWcs[2].ToString("f4");
//                         exportETableStr += "\n\t\t";
//                         bufStr = exportETableStr;
//                         exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                     }
//                     else if (baseCornerAry[i].sFaceData.dir[2] < -0.9999)
//                     {
//                         //基準角到Z中心
//                         nameStr = "CONNER_Z";
//                         valueStr = minWcs[2].ToString("f4");
//                         exportETableStr += "\n\t\t";
//                         bufStr = exportETableStr;
//                         exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                     }
//                 }

                //匯出工件T面轉BO面的OFFSET數值(Z_OFFSET)
                nameStr = "Z_OFFSET";
                valueStr = sExportWorkTabel.Z_MOVE;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出Zbase(BOTTOM_Z)(目前給正值)
                nameStr = "BOTTOM_Z";
                if (BOTTOM_Z < 0)
                {
                    BOTTOM_Z = BOTTOM_Z * (-1);
                }
                valueStr = BOTTOM_Z.ToString("f4");
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                // 20150817 改取裝夾圖屬性值為基準距離
                //X+
                nameStr = "MAX_X_POSITION";
                valueStr = sBaseDist.MAX_X_POSITION.ToString("f3");
//                 valueStr = maxDesignBodyWcs[0].ToString("f4");
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //Y+
                nameStr = "MAX_Y_POSITION";
                valueStr = sBaseDist.MAX_Y_POSITION.ToString("f3");
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //Z+
                nameStr = "MAX_Z_POSITION";
                valueStr = sBaseDist.MAX_Z_POSITION.ToString("f3");
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //X-
                nameStr = "MIN_X_POSITION";
                valueStr = sBaseDist.MIN_X_POSITION.ToString("f3");
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //Y-
                nameStr = "MIN_Y_POSITION";
                valueStr = sBaseDist.MIN_Y_POSITION.ToString("f3");
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //Z-
                nameStr = "MIN_Z_POSITION";
                valueStr = sBaseDist.MIN_Z_POSITION.ToString("f3");
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                // 20151013 新增輸出判斷是否重新裝夾/校正之參數
                nameStr = "AXIS_ANGLE";
                valueStr = clampCalibParam.AXIS_ANGLE;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                nameStr = "BASE_CORNER_QUADRANT";
                valueStr = clampCalibParam.BASE_CORNER_QUADRANT;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                nameStr = "BASE_CORNER_VECTOR";
                valueStr = clampCalibParam.BASE_CORNER_VECTOR;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                nameStr = "Z_AXIS_BOTTOM_HEIGHT";
                valueStr = clampCalibParam.Z_AXIS_BOTTOM_HEIGHT;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);


                // 20150526 新增多治具處理 (可同時處理單一治具或多治具，不須判斷config資訊)
                // 找到所有治具
//                 List<CaxAsm.CompPart> fixtureLst = new List<CaxAsm.CompPart>();
//                 for (int i = 0; i < AsmCompAry.Count; i++)
//                 {
//                     try
//                     {
//                         string attr = AsmCompAry[i].componentOcc.GetStringAttribute(CaxDefineParam.ATTR_CIM_TYPE);
//                         if (attr == "FIXTURE")
//                         {
//                             fixtureLst.Add(AsmCompAry[i]);
//                         }
//                     }
//                     catch (System.Exception ex)
//                     {
//                         continue;
//                     }
//                 }

                #region 輸出工件+治具JT
//                 // 20150529 Stewart 搜所有次主件
//                 List<NXOpen.Assemblies.Component> subDesignCompLst = new List<NXOpen.Assemblies.Component>();
//                 if (sMesDatData.PROCESS_DESIGN == "ASM" || sMesDatData.PROCESS_DESIGN == "ASM_MODIFIED")
//                 {
//                     string attr = "";
//                     foreach (CaxAsm.CompPart compPart in AsmCompAry)
//                     {
//                         try
//                         {
//                             attr = compPart.componentOcc.GetStringAttribute(CaxDefineParam.ATTR_CIM_TYPE);
//                             if (attr == CaxDefineParam.ATTR_CIM_TYPE_SUB_DESIGN)
//                             {
//                                 subDesignCompLst.Add(compPart.componentOcc);
//                             }
//                         }
//                         catch (System.Exception ex)
//                         {
//                             continue;
//                         }
//                     }
//                 }

                // 顯示所有治具
                for (int i = 0; i < fixtureLst.Count; i++)
                {
                    CaxAsm.ComponentShow(fixtureLst[i].componentOcc);
                }
                CaxAsm.ComponentShow(sCimAsmCompPart.fixture.comp);

                // 顯示所有次主件
                for (int j = 0; j < subDesignCompLst.Count; j++)
                {
                    CaxAsm.ComponentShow(subDesignCompLst[j]);
                }

                //轉出JT檔(工件+治具)  20150529 + 次主件
                string displayPartName = Path.GetFileNameWithoutExtension(displayPart.FullPath);
                string displayPartNewName = displayPartName.Replace(".", "_");
                displayPartName = displayPartNewName;
                displayPartNewName = displayPartName.Replace("-", "_");

                string ExportJTtessUGconfigPath = string.Format(@"{0}\jt\tessUG.config", CaxCNC.GetCimLocalProgramsDir());
                if (!System.IO.File.Exists(ExportJTtessUGconfigPath))
                {
                    sCaxLoadingDlg.Stop();
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ExportJTtessUGconfigPath + " 檔案遺失!");
                    return false;
                }

                string jtPath = string.Format(@"{0}\{1}.jt", Path.GetDirectoryName(displayPart.FullPath), displayPartNewName);
                status = ExportJTAsASingleFile(jtPath, ExportJTtessUGconfigPath);
                if (!status)
                {
                    sCaxLoadingDlg.Stop();
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "JT 輸出失敗!");
                    return false;
                }

                //匯出JT路徑
                nameStr = "TOP_ASM_JT_LINK";
                valueStr = Path.GetFileName(jtPath);
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                // 隱藏所有治具
                for (int i = 0; i < fixtureLst.Count; i++)
                {
                    CaxAsm.ComponentHide(fixtureLst[i].componentOcc);
                }
                CaxAsm.ComponentHide(sCimAsmCompPart.fixture.comp);
//                 CaxAsm.ComponentShow(sCimAsmCompPart.fixture.comp);
// 
//                 //轉出JT檔(工件+治具)
//                 string displayPartName = Path.GetFileNameWithoutExtension(displayPart.FullPath);
//                 string displayPartNewName = displayPartName.Replace(".", "_");
//                 displayPartName = displayPartNewName;
//                 displayPartNewName = displayPartName.Replace("-", "_");
// 
//                 string ExportJTtessUGconfigPath = string.Format(@"{0}\jt\tessUG.config", CaxCNC.GetCimLocalProgramsDir());
//                 if (!System.IO.File.Exists(ExportJTtessUGconfigPath))
//                 {
//                     //tempsCaxLoadingDlg.Stop();
//                     UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ExportJTtessUGconfigPath + " 檔案遺失!");
//                     return false;
//                 }
// 
//                 string jtPath = string.Format(@"{0}\{1}.jt", Path.GetDirectoryName(displayPart.FullPath), displayPartNewName);
//                 status = ExportJTAsASingleFile(jtPath, ExportJTtessUGconfigPath);
//                 if (!status)
//                 {
//                     //tempsCaxLoadingDlg.Stop();
//                     UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "JT 輸出失敗!");
//                     return false;
//                 }
// 
//                 //匯出JT路徑
//                 nameStr = "TOP_ASM_JT_LINK";
//                 valueStr = Path.GetFileName(jtPath);
//                 exportETableStr += "\n\t\t";
//                 bufStr = exportETableStr;
//                 exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
// 
//                 CaxAsm.ComponentHide(sCimAsmCompPart.fixture.comp);

                #endregion


                //匯出設計時間(DESIGN_TIME)
                nameStr = "DESIGN_TIME";
                valueStr = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //匯出使用者名稱(USER_ID)
                nameStr = "USER_ID";
                valueStr = sMesDatData.USER_ID;//System.Environment.UserName;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //旋轉視角
                if (labelView.Text == "定義完成")
                {
                    theSession.Parts.Display.ModelingViews.WorkView.Orient(VIEW_MATRIX);
                    theSession.Parts.Display.ModelingViews.WorkView.SetScale(VIEW_SCALE);
                    theSession.Parts.Display.Views.WorkView.Fit();
                }
                else
                {
                    //未定義，使用目前視角輸出
                    Part workPart = theSession.Parts.Work;
                    VIEW_MATRIX = workPart.ModelingViews.WorkView.Matrix;
                    VIEW_SCALE = workPart.ModelingViews.WorkView.Scale;

                    //取得輸出字串
                    string viewMatrixStr;
                    string scaleStr;
                    viewMatrixStr = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", VIEW_MATRIX.Xx, VIEW_MATRIX.Xy, VIEW_MATRIX.Xz, VIEW_MATRIX.Yx, VIEW_MATRIX.Yy, VIEW_MATRIX.Yz, VIEW_MATRIX.Zx, VIEW_MATRIX.Zy, VIEW_MATRIX.Zz);
                    scaleStr = string.Format("{0}", VIEW_SCALE);
                    VIEW_DADA = string.Format("{0}_{1}", viewMatrixStr, scaleStr);
                    theSession.Parts.Display.Views.WorkView.Fit();
                }

                CaxAsm.ComponentHide(sCimAsmCompPart.fixture.comp);
                theSession.Parts.Display.Views.WorkView.Fit();

                sCaxLoadingDlg.SetLoadingText("輸出組立圖片檔...");

                //匯出組立圖片檔
                string asmPhotoPath;
                asmPhotoPath = Path.ChangeExtension(asmName, "jpg"); //變更副檔名: "D:\Downloads\test.txt"
                theUfSession.Disp.CreateImage(asmPhotoPath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);
                nameStr = "TOP_ASM_GRAPH_P";
                valueStr = Path.GetFileName(asmPhotoPath);
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                //MessageBox.Show(exportETableStr);

                
                //是否模仁
                if (sMesDatData.PART_TYPE_ID == "0")
                {
                    nameStr = "IS_CORE";
                    valueStr = "1";
                }
                else
                {
                    nameStr = "IS_CORE";
                    valueStr = "0";
                }
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                //MessageBox.Show(exportETableStr);

                #region 輸出 STL 模擬檔案
                //STL 模擬檔案
                string stlDataStr = "";
                if (checkBoxIS_STL.Checked)
                {
                    sCaxLoadingDlg.SetLoadingText("輸出STL模擬檔案...");

                    nameStr = "IS_STL";
                    valueStr = "1";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    Tag tagRootPart = theUfSession.Assem.AskRootPartOcc(displayPart.Tag);
                    if (tagRootPart == NXOpen.Tag.Null)
                    {
                        sCaxLoadingDlg.Stop();
                        return false;
                    }

                    //輸出STL - 素材
                    //sCimAsmCompPart.
                    sCaxLoadingDlg.SetLoadingText("輸出STL - 素材...");

                    string stlPath = Path.ChangeExtension(sCimAsmCompPart.blank.part.FullPath, "stl");
                    string stlName = Path.GetFileName(stlPath);

                    nameStr = "IPW_STL";
                    valueStr = stlName;
                    stlDataStr += "\n";
                    bufStr = stlDataStr;
                    stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    nameStr = "IPW_STL_PATH";
                    valueStr = stlName;
                    stlDataStr += "\n";
                    bufStr = stlDataStr;
                    stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    CaxPart.ExportBinarySTL(sCimAsmCompPart.blank.comp, FACET_TOLER, ADJ_TOLER);

                    //輸出STL - 治具...
                    sCaxLoadingDlg.SetLoadingText("輸出STL - 治具...");

                    // 20150526 處理多治具
                    if (isMultiFixture)
                    {
                        stlPath = Path.ChangeExtension(fixtureLst[0].partPrototype.FullPath, "stl");
                        stlName = Path.GetFileName(stlPath);
                        List<List<Feature>> SuppressFeatLstLst = new List<List<Feature>>();
                        for (int i = 0; i < fixtureLst.Count; i++)
                        {
                            //如果是治具零件，檢查是否有需要suppress的特徵
                            string attr_value = "";
                            Feature[] partFeats = fixtureLst[i].partPrototype.Features.ToArray();
//                             List<Feature> partFeatLst = new List<Feature>();
                            List<Feature> tempSuppressFeatAry = new List<Feature>();
                            for (int j = 0; j < partFeats.Length; j++)
                            {

                                try
                                {
                                    attr_value = "";
                                    attr_value = partFeats[j].GetStringAttribute(CaxDefineParam.ATTR_CIM_FEAT_SUPPRESS);
                                    if (attr_value == "1")
                                    {
                                        tempSuppressFeatAry.Add(partFeats[j]);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    continue;
                                }
                            }

                            //SuppressFeature
                            for (int j = 0; j < tempSuppressFeatAry.Count; j++)
                            {
                                CaxPart.SuppressFeature(fixtureLst[i].partPrototype, tempSuppressFeatAry[j]);
                            }
                            SuppressFeatLstLst.Add(tempSuppressFeatAry);
                        }


                        // 20150526 Stewart 多治具之處理
                        CaxPart.ExportBinarySTL(sCimAsmCompPart.fixture.comp, FIXTURE_FACET_TOLER, FIXTURE_ADJ_TOLER);
                        List<NXOpen.Assemblies.Component> fixtureCompLst = new List<NXOpen.Assemblies.Component>();
                        for (int k = 0; k < fixtureLst.Count; k++)
                        {
                            fixtureCompLst.Add(fixtureLst[k].componentOcc);
                        }
                        fixtureCompLst.RemoveAt(0);
                        ExportBinarySTL_multiComponent(fixtureLst[0].componentOcc, fixtureCompLst, FIXTURE_FACET_TOLER, FIXTURE_ADJ_TOLER);
                        //UnsuppressFeature
                        for (int i = 0; i < fixtureLst.Count; i++)
                        {
                            List<Feature> tempSuppressFeatAry = SuppressFeatLstLst[i];
                            for (int j = 0; j < tempSuppressFeatAry.Count; j++)
                            {
                                CaxPart.UnsuppressFeature(fixtureLst[i].partPrototype, tempSuppressFeatAry[j]);
                            }
                        }

                        nameStr = "FIXTURE_STL";
                        valueStr = stlName;
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "FIXTURE_STL_PATH";
                        valueStr = stlName;
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }
                    else
                    {
                        stlPath = Path.ChangeExtension(sCimAsmCompPart.fixture.part.FullPath, "stl");
                        stlName = Path.GetFileName(stlPath);

                        //如果是治具零件，檢查是否有需要suppress的特徵
                        string attr_value = "";
                        Feature[] partFeats = sCimAsmCompPart.fixture.part.Features.ToArray();
                        List<Feature> SuppressFeatAry = new List<Feature>();
                        for (int j = 0; j < partFeats.Length; j++)
                        {
                            try
                            {
                                attr_value = "";
                                attr_value = partFeats[j].GetStringAttribute(CaxDefineParam.ATTR_CIM_FEAT_SUPPRESS);
                                if (attr_value == "1")
                                {
                                    SuppressFeatAry.Add(partFeats[j]);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                continue;
                            }
                        }

                        //SuppressFeature
                        for (int j = 0; j < SuppressFeatAry.Count; j++)
                        {
                            CaxPart.SuppressFeature(sCimAsmCompPart.fixture.part, SuppressFeatAry[j]);
                        }

                        CaxPart.ExportBinarySTL(sCimAsmCompPart.fixture.comp, FIXTURE_FACET_TOLER, FIXTURE_ADJ_TOLER);

                        //UnsuppressFeature
                        for (int j = 0; j < SuppressFeatAry.Count; j++)
                        {
                            CaxPart.UnsuppressFeature(sCimAsmCompPart.fixture.part, SuppressFeatAry[j]);
                        }

                        nameStr = "FIXTURE_STL";
                        valueStr = stlName;
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "FIXTURE_STL_PATH";
                        valueStr = stlName;
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }

                    //輸出STL - 設計檔
                    sCaxLoadingDlg.SetLoadingText("輸出STL - 設計檔...");
                    stlPath = Path.ChangeExtension(sCimAsmCompPart.design.part.FullPath, "stl");
                    stlName = Path.GetFileName(stlPath);

                    nameStr = "WORK_STL";
                    valueStr = stlName;
                    stlDataStr += "\n";
                    bufStr = stlDataStr;
                    stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    nameStr = "WORK_STL_PATH";
                    valueStr = stlName;
                    stlDataStr += "\n";
                    bufStr = stlDataStr;
                    stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    // 20150525 Stewart 若為組立加工時，需輸出design + subdesign
                    // 判斷加工類型，若為組立加工則找到所有 CIM_TYPE 為 SUB_DESIGN 的component一起出stl
                    // 工件有電極沒有
                    if(sMesDatData.PROCESS_DESIGN == "ASM" || sMesDatData.PROCESS_DESIGN == "ASM_MODIFIED")
                    {
                        ExportBinarySTL_multiComponent(sCimAsmCompPart.design.comp, subDesignCompLst, FACET_TOLER, ADJ_TOLER);
//                         CaxPart.ExportBinarySTL(sCimAsmCompPart.design.comp, FACET_TOLER, ADJ_TOLER);
                    }
                    else
                    {
                        CaxPart.ExportBinarySTL(sCimAsmCompPart.design.comp, FACET_TOLER, ADJ_TOLER);
                    }

                    // 先不改!!
                    // 輸出STL - STOCK 20150624 暫時由手動轉檔，工單一樣把key值輸出
                    if (companyName == "DEPO" && sCimAsmCompPart.stock.comp != null)
                    {
                        sCaxLoadingDlg.SetLoadingText("輸出STL - STOCK...");
                        // 新增 匯出 外包殘料檔案名稱(OUT_STL_NAME)
                        stlPath = Path.ChangeExtension(sCimAsmCompPart.stock.part.FullPath, "stl");
                        stlName = Path.GetFileName(stlPath);
//                         CaxLog.ShowListingWindow("a");
                        nameStr = "OUT_STL_NAME";
                        valueStr = stlName;
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        export_solids_to_stl(sCimAsmCompPart.stock.comp, stlPath);
//                         CaxPart.ExportBinarySTL(sCimAsmCompPart.stock.comp, FACET_TOLER, ADJ_TOLER);
//                         CaxLog.ShowListingWindow(sCimAsmCompPart.stock.comp.Tag.ToString());

                    }
                    else
                    {
                        // 新增 匯出 外包殘料檔案名稱(OUT_STL_NAME)
                        nameStr = "OUT_STL_NAME";
                        valueStr = "";
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }




                    //寫出stl.dat
                    string stl_path = "";
                    stl_path = string.Format(@"{0}\{1}", ROOT_PATH, "stl.dat");
                    StreamWriter file_out_stlPath = new StreamWriter(stl_path, false);
                    file_out_stlPath.WriteLine("[DATA_START]");
                    string[] stlAry = stlDataStr.Split('\n');
                    for (int i = 0; i < stlAry.Length; i++)
                    {
                        if (stlAry[i].Trim() == "")
                        {
                            continue;
                        }
                        file_out_stlPath.WriteLine(stlAry[i]);
                    }
                    file_out_stlPath.WriteLine("[DATA_END]");
                    file_out_stlPath.Close();
                }
                else
                {
                    nameStr = "IS_STL";
                    valueStr = "0";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    nameStr = "IPW_STL";
                    valueStr = "";
                    stlDataStr += "\n";
                    bufStr = stlDataStr;
                    stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    nameStr = "IPW_STL_PATH";
                    valueStr = "";
                    stlDataStr += "\n";
                    bufStr = stlDataStr;
                    stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    nameStr = "WORK_STL";
                    valueStr = "";
                    stlDataStr += "\n";
                    bufStr = stlDataStr;
                    stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    nameStr = "WORK_STL_PATH";
                    valueStr = "";
                    stlDataStr += "\n";
                    bufStr = stlDataStr;
                    stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    nameStr = "FIXTURE_STL";
                    valueStr = "";
                    stlDataStr += "\n";
                    bufStr = stlDataStr;
                    stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    nameStr = "FIXTURE_STL_PATH";
                    valueStr = "";
                    stlDataStr += "\n";
                    bufStr = stlDataStr;
                    stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    // 新增 匯出 外包殘料檔案名稱(OUT_STL_NAME)
                    nameStr = "OUT_STL_NAME";
                    valueStr = "";
                    stlDataStr += "\n";
                    bufStr = stlDataStr;
                    stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);


                    //寫出stl.dat
                    string stl_path = "";
                    stl_path = string.Format(@"{0}\{1}", ROOT_PATH, "stl.dat");
                    if (!System.IO.File.Exists(stl_path))
                    {
                        StreamWriter file_out_stlPath = new StreamWriter(stl_path, false);
                        file_out_stlPath.WriteLine("[DATA_START]");
                        string[] stlAry = stlDataStr.Split('\n');
                        for (int i = 0; i < stlAry.Length; i++)
                        {
                            if (stlAry[i].Trim() == "")
                            {
                                continue;
                            }
                            file_out_stlPath.WriteLine(stlAry[i]);
                        }
                        file_out_stlPath.WriteLine("[DATA_END]");
                        file_out_stlPath.Close();
                    }
                }
                //MessageBox.Show(exportETableStr);
                #endregion

                #region 輸出Post

                //Post
                string postDataStr = "";

                //建立POST目錄
                string postFolder;
                postFolder = string.Format(@"{0}\{1}", ROOT_PATH, "POST");
                CaxFile.CreateWinFolder(postFolder);


                if (checkBoxIS_POST.Checked)
                {
                    sCaxLoadingDlg.SetLoadingText("輸出 POST ...");

                    nameStr = "IS_POST";
                    valueStr = "1";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    string postProgramNo = "";
                    string postProgramListPath = "";
                    for (int i = 0; i < ListToolLengehAry.Count; i++)
                    {
                        postProgramNo += ListToolLengehAry[i].oper_name + machine_group + ",";
                        postProgramListPath += "POST" + "/" + ListToolLengehAry[i].oper_name + machine_group + ",";
                    }
                    postProgramNo.TrimEnd(',');
                    postProgramListPath.TrimEnd(',');

                    //nameStr = "PROGRAM_NO";
                    nameStr = "PROGRAM_LIST_NAME";  //2014/01/06 kennyou 將PROGRAM_NO改為PROGRAM_LIST_NAME
                    valueStr = postProgramNo;
                    postDataStr += "";
                    bufStr = postDataStr;
                    postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    nameStr = "PROGRAM_LIST_PATH";
                    valueStr = postProgramListPath;
                    postDataStr += "\n";
                    bufStr = postDataStr;
                    postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //選取的機台廠牌
                    nameStr = "SUPPORT_MACHINE";
                    valueStr = support_machine;
                    postDataStr += "\n";
                    bufStr = postDataStr;
                    postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //選取的規格型號
                    nameStr = "MACHINE_TYPE";
                    valueStr = machine_type;
                    postDataStr += "\n";
                    bufStr = postDataStr;
                    postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //選取的控制器
                    nameStr = "CONTROLLER";
                    valueStr = controller;
                    postDataStr += "\n";
                    bufStr = postDataStr;
                    postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //輸出的機台類型格式(副檔名)
                    nameStr = "MACHINE_GROUP";
                    valueStr = machine_group;
                    postDataStr += "\n";
                    bufStr = postDataStr;
                    postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //RunPost
                    Part workPart = theSession.Parts.Work;
                    string postOutPath = "";
                    string postMsg = "";
                    NXOpen.CAM.CAMObject[] objects1 = new NXOpen.CAM.CAMObject[1];
                    for (int i = 0; i < ListToolLengehAry.Count; i++)
                    {
                        postMsg = string.Format(@"輸出 POST ({0}/{1}) - {2}...", (i + 1).ToString(), ListToolLengehAry.Count.ToString(), ListToolLengehAry[i].oper_name);
                        sCaxLoadingDlg.SetLoadingText(postMsg);

                        postOutPath = string.Format(@"{0}\{1}\{2}{3}", ROOT_PATH, "POST", ListToolLengehAry[i].oper_name, machine_group);
                        NXOpen.CAM.CAMObject camObj = (NXOpen.CAM.CAMObject)workPart.CAMSetup.CAMOperationCollection.FindObject(ListToolLengehAry[i].oper_name);
                        objects1[0] = camObj;
                        try
                        {
                            workPart.CAMSetup.PostprocessWithSetting(objects1, postFunction, postOutPath, NXOpen.CAM.CAMSetup.OutputUnits.PostDefined, NXOpen.CAM.CAMSetup.PostprocessSettingsOutputWarning.PostDefined, NXOpen.CAM.CAMSetup.PostprocessSettingsReviewTool.PostDefined);
                        }
                        catch (System.Exception ex)
                        {
                            int indexNum = i + 1;
                            CaxLog.ShowListingWindow("POST 輸出錯誤：[" + indexNum.ToString() + "] " + ListToolLengehAry[i].oper_name);
                            CaxLog.ShowListingWindow("POST Processor：" + postFunction);
                            CaxLog.ShowListingWindow("POST 輸出路徑：" + postOutPath);
                            CaxLog.ShowListingWindow(ex.Message);
                            //continue;
                            sCaxLoadingDlg.Stop();
                            return false;
                        }
                    }

                    //RunPost();

                    //寫出post.dat
                    string postPath = "";
                    postPath = string.Format(@"{0}\{1}\{2}", ROOT_PATH, "POST", "post.dat");
                    StreamWriter file_out_postPath = new StreamWriter(postPath, false);
                    file_out_postPath.WriteLine("[DATA_START]");
                    string[] postAry = postDataStr.Split('\n');
                    for (int i = 0; i < postAry.Length; i++)
                    {
                        file_out_postPath.WriteLine(postAry[i]);
                    }
                    file_out_postPath.WriteLine("[DATA_END]");
                    file_out_postPath.Close();
                }
                else
                {
                    nameStr = "IS_POST";
                    valueStr = "0";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    nameStr = "PROGRAM_NO";
                    valueStr = "";
                    postDataStr += "";
                    bufStr = postDataStr;
                    postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    nameStr = "PROGRAM_LIST_PATH";
                    valueStr = "";
                    postDataStr += "\n";
                    bufStr = postDataStr;
                    postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //選取的機台廠牌
                    nameStr = "SUPPORT_MACHINE";
                    valueStr = "";
                    postDataStr += "\n";
                    bufStr = postDataStr;
                    postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //選取的規格型號
                    nameStr = "MACHINE_TYPE";
                    valueStr = "";
                    postDataStr += "\n";
                    bufStr = postDataStr;
                    postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //選取的控制器
                    nameStr = "CONTROLLER";
                    valueStr = "";
                    postDataStr += "\n";
                    bufStr = postDataStr;
                    postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //輸出的機台類型格式(副檔名)
                    nameStr = "MACHINE_GROUP";
                    valueStr = "";
                    postDataStr += "\n";
                    bufStr = postDataStr;
                    postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //post.dat不存在才寫出post.dat(已存在代表之前已做過，不取代)
                    string postPath = "";
                    postPath = string.Format(@"{0}\{1}\{2}", ROOT_PATH, "POST", "post.dat");
                    if (!System.IO.File.Exists(postPath))
                    {
                        StreamWriter file_out_postPath = new StreamWriter(postPath, false);
                        file_out_postPath.WriteLine("[DATA_START]");
                        string[] postAry = postDataStr.Split('\n');
                        for (int i = 0; i < postAry.Length; i++)
                        {
                            file_out_postPath.WriteLine(postAry[i]);
                        }
                        file_out_postPath.WriteLine("[DATA_END]");
                        file_out_postPath.Close();
                    }
                }

                #endregion

                #region 輸出 Shop Doc

                //輸出 Shop Doc
                if (checkBoxIS_SHOPDOC.Checked)
                {
                    sCaxLoadingDlg.SetLoadingText("輸出 Shop Doc...");

                    nameStr = "IS_SHOPDOC";
                    valueStr = "1";
                    RunShopDoc(ROOT_PATH);
                }
                else
                {
                    nameStr = "IS_SHOPDOC";
                    valueStr = "0";
                }
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                #endregion




                sCaxLoadingDlg.SetLoadingText("輸出工單...");

                // 20150526 CMM拿掉，COXON也不要了
                nameStr = "IS_CMM";
                valueStr = "1";
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //自定義VIEW
                nameStr = "DEFINE_VIEW";
                valueStr = VIEW_DADA;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                //裝夾方式及治具定義
                nameStr = "DEFINE_FIXTURE";
                valueStr = "";
                // 20150526 多治具處理
                if (labelXFixture.Text == "多治具")
                {
                    for (int i = 0; i < fixtureLst.Count; i++)
                    {
                        try
                        {
                            string fixtureName = fixtureLst[i].componentOcc.GetStringAttribute("FIXTURE_TYPE");
                            valueStr += fixtureName;
                            if (i != fixtureLst.Count - 1)
                            {
                                valueStr += ",";
                            }
                        }
                        catch (System.Exception ex)
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    valueStr = labelXFixture.Text;
                }
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                exportETableStr += "\n[STL]";
                exportETableStr += "\n[SHOPDOC]";
                exportETableStr += "\n[POST]";

                //備註說明
                nameStr = "REMARK";
                valueStr = textBoxNote.Text;
                exportETableStr += "\n\t\t";
                bufStr = exportETableStr;
                exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }}", bufStr, nameStr, valueStr);

                exportETableStr += "\n\t],";
                exportETableStr += "\n\t\"programs\":[";

                exportETableStr += "\n[SHOPDOC_LIST]";

                exportETableStr += "\n\t]";
                exportETableStr += "\n}";


                //讀取shopdoc.dat
                string shopdocDataStr = "";
                string shopdocProgramsStr = "";
                string shopdocBuf = "";
                string shopdocPath = "";
                shopdocPath = string.Format(@"{0}\{1}\{2}", ROOT_PATH, "SHOP_DOC", "shopdoc.dat");
                ArrayList shopdocAry = new ArrayList();
                ReadFileAry(shopdocPath, shopdocAry);
                for (int i = 0; i < shopdocAry.Count; i++)
                {
/*
                    if (shopdocAry[i].ToString() == "[DATA_START]")
                    {
                        for (int j = i + 1; j < shopdocAry.Count; j++)
                        {
                            if (shopdocAry[j].ToString() == "[DATA_END]")
                            {
                                i = j;
                                shopdocBuf = shopdocDataStr;
                                shopdocDataStr = shopdocBuf.TrimEnd('\n');
                                break;
                            }
                            shopdocDataStr += "\t\t";
                            shopdocDataStr += shopdocAry[j];
                            shopdocDataStr += "\n";
                        }
                    }
*/

                    if (shopdocAry[i].ToString() == "[PROGRAMS_START]")
                    {
                        for (int j = i + 1; j < shopdocAry.Count; j++)
                        {
                            if (shopdocAry[j].ToString() == "[PROGRAMS_END]")
                            {
                                i = j;
                                shopdocBuf = shopdocProgramsStr;
                                shopdocProgramsStr = shopdocBuf.TrimEnd('\n');
                                shopdocBuf = shopdocProgramsStr;
                                shopdocProgramsStr = shopdocBuf.TrimEnd(',');
                                break;
                            }
                            shopdocProgramsStr += "\t\t";
                            shopdocProgramsStr += shopdocAry[j];
                            shopdocProgramsStr += "\n";
                        }
                    }
                }
                if (shopdocAry.Count != 0 && shopdocProgramsStr == "")
                {
                    sCaxLoadingDlg.Stop();
                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ShopDoc 執行錯誤!");
                    return false;
                }

                // 20150721 將安全高度取代成C#程式抓到的值
                shopdocProgramsStr = shopdocProgramsStr.Replace("[CLEARANE_PLANE]", CLEARANE_PLANE);

                //讀取post.dat
                string postData = "";
                string post_path = "";
                post_path = string.Format(@"{0}\{1}\{2}", ROOT_PATH, "POST", "post.dat");
                ArrayList postFileAry = new ArrayList();
                ReadFileAry(post_path, postFileAry);
                for (int i = 0; i < postFileAry.Count; i++)
                {
                    if (postFileAry[i].ToString() == "[DATA_START]")
                    {
                        for (int j = i + 1; j < postFileAry.Count; j++)
                        {
                            if (postFileAry[j].ToString() == "[DATA_END]")
                            {
                                i = j;
                                shopdocBuf = postData;
                                postData = shopdocBuf.TrimEnd('\n');
                                break;
                            }
                            postData += "\t\t";
                            postData += postFileAry[j];
                            postData += "\n";
                        }
                    }
                }

                //讀取stl.dat
                string stlData = "";
                string StlPath = "";
                StlPath = string.Format(@"{0}\{1}", ROOT_PATH, "stl.dat");
                ArrayList stlFileAry = new ArrayList();
                ReadFileAry(StlPath, stlFileAry);
                for (int i = 0; i < stlFileAry.Count; i++)
                {
                    if (stlFileAry[i].ToString() == "[DATA_START]")
                    {
                        for (int j = i + 1; j < stlFileAry.Count; j++)
                        {
                            if (stlFileAry[j].ToString() == "[DATA_END]")
                            {
                                i = j;
                                shopdocBuf = stlData;
                                stlData = shopdocBuf.TrimEnd('\n');
                                break;
                            }
                            stlData += "\t\t";
                            stlData += stlFileAry[j];
                            stlData += "\n";
                        }
                    }
                }

                shopdocBuf = exportETableStr;
                exportETableStr = shopdocBuf.Replace("[STL]", stlData);
                shopdocBuf = exportETableStr;
                exportETableStr = shopdocBuf.Replace("[SHOPDOC]", shopdocDataStr);
                shopdocBuf = exportETableStr;
                exportETableStr = shopdocBuf.Replace("[SHOPDOC_LIST]", shopdocProgramsStr);
                shopdocBuf = exportETableStr;
                exportETableStr = shopdocBuf.Replace("[POST]", postData);



                string etablePath = "";
                etablePath = string.Format(@"{0}\{1}", ROOT_PATH, "cam2mes.dat");

                StreamWriter file_out_etable = new StreamWriter(etablePath, false);

                string[] etableAry = exportETableStr.Split('\n');
                for (int i = 0; i < etableAry.Length; i++)
                {
                    file_out_etable.WriteLine(etableAry[i]);
                }
                file_out_etable.Close();

                CaxAsm.ComponentShow(sCimAsmCompPart.fixture.comp);

                //刪除原始輸出的裝夾圖(JPG)
                string fixturePNG = string.Format(@"{0}\{1}.jpg", Path.GetDirectoryName(displayPart.FullPath), section_face);
                if (System.IO.File.Exists(fixturePNG))
                {
                    System.IO.File.Delete(fixturePNG);
                }
                //刪除原始輸出的裝夾圖(CGM)
                string fixtureCGM = string.Format(@"{0}\{1}.cgm", Path.GetDirectoryName(displayPart.FullPath), section_face);
                if (System.IO.File.Exists(fixtureCGM))
                {
                    System.IO.File.Delete(fixtureCGM);
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool ExportElectrodeTable()
        {
            try
            {
                bool status;
                Part displayPart = theSession.Parts.Display;

                string newTaskFolder = "";
                //string newMachineFolder = "";

                string camVerStr = "";
                if (sMesDatData.PART_TYPE_ID == "5")
                {
                    //CAM版本號 = 設計版次英文字母 + EDM CAM工作流水號
                    //20150514 不需要CAM版本號，只使用EDM CAM工作流水號
                    camVerStr = string.Format(@"{0}", sMesDatData.EDM_TASK_NO);
                }
                else
                {
                    //CAM版本號 = 設計版次英文字母 + CNC CAM工作流水號
                    //20150514 不需要CAM版本號，只使用CNC CAM工作流水號
                    camVerStr = string.Format(@"{0}", sMesDatData.TASK_NO);
                }

                for (int a = 0; a < sMesDatData.ED_PARTS.Count;a++ )
                {
                    string exportETableStr = "";
                    string bufStr = "";
                    string nameStr = "";
                    string valueStr = "";

                    sCaxLoadingDlg.SetLoadingText("輸出裝夾圖...");

                    if (elecPartNoNote != null)
                    {
                        List<string> noteAry = new List<string>();
                        noteAry.Add(sMesDatData.ED_PARTS[a].PART_NO);
                        SetDraftingNote((NXOpen.Annotations.Note)elecPartNoNote, noteAry);
                    }

                    //建立SHOP_DOC目錄
                    string shopdocFolder = "";
                    shopdocFolder = string.Format(@"{0}\{1}", ROOT_PATH, "SHOP_DOC");
                    CaxFile.CreateWinFolder(shopdocFolder);
                    
                    //建立SHOP_DOC_TEMP目錄
                    string shopdocTempFolder = "";
                    shopdocTempFolder = string.Format(@"{0}\{1}", ROOT_PATH, "SHOP_DOC_TEMP");
                    CaxFile.CreateWinFolder(shopdocTempFolder);

                    //取得裝夾圖CGM輸出路徑
                    string cgmPath = string.Format(@"{0}\{1}.cgm", shopdocTempFolder, section_face);

                    // 20151102 出圖前顯示所有Curve, Sketch, Drawing Objects
                    int numberShown = theSession.DisplayManager.ShowByType(NXOpen.DisplayManager.ShowHideType.Sketches, NXOpen.DisplayManager.ShowHideScope.AnyInAssembly);
                    numberShown = theSession.DisplayManager.ShowByType(NXOpen.DisplayManager.ShowHideType.Curves, NXOpen.DisplayManager.ShowHideScope.AnyInAssembly);
                    numberShown = theSession.DisplayManager.ShowByType(NXOpen.DisplayManager.ShowHideType.DrawingObjects, NXOpen.DisplayManager.ShowHideScope.AnyInAssembly);
                    CaxPart.Update();

                    // 20150520 Stewart
                    // 出裝夾圖前先隱藏所有零件，只顯示design零件
                    List<CaxAsm.CompPart> AsmCompAry = new List<CaxAsm.CompPart>();
                    CaxAsm.GetAsmCompStruct(out AsmCompAry, out sCimAsmCompPart);
                    foreach (CaxAsm.CompPart compPart in AsmCompAry)
                    {
                        CaxAsm.ComponentHide(compPart.componentOcc);
                    }
                    CaxAsm.ComponentShow(sCimAsmCompPart.design.comp);

                    //輸出裝夾圖CGM
                    status = CaxExport.ExportCGM(section_face, cgmPath);
                    if (!status)
                    {
                        sCaxLoadingDlg.Stop();
                        CaxLog.ShowListingWindow("裝夾圖輸出錯誤...CGM");
                        return false;
                    }

                    //取得裝夾圖JPG輸出路徑
                    string jpgPath = Path.ChangeExtension(cgmPath, "jpg");
                    status = CaxExport.ExportCGM2JPG(section_face, cgmPath, jpgPath);
                    if (!status)
                    {
                        sCaxLoadingDlg.Stop();
                        CaxLog.ShowListingWindow("裝夾圖輸出錯誤...JPG");
                        return false;
                    }


                    //任務目錄\電極模號\電極件號\電極設計版本號\工段_加工面_CAM版本號\機台型號
                    newTaskFolder = string.Format(@"{0}\{1}\{2}\{3}\{4}_{5}_{6}\{7}", 
                        CaxCNC.GetCimLocalTaskDir(), 
                        sMesDatData.ED_PARTS[a].MOLD_NO, 
                        sMesDatData.ED_PARTS[a].PART_NO,
                        sMesDatData.ED_PARTS[a].DES_VER_NO,
                        sMesDatData.SECTION_ID, sMesDatData.FACE_TYPE_ID, camVerStr,
                        sMesDatData.ED_PARTS[a].MAC_MODEL_NO);

                    //CaxLog.ShowListingWindow(newTaskFolder);

                    //判斷此目錄是否已存在
                    if (!System.IO.Directory.Exists(newTaskFolder))
                    {
                        //CaxLog.ShowListingWindow("不存在，建立此目錄");
                        //不存在，建立此目錄
                        status = CaxFile.CreateWinFolder(newTaskFolder);
                        if (!status)
                        {
                            CaxLog.ShowListingWindow(newTaskFolder + " 目錄建立失敗...");
                            return false;
                        }
                    }

                    #region 輸出刀具-精銑面對應
                    
                    sCaxLoadingDlg.SetLoadingText("輸出刀具-精銑面對應...");

                    //輸出刀具-精銑面對應
                    //   讀取刀具-精銑面對應配置檔
                    string configDataPath = string.Format(@"{0}\Cimforce\CNC\config\{1}", CaxFile.GetCimforceEnvDir(), "MappingConfig.txt");
                    FaceToolMappingConfig sMappingConfig = new FaceToolMappingConfig();
                    status = ReadMappingConfig(configDataPath, out sMappingConfig);
                    if (!status)
                    {
                        sCaxLoadingDlg.Stop();
                        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "讀取刀具-精銑面對應配置檔...");
                        return false;
                    }
                    //   決定配對方式
                    string mappingType = sMappingConfig.MappingType;
                    pointMaxDist = Convert.ToDouble(sMappingConfig.pointMaxDist);
                    pointMinDist = Convert.ToDouble(sMappingConfig.pointMinDist);
                    if (mappingType == "1")
                    {
                        //CAM Group
                        status = faceToolMapping(newTaskFolder);
                        if (!status)
                        {
                            sCaxLoadingDlg.Stop();
                            UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "刀具-精銑面配對<1>錯誤...");
                            return false;
                        }
                    }
                    else if (mappingType == "2")
                    {
                        //幾何關係
                        status = faceToolMapping2(newTaskFolder);
                        if (!status)
                        {
                            sCaxLoadingDlg.Stop();
                            UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "刀具-精銑面配對<2>錯誤...");
                            return false;
                        }
                    }
                    else if (mappingType == "0")
                    {
                        // empty file
                        status = faceToolMapping0(newTaskFolder);
                        if (!status)
                        {
                            sCaxLoadingDlg.Stop();
                            UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "刀具-精銑面配對<0>錯誤...");
                            return false;
                        }
                    }
                    else
                    {
                        sCaxLoadingDlg.Stop();
                        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "刀具-精銑面對應類型錯誤，請查看配置檔...");
                        return false;
                    }
                    
                    #endregion

                    #region 輸出Json (前半)
                    exportETableStr += "{";
                    exportETableStr += "\n\t\"work\":[";

                    //裝夾圖名稱
                    nameStr = "FIXTURE_PNG";
                    valueStr = string.Format(@"{0}.jpg", section_face);
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //裝夾圖路徑
                    nameStr = "FIXTURE_PATH";
                    valueStr = string.Format(@"{0}/{1}.jpg", "SHOP_DOC", section_face);
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出上傳KEY (模具號碼)
                    nameStr = "MOLD_NO";
                    valueStr = sMesDatData.ED_PARTS[a].MOLD_NO;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出上傳KEY (設計版本號碼)
                    nameStr = "DES_VER_NO";
                    valueStr = sMesDatData.ED_PARTS[a].DES_VER_NO;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出上傳KEY (工令單號)
                    nameStr = "WORK_NO";
                    valueStr = sMesDatData.ED_PARTS[a].WORK_NO;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出上傳KEY (零件號碼)
                    nameStr = "PART_NO";
                    valueStr = sMesDatData.ED_PARTS[a].PART_NO;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出上傳KEY (製程版本編號)
                    nameStr = "MFC_NO";
                    valueStr = sMesDatData.ED_PARTS[a].MFC_NO;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出上傳KEY (製程項目編號)
                    nameStr = "MFC_TASK_NO";
                    valueStr = sMesDatData.ED_PARTS[a].MFC_TASK_NO;
                    //valueStr = sMesDatData.MFC_TASK_NO;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出上傳KEY (CAM工作流水號)
                    nameStr = "TASK_NO";
                    valueStr = sMesDatData.ED_PARTS[a].TASK_NO;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出上傳KEY (機台型號工廠內代號)
                    nameStr = "MAC_MODEL_NO";
                    valueStr = sMesDatData.ED_PARTS[a].MAC_MODEL_NO;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出上傳KEY (CNC CAM POST 工作流水號)
                    nameStr = "TASK_SRNO";
                    valueStr = sMesDatData.ED_PARTS[a].TASK_SRNO;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出上傳KEY (EDM_CAM流水號)
                    nameStr = "EDM_TASK_NO";
                    valueStr = sMesDatData.EDM_TASK_NO;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出任務名稱(TASK_NAME)
                    nameStr = "TASK_NAME";
                    valueStr = string.Format(@"{0}_{1}_{2}_{3}", sMesDatData.ED_PARTS[a].MOLD_NO, sMesDatData.ED_PARTS[a].PART_NO, sMesDatData.ED_PARTS[a].DES_VER_NO, sMesDatData.ED_PARTS[a].MAC_MODEL_NO);
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出 工段_粗精洗 名稱(SECTION_ID)
                    nameStr = "SECTION_ID";
                    //valueStr = string.Format(@"{0}_{1}_{2}", sMesDatData.section_id, sMesDatData.mill_type_id, sMesDatData.face_type_id);
                    valueStr = string.Format(@"{0}", sMesDatData.SECTION_ID);
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出 加工面 名稱(FACE_TYPE_ID)
                    nameStr = "FACE_TYPE_ID";
                    valueStr = string.Format(@"{0}", sMesDatData.FACE_TYPE_ID);
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);


                    double[] minWcs;
                    double[] maxWcs;
                    //基準角到X中心
                    if (sBaseFaces.hasA)
                    {
                        CaxPart.AskBoundingBoxExactByWCS(sBaseFaces.baseFaceA.face.Tag, out minWcs, out maxWcs);
                        nameStr = "CONNER_X";
                        if (sBaseFaces.baseFaceA.sFaceData.dir[0] > 0.9)
                        {
                            valueStr = maxWcs[0].ToString("f4");
                        }
                        else
                        {
                            valueStr = minWcs[0].ToString("f4");
                        }
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }
                    else
                    {
                        nameStr = "CONNER_X";
                        valueStr = maxDesignBodyWcs[0].ToString("f4");
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }
                    //基準角到Y中心
                    if (sBaseFaces.hasB)
                    {
                        CaxPart.AskBoundingBoxExactByWCS(sBaseFaces.baseFaceB.face.Tag, out minWcs, out maxWcs);
                        nameStr = "CONNER_Y";
                        if (sBaseFaces.baseFaceB.sFaceData.dir[1] > 0.9)
                        {
                            valueStr = maxWcs[1].ToString("f4");
                        }
                        else
                        {
                            valueStr = minWcs[1].ToString("f4");
                        }
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }
                    else
                    {
                        nameStr = "CONNER_Y";
                        valueStr = maxDesignBodyWcs[1].ToString("f4");
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }
                    //基準角到Z中心
                    if (sBaseFaces.hasC)
                    {
                        CaxPart.AskBoundingBoxExactByWCS(sBaseFaces.baseFaceC.face.Tag, out minWcs, out maxWcs);
                        nameStr = "CONNER_Z";
                        if (sBaseFaces.baseFaceC.sFaceData.dir[2] > 0.9)
                        {
                            valueStr = maxWcs[2].ToString("f4");
                        }
                        else
                        {
                            valueStr = minWcs[2].ToString("f4");
                        }
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }
                    else
                    {
                        nameStr = "CONNER_Z";
                        valueStr = maxDesignBodyWcs[2].ToString("f4");
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }

//                     for (int i = 0; i < baseCornerAry.Count; i++)
//                     {
//                         CaxPart.AskBoundingBoxExactByWCS(baseCornerAry[i].face.Tag, out minWcs, out maxWcs);
// 
//                         if (baseCornerAry[i].sFaceData.dir[0] > 0.9999)
//                         {
//                             //基準角到X中心
//                             nameStr = "CONNER_X";
//                             valueStr = maxWcs[0].ToString("f4");
//                             exportETableStr += "\n\t\t";
//                             bufStr = exportETableStr;
//                             exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                         }
//                         else if (baseCornerAry[i].sFaceData.dir[0] < -0.9999)
//                         {
//                             //基準角到X中心
//                             nameStr = "CONNER_X";
//                             valueStr = minWcs[0].ToString("f4");
//                             exportETableStr += "\n\t\t";
//                             bufStr = exportETableStr;
//                             exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                         }
//                         else if (baseCornerAry[i].sFaceData.dir[1] > 0.9999)
//                         {
//                             //基準角到Y中心
//                             nameStr = "CONNER_Y";
//                             valueStr = maxWcs[1].ToString("f4");
//                             exportETableStr += "\n\t\t";
//                             bufStr = exportETableStr;
//                             exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                         }
//                         else if (baseCornerAry[i].sFaceData.dir[1] < -0.9999)
//                         {
//                             //基準角到Y中心
//                             nameStr = "CONNER_Y";
//                             valueStr = minWcs[1].ToString("f4");
//                             exportETableStr += "\n\t\t";
//                             bufStr = exportETableStr;
//                             exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                         }
//                         else if (baseCornerAry[i].sFaceData.dir[2] > 0.9999)
//                         {
//                             //基準角到Z中心
//                             nameStr = "CONNER_Z";
//                             valueStr = maxWcs[2].ToString("f4");
//                             exportETableStr += "\n\t\t";
//                             bufStr = exportETableStr;
//                             exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                         }
//                         else if (baseCornerAry[i].sFaceData.dir[2] < -0.9999)
//                         {
//                             //基準角到Z中心
//                             nameStr = "CONNER_Z";
//                             valueStr = minWcs[2].ToString("f4");
//                             exportETableStr += "\n\t\t";
//                             bufStr = exportETableStr;
//                             exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
//                         }
//                     }

                    
                    //匯出工件T面轉BO面的OFFSET數值(Z_OFFSET) 2014/07/10 賴斌要求全部輸出正值
                    if (sExportWorkTabel.Z_MOVE == "")
                    {
                        nameStr = "Z_OFFSET";
                        valueStr = "0";
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }
                    else
                    {
                        double z_move = Convert.ToDouble(sExportWorkTabel.Z_MOVE);
                        nameStr = "Z_OFFSET";
                        valueStr = Math.Abs(z_move).ToString("f4");
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    }

                    //匯出Zbase(BOTTOM_Z)(目前給正值)      2014/07/10 賴斌要求全部輸出正值
                    nameStr = "BOTTOM_Z";
                    if (BOTTOM_Z < 0)
                    {
                        BOTTOM_Z = BOTTOM_Z * (-1);
                    }
                    valueStr = BOTTOM_Z.ToString("f4");
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //X+
                    nameStr = "MAX_X_POSITION";
                    valueStr = Math.Round(maxDesignBodyWcs[0], 3, MidpointRounding.ToEven).ToString("f3");
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //Y+
                    nameStr = "MAX_Y_POSITION";
                    valueStr = Math.Round(maxDesignBodyWcs[1], 3, MidpointRounding.ToEven).ToString("f3");
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //Z+
                    nameStr = "MAX_Z_POSITION";
                    valueStr = Math.Round(maxDesignBodyWcs[2], 3, MidpointRounding.ToEven).ToString("f3");
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //X-
                    nameStr = "MIN_X_POSITION";
                    valueStr = Math.Round(minDesignBodyWcs[0], 3, MidpointRounding.ToEven).ToString("f3");
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //Y-
                    nameStr = "MIN_Y_POSITION";
                    valueStr = Math.Round(minDesignBodyWcs[1], 3, MidpointRounding.ToEven).ToString("f3");
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //Z-
                    nameStr = "MIN_Z_POSITION";
                    valueStr = Math.Round(minDesignBodyWcs[2], 3, MidpointRounding.ToEven).ToString("f3");
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    #endregion

                    #region 輸出工件+治具JT

                    sCaxLoadingDlg.SetLoadingText("輸出工件&治具JT...");

                    CaxAsm.ComponentShow(sCimAsmCompPart.fixture.comp);

                    //轉出JT檔(工件+治具)
                    string displayPartName = Path.GetFileNameWithoutExtension(displayPart.FullPath);
                    string displayPartNewName = displayPartName.Replace(".", "_");
                    displayPartName = displayPartNewName;
                    displayPartNewName = displayPartName.Replace("-", "_");

                    string ExportJTtessUGconfigPath = string.Format(@"{0}\jt\tessUG.config", CaxCNC.GetCimLocalProgramsDir());
                    if (!System.IO.File.Exists(ExportJTtessUGconfigPath))
                    {
                        sCaxLoadingDlg.Stop();
                        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ExportJTtessUGconfigPath + " 檔案遺失!");
                        return false;
                    }

                    string jtPath = string.Format(@"{0}\{1}.jt", newTaskFolder, displayPartNewName);
                    status = ExportJTAsASingleFile(jtPath, ExportJTtessUGconfigPath);
                    if (!status)
                    {
                        sCaxLoadingDlg.Stop();
                        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "JT 輸出失敗!");
                        return false;
                    }

                    //匯出JT路徑
                    nameStr = "TOP_ASM_JT_LINK";
                    valueStr = Path.GetFileName(jtPath);
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    CaxAsm.ComponentHide(sCimAsmCompPart.fixture.comp);

                    #endregion


                    //匯出設計時間(DESIGN_TIME)
                    nameStr = "DESIGN_TIME";
                    valueStr = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //匯出使用者名稱(USER_ID)
                    nameStr = "USER_ID";
                    valueStr = sMesDatData.USER_ID;//System.Environment.UserName;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //旋轉視角
                    if (labelView.Text == "定義完成")
                    {
                        theSession.Parts.Display.ModelingViews.WorkView.Orient(VIEW_MATRIX);
                        theSession.Parts.Display.ModelingViews.WorkView.SetScale(VIEW_SCALE);
                        theSession.Parts.Display.Views.WorkView.Fit();
                    }
                    else
                    {
                        //未定義，使用目前視角輸出
                        Part workPart = theSession.Parts.Work;
                        VIEW_MATRIX = workPart.ModelingViews.WorkView.Matrix;
                        VIEW_SCALE = workPart.ModelingViews.WorkView.Scale;

                        //取得輸出字串
                        string viewMatrixStr;
                        string scaleStr;
                        viewMatrixStr = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", VIEW_MATRIX.Xx, VIEW_MATRIX.Xy, VIEW_MATRIX.Xz, VIEW_MATRIX.Yx, VIEW_MATRIX.Yy, VIEW_MATRIX.Yz, VIEW_MATRIX.Zx, VIEW_MATRIX.Zy, VIEW_MATRIX.Zz);
                        scaleStr = string.Format("{0}", VIEW_SCALE);
                        VIEW_DADA = string.Format("{0}_{1}", viewMatrixStr, scaleStr);
                        theSession.Parts.Display.Views.WorkView.Fit();
                    }

                    CaxAsm.ComponentHide(sCimAsmCompPart.fixture.comp);
                    theSession.Parts.Display.Views.WorkView.Fit();

                    sCaxLoadingDlg.SetLoadingText("輸出組立圖片檔...");

                    //匯出組立圖片檔
                    string asmPhotoPath;
                    //asmPhotoPath = Path.ChangeExtension(asmName, "jpg"); //變更副檔名: "D:\Downloads\test.txt"
                    asmPhotoPath = string.Format(@"{0}\{1}.jpg", newTaskFolder, Path.GetFileNameWithoutExtension(asmName));
                    theUfSession.Disp.CreateImage(asmPhotoPath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);
                    nameStr = "TOP_ASM_GRAPH_P";
                    valueStr = Path.GetFileName(asmPhotoPath);
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    //MessageBox.Show(exportETableStr);


                    //是否模仁
                    if (sMesDatData.PART_TYPE_ID == "0")
                    {
                        nameStr = "IS_CORE";
                        valueStr = "1";
                    }
                    else
                    {
                        nameStr = "IS_CORE";
                        valueStr = "0";
                    }
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    //MessageBox.Show(exportETableStr);

                    #region 輸出 STL 模擬檔案

                    //STL 模擬檔案
                    string stlDataStr = "";
                    if (checkBoxIS_STL.Checked)
                    {
                        sCaxLoadingDlg.SetLoadingText("輸出STL模擬檔案...");

                        nameStr = "IS_STL";
                        valueStr = "1";
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        Tag tagRootPart = theUfSession.Assem.AskRootPartOcc(displayPart.Tag);
                        if (tagRootPart == NXOpen.Tag.Null)
                        {
                            sCaxLoadingDlg.Stop();
                            return false;
                        }

                        //輸出STL - 素材
                        //sCimAsmCompPart.
                        sCaxLoadingDlg.SetLoadingText("輸出STL - 素材...");

                        string stlPath = Path.ChangeExtension(sCimAsmCompPart.blank.part.FullPath, "stl");
                        string stlName = Path.GetFileName(stlPath);

                        nameStr = "IPW_STL";
                        valueStr = stlName;
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "IPW_STL_PATH";
                        valueStr = stlName;
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        CaxPart.ExportBinarySTL(sCimAsmCompPart.blank.comp, FACET_TOLER, ADJ_TOLER, newTaskFolder);


                        //輸出STL - 治具...
                        sCaxLoadingDlg.SetLoadingText("輸出STL - 治具...");

//                         string fixtruValue = "";
//                         valueStr = stlName;
//                         fixtruValue += valueStr + ",";
                        stlPath = Path.ChangeExtension(sCimAsmCompPart.fixture.part.FullPath, "stl");
                        stlName = Path.GetFileName(stlPath);

                        //如果是治具零件，檢查是否有需要suppress的特徵
                        string attr_value = "";
                        Feature[] partFeats = sCimAsmCompPart.fixture.part.Features.ToArray();
                        List<Feature> SuppressFeatAry = new List<Feature>();
                        for (int j = 0; j < partFeats.Length; j++)
                        {
                            try
                            {
                                attr_value = "";
                                attr_value = partFeats[j].GetStringAttribute(CaxDefineParam.ATTR_CIM_FEAT_SUPPRESS);
                                if (attr_value == "1")
                                {
                                    SuppressFeatAry.Add(partFeats[j]);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                continue;
                            }
                        }

                        //SuppressFeature
                        for (int j = 0; j < SuppressFeatAry.Count; j++)
                        {
                            CaxPart.SuppressFeature(sCimAsmCompPart.fixture.part, SuppressFeatAry[j]);
                        }

                        CaxPart.ExportBinarySTL(sCimAsmCompPart.fixture.comp, FIXTURE_FACET_TOLER, FIXTURE_ADJ_TOLER, newTaskFolder);

                        //UnsuppressFeature
                        for (int j = 0; j < SuppressFeatAry.Count; j++)
                        {
                            CaxPart.UnsuppressFeature(sCimAsmCompPart.fixture.part, SuppressFeatAry[j]);
                        }

                        nameStr = "FIXTURE_STL";
                        valueStr = stlName;
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "FIXTURE_STL_PATH";
                        valueStr = stlName;
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //輸出STL - 設計檔
                        sCaxLoadingDlg.SetLoadingText("輸出STL - 設計檔...");

                        stlPath = Path.ChangeExtension(sCimAsmCompPart.design.part.FullPath, "stl");
                        stlName = Path.GetFileName(stlPath);

                        nameStr = "WORK_STL";
                        valueStr = stlName;
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "WORK_STL_PATH";
                        valueStr = stlName;
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        CaxPart.ExportBinarySTL(sCimAsmCompPart.design.comp, FACET_TOLER, ADJ_TOLER, newTaskFolder);

                        //寫出stl.dat
                        string stl_path = "";
                        stl_path = string.Format(@"{0}\{1}", newTaskFolder, "stl.dat");
                        StreamWriter file_out_stlPath = new StreamWriter(stl_path, false);
                        file_out_stlPath.WriteLine("[DATA_START]");
                        string[] stlAry = stlDataStr.Split('\n');
                        for (int i = 0; i < stlAry.Length; i++)
                        {
                            if (stlAry[i].Trim() == "")
                            {
                                continue;
                            }
                            file_out_stlPath.WriteLine(stlAry[i]);
                        }
                        file_out_stlPath.WriteLine("[DATA_END]");
                        file_out_stlPath.Close();
                    }
                    else
                    {
                        nameStr = "IS_STL";
                        valueStr = "0";
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "IPW_STL";
                        valueStr = "";
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "IPW_STL_PATH";
                        valueStr = "";
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "WORK_STL";
                        valueStr = "";
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "WORK_STL_PATH";
                        valueStr = "";
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "FIXTURE_STL";
                        valueStr = "";
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "FIXTURE_STL_PATH";
                        valueStr = "";
                        stlDataStr += "\n";
                        bufStr = stlDataStr;
                        stlDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //寫出stl.dat
                        string stl_path = "";
                        stl_path = string.Format(@"{0}\{1}", newTaskFolder, "stl.dat");
                        if (!System.IO.File.Exists(stl_path))
                        {
                            StreamWriter file_out_stlPath = new StreamWriter(stl_path, false);
                            file_out_stlPath.WriteLine("[DATA_START]");
                            string[] stlAry = stlDataStr.Split('\n');
                            for (int i = 0; i < stlAry.Length; i++)
                            {
                                if (stlAry[i].Trim() == "")
                                {
                                    continue;
                                }
                                file_out_stlPath.WriteLine(stlAry[i]);
                            }
                            file_out_stlPath.WriteLine("[DATA_END]");
                            file_out_stlPath.Close();
                        }
                    }
                    //MessageBox.Show(exportETableStr);

                    #endregion

                    #region 輸出Post

                    //Post
                    string postDataStr = "";

                    //建立POST目錄
                    string postFolder;
                    postFolder = string.Format(@"{0}\{1}", newTaskFolder, "POST");
                    CaxFile.CreateWinFolder(postFolder);

                    if (checkBoxIS_POST.Checked)
                    {
                        sCaxLoadingDlg.SetLoadingText("輸出 POST ...");

                        nameStr = "IS_POST";
                        valueStr = "1";
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        string postProgramNo = "";
                        string postProgramListPath = "";
                        for (int i = 0; i < ListToolLengehAry.Count; i++)
                        {
                            postProgramNo += ListToolLengehAry[i].oper_name + machine_group + ",";
                            postProgramListPath += "POST" + "/" + ListToolLengehAry[i].oper_name + machine_group + ",";
                        }
                        postProgramNo.TrimEnd(',');
                        postProgramListPath.TrimEnd(',');

                        //nameStr = "PROGRAM_NO";
                        nameStr = "PROGRAM_LIST_NAME";  //2014/01/06 kennyou 將PROGRAM_NO改為PROGRAM_LIST_NAME
                        valueStr = postProgramNo;
                        postDataStr += "";
                        bufStr = postDataStr;
                        postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "PROGRAM_LIST_PATH";
                        valueStr = postProgramListPath;
                        postDataStr += "\n";
                        bufStr = postDataStr;
                        postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //選取的機台廠牌
                        nameStr = "SUPPORT_MACHINE";
                        valueStr = support_machine;
                        postDataStr += "\n";
                        bufStr = postDataStr;
                        postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //選取的規格型號
                        nameStr = "MACHINE_TYPE";
                        valueStr = machine_type;
                        postDataStr += "\n";
                        bufStr = postDataStr;
                        postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //選取的控制器
                        nameStr = "CONTROLLER";
                        valueStr = controller;
                        postDataStr += "\n";
                        bufStr = postDataStr;
                        postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //輸出的機台類型格式(副檔名)
                        nameStr = "MACHINE_GROUP";
                        valueStr = machine_group;
                        postDataStr += "\n";
                        bufStr = postDataStr;
                        postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //RunPost
                        Part workPart = theSession.Parts.Work;
                        string postOutPath = "";
                        string postMsg = "";
                        NXOpen.CAM.CAMObject[] objects1 = new NXOpen.CAM.CAMObject[1];

                        //sMesDatData
                        List<string> operationNameAry = new List<string>();
                        for (int i = 0; i < ListToolLengehAry.Count; i++)
                        {
                            if (ListToolLengehAry[i].part_offset >= sMesDatData.ED_PARTS[a].DISCHARGE_GAP && ListToolLengehAry[i].part_offset <= sMesDatData.ED_PARTS[a].DISCHARGE_GAP)
                            {
                                operationNameAry.Add(ListToolLengehAry[i].oper_name);
                            }
                        }

                        for (int i = 0; i < operationNameAry.Count; i++)
                        {
                            postMsg = string.Format(@"輸出 POST ({0}/{1}) - {2}...", (i + 1).ToString(), operationNameAry.Count.ToString(), operationNameAry[i]);
                            sCaxLoadingDlg.SetLoadingText(postMsg);

                            Operation operation = (Operation)workPart.CAMSetup.CAMOperationCollection.FindObject(operationNameAry[i]);

                            double gap = sMesDatData.ED_PARTS[a].DISCHARGE_GAP;
                            if (gap > 0)
                            {
                                gap = gap * (-1);
                            }

                            // 20160216 電極CAM編程優化
                            NCGroup parentGeom = operation.GetParent(CAMSetup.View.Geometry);
//                             CaxLog.ShowListingWindow(parentGeom.Name);
//                             CaxLog.ShowListingWindow(operation.GetType().ToString());
                            if (parentGeom.Name == "WORKPIECE_EL_AREA_2D")
                            {
                                if (operation.GetType().ToString() == "NXOpen.CAM.VolumeBased25DMillingOperation")
                                {
//                                     CaxLog.ShowListingWindow(gap.ToString());
                                    MillOperationBuilder operBuilder = workPart.CAMSetup.CAMOperationCollection.CreateVolumeBased25dMillingOperationBuilder(operation);
                                    operBuilder.CutParameters.FloorStock.Value = gap;
                                    NXObject nXObject1;
                                    nXObject1 = operBuilder.Commit();
                                    operBuilder.Destroy();
                                }
                                else
                                {
                                    CaxLog.ShowListingWindow("Operation: " + operation.Name + ", 類型: " + operation.GetType().ToString() + ", 位於WORKPIECE_EL_AREA_2D底下, 目前不支援修改GAP之功能...");
                                }
                            }

                            status = CaxCAM.SetPartOffset(gap, "WORKPIECE_EL_AREA");
                            if (!status)
                            {
                                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, operation.Name + " Part Offset 設定失敗...");
                                return false;
                            }


                            if (operation.GetStatus() != CAMObject.Status.Complete && operation.GetStatus() != CAMObject.Status.Repost)
                            {
                                status = CaxCAM.GenerateToolPath(operation.Name);
                                if (!status)
                                {
                                    UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, operation.Name + " Generate 失敗...");
                                    return false;
                                }
                            }


                            postOutPath = string.Format(@"{0}\{1}\{2}{3}", newTaskFolder, "POST", operationNameAry[i], machine_group);
                            NXOpen.CAM.CAMObject camObj = (NXOpen.CAM.CAMObject)workPart.CAMSetup.CAMOperationCollection.FindObject(operationNameAry[i]);
                            objects1[0] = camObj;
                            try
                            {
                                workPart.CAMSetup.PostprocessWithSetting(objects1, postFunction, postOutPath, NXOpen.CAM.CAMSetup.OutputUnits.PostDefined, NXOpen.CAM.CAMSetup.PostprocessSettingsOutputWarning.PostDefined, NXOpen.CAM.CAMSetup.PostprocessSettingsReviewTool.PostDefined);
                            }
                            catch (System.Exception ex)
                            {
                                int indexNum = i + 1;
                                CaxLog.ShowListingWindow("POST 輸出錯誤：[" + indexNum.ToString() + "] " + operationNameAry[i]);
                                sCaxLoadingDlg.Stop();
                                return false;
                            }
                        }

                        //20150515 By Andy
                        #region 複製零件到其他Gap任務目錄
                        //零件類型為電極，將所有零件複製到其他Gap任務目錄裡
                        CaxPart.Save();
                        AsmCompAry = new List<CaxAsm.CompPart>();
                        CaxAsm.GetAsmCompTree(out AsmCompAry);
                        string displayPartFullPath = theSession.Parts.Display.FullPath;
                        //CaxLog.ShowListingWindow(displayPartFullPath);
                        string displaypartNewPath = string.Format(@"{0}\{1}", newTaskFolder, Path.GetFileName(displayPartFullPath));
                        //CaxLog.ShowListingWindow(displaypartNewPath);
                        try
                        {
                            if (displayPartFullPath != displaypartNewPath)
                            {
                                System.IO.File.Copy(theSession.Parts.Display.FullPath, displaypartNewPath, true);
                            }
                            else
                            {
                                //CaxLog.ShowListingWindow("重複摟display part");
                            }

                            //複製組件下所有零件檔案至其他Gap任務目錄
                            for (int j = 0; j < AsmCompAry.Count; j++)
                            {
                                string newCompPath = string.Format(@"{0}\{1}", newTaskFolder, Path.GetFileName(AsmCompAry[j].part_name));
                                //CaxLog.ShowListingWindow(newCompPath);
                                if (AsmCompAry[j].part_name != newCompPath)
                                {
                                    System.IO.File.Copy(AsmCompAry[j].part_name, newCompPath, true);
                                }
                                else
                                {
                                    //CaxLog.ShowListingWindow("重複摟");
                                }
                       
                            }
                            //CaxLog.ShowListingWindow("hahaha");
                        }
                        catch (System.Exception ex)
                        {
                            //CaxLog.ShowListingWindow("QQ");
                            //CaxLog.ShowListingWindow(ex.Message);
                        }
                        #endregion

                        //RunPost();

                        //寫出post.dat
                        string postPath = "";
                        postPath = string.Format(@"{0}\{1}\{2}", newTaskFolder, "POST", "post.dat");
                        StreamWriter file_out_postPath = new StreamWriter(postPath, false);
                        file_out_postPath.WriteLine("[DATA_START]");
                        string[] postAry = postDataStr.Split('\n');
                        for (int i = 0; i < postAry.Length; i++)
                        {
                            file_out_postPath.WriteLine(postAry[i]);
                        }
                        file_out_postPath.WriteLine("[DATA_END]");
                        file_out_postPath.Close();
                    }
                    else
                    {
                        nameStr = "IS_POST";
                        valueStr = "0";
                        exportETableStr += "\n\t\t";
                        bufStr = exportETableStr;
                        exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "PROGRAM_NO";
                        valueStr = "";
                        postDataStr += "";
                        bufStr = postDataStr;
                        postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        nameStr = "PROGRAM_LIST_PATH";
                        valueStr = "";
                        postDataStr += "\n";
                        bufStr = postDataStr;
                        postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //選取的機台廠牌
                        nameStr = "SUPPORT_MACHINE";
                        valueStr = "";
                        postDataStr += "\n";
                        bufStr = postDataStr;
                        postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //選取的規格型號
                        nameStr = "MACHINE_TYPE";
                        valueStr = "";
                        postDataStr += "\n";
                        bufStr = postDataStr;
                        postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //選取的控制器
                        nameStr = "CONTROLLER";
                        valueStr = "";
                        postDataStr += "\n";
                        bufStr = postDataStr;
                        postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //輸出的機台類型格式(副檔名)
                        nameStr = "MACHINE_GROUP";
                        valueStr = "";
                        postDataStr += "\n";
                        bufStr = postDataStr;
                        postDataStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                        //post.dat不存在才寫出post.dat(已存在代表之前已做過，不取代)
                        string postPath = "";
                        postPath = string.Format(@"{0}\{1}\{2}", newTaskFolder, "POST", "post.dat");
                        if (!System.IO.File.Exists(postPath))
                        {
                            StreamWriter file_out_postPath = new StreamWriter(postPath, false);
                            file_out_postPath.WriteLine("[DATA_START]");
                            string[] postAry = postDataStr.Split('\n');
                            for (int i = 0; i < postAry.Length; i++)
                            {
                                file_out_postPath.WriteLine(postAry[i]);
                            }
                            file_out_postPath.WriteLine("[DATA_END]");
                            file_out_postPath.Close();
                        }
                    }

                    #endregion

                    #region 輸出 Shop Doc

                    //輸出 Shop Doc
                    if (checkBoxIS_SHOPDOC.Checked)
                    {
                        sCaxLoadingDlg.SetLoadingText("輸出 Shop Doc...");

                        nameStr = "IS_SHOPDOC";
                        valueStr = "1";
                        RunShopDoc(newTaskFolder);
                    }
                    else
                    {
                        nameStr = "IS_SHOPDOC";
                        valueStr = "0";
                    }
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    #endregion


                    sCaxLoadingDlg.SetLoadingText("輸出工單...");

                    // 20150526 CMM拿掉，COXON也不要了
                    nameStr = "IS_CMM";
                    valueStr = "1";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    #region 20150527 新增匯出帝寶開的欄位，電極部分全部填空白
                    // 放電基準孔圖檔名稱、路徑
                    nameStr = "BASEHOLE_PNG";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    nameStr = "BASEHOLE_PATH";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    // 加工前檢測圖檔名稱、路徑
                    nameStr = "BEFORECNC_PNG";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    nameStr = "BEFORECNC_PATH";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    // 加工後檢測圖檔名稱、路徑
                    nameStr = "AFTERCNC_PNG";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    nameStr = "AFTERCNC_PATH";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    //外包殘料檔案名稱(OUT_STL_NAME)
                    nameStr = "OUT_STL_NAME";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    // 邊線邊面檔案名稱(SIDELINE_NAME)
                    nameStr = "SIDEPART_NAME";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);
                    #endregion

                    // 20150526新增 匯出 機外or機內校正(CALIBRATOR) "1":機外   "2":機內
                    // 20150810 CNC洗電極一律輸出機外!!
                    nameStr = "CALIBRATOR";
                    valueStr = "1";
//                     string measureType = "";
//                     try
//                     {
//                         measureType = sCimAsmCompPart.fixture.comp.GetStringAttribute("CIM_FIXTURE_MEASURE_FINAL");
//                     }
//                     catch (System.Exception ex)
//                     {
//                     }
//                     if (measureType == "OUT")
//                     {
//                         valueStr = "1";
//                     }
//                     if (measureType == "IN")
//                     {
//                         valueStr = "2";
//                     }
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    // 20150526新增 匯出 校正方法(CALIBRATOR_METHOD) "1":雙邊求中  "2":單邊求中第三象限  "3":單邊求中第四象限  "4":求圓心(模板)
                    // 目前先空著!!
                    nameStr = "CALIBRATOR_METHOD";
                    valueStr = "";
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //自定義VIEW
                    nameStr = "DEFINE_VIEW";
                    valueStr = VIEW_DADA;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    //裝夾方式及治具定義
                    nameStr = "DEFINE_FIXTURE";
                    valueStr = labelXFixture.Text;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }},", bufStr, nameStr, valueStr);

                    exportETableStr += "\n[STL]";
                    exportETableStr += "\n[SHOPDOC]";
                    exportETableStr += "\n[POST]";

                    //備註說明
                    nameStr = "REMARK";
                    valueStr = textBoxNote.Text;
                    exportETableStr += "\n\t\t";
                    bufStr = exportETableStr;
                    exportETableStr = string.Format(@"{0}{{ ""name"":""{1}"", ""value"":""{2}"" }}", bufStr, nameStr, valueStr);

                    exportETableStr += "\n\t],";
                    exportETableStr += "\n\t\"programs\":[";

                    exportETableStr += "\n[SHOPDOC_LIST]";

                    exportETableStr += "\n\t]";
                    exportETableStr += "\n}";


                    //讀取shopdoc.dat
                    string shopdocDataStr = "";
                    string shopdocProgramsStr = "";
                    string shopdocBuf = "";
                    string shopdocPath = "";
                    shopdocPath = string.Format(@"{0}\{1}\{2}", newTaskFolder, "SHOP_DOC", "shopdoc.dat");
                    //CaxLog.ShowListingWindow(shopdocPath);

                    ArrayList shopdocAry = new ArrayList();
                    ReadFileAry(shopdocPath, shopdocAry);
                    for (int i = 0; i < shopdocAry.Count; i++)
                    {
/*
                        if (shopdocAry[i].ToString() == "[DATA_START]")
                        {
                            for (int j = i + 1; j < shopdocAry.Count; j++)
                            {
                                if (shopdocAry[j].ToString() == "[DATA_END]")
                                {
                                    i = j;
                                    shopdocBuf = shopdocDataStr;
                                    shopdocDataStr = shopdocBuf.TrimEnd('\n');
                                    break;
                                }
                                shopdocDataStr += "\t\t";
                                shopdocDataStr += shopdocAry[j];
                                shopdocDataStr += "\n";
                            }
                        }
*/

                        if (shopdocAry[i].ToString() == "[PROGRAMS_START]")
                        {
                            for (int j = i + 1; j < shopdocAry.Count; j++)
                            {
                                if (shopdocAry[j].ToString() == "[PROGRAMS_END]")
                                {
                                    i = j;
                                    shopdocBuf = shopdocProgramsStr;
                                    shopdocProgramsStr = shopdocBuf.TrimEnd('\n');
                                    shopdocBuf = shopdocProgramsStr;
                                    shopdocProgramsStr = shopdocBuf.TrimEnd(',');
                                    break;
                                }
                                shopdocProgramsStr += "\t\t";
                                shopdocProgramsStr += shopdocAry[j];
                                shopdocProgramsStr += "\n";
                            }
                        }
                    }
                    if (shopdocAry.Count != 0 && shopdocProgramsStr == "")
                    {
                        sCaxLoadingDlg.Stop();
                        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "ShopDoc 執行錯誤!");
                        return false;
                    }

                    // 20150721 將安全高度取代成C#程式抓到的值
                    shopdocProgramsStr = shopdocProgramsStr.Replace("[CLEARANE_PLANE]", CLEARANE_PLANE);

                    //讀取post.dat
                    string postData = "";
                    string post_path = "";
                    post_path = string.Format(@"{0}\{1}\{2}", newTaskFolder, "POST", "post.dat");
                    ArrayList postFileAry = new ArrayList();
                    ReadFileAry(post_path, postFileAry);
                    for (int i = 0; i < postFileAry.Count; i++)
                    {
                        if (postFileAry[i].ToString() == "[DATA_START]")
                        {
                            for (int j = i + 1; j < postFileAry.Count; j++)
                            {
                                if (postFileAry[j].ToString() == "[DATA_END]")
                                {
                                    i = j;
                                    shopdocBuf = postData;
                                    postData = shopdocBuf.TrimEnd('\n');
                                    break;
                                }
                                postData += "\t\t";
                                postData += postFileAry[j];
                                postData += "\n";
                            }
                        }
                    }

                    //讀取stl.dat
                    string stlData = "";
                    string StlPath = "";
                    StlPath = string.Format(@"{0}\{1}", newTaskFolder, "stl.dat");
                    ArrayList stlFileAry = new ArrayList();
                    ReadFileAry(StlPath, stlFileAry);
                    for (int i = 0; i < stlFileAry.Count; i++)
                    {
                        if (stlFileAry[i].ToString() == "[DATA_START]")
                        {
                            for (int j = i + 1; j < stlFileAry.Count; j++)
                            {
                                if (stlFileAry[j].ToString() == "[DATA_END]")
                                {
                                    i = j;
                                    shopdocBuf = stlData;
                                    stlData = shopdocBuf.TrimEnd('\n');
                                    break;
                                }
                                stlData += "\t\t";
                                stlData += stlFileAry[j];
                                stlData += "\n";
                            }
                        }
                    }

                    shopdocBuf = exportETableStr;
                    exportETableStr = shopdocBuf.Replace("[STL]", stlData);
                    shopdocBuf = exportETableStr;
                    exportETableStr = shopdocBuf.Replace("[SHOPDOC]", shopdocDataStr);
                    shopdocBuf = exportETableStr;
                    exportETableStr = shopdocBuf.Replace("[SHOPDOC_LIST]", shopdocProgramsStr);
                    shopdocBuf = exportETableStr;
                    exportETableStr = shopdocBuf.Replace("[POST]", postData);

//                     // 20160106 加入判斷刀具切削路徑是否異常
//                     CheckToolPath(sMesDatData.ED_PARTS, exportETableStr);

                    string etablePath = "";
                    etablePath = string.Format(@"{0}\{1}", newTaskFolder, "cam2mes.dat");

                    StreamWriter file_out_etable = new StreamWriter(etablePath, false);

                    string[] etableAry = exportETableStr.Split('\n');
                    for (int i = 0; i < etableAry.Length; i++)
                    {
                        file_out_etable.WriteLine(etableAry[i]);
                    }
                    file_out_etable.Close();

                    CaxAsm.ComponentShow(sCimAsmCompPart.fixture.comp);
                }

                //刪除原始輸出的裝夾圖(JPG)
                string fixturePNG = string.Format(@"{0}\{1}.jpg", Path.GetDirectoryName(displayPart.FullPath), section_face);
                if (System.IO.File.Exists(fixturePNG))
                {
                    System.IO.File.Delete(fixturePNG);
                }

                //刪除原始輸出的裝夾圖(CGM)
                string fixtureCGM = string.Format(@"{0}\{1}.cgm", Path.GetDirectoryName(displayPart.FullPath), section_face);
                if (System.IO.File.Exists(fixtureCGM))
                {
                    System.IO.File.Delete(fixtureCGM);
                }
            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow(ex.Message);
                return false;
            }

            return true;
        }


        public static bool SetDraftingNote(NXOpen.Annotations.Note note, List<string> noteAry)
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");
                
                NXOpen.Annotations.Note note1 = note;
                NXOpen.Annotations.DraftingNoteBuilder draftingNoteBuilder1;
                draftingNoteBuilder1 = workPart.Annotations.CreateDraftingNoteBuilder(note1);

                draftingNoteBuilder1.Origin.Plane.PlaneMethod = NXOpen.Annotations.PlaneBuilder.PlaneMethodType.UserDefined;

                draftingNoteBuilder1.Text.TextBlock.CustomSymbolScale = 1.0;

                draftingNoteBuilder1.Origin.SetInferRelativeToGeometry(false);

                draftingNoteBuilder1.Origin.SetInferRelativeToGeometry(true);

                theSession.SetUndoMarkName(markId1, "Note Dialog");

                draftingNoteBuilder1.Origin.Plane.PlaneMethod = NXOpen.Annotations.PlaneBuilder.PlaneMethodType.UserDefined;

                draftingNoteBuilder1.Origin.SetInferRelativeToGeometry(true);

                NXOpen.Annotations.LeaderData leaderData1;
                leaderData1 = workPart.Annotations.CreateLeaderData();

                leaderData1.StubSize = 5.0;

                leaderData1.Arrowhead = NXOpen.Annotations.LeaderData.ArrowheadType.FilledArrow;

                draftingNoteBuilder1.Leader.Leaders.Append(leaderData1);

                leaderData1.StubSide = NXOpen.Annotations.LeaderSide.Inferred;

                double symbolscale1;
                symbolscale1 = draftingNoteBuilder1.Text.TextBlock.SymbolScale;

                double symbolaspectratio1;
                symbolaspectratio1 = draftingNoteBuilder1.Text.TextBlock.SymbolAspectRatio;

                draftingNoteBuilder1.Origin.SetInferRelativeToGeometry(true);

                draftingNoteBuilder1.Origin.SetInferRelativeToGeometry(true);

                // ----------------------------------------------
                //   Dialog Begin Note
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Note");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Note");

                theSession.SetUndoMarkName(markId3, "Note - Text Input");

                theSession.SetUndoMarkVisibility(markId3, null, NXOpen.Session.MarkVisibility.Visible);

                theSession.SetUndoMarkVisibility(markId1, null, NXOpen.Session.MarkVisibility.Invisible);

                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Note");

                string[] text1 = new string[noteAry.Count];

                for (int i = 0; i < noteAry.Count;i++ )
                {
                    text1[i] = noteAry[i];
                }

                draftingNoteBuilder1.Text.TextBlock.SetText(text1);

                theSession.SetUndoMarkName(markId4, "Note - Text Input");

                theSession.SetUndoMarkVisibility(markId4, null, NXOpen.Session.MarkVisibility.Visible);

                theSession.SetUndoMarkVisibility(markId1, null, NXOpen.Session.MarkVisibility.Invisible);

                NXOpen.Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Note");

                NXOpen.Session.UndoMarkId markId6;
                markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Note");

                NXObject nXObject1;
                nXObject1 = draftingNoteBuilder1.Commit();

                theSession.DeleteUndoMark(markId6, null);

                theSession.SetUndoMarkName(markId1, "Note");

                draftingNoteBuilder1.Destroy();

                theSession.DeleteUndoMark(markId5, null);

                theSession.SetUndoMarkVisibility(markId1, null, NXOpen.Session.MarkVisibility.Visible);

                theSession.DeleteUndoMark(markId4, null);

                theSession.DeleteUndoMark(markId3, null);
            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        
       //刀具-精銑面對應
        public bool faceToolMapping(string newTaskFolder)
        {
            Dictionary<string, string> faceToolMapDic = new Dictionary<string, string>();
            try
            {
                //刀具精銑面配對解析方法一：抓取operation資訊
                //1. 獲得精加工所有operation
                Part dispPart = theSession.Parts.Display;
                Part workPart = theSession.Parts.Work;
                
                List<CaxAsm.CompPart> sCompPart = new List<CaxAsm.CompPart>();
                CaxAsm.CimAsmCompPart sCimAsmCompPart = new CaxAsm.CimAsmCompPart();
                CaxAsm.GetAllAsmCompStruct(out sCompPart, out sCimAsmCompPart);

                OperationCollection Operations = dispPart.CAMSetup.CAMOperationCollection;
                Operation[] OperationAry = Operations.ToArray();
                List<Operation> finishOperationList = new List<Operation>();
                foreach (Operation oper in OperationAry)
                {
                    //只需要MILL_AREA_FINISH下的operation
                    NCGroup ncg = oper.GetParent(CAMSetup.View.Geometry);
                    string ncgGeomName;
                    theUfSession.Obj.AskName(ncg.Tag, out ncgGeomName);
                    if (ncgGeomName == "MILL_AREA_FINISH")
                    {
                        finishOperationList.Add(oper);
                    }
                }

                if (finishOperationList.Count == 0)
                {
                    foreach (Operation oper in OperationAry)
                    {
                        //找不到MILL_AREA_FINISH，改成找MILL_AREA_SIMI_FINISH下的operation
                        NCGroup ncg = oper.GetParent(CAMSetup.View.Geometry);
                        string ncgGeomName;
                        theUfSession.Obj.AskName(ncg.Tag, out ncgGeomName);
                        if (ncgGeomName == "MILL_AREA_SIMI_FINISH")
                        {
                            finishOperationList.Add(oper);
                        }
                    }
                }


                //2. 輪詢operation，獲得operation資訊
                for (int i = 0; i < finishOperationList.Count; i++)
                {
                    //   2.1 獲得operation刀具名稱
                    Tag cutterGroupTag;
                    theUfSession.Oper.AskCutterGroup(finishOperationList[i].Tag, out cutterGroupTag);
                    string cutterGroupName;
                    theUfSession.Obj.AskName(cutterGroupTag, out cutterGroupName);

                    //   2.2 獲得operation加工面
                    List<Face> cutAreaFaceList = new List<Face>();
                    NXOpen.CAM.FeatureGeometry featureGeometry1;
                    try
                    {
                        //找MILL_AREA_FINISH
                        featureGeometry1 = (NXOpen.CAM.FeatureGeometry)workPart.CAMSetup.CAMGroupCollection.FindObject("MILL_AREA_FINISH");
                    }
                    catch (System.Exception ex)
                    {
                        //沒有MILL_AREA_FINISH，找MILL_AREA_SIMI_FINISH
                        featureGeometry1 = (NXOpen.CAM.FeatureGeometry)workPart.CAMSetup.CAMGroupCollection.FindObject("MILL_AREA_SIMI_FINISH");
                    }

                    NXOpen.CAM.MillAreaGeomBuilder millAreaGeomBuilder1;
                    millAreaGeomBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillAreaGeomBuilder(featureGeometry1);
                    GeometrySet geometryList;
                    geometryList = millAreaGeomBuilder1.CutAreaGeometry.GeometryList.FindItem(0);
                    TaggedObject[] ScCollectorObjects = geometryList.ScCollector.GetObjects();
                    foreach (TaggedObject to in ScCollectorObjects)
                    {
                        Face face = (Face)to;
                        //轉型成Occurence
                        Face faceOcc;
                        CaxTransType.FacePrototypeToNXOpenOcc(sCimAsmCompPart.design.comp.Tag, face.Tag, out faceOcc);
                        //檢查是否是設定nc零件上的面
                        if (face.OwningPart == sCimAsmCompPart.nc.part)
                        {
                            //轉換成design零件上的面
                            changeNCFace2DesignFace(sCimAsmCompPart, face, out faceOcc);
                        }
                        if (faceOcc != null)
                        {
                            cutAreaFaceList.Add(faceOcc);
                        }
//                         cutAreaFaceList.Add(faceOcc);
                    }

                    //   2.3 在配對Dictionary內新增Face-Tool資料
                    bool chk;
                    for (int j = 0; j < cutAreaFaceList.Count; j++)
                    {
                        //要抓Face Prototype的handle
                        Face cutFaceProto = (Face)cutAreaFaceList[j].Prototype;
                        string faceHandle = theUfSession.Tag.AskHandleOfTag(cutFaceProto.Tag);
                        string stringValue;
                        //檢查dictionary裡是否有face handle
                        chk = faceToolMapDic.TryGetValue(faceHandle, out stringValue);
                        if (chk)
                        {
                            //20150225老大指示：只留下最後一把刀
                            faceToolMapDic[faceHandle] = cutterGroupName;

                            /*
                            //已存在face handle
                            if (stringValue.Contains(cutterGroupName))
                            {
                                //已存在刀具 不新增
                                continue;
                            }
                            else
                            {
                                stringValue = stringValue + "," + cutterGroupName;
                                faceToolMapDic[faceHandle] = stringValue;
                            }
                            */

                        }
                        else
                        {
                            //新增
                            faceToolMapDic.Add(faceHandle, cutterGroupName);
                        }
                    }
                }
                //3. 輸出JSON格式
                //取得輸出物件
                FaceToolMapping cFaceToolMapping = new FaceToolMapping();
                cFaceToolMapping.FaceToolMapList = new List<FaceToolMap>();

                cFaceToolMapping.MOLD_NO = sMesDatData.MOLD_NO;
                cFaceToolMapping.DES_VER_NO = sMesDatData.DES_VER_NO;
                cFaceToolMapping.WORK_NO = sMesDatData.WORK_NO;
                cFaceToolMapping.PART_NO = sMesDatData.PART_NO;
                cFaceToolMapping.MFC_NO = sMesDatData.MFC_NO;
                cFaceToolMapping.MFC_TASK_NO = sMesDatData.MFC_TASK_NO;
                cFaceToolMapping.MAC_MODEL = sMesDatData.MAC_MODAL;
                cFaceToolMapping.TASK_NO = sMesDatData.TASK_NO;
                cFaceToolMapping.TASK_SRNO = sMesDatData.TASK_SRNO;

                //List<FaceToolMap> faceToolMapList = new List<FaceToolMap>();
                foreach (KeyValuePair<string, string> kvp in faceToolMapDic)
                {
                    FaceToolMap cFaceToolMap = new FaceToolMap();
                    cFaceToolMap.finishTool = new List<string>();
                    cFaceToolMap.finishFace = kvp.Key;
                    string[] toolAry = kvp.Value.Split(',');
                    foreach (string str in toolAry)
                    {
                        cFaceToolMap.finishTool.Add(str);
                    }
                    cFaceToolMapping.FaceToolMapList.Add(cFaceToolMap);
                }

                //設定輸出路徑
                string jsonPath = string.Format(@"{0}\FaceToolMapping.dat", newTaskFolder);
                if (!CaxFile.WriteJsonFileData(jsonPath, cFaceToolMapping))
                {
                    UI.GetUI().NXMessageBox.Show("haha", NXMessageBox.DialogType.Warning, "json failed");
                }


            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool faceToolMapping2(string newTaskFolder)
        {
            Dictionary<string, string> faceToolMapDic = new Dictionary<string, string>();
            try
            {
                //刀具精銑面配對解析方法二：以刀具路徑與精銑面幾何關係判斷
                //1. 獲得精加工所有operation
                Part dispPart = theSession.Parts.Display;
                Part workPart = theSession.Parts.Work;

                List<CaxAsm.CompPart> sCompPart = new List<CaxAsm.CompPart>();
                CaxAsm.CimAsmCompPart sCimAsmCompPart = new CaxAsm.CimAsmCompPart();
                CaxAsm.GetAllAsmCompStruct(out sCompPart, out sCimAsmCompPart);

                OperationCollection Operations = dispPart.CAMSetup.CAMOperationCollection;
                Operation[] OperationAry = Operations.ToArray();
                List<Operation> finishOperationList = new List<Operation>();
                foreach (Operation oper in OperationAry)
                {
                    //只需要MILL_AREA_FINISH下的operation
                    NCGroup ncg = oper.GetParent(CAMSetup.View.Geometry);
                    string ncgGeomName;
                    theUfSession.Obj.AskName(ncg.Tag, out ncgGeomName);
                    if (ncgGeomName == "MILL_AREA_FINISH")
                    {
                        finishOperationList.Add(oper);
                    }
                }
                if (finishOperationList.Count == 0)
                {
                    foreach (Operation oper in OperationAry)
                    {
                        //找不到MILL_AREA_FINISH，改成找MILL_AREA_SIMI_FINISH下的operation
                        NCGroup ncg = oper.GetParent(CAMSetup.View.Geometry);
                        string ncgGeomName;
                        theUfSession.Obj.AskName(ncg.Tag, out ncgGeomName);
                        if (ncgGeomName == "MILL_AREA_SIMI_FINISH")
                        {
                            finishOperationList.Add(oper);
                        }
                    }
                }


                //2. 輪詢operation，獲得operation資訊
                for (int i = 0; i < finishOperationList.Count; i++)
                {
                    //   2.0 獲得operation刀具名稱
                    Tag cutterGroupTag;
                    theUfSession.Oper.AskCutterGroup(finishOperationList[i].Tag, out cutterGroupTag);
                    string cutterGroupName;
                    theUfSession.Obj.AskName(cutterGroupTag, out cutterGroupName);

                    //   2.1 獲得operation加工面
                    NXOpen.CAM.FeatureGeometry featureGeometry1;
                    featureGeometry1 = (NXOpen.CAM.FeatureGeometry)finishOperationList[i].GetParent(CAMSetup.View.Geometry);
                    //featureGeometry1 = (NXOpen.CAM.FeatureGeometry)workPart.CAMSetup.CAMGroupCollection.FindObject("MILL_AREA_FINISH");
                    NXOpen.CAM.MillAreaGeomBuilder millAreaGeomBuilder1;
                    millAreaGeomBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillAreaGeomBuilder(featureGeometry1);
                    GeometrySet geometryList;
                    geometryList = millAreaGeomBuilder1.CutAreaGeometry.GeometryList.FindItem(0);
                    //這裡是face的prototype
                    TaggedObject[] ScCollectorObjects = geometryList.ScCollector.GetObjects();
                    List<Face> cutAreaFaceList = new List<Face>();
                    foreach (TaggedObject to in ScCollectorObjects)
                    {
                        Face face = (Face)to;
                        //轉型成Occurence
                        Face faceOcc;
                        CaxTransType.FacePrototypeToNXOpenOcc(sCimAsmCompPart.design.comp.Tag, face.Tag, out faceOcc);
                        //檢查是否是設定nc零件上的面
                        if (face.OwningPart == sCimAsmCompPart.nc.part)
                        {
                            //轉換成design零件上的面
                            changeNCFace2DesignFace(sCimAsmCompPart, face, out faceOcc);
                        }
                        if (faceOcc != null)
                        {
                            cutAreaFaceList.Add(faceOcc);
                        }
                    }

                    //   2.2 獲取刀具路徑(CLSF)
                    NXOpen.CAM.CAMObject[] objects1 = new NXOpen.CAM.CAMObject[1];
                    objects1[0] = finishOperationList[i];
                    workPart.CAMSetup.ListToolPath(objects1);

                    //CLSF產生路徑
                    string displayPartDir = Path.GetDirectoryName(theSession.Parts.Display.FullPath);
                    string operationName;
                    theUfSession.Oper.AskNameFromTag(finishOperationList[i].Tag, out operationName);
                    string CLSFpath = string.Format(@"{0}\{1}.txt", displayPartDir, operationName);
                    theUfSession.Ui.SaveListingWindow(CLSFpath);
                    theUfSession.Ui.ExitListingWindow();

                    //   2.3 解析刀具路徑獲得點座標
                    List<double[]> pointPosList = new List<double[]>();
                    //double pointMaxDist = 1.0;
                    //double pointMinDist = 0.5;
                    bool isRealCut = false;
                    string readLine = "";
                    StreamReader sr = new StreamReader(CLSFpath);
                    while (!sr.EndOfStream)
                    {
                        readLine = sr.ReadLine();

                        //判斷是否進入切削路徑
                        if (!isRealCut)
                        {
                            if (readLine.Contains("FEDRAT"))
                            {
                                isRealCut = true;
                                continue;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (readLine.Contains("GOTO"))
                        {
                            //刪掉"GOTO/"
                            string pointPosStr = readLine.Substring(5);
                            string[] pointPosStrSplit = pointPosStr.Split(',');
                            double[] pointPos = new double[3];
                            for (int s = 0; s < 3; s++)
                            {
                                pointPos[s] = Convert.ToDouble(pointPosStrSplit[s]);
                            }

                            if (pointPosList.Count == 0)
                            {
                                //第一點
                                pointPosList.Add(pointPos);
                            }
                            else
                            {
                                //第二點開始，與前一點比較距離，大於pointMaxDist要補間格點
                                double[] lastPointPos = pointPosList[pointPosList.Count - 1];
                                double pointDist = Math.Sqrt(
                                    Math.Pow(pointPos[0] - lastPointPos[0], 2) +
                                    Math.Pow(pointPos[1] - lastPointPos[1], 2) +
                                    Math.Pow(pointPos[2] - lastPointPos[2], 2));

                                if (pointDist >= pointMaxDist)
                                {
                                    //補點

                                    //間隔數: 
                                    int stepNum = Convert.ToInt32(Math.Floor(pointDist / pointMaxDist));
                                    //int addPointNum = Convert.ToInt32(Math.Floor(pointDist / pointMinDist));

                                    //補點數: 比間隔少1 
                                    for (int n = 1; n < stepNum; n++)
                                    {
                                        double ratio = Convert.ToDouble(n) / Convert.ToDouble(stepNum);

                                        double[] addPoint = new double[3];

                                        addPoint[0] = lastPointPos[0] + (pointPos[0] - lastPointPos[0]) * ratio;
                                        addPoint[1] = lastPointPos[1] + (pointPos[1] - lastPointPos[1]) * ratio;
                                        addPoint[2] = lastPointPos[2] + (pointPos[2] - lastPointPos[2]) * ratio;

                                        pointPosList.Add(addPoint);
                                    }

                                    pointPosList.Add(pointPos);
                                }
                                else if (pointDist <= pointMinDist)
                                {
                                    continue;
                                }
                                else
                                {
                                    pointPosList.Add(pointPos);
                                }


                            }
                            //Tag pointTag = Tag.Null;
                            //theUfSession.Curve.CreatePoint(pointPos, out pointTag);
                            //CaxLog.ShowListingWindow(pospos);
                        }
                    }

                    //   2.4 輪詢刀具路徑各點，配對刀具名稱與加工面
                    for (int ppl = 0; ppl < pointPosList.Count; ppl++)
                    {
                        //以座標建立點物件
                        NXOpen.Point pathPoint = null;
                        Point3d pathPointCoord = new Point3d(pointPosList[ppl][0], pointPosList[ppl][1], pointPosList[ppl][2]);
                        //PointCollection aa = new PointCollection();
                        pathPoint = workPart.Points.CreatePoint(pathPointCoord);
                        //pathPoint = aa.CreatePoint(pathPointCoord);
                        //pathPoint.SetCoordinates(pathPointCoord);

                        NXOpen.Point testPNT = null;
                        Point3d testPNTCoord = new Point3d(0, 0, 0);
                        testPNT = workPart.Points.CreatePoint(testPNTCoord);

                        //                     Tag pointTag = Tag.Null;
                        //                     theUfSession.Curve.CreatePoint(pointPosList[ppl], out pointTag);
                        //                     Point pathPoint = (Point)NXObjectManager.Get(pointTag);

                        //      2.4.1 輪詢各加工面
                        Face cutFace = cutAreaFaceList[0];
                        for (int caf = 0; caf < cutAreaFaceList.Count; caf++)
                        {

                            //      2.4.2 計算點到加工面距離
                            double[] guess1 = new double[3];
                            double[] guess2 = new double[3];
                            double[] pt_on_obj1 = new double[3];
                            double[] pt_on_obj2 = new double[3];

                            double min_dist;
                            theUfSession.Modl.AskMinimumDist(pathPoint.Tag, cutAreaFaceList[caf].Tag, 0, guess1, 0, guess2, out min_dist, pt_on_obj1, pt_on_obj2);

                            double min_dist2;
                            theUfSession.Modl.AskMinimumDist(pathPoint.Tag, cutFace.Tag, 0, guess1, 0, guess2, out min_dist2, pt_on_obj1, pt_on_obj2);

                            if (min_dist <= min_dist2)
                            {
                                cutFace = cutAreaFaceList[caf];
                            }

                        }

                        //      2.4.3 取得最近距離的面，加入配對Dictionary
                        //檢查dictionary裡是否有face handle
                        bool chk;
                        //要抓Face Prototype的handle
                        Face cutFaceProto = (Face)cutFace.Prototype;
                        string faceHandle = theUfSession.Tag.AskHandleOfTag(cutFaceProto.Tag);
                        string stringValue;
                        chk = faceToolMapDic.TryGetValue(faceHandle, out stringValue);
                        if (chk)
                        {
                            //20150225老大指示：只留下最後一把刀
                            faceToolMapDic[faceHandle] = cutterGroupName;


                            /*
                            //已存在face handle
                            if (stringValue.Contains(cutterGroupName))
                            {
                                //已有此配對，不用新增
                            }
                            else
                            {
                                //此面沒有此刀配對，須新增
                                stringValue = stringValue + "," + cutterGroupName;
                                faceToolMapDic[faceHandle] = stringValue;
                            }
                            */

                        }
                        else
                        {

                            //新增
                            faceToolMapDic.Add(faceHandle, cutterGroupName);
                        }
                    }
                }

                //3. 輸出JSON格式
                //取得輸出物件
                FaceToolMapping cFaceToolMapping = new FaceToolMapping();
                cFaceToolMapping.FaceToolMapList = new List<FaceToolMap>();

                cFaceToolMapping.MOLD_NO = sMesDatData.MOLD_NO;
                cFaceToolMapping.DES_VER_NO = sMesDatData.DES_VER_NO;
                cFaceToolMapping.WORK_NO = sMesDatData.WORK_NO;
                cFaceToolMapping.PART_NO = sMesDatData.PART_NO;
                cFaceToolMapping.MFC_NO = sMesDatData.MFC_NO;
                cFaceToolMapping.MFC_TASK_NO = sMesDatData.MFC_TASK_NO;
                cFaceToolMapping.MAC_MODEL = sMesDatData.MAC_MODAL;
                cFaceToolMapping.TASK_NO = sMesDatData.TASK_NO;
                cFaceToolMapping.TASK_SRNO = sMesDatData.TASK_SRNO;

                //List<FaceToolMap> faceToolMapList = new List<FaceToolMap>();
                foreach (KeyValuePair<string, string> kvp in faceToolMapDic)
                {
                    FaceToolMap cFaceToolMap = new FaceToolMap();
                    cFaceToolMap.finishTool = new List<string>();
                    cFaceToolMap.finishFace = kvp.Key;
                    string[] toolAry = kvp.Value.Split(',');
                    foreach (string str in toolAry)
                    {
                        cFaceToolMap.finishTool.Add(str);
                    }
                    cFaceToolMapping.FaceToolMapList.Add(cFaceToolMap);
                }

                //設定輸出路徑
                string jsonPath = string.Format(@"{0}\FaceToolMapping.dat", newTaskFolder);
                if (!CaxFile.WriteJsonFileData(jsonPath, cFaceToolMapping))
                {
                    UI.GetUI().NXMessageBox.Show("haha", NXMessageBox.DialogType.Warning, "json failed");
                }

            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow(ex.Message);
                return false;
            }
            return true;
        }

        public bool faceToolMapping0(string newTaskFolder)
        {
            try
            {
                //輸出空的檔案
                //取得輸出物件
                FaceToolMapping cFaceToolMapping = new FaceToolMapping();
                cFaceToolMapping.FaceToolMapList = new List<FaceToolMap>();

                cFaceToolMapping.MOLD_NO = sMesDatData.MOLD_NO;
                cFaceToolMapping.DES_VER_NO = sMesDatData.DES_VER_NO;
                cFaceToolMapping.WORK_NO = sMesDatData.WORK_NO;
                cFaceToolMapping.PART_NO = sMesDatData.PART_NO;
                cFaceToolMapping.MFC_NO = sMesDatData.MFC_NO;
                cFaceToolMapping.MFC_TASK_NO = sMesDatData.MFC_TASK_NO;
                cFaceToolMapping.MAC_MODEL = sMesDatData.MAC_MODAL;
                cFaceToolMapping.TASK_NO = sMesDatData.TASK_NO;
                cFaceToolMapping.TASK_SRNO = sMesDatData.TASK_SRNO;

                //設定輸出路徑
                string jsonPath = string.Format(@"{0}\FaceToolMapping.dat", newTaskFolder);
                if (!CaxFile.WriteJsonFileData(jsonPath, cFaceToolMapping))
                {
                    UI.GetUI().NXMessageBox.Show("haha", NXMessageBox.DialogType.Warning, "json failed");
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool changeNCFace2DesignFace(CaxAsm.CimAsmCompPart sCimAsmCompPart, List<Face> NCFaceList, out List<Face> designFaceList)
        {
            designFaceList = new List<Face>();
            try
            {
                Body designBody;
                CaxPart.GetLayerBody(sCimAsmCompPart.design.comp, out designBody);

                Tag[] bodyTagAry = new Tag[0];
                bodyTagAry[0] = designBody.Tag;

                double[] transform = { 1.0,0.0,0.0,0.0,
                                                    0.0,1.0,0.0,0.0,
                                                    0.0,0.0,1.0,0.0,
                                                    0.0,0.0,0.0,1.0 };
                int num_result;
                UFModl.RayHitPointInfo[] ray_hit_list;
                UFModl.RayHitPointInfo ray_hit;

                foreach (Face ncface in NCFaceList)
                {
                    CaxGeom.FaceData sFaceData;
                    CaxGeom.GetFaceData(ncface.Tag, out sFaceData);
                    double[] ray_origin = sFaceData.point;
                    double[] ray_dir = { sFaceData.dir[0] * -1, sFaceData.dir[1] * -1, sFaceData.dir[2] * -1 };

                    theUfSession.Modl.TraceARay(1, bodyTagAry, ray_origin, ray_dir, transform, 0, out num_result, out ray_hit_list);

                    Tag designFaceProtoTag = ray_hit_list[0].hit_face;
                    Face designFaceOcc;
                    CaxTransType.FacePrototypeToNXOpenOcc(sCimAsmCompPart.design.comp.Tag, designFaceProtoTag, out designFaceOcc);

                    designFaceList.Add(designFaceOcc);
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool changeNCFace2DesignFace(CaxAsm.CimAsmCompPart sCimAsmCompPart, Face NCFaceProto, out Face designFaceOcc)
        {
            designFaceOcc = null;
            try
            {
                Body designBody;
                CaxPart.GetLayerBody(sCimAsmCompPart.design.comp, out designBody);

                Tag[] bodyTagAry = new Tag[1];
                bodyTagAry[0] = designBody.Tag;

                double[] transform = { 1.0,0.0,0.0,0.0,
                                                    0.0,1.0,0.0,0.0,
                                                    0.0,0.0,1.0,0.0,
                                                    0.0,0.0,0.0,1.0 };
                int num_result;
                UFModl.RayHitPointInfo[] ray_hit_list;
                UFModl.RayHitPointInfo ray_hit;

                Face NCFaceOcc;
                CaxTransType.FacePrototypeToNXOpenOcc(sCimAsmCompPart.nc.comp.Tag, NCFaceProto.Tag, out NCFaceOcc);
                CaxGeom.FaceData sFaceData;
                CaxGeom.GetFaceData(NCFaceOcc.Tag, out sFaceData);

                int norchk;
                NXOpen.UF.UFSession.GetUFSession().Modl.AskPointContainment(sFaceData.point, NCFaceOcc.Tag, out norchk);

                if ((sFaceData.dir[0] <= 0.01 && sFaceData.dir[1] <= 0.01 && sFaceData.dir[2] <= 0.01) || (norchk != 1))
                {
                    findFacePntDir(sCimAsmCompPart.nc.part, NCFaceOcc, out sFaceData.point, out sFaceData.dir);
                }
                double[] ray_origin = sFaceData.point;
                double[] ray_dir = { sFaceData.dir[0] * -1, sFaceData.dir[1] * -1, sFaceData.dir[2] * -1 };
                try
                {
                    theUfSession.Modl.TraceARay(1, bodyTagAry, ray_origin, ray_dir, transform, 0, out num_result, out ray_hit_list);
                }
                catch (System.Exception ex)
                {
                    NCFaceOcc.Highlight();
                    return false;
                }
                //theUfSession.Modl.TraceARay(1, bodyTagAry, ray_origin, ray_dir, transform, 0, out num_result, out ray_hit_list);

                Tag designFaceProtoTag = ray_hit_list[0].hit_face;

                CaxTransType.FacePrototypeToNXOpenOcc(sCimAsmCompPart.design.comp.Tag, designFaceProtoTag, out designFaceOcc);




            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow(ex.Message);
                return false;
            }
            return true;
        }

        public static bool findFacePntDir(Part ownerPart, Face faceOcc, out double[] pointOnFace, out double[] unitNormDir)
        {
            pointOnFace = new double[3];
            unitNormDir = new double[3];
            try
            {
                bool isFindPointOnFace = false;

                while (!isFindPointOnFace)
                {
                    Random rdn1 = new Random(Guid.NewGuid().GetHashCode());
                    Random rdn2 = new Random(Guid.NewGuid().GetHashCode());
                    double u = rdn1.NextDouble();
                    double v = rdn2.NextDouble();

                    isFindPointOnFace = create_cmm_point_on_face_by_uv(ownerPart, faceOcc, u, v, out pointOnFace);
                    if (isFindPointOnFace)
                    {
                        get_on_face_point_unit_norm(pointOnFace, faceOcc, out unitNormDir);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool create_cmm_point_on_face_by_uv(Part owner_part, Face faceOcc, double u, double v, out double[] point3d)
        {
            point3d = new double[3];

            try
            {

                Expression expressionU;
                expressionU = owner_part.Expressions.CreateSystemExpressionWithUnits(string.Format("{0}", u), null);
                Expression expressionV;
                expressionV = owner_part.Expressions.CreateSystemExpressionWithUnits(string.Format("{0}", v), null);
                Scalar scalarU;
                scalarU = owner_part.Scalars.CreateScalarExpression(expressionU, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);
                Scalar scalarV;
                scalarV = owner_part.Scalars.CreateScalarExpression(expressionV, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.WithinModeling);
                NXOpen.Point point;
                point = owner_part.Points.CreatePoint(faceOcc, scalarU, scalarV, NXOpen.SmartObject.UpdateOption.WithinModeling);
                point3d = new double[3] { point.Coordinates.X, point.Coordinates.Y, point.Coordinates.Z };
                int norchk;
                NXOpen.UF.UFSession.GetUFSession().Modl.AskPointContainment(point3d, faceOcc.Tag, out norchk);

                if (norchk != 1)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool get_on_face_point_unit_norm(double[] point3d, Face faceOcc, out double[] unit_norm)
        {
            unit_norm = new double[3];
            try
            {
                double[] para = { 0, 0 };
                double[] facept = { 0, 0, 0 };
                NXOpen.UF.UFSession.GetUFSession().Modl.AskFaceParm(faceOcc.Tag, point3d, para, facept);
                double[] dummy = { 0, 0, 0 };
                double[] radil = new double[2];
                NXOpen.UF.UFSession.GetUFSession().Modl.AskFaceProps(faceOcc.Tag, para, dummy, dummy, dummy, dummy, dummy, unit_norm, radil);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool ReadMappingConfig(string jsonPath, out FaceToolMappingConfig cMappingConfig)
        {
            cMappingConfig = new FaceToolMappingConfig();

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
                cMappingConfig = JsonConvert.DeserializeObject<FaceToolMappingConfig>(jsonText);
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


        // 轉faceted body to stl檔  20150625 Stewart
        public static void export_solids_to_stl(NXOpen.Assemblies.Component theComp, String targetFile)
        {
            Part thePart = (Part)theComp.Prototype;
            thePart.LoadFully();

            // Open the STL binary file

            FileStream aFileStream = default(FileStream);
            aFileStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(aFileStream);

            // Write the header
            string headerString = "Created by NXOpen";
            byte[] btText = System.Text.Encoding.UTF8.GetBytes(headerString.PadRight(80, ' '));
            bw.Write(btText);

            // Export all faceted bodies with current WCS
            NXOpen.Facet.FacetedBodyCollection fbodies = thePart.FacetedBodies;


            // Count all facets in all faceted bodies for binary STL
            int num_all_facets = 0;

            NXOpen.Facet.FacetedBody[] fbAry = fbodies.ToArray();
            // 20150709 轉成occurance!!
            for (int i = 0; i < fbAry.Length; i++)
            {
                Tag fbOccTag = theUfSession.Assem.FindOccurrence(theComp.Tag, fbAry[i].Tag);
                fbAry[i] = (NXOpen.Facet.FacetedBody)NXObjectManager.Get(fbOccTag);
            }
            // count
            for (int i = 0; i < fbAry.Length; i++)
            {
                int num_facets = 0;
                theUfSession.Facet.AskNFacetsInModel(fbAry[i].Tag, out num_facets);
                num_all_facets = num_all_facets + num_facets;
            }
            bw.Write(num_all_facets);
            // write
            for (int i = 0; i < fbAry.Length; i++)
            {
                NXOpen.Facet.FacetedBody fbd = fbAry[i];
                int facet_id = UFConstants.UF_FACET_NULL_FACET_ID;
                theUfSession.Facet.CycleFacets(fbd.Tag, ref facet_id);

                while (facet_id != UFConstants.UF_FACET_NULL_FACET_ID)
                {
                    double[] plane_normal = new double[3];
                    double d_coefficient = 0;
                    theUfSession.Facet.AskPlaneEquation(fbd.Tag, facet_id, plane_normal, out d_coefficient);
                    bw.Write(Convert.ToSingle(plane_normal[0]));
                    bw.Write(Convert.ToSingle(plane_normal[1]));
                    bw.Write(Convert.ToSingle(plane_normal[2]));

                    int num_vertices = 0;
                    theUfSession.Facet.AskNumVertsInFacet(fbd.Tag, facet_id, out num_vertices);
                    double[,] vertices = new double[num_vertices, 3];

                    theUfSession.Facet.AskVerticesOfFacet(fbd.Tag, facet_id, out num_vertices, vertices);
                    for (int ii = 0; ii <= num_vertices - 1; ii++)
                    {
                        bw.Write(Convert.ToSingle(vertices[ii, 0]));
                        bw.Write(Convert.ToSingle(vertices[ii, 1]));
                        bw.Write(Convert.ToSingle(vertices[ii, 2]));
                    }

                    short abc = 0;
                    //attribute_byte_count
                    bw.Write(abc);

                    theUfSession.Facet.CycleFacets(fbd.Tag, ref facet_id);
                }
            }
            // Close the file
            aFileStream.Close();

        }

        // 20160106 加入判斷刀具切削路徑是否異常
        public static bool CheckToolPath(List<EDPART> MesEdParts, string exportETableStr)
        {
            try
            {
                Cam2MesData sCam2MesData;
                string tempStr = exportETableStr;
                CaxLog.ShowListingWindow(exportETableStr.Contains("[\n\t\t\t\t[").ToString());
                CaxLog.ShowListingWindow(exportETableStr.Contains("]\n\t\t\t]").ToString());
                exportETableStr = tempStr.Replace("[\n\t\t\t\t[", "[");
                tempStr = exportETableStr;
                exportETableStr = tempStr.Replace("]\n\t\t\t]", "]");
                sCam2MesData = Newtonsoft.Json.JsonConvert.DeserializeObject<Cam2MesData>(exportETableStr);
                CaxLog.ShowListingWindow(sCam2MesData.programs.Count.ToString());
                for (int j = 0; j < sCam2MesData.programs.Count; j++)
                {
                    Program tempProgramClass = sCam2MesData.programs[j];
                    CaxLog.ShowListingWindow("RADIUS = " + tempProgramClass.data.Find(s => s.name.ToUpper() == "TOOL_RADIUS").value);
                    CaxLog.ShowListingWindow("PART STOCK = " + tempProgramClass.tool_path.Find(s => s.name.ToUpper() == "XY_STOCK").value);
                    CaxLog.ShowListingWindow("WALL STOCK = " + tempProgramClass.tool_path.Find(s => s.name.ToUpper() == "Z_STOCK").value);
                    CaxLog.ShowListingWindow("");
                    /*
                     * ALL OPERATION TYPES
                     * o Face Milling
                     * x Cavity Milling  
                     * x Z-Level Milling
                     * x Fixed-axis Surface Contouring
                     * o Planar Milling
                     * o Volume Based 2.5D Milling
                     */
                }
                for (int i = 0; i < MesEdParts.Count; i++)
                {
                    double gap = MesEdParts[i].DISCHARGE_GAP;

                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }


        #region not used functions

        public static bool PlotDraftingPNG(string drawingSheetName)
        {
            string exportPngPath = "";

            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: File->Plot...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

                PlotBuilder plotBuilder1;
                plotBuilder1 = workPart.PlotManager.CreatePlotBuilder();

                plotBuilder1.Copies = 1;

                plotBuilder1.Tolerance = 0.001;

                plotBuilder1.RasterImages = true;

                plotBuilder1.ImageResolution = NXOpen.PlotBuilder.ImageResolutionOption.High;

                plotBuilder1.Units = NXOpen.PlotBuilder.UnitsOption.English;

                plotBuilder1.XDisplay = NXOpen.PlotBuilder.XdisplayOption.Right;

                plotBuilder1.XOffset = 3.8;

                plotBuilder1.CharacterSize = 1.52;

                plotBuilder1.Rotation = NXOpen.PlotBuilder.RotationOption.Degree90;

                plotBuilder1.JobName = Path.GetFileNameWithoutExtension(displayPart.FullPath);

                theSession.SetUndoMarkName(markId1, "##04Plot Dialog");

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "##04Plot");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "##04Plot");

                NXObject[] sheets1 = new NXObject[1];
                NXOpen.Drawings.DrawingSheet drawingSheet1 = (NXOpen.Drawings.DrawingSheet)workPart.DrawingSheets.FindObject(drawingSheetName);
                if (drawingSheet1 == null)
                {
                    CaxLog.ShowListingWindow("工程圖尚未建立 : " + drawingSheetName);
                    return false;
                }
                sheets1[0] = drawingSheet1;
                plotBuilder1.SourceBuilder.SetSheets(sheets1);

                plotBuilder1.PlotterText = "PNG";

                plotBuilder1.ProfileText = "<System Profile>";

                plotBuilder1.ColorsWidthsBuilder.Colors = NXOpen.PlotColorsWidthsBuilder.Color.BlackOnWhite;

                plotBuilder1.ColorsWidthsBuilder.Widths = NXOpen.PlotColorsWidthsBuilder.Width.StandardWidths;

                exportPngPath = string.Format(@"{0}\{1}.png", Path.GetDirectoryName(displayPart.FullPath), drawingSheetName);

                string[] filenames1 = new string[1];
                filenames1[0] = exportPngPath;
                plotBuilder1.SetGraphicFilenames(filenames1);

                string exportPath = string.Format(@"{0}\{1}.cgm", Path.GetDirectoryName(displayPart.FullPath), drawingSheetName);

                string[] filenames2 = new string[1];
                filenames2[0] = exportPath;
                plotBuilder1.SetFilenames(filenames2);

                NXObject nXObject1;
                nXObject1 = plotBuilder1.Commit();

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "##04Plot");

                plotBuilder1.Destroy();

                theSession.DeleteUndoMark(markId1, null);
            }
            catch (System.Exception ex)
            {
                CaxLog.WriteLog(ex.Message);
                return false;
            }
            //CaxLog.ShowListingWindow(exportPngPath);
            if (!System.IO.File.Exists(exportPngPath))
            {
                CaxLog.WriteLog("裝夾圖輸出失敗..." + exportPngPath);
                return false;
            }

            return true;
        }

        public static bool ExportCGM(string drawingSheetName)
        {
            string exportPath = "";

            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: File->Export->CGM...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

                CGMBuilder cGMBuilder1;
                cGMBuilder1 = workPart.PlotManager.CreateCgmBuilder();

                cGMBuilder1.Units = NXOpen.CGMBuilder.UnitsOption.English;

                cGMBuilder1.XDimension = 8.5;

                cGMBuilder1.YDimension = 11.0;

                cGMBuilder1.OutputText = NXOpen.CGMBuilder.OutputTextOption.Polylines;

                cGMBuilder1.VdcCoordinates = NXOpen.CGMBuilder.Vdc.Real;

                cGMBuilder1.RasterImages = true;

                theSession.SetUndoMarkName(markId1, "Export CGM Dialog");

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Export CGM");

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Export CGM");

                NXObject[] sheets1 = new NXObject[1];
                NXOpen.Drawings.DrawingSheet drawingSheet1 = (NXOpen.Drawings.DrawingSheet)workPart.DrawingSheets.FindObject(drawingSheetName);
                if (drawingSheet1 == null)
                {
                    return false;
                }

                sheets1[0] = drawingSheet1;
                cGMBuilder1.SourceBuilder.SetSheets(sheets1);

                exportPath = string.Format(@"{0}\{1}.cgm", Path.GetDirectoryName(displayPart.FullPath), drawingSheetName);

                string[] filenames1 = new string[1];
                filenames1[0] = exportPath;
                cGMBuilder1.SetFilenames(filenames1);

                NXObject nXObject1;
                nXObject1 = cGMBuilder1.Commit();

                theSession.DeleteUndoMark(markId3, null);

                theSession.SetUndoMarkName(markId1, "Export CGM");

                cGMBuilder1.Destroy();

                theSession.DeleteUndoMark(markId1, null);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            if (!System.IO.File.Exists(exportPath))
            {
                CaxLog.WriteLog("裝夾圖輸出失敗..." + exportPath);
                return false;
            }


            return true;
        }

        public static bool ExportCGM2PNG(string drawingSheetName)
        {
            try
            {
                Session theSession = Session.GetSession();
                Part displayPart = theSession.Parts.Display;

                //開始上傳
                Process proc = null;
                try
                {
                    string UgiiBaseDir = Environment.GetEnvironmentVariable("UGII_BASE_DIR");
                    if (!System.IO.Directory.Exists(UgiiBaseDir))
                    {
                        CaxLog.ShowListingWindow("UgiiRootDir : " + UgiiBaseDir);
                        return false;
                    }

                    string targetDir = string.Format(@"{0}\{1}", UgiiBaseDir, "NXPLOT");
                    if (!System.IO.Directory.Exists(targetDir))
                    {
                        CaxLog.ShowListingWindow("targetDir : " + targetDir);
                        return false;
                    }

                    string FileName = string.Format(@"{0}\{1}", targetDir, "nxplot.exe");
                    if (!System.IO.File.Exists(FileName))
                    {
                        CaxLog.ShowListingWindow("FileName : " + FileName);
                        return false;
                    }

                    string exportCgmPath = string.Format(@"{0}\{1}.cgm", Path.GetDirectoryName(displayPart.FullPath), drawingSheetName);
                    if (!System.IO.File.Exists(exportCgmPath))
                    {
                        CaxLog.ShowListingWindow("exportCgmPath : " + exportCgmPath);
                        return false;
                    }

                    string exportPngPath = string.Format(@"{0}\{1}.png", Path.GetDirectoryName(displayPart.FullPath), drawingSheetName);

                    string Arguments = string.Format(@"-input={0} -format={1} -output={2}", exportCgmPath, "PNG", exportPngPath);

                    proc = new Process();
                    proc.StartInfo.WorkingDirectory = targetDir;
                    //proc.StartInfo.FileName = EXPORT_BAT_NAME;
                    proc.StartInfo.FileName = FileName;
                    proc.StartInfo.Arguments = Arguments;
                    //proc.StartInfo.CreateNoWindow = false;


                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                    while (!proc.HasExited)
                    {
                    }
                    proc.WaitForExit(5);
                    proc.Close();
                }
                catch (Exception ex)
                {
                    CaxLog.ShowListingWindow(ex.Message);
                    return false;
                }

            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        public void ExportSetupDrawing()
        {
            /*
            Part disp = theSession.Parts.Display;
            Tag obj = Tag.Null;
            Tag drwSheet = Tag.Null;
            string drawingName;
            string pathBase = Path.GetDirectoryName(disp.FullPath) + @"\" + Path.GetFileNameWithoutExtension(disp.FullPath);

            theUfSession.Obj.CycleObjsInPart(disp.Tag, UFConstants.UF_drawing_type, ref obj);
            while (obj != Tag.Null)
            {
                theUfSession.Obj.AskName(obj, out drawingName);
                if (drawingName.StartsWith("NC_SETUP"))
                {
                    drwSheet = obj;
                    string cgmFileName = pathBase + "_" + drawingName + ".cgm";
                    string jpgFileName = pathBase + "_" + drawingName + ".jpg";

                    ExportDrawing.ToCGM(drwSheet, cgmFileName);
                    ExportDrawing.ToJPG(drwSheet, jpgFileName);
                }

                theUfSession.Obj.CycleObjsInPart(disp.Tag, UFConstants.UF_drawing_type, ref obj);
            }

            if (drwSheet == Tag.Null)
                throw new Exception("Unable to find NC_SETUP drawing sheet!");

            */
        }


        #endregion


    }
}

