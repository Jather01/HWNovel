using HWNovel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        [HttpPost]
        public ActionResult InfoUpdateForm(string pwYn)
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
                if (pwYn == "")
                {
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpPost]
        public ActionResult InfoUpdate(User user)
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
                if (user == null)
                {
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    string name = user.Userid;
                    string birthday = user.Birthday;
                    string nickname = user.Nickname;

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(birthday) && !string.IsNullOrEmpty(nickname))
                    {
                        using (var db = new HWNovelEntities())
                        {
                            db.PRO_USER_INFOUPDATE(userinfo[0], name, birthday, nickname);
                            db.SaveChanges();
                        }
                    }

                    return RedirectToAction("Min", "Mypage");
                }
            }
        }

        [HttpPost]
        public ActionResult PwUpdateForm(string pwYn)
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
                if (pwYn == "")
                {
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpPost]
        public ActionResult PwUpdate(User user)
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
                if (user == null)
                {
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    string id = userinfo[0];
                    string password = user.Password;

                    if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password))
                    {
                        using (var db = new HWNovelEntities())
                        {
                            var result = db.HWN01.SingleOrDefault(b => b.USERID == id);
                            if (result != null)
                            {
                                SHA256 sha = new SHA256Managed();
                                byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(password));
                                StringBuilder sb = new StringBuilder();
                                foreach (byte b in hash)
                                {
                                    sb.AppendFormat("{0:x2}", b);
                                }

                                result.ENCPASSWORD = sb.ToString();
                                db.SaveChanges();
                            }
                        }
                    }
                    return RedirectToAction("Min", "Mypage");
                }
            }
        }

        [HttpPost]
        public ActionResult UserDeleteForm(string pwYn)
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
                if (pwYn == "")
                {
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpPost]
        public ActionResult UserDelete(User user)
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
                if (user == null)
                {
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    string id = userinfo[0];

                    if (!string.IsNullOrEmpty(id))
                    {
                        using (var db = new HWNovelEntities())
                        {
                            var result = db.HWN01.SingleOrDefault(b => b.USERID == id);
                            if (result != null)
                            {
                                result.USEYN = "2";
                                db.SaveChanges();
                            }
                        }

                        Response.Cookies["HWNovel"].Expires = DateTime.Today.AddDays(-1);
                    }

                    return RedirectToAction("Logout", "User");
                }
            }
        }
    }
}