using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Models;
using LINQtoCSV;
using System.IO;
using System.Linq.Expressions;

namespace MSS_DEMO.Controllers
{
    public class ImportStudentController : Controller
    {
        private MSSEntities db = new MSSEntities();

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Import()//HttpPostedFileBase postedFile)
        {
            string messageImport = "";
            try
            {
                HttpPostedFileBase postedFile = Request.Files[0];          
            if (postedFile != null)
            {
                try
                {
                    string fileExtension = Path.GetExtension(postedFile.FileName);

                    if (fileExtension != ".csv")
                    {
                        messageImport = "Please select the csv file with .csv extension";
                        return Json(new { message = messageImport });
                    }


                    using (var context = new MSSEntities())
                    {
                        using (DbContextTransaction transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                using (var sreader = new StreamReader(postedFile.InputStream))
                                {
                                    string[] headers = sreader.ReadLine().Split(',');
                                    while (!sreader.EndOfStream)
                                    {

                                        string[] rows = sreader.ReadLine().Split(',');

                                        //var classes = context.Classes.Find(GetClassStudent(rows).Class_ID);
                                        // var subject = context.Subjects.Find(GetSubjectStudent(rows).Subject_ID);

                                        //if (context.Students.Find(GetStudent(rows).Roll) != null
                                        //    && context.Subjects.Find(GetSubjectStudent(rows).Subject_ID) == null)
                                        //{
                                        //    context.Subject_Student.Add(GetSubjectStudent(rows));
                                        //    //context.Class_Student.Add(GetClassStudent(rows));
                                        //}
                                        //else
                                        if (context.Students.Find(GetStudent(rows).Roll) == null)
                                        {
                                            context.Students.Add(GetStudent(rows));
                                            //   context.Subject_Student.Add(GetSubjectStudent(rows));
                                            //  context.Class_Student.Add(GetClassStudent(rows));
                                        }


                                    }
                                    context.SaveChanges();
                                    transaction.Commit();
                                    messageImport = "Import successfull!";
                                }
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                messageImport = ex.Message;
                            }

                        }
                    }
                    return Json(new { message = messageImport });
                }
                catch (Exception ex)
                {
                    messageImport = ex.Message;
                }
            }
            else
            {
                messageImport = "Please select the file first to upload.";
            }
        }
        catch{
                messageImport = "Please select the file first to upload.";
            }
            return Json(new { message = messageImport });
        }
        private Student GetStudent(String[] row)
        {
            return new Student
            {
                Email = row[1].ToString(),
                Roll = row[0].ToString(),
            };

        }
        private Class_Student GetClassStudent(String[] row)
        {
            return new Class_Student
            {
                Roll = row[1].ToString(),
                Class_ID = row[2].ToString(),
            };

        }
        private Subject_Student GetSubjectStudent(String[] row)
        {
            return new Subject_Student
            {
                Roll = row[0].ToString(),
                Subject_ID = row[2].ToString().Split('-')[0],
            };

        }
    }
}
