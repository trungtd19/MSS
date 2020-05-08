using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Models
{
    public class StudentDetailsViewModel
    {
        public IList<Student_Course_Log> UsageReport;
        public IList<InfoStudent> MemberReport;
        public string Note { get; set; }
    }
}