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
    
    public partial class Certificate
    {
        public int Certificate_ID { get; set; }
        public string Link { get; set; }
        public System.DateTime Date_Submit { get; set; }
        public string Roll { get; set; }
        public Nullable<int> Course_ID { get; set; }
        public Nullable<int> Specification_ID { get; set; }
        public string Semester_ID { get; set; }
    
        public virtual Course Course { get; set; }
        public virtual Specification Specification { get; set; }
        public virtual Student Student { get; set; }
    }
}
