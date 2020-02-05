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
using MSS_DEMO.Core.Interface;
using MSS_DEMO.Repository;

namespace MSS_DEMO.Controllers
{
    public class ExportDataController : Controller
    {
        private MSSEntities db = new MSSEntities();
        private IUnitOfWork unitOfWork;
        private IGetRow getRow;
        public ExportDataController(IUnitOfWork _unitOfWork, IGetRow _getRow)
        {
            this.unitOfWork = _unitOfWork;
            this.getRow = _getRow;
        }
        public ActionResult Index()
        {
            return View("~/Views/ImportDataDaily/Index.cshtml");
        }
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
                                                    Student_Specification_Log st = getRow.GetStudentSpec(rows);
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
                                                    string[] rows = stringFormat.Split(',');
                                                    context.Student_Course_Log.Add(getRow.GetStudentCourse(rows));
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
    }
}
