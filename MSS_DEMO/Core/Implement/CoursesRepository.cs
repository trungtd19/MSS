using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Components
{
    public class CoursesRepository : GenericRepository<Course>
    {
        public CoursesRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<Course> GetPageList()
        {
            List<Course> cour = new List<Course>();
            using (MSSEntities db = new MSSEntities())
            {
                cour = (from o in db.Courses
                           orderby o.Course_ID descending
                           select o)
                          .ToList();

                return cour;
            }

        }
    }
 
}