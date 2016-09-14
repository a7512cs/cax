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
            //取得METEDownload_Upload.dat
            CaxGetDatData.GetMETEDownload_Upload(out cMETE_Download_Upload_Path);

            //取得料號
            PartNoLabel.Text = Path.GetFileNameWithoutExtension(displayPart.FullPath).Split('_')[0];
            OISLabel.Text = Path.GetFileNameWithoutExtension(displayPart.FullPath).Split('_')[1];

            //將Local_Folder_OIS先暫存起來，然後改變成Server路徑
            tempLocal_Folder_OIS = cMETE_Download_Upload_Path.Local_Folder_OIS;

            CaxPublic.GetAllPath("ME", displayPart.FullPath, ref cMETE_Download_Upload_Path);

            /*
            //拆零件路徑字串取得客戶名稱、料號、版本
            string PartFullPath = displayPart.FullPath;
            string[] SplitPath = PartFullPath.Split('\\');
            PartInfo.CusName = SplitPath[3];
            PartInfo.PartNo = SplitPath[4];
            PartInfo.CusRev = SplitPath[5];
            PartInfo.OpNum = Path.GetFileNameWithoutExtension(displayPart.FullPath).Split(new string[] { "OIS" }, StringSplitOptions.RemoveEmptyEntries)[1];

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
            }

            long ExcelIPQCFileTime = new long();
            long ExcelSelfCheckFileTime = new long();
            long ExcelIQCFileTime = new long();
            long ExcelFAIFileTime = new long();

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
            #endregion

        }

        private void OK_Click(object sender, EventArgs e)
        {
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
                    CaxLog.ShowListingWindow(Path.GetFileName(kvp.Value.PartLocalDir) + "上傳失敗");
                    this.Close();
                }
                
                ListPartName.Add(kvp.Key + ".prt"); 
            }
            PartText = ListPartName.ToArray();
            System.IO.File.WriteAllLines(string.Format(@"{0}\{1}", Server_OP_Folder, "PartNameText_OIS.txt"), PartText);


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

            MessageBox.Show("上傳完成！");
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
