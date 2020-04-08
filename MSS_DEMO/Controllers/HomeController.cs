using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSS_DEMO.ServiceReference;
namespace MSS_DEMO.Controllers
{
    public class HomeController : BaseController
    {
        
        // GET: Home
        public ActionResult Index()
        {
            MSSWSSoapClient soap = new MSSWSSoapClient();
            ViewBag.DemoSoap = soap.HelloWorld();
            return View();
        }
        public ActionResult Report()
        {
            return View();
        }
    }
}