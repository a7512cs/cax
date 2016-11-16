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
using System.IO;
using NHibernate;
using MEUpload.DatabaseClass;
using NXOpen.Utilities;
using DevComponents.DotNetBar;

namespace MEUpload
{
    public partial class MEUploadDlg : DevComponents.DotNetBar.Office2007Form
    {
        public static Session theSession = Session.GetSession();
        public static UI theUI;
        public static UFSession theUfSession = UFSession.GetUFSession();
        public static Part workPart = theSession.Parts.Work;
        public static Part displayPart = theSession.Parts.Display;
        public static string Server_OP_Folder = "", tempLocal_Folder_OIS = "";
        public static METE_Download_Upload_Path cMETE_Download_Upload_Path = new METE_Download_Upload_Path();
        public static Dictionary<string, PartDirData> DicPartDirData = new Dictionary<string, PartDirData>();
        public static ExcelDirData sExcelDirData = new ExcelDirData();
        public static ISession session = MyHibernateHelper.SessionFactory.OpenSession();

        public struct PartDirData
        {
            public string PartLocalDir { get; set; }
            public string PartServer1Dir { get; set; }
            //public string PartServer2Dir { get; set; }
        }

        public struct ExcelDirData
        {
            public string ExcelIPQCLocalDir { get; set; }
            public string ExcelIPQCServerDir { get; set; }
            public string ExcelSelfCheckLocalDir { get; set; }
            public string ExcelSelfCheckServerDir { get; set; }
            public string ExcelIQCLocalDir { get; set; }
            public string ExcelIQCServerDir { get; set; }
            public string ExcelFAILocalDir { get; set; }
            public string ExcelFAIServerDir { get; set; }
            public string ExcelFQCLocalDir { get; set; }
            public string ExcelFQCServerDir { get; set; }
        }

        public struct PartInfo 
        {
            public static string CusName { get; set; }
            public static string PartNo { get; set; }
            public static string CusRev { get; set; }
            public static string OpNum { get; set; }
        }

        public MEUploadDlg()
        {
            InitializeComponent();
        }

