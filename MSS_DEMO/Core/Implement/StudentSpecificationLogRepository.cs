using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Implement
{
    public class StudentSpecificationLogRepository : BaseRepository<Student_Specification_Log>
    {
        public StudentSpecificationLogRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<Student_Specification_Log> GetPageList()
        {
            List<Student_Specification_Log> student = new List<Student_Specification_Log>();
            using (MSSEntities db = new MSSEntities())
            {
                student = (from o in db.Student_Specification_Log
                           orderby o.Specification_Log_ID descending
                           select o)
                          .ToList();

                return student;
            }

        }
        public int CleanSpecificationReport(string ImportedDate, string Semester_ID)
        {
            int rowDelete = 0;
            string sqlQuery = "";
            if (!string.IsNullOrEmpty(ImportedDate) && !string.IsNullOrEmpty(Semester_ID))
            {
                DateTime dt = DateTime.ParseExact(ImportedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                sqlQuery = "DELETE FROM Student_Specification_Log WHERE Date_Import = '" + dt + "' and Semester_ID = '" + Semester_ID + "'";
            }
            else
            {
                if (!string.IsNullOrEmpty(ImportedDate))
                {
                    DateTime dt = DateTime.ParseExact(ImportedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    sqlQuery = "DELETE FROM Student_Specification_Log WHERE Date_Import = '" + dt + "'";
                }
                else
                {
                    sqlQuery = "DELETE FROM Student_Specification_Log WHERE Semester_ID = '" + Semester_ID + "'";
                }
            }
            rowDelete = context.Database.ExecuteSqlCommand(sqlQuery);
            return rowDelete;
        }
        public bool IsExitsDateImport(string date)
        {
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var log = context.Student_Specification_Log.Where(o => o.Date_Import == dt).FirstOrDefault();
            if (log == null)
            {
                return false;
            }
            else return true;
        }
        public List<Student_Specification_Log> listSpecCompulsory(List<Student_Specification_Log>  logList, string SemesterID, string checkString)
        {
            List<Student_Specification_Log> lst = new List<Student_Specification_Log>();
            if (checkString == "Yes")
            {
                lst = (from log in logList
                            join spec in context.Specifications on log.Specification_ID equals spec.Specification_ID
                            join sub in context.Subjects on spec.Subject_ID equals sub.Subject_ID
                            join subStud in context.Subject_Student on sub.Subject_ID equals subStud.Subject_ID
                            where (subStud.Semester_ID == SemesterID && sub.Subject_Active == true)
                            select log)
                          .ToList();
            }
            else
            {
               var lstID = (from log in logList
                       join spec in context.Specifications on log.Specification_ID equals spec.Specification_ID
                       join sub in context.Subjects on spec.Subject_ID equals sub.Subject_ID
                       join subStud in context.Subject_Student on sub.Subject_ID equals subStud.Subject_ID
                       where (subStud.Semester_ID == SemesterID && sub.Subject_Active == true)
                       select log.Specification_Log_ID)
                       .ToList();
                lst = logList;
                foreach (var item in lstID)
                {
                    lst.Remove(lst.Where(l => l.Specification_Log_ID == item).FirstOrDefault());
                }
            }
            return lst;
        }
        public List<string> getSubjectID(string SpecName, string Roll, string Semester)
        {
            var list = context.Database.SqlQuery<string>("SELECT DISTINCT c.Subject_ID from Specification c  " +
                "inner join Subject_Student d on c.Subject_ID = d.Subject_ID " +
                "where c.Specification_Name ='"+ SpecName + "' and d.Roll = '" + Roll + "' and d.Semester_ID = '" + Semester + "'")
                .ToList();
            return list;
        }
        public List<string> getDatebySemester(Semester semester)
        {
            var dateList = context.Student_Specification_Log.Select(o => o.Date_Import).Where(o => o <= semester.End_Date && o >= semester.Start_Date).ToList();
            dateList = dateList.Distinct().OrderByDescending(o => o).ToList();
            List<string> date = new List<string>();
            foreach (var _date in dateList)
            {
                date.Add(Convert.ToDateTime(_date).ToString("dd/MM/yyyy"));
            }
            return date;
        }
        public string getSqlQuery(List<string> row, int userID, string dateImport, List<Specification> specifications, string semesterID, List<string> lstSubjectID)
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
            string sqlText = "";
            if (Spec_ID_CSV != -1)
            {
                sqlText = "Insert into [dbo].[Student_Specification_Log] ([Roll],[Specialization],[Specialization_Slug] ,[University] ,[Specialization_Enrollment_Time]," +
                    "[Last_Specialization_Activity_Time],[Completed],[Status] ,[Program_Slug],[Program_Name] ,[Specialization_Completion_Time],[Campus],[Date_Import],[User_ID]," +
                    "[Email],[Specification_ID] ,[Semester_ID],[Name],[Enrollment_Source],[Subject_ID]) Values( '" +
               "" + row[2].ToString().Split('-')[2] + "','" +
               "" + row[3].ToString() + "','" +
               "" + row[4].ToString() + "','" +
               "" + row[5].ToString() + "','" +
               "" + (row[6].ToString() != "" ? DateTime.Parse(row[6].ToString()) : DateTime.Parse("01/01/1970")) + "','" +
               "" + (row[7].ToString() != "" ? DateTime.Parse(row[7].ToString()) : DateTime.Parse("01/01/1970")) + "','" +
               "" + Boolean.Parse((row[8].ToString().ToLower() == "yes" ? "True" : "False")) + "','" +
               "" + Boolean.Parse((row[9].ToString().ToLower() == "yes" ? "True" : "False")) + "','" +
               "" + row[10].ToString() + "','" +
               "" + row[11].ToString() + "','" +
               "" + (row[13].ToString() != "" ? DateTime.Parse(row[13].ToString().Substring(0, 10)) : DateTime.Parse("01/01/1970")) + "','" +
               "" + row[2].ToString().Split('-')[1] + "','" +
               "" + _dateImport + "','" +
               "" + userID + "','" +
               "" + row[1].ToString() + "','" +
               "" + Spec_ID_CSV + "','" + 
               "" + semesterID + "','" +
               "" + row[0].ToString() + "','" +
               "" + row[12].ToString() + "','" +
               "" + (lstSubjectID.Count > 0 ? lstSubjectID[0] : null) + "'" +
               ");";
            }
            else
            {
                sqlText = "Insert into [dbo].[Student_Specification_Log] ([Roll],[Specialization],[Specialization_Slug] ,[University] ,[Specialization_Enrollment_Time]," +
                    "[Last_Specialization_Activity_Time],[Completed],[Status] ,[Program_Slug],[Program_Name] ,[Specialization_Completion_Time],[Campus],[Date_Import],[User_ID]," +
                    "[Email],[Semester_ID],[Name],[Enrollment_Source],[Subject_ID]) Values( '" +
               "" + row[2].ToString().Split('-')[2] + "','" +
               "" + row[3].ToString() + "','" +
               "" + row[4].ToString() + "','" +
               "" + row[5].ToString() + "','" +
               "" + (row[6].ToString() != "" ? DateTime.Parse(row[6].ToString()) : DateTime.Parse("01/01/1970")) + "','" +
               "" + (row[7].ToString() != "" ? DateTime.Parse(row[7].ToString()) : DateTime.Parse("01/01/1970")) + "','" +
               "" + Boolean.Parse((row[8].ToString().ToLower() == "yes" ? "True" : "False")) + "','" +
               "" + Boolean.Parse((row[9].ToString().ToLower() == "yes" ? "True" : "False")) + "','" +
               "" + row[10].ToString() + "','" +
               "" + row[11].ToString() + "','" +
               "" + (row[13].ToString() != "" ? DateTime.Parse(row[13].ToString().Substring(0, 10)) : DateTime.Parse("01/01/1970")) + "','" +
               "" + row[2].ToString().Split('-')[1] + "','" +
               "" + _dateImport + "','" +
               "" + userID + "','" +
               "" + row[1].ToString() + "','" +
               "" + semesterID + "','" +
               "" + row[0].ToString() + "','" +
               "" + row[12].ToString() + "','" +
               "" + (lstSubjectID.Count > 0 ? lstSubjectID[0] : null) + "'" +
               ");";
            }
            return sqlText;
        }
    }
}