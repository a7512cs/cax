using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaxGlobaltek
{
    public class Com_MEMain
    {
        public virtual Int32 meSrNo { get; set; }
        public virtual Com_PartOperation comPartOperation { get; set; }
        public virtual Sys_MEExcel sysMEExcel { get; set; }
        public virtual IList<Com_Dimension> comDimension { get; set; }
        public virtual string partDescription { get; set; }
        public virtual string createDate { get; set; }
        public virtual string material { get; set; }
        public virtual string draftingVer { get; set; }
    }

    public class Com_Dimension
    {
        public virtual Int32 dimensionSrNo { get; set; }
        public virtual Com_MEMain comMEMain { get; set; }

        //Type=FcfData
        public virtual string characteristic { get; set; }
        public virtual string zoneShape { get; set; }
        public virtual string toleranceValue { get; set; }
        public virtual string materialModifer { get; set; }
        public virtual string primaryDatum { get; set; }
        public virtual string primaryMaterialModifier { get; set; }
        public virtual string secondaryDatum { get; set; }
        public virtual string secondaryMaterialModifier { get; set; }
        public virtual string tertiaryDatum { get; set; }
        public virtual string tertiaryMaterialModifier { get; set; }


        public virtual string dimensionType { get; set; }
        public virtual string beforeText { get; set; }
        public virtual string mainText { get; set; }
        public virtual string toleranceSymbol { get; set; }
        public virtual string upTolerance { get; set; }
        public virtual string lowTolerance { get; set; }
        public virtual string draftingVer { get; set; }
        public virtual string draftingDate { get; set; }
        public virtual string ballon { get; set; }
        public virtual string location { get; set; }
        public virtual string instrument { get; set; }
        public virtual string frequency { get; set; }
        public virtual string maxTolerance { get; set; }
        public virtual string minTolerance { get; set; }
        public virtual string toleranceType { get; set; }
    }

    public class Sys_MEExcel
    {
        public virtual Int32 meExcelSrNo { get; set; }
        public virtual string meExcelType { get; set; }
        public virtual IList<Com_MEMain> comMEMain { get; set; }
    }
}
