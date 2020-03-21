using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using PagedList;
using MSS_DEMO.Common;

namespace MSS_DEMO.Controllers
{
    [CheckCredential(Role_ID = "3")]
    public class ReportController : Controller
    {
        // GET: Report

        public ActionResult Index(Report rp)
        {
            List<Report> rep = new List<Report>();
            List<Report> rep2= new List<Report>();      

            var context = new MSSEntities();

            foreach (var sub in context.Subjects)
            {
                var Totall = (from a in context.Subjects
                              join b in context.Subject_Student on a.Subject_ID equals b.Subject_ID
                              join c in context.Students on b.Roll equals c.Roll
                             where sub.Subject_ID == a.Subject_ID
                             select c.Roll).Count();

                var Type = (from a in context.Specifications
                            where sub.Subject_ID == a.Subject_ID && a.Is_Real_Specification == true
                            select a.Specification_ID).Count();
                String ListType = "";
                if (Type == 1)
                {
                    ListType = "Spec";
                }
                else
                {
                    ListType = "Course";
                }

                List<double> count = new List<double>();
                List<string> name = new List<string>();

                foreach (var cp in context.Campus)
                {
                    count.Add(Campus(sub.Subject_ID, cp.Campus_ID));
                    name.Add(cp.Campus_ID);
                }
                ViewBag.Name = name;
                ViewBag.Cmp = count;

                rep.Add(new Report {Sub = sub.Subject_ID, Name = sub.Subject_Name, Type = ListType, Total = Totall, Cmp = count });

                List<double> count2 = new List<double>();
                int Total = Count(sub.Subject_ID, "");

                foreach(var cp in context.Campus)
                {
                    count2.Add(Count(sub.Subject_ID, cp.Campus_ID));
                }
                rep2.Add(new Report { Sub = sub.Subject_ID, Name = sub.Subject_Name, Study = percent(Total, Totall),Total = Total, Cmp = count2 });
            }
            var TotalStudent = (from a in context.Students
                                select a.Roll).Count();
            List<double> temp = new List<double>();
            temp.Add(Campus("", ""));
            foreach (var cp in context.Campus)
            {
                temp.Add(Campus("",cp.Campus_ID));
            }
            ViewBag.Count = temp;
            ViewBag.TotalStudent1 = TotalStudent;
            int TotalStudent2 = Count("", "");
            ViewBag.TotalStudent2 = TotalStudent2;


            List<double> temp2 = new List<double>();
            ViewBag.TotalPercent = percent(TotalStudent2, TotalStudent);
            temp2.Add(Count("", ""));
            foreach (var cp in context.Campus)
            {
                temp2.Add(Count("", cp.Campus_ID));
            }
            ViewBag.Count2 = temp2;

            List<double> per = new List<double>();
            //per.Add(percent(Count("", ""), Campus("", "")));
            foreach (var cp in context.Campus)
            {
                per.Add(percent(Count("", cp.Campus_ID), Campus("", cp.Campus_ID)));
            }
            ViewBag.Per = per;

            var studentComplete = (from a in context.Student_Course_Log
                                   where a.Completed == true && a.Course_ID != null
                                   select a.Roll).Distinct().Count();
            var courseComplete = (from a in context.Student_Course_Log
                                  where a.Completed == true && a.Course_ID != null
                                  select a.Course_ID).Count();

            ViewBag.studentComplete = studentComplete;
            ViewBag.courseComplete = courseComplete;


            var Estimated = (from a in context.Student_Course_Log
                             where a.Estimated < 5
                             select a.Roll).Distinct().Count();
            var perc = percent(Estimated, TotalStudent);
            ViewBag.Estimated = Estimated;
            ViewBag.Percent = perc;

            rp.rp1 = rep;
            rp.rp2 = rep2;
            return View("Index", rp);
        }

