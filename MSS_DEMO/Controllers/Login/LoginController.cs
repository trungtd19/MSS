using MSS_DEMO.Common;
using MSS_DEMO.Core.Implement;
using MSS_DEMO.Models;
using MSS_DEMO.MssService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

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
        [HttpPost]
        public ActionResult LoginWithGoogle(string Email)
        {
            User_Role user = null;
            string[] temp = Email.Split('@');
            string checkmail = temp[1];
            string checkStudent;           
            checkStudent = temp[0].Substring(temp[0].Length - 5);
            using (var ctx = new MSSEntities())
            {
                 user = ctx.User_Role
                                .SqlQuery("Select * from User_Role where Login=@Login", new SqlParameter("@Login", Email))
                                .FirstOrDefault();
            }        
            if (user != null)
            {
                if(user.isActive == false)
                {
                    ViewBag.Error = "Login Fail!";
                    return View("Login");
                }
                else
                {
                    var RoleSession = new RoleLogin();
                    RoleSession.Role = user.Role_ID;
                    var UserSession = new UserLogin();
                    UserSession.UserID = user.User_ID;
                    UserSession.UserName = Email;
                    Session.Add(CommonConstants.ROLE_Session, RoleSession);
                    Session.Add(CommonConstants.User_Session, UserSession);
                    return RedirectToAction("Index", "Home");
                }
                
            }          
            if (user == null)
            {
                if (!checkmail.Equals("fpt.edu.vn"))
                {
                    ViewBag.Error = "Login Fail!";
                    return View("Login");
                }
                else if (IsNumber(checkStudent))
                {
                    var RoleSession = new RoleLogin();
                    RoleSession.Role = 5;
                    var UserSession = new UserLogin();
                    UserSession.UserID = 5;
                    UserSession.UserName = Email;
                    Session.Add(CommonConstants.ROLE_Session, RoleSession);
                    Session.Add(CommonConstants.User_Session, UserSession);
                    return RedirectToAction("Index", "Home");
                }
                else if(checkMentor(Email) == true)
                {
                    var RoleSession = new RoleLogin();
                    RoleSession.Role = 3;
                    var UserSession = new UserLogin();
                    UserSession.UserID = 3;
                    UserSession.UserName = Email;
                    Session.Add(CommonConstants.ROLE_Session, RoleSession);
                    Session.Add(CommonConstants.User_Session, UserSession);
                    return RedirectToAction("Index", "Home");
                }             
                else
                {
                    ViewBag.Error = "Login Fail!";
                    return View("Login");
                }
                
            }
            else
            {
                ViewBag.Error = "Login Fail!";
                return View("Login");
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
        public bool IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }
        public ActionResult Logout()
        {           
            Session[CommonConstants.ROLE_Session] = null;
            Session[CommonConstants.User_Session] = null;
            return RedirectToAction("Login","Login");

        }



    }
}