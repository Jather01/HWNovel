using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HWNovel.ViewModels
{
    public class User
    {
        public string Userid { get; set; }          // 아이디
        public string Password { get; set; }        // 비밀번호
        public string Encpassword { get; set; }     // 암호화비밀번호
        public string Name { get; set; }            // 이름
        public string Birthday { get; set; }        // 생년월일
        public string Nickname { get; set; }        // 닉네임
        public string Power { get; set; }           // 권한
        public string Useyn { get; set; }           // 사용여부
        public DateTime Signupdate { get; set; }    // 회원가입 일자
        public string Novelid { get; set; }         // 소설 아이디
        public DateTime Date { get; set; }          // 관심작품 등록일
        public int Volumeno { get; set; }           // 회차번호
        public string CookyId { get; set; }         // 아이디 저장 여부

        public string PreUrl { get; set; }          // 로그인/로그아웃 후 돌아갈 페이지
        public string LoginError { get; set; }          // 로그인 에러 여부
    }
}