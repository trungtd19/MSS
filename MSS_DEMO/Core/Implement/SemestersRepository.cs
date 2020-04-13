using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Components
{
    public class SemestersRepository : BaseRepository<Semester>
    {
        public SemestersRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<_Semester> toList()
        {
            using (MSSEntities db = new MSSEntities())
            {
                var sem = (from o in db.Semesters
                           orderby o.Start_Date descending
                           select new
                           {
                               Semester_ID = o.Semester_ID,
                               Semester_Name = o.Semester_Name,
                               Start_Date = o.Start_Date,
                               End_Date = o.End_Date
                           }).ToList().Select(x=> new _Semester
                           {
                               Semester_ID = x.Semester_ID,
                               Semester_Name = x.Semester_Name,
                               dateStart = convertDate(x.Start_Date),
                               dateEnd = convertDate(x.End_Date),
                           })
                           .ToList();
                return sem;
            }          
        }
        public string convertDate(DateTime? dateTime)
        {
            string date = Convert.ToDateTime(dateTime).ToString("dd/MM/yyyy");
            return date;
        }
        public string checkDateOfSemester(string dateImport)
        {
            DateTime _dateImport = DateTime.Parse(dateImport);     
            var semester = context.Semesters.Where(sem => sem.Start_Date < _dateImport && sem.End_Date > _dateImport).FirstOrDefault();
            if (semester == null)
            {
                throw new Exception("Imported date error!");
            }
            return semester.Semester_ID;
        }
        public bool IsExitsSemester(string semID, string semName)
        {
            bool check = true;
            Semester sem = context.Semesters.Where(x => x.Semester_ID == semID || x.Semester_Name == semName).FirstOrDefault();
            if (sem != null)
            {
                check = true;
            }
            else
                check = false;
            return check;
        }
    }
    public class _Semester : Semester
    {
        public string dateStart { get; set; }
        public string dateEnd { get; set; }

    }
    
}