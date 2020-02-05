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

namespace MSS_DEMO.Controllers
{
    public class CampusController : Controller
    {
        private IUnitOfWork unitOfWork;
        public CampusController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }

        public ActionResult Index()
        {
            var campus = unitOfWork.Campus.GetAll();
            return View(campus);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Campus_ID,Campus_Name,Address,Contact_Point")] Campu campu)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Campus.Insert(campu);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(campu);
        }

        public ActionResult Edit(string id)
        {
            var campu = unitOfWork.Campus.GetById(id);        
            return View(campu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Campus_ID,Campus_Name,Address,Contact_Point")] Campu campu)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Campus.Update(campu);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(campu);
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var campu = unitOfWork.Campus.GetById(id);
            if (campu == null)
            {
                return HttpNotFound();
            }
            return View(campu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var campu = unitOfWork.Campus.GetById(id);
            unitOfWork.Campus.Delete(campu);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
