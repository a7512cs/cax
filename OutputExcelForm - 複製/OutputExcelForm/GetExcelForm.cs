using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevComponents.DotNetBar.SuperGrid;
using System.Collections;

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
        public static bool GetMEExcelForm(string ExcelType)
        {
            try
            {
                //取得Server->ME_Config->Config內的ExcelForm資料
                string ServerMEConfig = string.Format(@"{0}\{1}\{2}\{3}", GetGlobaltekEnvDir(), "ME_Config", "Config", ExcelType);
                if (!Directory.Exists(ServerMEConfig))
                {
                    MessageBox.Show(string.Format(@"{0}{1}{2}", "路徑：", ServerMEConfig, "不存在，請確定ServerIP是否正確"));
                    return false;
                }

                string[] FolderFile = System.IO.Directory.GetFileSystemEntries(ServerMEConfig, "*.xls");
                ExcelComboBox ExcelData = new ExcelComboBox(FolderFile);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
    }

    public class ExcelComboBox : GridComboBoxExEditControl
    {
        public ExcelComboBox(IEnumerable ExcelFiles)
        {
            DataSource = ExcelFiles;
        }
    }
}
