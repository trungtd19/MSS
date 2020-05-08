using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Models
{
    public class StatusOverviewModel
    {
        public string Roll { get; set; }
        public string Email { get; set; }
        public string SubjectID { get; set; }
        public string SubjectName { get; set; }
        public Nullable<int> No_Compulsory_Course { get; set; }
        public Nullable<int> No_Course_Completed { get; set; }
        public string Spec_Completed { get; set; }
        public string Final_Status { get; set; }
        public string Campus { get; set; }
        public string Semester { get; set; }
        public string Date { get; set; }
        public List<StatusOverviewModel> OverviewList { get; set; }
        public string Note { get; set; }
        public string searchCheck { get; set; }

    }
}