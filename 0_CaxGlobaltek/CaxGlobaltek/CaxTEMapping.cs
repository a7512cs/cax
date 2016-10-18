using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaxGlobaltek
{
    public class Sys_TEExcel
    {
        public virtual Int32 teExcelSrNo { get; set; }
        public virtual string teExcelType { get; set; }
        public virtual IList<Com_MEMain> comTEMain { get; set; }
    }

    public class Com_TEMain
    {
        public virtual Int32 teSrNo { get; set; }
        public virtual Com_PartOperation comPartOperation { get; set; }
        public virtual Sys_TEExcel sysTEExcel { get; set; }
        public virtual IList<Com_ShopDoc> comShopDoc { get; set; }
        public virtual string ncGroupName { get; set; }
        public virtual string totalCuttingTime { get; set; }
        public virtual string fixtureImgPath { get; set; }
        public virtual string createDate { get; set; }
    }

    public class Com_ShopDoc
    {
        public virtual Int32 shopDocSrNo { get; set; }
        public virtual Com_TEMain comTEMain { get; set; }
        public virtual string toolNo { get; set; }
        public virtual string toolID { get; set; }
        public virtual string operationName { get; set; }
        public virtual string holderID { get; set; }
        public virtual string feed { get; set; }
        public virtual string speed { get; set; }
        public virtual string machiningtime { get; set; }
        public virtual string opImagePath { get; set; }
        public virtual string partStock { get; set; }
        
    }
}
