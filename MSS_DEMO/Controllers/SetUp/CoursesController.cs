using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using PagedList;

namespace MSS_DEMO.Controllers.SetUp
{
    public class CoursesController : Controller
    {
        private IUnitOfWork unitOfWork;
        public CoursesController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
        public ActionResult Index()
        {
            List<Course> LogList = new List<Course>();
            return View(LogList.ToList().ToPagedList(1, 1));
        }
        [HttpPost]
        public ActionResult Index(int? page, string SearchString)
        {
            List<Course> LogList = unitOfWork.Courses.GetPageList();
            if (!String.IsNullOrEmpty(SearchString))
            {
                LogList = LogList.Where(s => s.Course_ID.ToUpper().Contains(SearchString.ToUpper())).ToList();
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(LogList.ToList().ToPagedList(pageNumber, pageSize));

        }
        public ActionResult Details(string id)
        {
            var Courses = unitOfWork.Courses.GetById(id);
            return View(Courses);
        }
        public ActionResult Create()
        {
            List<Specification> spec = new List<Specification>();

            foreach (var coures in unitOfWork.Specifications.GetAll())
            {
                if (!coures.Is_Real_Specification) {
                    spec.Add(coures);
                }
            }
            ViewBag.Specification_ID = new SelectList(spec, "Specification_ID", "Subject_ID");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Course_ID,Course_Name,Course_Slug,Specification_ID")] Course course)
        {
            if (ModelState.IsValid)
            {

                unitOfWork.Courses.Insert(course);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.Specification_ID = new SelectList(unitOfWork.Specifications.GetAll(), "Specification_ID", "Subject_ID", course.Specification_ID);
            return View(course);
        }


        public ActionResult Edit(string id)
        {
            Course course = unitOfWork.Courses.GetById(id);
            ViewBag.Specification_ID = new SelectList(unitOfWork.Specifications.GetAll(), "Specification_ID", "Subject_ID", course.Specification_ID);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Course_ID,Course_Name,Course_Slug,Specification_ID")] Course course)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Courses.Update(course);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.Specification_ID = new SelectList(unitOfWork.Specifications.GetAll(), "Specification_ID", "Subject_ID", course.Specification_ID);
            return View(course);
        }

        public ActionResult Delete(string id)
        {

            Course course = unitOfWork.Courses.GetById(id);
            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var course = unitOfWork.Courses.GetById(id);
            unitOfWork.Courses.Delete(course);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
      
    }
}
