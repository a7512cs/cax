﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;
using DevComponents.DotNetBar.SuperGrid;
using NXOpen.CAM;
using System.Text.RegularExpressions;
using CaxGlobaltek;
using System.IO;

namespace PostProcessor
{
    public partial class PostProcessorDlg : DevComponents.DotNetBar.Office2007Form
    {
        public static Session theSession = Session.GetSession();
        public static UI theUI;
        public static UFSession theUfSession = UFSession.GetUFSession();
        public static Part workPart = theSession.Parts.Work;
        public static Part displayPart = theSession.Parts.Display;

        public static GridPanel OperPanel = new GridPanel();
        public static GridPanel PostPanel = new GridPanel();
        public static NXOpen.CAM.NCGroup[] NCGroupAry = new NXOpen.CAM.NCGroup[] { };
        public static NXOpen.CAM.Operation[] OperationAry = new NXOpen.CAM.Operation[] { };
        public static Dictionary<string, string> DicNCData = new Dictionary<string, string>();
        public static string CurrentNCGroup = "", CurrentSelPostName = "";
        public static int CurrentRowIndexofPostPanel = -1;
        public static string NCFolderPath = "";


        public PostProcessorDlg()
        {
            InitializeComponent();

            //建立panel物件
            OperPanel = SuperGridOperPanel.PrimaryGrid;
            PostPanel = SuperGridPostPanel.PrimaryGrid;
            
            //SelectAll.Checked = true;
            
        }

        private void PostProcessorDlg_Load(object sender, EventArgs e)
        {
            //取得所有GroupAry，用來判斷Group的Type決定是NC、Tool、Geometry
            NCGroupAry = displayPart.CAMSetup.CAMGroupCollection.ToArray();
            //取得所有operationAry
            OperationAry = displayPart.CAMSetup.CAMOperationCollection.ToArray();
            //建立NC資料夾
            NCFolderPath = string.Format(@"{0}\{1}", Path.GetDirectoryName(displayPart.FullPath), "NC");
            if (!Directory.Exists(NCFolderPath))
            {
                System.IO.Directory.CreateDirectory(NCFolderPath);
            }

            #region 取得相關資訊，填入Dic
            DicNCData = new Dictionary<string, string>();
            foreach (NXOpen.CAM.NCGroup ncGroup in NCGroupAry)
            {
                int type;
                int subtype;
                theUfSession.Obj.AskTypeAndSubtype(ncGroup.Tag, out type, out subtype);
                //判斷是否為Program群組
                if (type != UFConstants.UF_machining_task_type)
                {
                    continue;
                }

                foreach (NXOpen.CAM.Operation item in OperationAry)
                {
                    //取得父層的群組(回傳：NCGroup XXXX)
                    string NCProgramTag = item.GetParent(CAMSetup.View.ProgramOrder).ToString();
                    NCProgramTag = Regex.Replace(NCProgramTag, "[^0-9]", "");
                    if (NCProgramTag == ncGroup.Tag.ToString())
                    {
                        bool cheValue;
                        string OperName = "";
                        cheValue = DicNCData.TryGetValue(ncGroup.Name, out OperName);
                        if (!cheValue)
                        {
                            OperName = item.Name;
                            DicNCData.Add(ncGroup.Name, OperName);
                        }
                        else
                        {
                            OperName = OperName + "," + item.Name;
                            DicNCData[ncGroup.Name] = OperName;
                        }
                    }
                }
            }
            #endregion

            //將DicProgName的key存入程式群組下拉選單中
            comboBoxNCgroup.Items.AddRange(DicNCData.Keys.ToArray());

            #region 將控制器名稱填入SuperGridPostPanel

            GridRow row = new GridRow();
            //-----暫時使用版本(路徑指向UG)
            string TemplateRoot = string.Format(@"{0}\{1}\{2}\{3}", Environment.GetEnvironmentVariable("UGII_BASE_DIR"), "MACH", "resource", "postprocessor");
            string TemplatePostPath = string.Format(@"{0}\{1}", TemplateRoot, "template_post.dat");
            string[] TemplatePostData = System.IO.File.ReadAllLines(TemplatePostPath);
            //-----發佈使用版本(Server需有MACH檔案)
            //string[] TemplatePostData = CaxGetDatData.GetTemplatePostData();
            if (TemplatePostData.Length == 0)
            {
                CaxLog.ShowListingWindow("template_post.dat為空，請檢查！");
            }
            for (int i = 0; i < TemplatePostData.Length;i++ )
            {
                if (i>6)
                {
                    string PostName = TemplatePostData[i].Split(',')[0];
                    row = new GridRow(PostName);
                    PostPanel.Rows.Add(row);
                }
            }

            #endregion
        }

