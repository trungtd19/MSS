using MSS_DEMO.Core.Components;
using MSS_DEMO.Models;
using MSS_DEMO.MssService;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

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
        public List<MentorObject> getListSubjectClass(string userMentor,Semester semester)
        {
            CourseraApiSoapClient courseraApiSoap = new CourseraApiSoapClient();
            var listSubjectID = context.Subjects.Where(o => o.Subject_Active == true).Select(o => o.Subject_ID).ToArray();
            string authenKey = "A90C9954-1EDD-4330-B9F3-3D8201772EEA";
            List<MentorObject> objectMentor = new List<MentorObject>();
            try
            {
                string jsonData = courseraApiSoap.GetScheduledSubject(authenKey, userMentor.Split('@')[0], listSubjectID, semester.Semester_Name);
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
        public List<string> getSubjectID(string CourseName, string Roll, string Semester)
        {
            List<string> list = new List<string>();
            try
            {
                list = context.Database.SqlQuery<string>("SELECT DISTINCT d.Subject_ID FROM  Course b " +
            "inner join Specification c on b.Specification_ID = c.Specification_ID " +
            "inner join Subject_Student d on c.Subject_ID = d.Subject_ID " +
            "where b.Course_Name = '" + CourseName + "' and d.Roll = '" + Roll + "' and d.Semester_ID = '" + Semester + "'")
            .ToList();
            }
            catch
            {
                list = new List<string>();
            }      
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
        public List<string> getDatebySemester(Semester semester)
        {
            var dateList = context.Student_Course_Log.OrderByDescending(o => o.Date_Import).Select(o => o.Date_Import).Where(o => o <= semester.End_Date && o >= semester.Start_Date).Distinct().ToList();

            List<string> date = new List<string>();
            foreach (var _date in dateList)
            {
                date.Add(Convert.ToDateTime(_date).ToString("dd/MM/yyyy"));
            }
            return date;
        }
        public List<InfoStudent> MemberReport(string Roll, string Semester, DateTime? ImportedDate)
        {
            List<InfoStudent> Info = new List<InfoStudent>();
            var listCourseLog = (from stu_cour_log in context.Student_Course_Log
                                 where stu_cour_log.Roll == Roll && stu_cour_log.Semester_ID == Semester && stu_cour_log.Date_Import == ImportedDate
                                 select stu_cour_log).ToList();
            var listCourCp = (from stu in context.Students
                              join sub_stu in context.Subject_Student on stu.Roll equals sub_stu.Roll
                              join sub in context.Subjects on sub_stu.Subject_ID equals sub.Subject_ID
                              join spec in context.Specifications on sub.Subject_ID equals spec.Subject_ID
                              join cour in context.Courses on spec.Specification_ID equals cour.Specification_ID
                              where stu.Semester_ID == Semester && sub_stu.Semester_ID == Semester && stu.Roll == Roll && sub.Subject_Active == true
                              select new { cour, sub }).ToList();
            var listCourseIdLog = listCourseLog.Where(m => m.Course_ID != null).Select(m => m.Course_ID).Cast<int>().ToList();
            var listCourIdCp = listCourCp.Select(m => m.cour.Course_ID).ToList();
            var noEnoll = listCourIdCp.Except(listCourseIdLog);
            string timeActive = "";
            string timeCompleted = "";
            foreach (var item in listCourseLog)
            {
                var lastActive = (DateTime)item.Last_Course_Activity_Time;
                var comple = (DateTime)item.Completion_Time;
                string subjectId, subjectName;
                if (item.Course_ID == null)
                {
                    subjectId = "";
                    subjectName = "";
                }
                else
                {
                    subjectId = item.Subject_ID;
                    subjectName = listCourCp.Where(m => m.sub.Subject_ID == subjectId).Select(m => m.sub.Subject_Name).FirstOrDefault();
                }
                if (lastActive.ToString("dd/MM/yyyy") == "01/01/1970")
                {
                    timeActive = "";
                }
                else
                {
                    timeActive = lastActive.ToString("dd/MM/yyyy");
                }
                if (comple.ToString("dd/MM/yyyy") == "01/01/1970")
                {
                    timeCompleted = "";
                }
                else
                {
                    timeCompleted = comple.ToString("dd/MM/yyyy");
                }
                Info.Add(new InfoStudent
                {
                    SubjectID = subjectId,
                    Subject = subjectName,
                    Course_Name = item.Course_Name,
                    Course_Enrollment_Time = ((DateTime)item.Course_Enrollment_Time).ToString(),
                    Last_Course_Activity_Time = timeActive,
                    Overall_Progress = Math.Round((double)item.Overall_Progress, 1),
                    Completion_Time = timeCompleted,
                    Estimated = Math.Round((double)item.Estimated, 1),
                    Completed = (bool)item.Completed,
                });
            }
            foreach (var item in noEnoll)
            {
                Info.Add(new InfoStudent
                {
                    SubjectID = listCourCp.Where(m => m.cour.Course_ID == item).Select(m => m.sub.Subject_ID).FirstOrDefault(),
                    Subject = listCourCp.Where(m => m.cour.Course_ID == item).Select(m => m.sub.Subject_Name).FirstOrDefault(),
                    Course_Name = listCourCp.Where(m => m.cour.Course_ID == item).Select(m => m.cour.Course_Name).FirstOrDefault()
                });
            }
            return Info;
        }
        public string getSqlQuery(List<string> row, int userID, string dateImport, List<Course_Spec_Sub> course_Spec_Subs, string semesterID, List<string> lstSubjectID)
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
            string sqlText = "";
            if (Cour_ID_CSV != -1)
            {
                sqlText = "Insert into Student_Course_Log ([Course_Enrollment_Time],[Course_Start_Time]," +
               "[Last_Course_Activity_Time],[Overall_Progress],[Estimated],[Completed],[Status],[Program_Slug] " +
               ",[Program_Name],[Completion_Time],[Course_ID],[Course_Grade],[Roll],[Date_Import],[User_ID],[Email]," +
               "[Campus],[Subject_ID],[Course_Name],[Semester_ID],[Name],[Course_Slug],[University],[Enrollment_Source]) Values( '" +
               "" + (row[7].ToString() != "" ? DateTime.Parse(row[7].ToString()) : DateTime.Parse("01/01/1970")) + "','" +
               "" + (row[8].ToString() != "" ? DateTime.Parse(row[8].ToString()) : DateTime.Parse("01/01/1970")) + "','" +
               "" + (row[9].ToString() != "" ? DateTime.Parse(row[9].ToString()) : DateTime.Parse("01/01/1970")) + "','" +
               "" + Double.Parse(row[10].ToString()) + "','" +
               "" + Double.Parse(row[11].ToString()) + "','" +
               "" + Boolean.Parse((row[12].ToString().ToLower() == "yes" ? "True" : "False")) + "','" +
               "" + Boolean.Parse((row[13].ToString().ToLower() == "yes" ? "True" : "False")) + "','" +
               "" + row[14].ToString() + "','" +
               "" + row[15].ToString() + "','" +
               "" + (row[17].ToString() != "" ? DateTime.Parse(row[17].ToString().Substring(0, 10)) : DateTime.Parse("01/01/1970")) + "','" +
               "" + Cour_ID_CSV + "','" +
               "" + Double.Parse(row[18].ToString()) + "','" +
               "" + row[2].ToString().Split('-')[2] + "','" +
               "" + _dateImport + "','" +
               "" + userID + "','" +
               "" + row[1].ToString() + "','" +
               "" + row[2].ToString().Split('-')[1] + "','" +
               "" + (lstSubjectID.Count > 0 ? lstSubjectID[0] : null) + "','" +
               "" + row[3].ToString() + "','" +
               "" + semesterID + "','" +
               "" + row[0].ToString() + "','" +
               "" + row[5].ToString() + "','" +
               "" + row[6].ToString() + "','" +
               "" + row[16].ToString() + "'" +
               ");";
            }
            else
            {
                sqlText = "Insert into Student_Course_Log ([Course_Enrollment_Time],[Course_Start_Time]," +
               "[Last_Course_Activity_Time],[Overall_Progress],[Estimated],[Completed],[Status],[Program_Slug] " +
               ",[Program_Name],[Completion_Time],[Course_Grade],[Roll],[Date_Import],[User_ID],[Email]," +
               "[Campus],[Subject_ID],[Course_Name],[Semester_ID],[Name],[Course_Slug],[University],[Enrollment_Source]) Values( '" +
               "" + (row[7].ToString() != "" ? DateTime.Parse(row[7].ToString()) : DateTime.Parse("01/01/1970")) + "','" +
               "" + (row[8].ToString() != "" ? DateTime.Parse(row[8].ToString()) : DateTime.Parse("01/01/1970")) + "','" +
               "" + (row[9].ToString() != "" ? DateTime.Parse(row[9].ToString()) : DateTime.Parse("01/01/1970")) + "','" +
               "" + Double.Parse(row[10].ToString()) + "','" +
               "" + Double.Parse(row[11].ToString()) + "','" +
               "" + Boolean.Parse((row[12].ToString().ToLower() == "yes" ? "True" : "False")) + "','" +
               "" + Boolean.Parse((row[13].ToString().ToLower() == "yes" ? "True" : "False")) + "','" +
               "" + row[14].ToString() + "','" +
               "" + row[15].ToString() + "','" +
               "" + (row[17].ToString() != "" ? DateTime.Parse(row[17].ToString().Substring(0, 10)) : DateTime.Parse("01/01/1970")) + "','" +
               "" + Double.Parse(row[18].ToString()) + "','" +
               "" + row[2].ToString().Split('-')[2] + "','" +
               "" + _dateImport + "','" +
               "" + userID + "','" +
               "" + row[1].ToString() + "','" +
               "" + row[2].ToString().Split('-')[1] + "','" +
               "" + (lstSubjectID.Count > 0 ? lstSubjectID[0] : null) + "','" +
               "" + row[3].ToString() + "','" +
               "" + semesterID + "','" +
               "" + row[0].ToString() + "','" +
               "" + row[5].ToString() + "','" +
               "" + row[6].ToString() + "','" +
               "" + row[16].ToString() + "'" +
               ");";
            }
            return sqlText;
        }
        public int countInsert(StringBuilder sb)
        {
            int recordsInserted = 0;
            var ConnectionString = context.Database.Connection.ConnectionString;
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            SqlTransaction sqlTran = connection.BeginTransaction();
            SqlCommand command = new SqlCommand(sb.ToString(), connection);
            command.Transaction = sqlTran;
            try
            {
                command.CommandType = CommandType.Text;
                recordsInserted = command.ExecuteNonQuery();
                sqlTran.Commit();
            }
            catch (Exception ex)
            {
                sqlTran.Rollback();
            }
            return recordsInserted;
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
