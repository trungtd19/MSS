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

namespace MSS_DEMO.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report

        public ActionResult Index(Report rp)
        {
            List<Report> rep = new List<Report>();
            List<Report2> rep2= new List<Report2>();
            List<Report3> rep3 = new List<Report3>();

            var context = new MSSEntities();

            foreach (var sub in context.Subjects)
            {
                var Totall = (from a in context.Subjects
                              join b in context.Subject_Student on a.Subject_ID equals b.Subject_ID
                              join c in context.Students on b.Roll equals c.Roll
                             where sub.Subject_ID == a.Subject_ID
                             select c.Roll).Count();

                var HaNoi = Campus(sub.Subject_ID, "HN");
                var DaNang = Campus(sub.Subject_ID, "DN");
                var SaiGon = Campus(sub.Subject_ID, "SG");
                var CanTho = Campus(sub.Subject_ID, "CT");

                var Type = (from a in context.Specifications
                            where sub.Subject_ID == a.Subject_ID && a.Is_Real_Specification == true
                            select a.Specification_ID).Count();
                String ListType = "";
                if(Type == 1)
                {
                    ListType = "Spec";
                }
                else
                {
                    ListType = "Course";
                }

                rep.Add(new Report { Sub = sub.Subject_ID, Name = sub.Subject_Name, Type = ListType, Total = Totall, HN = HaNoi, DN = DaNang, SG = SaiGon, CT = CanTho });


                int Total = Count(sub.Subject_ID, "");
                int HaNoiStudy = Count(sub.Subject_ID, "HN");
                int DaNangStudy = Count(sub.Subject_ID, "DN");
                int SaiGonStudy = Count(sub.Subject_ID, "SG");
                int CanThoStudy = Count(sub.Subject_ID, "CT");
                rep2.Add(new Report2 { Sub = sub.Subject_ID, Name = sub.Subject_Name, Study = percent(Total,Totall),Total = Total, HN = HaNoiStudy, DN = DaNangStudy, SG = SaiGonStudy, CT = CanThoStudy });

            }

            int Total1 = rep.Sum(x => x.Total);
            int HN1 = rep.Sum(x => x.HN);
            int DN1 = rep.Sum(x => x.DN);
            int SG1 = rep.Sum(x => x.SG);
            int CT1 = rep.Sum(x => x.CT);

            int Total2 = rep2.Sum(x => x.Total);
            double HN2 = rep2.Sum(x => x.HN);
            double DN2 = rep2.Sum(x => x.DN);
            double SG2 = rep2.Sum(x => x.SG);
            double CT2 = rep2.Sum(x => x.CT);

            ViewBag.Total1 = Total1;
            ViewBag.Total2 = Total2;

            var studentComplete = (from a in context.Student_Course_Log
                                   join b in context.Courses on a.Course_ID equals b.Course_ID
                                   join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                                   where a.Completed == true
                                   select a.Roll).Distinct().Count();
            var courseComplete = (from a in context.Student_Course_Log
                                   join b in context.Courses on a.Course_ID equals b.Course_ID
                                   join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                                   where a.Completed == true
                                   select a.Course_ID).Count();

            ViewBag.studentComplete = studentComplete;
            ViewBag.courseComplete = courseComplete;


            var Estimated = (from a in context.Student_Course_Log
                             where a.Estimated < 5
                             select a.Roll).Distinct().Count();
            var per = percent(Estimated, Total1);
            ViewBag.Estimated = Estimated;
            ViewBag.Percent = per;
            

            rep.Add(new Report { Total = Total1, HN = HN1, DN = DN1, SG = SG1, CT = CT1 });
            rep2.Add(new Report2 { Study = percent(Total2,Total1), Total = Total2, HN = HN2, DN = DN2, SG = SG2, CT = CT2 });
            rep3.Add(new Report3 { Title = "% Vào học",HN = percent((int)HN2,HN1), DN = percent((int)DN2,DN1),
            SG = percent((int)SG2,SG1), CT = percent((int)CT2,CT1)});
            rp.Info = rep;
            rp.Info2 = rep2;
            rp.Info3 = rep3;
            return View("Index", rp);
        }

        public ActionResult Enrollment(int? page, string searchString, string searchCheck)
        {
            var context = new MSSEntities();
            List<string> NoEnrollment = new List<string>();
            List<Student> s = new List<Student>();

            if(page == null)
            {
                page = 1;
            }
            int pageSize = 30;
            int pageNumber = (page ?? 1);
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
                foreach (var b in NoEnrollment)
                {
                    List<Student> infoStudent = (from c in context.Students
                                                 where c.Roll == b
                                                 select new
                                                 {
                                                     Email = c.Email,
                                                     Roll = c.Roll,
                                                     Full_Name = c.Full_Name,
                                                     Campus = c.Campus,
                                                 }).ToList().Select(p => new Student
                                                 {
                                                     Email = p.Email,
                                                     Roll = p.Roll,
                                                     Full_Name = p.Full_Name,
                                                     Campus = p.Campus
                                                 }).ToList();
                    foreach (var i in infoStudent)
                    {
                        s.Add(new Student { Email = i.Email, Roll = i.Roll, Full_Name = i.Full_Name, Campus = i.Campus });
                    }

                }
                if (!String.IsNullOrEmpty(searchString))
                {
                    s = s.Where(a => a.Roll.Contains(searchString)).ToList();
                }
            }
            //rp.Students = s;
            return View("Enrollment", s.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Member(int? page, string searchString, string searchCheck)
        {
            var context = new MSSEntities();
            List<string> NoEnrollment = new List<string>();
            List<Student> s = new List<Student>();

            if (page == null)
            {
                page = 1;
            }
            int pageSize = 30;
            int pageNumber = (page ?? 1);
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
                foreach (var b in NoEnrollment)
                {
                    List<Student> infoStudent = (from c in context.Students
                                                 where c.Roll == b
                                                 select new
                                                 {
                                                     Email = c.Email,
                                                     Roll = c.Roll,
                                                     Full_Name = c.Full_Name,
                                                     Campus = c.Campus,
                                                 }).ToList().Select(p => new Student
                                                 {
                                                     Email = p.Email,
                                                     Roll = p.Roll,
                                                     Full_Name = p.Full_Name,
                                                     Campus = p.Campus
                                                 }).ToList();
                    foreach (var i in infoStudent)
                    {
                        s.Add(new Student { Email = i.Email, Roll = i.Roll, Full_Name = i.Full_Name, Campus = i.Campus });
                    }

                }
                if (!String.IsNullOrEmpty(searchString))
                {
                    s = s.Where(a => a.Roll.Contains(searchString)).ToList();
                }
            }
            //rp.Students = s;
            return View("Member", s.ToList().ToPagedList(pageNumber, pageSize));
        }

        private int Count(string a1, string a2)
        {
            //double per;
            int count = 0;
            var context = new MSSEntities();
            if (a2 == "")
            {
                count = (from a in context.Student_Course_Log
                         join b in context.Courses on a.Course_ID equals b.Course_ID
                         join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                         join d in context.Subjects on c.Subject_ID equals d.Subject_ID
                         where a1 == d.Subject_ID
                         select a.Roll).Distinct().Count();
            }
            else
            {
                count = (from a in context.Student_Course_Log
                         join b in context.Courses on a.Course_ID equals b.Course_ID
                         join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                         join d in context.Subjects on c.Subject_ID equals d.Subject_ID
                         where a1 == d.Subject_ID && a.Campus == a2
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
            var student = (from a in context.Subjects
                         join b in context.Subject_Student on a.Subject_ID equals b.Subject_ID
                         join c in context.Students on b.Roll equals c.Roll
                         where subjectId == a.Subject_ID && c.Campus == campus
                         select c.Roll).Count();
            return student;
        }
    }
}