        public ActionResult Enrollment(ListStudent ls,string searchString, string searchCheck, string selectString2, string selectString3)
        {
            var context = new MSSEntities();
            List<string> NoEnrollment = new List<string>();
            List<ListStudent> s = new List<ListStudent>();

            var CourseStudent = (from a in context.Student_Course_Log
                                 select new
                                 {
                                     Roll = a.Roll
                                 }).Distinct().ToList();
            var Student = (from a in context.Students
                           select new
                           {
                               Roll = a.Roll
                           }).Distinct().ToList();
            foreach(var cs in Student)
            {
                int temp = 0;
                foreach(var st in CourseStudent)
                {
                    if (cs.Roll.Equals(st.Roll))
                    {
                        temp = 0;
                        break;
                    }
                    else
                    {
                        temp = 1;
                    }
                }
                if(temp == 1)
                {
                    NoEnrollment.Add(cs.Roll);
                }
                
            }

            if (!String.IsNullOrEmpty(searchCheck))
            {
                int rowNo = 0;
                foreach (var b in NoEnrollment)
                {
                    List<ListStudent> infoStudent = (from c in context.Students
                                                     where c.Roll == b
                                                     select new
                                                     {
                                                         Email = c.Email,
                                                         Roll = c.Roll,
                                                         Full_Name = c.Full_Name,
                                                         Campus = c.Campus
                                                     }).ToList().Select(p => new ListStudent
                                                     {
                                                         STT = rowNo++,
                                                         Email = p.Email,
                                                         Roll = p.Roll,
                                                         Full_Name = p.Full_Name,
                                                         Campus = p.Campus
                                                     }).ToList();
                    List<string> subjectStudent =   (from c in context.Students
                                                     join d in context.Subject_Student on c.Roll equals d.Roll
                                                     join e in context.Subjects on d.Subject_ID equals e.Subject_ID
                                                     where c.Roll == b
                                                     select e.Subject_Name).ToList();

                                
                    foreach (var i in infoStudent)
                    {
                        s.Add(new ListStudent { STT = rowNo, Email = i.Email, Roll = i.Roll, Full_Name = i.Full_Name, Campus = i.Campus, ListSubject = subjectStudent });
                    }

                }
                if (!String.IsNullOrEmpty(searchString))
                {
                    s = s.Where(a => a.Roll.Contains(searchString)).ToList();        
                }
                if (!String.IsNullOrEmpty(selectString2))
                {
                    s = s.Where(a => a.Campus.Contains(selectString2)).ToList();
                }
                if (!String.IsNullOrEmpty(selectString3))
                {
                    s = s.Where(a => a.ListSubject.Contains(selectString3)).ToList();
                }
            }

            var listCampus = (from a in context.Campus
                              select a.Campus_ID).ToList();
            var listSubject = (from a in context.Subjects
                               select a.Subject_Name).ToList();
            List<SelectListItem> selectCp = new List<SelectListItem>();
            List<SelectListItem> selectSj = new List<SelectListItem>();
            selectCp.Add(new SelectListItem
            {
                Text = "--Select Campus--",
                Value = ""
            });
            foreach (var a in listCampus)
            {
                selectCp.Add(new SelectListItem
                {
                    Text = a,
                    Value = a
                });
            }

            selectSj.Add(new SelectListItem
            {
                Text = "--Select Subject--",
                Value = ""
            });
            foreach (var a in listSubject)
            {
                selectSj.Add(new SelectListItem
                {
                    Text = a,
                    Value = a
                });
            }
            ViewBag.SelectString3 = selectSj;
            ViewBag.SelectString2 = selectCp;
            ls.ls1 = s;

            ViewBag.TotalSearch = s.Count();
            //rp.Students = s;
            return View("Enrollment", ls);
        }

