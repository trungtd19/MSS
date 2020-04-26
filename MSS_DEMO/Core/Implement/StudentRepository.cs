using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;

namespace MSS_DEMO.Repository
{
    public class StudentRepository : BaseRepository<Student>
    {
        public StudentRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<Student> GetPageList()
        {
            List<Student> student = new List<Student>();

                student = (from o in context.Students
                          orderby o.Roll ascending
                          select o)                      
                          .ToList();

                return student;

        }

        public bool IsExtisStudent(string studentID, string semesterID)
        {
            bool check = true;
            Student student = context.Students.Where(x => x.Roll == studentID && x.Semester_ID == semesterID).FirstOrDefault();
            if (student != null)
            {
                check = true;
            }
            else
                check = false;
            return check;
        }
        public Student getByRollAndSemester(string id)
        {
            string Roll = id.Split('^')[0];
            string SemesterID = id.Split('^')[1];
            return context.Students.Find(Roll, SemesterID);
        }    

    }
    public class StudentSubjectView :Student
    {
        public string Subject { set; get; }
    }
}