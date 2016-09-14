using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using CaxGlobaltek;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;

namespace AssignGauge
{
    public partial class AssignGaugeDlg : Office2007Form
    {
        public static Session theSession = Session.GetSession();
        public static UI theUI;
        public static UFSession theUfSession = UFSession.GetUFSession();
        public static Part workPart = theSession.Parts.Work;
        public static Part displayPart = theSession.Parts.Display;
        

        public static bool status;
        public static Dictionary<string, GaugeData> DicGaugeData = new Dictionary<string, GaugeData>();
        public static Dictionary<NXObject, string> DicSelDimension = new Dictionary<NXObject, string>();
        public static int BallonNum = 0;
        public static CoordinateData cCoordinateData = new CoordinateData();
        public static double SheetLength = 0.0, SheetHeight = 0.0;

        public class GaugeData
        {
            public string Color { get; set; }
            public string EngName { get; set; }
        }

        public AssignGaugeDlg()
        {
            InitializeComponent();
        }

        private void AssignGaugeDlg_Load(object sender, EventArgs e)
        {
            /*
            //取回目前泡泡最大值
            try
            {
                BallonNum = Convert.ToInt32(workPart.GetStringAttribute(CaxME.DimenAttr.BallonNum));
            }
            catch (System.Exception ex)
            {
                BallonNum = 0;
            }
            */

            //預設關閉選擇物件
            SelectObject.Enabled = false;

            //取得AssignGaugeData
            string[] AGData = new string[] { };
            status = CaxGetDatData.GetAssignGaugeData(out AGData);
            if (!status)
            {
                CaxLog.ShowListingWindow("GetAssignGaugeData失敗，請檢查MEConfig是否有檔案");
                return;
            }

            #region 存AGData到DicGaugeData中
            foreach (string Row in AGData)
            {
                string[] splitRow = Row.Split(',');
                if (splitRow.Length == 0)
                {
                    continue;
                }
                
                GaugeData cGaugeData = new GaugeData();
                status = DicGaugeData.TryGetValue(splitRow[1], out cGaugeData);
                if (status)
                {
                    continue;
                }

                cGaugeData = new GaugeData();
                cGaugeData.Color = splitRow[0];
                try
                {
                    cGaugeData.EngName = splitRow[2];
                }
                catch (System.Exception ex)
                {
                    cGaugeData.EngName = "";
                }
                DicGaugeData.Add(splitRow[1], cGaugeData);
            }
            #endregion

            //填檢具到IQC、IPQC下拉選單中
            IQCGauge.Items.Add("");
            IQCGauge.Items.AddRange(DicGaugeData.Keys.ToArray());
            IPQCGauge.Items.Add("");
            IPQCGauge.Items.AddRange(DicGaugeData.Keys.ToArray());
            //填檢具到SelfCheck下拉選單中
            SelfCheckGauge.Items.Add("");
            foreach (KeyValuePair<string,GaugeData> kvp in DicGaugeData)
            {
                if (kvp.Key.Contains("T"))
                {
                    continue;
                }
                SelfCheckGauge.Items.Add(kvp.Key);
            }

            //取得sheet並填入下拉選單中
            int SheetCount = 0;
            NXOpen.Tag[] SheetTagAry = null;
            theUfSession.Draw.AskDrawings(out SheetCount, out SheetTagAry);
            for (int i = 0; i < SheetCount; i++)
            {
                NXOpen.Drawings.DrawingSheet CurrentSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(SheetTagAry[i]);
                ListSheet.Items.Add(CurrentSheet.Name);
            }

            //預設開啟sheet1圖紙
            NXOpen.Drawings.DrawingSheet DefaultSheet = (NXOpen.Drawings.DrawingSheet)NXObjectManager.Get(SheetTagAry[0]);
            ListSheet.Text = DefaultSheet.Name;

            //取得圖紙長寬
            SheetLength = DefaultSheet.Length;
            SheetHeight = DefaultSheet.Height;

            //填入IQC、IPQC與SelfCheck的單位
            string[] CheckUnits = new string[] { "HRS", "PCS", "100%Check" };
            IQC_Units.Items.AddRange(CheckUnits.ToArray());
            IPQC_Units.Items.AddRange(CheckUnits.ToArray());
            SelfCheck_Units.Items.AddRange(CheckUnits.ToArray());

            //取得圖紙範圍資料Data
            CaxGetDatData.GetDraftingCoordinateData(out cCoordinateData);
        }

