using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Windows.Forms;
using NXOpen;

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

    #region 控制器MasterValue Json 結構

    public class Controler
    {
        public string CompanyName { get; set; }
        public string MasterValue1 { get; set; }
        public string MasterValue2 { get; set; }
        public string MasterValue3 { get; set; }
    }

    public class ControlerConfig
    {
        public List<Controler> Controler { get; set; }
    }

    #endregion

    #region DraftingConfig Json 結構

    public class Drafting
    {
        public string SheetSize { get; set; }
        public string PartDescriptionPosText { get; set; }
        public string PartDescriptionPos { get; set; }
        public string PartNumberPosText { get; set; }
        public string PartNumberPos { get; set; }
        public string RevStartPosText { get; set; }
        public string RevStartPos { get; set; }
        public string PartUnitPosText { get; set; }
        public string PartUnitPos { get; set; }
        public string RevRowHeight { get; set; }
        public string PartDescriptionFontSize { get; set; }
        public string PartNumberFontSize { get; set; }
        public string RevFontSize { get; set; }
        public string PartUnitFontSize { get; set; }
        public string TolTitle0PosText { get; set; }
        public string TolTitle0Pos { get; set; }
        public string TolTitle1PosText { get; set; }
        public string TolTitle1Pos { get; set; }
        public string TolTitle2PosText { get; set; }
        public string TolTitle2Pos { get; set; }
        public string TolTitle3PosText { get; set; }
        public string TolTitle3Pos { get; set; }
        public string TolTitle4PosText { get; set; }
        public string TolTitle4Pos { get; set; }
        public string TolValue0PosText { get; set; }
        public string TolValue0Pos { get; set; }
        public string TolValue1PosText { get; set; }
        public string TolValue1Pos { get; set; }
        public string TolValue2PosText { get; set; }
        public string TolValue2Pos { get; set; }
        public string TolValue3PosText { get; set; }
        public string TolValue3Pos { get; set; }
        public string TolValue4PosText { get; set; }
        public string TolValue4Pos { get; set; }
        public string TolFontSize { get; set; }
        public string RevDateStartPosText { get; set; }
        public string RevDateStartPos { get; set; }
        public string AuthDatePosText { get; set; }
        public string AuthDatePos { get; set; }
        public string MaterialPosText { get; set; }
        public string MaterialPos { get; set; }
        public string RevDateFontSize { get; set; }
        public string AuthDateFontSize { get; set; }
        public string MaterialFontSize { get; set; }
        public string MatMinFontSize { get; set; }
        public string PartNumberWidth { get; set; }
        public string PartDescriptionWidth { get; set; }
        public string MaterialWidth { get; set; }
        public string ProcNamePosText { get; set; }
        public string ProcNamePos { get; set; }
        public string ProcNameFontSize { get; set; }
        public string PageNumberPosText { get; set; }
        public string PageNumberPos { get; set; }
        public string PageNumberFontSize { get; set; }
        public string SecondPartNumberPosText { get; set; }
        public string SecondPartNumberPos { get; set; }
        public string SecondPageNumberPosText { get; set; }
        public string SecondPageNumberPos { get; set; }
        public string SecondPartNumberWidth { get; set; }
    }

    public class DraftingConfig
    {
        public List<Drafting> Drafting { get; set; }
    }

    #endregion

    #region test
    public class RegionX
    {
        public string Zone { get; set; }
        public string X0 { get; set; }
        public string X1 { get; set; }
    }

    public class RegionY
    {
        public string Zone { get; set; }
        public string Y0 { get; set; }
        public string Y1 { get; set; }
    }

    public class DraftingCoordinate
    {
        public string SheetSize { get; set; }
        public List<RegionX> RegionX { get; set; }
        public List<RegionY> RegionY { get; set; }
    }

    public class CoordinateData
    {
        public List<DraftingCoordinate> DraftingCoordinate { get; set; }
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

        public static bool ReadControlerConfig(string jsonPath, out ControlerConfig cControlerConfig)
        {
            cControlerConfig = null;
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

                cControlerConfig = JsonConvert.DeserializeObject<ControlerConfig>(jsonText);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool ReadDraftingConfig(string jsonPath, out DraftingConfig cDraftingConfig)
        {
            cDraftingConfig = null;
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

                cDraftingConfig = JsonConvert.DeserializeObject<DraftingConfig>(jsonText);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 預設為Part檔，使用者可自行定義，fileName=(檔名+副檔名)，filePath=(路徑+檔名+副檔名)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool OpenFileDialog(out string fileName, out string filePath, string filter = "Part Files (*.prt)|*.prt|All Files (*.*)|*.*")
        {
            fileName = "";
            filePath = "";
            try
            {
                OpenFileDialog cOpenFileDialog = new OpenFileDialog();
                cOpenFileDialog.Filter = filter;
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

        /// <summary>
        /// 多選物件
        /// </summary>
        /// <param name="objary"></param>
        /// <returns></returns>
        public static bool SelectObjects(out NXObject[] objary)
        {
            objary = new NXObject[] { };
            try
            {
                UI theUI = UI.GetUI();
                objary = new NXObject[] { };
                theUI.SelectionManager.SelectObjects("Select Object", "Select Object", Selection.SelectionScope.AnyInAssembly, true, false, out objary);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 刪除指定的Object
        /// </summary>
        /// <param name="Nxobject"></param>
        /// <returns></returns>
        public static bool DelectObject(NXObject Nxobject)
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: Edit->Delete...
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Delete");

                bool notifyOnDelete1;
                notifyOnDelete1 = theSession.Preferences.Modeling.NotifyOnDelete;

                theSession.UpdateManager.ClearErrorList();

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Delete");

                NXObject[] objects1 = new NXObject[1];
                UI theUI = UI.GetUI();

                objects1[0] = Nxobject;
                int nErrs1;
                nErrs1 = theSession.UpdateManager.AddToDeleteList(objects1);

                bool notifyOnDelete2;
                notifyOnDelete2 = theSession.Preferences.Modeling.NotifyOnDelete;

                int nErrs2;
                nErrs2 = theSession.UpdateManager.DoUpdate(markId2);

                theSession.DeleteUndoMark(markId1, null);
            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 取得圖紙各區域座標
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <param name="cCoordinateData"></param>
        /// <returns></returns>
        public static bool ReadCoordinateData(string jsonPath, out CoordinateData cCoordinateData)
        {
            cCoordinateData = null;
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

                cCoordinateData = JsonConvert.DeserializeObject<CoordinateData>(jsonText);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
