using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Implement
{
    public class UserRepository : BaseRepository<User_Role>
    {
        public UserRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<User_Role> GetPageList()
        {
            List<User_Role> users = new List<User_Role>();
            using (MSSEntities db = new MSSEntities())
            {
                users = (from o in db.User_Role
                         orderby o.User_ID ascending
                         select o)
                          .ToList();

                return users;
            }

        }
    }
  
}

