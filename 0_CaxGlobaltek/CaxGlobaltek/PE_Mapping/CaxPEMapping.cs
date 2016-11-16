using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iesi.Collections.Generic;

namespace CaxGlobaltek
{
    public class Sys_Customer
    {
        public virtual Int32 customerSrNo { get; set; }
        public virtual string customerName { get; set; }
        public virtual IList<Com_PEMain> comPEMain { get; set; }
    }

    public class Sys_Operation2
    {
        public virtual Int32 operation2SrNo { get; set; }
        public virtual string operation2Name { get; set; }
        public virtual IList<Com_PartOperation> comPartOperation { get; set; }
    }

    public class Com_PEMain
    {
        public virtual Int32 peSrNo { get; set; }
        public virtual string partName { get; set; }
        public virtual string customerVer { get; set; }
        public virtual string createDate { get; set; }
        public virtual Sys_Customer sysCustomer { get; set; }
        public virtual IList<Com_PartOperation> comPartOperation { get; set; }
    }

    public class Com_PartOperation
    {
        public virtual Int32 partOperationSrNo { get; set; }
        public virtual string operation1 { get; set; }
        public virtual Com_PEMain comPEMain { get; set; }
        public virtual Sys_Operation2 sysOperation2 { get; set; }
        public virtual IList<Com_MEMain> comMEMain { get; set; }
        public virtual IList<Com_TEMain> comTEMain { get; set; }
    }
}
