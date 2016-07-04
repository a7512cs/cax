using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CimforceCaxTwFixture;
using CimforceCaxTwPublic;
using NXOpen;
using NXOpen.UF;
using Newtonsoft.Json;

namespace CaxUGforEspritt
{
    public class WeFixSystem
    {
        private static Session theSession;
        private static UI theUI;
        private static UFSession theUfSession;

        public bool status;
        public string FixtureDir = "";      //治具配置檔的路徑
        public CimforceCaxTwFixture.FixtureData classFixtureData = null;        //治具配置 Json Class
        public WeFixData sWeFixSystemData;  //治具資料
        public List<WeFixData> allowFixtureDataLstSY = null;  // 可用治具列表


        public WeFixSystem()
        {
            // 初始化所有參數
            theSession = Session.GetSession();
            theUI = UI.GetUI();
            theUfSession = UFSession.GetUFSession();

            classFixtureData = new CimforceCaxTwFixture.FixtureData();
        }

        public bool GetFixtureConfigDat()
        {
            try
            {
                // 1. 讀取網路磁碟治具目錄\\xx.xx.xx.xx\NXcustom\NX_Cimforce\Cimforce\Fixture
                FixtureDir = "";
                CaxFixture.GetFixtureDir(out FixtureDir);
                if (FixtureDir == "")
                {
                    CaxLog.ShowListingWindow("網路磁碟治具目錄 讀取失敗...");
                    return false;
                }
                if (!System.IO.Directory.Exists(FixtureDir))
                {
                    CaxLog.ShowListingWindow("路徑不存在..." + FixtureDir);
                    return false;
                }

                // 2. 讀取治具配置檔\\xx.xx.xx.xx\NXcustom\NX_Cimforce\Cimforce\Fixture\fixture_data.dat
                string fixture_data = "";
                fixture_data = string.Format(@"{0}\{1}", FixtureDir, "fixture_data.dat");
                if (!System.IO.File.Exists(fixture_data))
                {
                    CaxLog.ShowListingWindow("fixture_data取得失敗或是沒有fixture_data檔案");
                    return false;
                }

                classFixtureData = new FixtureData();
                //status = GetFixtureData(fixture_data, out classFixtureData);
               
                status = CaxFixture.ReadFixtureDataJsonData(out classFixtureData);
                if (!status || classFixtureData == null)
                {
                    string fixtureDataPath = "";
                    CaxFixture.GetFixtureDataPath(out fixtureDataPath);
                    CaxLog.ShowListingWindow("治具配置檔讀取失敗，請MES確認數據是否有傳遞...\n檔案路徑：" + fixtureDataPath);
                    return false;
                }

                // 3. 取得可用治具列表
                status = GetAllowFixture();
                if (!status)
                {
                    CaxLog.ShowListingWindow("取得可用治具列表 失敗");
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool GetAllowFixture()
        {
            allowFixtureDataLstSY = new List<WeFixData>(); 
            try
            {
                #region 落落程式
                for (int i = 0; i < classFixtureData.FIXTURES.Count; i++)
                {
                    for (int j = 0; j < classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID.Count; j++)
                    {
                        for (int k = 0; k < classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES.Count; k++)
                        {
                            // for 單一治具可用零件類型
                            for (int a = 0; a < classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].SUPPORT_TYPES.Count; a++)
                            {
                                // if (治具可用於此零件類型 or 治具可用於"其他(7)")，加入可用治具列表
                                if ((classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].SUPPORT_TYPES[a].PART_ID == CaxWE.Task_Type) ||
                                    (classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].SUPPORT_TYPES[a].PART_ID == "7"))
                                {
                                    // 取需要的治具資訊
                                    // 資訊 for superGrid
                                    WeFixData tempFixtureData = new WeFixData();
                                    tempFixtureData.comName = classFixtureData.FIXTURES[i].FIXTURE_COM_NAME;                                        //廠商
                                    tempFixtureData.modelID = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].FIXTURE_MODEL_ID;     //編碼
                                    tempFixtureData.modelName = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].FIXTURE_MODEL_NAME; //名稱
                                    tempFixtureData.length = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].LENGTH;                //長
                                    tempFixtureData.width = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].WIDTH;                  //寬
                                    tempFixtureData.height = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].HEIGHT;                //高
                                    // 檔案路徑
                                    tempFixtureData.filePath = string.Format(@"{0}\{1}", FixtureDir, classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].FIXTURE_PATH);
                                    tempFixtureData.imagePath = string.Format(@"{0}\{1}", FixtureDir, classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].FIXTURE_IMG_PATH);
                                    // 屬性值
                                    //tempFixtureData.ATTRIBUTE_FIXTURE_TYPE = string.Format(@"{0}#{1}", classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].FIXTURE_MODEL_ID, classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].FIXTURE_MODEL_NAME);
                                    // 資訊 for 判斷校正方式
                                    tempFixtureData.CLP_LENGTH = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].CLP_LENGTH;
                                    tempFixtureData.CLP_WIDTH = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].CLP_WIDTH;
                                    tempFixtureData.CLP_HEIGHT = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].CLP_HEIGHT;
                                    tempFixtureData.FIX_CLAMP_MODEL = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].FIX_CLAMP_MODEL;
                                    tempFixtureData.FIX_KG = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].FIX_KG;
                                    //FIXTURE_TYPE
                                    tempFixtureData.FIXTURE_TYPE = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].FIXTURE_TYPE;

                                    allowFixtureDataLstSY.Add(tempFixtureData);

                                    goto GOTO_NEXT_MES_FIXTURE;
                                }
                            }
                            
                        GOTO_NEXT_MES_FIXTURE:
                            continue;
                        }
                    }
                }
                #endregion

                /*
                for (int i = 0; i < classFixtureData.FIXTURES.Count; i++)
                {
                    for (int j = 0; j < classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID.Count;j++ )
                    {
                        for (int k = 0; k < classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES.Count;k++ )
                        {
                            bool chk_sameType = false;
                            for (int l = 0; l < classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].SUPPORT_TYPES.Count;l++ )
                            {
                                string Task_Type = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].SUPPORT_TYPES[l].PART_ID;
                                if (CaxWE.Task_Type == Task_Type)
                                {
                                    chk_sameType = true;
                                    break;
                                }
                            }

                            if (chk_sameType)
                            {
                                sWeFixSystemData.comName = classFixtureData.FIXTURES[i].FIXTURE_COM_NAME;
                                sWeFixSystemData.modelID = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].FIXTURE_MODEL_ID;
                                sWeFixSystemData.modelName = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].FIXTURE_MODEL_NAME;
                                sWeFixSystemData.length = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].LENGTH;
                                sWeFixSystemData.width = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].WIDTH;
                                sWeFixSystemData.height = classFixtureData.FIXTURES[i].FIXTURE_SPEC_ID[j].SUB_FIXTURES[k].HEIGHT;
                            }
                        }
                    }
                }
                */

            }
            catch (System.Exception ex)
            {
                CaxLog.ShowListingWindow(ex.ToString());
                return false;
            }
            return true;
        }

        public static bool GetFixtureData(string jsonPath, out FixtureData classFixtureData)
        {
            classFixtureData = new FixtureData();

            try
            {
                bool status;

                //判斷檔案是否存在
                if (!System.IO.File.Exists(jsonPath))
                {
                    return false;
                }
                string jsonText;
                status = ReadFileDataUTF8(jsonPath, out jsonText);
                if (!status)
                {
                    return false;
                }

                string DecJsonText = "";
                DecJsonText = CaxTransType.Decrypt(jsonText, CaxDefineParam.CIMFORCE_KEY);

                classFixtureData = JsonConvert.DeserializeObject<FixtureData>(DecJsonText);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }


        public static bool ReadFileDataUTF8(string file_path, out string allContent)
        {
            allContent = "";

            if (!System.IO.File.Exists(file_path))
            {
                return false;
            }

            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(file_path, Encoding.UTF8);

            int index = 0;
            while ((line = file.ReadLine()) != null)
            {
                if (index == 0)
                {
                    allContent += line;
                }
                else
                {
                    allContent += "\n";
                    allContent += line;
                }
                index++;
            }
            file.Close();

            return true;
        }
    }
}
