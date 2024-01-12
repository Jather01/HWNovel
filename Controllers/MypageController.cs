using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HWNovel.Controllers
{
    public class MypageController : Controller
    {
        public ActionResult Rec()
        {
            ViewBag.topmenu = "Rec";
            return View();
        }

        public ActionResult Wis()
        {
            ViewBag.topmenu = "Wis";
            return View();
        }

        public ActionResult Cha()
        {
            ViewBag.topmenu = "Cha";
            return View();
        }

        public ActionResult Min()
        {
            ViewBag.topmenu = "Min";
            return View();
        }
    }
}