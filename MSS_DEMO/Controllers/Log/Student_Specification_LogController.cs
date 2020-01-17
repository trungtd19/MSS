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
    public class Student_Specification_LogController : Controller
    {
        private MSSEntities db = new MSSEntities();

        // GET: Student_Specification_Log
        public ActionResult Index()
        {
            var student_Specification_Log = db.Student_Specification_Log.Include(s => s.Specification).Include(s => s.Student);
            return View(student_Specification_Log.ToList());
        }

        // GET: Student_Specification_Log/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student_Specification_Log student_Specification_Log = db.Student_Specification_Log.Find(id);
            if (student_Specification_Log == null)
            {
                return HttpNotFound();
            }
            return View(student_Specification_Log);
        }

        // GET: Student_Specification_Log/Create
        public ActionResult Create()
        {
            ViewBag.Specification_ID = new SelectList(db.Specifications, "Specification_ID", "Subject_ID");
            ViewBag.Roll = new SelectList(db.Students, "Roll", "Email");
            return View();
        }

        // POST: Student_Specification_Log/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Roll,Subject_ID,Campus,Specialization,Specialization_Slug,University,Specialization_Enrollment_Time,Last_Specialization_Activity_Time,Completed,Status,Program_Slug,Program_Name,Specialization_Completion_Time,Specification_ID,Course_ID")] Student_Specification_Log student_Specification_Log)
        {
            if (ModelState.IsValid)
            {
                db.Student_Specification_Log.Add(student_Specification_Log);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Specification_ID = new SelectList(db.Specifications, "Specification_ID", "Subject_ID", student_Specification_Log.Specification_ID);
            ViewBag.Roll = new SelectList(db.Students, "Roll", "Email", student_Specification_Log.Roll);
            return View(student_Specification_Log);
        }

        // GET: Student_Specification_Log/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student_Specification_Log student_Specification_Log = db.Student_Specification_Log.Find(id);
            if (student_Specification_Log == null)
            {
                return HttpNotFound();
            }
            ViewBag.Specification_ID = new SelectList(db.Specifications, "Specification_ID", "Subject_ID", student_Specification_Log.Specification_ID);
            ViewBag.Roll = new SelectList(db.Students, "Roll", "Email", student_Specification_Log.Roll);
            return View(student_Specification_Log);
        }

        // POST: Student_Specification_Log/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Roll,Subject_ID,Campus,Specialization,Specialization_Slug,University,Specialization_Enrollment_Time,Last_Specialization_Activity_Time,Completed,Status,Program_Slug,Program_Name,Specialization_Completion_Time,Specification_ID,Course_ID")] Student_Specification_Log student_Specification_Log)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student_Specification_Log).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Specification_ID = new SelectList(db.Specifications, "Specification_ID", "Subject_ID", student_Specification_Log.Specification_ID);
            ViewBag.Roll = new SelectList(db.Students, "Roll", "Email", student_Specification_Log.Roll);
            return View(student_Specification_Log);
        }

        // GET: Student_Specification_Log/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student_Specification_Log student_Specification_Log = db.Student_Specification_Log.Find(id);
            if (student_Specification_Log == null)
            {
                return HttpNotFound();
            }
            return View(student_Specification_Log);
        }

        // POST: Student_Specification_Log/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student_Specification_Log student_Specification_Log = db.Student_Specification_Log.Find(id);
            db.Student_Specification_Log.Remove(student_Specification_Log);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult DeleteAll()
        {
            db.Database.ExecuteSqlCommand("Delete from Student_Specification_Log");
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
