using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CaxGlobaltek;
using System.IO;

namespace TEDownload
{
    public partial class TEDownloadDlg : DevComponents.DotNetBar.Office2007Form
    {
        public static bool status;
        public static METEDownloadData cMETEDownloadData = new METEDownloadData();
        public static METE_Download_Upload_Path cMETE_Download_Upload_Path = new METE_Download_Upload_Path();
        public static string CurrentCusName = "", CurrentPartNo = "", CurrentCusRev = "", CurrentOper1 = "";
        public static string Server_MODEL = "", Server_TEDownloadPart = "";
        public static string Local_Folder_MODEL = "", Local_Folder_CAM = "", Local_Folder_OIS = "";
        public static Dictionary<string, string> DicSeleOper1 = new Dictionary<string, string>();
        public static List<string> ListSeleOper1 = new List<string>();
        public static string tempServer_TEDownloadPart = "", tempLocal_Folder_CAM = "", tempLocal_Folder_OIS = "";
        public static int IndexofCusName = -1, IndexofPartNo = -1;

        public TEDownloadDlg()
        {
            InitializeComponent();

            //取得METEDownloadData資料
            status = CaxGetDatData.GetMETEDownloadData(out cMETEDownloadData);
            if (!status)
            {
                MessageBox.Show("取得METEDownloadData失敗");
                return;
            }

            //存入下拉選單-客戶
            for (int i = 0; i < cMETEDownloadData.EntirePartAry.Count; i++)
            {
                comboBoxCusName.Items.Add(cMETEDownloadData.EntirePartAry[i].CusName);
            }

            PartNocomboBox.Enabled = false;
            CusRevcomboBox.Enabled = false;
            Oper1comboBox.Enabled = false;

            //取得METEDownload_Upload資料
            status = CaxGetDatData.GetMETEDownload_Upload(out cMETE_Download_Upload_Path);
            if (!status)
            {
                MessageBox.Show("取得METEDownload_Upload失敗");
                return;
            }
        }

        private void comboBoxCusName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空ListView資訊
            listView.Items.Clear();
            //取得當前選取的客戶
            CurrentCusName = comboBoxCusName.Text;
            //打開&清空下拉選單-料號
            PartNocomboBox.Enabled = true;
            PartNocomboBox.Items.Clear();
            PartNocomboBox.Text = "";
            //關閉&清空下拉選單-客戶版次
            CusRevcomboBox.Enabled = false;
            CusRevcomboBox.Items.Clear();
            CusRevcomboBox.Text = "";
            //關閉&清空下拉選單-製程序
            Oper1comboBox.Enabled = false;
            Oper1comboBox.Items.Clear();
            Oper1comboBox.Text = "";

            //比對選擇的客戶取得對應的料號並塞入料號下拉選單中
            for (int i = 0; i < cMETEDownloadData.EntirePartAry.Count; i++)
            {
                if (CurrentCusName == cMETEDownloadData.EntirePartAry[i].CusName)
                {
                    IndexofCusName = i;
                    for (int j = 0; j < cMETEDownloadData.EntirePartAry[i].CusPart.Count; j++)
                    {
                        PartNocomboBox.Items.Add(cMETEDownloadData.EntirePartAry[i].CusPart[j].PartNo);
                    }
                }
            }
        }

        private void PartNocomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空ListView資訊
            listView.Items.Clear();
            //取得當前選取的料號
            CurrentPartNo = PartNocomboBox.Text;
            //打開&清空下拉選單-客戶版次
            CusRevcomboBox.Enabled = true;
            CusRevcomboBox.Items.Clear();
            CusRevcomboBox.Text = "";
            //關閉&清空下拉選單-製程序
            Oper1comboBox.Enabled = false;
            Oper1comboBox.Items.Clear();
            Oper1comboBox.Text = "";

