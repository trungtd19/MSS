using MSS_DEMO.Core.Components;
using MSS_DEMO.Models;
using MSS_DEMO.MssService;
using MSS_DEMO.Repository;
using MSS_DEMO.ServiceReference;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using ArrayOfString = MSS_DEMO.MssService.ArrayOfString;

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
        public List<MentorObject> getListSubjectClass(string userMentor,string semesterName)
        {
            CourseraApiSoapClient courseraApiSoap = new CourseraApiSoapClient();
            var listSubjectID = context.Subjects.Where(o => o.Subject_Active == true).Select(o => o.Subject_ID).ToList();
            ArrayOfString arraySubject = new ArrayOfString();
            arraySubject.AddRange(listSubjectID);
            string authenKey = "A90C9954-1EDD-4330-B9F3-3D8201772EEA";
            List<MentorObject> objectMentor = new List<MentorObject>();
            try
            {
                string jsonData = courseraApiSoap.GetScheduledSubject(authenKey, userMentor.Split('@')[0], arraySubject, semesterName);
                var scheduledSubject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MertorFAP>>(jsonData);
                scheduledSubject = scheduledSubject.Distinct(new ListComparer()).ToList();
               
                foreach (var scheduled in scheduledSubject)
                {
                    objectMentor.Add(new MentorObject
                    {
                        Subject_ID = scheduled.SubjectCode,
                        Class_ID = scheduled.ClassName
                    });
                }             
            }
            catch
            {
                objectMentor = new List<MentorObject>();
            }
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
        // không sử dụng 
        public void GetStudentCourse(string [] row, string userID, string dateImport, List<Course_Spec_Sub> course_Spec_Subs, string semesterID)
        {
            dateImport = DateTime.ParseExact(dateImport, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime _dateImport = DateTime.Parse(dateImport);
            int Cour_ID_CSV = -1;
            foreach (var listID in course_Spec_Subs)
            {
                string courseName = listID.Course_Name.ToLower().Trim();
                string csvCourseName = row[3].ToString().ToLower().Trim();
                string SubID = row[2].ToString().Split('-')[0];

                if (courseName.Equals(csvCourseName) && SubID == listID.Subject_ID)
                {
                    Cour_ID_CSV = listID.Course_ID;
                }
                else
                {
                    Cour_ID_CSV = -1;
                }
                if (Cour_ID_CSV != -1) break;
            }
            if (Cour_ID_CSV != -1)
            {
              context.Database.ExecuteSqlCommand(
              "Insert into tableName Values(@Email, @Roll, @Course_ID,@Course_Name, @Subject_ID, @Campus, @Course_Enrollment_Time, @Course_Start_Time," +
              "@Last_Course_Activity_Time, @Overall_Progress, @Estimated, @Completed, @Status, @Program_Slug, @Program_Name, @Completion_Time," +
              "@Course_Grade, @User_ID, @Date_Import, @Semester_ID, @Name, @University, @Course_Slug, @Enrollment_Source)",
              new SqlParameter("Email", row[1].ToString()),
              new SqlParameter("Roll", row[2].ToString().Split('-')[2]),
              new SqlParameter("Course_ID", Cour_ID_CSV),
              new SqlParameter("Course_Name", row[3].ToString()),
              new SqlParameter("Subject_ID", row[2].ToString().Split('-')[0]),
              new SqlParameter("Campus", row[2].ToString().Split('-')[1]),
              new SqlParameter("Course_Enrollment_Time", row[7].ToString() != "" ? DateTime.Parse(row[7].ToString()) : DateTime.Parse("01/01/1970")),
              new SqlParameter("Course_Start_Time", row[8].ToString() != "" ? DateTime.Parse(row[8].ToString()) : DateTime.Parse("01/01/1970")),
              new SqlParameter("Last_Course_Activity_Time", row[9].ToString() != "" ? DateTime.Parse(row[9].ToString()) : DateTime.Parse("01/01/1970")),
              new SqlParameter("Overall_Progress", Double.Parse(row[10].ToString())),
              new SqlParameter("Estimated", Double.Parse(row[11].ToString())),
              new SqlParameter("Completed", Boolean.Parse((row[12].ToString().ToLower() == "yes" ? "True" : "False"))),
              new SqlParameter("Status", Boolean.Parse((row[13].ToString().ToLower() == "yes" ? "True" : "False"))),
              new SqlParameter("Program_Slug", row[14].ToString()),
              new SqlParameter("Program_Name", row[15].ToString()),
              new SqlParameter("Completion_Time", row[17].ToString() != "" ? DateTime.Parse(row[17].ToString().Substring(0, 10)) : DateTime.Parse("01/01/1970")),
              new SqlParameter("Course_Grade", Double.Parse(row[18].ToString())),
              new SqlParameter("User_ID", userID),
              new SqlParameter("Date_Import", _dateImport),
              new SqlParameter("Semester_ID", semesterID),
              new SqlParameter("Name", row[0].ToString()),
              new SqlParameter("University", row[6].ToString()),
              new SqlParameter("Course_Slug", row[5].ToString()),
              new SqlParameter("Enrollment_Source", row[16].ToString()));
            }
            else
            {
                context.Database.ExecuteSqlCommand(
              "Insert into tableName Values(@Email, @Roll, @Course_Name, @Subject_ID, @Campus, @Course_Enrollment_Time, @Course_Start_Time," +
              " @Last_Course_Activity_Time, @Overall_Progress, @Estimated, @Completed, @Status, @Program_Slug, @Program_Name, @Completion_Time," +
              " @Course_Grade, @User_ID, @Date_Import, @Semester_ID, @Name, @University, @Course_Slug, @Enrollment_Source)",
              new SqlParameter("Email", row[1].ToString()),
              new SqlParameter("Roll", row[2].ToString().Split('-')[2]),
              new SqlParameter("Course_Name", row[3].ToString()),
              new SqlParameter("Subject_ID", row[2].ToString().Split('-')[0]),
              new SqlParameter("Campus", row[2].ToString().Split('-')[1]),
              new SqlParameter("Course_Enrollment_Time", row[7].ToString() != "" ? DateTime.Parse(row[7].ToString()) : DateTime.Parse("01/01/1970")),
              new SqlParameter("Course_Start_Time", row[8].ToString() != "" ? DateTime.Parse(row[8].ToString()) : DateTime.Parse("01/01/1970")),
              new SqlParameter("Last_Course_Activity_Time", row[9].ToString() != "" ? DateTime.Parse(row[9].ToString()) : DateTime.Parse("01/01/1970")),
              new SqlParameter("Overall_Progress", Double.Parse(row[10].ToString())),
              new SqlParameter("Estimated", Double.Parse(row[11].ToString())),
              new SqlParameter("Completed", Boolean.Parse((row[12].ToString().ToLower() == "yes" ? "True" : "False"))),
              new SqlParameter("Status", Boolean.Parse((row[13].ToString().ToLower() == "yes" ? "True" : "False"))),
              new SqlParameter("Program_Slug", row[14].ToString()),
              new SqlParameter("Program_Name", row[15].ToString()),
              new SqlParameter("Completion_Time", row[17].ToString() != "" ? DateTime.Parse(row[17].ToString().Substring(0, 10)) : DateTime.Parse("01/01/1970")),
              new SqlParameter("Course_Grade", Double.Parse(row[18].ToString())),
              new SqlParameter("User_ID", userID),
              new SqlParameter("Date_Import", _dateImport),
              new SqlParameter("Semester_ID", semesterID),
              new SqlParameter("Name", row[0].ToString()),
              new SqlParameter("University", row[6].ToString()),
              new SqlParameter("Course_Slug", row[5].ToString()),
              new SqlParameter("Enrollment_Source", row[16].ToString()));
            }
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
    public class MertorFAP
    {
        public string Lecturer { get; set; }
        public string SubjectCode { get; set; }
        public string ClassName { get; set; }
    }
    class ListComparer : IEqualityComparer<MertorFAP>
    {
        public bool Equals(MertorFAP x, MertorFAP y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.ClassName == y.ClassName && x.SubjectCode == y.SubjectCode;
        }
        public int GetHashCode(MertorFAP product)
        {
            if (Object.ReferenceEquals(product, null)) return 0;
            int hashclassName = product.ClassName == null ? 0 : product.ClassName.GetHashCode();
            int hashsubCode = product.SubjectCode.GetHashCode();
            return hashclassName ^ hashsubCode;
        }
    }
    public class RollFAP
    {        
             public string RollNumber { get; set; }
    }
}
