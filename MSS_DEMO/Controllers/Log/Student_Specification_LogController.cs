using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MSS_DEMO.Common;
using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using PagedList;

namespace MSS_DEMO.Controllers.Log
{
    [CheckCredential(Role_ID = "3")]
    public class Student_Specification_LogController : Controller
    {
        private IUnitOfWork unitOfWork;
        public Student_Specification_LogController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }

        public ActionResult Index(SpecReportViewModel model, int? page, string searchCheck)
        {
            List<Student_Specification_Log> LogList = new List<Student_Specification_Log>();
            string SearchString = model.Email;
            model.searchCheck = searchCheck;
            List<string> listSubjiect = unitOfWork.Subject.GetAll().Select(o => o.Subject_Name).ToList();
            model.listSubject = listSubjiect;

            if (searchCheck == null)
            {
                page = 1;
            }
            else
            {
                LogList = unitOfWork.SpecificationsLog.GetPageList();
            }
            if (!String.IsNullOrEmpty(searchCheck))
            {              
                if (!String.IsNullOrEmpty(SearchString))
                {
                    LogList = LogList.Where(s => s.Email.ToUpper().Contains(SearchString.ToUpper())).ToList();
                }
                if (!String.IsNullOrEmpty(model.Date_Import.ToString()))
                {
                    LogList = LogList.Where(s => s.Date_Import == model.Date_Import).ToList();
                }
                if (model.completedSpec != null)
                {
                    LogList = model.completedSpec == "Yes" ? LogList.Where(s => s.Completed == true).ToList() : LogList.Where(s => s.Completed == false).ToList();
                }
                if (model.compulsorySpec != null)
                {
                    LogList = model.compulsorySpec == "Yes" ? LogList = LogList.Where(s => s.Specification_ID != null).ToList() : LogList = LogList.Where(s => s.Specification_ID == null).ToList();
                }
                if (!String.IsNullOrEmpty(model.Subject_Name))
                {
                    var sub = unitOfWork.Subject.GetAll().Where(x => x.Subject_Name == model.Subject_Name).Select(y => y.Subject_ID).FirstOrDefault();
                    LogList = LogList.Where(s => s.Subject_ID == sub).ToList();
                }
                if (LogList.Count == 0)
                {
                    ViewBag.Nodata = "Not found data";
                }
                else
                {
                    ViewBag.Nodata = "";
                }
            }
            List<string> listCompleted = new List<string>() { "Yes", "No" };
            List<string> listCompulsory = new List<string>() { "Yes", "No" };
            model.listCompleted = listCompleted;
            model.listCompulsory = listCompulsory;
            ViewBag.Count = LogList.Count();
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            model.PageList = LogList.ToList().ToPagedList(pageNumber, pageSize);
            return View(model);

        }
        public ActionResult DeleteAll()
        {
            MSSEntities db = new MSSEntities();
            db.Database.ExecuteSqlCommand("Delete from Student_Specification_Log");
            return RedirectToAction("Index");
        }     
    }
}
