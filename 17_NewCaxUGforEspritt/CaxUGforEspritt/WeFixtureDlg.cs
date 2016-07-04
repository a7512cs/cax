using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.SuperGrid;
using CimforceCaxTwPublic;
using WeData;
using NXOpen;

namespace CaxUGforEspritt
{
    public partial class WeFixtureDlg : DevComponents.DotNetBar.Office2007Form
    {
        public static string FIXTURE_COM_NAME = "";
        public static string FIXTURE_TYPE = "";
        public static string FIXTURE_MODEL_NAME = "";
        public static string FIXTURE_IMG_PATH = "";
        public static string IS_FIX = "N";
        public static int Click_Form1_Index;
        public static WeListKey sWeListKey;

        public static List<WeFixData> allowFixtureDataLst;
        public static Dictionary<WeListKey, WeFaceGroup> WE_FACE_DIC = new Dictionary<WeListKey, WeFaceGroup>();

        public WeFixtureDlg(List<WeFixData> allowFixtureDataLstSY, 
                                      string FixtureDirInput, Dictionary<WeListKey, 
                                      WeFaceGroup> WE_FACE_DIC_INPUT, 
                                      int Click_Form1_Index_Input, 
                                      WeListKey sWeListKeyInput)
        {
            allowFixtureDataLst = allowFixtureDataLstSY;
            FixtureDir = FixtureDirInput;
            Click_Form1_Index = Click_Form1_Index_Input;
            sWeListKey = sWeListKeyInput;
            WE_FACE_DIC = WE_FACE_DIC_INPUT;
            InitializeComponent();
        }

        private void buttonX1_OK_Click(object sender, EventArgs e)
        {
            //WeFixtureDlg cWeFixtureDlg = (WeFixtureDlg)sender;

            if (FIXTURE_COM_NAME == "")
            {
                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "尚未選取治具");
                return;
            }
            
            WeFaceGroup sWeFaceGroup;
            WE_FACE_DIC.TryGetValue(sWeListKey, out sWeFaceGroup);
            sWeFaceGroup.WE_FIX = FIXTURE_COM_NAME + "_" + FIXTURE_TYPE + "_" + FIXTURE_MODEL_NAME;
            sWeFaceGroup.IS_FIX = IS_FIX;
            sWeFaceGroup.WE_FIX_PATH = FIXTURE_IMG_PATH;
            WE_FACE_DIC[sWeListKey] = sWeFaceGroup;


            SelectWorkPart.WE_FACE_DIC = WE_FACE_DIC;


            //變更為已選治具
            ((DevComponents.DotNetBar.SuperGrid.GridRow)(CaxWE.cSelectWorkPartFrom.superGridMainControl.ActiveRow)).Cells[6].Value = "已選治具";

            this.Owner.Enabled = true;
            this.Owner.Show();
            this.Close();
        }

        private void WeFixtureDlg_Load(object sender, EventArgs e)
        {
            string defaultImgPath = string.Format(@"{0}\{1}", FixtureDir, "cimforce_logo.jpg");
            pictureBox1.ImageLocation = defaultImgPath;

            //設定對話框風格
            StyleController styleControl = new StyleController();
            styleControl.SetStyleManager(styleManager1);
            styleControl.SetAllStyle(this);
            
            //寫入對話框
            for (int i = 0; i < allowFixtureDataLst.Count; i++)
            {
                GridRow row = new GridRow(
                    allowFixtureDataLst[i].comName,
                    allowFixtureDataLst[i].modelID,
                    allowFixtureDataLst[i].modelName,
                    allowFixtureDataLst[i].length,
                    allowFixtureDataLst[i].width,
                    allowFixtureDataLst[i].height);
                superGridControl1.PrimaryGrid.Rows.Add(row);
            }

            WeFaceGroup sWeFaceGroup;
            WE_FACE_DIC.TryGetValue(sWeListKey, out sWeFaceGroup);
            Body tempBody;
            CaxPart.GetLayerBody(sWeFaceGroup.comp, out tempBody);
            double[] WPmin1 = new double[3];
            double[] WPmax1 = new double[3];
            CaxPart.AskBoundingBoxExactByWCS(tempBody.Tag, out WPmin1, out WPmax1);
            WP_Length.Text = Math.Round((WPmax1[0] - WPmin1[0]), 3).ToString();
            WP_Width.Text = Math.Round((WPmax1[1] - WPmin1[1]), 3).ToString();
            WP_Height.Text = Math.Round((WPmax1[2] - WPmin1[2]), 3).ToString();
            WP_Type.Text = TransType(CaxWE.Task_Type);
            
        }

