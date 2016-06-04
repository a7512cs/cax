using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.SuperGrid;
using System.Collections;
using System.IO;
using CaxGlobaltek;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;



namespace PEGenerateFile
{
    public partial class PEGenerateDlg : DevComponents.DotNetBar.Office2007Form
    {
        private static UFSession theUfSession = UFSession.GetUFSession();
        private static Session theSession = Session.GetSession();

        public bool status;
        public static string Oper1String = "";
        public static string Oper2String = "";
        public static OperationArray cOperationArray = new OperationArray();
        public static string[] Oper2StringAry = new string[]{};
        public static GridPanel panel = new GridPanel();
        public static string CusName = "";
        public static string PartNo = "";
        public static string CusRev = "";
        public static string PartPath = "-1";
        public static Dictionary<string, PE_OutPutDat> DicDataSave = new Dictionary<string, PE_OutPutDat>();



        public PEGenerateDlg()
        {
            InitializeComponent();

            //取得CustomerName配置檔
            string CustomerName_dat = "CustomerName.dat";
            string CustomerNameDatPath = string.Format(@"{0}\{1}", CaxPE.GetPEConfigDir(), CustomerName_dat);

            //讀取OperationArray配置檔內容，並存入結構中
            CusName cCusName = new CusName();
            CaxPE.ReadCustomerNameData(CustomerNameDatPath, out cCusName);

            //將客戶名稱填入下拉選單-客戶
            comboBoxCusName.Items.AddRange(cCusName.CustomerName.ToArray());

            //取得OperationArray配置檔
            string OperationArray_dat = "OperationArray.dat";
            string OperationArrayDatPath = string.Format(@"{0}\{1}", CaxPE.GetPEConfigDir(), OperationArray_dat);
            
            //讀取OperationArray配置檔內容，並存入結構中
            CaxPE.ReadOperationArrayData(OperationArrayDatPath, out cOperationArray);
            
            //建立GridPanel
            panel = OperSuperGridControl.PrimaryGrid;
            
            //將Operation2Array塞入陣列Oper2StringAry中
            Oper2StringAry = cOperationArray.Operation2Array.ToArray();

            //設定製程別的基礎型態與數據
            panel.Columns["Oper2Ary"].EditorType = typeof(PEComboBox);
            panel.Columns["Oper2Ary"].EditorParams = new object[] { Oper2StringAry };

            //設定刪除的基礎型態
            panel.Columns["Delete"].EditorType = typeof(OperDeleteBtn);
        }

        private void PEGenerateDlg_Load(object sender, EventArgs e)
        {
            //初始設定
            textPartNo.Text = "";
            textCusRev.Text = "";

            

            
            //將OperationArray配置檔內容塞入製程序&製程別下拉選單中
            //comboOperArray1.Items.AddRange(cOperationArray.OperationArray1.ToArray());
            //comboOperArray2.Items.AddRange(cOperationArray.OperationArray2.ToArray());
        }

        private void OperCreateBtn_Click(object sender, EventArgs e)
        {
            //取得使用者選取的製程序
            List<string> ListSelectOper = new List<string>();
            status = CheckOper1Status(out ListSelectOper);
            if (!status)
            {
                MessageBox.Show("檢查製程序失敗");
                return;
            }

            //判斷使用者選取的製程序是否已經存在於OperSuperGridControl
            //CaxLog.ShowListingWindow(panel.Rows.Count.ToString());
            if (!(panel.Rows.Count == 0))
            {
                for (int i = 0; i < panel.Rows.Count;i++ )
                {
                    for (int j = 0; j < ListSelectOper.Count;j++ )
                    {
                        if (panel.GridPanel.GetCell(i, 0).Value.ToString() == ListSelectOper[j])
                        {
                            MessageBox.Show("已有重複的製程序");
                            //清除使用者選取的製程序
                            ClearSelectOper1();
                            return;
                        }
                    }
                }
            }

            //將製程序填入OperSuperGridControl
            GridRow row = new GridRow();
            foreach (var i in ListSelectOper)
            {
                row = new GridRow(i, "","刪除");
                OperSuperGridControl.PrimaryGrid.Rows.Add(row);
            }
            
            //清除使用者選取的製程序
            ClearSelectOper1();



        }

