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
    }
}