using HWNovel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HWNovel.Controllers
{
    public class SerialController : Controller
    {
        public ActionResult Day()
        {
            ViewBag.topmenu = "Day";
            ViewBag.userinfo = Session["userinfo"];

            List<HWN021> genreList = new List<HWN021>();

            using (var db = new HWNovelEntities())
            {
                genreList = db.HWN021.Where(x => x.GROUPNO.Equals("03")).ToList();
            }

            ViewBag.GenreList = genreList;

            return View();
        }

        public ActionResult New()
        {
            ViewBag.topmenu = "New";
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

        public ActionResult Fin()
        {
            ViewBag.topmenu = "Fin";
            ViewBag.userinfo = Session["userinfo"];
            return View();
        }

        public ActionResult NovelManage()
        {
            ViewBag.topmenu = "NovelManage";
            ViewBag.userinfo = Session["userinfo"];

            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                if (userinfo[3] != "1")
                {
                    // 로그인 정보가 있는데 관리자 계정이 아니면 공지사항 목록 화면으로 이동
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    List<HWN021> genreList = new List<HWN021>();

                    using (var db = new HWNovelEntities())
                    {
                        genreList = db.HWN021.Where(x => x.GROUPNO.Equals("03")).ToList();
                    }

                    ViewBag.GenreList = genreList;

                    return View();
                }
            }
        }

        public ActionResult NovelWrite()
        {
            ViewBag.topmenu = "NovelManage";
            ViewBag.userinfo = Session["userinfo"];

            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                if (userinfo[3] != "1")
                {
                    // 로그인 정보가 있는데 관리자 계정이 아니면 공지사항 목록 화면으로 이동
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    List<HWN021> genreList = new List<HWN021>();

                    using (var db = new HWNovelEntities())
                    {
                        genreList = db.HWN021.Where(x => x.GROUPNO.Equals("03")).ToList();
                    }

                    ViewBag.GenreList = genreList;

                    return View();
                }
            }
        }
    }
}