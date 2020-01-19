using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSS_DEMO.Controllers
{
    public class Import2Controller : Controller
    {
        // GET: Import2
        public ActionResult Index()
        {
            return View();
        }
        public String ReplaceCharacter(string item)
        {
           
            char c = '"';
            if  (item.IndexOf(c)>0)
                    {
                        int pFrom = item.IndexOf(c) + 1;
                        int pTo = item.LastIndexOf(c);
                        string result = item.Substring(pFrom, pTo - pFrom);
                        return result.Replace(" ", "-");
                    }
            return item;
            
        }
        public ActionResult Upload(HttpPostedFileBase postedFile)
        {

            //string fileExtension = Path.GetExtension(postedFile.FileName);

            var csvReader = new StreamReader(postedFile.InputStream);
            var uploadModelList = new List<Student_Specification_Log>();
            string inputDataRead;
            var values = new List<string>();
            while ((inputDataRead = csvReader.ReadLine()) != null)
            {
                values.Add(inputDataRead.Trim().Replace(" ", "").Replace(",", " "));
            }
            values.Remove(values[0]);
           // values.Remove(values[values.Count - 1]);
            using (var context = new MSSEntities())
            {
                foreach (var value in values)
                {
                    var uploadModelRecord = new Student_Specification_Log();
                    ReplaceCharacter(value);
                    var eachValue = value.Split(' ');
                    uploadModelRecord.Roll= eachValue[0] != "" ? eachValue[2].Split('-')[2] : string.Empty;
                    uploadModelRecord.Subject_ID = eachValue[3] != "" ? eachValue[3] : string.Empty;
                    uploadModelRecord.Campus = eachValue[4] != "" ? eachValue[4] : string.Empty;
                    uploadModelRecord.Specialization = eachValue[5] != "" ? eachValue[5] : string.Empty;
                    uploadModelRecord.Specialization_Slug = eachValue[6] != "" ? eachValue[6] : string.Empty;
                    uploadModelRecord.University = eachValue[7] != "" ? eachValue[7] : string.Empty;
                    //uploadModelRecord.Specialization_Enrollment_Time = eachValue[3] != "" ? eachValue[3] : string.Empty;
                    //uploadModelRecord.Last_Specialization_Activity_Time = eachValue[3] != "" ? eachValue[3] : string.Empty;
                    //uploadModelRecord.Completed = eachValue[3] != "" ? eachValue[3] : string.Empty;
                    uploadModelList.Add(uploadModelRecord);// newModel needs to be an object of type ContextTables.
                    context.Student_Specification_Log.Add(uploadModelRecord);
                }
                context.SaveChanges();
            }
            return Json(null);
        }
    }
}