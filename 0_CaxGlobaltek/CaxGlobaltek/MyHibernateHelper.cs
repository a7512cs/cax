using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using System.Reflection;

namespace CaxGlobaltek
{
    public class MyHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        public static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    try
                    {
                        Configuration configuration = new Configuration();
                        //configuration.Configure();//預設抓目錄下的hibernate.cfg.xml
                        //或者給絕對路徑↓
                        //string xmppath = string.Format(@"{0}\{1}", "D:\\GlobalCode\\0_CaxGlobaltek\\CaxGlobaltek", "hibernate.cfg.xml");
                        string xmppath = string.Format(@"{0}\{1}", "\\\\192.168.31.55\\cax", "hibernate.cfg.xml");
                        
                        configuration.Configure(xmppath);
                        //configuration.AddAssembly(Assembly.GetExecutingAssembly());
                        _sessionFactory = configuration.BuildSessionFactory();
                    }
                    catch (System.Exception ex)
                    {
                        CaxLog.ShowListingWindow(ex.ToString());
                    }

                }
                return _sessionFactory;
            }

        }
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
