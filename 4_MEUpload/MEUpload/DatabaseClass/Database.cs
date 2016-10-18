using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using CaxGlobaltek;

namespace MEUpload.DatabaseClass
{
    public class DimensionData
    {
        public string type { get; set; }
        public string mainText { get; set; }
        public string beforeText { get; set; }
        public string afterText { get; set; }
        public string aboveText { get; set; }
        public string belowText { get; set; }
        public string toleranceSymbol { get; set; }
        public string upperTol { get; set; }
        public string lowerTol { get; set; }
        public string tolType { get; set; }
        public string instrument { get; set; }
        public string location { get; set; }
        public string frequency { get; set; }
        public string ballonNum { get; set; }
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
        public static Dictionary<MEMainData, DimensionData> DicDimenData = new Dictionary<MEMainData, DimensionData>();
        public static List<DimensionData> listDimensionData = new List<DimensionData>();

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
                string[] mainText;
                string[] dualText;


                if (excelType == "FAI")
                {
                    try { cDimensionData.instrument = singleObj.GetStringAttribute(CaxME.DimenAttr.FAI_Gauge); }
                    catch (System.Exception ex) { return false; }

                    try { cDimensionData.frequency = singleObj.GetStringAttribute(CaxME.DimenAttr.FAI_Freq); }
                    catch (System.Exception ex) { return false; }
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
                    catch (System.Exception ex) { return false; }

                    try { cDimensionData.frequency = singleObj.GetStringAttribute(CaxME.DimenAttr.IQC_Freq); }
                    catch (System.Exception ex) { return false; }
                }
                else if (excelType == "SelfCheck")
                {
                    try { cDimensionData.instrument = singleObj.GetStringAttribute(CaxME.DimenAttr.SelfCheck_Gauge); }
                    catch (System.Exception ex) { return false; }

                    try { cDimensionData.frequency = singleObj.GetStringAttribute(CaxME.DimenAttr.SelfCheck_Freq); }
                    catch (System.Exception ex) { return false; }
                }

                try { cDimensionData.ballonNum = singleObj.GetStringAttribute(CaxME.DimenAttr.BallonNum); }
                catch (System.Exception ex) { return false; }

                try { cDimensionData.location = singleObj.GetStringAttribute(CaxME.DimenAttr.BallonLocation); }
                catch (System.Exception ex) { return false; }

                if (singleObjType == "NXOpen.Annotations.VerticalDimension")
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
                else if (singleObjType == "NXOpen.Annotations.PerpendicularDimension")
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
                else if (singleObjType == "NXOpen.Annotations.MinorAngularDimension")
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
                else if (singleObjType == "NXOpen.Annotations.RadiusDimension")
                {
                    #region MinorAngularDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.RadiusDimension";
                    NXOpen.Annotations.RadiusDimension temp = (NXOpen.Annotations.RadiusDimension)singleObj;

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
                else if (singleObjType == "NXOpen.Annotations.HorizontalDimension")
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
                else if (singleObjType == "NXOpen.Annotations.IdSymbol")
                {
                    #region IdSymbol取Text
                    cDimensionData.type = "NXOpen.Annotations.IdSymbol";
                    NXOpen.Annotations.IdSymbol temp = (NXOpen.Annotations.IdSymbol)singleObj;

                    #endregion
                }
                else if (singleObjType == "NXOpen.Annotations.Note")
                {
                    #region Note取Text
                    cDimensionData.type = "NXOpen.Annotations.Note";
                    NXOpen.Annotations.Note temp = (NXOpen.Annotations.Note)singleObj;
                    //判斷是否由CAX產生的Note
                    string createby = "";
                    try
                    {
                        createby = temp.GetStringAttribute("Createby");
                    }
                    catch (System.Exception ex)
                    {
                        createby = "";
                    }
                    if (createby == "")
                    {
                        string tempStr = temp.GetText()[0].Replace("<F2>", "");
                        tempStr = tempStr.Replace("<F>", "");
                        cDimensionData.mainText = tempStr;
                    }
                    else
                    {
                        cDimensionData.mainText = temp.GetText()[0];
                    }
                    #endregion
                }
                else if (singleObjType == "NXOpen.Annotations.DraftingFcf")
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
                else if (singleObjType == "NXOpen.Annotations.Label")
                {
                    #region Label取Text
                    cDimensionData.type = "NXOpen.Annotations.Label";
                    NXOpen.Annotations.Label temp = (NXOpen.Annotations.Label)singleObj;
                    cDimensionData.mainText = temp.GetText()[0];
                    #endregion
                }
                else if (singleObjType == "NXOpen.Annotations.DraftingDatum")
                {
                    #region DraftingDatum取Text
                    cDimensionData.type = "NXOpen.Annotations.DraftingDatum";
                    NXOpen.Annotations.DraftingDatum temp = (NXOpen.Annotations.DraftingDatum)singleObj;
                    #endregion
                }
                else if (singleObjType == "NXOpen.Annotations.DiameterDimension")
                {
                    #region DiameterDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.DiameterDimension";
                    NXOpen.Annotations.DiameterDimension temp = (NXOpen.Annotations.DiameterDimension)singleObj;

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
                else if (singleObjType == "NXOpen.Annotations.AngularDimension")
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
                else if (singleObjType == "NXOpen.Annotations.CylindricalDimension")
                {
                    #region CylindricalDimension取Text
                    cDimensionData.type = "NXOpen.Annotations.CylindricalDimension";
                    NXOpen.Annotations.CylindricalDimension temp = (NXOpen.Annotations.CylindricalDimension)singleObj;

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
                else if (singleObjType == "NXOpen.Annotations.ChamferDimension")
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

        public static string GetMaterialModifier(string Value)
        {
            string ValueStr = "";
            if (Value != "" & Value != "None")
            {
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
                cCom_Dimension.draftingVer = input.draftingVer;
                cCom_Dimension.draftingDate = input.draftingDate;
                cCom_Dimension.ballon = input.ballonNum;
                cCom_Dimension.location = input.location;
                cCom_Dimension.instrument = input.instrument;
                cCom_Dimension.frequency = input.frequency;
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
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
