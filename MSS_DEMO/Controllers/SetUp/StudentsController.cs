using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MSS_DEMO.Common;
using MSS_DEMO.Core.Import;
using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using PagedList;

namespace MSS_DEMO.Controllers.SetUp
{
    [CheckCredential(Role_ID = "3")]
    public class StudentsController : Controller
    {
        
        private IUnitOfWork unitOfWork;
        public StudentsController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
        public ActionResult Index(StudentViewModel model, int? page, string searchCheck, string Semester_ID)
        {
            List<Student> students = new List<Student>();
            string SearchString = model.Email;
            if (searchCheck != null)
            {
                students = unitOfWork.Students.GetPageList();
            }
            else
            {
                page = 1;
            }
            if (!String.IsNullOrEmpty(searchCheck))
            {
                if (!String.IsNullOrWhiteSpace(SearchString))
                {
                    students = students.Where(s => s.Email.Trim().ToUpper().Contains(SearchString.Trim().ToUpper())).ToList();
                }
                if (!String.IsNullOrWhiteSpace(model.Semester_ID))
                {
                    students = students.Where(s => s.Semester_ID.Trim().Equals(model.Semester_ID.Trim())).ToList();
                }
                if (!String.IsNullOrWhiteSpace(model.Campus_ID))
                {
                    students = students.Where(s => s.Campus_ID.Trim().Equals(model.Campus_ID.Trim())).ToList();
                }
                if (students.Count == 0)
                {
                    ViewBag.Nodata = "Showing 0 results";
                }
                else
                {
                    ViewBag.Nodata = "";
                }
            }
            List<SelectListItem> semesterList = new List<SelectListItem>();
            var semester = unitOfWork.Semesters.GetAll();
            foreach (var sem in semester)
            {
                semesterList.Add(new SelectListItem
                {
                    Text = sem.Semester_Name,
                    Value = sem.Semester_ID
                });
            }
            List<SelectListItem> campusList = new List<SelectListItem>();
            var campus = unitOfWork.Campus.GetAll();
            foreach (var cam in campus)
            {
                campusList.Add(new SelectListItem
                {
                    Text = cam.Campus_Name,
                    Value = cam.Campus_ID
                });
            }
            model.lstSemester = semesterList;
            model.lstCampus = campusList;
            model.searchCheck = searchCheck;
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            List<StudentSubjectView> lst = new List<StudentSubjectView>();
            foreach (var stud in students)
            {
                lst.Add(new StudentSubjectView
                {
                    Roll = stud.Roll,
                    Full_Name = stud.Full_Name,
                    Email = stud.Email,
                    Campus_ID = stud.Campus_ID,
                    Semester = stud.Semester,
                    Subject = unitOfWork.SubjectStudent.getListSubject(stud.Roll + "^" + stud.Semester_ID).ToString()
            });
            }
            model.PageList = lst.ToList().ToPagedList(pageNumber, pageSize);
            ViewBag.Count = lst.Count();
            return View(model);

        }
        public ActionResult Details(string id)
        {

            ViewBag.Subject = unitOfWork.SubjectStudent.getListSubject(id);
            var student = unitOfWork.Students.getByRollAndSemester(id);
            return View(student);
        }
        public ActionResult Create()
        {
            GetListSelect();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student, string Subject_ID, string Campus_ID)
        {
            GetListSelect();
            student.Campus_ID = Campus_ID;
            Subject_Student list = new Subject_Student();
            list = new Subject_Student
            {
                Subject_ID = Subject_ID,
                Roll = student.Roll,
                Semester_ID = student.Semester_ID
            };
            if (ModelState.IsValid)
            {
                if (unitOfWork.Subject.CheckExitsSubject(Subject_ID))
                {
                    if (unitOfWork.Students.IsExtisStudent(student.Roll, student.Semester_ID)
                        && unitOfWork.Subject.IsExitsSubject(Subject_ID))
                    {
                        if (unitOfWork.SubjectStudent.IsExitsSubjectStudent(student.Roll, Subject_ID))
                        {
                            ViewBag.Error = "Student  " + student.Roll + " has registered for subject " + Subject_ID;
                            return View(student);
                        }
                        unitOfWork.SubjectStudent.Insert(list);
                    }
                    else
                    if (!unitOfWork.Students.IsExtisStudent(student.Roll, student.Semester_ID))
                    {
                        unitOfWork.Students.Insert(student);
                        unitOfWork.SubjectStudent.Insert(list);
                    }
                }
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(student);
        }


        public ActionResult Edit(string id)
        {

            List<Campu> cam = unitOfWork.Campus.GetAll();
            List<Campu> _cam = new List<Campu>();
            _cam.Add(new Campu { Campus_ID = "", Campus_Name = "--- Choose Campus ---" });
            foreach (var c in cam)
            {
                _cam.Add(c);
            }
            ViewBag.Campus_ID = new SelectList(_cam, "Campus_ID", "Campus_Name");
            var student = unitOfWork.Students.getByRollAndSemester(id);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student, string Campus_ID)
        {
            student.Campus_ID = Campus_ID;
            if (ModelState.IsValid)
            {
                unitOfWork.Students.Update(student);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(student);
        }
        public ActionResult Delete(string id)
        {
            //StringBuilder sb = new StringBuilder();
            //var subject = unitOfWork.SubjectStudent.GetAll().Where(s => s.Roll == id).ToList();
            //foreach (var _subject in subject)
            //{
            //    sb.Append(" - " + _subject.Subject_ID);
            //}
            //sb.Remove(0, 2);
            //ViewBag.Subject = sb;
            var student = unitOfWork.Students.getByRollAndSemester(id);
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var student = unitOfWork.Students.getByRollAndSemester(id);
            var subject = unitOfWork.SubjectStudent.GetAll();
            foreach (var _subject in subject)
            {
                if ((_subject.Roll == student.Roll) && (_subject.Semester_ID == student.Semester_ID))
                {
                    unitOfWork.SubjectStudent.Delete(_subject);
                }
            }
            unitOfWork.Students.Delete(student);
            if (!unitOfWork.Save())
            {
                ViewBag.mess = "Can't delete this student!";
                return View(student);
            }
            return RedirectToAction("Index");
        }
        public void Export_Student_CSV()
        {
            CSVConvert csv = new CSVConvert();
            var sb = new StringBuilder();
            IEnumerable<Student> query = unitOfWork.Students.GetAll();
            var list = query.ToList();
            Type type = typeof(Student);
            var props = type.GetProperties();
            sb.Append(string.Join(",", "ROLL", "Email"));
            sb.Append(Environment.NewLine);
            foreach (var item in list)
            {
                sb.Append(string.Join(",", csv.AddCSVQuotes(item.Roll), csv.AddCSVQuotes(item.Email)));
                sb.Append(Environment.NewLine);
            }
            var response = System.Web.HttpContext.Current.Response;
            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.ContentEncoding = Encoding.Unicode;
            response.AddHeader("content-disposition", "attachment;filename=Student.CSV ");
            response.ContentType = "text/plain";
            response.Write(sb.ToString());
            response.End();
        }
        [HttpPost]
        public ActionResult GetID()
        {
            string roll = Request["roll"];
            string semesterID = Request["semesterID"];
            if (unitOfWork.Students.IsExtisStudent(roll, semesterID))
            {
                return Json(new { message = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { message = "false" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteListSubject(string Subject_ID, string Semester_ID, string Campus_ID)
        {
            string message = "";
            if (!string.IsNullOrEmpty(Subject_ID) && !string.IsNullOrEmpty(Subject_ID))
            {
                message = unitOfWork.SubjectStudent.DeleteListSubject(Subject_ID, Semester_ID, Campus_ID);
            }
            GetListSelect();
            ViewBag.mess = "";
            if (!string.IsNullOrEmpty(message) && message.Contains("success"))
            {
                ViewBag.success = message;
            }
            else
            {
                ViewBag.mess = message;
            }
            return View();
        }
        public void GetListSelect()
        {
            List<Semester> semester = unitOfWork.Semesters.GetAll();
            List<Semester> _semester = new List<Semester>();
            _semester.Add(new Semester { Semester_ID = "", Semester_Name = "--- Choose Semester ---" });
            foreach (var sem in semester)
            {
                _semester.Add(sem);
            }
            ViewBag.Semester_ID = new SelectList(_semester, "Semester_ID", "Semester_Name");
            List<Subject> sub = unitOfWork.Subject.GetAll();
            List<Subject> _sub = new List<Subject>();
            _sub.Add(new Subject { Subject_ID = "", Subject_Name = "--- Choose Subject ---" });
            foreach (var sem in sub)
            {
                _sub.Add(sem);
            }
            ViewBag.Subject_ID = new SelectList(_sub, "Subject_ID", "Subject_Name");
            List<Campu> cam = unitOfWork.Campus.GetAll();
            List<Campu> _cam = new List<Campu>();
            _cam.Add(new Campu { Campus_ID = "", Campus_Name = "--- Choose Campus ---" });
            foreach (var c in cam)
            {
                _cam.Add(c);
            }
            ViewBag.Campus_ID = new SelectList(_cam, "Campus_ID", "Campus_Name");
        }

        [HttpGet]
        public ActionResult Export(string check)
        {

            string searchCheck = check.Split('^')[0];
            string Email = check.Split('^')[1];
            string Semester_ID = check.Split('^')[2];
            string Campus_ID = check.Split('^')[3];
  
            var students = unitOfWork.Students.GetAll();

            if (searchCheck != "1")
            {
                if (Email != "2")
                {
                    students = students.Where(s => s.Email.Trim().ToUpper().Contains(Email.Trim().ToUpper())).ToList();
                }
                if (Semester_ID != "3")
                {
                    students = students.Where(s => s.Semester_ID.Trim() == Semester_ID.Trim()).ToList();
                }
                if (Campus_ID != "4")
                {
                    students = students.Where(s => s.Campus_ID.Trim() == Campus_ID.Trim()).ToList();
                }      
            }
            var myExport = new CSVExport();

            foreach (var student in students)
            {
                myExport.AddRow();
                myExport["Roll Number"] = student.Roll;
                myExport["Full Name"] = student.Full_Name;
                myExport["Email"] = student.Email;
            }

            return File(myExport.ExportToBytes(), "text/csv", "Student.csv");
        }
        [HttpGet]
        public ActionResult ExportCoursera(string check)
        {

            string searchCheck = check.Split('^')[0];
            string Email = check.Split('^')[1];
            string Semester_ID = check.Split('^')[2];
            string Campus_ID = check.Split('^')[3];

            var students = unitOfWork.Students.GetAll();

            if (searchCheck != "1")
            {
                if (Email != "2")
                {
                    students = students.Where(s => s.Email.Trim().ToUpper().Contains(Email.Trim().ToUpper())).ToList();
                }
                if (Semester_ID != "3")
                {
                    students = students.Where(s => s.Semester_ID.Trim() == Semester_ID.Trim()).ToList();
                }
                if (Campus_ID != "4")
                {
                    students = students.Where(s => s.Campus_ID.Trim() == Campus_ID.Trim()).ToList();
                }
            }
            var myExport = new CSVExport();

            foreach (var student in students)
            {
                myExport.AddRow();
                myExport["Full Name"] = student.Full_Name;
                myExport["Email"] = student.Email;
                var subject = unitOfWork.SubjectStudent.getListSubject(student.Roll + "^" + student.Semester_ID).ToString();
                myExport["External ID"] = subject +"@" + student.Campus_ID + "-" + student.Roll;
            }

            return File(myExport.ExportToBytes(), "text/csv", "Coursera-Invitation.csv");
        }
    }
}
