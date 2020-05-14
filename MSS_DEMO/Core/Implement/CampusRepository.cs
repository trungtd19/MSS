using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Components
{
    public class CampusRepository : BaseRepository<Campu>
    {
        public CampusRepository(MSSEntities context)
           : base(context)
        {
        }
        public bool IsExitsCampus(string campusID, string campusName)
        {
            bool check = true;
            Campu cam = context.Campus.Where(x => x.Campus_ID == campusID || x.Campus_Name == campusName).FirstOrDefault();
            if (cam != null)
            {
                check = true;
            }
            else
                check = false;
            return check;
        }
        public bool IsExitsCampusEdit(string campusID)
        {
            bool check = true;
            Campu cam = context.Campus.Where(x => x.Campus_ID == campusID).FirstOrDefault();
            if (cam != null)
            {
                check = true;
            }
            else
                check = false;
            return check;
        }
        public bool IsExitsCampusID(string campusID)
        {
            bool check = true;
            Campu cam = context.Campus.Where(x => x.Campus_ID == campusID).FirstOrDefault();
            if (cam != null)
            {
                check = true;
            }
            else
                check = false;
            return check;
        }
    }
}