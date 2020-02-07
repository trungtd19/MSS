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
        public ActionResult UploadFiles()
        {
            string messageImport = null;
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase postedFile = files[i];

                if (postedFile != null)
                {
                    try
                    {
                        string fileExtension = Path.GetExtension(postedFile.FileName);
                        if (fileExtension != ".xls" && fileExtension != ".xlsx" && fileExtension != ".csv")
                        {
                            messageImport = "Please select the excel file with .xls or .xlsx or .csv extension";
                            return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        if (fileExtension == ".csv")
                        {
                            using (var sreader = new StreamReader(postedFile.InputStream))
                            {
                                if (postedFile.FileName.Contains("specialization-report"))
                                {
                                    string[] headers = sreader.ReadLine().Split(',');

                                    while (!sreader.EndOfStream)
                                    {
                                        string stringFormat = sreader.ReadLine();
                                        string[] rows = stringFormat.Split(',');
                                        Student_Specification_Log st = getRow.GetStudentSpec(rows);
                                        unitOfWork.SpecificationsLog.Insert(st);
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
                                        unitOfWork.CoursesLog.Insert(getRow.GetStudentCourse(rows));
                                    }

                                }
                                else
                                {
                                    messageImport = "File không chuẩn!";
                                    return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            unitOfWork.Save();
                            if (unitOfWork.Save())
                            {
                                messageImport = "Import successfull!";
                            }
                            else
                            {
                                messageImport = "Exception!";
                            }
                        }
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
            return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
        }
    }

}