        private void MEUploadDlg_Load(object sender, EventArgs e)
        {
            //int module_id;
            //theUfSession.UF.AskApplicationModule(out module_id);
            //if (module_id != UFConstants.UF_APP_DRAFTING)
            //{
            //    MessageBox.Show("請先轉換為製圖模組後再執行！");
            //    this.Close();
            //}

            //取得METEDownload_Upload.dat
            CaxGetDatData.GetMETEDownload_Upload(out cMETE_Download_Upload_Path);

            //取得料號
            PartNoLabel.Text = Path.GetFileNameWithoutExtension(displayPart.FullPath).Split('_')[0];
            OISLabel.Text = Path.GetFileNameWithoutExtension(displayPart.FullPath).Split('_')[1];

            //將Local_Folder_OIS先暫存起來，然後改變成Server路徑
            tempLocal_Folder_OIS = cMETE_Download_Upload_Path.Local_Folder_OIS;

            CaxPublic.GetAllPath("ME", displayPart.FullPath, ref cMETE_Download_Upload_Path);

            
            //拆零件路徑字串取得客戶名稱、料號、版本
            string PartFullPath = displayPart.FullPath;
            string[] SplitPath = PartFullPath.Split('\\');
            PartInfo.CusName = SplitPath[3];
            PartInfo.PartNo = SplitPath[4];
            PartInfo.CusRev = SplitPath[5];
            PartInfo.OpNum = Path.GetFileNameWithoutExtension(displayPart.FullPath).Split(new string[] { "OIS" }, StringSplitOptions.RemoveEmptyEntries)[1];
            
            /*
            #region 取代Server路徑字串
            cMETE_Download_Upload_Path.Server_ShareStr = cMETE_Download_Upload_Path.Server_ShareStr.Replace("[Server_IP]", cMETE_Download_Upload_Path.Server_IP);
            cMETE_Download_Upload_Path.Server_ShareStr = cMETE_Download_Upload_Path.Server_ShareStr.Replace("[CusName]", PartInfo.CusName);
            cMETE_Download_Upload_Path.Server_ShareStr = cMETE_Download_Upload_Path.Server_ShareStr.Replace("[PartNo]", PartInfo.PartNo);
            cMETE_Download_Upload_Path.Server_ShareStr = cMETE_Download_Upload_Path.Server_ShareStr.Replace("[CusRev]", PartInfo.CusRev);
            Server_OP_Folder = string.Format(@"{0}\{1}", cMETE_Download_Upload_Path.Server_ShareStr, "OP" + PartInfo.OpNum);
            #endregion

            #region 取代Local路徑字串
            //將Local_Folder_OIS先暫存起來，然後改變成Server路徑
            tempLocal_Folder_OIS = cMETE_Download_Upload_Path.Local_Folder_OIS;
            cMETE_Download_Upload_Path.Local_ShareStr = cMETE_Download_Upload_Path.Local_ShareStr.Replace("[Local_IP]", cMETE_Download_Upload_Path.Local_IP);
            cMETE_Download_Upload_Path.Local_ShareStr = cMETE_Download_Upload_Path.Local_ShareStr.Replace("[CusName]", PartInfo.CusName);
            cMETE_Download_Upload_Path.Local_ShareStr = cMETE_Download_Upload_Path.Local_ShareStr.Replace("[PartNo]", PartInfo.PartNo);
            cMETE_Download_Upload_Path.Local_ShareStr = cMETE_Download_Upload_Path.Local_ShareStr.Replace("[CusRev]", PartInfo.CusRev);
            cMETE_Download_Upload_Path.Local_Folder_OIS = cMETE_Download_Upload_Path.Local_Folder_OIS.Replace("[Local_ShareStr]", cMETE_Download_Upload_Path.Local_ShareStr);
            cMETE_Download_Upload_Path.Local_Folder_OIS = cMETE_Download_Upload_Path.Local_Folder_OIS.Replace("[Oper1]", PartInfo.OpNum);
            #endregion
            */

            DicPartDirData = new Dictionary<string, PartDirData>();
            #region 處理Part的路徑
            //預先加入總組立檔
            PartDirData sPartDirData = new PartDirData();
            sPartDirData.PartLocalDir = displayPart.FullPath;
            sPartDirData.PartServer1Dir = string.Format(@"{0}\{1}", cMETE_Download_Upload_Path.Server_ShareStr, Path.GetFileName(displayPart.FullPath));
            //sPartDirData.PartServer2Dir = string.Format(@"{0}\{1}", cMETE_Download_Upload_Path.Server_ShareStr, "OP" + PartInfo.OIS);
            DicPartDirData.Add(Path.GetFileNameWithoutExtension(displayPart.FullPath), sPartDirData);
            listView1.Items.Add(Path.GetFileName(displayPart.FullPath));

            //加入Comp資訊
            NXOpen.Assemblies.ComponentAssembly casm = displayPart.ComponentAssembly;
            //NXOpen.Assemblies.Component[] compary = casm.RootComponent.GetChildren();
            List<NXOpen.Assemblies.Component> ListChildrenComp = new List<NXOpen.Assemblies.Component>();
            CaxAsm.GetCompChildren(casm.RootComponent, ref ListChildrenComp);
            foreach (NXOpen.Assemblies.Component i in ListChildrenComp)
            {
                sPartDirData = new PartDirData();
                
                //判斷Server是否已存在此檔案，如果有就不上傳
                string ServerPartPath = string.Format(@"{0}\{1}", cMETE_Download_Upload_Path.Server_ShareStr, Path.GetFileName(((Part)i.Prototype).FullPath));
                if (File.Exists(ServerPartPath))
                {
                    if (!((Part)i.Prototype).FullPath.Contains(PartInfo.OpNum))
                    {
                        continue;
                    }
                }
                sPartDirData.PartLocalDir = ((Part)i.Prototype).FullPath;
                sPartDirData.PartServer1Dir = ServerPartPath;
                DicPartDirData.Add(i.Name, sPartDirData);
                listView1.Items.Add(Path.GetFileName(((Part)i.Prototype).FullPath));
                //sPartDirData.PartServer2Dir = string.Format(@"{0}\{1}", cMETE_Download_Upload_Path.Server_ShareStr, "OP" + PartInfo.OIS);
            }
            #endregion

            #region 處理Excel的路徑
            string[] FolderFile = System.IO.Directory.GetFileSystemEntries(cMETE_Download_Upload_Path.Local_Folder_OIS, "*.xls");
            //篩選出IPQC、SelfCheck
            List<string> List_ExcelIPQC = new List<string>();
            List<string> List_ExcelSelfCheck = new List<string>();
            List<string> List_ExcelIQC = new List<string>();
            List<string> List_ExcelFAI = new List<string>();
            List<string> List_ExcelFQC = new List<string>();
            foreach (string i in FolderFile)
            {
                if (i.Contains("IPQC"))
                {
                    List_ExcelIPQC.Add(i);
                }
                if (i.Contains("SelfCheck"))
                {
                    List_ExcelSelfCheck.Add(i);
                }
                if (i.Contains("IQC"))
                {
                    List_ExcelIQC.Add(i);
                }
                if (i.Contains("FAI"))
                {
                    List_ExcelFAI.Add(i);
                }
                if (i.Contains("FQC"))
                {
                    List_ExcelFQC.Add(i);
                }
            }

            long ExcelIPQCFileTime = new long();
            long ExcelSelfCheckFileTime = new long();
            long ExcelIQCFileTime = new long();
            long ExcelFAIFileTime = new long();
            long ExcelFQCFileTime = new long();

            #region 處理IPQC
            foreach (string i in List_ExcelIPQC)
            {
                System.IO.FileInfo ExcelInfo = new System.IO.FileInfo(i);
                if (ExcelInfo.LastAccessTime.ToFileTime() > ExcelIPQCFileTime)
                {
                    ExcelIPQCFileTime = ExcelInfo.LastAccessTime.ToFileTime();
                    sExcelDirData.ExcelIPQCLocalDir = i;
                    string Server_Folder_OIS = "";
                    Server_Folder_OIS = tempLocal_Folder_OIS.Replace("[Local_ShareStr]", cMETE_Download_Upload_Path.Server_ShareStr);
                    Server_Folder_OIS = Server_Folder_OIS.Replace("[Oper1]", PartInfo.OpNum);
                    sExcelDirData.ExcelIPQCServerDir = string.Format(@"{0}\{1}", Server_Folder_OIS, ExcelInfo.Name);
                }
            }
            if (List_ExcelIPQC.Count != 0)
            {
                listView1.Items.Add(Path.GetFileName(sExcelDirData.ExcelIPQCLocalDir));
            }
            #endregion

            #region 處理SelfCheck
            foreach (string i in List_ExcelSelfCheck)
            {
                System.IO.FileInfo ExcelInfo = new System.IO.FileInfo(i);
                if (ExcelInfo.LastAccessTime.ToFileTime() > ExcelSelfCheckFileTime)
                {
                    ExcelSelfCheckFileTime = ExcelInfo.LastAccessTime.ToFileTime();
                    sExcelDirData.ExcelSelfCheckLocalDir = i;
                    string Server_Folder_OIS = "";
                    Server_Folder_OIS = tempLocal_Folder_OIS.Replace("[Local_ShareStr]", cMETE_Download_Upload_Path.Server_ShareStr);
                    Server_Folder_OIS = Server_Folder_OIS.Replace("[Oper1]", PartInfo.OpNum);
                    sExcelDirData.ExcelSelfCheckServerDir = string.Format(@"{0}\{1}", Server_Folder_OIS, ExcelInfo.Name);
                }
            }
            if (List_ExcelSelfCheck.Count != 0)
            {
                listView1.Items.Add(Path.GetFileName(sExcelDirData.ExcelSelfCheckLocalDir));
            }
            #endregion

            #region 處理IQC
            foreach (string i in List_ExcelIQC)
            {
                System.IO.FileInfo ExcelInfo = new System.IO.FileInfo(i);
                if (ExcelInfo.LastAccessTime.ToFileTime() > ExcelIQCFileTime)
                {
                    ExcelIQCFileTime = ExcelInfo.LastAccessTime.ToFileTime();
                    sExcelDirData.ExcelIQCLocalDir = i;
                    string Server_Folder_OIS = "";
                    Server_Folder_OIS = tempLocal_Folder_OIS.Replace("[Local_ShareStr]", cMETE_Download_Upload_Path.Server_ShareStr);
                    Server_Folder_OIS = Server_Folder_OIS.Replace("[Oper1]", PartInfo.OpNum);
                    sExcelDirData.ExcelIQCServerDir = string.Format(@"{0}\{1}", Server_Folder_OIS, ExcelInfo.Name);
                }
            }
            if (List_ExcelIQC.Count != 0)
            {
                listView1.Items.Add(Path.GetFileName(sExcelDirData.ExcelIQCLocalDir));
            }
            #endregion

            #region 處理FAI
            foreach (string i in List_ExcelFAI)
            {
                System.IO.FileInfo ExcelInfo = new System.IO.FileInfo(i);
                if (ExcelInfo.LastAccessTime.ToFileTime() > ExcelFAIFileTime)
                {
                    ExcelFAIFileTime = ExcelInfo.LastAccessTime.ToFileTime();
                    sExcelDirData.ExcelFAILocalDir = i;
                    string Server_Folder_OIS = "";
                    Server_Folder_OIS = tempLocal_Folder_OIS.Replace("[Local_ShareStr]", cMETE_Download_Upload_Path.Server_ShareStr);
                    Server_Folder_OIS = Server_Folder_OIS.Replace("[Oper1]", PartInfo.OpNum);
                    sExcelDirData.ExcelFAIServerDir = string.Format(@"{0}\{1}", Server_Folder_OIS, ExcelInfo.Name);
                }
            }
            if (List_ExcelFAI.Count != 0)
            {
                listView1.Items.Add(Path.GetFileName(sExcelDirData.ExcelFAILocalDir));
            }
            #endregion

            #region 處理FQC
            foreach (string i in List_ExcelFQC)
            {
                System.IO.FileInfo ExcelInfo = new System.IO.FileInfo(i);
                if (ExcelInfo.LastAccessTime.ToFileTime() > ExcelFQCFileTime)
                {
                    ExcelFQCFileTime = ExcelInfo.LastAccessTime.ToFileTime();
                    sExcelDirData.ExcelFQCLocalDir = i;
                    string Server_Folder_OIS = "";
                    Server_Folder_OIS = tempLocal_Folder_OIS.Replace("[Local_ShareStr]", cMETE_Download_Upload_Path.Server_ShareStr);
                    Server_Folder_OIS = Server_Folder_OIS.Replace("[Oper1]", PartInfo.OpNum);
                    sExcelDirData.ExcelFQCServerDir = string.Format(@"{0}\{1}", Server_Folder_OIS, ExcelInfo.Name);
                }
            }
            if (List_ExcelFQC.Count != 0)
            {
                listView1.Items.Add(Path.GetFileName(sExcelDirData.ExcelFQCLocalDir));
            }
            #endregion
            #endregion

        }

