﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSS_DEMO.Common
{
    public class CheckCredentialAttribute :AuthorizeAttribute
    {
        public string Role_ID { set; get; }
        UserLogin session = (UserLogin)HttpContext.Current.Session[CommonConstants.User_Session];
        RoleLogin role = (RoleLogin)HttpContext.Current.Session[CommonConstants.ROLE_Session];
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            List<string> admin = new List<string> {"1"};
            List<string> Hana = new List<string> { "3","2" };
            List<string> Mentor = new List<string> {"2" };
            List<string> Acad = new List<string> { "1","2","3","4","5" };
            List<string> Student = new List<string> { "5" };
            
            string checkStudent;
           
            string[] temp = session.UserName.Split('@');
            checkStudent = temp[0].Substring(temp[0].Length - 5);
           
          
            if (session == null)
            {
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
            else if (role.Role == 4)
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
            else if (role.Role == 2)
            {
                if (Hana.Contains(this.Role_ID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (IsNumber(checkStudent))
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
        public bool IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
           
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/401.cshtml"
            };
        }

    }
}