using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Models
{
    public class CertificateViewModel
    {
        public string CourseName { get; set; }
        public string Link { get; set; }
        public int CourseId { get; set; }
        public int SpecID { get; set; }
        public string Roll { get; set; }
        public List<CertificateViewModel> certificatesModel { get; set; }
        public string SubjectName { get; set; }
        public DateTime Date_Submit { get; set; }
    }
}