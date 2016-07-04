using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CaxUGforEspritt
{
    public partial class PingType : DevComponents.DotNetBar.Office2007Form
    {
        public PingType()
        {
            InitializeComponent();
        }

        private void checkBoxX1_CheckedChanged(object sender, EventArgs e)
        {
            StringPingType = "1";
        }

        private void checkBoxX2_CheckedChanged(object sender, EventArgs e)
        {
            StringPingType = "2";
        }

        private void checkBoxX3_CheckedChanged(object sender, EventArgs e)
        {
            StringPingType = "3";
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
