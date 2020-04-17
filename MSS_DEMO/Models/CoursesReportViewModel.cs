using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MSS_DEMO.Core.Implement.StudentCoursesLogRepository;

namespace MSS_DEMO.Models
{
    public class CoursesReportViewModel : Student_Course_Log
    {
        public IPagedList<Student_Course_Log> PageList;
        public string searchCheck { get; set; }
        public string Subject_Name { get; set; }
        public string completedCourse { get; set; }        
        public string compulsoryCourse { get; set; }
        public string ImportedDate { get; set; }
        public IList<string> completedCour;
        public IList<string> compulsoryCour;
        public IList<string> importedDate;
        public IList<SelectListItem> listSubject;
        public IList<SelectListItem> lstCampus;
        public IList<SelectListItem> lstSemester;

    }
}