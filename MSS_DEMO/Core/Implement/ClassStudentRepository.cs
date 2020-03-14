using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Implement
{
    public class ClassStudentRepository : BaseRepository<Class_Student>
    {
        public ClassStudentRepository (MSSEntities context)
           : base(context)
        {
        }
    }
}