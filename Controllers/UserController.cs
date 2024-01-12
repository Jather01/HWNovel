using HWNovel.Models;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Web.Routing;
using System.Web.WebPages;

namespace HWNovel.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string id)
        {
            ViewBag.Id = id;
            return View();
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

        //[HttpPost]
        //public ActionResult FindId(User model)
        //{
        //    TempData["model"] = model;
        //    return RedirectToAction("FoundId", "User");
        //}

        //[HttpPost]
        //public ActionResult FoundId()
        //{
        //    User model = TempData["model"] as User;
        //    string id = model.Userid;
        //    string[] ids = null;
        //    if (id != null)
        //    {
        //        ids = id.Split(',');
        //    }
            
        //    ViewBag.ids = ids;
        //    ViewBag.name = model.Name;

        //    return View();
        //}

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
    }
}