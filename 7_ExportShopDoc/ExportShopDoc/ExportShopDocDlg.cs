using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;
using CaxGlobaltek;
using NXOpen.Utilities;
using NXOpen.CAM;
using System.Text.RegularExpressions;
using DevComponents.DotNetBar.SuperGrid;
using System.Data.OleDb;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.IO;

namespace ExportShopDoc
{
    public partial class ExportShopDocDlg : DevComponents.DotNetBar.Office2007Form
    {
        public static Session theSession = Session.GetSession();
        public static UI theUI;
        public static UFSession theUfSession = UFSession.GetUFSession();
        public static Dictionary<string, OperData> DicNCData = new Dictionary<string, OperData>();
        public static GridPanel panel = new GridPanel();
        public static string CurrentNCGroup = "", PartNo = "";
        public static NXOpen.CAM.Operation[] OperationAry = new NXOpen.CAM.Operation[] { };
        public static NXOpen.CAM.NCGroup[] NCGroupAry = new NXOpen.CAM.NCGroup[] { };
        public static Part workPart = theSession.Parts.Work;
        public static Part displayPart = theSession.Parts.Display;
        public static bool status;
        public static METE_Download_Upload_Path cMETE_Download_Upload_Path = new METE_Download_Upload_Path();
        public static string Local_Folder_CAM = "";

        public struct ProgramName
        {
            public string OldOperName { get; set; }
            public string NewOperName { get; set; }
        }

        public struct OperData
        {
            public string OperName { get; set; }
            public string ToolName { get; set; }
            public string HolderDescription { get; set; }
            public string CuttingLength { get; set; }
            public string CuttingTime { get; set; }
            public string ToolFeed { get; set; }
            public string ToolNumber { get; set; }
            public string ToolSpeed { get; set; }
        }

        public struct RowColumn
        {
            public int ToolNumberRow { get; set; }
            public int ToolNumberColumn { get; set; }
            public int ToolNameRow { get; set; }
            public int ToolNameColumn { get; set; }
            public int OperNameRow { get; set; }
            public int OperNameColumn { get; set; }
            public int HolderRow { get; set; }
            public int HolderColumn { get; set; }
            public int CuttingTimeRow { get; set; }
            public int CuttingTimeColumn { get; set; }
            public int ToolFeedRow { get; set; }
            public int ToolFeedColumn { get; set; }
            public int ToolSpeedRow { get; set; }
            public int ToolSpeedColumn { get; set; }
            public int ImgToolRow { get; set; }
            public int ImgToolColumn { get; set; }
            //public int ImgOperRow { get; set; }
            //public int ImgOperColumn { get; set; }
        }

        public struct ImgPosiSize
        {
            public float PosiLeft { get; set; }
            public float PosiTop { get; set; }
            public float ImgWidth { get; set; }
            public float ImgHeight { get; set; }
        }

        public ExportShopDocDlg()
        {
            InitializeComponent();

            //建立panel物件
            panel = superGridProg.PrimaryGrid;

            panel.Columns["拍照"].EditorType = typeof(SetView);

            //取得METEDownload_Upload資料
            status = CaxGetDatData.GetMETEDownload_Upload(out cMETE_Download_Upload_Path);
            if (!status)
            {
                MessageBox.Show("取得METEDownload_Upload失敗");
                return;
            }

            #region 註解中，驗證的資料
            
            /*取得刀具名稱&修改程式名稱
            IntPtr[] b = new IntPtr[] { };
            int a=0;
            foreach (NXOpen.CAM.NCGroup ncGroup in NCGroupAry)
            {
                int type;
                int subtype;
                
                theUfSession.Obj.AskTypeAndSubtype(ncGroup.Tag, out type, out subtype);
                
                if (type == UFConstants.UF_machining_tool_type)
                {
                    CaxLog.ShowListingWindow(ncGroup.Name);
                }
            }
            NXOpen.CAM.Operation[] aaa = displayPart.CAMSetup.CAMOperationCollection.ToArray();//取得operationName
            for (int i = 0; i < aaa.Length; i++)
            {
                aaa[i].SetName(600 + i.ToString());
            }
            */
            

            //this.Close();
//             int count = 0;
//             string[] type_names;
//             theUfSession.Cam.OptAskTypes(out count, out type_names);//取得Create Tool中的Type(count=總數、type_names=參數名稱)
//             CaxLog.ShowListingWindow("count：" + count);
//             for (int i = 0; i < type_names.Length;i++ )
//             {
//                 CaxLog.ShowListingWindow("type_names[i]："+type_names[i].ToString());
//             }
//             int sub_count = 0;
//             string[] subtype_names;
//             UFCam.OptStypeCls subtype_class = UFCam.OptStypeCls.OptStypeClsOper;
//             theUfSession.Cam.OptAskSubtypes(type_names[0], subtype_class, out sub_count, out subtype_names);
//             CaxLog.ShowListingWindow("sub_count：" + sub_count);
//             for (int i = 0; i < subtype_names.Length; i++)
//             {
//                 CaxLog.ShowListingWindow("subtype_names[i]：" + subtype_names[i].ToString());
//             }
//             try
//             {
//                 string a = "";
//                 theUfSession.Part.AskPartName(displayPart.Tag, out a);
//                 CaxLog.ShowListingWindow(a.ToString());
//             }
//             catch (System.Exception ex)
//             {
//                 CaxLog.ShowListingWindow("沒有displayPart.Name");
//             }
//             try
//             {
//                 CaxLog.ShowListingWindow(displayPart.FullPath.ToString());
//             }
//             catch (System.Exception ex)
//             {
//                 CaxLog.ShowListingWindow("沒有displayPart.FullPath");
//             }
//             try
//             {
//                 NXOpen.Tag tagRootPart = NXOpen.Tag.Null;
//                 tagRootPart = theUfSession.Assem.AskRootPartOcc(displayPart.Tag);
//                 CaxLog.ShowListingWindow(displayPart.Tag.ToString());
//                 CaxLog.ShowListingWindow(tagRootPart.ToString());
//             }
//             catch (System.Exception ex)
//             {
//                 CaxLog.ShowListingWindow("沒有displayPart.Tag");
//             }

            #endregion
        }

