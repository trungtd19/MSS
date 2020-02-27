using MSS_DEMO.Core.Components;
using MSS_DEMO.Core.Interface;
using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Import
{
    public class GetRow : IGetRow
    {
        public Student GetStudent(List<string> row)
        {
            return new Student
            {
                Email = row[2].ToString(),
                Roll = row[0].ToString(),
                Full_Name = row[1].ToString(),
                Campus = row[3].ToString().Split('-')[1],
            };

        }
        public Class_Student GetClassStudent(List<string> row)
        {
            return new Class_Student
            {
                Roll = row[1].ToString(),
                Class_ID = row[2].ToString(),
            };

        }
        public Subject_Student GetSubjectStudent(List<string> row)
        {
            return new Subject_Student
            {
                Roll = row[0].ToString(),
                Subject_ID = row[3].ToString().Split('-')[0],
            };

        }
        public Student_Specification_Log GetStudentSpec(List<string> row,int userID, string dateImport, List<Specification> listIdSubjects)
        {
            DateTime _dateImport = DateTime.Parse(dateImport);
            var Spec_ID_CSV = "";
            foreach (var listIdSubject in listIdSubjects)
            {
                Spec_ID_CSV = listIdSubject.Subject_ID.Trim() == row[2].ToString().Split('-')[0] ? listIdSubject.Specification_ID.Trim() : null;
                if (Spec_ID_CSV != null) break;
            }
            return new Student_Specification_Log
            {
                Roll = row[1].ToString().Split('@')[0],
                // Roll = row[0].ToString(),
                Subject_ID = row[2].ToString().Split('-')[0],
                Specification_ID = Spec_ID_CSV,
                Campus = row[2].ToString().Split('-')[1],
                Specialization = row[3].ToString(),
                Specialization_Slug = row[4].ToString(),
                University = row[5].ToString(),
                Specialization_Enrollment_Time = row[6].ToString() != "" ? DateTime.Parse(row[6].ToString()) : DateTime.Parse("01/01/1970"),
                Last_Specialization_Activity_Time = row[7].ToString() != "" ? DateTime.Parse(row[7].ToString()) : DateTime.Parse("01/01/1970"),
                Completed = bool.Parse((row[8].ToString().ToLower() == "yes" ? "True" : "False")),
                Status = bool.Parse((row[9].ToString().ToLower() == "yes" ? "True" : "False")),
                Program_Slug = row[10].ToString(),
                Program_Name = row[11].ToString(),
                Specialization_Completion_Time = row[13].ToString() != "" ? DateTime.Parse(row[13].ToString()) : DateTime.Parse("01/01/1970"),
                User_ID = userID,
                Date_Import = _dateImport,
            };
        }
        public Student_Course_Log GetStudentCourse(List<string> row, int userID, string dateImport, List<Course_Spec_Sub> course_Spec_Subs)
        {
            DateTime _dateImport = DateTime.Parse(dateImport);
            var Cour_ID_CSV = "";
            foreach (var listID in course_Spec_Subs)
            {
                Cour_ID_CSV = listID.Subject_ID.Trim() == row[2].ToString().Split('-')[0] ? listID.Course_ID.Trim() : null;
                if (Cour_ID_CSV != null) break;
            }
            Student_Course_Log log1 = new Student_Course_Log
            {
                Roll = row[1].ToString().Split('@')[0],
                // Roll = row[0].ToString(),
                Course_ID = Cour_ID_CSV,
                Course_Enrollment_Time = row[7].ToString() != "" ? DateTime.Parse(row[7].ToString()) : DateTime.Parse("01/01/1970"),
                Course_Start_Time = row[8].ToString() != "" ? DateTime.Parse(row[8].ToString()) : DateTime.Parse("01/01/1970"),
                Last_Course_Activity_Time = row[9].ToString() != "" ? DateTime.Parse(row[9].ToString()) : DateTime.Parse("01/01/1970"),
                Overall_Progress = Double.Parse(row[10].ToString()),
                Estimated = Double.Parse(row[11].ToString()),
                Completed = Boolean.Parse((row[12].ToString().ToLower() == "yes" ? "True" : "False")),
                Status = Boolean.Parse((row[13].ToString().ToLower() == "yes" ? "True" : "False")),
                Program_Slug = row[14].ToString(),
                Program_Name = row[15].ToString(),
                Completion_Time = row[17].ToString() != "" ? DateTime.Parse(row[17].ToString()) : DateTime.Parse("01/01/1970"),
                Course_Grade = Double.Parse(row[18].ToString()),
                User_ID = userID,
                Date_Import = _dateImport,
            };
            return log1;
        }
        public String ChangeBoolean(string name)
        {
            if (name.ToLower() == "yes") return "True";
            else return "False";
        }
        public void checkIdSubject()
        {

        }

    }
}