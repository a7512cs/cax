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
using System.Text.RegularExpressions;

namespace TEUpload
{
    public partial class TEUploadDlg : DevComponents.DotNetBar.Office2007Form
    {
        public static Session theSession = Session.GetSession();
        public static UI theUI;
        public static UFSession theUfSession = UFSession.GetUFSession();
        public static Part workPart = theSession.Parts.Work;
        public static Part displayPart = theSession.Parts.Display;
        public static METE_Download_Upload_Path cMETE_Download_Upload_Path = new METE_Download_Upload_Path();
        public static string Server_OP_Folder = "", tempLocal_Folder_CAM = "";
        public static Dictionary<string, PartDirData> DicPartDirData = new Dictionary<string, PartDirData>();
        public static ExcelDirData sExcelDirData = new ExcelDirData();
        public static NCProgramDirData sNCProgramDirData = new NCProgramDirData();

        public struct NCProgramDirData
        {
            public string NCProgramLocalDir { get; set; }
            public string NCProgramServerDir { get; set; }
        }

        public struct ExcelDirData
        {
            public string ExcelShopDocLocalDir { get; set; }
            public string ExcelShopDocServerDir { get; set; }
        }

        public struct PartDirData
        {
            public string PartLocalDir { get; set; }
            public string PartServer1Dir { get; set; }
            //public string PartServer2Dir { get; set; }
        }

        public struct PartInfo
        {
            public static string CusName { get; set; }
            public static string PartNo { get; set; }
            public static string CusRev { get; set; }
            public static string OpNum { get; set; }
        }

        public TEUploadDlg()
        {
            InitializeComponent();
        }

