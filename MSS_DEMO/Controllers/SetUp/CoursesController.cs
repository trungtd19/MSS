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
            return View(unitOfWork.Courses.GetAll());
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
