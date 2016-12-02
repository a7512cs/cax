using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevComponents.DotNetBar.SuperGrid;
using System.Collections;
using CaxGlobaltek;
using OutputExcelForm.Excel;

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
                string serverMEConfig = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", "\\\\192.168.31.55", "cax", "Globaltek", "ME_Config", "Config", ExcelType);
                //string serverMEConfig = string.Format(@"{0}\{1}\{2}\{3}", GetGlobaltekEnvDir(), "ME_Config", "Config", ExcelType);
                if (!Directory.Exists(serverMEConfig))
                {
                    MessageBox.Show(string.Format(@"{0}{1}{2}", "路徑：", serverMEConfig, "不存在，請確定ServerIP是否正確"));
                    return false;
                }
                string[] FolderFile = System.IO.Directory.GetFileSystemEntries(serverMEConfig, "*.xls");
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
                string serverTEConfig = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", "\\\\192.168.31.55", "cax", "Globaltek", "TE_Config", "Config", ExcelType);
                //string serverTEConfig = string.Format(@"{0}\{1}\{2}\{3}", GetGlobaltekEnvDir(), "TE_Config", "Config", ExcelType);
                if (!Directory.Exists(serverTEConfig))
                {
                    MessageBox.Show(string.Format(@"{0}{1}{2}", "路徑：", serverTEConfig, "不存在，請確定ServerIP是否正確"));
                    return false;
                }
                string[] FolderFile = System.IO.Directory.GetFileSystemEntries(serverTEConfig, "*.xls");
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
        public static bool InsertDataToMEExcel(string partNo, string cusVer, string opVer, string op1)
        {
            try
            {
                foreach (KeyValuePair<DB_MEMain, IList<Com_Dimension>> kvp in OutputForm.DicDimensionData)
                {
                    if (kvp.Key.comMEMain.sysMEExcel.meExcelType == "FAI")
                    {
                        #region FAI
                        if (kvp.Key.factory == "XinWu_FAI")
                        {
                            status = Excel_FAI.CreateFAIExcel_XinWu(partNo, cusVer, opVer, op1, kvp.Key, kvp.Value);
                            if (!status)
                            {
                                MessageBox.Show("輸出XinWu_FAI失敗，請聯繫開發工程師");
                                return false;
                            }
                        }
                        else if (kvp.Key.factory == "WuXi_FAI")
                        {

                        }
                        else if (kvp.Key.factory == "XiAn_FAI")
                        {
                        }
                        #endregion
                    }
                    else if (kvp.Key.comMEMain.sysMEExcel.meExcelType == "FQC")
                    {
                        #region FQC
                        if (kvp.Key.factory == "XinWu_FQC")
                        {
                            status = Excel_FQC.CreateFQCExcel_XinWu(partNo, cusVer, opVer, op1, kvp.Key, kvp.Value);
                            if (!status)
                            {
                                MessageBox.Show("輸出XinWu_FQC失敗，請聯繫開發工程師");
                                return false;
                            }
                        }
                        else if (kvp.Key.factory == "WuXi_FQC")
                        {

                        }
                        else if (kvp.Key.factory == "XiAn_FQC")
                        {
                        }
                        #endregion
                    }
                    else if (kvp.Key.comMEMain.sysMEExcel.meExcelType == "IQC")
                    {
                        #region IQC
                        if (kvp.Key.factory == "XinWu_IQC")
                        {
                            status = Excel_IQC.CreateIQCExcel_XinWu(partNo, cusVer, opVer, op1, kvp.Key, kvp.Value);
                            if (!status)
                            {
                                MessageBox.Show("輸出XinWu_IQC失敗，請聯繫開發工程師");
                                return false;
                            }
                        }
                        else if (kvp.Key.factory == "WuXi_IQC")
                        {

                        }
                        else if (kvp.Key.factory == "XiAn_IQC")
                        {
                        }
                        #endregion
                    }
                    else if (kvp.Key.comMEMain.sysMEExcel.meExcelType == "IPQC")
                    {
                        #region IPQC
                        if (kvp.Key.factory == "XinWu_IPQC")
                        {
                            status = Excel_IPQC.CreateIPQCExcel_XinWu(partNo, cusVer, opVer, op1, kvp.Key, kvp.Value);
                            if (!status)
                            {
                                MessageBox.Show("輸出XinWu_IPQC失敗，請聯繫開發工程師");
                                return false;
                            }
                        }
                        else if (kvp.Key.factory == "WuXi_IPQC")
                        {

                        }
                        else if (kvp.Key.factory == "XiAn_IPQC")
                        {
                        }
                        #endregion
                    }
                    else if (kvp.Key.comMEMain.sysMEExcel.meExcelType == "SelfCheck")
                    {
                        #region SelfCheck
                        if (kvp.Key.factory == "XinWu_SelfCheck")
                        {
                            status = Excel_SelfCheck.CreateSelfCheckExcel_XinWu(partNo, cusVer, opVer, op1, kvp.Key, kvp.Value);
                            if (!status)
                            {
                                MessageBox.Show("輸出XinWu_SelfCheck失敗，請聯繫開發工程師");
                                return false;
                            }
                        }
                        else if (kvp.Key.factory == "WuXi_SelfCheck")
                        {

                        }
                        else if (kvp.Key.factory == "XiAn_SelfCheck")
                        {
                        }
                        #endregion
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool InsertDataToTEExcel(string partNo, string cusVer, string opVer, string op1)
        {
            try
            {
                foreach (KeyValuePair<DB_TEMain, IList<Com_ShopDoc>> kvp in OutputForm.DicShopDocData)
                {
                    if (kvp.Key.comTEMain.sysTEExcel.teExcelType == "ShopDoc")
                    {
                        if (kvp.Key.factory == "XinWu_ShopDoc")
                        {
                            status = Excel_ShopDoc.CreateShopDocExcel_XinWu(partNo, cusVer, opVer, op1, kvp.Key, kvp.Value);
                            if (!status)
                            {
                                MessageBox.Show("輸出XinWu_ShopDoc失敗，請聯繫開發工程師");
                                return false;
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

    }
}
