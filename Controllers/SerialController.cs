using HWNovel.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;

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

        public ActionResult NovelManage(Novel model)
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
                    if (model != null)
                    {
                        if (model.Noveltitle != "" && model.Genre != "" && model.Novelinfo != "" && model.ThumbnailFile != null)
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
            string topmenu = Request.Params["topmenu"];
            string novelId = Request.Params["novelid"];

            ViewBag.topmenu = topmenu;
            ViewBag.userinfo = Session["userinfo"];

            if(novelId == null || novelId.Equals(""))
            {
                // 소설 아이디 정보가 없으면 메인 홈 화면으로 이동
                return RedirectToAction("Main", "Home");
            }
            else
            {


                return View();
            }
        }
    }
}