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

        // GET: ImportCSV
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

                    //Validate uploaded file and return error.
                    if (fileExtension != ".csv")
                    {
                        ViewBag.Message = "Please select the csv file with .csv extension";
                        return View();
                    }


                    using (var context = new MSSEntities())

                    {
                        using (var sreader = new StreamReader(postedFile.InputStream))
                        {
                            //First line is header. If header is not passed in csv then we can neglect the below line.
                            string[] headers = sreader.ReadLine().Split(',');
                            //Loop through the records
                            while (!sreader.EndOfStream)
                            {
                                string[] rows = sreader.ReadLine().Split(',');
                                context.Students.Add(GetStudentFromExcelRow(rows));
                            }
                        }
                        context.SaveChanges();
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
        private Student GetStudentFromExcelRow(String[] row)
        {
            return new Student
            {
                External_ID = row[2].ToString(),

            };
        }

    }
}
