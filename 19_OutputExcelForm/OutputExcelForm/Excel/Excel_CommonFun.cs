using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using CaxGlobaltek;
using System.Diagnostics;

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
        public static bool MappingDimenData(Com_Dimension cCom_Dimension, Range workRange, int currentRow, int dimenColumn)
        {
            try
            {
                if (cCom_Dimension.dimensionType == "NXOpen.Annotations.DraftingFcf")
                {
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
            }
            catch (System.Exception ex)
            {
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
