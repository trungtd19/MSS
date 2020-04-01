using MSS_DEMO.Common;
using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ASPSnippets.GoogleAPI;
using System.Web.Script.Serialization;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
        //        return View("Login");

        //    }      
        [HttpPost]
        public ActionResult LoginWithGoogle()
        {
            string xy = Request["Mail"];          
            var user = db.User_Role.SingleOrDefault(x => x.Login.Equals(xy));
            if(user != null)
            {
                var role = user.Role_ID;
                var UserSession = new UserLogin();
                UserSession.UserID = user.User_ID;
                UserSession.UserName = xy;
                Session.Add(CommonConstants.ROLE_Session, role);
                Session.Add(CommonConstants.User_Session, UserSession);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
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