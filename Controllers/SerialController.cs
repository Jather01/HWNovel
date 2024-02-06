using HWNovel.ViewModels;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using System.Web.UI.WebControls;

namespace HWNovel.Controllers
{
    public class SerialController : Controller
    {
        public ActionResult Day(Novel model)
        {
            ViewBag.topmenu = "Day";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            DayOfWeek today = DateTime.Today.DayOfWeek;
            int todayint = (int)today;

            string searchDay = model.searchDay ?? todayint.ToString();
            string searchGenre = model.searchGenre ?? "all";
            string searchOrder = model.searchOrder ?? "view";

            int listCount = 20;
            int pageNum = 1;
            int pageSize = 10;
            int totalCount = 0;

            if (model.searchPage != 0)
            {
                pageNum = model.searchPage;
            }

            List<HWN021> genreList = new List<HWN021>();
            List<Novel> novelList = new List<Novel>();

            string nowDate = DateTime.Now.ToString("yyyyMMdd");

            using (var db = new HWNovelEntities())
            {
                genreList = db.HWN021.Where(x => x.GROUPNO.Equals("03")).OrderBy(x => x.CODENO).ToList();

                List<HWN03> hwn03List = db.HWN03.ToList();

                List<HWN031> hwn031List = (from a in db.HWN031.ToList()
                                           where a.OPENDT.CompareTo(nowDate) <= 0
                                           group a by new
                                           {
                                               a.NOVELID
                                           } into b
                                           select new HWN031
                                           {
                                               NOVELID = b.Key.NOVELID,
                                               VOLUMENO = b.Count(),
                                               OPENDT = b.Max(x => x.OPENDT),
                                               VIEWCNT = b.Sum(x => x.VIEWCNT)
                                           }).ToList();

                List<HWN033> hwn033List = (from a in db.HWN033.ToList()
                                           from b in (from c in db.HWN031.ToList()
                                                      where c.OPENDT.CompareTo(nowDate) <= 0
                                                      select new { NOVELID = c.NOVELID, VOLUMENO = c.VOLUMENO })
                                           where a.NOVELID == b.NOVELID
                                              && a.VOLUMENO == b.VOLUMENO
                                           group a by new
                                           {
                                               a.NOVELID
                                           } into d
                                           select new HWN033
                                           {
                                               NOVELID = d.Key.NOVELID,
                                               STARPOINT = d.Average(x => x.STARPOINT)
                                           }).ToList();

                var hwn011List = (from a in db.HWN011.ToList()
                                  group a by new
                                  {
                                      a.NOVELID
                                  } into b
                                  select new
                                  {
                                      NOVELID = b.Key.NOVELID,
                                      FAVORITECNT = b.Count()
                                  }).ToList();


                novelList = (from a in hwn03List
                             join b in hwn031List
                             on a.NOVELID equals b.NOVELID into table1
                             from b in table1.ToList().DefaultIfEmpty()
                             join c in hwn033List
                             on a.NOVELID equals c.NOVELID into table2
                             from c in table2.ToList().DefaultIfEmpty()
                             join d in hwn011List
                             on a.NOVELID equals d.NOVELID into table3
                             from d in table3.ToList().DefaultIfEmpty()
                             select new Novel
                             {
                                 Novelid = a.NOVELID,
                                 Noveltitle = a.NOVELTITLE,
                                 Writer = a.WRITER,
                                 Genre = a.GENRE,
                                 Thumnail = a.THUMNAIL,
                                 Mon = a.MON,
                                 Tue = a.TUE,
                                 Wed = a.WED,
                                 Thu = a.THU,
                                 Fri = a.FRI,
                                 Sat = a.SAT,
                                 Sun = a.SUN,
                                 Opendt = b.OPENDT,
                                 Viewcnt = b?.VIEWCNT ?? 0,
                                 Volumecnt = Decimal.ToInt32(b?.VOLUMENO ?? 0),
                                 StarPointAvg = Math.Round(c?.STARPOINT ?? 0, 2),
                                 Favoritecnt = d?.FAVORITECNT ?? 0

                             }).ToList();

                if (!string.IsNullOrWhiteSpace(model.searchGenre))
                {
                    if (!model.searchGenre.Equals("all"))
                        novelList = novelList.Where(x => x.Genre.Equals(model.searchGenre)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(searchDay))
                {
                    if (!searchDay.Equals("7"))
                    {
                        switch (searchDay)
                        {
                            case "1":
                                novelList = novelList.Where(x => x.Mon.Equals("1")).ToList();
                                break;

                            case "2":
                                novelList = novelList.Where(x => x.Tue.Equals("1")).ToList();
                                break;

                            case "3":
                                novelList = novelList.Where(x => x.Wed.Equals("1")).ToList();
                                break;

                            case "4":
                                novelList = novelList.Where(x => x.Thu.Equals("1")).ToList();
                                break;

                            case "5":
                                novelList = novelList.Where(x => x.Fri.Equals("1")).ToList();
                                break;

                            case "6":
                                novelList = novelList.Where(x => x.Sat.Equals("1")).ToList();
                                break;

                            case "0":
                                novelList = novelList.Where(x => x.Sun.Equals("1")).ToList();
                                break;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.searchOrder))
                {
                    switch (model.searchOrder)
                    {
                        case "view":
                            novelList = novelList.OrderByDescending(x => x.Viewcnt).ToList();
                            break;

                        case "star":
                            novelList = novelList.OrderByDescending(x => x.StarPointAvg).ToList();
                            break;

                        case "title":
                            novelList = novelList.OrderBy(x => x.Noveltitle).ToList();
                            break;

                        case "favorite":
                            novelList = novelList.OrderByDescending(x => x.Favoritecnt).ToList();
                            break;
                    }
                }

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

            foreach (var n in novelList)
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
            ViewBag.NowDate = nowDate;

            ViewBag.NovelList = novelList;
            ViewBag.GenreList = genreList;

            ViewBag.searchDay = searchDay;
            ViewBag.searchPage = pageNum;
            ViewBag.searchGenre = searchGenre;
            ViewBag.searchOrder = searchOrder;

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

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            return View();
        }

        public ActionResult Fin()
        {
            ViewBag.topmenu = "Fin";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            return View();
        }

        public ActionResult NovelManage(Novel model)
        {
            ViewBag.topmenu = "NovelManage";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

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

                    int listCount = 20;
                    int pageNum = 1;
                    int pageSize = 10;
                    int totalCount = 0;

                    if (model.searchPage != 0)
                    {
                        pageNum = model.searchPage;
                    }

                    List<HWN021> genreList = new List<HWN021>();
                    List<Novel> novelList = new List<Novel>();

                    using (var db = new HWNovelEntities())
                    {
                        genreList = db.HWN021.Where(x => x.GROUPNO.Equals("03")).OrderBy(x => x.CODENO).ToList();

                        List<HWN03> hwn03List = db.HWN03.ToList();
                        List<HWN031> hwn031List = db.HWN031.ToList();

                        novelList = (from h03 in hwn03List
                                   join h031 in hwn031List
                                   on h03.NOVELID equals h031.NOVELID into c
                                   from d in c.DefaultIfEmpty()
                                   select new
                                   {
                                       NOVELID = h03.NOVELID,
                                       NOVELTITLE = h03.NOVELTITLE,
                                       WRITER = h03.WRITER,
                                       GENRE = h03.GENRE,
                                       THUMNAIL = h03.THUMNAIL,
                                       OPENDT = d?.OPENDT ?? string.Empty
                                   } into e
                                   group e by new
                                   {
                                       e.NOVELID,
                                       e.NOVELTITLE,
                                       e.WRITER,
                                       e.GENRE,
                                       e.THUMNAIL
                                   } into f
                                   select new
                                   {
                                       NOVELID = f.Key.NOVELID,
                                       NOVELTITLE = f.Key.NOVELTITLE,
                                       WRITER = f.Key.WRITER,
                                       GENRE = f.Key.GENRE,
                                       THUMNAIL = f.Key.THUMNAIL,
                                       OPENDT = f.Select(o => o.OPENDT).Min()
                                   } into g
                                   where g.OPENDT.Equals(string.Empty) || g.OPENDT.CompareTo(DateTime.Now.ToString("yyyyMMdd")) > 0
                                   select new Novel
                                   {
                                       Novelid = g.NOVELID,
                                       Noveltitle = g.NOVELTITLE,
                                       Writer = g.WRITER,
                                       Genre = g.GENRE,
                                       Thumnail = g.THUMNAIL,
                                       Opendt = g.OPENDT
                                   }).ToList();

                        if (!string.IsNullOrWhiteSpace(model.searchValue))
                        {
                            novelList = novelList.Where(x => x.Writer.IndexOf(model.searchValue) >= 0 || x.Noveltitle.IndexOf(model.searchValue) >= 0).ToList();
                        }
                        if (!string.IsNullOrWhiteSpace(model.searchGenre))
                        {
                            if(!model.searchGenre.Equals("all"))
                            novelList = novelList.Where(x => x.Genre.Equals(model.searchGenre)).ToList();
                        }

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

                    foreach(var n in novelList)
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

                    ViewBag.NovelList = novelList;
                    ViewBag.GenreList = genreList;

                    ViewBag.searchValue = model.searchValue;
                    ViewBag.searchPage = pageNum;
                    ViewBag.searchGenre = model.searchGenre;

                    return View();
                }
            }
        }

        public ActionResult NovelWrite(Novel model)
        {
            ViewBag.topmenu = "NovelManage";

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

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

                    ViewBag.searchValue = model.searchValue;
                    ViewBag.searchPage = model.searchPage;
                    ViewBag.searchGenre = model.searchGenre;

                    return View();
                }
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
                if (userinfo[3] != "1")
                {
                    // 로그인 정보가 있는데 관리자 계정이 아니면 공지사항 목록 화면으로 이동
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    if (model != null)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Noveltitle) && !string.IsNullOrWhiteSpace(model.Genre) && !string.IsNullOrWhiteSpace(model.Novelinfo) && model.ThumbnailFile != null)
                        {
                            model.Mon = model.Mon != "1" ? "0" : model.Mon;
                            model.Tue = model.Tue != "1" ? "0" : model.Tue;
                            model.Wed = model.Wed != "1" ? "0" : model.Wed;
                            model.Thu = model.Thu != "1" ? "0" : model.Thu;
                            model.Fri = model.Fri != "1" ? "0" : model.Fri;
                            model.Sat = model.Sat != "1" ? "0" : model.Sat;
                            model.Sun = model.Sun != "1" ? "0" : model.Sun;

                            HttpPostedFileBase thumbnailFile = model.ThumbnailFile;

                            if(thumbnailFile.ContentLength > 0)
                            {
                                string k = "";

                                using (var db = new HWNovelEntities())
                                {
                                    k = db.HWN03.Max(x => x.NOVELID) ?? "0";
                                    k = (Int32.Parse(k) + 1).ToString().PadLeft(5,'0');

                                    string thumbnailPath = "E:\\HWNovel\\Thumnail\\";
                                    DirectoryInfo di = new DirectoryInfo(thumbnailPath);

                                    if (di.Exists == false) di.Create();

                                    string fileFullPath = thumbnailPath + k + ".jpg";
                                    
                                    thumbnailFile.SaveAs(fileFullPath);

                                    model.Novelid = k;
                                    model.Thumnail = fileFullPath;

                                    db.PRO_NOVEL_WRITE(model.Novelid, model.Noveltitle, model.Novelinfo, model.Writer, model.Genre, model.Thumnail,
                                        model.Mon, model.Tue, model.Wed, model.Thu, model.Fri, model.Sat, model.Sun);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    return RedirectToAction("NovelManage", "Serial", new { searchValue = model.searchValue, searchGenre = model.searchGenre, searchPage = model.searchPage });
                }
            }
        }

        public ActionResult NovelInfo()
        {
            string topmenu = "none";
            string novelId = Request.Params["novelid"];

            List<string> userinfo = (List<string>)Session["userinfo"];
            string userid = "";
            string userPower = "";
            if (userinfo != null) {
                userid = userinfo[0];
                userPower = userinfo[3];
            }

            ViewBag.topmenu = topmenu;
            ViewBag.userinfo = userinfo;

            int listCount = 20;
            int pageNum = 1;
            int pageSize = 10;
            int totalCount = 0;
            string searchPage = Request.Params["pageNum"] ?? "0";
            string order = Request.Params["order"] ?? "update";

            if (Int32.Parse(searchPage) != 0)
            {
                pageNum = Int32.Parse(searchPage);
            }

            HWN011 u = new HWN011();
            HWN021 genreCode = new HWN021();
            Novel n = new Novel();
            List<HWN031> nList = new List<HWN031>();
            List<Novel> novleList = new List<Novel>();
            List<HWN011> favorits = new List<HWN011>();
            int favorit = 0;
            string nowDate = DateTime.Now.ToString("yyyyMMdd");

            using (var db = new HWNovelEntities())
            {
                var n03 = db.HWN03.Where(x => x.NOVELID.Equals(novelId)).ToList();

                var hwn033List = (from a in db.HWN033.ToList()
                                  from b in (from c in db.HWN031.ToList()
                                              where c.OPENDT.CompareTo(nowDate) <= 0
                                              select new { NOVELID = c.NOVELID, VOLUMENO = c.VOLUMENO })
                                  where a.NOVELID == b.NOVELID
                                      && a.VOLUMENO == b.VOLUMENO
                                  group a by new
                                  {
                                      a.NOVELID
                                  } into d
                                  select new HWN033
                                  {
                                      NOVELID = d.Key.NOVELID,
                                      STARPOINT = d.Average(x => x.STARPOINT)
                                  }).ToList();

                n = (from a in n03
                     join b in hwn033List
                     on a.NOVELID equals b.NOVELID into table2
                     from b in table2.ToList().DefaultIfEmpty()
                     select new Novel
                     {
                         Novelid = a.NOVELID,
                         Noveltitle = a.NOVELTITLE,
                         Novelinfo = a.NOVELINFO,
                         Writer = a.WRITER,
                         searchGenre = a.GENRE,
                         Thumnail = a.THUMNAIL,
                         Mon = a.MON,
                         Tue = a.TUE,
                         Wed = a.WED,
                         Thu = a.THU,
                         Fri = a.FRI,
                         Sat = a.SAT,
                         Sun = a.SUN,
                         StarPointAvg = Math.Round(b?.STARPOINT ?? 0, 2)
                     }).SingleOrDefault();

                genreCode = db.HWN021.Where(x => x.GROUPNO.Equals("03") && x.CODENO.Equals(n.searchGenre)).SingleOrDefault();

                n.Genre = genreCode.CODENAME;

                if (!string.IsNullOrWhiteSpace(userid))
                {
                    List<HWN011> list = db.HWN011.ToList();
                    u = (from a in list
                        where a.NOVELID.Equals(novelId) && a.USERID.Equals(userid) && a.NOVELKIND.Equals("1")
                        select new HWN011 {
                            USERID = a.USERID,
                            NOVELID = a.NOVELID
                        }).SingleOrDefault();
                }

                if (userPower.Equals("1"))
                {
                    nList = db.HWN031.Where(x => x.NOVELID.Equals(novelId)).ToList();
                }
                else
                {
                    string nowdate = DateTime.Now.ToString("yyyyMMdd");
                    nList = db.HWN031.Where(x => x.NOVELID.Equals(novelId) && x.OPENDT.CompareTo(nowdate) <= 0).ToList();
                }

                var n032List = db.HWN032.Where(x => x.NOVELID.Equals(novelId))
                                        .GroupBy(x => new { x.NOVELID, x.VOLUMENO })
                                        .Select(x => new
                                        {
                                            NOVELID = x.Key.NOVELID,
                                            VOLUMENO = x.Key.VOLUMENO,
                                            COMMENTCNT = x.Count()
                                        })
                                        .ToList();

                var n033List = db.HWN033.Where(x => x.NOVELID.Equals(novelId))
                                        .GroupBy(x => new { x.NOVELID, x.VOLUMENO })
                                        .Select(x => new
                                        {
                                            NOVELID = x.Key.NOVELID,
                                            VOLUMENO = x.Key.VOLUMENO,
                                            STARPOINTAVG = x.Average(a => a.STARPOINT)
                                        })
                                        .ToList();

                novleList = (from a in nList
                                join b in n032List
                                on new { a.NOVELID, a.VOLUMENO } equals new { b.NOVELID, b.VOLUMENO } into c
                                from d in c.DefaultIfEmpty()
                                select new
                                {
                                    NOVELID = a.NOVELID,
                                    VOLUMENO = a.VOLUMENO,
                                    VOLUMTITLE = a.VOLUMTITLE,
                                    OPENDT = a.OPENDT,
                                    COMMENTCNT = d?.COMMENTCNT ?? 0
                                } into e
                                join g in n033List
                                on new { e.NOVELID, e.VOLUMENO } equals new { g.NOVELID, g.VOLUMENO } into h
                                from i in h.DefaultIfEmpty()
                                select new Novel
                                {
                                    Novelid = e.NOVELID,
                                    Volumeno = e.VOLUMENO,
                                    Volumtitle = e.VOLUMTITLE,
                                    Opendt = e.OPENDT,
                                    Commentcnt = e.COMMENTCNT,
                                    StarPointAvg = Math.Round(i?.STARPOINTAVG ?? 0, 2)
                                })
                                .ToList();

                favorits = db.HWN011.Where(x => x.NOVELID.Equals(novelId) && x.NOVELKIND.Equals("1")).ToList();
                favorit = favorits.Count;
            }

            if(n == null)
            {
                return RedirectToAction("NovelInfo", "Serial", new { novelid = novelId, pageNum = searchPage, order = order });
            }

            if (order.Equals("update"))
            {
                novleList = novleList.OrderByDescending(x => x.Volumeno).ToList();
            }
            else if (order.Equals("oldest"))
            {
                novleList = novleList.OrderBy(x => x.Volumeno).ToList();
            }

            string imgPath = n.Thumnail;
            string imgBase24 = "";

            if (System.IO.File.Exists(imgPath))
            {
                byte[] b = System.IO.File.ReadAllBytes(imgPath);
                imgBase24 = Convert.ToBase64String(b);
            }

            n.ThumbnailBase64 = imgBase24;

            totalCount = novleList.Count;

            novleList = novleList.Skip((pageNum - 1) * listCount)
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

            // 검색어
            ViewBag.novelid = novelId;
            ViewBag.topmenu = topmenu;
            ViewBag.pageNum = pageNum;
            ViewBag.order = order;

            ViewBag.StartPageNum = startPageNum;
            ViewBag.EndPageNum = endPageNum;
            ViewBag.TotalCount = totalCount;
            ViewBag.ListCount = listCount;

            ViewBag.Novel = n;
            ViewBag.UserFavorite = u;
            ViewBag.NList = novleList;
            ViewBag.NCount = totalCount;
            ViewBag.FavoritCnt = favorit;

            return View();
        }

        public ActionResult NovelDelete()
        {
            ViewBag.topmenu = "none";
            ViewBag.userinfo = Session["userinfo"];

            string novelid = Request.Params["novelid"];

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
                    HWN03 h03 = new HWN03();
                    List<HWN031> h031 = new List<HWN031>();

                    using (var db = new HWNovelEntities())
                    {
                        h03 = db.HWN03.Where(x => x.NOVELID.Equals(novelid)).SingleOrDefault();
                        db.HWN03.Remove(h03);

                        h031 = db.HWN031.Where(x => x.NOVELID.Equals(novelid)).ToList();
                        db.HWN031.RemoveRange(h031);
                        db.SaveChanges();
                    }

                    return RedirectToAction("Day", "Serial");
                }
            }
        }

