using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Models
{
    public class Report
    {
        public string Sub { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Total { get; set; }
        public int HN { get; set; }
        public int DN { get; set; }
        public int SG { get; set; }
        public int CT { get; set; }

        public List<Report> Info { get; set; }
        public List<Report2> Info2 { get; set; }
        public List<Report3> Info3 { get; set; }

    }

    public class Report2
    {
        public string Sub { get; set; }
        public string Name { get; set; }
        public double Study { get; set; }
        public int Total { get; set; }
        public double HN { get; set; }
        public double DN { get; set; }
        public double SG { get; set; }
        public double CT { get; set; }

    }

    public class Report3
    {
        public string Title { get; set; }
        public double HN { get; set; }
        public double DN { get; set; }
        public double SG { get; set; }
        public double CT { get; set; }
    }
}