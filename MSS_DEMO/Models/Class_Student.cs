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
    
    public partial class Class_Student
    {
        public int ID { get; set; }
        public string Roll { get; set; }
        public string Class_ID { get; set; }
    
        public virtual Class Class { get; set; }
        public virtual Student Student { get; set; }
    }
}