using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevComponents.DotNetBar.SuperGrid;
using System.Collections;
using CaxGlobaltek;

namespace OutputExcelForm
{
    public class GetExcelForm
    {
        private static bool status;
        private static string ServerEnvVari = "Globaltek_Server_Env";
        public static string GetGlobaltekEnvDir()
        {
            string GlobaltekEnvDir = Environment.GetEnvironmentVariable(ServerEnvVari);
            return GlobaltekEnvDir;
        }
        public static bool GetMEExcelForm(string ExcelType, out List<string> ExcelData)
        {
            ExcelData = new List<string>();
            try
            {
                //取得Server->ME_Config->Config內的ExcelForm資料
                OutputForm.serverMEConfig = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", "\\\\192.168.31.55", "cax", "Globaltek", "ME_Config", "Config", ExcelType);
                //string ServerMEConfig = string.Format(@"{0}\{1}\{2}\{3}", GetGlobaltekEnvDir(), "ME_Config", "Config", ExcelType);
                if (!Directory.Exists(OutputForm.serverMEConfig))
                {
                    MessageBox.Show(string.Format(@"{0}{1}{2}", "路徑：", OutputForm.serverMEConfig, "不存在，請確定ServerIP是否正確"));
                    return false;
                }
                string[] FolderFile = System.IO.Directory.GetFileSystemEntries(OutputForm.serverMEConfig, "*.xls");
                //List<string> ExcelData = new List<string>();
                for (int i = 0; i < FolderFile.Length;i++ )
                {
                    ExcelData.Add(Path.GetFileNameWithoutExtension(FolderFile[i]));
                }
                //foreach (string i in FolderFile)
                //{
                //    ExcelData.Add(Path.GetFileNameWithoutExtension(i));
                //}
                //ExcelComboBox ExcelData1 = new ExcelComboBox(ExcelData.ToArray());
                
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool GetTEExcelForm(string ExcelType, out List<string> ExcelData)
        {
            ExcelData = new List<string>();
            try
            {
                //取得Server->TE_Config->Config內的ExcelForm資料
                OutputForm.serverTEConfig = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", "\\\\192.168.31.55", "cax", "Globaltek", "TE_Config", "Config", ExcelType);
                //string ServerMEConfig = string.Format(@"{0}\{1}\{2}\{3}", GetGlobaltekEnvDir(), "ME_Config", "Config", ExcelType);
                if (!Directory.Exists(OutputForm.serverTEConfig))
                {
                    MessageBox.Show(string.Format(@"{0}{1}{2}", "路徑：", OutputForm.serverTEConfig, "不存在，請確定ServerIP是否正確"));
                    return false;
                }
                string[] FolderFile = System.IO.Directory.GetFileSystemEntries(OutputForm.serverTEConfig, "*.xls");
                //List<string> ExcelData = new List<string>();
                for (int i = 0; i < FolderFile.Length; i++)
                {
                    ExcelData.Add(Path.GetFileNameWithoutExtension(FolderFile[i]));
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool InsertDataToExcel()
        {
            try
            {
                foreach (KeyValuePair<DB_MEMain, IList<Com_Dimension>> kvp in OutputForm.DicDimenData)
                {
                    if (kvp.Key.comMEMain.sysMEExcel.meExcelType == "FAI")
                    {
                        if (kvp.Key.factory == "XinWu_FAI")
                        {
                            status = Excel_FAI.CreateFAIExcel_XinWu(kvp.Key, kvp.Value);
                            if (!status)
                            {
                                MessageBox.Show("輸出FAIExcel_XinWu版失敗，請聯繫開發工程師");
                                return false;
                            }
                        }
                        else if (kvp.Key.factory == "WuXi_FAI")
                        {

                        }
                        else if (kvp.Key.factory == "XiAn_FAI")
                        {
                        }

                    }
                    else if (kvp.Key.comMEMain.sysMEExcel.meExcelType == "FQC")
                    {
                    }
                    else if (kvp.Key.comMEMain.sysMEExcel.meExcelType == "IQC")
                    {
                    }
                    else if (kvp.Key.comMEMain.sysMEExcel.meExcelType == "IPQC")
                    {
                        MessageBox.Show("IPQC");
                    }
                    else if (kvp.Key.comMEMain.sysMEExcel.meExcelType == "SelfCheck")
                    {
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
