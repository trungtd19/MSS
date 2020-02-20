using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Components
{
    public class ClassRepository : GenericRepository<Class>
    {
        public ClassRepository(MSSEntities context)
           : base(context)
        {
        }
        public bool CheckExitsClass(string id)
        {
            bool check = true;
            Class _class = context.Classes.Where(x => x.Class_ID == id).FirstOrDefault();
            if (_class == null)
            {
                check = false;
                throw new Exception("Lớp " + id + " không tồn tại!");               
            }
            else
                check = true;
            return check;
        }
    }

}