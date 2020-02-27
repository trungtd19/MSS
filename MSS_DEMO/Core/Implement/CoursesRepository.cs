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
        public List<Course_Spec_Sub> GetListID()
        {
            List<Course_Spec_Sub> cour = new List<Course_Spec_Sub>();
            using (MSSEntities db = new MSSEntities())
            {
                cour = (from cou in db.Courses
                       join sp in db.Specifications on cou.Specification_ID equals sp.Specification_ID
                       join su in db.Subjects on sp.Subject_ID equals su.Subject_ID
                       select new Course_Spec_Sub { Course_ID = cou.Course_ID, Subject_ID = su.Subject_ID }).ToList();
                return cour;
            }
        }
    }
    public class Course_Spec_Sub : Course
    {
        public string Subject_ID { get; set; }
    }
 
}