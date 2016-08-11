using System;
using NXOpen;
using NXOpen.UF;
using ExportIPQC;
using System.Collections.Generic;
using NXOpen.Utilities;
using CaxGlobaltek;
using NXOpen.Annotations;
using System.IO;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

public class Program
{
    // class members
    private static Session theSession;
    private static UI theUI;
    private static UFSession theUfSession;
    public static Program theProgram;
    public static bool isDisposeCalled;

    //------------------------------------------------------------------------------
    // Constructor
    //------------------------------------------------------------------------------
    public Program()
    {
        try
        {
            theSession = Session.GetSession();
            theUI = UI.GetUI();
            theUfSession = UFSession.GetUFSession();
            isDisposeCalled = false;
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----
            // UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ex.Message);
        }
    }

    //------------------------------------------------------------------------------
    //  Explicit Activation
    //      This entry point is used to activate the application explicitly
    //------------------------------------------------------------------------------
    public static int Main(string[] args)
    {
        int retValue = 0;
        try
        {
            theProgram = new Program();
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            
             
            bool status;

            //���o�Ƹ�
            string PartNo = Path.GetFileNameWithoutExtension(displayPart.FullPath);

            //���o���
            string CurrentDate = DateTime.Now.ToShortDateString();

            int SheetCount = 0;
            NXOpen.Tag[] SheetTagAry = null;
            theUfSession.Draw.AskDrawings(out SheetCount, out SheetTagAry);

            DefineVariable.DicDimenData = new Dictionary<string, TextData>();
            for (int i = 0; i < SheetCount; i++)
            {
                //���}Sheet�ðO���Ҧ�OBJ
                NXOpen.Drawings.DrawingSheet CurrentSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(SheetTagAry[i]);
                CurrentSheet.Open();
                if (i == 0)
                {
                    //�O���Ĥ@�iSheet
                    DefineVariable.FirstDrawingSheet = CurrentSheet;
                }
                DisplayableObject[] SheetObj = CurrentSheet.View.AskVisibleObjects();
                foreach (DisplayableObject singleObj in SheetObj)
                {
                    TextData cTextData = new TextData();
                    string singleObjType = singleObj.GetType().ToString();
                    string IPQC_Gauge = "", BallonNum = "", Frequency = "", Location = "";
                    string[] mainText;
                    string[] dualText;

                    #region ��IPQC�@���ݩ�(�w�w�ȡB�˨�W�١B�����W�v�B�w�w�Ҧb�ϰ�)�A�p�G���S�ݩʴN��U�@��
                    try
                    {
                        IPQC_Gauge = singleObj.GetStringAttribute(CaxME.DimenAttr.IPQC_Gauge);
                        BallonNum = singleObj.GetStringAttribute(CaxME.DimenAttr.BallonNum);
                        Frequency = singleObj.GetStringAttribute(CaxME.DimenAttr.IPQC_Freq);
                        Location = singleObj.GetStringAttribute(CaxME.DimenAttr.BallonLocation);
                    }
                    catch (System.Exception ex)
                    {
                        IPQC_Gauge = "";
                    }
                    if (IPQC_Gauge == "")
                    {
                        continue;
                    }
                    #endregion

                    #region �����@���ݩ�(�w�w�ȡB�˨�W�١B�����W�v�B�w�w�Ҧb�ϰ�)

                    //���o�w�w��
                    cTextData.BallonNum = BallonNum;

                    //���o�˨�W��
                    cTextData.Gauge = IPQC_Gauge;

                    //���o�����W�v
                    cTextData.Frequency = Frequency;

                    //���o�w�w�Ҧb�ϰ�
                    cTextData.Location = Location;

                    #endregion

                    if (singleObjType == "NXOpen.Annotations.VerticalDimension")
                    {
                        #region VerticalDimension��Text
                        cTextData.Type = "NXOpen.Annotations.VerticalDimension";
                        NXOpen.Annotations.VerticalDimension temp = (NXOpen.Annotations.VerticalDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.MainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.TolType = "BilateralOneLine";
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.PerpendicularDimension")
                    {
                        #region PerpendicularDimension��Text
                        cTextData.Type = "NXOpen.Annotations.PerpendicularDimension";
                        NXOpen.Annotations.PerpendicularDimension temp = (NXOpen.Annotations.PerpendicularDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.MainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.TolType = "BilateralOneLine";
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.MinorAngularDimension")
                    {
                        #region MinorAngularDimension��Text
                        cTextData.Type = "NXOpen.Annotations.MinorAngularDimension";
                        NXOpen.Annotations.MinorAngularDimension temp = (NXOpen.Annotations.MinorAngularDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.MainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.TolType = "BilateralOneLine";
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.RadiusDimension")
                    {
                        #region MinorAngularDimension��Text
                        cTextData.Type = "NXOpen.Annotations.RadiusDimension";
                        NXOpen.Annotations.RadiusDimension temp = (NXOpen.Annotations.RadiusDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.MainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.TolType = "BilateralOneLine";
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.HorizontalDimension")
                    {
                        #region HorizontalDimension��Text
                        cTextData.Type = "NXOpen.Annotations.HorizontalDimension";
                        NXOpen.Annotations.HorizontalDimension temp = (NXOpen.Annotations.HorizontalDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.MainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.TolType = "BilateralOneLine";
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.IdSymbol")
                    {
                        #region IdSymbol��Text
                        cTextData.Type = "NXOpen.Annotations.IdSymbol";
                        NXOpen.Annotations.IdSymbol temp = (NXOpen.Annotations.IdSymbol)singleObj;

                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.Note")
                    {
                        #region Note��Text
                        cTextData.Type = "NXOpen.Annotations.Note";
                        NXOpen.Annotations.Note temp = (NXOpen.Annotations.Note)singleObj;

                        cTextData.MainText = temp.GetText()[0];
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.DraftingFcf")
                    {
                        #region DraftingFcf��Text
                        NXOpen.Annotations.DraftingFcf temp = (NXOpen.Annotations.DraftingFcf)singleObj;
                        CaxME.FcfData sFcfData = new CaxME.FcfData();
                        CaxME.GetFcfData(temp, out sFcfData);
                        cTextData.Type = "NXOpen.Annotations.DraftingFcf";
                        cTextData.Characteristic = sFcfData.Characteristic;
                        cTextData.ZoneShape = sFcfData.ZoneShape;
                        cTextData.ToleranceValue = sFcfData.ToleranceValue;
                        cTextData.MaterialModifier = sFcfData.MaterialModifier;
                        cTextData.PrimaryDatum = sFcfData.PrimaryDatum;
                        cTextData.PrimaryMaterialModifier = sFcfData.PrimaryMaterialModifier;
                        cTextData.SecondaryDatum = sFcfData.SecondaryDatum;
                        cTextData.SecondaryMaterialModifier = sFcfData.SecondaryMaterialModifier;
                        cTextData.TertiaryDatum = sFcfData.TertiaryDatum;
                        cTextData.TertiaryMaterialModifier = sFcfData.TertiaryMaterialModifier;
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.Label")
                    {
                        #region Label��Text
                        cTextData.Type = "NXOpen.Annotations.Label";
                        NXOpen.Annotations.Label temp = (NXOpen.Annotations.Label)singleObj;
                        cTextData.MainText = temp.GetText()[0];
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.DraftingDatum")
                    {
                        #region DraftingDatum��Text
                        cTextData.Type = "NXOpen.Annotations.DraftingDatum";
                        NXOpen.Annotations.DraftingDatum temp = (NXOpen.Annotations.DraftingDatum)singleObj;
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.DiameterDimension")
                    {
                        #region DiameterDimension��Text
                        cTextData.Type = "NXOpen.Annotations.DiameterDimension";
                        NXOpen.Annotations.DiameterDimension temp = (NXOpen.Annotations.DiameterDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.MainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.TolType = "BilateralOneLine";
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.AngularDimension")
                    {
                        #region AngularDimension��Text
                        cTextData.Type = "NXOpen.Annotations.AngularDimension";
                        NXOpen.Annotations.AngularDimension temp = (NXOpen.Annotations.AngularDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.MainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.TolType = "BilateralOneLine";
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }

                    //�p��w�w�`��
                    DefineVariable.BallonCount++;

                    DefineVariable.DicDimenData[BallonNum] = cTextData;
                }
            }


            /*
            for (int i=0;i<200;i++)
            {
                TextData cTextData = new TextData();
                status = DefineVariable.DicDimenData.TryGetValue(i.ToString(), out cTextData);
                if (!status)
                {
                    continue; 
                }

                CaxLog.ShowListingWindow("Gauge�G" + cTextData.Gauge);
                CaxLog.ShowListingWindow("Location�G" + cTextData.Location);
                CaxLog.ShowListingWindow("mainText�G" + cTextData.MainText);
                CaxLog.ShowListingWindow("BallonNum�G" + cTextData.BallonNum);
                CaxLog.ShowListingWindow("AboveText�G" + cTextData.AboveText);
                CaxLog.ShowListingWindow("AfterText�G" + cTextData.AfterText);
                CaxLog.ShowListingWindow("BeforeText�G" + cTextData.BeforeText);
                CaxLog.ShowListingWindow("BelowText�G" + cTextData.BelowText);
                CaxLog.ShowListingWindow("Characteristic�G" + cTextData.Characteristic);
                CaxLog.ShowListingWindow("Frequency�G" + cTextData.Frequency);
                CaxLog.ShowListingWindow("LowerTol�G" + cTextData.LowerTol);
                CaxLog.ShowListingWindow("MaterialModifier�G" + cTextData.MaterialModifier);
                CaxLog.ShowListingWindow("PrimaryDatum�G" + cTextData.PrimaryDatum);
                CaxLog.ShowListingWindow("PrimaryMaterialModifier�G" + cTextData.PrimaryMaterialModifier);
                CaxLog.ShowListingWindow("SecondaryDatum�G" + cTextData.SecondaryDatum);
                CaxLog.ShowListingWindow("SecondaryMaterialModifier�G" + cTextData.SecondaryMaterialModifier);
                CaxLog.ShowListingWindow("TertiaryDatum�G" + cTextData.TertiaryDatum);
                CaxLog.ShowListingWindow("TertiaryMaterialModifier�G" + cTextData.TertiaryMaterialModifier);
                CaxLog.ShowListingWindow("ToleranceValue�G" + cTextData.ToleranceValue);
                CaxLog.ShowListingWindow("Type�G" + cTextData.Type);
                CaxLog.ShowListingWindow("UpperTol�G" + cTextData.UpperTol);
                CaxLog.ShowListingWindow("ZoneShape�G" + cTextData.ZoneShape);
                CaxLog.ShowListingWindow("---------");
            }
            */







            /*
            DefineVariable.DicDimenData = new Dictionary<string, List<TextData>>();
            for (int i = 0; i < SheetCount; i++)
            {
                //���}Sheet�ðO���Ҧ�OBJ
                NXOpen.Drawings.DrawingSheet CurrentSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(SheetTagAry[i]);
                NXOpen.Drawings.DrawingSheet drawingSheet = (NXOpen.Drawings.DrawingSheet)workPart.DrawingSheets.FindObject(CurrentSheet.Name);
                drawingSheet.Open();
                DisplayableObject[] SheetObj = drawingSheet.View.AskVisibleObjects();
                foreach (DisplayableObject singleObj in SheetObj)
                {
                    List<TextData> ListTextData = new List<TextData>();
                    TextData cTextData = new TextData();
                    string singleObjType = singleObj.GetType().ToString();
                    string IPQC_Gauge = "", BallonNum = "", Frequency = "", Location = "";
                    string[] mainText;
                    string[] dualText;

                    #region ��IPQC�@���ݩ�(�w�w�ȡB�˨�W�١B�����W�v�B�w�w�Ҧb�ϰ�)�A�p�G���S�ݩʴN��U�@��
                    try
                    {
                        IPQC_Gauge = singleObj.GetStringAttribute(CaxME.DimenAttr.IPQC_Gauge);
                        BallonNum = singleObj.GetStringAttribute(CaxME.DimenAttr.BallonNum);
                        Frequency = singleObj.GetStringAttribute(CaxME.DimenAttr.IPQC_Freq);
                        Location = singleObj.GetStringAttribute(CaxME.DimenAttr.BallonLocation);
                    }
                    catch (System.Exception ex)
                    {
                        IPQC_Gauge = "";
                    }
                    if (IPQC_Gauge == "")
                    {
                        continue;
                    }
                    #endregion

                    #region �����@���ݩ�(�w�w�ȡB�˨�W�١B�����W�v�B�w�w�Ҧb�ϰ�)
                    
                    //���o�w�w��
                    //cTextData.BallonNum = BallonNum;

                    //���o�˨�W��
                    cTextData.Gauge = IPQC_Gauge;

                    //���o�����W�v
                    cTextData.Frequency = Frequency;

                    //���o�w�w�Ҧb�ϰ�
                    cTextData.Location = Location;

                    #endregion

                    if (singleObjType == "NXOpen.Annotations.VerticalDimension")
                    {
                        #region VerticalDimension��Text
                        cTextData.Type = "NXOpen.Annotations.VerticalDimension";
                        NXOpen.Annotations.VerticalDimension temp = (NXOpen.Annotations.VerticalDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.mainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.PerpendicularDimension")
                    {
                        #region PerpendicularDimension��Text
                        cTextData.Type = "NXOpen.Annotations.PerpendicularDimension";
                        NXOpen.Annotations.PerpendicularDimension temp = (NXOpen.Annotations.PerpendicularDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.mainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.MinorAngularDimension")
                    {
                        #region MinorAngularDimension��Text
                        cTextData.Type = "NXOpen.Annotations.MinorAngularDimension";
                        NXOpen.Annotations.MinorAngularDimension temp = (NXOpen.Annotations.MinorAngularDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.mainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.RadiusDimension")
                    {
                        #region MinorAngularDimension��Text
                        cTextData.Type = "NXOpen.Annotations.RadiusDimension";
                        NXOpen.Annotations.RadiusDimension temp = (NXOpen.Annotations.RadiusDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.mainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.HorizontalDimension")
                    {
                        #region HorizontalDimension��Text
                        cTextData.Type = "NXOpen.Annotations.HorizontalDimension";
                        NXOpen.Annotations.HorizontalDimension temp = (NXOpen.Annotations.HorizontalDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.mainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.IdSymbol")
                    {
                        #region IdSymbol��Text
                        cTextData.Type = "NXOpen.Annotations.IdSymbol";
                        NXOpen.Annotations.IdSymbol temp = (NXOpen.Annotations.IdSymbol)singleObj;

                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.Note")
                    {
                        #region Note��Text
                        cTextData.Type = "NXOpen.Annotations.Note";
                        NXOpen.Annotations.Note temp = (NXOpen.Annotations.Note)singleObj;

                        cTextData.mainText = temp.GetText()[0];
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.DraftingFcf")
                    {
                        #region DraftingFcf��Text
                        NXOpen.Annotations.DraftingFcf temp = (NXOpen.Annotations.DraftingFcf)singleObj;
                        CaxME.FcfData sFcfData = new CaxME.FcfData();
                        CaxME.GetFcfData(temp, out sFcfData);
                        cTextData.Type = "NXOpen.Annotations.DraftingFcf";
                        cTextData.Characteristic = sFcfData.Characteristic;
                        cTextData.ZoneShape = sFcfData.ZoneShape;
                        cTextData.ToleranceValue = sFcfData.ToleranceValue;
                        cTextData.MaterialModifier = sFcfData.MaterialModifier;
                        cTextData.PrimaryDatum = sFcfData.PrimaryDatum;
                        cTextData.PrimaryMaterialModifier = sFcfData.PrimaryMaterialModifier;
                        cTextData.SecondaryDatum = sFcfData.SecondaryDatum;
                        cTextData.SecondaryMaterialModifier = sFcfData.SecondaryMaterialModifier;
                        cTextData.TertiaryDatum = sFcfData.TertiaryDatum;
                        cTextData.TertiaryMaterialModifier = sFcfData.TertiaryMaterialModifier;
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.Label")
                    {
                        #region Label��Text
                        cTextData.Type = "NXOpen.Annotations.Label";
                        NXOpen.Annotations.Label temp = (NXOpen.Annotations.Label)singleObj;
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.DraftingDatum")
                    {
                        #region DraftingDatum��Text
                        cTextData.Type = "NXOpen.Annotations.DraftingDatum";
                        NXOpen.Annotations.DraftingDatum temp = (NXOpen.Annotations.DraftingDatum)singleObj;
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.DiameterDimension")
                    {
                        #region DiameterDimension��Text
                        cTextData.Type = "NXOpen.Annotations.DiameterDimension";
                        NXOpen.Annotations.DiameterDimension temp = (NXOpen.Annotations.DiameterDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.mainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    else if (singleObjType == "NXOpen.Annotations.AngularDimension")
                    {
                        #region AngularDimension��Text
                        cTextData.Type = "NXOpen.Annotations.AngularDimension";
                        NXOpen.Annotations.AngularDimension temp = (NXOpen.Annotations.AngularDimension)singleObj;

                        temp.GetDimensionText(out mainText, out dualText);

                        if (mainText.Length > 0)
                        {
                            cTextData.mainText = mainText[0];
                        }
                        if (temp.GetAppendedText().GetBeforeText().Length > 0)
                        {
                            cTextData.BeforeText = temp.GetAppendedText().GetBeforeText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAfterText().Length > 0)
                        {
                            cTextData.AfterText = temp.GetAppendedText().GetAfterText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetAboveText().Length > 0)
                        {
                            cTextData.AboveText = temp.GetAppendedText().GetAboveText()[0].ToString();
                        }
                        if (temp.GetAppendedText().GetBelowText().Length > 0)
                        {
                            cTextData.BelowText = temp.GetAppendedText().GetBelowText()[0].ToString();
                        }
                        if (temp.ToleranceType.ToString() == "BilateralOneLine")
                        {
                            cTextData.UpperTol = temp.UpperMetricToleranceValue.ToString();
                            cTextData.LowerTol = (-1 * temp.UpperMetricToleranceValue).ToString();
                        }
                        #endregion
                    }
                    
                    //DefineVariable.ListTextData.Add(cTextData);

                    status = DefineVariable.DicDimenData.TryGetValue(IPQC_Gauge, out ListTextData);
                    if (status)
                    {
                        ListTextData.Add(cTextData);
                        DefineVariable.DicDimenData[IPQC_Gauge] = ListTextData;
                    }
                    else
                    {
                        ListTextData = new List<TextData>();
                        ListTextData.Add(cTextData);
                        DefineVariable.DicDimenData.Add(IPQC_Gauge, ListTextData);
                    }
                    
                }
            }
            */

            
            //�]�w��X���|
            string[] FolderFile = System.IO.Directory.GetFileSystemEntries(Path.GetDirectoryName(displayPart.FullPath), "*.xls");
            string OutputPath = string.Format(@"{0}\{1}", Path.GetDirectoryName(displayPart.FullPath),
                                                               Path.GetFileNameWithoutExtension(displayPart.FullPath) + "_" + "IPQC" + "_" + (FolderFile.Length + 1) + ".xls");
            
            //�ˬdPC���LExcel�b����
            foreach (var item in Process.GetProcesses())
            {
                if (item.ProcessName == "EXCEL")
                {
                    CaxLog.ShowListingWindow("�Х������Ҧ�Excel�A���s�����X�A�p�S��EXCEL�b����A�ж}�Ҥu�@�޲z�������I��EXCEL");
                    return retValue;
                }
                else
                {
                    continue;
                }
            }  
            
            
            
            Excel.ApplicationClass x = new Excel.ApplicationClass();
            Excel.Workbook book = null;
            Excel.Worksheet sheet = null;
            Excel.Range oRng = null;

            x.Visible = false;
            book = x.Workbooks.Open(@"D:\IPQC.xls");
            sheet = (Excel.Worksheet)book.Sheets[1];
            

            
            //�������`�ƶ}�ҲŦX�`�ƪ�����
            int needSheetNo = (DefineVariable.BallonCount / 18);
            int needSheetNo_Reserve = (DefineVariable.BallonCount % 18);
            if (needSheetNo_Reserve != 0)
            {
                needSheetNo++;
            }
            for (int i = 1; i < needSheetNo; i++)
            {
                sheet.Copy(System.Type.Missing, x.Workbooks[1].Worksheets[1]);
            }

            //���C�@��Sheet���W�ٻP����
            for (int i = 0; i < book.Worksheets.Count; i++)
            {
                sheet = (Excel.Worksheet)book.Sheets[i + 1];
                if (i == 0 && book.Worksheets.Count > 1)
                {
                    sheet.Name = PartNo;
                    oRng = (Excel.Range)sheet.Cells[5, 29];
                    oRng.Value = oRng.Value.ToString().Replace("1/1", "1/" + (book.Worksheets.Count).ToString());
                }
                else
                {
                    sheet.Name = PartNo + "(" + (i + 1) + ")";
                    oRng = (Excel.Range)sheet.Cells[5, 29];
                    string temp = (i + 1).ToString();
                    oRng.Value = oRng.Value.ToString().Replace("1/1", temp + "/" + (book.Worksheets.Count).ToString());
                }
            }
            
            //���
            int ExcelSequenceNo = -1;
            for (int i = 1; i < 1000; i++ )
            {
                ExcelSequenceNo++;

                TextData cTextData;
                DefineVariable.DicDimenData.TryGetValue(i.ToString(), out cTextData);
                if (cTextData == null)
                {
                    ExcelSequenceNo--;
                    continue;
                }

                RowColumn sRowColumn;
                DefineVariable.GetExcelRowColumn(ExcelSequenceNo, out sRowColumn);
                int currentSheet_Value = (ExcelSequenceNo / 18);
                int currentSheet_Reserve = (ExcelSequenceNo % 18);
                if (currentSheet_Value == 0)
                {
                    sheet = (Excel.Worksheet)book.Sheets[1];
                }
                else
                {
                    sheet = (Excel.Worksheet)book.Sheets[currentSheet_Value + 1];
                }

                oRng = (Excel.Range)sheet.Cells;


                if (cTextData.Type == "NXOpen.Annotations.DraftingFcf")
                {
                    #region DraftingFcf����
                    if (cTextData.Characteristic != "")
                    {
                        oRng[sRowColumn.CharacteristicRow, sRowColumn.CharacteristicColumn] = DefineVariable.GetCharacteristicSymbol(cTextData.Characteristic);
                    }
                    if (cTextData.ZoneShape != "")
                    {
                        oRng[sRowColumn.ZoneShapeRow, sRowColumn.ZoneShapeColumn] = DefineVariable.GetZoneShapeSymbol(cTextData.ZoneShape);
                    }
                    oRng[sRowColumn.ToleranceValueRow, sRowColumn.ToleranceValueColumn] = cTextData.ToleranceValue;
                    if (cTextData.MaterialModifier != "" & cTextData.MaterialModifier != "None")
                    {
                        string ValueStr = cTextData.MaterialModifier;
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
                        oRng[sRowColumn.MaterialModifierRow, sRowColumn.MaterialModifierColumn] = ValueStr;
                    }
                    //Primary
                    oRng[sRowColumn.PrimaryDatumRow, sRowColumn.PrimaryDatumColumn] = cTextData.PrimaryDatum;
                    if (cTextData.PrimaryMaterialModifier != "" & cTextData.PrimaryMaterialModifier != "None")
                    {
                        string ValueStr = cTextData.PrimaryMaterialModifier;
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
                        oRng[sRowColumn.PrimaryMaterialModifierRow, sRowColumn.PrimaryMaterialModifierColumn] = ValueStr;
                    }
                    //Secondary
                    oRng[sRowColumn.SecondaryDatumRow, sRowColumn.SecondaryDatumColumn] = cTextData.SecondaryDatum;
                    if (cTextData.SecondaryMaterialModifier != "" & cTextData.SecondaryMaterialModifier != "None")
                    {
                        string ValueStr = cTextData.SecondaryMaterialModifier;
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
                        oRng[sRowColumn.SecondaryMaterialModifierRow, sRowColumn.SecondaryMaterialModifierColumn] = ValueStr;
                    }
                    //Tertiary
                    oRng[sRowColumn.TertiaryDatumRow, sRowColumn.TertiaryDatumColumn] = cTextData.TertiaryDatum;
                    if (cTextData.TertiaryMaterialModifier != "" & cTextData.TertiaryMaterialModifier != "None")
                    {
                        string ValueStr = cTextData.TertiaryMaterialModifier;
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
                        oRng[sRowColumn.TertiaryMaterialModifierRow, sRowColumn.TertiaryMaterialModifierColumn] = ValueStr;
                    }
                    #endregion
                }
                else if (cTextData.Type == "NXOpen.Annotations.Label")
                {
                    oRng[sRowColumn.MainTextRow, sRowColumn.MainTextColumn] = cTextData.MainText;
                    ((Range)oRng[sRowColumn.MainTextRow, sRowColumn.MainTextColumn]).Interior.ColorIndex = 50;
                }
                else
                {
                    #region Dimension����
                    if (cTextData.BeforeText != null)
                    {
                        oRng[sRowColumn.BeforeTextRow, sRowColumn.BeforeTextColumn] = DefineVariable.GetGDTWord(cTextData.BeforeText);
                    }
                    oRng[sRowColumn.MainTextRow, sRowColumn.MainTextColumn] = DefineVariable.GetGDTWord(cTextData.MainText);
                    //Range a = (Range)oRng[sRowColumn.MainTextRow, sRowColumn.MainTextColumn];
                    //a.Interior.ColorIndex = 39;
                    oRng[sRowColumn.UpperTolRow, sRowColumn.UpperTolColumn] = cTextData.UpperTol;
                    if (cTextData.UpperTol != "" & cTextData.TolType == "BilateralOneLine")
                    {
                        oRng[sRowColumn.ToleranceSymbolRow, sRowColumn.ToleranceSymbolColumn] = "��";
                    }
                    #endregion
                }

                #region �˨�B�W�v�BMax�BMin�B�w�w�B�w�w��m�B�Ƹ��B���
                oRng[sRowColumn.GaugeRow, sRowColumn.GaugeColumn] = cTextData.Gauge;
                oRng[sRowColumn.FrequencyRow, sRowColumn.FrequencyColumn] = cTextData.Frequency;
                oRng[sRowColumn.BallonRow, sRowColumn.BallonColumn] = cTextData.BallonNum;
                oRng[sRowColumn.LocationRow, sRowColumn.LocationColumn] = cTextData.Location;
                oRng[sRowColumn.PartNoRow, sRowColumn.PartNoColumn] = PartNo;
                oRng[sRowColumn.DateRow, sRowColumn.DateColumn] = CurrentDate;
                #endregion
            }





            /*
            foreach (KeyValuePair<string, TextData> kvp in DefineVariable.DicDimenData)
            {
                ExcelSequenceNo++;
                InputKey++;

                TextData cTextData;
                DefineVariable.DicDimenData.TryGetValue(InputKey.ToString(), out cTextData);
                if (cTextData == null)
                {
                    ExcelSequenceNo--;
                    continue;
                }

                RowColumn sRowColumn;
                DefineVariable.GetExcelRowColumn(ExcelSequenceNo, out sRowColumn);
                int currentSheet_Value = (ExcelSequenceNo / 18);
                int currentSheet_Reserve = (ExcelSequenceNo % 18);
                if (currentSheet_Value == 0)
                {
                    sheet = (Excel.Worksheet)book.Sheets[1];
                }
                else
                {
                    sheet = (Excel.Worksheet)book.Sheets[currentSheet_Value + 1];
                }

                oRng = (Excel.Range)sheet.Cells;
                
                
                if (cTextData.Type == "NXOpen.Annotations.DraftingFcf")
                {
                    #region DraftingFcf����
                    if (cTextData.Characteristic != "")
                    {
                        oRng[sRowColumn.CharacteristicRow, sRowColumn.CharacteristicColumn] = DefineVariable.GetCharacteristicSymbol(cTextData.Characteristic);
                    }
                    if (kvp.Value.ZoneShape != "")
                    {
                        oRng[sRowColumn.ZoneShapeRow, sRowColumn.ZoneShapeColumn] = DefineVariable.GetZoneShapeSymbol(cTextData.ZoneShape);
                    }
                    oRng[sRowColumn.ToleranceValueRow, sRowColumn.ToleranceValueColumn] = cTextData.ToleranceValue;
                    if (cTextData.MaterialModifier != "" & cTextData.MaterialModifier != "None")
                    {
                        string ValueStr = cTextData.MaterialModifier;
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
                        oRng[sRowColumn.MaterialModifierRow, sRowColumn.MaterialModifierColumn] = ValueStr;
                    }
                    //Primary
                    oRng[sRowColumn.PrimaryDatumRow, sRowColumn.PrimaryDatumColumn] = cTextData.PrimaryDatum;
                    if (cTextData.PrimaryMaterialModifier != "" & cTextData.PrimaryMaterialModifier != "None")
                    {
                        string ValueStr = cTextData.PrimaryMaterialModifier;
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
                        oRng[sRowColumn.PrimaryMaterialModifierRow, sRowColumn.PrimaryMaterialModifierColumn] = ValueStr;
                    }
                    //Secondary
                    oRng[sRowColumn.SecondaryDatumRow, sRowColumn.SecondaryDatumColumn] = cTextData.SecondaryDatum;
                    if (cTextData.SecondaryMaterialModifier != "" & cTextData.SecondaryMaterialModifier != "None")
                    {
                        string ValueStr = cTextData.SecondaryMaterialModifier;
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
                        oRng[sRowColumn.SecondaryMaterialModifierRow, sRowColumn.SecondaryMaterialModifierColumn] = ValueStr;
                    }
                    //Tertiary
                    oRng[sRowColumn.TertiaryDatumRow, sRowColumn.TertiaryDatumColumn] = cTextData.TertiaryDatum;
                    if (cTextData.TertiaryMaterialModifier != "" & cTextData.TertiaryMaterialModifier != "None")
                    {
                        string ValueStr = cTextData.TertiaryMaterialModifier;
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
                        oRng[sRowColumn.TertiaryMaterialModifierRow, sRowColumn.TertiaryMaterialModifierColumn] = ValueStr;
                    }
                    #endregion
                }
                else if (cTextData.Type == "NXOpen.Annotations.Label")
                {
                    oRng[sRowColumn.MainTextRow, sRowColumn.MainTextColumn] = cTextData.MainText;
                    ((Range)oRng[sRowColumn.MainTextRow, sRowColumn.MainTextColumn]).Interior.ColorIndex = 50;
                }
                else 
                {
                    #region Dimension����
                    if (cTextData.BeforeText != null)
                    {
                        oRng[sRowColumn.BeforeTextRow, sRowColumn.BeforeTextColumn] = DefineVariable.GetGDTWord(cTextData.BeforeText);
                    }
                    oRng[sRowColumn.MainTextRow, sRowColumn.MainTextColumn] = DefineVariable.GetGDTWord(cTextData.MainText);
                    //Range a = (Range)oRng[sRowColumn.MainTextRow, sRowColumn.MainTextColumn];
                    //a.Interior.ColorIndex = 39;
                    oRng[sRowColumn.UpperTolRow, sRowColumn.UpperTolColumn] = cTextData.UpperTol;
                    if (cTextData.UpperTol != "" & cTextData.TolType == "BilateralOneLine")
                    {
                        oRng[sRowColumn.ToleranceSymbolRow, sRowColumn.ToleranceSymbolColumn] = "��";
                    }
                    #endregion
                }

                #region �˨�B�W�v�BMax�BMin�B�w�w�B�w�w��m�B�Ƹ��B���
                oRng[sRowColumn.GaugeRow, sRowColumn.GaugeColumn] = cTextData.Gauge;
                oRng[sRowColumn.FrequencyRow, sRowColumn.FrequencyColumn] = cTextData.Frequency;
                oRng[sRowColumn.BallonRow, sRowColumn.BallonColumn] = cTextData.BallonNum;
                oRng[sRowColumn.LocationRow, sRowColumn.LocationColumn] = cTextData.Location;
                oRng[sRowColumn.PartNoRow, sRowColumn.PartNoColumn] = PartNo;
                oRng[sRowColumn.DateRow, sRowColumn.DateColumn] = CurrentDate;
                #endregion
                
            }
            */
            
            book.SaveAs(OutputPath, Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            book.Close(Type.Missing, Type.Missing, Type.Missing);
            x.Quit();

            //���^�Ĥ@�iSheet
            DefineVariable.FirstDrawingSheet.Open();



            theProgram.Dispose();
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----

        }
        return retValue;
    }

    //------------------------------------------------------------------------------
    // Following method disposes all the class members
    //------------------------------------------------------------------------------
    public void Dispose()
    {
        try
        {
            if (isDisposeCalled == false)
            {
                //TODO: Add your application code here 
            }
            isDisposeCalled = true;
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----

        }
    }

    public static int GetUnloadOption(string arg)
    {
        //Unloads the image explicitly, via an unload dialog
        //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);

        //Unloads the image immediately after execution within NX
        return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);

        //Unloads the image when the NX session terminates
        // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
    }

}

