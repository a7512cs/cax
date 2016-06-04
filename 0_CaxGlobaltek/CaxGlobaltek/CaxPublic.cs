using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace CaxGlobaltek
{
    #region METE_Download_Upload Path Json 結構

    public class METE_Download_Upload_Path
    {
        public string Server_IP { get; set; }
        public string Server_ShareStr { get; set; }
        public string Server_MODEL { get; set; }
        public string Server_MEDownloadPart { get; set; }
        public string Server_TEDownloadPart { get; set; }
        public string Local_IP { get; set; }
        public string Local_ShareStr { get; set; }
        public string Local_Folder_MODEL { get; set; }
        public string Local_Folder_CAM { get; set; }
        public string Local_Folder_OIS { get; set; }
    }

    #endregion

    #region METEDownloadData Json 結構

    public class CusRev
    {
        public string RevNo { get; set; }
        public List<string> OperAry1 { get; set; }
    }

    public class CusPart
    {
        public string PartNo { get; set; }
        public List<CusRev> CusRev { get; set; }
    }

    public class EntirePartAry
    {
        public string CusName { get; set; }
        public List<CusPart> CusPart { get; set; }
    }

    public class METEDownloadData
    {
        public List<EntirePartAry> EntirePartAry { get; set; }
    }

    #endregion

    public class CaxPublic
    {
        public static bool ReadMETEDownloadData(string jsonPath, out METEDownloadData cOperationArray)
        {
            cOperationArray = null;

            try
            {
                if (!System.IO.File.Exists(jsonPath))
                {
                    return false;
                }

                bool status;

                string jsonText;
                status = ReadFileDataUTF8(jsonPath, out jsonText);
                if (!status)
                {
                    return false;
                }

                cOperationArray = JsonConvert.DeserializeObject<METEDownloadData>(jsonText);
            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        public static bool ReadMETEDownloadUpload_Path(string jsonPath, out METE_Download_Upload_Path cOperationArray)
        {
            cOperationArray = null;

            try
            {
                if (!System.IO.File.Exists(jsonPath))
                {
                    return false;
                }

                bool status;

                string jsonText;
                status = ReadFileDataUTF8(jsonPath, out jsonText);
                if (!status)
                {
                    return false;
                }

                cOperationArray = JsonConvert.DeserializeObject<METE_Download_Upload_Path>(jsonText);
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

        /// <summary>
        /// fileName=(檔名+副檔名)，filePath=(路徑+檔名+副檔名)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool OpenFileDialog(out string fileName, out string filePath)
        {
            fileName = "";
            filePath = "";
            try
            {
                OpenFileDialog cOpenFileDialog = new OpenFileDialog();
                cOpenFileDialog.Filter = "Part Files (*.prt)|*.prt|All Files (*.*)|*.*";
                DialogResult result = cOpenFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    //取得檔案名稱(檔名+副檔名)
                    fileName = cOpenFileDialog.SafeFileName;
                    //取得檔案完整路徑(路徑+檔名+副檔名)
                    filePath = cOpenFileDialog.FileName;

                    //MessageBox.Show(textPartFileName.Text);
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool SaveFileDialog(string initialDire, out string filePath)
        {
            filePath = "";
            try
            {
                SaveFileDialog cSaveFileDialog = new SaveFileDialog();
                cSaveFileDialog.Filter = "Excel Files (*.xls)|*.xls";
                //cSaveFileDialog.InitialDirectory = initialDire;
                cSaveFileDialog.FileName = initialDire;
                DialogResult result = cSaveFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    filePath = cSaveFileDialog.FileName;
                }
                else
                {
                    filePath = initialDire;
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
