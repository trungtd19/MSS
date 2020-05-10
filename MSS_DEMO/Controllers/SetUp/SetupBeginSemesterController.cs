using MSS_DEMO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSS_DEMO.Controllers
{
    public class SetupBeginSemesterController : Controller
    {
        [CheckCredential(Role_ID = "1")]
        public ActionResult Index()
        {
            return View();
        }
    }
}