        private void OK_Click(object sender, EventArgs e)
        {
            #region Part上傳
            //Part上傳
            List<string> ListPartName = new List<string>();
            string[] PartText;
            foreach (KeyValuePair<string, PartDirData> kvp in DicPartDirData)
            {
                //判斷Part是否存在
                if (!File.Exists(kvp.Value.PartLocalDir))
                {
                    CaxLog.ShowListingWindow("Part不存在，無法上傳");
                    return;
                }
                try
                {
                    File.Copy(kvp.Value.PartLocalDir, kvp.Value.PartServer1Dir, true);
                }
                catch (System.Exception ex)
                {
                    CaxLog.ShowListingWindow(ex.ToString());
                    CaxLog.ShowListingWindow(Path.GetFileName(kvp.Value.PartLocalDir) + "上傳失敗");
                    this.Close();
                }
                
                ListPartName.Add(kvp.Key + ".prt"); 
            }
            PartText = ListPartName.ToArray();
            System.IO.File.WriteAllLines(string.Format(@"{0}\{1}\{2}", cMETE_Download_Upload_Path.Server_ShareStr, "OP" + PartInfo.OpNum, "PartNameText_OIS.txt"), PartText);
            //System.IO.File.WriteAllLines(string.Format(@"{0}\{1}", Server_OP_Folder, "PartNameText_OIS.txt"), PartText);
            #endregion

            #region Excel上傳
            //Excel上傳
            if (File.Exists(sExcelDirData.ExcelIPQCLocalDir))
            {
                try
                {
                    File.Copy(sExcelDirData.ExcelIPQCLocalDir, sExcelDirData.ExcelIPQCServerDir, true);
                }
                catch (System.Exception ex)
                {
                    CaxLog.ShowListingWindow("IPQC.xls上傳失敗");
                    this.Close();
                }
            }

            if (File.Exists(sExcelDirData.ExcelSelfCheckLocalDir))
            {
                try
                {
                    File.Copy(sExcelDirData.ExcelSelfCheckLocalDir, sExcelDirData.ExcelSelfCheckServerDir, true);
                }
                catch (System.Exception ex)
                {
                    CaxLog.ShowListingWindow("SelfCheck.xls上傳失敗");
                    this.Close();
                }
            }

            if (File.Exists(sExcelDirData.ExcelIQCLocalDir))
            {
                try
                {
                    File.Copy(sExcelDirData.ExcelIQCLocalDir, sExcelDirData.ExcelIQCServerDir, true);
                }
                catch (System.Exception ex)
                {
                    CaxLog.ShowListingWindow("IQC.xls上傳失敗");
                    this.Close();
                }
            }

            if (File.Exists(sExcelDirData.ExcelFAILocalDir))
            {
                try
                {
                    File.Copy(sExcelDirData.ExcelFAILocalDir, sExcelDirData.ExcelFAIServerDir, true);
                }
                catch (System.Exception ex)
                {
                    CaxLog.ShowListingWindow("FAI.xls上傳失敗");
                    this.Close();
                }
            }

            if (File.Exists(sExcelDirData.ExcelFQCLocalDir))
            {
                try
                {
                    File.Copy(sExcelDirData.ExcelFQCLocalDir, sExcelDirData.ExcelFQCServerDir, true);
                }
                catch (System.Exception ex)
                {
                    CaxLog.ShowListingWindow("FQC.xls上傳失敗");
                    this.Close();
                }
            }
            #endregion

            #region 資料上傳至Database
            //取得excelType是哪一種報表
            string meExcelType = "";
            try
            {
                meExcelType = workPart.GetStringAttribute("EXCELTYPE");
            }
            catch (System.Exception ex)
            {
                meExcelType = "";
            }

            if (meExcelType != "")
            {
                #region 取得PartInformation資訊(draftingVer、draftingDate、createDate、partDescription、material)
                string draftingVer = "", draftingDate = "", createDate = "", partDescription = "", material = "";
                try
                {
                    draftingVer = workPart.GetStringAttribute("REVSTARTPOS");
                }
                catch (System.Exception ex)
                {
                    draftingVer = "";
                }
                try
                {
                    partDescription = workPart.GetStringAttribute("PARTDESCRIPTIONPOS");
                }
                catch (System.Exception ex)
                {
                    partDescription = "";
                }
                try
                {
                    draftingDate = workPart.GetStringAttribute("REVDATESTARTPOS");
                }
                catch (System.Exception ex)
                {
                    draftingDate = "";
                }
                try
                {
                    material = workPart.GetStringAttribute("MATERIALPOS");
                }
                catch (System.Exception ex)
                {
                    material = "";
                }
                createDate = DateTime.Now.ToString();
                #endregion

                bool dataOK = true;
                #region 資訊遺漏提醒事項
                if (draftingVer == "" || draftingDate == "" || partDescription == "" || material == "")
                {
                    dataOK = false;
                    MessageBox.Show("量測資訊不足，僅上傳實體檔案到伺服器");
                }
                #endregion

                if (dataOK)
                {
                    #region 取得所有量測尺寸資料
                    int SheetCount = 0;
                    NXOpen.Tag[] SheetTagAry = null;
                    theUfSession.Draw.AskDrawings(out SheetCount, out SheetTagAry);

                    Database.listDimensionData = new List<DimensionData>();
                    for (int i = 0; i < SheetCount; i++)
                    {
                        //打開Sheet並記錄所有OBJ
                        NXOpen.Drawings.DrawingSheet CurrentSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(SheetTagAry[i]);
                        CurrentSheet.Open();
                        DisplayableObject[] SheetObj = CurrentSheet.View.AskVisibleObjects();

                        foreach (DisplayableObject singleObj in SheetObj)
                        {
                            DimensionData cDimensionData = new DimensionData();
                            bool status = Database.GetDimensionData(meExcelType, singleObj, out cDimensionData);
                            if (!status)
                            {
                                continue;
                            }
                            cDimensionData.draftingVer = draftingVer;
                            cDimensionData.draftingDate = draftingDate;
                            Database.listDimensionData.Add(cDimensionData);
                        }
                    }
                    #endregion

                    Com_PEMain comPEMain = new Com_PEMain();
                    #region 由料號查peSrNo  
                    try
                    {
                        comPEMain = session.QueryOver<Com_PEMain>().Where(x => x.partName == PartInfo.PartNo).SingleOrDefault<Com_PEMain>();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("資料庫中沒有此料號的紀錄，故無法上傳量測尺寸，僅成功上傳實體檔案");
                        return;
                    }
                    #endregion

                    Com_PartOperation comPartOperation = new Com_PartOperation();
                    #region 由peSrNo和Op查partOperationSrNo
                    try
                    {
                        comPartOperation = session.QueryOver<Com_PartOperation>()
                                                             .Where(x => x.comPEMain.peSrNo == comPEMain.peSrNo)
                                                             .Where(x => x.operation1 == PartInfo.OpNum)
                                                             .SingleOrDefault<Com_PartOperation>();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("資料庫中沒有此料號的紀錄，故無法上傳量測尺寸，僅成功上傳實體檔案");
                        return;
                    }
                    #endregion

                    Sys_MEExcel sysMEExcel = new Sys_MEExcel();
                    #region 由excelType查meExcelSrNo
                    try
                    {
                        sysMEExcel = session.QueryOver<Sys_MEExcel>().Where(x => x.meExcelType == meExcelType).SingleOrDefault<Sys_MEExcel>();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("資料庫中沒有此料號的紀錄，故無法上傳量測尺寸，僅成功上傳實體檔案");
                        return;
                    }
                    #endregion

                    #region 比對資料庫MEMain是否有同筆數據
                    IList<Com_MEMain> DBData_ComMEMain = new List<Com_MEMain>();
                    DBData_ComMEMain = session.QueryOver<Com_MEMain>().List<Com_MEMain>();

                    bool Is_Exist = false;
                    Com_MEMain currentComMEMain = new Com_MEMain();
                    foreach (Com_MEMain i in DBData_ComMEMain)
                    {
                        if (i.comPartOperation == comPartOperation && i.partDescription == partDescription && 
                            i.sysMEExcel == sysMEExcel && i.draftingVer == draftingVer && i.material == material)
                        {
                            Is_Exist = true;
                            currentComMEMain = i;
                            break;
                        }
                    }
                    #endregion

                    #region 如果本次上傳的資料不存在於資料庫，則開始上傳資料；如果已存在資料庫，則詢問是否要更新尺寸
                    if (!Is_Exist)
                    {
                        #region 整理資料並上傳
                        try
                        {
                            Com_MEMain cCom_MEMain = new Com_MEMain();
                            cCom_MEMain.comPartOperation = comPartOperation;
                            cCom_MEMain.sysMEExcel = sysMEExcel;
                            cCom_MEMain.partDescription = partDescription;
                            cCom_MEMain.createDate = createDate;
                            cCom_MEMain.material = material;
                            cCom_MEMain.draftingVer = draftingVer;

                            IList<Com_Dimension> listCom_Dimension = new List<Com_Dimension>();
                            foreach (DimensionData i in Database.listDimensionData)
                            {
                                Com_Dimension cCom_Dimension = new Com_Dimension();
                                cCom_Dimension.comMEMain = cCom_MEMain;
                                Database.MappingData(i, ref cCom_Dimension);
                                listCom_Dimension.Add(cCom_Dimension);
                            }

                            cCom_MEMain.comDimension = listCom_Dimension;

                            using (ITransaction trans = session.BeginTransaction())
                            {
                                session.Save(cCom_MEMain);
                                trans.Commit();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("上傳資料庫時發生錯誤，僅上傳實體檔案");
                        }
                        #endregion
                    }
                    else
                    {
                        if (eTaskDialogResult.Yes == CaxPublic.ShowMsgYesNo("此料號已存在上一次的標註尺寸資料，是否更新?"))
                        {
                            try
                            {
                                #region 先刪除尺寸資料表
                                IList<Com_Dimension> DB_ComDimension = new List<Com_Dimension>();
                                DB_ComDimension = session.QueryOver<Com_Dimension>()
                                                         .Where(x => x.comMEMain == currentComMEMain).List<Com_Dimension>();
                                using (ITransaction trans = session.BeginTransaction())
                                {
                                    foreach (Com_Dimension i in DB_ComDimension)
                                    {
                                        session.Delete(i);
                                    }
                                    trans.Commit();
                                }
                                #endregion

                                #region 重新插入所有尺寸
                                IList<Com_Dimension> listCom_Dimension = new List<Com_Dimension>();
                                foreach (DimensionData i in Database.listDimensionData)
                                {
                                    Com_Dimension cCom_Dimension = new Com_Dimension();
                                    cCom_Dimension.comMEMain = currentComMEMain;
                                    Database.MappingData(i, ref cCom_Dimension);
                                    listCom_Dimension.Add(cCom_Dimension);
                                }
                                using (ITransaction trans = session.BeginTransaction())
                                {
                                    foreach (Com_Dimension i in listCom_Dimension)
                                    {
                                        session.Save(i);
                                    }
                                    trans.Commit();
                                }
                                #endregion
                            }
                            catch (System.Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                    }
                    #endregion
                }
            }
            else
            {
                MessageBox.Show("尚未指定量測尺寸，故量測資料無法入資料庫");
            }

            #endregion

            MessageBox.Show("上傳完成！");
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
