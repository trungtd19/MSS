using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System;
using MSS_DEMO.Core.Interface;
using MSS_DEMO.Repository;
using MSS_DEMO.Core.Import;

namespace MSS_DEMO.Controllers
{
    public class ExportDataController : Controller
    {
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
            CSVConvert csv = new CSVConvert();
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
                        if (fileExtension != ".csv")
                        {
                            messageImport = "Please select the excel file with .csv extension";
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
                                        List<string> rows = csv.RegexRow(sreader);                                  
                                        unitOfWork.SpecificationsLog.Insert(getRow.GetStudentSpec(rows));
                                    }
                                }
                                else
                                if (postedFile.FileName.Contains("usage-report"))
                                {
                                    string[] headers = sreader.ReadLine().Split(',');
                                    while (!sreader.EndOfStream)
                                    {
                                        List<string> rows = csv.RegexRow(sreader);
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

