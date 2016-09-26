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
        public virtual IList<Com_Dimension> comDimension { get; set; }
        public virtual string draftingVer { get; set; }
        public virtual string draftingdate { get; set; }
        public virtual string ballon { get; set; }
        public virtual string location { get; set; }
        public virtual string instrument { get; set; }
        public virtual string frequency { get; set; }
        public virtual string createDate { get; set; }
    }

    public class Com_Dimension
    {
        public virtual Int32 dimensionSrNo { get; set; }
        public virtual Com_MEMain comMEMain { get; set; }
        public virtual string characteristic { get; set; }
        public virtual string zoneShape { get; set; }
        public virtual string toleranceValue { get; set; }
        public virtual string materialModifer { get; set; }
        public virtual string primaryDatum { get; set; }
        public virtual string primaryMaterialModifer { get; set; }
        public virtual string secondDatum { get; set; }
        public virtual string secondMaterialModifer { get; set; }
        public virtual string tertiaryDatum { get; set; }
        public virtual string tertiaryMaterialModifer { get; set; }
        public virtual string maxTolerance { get; set; }
        public virtual string minTolerance { get; set; }
        public virtual string beforeText { get; set; }
        public virtual string mainText { get; set; }
        public virtual string toleranceSymbol { get; set; }
        public virtual string upTolerance { get; set; }
        public virtual string lowTolerance { get; set; }
    }
}
