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

            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 로그인 화면으로 이동
                return RedirectToAction("LoginForm", "User", new { PreUrl = "/Mypage/Rec" });
            }
            else
            {
                return View();
            }
        }

        public ActionResult Wis()
        {
            ViewBag.topmenu = "Wis";
            ViewBag.userinfo = Session["userinfo"];

            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                ViewBag.PreUrl = "/Mypage/Rec";
                // 로그인 정보가 없으면 로그인 화면으로 이동
                return RedirectToAction("LoginForm", "User", new { PreUrl = "/Mypage/Wis" });
            }
            else
            {
                return View();
            }
        }

        public ActionResult Cha()
        {
            ViewBag.topmenu = "Cha";
            ViewBag.userinfo = Session["userinfo"];

            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                ViewBag.PreUrl = "/Mypage/Rec";
                // 로그인 정보가 없으면 로그인 화면으로 이동
                return RedirectToAction("LoginForm", "User", new { PreUrl = "/Mypage/Cha" });
            }
            else
            {
                return View();
            }
        }

        public ActionResult Min()
        {
            ViewBag.topmenu = "Min";
            ViewBag.userinfo = Session["userinfo"];

            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 로그인 화면으로 이동
                return RedirectToAction("LoginForm", "User", new { PreUrl = "/Mypage/Min" });
            }
            else
            {
                string userid = userinfo[0];

                HWN01 result = new HWN01();
                using (var db = new HWNovelEntities())
                {
                    result = db.HWN01.Where(x =>
                               x.USERID == userid
                            && x.USEYN == "1")
                            .SingleOrDefault();
                }

                return View(result);
            }
        }

        [HttpPost]
        public ActionResult Pwcheck(string NextUrl)
        {
            ViewBag.topmenu = "Min";
            ViewBag.userinfo = Session["userinfo"];

            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 로그인 화면으로 이동
                return RedirectToAction("LoginForm", "User", new { PreUrl = "/Mypage/Min" });
            } else
            {
                if (NextUrl == null)
                {
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    ViewBag.NextUrl = NextUrl;
                    return View();
                }
            }
        }
    }
}