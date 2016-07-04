using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace CaxGlobaltek
{
    public class CaxGetDatData
    {
        /// <summary>
        /// 取得METEDownload_Upload.dat資料
        /// </summary>
        /// <param name="cMETE_Download_Upload_Path"></param>
        /// <returns></returns>
        public static bool GetMETEDownload_Upload(out METE_Download_Upload_Path cMETE_Download_Upload_Path)
        {
            cMETE_Download_Upload_Path = new METE_Download_Upload_Path();
            try
            {
                string METEDownload_Upload_dat = "METEDownload_Upload.dat";
                string METEDownload_Upload_Path = string.Format(@"{0}\{1}", CaxEnv.GetGlobaltekEnvDir(), METEDownload_Upload_dat);
                CaxPublic.ReadMETEDownloadUpload_Path(METEDownload_Upload_Path, out cMETE_Download_Upload_Path);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 取得METEDownloadData.dat資料
        /// </summary>
        /// <param name="cMETEDownloadData"></param>
        /// <returns></returns>
        public static bool GetMETEDownloadData(out METEDownloadData cMETEDownloadData)
        {
            cMETEDownloadData = new METEDownloadData();
            try
            {
                string METEDownloadDat_dat = "METEDownloadData.dat";
                string METEDownloadDatPath = string.Format(@"{0}\{1}", CaxEnv.GetGlobaltekTaskDir(), METEDownloadDat_dat);
                CaxPublic.ReadMETEDownloadData(METEDownloadDatPath, out cMETEDownloadData);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 取得template_post.dat內容
        /// </summary>
        /// <returns></returns>
        public static string[] GetTemplatePostData()
        {
            string[] DatData = new string[]{};
            try
            {
                string TemplatePostPath = string.Format(@"{0}\{1}", CaxEnv.GetGlobaltekPostProcessorDir(), "template_post.dat");
                return DatData = System.IO.File.ReadAllLines(TemplatePostPath);
            }
            catch (System.Exception ex)
            {
                return DatData;
            }
        }

        /// <summary>
        /// 取得ControlerConfig.dat資料  (路徑：IP:Globaltek\TE_Config\ControlerConfig.dat)
        /// </summary>
        /// <param name="cControlerConfig"></param>
        /// <returns></returns>
        public static bool GetControlerConfigData(out ControlerConfig cControlerConfig)
        {
            cControlerConfig = new ControlerConfig();
            try
            {
                string ControlerConfig_dat = "ControlerConfig.dat";
                string ControlerConfig_Path = string.Format(@"{0}\{1}\{2}", CaxEnv.GetGlobaltekEnvDir(), "TE_Config", ControlerConfig_dat);
                if (!System.IO.File.Exists(ControlerConfig_Path))
                {
                    MessageBox.Show("路徑：" + ControlerConfig_Path + "不存在");
                    return false;
                }

                CaxPublic.ReadControlerConfig(ControlerConfig_Path, out cControlerConfig);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
