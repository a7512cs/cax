using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using CaxGlobaltek;
using NXOpen.UF;
using System.Windows.Forms;

namespace MEUpload.DatabaseClass
{
    public class DimensionData
    {
        public string type { get; set; }
        
        public string aboveText { get; set; }
        public string belowText { get; set; }
        public string beforeText { get; set; }
        public string afterText { get; set; }
        public string toleranceSymbol { get; set; }
        public string mainText { get; set; }
        public string upperTol { get; set; }
        public string lowerTol { get; set; }
        public string x { get; set; }
        public string chamferAngle { get; set; }
        public string maxTolerance { get; set; }
        public string minTolerance { get; set; }
        public string tolType { get; set; }
        public string instrument { get; set; }
        public string location { get; set; }
        public string frequency { get; set; }
        public int ballonNum { get; set; }
        public string draftingVer { get; set; }
        public string draftingDate { get; set; }

        //FcfData
        public string characteristic { get; set; }
        public string zoneShape { get; set; }
        public string toleranceValue { get; set; }
        public string materialModifier { get; set; }
        public string primaryDatum { get; set; }
        public string primaryMaterialModifier { get; set; }
        public string secondaryDatum { get; set; }
        public string secondaryMaterialModifier { get; set; }
        public string tertiaryDatum { get; set; }
        public string tertiaryMaterialModifier { get; set; }
    }

    public class MEMainData
    {
        public string createDate { get; set; }
    }

    public class Database
    {
        public static Session theSession = Session.GetSession();
        public static UI theUI;
        public static UFSession theUfSession = UFSession.GetUFSession();
        public static Dictionary<MEMainData, DimensionData> DicDimenData = new Dictionary<MEMainData, DimensionData>();
        public static List<DimensionData> listDimensionData = new List<DimensionData>();

