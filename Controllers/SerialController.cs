using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HWNovel.Controllers
{
    public class SerialController : Controller
    {
        public ActionResult New()
        {
            ViewBag.topmenu = "New";
            ViewBag.userinfo = Session["userinfo"];
            return View();
        }

        public ActionResult Day()
        {
            ViewBag.topmenu = "Day";
            ViewBag.userinfo = Session["userinfo"];
            return View();
        }

        public ActionResult Fin()
        {
            ViewBag.topmenu = "Fin";
            ViewBag.userinfo = Session["userinfo"];
            return View();
        }

        public ActionResult Gen()
        {
            string genre = Request.Params["genre"] as string;
            string genName = "";

            if(genre == "1")
            {
                genName = "Rom";
            }
            else if (genre == "2")
            {
                genName = "Rof";
            }
            else if (genre == "3")
            {
                genName = "Fan";
            }
            else if (genre == "4")
            {
                genName = "Hro";
            }
            else if (genre == "5")
            {
                genName = "Mys";
            }

            ViewBag.topmenu = genName;
            ViewBag.userinfo = Session["userinfo"];

            return View();
        }
    }
}