using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Models;
using MSS_DEMO.Common;

namespace MSS_DEMO.Controllers.Certificates
{
    [CheckCredential(Role_ID = "5")]
    public class CertificatesController : Controller
    {
        // GET: Certificate
        public ActionResult Index(CertificateViewModel CerModel)
        {
            List<Certificate> certificates = new List<Certificate>();
            List<CertificateViewModel> courseNameList = new List<CertificateViewModel>();
            var context = new MSSEntities();
            var session = (UserLogin)HttpContext.Session[CommonConstants.User_Session];
            var SelectSemester = (from a in context.Semesters
                              where a.Start_Date < DateTime.Now && a.End_Date > DateTime.Now
                              select a.Semester_ID).FirstOrDefault();
            if (session != null)
            {
                var rollNumber = (from a in context.Students
                                  where a.Email == session.UserName && a.Semester_ID == SelectSemester
                                  select a.Roll).FirstOrDefault();

                var courseList = (from a in context.Students
                                  join b in context.Subject_Student on a.Roll equals b.Roll
                                  join c in context.Subjects on b.Subject_ID equals c.Subject_ID
                                  join d in context.Specifications on c.Subject_ID equals d.Subject_ID
                                  join e in context.Courses on d.Specification_ID equals e.Specification_ID
                                  where a.Roll == rollNumber && c.Subject_Active == true && a.Semester_ID == SelectSemester
                                  select e).Distinct().ToList();


                foreach (var t in courseList)
                {
                    var courseName = (from a in context.Courses
                                      where a.Course_ID == t.Course_ID
                                      select a.Course_Name).FirstOrDefault();
                    var link = (from a in context.Certificates
                                where a.Course_ID == t.Course_ID && a.Roll == rollNumber && a.Semester_ID == SelectSemester
                                select a.Link).FirstOrDefault();
                    var subjectName = (from a in context.Courses
                                       join b in context.Specifications on a.Specification_ID equals b.Specification_ID
                                       where a.Course_ID == t.Course_ID
                                       select b.Specification_Name).FirstOrDefault();
                    courseNameList.Add(new CertificateViewModel {SubjectName = subjectName, CourseName = courseName, Link = link });
                }
                var specList = (from a in context.Students
                                join b in context.Subject_Student on a.Roll equals b.Roll
                                join c in context.Subjects on b.Subject_ID equals c.Subject_ID
                                join d in context.Specifications on c.Subject_ID equals d.Subject_ID
                                where a.Roll == rollNumber && c.Subject_Active == true && d.Is_Real_Specification == true && a.Semester_ID == SelectSemester
                                select d).ToList();
                foreach(var t in specList)
                {
                    var link = (from a in context.Certificates
                                where a.Course_ID == 0 && a.Roll == rollNumber && a.Semester_ID == SelectSemester
                                select a.Link).FirstOrDefault();
                    courseNameList.Add(new CertificateViewModel { SubjectName = t.Specification_Name, CourseName = t.Specification_Name, Link = link });
                }
            }


            CerModel.certificatesModel = courseNameList.OrderBy(m => m.SubjectName).ToList();
            return View("Index", CerModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(CertificateViewModel CerModel, string[] CourseN)
        {
            List<CertificateViewModel> courseNameList = new List<CertificateViewModel>();
            var context = new MSSEntities();
            var session = (UserLogin)HttpContext.Session[CommonConstants.User_Session];
            var SelectSemester = (from a in context.Semesters
                                  where a.Start_Date < DateTime.Now && a.End_Date > DateTime.Now
                                  select a.Semester_ID).FirstOrDefault();
            if (session != null)
            {
                var rollNumber = (from a in context.Students
                                  where a.Email == session.UserName && a.Semester_ID == SelectSemester
                                  select a.Roll).FirstOrDefault();

                var courseList = (from a in context.Students
                                  join b in context.Subject_Student on a.Roll equals b.Roll
                                  join c in context.Subjects on b.Subject_ID equals c.Subject_ID
                                  join d in context.Specifications on c.Subject_ID equals d.Subject_ID
                                  join e in context.Courses on d.Specification_ID equals e.Specification_ID
                                  where a.Roll == rollNumber && c.Subject_Active == true && a.Semester_ID == SelectSemester
                                  select e).Distinct().ToList();
                //var semester = (from a in context.Semesters
                //                where a.Start_Date < DateTime.Now && a.End_Date > DateTime.Now
                //                select a.Semester_ID).FirstOrDefault();


                foreach (var t in courseList)
                {
                    var courseName = (from a in context.Courses
                                      where a.Course_ID == t.Course_ID
                                      select a.Course_Name).FirstOrDefault();
                    var link = (from a in context.Certificates
                                where a.Course_ID == t.Course_ID && a.Roll == rollNumber && a.Semester_ID == SelectSemester
                                select a.Link).FirstOrDefault();
                    var specId = (from a in context.Courses
                                  where a.Course_ID == t.Course_ID
                                  select a.Specification_ID).FirstOrDefault();

                    courseNameList.Add(new CertificateViewModel { CourseName = courseName, Link = link, CourseId = (int)t.Course_ID, SpecID = (int)specId,Roll = rollNumber, SemesterId = SelectSemester });
                }
                var specList = (from a in context.Students
                                join b in context.Subject_Student on a.Roll equals b.Roll
                                join c in context.Subjects on b.Subject_ID equals c.Subject_ID
                                join d in context.Specifications on c.Subject_ID equals d.Subject_ID
                                where a.Roll == rollNumber && c.Subject_Active == true && d.Is_Real_Specification == true && a.Semester_ID == SelectSemester
                                select d).ToList();
                foreach (var t in specList)
                {
                    var link = (from a in context.Certificates
                                where a.Course_ID == 0 && a.Roll == rollNumber && a.Semester_ID == SelectSemester
                                select a.Link).FirstOrDefault();
                    courseNameList.Add(new CertificateViewModel { SubjectName = t.Specification_Name,CourseId = 0, CourseName = t.Specification_Name, Link = link,Roll = rollNumber, SemesterId = SelectSemester,SpecID = t.Specification_ID });
                }
            }

            CerModel.certificatesModel = courseNameList.OrderBy(m => m.SpecID).ToList();
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    for (var i = 0; i < CerModel.certificatesModel.Count; i++)
                    {
                        int courseId = CerModel.certificatesModel[i].CourseId;
                        string roll = CerModel.certificatesModel[i].Roll;
                        string semester = CerModel.certificatesModel[i].SemesterId;
                        int specId = CerModel.certificatesModel[i].SpecID;
                        var Exists = context.Certificates.Any(x => x.Course_ID == courseId && x.Roll == roll && x.Semester_ID == semester && x.Specification_ID == specId);
                        if (Exists)
                        {
                            var update = context.Certificates.FirstOrDefault(x => x.Course_ID == courseId && x.Roll == roll);
                            update.Link = CourseN[i];
                        }
                        else
                        {
                            context.Certificates.Add(new Certificate { Link = CourseN[i], Date_Submit = DateTime.Now, Roll = CerModel.certificatesModel[i].Roll, Course_ID = CerModel.certificatesModel[i].CourseId, Specification_ID = CerModel.certificatesModel[i].SpecID, Semester_ID = CerModel.certificatesModel[i].SemesterId });
                        }
                       
                    }
                    ViewBag.Message = "Successful";
                    context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch(Exception ex)
                {
                    ViewBag.Message = "Error";
                    dbContextTransaction.Rollback();
                }
                
            }


            return RedirectToAction("Index", "Certificates");
            //return View("Index", CerModel);
        }
    }
}