using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HWNovel.ViewModels
{
    public class Comment
    {
        public string Novelid { get; set; }
        public decimal Volumeno { get; set; }
        public decimal Usercommentno { get; set; }
        public string Usercomment { get; set; }
        public string Userid { get; set; }
        public string Nickname { get; set; }
        public DateTime Insertdt { get; set; }

        public int searchPage { get; set; } = 0;
    }
}