using HWNovel.Models;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Data.Entity.Validation;

namespace HWNovel.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Login()
        {
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
            try
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
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
            return View(model);
        }

        public ActionResult FindId()
        {
            return View();
        }

        public ActionResult FindPw()
        {
            return View();
        }
    }
}