            //比對選擇的客戶與料號取得對應的客戶版次並塞入客戶版次下拉選單中
            for (int i = 0; i < cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart.Count; i++)
            {
                if (CurrentPartNo == cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[i].PartNo)
                {
                    IndexofPartNo = i;
                    for (int j = 0; j < cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[i].CusRev.Count; j++)
                    {
                        CusRevcomboBox.Items.Add(cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[i].CusRev[j].RevNo);
                    }
                }
            }
        }

        private void CusRevcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空ListView資訊
            listView.Items.Clear();
            //取得當前選取的客戶版次
            CurrentCusRev = CusRevcomboBox.Text;
            //打開&清空下拉選單-製程序
            Oper1comboBox.Enabled = true;
            Oper1comboBox.Items.Clear();
            Oper1comboBox.Text = "";

            //比對選擇的客戶、料號、版次取得對應的製程序並塞入下拉選單中
            for (int i = 0; i < cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[IndexofPartNo].CusRev.Count; i++)
            {
                if (CurrentCusRev == cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[IndexofPartNo].CusRev[i].RevNo)
                {
                    Oper1comboBox.Items.AddRange(cMETEDownloadData.EntirePartAry[IndexofCusName].CusPart[IndexofPartNo].CusRev[i].OperAry1.ToArray());
                }
            }
            Oper1comboBox.Items.Add("全部下載");
        }

        private void Oper1comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空ListView資訊
            listView.Items.Clear();
            //取得當前選取的製程序
            CurrentOper1 = Oper1comboBox.Text;

            //建立Server路徑資料
            string Server_IP = cMETE_Download_Upload_Path.Server_IP;
            string Server_ShareStr = cMETE_Download_Upload_Path.Server_ShareStr;
            Server_MODEL = cMETE_Download_Upload_Path.Server_MODEL;
            Server_TEDownloadPart = cMETE_Download_Upload_Path.Server_TEDownloadPart;

            //取代字串成正確路徑
            Server_ShareStr = Server_ShareStr.Replace("[Server_IP]", Server_IP);
            Server_ShareStr = Server_ShareStr.Replace("[CusName]", CurrentCusName);
            Server_ShareStr = Server_ShareStr.Replace("[PartNo]", CurrentPartNo);
            Server_ShareStr = Server_ShareStr.Replace("[CusRev]", CurrentCusRev);
            Server_MODEL = Server_MODEL.Replace("[Server_ShareStr]", Server_ShareStr);
            Server_MODEL = Server_MODEL.Replace("[PartNo]", CurrentPartNo);
            Server_TEDownloadPart = Server_TEDownloadPart.Replace("[Server_ShareStr]", Server_ShareStr);
            Server_TEDownloadPart = Server_TEDownloadPart.Replace("[PartNo]", CurrentPartNo);

            //先判斷客戶檔案是否存在
            if (!File.Exists(Server_MODEL))
            {
                listView.Items.Add("客戶檔案不存在，無法下載");
                return;
            }
            listView.Items.Add("客戶檔案：" + Path.GetFileName(Server_MODEL));

            //暫存一個Server_MEDownloadPart，目的要讓程式每次都能有[Oper1]可取代
            tempServer_TEDownloadPart = Server_TEDownloadPart;

            //將選取到的Oper1紀錄成DicSeleOper1(Key = 製程序,Value = ServerPartPath)
            DicSeleOper1 = new Dictionary<string, string>();
            if (CurrentOper1 == "全部下載")
            {
                for (int i = 0; i < Oper1comboBox.Items.Count; i++)
                {
                    if (Oper1comboBox.Items[i].ToString() == "全部下載")
                    {
                        continue;
                    }
                    Server_TEDownloadPart = tempServer_TEDownloadPart;
                    Server_TEDownloadPart = Server_TEDownloadPart.Replace("[Oper1]", Oper1comboBox.Items[i].ToString());

                    string ServerPartPath = "";
                    status = DicSeleOper1.TryGetValue(Oper1comboBox.Items[i].ToString(), out ServerPartPath);
                    if (!status)
                    {
                        DicSeleOper1.Add(Oper1comboBox.Items[i].ToString(), Server_TEDownloadPart);
                    }
                }
            }
            else
            {
                Server_TEDownloadPart = tempServer_TEDownloadPart;
                Server_TEDownloadPart = Server_TEDownloadPart.Replace("[Oper1]", CurrentOper1);
                DicSeleOper1.Add(CurrentOper1, Server_TEDownloadPart);
            }

            //判斷製程檔案是否存在
            foreach (KeyValuePair<string, string> kvp in DicSeleOper1)
            {
                if (!File.Exists(kvp.Value))
                {
                    listView.Items.Add("製程序" + kvp.Key + "檔案不存在，無法下載");
                    return;
                }
                listView.Items.Add("製程序檔案：" + Path.GetFileName(kvp.Value));
            }



        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            //建立Local路徑資料
            string Local_IP = cMETE_Download_Upload_Path.Local_IP;
            string Local_ShareStr = cMETE_Download_Upload_Path.Local_ShareStr;
            Local_Folder_MODEL = cMETE_Download_Upload_Path.Local_Folder_MODEL;
            Local_Folder_CAM = cMETE_Download_Upload_Path.Local_Folder_CAM;
            Local_Folder_OIS = cMETE_Download_Upload_Path.Local_Folder_OIS;

            //取代字串成正確路徑
            Local_ShareStr = Local_ShareStr.Replace("[Local_IP]", Local_IP);
            Local_ShareStr = Local_ShareStr.Replace("[CusName]", CurrentCusName);
            Local_ShareStr = Local_ShareStr.Replace("[PartNo]", CurrentPartNo);
            Local_ShareStr = Local_ShareStr.Replace("[CusRev]", CurrentCusRev);
            Local_Folder_MODEL = Local_Folder_MODEL.Replace("[Local_ShareStr]", Local_ShareStr);
            Local_Folder_CAM = Local_Folder_CAM.Replace("[Local_ShareStr]", Local_ShareStr);
            Local_Folder_OIS = Local_Folder_OIS.Replace("[Local_ShareStr]", Local_ShareStr);

            #region 建立Local_Folder_MODEL資料夾

            if (!File.Exists(Local_Folder_MODEL))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(Local_Folder_MODEL);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }

            #endregion

            #region 複製Server客戶檔案到Local_Folder_MODEL資料夾內

            //判斷客戶檔案是否存在
            status = System.IO.File.Exists(Server_MODEL);
            if (!status)
            {
                MessageBox.Show("指定的檔案不存在，請再次確認");
                return;
            }

            //建立Local_Folder_MODEL資料夾內客戶檔案路徑
            string Local_CusPartFullPath = string.Format(@"{0}\{1}", Local_Folder_MODEL, Path.GetFileName(Server_MODEL));

            //判斷是否存在，不存在則開始複製
            if (!File.Exists(Local_CusPartFullPath))
            {
                File.Copy(Server_MODEL, Local_CusPartFullPath, true);
            }

            #endregion

            #region 建立Local_Folder_CAM、Local_Folder_OIS資料夾

            //暫存一個tempLocal_Folder_CAM、Local_Folder_OIS，目的要讓程式每次都能有[Oper1]可取代
            tempLocal_Folder_CAM = Local_Folder_CAM;
            tempLocal_Folder_OIS = Local_Folder_OIS;

            //DicSeleOper1(Key = 製程序,Value = ServerPartPath)
            foreach (KeyValuePair<string, string> kvp in DicSeleOper1)
            {
                Local_Folder_CAM = tempLocal_Folder_CAM;
                Local_Folder_OIS = tempLocal_Folder_OIS;
                Local_Folder_CAM = Local_Folder_CAM.Replace("[Oper1]", kvp.Key);
                Local_Folder_OIS = Local_Folder_OIS.Replace("[Oper1]", kvp.Key);
                if (!File.Exists(Local_Folder_CAM))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(Local_Folder_CAM);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return;
                    }
                }
                if (!File.Exists(Local_Folder_OIS))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(Local_Folder_OIS);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return;
                    }
                }
            }

            #endregion

            #region 複製Server製程序檔案到Local資料夾內

            foreach (KeyValuePair<string, string> kvp in DicSeleOper1)
            {
                //判斷製程序檔案是否存在
                Server_TEDownloadPart = tempServer_TEDownloadPart;
                Server_TEDownloadPart = Server_TEDownloadPart.Replace("[Oper1]", kvp.Key);
                status = System.IO.File.Exists(Server_TEDownloadPart);
                if (!status)
                {
                    MessageBox.Show("製程序檔案" + Path.GetFileName(Server_TEDownloadPart) + "不存在，請再次確認");
                    return;
                }

                //建立Local_ShareStr資料夾內製程序檔案路徑
                string Local_Oper1PartFullPath = string.Format(@"{0}\{1}", Local_ShareStr, Path.GetFileName(Server_TEDownloadPart));

                //開始複製
                File.Copy(Server_TEDownloadPart, Local_Oper1PartFullPath, true);
            }

            #endregion

            MessageBox.Show("下載完成！");
            this.Close();
        }
    }
}
