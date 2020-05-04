using MSS_DEMO.Core.Components;
using MSS_DEMO.Core.Interface;
using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Import
{
    public class GetRow : IGetRow
    {
        public Student GetStudent(List<string> row,string semester, string campus)
        {
            return new Student
            {
                Email = row[2].ToString(),
                Roll = row[0].ToString(),
                Full_Name = row[1].ToString(),
                Campus_ID = campus,
                Semester_ID = semester
            };

        }

        public Subject_Student GetSubjectStudent(List<string> row, string semester)
        {
            return new Subject_Student
            {
                Roll = row[0].ToString(),
                Subject_ID = row[3].ToString().Split('-')[0],
                Semester_ID = semester
            };

        }
        public Student_Specification_Log GetStudentSpec(List<string> row,int userID, string dateImport, List<Specification> specifications, string semesterID, List<string> lstSubjectID)
        {
            dateImport = DateTime.ParseExact(dateImport, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime _dateImport = DateTime.Parse(dateImport);
            var Spec_ID_CSV = -1;
            foreach (var spec in specifications)
            {
                string specName = spec.Specification_Name.ToLower().Trim();
                string csvSpecName = row[3].ToString().ToLower().Trim();
                Spec_ID_CSV = specName.Equals(csvSpecName) ? spec.Specification_ID : -1;
                if (Spec_ID_CSV != -1) break;
            }
            if (Spec_ID_CSV != -1)
            {
                return new Student_Specification_Log
                {
                    Email = row[1].ToString(),
                    Roll = row[2].ToString().Split('-')[2],
                    Subject_ID = lstSubjectID.Count > 0 ? lstSubjectID[0] : null,
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
                    Semester_ID = semesterID,
                    Name = row[0].ToString(),
                    Enrollment_Source = row[12].ToString(),
                };
            }
            else
            {
                return new Student_Specification_Log
                {
                    Email = row[1].ToString(),
                    Roll = row[2].ToString().Split('-')[2],
                    Subject_ID = lstSubjectID.Count > 0 ? lstSubjectID[0] : null,
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
                    Semester_ID = semesterID,
                    Name = row[0].ToString(),
                    Enrollment_Source = row[12].ToString(),
                };
            }
        
        }
        public Student_Course_Log GetStudentCourse(List<string> row, int userID, string dateImport, List<Course_Spec_Sub> course_Spec_Subs, string semesterID, List<string> lstSubjectID)
        {
            Student_Course_Log log = new Student_Course_Log();
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
                log = new Student_Course_Log
                {
                    Email = row[1].ToString(),
                    Roll = row[2].ToString().Split('-')[2],
                    Course_ID = Cour_ID_CSV,
                    Course_Name = row[3].ToString(),
                    Subject_ID = lstSubjectID.Count > 0 ? lstSubjectID[0] : null,
                    Campus = row[2].ToString().Split('-')[1],
                    Course_Enrollment_Time = row[7].ToString() != "" ? DateTime.Parse(row[7].ToString()) : DateTime.Parse("01/01/1970"),
                    Course_Start_Time = row[8].ToString() != "" ? DateTime.Parse(row[8].ToString()) : DateTime.Parse("01/01/1970"),
                    Last_Course_Activity_Time = row[9].ToString() != "" ? DateTime.Parse(row[9].ToString()) : DateTime.Parse("01/01/1970"),
                    Overall_Progress = Double.Parse(row[10].ToString()),
                    Estimated = Double.Parse(row[11].ToString()),
                    Completed = Boolean.Parse((row[12].ToString().ToLower() == "yes" ? "True" : "False")),
                    Status = Boolean.Parse((row[13].ToString().ToLower() == "yes" ? "True" : "False")),
                    Program_Slug = row[14].ToString(),
                    Program_Name = row[15].ToString(),
                    Completion_Time = row[17].ToString() != "" ? DateTime.Parse(row[17].ToString().Substring(0, 10)) : DateTime.Parse("01/01/1970"),
                    Course_Grade = Double.Parse(row[18].ToString()),
                    User_ID = userID,
                    Date_Import = _dateImport,
                    Semester_ID = semesterID,
                    Name = row[0].ToString(),
                    University = row[6].ToString(),
                    Course_Slug = row[5].ToString(),
                    Enrollment_Source = row[16].ToString()
                };
            }
            else
            {
                log = new Student_Course_Log
                {
                    Email = row[1].ToString(),
                    Roll = row[2].ToString().Split('-')[2],
                    Course_Name = row[3].ToString(),
                    Subject_ID = lstSubjectID.Count > 0 ? lstSubjectID[0] : null,
                    Campus = row[2].ToString().Split('-')[1],
                    Course_Enrollment_Time = row[7].ToString() != "" ? DateTime.Parse(row[7].ToString()) : DateTime.Parse("01/01/1970"),
                    Course_Start_Time = row[8].ToString() != "" ? DateTime.Parse(row[8].ToString()) : DateTime.Parse("01/01/1970"),
                    Last_Course_Activity_Time = row[9].ToString() != "" ? DateTime.Parse(row[9].ToString()) : DateTime.Parse("01/01/1970"),
                    Overall_Progress = Double.Parse(row[10].ToString()),
                    Estimated = Double.Parse(row[11].ToString()),
                    Completed = Boolean.Parse((row[12].ToString().ToLower() == "yes" ? "True" : "False")),
                    Status = Boolean.Parse((row[13].ToString().ToLower() == "yes" ? "True" : "False")),
                    Program_Slug = row[14].ToString(),
                    Program_Name = row[15].ToString(),
                    Completion_Time = row[17].ToString() != "" ? DateTime.Parse(row[17].ToString().Substring(0,10)) : DateTime.Parse("01/01/1970"),
                    Course_Grade = Double.Parse(row[18].ToString()),
                    User_ID = userID,
                    Date_Import = _dateImport,
                    Semester_ID = semesterID,
                    Name = row[0].ToString(),
                    University = row[6].ToString(),
                    Course_Slug = row[5].ToString(),
                    Enrollment_Source = row[16].ToString()
                };
            } 
            return log;
        }
    }
}