        private void ExportShopDocDlg_Load(object sender, EventArgs e)
        {
            
            //取得所有GroupAry，用來判斷Group的Type決定是NC、Tool、Geometry
            NCGroupAry = displayPart.CAMSetup.CAMGroupCollection.ToArray();
            //取得所有operationAry
            OperationAry = displayPart.CAMSetup.CAMOperationCollection.ToArray();

            #region test
            /*
            foreach (NXOpen.CAM.NCGroup ncGroup in NCGroupAry)
            {
                int type;
                int subtype;

                theUfSession.Obj.AskTypeAndSubtype(ncGroup.Tag, out type, out subtype);
                
                if (type == UFConstants.UF_machining_tool_type)
                {
                    NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)NXObjectManager.Get(ncGroup.Tag);
                    
                    Tool.Types type1;
                    Tool.Subtypes subtype1;
                    tool1.GetTypeAndSubtype(out type1, out subtype1);
                    if (type1 == Tool.Types.Drill)
                    {
                        NXOpen.CAM.DrillStdToolBuilder drillStdToolBuilder1;
                        drillStdToolBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateDrillStdToolBuilder(tool1);
                        string aaaaa = drillStdToolBuilder1.TlHolderDescription;
                        //CaxLog.ShowListingWindow(aaaaa);
                    }
                    else if (type1 == Tool.Types.Mill)
                    {
                        NXOpen.CAM.MillingToolBuilder drillStdToolBuilder1;
                        drillStdToolBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillToolBuilder(tool1);
                        string aaaaa = drillStdToolBuilder1.TlHolderDescription;
                        //CaxLog.ShowListingWindow(aaaaa);
                    }
                    else if (type1 == Tool.Types.MillForm)
                    {
                        NXOpen.CAM.MillingToolBuilder drillStdToolBuilder1;
                        drillStdToolBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillToolBuilder(tool1);
                        string aaaaa = drillStdToolBuilder1.TlHolderDescription;
                        //CaxLog.ShowListingWindow(aaaaa);
                    }
                }
                else if (type == UFConstants.UF_machining_task_type)
                {
                    //取得NCProgram名稱
                    NXOpen.CAM.NCGroup tool1 = (NXOpen.CAM.NCGroup)NXObjectManager.Get(ncGroup.Tag);

                }



                if (type == UFConstants.UF_machining_tool_type)
                {
                    NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)NXObjectManager.Get(ncGroup.Tag);
                    Tool.Types type1;
                    Tool.Subtypes subtype1;
                    tool1.GetTypeAndSubtype(out type1, out subtype1);
                    
                    //                     for (int i = 0; i < type1.Length; i++)
                    //                     {
                    //                         CaxLog.ShowListingWindow(a[i].Name);
                    //                     }

                    //NXOpen.CAM.MillingToolBuilder drillStdToolBuilder1;
                    //drillStdToolBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillToolBuilder(tool1);
                    //drillStdToolBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateDrillStdToolBuilder(tool1);
                    //string aaaaa = drillStdToolBuilder1.TlHolderDescription;
                    //CaxLog.ShowListingWindow(aaaaa);
                    //drillStdToolBuilder1.TlHolderDescription = "123";//設定或取得Description數值
                    //drillStdToolBuilder1.Commit();
                    //drillStdToolBuilder1.Destroy();
                }
            }
            */
            #endregion

            #region 取得相關資訊，填入DIC
            string ncGroupName = "";
            DicNCData = new Dictionary<string, OperData>();
            foreach (NXOpen.CAM.NCGroup ncGroup in NCGroupAry)
            {
                int type;
                int subtype;
                theUfSession.Obj.AskTypeAndSubtype(ncGroup.Tag, out type, out subtype);

                if (type == UFConstants.UF_machining_task_type)//此處比對是否為Program群組
                {
                    if (!ncGroup.Name.Contains("OP"))
                    {
                        UI.GetUI().NXMessageBox.Show("注意", NXMessageBox.DialogType.Error, "請先手動將Group名稱：" + ncGroup.Name + "，改為正確格式，再重新啟動功能！");
                        this.Close();
                        return;
                    }

                    ncGroupName = ncGroup.Name;
                    
                    foreach (NXOpen.CAM.Operation item in OperationAry)
                    {
                        //取得父層的群組(回傳：NCGroup XXXX)
                        string NCProgramTag = item.GetParent(CAMSetup.View.ProgramOrder).ToString();
                        NCProgramTag = Regex.Replace(NCProgramTag, "[^0-9]", "");
                        if (NCProgramTag == ncGroup.Tag.ToString())
                        {
                            
                            //123

                            bool cheValue;
                            OperData sOperData = new OperData();
                            cheValue = DicNCData.TryGetValue(ncGroup.Name, out sOperData);
                            if (!cheValue)
                            {
                                sOperData.OperName = item.Name;
                                sOperData.ToolName = CaxOper.AskOperToolNameFromTag(item.Tag);
                                sOperData.HolderDescription = CaxOper.AskOperHolderDescription(item);
                                sOperData.CuttingLength = Convert.ToDouble(CaxOper.AskOperCuttingLength(item)).ToString("f3");
                                sOperData.ToolFeed = CaxOper.AskOperToolFeed(item);
                                sOperData.CuttingTime = (Convert.ToDouble(CaxOper.AskOperCuttingTime(item)) * 60).ToString("f3");//因為進給單位mmpm，距離單位mm，將進給的60放來這邊乘
                                sOperData.ToolNumber = "T" + CaxOper.AskOperToolNumber(item);
                                sOperData.ToolSpeed = CaxOper.AskOperToolSpeed(item);
                                DicNCData.Add(ncGroup.Name, sOperData);
                            }
                            else
                            {
                                sOperData.OperName = sOperData.OperName + "," + item.Name;
                                sOperData.ToolName = sOperData.ToolName + "," + CaxOper.AskOperToolNameFromTag(item.Tag);
                                sOperData.HolderDescription = sOperData.HolderDescription + "," + CaxOper.AskOperHolderDescription(item);
                                sOperData.CuttingLength = sOperData.CuttingLength + "," + Convert.ToDouble(CaxOper.AskOperCuttingLength(item)).ToString("f3");
                                sOperData.ToolFeed = sOperData.ToolFeed + "," + CaxOper.AskOperToolFeed(item);
                                sOperData.CuttingTime = sOperData.CuttingTime + "," + (Convert.ToDouble(CaxOper.AskOperCuttingTime(item)) * 60).ToString("f3");//因為進給單位mmpm，距離單位mm，將進給的60放來這邊乘
                                sOperData.ToolNumber = sOperData.ToolNumber + "," + "T" + CaxOper.AskOperToolNumber(item);
                                sOperData.ToolSpeed = sOperData.ToolSpeed + "," + CaxOper.AskOperToolSpeed(item);
                                DicNCData[ncGroup.Name] = sOperData;
                            }
                        }
                    }
                }
                else if (type == UFConstants.UF_machining_tool_type)
                {

                }
            }

            //將DicProgName的key存入程式群組下拉選單中
            foreach (KeyValuePair<string, OperData> kvp in DicNCData)
            {
                comboBoxNCgroup.Items.Add(kvp.Key);
            }

            #endregion

            #region 設定輸出路徑

            //取得總組立名稱與全路徑
            PartNo = Path.GetFileNameWithoutExtension(displayPart.FullPath);
            string PartNoFullPath = Path.GetDirectoryName(displayPart.FullPath);//回傳：IP:\Globaltek\Task\廠商\料號\版次
            string[] splitPartNoFullPath = PartNoFullPath.Split('\\');

            string Local_IP = cMETE_Download_Upload_Path.Local_IP;
            string Local_ShareStr = cMETE_Download_Upload_Path.Local_ShareStr;
            Local_Folder_CAM = cMETE_Download_Upload_Path.Local_Folder_CAM;

            Local_ShareStr = Local_ShareStr.Replace("[Local_IP]", Local_IP);
            Local_ShareStr = Local_ShareStr.Replace("[CusName]", splitPartNoFullPath[3]);
            Local_ShareStr = Local_ShareStr.Replace("[PartNo]", splitPartNoFullPath[4]);
            Local_ShareStr = Local_ShareStr.Replace("[CusRev]", splitPartNoFullPath[5]);
            Local_Folder_CAM = Local_Folder_CAM.Replace("[Local_ShareStr]", Local_ShareStr);
            Local_Folder_CAM = Local_Folder_CAM.Replace("[Oper1]", Regex.Replace(ncGroupName, "[^0-9]", ""));

            //取得資料夾內所有檔案
            string[] FolderFile = System.IO.Directory.GetFileSystemEntries(Local_Folder_CAM, "*.xls");
            //設定輸出路徑與檔名
            OutputPath.Text = string.Format(@"{0}\{1}", Local_Folder_CAM, PartNo + "_" + (FolderFile.Length + 1) + ".xls");

            #endregion
            

            
        }

