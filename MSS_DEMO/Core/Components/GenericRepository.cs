using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MSS_DEMO.Models;

namespace MSS_DEMO.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        internal MSSEntities context;
        internal DbSet<T> dbSet;

        public GenericRepository(MSSEntities context)
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
        public void Delete(object id)
        {
            T existing = dbSet.Find(id);
            dbSet.Remove(existing);
        }
        public void Save()
        {
            context.SaveChanges();
        }

    }
}