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
    public class AjaxController : Controller
    {
        [HttpPost]
        public JsonResult IdOverlap(string id)
        {
            string result = "1"; // 0: 중복 있음, 1: 중복 없음
            int totalCount = 0;

            var ids = new List<HWN01>();

            if (!string.IsNullOrWhiteSpace(id))
            {
                using (var db = new HWNovelEntities())
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

            if (!string.IsNullOrWhiteSpace(nickname))
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
        public JsonResult NicknameOverlapInfoUpdate(string nickname, string id)
        {
            string result = "1"; // 0: 중복 있음, 1: 중복 없음
            int totalCount = 0;

            var nicknames = new List<HWN01>();

            if (!string.IsNullOrWhiteSpace(nickname))
            {
                using (var db = new HWNovelEntities())
                {
                    nicknames = db.HWN01.Where(x => x.NICKNAME == nickname && !x.USERID.Equals(id)).ToList();
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

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(birthday))
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

            if (!string.IsNullOrWhiteSpace(id) &&  !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(birthday))
            {
                using (var db = new HWNovelEntities())
                {
                    ids = db.HWN01.Where(x =>
                           x.USERID == id
                        && x.NAME == name
                        && x.BIRTHDAY == birthday
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

        [HttpPost]
        public JsonResult PwCheck(string id, string pw)
        {
            string result = "0"; // 0: 인증 실패, 1: 인증 성공

            HWN01 user = new HWN01();

            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(pw))
            {
                SHA256 sha = new SHA256Managed();
                byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(pw));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.AppendFormat("{0:x2}", b);
                }

                string encpassword = sb.ToString();

                using (var db = new HWNovelEntities())
                {
                    user = db.HWN01.Where(x =>
                           x.USERID == id
                        && x.ENCPASSWORD == encpassword
                        && x.USEYN == "1")
                        .SingleOrDefault();
                }
            }
            if (user != null)
            {
                result = "1";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult NovelFavorite(string novelid, string novelkind)
        {
            string result = ""; // on: 관심 등록, off: 관심 해제

            List<string> userinfo = (List<string>)Session["userinfo"];

           if (userinfo != null && !string.IsNullOrWhiteSpace(novelid))
            {
                string userid = userinfo[0];
                HWN011 h011 = new HWN011();
                int h011Cnt = 0;

                using (var db = new HWNovelEntities())
                {
                    h011 = db.HWN011.Where(x => x.USERID.Equals(userid) && x.NOVELID.Equals(novelid) && x.NOVELKIND.Equals(novelkind)).SingleOrDefault();
                    if(h011 == null)
                    {
                        HWN011 favoritNovel = new HWN011();
                        favoritNovel.USERID = userid;
                        favoritNovel.NOVELID = novelid;
                        favoritNovel.NOVELKIND = novelkind;
                        favoritNovel.DATE = DateTime.Now;

                        db.HWN011.Add(favoritNovel);
                        db.SaveChanges();
                        result = "on";
                    }
                    else
                    {
                        db.HWN011.Remove(h011);
                        db.SaveChanges();
                        result = "off";
                    }

                    h011Cnt = db.HWN011.Where(x => x.NOVELID.Equals(novelid) && x.NOVELKIND.Equals(novelkind)).ToList().Count();
                    result += ("/" + h011Cnt);
                }
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult StarPointSubmit(string novelid, int volumeno, int starpoint)
        {
            string result = "0/0"; // 별점참여인원수/별점평균

            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo != null && !string.IsNullOrWhiteSpace(novelid))
            {
                string userid = userinfo[0];
                HWN033 h033 = new HWN033();

                using (var db = new HWNovelEntities())
                {
                    h033.USERID = userid;
                    h033.NOVELID = novelid;
                    h033.VOLUMENO = volumeno;
                    h033.INSERTDT = DateTime.Now;
                    h033.STARPOINT = starpoint;

                    db.HWN033.Add(h033);
                    db.SaveChanges();

                    Novel spResult = db.HWN033.Where(x => x.NOVELID.Equals(novelid) && x.VOLUMENO == volumeno)
                                        .GroupBy(x => new { x.NOVELID, x.VOLUMENO })
                                        .Select(x => new Novel
                                        {
                                            Novelid = x.Key.NOVELID,
                                            Volumeno = x.Key.VOLUMENO,
                                            StarPointAvg = x.Average(a => a.STARPOINT),
                                            Commentcnt = x.Count()
                                        }).SingleOrDefault();

                    result = spResult.Commentcnt.ToString() + "/";
                    if (spResult.StarPointAvg == 10)
                    {
                        result += Math.Round(spResult.StarPointAvg, 1).ToString();
                    }
                    else
                    {
                        result += Math.Round(spResult.StarPointAvg, 2).ToString();
                    }
                }
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult StarPointChallengeNovelSubmit(string novelid, int volumeno, int starpoint)
        {
            string result = "0/0"; // 별점참여인원수/별점평균

            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo != null && !string.IsNullOrWhiteSpace(novelid))
            {
                string userid = userinfo[0];
                HWN043 h043 = new HWN043();

                using (var db = new HWNovelEntities())
                {
                    h043.USERID = userid;
                    h043.NOVELID = novelid;
                    h043.VOLUMENO = volumeno;
                    h043.INSERTDT = DateTime.Now;
                    h043.STARPOINT = starpoint;

                    db.HWN043.Add(h043);
                    db.SaveChanges();

                    Novel spResult = db.HWN043.Where(x => x.NOVELID.Equals(novelid) && x.VOLUMENO == volumeno)
                                        .GroupBy(x => new { x.NOVELID, x.VOLUMENO })
                                        .Select(x => new Novel
                                        {
                                            Novelid = x.Key.NOVELID,
                                            Volumeno = x.Key.VOLUMENO,
                                            StarPointAvg = x.Average(a => a.STARPOINT),
                                            Commentcnt = x.Count()
                                        }).SingleOrDefault();

                    result = spResult.Commentcnt.ToString() + "/";
                    if (spResult.StarPointAvg == 10)
                    {
                        result += Math.Round(spResult.StarPointAvg, 1).ToString();
                    }
                    else
                    {
                        result += Math.Round(spResult.StarPointAvg, 2).ToString();
                    }
                }
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult writeSerialNovelComment(string novelid, int volumeno, string comment)
        {
            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo != null && !string.IsNullOrWhiteSpace(novelid))
            {
                string userid = userinfo[0];

                using (var db = new HWNovelEntities())
                {
                    db.PRO_SERIAL_NOVEL_COMMENT_WRITE(novelid, volumeno, comment, userid);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("CommentInclude", "Serial", new { Novelid = novelid, Volumeno = volumeno });
        }

        [HttpPost]
        public ActionResult deleteSerialNovelComment(string novelid, int volumeno, int usercommentno)
        {
            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo != null && !string.IsNullOrWhiteSpace(novelid))
            {
                string userid = userinfo[0];
                HWN032 h032 = new HWN032();

                using (var db = new HWNovelEntities())
                {
                    h032 = db.HWN032.Where(x => x.NOVELID.Equals(novelid) && x.VOLUMENO == volumeno && x.USERCOMMENTNO == usercommentno).SingleOrDefault();
                    if(h032 != null)
                    {
                        if (userid.Equals(h032.USERID) || userinfo[3].Equals("1"))
                        {
                            db.HWN032.Remove(h032);
                            db.SaveChanges();
                        }
                    }
                }
            }

            return RedirectToAction("CommentInclude", "Serial", new { Novelid = novelid, Volumeno = volumeno });
        }

        [HttpPost]
        public ActionResult writeChallengeNovelComment(string novelid, int volumeno, string comment)
        {
            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo != null && !string.IsNullOrWhiteSpace(novelid))
            {
                string userid = userinfo[0];

                using (var db = new HWNovelEntities())
                {
                    db.PRO_CHALLENGE_NOVEL_COMMENT_WRITE(novelid, volumeno, comment, userid);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("CommentInclude", "Challenge", new { Novelid = novelid, Volumeno = volumeno });
        }

        [HttpPost]
        public ActionResult deleteChallengeNovelComment(string novelid, int volumeno, int usercommentno)
        {
            List<string> userinfo = (List<string>)Session["userinfo"];

            if (userinfo != null && !string.IsNullOrWhiteSpace(novelid))
            {
                string userid = userinfo[0];
                HWN042 h042 = new HWN042();

                using (var db = new HWNovelEntities())
                {
                    h042 = db.HWN042.Where(x => x.NOVELID.Equals(novelid) && x.VOLUMENO == volumeno && x.USERCOMMENTNO == usercommentno).SingleOrDefault();
                    if (h042 != null)
                    {
                        if (userid.Equals(h042.USERID) || userinfo[3].Equals("1"))
                        {
                            db.HWN042.Remove(h042);
                            db.SaveChanges();
                        }
                    }
                }
            }

            return RedirectToAction("CommentInclude", "Challenge", new { Novelid = novelid, Volumeno = volumeno });
        }
    }
}