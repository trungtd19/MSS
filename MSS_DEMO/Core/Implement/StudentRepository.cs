using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
        public bool CheckExitsStudent(string id)
        {
            bool check = true;
            Student student = context.Students.Where(x => x.Roll == id).FirstOrDefault();
            if (student != null)
            {
                throw new Exception("Sinh viên " + id + " đã tồn tại!");
            }
            else
                check = false;
            return check;
        }
        public bool IsExtisStudent(string id)
        {
            bool check = true;
            Student student = context.Students.Where(x => x.Roll == id).FirstOrDefault();
            if (student != null)
            {
                check = true;
            }
            else
                check = false;
            return check;
        }

    }
}