﻿//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 템플릿에서 생성되었습니다.
//
//     이 파일을 수동으로 변경하면 응용 프로그램에서 예기치 않은 동작이 발생할 수 있습니다.
//     이 파일을 수동으로 변경하면 코드가 다시 생성될 때 변경 내용을 덮어씁니다.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HWNovel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class HWNovelEntities : DbContext
    {
        public HWNovelEntities()
            : base("name=HWNovelEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<HWN01> HWN01 { get; set; }
        public virtual DbSet<HWN011> HWN011 { get; set; }
        public virtual DbSet<HWN012> HWN012 { get; set; }
        public virtual DbSet<HWN02> HWN02 { get; set; }
        public virtual DbSet<HWN021> HWN021 { get; set; }
        public virtual DbSet<HWN03> HWN03 { get; set; }
        public virtual DbSet<HWN031> HWN031 { get; set; }
        public virtual DbSet<HWN032> HWN032 { get; set; }
        public virtual DbSet<HWN033> HWN033 { get; set; }
        public virtual DbSet<HWN04> HWN04 { get; set; }
        public virtual DbSet<HWN041> HWN041 { get; set; }
        public virtual DbSet<HWN042> HWN042 { get; set; }
        public virtual DbSet<HWN043> HWN043 { get; set; }
        public virtual DbSet<HWNNOTICE> HWNNOTICE { get; set; }
    
        public virtual int PRO_NOTICE_WRITE(Nullable<decimal> nOTICENO, string nOTICETITLE, string nOTICETEXT)
        {
            var nOTICENOParameter = nOTICENO.HasValue ?
                new ObjectParameter("NOTICENO", nOTICENO) :
                new ObjectParameter("NOTICENO", typeof(decimal));
    
            var nOTICETITLEParameter = nOTICETITLE != null ?
                new ObjectParameter("NOTICETITLE", nOTICETITLE) :
                new ObjectParameter("NOTICETITLE", typeof(string));
    
            var nOTICETEXTParameter = nOTICETEXT != null ?
                new ObjectParameter("NOTICETEXT", nOTICETEXT) :
                new ObjectParameter("NOTICETEXT", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PRO_NOTICE_WRITE", nOTICENOParameter, nOTICETITLEParameter, nOTICETEXTParameter);
        }
    
        public virtual int PRO_NOTICE_EDIT(Nullable<decimal> nOTICENO, string nOTICETITLE, string nOTICETEXT)
        {
            var nOTICENOParameter = nOTICENO.HasValue ?
                new ObjectParameter("NOTICENO", nOTICENO) :
                new ObjectParameter("NOTICENO", typeof(decimal));
    
            var nOTICETITLEParameter = nOTICETITLE != null ?
                new ObjectParameter("NOTICETITLE", nOTICETITLE) :
                new ObjectParameter("NOTICETITLE", typeof(string));
    
            var nOTICETEXTParameter = nOTICETEXT != null ?
                new ObjectParameter("NOTICETEXT", nOTICETEXT) :
                new ObjectParameter("NOTICETEXT", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PRO_NOTICE_EDIT", nOTICENOParameter, nOTICETITLEParameter, nOTICETEXTParameter);
        }
    }
}
