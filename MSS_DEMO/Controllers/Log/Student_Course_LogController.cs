using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Models;

namespace MSS_DEMO.Controllers.Log
{
    public class Student_Course_LogController : Controller
    {
        private MSSEntities db = new MSSEntities();

        // GET: Student_Course_Log
        public ActionResult Index()
        {
            var student_Course_Log = db.Student_Course_Log.Include(s => s.Course).Include(s => s.Student);
            return View(student_Course_Log.ToList());
        }

        // GET: Student_Course_Log/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student_Course_Log student_Course_Log = db.Student_Course_Log.Find(id);
            if (student_Course_Log == null)
            {
                return HttpNotFound();
            }
            return View(student_Course_Log);
        }

        // GET: Student_Course_Log/Create
        public ActionResult Create()
        {
            ViewBag.Course_ID = new SelectList(db.Courses, "Course_ID", "Course_Name");
            ViewBag.Roll = new SelectList(db.Students, "Roll", "Email");
            return View();
        }

        // POST: Student_Course_Log/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Course_Enrollment_Time,Course_Start_Time,Last_Course_Activity_Time,Overall_Progress,Estimated,Completed,Status,Program_Slug,Program_Name,Completion_Time,Course_ID,Course_Grade,Roll")] Student_Course_Log student_Course_Log)
        {
            if (ModelState.IsValid)
            {
                db.Student_Course_Log.Add(student_Course_Log);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Course_ID = new SelectList(db.Courses, "Course_ID", "Course_Name", student_Course_Log.Course_ID);
            ViewBag.Roll = new SelectList(db.Students, "Roll", "Email", student_Course_Log.Roll);
            return View(student_Course_Log);
        }

        // GET: Student_Course_Log/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student_Course_Log student_Course_Log = db.Student_Course_Log.Find(id);
            if (student_Course_Log == null)
            {
                return HttpNotFound();
            }
            ViewBag.Course_ID = new SelectList(db.Courses, "Course_ID", "Course_Name", student_Course_Log.Course_ID);
            ViewBag.Roll = new SelectList(db.Students, "Roll", "Email", student_Course_Log.Roll);
            return View(student_Course_Log);
        }

        // POST: Student_Course_Log/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Course_Enrollment_Time,Course_Start_Time,Last_Course_Activity_Time,Overall_Progress,Estimated,Completed,Status,Program_Slug,Program_Name,Completion_Time,Course_ID,Course_Grade,Roll")] Student_Course_Log student_Course_Log)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student_Course_Log).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Course_ID = new SelectList(db.Courses, "Course_ID", "Course_Name", student_Course_Log.Course_ID);
            ViewBag.Roll = new SelectList(db.Students, "Roll", "Email", student_Course_Log.Roll);
            return View(student_Course_Log);
        }

        // GET: Student_Course_Log/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student_Course_Log student_Course_Log = db.Student_Course_Log.Find(id);
            if (student_Course_Log == null)
            {
                return HttpNotFound();
            }
            return View(student_Course_Log);
        }

        // POST: Student_Course_Log/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student_Course_Log student_Course_Log = db.Student_Course_Log.Find(id);
            db.Student_Course_Log.Remove(student_Course_Log);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult DeleteAll()
        {
            db.Database.ExecuteSqlCommand("Delete from Student_Course_Log");
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
