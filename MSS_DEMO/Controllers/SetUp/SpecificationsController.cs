using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Models;
using System.Data.SqlClient;

namespace MSS_DEMO.Controllers
{
    public class SpecificationsController : Controller
    {
        private MSSEntities db = new MSSEntities();

        // GET: Specifications
        public ActionResult Index()
        {
            var specifications = db.Specifications.Include(s => s.Subject);
            return View(specifications.ToList());
        }

        // GET: Specifications/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Specification specification = db.Specifications.Find(id);
            if (specification == null)
            {
                return HttpNotFound();
            }
            return View(specification);
        }

        // GET: Specifications/Create
        public ActionResult Create()
        {
         //   Subject subject = new Subject { Subject_ID = "3", Subject_Name = "None" };

            var ListSubject = new List<Subject>();
         //   ListSubject.Add(subject);
            foreach (var subjects in db.Subjects.ToList<Subject>())
            {
                ListSubject.Add(subjects);
            }
               
            ViewBag.Subject_ID = new SelectList(ListSubject, "Subject_ID", "Subject_Name");
            return View();
        }

        // POST: Specifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Specification_ID,Subject_ID")] Specification specification)
        {
            if (ModelState.IsValid)
            {
                //db.Database.ExecuteSqlCommand(
                // "Insert into Specification Values(@Specification_ID, @Subject_ID)",
                //          new SqlParameter("Specification_ID", specification.Specification_ID),
                //          new SqlParameter("Subject_ID", specification.Subject_ID));      
             db.Specifications.Add(specification);
               db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Subject_ID = new SelectList(db.Subjects, "Subject_ID", "Subject_Name", specification.Subject_ID);
            return View(specification);
        }

        // GET: Specifications/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Specification specification = db.Specifications.Find(id);
            if (specification == null)
            {
                return HttpNotFound();
            }
            ViewBag.Subject_ID = new SelectList(db.Subjects, "Subject_ID", "Subject_Name", specification.Subject_ID);
            return View(specification);
        }

        // POST: Specifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Specification_ID,Subject_ID")] Specification specification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(specification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Subject_ID = new SelectList(db.Subjects, "Subject_ID", "Subject_Name", specification.Subject_ID);
            return View(specification);
        }

        // GET: Specifications/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Specification specification = db.Specifications.Find(id);
            if (specification == null)
            {
                return HttpNotFound();
            }
            return View(specification);
        }

        // POST: Specifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Specification specification = db.Specifications.Find(id);
            db.Specifications.Remove(specification);
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
