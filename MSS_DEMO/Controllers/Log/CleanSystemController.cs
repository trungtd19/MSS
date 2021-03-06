﻿using MSS_DEMO.Common;
using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSS_DEMO.Controllers.Log
{
    
    public class CleanSystemController : Controller
    {
        private IUnitOfWork unitOfWork;
        public CleanSystemController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
        [CheckCredential(Role_ID = "1")]
        public ActionResult Index()
        {
            return View();
        }
        [CheckCredential(Role_ID = "1")]
        public ActionResult UsageReport(string ImportedDate, string Semester_ID, string checkDelete)
        {
            List<Semester> semester = unitOfWork.Semesters.GetAll();
            List<Semester> _semester = new List<Semester>();
            _semester.Add(new Semester { Semester_ID = "", Semester_Name = "--- Choose Semester ---" });
            foreach (var sem in semester)
            {
                _semester.Add(sem);
            }
            ViewBag.Semester_ID = new SelectList(_semester, "Semester_ID", "Semester_Name");
            List<string> date = new List<string>();
            ViewBag.ImportedDate = date;
            
            if (!string.IsNullOrEmpty(ImportedDate) && !string.IsNullOrEmpty(Semester_ID))
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
                ViewBag.error = "Choose Reported date and Semester";
            }
            
            return View();
        }
        [CheckCredential(Role_ID = "1")]
        public ActionResult SpecificationReport(string ImportedDate, string Semester_ID, string checkDelete)
        {
            List<Semester> semester = unitOfWork.Semesters.GetAll();
            List<Semester> _semester = new List<Semester>();
            _semester.Add(new Semester { Semester_ID = "", Semester_Name = "--- Choose Semester ---" });
            foreach (var sem in semester)
            {
                _semester.Add(sem);
            }
            ViewBag.Semester_ID = new SelectList(_semester, "Semester_ID", "Semester_Name");
            List<string> date = new List<string>();
            ViewBag.ImportedDate = date;

            if (!string.IsNullOrEmpty(ImportedDate) && !string.IsNullOrEmpty(Semester_ID))
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
                ViewBag.error = "Choose Reported date and Semester";
            }

            return View();
        }
        [HttpPost]
        public ActionResult getListDate(string Semester_ID)
        {
            var semester = unitOfWork.Semesters.GetById(Semester_ID);
            return (ActionResult)this.Json((object)new
            {
                list = unitOfWork.CoursesLog.getDatebySemester(semester)
            });
        }
        [HttpPost]
        public ActionResult getListDateSpec(string Semester_ID)
        {
            var semester = unitOfWork.Semesters.GetById(Semester_ID);
            return (ActionResult)this.Json((object)new
            {
                list = unitOfWork.SpecificationsLog.getDatebySemester(semester)
            });
        }
    }
}