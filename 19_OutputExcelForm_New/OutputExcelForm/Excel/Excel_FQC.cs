using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CaxGlobaltek;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Windows.Forms;

namespace OutputExcelForm.Excel
{
    public class Excel_FQC
    {
        private static bool status;
        public static ApplicationClass excelApp = new ApplicationClass();
        public static Workbook workBook = null;
        public static Worksheet workSheet = null;
        public static Range workRange = null;

        public static bool CreateFQCExcel_XinWu(string partNo, string cusVer, string opVer, string op1, DB_MEMain sDB_MEMain, IList<Com_Dimension> cCom_Dimension)
        {
            try
            {
                //判斷Server的Template是否存在
                if (!File.Exists(sDB_MEMain.excelTemplateFilePath))
                {
                    return false;
                }

                //1.開啟Excel
                //2.將Excel設為不顯示
                //3.取得第一頁Sheet
                workBook = excelApp.Workbooks.Open(sDB_MEMain.excelTemplateFilePath);
                excelApp.Visible = false;
                workSheet = (Worksheet)workBook.Sheets[1];
                workRange = (Range)workSheet.Cells;
                workRange[5, 5] = partNo;

                //設定欄位的Row,Column
                int currentRow = 7, ballonColumn = 1, locationColumn = 2, dimenColumn = 3, instrumentColumn = 4;
                for (int i = 0; i < cCom_Dimension.Count; i++)
                {
                    workRange = (Range)workSheet.Cells;
                    //取得Row,Column
                    currentRow = currentRow + 1;

                    status = Excel_CommonFun.MappingDimenData(cCom_Dimension[i], workSheet, currentRow, dimenColumn);
                    //status = Excel_CommonFun.MappingDimenData(cCom_Dimension[i], workRange, currentRow, dimenColumn);
                    if (!status)
                    {
                        MessageBox.Show("MappingDimenData時發生錯誤，請聯繫開發工程師");
                        return false;
                    }

                    #region 檢具、頻率、Max、Min、泡泡、泡泡位置、料號、日期
                    workRange[currentRow, instrumentColumn] = cCom_Dimension[i].instrument;
                    workRange[currentRow, ballonColumn] = cCom_Dimension[i].ballon;
                    workRange[currentRow, locationColumn] = cCom_Dimension[i].location;
                    #endregion
                }

                //設定輸出路徑
                string OutputPath = string.Format(@"{0}\{1}_{2}_{3}_OP{4}\{5}_{6}_{7}_{8}"
                                                    , Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                                                    , partNo
                                                    , cusVer
                                                    , opVer
                                                    , op1
                                                    , partNo
                                                    , cusVer
                                                    , opVer
                                                    , op1 + "_" + "FQC" + ".xls");


                workBook.SaveAs(OutputPath, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workBook.Close(Type.Missing, Type.Missing, Type.Missing);
                excelApp.Quit();

            }
            catch (System.Exception ex)
            {
                workBook.SaveAs(sDB_MEMain.excelTemplateFilePath, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workBook.Close(Type.Missing, Type.Missing, Type.Missing);
                excelApp.Quit();
                return false;
            }
            return true;
        }
    }
}
