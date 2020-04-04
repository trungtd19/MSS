using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Implement
{
    public class MentorRepository : BaseRepository<Mentor>
    {
        public MentorRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<Courses_Class_Mentor> getList(string userMentor)
        {
            var mentor = (from a in context.Mentors
                          join b in context.Classes on a.Mentor_ID equals b.Mentor_ID
                          join c in context.Class_Student on b.Class_ID equals c.Class_ID
                          join d in context.Students on c.Roll equals d.Roll
                          join e in context.Subject_Student  on d.Roll equals e.Roll
                          join f in context.Subjects on e.Subject_ID equals f.Subject_ID
                          join g in context.Specifications on f.Subject_ID equals g.Subject_ID
                          join h in context.Courses on g.Specification_ID equals h.Specification_ID
                          where (a.Email == userMentor)
                          select new 
                       {
                           Subject_ID = f.Subject_ID,
                           Courses_ID = h.Course_ID,
                           Course_Name = h.Course_Name,
                           Class_ID = b.Class_ID
                       }).ToList().Select(x => new Courses_Class_Mentor
                       {   CoursesID_ClassID = x.Courses_ID.ToString() + "^" + x.Class_ID,
                           Subject_ID = x.Subject_ID,
                           Course_Name = x.Course_Name,
                           Class_ID = x.Class_ID
                       })
                         .ToList();
            return mentor.Distinct(new ListComparer()).ToList(); ;
        }
        public List<Student_Course_Log> getReport(string CoursesID_ClassID)
        {
            int Courses_ID = int.Parse(CoursesID_ClassID.Split('^')[0]);
            string Class_ID = CoursesID_ClassID.Split('^')[1];
            var maxDate = context.Student_Course_Log.OrderByDescending(o => o.Date_Import).FirstOrDefault().Date_Import;
            var list = (from a in context.Student_Course_Log
                        join b in context.Courses on a.Course_ID equals b.Course_ID
                        join c in context.Students on a.Roll equals c.Roll
                        join d in context.Class_Student on c.Roll equals d.Roll
                        join e in context.Classes on d.Class_ID equals e.Class_ID
                        where ((b.Course_ID == Courses_ID) && (e.Class_ID == Class_ID) && (a.Date_Import == maxDate))
                        select a)
                        .ToList();
            return list;
        }
    }
    public class Courses_Class_Mentor
    {
        public string Subject_ID { get; set; }
        public string CoursesID_ClassID { get; set; }
        public string Course_Name { get; set; }
        public string Class_ID { get; set; }
    }
    class ListComparer : IEqualityComparer<Courses_Class_Mentor>
    {
        public bool Equals(Courses_Class_Mentor x, Courses_Class_Mentor y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Class_ID == y.Class_ID && x.Course_Name == y.Course_Name;
        }
        public int GetHashCode(Courses_Class_Mentor product)
        {
            if (Object.ReferenceEquals(product, null)) return 0;
            int hashProductName = product.Course_Name == null ? 0 : product.Course_Name.GetHashCode();
            int hashProductCode = product.Class_ID.GetHashCode();
            return hashProductName ^ hashProductCode;
        }
    }
}