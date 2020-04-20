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
using MSS_DEMO.ServiceReference;
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
        [CheckCredential(Role_ID = "4")]
        public ActionResult Index(CoursesReportViewModel model, int? page, string searchCheck)
        {
            List<Student_Course_Log> LogList = new List<Student_Course_Log>();
            string SearchString = model.Email;
            model.searchCheck = searchCheck;
            List<SelectListItem> listSubjiect = new List<SelectListItem>();
            var subject = unitOfWork.Subject.GetAll();
            foreach (var sub in subject)
            {
                listSubjiect.Add(new SelectListItem
                {
                    Text = sub.Subject_Name,
                    Value = sub.Subject_ID
                });
            }
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
            var listDate = unitOfWork.CoursesLog.GetAll().OrderByDescending(o => o.Date_Import).Select(o => o.Date_Import).Distinct();
            List<string> date = new List<string>();
            foreach (var _date in listDate)
            {
                date.Add(Convert.ToDateTime(_date).ToString("dd/MM/yyyy"));
            }
            date  = date.Distinct().ToList();
            model.importedDate = date;
            model.lstSemester = semesterList;
            model.lstCampus = campusList;
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
                if (!String.IsNullOrWhiteSpace(model.ImportedDate))
                {
                    DateTime dt = DateTime.ParseExact(model.ImportedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    LogList = LogList.Where(s => s.Date_Import == dt).ToList();
                }
                if (model.completedCourse != null)
                {
                    LogList = model.completedCourse =="Yes" ? LogList.Where(s => s.Completed == true).ToList() : LogList.Where(s => s.Completed == false).ToList();
                }
                if (model.compulsoryCourse != null)
                {
                    LogList = model.compulsoryCourse == "Yes" ? LogList = LogList.Where(s => s.Course_ID != null).ToList() : LogList = LogList.Where(s => s.Course_ID == null).ToList();
                }
                if (!String.IsNullOrWhiteSpace(model.Subject_ID))
                {                  
                    LogList = LogList.Where(s => s.Subject_ID == model.Subject_ID).ToList();
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
            List<string> completedCour = new List<string>() { "Yes","No"};
            List<string> compulsoryCour = new List<string>() { "Yes","No" };           
            model.completedCour = completedCour;
            model.compulsoryCour = compulsoryCour;
            ViewBag.CountRoll = LogList.Select(o => o.Roll).Distinct().Count();
            ViewBag.CountLog = LogList.Count();
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            model.PageList = LogList.ToList().ToPagedList(pageNumber, pageSize);
            return View(model);
        }
        public ActionResult Mentor()
        {
            var userMentor = (UserLogin)HttpContext.Session[CommonConstants.User_Session];           
            return View(unitOfWork.CoursesLog.getListSubjectClass(userMentor.UserName));
        }
        public ActionResult Detail(string id)
        {

            var userMentor = (UserLogin)HttpContext.Session[CommonConstants.User_Session];
            var listSubjectClass = unitOfWork.CoursesLog.getListSubjectClass(userMentor.UserName);
            MSSWSSoapClient soap = new MSSWSSoapClient();
            string jsonDataClass = "";
            List<string> listRoll = new List<string>();
            List<Student_Course_Log> list = new List<Student_Course_Log>();
            foreach (var subjectClass in listSubjectClass)
            {
                if (id == subjectClass.id)
                {
                    jsonDataClass = soap.GetClass(userMentor.UserName, subjectClass.Class_ID.Trim(), subjectClass.Subject_ID.Trim());
                    listRoll = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jsonDataClass);
                    foreach (var roll in listRoll)
                    {
                        var student = unitOfWork.CoursesLog.GetAll().Where(o => o.Roll.Contains(roll.Trim()) && o.Subject_ID.Contains(subjectClass.Subject_ID.Trim())).FirstOrDefault();
                        if (student != null)
                        {
                            list.Add(student);
                        }
                    }
                    var maxDate = list.OrderByDescending(o => o.Date_Import).FirstOrDefault().Date_Import;
                    list = list.Where(o => o.Date_Import == maxDate).ToList();
                    ViewBag.Subject = subjectClass.Subject_Name;
                    ViewBag.Class = subjectClass.Class_ID;
                    break;
                }
            }       
            return View(list);
        }
        [HttpGet]
        public void Export(string check)
        {

            string searchCheck = check.Split('^')[0];
            string Campus = check.Split('^')[1];
            string Semester_ID = check.Split('^')[2];
            string Subject_ID = check.Split('^')[3];
            string completedSpec = check.Split('^')[4];
            string compulsorySpec = check.Split('^')[5];
            string ImportedDate = check.Split('^')[6];
            string Email = check.Split('^')[7];
            var LogList = unitOfWork.CoursesLog.GetPageList();
            if (searchCheck != "1")
            {
                if (Email != "8")
                {
                    LogList = LogList.Where(s => s.Email.ToUpper().Contains(Email.ToUpper())).ToList();
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
                    LogList = compulsorySpec == "Yes" ? LogList = LogList.Where(s => s.Course_ID != null).ToList() : LogList = LogList.Where(s => s.Course_ID == null).ToList();
                }
                if (Subject_ID != "4")
                {
                    LogList = LogList.Where(s => s.Subject_ID == Subject_ID.Trim()).ToList();
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
            sb.Append(string.Join(",","Name", "Email", "Subject ID", "Campus", "Course Name", "Course Slug","University" ,"Enrollment Time", "Start Time", "Last ActivityTime", "Overall Progress", "Estimated",
                "Completed", "Status", "Program Slug", "Program Name", "Enrollment Source", "Completion Time", "Course Grade", "Date Import"));
            sb.Append(Environment.NewLine);
            foreach (var item in list)
            {
                sb.Append(string.Join(",",
                    csv.AddCSVQuotes(item.Name),
                    csv.AddCSVQuotes(item.Email),
                    csv.AddCSVQuotes(item.Subject_ID),
                    csv.AddCSVQuotes(item.Campus),
                    csv.AddCSVQuotes(item.Course_Name),
                    csv.AddCSVQuotes(item.Course_Slug),
                    csv.AddCSVQuotes(item.University),
                    csv.AddCSVQuotes(item.Course_Enrollment_Time.ToString().Contains("1/1/1970") ? "" : item.Course_Enrollment_Time.ToString()),
                    csv.AddCSVQuotes(item.Course_Start_Time.ToString().Contains("1/1/1970") ? "" : item.Course_Start_Time.ToString()),
                    csv.AddCSVQuotes(item.Last_Course_Activity_Time.ToString().Contains("1/1/1970") ? "" : item.Last_Course_Activity_Time.ToString()),
                    csv.AddCSVQuotes(item.Overall_Progress.ToString()),
                    csv.AddCSVQuotes(item.Estimated.ToString()),
                    csv.AddCSVQuotes(item.Completed.ToString()),
                    csv.AddCSVQuotes(item.Status.ToString()),
                    csv.AddCSVQuotes(item.Program_Slug),
                    csv.AddCSVQuotes(item.Program_Name),
                    csv.AddCSVQuotes(item.Enrollment_Source),
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
    }
}
