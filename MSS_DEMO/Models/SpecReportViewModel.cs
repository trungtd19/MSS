using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSS_DEMO.Models
{
    public class SpecReportViewModel : Student_Specification_Log
    {
        public IPagedList<Student_Specification_Log> PageList;
        public string searchCheck { get; set; }
        public string completedSpec { get; set; }
        public string compulsorySpec { get; set; }
        public IList<string> listCompleted;
        public IList<string> listCompulsory;
        public string Subject_Name { get; set; }
        public string ImportedDate { get; set; }
        public IList<string> importedDate;
        public IList<SelectListItem> listSubject;
        public IList<SelectListItem> lstCampus;
        public IList<SelectListItem> lstSemester;
    }
}