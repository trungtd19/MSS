using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MSS_DEMO.Common;
using MSS_DEMO.Models;
using MSS_DEMO.Repository;
using PagedList;

namespace MSS_DEMO.Controllers
{
    [CheckCredential(Role_ID = "1")]
    public class SubjectsController : Controller
    {
        private IUnitOfWork unitOfWork;
        public SubjectsController(IUnitOfWork _unitOfWork)
        {
            this.unitOfWork = _unitOfWork;
        }
        public ActionResult Index(int? page, string SearchString, string searchCheck, string currentFilter, bool checkActive = true, bool checkActivePage = true)
        {
            List<Subject> List = new List<Subject>();
            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
                checkActive = checkActivePage;
            }
            ViewBag.CurrentFilter = SearchString;
            ViewBag.checkActivePage = checkActive;
            if (!String.IsNullOrEmpty(searchCheck))
            {
                List = unitOfWork.Subject.GetPageList();
                if (!String.IsNullOrWhiteSpace(SearchString))
                {
                    List = List.Where(s => s.Subject_ID.Trim().ToUpper().Contains(SearchString.Trim().ToUpper())).ToList();
                }
                if (checkActive)
                {
                    List = List.Where(s => s.Subject_Active == true).ToList();
                }
                if (List.Count == 0)
                {
                    ViewBag.Nodata = "Showing 0 results";
                }
                else
                {
                    ViewBag.Nodata = "";
                }
            }
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            ViewBag.Count = List.Count();
            return View(List.ToList().ToPagedList(pageNumber, pageSize));

        }

        public ActionResult Details(string id)
        {
            var Subject = unitOfWork.Subject.GetById(id);
            return View(Subject);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Subject_ID,Subject_Name,Subject_Active")] Subject subject)
        {
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
            if (!regexItem.IsMatch(subject.Subject_ID))
            {
                ViewBag.Error = "Subject ID invalid!";
                return View();
            }
            if (unitOfWork.Subject.IsExitsSubject(subject.Subject_ID))
            {
                ViewBag.Error = "This subject exits!";
                return View();
            }
            if (ModelState.IsValid)
            {
                unitOfWork.Subject.Insert(subject);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(subject);
        }


        public ActionResult Edit(string id)
        {
            var subject = unitOfWork.Subject.GetById(id);
            return View(subject);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Subject_ID,Subject_Name,Subject_Active")] Subject subject)
        {    
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.Subject.Update(subject);
                    unitOfWork.Subject.Save(subject.Subject_Name);
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                ViewBag.Error = "This subject exits!";
                return View(subject);
            }
            return View(new Subject());
        }
        public ActionResult Delete(string id)
        {
            var subject = unitOfWork.Subject.GetById(id);
            return View(subject);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var subject = unitOfWork.Subject.GetById(id);
            unitOfWork.Subject.Delete(subject);
            if (!unitOfWork.Save())
            {
                ViewBag.mess = "Can't delete this subject!";
                return View(subject);
            }
            return RedirectToAction("Index");
        }
    }
}
