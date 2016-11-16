using System;
using NXOpen;
using NXOpen.UF;
using NXOpen.Annotations;
using CaxGlobaltek;
using System.Collections.Generic;
using CreateBallon;
using NXOpen.Utilities;
using NXOpen.Drawings;
using System.Windows.Forms;
using NXOpenUI;

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

            int module_id;
            theUfSession.UF.AskApplicationModule(out module_id);
            if (module_id != UFConstants.UF_APP_DRAFTING)
            {
                MessageBox.Show("�Х��ഫ���s�ϼҲի�A����I");
                return retValue;
            }

            bool status,Is_Keep;

            Application.EnableVisualStyles();
            CreateBallonDlg cCreateBallonDlg = new CreateBallonDlg();
            FormUtilities.ReparentForm(cCreateBallonDlg);
            System.Windows.Forms.Application.Run(cCreateBallonDlg);
            if (cCreateBallonDlg.DialogResult == DialogResult.Yes)
            {
                Is_Keep = cCreateBallonDlg.Is_Keep;
                cCreateBallonDlg.Dispose();
            }
            else
            {
                cCreateBallonDlg.Dispose();
                theProgram.Dispose();
                return retValue;
            }


            #region �e�m�B�z

            string Is_Local = Environment.GetEnvironmentVariable("UGII_ENV_FILE");

            CoordinateData cCoordinateData = new CoordinateData();
            if (Is_Local != null)
            {
                //���o�ϯȽd����Data
                status = CaxGetDatData.GetDraftingCoordinateData(out cCoordinateData);
                if (!status)
                {
                    return retValue;
                }
            }
            else
            {
                string DraftingCoordinate_dat = "DraftingCoordinate.dat";
                string DraftingCoordinate_Path = string.Format(@"{0}\{1}", "D:", DraftingCoordinate_dat);
                CaxPublic.ReadCoordinateData(DraftingCoordinate_Path, out cCoordinateData);
            }
            
            
            

            //�ϯȪ��B��
            double SheetLength = 0;
            double SheetHeight = 0;

            //����ثe�ϯȼƶq�MTag
            int SheetCount = 0;
            NXOpen.Tag[] SheetTagAry = null;
            theUfSession.Draw.AskDrawings(out SheetCount, out SheetTagAry);

            //���o�̫�@���w�w���Ʀr
            int MaxBallonNum;
            try
            {
                MaxBallonNum = Convert.ToInt32(workPart.GetStringAttribute(CaxME.DimenAttr.BallonNum));
            }
            catch (System.Exception ex)
            {
                MaxBallonNum = 0;
            }
            #endregion


            if (Is_Keep == false)
            {
                #region �R�������w�w
                IdSymbolCollection BallonCollection = workPart.Annotations.IdSymbols;
                IdSymbol[] BallonAry = BallonCollection.ToArray();
                foreach (IdSymbol i in BallonAry)
                {
                    CaxPublic.DelectObject(i);
                }
                workPart.DeleteAttributeByTypeAndTitle(NXObject.AttributeType.String, "BALLONNUM");
                #endregion
                
                #region �sDicDimenData(string=�˨�W��,DimenData=�ؤo����B�w�w�y��)
                DefineParam.DicDimenData = new Dictionary<string, List<DimenData>>();
                for (int i = 0; i < SheetCount; i++)
                {
                    //���}Sheet�ðO���Ҧ�OBJ
                    NXOpen.Drawings.DrawingSheet CurrentSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(SheetTagAry[i]);
                    string SheetName = "S" + (i + 1).ToString();
                    CaxME.SheetRename(CurrentSheet, SheetName);
                    CurrentSheet.Open();

                    if (i == 0)
                    {
                        DefineParam.FirstDrawingSheet = CurrentSheet;
                    }

                    //���o�ϯȪ��B��
                    SheetLength = CurrentSheet.Length;
                    SheetHeight = CurrentSheet.Height;

                    DisplayableObject[] SheetObj = CurrentSheet.View.AskVisibleObjects();
                    foreach (DisplayableObject singleObj in SheetObj)
                    {
                        string IPQC_Gauge = "", IQC_Gauge = "", FQC_Gauge = "", FAI_Gauge = "", AssignExcelType = "";
                        #region �P�_�O�_���ݩʡA�S���ݩʴN���U�@��
                        try
                        {
                            AssignExcelType = singleObj.GetStringAttribute(CaxME.DimenAttr.AssignExcelType);
                        }
                        catch (System.Exception ex)
                        {
                        	continue;
                        }
                        try
                        {
                            IPQC_Gauge = singleObj.GetStringAttribute(CaxME.DimenAttr.IPQC_Gauge);
                        }
                        catch (System.Exception ex)
                        {
                        	
                        }
                        /*
                        try
                        {
                            IPQC_Gauge = singleObj.GetStringAttribute(CaxME.DimenAttr.IPQC_Gauge);
                        }
                        catch (System.Exception ex)
                        {
                            
                        }
                        try
                        {
                            IQC_Gauge = singleObj.GetStringAttribute(CaxME.DimenAttr.IQC_Gauge);
                        }
                        catch (System.Exception ex)
                        {

                        }
                        try
                        {
                            FQC_Gauge = singleObj.GetStringAttribute(CaxME.DimenAttr.FQC_Gauge);
                        }
                        catch (System.Exception ex)
                        {

                        }
                        try
                        {
                            FAI_Gauge = singleObj.GetStringAttribute(CaxME.DimenAttr.FAI_Gauge);
                        }
                        catch (System.Exception ex)
                        {

                        }
                        if (IPQC_Gauge == "" & IQC_Gauge == "" & FQC_Gauge == "" & FAI_Gauge == "")
                        {
                            continue;
                        }
                        */
                        #endregion

                        //�ƥ���J�Ӥؤo�ҦbSheet
                        singleObj.SetAttribute("SheetName", SheetName);

                        string DimeType = "";
                        DimeType = singleObj.GetType().ToString();
                        CaxME.BoxCoordinate cBoxCoordinate = new CaxME.BoxCoordinate();
                        
                        if (DimeType == "NXOpen.Annotations.VerticalDimension")
                        {
                            #region VerticalDimension��Location
                            NXOpen.Annotations.VerticalDimension temp = (NXOpen.Annotations.VerticalDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.PerpendicularDimension")
                        {
                            #region PerpendicularDimension��Location
                            NXOpen.Annotations.PerpendicularDimension temp = (NXOpen.Annotations.PerpendicularDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.MinorAngularDimension")
                        {
                            #region MinorAngularDimension��Location
                            NXOpen.Annotations.MinorAngularDimension temp = (NXOpen.Annotations.MinorAngularDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.RadiusDimension")
                        {
                            #region MinorAngularDimension��Location
                            NXOpen.Annotations.RadiusDimension temp = (NXOpen.Annotations.RadiusDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.HorizontalDimension")
                        {
                            #region HorizontalDimension��Location
                            NXOpen.Annotations.HorizontalDimension temp = (NXOpen.Annotations.HorizontalDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.IdSymbol")
                        {
                            #region IdSymbol��Location
                            NXOpen.Annotations.IdSymbol temp = (NXOpen.Annotations.IdSymbol)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.Note")
                        {
                            #region Note��Location
                            NXOpen.Annotations.Note temp = (NXOpen.Annotations.Note)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.DraftingFcf")
                        {
                            #region DraftingFcf��Location
                            NXOpen.Annotations.DraftingFcf temp = (NXOpen.Annotations.DraftingFcf)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.Label")
                        {
                            #region Label��Location
                            NXOpen.Annotations.Label temp = (NXOpen.Annotations.Label)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.DraftingDatum")
                        {
                            #region DraftingDatum��Location
                            NXOpen.Annotations.DraftingDatum temp = (NXOpen.Annotations.DraftingDatum)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.DiameterDimension")
                        {
                            #region DiameterDimension��Location
                            NXOpen.Annotations.DiameterDimension temp = (NXOpen.Annotations.DiameterDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.AngularDimension")
                        {
                            #region AngularDimension��Location
                            NXOpen.Annotations.AngularDimension temp = (NXOpen.Annotations.AngularDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.CylindricalDimension")
                        {
                            #region CylindricalDimension��Location
                            NXOpen.Annotations.CylindricalDimension temp = (NXOpen.Annotations.CylindricalDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.ChamferDimension")
                        {
                            #region ChamferDimension��Location
                            NXOpen.Annotations.ChamferDimension temp = (NXOpen.Annotations.ChamferDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.HoleDimension")
                        {
                            #region HoleDimension��Location
                            NXOpen.Annotations.HoleDimension temp = (NXOpen.Annotations.HoleDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            //CaxLog.ShowListingWindow(cBoxCoordinate.lower_left[0].ToString());
                            //CaxLog.ShowListingWindow(cBoxCoordinate.lower_left[1].ToString());
                            //CaxLog.ShowListingWindow(cBoxCoordinate.lower_right[0].ToString());
                            //CaxLog.ShowListingWindow(cBoxCoordinate.lower_right[1].ToString());
                            //CaxLog.ShowListingWindow(cBoxCoordinate.upper_right[0].ToString());
                            //CaxLog.ShowListingWindow(cBoxCoordinate.upper_right[1].ToString());
                            //CaxLog.ShowListingWindow(cBoxCoordinate.upper_left[0].ToString());
                            //CaxLog.ShowListingWindow(cBoxCoordinate.upper_left[1].ToString());
                            //CaxLog.ShowListingWindow("----");
                            //CaxME.DrawTextBox(temp.Tag);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.ParallelDimension")
                        {
                            #region ParallelDimension��Location
                            NXOpen.Annotations.ParallelDimension temp = (NXOpen.Annotations.ParallelDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.FoldedRadiusDimension")
                        {
                            #region FoldedRadiusDimension��Location
                            NXOpen.Annotations.FoldedRadiusDimension temp = (NXOpen.Annotations.FoldedRadiusDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.ArcLengthDimension")
                        {
                            #region ArcLengthDimension��Location
                            NXOpen.Annotations.ArcLengthDimension temp = (NXOpen.Annotations.ArcLengthDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                            

                        #region �p��w�w�y��
                        DimenData sDimenData = new DimenData();
                        sDimenData.Obj = singleObj;
                        sDimenData.CurrentSheet = CurrentSheet;
                        Functions.CalculateBallonCoordinate(cBoxCoordinate, ref sDimenData);
                        #endregion

                        if (IPQC_Gauge != "")
                        {
                            List<DimenData> ListDimenData = new List<DimenData>();
                            status = DefineParam.DicDimenData.TryGetValue(IPQC_Gauge, out ListDimenData);
                            if (status)
                            {
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData[IPQC_Gauge] = ListDimenData;
                            }
                            else
                            {
                                ListDimenData = new List<DimenData>();
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData.Add(IPQC_Gauge, ListDimenData);
                            }
                        }
                        else
                        {
                            List<DimenData> ListDimenData = new List<DimenData>();
                            status = DefineParam.DicDimenData.TryGetValue(AssignExcelType, out ListDimenData);
                            if (status)
                            {
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData[AssignExcelType] = ListDimenData;
                            }
                            else
                            {
                                ListDimenData = new List<DimenData>();
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData.Add(AssignExcelType, ListDimenData);
                            }
                        }
                        /*
                        if (IQC_Gauge != "")
                        {
                            List<DimenData> ListDimenData = new List<DimenData>();
                            status = DefineParam.DicDimenData.TryGetValue(IQC_Gauge, out ListDimenData);
                            if (status)
                            {
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData[IQC_Gauge] = ListDimenData;
                            }
                            else
                            {
                                ListDimenData = new List<DimenData>();
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData.Add(IQC_Gauge, ListDimenData);
                            }
                        }
                        if (FAI_Gauge != "")
                        {
                            List<DimenData> ListDimenData = new List<DimenData>();
                            status = DefineParam.DicDimenData.TryGetValue(FAI_Gauge, out ListDimenData);
                            if (status)
                            {
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData[FAI_Gauge] = ListDimenData;
                            }
                            else
                            {
                                ListDimenData = new List<DimenData>();
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData.Add(FAI_Gauge, ListDimenData);
                            }
                        }
                        if (FQC_Gauge != "")
                        {
                            List<DimenData> ListDimenData = new List<DimenData>();
                            status = DefineParam.DicDimenData.TryGetValue(FQC_Gauge, out ListDimenData);
                            if (status)
                            {
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData[FQC_Gauge] = ListDimenData;
                            }
                            else
                            {
                                ListDimenData = new List<DimenData>();
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData.Add(FQC_Gauge, ListDimenData);
                            }
                        }
                        */
                    }
                }
                #endregion

                #region ���J�w�w
                int BallonNum = 0;
                double BallonNumSize = 0;
                foreach (KeyValuePair<string, List<DimenData>> kvp in DefineParam.DicDimenData)
                {
                    List<DimenData> tempListDimenData = new List<DimenData>();
                    DefineParam.DicDimenData.TryGetValue(kvp.Key, out tempListDimenData);
                    for (int i = 0; i < tempListDimenData.Count; i++)
                    {
                        tempListDimenData[i].CurrentSheet.Open();
                        BallonNum++;
                        Point3d BallonLocation = new Point3d();
                        BallonLocation.X = tempListDimenData[i].LocationX;
                        BallonLocation.Y = tempListDimenData[i].LocationY;

                        //�M�w�Ʀr���j�p
                        if (BallonNum <= 9)
                        {
                            BallonNumSize = 2.5;
                        }
                        else if (BallonNum > 9 && BallonNum <= 99)
                        {
                            BallonNumSize = 1.5;
                        }
                        else
                        {
                            BallonNumSize = 1;
                        }
                        CaxME.CreateBallonOnSheet(BallonNum.ToString(), BallonLocation, BallonNumSize);

                        //���o�Ӥؤo�Ҧb�ϯ�
                        string SheetNum = tempListDimenData[i].Obj.GetStringAttribute("SheetName");
                        #region �p��w�w�۹��m
                        //�p��w�w�۹��m
                        string RegionX = "", RegionY = "";
                        for (int ii = 0; ii < cCoordinateData.DraftingCoordinate.Count; ii++)
                        {
                            string SheetSize = cCoordinateData.DraftingCoordinate[ii].SheetSize;
                            if (Math.Ceiling(SheetHeight) != Convert.ToDouble(SheetSize.Split(',')[0]) || Math.Ceiling(SheetLength) != Convert.ToDouble(SheetSize.Split(',')[1]))
                            {
                                continue;
                            }
                            //���X
                            for (int j = 0; j < cCoordinateData.DraftingCoordinate[ii].RegionX.Count; j++)
                            {
                                string X0 = cCoordinateData.DraftingCoordinate[ii].RegionX[j].X0;
                                string X1 = cCoordinateData.DraftingCoordinate[ii].RegionX[j].X1;
                                if (BallonLocation.X >= Convert.ToDouble(X0) && BallonLocation.X <= Convert.ToDouble(X1))
                                {
                                    RegionX = cCoordinateData.DraftingCoordinate[ii].RegionX[j].Zone;
                                }
                            }
                            //���Y
                            for (int j = 0; j < cCoordinateData.DraftingCoordinate[ii].RegionY.Count; j++)
                            {
                                string Y0 = cCoordinateData.DraftingCoordinate[ii].RegionY[j].Y0;
                                string Y1 = cCoordinateData.DraftingCoordinate[ii].RegionY[j].Y1;
                                if (BallonLocation.Y >= Convert.ToDouble(Y0) && BallonLocation.Y <= Convert.ToDouble(Y1))
                                {
                                    RegionY = cCoordinateData.DraftingCoordinate[ii].RegionY[j].Zone;
                                }
                            }
                        }
                        #endregion
                        tempListDimenData[i].Obj.SetAttribute(CaxME.DimenAttr.BallonNum, BallonNum.ToString());
                        tempListDimenData[i].Obj.SetAttribute(CaxME.DimenAttr.BallonLocation, SheetNum + "-" + RegionY + RegionX);
                    }
                }
                #endregion

                //�N�̫�@���w�w���Ʀr���J�s��
                workPart.SetAttribute(CaxME.DimenAttr.BallonNum, BallonNum.ToString());
            }
            else
            {
                #region �sDicDimenData(string=�˨�W��,DimenData=�ؤo����B�w�w�y��)
                DefineParam.DicDimenData = new Dictionary<string, List<DimenData>>();
                for (int i = 0; i < SheetCount; i++)
                {
                    //���}Sheet�ðO���Ҧ�OBJ
                    NXOpen.Drawings.DrawingSheet CurrentSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(SheetTagAry[i]);
                    string SheetName = "S" + (i + 1).ToString();
                    CaxME.SheetRename(CurrentSheet, SheetName);
                    CurrentSheet.Open();

                    if (i == 0)
                    {
                        DefineParam.FirstDrawingSheet = CurrentSheet;
                    }

                    //���o�ϯȪ��B��
                    SheetLength = CurrentSheet.Length;
                    SheetHeight = CurrentSheet.Height;

                    DisplayableObject[] SheetObj = CurrentSheet.View.AskVisibleObjects();
                    foreach (DisplayableObject singleObj in SheetObj)
                    {
                        string IPQC_Gauge = "", IQC_Gauge = "", FQC_Gauge = "", FAI_Gauge = "", AssignExcelType = "";
                        #region �P�_�O�_���ݩʡA�S���ݩʴN���U�@��
                        try
                        {
                            AssignExcelType = singleObj.GetStringAttribute(CaxME.DimenAttr.AssignExcelType);
                        }
                        catch (System.Exception ex)
                        {
                            continue;
                        }
                        try
                        {
                            IPQC_Gauge = singleObj.GetStringAttribute(CaxME.DimenAttr.IPQC_Gauge);
                        }
                        catch (System.Exception ex)
                        {

                        }

                        /*
                        try
                        {
                            IPQC_Gauge = singleObj.GetStringAttribute(CaxME.DimenAttr.IPQC_Gauge);
                        }
                        catch (System.Exception ex)
                        {
                            
                        }
                        try
                        {
                            IQC_Gauge = singleObj.GetStringAttribute(CaxME.DimenAttr.IQC_Gauge);
                        }
                        catch (System.Exception ex)
                        {

                        }
                        try
                        {
                            FQC_Gauge = singleObj.GetStringAttribute(CaxME.DimenAttr.FQC_Gauge);
                        }
                        catch (System.Exception ex)
                        {

                        }
                        try
                        {
                            FAI_Gauge = singleObj.GetStringAttribute(CaxME.DimenAttr.FAI_Gauge);
                        }
                        catch (System.Exception ex)
                        {

                        }

                        if (IPQC_Gauge == "" & IQC_Gauge == "" & FQC_Gauge == "" & FAI_Gauge == "")
                        {
                            continue;
                        }
                        */
                        #endregion

                        string OldBallonNum = "";
                        #region �P�_�O�_�����ª��ؤo�A�p�G�O�N���U�@��
                        try
                        {
                            OldBallonNum = singleObj.GetStringAttribute(CaxME.DimenAttr.BallonNum);
                            continue;
                        }
                        catch (System.Exception ex)
                        {
                            
                        }
                        #endregion

                        //�ƥ���J�Ӥؤo�ҦbSheet
                        singleObj.SetAttribute("SheetName", SheetName);

                        string DimeType = "";
                        DimeType = singleObj.GetType().ToString();
                        CaxME.BoxCoordinate cBoxCoordinate = new CaxME.BoxCoordinate();

                        if (DimeType == "NXOpen.Annotations.VerticalDimension")
                        {
                            #region VerticalDimension��Location
                            NXOpen.Annotations.VerticalDimension temp = (NXOpen.Annotations.VerticalDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.PerpendicularDimension")
                        {
                            #region PerpendicularDimension��Location
                            NXOpen.Annotations.PerpendicularDimension temp = (NXOpen.Annotations.PerpendicularDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.MinorAngularDimension")
                        {
                            #region MinorAngularDimension��Location
                            NXOpen.Annotations.MinorAngularDimension temp = (NXOpen.Annotations.MinorAngularDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.RadiusDimension")
                        {
                            #region MinorAngularDimension��Location
                            NXOpen.Annotations.RadiusDimension temp = (NXOpen.Annotations.RadiusDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.HorizontalDimension")
                        {
                            #region HorizontalDimension��Location
                            NXOpen.Annotations.HorizontalDimension temp = (NXOpen.Annotations.HorizontalDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.IdSymbol")
                        {
                            #region IdSymbol��Location
                            NXOpen.Annotations.IdSymbol temp = (NXOpen.Annotations.IdSymbol)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.Note")
                        {
                            #region Note��Location
                            NXOpen.Annotations.Note temp = (NXOpen.Annotations.Note)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.DraftingFcf")
                        {
                            #region DraftingFcf��Location
                            NXOpen.Annotations.DraftingFcf temp = (NXOpen.Annotations.DraftingFcf)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.Label")
                        {
                            #region Label��Location
                            NXOpen.Annotations.Label temp = (NXOpen.Annotations.Label)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.DraftingDatum")
                        {
                            #region DraftingDatum��Location
                            NXOpen.Annotations.DraftingDatum temp = (NXOpen.Annotations.DraftingDatum)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.DiameterDimension")
                        {
                            #region DiameterDimension��Location
                            NXOpen.Annotations.DiameterDimension temp = (NXOpen.Annotations.DiameterDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.AngularDimension")
                        {
                            #region AngularDimension��Location
                            NXOpen.Annotations.AngularDimension temp = (NXOpen.Annotations.AngularDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.CylindricalDimension")
                        {
                            #region CylindricalDimension��Location
                            NXOpen.Annotations.CylindricalDimension temp = (NXOpen.Annotations.CylindricalDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }
                        else if (DimeType == "NXOpen.Annotations.ChamferDimension")
                        {
                            #region ChamferDimension��Location
                            NXOpen.Annotations.ChamferDimension temp = (NXOpen.Annotations.ChamferDimension)singleObj;
                            CaxME.GetTextBoxCoordinate(temp.Tag, out cBoxCoordinate);
                            #endregion
                        }

                        #region �p��w�w�y��
                        DimenData sDimenData = new DimenData();
                        sDimenData.Obj = singleObj;
                        sDimenData.CurrentSheet = CurrentSheet;
                        if (Math.Abs(cBoxCoordinate.upper_left[0] - cBoxCoordinate.lower_left[0]) < 0.01)
                        {
                            sDimenData.LocationX = (cBoxCoordinate.upper_left[0] + cBoxCoordinate.lower_left[0]) / 2 - 2;
                            sDimenData.LocationY = (cBoxCoordinate.upper_left[1] + cBoxCoordinate.lower_left[1]) / 2;
                        }
                        else
                        {
                            sDimenData.LocationX = (cBoxCoordinate.upper_left[0] + cBoxCoordinate.lower_left[0]) / 2;
                            sDimenData.LocationY = (cBoxCoordinate.upper_left[1] + cBoxCoordinate.lower_left[1]) / 2 - 2;
                        }
                        sDimenData.LocationZ = 0;
                        #endregion

                        
                        if (IPQC_Gauge != "")
                        {
                            List<DimenData> ListDimenData = new List<DimenData>();
                            status = DefineParam.DicDimenData.TryGetValue(IPQC_Gauge, out ListDimenData);
                            if (status)
                            {
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData[IPQC_Gauge] = ListDimenData;
                            }
                            else
                            {
                                ListDimenData = new List<DimenData>();
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData.Add(IPQC_Gauge, ListDimenData);
                            }
                        }
                        else
                        {
                            List<DimenData> ListDimenData = new List<DimenData>();
                            status = DefineParam.DicDimenData.TryGetValue(AssignExcelType, out ListDimenData);
                            if (status)
                            {
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData[AssignExcelType] = ListDimenData;
                            }
                            else
                            {
                                ListDimenData = new List<DimenData>();
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData.Add(AssignExcelType, ListDimenData);
                            }
                        }
                        /*
                        if (IQC_Gauge != "")
                        {
                            List<DimenData> ListDimenData = new List<DimenData>();
                            status = DefineParam.DicDimenData.TryGetValue(IQC_Gauge, out ListDimenData);
                            if (status)
                            {
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData[IQC_Gauge] = ListDimenData;
                            }
                            else
                            {
                                ListDimenData = new List<DimenData>();
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData.Add(IQC_Gauge, ListDimenData);
                            }
                        }
                        if (FAI_Gauge != "")
                        {
                            List<DimenData> ListDimenData = new List<DimenData>();
                            status = DefineParam.DicDimenData.TryGetValue(FAI_Gauge, out ListDimenData);
                            if (status)
                            {
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData[FAI_Gauge] = ListDimenData;
                            }
                            else
                            {
                                ListDimenData = new List<DimenData>();
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData.Add(FAI_Gauge, ListDimenData);
                            }
                        }
                        if (FQC_Gauge != "")
                        {
                            List<DimenData> ListDimenData = new List<DimenData>();
                            status = DefineParam.DicDimenData.TryGetValue(FQC_Gauge, out ListDimenData);
                            if (status)
                            {
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData[FQC_Gauge] = ListDimenData;
                            }
                            else
                            {
                                ListDimenData = new List<DimenData>();
                                ListDimenData.Add(sDimenData);
                                DefineParam.DicDimenData.Add(FQC_Gauge, ListDimenData);
                            }
                        }
                        */
                    }
                }
                #endregion

                #region ���J�w�w
                double BallonNumSize = 0;
                foreach (KeyValuePair<string, List<DimenData>> kvp in DefineParam.DicDimenData)
                {
                    List<DimenData> tempListDimenData = new List<DimenData>();
                    DefineParam.DicDimenData.TryGetValue(kvp.Key, out tempListDimenData);
                    for (int i = 0; i < tempListDimenData.Count; i++)
                    {
                        tempListDimenData[i].CurrentSheet.Open();
                        MaxBallonNum++;
                        Point3d BallonLocation = new Point3d();
                        BallonLocation.X = tempListDimenData[i].LocationX;
                        BallonLocation.Y = tempListDimenData[i].LocationY;
                        //�M�w�Ʀr���j�p
                        if (MaxBallonNum <= 9)
                        {
                            BallonNumSize = 2.5;
                        }
                        else if (MaxBallonNum > 9 && MaxBallonNum <= 99)
                        {
                            BallonNumSize = 1.5;
                        }
                        else
                        {
                            BallonNumSize = 1;
                        }
                        CaxME.CreateBallonOnSheet(MaxBallonNum.ToString(), BallonLocation, BallonNumSize);

                        //���o�Ӥؤo�Ҧb�ϯ�
                        string SheetNum = tempListDimenData[i].Obj.GetStringAttribute("SheetName");
                        #region �p��w�w�۹��m
                        //�p��w�w�۹��m
                        string RegionX = "", RegionY = "";
                        for (int ii = 0; ii < cCoordinateData.DraftingCoordinate.Count; ii++)
                        {
                            string SheetSize = cCoordinateData.DraftingCoordinate[ii].SheetSize;
                            if (Math.Ceiling(SheetHeight) != Convert.ToDouble(SheetSize.Split(',')[0]) || Math.Ceiling(SheetLength) != Convert.ToDouble(SheetSize.Split(',')[1]))
                            {
                                continue;
                            }
                            //���X
                            for (int j = 0; j < cCoordinateData.DraftingCoordinate[ii].RegionX.Count; j++)
                            {
                                string X0 = cCoordinateData.DraftingCoordinate[ii].RegionX[j].X0;
                                string X1 = cCoordinateData.DraftingCoordinate[ii].RegionX[j].X1;
                                if (BallonLocation.X >= Convert.ToDouble(X0) && BallonLocation.X <= Convert.ToDouble(X1))
                                {
                                    RegionX = cCoordinateData.DraftingCoordinate[ii].RegionX[j].Zone;
                                }
                            }
                            //���Y
                            for (int j = 0; j < cCoordinateData.DraftingCoordinate[ii].RegionY.Count; j++)
                            {
                                string Y0 = cCoordinateData.DraftingCoordinate[ii].RegionY[j].Y0;
                                string Y1 = cCoordinateData.DraftingCoordinate[ii].RegionY[j].Y1;
                                if (BallonLocation.Y >= Convert.ToDouble(Y0) && BallonLocation.Y <= Convert.ToDouble(Y1))
                                {
                                    RegionY = cCoordinateData.DraftingCoordinate[ii].RegionY[j].Zone;
                                }
                            }
                        }
                        #endregion
                        tempListDimenData[i].Obj.SetAttribute(CaxME.DimenAttr.BallonNum, MaxBallonNum.ToString());
                        tempListDimenData[i].Obj.SetAttribute(CaxME.DimenAttr.BallonLocation, SheetNum + "-" + RegionY + RegionX);
                    }
                }
                #endregion

                //�N�̫�@���w�w���Ʀr���J�s��
                workPart.SetAttribute(CaxME.DimenAttr.BallonNum, MaxBallonNum.ToString());
            }

            //���^�Ĥ@�iSheet
            DefineParam.FirstDrawingSheet.Open();
            
            
            








            /*
            DefineParam.DicSheetData_IPQC = new Dictionary<NXOpen.Drawings.DrawingSheet, List<DisplayableObject>>();
            DefineParam.DicSheetData_Self = new Dictionary<NXOpen.Drawings.DrawingSheet, List<DisplayableObject>>();
            List<DisplayableObject> ListAnn_IPQC = new List<DisplayableObject>();
            List<DisplayableObject> ListAnn_Self = new List<DisplayableObject>();
            for (int i = 0; i < SheetCount; i++)
            {
                //���}Sheet�ðO���Ҧ�OBJ
                NXOpen.Drawings.DrawingSheet CurrentSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(SheetTagAry[i]);
                NXOpen.Drawings.DrawingSheet drawingSheet = (NXOpen.Drawings.DrawingSheet)workPart.DrawingSheets.FindObject(CurrentSheet.Name);
                drawingSheet.Open();
                DisplayableObject[] SheetObj = drawingSheet.View.AskVisibleObjects();
                foreach (DisplayableObject singleObj in SheetObj)
                {
                    string IPQC_Gauge = "";
                    string SelfCheck_Gauge = "";
                    NXOpen.NXObject ann = (NXOpen.NXObject)singleObj;
                    //NXOpen.Annotations.Annotation ann = (NXOpen.Annotations.Annotation)obj;
                    #region ��IPQC��SelfCheck�ݩ�
                    try
                    {
                        IPQC_Gauge = ann.GetStringAttribute(CaxME.DimenAttr.IPQC_Gauge);
                    }
                    catch (System.Exception ex)
                    {
                        IPQC_Gauge = "";
                    }

                    try
                    {
                        SelfCheck_Gauge = ann.GetStringAttribute(CaxME.DimenAttr.SelfCheck_Gauge);
                    }
                    catch (System.Exception ex)
                    {
                        SelfCheck_Gauge = "";
                    }
                    #endregion

                    if (IPQC_Gauge == "" & SelfCheck_Gauge == "")
                    {
                        continue;
                    }

                    if (IPQC_Gauge != "")
                    {
                        ListAnn_IPQC.Add(singleObj);
                        DefineParam.DicSheetData_IPQC[drawingSheet] = ListAnn_IPQC;
                        //status = DefineParam.DicSheetData_IPQC.TryGetValue(drawingSheet, out ListAnn_IPQC);
                        //if (status)
                        //{

                        //}
                        //else
                        //{
                        //    ListAnn_IPQC.Add(singleObj);
                        //    DefineParam.DicSheetData_IPQC.Add(drawingSheet, ListAnn_IPQC);
                        //}
                    }
                    if (SelfCheck_Gauge != "")
                    {
                        ListAnn_Self.Add(singleObj);
                        DefineParam.DicSheetData_Self[drawingSheet] = ListAnn_Self;
                    }
                }

                #region ���Ѥ�
                
                foreach (DisplayableObject singleObj in SheetObj)
                {
                    string singleObjType = singleObj.GetType().ToString();
                    string IPQC_Gauge = "";
                    string SelfCheck_Gauge = "";
                    NXOpen.Annotations.Annotation ann = null;

                    #region �ഫ���A�A�æs��Annotation
                    if (singleObjType == "NXOpen.Annotations.VerticalDimension")
                    {
                        NXOpen.Annotations.VerticalDimension temp = (NXOpen.Annotations.VerticalDimension)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.PerpendicularDimension")
                    {
                        NXOpen.Annotations.PerpendicularDimension temp = (NXOpen.Annotations.PerpendicularDimension)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.MinorAngularDimension")
                    {
                        NXOpen.Annotations.MinorAngularDimension temp = (NXOpen.Annotations.MinorAngularDimension)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.RadiusDimension")
                    {
                        NXOpen.Annotations.RadiusDimension temp = (NXOpen.Annotations.RadiusDimension)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.HorizontalDimension")
                    {
                        NXOpen.Annotations.HorizontalDimension temp = (NXOpen.Annotations.HorizontalDimension)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.IdSymbol")
                    {
                        NXOpen.Annotations.IdSymbol temp = (NXOpen.Annotations.IdSymbol)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.Note")
                    {
                        NXOpen.Annotations.Note temp = (NXOpen.Annotations.Note)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.DraftingFcf")
                    {
                        NXOpen.Annotations.DraftingFcf temp = (NXOpen.Annotations.DraftingFcf)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.Label")
                    {
                        NXOpen.Annotations.Label temp = (NXOpen.Annotations.Label)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.DraftingDatum")
                    {
                        NXOpen.Annotations.DraftingDatum temp = (NXOpen.Annotations.DraftingDatum)singleObj;
                        ann = temp;
                    }
                    #endregion

                    #region ��IPQC��SelfCheck�ݩ�
                    try
                    {
                        IPQC_Gauge = ann.GetStringAttribute(CaxME.DimenAttr.IPQC_Gauge);
                    }
                    catch (System.Exception ex)
                    {
                        IPQC_Gauge = "";
                    }

                    try
                    {
                        SelfCheck_Gauge = ann.GetStringAttribute(CaxME.DimenAttr.SelfCheck_Gauge);
                    }
                    catch (System.Exception ex)
                    {
                        SelfCheck_Gauge = "";
                    }
                    #endregion
                    
                    if (IPQC_Gauge == "" & SelfCheck_Gauge == "")
                    {
                        continue;
                    }

                    CaxME.BoxCoordinate TextCoordi = new CaxME.BoxCoordinate();
                    Point3d CreateBallonPt = new Point3d();
                    CaxME.GetTextBoxCoordinate(ann.Tag, out TextCoordi);
                    if (Math.Abs(TextCoordi.upper_left[0] - TextCoordi.lower_left[0]) < 0.01)
                    {
                        CreateBallonPt.X = (TextCoordi.upper_left[0] + TextCoordi.lower_left[0]) / 2 - 2;
                        CreateBallonPt.Y = (TextCoordi.upper_left[1] + TextCoordi.lower_left[1]) / 2;
                    }
                    else
                    {
                        CreateBallonPt.X = (TextCoordi.upper_left[0] + TextCoordi.lower_left[0]) / 2;
                        CreateBallonPt.Y = (TextCoordi.upper_left[1] + TextCoordi.lower_left[1]) / 2 - 2;
                    }

                    Functions.CreateBallonOnSheet("3", CreateBallonPt);

                }
                
                #endregion
            }
            */

            /*
            int BallonNum = 0;
            DefineParam.DicDimenData_IPQC = new Dictionary<DisplayableObject, TextData>();
            foreach (KeyValuePair<DrawingSheet, List<DisplayableObject>> kvp in DefineParam.DicSheetData_IPQC)
            {
                kvp.Key.Open();

                foreach (DisplayableObject singleObj in kvp.Value)
                {
                    string singleObjType = singleObj.GetType().ToString();
                    string IPQC_Gauge = "";
                    string SelfCheck_Gauge = "";
                    NXOpen.Annotations.Annotation ann = null;
                    TextData cTextData = new TextData();
                    string[] mainText;
                    string[] dualText;

                    #region �ഫ���A�A�æs��Annotation
                    if (singleObjType == "NXOpen.Annotations.VerticalDimension")
                    {
                        NXOpen.Annotations.VerticalDimension temp = (NXOpen.Annotations.VerticalDimension)singleObj;
                        temp.GetDimensionText(out mainText, out dualText);
                        cTextData.mainText = mainText[0];
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
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.PerpendicularDimension")
                    {
                        NXOpen.Annotations.PerpendicularDimension temp = (NXOpen.Annotations.PerpendicularDimension)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.MinorAngularDimension")
                    {
                        NXOpen.Annotations.MinorAngularDimension temp = (NXOpen.Annotations.MinorAngularDimension)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.RadiusDimension")
                    {
                        NXOpen.Annotations.RadiusDimension temp = (NXOpen.Annotations.RadiusDimension)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.HorizontalDimension")
                    {
                        NXOpen.Annotations.HorizontalDimension temp = (NXOpen.Annotations.HorizontalDimension)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.IdSymbol")
                    {
                        NXOpen.Annotations.IdSymbol temp = (NXOpen.Annotations.IdSymbol)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.Note")
                    {
                        NXOpen.Annotations.Note temp = (NXOpen.Annotations.Note)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.DraftingFcf")
                    {
                        NXOpen.Annotations.DraftingFcf temp = (NXOpen.Annotations.DraftingFcf)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.Label")
                    {
                        NXOpen.Annotations.Label temp = (NXOpen.Annotations.Label)singleObj;
                        ann = temp;
                    }
                    else if (singleObjType == "NXOpen.Annotations.DraftingDatum")
                    {
                        NXOpen.Annotations.DraftingDatum temp = (NXOpen.Annotations.DraftingDatum)singleObj;
                        ann = temp;
                    }
                    #endregion

                    #region ��IPQC��SelfCheck�ݩ�
                    try
                    {
                        IPQC_Gauge = ann.GetStringAttribute(CaxME.DimenAttr.IPQC_Gauge);
                    }
                    catch (System.Exception ex)
                    {
                        IPQC_Gauge = "";
                    }

                    try
                    {
                        SelfCheck_Gauge = ann.GetStringAttribute(CaxME.DimenAttr.SelfCheck_Gauge);
                    }
                    catch (System.Exception ex)
                    {
                        SelfCheck_Gauge = "";
                    }
                    #endregion

                    if (IPQC_Gauge == "" & SelfCheck_Gauge == "")
                    {
                        continue;
                    }

                    //�w�w���ǼW�[
                    BallonNum++;

                    //���o�w�w���y��
                    CaxME.BoxCoordinate TextCoordi = new CaxME.BoxCoordinate();
                    Point3d CreateBallonPt = new Point3d();
                    CaxME.GetTextBoxCoordinate(ann.Tag, out TextCoordi);
                    if (Math.Abs(TextCoordi.upper_left[0] - TextCoordi.lower_left[0]) < 0.01)
                    {
                        CreateBallonPt.X = (TextCoordi.upper_left[0] + TextCoordi.lower_left[0]) / 2 - 2;
                        CreateBallonPt.Y = (TextCoordi.upper_left[1] + TextCoordi.lower_left[1]) / 2;
                    }
                    else
                    {
                        CreateBallonPt.X = (TextCoordi.upper_left[0] + TextCoordi.lower_left[0]) / 2;
                        CreateBallonPt.Y = (TextCoordi.upper_left[1] + TextCoordi.lower_left[1]) / 2 - 2;
                    }

                    Functions.CreateBallonOnSheet(BallonNum.ToString(), CreateBallonPt);
                }
            }
            */




            /*
            List<NXObject> ListNXObject = new List<NXObject>();

            //���oFeature Control Frame
            GdtCollection Gdts = workPart.Gdts;
            Gdt[] GdtAry = Gdts.ToArray();
            foreach (Gdt i in GdtAry)
            {
                ListNXObject.Add(i);
            }

            //���oDimension
            DimensionCollection DimenCollection = workPart.Dimensions;
            Dimension[] DimenAry = DimenCollection.ToArray();
            foreach (Dimension i in DimenAry)
            {
                ListNXObject.Add(i);
            }

            
            foreach (NXObject i in ListNXObject)
            {
                string IPQC_Gauge = "";
                string SelfCheck_Gauge = "";
                NXOpen.Annotations.Annotation ann = (NXOpen.Annotations.Annotation)i;
                try
                {
                    IPQC_Gauge = ann.GetStringAttribute(CaxME.DimenAttr.IPQC_Gauge);
                }
                catch (System.Exception ex)
                {
                    IPQC_Gauge = "";
                }

                try
                {
                    SelfCheck_Gauge = ann.GetStringAttribute(CaxME.DimenAttr.SelfCheck_Gauge);
                }
                catch (System.Exception ex)
                {
                    SelfCheck_Gauge = "";
                }

                if (IPQC_Gauge == "" & SelfCheck_Gauge == "")
                {
                    continue;
                }

                CaxME.BoxCoordinate TextCoordi = new CaxME.BoxCoordinate();
                Point3d CreateBallonPt = new Point3d();
                CaxME.GetTextBoxCoordinate(i.Tag, out TextCoordi);
                if (Math.Abs(TextCoordi.upper_left[0] - TextCoordi.lower_left[0]) < 0.01)
                {
                    CreateBallonPt.X = (TextCoordi.upper_left[0] + TextCoordi.lower_left[0]) / 2 - 1.5;
                    CreateBallonPt.Y = (TextCoordi.upper_left[1] + TextCoordi.lower_left[1]) / 2;
                }
                else
                {
                    CreateBallonPt.X = (TextCoordi.upper_left[0] + TextCoordi.lower_left[0]) / 2;
                    CreateBallonPt.Y = (TextCoordi.upper_left[1] + TextCoordi.lower_left[1]) / 2 - 1.5;
                }
                
                Functions.CreateBallonOnSheet("2", CreateBallonPt);
            }
            */

            theProgram.Dispose();
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----
            CaxLog.ShowListingWindow(ex.ToString());
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

