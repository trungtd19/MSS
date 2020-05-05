using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MSS_DEMO.Core.Implement;
using MSS_DEMO.Core.Import;
using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using PagedList;

namespace MSS_DEMO.Controllers.SetUp
{
    public class Course_DeadlineController : Controller
    {
        private IUnitOfWork unitOfWork;
        public Course_DeadlineController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }

        public ActionResult Index(DeadlineCoursesViewModel model, int? page, string searchCheck)
        {
            List<Cour_dealine> List = new List<Cour_dealine>();
            string SearchString = model.Courses_Name;
            if (searchCheck != null)
            {
                List = unitOfWork.DeadLine.GetPageList(null, null);
            }
            else
            {
                page = 1;
            }
            if (!String.IsNullOrEmpty(searchCheck))
            {
                List = unitOfWork.DeadLine.GetPageList(SearchString, model.Semester_ID);
                
                if (List.Count == 0)
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
            model.lstSemester = semesterList;
            model.searchCheck = searchCheck;
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            ViewBag.Count = List.Count();
            model.PageList = List.ToList().ToPagedList(pageNumber, pageSize);
            return View(model);

        }

        public ActionResult Details(int? id)
        {
            return View(unitOfWork.DeadLine.GetById(id));
        }

        public ActionResult Create()
        {
            ViewBag.Subject_ID = new SelectList(unitOfWork.Subject.GetAll().Where(o => o.Subject_Active == true).ToList(), "Subject_ID", "Subject_Name");
            List<Course> list = new List<Course>();
            ViewBag.Course_ID = new SelectList(list, "Course_ID", "Course_Name");
            ViewBag.Semester_ID = new SelectList(unitOfWork.Semesters.GetAll(), "Semester_ID", "Semester_Name");
            return View();
        }
        [HttpPost]
        public ActionResult getListCourse(string Subject_ID)
        {
            return (ActionResult)this.Json((object)new
            {
                list = unitOfWork.Courses.GetList().Where(o => o.Subject_ID == Subject_ID).ToList()
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Semester_ID,Course_ID,Course_Deadline_ID")] Course_Deadline course_Deadline, string Deadline)
        {
            Deadline = DateTime.ParseExact(Deadline, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            course_Deadline.Deadline = DateTime.Parse(Deadline);
            ViewBag.CheckExits = "";
            ViewBag.Subject_ID = new SelectList(unitOfWork.Subject.GetAll().Where(o => o.Subject_Active == true).ToList(), "Subject_ID", "Subject_Name");
            List<Course> list = new List<Course>();
            ViewBag.Course_ID = new SelectList(list, "Course_ID", "Course_Name");
            ViewBag.Semester_ID = new SelectList(unitOfWork.Semesters.GetAll(), "Semester_ID", "Semester_Name");
            if (unitOfWork.DeadLine.IsExitsDeadline(course_Deadline))
            {
                ViewBag.CheckExits = "true";
                return View();
            }
            if (ModelState.IsValid)
            {
                unitOfWork.DeadLine.Insert(course_Deadline);
                unitOfWork.Save();
                ViewBag.success = "Add successfull";
                return View();
            }        
            return View(course_Deadline);
        }

        public ActionResult Edit(int? id)
        {
            ViewBag.Course_ID = new SelectList(unitOfWork.Courses.GetAll(), "Course_ID", "Course_Name");
            ViewBag.Semester_ID = new SelectList(unitOfWork.Semesters.GetAll(), "Semester_ID", "Semester_Name");
            var deadline = unitOfWork.DeadLine.GetById(id);
            return View(deadline);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Semester_ID,Course_ID,Course_Deadline_ID")] Course_Deadline course_Deadline, string Deadline)
        {
            Deadline = DateTime.ParseExact(Deadline, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            course_Deadline.Deadline = DateTime.Parse(Deadline);
            if (ModelState.IsValid)
            {
                unitOfWork.DeadLine.Update(course_Deadline);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.Course_ID = new SelectList(unitOfWork.Courses.GetAll(), "Course_ID", "Course_Name");
            ViewBag.Semester_ID = new SelectList(unitOfWork.Semesters.GetAll(), "Semester_ID", "Semester_Name");
            return View(course_Deadline);
        }

        public ActionResult Delete(int? id)
        {
            return View(unitOfWork.DeadLine.GetById(id));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var deadline = unitOfWork.DeadLine.GetById(id);
            unitOfWork.DeadLine.Delete(deadline);
            if (!unitOfWork.Save())
            {
                ViewBag.mess = "Can't delete this deadline!";
                return View(deadline);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult ExportCoursesDeadline(string check)
        {

            string searchCheck = check.Split('^')[0];
            string Course_Name = check.Split('^')[1];
            string Semester_ID = check.Split('^')[2];
            Course_Name = Course_Name == "2" ? "" : Course_Name;
            Semester_ID = Semester_ID == "3" ? "" : Semester_ID;
            List<Cour_dealine> list = new List<Cour_dealine>();

            if (searchCheck != "1")
            {
                list = unitOfWork.DeadLine.GetPageList(Course_Name, Semester_ID);
            }
            var myExport = new CSVExport();

            foreach (var course in list)
            {
                myExport.AddRow();
                myExport[""] = course.groupRowNo;
                myExport["Subject ID"] = course.Subject_ID;
                myExport["Course Name"] = course.Courses_Name;
                myExport["Deadline"] = course.deadlineString;
                myExport["Semester"] = course.Semester_Name;
            }

            return File(myExport.ExportToBytes(), "text/csv", "Courses-Deadline.csv");
        }

    }
}
