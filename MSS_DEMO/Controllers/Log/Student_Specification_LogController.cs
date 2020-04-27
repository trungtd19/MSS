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
    
    public class Student_Specification_LogController : Controller
    {
        private IUnitOfWork unitOfWork;
        public Student_Specification_LogController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
        [CheckCredential(Role_ID = "3")]
        public ActionResult Index(SpecReportViewModel model, int? page, string searchCheck)
        {
            List<Student_Specification_Log> LogList = new List<Student_Specification_Log>();
            string SearchString = model.Email;
            model.searchCheck = searchCheck;
            List<SelectListItem> campusList = new List<SelectListItem>();
            var campus = unitOfWork.Campus.GetAll();
            foreach (var cam in campus)
            {
                campusList.Add(new SelectListItem
                {
                    Text = cam.Campus_Name,
                    Value = cam.Campus_ID
                });
            }
            List<SelectListItem> semesterList = new List<SelectListItem>();
            var semester = unitOfWork.Semesters.GetAll();
            foreach (var sem in semester)
            {
                semesterList.Add(new SelectListItem
                {
                    Text = sem.Semester_Name,
                    Value = sem.Semester_ID
                });
            }
            var listDate = unitOfWork.SpecificationsLog.GetAll().OrderByDescending(o => o.Date_Import).Select(o => o.Date_Import).Distinct();
            List<string> date = new List<string>();
            foreach (var _date in listDate)
            {
                date.Add(Convert.ToDateTime(_date).ToString("dd/MM/yyyy"));
            }
            date = date.Distinct().ToList();
            model.importedDate = date;
            model.lstSemester = semesterList;
            model.lstCampus = campusList;
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
                if (!String.IsNullOrWhiteSpace(SearchString))
                {
                    LogList = LogList.Where(s => s.Email.Trim().ToUpper().Contains(SearchString.Trim().ToUpper())).ToList();
                }
                if (!String.IsNullOrWhiteSpace(model.ImportedDate))
                {
                    DateTime dt = DateTime.ParseExact(model.ImportedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    LogList = LogList.Where(s => s.Date_Import == dt).ToList();
                }
                if (model.completedSpec != null)
                {
                    LogList = model.completedSpec == "Yes" ? LogList.Where(s => s.Completed == true).ToList() : LogList.Where(s => s.Completed == false).ToList();
                }
                if (model.compulsorySpec != null)
                {
                    LogList = model.compulsorySpec == "Yes" ? LogList = LogList.Where(s => s.Specification_ID != null).ToList() : LogList = LogList.Where(s => s.Specification_ID == null).ToList();
                }
                if (!String.IsNullOrWhiteSpace(model.Campus))
                {                 
                    LogList = LogList.Where(s => s.Campus == model.Campus).ToList();
                }
                if (!String.IsNullOrWhiteSpace(model.Semester_ID))
                {
                    LogList = LogList.Where(s => s.Semester_ID == model.Semester_ID).ToList();
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
            ViewBag.CountRoll = LogList.Select(o => o.Roll).Distinct().Count();
            ViewBag.CountLog = LogList.Count();
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            model.PageList = LogList.ToList().ToPagedList(pageNumber, pageSize);
            return View(model);

        }
        [HttpGet]
        public void Export(string check)
        {

            string searchCheck = check.Split('^')[0];
            string Campus = check.Split('^')[1];
            string Semester_ID = check.Split('^')[2];
            string completedSpec = check.Split('^')[4];
            string compulsorySpec = check.Split('^')[5];
            string ImportedDate = check.Split('^')[6];
            string Email = check.Split('^')[7];
            var LogList = unitOfWork.SpecificationsLog.GetPageList();
            if (searchCheck != "1")
            {
                if (Email != "8")
                {
                    LogList = LogList.Where(s => s.Email.Trim().ToUpper().Contains(Email.Trim().ToUpper())).ToList();
                }
                if (ImportedDate != "7")
                {
                    DateTime dt = DateTime.ParseExact(ImportedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    LogList = LogList.Where(s => s.Date_Import == dt).ToList();
                }
                if (completedSpec != "5")
                {
                    LogList = completedSpec == "Yes" ? LogList.Where(s => s.Completed == true).ToList() : LogList.Where(s => s.Completed == false).ToList();
                }
                if (compulsorySpec != "6")
                {
                    LogList = compulsorySpec == "Yes" ? LogList = LogList.Where(s => s.Specification_ID != null).ToList() : LogList = LogList.Where(s => s.Specification_ID == null).ToList();
                }
                if (Campus != "2")
                {
                    LogList = LogList.Where(s => s.Campus == Campus.Trim()).ToList();
                }
                if (Semester_ID != "3")
                {
                    LogList = LogList.Where(s => s.Semester_ID == Semester_ID.Trim()).ToList();
                }
            }
            CSVConvert csv = new CSVConvert();
            var sb = new StringBuilder();
            var list = LogList.ToList();
            sb.Append(string.Join(",", "Name", "Email", "Campus", "Specialization", "Specialization Slug", "University", "Enrollment Time", "Last Activity Time",
                "Completed", "Status", "Program Slug", "Program Name", "Enrollment Sourse", "Completion Time", "Date Import"));
            sb.Append(Environment.NewLine);
            foreach (var item in list)
            {
                sb.Append(string.Join(",",
                    csv.AddCSVQuotes(item.Name),
                    csv.AddCSVQuotes(item.Email),
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
                    csv.AddCSVQuotes(item.Enrollment_Source),
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
