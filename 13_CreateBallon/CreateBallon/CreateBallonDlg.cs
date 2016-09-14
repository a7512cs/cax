using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using NXOpen;

namespace CreateBallon
{
    public partial class CreateBallonDlg : Office2007Form
    {
        public CreateBallonDlg()
        {
            InitializeComponent();
        }

        private void chb_keepOrigination_CheckedChanged(object sender, EventArgs e)
        {
            if (chb_keepOrigination.Checked == true)
            {
                chb_Regeneration.Checked = false;
            }
        }

        private void chb_Regeneration_CheckedChanged(object sender, EventArgs e)
        {
            if (chb_Regeneration.Checked == true)
            {
                chb_keepOrigination.Checked = false;
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (chb_keepOrigination.Checked == false & chb_Regeneration.Checked == false)
            {
                this.Hide();
                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Information, "請先選擇一個選項");
                this.Show();
                return;
            }
            if (chb_keepOrigination.Checked == true)
            {
                Is_Keep = true;
            }
            if (chb_Regeneration.Checked == true)
            {
                Is_Keep = false;
            }
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

    }
}
