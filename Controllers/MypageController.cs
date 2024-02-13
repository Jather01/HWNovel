using HWNovel.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
                List<HWN04> hwn04list = new List<HWN04>();
                List<HWN041> hwn041list = new List<HWN041>();

                List<Recent> rlist = new List<Recent>();

                string userid = userinfo[0];
                string today = DateTime.Now.ToString("yyyyMMdd");

                int listCount = 10;
                int pageNum = searchPage == 0 ? 1 : searchPage;
                int pageSize = 10;
                int totalCount = 0;

                using (var db = new HWNovelEntities())
                {
                    hwn012list = db.HWN012.Where(x => x.USERID.Equals(userid) && x.NOVELKIND.Equals("1")).ToList();
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

                    List< Recent> list2 = (from a in list
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

                    hwn012list = db.HWN012.Where(x => x.USERID.Equals(userid) && x.NOVELKIND.Equals("2")).ToList();
                    hwn04list = db.HWN04.ToList();
                    if (userinfo[3].Equals("1"))
                    {
                        hwn041list = db.HWN041.ToList();
                    }
                    else
                    {
                        hwn041list = db.HWN041.Where(x => x.OPENDT.CompareTo(today) <= 0).ToList();
                    }

                    var list3 = (from a in hwn012list
                                from b in hwn04list
                                from c in hwn041list
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

                    List<Recent> list4 = (from a in list3
                                          join b in hwn041list
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
                                          } into c
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

                    rlist = list2.Union(list4).ToList();

                    rlist = rlist.OrderByDescending(x => x.Date).ToList();

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

        public ActionResult DeletRecent()
        {

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 메인 홈 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                int pageNum = Int32.Parse(Request.Params["searchPage"] ?? "1");
                string novelid = Request.Params["novelid"];
                string novelkind = Request.Params["novelkind"];
                string userid = userinfo[0];

                if (!string.IsNullOrWhiteSpace(novelid))
                {
                    using (var db = new HWNovelEntities())
                    {
                        HWN012 rec = new HWN012();

                        rec = db.HWN012.Where(x => x.USERID.Equals(userid) && x.NOVELID.Equals(novelid) && x.NOVELKIND.Equals(novelkind)).SingleOrDefault();

                        db.HWN012.Remove(rec);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Rec", "Mypage", new { searchPage = pageNum });
            }
        }

        public ActionResult Wis()
        {
            ViewBag.topmenu = "Wis";
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
                List<HWN011> hwn011list = new List<HWN011>();
                List<HWN03> hwn03list = new List<HWN03>();
                List<HWN031> hwn031list = new List<HWN031>();
                List<HWN04> hwn04list = new List<HWN04>();
                List<HWN041> hwn041list = new List<HWN041>();

                List<Recent> rlist = new List<Recent>();

                string userid = userinfo[0];
                string today = DateTime.Now.ToString("yyyyMMdd");

                int listCount = 10;
                int pageNum = searchPage == 0 ? 1 : searchPage;
                int pageSize = 10;
                int totalCount = 0;

                using (var db = new HWNovelEntities())
                {
                    hwn011list = db.HWN011.Where(x => x.USERID.Equals(userid) && x.NOVELKIND.Equals("1")).ToList();
                    hwn03list = db.HWN03.ToList();
                    var hwn031list1 = new List<HWN031>();
                    if (userinfo[3].Equals("1"))
                    {
                        hwn031list1 = (from a in db.HWN031.ToList()
                                       group a by a.NOVELID into b
                                       select new HWN031
                                       {
                                           NOVELID = b.Key,
                                           VOLUMENO = b.Max(x => x.VOLUMENO),
                                           OPENDT = b.Max(x => x.OPENDT)
                                       }).ToList();
                    }
                    else
                    {
                        hwn031list1 = (from a in db.HWN031.ToList()
                                       where a.OPENDT.CompareTo(today) <= 0
                                       group a by a.NOVELID into b
                                       select new HWN031
                                       {
                                           NOVELID = b.Key,
                                           VOLUMENO = b.Max(x => x.VOLUMENO),
                                           OPENDT = b.Max(x => x.OPENDT)
                                       }).ToList();
                    }
                    hwn031list = (from a in db.HWN031.ToList()
                                  from b in hwn031list1
                                  where a.NOVELID == b.NOVELID
                                     && a.VOLUMENO == b.VOLUMENO
                                  select new HWN031
                                  {
                                      NOVELID = a.NOVELID,
                                      VOLUMENO = b.VOLUMENO,
                                      VOLUMTITLE = a.VOLUMTITLE,
                                      OPENDT = b.OPENDT
                                  }).ToList();

                    var rlist1 = (from a in hwn011list
                                  from b in hwn03list
                                  from c in hwn031list
                                  where a.NOVELID == b.NOVELID
                                     && a.NOVELID == c.NOVELID
                                  select new Recent
                                  {
                                      Userid = a.USERID,
                                      Novelid =a.NOVELID,
                                      Novelkind = a.NOVELKIND,
                                      Thumnail = b.THUMNAIL,
                                      Noveltitle = b.NOVELTITLE,
                                      Volumeno = c.VOLUMENO,
                                      Volumtitle = c.VOLUMTITLE,
                                      Opendt = c.OPENDT
                                  }).ToList();

                    hwn011list = db.HWN011.Where(x => x.USERID.Equals(userid) && x.NOVELKIND.Equals("2")).ToList();
                    hwn04list = db.HWN04.ToList();
                    var hwn041list1 = new List<HWN041>();
                    if (userinfo[3].Equals("1"))
                    {
                        hwn041list1 = (from a in db.HWN041.ToList()
                                       group a by a.NOVELID into b
                                       select new HWN041
                                       {
                                           NOVELID = b.Key,
                                           VOLUMENO = b.Max(x => x.VOLUMENO),
                                           OPENDT = b.Max(x => x.OPENDT)
                                       }).ToList();
                    }
                    else
                    {
                        hwn041list1 = (from a in db.HWN041.ToList()
                                       where a.OPENDT.CompareTo(today) <= 0
                                       group a by a.NOVELID into b
                                       select new HWN041
                                       {
                                           NOVELID = b.Key,
                                           VOLUMENO = b.Max(x => x.VOLUMENO),
                                           OPENDT = b.Max(x => x.OPENDT)
                                       }).ToList();
                    }
                    hwn041list = (from a in db.HWN041.ToList()
                                  from b in hwn041list1
                                  where a.NOVELID == b.NOVELID
                                     && a.VOLUMENO == b.VOLUMENO
                                  select new HWN041
                                  {
                                      NOVELID = a.NOVELID,
                                      VOLUMENO = b.VOLUMENO,
                                      VOLUMTITLE = a.VOLUMTITLE,
                                      OPENDT = b.OPENDT
                                  }).ToList();

                    var rlist2 = (from a in hwn011list
                                  from b in hwn04list
                                  from c in hwn041list
                                  where a.NOVELID == b.NOVELID
                                     && a.NOVELID == c.NOVELID
                                  select new Recent
                                  {
                                      Userid = a.USERID,
                                      Novelid = a.NOVELID,
                                      Novelkind = a.NOVELKIND,
                                      Thumnail = b.THUMNAIL,
                                      Noveltitle = b.NOVELTITLE,
                                      Volumeno = c.VOLUMENO,
                                      Volumtitle = c.VOLUMTITLE,
                                      Opendt = c.OPENDT
                                  }).ToList();

                    rlist = rlist1.Union(rlist2).ToList();

                    rlist = rlist.OrderByDescending(x => x.Date).ToList();

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
                ViewBag.NowDate = today;

                return View();
            }
        }

        public ActionResult DeletWish()
        {
            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 메인 홈 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                int pageNum = Int32.Parse(Request.Params["searchPage"] ?? "1");
                string novelid = Request.Params["novelid"];
                string novelkind = Request.Params["novelkind"];
                string userid = userinfo[0];

                if (!string.IsNullOrWhiteSpace(novelid))
                {
                    using (var db = new HWNovelEntities())
                    {
                        HWN011 rec = new HWN011();

                        rec = db.HWN011.Where(x => x.USERID.Equals(userid) && x.NOVELID.Equals(novelid) && x.NOVELKIND.Equals(novelkind)).SingleOrDefault();

                        db.HWN011.Remove(rec);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Wis", "Mypage", new { searchPage = pageNum });
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
                int listCount = 20;
                int pageNum = Int32.Parse(Request.Params["searchPage"] ?? "1");
                int pageSize = 10;
                int totalCount = 0;

                List<Novel> novelList = new List<Novel>();

                string nowDate = DateTime.Now.ToString("yyyyMMdd");
                string userid = userinfo[0];

                using (var db = new HWNovelEntities())
                {
                    List<HWN04> HWN04List = db.HWN04.Where(x => x.WRITER.Equals(userid)).ToList();

                    List<HWN01> HWN01List = db.HWN01.Where(x => x.USERID.Equals(userid)).ToList();

                    novelList = (from a in HWN04List
                                 join e in HWN01List
                                 on a.WRITER equals e.USERID into table4
                                 from e in table4.ToList()
                                 select new Novel
                                 {
                                     Novelid = a.NOVELID,
                                     Noveltitle = a.NOVELTITLE,
                                     Writer = e.NICKNAME,
                                     Genre = a.GENRE,
                                     Thumnail = a.THUMNAIL
                                 }).ToList();

                    novelList = novelList.OrderByDescending(x => x.Insertdt).ToList();

                    totalCount = novelList.Count;
                }

                novelList = novelList.Skip((pageNum - 1) * listCount)
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

                ViewBag.StartPageNum = startPageNum;
                ViewBag.EndPageNum = endPageNum;
                ViewBag.TotalCount = totalCount;
                ViewBag.ListCount = listCount;
                ViewBag.NowDate = nowDate;

                ViewBag.ChaList = novelList;

                ViewBag.searchPage = pageNum;

                return View();
            }
        }

        public ActionResult NovelWrite(Novel model)
        {
            ViewBag.topmenu = "none";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
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

                ViewBag.searchPage = model.searchPage;

                return View();
            }
        }

        [HttpPost]
        public ActionResult NovelWritePro(Novel model)
        {
            ViewBag.topmenu = "none";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                if (model != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.Noveltitle) && !string.IsNullOrWhiteSpace(model.Genre) && !string.IsNullOrWhiteSpace(model.Novelinfo) && !string.IsNullOrWhiteSpace(model.Thumnail))
                    {
                        using (var db = new HWNovelEntities())
                        {
                            string k = db.HWN04.Max(x => x.NOVELID) ?? "0";
                            k = (Int32.Parse(k) + 1).ToString().PadLeft(5, '0');

                            model.Novelid = k;
                            model.Writer = userinfo[0];

                            db.PRO_CHALLENGE_NOVEL_WRITE(model.Novelid, model.Noveltitle, model.Novelinfo, model.Writer, model.Genre, model.Thumnail);
                            db.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Cha", "Mypage", new { searchPage = model.searchPage });
            }
        }

        public ActionResult NovelUpdate()
        {
            ViewBag.topmenu = "none";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            string pageNum = Request.Params["searchPage"];
            string novelid = Request.Params["novelid"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                List<HWN021> genreList = new List<HWN021>();
                Novel novel = new Novel();

                using (var db = new HWNovelEntities())
                {
                    genreList = db.HWN021.Where(x => x.GROUPNO.Equals("03")).ToList();
                    var h04 = db.HWN04.ToList();

                    novel = (from a in h04
                             where a.NOVELID == novelid
                             select new Novel
                             {
                                 Novelid = a.NOVELID,
                                 Noveltitle = a.NOVELTITLE,
                                 Novelinfo = a.NOVELINFO,
                                 Writer = a.WRITER,
                                 Genre = a.GENRE,
                                 Thumnail = a.THUMNAIL,
                                 Endyn = a.ENDYN
                             }).SingleOrDefault();
                }

                ViewBag.GenreList = genreList;
                ViewBag.Novel = novel;

                ViewBag.searchPage = pageNum;

                return View();
            }
        }


        [HttpPost]
        public ActionResult NovelUpdatePro(Novel model)
        {
            ViewBag.topmenu = "none";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            int pageNum = model.searchPage;

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                if (model != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.Noveltitle) && !string.IsNullOrWhiteSpace(model.Genre) && !string.IsNullOrWhiteSpace(model.Novelinfo) && !string.IsNullOrWhiteSpace(model.Thumnail))
                    {
                        using (var db = new HWNovelEntities())
                        {
                            model.Writer = userinfo[0];
                            db.PRO_CHALLENGE_NOVEL_UPDATE(model.Novelid, model.Noveltitle, model.Novelinfo, model.Writer, model.Genre, model.Thumnail);
                            db.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Cha", "Mypage", new { searchPage = pageNum });
            }
        }

        public ActionResult NovelDelete()
        {
            ViewBag.topmenu = "none";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            string novelid = Request.Params["novelid"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                HWN04 h04 = new HWN04();
                List<HWN041> h041 = new List<HWN041>();

                using (var db = new HWNovelEntities())
                {
                    h04 = db.HWN04.Where(x => x.NOVELID.Equals(novelid)).SingleOrDefault();
                    db.HWN04.Remove(h04);

                    h041 = db.HWN041.Where(x => x.NOVELID.Equals(novelid)).ToList();
                    db.HWN041.RemoveRange(h041);
                    db.SaveChanges();
                }

                return RedirectToAction("Cha", "Mypage");
            }
        }

        public ActionResult NovelEnd()
        {
            ViewBag.topmenu = "none";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            string novelid = Request.Params["novelid"];
            string pageNum = Request.Params["pageNum"];
            string order = Request.Params["order"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                HWN04 h04 = new HWN04();
                List<HWN041> h041 = new List<HWN041>();

                using (var db = new HWNovelEntities())
                {
                    h04 = db.HWN04.Where(x => x.NOVELID.Equals(novelid)).SingleOrDefault();
                    h04.ENDYN = "2";
                    db.SaveChanges();
                }

                return RedirectToAction("Cha", "Mypage", new { novelid = novelid, pageNum = pageNum, order = order });
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