        public ActionResult Member(ListStudent ls, string searchString, string searchCheck, string selectString2, string selectString3)
        {
            var context = new MSSEntities();
            List<string> NoEnrollment = new List<string>();
            List<ListStudent> s = new List<ListStudent>();

            var CourseStudent = (from a in context.Student_Course_Log
                                 select new
                                 {
                                     Roll = a.Roll
                                 }).Distinct().ToList();
            var Student = (from a in context.Student_Course_Log
                           join b in context.Courses on a.Course_ID equals b.Course_ID
                           join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                           join d in context.Subjects on c.Subject_ID equals d.Subject_ID
                           select new
                           {
                               Roll = a.Roll
                           }).Distinct().ToList();
            foreach (var cs in CourseStudent)
            {
                int temp = 0;
                foreach (var st in Student)
                {
                    if (cs.Roll.Equals(st.Roll))
                    {
                        temp = 0;
                        break;
                    }
                    else
                    {
                        temp = 1;
                    }
                }
                if (temp == 1)
                {
                    NoEnrollment.Add(cs.Roll);
                }

            }
            if (!String.IsNullOrEmpty(searchCheck))
            {
                int rowNo = 0;
                foreach (var b in NoEnrollment)
                {
                    List<ListStudent> infoStudent = (from c in context.Students
                                                     where c.Roll == b
                                                     select new
                                                     {
                                                         Email = c.Email,
                                                         Roll = c.Roll,
                                                         Full_Name = c.Full_Name,
                                                         Campus = c.Campus
                                                     }).ToList().Select(p => new ListStudent
                                                     {
                                                         STT = rowNo++,
                                                         Email = p.Email,
                                                         Roll = p.Roll,
                                                         Full_Name = p.Full_Name,
                                                         Campus = p.Campus
                                                     }).ToList();
                    List<string> subjectStudent = (from c in context.Students
                                                   join d in context.Subject_Student on c.Roll equals d.Roll
                                                   join e in context.Subjects on d.Subject_ID equals e.Subject_ID
                                                   where c.Roll == b
                                                   select e.Subject_Name).ToList();


                    foreach (var i in infoStudent)
                    {
                        s.Add(new ListStudent { STT = rowNo, Email = i.Email, Roll = i.Roll, Full_Name = i.Full_Name, Campus = i.Campus, ListSubject = subjectStudent });
                    }

                }
                if (!String.IsNullOrEmpty(searchString))
                {
                    s = s.Where(a => a.Roll.Contains(searchString)).ToList();
                }
                if (!String.IsNullOrEmpty(selectString2))
                {
                    s = s.Where(a => a.Campus.Contains(selectString2)).ToList();
                }
                if (!String.IsNullOrEmpty(selectString3))
                {
                    s = s.Where(a => a.ListSubject.Contains(selectString3)).ToList();
                }
            }
            var listCampus = (from a in context.Campus
                              select a.Campus_ID).ToList();
            var listSubject = (from a in context.Subjects
                              select a.Subject_Name).ToList();
            List<SelectListItem> selectCp = new List<SelectListItem>();
            List<SelectListItem> selectSj = new List<SelectListItem>();
            selectCp.Add(new SelectListItem
            {
                Text = "--Select Campus--",
                Value = ""
            });
            foreach (var a in listCampus)
            {
                selectCp.Add(new SelectListItem
                {
                    Text = a,
                    Value = a
                });
            }

            selectSj.Add(new SelectListItem
            {
                Text = "--Select Subject--",
                Value = ""
            });
            foreach (var a in listSubject)
            {
                selectSj.Add(new SelectListItem
                {
                    Text = a,
                    Value = a
                });
            }
            ViewBag.SelectString3 = selectSj;
            ViewBag.SelectString2 = selectCp;
            ls.ls1 = s;
            ViewBag.TotalSearch = s.Count();
            //rp.Students = s;
            return View("Member", ls);
        }

