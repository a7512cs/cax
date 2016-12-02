using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using CaxGlobaltek;

namespace AddDeleteDB
{
    public class Operation2
    {
        private static ISession session = MyHibernateHelper.SessionFactory.OpenSession();
        public static bool SetOperation2Data(out List<string> listOperation2)
        {
            listOperation2 = new List<string>();
            try
            {
                IList<Sys_Operation2> sysCustomer = session.QueryOver<Sys_Operation2>().List<Sys_Operation2>();
                foreach (Sys_Operation2 i in sysCustomer)
                {
                    listOperation2.Add(i.operation2Name.ToString());
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
