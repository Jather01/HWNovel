using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HWNovel.ViewModels
{
    public class Novel
    {
        public HWN03 Hwn03 { get; set; }
        public HWN031 Hwn031 { get; set; }

        public string Novelid { get; set; }

        public HttpPostedFileBase ThumbnailFile { get; set; }

        public string searchValue { get; set; }                 // 검색어
        public int searchPage { get; set; } = 0;                // 목록 페이지
        public string searchOrder { get; set; }                 // 검색 정렬
        public string searchGenre { get; set; }                 // 검색 장르
    }
}