        private void buttonSelePath_Click(object sender, EventArgs e)
        {
            string selectDir = "";
            CaxPublic.SaveFileDialog(OutputPath.Text, out selectDir);
            OutputPath.Text = selectDir;
        }

        private void comboBoxNCgroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空superGrid資料
            panel.Rows.Clear();
            //取得comboBox資料
            CurrentNCGroup = comboBoxNCgroup.Text;
            //拆群組名稱字串取得製程序(EX：OP210=>210)
            string OperNum = Regex.Replace(CurrentNCGroup, "[^0-9]", "");

            #region 拍Oper刀具路徑圖

            //foreach (NXOpen.CAM.NCGroup ncGroup in NCGroupAry)
            //{
            //    if (CurrentNCGroup == ncGroup.Name)
            //    {
            //        for (int i = 0; i < OperationAry.Length; i++)
            //        {
            //            //取得父層的群組(回傳：NCGroup XXXX)
            //            string NCProgramTag = OperationAry[i].GetParent(CAMSetup.View.ProgramOrder).ToString();
            //            NCProgramTag = Regex.Replace(NCProgramTag, "[^0-9]", "");
            //            NXOpen.CAM.CAMObject[] tempObjToCreateImg = new CAMObject[1];
            //            string ImagePath = "";
            //            if (NCProgramTag == ncGroup.Tag.ToString())
            //            {
            //                tempObjToCreateImg[0] = (NXOpen.CAM.CAMObject)OperationAry[i];
            //                workPart.CAMSetup.ReplayToolPath(tempObjToCreateImg);
            //                workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);
            //                ImagePath = string.Format(@"{0}\{1}", Local_Folder_CAM, OperationAry[i].Name);
            //                theUfSession.Disp.CreateImage(ImagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);
            //            }
            //        }
            //    }
            //}

            #endregion

            #region 填值到SuperGridPanel

            GridRow row = new GridRow();
            foreach (KeyValuePair<string, OperData> kvp in DicNCData)
            {
                if (CurrentNCGroup == kvp.Key)
                {
                    string[] splitOperName = kvp.Value.OperName.Split(',');
                    string[] splitToolName = kvp.Value.ToolName.Split(',');
                    string[] splitHolderDescription = kvp.Value.HolderDescription.Split(',');
                    string[] splitOperCuttingLength = kvp.Value.CuttingLength.Split(',');
                    string[] splitOperToolFeed = kvp.Value.ToolFeed.Split(',');
                    string[] splitOperCuttingTime = kvp.Value.CuttingTime.Split(',');
                    string[] splitOperToolNumber = kvp.Value.ToolNumber.Split(',');
                    string[] splitOperToolSpeed = kvp.Value.ToolSpeed.Split(',');

                    for (int i = 0; i < splitOperName.Length; i++)
                    {
                        int y = i + 1;
                        if (i < 9)
                        {
                            row = new GridRow(splitOperName[i], "O" + OperNum + y, splitOperToolNumber[i], splitToolName[i],
                                splitHolderDescription[i], splitOperCuttingLength[i], splitOperToolFeed[i], splitOperToolSpeed[i], splitOperCuttingTime[i], "拍照");
                        }
                        else
                        {
                            string tempOperNum = (Convert.ToDouble(OperNum) * 0.1).ToString();
                            row = new GridRow(splitOperName[i], "O" + tempOperNum + y, splitOperToolNumber[i], splitToolName[i],
                                splitHolderDescription[i], splitOperCuttingLength[i], splitOperToolFeed[i], splitOperToolSpeed[i], splitOperCuttingTime[i], "拍照");
                        }

                        panel.Rows.Add(row);
                    }
                }
            }

