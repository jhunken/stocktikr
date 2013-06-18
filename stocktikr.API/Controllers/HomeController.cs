using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace stocktikr.API.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //our initial view
            return View();
        }
        public ActionResult About()
        {
            //about view
            return View();
        }
    }
}
