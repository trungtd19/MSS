using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Implement
{
    public class StudentSpecificationLogRepository : GenericRepository<Student_Specification_Log>
    {
        public StudentSpecificationLogRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<Student_Specification_Log> GetPageList()
        {
            List<Student_Specification_Log> student = new List<Student_Specification_Log>();
            using (MSSEntities db = new MSSEntities())
            {
                student = (from o in db.Student_Specification_Log
                           orderby o.Roll descending
                           select o)
                          .ToList();

                return student;
            }

        }
    }
}