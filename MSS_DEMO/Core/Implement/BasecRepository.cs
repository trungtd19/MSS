using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MSS_DEMO.Models;
namespace MSS_DEMO.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        internal MSSEntities context;
        internal DbSet<T> dbSet;

        public BaseRepository(MSSEntities context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }
  
        public List<T> GetAll()
        {
            return dbSet.ToList();
        }
        public T GetById(object id)
        {
            return dbSet.Find(id);
        }
        public void Insert(T obj)
        {
            dbSet.Add(obj);
        }
        public void Update(T obj)
        {
            dbSet.Attach(obj);
            context.Entry(obj).State = EntityState.Modified;
        }
        public void Delete(T obj)
        {
            dbSet.Remove(obj);
        }
    }
}