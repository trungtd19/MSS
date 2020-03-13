using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Common;
using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using PagedList;

namespace MSS_DEMO.Controllers.UserManagement
{
    [CheckCredential(Role_ID = "1")]
    public class User_RoleController : Controller
    {
        private IUnitOfWork unitOfWork;
        private MSSEntities db = new MSSEntities();
        public User_RoleController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        
        public ActionResult Index(string SearchString)
        {
            IQueryable<User_Role> model = db.User_Role;
            if (!string.IsNullOrEmpty(SearchString))
            {
                model = model.Where(x => x.Login.Contains(SearchString));

            }
            return View(model.OrderByDescending(x => x.Role_ID).ToList());

        }                        
        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Role user_Role = db.User_Role.Find(id);
            if (user_Role == null)
            {
                return HttpNotFound();
            }
            return View(user_Role);
        }
        
        // GET: User_Role/Create
        public ActionResult Create()
        {
            ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "Role_Name");
            return View();
        }

        // POST: User_Role/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Create([Bind(Include = "User_ID,Role_ID,Login,isActive")] User_Role user_Role)
        {
            if (ModelState.IsValid)
            {
                user_Role.Login = user_Role.Login + "@fpt.edu.vn";
                db.User_Role.Add(user_Role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "Role_Name", user_Role.Role_ID);
            return View(user_Role);
        }

        // GET: User_Role/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Role user_Role = db.User_Role.Find(id);
            string[] arrListStr = user_Role.Login.Split('@');
            user_Role.Login = arrListStr[0];
            if (user_Role == null)
            {
                return HttpNotFound();
            }
            ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "Role_Name", user_Role.Role_ID);
            return View(user_Role);
        }

        // POST: User_Role/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "User_ID,Role_ID,Login,isActive")] User_Role user_Role)
        {
            if (ModelState.IsValid)
            {
                user_Role.Login = user_Role.Login + "@fpt.edu.vn";
                db.Entry(user_Role).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Role_ID = new SelectList(db.Roles, "Role_ID", "Role_Name", user_Role.Role_ID);
            return View(user_Role);
        }

        // GET: User_Role/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Role user_Role = db.User_Role.Find(id);
            if (user_Role == null)
            {
                return HttpNotFound();
            }
            return View(user_Role);
        }

        // POST: User_Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User_Role user_Role = db.User_Role.Find(id);
            db.User_Role.Remove(user_Role);
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