        private void ListSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            NXOpen.Drawings.DrawingSheet drawingSheet1 = (NXOpen.Drawings.DrawingSheet)workPart.DrawingSheets.FindObject(ListSheet.Text);
            drawingSheet1.Open();
        }

        private void SelectObject_Click(object sender, EventArgs e)
        {
            this.Hide();
            NXObject[] SelDimensionAry;
            CaxPublic.SelectObjects(out SelDimensionAry);
            DicSelDimension = new Dictionary<NXObject, string>();
            foreach (NXObject single in SelDimensionAry)
            {
                string DimenType = single.GetType().ToString();
                DicSelDimension.Add(single, DimenType);
            }
            this.Show();
            SelectObject.Text = string.Format("選擇物件({0})", SelDimensionAry.Length.ToString());
        }

        private void chb_Assign_CheckedChanged(object sender, EventArgs e)
        {
            if (chb_Assign.Checked == true)
            {
                chb_Remove.Checked = false;
                SelectObject.Enabled = true;
            }
        }

        private void chb_Remove_CheckedChanged(object sender, EventArgs e)
        {
            if (chb_Remove.Checked == true)
            {
                chb_Assign.Checked = false;
                SelectObject.Enabled = true;
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (FAIcheckBox.Checked == false & IQCcheckBox.Checked == false & IPQCcheckBox.Checked == false & FQCcheckBox.Checked == false)
            {
                CaxLog.ShowListingWindow("請先選擇一種檢驗報告格式！");
                return;
            }
            if (chb_Assign.Checked == true)
            {
                if (IQCGauge.Text == "" && IPQCGauge.Text == "" && SelfCheckGauge.Text == "")
                {
                    CaxLog.ShowListingWindow("資料不足，請先填寫【IQC】或【IPQC】或【SelfCheck】！");
                    return;
                }
                if (IQCGauge.Text != "" && (IQC_0.Text == "" || IQC_1.Text == "" || IQC_Units.Text == ""))
                {
                    CaxLog.ShowListingWindow("IQC尚未填寫完畢！");
                    return;
                }
                if (IPQCGauge.Text != "" && (IPQC_0.Text == "" || IPQC_1.Text == "" || IPQC_Units.Text == ""))
                {
                    CaxLog.ShowListingWindow("IPQC尚未填寫完畢！");
                    return;
                }
                if (SelfCheckGauge.Text != "" && (SelfCheck_0.Text == "" || SelfCheck_1.Text == "" || SelfCheck_Units.Text == ""))
                {
                    CaxLog.ShowListingWindow("SelfCheck尚未填寫完畢！");
                    return;
                }
            }
            
            if (DicSelDimension.Keys.Count == 0)
            {
                CaxLog.ShowListingWindow("請使用【選擇物件】選擇標註尺寸！");
                return;
            }
            #region 選擇Assign
            if (chb_Assign.Checked == true)
            {
                foreach (KeyValuePair<NXObject, string> kvp in DicSelDimension)
                {
                    //取得原始顏色
                    int oldColor = CaxME.GetDimensionColor(kvp.Key);
                    if (oldColor == -1)
                    {
                        oldColor = 125;
                    }
                    //取得檢具顏色
                    GaugeData cGaugeData = new GaugeData();
                    if (SelfCheckGauge.Text != "")
                    {
                        status = DicGaugeData.TryGetValue(SelfCheckGauge.Text, out cGaugeData);
                        if (!status)
                        {
                            CaxLog.ShowListingWindow("此檢具資料可能有誤");
                            return;
                        }
                    }
                    if (IQCGauge.Text != "")
                    {
                        status = DicGaugeData.TryGetValue(IQCGauge.Text, out cGaugeData);
                        if (!status)
                        {
                            CaxLog.ShowListingWindow("此檢具資料可能有誤");
                            return;
                        }
                    }
                    if (IPQCGauge.Text != "")
                    {
                        status = DicGaugeData.TryGetValue(IPQCGauge.Text, out cGaugeData);
                        if (!status)
                        {
                            CaxLog.ShowListingWindow("此檢具資料可能有誤");
                            return;
                        }
                    }
                    
                    //改變標註尺寸顏色
                    CaxME.SetDimensionColor(kvp.Key, Convert.ToInt32(cGaugeData.Color));
                    //塞屬性
                    kvp.Key.SetAttribute(CaxME.DimenAttr.OldColor, oldColor.ToString());//舊顏色
                    if (IQCGauge.Text != "")
                    {
                        kvp.Key.SetAttribute(CaxME.DimenAttr.IQC_Gauge, IQCGauge.Text);//檢具名稱
                        kvp.Key.SetAttribute(CaxME.DimenAttr.IQC_Freq, IQC_0.Text + "PC/" + IQC_1.Text + IQC_Units.Text);//IPQC
                    }
                    if (IPQCGauge.Text != "")
                    {
                        kvp.Key.SetAttribute(CaxME.DimenAttr.IPQC_Gauge, IPQCGauge.Text);//檢具名稱
                        kvp.Key.SetAttribute(CaxME.DimenAttr.IPQC_Freq, IPQC_0.Text + "PC/" + IPQC_1.Text + IPQC_Units.Text);//IPQC
                    }
                    if (SelfCheckGauge.Text != "")
                    {
                        kvp.Key.SetAttribute(CaxME.DimenAttr.SelfCheck_Gauge, SelfCheckGauge.Text);//檢具名稱
                        kvp.Key.SetAttribute(CaxME.DimenAttr.SelfCheck_Freq, SelfCheck_0.Text + "PC/" + SelfCheck_1.Text + SelfCheck_Units.Text);//SelfCheck
                    }
                    /*
                    //泡泡順序增加
                    BallonNum++;
                    //取得泡泡的座標
                    CaxME.BoxCoordinate TextCoordi = new CaxME.BoxCoordinate();
                    Point3d CreateBallonPt = new Point3d();
                    CaxME.GetTextBoxCoordinate(kvp.Key.Tag, out TextCoordi);
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
                    //插入泡泡
                    CaxME.CreateBallonOnSheet(BallonNum.ToString(), CreateBallonPt);
                    //取得泡泡在圖紙的區域
                    string SheetNum = ListSheet.Text, RegionX = "", RegionY = "";
                    for (int i = 0; i < cCoordinateData.DraftingCoordinate.Count;i++ )
                    {
                        string SheetSize = cCoordinateData.DraftingCoordinate[i].SheetSize;
                        if (Math.Ceiling(SheetHeight) != Convert.ToDouble(SheetSize.Split(',')[0]) || Math.Ceiling(SheetLength) != Convert.ToDouble(SheetSize.Split(',')[1]))
                        {
                            continue;
                        }
                        //比對X
                        for (int j = 0; j < cCoordinateData.DraftingCoordinate[i].RegionX.Count;j++ )
                        {
                            string X0 = cCoordinateData.DraftingCoordinate[i].RegionX[j].X0;
                            string X1 = cCoordinateData.DraftingCoordinate[i].RegionX[j].X1;
                            if (CreateBallonPt.X >= Convert.ToDouble(X0) && CreateBallonPt.X <= Convert.ToDouble(X1))
                            {
                                RegionX = cCoordinateData.DraftingCoordinate[i].RegionX[j].Zone;
                            }
                        }
                        //比對Y
                        for (int j = 0; j < cCoordinateData.DraftingCoordinate[i].RegionY.Count; j++)
                        {
                            string Y0 = cCoordinateData.DraftingCoordinate[i].RegionY[j].Y0;
                            string Y1 = cCoordinateData.DraftingCoordinate[i].RegionY[j].Y1;
                            if (CreateBallonPt.Y >= Convert.ToDouble(Y0) && CreateBallonPt.Y <= Convert.ToDouble(Y1))
                            {
                                RegionY = cCoordinateData.DraftingCoordinate[i].RegionY[j].Zone;
                            }
                        }
                    }
                    //塞泡泡屬性
                    kvp.Key.SetAttribute(CaxME.DimenAttr.BallonNum, BallonNum.ToString());
                    kvp.Key.SetAttribute(CaxME.DimenAttr.BallonLocation, SheetNum + "-" + RegionY + RegionX);
                    */
                }
            }
            #endregion

            #region 選擇Remove
            if (chb_Remove.Checked == true)
            {
                foreach (KeyValuePair<NXObject, string> kvp in DicSelDimension)
                {
                    //恢復原始顏色
                    string oldColor = "";
                    try
                    {
                        //第二次以上指定顏色的話，抓出來的顏色就不是內建顏色EX:125->108->186，抓到的是108
                        oldColor = kvp.Key.GetStringAttribute(CaxME.DimenAttr.OldColor);
                        //內建原始顏色
                        oldColor = "125";
                    }
                    catch (System.Exception ex)
                    {
                        oldColor = "125";
                    }
                    CaxME.SetDimensionColor(kvp.Key, Convert.ToInt32(oldColor));
                    
                    //取得泡泡資訊
                    string BallonNum = "";
                    try
                    {
                        BallonNum = kvp.Key.GetStringAttribute(CaxME.DimenAttr.BallonNum);
                    }
                    catch (System.Exception ex)
                    {
                        BallonNum = "";
                    }
                    if (BallonNum != "")
                    {
                        CaxME.DeleteBallon(BallonNum);
                    }

                    kvp.Key.DeleteAllAttributesByType(NXObject.AttributeType.String);
                }
            }
            #endregion

            SelectObject.Text = "選擇物件(0)";
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SameIPQC_CheckedChanged(object sender, EventArgs e)
        {
            if (SameIPQC.Checked == true)
            {
                if (IPQCGauge.Text.Contains("T"))
                {
                    CaxLog.ShowListingWindow("現場量測檢具不具有【" + IPQCGauge.Text + "】");
                    SelfCheck_0.Text = IPQC_0.Text;
                    SelfCheck_1.Text = IPQC_1.Text;
                    SelfCheck_Units.Text = IPQC_Units.Text;
                    return;
                }
                else
                {
                    SelfCheckGauge.Text = IPQCGauge.Text;
                    SelfCheck_0.Text = IPQC_0.Text;
                    SelfCheck_1.Text = IPQC_1.Text;
                    SelfCheck_Units.Text = IPQC_Units.Text;
                }
            }
            else
            {
                SelfCheckGauge.Text = "";
                SelfCheck_0.Text = "";
                SelfCheck_1.Text = "";
                SelfCheck_Units.Text = "";
            }
        }

        private void IPQCGauge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SameIPQC.Checked == true)
            {
                if (IPQCGauge.Text.Contains("T"))
                {
                    CaxLog.ShowListingWindow("現場量測檢具不具有【" + IPQCGauge.Text + "】");
                    return;
                }
                SelfCheckGauge.Text = IPQCGauge.Text;
            }
        }

