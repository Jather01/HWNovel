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
        public string Insertdt { get; set; }
        public int Volumeno { get; set; }
        public string Volumtitle { get; set; }
        public string Noveltext { get; set; }
        public string Writercomment { get; set; }
        public int Viewcnt { get; set; }
        public string Opendt { get; set; }

        public HttpPostedFileBase ThumbnailFile { get; set; }
    }
}