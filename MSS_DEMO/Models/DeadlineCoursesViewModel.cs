using MSS_DEMO.Core.Implement;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSS_DEMO.Models
{
    public class DeadlineCoursesViewModel : Cour_dealine
    {
        public IPagedList<Cour_dealine> PageList;
        public string searchCheck { get; set; }
        public IList<SelectListItem> lstSemester;
    }
}