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
            using (MSSEntities db = new MSSEntities())
            {
                cour = (from cd in db.Course_Deadline
                        join c in db.Courses on cd.Course_ID equals c.Course_ID
                        join s in db.Semesters on cd.Semester_ID equals s.Semester_ID
                        into tempTable from ed in tempTable.DefaultIfEmpty()
                        select new Cour_dealine
                        {
                            Course_Deadline_ID = cd.Course_Deadline_ID,
                            Course_ID = cd.Course_ID,
                            Courses_Name = c.Course_Name,
                            Semester_ID = ed.Semester_ID == null ? "" : ed.Semester_ID,
                            Semester_Name = ed.Semester_Name == null ? "" : ed.Semester_Name,
                            Deadline = cd.Deadline
                        })
                        .ToList();
                cour = (from o in cour
                        orderby o.Course_ID descending
                        select o)
                        .ToList();
                return cour;
            }

        }
    }
    public class Cour_dealine : Course_Deadline
    {
        public string Semester_Name { get; set; }
        public string Courses_Name { get; set; }
    }
}