            #endregion
            
        }

        private void ConfirmRename_Click(object sender, EventArgs e)
        {
            string RenameStr = "";

            foreach (NXOpen.CAM.NCGroup ncGroup in NCGroupAry)
            {
                if (CurrentNCGroup == ncGroup.Name)
                {
                    //取得此NCGroup下的所有Oper
                    CAMObject[] OperGroup = ncGroup.GetMembers();

                    for (int i = 0; i < OperGroup.Length;i++ )
                    {
                        RenameStr = panel.GetCell(i, 1).Value.ToString();
                        OperGroup[i].SetName(RenameStr);

                        #region 註解中，驗證資料使用

                        //NXOpen.CAM.Operation abc = (NXOpen.CAM.Operation)OperGroup[i];
                        //string cba;

                        //NXOpen.CAM.CAMObject[] params1 = new NXOpen.CAM.CAMObject[1];
                        //params1[0] = abc;
                        //NXOpen.CAM.ObjectsFeedsBuilder objectsFeedsBuilder1;
                        //objectsFeedsBuilder1 = workPart.CAMSetup.CreateFeedsBuilder(params1);
                        //CaxLog.ShowListingWindow(objectsFeedsBuilder1.FeedsBuilder.SpindleRpmBuilder.Value.ToString());


                        //CaxLog.ShowListingWindow("OperHolderDescription：" + CaxOper.AskOperHolderDescription(abc));

                        //CaxLog.ShowListingWindow("OperToolNameFromTag：" + CaxOper.AskOperToolNameFromTag(abc.Tag));
                        /*
                        NCGroup aaa = abc.GetParent(CAMSetup.View.MachineTool);//由Oper取得刀子
                        NXOpen.CAM.Tool tool01 = (NXOpen.CAM.Tool)NXObjectManager.Get(aaa.Tag);//取得Oper的刀子名稱
                        Tool.Types type;
                        Tool.Subtypes subtype;
                        tool01.GetTypeAndSubtype(out type, out subtype);
                        if (type == Tool.Types.Drill)
                        {
                            NXOpen.CAM.DrillStdToolBuilder ToolBuilder1;
                            ToolBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateDrillStdToolBuilder(tool01);
                            string operHolderDescription = ToolBuilder1.TlHolderDescription;
                            CaxLog.ShowListingWindow("operHolderDescription：" + operHolderDescription);
                        }
                        else if (type == Tool.Types.Mill)
                        {
                            NXOpen.CAM.MillingToolBuilder ToolBuilder1;
                            ToolBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillToolBuilder(tool01);
                            string operHolderDescription = ToolBuilder1.TlHolderDescription;
                            CaxLog.ShowListingWindow("operHolderDescription：" + operHolderDescription);
                        }
                        else if (type == Tool.Types.MillForm)
                        {
                            NXOpen.CAM.MillingToolBuilder ToolBuilder1;
                            ToolBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillToolBuilder(tool01);
                            string operHolderDescription = ToolBuilder1.TlHolderDescription;
                            CaxLog.ShowListingWindow("operHolderDescription：" + operHolderDescription);
                        }
                        */
                        //CaxLog.ShowListingWindow("Oper：" + abc.Name);
                        //CaxLog.ShowListingWindow(abc.Name + ".ToolpathTime：" + abc.GetToolpathTime().ToString());
                        //CaxLog.ShowListingWindow(abc.Name + ".ToolpathLength：" + abc.GetToolpathLength().ToString());
                        //CaxLog.ShowListingWindow(abc.Name + ".ToolpathCuttingTime：" + abc.GetToolpathCuttingTime().ToString());
                        //CaxLog.ShowListingWindow(abc.Name + ".ToolpathCuttingLength：" + abc.GetToolpathCuttingLength().ToString());
                        //CaxLog.ShowListingWindow(abc.Name + ".Feed：" + abc.GetToolpathCuttingLength() / abc.GetToolpathCuttingTime());

                        //CaxLog.ShowListingWindow("-----");
                        //NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)NXObjectManager.Get(ncGroup.Tag);
                        //CaxLog.ShowListingWindow("0220");

                        //CaxOper.AskOperHolderNameFromTag(abc.Tag, out cba);

                        //CaxLog.ShowListingWindow("abc.Type：" + abc.GetType());
                        //CaxLog.ShowListingWindow("cba：" + cba);
                        //CaxLog.ShowListingWindow("abc.GetToolpathCuttingTime：" + abc.GetToolpathCuttingTime());
                        //CaxLog.ShowListingWindow("abc.GetToolpathCuttingLength：" + abc.GetToolpathCuttingLength());
                        //CaxLog.ShowListingWindow("abc.GetToolpathTime：" + abc.GetToolpathTime());
                        //CaxLog.ShowListingWindow("abc.GetToolpathLength：" + abc.GetToolpathLength());


                        //UFOper.MachMode aa ;
                        //theUfSession.Oper.AskMachiningMode(OperGroup[i].Tag, out aa);

                        //Tag bb,ee,ff,hh;
                        //theUfSession.Oper.AskMethodGroup(OperGroup[i].Tag, out bb);
                        //NXOpen.CAM.Method cc = (NXOpen.CAM.Method)NXObjectManager.Get(bb);//取得Oper的加工方法名稱

                        //string test;
                        //theUfSession.Oper.AskNameFromTag(OperGroup[i].Tag, out test);//取得Oper的名稱

                        //int typee;
                        //theUfSession.Oper.AskOperType(OperGroup[i].Tag, out typee);

                        //theUfSession.Oper.AskCutterGroup(OperGroup[i].Tag, out ee);
                        //NXOpen.CAM.Tool dd = (NXOpen.CAM.Tool)NXObjectManager.Get(ee);//取得Oper的刀子名稱
                        //NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)NXObjectManager.Get(NCGroup.Tag);
                        //CaxLog.ShowListingWindow("dd`:" + dd.Name.ToString());
                        //CaxLog.ShowListingWindow("tool1`:" + tool1.Name.ToString());



                        //theUfSession.Oper.AskProgramGroup(OperGroup[i].Tag, out ff);
                        //NXOpen.CAM.NCGroup gg = (NXOpen.CAM.NCGroup)NXObjectManager.Get(ff);//取得Oper的父層名稱

                        //theUfSession.Oper.AskGeomGroup(OperGroup[i].Tag, out hh);
                        //NXOpen.CAM.OrientGeometry ii = (NXOpen.CAM.OrientGeometry)NXObjectManager.Get(hh);//取得Oper的座標系名稱


                        //CaxOper.AskOperProgramNameFromTag(abc.Tag,out cba);
                        //CaxLog.ShowListingWindow(cba.ToString());
                        //                         CaxLog.ShowListingWindow(aa.ToString());
                        //                         CaxLog.ShowListingWindow(bb.ToString());
                        //                         CaxLog.ShowListingWindow(bb.GetType().ToString());
                        //                         CaxLog.ShowListingWindow(cc.Name);
                        //                         CaxLog.ShowListingWindow(test);
                        //                         CaxLog.ShowListingWindow("typee:" + typee.ToString());
                        //                         CaxLog.ShowListingWindow(dd.Name.ToString());
                        //                         CaxLog.ShowListingWindow(gg.Name.ToString());
                        //                         CaxLog.ShowListingWindow(ii.Name.ToString());
                        //CaxLog.ShowListingWindow("*-----");

                        #endregion
                    }
                }
            }

            theUfSession.UiOnt.Refresh();

            #region 重新將更名後的Oper名稱寫回Dic中

            string NewOperName = "";
            for (int i = 0; i < panel.Rows.Count;i++ )
            {
                string tempOperName = panel.GetCell(i, 1).Value.ToString();
                if (i==0)
                {
                    NewOperName = tempOperName;
                }
                else
                {
                    NewOperName = NewOperName + "," + tempOperName;
                }
            }
            
            OperData NewOperData = new OperData();
            DicNCData.TryGetValue(CurrentNCGroup, out NewOperData);
            NewOperData.OperName = NewOperName;
            DicNCData[CurrentNCGroup] = NewOperData;

            #endregion

            /*
            foreach (KeyValuePair<string, string> kvp in DicProgName)
            {
                if (CurrentNCGroup == kvp.Key)
                {
                    CAMObject[] a = NCGroupAry[0].GetMembers();
                    for (int i = 0; i < a.Length;i++ )
                    {
                        CaxLog.ShowListingWindow(a[i].Name);
                    }

                    //for (int i = 0; i < OperationAry.Length; i++)
                    //{
                    //    //取得superGridPanel中更名後的資料
                    //    RenameStr = panel.GetCell(i, 1).Value.ToString();
                    //    OperationAry[i].SetName(RenameStr);
                    //}
                }
            }
            */
        }

