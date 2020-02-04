using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Repository
{
    public class StudentRepository : GenericRepository<Student>
    {
        public StudentRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<Student> GetPageList()
        {
            List<Student> student = new List<Student>();
            using (MSSEntities db = new MSSEntities())
            {
                student = (from o in db.Students
                          orderby o.Roll descending
                          select o)                      
                          .ToList();

                return student;
            }

        }
    }
}