        public ActionResult NovelEnd()
        {
            ViewBag.topmenu = "none";
            ViewBag.userinfo = Session["userinfo"];

            string novelid = Request.Params["novelid"];
            string pageNum = Request.Params["pageNum"];
            string order = Request.Params["order"];

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
                    HWN03 h03 = new HWN03();
                    List<HWN031> h031 = new List<HWN031>();

                    using (var db = new HWNovelEntities())
                    {
                        h03 = db.HWN03.Where(x => x.NOVELID.Equals(novelid)).SingleOrDefault();
                        h03.ENDYN = "2";
                        db.SaveChanges();
                    }

                    return RedirectToAction("NovelInfo", "Serial", new { novelid = novelid, pageNum = pageNum, order = order });
                }
            }
        }

        public ActionResult NovelUpdate()
        {
            ViewBag.topmenu = "none";
            ViewBag.userinfo = Session["userinfo"];

            string novelid = Request.Params["novelid"];
            string pageNum = Request.Params["pageNum"];
            string order = Request.Params["order"];

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
                    Novel novel = new Novel();

                    using (var db = new HWNovelEntities())
                    {
                        genreList = db.HWN021.Where(x => x.GROUPNO.Equals("03")).ToList();
                        var h03 = db.HWN03.ToList();

                        novel = (from a in h03
                                 where a.NOVELID == novelid
                                 select new Novel
                                 {
                                     Novelid = a.NOVELID,
                                     Noveltitle = a.NOVELTITLE,
                                     Novelinfo = a.NOVELINFO,
                                     Writer = a.WRITER,
                                     Genre = a.GENRE,
                                     Thumnail = a.THUMNAIL,
                                     Endyn = a.ENDYN,
                                     Mon = a.MON,
                                     Tue = a.TUE,
                                     Wed = a.WED,
                                     Thu = a.THU,
                                     Fri = a.FRI,
                                     Sat = a.SAT,
                                     Sun = a.SUN
                                 }).SingleOrDefault();
                    }

                    string imgPath = novel.Thumnail;
                    string imgBase24 = "";

                    if (System.IO.File.Exists(imgPath))
                    {
                        byte[] b = System.IO.File.ReadAllBytes(imgPath);
                        imgBase24 = Convert.ToBase64String(b);
                    }

                    novel.ThumbnailBase64 = imgBase24;

                    ViewBag.GenreList = genreList;
                    ViewBag.Novel = novel;

                    ViewBag.novelid = novelid;
                    ViewBag.pageNum = pageNum;
                    ViewBag.order = order;

                    return View();
                }
            }
        }


        [HttpPost]
        public ActionResult NovelUpdatePro(Novel model)
        {
            ViewBag.topmenu = "none";
            ViewBag.userinfo = Session["userinfo"];

            string novelid = model.Novelid;
            int pageNum = model.searchPage;
            string order = model.searchOrder;

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
                    if (model != null)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Noveltitle) && !string.IsNullOrWhiteSpace(model.Genre) && !string.IsNullOrWhiteSpace(model.Novelinfo))
                        {
                            model.Mon = model.Mon != "1" ? "0" : model.Mon;
                            model.Tue = model.Tue != "1" ? "0" : model.Tue;
                            model.Wed = model.Wed != "1" ? "0" : model.Wed;
                            model.Thu = model.Thu != "1" ? "0" : model.Thu;
                            model.Fri = model.Fri != "1" ? "0" : model.Fri;
                            model.Sat = model.Sat != "1" ? "0" : model.Sat;
                            model.Sun = model.Sun != "1" ? "0" : model.Sun;

                            HttpPostedFileBase thumbnailFile = model.ThumbnailFile;

                            if (thumbnailFile != null && thumbnailFile.ContentLength > 0)
                            {
                                if(System.IO.File.Exists(model.Thumnail)) System.IO.File.Delete(model.Thumnail);

                                string thumbnailPath = "E:\\HWNovel\\Thumnail\\";
                                DirectoryInfo di = new DirectoryInfo(thumbnailPath);

                                if (di.Exists == false) di.Create();

                                string fileFullPath = thumbnailPath + model.Novelid + ".jpg";

                                thumbnailFile.SaveAs(fileFullPath);

                                model.Thumnail = fileFullPath;
                            }

                            using (var db = new HWNovelEntities())
                            {
                                db.PRO_NOVEL_UPDATE(model.Novelid, model.Noveltitle, model.Novelinfo, model.Writer, model.Genre, model.Thumnail,
                                    model.Mon, model.Tue, model.Wed, model.Thu, model.Fri, model.Sat, model.Sun);
                                db.SaveChanges();
                            }
                        }
                    }

                    return RedirectToAction("NovelInfo", "Serial", new { novelid = novelid, pageNum = pageNum, order = order });
                }
            }
        }

        public ActionResult VolumeWrite()
        {
            ViewBag.topmenu = "none";
            ViewBag.userinfo = Session["userinfo"];

            string novelid = Request.Params["novelid"];
            string pageNum = Request.Params["pageNum"];
            string order = Request.Params["order"];

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
                    ViewBag.novelid = novelid;
                    ViewBag.pageNum = pageNum;
                    ViewBag.order = order;

                    return View();
                }
            }
        }

        [HttpPost]
        public ActionResult VolumeWritePro(Novel model)
        {
            ViewBag.topmenu = "none";
            ViewBag.userinfo = Session["userinfo"];

            string novelid = model.Novelid;
            int pageNum = model.searchPage;
            string order = model.searchOrder;

            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 메인 홈 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                if (userinfo[3] != "1")
                {
                    // 로그인 정보가 있는데 관리자 계정이 아니면 메인 홈 화면으로 이동
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    if (model != null)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Volumtitle) && !string.IsNullOrWhiteSpace(model.Noveltext) && !string.IsNullOrWhiteSpace(model.Opendt))
                        {
                            string opendt = model.Opendt.Replace(".", "");
                            string writerComment = model.Writercomment ?? ".";
                            using (var db = new HWNovelEntities())
                            {
                                db.PRO_VOLUME_WRITE(model.Novelid, model.Volumtitle, model.Noveltext, writerComment, opendt);
                                db.SaveChanges();
                            }
                        }
                    }

                    return RedirectToAction("NovelInfo", "Serial", new { novelid = novelid, pageNum = pageNum, order = order });
                }
            }
        }

        public ActionResult NovelView()
        {
            string topmenu = "none";
            string novelId = Request.Params["novelid"];
            string volumeNo = Request.Params["volumeno"];
            int volumeNoNum = Int32.Parse(volumeNo);
            string searchPage = Request.Params["pageNum"] ?? "0";
            string order = Request.Params["order"] ?? "update";

            List<string> userinfo = (List<string>)Session["userinfo"];
            string userid = "";
            string userPower = "";
            if (userinfo != null)
            {
                userid = userinfo[0];
                userPower = userinfo[3];
            }

            ViewBag.topmenu = topmenu;
            ViewBag.userinfo = userinfo;

            Novel n = new Novel();
            Novel nextN = new Novel();
            Novel prevN = new Novel();
            List<Novel> nList = new List<Novel>();
            int starYn = 0;
            decimal[] spText = { 0, 0 };

            using (var db = new HWNovelEntities())
            {
                n = (from a in db.HWN031
                     from b in db.HWN03
                     where (a.NOVELID == novelId
                         && a.NOVELID == b.NOVELID
                         && a.VOLUMENO.ToString() == volumeNo
                         )
                     select new Novel
                     {
                         Novelid = a.NOVELID,
                         Noveltitle = b.NOVELTITLE,
                         Volumtitle = a.VOLUMTITLE,
                         Noveltext = a.NOVELTEXT,
                         Volumeno = a.VOLUMENO,
                         Writercomment = a.WRITERCOMMENT,
                         Writer = b.WRITER,
                         Opendt = a.OPENDT,
                         Thumnail = b.THUMNAIL
                     }).SingleOrDefault();

                nList = (from a in db.HWN031
                         where (a.NOVELID == novelId)
                         orderby a.VOLUMENO descending
                         select new Novel
                         {
                             Novelid = a.NOVELID,
                             Volumtitle = a.VOLUMTITLE,
                             Volumeno = a.VOLUMENO,
                             Opendt = a.OPENDT
                         }).ToList();

                if (n == null)
                {
                    return RedirectToAction("NovelInfo", "Serial", new { novelid = novelId, pageNum = searchPage, order = order });
                }

                string nowdate = DateTime.Now.ToString("yyyyMMdd");

                if (!userPower.Equals("1"))
                {
                    if (n.Opendt.CompareTo(nowdate) > 0)
                    {
                        return RedirectToAction("NovelInfo", "Serial", new { novelid = novelId, pageNum = searchPage, order = order });
                    }

                    nList = nList.Where(x => x.Opendt.CompareTo(nowdate) <= 0).ToList();
                }

                nextN = (from a in nList
                         where (a.Volumeno > volumeNoNum)
                         group a by a.Novelid into b
                         select new
                         {
                             Novelid = b.Key,
                             Volumeno = b.Min(x => x.Volumeno)
                         } into c
                         from d in db.HWN031
                         where (c.Novelid == d.NOVELID &&
                             c.Volumeno == d.VOLUMENO)
                         select new Novel
                         {
                             Novelid = d.NOVELID,
                             Volumeno = d.VOLUMENO,
                             Opendt = d.OPENDT
                         }).SingleOrDefault();

                prevN = (from a in nList
                         where (a.Volumeno < volumeNoNum)
                         group a by a.Novelid into b
                         select new
                         {
                             Novelid = b.Key,
                             Volumeno = b.Max(x => x.Volumeno)
                         } into c
                         from d in db.HWN031
                         where (c.Novelid == d.NOVELID &&
                             c.Volumeno == d.VOLUMENO)
                         select new Novel
                         {
                             Novelid = d.NOVELID,
                             Volumeno = d.VOLUMENO,
                             Opendt = d.OPENDT
                         }).SingleOrDefault();

                if (userinfo != null)
                {
                    db.PRO_USER_RECENT_NOVEL(userid, n.Novelid, n.Volumeno, "1");

                    starYn = db.HWN033.Where(x => x.NOVELID.Equals(novelId) && x.VOLUMENO.ToString().Equals(volumeNo) && x.USERID.Equals(userid)).ToList().Count;
                }

                db.PRO_VIEWCNT_UPDATE(n.Novelid, n.Volumeno);
                db.SaveChanges();

                Novel spResult = db.HWN033.Where(x => x.NOVELID.Equals(novelId) && x.VOLUMENO.ToString().Equals(volumeNo))
                                        .GroupBy(x => new { x.NOVELID, x.VOLUMENO })
                                        .Select(x => new Novel
                                        {
                                            Novelid = x.Key.NOVELID,
                                            Volumeno = x.Key.VOLUMENO,
                                            StarPointAvg = x.Average(a => a.STARPOINT),
                                            Commentcnt = x.Count()
                                        }).SingleOrDefault();

                if(spResult != null)
                {
                    spText[0] = spResult.Commentcnt;
                    spText[1] = Math.Round(spResult.StarPointAvg, 2);
                }
            }

            string imgPath = n.Thumnail;
            string imgBase24 = "";

            if (System.IO.File.Exists(imgPath))
            {
                byte[] b = System.IO.File.ReadAllBytes(imgPath);
                imgBase24 = Convert.ToBase64String(b);
            }

            n.ThumbnailBase64 = imgBase24;

            // 검색어
            ViewBag.novelid = novelId;
            ViewBag.topmenu = topmenu;
            ViewBag.pageNum = searchPage;
            ViewBag.order = order;

            ViewBag.Novel = n;
            ViewBag.NList = nList;
            ViewBag.StarYn = starYn;
            ViewBag.SpText = spText;
            ViewBag.NextN = nextN;
            ViewBag.PrevN = prevN;

            return View();
        }

        public ActionResult CommentInclude(Comment model)
        {
            List<string> userinfo = (List<string>)Session["userinfo"];

            int listCount = 10;
            int pageNum = 1;
            int pageSize = 10;
            int totalCount = 0;

            if (model.searchPage != 0)
            {
                pageNum = model.searchPage;
            }

            List<Comment> cList = new List<Comment>();

            if (!string.IsNullOrWhiteSpace(model.Novelid))
            {
                using (var db = new HWNovelEntities())
                {
                    cList = (from a in db.HWN032
                             from b in db.HWN01
                             where (a.NOVELID == model.Novelid
                                 && a.VOLUMENO == model.Volumeno
                                 && a.USERID == b.USERID)
                             orderby a.USERCOMMENTNO descending
                             select new Comment
                             {
                                 Novelid = a.NOVELID,
                                 Volumeno = a.VOLUMENO,
                                 Usercommentno = a.USERCOMMENTNO,
                                 Usercomment = a.USERCOMMENT,
                                 Userid = a.USERID,
                                 Nickname = b.NICKNAME,
                                 Insertdt = a.INSERTDT
                             }).ToList();
                    totalCount = cList.Count;
                }
            }

            cList = cList.Skip((pageNum - 1) * listCount)
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
            ViewBag.searchPage = pageNum;

            ViewBag.cList = cList;
            ViewBag.cCount = totalCount;
            ViewBag.userinfo = userinfo;
            ViewBag.novelid = model.Novelid;
            ViewBag.Volumeno = model.Volumeno;

            return View();
        }

        public ActionResult VolumeUpdate()
        {
            ViewBag.topmenu = "none";

            string novelid = Request.Params["novelid"];
            string volumeno = Request.Params["volumeno"];

            string pageNum = Request.Params["pageNum"];
            string order = Request.Params["order"];

            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 메인 홈 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                if (userinfo[3] != "1")
                {
                    // 로그인 정보가 있는데 관리자 계정이 아니면 메인 홈 화면으로 이동
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    HWN031 n = new HWN031();

                    using (var db = new HWNovelEntities())
                    {
                        n = db.HWN031.Where(x => x.NOVELID.Equals(novelid) && x.VOLUMENO.ToString().Equals(volumeno)).SingleOrDefault();
                    }

                    ViewBag.Novel = n;

                    ViewBag.novelid = novelid;
                    ViewBag.volumeno = volumeno;
                    ViewBag.pageNum = pageNum;
                    ViewBag.order = order;

                    return View();
                }
            }
        }

        [HttpPost]
        public ActionResult VolumeUpdatePro(Novel model)
        {
            ViewBag.topmenu = "none";
            ViewBag.userinfo = Session["userinfo"];

            string novelid = model.Novelid;
            int pageNum = model.searchPage;
            string order = model.searchOrder;

            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 메인 홈 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {
                if (userinfo[3] != "1")
                {
                    // 로그인 정보가 있는데 관리자 계정이 아니면 메인 홈 화면으로 이동
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    if (model != null)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Volumtitle) && !string.IsNullOrWhiteSpace(model.Noveltext) && !string.IsNullOrWhiteSpace(model.Opendt))
                        {
                            string opendt = model.Opendt.Replace(".", "");
                            string writerComment = model.Writercomment ?? ".";
                            using (var db = new HWNovelEntities())
                            {
                                db.PRO_VOLUME_UPDATE(model.Novelid, model.Volumeno, model.Volumtitle, model.Noveltext, writerComment, opendt);
                                db.SaveChanges();
                            }
                        }
                    }

                    return RedirectToAction("NovelInfo", "Serial", new { novelid = novelid, pageNum = pageNum, order = order });
                }
            }
        }
    }
}