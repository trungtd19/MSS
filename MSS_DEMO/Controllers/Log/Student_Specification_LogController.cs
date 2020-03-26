using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MSS_DEMO.Common;
using MSS_DEMO.Core.Import;
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
            List<string> campus = unitOfWork.Campus.GetAll().Select(o => o.Campus_Name).ToList();
            model.listSubject = listSubjiect;
            model.lstCampus = campus;
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
                if (!String.IsNullOrEmpty(model.Campus))
                {
                    var cp = unitOfWork.Campus.GetAll().Where(cmp => cmp.Campus_Name == model.Campus).Select(cmp => cmp.Campus_ID).FirstOrDefault();
                    LogList = LogList.Where(s => s.Campus.ToUpper().Contains(cp)).ToList();
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
        public void Export_Specification()
        {
            CSVConvert csv = new CSVConvert();
            var sb = new StringBuilder();
            IEnumerable<Student_Specification_Log> query = unitOfWork.SpecificationsLog.GetAll();
            var list = query.ToList();
            Type type = typeof(Student);
            var props = type.GetProperties();
            sb.Append(string.Join(",", "Email", "Subject ID", "Campus", "Specialization", "Specialization Slug", "University", "Enrollment Time", "Last Activity Time",
                "Completed", "Status", "Program Slug", "Program Name", "Completion Time",  "Date Import"));
            sb.Append(Environment.NewLine);
            foreach (var item in list)
            {
                sb.Append(string.Join(",",
                    csv.AddCSVQuotes(item.Email),
                    csv.AddCSVQuotes(item.Subject_ID),
                    csv.AddCSVQuotes(item.Campus),
                    csv.AddCSVQuotes(item.Specialization),
                    csv.AddCSVQuotes(item.Specialization_Slug),
                    csv.AddCSVQuotes(item.University),
                    csv.AddCSVQuotes(item.Specialization_Enrollment_Time.ToString().Contains("1/1/1970") ? "" : item.Specialization_Enrollment_Time.ToString()),
                    csv.AddCSVQuotes(item.Last_Specialization_Activity_Time.ToString().Contains("1/1/1970") ? "" : item.Last_Specialization_Activity_Time.ToString()),
                    csv.AddCSVQuotes(item.Completed.ToString()),
                    csv.AddCSVQuotes(item.Status.ToString()),
                    csv.AddCSVQuotes(item.Program_Slug),
                    csv.AddCSVQuotes(item.Program_Name),
                    csv.AddCSVQuotes(item.Specialization_Completion_Time.ToString().Contains("1/1/1970") ? "" : item.Specialization_Completion_Time.ToString()),
                    csv.AddCSVQuotes(item.Date_Import.ToString())
                    ));
                sb.Append(Environment.NewLine);
            }
            var response = System.Web.HttpContext.Current.Response;
            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.ContentEncoding = Encoding.Unicode;
            response.AddHeader("content-disposition", "attachment;filename=Specification-Report.CSV ");
            response.ContentType = "text/plain";
            response.Write(sb.ToString());
            response.End();
        }
    }
}
