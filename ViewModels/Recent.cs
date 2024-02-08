using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace HWNovel.ViewModels
{
    public class Recent
    {
        public string Userid { get; set; }
        public string Novelid { get; set; }
        public string Noveltitle { get; set; }
        public string Thumnail { get; set; }
        public string ThumbnailBase64 { get; set; }
        public decimal Volumeno { get; set; }
        public decimal Nextvolumeno { get; set; }
        public string Volumtitle { get; set; }
        public string Novelkind { get; set; }
        public string Opendt { get; set; }
        public DateTime Date { get; set; }
    }
}