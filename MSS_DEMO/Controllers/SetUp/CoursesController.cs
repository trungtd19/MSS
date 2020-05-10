﻿  using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Common;
using MSS_DEMO.Core.Components;
using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using PagedList;

namespace MSS_DEMO.Controllers.SetUp
{
    [CheckCredential(Role_ID = "1")]
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
            List<Cours_Spec> LogList = new List<Cours_Spec>();
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
                if (!String.IsNullOrWhiteSpace(SearchString))
                {
                    LogList = LogList.Where(s => s.Course_Name.Trim().ToUpper().Contains(SearchString.Trim().ToUpper())).ToList();
                }
                if (LogList.Count == 0)
                {
                    ViewBag.Nodata = "Showing 0 results";
                }
                else
                {
                    ViewBag.Nodata = "";
                }
            }
            ViewBag.Count = LogList.Count();
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            return View(LogList.ToList().ToPagedList(pageNumber, pageSize));

        }
        public ActionResult Details(int id)
        {
            var Courses = unitOfWork.Courses.GetListByID(id);
            return View(Courses);
        }
        public ActionResult Create()
        {
            SelectSpecID();
            specList();
            listSemester();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Course_Name,Specification_ID")] Course course, string Specification_Name)
        {
            SelectSpecID();
            specList();
            listSemester();
            if (Specification_Name != "-1" ) course.Specification_ID = int.Parse(Specification_Name);
            if (unitOfWork.Courses.IsExitsCourse(course.Specification_ID , course.Course_Name))
            {
                ViewBag.Message = "This course exits!";
                return View();
            }
            if (ModelState.IsValid)
            {             
                unitOfWork.Courses.Insert(course);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.Specification_ID = new SelectList(unitOfWork.Specifications.GetAll(), "Specification_ID", "Specification_ID", course.Specification_ID);
            return View(course);
        }


        public ActionResult Edit(int id)
        {
            Course course = unitOfWork.Courses.GetById(id);
            SelectSpecID();
            listSemester();
            specList();
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Course_ID,Course_Name,Specification_ID")]  Course course, string Specification_Name)
        {
            SelectSpecID();
            specList();
            listSemester();
            course.Specification_ID = int.Parse(Specification_Name);
            if (unitOfWork.Courses.IsExitsCourse(course.Specification_ID, course.Course_Name))
            {
                ViewBag.Message = "This course exits!";
                return View();
            }
            if (ModelState.IsValid)
            {              
                unitOfWork.Courses.Update(course);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.Specification_ID = new SelectList(unitOfWork.Specifications.GetAll(), "Specification_ID", "Specification_ID", course.Specification_ID);
            return View(course);
        }

        public ActionResult Delete(int id)
        {

            Course course = unitOfWork.Courses.GetById(id);
            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var course = unitOfWork.Courses.GetById(id);
            unitOfWork.Courses.Delete(course);
            if (!unitOfWork.Save())
            {
                ViewBag.mess = "Can't delete this course!";
                return View(course);
            }
            return RedirectToAction("Index");
        }
      
        public void SelectSpecID()
        {
            List<Specification> _specs = new List<Specification>();
            List<Specification> specs = unitOfWork.Specifications.GetAll();
            _specs.Add(new Specification { Specification_Name = NONE, Is_Real_Specification = false });

            foreach (var spec in specs)
            {
                //if (!spec.Is_Real_Specification)
               // {
                    _specs.Add(spec);
              //  }
            }
            ViewBag.Specification_ID = new SelectList(_specs, "Semester_Name", "Semester_Name");
        }
        public void listSemester()
        {
            SelectSpecID();
            List<Semester> semester = unitOfWork.Semesters.GetAll();
            List<Semester> _semester = new List<Semester>();
            _semester.Add(new Semester { Semester_ID = "None", Semester_Name = "--- Choose Semester ---" });
            foreach (var sem in semester)
            {
                _semester.Add(sem);
            }
            ViewBag.Semester_ID = new SelectList(_semester, "Semester_ID", "Semester_Name");
        }
        public void specList()
        {
            SelectSpecID();
            List<Specification> spec = unitOfWork.Specifications.GetAll();
            List<Specification> _spec = new List<Specification>();
            _spec.Add(new Specification { Specification_ID =-1, Specification_Name = "--- Choose Specification ---" });
            foreach (var sem in spec)
            {
                _spec.Add(sem);
            }
            ViewBag.Specification_Name = new SelectList(_spec, "Specification_ID", "Specification_Name");
        }
    }
}
