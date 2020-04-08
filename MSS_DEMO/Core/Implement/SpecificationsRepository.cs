using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Components
{
    public class SpecificationsRepository : BaseRepository<Specification>
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
        public List<Specification> GetListID()
        {
            List<Specification> spec = new List<Specification>();
            using (MSSEntities db = new MSSEntities())
            {
                spec = (from sp in db.Specifications     
                        join su in db.Subjects on sp.Subject_ID equals su.Subject_ID
                       select new Specification { Subject_ID = su.Subject_ID , Specification_ID = sp.Specification_ID }).ToList();
                return spec;
            }
        }
        public bool IsExitsSpec(string spec_Name)
        {
            bool check = true;
            Specification student = context.Specifications.Where(x => x.Specification_Name == spec_Name).FirstOrDefault();
            if (student != null)
            {
                check = true;
            }
            else
                check = false;
            return check;
        }

    }
}