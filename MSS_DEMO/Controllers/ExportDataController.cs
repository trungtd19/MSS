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
        public ActionResult Index(HttpPostedFileBase postedFile1) //, HttpPostedFileBase postedFile2, HttpPostedFileBase postedFile3)
        {
            ArrayList postedList = new ArrayList();
            postedList.Add(postedFile1);
            //  postedList.Add(postedFile2);
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
                                   
                                    string[] headers = sreader.ReadLine().Split(',');
                                    while (!sreader.EndOfStream)
                                    {
                                        string[] rows = sreader.ReadLine().Split(',');
                                      //  if (postedFile.FileName.Contains("membership"))
                                     //   {
                                            context.Students.Add(GetStudent(rows));
                                     //   }
                                        //else
                                        //     if (postedFile.FileName.Contains("specialization"))
                                        //{
                                        //    context.Subjects.
                                        //}
                                        //else
                                        //     if (postedFile.FileName.Contains("usage"))
                                        //{

                                        //}
                                    }
                                }
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            string folderPath = Server.MapPath("~/UploadedFiles/");
                        
                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            var filePath = folderPath + Path.GetFileName(postedFile.FileName);
                            postedFile.SaveAs(filePath);
                            string excelConString = "";
                            switch (fileExtension)
                            {                        
                                case ".xls":
                                    excelConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                                    break;                      
                                case ".xlsx":
                                    excelConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                                    break;
                              
                            }

                            DataTable dt = new DataTable();
                            excelConString = string.Format(excelConString, filePath);

                            using (OleDbConnection excelOledbConnection = new OleDbConnection(excelConString))
                            {
                                using (OleDbCommand excelDbCommand = new OleDbCommand())
                                {
                                    using (OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter())
                                    {
                                        excelDbCommand.Connection = excelOledbConnection;

                                        excelOledbConnection.Open();
                                        DataTable excelSchema = GetSchemaFromExcel(excelOledbConnection);
                                        string sheetName = excelSchema.Rows[0]["TABLE_NAME"].ToString();
                                        excelOledbConnection.Close();

                                        excelOledbConnection.Open();
                                        excelDbCommand.CommandText = "SELECT * From [" + sheetName + "]";
                                        excelDataAdapter.SelectCommand = excelDbCommand;
                                        excelDataAdapter.Fill(dt);
                                        excelOledbConnection.Close();
                                    }
                                }
                            }


                            using (var context = new MSSEntities())

                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    //if (postedFile.FileName.Contains("CP"))
                                    //{
                                    //    context.Campus.Add(GetStudentFromExcelRow(row));
                                    //}
                                    //else
                                    //if (postedFile.FileName.Contains("Class"))
                                    //{
                                    //  //  context.Classes.Add(GetClassFromExcelRow(row));
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
   
        private static DataTable GetSchemaFromExcel(OleDbConnection excelOledbConnection)
        {
            return excelOledbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        }

        private Campu GetStudent(DataRow row)
        {
            return new Campu
            {
              Campus_Name = row[0].ToString(),

            };
        }
       
        private Student GetStudent(String[] row)
        {
           
            return new Student
            {
                Student_Name = row[0].ToString(),
                Email = row[1].ToString(),
                External_ID = row[2].ToString(),
                Sum_Enrolled_Courses = int.Parse(row[5].ToString()),
                Sum_Completed_Courses = int.Parse(row[6].ToString()),
                Member_State = row[7].ToString(),
                Join_Date = DateTime.Parse(row[8].ToString()),
                Invitation_Date = DateTime.Parse(row[9].ToString()),
                Last_Activity_Time = DateTime.Parse(row[10].ToString()),

            };
        }
        private Student_Specification_Log GetStudentSpec(String[] row)
        {
            return new Student_Specification_Log
            {               
                External_ID = row[1].ToString(),
                //Subject_Name = row[1].ToString(),
                Campus = row[1].ToString(),
                Specialization = row[1].ToString(),
                Specialization_Slug = row[1].ToString(),
                University = row[1].ToString(),
                Specialization_Enrollment_Time = DateTime.Parse(row[9].ToString()),
                Last_Specialization_Activity_Time = DateTime.Parse(row[9].ToString()),
                Completed = Boolean.Parse(row[1].ToString()),
                Status = Boolean.Parse(row[1].ToString()),
                Program_Slug = row[7].ToString(),
                Program_Name = row[7].ToString(),
                
            };
        }
        private Student_Course_Log GetStudentCourse(String[] row)
        {
            return new Student_Course_Log
            {
                //External_ID = row[1].ToString(),            
                Course_Enrollment_Time = DateTime.Parse(row[9].ToString()),
                Course_Start_Time = DateTime.Parse(row[9].ToString()),
                Last_Course_Activity_Time = DateTime.Parse(row[9].ToString()),
                Overall_Progress = int.Parse(row[1].ToString()),
                Estimated = int.Parse(row[1].ToString()),
                Completed = Boolean.Parse(row[1].ToString()),
                Status = Boolean.Parse(row[1].ToString()),
                Program_Slug = row[7].ToString(),
                Program_Name = row[7].ToString(),
                Completion_Time = DateTime.Parse(row[9].ToString()),
                Course_Grade = int.Parse(row[1].ToString()),

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
               Subject_ID = row[1].ToString(),
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
        private Class_Detail GetClassDetail(String[] row)
        {
            return new Class_Detail
            {

                Class_ID = row[1].ToString(),
                External_ID = row[1].ToString(),

            };
        }
        private Subject_detail GetSubjectDetail(String[] row)
        {
            return new Subject_detail
            {

                Subject_ID = row[1].ToString(),
                External_ID = row[1].ToString(),

            };
        }

    }
}
