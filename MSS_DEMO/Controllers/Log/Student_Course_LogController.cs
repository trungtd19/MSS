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
    
    public class Student_Course_LogController : Controller
    {
        private IUnitOfWork unitOfWork;
        public Student_Course_LogController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
        [CheckCredential(Role_ID = "3")]
        public ActionResult Index(CoursesReportViewModel model, int? page, string searchCheck)
        {
            List<Student_Course_Log> LogList = new List<Student_Course_Log>();
            string SearchString = model.Email;
            model.searchCheck = searchCheck;
            List<string> listSubjiect = unitOfWork.Subject.GetAll().Select(o => o.Subject_Name).ToList();
            List<string> campus = unitOfWork.Campus.GetAll().Select(o => o.Campus_Name).ToList();
            model.lstCampus = campus;
            model.listSubject = listSubjiect;
            if (searchCheck == null)
            {
                page = 1;
            }
            else
            {
                LogList = unitOfWork.CoursesLog.GetPageList();
            }
            if (!String.IsNullOrEmpty(searchCheck))
            {
                if (!String.IsNullOrWhiteSpace(SearchString))
                {
                    LogList = LogList.Where(s => s.Email.ToUpper().Contains(SearchString.ToUpper())).ToList();
                }
                if (!String.IsNullOrWhiteSpace(model.Date_Import.ToString()))
                {
                    LogList = LogList.Where(s => s.Date_Import == model.Date_Import).ToList();
                }
                if (model.completedCourse != null)
                {
                    LogList = model.completedCourse =="Yes" ? LogList.Where(s => s.Completed == true).ToList() : LogList.Where(s => s.Completed == false).ToList();
                }
                if (model.compulsoryCourse != null)
                {
                    LogList = model.compulsoryCourse == "Yes" ? LogList = LogList.Where(s => s.Course_ID != null).ToList() : LogList = LogList.Where(s => s.Course_ID == null).ToList();
                }
                if (!String.IsNullOrWhiteSpace(model.Subject_Name))
                {
                    var sub = unitOfWork.Subject.GetAll().Where(x => x.Subject_Name == model.Subject_Name).Select(y => y.Subject_ID).FirstOrDefault();
                    LogList = LogList.Where(s => s.Subject_ID == sub).ToList();
                }
                if (!String.IsNullOrWhiteSpace(model.Campus))
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
            List<string> completedCour = new List<string>() { "Yes","No"};
            List<string> compulsoryCour = new List<string>() { "Yes","No" };
            model.completedCour = completedCour;
            model.compulsoryCour = compulsoryCour;
            ViewBag.Count = LogList.Count();
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            model.PageList = LogList.ToList().ToPagedList(pageNumber, pageSize);
            return View(model);
        }
        public void Export_Usage()
        {
            CSVConvert csv = new CSVConvert();
            var sb = new StringBuilder();
            IEnumerable<Student_Course_Log> query = unitOfWork.CoursesLog.GetAll();
            var list = query.ToList();
            Type type = typeof(Student);
            var props = type.GetProperties();
            sb.Append(string.Join(",", "Email", "Subject ID", "Campus","Enrollment Time", "Start Time", "Last ActivityTime", "Overall Progress", "Estimated",
                "Completed", "Status", "Program Slug", "Program Name", "Completion Time", "Course Grade", "Date Import"));
            sb.Append(Environment.NewLine);
            foreach (var item in list)
            {
                sb.Append(string.Join(",",
                    csv.AddCSVQuotes(item.Email),
                    csv.AddCSVQuotes(item.Subject_ID),
                    csv.AddCSVQuotes(item.Campus),
                    csv.AddCSVQuotes(item.Course_Enrollment_Time.ToString().Contains("1/1/1970") ? "" : item.Course_Enrollment_Time.ToString()),
                    csv.AddCSVQuotes(item.Course_Start_Time.ToString().Contains("1/1/1970") ? "" : item.Course_Start_Time.ToString()),
                    csv.AddCSVQuotes(item.Last_Course_Activity_Time.ToString().Contains("1/1/1970") ? "" : item.Last_Course_Activity_Time.ToString()),
                    csv.AddCSVQuotes(item.Overall_Progress.ToString()),
                    csv.AddCSVQuotes(item.Estimated.ToString()),
                    csv.AddCSVQuotes(item.Completed.ToString()),
                    csv.AddCSVQuotes(item.Status.ToString()),
                    csv.AddCSVQuotes(item.Program_Slug),
                    csv.AddCSVQuotes(item.Program_Name),
                    csv.AddCSVQuotes(item.Completion_Time.ToString().Contains("1/1/1970") ? "" : item.Completion_Time.ToString()),
                    csv.AddCSVQuotes(item.Course_Grade.ToString()),
                    csv.AddCSVQuotes(item.Date_Import.ToString())
                    ));
                sb.Append(Environment.NewLine);
            }
            var response = System.Web.HttpContext.Current.Response;
            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.ContentEncoding = Encoding.Unicode;
            response.AddHeader("content-disposition", "attachment;filename=Usage-Report.CSV ");
            response.ContentType = "text/plain";
            response.Write(sb.ToString());
            response.End();
        }
        public ActionResult Mentor()
        {
            var userMentor = (UserLogin)HttpContext.Session[CommonConstants.User_Session];
            var list = unitOfWork.Mentor.getListCourses(userMentor.UserName);
            return View(list);
        }
        public ActionResult Detail(string id)
        {
            ViewBag.Course_Name = unitOfWork.Courses.GetById(int.Parse(id.Split('^')[0])).Course_Name;
            ViewBag.Class = id.Split('^')[1];
            var list = unitOfWork.Mentor.getReport(id);
            return View(list);
        }
    }
}
