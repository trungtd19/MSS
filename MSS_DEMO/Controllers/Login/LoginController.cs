using MSS_DEMO.Common;
using MSS_DEMO.Core.Implement;
using MSS_DEMO.Models;
using MSS_DEMO.MssService;
using System;
using System.Collections.Generic;
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
            User_Role user = null;
            string userLogin = Request["Mail"];
            string[] temp = userLogin.Split('@');
            string checkmail = temp[1];                    
                user = db.User_Role.SingleOrDefault(x => x.Login.Equals(userLogin));
            if (user != null)
            {
                var RoleSession = new RoleLogin();
                RoleSession.Role = user.Role_ID;
                var UserSession = new UserLogin();
                UserSession.UserID = user.User_ID;
                UserSession.UserName = userLogin;
                Session.Add(CommonConstants.ROLE_Session, RoleSession);
                Session.Add(CommonConstants.User_Session, UserSession);
                return Json(new { message = "true" }, JsonRequestBehavior.AllowGet);
            }          
            if (user == null)
            {
                if (!checkmail.Equals("fpt.edu.vn"))
                {
                    return Json(new { message = "false" }, JsonRequestBehavior.AllowGet);
                }
                else if(checkMentor(userLogin) == true)
                {
                    var RoleSession = new RoleLogin();
                    RoleSession.Role = 3;
                    var UserSession = new UserLogin();
                    UserSession.UserID = 3;
                    UserSession.UserName = userLogin;
                    Session.Add(CommonConstants.ROLE_Session, RoleSession);
                    Session.Add(CommonConstants.User_Session, UserSession);
                    return Json(new { message = "true" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var RoleSession = new RoleLogin();
                    RoleSession.Role = 5;
                    var UserSession = new UserLogin();
                    UserSession.UserID = 5;
                    UserSession.UserName = userLogin;
                    Session.Add(CommonConstants.ROLE_Session, RoleSession);
                    Session.Add(CommonConstants.User_Session, UserSession);
                    return Json(new { message = "true" }, JsonRequestBehavior.AllowGet);
                }
                
            }
            else
            {
                return Json(new { message = "false" }, JsonRequestBehavior.AllowGet);
            }



        }

        private bool checkMentor( string userMentor)
        {           
            MSSEntities context = new MSSEntities();
            Semester semester = new Semester();
            DateTime date = DateTime.Now;
            CourseraApiSoapClient courseraApiSoap = new CourseraApiSoapClient();
            semester = context.Semesters.Where(sem => sem.Start_Date < date && sem.End_Date > date).FirstOrDefault();
            var listSubjectID = context.Subjects.Where(o => o.Subject_Active == true).Select(o => o.Subject_ID).ToArray();
            string authenKey = "A90C9954-1EDD-4330-B9F3-3D8201772EEA";
            List<MentorObject> objectMentor = new List<MentorObject>();         
            try
            {
                string jsonData = courseraApiSoap.GetScheduledSubject(authenKey, userMentor.Split('@')[0], listSubjectID, semester.Semester_Name.Replace(" ",""));
                var scheduledSubject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MertorFAP>>(jsonData);
                scheduledSubject = scheduledSubject.Distinct(new ListComparer()).ToList();

                foreach (var scheduled in scheduledSubject)
                {
                    objectMentor.Add(new MentorObject
                    {
                        Subject_ID = scheduled.SubjectCode,
                        Class_ID = scheduled.ClassName
                    });
                }
            }
            catch
            {
                objectMentor = new List<MentorObject>();
            }
            if(objectMentor.Count >0)
            {
                return true;
            }
            return false; 
        }

        public ActionResult Logout()
        {           
            Session[CommonConstants.ROLE_Session] = null;
            Session[CommonConstants.User_Session] = null;
            return RedirectToAction("Login","Login");

        }



    }
}