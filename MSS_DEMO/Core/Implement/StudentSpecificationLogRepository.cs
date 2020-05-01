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
                //var listSpecCompulsoryCompleted = context.sp_Get_Compulsory_Spec_Completion(ImportedDate, SemesterID).ToList();
                //foreach (var item in listSpecCompulsoryCompleted)
                //{
                //    var roll = item.Split('-')[0];
                //    var sub = item.Split('-')[1];
                //    var info = context.Students.Where(m => m.Roll == roll && m.Semester_ID == SemesterID).FirstOrDefault();
                //    var spec = context.Specifications.Where(m => m.Subject_ID == sub).FirstOrDefault();
                //    logList = logList.Where(l => l.Roll == info.Roll && l.Specification_ID == spec.Specification_ID).ToList();
                //}
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
    }
}