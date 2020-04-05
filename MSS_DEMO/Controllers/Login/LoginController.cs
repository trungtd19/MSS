using MSS_DEMO.Common;
using MSS_DEMO.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MSS_DEMO.Controllers.Login
{
    public class LoginController : Controller
    {
        private MSSEntities db = new MSSEntities();



        public ActionResult Index()
        {
            return View();
        }   

        public ActionResult Login()
        {

            return View();
        }
        //[HttpPost]
        //public ActionResult Login(string UserName, string PassWord)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        UserName = UserName + "@fpt.edu.vn";
        //        var user = db.User_Role.SingleOrDefault(x => x.Login.Equals(UserName));
        //        if (user == null)
        //        {
        //            ModelState.AddModelError("", " Accont is not exist");
        //        }
        //        else if (user.isActive == false)
        //        {
        //            ModelState.AddModelError("", " Accont is locked ");
        //        }
        //        else
        //        {
        //            var role = user.Role_ID;
        //            var UserSession = new UserLogin();
        //            UserSession.UserID = user.User_ID;
        //            UserSession.UserName = user.Login;
        //            Session.Add(CommonConstants.ROLE_Session, role);
        //            Session.Add(CommonConstants.User_Session, UserSession);
        //            return RedirectToAction("Index", "Home");
        //        }
        //    }

        //    else
        //    {
        //        ModelState.AddModelError("", "Login Fail");
        //    }
        //    return View("Login");

        //}
        [HttpPost]
        public ActionResult LoginWithGoogle()
        {
                    
            string xy = Request["Mail"];
            User_Role user = null;
           
                user = db.User_Role.SingleOrDefault(x => x.Login.Equals(xy));

            if (user == null)
            {
                var RoleSession = new RoleLogin();
                RoleSession.Role = 0;
                var UserSession = new UserLogin();
                UserSession.UserID = 0;
                UserSession.UserName = xy;
                Session.Add(CommonConstants.ROLE_Session, RoleSession);
                Session.Add(CommonConstants.User_Session, UserSession);
                return Json(new { message = "true" }, JsonRequestBehavior.AllowGet);
            }
            else if (user.isActive == false)
            {
                ModelState.AddModelError("", " Account is locked ");
            }
            if (user != null)
            {
                var RoleSession = new RoleLogin();
                RoleSession.Role = user.Role_ID;
                var UserSession = new UserLogin();
                UserSession.UserID = user.User_ID;
                UserSession.UserName = xy;
                Session.Add(CommonConstants.ROLE_Session, RoleSession);
                Session.Add(CommonConstants.User_Session, UserSession);
                return Json(new { message = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "false" }, JsonRequestBehavior.AllowGet);
            }
                
            
        }
        public ActionResult Logout()
        {           
            Session[CommonConstants.ROLE_Session] = null;
            Session[CommonConstants.User_Session] = null;
            return RedirectToAction("Login","Login");

        }



    }
}