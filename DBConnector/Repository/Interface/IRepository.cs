namespace KSS.DBConnector
{
    internal interface IRepository<T>
    {
        void Add(T item);
        void Update(T item);
        void Delete(T item);
        T GetById(long itemGuid);
        T GetByName(string name, string field = "Name");
    }
}
