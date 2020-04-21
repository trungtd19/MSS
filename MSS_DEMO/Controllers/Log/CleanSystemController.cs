using MSS_DEMO.Common;
using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSS_DEMO.Controllers.Log
{
    [CheckCredential(Role_ID = "1")]
    public class CleanSystemController : Controller
    {
        private IUnitOfWork unitOfWork;
        public CleanSystemController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UsageReport(string ImportedDate, string Semester_ID, string checkDelete)
        {
            List<Semester> semester = unitOfWork.Semesters.GetAll();
            ViewBag.Semester_ID = new SelectList(semester, "Semester_ID", "Semester_Name");
            var listDate = unitOfWork.CoursesLog.GetAll().OrderByDescending(o => o.Date_Import).Select(o => o.Date_Import).Distinct();
            List<string> date = new List<string>();
            foreach (var _date in listDate)
            {
                date.Add(Convert.ToDateTime(_date).ToString("dd/MM/yyyy"));
            }
            date = date.Distinct().ToList();
            ViewBag.ImportedDate = date;
            
            if (!string.IsNullOrEmpty(ImportedDate) || !string.IsNullOrEmpty(Semester_ID))
            {
                int deleteRecord = 0;
                deleteRecord = unitOfWork.CoursesLog.CleanUsageReport(ImportedDate, Semester_ID);
                if (deleteRecord != 0)
                {
                    ViewBag.success = "Delete success " + deleteRecord + " records";
                }
                else
                {
                    ViewBag.error = "Delete 0 record";
                }
            }
            else
            if (!string.IsNullOrEmpty(checkDelete))
            {
                ViewBag.error = "Choose Reported date or Semester";
            }
            
            return View();
        }
        public ActionResult SpecificationReport(string ImportedDate, string Semester_ID, string checkDelete)
        {
            List<Semester> semester = unitOfWork.Semesters.GetAll();
            ViewBag.Semester_ID = new SelectList(semester, "Semester_ID", "Semester_Name");
            var listDate = unitOfWork.SpecificationsLog.GetAll().OrderByDescending(o => o.Date_Import).Select(o => o.Date_Import).Distinct();
            List<string> date = new List<string>();
            foreach (var _date in listDate)
            {
                date.Add(Convert.ToDateTime(_date).ToString("dd/MM/yyyy"));
            }
            date = date.Distinct().ToList();
            ViewBag.ImportedDate = date;

            if (!string.IsNullOrEmpty(ImportedDate) || !string.IsNullOrEmpty(Semester_ID))
            {
                int deleteRecord = 0;
                deleteRecord = unitOfWork.SpecificationsLog.CleanSpecificationReport(ImportedDate, Semester_ID);
                if (deleteRecord != 0)
                {
                    ViewBag.success = "Delete success " + deleteRecord + " records";
                }
                else
                {
                    ViewBag.error = "Delete 0 record";
                }
            }
            else
            if (!string.IsNullOrEmpty(checkDelete))
            {
                ViewBag.error = "Choose Reported date or Semester";
            }

            return View();
        }
    }
}