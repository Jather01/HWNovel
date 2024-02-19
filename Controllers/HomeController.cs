using HWNovel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;

namespace HWNovel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Main()
        {
            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            List<HWNNOTICE> noticeList = new List<HWNNOTICE>();
            List<Novel> romList = new List<Novel>();
            List<Novel> rofList = new List<Novel>();
            List<Novel> fanList = new List<Novel>();
            List<Novel> hroList = new List<Novel>();
            List<Novel> mysList = new List<Novel>();
            List<Novel> newList = new List<Novel>();

            string nowDate = DateTime.Now.ToString("yyyyMMdd");

            using (var db = new HWNovelEntities())
            {
                noticeList = db.HWNNOTICE.OrderByDescending(x => x.NOTICENO)
                                .Take(5)
                                .ToList();

                List<HWN03> hwn03List = db.HWN03.Where(x => x.ENDYN.Equals("1")).ToList();

                List<HWN031> hwn031List = (from a in db.HWN031.ToList()
                                           where a.OPENDT.CompareTo(nowDate) <= 0
                                           group a by new
                                           {
                                               a.NOVELID
                                           } into b
                                           select new HWN031
                                           {
                                               NOVELID = b.Key.NOVELID,
                                               VIEWCNT = b.Sum(x => x.VIEWCNT),
                                               OPENDT = b.Max(x => x.OPENDT)
                                           }).ToList();

                romList = (from a in hwn03List
                           from b in hwn031List
                           where a.NOVELID == b.NOVELID
                              && a.GENRE == "1"
                           orderby b.VIEWCNT descending
                           select new Novel
                           {
                               Novelid = a.NOVELID,
                               Noveltitle = a.NOVELTITLE,
                               Writer = a.WRITER,
                               Viewcnt = b.VIEWCNT,
                               Genre = a.GENRE,
                               Opendt = b.OPENDT,
                               Thumnail = a.THUMNAIL
                           }).Take(5).ToList();

                rofList = (from a in hwn03List
                           from b in hwn031List
                           where a.NOVELID == b.NOVELID
                              && a.GENRE == "2"
                           orderby b.VIEWCNT descending
                           select new Novel
                           {
                               Novelid = a.NOVELID,
                               Noveltitle = a.NOVELTITLE,
                               Writer = a.WRITER,
                               Viewcnt = b.VIEWCNT,
                               Genre = a.GENRE,
                               Opendt = b.OPENDT,
                               Thumnail = a.THUMNAIL
                           }).Take(5).ToList();

                fanList = (from a in hwn03List
                           from b in hwn031List
                           where a.NOVELID == b.NOVELID
                              && a.GENRE == "3"
                           orderby b.VIEWCNT descending
                           select new Novel
                           {
                               Novelid = a.NOVELID,
                               Noveltitle = a.NOVELTITLE,
                               Writer = a.WRITER,
                               Viewcnt = b.VIEWCNT,
                               Genre = a.GENRE,
                               Opendt = b.OPENDT,
                               Thumnail = a.THUMNAIL
                           }).Take(5).ToList();

                hroList = (from a in hwn03List
                           from b in hwn031List
                           where a.NOVELID == b.NOVELID
                              && a.GENRE == "4"
                           orderby b.VIEWCNT descending
                           select new Novel
                           {
                               Novelid = a.NOVELID,
                               Noveltitle = a.NOVELTITLE,
                               Writer = a.WRITER,
                               Viewcnt = b.VIEWCNT,
                               Genre = a.GENRE,
                               Opendt = b.OPENDT,
                               Thumnail = a.THUMNAIL
                           }).Take(5).ToList();

                mysList = (from a in hwn03List
                           from b in hwn031List
                           where a.NOVELID == b.NOVELID
                              && a.GENRE == "5"
                           orderby b.VIEWCNT descending
                           select new Novel
                           {
                               Novelid = a.NOVELID,
                               Noveltitle = a.NOVELTITLE,
                               Writer = a.WRITER,
                               Viewcnt = b.VIEWCNT,
                               Genre = a.GENRE,
                               Opendt = b.OPENDT,
                               Thumnail = a.THUMNAIL
                           }).Take(5).ToList();

                var tempList1 = (from a in db.HWN031.ToList()
                                  where a.OPENDT.CompareTo(nowDate) <= 0
                                  group a by new
                                  {
                                      a.NOVELID
                                  } into b
                                  select new
                                  {
                                      NOVELID = b.Key.NOVELID,
                                      VOLUMENO = b.Count(),
                                      MINOPENDT = b.Min(x => x.OPENDT),
                                      MAXOPENDT = b.Max(x => x.OPENDT),
                                      VIEWCNT = b.Sum(x => x.VIEWCNT)
                                  }).ToList();

                newList = (from a in hwn03List
                           from b in tempList1
                           from c in db.HWN021.ToList()
                           where b.MINOPENDT.Substring(0, 6) == nowDate.Substring(0, 6)
                              && a.NOVELID == b.NOVELID
                              && c.GROUPNO == "03"
                              && a.GENRE == c.CODENO
                           orderby a.NOVELID
                           select new Novel
                           {
                               Novelid = a.NOVELID,
                               Noveltitle = a.NOVELTITLE,
                               Novelinfo = a.NOVELINFO,
                               Writer = a.WRITER,
                               Genre = c.CODENAME,
                               Thumnail = a.THUMNAIL,
                               Endyn = a.ENDYN
                           }).ToList();
            }

            foreach (var n in romList)
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

            foreach (var n in rofList)
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

            foreach (var n in fanList)
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

            foreach (var n in hroList)
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

            foreach (var n in mysList)
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

            foreach (var n in newList)
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

            ViewBag.noticeList = noticeList;
            ViewBag.romList = romList;
            ViewBag.rofList = rofList;
            ViewBag.fanList = fanList;
            ViewBag.hroList = hroList;
            ViewBag.mysList = mysList;
            ViewBag.newList = newList;

            ViewBag.nowDate = nowDate;

            return View();
        }

        public ActionResult Search()
        {
            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            string searchValue = Request.Params["searchValue"] as string;
            string Section = Request.Params["Section"] as string ?? "all";
            int searchPage = Int32.Parse(Request.Params["searchPage"] ?? "1");

            ViewBag.searchValue = searchValue;
            ViewBag.Section = Section;

            List<Novel> sList = new List<Novel>();
            int sCnt = 0;
            List<Novel> cList = new List<Novel>();
            int cCnt = 0;

            string nowDate = DateTime.Now.ToString("yyyyMMdd");

            int listCount = 10;
            int pageNum = searchPage;
            int pageSize = 10;

            using (var db = new HWNovelEntities())
            {
                List<HWN03> hwn03List = db.HWN03.Where(x => x.NOVELTITLE.IndexOf(searchValue) >= 0 || x.WRITER.IndexOf(searchValue) >= 0).ToList();

                List<HWN031> hwn031List = (from a in db.HWN031.ToList()
                                           where a.OPENDT.CompareTo(nowDate) <= 0
                                           group a by new
                                           {
                                               a.NOVELID
                                           } into b
                                           select new HWN031
                                           {
                                               NOVELID = b.Key.NOVELID,
                                               OPENDT = b.Max(x => x.OPENDT)
                                           }).ToList();

                sList = (from a in hwn03List
                         join b in hwn031List
                         on a.NOVELID equals b.NOVELID into table1
                         from b in table1.ToList()
                         orderby a.NOVELID
                         select new Novel
                         {
                             Novelid = a.NOVELID,
                             Noveltitle = a.NOVELTITLE,
                             Writer = a.WRITER,
                             Thumnail = a.THUMNAIL,
                             Endyn = a.ENDYN,
                             Opendt = b.OPENDT

                         }).ToList();

                sCnt = sList.Count;

                List<HWN04> hwn04List = db.HWN04.Where(x => x.NOVELTITLE.IndexOf(searchValue) >= 0 || (db.HWN01.Where(y => y.USERID.Equals(x.WRITER)).Select(y => y.NICKNAME).FirstOrDefault()).IndexOf(searchValue) >=0).ToList();

                List<HWN041> hwn041List = (from a in db.HWN041.ToList()
                                           where a.OPENDT.CompareTo(nowDate) <= 0
                                           group a by new
                                           {
                                               a.NOVELID
                                           } into b
                                           select new HWN041
                                           {
                                               NOVELID = b.Key.NOVELID,
                                               OPENDT = b.Max(x => x.OPENDT)
                                           }).ToList();

                cList = (from a in hwn04List
                         join b in hwn041List
                         on a.NOVELID equals b.NOVELID into table1
                         from b in table1.ToList()
                         orderby a.NOVELID
                         select new Novel
                         {
                             Novelid = a.NOVELID,
                             Noveltitle = a.NOVELTITLE,
                             Userid = a.WRITER,
                             Writer = (db.HWN01.Where(x => x.USERID.Equals(a.WRITER)).Select(x => x.NICKNAME).SingleOrDefault()),
                             Thumnail = a.THUMNAIL,
                             Endyn = a.ENDYN,
                             Opendt = b.OPENDT

                         }).ToList();

                cCnt = cList.Count;
            }

            if (Section.Equals("all"))
            {

                sList = sList.Take(3)
                     .ToList();

                cList = cList.Take(3)
                     .ToList();

                foreach (var n in sList)
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
            } else if (Section.Equals("Serial"))
            {
                sList = sList.Skip((pageNum - 1) * listCount)
                     .Take(listCount)
                     .ToList();

                foreach (var n in sList)
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

                //하단 시작 페이지 번호 
                int startPageNum = 1 + ((pageNum - 1) / pageSize) * pageSize;
                //하단 끝 페이지 번호
                int endPageNum = startPageNum + pageSize - 1;

                //전체 페이지의 갯수 구하기
                int totalPageCount = (int)Math.Ceiling(sCnt / (double)listCount);
                //끝 페이지 번호가 이미 전체 페이지 갯수보다 크게 계산되었다면 잘못된 값이다.
                if (endPageNum > totalPageCount)
                {
                    endPageNum = totalPageCount; //보정해 준다. 
                }
            } else if (Section.Equals("Challenge"))
            {
                cList = cList.Skip((pageNum - 1) * listCount)
                     .Take(listCount)
                     .ToList();

                //하단 시작 페이지 번호 
                int startPageNum = 1 + ((pageNum - 1) / pageSize) * pageSize;
                //하단 끝 페이지 번호
                int endPageNum = startPageNum + pageSize - 1;

                //전체 페이지의 갯수 구하기
                int totalPageCount = (int)Math.Ceiling(cCnt / (double)listCount);
                //끝 페이지 번호가 이미 전체 페이지 갯수보다 크게 계산되었다면 잘못된 값이다.
                if (endPageNum > totalPageCount)
                {
                    endPageNum = totalPageCount; //보정해 준다. 
                }
            }

            ViewBag.sList = sList;
            ViewBag.sListCnt = sCnt;
            ViewBag.cList = cList;
            ViewBag.cListCnt = cCnt;

            return View();
        }

        public ActionResult Notice(Notice model)
        {
            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            int listCount = 10;
            int pageNum = 1;
            int pageSize = 10;
            string keyword = model.searchValue ?? string.Empty;
            int totalCount = 0;

            if (model.searchPage != 0)
            {
                pageNum = model.searchPage;
            }

            List<HWNNOTICE> noticeList = new List<HWNNOTICE>();

            using (var db = new HWNovelEntities())
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    noticeList = db.HWNNOTICE.ToList();
                    totalCount = noticeList.Count();
                }
                else
                {
                    noticeList = db.HWNNOTICE.Where(x => x.NOTICETITLE.Contains(keyword) || x.NOTICETEXT.Contains(keyword)).ToList();
                    totalCount = noticeList.Count();
                }
            }

            noticeList = noticeList.OrderByDescending(x => x.NOTICENO)
                         .Skip((pageNum - 1) * listCount)
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

            ViewBag.PageNum = pageNum;
            ViewBag.StartPageNum = startPageNum;
            ViewBag.EndPageNum = endPageNum;
            ViewBag.TotalCount = totalCount;
            ViewBag.ListCount = listCount;
            ViewBag.KeyWord = keyword;

            return View(noticeList);
        }

        public ActionResult NoticeWrite(Notice model)
        {
            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
                return RedirectToAction("Notice", "Home", new { searchValue = model.searchValue, searchPage = model.searchPage });
            }
            else
            {
                if (userinfo[3] != "1")
                {
                    // 로그인 정보가 있는데 관리자 계정이 아니면 공지사항 목록 화면으로 이동
                    return RedirectToAction("Notice", "Home", new { searchValue = model.searchValue, searchPage = model.searchPage });
                }
            }

            ViewBag.PageNum = model.searchPage;
            ViewBag.KeyWord = model.searchValue;
            ViewBag.userinfo = userinfo;

            return View();
        }

        [HttpPost]
        public ActionResult NoticeWritePro(Notice model)
        {
            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
                return RedirectToAction("Notice", "Home", new { searchValue = model.searchValue, searchPage = model.searchPage });
            }
            else
            {
                if (userinfo[3] != "1")
                {
                    // 로그인 정보가 있는데 관리자 계정이 아니면 공지사항 목록 화면으로 이동
                    return RedirectToAction("Notice", "Home", new { searchValue = model.searchValue, searchPage = model.searchPage });
                }
            }

            if (model != null)
            {
                if(model.Noticetitle != "" && model.NoticeTextFile != null)
                {
                    HttpPostedFileBase noticeFile = model.NoticeTextFile;
                    string noticeText = "";
                    if (noticeFile != null && noticeFile.ContentLength > 0)
                    {
                        using (var reader = new StreamReader(noticeFile.InputStream))
                        {
                            while (!reader.EndOfStream)
                            {
                                //ReadLine 메서드로 한 행을 읽어 들여 line 변수에 대입
                                noticeText += reader.ReadLine();
                            }
                        }
                    }

                    using (var db = new HWNovelEntities())
                    {
                        HWNNOTICE noticeData = new HWNNOTICE();
                        decimal k = db.HWNNOTICE.Max(x => (decimal?)x.NOTICENO) ?? 0;
                        noticeData.NOTICENO = k + 1;
                        noticeData.NOTICETEXT = noticeText;

                        db.PRO_NOTICE_WRITE(noticeData.NOTICENO, model.Noticetitle, noticeData.NOTICETEXT);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Notice", "Home", new { searchValue = model.searchValue, searchPage = model.searchPage });
        }

        public ActionResult NoticeDetail(Notice model)
        {
            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            HWNNOTICE noticeView = new HWNNOTICE();
            using (var db = new HWNovelEntities())
            {
                noticeView = db.HWNNOTICE.Where(x => x.NOTICENO.Equals(model.Noticeno)).SingleOrDefault();
            }

            ViewBag.PageNum = model.searchPage;
            ViewBag.KeyWord = model.searchValue;
            ViewBag.noticeView = noticeView;

            return View(noticeView);
        }

        public ActionResult NoticeDelete(Notice model)
        {
            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
                return RedirectToAction("Notice", "Home", new { searchValue = model.searchValue, searchPage = model.searchPage });
            }
            else {
                if (userinfo[3] == "1")
                {
                    using (var db = new HWNovelEntities())
                    {
                        HWNNOTICE noticeView = db.HWNNOTICE.Where(x => x.NOTICENO.Equals(model.Noticeno)).SingleOrDefault();
                        db.HWNNOTICE.Remove(noticeView);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Notice", "Home", new { searchValue = model.searchValue, searchPage = model.searchPage });
        }

        public ActionResult NoticeEdit(Notice model)
        {
            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
                return RedirectToAction("Notice", "Home", new { searchValue = model.searchValue, searchPage = model.searchPage });
            }
            else
            {
                if (userinfo[3] != "1")
                {
                    // 로그인 정보가 있는데 관리자 계정이 아니면 공지사항 목록 화면으로 이동
                    return RedirectToAction("Notice", "Home", new { searchValue = model.searchValue, searchPage = model.searchPage });
                }
            }

            HWNNOTICE noticeView = new HWNNOTICE();
            using (var db = new HWNovelEntities())
            {
                noticeView = db.HWNNOTICE.Where(x => x.NOTICENO.Equals(model.Noticeno)).SingleOrDefault();
            }

            ViewBag.PageNum = model.searchPage;
            ViewBag.KeyWord = model.searchValue;
            ViewBag.userinfo = userinfo;

            return View(noticeView);
        }

        [HttpPost]
        public ActionResult NoticeEditPro(Notice model)
        {
            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo == null)
            {
                // 로그인 정보가 없으면 공지사항 목록 화면으로 이동
                return RedirectToAction("Notice", "Home", new { searchValue = model.searchValue, searchPage = model.searchPage });
            }
            else
            {
                if (userinfo[3] != "1")
                {
                    // 로그인 정보가 있는데 관리자 계정이 아니면 공지사항 목록 화면으로 이동
                    return RedirectToAction("Notice", "Home", new { searchValue = model.searchValue, searchPage = model.searchPage });
                }
            }

            if (model != null)
            {
                if (model.Noticetitle != "" && model.NoticeTextFile != null)
                {
                    HttpPostedFileBase noticeFile = model.NoticeTextFile;
                    string noticeText = "";
                    if (noticeFile != null && noticeFile.ContentLength > 0)
                    {
                        using (var reader = new StreamReader(noticeFile.InputStream))
                        {
                            while (!reader.EndOfStream)
                            {
                                //ReadLine 메서드로 한 행을 읽어 들여 line 변수에 대입
                                noticeText += reader.ReadLine();
                            }
                        }
                    }

                    using (var db = new HWNovelEntities())
                    {
                        db.PRO_NOTICE_EDIT(model.Noticeno, model.Noticetitle, noticeText);
                        db.SaveChanges();
                    }
                }
            }

            ViewBag.PageNum = model.searchPage;
            ViewBag.KeyWord = model.searchValue;
            ViewBag.userinfo = userinfo;

            return RedirectToAction("Notice", "Home", new { searchValue = model.searchValue, searchPage = model.searchPage });
        }
    }
}