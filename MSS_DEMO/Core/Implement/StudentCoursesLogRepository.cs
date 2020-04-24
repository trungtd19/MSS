using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using MSS_DEMO.ServiceReference;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Text;
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

        public int CleanUsageReport(string ImportedDate, string Semester_ID)
        {
            int rowDelete = 0;
            string sqlQuery = "";
            if (!string.IsNullOrEmpty(ImportedDate) && !string.IsNullOrEmpty(Semester_ID))
            {
                DateTime dt = DateTime.ParseExact(ImportedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                sqlQuery = "DELETE FROM Student_Course_Log WHERE Date_Import = '" + dt + "' and Semester_ID = '" + Semester_ID + "'";
            }
            else
            {
                if (!string.IsNullOrEmpty(ImportedDate))
                {
                    DateTime dt = DateTime.ParseExact(ImportedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    sqlQuery = "DELETE FROM Student_Course_Log WHERE Date_Import = '" + dt + "'";
                }
                else
                {
                    sqlQuery = "DELETE FROM Student_Course_Log WHERE Semester_ID = '" + Semester_ID + "'";
                }
            }
            rowDelete = context.Database.ExecuteSqlCommand(sqlQuery);
            return rowDelete;
        }
        public List<UsageReportNote> getListUsageReportNote()
        {
            var list = context.Database.SqlQuery<UsageReportNote>("Select " +
                " a.Roll,        " +
                " a.Email,       " +
                " a.Course_Name, " +
                " a.Course_Enrollment_Time,    " +
                " a.Course_Start_Time,         " +
                " a.Last_Course_Activity_Time, " +
                " a.Overall_Progress,          " +
                " a.Estimated, " +
                " a.Completed, " +
                " a.Status,    " +
                " a.Completion_Time, " +
                " a.Course_Grade,    " +
                " a.Semester_ID,     " +
                " a.Subject_ID,      " +
                " b.Note             " +
                " FROM Student_Course_Log a LEFT JOIN Mentor_Log b " +
                " ON a.Roll = b.Roll and a.Semester_ID = b.Semester_ID and a.Subject_ID = b.Subject_ID").ToList();         
            return list;
        }
        public bool IsExitsDateImport(string date)
        {
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var log = context.Student_Course_Log.Where(o => o.Date_Import == dt).FirstOrDefault();
            if (log == null)
            {
                return false;
            }
            else return true;
        }
    }
    public class MentorObject
    {
        public string id { get; set; }
        public string Class_ID { get; set; }
        public string Subject_Name { get; set; }
        public string Subject_ID { get; set; }
    }
    public class UsageReportNote : Student_Course_Log
    {
        public string Note { get; set; }
    }
}