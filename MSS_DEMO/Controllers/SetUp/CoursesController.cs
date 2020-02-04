using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Models;

namespace MSS_DEMO.Controllers.SetUp
{
    public class CoursesController : Controller
    {
        private MSSEntities db = new MSSEntities();

        public ActionResult Index()
        {
            var courses = db.Courses.Include(c => c.Specification);
            return View(courses.ToList());
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }


        public ActionResult Create()
        {
            List<Specification> spec = new List<Specification>();

            foreach (var coures in db.Specifications)
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
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Specification_ID = new SelectList(db.Specifications, "Specification_ID", "Subject_ID", course.Specification_ID);
            return View(course);
        }


        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.Specification_ID = new SelectList(db.Specifications, "Specification_ID", "Subject_ID", course.Specification_ID);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Course_ID,Course_Name,Course_Slug,Specification_ID")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Specification_ID = new SelectList(db.Specifications, "Specification_ID", "Subject_ID", course.Specification_ID);
            return View(course);
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
