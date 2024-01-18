using HWNovel.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HWNovel.Controllers
{
    public class AjaxController : Controller
    {
        [HttpPost]
        public JsonResult IdOverlap(string id)
        {
            string result = "1"; // 0: 중복 있음, 1: 중복 없음
            int totalCount = 0;

            var ids = new List<HWN01>();

            if (!string.IsNullOrEmpty(id))
            {
                using(var db = new HWNovelEntities())
                {
                    ids = db.HWN01.Where(x => x.USERID == id).ToList();
                    totalCount = ids.Count();
                }
            }

            if(totalCount > 0) result = "0";
            else result = "1";

            return Json(result);
        }

        [HttpPost]
        public JsonResult NicknameOverlap(string nickname)
        {
            string result = "1"; // 0: 중복 있음, 1: 중복 없음
            int totalCount = 0;

            var nicknames = new List<HWN01>();

            if (!string.IsNullOrEmpty(nickname))
            {
                using (var db = new HWNovelEntities())
                {
                    nicknames = db.HWN01.Where(x => x.NICKNAME == nickname).ToList();
                    totalCount = nicknames.Count();
                }
            }

            if (totalCount > 0) result = "0";
            else result = "1";
            return Json(result);
        }

        [HttpPost]
        public JsonResult FindId(string name, string birthday)
        {
            string result = "";

            var ids = new List<string>();

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(birthday))
            {
                using (var db = new HWNovelEntities())
                {
                    ids = db.HWN01.Where(x =>
                           x.NAME == name
                        && x.BIRTHDAY == birthday
                        && x.POWER == "2"
                        && x.USEYN == "1")
                        .Select(x => x.USERID)
                        .ToList();
                }
            }
            if(ids.Count > 0)
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    if (i == 0)
                    {
                        result += ids[i];
                    } else
                    {
                        result += "," + ids[i];
                    }
                }
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult FindPw(string id, string name, string birthday)
        {
            string result = "0"; // 0: 아이디 없음, 1: 아이디 있음

            var ids = new List<string>();

            if (!string.IsNullOrEmpty(id) &&  !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(birthday))
            {
                using (var db = new HWNovelEntities())
                {
                    ids = db.HWN01.Where(x =>
                           x.USERID == id
                        && x.NAME == name
                        && x.BIRTHDAY == birthday
                        && x.POWER == "2"
                        && x.USEYN == "1")
                        .Select(x => x.USERID)
                        .ToList();
                }
            }
            if (ids.Count > 0)
            {
                result = "1";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult NoticeFileRead()
        {
            string result = "";
            if (Request != null)
            {
                HttpPostedFileBase noticeFile = Request.Files["NoticeTextFile"];

                if (noticeFile != null && noticeFile.ContentLength > 0)
                {
                    using (var reader = new StreamReader(noticeFile.InputStream))
                    {
                        while (!reader.EndOfStream)
                        {
                            //ReadLine 메서드로 한 행을 읽어 들여 line 변수에 대입
                            result += reader.ReadLine();
                        }
                    }
                }
            }

            return Json(result);
        }
    }
}