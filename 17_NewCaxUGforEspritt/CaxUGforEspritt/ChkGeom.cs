using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NXOpen;
using CimforceCaxTwPublic;
using NXOpen.Utilities;

namespace CaxUGforEspritt
{
    public partial class ChkGeom : DevComponents.DotNetBar.Office2007Form
    {
        public ChkGeom()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonNO_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }


    }
}
