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
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            if (postedFile != null)
            {
                try
                {
                    string fileExtension = Path.GetExtension(postedFile.FileName);

                    if (fileExtension != ".csv")
                    {
                        ViewBag.Message = "Please select the csv file with .csv extension";
                        return View();
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
                                    
                                            var classes = context.Classes.Find(GetClassStudent(rows).Class_ID);
                                            var subject = context.Subjects.Find(GetSubjectStudent(rows).Subject_ID);

                                        if (context.Students.Find(GetStudent(rows).Roll) != null 
                                            && context.Subjects.Find(GetSubjectStudent(rows).Subject_ID) == null)
                                        {
                                            context.Subject_Student.Add(GetSubjectStudent(rows));
                                            context.Class_Student.Add(GetClassStudent(rows));
                                        }
                                        else
                                        {
                                            context.Students.Add(GetStudent(rows));
                                            context.Subject_Student.Add(GetSubjectStudent(rows));
                                            context.Class_Student.Add(GetClassStudent(rows));
                                        }
                                            //classes.Students.Add(GetStudent(rows));    
                                            //subject.Students.Add(GetStudent(rows));                                  
                                                                          
                                    }
                                    context.SaveChanges();                                   
                                    transaction.Commit();
                                    ViewBag.Message = "Import successfull!";
                                }
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                ViewBag.Message = ex.Message;                    
                            }

                        }
                    }
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "Please select the file first to upload.";
            }
            return View();
        }
        private Student GetStudent(String[] row)
        {
            return new Student
            {
                Email = row[0].ToString(),
                Roll = row[1].ToString(),
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
                Roll = row[1].ToString(),
                Subject_ID = row[3].ToString(),
            };

        }
    }
}