        private void IPQC_0_TextChanged(object sender, EventArgs e)
        {
            if (SameIPQC.Checked == true)
            {
                SelfCheck_0.Text = IPQC_0.Text;
            }
        }

        private void IPQC_1_TextChanged(object sender, EventArgs e)
        {
            if (SameIPQC.Checked == true)
            {
                SelfCheck_1.Text = IPQC_1.Text;
            }
        }

        private void IPQC_Units_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SameIPQC.Checked == true)
            {
                SelfCheck_Units.Text = IPQC_Units.Text;
            }
        }

        private void FAIcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (FAIcheckBox.Checked == true)
            {
                IQCcheckBox.Checked = false;
                IPQCcheckBox.Checked = false;
                FQCcheckBox.Checked = false;
            }
        }

        private void IQCcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (IQCcheckBox.Checked == true)
            {
                FAIcheckBox.Checked = false;
                IPQCcheckBox.Checked = false;
                FQCcheckBox.Checked = false;
            }
        }

        private void IPQCcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (IPQCcheckBox.Checked == true)
            {
                FAIcheckBox.Checked = false;
                IQCcheckBox.Checked = false;
                FQCcheckBox.Checked = false;
            }
        }

        private void FQCcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (FQCcheckBox.Checked == true)
            {
                FAIcheckBox.Checked = false;
                IQCcheckBox.Checked = false;
                IPQCcheckBox.Checked = false;
            }
        }


    }
}
