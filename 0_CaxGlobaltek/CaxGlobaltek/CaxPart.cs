using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using NXOpen.UF;
using System.Collections;
using NXOpen.Utilities;
using System.Windows.Forms;

namespace CaxGlobaltek
{
    public class CaxPart
    {
        public static bool Save(BasePart.SaveComponents saveComponentParts = NXOpen.BasePart.SaveComponents.True, BasePart.CloseAfterSave close = NXOpen.BasePart.CloseAfterSave.False)
        {
            try
            {
                Session theSession = Session.GetSession();
                //Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                // ----------------------------------------------
                //   Menu: File->Save
                // ----------------------------------------------
                PartSaveStatus partSaveStatus1;
                partSaveStatus1 = displayPart.Save(saveComponentParts, close);
                partSaveStatus1.Dispose();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }

            return true;
        }

        public static bool OpenBaseDisplay(string partPath, out BasePart newPart)
        {
            newPart = null;

            try
            {
                Session theSession = Session.GetSession();
                PartLoadStatus partLoadStatus;
                newPart = theSession.Parts.OpenBaseDisplay(partPath, out partLoadStatus);
                partLoadStatus.Dispose();
            }
            catch (System.Exception ex)
            {
                //theSession.ListingWindow.Open();
                //theSession.ListingWindow.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
    }
}