        private string TransType(string tasktype)
        {
            string WP_Type = "";

            if (tasktype=="0")
            {
                WP_Type = "模仁";
            }
            if (tasktype == "1")
            {
                WP_Type = "模板";
            }
            if (tasktype == "2")
            {
                WP_Type = "滑塊";
            }
            if (tasktype == "3")
            {
                WP_Type = "斜銷";
            }
            if (tasktype == "4")
            {
                WP_Type = "入子";
            }
            if (tasktype == "5")
            {
                WP_Type = "電極";
            }
            if (tasktype == "6")
            {
                WP_Type = "小零件";
            }
            if (tasktype == "7")
            {
                WP_Type = "其他";
            }
            if (tasktype == "8")
            {
                WP_Type = "U類";
            }
            return WP_Type;
        }

        private void WeFixtureDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Owner.Enabled = true;
            this.Owner.Show();
            this.Close();
        }

        private void superGridControl1_RowClick(object sender, GridRowClickEventArgs e)
        {
            for (int i = 0; i < allowFixtureDataLst.Count; i++)
            {
                // 找到所選的治具
                if (allowFixtureDataLst[i].modelID == e.GridPanel.GetCell(e.GridRow.Index, 1).Value.ToString())
                {
//                     CaxLog.ShowListingWindow("comName：" + allowFixtureDataLst[i].comName.ToString());
//                     CaxLog.ShowListingWindow("modelID：" + allowFixtureDataLst[i].modelName.ToString());
//                     CaxLog.ShowListingWindow("TYPE：" + allowFixtureDataLst[i].FIXTURE_TYPE.ToString());

                    FIXTURE_COM_NAME = allowFixtureDataLst[i].comName;
                    FIXTURE_TYPE = allowFixtureDataLst[i].FIXTURE_TYPE;
                    FIXTURE_MODEL_NAME = allowFixtureDataLst[i].modelName;
                    FIXTURE_IMG_PATH = allowFixtureDataLst[i].imagePath;
                    IS_FIX = "Y";
                    
                    // 變更顯示圖片
                    string imgPath = allowFixtureDataLst[i].imagePath;


                    if (System.IO.File.Exists(imgPath))
                    {
                        pictureBox1.ImageLocation = imgPath;

                        //pictureBoxFixture.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    else
                    {
                        string defaultImgPath = string.Format(@"{0}\{1}", FixtureDir, "cimforce_logo.jpg");
                        pictureBox1.ImageLocation = defaultImgPath;
                        // Stretch the picture to fit the control.
                        //pictureBoxFixture.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    // 變更顯示校正方式
                    //string measureType;
                    //bool status = DetermineMeasureType(allowFixtureDataLstSY[i], out measureType);
                    //labelXMeasureType.Text = measureType;

                    break;
                }
            }
        }

        private void buttonX2_Cancel_Click(object sender, EventArgs e)
        {
            WeFaceGroup sWeFaceGroup;
            WE_FACE_DIC.TryGetValue(sWeListKey, out sWeFaceGroup);
            sWeFaceGroup.WE_FIX = "";
            sWeFaceGroup.IS_FIX = "N";
            sWeFaceGroup.WE_FIX_PATH = "";
            WE_FACE_DIC[sWeListKey] = sWeFaceGroup;

            this.Owner.Enabled = true;
            this.Owner.Show();
            this.Close();
        }


    }
}
