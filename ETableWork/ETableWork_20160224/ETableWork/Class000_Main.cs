using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using System.Windows.Forms;
using CimforceCaxTwPublic;
using NXOpenUI;

namespace ETableWork
{
    class Class000_Main
    {
        //user define
        private const string SUPPORT_MACHINE = "";
        private const string MACHINE_TYPE = "";
        private const string CONTROLLER = "";
        public static string METHOD_NORMAL = "NORMAL";
        public static string METHOD_AUTOCAM = "AUTOCAM";

        public static void DoEverything(string method)
        {
            try
            {
                // 取得初始化資訊
                Class001_ETableInit eTableWorkInit = new Class001_ETableInit();
                int errorNo = eTableWorkInit.GetAllParameter();
                if (errorNo != 0)
                {
                    if (errorNo != -1)
                    {
                        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, "取得初始化資訊發生錯誤...");
                    }
                    return;
                }

                // 將所有資訊傳入UI
                Application.EnableVisualStyles();

                eTbale eworkTableDlg = new eTbale();
                eworkTableDlg.labelXWorkSection.Text = eTableWorkInit.section_face;
                eworkTableDlg.asmName = eTableWorkInit.asmName;
                eworkTableDlg.ROOT_PATH = eTableWorkInit.rootPath;
                eworkTableDlg.sMesDatData = eTableWorkInit.sMesDatData;
                eworkTableDlg.labelXFixture.Text = eTableWorkInit.fixture_type;
                eworkTableDlg.sCimAsmCompPart = eTableWorkInit.sCimAsmCompPart;
                eworkTableDlg.maxDesignBodyWcs = eTableWorkInit.maxDesignBodyWcs;
                eworkTableDlg.minDesignBodyWcs = eTableWorkInit.minDesignBodyWcs;
                eworkTableDlg.sBaseFaces = eTableWorkInit.sBaseFaces;

                eworkTableDlg.support_machine = SUPPORT_MACHINE;
                eworkTableDlg.machine_type = MACHINE_TYPE;
                eworkTableDlg.controller = CONTROLLER;
                eworkTableDlg.machine_group = eTableWorkInit.sMesDatData.MAC_POST_TYPE;
                eworkTableDlg.postFunction = eTableWorkInit.postFunctionName;
                eworkTableDlg.sExportWorkTabel = eTableWorkInit.sExportWorkTabel;
                eworkTableDlg.BOTTOM_Z = eTableWorkInit.minDesignBodyWcs[2];
                eworkTableDlg.section_face = eTableWorkInit.section_face;
                eworkTableDlg.elecPartNoNote = eTableWorkInit.elecPartNoNote;
                // 20150525 Stewart
                eworkTableDlg.baseHoleName = eTableWorkInit.baseHoleName;
                eworkTableDlg.beforeCNCName = eTableWorkInit.beforeCNCName;
                eworkTableDlg.afterCNCName = eTableWorkInit.afterCNCName;
                eworkTableDlg.isMultiFixture = eTableWorkInit.isMultiFixture;
                eworkTableDlg.fixtureLst = eTableWorkInit.fixtureLst;
                eworkTableDlg.subDesignCompLst = eTableWorkInit.subDesignCompLst;
                // 20150721 安全高度
                eworkTableDlg.CLEARANE_PLANE = eTableWorkInit.CLEARANE_PLANE;
                // 20150817 裝夾圖上的基準面距離
                eworkTableDlg.sBaseDist = eTableWorkInit.sBaseDist;
                // 20151013 判斷是否重新裝夾/校正之參數
                eworkTableDlg.clampCalibParam = eTableWorkInit.clampCalibParam;

                // 傳入配置檔資訊
                eworkTableDlg.companyName = eTableWorkInit.config.companyName;
                eworkTableDlg.hasCMM = (eTableWorkInit.config.hasCMM == "1");
                eworkTableDlg.hasMultiFixture = eTableWorkInit.hasMultiFixture;

                eworkTableDlg.ListToolLengehAry = new List<ListToolLengeh>();
                eworkTableDlg.ListToolLengehAry.AddRange(eTableWorkInit.ListToolLengehAry);

                if (method == METHOD_NORMAL)
                {
                    //開啟對話框
                    FormUtilities.ReparentForm(eworkTableDlg);
                    System.Windows.Forms.Application.Run(eworkTableDlg);
                    eworkTableDlg.Dispose();
                }
                else if (method == METHOD_AUTOCAM)
                {
                    eworkTableDlg.buttonXOK_Click(null, null);
                }

            
            }
            catch (System.Exception ex)
            {
                return;
            }
            return;



        }
    }
}
