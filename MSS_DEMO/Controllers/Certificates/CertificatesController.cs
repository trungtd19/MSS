using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Models;
using MSS_DEMO.Common;

namespace MSS_DEMO.Controllers.Certificates
{
    public class CertificatesController : Controller
    {
        // GET: Certificate
        public ActionResult Index(CertificateViewModel CerModel)
        {
            List<Certificate> certificates = new List<Certificate>();
            List<CertificateViewModel> courseNameList = new List<CertificateViewModel>();
            var context = new MSSEntities();
            var session = (UserLogin)HttpContext.Session[CommonConstants.User_Session];
            if (session != null)
            {
                var rollNumber = (from a in context.Students
                                  where a.Email == session.UserName
                                  select a.Roll).FirstOrDefault();

                var courseList = (from a in context.Student_Course_Log
                                  where a.Roll == rollNumber && a.Completed == true
                                  select a.Course_ID).ToList();


                foreach (var t in courseList)
                {
                    var courseName = (from a in context.Courses
                                      where a.Course_ID == t
                                      select a.Course_Name).FirstOrDefault();
                    var link = (from a in context.Certificates
                                where a.Course_ID == t && a.Roll == rollNumber
                                select a.Link).FirstOrDefault();
                    courseNameList.Add(new CertificateViewModel { CourseName = courseName, Link = link });
                }
            }

            CerModel.certificatesModel = courseNameList;
            return View("Index", CerModel);
        }

        [HttpPost]
        public ActionResult Save(CertificateViewModel CerModel, string[] CourseN)
        {
            List<CertificateViewModel> courseNameList = new List<CertificateViewModel>();
            var context = new MSSEntities();
            var session = (UserLogin)HttpContext.Session[CommonConstants.User_Session];
            if (session != null)
            {
                var rollNumber = (from a in context.Students
                                  where a.Email == session.UserName
                                  select a.Roll).FirstOrDefault();

                var courseList = (from a in context.Student_Course_Log
                                  where a.Roll == rollNumber && a.Completed == true
                                  select a.Course_ID).ToList();


                foreach (var t in courseList)
                {
                    var courseName = (from a in context.Courses
                                      where a.Course_ID == t
                                      select a.Course_Name).FirstOrDefault();
                    var link = (from a in context.Certificates
                                where a.Course_ID == t && a.Roll == rollNumber
                                select a.Link).FirstOrDefault();
                    var specId = (from a in context.Courses
                                  where a.Course_ID == t
                                  select a.Specification_ID).FirstOrDefault();
                    courseNameList.Add(new CertificateViewModel { CourseName = courseName, Link = link, CourseId = (int)t, SpecID = (int)specId,Roll = rollNumber });
                }
            }
            CerModel.certificatesModel = courseNameList;
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    for (var i = 0; i < CerModel.certificatesModel.Count; i++)
                    {
                        int courseId = CerModel.certificatesModel[i].CourseId;
                        string roll = CerModel.certificatesModel[i].Roll;
                        var Exists = context.Certificates.Any(x => x.Course_ID == courseId && x.Roll == roll);
                        if (Exists)
                        {
                            var update = context.Certificates.FirstOrDefault(x => x.Course_ID == courseId && x.Roll == roll);
                            update.Link = CourseN[i];
                        }
                        else
                        {
                            context.Certificates.Add(new Certificate { Link = CourseN[i], Date_Submit = DateTime.Now, Roll = CerModel.certificatesModel[i].Roll, Course_ID = CerModel.certificatesModel[i].CourseId, Specification_ID = CerModel.certificatesModel[i].SpecID });
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