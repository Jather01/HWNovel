﻿using HWNovel.ViewModels;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Security;
using WebGrease.Activities;
using System.Net;

namespace HWNovel.Controllers
{
    public class UserController : Controller
    {
        public ActionResult LoginForm(User model)
        {
            ViewBag.Id = model.Userid;
            ViewBag.PreUrl = model.PreUrl;
            ViewBag.loginError = model.LoginError;

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
                            && x.USEYN == "1")
                            .SingleOrDefault();
                }
            }
            if (result != null)
            {
                // select 결과가 null이 아니면 로그인에 성공

                // 세션에 로그인 정보 저장
                List<string> userinfo = new List<string>(new string[] {
                    result.USERID,      // 아이디
                    result.NAME,        // 이름
                    result.NICKNAME,    // 닉네임
                    result.POWER        // 권한
                });

                Session["userinfo"] = userinfo;
                if (model.CookyId == "true")
                {
                    HttpCookie cookie = new HttpCookie("HWNovel")
                    {
                        Domain = "localhost",
                        Expires = DateTime.Now.AddDays(3)
                    };

                    cookie["id"] = Server.UrlEncode(id);

                    Response.Cookies.Add(cookie);
                }
                else
                {
                    Response.Cookies["HWNovel"].Expires = DateTime.Today.AddDays(-1);
                }

                if(model.PreUrl != null && model.PreUrl != "")
                {
                    Response.Redirect(model.PreUrl, false);
                    return null;
                }
                // 메인 홈 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                ViewBag.Id = id;
                ViewBag.PreUrl = model.PreUrl;
                ViewBag.loginError = "error";

                return RedirectToAction("LoginForm", "User", new { Userid = id, PreUrl = model.PreUrl, LoginError = "error" });
            }
        }

        public ActionResult Logout(User model)
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
                return RedirectToAction("LoginForm", "User");
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

            return RedirectToAction("LoginForm", "User");
        }
    }
}