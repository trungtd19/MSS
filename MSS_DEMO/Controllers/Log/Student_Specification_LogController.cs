using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
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

        public ActionResult Index(int? page, string SearchString,string dateImport , string dateFilter,string searchCheck, string currentFilter, string SelectFilter, string SelectString)
        {
            List<Student_Specification_Log> LogList = new List<Student_Specification_Log>();
            if (SearchString != null && dateImport != null && SearchString != "")
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
                dateImport = dateFilter;
                SearchString = SelectFilter;
            }
            string date = String.IsNullOrEmpty(dateImport) ? "1900/01/01" : dateImport.Replace("-", "/");
            DateTime _dateImport = DateTime.ParseExact(date, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            ViewBag.CurrentFilter = SearchString;
            ViewBag.DateFilter = dateImport;
            if (!String.IsNullOrEmpty(searchCheck))
            {
                LogList = unitOfWork.SpecificationsLog.GetPageList();
                if (!String.IsNullOrEmpty(SearchString))
                {
                    LogList = LogList.Where(s => s.Roll.ToUpper().Contains(SearchString.ToUpper())).ToList();
                }
                if (!String.IsNullOrEmpty(dateImport))
                {
                    LogList = LogList.Where(s => s.Date_Import == _dateImport).ToList();
                }
                if (!String.IsNullOrEmpty(SelectString))
                {
                    LogList = SelectFilter == "Compeleted" ? LogList.Where(s => s.Completed == true).ToList() : LogList.Where(s => s.Completed == true).ToList();
                }
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.SelectString = new List<SelectListItem>
                {
                     new SelectListItem { Selected = true, Text = "---None---", Value = ""},
                     new SelectListItem { Selected = false, Text = "Compeleted", Value = "Compeleted"},
                     new SelectListItem { Selected = false, Text = "Compulsory", Value = "Compulsory"},
                };
            return View(LogList.ToList().ToPagedList(pageNumber, pageSize));

        }
        public ActionResult DeleteAll()
        {
            MSSEntities db = new MSSEntities();
            db.Database.ExecuteSqlCommand("Delete from Student_Specification_Log");
            return RedirectToAction("Index");
        }     
    }
}