        private void ExportExcel_Click(object sender, EventArgs e)
        {
            //拍等角試圖照片
            //workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);
            //string ImagePath = @"C:\Users\Alex_Chiu\Desktop\Trimetric.jpg";
            //theUfSession.Disp.CreateImage(ImagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);

            //檢查PC有無Excel在執行
            bool flag = false;
            foreach (var item in Process.GetProcesses())
            {
                if (item.ProcessName == "EXCEL")
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                CaxLog.ShowListingWindow("請先關閉所有Excel再重新執行輸出，如沒有EXCEL在執行，請開啟工作管理員關閉背景EXCEL");
                return;
            }

            //判斷是否已經指定路徑
            if (OutputPath.Text == "")
            {
                UI.GetUI().NXMessageBox.Show("注意", NXMessageBox.DialogType.Error, "請指定刀具路徑與清單的輸出路徑！");
                return;
            }

            #region 拍OperToolPath圖片

            //取得已經手動拍攝過的OperImg
            string[] FolderImageAry = System.IO.Directory.GetFileSystemEntries(Local_Folder_CAM, "*.jpg");

            //重新取operationAry獲得新名稱
            OperationAry = displayPart.CAMSetup.CAMOperationCollection.ToArray();

            foreach (NXOpen.CAM.NCGroup ncGroup in NCGroupAry)
            {
                if (CurrentNCGroup == ncGroup.Name)
                {
                    for (int i = 0; i < OperationAry.Length; i++)
                    {
                        //取得父層的群組(回傳：NCGroup XXXX)
                        string NCProgramTag = OperationAry[i].GetParent(CAMSetup.View.ProgramOrder).ToString();
                        NCProgramTag = Regex.Replace(NCProgramTag, "[^0-9]", "");
                        NXOpen.CAM.CAMObject[] tempObjToCreateImg = new CAMObject[1];
                        string ImagePath = "";
                        if (NCProgramTag == ncGroup.Tag.ToString())
                        {
                            //判斷是否已經手動拍攝過，如拍攝過就不再拍攝
                            bool checkStatus = false;
                            foreach (string single in FolderImageAry)
                            {
                                if (Path.GetFileNameWithoutExtension(single) == CaxOper.AskOperNameFromTag(OperationAry[i].Tag))
                                {
                                    checkStatus = true;
                                }
                            }

                            if (!checkStatus)
                            {
                                tempObjToCreateImg[0] = (NXOpen.CAM.CAMObject)OperationAry[i];
                                workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);
                                workPart.CAMSetup.ReplayToolPath(tempObjToCreateImg);
                                
                                ImagePath = string.Format(@"{0}\{1}", Local_Folder_CAM, OperationAry[i].Name);
                                theUfSession.Disp.CreateImage(ImagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);
                            }
                        }
                    }
                }
            }

            #endregion


            Excel.ApplicationClass x = new Excel.ApplicationClass();
            Excel.Workbook book = null;
            Excel.Worksheet sheet = null;
            Excel.Range oRng = null;

            x.Visible = false;
            book = x.Workbooks.Open(@"D:\ShopDoc.xlsx");
            sheet = (Excel.Worksheet)book.Sheets[1];

            foreach (KeyValuePair<string,OperData> kvp in DicNCData)
            {
                string[] splitOperName = kvp.Value.OperName.Split(',');
                int needSheetNo = (splitOperName.Length / 8);
                for (int i = 1; i < (needSheetNo + 1); i++ )
                {
                    sheet.Copy(System.Type.Missing, x.Workbooks[1].Worksheets[1]);
                }
                break;
            }

