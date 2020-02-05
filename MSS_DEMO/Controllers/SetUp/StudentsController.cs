using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
        public ActionResult Index(int? page)
        {
            List<Student> students = unitOfWork.Students.GetPageList();      
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(students.ToList().ToPagedList(pageNumber, pageSize));

        }      
    }
}
