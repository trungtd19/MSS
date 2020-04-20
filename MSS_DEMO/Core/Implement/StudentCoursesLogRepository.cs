using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using MSS_DEMO.ServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Implement
{
    public class StudentCoursesLogRepository : BaseRepository<Student_Course_Log>
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
                        select new
                        {
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