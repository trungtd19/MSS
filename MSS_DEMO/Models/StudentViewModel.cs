using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Models
{
    public class StudentViewModel : Student
    {
        public IPagedList<Student> PageList;
        public string searchCheck { get; set; }
        public IList<string> lstSemester;
        public IList<string> lstCampus;
    }
}