            for (int i = 0; i < book.Worksheets.Count;i++ )
            {
                sheet = (Excel.Worksheet)book.Sheets[i+1];
                if (i == 0 && book.Worksheets.Count > 1 )
                {
                    sheet.Name = PartNo;
                    oRng = (Excel.Range)sheet.Cells[4, 1];
                    oRng.Value = oRng.Value.ToString().Replace("1/1", "1/" + (book.Worksheets.Count).ToString());
                }
                else
                {
                    sheet.Name = PartNo + "(" + (i + 1) + ")";
                    oRng = (Excel.Range)sheet.Cells[4, 1];
                    string temp = (i + 1).ToString();
                    oRng.Value = oRng.Value.ToString().Replace("1/1", temp + "/" + (book.Worksheets.Count).ToString());
                }
            }

            //填表
            foreach (KeyValuePair<string,OperData> kvp in DicNCData)
            {
                if (CurrentNCGroup == kvp.Key)
                {
                    
                    /*
                    for (int i = 0; i < book.Worksheets.Count;i++ )
                    {
                    */
                        string[] splitOperName = kvp.Value.OperName.Split(',');
                        string[] splitToolName = kvp.Value.ToolName.Split(',');
                        string[] splitHolderDescription = kvp.Value.HolderDescription.Split(',');
                        string[] splitOperCuttingLength = kvp.Value.CuttingLength.Split(',');
                        string[] splitOperToolFeed = kvp.Value.ToolFeed.Split(',');
                        string[] splitOperCuttingTime = kvp.Value.CuttingTime.Split(',');
                        string[] splitOperToolNumber = kvp.Value.ToolNumber.Split(',');
                        string[] splitOperToolSpeed = kvp.Value.ToolSpeed.Split(',');

                        for (int j = 0; j < splitOperName.Length; j++)
                        {
                            RowColumn sRowColumn;
                            GetExcelRowColumn(j, out sRowColumn);
                            switch(j/8)
                            {
                                case 0:
                                    sheet = (Excel.Worksheet)book.Sheets[1];
                                    break;
                                case 1:
                                    sheet = (Excel.Worksheet)book.Sheets[2];
                                    break;
                                case 2:
                                    sheet = (Excel.Worksheet)book.Sheets[3];
                                    break;
                            }
                            oRng = (Excel.Range)sheet.Cells;
                            oRng[sRowColumn.ImgToolRow, sRowColumn.ImgToolColumn] = splitOperToolNumber[j] + "，" + splitOperName[j];
                            //oRng[sRowColumn.ImgOperRow, sRowColumn.ImgOperColumn] = splitOperName[j];
                            oRng[sRowColumn.ToolNumberRow, sRowColumn.ToolNumberColumn] = splitOperToolNumber[j];
                            oRng[sRowColumn.ToolNameRow, sRowColumn.ToolNameColumn] = splitToolName[j];
                            oRng[sRowColumn.OperNameRow, sRowColumn.OperNameColumn] = splitOperName[j];
                            oRng[sRowColumn.HolderRow, sRowColumn.HolderColumn] = splitHolderDescription[j];
                            oRng[sRowColumn.ToolFeedRow, sRowColumn.ToolFeedColumn] = "F：" + splitOperToolFeed[j];
                            oRng[sRowColumn.ToolSpeedRow, sRowColumn.ToolSpeedColumn] = "S：" + splitOperToolSpeed[j];
                            oRng[sRowColumn.CuttingTimeRow, sRowColumn.CuttingTimeColumn] = splitOperCuttingTime[j];

                            ImgPosiSize sImgPosiSize = new ImgPosiSize();
                            GetImgPosiAndSize(j, sheet, oRng, out sImgPosiSize);


                            string ImagePath = string.Format(@"{0}\{1}", Local_Folder_CAM, splitOperName[j] + ".jpg");

                            sheet.Shapes.AddPicture(ImagePath, Microsoft.Office.Core.MsoTriState.msoFalse,
                                Microsoft.Office.Core.MsoTriState.msoTrue, sImgPosiSize.PosiLeft,
                                sImgPosiSize.PosiTop, sImgPosiSize.ImgWidth, sImgPosiSize.ImgHeight);

                            System.IO.File.Delete(ImagePath);
                        }
                    /*}*/
                    
                }
            }

            book.SaveAs(OutputPath.Text, Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            book.Close(Type.Missing, Type.Missing, Type.Missing);
            x.Quit();

            //this.Hide();

            //UI.GetUI().NXMessageBox.Show("恭喜", NXMessageBox.DialogType.Information, "刀具路徑與清單輸出完成！");

            //this.Show();
        }

