﻿using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Core.Implement
{
    public class SubjectRepository : BaseRepository<Subject>
    {
        public SubjectRepository(MSSEntities context)
           : base(context)
        {
        }
        public List<Subject> GetPageList()
        {
            List<Subject> cour = new List<Subject>();
            using (MSSEntities db = new MSSEntities())
            {
                cour = (from o in db.Subjects
                        orderby o.Subject_ID descending
                        select o)
                       .ToList<Subject>();

                return cour;
            }

        }
        public bool IsExitsSubject(string id)
        {
            bool check = true;
            Subject subject = context.Subjects.Where(x => x.Subject_ID == id).FirstOrDefault();
            if (subject != null)
            {
                check = true;
            }
            else
                check = false;
            return check;
        }
        public bool CheckExitsSubject(string id)
        {
            bool check = true;
            Subject subject = context.Subjects.Where(x => x.Subject_ID == id).FirstOrDefault();
            if (subject == null)
            {
                check = false;
                throw new Exception("Môn học " + id + " không tồn tại!");
            }
            else
                check = true;
            return check;
        }

        public bool Save(string Subject_Name)
        {
            bool returnValue = true;
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.SaveChanges();
                    var subject = context.Subjects.Where(x => x.Subject_Name.Trim() == Subject_Name.Trim()).ToList().Count();
                    if (subject > 1)
                    {
                        returnValue = false;
                        dbContextTransaction.Rollback();
                        throw new Exception("Subject exited");
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