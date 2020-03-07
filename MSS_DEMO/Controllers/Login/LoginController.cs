using MSS_DEMO.Common;
using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MSS_DEMO.Controllers.Login
{
    public class LoginController : Controller
    {
        private MSSEntities db = new MSSEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }            
       
        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login( string UserName,string PassWord)
        {
          
            if (ModelState.IsValid)
            {
                UserName = UserName + "@fpt.edu.vn";
                var user = db.User_Role.SingleOrDefault(x => x.Login == UserName);
                if (user == null)
                {
                    ModelState.AddModelError("", " Accont is not exist");
                }
                else if (user.isActive == false)
                {
                    ModelState.AddModelError("", " Accont is locked ");
                }
                else
                {
                   
                    var UserSession =new UserLogin();
                    UserSession.UserID = user.User_ID;
                    UserSession.UserName = user.Login;
                    Session.Add(CommonConstants.User_Session, UserSession); 
                    return RedirectToAction("Index","Home");
                }
                
               
               

            }
           
            else
            {
                ModelState.AddModelError("", "Login Fail");
            }

            return View("Login");
            
            
        }
        public ActionResult Logout()
        {
            Session[CommonConstants.User_Session] = null;
            return RedirectToAction("Login","Login");

        }



    }
}