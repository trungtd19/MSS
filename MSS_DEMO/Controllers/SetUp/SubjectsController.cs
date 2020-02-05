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
    public class SubjectsController : Controller
    {
        private IUnitOfWork unitOfWork;
        public SubjectsController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
        public ActionResult Index()
        {
            var subjects = unitOfWork.Subject.GetAll();
            return View(subjects);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Subject_ID,Subject_Name")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Subject.Insert(subject);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(subject);
        }


        public ActionResult Edit(string id)
        {
            var subject = unitOfWork.Subject.GetById(id);
            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Subject_ID,Subject_Name")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Subject.Update(subject);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(subject);
        }
        public ActionResult Delete(string id)
        {
            var subject = unitOfWork.Subject.GetById(id);
            return View(subject);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var subject = unitOfWork.Subject.GetById(id);
            unitOfWork.Subject.Delete(subject);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
