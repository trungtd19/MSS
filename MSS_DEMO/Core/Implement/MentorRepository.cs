using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using MSS_DEMO.ServiceReference;
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
        public List<MentorObject> getListSubjectClass(string userMentor)
        {
            MSSWSSoapClient soap = new MSSWSSoapClient();
            var listSubjectID = context.Subjects.Where(o => o.Subject_Active == true).Select(o => o.Subject_ID).ToList();
            ArrayOfString arraySubject = new ArrayOfString();
            arraySubject.AddRange(listSubjectID);
            string jsonData = soap.GetMentor(userMentor, arraySubject);
            List<MentorObject> objectMentor = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MentorObject>>(jsonData);
            var list = (from a in objectMentor
                        join b in context.Subjects on a.Subject_ID equals b.Subject_ID
                     select new
                     {
                         Subject_ID = a.Subject_ID,
                         Subject_Name = b.Subject_Name,
                         Class_ID = a.Class_ID,
                         id = a.Subject_ID + "^" + a.Class_ID
                     }).ToList()
                     .Select(o => new MentorObject
                     {
                         Subject_ID = o.Subject_ID,
                         Subject_Name = o.Subject_Name,
                         Class_ID = o.Class_ID,
                         id = o.id
                     }).ToList();
            return list;
        }
        public List<CourseSubject> getListCourseSubject(string SubjectID)
        {
            var list = (from a in context.Courses 
                        join b in context.Specifications on a.Specification_ID equals b.Specification_ID
                        join c in context.Subjects on b.Subject_ID equals c.Subject_ID
                        where (c.Subject_Active == true && c.Subject_ID.Trim() == SubjectID.Trim())
                        select new {
                            Course_Name = a.Course_Name,
                            Subject_ID = c.Subject_ID,
                            Subject_Name = c.Subject_Name
                        }).ToList()
                          .Select(x => new CourseSubject
                          {
                              Course_Name = x.Course_Name,
                              Subject_ID = x.Subject_ID,
                              Subject_Name = x.Subject_Name
                          })
                       .ToList();

            return list;
        }
        public List<Courses_Class_Mentor> getListCourses(string userMentor)
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
                       })
                       .ToList()
                       .Select(x => new Courses_Class_Mentor
                       {   CoursesID_ClassID = x.Courses_ID.ToString() + "^" + x.Class_ID,
                           Subject_ID = x.Subject_ID,
                           Course_Name = x.Course_Name,
                           Class_ID = x.Class_ID
                       })
                       .ToList();
            return mentor.Distinct(new ListComparer()).ToList();
        }
        public List<Spec_Class_Mentor> getListSpec(string userMentor)
        {
            var mentor = (from a in context.Mentors
                          join b in context.Classes on a.Mentor_ID equals b.Mentor_ID
                          join c in context.Class_Student on b.Class_ID equals c.Class_ID
                          join d in context.Students on c.Roll equals d.Roll
                          join e in context.Subject_Student on d.Roll equals e.Roll
                          join f in context.Subjects on e.Subject_ID equals f.Subject_ID
                          join g in context.Specifications on f.Subject_ID equals g.Subject_ID
                          where (a.Email == userMentor)
                          select new
                          {
                              Subject_ID = f.Subject_ID,
                              Specification_ID = g.Specification_ID,
                              Specification_Name = g.Specification_Name,
                              Class_ID = b.Class_ID
                          })
                       .ToList()
                       .Select(x => new Spec_Class_Mentor
                       {
                           SpecID_ClassID = x.Specification_ID.ToString() + "^" + x.Class_ID,
                           Subject_ID = x.Subject_ID,
                           Specification_Name = x.Specification_Name,
                           Class_ID = x.Class_ID
                       })
                       .ToList();
            return mentor.Distinct(new ListSpecComparer()).ToList();
        }
        public List<Student_Course_Log> getReport(string CoursesID_ClassID)
        {
            int Courses_ID = int.Parse(CoursesID_ClassID.Split('^')[0]);
            string Class_ID = CoursesID_ClassID.Split('^')[1];            
            var list = (from a in context.Student_Course_Log
                        join b in context.Courses on a.Course_ID equals b.Course_ID
                        join c in context.Students on a.Roll equals c.Roll
                        join d in context.Class_Student on c.Roll equals d.Roll
                        join e in context.Classes on d.Class_ID equals e.Class_ID
                        where ((b.Course_ID == Courses_ID) && (e.Class_ID == Class_ID))
                        select a)
                        .ToList();
            if (list.Count() > 0)
            {
                var maxDate = list.OrderByDescending(o => o.Date_Import).FirstOrDefault().Date_Import;
                list = list.Where(o => o.Date_Import == maxDate).ToList();
            }            
            return list;
        }
        public List<Student_Specification_Log> getReportSpec(string SpecID_ClassID)
        {
            int SpecID = int.Parse(SpecID_ClassID.Split('^')[0]);
            string Class_ID = SpecID_ClassID.Split('^')[1];
            var list = (from a in context.Student_Specification_Log
                        join b in context.Specifications on a.Specification_ID equals b.Specification_ID
                        join c in context.Students on a.Roll equals c.Roll
                        join d in context.Class_Student on c.Roll equals d.Roll
                        join e in context.Classes on d.Class_ID equals e.Class_ID
                        where ((b.Specification_ID == SpecID) && (e.Class_ID == Class_ID))
                        select a)
                        .ToList();
            if (list.Count() > 0)
            {
                var maxDate = list.OrderByDescending(o => o.Date_Import).FirstOrDefault().Date_Import;
                list = list.Where(o => o.Date_Import == maxDate).ToList();
            }
            return list;
        }
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
        public int GetHashCode(Courses_Class_Mentor metor)
        {
            if (Object.ReferenceEquals(metor, null)) return 0;
            int hashCourseName = metor.Course_Name == null ? 0 : metor.Course_Name.GetHashCode();
            int hashClassID = metor.Class_ID.GetHashCode();
            return hashCourseName ^ hashClassID;
        }
    }
    class ListSpecComparer : IEqualityComparer<Spec_Class_Mentor>
    {
        public bool Equals(Spec_Class_Mentor x, Spec_Class_Mentor y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Class_ID == y.Class_ID && x.Specification_Name == y.Specification_Name;
        }
        public int GetHashCode(Spec_Class_Mentor metor)
        {
            if (Object.ReferenceEquals(metor, null)) return 0;
            int hashSpecName = metor.Specification_Name == null ? 0 : metor.Specification_Name.GetHashCode();
            int hashClassID = metor.Class_ID.GetHashCode();
            return hashSpecName ^ hashClassID;
        }
    }
    public class Courses_Class_Mentor
    {
        public string Subject_ID { get; set; }
        public string CoursesID_ClassID { get; set; }
        public string Course_Name { get; set; }
        public string Class_ID { get; set; }
    }
    public class Spec_Class_Mentor
    {
        public string Subject_ID { get; set; }
        public string SpecID_ClassID { get; set; }
        public string Specification_Name { get; set; }
        public string Class_ID { get; set; }
    }
    public class CourseSubject
    {
        public string Subject_ID { get; set; }
        public string Subject_Name { get; set; }
        public string Course_Name { get; set; }
    }
    public class MentorObject
    {
        public string id { get; set; }
        public string Class_ID { get; set; }
        public string Subject_Name { get; set; }
        public string Subject_ID { get; set; }
    }
}
