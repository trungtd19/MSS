using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Implement
{
    public class CoursesDeadlineRepository : BaseRepository<Course_Deadline>
    {
        public CoursesDeadlineRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<Cour_dealine> GetPageList()
        {
            List<Cour_dealine> cour = new List<Cour_dealine>();
            int coursesCount = 1;
            int growRow = 1;
            int growRowNo = 1;
            using (MSSEntities db = new MSSEntities())
            {
                cour = (from cd in db.Course_Deadline
                        join c in db.Courses on cd.Course_ID equals c.Course_ID
                        join sp in db.Specifications on c.Specification_ID equals sp.Specification_ID
                        join sub in db.Subjects on sp.Subject_ID equals sub.Subject_ID
                        join s in db.Semesters on cd.Semester_ID equals s.Semester_ID                       
                        into tempTable from ed in tempTable.DefaultIfEmpty()
                        select new 
                        {
                            Course_Deadline_ID = cd.Course_Deadline_ID,
                            Course_ID = cd.Course_ID,
                            Courses_Name = c.Course_Name,
                            Semester_ID = ed.Semester_ID == null ? "" : ed.Semester_ID,
                            Semester_Name = ed.Semester_Name == null ? "" : ed.Semester_Name,
                            Deadline = cd.Deadline,
                            Subject_ID = sub.Subject_ID
                        }).ToList().Select(x => new Cour_dealine
                        {
                            Course_Deadline_ID = x.Course_Deadline_ID,
                            Course_ID = x.Course_ID,
                            Courses_Name = x.Courses_Name,
                            Semester_ID = x.Semester_ID,
                            Semester_Name = x.Semester_Name,
                            deadlineString = Convert.ToDateTime(x.Deadline).ToString("dd/MM/yyyy"),
                            Subject_ID = x.Subject_ID
                        }).ToList();
                cour = (from o in cour
                        orderby o.Subject_ID descending
                        select o)
                        .ToList();
                cour[0].groupRow = growRow;
                cour[0].groupRowNo = growRowNo;
                while (coursesCount < cour.Count())
                {
                    if (cour[coursesCount - 1].Subject_ID == cour[coursesCount].Subject_ID)
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
        public bool IsExitsDeadline(Course_Deadline dl)
        {
            bool check = true;
            Course_Deadline cd = context
                                .Course_Deadline
                                .Where(x=> x.Course_ID == dl.Course_ID && x.Semester_ID == dl.Semester_ID)
                                .FirstOrDefault();
            if (cd != null)
            {
                check = true;
            }
            else
                check = false;
            return check;
        }
    }
    public class Cour_dealine : Course_Deadline
    {
        public string Subject_ID { get; set; }
        public string Semester_Name { get; set; }
        public string Courses_Name { get; set; }
        public int groupRow { get; set; }
        public int groupRowNo { get; set; }
        public string deadlineString { get; set; }
    }
}