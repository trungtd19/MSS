using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MSS_DEMO.Core.Implement
{
    public class SubjectStudentRepository : BaseRepository<Subject_Student>
    {
        public SubjectStudentRepository(MSSEntities context)
           : base(context)
        {
        }
        public string DeleteListSubject(string Subject_ID, string SemesterID)
        {
            string message = "";
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {            
                try
                {
                    var student = context.Students.ToList();
                    var list = context.Subject_Student.ToList();
                    var list1 = list.Where(x => x.Subject_ID == Subject_ID && x.Semester_ID == SemesterID).ToList();
                    List<string> li = list1.Select(x => x.Roll).ToList();
                    foreach (var lst in list1)
                    {
                        var sub = context.Subject_Student.Find(lst.ID);
                        context.Subject_Student.Remove(sub);
                    }
                    list = context.Subject_Student.ToList();
                    foreach (var ls in li)
                    {
                        if (list.Where(x => x.Roll == ls && x.Semester_ID == SemesterID).FirstOrDefault() == null)
                        {
                            var stu = context.Students.Find(ls, SemesterID);
                            context.Students.Remove(stu);
                        }
                        else continue;
                    }
                    context.SaveChanges();
                    dbContextTransaction.Commit();
                    message = "Delete succesfull!";
                }
                catch (Exception ex)
                {
                    message = "Can't delete subjects";
                    dbContextTransaction.Rollback();
                }
            }
            return message;
        }
        public bool IsExitsSubjectStudent(string roll, string subject_id)
        {
            var subStudent = context.Subject_Student.Where(o => o.Roll == roll && o.Subject_ID == subject_id).FirstOrDefault();
            if (subStudent != null) return true;
            return false;
        }
        public StringBuilder getListSubject(string id)
        {
            string Roll = id.Split('^')[0];
            string SemesterID = id.Split('^')[1];
            StringBuilder sb = new StringBuilder();
            var subject = context.Subject_Student.Where(s => s.Roll == Roll && s.Semester_ID == SemesterID).Select(s => s.Subject_ID).ToList();
            foreach (var _subject in subject)
            {
                sb.Append(" - " + _subject);
            }
            sb.Remove(0, 2);
            return sb;
        }
        public Student getByRollAndSemester(string id)
        {
            string Roll = id.Split('^')[0];
            string SemesterID = id.Split('^')[1];
            return context.Students.Find(Roll, SemesterID);
        }
    }
}