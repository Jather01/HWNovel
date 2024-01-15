using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HWNovel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Main()
        {
            ViewBag.userinfo = Session["userinfo"];
            return View();
        }
    }
}