        private void comboBoxNCgroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空superGrid資料
            OperPanel.Rows.Clear();
            //取得comboBox資料
            CurrentNCGroup = comboBoxNCgroup.Text;

            SelectAll.Checked = true;

            #region (註解中)填值到SuperGridOperPanel
            /*
            GridRow row = new GridRow();
            foreach (KeyValuePair<string, string> kvp in DicNCData)
            {
                if (CurrentNCGroup != kvp.Key)
                {
                    continue;
                }

                
                string[] splitOperName = kvp.Value.Split(',');
                for (int i = 0; i < splitOperName.Length; i++)
                {
                    row = new GridRow(true, splitOperName[i],"");
                    OperPanel.Rows.Add(row);
                }
            }
            */
            #endregion
        }

        private void SelectAll_CheckedChanged(object sender, EventArgs e)
        {
            //清空superGrid資料
            OperPanel.Rows.Clear();

            #region 填值到SuperGridOperPanel

            GridRow row = new GridRow();
            if (SelectAll.Checked == false)
            {

                foreach (KeyValuePair<string, string> kvp in DicNCData)
                {
                    if (CurrentNCGroup != kvp.Key)
                    {
                        continue;
                    }

                    string[] splitOperName = kvp.Value.Split(',');
                    for (int i = 0; i < splitOperName.Length; i++)
                    {
                        row = new GridRow(false, splitOperName[i], "");
                        OperPanel.Rows.Add(row);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> kvp in DicNCData)
                {
                    if (CurrentNCGroup != kvp.Key)
                    {
                        continue;
                    }

                    string[] splitOperName = kvp.Value.Split(',');
                    for (int i = 0; i < splitOperName.Length; i++)
                    {
                        row = new GridRow(true, splitOperName[i], "");
                        OperPanel.Rows.Add(row);
                    }
                }
            }

            #endregion
            
        }

        private void SuperGridPostPanel_RowClick(object sender, GridRowClickEventArgs e)
        {
            //取得點選的RowIndex
            CurrentRowIndexofPostPanel = e.GridRow.Index; 
            CurrentSelPostName = PostPanel.GetCell(CurrentRowIndexofPostPanel, 0).Value.ToString();
        }

        private void Output_Click(object sender, EventArgs e)
        {

            if (CurrentSelPostName == "")
            {
                UI.GetUI().NXMessageBox.Show("注意", NXMessageBox.DialogType.Error, "先指定一個後處理器名稱！");
                return;
            }
            
            foreach (NXOpen.CAM.NCGroup ncGroup in NCGroupAry)
            {
                int type;
                int subtype;
                theUfSession.Obj.AskTypeAndSubtype(ncGroup.Tag, out type, out subtype);
                //判斷是否為Program群組
                if (type != UFConstants.UF_machining_task_type)
                {
                    continue;
                }

                //判斷是否已更名為OPXXX
                if (!ncGroup.Name.Contains("OP"))
                {
                    UI.GetUI().NXMessageBox.Show("注意", NXMessageBox.DialogType.Error, "請先手動將Group名稱：" + ncGroup.Name + "，改為正確格式，再重新啟動功能！");
                    this.Close();
                    return;
                }

                //選到的OP與Collection做比對
                if (CurrentNCGroup != ncGroup.Name)
                {
                    continue;
                }
                
                //記錄checkbox為true的OP(Key=index,Value=OPName)
                List<string> ListSelectOP = new List<string>();
                for (int i = 0; i < OperPanel.Rows.Count; i++)
                {
                    bool check_sel = false;
                    check_sel = (bool)OperPanel.GetCell(i, 0).Value;
                    if (check_sel)
                    {
                        ListSelectOP.Add(OperPanel.GetCell(i, 1).Value.ToString());
                    }
                }

                //取得此OP下的Operation
                CAMObject[] OperGroup = ncGroup.GetMembers();

                //開始輸出後處理
                foreach (string i in ListSelectOP)
                {
                    foreach (CAMObject y in OperGroup)
                    {
                        if (i != y.Name)
                        {
                            continue;
                        }
                        string outputpath = string.Format(@"{0}\{1}", NCFolderPath, y.Name);
                        bool checkSucess = CaxOper.CreatePost(y, CurrentSelPostName, outputpath);
                        if (!checkSucess)
                        {
                            CaxLog.ShowListingWindow("程式：" + y.Name + "可能尚未完成，導致輸出的後處理不完全");
                        }
                    }
                }
            }

        }

        private void CloseDlg_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
