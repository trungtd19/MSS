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
using System.Globalization;
using Rotativa;
using Microsoft.Ajax.Utilities;

namespace MSS_DEMO.Controllers
{
    [CheckCredential(Role_ID = "3")]
    public class ReportController : Controller
    {
       
        // GET: Report       
        public ActionResult Index(Report rp, string SelectDatetime, string searchCheck, string weekNumber, string SelectSemester)
        {
            List<Report> reportStudent = new List<Report>();
            List<Report> reportCourse= new List<Report>();

            var context = new MSSEntities();

            List<SelectListItem> selectSemes = new List<SelectListItem>();

            var listSemes = (from a in context.Semesters
                             select a).ToList();
            var orderedListSemes = listSemes.OrderBy(x => x.Start_Date).ToList();
            foreach (var a in orderedListSemes)
            {
                selectSemes.Add(new SelectListItem
                {
                    Text = a.Semester_Name,
                    Value = a.Semester_ID
                });
            }
            ViewBag.SelectSemester = selectSemes;

            if (String.IsNullOrEmpty(searchCheck) && orderedListSemes.Count > 0)
            {
                SelectSemester = (from a in context.Semesters
                                  where a.Start_Date < DateTime.Now && a.End_Date > DateTime.Now
                                  select a.Semester_ID).FirstOrDefault();
            }

            DateTime date;
            ViewBag.SelectDatetime = Date(SelectSemester);
            date = Convert.ToDateTime(Date(SelectSemester).Select(m => m.Value).FirstOrDefault());
            if (SelectDatetime != null)
            {
                date = Convert.ToDateTime(SelectDatetime);
            }
            if (searchCheck == "1")
            {
                List<string> dateL = new List<string>();
                dateL.Add(Convert.ToDateTime(date).ToString("dd/MM/yyyy"));
                ViewBag.SelectDatetime = new SelectList(dateL);
            }
            if (!String.IsNullOrEmpty(searchCheck))
            {
                var listSub = (from a in context.Subjects
                               where a.Subject_Active == true
                               select a).ToList();
                foreach (var sub in listSub)
                {
                    var Totall = (from a in context.Subjects
                                  join b in context.Subject_Student on a.Subject_ID equals b.Subject_ID
                                  join c in context.Students on b.Roll equals c.Roll
                                  where sub.Subject_ID == a.Subject_ID && c.Semester_ID == SelectSemester && b.Semester_ID == SelectSemester
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
                        count.Add(Campus(sub.Subject_ID, cp.Campus_ID, SelectSemester));
                        name.Add(cp.Campus_ID);
                    }
                    ViewBag.Name = name;
                    ViewBag.Cmp = count;

                    reportStudent.Add(new Report { Sub = sub.Subject_ID, Name = sub.Subject_Name, Type = ListType, Total = Totall, Cmp = count });

                    List<double> count2 = new List<double>();
                    if (date != null)
                    {
                        int Total = Count(sub.Subject_ID, "", date);

                        foreach (var cp in context.Campus)
                        {
                            count2.Add(Count(sub.Subject_ID, cp.Campus_ID, date));
                        }
                        reportCourse.Add(new Report { Sub = sub.Subject_ID, Name = sub.Subject_Name, Study = percent(Total, Totall), Total = Total, Cmp = count2 });
                    }

                }
                var TotalStudent = (from a in context.Students
                                    where a.Semester_ID == SelectSemester
                                    select a.Roll).Count();
                List<double> temp = new List<double>();
                temp.Add(Campus("", "", SelectSemester));
                foreach (var cp in context.Campus)
                {
                    temp.Add(Campus("", cp.Campus_ID, SelectSemester));
                }
                ViewBag.Count = temp;
                ViewBag.TotalStudent1 = TotalStudent;
                int TotalStudent2 = Count("", "", date);
                ViewBag.TotalStudent2 = TotalStudent2;


                List<double> temp2 = new List<double>();
                ViewBag.TotalPercent = percent(TotalStudent2, TotalStudent);
                temp2.Add(Count("", "", date));
                foreach (var cp in context.Campus)
                {
                    temp2.Add(Count("", cp.Campus_ID, date));
                }
                ViewBag.Count2 = temp2;

                List<double> per = new List<double>();
                //per.Add(percent(Count("", ""), Campus("", "")));
                foreach (var cp in context.Campus)
                {
                    per.Add(percent(Count("", cp.Campus_ID, date), Campus("", cp.Campus_ID, SelectSemester)));
                }
                ViewBag.Per = per;

                var studentComplete = (from a in context.Student_Course_Log
                                       where a.Completed == true && a.Course_ID != null && a.Date_Import == date && a.Semester_ID == SelectSemester
                                       select a.Roll).Distinct().Count();
                var courseComplete = (from a in context.Student_Course_Log
                                      where a.Completed == true && a.Course_ID != null && a.Date_Import == date && a.Semester_ID == SelectSemester
                                      select a.Course_ID).Count();

                ViewBag.studentComplete = studentComplete;
                ViewBag.courseComplete = courseComplete;

                var RollList = (from a in context.Students
                                where a.Semester_ID == SelectSemester
                                select a.Roll).ToList();
                int CountStudent = 0;
                //DateTime nowDate = DateTime.Now;
                var starDate = (from a in context.Semesters
                                where a.Semester_ID == SelectSemester
                                select a.Start_Date).FirstOrDefault();
                TimeSpan timeSpan = date - (DateTime)starDate;
                //var weekN = Math.Floor((double)(date.Day - starDate.Value.Day) / 7);
                double weekN = 0;
                if (date != DateTime.MinValue)
                {
                    weekN = Math.Floor(timeSpan.TotalDays / 7);
                }
                //ViewBag.Week = weekN;
                int weekOff = 0;
                if (!String.IsNullOrEmpty(weekNumber))
                {
                    weekOff = Int32.Parse(weekNumber);
                }
                ViewBag.weekNumber = weekOff;
                ViewBag.date = date;
                ViewBag.semester = SelectSemester;
                var weekTotal = weekN - weekOff;
                if (weekTotal > 0)
                {
                    foreach (var t in RollList)
                    {
                        var Estimated = (from a in context.Student_Course_Log
                                         where a.Roll == t && a.Course_ID != null && a.Date_Import == date && a.Semester_ID == SelectSemester
                                         select a.Estimated).ToList();
                        var EstimatedTotal = Estimated.Sum();
                        if ((EstimatedTotal / weekTotal) < 5)
                        {
                            CountStudent++;
                        }
                    }
                    var perc = percent(CountStudent, TotalStudent);
                    ViewBag.Estimated = CountStudent;
                    ViewBag.Percent = perc;
                }
            }

            rp.rp1 = reportStudent;
            rp.rp2 = reportCourse;
            return View(rp);
        }

