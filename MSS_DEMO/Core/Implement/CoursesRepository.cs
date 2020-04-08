using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Components
{
    public class CoursesRepository : BaseRepository<Course>
    {
        public CoursesRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<Cours_Spec> GetPageList()
        {
            List<Cours_Spec> cour = new List<Cours_Spec>();
            int coursesCount = 1;
            int growRow = 1;
            int growRowNo = 1;
            using (MSSEntities db = new MSSEntities())
            {
                cour = (from cou in db.Courses
                        join dl in db.Specifications on cou.Specification_ID equals dl.Specification_ID into empSpec
                        from ed in empSpec.DefaultIfEmpty()
                        select new Cours_Spec { Course_ID = cou.Course_ID, Course_Name = cou.Course_Name, Specification_ID = cou.Specification_ID, Specification_Name = ed.Specification_Name == null ? "Not Map" : ed.Specification_Name }).ToList();
                cour = (from o in cour
                        orderby o.Specification_ID descending
                        select o)
                        .ToList();
               cour[0].groupRow = growRow;
               cour[0].groupRowNo = growRowNo;
               while (coursesCount < cour.Count())
                {
                    if (cour[coursesCount - 1].Specification_ID == cour[coursesCount].Specification_ID)
                    {
                        cour[coursesCount].groupRow = growRow;
                        growRowNo++;
                        cour[coursesCount].groupRowNo = growRowNo;
                    }
                    else
                    {
                        growRow++;
                        growRowNo = 1;
                        cour[coursesCount].groupRow = growRow;
                        cour[coursesCount].groupRowNo = growRowNo;
                    }
                    coursesCount++;
                }
                return cour;
            }

        }

        public Cours_Spec GetListByID(int id)
        {
            return GetPageList().Where(s => s.Course_ID == id).FirstOrDefault();
        }

        public List<Course_Spec_Sub> GetList()
        {
            List<Course_Spec_Sub> cour = new List<Course_Spec_Sub>();
            using (MSSEntities db = new MSSEntities())
            {
                cour = (from cou in db.Courses
                       join sp in db.Specifications on cou.Specification_ID equals sp.Specification_ID
                       join su in db.Subjects on sp.Subject_ID equals su.Subject_ID
                       select new Course_Spec_Sub { Course_ID = cou.Course_ID, Subject_ID = su.Subject_ID, Course_Name = cou.Course_Name}).ToList();
                return cour;
            }
        }
        public bool IsExitsCourse(int? spec_ID, string course_Name)
        {
            bool check = true;
            Course student = context.Courses.Where(x => x.Specification_ID == spec_ID && x.Course_Name == course_Name).FirstOrDefault();
            if (student != null)
            {
                check = true;
            }
            else
                check = false;
            return check;
        }
    }
    public class Course_Spec_Sub : Course
    {
        public string Subject_ID { get; set; }
    }
    public class Cours_Spec : Course
    {
        public string Specification_Name { get; set; }
        public int groupRow { get; set; } 
        public int groupRowNo { get; set; }

    }
}