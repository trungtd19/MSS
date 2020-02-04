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
using System.Data.Entity;
using System.Data.SqlTypes;

namespace MSS_DEMO.Controllers
{
    public class ExportDataController : Controller
    {
        private MSSEntities db = new MSSEntities();
        [Route("Import-data")]
        public ActionResult Index()
        {
            return View("~/Views/ImportDataDaily/Index.cshtml");
        }
        [Route("Import-data")]
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile1, HttpPostedFileBase postedFile2)//, HttpPostedFileBase postedFile3)
        {
            ArrayList postedList = new ArrayList();
            postedList.Add(postedFile1);
            postedList.Add(postedFile2);
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
                                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                                {
                                    int i = 0;
                                    try
                                    {
                                        using (var sreader = new StreamReader(postedFile.InputStream))
                                        {
                                            if (postedFile.FileName.Contains("specialization-report"))
                                            {
                                                string[] headers = sreader.ReadLine().Split(',');

                                                while (!sreader.EndOfStream)
                                                {
                                                    i++;
                                                    string stringFormat = sreader.ReadLine();
                                                    string[] rows = stringFormat.Split(',');
                                                    Student_Specification_Log st = GetStudentSpec(rows);
                                                    context.Student_Specification_Log.Add(st);
                                                }
                                            }
                                            else
                                            if (postedFile.FileName.Contains("usage-report"))
                                            {
                                                string[] headers = sreader.ReadLine().Split(',');
                                                while (!sreader.EndOfStream)
                                                {
                                                    string stringFormat = sreader.ReadLine();
                                                    // stringFormat = stringFormat.Replace(",,", ",-,");
                                                    string[] rows = stringFormat.Split(',');
                                                    context.Student_Course_Log.Add(GetStudentCourse(rows));
                                                }

                                            }
                                        }
                                        context.SaveChanges();
                                        transaction.Commit();
                                        ViewBag.Message = "Data Imported Successfully.";
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        ViewBag.Message = ex.Message + i;
                                    }
                                }
                            }
                        }

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
            return View("~/Views/ImportDataDaily/Index.cshtml");
        }
        //private String Between(string STR)
        //{
        //    if (STR.IndexOf(",\"") > 0)
        //    {
        //        string FinalString;
        //        int Pos1 = STR.IndexOf(",\"") + ",\"".Length;
        //        int Pos2 = STR.IndexOf("\",");
        //        FinalString = STR.Substring(Pos1, Pos2 - Pos1);
        //        return FinalString;
        //    }
        //    return STR;
        //}

        private String ChangeBoolean(string name)
        {
            if (name.ToLower() == "yes") return "True";
            else return "False";
        }

        private Student_Specification_Log GetStudentSpec(String[] row)
        {
            return new Student_Specification_Log
            {
                //Roll = row[2].ToString().Split('-')[2],
                Roll = row[0].ToString(),
                Subject_ID = row[2].ToString().Split('-')[0],
                Campus = row[2].ToString().Split('-')[1],
                Specialization = row[3].ToString(),
                Specialization_Slug = row[4].ToString(),
                University = row[5].ToString(),
                Specialization_Enrollment_Time = row[6].ToString() != "" ? DateTime.Parse(row[6].ToString()) : DateTime.Parse("01/01/1970"),
                Last_Specialization_Activity_Time = row[7].ToString() != "" ? DateTime.Parse(row[7].ToString()) : DateTime.Parse("01/01/1970"),
                Completed = bool.Parse(ChangeBoolean(row[8].ToString())),
                Status = bool.Parse(ChangeBoolean(row[9].ToString())),
                Program_Slug = row[10].ToString(),
                Program_Name = row[11].ToString(),
                Specialization_Completion_Time = row[13].ToString() != "" ? DateTime.Parse(row[13].ToString()) : DateTime.Parse("01/01/1970"),

            };
        }
        private Student_Course_Log GetStudentCourse(String[] row)
        {

            Student_Course_Log log1 = new Student_Course_Log
            {
                // Roll = row[2].ToString().Split('-')[2],
                Roll = row[0].ToString(),
                Course_Enrollment_Time = row[7].ToString() != "" ? DateTime.Parse(row[7].ToString()) : DateTime.Parse("01/01/1970"),
                Course_Start_Time = row[8].ToString() != "" ? DateTime.Parse(row[8].ToString()) : DateTime.Parse("01/01/1970"),
                Last_Course_Activity_Time = row[9].ToString() != "" ? DateTime.Parse(row[9].ToString()) : DateTime.Parse("01/01/1970"),
                Overall_Progress = Double.Parse(row[10].ToString()),
                Estimated = Double.Parse(row[11].ToString()),
                Completed = Boolean.Parse(ChangeBoolean(row[12].ToString())),
                Status = Boolean.Parse(ChangeBoolean(row[13].ToString())),
                Program_Slug = row[14].ToString(),
                Program_Name = row[15].ToString(),
                Completion_Time = row[17].ToString() != "" ? DateTime.Parse(row[17].ToString()) : DateTime.Parse("01/01/1970"),
                Course_Grade = Double.Parse(row[18].ToString()),
            };
            return log1;
        }
    }
 
}