        public ActionResult Enrollment(ListStudent ls,string searchString, string searchCheck, string SelectCampus, string SelectSubject, string SelectSemester, string SelectDatetime)
        {
            var context = new MSSEntities();
            List<string> NoEnrollment = new List<string>();
            List<ListStudent> s = new List<ListStudent>();
            List<string> NotRequiredCourse = new List<string>();

            List<SelectListItem> selectSemes = new List<SelectListItem>();

            var listSemes = (from a in context.Semesters
                             select a).ToList();
            var orderedListSemes = listSemes.OrderByDescending(x => x.Start_Date).ToList();
            foreach (var a in orderedListSemes)
            {
                selectSemes.Add(new SelectListItem
                {
                    Text = a.Semester_Name,
                    Value = a.Semester_ID
                });
            }
            ViewBag.SelectSemester = selectSemes;

            DateTime date;
            ViewBag.SelectDatetime = Date(SelectSemester);
            date = Convert.ToDateTime(Date(SelectSemester).Select(m => m.Value).FirstOrDefault());
            if (SelectDatetime != null)
            {
                date = Convert.ToDateTime(SelectDatetime);
            }


            if (!String.IsNullOrEmpty(searchCheck))
            {

                var CourseStudent = (from a in context.Student_Course_Log
                                     where a.Semester_ID == SelectSemester
                                     select new
                                     {
                                         Roll = a.Roll
                                     }).Distinct().ToList();
                var Student = (from a in context.Students
                               where a.Semester_ID == SelectSemester
                               select new
                               {
                                   Roll = a.Roll
                               }).Distinct().ToList();

                var listStudentEnrollment = (from a in context.Student_Course_Log
                                             where a.Course_ID != null && a.Semester_ID == SelectSemester
                                             select new
                                             {
                                                 Roll = a.Roll
                                             }).Distinct().ToList();

                foreach (var cs in Student)
                {
                    int temp = 0;
                    foreach (var st in CourseStudent)
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

                foreach (var cs in CourseStudent)
                {
                    int temp = 0;
                    foreach (var st in listStudentEnrollment)
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
                        NotRequiredCourse.Add(cs.Roll);
                    }

                }
                int rowNo = 0;
                foreach (var b in NoEnrollment)
                {
                    List<ListStudent> infoStudent = (from c in context.Students
                                                     where c.Roll == b && c.Semester_ID == SelectSemester
                                                     select new
                                                     {
                                                         Email = c.Email,
                                                         Roll = c.Roll,
                                                         Full_Name = c.Full_Name,
                                                         Campus = c.Campus_ID
                                                     }).ToList().Select(p => new ListStudent
                                                     {
                                                         Email = p.Email,
                                                         Roll = p.Roll,
                                                         Full_Name = p.Full_Name,
                                                         Campus = p.Campus
                                                     }).ToList();
                    List<string> subjectStudent =   (from c in context.Students
                                                     join d in context.Subject_Student on c.Roll equals d.Roll
                                                     join e in context.Subjects on d.Subject_ID equals e.Subject_ID
                                                     where c.Roll == b && c.Semester_ID == SelectSemester && d.Semester_ID == SelectSemester
                                                     select e.Subject_Name).ToList();

                                
                    foreach (var i in infoStudent)
                    {
                        s.Add(new ListStudent { STT = rowNo, Email = i.Email, Roll = i.Roll, Full_Name = i.Full_Name, Campus = i.Campus, ListSubject = subjectStudent, Note = "No Enrollment" });
                    }

                }
                foreach (var b in NotRequiredCourse)
                {
                    List<ListStudent> infoStudent = (from c in context.Students
                                                     where c.Roll == b && c.Semester_ID == SelectSemester
                                                     select new
                                                     {
                                                         Email = c.Email,
                                                         Roll = c.Roll,
                                                         Full_Name = c.Full_Name,
                                                         Campus = c.Campus_ID
                                                     }).ToList().Select(p => new ListStudent
                                                     {
                                                         Email = p.Email,
                                                         Roll = p.Roll,
                                                         Full_Name = p.Full_Name,
                                                         Campus = p.Campus
                                                     }).ToList();
                    List<string> subjectStudent = (from c in context.Students
                                                   join d in context.Subject_Student on c.Roll equals d.Roll
                                                   join e in context.Subjects on d.Subject_ID equals e.Subject_ID
                                                   where c.Roll == b && c.Semester_ID == SelectSemester && d.Semester_ID == SelectSemester
                                                   select e.Subject_Name).ToList();


                    foreach (var i in infoStudent)
                    {
                        s.Add(new ListStudent { STT = rowNo, Email = i.Email, Roll = i.Roll, Full_Name = i.Full_Name, Campus = i.Campus, ListSubject = subjectStudent, Note = "Have not entered the required course" });
                    }

                }

                if (!String.IsNullOrEmpty(searchString))
                {
                    s = s.Where(a => a.Roll.Contains(searchString)).ToList();        
                }
                if (!String.IsNullOrEmpty(SelectCampus))
                {
                    s = s.Where(a => a.Campus.Contains(SelectCampus)).ToList();
                }
                if (!String.IsNullOrEmpty(SelectSubject))
                {
                    s = s.Where(a => a.ListSubject.Contains(SelectSubject)).ToList();
                }
            }

            var listCampus = (from a in context.Campus
                              select a).ToList();
            var listSubject = (from a in context.Subjects
                               where a.Subject_Active == true
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
                    Text = a.Campus_Name,
                    Value = a.Campus_ID
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
            ViewBag.SelectSubject = selectSj;
            ViewBag.SelectCampus = selectCp;
            ls.ls1 = s;

            ViewBag.TotalSearch = s.Count();
            //rp.Students = s;
            return View("Enrollment", ls);
        }

        public ActionResult Member(InfoStudent Info, string searchString, string searchCheck, string SelectSemester, string SelectDatetime)
        {
            var context = new MSSEntities();
            List<InfoStudent> infoOfStudent = new List<InfoStudent>();
            //List<InfoStudent> distinctList = new List<InfoStudent>();

            List<SelectListItem> selectSemes = new List<SelectListItem>();

            var listSemes = (from a in context.Semesters
                             select a).ToList();
            var orderedListSemes = listSemes.OrderByDescending(x => x.Start_Date).ToList();
            foreach (var a in orderedListSemes)
            {
                selectSemes.Add(new SelectListItem
                {
                    Text = a.Semester_Name,
                    Value = a.Semester_ID
                });
            }
            ViewBag.SelectSemester = selectSemes;

            DateTime date;
            ViewBag.SelectDatetime = Date(SelectSemester);
            date = Convert.ToDateTime(Date(SelectSemester).Select(m => m.Value).FirstOrDefault());
            if (SelectDatetime != null)
            {
                date = Convert.ToDateTime(SelectDatetime);
            }

            if (!String.IsNullOrEmpty(searchCheck))
            {
                infoOfStudent = (from a in context.Student_Course_Log
                                 where a.Roll == searchString && a.Semester_ID == SelectSemester && a.Date_Import == date
                                 select new
                                 {
                                     Course_Name = a.Course_Name,
                                     Course_Enrollment_Time = a.Course_Enrollment_Time,
                                     Last_Course_Activity_Time = a.Last_Course_Activity_Time,
                                     Overall_Progress = a.Overall_Progress,
                                     Completion_Time = a.Completion_Time,
                                     Estimated = a.Estimated,
                                     Completed = a.Completed
                                 }).ToList().Select(p => new InfoStudent
                                 {
                                     Course_Name = p.Course_Name,
                                     Course_Enrollment_Time = (DateTime)p.Course_Enrollment_Time,
                                     Last_Course_Activity_Time = (DateTime)p.Last_Course_Activity_Time,
                                     Overall_Progress = (double)p.Overall_Progress,
                                     Completion_Time = (DateTime)p.Completion_Time,
                                     Estimated = (double)p.Estimated,
                                     Completed = (bool)p.Completed
                                 }).ToList();
            }
            var distinctList = infoOfStudent.GroupBy(x => x.Course_Name).Select(y => y.FirstOrDefault());
            ViewBag.TotalSearch = distinctList.Count();
            Info.InforList = distinctList.OrderBy(t => t.Overall_Progress).ToList();
            return View("Member", Info);
        }

        public ActionResult NotRequiredCourse(ListNotRequiredCourse lc, string SelectString, string searchCheck, string SelectSemester, string SelectDatetime)
        {
            var context = new MSSEntities();
            List<ListNotRequiredCourse> listNotRequiredCourses = new List<ListNotRequiredCourse>();

            var NotRequired = (from a in context.Student_Course_Log
                               where a.Course_ID == null && a.Semester_ID == SelectSemester
                               select a.Course_Name).Distinct().ToList();

            List<SelectListItem> selectSemes = new List<SelectListItem>();

            var listSemes = (from a in context.Semesters
                             select a).ToList();
            var orderedListSemes = listSemes.OrderByDescending(x => x.Start_Date).ToList();
            foreach (var a in orderedListSemes)
            {
                selectSemes.Add(new SelectListItem
                {
                    Text = a.Semester_Name,
                    Value = a.Semester_ID
                });
            }
            ViewBag.SelectSemester = selectSemes;

            if (String.IsNullOrEmpty(searchCheck) && orderedListSemes.Count > 0)
            {
                SelectSemester = orderedListSemes[0].Semester_ID;
            }

            DateTime date;
            ViewBag.SelectDatetime = Date(SelectSemester);
            date = Convert.ToDateTime(Date(SelectSemester).Select(m => m.Value).FirstOrDefault());
            if (SelectDatetime != null)
            {
                date = Convert.ToDateTime(SelectDatetime);
            }

            if (!String.IsNullOrEmpty(searchCheck))
            {
                foreach (var a in NotRequired)
                {
                    var completed = (from b in context.Student_Course_Log
                                     where b.Course_Name == a && b.Completed == true && b.Course_ID == null && b.Semester_ID == SelectSemester && b.Date_Import == date
                                     select b.Roll).Distinct().Count();
                    var notCompleted = (from b in context.Student_Course_Log
                                        where b.Course_Name == a && b.Completed == false && b.Course_ID == null && b.Semester_ID == SelectSemester && b.Date_Import == date
                                        select b.Roll).Distinct().Count();
                    listNotRequiredCourses.Add(new ListNotRequiredCourse { Name = a, Complelted = completed, NotComplelted = notCompleted });
                }

                if (!String.IsNullOrEmpty(SelectString))
                {
                    if(SelectString == "1")
                    {
                        listNotRequiredCourses = listNotRequiredCourses.Where(a => (a.Complelted > 0)).OrderByDescending(a => a.Complelted).ToList();
                    }
                    if (SelectString == "2")
                    {
                        listNotRequiredCourses = listNotRequiredCourses.Where(a => (a.Complelted == 0)).OrderByDescending(a => a.NotComplelted).ToList();
                    }

                }
            }

            List<SelectListItem> selectCompleted = new List<SelectListItem>();
            selectCompleted.Add(new SelectListItem { Text = "--Select Completed--", Value = ""});
            selectCompleted.Add(new SelectListItem { Text = "Completed", Value = "1" });
            selectCompleted.Add(new SelectListItem { Text = "Not Completed", Value = "2" });

            ViewBag.SelectString = selectCompleted;
            lc.lc1 = listNotRequiredCourses.OrderByDescending(a => a.Complelted).ToList();

            ViewBag.TotalSearch = listNotRequiredCourses.Count();
            //rp.Students = s;
            return View("NotRequiredCourse", lc);
        }

        public ActionResult CertificateReport(CertificateViewModel cv, string searchCheck,string SearchString, string SelectString, string SelectSemester)
        {
            var context = new MSSEntities();
            var rollList = (from a in context.Certificates
                            where a.Semester_ID == SelectSemester
                            select a.Roll).Distinct().ToList();
            List<CertificateViewModel> certificate = new List<CertificateViewModel>();

            List<SelectListItem> selectSemes = new List<SelectListItem>();

            var listSemes = (from a in context.Semesters
                             select a).ToList();
            var orderedListSemes = listSemes.OrderByDescending(x => x.Start_Date).ToList();
            foreach (var a in orderedListSemes)
            {
                selectSemes.Add(new SelectListItem
                {
                    Text = a.Semester_Name,
                    Value = a.Semester_ID
                });
            }
            ViewBag.SelectSemester = selectSemes;

            if (!String.IsNullOrEmpty(searchCheck))
            {
                 certificate = (from a in context.Certificates
                                where a.Semester_ID == SelectSemester
                                   select new
                                   {
                                       Link = a.Link,
                                       Date_Submit = a.Date_Submit,
                                       Roll = a.Roll,
                                       Course_ID = a.Course_ID,
                                       Specification_ID = a.Specification_ID
                                   }).ToList().Select(p => new CertificateViewModel
                                   {
                                       Link = p.Link,
                                       Date_Submit = p.Date_Submit,
                                       Roll = p.Roll,
                                       CourseId = (int)p.Course_ID,
                                       SpecID = (int)p.Specification_ID,
                                       CourseName = (from b in context.Courses
                                                     where b.Course_ID == p.Course_ID
                                                     select b.Course_Name).FirstOrDefault(),
                                       SubjectName = (from b in context.Specifications
                                                      join c in context.Subjects on b.Subject_ID equals c.Subject_ID
                                                      where b.Specification_ID == p.Specification_ID
                                                      select c.Subject_Name).FirstOrDefault()
                                   }).ToList();
                if (!String.IsNullOrEmpty(SearchString))
                {
                    certificate = certificate.Where(a => a.Roll.Contains(SearchString)).ToList();
                }
                if (!String.IsNullOrEmpty(SelectString))
                {
                    certificate = certificate.Where(a => a.SubjectName.Contains(SelectString)).ToList();
                }
            }
               
            var listSubject = (from a in context.Subjects
                               where a.Subject_Active == true
                               select a.Subject_Name).ToList();
            List<SelectListItem> selectSj = new List<SelectListItem>();
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
            ViewBag.SelectString = selectSj;
            ViewBag.TotalSearch = certificate.Count();

            cv.certificatesModel = certificate;



            return View("CertificateReport", cv);
        }
        
        public ActionResult PrintViewToPdf(string SelectDatetime, string searchCheck, string weekNumber, string SelectSemester)
        {
            var rpN = new Report();
            var report = new ActionAsPdf("Index", new { rpN, weekNumber, searchCheck, SelectDatetime, SelectSemester });
            return report;
        }

        public ActionResult SpecCompleted(ListStudent listS, string SelectSemester, string searchCheck, string SearchString, string SelectDatetime, string SelectCampus, string Compulsory)
        {
            var context = new MSSEntities();
            List<SelectListItem> selectSemes = new List<SelectListItem>();

            var listSemes = (from a in context.Semesters
                             select a).ToList();
            var orderedListSemes = listSemes.OrderByDescending(x => x.Start_Date).ToList();
            foreach (var a in orderedListSemes)
            {
                selectSemes.Add(new SelectListItem
                {
                    Text = a.Semester_Name,
                    Value = a.Semester_ID
                });
            }
            ViewBag.SelectSemester = selectSemes;


            var listCampus = (from a in context.Campus
                              select a).ToList();
            List<SelectListItem> selectCp = new List<SelectListItem>();
            selectCp.Add(new SelectListItem
            {
                Text = "--Select Campus--",
                Value = ""
            });
            foreach (var a in listCampus)
            {
                selectCp.Add(new SelectListItem
                {
                    Text = a.Campus_Name,
                    Value = a.Campus_ID
                });
            }
            ViewBag.SelectCampus = selectCp;

            if (String.IsNullOrEmpty(searchCheck) && orderedListSemes.Count > 0)
            {
                SelectSemester = orderedListSemes[0].Semester_ID;
            }
            DateTime date;
            ViewBag.SelectDatetime = Date(SelectSemester);
            date = Convert.ToDateTime(Date(SelectSemester).Select(m => m.Value).FirstOrDefault());
            if (SelectDatetime != null)
            {
                date = Convert.ToDateTime(SelectDatetime);
            }
            List<ListStudent> ListStudentCompleted = new List<ListStudent>();
            List<Student_Specification_Log> specCompletedListSpec = new List<Student_Specification_Log>();
            List<string> studentList = new List<string>();
            if (!String.IsNullOrEmpty(SearchString))
            {
                SearchString = SearchString.Trim();
            }
            if (!String.IsNullOrEmpty(searchCheck))
            {
                if (String.IsNullOrEmpty(SearchString))
                {
                    if(Compulsory == "true")
                    {
                        specCompletedListSpec = (from a in context.Student_Specification_Log
                                                 join b in context.Specifications on a.Specification_ID equals b.Specification_ID
                                                 where a.Completed == true && a.Semester_ID == SelectSemester && a.Date_Import == date
                                                 select a).ToList();
                        studentList = (from a in context.Student_Course_Log
                                       where a.Semester_ID == SelectSemester && a.Completed == true && a.Course_ID != null
                                       select a.Roll).Distinct().ToList();
                    }
                    else
                    {
                        specCompletedListSpec = (from a in context.Student_Specification_Log
                                                 where a.Completed == true && a.Semester_ID == SelectSemester && a.Date_Import == date
                                                 select a).ToList();
                        studentList = (from a in context.Student_Course_Log
                                       where a.Semester_ID == SelectSemester && a.Completed == true
                                       select a.Roll).Distinct().ToList();
                    }

                }
                else
                {
                    if (Compulsory == "true")
                    {
                        specCompletedListSpec = (from a in context.Student_Specification_Log
                                                 join b in context.Specifications on a.Specification_ID equals b.Specification_ID
                                                 where a.Completed == true && a.Semester_ID == SelectSemester && a.Roll == SearchString && a.Date_Import == date
                                                 select a).ToList();
                    }
                    else
                    {
                        specCompletedListSpec = (from a in context.Student_Specification_Log
                                                 where a.Completed == true && a.Semester_ID == SelectSemester && a.Roll == SearchString && a.Date_Import == date
                                                 select a).ToList();
                    }

                    studentList.Add(SearchString);
                }
                foreach (var t in specCompletedListSpec)
                {
                    ListStudentCompleted.Add(new ListStudent { Roll = t.Roll, Campus = t.Campus, Subject = t.Specialization, Semester_ID = t.Semester_ID,Email = t.Email });
                }
                foreach (var t in studentList)
                {
                    var listSubOfStudent = (from a in context.Students
                                            join b in context.Subject_Student on a.Roll equals b.Roll
                                            join c in context.Subjects on b.Subject_ID equals c.Subject_ID
                                            where a.Semester_ID == SelectSemester && b.Semester_ID == SelectSemester && a.Roll == t
                                            select c).ToList();
                    foreach (var u in listSubOfStudent)
                    {
                        var countOfCourse = (from a in context.Courses
                                             join b in context.Specifications on a.Specification_ID equals b.Specification_ID
                                             where b.Subject_ID == u.Subject_ID
                                             select a.Course_ID).Count();
                        var info = (from a in context.Students
                                    where a.Semester_ID == SelectSemester && a.Roll == t
                                    select a).FirstOrDefault();

                        var completed = (from a in context.Student_Course_Log
                                         where a.Roll == t && a.Semester_ID == SelectSemester && a.Subject_ID == u.Subject_ID && a.Completed == true && a.Course_ID != null && a.Date_Import == date
                                         select a).Count();

                        if (countOfCourse == completed)
                        {
                            ListStudentCompleted.Add(new ListStudent { Roll = t, Campus = info.Campus_ID, Subject = u.Subject_Name, Semester_ID = info.Semester_ID,Email = info.Email });
                        }
                    }
                }

                ListStudentCompleted = ListStudentCompleted.GroupBy(m => new { m.Roll, m.Subject }).Select(m => m.First()).ToList();
                if (!String.IsNullOrEmpty(SearchString))
                {
                    ListStudentCompleted = ListStudentCompleted.Where(a => a.Roll.Contains(SearchString)).ToList();
                    ViewBag.TotalSearch = ListStudentCompleted.Count();
                    ViewBag.Spec = (from a in context.Students
                                    join b in context.Subject_Student on a.Roll equals b.Roll
                                    join c in context.Subjects on b.Subject_ID equals c.Subject_ID
                                    where a.Semester_ID == SelectSemester && b.Semester_ID == SelectSemester && a.Roll == SearchString
                                    select c.Subject_ID).Count();
                    if(ViewBag.TotalSearch > 0 && ViewBag.Spec > 0)
                    {
                        ViewBag.Percent = percent(ViewBag.TotalSearch, ViewBag.Spec);
                    }
                }
                else if (String.IsNullOrEmpty(SearchString))
                {
                    ViewBag.TotalSearchForSearchNull = ListStudentCompleted.Count();
                }
                if (!String.IsNullOrEmpty(SelectCampus))
                {
                    ListStudentCompleted = ListStudentCompleted.Where(a => a.Campus.Contains(SelectCampus)).ToList();
                }
            }
            
            listS.ls1 = ListStudentCompleted;

            return View("SpecCompleted", listS);
        }

        public ActionResult Bonus(ListStudent listS, string SelectSemester, string searchCheck, string SearchString, string SelectDatetime)
        {
            var context = new MSSEntities();
            List<ListStudent> bonusListStudent = new List<ListStudent>();
            List<ListStudent> ListCompleted = new List<ListStudent>();
            List<SelectListItem> selectSemes = new List<SelectListItem>();

            var listSemes = (from a in context.Semesters
                             select a).ToList();
            var orderedListSemes = listSemes.OrderByDescending(x => x.Start_Date).ToList();
            foreach (var a in orderedListSemes)
            {
                selectSemes.Add(new SelectListItem
                {
                    Text = a.Semester_Name,
                    Value = a.Semester_ID
                });
            }
            ViewBag.SelectSemester = selectSemes;
            if (String.IsNullOrEmpty(searchCheck) && orderedListSemes.Count > 0)
            {
                SelectSemester = orderedListSemes[0].Semester_ID;
            }

            DateTime date;
            ViewBag.SelectDatetime = Date(SelectSemester);
            date = Convert.ToDateTime(Date(SelectSemester).Select(m => m.Value).FirstOrDefault());
            if(SelectDatetime!= null)
            {
                date = Convert.ToDateTime(SelectDatetime);
            }


            if (!String.IsNullOrEmpty(searchCheck) && date != DateTime.MinValue)
            {
                var listCourseCompleted = (from a in context.Student_Course_Log
                                           where a.Completed == true && a.Date_Import == date && a.Course_ID != null
                                           select a).ToList();
                var listStudentCompleted = (from a in context.Student_Course_Log
                                            where a.Completed == true && a.Date_Import == date && a.Course_ID != null
                                            select a.Roll).Distinct().ToList();
                foreach (var t in listCourseCompleted)
                {
                    var deadLine = (from a in context.Course_Deadline
                                    where a.Course_ID == t.Course_ID && a.Semester_ID == SelectSemester
                                    select a.Deadline).FirstOrDefault();
                    if (t.Completion_Time < deadLine)
                    {
                        ListCompleted.Add(new ListStudent { Roll = t.Roll, Campus = t.Campus });
                    }
                }
                foreach (var t in listStudentCompleted)
                {
                    var countBonus = ListCompleted.Where(m => m.Roll == t).Count();
                    var campus = (from a in context.Students
                                  where a.Roll == t && a.Semester_ID == SelectSemester
                                  select a.Campus_ID).FirstOrDefault();
                    double bonus = 0;
                    bonus = countBonus * 0.25;
                    if (bonus > 1)
                    {
                        bonus = 1;
                    }
                    if (bonus > 0)
                    {
                        bonusListStudent.Add(new ListStudent { Roll = t, Campus = campus, PBonus = bonus });
                    }
                }
                if (!String.IsNullOrEmpty(SearchString))
                {
                    bonusListStudent = bonusListStudent.Where(a => a.Roll.Contains(SearchString)).ToList();
                }
            }
                
            ViewBag.TotalSearch = bonusListStudent.Count();
            listS.ls1 = bonusListStudent;
            return View("Bonus", listS);
        }

        private int Count(string subject, string campus, DateTime dateImport)
        {
            //double per;
            //DateTime date = Convert.ToDateTime(dateImport);
            int count = 0;
            var context = new MSSEntities();
            if (subject != "" && campus == "")
            {
                count = (from a in context.Student_Course_Log
                         join b in context.Courses on a.Course_ID equals b.Course_ID
                         join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                         join d in context.Subjects on c.Subject_ID equals d.Subject_ID
                         where subject == a.Subject_ID && a.Date_Import == dateImport
                         select a.Roll).Distinct().Count();
            }
            else if (subject == "" && campus == "")
            {
                count = (from a in context.Student_Course_Log
                         join b in context.Courses on a.Course_ID equals b.Course_ID
                         join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                         join d in context.Subjects on c.Subject_ID equals d.Subject_ID
                         where a.Date_Import == dateImport
                         select a.Roll).Distinct().Count();
            }
            else if(subject != "" && campus != "")
            {
                count = (from a in context.Student_Course_Log
                         join b in context.Courses on a.Course_ID equals b.Course_ID
                         join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                         join d in context.Subjects on c.Subject_ID equals d.Subject_ID
                         where  a.Campus == campus && a.Subject_ID == subject && a.Date_Import == dateImport
                         select a.Roll).Distinct().Count();
            }
            else if(subject == "" && campus != "")
            {
                count = (from a in context.Student_Course_Log
                         join b in context.Courses on a.Course_ID equals b.Course_ID
                         join c in context.Specifications on b.Specification_ID equals c.Specification_ID
                         join d in context.Subjects on c.Subject_ID equals d.Subject_ID
                         where a.Campus == campus && a.Date_Import == dateImport
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

        private int Campus(string subjectId, string campus, string semester)
        {
            var context = new MSSEntities();
            var student = 0;
            if (subjectId == "" && campus != "")
            {
                student = (from a in context.Subjects
                               join b in context.Subject_Student on a.Subject_ID equals b.Subject_ID
                               join c in context.Students on b.Roll equals c.Roll
                               where c.Campus_ID == campus && c.Semester_ID == semester && a.Subject_Active == true && b.Semester_ID == semester
                               select c.Roll).Count();
            }
            else if(subjectId == "" && campus == "")
            {
                student = (from a in context.Subjects
                           join b in context.Subject_Student on a.Subject_ID equals b.Subject_ID
                           join c in context.Students on b.Roll equals c.Roll
                           where c.Semester_ID == semester && a.Subject_Active == true && b.Semester_ID == semester
                           select c.Roll).Count();
            }
            else
            {
                student = (from a in context.Subjects
                               join b in context.Subject_Student on a.Subject_ID equals b.Subject_ID
                               join c in context.Students on b.Roll equals c.Roll
                               where subjectId == a.Subject_ID && c.Campus_ID == campus && c.Semester_ID == semester && a.Subject_Active == true && b.Semester_ID == semester
                           select c.Roll).Count();
            }
            
            return student;
        }
        private List<SelectListItem> Date(string SelectSemester)
        {
            var context = new MSSEntities();
            List<SelectListItem> selectDate = new List<SelectListItem>();
            var starDate = (from a in context.Semesters
                            where a.Semester_ID == SelectSemester
                            select a.Start_Date).FirstOrDefault();
            var endDate = (from a in context.Semesters
                           where a.Semester_ID == SelectSemester
                           select a.End_Date).FirstOrDefault();


            var listDate = (from a in context.Student_Course_Log
                            where (a.Date_Import <= endDate && a.Date_Import >= starDate)
                            select a.Date_Import).Distinct().ToList();

            var orderedListDate = listDate.OrderByDescending(x => x).ToList();
            foreach (var a in orderedListDate)
            {
                selectDate.Add(new SelectListItem
                {
                    Text = Convert.ToDateTime(a).ToString("dd/MM/yyyy"),
                    Value = a.ToString()
                });
            }
            return selectDate;
        }
    }
}
