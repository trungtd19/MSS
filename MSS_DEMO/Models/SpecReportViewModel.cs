using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Models
{
    public class SpecReportViewModel : Student_Specification_Log
    {
        public IPagedList<Student_Specification_Log> PageList;
        public string searchCheck { get; set; }
        public string option { get; set; }
        public IList<string> listOption;
        public IList<string> listSubject;
    }
}