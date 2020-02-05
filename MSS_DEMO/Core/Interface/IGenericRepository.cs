using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS_DEMO.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(object id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(T obj);
        void Save();
    }
}