        private void TEUploadDlg_Load(object sender, EventArgs e)
        {
            //取得METEDownload_Upload.dat
            CaxGetDatData.GetMETEDownload_Upload(out cMETE_Download_Upload_Path);

            //取得料號
            PartNoLabel.Text = Path.GetFileNameWithoutExtension(displayPart.FullPath).Split('_')[0];
            OISLabel.Text = Path.GetFileNameWithoutExtension(displayPart.FullPath).Split('_')[1];

            //將Local_Folder_OIS先暫存起來，然後改變成Server路徑
            tempLocal_Folder_CAM = cMETE_Download_Upload_Path.Local_Folder_CAM;

            CaxPublic.GetAllPath("TE", displayPart.FullPath, ref cMETE_Download_Upload_Path);


            
            //拆零件路徑字串取得客戶名稱、料號、版本
            string PartFullPath = displayPart.FullPath;
            string[] SplitPath = PartFullPath.Split('\\');
            PartInfo.CusName = SplitPath[3];
            PartInfo.PartNo = SplitPath[4];
            PartInfo.CusRev = SplitPath[5];
            PartInfo.OpNum = Regex.Replace(Path.GetFileNameWithoutExtension(displayPart.FullPath).Split('_')[1], "[^0-9]", "");
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
            tempLocal_Folder_CAM = cMETE_Download_Upload_Path.Local_Folder_CAM;
            cMETE_Download_Upload_Path.Local_ShareStr = cMETE_Download_Upload_Path.Local_ShareStr.Replace("[Local_IP]", cMETE_Download_Upload_Path.Local_IP);
            cMETE_Download_Upload_Path.Local_ShareStr = cMETE_Download_Upload_Path.Local_ShareStr.Replace("[CusName]", PartInfo.CusName);
            cMETE_Download_Upload_Path.Local_ShareStr = cMETE_Download_Upload_Path.Local_ShareStr.Replace("[PartNo]", PartInfo.PartNo);
            cMETE_Download_Upload_Path.Local_ShareStr = cMETE_Download_Upload_Path.Local_ShareStr.Replace("[CusRev]", PartInfo.CusRev);
            cMETE_Download_Upload_Path.Local_Folder_CAM = cMETE_Download_Upload_Path.Local_Folder_CAM.Replace("[Local_ShareStr]", cMETE_Download_Upload_Path.Local_ShareStr);
            cMETE_Download_Upload_Path.Local_Folder_CAM = cMETE_Download_Upload_Path.Local_Folder_CAM.Replace("[Oper1]", PartInfo.OpNum);
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
            //foreach (NXOpen.Assemblies.Component i in ListChildrenComp)
            //{
            //    sPartDirData = new PartDirData();
            //    sPartDirData.PartLocalDir = ((Part)i.Prototype).FullPath;
            //    sPartDirData.PartServer1Dir = string.Format(@"{0}\{1}", cMETE_Download_Upload_Path.Server_ShareStr, Path.GetFileName(((Part)i.Prototype).FullPath));
            //    //sPartDirData.PartServer2Dir = string.Format(@"{0}\{1}", cMETE_Download_Upload_Path.Server_ShareStr, "OP" + PartInfo.OIS);
            //    DicPartDirData.Add(i.Name, sPartDirData);
            //    listView1.Items.Add(Path.GetFileName(((Part)i.Prototype).FullPath));
            //}
            #endregion

            #region 處理Excel的路徑
            string[] FolderFile = System.IO.Directory.GetFileSystemEntries(cMETE_Download_Upload_Path.Local_Folder_CAM, "*.xls");
            string Server_Folder_CAM = "";
            Server_Folder_CAM = tempLocal_Folder_CAM.Replace("[Local_ShareStr]", cMETE_Download_Upload_Path.Server_ShareStr);
            Server_Folder_CAM = Server_Folder_CAM.Replace("[Oper1]", PartInfo.OpNum);
            long FileTime = new long();
            for (int i = 0; i < FolderFile.Length; i++)
            {
                System.IO.FileInfo ExcelInfo = new System.IO.FileInfo(FolderFile[i]);
                if (ExcelInfo.LastAccessTime.ToFileTime() > FileTime)
                {
                    FileTime = ExcelInfo.LastAccessTime.ToFileTime();
                    sExcelDirData.ExcelShopDocLocalDir = FolderFile[i];
                    sExcelDirData.ExcelShopDocServerDir = string.Format(@"{0}\{1}", Server_Folder_CAM, ExcelInfo.Name);
                }
            }
            if (File.Exists(sExcelDirData.ExcelShopDocLocalDir))
            {
                listView1.Items.Add(Path.GetFileName(sExcelDirData.ExcelShopDocLocalDir));
            }
            #endregion

            #region 處理NC程式的路徑
            string Local_NC_Folder = string.Format(@"{0}\{1}", cMETE_Download_Upload_Path.Local_Folder_CAM, "NC");
            if (Directory.Exists(Local_NC_Folder))
            {
                sNCProgramDirData.NCProgramLocalDir = Local_NC_Folder;
                sNCProgramDirData.NCProgramServerDir = string.Format(@"{0}\{1}", Server_Folder_CAM, "NC");
                listView1.Items.Add("NC資料夾");
            }
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
            System.IO.File.WriteAllLines(string.Format(@"{0}\{1}", Server_OP_Folder, "PartNameText_CAM.txt"), PartText);


            //Excel上傳
            if (File.Exists(sExcelDirData.ExcelShopDocLocalDir))
            {
                try
                {
                    File.Copy(sExcelDirData.ExcelShopDocLocalDir, sExcelDirData.ExcelShopDocServerDir, true);
                }
                catch (System.Exception ex)
                {
                    CaxLog.ShowListingWindow("ShopDoc.xls上傳失敗");
                    this.Close();
                }
            }

            //NC上傳
            //判斷NC是否存在
            if (Directory.Exists(sNCProgramDirData.NCProgramLocalDir))
            {
                try
                {
                    CaxPublic.DirectoryCopy(sNCProgramDirData.NCProgramLocalDir, sNCProgramDirData.NCProgramServerDir,true);
                }
                catch (System.Exception ex)
                {
                    CaxLog.ShowListingWindow("NC上傳失敗");
                    this.Close();
                }
            }


            MessageBox.Show("上傳完成！");
            this.Close();
        }
    }
}
