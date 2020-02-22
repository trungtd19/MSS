using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Components
{
    public class SpecificationsRepository : GenericRepository<Specification>
    {
        public SpecificationsRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<Specification> GetPageList()
        {
            List<Specification> cour = new List<Specification>();
            using (MSSEntities db = new MSSEntities())
            {
                cour = (from o in db.Specifications
                        orderby o.Specification_ID descending
                        select o)
                          .ToList();

                return cour;
            }

        }
    }
}