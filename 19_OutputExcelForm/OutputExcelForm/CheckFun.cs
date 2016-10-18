using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OutputExcelForm
{
    public class CheckFun
    {
        public static bool status;
        public static bool CheckAll()
        {
            try
            {
                //檢查是否有選擇
                status = Is_Select();
                if (!status)
                {
                    return false;
                }

                //檢查是否有選擇Excel格式
                status = Is_SelectForm();
                if (!status)
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool Is_Select()
        {
            try
            {
                int count = 0;
                for (int i = 0; i < OutputForm.panel.Rows.Count; i++)
                {
                    if (((bool)OutputForm.panel.GetCell(i, 0).Value) == false)
                    {
                        count++;
                    }
                }
                if (count == OutputForm.panel.Rows.Count)
                {
                    MessageBox.Show("尚未選擇欲輸出的Excel表單");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool Is_SelectForm()
        {
            try
            {
                for (int i = 0; i < OutputForm.panel.Rows.Count;i++ )
                {
                    if (((bool)OutputForm.panel.GetCell(i, 0).Value) == false)
                    {
                        continue;
                    }
                    if (OutputForm.panel.GetCell(i, 2).Value == "" || OutputForm.panel.GetCell(i, 2).Value == "雙擊此區選擇表單")
                    {
                        MessageBox.Show("表單 " + OutputForm.panel.GetCell(i, 1).Value.ToString() + " 尚未選擇Excel格式");
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
