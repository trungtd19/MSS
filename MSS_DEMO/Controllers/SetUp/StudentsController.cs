using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MSS_DEMO.Core.Import;
using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using PagedList;

namespace MSS_DEMO.Controllers.SetUp
{
    public class StudentsController : Controller
    {
        private IUnitOfWork unitOfWork;
        public StudentsController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;           
        }

        public ActionResult Index(int? page, string SearchString, string searchCheck, string currentFilter)
        {
            List<Student> students = new List<Student>();
            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }
            ViewBag.CurrentFilter = SearchString;
            if (!String.IsNullOrEmpty(searchCheck))
            {
                students = unitOfWork.Students.GetPageList();
                if (!String.IsNullOrEmpty(SearchString))
                {
                    students = students.Where(s => s.Roll.ToUpper().Contains(SearchString.ToUpper())).ToList();
                }
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(students.ToList().ToPagedList(pageNumber, pageSize));

        }
        public void Export_Student_CSV()
        {
            CSVConvert csv = new CSVConvert();
            var sb = new StringBuilder();
            IEnumerable<Student> query = unitOfWork.Students.GetAll();
            var list = query.ToList();
            Type type = typeof(Student);
            var props = type.GetProperties();
            sb.Append(string.Join(",", "ROLL", "Email"));
            sb.Append(Environment.NewLine);
            foreach (var item in list)
            {
                sb.Append(string.Join(",", csv.AddCSVQuotes(item.Roll), csv.AddCSVQuotes(item.Email)));
                sb.Append(Environment.NewLine);            
            }
            var response = System.Web.HttpContext.Current.Response;
            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.ContentEncoding = Encoding.Unicode;
            response.AddHeader("content-disposition", "attachment;filename=Student.CSV ");
            response.ContentType = "text/plain";
            response.Write(sb.ToString());
            response.End();
        }
    }
}
