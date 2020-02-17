using MSS_DEMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSS_DEMO.Controllers.SetUp
{
    public class UserController : Controller
    {
        // GET: User
        private MSSEntities DB = new MSSEntities();
        // GET: ADD_User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(User_Role ur)
        {

            if (ModelState.IsValid)
            {
                ur.Login = ur.Login + "@fpt.edu.vn";
                DB.User_Role.Add(ur);

                DB.SaveChanges();
                ModelState.AddModelError("", "Success");
                return RedirectToAction("ListAll");
            }


            ViewBag.Role_ID = new SelectList(DB.Roles, "Role_ID", "Role_Name", ur.Role_ID);
            return View();
        }

        public ActionResult Edit(string id)
        {
            var ur = DB.User_Role.Find(id);
            if (ur == null)
            {
                return HttpNotFound();
            }
            string[] arrListStr = ur.Login.Split('@');
            ur.Login = arrListStr[0];

            ViewBag.Role_ID = new SelectList(DB.Roles, "Role_ID", "Role_Name", ur.Role_ID);

            return View(ur);
        }
        [HttpPost]
        public ActionResult Edit(User_Role userrole)
        {
            var ur = DB.User_Role.Find(userrole.User_ID);
            ur.User_ID = userrole.User_ID;
            ur.Login = userrole.Login + "@fpt.edu.vn";
            ur.Role = ur.Role;
            ur.isActive = userrole.isActive;
            DB.SaveChanges();
            return RedirectToAction("ListAll"); ;
        }
        public ActionResult ListAll()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ListAll(string search)
        {

            IQueryable<User_Role> model = DB.User_Role;
            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(x => x.Login.Contains(search));

            }
            return View(model.OrderByDescending(x => x.Role_ID).ToList());


        }
    }
}