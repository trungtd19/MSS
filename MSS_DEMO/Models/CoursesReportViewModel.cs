using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Models
{
    public class CoursesReportViewModel : Student_Course_Log
    {
        public IPagedList<Student_Course_Log> PageList;
        public string searchCheck { get; set; }
        public string option { get; set; }
        public IList<string> listOption;
        public IList<string> listSubject;
    }
}