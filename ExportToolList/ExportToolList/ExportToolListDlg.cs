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
using NXOpen.CAM;
using DevComponents.DotNetBar.SuperGrid;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace ExportToolList
{
    public partial class ExportToolListDlg : DevComponents.DotNetBar.Office2007Form
    {
        public static Session theSession = Session.GetSession();
        public static UI theUI;
        public static UFSession theUfSession = UFSession.GetUFSession();
        public static bool status;
        public static string ToolListPath = "", Is_Local = "";
        public static METE_Download_Upload_Path cMETE_Download_Upload_Path = new METE_Download_Upload_Path();
        public static Part workPart = theSession.Parts.Work;
        public static Part displayPart = theSession.Parts.Display;
        public static PartInfo sPartInfo = new PartInfo();
        public static Dictionary<string, OperData> DicNCData = new Dictionary<string, OperData>();
        public static GridPanel panel = new GridPanel();
        public static string CurrentNCName = "", PartNo = "", ToolListFolder = "", OutputPath = "";
        public ApplicationClass excelApp = null;
        public Workbook book = null;
        public Worksheet sheet = null;
        public Range oRng = null;

        public struct OperData
        {
            public string OperName { get; set; }
            public string ToolName { get; set; }
            public string HolderDescription { get; set; }
            //public string CuttingLength { get; set; }
            //public string CuttingTime { get; set; }
            //public string ToolFeed { get; set; }
            public string ToolNumber { get; set; }
            //public string ToolSpeed { get; set; }
            //public string PartStock { get; set; }
            //public string PartFloorStock { get; set; }
        }

        public ExportToolListDlg()
        {
            InitializeComponent();
        }

        private static bool PreCheckNC(NXOpen.CAM.NCGroup ncGroup)
        {
            try
            {
                int type;
                int subtype;
                theUfSession.Obj.AskTypeAndSubtype(ncGroup.Tag, out type, out subtype);

                //比對是否為Program群組
                if (type != UFConstants.UF_machining_task_type)
                {
                    return false;
                }
                //過濾PROGRAM
                if (ncGroup.Name == "PROGRAM")
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
        public static bool Doit(ComboBox comboBoxNCName)
        {
            try
            {
                int module_id;
                theUfSession.UF.AskApplicationModule(out module_id);
                if (module_id != UFConstants.UF_APP_CAM)
                {
                    MessageBox.Show("請先轉換為加工模組後再執行！");
                    return false;
                }

                if (!GetToolListPath(out ToolListPath))
                {
                    MessageBox.Show("取得ToolList.xls失敗");
                    return false;
                }
                 
                

                //取得正確路徑，拆零件路徑字串取得客戶名稱、料號、版本
                status = CaxPublic.GetAllPath("TE", displayPart.FullPath, out sPartInfo, ref cMETE_Download_Upload_Path);
                if (!status)
                {
                    Is_Local = null;
                }

                //有此條件判斷是否為走系統的零件
                if (!displayPart.FullPath.Contains("Task"))
                {
                    Is_Local = null;
                }

                if (Is_Local == null)
                {
                    PartNo = Path.GetFileNameWithoutExtension(displayPart.FullPath);
                }
                else
                {
                    PartNo = sPartInfo.PartNo;
                }

                //取得所有GroupAry，用來判斷Group的Type決定是NC、Tool、Geometry
                NXOpen.CAM.NCGroup[] NCGroupAry = displayPart.CAMSetup.CAMGroupCollection.ToArray();
                //取得所有OperationAry
                NXOpen.CAM.Operation[] OperationAry = displayPart.CAMSetup.CAMOperationCollection.ToArray();

                #region 取得相關資訊，填入DIC
                DicNCData = new Dictionary<string, OperData>();
                foreach (NXOpen.CAM.NCGroup ncGroup in NCGroupAry)
                {
                    if (!PreCheckNC(ncGroup))
                    {
                        continue;
                    }

                    if (!ncGroup.Name.Contains("OP"))
                    {
                        MessageBox.Show("請先手動將Group名稱：" + ncGroup.Name + "，改為正確格式，再重新啟動功能！");
                        return false;
                    }

                    //取得此NCGroup下的所有Oper
                    CAMObject[] OperGroup = ncGroup.GetMembers();

                    foreach (NXOpen.CAM.Operation item in OperGroup)
                    {
                        bool cheValue;
                        OperData sOperData = new OperData();
                        cheValue = DicNCData.TryGetValue(ncGroup.Name, out sOperData);
                        if (!cheValue)
                        {
                            sOperData.OperName = item.Name;
                            sOperData.ToolNumber = "T" + CaxOper.AskOperToolNumber(item);
                            sOperData.ToolName = CaxOper.AskOperToolNameFromTag(item.Tag);
                            sOperData.HolderDescription = CaxOper.AskOperHolderDescription(item);
                            //sOperData.CuttingLength = Convert.ToDouble(CaxOper.AskOperTotalCuttingLength(item)).ToString("f3");
                            //sOperData.ToolFeed = Math.Round(Convert.ToDouble(CaxOper.AskOperToolFeed(item)), 3, MidpointRounding.AwayFromZero).ToString();
                            //sOperData.CuttingTime = Math.Ceiling((Convert.ToDouble(CaxOper.AskOperTotalCuttingTime(item)) * 60)).ToString();//因為進給單位mmpm，距離單位mm，將進給的60放來這邊乘
                            //sOperData.ToolSpeed = CaxOper.AskOperToolSpeed(item);
                            //sOperData.PartStock = StockStr;
                            //sOperData.PartFloorStock = FloorstockStr;
                            DicNCData.Add(ncGroup.Name, sOperData);
                        }
                        else
                        {
                            sOperData.OperName = sOperData.OperName + "," + item.Name;
                            sOperData.ToolNumber = sOperData.ToolNumber + "," + "T" + CaxOper.AskOperToolNumber(item);
                            sOperData.ToolName = sOperData.ToolName + "," + CaxOper.AskOperToolNameFromTag(item.Tag);
                            sOperData.HolderDescription = sOperData.HolderDescription + "," + CaxOper.AskOperHolderDescription(item);
                            //sOperData.CuttingLength = sOperData.CuttingLength + "," + Convert.ToDouble(CaxOper.AskOperTotalCuttingLength(item)).ToString("f3");
                            //sOperData.ToolFeed = sOperData.ToolFeed + "," + Math.Round(Convert.ToDouble(CaxOper.AskOperToolFeed(item)), 3, MidpointRounding.AwayFromZero).ToString();
                            //sOperData.CuttingTime = sOperData.CuttingTime + "," + Math.Ceiling((Convert.ToDouble(CaxOper.AskOperTotalCuttingTime(item)) * 60)).ToString();//因為進給單位mmpm，距離單位mm，將進給的60放來這邊乘
                            //sOperData.ToolSpeed = sOperData.ToolSpeed + "," + CaxOper.AskOperToolSpeed(item);
                            //sOperData.PartStock = sOperData.PartStock + "," + StockStr;
                            //sOperData.PartFloorStock = sOperData.PartFloorStock + "," + FloorstockStr;
                            DicNCData[ncGroup.Name] = sOperData;
                        }
                    }
                }

                //將DicProgName的key存入程式群組下拉選單中
                foreach (KeyValuePair<string, OperData> kvp in DicNCData)
                {
                    comboBoxNCName.Items.Add(kvp.Key);
                }
                #endregion
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        private static bool GetToolListPath(out string ToolListPath)
        {
            ToolListPath = "";
            try
            {
                cMETE_Download_Upload_Path = new METE_Download_Upload_Path();
                Is_Local = Environment.GetEnvironmentVariable("UGII_ENV_FILE");
                if (Is_Local != null)
                {
                    //取得Server的ToolListPath.xls路徑
                    ToolListPath = string.Format(@"{0}\{1}\{2}\{3}", CaxEnv.GetGlobaltekEnvDir(), "TE_Config", "Config", "ToolList.xls");

                    //取得METEDownload_Upload.dat
                    status = CaxGetDatData.GetMETEDownload_Upload(out cMETE_Download_Upload_Path);
                    if (!status)
                    {
                        MessageBox.Show("取得METEDownload_Upload.dat失敗");
                        return false;
                    }
                }
                else
                {
                    ToolListPath = string.Format(@"{0}\{1}", "D:", "ToolList.xls");
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
            
        }
        private void ExportToolListDlg_Load(object sender, EventArgs e)
        {
            //建立panel物件
            panel = SGC.PrimaryGrid;

            if (!Doit(comboBoxNCName))
            {
                return;
            }
        }

        private void comboBoxNCName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空superGrid資料
            panel.Rows.Clear();
            //取得comboBox資料
            CurrentNCName = comboBoxNCName.Text;

            #region 建立ToolListFolder資料夾
            if (Is_Local != null)
            {
                ToolListFolder = string.Format(@"{0}\{1}_ToolList", cMETE_Download_Upload_Path.Local_Folder_CAM, CurrentNCName);
            }
            else
            {
                ToolListFolder = string.Format(@"{0}\{1}_ToolList", Path.GetDirectoryName(displayPart.FullPath), CurrentNCName);
            }
            if (!Directory.Exists(ToolListFolder))
            {
                System.IO.Directory.CreateDirectory(ToolListFolder);
            }
            #endregion

            //變更路徑
            string[] FolderFile = System.IO.Directory.GetFileSystemEntries(ToolListFolder, "*.xls");
            OutputPath = string.Format(@"{0}\{1}", ToolListFolder
                                                      , PartNo + "_" + CurrentNCName + "_" + (FolderFile.Length + 1) + ".xls");

            //拆群組名稱字串取得製程序(EX：OP210=>210)
            //string[] splitCurrentNCName = CurrentNCName.Split('_');
            //string OperNum = Regex.Replace(splitCurrentNCName[0], "[^0-9]", "");

            #region 填值到SuperGridPanel

            GridRow row = new GridRow();
            foreach (KeyValuePair<string, OperData> kvp in DicNCData)
            {
                if (CurrentNCName != kvp.Key)
                {
                    continue;
                }
                string[] splitOperName = kvp.Value.OperName.Split(',');
                string[] splitOperToolNumber = kvp.Value.ToolNumber.Split(',');
                string[] splitToolName = kvp.Value.ToolName.Split(',');
                string[] splitHolderDescription = kvp.Value.HolderDescription.Split(',');
                //string[] splitOperCuttingLength = kvp.Value.CuttingLength.Split(',');
                //string[] splitOperToolFeed = kvp.Value.ToolFeed.Split(',');
                //string[] splitOperCuttingTime = kvp.Value.CuttingTime.Split(',');
                //string[] splitOperToolSpeed = kvp.Value.ToolSpeed.Split(',');

                for (int i = 0; i < splitOperName.Length; i++)
                {
                    row = new GridRow(splitOperName[i], splitOperToolNumber[i], splitToolName[i], splitHolderDescription[i]);
                    panel.Rows.Add(row);
                }
            }

            #endregion
        }

        private void OK_Click(object sender, EventArgs e)
        {
            try
            {
                #region 檢查PC有無Excel在執行
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
                    MessageBox.Show("請先關閉所有Excel再重新執行輸出，如沒有EXCEL在執行，請開啟工作管理員關閉背景EXCEL");
                    return;
                }
                #endregion

                excelApp = new ApplicationClass();
                book = null;
                sheet = null;
                oRng = null;

                excelApp.Visible = false;
                book = excelApp.Workbooks.Open(ToolListPath);
                sheet = (Worksheet)book.Sheets[1];
                oRng = (Range)sheet.Cells;
                oRng[4, 2] = PartNo;

                //Insert所需欄位並填入資料
                int CurrentRow = 7, OpNameColumn = 1, ToolNumberColumn = 2, ToolNameColumn = 3, ToolDescColumn = 7;

                for (int i = 1; i < panel.Rows.Count; i++)
                {
                    oRng = (Range)sheet.Range["A8"].EntireRow;
                    oRng.Insert(Type.Missing,Type.Missing);
                    oRng = (Range)sheet.Range["A9"].EntireRow;
                    Range oRng1 = sheet.get_Range(sheet.Cells[8, 1], sheet.Cells[8, 10]);
                    oRng.EntireRow.Copy(oRng1);
                }

                
                for (int i = 0; i < panel.Rows.Count; i++)
                {
                    oRng = (Range)sheet.Cells;
                    //取得Row,Column
                    CurrentRow = CurrentRow + 1;

                    oRng[CurrentRow, OpNameColumn] = panel.GetCell(i, 0).Value.ToString();
                    oRng[CurrentRow, ToolNumberColumn] = panel.GetCell(i, 1).Value.ToString();
                    oRng[CurrentRow, ToolNameColumn] = panel.GetCell(i, 2).Value.ToString();
                    oRng[CurrentRow, ToolDescColumn] = panel.GetCell(i, 3).Value.ToString();
                }
                
                book.SaveAs(OutputPath, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                book.Close(Type.Missing, Type.Missing, Type.Missing);
                excelApp.Quit();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("刀具清單輸出失敗！");
                book.SaveAs(OutputPath, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                book.Close(Type.Missing, Type.Missing, Type.Missing);
                excelApp.Quit();
                this.Close();
            }
            MessageBox.Show("刀具清單輸出完成！");
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }



    }
}
