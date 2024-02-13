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
    }
}