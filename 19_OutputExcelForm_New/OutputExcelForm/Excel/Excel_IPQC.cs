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
    public class Excel_IPQC
    {
        private static bool status;
        public static ApplicationClass excelApp = new ApplicationClass();
        public static Workbook workBook = null;
        public static Worksheet workSheet = null;
        public static Range workRange = null;
        public struct RowColumn
        {
            //表單資訊
            public int PartNoRow { get; set; }
            public int PartNoColumn { get; set; }
            public int OISRow { get; set; }
            public int OISColumn { get; set; }
            public int OISRevRow { get; set; }
            public int OISRevColumn { get; set; }
            public int DateRow { get; set; }
            public int DateColumn { get; set; }

            //泡泡位置
            public int BallonRow { get; set; }
            public int BallonColumn { get; set; }

            //泡泡相對圖表位置
            public int LocationRow { get; set; }
            public int LocationColumn { get; set; }

            //檢具
            public int GaugeRow { get; set; }
            public int GaugeColumn { get; set; }

            //頻率
            public int FrequencyRow { get; set; }
            public int FrequencyColumn { get; set; }

            //Dimension
            public int DimensionRow { get; set; }
            public int DimensionColumn { get; set; }

            /*
            //第1格
            public int CharacteristicRow { get; set; }
            public int CharacteristicColumn { get; set; }

            //第2格
            public int ZoneShapeRow { get; set; }
            public int ZoneShapeColumn { get; set; }
            public int BeforeTextRow { get; set; }
            public int BeforeTextColumn { get; set; }

            //第3格
            public int ToleranceValueRow { get; set; }
            public int ToleranceValueColumn { get; set; }
            public int MainTextRow { get; set; }
            public int MainTextColumn { get; set; }

            //第4格
            public int MaterialModifierRow { get; set; }
            public int MaterialModifierColumn { get; set; }
            public int ToleranceSymbolRow { get; set; }
            public int ToleranceSymbolColumn { get; set; }

            //第5格
            public int UpperTolRow { get; set; }
            public int UpperTolColumn { get; set; }

            //第6格
            public int PrimaryDatumRow { get; set; }
            public int PrimaryDatumColumn { get; set; }

            //第7格
            public int PrimaryMaterialModifierRow { get; set; }
            public int PrimaryMaterialModifierColumn { get; set; }

            //第8格
            public int SecondaryDatumRow { get; set; }
            public int SecondaryDatumColumn { get; set; }

            //第9格
            public int SecondaryMaterialModifierRow { get; set; }
            public int SecondaryMaterialModifierColumn { get; set; }

            //第10格
            public int TertiaryDatumRow { get; set; }
            public int TertiaryDatumColumn { get; set; }

            //第11格
            public int TertiaryMaterialModifierRow { get; set; }
            public int TertiaryMaterialModifierColumn { get; set; }

            //Max
            public int MaxRow { get; set; }
            public int MaxColumn { get; set; }

            //Min
            public int MinRow { get; set; }
            public int MinColumn { get; set; }
            */

        }

        public static void GetExcelRowColumn(int i, out RowColumn sRowColumn)
        {
            sRowColumn = new RowColumn();
            sRowColumn.PartNoRow = 5;
            sRowColumn.PartNoColumn = 4;
            sRowColumn.OISRow = 5;
            sRowColumn.OISColumn = 6;
            sRowColumn.OISRevRow = 5;
            sRowColumn.OISRevColumn = 8;
            sRowColumn.DateRow = 5;
            sRowColumn.DateColumn = 19;

            int currentNo = (i % 13);

            int RowNo = 0;

            if (currentNo == 0)
            {
                RowNo = 8;
            }
            else if (currentNo == 1)
            {
                RowNo = 9;
            }
            else if (currentNo == 2)
            {
                RowNo = 10;
            }
            else if (currentNo == 3)
            {
                RowNo = 11;
            }
            else if (currentNo == 4)
            {
                RowNo = 12;
            }
            else if (currentNo == 5)
            {
                RowNo = 13;
            }
            else if (currentNo == 6)
            {
                RowNo = 14;
            }
            else if (currentNo == 7)
            {
                RowNo = 15;
            }
            else if (currentNo == 8)
            {
                RowNo = 16;
            }
            else if (currentNo == 9)
            {
                RowNo = 17;
            }
            else if (currentNo == 10)
            {
                RowNo = 18;
            }
            else if (currentNo == 11)
            {
                RowNo = 19;
            }
            else if (currentNo == 12)
            {
                RowNo = 20;
            }
            //else if (currentNo == 13)
            //{
            //    RowNo = 21;
            //}
            //else if (currentNo == 14)
            //{
            //    RowNo = 22;
            //}
            //else if (currentNo == 15)
            //{
            //    RowNo = 23;
            //}
            //else if (currentNo == 16)
            //{
            //    RowNo = 24;
            //}
            //else if (currentNo == 17)
            //{
            //    RowNo = 25;
            //}

            sRowColumn.DimensionRow = RowNo;
            sRowColumn.DimensionColumn = 4;

            sRowColumn.GaugeRow = RowNo;
            sRowColumn.GaugeColumn = 5;

            sRowColumn.FrequencyRow = RowNo;
            sRowColumn.FrequencyColumn = 7;

            sRowColumn.BallonRow = RowNo;
            sRowColumn.BallonColumn = 2;

            sRowColumn.LocationRow = RowNo;
            sRowColumn.LocationColumn = 3;
            //sRowColumn.CharacteristicRow = RowNo;
            //sRowColumn.CharacteristicColumn = 4;

            //sRowColumn.ZoneShapeRow = RowNo;
            //sRowColumn.ZoneShapeColumn = 5;
            //sRowColumn.BeforeTextRow = RowNo;
            //sRowColumn.BeforeTextColumn = 5;

            //sRowColumn.ToleranceValueRow = RowNo;
            //sRowColumn.ToleranceValueColumn = 6;
            //sRowColumn.MainTextRow = RowNo;
            //sRowColumn.MainTextColumn = 6;

            //sRowColumn.MaterialModifierRow = RowNo;
            //sRowColumn.MaterialModifierColumn = 7;
            //sRowColumn.ToleranceSymbolRow = RowNo;
            //sRowColumn.ToleranceSymbolColumn = 7;

            //sRowColumn.UpperTolRow = RowNo;
            //sRowColumn.UpperTolColumn = 8;

            //sRowColumn.PrimaryDatumRow = RowNo;
            //sRowColumn.PrimaryDatumColumn = 9;

            //sRowColumn.PrimaryMaterialModifierRow = RowNo;
            //sRowColumn.PrimaryMaterialModifierColumn = 10;

            //sRowColumn.SecondaryDatumRow = RowNo;
            //sRowColumn.SecondaryDatumColumn = 11;

            //sRowColumn.SecondaryMaterialModifierRow = RowNo;
            //sRowColumn.SecondaryMaterialModifierColumn = 12;

            //sRowColumn.TertiaryDatumRow = RowNo;
            //sRowColumn.TertiaryDatumColumn = 13;

            //sRowColumn.TertiaryMaterialModifierRow = RowNo;
            //sRowColumn.TertiaryMaterialModifierColumn = 14;

            //sRowColumn.MaxRow = RowNo;
            //sRowColumn.MaxColumn = 15;

            //sRowColumn.MinRow = RowNo;
            //sRowColumn.MinColumn = 16;

            //sRowColumn.GaugeRow = RowNo;
            //sRowColumn.GaugeColumn = 17;

            //sRowColumn.FrequencyRow = RowNo;
            //sRowColumn.FrequencyColumn = 19;

            //sRowColumn.BallonRow = RowNo;
            //sRowColumn.BallonColumn = 2;

            //sRowColumn.LocationRow = RowNo;
            //sRowColumn.LocationColumn = 3;
        }
        public static bool CreateIPQCExcel_XinWu(string partNo, string cusVer, string opVer, string op1, DB_MEMain sDB_MEMain, IList<Com_Dimension> cCom_Dimension)
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

                //建立Sheet頁數符合所有的Dimension
                status = Excel_CommonFun.AddNewSheet(cCom_Dimension.Count, 13, excelApp, workSheet);
                if (!status)
                {
                    MessageBox.Show("建立Sheet頁失敗，請聯繫開發工程師");
                    workBook.SaveAs(sDB_MEMain.excelTemplateFilePath, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing
                        , XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    workBook.Close(Type.Missing, Type.Missing, Type.Missing);
                    excelApp.Quit();
                    return false;
                }

                //修改每一個Sheet名字和頁數
                status = Excel_CommonFun.ModifySheet(partNo, "IPQC", workBook, workSheet, workRange);
                if (!status)
                {
                    MessageBox.Show("修改Sheet名字和頁數失敗，請聯繫開發工程師");
                    //workBook.SaveAs(sDB_MEMain.excelTemplateFilePath, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing
                    //    , XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    workBook.Close(Type.Missing, Type.Missing, Type.Missing);
                    excelApp.Quit();
                    return false;
                }

                RowColumn sRowColumn = new RowColumn();
                int currentSheet_Value;
                for (int i = 0; i < cCom_Dimension.Count; i++)
                {
                    GetExcelRowColumn(i, out sRowColumn);
                    currentSheet_Value = (i / 13);
                    if (currentSheet_Value == 0)
                    {
                        workSheet = (Worksheet)workBook.Sheets[1];
                    }
                    else
                    {
                        workSheet = (Worksheet)workBook.Sheets[currentSheet_Value + 1];
                    }
                    workRange = (Range)workSheet.Cells; /*workSheet.Range[sRowColumn.DimensionRow, sRowColumn.DimensionColumn].Characters[1]*/

                    status = Excel_CommonFun.MappingDimenData(cCom_Dimension[i], workSheet, sRowColumn.DimensionRow, sRowColumn.DimensionColumn);
                    if (!status)
                    {
                        MessageBox.Show("MappingDimenData時發生錯誤，請聯繫開發工程師");
                        //workBook.SaveAs(sDB_MEMain.excelTemplateFilePath, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing
                        //, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                        workBook.Close(Type.Missing, Type.Missing, Type.Missing);
                        excelApp.Quit();
                        return false;
                    }

                    #region 檢具、頻率、Max、Min、泡泡、泡泡位置、料號、日期
                    workRange[sRowColumn.GaugeRow, sRowColumn.GaugeColumn] = cCom_Dimension[i].instrument;
                    workRange[sRowColumn.FrequencyRow, sRowColumn.FrequencyColumn] = cCom_Dimension[i].frequency;
                    workRange[sRowColumn.BallonRow, sRowColumn.BallonColumn] = cCom_Dimension[i].ballon;
                    workRange[sRowColumn.LocationRow, sRowColumn.LocationColumn] = cCom_Dimension[i].location;
                    workRange[sRowColumn.PartNoRow, sRowColumn.PartNoColumn] = partNo;
                    workRange[sRowColumn.OISRow, sRowColumn.OISColumn] = op1;
                    workRange[sRowColumn.OISRevRow, sRowColumn.OISRevColumn] = cusVer;
                    workRange[sRowColumn.DateRow, sRowColumn.DateColumn] = DateTime.Now.ToShortDateString();
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
                                                    , op1 + "_" + "IPQC" + ".xls");

                workBook.SaveAs(OutputPath, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing
                    , XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workBook.Close(Type.Missing, Type.Missing, Type.Missing);
                excelApp.Quit();
            }
            catch (System.Exception ex)
            {
                //workBook.SaveAs(sDB_MEMain.excelTemplateFilePath, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing
                //        , XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workBook.Close(Type.Missing, Type.Missing, Type.Missing);
                excelApp.Quit();
            	return false;
            }
            return true;
        }



    }
}
