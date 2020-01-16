//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MSS_DEMO.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Student_Course_Log
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> Course_Enrollment_Time { get; set; }
        public Nullable<System.DateTime> Course_Start_Time { get; set; }
        public Nullable<System.DateTime> Last_Course_Activity_Time { get; set; }
        public Nullable<double> Overall_Progress { get; set; }
        public Nullable<double> Estimated { get; set; }
        public Nullable<bool> Completed { get; set; }
        public Nullable<bool> Status { get; set; }
        public string Program_Slug { get; set; }
        public string Program_Name { get; set; }
        public Nullable<System.DateTime> Completion_Time { get; set; }
        public string Course_ID { get; set; }
        public Nullable<double> Course_Grade { get; set; }
        public string Roll { get; set; }
    
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}
