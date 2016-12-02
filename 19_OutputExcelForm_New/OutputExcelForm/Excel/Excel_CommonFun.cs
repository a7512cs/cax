using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using CaxGlobaltek;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace OutputExcelForm.Excel
{
    

    public class Excel_CommonFun
    {
        public static bool AddNewSheet(int dataCount, int oneSheetCount, ApplicationClass excelApp, Worksheet workSheet)
        {
            try
            {
                int needSheetNum = (dataCount / oneSheetCount);
                int needSheetNum_Reserve = (dataCount % oneSheetCount);
                if (needSheetNum_Reserve != 0)
                {
                    needSheetNum++;
                }
                for (int i = 1; i < needSheetNum; i++)
                {
                    workSheet.Copy(System.Type.Missing, excelApp.Workbooks[1].Worksheets[1]);
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool ModifySheet(string partNo, string section, Workbook workBook, Worksheet workSheet, Range workRange)
        {
            try
            {
                for (int i = 0; i < workBook.Worksheets.Count; i++)
                {
                    workSheet = (Worksheet)workBook.Sheets[i + 1];
                    if (section == "ShopDoc")
                    {
                        workRange = (Range)workSheet.Cells[4, 1];
                        workRange.Value = workRange.Value.ToString().Replace("1/1", (i + 1).ToString() + "/" + (workBook.Worksheets.Count).ToString());
                    }
                    else if (section == "IPQC")
                    {
                        workRange = (Range)workSheet.Cells[5, 17];
                        workRange.Value = workRange.Value.ToString().Replace("1/1", (i + 1).ToString() + "/" + (workBook.Worksheets.Count).ToString());
                    }
                    else if (section == "SelfCheck")
                    {
                        workRange = (Range)workSheet.Cells[4, 5];
                        workRange.Value = workRange.Value.ToString().Replace("1/1", (i + 1).ToString() + "/" + (workBook.Worksheets.Count).ToString());
                    }
                    
                    //Sheet的名稱不得超過31個，否則會錯
                    if (partNo.Length >= 28)
                    {
                        workSheet.Name = string.Format("({0})", (i + 1).ToString());
                    }
                    else
                    {
                        workSheet.Name = string.Format("{0}({1})", partNo, (i + 1).ToString());
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
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
        public static string GetCharacteristicSymbol(string NXData)
        {
            string ExcelSymbol = "";

            if (NXData == "Straightness")
            {
                ExcelSymbol = "u";
            }
            else if (NXData == "Flatness")
            {
                ExcelSymbol = "c";
            }
            else if (NXData == "Circularity")
            {
                ExcelSymbol = "e";
            }
            else if (NXData == "Cylindricity")
            {
                ExcelSymbol = "g";
            }
            else if (NXData == "Profile of a Line")
            {
                ExcelSymbol = "k";
            }
            else if (NXData == "Profile of a Surface")
            {
                ExcelSymbol = "d";
            }
            else if (NXData == "Angularity")
            {
                ExcelSymbol = "a";
            }
            else if (NXData == "Perpendicularity")
            {
                ExcelSymbol = "b";
            }
            else if (NXData == "Parallelism")
            {
                ExcelSymbol = "f";
            }
            else if (NXData == "Position")
            {
                ExcelSymbol = "j";
            }
            else if (NXData == "Concentricity")
            {
                ExcelSymbol = "r";
            }
            else if (NXData == "Symmetry")
            {
                ExcelSymbol = "i";
            }
            else if (NXData == "Circular Runout")
            {
                ExcelSymbol = "h";
            }
            else if (NXData == "Total Runout")
            {
                ExcelSymbol = "t";
            }

            return ExcelSymbol;
        }
        public static string GetZoneShapeSymbol(string NXData)
        {
            string ExcelSymbol = "";
            if (NXData == "Diameter")
            {
                ExcelSymbol = "n";
            }
            return ExcelSymbol;
        }
        public static bool MappingDimenData(Com_Dimension cCom_Dimension, Worksheet worksheet, int currentRow, int dimenColumn)
        {
            try
            {
                Dictionary<int, bool> TranslateWords = new Dictionary<int, bool>();
                int start = 1, length = 0;
                if (cCom_Dimension.characteristic != "" & cCom_Dimension.characteristic != null & cCom_Dimension.characteristic != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = cCom_Dimension.characteristic;
                    length = cCom_Dimension.characteristic.Length;
                    for (int i = 0; i < length;i++ )
                    {
                        TranslateWords.Add(start, true);
                        start++;
                    }
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "|";
                    TranslateWords.Add(start, false);
                    start++;
                    //strLen = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value.ToString().Length;
                    //((Range)worksheet.Cells[currentRow, dimenColumn]).Cells.Characters[start, length].Font.Name = "GDT";
                    //start = start + length;
                }
                if (cCom_Dimension.zoneShape != "" & cCom_Dimension.zoneShape != null & cCom_Dimension.zoneShape != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.zoneShape;
                    length = cCom_Dimension.zoneShape.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, true);
                        start++;
                    }
                    //int StrLen = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value.ToString().Length;
                    //((Range)worksheet.Cells[currentRow, dimenColumn]).Cells.Characters[start, length].Font.Name = "GDT";
                    //start = start + length;
                }
                if (cCom_Dimension.toleranceValue != "" & cCom_Dimension.toleranceValue != null & cCom_Dimension.toleranceValue != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.toleranceValue;
                    length = cCom_Dimension.toleranceValue.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    if (cCom_Dimension.materialModifer != "" & cCom_Dimension.materialModifer != null & cCom_Dimension.materialModifer != "None")
                    {
                    }
                    else
                    {
                        worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "|";
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    
                    //((Range)worksheet.Cells[currentRow, dimenColumn]).Cells.Characters[start, length].Font.Name = "Arial";
                    //start = start + length;
                }
                if (cCom_Dimension.materialModifer != "" & cCom_Dimension.materialModifer != null & cCom_Dimension.materialModifer != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.materialModifer;
                    length = cCom_Dimension.materialModifer.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, true);
                        start++;
                    }
                    
                    //((Range)worksheet.Cells[currentRow, dimenColumn]).Cells.Characters[start, length].Font.Name = "GDT";
                    //start = start + length;
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.materialModifer;
                }
                if (cCom_Dimension.primaryDatum != "" & cCom_Dimension.primaryDatum != null & cCom_Dimension.primaryDatum != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "|";
                    TranslateWords.Add(start, false);
                    start++;

                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.primaryDatum;
                    length = cCom_Dimension.primaryDatum.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    if (cCom_Dimension.primaryMaterialModifier != "" & cCom_Dimension.primaryMaterialModifier != null & cCom_Dimension.primaryMaterialModifier != "None")
                    {
                    }
                    else
                    {
                        worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "|";
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    
                    //((Range)worksheet.Cells[currentRow, dimenColumn]).Cells.Characters[start, length].Font.Name = "Arial";
                    //start = start + length;
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.primaryDatum;
                }
                if (cCom_Dimension.primaryMaterialModifier != "" & cCom_Dimension.primaryMaterialModifier != null & cCom_Dimension.primaryMaterialModifier != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.primaryMaterialModifier;
                    length = cCom_Dimension.primaryMaterialModifier.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, true);
                        start++;
                    }
                    
                    //((Range)worksheet.Cells[currentRow, dimenColumn]).Cells.Characters[start, length].Font.Name = "GDT";
                    //start = start + length;
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.primaryMaterialModifier;
                }
                if (cCom_Dimension.secondaryDatum != "" & cCom_Dimension.secondaryDatum != null & cCom_Dimension.secondaryDatum != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "|";
                    TranslateWords.Add(start, false);
                    start++;

                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.secondaryDatum;
                    length = cCom_Dimension.secondaryDatum.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    if (cCom_Dimension.secondaryMaterialModifier != "" & cCom_Dimension.secondaryMaterialModifier != null & cCom_Dimension.secondaryMaterialModifier != "None")
                    {
                    }
                    else
                    {
                        worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "|";
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    //((Range)worksheet.Cells[currentRow, dimenColumn]).Cells.Characters[start, length].Font.Name = "Arial";
                    //start = start + length;
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.secondaryDatum;
                }
                if (cCom_Dimension.secondaryMaterialModifier != "" & cCom_Dimension.secondaryMaterialModifier != null & cCom_Dimension.secondaryMaterialModifier != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.secondaryMaterialModifier;
                    length = cCom_Dimension.secondaryMaterialModifier.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, true);
                        start++;
                    }
                    //((Range)worksheet.Cells[currentRow, dimenColumn]).Cells.Characters[start, length].Font.Name = "GDT";
                    //start = start + length;
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.secondaryMaterialModifier;
                }
                if (cCom_Dimension.tertiaryDatum != "" & cCom_Dimension.tertiaryDatum != null & cCom_Dimension.tertiaryDatum != "None")
                {

                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "|";
                    TranslateWords.Add(start, false);
                    start++;

                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.tertiaryDatum;
                    length = cCom_Dimension.tertiaryDatum.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    if (cCom_Dimension.tertiaryMaterialModifier != "" & cCom_Dimension.tertiaryMaterialModifier != null & cCom_Dimension.tertiaryMaterialModifier != "None")
                    {
                    }
                    else
                    {
                        worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "|";
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    //((Range)worksheet.Cells[currentRow, dimenColumn]).Cells.Characters[start, length].Font.Name = "Arial";
                    //start = start + length;
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.tertiaryDatum;
                }
                if (cCom_Dimension.tertiaryMaterialModifier != "" & cCom_Dimension.tertiaryMaterialModifier != null & cCom_Dimension.tertiaryMaterialModifier != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.tertiaryMaterialModifier;
                    length = cCom_Dimension.tertiaryMaterialModifier.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, true);
                        start++;
                    }
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "|";
                    TranslateWords.Add(start, false);
                    start++;
                    //((Range)worksheet.Cells[currentRow, dimenColumn]).Cells.Characters[start, length].Font.Name = "GDT";
                    //start = start + length;
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.tertiaryMaterialModifier;
                }
                if (cCom_Dimension.aboveText != "" & cCom_Dimension.aboveText != null & cCom_Dimension.aboveText != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.aboveText;
                    length = cCom_Dimension.aboveText.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.aboveText;
                }
                if (cCom_Dimension.beforeText != "" & cCom_Dimension.beforeText != null & cCom_Dimension.beforeText != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.beforeText;
                    length = cCom_Dimension.beforeText.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.beforeText;
                }
                if (cCom_Dimension.toleranceSymbol != "" & cCom_Dimension.toleranceSymbol != null & cCom_Dimension.toleranceSymbol != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.toleranceSymbol;
                    length = cCom_Dimension.toleranceSymbol.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, true);
                        start++;
                    }
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.toleranceSymbol;
                }
                if (cCom_Dimension.mainText != "" & cCom_Dimension.mainText != null & cCom_Dimension.mainText != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.mainText;
                    length = cCom_Dimension.mainText.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.mainText;
                }
                if ((cCom_Dimension.upTolerance != "" & cCom_Dimension.upTolerance != null & cCom_Dimension.upTolerance != "None") ||
                    (cCom_Dimension.lowTolerance != "" & cCom_Dimension.upTolerance != null & cCom_Dimension.upTolerance != "None"))
                {
                    if (cCom_Dimension.upTolerance == cCom_Dimension.lowTolerance)
                    {
                        //如果上下公差相同，則加入±到字串中，所以在TranslateWords中必須加一次資訊
                        worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "±";
                        TranslateWords.Add(start, false); start++;
                        //加入公差的長度
                        worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.upTolerance;
                        length = cCom_Dimension.upTolerance.Length;
                        for (int i = 0; i < length; i++)
                        {
                            TranslateWords.Add(start, false);
                            start++;
                        }
                    }
                    else
                    {
                        if (Convert.ToDouble(cCom_Dimension.upTolerance)*10000 > 0)
                        {
                            //表示有上公差，所以加入+到字串中，所以在TranslateWords中必須加一次資訊
                            worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "+";
                            TranslateWords.Add(start, false); start++;
                            //加入公差的長度
                            worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.upTolerance;
                            length = cCom_Dimension.upTolerance.Length;
                            for (int i = 0; i < length; i++)
                            {
                                TranslateWords.Add(start, false);
                                start++;
                            }
                        }
                        if (Convert.ToDouble(cCom_Dimension.lowTolerance) * 10000 > 0)
                        {
                            worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "/";
                            TranslateWords.Add(start, false);
                            start++;
                            //表示有上公差，所以加入+到字串中，所以在TranslateWords中必須加一次資訊
                            worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "-";
                            TranslateWords.Add(start, false); start++;
                            //加入公差的長度
                            worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.lowTolerance;
                            length = cCom_Dimension.lowTolerance.Length;
                            for (int i = 0; i < length; i++)
                            {
                                TranslateWords.Add(start, false);
                                start++;
                            }
                        }
                    }
                    
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + "+" + cCom_Dimension.upTolerance;
                }
                if (cCom_Dimension.minTolerance != "" & cCom_Dimension.minTolerance != null & cCom_Dimension.minTolerance != "None")
                {
                    //先加入"("符號
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "(";
                    TranslateWords.Add(start, false);
                    start++;
                    //再加入最小公差
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.minTolerance;
                    length = cCom_Dimension.minTolerance.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    
                }
                if (cCom_Dimension.maxTolerance != "" & cCom_Dimension.maxTolerance != null & cCom_Dimension.maxTolerance != "None")
                {
                    //先加入"~"符號
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + "~";
                    TranslateWords.Add(start, false);
                    start++;
                    //再加入最小公差
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.maxTolerance;
                    length = cCom_Dimension.maxTolerance.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    //再加入")"符號
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + ")";
                    TranslateWords.Add(start, false);
                    start++;
                }
                //if (cCom_Dimension.lowTolerance != "" & cCom_Dimension.lowTolerance != null & cCom_Dimension.lowTolerance != "None")
                //{
                //    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.lowTolerance;
                //    length = cCom_Dimension.lowTolerance.Length;
                //    for (int i = 0; i < length; i++)
                //    {
                //        TranslateWords.Add(start, false);
                //        start++;
                //    }
                //}
                if (cCom_Dimension.x != "" & cCom_Dimension.x != null & cCom_Dimension.x != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.x;
                    length = cCom_Dimension.x.Length;
                    for (int i = 0; i < length; i++)
                    {
                        TranslateWords.Add(start, false);
                        start++;
                    }
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.x.ToUpper();
                }
                if (cCom_Dimension.chamferAngle != "" & cCom_Dimension.chamferAngle != null & cCom_Dimension.chamferAngle != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.chamferAngle;
                    length = cCom_Dimension.chamferAngle.Length;
                    char[] splitText = cCom_Dimension.chamferAngle.ToCharArray();
                    for (int i = 0; i < length; i++)
                    {
                        if (splitText[i] == '~')
                        {
                            TranslateWords.Add(start, true);
                        }
                        else
                        {
                            TranslateWords.Add(start, false);
                        }
                        start++;
                    }
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.chamferAngle;
                }
                if (cCom_Dimension.afterText != "" & cCom_Dimension.afterText != null & cCom_Dimension.afterText != "None")
                {
                    worksheet.Cells[currentRow, dimenColumn] = ((Range)worksheet.Cells[currentRow, dimenColumn]).Value + cCom_Dimension.afterText;
                    length = cCom_Dimension.afterText.Length;
                    //MessageBox.Show(length.ToString());
                    char[] splitText = cCom_Dimension.afterText.ToCharArray();
                    //MessageBox.Show(splitText.Length.ToString());
                    for (int i = 0; i < length; i++)
                    {
                        //MessageBox.Show(splitText[i].ToString());
                        if (splitText[i] == '~')
                        {
                            TranslateWords.Add(start, true);
                        }
                        else
                        {
                            TranslateWords.Add(start, false);
                        }
                        start++;
                    }
                    //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.afterText.ToUpper();
                }

                foreach (KeyValuePair<int, bool> kvp in TranslateWords)
                {
                    if (kvp.Value)
                    {
                        ((Range)worksheet.Cells[currentRow, dimenColumn]).Cells.Characters[kvp.Key, 1].Font.Name = "GDT";
                    }
                    else
                    {
                        ((Range)worksheet.Cells[currentRow, dimenColumn]).Cells.Characters[kvp.Key, 1].Font.Name = "Arial";
                    }

                }
                
                /*
                if (cCom_Dimension.dimensionType == "NXOpen.Annotations.DraftingFcf")
                {
                    #region InitialData
                    if (cCom_Dimension.characteristic != "")
                    {
                        //workRange[currentRow, dimenColumn] = GetCharacteristicSymbol(cCom_Dimension.characteristic);
                        workRange[currentRow, dimenColumn] = cCom_Dimension.characteristic;
                    }
                    if (cCom_Dimension.zoneShape != "")
                    {
                        //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + GetZoneShapeSymbol(cCom_Dimension.zoneShape);
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.zoneShape;
                    }
                    if (cCom_Dimension.toleranceValue != "")
                    {
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.toleranceValue;
                    }
                    if (cCom_Dimension.materialModifer != "" & cCom_Dimension.materialModifer != "None")
                    {
                        string ValueStr = cCom_Dimension.materialModifer;
                        if (ValueStr == "LeastMaterialCondition")
                        {
                            ValueStr = "l";
                        }
                        else if (ValueStr == "MaximumMaterialCondition")
                        {
                            ValueStr = "m";
                        }
                        else if (ValueStr == "RegardlessOfFeatureSize")
                        {
                            ValueStr = "s";
                        }
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + ValueStr;
                    }
                    #endregion

                    #region Primary
                    if (cCom_Dimension.primaryDatum != "")
                    {
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.primaryDatum;
                    }
                    if (cCom_Dimension.primaryMaterialModifier != "" & cCom_Dimension.primaryMaterialModifier != "None")
                    {
                        string ValueStr = cCom_Dimension.primaryMaterialModifier;
                        if (ValueStr == "LeastMaterialCondition")
                        {
                            ValueStr = "l";
                        }
                        else if (ValueStr == "MaximumMaterialCondition")
                        {
                            ValueStr = "m";
                        }
                        else if (ValueStr == "RegardlessOfFeatureSize")
                        {
                            ValueStr = "s";
                        }
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + ValueStr;
                    }
                    #endregion
                    
                    #region Secondary
                    if (cCom_Dimension.secondaryDatum != "")
                    {
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.secondaryDatum;
                    }
                    if (cCom_Dimension.secondaryMaterialModifier != "" & cCom_Dimension.secondaryMaterialModifier != "None")
                    {
                        string ValueStr = cCom_Dimension.secondaryMaterialModifier;
                        if (ValueStr == "LeastMaterialCondition")
                        {
                            ValueStr = "l";
                        }
                        else if (ValueStr == "MaximumMaterialCondition")
                        {
                            ValueStr = "m";
                        }
                        else if (ValueStr == "RegardlessOfFeatureSize")
                        {
                            ValueStr = "s";
                        }
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + ValueStr;
                    }
                    #endregion
                    
                    #region Tertiary
                    if (cCom_Dimension.tertiaryDatum != "")
                    {
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.tertiaryDatum;
                    }
                    if (cCom_Dimension.tertiaryMaterialModifier != "" & cCom_Dimension.tertiaryMaterialModifier != "None")
                    {
                        string ValueStr = cCom_Dimension.tertiaryMaterialModifier;
                        if (ValueStr == "LeastMaterialCondition")
                        {
                            ValueStr = "l";
                        }
                        else if (ValueStr == "MaximumMaterialCondition")
                        {
                            ValueStr = "m";
                        }
                        else if (ValueStr == "RegardlessOfFeatureSize")
                        {
                            ValueStr = "s";
                        }
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + ValueStr;
                    }
                    #endregion
                }
                else if (cCom_Dimension.dimensionType == "NXOpen.Annotations.Label")
                {
                    workRange[currentRow, dimenColumn] = cCom_Dimension.mainText;
                }
                else
                {
                    #region Dimension
                    
                    if (cCom_Dimension.beforeText != null)
                    {
                        //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + GetGDTWord(cCom_Dimension.beforeText);
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.beforeText;
                    }
                    if (cCom_Dimension.mainText != "")
                    {
                        //workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + GetGDTWord(cCom_Dimension.mainText);
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.mainText;
                    }
                    if (cCom_Dimension.upTolerance != "" & cCom_Dimension.toleranceType == "BilateralOneLine")
                    {
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + "±";
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.upTolerance;
                        string MaxMinStr = "(" + (Convert.ToDouble(cCom_Dimension.mainText) - Convert.ToDouble(cCom_Dimension.upTolerance)).ToString() + "-" + (Convert.ToDouble(cCom_Dimension.mainText) + Convert.ToDouble(cCom_Dimension.upTolerance)).ToString() + ")";
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + MaxMinStr;
                    }
                    else if (cCom_Dimension.upTolerance != "" & cCom_Dimension.toleranceType == "BilateralTwoLines")
                    {
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + "+";
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.upTolerance;
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + "/";
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.lowTolerance;
                        string MaxMinStr = "(" + (Convert.ToDouble(cCom_Dimension.mainText) + Convert.ToDouble(cCom_Dimension.lowTolerance)).ToString() + "-" + (Convert.ToDouble(cCom_Dimension.mainText) + Convert.ToDouble(cCom_Dimension.upTolerance)).ToString() + ")";
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + MaxMinStr;
                    }
                    else if (cCom_Dimension.upTolerance != "" & cCom_Dimension.toleranceType == "UnilateralAbove")
                    {
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + "+";
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.upTolerance;
                    }
                    else if (cCom_Dimension.upTolerance != "" & cCom_Dimension.toleranceType == "UnilateralBelow")
                    {
                        workRange[currentRow, dimenColumn] = ((Range)workRange[currentRow, dimenColumn]).Value + cCom_Dimension.lowTolerance;
                    }

                    #endregion
                }
                */
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            return true;
        }
        public static bool CheckExcelProcess()
        {
            try
            {
                //檢查PC有無Excel在執行
                foreach (var item in Process.GetProcesses())
                {
                    if (item.ProcessName == "EXCEL")
                    {
                        return false;
                    }
                    else
                    {
                        continue;
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
