using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Models;

namespace MSS_DEMO.Controllers
{
    public class CampusController : Controller
    {
        private MSSEntities db = new MSSEntities();

        // GET: Campus
        public ActionResult Index()
        {
            return View(db.Campus.ToList());
        }

        // GET: Campus/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campu campu = db.Campus.Find(id);
            if (campu == null)
            {
                return HttpNotFound();
            }
            return View(campu);
        }

        // GET: Campus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Campus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Campus_ID,Campus_Name,Address,Contact_Point")] Campu campu)
        {
            if (ModelState.IsValid)
            {
                db.Campus.Add(campu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(campu);
        }

        // GET: Campus/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campu campu = db.Campus.Find(id);
            if (campu == null)
            {
                return HttpNotFound();
            }
            return View(campu);
        }

        // POST: Campus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Campus_ID,Campus_Name,Address,Contact_Point")] Campu campu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(campu);
        }

        // GET: Campus/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campu campu = db.Campus.Find(id);
            if (campu == null)
            {
                return HttpNotFound();
            }
            return View(campu);
        }

        // POST: Campus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Campu campu = db.Campus.Find(id);
            db.Campus.Remove(campu);
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
