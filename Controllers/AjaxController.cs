using HWNovel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HWNovel.Controllers
{
    public class AjaxController : Controller
    {
        private HWNovelEntities db = new HWNovelEntities();

        [HttpPost]
        public JsonResult IdOverlap(string id)
        {
            string result = "1"; // 0: 중복 있음, 1: 중복 없음
            int totalCount = 0;

            var ids = new List<HWN01>();

            if (!string.IsNullOrEmpty(id))
            {
                ids = db.HWN01.Where(x => x.USERID == id).ToList();
                totalCount = ids.Count();
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
                nicknames = db.HWN01.Where(x => x.NICKNAME == nickname).ToList();
                totalCount = nicknames.Count();
            }

            if (totalCount > 0) result = "0";
            else result = "1";
            return Json(result);
        }
    }
}