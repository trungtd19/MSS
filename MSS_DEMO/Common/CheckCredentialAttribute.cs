
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSS_DEMO.Common
{
    public class CheckCredentialAttribute :AuthorizeAttribute
    {
        public string Role_ID { set; get; }
        int checkSession = 0;
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            List<string> admin = new List<string> { "1", "2", "3", "4", "5" };
            List<string> Acad = new List<string> { "3","2" };
            List<string> Mentor = new List<string> {"3" };         
            List<string> Student = new List<string> { "5" };
            UserLogin session = (UserLogin)HttpContext.Current.Session[CommonConstants.User_Session];
            RoleLogin role = (RoleLogin)HttpContext.Current.Session[CommonConstants.ROLE_Session];
            string url = HttpContext.Current.Request.Url.AbsoluteUri;
            if(url.Contains("Report?rpN"))
            {
                return true;
            }
            if (session == null)
            {
                checkSession = 1;
                return false;
            }                     
            if(role.Role ==1)
            {
                if (admin.Contains(this.Role_ID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (role.Role == 2)
            {
                if (Acad.Contains(this.Role_ID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (role.Role == 3)
            {
                if (Mentor.Contains(this.Role_ID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }       
           
            else if (role.Role == 5)
            {
                if (Student.Contains(this.Role_ID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }


        }
       
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if(checkSession==1)
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Login/Login.cshtml"
                };
                
            }
             else
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/401.cshtml"
                };
            }
            
        }

    }
}