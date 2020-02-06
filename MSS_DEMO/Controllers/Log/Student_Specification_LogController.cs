﻿using System;
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

namespace MSS_DEMO.Controllers.Log
{
    public class Student_Specification_LogController : Controller
    {
        private IUnitOfWork unitOfWork;
        public Student_Specification_LogController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
        public ActionResult Index(int? page)
        {
            List<Student_Specification_Log> students = unitOfWork.SpecificationsLog.GetPageList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(students.ToList().ToPagedList(pageNumber, pageSize));

        }
        public ActionResult DeleteAll()
        {
            MSSEntities db = new MSSEntities();
            db.Database.ExecuteSqlCommand("Delete from Student_Specification_Log");
            return RedirectToAction("Index");
        }     
    }
}
