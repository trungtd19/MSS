using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace MSSWebService
{
    /// <summary>
    /// Summary description for MSSWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MSSWS : System.Web.Services.WebService
    {

        [WebMethod]
        public string getScheduledSubject(string userLogin, string[] listSubject)
        {
            // List class mô phỏng
            List<SubjectClass> listA = new List<SubjectClass>();
            listA.Add(new SubjectClass
            {
                Subject_ID = "SSL101",
                Class_ID = "12"
            });
            listA.Add(new SubjectClass
            {
                Subject_ID = "SSL101",
                Class_ID = "11"
            });
            // Object Mentor mô phỏng
            Menotr[] objstudents = new Menotr[]
                {
                    new Menotr()
                    {
                        Email = "trungtdse05083@fpt.edu.vn",
                        ListSubjectClass =  listA
                    },
                 };
            StringBuilder sb = new StringBuilder();
            foreach (var x in objstudents)
            {
                if (x.Email == userLogin)
                {
                
                    foreach (var y in x.ListSubjectClass)
                    {
                        if(IsExistCourses(y.Subject_ID, listSubject))
                        {
                            sb.Append(y.Subject_ID + ";" + y.Class_ID + ",");            
                        }                     
                    }
                }
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(sb.ToString());
        }
        public bool IsExistCourses(string couresName, string[] listCourse)
        {
            foreach (var cou in listCourse)
            {
                if (couresName.Contains(cou.Trim()))
                {
                    return true;
                }
            }
            return false;
        }
        [WebMethod]
        public string getStudents(string userLogin, string classID, string subjectID)
        {
            // List Roll Student mô phỏng
            List<string> listStudent = new List<string>();
            listStudent.Add("HE150005");
            listStudent.Add("HE150820");
            // Object Mentor mô phỏng
            Menotr[] objstudents = new Menotr[]
                {
                    new Menotr()
                    {
                        Email = "trungtdse05083@fpt.edu.vn",
                        Class_ID ="12",
                        Subject_ID ="SSL101",
                        ListRoll = listStudent

                    },
                     new Menotr()
                    {
                        Email = "trungtdse05083@fpt.edu.vn",
                        Class_ID ="11",
                        Subject_ID ="SSL101",
                        ListRoll = listStudent
                    },
                 };
            StringBuilder sb = new StringBuilder();
            foreach (var x in objstudents)
            {
                if (x.Email == userLogin && x.Class_ID == classID && x.Subject_ID == subjectID)
                {
                    foreach (var y in x.ListRoll)
                    {
                        sb.Append(y + ",");
                    }
                }
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(sb.ToString());
        }
        public class Menotr
        {
            public string Email
            {
                get;
                set;
            }
            public string Class_ID
            {
                get;
                set;
            }
            public string Subject_ID
            {
                get;
                set;
            }
            public List<string> ListRoll;

            public List<SubjectClass> ListSubjectClass;

        }
        public class SubjectClass
        {
            public string Subject_ID
            {
                get;
                set;
            }
            public string Class_ID
            {
                get;
                set;
            }

        }
    }
}

