using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AddDeleteDB
{
    public class CommonFun
    {
        public static bool CheckData(string AddText, List<string> DBdata)
        {
            try
            {
                if (AddText == "")
                {
                    return false;
                }
                foreach (string i in DBdata)
                {
                    if (i.ToUpper() == AddText.ToUpper())
                    {
                        MessageBox.Show("已存在此客戶名稱");
                        return false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
