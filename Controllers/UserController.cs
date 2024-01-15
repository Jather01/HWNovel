using HWNovel.ViewModels;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace HWNovel.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User model)
        {
            string id = model.Userid;
            string password = model.Password;

            HWN01 result = new HWN01();
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password))
            {
                SHA256 sha = new SHA256Managed();
                byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.AppendFormat("{0:x2}", b);
                }

                string encpassword = sb.ToString();

                using (var db = new HWNovelEntities())
                {
                    result = db.HWN01.Where(x =>
                               x.USERID == id
                            && x.ENCPASSWORD == encpassword
                            && x.POWER == "2"
                            && x.USEYN == "1")
                            .SingleOrDefault();
                }
            }
            if (result != null)
            {
                // select 결과가 null이 아니면 로그인에 성공

                // 세션에 로그인 정보 저장
                List<string> userinfo = new List<string>(new string[] {
                    result.USERID,
                    result.NAME,
                    result.NICKNAME,
                    result.POWER
                });

                Session["userinfo"] = userinfo;

                HttpCookie cookie = new HttpCookie("HWNovel")
                {
                    Domain = "localhost",
                    Expires = DateTime.Now.AddDays(3)
                };

                cookie["id"] = Server.UrlEncode(id);

                Response.Cookies.Add(cookie);

                // 메인 홈 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                ViewBag.Id = id;
                ViewBag.loginError = "error";

                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Main", "Home");
        }

        public ActionResult SigninAgree()
        {
            return View();
        }

        public ActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signin(User model)
        {
            SHA256 sha = new SHA256Managed();
            if (ModelState.IsValid)
            {
                HWN01 user = new HWN01();
                user.USERID = model.Userid;
                user.NAME = model.Name;
                user.BIRTHDAY = model.Birthday;
                user.NICKNAME = model.Nickname;
                user.SIGNUPDATE = DateTime.Now;
                user.POWER = "2";
                user.USEYN = "1";

                byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(model.Password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.AppendFormat("{0:x2}", b);
                }

                user.ENCPASSWORD = sb.ToString();

                using (var db = new HWNovelEntities())
                {
                    db.HWN01.Add(user);
                    db.SaveChanges();
                }
                return RedirectToAction("Login", "User");
            }
            return View(model);
        }

        public ActionResult FindId()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FoundId(User model)
        {
            string id = model.Userid;
            string[] ids = null;
            if (id != null)
            {
                ids = id.Split(',');
            }

            ViewBag.ids = ids;
            ViewBag.name = model.Name;

            return View();
        }

        public ActionResult FindPw()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPw(User model)
        {
            string id = model.Userid;

            ViewBag.id = id;

            return View();
        }

        [HttpPost]
        public ActionResult UpdatePw(User model)
        {
            string id = model.Userid;
            string password = model.Password;

            if(!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password))
            {
                using (var db = new HWNovelEntities())
                {
                    var result = db.HWN01.SingleOrDefault(b => b.USERID == id);
                    if (result != null)
                    {
                        SHA256 sha = new SHA256Managed();
                        byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(model.Password));
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

            return RedirectToAction("Login", "User");
        }
    }
}