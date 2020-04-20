using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Models;
using MSS_DEMO.Repository;

namespace MSS_DEMO.Controllers
{
    public class SemestersController : Controller
    {
        private IUnitOfWork unitOfWork;
        public SemestersController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
        public ActionResult Index()
        {
            return View(unitOfWork.Semesters.toList());
        }

        public ActionResult Details(string id)
        {
            var semester = unitOfWork.Semesters.GetById(id);
            return View(semester);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Semester_ID,Semester_Name")] Semester semester,string Start_Date,string End_Date)
        {
            Start_Date =  DateTime.ParseExact(Start_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            End_Date = DateTime.ParseExact(End_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            semester.Start_Date = DateTime.Parse(Start_Date);
            semester.End_Date = DateTime.Parse(End_Date);
            if (unitOfWork.Semesters.IsExitsSemester(semester.Semester_ID,semester.Semester_Name))
            {
                ViewBag.Error = "This semester exits!";
                return View();
            }
            if (ModelState.IsValid)
            {
                unitOfWork.Semesters.Insert(semester);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(semester);
        }
        public ActionResult Edit(string id)
        {
            var semester = unitOfWork.Semesters.GetById(id);
            return View(semester);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Semester_ID,Semester_Name,Start_Date,End_Date")] Semester semester)
        { 
            if (ModelState.IsValid)
            {
                unitOfWork.Semesters.Update(semester);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(semester);
        }

        public ActionResult Delete(string id)
        {
            var semester = unitOfWork.Semesters.GetById(id);
            return View(semester);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var semester = unitOfWork.Semesters.GetById(id);
            unitOfWork.Semesters.Delete(semester);
            if (!unitOfWork.Save())
            {
                ViewBag.mess = "Can't delete this semester!";
                return View(semester);
            }
            return RedirectToAction("Index");
        }
    }
}
