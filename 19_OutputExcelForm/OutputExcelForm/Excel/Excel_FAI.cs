using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using CaxGlobaltek;
using System.IO;

namespace OutputExcelForm
{
    public class Excel_FAI
    {
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
        public static bool CreateFAIExcel_XinWu(DB_MEMain sDB_MEMain, IList<Com_Dimension> cComDimension)
        {
            try
            {
                //判斷Server的Template是否存在
                if (!File.Exists(sDB_MEMain.excelTemplateFilePath))
                {
                    return false;
                }

                //開啟Excel
                workBook = excelApp.Workbooks.Open(sDB_MEMain.excelTemplateFilePath);

                //將Excel設為不顯示
                excelApp.Visible = false;

                //取得第一頁sheet
                workSheet = (Worksheet)workBook.Sheets[1];
                #region 處理第一頁
                workRange = (Range)workSheet.Cells;
                workRange[10, 1] = OutputForm.PartNoCombobox.Text;
                workRange[10, 2] = sDB_MEMain.comMEMain.partDescription;
                #endregion

                //取得第二頁sheet
                workSheet = (Worksheet)workBook.Sheets[2];
                #region 處理第二頁
                workRange = (Range)workSheet.Cells;
                workRange[10, 1] = OutputForm.PartNoCombobox.Text;
                workRange[10, 2] = sDB_MEMain.comMEMain.partDescription;
                #endregion

                //取得第三頁sheet
                workSheet = (Worksheet)workBook.Sheets[3];
                #region 處理第三頁
                workRange = (Range)workSheet.Cells;
                workRange[10, 1] = OutputForm.PartNoCombobox.Text;
                workRange[10, 5] = sDB_MEMain.comMEMain.partDescription;

                //Insert所需欄位
                for (int i = 1; i < cComDimension.Count; i++)
                {
                    workRange = (Range)workSheet.Range["A17"].EntireRow;
                    workRange.Insert(XlInsertShiftDirection.xlShiftDown, XlInsertFormatOrigin.xlFormatFromRightOrBelow);
                }

                //填入資料
                int currentRow = 16, ballonColumn = 1, locationColumn = 2, dimenColumn = 4, instrumentColumn = 6;

                for (int i = 0; i < cComDimension.Count; i++)
                {
                    workRange = (Range)workSheet.Cells;

                    //取得Row,Column
                    currentRow = currentRow + 1;

                    if (cComDimension[i].dimensionType == "NXOpen.Annotations.DraftingFcf")
                    {
                    }
                    else if (cComDimension[i].dimensionType == "NXOpen.Annotations.Label")
                    {
                    }
                    else
                    {
                        if (cComDimension[i].beforeText != null)
                        {
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + GetGDTWord(cComDimension[i].beforeText);
                        }
                        if (cComDimension[i].mainText != "")
                        {
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + GetGDTWord(cComDimension[i].mainText);
                        }
                        if (cComDimension[i].upTolerance != "" & cComDimension[i].toleranceType == "BilateralOneLine")
                        {
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + "±";
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cComDimension[i].upTolerance;
                            string MaxMinStr = "(" + (Convert.ToDouble(cComDimension[i].mainText) - Convert.ToDouble(cComDimension[i].upTolerance)).ToString() + "-" + (Convert.ToDouble(cComDimension[i].mainText) + Convert.ToDouble(cComDimension[i].upTolerance)).ToString() + ")";
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + MaxMinStr;
                        }
                        else if (cComDimension[i].upTolerance != "" & cComDimension[i].toleranceType == "BilateralTwoLines")
                        {
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + "+";
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cComDimension[i].upTolerance;
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + "/";
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cComDimension[i].lowTolerance;
                            string MaxMinStr = "(" + (Convert.ToDouble(cComDimension[i].mainText) + Convert.ToDouble(cComDimension[i].lowTolerance)).ToString() + "-" + (Convert.ToDouble(cComDimension[i].mainText) + Convert.ToDouble(cComDimension[i].upTolerance)).ToString() + ")";
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + MaxMinStr;
                        }
                        else if (cComDimension[i].upTolerance != "" & cComDimension[i].toleranceType == "UnilateralAbove")
                        {
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + "+";
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cComDimension[i].upTolerance;
                        }
                        else if (cComDimension[i].upTolerance != "" & cComDimension[i].toleranceType == "UnilateralBelow")
                        {
                            workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cComDimension[i].lowTolerance;
                        }
                    }

                    #region 檢具、頻率、Max、Min、泡泡、泡泡位置、料號、日期
                    workRange[currentRow, instrumentColumn] = cComDimension[i].instrument;
                    workRange[currentRow, ballonColumn] = cComDimension[i].ballon;
                    workRange[currentRow, locationColumn] = cComDimension[i].location;
                    #endregion
                }
                #endregion

                //設定輸出路徑
                string OutputPath = string.Format(@"{0}\{1}_{2}_OP{3}\{4}_{5}_{6}", Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                                                                          , OutputForm.PartNoCombobox.Text
                                                                          , OutputForm.CusVerCombobox.Text
                                                                          , OutputForm.Op1Combobox.Text
                                                                          , OutputForm.PartNoCombobox.Text 
                                                                          , OutputForm.CusVerCombobox.Text
                                                                          , OutputForm.Op1Combobox.Text + "_" + "FAI" + ".xls");


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