        private void CloseDlg_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GetExcelRowColumn(int i,out RowColumn sRowColumn)
        {
            sRowColumn = new RowColumn();

            if (i == 0 || i == 8 || i == 16)
            {
                sRowColumn.ImgToolRow = 5;
                sRowColumn.ImgToolColumn = 1;

                //sRowColumn.ImgOperRow = 5;
                //sRowColumn.ImgOperColumn = 2;

                sRowColumn.ToolNumberRow = 23;
                sRowColumn.ToolNumberColumn = 2;

                sRowColumn.ToolNameRow = 23;
                sRowColumn.ToolNameColumn = 3;

                sRowColumn.OperNameRow = 24;
                sRowColumn.OperNameColumn = 2;

                sRowColumn.HolderRow = 25;
                sRowColumn.HolderColumn = 2;

                sRowColumn.ToolFeedRow = 27;
                sRowColumn.ToolFeedColumn = 2;

                sRowColumn.ToolSpeedRow = 27;
                sRowColumn.ToolSpeedColumn = 3;

                sRowColumn.CuttingTimeRow = 28;
                sRowColumn.CuttingTimeColumn = 2;
            }
            else if (i == 1 || i == 9 || i == 17)
            {
                sRowColumn.ImgToolRow = 5;
                sRowColumn.ImgToolColumn = 4;

                //sRowColumn.ImgOperRow = 5;
                //sRowColumn.ImgOperColumn = 5;

                sRowColumn.ToolNumberRow = 29;
                sRowColumn.ToolNumberColumn = 2;

                sRowColumn.ToolNameRow = 29;
                sRowColumn.ToolNameColumn = 3;

                sRowColumn.OperNameRow = 30;
                sRowColumn.OperNameColumn = 2;

                sRowColumn.HolderRow = 31;
                sRowColumn.HolderColumn = 2;

                sRowColumn.ToolFeedRow = 33;
                sRowColumn.ToolFeedColumn = 2;

                sRowColumn.ToolSpeedRow = 33;
                sRowColumn.ToolSpeedColumn = 3;

                sRowColumn.CuttingTimeRow = 34;
                sRowColumn.CuttingTimeColumn = 2;
            }
            else if (i == 2 || i == 10 || i == 18)
            {
                sRowColumn.ImgToolRow = 5;
                sRowColumn.ImgToolColumn = 7;

                //sRowColumn.ImgOperRow = 5;
                //sRowColumn.ImgOperColumn = 8;

                sRowColumn.ToolNumberRow = 35;
                sRowColumn.ToolNumberColumn = 2;

                sRowColumn.ToolNameRow = 35;
                sRowColumn.ToolNameColumn = 3;

                sRowColumn.OperNameRow = 36;
                sRowColumn.OperNameColumn = 2;

                sRowColumn.HolderRow = 37;
                sRowColumn.HolderColumn = 2;

                sRowColumn.ToolFeedRow = 39;
                sRowColumn.ToolFeedColumn = 2;

                sRowColumn.ToolSpeedRow = 39;
                sRowColumn.ToolSpeedColumn = 3;

                sRowColumn.CuttingTimeRow = 40;
                sRowColumn.CuttingTimeColumn = 2;
            }
            else if (i == 3 || i == 11 || i == 19)
            {
                sRowColumn.ImgToolRow = 5;
                sRowColumn.ImgToolColumn = 10;

                //sRowColumn.ImgOperRow = 5;
                //sRowColumn.ImgOperColumn = 11;

                sRowColumn.ToolNumberRow = 41;
                sRowColumn.ToolNumberColumn = 2;

                sRowColumn.ToolNameRow = 41;
                sRowColumn.ToolNameColumn = 3;

                sRowColumn.OperNameRow = 42;
                sRowColumn.OperNameColumn = 2;

                sRowColumn.HolderRow = 43;
                sRowColumn.HolderColumn = 2;

                sRowColumn.ToolFeedRow = 45;
                sRowColumn.ToolFeedColumn = 2;

                sRowColumn.ToolSpeedRow = 45;
                sRowColumn.ToolSpeedColumn = 3;

                sRowColumn.CuttingTimeRow = 46;
                sRowColumn.CuttingTimeColumn = 2;
            }
            else if (i == 4 || i == 12 || i == 20)
            {
                sRowColumn.ImgToolRow = 13;
                sRowColumn.ImgToolColumn = 1;

                //sRowColumn.ImgOperRow = 13;
                //sRowColumn.ImgOperColumn = 2;

                sRowColumn.ToolNumberRow = 23;
                sRowColumn.ToolNumberColumn = 6;

                sRowColumn.ToolNameRow = 23;
                sRowColumn.ToolNameColumn = 7;

                sRowColumn.OperNameRow = 24;
                sRowColumn.OperNameColumn = 6;

                sRowColumn.HolderRow = 25;
                sRowColumn.HolderColumn = 6;

                sRowColumn.ToolFeedRow = 27;
                sRowColumn.ToolFeedColumn = 6;

                sRowColumn.ToolSpeedRow = 27;
                sRowColumn.ToolSpeedColumn = 7;

                sRowColumn.CuttingTimeRow = 28;
                sRowColumn.CuttingTimeColumn = 6;
            }
            else if (i == 5 || i == 13 || i == 21)
            {
                sRowColumn.ImgToolRow = 13;
                sRowColumn.ImgToolColumn = 4;

                //sRowColumn.ImgOperRow = 13;
                //sRowColumn.ImgOperColumn = 5;

                sRowColumn.ToolNumberRow = 29;
                sRowColumn.ToolNumberColumn = 6;

                sRowColumn.ToolNameRow = 29;
                sRowColumn.ToolNameColumn = 7;

                sRowColumn.OperNameRow = 30;
                sRowColumn.OperNameColumn = 6;

                sRowColumn.HolderRow = 31;
                sRowColumn.HolderColumn = 6;

                sRowColumn.ToolFeedRow = 33;
                sRowColumn.ToolFeedColumn = 6;

                sRowColumn.ToolSpeedRow = 33;
                sRowColumn.ToolSpeedColumn = 7;

                sRowColumn.CuttingTimeRow = 34;
                sRowColumn.CuttingTimeColumn = 6;
            }
            else if (i == 6 || i == 14 || i == 22)
            {
                sRowColumn.ImgToolRow = 13;
                sRowColumn.ImgToolColumn = 7;

                //sRowColumn.ImgOperRow = 13;
                //sRowColumn.ImgOperColumn = 8;

                sRowColumn.ToolNumberRow = 35;
                sRowColumn.ToolNumberColumn = 6;

                sRowColumn.ToolNameRow = 35;
                sRowColumn.ToolNameColumn = 7;

                sRowColumn.OperNameRow = 36;
                sRowColumn.OperNameColumn = 6;

                sRowColumn.HolderRow = 37;
                sRowColumn.HolderColumn = 6;

                sRowColumn.ToolFeedRow = 39;
                sRowColumn.ToolFeedColumn = 6;

                sRowColumn.ToolSpeedRow = 39;
                sRowColumn.ToolSpeedColumn = 7;

                sRowColumn.CuttingTimeRow = 40;
                sRowColumn.CuttingTimeColumn = 6;
            }
            else if (i == 7 || i == 15 || i == 23)
            {
                sRowColumn.ImgToolRow = 13;
                sRowColumn.ImgToolColumn = 10;

                //sRowColumn.ImgOperRow = 13;
                //sRowColumn.ImgOperColumn = 11;

                sRowColumn.ToolNumberRow = 41;
                sRowColumn.ToolNumberColumn = 6;

                sRowColumn.ToolNameRow = 41;
                sRowColumn.ToolNameColumn = 7;

                sRowColumn.OperNameRow = 42;
                sRowColumn.OperNameColumn = 6;

                sRowColumn.HolderRow = 43;
                sRowColumn.HolderColumn = 6;

                sRowColumn.ToolFeedRow = 45;
                sRowColumn.ToolFeedColumn = 6;

                sRowColumn.ToolSpeedRow = 45;
                sRowColumn.ToolSpeedColumn = 7;

                sRowColumn.CuttingTimeRow = 46;
                sRowColumn.CuttingTimeColumn = 6;
            }
        }

