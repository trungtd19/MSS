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
        public List<Cour_dealine> GetPageList()
        {
            List<Cour_dealine> cour = new List<Cour_dealine>();
            using (MSSEntities db = new MSSEntities())
            {
                cour = (from cou in db.Courses
                        join dl in db.Course_Deadline on cou.Course_ID equals dl.Course_ID
                        select new Cour_dealine { Course_ID = cou.Course_ID, Deadline = dl.Deadline, Semester_ID = dl.Semester_ID, Course_Name = cou.Course_Name, Specification_ID = cou.Specification_ID }).ToList();
                cour = (from o in cour
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
    public class Cour_dealine : Course
    {
        public string Semester_ID { get; set; }
        public DateTime Deadline { get; set; }
    }
}