﻿using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MSS_DEMO.Core.Implement
{
    public class SubjectStudentRepository : BaseRepository<Subject_Student>
    {
        public SubjectStudentRepository(MSSEntities context)
           : base(context)
        {
        }
        public string DeleteListSubject(string Subject_ID, string SemesterID, string CampusID)
        {
            int countDelete = 0;
            string message = "";
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {            
                try
                {
                    var student = context.Students.ToList();
                    var listSubStudennt = context.Subject_Student.ToList();
                    var list = listSubStudennt.Where(x => x.Subject_ID == Subject_ID && x.Semester_ID == SemesterID).ToList();
                    List<string> listRoll = list.Select(x => x.Roll).ToList();
                    foreach (var lst in list)
                    {
                        var sub = context.Subject_Student.Find(lst.ID);
                        context.Subject_Student.Remove(sub);
                    }
                    listSubStudennt = context.Subject_Student.ToList();
                    foreach (var ls in listRoll)
                    {
                        if (listSubStudennt.Where(x => x.Roll == ls && x.Semester_ID == SemesterID).FirstOrDefault() == null)
                        {
                            var stu = context.Students.Find(ls, SemesterID);
                            if (stu.Campus_ID == CampusID)
                            {
                                context.Students.Remove(stu);
                                countDelete++;
                            }               
                        }
                        else continue;
                    }
                    context.SaveChanges();
                    if (countDelete == 0)
                    {
                        message = "Delete fail";
                        dbContextTransaction.Rollback();
                    }
                    else
                    {
                        dbContextTransaction.Commit();
                        message =  "Delete success " + countDelete + " students";
                    }                 
                }
                catch (Exception ex)
                {
                    message = "Can't delete students";
                    dbContextTransaction.Rollback();
                }
            }
            return message;
        }
        public bool IsExitsSubjectStudent(string roll, string subject_id)
        {
            var subStudent = context.Subject_Student.Where(o => o.Roll == roll && o.Subject_ID == subject_id).FirstOrDefault();
            if (subStudent != null) return true;
            return false;
        }

        public StringBuilder getListSubject(string id)
        {
            string Roll = id.Split('^')[0];
            string SemesterID = id.Split('^')[1];
            StringBuilder sb = new StringBuilder();
            var subject = context.Subject_Student.Where(s => s.Roll == Roll && s.Semester_ID == SemesterID).Select(s => s.Subject_ID).ToList();
            foreach (var _subject in subject)
            {
                sb.Append("-" + _subject);
            }
            if (!string.IsNullOrEmpty(sb.ToString())) sb.Remove(0, 1);
            return sb;
        }
    }
}