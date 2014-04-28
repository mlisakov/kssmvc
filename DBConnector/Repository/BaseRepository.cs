using System.Collections.Generic;
using System.Linq;
using KSS.DBConnector.Models;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace KSS.DBConnector
{
    public class BaseRepository<T> : IRepository<T>
    {
        public void Add(T item)
        {
            using (ISession session = SessionEngine.Session)
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(item);
                transaction.Commit();
            }
        }

        public void Update(T item)
        {
            using (ISession session = SessionEngine.Session)
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Update(item);
                transaction.Commit();
            }
        }

        public void Delete(T item)
        {
            using (ISession session = SessionEngine.Session)
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Delete(item);
                transaction.Commit();
            }
        }

        public IList<T> GetAll()
        {
            using (ISession session = SessionEngine.Session)
            {
                return session.Query<T>().ToList();
            }
        }

        public T GetById(long itemGuid)
        {
            using (ISession session = SessionEngine.Session)
                return session.Get<T>(itemGuid);
        }

        public T GetByName(string name, string field = "Name")
        {
            using (ISession session = SessionEngine.Session)
            {
                T product = session
                    .CreateCriteria(typeof(T))
                    .Add(Restrictions.Eq(field, name))
                    .List<T>().FirstOrDefault();
                return product;
            }
        }

        public bool IsExists(string name, string field = "Name")
        {
            using (ISession session = SessionEngine.Session)
            {
                T product = session
                    .CreateCriteria(typeof(T))
                    .Add(Restrictions.Eq(field, name))
                    .List<T>().FirstOrDefault();
                return product != null;
            }
        }
    }
}
