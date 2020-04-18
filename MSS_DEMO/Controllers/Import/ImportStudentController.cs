using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.Models;
using System.IO;
using MSS_DEMO.Repository;
using MSS_DEMO.Core.Interface;
using MSS_DEMO.Core.Import;
using System.Text;
using System.Linq;

namespace MSS_DEMO.Controllers
{
    public class ImportStudentController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IGetRow getRow;
        public ImportStudentController(IUnitOfWork _unitOfWork, IGetRow _getRow)
        {
            this.unitOfWork = _unitOfWork;
            this.getRow = _getRow;
        }

        public ActionResult Index()
        {
            List<Semester> semester = unitOfWork.Semesters.GetAll();
            List<Semester> _semester = new List<Semester>();
            _semester.Add(new Semester { Semester_ID = "None", Semester_Name = "--- Choose Semester ---" });
            foreach (var sem in semester)
            {
                _semester.Add(sem);
            }
            ViewBag.Semester_ID = new SelectList(_semester, "Semester_ID", "Semester_Name");
            return View();
        }

        [HttpPost]
        public ActionResult Import()
        {
            int countFail = 0;
            int countSuccess = 0;
            string messageImport = "";
            CSVConvert csv = new CSVConvert();
            try
            {
                HttpPostedFileBase postedFile = Request.Files[0];
                string semester = Request["Semester"];
                if (postedFile != null)
                {
                    try
                    {
                        string fileExtension = Path.GetExtension(postedFile.FileName);
                        if (fileExtension.ToLower() != ".csv")
                        {
                            messageImport = "Please select the csv file with .csv extension";
                            return Json(new { message = messageImport });
                        }
                        try
                        {
                            using (var sreader = new StreamReader(postedFile.InputStream))
                            {
                                string[] headers = sreader.ReadLine().Split(',');
                                if (headers.Length == 4)
                                {
                                    if (!headers[0].Contains("Roll Number") || !headers[1].Contains("Full Name") || !headers[2].Contains("Email") || !headers[3].Contains("Subject"))
                                    {
                                        messageImport = "Invalid file structure";
                                        return Json(new { message = messageImport });
                                    }
                                }
                                else
                                {
                                    messageImport = "The number of columns is invalid";
                                    return Json(new { message = messageImport });
                                }
                                int startRow = 1;
                                List<string> listAddRoll = new List<string>();
                                while (!sreader.EndOfStream)
                                {
                                    List<string> rows = csv.RegexRow(sreader);
                                    if (!rows[2].Contains("@fpt.edu.vn"))
                                    {
                                        messageImport = "Row "+ startRow+": Email invalid";
                                        return Json(new { message = messageImport });
                                    }       
                                    //if (!unitOfWork.Classes.CheckExitsClass(getRow.GetClassStudent(rows).Class_ID)
                                    //    || 
                                    if (unitOfWork.Subject.IsExitsSubject(getRow.GetSubjectStudent(rows, semester).Subject_ID))
                                    {                       
                                        var countAdd = listAddRoll.Where(o => o.Contains(getRow.GetStudent(rows, semester, campus).Roll)).ToList().Count();
                                        if ((unitOfWork.Students.IsExtisStudent(getRow.GetStudent(rows, semester, campus).Roll, semester)
                                            && !unitOfWork.Subject.IsExitsSubject(getRow.GetSubjectStudent(rows, semester).Subject_ID))
                                            || (countAdd > 0))
                                        {
                                            unitOfWork.SubjectStudent.Insert(getRow.GetSubjectStudent(rows, semester));
                                            // unitOfWork.ClassStudent.Insert(getRow.GetClassStudent(rows));
                                            countSuccess++;
                                        }
                                        else
                                        if (!unitOfWork.Students.IsExtisStudent(getRow.GetStudent(rows, semester, campus).Roll, semester) && countAdd == 0)
                                        {
                                            unitOfWork.Students.Insert(getRow.GetStudent(rows, semester));
                                            unitOfWork.SubjectStudent.Insert(getRow.GetSubjectStudent(rows, semester));
                                            listAddRoll.Add(getRow.GetStudent(rows, semester, campus).Roll);
                                            //unitOfWork.ClassStudent.Insert(getRow.GetClassStudent(rows));
                                            countSuccess++;
                                        }
                                        else
                                        {
                                            countFail++;
                                        }
                                    }
                                    else
                                    {
                                        countFail++;
                                    }
                                }
                            }
                            if (unitOfWork.Save())
                            {
                                if (countSuccess != 0)
                                {
                                    messageImport = countFail != 0 ? "Import success" + countSuccess + " students and fail " + countFail + " students!" : "Import success " + countSuccess + " students!";
                                }
                                else messageImport = "Import fail " + countFail + " students";
                            }
                            else
                            {
                                messageImport = "Import Fail!";
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
        public void Export_Student_CSV()
        {
            var sb = new StringBuilder(); 
            Type type = typeof(Student);
            var props = type.GetProperties();
            sb.Append(string.Join(",", "No", "Full Name","Email", "External Id","Campus","Subject"));
            sb.Append(Environment.NewLine);
            sb.Append(string.Join(",", "SE0001", "Full Name SE0001", "FullnameSE0001@fpt.edu.vn", "HRM201c-HN-SE0001","SG","HRM201c"));
            sb.Append(Environment.NewLine);
            sb.Append(string.Join(",", "SE0002", "Full Name SE0002", "FullnameSE0002@fpt.edu.vn", "SSL201c-SG-SE0002","HN", "SSL201c"));
            sb.Append(Environment.NewLine);
            sb.Append(string.Join(",", "SE0003", "Full Name SE0003", "FullnameSE0003@fpt.edu.vn", "PMG201c-DN-SE0003","DN", "PMG201c"));
            sb.Append(Environment.NewLine);
            var response = System.Web.HttpContext.Current.Response;
            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.ContentEncoding = Encoding.Unicode;
            response.AddHeader("content-disposition", "attachment;filename=StudentTemplate.CSV ");
            response.ContentType = "text/plain";
            response.Write(sb.ToString());
            response.End();
        }

    }
}
