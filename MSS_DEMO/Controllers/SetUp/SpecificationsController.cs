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
using PagedList;

namespace MSS_DEMO.Controllers.SetUp
{
    public class SpecificationsController : Controller
    {
        private IUnitOfWork unitOfWork;
        public SpecificationsController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
        public ActionResult Index()
        {
            List<Specification> LogList = new List<Specification>();
            return View(LogList.ToList().ToPagedList(1, 1));
        }
        [HttpPost]
        public ActionResult Index(int? page, string SearchString)
        {
            List<Specification> LogList = unitOfWork.Specifications.GetPageList();
            if (!String.IsNullOrEmpty(SearchString))
            {
                LogList = LogList.Where(s => s.Specification_ID.ToUpper().Contains(SearchString.ToUpper())).ToList();
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(LogList.ToList().ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Details(string id)
        {
            var Specifications = unitOfWork.Specifications.GetById(id);
            return View(Specifications);
        }

        public ActionResult Create()
        {
            ViewBag.Subject_ID = new SelectList(unitOfWork.Subject.GetAll(), "Subject_ID", "Subject_Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Specification_ID,Specification_Name,Subject_ID,Is_Real_Specification")] Specification specification)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Specifications.Insert(specification);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.Subject_ID = new SelectList(unitOfWork.Subject.GetAll(), "Subject_ID", "Subject_Name", specification.Subject_ID);
            return View(specification);
        }


        public ActionResult Edit(string id)
        {
            var specification = unitOfWork.Specifications.GetById(id);
            ViewBag.Subject_ID = new SelectList(unitOfWork.Subject.GetAll(), "Subject_ID", "Subject_Name", specification.Subject_ID);
            return View(specification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Specification_ID,Subject_ID")] Specification specification)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Specifications.Update(specification);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.Subject_ID = new SelectList(unitOfWork.Subject.GetAll(), "Subject_ID", "Subject_Name", specification.Subject_ID);
            return View(specification);
        }

        public ActionResult Delete(string id)
        {
            var specification = unitOfWork.Specifications.GetById(id);
            return View(specification);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var specification = unitOfWork.Specifications.GetById(id);
            unitOfWork.Specifications.Delete(specification);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
