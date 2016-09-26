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
using NHibernate;
using NHibernate.Criterion;


namespace PEGenerateFile
{
    public partial class PEGenerateDlg : DevComponents.DotNetBar.Office2007Form
    {
        private static UFSession theUfSession = UFSession.GetUFSession();
        private static Session theSession = Session.GetSession();

        public bool status,Is_OldPart = false;
        public static string Oper1String = "",Oper2String = "",CusName = "",PartNo = "",CusRev = "",PartPath = "-1",CurrentOldCusName = "",CurrentOldPartNo = "",CurrentOldCusRev = "";
        public static OperationArray cOperationArray = new OperationArray();
        //public static string[] Oper2StringAry = new string[]{};
        public static GridPanel panel = new GridPanel();
        public static Dictionary<string, PECreateData> DicDataSave = new Dictionary<string, PECreateData>();
        public static METEDownloadData cMETEDownloadData = new METEDownloadData();
        public static int IndexofCusName = -1, IndexofPartNo = -1, IndexofCusRev = -1;
        public static List<string> ListAddOper = new List<string>();
        public static PECreateData cPECreateData = new PECreateData();
        public static Com_PEMain cCom_PEMain = new Com_PEMain();
        public static IList<Sys_Operation2> operation2Name = new List<Sys_Operation2>();
        public static Com_PartOperation cCom_PartOperation = new Com_PartOperation();


        public PEGenerateDlg()
        {
            InitializeComponent();

            #region 舊客戶資料填入
            string[] S_Task_CusName = Directory.GetDirectories(CaxEnv.GetGlobaltekTaskDir());
            if (S_Task_CusName.Length == 0)
            {
                comboBoxOldCusName.Items.Add("沒有舊資料");
            }
            else
            {
                foreach (string item in S_Task_CusName)
                {
                    comboBoxOldCusName.Items.Add(Path.GetFileNameWithoutExtension(item));//走訪每個元素只取得目錄名稱(不含路徑)並加入dirlist集合中
                }
            }
            comboBoxOldPartNo.Enabled = false;
            comboBoxOldCusRev.Enabled = false;
            #endregion

            using (ISession session = MyHibernateHelper.SessionFactory.OpenSession())
            {
                //IList<Sys_Customer> customerName = new List<Sys_Customer>();
                //customerName = session.QueryOver<Sys_Customer>().List();
                //comboBoxCusName.Items.AddRange(((List<string>)customerName).ToArray());

                //IList<string> customerName = session.QueryOver<Sys_Customer>().Select(x => x.customerName).List<string>();
                //comboBoxCusName.Items.AddRange(((List<string>)customerName).ToArray());

                IList<Sys_Customer> customerName = session.QueryOver<Sys_Customer>().List<Sys_Customer>();
                comboBoxCusName.DisplayMember = "customerName";
                comboBoxCusName.ValueMember = "customerSrNo";
                foreach (Sys_Customer i in customerName)
                {
                    comboBoxCusName.Items.Add(i);
                    //comboBoxCusName.Items.Add(new { customerName = i.customerName, customerSrNo = i.customerSrNo.ToString() });
                }
                //comboBoxCusName.DataSource = new BindingSource(customerName, null);

                //可由CAX查到流水號
                //var aa = session.QueryOver<Sys_Customer>().Where(x => x.customerName == "CAX").Select(x => x.customerSrNo).SingleOrDefault<Int32>();
                //CaxLog.ShowListingWindow(aa.ToString());

                //方法一
                operation2Name = session.QueryOver<Sys_Operation2>().List<Sys_Operation2>();
                //方法二
                //IList<string> operation2 = session.QueryOver<Sys_Operation2>().Select(x => x.operation2Name).List<string>();
                //Oper2StringAry = ((List<string>)operation2).ToArray();
                

                session.Close();
            }
            
            
            //取得CustomerName配置檔
            //string CustomerName_dat = "CustomerName.dat";
            //string CustomerNameDatPath = string.Format(@"{0}\{1}", CaxPE.GetPEConfigDir(), CustomerName_dat);
            //CusName cCusName = new CusName();
            //CaxPE.ReadCustomerNameData(CustomerNameDatPath, out cCusName);

            //將客戶名稱填入下拉選單-客戶
            //comboBoxCusName.Items.AddRange(cCusName.CustomerName.ToArray());

            //取得OperationArray配置檔
            //string OperationArray_dat = "OperationArray.dat";
            //string OperationArrayDatPath = string.Format(@"{0}\{1}", CaxPE.GetPEConfigDir(), OperationArray_dat);
            //CaxPE.ReadOperationArrayData(OperationArrayDatPath, out cOperationArray);

            //將Operation2Array塞入陣列Oper2StringAry中
            //Oper2StringAry = cOperationArray.Operation2Array.ToArray();
            
            //建立GridPanel
            panel = OperSuperGridControl.PrimaryGrid;
            
            //設定製程別的基礎型態與數據
            panel.Columns["Oper2Ary"].EditorType = typeof(PEComboBox);
            //panel.Columns["Oper2Ary"].EditorParams = new object[] { Oper2StringAry };
            panel.Columns["Oper2Ary"].EditorParams = new object[] { operation2Name };

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
            //初始路徑
            //openFileDialog1.InitialDirectory = @"C:";

            openFileDialog1.Filter = "Part Files (*.prt)|*.prt|All Files (*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                //取得檔案名稱(檔名+副檔名)
                labelPartFileName.Text = openFileDialog1.SafeFileName;
                //取得檔案完整路徑(路徑+檔名+副檔名)
                PartPath = openFileDialog1.FileName;
                //開啟選擇的檔案
                CaxPart.OpenBaseDisplay(PartPath);
            }
            
        }

