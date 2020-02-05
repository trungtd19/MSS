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
using MSS_DEMO.Repository;
using MSS_DEMO.Core.Interface;

namespace MSS_DEMO.Controllers
{
    public class ImportStudentController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IGetRow getRow;
        public ImportStudentController(IUnitOfWork _unitOfWork,IGetRow _getRow)
        {
            this.unitOfWork = _unitOfWork;
            this.getRow = _getRow;
        }

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
                                    //var x = unitOfWork.Students.GetById(GetStudent(rows).Roll);
                                    var x = unitOfWork.Students.CheckExits(getRow.GetStudent(rows).Roll);
                                    if (x == null)
                                    {
                                        unitOfWork.Students.Insert(getRow.GetStudent(rows));
                                        //   context.Subject_Student.Add(GetSubjectStudent(rows));
                                        //  context.Class_Student.Add(GetClassStudent(rows));
                                    }
                                    else
                                    {
                                        messageImport = "ROll exits";
                                        return Json(new { message = messageImport });
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
            catch
            {
                messageImport = "Please select the file first to upload.";
            }
            return Json(new { message = messageImport });
        }      
    }
}