        private void GetImgPosiAndSize(int i, Excel.Worksheet sheet, Excel.Range oRng, out ImgPosiSize sImgPosiSize)
        {
            sImgPosiSize = new ImgPosiSize();
            //oRng = (Excel.Range)sheet.get_Range("A1");
            //CaxLog.ShowListingWindow("oRng.ColumnWidth：" + oRng.ColumnWidth.ToString());
            //CaxLog.ShowListingWindow("oRng.Width：" + oRng.Width.ToString());
            //CaxLog.ShowListingWindow("oRng.Height：" + oRng.Height.ToString());
            //CaxLog.ShowListingWindow("oRng.Left：" + oRng.Left.ToString());
            //CaxLog.ShowListingWindow("oRng.Width.GetType()：" + oRng.Width.GetType().ToString());
            //CaxLog.ShowListingWindow("oRng.Top：" + oRng.Top.ToString());
            //float xx = System.Convert.ToSingle(oRng.Width.ToString());
            //float yy = System.Convert.ToSingle(oRng.Height.ToString());

            //sheet.Shapes.AddPicture(jpgpath, Microsoft.Office.Core.MsoTriState.msoFalse,
            //    Microsoft.Office.Core.MsoTriState.msoTrue, System.Convert.ToSingle(oRng.Left.ToString()) + 100,
            //    System.Convert.ToSingle(oRng.Top.ToString()), xx, yy);



            if (i == 0 || i == 8 || i == 16)
            {
                sImgPosiSize.PosiLeft = 10;
                sImgPosiSize.PosiTop = 108;
                sImgPosiSize.ImgWidth = 155;
                sImgPosiSize.ImgHeight = 105;
            }
            else if (i == 1 || i == 9 || i == 17)
            {
                sImgPosiSize.PosiLeft = 190;
                sImgPosiSize.PosiTop = 108;
                sImgPosiSize.ImgWidth = 155;
                sImgPosiSize.ImgHeight = 105;
            }
            else if (i == 2 || i == 10 || i == 18)
            {
                sImgPosiSize.PosiLeft = 370;
                sImgPosiSize.PosiTop = 108;
                sImgPosiSize.ImgWidth = 155;
                sImgPosiSize.ImgHeight = 105;
            }
            else if (i == 3 || i == 11 || i == 19)
            {
                sImgPosiSize.PosiLeft = 545;
                sImgPosiSize.PosiTop = 108;
                sImgPosiSize.ImgWidth = 155;
                sImgPosiSize.ImgHeight = 105;
            }
            else if (i == 4 || i == 12 || i == 20)
            {
                sImgPosiSize.PosiLeft = 10;
                sImgPosiSize.PosiTop = 235;
                sImgPosiSize.ImgWidth = 155;
                sImgPosiSize.ImgHeight = 105;
            }
            else if (i == 5 || i == 13 || i == 21)
            {
                sImgPosiSize.PosiLeft = 190;
                sImgPosiSize.PosiTop = 235;
                sImgPosiSize.ImgWidth = 155;
                sImgPosiSize.ImgHeight = 105;
            }
            else if (i == 6 || i == 14 || i == 22)
            {
                sImgPosiSize.PosiLeft = 370;
                sImgPosiSize.PosiTop = 235;
                sImgPosiSize.ImgWidth = 155;
                sImgPosiSize.ImgHeight = 105;
            }
            else if (i == 7 || i == 15 || i == 23)
            {
                sImgPosiSize.PosiLeft = 545;
                sImgPosiSize.PosiTop = 235;
                sImgPosiSize.ImgWidth = 155;
                sImgPosiSize.ImgHeight = 105;
            }
        }

        public class SetView : GridButtonXEditControl
        {
            public static Matrix3x3 VIEW_MATRIX;
            public static double VIEW_SCALE;

            public SetView()
            {
                try
                {
                    Click += SetViewClick;
                }
                catch (System.Exception ex)
                {
                }
            }
            public void SetViewClick(object sender, EventArgs e)
            {
                SetView cSetView = (SetView)sender;
                int index = cSetView.EditorCell.RowIndex;
                string CurrentOperName = panel.GetCell(index, 0).Value.ToString();

                foreach (NXOpen.CAM.NCGroup ncGroup in NCGroupAry)
                {
                    if (CurrentNCGroup == ncGroup.Name)
                    {
                        for (int i = 0; i < OperationAry.Length; i++)
                        {
                            //取得父層的群組(回傳：NCGroup XXXX)
                            string NCProgramTag = OperationAry[i].GetParent(CAMSetup.View.ProgramOrder).ToString();
                            NCProgramTag = Regex.Replace(NCProgramTag, "[^0-9]", "");
                            NXOpen.CAM.CAMObject[] tempObjToCreateImg = new CAMObject[1];
                            string ImagePath = "";
                            if (NCProgramTag == ncGroup.Tag.ToString() && CurrentOperName == CaxOper.AskOperNameFromTag(OperationAry[i].Tag))
                            {
                                tempObjToCreateImg[0] = (NXOpen.CAM.CAMObject)OperationAry[i];
                                
                                workPart.CAMSetup.ReplayToolPath(tempObjToCreateImg);
                                
                                ImagePath = string.Format(@"{0}\{1}", Local_Folder_CAM, OperationAry[i].Name);
                                theUfSession.Disp.CreateImage(ImagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);
                            }
                        }
                    }
                }


                //VIEW_MATRIX = workPart.ModelingViews.WorkView.Matrix;
                //VIEW_SCALE = workPart.ModelingViews.WorkView.Scale;
                

                //string ImagePath = string.Format(@"{0}\{1}", Local_Folder_CAM, CurrentOperName + ".jpg");
                //theUfSession.Disp.CreateImage(ImagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);

            }
        }
    }
}