        public static bool SplitMainText(string text, out string mainText)
        {
            mainText = "";
            try
            {
                string[] splitTol = text.Split('!');
                if (splitTol.Length > 1)
                {
                    splitTol[0] = splitTol[0].Remove(0, 2);
                    splitTol[1] = splitTol[1].Remove(splitTol[1].Length - 1);
                    mainText = splitTol[0] + "-" + splitTol[1];
                }
                else
                {
                    mainText = text;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool SplitTolerance(string tolerance, out string upperTol, out string lowerTol)
        {
            upperTol = "";
            lowerTol = "";
            try
            {
                string[] splitTol = tolerance.Split('!');
                if (splitTol.Length > 1)
                {
                    upperTol = splitTol[0].Remove(0, 2);
                    upperTol = upperTol.Replace("+", "");
                    lowerTol = splitTol[1].Remove(splitTol[1].Length - 1);
                    lowerTol = lowerTol.Replace("-", "");
                }
                else if (splitTol[0].Contains("<$t>"))
                {
                    upperTol = splitTol[0].Remove(0, 4);
                    lowerTol = upperTol;
                }
                else
                {
                    upperTol = tolerance;
                    lowerTol = "";
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool MappingDimensionData(int ann_type, int ann_form, string text, ref DimensionData cDimensionData)
        {
            try
            {
                if (ann_type == 3)
                {
                    switch (ann_form)
                    {
                        case 1:
                            string mainText = "";
                            SplitMainText(text,out mainText);
                            if (cDimensionData.mainText == null)
                            {
                                cDimensionData.mainText = mainText;
                            }
                            else
                            {
                                cDimensionData.mainText = cDimensionData.mainText + "\r\n" + mainText;
                            }
                            break;
                        case 3:
                            string upperTol = "", lowerTol = "";
                            SplitTolerance(text,out upperTol,out lowerTol);
                            cDimensionData.upperTol = upperTol;
                            cDimensionData.lowerTol = lowerTol;
                            break;
                        case 5:
                            cDimensionData.toleranceSymbol = text;
                            break;
                        case 50:
                            cDimensionData.aboveText = text;
                            break;
                        case 51:
                            cDimensionData.belowText = text;
                            break;
                        case 52:
                            cDimensionData.beforeText = text;
                            break;
                        case 53:
                            cDimensionData.afterText = text;
                            break;
                        case 63:
                            cDimensionData.x = text;
                            break;
                        case 64:
                            cDimensionData.chamferAngle = text;
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool GetDimensionData(string excelType, DisplayableObject singleObj, out DimensionData cDimensionData)
        {
            cDimensionData = new DimensionData();
            try
            {
                if (excelType == "")
                {
                    return false;
                }

                string singleObjType = singleObj.GetType().ToString();
                //string[] mainText;
                //string[] dualText;


                if (excelType == "FAI")
                {
                    try { cDimensionData.instrument = singleObj.GetStringAttribute(CaxME.DimenAttr.FAI_Gauge); }
                    catch (System.Exception ex) { /*return false;*/ }

                    try { cDimensionData.frequency = singleObj.GetStringAttribute(CaxME.DimenAttr.FAI_Freq); }
                    catch (System.Exception ex) { /*return false;*/ }
                }
                else if (excelType == "IPQC")
                {
                    try { cDimensionData.instrument = singleObj.GetStringAttribute(CaxME.DimenAttr.IPQC_Gauge); }
                    catch (System.Exception ex) { return false; }

                    try { cDimensionData.frequency = singleObj.GetStringAttribute(CaxME.DimenAttr.IPQC_Freq); }
                    catch (System.Exception ex) { return false; }
                }
                else if (excelType == "IQC")
                {
                    try { cDimensionData.instrument = singleObj.GetStringAttribute(CaxME.DimenAttr.IQC_Gauge); }
                    catch (System.Exception ex) { /*return false;*/ }

                    try { cDimensionData.frequency = singleObj.GetStringAttribute(CaxME.DimenAttr.IQC_Freq); }
                    catch (System.Exception ex) { /*return false;*/ }
                }
                else if (excelType == "SelfCheck")
                {
                    try { cDimensionData.instrument = singleObj.GetStringAttribute(CaxME.DimenAttr.SelfCheck_Gauge); }
                    catch (System.Exception ex) { return false; }

                    try { cDimensionData.frequency = singleObj.GetStringAttribute(CaxME.DimenAttr.SelfCheck_Freq); }
                    catch (System.Exception ex) { return false; }
                }

                try { cDimensionData.ballonNum = Convert.ToInt32(singleObj.GetStringAttribute(CaxME.DimenAttr.BallonNum)); }
                catch (System.Exception ex) { return false; }

                try { cDimensionData.location = singleObj.GetStringAttribute(CaxME.DimenAttr.BallonLocation); }
                catch (System.Exception ex) { return false; }

                
                if (singleObj is NXOpen.Annotations.Annotation)
                {
                    if (singleObj is NXOpen.Annotations.DraftingFcf)
                    {
                        #region DraftingFcf取Text
                        
                        NXOpen.Annotations.DraftingFcf temp = (NXOpen.Annotations.DraftingFcf)singleObj;
                        CaxME.FcfData sFcfData = new CaxME.FcfData();
                        CaxME.GetFcfData(temp, out sFcfData);
                        cDimensionData.type = "NXOpen.Annotations.DraftingFcf";
                        cDimensionData.characteristic = sFcfData.Characteristic;
                        cDimensionData.zoneShape = sFcfData.ZoneShape;
                        cDimensionData.toleranceValue = sFcfData.ToleranceValue;
                        cDimensionData.materialModifier = sFcfData.MaterialModifier;
                        cDimensionData.primaryDatum = sFcfData.PrimaryDatum;
                        cDimensionData.primaryMaterialModifier = sFcfData.PrimaryMaterialModifier;
                        cDimensionData.secondaryDatum = sFcfData.SecondaryDatum;
                        cDimensionData.secondaryMaterialModifier = sFcfData.SecondaryMaterialModifier;
                        cDimensionData.tertiaryDatum = sFcfData.TertiaryDatum;
                        cDimensionData.tertiaryMaterialModifier = sFcfData.TertiaryMaterialModifier;
                        
                        #endregion
                    }
                    else
                    {
                        Tag annTag = singleObj.Tag;

                        int
                            ann_data_type,
                            ann_data_form,
                            num_segments,
                            cycle = 0,
                            ii,
                            length,
                            size;
                        double
                            radius_angle;
                        int[]
                            ann_data = new int[10],
                            mask = new int[4] { 0, 0, 1, 0 };
                        double[]
                            ann_origin = new double[2];


                    AskAnnData:
                        theUfSession.Drf.AskAnnData(ref annTag,
                                                           mask,
                                                           ref cycle,
                                                           ann_data,
                                                           out ann_data_type,
                                                           out ann_data_form,
                                                           out num_segments,
                                                           ann_origin,
                                                           out radius_angle);
                        //CaxLog.ShowListingWindow("ann_data_type：" + ann_data_type.ToString());
                        //CaxLog.ShowListingWindow("ann_data_form：" + ann_data_form.ToString());
                        if (cycle != 0)
                        {
                            for (ii = 1; ii <= num_segments; ii++)
                            {
                                string cr3;
                                theUfSession.Drf.AskTextData(ii, ref ann_data[0], out cr3, out size, out length);
                                MappingDimensionData(ann_data_type, ann_data_form, cr3, ref cDimensionData);
                                //CaxLog.ShowListingWindow("cr3：" + cr3);
                                //CaxLog.ShowListingWindow(size.ToString());
                                //CaxLog.ShowListingWindow(length.ToString());
                            }
                            goto AskAnnData;
                        }
                        //CaxLog.ShowListingWindow("----");
                    }
                    try
                    {
                        if (cDimensionData.upperTol != null)
                        {
                            cDimensionData.maxTolerance = (Convert.ToDouble(cDimensionData.mainText) + Convert.ToDouble(cDimensionData.upperTol)).ToString();
                        }
                        if (cDimensionData.lowerTol != null)
                        {
                            cDimensionData.minTolerance = (Convert.ToDouble(cDimensionData.mainText) - Convert.ToDouble(cDimensionData.lowerTol)).ToString();
                        }
                    }
                    catch (System.Exception ex)
                    {
                    	
                    }
                }
                
                
                /*
                if (singleObj.GetType().ToString() == "NXOpen.Annotations.VerticalDimension")
                {
                    #region VerticalDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.VerticalDimension";
                    NXOpen.Annotations.VerticalDimension temp = (NXOpen.Annotations.VerticalDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = mainText[0];
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.PerpendicularDimension")
                {
                    #region PerpendicularDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.PerpendicularDimension";
                    NXOpen.Annotations.PerpendicularDimension temp = (NXOpen.Annotations.PerpendicularDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = mainText[0];
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.MinorAngularDimension")
                {
                    #region MinorAngularDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.MinorAngularDimension";
                    NXOpen.Annotations.MinorAngularDimension temp = (NXOpen.Annotations.MinorAngularDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = mainText[0];
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.RadiusDimension")
                {
                    #region RadiusDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.RadiusDimension";
                    NXOpen.Annotations.RadiusDimension temp = (NXOpen.Annotations.RadiusDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = "R" + mainText[0];
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.HorizontalDimension")
                {
                    #region HorizontalDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.HorizontalDimension";
                    NXOpen.Annotations.HorizontalDimension temp = (NXOpen.Annotations.HorizontalDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = mainText[0];
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                    
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.IdSymbol")
                {
                    #region IdSymbol取Text
                    cDimensionData.type = "NXOpen.Annotations.IdSymbol";
                    NXOpen.Annotations.IdSymbol temp = (NXOpen.Annotations.IdSymbol)singleObj;

                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.Note")
                {
                    #region Note取Text
                    cDimensionData.type = "NXOpen.Annotations.Note";
                    NXOpen.Annotations.Note temp = (NXOpen.Annotations.Note)singleObj;

                    string tempStr = "";
                    for (int i = 0; i < temp.GetText().Length;i++ )
                    {
                        if (i == 0)
                        {
                            tempStr = temp.GetText()[i];
                        }
                        else
                        {
                            tempStr = tempStr + "\r\n" + temp.GetText()[i];
                        }



                        //if (i == 0)
                        //{
                        //    tempStr = temp.GetText()[i].Replace("<F2>", "");
                        //    tempStr = tempStr.Replace("<F>", "");
                        //}
                        //else
                        //{
                        //    string tempStr1 = temp.GetText()[i].Replace("<F2>", "");
                        //    tempStr1 = tempStr1.Replace("<F>", "");

                        //    tempStr = tempStr + "\r\n" + tempStr1;
                        //}
                    }
                    //foreach (string i in temp.GetText())
                    //{
                    //    tempStr = tempStr + i.Replace("<F2>", "");
                    //    tempStr = tempStr.Replace("<F>", "");
                    //}

                    cDimensionData.mainText = tempStr;

                    //判斷是否由CAX產生的Note
                    //string createdby = "";
                    //try
                    //{
                    //    createdby = temp.GetStringAttribute("CREATEDBY");
                    //}
                    //catch (System.Exception ex)
                    //{
                    //    createdby = "";
                    //}
                    //if (createdby == "")
                    //{
                    //    string tempStr = temp.GetText()[0].Replace("<F2>", "");
                    //    tempStr = tempStr.Replace("<F>", "");
                    //    cDimensionData.mainText = tempStr;
                    //}
                    //else
                    //{
                    //    cDimensionData.mainText = temp.GetText()[0];
                    //}
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.DraftingFcf")
                {
                    #region DraftingFcf取Text
                    NXOpen.Annotations.DraftingFcf temp = (NXOpen.Annotations.DraftingFcf)singleObj;
                    CaxME.FcfData sFcfData = new CaxME.FcfData();
                    CaxME.GetFcfData(temp, out sFcfData);
                    cDimensionData.type = "NXOpen.Annotations.DraftingFcf";
                    cDimensionData.characteristic = sFcfData.Characteristic;
                    cDimensionData.zoneShape = sFcfData.ZoneShape;
                    cDimensionData.toleranceValue = sFcfData.ToleranceValue;
                    cDimensionData.materialModifier = sFcfData.MaterialModifier;
                    cDimensionData.primaryDatum = sFcfData.PrimaryDatum;
                    cDimensionData.primaryMaterialModifier = sFcfData.PrimaryMaterialModifier;
                    cDimensionData.secondaryDatum = sFcfData.SecondaryDatum;
                    cDimensionData.secondaryMaterialModifier = sFcfData.SecondaryMaterialModifier;
                    cDimensionData.tertiaryDatum = sFcfData.TertiaryDatum;
                    cDimensionData.tertiaryMaterialModifier = sFcfData.TertiaryMaterialModifier;
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.Label")
                {
                    #region Label取Text
                    cDimensionData.type = "NXOpen.Annotations.Label";
                    NXOpen.Annotations.Label temp = (NXOpen.Annotations.Label)singleObj;
                    cDimensionData.mainText = temp.GetText()[0];
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.DraftingDatum")
                {
                    #region DraftingDatum取Text
                    cDimensionData.type = "NXOpen.Annotations.DraftingDatum";
                    NXOpen.Annotations.DraftingDatum temp = (NXOpen.Annotations.DraftingDatum)singleObj;
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.DiameterDimension")
                {
                    #region DiameterDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.DiameterDimension";
                    NXOpen.Annotations.DiameterDimension temp = (NXOpen.Annotations.DiameterDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = "n" + mainText[0];
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.AngularDimension")
                {
                    #region AngularDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.AngularDimension";
                    NXOpen.Annotations.AngularDimension temp = (NXOpen.Annotations.AngularDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = mainText[0];
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.CylindricalDimension")
                {
                    #region CylindricalDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.CylindricalDimension";
                    NXOpen.Annotations.CylindricalDimension temp = (NXOpen.Annotations.CylindricalDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = "n" + mainText[0];
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.ChamferDimension")
                {
                    #region ChamferDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.ChamferDimension";
                    NXOpen.Annotations.ChamferDimension temp = (NXOpen.Annotations.ChamferDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = mainText[0] + "X" + "45" + "<$s>";
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.ArcLengthDimension")
                {
                    #region ArcLengthDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.ArcLengthDimension";
                    NXOpen.Annotations.ArcLengthDimension temp = (NXOpen.Annotations.ArcLengthDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = mainText[0] + "X" + "45" + "<$s>";
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.HoleDimension")
                {
                    #region HoleDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.HoleDimension";
                    NXOpen.Annotations.HoleDimension temp = (NXOpen.Annotations.HoleDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = "n" + mainText[0];
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.FoldedRadiusDimension")
                {
                    #region FoldedRadiusDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.FoldedRadiusDimension";
                    NXOpen.Annotations.FoldedRadiusDimension temp = (NXOpen.Annotations.FoldedRadiusDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = "R" + mainText[0];
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                }
                else if (singleObj.GetType().ToString() == "NXOpen.Annotations.ParallelDimension")
                {
                    #region ParallelDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.ParallelDimension";
                    NXOpen.Annotations.ParallelDimension temp = (NXOpen.Annotations.ParallelDimension)singleObj;

                    temp.GetDimensionText(out mainText, out dualText);

                    if (mainText.Length > 0)
                    {
                        cDimensionData.mainText = mainText[0];
                    }
                    if (temp.GetAppendedText().GetBeforeText().Length > 0)
                    {
                        cDimensionData.beforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAfterText().Length > 0)
                    {
                        cDimensionData.afterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetAboveText().Length > 0)
                    {
                        cDimensionData.aboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                    }
                    if (temp.GetAppendedText().GetBelowText().Length > 0)
                    {
                        cDimensionData.belowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralOneLine")
                    {
                        cDimensionData.tolType = "BilateralOneLine";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                    }
                    if (temp.ToleranceType.ToString() == "BilateralTwoLines")
                    {
                        cDimensionData.tolType = "BilateralTwoLines";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralAbove")
                    {
                        cDimensionData.tolType = "UnilateralAbove";
                        cDimensionData.upperTol = temp.UpperMetricToleranceValue.ToString();
                        cDimensionData.lowerTol = "0";
                    }
                    if (temp.ToleranceType.ToString() == "UnilateralBelow")
                    {
                        cDimensionData.tolType = "UnilateralBelow";
                        cDimensionData.upperTol = "0";
                        cDimensionData.lowerTol = temp.LowerMetricToleranceValue.ToString();
                    }
                    #endregion
                }
                */
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
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

        public static string GetGDTWord(string NXData)
        {
            string ExcelSymbol = "";

            if (NXData == "LeastMaterialCondition")
            {
                ExcelSymbol = "l";
            }
            else if (NXData == "MaximumMaterialCondition")
            {
                ExcelSymbol = "m";
            }
            else if (NXData == "RegardlessOfFeatureSize")
            {
                ExcelSymbol = "s";
            }
            else if (NXData == "Straightness")
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
            else if (NXData == "Diameter")
            {
                ExcelSymbol = "n";
            }
            else
            {
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
            }
            return ExcelSymbol;
        }

        public static string GetMaterialModifier(string Value)
        {
            string ValueStr = "";
            if (Value != "" & Value != "None")
            {
                if (Value == "LeastMaterialCondition")
                {
                    ValueStr = "l";
                }
                else if (Value == "MaximumMaterialCondition")
                {
                    ValueStr = "m";
                }
                else if (Value == "RegardlessOfFeatureSize")
                {
                    ValueStr = "s";
                }
            }
            return ValueStr;
        }

        public static bool GetTolerance(DimensionData cDimensionData, ref Com_Dimension cCom_Dimension)
        {
            try
            {
                if (cDimensionData.upperTol != "" & cDimensionData.tolType == "BilateralOneLine")
                {
                    cCom_Dimension.toleranceType = cDimensionData.tolType;
                    cCom_Dimension.upTolerance = cDimensionData.upperTol;
                    cCom_Dimension.lowTolerance = cDimensionData.lowerTol;
                }
                else if (cDimensionData.upperTol != "" & cDimensionData.tolType == "BilateralTwoLines")
                {
                    cCom_Dimension.toleranceType = cDimensionData.tolType;
                    cCom_Dimension.upTolerance = cDimensionData.upperTol;
                    cCom_Dimension.lowTolerance = cDimensionData.lowerTol;
                }
                else if (cDimensionData.upperTol != "" & cDimensionData.tolType == "UnilateralAbove")
                {
                    cCom_Dimension.toleranceType = cDimensionData.tolType;
                    cCom_Dimension.upTolerance = cDimensionData.upperTol;
                    cCom_Dimension.lowTolerance = "0";
                }
                else if (cDimensionData.upperTol != "" & cDimensionData.tolType == "UnilateralBelow")
                {
                    cCom_Dimension.toleranceType = cDimensionData.tolType;
                    cCom_Dimension.upTolerance = "0";
                    cCom_Dimension.lowTolerance = cDimensionData.lowerTol;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool MappingData(DimensionData input, ref Com_Dimension cCom_Dimension)
        {
            try
            {
                if (input.characteristic != null)
                {
                    cCom_Dimension.characteristic = GetGDTWord(input.characteristic);
                }
                if (input.zoneShape != null)
                {
                    cCom_Dimension.zoneShape = GetGDTWord(input.zoneShape);
                }
                if (input.toleranceValue != null)
                {
                    cCom_Dimension.toleranceValue = input.toleranceValue;
                }
                if (input.materialModifier != null)
                {
                    cCom_Dimension.materialModifer = GetGDTWord(input.materialModifier);
                }
                if (input.primaryDatum != null)
                {
                    cCom_Dimension.primaryDatum = input.primaryDatum;
                }
                if (input.primaryMaterialModifier != null)
                {
                    cCom_Dimension.primaryMaterialModifier = GetGDTWord(input.primaryMaterialModifier);
                }
                if (input.secondaryDatum != null)
                {
                    cCom_Dimension.secondaryDatum = input.secondaryDatum;
                }
                if (input.secondaryMaterialModifier != null)
                {
                    cCom_Dimension.secondaryMaterialModifier = GetGDTWord(input.secondaryMaterialModifier);
                }
                if (input.tertiaryDatum != null)
                {
                    cCom_Dimension.tertiaryDatum = input.tertiaryDatum;
                }
                if (input.tertiaryMaterialModifier != null)
                {
                    cCom_Dimension.tertiaryMaterialModifier = GetGDTWord(input.tertiaryMaterialModifier);
                }
                if (input.aboveText != null)
                {
                    cCom_Dimension.aboveText = GetGDTWord(input.aboveText);
                }
                if (input.belowText != null)
                {
                    cCom_Dimension.belowText = GetGDTWord(input.belowText);
                }
                if (input.beforeText != null)
                {
                    cCom_Dimension.beforeText = GetGDTWord(input.beforeText);
                }
                if (input.afterText != null)
                {
                    cCom_Dimension.afterText = GetGDTWord(input.afterText);
                }
                if (input.toleranceSymbol != null)
                {
                    cCom_Dimension.toleranceSymbol = GetGDTWord(input.toleranceSymbol);
                }
                if (input.mainText != null)
                {
                    cCom_Dimension.mainText = input.mainText;
                }
                if (input.upperTol != null)
                {
                    cCom_Dimension.upTolerance = input.upperTol;
                }
                if (input.lowerTol != null)
                {
                    cCom_Dimension.lowTolerance = input.lowerTol;
                }
                if (input.x != null)
                {
                    cCom_Dimension.x = input.x;
                }
                if (input.chamferAngle != null)
                {
                    cCom_Dimension.chamferAngle = GetGDTWord(input.chamferAngle);
                }
                if (input.maxTolerance != null)
                {
                    cCom_Dimension.maxTolerance = input.maxTolerance;
                }
                if (input.minTolerance != null)
                {
                    cCom_Dimension.minTolerance = input.minTolerance;
                }
                cCom_Dimension.draftingVer = input.draftingVer;
                cCom_Dimension.draftingDate = input.draftingDate;
                cCom_Dimension.ballon = input.ballonNum;
                cCom_Dimension.location = input.location;
                cCom_Dimension.frequency = input.frequency;
                cCom_Dimension.instrument = input.instrument;
                
                /*
                if (input.type == "NXOpen.Annotations.DraftingFcf")
                {
                    cCom_Dimension.dimensionType = input.type;
                    #region DraftingFcf
                    if (input.characteristic != "")
                    {
                        cCom_Dimension.characteristic = GetCharacteristicSymbol(input.characteristic);
                    }
                    if (input.zoneShape != "")
                    {
                        cCom_Dimension.zoneShape = GetZoneShapeSymbol(input.zoneShape);
                    }
                    if (input.toleranceValue != "")
                    {
                        cCom_Dimension.toleranceValue = input.toleranceValue;
                    }
                    if (input.materialModifier != "")
                    {
                        cCom_Dimension.materialModifer = GetMaterialModifier(input.materialModifier);
                    }
                    if (input.primaryDatum != "")
                    {
                        cCom_Dimension.primaryDatum = input.primaryDatum;
                    }
                    if (input.primaryMaterialModifier != "")
                    {
                        cCom_Dimension.primaryMaterialModifier = GetMaterialModifier(input.primaryMaterialModifier);
                    }
                    if (input.secondaryDatum != "")
                    {
                        cCom_Dimension.secondaryDatum = input.secondaryDatum;
                    }
                    if (input.secondaryMaterialModifier != "")
                    {
                        cCom_Dimension.secondaryMaterialModifier = GetMaterialModifier(input.secondaryMaterialModifier);
                    }
                    if (input.tertiaryDatum != "")
                    {
                        cCom_Dimension.tertiaryDatum = input.tertiaryDatum;
                    }
                    if (input.tertiaryMaterialModifier != "")
                    {
                        cCom_Dimension.tertiaryMaterialModifier = GetMaterialModifier(input.tertiaryMaterialModifier);
                    }
                    #endregion
                }
                else if (input.type == "NXOpen.Annotations.Label")
                {
                    cCom_Dimension.dimensionType = input.type;
                    #region Label
                    if (input.mainText != "")
                    {
                        cCom_Dimension.mainText = input.mainText;
                    }
                    #endregion
                }
                else
                {
                    cCom_Dimension.dimensionType = input.type;
                    #region Dimension
                    if (input.beforeText != null)
                    {
                        cCom_Dimension.beforeText = GetGDTWord(input.beforeText);
                    }
                    if (input.mainText != "")
                    {
                        cCom_Dimension.mainText = GetGDTWord(input.mainText);
                    }
                    if (input.upperTol != "")
                    {
                        Database.GetTolerance(input, ref cCom_Dimension);
                    }
                    #endregion
                }
                */
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
