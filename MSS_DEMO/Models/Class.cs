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
    
    public partial class Class
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Class()
        {
            this.Class_Student = new HashSet<Class_Student>();
        }
    
        public string Class_ID { get; set; }
        public string University { get; set; }
        public Nullable<System.DateTime> Class_Start_Time { get; set; }
        public string Enrollment_Source { get; set; }
        public string Mentor_ID { get; set; }
        public string Semester_ID { get; set; }
        public string Campus_Name { get; set; }
    
        public virtual Campu Campu { get; set; }
        public virtual Mentor Mentor { get; set; }
        public virtual Semester Semester { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Class_Student> Class_Student { get; set; }
    }
}
