using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MSS_DEMO.Core.Import;
using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using PagedList;

namespace MSS_DEMO.Controllers.SetUp
{
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
            string SearchString = model.Roll;
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
                if (!String.IsNullOrEmpty(SearchString))
                {
                    students = students.Where(s => s.Email.ToUpper().Contains(SearchString.ToUpper())).ToList();
                }
                if (!String.IsNullOrEmpty(model.Semester_ID))
                {
                   students = students.Where(s => s.Semester.Semester_Name.ToUpper().Contains(model.Semester_ID.ToUpper())).ToList();
                }
                if (!String.IsNullOrEmpty(model.Campus))
                {
                    students = students.Where(s => s.Campus.ToUpper().Contains(model.Campus.ToUpper())).ToList();
                }
            }           
            List<string> semester = unitOfWork.Semesters.GetAll().Select(o => o.Semester_Name).ToList();
            List<string> campus = unitOfWork.Campus.GetAll().Select(o => o.Campus_ID).ToList();
            model.lstSemester = semester;
            model.lstCampus = campus;
            model.searchCheck = searchCheck;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            model.PageList = students.ToList().ToPagedList(pageNumber, pageSize);
            return View(model);

        }
        public ActionResult Details(string id)
        {
            StringBuilder sb = new StringBuilder();
            var subject = unitOfWork.SubjectStudent.GetAll().Where(s => s.Roll == id).ToList();
            foreach (var _subject in subject)
            {
                sb.Append(" - " + _subject.Subject_ID);
            }
            sb.Remove(0, 2);
            ViewBag.Subject = sb;
            var student = unitOfWork.Students.GetById(id);
            return View(student);
        }
        public ActionResult Create()
        {
            List<Semester> semester = unitOfWork.Semesters.GetAll();
            List<Semester> _semester = new List<Semester>();
            _semester.Add(new Semester { Semester_ID = "None", Semester_Name = "--- Choose Semester ---" });
            foreach (var sem in semester)
            {
                _semester.Add(sem);
            }
            ViewBag.Semester_ID = new SelectList(_semester, "Semester_ID", "Semester_Name");
            List<Subject> sub = unitOfWork.Subject.GetAll();
            List<Subject> _sub = new List<Subject>();
            _sub.Add(new Subject { Subject_ID = "None", Subject_Name = "--- Choose Subject ---" });
            foreach (var sem in sub)
            {
                _sub.Add(sem);
            }
            ViewBag.Subject_ID = new SelectList(_sub, "Subject_ID", "Subject_Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student, string Subject_ID)
        {
            Subject_Student list = new Subject_Student();
            list = new Subject_Student
            {
                Subject_ID = Subject_ID,
                Roll = student.Roll
            };
            if (ModelState.IsValid)
            {
                if (unitOfWork.Subject.CheckExitsSubject(Subject_ID))
                {
                    if (unitOfWork.Students.IsExtisStudent(student.Roll)
                        && unitOfWork.Subject.IsExitsSubject(Subject_ID))
                    {
                        unitOfWork.SubjectStudent.Insert(list);
                        // unitOfWork.ClassStudent.Insert(getRow.GetClassStudent(rows));
                    }
                    else
                    if (!unitOfWork.Students.CheckExitsStudent(student.Roll))
                    {
                        unitOfWork.Students.Insert(student);
                        unitOfWork.SubjectStudent.Insert(list);
                        //unitOfWork.ClassStudent.Insert(getRow.GetClassStudent(rows));
                    }
                }
                //unitOfWork.Students.Insert(student);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(student);
        }


        public ActionResult Edit(string id)
        {
            var student = unitOfWork.Students.GetById(id);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student)
        {
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
            var student = unitOfWork.Students.GetById(id);
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var student = unitOfWork.Students.GetById(id);
            //var subject = unitOfWork.SubjectStudent.GetAll();
            //foreach (var _subject in subject)
            //{
            //    if (_subject.Roll == id)
            //    {
            //        unitOfWork.SubjectStudent.Delete(_subject);
            //    }
            //}
            unitOfWork.Students.Delete(student);         
            unitOfWork.Save();
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
            if (unitOfWork.Students.IsExtisStudent(roll))
            {
                return Json(new { message = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { message = "false" }, JsonRequestBehavior.AllowGet);
        }
    }
}
