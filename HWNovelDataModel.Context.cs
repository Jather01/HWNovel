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
    
        public virtual int PRO_USER_INFOUPDATE(string uSERID, string nAME, string bIRTHDAY, string nICKNAME)
        {
            var uSERIDParameter = uSERID != null ?
                new ObjectParameter("USERID", uSERID) :
                new ObjectParameter("USERID", typeof(string));
    
            var nAMEParameter = nAME != null ?
                new ObjectParameter("NAME", nAME) :
                new ObjectParameter("NAME", typeof(string));
    
            var bIRTHDAYParameter = bIRTHDAY != null ?
                new ObjectParameter("BIRTHDAY", bIRTHDAY) :
                new ObjectParameter("BIRTHDAY", typeof(string));
    
            var nICKNAMEParameter = nICKNAME != null ?
                new ObjectParameter("NICKNAME", nICKNAME) :
                new ObjectParameter("NICKNAME", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PRO_USER_INFOUPDATE", uSERIDParameter, nAMEParameter, bIRTHDAYParameter, nICKNAMEParameter);
        }
    
        public virtual int PRO_NOVEL_WRITE(string nOVELID, string nOVELTITLE, string nOVELINFO, string wRITER, string gENRE, string tHUMNAIL, string mON, string tUE, string wED, string tHU, string fRI, string sAT, string sUN)
        {
            var nOVELIDParameter = nOVELID != null ?
                new ObjectParameter("NOVELID", nOVELID) :
                new ObjectParameter("NOVELID", typeof(string));
    
            var nOVELTITLEParameter = nOVELTITLE != null ?
                new ObjectParameter("NOVELTITLE", nOVELTITLE) :
                new ObjectParameter("NOVELTITLE", typeof(string));
    
            var nOVELINFOParameter = nOVELINFO != null ?
                new ObjectParameter("NOVELINFO", nOVELINFO) :
                new ObjectParameter("NOVELINFO", typeof(string));
    
            var wRITERParameter = wRITER != null ?
                new ObjectParameter("WRITER", wRITER) :
                new ObjectParameter("WRITER", typeof(string));
    
            var gENREParameter = gENRE != null ?
                new ObjectParameter("GENRE", gENRE) :
                new ObjectParameter("GENRE", typeof(string));
    
            var tHUMNAILParameter = tHUMNAIL != null ?
                new ObjectParameter("THUMNAIL", tHUMNAIL) :
                new ObjectParameter("THUMNAIL", typeof(string));
    
            var mONParameter = mON != null ?
                new ObjectParameter("MON", mON) :
                new ObjectParameter("MON", typeof(string));
    
            var tUEParameter = tUE != null ?
                new ObjectParameter("TUE", tUE) :
                new ObjectParameter("TUE", typeof(string));
    
            var wEDParameter = wED != null ?
                new ObjectParameter("WED", wED) :
                new ObjectParameter("WED", typeof(string));
    
            var tHUParameter = tHU != null ?
                new ObjectParameter("THU", tHU) :
                new ObjectParameter("THU", typeof(string));
    
            var fRIParameter = fRI != null ?
                new ObjectParameter("FRI", fRI) :
                new ObjectParameter("FRI", typeof(string));
    
            var sATParameter = sAT != null ?
                new ObjectParameter("SAT", sAT) :
                new ObjectParameter("SAT", typeof(string));
    
            var sUNParameter = sUN != null ?
                new ObjectParameter("SUN", sUN) :
                new ObjectParameter("SUN", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PRO_NOVEL_WRITE", nOVELIDParameter, nOVELTITLEParameter, nOVELINFOParameter, wRITERParameter, gENREParameter, tHUMNAILParameter, mONParameter, tUEParameter, wEDParameter, tHUParameter, fRIParameter, sATParameter, sUNParameter);
        }
    
        public virtual int PRO_NOVEL_UPDATE(string nOVELID, string nOVELTITLE, string nOVELINFO, string wRITER, string gENRE, string tHUMNAIL, string mON, string tUE, string wED, string tHU, string fRI, string sAT, string sUN)
        {
            var nOVELIDParameter = nOVELID != null ?
                new ObjectParameter("NOVELID", nOVELID) :
                new ObjectParameter("NOVELID", typeof(string));
    
            var nOVELTITLEParameter = nOVELTITLE != null ?
                new ObjectParameter("NOVELTITLE", nOVELTITLE) :
                new ObjectParameter("NOVELTITLE", typeof(string));
    
            var nOVELINFOParameter = nOVELINFO != null ?
                new ObjectParameter("NOVELINFO", nOVELINFO) :
                new ObjectParameter("NOVELINFO", typeof(string));
    
            var wRITERParameter = wRITER != null ?
                new ObjectParameter("WRITER", wRITER) :
                new ObjectParameter("WRITER", typeof(string));
    
            var gENREParameter = gENRE != null ?
                new ObjectParameter("GENRE", gENRE) :
                new ObjectParameter("GENRE", typeof(string));
    
            var tHUMNAILParameter = tHUMNAIL != null ?
                new ObjectParameter("THUMNAIL", tHUMNAIL) :
                new ObjectParameter("THUMNAIL", typeof(string));
    
            var mONParameter = mON != null ?
                new ObjectParameter("MON", mON) :
                new ObjectParameter("MON", typeof(string));
    
            var tUEParameter = tUE != null ?
                new ObjectParameter("TUE", tUE) :
                new ObjectParameter("TUE", typeof(string));
    
            var wEDParameter = wED != null ?
                new ObjectParameter("WED", wED) :
                new ObjectParameter("WED", typeof(string));
    
            var tHUParameter = tHU != null ?
                new ObjectParameter("THU", tHU) :
                new ObjectParameter("THU", typeof(string));
    
            var fRIParameter = fRI != null ?
                new ObjectParameter("FRI", fRI) :
                new ObjectParameter("FRI", typeof(string));
    
            var sATParameter = sAT != null ?
                new ObjectParameter("SAT", sAT) :
                new ObjectParameter("SAT", typeof(string));
    
            var sUNParameter = sUN != null ?
                new ObjectParameter("SUN", sUN) :
                new ObjectParameter("SUN", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PRO_NOVEL_UPDATE", nOVELIDParameter, nOVELTITLEParameter, nOVELINFOParameter, wRITERParameter, gENREParameter, tHUMNAILParameter, mONParameter, tUEParameter, wEDParameter, tHUParameter, fRIParameter, sATParameter, sUNParameter);
        }
    
        public virtual int PRO_VOLUME_WRITE(string nOVELID, string vOLUMTITLE, string nOVELTEXT, string wRITERCOMMENT, string oPENDT)
        {
            var nOVELIDParameter = nOVELID != null ?
                new ObjectParameter("NOVELID", nOVELID) :
                new ObjectParameter("NOVELID", typeof(string));
    
            var vOLUMTITLEParameter = vOLUMTITLE != null ?
                new ObjectParameter("VOLUMTITLE", vOLUMTITLE) :
                new ObjectParameter("VOLUMTITLE", typeof(string));
    
            var nOVELTEXTParameter = nOVELTEXT != null ?
                new ObjectParameter("NOVELTEXT", nOVELTEXT) :
                new ObjectParameter("NOVELTEXT", typeof(string));
    
            var wRITERCOMMENTParameter = wRITERCOMMENT != null ?
                new ObjectParameter("WRITERCOMMENT", wRITERCOMMENT) :
                new ObjectParameter("WRITERCOMMENT", typeof(string));
    
            var oPENDTParameter = oPENDT != null ?
                new ObjectParameter("OPENDT", oPENDT) :
                new ObjectParameter("OPENDT", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PRO_VOLUME_WRITE", nOVELIDParameter, vOLUMTITLEParameter, nOVELTEXTParameter, wRITERCOMMENTParameter, oPENDTParameter);
        }
    
        public virtual int PRO_USER_RECENT_NOVEL(string uSERID, string nOVELID, Nullable<decimal> vOLUMENO, string nOVELKIND)
        {
            var uSERIDParameter = uSERID != null ?
                new ObjectParameter("USERID", uSERID) :
                new ObjectParameter("USERID", typeof(string));
    
            var nOVELIDParameter = nOVELID != null ?
                new ObjectParameter("NOVELID", nOVELID) :
                new ObjectParameter("NOVELID", typeof(string));
    
            var vOLUMENOParameter = vOLUMENO.HasValue ?
                new ObjectParameter("VOLUMENO", vOLUMENO) :
                new ObjectParameter("VOLUMENO", typeof(decimal));
    
            var nOVELKINDParameter = nOVELKIND != null ?
                new ObjectParameter("NOVELKIND", nOVELKIND) :
                new ObjectParameter("NOVELKIND", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PRO_USER_RECENT_NOVEL", uSERIDParameter, nOVELIDParameter, vOLUMENOParameter, nOVELKINDParameter);
        }
    
        public virtual int PRO_SERIAL_NOVEL_COMMENT_WRITE(string nOVELID, Nullable<decimal> vOLUMENO, string uSERCOMMENT, string uSERID)
        {
            var nOVELIDParameter = nOVELID != null ?
                new ObjectParameter("NOVELID", nOVELID) :
                new ObjectParameter("NOVELID", typeof(string));
    
            var vOLUMENOParameter = vOLUMENO.HasValue ?
                new ObjectParameter("VOLUMENO", vOLUMENO) :
                new ObjectParameter("VOLUMENO", typeof(decimal));
    
            var uSERCOMMENTParameter = uSERCOMMENT != null ?
                new ObjectParameter("USERCOMMENT", uSERCOMMENT) :
                new ObjectParameter("USERCOMMENT", typeof(string));
    
            var uSERIDParameter = uSERID != null ?
                new ObjectParameter("USERID", uSERID) :
                new ObjectParameter("USERID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PRO_SERIAL_NOVEL_COMMENT_WRITE", nOVELIDParameter, vOLUMENOParameter, uSERCOMMENTParameter, uSERIDParameter);
        }
    
        public virtual int PRO_VOLUME_UPDATE(string nOVELID, Nullable<decimal> vOLUMENO, string vOLUMTITLE, string nOVELTEXT, string wRITERCOMMENT, string oPENDT)
        {
            var nOVELIDParameter = nOVELID != null ?
                new ObjectParameter("NOVELID", nOVELID) :
                new ObjectParameter("NOVELID", typeof(string));
    
            var vOLUMENOParameter = vOLUMENO.HasValue ?
                new ObjectParameter("VOLUMENO", vOLUMENO) :
                new ObjectParameter("VOLUMENO", typeof(decimal));
    
            var vOLUMTITLEParameter = vOLUMTITLE != null ?
                new ObjectParameter("VOLUMTITLE", vOLUMTITLE) :
                new ObjectParameter("VOLUMTITLE", typeof(string));
    
            var nOVELTEXTParameter = nOVELTEXT != null ?
                new ObjectParameter("NOVELTEXT", nOVELTEXT) :
                new ObjectParameter("NOVELTEXT", typeof(string));
    
            var wRITERCOMMENTParameter = wRITERCOMMENT != null ?
                new ObjectParameter("WRITERCOMMENT", wRITERCOMMENT) :
                new ObjectParameter("WRITERCOMMENT", typeof(string));
    
            var oPENDTParameter = oPENDT != null ?
                new ObjectParameter("OPENDT", oPENDT) :
                new ObjectParameter("OPENDT", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PRO_VOLUME_UPDATE", nOVELIDParameter, vOLUMENOParameter, vOLUMTITLEParameter, nOVELTEXTParameter, wRITERCOMMENTParameter, oPENDTParameter);
        }
    
        public virtual int PRO_VIEWCNT_UPDATE(string nOVELID, Nullable<decimal> vOLUMENO)
        {
            var nOVELIDParameter = nOVELID != null ?
                new ObjectParameter("NOVELID", nOVELID) :
                new ObjectParameter("NOVELID", typeof(string));
    
            var vOLUMENOParameter = vOLUMENO.HasValue ?
                new ObjectParameter("VOLUMENO", vOLUMENO) :
                new ObjectParameter("VOLUMENO", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PRO_VIEWCNT_UPDATE", nOVELIDParameter, vOLUMENOParameter);
        }
    }
}
