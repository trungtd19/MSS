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
                                   select a.Course_ID).Distinct().Count();

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



        private int Count(string a1, string a2)
        {
            //double per;
            int count = 0;
            var context = new MSSEntities();
            if (a2 == "")
            {
                List<SubExtend> rp = (from a in context.Student_Specification_Log
                                      join b in context.Subjects on a.Subject_ID equals b.Subject_ID
                                      where a1 == a.Subject_ID
                                      select new
                                      {
                                          Specialization_Slug = a.Specialization_Slug,
                                          Subject_Name = b.Subject_Name
                                      }).ToList().Select(p => new SubExtend
                                      {
                                          Specialization_Slug = p.Specialization_Slug,
                                          Subject_Name = p.Subject_Name
                                      }).ToList();
                foreach (var item in rp)
                {
                    string s1 = item.Subject_Name.Trim().ToLower();
                    string s2 = item.Specialization_Slug.Replace("-", " ").Trim().ToLower();
                    if (s1.Equals(s2))
                    {
                        count++;
                    }
                }
            }
            else
            {
                List<SubExtend> rp = (from a in context.Student_Specification_Log
                                      join b in context.Subjects on a.Subject_ID equals b.Subject_ID
                                      where a1 == a.Subject_ID && a.Campus == a2
                                      select new
                                      {
                                          Specialization_Slug = a.Specialization_Slug,
                                          Subject_Name = b.Subject_Name
                                      }).ToList().Select(p => new SubExtend
                                      {
                                          Specialization_Slug = p.Specialization_Slug,
                                          Subject_Name = p.Subject_Name
                                      }).ToList();
                foreach (var item in rp)
                {
                    string s1 = item.Subject_Name.Trim().ToLower();
                    string s2 = item.Specialization_Slug.Replace("-", " ").Trim().ToLower();
                    if (s1.Equals(s2))
                    {
                        count++;
                    }
                }
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
