using HWNovel.ViewModels;
using Newtonsoft.Json.Linq;
using System;
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

            using (var db = new HWNovelEntities())
            {
                noticeList = db.HWNNOTICE.OrderByDescending(x => x.NOTICENO)
                                .Take(5)
                                .ToList();
            }

            ViewBag.noticeList = noticeList;

            return View();
        }

        public ActionResult Search()
        {
            List<string> userinfo = (List<string>)Session["userinfo"];
            ViewBag.userinfo = userinfo;
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