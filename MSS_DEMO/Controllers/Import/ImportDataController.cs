using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Models;
using System.Data.OleDb;
using System.Data.Entity.Validation;
using System.IO;
using System;
using System.Collections;

namespace MSS_DEMO.Controllers
{
    public class ExportDataController : Controller
    {
        private MSSEntities db = new MSSEntities();

        [Route("Import-data")]
        public ActionResult Index()
        {
            return View("~/Views/ExportData/Index.cshtml");
        }
        [Route("Import-data")]
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile1)//,HttpPostedFileBase postedFile2)//, HttpPostedFileBase postedFile3)
        {
            ArrayList postedList = new ArrayList();
            postedList.Add(postedFile1);
           // postedList.Add(postedFile2);
            //  postedList.Add(postedFile3);
            foreach (HttpPostedFileBase postedFile in postedList)
            {
                if (postedFile != null)
                {
                    try
                    {
                        string fileExtension = Path.GetExtension(postedFile.FileName);

                        if (fileExtension != ".xls" && fileExtension != ".xlsx" && fileExtension != ".csv")
                        {
                            ViewBag.Message = "Please select the excel file with .xls or .xlsx or .csv extension";
                            return View();
                        }
                        else
                        if (fileExtension == ".csv")
                        {
                            using (var context = new MSSEntities())

                            {
                                using (var sreader = new StreamReader(postedFile.InputStream))
                                {
                                    if (postedFile.FileName.Contains("specialization-report_sample"))
                                    {
                                    string[] headers = sreader.ReadLine().Split(',');
                                    while (!sreader.EndOfStream)
                                    {
                                        string[] rows = sreader.ReadLine().Split(',');                                
                                        context.Student_Specification_Log.Add(GetStudentSpec(rows));                                   
                                    }
                                    }
                                    //else
                                    //    if (postedFile.FileName.Contains("usage-report_sample"))
                                    //{
                                    //    string[] headers = sreader.ReadLine().Split(',');
                                    //    while (!sreader.EndOfStream)
                                    //    {
                                    //        string[] rows = sreader.ReadLine().Split(',');
                                    //        context.Student_Course_Log.Add(GetStudentCourse(rows));
                                    //    }
                                       
                                    //}
                                }
                                context.SaveChanges();
                            }
                        }                      
                        ViewBag.Message = "Data Imported Successfully.";
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
            }
            return View("~/Views/ExportData/Index.cshtml");
        }
        private String ChangeBoolean(string name)
        {
            if (name.ToLower() == "yes") return "True";
            else return "False";         
        }

        private Student_Specification_Log GetStudentSpec(String[] row)
        {
            return new Student_Specification_Log
            {               
                Roll = row[2].ToString().Split('-')[2],
                Subject_ID = row[3].ToString(),
                Campus = row[4].ToString(),
                Specialization = row[5].ToString(),
                Specialization_Slug = row[6].ToString(),
                University = row[7].ToString(),
              //  Specialization_Enrollment_Time = DateTime.Parse(row[8].ToString()),
               // Last_Specialization_Activity_Time = DateTime.Parse(row[9].ToString()),
                Completed = bool.Parse(ChangeBoolean(row[10].ToString())),
                Status = bool.Parse(ChangeBoolean(row[11].ToString())),
                Program_Slug = row[12].ToString(),
                Program_Name = row[13].ToString(),
               // Specialization_Completion_Time = DateTime.Parse(row[15].ToString()),

            };
        }
        private Student_Course_Log GetStudentCourse(String[] row)
        {
            return new Student_Course_Log
            {
                Roll = row[2].ToString().Split('-')[2],
             //   Course_Enrollment_Time = DateTime.Parse(row[7].ToString()),
               // Course_Start_Time = DateTime.Parse(row[8].ToString()),
                //Last_Course_Activity_Time = DateTime.Parse(row[9].ToString()),
                Overall_Progress = Double.Parse(row[10].ToString()),
                Estimated = Double.Parse(row[11].ToString()),
                Completed = Boolean.Parse(ChangeBoolean(row[12].ToString())),
                Status = Boolean.Parse(ChangeBoolean(row[13].ToString())),
                Program_Slug = row[14].ToString(),
                Program_Name = row[15].ToString(),
                //Completion_Time = DateTime.Parse(row[17].ToString()),
                Course_Grade = Double.Parse(row[18].ToString()),

            };
        }
        private Course GetCourse(String[] row)
        {
            return new Course
            {
               Course_ID = row[1].ToString(),
               Course_Name = row[1].ToString(),
               Course_Slug = row[1].ToString(),
               Specification_ID = row[1].ToString(),
               //Subject_ID = row[1].ToString(),
            };
        }
        private Specification GetSpec(String[] row)
        {
            return new Specification
            {
                Specification_ID = row[1].ToString(),
                Subject_ID = row[1].ToString(),
             
            };
        }
        private Subject GetSubject(String[] row)
        {
            return new Subject
            {
                
                Subject_ID = row[1].ToString(),
                Subject_Name = row[1].ToString(),

            };
        }
      
    }
}
