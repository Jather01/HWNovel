using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HWNovel.ViewModels
{
    public class Notice
    {
        public decimal Noticeno { get; set; }                    // 공지 제목
        public string Noticetitle { get; set; }                 // 공지 제목
        public HttpPostedFileBase NoticeTextFile { get; set; }  // 공지 내용 파일
        public string NoticeTextFilePath { get; set; }          // 공지 내용 파일 경로
        public string NoticeText { get; set; }                  // 공지 내용
        public DateTime NoticeDate { get; set; }                // 공지 일자

        public string searchValue { get; set; }                 // 공지사항 목록 검색
        public int searchPage { get; set; } = 0;                // 공지사항 목록 페이지
    }
}