using HWNovel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace HWNovel.Controllers
{
    public class MypageController : Controller
    {
        public ActionResult Rec()
        {
            ViewBag.topmenu = "Rec";
            int searchPage = Int32.Parse(Request.Params["searchPage"] ?? "1");

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 로그인 화면으로 이동
                return RedirectToAction("LoginForm", "User", new { PreUrl = "/Mypage/Rec" });
            }
            else
            {
                List<HWN012> hwn012list = new List<HWN012>();
                List<HWN03> hwn03list = new List<HWN03>();
                List<HWN031> hwn031list = new List<HWN031>();

                List<Recent> rlist = new List<Recent>();

                string userid = userinfo[0];
                string today = DateTime.Now.ToString("yyyyMMdd");

                int listCount = 10;
                int pageNum = searchPage == 0 ? 1 : searchPage;
                int pageSize = 10;
                int totalCount = 0;

                using (var db = new HWNovelEntities())
                {
                    hwn012list = db.HWN012.Where(x => x.USERID.Equals(userid)).ToList();
                    hwn03list = db.HWN03.ToList();
                    if (userinfo[3].Equals("1"))
                    {
                        hwn031list = db.HWN031.ToList();
                    }
                    else
                    {
                        hwn031list = db.HWN031.Where(x => x.OPENDT.CompareTo(today) <= 0).ToList();
                    }

                    var list = (from a in hwn012list
                                from b in hwn03list
                                from c in hwn031list
                                where a.NOVELID == b.NOVELID
                                   && a.NOVELID == c.NOVELID
                                   && a.VOLUMENO == c.VOLUMENO
                                select new
                                {
                                    USERID = a.USERID,
                                    NOVELID = a.NOVELID,
                                    VOLUMENO = a.VOLUMENO,
                                    NOVELKIND = a.NOVELKIND,
                                    DATE = a.DATE,
                                    NOVELTITLE = b.NOVELTITLE,
                                    THUMNAIL = b.THUMNAIL,
                                    VOLUMTITLE = c.VOLUMTITLE
                                }
                                ).ToList();

                    rlist = (from a in list
                             join b in hwn031list
                             on a.NOVELID equals b.NOVELID
                             select new Recent
                             {
                                 Userid = a.USERID,
                                 Novelid = a.NOVELID,
                                 Noveltitle = a.NOVELTITLE,
                                 Thumnail = a.THUMNAIL,
                                 Volumeno = a.VOLUMENO,
                                 Nextvolumeno = b.VOLUMENO > a.VOLUMENO ? b.VOLUMENO : 999999,
                                 Volumtitle = a.VOLUMTITLE,
                                 Novelkind = a.NOVELKIND,
                                 Date = a.DATE
                             }into c
                             group c by new
                             {
                                 c.Userid,
                                 c.Novelid,
                                 c.Noveltitle,
                                 c.Thumnail,
                                 c.Volumeno,
                                 c.Volumtitle,
                                 c.Novelkind,
                                 c.Date
                             } into d
                             orderby d.Key.Date descending
                             select new Recent
                             {
                                 Userid = d.Key.Userid,
                                 Novelid = d.Key.Novelid,
                                 Noveltitle = d.Key.Noveltitle,
                                 Thumnail = d.Key.Thumnail,
                                 Volumeno = d.Key.Volumeno,
                                 Nextvolumeno = d.Min(x => x.Nextvolumeno),
                                 Volumtitle = d.Key.Volumtitle,
                                 Novelkind = d.Key.Novelkind,
                                 Date = d.Key.Date
                             }).ToList();

                    totalCount = rlist.Count;
                }

                rlist = rlist.Skip((pageNum - 1) * listCount)
                     .Take(listCount)
                     .ToList();

                //하단 시작 페이지 번호 
                int startPageNum = 1 + ((pageNum - 1) / pageSize) * pageSize;
                //하단 끝 페이지 번호
                int endPageNum = startPageNum + pageSize - 1;

                //전체 페이지의 갯수 구하기
                int totalPageCount = (int)Math.Ceiling(totalCount / (double)listCount);
                //끝 페이지 번호가 이미 전체 페이지 갯수보다 크게 계산되었다면 잘못된 값이다.
                if (endPageNum > totalPageCount)
                {
                    endPageNum = totalPageCount; //보정해 준다. 
                }

                foreach (var n in rlist)
                {
                    string imgPath = n.Thumnail;
                    string imgBase24 = "";

                    if (System.IO.File.Exists(imgPath))
                    {
                        byte[] b = System.IO.File.ReadAllBytes(imgPath);
                        imgBase24 = Convert.ToBase64String(b);
                    }

                    n.ThumbnailBase64 = imgBase24;
                }

                ViewBag.StartPageNum = startPageNum;
                ViewBag.EndPageNum = endPageNum;
                ViewBag.TotalCount = totalCount;
                ViewBag.ListCount = listCount;
                ViewBag.searchPage = pageNum;

                ViewBag.RecentList = rlist;

                return View();
            }
        }

        public ActionResult Wis()
        {
            ViewBag.topmenu = "Wis";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

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

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

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

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

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

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

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

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

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
                    User u = new User();
                    string userid = userinfo[0];
                    using (var db = new HWNovelEntities())
                    {
                        u = db.HWN01.Where(x => x.USERID.Equals(userid) && x.USEYN.Equals("1")).Select(x => new User { Userid = x.USERID, Name = x.NAME, Birthday = x.BIRTHDAY, Nickname = x.NICKNAME}).SingleOrDefault();
                    }

                    ViewBag.User = u;

                    return View();
                }
            }
        }

        [HttpPost]
        public ActionResult InfoUpdate(User user)
        {
            ViewBag.topmenu = "Min";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

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
                    string name = user.Name;
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

                    // 세션에 있는 로그인 정보 수정
                    userinfo[1] = name;
                    userinfo[2] = nickname;

                    Session["userinfo"] = userinfo;

                    return RedirectToAction("Min", "Mypage");
                }
            }
        }

        [HttpPost]
        public ActionResult PwUpdateForm(string pwYn)
        {
            ViewBag.topmenu = "Min";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

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

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

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

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

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

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

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