using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using CaxGlobaltek;
using System.IO;
using System.Windows.Forms;

namespace OutputExcelForm.Excel
{
    public class Excel_FAI
    {
        private static bool status;
        public static ApplicationClass excelApp = new ApplicationClass();
        public static Workbook workBook = null;
        public static Worksheet workSheet = null;
        public static Range workRange = null;

        public static string GetGDTWord(string NXData)
        {
            string ExcelSymbol = "";

            ExcelSymbol = NXData.Replace("<#C>", "w");
            ExcelSymbol = ExcelSymbol.Replace("<#B>", "v");
            ExcelSymbol = ExcelSymbol.Replace("<#D>", "x");
            ExcelSymbol = ExcelSymbol.Replace("<#E>", "y");
            ExcelSymbol = ExcelSymbol.Replace("<#G>", "z");
            ExcelSymbol = ExcelSymbol.Replace("<#F>", "o");
            ExcelSymbol = ExcelSymbol.Replace("<$s>", "~");
            ExcelSymbol = ExcelSymbol.Replace("<O>", "n");
            ExcelSymbol = ExcelSymbol.Replace("S<O>", "Sn");
            ExcelSymbol = ExcelSymbol.Replace("&1", "u");
            ExcelSymbol = ExcelSymbol.Replace("&2", "c");
            ExcelSymbol = ExcelSymbol.Replace("&3", "e");
            ExcelSymbol = ExcelSymbol.Replace("&4", "g");
            ExcelSymbol = ExcelSymbol.Replace("&5", "k");
            ExcelSymbol = ExcelSymbol.Replace("&6", "d");
            ExcelSymbol = ExcelSymbol.Replace("&7", "a");
            ExcelSymbol = ExcelSymbol.Replace("&8", "b");
            ExcelSymbol = ExcelSymbol.Replace("&9", "f");
            ExcelSymbol = ExcelSymbol.Replace("&10", "j");
            ExcelSymbol = ExcelSymbol.Replace("&11", "r");
            ExcelSymbol = ExcelSymbol.Replace("&12", "i");
            ExcelSymbol = ExcelSymbol.Replace("&13", "h");
            ExcelSymbol = ExcelSymbol.Replace("&15", "t");
            ExcelSymbol = ExcelSymbol.Replace("<M>", "m");
            ExcelSymbol = ExcelSymbol.Replace("<E>", "l");
            ExcelSymbol = ExcelSymbol.Replace("<S>", "s");
            ExcelSymbol = ExcelSymbol.Replace("<P>", "p");
            ExcelSymbol = ExcelSymbol.Replace("<$t>", "±");

            /*
            if (NXData.Contains("<#C>"))
            {
                ExcelSymbol = NXData.Replace("<#C>", "w");
            }
            if (NXData.Contains("<#B>"))
            {
                ExcelSymbol = NXData.Replace("<#B>", "v");
            }
            if (NXData.Contains("<#D>"))
            {
                ExcelSymbol = NXData.Replace("<#D>", "x");
            }
            if (NXData.Contains("<#E>"))
            {
                ExcelSymbol = NXData.Replace("<#E>", "y");
            }
            if (NXData.Contains("<#G>"))
            {
                ExcelSymbol = NXData.Replace("<#G>", "z");
            }
            if (NXData.Contains("<#F>"))
            {
                ExcelSymbol = NXData.Replace("<#F>", "o");
            }
            if (NXData.Contains("<$s>"))
            {
                ExcelSymbol = NXData.Replace("<$s>", "~");
            }
            if (NXData.Contains("<O>"))
            {
                ExcelSymbol = NXData.Replace("<O>", "n");
            }
            if (NXData.Contains("S<O>"))
            {
                ExcelSymbol = NXData.Replace("S<O>", "Sn");
            }
            if (NXData.Contains("&1"))
            {
                ExcelSymbol = NXData.Replace("&1", "u");
            }
            if (NXData.Contains("&2"))
            {
                ExcelSymbol = NXData.Replace("&2", "c");
            }
            if (NXData.Contains("&3"))
            {
                ExcelSymbol = NXData.Replace("&3", "e");
            }
            if (NXData.Contains("&4"))
            {
                ExcelSymbol = NXData.Replace("&4", "g");
            }
            if (NXData.Contains("&5"))
            {
                ExcelSymbol = NXData.Replace("&5", "k");
            }
            if (NXData.Contains("&6"))
            {
                ExcelSymbol = NXData.Replace("&6", "d");
            }
            if (NXData.Contains("&7"))
            {
                ExcelSymbol = NXData.Replace("&7", "a");
            }
            if (NXData.Contains("&8"))
            {
                ExcelSymbol = NXData.Replace("&8", "b");
            }
            if (NXData.Contains("&9"))
            {
                ExcelSymbol = NXData.Replace("&9", "f");
            }
            if (NXData.Contains("&10"))
            {
                ExcelSymbol = NXData.Replace("&10", "j");
            }
            if (NXData.Contains("&11"))
            {
                ExcelSymbol = NXData.Replace("&11", "r");
            }
            if (NXData.Contains("&12"))
            {
                ExcelSymbol = NXData.Replace("&12", "i");
            }
            if (NXData.Contains("&13"))
            {
                ExcelSymbol = NXData.Replace("&13", "h");
            }
            if (NXData.Contains("&15"))
            {
                ExcelSymbol = NXData.Replace("&15", "t");
            }
            if (NXData.Contains("<M>"))
            {
                ExcelSymbol = NXData.Replace("<M>", "m");
            }
            if (NXData.Contains("<E>"))
            {
                ExcelSymbol = NXData.Replace("<E>", "l");
            }
            if (NXData.Contains("<S>"))
            {
                ExcelSymbol = NXData.Replace("<S>", "s");
            }
            if (NXData.Contains("<P>"))
            {
                ExcelSymbol = NXData.Replace("<P>", "p");
            }
            if (NXData.Contains("<$t>"))
            {
                ExcelSymbol = NXData.Replace("<$t>", "±");
            }
            */

            return ExcelSymbol;
        }
        public static bool CreateFAIExcel_XinWu(string partNo, string cusVer, string opVer, string op1, DB_MEMain sDB_MEMain, IList<Com_Dimension> cCom_Dimension)
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
                #region 處理第一頁
                workRange = (Range)workSheet.Cells;
                workRange[10, 1] = partNo;
                workRange[10, 2] = sDB_MEMain.comMEMain.partDescription;
                #endregion

                //取得第二頁sheet
                workSheet = (Worksheet)workBook.Sheets[2];
                #region 處理第二頁
                workRange = (Range)workSheet.Cells;
                workRange[10, 1] = partNo;
                workRange[10, 2] = sDB_MEMain.comMEMain.partDescription;
                #endregion

                //取得第三頁sheet
                workSheet = (Worksheet)workBook.Sheets[3];
                #region 處理第三頁
                workRange = (Range)workSheet.Cells;
                workRange[10, 1] = partNo;
                workRange[10, 5] = sDB_MEMain.comMEMain.partDescription;

                //Insert所需欄位
                for (int i = 1; i < cCom_Dimension.Count; i++)
                {
                    workRange = (Range)workSheet.Range["A17"].EntireRow;
                    workRange.Insert(XlInsertShiftDirection.xlShiftDown, XlInsertFormatOrigin.xlFormatFromRightOrBelow);
                }

                //設定欄位的Row,Column
                int currentRow = 16, ballonColumn = 1, locationColumn = 2, dimenColumn = 4, instrumentColumn = 6;

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
                #endregion

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
                                                    , op1 + "_" + "FAI" + ".xls");


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
