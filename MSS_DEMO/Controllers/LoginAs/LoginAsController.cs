using MSS_DEMO.Common;
using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSS_DEMO.Controllers.LoginAs
{
    [CheckCredential(Role_ID = "1")]
    public class LoginAsController : Controller
    {
        private MSSEntities db = new MSSEntities();
        // GET: LoginAs

       
        public ActionResult Index()
        {
           
            return View();
        }
        [HttpPost]
        public ActionResult Index(string SearchString)
        {
            Student student = new Student();

            student = db.Students.SingleOrDefault(x => x.Email.Contains(SearchString));
            if (student != null)
            {
                var RoleSession = new RoleLogin();
                RoleSession.Role = 5;
                var UserSession = new UserLogin();
                UserSession.UserID = 0;
                UserSession.UserName = student.Email;
                Session.Add(CommonConstants.ROLE_Session, RoleSession);
                Session.Add(CommonConstants.User_Session, UserSession);
            }
            else
            {
                ViewBag.Nodata = "Student not found";
                return View();
            }
            return RedirectToAction("Index","Home");
        }
    }
}