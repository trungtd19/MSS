using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Components
{
    public class CampusRepository : GenericRepository<Campu>
    {
        public CampusRepository(MSSEntities context)
           : base(context)
        {
        }
    }
}