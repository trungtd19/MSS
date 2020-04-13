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
    
    public partial class Student_Specification_Log
    {
        public int Specification_Log_ID { get; set; }
        public string Roll { get; set; }
        public string Subject_ID { get; set; }
        public string Specialization { get; set; }
        public string Specialization_Slug { get; set; }
        public string University { get; set; }
        public Nullable<System.DateTime> Specialization_Enrollment_Time { get; set; }
        public Nullable<System.DateTime> Last_Specialization_Activity_Time { get; set; }
        public Nullable<bool> Completed { get; set; }
        public Nullable<bool> Status { get; set; }
        public string Program_Slug { get; set; }
        public string Program_Name { get; set; }
        public Nullable<System.DateTime> Specialization_Completion_Time { get; set; }
        public string Campus { get; set; }
        public Nullable<System.DateTime> Date_Import { get; set; }
        public Nullable<int> User_ID { get; set; }
        public string Email { get; set; }
        public Nullable<int> Specification_ID { get; set; }
        public string Semester_ID { get; set; }
    
        public virtual Specification Specification { get; set; }
        public virtual Student Student { get; set; }
        public virtual User_Role User_Role { get; set; }
    }
}
