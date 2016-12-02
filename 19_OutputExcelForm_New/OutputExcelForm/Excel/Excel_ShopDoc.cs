using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CaxGlobaltek;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Windows.Forms;

namespace OutputExcelForm.Excel
{
    public class Excel_ShopDoc
    {
        private static bool status;
        private static ApplicationClass excelApp = new ApplicationClass();
        private static Workbook workBook = null;
        private static Worksheet workSheet = null;
        private static Range workRange = null;
        public struct RowColumn
        {
            public int ToolNumberRow { get; set; }
            public int ToolNumberColumn { get; set; }
            public int ToolNameRow { get; set; }
            public int ToolNameColumn { get; set; }
            public int OperNameRow { get; set; }
            public int OperNameColumn { get; set; }
            public int HolderRow { get; set; }
            public int HolderColumn { get; set; }
            public int CuttingTimeRow { get; set; }
            public int CuttingTimeColumn { get; set; }
            public int ToolFeedRow { get; set; }
            public int ToolFeedColumn { get; set; }
            public int ToolSpeedRow { get; set; }
            public int ToolSpeedColumn { get; set; }
            public int OperImgToolRow { get; set; }
            public int OperImgToolColumn { get; set; }
            public int PartStockRow { get; set; }
            public int PartStockColumn { get; set; }
            public int TotalCuttingTimeRow { get; set; }
            public int TotalCuttingTimeColumn { get; set; }
            public int PartNoRow { get; set; }
            public int PartNoColumn { get; set; }
            public int PartDescRow { get; set; }
            public int PartDescColumn { get; set; }
        }
        public struct OperImgPosiSize
        {
            public float OperPosiLeft { get; set; }
            public float OperPosiTop { get; set; }
            public float OperImgWidth { get; set; }
            public float OperImgHeight { get; set; }
        }
        public struct FixImgPosiSize
        {
            public float FixPosiLeft { get; set; }
            public float FixPosiTop { get; set; }
            public float FixImgWidth { get; set; }
            public float FixImgHeight { get; set; }
        }
        public static void GetExcelRowColumn(int i, out RowColumn sRowColumn)
        {
            sRowColumn = new RowColumn();
            sRowColumn.PartNoRow = 52;
            sRowColumn.PartNoColumn = 11;
            sRowColumn.TotalCuttingTimeRow = 51;
            sRowColumn.TotalCuttingTimeColumn = 2;
            sRowColumn.PartDescRow = 51;
            sRowColumn.PartDescColumn = 11;


            int currentNo = (i % 8);

            if (currentNo == 0)
            {
                sRowColumn.PartStockRow = 27;
                sRowColumn.PartStockColumn = 4;

                sRowColumn.OperImgToolRow = 5;
                sRowColumn.OperImgToolColumn = 1;

                sRowColumn.ToolNumberRow = 23;
                sRowColumn.ToolNumberColumn = 2;

                sRowColumn.ToolNameRow = 23;
                sRowColumn.ToolNameColumn = 3;

                sRowColumn.OperNameRow = 24;
                sRowColumn.OperNameColumn = 2;

                sRowColumn.HolderRow = 25;
                sRowColumn.HolderColumn = 2;

                sRowColumn.ToolFeedRow = 27;
                sRowColumn.ToolFeedColumn = 2;

                sRowColumn.ToolSpeedRow = 27;
                sRowColumn.ToolSpeedColumn = 3;

                sRowColumn.CuttingTimeRow = 28;
                sRowColumn.CuttingTimeColumn = 2;
            }
            else if (currentNo == 1)
            {
                sRowColumn.PartStockRow = 33;
                sRowColumn.PartStockColumn = 4;

                sRowColumn.OperImgToolRow = 5;
                sRowColumn.OperImgToolColumn = 4;

                sRowColumn.ToolNumberRow = 29;
                sRowColumn.ToolNumberColumn = 2;

                sRowColumn.ToolNameRow = 29;
                sRowColumn.ToolNameColumn = 3;

                sRowColumn.OperNameRow = 30;
                sRowColumn.OperNameColumn = 2;

                sRowColumn.HolderRow = 31;
                sRowColumn.HolderColumn = 2;

                sRowColumn.ToolFeedRow = 33;
                sRowColumn.ToolFeedColumn = 2;

                sRowColumn.ToolSpeedRow = 33;
                sRowColumn.ToolSpeedColumn = 3;

                sRowColumn.CuttingTimeRow = 34;
                sRowColumn.CuttingTimeColumn = 2;
            }
            else if (currentNo == 2)
            {
                sRowColumn.PartStockRow = 39;
                sRowColumn.PartStockColumn = 4;

                sRowColumn.OperImgToolRow = 5;
                sRowColumn.OperImgToolColumn = 7;

                sRowColumn.ToolNumberRow = 35;
                sRowColumn.ToolNumberColumn = 2;

                sRowColumn.ToolNameRow = 35;
                sRowColumn.ToolNameColumn = 3;

                sRowColumn.OperNameRow = 36;
                sRowColumn.OperNameColumn = 2;

                sRowColumn.HolderRow = 37;
                sRowColumn.HolderColumn = 2;

                sRowColumn.ToolFeedRow = 39;
                sRowColumn.ToolFeedColumn = 2;

                sRowColumn.ToolSpeedRow = 39;
                sRowColumn.ToolSpeedColumn = 3;

                sRowColumn.CuttingTimeRow = 40;
                sRowColumn.CuttingTimeColumn = 2;
            }
            else if (currentNo == 3)
            {
                sRowColumn.PartStockRow = 45;
                sRowColumn.PartStockColumn = 4;

                sRowColumn.OperImgToolRow = 5;
                sRowColumn.OperImgToolColumn = 10;

                sRowColumn.ToolNumberRow = 41;
                sRowColumn.ToolNumberColumn = 2;

                sRowColumn.ToolNameRow = 41;
                sRowColumn.ToolNameColumn = 3;

                sRowColumn.OperNameRow = 42;
                sRowColumn.OperNameColumn = 2;

                sRowColumn.HolderRow = 43;
                sRowColumn.HolderColumn = 2;

                sRowColumn.ToolFeedRow = 45;
                sRowColumn.ToolFeedColumn = 2;

                sRowColumn.ToolSpeedRow = 45;
                sRowColumn.ToolSpeedColumn = 3;

                sRowColumn.CuttingTimeRow = 46;
                sRowColumn.CuttingTimeColumn = 2;
            }
            else if (currentNo == 4)
            {
                sRowColumn.PartStockRow = 27;
                sRowColumn.PartStockColumn = 8;

                sRowColumn.OperImgToolRow = 13;
                sRowColumn.OperImgToolColumn = 1;

                sRowColumn.ToolNumberRow = 23;
                sRowColumn.ToolNumberColumn = 6;

                sRowColumn.ToolNameRow = 23;
                sRowColumn.ToolNameColumn = 7;

                sRowColumn.OperNameRow = 24;
                sRowColumn.OperNameColumn = 6;

                sRowColumn.HolderRow = 25;
                sRowColumn.HolderColumn = 6;

                sRowColumn.ToolFeedRow = 27;
                sRowColumn.ToolFeedColumn = 6;

                sRowColumn.ToolSpeedRow = 27;
                sRowColumn.ToolSpeedColumn = 7;

                sRowColumn.CuttingTimeRow = 28;
                sRowColumn.CuttingTimeColumn = 6;
            }
            else if (currentNo == 5)
            {
                sRowColumn.PartStockRow = 33;
                sRowColumn.PartStockColumn = 8;

                sRowColumn.OperImgToolRow = 13;
                sRowColumn.OperImgToolColumn = 4;

                sRowColumn.ToolNumberRow = 29;
                sRowColumn.ToolNumberColumn = 6;

                sRowColumn.ToolNameRow = 29;
                sRowColumn.ToolNameColumn = 7;

                sRowColumn.OperNameRow = 30;
                sRowColumn.OperNameColumn = 6;

                sRowColumn.HolderRow = 31;
                sRowColumn.HolderColumn = 6;

                sRowColumn.ToolFeedRow = 33;
                sRowColumn.ToolFeedColumn = 6;

                sRowColumn.ToolSpeedRow = 33;
                sRowColumn.ToolSpeedColumn = 7;

                sRowColumn.CuttingTimeRow = 34;
                sRowColumn.CuttingTimeColumn = 6;
            }
            else if (currentNo == 6)
            {
                sRowColumn.PartStockRow = 39;
                sRowColumn.PartStockColumn = 8;

                sRowColumn.OperImgToolRow = 13;
                sRowColumn.OperImgToolColumn = 7;

                sRowColumn.ToolNumberRow = 35;
                sRowColumn.ToolNumberColumn = 6;

                sRowColumn.ToolNameRow = 35;
                sRowColumn.ToolNameColumn = 7;

                sRowColumn.OperNameRow = 36;
                sRowColumn.OperNameColumn = 6;

                sRowColumn.HolderRow = 37;
                sRowColumn.HolderColumn = 6;

                sRowColumn.ToolFeedRow = 39;
                sRowColumn.ToolFeedColumn = 6;

                sRowColumn.ToolSpeedRow = 39;
                sRowColumn.ToolSpeedColumn = 7;

                sRowColumn.CuttingTimeRow = 40;
                sRowColumn.CuttingTimeColumn = 6;
            }
            else if (currentNo == 7)
            {
                sRowColumn.PartStockRow = 45;
                sRowColumn.PartStockColumn = 8;

                sRowColumn.OperImgToolRow = 13;
                sRowColumn.OperImgToolColumn = 10;

                sRowColumn.ToolNumberRow = 41;
                sRowColumn.ToolNumberColumn = 6;

                sRowColumn.ToolNameRow = 41;
                sRowColumn.ToolNameColumn = 7;

                sRowColumn.OperNameRow = 42;
                sRowColumn.OperNameColumn = 6;

                sRowColumn.HolderRow = 43;
                sRowColumn.HolderColumn = 6;

                sRowColumn.ToolFeedRow = 45;
                sRowColumn.ToolFeedColumn = 6;

                sRowColumn.ToolSpeedRow = 45;
                sRowColumn.ToolSpeedColumn = 7;

                sRowColumn.CuttingTimeRow = 46;
                sRowColumn.CuttingTimeColumn = 6;
            }
        }
        public static void GetOperImgPosiAndSize(int i, out OperImgPosiSize sOperImgPosiSize)
        {
            sOperImgPosiSize = new OperImgPosiSize();
            int currentNo = (i % 8);

            if (currentNo == 0)
            {
                sOperImgPosiSize.OperPosiLeft = 5;
                sOperImgPosiSize.OperPosiTop = 118;
                sOperImgPosiSize.OperImgWidth = 160;
                sOperImgPosiSize.OperImgHeight = 115;
            }
            else if (currentNo == 1)
            {
                sOperImgPosiSize.OperPosiLeft = 185;
                sOperImgPosiSize.OperPosiTop = 118;
                sOperImgPosiSize.OperImgWidth = 160;
                sOperImgPosiSize.OperImgHeight = 115;
            }
            else if (currentNo == 2)
            {
                sOperImgPosiSize.OperPosiLeft = 365;
                sOperImgPosiSize.OperPosiTop = 118;
                sOperImgPosiSize.OperImgWidth = 160;
                sOperImgPosiSize.OperImgHeight = 115;
            }
            else if (currentNo == 3)
            {
                sOperImgPosiSize.OperPosiLeft = 540;
                sOperImgPosiSize.OperPosiTop = 118;
                sOperImgPosiSize.OperImgWidth = 160;
                sOperImgPosiSize.OperImgHeight = 115;
            }
            else if (currentNo == 4)
            {
                sOperImgPosiSize.OperPosiLeft = 10;
                sOperImgPosiSize.OperPosiTop = 265;
                sOperImgPosiSize.OperImgWidth = 160;
                sOperImgPosiSize.OperImgHeight = 115;
            }
            else if (currentNo == 5)
            {
                sOperImgPosiSize.OperPosiLeft = 190;
                sOperImgPosiSize.OperPosiTop = 265;
                sOperImgPosiSize.OperImgWidth = 160;
                sOperImgPosiSize.OperImgHeight = 115;
            }
            else if (currentNo == 6)
            {
                sOperImgPosiSize.OperPosiLeft = 370;
                sOperImgPosiSize.OperPosiTop = 265;
                sOperImgPosiSize.OperImgWidth = 160;
                sOperImgPosiSize.OperImgHeight = 115;
            }
            else if (currentNo == 7)
            {
                sOperImgPosiSize.OperPosiLeft = 545;
                sOperImgPosiSize.OperPosiTop = 265;
                sOperImgPosiSize.OperImgWidth = 160;
                sOperImgPosiSize.OperImgHeight = 115;
            }
        }
        public static void GetFixImgPosiAndSize(out FixImgPosiSize sFixImgPosiSize)
        {
            sFixImgPosiSize = new FixImgPosiSize();
            sFixImgPosiSize.FixPosiLeft = 485;
            sFixImgPosiSize.FixPosiTop = 423;
            sFixImgPosiSize.FixImgWidth = 225;
            sFixImgPosiSize.FixImgHeight = 198;
        }
        public static bool CreateShopDocExcel_XinWu(string partNo, string cusVer, string opVer, string op1, DB_TEMain sDB_TEMain, IList<Com_ShopDoc> cCom_ShopDoc)
        {
            try
            {
                //判斷Server的Template是否存在
                if (!File.Exists(sDB_TEMain.excelTemplateFilePath))
                {
                    return false;
                }

                //1.開啟Excel
                //2.將Excel設為不顯示
                //3.取得第一頁Sheet
                workBook = excelApp.Workbooks.Open(sDB_TEMain.excelTemplateFilePath);
                excelApp.Visible = false;
                workSheet = (Worksheet)workBook.Sheets[1];

                //建立Sheet頁數符合所有的Operation
                status = Excel_CommonFun.AddNewSheet(cCom_ShopDoc.Count, 8, excelApp, workSheet);
                if (!status)
                {
                    MessageBox.Show("建立Sheet頁失敗，請聯繫開發工程師");
                    return false;
                }

                //修改每一個Sheet名字和頁數
                status = Excel_CommonFun.ModifySheet(partNo, "ShopDoc", workBook, workSheet, workRange);
                if (!status)
                {
                    MessageBox.Show("修改Sheet名字和頁數失敗，請聯繫開發工程師");
                    return false;
                }

                //開始填表
                RowColumn sRowColumn;
                for (int i = 0; i < cCom_ShopDoc.Count; i++)
                {
                    GetExcelRowColumn(i, out sRowColumn);
                    //取得當前Operation該放置的Sheet
                    int currentSheet_Value = (i / 8);
                    //int currentSheet_Reserve = (i % 8);
                    if (currentSheet_Value == 0)
                    {
                        workSheet = (Worksheet)workBook.Sheets[1];
                    }
                    else
                    {
                        workSheet = (Worksheet)workBook.Sheets[currentSheet_Value + 1];
                    }

                    workRange = (Range)workSheet.Cells;
                    workRange[sRowColumn.OperImgToolRow, sRowColumn.OperImgToolColumn] = cCom_ShopDoc[i].toolNo + "_" + cCom_ShopDoc[i].operationName;
                    workRange[sRowColumn.ToolNumberRow, sRowColumn.ToolNumberColumn] = cCom_ShopDoc[i].toolNo;
                    workRange[sRowColumn.ToolNameRow, sRowColumn.ToolNameColumn] = cCom_ShopDoc[i].toolID;
                    workRange[sRowColumn.OperNameRow, sRowColumn.OperNameColumn] = cCom_ShopDoc[i].operationName;
                    workRange[sRowColumn.HolderRow, sRowColumn.HolderColumn] = cCom_ShopDoc[i].holderID;
                    workRange[sRowColumn.ToolFeedRow, sRowColumn.ToolFeedColumn] = "F：" + cCom_ShopDoc[i].feed;
                    workRange[sRowColumn.ToolSpeedRow, sRowColumn.ToolSpeedColumn] = "S：" + cCom_ShopDoc[i].speed;
                    workRange[sRowColumn.PartStockRow, sRowColumn.PartStockColumn] = cCom_ShopDoc[i].partStock;
                    workRange[sRowColumn.CuttingTimeRow, sRowColumn.CuttingTimeColumn] = cCom_ShopDoc[i].machiningtime;
                    workRange[sRowColumn.PartNoRow, sRowColumn.PartNoColumn] = partNo;
                    workRange[sRowColumn.TotalCuttingTimeRow, sRowColumn.TotalCuttingTimeColumn] = sDB_TEMain.comTEMain.totalCuttingTime;
                    workRange[sRowColumn.PartDescRow, sRowColumn.PartDescColumn] = sDB_TEMain.partDesc;

                    OperImgPosiSize sImgPosiSize = new OperImgPosiSize();
                    GetOperImgPosiAndSize(i, out sImgPosiSize);
                    if (File.Exists(cCom_ShopDoc[i].opImagePath))
                    {
                        workSheet.Shapes.AddPicture(cCom_ShopDoc[i].opImagePath, Microsoft.Office.Core.MsoTriState.msoFalse,
                                   Microsoft.Office.Core.MsoTriState.msoTrue, sImgPosiSize.OperPosiLeft,
                                   sImgPosiSize.OperPosiTop, sImgPosiSize.OperImgWidth, sImgPosiSize.OperImgHeight);
                    }
                }
                if (sDB_TEMain.comTEMain.fixtureImgPath != "")
                {
                    FixImgPosiSize sFixImgPosiSize = new FixImgPosiSize();
                    GetFixImgPosiAndSize(out sFixImgPosiSize);
                    for (int i = 0; i < workBook.Sheets.Count; i++)
                    {
                        workSheet = (Worksheet)workBook.Sheets[i + 1];

                        if (File.Exists(sDB_TEMain.comTEMain.fixtureImgPath))
                        {
                            workSheet.Shapes.AddPicture(sDB_TEMain.comTEMain.fixtureImgPath, Microsoft.Office.Core.MsoTriState.msoFalse,
                            Microsoft.Office.Core.MsoTriState.msoTrue, sFixImgPosiSize.FixPosiLeft,
                            sFixImgPosiSize.FixPosiTop, sFixImgPosiSize.FixImgWidth, sFixImgPosiSize.FixImgHeight);
                        }
                    }
                }

                //設定輸出路徑
                string OutputPath = string.Format(@"{0}\{1}_{2}_{3}_OP{4}\{5}_{6}_{7}_{8}_{9}"
                                                    , Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                                                    , partNo
                                                    , cusVer
                                                    , opVer
                                                    , op1
                                                    , partNo
                                                    , cusVer
                                                    , opVer
                                                    , op1
                                                    , sDB_TEMain.comTEMain.ncGroupName + "_" + "ShopDoc" + ".xls");

                workBook.SaveAs(OutputPath, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workBook.Close(Type.Missing, Type.Missing, Type.Missing);
                excelApp.Quit();

            }
            catch (System.Exception ex)
            {
                workBook.SaveAs(sDB_TEMain.excelTemplateFilePath, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workBook.Close(Type.Missing, Type.Missing, Type.Missing);
                excelApp.Quit();
                return false;
            }
            return true;
        }
    }
}
