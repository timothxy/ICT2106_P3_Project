using System.Collections.Generic;
using YouthActionDotNet.Models;

namespace YouthActionDotNet.DAL
{
    public interface IGenericDataRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetByID(object id);
        bool Insert(T obj);
        bool Update(T obj);
        IEnumerable<T> UpdateAndFetch(T ojb);
        bool Delete(object id);
        Settings GetSettings();
        void Save();
    }
}