        private void OK_Click(object sender, EventArgs e)
        {
            //先關閉所有檔案
            CaxPart.CloseAllParts();

            try
            {
                if (Is_OldPart == true)
                {
                    //定義總組立檔案、二階檔案、三階檔案名稱
                    string AsmCompFileFullPath = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", CaxEnv.GetGlobaltekEnvDir(), "Task", comboBoxOldCusName.Text, comboBoxOldPartNo.Text, comboBoxOldCusRev.Text.ToUpper(), comboBoxOldPartNo.Text + "_MOT.prt");
                    string SecondFileFullPath = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", CaxEnv.GetGlobaltekEnvDir(), "Task", comboBoxOldCusName.Text, comboBoxOldPartNo.Text, comboBoxOldCusRev.Text.ToUpper(), comboBoxOldPartNo.Text + "_OP" + "[Oper1]" + ".prt");
                    string ThirdFileFullPath_OIS = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", CaxEnv.GetGlobaltekEnvDir(), "Task", comboBoxOldCusName.Text, comboBoxOldPartNo.Text, comboBoxOldCusRev.Text.ToUpper(), comboBoxOldPartNo.Text + "_OIS" + "[Oper1]" + ".prt");
                    string ThirdFileFullPath_CAM = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", CaxEnv.GetGlobaltekEnvDir(), "Task", comboBoxOldCusName.Text, comboBoxOldPartNo.Text, comboBoxOldCusRev.Text.ToUpper(), comboBoxOldPartNo.Text + "_OP" + "[Oper1]" + "_CAM.prt");
                    string OPFolderPath = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", CaxEnv.GetGlobaltekEnvDir(), "Task", comboBoxOldCusName.Text, comboBoxOldPartNo.Text, comboBoxOldCusRev.Text.ToUpper(), "OP" + "[Oper1]");
                    string tempSecondFileFullPath = SecondFileFullPath;
                    string tempThirdFileFullPath_OIS = ThirdFileFullPath_OIS;
                    string tempThirdFileFullPath_CAM = ThirdFileFullPath_CAM;
                    string tempOPFolderPath = OPFolderPath;


                    #region 開啟總組立
                    if (File.Exists(AsmCompFileFullPath))
                    {
                        //組件存在，直接開啟任務組立
                        BasePart newAsmPart;
                        status = CaxPart.OpenBaseDisplay(AsmCompFileFullPath, out newAsmPart);
                        if (!status)
                        {
                            CaxLog.ShowListingWindow("組立開啟失敗，料號不可有中文字！");
                            return;
                        }
                    }
                    else
                    {
                        CaxLog.ShowListingWindow("開啟失敗：找不到總組立" + Path.GetFileNameWithoutExtension(AsmCompFileFullPath));
                        return;
                    }
                    #endregion

                    #region 建立新插入的製程
                    NXOpen.Assemblies.Component tempComp;
                    
                    foreach (string i in ListAddOper)
                    {
                        //設定一階為WorkComp
                        CaxAsm.SetWorkComponent(null);

                        //建立二階檔案
                        SecondFileFullPath = tempSecondFileFullPath;
                        SecondFileFullPath = SecondFileFullPath.Replace("[Oper1]", i);
                        status = CaxAsm.CreateNewEmptyComp(SecondFileFullPath, out tempComp);
                        if (!status)
                        {
                            CaxLog.ShowListingWindow("建立二階製程檔失敗");
                            return;
                        }

                        //取得二階所有comp
                        List<NXOpen.Assemblies.Component> ChildenComp = new List<NXOpen.Assemblies.Component>();
                        CaxAsm.GetCompChildren(out ChildenComp);

                        foreach (NXOpen.Assemblies.Component ii in ChildenComp)
                        {
                            if (ii.Name == Path.GetFileNameWithoutExtension(SecondFileFullPath).ToUpper())
                            {
                                CaxAsm.SetWorkComponent(ii);
                            }
                        }

                        //建立三階檔案
                        ThirdFileFullPath_OIS = tempThirdFileFullPath_OIS;
                        ThirdFileFullPath_CAM = tempThirdFileFullPath_CAM;

                        ThirdFileFullPath_OIS = ThirdFileFullPath_OIS.Replace("[Oper1]", i);
                        ThirdFileFullPath_CAM = ThirdFileFullPath_CAM.Replace("[Oper1]", i);
                        status = CaxAsm.CreateNewEmptyComp(ThirdFileFullPath_OIS, out tempComp);
                        if (!status)
                        {
                            CaxLog.ShowListingWindow("建立三階OIS檔失敗");
                            return;
                        }
                        status = CaxAsm.CreateNewEmptyComp(ThirdFileFullPath_CAM, out tempComp);
                        if (!status)
                        {
                            CaxLog.ShowListingWindow("建立三階CAM檔失敗");
                            return;
                        }
                    }
                    #endregion

                    #region 建立新插入的製程資料夾
                    foreach (string i in ListAddOper)
                    {
                        OPFolderPath = tempOPFolderPath;
                        OPFolderPath = OPFolderPath.Replace("[Oper1]", i);
                        string OISFolderPath = string.Format(@"{0}\{1}", OPFolderPath, "OIS");
                        string CAMFolderPath = string.Format(@"{0}\{1}", OPFolderPath, "CAM");

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
                    }
                    #endregion

                    #region 將值儲存起來
                    cPECreateData.cusName = comboBoxOldCusName.Text;
                    cPECreateData.partName = comboBoxOldPartNo.Text;
                    cPECreateData.cusRev = comboBoxOldCusRev.Text.ToUpper();
                    cPECreateData.listOperation = new List<Operation>();
                    Operation cOperation = new Operation();
                    cPECreateData.oper1Ary = new List<string>();
                    cPECreateData.oper2Ary = new List<string>();
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

                        cPECreateData.listOperation.Add(cOperation);

                        cPECreateData.oper1Ary.Add(panel.GetCell(i, 0).Value.ToString());
                        cPECreateData.oper2Ary.Add(panel.GetCell(i, 1).Value.ToString());
                    }
                    #endregion

                    #region 寫出PECreateData.dat
                    string PECreateDataJsonDat = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", CaxEnv.GetGlobaltekTaskDir(), CurrentOldCusName, CurrentOldPartNo, CurrentOldCusRev, "MODEL", "PECreateData.dat");
                    status = CaxFile.WriteJsonFileData(PECreateDataJsonDat, cPECreateData);
                    if (!status)
                    {
                        MessageBox.Show("PECreateData.dat 輸出失敗...");
                        return;
                    }
                    #endregion

                    //寫Update Datebase

                }
                else
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
                    if (!System.IO.File.Exists(CustomerPartFullPath))
                    {
                        File.Copy(PartPath, CustomerPartFullPath, true);
                    }
                

                    #endregion

                    #region 將值儲存起來

                    cPECreateData.cusName = CusName;
                    cPECreateData.partName = PartNo;
                    cPECreateData.cusRev = CusRev.ToUpper();
                    //cPE_OutPutDat.PartPath = PartPath;
                    cPECreateData.listOperation = new List<Operation>();
                    Operation cOperation = new Operation();
                    cPECreateData.oper1Ary = new List<string>();
                    cPECreateData.oper2Ary = new List<string>();
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
                    //cOperation.CAMFolderPath = CAMFolderPath;

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
                    //cOperation.OISFolderPath = OISFolderPath;

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

                    cPECreateData.listOperation.Add(cOperation);

                    cPECreateData.oper1Ary.Add(panel.GetCell(i, 0).Value.ToString());
                    cPECreateData.oper2Ary.Add(panel.GetCell(i, 1).Value.ToString()); 
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

                    for (int i = 0; i < cPECreateData.listOperation.Count; i++)
                    {
                        //設定一階為WorkComp
                        CaxAsm.SetWorkComponent(null);

                        //建立二階製程檔
                        OPCompName = string.Format(@"{0}\{1}", Path.GetDirectoryName(AsmCompFileFullPath), PartNo + "_OP" + cPECreateData.listOperation[i].Oper1 + ".prt");
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
                    CaxAsm.GetCompChildren(out ChildenComp);

                    for (int i = 0; i < ChildenComp.Count; i++)
                    {
                        CaxAsm.SetWorkComponent(ChildenComp[i]);
                        string OperStr = ChildenComp[i].Name.Split(new string[] { "OP" }, StringSplitOptions.RemoveEmptyEntries)[1];

                        #region 建立三階CAM檔
                        //建立三階CAM檔
                        CAMCompFullPath = string.Format(@"{0}\{1}", Path.GetDirectoryName(AsmCompFileFullPath), PartNo + "_OP" + OperStr + "_CAM.prt");
                        status = CaxAsm.CreateNewEmptyComp(CAMCompFullPath, out tempComp);
                        if (!status)
                        {
                            CaxLog.ShowListingWindow("建立三階CAM檔失敗");
                            return;
                        }
                        #endregion
                    

                        #region 建立三階OIS檔
                        //先複製drafting_template.prt到OIS檔
                        string drafting_template_Path = string.Format(@"{0}\{1}", CaxEnv.GetGlobaltekEnvDir(), "drafting_template.prt");
                        OISCompFullPath = string.Format(@"{0}\{1}", Path.GetDirectoryName(AsmCompFileFullPath), PartNo + "_OIS" + OperStr + ".prt");
                        if (!File.Exists(drafting_template_Path))
                        {
                            CaxLog.ShowListingWindow("drafting_template.prt遺失，請聯繫開發工程師");
                            return;
                        }
                        System.IO.File.Copy(drafting_template_Path, OISCompFullPath, true);

                        //組立三階OIS檔
                        //status = CaxAsm.CreateNewEmptyComp(OISCompFullPath, out tempComp);
                        status = CaxAsm.AddComponentToAsmByDefault(OISCompFullPath, out tempComp);
                        if (!status)
                        {
                            CaxLog.ShowListingWindow("組立三階OIS檔失敗");
                            return;
                        }
                        #endregion
                    
                    }

                    #endregion

                    #region 寫出PECreateData.dat

                    string PECreateDataJsonDat = string.Format(@"{0}\{1}", ModelFolderFullPath, "PECreateData.dat");
                    status = CaxFile.WriteJsonFileData(PECreateDataJsonDat, cPECreateData);
                    if (!status)
                    {
                        MessageBox.Show("PECreateData.dat 輸出失敗...");
                        return;
                    }

                    #endregion

                    #region (註解中)寫出METEDownloadData.dat

                    //string METEDownloadData = string.Format(@"{0}\{1}", CaxEnv.GetGlobaltekTaskDir(), "METEDownloadData.dat");
                    //METEDownloadData cMETEDownloadData = new METEDownloadData();

                    //if (File.Exists(METEDownloadData))
                    //{
                    //    #region METEDownloadData.dat檔案存在

                    //    status = CaxPublic.ReadMETEDownloadData(METEDownloadData, out cMETEDownloadData);
                    //    if (!status)
                    //    {
                    //        MessageBox.Show("METEDownloadData.dat讀取失敗...");
                    //        return;
                    //    }

                    //    int CusCount = 0, IndexOfCusName = -1;
                    //    for (int i = 0; i < cMETEDownloadData.EntirePartAry.Count; i++)
                    //    {
                    //        if (CusName != cMETEDownloadData.EntirePartAry[i].CusName)
                    //        {
                    //            CusCount++;
                    //        }
                    //        else
                    //        {
                    //            IndexOfCusName = i;
                    //            break;
                    //        }
                    //    }

                    //    //新的客戶且已經有METEDownloadDat.dat
                    //    if (CusCount == cMETEDownloadData.EntirePartAry.Count)
                    //    {
                    //        EntirePartAry cEntirePartAry = new EntirePartAry();
                    //        cEntirePartAry.CusName = CusName;
                    //        cEntirePartAry.CusPart = new List<CusPart>();

                    //        CusPart cCusPart = new CusPart();
                    //        cCusPart.PartNo = PartNo;
                    //        cCusPart.CusRev = new List<CusRev>();

                    //        CusRev cCusRev = new CusRev();
                    //        cCusRev.RevNo = CusRev.ToUpper();
                    //        cCusRev.OperAry1 = new List<string>();
                    //        cCusRev.OperAry2 = new List<string>();
                    //        cCusRev.OperAry1 = cPECreateData.Oper1Ary;
                    //        cCusRev.OperAry2 = cPECreateData.Oper2Ary;

                    //        cCusPart.CusRev.Add(cCusRev);
                    //        cEntirePartAry.CusPart.Add(cCusPart);
                    //        cMETEDownloadData.EntirePartAry.Add(cEntirePartAry);
                    //    }
                    //    //舊的客戶新增料號
                    //    else
                    //    {
                    //        //判斷料號是否已存在
                    //        int PartCount = 0; int IndexOfPartNo = -1;
                    //        for (int i = 0; i < cMETEDownloadData.EntirePartAry[IndexOfCusName].CusPart.Count; i++)
                    //        {
                    //            if (PartNo != cMETEDownloadData.EntirePartAry[IndexOfCusName].CusPart[i].PartNo)
                    //            {
                    //                PartCount++;
                    //            }
                    //            else
                    //            {
                    //                IndexOfPartNo = i;
                    //                break;
                    //            }
                    //        }

                    //        //舊的客戶且新的料號 PartCount == CusPart.Count 表示新的料號
                    //        if (PartCount == cMETEDownloadData.EntirePartAry[IndexOfCusName].CusPart.Count)
                    //        {
                    //            CusPart cCusPart = new CusPart();
                    //            cCusPart.PartNo = PartNo;
                    //            cCusPart.CusRev = new List<CusRev>();

                    //            CusRev cCusRev = new CusRev();
                    //            cCusRev.RevNo = CusRev.ToUpper();
                    //            cCusRev.OperAry1 = new List<string>();
                    //            cCusRev.OperAry2 = new List<string>();
                    //            cCusRev.OperAry1 = cPECreateData.Oper1Ary;
                    //            cCusRev.OperAry2 = cPECreateData.Oper2Ary;

                    //            cCusPart.CusRev.Add(cCusRev);
                    //            cMETEDownloadData.EntirePartAry[IndexOfCusName].CusPart.Add(cCusPart);
                    //        }
                    //        //舊的客戶且舊的料號新增客戶版次
                    //        else
                    //        {
                    //            CusRev cCusRev = new CusRev();
                    //            cCusRev.RevNo = CusRev.ToUpper();
                    //            cCusRev.OperAry1 = new List<string>();
                    //            cCusRev.OperAry1 = cPECreateData.Oper1Ary;

                    //            cMETEDownloadData.EntirePartAry[IndexOfCusName].CusPart[IndexOfPartNo].CusRev.Add(cCusRev);
                    //        }
                    //    }
                    //    /*
                    //    int PartCount = 0; int IndexOfPartNo = -1;
                    //    for (int i = 0; i < cMETEDownloadData.EntirePartAry.Count; i++)
                    //    {
                    //    if (PartNo != cMETEDownloadData.EntirePartAry[i].PartNo)
                    //    {
                    //    PartCount++;
                    //    }
                    //    else
                    //    {
                    //    IndexOfPartNo = i;
                    //    break;
                    //    }
                    //    }

                    //    //新的料號且已經有METEDownloadDat.dat
                    //    if (PartCount == cMETEDownloadData.EntirePartAry.Count)
                    //    {
                    //    EntirePartAry cEntirePartAry = new EntirePartAry();
                    //    cEntirePartAry.CusRev = new List<CusRev>();

                    //    CusRev cCusRev = new CusRev();
                    //    cCusRev.OperAry1 = new List<string>();
                    //    cCusRev.RevNo = CusRev.ToUpper();
                    //    cCusRev.OperAry1 = cPE_OutPutDat.Oper1Ary;

                    //    cEntirePartAry.CusName = CusName;
                    //    cEntirePartAry.PartNo = PartNo;
                    //    cEntirePartAry.CusRev.Add(cCusRev);

                    //    cMETEDownloadData.EntirePartAry.Add(cEntirePartAry);
                    //    }
                    //    //舊的料號新增客戶版次
                    //    else
                    //    {
                    //    CusRev cCusRev = new CusRev();
                    //    cCusRev.OperAry1 = new List<string>();
                    //    cCusRev.RevNo = CusRev.ToUpper();
                    //    cCusRev.OperAry1 = cPE_OutPutDat.Oper1Ary;

                    //    cMETEDownloadData.EntirePartAry[IndexOfPartNo].CusRev.Add(cCusRev);
                    //    }
                    //    */
                    //    #endregion
                    //}
                    //else
                    //{
                    //    #region METEDownloadData.dat檔案不存在

                    //    cMETEDownloadData.EntirePartAry = new List<EntirePartAry>();
                    //    EntirePartAry cEntirePartAry = new EntirePartAry();
                    //    cEntirePartAry.CusName = CusName;
                    //    cEntirePartAry.CusPart = new List<CusPart>();

                    //    CusPart cCusPart = new CusPart();
                    //    cCusPart.PartNo = PartNo;
                    //    cCusPart.CusRev = new List<CusRev>();

                    //    CusRev cCusRev = new CusRev();
                    //    cCusRev.RevNo = CusRev.ToUpper();
                    //    cCusRev.OperAry1 = new List<string>();
                    //    cCusRev.OperAry2 = new List<string>();
                    //    cCusRev.OperAry1 = cPECreateData.Oper1Ary;
                    //    cCusRev.OperAry2 = cPECreateData.Oper2Ary;

                    //    cCusPart.CusRev.Add(cCusRev);
                    //    cEntirePartAry.CusPart.Add(cCusPart);
                    //    cMETEDownloadData.EntirePartAry.Add(cEntirePartAry);

                    //    #endregion
                    //}

                    //status = CaxFile.WriteJsonFileData(METEDownloadData, cMETEDownloadData);
                    //if (!status)
                    //{
                    //    MessageBox.Show("METEDownloadData.dat輸出失敗...");
                    //    return;
                    //}

                    #endregion

                    //寫Save Datebase
                    using (ISession session = MyHibernateHelper.SessionFactory.OpenSession())
                    {
                        

                        #region 插入Com_PEMain
                        cCom_PEMain.partName = cPECreateData.partName;
                        cCom_PEMain.customerVer = cPECreateData.cusRev;
                        cCom_PEMain.createDate = DateTime.Now.ToString();
                        

                        IList<Com_PartOperation> listComPartOperation = new List<Com_PartOperation>();
                        foreach (Operation i in cPECreateData.listOperation)
                        {
                            cCom_PartOperation = new Com_PartOperation();
                            cCom_PartOperation.operation1 = i.Oper1;
                            cCom_PartOperation.sysOperation2 = session.QueryOver<Sys_Operation2>()
                                                                .Where(x => x.operation2Name == i.Oper2).SingleOrDefault();
                            cCom_PartOperation.comPEMain = cCom_PEMain;
                            listComPartOperation.Add(cCom_PartOperation);
                        }
                        cCom_PEMain.comPartOperation = listComPartOperation;

                        using (ITransaction trans = session.BeginTransaction())
                        {
                            //session.Save(cCom_PartOperation);
                            session.Save(cCom_PEMain);

                            trans.Commit();
                        }
                        #endregion


                        /*
                        #region 插入Com_PartOperation
                        foreach (Operation i in cPECreateData.listOperation)
                        {
                            cCom_PartOperation = new Com_PartOperation();
                            cCom_PartOperation.operation1 = i.Oper1;
                            cCom_PartOperation.sysOperation2 = session.QueryOver<Sys_Operation2>()
                                                                .Where(x => x.operation2Name == i.Oper2).SingleOrDefault();
                            cCom_PartOperation.comPEMain = session.QueryOver<Com_PEMain>()
                                                            .Where(x => x.partName == PartNo).SingleOrDefault();
                            using (ITransaction trans = session.BeginTransaction())
                            {
                                session.Save(cCom_PartOperation);
                                trans.Commit();
                            }
                        }
                        #endregion
                        */

                        session.Close();
                        



                    }

                }

                
                
                
                //Console.Read();//暫停畫面用

                CaxAsm.SetWorkComponent(null);
                CaxPart.Save();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow(ex.ToString());
            }

        }

        private void UserDefine_Click(object sender, EventArgs e)
        {
            //判斷使用者選取的製程序是否已經存在於OperSuperGridControl
            if (!(panel.Rows.Count == 0))
            {
                for (int i = 0; i < panel.Rows.Count; i++)
                {
                    if (panel.GridPanel.GetCell(i, 0).Value.ToString() == UserDefineProcess.Text)
                    {
                        MessageBox.Show("已有重複的製程序");
                        //清除使用者選取的製程序
                        UserDefineProcess.Text = "";
                        return;
                    }
                }
            }

            //將製程序填入OperSuperGridControl
            GridRow row = new GridRow();
            row = new GridRow(UserDefineProcess.Text, "", "刪除");
            panel.Rows.Add(row);
            ListAddOper.Add(UserDefineProcess.Text);
            UserDefineProcess.Text = "";
            
        }

        private void PEGenerateDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
            {
                CaxPart.CloseAllParts();
            }
        }

        private void comboBoxOldCusName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空superGrid資料
            panel.Rows.Clear();
            //取得當前選取的客戶
            CurrentOldCusName = comboBoxOldCusName.Text;
            //打開&清空下拉選單-料號
            comboBoxOldPartNo.Enabled = true;
            comboBoxOldPartNo.Items.Clear();
            comboBoxOldPartNo.Text = "";
            //關閉&清空下拉選單-客戶版次
            comboBoxOldCusRev.Enabled = false;
            comboBoxOldCusRev.Items.Clear();
            comboBoxOldCusRev.Text = "";

            string S_Task_CusName_Path = string.Format(@"{0}\{1}", CaxEnv.GetGlobaltekTaskDir(), CurrentOldCusName);
            string[] S_Task_PartNo = Directory.GetDirectories(S_Task_CusName_Path);
            foreach (string item in S_Task_PartNo)
            {
                comboBoxOldPartNo.Items.Add(Path.GetFileNameWithoutExtension(item));//走訪每個元素只取得目錄名稱(不含路徑)並加入dirlist集合中
            }

            /*
            //比對選擇的客戶取得對應的料號並塞入料號下拉選單中
            for (int i = 0; i < cMETEDownloadData.EntirePartAry.Count; i++)
            {
                if (CurrentOldCusName == cMETEDownloadData.EntirePartAry[i].CusName)
                {
                    IndexofCusName = i;
                    for (int j = 0; j < cMETEDownloadData.EntirePartAry[i].CusPart.Count; j++)
                    {
                        comboBoxOldPartNo.Items.Add(cMETEDownloadData.EntirePartAry[i].CusPart[j].PartNo);
                    }
                }
            }
            */
        }

        private void comboBoxOldPartNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空superGrid資料
            panel.Rows.Clear();
            //取得當前選取的料號
            CurrentOldPartNo = comboBoxOldPartNo.Text;
            //打開&清空下拉選單-客戶版次
            comboBoxOldCusRev.Enabled = true;
            comboBoxOldCusRev.Items.Clear();
            comboBoxOldCusRev.Text = "";

            string S_Task_PartNo_Path = string.Format(@"{0}\{1}\{2}", CaxEnv.GetGlobaltekTaskDir(), CurrentOldCusName, CurrentOldPartNo);
            string[] S_Task_CusRev = Directory.GetDirectories(S_Task_PartNo_Path);
            foreach (string item in S_Task_CusRev)
            {
                comboBoxOldCusRev.Items.Add(Path.GetFileNameWithoutExtension(item));//走訪每個元素只取得目錄名稱(不含路徑)並加入dirlist集合中
            }

            /*
            //比對選擇的客戶與料號取得對應的客戶版次並塞入客戶版次下拉選單中
            for (int i = 0; i < cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart.Count; i++)
            {
                if (CurrentOldPartNo == cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[i].PartNo)
                {
                    IndexofPartNo = i;
                    for (int j = 0; j < cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[i].CusRev.Count; j++)
                    {
                        comboBoxOldCusRev.Items.Add(cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[i].CusRev[j].RevNo);
                    }
                }
            }
            */
        }

        private void comboBoxOldCusRev_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空superGrid資料
            panel.Rows.Clear();
            //取得當前選取的客戶版次
            CurrentOldCusRev = comboBoxOldCusRev.Text;

            //取得PECreateData.dat
            string PECreateData_Path = string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", CaxEnv.GetGlobaltekTaskDir(), CurrentOldCusName, CurrentOldPartNo, CurrentOldCusRev, "MODEL", "PECreateData.dat");
            if (!File.Exists(PECreateData_Path))
            {
                CaxLog.ShowListingWindow("此料號沒有舊資料檔案，請檢查PECreateData.dat");
                return;
            }
            CaxPE.ReadPECreateData(PECreateData_Path, out cPECreateData);

            //將舊資料填入SuperGridControl
            GridRow row = new GridRow();
            for (int i = 0; i < cPECreateData.oper1Ary.Count;i++ )
            {
                row = new GridRow(cPECreateData.oper1Ary[i], cPECreateData.oper2Ary[i], "刪除");
                panel.Rows.Add(row);
            }

            /*
            //比對選擇的客戶版次取得對應的Oper並塞入SuperGridControl
            List<string> ListOper1 = new List<string>();
            List<string> ListOper2 = new List<string>();
            for (int i = 0; i < cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[IndexofPartNo].CusRev.Count; i++)
            {
                if (CurrentOldCusRev == cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[IndexofPartNo].CusRev[i].RevNo)
                {
                    ListOper1 = cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[IndexofPartNo].CusRev[i].OperAry1;
                    ListOper2 = cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[IndexofPartNo].CusRev[i].OperAry2;
                }
            }

            GridRow row = new GridRow();
            for (int i = 0; i < ListOper1.Count;i++ )
            {
                row = new GridRow(ListOper1[i], ListOper2[i], "刪除");
                panel.Rows.Add(row);
            }
            */
            Is_OldPart = true;
        }

        private void comboBoxCusName_SelectedIndexChanged(object sender, EventArgs e)
        {
            cCom_PEMain.sysCustomer = ((Sys_Customer)comboBoxCusName.SelectedItem);
        }
    }
     
    public class PEComboBox : GridComboBoxExEditControl
    {
        public PEComboBox(IEnumerable Oper2StringAry)
        {
            DataSource = Oper2StringAry;

            DisplayMember = "operation2Name";
            //ValueMember = "operation2SrNo";
            
            DropDownStyle = ComboBoxStyle.DropDownList;
            //this.MouseWheel += new MouseEventHandler(Oper2Changed);
            this.DropDownClosed += new EventHandler(Oper2Changed);
        }

        public void Oper2Changed(object sender, EventArgs e)
        {
            //Sys_Operation2 a = new Sys_Operation2();
            //a = (Sys_Operation2)SelectedItem;
            //CaxLog.ShowListingWindow(a.operation2SrNo.ToString());
            //CaxLog.ShowListingWindow(a.operation2Name.ToString());

            //CaxLog.ShowListingWindow("1");
            //PEComboBox aa = (PEComboBox)sender;
            //int index = aa.EditorCell.RowIndex;
            //MessageBox.Show(index.ToString());
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
            string SelOper = panel.GetCell(index, 0).Value.ToString();
            PEGenerateDlg.ListAddOper.Remove(SelOper);
            panel.Rows.RemoveAt(index);
        }
    }
}
