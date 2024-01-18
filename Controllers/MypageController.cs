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
            ViewBag.userinfo = Session["userinfo"];
            return View();
        }

        public ActionResult Wis()
        {
            ViewBag.topmenu = "Wis";
            ViewBag.userinfo = Session["userinfo"];
            return View();
        }

        public ActionResult Cha()
        {
            ViewBag.topmenu = "Cha";
            ViewBag.userinfo = Session["userinfo"];
            return View();
        }

        public ActionResult Min()
        {
            ViewBag.topmenu = "Min";
            ViewBag.userinfo = Session["userinfo"];
            return View();
        }
    }
}