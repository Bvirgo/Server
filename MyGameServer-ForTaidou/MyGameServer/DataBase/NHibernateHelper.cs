using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;

namespace MyGameServer.DataBase
{
    public class NHibernateHelper
    {
        private static ISessionFactory sessionFactory = null;//单利模式

        private static void InitializeSessionFactory()
        {
            sessionFactory =
                Fluently.Configure().Database(MySQLConfiguration.Standard.ConnectionString(db => db.Server("localhost").Database("mygame").Username("root").Password("root")))
                    .Mappings(x => x.FluentMappings.AddFromAssemblyOf<NHibernateHelper>()).BuildSessionFactory();
        }

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (sessionFactory == null)
                    InitializeSessionFactory();
                return sessionFactory;
            }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