        public ActionResult NotRequiredCourse(ListNotRequiredCourse lc, string SelectString, string searchCheck)
        {
            var context = new MSSEntities();
            List<ListNotRequiredCourse> listNotRequiredCourses = new List<ListNotRequiredCourse>();

            var NotRequired = (from a in context.Student_Course_Log
                               where a.Course_ID == null
                               select a.Course_Name).Distinct().ToList();

           

            if (!String.IsNullOrEmpty(searchCheck))
            {
                foreach (var a in NotRequired)
                {
                    var completed = (from b in context.Student_Course_Log
                                     where b.Course_Name == a && b.Completed == true && b.Course_ID == null
                                     select b.Roll).Distinct().Count();
                    var notCompleted = (from b in context.Student_Course_Log
                                        where b.Course_Name == a && b.Completed == false && b.Course_ID == null
                                        select b.Roll).Distinct().Count();
                    listNotRequiredCourses.Add(new ListNotRequiredCourse { Name = a, Complelted = completed, NotComplelted = notCompleted });
                }

                if (!String.IsNullOrEmpty(SelectString))
                {
                    if(SelectString == "1")
                    {
                        listNotRequiredCourses = listNotRequiredCourses.Where(a => (a.Complelted > 0)).ToList();
                    }
                    if (SelectString == "2")
                    {
                        listNotRequiredCourses = listNotRequiredCourses.Where(a => (a.Complelted == 0)).ToList();
                    }

                }
            }

            List<SelectListItem> selectCompleted = new List<SelectListItem>();
            selectCompleted.Add(new SelectListItem { Text = "--Select Completed--", Value = ""});
            selectCompleted.Add(new SelectListItem { Text = "Completed", Value = "1" });
            selectCompleted.Add(new SelectListItem { Text = "Not Completed", Value = "2" });

            ViewBag.SelectString = selectCompleted;
            lc.lc1 = listNotRequiredCourses;

            ViewBag.TotalSearch = listNotRequiredCourses.Count();
            //rp.Students = s;
            return View("NotRequiredCourse", lc);
        }



        private int Count(string subject, string campus)
        {
            //double per;
            int count = 0;
            var context = new MSSEntities();
            if (subject != "" && campus == "")
            {
                count = (from a in context.Student_Course_Log
                         join b in context.Courses on a.Course_ID equals b.Course_ID
                         join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                         join d in context.Subjects on c.Subject_ID equals d.Subject_ID
                         where subject == a.Subject_ID
                         select a.Roll).Distinct().Count();
            }
            else if (subject == "" && campus == "")
            {
                count = (from a in context.Student_Course_Log
                         join b in context.Courses on a.Course_ID equals b.Course_ID
                         join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                         join d in context.Subjects on c.Subject_ID equals d.Subject_ID
                         select a.Roll).Distinct().Count();
            }
            else if(subject != "" && campus != "")
            {
                count = (from a in context.Student_Course_Log
                         join b in context.Courses on a.Course_ID equals b.Course_ID
                         join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                         join d in context.Subjects on c.Subject_ID equals d.Subject_ID
                         where  a.Campus == campus && a.Subject_ID == subject
                         select a.Roll).Distinct().Count();
            }
            else if(subject == "" && campus != "")
            {
                count = (from a in context.Student_Course_Log
                         join b in context.Courses on a.Course_ID equals b.Course_ID
                         join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                         join d in context.Subjects on c.Subject_ID equals d.Subject_ID
                         where a.Campus == campus
                         select a.Roll).Distinct().Count();
            }
            return count;
        }

        private double percent(int a, int b)
        {
            double per;
            if (b > 0)
            {
                per = Math.Round(((double)a / (double)b) * 100);
            }
            else
            {
                per = 0;
            }
            return per;
        }

        private int Campus(string subjectId, string campus)
        {
            var context = new MSSEntities();
            var student = 0;
            if (subjectId == "" && campus != "")
            {
                student = (from a in context.Subjects
                               join b in context.Subject_Student on a.Subject_ID equals b.Subject_ID
                               join c in context.Students on b.Roll equals c.Roll
                               where c.Campus == campus
                               select c.Roll).Count();
            }
            else if(subjectId == "" && campus == "")
            {
                student = (from a in context.Subjects
                           join b in context.Subject_Student on a.Subject_ID equals b.Subject_ID
                           join c in context.Students on b.Roll equals c.Roll
                           select c.Roll).Count();
            }
            else
            {
                student = (from a in context.Subjects
                               join b in context.Subject_Student on a.Subject_ID equals b.Subject_ID
                               join c in context.Students on b.Roll equals c.Roll
                               where subjectId == a.Subject_ID && c.Campus == campus
                               select c.Roll).Count();
            }
            
            return student;
        }
    }
}
