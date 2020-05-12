using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System;
using MSS_DEMO.Core.Interface;
using MSS_DEMO.Repository;
using MSS_DEMO.Core.Import;
using MSS_DEMO.Common;
using System.Text;

namespace MSS_DEMO.Controllers
{
    [CheckCredential(Role_ID = "1")]
    public class ImportDataDailyController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IGetRow getRow;
        public ImportDataDailyController(IUnitOfWork _unitOfWork, IGetRow _getRow)
        {
            this.unitOfWork = _unitOfWork;
            this.getRow = _getRow;
        }

        public ActionResult UsageReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadFilesUsage()
        {
            int countSusscess = 0;
            int countFail = 0;
            int userID = int.Parse(Request["UserID"]);
            string _dateImport = Request["dateImport"];
            CSVConvert csv = new CSVConvert();
            string messageImport = "";
            HttpFileCollectionBase files = Request.Files;
            StringBuilder sb = new StringBuilder();
            if (files.Count == 0)
            {
                messageImport = "Please select the file first to upload!";
                return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
            }
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase postedFile = files[i];

                if (postedFile != null)
                {
                    try
                    {
                        string fileExtension = Path.GetExtension(postedFile.FileName);
                        if (fileExtension.ToLower() != ".csv")
                        {
                            messageImport = "Please select the excel file with .csv extension";
                            return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        if (fileExtension == ".csv")
                        {
                            using (var sreader = new StreamReader(postedFile.InputStream))
                            {
                                int startRow = 1;
                                string[] headers = sreader.ReadLine().Split(',');
                                if (headers.Length == 19)
                                {
                                    if (!headers[0].Contains("Name") || !headers[1].Contains("Email") || !headers[2].Contains("External Id") || !headers[3].Contains("Course") || !headers[4].Contains("Course ID") ||
                                        !headers[5].Contains("Course Slug") || !headers[6].Contains("University") || !headers[7].Contains("Enrollment Time") || !headers[8].Contains("Class Start Time") ||
                                        !headers[9].Contains("Last Course Activity Time") || !headers[10].Contains("Overall Progress") || !headers[11].Contains("Estimated Learning Hours") ||
                                        !headers[12].Contains("Completed") || !headers[13].Contains("Removed From Program") || !headers[14].Contains("Program Slug") ||
                                        !headers[15].Contains("Program Name") || !headers[16].Contains("Enrollment Source") || !headers[17].Contains("Completion Time") || !headers[18].Contains("Course Grade"))
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
                                if (unitOfWork.CoursesLog.IsExitsDateImport(_dateImport))
                                {
                                    messageImport = "Reported date existed, if you continue please delete records this date!";
                                    return Json(new { message = messageImport });
                                }
                                var listCoureseName = unitOfWork.Courses.GetList();
                                string semesterID = unitOfWork.Semesters.checkDateOfSemester(_dateImport);

                                while (!sreader.EndOfStream)
                                {
                                    startRow++;
                                    try
                                    {
                                        List<string> rows = csv.RegexRow(sreader);
                                        if (!unitOfWork.Subject.IsExitsSubject(rows[2].ToString().Split('-')[0]))
                                        {
                                            countFail++;
                                            continue;
                                        }
                                        if (!unitOfWork.Campus.IsExitsCampusID(rows[2].ToString().Split('-')[1]))
                                        {
                                            countFail++;
                                            continue;
                                        }
                                        if (!unitOfWork.Students.IsExtisStudent(rows[2].ToString().Split('-')[2], semesterID))
                                        {
                                            countFail++;
                                            continue;
                                        }
                                        for (int j = 0; j< rows.Count; j++)
                                        {
                                            rows[j] = rows[j].Replace("'","''");
                                        }
                                        sb.Append(unitOfWork.CoursesLog.getSqlQuery(rows, userID, _dateImport, listCoureseName, semesterID, unitOfWork.CoursesLog.getSubjectID(rows[3].ToString(), rows[2].ToString().Split('-')[2], semesterID)));
                                    }
                                    catch
                                    {
                                        messageImport = "Row " + startRow + " invaild!";
                                        return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            try
                            {
                                countSusscess = unitOfWork.CoursesLog.countInsert(sb);
                            }
                            catch (Exception ex)
                            {
                                messageImport = ex.Message.Split('/')[0];
                                return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
                            }
                            if (countFail == 0 && countSusscess != 0)
                            {
                                messageImport = "Import success " + countSusscess + " records";
                            }
                            else
                                messageImport = countSusscess == 0 ? "Import Fail" : "Import success " + countSusscess + " and fail " + countFail + " records!";
                        }
                    }
                    catch (Exception ex)
                    {
                        messageImport = ex.Message;
                    }
                }
                else
                {
                    messageImport = "Please select the file first to upload!";
                }
            }
            return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SpecializationReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadFilesSpecialization()
        {
            int countSusscess = 0;
            int countFail = 0;
            int userID = int.Parse(Request["UserID"]);
            string _dateImport = Request["dateImport"];
            CSVConvert csv = new CSVConvert();
            string messageImport = "";
            StringBuilder sb = new StringBuilder();
            HttpFileCollectionBase files = Request.Files;
            if (files.Count == 0)
            {
                messageImport = "Please select the file first to upload!";
                return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
            }
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase postedFile = files[i];

                if (postedFile != null)
                {
                    try
                    {
                        string fileExtension = Path.GetExtension(postedFile.FileName);
                        if (fileExtension.ToLower() != ".csv")
                        {
                            messageImport = "Please select the excel file with .csv extension";
                            return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        if (fileExtension == ".csv")
                        {
                            using (var sreader = new StreamReader(postedFile.InputStream))
                            {
                                int startRow = 0;
                                string[] headers = sreader.ReadLine().Split(',');
                                if (headers.Length == 14)
                                {
                                    if (!headers[0].Contains("Name") || !headers[1].Contains("Email") || !headers[2].Contains("External Id") || !headers[3].Contains("Specialization") || !headers[4].Contains("Specialization Slug") ||
                                       !headers[5].Contains("University") || !headers[6].Contains("Enrollment Time") || !headers[7].Contains("Last Specialization Activity Time") || !headers[8].Contains("Completed") ||
                                       !headers[9].Contains("Removed From Program") || !headers[10].Contains("Program Slug") || !headers[11].Contains("Program Name") ||
                                       !headers[12].Contains("Enrollment Source") || !headers[13].Contains("Specialization Completion Time"))
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
                                if (unitOfWork.SpecificationsLog.IsExitsDateImport(_dateImport))
                                {
                                    messageImport = "Reported date existed, if you continue please delete records this date!";
                                    return Json(new { message = messageImport });
                                }
                                var specifications = unitOfWork.Specifications.GetAll();
                                string semesterID = unitOfWork.Semesters.checkDateOfSemester(_dateImport);
                                while (!sreader.EndOfStream)
                                {
                                    startRow++;
                                    try
                                    {
                                        List<string> rows = csv.RegexRow(sreader);
                                        if (!unitOfWork.Campus.IsExitsCampusID(rows[2].ToString().Split('-')[1]))
                                        {
                                            countFail++;
                                            continue;
                                        }
                                        if (!unitOfWork.Students.IsExtisStudent(rows[2].ToString().Split('-')[2], semesterID))
                                        {
                                            countFail++;
                                            continue;
                                        }
                                        for (int j = 0; j < rows.Count; j++)
                                        {
                                            rows[j] = rows[j].Replace("'", "''");
                                        }
                                        sb.Append(unitOfWork.SpecificationsLog.getSqlQuery(rows, userID, _dateImport, specifications, semesterID, unitOfWork.SpecificationsLog.getSubjectID(rows[3].ToString(), rows[2].ToString().Split('-')[2], semesterID)));
                                    }
                                    catch
                                    {
                                        messageImport = "Row " + startRow + " invaild!";
                                        return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            try
                            {
                                countSusscess = unitOfWork.CoursesLog.countInsert(sb);
                            }
                            catch (Exception ex)
                            {
                                messageImport = ex.Message;
                                return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
                            }                            
                            if (countFail == 0 && countSusscess != 0)
                            {
                                messageImport = "Import success " + countSusscess + " records";
                            }
                            else
                                messageImport = countSusscess == 0 ? "Import Fail" : "Import success " + countSusscess + " and fail " + countFail + " records!";
                        }
                    }
                    catch (Exception ex)
                    {
                        messageImport = ex.Message;
                    }
                }
                else
                {
                    messageImport = "Please select the file first to upload!";
                }
            }
            return Json(new { message = messageImport }, JsonRequestBehavior.AllowGet);
        }
    }
}

