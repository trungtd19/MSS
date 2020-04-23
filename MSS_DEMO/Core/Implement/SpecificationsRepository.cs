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
        public bool Save(string Specification_Name)
        {
            bool returnValue = true;
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.SaveChanges();
                    var spec = context.Specifications.Where(x => x.Specification_Name.Trim() == Specification_Name.Trim()).ToList().Count();
                    if (spec > 1)
                    {
                        returnValue = false;
                        dbContextTransaction.Rollback();
                        throw new Exception("Specifications exited");
                    }
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    returnValue = false;
                    dbContextTransaction.Rollback();
                }
            }
            return returnValue;
        }

    }
}