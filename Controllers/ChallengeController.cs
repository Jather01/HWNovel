using HWNovel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HWNovel.Controllers
{
    public class ChallengeController : Controller
    {
        public ActionResult Gen(Novel model)
        {
            string genre = Request.Params["genre"] as string;
            string genName = "";

            if (genre == "1")
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

            string searchGenre = genre;
            string searchOrder = model.searchOrder ?? "view";
            string searchValue = model.searchValue ?? "1";

            int listCount = 20;
            int pageNum = 1;
            int pageSize = 10;
            int totalCount = 0;

            if (model.searchPage != 0)
            {
                pageNum = model.searchPage;
            }

            List<Novel> novelList = new List<Novel>();

            string nowDate = DateTime.Now.ToString("yyyyMMdd");

            using (var db = new HWNovelEntities())
            {
                List<HWN04> HWN04List = db.HWN04.Where(x => x.ENDYN.Equals(searchValue) && x.GENRE.Equals(searchGenre)).ToList();

                List<HWN041> HWN041List = (from a in db.HWN041.ToList()
                                           where a.OPENDT.CompareTo(nowDate) <= 0
                                           group a by new
                                           {
                                               a.NOVELID
                                           } into b
                                           select new HWN041
                                           {
                                               NOVELID = b.Key.NOVELID,
                                               VOLUMENO = b.Count(),
                                               OPENDT = b.Max(x => x.OPENDT),
                                               VIEWCNT = b.Sum(x => x.VIEWCNT)
                                           }).ToList();

                List<HWN043> HWN043List = (from a in db.HWN043.ToList()
                                           from b in (from c in db.HWN041.ToList()
                                                      where c.OPENDT.CompareTo(nowDate) <= 0
                                                      select new { NOVELID = c.NOVELID, VOLUMENO = c.VOLUMENO })
                                           where a.NOVELID == b.NOVELID
                                              && a.VOLUMENO == b.VOLUMENO
                                           group a by new
                                           {
                                               a.NOVELID
                                           } into d
                                           select new HWN043
                                           {
                                               NOVELID = d.Key.NOVELID,
                                               STARPOINT = d.Average(x => x.STARPOINT)
                                           }).ToList();
                List<HWN01> HWN01List = db.HWN01.ToList();

                var hwn011List = (from a in db.HWN011.ToList()
                                  where a.NOVELKIND == "2"
                                  group a by new
                                  {
                                      a.NOVELID
                                  } into b
                                  select new
                                  {
                                      NOVELID = b.Key.NOVELID,
                                      FAVORITECNT = b.Count()
                                  }).ToList();


                novelList = (from a in HWN04List
                             join b in HWN041List
                             on a.NOVELID equals b.NOVELID into table1
                             from b in table1.ToList()
                             join c in HWN043List
                             on a.NOVELID equals c.NOVELID into table2
                             from c in table2.ToList().DefaultIfEmpty()
                             join d in hwn011List
                             on a.NOVELID equals d.NOVELID into table3
                             from d in table3.ToList().DefaultIfEmpty()
                             join e in HWN01List
                             on a.WRITER equals e.USERID into table4
                             from e in table4.ToList()
                             select new Novel
                             {
                                 Novelid = a.NOVELID,
                                 Noveltitle = a.NOVELTITLE,
                                 Writer = e.NICKNAME,
                                 Genre = a.GENRE,
                                 Thumnail = a.THUMNAIL,
                                 Endyn = a.ENDYN,
                                 Opendt = b?.OPENDT ?? "",
                                 Viewcnt = b?.VIEWCNT ?? 0,
                                 Volumecnt = Decimal.ToInt32(b?.VOLUMENO ?? 0),
                                 StarPointAvg = Math.Round(c?.STARPOINT ?? 0, 2),
                                 Favoritecnt = d?.FAVORITECNT ?? 0

                             }).ToList();

                if (!string.IsNullOrWhiteSpace(searchOrder))
                {
                    switch (searchOrder)
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

            ViewBag.StartPageNum = startPageNum;
            ViewBag.EndPageNum = endPageNum;
            ViewBag.TotalCount = totalCount;
            ViewBag.ListCount = listCount;
            ViewBag.NowDate = nowDate;

            ViewBag.NovelList = novelList;

            ViewBag.searchPage = pageNum;
            ViewBag.searchGenre = searchGenre;
            ViewBag.searchOrder = searchOrder;
            ViewBag.searchValue = searchValue;

            return View();
        }

        public ActionResult NovelInfo()
        {
            string topmenu = "none";
            string novelId = Request.Params["novelid"];

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

            HWN011 u = null;
            HWN021 genreCode = new HWN021();
            Novel n = new Novel();
            List<HWN041> nList = new List<HWN041>();
            List<Novel> novleList = new List<Novel>();
            List<HWN011> favorits = new List<HWN011>();
            int favorit = 0;
            string nowDate = DateTime.Now.ToString("yyyyMMdd");

            using (var db = new HWNovelEntities())
            {
                var n04 = db.HWN04.Where(x => x.NOVELID.Equals(novelId)).ToList();

                var hwn043List = (from a in db.HWN043.ToList()
                                  from b in (from c in db.HWN041.ToList()
                                             where c.OPENDT.CompareTo(nowDate) <= 0
                                             select new { NOVELID = c.NOVELID, VOLUMENO = c.VOLUMENO })
                                  where a.NOVELID == b.NOVELID
                                      && a.VOLUMENO == b.VOLUMENO
                                  group a by new
                                  {
                                      a.NOVELID
                                  } into d
                                  select new HWN043
                                  {
                                      NOVELID = d.Key.NOVELID,
                                      STARPOINT = d.Average(x => x.STARPOINT)
                                  }).ToList();

                n = (from a in n04
                     join b in hwn043List
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
                         StarPointAvg = Math.Round(b?.STARPOINT ?? 0, 2)
                     }).SingleOrDefault();

                genreCode = db.HWN021.Where(x => x.GROUPNO.Equals("03") && x.CODENO.Equals(n.searchGenre)).SingleOrDefault();

                n.Genre = genreCode.CODENAME;

                if (!string.IsNullOrWhiteSpace(userid))
                {
                    List<HWN011> list = db.HWN011.ToList();
                    u = (from a in list
                         where a.NOVELID.Equals(novelId) && a.USERID.Equals(userid) && a.NOVELKIND.Equals("1")
                         select new HWN011
                         {
                             USERID = a.USERID,
                             NOVELID = a.NOVELID
                         }).SingleOrDefault();
                }

                if (userPower.Equals("1"))
                {
                    nList = db.HWN041.Where(x => x.NOVELID.Equals(novelId)).ToList();
                }
                else
                {
                    string nowdate = DateTime.Now.ToString("yyyyMMdd");
                    nList = db.HWN041.Where(x => x.NOVELID.Equals(novelId) && x.OPENDT.CompareTo(nowdate) <= 0).ToList();
                }

                var n042List = db.HWN042.Where(x => x.NOVELID.Equals(novelId))
                                        .GroupBy(x => new { x.NOVELID, x.VOLUMENO })
                                        .Select(x => new
                                        {
                                            NOVELID = x.Key.NOVELID,
                                            VOLUMENO = x.Key.VOLUMENO,
                                            COMMENTCNT = x.Count()
                                        })
                                        .ToList();

                var n043List = db.HWN043.Where(x => x.NOVELID.Equals(novelId))
                                        .GroupBy(x => new { x.NOVELID, x.VOLUMENO })
                                        .Select(x => new
                                        {
                                            NOVELID = x.Key.NOVELID,
                                            VOLUMENO = x.Key.VOLUMENO,
                                            STARPOINTAVG = x.Average(a => a.STARPOINT)
                                        })
                                        .ToList();

                novleList = (from a in nList
                             join b in n042List
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
                             join g in n043List
                             on new { e.NOVELID, e.VOLUMENO } equals new { g.NOVELID, g.VOLUMENO } into h
                             from i in h.DefaultIfEmpty()
                             select new Novel
                             {
                                 Novelid = e.NOVELID,
                                 Volumeno = e.VOLUMENO,
                                 Volumtitle = e.VOLUMTITLE,
                                 Opendt = e?.OPENDT ?? "",
                                 Commentcnt = e.COMMENTCNT,
                                 StarPointAvg = Math.Round(i?.STARPOINTAVG ?? 0, 2)
                             })
                                .ToList();

                favorits = db.HWN011.Where(x => x.NOVELID.Equals(novelId) && x.NOVELKIND.Equals("1")).ToList();
                favorit = favorits.Count;
            }

            if (n == null)
            {
                return RedirectToAction("NovelInfo", "Challenge", new { novelid = novelId, pageNum = searchPage, order = order });
            }

            if (order.Equals("update"))
            {
                novleList = novleList.OrderByDescending(x => x.Volumeno).ToList();
            }
            else if (order.Equals("oldest"))
            {
                novleList = novleList.OrderBy(x => x.Volumeno).ToList();
            }

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
                n = (from a in db.HWN041
                     from b in db.HWN04
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

                nList = (from a in db.HWN041
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
                    return RedirectToAction("NovelInfo", "Challenge", new { novelid = novelId, pageNum = searchPage, order = order });
                }

                string nowdate = DateTime.Now.ToString("yyyyMMdd");

                if (!userPower.Equals("1") && !n.Writer.Equals(userid))
                {
                    if (n.Opendt.CompareTo(nowdate) > 0)
                    {
                        return RedirectToAction("NovelInfo", "Challenge", new { novelid = novelId, pageNum = searchPage, order = order });
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
                         from d in db.HWN041
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
                         from d in db.HWN041
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
                    db.PRO_USER_RECENT_NOVEL(userid, n.Novelid, n.Volumeno, "2");

                    starYn = db.HWN043.Where(x => x.NOVELID.Equals(novelId) && x.VOLUMENO.ToString().Equals(volumeNo) && x.USERID.Equals(userid)).ToList().Count;
                }

                db.PRO_CHALLENGE_VIEWCNT_UPDATE(n.Novelid, n.Volumeno);
                db.SaveChanges();

                Novel spResult = db.HWN043.Where(x => x.NOVELID.Equals(novelId) && x.VOLUMENO.ToString().Equals(volumeNo))
                                        .GroupBy(x => new { x.NOVELID, x.VOLUMENO })
                                        .Select(x => new Novel
                                        {
                                            Novelid = x.Key.NOVELID,
                                            Volumeno = x.Key.VOLUMENO,
                                            StarPointAvg = x.Average(a => a.STARPOINT),
                                            Commentcnt = x.Count()
                                        }).SingleOrDefault();

                if (spResult != null)
                {
                    spText[0] = spResult.Commentcnt;
                    spText[1] = Math.Round(spResult.StarPointAvg, 2);
                }
            }

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
                    cList = (from a in db.HWN042
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
    }
}