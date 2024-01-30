using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HWNovel.ViewModels
{
    public class Novel
    {
        public string Novelid { get; set; }
        public string Noveltitle { get; set; }
        public string Novelinfo { get; set; }
        public string Writer { get; set; }
        public string Genre { get; set; }
        public string Thumnail { get; set; }
        public string Endyn { get; set; }
        public string Mon { get; set; }
        public string Tue { get; set; }
        public string Wed { get; set; }
        public string Thu { get; set; }
        public string Fri { get; set; }
        public string Sat { get; set; }
        public string Sun { get; set; }

        public decimal Volumeno { get; set; }
        public string Volumtitle { get; set; }
        public string Noveltext { get; set; }
        public string Writercomment { get; set; }
        public decimal Viewcnt { get; set; }
        public string Opendt { get; set; }
        public DateTime Insertdt { get; set; }

        public int Commentcnt { get; set; }
        public decimal StarPointAvg { get; set; }

        public HttpPostedFileBase ThumbnailFile { get; set; }
        public string ThumbnailBase64 { get; set; }

        public string searchValue { get; set; }                 // 검색어
        public int searchPage { get; set; } = 0;                // 목록 페이지
        public string searchOrder { get; set; }                 // 검색 정렬
        public string searchGenre { get; set; }                 // 검색 장르
    }
}