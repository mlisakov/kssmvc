using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;
using NLog;

namespace KSS.DBConnector
{
    class SessionEngine
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        //[ThreadStatic]
        private static object _lockObject = new object();
        private static ISessionFactory sessionFactory;
        private static Configuration _configuration;

        public static ISession Session
        {
            get
            {
                if (sessionFactory == null)
                {
                    Init();
                }
                return sessionFactory.OpenSession();
            }
        }

        public static async Task<ISession> GetSession()
        {
            if (sessionFactory == null)
            {
                await Task.Run(() => Init());
            }
            return sessionFactory.OpenSession();
        }

        private static void Init()
        {
            try
            {
                lock (_lockObject)
                {
                    if (sessionFactory == null)
                    {
                        _configuration = new Configuration();
                        Dictionary<string, string> properties = new Dictionary<string, string>();
                        properties.Add(NHibernate.Cfg.Environment.ConnectionProvider,"NHibernate.Connection.DriverConnectionProvider");
                        properties.Add(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.SqlClientDriver");
                        properties.Add(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.MsSql2008Dialect");

                        properties.Add(NHibernate.Cfg.Environment.ConnectionString, "Server=KRAKOTSPC;Initial Catalog=Company_v2.0;User ID=QueryBase;Password=KSSAdmin");
                        //properties.Add(NHibernate.Cfg.Environment.ConnectionString, "Server=EPRUPETW0475;Initial Catalog=Company_v2.0;Integrated Security = true;");

                        _configuration.SetProperties(properties);
                        _configuration.AddAssembly("KSS.DBConnector");
                        sessionFactory = _configuration.BuildSessionFactory();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка инициализации NHibernate", ex);
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw new Exception("Ошибка доступа к Базе данных \"КСС\"." + ex.Message);
            }
        }
    }
}
