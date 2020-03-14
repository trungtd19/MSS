using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Implement
{
    public class SubjectStudentRepository : BaseRepository<Subject_Student>
    {
        public SubjectStudentRepository (MSSEntities context)
           : base(context)
        {
        }
    }
}