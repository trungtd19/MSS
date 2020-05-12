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
                var rollNumber = (from semes in context.Students
                                  where semes.Email == session.UserName && semes.Semester_ID == SelectSemester
                                  select semes.Roll).FirstOrDefault();

                var courseList = (from stu in context.Students
                                  join sub_stu in context.Subject_Student on stu.Roll equals sub_stu.Roll
                                  join sub in context.Subjects on sub_stu.Subject_ID equals sub.Subject_ID
                                  join spec in context.Specifications on sub.Subject_ID equals spec.Subject_ID
                                  join cour in context.Courses on spec.Specification_ID equals cour.Specification_ID
                                  where stu.Roll == rollNumber && sub.Subject_Active == true && stu.Semester_ID == SelectSemester && sub_stu.Semester_ID == SelectSemester
                                  select cour).Distinct().ToList();


                foreach (var t in courseList)
                {
                    var courseName = (from cour in context.Courses
                                      where cour.Course_ID == t.Course_ID
                                      select cour.Course_Name).FirstOrDefault();
                    var link = (from certi in context.Certificates
                                where certi.Course_ID == t.Course_ID && certi.Roll == rollNumber && certi.Semester_ID == SelectSemester
                                select certi.Link).FirstOrDefault();
                    var subjectName = (from cour in context.Courses
                                       join spec in context.Specifications on cour.Specification_ID equals spec.Specification_ID
                                       where cour.Course_ID == t.Course_ID
                                       select spec.Specification_Name).FirstOrDefault();
                    courseNameList.Add(new CertificateViewModel {SubjectName = subjectName, CourseName = courseName, Link = link });
                }
                var specList = (from stu in context.Students
                                join sub_stu in context.Subject_Student on stu.Roll equals sub_stu.Roll
                                join sub in context.Subjects on sub_stu.Subject_ID equals sub.Subject_ID
                                join spec in context.Specifications on sub.Subject_ID equals spec.Subject_ID
                                where stu.Roll == rollNumber && sub.Subject_Active == true && spec.Is_Real_Specification == true && stu.Semester_ID == SelectSemester && sub_stu.Semester_ID == SelectSemester
                                select spec).ToList();
                foreach(var t in specList)
                {
                    var link = (from certi in context.Certificates
                                where certi.Course_ID == 0 && certi.Roll == rollNumber && certi.Semester_ID == SelectSemester
                                select certi.Link).FirstOrDefault();
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
            var SelectSemester = (from semes in context.Semesters
                                  where semes.Start_Date < DateTime.Now && semes.End_Date > DateTime.Now
                                  select semes.Semester_ID).FirstOrDefault();
            if (session != null)
            {
                var rollNumber = (from stu in context.Students
                                  where stu.Email == session.UserName && stu.Semester_ID == SelectSemester
                                  select stu.Roll).FirstOrDefault();

                var courseList = (from stu in context.Students
                                  join sub_stu in context.Subject_Student on stu.Roll equals sub_stu.Roll
                                  join sub in context.Subjects on sub_stu.Subject_ID equals sub.Subject_ID
                                  join spec in context.Specifications on sub.Subject_ID equals spec.Subject_ID
                                  join cour in context.Courses on spec.Specification_ID equals cour.Specification_ID
                                  where stu.Roll == rollNumber && sub.Subject_Active == true && stu.Semester_ID == SelectSemester && sub_stu.Semester_ID == SelectSemester
                                  select cour).Distinct().ToList();

                foreach (var t in courseList)
                {
                    var courseName = (from cour in context.Courses
                                      where cour.Course_ID == t.Course_ID
                                      select cour.Course_Name).FirstOrDefault();
                    var link = (from certi in context.Certificates
                                where certi.Course_ID == t.Course_ID && certi.Roll == rollNumber && certi.Semester_ID == SelectSemester
                                select certi.Link).FirstOrDefault();
                    var specId = (from cour in context.Courses
                                  where cour.Course_ID == t.Course_ID
                                  select cour.Specification_ID).FirstOrDefault();

                    courseNameList.Add(new CertificateViewModel { CourseName = courseName, Link = link, CourseId = (int)t.Course_ID, SpecID = (int)specId, Roll = rollNumber, SemesterId = SelectSemester });
                }
                var specList = (from stu in context.Students
                                join sub_stu in context.Subject_Student on stu.Roll equals sub_stu.Roll
                                join sub in context.Subjects on sub_stu.Subject_ID equals sub.Subject_ID
                                join spec in context.Specifications on sub.Subject_ID equals spec.Subject_ID
                                where stu.Roll == rollNumber && sub.Subject_Active == true && spec.Is_Real_Specification == true && stu.Semester_ID == SelectSemester && sub_stu.Semester_ID == SelectSemester
                                select spec).ToList();
                foreach (var t in specList)
                {
                    var link = (from certi in context.Certificates
                                where certi.Course_ID == 0 && certi.Roll == rollNumber && certi.Semester_ID == SelectSemester
                                select certi.Link).FirstOrDefault();
                    courseNameList.Add(new CertificateViewModel { SubjectName = t.Specification_Name, CourseId = 0, CourseName = t.Specification_Name, Link = link, Roll = rollNumber, SemesterId = SelectSemester, SpecID = t.Specification_ID });
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
                            var update = context.Certificates.FirstOrDefault(x => x.Course_ID == courseId && x.Roll == roll && x.Semester_ID == semester && x.Specification_ID == specId);
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