        private void OperDeleteBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void OperListView_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("2");
        }

        private bool CheckOper1Status(out List<string> ListSelectOper)
        {
            ListSelectOper = new List<string>();
            bool status = false;
            try
            {
                status = check001.Checked;
                if (status)
                {
                    ListSelectOper.Add(check001.Text);
                }
                status = check210.Checked;
                if (status)
                {
                    ListSelectOper.Add(check210.Text);
                }
                status = check220.Checked;
                if (status)
                {
                    ListSelectOper.Add(check220.Text);
                }
                status = check230.Checked;
                if (status)
                {
                    ListSelectOper.Add(check230.Text);
                }
                status = check240.Checked;
                if (status)
                {
                    ListSelectOper.Add(check240.Text);
                }
                status = check250.Checked;
                if (status)
                {
                    ListSelectOper.Add(check250.Text);
                }
                status = check260.Checked;
                if (status)
                {
                    ListSelectOper.Add(check260.Text);
                }
                status = check270.Checked;
                if (status)
                {
                    ListSelectOper.Add(check270.Text);
                }
                status = check280.Checked;
                if (status)
                {
                    ListSelectOper.Add(check280.Text);
                }
                status = check290.Checked;
                if (status)
                {
                    ListSelectOper.Add(check290.Text);
                }
                status = check300.Checked;
                if (status)
                {
                    ListSelectOper.Add(check300.Text);
                }
                status = check310.Checked;
                if (status)
                {
                    ListSelectOper.Add(check310.Text);
                }
                status = check320.Checked;
                if (status)
                {
                    ListSelectOper.Add(check320.Text);
                }
                status = check330.Checked;
                if (status)
                {
                    ListSelectOper.Add(check330.Text);
                }
                status = check340.Checked;
                if (status)
                {
                    ListSelectOper.Add(check340.Text);
                }
                status = check350.Checked;
                if (status)
                {
                    ListSelectOper.Add(check350.Text);
                }
                status = check360.Checked;
                if (status)
                {
                    ListSelectOper.Add(check360.Text);
                }
                status = check370.Checked;
                if (status)
                {
                    ListSelectOper.Add(check370.Text);
                }
                status = check380.Checked;
                if (status)
                {
                    ListSelectOper.Add(check380.Text);
                }
                status = check999.Checked;
                if (status)
                {
                    ListSelectOper.Add(check999.Text);
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool ClearSelectOper1()
        {
            try
            {
                check001.Checked = false;
                check210.Checked = false;
                check220.Checked = false;
                check230.Checked = false;
                check240.Checked = false;
                check250.Checked = false;
                check260.Checked = false;
                check270.Checked = false;
                check280.Checked = false;
                check290.Checked = false;
                check300.Checked = false;
                check310.Checked = false;
                check320.Checked = false;
                check330.Checked = false;
                check340.Checked = false;
                check350.Checked = false;
                check360.Checked = false;
                check370.Checked = false;
                check380.Checked = false;
                check999.Checked = false;
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private void SelectPartFileBtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = @"D:\Globaltek";

            openFileDialog1.Filter = "Part Files (*.prt)|*.prt|All Files (*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                //取得檔案名稱(檔名+副檔名)
                labelPartFileName.Text = openFileDialog1.SafeFileName;
                //取得檔案完整路徑(路徑+檔名+副檔名)
                PartPath = openFileDialog1.FileName;
                
                //MessageBox.Show(textPartFileName.Text);
            }
            
        }

        private void OK_Click(object sender, EventArgs e)
        {

            #region 取得客戶名稱

            CusName = comboBoxCusName.Text;
            if (CusName == "")
            {
                MessageBox.Show("尚未填寫客戶！");
                return;
            }

            #endregion

            #region 取得料號

            PartNo = textPartNo.Text;
            if (PartNo == "")
            {
                MessageBox.Show("尚未填寫料號！");
                return;
            }

            #endregion

            #region 取得客戶版次

            CusRev = textCusRev.Text;
            if (CusRev == "")
            {
                MessageBox.Show("尚未填寫客戶版次！");
                return;
            }

            #endregion

            #region 取得檔案路徑

            if (PartPath == "-1")
            {
                MessageBox.Show("尚未選擇客戶檔案！");
                return;
            }

            #endregion

            #region 定義根目錄

            //定義MODEL資料夾路徑
            string ModelFolderFullPath = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", CaxEnv.GetGlobaltekEnvDir(), "Task", CusName, PartNo, CusRev.ToUpper(), "MODEL");
            
            //定義總組立檔案名稱
            string AsmCompFileFullPath = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", CaxEnv.GetGlobaltekEnvDir(), "Task", CusName, PartNo, CusRev.ToUpper(), PartNo + "_MOT.prt");

            //定義CAM資料夾路徑、OIS資料夾路徑、三階檔案路徑
            string CAMFolderPath = "", OISFolderPath = "", ThridOperPartPath = "";

            #endregion

            #region 建立MODEL資料夾

            if (!File.Exists(ModelFolderFullPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(ModelFolderFullPath);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }

            #endregion

            #region 複製客戶檔案到MODEL資料夾內

            //判斷客戶的檔案是否存在
            status = System.IO.File.Exists(PartPath);
            if (!status)
            {
                MessageBox.Show("指定的檔案不存在，請再次確認");
                return;
            }

            //建立MODEL資料夾內客戶檔案路徑
            string CustomerPartFullPath = string.Format(@"{0}\{1}", ModelFolderFullPath, PartNo + ".prt");

            //開始複製
            File.Copy(PartPath, CustomerPartFullPath, true);

            #endregion
            
            #region 將值儲存起來(未存完)

            PE_OutPutDat cPE_OutPutDat = new PE_OutPutDat();
            cPE_OutPutDat.CusName = CusName;
            cPE_OutPutDat.PartNo = PartNo;
            cPE_OutPutDat.CusRev = CusRev.ToUpper();
            cPE_OutPutDat.PartPath = PartPath;
            cPE_OutPutDat.ListOperation = new List<Operation>();
            Operation cOperation = new Operation();
            cPE_OutPutDat.Oper1Ary = new List<string>();
            cPE_OutPutDat.Oper2Ary = new List<string>(); 
            for (int i = 0; i < panel.Rows.Count; i++)
            {
                if (panel.Rows.Count == 0)
                {
                    MessageBox.Show("尚未選擇製程序與製程別！");
                    return;
                }

                if (panel.GetCell(i, 1).Value.ToString() == "")
                {
                    MessageBox.Show("製程序" + panel.GetCell(i, 0).Value + "尚未選取製程別！");
                    return;
                }

                cOperation = new Operation();
                cOperation.Oper1 = panel.GetCell(i, 0).Value.ToString();
                cOperation.Oper2 = panel.GetCell(i, 1).Value.ToString();

                //建立CAM資料夾路徑
                CAMFolderPath = string.Format(@"{0}\{1}\{2}", Path.GetDirectoryName(AsmCompFileFullPath), "OP" + panel.GetCell(i, 0).Value.ToString(), "CAM");

                //儲存CAM資料夾路徑
                cOperation.CAMFolderPath = CAMFolderPath;

                //建立CAM資料夾
                if (!File.Exists(CAMFolderPath))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(CAMFolderPath);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return;
                    }
                }

                //建立OIS資料夾路徑
                OISFolderPath = string.Format(@"{0}\{1}\{2}", Path.GetDirectoryName(AsmCompFileFullPath), "OP" + panel.GetCell(i, 0).Value.ToString(), "OIS");

                //儲存OIS資料夾路徑
                cOperation.OISFolderPath = OISFolderPath;

                //建立OIS資料夾
                if (!File.Exists(OISFolderPath))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(OISFolderPath);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return;
                    }
                }

                //建立三階檔案路徑
                ThridOperPartPath = Path.GetDirectoryName(AsmCompFileFullPath);

                cPE_OutPutDat.ListOperation.Add(cOperation);

                cPE_OutPutDat.Oper1Ary.Add(panel.GetCell(i, 0).Value.ToString());
                cPE_OutPutDat.Oper2Ary.Add(panel.GetCell(i, 1).Value.ToString());
            }

            #endregion
            
            #region (註解中)複製MODEL內的客戶檔案到料號資料夾內，並更名XXX_MOT.prt
            /*
            //判斷要複製的檔案是否存在
            status = System.IO.File.Exists(destFileName_Model);
            if (!status)
            {
                MessageBox.Show("指定的檔案不存在，請再次確認");
                return;
            }

            //建立目的地(客戶版次)檔案全路徑
            string destFileName_CusRev = string.Format(@"{0}\{1}\{2}\{3}", CaxEnv.GetGlobalTekEnvDir(), PartNo, CusRev.ToUpper(), PartNo + "_MOT.prt");

            //開始複製
            File.Copy(destFileName_Model, destFileName_CusRev, true);
            */
            #endregion
            
            #region 自動建立總組立檔案架構，並組立相關製程

            status = CaxAsm.CreateNewAsm(AsmCompFileFullPath);
            if (!status)
            {
                CaxLog.ShowListingWindow("建立一階總組立檔失敗");
                return;
            }

            CaxPart.Save();


            string OPCompName = "";
            NXOpen.Assemblies.Component tempComp;
            //List<double> ListOperDouble = new List<double>();
            //for (int i = 0; i < cPE_OutPutDat.ListOperation.Count; i++)
            //{
            //    ListOperDouble.Add(Convert.ToDouble(cPE_OutPutDat.ListOperation[i].Oper1));
            //}
            //ListOperDouble.Sort();

            for (int i = 0; i < cPE_OutPutDat.ListOperation.Count; i++)
            {
                //設定一階為WorkComp
                CaxAsm.SetWorkComponent(null); 
                
                //建立二階製程檔
                OPCompName = string.Format(@"{0}\{1}", Path.GetDirectoryName(AsmCompFileFullPath), PartNo + "_OP" + cPE_OutPutDat.ListOperation[i].Oper1 + ".prt");
                status = CaxAsm.CreateNewEmptyComp(OPCompName, out tempComp);
                if (!status)
                {
                    CaxLog.ShowListingWindow("建立二階製程檔失敗");
                    return;
                }
            }

            string OISCompFullPath = "", CAMCompFullPath = "";

            //取得二階所有comp
            List<NXOpen.Assemblies.Component> ChildenComp = new List<NXOpen.Assemblies.Component>();
            CaxAsm.GetCompChilden(out ChildenComp);

            for (int i = 0; i < ChildenComp.Count; i++)
            {
                CaxAsm.SetWorkComponent(ChildenComp[i]);
                string OperStr = ChildenComp[i].Name.Split(new string[] { "OP" }, StringSplitOptions.RemoveEmptyEntries)[1];

                //建立三階CAM檔
                CAMCompFullPath = string.Format(@"{0}\{1}", Path.GetDirectoryName(AsmCompFileFullPath), PartNo + "_OP" + OperStr + "_CAM.prt");
                status = CaxAsm.CreateNewEmptyComp(CAMCompFullPath, out tempComp);
                if (!status)
                {
                    CaxLog.ShowListingWindow("建立三階CAM檔失敗");
                    return;
                }

                //建立三階OIS檔
                OISCompFullPath = string.Format(@"{0}\{1}", Path.GetDirectoryName(AsmCompFileFullPath), PartNo + "_OIS" + OperStr + ".prt");
                status = CaxAsm.CreateNewEmptyComp(OISCompFullPath, out tempComp);
                if (!status)
                {
                    CaxLog.ShowListingWindow("建立三階OIS檔失敗");
                    return;
                }
            }

            #endregion

            #region 寫出PEdat

            string PECreateDataJsonDat = string.Format(@"{0}\{1}", ModelFolderFullPath, "PECreateData.dat");
            status = CaxFile.WriteJsonFileData(PECreateDataJsonDat, cPE_OutPutDat);
            if (!status)
            {
                MessageBox.Show("PECreateData.dat 輸出失敗...");
                return;
            }

            #endregion

            #region 寫出METEDownloadDat

            string METEDownloadData = string.Format(@"{0}\{1}", CaxEnv.GetGlobaltekTaskDir(), "METEDownloadData.dat");
            METEDownloadData cMETEDownloadData = new METEDownloadData();

            if (File.Exists(METEDownloadData))
            {
                #region METEDownloadDat檔案存在

                status = CaxPublic.ReadMETEDownloadData(METEDownloadData, out cMETEDownloadData);
                if (!status)
                {
                    MessageBox.Show("METEDownloadData.dat 讀取失敗...");
                    return;
                }

                int CusCount = 0, IndexOfCusName = -1;
                for (int i = 0; i < cMETEDownloadData.EntirePartAry.Count; i++)
                {
                    if (CusName != cMETEDownloadData.EntirePartAry[i].CusName)
                    {
                        CusCount++;
                    }
                    else
                    {
                        IndexOfCusName = i;
                        break;
                    }
                }

                //新的客戶且已經有METEDownloadDat.dat
                if (CusCount == cMETEDownloadData.EntirePartAry.Count)
                {
                    EntirePartAry cEntirePartAry = new EntirePartAry();
                    cEntirePartAry.CusName = CusName;
                    cEntirePartAry.CusPart = new List<CusPart>();

                    CusPart cCusPart = new CusPart();
                    cCusPart.PartNo = PartNo;
                    cCusPart.CusRev = new List<CusRev>();

                    CusRev cCusRev = new CusRev();
                    cCusRev.RevNo = CusRev.ToUpper();
                    cCusRev.OperAry1 = new List<string>();
                    cCusRev.OperAry1 = cPE_OutPutDat.Oper1Ary;

                    cCusPart.CusRev.Add(cCusRev);
                    cEntirePartAry.CusPart.Add(cCusPart);
                    cMETEDownloadData.EntirePartAry.Add(cEntirePartAry);
                }
                //舊的客戶新增料號
                else
                {
                    //判斷料號是否已存在
                    int PartCount = 0; int IndexOfPartNo = -1;
                    for (int i = 0; i < cMETEDownloadData.EntirePartAry[IndexOfCusName].CusPart.Count; i++)
                    {
                        if (PartNo != cMETEDownloadData.EntirePartAry[IndexOfCusName].CusPart[i].PartNo)
                        {
                            PartCount++;
                        }
                        else
                        {
                            IndexOfPartNo = i;
                            break;
                        }
                    }

                    //舊的客戶且新的料號 PartCount == CusPart.Count 表示新的料號
                    if (PartCount == cMETEDownloadData.EntirePartAry[IndexOfCusName].CusPart.Count)
                    {
                        CusPart cCusPart = new CusPart();
                        cCusPart.PartNo = PartNo;
                        cCusPart.CusRev = new List<CusRev>();

                        CusRev cCusRev = new CusRev();
                        cCusRev.RevNo = CusRev.ToUpper();
                        cCusRev.OperAry1 = new List<string>();
                        cCusRev.OperAry1 = cPE_OutPutDat.Oper1Ary;

                        cCusPart.CusRev.Add(cCusRev);
                        cMETEDownloadData.EntirePartAry[IndexOfCusName].CusPart.Add(cCusPart);
                    }
                    //舊的客戶且舊的料號新增客戶版次
                    else
                    {
                        CusRev cCusRev = new CusRev();
                        cCusRev.RevNo = CusRev.ToUpper();
                        cCusRev.OperAry1 = new List<string>();
                        cCusRev.OperAry1 = cPE_OutPutDat.Oper1Ary;

                        cMETEDownloadData.EntirePartAry[IndexOfCusName].CusPart[IndexOfPartNo].CusRev.Add(cCusRev);
                    }
                }
                /*
                int PartCount = 0; int IndexOfPartNo = -1;
                for (int i = 0; i < cMETEDownloadData.EntirePartAry.Count; i++)
                {
                    if (PartNo != cMETEDownloadData.EntirePartAry[i].PartNo)
                    {
                        PartCount++;
                    }
                    else
                    {
                        IndexOfPartNo = i;
                        break;
                    }
                }

                //新的料號且已經有METEDownloadDat.dat
                if (PartCount == cMETEDownloadData.EntirePartAry.Count)
                {
                    EntirePartAry cEntirePartAry = new EntirePartAry();
                    cEntirePartAry.CusRev = new List<CusRev>();

                    CusRev cCusRev = new CusRev();
                    cCusRev.OperAry1 = new List<string>();
                    cCusRev.RevNo = CusRev.ToUpper();
                    cCusRev.OperAry1 = cPE_OutPutDat.Oper1Ary;

                    cEntirePartAry.CusName = CusName;
                    cEntirePartAry.PartNo = PartNo;
                    cEntirePartAry.CusRev.Add(cCusRev);

                    cMETEDownloadData.EntirePartAry.Add(cEntirePartAry);
                }
                //舊的料號新增客戶版次
                else
                {
                    CusRev cCusRev = new CusRev();
                    cCusRev.OperAry1 = new List<string>();
                    cCusRev.RevNo = CusRev.ToUpper();
                    cCusRev.OperAry1 = cPE_OutPutDat.Oper1Ary;

                    cMETEDownloadData.EntirePartAry[IndexOfPartNo].CusRev.Add(cCusRev);
                }
                */
                #endregion
            }
            else
            {
                #region METEDownloadDat檔案不存在

                cMETEDownloadData.EntirePartAry = new List<EntirePartAry>();
                EntirePartAry cEntirePartAry = new EntirePartAry();
                cEntirePartAry.CusName = CusName;
                cEntirePartAry.CusPart = new List<CusPart>();

                CusPart cCusPart = new CusPart();
                cCusPart.PartNo = PartNo;
                cCusPart.CusRev = new List<CusRev>();

                CusRev cCusRev = new CusRev();
                cCusRev.RevNo = CusRev.ToUpper();
                cCusRev.OperAry1 = new List<string>();
                cCusRev.OperAry1 = cPE_OutPutDat.Oper1Ary;

                cCusPart.CusRev.Add(cCusRev);
                cEntirePartAry.CusPart.Add(cCusPart);
                cMETEDownloadData.EntirePartAry.Add(cEntirePartAry);

                #endregion
            }

            status = CaxFile.WriteJsonFileData(METEDownloadData, cMETEDownloadData);
            if (!status)
            {
                MessageBox.Show("METEDownloadDat.dat 輸出失敗...");
                return;
            }

            #endregion
            
            CaxAsm.SetWorkComponent(null);
            this.Close();
            
            CaxPart.Save();

        }
    }
     
    public class PEComboBox : GridComboBoxExEditControl
    {
        public PEComboBox(IEnumerable Oper2StringAry)
        {
            DataSource = Oper2StringAry;
            DropDownStyle = ComboBoxStyle.DropDownList;
            //this.MouseWheel += new MouseEventHandler(Oper2Changed);
            //this.DropDownClosed += new EventHandler(Oper2Changed);
        }

        public void Oper2Changed(object sender, EventArgs e)
        {
            
//             PEComboBox aa = (PEComboBox)sender;
//             
//             int index = aa.EditorCell.RowIndex;
//             string b = aa.EditorCell.GridRow.Cells["Oper2Ary"].Value.ToString();
// 
//             MessageBox.Show(index.ToString());
            //string a = (string)sender.ToString();
            //MessageBox.Show(a);
        }
    }

    public class OperDeleteBtn : GridButtonXEditControl
    {
        public OperDeleteBtn()
        {
            try
            {
                Click += DeleteBtnClick;
            }
            catch (System.Exception ex)
            {

            }
        }
        public void DeleteBtnClick(object sender, EventArgs e)
        {
            GridPanel panel = new GridPanel();
            panel = PEGenerateDlg.panel;
            
            OperDeleteBtn cOperDelectBtn = (OperDeleteBtn)sender;
            int index = cOperDelectBtn.EditorCell.RowIndex;

            panel.Rows.RemoveAt(index);
        }
    }
}
