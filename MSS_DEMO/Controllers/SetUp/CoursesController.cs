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
        private const string NONE = "--- None ---";
        private const string NOTMAP = "Not Map";
        private IUnitOfWork unitOfWork;
        public CoursesController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
  
        public ActionResult Index(int? page, string SearchString, string searchCheck, string currentFilter)
        {
            List<Course> LogList = new List<Course>();
            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }
            ViewBag.CurrentFilter = SearchString;
            if (!String.IsNullOrEmpty(searchCheck))
            {
               LogList = unitOfWork.Courses.GetPageList();
                if (!String.IsNullOrEmpty(SearchString))
                {
                    LogList = LogList.Where(s => s.Course_ID.ToUpper().Contains(SearchString.ToUpper())).ToList();
                }
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(LogList.ToList().ToPagedList(pageNumber, pageSize));

        }
        public ActionResult Details(string id)
        {
            var Courses = unitOfWork.Courses.GetById(id);
            if (Courses.Specification_ID == null) Courses.Specification_ID = NOTMAP;
            return View(Courses);
        }
        public ActionResult Create()
        {
            SelectSpecID();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Course_ID,Course_Name,Course_Slug,Specification_ID")] Course course)
        {
            if (course.Specification_ID == NONE) course.Specification_ID = null;
            if (ModelState.IsValid)
            {
                unitOfWork.Courses.Insert(course);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.Specification_ID = new SelectList(unitOfWork.Specifications.GetAll(), "Specification_ID", "Specification_ID", course.Specification_ID);
            return View(course);
        }


        public ActionResult Edit(string id)
        {
            Course course = unitOfWork.Courses.GetById(id);
            SelectSpecID();
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Course_ID,Course_Name,Course_Slug,Specification_ID")] Course course)
        {
            if (course.Specification_ID == NONE) course.Specification_ID = null;
            if (ModelState.IsValid)
            {
                unitOfWork.Courses.Update(course);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.Specification_ID = new SelectList(unitOfWork.Specifications.GetAll(), "Specification_ID", "Specification_ID", course.Specification_ID);
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
      
        public void SelectSpecID()
        {
            List<Specification> _specs = new List<Specification>();
            List<Specification> specs = unitOfWork.Specifications.GetAll();
            _specs.Add(new Specification { Specification_ID = NONE, Is_Real_Specification = false });

            foreach (var spec in specs)
            {
                if (!spec.Is_Real_Specification)
                {
                    _specs.Add(spec);
                }
            }
            ViewBag.Specification_ID = new SelectList(_specs, "Specification_ID", "Specification_ID");
        }
    }
}
