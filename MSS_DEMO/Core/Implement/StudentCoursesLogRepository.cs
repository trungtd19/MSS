using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Implement
{
    public class StudentCoursesLogRepository : GenericRepository<Student_Course_Log>
    {
        public StudentCoursesLogRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<Student_Course_Log> GetPageList()
        {
            List<Student_Course_Log> student = new List<Student_Course_Log>();
            using (MSSEntities db = new MSSEntities())
            {
                student = (from o in db.Student_Course_Log
                           orderby o.Course_Log_ID descending
                           select o)
                          .ToList();

                return student;
            }

        }
    }
}